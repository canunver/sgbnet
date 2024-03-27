using System;
using OrtakClass;

namespace TasinirMal
{
    public partial class TuketimUretimAna : istemciUzayi.GenelSayfa
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = Resources.TasinirMal.FRMTUA004;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);
            SayfaUstAltBolumYaz(this);

            if (Request.QueryString["menuYok"] + "" != "")
                BorderLayout1.Center.MarginsSummary = "0 0 0 0";

            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            panelIslem.AutoLoad.Url = "TuketimUretim.aspx?menuYok=1";
            panelSorgu.AutoLoad.Url = "TuketimUretimSorgu.aspx?menuYok=1";
        }
    }
}