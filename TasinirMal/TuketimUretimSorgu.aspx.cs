using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class TuketimUretimSorgu : TMMSayfa
    {
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMTUL014;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan))
                    GenelIslemler.SayfayaGirmesin(true);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
                BelgeTurDoldur();

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            }
            else
                DegiskenSakla();

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (!string.IsNullOrEmpty(txtAmbar.Text.Trim()))
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = string.Empty;
        }

        private void DegiskenSakla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
        }

        private void BelgeTurDoldur()
        {
            ddlTur.Items.Clear();
            ddlTur.Items.Add(new ListItem(string.Empty, "0"));
            ddlTur.Items.Add(new ListItem(Resources.TasinirMal.FRMTUG020, ((int)ENUMTuketimUretim.TUKETIM).ToString()));
            ddlTur.Items.Add(new ListItem(Resources.TasinirMal.FRMTUG021, ((int)ENUMTuketimUretim.URETIM).ToString()));
        }

        private TNS.TMM.TuketimUretim KriterTopla()
        {
            TNS.TMM.TuketimUretim tu = new TNS.TMM.TuketimUretim();
            tu.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            tu.muhasebe = new Muhasebe();
            tu.muhasebe.kod = txtMuhasebe.Text.Trim();
            tu.harcamaBirimi = new HarcamaBirimi();
            tu.harcamaBirimi.kod = txtHarcamaBirimi.Text.Trim();
            tu.ambar = new Ambar();
            tu.ambar.kod = txtAmbar.Text.Trim();
            tu.tur = OrtakFonksiyonlar.ConvertToInt(ddlTur.SelectedValue.Trim(), 0);
            return tu;
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            ObjectArray bilgi = servis.TuketimUretimListele(kullanan, KriterTopla(), false);
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }
            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("belgeNo");
            dt.Columns.Add("yil");
            dt.Columns.Add("belgeTarih");
            dt.Columns.Add("muhasebe");
            dt.Columns.Add("muhasebeAd");
            dt.Columns.Add("harcamaBirimi");
            dt.Columns.Add("harcamaBirimiAd");
            dt.Columns.Add("ambar");
            dt.Columns.Add("belgeTur");

            foreach (TNS.TMM.TuketimUretim tu in bilgi.objeler)
                dt.Rows.Add(tu.belgeNo.PadLeft(6, '0'), tu.yil, tu.belgeTarihi.ToString(), tu.muhasebe.kod, tu.muhasebe.ad, tu.harcamaBirimi.kod, tu.harcamaBirimi.ad, tu.ambar.kod + " (" + tu.ambar.ad + ")", tu.tur == 1 ? Resources.TasinirMal.FRMTUL018 : Resources.TasinirMal.FRMTUL019);

            gvBelgeler.DataSource = dt;
            gvBelgeler.DataBind();

            for (int i = 0; i < gvBelgeler.Rows.Count; i++)
                gvBelgeler.Rows[i].Cells[0].Text = Server.HtmlDecode(gvBelgeler.Rows[i].Cells[0].Text);
        }
    }
}