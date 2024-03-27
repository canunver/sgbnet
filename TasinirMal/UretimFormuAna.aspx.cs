using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class UretimFormuAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = "Üretim Formu";
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "UretimFormuForm.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "UretimFormuSorgu.aspx?menuYok=1";
        }
    }
}