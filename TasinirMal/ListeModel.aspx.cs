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
    /// Model listesinin verilen kritere g�re d�nd�r�l�p listelendi�i sayfa
    /// </summary>
    public partial class ListeModel : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur, yoksa giri� ekran�na y�nlendirilir varsa sayfa
        ///     adresinde gelen marka girdi dizgisine ait modeller cevap (response) olarak d�nd�r�l�r.
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

                Model m = new Model();
                m.markaKodu = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["marka"] + "", 0);

                ModelDoldur(m);
            }
        }

        /// <summary>
        /// Parametre olarak verilen model kriterine uyan modelleri sayfadaki GridView kontrol�ne dolduran yordam
        /// </summary>
        /// <param name="model">Model kriter bilgilerini tutan nesne</param>
        private void ModelDoldur(Model model)
        {
            ObjectArray bilgi = servisTMM.ModelListele(kullanan, model);

            List<object> liste = new List<object>();
            if (!bilgi.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLMA002);

            foreach (TNS.TMM.Model item in bilgi.objeler)
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