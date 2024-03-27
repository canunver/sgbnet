using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// İhtiyaç fazlası taşınır demirbaşlarının istek tarihçelerinin listeleme işleminin yapıldığı sayfa
    /// </summary>
    public partial class AcikPazarIstekTarihce : istemciUzayi.GenelSayfa
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur ve demirbaşın istek tarihçe
        ///     bilgilerinin listelenmesi için Listele yordamı çağırılır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            if (!IsPostBack)
            {
                Listele();
            }
        }

        /// <summary>
        /// muhasebeKod, harcamaKod ve prSicilNo listeleme kriterleri sayfa adresindeki girdi dizgilerinden okunur,
        /// ihtiyaç fazlası taşınır istek nesnesine doldurulur ve sunucuya gönderilir. Hata varsa ekrana hata
        /// bilgisi yazılır, yoksa sunucudan gelen istek tarihçe bilgileri dgListe DataGrid kontrolüne doldurulur.
        /// </summary>
        private void Listele()
        {
            TNS.TMM.AcikPazarIstek istek = new TNS.TMM.AcikPazarIstek();
            istek.muhasebeKod = Request.QueryString["muhasebeKod"].ToString();
            istek.harcamaKod = Request.QueryString["harcamaKod"].ToString();
            istek.prSicilNo = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["prSicilNo"].ToString(), 0);

            ObjectArray istekler = servisTMM.AcikPazarIstekListele(kullanan, istek);

            if (!istekler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, istekler.sonuc.hataStr);
                dgListe.DataBind();
                return;
            }

            if (istekler.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu(this, istekler.sonuc.bilgiStr);
                dgListe.DataBind();
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add("SiraNo");
            table.Columns.Add("MuhasebeKod");
            table.Columns.Add("MuhasebeAd");
            table.Columns.Add("HarcamaKod");
            table.Columns.Add("HarcamaAd");
            table.Columns.Add("IstekTarihi");

            int siraNo = 1;
            foreach (TNS.TMM.AcikPazarIstek i in istekler.objeler)
            {
                table.Rows.Add(siraNo, i.istekYapanMuhasebeKod, i.istekYapanMuhasebeAd, i.istekYapanHarcamaKod,
                               i.istekYapanHarcamaAd, i.istekTarihi);

                siraNo++;
            }

            dgListe.DataSource = table;
            dgListe.DataBind();
        }
    }
}