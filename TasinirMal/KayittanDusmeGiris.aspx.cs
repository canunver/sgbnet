using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Kayýttan düþme teklif ve onay tutanaðý bilgilerinin kayýt, listeleme, silme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class KayittanDusmeGiris : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama ve tutanakNo girdi dizgileri dolu ise
        ///     ilgili kayýttan düþme teklif ve onay tutanaðý bilgileri listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMKDG001;
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtTutanakTarihi.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
                ListeTemizle();
            }
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Geçici alýndý fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(Object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            KayittanDusmeForm kf = new KayittanDusmeForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kf.tutanakNo = txtBelgeNo.Text;
            kf.muhasebeKod = txtMuhasebe.Text.Trim();
            kf.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kf.ambarKod = txtAmbar.Text.Trim();
            kf.tutanakTarihi = new TNSDateTime(txtTutanakTarihi.RawText);

            int siraNo = 1;
            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                KayittanDusmeDetay detay = new KayittanDusmeDetay();

                detay.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
                detay.muhasebeKod = kf.muhasebeKod;
                detay.harcamaKod = kf.harcamaKod;
                detay.tutanakNo = kf.tutanakNo;
                detay.hesapPlanKod = TasinirGenel.DegerAlStr(item, "TASINIRHESAPKOD");
                detay.gorSicilNo = TasinirGenel.DegerAlStr(item, "SICILNO");
                detay.miktar = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "MIKTAR"));
                detay.kdvOran = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.DegerAlInt(item, "KDVORANI"), 0);
                detay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "BIRIMFIYATI"));

                detay.hesapPlanKod = detay.hesapPlanKod.Replace(".", "");

                if (!string.IsNullOrEmpty(detay.hesapPlanKod))
                {
                    detay.siraNo = siraNo++;
                    kf.detay.Add(detay);
                }
            }

            Sonuc sonuc = servisTMM.KayittanDusmeKaydet(kullanan, kf);

            if (sonuc.islemSonuc)
            {
                txtBelgeNo.Text = sonuc.anahtar;
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Listele resmine basýlýnca çalýþan olay metodu
        /// Sunucudan geçici alýndý fiþinin bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            KayittanDusmeForm kf = new KayittanDusmeForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kf.muhasebeKod = txtMuhasebe.Text.Trim();
            kf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kf.ambarKod = txtAmbar.Text.Trim();
            kf.tutanakTarihi = new TNSDateTime(txtTutanakTarihi.RawText);
            kf.tutanakNo = txtBelgeNo.Text;

            if (string.IsNullOrEmpty(kf.tutanakNo))
            {
                GenelIslemler.MesajKutusu("Uyarý", "Tutanak Numarasý boþ býrakýlamaz.");
                return;
            }

            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listelenecek kayýt bulunamadý." + bilgi.sonuc.hataStr);
                ListeTemizle();
                return;
            }

            KayittanDusmeForm kform = (KayittanDusmeForm)bilgi.objeler[0];
            txtTutanakTarihi.Text = kform.tutanakTarihi.ToString();

            List<object> liste = new List<object>();
            foreach (KayittanDusmeDetay td in kform.detay)
            {
                liste.Add(new
                {
                    TASINIRHESAPKOD = td.hesapPlanKod,
                    TASINIRHESAPADI = td.hesapPlanAd,
                    SICILNO = td.gorSicilNo,
                    MIKTAR = OrtakFonksiyonlar.ConvertToDbl(td.miktar),
                    OLCUBIRIMIKOD = td.olcuBirimAd,
                    KDVORANI = td.kdvOran,
                    BIRIMFIYATI = OrtakFonksiyonlar.ConvertToDbl(td.birimFiyat),//decimal olduðunda grid üzerinde son kýsýmda 0000 lar gözüküyor
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve
        /// toplanan kriterler Yazdir yordamýna gönderilir ve rapor hazýrlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnBelgeYazdir_Click(object sender, DirectEventArgs e)
        {
            KayittanDusmeForm kf = new KayittanDusmeForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kf.muhasebeKod = txtMuhasebe.Text.Trim();
            kf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kf.ambarKod = txtAmbar.Text.Trim();
            kf.tutanakNo = txtBelgeNo.Text;
            //kf.tutanakTarihi = new TNSDateTime(txtTutanakTarihi.RawText);

            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "KAYITTANDUSMETUTANAK.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            //Satir eklenince adreslerin yeri kayýyor
            //Bu nedenle imza bilgileri önce yazýlýyor
            ImzaEkle(XLS);

            KayittanDusmeForm kform = (KayittanDusmeForm)bilgi.objeler[0];
            XLS.HucreAdBulYaz("TutanakNo", kform.tutanakNo);
            XLS.HucreAdBulYaz("IlAd", kform.ilAd + "-" + kform.ilceAd);
            XLS.HucreAdBulYaz("IlKod", kform.ilKod + "-" + kform.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", kform.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", kform.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", kform.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", kform.muhasebeKod);

            satir = kaynakSatir;

            decimal miktarToplam = 0;

            for (int i = 0; i < kform.detay.Count; i++)
            {
                KayittanDusmeDetay kd = (KayittanDusmeDetay)kform.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 3);
                XLS.HucreBirlestir(satir, sutun + 5, satir, sutun + 6);
                XLS.HucreBirlestir(satir, sutun + 8, satir, sutun + 9);
                XLS.HucreBirlestir(satir, sutun + 10, satir, sutun + 12);

                XLS.HucreDegerYaz(satir, sutun, i + 1);
                if (string.IsNullOrEmpty(kd.gorSicilNo))
                    XLS.HucreDegerYaz(satir, sutun + 1, kd.hesapPlanKod);
                else
                    XLS.HucreDegerYaz(satir, sutun + 1, kd.gorSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 4, kd.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, kd.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(kd.miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(kd.birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(kd.tutar.ToString(), (double)0));

                miktarToplam += kd.miktar;
            }

            XLS.HucreDegerYaz(satir + 3, sutun + 1, string.Format(Resources.TasinirMal.FRMKDG002, kform.detay.Count.ToString(), miktarToplam.ToString()));

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            ListeTemizle();
            txtYil.Value = DateTime.Now.Year;
            txtTutanakTarihi.Clear();
            txtBelgeNo.Clear();
            lblFormDurum.Text = "";
            if (txtMuhasebe.Text == "") lblMuhasebeAd.Text = "";
            if (txtHarcamaBirimi.Text == "") lblHarcamaBirimiAd.Text = "";
            if (txtAmbar.Text == "") lblAmbarAd.Text = "";
        }

        private void ListeTemizle()
        {
            List<object> liste = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                liste.Add(new
                {
                    KOD = ""
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Kayýttan düþme teklif ve onay tutanaðý bilgilerini ekrandaki ilgili kontrollerden toplayan
        /// yordam çaðýrýlýr ve daha sonra toplanan bilgiler silinmek üzere Sil yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            KayittanDusmeForm kf = new KayittanDusmeForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kf.tutanakNo = txtBelgeNo.Text;
            kf.muhasebeKod = txtMuhasebe.Text.Trim();
            kf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kf.ambarKod = txtAmbar.Text.Trim();
            kf.tutanakTarihi = new TNSDateTime(txtTutanakTarihi.RawText);

            if (string.IsNullOrEmpty(kf.tutanakNo))
            {
                GenelIslemler.MesajKutusu("Uyarý", "Tutanak Numarasý boþ býrakýlamaz.");
                return;
            }

            Sonuc sonuc = servisTMM.KayittanDusmeSil(kullanan, kf);
            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
            else
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMKDG003);
                btnTemizle_Click(null, null);
            }
        }

        /// <summary>
        /// TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Kayýttan düþme teklif ve onay tutanaðýnýn bilgilerini sunucudan
        /// alýr, sessiona yazar ve taþýnýr iþlem fiþi ekranýna yönlendirir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIF_Click(object sender, DirectEventArgs e)
        {
            KayittanDusmeForm kf = new KayittanDusmeForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kf.muhasebeKod = txtMuhasebe.Text.Trim();
            kf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kf.ambarKod = txtAmbar.Text.Trim();
            kf.tutanakTarihi = new TNSDateTime(txtTutanakTarihi.RawText);
            kf.tutanakNo = txtBelgeNo.Text;

            if (string.IsNullOrEmpty(kf.tutanakNo))
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMKDG004);
                return;
            }

            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.bilgiStr);
                return;
            }

            TNS.TMM.TasinirIslemForm form = new TNS.TMM.TasinirIslemForm();

            foreach (KayittanDusmeForm kd in bilgi.objeler)
            {
                form.yil = DateTime.Now.Year;
                form.muhasebeKod = kd.muhasebeKod;
                form.harcamaKod = kd.harcamaKod;
                form.ambarKod = kd.ambarKod;
                form.fisTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
                form.nereyeGitti = ".";

                int siraNo = 1;
                foreach (KayittanDusmeDetay detay in kd.detay)
                {
                    if (form.islemTipKod == 0)
                    {
                        form.islemTipTur = (detay.hesapPlanKod.Substring(0, 3) == ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() ? (int)ENUMIslemTipi.KULLANILMAZCIKIS : (int)ENUMIslemTipi.HURDACIKIS);
                        form.islemTipKod = TasinirGenel.IslemTipiGetir(servisTMM, kullanan, form.islemTipTur, false);
                    }

                    TNS.TMM.TasinirIslemDetay tfd = new TasinirIslemDetay();
                    tfd.yil = form.yil;
                    tfd.muhasebeKod = form.muhasebeKod;
                    tfd.harcamaKod = form.harcamaKod;
                    tfd.ambarKod = form.ambarKod;

                    tfd.hesapPlanKod = detay.hesapPlanKod;
                    tfd.gorSicilNo = detay.gorSicilNo;
                    tfd.hesapPlanAd = detay.hesapPlanAd;
                    tfd.miktar = detay.miktar;
                    tfd.olcuBirimAd = detay.olcuBirimAd;
                    tfd.kdvOran = detay.kdvOran;
                    tfd.birimFiyat = detay.birimFiyat;
                    tfd.siraNo = siraNo++;

                    form.detay.objeler.Add(tfd);
                }
            }

            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, form);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Taþýnýr Ýþlem Fiþi baþarýyla oluþturuldu.<br><br>Belge Numarasý: <a href='../TasinirMal/TasinirislemFormAna.aspx?yil=" + form.yil + "&muhasebe=" + form.muhasebeKod + "&harcamaBirimi=" + form.harcamaKod + "&belgeNo=" + sonuc.anahtar.Split('-')[0] + "' target='_blank'>" + sonuc.anahtar.Split('-')[0] + "</a>");
            }
            else
            {
                GenelIslemler.MesajKutusu("Hata", "Taþýnýr Ýþlem Fiþi oluþturma sýrasýnda hata oluþtu.<br>Hata: " + sonuc.hataStr);
            }
        }

        /// <summary>
        /// Excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        private void ImzaEkle(Tablo XLS)
        {
            ObjectArray imzalar = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), 0);
            if (!imzalar.sonuc.islemSonuc)
                return;

            for (int i = 1; i <= 3; i++)
                XLS.HucreAdBulYaz("Tarih" + i.ToString(), DateTime.Today.Date.ToShortDateString());

            foreach (ImzaBilgisi imza in imzalar.objeler)
            {
                string adresSira = string.Empty;
                if (imza.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI)
                {
                    adresSira = "1";
                    XLS.HucreAdBulYaz("AdSoyad" + adresSira, imza.adSoyad);
                    XLS.HucreAdBulYaz("Unvan" + adresSira, imza.unvan);
                    adresSira = "3";
                }
                else if (imza.imzaYer == (int)ENUMImzaYer.KAYITTANDUSMEKOMISYONBASKANI)
                    adresSira = "2";
                else if (imza.imzaYer == (int)ENUMImzaYer.KAYITTANDUSMEKOMISYONUYESI)
                    adresSira = "4";
                else if (imza.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI)
                    adresSira = "5";
                else if (imza.imzaYer == (int)ENUMImzaYer.USTYONETICI)
                    adresSira = "6";

                XLS.HucreAdBulYaz("AdSoyad" + adresSira, imza.adSoyad);
                XLS.HucreAdBulYaz("Unvan" + adresSira, imza.unvan);
            }
        }
    }
}