using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Amortisman işlemi uygulanacak malzemelerin amortisman sürelerinin
    /// kayıt, silme ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimAmortismanSure : TMMSayfaV2
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        ///     Sayfa ilk defa çağırılıyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMTTS001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                //TasinirGenel.AmortismanLisansKontrol(this);

                ddlYil.Items.Add(new ListItem(""));
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
            }

            if (txtHesapPlanKod.Text.Trim() != "")
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = "";
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// Amortisman işlemi uygulanacak malzemenin amortisman süresi kaydedilmek üzere
        /// sunucuya gönderilir, gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            //AmortismanSure sure = new AmortismanSure();
            //sure.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            //sure.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            //sure.sure = OrtakFonksiyonlar.ConvertToInt(txtSure.Text.Trim(), 0);

            //Sonuc sonuc = servisTMM.AmortismanSureKaydet(kullanan, sure);

            //if (sonuc.islemSonuc)
            //{
            //    Ara();
            //    txtHesapPlanKod.Text = string.Empty;
            //    lblHesapPlanAd.Text = string.Empty;
            //    txtSure.Text = string.Empty;
            //    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTTS002);
            //}
            //else
            //    GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Sil tuşuna basılınca çalışan olay metodu
        /// Seçili olan malzemenin amortisman süresi silinmek üzere sunucuya
        /// gönderilir, gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, EventArgs e)
        {
            //AmortismanSure sure = new AmortismanSure();
            //sure.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            //sure.hesapPlanKod = txtHesapPlanKod.Text.Trim();

            //Sonuc sonuc = servisTMM.AmortismanSureSil(kullanan, sure);

            //if (sonuc.islemSonuc)
            //{
            //    Ara();
            //    txtHesapPlanKod.Text = string.Empty;
            //    lblHesapPlanAd.Text = string.Empty;
            //    txtSure.Text = string.Empty;
            //    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTTS003);
            //}
            //else
            //    GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Kaydetme, silme gibi işlemlerden sonra son arama kriterleriyle listeleme yapan yordam
        /// </summary>
        private void Ara()
        {
            if (Session["AmortismanSureArama"] != null)
                Ara((AmortismanSure)Session["AmortismanSureArama"]);
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Sunucudan amortisman işlemi uygulanacak malzemelerin amortisman süre bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            AmortismanSure sure = new AmortismanSure();
            sure.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            sure.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            Ara(sure);
        }

        /// <summary>
        /// Parametre olarak verilen amortisman süre kriteri sunucuya gönderilir ve amortisman işlemi
        /// uygulanacak malzemelerin amortisman süre bilgileri alınır. Hata varsa ekrana hata bilgisi
        /// yazılır, yoksa gelen amortisman süre bilgileri dgListe GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="sure">Amortisman süre kriteri</param>
        private void Ara(AmortismanSure sure)
        {
            //Session["AmortismanSureArama"] = sure;

            //ObjectArray listeVAN = servisTMM.AmortismanSureListele(kullanan, sure);

            //dgListe.DataBind();

            //if (!listeVAN.sonuc.islemSonuc)
            //{
            //    GenelIslemler.MesajKutusu("Uyarı", listeVAN.sonuc.hataStr);
            //    return;
            //}

            //if (listeVAN.objeler.Count <= 0)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", listeVAN.sonuc.bilgiStr);
            //    return;
            //}

            //txtSure.Text = string.Empty;

            //DataTable table = new DataTable();
            //table.Columns.Add("yil");
            //table.Columns.Add("hesapKod");
            //table.Columns.Add("hesapAd");
            //table.Columns.Add("sure");

            //foreach (AmortismanSure s in listeVAN.objeler)
            //    table.Rows.Add(s.yil, "<a href='#' onclick=\"VeriGoster('" + s.yil.ToString() + "','" + s.hesapPlanKod + "','" + s.hesapPlanAd + "','" + s.sure.ToString() + "');return false;\">" + s.hesapPlanKod + "</a>", s.hesapPlanAd, s.sure);

            //dgListe.DataSource = table;
            //dgListe.DataBind();

            //for (int i = 0; i < dgListe.Rows.Count; i++)
            //    dgListe.Rows[i].Cells[1].Text = Server.HtmlDecode(dgListe.Rows[i].Cells[1].Text);
        }
    }
}