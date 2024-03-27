using System;
using System.Web.UI;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Varolan bir harcama birimini yeni bir harcama birimi ile deðiþtirme iþleminin yapýldýðý sayfa
    /// Deðiþiklik yapýlmadan önce taþýnýr mal veri tabanýnýn yedeði alýnmasý önerilir, çünkü veri
    /// tabanýnýn bütün tablolarýndaki harcama birimi bilgileri yenisiyle deðiþtirilecektir.
    /// </summary>
    public partial class HarcamaBirimDegistir : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ile ilgili ayarlamalar yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMHBD001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    tabPanelAna.Border = true;
                }
                else
                    hdnSecKapat.Value = 1;
            }
        }

        /// <summary>
        /// Deðiþtir tuþuna basýlýnca çalýþan olay metodu
        /// Eski ve yeni harcam birimi bilgileri ekrandaki ilgili kontrollerden toplanýp deðiþtirilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDegistir_Click(object sender, DirectEventArgs e)
        {
            HarcamaBirimi hbEski = new HarcamaBirimi();
            hbEski.muhasebeKod = txtMuhasebe.Text.Trim();
            hbEski.kod = txtHarcamaBirimi.Text.Trim();

            HarcamaBirimi hbYeni = new HarcamaBirimi();
            hbYeni.muhasebeKod = txtGonMuhasebe.Text.Trim();
            hbYeni.kod = txtGonHarcamaBirimi.Text.Trim();

            Sonuc sonuc = servisTMM.HarcamaBirimDegistir(kullanan, hbEski, hbYeni);
            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }
    }
}