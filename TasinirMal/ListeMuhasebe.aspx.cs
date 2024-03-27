using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Muhasebe birimi listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeMuhasebe : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

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
                htnButun.Value = Request.QueryString["butun"] + "";

                MuhasebeBirimiDoldur();
            }
        }

        /// <summary>
        /// Parametre olarak verilen muhasebe birimi kriterine uyan muhasebe birimlerini sayfadaki GridView kontrolüne dolduran yordam
        /// </summary>
        private void MuhasebeBirimiDoldur()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["butunListe"]))
                kullanan = null;

            TNS.TMM.Muhasebe muh = new Muhasebe();
            muh.kapali = 0;
            ObjectArray bilgi = servisTMM.MuhasebeListele(kullanan, muh);

            if (!bilgi.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLMH002);

            bool butun = OrtakFonksiyonlar.ConvertToInt(htnButun.Value, 0) > 0;
            List<object> liste = new List<object>();
            foreach (TNS.TMM.Muhasebe item in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    ADI = item.ad,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }
    }
}