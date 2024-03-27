using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþinin veya zimmet fiþinin durum tarihçelerinin listeleme iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class BelgeTarihce : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Sayfa adresinde gelen yil, belgeNo, muhasebe, harcamaBirimi ve tarihceTur girdi dizgilerini alýp, eksik
        ///     bilgi yoksa tarihceTur girdi dizgisine bakarak TarihceListeTasinir veya TarihceListeZimmet yordamýný çaðýrýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
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
        /// Ýstenen taþýnýr iþlem fiþinin durum tarihçesi bilgilerini sunucudan alýp ekrana yazan yordam
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
                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && ((sayi != tVAN.objeler.Count && t.islem == "Onaylandý") || (t.onayDurum != (int)ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI && t.islem == "Onaylandý")))
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
        /// Ýstenen zimmet fiþinin durum tarihçesi bilgilerini sunucudan alýp ekrana yazan yordam
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