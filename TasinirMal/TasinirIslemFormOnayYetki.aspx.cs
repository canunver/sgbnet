using Ext1.Net;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;
using System.IO;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r i�lem fi�i bilgilerinin onay yetkisi i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TasinirIslemFormOnayYetki : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!X.IsAjaxRequest)
            {
                formAdi = "B-A Onay Yetki Tan�mlama Sayfas�";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                if (Request.QueryString["menuYok"] == "1")
                {
                    pnlAna.Margins = "0 0 0 0";
                    pnlAna.StyleSpec += "padding:5px";
                }

                btnListele_Click(null, null);
            }

        }

        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            strListeA.DataSource = ListeGetir(ENUMTasinirIslemFormOnayTur.AONAYI);
            strListeA.DataBind();

            strListeB.DataSource = ListeGetir(ENUMTasinirIslemFormOnayTur.BONAYI);
            strListeB.DataBind();
        }

        private DataTable ListeGetir(ENUMTasinirIslemFormOnayTur tur)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("muhasebeKod")   { DataType = typeof(string) },
                new DataColumn("harcamaBirimKod")   { DataType = typeof(string) },
                new DataColumn("mernis")   { DataType = typeof(string) },
                new DataColumn("adSoyad")   { DataType = typeof(string) },
                new DataColumn("onayTur")     { DataType = typeof(string) }
           });

            ObjectArray bilgi = servisTMM.TasinirIslemFisiOnayYetkiListele(kullanan, new TasinirIslemOnayYetki() { muhasebeKod = txtMuhasebe.Text, harcamaBirimKod = txtHarcamaBirimi.Text, onayTur = (int)tur });
            if (bilgi != null)
            {
                if (bilgi.sonuc.islemSonuc)
                {
                    foreach (TNS.TMM.TasinirIslemOnayYetki yetki in bilgi.objeler)
                    {
                        string onayTur = "";
                        if (yetki.onayTur == (int)ENUMTasinirIslemFormOnayTur.AONAYI)
                            onayTur = "A Onay�";
                        else if (yetki.onayTur == (int)ENUMTasinirIslemFormOnayTur.BONAYI)
                            onayTur = "B Onay�";

                        dt.Rows.Add(yetki.muhasebeKod, yetki.harcamaBirimKod, yetki.mernis, yetki.adSoyad, onayTur);
                    }
                }
                else
                    GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr + bilgi.sonuc.vtHatasi);
            }

            return dt;
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            JArray jArrayA = (JArray)JSON.Deserialize(e.ExtraParams["jsonA"]);
            JArray jArrayB = (JArray)JSON.Deserialize(e.ExtraParams["jsonB"]);

            List<TasinirIslemOnayYetki> listeYetki = new List<TasinirIslemOnayYetki>();

            foreach (JContainer jc in jArrayA)
            {
                TasinirIslemOnayYetki yetki = new TasinirIslemOnayYetki
                {
                    muhasebeKod = txtMuhasebe.Text,
                    harcamaBirimKod = txtHarcamaBirimi.Text,
                    mernis = jc.Value<string>("mernis"),
                    adSoyad = jc.Value<string>("adSoyad"),
                    onayTur = (int)ENUMTasinirIslemFormOnayTur.AONAYI
                };

                listeYetki.Add(yetki);
            }

            foreach (JContainer jc in jArrayB)
            {
                TasinirIslemOnayYetki yetki = new TasinirIslemOnayYetki
                {
                    muhasebeKod = txtMuhasebe.Text,
                    harcamaBirimKod = txtHarcamaBirimi.Text,
                    mernis = jc.Value<string>("mernis"),
                    adSoyad = jc.Value<string>("adSoyad"),
                    onayTur = (int)ENUMTasinirIslemFormOnayTur.BONAYI
                };

                listeYetki.Add(yetki);
            }

            Sonuc sonuc = servisTMM.TasinirIslemFisiOnayYetkiKaydet(kullanan, txtMuhasebe.Text, txtHarcamaBirimi.Text, listeYetki.ToArray());
            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
                GenelIslemler.ExtNotification(sonuc.bilgiStr, "Bilgi", Icon.LightningGo);

        }
    }
}