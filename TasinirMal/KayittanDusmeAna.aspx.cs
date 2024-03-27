using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class KayittanDusmeAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = Resources.TasinirMal.FRMKDG000;
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "KayittanDusmeGiris.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "KayittanDusmeListe.aspx?menuYok=1";
        }
    }
}