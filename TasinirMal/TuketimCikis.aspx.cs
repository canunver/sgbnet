using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr tüketim malzemelerinin çýkýþ ve fark bilgilerinin raporlama iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class TuketimCikis : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ile ilgili ayarlamalar yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["fark"]) && Request.QueryString["fark"] == "1")
                    formAdi = Resources.TasinirMal.FRMTMC001;
                else
                    formAdi = Resources.TasinirMal.FRMTMC002;

                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                if (TNS.TMM.Arac.MeclisKullaniyor())//Meclis ambar gösterimesini istemiyor
                {
                    txtAmbar.Text = "";
                    cmpAmbar.Hide();
                }

                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler
        /// sayfa adresinde gelen fark girdi dizgisine bakýlarak TuketimCikisFarkYazdir
        /// yordamýna veya TuketimCikisYazdir yordamýna gönderilir ve rapor hazýrlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["fark"]) && Request.QueryString["fark"] == "1")
                TuketimCikisFarkYazdir(KriterTopla());
            else
                TuketimCikisYazdir(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Tüketim çýkýþ kriter bilgileri döndürülür.</returns>
        private TNS.TMM.TuketimCikis KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TuketimCikis kriter = new TNS.TMM.TuketimCikis();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.tarih1 = new TNSDateTime(txtTarih1.RawText);
            kriter.tarih2 = new TNSDateTime(txtTarih2.RawText);
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen tüketim çýkýþ kriterlerini sunucudaki TuketimCikis
        /// yordamýna gönderir, sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="kriter">Tüketim çýkýþ kriter bilgilerini tutan nesne</param>
        private void TuketimCikisYazdir(TNS.TMM.TuketimCikis kriter)
        {
            ObjectArray bilgi = servisTMM.TuketimCikis(kullanan, kriter, false);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TuketimCikis.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.TuketimCikis tk = (TNS.TMM.TuketimCikis)bilgi.objeler[0];
            XLS.HucreAdBulYaz("IlAd", tk.ilAd + "-" + tk.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tk.ilKod + "-" + tk.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tk.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tk.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tk.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tk.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tk.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tk.muhasebeKod);

            decimal toplamTutar = 0;
            foreach (TNS.TMM.TuketimCikisDetay detay in tk.detay)
                toplamTutar += (decimal)OrtakFonksiyonlar.YuvarlaYukari(OrtakFonksiyonlar.ConvertToDbl(detay.tutar[0]), 2);

            string pBirimi = TasinirGenel.ParaBirimiDondur();

            string aciklama = string.Empty;
            if (!(kriter.tarih1.isNull && kriter.tarih2.isNull))
                aciklama = string.Format(Resources.TasinirMal.FRMTMC003, kriter.tarih1.ToString(), kriter.tarih2.ToString(), toplamTutar.ToString("#,###.00"), pBirimi);
            else
                aciklama = string.Format(Resources.TasinirMal.FRMTMC004, toplamTutar.ToString("#,###.00"), pBirimi);

            if (!chkDetay.Checked)
            {
                XLS.HucreDegerYaz(satir - 1, sutun, Resources.TasinirMal.FRMTMC005);
                XLS.HucreDegerYaz(satir - 1, sutun + 2, Resources.TasinirMal.FRMTMC006);
                XLS.HucreDegerYaz(satir - 1, sutun + 6, Resources.TasinirMal.FRMTMC007);

                XLS.HucreBirlestir(satir - 1, sutun, satir - 1, sutun + 1);
                XLS.HucreBirlestirme(satir - 1, sutun + 2, satir - 1, sutun + 4);
                XLS.HucreBirlestir(satir - 1, sutun + 2, satir - 1, sutun + 5);
                XLS.HucreBirlestir(satir - 1, sutun + 6, satir - 1, sutun + 7);
            }

            ImzaEkle(XLS, aciklama);

            decimal toplam2Duzey = 0;
            string eskiHesap = string.Empty;
            string eskiHesapAd = string.Empty;
            int siraNo = 0;
            for (int i = 0; i < tk.detay.Count; i++)
            {
                TNS.TMM.TuketimCikisDetay detay = (TNS.TMM.TuketimCikisDetay)tk.detay[i];

                if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod)
                {
                    if (chkDetay.Checked)
                        ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam2Duzey);
                    else
                        ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam2Duzey);

                    toplam2Duzey = 0;
                }

                if (chkDetay.Checked)
                {
                    siraNo++;
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, detay.olcuBirimAd);

                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.miktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[0].ToString("#,###.00"), (double)0));
                }

                toplam2Duzey += (decimal)OrtakFonksiyonlar.YuvarlaYukari(OrtakFonksiyonlar.ConvertToDbl(detay.tutar[0]), 2);
                eskiHesap = detay.hesapPlanKod;
                eskiHesapAd = detay.hesapPlanAd;
            }

            if (chkDetay.Checked)
                ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam2Duzey);
            else
                ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam2Duzey);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Tüketim malzemeleri çýkýþ excel raporuna toplam tutar bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun baþladýðý kaynak satýr numarasý</param>
        /// <param name="satir">Toplam bilgilerinin yazýlacaðý satýr numarasý</param>
        /// <param name="sutun">Toplam bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        /// <param name="hesapKod">Toplam bilgilerinin ait olduðu taþýnýr hesap planý kodu</param>
        /// <param name="hesapAd">Toplam bilgilerinin ait olduðu taþýnýr hesap planý adý</param>
        /// <param name="toplam">Taþýnýr hesap planýna ait toplam tutarý</param>
        private void ToplamYaz(Tablo XLS, int kaynakSatir, ref int satir, int sutun, string hesapKod, string hesapAd, decimal toplam)
        {
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);

            if (chkDetay.Checked)
            {
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 6);
                XLS.DuseyHizala(satir, sutun, 1);
                XLS.KoyuYap(satir, sutun, true);
                XLS.HucreDegerYaz(satir, sutun, hesapKod);
                XLS.KoyuYap(satir, sutun + 7, satir, sutun + 7, true);
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(toplam.ToString("#,###.00"), (double)0));
            }
            else
            {
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 5);
                XLS.HucreBirlestir(satir, sutun + 6, satir, sutun + 7);
                XLS.HucreDegerYaz(satir, sutun, hesapKod);
                XLS.HucreDegerYaz(satir, sutun + 2, hesapAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(toplam.ToString("#,###.00"), (double)0));
            }
        }

        /// <summary>
        /// Tüketim malzemeleri çýkýþ ve fark excel raporlarýna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="aciklama">Rapora eklenecek açýklama bilgisi</param>
        private void ImzaEkle(Tablo XLS, string aciklama)
        {
            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), 0);

            string[] ad = new string[3];
            string[] unvan = new string[3];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI && string.IsNullOrEmpty(ad[1]))
                    {
                        ad[1] = iBilgi.adSoyad;
                        unvan[1] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKONTROLYETKILISI && string.IsNullOrEmpty(ad[2]))
                    {
                        ad[2] = iBilgi.adSoyad;
                        unvan[2] = iBilgi.unvan;
                    }
                }
            }

            if (!string.IsNullOrEmpty(aciklama))
                XLS.HucreAdBulYaz("Aciklama", aciklama);

            if (!string.IsNullOrEmpty(ad[0]))
                XLS.HucreAdBulYaz("AdSoyad1", ad[0]);

            if (!string.IsNullOrEmpty(unvan[0]))
                XLS.HucreAdBulYaz("Unvan1", unvan[0]);

            XLS.HucreAdBulYaz("Tarih1", DateTime.Today.Date.ToShortDateString());

            if (!string.IsNullOrEmpty(ad[1]))
                XLS.HucreAdBulYaz("AdSoyad2", ad[1]);

            if (!string.IsNullOrEmpty(unvan[1]))
                XLS.HucreAdBulYaz("Unvan2", unvan[1]);

            XLS.HucreAdBulYaz("Tarih2", DateTime.Today.Date.ToShortDateString());

            if (!string.IsNullOrEmpty(ad[2]))
                XLS.HucreAdBulYaz("AdSoyad3", ad[2]);

            if (!string.IsNullOrEmpty(unvan[2]))
                XLS.HucreAdBulYaz("Unvan3", unvan[2]);

            XLS.HucreAdBulYaz("Tarih3", DateTime.Today.Date.ToShortDateString());
        }

        /// <summary>
        /// Parametre olarak verilen tüketim çýkýþ kriterlerini sunucudaki TuketimCikis
        /// yordamýna gönderir, sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="kriter">Tüketim çýkýþ kriter bilgilerini tutan nesne</param>
        private void TuketimCikisFarkYazdir(TNS.TMM.TuketimCikis kriter)
        {
            ObjectArray bilgi = servisTMM.TuketimCikis(kullanan, kriter, true);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "KurusFarkRaporuTuketim.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.TuketimCikis tk = (TNS.TMM.TuketimCikis)bilgi.objeler[0];
            XLS.HucreAdBulYaz("IlAd", tk.ilAd + "-" + tk.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tk.ilKod + "-" + tk.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tk.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tk.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tk.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tk.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tk.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tk.muhasebeKod);

            string aciklama = string.Empty;
            if (!(kriter.tarih1.isNull && kriter.tarih2.isNull))
                aciklama = string.Format(Resources.TasinirMal.FRMTMC008, kriter.tarih1.ToString(), kriter.tarih2.ToString());
            else
                aciklama = Resources.TasinirMal.FRMTMC009;

            ImzaEkle(XLS, aciklama);

            if (!chkDetay.Checked)
            {
                XLS.HucreDegerYaz(satir - 1, sutun, Resources.TasinirMal.FRMTMC005);
                XLS.HucreBirlestir(satir - 1, sutun, satir - 1, sutun + 1);
                XLS.SutunGizle(sutun + 5, sutun + 6, true);
            }

            decimal[] toplam = new decimal[4];
            string eskiHesap = string.Empty;
            string eskiHesapAd = string.Empty;
            int siraNo = 0;
            for (int i = 0; i < tk.detay.Count; i++)
            {
                TNS.TMM.TuketimCikisDetay detay = (TNS.TMM.TuketimCikisDetay)tk.detay[i];

                if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod)
                {
                    if (chkDetay.Checked)
                        FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam);
                    else
                        FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam);

                    for (int k = 0; k < toplam.Length; k++)
                        toplam[k] = 0;
                }

                if (chkDetay.Checked)
                {
                    siraNo++;
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, detay.olcuBirimAd);

                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.miktar.ToString(), (double)0));

                    decimal fark = 0;
                    for (int k = 0; k < detay.tutar.Length; k++)
                    {
                        fark += OrtakFonksiyonlar.ConvertToDecimal(detay.tutar[k].ToString("#,###.00"), (decimal)0) - detay.tutar[k];
                        XLS.HucreDegerYaz(satir, sutun + 7 + k, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[k].ToString("#,###.00"), (double)0));
                    }
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(fark.ToString(), (double)0));
                }

                for (int k = 0; k < toplam.Length; k++)
                    toplam[k] += detay.tutar[k];

                eskiHesap = detay.hesapPlanKod;
                eskiHesapAd = detay.hesapPlanAd;
            }

            if (chkDetay.Checked)
                FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam);
            else
                FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Tüketim malzemeleri fark excel raporuna toplam tutar bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun baþladýðý kaynak satýr numarasý</param>
        /// <param name="satir">Toplam bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Toplam bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        /// <param name="hesapKod">Toplam bilgilerinin ait olduðu taþýnýr hesap planý kodu</param>
        /// <param name="hesapAd">Toplam bilgilerinin ait olduðu taþýnýr hesap planý adý</param>
        /// <param name="toplam">Tüketim malzemeleri farklarýna ait toplam bilgilerini tutan dizi</param>
        private void FarkToplamYaz(Tablo XLS, int kaynakSatir, ref int satir, int sutun, string hesapKod, string hesapAd, decimal[] toplam)
        {
            decimal fark = 0;
            for (int i = 0; i < toplam.Length; i++)
                fark += OrtakFonksiyonlar.ConvertToDecimal(toplam[i].ToString("#,###.00"), (decimal)0) - toplam[i];
            decimal fark2Duzey = OrtakFonksiyonlar.ConvertToDecimal(fark.ToString("#,###.00"), (decimal)0);

            if (((fark2Duzey >= (decimal)0.01 || fark2Duzey <= (decimal)-0.01) && !chkDetay.Checked) || chkDetay.Checked)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);

                if (chkDetay.Checked)
                {
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 6);
                    XLS.DuseyHizala(satir, sutun, 1);
                    XLS.KoyuYap(satir, sutun, true);
                    XLS.HucreDegerYaz(satir, sutun, hesapKod);
                    XLS.KoyuYap(satir, sutun + 7, satir, sutun + 11, true);

                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(fark.ToString(), (double)0));
                }
                else
                {
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);
                    XLS.HucreDegerYaz(satir, sutun, hesapKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, hesapAd);

                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(fark2Duzey.ToString(), (double)0));
                }

                for (int i = 0; i < toplam.Length; i++)
                    XLS.HucreDegerYaz(satir, sutun + 7 + i, OrtakFonksiyonlar.ConvertToDouble(toplam[i].ToString("#,###.00"), (double)0));
            }
        }
    }
}