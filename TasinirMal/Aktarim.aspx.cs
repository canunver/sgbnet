using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class Aktarim : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = "Toplu Fiş Üretim Formu";
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "AktarimForm.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "AktarimSorgu.aspx?menuYok=1";
        }
    }
}