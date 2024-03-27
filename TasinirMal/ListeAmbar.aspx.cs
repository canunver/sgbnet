using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Ambar listesinin verilen kritere g�re d�nd�r�l�p listelendi�i sayfa
    /// </summary>
    public partial class ListeAmbar : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Sayfa adresinde gelen mb ve hb girdi dizgileri kullan�larak
        ///     ambarlar cevap (response) olarak d�nd�r�l�r.
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

                Ambar amb = new Ambar();
                amb.muhasebeKod = Request.QueryString["mb"] + "";
                amb.harcamaBirimKod = Request.QueryString["hb"] + "";
                amb.kapali = 0;

                AmbarDoldur(amb);
            }
        }

        /// <summary>
        /// Parametre olarak verilen ambar kriterine uyan ambarlar� sayfadaki GridView kontrol�ne dolduran yordam
        /// </summary>
        /// <param name="ambar">Ambar kriter bilgilerini tutan nesne</param>
        private void AmbarDoldur(Ambar ambar)
        {
            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, ambar);

            List<object> liste = new List<object>();
            if (!bilgi.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLAM001);

            bool yetki = false;
            bool butun = OrtakFonksiyonlar.ConvertToInt(htnButun.Value, 0) > 0;

            foreach (Ambar item in bilgi.objeler)
            {
                if (!butun)
                {
                    yetki = servisTMM.GormeYetkisiVarMi(kullanan, item.muhasebeKod, item.harcamaBirimKod, item.kod);
                    if (!yetki) continue;
                }

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