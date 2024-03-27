using System;
using System.Web.UI;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Varolan bir harcama birimini yeni bir harcama birimi ile deðiþtirme iþleminin yapýldýðý sayfa
    /// Deðiþiklik yapýlmadan önce taþýnýr mal veri tabanýnýn yedeði alýnmasý önerilir, çünkü veri
    /// tabanýnýn bütün tablolarýndaki harcama birimi bilgileri yenisiyle deðiþtirilecektir.
    /// </summary>
    public partial class HarcamaBirimDegistirEski : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

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

            //Bir hata nedeniyle bazen otomatik __doPostBack yaratýlamýyor. 
            //Aþaðýdaki kod __doPostBack yaratýr. 
            //Düðmeye basýldýktan sonra Onay iþlemi gelir ondan sonra submit 
            //iþlemi için __doPostBack gerekli
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
        /// Deðiþtir tuþuna basýlýnca çalýþan olay metodu
        /// Eski ve yeni harcam birimi bilgileri ekrandaki ilgili kontrollerden toplanýp deðiþtirilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
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
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }
    }
}