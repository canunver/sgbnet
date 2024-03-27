using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Ambar listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeAmbar : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Sayfa adresinde gelen mb ve hb girdi dizgileri kullanýlarak
        ///     ambarlar cevap (response) olarak döndürülür.
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

                Ambar amb = new Ambar();
                amb.muhasebeKod = Request.QueryString["mb"] + "";
                amb.harcamaBirimKod = Request.QueryString["hb"] + "";
                amb.kapali = 0;

                AmbarDoldur(amb);
            }
        }

        /// <summary>
        /// Parametre olarak verilen ambar kriterine uyan ambarlarý sayfadaki GridView kontrolüne dolduran yordam
        /// </summary>
        /// <param name="ambar">Ambar kriter bilgilerini tutan nesne</param>
        private void AmbarDoldur(Ambar ambar)
        {
            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, ambar);

            List<object> liste = new List<object>();
            if (!bilgi.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLAM001);

            bool yetki = false;
            bool butun = OrtakFonksiyonlar.ConvertToInt(htnButun.Value, 0) > 0;

            foreach (Ambar item in bilgi.objeler)
            {
                if (!butun)
                {
                    yetki = servisTMM.GormeYetkisiVarMi(kullanan, item.muhasebeKod, item.harcamaBirimKod, item.kod);
                    if (!yetki) continue;
                }

                liste.Add(new
                {
                    KOD = item.kod,
                    ADI = item.ad,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }
    }
}