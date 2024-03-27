using Ext1.Net;
using OrtakClass;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS;
using TNS.KYM;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr istek belgesi bilgilerinin kayýt, listeleme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIstekGiris : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama, ambar ve belgeNo girdi
        ///     dizgileri dolu ise ilgili taþýnýr istek belgesi bilgileri listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
                    formAdi = Resources.TasinirMal.FRMIGF001;
                else
                    formAdi = Resources.TasinirMal.FRMIGF042;

                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtBelgeTarihi.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                ListeTemizle();
                txtYil.Value = DateTime.Now.Year;
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

            IstekForm iForm = new IstekForm();
            iForm.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            iForm.muhasebeKod = txtMuhasebe.Text.Trim();
            iForm.harcamaKod = txtHarcamaBirimi.Text.Trim();
            iForm.ambarKod = txtAmbar.Text.Trim();
            iForm.belgeNo = txtBelgeNo.Text.Trim();
            iForm.belgeTarihi = new TNSDateTime(txtBelgeTarihi.RawText);
            iForm.istekYapanKod = txtPersonel.Text.Trim();
            iForm.hediyelik = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0);

            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                IstekDetay detay = new IstekDetay();
                detay.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
                detay.muhasebeKod = txtMuhasebe.Text.Trim();
                detay.harcamaKod = txtHarcamaBirimi.Text.Trim();
                detay.ambarKod = txtAmbar.Text.Trim();
                detay.belgeNo = txtBelgeNo.Text.Trim();

                if (TasinirGenel.DegerAlStr(item, "TASINIRHESAPKOD") == "")
                    continue;

                detay.hesapPlanKod = TasinirGenel.DegerAlStr(item, "TASINIRHESAPKOD");
                detay.istenilenMiktar = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "ISTENILENMIKTAR"));

                if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
                {
                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                        detay.karsilananMiktar = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "KARSILANANMIKTAR"));
                }
                else
                {
                    detay.kdvOran = TasinirGenel.DegerAlInt(item, "KDVORANI");
                    detay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "BIRIMFIYATI"));
                    detay.aciklama = TasinirGenel.DegerAlStr(item, "ACIKLAMA");
                }


                if (detay.hesapPlanKod.Replace(".", "").StartsWith("25506"))
                {
                    GenelIslemler.MesajKutusu("Uyarý", "255.06 ile baþlayan hesap kodu ile istek yapýlamaz. Lütfen düzeltip tekrar deneyin");
                    return;
                }


                iForm.detay.Add(detay);
            }

            Sonuc sonuc = servisTMM.IstekKaydet(kullanan, iForm);

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

            IstekForm iForm = new IstekForm();
            iForm.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            iForm.muhasebeKod = txtMuhasebe.Text.Trim();
            iForm.harcamaKod = txtHarcamaBirimi.Text.Trim();
            //iForm.ambarKod = txtAmbar.Text.Trim();
            iForm.belgeNo = txtBelgeNo.Text.Trim();
            //iForm.belgeTarihi = new TNSDateTime(txtBelgeTarihi.RawText);
            //iForm.istekYapanKod = txtPersonel.Text.Trim();
            iForm.hediyelik = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0);

            ObjectArray bilgi = servisTMM.IstekListele(kullanan, iForm, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }
            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            IstekForm istekForm = (IstekForm)bilgi.objeler[0];

            txtYil.Text = istekForm.yil.ToString();
            txtMuhasebe.Text = istekForm.muhasebeKod;
            lblMuhasebeAd.Text = istekForm.muhasebeAd;
            txtHarcamaBirimi.Text = istekForm.harcamaKod;
            lblHarcamaBirimiAd.Text = istekForm.harcamaAd;
            txtAmbar.Text = istekForm.ambarKod;
            lblAmbarAd.Text = istekForm.ambarAd;
            txtBelgeTarihi.Value = istekForm.belgeTarihi.ToString();
            txtBelgeNo.Value = istekForm.belgeNo;
            txtPersonel.Text = istekForm.istekYapanKod;
            lblPersonelAd.Text = istekForm.istekYapanAd;

            List<object> liste = new List<object>();
            foreach (IstekDetay td in istekForm.detay)
            {
                liste.Add(new
                {
                    TASINIRHESAPKOD = td.hesapPlanKod,
                    TASINIRHESAPADI = td.hesapPlanAd.ToString(),
                    ISTENILENMIKTAR = OrtakFonksiyonlar.ConvertToDbl(td.istenilenMiktar),
                    KARSILANANMIKTAR = OrtakFonksiyonlar.ConvertToDbl(td.karsilananMiktar),
                    OLCUBIRIMIKOD = td.olcuBirimAd,
                    KDVORANI = td.kdvOran,
                    BIRIMFIYATI = OrtakFonksiyonlar.ConvertToDbl(td.birimFiyat),
                    ACIKLAMA = td.aciklama.ToString()
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan
        /// kriterler Yazdir yordamýna gönderilir ve rapor hazýrlanmýþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            IstekForm iForm = new IstekForm();
            iForm.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            iForm.muhasebeKod = txtMuhasebe.Text.Trim();
            iForm.harcamaKod = txtHarcamaBirimi.Text.Trim();
            iForm.ambarKod = txtAmbar.Text.Trim();
            iForm.belgeNo = txtBelgeNo.Text.Trim();
            iForm.belgeTarihi = new TNSDateTime(txtBelgeTarihi.RawText);
            iForm.istekYapanKod = txtPersonel.Text.Trim();
            iForm.hediyelik = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0);

            ObjectArray bilgi = servisTMM.IstekListele(kullanan, iForm, true);
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

            if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
                Yazdir((IstekForm)bilgi.objeler[0]);
            else
                Yazdir_Hediyelik((IstekForm)bilgi.objeler[0]);
        }

        /// <summary>
        /// Parametre olarak verilen istek formuna ait kriterleri
        /// sunucudaki taþýnýr istek belgesi raporlama yordamýna gönderir,
        /// sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="istek">Taþýnýr istek belgesi kriter bilgilerini tutan nesne</param>
        private void Yazdir(IstekForm istek)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TASINIRISTEKBELGESI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("IstekYapan", istek.harcamaAd);
            XLS.HucreAdBulYaz("Tarih", istek.belgeTarihi.ToString());
            XLS.HucreAdBulYaz("BelgeNo", istek.belgeNo.PadLeft(6, '0'));

            satir = kaynakSatir;

            for (int i = 0; i < istek.detay.Count; i++)
            {
                IstekDetay id = (IstekDetay)istek.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                XLS.HucreDegerYaz(satir, sutun, i + 1);
                XLS.HucreDegerYaz(satir, sutun + 1, id.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, id.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, id.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(id.istenilenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(id.karsilananMiktar.ToString(), (double)0));

            }

            ImzaEkle(XLS, ref satir, sutun);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Taþýnýr istek belgesi excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        private void ImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 2;

            ObjectArray imza1 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.BIRIMYETKILISI);
            ImzaBilgisi i1 = null;
            if (imza1.sonuc.islemSonuc && imza1.objeler.Count > 0)
                i1 = (ImzaBilgisi)imza1.objeler[0];
            string ad1 = string.Empty;
            string unvan1 = string.Empty;
            if (i1 != null)
            {
                ad1 = i1.adSoyad;
                unvan1 = i1.unvan;
            }

            ObjectArray imza2 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.TASINIRKAYITYETKILISI);
            ImzaBilgisi i2 = null;
            if (imza2.sonuc.islemSonuc && imza2.objeler.Count > 0)
                i2 = (ImzaBilgisi)imza2.objeler[0];
            string ad2 = string.Empty;
            string unvan2 = string.Empty;
            if (i2 != null)
            {
                ad2 = i2.adSoyad;
                unvan2 = i2.unvan;
            }

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 5, sutun + 7, satir, sutun);

            for (int i = satir; i < satir + 5; i++)
            {
                XLS.HucreBirlestir(i, sutun, i, sutun + 3);
                XLS.HucreBirlestir(i, sutun + 4, i, sutun + 7);
            }
            XLS.HucreBirlestir(satir + 5, sutun, satir + 5, sutun + 7);

            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMIGF003);
            XLS.DuseyHizala(satir, sutun, 2);

            XLS.HucreDegerYaz(satir, sutun + 4, Resources.TasinirMal.FRMIGF004);
            XLS.DuseyHizala(satir, sutun + 4, 2);

            XLS.HucreDegerYaz(satir + 1, sutun, Resources.TasinirMal.FRMIGF005);
            XLS.DuseyHizala(satir + 1, sutun, 2);
            XLS.KoyuYap(satir + 1, sutun, true);

            XLS.HucreDegerYaz(satir + 1, sutun + 4, Resources.TasinirMal.FRMIGF006);
            XLS.DuseyHizala(satir + 1, sutun + 4, 2);
            XLS.KoyuYap(satir + 1, sutun + 4, true);

            XLS.HucreDegerYaz(satir + 2, sutun, string.Format(Resources.TasinirMal.FRMIGF007, ad1));
            XLS.HucreDegerYaz(satir + 2, sutun + 4, string.Format(Resources.TasinirMal.FRMIGF007, ad2));
            XLS.HucreDegerYaz(satir + 3, sutun, string.Format(Resources.TasinirMal.FRMIGF008, unvan1));
            XLS.HucreDegerYaz(satir + 3, sutun + 4, string.Format(Resources.TasinirMal.FRMIGF008, unvan2));
            XLS.HucreDegerYaz(satir + 4, sutun, Resources.TasinirMal.FRMIGF009);
            XLS.HucreDegerYaz(satir + 4, sutun + 4, Resources.TasinirMal.FRMIGF009);
            XLS.HucreDegerYaz(satir + 5, sutun, Resources.TasinirMal.FRMIGF010);

            XLS.YatayCizgiCiz(satir, sutun, sutun + 7, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 7, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 8, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 5;
        }

        private void Yazdir_Hediyelik(IstekForm istek)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "DogrudanVerme.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("BelgeNo", istek.belgeNo.PadLeft(6, '0'));
            XLS.HucreAdBulYaz("KimeVerildi", istek.istekYapanAd);
            XLS.HucreAdBulYaz("BelgeTarih", istek.belgeTarihi.ToString());
            XLS.HucreAdBulYaz("HarcamaAd", istek.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", istek.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", istek.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", istek.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", istek.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", istek.muhasebeKod);

            ImzaEkle_Hediyelik(XLS);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < istek.detay.Count; i++)
            {
                IstekDetay id = (IstekDetay)istek.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 9, satir, sutun + 11);

                XLS.HucreDegerYaz(satir, sutun, i + 1);
                XLS.HucreDegerYaz(satir, sutun + 1, id.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, id.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, id.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(id.istenilenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(id.birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble((id.istenilenMiktar * id.birimFiyat * ((decimal)id.kdvOran / 100 + 1)).ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 9, id.aciklama);
            }

            if (satir - kaynakSatir % 56 > 45)
                XLS.SayfaSonuKoyHucresel(satir);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        private void ImzaEkle_Hediyelik(Tablo XLS)
        {
            ObjectArray imzalar = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), 0);
            if (!imzalar.sonuc.islemSonuc || imzalar.objeler.Count <= 0)
                return;

            Hashtable ht = new Hashtable();
            ht.Add((int)ENUMImzaYer.TASINIRKAYITYETKILISI, new string[2] { "AdSoyadTKKY", "UnvanTKKY" });
            ht.Add((int)ENUMImzaYer.HARCAMAYETKILISI, new string[2] { "AdSoyadHY", "UnvanHY" });

            foreach (ImzaBilgisi imza in imzalar.objeler)
            {
                string[] imzaAdres = (string[])ht[imza.imzaYer];
                if (imzaAdres == null)
                    continue;

                XLS.HucreAdBulYaz(imzaAdres[0], imza.adSoyad);
                XLS.HucreAdBulYaz(imzaAdres[1], imza.unvan);
            }
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
            txtBelgeTarihi.Value = DateTime.Now.Date;
            txtBelgeNo.Clear();
            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            txtPersonel.Clear();
            lblPersonelAd.Text = "";
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

        protected void HesapStore_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Parameters["query"]))
                return;

            List<object> liste = HesapListesiDoldur(e.Parameters["query"]);

            e.Total = 0;
            if (liste != null && liste.Count != 0)
            {
                var limit = e.Limit;
                if ((e.Start + e.Limit) > liste.Count)
                    limit = liste.Count - e.Start;

                e.Total = liste.Count;
                List<object> rangeData = (e.Start < 0 || limit < 0) ? liste : liste.GetRange(e.Start, limit);
                strHesapPlan.DataSource = (object[])rangeData.ToArray();
                strHesapPlan.DataBind();
            }
            else
            {
                strHesapPlan.DataSource = new object[] { };
                strHesapPlan.DataBind();
            }
        }

        List<object> HesapListesiDoldur(string kriter)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.hesapKodAciklama = kriter;
            h.detay = true;
            ObjectArray hesap = servisTMM.HesapPlaniListele(kullanan, h, new Sayfalama());

            List<object> liste = new List<object>();
            foreach (HesapPlaniSatir detay in hesap.objeler)
            {
                liste.Add(new
                {
                    KOD = detay.hesapKod + " - " + detay.aciklama + " - " + detay.olcuBirimAd + " - " + detay.kdv
                });
            }
            return liste;
        }
    }
}