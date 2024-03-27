using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Geçici alýndý fiþi bilgilerinin kayýt ve listeleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class GeciciAlindiFormEski : istemciUzayi.GenelSayfaV3
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Gride eklenecek satýr sayýsý
        /// </summary>
        int ekleSatirSayisi = 30;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            formAdi = Resources.TasinirMal.FRMGAG001;
            SayfaUstAltBolumYaz(this);
            hdnFirmaHarcamadanAlma.Value = TasinirGenel.tasinirFirmaBilgisiniHarcamadanAlma;

            //Sayfaya giriþ izni varmý?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                GenelIslemler.SayfayaGirmesin(true);

            this.fpL.Attributes.Add("onDataChanged", "HucreDegisti(this)");
            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");
            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");

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
                YilDoldur();
                GridInit(fpL);
                txtBelgeTarih.Text = DateTime.Now.Date.ToShortDateString();
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
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
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Geçici alýndý fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(Object sender, EventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.fisTarih = new TNSDateTime(txtBelgeTarih.Text);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.ambarKod = txtAmbar.Text.Replace(".", "");
            tf.neredenGeldi = txtNeredenGeldi.Text;
            tf.faturaNo = txtFaturaNo.Text;
            tf.faturaTarih = new TNSDateTime(txtFaturaTarih.Text);
            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;

            fpL.SaveChanges();

            int siraNo = 1;

            for (int i = 0; i < fpL.Rows.Count; i++)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();

                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.harcamaKod = tf.harcamaKod;
                td.ambarKod = tf.ambarKod;
                td.siraNo = siraNo;
                td.hesapPlanKod = fpL.Cells[i, 0].Text.Trim().Replace(".", "");
                td.miktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Cells[i, 2].Text);
                td.kdvOran = OrtakFonksiyonlar.ConvertToInt(fpL.Cells[i, 4].Text, 0);
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(fpL.Cells[i, 5].Text);
                //Serverda hesaplanýyor (kdv mükellefi olan muhasebelerden dolayý commentlendi)
                //td.birimFiyatKDVLi = (1 + (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100)) * td.birimFiyat;

                if (td.hesapPlanKod == string.Empty)
                    continue;
                else
                    siraNo++;

                tf.detay.Ekle(td);
            }

            Sonuc sonuc = servis.GeciciAlindiFisiKaydet(kullanan, tf);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                txtBelgeNo.Text = sonuc.anahtar;
            }
            else
                GenelIslemler.HataYaz(this, sonuc.hataStr);
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(Object sender, EventArgs e)
        {
            fpL.CancelEdit();
            fpL.Sheets[0].RowCount = 0;
            fpL.Sheets[0].RowCount = ekleSatirSayisi;
            ddlYil.SelectedValue = DateTime.Now.Year.ToString();
            txtBelgeTarih.Text = DateTime.Now.Date.ToShortDateString();
            txtBelgeNo.Text = string.Empty;
            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            txtFaturaNo.Text = string.Empty;
            txtFaturaTarih.Text = string.Empty;
            txtNeredenGeldi.Text = string.Empty;
            if (txtMuhasebe.Text == "")
                lblMuhasebeAd.Text = string.Empty;
            if (txtHarcamaBirimi.Text == "")
                lblHarcamaBirimiAd.Text = string.Empty;
            if (txtAmbar.Text == "")
                lblAmbarAd.Text = string.Empty;
        }

        /// <summary>
        /// TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Geçici alýndý fiþinin bilgilerini sunucudan alýr, sessiona yazar ve taþýnýr iþlem fiþi ekranýna yönlendirir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIF_Click(object sender, EventArgs e)
        {
            FisAc(false, true);
        }

        /// <summary>
        /// Belge Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Geçici alýndý fiþinin bilgilerini sunucudan alýr ve excel dosyasýna yazýp kullanýcýya gönderir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnGeciciAlindi_Click(object sender, EventArgs e)
        {
            FisAc(true, false);
        }

        /// <summary>
        /// Geçici alýndý fiþinin bilgilerini sunucudaki ilgili yordamdan ister,
        /// hata varsa hatayý görüntüler, yoksa sunucudan gelen bilgileri verilen
        /// parametrelere bakarak ExceleYaz yordamýna ya da TifOlustur yordamýna yönlendirir.
        /// </summary>
        /// <param name="excelMi">Geçici alýndý fiþinin excel raporu mu isteniyor bilgisi</param>
        /// <param name="tifMi">Geçici alýndý fiþinin taþýnýr iþlem fiþi mi oluþturulacak bilgisi</param>
        private void FisAc(bool excelMi, bool tifMi)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            tf.fisNo = txtBelgeNo.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();

            ObjectArray bilgi = servis.GeciciAlindiFisiAc(kullanan, tf);

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

            tf = (TNS.TMM.TasinirIslemForm)bilgi[0];

            if (excelMi)
            {
                ExceleYaz(tf);
            }
            else if (tifMi)
                TifOlustur(tf);
        }

        /// <summary>
        /// Parametre olarak verilen geçici alýndý fiþini sessiona yazar ve taþýnýr iþlem fiþi ekranýna yönlendirme yapar.
        /// </summary>
        /// <param name="tf">Geçici alýndý fiþi bilgilerini tutan nesne</param>
        private void TifOlustur(TNS.TMM.TasinirIslemForm tf)
        {
            Session.Add("GeciciAlindidanTIF", tf);
            //Response.Redirect("TasinirIslemFormAna.aspx");
            ClientScript.RegisterStartupScript(this.GetType(), "tifAc", "top.location.href='TasinirIslemFormAna.aspx';", true);


        }

        /// <summary>
        /// Listele resmine basýlýnca çalýþan olay metodu
        /// Sunucudan geçici alýndý fiþinin bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, ImageClickEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TasinirIslemForm tff = new TNS.TMM.TasinirIslemForm();

            fpL.Sheets[0].RowCount = 0;
            GridInit(fpL);

            tff.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tff.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tff.muhasebeKod = txtMuhasebe.Text;
            tff.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray bilgi = servis.GeciciAlindiFisiAc(kullanan, tff);

            if (bilgi.sonuc.islemSonuc)
            {
                TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
                tf = (TNS.TMM.TasinirIslemForm)bilgi[0];

                txtBelgeNo.Text = tf.fisNo;
                txtFaturaNo.Text = tf.faturaNo;
                txtFaturaTarih.Text = tf.faturaTarih.ToString();
                txtNeredenGeldi.Text = tf.neredenGeldi;

                fpL.Sheets[0].RowCount = tf.detay.ObjeSayisi;
                int satir = 0;
                foreach (TasinirIslemDetay td in tf.detay.objeler)
                {
                    fpL.Cells[satir, 0].Text = td.hesapPlanKod;
                    fpL.Cells[satir, 2].Text = td.miktar.ToString();
                    fpL.Cells[satir, 3].Text = td.olcuBirimAd;
                    fpL.Cells[satir, 4].Text = td.kdvOran.ToString();
                    fpL.Cells[satir, 5].Text = td.birimFiyat.ToString();
                    fpL.Cells[satir, 6].Text = td.hesapPlanAd;
                    satir++;
                }
            }
            else
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Parametre olarak verilen geçici alýndý fiþinin excel raporunu oluþturur.
        /// </summary>
        /// <param name="tf">Geçici alýndý fiþi bilgilerini tutan nesne</param>
        private void ExceleYaz(TNS.TMM.TasinirIslemForm tf)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirGeciciAlindi.XLT";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("IlAd", tf.ilAd + "-" + tf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tf.ilKod + "-" + tf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tf.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tf.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tf.muhasebeKod);
            XLS.HucreAdBulYaz("FaturaTarih", tf.faturaTarih.ToString());
            XLS.HucreAdBulYaz("FaturaSayi", tf.faturaNo);
            XLS.HucreAdBulYaz("NeredenGeldi", tf.neredenGeldi);
            XLS.HucreAdBulYaz("BelgeTarih", tf.fisTarih.ToString());
            XLS.HucreAdBulYaz("BelgeNo", tf.fisNo);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;

            int siraNo = 0;
            foreach (TasinirIslemDetay dt in tf.detay.objeler)
            {
                siraNo++;
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
                XLS.HucreDegerYaz(satir, sutun + 1, dt.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, dt.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 4, dt.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(dt.miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(dt.birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble((dt.miktar * dt.birimFiyat).ToString(), (double)0));
            }

            ImzaEkle(XLS, ref satir, sutun, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Geçici alýndý fiþi excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        private void ImzaEkle(Tablo XLS, ref int satir, int sutun, string muhasebe, string harcama, string ambar)
        {
            satir += 2;

            ObjectArray imza = servis.ImzaListele(kullanan, muhasebe, harcama, ambar, (int)ENUMImzaYer.TASINIRKAYITYETKILISI);

            ImzaBilgisi iBilgi = new ImzaBilgisi();
            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
                iBilgi = (ImzaBilgisi)imza[0];

            XLS.SatirAc(satir, 9);
            XLS.HucreKopyala(0, sutun, 8, sutun + 7, satir, sutun);

            XLS.HucreDegerYaz(satir, sutun + 1, Resources.TasinirMal.FRMGAG002);
            XLS.HucreDegerYaz(satir + 2, sutun + 6, string.Format(Resources.TasinirMal.FRMGAG003, DateTime.Today.Date.ToShortDateString()));
            XLS.HucreDegerYaz(satir + 3, sutun + 4, Resources.TasinirMal.FRMGAG004);
            XLS.HucreDegerYaz(satir + 4, sutun + 4, string.Format(Resources.TasinirMal.FRMGAG005, iBilgi.adSoyad));
            XLS.HucreDegerYaz(satir + 5, sutun + 4, string.Format(Resources.TasinirMal.FRMGAG006, iBilgi.unvan));
            XLS.HucreDegerYaz(satir + 6, sutun + 4, Resources.TasinirMal.FRMGAG007);

            for (int i = satir; i <= satir + 6; i++)
            {
                if (i == satir)
                    XLS.HucreBirlestir(i, sutun + 1, i, sutun + 6);
                else if (i != satir + 2)
                    XLS.HucreBirlestir(i, sutun + 4, i, sutun + 6);

                if (i == satir + 3)
                    XLS.KoyuYap(i, sutun + 4, true);

                if (i == satir)
                {
                    XLS.DuseyHizala(i, sutun + 1, 0);
                    XLS.YatayHizala(i, sutun + 1, 2);
                }
                else if (i == satir + 3)
                {
                    XLS.DuseyHizala(i, sutun + 4, 2);
                    XLS.YatayHizala(i, sutun + 4, 2);
                }
                else
                {
                    XLS.DuseyHizala(i, sutun + 4, 0);
                    XLS.YatayHizala(i, sutun + 4, 2);
                }
            }

            XLS.YatayCizgiCiz(satir + 8, sutun, sutun + 7, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir - 1, satir + 7, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir - 1, satir + 7, sutun + 8, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 7;
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
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
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 7;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMGAG008;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMGAG009;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMGAG010;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMGAG011;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMGAG012;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMGAG013;

            kontrol.Sheets[0].Columns[0].Width = 120;
            kontrol.Sheets[0].Columns[1].Width = 30;
            kontrol.Sheets[0].Columns[2].Width = 60;
            kontrol.Sheets[0].Columns[3].Width = 60;
            kontrol.Sheets[0].Columns[4].Width = 30;
            kontrol.Sheets[0].Columns[5].Width = 100;
            kontrol.Sheets[0].Columns[6].Width = 190;

            kontrol.Sheets[0].Columns[2].HorizontalAlign = HorizontalAlign.Right;
            kontrol.Sheets[0].Columns[4, 5].HorizontalAlign = HorizontalAlign.Right;

            TasinirGenel.MyLinkType hesapPlaniLink = new TasinirGenel.MyLinkType("HesapPlaniGoster()");
            hesapPlaniLink.ImageUrl = "../App_themes/images/bul1.gif";

            kontrol.Sheets[0].Columns[1].CellType = hesapPlaniLink;

            kontrol.Sheets[0].Columns[3].Locked = true; //Ölçü Birimi
            kontrol.Sheets[0].Columns[6].Locked = true; //Hesap Planý Adý

            kontrol.Sheets[0].Columns[0, kontrol.Sheets[0].Columns.Count - 1].ForeColor = System.Drawing.Color.Black;

            kontrol.Sheets[0].Columns[3].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[6].BackColor = System.Drawing.Color.LightGoldenrodYellow;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;
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
                img.AlternateText = Resources.TasinirMal.FRMGAG014;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMGAG015;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMGAG016;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMGAG017;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/sigma.gif";
                img.AlternateText = Resources.TasinirMal.FRMGAG018;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ToplamHesapla(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);
            }

            base.Render(writer);
        }

        /// <summary>
        /// Farpoint grid kontrolü ile ilgili boþ satýr ekleme, araya
        /// satýr ekleme ve satýr silme iþlemlerinin yapýldýðý yordam
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