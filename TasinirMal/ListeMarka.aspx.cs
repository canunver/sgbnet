using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Marka listesinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeMarka : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir
        ///     varsa markalar cevap (response) olarak döndürülür.
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

                MarkaDoldur(new Marka());
            }
        }

        /// <summary>
        /// Parametre olarak verilen marka kriterine uyan markalarý sayfadaki GridView kontrolüne dolduran yordam
        /// </summary>
        /// <param name="marka">Marka kriter bilgilerini tutan nesne</param>
        private void MarkaDoldur(Marka marka)
        {
            ObjectArray bilgi = servisTMM.MarkaListele(kullanan, marka);

            List<object> liste = new List<object>();
            if (!bilgi.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLMA002);

            foreach (TNS.TMM.Marka item in bilgi.objeler)
            {
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