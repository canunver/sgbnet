using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class SayimAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = Resources.TasinirMal.FRMSYG000;
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giri� izni varm�?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "SayimGiris.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "SayimListe.aspx?menuYok=1";
        }
    }
}