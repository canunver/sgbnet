using OrtakClass;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TNS;
using TNS.ORG;

namespace TasinirMal
{
    public class TMMSayfaV2 : istemciUzayi.GenelSayfa
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.Page.Response.Expires = 0;

            string[] jQueryJS = new string[] { "ModulScripts/OrtakExt.js?v=15", "ModulScripts/TasinirOrtak.js?v=13" };
            for (int i = jQueryJS.Length - 1; i >= 0; i--)
            {
                HtmlGenericControl incJQ = new HtmlGenericControl("script");
                incJQ.Attributes.Add("type", "text/javascript");
                incJQ.Attributes.Add("language", "javascript");
                incJQ.Attributes.Add("src", jQueryJS[i]);
                this.Page.Header.Controls.AddAt(1, incJQ);
            }

            this.Page.Header.Controls.AddAt(1, istemciUzayi.GenelSayfa.HtmlLinkOl("../Script/GridExt.css", "text/css", "stylesheet"));
        }

        public override void SayfaUstAltBolumYaz(Page p)
        {
            base.SayfaUstAltBolumYaz(p);

            if (p.IsPostBack)
                return;

            string muhasebe = string.Empty;
            string harcama = string.Empty;
            string ambar = string.Empty;
            try
            {
                Ext1.Net.TriggerField tb = (Ext1.Net.TriggerField)p.FindControl("txtMuhasebe");
                if (tb != null)
                {
                    tb.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                    muhasebe = tb.Text;
                }

                tb = (Ext1.Net.TriggerField)p.FindControl("txtHarcamaBirimi");
                if (tb != null)
                {
                    tb.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                    harcama = tb.Text;
                }

                tb = (Ext1.Net.TriggerField)p.FindControl("txtAmbar");
                if (tb != null)
                {
                    tb.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
                    ambar = tb.Text;
                }
            }
            catch (Exception e)
            {
            }
        }
    }

    public class TMMSayfa : TMMSayfaV2
    {
        public override void SayfaUstAltBolumYaz(Page p)
        {
            base.SayfaUstAltBolumYaz(p);

            if (p.IsPostBack)
                return;

            if (!GenelIslemlerIstemci.OrgSemaKullaniyorMu())
                return;

            int i = 0;
            while (true)
            {
                HtmlGenericControl divMHA = (HtmlGenericControl)p.FindControl("divMHA" + i);
                if (divMHA == null)
                    break;
                HtmlGenericControl divOrg = (HtmlGenericControl)p.FindControl("divOrg" + i);
                if (divOrg == null)
                    break;

                divMHA.Style.Add("display", "none");
                divOrg.Style.Remove("display");

                i++;
            }

            string muhasebe = string.Empty;
            TextBox tb = (TextBox)p.FindControl("txtMuhasebe");
            if (tb != null)
            {
                tb.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                muhasebe = tb.Text;
            }

            string harcama = string.Empty;
            tb = (TextBox)p.FindControl("txtHarcamaBirimi");
            if (tb != null)
            {
                tb.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                harcama = tb.Text;
            }

            string ambar = string.Empty;
            tb = (TextBox)p.FindControl("txtAmbar");
            if (tb != null)
            {
                tb.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
                ambar = tb.Text;
            }

            Ext1.Net.DropDownField ddlBirim = (Ext1.Net.DropDownField)p.FindControl("ddlBirim");
            if (ddlBirim != null)
            {
                OrganizasyonBirim ob = new OrganizasyonBirim();
                ob.muhasebekodu = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0).ToString();
                if (!string.IsNullOrEmpty(harcama))
                {
                    ob.muhasebebirim = harcama.Substring(harcama.LastIndexOf('.') + 1);
                    ob.kurumsalkod = harcama.Substring(0, harcama.LastIndexOf('.'));
                }
                ob.ambarKod = ambar;

                ObjectArray obler = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(ob, false, false);
                if (obler.sonuc.islemSonuc && obler.objeler.Count > 0)
                {
                    ob = (OrganizasyonBirim)obler[0];
                    ddlBirim.Value = ob.kod.ToString();
                    ddlBirim.Text = ob.adi;
                }
            }
        }
    }
}