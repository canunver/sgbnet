using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.IO;
using TNS;
using TNS.DEG;
using TNS.TMM;

namespace TasinirMal
{
    public partial class TasinirIslemDosya : TMMSayfaV2
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        ///     Sayfa ilk defa çağırılıyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "İşlem Dosya";
                //SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnYil.Value = Request.QueryString["yil"] + "";
                hdnMuhasebeKod.Value = Request.QueryString["mb"] + "";
                hdnHarcamaBirimKod.Value = Request.QueryString["hb"] + "";
                hdnFisNo.Value = Request.QueryString["belgeNo"] + "";

                DosyaListele();
            }
        }

        protected void ResimKayit(object sender, DirectEventArgs e)
        {
            string fileName = fileDosya.PostedFile.FileName;

            int intLength = Convert.ToInt32(fileDosya.PostedFile.InputStream.Length);
            byte[] dosyaByte = new byte[intLength];
            fileDosya.PostedFile.InputStream.Read(dosyaByte, 0, intLength);

            //TasResim resim = ResimBoyutlandir(resimByte, System.Drawing.RotateFlipType.RotateNoneFlipNone, 1280, 1280);
            TNS.TMM.IslemDosya bilgi = new TNS.TMM.IslemDosya();
            bilgi.resim = dosyaByte;

            bilgi.yil = OrtakFonksiyonlar.ConvertToInt(hdnYil.Value, 0);
            bilgi.muhasebeKod = hdnMuhasebeKod.Value.ToString();
            bilgi.harcamaBirimKod = hdnHarcamaBirimKod.Value.ToString().Trim().Replace(".", "");
            bilgi.fisNo = hdnFisNo.Value.ToString().Trim().PadLeft(6, '0');
            bilgi.siraNo = 1;
            bilgi.adi = fileDosya.PostedFile.FileName;
            bilgi.boyutu = fileDosya.PostedFile.ContentLength;
            bilgi.ekleyenKisiKod = kullanan.mernis;
            bilgi.ekleyenKisiAdi = kullanan.ADSOYAD;
            bilgi.eklemeTarihi = new TNSDateTime(DateTime.Now);

            string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
            bilgi.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirIslemBelgesi");

            servisTMM.IslemDosyaKaydet(bilgi);
            DosyaListele();
        }

        private void DosyaListele()
        {
            TNS.TMM.IslemDosya bilgi = new TNS.TMM.IslemDosya();
            bilgi.yil = OrtakFonksiyonlar.ConvertToInt(hdnYil.Value.ToString(), 0);
            bilgi.fisNo = hdnFisNo.Value.ToString();
            bilgi.muhasebeKod = hdnMuhasebeKod.Value.ToString().Replace(".", "");
            bilgi.harcamaBirimKod = hdnHarcamaBirimKod.Value.ToString().Replace(".", "");

            List<object> resimler = new List<object>();
            ObjectArray liste = servisTMM.IslemDosyaGetir(bilgi);

            if (liste.sonuc.islemSonuc)
            {
                foreach (TNS.TMM.IslemDosya dosya in liste.objeler)
                {
                    resimler.Add(new
                    {
                        DOSYAID = dosya.dosyaKod,
                        YIL = dosya.yil,
                        MUHASEBEKOD = dosya.muhasebeKod,
                        HARCAMABIRIMKOD = dosya.harcamaBirimKod,
                        FISNO = dosya.fisNo,
                        SIRANO = dosya.siraNo,
                        ADI = dosya.adi,
                        BOYUTU = (dosya.boyutu / 1000).ToString("#,###KB"),
                        EKLEYENKISIAD = dosya.ekleyenKisiAdi,
                        EKLEYENKISIKOD = dosya.ekleyenKisiKod,
                        EKLEMETARIHI = dosya.eklemeTarihi.ToString()
                    });
                }
            }

            strListe.DataSource = resimler.ToArray();
            strListe.DataBind();
        }


        [DirectMethod]
        public void SatirSil(int dosyaID)
        {
            string note = string.Empty;
            note = "Silme işlemi başarılı.";
            if (dosyaID == 0)
            {
                GenelIslemler.MesajKutusu("Uyarı", "Lütfen silmek istenilen resmi seçiniz.");
                return;
            }

            TNS.TMM.IslemDosya bilgi = new TNS.TMM.IslemDosya();
            bilgi.dosyaKod = dosyaID;

            string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
            bilgi.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirIslemBelgesi");

            Sonuc sonuc = servisTMM.IslemDosyaSil(bilgi);
            if (sonuc.islemSonuc)
                Ext1.Net.Notification.Show(new NotificationConfig() { Icon = Icon.Information, HideDelay = 2000, PinEvent = "click", Html = note, Title = "Bilgi" });
            else
                Ext1.Net.Notification.Show(new NotificationConfig() { Icon = Icon.Information, HideDelay = 2000, PinEvent = "click", Html = sonuc.hataStr, Title = "Hata" });

            DosyaListele();
        }

        [DirectMethod]
        public void DosyaIndir(int dosyaID)
        {
            TNS.TMM.IslemDosya bilgi = new TNS.TMM.IslemDosya();
            bilgi.yil = OrtakFonksiyonlar.ConvertToInt(hdnYil.Value.ToString(), 0);
            bilgi.dosyaKod = dosyaID;

            string gonderilecekDosyaTmp = "";
            string gonderilecekDosya = "";
            ObjectArray liste = servisTMM.IslemDosyaGetir(bilgi);

            if (liste.sonuc.islemSonuc)
            {
                foreach (TNS.TMM.IslemDosya dosya in liste.objeler)
                {
                    string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
                    dosya.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirIslemBelgesi");
                    string dosyaYol = Path.Combine(dosya.kayitEdilecekDosyaYol, dosya.yil.ToString());
                    dosyaYol = Path.Combine(dosyaYol, dosya.yil + "_" + dosya.dosyaKod);
                    if (File.Exists(dosyaYol))
                        dosya.resim = File.ReadAllBytes(dosyaYol);

                    gonderilecekDosyaTmp = Path.GetTempFileName();
                    gonderilecekDosya = dosya.adi;
                    File.WriteAllBytes(gonderilecekDosyaTmp, dosya.resim);
                }

                FileInfo file = new FileInfo(gonderilecekDosyaTmp);
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpContext.Current.Server.UrlEncode(gonderilecekDosya).Replace('+', ' '));
                System.Web.HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                System.Web.HttpContext.Current.Response.Buffer = false;
                System.Web.HttpContext.Current.Response.WriteFile(file.FullName);
                System.Web.HttpContext.Current.Response.Flush();

                System.Web.HttpContext.Current.Response.End();
            }
        }

    }
}