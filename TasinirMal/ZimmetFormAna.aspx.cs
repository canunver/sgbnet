using OrtakClass;
using System;

namespace TasinirMal
{
    public partial class ZimmetFormAna : TMMSayfaV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string belgeTuru = Request.QueryString["tur"] + "";
            if (belgeTuru == "10")
                formAdi = Resources.TasinirMal.FRMZFG002;
            else if (belgeTuru == "20")
                formAdi = Resources.TasinirMal.FRMZFG003;
            else
                formAdi = Resources.TasinirMal.FRMBRK046;

            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            panelIslem.AutoLoad.Url = "ZimmetForm.aspx?menuYok=1&belgeTur=" + belgeTuru;
            panelSorgu.AutoLoad.Url = "ZimmetFormSorgu.aspx?menuYok=1&belgeTur=" + belgeTuru;
        }
    }
}