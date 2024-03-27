using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Kay�ttan d��me teklif ve onay tutana�� bilgilerinin kay�t, listeleme, silme ve raporlama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class KayittanDusmeGiris : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama ve tutanakNo girdi dizgileri dolu ise
        ///     ilgili kay�ttan d��me teklif ve onay tutana�� bilgileri listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMKDG001;
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
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
        /// Kaydet tu�una bas�l�nca �al��an olay metodu
        /// Ge�ici al�nd� fi�i bilgileri ekrandaki ilgili kontrollerden toplan�p kaydedilmek
        /// �zere sunucuya g�nderilir, gelen sonuca g�re hata mesaj� veya bilgi mesaj� verilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyar�", sonuc.hataStr);
        }

        /// <summary>
        /// Listele resmine bas�l�nca �al��an olay metodu
        /// Sunucudan ge�ici al�nd� fi�inin bilgileri al�n�r ve listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyar�", "Tutanak Numaras� bo� b�rak�lamaz.");
                return;
            }

            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyar�", "Listelenecek kay�t bulunamad�." + bilgi.sonuc.hataStr);
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
                    BIRIMFIYATI = OrtakFonksiyonlar.ConvertToDbl(td.birimFiyat),//decimal oldu�unda grid �zerinde son k�s�mda 0000 lar g�z�k�yor
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam �a��r�l�r ve
        /// toplanan kriterler Yazdir yordam�na g�nderilir ve rapor haz�rlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.bilgiStr);
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

            //Satir eklenince adreslerin yeri kay�yor
            //Bu nedenle imza bilgileri �nce yaz�l�yor
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
        /// Temizle tu�una bas�l�nca �al��an olay metodu
        /// Kullan�c� taraf�ndan sayfadaki kontrollere yaz�lm�� bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
        /// Sil tu�una bas�l�nca �al��an olay metodu
        /// Kay�ttan d��me teklif ve onay tutana�� bilgilerini ekrandaki ilgili kontrollerden toplayan
        /// yordam �a��r�l�r ve daha sonra toplanan bilgiler silinmek �zere Sil yordam�na g�nderilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyar�", "Tutanak Numaras� bo� b�rak�lamaz.");
                return;
            }

            Sonuc sonuc = servisTMM.KayittanDusmeSil(kullanan, kf);
            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyar�", sonuc.hataStr);
            else
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMKDG003);
                btnTemizle_Click(null, null);
            }
        }

        /// <summary>
        /// T�F Olu�tur tu�una bas�l�nca �al��an olay metodu
        /// Kay�ttan d��me teklif ve onay tutana��n�n bilgilerini sunucudan
        /// al�r, sessiona yazar ve ta��n�r i�lem fi�i ekran�na y�nlendirir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMKDG004);
                return;
            }

            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.bilgiStr);
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
                GenelIslemler.MesajKutusu("Bilgi", "Ta��n�r ��lem Fi�i ba�ar�yla olu�turuldu.<br><br>Belge Numaras�: <a href='../TasinirMal/TasinirislemFormAna.aspx?yil=" + form.yil + "&muhasebe=" + form.muhasebeKod + "&harcamaBirimi=" + form.harcamaKod + "&belgeNo=" + sonuc.anahtar.Split('-')[0] + "' target='_blank'>" + sonuc.anahtar.Split('-')[0] + "</a>");
            }
            else
            {
                GenelIslemler.MesajKutusu("Hata", "Ta��n�r ��lem Fi�i olu�turma s�ras�nda hata olu�tu.<br>Hata: " + sonuc.hataStr);
            }
        }

        /// <summary>
        /// Excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
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