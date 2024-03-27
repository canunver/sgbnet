using Ext1.Net;
using OrtakClass;
using System;
using System.IO;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r malzemelerinin kuru� fark tutarlar� bilgilerinin raporlama i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class KurusFarkRaporu : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ile ilgili ayarlamalar yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMKFR001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
            }
        }

        protected void btnYazdirListe_Click(object sender, EventArgs e)
        {
            ObjectArray bilgi = servisTMM.KurusFarklariRaporu(kullanan, KriterTopla());

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.BosDosyaAc(sonucDosyaAd);

            satir = 0;
            XLS.HucreDegerYaz(satir, 0, "Muhasebe Kod");
            XLS.HucreDegerYaz(satir, 1, "Muhasebe Ad");
            XLS.HucreDegerYaz(satir, 2, "Birim Kod");
            XLS.HucreDegerYaz(satir, 3, "Birim Ad");
            XLS.HucreDegerYaz(satir, 4, "Fi� No");
            XLS.HucreDegerYaz(satir, 5, "Hesap Kod");
            XLS.HucreDegerYaz(satir, 6, "Hesap Ad");
            XLS.HucreDegerYaz(satir, 7, "Giri� Tutar (Muh)");
            XLS.HucreDegerYaz(satir, 8, "��k�� Tutar (Muh)");
            XLS.HucreDegerYaz(satir, 9, "Giri� Tutar (Ta�)");
            XLS.HucreDegerYaz(satir, 10, "��k�� Tutar (Ta�)");
            XLS.HucreDegerYaz(satir, 11, "Giri� Fark");
            XLS.HucreDegerYaz(satir++, 12, "��k�� Fark");

            foreach (TNS.TMM.KurusFarki kf in bilgi.objeler)
            {
                ToplamAl ta = new ToplamAl(2, 6);
                TNS.TMM.KurusFarkiDetay toplamKfd = null;
                for (int i = 0; i < kf.detay.Count; i++)
                {
                    TNS.TMM.KurusFarkiDetay detay = (TNS.TMM.KurusFarkiDetay)kf.detay[i];
                    if (Farkli(detay, toplamKfd))
                    {
                        if (toplamKfd != null)
                        {
                            SatirYaz(XLS, ref satir, kf, toplamKfd, ta, 0, false);
                            ta.SatirSifirla(0);
                        }
                        toplamKfd = detay;
                    }

                    decimal girisTutar = 0;
                    decimal cikisTutar = 0;

                    if (detay.islemTipiTur <= (int)ENUMIslemTipi.ACILIS &&
                       !(detay.islemTipiTur == (int)ENUMIslemTipi.DEGERARTTIR && detay.hesapPlanKod.Substring(0, 3) == ((int)ENUMTasinirHesapKodu.TUKETIM).ToString()))
                        girisTutar = detay.tutar[0];
                    else
                        cikisTutar = detay.tutar[0];

                    decimal girisTutarKr = Math.Round(girisTutar, 2);
                    decimal cikisTutarKr = Math.Round(cikisTutar, 2);
                    decimal girisFark = girisTutarKr - girisTutar;
                    decimal cikisFark = cikisTutarKr - cikisTutar;

                    ta.DegerAta(1, 0, Convert.ToDouble(girisTutarKr));
                    ta.DegerAta(1, 1, Convert.ToDouble(cikisTutarKr));
                    ta.DegerAta(1, 2, Convert.ToDouble(girisTutar));
                    ta.DegerAta(1, 3, Convert.ToDouble(cikisTutar));
                    ta.DegerAta(1, 4, Convert.ToDouble(girisFark));
                    ta.DegerAta(1, 5, Convert.ToDouble(cikisFark));

                    ta.ToplamEkle(0, 1);
                    if (chkDetay.Checked)
                    {
                        SatirYaz(XLS, ref satir, kf, detay, ta, 1, true);
                    }
                }
                if (toplamKfd != null)
                {
                    SatirYaz(XLS, ref satir, kf, toplamKfd, ta, 0, false);
                }
            }
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), Path.GetFileName(sonucDosyaAd), true, GenelIslemler.ExcelTur());
        }

        private bool Farkli(KurusFarkiDetay detay, KurusFarkiDetay toplamKfd)
        {
            if (toplamKfd == null || detay == null) return true;
            if (toplamKfd.hesapPlanKod != detay.hesapPlanKod) return true;
            return false;
        }

        private void SatirYaz(Tablo XLS, ref int satir, KurusFarki kf, TNS.TMM.KurusFarkiDetay detay, ToplamAl ta, int p, bool detayYaz)
        {
            if (ta.toplam[p, 4] != 0 || ta.toplam[p, 5] != 0)
            {
                XLS.HucreDegerYaz(satir, 0, kf.muhasebeKod);
                XLS.HucreDegerYaz(satir, 1, kf.muhasebeAd);
                XLS.HucreDegerYaz(satir, 2, kf.harcamaKod);
                XLS.HucreDegerYaz(satir, 3, kf.harcamaAd);
                if (detayYaz)
                    XLS.HucreDegerYaz(satir, 4, detay.fisNo);
                XLS.HucreDegerYaz(satir, 5, detay.hesapPlanKod);
                XLS.HucreDegerYaz(satir, 6, detay.hesapPlanAd);
                XLS.HucreDegerYaz(satir, 7, ta.toplam[p, 0]);
                XLS.HucreDegerYaz(satir, 8, ta.toplam[p, 1]);
                XLS.HucreDegerYaz(satir, 9, ta.toplam[p, 2]);
                XLS.HucreDegerYaz(satir, 10, ta.toplam[p, 3]);
                XLS.HucreDegerYaz(satir, 11, ta.toplam[p, 4]);
                XLS.HucreDegerYaz(satir++, 12, ta.toplam[p, 5]);
            }
        }

        /// <summary>
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam �a��r�l�r ve
        /// toplanan kriterler KurusFarkiRaporuYazdir yordam�na g�nderilir ve rapor haz�rlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            KurusFarkiRaporuYazdir(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>Kuru� fark� kriter bilgileri d�nd�r�l�r.</returns>
        private TNS.TMM.KurusFarki KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.KurusFarki kriter = new TNS.TMM.KurusFarki();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.tarih1 = new TNSDateTime(txtTarih1.RawText);
            kriter.tarih2 = new TNSDateTime(txtTarih2.RawText);
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen kuru� fark�na ait kriterleri sunucudaki KurusFarklariRaporu
        /// yordam�na g�nderir, sunucudan gelen bilgi k�mesini excel raporuna aktar�r.
        /// </summary>
        /// <param name="kriter">Kuru� fark� kriter bilgilerini tutan nesne</param>
        private void KurusFarkiRaporuYazdir(TNS.TMM.KurusFarki kriter)
        {
            ObjectArray bilgi = servisTMM.KurusFarklariRaporu(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "KurusFarkRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;
            TNS.TMM.KurusFarki kf = (TNS.TMM.KurusFarki)bilgi.objeler[0];
            XLS.HucreAdBulYaz("IlAd", kf.ilAd + "-" + kf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", kf.ilKod + "-" + kf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", kf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", kf.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", kf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", kf.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", kf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", kf.muhasebeKod);

            string aciklama = string.Empty;
            if (!(kriter.tarih1.isNull && kriter.tarih2.isNull))
                aciklama = string.Format(Resources.TasinirMal.FRMKFR002, kriter.tarih1.ToString(), kriter.tarih2.ToString());
            else
                aciklama = Resources.TasinirMal.FRMKFR003;

            ImzaEkle(XLS, aciklama);

            decimal[] toplam = new decimal[4];
            string eskiHesap = string.Empty;
            string eskiHesapAd = string.Empty;
            for (int i = 0; i < kf.detay.Count; i++)
            {
                TNS.TMM.KurusFarkiDetay detay = (TNS.TMM.KurusFarkiDetay)kf.detay[i];

                if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod)
                {
                    if (chkDetay.Checked)
                        ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam);
                    else
                        ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam);

                    for (int k = 0; k < toplam.Length; k++)
                        toplam[k] = 0;
                }

                if (chkDetay.Checked)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                    XLS.HucreDegerYaz(satir, sutun, detay.fisNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
                }

                bool giris = false;
                if (detay.islemTipiTur <= (int)ENUMIslemTipi.ACILIS &&
                   !(detay.islemTipiTur == (int)ENUMIslemTipi.DEGERARTTIR && detay.hesapPlanKod.Substring(0, 3) == ((int)ENUMTasinirHesapKodu.TUKETIM).ToString()))
                    giris = true;

                if (giris)
                {
                    if (chkDetay.Checked)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[0].ToString("#,###.00"), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[0].ToString(), (double)0));
                    }

                    toplam[0] += OrtakFonksiyonlar.ConvertToDecimal(detay.tutar[0].ToString("#,###.00"), (decimal)0);
                    toplam[2] += OrtakFonksiyonlar.ConvertToDecimal(detay.tutar[0].ToString(), (decimal)0);
                }
                else
                {
                    if (chkDetay.Checked)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[0].ToString("#,###.00"), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[0].ToString(), (double)0));
                    }

                    toplam[1] += OrtakFonksiyonlar.ConvertToDecimal(detay.tutar[0].ToString("#,###.00"), (decimal)0);
                    toplam[3] += OrtakFonksiyonlar.ConvertToDecimal(detay.tutar[0].ToString(), (decimal)0);
                }

                eskiHesap = detay.hesapPlanKod;
                eskiHesapAd = detay.hesapPlanAd;
            }

            if (chkDetay.Checked)
                ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam);
            else
                ToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Excel raporuna kuru� farklar� toplam tutar bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun ba�lad��� kaynak sat�r numaras�</param>
        /// <param name="satir">Toplam bilgilerinin yaz�lmaya ba�lanaca�� sat�r numaras�</param>
        /// <param name="sutun">Toplam bilgilerinin yaz�lmaya ba�lanaca�� s�tun numaras�</param>
        /// <param name="hesapKod">Toplam bilgilerinin ait oldu�u ta��n�r hesap plan� kodu</param>
        /// <param name="hesapAd">Toplam bilgilerinin ait oldu�u ta��n�r hesap plan� ad�</param>
        /// <param name="toplam">Kuru� farklar�na ait toplam bilgilerini tutan dizi</param>
        private void ToplamYaz(Tablo XLS, int kaynakSatir, ref int satir, int sutun, string hesapKod, string hesapAd, decimal[] toplam)
        {
            decimal girisFark = OrtakFonksiyonlar.ConvertToDecimal((toplam[0] - toplam[2]).ToString("#,###.00"), (decimal)0);
            decimal cikisFark = OrtakFonksiyonlar.ConvertToDecimal((toplam[1] - toplam[3]).ToString("#,###.00"), (decimal)0);
            if (((girisFark >= (decimal)0.01 || girisFark <= (decimal)-0.01 || cikisFark >= (decimal)0.01 || cikisFark <= (decimal)-0.01) && !chkDetay.Checked) || chkDetay.Checked)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);

                if (chkDetay.Checked)
                {
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 4);
                    XLS.DuseyHizala(satir, sutun, 1);
                    XLS.KoyuYap(satir, sutun, true);
                    XLS.HucreDegerYaz(satir, sutun, hesapKod);

                    XLS.KoyuYap(satir, sutun + 5, satir, sutun + 10, true);
                }
                else
                {
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);
                    XLS.SutunGizle(sutun, sutun, true);
                    XLS.HucreDegerYaz(satir, sutun + 1, hesapKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, hesapAd);

                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(girisFark.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(cikisFark.ToString(), (double)0));
                }

                for (int i = 0; i < toplam.Length; i++)
                    XLS.HucreDegerYaz(satir, sutun + 5 + i, OrtakFonksiyonlar.ConvertToDouble(toplam[i].ToString(), (double)0));
            }
        }

        /// <summary>
        /// Excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
        /// <param name="aciklama">Rapora eklenecek a��klama bilgisi</param>
        private void ImzaEkle(Tablo XLS, string aciklama)
        {
            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), 0);

            string[] ad = new string[2];
            string[] unvan = new string[2];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI && string.IsNullOrEmpty(ad[1]))
                    {
                        ad[1] = iBilgi.adSoyad;
                        unvan[1] = iBilgi.unvan;
                    }
                }
            }

            if (!string.IsNullOrEmpty(aciklama))
                XLS.HucreAdBulYaz("Aciklama", aciklama);

            if (!string.IsNullOrEmpty(ad[0]))
                XLS.HucreAdBulYaz("AdSoyad1", ad[0]);

            if (!string.IsNullOrEmpty(unvan[0]))
                XLS.HucreAdBulYaz("Unvan1", unvan[0]);

            XLS.HucreAdBulYaz("Tarih1", DateTime.Today.Date.ToShortDateString());

            if (!string.IsNullOrEmpty(ad[1]))
                XLS.HucreAdBulYaz("AdSoyad2", ad[1]);

            if (!string.IsNullOrEmpty(unvan[1]))
                XLS.HucreAdBulYaz("Unvan2", unvan[1]);

            XLS.HucreAdBulYaz("Tarih2", DateTime.Today.Date.ToShortDateString());
        }
    }
}