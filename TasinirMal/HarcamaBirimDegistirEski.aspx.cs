using System;
using System.Web.UI;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Varolan bir harcama birimini yeni bir harcama birimi ile de�i�tirme i�leminin yap�ld��� sayfa
    /// De�i�iklik yap�lmadan �nce ta��n�r mal veri taban�n�n yede�i al�nmas� �nerilir, ��nk� veri
    /// taban�n�n b�t�n tablolar�ndaki harcama birimi bilgileri yenisiyle de�i�tirilecektir.
    /// </summary>
    public partial class HarcamaBirimDegistirEski : TMMSayfa
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

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
            TasinirGenel.JSResourceEkle_ZimmetForm(this);
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMHBD001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))// || kullanan.kullaniciKodu != "stratek")
                GenelIslemler.SayfayaGirmesin(false, Resources.TasinirMal.FRMHBD002);

            this.btnDegistir.Attributes.Add("onclick", "return OnayAl('harcamaBirimDegistir','btnDegistir');");
            this.txtMuhasebeEski.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAdEski',true,new Array('txtMuhasebeEski'),'KONTROLDENOKU');");
            this.txtHarcamaBirimiEski.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAdEski',true,new Array('txtMuhasebeEski','txtHarcamaBirimiEski'),'KONTROLDENOKU');");
            this.txtMuhasebeYeni.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAdYeni',true,new Array('txtMuhasebeYeni'),'KONTROLDENOKU');");
            this.txtHarcamaBirimiYeni.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAdYeni',true,new Array('txtMuhasebeYeni','txtHarcamaBirimiYeni'),'KONTROLDENOKU');");

            //Bir hata nedeniyle bazen otomatik __doPostBack yarat�lam�yor. 
            //A�a��daki kod __doPostBack yarat�r. 
            //D��meye bas�ld�ktan sonra Onay i�lemi gelir ondan sonra submit 
            //i�lemi i�in __doPostBack gerekli
            Page.ClientScript.GetPostBackEventReference(this, String.Empty);

            if (!IsPostBack)
            {
            }

            if (txtMuhasebeEski.Text.Trim() != "")
                lblMuhasebeAdEski.Text = GenelIslemler.KodAd(31, txtMuhasebeEski.Text.Trim(), true);
            else
                lblMuhasebeAdEski.Text = "";

            if (txtHarcamaBirimiEski.Text.Trim() != "")
                lblHarcamaBirimiAdEski.Text = GenelIslemler.KodAd(32, txtMuhasebeEski.Text.Trim() + "-" + txtHarcamaBirimiEski.Text.Trim(), true);
            else
                lblHarcamaBirimiAdEski.Text = "";

            if (txtMuhasebeYeni.Text.Trim() != "")
                lblMuhasebeAdYeni.Text = GenelIslemler.KodAd(31, txtMuhasebeYeni.Text.Trim(), true);
            else
                lblMuhasebeAdYeni.Text = "";

            if (txtHarcamaBirimiYeni.Text.Trim() != "")
                lblHarcamaBirimiAdYeni.Text = GenelIslemler.KodAd(32, txtMuhasebeYeni.Text.Trim() + "-" + txtHarcamaBirimiYeni.Text.Trim(), true);
            else
                lblHarcamaBirimiAdYeni.Text = "";
        }

        /// <summary>
        /// De�i�tir tu�una bas�l�nca �al��an olay metodu
        /// Eski ve yeni harcam birimi bilgileri ekrandaki ilgili kontrollerden toplan�p de�i�tirilmek
        /// �zere sunucuya g�nderilir, gelen sonuca g�re hata mesaj� veya bilgi mesaj� verilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDegistir_Click(object sender, EventArgs e)
        {
            HarcamaBirimi hbEski = new HarcamaBirimi();
            hbEski.muhasebeKod = txtMuhasebeEski.Text.Trim();
            hbEski.kod = txtHarcamaBirimiEski.Text.Trim();

            HarcamaBirimi hbYeni = new HarcamaBirimi();
            hbYeni.muhasebeKod = txtMuhasebeYeni.Text.Trim();
            hbYeni.kod = txtHarcamaBirimiYeni.Text.Trim();

            Sonuc sonuc = servis.HarcamaBirimDegistir(kullanan, hbEski, hbYeni);
            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            else
                GenelIslemler.MesajKutusu("Uyar�", sonuc.hataStr);
        }
    }
}