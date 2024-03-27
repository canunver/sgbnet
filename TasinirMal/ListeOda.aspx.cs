using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;
using System.Collections;

namespace TasinirMal
{
    /// <summary>
    /// Oda listesinin verilen kritere g�re d�nd�r�l�p listelendi�i sayfa
    /// </summary>
    public partial class ListeOda : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur, yoksa giri� ekran�na y�nlendirilir varsa sayfa y�klenir.
        ///     Sayfa adresinde gelen mb, hb ve ab girdi dizgileri kullan�larak odalar cevap (response) olarak d�nd�r�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";

                string kisiKimlikNo = Request.QueryString["kk"] + "";

                if (kisiKimlikNo != "") KisininOdalari(kisiKimlikNo);
            }
        }

        private void KisininOdalari(string kisiKimlikNo)
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.muhasebeKod = Request.QueryString["mb"] + "";
            shBilgi.harcamaBirimKod = Request.QueryString["hb"] + "";
            shBilgi.kimeGitti = kisiKimlikNo;

            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());

            List<object> liste = new List<object>();
            Hashtable htOda = new Hashtable();
            foreach (SicilNoHareket dt in bilgi.objeler)
            {
                if (htOda[dt.odaKod] != null)
                    continue;

                htOda[dt.odaKod] = dt.odaAd;

                liste.Add(new
                {
                    KOD = dt.odaKod,
                    ADI = dt.odaAd,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        [DirectMethod]
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            Oda oda = new Oda();
            string muhasebeKod = Request.QueryString["mb"] + "";
            string harcamaBirimKod = Request.QueryString["hb"] + "";

            if (rdKurum.Checked)
            {
                oda.muhasebeKod = string.Empty;
                oda.harcamaBirimKod = string.Empty;
            }
            else if (rdMuhasebe.Checked)
            {
                oda.muhasebeKod = muhasebeKod;
                oda.harcamaBirimKod = string.Empty;
            }
            else if (rdHarcamaBirimi.Checked)
            {
                oda.muhasebeKod = muhasebeKod;
                oda.harcamaBirimKod = harcamaBirimKod;
            }
            else
            {
                oda.muhasebeKod = muhasebeKod;
                oda.harcamaBirimKod = harcamaBirimKod;
            }
            oda.harcamaBirimKod = harcamaBirimKod;
            oda.ad = txtFiltre.Text;
            ObjectArray bilgi = servisTMM.OdaListele(kullanan, oda);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLOD002);

            List<object> liste = new List<object>();
            foreach (Oda o in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = o.kod,
                    ADI = o.ad,
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }
    }
}