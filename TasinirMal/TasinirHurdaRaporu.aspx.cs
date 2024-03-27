using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using TNS.UZY;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Hurda ve kullanýlmaz taþýnýr malzeme bilgilerinin raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirHurdaRaporu : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

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
                formAdi = Resources.TasinirMal.FRMTHU001;

                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgileri toplanýr ve toplanan kriterler chk2Duzey CheckBox kontrolü
        /// iþaretliyse TasinirIslemTarihceRaporu2Duzey yordamýna, iþaretli deðilse TasinirIslemTarihceRaporu
        /// yordamýna gönderilir, böylece excel raporu üretilip kullanýcýya gönderilmiþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();

            TNS.TMM.TasinirFormKriter kriter = new TNS.TMM.TasinirFormKriter();
            kriter.belgeTarihBasla = new TNSDateTime(txtTarih1.RawText);
            kriter.belgeTarihBit = new TNSDateTime(txtTarih2.RawText);
            kriter.hesapKodu = txtHesapPlanKod.Text.Trim();
            //kriter.islemTipleri = "11;12;17";//"54;55;56"
            int[] turListe = { 54, 55 };//56 
            kriter.islemTipleri = TipListesiVer(turListe);

            if (string.IsNullOrEmpty(tf.muhasebeKod))
                tf.muhasebeKod = "empty";

            if (!chk2Duzey.Checked)
                TasinirIslemTarihceRaporu(tf, kriter);
            else
                TasinirIslemTarihceRaporu2Duzey(tf, kriter);
        }

        /// <summary>
        /// Parametre olarak verilen iþlem türleri listesindeki her bir türün kullanýldýðý
        /// iþlem tiplerini bulur ve iþlem tipi kodlarýný ; ayracý ile birleþtirerek döndürür.
        /// </summary>
        /// <param name="tur">Ýþlem türleri dizisi</param>
        /// <returns>; ayracý ile birleþtirilmiþ iþlem tipi kodlarý döndürülür.</returns>
        private string TipListesiVer(int[] tur)
        {
            string tipler = "";
            ObjectArray turTip = new ObjectArray();
            IslemTip islemTip = new IslemTip();

            for (int i = 0; i < tur.Length; i++)
            {
                islemTip.tur = tur[i];
                turTip = servisTMM.IslemTipListele(kullanan, islemTip);
                foreach (IslemTip iT in turTip.objeler)
                {
                    if (tipler != "") tipler += ";";
                    tipler += iT.kod;
                }
            }

            return tipler;
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="tf">Taþýnýr iþlem fiþi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taþýnýr iþlem fiþi detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu2Duzey(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, tfKriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
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
            string sablonAd = "TasinirHurdaRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                string eskiHesap = string.Empty;
                decimal miktarToplam2Duzey = 0;
                decimal tutarToplam2Duzey = 0;
                decimal miktar = 0;
                decimal tutar = 0;
                int sayac = 0;
                int sonSatirdongu = 1;

                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    sayac++;

                    if ((!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod.Substring(0, 9)) || sayac == tif.detay.objeler.Count)
                    {
                        if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod.Substring(0, 9) && sayac == tif.detay.objeler.Count)
                            sonSatirdongu = 2;

                        for (int i = 0; i < sonSatirdongu; i++)
                        {
                            satir++;

                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);

                            XLS.HucreDegerYaz(satir, sutun, tif.muhasebeKod + " - " + tif.muhasebeAd);
                            XLS.HucreDegerYaz(satir, sutun + 1, tif.harcamaKod + " - " + tif.harcamaAd);
                            XLS.HucreDegerYaz(satir, sutun + 2, tif.ambarKod + " - " + tif.ambarAd);
                            XLS.HucreDegerYaz(satir, sutun + 3, tif.fisNo);
                            XLS.HucreDegerYaz(satir, sutun + 4, tif.fisTarih.ToString());
                            XLS.HucreDegerYaz(satir, sutun + 5, tif.islemTipAd);

                            XLS.HucreDegerYaz(satir, sutun + 6, tif.nereyeGitti);

                            if (sayac == tif.detay.objeler.Count)
                            {
                                //if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                                //    miktar = detay.miktar;
                                //else
                                //    miktar = -detay.miktar;
                                miktar = detay.miktar;

                                tutar = miktar * detay.birimFiyatKDVLi;

                                if (eskiHesap == detay.hesapPlanKod.Substring(0, 9))
                                {
                                    miktarToplam2Duzey += miktar;
                                    tutarToplam2Duzey += tutar;
                                }
                                else if (i == 1 || string.IsNullOrEmpty(eskiHesap))
                                {
                                    miktarToplam2Duzey = miktar;
                                    tutarToplam2Duzey = tutar;

                                    eskiHesap = detay.hesapPlanKod.Substring(0, 9);
                                }
                            }

                            XLS.HucreDegerYaz(satir, sutun + 7, eskiHesap);
                            XLS.HucreDegerYaz(satir, sutun + 8, servisUZY.UzayDegeriStr(null, "TASHESAPPLAN", eskiHesap, true));
                            XLS.HucreDegerYaz(satir, sutun + 9, servisUZY.UzayDegeriStr(null, "TASOLCUBIRIMAD", eskiHesap.Replace(".", ""), true));

                            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(miktarToplam2Duzey.ToString(), (double)0));
                            XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(tutarToplam2Duzey.ToString(), (double)0));

                            if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 13, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                                XLS.HucreDegerYaz(satir, sutun + 14, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                                XLS.HucreDegerYaz(satir, sutun + 15, tif.gAmbarKod + " - " + tif.gAmbarAd);
                            }

                            miktarToplam += miktarToplam2Duzey;
                            tutarToplam += tutarToplam2Duzey;

                            miktarToplam2Duzey = 0;
                            tutarToplam2Duzey = 0;
                        }
                    }

                    if (sayac != tif.detay.objeler.Count)
                    {
                        //if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                        //    miktar = detay.miktar;
                        //else
                        //    miktar = -detay.miktar;
                        miktar = detay.miktar;

                        tutar = miktar * detay.birimFiyatKDVLi;

                        miktarToplam2Duzey += miktar;
                        tutarToplam2Duzey += tutar;

                        eskiHesap = detay.hesapPlanKod.Substring(0, 9);
                    }
                }
            }

            //Toplamlar yazýlýyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 11);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTHU002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 12, true);
            XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 13, true);

            XLS.SutunGizle(sutun + 10, sutun + 11, true);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="tf">Taþýnýr iþlem fiþi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taþýnýr iþlem fiþi detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, tfKriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
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
            string sablonAd = "TasinirHurdaRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, tif.muhasebeKod + " - " + tif.muhasebeAd);
                    XLS.HucreDegerYaz(satir, sutun + 1, tif.harcamaKod + " - " + tif.harcamaAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, tif.ambarKod + " - " + tif.ambarAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, tif.fisNo);
                    XLS.HucreDegerYaz(satir, sutun + 4, tif.fisTarih.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, tif.islemTipAd);

                    XLS.HucreDegerYaz(satir, sutun + 6, tif.nereyeGitti);

                    XLS.HucreDegerYaz(satir, sutun + 7, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 8, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 9, detay.olcuBirimAd);

                    //decimal miktar = 0;
                    //if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                    //    miktar = detay.miktar;
                    //else
                    //    miktar = -detay.miktar;

                    decimal miktar = detay.miktar;

                    decimal tutar = miktar * detay.birimFiyatKDVLi;

                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyat.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(miktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));

                    if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 13, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                        XLS.HucreDegerYaz(satir, sutun + 14, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                        XLS.HucreDegerYaz(satir, sutun + 15, tif.gAmbarKod + " - " + tif.gAmbarAd);
                    }

                    miktarToplam += miktar;
                    tutarToplam += tutar;
                }
            }

            //Toplamlar yazýlýyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 11);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTHU002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 12, true);
            XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 13, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}