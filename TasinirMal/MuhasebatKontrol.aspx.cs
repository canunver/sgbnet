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
    public partial class MuhasebatKontrol : TMMSayfaV2
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
                formAdi = Resources.TasinirMal.FRMMHK001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                txtYil.Value = DateTime.Now.Year;
                DuzeyDoldur();
            }
        }

        /// <summary>
        /// Taşınır hesap planı kodunun maskesini bulur, parçalar ve ddlDuzey DropDownList kontrolüne seviye olarak doldurur.
        /// </summary>
        private void DuzeyDoldur()
        {
            List<object> liste = new List<object>();

            string hesapKodYapi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            if (!string.IsNullOrEmpty(hesapKodYapi))
            {
                string[] parcali = hesapKodYapi.Split('.');
                for (int i = 0; i < parcali.Length; i++)
                {
                    liste.Add(new { KOD = ((i + 1) * 2 + 1).ToString(), ADI = string.Format(Resources.TasinirMal.FRMMHK002, (i + 1).ToString()) });
                }
            }

            strDuzey.DataSource = liste;
            strDuzey.DataBind();
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
            MuhasebatKontrolRaporu(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden muhasebat kontrol raporu kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Muhasebat kontrol raporu kriter bilgileri döndürülür.</returns>
        private TNS.TMM.MuhasebatKontrol KriterTopla()
        {
            TNS.TMM.MuhasebatKontrol kriter = new TNS.TMM.MuhasebatKontrol();

            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kriter.hesapKod = txtHesapPlanKod.Text.Trim();
            kriter.hesapKodUzunluk = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlDuzey), 0);

            if (rdMuhasebeBazinda.Checked)
                kriter.raporTur = 0;
            if (rdHarcamaBazinda.Checked)
                kriter.raporTur = 1;
            if (rdMuhasebeHarcamaBazinda.Checked)
                kriter.raporTur = 5;

            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen muhasebat kontrol raporu kriterlerini sunucudaki ilgili yordama
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Muhasebat kontrol raporu kriter bilgilerini tutan nesne</param>
        private void MuhasebatKontrolRaporu(TNS.TMM.MuhasebatKontrol kriter)
        {
            ObjectArray bilgi = servisTMM.MuhasebatKontrolRaporu(kullanan, kriter);

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
            string sablonAd = "MuhasebatKontrolRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (TNS.TMM.MuhasebatKontrol mk in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, mk.muhasebeKod);
                XLS.HucreDegerYaz(satir, sutun + 1, mk.harcamaKod);
                XLS.HucreDegerYaz(satir, sutun + 2, mk.hesapKod);
                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(mk.girenTutar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(mk.cikanTutar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(mk.yilBasindaGirenTutar.ToString(), (double)0));
                decimal kalan = mk.girenTutar - mk.cikanTutar + mk.yilBasindaGirenTutar;
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(kalan.ToString(), (double)0));
            }

            if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                XLS.SutunGizle(sutun + 1, sutun + 1, true);
            else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                XLS.SutunGizle(sutun, sutun, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}