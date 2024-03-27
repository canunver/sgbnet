using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Firma listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeFirma : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir varsa sayfa yüklenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //Giriþ sýrasýnda kullanýcýnýn varlýðýný kontrol et yoksa sayfaya girme
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            txtAd.Focus();
        }

        /// <summary>
        /// Bul tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan verilen kriterlere uygun olan firma bilgileri alýnýr ve dgListe DataGrid kontrolüne doldurulur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnBul_Click(object sender, EventArgs e)
        {
            ObjectArray fv = new ObjectArray();

            //if (txtAd.Text.Trim() == "")// || txtAd.Text.Trim().Length < 3)
            //{
            //    GenelIslemler.MesajKutusu(this, "Lütfen Firma aramasý için <b>Unvan alanýna</b> arama kriteri girilmelidir.");
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