using System;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.MUH;
using Ext1.Net;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Muhasebe birimi listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeTertip : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        IMUHServis servis = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Muhasebe birimleri cevap (response) olarak döndürülür.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";

                ListeDoldur();
            }
        }

        /// <summary>
        /// Parametre olarak verilen muhasebe birimi kriterine uyan muhasebe birimlerini sayfadaki GridView kontrolüne dolduran yordam
        /// </summary>
        private void ListeDoldur()
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);
            if (yil == 0) yil = DateTime.Now.Year;

            HesapPlaniSatir kriter = new HesapPlaniSatir();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
            kriter.nitelik = ((int)ENUMHesapKodNitelik.KASAHESABI).ToString();
            kriter.seviye = -1;
            ObjectArray bilgi = servis.HesapPlaniListele(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLMH002);

            List<object> liste = new List<object>();
            foreach (HesapPlaniSatir item in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = item.hesapKod,
                    ADI = item.aciklama,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }
    }
}