using Ext1.Net;
using System.Linq;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþi bilgilerinin sorgulama, yazdýrma ve durum deðiþtirme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIslemFormSorgu : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý kütüphane malzemeleri için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        static bool kutuphaneGoster = false;

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý müze malzemeleri için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        static bool muzeGoster = false;

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý daðýtým Ýade için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        static bool dagitimIade = false;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //Sayfaya giriþ izni varmý?
                bool izin = false;
                if (dagitimIade)
                    izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan);
                else
                    izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan);

                if (!izin)
                    GenelIslemler.SayfayaGirmesin(true);

                kutuphaneGoster = (Request.QueryString["kutuphane"] + "" != "") ? true : false;
                muzeGoster = (Request.QueryString["muze"] + "" != "") ? true : false;
                dagitimIade = (Request.QueryString["dagitimIade"] + "" != "") ? true : false;

                DurumDoldur();
                IslemTipiDoldur();

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                //pgFiltre.UpdateProperty("prpDurum", OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ZIMDURUM"), 0));
                pgFiltre.UpdateProperty("prpDurum", 0);

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    string ambar = "";

                    if (EkranTurDondur() == "GM")
                    {
                        string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULHESAPKOD") + "";

                        pgFiltre.UpdateProperty("prpHesapKod", "@" + hesapKodGM);
                        pgFiltre.Source["prpHesapKod"].Editor.Editor.ReadOnly = true;

                        ambar = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULAMBAR") + "";
                    }
                    else if (EkranTurDondur() == "YZ")
                    {
                        string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMHESAPKOD") + "";

                        pgFiltre.UpdateProperty("prpHesapKod", "@" + hesapKodGM);
                        pgFiltre.Source["prpHesapKod"].Editor.Editor.ReadOnly = true;

                        ambar = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMAMBAR") + "";
                    }
                    else
                        ambar = "01";

                    pgFiltre.UpdateProperty("prpAmbar", ambar);
                    pgFiltre.Source["prpAmbar"].Editor.Editor.ReadOnly = true;
                }
                else
                    pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                //B-A Onayý Kontrolü
                string baOnayi = OrtakFonksiyonlar.WebConfigOku("TasinirBAOnayi", "");
                if (baOnayi == "1")
                {
                    btnOnayaGonder.Hidden = false;
                    btnOnayla.Hidden = true;
                    btnOnayKaldir.Hidden = true;
                }
                //*******************************************
            }
        }

        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("fisNo")   { DataType = typeof(string) },
                new DataColumn("yil")     { DataType = typeof(int) },
                new DataColumn("muhasebe")    { DataType = typeof(string) },
                new DataColumn("harcamaBirimi") { DataType = typeof(string) },
                new DataColumn("harcamaBirimiAd") { DataType = typeof(string) },
                new DataColumn("ambar") { DataType = typeof(string) },
                new DataColumn("fisTarihi") { DataType = typeof(DateTime) },
                new DataColumn("islemTipi") { DataType = typeof(int) },
                new DataColumn("durum") { DataType = typeof(string) },
                new DataColumn("islemTarih") { DataType = typeof(DateTime) },
                new DataColumn("islemYapan") { DataType = typeof(string) },
                new DataColumn("kod") { DataType = typeof(string) },
                new DataColumn("faturaTarihi") { DataType = typeof(DateTime) },
                new DataColumn("faturaNo") { DataType = typeof(string) },
           });

            ObjectArray bilgiler = TasinirIslemFisiListele();

            if (!bilgiler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgiler.sonuc.hataStr);
                return;
            }

            if (bilgiler.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTIS057);
                return;
            }
            foreach (TNS.TMM.TasinirIslemForm tasForm in bilgiler.objeler)
            {
                string durum = "";
                if (tasForm.durum == (int)ENUMBelgeDurumu.ONAYLI && tasForm.onayDurum == 0 && TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    durum = "Numara Verildi";
                else if (tasForm.durum == (int)ENUMBelgeDurumu.YENI)
                    durum = Resources.TasinirMal.FRMTIS006;
                else if (tasForm.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    durum = Resources.TasinirMal.FRMTIS007;
                else if (tasForm.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    durum = Resources.TasinirMal.FRMTIS008;
                else if (tasForm.durum == (int)ENUMBelgeDurumu.IPTAL)
                    durum = Resources.TasinirMal.FRMTIS009;

                if (!dagitimIade && (tasForm.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tasForm.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                    continue;
                if (dagitimIade && (tasForm.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADECIKIS && tasForm.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                    continue;

                dt.Rows.Add(tasForm.fisNo.Trim(), tasForm.yil, tasForm.muhasebeKod, tasForm.harcamaKod, tasForm.harcamaAd, tasForm.ambarKod + "-" + tasForm.ambarAd, tasForm.fisTarih.Oku(), tasForm.islemTipKod, durum, tasForm.islemTarih.Oku(), tasForm.islemYapan, tasForm.kod, tasForm.faturaTarih.Oku(), tasForm.faturaNo);
            }

            strListe.DataSource = dt;
            strListe.DataBind();
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
            pgFiltre.UpdateProperty("prpBelgeNo1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNo2", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi2", string.Empty);
            pgFiltre.UpdateProperty("prpDurumTarihi1", string.Empty);
            pgFiltre.UpdateProperty("prpDurumTarihi2", string.Empty);
            pgFiltre.UpdateProperty("prpNereye", string.Empty);
            pgFiltre.UpdateProperty("prpFaturaNo", string.Empty);
            pgFiltre.UpdateProperty("prpKime", string.Empty);
            pgFiltre.UpdateProperty("prpHesapKod", string.Empty);
            pgFiltre.UpdateProperty("prpNereden", string.Empty);
            pgFiltre.UpdateProperty("prpIslemTipi", string.Empty);
            pgFiltre.UpdateProperty("prpIslemYapan", string.Empty);
            pgFiltre.UpdateProperty("prpGonMuhasebe", string.Empty);
            pgFiltre.UpdateProperty("prpGonHarcamaBirimi", string.Empty);
            pgFiltre.UpdateProperty("prpGonAmbar", string.Empty);
            pgFiltre.UpdateProperty("prpAciklama", string.Empty);
        }

        /// <summary>
        /// Liste Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplanýr, sunucuya gönderilir
        /// ve taþýnýr iþlem fiþi bilgileri sunucudan alýnýr. Hata varsa ekrana hata bilgisi
        /// yazýlýr, yoksa gelen taþýnýr iþlem fiþi bilgilerini içeren excel raporu üretilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListeYazdir_Click(object sender, DirectEventArgs e)
        {
            ObjectArray bilgi = TasinirIslemFisiListele();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TIFListe.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            List<IslemTip> islemTipListe = TasinirGenel.IslemTipListele(servisTMM, kullanan, dagitimIade);

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                TNS.TMM.TasinirIslemForm tifBelge = (TNS.TMM.TasinirIslemForm)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, tifBelge.yil);
                XLS.HucreDegerYaz(satir, sutun + 1, tifBelge.muhasebeKod + " - " + tifBelge.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 2, tifBelge.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 3, tifBelge.fisNo);

                var islemTip = islemTipListe.Find(x => x.kod == tifBelge.islemTipKod);

                XLS.HucreDegerYaz(satir, sutun + 4, islemTip.ad);

                string durum = "";
                if (tifBelge.durum == (int)ENUMBelgeDurumu.YENI)
                    durum = Resources.TasinirMal.FRMTIS006;
                else if (tifBelge.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    durum = Resources.TasinirMal.FRMTIS007;
                else if (tifBelge.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    durum = Resources.TasinirMal.FRMTIS008;
                else if (tifBelge.durum == (int)ENUMBelgeDurumu.IPTAL)
                    durum = Resources.TasinirMal.FRMTIS009;
                XLS.HucreDegerYaz(satir, sutun + 5, durum);

                XLS.HucreDegerYaz(satir, sutun + 6, tifBelge.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 7, tifBelge.faturaTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, tifBelge.islemTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 9, tifBelge.islemYapan.ToString());
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        protected void btnIslem_Click(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["json"];
            string islem = e.ExtraParams["islem"];

            if (string.IsNullOrEmpty(json) || ((JArray)JSON.Deserialize(json)).Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listeden iþlem yapýlacak belge seçilmemiþ.");
                return;
            }

            if (islem == "Yazdir")
            {
                string tempFileName = System.IO.Path.GetTempFileName();
                string klasor = tempFileName + ".dir";
                System.IO.DirectoryInfo dri = System.IO.Directory.CreateDirectory(klasor);
                klasor += "\\";

                foreach (JContainer jc in (JArray)JSON.Deserialize(json))
                {
                    string tmpFile = System.IO.Path.GetTempFileName();
                    string excelYazYer = klasor + jc.Value<string>("fisNo") + "." + GenelIslemler.ExcelTur();

                    System.IO.File.Move(tmpFile, excelYazYer);
                    System.IO.File.Delete(tmpFile);

                    string tifTur = string.Empty;
                    if (Request.QueryString["kutuphane"] == "1")
                        tifTur = "kutuphane";
                    else if (Request.QueryString["muze"] == "1")
                        tifTur = "muze";
                    TasinirIslemFormYazdir.Yazdir(kullanan, jc.Value<int>("yil"), jc.Value<string>("fisNo"), jc.Value<string>("harcamaBirimi"), jc.Value<string>("muhasebe"), excelYazYer, tifTur);
                }

                string[] dosyalar = { "" };
                string sonucDosyaAd = System.IO.Path.GetTempFileName();

                OrtakClass.Zip.Ziple(dri.FullName, sonucDosyaAd, dosyalar);
                dri.Delete(true);
                System.IO.File.Delete(tempFileName);
                OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "Belgeler.zip", true, "zip");
            }
            else if (islem == "OnayaGonder")
            {
                Sonuc sonuc = new Sonuc();
                string bilgiStr = string.Empty;

                foreach (JContainer jc in (JArray)JSON.Deserialize(json))
                {
                    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                    tf.yil = jc.Value<int>("yil");
                    tf.muhasebeKod = jc.Value<string>("muhasebe");
                    tf.harcamaKod = (jc.Value<string>("harcamaBirimi") + "").Replace(".", "");
                    tf.fisNo = (jc.Value<string>("fisNo") + "").PadLeft(6, '0');
                    tf.kod = jc.Value<string>("kod");

                    sonuc = servisTMM.TasinirIslemFisiOnayDurumDegistir(kullanan, tf, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);
                    if (!sonuc.islemSonuc)
                    {
                        GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                        return;
                    }
                    else
                        bilgiStr += sonuc.bilgiStr;
                }

                GenelIslemler.ExtNotification(bilgiStr, "Bilgi", Icon.LightningGo);

            }
            else
            {
                Sonuc sonuc = new Sonuc();
                string bilgiStr = string.Empty;

                foreach (JContainer jc in (JArray)JSON.Deserialize(json))
                {
                    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                    tf.yil = jc.Value<int>("yil");
                    tf.muhasebeKod = jc.Value<string>("muhasebe");
                    tf.harcamaKod = jc.Value<string>("harcamaBirimi");
                    tf.fisNo = jc.Value<string>("fisNo");

                    sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, islem);

                    if (sonuc.hataStr.StartsWith("Belgenin onayýnýn kaldýrýlabilmesi için"))
                    {
                        bilgiStr += sonuc.hataStr;
                        sonuc.islemSonuc = true;
                    }

                    if (!sonuc.islemSonuc)
                    {
                        GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                        return;
                    }
                    else
                        bilgiStr += sonuc.bilgiStr;
                }

                GenelIslemler.ExtNotification(bilgiStr, "Bilgi", Icon.Information);

            }
        }

        private ObjectArray TasinirIslemFisiListele()
        {
            string hata = "";
            TNS.TMM.TasinirIslemForm tf = TasinirIslemFormSorguOku();

            GenelIslemler.KullaniciDegiskenSakla(kullanan, "ZIMDURUM", tf.durum.ToString());

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRKULLANICIBIRIM))
            {
                if (tf.muhasebeKod == "" || tf.harcamaKod == "" || tf.ambarKod == "")
                    hata = Resources.TasinirMal.FRMLDV001;
            }

            if (hata == "")
            {
                TasinirGenel.DegiskenleriKaydet(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

                TasinirFormKriter kriter = TasinirFormKriterSorguOku();

                if (kutuphaneGoster && kriter.hesapKodu != "")
                {
                    if (kriter.hesapKodu.IndexOf(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString()) < 0)
                        kriter.hesapKodu = "";
                }
                if (muzeGoster && kriter.hesapKodu != "")
                {
                    if (kriter.hesapKodu.IndexOf(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString()) < 0)
                        kriter.hesapKodu = "";
                }

                if (kutuphaneGoster && kriter.hesapKodu == "")
                    kriter.hesapKodu = OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString();
                else if (muzeGoster && kriter.hesapKodu == "")
                    kriter.hesapKodu = OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString();

                return servisTMM.TasinirIslemFisiListele(kullanan, tf, kriter);
            }
            else
            {
                GenelIslemler.MesajKutusu("Hata", hata);
                return null;
            }

        }

        private void DurumDoldur()
        {
            List<object> liste = new List<object>();

            liste.Add(new { KOD = 1, ADI = Resources.TasinirMal.FRMTIS002 });
            liste.Add(new { KOD = 5, ADI = Resources.TasinirMal.FRMTIS003 });
            liste.Add(new { KOD = 9, ADI = Resources.TasinirMal.FRMTIS004 });
            liste.Add(new { KOD = 0, ADI = Resources.TasinirMal.FRMTIS005 });

            strDurum.DataSource = liste;
            strDurum.DataBind();
        }

        private void IslemTipiDoldur()
        {
            List<object> liste = new List<object>();

            ObjectArray bilgi = servisTMM.IslemTipListele(kullanan, new IslemTip());

            liste.Add(new { KOD = (dagitimIade ? -999 : 0), ADI = Resources.TasinirMal.FRMTIS010 });

            foreach (IslemTip tip in bilgi.objeler)
            {
                if (dagitimIade)
                {
                    if (tip.tur != (int)ENUMIslemTipi.DAGITIMIADECIKIS && tip.tur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        continue;
                }
                else
                {
                    if (tip.tur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tip.tur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        continue;
                }
                liste.Add(new { KOD = tip.kod, ADI = tip.ad });
            }

            strIslemTipi.DataSource = liste;
            strIslemTipi.DataBind();
        }

        private TNS.TMM.TasinirIslemForm TasinirIslemFormSorguOku()
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0);
            tf.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurum"].Value.Trim(), 0);
            tf.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            tf.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim();
            tf.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            tf.gMuhasebeKod = pgFiltre.Source["prpGonMuhasebe"].Value.Trim();
            tf.gHarcamaKod = pgFiltre.Source["prpGonHarcamaBirimi"].Value.Trim();
            tf.gAmbarKod = pgFiltre.Source["prpGonAmbar"].Value.Trim();

            return tf;
        }

        private TasinirFormKriter TasinirFormKriterSorguOku()
        {
            TasinirFormKriter kriter = new TasinirFormKriter();
            kriter.belgeNoBasla = pgFiltre.Source["prpBelgeNo1"].Value.Trim() == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo1"].Value.Trim().PadLeft(6, '0');
            kriter.belgeNoBit = pgFiltre.Source["prpBelgeNo2"].Value.Trim() == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo2"].Value.Trim().PadLeft(6, '0');
            kriter.belgeTarihBasla = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            kriter.belgeTarihBit = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            kriter.durumTarihBasla = new TNSDateTime(pgFiltre.Source["prpDurumTarihi1"].Value.Trim());
            kriter.durumTarihBit = new TNSDateTime(pgFiltre.Source["prpDurumTarihi2"].Value.Trim());
            kriter.hesapKodu = pgFiltre.Source["prpHesapKod"].Value.Trim().Replace(".", "");
            kriter.islemTipi = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpIslemTipi"].Value.Trim(), 0);
            kriter.nereyeVeridi = pgFiltre.Source["prpNereye"].Value.Trim();
            kriter.kimeVeridi = pgFiltre.Source["prpKime"].Value.Trim();
            kriter.neredenGeldi = pgFiltre.Source["prpNereden"].Value.Trim();
            kriter.faturaNo = pgFiltre.Source["prpFaturaNo"].Value.Trim();
            kriter.aciklama = pgFiltre.Source["prpAciklama"].Value.Trim();

            if (EkranTurDondur() == "" && TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULHESAPKOD") + "";
                string hesapKodYZ = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMHESAPKOD") + "";

                kriter.degilHesapKodu = (hesapKodGM + " " + hesapKodYZ).Trim();
            }

            return kriter;
        }

        public string EkranTurDondur()
        {
            string tur = Request.QueryString["gm"] + "";
            if (tur == "1") return "GM";
            else if (tur == "2") return "YZ";
            else return "";
        }
    }
}