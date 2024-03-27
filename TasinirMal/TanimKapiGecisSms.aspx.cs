using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// RFID entegrasyonu olan kurumlarda çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak olursa
    /// bilgilendirilecek personele ait iletişim bilgilerinin kayıt, silme ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimKapiGecisSms : TMMSayfaV2
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

            formAdi = Resources.TasinirMal.FRMTKI001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            if (!IsPostBack)
            {
                this.txtTCKimlikNo.Attributes.Add("onblur", "kodAdGetir('36','lblKisiAd',true,new Array('txtTCKimlikNo'),'KONTROLDENOKU');");
            }

            if (txtTCKimlikNo.Text.Trim() != "")
                lblKisiAd.Text = GenelIslemler.KodAd(36, txtTCKimlikNo.Text.Trim(), true);
            else
                lblKisiAd.Text = "";
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// Personel iletişim bilgileri kaydedilmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            Sonuc sonuc = servisTMM.KapiGecisSmsKaydet(kullanan, KriterTopla());

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTKI002);
                Ara(new Personel());
            }
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Sil tuşuna basılınca çalışan olay metodu
        /// Seçili olan personel, iletişim bilgileri silinmek üzere sunucuya
        /// gönderilir, gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, EventArgs e)
        {
            Sonuc sonuc = servisTMM.KapiGecisSmsSil(kullanan, KriterTopla());

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTKI003);
                Ara(new Personel());
            }
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Bul tuşuna basılınca çalışan olay metodu
        /// Sunucudan personel iletişim bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, EventArgs e)
        {
            Ara(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden personel iletişim bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Personel iletişim bilgileri döndürülür.</returns>
        private Personel KriterTopla()
        {
            Personel personel = new Personel();
            personel.kod = txtTCKimlikNo.Text.Trim();
            personel.cepTel = txtCepTel.Text.Trim();
            personel.eposta = txtEposta.Text.Trim();
            personel.smsAt = chkKullanim.Items[0].Selected;
            personel.epostaAt = chkKullanim.Items[1].Selected;

            return personel;
        }

        /// <summary>
        /// Parametre olarak verilen personel nesnesi sunucuya gönderilir ve personel
        /// iletişim bilgileri alınır. Hata varsa ekrana hata bilgisi yazılır, yoksa
        /// gelen personel iletişim bilgileri dgListe DataGrid kontrolüne doldurulur.
        /// </summary>
        /// <param name="personel">Personel iletişim kriter bilgilerini tutan nesne</param>
        private void Ara(Personel personel)
        {
            ObjectArray listeVAN = servisTMM.KapiGecisSmsListele(kullanan, personel);

            dgListe.DataBind();

            if (!listeVAN.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", listeVAN.sonuc.hataStr);
                return;
            }

            if (listeVAN.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", listeVAN.sonuc.bilgiStr);
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add("kod");
            table.Columns.Add("ad");
            table.Columns.Add("cepTel");
            table.Columns.Add("eposta");
            table.Columns.Add("cepTelKullanim");
            table.Columns.Add("epostaKullanim");

            string cepTelKullanim = string.Empty;
            string epostaKullanim = string.Empty;
            string link = string.Empty;
            foreach (Personel p in listeVAN.objeler)
            {
                cepTelKullanim = p.smsAt ? Resources.TasinirMal.FRMTKI004 : Resources.TasinirMal.FRMTKI005;
                epostaKullanim = p.epostaAt ? Resources.TasinirMal.FRMTKI004 : Resources.TasinirMal.FRMTKI005;
                link = "VeriGoster('" + p.kod.ToString() + "','" + p.ad + "','" + p.cepTel + "','" + p.eposta + "','" + p.smsAt + "','" + p.epostaAt + "');return false;";
                table.Rows.Add(p.kod, "<a href='#' onclick=\"" + link + "\">" + p.ad + "</a>", p.cepTel, p.eposta, cepTelKullanim, epostaKullanim);
            }

            dgListe.DataSource = table;
            dgListe.DataBind();
        }
    }
}