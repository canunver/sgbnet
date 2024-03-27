using System;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.DEG;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Muhasebat kontrol raporunun excel dosyasına yazılarak hazırlandığı sayfa
    /// </summary>
    public partial class MuhasebatKontrolEski : TMMSayfa
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Degisken.xml dosyasındaki değişkenlere ulaşan servis
        /// </summary>
        IDEGServis servisDEG = TNS.DEG.Arac.Tanimla();

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
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMMHK001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                YilDoldur();
                DuzeyDoldur();
                RaporTurDoldur();

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtHesapPlanKod.Text.Trim() != "")
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = "";
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yıl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Taşınır hesap planı kodunun maskesini bulur, parçalar ve ddlDuzey DropDownList kontrolüne seviye olarak doldurur.
        /// </summary>
        private void DuzeyDoldur()
        {
            string hesapKodYapi = servisDEG.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            if (!string.IsNullOrEmpty(hesapKodYapi))
            {
                string[] parcali = hesapKodYapi.Split('.');
                for (int i = 0; i < parcali.Length; i++)
                {
                    ddlDuzey.Items.Add(new ListItem(string.Format(Resources.TasinirMal.FRMMHK002, (i + 1).ToString()), ((i + 1) * 2 + 1).ToString()));
                }
            }
        }

        /// <summary>
        /// Sayfadaki rblRaporTur RadioButtonList kontrolüne rapor türleri doldurulur.
        /// </summary>
        private void RaporTurDoldur()
        {
            rblRaporTur.Items.Add(new ListItem(Resources.TasinirMal.FRMMHK003, ((int)ENUMDepoDurumRaporTur.MUHASEBE).ToString()));
            rblRaporTur.Items.Add(new ListItem(Resources.TasinirMal.FRMMHK004, ((int)ENUMDepoDurumRaporTur.HARCAMA).ToString()));
            rblRaporTur.Items.Add(new ListItem(Resources.TasinirMal.FRMMHK005, ((int)ENUMDepoDurumRaporTur.MUHASEBE_HARCAMA).ToString()));
            rblRaporTur.Items[0].Selected = true;
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

            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kriter.hesapKod = txtHesapPlanKod.Text.Trim();
            kriter.hesapKodUzunluk = OrtakFonksiyonlar.ConvertToInt(ddlDuzey.SelectedValue.Trim(), 0);
            kriter.raporTur = OrtakFonksiyonlar.ConvertToInt(rblRaporTur.SelectedValue.Trim(), 0);

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