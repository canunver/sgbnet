using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// RFID entegrasyonu olan kurumlarda taşınır demirbaş malzemeleriyle
    /// RFID bilgilerinin eşleştirme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class RfIdEsleme : TMMSayfa
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

            formAdi = Resources.TasinirMal.FRMRFE001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");

            if (!IsPostBack)
            {
                GenelIslemler.YilDoldur(ddlBelgeYil, 2007, DateTime.Now.Year, DateTime.Now.Year);

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
        /// Ekrandan seçilmiş olan muhasebe birimi, harcama birimi ve ambar
        /// bilgileri işlem yapan kullanıcının değişken listesine saklanır.
        /// </summary>
        private void DegiskenSakla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan KriterTopla yordamı çağırılır ve toplanan kriterler
        /// listeleme işlemini yapan Listele yordamına gönderilir, böylece listeleme işlemi yapılmış olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Listeleme kriterleri SicilNoHareket nesnesinde parametre olarak alınır, sunucuya gönderilir
        /// ve kriterlere uygun olan demirbaş bilgileri sunucudan alınır. Hata varsa ekrana hata
        /// bilgisi yazılır, yoksa gelen demirbaş bilgileri EkranaYaz yordamına gönderilir.
        /// </summary>
        /// <param name="sh">Demirbaş listeleme kriterlerini tutan nesne</param>
        private void Listele(SicilNoHareket sh)
        {
            ObjectArray sicilVAN = servisTMM.SicilListeleRFIDEslesmeyen(kullanan, sh);

            Temizle();

            if (!sicilVAN.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", sicilVAN.sonuc.hataStr);
                return;
            }

            if (sicilVAN.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", sicilVAN.sonuc.bilgiStr);
                return;
            }

            EkranaYaz(sicilVAN);
        }

        /// <summary>
        /// Parametre olarak verilen demirbaş bilgilerini ekrana yazan yordam
        /// </summary>
        /// <param name="sicilVAN">Demirbaş bilgilerini tutan nesne</param>
        private void EkranaYaz(ObjectArray sicilVAN)
        {
            fpL.Sheets[0].RowCount = sicilVAN.objeler.Count;

            int sayac = 0;
            foreach (TNS.TMM.SicilNoHareket detay in sicilVAN.objeler)
            {
                fpL.Sheets[0].Cells[sayac, 0].Text = detay.sicilNo;
                fpL.Sheets[0].Cells[sayac, 1].Text = detay.hesapPlanKod;
                fpL.Sheets[0].Cells[sayac, 2].Text = detay.hesapPlanAd;
                fpL.Sheets[0].Cells[sayac, 3].Text = detay.odaAd;
                fpL.Sheets[0].Cells[sayac, 4].Text = detay.kisiAd;

                string ozellik = detay.ozellik.markaAd;
                if (!string.IsNullOrEmpty(detay.ozellik.modelAd))
                {
                    if (!string.IsNullOrEmpty(ozellik))
                        ozellik += " - ";
                    ozellik += detay.ozellik.modelAd;
                }
                if (!string.IsNullOrEmpty(detay.ozellik.saseNo))
                {
                    if (!string.IsNullOrEmpty(ozellik))
                        ozellik += " - ";
                    ozellik += detay.ozellik.saseNo;
                }
                fpL.Sheets[0].Cells[sayac, 5].Text = ozellik;

                fpL.Sheets[0].Cells[sayac, 6].Text = detay.kdvliBirimFiyat.ToString("#,###.000000000000");
                fpL.Sheets[0].Cells[sayac, 7].Text = detay.rfIdNo.ToString();
                fpL.Sheets[0].Cells[sayac, 8].Text = detay.prSicilNo.ToString();

                sayac++;
            }
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// Demirbaş ve RFID eşleşme bilgileri kaydedilmek üzere sunucuya
        /// gönderilir, gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            TNSCollection rfIdler = KriterToplaDetay();

            Sonuc sonuc = servisTMM.SicilRFIDKaydet(kullanan, rfIdler, false);
            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMRFE002);
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden demirbaş kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Demirbaş kriter bilgileri döndürülür.</returns>
        private SicilNoHareket KriterTopla()
        {
            SicilNoHareket sh = new SicilNoHareket();
            sh.muhasebeKod = txtMuhasebe.Text.Trim();
            sh.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            sh.ambarKod = txtAmbar.Text.Trim();
            sh.yil = OrtakFonksiyonlar.ConvertToInt(ddlBelgeYil.SelectedValue.Trim(), 0);
            sh.fisNo = !string.IsNullOrEmpty(txtBelgeNo.Text.Trim()) ? txtBelgeNo.Text.Trim().PadLeft(6, '0') : string.Empty;
            sh.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            return sh;
        }

        /// <summary>
        /// Sayfadaki grid kontrolündeki demirbaş ve RFID eşleşme bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Demirbaş ve RFID eşleşme bilgileri döndürülür.</returns>
        private TNSCollection KriterToplaDetay()
        {
            TNSCollection rfIdler = new TNSCollection();

            fpL.SaveChanges();
            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                TNS.TMM.SicilRFID detay = new TNS.TMM.SicilRFID();
                detay.muhasebeKod = txtMuhasebe.Text.Trim();
                detay.harcamaKod = txtHarcamaBirimi.Text.Trim();
                detay.ambarKod = txtAmbar.Text.Trim();

                detay.prSicilNo = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 8].Text.Trim(), 0);
                detay.rfIdNo = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 7].Text.Trim(), 0);

                if (detay.rfIdNo > 0)
                    rfIdler.Add(detay);
            }

            return rfIdler;
        }

        /// <summary>
        /// Sayfadaki kontrollere yazılmış bilgilerin temizlenmesini sağlar.
        /// </summary>
        private void Temizle()
        {
            fpL.CancelEdit();
            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = "";

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
                fpL.Sheets[0].Cells[i, 0, i, 1].CellType = new FarPoint.Web.Spread.TextCellType();
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

            fpL.Sheets[0].RowCount = 20;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 9;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMRFE003;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 1].Value = Resources.TasinirMal.FRMRFE004;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMRFE005;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMRFE006;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMRFE007;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMRFE008;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMRFE009;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMRFE010;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = "PrSicil No";
            kontrol.Sheets[0].Columns[8].Visible = false;

            kontrol.Sheets[0].Columns[0, 5].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[6, 7].HorizontalAlign = HorizontalAlign.Right;

            kontrol.Sheets[0].Columns[0, 6].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[0, 6].Locked = true;

            kontrol.Sheets[0].Columns[0].Width = 120;
            kontrol.Sheets[0].Columns[1].Width = 90;
            kontrol.Sheets[0].Columns[2].Width = 120;
            kontrol.Sheets[0].Columns[3, 7].Width = 80;
            kontrol.Sheets[0].Columns[5].Width = 120;
            kontrol.Sheets[0].Columns[6].Width = 70;
            kontrol.Sheets[0].Columns[7].Width = 170;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0, 1].CellType = cTextType;
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

            base.Render(writer);
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