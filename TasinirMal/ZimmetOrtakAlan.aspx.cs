using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Ortak alana verilmiþ zimmetli demirbaþ bilgilerinin raporlama iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetOrtakAlan : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMZOA001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler
        /// ortak alana verilmiþ zimmetli demirbaþlar raporunu üreten ZimmetOrtakAlanYazdir yordamýna
        /// gönderilir, böylece excel raporu üretilip kullanýcýya gönderilmiþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            ZimmetOrtakAlanYazdir(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden zimmet listeleme kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Zimmet listeleme kriter bilgileri döndürülür.</returns>
        private TNS.TMM.ZimmetOrtakAlanVeKisi KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new TNS.TMM.ZimmetOrtakAlanVeKisi();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.odaKod = txtNereyeVerildi.Text.Trim();
            if (chkKisiDahilEt.Checked)
                kriter.kisiDahilEt = 1;

            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetOrtakAlan yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetOrtakAlanYazdir(TNS.TMM.ZimmetOrtakAlanVeKisi kriter)
        {
            ObjectArray bilgi = servisTMM.ZimmetOrtakAlan(kullanan, kriter);

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
            string sablonAd = "ZimmetOrtakAlan.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            XLS.HucreAdBulYaz("HarcamaAd", zoa.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", zoa.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", zoa.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", zoa.muhasebeKod);
            //XLS.HucreAdBulYaz("OdaAd", zoa.odaAd);
            //XLS.HucreAdBulYaz("OdaKod", zoa.odaKod);
            XLS.HucreAdBulYaz("OdaAd", lblNereyeVerildi.Text.Trim());
            XLS.HucreAdBulYaz("OdaKod", txtNereyeVerildi.Text.Trim());

            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 8);

                XLS.HucreDegerYaz(satir, sutun, detay.harcamaKod + " - " + detay.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 1, detay.ambarKod + " - " + detay.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilAd);
                XLS.HucreDegerYaz(satir, sutun + 5, detay.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 6, detay.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 7, detay.sorumluKisiAd);
                XLS.HucreDegerYaz(satir, sutun + 9, detay.aciklama);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}