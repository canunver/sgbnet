using System;
using OrtakClass;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþi bilgilerinin kayýt ve listeleme iþlemlerini yapan sayfalarý içeren sayfa
    /// </summary>
    public partial class TasinirIslemFormAna : TMMSayfaV2
    {
        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BaslikBilgileriniAyarla();

            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            //Sayfaya giriþ izni varmý?
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
                formAdi = "Gayrimenkul Ýþlem Fiþi";
            if (Request.QueryString["gm"] + "" == "2")
                formAdi = "Yazýlým Ýþlem Fiþi";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

    }
}