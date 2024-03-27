using Ext1.Net;
using OrtakClass;
using System.Data;
using TNS;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r demirba�lar�n�n hareket tarih�e bilgilerinin listeleme i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class SicilNoTarihce : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

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
                formAdi = Resources.TasinirMal.FRMSCT001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

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
            ObjectArray yler = servisTMM.SicilNoTarihceListele(kullanan, sicilNo);

            TNSCollection prSiciller = new TNSCollection();
            if (!yler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", yler.sonuc.hataStr);
                return;
            }
            if (yler.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMSCT002);
                strListe.DataBind();
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("SICILNO");
            dt.Columns.Add("SICILAD");
            dt.Columns.Add("ESKISICILNO");
            dt.Columns.Add("TARIH");
            dt.Columns.Add("MUHASEBEBIRIMIKOD");
            dt.Columns.Add("HARCAMABIRIMIKOD");
            dt.Columns.Add("AMBAR");
            dt.Columns.Add("BELGENO");
            dt.Columns.Add("FIYATI");
            dt.Columns.Add("ISLEM");

            foreach (SicilNoHareket sh in yler.objeler)
            {
                string islemAciklama = "";
                if (sh.kimeGitti != "")
                {
                    string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", sh.kimeGitti, true, "");
                    if (string.IsNullOrEmpty(ad))
                        islemAciklama = sh.kimeGitti;
                    else
                        islemAciklama = ad;
                }
                if (sh.nereyeGitti != "")
                {
                    if (islemAciklama != "")
                        islemAciklama += ", ";

                    string oda = servisUZY.UzayDegeriStr(null, "TASODA", sh.muhasebeKod + "-" + sh.harcamaBirimKod + "-" + sh.nereyeGitti, true, "");
                    if (string.IsNullOrEmpty(oda))
                        islemAciklama += sh.odaAd;
                    else
                        islemAciklama += oda;
                }
                if (islemAciklama != "")
                    islemAciklama = "(" + islemAciklama + ")";

                if (sh.islemTurKod == (int)ENUMIslemTipi.ENFLASYONARTISI)
                    sh.fiyat = (sh.kdvliBirimFiyat + sh.degerArtisi) / (1 + (((decimal)sh.kdvOran) / 100));


                dt.Rows.Add(sh.sicilNo + " - " + sh.rfIdNo, sh.hesapPlanAd, sh.eSicilNo, sh.fisTarih, sh.muhasebeKod, sh.harcamaBirimKod, sh.ambarKod, sh.fisNo, sh.fiyat.ToString("#,###.0000"), sh.islemTipiAd + islemAciklama);
                prSiciller.Add(sh.prSicilNo);
            }
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

            Listele(y);
        }
        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
        }
    }
}