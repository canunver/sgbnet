using System;
using OrtakClass;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r i�lem fi�i bilgilerinin kay�t ve listeleme i�lemlerini yapan sayfalar� i�eren sayfa
    /// </summary>
    public partial class TasinirIslemFormAna : TMMSayfaV2
    {
        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BaslikBilgileriniAyarla();

            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giri� izni varm�?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            var islemSayfa = "TasinirIslemFormMB.aspx?menuYok=1";
            if (Request.QueryString["gm"] + "" == "1")
            {
                if (Request.QueryString["coklu"] != "1")
                    islemSayfa = "TasinirIslemFormGM.aspx?menuYok=1&gm=" + Request.QueryString["gm"] + "";
                else
                    islemSayfa = islemSayfa + "&gm=" + Request.QueryString["gm"] + "";
            }
            else if (Request.QueryString["gm"] + "" == "2")
                islemSayfa = islemSayfa + "&gm=" + Request.QueryString["gm"] + "";


            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                panelIslemYeni.Hidden = true;

            panelIslem.AutoLoad.Url = islemSayfa +
                "&kutuphane=" + Request.QueryString["kutuphane"] +
                "&muze=" + Request.QueryString["muze"] +
                "&yil=" + Request.QueryString["yil"] +
                "&belgeNo=" + Request.QueryString["belgeNo"] +
                "&muhasebe=" + Request.QueryString["muhasebe"] +
                "&harcamaBirimi=" + Request.QueryString["harcamaBirimi"] +
                "&aramaYok=" + Request.QueryString["aramaYok"] +
                "&dagitimIade=" + Request.QueryString["dagitimIade"];


            panelSorgu.AutoLoad.Url = "TasinirIslemFormSorgu.aspx?menuYok=1" +
                "&kutuphane=" + Request.QueryString["kutuphane"] +
                "&muze=" + Request.QueryString["muze"] +
                "&dagitimIade=" + Request.QueryString["dagitimIade"] +
                "&gm=" + Request.QueryString["gm"];

            panelIslemYeni.AutoLoad.Url = "TasinirIslemForm.aspx?menuYok=1" +
                "&kutuphane=" + Request.QueryString["kutuphane"] +
                "&muze=" + Request.QueryString["muze"] +
                "&yil=" + Request.QueryString["yil"] +
                "&belgeNo=" + Request.QueryString["belgeNo"] +
                "&muhasebe=" + Request.QueryString["muhasebe"] +
                "&harcamaBirimi=" + Request.QueryString["harcamaBirimi"] +
                "&aramaYok=" + Request.QueryString["aramaYok"] +
                "&dagitimIade=" + Request.QueryString["dagitimIade"];
        }

        void BaslikBilgileriniAyarla()
        {
            formAdi = Resources.TasinirMal.FRMTIG001;
            if (Request.QueryString["kutuphane"] + "" != "")
                formAdi = Resources.TasinirMal.FRMTIG002;
            if (Request.QueryString["muze"] + "" != "")
                formAdi = Resources.TasinirMal.FRMTIG003;
            if (Request.QueryString["dagitimIade"] + "" != "")
                formAdi = Resources.TasinirMal.FRMTIG126;
            if (Request.QueryString["gm"] + "" == "1")
                formAdi = "Gayrimenkul ��lem Fi�i";
            if (Request.QueryString["gm"] + "" == "2")
                formAdi = "Yaz�l�m ��lem Fi�i";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

    }
}