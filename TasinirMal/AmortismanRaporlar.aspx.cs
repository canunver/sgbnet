using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using TNS.UZY;
using TNS.Raporlama;
using TNS.KYM;
using System.IO;
using System.Collections;

namespace TasinirMal
{
    /// <summary>
    /// Demirbaş amortisman bilgilerinin raporlama işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class AmortismanRaporlar : TMMSayfa
    {
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
        }

        /// <summary>
        /// Yazdır tuşuna basılınca çalışan olay metodu
        /// Sayfa adresinde gelen tur girdi dizgisine bakarak ilgili raporu hazırlayan yordamı çağırır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
        }
    }


    public class AmortismanRapor : IRapor
    {
        TNS.TMM.AmortismanKriter ak;
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
        Kullanici kullanan;

        /// <summary>
        /// Raporlama kriterleri amortisman kriter nesnesinde parametre olarak alınır ve sunucuya gönderilir
        /// ve kriterlere uygun olan demirbaşların amortisman bilgileri sunucudan alınır. Hata varsa ekrana hata
        /// bilgisi yazılır, yoksa gelen amortisman bilgileri excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="ak">Amortisman raporlama kriterlerini tutan nesne</param>
        public static Sonuc AmortismanMIFUret(Kullanici kullanan, int yil, int muhasebeKod, string strBelgeNolar)
        {
            string[] belgeNolar = strBelgeNolar.Split(',', ';');
            AmortismanKriter ak = new AmortismanKriter();
            ak.raporTur = 2;
            ak.yil = yil;
            ak.muhasebeKod = muhasebeKod.ToString("00000");
            ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
            ObjectArray bilgi = servisTMM.AmortismanRaporla(kullanan, ak, false);

            if (!bilgi.sonuc.islemSonuc)
            {
                return bilgi.sonuc;
            }
            bool tekMIF = true;

            int mifSay = 0;
            string yeniBelgenolar = "";
            Sonuc s2 = new Sonuc();
            while (true)
            {
                if (bilgi.objeler.Count == 0) break;
                TNS.TMM.AmortismanRapor ilkSatir = (TNS.TMM.AmortismanRapor)bilgi.objeler[0];
                string birimKurkod = "";
                if (!tekMIF)
                {
                    birimKurkod = ilkSatir.birimKurkod;
                }
                TNS.MUH.OdemeEmriMIFForm f = new TNS.MUH.OdemeEmriMIFForm();
                f.yil = yil;
                f.surecYil = 0;
                f.surecNo = "";
                f.islemTarih = new TNSDateTime("31.12." + yil.ToString());
                f.aciklama = "Amortisman Fişi";
                f.belgeNo = OrtakFonksiyonlar.DiziElemanAl(belgeNolar, mifSay, "");
                mifSay++;
                f.muhasebe = muhasebeKod;
                if (tekMIF)
                    f.birim = "000";
                else
                {
                    f.birim = ilkSatir.birimKod;
                    f.kurKod = ilkSatir.kurKod;
                }
                f.ilgiliNo = "";

                while (true)
                {
                    TNS.TMM.AmortismanRapor detay = NesneAlSil(bilgi, birimKurkod);
                    if (detay != null)
                    {
                        string hesapKod = detay.hesapPlanKod;

                        double alacak = Math.Round(Convert.ToDouble(detay.cariToplamAmortismanTutar), 2);
                        double borc = Math.Round(Convert.ToDouble(detay.maliyetAmortismanBirikmisTutar), 2);

                        TNS.MUH.OdemeEmriMIFDetay d = new TNS.MUH.OdemeEmriMIFDetay(f.yil, hesapKod, borc, alacak);
                        if (detay.harcamaBirimiKod.Length == 11)
                        {
                            d.detayBirim = detay.harcamaBirimiKod.Substring(8, 3) + "." + detay.harcamaBirimiKod.Substring(0, 8);
                        }
                        d.BorcAlacakAyarla();
                        if (d.borc > 0 || d.alacak > 0)
                            f.detay.Ekle(d);
                    }
                    else
                        break;
                }

                if (f.detay.ObjeSayisi > 0)
                {
                    TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                    Sonuc s = servisMUH.OdemeEmriMIFKaydet(kullanan, f);
                    if (s.islemSonuc)
                    {
                        if (yeniBelgenolar != "") yeniBelgenolar += ",";
                        yeniBelgenolar += s.anahtar;
                    }
                    else
                    {
                        s2 = s;
                        break;
                    }
                }
                else
                    break;
            }
            s2.anahtar = yeniBelgenolar;
            if (yeniBelgenolar == "") s2.DegerAta("Kaydedilecek kayıt bulunamadı");
            if (s2.islemSonuc)
            {
                Sonuc s3 = servisTMM.AmortismanSinirMIFKaydet(kullanan, yil, yeniBelgenolar);
                if (!s3.islemSonuc)
                {
                    s2.DegerAta(yeniBelgenolar + " numaralı belge/belgeler üretildi fakat, amortisman yılı ile ilişkilendirilemedi. Oluşan hata:" + s3.hataStr);
                }
            }
            return s2;
        }

        private static TNS.TMM.AmortismanRapor NesneAlSil(ObjectArray bilgi, string kurKod)
        {
            int yer = -1;
            TNS.TMM.AmortismanRapor bulunan = null;
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                bulunan = (TNS.TMM.AmortismanRapor)bilgi.objeler[i];
                string gb = bulunan.birimKurkod;
                if (gb == kurKod || kurKod == "")
                {
                    yer = i;
                    break;
                }
            }
            if (yer == -1) return null;
            bilgi.objeler.RemoveAt(yer);
            return bulunan;
        }

        /// <summary>
        /// Raporlama kriterleri amorti
        /// sman kriter nesnesinde parametre olarak alınır ve sunucuya gönderilir
        /// ve kriterlere uygun olan demirbaşların amortisman bilgileri sunucudan alınır. Hata varsa ekrana hata
        /// bilgisi yazılır, yoksa gelen amortisman bilgileri excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="ak">Amortisman raporlama kriterlerini tutan nesne</param>
        private string AmortismanMIFYazdir(AmortismanKriter ak)
        {
            ak.raporTur = 2;
            ObjectArray bilgi = servisTMM.AmortismanRaporla(kullanan, ak, false);

            if (!bilgi.sonuc.islemSonuc)
            {
                throw new Exception(bilgi.sonuc.hataStr);
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int kaynakSutun = 0;
            this.sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(this.sablonAd, this.sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", string.Format("{0} Yılı " + (ak.kumulatif == "1" ? "Birikmiş" : "") + " Amortisman Bilgisi Muhasebe İşlem Fişi", ak.yil.ToString()));
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref kaynakSutun);
            int satirYukseklik = XLS.SatirYukseklikAl(satir);

            foreach (TNS.TMM.AmortismanRapor detay in bilgi.objeler)
            {
                XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 6, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 0, ParcalAl(detay.harcamaBirimiKod, 8, 3) + "." + ParcalAl(detay.harcamaBirimiKod, 0, 8));
                XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 3, Convert.ToDouble(detay.maliyetAmortismanBirikmisTutar));
                XLS.HucreDegerYaz(satir, sutun + 4, Convert.ToDouble(detay.cariToplamAmortismanTutar));
                XLS.HucreDegerYaz(satir, sutun + 5, Convert.ToDouble(detay.maliyetTutar));
                satir++;
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, satirYukseklik);
            XLS.DosyaSaklaTamYol();
            return this.sonucDosyaAd;
        }

        private string ParcalAl(string str, int basla, int uz)
        {
            if (String.IsNullOrWhiteSpace(str))
                return "";
            int strUz = str.Length;
            if (basla + uz <= strUz)
            {
                return str.Substring(basla, uz);
            }
            if (basla < uz)
                return str.Substring(basla);
            return str;
        }

        /// <summary>
        /// Raporlama kriterleri amortisman kriter nesnesinde parametre olarak alınır ve sunucuya gönderilir
        /// ve kriterlere uygun olan demirbaşların amortisman bilgileri sunucudan alınır. Hata varsa ekrana hata
        /// bilgisi yazılır, yoksa gelen amortisman bilgileri excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="ak">Amortisman raporlama kriterlerini tutan nesne</param>
        private string AmortismanRaporla(AmortismanKriter ak)
        {
            ObjectArray bilgi = servisTMM.AmortismanRaporla2(kullanan, ak); //Dönem eklendi

            if (!bilgi.sonuc.islemSonuc)
            {
                throw new Exception(bilgi.sonuc.hataStr);
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int sutun = 0;
            int kaynakSatir = 0;
            this.sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(this.sablonAd, this.sonucDosyaAd);

            string baslik = string.Format(Resources.TasinirMal.FRMAMR003, ak.yil.ToString());
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                baslik = string.Format("{0} YILI {1}{2}AMORTİSMAN RAPORU", ak.yil.ToString(), ak.donem > 0 ? ak.donem + ". AY " : "", ak.raporTur == 7 ? "TTK " : "");

            XLS.HucreAdBulYaz("Baslik", baslik);
            XLS.HucreAdBulYaz("RaporTarihi", string.Format("Rapor Tarihi : {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm")));
            XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref sutun);
            int satir = -1;

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int ilkSatir = satir;
            satir--;
            decimal[] genelToplam = new decimal[9];
            int sayac = 1;
            foreach (TNS.TMM.AmortismanRapor detay in bilgi.objeler)
            {
                satir++;
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);

                double amortiYuzde = 0;
                if (detay.amortismanSuresi > 0) amortiYuzde = 100 / detay.amortismanSuresi;

                string sicilNo = detay.gorSicilNo;
                if (detay.bisNo != "") sicilNo += "-" + detay.bisNo;

                string eSicilNo = detay.eskiSicilNo;
                if (detay.eskiBisNo != "") eSicilNo += "-" + detay.eskiBisNo;
                XLS.HucreDegerYaz(satir, sutun + 0, sayac++);
                XLS.HucreDegerYaz(satir, sutun + 1, amortiYuzde);
                XLS.HucreDegerYaz(satir, sutun + 2, detay.girisTarih.Oku());
                XLS.HucreDegerYaz(satir, sutun + 3, sicilNo);
                XLS.HucreDegerYaz(satir, sutun + 4, eSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 5, detay.harcamaBirimiAdi);
                if (detay.amortismanDurum != 0)
                {
                    XLS.HucreDegerYaz(satir, sutun + 6, detay.islemDurumAciklama + " (" + detay.cikisTarih + "-" + detay.cikisFisNo + ")");
                    if (detay.amortismanDurum == 6 || detay.amortismanDurum == 7)
                        XLS.ArkaPlanRenk(satir, 0, satir, 17, System.Drawing.Color.Yellow);
                }
                else
                    XLS.HucreDegerYaz(satir, sutun + 6, detay.nereyeGittiAd);
                XLS.HucreDegerYaz(satir, sutun + 7, detay.hesapPlanKod + "-" + detay.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 8, detay.maliyetTutar);
                XLS.HucreDegerYaz(satir, sutun + 9, detay.degerlemeTutar);
                XLS.HucreDegerYaz(satir, sutun + 10, detay.toplamTutar);
                XLS.HucreDegerYaz(satir, sutun + 11, (ak.raporTur == 7 ? detay.cariMaliyetAmortismanTutarTTK : detay.cariMaliyetAmortismanTutar));
                XLS.HucreDegerYaz(satir, sutun + 12, detay.maliyetAmortismanBirikmisTutar);
                XLS.HucreDegerYaz(satir, sutun + 13, detay.cariDegerlemeAmortismanTutar);
                XLS.HucreDegerYaz(satir, sutun + 14, detay.degerlemeAmortismanBirikmisTutar);
                XLS.HucreDegerYaz(satir, sutun + 15, detay.toplamAmortismanTutar);
                XLS.HucreDegerYaz(satir, sutun + 16, detay.kalanTutar);
                XLS.HucreDegerYaz(satir, sutun + 17, detay.amortismanDurum);

                genelToplam[0] += detay.maliyetTutar;
                genelToplam[1] += detay.degerlemeTutar;
                genelToplam[2] += detay.toplamTutar;
                genelToplam[3] += (ak.raporTur == 7 ? detay.cariMaliyetAmortismanTutarTTK : detay.cariMaliyetAmortismanTutar);
                genelToplam[4] += detay.maliyetAmortismanBirikmisTutar;
                genelToplam[5] += detay.cariDegerlemeAmortismanTutar;
                genelToplam[6] += detay.degerlemeAmortismanBirikmisTutar;
                genelToplam[7] += detay.toplamAmortismanTutar;
                genelToplam[8] += detay.kalanTutar;
            }

            //TTK
            if (ak.raporTur == 7)
            {
                //XLS.SutunGizle(sutun + 11, sutun + 11, true);
                XLS.SutunGizle(sutun + 13, sutun + 13, true);
            }

            //Genel Toplam Satırı ****************************************************************
            satir++;
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 7);
            XLS.HucreDegerYaz(satir, sutun, "GENEL TOPLAM");
            XLS.KoyuYap(satir, sutun, satir, sutun + 16, true);

            XLS.HucreDegerYaz(satir, sutun + 8, genelToplam[0]);
            XLS.HucreDegerYaz(satir, sutun + 9, genelToplam[1]);
            XLS.HucreDegerYaz(satir, sutun + 10, genelToplam[2]);
            XLS.HucreDegerYaz(satir, sutun + 11, genelToplam[3]);
            XLS.HucreDegerYaz(satir, sutun + 12, genelToplam[4]);
            XLS.HucreDegerYaz(satir, sutun + 13, genelToplam[5]);
            XLS.HucreDegerYaz(satir, sutun + 14, genelToplam[6]);
            XLS.HucreDegerYaz(satir, sutun + 15, genelToplam[7]);
            XLS.HucreDegerYaz(satir, sutun + 16, genelToplam[8]);
            //**************************************************************************************

            //Eklenen satırların yükseklikleri ayarlanıyor
            XLS.SatirYukseklikAyarla(ilkSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            return this.sonucDosyaAd;
        }

        private string AmortismanRaporla3(AmortismanKriter ak)
        {
            if (ak.raporTur == 8)
                return AmortismanRaporlaMaliyetMuhasebesi(ak);

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && (ak.raporTur == 6 || ak.raporTur == 4))
            {
                ObjectArray bilgi = servisTMM.AmortismanRaporla3(kullanan, ak);

                if (!bilgi.sonuc.islemSonuc)
                {
                    throw new Exception(bilgi.sonuc.hataStr);
                }

                string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");
                this.sablonAd = raporSablonYol + "\\TMM\\" + "AmortismanRaporuMB.xlt";

                Tablo XLS = GenelIslemler.NewTablo();
                int sutun = 0;
                int kaynakSatir = 0;
                this.sonucDosyaAd += "." + XLS.UzantiBul();
                XLS.DosyaAc(this.sablonAd, this.sonucDosyaAd);


                string baslik = string.Format(Resources.TasinirMal.FRMAMR003, ak.yil.ToString());
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    baslik = string.Format("{0} YILI {1}AMORTİSMAN RAPORU", ak.yil.ToString(), ak.donem > 0 ? ak.donem + ". AY " : "");

                XLS.HucreAdBulYaz("Baslik", baslik);
                XLS.HucreAdBulYaz("RaporTarihi", string.Format("Rapor Tarihi : {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm")));
                XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref sutun);
                int satir = -1;

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int ilkSatir = satir;
                satir--;


                decimal[] genelToplam = new decimal[14];
                int sayac = 1;

                foreach (TNS.TMM.AmortismanRapor3 ar in bilgi.objeler)
                {
                    satir++;
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);

                    string sicilNo = ar.gorSicilNo;
                    if (!string.IsNullOrWhiteSpace(sicilNo) && !string.IsNullOrWhiteSpace(ar.bisNo))
                        sicilNo += "-" + ar.bisNo;

                    string eSicilNo = ar.eskiSicilNo;
                    if (!string.IsNullOrWhiteSpace(eSicilNo) && !string.IsNullOrWhiteSpace(ar.eskiBisNo))
                        eSicilNo += "-" + ar.eskiBisNo;
                    XLS.HucreDegerYaz(satir, sutun + 0, sayac++);
                    XLS.HucreDegerYaz(satir, sutun + 1, ar.amortismanYuzdesi);
                    XLS.HucreDegerYaz(satir, sutun + 2, ar.girisTarih.Oku());
                    XLS.HucreDegerYaz(satir, sutun + 3, sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 4, eSicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 5, ar.harcamaBirimiAdi);
                    if (ar.amortismanDurum == 1)
                        XLS.HucreDegerYaz(satir, sutun + 6, ar.islemDurumAciklama + " (" + ar.cikisTarih + "-" + ar.cikisFisNo + ")");
                    else
                        XLS.HucreDegerYaz(satir, sutun + 6, ar.nereyeGittiAd);
                    XLS.HucreDegerYaz(satir, sutun + 7, ar.hesapPlanKod + "-" + ar.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 8, ar.maliyetTutar);
                    XLS.HucreDegerYaz(satir, sutun + 9, ar.degerArtisToplamTutar);
                    XLS.HucreDegerYaz(satir, sutun + 10, ar.enflasyonToplamTutar);
                    XLS.HucreDegerYaz(satir, sutun + 11, ar.toplamTutar);

                    if (ak.raporTur == 7)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 12, ar.maliyetCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 13, ar.maliyetBirikmisAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 14, ar.degerArtisCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 15, ar.degerArtisBirikmisAmortismanTTK);
                        XLS.HucreDegerYaz(satir, sutun + 16, ar.enflasyonCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 17, ar.enflasyonBirikmisAmortismanTTK);
                        XLS.HucreDegerYaz(satir, sutun + 18, ar.toplamCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 19, ar.toplamBirikmisAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 20, ar.toplamAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 21, ar.toplamKalanAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 22, ar.amortismanDurum);

                        genelToplam[0] += ar.maliyetTutar;
                        genelToplam[1] += ar.degerArtisToplamTutar;
                        genelToplam[2] += ar.enflasyonToplamTutar;
                        genelToplam[3] += ar.toplamTutar;
                        genelToplam[4] += ar.maliyetCariAmortisman;
                        genelToplam[5] += ar.maliyetBirikmisAmortisman;
                        genelToplam[6] += ar.degerArtisCariAmortisman;
                        genelToplam[7] += ar.degerArtisBirikmisAmortismanTTK;
                        genelToplam[8] += ar.enflasyonCariAmortisman;
                        genelToplam[9] += ar.enflasyonBirikmisAmortismanTTK;
                        genelToplam[10] += ar.toplamCariAmortisman;
                        genelToplam[11] += ar.toplamBirikmisAmortisman;
                        genelToplam[12] += ar.toplamAmortisman;
                        genelToplam[13] += ar.toplamKalanAmortisman;
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, sutun + 12, ar.maliyetCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 13, ar.maliyetBirikmisAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 14, ar.degerArtisCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 15, ar.degerArtisBirikmisAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 16, ar.enflasyonCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 17, ar.enflasyonBirikmisAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 18, ar.toplamCariAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 19, ar.toplamBirikmisAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 20, ar.toplamAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 21, ar.toplamKalanAmortisman);
                        XLS.HucreDegerYaz(satir, sutun + 22, ar.amortismanDurum);

                        genelToplam[0] += ar.maliyetTutar;
                        genelToplam[1] += ar.degerArtisToplamTutar;
                        genelToplam[2] += ar.enflasyonToplamTutar;
                        genelToplam[3] += ar.toplamTutar;
                        genelToplam[4] += ar.maliyetCariAmortisman;
                        genelToplam[5] += ar.maliyetBirikmisAmortisman;
                        genelToplam[6] += ar.degerArtisCariAmortisman;
                        genelToplam[7] += ar.degerArtisBirikmisAmortisman;
                        genelToplam[8] += ar.enflasyonCariAmortisman;
                        genelToplam[9] += ar.enflasyonBirikmisAmortisman;
                        genelToplam[10] += ar.toplamCariAmortisman;
                        genelToplam[11] += ar.toplamBirikmisAmortisman;
                        genelToplam[12] += ar.toplamAmortisman;
                        genelToplam[13] += ar.toplamKalanAmortisman;
                    }
                }

                //TTK
                if (ak.raporTur == 7)
                {
                    XLS.SutunGizle(sutun + 12, sutun + 12, true);
                    XLS.SutunGizle(sutun + 14, sutun + 14, true);
                    XLS.SutunGizle(sutun + 16, sutun + 16, true);
                    XLS.SutunGizle(sutun + 18, sutun + 18, true);
                    XLS.SutunGizle(sutun + 20, sutun + 20, true);
                }

                //Genel Toplam Satırı ****************************************************************
                satir++;
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 7);
                XLS.HucreDegerYaz(satir, sutun, "GENEL TOPLAM");
                XLS.KoyuYap(satir, sutun, satir, sutun + 21, true);

                XLS.HucreDegerYaz(satir, sutun + 8, genelToplam[0]);
                XLS.HucreDegerYaz(satir, sutun + 9, genelToplam[1]);
                XLS.HucreDegerYaz(satir, sutun + 10, genelToplam[2]);
                XLS.HucreDegerYaz(satir, sutun + 11, genelToplam[3]);
                XLS.HucreDegerYaz(satir, sutun + 12, genelToplam[4]);
                XLS.HucreDegerYaz(satir, sutun + 13, genelToplam[5]);
                XLS.HucreDegerYaz(satir, sutun + 14, genelToplam[6]);
                XLS.HucreDegerYaz(satir, sutun + 15, genelToplam[7]);
                XLS.HucreDegerYaz(satir, sutun + 16, genelToplam[8]);
                XLS.HucreDegerYaz(satir, sutun + 17, genelToplam[9]);
                XLS.HucreDegerYaz(satir, sutun + 18, genelToplam[10]);
                XLS.HucreDegerYaz(satir, sutun + 19, genelToplam[11]);
                XLS.HucreDegerYaz(satir, sutun + 20, genelToplam[12]);
                XLS.HucreDegerYaz(satir, sutun + 21, genelToplam[13]);
                //**************************************************************************************

                //Eklenen satırların yükseklikleri ayarlanıyor
                XLS.SatirYukseklikAyarla(ilkSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.DosyaSaklaTamYol();

                return this.sonucDosyaAd;
            }
            else
                return AmortismanRaporla(ak);
        }

        private string AmortismanRaporlaMaliyetMuhasebesi(AmortismanKriter ak)
        {
            ak.raporTur = 6;
            if (string.IsNullOrWhiteSpace(ak.ambarKod) || ak.ambarKod == "01" || ak.ambarKod == "51")
                ak.ambarKod = "@01,51"; //Demirbaş ve Yazılım birlikte alınsın isteniyor.

            ObjectArray bilgi = servisTMM.AmortismanRaporla3(kullanan, ak);

            if (!bilgi.sonuc.islemSonuc)
            {
                throw new Exception(bilgi.sonuc.hataStr);
            }

            Hashtable liste = new Hashtable();
            foreach (TNS.TMM.AmortismanRapor3 ar in bilgi.objeler)
            {
                if (ar.maliyetMerkezi == null)
                    continue;

                if (liste.ContainsKey(ar.maliyetMerkezi))
                    liste[ar.maliyetMerkezi] = (decimal)liste[ar.maliyetMerkezi] + ar.toplamCariAmortisman;
                else
                    liste.Add(ar.maliyetMerkezi, ar.toplamCariAmortisman);
            }

            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");
            this.sablonAd = raporSablonYol + "\\TMM\\" + "AmortismanRaporuMaliyetMuhasebe.xlt";

            Tablo XLS = GenelIslemler.NewTablo();
            int sutun = 0;
            int kaynakSatir = 0;
            this.sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(this.sablonAd, this.sonucDosyaAd);


            XLS.HucreAdBulYaz("Baslik", "MALİYET MUHASEBESİ RAPORU");
            XLS.HucreAdBulYaz("RaporTarihi", string.Format("Rapor Tarihi : {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm")));
            XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref sutun);
            int satir = -1;

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int ilkSatir = satir;
            satir--;

            decimal genelToplam = 0;
            int sayac = 1;

            foreach (DictionaryEntry di in liste)
            {
                string maliyetMerkezi = (string)di.Key;
                decimal toplamCariAmortisman = (decimal)di.Value;

                if (string.IsNullOrWhiteSpace(maliyetMerkezi) && toplamCariAmortisman == 0)
                    continue;

                satir++;
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 1, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 0, maliyetMerkezi);
                XLS.HucreDegerYaz(satir, sutun + 1, toplamCariAmortisman);

                genelToplam += toplamCariAmortisman;
            }

            //Genel Toplam Satırı ****************************************************************
            satir++;
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 1, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun, "GENEL TOPLAM");
            XLS.KoyuYap(satir, sutun, satir, sutun + 1, true);

            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam);
            //**************************************************************************************

            XLS.SatirYukseklikAyarla(ilkSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();

            return this.sonucDosyaAd;
        }

        //public static string AmortismanRaporla2(AmortismanKriter ak, string sonucDosyaAd)
        //{
        //    string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");

        //    AmortismanRapor r = new AmortismanRapor();
        //    r.sablonAd = raporSablonYol + "\\TMM\\" + "AmortismanRaporu.xlt";
        //    r.sonucDosyaAd = sonucDosyaAd;

        //    return r.AmortismanRaporla(ak);
        //}

        private string AmortismanRaporlaMuhasebat(AmortismanKriter ak)
        {
            ObjectArray bilgi = servisTMM.AmortismanRaporla2(kullanan, ak); //Dönem eklendi

            if (!bilgi.sonuc.islemSonuc)
            {
                throw new Exception(bilgi.sonuc.hataStr);
            }

            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");
            string sablonAd = raporSablonYol + "\\TMM\\" + "AmortismanRaporuMuhasebat.xltx";

            Tablo XLS = GenelIslemler.NewTablo();
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = Path.GetTempFileName();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Yil", ak.yil.ToString());
            XLS.HucreAdBulYaz("RaporTarihi", string.Format("Rapor Tarihi : {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm")));
            XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref sutun);
            int satir = -1;

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int ilkSatir = satir;
            satir--;
            decimal[] genelToplam = new decimal[9];
            foreach (TNS.TMM.AmortismanRapor detay in bilgi.objeler)
            {
                satir++;
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 17, satir, sutun);

                double amortiYuzde = 0;
                if (detay.amortismanSuresi > 0) amortiYuzde = 100 / detay.amortismanSuresi;

                XLS.HucreDegerYaz(satir, sutun + 0, detay.hesapPlanKod.Substring(0, 3));
                XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod.Substring(3));
                XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 3, detay.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 4, detay.girisTarih.Oku());

                XLS.HucreDegerYaz(satir, sutun + 5, "");//YEVMİYE NUMARASI
                XLS.HucreDegerYaz(satir, sutun + 6, detay.maliyetTutar);//MALİYET / İKTİSAP DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 7, detay.degerArtisTutar);//DEĞER ARTTIRICI HARCAMALAR

                XLS.HucreDegerYaz(satir, sutun + 8, detay.amortismanSuresi);

                if (detay.cikisTarih != null && !detay.cikisTarih.isNull)
                    XLS.HucreDegerYaz(satir, sutun + 9, detay.cikisTarih.Oku());//SATIŞ VEYA TERKİN TARİHİ

                XLS.HucreDegerYaz(satir, sutun + 10, amortiYuzde);

                XLS.HucreDegerYaz(satir, sutun + 11, "");//DÜZELTME / TAŞIMA KAYSAYISI

                XLS.HucreDegerYaz(satir, sutun + 12, detay.toplamTutar);//ENFLASYON DÜZELTMESİ ÖNCESİ MALİYET BEDELİ
                XLS.HucreDegerYaz(satir, sutun + 13, detay.toplamAmortismanTutar);//ENFLASYON ÖNCESİ BİRİKMİŞ AMORTİSMAN TUTARI
                XLS.HucreDegerYaz(satir, sutun + 14, detay.toplamTutar);//ENFLASYON DÜZELTMESİ SONRASI TUTAR
                XLS.HucreDegerYaz(satir, sutun + 15, detay.toplamAmortismanTutar);//ENFLASYON DÜZELTMESİ SONRASI BİRİKMİŞ AMORTİSMAN TUTARI
                XLS.HucreDegerYaz(satir, sutun + 16, detay.toplamAmortismanTutar);//BİRİKMİŞ AMORTİSMAN TUTARI
                XLS.HucreDegerYaz(satir, sutun + 17, detay.kalanTutar);//NET DEĞER

                genelToplam[0] += detay.toplamTutar;
                genelToplam[1] += detay.toplamAmortismanTutar;
                genelToplam[2] += detay.toplamTutar;
                genelToplam[3] += detay.toplamAmortismanTutar;
                genelToplam[4] += detay.toplamAmortismanTutar;
                genelToplam[5] += detay.kalanTutar;
            }

            //Genel Toplam Satırı ****************************************************************
            satir++;
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 17, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 11);
            XLS.HucreDegerYaz(satir, sutun, "GENEL TOPLAM");
            XLS.KoyuYap(satir, sutun, satir, sutun + 17, true);
            XLS.DuseyHizala(satir, sutun, satir, sutun + 17, 1);

            XLS.HucreDegerYaz(satir, sutun + 12, genelToplam[0]);
            XLS.HucreDegerYaz(satir, sutun + 13, genelToplam[1]);
            XLS.HucreDegerYaz(satir, sutun + 14, genelToplam[2]);
            XLS.HucreDegerYaz(satir, sutun + 15, genelToplam[3]);
            XLS.HucreDegerYaz(satir, sutun + 16, genelToplam[4]);
            XLS.HucreDegerYaz(satir, sutun + 17, genelToplam[5]);
            //**************************************************************************************

            //Eklenen satırların yükseklikleri ayarlanıyor
            XLS.SatirYukseklikAyarla(ilkSatir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Raporlama kriterleri defter kriter nesnesinde parametre olarak alınır ve sunucuya gönderilir ve kriterlere
        /// uygun olan demirbaşların amortisman bilgileri dayanıklı taşınır defteri formatında sunucudan alınır.
        /// Hata varsa ekrana hata bilgisi yazılır, yoksa gelen bilgiler excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="kriter">Dayanıklı taşınır defteri raporlama kriterlerini tutan nesne</param>
        //private string DayanikliTasinirDefteri(AmortismanKriter ak)
        //{
        //    DefterKriter kriter = new DefterKriter(ak);
        //    ObjectArray bilgi = servisTMM.AmortismanDTDOlustur(kullanan, kriter);

        //    if (!bilgi.sonuc.islemSonuc)
        //        throw new Exception(bilgi.sonuc.hataStr);

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    this.sonucDosyaAd += "." + XLS.UzantiBul();
        //    XLS.DosyaAc(this.sablonAd, this.sonucDosyaAd);

        //    int kaynakSatir = 0;
        //    int kaynakSutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);

        //    int satirEkle = kaynakSatir + 1;
        //    int aktifSayfa = 0;
        //    int sayfadakiSatirSayisi = 50000;

        //    //Boş sayfa yarat ve 0. sayfayı şablon olarak kullan
        //    //*****************************************************
        //    XLS.YeniSheetEkle(0);
        //    aktifSayfa++;
        //    XLS.AktifSheetDegistir(aktifSayfa);
        //    //*****************************************************

        //    for (int k = 0; k < bilgi.objeler.Count; k++)
        //    {
        //        DTDBilgi dtd = (DTDBilgi)bilgi.objeler[k];

        //        //liste satır sayısını aşıyor ise yeni sayfaya geç
        //        if (satirEkle + 1000 > sayfadakiSatirSayisi)
        //        {
        //            satirEkle = kaynakSatir + 1;
        //            XLS.YeniSheetEkle(0);
        //            aktifSayfa++;
        //            XLS.AktifSheetDegistir(aktifSayfa);
        //        }

        //        XLS.SatirAc(satirEkle, 9);
        //        XLS.HucreKopyala(0, kaynakSutun, 8, kaynakSutun + 20, satirEkle, kaynakSutun);
        //        XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 6, GenelIslemler.JexcelBirimtoExcelBirim(400));
        //        XLS.SatirYukseklikAyarla(satirEkle + 7, satirEkle + 8, GenelIslemler.JexcelBirimtoExcelBirim(600));

        //        XLS.HucreBirlestir(satirEkle, kaynakSutun, satirEkle, kaynakSutun + 20);
        //        XLS.HucreBirlestir(satirEkle + 1, kaynakSutun, satirEkle + 1, kaynakSutun + 1);
        //        XLS.HucreBirlestir(satirEkle + 1, kaynakSutun + 19, satirEkle + 1, kaynakSutun + 20);

        //        for (int birlestirSatir = satirEkle + 2; birlestirSatir < satirEkle + 6; birlestirSatir++)
        //        {
        //            XLS.HucreBirlestir(birlestirSatir, kaynakSutun, birlestirSatir, kaynakSutun + 2);
        //            XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 4, birlestirSatir, kaynakSutun + 6);
        //            XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 8, birlestirSatir, kaynakSutun + 10);
        //        }

        //        XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 12, satirEkle + 5, kaynakSutun + 12);
        //        XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 13, satirEkle + 3, kaynakSutun + 14);
        //        XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 15, satirEkle + 3, kaynakSutun + 20);

        //        for (int birlestirSatir = satirEkle + 4; birlestirSatir < satirEkle + 6; birlestirSatir++)
        //        {
        //            XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 13, birlestirSatir, kaynakSutun + 14);
        //            XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 15, birlestirSatir, kaynakSutun + 20);
        //        }

        //        XLS.HucreBirlestir(satirEkle + 7, kaynakSutun, satirEkle + 8, kaynakSutun);
        //        XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 1, satirEkle + 7, kaynakSutun + 2);

        //        for (int birlestirSutun = kaynakSutun + 3; birlestirSutun <= kaynakSutun + 10; birlestirSutun++)
        //            XLS.HucreBirlestir(satirEkle + 7, birlestirSutun, satirEkle + 8, birlestirSutun);

        //        XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 12, satirEkle + 7, kaynakSutun + 14);

        //        for (int birlestirSutun = kaynakSutun + 15; birlestirSutun <= kaynakSutun + 20; birlestirSutun++)
        //            XLS.HucreBirlestir(satirEkle + 7, birlestirSutun, satirEkle + 8, birlestirSutun);

        //        XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 4, dtd.ilAd + "-" + dtd.ilceAd);
        //        XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 8, dtd.ilKod + "-" + dtd.ilceKod);
        //        XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 4, dtd.harcamaAd);
        //        XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 8, dtd.harcamaKod);
        //        XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 4, dtd.ambarAd);
        //        XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 8, dtd.ambarKod);
        //        XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 4, dtd.muhasebeAd);
        //        XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 8, dtd.muhasebeKod);
        //        XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 15, dtd.hesapAd);
        //        XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 15, dtd.hesapKod);
        //        XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 15, dtd.olcuBirimAd);

        //        satirEkle += 9;
        //        int eskiSatirEkle = satirEkle;
        //        int satir = satirEkle;

        //        int siraNo = 0;
        //        int oncekiIslemTipTur = -1;
        //        for (int i = 0; i < dtd.detaylar.Count; i++)
        //        {
        //            int sutun = 0;

        //            DTDBilgiDetay detay = (DTDBilgiDetay)dtd.detaylar[i];

        //            bool satirEklemeliMi = true;
        //            if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur <= (int)ENUMIslemTipi.DEGERARTTIR)
        //                && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
        //            {
        //                siraNo++;
        //                sutun = kaynakSutun;
        //                satirEklemeliMi = true;
        //            }
        //            else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
        //                || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
        //                || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
        //            {
        //                sutun = kaynakSutun + 12;
        //                if (oncekiIslemTipTur >= (int)ENUMIslemTipi.SATINALMAGIRIS && oncekiIslemTipTur <= (int)ENUMIslemTipi.ACILIS
        //                    && oncekiIslemTipTur != (int)ENUMIslemTipi.ZFDUSME && oncekiIslemTipTur != (int)ENUMIslemTipi.DTLDUSME)
        //                    satirEklemeliMi = false;
        //                else
        //                    satirEklemeliMi = true;
        //            }

        //            oncekiIslemTipTur = detay.islemTipi.tur;

        //            //Yeni satır gerekli mi 
        //            //bool satirEklemeliMi = true;
        //            //for (int j = eskiSatirEkle; j < satirEkle; j++)
        //            //    if (string.IsNullOrEmpty(XLS.HucreDegerAl(j, sutun).Trim()))
        //            //        satirEklemeliMi = false;

        //            if (satirEklemeliMi)
        //            {
        //                XLS.SatirAc(satirEkle, 1);
        //                XLS.SatirYukseklikAyarla(satirEkle, satirEkle, GenelIslemler.JexcelBirimtoExcelBirim(400));
        //                XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 20, satirEkle, kaynakSutun);
        //                satirEkle++;
        //            }
        //            else
        //                satir--;

        //            if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur == (int)ENUMIslemTipi.DEGERARTTIR)
        //                && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
        //            {
        //                XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
        //                XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
        //                XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
        //                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilNo);
        //                XLS.HucreDegerYaz(satir, sutun + 4, detay.islemTipi.ad);

        //                if (!string.IsNullOrEmpty(detay.neredenGeldi))
        //                    XLS.HucreDegerYaz(satir, sutun + 5, detay.neredenGeldi);
        //                else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
        //                    XLS.HucreDegerYaz(satir, sutun + 5, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);

        //                XLS.HucreDegerYaz(satir, sutun + 6, detay.birimFiyatKDVLi);
        //                XLS.HucreDegerYaz(satir, sutun + 7, detay.miktar);
        //                XLS.HucreDegerYaz(satir, sutun + 8, detay.degerArtisi);
        //                //XLS.HucreDegerYaz(satir, sutun + 8, detay.miktar);
        //                XLS.HucreDegerYaz(satir, sutun + 9, detay.toplamDeger);
        //                XLS.HucreDegerYaz(satir, sutun + 10, detay.amortismanTutar);

        //                XLS.HucreDegerYaz(satir, sutun + 19, detay.ozellikler);

        //            }
        //            else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
        //                || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
        //                || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
        //            {
        //                XLS.HucreDegerYaz(satir, sutun, detay.cikisCinsi);
        //                XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
        //                XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
        //                XLS.HucreDegerYaz(satir, sutun + 3, detay.islemTipi.ad);

        //                if (!string.IsNullOrEmpty(detay.kimeVerildi))
        //                {
        //                    string ad = uzyServis.UzayDegeriStr(kullanan, "TASPERSONEL", detay.kimeVerildi, true, "");
        //                    if (string.IsNullOrEmpty(ad))
        //                        XLS.HucreDegerYaz(satir, sutun + 4, detay.kimeVerildi);
        //                    else
        //                        XLS.HucreDegerYaz(satir, sutun + 4, ad);
        //                }

        //                if (detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
        //                || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
        //                {
        //                    string odaAd = uzyServis.UzayDegeriStr(null, "TASODA", dtd.muhasebeKod + "-" + dtd.harcamaKod + "-" + detay.nereyeVerildi, true, "");
        //                    if (!string.IsNullOrEmpty(odaAd))
        //                        XLS.HucreDegerYaz(satir, sutun + 5, detay.nereyeVerildi + "-" + odaAd);
        //                    else
        //                        XLS.HucreDegerYaz(satir, sutun + 5, detay.nereyeVerildi);
        //                    XLS.HucreDegerYaz(satir, sutun + 7, detay.ozellikler);
        //                }
        //                else
        //                {
        //                    if (!string.IsNullOrEmpty(detay.nereyeGonderildi))
        //                        XLS.HucreDegerYaz(satir, sutun + 6, detay.nereyeGonderildi);
        //                    else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
        //                        XLS.HucreDegerYaz(satir, sutun + 6, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);

        //                    XLS.HucreDegerYaz(satir, sutun + 8, detay.toplamDeger);
        //                }
        //            }
        //            satir++;
        //        }

        //        XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 4, GenelIslemler.JexcelBirimtoExcelBirim(400));
        //        XLS.SayfaSonuKoyHucresel(satirEkle + 4);
        //        satirEkle += 4;
        //    }

        //    //Şablon olrak kullanılan ilk sayfayı sil
        //    XLS.SheetSil(0);
        //    XLS.DosyaSaklaTamYol();
        //    return this.sonucDosyaAd;
        //}

        string sablonAd;
        string sonucDosyaAd;

        public string RaporAl(object o_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            this.sablonAd = sablonAd;
            this.sonucDosyaAd = sonucDosyaAd;
            kullanan = (Kullanici)o_kullanan;
            ak = new TNS.TMM.AmortismanKriter();
            ak.yil = OrtakFonksiyonlar.ConvertToInt(raporParam["yil"], 0);
            ak.donem = OrtakFonksiyonlar.ConvertToInt(raporParam["donem"], 0);
            ak.muhasebeKod = (string)raporParam["muhasebeKod"];
            ak.harcamaKod = (string)raporParam["harcamaKod"];
            ak.ambarKod = (string)raporParam["ambarKod"];
            ak.hesapPlanKod = (string)raporParam["hesapPlanKod"];
            ak.kumulatif = (string)raporParam["kumulatif"];
            ak.raporTur = OrtakFonksiyonlar.ConvertToInt(raporParam["raporTur"], 0);
            int raporSekli = OrtakFonksiyonlar.ConvertToInt(raporParam["raporSekli"], 0);
            ak.ihracDahil = OrtakFonksiyonlar.ConvertToInt(raporParam["ihracDahil"], 0) == 1;

            string[] prSicilNolar = ((string)raporParam["prSicilNolar"] + "").Split(',');
            for (int i = 0; i < prSicilNolar.Length; i++)
            {
                if (string.IsNullOrEmpty(prSicilNolar[i])) continue;
                ak.prSicilNolar.Add(prSicilNolar[i]);
            }

            if (raporSekli == 99)//Muhasebat
                return AmortismanRaporlaMuhasebat(ak);
            else if (ekBilgi == "mif")
                return AmortismanMIFYazdir(ak);
            //else if (ekBilgi == "dtd")
            //    return DayanikliTasinirDefteri(ak);
            else
                return AmortismanRaporla3(ak);
        }
    }
}