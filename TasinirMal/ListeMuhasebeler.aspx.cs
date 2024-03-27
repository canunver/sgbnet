using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Muhasebe birimlerinin çoklu seçim iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ListeMuhasebeler : istemciUzayi.GenelSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir varsa
        ///     sayfa adresinde gelen girdi dizgileri kullanýlarak sayfadaki kontroller
        ///     ayarlanýr ve muhasebe listeleme iþlemini yapan ListeDoldur yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMLML001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);

            btnTamam.Attributes.Add("onclick", "listeye_ekle_sayfa('lstDest','" + Request.QueryString["degerAlanAd"] + "','" + Request.QueryString["degerAlanDegerAlanAd"] + "');javascript:self.close();");
            Page.ClientScript.RegisterOnSubmitStatement(Page.GetType(), "OnSubmit", "hepsini_sec('lstDest')");

            if (!IsPostBack)
            {
                if (Request.QueryString["degerAlanAd"].StartsWith("lst"))
                    ClientScript.RegisterStartupScript(this.GetType(), "onLoad", "<SCRIPT language=javascript>hedef_liste_olustur_listeden(opener.document.getElementById('" + Request.QueryString["degerAlanAd"] + "'),document.getElementById('lstDest'));</SCRIPT>");
                else
                    ClientScript.RegisterStartupScript(this.GetType(), "onLoad", "<SCRIPT language=javascript>hedef_liste_olustur('lstSrc','lstDest','" + Request.QueryString["degerAlanDegerAlanAd"] + "');</SCRIPT>");

                hdngirisListeAd.Value = Request.QueryString["girisListeAd"];
                string listeDoldurma = Request.QueryString["girisListeDoldurma"] + "";
                lblYeniGiris.Text = "<a href=\"#\" onClick=\"SozlukAc('" + Request.QueryString["yeniKayitURL"] + "');\">" + Resources.TasinirMal.FRMLML002 + "</a>";

                //seçim tek veya çoklu olmasý
                txtSecimSayisi.Value = Request.QueryString["degerSayisi"];

                if (OrtakFonksiyonlar.ConvertToInt(listeDoldurma, 0) != 1)
                {
                    lstSrc.Items.Clear();

                    ListeDoldur(new TNS.TMM.Muhasebe(), lstSrc);
                }
                txtArama.Focus();
            }
        }

        /// <summary>
        /// Parametre olarak verilen muhasebe birimi kriterine uyan muhasebe birimlerini verilen kontrole dolduran yordam
        /// </summary>
        /// <param name="sozluk">Muhasebe birimi kriter bilgilerini tutan nesne</param>
        /// <param name="lstKontrol">Muhasebe birimlerinin doldurulacaðý kontrol</param>
        void ListeDoldur(TNS.TMM.Muhasebe sozluk, HtmlSelect lstKontrol)
        {
            ObjectArray listeVAN = servisTMM.MuhasebeListele(kullanan, sozluk);
            foreach (TNS.TMM.Muhasebe y in listeVAN.objeler)
                lstKontrol.Items.Add(new ListItem(y.kod + " - " + y.ad, y.kod));
        }

        /// <summary>
        /// Bul tuþuna basýlýnca çalýþan olay metodu
        /// lstDest ve lstSrc kontrollerine ilgili muhasebe birimlerini doldurur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, EventArgs e)
        {
            string dest = (string)Request.Form["lstDest"];
            if (dest != "" && dest != null)
            {
                foreach (string kod in dest.Split(','))
                {
                    if (kod != "")
                    {
                        TNS.TMM.Muhasebe s = new TNS.TMM.Muhasebe();
                        s.kod = kod;
                        ListeDoldur(s, lstDest);
                    }
                }
            }

            TNS.TMM.Muhasebe ss = new TNS.TMM.Muhasebe();
            ss.ad = txtArama.Text.Trim();

            lstSrc.Items.Clear();
            ListeDoldur(ss, lstSrc);
        }
    }
}