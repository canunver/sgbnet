using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r i�lem fi�inin veya zimmet fi�inin durum tarih�elerinin listeleme i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class BelgeTarihce : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Sayfa adresinde gelen yil, belgeNo, muhasebe, harcamaBirimi ve tarihceTur girdi dizgilerini al�p, eksik
        ///     bilgi yoksa tarihceTur girdi dizgisine bakarak TarihceListeTasinir veya TarihceListeZimmet yordam�n� �a��r�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            if (!IsPostBack)
            {
                int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);
                string belgeNo = Request.QueryString["belgeNo"] + "";
                string muhasebe = Request.QueryString["muhasebe"] + "";
                string harcamaBirimi = Request.QueryString["harcamaBirimi"] + "";
                string tarihceTur = Request.QueryString["tarihceTur"] + "";

                if (yil == 0 || muhasebe == "" || harcamaBirimi == "" || belgeNo.Trim() == "")
                {
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMBTR001);
                    return;
                }

                if (tarihceTur == "tif" || tarihceTur == "")
                    TarihceListeTasinir();
                else if (tarihceTur == "zimmet")
                    TarihceListeZimmet();
            }
        }

        /// <summary>
        /// �stenen ta��n�r i�lem fi�inin durum tarih�esi bilgilerini sunucudan al�p ekrana yazan yordam
        /// </summary>
        void TarihceListeTasinir()
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);
            string belgeNo = Request.QueryString["belgeNo"] + "";
            string muhasebe = Request.QueryString["muhasebe"] + "";
            string harcamaBirimi = Request.QueryString["harcamaBirimi"] + "";

            ObjectArray tVAN = servisTMM.IslemFormTarihceListele(kullanan, yil, muhasebe, harcamaBirimi, belgeNo);

            List<object> storeListe = new List<object>();

            if (tVAN.sonuc.islemSonuc)
            {
                lblYil.Text = yil.ToString();
                lblMuhasebe.Text = muhasebe;
                lblHarcamaBirimi.Text = harcamaBirimi;
                lblBelgeNo.Text = belgeNo;

                int sayi = 0;
                foreach (TarihceBilgisi t in tVAN.objeler)
                {
                    sayi++;
                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && ((sayi != tVAN.objeler.Count && t.islem == "Onayland�") || (t.onayDurum != (int)ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI && t.islem == "Onayland�")))
                        t.islem = "Numara Verildi";

                    storeListe.Add(new { islemTarihi = t.islemTarih.ToLongString(), islem = t.islem, islemYapan = t.islemYapan });
                }
            }
            else
                GenelIslemler.MesajKutusu("Hata", tVAN.sonuc.hataStr);

            tarihceStore.DataSource = storeListe;
            tarihceStore.DataBind();
        }

        /// <summary>
        /// �stenen zimmet fi�inin durum tarih�esi bilgilerini sunucudan al�p ekrana yazan yordam
        /// </summary>
        void TarihceListeZimmet()
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);
            string belgeNo = Request.QueryString["belgeNo"] + "";
            string muhasebe = Request.QueryString["muhasebe"] + "";
            string harcamaBirimi = Request.QueryString["harcamaBirimi"] + "";
            int belgeTur = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["belgeTur"] + "", 0);

            ObjectArray tVAN = servisTMM.ZimmetTarihceListele(kullanan, yil, muhasebe, harcamaBirimi, belgeNo, belgeTur);

            List<object> storeListe = new List<object>();

            if (tVAN.sonuc.islemSonuc)
            {
                lblYil.Text = yil.ToString();
                lblMuhasebe.Text = muhasebe;
                lblHarcamaBirimi.Text = harcamaBirimi;
                lblBelgeNo.Text = belgeNo;


                foreach (TarihceBilgisi t in tVAN.objeler)
                    storeListe.Add(new { islemTarihi = t.islemTarih.ToLongString(), islem = t.islem, islemYapan = t.islemYapan });
            }
            else
                GenelIslemler.MesajKutusu("Hata", tVAN.sonuc.hataStr);

            tarihceStore.DataSource = storeListe;
            tarihceStore.DataBind();
        }
    }
}