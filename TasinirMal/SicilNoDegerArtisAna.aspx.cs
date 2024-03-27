using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class SicilNoDegerArtisAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = "Sicil No Deðer Artýþý";
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            if (Request.QueryString["menuYok"] == "1")
            {
                tabPanelAna.Margins = "0 0 0 0";
                tabPanelAna.StyleSpec += "padding:5px";
            }

            panelIslem.AutoLoad.Url = "SicilNoDegerArtisForm.aspx?menuYok=1&prSicilNo=" + Request.QueryString["prSicilNo"];
            panelSorgu.AutoLoad.Url = "SicilNoDegerArtisSorgu.aspx?menuYok=1&prSicilNo=" + Request.QueryString["prSicilNo"];
        }
    }
}