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
    public partial class PersonelRapor : TMMSayfaV2
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
                formAdi = "Personel Rapor";
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
            PersonelListesi(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden muhasebat kontrol raporu kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Personel  raporu kriter bilgileri döndürülür.</returns>
        public Personel KriterTopla()
        {
            TNS.TMM.Personel personel = new TNS.TMM.Personel();
            personel.muhasebeKod = txtMuhasebe.Text;
            personel.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            personel.kod = txtKod.Text;
            personel.ad = txtAdi.Text;
            personel.oda = txtOda.Text;

            return personel;
        }

        /// <summary>
        /// Parametre olarak verilen muhasebat kontrol raporu kriterlerini sunucudaki ilgili yordama
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Muhasebat kontrol raporu kriter bilgilerini tutan nesne</param>
        private void PersonelListesi(Personel kriter)
        {
            ObjectArray bilgi = servisTMM.PersonelListele(kullanan, kriter);

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
            int siraNo = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "PersonelListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (Personel p in bilgi.objeler)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);

                siraNo++;

                XLS.HucreDegerYaz(satir, 0, siraNo);
                XLS.HucreDegerYaz(satir, 1, p.il);
                XLS.HucreDegerYaz(satir, 2, p.ilce);
                XLS.HucreDegerYaz(satir, 3, p.muhasebeKod);
                XLS.HucreDegerYaz(satir, 4, p.muhasebeAd);
                XLS.HucreDegerYaz(satir, 5, p.harcamaBirimKod);
                XLS.HucreDegerYaz(satir, 6, p.harcamaBirimAd);
                XLS.HucreDegerYaz(satir, 7, p.kod);
                XLS.HucreDegerYaz(satir, 8, p.ad);
                XLS.HucreDegerYaz(satir, 9, p.unvan);
                XLS.HucreDegerYaz(satir, 10, p.gorev);
                XLS.HucreDegerYaz(satir, 11, p.telefon);
                XLS.HucreDegerYaz(satir, 12, p.eposta);
                XLS.HucreDegerYaz(satir, 13, p.faks);
                XLS.HucreDegerYaz(satir, 14, p.oda);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}