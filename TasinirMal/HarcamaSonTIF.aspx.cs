using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Harcama birimlerinin kesmi� olduklar� son ta��n�r i�lem fi�lerinin numaralar�n�n raporlama i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class HarcamaSonTIF : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ile ilgili ayarlamalar yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMHST001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan harcama birimlerinin kestikleri son ta��n�r i�lem fi�lerine
        /// ait bilgiler al�n�r ve excel dosyas�na yaz�l�p kullan�c�ya g�nderilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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