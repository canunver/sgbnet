using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class TasinirIstekAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = Resources.TasinirMal.FRMIGF000;
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "TasinirIstekGiris.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "TasinirIstekListe.aspx?menuYok=1";
        }
    }
}