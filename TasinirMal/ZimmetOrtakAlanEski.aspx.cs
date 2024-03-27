using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ortak alana verilmiþ zimmetli demirbaþ bilgilerinin raporlama iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetOrtakAlanEski : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

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
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMZOA001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtNereyeGitti.Attributes.Add("onblur", "kodAdGetir('35','lblNereyeGittiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtNereyeGitti'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                YilDoldur();
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtNereyeGitti.Text.Trim() != "")
                lblNereyeGittiAd.Text = GenelIslemler.KodAd(35, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtNereyeGitti.Text.Trim(), true);
            else
                lblNereyeGittiAd.Text = "";
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler
        /// ortak alana verilmiþ zimmetli demirbaþlar raporunu üreten ZimmetOrtakAlanYazdir yordamýna
        /// gönderilir, böylece excel raporu üretilip kullanýcýya gönderilmiþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            ZimmetOrtakAlanYazdir(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden zimmet listeleme kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Zimmet listeleme kriter bilgileri döndürülür.</returns>
        private TNS.TMM.ZimmetOrtakAlanVeKisi KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new TNS.TMM.ZimmetOrtakAlanVeKisi();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.odaKod = txtNereyeGitti.Text.Trim();
            if (chkKisiDahilEt.Checked)
                kriter.kisiDahilEt = 1;

            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetOrtakAlan yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetOrtakAlanYazdir(TNS.TMM.ZimmetOrtakAlanVeKisi kriter)
        {
            ObjectArray bilgi = servisTMM.ZimmetOrtakAlan(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZimmetOrtakAlan.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            XLS.HucreAdBulYaz("HarcamaAd", zoa.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", zoa.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", zoa.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", zoa.muhasebeKod);
            //XLS.HucreAdBulYaz("OdaAd", zoa.odaAd);
            //XLS.HucreAdBulYaz("OdaKod", zoa.odaKod);
            XLS.HucreAdBulYaz("OdaAd", lblNereyeGittiAd.Text.Trim());
            XLS.HucreAdBulYaz("OdaKod", txtNereyeGitti.Text.Trim());

            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 8);

                XLS.HucreDegerYaz(satir, sutun, detay.harcamaKod + " - " + detay.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 1, detay.ambarKod + " - " + detay.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilAd);
                XLS.HucreDegerYaz(satir, sutun + 5, detay.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 6, detay.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 7, detay.sorumluKisiAd);
                XLS.HucreDegerYaz(satir, sutun + 9, detay.aciklama);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}