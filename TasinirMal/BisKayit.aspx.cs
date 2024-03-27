using System;
using Ext1.Net;
using OrtakClass;
using System.Data;
using TNS;
using TNS.TMM;
using TNS.UZY;
using System.Collections.Generic;

namespace TasinirMal
{
    public partial class BisKayit : TMMSayfaV2
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
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "�li�kili Malzeme ��lemleri";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                if (!string.IsNullOrEmpty(Request.QueryString["prSicilNo"]))
                {
                    SicilNoHareket s = new SicilNoHareket();
                    s.prSicilNo = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["prSicilNo"], 0);
                    Listele(s);
                }
            }
        }

        /// <summary>
        /// Listeleme kriterleri SicilNoHareket nesnesinde parametre olarak al�n�r, sunucuya
        /// g�nderilir ve demirba� hareket tarih�e bilgileri sunucudan al�n�r. Hata varsa
        /// ekrana hata bilgisi yaz�l�r, yoksa gelen bilgiler GridView kontrol�ne doldurulur.
        /// </summary>
        /// <param name="sicilNo">Demirba� hareket tarih�e listeleme kriterlerini tutan nesne</param>
        private void Listele(SicilNoHareket sicilNo)
        {
            ObjectArray yler = servisTMM.BisSicilListele(kullanan, sicilNo); //****

            if (!yler.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyar�", yler.sonuc.hataStr);
            else if (yler.objeler.Count == 0)
                GenelIslemler.MesajKutusu("Bilgi", "�li�kili malzeme bulunamad�");

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("PRSICILNO")   { DataType = typeof(string) },
                new DataColumn("SICILNO")   { DataType = typeof(string) },
                new DataColumn("TASINIRHESAPKOD")   { DataType = typeof(string) },
                new DataColumn("TASINIRHESAPADI")   { DataType = typeof(string) },
            });

            foreach (SicilNoHareket sh in yler.objeler)
                dt.Rows.Add(sh.prSicilNo, sh.sicilNo, sh.hesapPlanKod, sh.hesapPlanAd);

            strListe.DataSource = dt;
            strListe.DataBind();
        }


        /// <summary>
        /// Bul tu�una bas�l�nca �al��an olay metodu
        /// Ekrandaki kriterler toplan�r ve Listele yordam� �a��r�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListe_Click(object sender, System.EventArgs e)
        {
            SicilNoHareket y = new SicilNoHareket();
            y.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            y.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            y.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            y.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim().Replace(".", "");
            y.eSicilNo = pgFiltre.Source["prpEskiSicilNo"].Value.Trim().Replace(".", "");

            string hata = "";
            if (y.muhasebeKod == "")
                hata += Resources.TasinirMal.FRMSCO004;
            if (y.harcamaBirimKod == "")
                hata += Resources.TasinirMal.FRMSCO005;
            if (y.ambarKod == "")
                hata += Resources.TasinirMal.FRMSCO006;
            if (y.sicilNo == "" && y.eSicilNo == "")
                hata += "L�tfen ili�kilendirmek istedi�iniz malzemenin sicil numaras�n� giriniz!";

            if (hata != "")
                GenelIslemler.MesajKutusu("Uyar�", hata);
            else
                Listele(y);
        }

        protected void btnKaydet_Click(Object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            SicilNoHareket y = new SicilNoHareket();
            y.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            y.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            y.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            y.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim().Replace(".", "");

            List<TNS.TMM.SicilNoOzellik> liste = new List<TNS.TMM.SicilNoOzellik>();

            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                TNS.TMM.SicilNoOzellik so = new TNS.TMM.SicilNoOzellik();

                so.prSicilno = TasinirGenel.DegerAlInt(item, "PRSICILNO");

                liste.Add(so);
            }

            Sonuc sonuc = servisTMM.BisKaydet(kullanan, y.yil, y.muhasebeKod, y.harcamaBirimKod, y.ambarKod, y.sicilNo, 0, liste.ToArray());
            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyar�", sonuc.hataStr);
            else
                GenelIslemler.ExtNotification(sonuc.bilgiStr, "Bilgi", Icon.Information);
            btnListe_Click(null, null);
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
        }

    }
}