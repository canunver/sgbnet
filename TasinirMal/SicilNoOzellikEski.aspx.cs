using System;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr demirbaþlarýnýn özellik bilgilerinin kayýt, listeleme ve silme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class SicilNoOzellikEski : TMMSayfa
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
        ///     Sayfa adresinde gelen girdi dizgileri ilgili kontrollere yazýlýr
        ///     ve kontrollerden bazýlarý bu dizgilere göre gizlenir/gösterilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            GenelIslemlerIstemci.JSResourceEkle(Resources.TasinirMal.ResourceManager, this, "SicilNoOzellik", "FRMJSC011");

            formAdi = Resources.TasinirMal.FRMSCO001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnSil.Attributes.Add("onclick", "return OnayAl('TasinirOzellikSil','btnSil');");

            if (!IsPostBack)
            {
                ViewState["fpID"] = DateTime.Now.ToLongTimeString();

                YilDoldur();
                GridInit(fpL);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                //yil=2009&mb=00007&hbk=12.01.00.23.923&ak=02&belgeNo=&tur=
                if (Request.QueryString["mb"] + "" != "")
                    txtMuhasebe.Text = Request.QueryString["mb"] + "";
                if (Request.QueryString["hbk"] + "" != "")
                    txtHarcamaBirimi.Text = Request.QueryString["hbk"] + "";
                if (Request.QueryString["ak"] + "" != "")
                    txtAmbar.Text = Request.QueryString["ak"] + "";

                txtBelgeTur.Value = "";
                if (Request.QueryString["tur"] + "" == "kutuphane")
                {
                    optMuze.Enabled = false;
                    optTasinir.Enabled = false;
                    txtSicilNo.Text = Convert.ToString((int)ENUMTasinirHesapKodu.KUTUPHANE);
                    txtBelgeTur.Value = "kutuphane";
                    optKutuphane.Checked = true;
                }
                else if (Request.QueryString["tur"] + "" == "muze")
                {
                    optKutuphane.Enabled = false;
                    optTasinir.Enabled = false;
                    txtSicilNo.Text = Convert.ToString((int)ENUMTasinirHesapKodu.MUZE);
                    txtBelgeTur.Value = "muze";
                    optMuze.Checked = true;
                }
                else
                {
                    optMuze.Enabled = false;
                    optKutuphane.Enabled = false;
                    optTasinir.Checked = true;
                }

                if (Request.QueryString["belgeNo"] != null && Request.QueryString["belgeNo"] != "")
                {
                    if (Request.QueryString["yil"] != null && Request.QueryString["yil"] != "")
                        ddlYil.SelectedValue = Request.QueryString["yil"].Trim();
                    txtBelgeNo.Text = Request.QueryString["belgeNo"].Trim();
                    btnAra_Click(null, null);
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
        /// Listeleme kriterleri SicilNoHareket nesnesinde parametre olarak alýnýr,
        /// sunucuya gönderilir ve demirbaþ bilgileri sunucudan alýnýr. Hata varsa ekrana
        /// hata bilgisi yazýlýr, yoksa gelen bilgiler farpoint grid kontrolüne doldurulur.
        /// </summary>
        /// <param name="sicilNo">Demirbaþ listeleme kriterlerini tutan nesne</param>
        private void Listele(SicilNoHareket sicilNo)
        {
            ObjectArray yler = servisTMM.ButunSicilNoListele(kullanan, sicilNo);

            if (yler.sonuc.islemSonuc)
            {
                if (yler.objeler.Count > 0)
                {
                    optTasinir.Enabled = false;
                    optKutuphane.Enabled = false;
                    optMuze.Enabled = false;

                    foreach (TNS.TMM.SicilNoHareket y in yler.objeler)
                    {
                        if (y.sicilNo.StartsWith(Convert.ToString((int)ENUMTasinirHesapKodu.KUTUPHANE)))
                        {
                            txtBelgeTur.Value = "kutuphane";
                            optKutuphane.Checked = true;
                            optKutuphane.Enabled = true;
                            GridInit(fpL);
                            break;
                        }
                        else if (y.sicilNo.StartsWith(Convert.ToString((int)ENUMTasinirHesapKodu.MUZE)))
                        {
                            txtBelgeTur.Value = "muze";
                            optMuze.Checked = true;
                            optMuze.Enabled = true;
                            GridInit(fpL);
                            break;
                        }
                        else
                        {
                            txtBelgeTur.Value = "";
                            optTasinir.Enabled = true;
                            optTasinir.Checked = true;
                            GridInit(fpL);
                            break;
                        }
                    }

                    fpL.Sheets[0].RowCount = yler.objeler.Count;
                    int satir = 0;

                    foreach (TNS.TMM.SicilNoHareket y in yler.objeler)
                    {
                        fpL.Sheets[0].Cells[satir, 0].Text = y.prSicilNo.ToString();
                        fpL.Sheets[0].Cells[satir, 1].Text = y.sicilNo;
                        fpL.Sheets[0].Cells[satir, 2].Text = y.hesapPlanAd;
                        if (y.ozellik.markaKod > 0)
                            fpL.Sheets[0].Cells[satir, 3].Text = y.ozellik.markaKod.ToString() + "-" + y.ozellik.markaAd;
                        if (y.ozellik.modelKod > 0)
                            fpL.Sheets[0].Cells[satir, 5].Text = y.ozellik.modelKod.ToString() + "-" + y.ozellik.modelAd;
                        fpL.Sheets[0].Cells[satir, 7].Text = y.ozellik.saseNo;

                        if (y.sicilNo.StartsWith(Convert.ToString((int)ENUMTasinirHesapKodu.TASIT)))
                        {
                            fpL.Sheets[0].Columns[8, 9].Visible = true;
                            fpL.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMSCO002;

                            fpL.Sheets[0].Cells[satir, 8].Text = y.ozellik.motorNo;
                            fpL.Sheets[0].Cells[satir, 9].Text = y.ozellik.plaka;
                        }

                        //if (txtBelgeTur.Value == "kutuphane" || y.sicilNo.StartsWith(Convert.ToString((int)ENUMTasinirHesapKodu.KUTUPHANE)))
                        //{
                        fpL.Sheets[0].Cells[satir, 10].Text = y.ozellik.disSicilNo;
                        fpL.Sheets[0].Cells[satir, 11].Text = y.ozellik.ciltNo;
                        fpL.Sheets[0].Cells[satir, 12].Text = y.ozellik.dil;
                        fpL.Sheets[0].Cells[satir, 13].Text = y.ozellik.yazarAdi;
                        fpL.Sheets[0].Cells[satir, 14].Text = y.ozellik.adi;
                        fpL.Sheets[0].Cells[satir, 15].Text = y.ozellik.yayinYeri;
                        fpL.Sheets[0].Cells[satir, 16].Text = y.ozellik.yayinTarihi;
                        fpL.Sheets[0].Cells[satir, 17].Text = y.ozellik.neredenGeldi;
                        fpL.Sheets[0].Cells[satir, 18].Text = y.ozellik.boyutlari;
                        fpL.Sheets[0].Cells[satir, 19].Text = y.ozellik.satirSayisi;
                        fpL.Sheets[0].Cells[satir, 20].Text = y.ozellik.yaprakSayisi;
                        fpL.Sheets[0].Cells[satir, 21].Text = y.ozellik.sayfaSayisi;
                        fpL.Sheets[0].Cells[satir, 22].Text = y.ozellik.ciltTuru;
                        fpL.Sheets[0].Cells[satir, 23].Text = y.ozellik.cesidi;
                        fpL.Sheets[0].Cells[satir, 24].Text = y.ozellik.yeriKonusu;
                        //}
                        //else if (txtBelgeTur.Value == "muze" || y.sicilNo.StartsWith(Convert.ToString((int)ENUMTasinirHesapKodu.MUZE)))
                        //{
                        fpL.Sheets[0].Cells[satir, 25].Text = y.ozellik.disSicilNo;
                        fpL.Sheets[0].Cells[satir, 26].Text = y.ozellik.adi;
                        fpL.Sheets[0].Cells[satir, 27].Text = y.ozellik.gelisTarihi;
                        fpL.Sheets[0].Cells[satir, 28].Text = y.ozellik.neredenGeldi;
                        fpL.Sheets[0].Cells[satir, 29].Text = y.ozellik.neredeBulundu;
                        fpL.Sheets[0].Cells[satir, 30].Text = y.ozellik.cagi;
                        fpL.Sheets[0].Cells[satir, 31].Text = y.ozellik.boyutlari;
                        fpL.Sheets[0].Cells[satir, 32].Text = y.ozellik.durumuMaddesi;
                        fpL.Sheets[0].Cells[satir, 33].Text = y.ozellik.onYuz;
                        fpL.Sheets[0].Cells[satir, 34].Text = y.ozellik.arkaYuz;
                        fpL.Sheets[0].Cells[satir, 35].Text = y.ozellik.puan;
                        fpL.Sheets[0].Cells[satir, 36].Text = y.ozellik.yeriKonusu;
                        //}

                        satir++;
                    }
                }
                else
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMSCO003);
            }
            else
                GenelIslemler.MesajKutusu(this, yler.sonuc.hataStr);
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandaki kriterler toplanýr ve Listele yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, System.EventArgs e)
        {
            GridInit(fpL);

            SicilNoHareket y = new SicilNoHareket();
            y.muhasebeKod = txtMuhasebe.Text.Trim();
            y.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            y.ambarKod = txtAmbar.Text.Replace(".", "");
            y.sicilNo = txtSicilNo.Text.Trim().Replace(".", "");
            if (txtBelgeNo.Text.Trim() != "")
            {
                y.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
                y.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
            }

            string hata = "";
            if (y.muhasebeKod == "")
                hata += Resources.TasinirMal.FRMSCO004;
            if (y.harcamaBirimKod == "")
                hata += Resources.TasinirMal.FRMSCO005;
            if (y.ambarKod == "")
                hata += Resources.TasinirMal.FRMSCO006;
            if (y.sicilNo == "" && y.fisNo == "")
                hata += Resources.TasinirMal.FRMSCO007;

            if (hata != "")
                GenelIslemler.MesajKutusu(this, hata);
            else
                Listele(y);
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

            fpL.Sheets[0].RowCount = 0;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;
            kontrol.CommandBar.Visible = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 37;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMSCO008;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMSCO009;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMSCO010;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMSCO011;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMSCO012;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMSCO013;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 9].Value = Resources.TasinirMal.FRMSCO014;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 10].Value = Resources.TasinirMal.FRMSCO015;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 11].Value = Resources.TasinirMal.FRMSCO016;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 12].Value = Resources.TasinirMal.FRMSCO017;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 13].Value = Resources.TasinirMal.FRMSCO018;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 14].Value = Resources.TasinirMal.FRMSCO019;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 15].Value = Resources.TasinirMal.FRMSCO020;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 16].Value = Resources.TasinirMal.FRMSCO021;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 17].Value = Resources.TasinirMal.FRMSCO022;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 18].Value = Resources.TasinirMal.FRMSCO023;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 19].Value = Resources.TasinirMal.FRMSCO024;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 20].Value = Resources.TasinirMal.FRMSCO025;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 21].Value = Resources.TasinirMal.FRMSCO026;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 22].Value = Resources.TasinirMal.FRMSCO027;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 23].Value = Resources.TasinirMal.FRMSCO028;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 24].Value = Resources.TasinirMal.FRMSCO029;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 25].Value = Resources.TasinirMal.FRMSCO030;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 26].Value = Resources.TasinirMal.FRMSCO031;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 27].Value = Resources.TasinirMal.FRMSCO032;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 28].Value = Resources.TasinirMal.FRMSCO033;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 29].Value = Resources.TasinirMal.FRMSCO034;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 30].Value = Resources.TasinirMal.FRMSCO035;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 31].Value = Resources.TasinirMal.FRMSCO036;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 32].Value = Resources.TasinirMal.FRMSCO037;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 33].Value = Resources.TasinirMal.FRMSCO038;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 34].Value = Resources.TasinirMal.FRMSCO039;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 35].Value = Resources.TasinirMal.FRMSCO040;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 36].Value = Resources.TasinirMal.FRMSCO041;

            kontrol.Sheets[0].Columns[0].Width = 0;
            kontrol.Sheets[0].Columns[1].Width = 130;
            kontrol.Sheets[0].Columns[2].Width = 110;
            kontrol.Sheets[0].Columns[3].Width = 110;
            kontrol.Sheets[0].Columns[4].Width = 30;
            kontrol.Sheets[0].Columns[5].Width = 110;
            kontrol.Sheets[0].Columns[6].Width = 30;
            kontrol.Sheets[0].Columns[7].Width = 130;
            kontrol.Sheets[0].Columns[8, 36].Width = 100;

            TasinirGenel.MyLinkType markaLink = new TasinirGenel.MyLinkType("MarkaGoster()");
            markaLink.ImageUrl = "../App_themes/images/bul1.gif";

            TasinirGenel.MyLinkType modelLink = new TasinirGenel.MyLinkType("ModelGoster()");
            modelLink.ImageUrl = "../App_themes/images/bul1.gif";

            kontrol.Sheets[0].Columns[4].CellType = markaLink;
            kontrol.Sheets[0].Columns[6].CellType = modelLink;

            kontrol.Sheets[0].Columns[0, 2].Locked = true;
            kontrol.Sheets[0].Columns[3].Locked = true;
            kontrol.Sheets[0].Columns[5].Locked = true;

            kontrol.Sheets[0].Columns[0, 2].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[3].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[5].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[0, 0].ForeColor = System.Drawing.Color.LightGoldenrodYellow;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0, 3].CellType = cTextType;
            kontrol.Sheets[0].Columns[7, 36].CellType = cTextType;

            kontrol.Sheets[0].Columns[0, fpL.Sheets[0].Columns.Count - 1].Visible = true;
            if (txtBelgeTur.Value == "kutuphane")
            {
                kontrol.Sheets[0].Columns[3, 9].Visible = false;
                kontrol.Sheets[0].Columns[25, fpL.Sheets[0].Columns.Count - 1].Visible = false;
            }
            else if (txtBelgeTur.Value == "muze")
                kontrol.Sheets[0].Columns[3, 24].Visible = false;
            else
                kontrol.Sheets[0].Columns[8, fpL.Sheets[0].Columns.Count - 1].Visible = false;
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Demirbaþlara ait özellik bilgileri toplanýr ve kaydedilmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata varsa hata görüntülenir, yoksa bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            fpL.SaveChanges();
            ObjectArray o = new ObjectArray();

            int satir = 0;

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                TNS.TMM.SicilNoOzellik s = new TNS.TMM.SicilNoOzellik();

                int prSicilNo = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 0].Text, 0);

                if (chkKaydet.Checked)
                    satir = 0;
                else
                    satir = i;

                s.prSicilno = prSicilNo;
                //Eðer disSicil envannter giriþinde verilmiþ ve özellik giriþi yapýlýyor ise dýssicil kütüphane ve müze dýþýnda siliniyordu.KAYDETME ile serverda update edilmemesi saðlandý.
                s.disSicilNo = "KAYDETME";

                if (txtBelgeTur.Value == "kutuphane")
                {
                    s.disSicilNo = fpL.Sheets[0].Cells[satir, 10].Text.Trim();
                    s.ciltNo = fpL.Sheets[0].Cells[satir, 11].Text.Trim();
                    s.dil = fpL.Sheets[0].Cells[satir, 12].Text.Trim();
                    s.yazarAdi = fpL.Sheets[0].Cells[satir, 13].Text.Trim();
                    s.adi = fpL.Sheets[0].Cells[satir, 14].Text.Trim();
                    s.yayinYeri = fpL.Sheets[0].Cells[satir, 15].Text.Trim();
                    s.yayinTarihi = fpL.Sheets[0].Cells[satir, 16].Text.Trim();
                    s.neredenGeldi = fpL.Sheets[0].Cells[satir, 17].Text.Trim();
                    s.boyutlari = fpL.Sheets[0].Cells[satir, 18].Text.Trim();
                    s.satirSayisi = fpL.Sheets[0].Cells[satir, 19].Text.Trim();
                    s.yaprakSayisi = fpL.Sheets[0].Cells[satir, 20].Text.Trim();
                    s.sayfaSayisi = fpL.Sheets[0].Cells[satir, 21].Text.Trim();
                    s.ciltTuru = fpL.Sheets[0].Cells[satir, 22].Text.Trim();
                    s.cesidi = fpL.Sheets[0].Cells[satir, 23].Text.Trim();
                    s.yeriKonusu = fpL.Sheets[0].Cells[satir, 24].Text.Trim();
                }
                else if (txtBelgeTur.Value == "muze")
                {
                    s.disSicilNo = fpL.Sheets[0].Cells[satir, 25].Text.Trim();
                    s.adi = fpL.Sheets[0].Cells[satir, 26].Text.Trim();
                    s.gelisTarihi = fpL.Sheets[0].Cells[satir, 27].Text.Trim();
                    s.neredenGeldi = fpL.Sheets[0].Cells[satir, 28].Text.Trim();
                    s.neredeBulundu = fpL.Sheets[0].Cells[satir, 29].Text.Trim();
                    s.cagi = fpL.Sheets[0].Cells[satir, 30].Text.Trim();
                    s.boyutlari = fpL.Sheets[0].Cells[satir, 31].Text.Trim();
                    s.durumuMaddesi = fpL.Sheets[0].Cells[satir, 32].Text.Trim();
                    s.onYuz = fpL.Sheets[0].Cells[satir, 33].Text.Trim();
                    s.arkaYuz = fpL.Sheets[0].Cells[satir, 34].Text.Trim();
                    s.puan = fpL.Sheets[0].Cells[satir, 35].Text.Trim();
                    s.yeriKonusu = fpL.Sheets[0].Cells[satir, 36].Text.Trim();
                }
                //else
                //{
                string marka = fpL.Sheets[0].Cells[satir, 3].Text;
                if (marka != "")
                {
                    marka = marka.Split('-')[0];
                    s.markaKod = OrtakFonksiyonlar.ConvertToInt(marka, 0);
                }
                string model = fpL.Sheets[0].Cells[satir, 5].Text;
                if (model != "")
                {
                    model = model.Split('-')[0];
                    s.modelKod = OrtakFonksiyonlar.ConvertToInt(model, 0);
                }
                s.saseNo = fpL.Sheets[0].Cells[satir, 7].Text.Trim();
                s.motorNo = fpL.Sheets[0].Cells[satir, 8].Text.Trim();
                s.plaka = fpL.Sheets[0].Cells[satir, 9].Text.Trim();
                //}

                o.objeler.Add(s);
            }

            Sonuc sonuc = servisTMM.SicilNoOzellikKaydet(kullanan, 0, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), o);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMSCO042);
                btnAra_Click(null, null);
            }
            else
                GenelIslemler.MesajKutusu(this, string.Format(Resources.TasinirMal.FRMSCO043, sonuc.hataStr));
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

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Demirbaþlara ait özellik bilgileri silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata varsa hata görüntülenir, yoksa bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, EventArgs e)
        {
            int[] prSicilNolar = new int[fpL.Sheets[0].RowCount];

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                prSicilNolar[i] = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 0].Text, 0);
            }

            Sonuc sonuc = servisTMM.SicilNoOzellikSil(kullanan, 0, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), prSicilNolar);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMSCO044);
                btnAra_Click(null, null);
            }
            else
                GenelIslemler.MesajKutusu(this, string.Format(Resources.TasinirMal.FRMSCO045, sonuc.hataStr));
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        protected void btnSeriNoYukle_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (fupSeriNo.PostedFile == null || fupSeriNo.PostedFile.ContentLength <= 0)
                {
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG078);
                    return;
                }

                byte[] myData = new byte[fupSeriNo.PostedFile.ContentLength];
                fupSeriNo.PostedFile.InputStream.Read(myData, 0, fupSeriNo.PostedFile.ContentLength);

                string dosyaAd = System.IO.Path.GetTempFileName();
                if (string.IsNullOrEmpty(dosyaAd))
                {
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG077);
                    return;
                }

                System.IO.FileStream newFile = new System.IO.FileStream(dosyaAd, System.IO.FileMode.Create);
                newFile.Write(myData, 0, myData.Length);
                newFile.Close();

                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(dosyaAd);
                for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
                {
                    string hucreBilgi = XLS.HucreDegerAl(i, 0);
                    if (string.IsNullOrEmpty(hucreBilgi))
                        break;
                    fpL.Sheets[0].Cells[i, 7].Text = hucreBilgi;
                }

                XLS.DosyaKapat();
                System.IO.File.Delete(dosyaAd);

                btnKaydet_Click(null, null);

            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu("Uyarý", ex.Message);
                fpL.CancelEdit();
            }
        }
    }
}