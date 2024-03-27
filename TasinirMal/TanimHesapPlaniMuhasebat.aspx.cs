using System;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r hesap plan� bilgilerinin excel dosyas�ndan okunup kay�t i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class HesapPlaniMuhasebat : TMMSayfaV2
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
            formAdi = Resources.TasinirMal.FRMTHM001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giri� izni varm�?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            if (!IsPostBack)
            {
                ddlSablon_SelectedIndexChanged(null, null);
            }
        }

        /// <summary>
        /// Hesap Plan�n� G�ncelle tu�una bas�l�nca �al��an olay metodu
        /// Ta��n�r hesap plan� bilgileri verilen excel dosyas�ndan okunur ve kaydedilmek
        /// �zere sunucuya g�nderilir, gelen sonuca g�re hata veya bilgi mesaj� g�r�nt�lenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, System.EventArgs e)
        {
            //if (fileListe.PostedFile == null)
            //    return;
            //else
            //{
            //    System.Web.HttpPostedFile myFile = fileListe.PostedFile;
            //    string hata = "";
            //    string dosyaAd = "";
            //    byte[] myData = new byte[1024];
            //    int nFileLen = myFile.ContentLength;
            //    System.IO.FileStream newFile = null;
            //    if (myData.Length < 1000)
            //        hata = Resources.TasinirMal.FRMTHM002;
            //    if (nFileLen <= 0)
            //        hata = Resources.TasinirMal.FRMTHM003;

            //    dosyaAd = System.IO.Path.GetTempFileName();
            //    if (hata == "")
            //    {
            //        try
            //        {
            //            newFile = new System.IO.FileStream(dosyaAd, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);
            //        }
            //        catch
            //        {
            //            hata = Resources.TasinirMal.FRMTHM004;
            //        }
            //    }

            //    if (hata == "")
            //    {
            //        int okunan;
            //        try
            //        {
            //            do
            //            {
            //                okunan = myFile.InputStream.Read(myData, 0, myData.Length);
            //                if (myData != null)
            //                    newFile.Write(myData, 0, okunan);
            //            } while (okunan != 0);

            //            myFile.InputStream.Close();
            //        }
            //        catch
            //        {
            //            hata = Resources.TasinirMal.FRMTHM005;
            //        }
            //        newFile.Close();
            //    }

            //    if (hata == "")
            //    {
            //        if (dosyaAd != "")
            //            Kaydet(dosyaAd);
            //        else
            //            hata = Resources.TasinirMal.FRMTHM006;
            //    }

            //    if (dosyaAd != "")
            //        System.IO.File.Delete(dosyaAd);

            //    if (hata != "")
            //        GenelIslemler.MesajKutusu("Uyar�", hata);
            //}
        }

        /// <summary>
        /// Ta��n�r hesap plan� bilgileri verilen excel dosyas�ndan okunur ve kaydedilmek
        /// �zere sunucuya g�nderilir, gelen sonuca g�re hata veya bilgi mesaj�
        /// g�r�nt�lenir. Kay�t i�lemi do�ruysa ta��n�r hesap planlar�n�n detay
        /// say�lar�n� d�zenleyen HesapPlaniDetaySayilariDuzenle sunucu yordam� �a��r�l�r.
        /// </summary>
        /// <param name="sablonAd">Ta��n�r hesap plan� bilgilerini i�eren excel dosyas�n�n ad�</param>
        private void Kaydet(string sablonAd)
        {
            string hata = "";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyar�", hata + Resources.TasinirMal.FRMTHM007);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd);

            bool tamamlandi = false;
            int bosSatir = 0;

            int baslaSatir = OrtakFonksiyonlar.ConvertToInt(txtBaslaSatir.Text, 0) - 1;
            int hesapKodKolon = OrtakFonksiyonlar.ConvertToInt(txtHesapKodKolon.Text, 0) - 1;
            int hesapKodKolonSay = OrtakFonksiyonlar.ConvertToInt(txtHesapKodKolonSay.Text, 0) - 1;
            int aciklamaKolon = OrtakFonksiyonlar.ConvertToInt(txtAciklamaKolon.Text, 0) - 1;
            int olcuBirimiKolon = OrtakFonksiyonlar.ConvertToInt(txtOlcuBirimiKolon.Text, 0) - 1;
            int kdvKolon = OrtakFonksiyonlar.ConvertToInt(txtKDVKolon.Text, 0) - 1;
            int kullanilmiyorKolon = OrtakFonksiyonlar.ConvertToInt(txtKullanilmiyor.Text, 0) - 1;
            int guncellemeKolon = OrtakFonksiyonlar.ConvertToInt(txtGuncelleme.Text, 0) - 1;

            int satir = baslaSatir;

            OlcuBirim olcuBirim = new OlcuBirim();
            ObjectArray olcuBirimleri = servisTMM.OlcuBirimListele(kullanan, olcuBirim);

            ObjectArray hesapPlani = new ObjectArray();
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

            while (!tamamlandi)
            {
                if (bosSatir > 20)
                    break;

                HesapPlaniSatir hs = new HesapPlaniSatir();

                if (hesapKodKolon >= 0)
                {
                    hs.hesapKod = XLS.HucreDegerAl(satir, hesapKodKolon).Trim();
                    if (hs.hesapKod != "")
                    {
                        for (int i = 1; i <= hesapKodKolonSay; i++)
                            hs.hesapKod += XLS.HucreDegerAl(satir, hesapKodKolon + i).Trim();
                    }
                }
                if (aciklamaKolon >= 0)
                    hs.aciklama = XLS.HucreDegerAl(satir, aciklamaKolon).Trim().Replace("'", "");

                if (olcuBirimiKolon >= 0)
                {
                    hs.olcuBirimAd = XLS.HucreDegerAl(satir, olcuBirimiKolon).Trim().Replace("'", "");
                    foreach (OlcuBirim birim in olcuBirimleri.objeler)
                    {
                        if (birim.ad.ToUpper() == hs.olcuBirimAd.Trim().ToUpper())
                        {
                            hs.olcuBirim = birim.kod;
                            break;
                        }
                    }

                }
                if (kdvKolon >= 0)
                {
                    string kdv = XLS.HucreDegerAl(satir, kdvKolon).Trim().Replace("'", "");
                    if (kdv != "")
                        hs.kdv = OrtakFonksiyonlar.ConvertToInt(kdv, 0);
                }
                if (kullanilmiyorKolon >= 0)
                {
                    string kullanilmiyor = XLS.HucreDegerAl(satir, kullanilmiyorKolon).Trim().Replace("'", "");
                    if (kullanilmiyor != "")
                        hs.kullanilmiyor = OrtakFonksiyonlar.ConvertToInt(kullanilmiyor, 0);
                }
                if (guncellemeKolon >= 0)
                {
                    string guncelleme = XLS.HucreDegerAl(satir, guncellemeKolon).Trim().Replace("'", "");
                    if (guncelleme != "")
                        hs.guncelleme = OrtakFonksiyonlar.ConvertToInt(guncelleme, 0);
                }
                if (hs.hesapKod != "" && hs.aciklama != "")// && (hs.kdv > -1 || hs.olcuBirim > -1 || hs.kullanilmiyor > -1 || hs.guncelleme > -1))
                {
                    hs.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hs.hesapKod);
                    hesapPlani.objeler.Add(hs);
                    bosSatir = 0;
                }
                else
                    bosSatir++;

                satir++;
            }

            Sonuc sonuc = servisTMM.HesapPlaniKaydet(kullanan, hesapPlani, true, true);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyar�", sonuc.hataStr);
            else
            {
                servisTMM.HesapPlaniDetaySayilariDuzenle();
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTHM008);
            }
        }

        /// <summary>
        /// �ablon se�imi de�i�ti�inde �al��an olay metodu
        /// Se�ilen �ablona ait excel sat�r, s�tun numaralar� sayfadaki ilgili kontrollere doldurulur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void ddlSablon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSablon.SelectedValue == "1")
            {
                txtBaslaSatir.Text = "6";
                txtHesapKodKolon.Text = "2";
                txtHesapKodKolonSay.Text = "6";
                txtAciklamaKolon.Text = "8";
                txtOlcuBirimiKolon.Text = "0";
                txtKDVKolon.Text = "0";
                txtKullanilmiyor.Text = "0";
                txtGuncelleme.Text = "0";
            }
            if (ddlSablon.SelectedValue == "2")
            {
                txtBaslaSatir.Text = "5";
                txtHesapKodKolon.Text = "1";
                txtHesapKodKolonSay.Text = "1";
                txtAciklamaKolon.Text = "2";
                txtOlcuBirimiKolon.Text = "3";
                txtKDVKolon.Text = "4";
                txtKullanilmiyor.Text = "5";
                txtGuncelleme.Text = "6";
            }
            if (ddlSablon.SelectedValue == "3")
            {
                txtBaslaSatir.Text = "0";
                txtHesapKodKolon.Text = "0";
                txtHesapKodKolonSay.Text = "0";
                txtAciklamaKolon.Text = "0";
                txtOlcuBirimiKolon.Text = "0";
                txtKDVKolon.Text = "0";
                txtKullanilmiyor.Text = "0";
                txtGuncelleme.Text = "0";
            }
        }
    }
}