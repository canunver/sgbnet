using System;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using TNS.UZY;
using Ext1.Net;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r i�lem fi�i malzeme bilgilerinin raporlama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TasinirIslemMalzemeTarihceRaporu : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["kutuk"]))
                {
                    chk2Duzey.Visible = false;
                    formAdi = Resources.TasinirMal.FRMTMT029;
                }
                else
                    formAdi = Resources.TasinirMal.FRMTMT001;

                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnFirmaHarcamadanAlma.Value = TasinirGenel.tasinirFirmaBilgisiniHarcamadanAlma;

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
                IslemTipiDoldur();
            }
        }

        /// <summary>
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Ekrandan kriter bilgileri toplan�r ve toplanan kriterler chk2Duzey CheckBox kontrol�
        /// i�aretliyse TasinirIslemTarihceRaporu2Duzey yordam�na, i�aretli de�ilse TasinirIslemTarihceRaporu
        /// yordam�na g�nderilir, b�ylece excel raporu �retilip kullan�c�ya g�nderilmi� olur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlIslemTipi), 0);
            tf.gMuhasebeKod = txtGonMuhasebe.Text.Trim();
            tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Trim();
            tf.gAmbarKod = txtGonAmbar.Text.Trim();
            tf.kimeGitti = txtKimeGitti.Text.Trim();
            tf.neredenGeldi = txtNeredenGeldi.Text.Trim();
            tf.nereyeGitti = txtNereyeGitti.Text.Trim();

            TNS.TMM.TasinirFormKriter kriter = new TNS.TMM.TasinirFormKriter();
            kriter.belgeTarihBasla = new TNSDateTime(txtTarih1.RawText);
            kriter.belgeTarihBit = new TNSDateTime(txtTarih2.RawText);
            kriter.hesapKodu = txtHesapPlanKod.Text.Trim();

            if (!string.IsNullOrEmpty(Request.QueryString["kutuk"]))
                BelgeKayitKutuguRaporu(tf, kriter);
            else
            {
                if (!chk2Duzey.Checked)
                    TasinirIslemTarihceRaporu(tf, kriter);
                else
                    TasinirIslemTarihceRaporu2Duzey(tf, kriter);
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordam�na
        /// g�nderir, sunucudan gelen bilgi k�mesini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// Belge Kay�t K�t��� rapor format�nda ��kt� �retir.
        /// </summary>
        /// <param name="tf">Ta��n�r i�lem fi�i �st kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ta��n�r i�lem fi�i detay kriter bilgilerini tutan nesne</param>
        private void BelgeKayitKutuguRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            if (string.IsNullOrEmpty(tf.muhasebeKod) || string.IsNullOrEmpty(tf.harcamaKod))
            {
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTMT026);
                return;
            }

            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, tfKriter);

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
            string sablonAd = "BelgeKayitKutugu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            TNS.TMM.TasinirIslemForm tifIlk = (TNS.TMM.TasinirIslemForm)bilgi.objeler[0];
            XLS.HucreAdBulYaz("Muhasebe", tifIlk.muhasebeKod + " - " + tifIlk.muhasebeAd + " / " + tifIlk.harcamaKod + " - " + tifIlk.harcamaAd);
            XLS.HucreAdBulYaz("Yil", tifIlk.yil);

            satir = kaynakSatir;

            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, tif.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 1, tif.fisTarih.ToString());

                XLS.HucreDegerYaz(satir, sutun + 2, tif.faturaNo);
                XLS.HucreDegerYaz(satir, sutun + 3, tif.faturaTarih.ToString());

                XLS.HucreDegerYaz(satir, sutun + 4, tif.islemTipAd);

                string girisCikisNeden;
                if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                    girisCikisNeden = string.Format(Resources.TasinirMal.FRMTMT027, tif.islemTipAd);
                else
                    girisCikisNeden = string.Format(Resources.TasinirMal.FRMTMT028, tif.islemTipAd);
                XLS.HucreDegerYaz(satir, sutun + 5, girisCikisNeden);

                if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                    tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS ||
                    tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS ||
                    tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    XLS.HucreDegerYaz(satir, sutun + 10, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd + " / " + tif.gHarcamaKod + " - " + tif.gHarcamaAd);

                decimal miktar = 0;
                string aciklamalar = string.Empty;
                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    miktar += detay.miktar;
                    aciklamalar += detay.eAciklama;
                }
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, 0);
                XLS.HucreDegerYaz(satir, sutun + 8, 0);
                XLS.HucreDegerYaz(satir, sutun + 9,tif.neredenGeldi );
                XLS.HucreDegerYaz(satir, sutun + 10, aciklamalar);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordam�na
        /// g�nderir, sunucudan gelen bilgi k�mesini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="tf">Ta��n�r i�lem fi�i �st kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ta��n�r i�lem fi�i detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu2Duzey(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, tfKriter);

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
            string sablonAd = "TasinirIslemMalzemeTarihce.xlt";
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

                            if (sayac == tif.detay.objeler.Count)
                            {
                                if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                                    miktar = detay.miktar;
                                else
                                    miktar = -detay.miktar;

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

                            XLS.HucreDegerYaz(satir, sutun + 6, eskiHesap);
                            XLS.HucreDegerYaz(satir, sutun + 7, servisUZY.UzayDegeriStr(null, "TASHESAPPLAN", eskiHesap, true));
                            XLS.HucreDegerYaz(satir, sutun + 8, servisUZY.UzayDegeriStr(null, "TASOLCUBIRIMAD", eskiHesap.Replace(".", ""), true));

                            //XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyat.ToString(), (double)0));
                            //XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktarToplam2Duzey.ToString(), (double)0));
                            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutarToplam2Duzey.ToString(), (double)0));

                            if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 13, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                                XLS.HucreDegerYaz(satir, sutun + 14, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                                XLS.HucreDegerYaz(satir, sutun + 15, tif.gAmbarKod + " - " + tif.gAmbarAd);
                            }

                            XLS.HucreDegerYaz(satir, sutun + 16, tif.neredenGeldi);

                            miktarToplam += miktarToplam2Duzey;
                            tutarToplam += tutarToplam2Duzey;

                            miktarToplam2Duzey = 0;
                            tutarToplam2Duzey = 0;
                        }
                    }

                    if (sayac != tif.detay.objeler.Count)
                    {
                        if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                            miktar = detay.miktar;
                        else
                            miktar = -detay.miktar;

                        tutar = miktar * detay.birimFiyatKDVLi;

                        miktarToplam2Duzey += miktar;
                        tutarToplam2Duzey += tutar;

                        eskiHesap = detay.hesapPlanKod.Substring(0, 9);
                    }
                }
            }

            //Toplamlar yaz�l�yor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 10);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTMT002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 11, true);
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 12, true);

            XLS.SutunGizle(sutun + 9, sutun + 10, true);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordam�na
        /// g�nderir, sunucudan gelen bilgi k�mesini excel dosyas�na yazar ve kullan�c�ya g�nderir.
        /// </summary>
        /// <param name="tf">Ta��n�r i�lem fi�i �st kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ta��n�r i�lem fi�i detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, tfKriter);

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
            string sablonAd = "TasinirIslemMalzemeTarihce.xlt";
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

                    XLS.HucreDegerYaz(satir, sutun + 6, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 7, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 8, detay.olcuBirimAd);

                    decimal miktar = 0;
                    if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                        miktar = detay.miktar;
                    else
                        miktar = -detay.miktar;

                    decimal tutar = miktar * detay.birimFiyatKDVLi;

                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyat.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));

                    if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 13, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                        XLS.HucreDegerYaz(satir, sutun + 14, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                        XLS.HucreDegerYaz(satir, sutun + 15, tif.gAmbarKod + " - " + tif.gAmbarAd);
                    }

                    XLS.HucreDegerYaz(satir, sutun + 16, tif.neredenGeldi);

                    miktarToplam += miktar;
                    tutarToplam += tutar;
                }
            }

            //Toplamlar yaz�l�yor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 10);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTMT002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 11, true);
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 12, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// ��lem tipi bilgileri sunucudan �ekilir ve ddlIslemTipi DropDownList kontrol�ne doldurulur.
        /// </summary>
        private void IslemTipiDoldur()
        {
            ObjectArray bilgi = servisTMM.IslemTipListele(kullanan, new IslemTip());

            List<object> liste = new List<object>();
            liste.Add(new { KOD = 0, ADI = Resources.TasinirMal.FRMTMT003 });

            foreach (IslemTip tip in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = tip.kod,
                    ADI = tip.ad
                });
            }

            strIslemTipi.DataSource = liste;
            strIslemTipi.DataBind();
        }

    }
}