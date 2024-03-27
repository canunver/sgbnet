using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Harcama birimlerinin kesmiþ olduklarý son taþýnýr iþlem fiþlerinin numaralarýnýn raporlama iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class HarcamaSonTIFEski : istemciUzayi.GenelSayfa
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
            formAdi = Resources.TasinirMal.FRMHST001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            if (!IsPostBack)
            {
                YilDoldur();
            }
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
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
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            TNSDateTime tarihIlk = new TNSDateTime(txtTarihIlk.Value);
            TNSDateTime tarihSon = new TNSDateTime(txtTarihSon.Value);

            ObjectArray bilgi = servisTMM.HarcamaBirimiSonTIFListele(kullanan, yil, tarihIlk, tarihSon);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
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