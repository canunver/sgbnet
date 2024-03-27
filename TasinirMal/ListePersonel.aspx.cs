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
    /// Personel listesinin verilen kritere g�re d�nd�r�l�p listelendi�i sayfa
    /// </summary>
    public partial class ListePersonel : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Sayfa adresinde gelen mb ve hb girdi dizgileri kullan�larak personeller cevap (response) olarak d�nd�r�l�r.
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
            }
        }

        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.Personel personel = new TNS.TMM.Personel();
            string muhasebeKod = Request.QueryString["mb"] + "";
            string harcamaBirimKod = Request.QueryString["hb"] + "";

            if (rdKurum.Checked)
            {
                personel.muhasebeKod = string.Empty;
                personel.harcamaBirimKod = string.Empty;
            }
            else if (rdMuhasebe.Checked)
            {
                personel.muhasebeKod = muhasebeKod;
                personel.harcamaBirimKod = string.Empty;
            }
            else
            {
                personel.muhasebeKod = muhasebeKod;
                personel.harcamaBirimKod = harcamaBirimKod;
            }

            personel.ad = txtFiltre.Text.Trim();

            if (string.IsNullOrEmpty(personel.ad))
            {
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLPR003);
                return;
            }

            ObjectArray bilgi = servisTMM.PersonelListele(kullanan, personel);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLPR002);

            //if (hdnCagiran.Value == "GRID")
            //    dt.Rows.Add(per.kod, "<a href='#' onclick=\"parent.PersonelListesindenGrideYaz('" + per.kod + "','" + per.ad + "');window.parent.hidePopWin();\">" + per.ad + "</a>", per.unvan + " / " + per.oda, per.harcamaBirimKod);
            //else
            //    dt.Rows.Add(per.kod, "<a href='#' onclick=\"SecKapatDeger('" + per.kod + "','" + per.ad + "');\">" + per.ad + "</a>", per.unvan + " / " + per.oda, per.harcamaBirimKod);

            List<object> liste = new List<object>();
            foreach (TNS.TMM.Personel per in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = per.kod,
                    ADI = per.ad,
                    UNVAN = per.unvan,
                    ODAKOD = per.oda,
                    BIRIMKOD = per.harcamaBirimKod

                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

    }
}