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
    /// Varolan bir harcama birimini yeni bir harcama birimi ile de�i�tirme i�leminin yap�ld��� sayfa
    /// De�i�iklik yap�lmadan �nce ta��n�r mal veri taban�n�n yede�i al�nmas� �nerilir, ��nk� veri
    /// taban�n�n b�t�n tablolar�ndaki harcama birimi bilgileri yenisiyle de�i�tirilecektir.
    /// </summary>
    public partial class HarcamaBirimDegistir : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ile ilgili ayarlamalar yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMHBD001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
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
        /// De�i�tir tu�una bas�l�nca �al��an olay metodu
        /// Eski ve yeni harcam birimi bilgileri ekrandaki ilgili kontrollerden toplan�p de�i�tirilmek
        /// �zere sunucuya g�nderilir, gelen sonuca g�re hata mesaj� veya bilgi mesaj� verilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyar�", sonuc.hataStr);
        }
    }
}