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
    /// Muhasebe birimi listesinin verilen kritere g�re d�nd�r�l�p listelendi�i sayfa
    /// </summary>
    public partial class ListeMuhasebe : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Muhasebe birimleri cevap (response) olarak d�nd�r�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";
                htnButun.Value = Request.QueryString["butun"] + "";

                MuhasebeBirimiDoldur();
            }
        }

        /// <summary>
        /// Parametre olarak verilen muhasebe birimi kriterine uyan muhasebe birimlerini sayfadaki GridView kontrol�ne dolduran yordam
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