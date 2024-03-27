using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Kayýttan düþme teklif ve onay tutanaðý bilgilerinin kayýt, listeleme, silme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class KayittanDusmeGirisEski : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Gride eklenecek satýr sayýsý
        /// </summary>
        int ekleSatirSayisi = 30;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama ve tutanakNo girdi dizgileri dolu ise
        ///     ilgili kayýttan düþme teklif ve onay tutanaðý bilgileri listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMKDG001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            //Sayfaya giriþ izni varmý?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                GenelIslemler.SayfayaGirmesin(true);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");
            this.btnSil.Attributes.Add("onclick", "return OnayAl('Sil','btnSil');");

            //fpL iþlemlerini karþýlamak için
            //***********************************************
            if (Request.Form["__EVENTTARGET"] == "fpL")
            {
                string arg = Request.Form["__EVENTARGUMENT"] + "";
                fpL_ButtonCommand(arg);
            }

            if (!IsPostBack)
            {
                ViewState["fpID"] = DateTime.Now.ToLongTimeString();

                GridInit(fpL);
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
                txtTutanakTarihi.Value = DateTime.Now.ToShortDateString();

                if (!string.IsNullOrEmpty(Request.QueryString["yil"])
                     && !string.IsNullOrEmpty(Request.QueryString["muhasebe"])
                     && !string.IsNullOrEmpty(Request.QueryString["harcama"])
                     && !string.IsNullOrEmpty(Request.QueryString["tutanakNo"]))
                {
                    ddlYil.SelectedValue = Request.QueryString["yil"];
                    txtMuhasebe.Text = Request.QueryString["muhasebe"];
                    txtHarcamaBirimi.Text = Request.QueryString["harcama"];
                    hdnTutanakNo.Value = Request.QueryString["tutanakNo"];
                    txtAmbar.Text = string.Empty;

                    ButonlariAktifYap();
                    Listele(KriterTopla());
                }
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
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve
        /// toplanan kriterler Yazdir yordamýna gönderilir ve rapor hazýrlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            Yazdir(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen kayýttan düþme formuna ait kriterleri
        /// sunucudaki kayýttan düþme teklif ve onay tutanaðý listeleme yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="kf">Kayýttan düþme teklif ve onay tutanaðý kriter bilgilerini tutan nesne</param>
        private void Yazdir(KayittanDusmeForm kf)
        {
            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

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
            string sablonAd = "KAYITTANDUSMETUTANAK.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            //Satir eklenince adreslerin yeri kayýyor
            //Bu nedenle imza bilgileri önce yazýlýyor
            ImzaEkle(XLS);

            KayittanDusmeForm kform = (KayittanDusmeForm)bilgi.objeler[0];
            XLS.HucreAdBulYaz("TutanakNo", kform.tutanakNo);
            XLS.HucreAdBulYaz("IlAd", kform.ilAd + "-" + kform.ilceAd);
            XLS.HucreAdBulYaz("IlKod", kform.ilKod + "-" + kform.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", kform.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", kform.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", kform.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", kform.muhasebeKod);

            satir = kaynakSatir;

            decimal miktarToplam = 0;

            for (int i = 0; i < kform.detay.Count; i++)
            {
                KayittanDusmeDetay kd = (KayittanDusmeDetay)kform.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 3);
                XLS.HucreBirlestir(satir, sutun + 5, satir, sutun + 6);
                XLS.HucreBirlestir(satir, sutun + 8, satir, sutun + 9);
                XLS.HucreBirlestir(satir, sutun + 10, satir, sutun + 12);

                XLS.HucreDegerYaz(satir, sutun, i + 1);
                if (string.IsNullOrEmpty(kd.gorSicilNo))
                    XLS.HucreDegerYaz(satir, sutun + 1, kd.hesapPlanKod);
                else
                    XLS.HucreDegerYaz(satir, sutun + 1, kd.gorSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 4, kd.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, kd.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(kd.miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(kd.birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(kd.tutar.ToString(), (double)0));

                miktarToplam += kd.miktar;
            }

            XLS.HucreDegerYaz(satir + 3, sutun + 1, string.Format(Resources.TasinirMal.FRMKDG002, kform.detay.Count.ToString(), miktarToplam.ToString()));

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        private void ImzaEkle(Tablo XLS)
        {
            ObjectArray imzalar = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), 0);
            if (!imzalar.sonuc.islemSonuc)
                return;

            for (int i = 1; i <= 3; i++)
                XLS.HucreAdBulYaz("Tarih" + i.ToString(), DateTime.Today.Date.ToShortDateString());

            foreach (ImzaBilgisi imza in imzalar.objeler)
            {
                string adresSira = string.Empty;
                if (imza.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI)
                {
                    adresSira = "1";
                    XLS.HucreAdBulYaz("AdSoyad" + adresSira, imza.adSoyad);
                    XLS.HucreAdBulYaz("Unvan" + adresSira, imza.unvan);
                    adresSira = "3";
                }
                else if (imza.imzaYer == (int)ENUMImzaYer.KAYITTANDUSMEKOMISYONBASKANI)
                    adresSira = "2";
                else if (imza.imzaYer == (int)ENUMImzaYer.KAYITTANDUSMEKOMISYONUYESI)
                    adresSira = "4";
                else if (imza.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI)
                    adresSira = "5";
                else if (imza.imzaYer == (int)ENUMImzaYer.USTYONETICI)
                    adresSira = "6";

                XLS.HucreAdBulYaz("AdSoyad" + adresSira, imza.adSoyad);
                XLS.HucreAdBulYaz("Unvan" + adresSira, imza.unvan);
            }
        }

        /// <summary>
        /// Listeleme kriterleri kayýttan düþme form nesnesinde parametre olarak alýnýr,
        /// sunucuya gönderilir ve kayýttan düþme teklif ve onay tutanaðý bilgileri sunucudan
        /// alýnýr. Hata varsa ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="kf">Kayýttan düþme teklif ve onay tutanaðý listeleme kriterlerini tutan nesne</param>
        private void Listele(KayittanDusmeForm kf)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            btnTemizle_Click(null, null);

            KayittanDusmeForm kform = (KayittanDusmeForm)bilgi.objeler[0];
            ddlYil.SelectedValue = kform.yil.ToString();
            txtMuhasebe.Text = kform.muhasebeKod;
            lblMuhasebeAd.Text = kform.muhasebeAd;
            txtHarcamaBirimi.Text = kform.harcamaKod;
            lblHarcamaBirimiAd.Text = kform.harcamaAd;
            txtAmbar.Text = kform.ambarKod;
            lblAmbarAd.Text = kform.ambarAd;
            txtTutanakTarihi.Value = kform.tutanakTarihi.ToString();
            hdnTutanakNo.Value = kform.tutanakNo;

            if (kform.detay.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = kform.detay.Count + 10;

            int i = 0;
            foreach (KayittanDusmeDetay kd in kform.detay)
            {
                fpL.Sheets[0].Cells[i, 0].Text = kd.hesapPlanKod;
                fpL.Sheets[0].Cells[i, 2].Text = kd.gorSicilNo;
                fpL.Sheets[0].Cells[i, 4].Text = kd.miktar.ToString();
                fpL.Sheets[0].Cells[i, 5].Text = kd.olcuBirimAd;
                fpL.Sheets[0].Cells[i, 6].Text = kd.kdvOran.ToString();
                fpL.Sheets[0].Cells[i, 7].Text = kd.birimFiyat.ToString();
                fpL.Sheets[0].Cells[i, 8].Text = kd.hesapPlanAd;
                i++;
            }
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Kayýttan düþme teklif ve onay tutanaðý bilgilerini ekrandaki ilgili kontrollerden toplayan
        /// yordam çaðýrýlýr ve daha sonra toplanan bilgiler kaydedilmek üzere Kaydet yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
            Kaydet(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Kayýttan düþme teklif ve onay tutanaðý bilgilerini ekrandaki ilgili kontrollerden toplayan
        /// yordam çaðýrýlýr ve daha sonra toplanan bilgiler silinmek üzere Sil yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, EventArgs e)
        {
            Sil(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen kayýttan düþme teklif ve onay tutanaðýnýn bilgilerini
        /// silen sunucu yordamýný çaðýrýr ve dönen sonuca göre hata veya bilgi mesajý verir.
        /// </summary>
        /// <param name="kf">Kayýttan düþme teklif ve onay tutanaðý bilgilerini tutan nesne</param>
        private void Sil(KayittanDusmeForm kf)
        {
            Sonuc sonuc = servisTMM.KayittanDusmeSil(kullanan, kf);
            if (!sonuc.islemSonuc)
                GenelIslemler.HataYaz(this, sonuc.hataStr);
            else
            {
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMKDG003);
                btnTemizle_Click(null, null);
            }
        }

        /// <summary>
        /// Sayfadaki buton kontrollerini aktif hale getiren yordam
        /// </summary>
        private void ButonlariAktifYap()
        {
            btnSil.Enabled = true;
            btnYazdir.Enabled = true;
            btnTIF.Enabled = true;
        }

        /// <summary>
        /// TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Kayýttan düþme teklif ve onay tutanaðýnýn bilgilerini sunucudan
        /// alýr, sessiona yazar ve taþýnýr iþlem fiþi ekranýna yönlendirir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIF_Click(object sender, EventArgs e)
        {
            KayittanDusmeForm kf = KriterTopla();
            if (string.IsNullOrEmpty(kf.tutanakNo))
            {
                GenelIslemler.HataYaz(this, Resources.TasinirMal.FRMKDG004);
                return;
            }

            ObjectArray bilgi = servisTMM.KayittanDusmeListele(kullanan, kf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            //bool eksikVarMi = false;

            //SayimForm sForm = (SayimForm)bilgi.objeler[0];
            //foreach (SayimDetay sd in sForm.detay)
            //    if (sd.fazlaMiktar > 0 || sd.noksanMiktar > 0)
            //    {
            //        eksikVarMi = true;
            //        break;
            //    }

            //if (eksikVarMi)
            //{
            //    Session.Add("SayimdanTIF", sForm);
            //    Response.Redirect("TasinirIslemFormAna.aspx");
            //}
            //else
            //    GenelIslemler.HataYaz(this, "TÝF oluþturmak için sayýmdaki miktarlarýn, kayýtlardakinden farklý olmasý gerekir!");

            KayittanDusmeForm kForm = (KayittanDusmeForm)bilgi.objeler[0];
            Session.Add("KayittanDusmedenTIF", kForm);
            Response.Redirect("TasinirIslemFormAna.aspx");
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden kayýttan düþme teklif ve onay tutanaðý bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Kayýttan düþme teklif ve onay tutanaðý bilgileri döndürülür.</returns>
        private KayittanDusmeForm KriterTopla()
        {
            KayittanDusmeForm kf = new KayittanDusmeForm();
            kf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            kf.muhasebeKod = txtMuhasebe.Text.Trim();
            kf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kf.tutanakNo = hdnTutanakNo.Value.Trim();
            kf.ambarKod = txtAmbar.Text.Trim();
            kf.tutanakTarihi = new TNSDateTime(txtTutanakTarihi.Value.Trim());
            return kf;
        }

        /// <summary>
        /// Parametre olarak verilen kayýttan düþme teklif ve onay tutanaðý üst bilgilerine detay
        /// bilgileri ekleyip kaydedilmek üzere sunucuya gönderir ve iþlem sonucunu ekranda görüntüler.
        /// </summary>
        /// <param name="kf">Kayýttan düþme teklif ve onay tutanaðý bilgilerini tutan nesne</param>
        private void Kaydet(KayittanDusmeForm kf)
        {
            fpL.SaveChanges();

            int siraNo = 1;

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                KayittanDusmeDetay detay = new KayittanDusmeDetay();
                detay.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
                detay.muhasebeKod = txtMuhasebe.Text.Trim();
                detay.harcamaKod = txtHarcamaBirimi.Text.Trim();
                detay.tutanakNo = hdnTutanakNo.Value.Trim();

                detay.hesapPlanKod = fpL.Sheets[0].Cells[i, 0].Text.Trim();
                detay.gorSicilNo = fpL.Sheets[0].Cells[i, 2].Text.Trim();
                detay.miktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 4].Text.Trim(), 0);
                detay.kdvOran = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 6].Text.Trim(), 0);
                detay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 7].Text.Trim(), 0);

                if (!string.IsNullOrEmpty(detay.hesapPlanKod))
                {
                    detay.siraNo = siraNo++;
                    kf.detay.Add(detay);
                }
            }

            Sonuc sonuc = servisTMM.KayittanDusmeKaydet(kullanan, kf);
            if (!sonuc.islemSonuc)
                GenelIslemler.HataYaz(this, sonuc.hataStr);
            else
            {
                hdnTutanakNo.Value = sonuc.anahtar;
                ButonlariAktifYap();
                Listele(KriterTopla());
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMKDG005);
            }
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            hdnTutanakNo.Value = "";

            fpL.CancelEdit();
            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = "";
            fpL.Sheets[0].RowCount = ekleSatirSayisi;
        }

        /// <summary>
        /// Sayfadaki farpoint grid kontrolünün ilk yükleniþte ayarlanmasýný saðlayan yordam
        /// </summary>
        /// <param name="kontrol">Farpoint grid kontrolü</param>
        void GridInit(FarPoint.Web.Spread.FpSpread kontrol)
        {
            kontrol.RenderCSSClass = true;
            kontrol.EditModeReplace = true;

            kontrol.Sheets.Count = 1;

            fpL.Sheets[0].RowCount = ekleSatirSayisi;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 9;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMKDG006;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMKDG007;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMKDG008;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMKDG009;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMKDG010;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMKDG011;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMKDG012;

            kontrol.Sheets[0].Columns[0].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[1].HorizontalAlign = HorizontalAlign.Center;
            kontrol.Sheets[0].Columns[2].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[3].HorizontalAlign = HorizontalAlign.Center;
            kontrol.Sheets[0].Columns[4, 5].HorizontalAlign = HorizontalAlign.Right;

            kontrol.Sheets[0].Columns[4, 8].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[4, 8].Locked = true;

            kontrol.Sheets[0].Columns[0].Width = 120;
            kontrol.Sheets[0].Columns[1].Width = 30;
            kontrol.Sheets[0].Columns[2].Width = 130;
            kontrol.Sheets[0].Columns[3].Width = 30;
            kontrol.Sheets[0].Columns[4].Width = 60;
            kontrol.Sheets[0].Columns[5].Width = 60;
            kontrol.Sheets[0].Columns[6].Width = 30;
            kontrol.Sheets[0].Columns[7].Width = 100;
            kontrol.Sheets[0].Columns[8].Width = 190;

            TasinirGenel.MyLinkType sicilNoLink = new TasinirGenel.MyLinkType("SicilNoListesiAc()");
            sicilNoLink.ImageUrl = "../App_themes/images/bul1.gif";
            kontrol.Sheets[0].Columns[3].CellType = sicilNoLink;

            TasinirGenel.MyLinkType hesapPlanLink = new TasinirGenel.MyLinkType("StokListesiAc(0)");
            hesapPlanLink.ImageUrl = "../App_themes/images/bul1.gif";
            kontrol.Sheets[0].Columns[1].CellType = hesapPlanLink;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;
            kontrol.Sheets[0].Columns[2].CellType = cTextType;

            kontrol.Attributes.Add("onDataChanged", "HucreDegisti(this)");
            GenelIslemlerIstemci.RakamAlanFormatla(kontrol, 4, 4, 4);
            GenelIslemlerIstemci.RakamAlanFormatla(kontrol, 5, 5, 6);
        }

        /// <summary>
        /// Sayfadaki kontrollerin htmle çevrilmesini yapan yordam
        /// </summary>
        /// <param name="writer">Kontrollerin içeriðini yazan nesne</param>
        protected override void Render(HtmlTextWriter writer)
        {
            GenelIslemler.ListeYazdirDugmeGizle(fpL.FindControl("Print"));
            GenelIslemler.ListeYazdirDugmeGizle(fpL.FindControl("Cancel"));
            GenelIslemler.ListeYazdirDugmeGizle(fpL.FindControl("Update"));

            Control updateBtn = fpL.FindControl("Paste");
            if (updateBtn != null)
            {
                TableCell tc = (TableCell)updateBtn.Parent;
                TableRow tr = (TableRow)tc.Parent;

                TableCell tc1 = new TableCell();
                tr.Cells.Add(tc1);

                Image img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMKDG013;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMKDG014;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMKDG015;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMKDG016;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/sigma.gif";
                img.AlternateText = Resources.TasinirMal.FRMKDG017;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ToplamHesapla(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow50.gif";
                img.AlternateText = Resources.TasinirMal.FRMKDG018;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc50(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow100.gif";
                img.AlternateText = Resources.TasinirMal.FRMKDG019;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc100(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);
            }

            base.Render(writer);
        }

        /// <summary>
        /// Farpoint grid kontrolü ile ilgili boþ satýr ekleme, araya
        /// satýr ekleme ve satýr silme gibi iþlemlerin yapýldýðý yordam
        /// </summary>
        /// <param name="tur">Ýþlemin ne olduðu</param>
        void fpL_ButtonCommand(string tur)
        {
            if (tur == "bossatirekle")
            {
                fpL.ActiveSheetView.RowCount += ekleSatirSayisi;
            }
            if (tur == "arayasatirekle")
            {
                try
                {
                    int aktifSatir = fpL.ActiveSheetView.ActiveRow;
                    int acSatir = Math.Abs(aktifSatir - fpL.ActiveSheetView.SelectionModel.LeadRow) + 1;
                    fpL.ActiveSheetView.AddRows(aktifSatir, acSatir);
                }
                catch { }
            }
            if (tur == "satirsil")
            {
                try
                {
                    int aktifSatir = fpL.ActiveSheetView.ActiveRow;
                    int acSatir = Math.Abs(aktifSatir - fpL.ActiveSheetView.SelectionModel.LeadRow) + 1;
                    fpL.ActiveSheetView.RemoveRows(aktifSatir, acSatir);
                }
                catch { }
            }
            if (tur == "bossatirekle50")
            {
                fpL.ActiveSheetView.RowCount += 50;
            }
            if (tur == "bossatirekle100")
            {
                fpL.ActiveSheetView.RowCount += 100;
            }
            GenelIslemlerIstemci.RakamAlanFormatla(fpL, 4, 4, 4);
            GenelIslemlerIstemci.RakamAlanFormatla(fpL, 5, 5, 6);
            fpL.SaveChanges();
        }

        /// <summary>
        /// Sayfadaki farpoint grid kontrolünün format bilgilerini sessiona saklayan ya da okuyan yordam
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void fpL_SaveOrLoadSheetState(object sender, FarPoint.Web.Spread.SheetViewStateEventArgs e)
        {
            object o;
            object temp = null;

            if (e.IsSave)
                Session["SpreadData" + e.Index + ViewState["fpID"]] = e.SheetView.SaveViewState();
            else
            {
                o = Session["SpreadData" + e.Index + ViewState["fpID"]];
                if (!(object.ReferenceEquals(o, temp)))
                    e.SheetView.LoadViewState(o);
            }
            e.Handled = true;
        }
    }
}