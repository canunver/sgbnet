using Ext1.Net;
using OrtakClass;
using System.Data;
using TNS;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr demirbaþlarýnýn hareket tarihçe bilgilerinin listeleme iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class SicilNoTarihce : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMSCT001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
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
        /// Listeleme kriterleri SicilNoHareket nesnesinde parametre olarak alýnýr, sunucuya
        /// gönderilir ve demirbaþ hareket tarihçe bilgileri sunucudan alýnýr. Hata varsa
        /// ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="sicilNo">Demirbaþ hareket tarihçe listeleme kriterlerini tutan nesne</param>
        private void Listele(SicilNoHareket sicilNo)
        {
            ObjectArray yler = servisTMM.SicilNoTarihceListele(kullanan, sicilNo);

            TNSCollection prSiciller = new TNSCollection();
            if (!yler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", yler.sonuc.hataStr);
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
        /// Bul tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandaki kriterler toplanýr ve Listele yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
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