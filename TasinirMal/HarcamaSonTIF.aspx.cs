using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Harcama birimlerinin kesmiþ olduklarý son taþýnýr iþlem fiþlerinin numaralarýnýn raporlama iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class HarcamaSonTIF : TMMSayfaV2
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
                formAdi = Resources.TasinirMal.FRMHST001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan harcama birimlerinin kestikleri son taþýnýr iþlem fiþlerine
        /// ait bilgiler alýnýr ve excel dosyasýna yazýlýp kullanýcýya gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            TNSDateTime tarihIlk = new TNSDateTime(txtTarih1.RawText);
            TNSDateTime tarihSon = new TNSDateTime(txtTarih2.RawText);

            ObjectArray bilgi = servisTMM.HarcamaBirimiSonTIFListele(kullanan, yil, tarihIlk, tarihSon);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
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
            string sablonAd = "HarcamaSonTIF.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                TNS.TMM.HarcamaSonTIF hst = (TNS.TMM.HarcamaSonTIF)bilgi.objeler[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 5, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, hst.ilAd + "-" + hst.ilceAd);
                XLS.HucreDegerYaz(satir, sutun + 1, hst.muhasebeKod);
                XLS.HucreDegerYaz(satir, sutun + 2, hst.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 3, hst.harcamaKod);
                XLS.HucreDegerYaz(satir, sutun + 4, hst.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 5, hst.tifNo);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}