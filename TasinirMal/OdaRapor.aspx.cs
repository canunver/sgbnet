using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.DEG;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Muhasebat kontrol raporunun excel dosyasına yazılarak hazırlandığı sayfa
    /// </summary>
    public partial class OdaRapor : TMMSayfaV2
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
                formAdi = "Oda Rapor";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

            }
        }

        /// <summary>
        /// Yazdır tuşuna basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çağırılır ve toplanan kriterler
        /// muhasebat kontrol raporunu üreten MuhasebatKontrolRaporu yordamına
        /// gönderilir, böylece excel raporu üretilip kullanıcıya gönderilmiş olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            OdaListesi(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden muhasebat kontrol raporu kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Personel  raporu kriter bilgileri döndürülür.</returns>
        public Oda KriterTopla()
        {
            TNS.TMM.Oda kriter = new TNS.TMM.Oda();
            kriter.muhasebeKod = txtMuhasebe.Text;
            kriter.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            kriter.kod = txtOda.Text;

            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen muhasebat kontrol raporu kriterlerini sunucudaki ilgili yordama
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Muhasebat kontrol raporu kriter bilgilerini tutan nesne</param>
        private void OdaListesi(Oda kriter)
        {
            ObjectArray bilgi = servisTMM.OdaListele(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
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
            string sablonAd = "OdaListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            kaynakSatir = satir;
            int siraNo = 0;

            foreach (Oda oda in bilgi.objeler)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);

                siraNo++;

                XLS.HucreDegerYaz(satir, 0, siraNo);
                XLS.HucreDegerYaz(satir, 1, oda.il);
                XLS.HucreDegerYaz(satir, 2, oda.ilce);
                XLS.HucreDegerYaz(satir, 3, oda.muhasebeKod);
                XLS.HucreDegerYaz(satir, 4, oda.muhasebeAd);
                XLS.HucreDegerYaz(satir, 5, oda.harcamaBirimKod);
                XLS.HucreDegerYaz(satir, 6, oda.harcamaBirimAd);
                XLS.HucreDegerYaz(satir, 7, oda.ambarKod);
                XLS.HucreDegerYaz(satir, 8, oda.ambarAd);
                XLS.HucreDegerYaz(satir, 9, oda.kod);
                XLS.HucreDegerYaz(satir, 10, oda.ad);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}