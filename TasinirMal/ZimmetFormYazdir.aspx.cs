using OrtakClass;
using System;
using System.IO;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fiþi raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetFormYazdir : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        static ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        static IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama, fisNo ve belgeTur
        ///     girdi dizgileri kullanýlarak istenen excel raporu hazýrlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0) == 0 || Request.QueryString["fisNo"] == string.Empty || Request.QueryString["harcama"] == string.Empty)
                return;

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            //Sayfaya giriþ izni varmý?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                GenelIslemler.SayfayaGirmesin(true);

            int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0);
            string fisNo = Request.QueryString["fisNo"];
            string harcamaBirimKod = Request.QueryString["harcama"].Replace(".", "");
            string muhasebeKod = Request.QueryString["muhasebe"].Replace(".", "");
            int belgeTur = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["belgeTur"], 0);
            bool resimEkle = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["resim"], 0) > 0;

            Yazdir(kullanan, yil, fisNo, harcamaBirimKod, muhasebeKod, belgeTur, "", resimEkle);
        }

        /// <summary>
        /// Parametre olarak verilen zimmet fiþi kriterleriyle ilgili zimmet
        /// fiþi bilgilerini sunucudan alýr ve ExceleYaz yordamýna yönlendirir.
        /// </summary>
        /// <param name="kullanan">Ýþlemi yapan kullanýcýya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yýl kriteri</param>
        /// <param name="fisNo">Zimmet fiþinin belge numarasý</param>
        /// <param name="harcamaKod">Harcama birimi kodu</param>
        /// <param name="muhasebeKod">Muhasebe birimi kodu</param>
        /// <param name="belgeTur">Ortak alan zimmet fiþi mi, yoksa kiþi zimmet fiþi mi bilgisi</param>
        /// <param name="excelYazYer">Rapor gönderilirken kullanýlacak dosya adý</param>
        public static void Yazdir(Kullanici kullanan, int yil, string fisNo, string harcamaKod, string muhasebeKod, int belgeTur, string excelYazYer, bool resimEkle)
        {
            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            zf.yil = yil;
            zf.fisNo = fisNo;
            zf.harcamaBirimKod = harcamaKod;
            zf.muhasebeKod = muhasebeKod;
            zf.belgeTur = belgeTur;

            ObjectArray bilgi = servisTMM.ZimmetFisiAc(kullanan, zf);

            if (!bilgi.sonuc.islemSonuc)
                return;

            zf = (TNS.TMM.ZimmetForm)bilgi[0];

            ZimmetDetay zd = new ZimmetDetay();

            zd.yil = zf.yil;
            zd.muhasebeKod = zf.muhasebeKod;
            zd.harcamaBirimKod = zf.harcamaBirimKod;
            zd.fisNo = zf.fisNo;
            zd.belgeTur = zf.belgeTur;

            ObjectArray detay = servisTMM.ZimmetFisiDetayListele(kullanan, zf);

            if (!detay.sonuc.islemSonuc)
                return;

            ExceleYaz(zf, detay, excelYazYer, resimEkle);
        }

        /// <summary>
        /// Parametre olarak verilen zimmet fiþi bilgilerini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="zf">Zimmet fiþinin üst bilgilerini tutan nesne</param>
        /// <param name="detay">Zimmet fiþinin detay bilgileri listesini tutan nesne</param>
        /// <param name="excelYazYer">Rapor gönderilirken kullanýlacak dosya adý</param>
        private static void ExceleYaz(TNS.TMM.ZimmetForm zf, ObjectArray detay, string excelYazYer, bool resimEkle)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            //int satir = 0;
            //int sutun = 0;
            //int tSatir = 0;

            string sablonAd = "";

            if (zf.belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
                sablonAd = "DTL.XLT";
            else if (zf.belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
            {
                if (zf.tip == (int)ENUMZimmetTipi.DEMIRBASCIHAZ)
                    sablonAd = "ZIFDEMIRBAS.XLT";
                else if (zf.tip == (int)ENUMZimmetTipi.TASITMAKINE)
                    sablonAd = "ZIFTASIT.XLT";
            }

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && zf.tip == (int)ENUMZimmetTipi.DEMIRBASCIHAZ && zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
                sablonAd = "ZIFDEMIRBASMB.XLT";

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

            //if (zf.belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
            //    goto Gecici; //Düzeltilecek

            //XLS.HucreAdBulYaz("txtFisNo", zf.fisNo);
            //XLS.HucreAdBulYaz("txtBelgeTarih", "TARÝHÝ:" + zf.fisTarih.ToString());
            //XLS.HucreAdBulYaz("txtIlIlceAd", zf.ilAd + " - " + zf.ilceAd);
            //XLS.HucreAdBulYaz("txtIlIlceKod", zf.ilKod + " - " + zf.ilceKod);
            //XLS.HucreAdBulYaz("txtHarcamaKod", zf.harcamaBirimKod);
            //XLS.HucreAdBulYaz("txtHarcamaAd", zf.harcamaBirimAd);

            if (zf.belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
                DayanikliYaz(XLS, zf, detay, resimEkle);
            else if (zf.belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
            {
                if (zf.tip == (int)ENUMZimmetTipi.DEMIRBASCIHAZ)
                    DemirbasCihazYaz(XLS, zf, detay, resimEkle);
                else if (zf.tip == (int)ENUMZimmetTipi.TASITMAKINE)
                    TasitMakineYaz(XLS, zf, detay, resimEkle);
            }

            XLS.DosyaSaklaTamYol();

            if (gonder)
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, zf.fisNo, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen ortak alan zimmet fiþi bilgilerini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="zf">Ortak alan zimmet fiþinin üst bilgilerini tutan nesne</param>
        /// <param name="detay">Ortak alan zimmet fiþinin detay bilgileri listesini tutan nesne</param>
        private static void DayanikliYaz(Tablo XLS, TNS.TMM.ZimmetForm zf, ObjectArray detay, bool resimEkle)
        {
            XLS.HucreAdBulYaz("BelgeNo", zf.fisNo);
            XLS.HucreAdBulYaz("BelgeTarih", zf.fisTarih.ToString());
            XLS.HucreAdBulYaz("IlAd", zf.ilAd + "-" + zf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", zf.ilKod + "-" + zf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", zf.harcamaBirimAd);
            XLS.HucreAdBulYaz("HarcamaKod", zf.harcamaBirimKod + "-" + zf.ambarKod);

            if (!string.IsNullOrEmpty(zf.nereyeGitti))
            {
                string oda = servisUZY.UzayDegeriStr(null, "TASODA", zf.muhasebeKod + "-" + zf.harcamaBirimKod + "-" + zf.nereyeGitti, true, "");
                if (string.IsNullOrEmpty(oda))
                    XLS.HucreAdBulYaz("Yer", zf.nereyeGitti);
                else
                    XLS.HucreAdBulYaz("Yer", zf.nereyeGitti + "-" + oda);
            }

            if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
            {
                XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY001);
                XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY002);
                XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY003);
            }
            else if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
            {
                XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY004);
                XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY005);
                XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY006);
            }

            //Satýr ekleyince hücre adresleri kaydýðý için önce yazýyorum
            XLS.HucreAdBulYaz("Tarih", DateTime.Today.Date.ToShortDateString());
            if (!string.IsNullOrEmpty(zf.kimeGitti))
            {
                string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "");
                if (string.IsNullOrEmpty(ad))
                    XLS.HucreAdBulYaz("AdSoyad2", zf.kimeGitti);
                else
                    XLS.HucreAdBulYaz("AdSoyad2", ad);

                string unvan = servisUZY.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "UNVAN");
                if (!string.IsNullOrEmpty(unvan))
                    XLS.HucreAdBulYaz("Unvan2", unvan);
            }
            ImzaEkle(XLS, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);

            int kaynakSatir = 0;
            int sutun = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int sayac = 0;
            int satir = kaynakSatir;
            foreach (ZimmetDetay zd in detay.objeler)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);

                if (resimEkle)
                {
                    XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                    XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                }
                else
                    XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 4);

                XLS.HucreBirlestir(satir, sutun + 6, satir, sutun + 7);
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

                string disSicilNo = "";
                if (zd.ozellikSicil.disSicilNo != "")
                    disSicilNo = "(" + zd.ozellikSicil.disSicilNo + ") ";

                string yazilacakBilgi = zd.hesapPlanAd;
                string ozellik = OzellikBilgileri(zd);
                if (!string.IsNullOrEmpty(ozellik))
                    yazilacakBilgi += "-" + ozellik;

                XLS.HucreDegerYaz(satir, sutun, ++sayac);
                XLS.HucreDegerYaz(satir, sutun + 1, yazilacakBilgi);
                XLS.HucreDegerYaz(satir, sutun + 5, "1");

                if (TasinirGenel.rfIdVarMi)
                    XLS.HucreDegerYaz(satir, sutun + 6, zd.gorSicilNo + " " + zd.rfIdNo + " " + disSicilNo);
                else
                    XLS.HucreDegerYaz(satir, sutun + 6, zd.gorSicilNo + " " + disSicilNo);

                if (resimEkle)
                {
                    byte[] resimByte = new byte[0];
                    ObjectArray liste = servisTMM.TasinirResimGetir(zd.prSicilNo, "");
                    foreach (TasResim resim in liste.objeler)
                    {
                        string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
                        resim.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirResimleri");
                        string dosyaYol = Path.Combine(resim.kayitEdilecekDosyaYol, "RESIM");
                        dosyaYol = Path.Combine(dosyaYol, "RESIM" + "_" + resim.resimID);
                        if (File.Exists(dosyaYol))
                            resim.resim = File.ReadAllBytes(dosyaYol);

                        resimByte = resim.resim;
                        break;
                    }

                    if (resimByte.Length > 0)
                    {
                        TasResim r = TanimDemirbasResim.ResimBoyutlandir(resimByte, System.Drawing.RotateFlipType.RotateNoneFlipNone, 230, 160);
                        System.IO.MemoryStream msResim = new System.IO.MemoryStream(r.resim);
                        System.Drawing.Image orjinalResim = System.Drawing.Image.FromStream(msResim);

                        XLS.ResimEkle(satir, sutun + 3, orjinalResim.Width, orjinalResim.Height, msResim);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(2400));
                    }
                }
            }
        }

        /// <summary>
        /// Parametre olarak verilen kiþi zimmet fiþi bilgilerini (Demirbaþ, makine veya cihaz) excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="zf">Kiþi zimmet fiþinin üst bilgilerini (Demirbaþ, makine veya cihaz) tutan nesne</param>
        /// <param name="detay">Kiþi zimmet fiþinin detay bilgileri (Demirbaþ, makine veya cihaz) listesini tutan nesne</param>
        private static void DemirbasCihazYaz(Tablo XLS, TNS.TMM.ZimmetForm zf, ObjectArray detay, bool resimEkle)
        {
            XLS.HucreAdBulYaz("BelgeNo", zf.fisNo);
            XLS.HucreAdBulYaz("BelgeTarih", zf.fisTarih.ToString());
            XLS.HucreAdBulYaz("IlAd", zf.ilAd + "-" + zf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", zf.ilKod + "-" + zf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", zf.harcamaBirimAd);
            XLS.HucreAdBulYaz("HarcamaKod", zf.harcamaBirimKod + "-" + zf.ambarKod);

            if (!string.IsNullOrEmpty(zf.kimeGitti))
            {
                string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "");
                if (string.IsNullOrEmpty(ad))
                    XLS.HucreAdBulYaz("KimeVerildi", zf.kimeGitti);
                else
                    XLS.HucreAdBulYaz("KimeVerildi", zf.kimeGitti + "-" + ad);
            }

            if (!string.IsNullOrEmpty(zf.nereyeGitti))
            {
                string oda = servisUZY.UzayDegeriStr(null, "TASODA", zf.muhasebeKod + "-" + zf.harcamaBirimKod + "-" + zf.nereyeGitti, true, "");
                if (string.IsNullOrEmpty(oda))
                    XLS.HucreAdBulYaz("NereyeVerildi", zf.nereyeGitti);
                else
                    XLS.HucreAdBulYaz("NereyeVerildi", zf.nereyeGitti + "-" + oda);
            }

            if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
            {
                XLS.HucreAdBulYaz("KimeVerildiYazi", Resources.TasinirMal.FRMZFY007);
                XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY008);
                XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY002);
                XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY003);
            }
            else if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
            {
                XLS.HucreAdBulYaz("KimeVerildiYazi", Resources.TasinirMal.FRMZFY009);
                XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY010);
                XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY005);
                XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY006);
            }

            //Satýr ekleyince hücre adresleri kaydýðý için önce yazýyorum
            XLS.HucreAdBulYaz("Tarih1", DateTime.Today.Date.ToShortDateString());
            //XLS.HucreAdBulYaz("Tarih2", DateTime.Today.Date.ToShortDateString());
            if (!string.IsNullOrEmpty(zf.kimeGitti))
            {
                string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "");
                if (string.IsNullOrEmpty(ad))
                    XLS.HucreAdBulYaz("AdSoyad2", zf.kimeGitti);
                else
                    XLS.HucreAdBulYaz("AdSoyad2", ad);

                string unvan = servisUZY.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "UNVAN");
                if (!string.IsNullOrEmpty(unvan))
                    XLS.HucreAdBulYaz("Unvan2", unvan);
            }

            ImzaEkle(XLS, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);

            int kaynakSatir = 0;
            int sutun = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int sayac = 0;
            int satir = kaynakSatir;
            foreach (ZimmetDetay zd in detay.objeler)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 5);
                XLS.HucreBirlestir(satir, sutun + 6, satir, sutun + 7);

                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

                XLS.HucreDegerYaz(satir, sutun, ++sayac);

                if (TasinirGenel.rfIdVarMi)
                    XLS.HucreDegerYaz(satir, sutun + 1, zd.gorSicilNo + " - " + zd.rfIdNo);
                else
                    XLS.HucreDegerYaz(satir, sutun + 1, zd.gorSicilNo);


                string disSicilNo = "";
                if (zd.ozellikSicil.disSicilNo != "")
                    disSicilNo = "(" + zd.ozellikSicil.disSicilNo + ") ";

                string yazilacakBilgi = zd.hesapPlanAd;
                string ozellik = OzellikBilgileri(zd);
                //if (!string.IsNullOrEmpty(ozellik))
                //    yazilacakBilgi += "-" + ozellik;

                XLS.HucreDegerYaz(satir, sutun + 3, yazilacakBilgi);

                if (TasinirGenel.tasinirZimmeteOnay)
                {
                    Kullanici kul = OturumBilgisiIslem.KullaniciBilgiOku(true);
                    ObjectArray sler = servisTMM.SicilNoOzellikListele(kul, zd.prSicilNo);
                    string saseNo = string.Empty;
                    if (sler.sonuc.islemSonuc && sler.objeler.Count > 0)
                    {
                        TNS.TMM.SicilNoOzellik s = (TNS.TMM.SicilNoOzellik)sler.objeler[0];
                        saseNo = s.saseNo;
                    }
                    XLS.HucreDegerYaz(satir, sutun + 6, ozellik + " " + saseNo + " " + disSicilNo);
                }
                else
                    XLS.HucreDegerYaz(satir, sutun + 6, ozellik + " " + disSicilNo);

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
                        XLS.HucreDegerYaz(satir, sutun + 8, zd.ozellikSicilEski.bulunduguYer + " - " + zd.ozellikSicilEski.bulunduguYerAd);
                }

                if (resimEkle)
                {
                    byte[] resimByte = new byte[0];
                    ObjectArray liste = servisTMM.TasinirResimGetir(zd.prSicilNo, "");
                    foreach (TasResim resim in liste.objeler)
                    {
                        string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
                        resim.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirResimleri");
                        string dosyaYol = Path.Combine(resim.kayitEdilecekDosyaYol, "RESIM");
                        dosyaYol = Path.Combine(dosyaYol, "RESIM" + "_" + resim.resimID);
                        if (File.Exists(dosyaYol))
                            resim.resim = File.ReadAllBytes(dosyaYol);

                        resimByte = resim.resim;
                        break;
                    }

                    if (resimByte.Length > 0)
                    {
                        TasResim r = TanimDemirbasResim.ResimBoyutlandir(resimByte, System.Drawing.RotateFlipType.RotateNoneFlipNone, 230, 160);
                        System.IO.MemoryStream msResim = new System.IO.MemoryStream(r.resim);
                        System.Drawing.Image orjinalResim = System.Drawing.Image.FromStream(msResim);

                        XLS.ResimEkle(satir, sutun + 6, orjinalResim.Width, orjinalResim.Height, msResim);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(2400));
                    }
                }

            }
        }

        /// <summary>
        /// Parametre olarak verilen kiþi zimmet fiþi bilgilerini (Taþýt veya iþ makinesi) excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="zf">Kiþi zimmet fiþinin üst bilgilerini (Taþýt veya iþ makinesi) tutan nesne</param>
        /// <param name="detay">Kiþi zimmet fiþinin detay bilgileri (Taþýt veya iþ makinesi) listesini tutan nesne</param>
        private static void TasitMakineYaz(Tablo XLS, TNS.TMM.ZimmetForm zf, ObjectArray detay, bool resimEkle)
        {
            XLS.HucreAdBulYaz("BelgeNo", zf.fisNo);
            XLS.HucreAdBulYaz("BelgeTarih", zf.fisTarih.ToString());
            XLS.HucreAdBulYaz("IlAd", zf.ilAd + "-" + zf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", zf.ilKod + "-" + zf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", zf.harcamaBirimAd);
            XLS.HucreAdBulYaz("HarcamaKod", zf.harcamaBirimKod + "-" + zf.ambarKod);

            ZimmetDetay zd = (ZimmetDetay)detay.objeler[0];

            if (TasinirGenel.rfIdVarMi)
                XLS.HucreAdBulYaz("SicilNo", zd.gorSicilNo + " - " + zd.rfIdNo);
            else
                XLS.HucreAdBulYaz("SicilNo", zd.gorSicilNo);

            XLS.HucreAdBulYaz("SicilAd", zd.hesapPlanAd);
            XLS.HucreAdBulYaz("Diger", zd.ozellik);
            XLS.HucreAdBulYaz("Durum", zd.teslimDurum);


            Kullanici kul = OturumBilgisiIslem.KullaniciBilgiOku(true);
            ObjectArray sler = servisTMM.SicilNoOzellikListele(kul, zd.prSicilNo);
            if (sler.sonuc.islemSonuc && sler.objeler.Count > 0)
            {
                TNS.TMM.SicilNoOzellik s = (TNS.TMM.SicilNoOzellik)sler.objeler[0];
                XLS.HucreAdBulYaz("Marka", s.markaAd);
                XLS.HucreAdBulYaz("Model", s.modelAd);
                XLS.HucreAdBulYaz("SaseNo", s.saseNo);
                XLS.HucreAdBulYaz("MotorNo", s.motorNo);
                XLS.HucreAdBulYaz("PlakaNo", s.plaka);
            }

            if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
            {
                XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY008);
                XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY002);
                XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY003);
            }
            else if (zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
            {
                XLS.HucreAdBulYaz("TurAciklama", Resources.TasinirMal.FRMZFY010);
                XLS.HucreAdBulYaz("TeslimAlVer", Resources.TasinirMal.FRMZFY005);
                XLS.HucreAdBulYaz("VermeDusme", Resources.TasinirMal.FRMZFY006);
            }

            XLS.HucreAdBulYaz("Tarih1", DateTime.Today.Date.ToShortDateString());
            //XLS.HucreAdBulYaz("Tarih2", DateTime.Today.Date.ToShortDateString());
            if (!string.IsNullOrEmpty(zf.kimeGitti))
            {
                string ad = servisUZY.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "");
                if (string.IsNullOrEmpty(ad))
                    XLS.HucreAdBulYaz("AdSoyad2", zf.kimeGitti);
                else
                    XLS.HucreAdBulYaz("AdSoyad2", ad);

                string unvan = servisUZY.UzayDegeriStr(null, "TASPERSONEL", zf.kimeGitti, true, "UNVAN");
                if (!string.IsNullOrEmpty(unvan))
                    XLS.HucreAdBulYaz("Unvan2", unvan);
            }
            ImzaEkle(XLS, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);
        }

        /// <summary>
        /// Zimmet fiþi excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        private static void ImzaEkle(Tablo XLS, string muhasebe, string harcama, string ambar)
        {
            Kullanici kul = OturumBilgisiIslem.KullaniciBilgiOku(true);
            ObjectArray imza = servisTMM.ImzaListele(kul, muhasebe, harcama, ambar, (int)ENUMImzaYer.TASINIRKAYITYETKILISI);

            ImzaBilgisi iBilgi = new ImzaBilgisi();
            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
                iBilgi = (ImzaBilgisi)imza[0];

            if (!string.IsNullOrEmpty(iBilgi.adSoyad))
                XLS.HucreAdBulYaz("AdSoyad1", iBilgi.adSoyad);

            if (!string.IsNullOrEmpty(iBilgi.unvan))
                XLS.HucreAdBulYaz("Unvan1", iBilgi.unvan);
        }

        private static string OzellikBilgileri(ZimmetDetay zd)
        {
            string ozellik = "";

            if (!string.IsNullOrEmpty(zd.ozellikSicil.markaAd))
                ozellik = zd.ozellikSicil.markaAd;
            if (!string.IsNullOrEmpty(zd.ozellikSicil.modelAd))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.modelAd;
            }
            if (!string.IsNullOrEmpty(zd.ozellikSicil.plaka))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.plaka;
            }
            if (!string.IsNullOrEmpty(zd.ozellikSicil.adi))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.adi;
            }
            if (!string.IsNullOrEmpty(zd.ozellikSicil.yazarAdi))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.yazarAdi;
            }
            if (!string.IsNullOrEmpty(zd.ozellikSicil.saseNo))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.saseNo;
            }
            if (!string.IsNullOrEmpty(zd.ozellikSicil.yeriKonusu))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.yeriKonusu;
            }
            if (!string.IsNullOrEmpty(zd.ozellik))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellik;
            }
            if (!string.IsNullOrEmpty(zd.teslimDurum))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.teslimDurum;
            }
            if (!string.IsNullOrEmpty(zd.ozellikSicil.ekNo))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.ekNo;
            }
            if (!string.IsNullOrEmpty(zd.ozellikSicil.giai))
            {
                if (ozellik != "") ozellik += "-";
                ozellik += zd.ozellikSicil.giai;
            }

            return ozellik;
        }


    }
}