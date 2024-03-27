using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Harcama birimi listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeHarcamaBirimi : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Sayfa adresinde gelen mb girdi dizgisi kullanýlarak
        ///     harcama birimleri cevap (response) olarak döndürülür.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";
                htnButun.Value = Request.QueryString["butun"] + "";

                HarcamaBirimi harcama = new HarcamaBirimi();
                harcama.muhasebeKod = Request.QueryString["mb"] + "";
                harcama.kapali = 0;

                HarcamaBirimiDoldur(harcama);
            }
        }

        /// <summary>
        /// Parametre olarak verilen harcama birimi kriterine uyan harcama birimlerini sayfadaki GridView kontrolüne dolduran yordam
        /// </summary>
        /// <param name="harcama">Harcama birimi kriter bilgilerini tutan nesne</param>
        private void HarcamaBirimiDoldur(HarcamaBirimi harcama)
        {
            ObjectArray bilgi = servisTMM.HarcamaBirimiListele(kullanan, harcama);

            List<object> liste = new List<object>();
            if (!bilgi.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLHB001);

            bool yetki = false;
            bool butun = true;// OrtakFonksiyonlar.ConvertToInt(htnButun.Value, 0) > 0;

            foreach (HarcamaBirimi item in bilgi.objeler)
            {
                if (!butun)
                {
                    yetki = servisTMM.GormeYetkisiVarMi(kullanan, item.muhasebeKod, item.kod, "");
                    if (!yetki) continue;
                }

                liste.Add(new
                {
                    KOD = item.kod,
                    ADI = item.ad,
                    MUHASEBEKOD = item.muhasebeKod,
                    MUHASEBEADI = item.muhasebeAd,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }
    }
}