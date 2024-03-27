using System;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class TekrarEdenSicil : TMMSayfa
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            //TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMTES007;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            //this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");

            if (!IsPostBack)
            {
                ViewState["fpID"] = DateTime.Now.ToLongTimeString();

                YilDoldur();
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

        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlBelgeYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        private void DegiskenSakla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            fpL.SaveChanges();

            TNSCollection detaylar = new TNSCollection();
            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                if (fpL.Sheets[0].Cells[i, 1].Value == null || fpL.Sheets[0].Cells[i, 1].Value.ToString() != "1")
                    continue;

                TNS.TMM.SicilNoHareket detay = new TNS.TMM.SicilNoHareket();
                detay.prSicilNo = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 0].Text.Trim(), 0);
                detay.sicilNo = fpL.Sheets[0].Cells[i, 2].Text.Trim();
                detay.muhasebeKod = fpL.Sheets[0].Cells[i, 6].Text.Trim().Split('-')[0].Trim();
                detay.harcamaBirimKod = fpL.Sheets[0].Cells[i, 7].Text.Trim().Split('-')[0].Trim();
                detay.ambarKod = fpL.Sheets[0].Cells[i, 8].Text.Trim().Split('-')[0].Trim();

                detaylar.Add(detay);
            }

            if (detaylar.Count <= 0)
                return;

            Sonuc sonuc = servisTMM.SicilNoGuncelle(kullanan, (SicilNoHareket[])detaylar.ToArray(typeof(SicilNoHareket)));
            if (sonuc.islemSonuc)
            {
                btnListele_Click(null, null);
                GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
            }
            else
                GenelIslemler.HataYaz(this, sonuc.hataStr);
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            GridInit(fpL);

            SicilNoHareket kriter = new SicilNoHareket();
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.sicilNo = txtSicilNo.Text.Trim();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlBelgeYil.SelectedValue.Trim(), 0);
            kriter.fisNo = !string.IsNullOrEmpty(txtBelgeNo.Text.Trim()) ? txtBelgeNo.Text.Trim().PadLeft(6, '0') : string.Empty;

            Listele(kriter);
        }

        private void Temizle()
        {
            fpL.CancelEdit();
            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = string.Empty;
        }

        private void Listele(SicilNoHareket kriter)
        {
            Temizle();
        
            ObjectArray siciller = servisTMM.TekrarEdenSicilleriBul(kullanan, kriter);
            if (!siciller.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, siciller.sonuc.hataStr);
                return;
            }
            if (siciller.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, siciller.sonuc.bilgiStr);
                return;
            }

            if (siciller.objeler.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = siciller.objeler.Count;

            int sayac = 0;
            foreach (SicilNoHareket sicil in siciller.objeler)
            {
                fpL.Sheets[0].Cells[sayac, 0].Text = sicil.prSicilNo.ToString();
                fpL.Sheets[0].Cells[sayac, 1].Value = "0";
                fpL.Sheets[0].Cells[sayac, 2].Text = sicil.sicilNo;
                fpL.Sheets[0].Cells[sayac, 4].Text = sicil.hesapPlanAd;
                fpL.Sheets[0].Cells[sayac, 5].Text = sicil.fiyat.ToString("#,###.00");
                fpL.Sheets[0].Cells[sayac, 6].Text = sicil.muhasebeKod + " - " + sicil.muhasebeAd;
                fpL.Sheets[0].Cells[sayac, 7].Text = sicil.harcamaBirimKod + " - " + sicil.harcamaBirimAd;
                fpL.Sheets[0].Cells[sayac, 8].Text = sicil.ambarKod + " - " + sicil.ambarAd;

                TasinirGenel.MyLinkType sicilNoLink = new TasinirGenel.MyLinkType("SicilNoTarihceAc('" + sicil.prSicilNo.ToString() + "')");
                sicilNoLink.ImageUrl = "../App_themes/images/bul1.gif";
                fpL.Sheets[0].Cells[sayac, 3].CellType = sicilNoLink;

                sayac++;
            }
        }

        void GridInit(FarPoint.Web.Spread.FpSpread kontrol)
        {
            kontrol.RenderCSSClass = true;
            kontrol.EditModeReplace = true;

            kontrol.Sheets.Count = 1;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 9;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].ColumnSpan = 2;

            kontrol.Sheets[0].RowCount = 10;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 1].Value = Resources.TasinirMal.FRMTES015;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMTES016;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMTES017;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMTES021;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMTES018;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMTES019;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMTES020;

            kontrol.Sheets[0].Columns[0, kontrol.Sheets[0].ColumnCount - 1].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[2].Locked = true;
            kontrol.Sheets[0].Columns[4, kontrol.Sheets[0].ColumnCount - 1].Locked = true;
            kontrol.Sheets[0].Columns[0].Visible = false;

            kontrol.Sheets[0].Columns[5].HorizontalAlign = HorizontalAlign.Right;

            kontrol.Sheets[0].Columns[0, kontrol.Sheets[0].ColumnCount - 1].Width = 200;
            kontrol.Sheets[0].Columns[1].Width = 20;
            kontrol.Sheets[0].Columns[3].Width = 20;
            kontrol.Sheets[0].Columns[5].Width = 100;

            FarPoint.Web.Spread.CheckBoxCellType cChkType = new FarPoint.Web.Spread.CheckBoxCellType();
            kontrol.Sheets[0].Columns[1].CellType = cChkType;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[2, kontrol.Sheets[0].ColumnCount - 1].CellType = cTextType;

            TasinirGenel.MyLinkType sicilNoLink = new TasinirGenel.MyLinkType();
            sicilNoLink.ImageUrl = "../App_themes/images/bul1.gif";
            kontrol.Sheets[0].Columns[3].CellType = sicilNoLink;

            GenelIslemlerIstemci.RakamAlanFormatla(kontrol, 5, 5, 2);
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