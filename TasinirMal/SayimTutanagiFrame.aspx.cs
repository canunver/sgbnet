using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;

namespace TasinirMal
{
    public partial class SayimTutanagiFrame : TMMSayfa
    {
        bool uyumlulukModuVar = false;
        public override bool UyumlulukModuVar()
        {
            return uyumlulukModuVar;
        }

        /// <summary>
        /// Sayfayının yüklenmesi sırasında çağrılan fonksiyon.
        /// </summary>
        /// <param name="sender">Olayı çağıran nesne, Object türünde nesne</param>
        /// <param name="e">Olay argümanı, EventArgs türünde nesne</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string yil = Request.QueryString["yil"] + "";
            string belgeNo = Request.QueryString["belgeNo"] + "";
            string aramaYok = Request.QueryString["aramaYok"] + "";
            string belgeTarih = Request.QueryString["belgeTarih"];
            string harcamaBirimi = Request.QueryString["harcamaBirimi"] + "";
            string ambar = Request.QueryString["ambar"] + "";
            string sayimNo = Request.QueryString["sayimNo"] + "";
            string arama = hdnArama.Value + "";

            formAdi = "Sayım Tutanağı Belgesi İşlemleri";

            if (arama == "1")
                panelIslem.AutoLoad.Url = "SayimTutanagiKayit.aspx?menuID=Menu10_6&yil=" + yil + "&muhasebe=" + belgeNo + "&harcama=" + harcamaBirimi + "&ambar=" + ambar + "&sayimNo=" + sayimNo;
            else
                panelIslem.AutoLoad.Url = "SayimTutanagiKayit.aspx?menuYok=1&yil=" + yil + "&belgeNo=" + belgeNo + "&aramaYok=" + aramaYok;

            if (aramaYok != "1")
            {
                panelSorgu.AutoLoad.Url = "SayimTutanagiSorgu.aspx?menuYok=1";
            }
            else
            {
                panelSorgu.Visible = false;
            }

            SayfaUstAltBolumYaz(this);
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

     
        }
    }
}