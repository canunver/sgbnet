using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class UretimRecetesiAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = "Üretim Reçetesi";
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "UretimRecetesiForm.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "UretimRecetesiSorgu.aspx?menuYok=1";
        }
    }
}