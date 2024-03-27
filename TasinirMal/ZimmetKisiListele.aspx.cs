using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.KYM;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Kiþiye verilmiþ zimmetli demirbaþ bilgilerinin listeme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetKisiListele : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Sayfa adresinde gelen TCKimlik girdi dizgisi dolu ise zimmet listeleme yapýlýr, yoksa sayfa boþ açýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (GenelIslemlerIstemci.SifresizBolumYok())
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            else
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);

            if (!X.IsAjaxRequest)
            {
                formAdi = "Üzerimdeki Zimmetli Malzemeler";
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                hdnKimlikNo.Value = kullanan.mernis;

                ZimmetKisiGrideYaz();
            }
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetKisi yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini sayfadaki gvZimmet GridView kontrolüne doldurur.
        /// </summary>
        [DirectMethod]
        public void ZimmetKisiGrideYaz()
        {
            TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new TNS.TMM.ZimmetOrtakAlanVeKisi();
            kriter.tcKimlikNo = hdnKimlikNo.Text;
            if (string.IsNullOrEmpty(kriter.tcKimlikNo))
            {
                X.AddScript("KimlikNoAl();");
                return;
            }

            ObjectArray bilgi = servisTMM.ZimmetKisi(new Kullanici(), kriter, false);

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
            if (bilgi.sonuc.islemSonuc)
            {
                List<object> liste = new List<object>();
                TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
                for (int i = 0; i < zoa.detay.Count; i++)
                {
                    ZimmetOrtakAlanVeKisiDetay detay = (ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];
                    string fisTarih = detay.fisTarih.ToString();
                    liste.Add(new
                    {
                        MUHASEBE = detay.muhasebeKod + " - " + detay.muhasebeAd,
                        HARCAMABIRIMI = detay.harcamaKod + " - " + detay.harcamaAd,
                        AMBAR = detay.ambarKod + " - " + detay.ambarAd,
                        SICILNO = detay.gorSicilNo,
                        MALZEMEADI = detay.sicilAd,
                        FISTARIHI = fisTarih
                    });
                }
                strListe.DataSource = liste;
                strListe.DataBind();
            }
        }

        protected void btnRaporYazdir_Click(object sender, EventArgs e)
        {
            ZimmetOrtakAlanVeKisi kriter = new ZimmetOrtakAlanVeKisi();
            kriter.tcKimlikNo = kullanan.mernis;
            ObjectArray bilgi = servisTMM.ZimmetKisi(kullanan, kriter, false);

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
            string sablonAd = "ZimmetKisi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            //XLS.HucreAdBulYaz("HarcamaAd", zoa.harcamaAd);
            //XLS.HucreAdBulYaz("HarcamaKod", zoa.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", zoa.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", zoa.muhasebeKod);
            XLS.HucreAdBulYaz("KisiAd", zoa.kisiAd);
            XLS.HucreAdBulYaz("KisiKod", zoa.tcKimlikNo);

            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 8, satir, sutun + 9);

                XLS.HucreDegerYaz(satir, sutun, detay.harcamaKod + " - " + detay.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 1, detay.ambarKod + " - " + detay.ambarAd);

                if (TasinirGenel.rfIdVarMi)
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo + " - " + detay.rfIdNo);
                else
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo);

                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilAd);
                XLS.HucreDegerYaz(satir, sutun + 5, detay.odaAd);//zoa.odaKod + "-" +
                XLS.HucreDegerYaz(satir, sutun + 6, detay.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 7, detay.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, detay.aciklama);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "Zimmet_" + kriter.tcKimlikNo, true, GenelIslemler.ExcelTur());
        }
    }
}