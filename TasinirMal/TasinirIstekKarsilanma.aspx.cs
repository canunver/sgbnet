using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Taşınır istenilen-karşılan bilgilerinin raporlama işleminin yapıldığı sayfa
    /// </summary>
    public partial class TasinirIstekKarsilanma : TMMSayfa
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
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMIKF001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtKimeGitti.Attributes.Add("onblur", "kodAdGetir('36','lblKimeGittiAd',true,new Array('txtKimeGitti'),'KONTROLDENOKU');");

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

            if (txtKimeGitti.Text.Trim() != "")
                lblKimeGittiAd.Text = GenelIslemler.KodAd(36, txtKimeGitti.Text.Trim(), true);
            else
                lblKimeGittiAd.Text = "";
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yıl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Yazdır tuşuna basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çağırılır ve toplanan kriterler
        /// taşınır istenilen-karşılan raporunu üreten IstekKarsilananYazdir yordamına
        /// gönderilir, böylece excel raporu üretilip kullanıcıya gönderilmiş olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            IstekKarsilananYazdir(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden istek form kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>İstek form kriter bilgileri döndürülür.</returns>
        private IstekForm KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            IstekForm kriter = new TNS.TMM.IstekForm();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.istekYapanKod = txtKimeGitti.Text.Trim();
            kriter.baslaTarih = new TNSDateTime(txtBelgeTarih1.Text.Trim());
            kriter.bitTarih = new TNSDateTime(txtBelgeTarih2.Text.Trim());

            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen istek form kriterlerini sunucudaki IstekKarsilananRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void IstekKarsilananYazdir(IstekForm kriter)
        {
            ObjectArray bilgi = servisTMM.IstekKarsilananRaporu(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMIKF002);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "IstekKarsilanan.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            IstekDetay id = (IstekDetay)bilgi.objeler[0];

            string tarihBilgisi = string.Empty;

            int yilSatir = 0;
            int yilSutun = 0;

            XLS.HucreAdAdresCoz("Yil", ref yilSatir, ref yilSutun);
            XLS.YaziTipBuyuklugu(yilSatir, yilSutun, yilSatir, yilSutun, 8);

            if (!string.IsNullOrEmpty(txtBelgeTarih1.Text) || !string.IsNullOrEmpty(txtBelgeTarih2.Text))
            {
                tarihBilgisi = "(" + txtBelgeTarih1.Text + "-" + txtBelgeTarih2.Text + ")";
                XLS.HucreDegerYaz(yilSatir, yilSutun - 1, Resources.TasinirMal.FRMIKF003);
            }
            else
                tarihBilgisi = id.yil.ToString();

            XLS.YatayHizala(yilSatir, yilSutun, 2);
            XLS.DuseyHizala(yilSatir, yilSutun, 2);

            XLS.HucreAdBulYaz("Yil", tarihBilgisi);
            XLS.HucreAdBulYaz("HarcamaAd", id.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", id.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", id.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", id.muhasebeKod);

            if (!string.IsNullOrEmpty(txtKimeGitti.Text))
            {
                XLS.HucreAdBulYaz("KisiAd", id.istekYapanAd);
                XLS.HucreAdBulYaz("KisiKod", id.istekYapanKod);
            }

            foreach (IstekDetay idGelen in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                //XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                //XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 8);

                XLS.HucreDegerYaz(satir, sutun, idGelen.istekYapanKod);
                XLS.HucreDegerYaz(satir, sutun + 1, idGelen.istekYapanAd);
                XLS.HucreDegerYaz(satir, sutun + 2, idGelen.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 3, idGelen.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(idGelen.istenilenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(idGelen.karsilananMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(idGelen.tuketilenMiktar.ToString(), (double)0));
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
    }
}