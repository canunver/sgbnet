using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class TuketimUretim : TMMSayfa
    {
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        int ekleSatirSayisi = 20;

        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            GenelIslemlerIstemci.GenelJSResourceEkle(this);

            formAdi = Resources.TasinirMal.FRMTUG015;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan))
                    GenelIslemler.SayfayaGirmesin(true);

            this.fpL.Attributes.Add("onDataChanged", "HucreDegisti(this)");
            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");
            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");

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

                YilDoldur();
                BelgeTurDoldur();
                GridInit(fpL);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
                txtBelgeTarih.Text = DateTime.Today.ToShortDateString();
            }
            else
                DegiskenSakla();

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (!string.IsNullOrEmpty(txtAmbar.Text.Trim()))
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = string.Empty;
        }

        private void DegiskenSakla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            Kaydet(KriterTopla(true));
        }

        private TNS.TMM.TuketimUretim KriterTopla(bool kaydet)
        {
            TNS.TMM.TuketimUretim tu = new TNS.TMM.TuketimUretim();
            tu.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            tu.muhasebe = new Muhasebe();
            tu.muhasebe.kod = txtMuhasebe.Text.Trim();
            tu.harcamaBirimi = new HarcamaBirimi();
            tu.harcamaBirimi.kod = txtHarcamaBirimi.Text.Trim();
            tu.belgeNo = string.IsNullOrEmpty(txtBelgeNo.Text) ? string.Empty : txtBelgeNo.Text.PadLeft(6, '0');
            if (kaydet)
            {
                tu.ambar = new Ambar();
                tu.ambar.kod = txtAmbar.Text.Trim();
                tu.tur = OrtakFonksiyonlar.ConvertToInt(ddlTur.SelectedValue.Trim(), 0);
                tu.belgeTarihi = new TNSDateTime(txtBelgeTarih.Text.Trim());
            }
            return tu;
        }

        private void Kaydet(TNS.TMM.TuketimUretim tu)
        {
            fpL.SaveChanges();

            tu.detay = new TNSCollection();
            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                TuketimUretimDetay detay = new TuketimUretimDetay();
                detay.hesapPlani = new HesapPlaniSatir();
                detay.hesapPlani.hesapKod = fpL.Sheets[0].Cells[i, 0].Text.Trim();
                detay.miktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 4].Text.Trim(), 0);

                if (!string.IsNullOrEmpty(detay.hesapPlani.hesapKod))
                {
                    int satir = 1;
                    foreach (TuketimUretimDetay d in tu.detay)
                    {
                        if (d.hesapPlani.hesapKod.Replace(".", "") == detay.hesapPlani.hesapKod.Replace(".", ""))
                        {
                            GenelIslemler.HataYaz(this, string.Format(Resources.TasinirMal.FRMTUG027, (i + 1).ToString(), satir.ToString()));
                            return;
                        }
                        satir++;

                    }
                    tu.detay.Add(detay);
                }
            }

            Sonuc sonuc = servis.TuketimUretimKaydet(kullanan, tu);
            if (!sonuc.islemSonuc)
                GenelIslemler.HataYaz(this, sonuc.hataStr);
            else
            {
                txtBelgeNo.Text = sonuc.anahtar;
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMTUG028);
            }
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla(false));
        }

        private void Listele(TNS.TMM.TuketimUretim tu)
        {
            ObjectArray bilgi = servis.TuketimUretimListele(kullanan, tu, true);
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

            btnTemizle_Click(null, null);

            tu = (TNS.TMM.TuketimUretim)bilgi.objeler[0];
            ddlYil.SelectedValue = tu.yil.ToString();
            txtMuhasebe.Text = tu.muhasebe.kod;
            lblMuhasebeAd.Text = tu.muhasebe.ad;
            txtHarcamaBirimi.Text = tu.harcamaBirimi.kod;
            lblHarcamaBirimiAd.Text = tu.harcamaBirimi.ad;
            txtAmbar.Text = tu.ambar.kod;
            lblAmbarAd.Text = tu.ambar.ad;
            txtBelgeTarih.Text = tu.belgeTarihi.ToString();
            txtBelgeNo.Text = tu.belgeNo;
            ddlTur.SelectedValue = tu.tur.ToString();

            if (tu.detay.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = tu.detay.Count + ekleSatirSayisi;

            int i = 0;
            foreach (TuketimUretimDetay detay in tu.detay)
            {
                fpL.Sheets[0].Cells[i, 0].Text = detay.hesapPlani.hesapKod;
                fpL.Sheets[0].Cells[i, 2].Text = detay.hesapPlani.aciklama;
                fpL.Sheets[0].Cells[i, 3].Text = detay.hesapPlani.olcuBirimAd;
                fpL.Sheets[0].Cells[i, 4].Text = detay.miktar.ToString();
                i++;
            }
        }

        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            ddlYil.SelectedValue = DateTime.Now.Year.ToString();
            txtBelgeTarih.Text = DateTime.Today.ToShortDateString();
            txtBelgeNo.Text = string.Empty;
            ddlTur.SelectedValue = ((int)ENUMTuketimUretim.TUKETIM).ToString();

            fpL.CancelEdit();
            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = string.Empty;
            fpL.Sheets[0].RowCount = ekleSatirSayisi;
        }

        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            TNS.TMM.TuketimUretim tu = KriterTopla(false);
            ObjectArray bilgi = servis.TuketimUretimListele(kullanan, tu, true);
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

            //btnTemizle_Click(null, null);

            tu = (TNS.TMM.TuketimUretim)bilgi.objeler[0];

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TuketimUretim.xlt";
            XLS.DosyaAc(System.Web.HttpContext.Current.Server.MapPath("~") + "/RaporSablon/TMM/" + sablonAd, sonucDosyaAd);


            XLS.HucreAdBulYaz("BIRIMAD", tu.harcamaBirimi.ad);
            XLS.HucreAdBulYaz("MUHASEBEAD", tu.muhasebe.ad);
            XLS.HucreAdBulYaz("AMBARAD", tu.ambar.ad);

            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int satir = kaynakSatir;

            int siraNo = 0;

            if (tu.tur == (int)ENUMTuketimUretim.TUKETIM)
            {
                foreach (TuketimUretimDetay detay in tu.detay)
                {
                    satir++;
                    siraNo++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlani.hesapKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlani.aciklama);
                    XLS.HucreDegerYaz(satir, sutun + 3, detay.miktar.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 4, detay.hesapPlani.olcuBirimAd);
                }
            }
            else if (tu.tur == (int)ENUMTuketimUretim.URETIM)
            {
                foreach (TuketimUretimDetay detay in tu.detay)
                {
                    satir++;
                    siraNo++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 5, detay.hesapPlani.aciklama);
                    XLS.HucreDegerYaz(satir, sutun + 6, detay.miktar.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 7, detay.hesapPlani.olcuBirimAd);
                }
            }

            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        private void BelgeTurDoldur()
        {
            ddlTur.Items.Clear();
            ddlTur.Items.Add(new ListItem(Resources.TasinirMal.FRMTUG020, ((int)ENUMTuketimUretim.TUKETIM).ToString()));
            ddlTur.Items.Add(new ListItem(Resources.TasinirMal.FRMTUG021, ((int)ENUMTuketimUretim.URETIM).ToString()));
        }

        void GridInit(FarPoint.Web.Spread.FpSpread kontrol)
        {
            kontrol.RenderCSSClass = true;
            kontrol.EditModeReplace = true;
            kontrol.ClientAutoCalculation = false;
            //kontrol.EnableClientScript = false;
            kontrol.HierarchicalView = false;
            kontrol.IsPrint = false;

            kontrol.Sheets.Count = 1;
            kontrol.Sheets[0].RowCount = ekleSatirSayisi;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].IsTrackingViewState = true;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 5;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].ColumnSpan = 2;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMTUG016;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMTUG017;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMTUG018;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMTUG022;

            kontrol.Sheets[0].Columns[0, kontrol.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[4].HorizontalAlign = HorizontalAlign.Right;

            kontrol.Sheets[0].Columns[2, 3].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[2, 3].Locked = true;

            kontrol.Sheets[0].Columns[0, kontrol.Sheets[0].ColumnCount - 1].Width = 300;
            kontrol.Sheets[0].Columns[1].Width = 20;
            kontrol.Sheets[0].Columns[3].Width = 100;
            kontrol.Sheets[0].Columns[4].Width = 150;

            TasinirGenel.MyLinkType hesapPlaniLink = new TasinirGenel.MyLinkType("HesapPlaniGoster()");
            hesapPlaniLink.ImageUrl = "../App_themes/images/bul1.gif";
            kontrol.Sheets[0].Columns[1].CellType = hesapPlaniLink;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;

            kontrol.Attributes.Add("onDataChanged", "HucreDegisti(this)");
            GenelIslemlerIstemci.RakamAlanFormatla(kontrol, 4, 4, 2);
        }

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
                img.AlternateText = Resources.TasinirMal.FRMTUG023;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMTUG024;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMTUG025;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMTUG026;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);
            }

            base.Render(writer);
        }

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
            GenelIslemlerIstemci.RakamAlanFormatla(fpL, 4, 5, 2);
            fpL.SaveChanges();
        }

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