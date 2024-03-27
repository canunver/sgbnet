using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.TMM;
using OrtakClass;
using TNS;
using System.Data;

namespace TasinirMal
{
    /// <summary>
    /// Sayım tutanağı bilgilerinin listeleme işleminin yapıldığı sayfa
    /// </summary>
    public partial class SayimTutanagiSorgu : istemciUzayi.GenelSayfa
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        /// Kullanıcı session'dan okunur.
        /// Yetki kontrolü yapılır.
        /// Sayfa ilk defa çağırılıyorsa kontrollere ilgili bilgiler
        /// doldurulur, sayfa ayarlanır ve sayım tutanakları listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMSYL001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            if (!IsPostBack)
            {
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
                ddlYil.SelectedItem.Text = DateTime.Now.Year.ToString();
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            }
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden sayım tutanağı bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Sayım tutanağı bilgileri döndürülür.</returns>
        private SayimForm KriterTopla()
        {
            SayimForm sf = new SayimForm();
            sf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedItem.Text, 0);
            sf.muhasebeKod = txtMuhasebe.Text.Trim();
            sf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            sf.ambarKod = txtAmbar.Text.Trim();
            return sf;
        }

        /// <summary>
        /// Listeleme kriterleri sayım form nesnesinde parametre olarak alınır,
        /// sunucuya gönderilir ve sayım tutanağı bilgileri sunucudan alınır. Hata varsa ekrana
        /// hata bilgisi yazılır, yoksa gelen bilgiler dgListe DataGrid kontrolüne doldurulur.
        /// </summary>
        /// <param name="sf">Sayım tutanağı listeleme kriterlerini tutan nesne</param>
        private void Listele(SayimForm sf)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            ObjectArray bilgi = servisTMM.SayimListele(kullanan, sf, false);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
            }

            DataTable tablo = new DataTable();
            tablo.Columns.Add("Yil");
            tablo.Columns.Add("Muhasebe");
            tablo.Columns.Add("Harcama");
            tablo.Columns.Add("Ambar");
            tablo.Columns.Add("BelgeNo");
            tablo.Columns.Add("Tarih", Type.GetType("System.DateTime"));

            foreach (SayimForm sForm in bilgi.objeler)
            {
                string link = "<a href='javascript:BelgeGirisAc(\"" + sForm.yil + ";" + sForm.muhasebeKod + ";" + sForm.harcamaKod + ";" + sForm.ambarKod + ";" + sForm.sayimNo + "\")'>" + sForm.yil + "</a>";
                tablo.Rows.Add(link, sForm.muhasebeKod, sForm.harcamaKod, sForm.ambarKod, sForm.sayimNo, sForm.sayimTarih.ToString());
            }

            stoIst1.DataSource = tablo;
            stoIst1.DataBind();
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Sunucudan sayım tutanağı bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListe_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla());
        }

    }
}