using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr sarf malzemeleri listesinin verilen kriterlere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeStokYeni : TMMSayfaV2
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
        protected void Page_Load(object sender, EventArgs e)
        {
            //Giriþ sýrasýnda kullanýcýnýn varlýðýný kontrol et yoksa sayfaya girme
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            if (!X.IsAjaxRequest)
            {
                formAdi = "Tüketim Malzemesi Seçim Formu";
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";

                pgFiltre.UpdateProperty("prpHesapKod", Request.QueryString["hpk"] + "");
            }
        }

        /// <summary>
        /// Sayfa adresinde gelen girdi dizgilerindeki ve sayfadaki kontrollerdeki kriterler toplanýr
        /// ve kriterlere uygun olan sarf malzemeleri listelenmek üzere StokDoldur yordamý çaðýrýlýr.
        /// </summary>
        void KriterTopla()
        {
            StokHareketBilgi shBilgi = new StokHareketBilgi();

            //int islemTur = 0;
            //islemTur = OrtakFonksiyonlar.ConvertToInt((Request.QueryString["it"] + "").Replace(".", ""), 0);

            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);
            shBilgi.muhasebeKod = Request.QueryString["mb"] + "";
            shBilgi.harcamaKod = Request.QueryString["hb"] + "";
            shBilgi.ambarKod = Request.QueryString["ak"] + "";
            shBilgi.hesapPlanKod = pgFiltre.Source["prpHesapKod"].Value.Trim();
            shBilgi.hesapPlanAd = pgFiltre.Source["prpHesapAdi"].Value.Trim();

            StokDoldur(shBilgi);
        }

        /// <summary>
        /// Parametre olarak verilen kriterlere uyan sarf malzemelerini sayfadaki GridView kontrolüne dolduran yordam
        /// </summary>
        /// <param name="shBilgi">Sarf malzemeleri listeleme kriter bilgilerini tutan nesne</param>
        private void StokDoldur(StokHareketBilgi shBilgi)
        {
            ObjectArray bilgi = servisTMM.TuketimListele(kullanan, shBilgi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
            }
            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLST007);
            }

            List<object> liste = new List<object>();
            foreach (StokHareketBilgi sBilgi in bilgi.objeler)
            {
                liste.Add(new
                {
                    HESAPPLANKOD = sBilgi.hesapPlanKod,
                    HESAPPLANADI = sBilgi.hesapPlanAd,
                    HESAPPLANKODADI = sBilgi.hesapPlanKod + "-" + sBilgi.hesapPlanAd,
                    MIKTAR = sBilgi.miktar,
                    KDVORAN = sBilgi.kdvOran,
                    BIRIMFIYAT = sBilgi.birimFiyat.ToString(),
                    OLCUBIRIM = sBilgi.olcuBirimAdi
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr sarf malzemeleri listelensin diye KriterTopla yordamýný çaðýrýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListe_Click(object sender, DirectEventArgs e)
        {
            KriterTopla();
        }
        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
        }

        [DirectMethod]
        public void Sorgula(string hesapKod)
        {
            StokHareketBilgi shBilgi = new StokHareketBilgi();

            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);
            shBilgi.muhasebeKod = Request.QueryString["mb"] + "";
            shBilgi.harcamaKod = Request.QueryString["hb"] + "";
            shBilgi.ambarKod = Request.QueryString["ak"] + "";
            shBilgi.hesapPlanKod = hesapKod;

            StokDoldur(shBilgi);
        }
    }
}