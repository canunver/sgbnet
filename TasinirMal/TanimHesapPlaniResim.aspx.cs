using System;
using System.Configuration;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Taşınır hesap planına ait resim bilgilerinin kayıt ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimHesapPlaniResim : istemciUzayi.GenelSayfa
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
            formAdi = Resources.TasinirMal.FRMTHR001;

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            if (!IsPostBack)
            {
                this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");
            }

            if (txtHesapPlanKod.Text.Trim() != "")
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = "";
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// Taşınır hesap planına ait resim bilgileri kaydedilmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajı görüntülenir. Daha sonra resim dosyası
        /// kaydedilir. Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            dgListe.DataBind();
            TNS.TMM.HesapPlaniSatir hpResim = KriterTopla();
            Sonuc sonuc = servisTMM.HesapPlaniResimKaydet(kullanan, hpResim);
            if (sonuc.islemSonuc)
            {
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMTHR002);
                DosyaKaydet();

                Listele(KriterTopla());
            }
            else
                GenelIslemler.HataYaz(this, sonuc.hataStr);
        }

        /// <summary>
        /// Taşınır hesap planı resim dosyasını sürücüdeki ilgili klasöre kaydeden yordam
        /// </summary>
        private void DosyaKaydet()
        {
            string hesapPlanDosyaYolYaz = ConfigurationManager.AppSettings["TasinirHesapPlanDosyaYolYaz"];
            Sonuc sonuc = GenelIslemler.DosyaKaydet(fupDosya, hesapPlanDosyaYolYaz, hdnDosya.Value.Trim());
            if (!sonuc.islemSonuc)
                GenelIslemler.HataYaz(this, sonuc.hataStr);
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden taşınır hesap planı resim kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Taşınır hesap planı resim kriter bilgileri döndürülür.</returns>
        private TNS.TMM.HesapPlaniSatir KriterTopla()
        {
            TNS.TMM.HesapPlaniSatir hpResim = new TNS.TMM.HesapPlaniSatir();
            hpResim.hesapKod = txtHesapPlanKod.Text.Trim();
            hpResim.aciklama = lblHesapPlanAd.Text.Trim();
            hpResim.resim = fupDosya.FileName;
            return hpResim;
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Sunucudan taşınır hesap planı resim bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            lblResim.Text = string.Empty;
            hdnDosya.Value = string.Empty;
            Listele(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen taşınır hesap planı resim nesnesi sunucuya gönderilir ve taşınır
        /// hesap planı resim bilgileri alınır. Hata varsa ekrana hata bilgisi yazılır, yoksa gelen
        /// taşınır hesap planı resim bilgileri ekrana yazılmak üzere GridDoldur yordamına gönderilir.
        /// </summary>
        /// <param name="hpResim">Taşınır hesap planı kriter bilgilerini tutan nesne</param>
        private void Listele(TNS.TMM.HesapPlaniSatir hpResim)
        {
            ObjectArray resimVan = servisTMM.HesapPlaniResimListele(kullanan, hpResim);
            if (!resimVan.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, resimVan.sonuc.hataStr);
                dgListe.DataBind();
                return;
            }

            if (resimVan.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, resimVan.sonuc.bilgiStr);
                dgListe.DataBind();
                return;
            }

            GridDoldur(resimVan.objeler);
        }

        /// <summary>
        /// Parametre olarak verilen taşınır hesap planı resim bilgileri dgListe DataGrid kontrolüne doldurulur.
        /// </summary>
        /// <param name="resimler">Taşınır hesap planı resim bilgileri listesini tutan nesne</param>
        private void GridDoldur(TNSCollection resimler)
        {
            string hesapPlanDosyaYolOku = ConfigurationManager.AppSettings["TasinirHesapPlanDosyaYolOku"];

            DataTable table = new DataTable();
            table.Columns.Add("hesapKod");
            table.Columns.Add("hesapAd");
            table.Columns.Add("resim");

            foreach (TNS.TMM.HesapPlaniSatir r in resimler)
                table.Rows.Add("<a href='#' onclick=\"VeriGoster('" + r.hesapKod + "','" + r.aciklama + "','" + r.resim + "')\">" + r.hesapKod + "</a>", r.aciklama, "<a href=\"" + hesapPlanDosyaYolOku + r.resim + "\" target=_blank>" + r.resim + "</a>");

            dgListe.DataSource = table;
            dgListe.DataBind();
        }
    }
}