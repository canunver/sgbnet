using OrtakClass;
using System;
using System.Collections;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r i�lem fi�i raporlama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TasinirIslemFormYazdir : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        static ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        static IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama, fisNo ve tifTur
        ///     girdi dizgileri kullan�larak istenen excel raporu haz�rlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0) == 0 || Request.QueryString["fisNo"] == string.Empty || Request.QueryString["harcama"] == string.Empty)
                return;

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            //Sayfaya giri� izni varm�?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                GenelIslemler.SayfayaGirmesin(true);

            int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0);
            string fisNo = Request.QueryString["fisNo"];
            string harcamaKod = Request.QueryString["harcama"].Replace(".", "");
            string muhasebeKod = Request.QueryString["muhasebe"].Replace(".", "");
            string tifTur = Request.QueryString["tifTur"].Trim();

            if (tifTur == "TIFSicil")
                TIFSicilYazdir(kullanan, yil, muhasebeKod, harcamaKod, fisNo);
            else
                Yazdir(kullanan, yil, fisNo, harcamaKod, muhasebeKod, "", tifTur);
        }

        /// <summary>
        /// Parametre olarak verilen ta��n�r i�lem fi�i kriterlerini sunucudaki ButunSicilNoListele
        /// yordam�na g�nderir, sunucudan gelen bilgi k�mesini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="yil">Y�l kriteri</param>
        /// <param name="muhasebeKod">Muhasebe birimi kodu</param>
        /// <param name="harcamaKod">Harcama birimi kodu</param>
        /// <param name="fisNo">Ta��n�r i�lem fi�inin belge numaras�</param>
        public static string TIFSicilYazdir(Kullanici kullanan, int yil, string muhasebeKod, string harcamaKod, string fisNo, bool dosyaAdDondur = false)
        {
            SicilNoHareket snh = new SicilNoHareket();
            snh.yil = yil;
            snh.muhasebeKod = muhasebeKod;
            snh.harcamaBirimKod = harcamaKod.Replace(".", "");
            snh.fisNo = fisNo.PadLeft(6, '0');

            string hata = "";
            if (snh.yil <= 0)
                hata += Resources.TasinirMal.FRMTIY001;
            if (string.IsNullOrEmpty(snh.muhasebeKod))
                hata += Resources.TasinirMal.FRMTIY002;
            if (string.IsNullOrEmpty(snh.harcamaBirimKod))
                hata += Resources.TasinirMal.FRMTIY003;
            if (string.IsNullOrEmpty(snh.fisNo))
                hata += Resources.TasinirMal.FRMTIY004;

            if (hata != "")
                return "";


            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                TIFYazMerkezBankasi(kullanan, snh.yil, snh.muhasebeKod, snh.harcamaBirimKod, snh.fisNo, "", 0);
                return "";
            }

            TNSDateTime faturaTarih = new TNSDateTime();
            string faturaNo = "";

            //***************  Fatura Tarihi i�in Tif Oku   ********************************
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm
            {
                yil = snh.yil,
                muhasebeKod = snh.muhasebeKod,
                harcamaKod = snh.harcamaBirimKod,
                fisNo = snh.fisNo
            };

            ObjectArray tfListe = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

            if (tfListe.sonuc.islemSonuc)
            {
                TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
                tform = (TNS.TMM.TasinirIslemForm)tfListe[0];

                faturaTarih = tform.faturaTarih;
                faturaNo = tform.faturaNo;
            }

            //****************************************************************

            ObjectArray bilgi = servisTMM.ButunSicilNoListele(kullanan, snh);

            if (!bilgi.sonuc.islemSonuc)
                return "";

            if (bilgi.objeler.Count <= 0)
                return "";

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TifSicilNumarasi.xlt";
            XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/TMM/" + sablonAd, sonucDosyaAd);

            SicilNoHareket sicil = (SicilNoHareket)bilgi.objeler[0];
            XLS.HucreAdBulYaz("HarcamaAd", sicil.harcamaBirimAd);
            XLS.HucreAdBulYaz("HarcamaKod", sicil.harcamaBirimKod);
            XLS.HucreAdBulYaz("AmbarAd", sicil.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", sicil.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", sicil.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", sicil.muhasebeKod);
            XLS.HucreAdBulYaz("FaturaTarih", faturaTarih.ToString());
            XLS.HucreAdBulYaz("FaturaNo", faturaNo);

            decimal toplamFiyat = 0;

            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int satir = kaynakSatir;

            int siraNo = 0;
            foreach (SicilNoHareket s in bilgi.objeler)
            {
                satir++;
                siraNo++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                XLS.HucreBirlestir(satir, sutun + 4, satir, sutun + 6);

                XLS.HucreDegerYaz(satir, sutun, siraNo);
                XLS.HucreDegerYaz(satir, sutun + 1, s.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 3, s.hesapPlanAd);

                if (TasinirGenel.rfIdVarMi)
                    XLS.HucreDegerYaz(satir, sutun + 4, s.sicilNo + " - " + s.rfIdNo);
                else
                    XLS.HucreDegerYaz(satir, sutun + 4, s.sicilNo);

                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(s.kdvliBirimFiyat.ToString(), (double)0));

                string alimYili = s.sicilNo.Substring(s.sicilNo.Length - 8);
                alimYili = alimYili.Substring(0, 2);
                int alimYiliInt = OrtakFonksiyonlar.ConvertToInt(alimYili, 0);
                if (alimYiliInt > 40)
                    alimYiliInt = 1900 + alimYiliInt;
                else
                    alimYiliInt = 2000 + alimYiliInt;

                XLS.HucreDegerYaz(satir, sutun + 8, alimYiliInt);
                XLS.HucreDegerYaz(satir, sutun + 9, s.ozellik.saseNo);

                toplamFiyat += s.kdvliBirimFiyat;
            }

            XLS.HucreDegerYaz(satir + 1, sutun + 7, (double)toplamFiyat);

            //Eklenen sat�rlar�n y�kseklikleri ayarlan�yor
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();

            if (dosyaAdDondur)
                return sonucDosyaAd;

            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());

            return "";
        }

        /// <summary>
        /// Parametre olarak verilen ta��n�r i�lem fi�i kriterleriyle ilgili ta��n�r i�lem fi�i bilgilerini
        /// sunucudan al�r ve tifTur parametresine g�re TIFYaz yordam�na veya TIF5AYaz yordam�na y�nlendirir.
        /// </summary>
        /// <param name="kullanan">��lemi yapan kullan�c�ya ait bilgileri tutan nesne</param>
        /// <param name="yil">Y�l kriteri</param>
        /// <param name="fisNo">Ta��n�r i�lem fi�inin belge numaras�</param>
        /// <param name="harcamaKod">Harcama birimi kodu</param>
        /// <param name="muhasebeKod">Muhasebe birimi kodu</param>
        /// <param name="excelYazYer">Rapor g�nderilirken kullan�lacak dosya ad�</param>
        /// <param name="tifTur">K�t�phane ta��n�r i�lem fi�i mi, m�ze ta��n�r i�lem fi�i mi yoksa di�er ta��n�r i�lem fi�i mi bilgisi</param>
        public static void Yazdir(Kullanici kullanan, int yil, string fisNo, string harcamaKod, string muhasebeKod, string excelYazYer, string tifTur, int islemTipi = 0)
        {
            DateTime dt = DateTime.Now;

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                TIFYazMerkezBankasi(kullanan, yil, muhasebeKod, harcamaKod, fisNo, excelYazYer, islemTipi);
            else
            {
                TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                tf.yil = yil;
                tf.fisNo = fisNo;
                tf.harcamaKod = harcamaKod;
                tf.muhasebeKod = muhasebeKod;

                ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, false);

                if (!bilgi.sonuc.islemSonuc)
                    return;

                tf = (TNS.TMM.TasinirIslemForm)bilgi[0];

                bool muhasebeKdvMukellefiMi = servisTMM.MuhasebeKdvMukellefiMi(kullanan, tf.muhasebeKod);

                tf.islemTipAd = servisUZY.UzayDegeriStr(null, "TASISLEMTIPAD", tf.islemTipKod.ToString(), true, "");


                TimeSpan ts = DateTime.Now - dt;
                OrtakFonksiyonlar.HataDosyaYaz("TasinirDetaySorgu.txt", DateTime.Now.ToString() + " - (" + tf.detay.ObjeSayisi + " adet) Data geldi Rapor haz�rlanmaya ba�lad� " + ts.TotalMilliseconds + " ms s�rd�", false);

                if (string.IsNullOrEmpty(tifTur))
                {
                    if (tf.islemTipTur == (int)ENUMIslemTipi.BAGISCIKIS && tf.islemTipAd.ToUpper().Contains(Resources.TasinirMal.FRMTIY035))
                        TIFYaz_Hediyelik(tf, excelYazYer, muhasebeKdvMukellefiMi);
                    else
                        TIFYaz(tf, excelYazYer, muhasebeKdvMukellefiMi);
                }
                else if (tifTur == "kutuphane" || tifTur == "muze")
                    TIF5AYaz(tf, excelYazYer, muhasebeKdvMukellefiMi);
                else if (tifTur == "dagitimIade")
                    TIFDagitimIadeYaz(tf, excelYazYer, muhasebeKdvMukellefiMi);

                ts = DateTime.Now - dt;
                OrtakFonksiyonlar.HataDosyaYaz("TasinirDetaySorgu.txt", DateTime.Now.ToString() + " - Raporun tamamlanmas� " + ts.TotalMilliseconds + " ms s�rd�", false);

            }
        }

        public static string OzellikVer(TNS.TMM.SicilNoOzellik sh)
        {
            string ozellik = "";

            if (!String.IsNullOrEmpty(sh.markaAd))
                ozellik = sh.markaAd;
            if (!String.IsNullOrEmpty(sh.modelAd))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += sh.modelAd;
            }
            if (!String.IsNullOrEmpty(sh.adi))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += sh.adi;
            }
            if (!String.IsNullOrEmpty(sh.yazarAdi))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += sh.yazarAdi;
            }
            if (!String.IsNullOrEmpty(sh.plaka))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += sh.plaka;
            }
            if (!String.IsNullOrEmpty(sh.disSicilNo))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += sh.disSicilNo;
            }
            if (!String.IsNullOrEmpty(sh.saseNo))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += sh.saseNo;
            }
            if (!String.IsNullOrEmpty(sh.yeriKonusu))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += sh.yeriKonusu;
            }
            if (ozellik != "") ozellik = " (" + ozellik + ")";

            return ozellik;
        }

        /// <summary>
        /// Parametre olarak verilen ta��n�r i�lem fi�i bilgilerini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="tf">Ta��n�r i�lem fi�i bilgilerini tutan nesne</param>
        /// <param name="excelYazYer">Rapor g�nderilirken kullan�lacak dosya ad�</param>
        /// <param name="muhasebeKdvMukellefiMi">Ta��n�r i�lem fi�inin ait oldu�u muhasebe birimi KDV m�kellefi mi, de�il mi bilgisi</param>
        private static void TIFYaz(TNS.TMM.TasinirIslemForm tf, string excelYazYer, bool muhasebeKdvMukellefiMi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int satir = 0;
            int sutun = 0;
            int siraNo = 0;
            int islemTur = 0;
            Hashtable htDoviz = null;
            string[] kurlar = null;
            bool manas = false;
            int defaultYukseklik = 14;

            string sablonAd = "TIF.XLT";
            if (GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "") == "1399")
            {
                manas = true;
                sablonAd = "TIF1399.XLT";
                TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

                TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();
                kriter.kurTarih = tf.faturaTarih;
                if (kriter.kurTarih.isNull)
                    kriter.kurTarih = tf.fisTarih;

                kurlar = new string[] { "KGS", "TRY" };

                ObjectArray liste = servisHRC.MerkezBankKurDegerleri(kriter, kurlar);
                htDoviz = new Hashtable();
                foreach (TNS.HRC.KurDegerleri kur in liste.objeler)
                {
                    htDoviz[kur.paraBirimi.kisaltma] = Convert.ToDecimal(kur.alis);
                }
            }

            string sonucDosyaAd = "";
            bool gonder = false;
            if (excelYazYer == "")
            {
                sonucDosyaAd = System.IO.Path.GetTempFileName();
                gonder = true;
            }
            else
                sonucDosyaAd = excelYazYer;

            //Detay �ekmek i�in olu�turulan nesne
            TNS.TMM.TasinirIslemForm sorguForm = new TNS.TMM.TasinirIslemForm();
            sorguForm.yil = tf.yil;
            sorguForm.muhasebeKod = tf.muhasebeKod;
            sorguForm.harcamaKod = tf.harcamaKod;
            sorguForm.fisNo = tf.fisNo;

            ObjectArray sListe = servisTMM.TasinirSurecNoListele(sorguForm);
            string surecNo = "";
            foreach (TasinirIslemMIF tif in sListe.objeler)
            {
                surecNo = tif.mifBelgeNo;
            }

            XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/TMM/" + sablonAd, sonucDosyaAd);

            int maxLen = 50;

            if (tf.muhasebeAd.Length > maxLen) tf.muhasebeAd = tf.muhasebeAd.Substring(0, maxLen - 3) + "...";
            if (tf.harcamaAd.Length > maxLen) tf.harcamaAd = tf.harcamaAd.Substring(0, maxLen - 3) + "...";
            if (tf.ambarAd.Length > maxLen) tf.ambarAd = tf.ambarAd.Substring(0, maxLen - 3) + "...";

            if (tf.gMuhasebeAd.Length > maxLen) tf.gMuhasebeAd = tf.gMuhasebeAd.Substring(0, maxLen - 3) + "...";
            if (tf.gHarcamaAd.Length > maxLen) tf.gHarcamaAd = tf.gHarcamaAd.Substring(0, maxLen - 3) + "...";
            if (tf.gAmbarAd.Length > maxLen) tf.gAmbarAd = tf.gAmbarAd.Substring(0, maxLen - 3) + "...";

            if (tf.neredenGeldi.Length > (maxLen + 10)) tf.neredenGeldi = tf.neredenGeldi.Substring(0, (maxLen + 10) - 3) + "...";
            if (tf.nereyeGitti.Length > maxLen) tf.nereyeGitti = tf.nereyeGitti.Substring(0, maxLen - 3) + "...";

            if (tf.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS)
            {
                XLS.HucreAdBulYaz("BaslikBirimFiyat", "B�R�M F�YATI");
                XLS.HucreAdBulYaz("BaslikTutar", "SATI� BEDEL�");
            }

            XLS.HucreAdBulYaz("FisNo", tf.fisNo);
            XLS.HucreAdBulYaz("FisTarih", tf.fisTarih.ToString());
            XLS.HucreAdBulYaz("IlAd", tf.ilAd + "-" + tf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tf.ilKod + "-" + tf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tf.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tf.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tf.muhasebeKod);
            XLS.HucreAdBulYaz("MuayeneTarih", tf.muayeneTarih.ToString());
            XLS.HucreAdBulYaz("MuayeneSayi", tf.muayeneNo);
            XLS.HucreAdBulYaz("DayanakTarih", tf.dayanakTarih.ToString());
            XLS.HucreAdBulYaz("DayanakSayi", tf.dayanakNo);
            XLS.HucreAdBulYaz("Aciklama", tf.aciklama);
            //XLS.HucreAdBulYaz("FaturaTarih", tf.faturaTarih.ToString());
            //XLS.HucreAdBulYaz("FaturaSayi", tf.faturaNo);

            XLS.HucreAdBulYaz("txtSurecNo", surecNo);
            XLS.HucreAdBulYaz("IslemCesit", tf.islemTipAd);
            XLS.HucreAdBulYaz("NeredenGeldi", tf.neredenGeldi);

            if (!string.IsNullOrEmpty(tf.kimeGitti))
            {
                string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", tf.kimeGitti, true, "");

                if (ad.Length > (maxLen - 10)) ad = ad.Substring(0, (maxLen - 10) - 3) + "...";
                if (tf.kimeGitti.Length > (maxLen - 10)) tf.kimeGitti = tf.kimeGitti.Substring(0, (maxLen - 10) - 3) + "...";

                if (string.IsNullOrEmpty(ad))
                    XLS.HucreAdBulYaz("KimeVerildi", tf.kimeGitti);
                else
                    XLS.HucreAdBulYaz("KimeVerildi", ad);
            }

            //if (tf.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
            //{
            //    int gonSatir = 0;
            //    int gonSutun = 0;
            //    XLS.HucreAdAdresCoz("GonHarcamaAd", ref gonSatir, ref gonSutun);
            //    XLS.HucreDegerYaz(gonSatir, 0, Resources.TasinirMal.FRMTIY043);
            //    XLS.HucreAdAdresCoz("GonAmbarAd", ref gonSatir, ref gonSutun);
            //    XLS.HucreDegerYaz(gonSatir, 0, Resources.TasinirMal.FRMTIY044);
            //    XLS.HucreAdAdresCoz("GonMuhasebeAd", ref gonSatir, ref gonSutun);
            //    XLS.HucreDegerYaz(gonSatir, 0, Resources.TasinirMal.FRMTIY045);
            //}

            XLS.HucreAdBulYaz("NereyeVerildi", tf.nereyeGitti);
            XLS.HucreAdBulYaz("GonHarcamaAd", tf.gHarcamaAd);
            XLS.HucreAdBulYaz("GonHarcamaKod", tf.gHarcamaKod);
            XLS.HucreAdBulYaz("GonAmbarAd", tf.gAmbarAd);
            XLS.HucreAdBulYaz("GonAmbarKod", tf.gAmbarKod);
            XLS.HucreAdBulYaz("GonMuhasebeAd", tf.gMuhasebeAd);
            XLS.HucreAdBulYaz("GonMuhasebeKod", tf.gMuhasebeKod);

            decimal miktarToplam = (decimal)0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            string eskiHesap = string.Empty;
            decimal araToplam = (decimal)0;
            decimal[] araToplamKur = new decimal[2];
            decimal toplam = (decimal)0;
            decimal[] toplamKur = new decimal[2];
            decimal[] kdvToplam = new decimal[99];//Bir grupta ka� tane farkl� KDV olabilir veya Grupdaki KDV Grup toplamlar� i�in kullan�l�yor

            if (GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "") == "1399")
            {
                araToplamKur = new decimal[kurlar.Length];
                toplamKur = new decimal[kurlar.Length];
            }
            else
            {
                araToplamKur = null;
                toplamKur = null;
            }

            int baslaSatir = kaynakSatir + 1;
            //int sayfadakiSatir = 74;
            //int cevapSatir = tf.detay.objeler.Count - 1;
            //int baslikSatirSayisi = 16;
            //int imzaSatirSayisi = 15;
            //int sayac = baslikSatirSayisi;

            ////aratoplamlarla birlikte ka� sat�r
            //foreach (TasinirIslemDetay td in tf.detay.objeler)
            //{
            //    if (td.hesapPlanKod.Length >= 9)
            //    {
            //        if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
            //        {
            //            cevapSatir++;
            //            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            //            {
            //                //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
            //                for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
            //                {
            //                    if (kdvToplam[i] > 0)
            //                        cevapSatir += (int)kdvToplam[i];
            //                }
            //                kdvToplam = new decimal[99];
            //            }
            //        }
            //        kdvToplam[td.kdvOran]++;
            //        eskiHesap = td.hesapPlanKod.Substring(0, 9);
            //    }
            //}

            ////Genel toplam i�in 2 ekleniyor. En son grup toplam� ile birlikte
            //cevapSatir += 2;

            ////KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m (Son grup i�in)
            //if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            //{
            //    //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
            //    for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
            //    {
            //        if (kdvToplam[i] > 0)
            //            cevapSatir += (int)kdvToplam[i];
            //    }
            //    kdvToplam = new decimal[99];
            //}

            ////Ba�l�k sat�r say�s�
            //cevapSatir += baslikSatirSayisi;

            ////�mza sat�r say�s�
            //cevapSatir += imzaSatirSayisi;
            //if (cevapSatir > ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
            //{
            //    int fark = cevapSatir - ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir;
            //    if (fark < imzaSatirSayisi)
            //        sayfadakiSatir -= fark;
            //}

            //int acSatir = 0;
            //if (cevapSatir != ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
            //    acSatir = ((cevapSatir / sayfadakiSatir) + 1) * sayfadakiSatir;
            //else
            //    acSatir = (cevapSatir / sayfadakiSatir) * sayfadakiSatir;

            //acSatir -= baslikSatirSayisi + imzaSatirSayisi + 1;//+1 sonradan eklendi 31.12.2016 Melih
            //XLS.SatirKopyalaAc(kaynakSatir, acSatir - 1, kaynakSatir + 1);

            //A�a��daki d�ng�de gerekli
            eskiHesap = "";
            decimal birimFiyat = 0;
            double satirYukseklik = 0;
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                //if (sayac % sayfadakiSatir == 0 && sayac > 10)
                //{
                //    //XLS.SayfaSonuKoyHucresel(satir);
                //    sayac = 0;
                //}

                if (td.hesapPlanKod.Length >= 9)
                {
                    if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
                    {
                        toplam += araToplam;

                        if (kurlar != null)
                        {
                            for (int i = 0; i < kurlar.Length; i++)
                            {
                                if (toplamKur != null && araToplamKur != null)
                                    toplamKur[i] += araToplamKur[i];

                            }
                        }

                        ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY005, eskiHesap), araToplam, araToplamKur);
                        //sayac++;

                        araToplam = (decimal)0;
                        if (kurlar != null)
                        {
                            for (int i = 0; i < kurlar.Length; i++)
                            {
                                if (araToplamKur != null)
                                    araToplamKur[i] = (decimal)0;
                            }
                        }

                        //KDV Toplam
                        if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
                        {
                            for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                            {
                                if (kdvToplam[i] > 0)
                                {
                                    ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY006, i.ToString()), kdvToplam[i]);
                                    toplam += kdvToplam[i];
                                    //sayac++;
                                }
                            }
                            kdvToplam = new decimal[99];
                        }
                    }
                }

                //if (sayac % sayfadakiSatir == 0 && sayac > 10)
                //{
                //    //XLS.SayfaSonuKoyHucresel(satir);
                //    sayac = 0;
                //}

                if (td.hesapPlanKod.Length >= 9)
                    eskiHesap = td.hesapPlanKod.Substring(0, 9);

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 15, satir, 0);
                //sayac++;

                XLS.HucreDegerYaz(satir, sutun, ++siraNo);

                if (manas)
                {
                    string hk = td.hesapPlanKod;
                    string ha = td.hesapPlanAd;
                    string ozellik = "";

                    if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                        tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                    {
                        if (td.sicilNo > 0)
                        {
                            Kullanici k = new Kullanici();
                            k.KullaniciTipiEkle(ENUMKullaniciTipi.SISTEMYONETICISI);
                            ObjectArray oliste = servisTMM.SicilNoOzellikListele(k, td.sicilNo);
                            foreach (TNS.TMM.SicilNoOzellik so in oliste.objeler)
                            {
                                ozellik = OzellikVer(so);
                            }
                        }
                    }
                    else
                    {
                        ozellik = OzellikVer(td.ozellik);
                    }
                    if (ozellik != "") ha += " / " + ozellik;

                    if (td.gorSicilNo != "") hk = td.gorSicilNo;
                    XLS.HucreDegerYaz(satir, sutun + 1, hk);
                    XLS.HucreDegerYaz(satir, sutun + 2, ha);

                    XLS.OtomatikYukseklik(XLS.AktifSheet(), satir, sutun + 2, sutun + 4);
                    satirYukseklik = XLS.SatirGercekYukseklikAl(satir) / 1.5;
                    if (satirYukseklik < defaultYukseklik) satirYukseklik = defaultYukseklik;
                    XLS.SatirGercekYukseklikAyarla(satir, satir, satirYukseklik);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);
                }
                else
                {
                    XLS.HucreDegerYaz(satir, sutun + 1, td.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, td.gorSicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 3, td.hesapPlanAd);

                    XLS.OtomatikYukseklik(XLS.AktifSheet(), satir, sutun + 3, sutun + 4);
                    satirYukseklik = XLS.SatirGercekYukseklikAl(satir) / 1.5;
                    if (satirYukseklik < defaultYukseklik) satirYukseklik = defaultYukseklik;
                    XLS.SatirGercekYukseklikAyarla(satir, satir, satirYukseklik);
                    XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                }

                XLS.HucreDegerYaz(satir, sutun + 5, td.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(td.miktar.ToString(), (double)0));
                miktarToplam += td.miktar;

                birimFiyat = td.birimFiyatKDVLi;

                if (tf.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS)
                    birimFiyat = Math.Round(birimFiyat, 2);

                if (tf.islemTipTur == (int)ENUMIslemTipi.ENFLASYONARTISI)
                {
                    birimFiyat = Math.Round(td.satisBedeli, 2);
                    td.kdvOran = 0;
                }

                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(birimFiyat.ToString(), (double)0));

                if (tf.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS && TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    birimFiyat = td.satisBedeli;

                double tutar = OrtakFonksiyonlar.ConvertToDouble(td.miktar * birimFiyat);

                XLS.HucreDegerYaz(satir, sutun + 8, tutar.ToString("#,###.00"));

                if (htDoviz != null && kurlar != null)
                {
                    for (int i = 0; i < kurlar.Length; i++)
                    {
                        decimal k1 = 0;
                        if (htDoviz[kurlar[i]] != null)
                        {
                            k1 = (decimal)htDoviz[kurlar[i]];
                            if (k1 > 0)
                            {
                                decimal t1 = OrtakFonksiyonlar.ConvertToDecimal(tutar) / k1;
                                XLS.HucreDegerYaz(satir, sutun + 9 + i, t1.ToString("#,###.00"));
                                if (araToplamKur != null)
                                    araToplamKur[i] += OrtakFonksiyonlar.ConvertToDecimal(t1.ToString("#,###.00"), (decimal)0);
                            }
                        }
                    }
                }

                araToplam += OrtakFonksiyonlar.ConvertToDecimal(tutar.ToString("#,###.00"), (decimal)0);
                kdvToplam[td.kdvOran] += OrtakFonksiyonlar.ConvertToDecimal(td.miktar) * (birimFiyat * (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100));
            }

            //Ara Toplam
            toplam += araToplam;
            if (kurlar != null)
            {
                for (int i = 0; i < kurlar.Length; i++)
                {
                    if (toplamKur != null && araToplamKur != null)
                        toplamKur[i] += araToplamKur[i];
                }
            }

            ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY005, eskiHesap), araToplam, araToplamKur);

            //KDV Toplamlar�
            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                {
                    if (kdvToplam[i] > 0)
                    {
                        ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY006, i.ToString()), kdvToplam[i]);
                        toplam += kdvToplam[i];
                    }
                }
            }

            //Genel Toplam
            ToplamEkle(XLS, kaynakSatir, ref satir, sutun, Resources.TasinirMal.FRMTIY007, toplam, toplamKur);

            islemTur = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(null, "TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "").ToString(), 0);
            ImzaEkle(XLS, satir, sutun, tf.muhasebeKod.Trim(), tf.harcamaKod.Trim(), tf.ambarKod.Trim(), tf.detay.objeler.Count, miktarToplam, islemTur, toplam);

            double yukseklik = 0;
            for (int i = 0; i < satir + 17; i++)//17 imza sat�r say�s�
            {
                yukseklik += XLS.SatirGercekYukseklikAl(i);
            }

            int sayfaYuksekligi = 930;
            int modSayfa = OrtakFonksiyonlar.ConvertToInt(yukseklik, 0) / sayfaYuksekligi;

            int farkYukseklik = OrtakFonksiyonlar.ConvertToInt((sayfaYuksekligi * (modSayfa + 1) - yukseklik), 0) / defaultYukseklik;

            for (int i = 0; i < farkYukseklik; i++)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 15, satir, 0);
                XLS.SatirGercekYukseklikAyarla(satir, satir, defaultYukseklik);
            }


            //if (!manas)
            //XLS.SatirYukseklikAyarla(0, kaynakSatir + acSatir + 31, GenelIslemler.JexcelBirimtoExcelBirim(260));

            XLS.SatirGizle(8, 8, true);//Fatura
            //XLS.SatirGizle(baslikSatirSayisi + 1, baslikSatirSayisi + 1, true);
            XLS.DosyaSaklaTamYol();

            if (gonder)
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, tf.fisNo, true, GenelIslemler.ExcelTur());
        }

        private static void TIFYaz_Hediyelik(TNS.TMM.TasinirIslemForm tf, string excelYazYer, bool muhasebeKdvMukellefiMi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int satir = 0;
            int sutun = 0;
            int siraNo = 0;

            string sablonAd = "TIF_Hediyelik.xlt";

            string sonucDosyaAd = "";
            bool gonder = false;
            if (excelYazYer == "")
            {
                sonucDosyaAd = System.IO.Path.GetTempFileName();
                gonder = true;
            }
            else
                sonucDosyaAd = excelYazYer;

            XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/TMM/" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("HarcamaAd", tf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tf.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tf.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tf.muhasebeKod);
            XLS.HucreAdBulYaz("DayanakTarih", tf.dayanakTarih.ToString());
            XLS.HucreAdBulYaz("DayanakSayi", tf.dayanakNo);
            XLS.HucreAdBulYaz("IslemCesit", tf.islemTipAd);
            XLS.HucreAdBulYaz("NeredenGeldi", tf.neredenGeldi);

            if (!string.IsNullOrEmpty(tf.kimeGitti))
            {
                string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", tf.kimeGitti, true, "");
                if (string.IsNullOrEmpty(ad))
                    XLS.HucreAdBulYaz("NereyeVerildi", tf.kimeGitti);
                else
                    XLS.HucreAdBulYaz("NereyeVerildi", ad);
            }
            else
                XLS.HucreAdBulYaz("NereyeVerildi", tf.nereyeGitti);

            XLS.HucreAdBulYaz("GonHarcamaAd", tf.gHarcamaAd);
            XLS.HucreAdBulYaz("GonHarcamaKod", tf.gHarcamaKod);
            XLS.HucreAdBulYaz("GonAmbarAd", tf.gAmbarAd);
            XLS.HucreAdBulYaz("GonAmbarKod", tf.gAmbarKod);
            XLS.HucreAdBulYaz("GonMuhasebeAd", tf.gMuhasebeAd);
            XLS.HucreAdBulYaz("GonMuhasebeKod", tf.gMuhasebeKod);

            decimal miktarToplam = (decimal)0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            string eskiHesap = string.Empty;
            decimal araToplam = (decimal)0;
            decimal toplam = (decimal)0;
            decimal[] kdvToplam = new decimal[99];//Bir grupta ka� tane farkl� KDV olabilir veya Grupdaki KDV Grup toplamlar� i�in kullan�l�yor

            int baslaSatir = kaynakSatir + 1;
            int sayfadakiSatir = 74;
            int cevapSatir = tf.detay.objeler.Count - 1;
            int baslikSatirSayisi = 15;
            int imzaSatirSayisi = 15;
            //int sayac = baslikSatirSayisi;

            //aratoplamlarla birlikte ka� sat�r
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                if (td.hesapPlanKod.Length >= 9)
                {
                    if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
                    {
                        cevapSatir++;
                        if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
                        {
                            //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
                            for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
                            {
                                if (kdvToplam[i] > 0)
                                    cevapSatir += (int)kdvToplam[i];
                            }
                            kdvToplam = new decimal[99];
                        }
                    }
                    kdvToplam[td.kdvOran]++;
                    eskiHesap = td.hesapPlanKod.Substring(0, 9);
                }
            }

            //Genel toplam i�in 2 ekleniyor. En son grup toplam� ile birlikte
            cevapSatir += 2;

            //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m (Son grup i�in)
            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
                for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
                {
                    if (kdvToplam[i] > 0)
                        cevapSatir += (int)kdvToplam[i];
                }
                kdvToplam = new decimal[99];
            }

            //Ba�l�k sat�r say�s�
            cevapSatir += baslikSatirSayisi;

            //�mza sat�r say�s�
            cevapSatir += imzaSatirSayisi;
            if (cevapSatir > ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
            {
                int fark = cevapSatir - ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir;
                if (fark < imzaSatirSayisi)
                    sayfadakiSatir -= fark;
            }

            int acSatir = 0;
            if (cevapSatir != ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
                acSatir = ((cevapSatir / sayfadakiSatir) + 1) * sayfadakiSatir;
            else
                acSatir = (cevapSatir / sayfadakiSatir) * sayfadakiSatir;

            acSatir -= baslikSatirSayisi + imzaSatirSayisi;
            XLS.SatirKopyalaAc(kaynakSatir, acSatir - 1, kaynakSatir + 1);

            //A�a��daki d�ng�de gerekli
            eskiHesap = "";
            decimal birimFiyat = 0;
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                //if (sayac % sayfadakiSatir == 0 && sayac > 10)
                //{
                //    //XLS.SayfaSonuKoyHucresel(satir);
                //    sayac = 0;
                //}

                if (td.hesapPlanKod.Length >= 9)
                {
                    if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
                    {
                        toplam += araToplam;
                        ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY005, eskiHesap), araToplam);
                        //sayac++;
                        araToplam = (decimal)0;

                        //KDV Toplam
                        if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
                        {
                            for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                            {
                                if (kdvToplam[i] > 0)
                                {
                                    ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY006, i.ToString()), kdvToplam[i]);
                                    toplam += kdvToplam[i];
                                    //sayac++;
                                }
                            }
                            kdvToplam = new decimal[99];
                        }
                    }
                }

                //if (sayac % sayfadakiSatir == 0 && sayac > 10)
                //{
                //    //XLS.SayfaSonuKoyHucresel(satir);
                //    sayac = 0;
                //}

                if (td.hesapPlanKod.Length >= 9)
                    eskiHesap = td.hesapPlanKod.Substring(0, 9);

                satir++;
                //sayac++;
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                XLS.HucreDegerYaz(satir, sutun, ++siraNo);
                XLS.HucreDegerYaz(satir, sutun + 1, td.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, td.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 4, td.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(td.miktar.ToString(), (double)0));
                miktarToplam += td.miktar;

                birimFiyat = td.birimFiyatKDVLi;

                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble((td.miktar * birimFiyat).ToString("#,###.00"), (double)0));
                araToplam += OrtakFonksiyonlar.ConvertToDecimal((td.miktar * birimFiyat).ToString("#,###.00"), (decimal)0);
                kdvToplam[td.kdvOran] += OrtakFonksiyonlar.ConvertToDecimal(td.miktar) * (birimFiyat * (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100));
            }

            //Ara Toplam
            toplam += araToplam;
            ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY005, eskiHesap), araToplam);

            //KDV Toplamlar�
            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                {
                    if (kdvToplam[i] > 0)
                    {
                        ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY006, i.ToString()), kdvToplam[i]);
                        toplam += kdvToplam[i];
                    }
                }
            }

            //Genel Toplam
            ToplamEkle(XLS, kaynakSatir, ref satir, sutun, Resources.TasinirMal.FRMTIY007, toplam);
            ImzaEkle_Hediyelik(XLS, kaynakSatir + acSatir, sutun);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, kaynakSatir + acSatir, GenelIslemler.JexcelBirimtoExcelBirim(260));
            XLS.DosyaSaklaTamYol();

            if (gonder)
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, tf.fisNo, true, GenelIslemler.ExcelTur());
        }

        private static void ImzaEkle_Hediyelik(Tablo XLS, int satir, int sutun)
        {
            int icSatir = satir + 2;
            XLS.HucreBirlestir(icSatir, sutun, icSatir, sutun + 8);
            XLS.HucreDegerYaz(icSatir, sutun, Resources.TasinirMal.FRMTIY036);

            icSatir++;
            XLS.HucreBirlestir(icSatir, sutun, icSatir, sutun + 8);
            XLS.HucreDegerYaz(icSatir, sutun, Resources.TasinirMal.FRMTIY037);

            icSatir += 2;
            XLS.HucreBirlestir(icSatir, sutun, icSatir, sutun + 1);
            XLS.HucreDegerYaz(icSatir, sutun, Resources.TasinirMal.FRMTIY038);
            XLS.KoyuYap(icSatir, sutun, true);
            XLS.DuseyHizala(icSatir, sutun, 2);

            XLS.HucreBirlestir(icSatir, sutun + 6, icSatir, sutun + 7);
            XLS.HucreDegerYaz(icSatir, sutun + 6, Resources.TasinirMal.FRMTIY039);
            XLS.KoyuYap(icSatir, sutun + 6, true);
            XLS.DuseyHizala(icSatir, sutun + 6, 2);

            icSatir += 2;
            XLS.HucreBirlestir(icSatir, sutun, icSatir, sutun + 1);
            XLS.HucreDegerYaz(icSatir, sutun, ".............................");
            XLS.DuseyHizala(icSatir, sutun, 2);

            XLS.HucreBirlestir(icSatir, sutun + 2, icSatir, sutun + 3);
            XLS.HucreDegerYaz(icSatir, sutun + 2, ".............................");
            XLS.DuseyHizala(icSatir, sutun + 2, 2);

            icSatir++;
            XLS.HucreBirlestir(icSatir, sutun, icSatir, sutun + 1);
            XLS.HucreDegerYaz(icSatir, sutun, Resources.TasinirMal.FRMTIY041);
            XLS.DuseyHizala(icSatir, sutun, 2);

            XLS.HucreBirlestir(icSatir, sutun + 2, icSatir, sutun + 3);
            XLS.HucreDegerYaz(icSatir, sutun + 2, Resources.TasinirMal.FRMTIY042);
            XLS.DuseyHizala(icSatir, sutun + 2, 2);

            icSatir += 2;
            XLS.HucreDegerYaz(icSatir, sutun + 4, Resources.TasinirMal.FRMTIY040);
            XLS.KoyuYap(icSatir, sutun + 4, true);
            XLS.DuseyHizala(icSatir, sutun + 4, 2);

            icSatir++;
            XLS.HucreDegerYaz(icSatir, sutun + 4, "...../...../.....");
            XLS.DuseyHizala(icSatir, sutun + 4, 2);

            icSatir += 3;
            XLS.HucreBirlestir(icSatir, sutun + 3, icSatir, sutun + 5);
            XLS.HucreDegerYaz(icSatir, sutun + 3, "..............................................");
            XLS.DuseyHizala(icSatir, sutun + 3, 2);

            icSatir++;
            XLS.HucreBirlestir(icSatir, sutun + 3, icSatir, sutun + 5);
            XLS.HucreDegerYaz(icSatir, sutun + 3, "..............................................");
            XLS.DuseyHizala(icSatir, sutun + 3, 2);

            XLS.SatirYukseklikAyarla(satir + 2, satir + 3, GenelIslemler.JexcelBirimtoExcelBirim(450));
            XLS.SatirYukseklikAyarla(satir + 4, icSatir, GenelIslemler.JexcelBirimtoExcelBirim(250));
        }

        /// <summary>
        /// Parametre olarak verilen k�t�phane/m�ze ta��n�r i�lem fi�i
        /// bilgilerini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="tf">Ta��n�r i�lem fi�i bilgilerini tutan nesne</param>
        /// <param name="excelYazYer">Rapor g�nderilirken kullan�lacak dosya ad�</param>
        /// <param name="muhasebeKdvMukellefiMi">Ta��n�r i�lem fi�inin ait oldu�u muhasebe birimi KDV m�kellefi mi, de�il mi bilgisi</param>
        private static void TIF5AYaz(TNS.TMM.TasinirIslemForm tf, string excelYazYer, bool muhasebeKdvMukellefiMi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int satir = 0;
            int sutun = 0;
            int siraNo = 0;
            int islemTur = 0;

            string sablonAd = "TIF5A.XLT";

            string sonucDosyaAd = "";
            bool gonder = false;
            if (excelYazYer == "")
            {
                sonucDosyaAd = System.IO.Path.GetTempFileName();
                gonder = true;
            }
            else
                sonucDosyaAd = excelYazYer;

            XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/TMM/" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("BelgeNo", tf.fisNo);
            XLS.HucreAdBulYaz("BelgeTarih", tf.fisTarih.ToString());
            XLS.HucreAdBulYaz("IlAd", tf.ilAd + "-" + tf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tf.ilKod + "-" + tf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tf.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tf.muhasebeKod);
            XLS.HucreAdBulYaz("AmbarAd", tf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tf.ambarKod);
            XLS.HucreAdBulYaz("KomisyonTarih", tf.muayeneTarih.ToString());
            XLS.HucreAdBulYaz("KomisyonSayi", tf.muayeneNo);
            XLS.HucreAdBulYaz("DayanakTarih", tf.dayanakTarih.ToString());
            XLS.HucreAdBulYaz("DayanakSayi", tf.dayanakNo);

            XLS.HucreAdBulYaz("IslemCesit", tf.islemTipAd);

            XLS.HucreAdBulYaz("NeredenGeldi", tf.neredenGeldi);

            if (!string.IsNullOrEmpty(tf.kimeGitti))
            {
                string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", tf.kimeGitti, true, "");
                if (string.IsNullOrEmpty(ad))
                    XLS.HucreAdBulYaz("KimeVerildi", tf.kimeGitti);
                else
                    XLS.HucreAdBulYaz("KimeVerildi", ad);
            }

            XLS.HucreAdBulYaz("NereyeVerildi", tf.nereyeGitti);
            //XLS.HucreAdBulYaz("Puan", tf.);
            XLS.HucreAdBulYaz("GonHarcamaAd", tf.gHarcamaAd);
            XLS.HucreAdBulYaz("GonHarcamaKod", tf.gHarcamaKod);
            XLS.HucreAdBulYaz("GonAmbarAd", tf.gAmbarAd);
            XLS.HucreAdBulYaz("GonAmbarKod", tf.gAmbarKod);
            XLS.HucreAdBulYaz("GonMuhasebeAd", tf.gMuhasebeAd);
            XLS.HucreAdBulYaz("GonMuhasebeKod", tf.gMuhasebeKod);

            Kullanici kul = OturumBilgisiIslem.KullaniciBilgiOku(true);
            ObjectArray imza = servisTMM.ImzaListele(kul, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod, (int)ENUMImzaYer.TASINIRKAYITYETKILISI);

            ImzaBilgisi iBilgi = new ImzaBilgisi();
            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
                iBilgi = (ImzaBilgisi)imza[0];

            islemTur = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(null, "TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "").ToString(), 0);

            if (islemTur < 50)
            {
                XLS.HucreAdBulYaz("Tarih1", DateTime.Today.Date.ToShortDateString());
                XLS.HucreAdBulYaz("Tarih2", DateTime.Today.Date.ToShortDateString());
                XLS.HucreAdBulYaz("AdSoyad1", iBilgi.adSoyad);
                XLS.HucreAdBulYaz("Unvan1", iBilgi.unvan);
            }
            else if (islemTur >= 50)
            {
                XLS.HucreAdBulYaz("Tarih3", DateTime.Today.Date.ToShortDateString());
                XLS.HucreAdBulYaz("Tarih4", DateTime.Today.Date.ToShortDateString());
                XLS.HucreAdBulYaz("AdSoyad3", iBilgi.adSoyad);
                XLS.HucreAdBulYaz("Unvan3", iBilgi.unvan);
            }

            decimal miktarToplam = (decimal)0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            string eskiHesap = string.Empty;
            decimal araToplam = (decimal)0;
            decimal toplam = (decimal)0;
            decimal[] kdvToplam = new decimal[99];

            int baslaSatir = kaynakSatir + 2;
            int sayfadakiSatir = 62;
            int cevapSatir = (tf.detay.objeler.Count - 1) * 2;
            int baslikSatirSayisi = 12;
            int imzaSatirSayisi = 12;
            int sayac = baslikSatirSayisi;

            //aratoplamlarla birlikte ka� sat�r
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                if (td.hesapPlanKod.Length >= 9)
                {
                    if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
                    {
                        cevapSatir += 2;
                        if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
                        {
                            //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
                            for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
                            {
                                if (kdvToplam[i] > 0)
                                    cevapSatir += (int)kdvToplam[i];
                            }
                            kdvToplam = new decimal[99];
                        }
                    }
                    kdvToplam[td.kdvOran]++;
                    eskiHesap = td.hesapPlanKod.Substring(0, 9);
                }
            }
            //Genel toplam i�in 4 ekleniyor. En son grup toplam� ile birlikte
            cevapSatir += 4;

            //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m (Son grup i�in)
            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
                for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
                {
                    if (kdvToplam[i] > 0)
                        cevapSatir += (int)kdvToplam[i];
                }
                kdvToplam = new decimal[99];
            }

            //Ba�l�k sat�r say�s�
            cevapSatir += baslikSatirSayisi;

            //�mza sat�r say�s�
            cevapSatir += imzaSatirSayisi;
            if (cevapSatir > ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
            {
                int fark = cevapSatir - ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir;
                if (fark < imzaSatirSayisi)
                    sayfadakiSatir -= fark;
            }

            int acSatir = 0;
            if (cevapSatir != ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
                acSatir = ((cevapSatir / sayfadakiSatir) + 1) * sayfadakiSatir;
            else
                acSatir = (cevapSatir / sayfadakiSatir) * sayfadakiSatir;

            acSatir -= baslikSatirSayisi + imzaSatirSayisi;

            int dongu = 0;
            if (acSatir % 2 == 0)
            {
                dongu = acSatir + baslaSatir + 1;
                XLS.SatirKopyalaAc(kaynakSatir, acSatir + 1, kaynakSatir + 2);
            }
            else
            {
                dongu = acSatir + baslaSatir + 2;
                XLS.SatirKopyalaAc(kaynakSatir, acSatir + 2, kaynakSatir + 2);
            }

            for (int j = kaynakSatir + 2; j < dongu; j += 2)
            {
                for (int i = sutun; i < sutun + 9; i++)
                {
                    if (i != sutun + 3)
                        XLS.HucreBirlestir(j, i, j + 1, i);
                    else
                    {
                        XLS.HucreBirlestir(j, i, j + 1, i + 1);
                        i++;
                    }
                }

                for (int i = sutun + 9; i < sutun + 12; i++)
                {
                    XLS.HucreBirlestir(j, i, j, i);
                    XLS.HucreBirlestir(j, i, j, i);
                }

                XLS.HucreBirlestir(j, sutun + 12, j + 1, sutun + 12);

                for (int i = sutun + 13; i < sutun + 16; i++)
                {
                    XLS.HucreBirlestir(j, i, j, i);
                    XLS.HucreBirlestir(j + 1, i, j + 1, i);
                }

                for (int i = sutun + 16; i < sutun + 19; i++)
                    XLS.HucreBirlestir(j, i, j + 1, i);
            }

            //A�a��daki d�ng�de gerekli
            eskiHesap = "";
            decimal birimFiyat = 0;
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                if (sayac % sayfadakiSatir == 0 && sayac > 20)
                {
                    //XLS.SayfaSonuKoyHucresel(satir);
                    sayac = 0;
                }

                if (td.hesapPlanKod.Length >= 9)
                {
                    if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
                    {
                        Toplam5AEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY005, eskiHesap), araToplam);
                        sayac += 2;
                        araToplam = (decimal)0;

                        //KDV Toplam
                        if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
                        {
                            for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                            {
                                if (kdvToplam[i] > 0)
                                {
                                    ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY006, i.ToString()), kdvToplam[i]);
                                    toplam += kdvToplam[i];
                                    sayac++;
                                }
                            }
                            kdvToplam = new decimal[99];
                        }
                    }
                }

                if (sayac % sayfadakiSatir == 0 && sayac > 20)
                {
                    //XLS.SayfaSonuKoyHucresel(satir);
                    sayac = 0;
                }

                if (td.hesapPlanKod.Length >= 9)
                    eskiHesap = td.hesapPlanKod.Substring(0, 9);
                satir += 2;
                sayac += 2;

                XLS.HucreDegerYaz(satir, sutun, ++siraNo);
                XLS.HucreDegerYaz(satir, sutun + 1, td.hesapPlanKod);
                string gorSicilNo = servisUZY.UzayDegeriStr(null, "TASSICILGETIR", td.muhasebeKod + "-" + td.harcamaKod + "-" + td.sicilNo.ToString(), true, "");
                XLS.HucreDegerYaz(satir, sutun + 2, gorSicilNo);
                string ad = td.hesapPlanAd;
                if (td.ozellik.adi != "")
                    ad += " - " + td.ozellik.adi;

                XLS.HucreDegerYaz(satir, sutun + 3, ad);
                XLS.HucreDegerYaz(satir, sutun + 5, td.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(td.miktar.ToString(), (double)0));
                miktarToplam += td.miktar;

                birimFiyat = td.birimFiyatKDVLi;

                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble((td.miktar * birimFiyat).ToString("#,###.00"), (double)0));
                araToplam += OrtakFonksiyonlar.ConvertToDecimal((td.miktar * birimFiyat).ToString("#,###.00"), (decimal)0);
                toplam += OrtakFonksiyonlar.ConvertToDecimal((td.miktar * birimFiyat).ToString("#,###.00"), (decimal)0);

                kdvToplam[td.kdvOran] += OrtakFonksiyonlar.ConvertToDecimal(td.miktar) * ((birimFiyat * (td.kdvOran / 100)));

                TNS.TMM.SicilNoOzellik sco = td.ozellik;
                XLS.HucreDegerYaz(satir, sutun + 9, sco.gelisTarihi);
                XLS.HucreDegerYaz(satir, sutun + 10, sco.cagi);

                if (!string.IsNullOrEmpty(sco.yayinTarihi) && !string.IsNullOrEmpty(sco.yayinYeri))
                    XLS.HucreDegerYaz(satir + 1, sutun + 10, sco.yayinTarihi + " - " + sco.yayinYeri);
                else if (!string.IsNullOrEmpty(sco.yayinTarihi))
                    XLS.HucreDegerYaz(satir + 1, sutun + 10, sco.yayinTarihi);
                else if (!string.IsNullOrEmpty(sco.yayinYeri))
                    XLS.HucreDegerYaz(satir + 1, sutun + 10, sco.yayinYeri);

                XLS.HucreDegerYaz(satir, sutun + 11, sco.neredeBulundu);
                if (sco.neredeBulundu.Trim() == "")
                    XLS.HucreDegerYaz(satir, sutun + 11, sco.neredenGeldi);
                XLS.HucreDegerYaz(satir + 1, sutun + 11, sco.yazarAdi);
                XLS.HucreDegerYaz(satir, sutun + 12, sco.boyutlari);
                XLS.HucreDegerYaz(satir, sutun + 13, sco.durumuMaddesi);
                XLS.HucreDegerYaz(satir + 1, sutun + 13, sco.ciltTuru);
                XLS.HucreDegerYaz(satir, sutun + 14, sco.onYuz);
                XLS.HucreDegerYaz(satir + 1, sutun + 14, sco.satirSayisi);
                XLS.HucreDegerYaz(satir, sutun + 15, sco.arkaYuz);

                if (!string.IsNullOrEmpty(sco.yaprakSayisi) && !string.IsNullOrEmpty(sco.sayfaSayisi))
                    XLS.HucreDegerYaz(satir + 1, sutun + 15, sco.yaprakSayisi + " - " + sco.sayfaSayisi);
                else if (!string.IsNullOrEmpty(sco.yaprakSayisi))
                    XLS.HucreDegerYaz(satir + 1, sutun + 15, sco.yaprakSayisi);
                else if (!string.IsNullOrEmpty(sco.sayfaSayisi))
                    XLS.HucreDegerYaz(satir + 1, sutun + 15, sco.sayfaSayisi);

                XLS.HucreDegerYaz(satir, sutun + 16, sco.fotograf);
                XLS.HucreDegerYaz(satir, sutun + 17, sco.dil);
                XLS.HucreDegerYaz(satir, sutun + 18, sco.yeriKonusu);
            }

            XLS.SatirAc(dongu + 1, 1);
            XLS.HucreKopyala(kaynakSatir - 4, sutun, kaynakSatir - 4, sutun + 18, dongu + 1, sutun);

            string aciklama1 = string.Format(Resources.TasinirMal.FRMTIY008, tf.detay.objeler.Count.ToString(), miktarToplam.ToString());
            string aciklama2 = string.Format(Resources.TasinirMal.FRMTIY009, tf.detay.objeler.Count.ToString(), miktarToplam.ToString());
            if (islemTur < 50)
            {
                XLS.HucreDegerYaz(dongu + 6, sutun, aciklama1);
                XLS.HucreDegerYaz(dongu + 6, sutun + 6, aciklama2);
            }
            else if (islemTur >= 50)
            {
                XLS.HucreDegerYaz(dongu + 6, sutun + 11, aciklama1);
                XLS.HucreDegerYaz(dongu + 6, sutun + 15, aciklama2);
            }

            Toplam5AEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY005, eskiHesap), araToplam);

            //KDV Toplam
            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                {
                    if (kdvToplam[i] > 0)
                    {
                        ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY006, i.ToString()), kdvToplam[i]);
                        toplam += kdvToplam[i];
                        sayac++;
                    }
                }
            }

            Toplam5AEkle(XLS, kaynakSatir, ref satir, sutun, Resources.TasinirMal.FRMTIY007, toplam);

            XLS.SatirYukseklikAyarla(0, kaynakSatir + acSatir + 24, GenelIslemler.JexcelBirimtoExcelBirim(260));
            XLS.SatirYukseklikAyarla(baslikSatirSayisi - 2, baslikSatirSayisi - 1, GenelIslemler.JexcelBirimtoExcelBirim(600));
            XLS.SatirGizle(baslikSatirSayisi, baslikSatirSayisi + 1, true);
            XLS.DosyaSaklaTamYol();

            if (gonder)
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, tf.fisNo, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen ta��n�r i�lem fi�i bilgilerini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="tf">Ta��n�r i�lem fi�i bilgilerini tutan nesne</param>
        /// <param name="excelYazYer">Rapor g�nderilirken kullan�lacak dosya ad�</param>
        /// <param name="muhasebeKdvMukellefiMi">Ta��n�r i�lem fi�inin ait oldu�u muhasebe birimi KDV m�kellefi mi, de�il mi bilgisi</param>
        private static void TIFDagitimIadeYaz(TNS.TMM.TasinirIslemForm tf, string excelYazYer, bool muhasebeKdvMukellefiMi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int satir = 0;
            int sutun = 0;
            int siraNo = 0;
            int islemTur = 0;

            string sablonAd = "TIFDAGITIMIADE.XLT";

            string sonucDosyaAd = "";
            bool gonder = false;
            if (excelYazYer == "")
            {
                sonucDosyaAd = System.IO.Path.GetTempFileName();
                gonder = true;
            }
            else
                sonucDosyaAd = excelYazYer;

            XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/TMM/" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("BelgeNo", tf.fisNo);
            XLS.HucreAdBulYaz("Tarih", tf.fisTarih.ToString());

            bool kullaniciBirimi = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(null, "TASAMBAR", tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.ambarKod, true, "KULLANICIBIRIMI"), 0) == 1 ? true : false;
            if (kullaniciBirimi)
            {
                XLS.HucreAdBulYaz("BirimAd", tf.ambarAd);
                XLS.HucreAdBulYaz("MuhasebeAd", tf.gAmbarAd);
            }
            else
            {
                XLS.HucreAdBulYaz("BirimAd", tf.gAmbarAd);
                XLS.HucreAdBulYaz("MuhasebeAd", tf.ambarAd);
            }

            if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
                XLS.HucreAdBulYaz("Dagitim", "X");
            else
                XLS.HucreAdBulYaz("Iade", "X");

            if ((tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS && kullaniciBirimi) || (tf.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADECIKIS && !kullaniciBirimi))
            {
                XLS.HucreAdBulYaz("DagitimAd", Resources.TasinirMal.FRMTIY046);

                string teslimEden = XLS.HucreDegerAl(kaynakSatir + 3, sutun);
                string teslimAlan = XLS.HucreDegerAl(kaynakSatir + 3, sutun + 6);
                XLS.HucreDegerYaz(kaynakSatir + 3, sutun, teslimAlan);
                XLS.HucreDegerYaz(kaynakSatir + 3, sutun + 6, teslimEden);
            }

            decimal miktarToplam = (decimal)0;
            satir = kaynakSatir;
            string eskiHesap = string.Empty;
            decimal araToplam = (decimal)0;
            decimal toplam = (decimal)0;
            decimal[] kdvToplam = new decimal[99];//Bir grupta ka� tane farkl� KDV olabilir veya Grupdaki KDV Grup toplamlar� i�in kullan�l�yor

            int baslaSatir = kaynakSatir + 1;
            int sayfadakiSatir = 73; //68 // 74;
            int cevapSatir = tf.detay.objeler.Count - 1;
            int baslikSatirSayisi = 16;
            int imzaSatirSayisi = 15;
            int sayac = baslikSatirSayisi;

            //aratoplamlarla birlikte ka� sat�r
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                if (td.hesapPlanKod.Length >= 9)
                {
                    if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
                    {
                        cevapSatir++;
                        if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
                        {
                            //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
                            for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
                            {
                                if (kdvToplam[i] > 0)
                                    cevapSatir += (int)kdvToplam[i];
                            }
                            kdvToplam = new decimal[99];
                        }
                    }
                    kdvToplam[td.kdvOran]++;
                    eskiHesap = td.hesapPlanKod.Substring(0, 9);
                }
            }

            //Genel toplam i�in 2 ekleniyor. En son grup toplam� ile birlikte
            cevapSatir += 2;

            //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m (Son grup i�in)
            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                //KDV grup toplamlar�ndan dolay� ka� sat�r a�mak laz�m
                for (int i = 1; i < kdvToplam.GetUpperBound(0); i++)
                {
                    if (kdvToplam[i] > 0)
                        cevapSatir += (int)kdvToplam[i];
                }
                kdvToplam = new decimal[99];
            }

            //Ba�l�k sat�r say�s�
            cevapSatir += baslikSatirSayisi;

            //�mza sat�r say�s�
            cevapSatir += imzaSatirSayisi;
            if (cevapSatir > ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
            {
                int fark = cevapSatir - ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir;
                if (fark < imzaSatirSayisi)
                    sayfadakiSatir -= fark;
            }

            int acSatir = 0;
            if (cevapSatir != ((cevapSatir / sayfadakiSatir)) * sayfadakiSatir)
                acSatir = ((cevapSatir / sayfadakiSatir) + 1) * sayfadakiSatir;
            else
                acSatir = (cevapSatir / sayfadakiSatir) * sayfadakiSatir;

            acSatir -= baslikSatirSayisi + imzaSatirSayisi;
            XLS.SatirKopyalaAc(kaynakSatir, acSatir - 1, kaynakSatir + 1);

            //A�a��daki d�ng�de gerekli
            eskiHesap = "";
            decimal birimFiyat = 0;
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                if (sayac % sayfadakiSatir == 0 && sayac > 10)
                {
                    //XLS.SayfaSonuKoyHucresel(satir);
                    sayac = 0;
                }

                if (td.hesapPlanKod.Length >= 9)
                {
                    if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != td.hesapPlanKod.Substring(0, 9))
                    {
                        toplam += araToplam;
                        sayac++;
                        araToplam = (decimal)0;

                        //KDV Toplam
                        if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
                        {
                            for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                            {
                                if (kdvToplam[i] > 0)
                                {
                                    toplam += kdvToplam[i];
                                    sayac++;
                                }
                            }
                            kdvToplam = new decimal[99];
                        }
                    }
                }

                if (sayac % sayfadakiSatir == 0 && sayac > 10)
                {
                    //XLS.SayfaSonuKoyHucresel(satir);
                    sayac = 0;
                }

                if (td.hesapPlanKod.Length >= 9)
                    eskiHesap = td.hesapPlanKod.Substring(0, 9);

                satir++;
                sayac++;
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2); //Stok Numaras�
                XLS.HucreBirlestir(satir, sutun + 6, satir, sutun + 7); //Al�nan/Verilen Miktar
                XLS.HucreBirlestir(satir, sutun + 8, satir, sutun + 12); //A��klamalar

                XLS.HucreDegerYaz(satir, sutun, ++siraNo);
                XLS.HucreDegerYaz(satir, sutun + 1, td.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 3, td.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, td.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 6, td.miktar);
                //XLS.HucreDegerYaz(satir, sutun + 8, td.eAciklama);
            }

            //Ara Toplam
            toplam += araToplam;

            //KDV Toplamlar�
            if (muhasebeKdvMukellefiMi && tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                for (int i = 0; i < kdvToplam.GetUpperBound(0); i++)
                {
                    if (kdvToplam[i] > 0)
                    {
                        ToplamEkle(XLS, kaynakSatir, ref satir, sutun, string.Format(Resources.TasinirMal.FRMTIY006, i.ToString()), kdvToplam[i]);
                        toplam += kdvToplam[i];
                    }
                }
            }

            int geciciSatir = kaynakSatir + acSatir;
            islemTur = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(null, "TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "").ToString(), 0);
            ImzaEkleDagitimIade(XLS, geciciSatir, sutun, tf.muhasebeKod.Trim(), tf.harcamaKod.Trim(), tf.ambarKod.Trim(), tf.detay.objeler.Count, miktarToplam, islemTur, toplam);

            XLS.DosyaSaklaTamYol();

            if (gonder)
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, tf.fisNo, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Ta��n�r i�lem fi�i excel raporuna toplam bilgileri ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun ba�lad��� kaynak sat�r numaras�</param>
        /// <param name="satir">Toplam bilgilerinin yaz�laca�� sat�r numaras�</param>
        /// <param name="sutun">Toplam bilgilerinin yaz�lmaya ba�lanaca�� s�tun numaras�</param>
        /// <param name="toplamAciklama">Toplama ait a��klama bilgisi</param>
        /// <param name="tutar">Toplam tutar�</param>
        /// <param name="tutarKur">The tutar kur.</param>
        private static void ToplamEkle(Tablo XLS, int kaynakSatir, ref int satir, int sutun, string toplamAciklama, decimal tutar, decimal[] tutarKur = null)
        {
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 15, satir, 0);

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 7);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun, toplamAciklama);
            XLS.KoyuYap(satir, sutun + 8, true);
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));

            if (tutarKur != null)
            {
                for (int i = 0; i < tutarKur.Length; i++)
                {
                    XLS.KoyuYap(satir, sutun + 9 + i, true);
                    XLS.HucreDegerYaz(satir, sutun + 9 + i, OrtakFonksiyonlar.ConvertToDouble(tutarKur[i].ToString(), (double)0));
                }
            }
        }

        /// <summary>
        /// K�t�phane/M�ze ta��n�r i�lem fi�i excel raporuna toplam bilgileri ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun ba�lad��� kaynak sat�r numaras�</param>
        /// <param name="satir">Toplam bilgilerinin yaz�laca�� sat�r numaras�</param>
        /// <param name="sutun">Toplam bilgilerinin yaz�lmaya ba�lanaca�� s�tun numaras�</param>
        /// <param name="toplamAciklama">Toplama ait a��klama bilgisi</param>
        /// <param name="tutar">Toplam tutar�</param>
        private static void Toplam5AEkle(Tablo XLS, int kaynakSatir, ref int satir, int sutun, string toplamAciklama, decimal tutar)
        {
            satir += 2;
            //XLS.SatirAc(satir, 2);
            //XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir + 1, sutun + 8, satir, sutun);
            XLS.HucreBirlestirme(satir, sutun, satir, sutun + 7);
            XLS.HucreBirlestir(satir, sutun, satir + 1, sutun + 7);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun, toplamAciklama);
            //XLS.HucreBirlestir(satir, sutun + 8, satir + 1, sutun + 8);
            XLS.KoyuYap(satir, sutun + 8, true);
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));
        }

        private static void ImzaEkleDagitimIade(Tablo XLS, int satir, int sutun, string muhasebe, string harcama, string ambar, int kalem, decimal miktar, int islemTur, decimal genelToplam)
        {
            Kullanici kul = OturumBilgisiIslem.KullaniciBilgiOku(true);
            ObjectArray imza = servisTMM.ImzaListele(kul, muhasebe, harcama, ambar, 0);

            ImzaBilgisi[] iBilgi = new ImzaBilgisi[3];
            for (int i = 0; i < iBilgi.Length; i++)
                iBilgi[i] = new ImzaBilgisi();

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi i in imza.objeler)
                {
                    if (i.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI)
                        iBilgi[0] = i;
                    else if (i.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI)
                        iBilgi[1] = i;
                    else if (i.imzaYer == (int)ENUMImzaYer.USTYONETICI)
                        iBilgi[2] = i;
                }
            }

            string girisKisiAd = "";
            string girisKisiUnvan = "";
            string girisTarih = "";
            string cikisKisiAd = "";
            string cikisKisiUnvan = "";
            string cikisTarih = "";

            string ekImzaGoster = System.Configuration.ConfigurationManager.AppSettings["TIFEkImza"];

            if (ekImzaGoster == "1")
            {
                if (islemTur < (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    girisKisiAd = iBilgi[0].adSoyad;
                    girisKisiUnvan = iBilgi[0].unvan;
                    girisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    girisKisiAd = Resources.TasinirMal.FRMTIY010;
                    girisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                //XLS.HucreDegerYaz(satir + 1, sutun, string.Format(Resources.TasinirMal.FRMTIY011, girisTarih));
                XLS.HucreDegerYaz(satir + 4, sutun + 2, girisKisiAd);
                XLS.HucreDegerYaz(satir + 5, sutun + 2, girisKisiUnvan);

                if (islemTur >= (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    cikisKisiAd = iBilgi[0].adSoyad;
                    cikisKisiUnvan = iBilgi[0].unvan;
                    cikisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    cikisKisiAd = Resources.TasinirMal.FRMTIY010;
                    cikisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                //XLS.HucreDegerYaz(satir + 1, sutun + 6, string.Format(Resources.TasinirMal.FRMTIY021, cikisTarih));
                XLS.HucreDegerYaz(satir + 4, sutun + 7, cikisKisiAd);
                XLS.HucreDegerYaz(satir + 5, sutun + 7, cikisKisiUnvan);

                if (islemTur == (int)ENUMIslemTipi.DEVIRCIKIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
                {
                    XLS.HucreDegerYaz(satir + 3, sutun + 3, Resources.TasinirMal.FRMTIY024);
                    if (!string.IsNullOrEmpty(iBilgi[1].adSoyad))
                        XLS.HucreDegerYaz(satir + 4, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY013, iBilgi[1].adSoyad));
                    else
                        XLS.HucreDegerYaz(satir + 4, sutun + 3, Resources.TasinirMal.FRMTIY018);

                    if (!string.IsNullOrEmpty(iBilgi[1].unvan))
                        XLS.HucreDegerYaz(satir + 5, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY014, iBilgi[1].unvan));
                    else
                        XLS.HucreDegerYaz(satir + 5, sutun + 3, Resources.TasinirMal.FRMTIY019);

                    XLS.HucreDegerYaz(satir + 6, sutun + 3, Resources.TasinirMal.FRMTIY020);

                    if (genelToplam >= 13500)
                    {
                        XLS.HucreDegerYaz(satir + 10, sutun + 3, Resources.TasinirMal.FRMTIY025);
                        if (!string.IsNullOrEmpty(iBilgi[2].adSoyad))
                            XLS.HucreDegerYaz(satir + 11, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY013, iBilgi[2].adSoyad));
                        else
                            XLS.HucreDegerYaz(satir + 11, sutun + 3, Resources.TasinirMal.FRMTIY018);

                        if (!string.IsNullOrEmpty(iBilgi[2].unvan))
                            XLS.HucreDegerYaz(satir + 12, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY014, iBilgi[2].unvan));
                        else
                            XLS.HucreDegerYaz(satir + 12, sutun + 3, Resources.TasinirMal.FRMTIY019);

                        XLS.HucreDegerYaz(satir + 13, sutun + 3, Resources.TasinirMal.FRMTIY020);
                    }
                }
            }
            else if (ekImzaGoster != "1")
            {
                if (islemTur < (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    girisKisiAd = iBilgi[0].adSoyad;
                    girisKisiUnvan = iBilgi[0].unvan;
                    girisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    girisKisiAd = Resources.TasinirMal.FRMTIY010;
                    girisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                XLS.HucreDegerYaz(satir + 8, sutun + 4, girisTarih);
                XLS.HucreDegerYaz(satir + 4, sutun + 2, girisKisiAd);
                XLS.HucreDegerYaz(satir + 5, sutun + 2, girisKisiUnvan);

                if (islemTur >= (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    cikisKisiAd = iBilgi[0].adSoyad;
                    cikisKisiUnvan = iBilgi[0].unvan;
                    cikisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    cikisKisiAd = Resources.TasinirMal.FRMTIY010;
                    cikisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                XLS.HucreDegerYaz(satir + 8, sutun + 4, cikisTarih);
                XLS.HucreDegerYaz(satir + 4, sutun + 7, cikisKisiAd);
                XLS.HucreDegerYaz(satir + 5, sutun + 7, cikisKisiUnvan);
            }
        }

        /// <summary>
        /// Ta��n�r i�lem fi�i excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
        /// <param name="satir">�mza bilgilerinin yaz�lmaya ba�lanaca�� sat�r numaras�</param>
        /// <param name="sutun">�mza bilgilerinin yaz�lmaya ba�lanaca�� s�tun numaras�</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="kalem">Ta��n�r i�lem fi�indeki sat�r baz�nda malzeme say�s�</param>
        /// <param name="miktar">Ta��n�r i�lem fi�indeki toplam malzeme miktar�</param>
        /// <param name="islemTur">Ta��n�r i�lem fi�inin i�lem t�r�</param>
        /// <param name="genelToplam">Ta��n�r i�lem fi�inin genel toplam tutar�</param>
        private static void ImzaEkle(Tablo XLS, int satir, int sutun, string muhasebe, string harcama, string ambar, int kalem, decimal miktar, int islemTur, decimal genelToplam)
        {
            string kurum = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");
            if (kurum == "5030")
            {
                XLS.HFDegerAta(1, 2, "");
            }

            satir += 2;

            Kullanici kul = OturumBilgisiIslem.KullaniciBilgiOku(true);
            ObjectArray imza = servisTMM.ImzaListele(kul, muhasebe, harcama, ambar, 0);

            ImzaBilgisi[] iBilgi = new ImzaBilgisi[3];
            for (int i = 0; i < iBilgi.Length; i++)
                iBilgi[i] = new ImzaBilgisi();

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi i in imza.objeler)
                {
                    if (i.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI)
                        iBilgi[0] = i;
                    else if (i.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI)
                        iBilgi[1] = i;
                    else if (i.imzaYer == (int)ENUMImzaYer.USTYONETICI)
                        iBilgi[2] = i;
                }
            }

            //XLS.SatirAc(satir, 15);
            //XLS.HucreKopyala(0, sutun, 14, sutun + 8, satir, sutun);

            string aciklama = string.Format(Resources.TasinirMal.FRMTIY008, kalem.ToString(), miktar);
            string girisKisiAd = "";
            string girisKisiUnvan = "";
            string girisTarih = "";
            string cikisKisiAd = "";
            string cikisKisiUnvan = "";
            string cikisTarih = "";

            string ekImzaGoster = System.Configuration.ConfigurationManager.AppSettings["TIFEkImza"];

            if (ekImzaGoster == "1")
            {
                if (islemTur < (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    XLS.HucreDegerYaz(satir, sutun, aciklama);
                    girisKisiAd = iBilgi[0].adSoyad;
                    girisKisiUnvan = iBilgi[0].unvan;
                    girisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    girisKisiAd = Resources.TasinirMal.FRMTIY010;
                    girisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                XLS.HucreDegerYaz(satir + 1, sutun, string.Format(Resources.TasinirMal.FRMTIY011, girisTarih));
                XLS.HucreDegerYaz(satir + 3, sutun, Resources.TasinirMal.FRMTIY012);
                XLS.HucreDegerYaz(satir + 4, sutun, string.Format(Resources.TasinirMal.FRMTIY013, girisKisiAd));
                XLS.HucreDegerYaz(satir + 5, sutun, string.Format(Resources.TasinirMal.FRMTIY014, girisKisiUnvan));
                XLS.HucreDegerYaz(satir + 6, sutun, Resources.TasinirMal.FRMTIY015);

                XLS.HucreDegerYaz(satir + 9, sutun, string.Format(Resources.TasinirMal.FRMTIY016, girisTarih));
                XLS.HucreDegerYaz(satir + 10, sutun, Resources.TasinirMal.FRMTIY017);
                XLS.HucreDegerYaz(satir + 11, sutun, Resources.TasinirMal.FRMTIY018);
                XLS.HucreDegerYaz(satir + 12, sutun, Resources.TasinirMal.FRMTIY019);
                XLS.HucreDegerYaz(satir + 13, sutun, Resources.TasinirMal.FRMTIY020);

                if (islemTur >= (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    XLS.HucreDegerYaz(satir, sutun + 6, aciklama);
                    cikisKisiAd = iBilgi[0].adSoyad;
                    cikisKisiUnvan = iBilgi[0].unvan;
                    cikisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    cikisKisiAd = Resources.TasinirMal.FRMTIY010;
                    cikisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                XLS.HucreDegerYaz(satir + 1, sutun + 6, string.Format(Resources.TasinirMal.FRMTIY021, cikisTarih));
                XLS.HucreDegerYaz(satir + 3, sutun + 6, Resources.TasinirMal.FRMTIY012);
                XLS.HucreDegerYaz(satir + 4, sutun + 6, string.Format(Resources.TasinirMal.FRMTIY013, cikisKisiAd));
                XLS.HucreDegerYaz(satir + 5, sutun + 6, string.Format(Resources.TasinirMal.FRMTIY014, cikisKisiUnvan));
                XLS.HucreDegerYaz(satir + 6, sutun + 6, Resources.TasinirMal.FRMTIY015);

                XLS.HucreDegerYaz(satir + 9, sutun + 6, string.Format(Resources.TasinirMal.FRMTIY022, cikisTarih));
                XLS.HucreDegerYaz(satir + 10, sutun + 6, Resources.TasinirMal.FRMTIY023);
                XLS.HucreDegerYaz(satir + 11, sutun + 6, Resources.TasinirMal.FRMTIY018);
                XLS.HucreDegerYaz(satir + 12, sutun + 6, Resources.TasinirMal.FRMTIY019);
                XLS.HucreDegerYaz(satir + 13, sutun + 6, Resources.TasinirMal.FRMTIY020);

                if (islemTur == (int)ENUMIslemTipi.DEVIRCIKIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
                {
                    XLS.HucreDegerYaz(satir + 3, sutun + 3, Resources.TasinirMal.FRMTIY024);
                    if (!string.IsNullOrEmpty(iBilgi[1].adSoyad))
                        XLS.HucreDegerYaz(satir + 4, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY013, iBilgi[1].adSoyad));
                    else
                        XLS.HucreDegerYaz(satir + 4, sutun + 3, Resources.TasinirMal.FRMTIY018);

                    if (!string.IsNullOrEmpty(iBilgi[1].unvan))
                        XLS.HucreDegerYaz(satir + 5, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY014, iBilgi[1].unvan));
                    else
                        XLS.HucreDegerYaz(satir + 5, sutun + 3, Resources.TasinirMal.FRMTIY019);

                    XLS.HucreDegerYaz(satir + 6, sutun + 3, Resources.TasinirMal.FRMTIY020);

                    if (genelToplam >= 13500)
                    {
                        XLS.HucreDegerYaz(satir + 10, sutun + 3, Resources.TasinirMal.FRMTIY025);
                        if (!string.IsNullOrEmpty(iBilgi[2].adSoyad))
                            XLS.HucreDegerYaz(satir + 11, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY013, iBilgi[2].adSoyad));
                        else
                            XLS.HucreDegerYaz(satir + 11, sutun + 3, Resources.TasinirMal.FRMTIY018);

                        if (!string.IsNullOrEmpty(iBilgi[2].unvan))
                            XLS.HucreDegerYaz(satir + 12, sutun + 3, string.Format(Resources.TasinirMal.FRMTIY014, iBilgi[2].unvan));
                        else
                            XLS.HucreDegerYaz(satir + 12, sutun + 3, Resources.TasinirMal.FRMTIY019);

                        XLS.HucreDegerYaz(satir + 13, sutun + 3, Resources.TasinirMal.FRMTIY020);
                    }
                }

                ImzaFormatlaEkImza(XLS, satir, sutun);
            }
            else if (ekImzaGoster != "1")
            {
                if (islemTur < (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    XLS.HucreDegerYaz(satir, sutun, aciklama);
                    girisKisiAd = iBilgi[0].adSoyad;
                    girisKisiUnvan = iBilgi[0].unvan;
                    girisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    girisKisiAd = Resources.TasinirMal.FRMTIY010;
                    girisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                XLS.HucreDegerYaz(satir + 1, sutun, string.Format(Resources.TasinirMal.FRMTIY026, girisTarih));
                XLS.HucreDegerYaz(satir + 3, sutun, Resources.TasinirMal.FRMTIY012);
                XLS.HucreDegerYaz(satir + 4, sutun, string.Format(Resources.TasinirMal.FRMTIY013, girisKisiAd));
                XLS.HucreDegerYaz(satir + 5, sutun, string.Format(Resources.TasinirMal.FRMTIY014, girisKisiUnvan));
                XLS.HucreDegerYaz(satir + 6, sutun, Resources.TasinirMal.FRMTIY015);

                XLS.HucreDegerYaz(satir + 9, sutun, string.Format(Resources.TasinirMal.FRMTIY016, girisTarih));
                XLS.HucreDegerYaz(satir + 10, sutun, Resources.TasinirMal.FRMTIY017);
                XLS.HucreDegerYaz(satir + 11, sutun, Resources.TasinirMal.FRMTIY018);
                XLS.HucreDegerYaz(satir + 12, sutun, Resources.TasinirMal.FRMTIY019);
                XLS.HucreDegerYaz(satir + 13, sutun, Resources.TasinirMal.FRMTIY020);

                if (islemTur >= (int)ENUMIslemTipi.TUKETIMCIKIS)
                {
                    XLS.HucreDegerYaz(satir, sutun + 6, aciklama);
                    cikisKisiAd = iBilgi[0].adSoyad;
                    cikisKisiUnvan = iBilgi[0].unvan;
                    cikisTarih = DateTime.Today.Date.ToShortDateString();
                }
                else
                {
                    cikisKisiAd = Resources.TasinirMal.FRMTIY010;
                    cikisKisiUnvan = Resources.TasinirMal.FRMTIY010;
                }

                XLS.HucreDegerYaz(satir + 1, sutun + 5, string.Format(Resources.TasinirMal.FRMTIY027, cikisTarih));
                XLS.HucreDegerYaz(satir + 3, sutun + 5, Resources.TasinirMal.FRMTIY012);
                XLS.HucreDegerYaz(satir + 4, sutun + 5, string.Format(Resources.TasinirMal.FRMTIY013, cikisKisiAd));
                XLS.HucreDegerYaz(satir + 5, sutun + 5, string.Format(Resources.TasinirMal.FRMTIY014, cikisKisiUnvan));
                XLS.HucreDegerYaz(satir + 6, sutun + 5, Resources.TasinirMal.FRMTIY015);

                XLS.HucreDegerYaz(satir + 9, sutun + 5, string.Format(Resources.TasinirMal.FRMTIY022, cikisTarih));
                XLS.HucreDegerYaz(satir + 10, sutun + 5, Resources.TasinirMal.FRMTIY023);
                XLS.HucreDegerYaz(satir + 11, sutun + 5, Resources.TasinirMal.FRMTIY018);
                XLS.HucreDegerYaz(satir + 12, sutun + 5, Resources.TasinirMal.FRMTIY019);
                XLS.HucreDegerYaz(satir + 13, sutun + 5, Resources.TasinirMal.FRMTIY020);

                ImzaFormatla(XLS, satir, sutun);
            }
        }

        /// <summary>
        /// Ta��n�r i�lem fi�i excel raporunun imza alanlar�n� formatlayan yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
        /// <param name="satir">�mza bilgilerinin yaz�lmaya ba�land��� sat�r numaras�</param>
        /// <param name="sutun">�mza bilgilerinin yaz�lmaya ba�land��� s�tun numaras�</param>
        private static void ImzaFormatla(Tablo XLS, int satir, int sutun)
        {
            XLS.YaziTipiAta(satir, sutun, satir + 14, sutun + 8, "TAHOMA");
            XLS.YaziTipBuyuklugu(satir, sutun, satir + 14, sutun + 8, 8);

            for (int i = satir; i < satir + 14; i++)
            {
                for (int j = sutun; j <= sutun + 5; j = j + 5)
                {
                    if (i != satir + 7 && i != satir + 8 && j != sutun + 3)
                        XLS.HucreBirlestir(i, j, i, j + 3);

                    if (i == satir || i == satir + 1 || i == satir + 2 || i == satir + 3 || i == satir + 9 || i == satir + 10)
                    {
                        if (i != satir)
                            XLS.KoyuYap(i, j, true);

                        XLS.DuseyHizala(i, j, 2);
                    }
                    else
                        XLS.DuseyHizala(i, j, 0);

                    XLS.YatayHizala(i, j, 2);
                }
            }

            XLS.YatayCizgiCiz(satir, sutun, sutun + 8, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 8, sutun, sutun + 8, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 15, sutun, sutun + 8, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 14, sutun, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 14, sutun + 9, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 14, sutun + 4, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, true);
        }

        /// <summary>
        /// Ta��n�r i�lem fi�i excel raporunun imza alanlar�n� formatlayan yordam
        /// Bu yordam, ImzaFormatla yordam�ndan farkl� olarak ek imza alanlar�n� da formatlar.
        /// </summary>
        /// <param name="XLS">Excel ile ilgili i�lemleri yapan nesne</param>
        /// <param name="satir">�mza bilgilerinin yaz�lmaya ba�land��� sat�r numaras�</param>
        /// <param name="sutun">�mza bilgilerinin yaz�lmaya ba�land��� s�tun numaras�</param>
        private static void ImzaFormatlaEkImza(Tablo XLS, int satir, int sutun)
        {
            XLS.YaziTipiAta(satir, sutun, satir + 14, sutun + 8, "TAHOMA");
            XLS.YaziTipBuyuklugu(satir, sutun, satir + 14, sutun + 8, 8);

            for (int i = satir; i < satir + 14; i++)
            {
                for (int j = sutun; j <= sutun + 6; j = j + 3)
                {
                    if (i != satir + 2 && i != satir + 7 && i != satir + 8)
                        XLS.HucreBirlestir(i, j, i, j + 2);

                    if (i == satir || i == satir + 1 || i == satir + 3 || i == satir + 9 || i == satir + 10)
                    {
                        if (i != satir)
                            XLS.KoyuYap(i, j, true);

                        XLS.DuseyHizala(i, j, 2);
                    }
                    else
                        XLS.DuseyHizala(i, j, 0);

                    XLS.YatayHizala(i, j, 2);
                }
            }

            XLS.YatayCizgiCiz(satir, sutun, sutun + 8, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 8, sutun, sutun + 8, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 15, sutun, sutun + 8, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 14, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 14, sutun + 9, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
        }

        /// <summary>
        /// Parametre olarak verilen ta��n�r i�lem fi�i bilgilerini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="tf">Ta��n�r i�lem fi�i bilgilerini tutan nesne</param>
        /// <param name="excelYazYer">Rapor g�nderilirken kullan�lacak dosya ad�</param>
        /// <param name="muhasebeKdvMukellefiMi">Ta��n�r i�lem fi�inin ait oldu�u muhasebe birimi KDV m�kellefi mi, de�il mi bilgisi</param>
        private static void TIFYazMerkezBankasi(Kullanici kullanan, int yil, string muhasebeKod, string harcamaKod, string fisNo, string excelYazYer, int islemTipi)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm()
            {
                yil = yil,
                muhasebeKod = muhasebeKod,
                harcamaKod = harcamaKod,
                fisNo = fisNo
            };

            int islemTur = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(null, "TASISLEMTIPTUR", islemTipi.ToString(), true, "").ToString(), 0);
            if (islemTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                tf.devirGirisiMi = true;

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, false);

            if (!bilgi.sonuc.islemSonuc)
                return;

            tf = (TNS.TMM.TasinirIslemForm)bilgi[0];


            Tablo XLS = GenelIslemler.NewTablo();

            string sablonAd = "TIFBAOnay.XLT";

            string sonucDosyaAd = "";
            bool gonder = false;
            if (excelYazYer == "")
            {
                sonucDosyaAd = System.IO.Path.GetTempFileName();
                gonder = true;
            }
            else
                sonucDosyaAd = excelYazYer;


            //Tarih�e bilgisi ta��n�r i�lem fi�i, de�er atr��� ve amortisman i�lemlerinin B/A onay bilgilerini okumak i�in eklenmi�tir.
            MuhasebeIslemiKriter formBA = new MuhasebeIslemiKriter
            {
                yil = tf.yil,
                muhasebeKod = tf.muhasebeKod,
                harcamaKod = tf.harcamaKod,
                fisNo = tf.fisNo
            };

            ObjectArray listeTarihce = servisTMM.BAOnayiTarihceListele(formBA);
            string islemKontrol = "";
            foreach (TarihceBilgisi t in listeTarihce.objeler)
            {
                if (t.islem == "De�i�tirildi")
                    continue;

                if (t.islem == "B Onay�na G�nderildi")
                {
                    formBA.girisZaman = t.islemTarih;
                    formBA.girisSicil = t.islemYapan + " - " + t.islemYapanAd;
                    islemKontrol = "B Onay�na G�nderildi";
                }
                else if (t.islem == "A Onay�na G�nderildi" && islemKontrol == "B Onay�na G�nderildi")
                {
                    formBA.bOnayZaman = t.islemTarih;
                    formBA.bOnaySicil = t.islemYapan + " - " + t.islemYapanAd;
                    islemKontrol = "A Onay�na G�nderildi";
                }
                else if (t.islem == "Onayland�" && (islemKontrol == "A Onay�na G�nderildi" || islemKontrol == "Onayland�"))
                {
                    formBA.aOnayZaman = t.islemTarih;
                    formBA.aOnaySicil = t.islemYapan + " - " + t.islemYapanAd;
                    islemKontrol = "Onayland�";
                }
                else
                {
                    formBA.bOnayZaman = new TNSDateTime();
                    formBA.bOnaySicil = "";
                    formBA.aOnayZaman = new TNSDateTime();
                    formBA.aOnaySicil = "";
                }
            }
            //***************************************************

            if (tf.islemTipKod == (int)ENUMIslemTipi.GECICIIHRAC)
            {
                tf.islemTipAd = "�hra� (Ge�ici)";
                tf.islemTipTur = (int)ENUMIslemTipi.DEVIRCIKISKURUM;
            }
            else
                tf.islemTipAd = servisUZY.UzayDegeriStr(null, "TASISLEMTIPAD", tf.islemTipKod.ToString(), true, "");


            XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/TMM/" + sablonAd, sonucDosyaAd);

            if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM)
            {
                XLS.SutunGizle(9, 9, true);
                XLS.SutunGizle(3, 5, true);
            }
            else if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS)
            {
                XLS.SutunGizle(9, 9, true);
                XLS.SutunGizle(11, 11, true);
            }
            else if (tf.islemTipKod != 10)
                XLS.SutunGizle(12, 12, true);

            string baslik = "DEM�RBA�";
            if (tf.ambarKod == "50")
                baslik = "GAYR�MENKUL";
            else if (tf.ambarKod == "51")
                baslik = "YAZILIM";

            if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                baslik += " DEV�R G�R�� F���";
            else if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS)
                baslik += " DEV�R �IKI� F���";
            else
                baslik += " ��LEM F���";


            XLS.HucreAdBulYaz("Baslik", baslik);
            XLS.HucreAdBulYaz("OnayTarih", formBA.aOnayZaman.ToString());
            XLS.HucreAdBulYaz("FisTarih", tf.fisTarih.ToString());
            XLS.HucreAdBulYaz("FisNo", tf.fisNo);
            XLS.HucreAdBulYaz("HarcamaAd", tf.harcamaAd);
            XLS.HucreAdBulYaz("MuhasebeAd", tf.muhasebeAd);
            XLS.HucreAdBulYaz("IslemAd", tf.islemTipAd);
            XLS.HucreAdBulYaz("AmbarAd", tf.ambarAd);
            XLS.HucreAdBulYaz("FisAciklamasi", tf.aciklama);

            XLS.HucreAdBulYaz("FisiYonlendiren", formBA.girisSicil);
            XLS.HucreAdBulYaz("BOnayi", formBA.bOnaySicil);
            XLS.HucreAdBulYaz("AOnayi", formBA.aOnaySicil);


            if (tf.islemTipTur != (int)ENUMIslemTipi.SATISCIKIS)
                XLS.SutunGizle(11, 11, true);


            decimal toplamSatisFiyat = 0;
            double toplamFiyat = 0;
            double toplamAmortisman = 0;
            int toplamMiktar = 0;

            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int satir = kaynakSatir;
            int siraNo = 0;


            //Sicil No Listele
            SicilNoHareket snh = new SicilNoHareket
            {
                yil = tf.yil,
                muhasebeKod = tf.muhasebeKod,
                harcamaBirimKod = tf.harcamaKod,
                fisNo = tf.fisNo,
                siraNo = -100
            };


            ObjectArray bilgiSicilNo = servisTMM.ButunSicilNoListele(kullanan, snh);

            //*********************************

            if (bilgiSicilNo.sonuc.islemSonuc && bilgiSicilNo.objeler.Count > 0
                && tf.islemTipTur != (int)ENUMIslemTipi.SATISCIKIS && tf.islemTipTur != (int)ENUMIslemTipi.DEVIRCIKISKURUM
                && tf.islemTipTur != (int)ENUMIslemTipi.DEVIRGIRIS && tf.islemTipTur != (int)ENUMIslemTipi.DEVIRCIKIS)
            {
                foreach (SicilNoHareket s in bilgiSicilNo.objeler)
                {
                    string bulunduguYerAd = s.ozellik.bulunduguYerAd;

                    if (string.IsNullOrWhiteSpace(s.ozellik.bulunduguYer))
                    {
                        foreach (TasinirIslemDetay td in tf.detay.objeler)
                        {
                            if (s.fisSiraNo == td.siraNo)
                            {
                                bulunduguYerAd = td.yerleskeYeriAd;
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(bulunduguYerAd))
                        bulunduguYerAd = tf.ambarAd;

                    satir++;
                    siraNo++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, s.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 3, bulunduguYerAd);
                    XLS.HucreDegerYaz(satir, sutun + 6, s.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 8, 0); //birim fiyat
                    XLS.HucreDegerYaz(satir, sutun + 9, 0); //miktar
                    XLS.HucreDegerYaz(satir, sutun + 10, (double)s.kdvliBirimFiyat);

                    toplamFiyat += (double)s.kdvliBirimFiyat;
                }

                XLS.SutunGizle(sutun + 8, sutun + 9, true);
                XLS.SutunGenislikAyarla(8, 8, 300);

                XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 10, "BEDEL�");
            }
            else
            {
                TNS.TMM.TasinirIslemForm tfDevirGiris = new TNS.TMM.TasinirIslemForm()
                {
                    yil = tf.gYil,
                    muhasebeKod = tf.gMuhasebeKod,
                    harcamaKod = tf.gHarcamaKod,
                    fisNo = tf.gFisNo,
                    devirGirisiMi = true

                };

                if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                {
                    ObjectArray bilgiGiris = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tfDevirGiris, false);

                    if (bilgiGiris.sonuc.islemSonuc)
                        tfDevirGiris = (TNS.TMM.TasinirIslemForm)bilgiGiris[0];
                }

                string yer = "";
                foreach (TasinirIslemDetay td in tf.detay.objeler)
                {
                    if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS)
                        yer = tf.gHarcamaAd;
                    else
                        yer = (td.yerleskeYeriAd != "" ? td.yerleskeYeriAd : tf.ambarAd);

                    satir++;
                    siraNo++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                    {
                        string girisSicilNo = "";
                        foreach (TasinirIslemDetay tdGiris in tfDevirGiris.detay.objeler)
                        {
                            if (td.eSicilNo == tdGiris.sicilNo + "")
                                girisSicilNo = tdGiris.gorSicilNo;
                        }

                        XLS.HucreDegerYaz(satir, sutun + 1, girisSicilNo);
                    }
                    else
                        XLS.HucreDegerYaz(satir, sutun + 1, td.gorSicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 3, yer);
                    XLS.HucreDegerYaz(satir, sutun + 6, td.hesapPlanAd);


                    if (tf.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS)
                    {
                        int sicilNoAmortisman = td.sicilNo;
                        string harcamaKodAmortisman = td.harcamaKod;

                        if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                        {
                            sicilNoAmortisman = OrtakFonksiyonlar.ConvertToInt(td.eSicilNo, 0); //Amortisman i�in
                            harcamaKodAmortisman = "";
                        }
                        if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS)
                            harcamaKodAmortisman = td.gonHarcamaKod;


                        //H�zl� ��z�m. Tekrar bak�lacak. H�seyin 
                        //int amortismanDonem = servisTMM.AmortismanDonemiBul(harcamaKodAmortisman, new TNSDateTime(formBA.aOnayZaman.Oku().AddMonths(-3)));
                        AmortismanBilgi ab = TasinirGenel.AmortismanBilgileri(servisTMM, kullanan, sicilNoAmortisman, td.muhasebeKod, harcamaKodAmortisman);

                        XLS.HucreDegerYaz(satir, sutun + 10, ab.sonBedel);

                        if (tf.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS)
                        {
                            XLS.HucreDegerYaz(satir, sutun + 11, (double)(td.satisBedeli));
                            toplamSatisFiyat += td.satisBedeli;
                        }
                        XLS.HucreDegerYaz(satir, sutun + 12, ab.duzBirikenAmortisman);

                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 10, "BEDEL�");
                        XLS.SutunGizle(sutun + 8, sutun + 9, true);

                        toplamFiyat += ab.sonBedel;
                        toplamAmortisman += ab.duzBirikenAmortisman;
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, sutun + 8, (double)td.birimFiyatKDVLi);
                        XLS.HucreDegerYaz(satir, sutun + 9, (int)td.miktar);
                        XLS.HucreDegerYaz(satir, sutun + 10, (double)(td.birimFiyatKDVLi * td.miktar));
                        toplamFiyat += (double)(td.birimFiyatKDVLi * td.miktar);
                    }

                    toplamMiktar += (int)td.miktar;
                }
                XLS.HucreDegerYaz(satir + 1, sutun + 9, toplamMiktar);


                if (tf.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS)
                {
                    XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 10, "BEDEL�");
                    if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 3, "GELD��� YER");
                }
            }

            XLS.HucreDegerYaz(satir + 1, sutun + 10, toplamFiyat);
            XLS.HucreDegerYaz(satir + 1, sutun + 11, (double)toplamSatisFiyat);
            XLS.HucreDegerYaz(satir + 1, sutun + 12, toplamAmortisman);

            //Eklenen sat�rlar�n y�kseklikleri ayarlan�yor
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();

            if (gonder)
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, tf.fisNo, true, GenelIslemler.ExcelTur());
        }
    }
}