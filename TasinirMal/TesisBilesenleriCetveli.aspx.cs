using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class TesisBilesenleriCetveli : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = Resources.TasinirMal.FRMTBC001;
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "TesisBilesenleriCetveliForm.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "TesisBilesenleriCetveliSorgu.aspx?menuYok=1";
        }
    }
}