using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Firma listesinin verilen kritere g�re d�nd�r�l�p listelendi�i sayfa
    /// </summary>
    public partial class ListeFirma : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur, yoksa giri� ekran�na y�nlendirilir varsa sayfa y�klenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //Giri� s�ras�nda kullan�c�n�n varl���n� kontrol et yoksa sayfaya girme
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            txtAd.Focus();
        }

        /// <summary>
        /// Bul tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan verilen kriterlere uygun olan firma bilgileri al�n�r ve dgListe DataGrid kontrol�ne doldurulur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnBul_Click(object sender, EventArgs e)
        {
            ObjectArray fv = new ObjectArray();

            //if (txtAd.Text.Trim() == "")// || txtAd.Text.Trim().Length < 3)
            //{
            //    GenelIslemler.MesajKutusu(this, "L�tfen Firma aramas� i�in <b>Unvan alan�na</b> arama kriteri girilmelidir.");
            //    return;
            //}

            dgListe.DataBind();

            FirmaBilgisi fSorgu = new FirmaBilgisi();
            fSorgu.ad = txtAd.Text;

            if (rdKendi.Checked)
                fSorgu.kullaniciKodu = kullanan.kullaniciKodu;

            fv = servisTMM.FirmaListele(fSorgu);

            if (fv.sonuc.islemSonuc)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ad");

                foreach (FirmaBilgisi f in fv.objeler)
                {
                    string ad = f.ad;
                    if (f.vno != "")
                        ad += " (" + f.vno + ")";

                    dt.Rows.Add("<a href='#' onclick=\"VeriGoster('" + f.ad.Replace("'", "") + "|" + f.vno.Replace("'", "") + "|" + f.vd.Replace("'", "") + "|" + f.banka.Replace("'", "") + "|" + f.hesapNo.Replace("'", "") + "');return false;\">" + ad + "</a>");
                }

                dgListe.DataSource = dt;
                dgListe.DataBind();
            }
        }
    }
}