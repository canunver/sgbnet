using System.Data;
using OrtakClass;
using TNS;
using TNS.DEG;
using TNS.TMM;
using System.Web.UI.WebControls;
using System;
using Ext1.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace TasinirMal
{
    /// <summary>
    /// Devir giriþ iþlemi yapýlmamýþ devir çýkýþ taþýnýr iþlem fiþlerinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class GridYapistir : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Sayfa adresinde gelen yil, mb, hbk ve ak girdi dizgileri kullanýlarak devir giriþ
        ///     iþlemi yapýlmamýþ devir çýkýþ taþýnýr iþlem fiþleri cevap (response) olarak döndürülür.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnCagiran.Value = Request.QueryString["cagiran"] + "";
                hdnCagiranLabel.Value = Request.QueryString["cagiranLabel"] + "";
            }
        }

        [DirectMethod]
        public List<HesapPlaniSatir> HesapPlanKoduYapistir(string kriter)
        {
            string[] kodlar = kriter.Split('\n');
            List<HesapPlaniSatir> liste = new List<HesapPlaniSatir>();
            Hashtable gelenListe = new Hashtable();

            HesapPlaniSatir h = new HesapPlaniSatir();
            for (int i = 0; i < kodlar.Length; i++)
            {
                HesapPlaniSatir gelenBilgi = new HesapPlaniSatir();
                string[] satir = kodlar[i].Split('\t');

                if (satir.Length > 0) gelenBilgi.hesapKod = satir[0];
                if (satir.Length > 1) gelenBilgi.numara = satir[1];//miktar
                if (satir.Length > 2) gelenBilgi.kdv = OrtakFonksiyonlar.ConvertToInt(satir[2], 0);//kdv
                if (satir.Length > 3) gelenBilgi.hesapKodAciklama = satir[3];

                gelenBilgi.hesapKod = gelenBilgi.hesapKod.Trim().Replace(".", "").Trim();
                if (string.IsNullOrEmpty(gelenBilgi.hesapKod)) continue;

                if (h.hesapKodAciklama != "") h.hesapKodAciklama += " ";
                h.hesapKodAciklama += gelenBilgi.hesapKod;

                gelenListe[i] = gelenBilgi;
            }
            h.hesapKodAciklama = "@" + h.hesapKodAciklama;

            ObjectArray hesap = servisTMM.HesapPlaniListele(kullanan, h, new Sayfalama());

            for (int i = 0; i < kodlar.Length; i++)
            {
                foreach (HesapPlaniSatir item in hesap.objeler)
                {
                    HesapPlaniSatir gl = (HesapPlaniSatir)gelenListe[i];
                    if (gl == null) break;
                    if (gl.hesapKod == item.hesapKod.Replace(".", ""))
                    {
                        HesapPlaniSatir ekle = new HesapPlaniSatir();
                        ekle.hesapKod = item.hesapKod;
                        ekle.aciklama = item.aciklama;
                        ekle.olcuBirimAd = item.olcuBirimAd;

                        ekle.numara = gl.numara;
                        ekle.hesapKodAciklama = gl.hesapKodAciklama;
                        if (gl.kdv < 0)
                            ekle.kdv = item.kdv;
                        else
                            ekle.kdv = gl.kdv;

                        liste.Add(ekle);
                        break;
                    }
                }
            }

            return liste;
        }


        [DirectMethod]
        public List<SicilNoHareket> SicilNoYapistir(string kriter, string muhasebeKod, string harcamaBirimiKod)
        {
            bool merkezBankasi = TNS.TMM.Arac.MerkezBankasiKullaniyor();

            string[] kodlar = kriter.Split('\n');
            List<SicilNoHareket> liste = new List<SicilNoHareket>();
            Hashtable gelenListe = new Hashtable();
            Sayfalama sayfa = new Sayfalama();

            SicilNoHareket h = new SicilNoHareket();
            h.muhasebeKod = muhasebeKod;
            h.harcamaBirimKod = harcamaBirimiKod;

            for (int i = 0; i < kodlar.Length; i++)
            {
                SicilNoHareket gelenBilgi = new SicilNoHareket();
                string[] satir = kodlar[i].Split('\t');

                if (satir.Length > 0) gelenBilgi.sicilNo = satir[0];

                gelenBilgi.sicilNo = gelenBilgi.sicilNo.Trim().Replace(".", "").Trim();
                if (string.IsNullOrEmpty(gelenBilgi.sicilNo)) continue;

                if (h.sicilNoAciklama != "") h.sicilNoAciklama += " ";
                h.sicilNoAciklama += gelenBilgi.sicilNo;

                gelenListe[i] = gelenBilgi;
            }

            if (merkezBankasi)
                h.sorguButunSicilNolardaAra = "@" + h.sicilNoAciklama;
            else
                h.sicilNoAciklama = "@" + h.sicilNoAciklama;

            ObjectArray hesap = servisTMM.BarkodSicilNoListele(kullanan, h, sayfa);

            for (int i = 0; i < kodlar.Length; i++)
            {
                foreach (SicilNoHareket item in hesap.objeler)
                {
                    SicilNoHareket gl = (SicilNoHareket)gelenListe[i];
                    if (gl == null) break;
                    bool sicilBulundu = (gl.sicilNo == item.sicilNo.Replace(".", ""));

                    if (!sicilBulundu && merkezBankasi)
                        sicilBulundu = (gl.sicilNo == item.ozellik.disSicilNo.Replace(".", ""));

                    if (sicilBulundu)
                    {
                        SicilNoHareket ekle = new SicilNoHareket();
                        ekle.prSicilNo = item.prSicilNo;
                        ekle.sicilNo = item.sicilNo;
                        ekle.hesapPlanKod = item.hesapPlanKod;
                        ekle.hesapPlanAd = item.hesapPlanAd;

                        //ekle.fiyat = gl.fiyat; //Sicil Yapýþtýr iþleminde fiyat sýfýr geliyor.
                        ekle.fiyat = item.fiyat;
                        ekle.sicilNoAciklama = gl.sicilNoAciklama;

                        ekle.odaAd = item.odaAd;
                        ekle.eSicilNo = item.ozellik.disSicilNo;
                        if (item.ozellik.eskiBisNo1 != "")
                            ekle.eSicilNo += "-" + item.ozellik.eskiBisNo1;

                        liste.Add(ekle);
                        break;
                    }
                }
            }

            return liste;
        }

        [DirectMethod]
        public List<SicilNoHareket> SicilNoYapistirSicilOzellik(string kriter, string muhasebeKod, string harcamaBirimiKod)
        {
            bool merkezBankasi = TNS.TMM.Arac.MerkezBankasiKullaniyor();

            string[] kodlar = kriter.Split('\n');
            List<SicilNoHareket> liste = new List<SicilNoHareket>();
            Hashtable gelenListe = new Hashtable();
            Sayfalama sayfa = new Sayfalama();

            SicilNoHareket h = new SicilNoHareket();
            h.muhasebeKod = muhasebeKod;
            h.harcamaBirimKod = harcamaBirimiKod;

            for (int i = 0; i < kodlar.Length; i++)
            {
                SicilNoHareket gelenBilgi = new SicilNoHareket();
                string[] satir = kodlar[i].Split('\t');

                if (satir.Length > 0) gelenBilgi.sicilNo = satir[0];
                if (satir.Length > 1) gelenBilgi.sicilNoAciklama = satir[1];//açýklama

                gelenBilgi.sicilNo = gelenBilgi.sicilNo.Trim().Replace(".", "").Trim();
                if (string.IsNullOrEmpty(gelenBilgi.sicilNo)) continue;

                if (h.sicilNoAciklama != "") h.sicilNoAciklama += " ";
                h.sicilNoAciklama += gelenBilgi.sicilNo;

                gelenListe[i] = gelenBilgi;
            }

            if (merkezBankasi)
                h.sorguButunSicilNolardaAra = "@" + h.sicilNoAciklama;
            else
                h.sicilNoAciklama = "@" + h.sicilNoAciklama;

            ObjectArray hesap = servisTMM.BarkodSicilNoListele(kullanan, h, sayfa);

            for (int i = 0; i < kodlar.Length; i++)
            {
                foreach (SicilNoHareket item in hesap.objeler)
                {
                    SicilNoHareket gl = (SicilNoHareket)gelenListe[i];
                    if (gl == null) break;
                    bool sicilBulundu = (gl.sicilNo == item.sicilNo.Replace(".", ""));

                    if (!sicilBulundu && merkezBankasi)
                        sicilBulundu = (gl.sicilNo == item.ozellik.disSicilNo.Replace(".", ""));

                    if (sicilBulundu)
                    {
                        SicilNoHareket ekle = new SicilNoHareket();
                        ekle.prSicilNo = item.prSicilNo;
                        ekle.sicilNo = item.sicilNo;
                        ekle.hesapPlanKod = item.hesapPlanKod;
                        ekle.hesapPlanAd = item.hesapPlanAd;

                        //ekle.fiyat = gl.fiyat; //Sicil Yapýþtýr iþleminde fiyat sýfýr geliyor.
                        ekle.fiyat = item.fiyat;
                        ekle.sicilNoAciklama = gl.sicilNoAciklama;

                        ekle.odaAd = item.odaAd;
                        ekle.eSicilNo = item.ozellik.disSicilNo;
                        if (item.ozellik.eskiBisNo1 != "")
                            ekle.eSicilNo += "-" + item.ozellik.eskiBisNo1;

                        liste.Add(ekle);
                        break;
                    }
                }
            }

            return liste;
        }


    }
}