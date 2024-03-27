using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// İhtiyaç fazlası taşınır formu bilgilerinin kayıt, listeleme, silme ve raporlama işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class AcikPazarForm : TMMSayfa
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Gride eklenecek satır sayısı
        /// </summary>
        int ekleSatirSayisi = 20;

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

            formAdi = Resources.TasinirMal.FRMAPF001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtIYMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblIYMuhasebeAd',true,new Array('txtIYMuhasebe'),'KONTROLDENOKU');");
            this.txtIYHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblIYHarcamaBirimiAd',true,new Array('txtIYMuhasebe','txtIYHarcamaBirimi'),'KONTROLDENOKU');");

            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            //this.btnYazdir.Attributes.Add("onclick", "return OnayAl('Yazdir','btnYazdir');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");
            this.btnSil.Attributes.Add("onclick", "return OnayAl('Sil','btnSil');");

            //fpL işlemlerini karşılamak için
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

            if (txtIYMuhasebe.Text.Trim() != "")
                lblIYMuhasebeAd.Text = GenelIslemler.KodAd(31, txtIYMuhasebe.Text.Trim(), true);
            else
                lblIYMuhasebeAd.Text = "";

            if (txtIYHarcamaBirimi.Text.Trim() != "")
                lblIYHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtIYMuhasebe.Text.Trim() + "-" + txtIYHarcamaBirimi.Text.Trim(), true);
            else
                lblIYHarcamaBirimiAd.Text = "";
        }

        /// <summary>
        /// Seçilmiş olan taşınır malzemelerini toplayan yordam
        /// </summary>
        /// <returns>İhtiyaç fazlası taşınır formuna ait seçili bilgileri tutan nesne</returns>
        private TNS.TMM.AcikPazarForm SeciliSatirlariTopla()
        {
            TNS.TMM.AcikPazarForm apf = new TNS.TMM.AcikPazarForm();
            apf.muhasebeKod = txtMuhasebe.Text.Trim();
            apf.muhasebeAd = lblMuhasebeAd.Text.Trim();
            apf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            apf.harcamaAd = lblHarcamaBirimiAd.Text.Trim();
            apf.istekYapanMuhasebeKod = txtIYMuhasebe.Text.Trim();
            apf.istekYapanMuhasebeAd = lblIYMuhasebeAd.Text.Trim();
            apf.istekYapanHarcamaKod = txtIYHarcamaBirimi.Text.Trim();
            apf.istekYapanHarcamaAd = lblIYHarcamaBirimiAd.Text.Trim();

            fpL.SaveChanges();
            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                if (fpL.Sheets[0].Cells[i, 0].Value != null)
                {
                    if (fpL.Sheets[0].Cells[i, 0].Value.ToString() == "1")
                    {
                        TNS.TMM.AcikPazarDetay detay = new TNS.TMM.AcikPazarDetay();

                        detay.gorSicilNo = fpL.Sheets[0].Cells[i, 2].Text.Trim();
                        detay.hesapKod = fpL.Sheets[0].Cells[i, 4].Text.Trim();
                        detay.hesapAd = fpL.Sheets[0].Cells[i, 5].Text.Trim();
                        detay.kdvOran = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 6].Text.Trim(), 0);
                        detay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 7].Text.Trim(), (decimal)0);
                        detay.aciklama = fpL.Sheets[0].Cells[i, 8].Text.Trim();
                        detay.eklenisTarihi = new TNSDateTime(fpL.Sheets[0].Cells[i, 9].Text.Trim());

                        if (!string.IsNullOrEmpty(detay.gorSicilNo))
                            apf.detaylar.Add(detay);
                    }
                }
            }

            return apf;
        }

        /// <summary>
        /// TİF Oluştur tuşuna basılınca çalışan olay metodu
        /// Seçili olan taşınır malzemeleri toplar, sessiona yazar ve taşınır işlem fişi ekranına yönlendirir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIF_Click(object sender, EventArgs e)
        {
            divIY.Attributes.Add("style", "");

            TNS.TMM.AcikPazarForm apf = SeciliSatirlariTopla();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(apf.istekYapanMuhasebeKod) || string.IsNullOrEmpty(apf.istekYapanHarcamaKod))
                hata = Resources.TasinirMal.FRMAPF002 + "<br>";
            if (apf.detaylar.Count <= 0)
                hata += Resources.TasinirMal.FRMAPF003;
            if (!string.IsNullOrEmpty(hata))
            {
                GenelIslemler.HataYaz(this, hata);
                return;
            }

            Session.Add("AcikPazardanTIF", apf);
            Response.Redirect("TasinirIslemFormAna.aspx");
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Sunucudan ihtiyaç fazlası taşınır bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla(false));
        }

        /// <summary>
        /// Listeleme kriterleri ihtiyaç fazlası taşınır nesnesinde parametre olarak alınır ve sunucuya
        /// gönderilir ve ihtiyaç fazlası taşınır bilgileri sunucudan alınır. Hata varsa ekrana hata
        /// bilgisi yazılır, yoksa gelen ihtiyaç fazlası taşınır bilgileri EkranaYaz yordamına gönderilir.
        /// </summary>
        /// <param name="apfKriter">İhtiyaç fazlası taşınır listeleme kriterlerini tutan nesne</param>
        private void Listele(TNS.TMM.AcikPazarForm apfKriter)
        {
            ObjectArray apfVAN = servisTMM.AcikPazarListele(kullanan, apfKriter, false);

            Temizle();

            if (!apfVAN.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, apfVAN.sonuc.hataStr);
                return;
            }

            if (apfVAN.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, apfVAN.sonuc.bilgiStr);
                return;
            }

            EkranaYaz(apfVAN);
        }

        /// <summary>
        /// Parametre olarak verilen ihtiyaç fazlası taşınır bilgilerini ekrana yazan yordam
        /// </summary>
        /// <param name="apfVAN">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        private void EkranaYaz(ObjectArray apfVAN)
        {
            GridSiyahYap();

            TNS.TMM.AcikPazarForm apf = (TNS.TMM.AcikPazarForm)apfVAN.objeler[0];
            txtMuhasebe.Text = apf.muhasebeKod.Trim();
            lblMuhasebeAd.Text = apf.muhasebeAd.Trim();
            txtHarcamaBirimi.Text = apf.harcamaKod.Trim();
            lblHarcamaBirimiAd.Text = apf.harcamaAd.Trim();

            if (apf.detaylar.Count >= fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = apf.detaylar.Count + ekleSatirSayisi;

            int sayac = 0;
            foreach (TNS.TMM.AcikPazarDetay detay in apf.detaylar)
            {
                TNSDateTime kontrolTarih = new TNSDateTime(detay.eklenisTarihi.ToString());
                kontrolTarih.AddDays(60);
                if (kontrolTarih.CompareTo(DateTime.Today) == -1)
                    fpL.Sheets[0].Rows[sayac].ForeColor = System.Drawing.Color.Red;

                if (detay.istekVar)
                {
                    TasinirGenel.MyLinkType istekLink = new TasinirGenel.MyLinkType("IstekTarihceAc('" + detay.prSicilNo + "')");
                    istekLink.ImageUrl = "../App_themes/images/aramainfo.gif";
                    fpL.Sheets[0].Cells[sayac, 1].Locked = false;
                    fpL.Sheets[0].Cells[sayac, 1].CellType = istekLink;
                }

                fpL.Sheets[0].Cells[sayac, 2].Text = detay.gorSicilNo;
                fpL.Sheets[0].Cells[sayac, 4].Text = detay.hesapKod;
                fpL.Sheets[0].Cells[sayac, 5].Text = detay.hesapAd;
                fpL.Sheets[0].Cells[sayac, 6].Text = detay.kdvOran.ToString();
                fpL.Sheets[0].Cells[sayac, 7].Text = detay.birimFiyat.ToString();
                fpL.Sheets[0].Cells[sayac, 8].Text = detay.aciklama;
                fpL.Sheets[0].Cells[sayac, 9].Text = detay.eklenisTarihi.ToString();

                sayac++;
            }
        }

        /// <summary>
        /// Farpoint grid kontrolünün tüm satırlarının fontunu siyah yapan yordam
        /// </summary>
        private void GridSiyahYap()
        {
            fpL.Sheets[0].Rows[0, fpL.Sheets[0].Rows.Count - 1].ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Sil tuşuna basılınca çalışan olay metodu
        /// İhtiyaç fazlası taşınır formu silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata varsa hata görüntülenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, EventArgs e)
        {
            GridSiyahYap();

            TNS.TMM.AcikPazarForm apf = KriterTopla(false);
            if (apf != null)
            {
                Sonuc sonuc = servisTMM.AcikPazarSil(kullanan, apf);
                if (sonuc.islemSonuc)
                    GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                else
                    GenelIslemler.HataYaz(this, sonuc.hataStr);
            }
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// İhtiyaç fazlası taşınır formu kaydedilmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata varsa hata görüntülenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            TNS.TMM.AcikPazarForm apf = KriterTopla(true);
            if (apf != null)
            {
                Sonuc sonuc = servisTMM.AcikPazarKaydet(kullanan, apf);
                if (sonuc.islemSonuc)
                {
                    Listele(KriterTopla(false));
                    GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                }
                else
                    GenelIslemler.HataYaz(this, sonuc.hataStr);
            }
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden ihtiyaç fazlası taşınır bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <param name="kaydet">Yordam kaydetme işlemi için mi çağırılıyor bilgisi</param>
        /// <returns>İhtiyaç fazlası taşınır bilgileri döndürülür.</returns>
        private TNS.TMM.AcikPazarForm KriterTopla(bool kaydet)
        {
            TNS.TMM.AcikPazarForm apf = new TNS.TMM.AcikPazarForm();
            apf.muhasebeKod = txtMuhasebe.Text.Trim();
            apf.muhasebeAd = lblMuhasebeAd.Text.Trim();
            apf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            apf.harcamaAd = lblHarcamaBirimiAd.Text.Trim();
            //apf.istekYapanMuhasebeKod = txtIYMuhasebe.Text.Trim();
            //apf.istekYapanMuhasebeAd = lblIYMuhasebeAd.Text.Trim();
            //apf.istekYapanHarcamaKod = txtIYHarcamaBirimi.Text.Trim();
            //apf.istekYapanHarcamaAd = lblIYHarcamaBirimiAd.Text.Trim();

            if (!kaydet)
                return apf;

            fpL.SaveChanges();
            int sayac = 1;
            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                TNS.TMM.AcikPazarDetay detay = new TNS.TMM.AcikPazarDetay();
                detay.muhasebeKod = apf.muhasebeKod;
                detay.harcamaKod = apf.harcamaKod;

                //Detay
                detay.siraNo = sayac;
                detay.gorSicilNo = fpL.Sheets[0].Cells[i, 2].Text.Trim();
                detay.aciklama = fpL.Sheets[0].Cells[i, 8].Text.Trim();
                detay.eklenisTarihi = new TNSDateTime(fpL.Sheets[0].Cells[i, 9].Text.Trim());

                if (!string.IsNullOrEmpty(detay.gorSicilNo))
                {
                    apf.detaylar.Add(detay);
                    sayac++;
                }
            }
            return apf;
        }

        /// <summary>
        /// Yazdır tuşuna basılınca çalışan olay metodu
        /// Ekrandan seçili satır bilgileri alınır ve excel dosyasına yazılmak üzere ExceleYaz yordamına gönderilir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            divIY.Attributes.Add("style", "");

            TNS.TMM.AcikPazarForm apf = SeciliSatirlariTopla();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(apf.istekYapanMuhasebeKod) || string.IsNullOrEmpty(apf.istekYapanHarcamaKod))
                hata = Resources.TasinirMal.FRMAPF004 + "<br>";
            if (apf.detaylar.Count <= 0)
                hata += Resources.TasinirMal.FRMAPF005;
            if (!string.IsNullOrEmpty(hata))
            {
                GenelIslemler.HataYaz(this, hata);
                return;
            }

            ExceleYaz(apf);
        }

        /// <summary>
        /// Parametre olarak verilen ihtiyaç fazlası taşınır bilgileri excel dosyasına yazılıp kullanıcıya gönderilir.
        /// </summary>
        /// <param name="apf">Yazdırılacak ihtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        private void ExceleYaz(TNS.TMM.AcikPazarForm apf)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AcikPazar.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Aciklama1", string.Format(Resources.TasinirMal.FRMAPF006, apf.muhasebeKod, apf.muhasebeAd, apf.harcamaKod, 
                apf.harcamaAd, apf.istekYapanMuhasebeKod, apf.istekYapanMuhasebeAd, apf.istekYapanHarcamaKod, apf.istekYapanHarcamaAd));

            XLS.HucreAdBulYaz("Tarih", DateTime.Now.Date.ToShortDateString());

            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int satir = kaynakSatir;

            decimal toplam = 0;
            foreach (TNS.TMM.AcikPazarDetay detay in apf.detaylar)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, satir - kaynakSatir);
                XLS.HucreDegerYaz(satir, sutun + 1, apf.muhasebeKod + "-" + apf.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 2, apf.harcamaKod + "-" + apf.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 3, detay.gorSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 4, detay.hesapAd);

                decimal bedel = (1 + detay.kdvOran / 100) * detay.birimFiyat;
                toplam += bedel;

                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(bedel.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 6, detay.aciklama);
                XLS.HucreDegerYaz(satir, sutun + 7, apf.istekYapanMuhasebeKod + "-" + apf.istekYapanMuhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 8, apf.istekYapanHarcamaKod + "-" + apf.istekYapanHarcamaAd);
            }

            ImzaEkle(XLS);

            if (toplam >= 13500)
                XLS.HucreAdBulYaz("Aciklama2", Resources.TasinirMal.FRMAPF007);
            else
            {
                XLS.HucreAdBulYaz("Aciklama2", Resources.TasinirMal.FRMAPF008);
                XLS.SatirGizle(kaynakSatir - 8, kaynakSatir - 4, true);
            }

            //Eklenen satırların yükseklikleri ayarlanıyor
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        private void ImzaEkle(Tablo XLS)
        {
            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", 0);

            string[] ad = new string[2];
            string[] unvan = new string[2];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.USTYONETICI && string.IsNullOrEmpty(ad[1]))
                    {
                        ad[1] = iBilgi.adSoyad;
                        unvan[1] = iBilgi.unvan;
                    }
                }
            }

            for (int i = 0; i < ad.Length; i++)
            {
                if (!string.IsNullOrEmpty(ad[i]))
                    XLS.HucreAdBulYaz("AdSoyad" + (i + 1).ToString(), ad[i]);

                if (!string.IsNullOrEmpty(unvan[i]))
                    XLS.HucreAdBulYaz("Unvan" + (i + 1).ToString(), unvan[i]);
            }
        }

        /// <summary>
        /// Temizle tuşuna basılınca çalışan olay metodu
        /// Kullanıcı tarafından sayfadaki kontrollere yazılmış bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            GridSiyahYap();
            Temizle();
        }

        /// <summary>
        /// Sayfadaki kontrollere yazılmış bilgilerin temizlenmesini sağlar.
        /// </summary>
        private void Temizle()
        {
            fpL.CancelEdit();
            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = "";
            fpL.Sheets[0].RowCount = ekleSatirSayisi;

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
                fpL.Sheets[0].Cells[i, 1].CellType = new FarPoint.Web.Spread.TextCellType();
        }

        /// <summary>
        /// Sayfadaki farpoint grid kontrolünün ilk yüklenişte ayarlanmasını sağlayan yordam
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
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 11;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 9].ColumnSpan = 2;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMAPF009;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 1].Value = Resources.TasinirMal.FRMAPF010;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMAPF011;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMAPF012;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMAPF013;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMAPF014;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMAPF015;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMAPF016;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 9].Value = Resources.TasinirMal.FRMAPF017;

            kontrol.Sheets[0].Columns[0, 3].HorizontalAlign = HorizontalAlign.Center;
            kontrol.Sheets[0].Columns[2].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[4, 8].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[6, 7].HorizontalAlign = HorizontalAlign.Right;
            kontrol.Sheets[0].Columns[9, 10].HorizontalAlign = HorizontalAlign.Center;

            kontrol.Sheets[0].Columns[1, 7].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[1, 7].Locked = true;
            kontrol.Sheets[0].Columns[3].Locked = false;

            kontrol.Sheets[0].Columns[0, 1].Width = 20;
            kontrol.Sheets[0].Columns[2].Width = 150;
            kontrol.Sheets[0].Columns[3].Width = 20;
            kontrol.Sheets[0].Columns[4, 5].Width = 110;
            kontrol.Sheets[0].Columns[6].Width = 30;
            kontrol.Sheets[0].Columns[7].Width = 70;
            kontrol.Sheets[0].Columns[8].Width = 120;
            kontrol.Sheets[0].Columns[9].Width = 60;
            kontrol.Sheets[0].Columns[10].Width = 20;

            TasinirGenel.MyLinkType sicilNoLink = new TasinirGenel.MyLinkType("SicilNoListesiAc()");
            sicilNoLink.ImageUrl = "../App_themes/images/bul1.gif";
            kontrol.Sheets[0].Columns[3].CellType = sicilNoLink;

            TasinirGenel.MyLinkType tarihLink = new TasinirGenel.MyLinkType("displayDatePicker('fpL', false, 'dmy', '.')");
            tarihLink.ImageUrl = "../App_themes/Images/takvim.gif";
            kontrol.Sheets[0].Columns[10].CellType = tarihLink;

            FarPoint.Web.Spread.CheckBoxCellType cChkType = new FarPoint.Web.Spread.CheckBoxCellType();
            kontrol.Sheets[0].Columns[0].CellType = cChkType;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[2].CellType = cTextType;
            kontrol.Sheets[0].Columns[4].CellType = cTextType;

            FarPoint.Web.Spread.DateTimeCellType cDateType = new FarPoint.Web.Spread.DateTimeCellType();
            kontrol.Sheets[0].Columns[9].CellType = cDateType;
        }

        /// <summary>
        /// Sayfadaki kontrollerin htmle çevrilmesini yapan yordam
        /// </summary>
        /// <param name="writer">Kontrollerin içeriğini yazan nesne</param>
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
                img.AlternateText = Resources.TasinirMal.FRMAPF018;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMAPF019;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMAPF020;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMAPF021;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);
            }

            base.Render(writer);
        }

        /// <summary>
        /// Farpoint grid kontrolü ile ilgili boş satır ekleme, araya
        /// satır ekleme ve satır silme işlemlerinin yapıldığı yordam
        /// </summary>
        /// <param name="tur">İşlemin ne olduğu</param>
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
        /// <param name="sender">Olayı tetikleyen nesne</param>
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