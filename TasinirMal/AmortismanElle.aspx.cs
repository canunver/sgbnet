using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Demirba� amortisman bilgilerinin kay�t, listeleme ve hesaplama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class AmortismanElle : TMMSayfa
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Gride eklenecek sat�r say�s�
        /// </summary>
        int ekleSatirSayisi = 20;

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMAME001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giri� izni varm�?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");

            //fpL i�lemlerini kar��lamak i�in
            //***********************************************
            if (Request.Form["__EVENTTARGET"] == "fpL")
            {
                string arg = Request.Form["__EVENTARGUMENT"] + "";
                fpL_ButtonCommand(arg);
            }

            if (!IsPostBack)
            {
                //TasinirGenel.AmortismanLisansKontrol(this);

                YilDoldur();

                ViewState["fpID"] = DateTime.Now.ToLongTimeString();

                GridInit(fpL);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            }
            else
                DegiskenSakla();

            if (!string.IsNullOrEmpty(txtMuhasebe.Text.Trim()))
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = string.Empty;

            if (!string.IsNullOrEmpty(txtHarcamaBirimi.Text.Trim()))
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = string.Empty;

            if (!string.IsNullOrEmpty(txtAmbar.Text.Trim()))
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = string.Empty;

            if (!string.IsNullOrEmpty(txtHesapPlanKod.Text.Trim()))
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = string.Empty;
        }

        /// <summary>
        /// Sayfadaki ddlYil ve ddlBelgeYil DropDownList kontrollerine y�l bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
            GenelIslemler.YilDoldur(ddlBelgeYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Ekrandan se�ilmi� olan muhasebe birimi, harcama birimi ve ambar
        /// bilgileri i�lem yapan kullan�c�n�n de�i�ken listesine saklan�r.
        /// </summary>
        private void DegiskenSakla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
        }

        /// <summary>
        /// Listele tu�una bas�l�nca �al��an olay metodu
        /// Sunucudan amortisman uygulanacak demirba�lar al�n�r ve listelenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Listeleme kriterleri amortisman kriter nesnesinde parametre olarak al�n�r ve sunucuya g�nderilir
        /// ve kriterlere uygun olan demirba� bilgileri sunucudan al�n�r. Hata varsa ekrana hata bilgisi
        /// yaz�l�r, yoksa gelen demirba� bilgileri ekrana yazd�r�lmak �zere EkranaYaz yordam�na aktar�l�r.
        /// Farpoint grid kontrol�n�n baz� kolonlar� ile ilgili g�rsel ayarlamalar yap�l�r.
        /// </summary>
        /// <param name="ak">Amortisman listeleme kriterlerini tutan nesne</param>
        private void Listele(AmortismanKriter ak)
        {
            ObjectArray amortismanVAN = new ObjectArray();
            //ObjectArray amortismanVAN = servisTMM.AmortismanRaporla(kullanan, ak, false);

            Temizle();

            if (!amortismanVAN.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, amortismanVAN.sonuc.hataStr);
                return;
            }

            if (amortismanVAN.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, amortismanVAN.sonuc.bilgiStr);
                return;
            }

            EkranaYaz(amortismanVAN);

            TNS.TMM.AmortismanRapor ar = (TNS.TMM.AmortismanRapor)amortismanVAN.objeler[0];
            if (ar.amortismanBaslamaYil > 0 && ar.amortismanBaslamaYil != OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0))
            {
                fpL.Sheets[0].Columns[6].BackColor = System.Drawing.Color.White;
                fpL.Sheets[0].Columns[6].Locked = false;

                fpL.Sheets[0].Columns[7].BackColor = System.Drawing.Color.LightGoldenrodYellow;
                fpL.Sheets[0].Columns[7].Locked = true;
            }
            else
            {
                fpL.Sheets[0].Columns[6].BackColor = System.Drawing.Color.LightGoldenrodYellow;
                fpL.Sheets[0].Columns[6].Locked = true;

                fpL.Sheets[0].Columns[7].BackColor = System.Drawing.Color.White;
                fpL.Sheets[0].Columns[7].Locked = false;
            }
        }

        /// <summary>
        /// Parametre olarak verilen demirba� amortisman bilgilerini ekrana yazan yordam
        /// </summary>
        /// <param name="amortismanVAN">Amortisman bilgilerini tutan nesne</param>
        private void EkranaYaz(ObjectArray amortismanVAN)
        {
            if (amortismanVAN.objeler.Count >= fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = amortismanVAN.objeler.Count + ekleSatirSayisi;

            int sayac = 0;
            foreach (TNS.TMM.AmortismanRapor detay in amortismanVAN.objeler)
            {
                fpL.Sheets[0].Cells[sayac, 0].Text = detay.gorSicilNo;
                fpL.Sheets[0].Cells[sayac, 1].Text = detay.hesapPlanKod;
                fpL.Sheets[0].Cells[sayac, 2].Text = detay.hesapPlanAd;
                fpL.Sheets[0].Cells[sayac, 3].Text = detay.maliyetTutar.ToString("#,###.000000000000");
                fpL.Sheets[0].Cells[sayac, 4].Text = detay.degerlemeTutar.ToString("#,###.000000000000");
                fpL.Sheets[0].Cells[sayac, 5].Text = detay.cariMaliyetAmortismanTutar.ToString("#,###.000000000000");
                fpL.Sheets[0].Cells[sayac, 6].Text = detay.cariDegerlemeAmortismanTutar.ToString("#,###.000000000000");
                fpL.Sheets[0].Cells[sayac, 7].Text = detay.kalanAmortismanSure.ToString();
                fpL.Sheets[0].Cells[sayac, 8].Text = detay.maliyetAmortismanBirikmisTutar.ToString("#,###.000000000000");
                fpL.Sheets[0].Cells[sayac, 9].Text = detay.degerlemeAmortismanBirikmisTutar.ToString("#,###.000000000000");

                sayac++;
            }
        }

        /// <summary>
        /// Kaydet tu�una bas�l�nca �al��an olay metodu
        /// Ekrana girilen amortisman bilgilerini toplay�p amortisman bilgilerini kaydeden
        /// sunucu yordam�na g�nderir, gelen sonuca g�re hata varsa hata g�r�nt�lenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            //AmortismanKriter ak = KriterTopla();
            //TNSCollection amortismanlar = KriterToplaDetay();

            //if (ak != null)
            //{
            //    Sonuc sonuc = servisTMM.AmortismanKaydetElle(kullanan, ak, amortismanlar);
            //    if (sonuc.islemSonuc)
            //        GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMAME002);
            //    else
            //        GenelIslemler.HataYaz(this, sonuc.hataStr);
            //}
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden amortisman kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>Amortisman kriter bilgileri d�nd�r�l�r.</returns>
        private AmortismanKriter KriterTopla()
        {
            AmortismanKriter ak = new AmortismanKriter();
            ak.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            ak.muhasebeKod = txtMuhasebe.Text.Trim();
            ak.harcamaKod = txtHarcamaBirimi.Text.Trim();
            ak.ambarKod = txtAmbar.Text.Trim();
            ak.belgeYil = OrtakFonksiyonlar.ConvertToInt(ddlBelgeYil.SelectedValue.Trim(), 0);
            ak.belgeNo = !string.IsNullOrEmpty(txtBelgeNo.Text.Trim()) ? txtBelgeNo.Text.Trim().PadLeft(6, '0') : string.Empty;
            ak.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            ak.gorSicilNo = txtSicilNo.Text.Trim();
            ak.edinmeIlkYili = OrtakFonksiyonlar.ConvertToInt(txtEdinmeIlk.Text.Trim(), 0);
            ak.edinmeSonYili = OrtakFonksiyonlar.ConvertToInt(txtEdinmeSon.Text.Trim(), 0);
            ak.birikmisAmortismanOran = OrtakFonksiyonlar.ConvertToDouble(txtBirikmisOran.Text.Trim(), (double)0);
            ak.kalanAmortismanSure = OrtakFonksiyonlar.ConvertToInt(txtKalanSure.Text.Trim(), 0);
            return ak;
        }

        /// <summary>
        /// Farpoint grid kontrol�ndeki demirba� amortisman bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>Demirba� amortisman bilgileri listesini tutan nesne</returns>
        private TNSCollection KriterToplaDetay()
        {
            TNSCollection amortismanlar = new TNSCollection();

            fpL.SaveChanges();
            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                int dongu = 1;
                for (int j = 0; j < dongu; j++)
                {
                    if (dongu == 1 && OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 5 + dongu].Text.Trim(), (decimal)0) > 0)
                        dongu++;

                    TNS.TMM.Amortisman detay = new TNS.TMM.Amortisman();

                    if (j == 0)
                        detay.amortismanTur = (int)ENUMAmortismanTur.NORMAL;
                    else
                        detay.amortismanTur = (int)ENUMAmortismanTur.DEGERLEME;

                    detay.gorSicilNo = fpL.Sheets[0].Cells[i, 0].Text.Trim();
                    detay.hesapPlanKod = fpL.Sheets[0].Cells[i, 1].Text.Trim();
                    detay.hesapPlanAd = fpL.Sheets[0].Cells[i, 2].Text.Trim();

                    detay.maliyetTutar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 3 + j].Text.Trim(), (decimal)0);
                    detay.amortismanMiktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 5 + j].Text.Trim(), (decimal)0);
                    detay.yapilanToplamAmortisman = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 8 + j].Text.Trim(), (decimal)0);
                    detay.kalanAmortismanSure = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 7].Text.Trim(), 0);

                    if (!string.IsNullOrEmpty(detay.gorSicilNo))
                        amortismanlar.Add(detay);
                }
            }

            return amortismanlar;
        }

        /// <summary>
        /// Hesapla tu�una bas�l�nca �al��an olay metodu
        /// Ekrana girilmi� kriter bilgileri toplan�r ve kriterlere uygun olan demirba�lar�n amortisman bilgilerinin hesaplanmas�
        /// i�in sunucuya g�nderilir. Sunucudan gelen hesaplanm�� amortisman bilgileri farpoint grid kontrol�ne yaz�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnHesapla_Click(object sender, EventArgs e)
        {
            //ObjectArray amortismanVAN = servisTMM.AmortismanHesapla(kullanan, KriterTopla());

            //Temizle();

            //if (!amortismanVAN.sonuc.islemSonuc)
            //{
            //    GenelIslemler.HataYaz(this, amortismanVAN.sonuc.hataStr);
            //    return;
            //}

            //if (amortismanVAN.objeler.Count <= 0)
            //{
            //    GenelIslemler.BilgiYaz(this, amortismanVAN.sonuc.bilgiStr);
            //    return;
            //}

            //if (amortismanVAN.objeler.Count >= fpL.Sheets[0].RowCount)
            //    fpL.Sheets[0].RowCount = amortismanVAN.objeler.Count + ekleSatirSayisi;

            //int sayac = 0;
            //foreach (TNS.TMM.Amortisman detay in amortismanVAN.objeler)
            //{
            //    fpL.Sheets[0].Cells[sayac, 0].Text = detay.gorSicilNo;
            //    fpL.Sheets[0].Cells[sayac, 1].Text = detay.hesapPlanKod;
            //    fpL.Sheets[0].Cells[sayac, 2].Text = detay.hesapPlanAd;
            //    fpL.Sheets[0].Cells[sayac, 3].Text = detay.maliyetTutar.ToString("#,###.000000000000");
            //    fpL.Sheets[0].Cells[sayac, 5].Text = detay.amortismanMiktar.ToString("#,###.000000000000");
            //    fpL.Sheets[0].Cells[sayac, 7].Text = detay.kalanAmortismanSure.ToString();
            //    fpL.Sheets[0].Cells[sayac, 8].Text = detay.yapilanToplamAmortisman.ToString("#,###.000000000000");

            //    sayac++;
            //}
        }

        /// <summary>
        /// Temizle tu�una bas�l�nca �al��an olay metodu
        /// Kullan�c� taraf�ndan sayfadaki kontrollere yaz�lm�� bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        /// <summary>
        /// Sayfadaki kontrollere yaz�lm�� bilgilerin temizlenmesini sa�lar.
        /// </summary>
        private void Temizle()
        {
            fpL.CancelEdit();
            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = "";
            fpL.Sheets[0].RowCount = ekleSatirSayisi;

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
                fpL.Sheets[0].Cells[i, 0, i, 1].CellType = new FarPoint.Web.Spread.TextCellType();
        }

        /// <summary>
        /// Sayfadaki farpoint grid kontrol�n�n ilk y�kleni�te ayarlanmas�n� sa�layan yordam
        /// </summary>
        /// <param name="kontrol">Farpoint grid kontrol�</param>
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
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 10;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMAME003;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 1].Value = Resources.TasinirMal.FRMAME004;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMAME005;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMAME006;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMAME007;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMAME008;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMAME009;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMAME010;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMAME011;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 9].Value = Resources.TasinirMal.FRMAME012;

            kontrol.Sheets[0].Columns[0, 2].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[3, 6].HorizontalAlign = HorizontalAlign.Right;
            kontrol.Sheets[0].Columns[7].HorizontalAlign = HorizontalAlign.Center;
            kontrol.Sheets[0].Columns[8, 9].HorizontalAlign = HorizontalAlign.Right;

            kontrol.Sheets[0].Columns[0, 4].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[0, 4].Locked = true;

            kontrol.Sheets[0].Columns[8, 9].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[8, 9].Locked = true;

            kontrol.Sheets[0].Columns[0].Width = 120;
            kontrol.Sheets[0].Columns[1].Width = 80;
            kontrol.Sheets[0].Columns[2].Width = 120;
            kontrol.Sheets[0].Columns[3, 6].Width = 60;
            kontrol.Sheets[0].Columns[7].Width = 55;
            kontrol.Sheets[0].Columns[8, 9].Width = 60;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0, 1].CellType = cTextType;

            kontrol.Attributes.Add("onDataChanged", "HucreDegisti(this)");
        }

        /// <summary>
        /// Sayfadaki kontrollerin htmle �evrilmesini yapan yordam
        /// </summary>
        /// <param name="writer">Kontrollerin i�eri�ini yazan nesne</param>
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
                img.AlternateText = Resources.TasinirMal.FRMAME013;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMAME014;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMAME015;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMAME016;
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
        /// Farpoint grid kontrol� ile ilgili bo� sat�r ekleme, araya
        /// sat�r ekleme ve sat�r silme i�lemlerinin yap�ld��� yordam
        /// </summary>
        /// <param name="tur">��lemin ne oldu�u</param>
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
        /// Sayfadaki farpoint grid kontrol�n�n format bilgilerini sessiona saklayan ya da okuyan yordam
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
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