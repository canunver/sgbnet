using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Web.UI.WebControls;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr demirbaþlarýnýn son durum bilgilerinin raporlama iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirSicilRaporuEski : TMMSayfa
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

            formAdi = Resources.TasinirMal.FRMTSR001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");
            this.txtKimeGitti.Attributes.Add("onblur", "kodAdGetir('36','lblKimeGittiAd',true,new Array('txtKimeGitti'),'KONTROLDENOKU');");
            this.txtNereyeGitti.Attributes.Add("onblur", "kodAdGetir('35','lblNereyeGittiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtNereyeGitti'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                IslemTipiDoldur();
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtAmbar.Text.Trim() != "")
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = "";

            if (txtHesapPlanKod.Text.Trim() != "")
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = "";
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler
        /// taþýnýr demirbaþ son durum raporunu üreten SicilRaporu yordamýna
        /// gönderilir, böylece excel raporu üretilip kullanýcýya gönderilmiþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            SicilRaporu(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden demirbaþ listeleme kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Demirbaþ listeleme kriter bilgileri döndürülür.</returns>
        private SicilNoHareket KriterTopla()
        {
            SicilNoHareket kriter = new SicilNoHareket();
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaBirimKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.kimeGitti = txtKimeGitti.Text.Trim();
            kriter.nereyeGitti = txtNereyeGitti.Text.Trim();
            kriter.fisTarih = new TNSDateTime(txtTarih1.Value.Trim());
            kriter.islemTarih = new TNSDateTime(txtTarih2.Value.Trim());
            kriter.fiyat = OrtakFonksiyonlar.ConvertToDecimal(txtBirimFiyat.Text.Trim(), (decimal)0);
            kriter.islemTipKod = OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue, 0);
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaþ listeleme kriterlerini sunucudaki SicilRaporu yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void SicilRaporu(SicilNoHareket kriter)
        {
            ObjectArray bilgi = servisTMM.SicilRaporu(kullanan, kriter);

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
            string sablonAd = "TasinirSicilRaporu.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (SicilNoHareket snh in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                if (TasinirGenel.tasinirSicilNoRFIDFarkli)
                    XLS.HucreDegerYaz(satir, sutun, snh.sicilNo + " -" + snh.rfIdNo.ToString());
                else
                    XLS.HucreDegerYaz(satir, sutun, snh.sicilNo);
                XLS.HucreDegerYaz(satir, sutun + 1, snh.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 2, snh.kdvOran);
                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(snh.fiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(snh.kdvliBirimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 5, snh.ilkGirisIslemTip.ad);
                XLS.HucreDegerYaz(satir, sutun + 6, snh.ilkGirisTarih.ToString());

                if (!string.IsNullOrEmpty(snh.islemTipiAd))
                    XLS.HucreDegerYaz(satir, sutun + 7, snh.islemTipiAd);
                else if (snh.islemTurKod == (int)ENUMIslemTipi.ZFVERME)
                    XLS.HucreDegerYaz(satir, sutun + 7, Resources.TasinirMal.FRMTSR002);
                else if (snh.islemTurKod == (int)ENUMIslemTipi.DTLVERME)
                    XLS.HucreDegerYaz(satir, sutun + 7, Resources.TasinirMal.FRMTSR003);
                else if (snh.islemTurKod == (int)ENUMIslemTipi.ZFDUSME)
                    XLS.HucreDegerYaz(satir, sutun + 7, Resources.TasinirMal.FRMTSR004);
                else if (snh.islemTurKod == (int)ENUMIslemTipi.DTLDUSME)
                    XLS.HucreDegerYaz(satir, sutun + 7, Resources.TasinirMal.FRMTSR005);

                XLS.HucreDegerYaz(satir, sutun + 8, snh.islemTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 9, snh.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 10, snh.harcamaBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 11, snh.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 12, snh.kisiAd);
                XLS.HucreDegerYaz(satir, sutun + 13, snh.odaAd);
                XLS.HucreDegerYaz(satir, sutun + 14, snh.zimmetOzellik);
                XLS.HucreDegerYaz(satir, sutun + 15, snh.ozellik.markaAd);
                XLS.HucreDegerYaz(satir, sutun + 16, snh.ozellik.modelAd);
                XLS.HucreDegerYaz(satir, sutun + 17, snh.ozellik.saseNo);
                XLS.HucreDegerYaz(satir, sutun + 18, snh.ozellik.motorNo);
                XLS.HucreDegerYaz(satir, sutun + 19, snh.ozellik.plaka);
                XLS.HucreDegerYaz(satir, sutun + 20, snh.ozellik.disSicilNo);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        private void IslemTipiDoldur()
        {
            ObjectArray bilgi = servisTMM.IslemTipListele(kullanan, new IslemTip());

            ddlIslemTipi.Items.Add(new ListItem("", ""));

            foreach (IslemTip it in bilgi.objeler)
            {
                if (it.tur > 49) continue;

                ddlIslemTipi.Items.Add(new ListItem(it.ad, it.kod.ToString()));
            }
        }
    }
}

