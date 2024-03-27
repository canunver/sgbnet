using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.DEG;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Demirbaþ sicil bilgilerinin barkoda yazdýrýlmasý iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class BarkodYazdirEski : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();
        /// <summary>
        /// Uzaylar servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Degisken.xml dosyasýndaki deðiþkenlere ulaþan servis
        /// </summary>
        IDEGServis servisDEG = TNS.DEG.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur.
        ///     Kullanýcýnýn deðiþken listesinden alýnan deðiþken deðerleriyle, sayfa adresinde gelen
        ///     girdi dizgileri kullanýlarak sayfa ayarlanýr ve ilgili demirbaþ listeleme yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMBRK001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtKimeGitti.Attributes.Add("onblur", "kodAdGetir('36','lblKimeGittiAd',true,new Array('txtKimeGitti'),'KONTROLDENOKU');");
            this.txtNereyeGitti.Attributes.Add("onblur", "kodAdGetir('35','lblNereyeGittiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtNereyeGitti'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                hdnRFIDYaziciURL.Value = servisDEG.DegiskenDegerBul(0, "/RFIDYaziciURL");
                if (!string.IsNullOrEmpty(hdnRFIDYaziciURL.Value))
                    btnRFIDYazdir.Visible = true;

                YilDoldur();

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                hdnYazilimAdi.Value = System.Configuration.ConfigurationManager.AppSettings.Get("yazilimAd");
                rdZebra.Checked = true;

                string barkodYazici = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETYAZICI");
                string barkodYaziBuyuklugu = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETFONT");
                txtYukseklik.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETYUK");
                txtGenislik.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETGEN");
                txtSoldanBosluk.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETSOL");
                txtUsttenBosluk.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETUST");

                string yeniBarkod = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODYENI");
                if (yeniBarkod == "1")
                {
                    chkYeni.Checked = true;
                    ClientScript.RegisterStartupScript(this.GetType(), "tasarim", "YeniGG();TasarimGG();", true);
                }

                if (barkodYazici == "zebra") rdZebra.Checked = true;
                else if (barkodYazici == "argox") rdArgox.Checked = true;
                else if (barkodYazici == "datamax") rdDatamax.Checked = true;

                if (barkodYaziBuyuklugu == "1") rdBarkodCokKucuk.Checked = true;
                else if (barkodYaziBuyuklugu == "2") rdBarkodKucuk.Checked = true;
                else if (barkodYaziBuyuklugu == "3") rdBarkodNormal.Checked = true;
                else rdBarkodBuyuk.Checked = true;

                string data = "";
                for (int i = 0; i < 20; i++)
                {
                    data = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETDATA" + i);
                    if (data == "")
                        break;
                    txtData.Text += data;
                }

                string tasarim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETASARIM");
                if (tasarim == "1")
                {
                    chkTasarim.Checked = true;
                    ClientScript.RegisterStartupScript(this.GetType(), "tasarim", "YeniGG();TasarimGG();", true);
                }
                //if (bYaziciTip == "zebra")
                //    rdZebra.Checked = true;
                //else if (bYaziciTip == "bixolon")
                //    rdBixolon.Checked = true;
                //else if (bYaziciTip == "datamax")
                //    rdDatamax.Checked = true;
                //else if (bYaziciTip == "diger")
                //{
                //    rdDiger.Checked = true;
                //    txtYaziciAdi.Value = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BYAZICIAD");
                //}

                //txtbEn.Value = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BETIKETEN");

                string bZimmetEserBilgi = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BZIMMETESERBILGI");

                if (bZimmetEserBilgi == "zimmeteser")
                {
                    chkZimmetBilgi.Checked = true;
                    chkEserBilgi.Checked = true;
                }
                else if (bZimmetEserBilgi == "zimmet")
                    chkZimmetBilgi.Checked = true;
                else if (bZimmetEserBilgi == "eser")
                    chkEserBilgi.Checked = true;

                //string bBarkodBoyut = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BBARKODBOYUT");

                //if (bBarkodBoyut == "kucuk")
                //    rdBarkodKucuk.Checked = true;
                //else if (bBarkodBoyut == "buyuk")
                //    rdBarkodBuyuk.Checked = true;
                //else rdBarkodNormal.Checked = true;


                bool bTif = false;
                bool bZim = false;
                bool bBos = true;

                if (Request.QueryString["y"] != null)
                {
                    if (Request.QueryString["m"] != null)
                        txtMuhasebe.Text = Request.QueryString["m"].ToString();
                    if (Request.QueryString["h"] != null)
                        txtHarcamaBirimi.Text = Request.QueryString["h"].ToString();
                    if (Request.QueryString["a"] != null)
                        txtAmbar.Text = Request.QueryString["a"].ToString();

                    if (Request.QueryString["bTur"] != null)
                    {
                        string bTur = Request.QueryString["bTur"];

                        if (bTur == "TIF")
                        {
                            bTif = true;
                            if (Request.QueryString["b"] != null)
                                txtBelgeNo.Text = Request.QueryString["b"].ToString();
                        }
                        else if (bTur == "ZIM")
                        {
                            bZim = false;
                            rdKisi.Checked = true;
                            if (Request.QueryString["b"] != null)
                                txtZimmetBelgeNo.Text = Request.QueryString["b"].ToString();
                        }
                        else if (bTur == "ORT")
                        {
                            bZim = true;
                            rdOrtak.Checked = true;
                            if (Request.QueryString["b"] != null)
                                txtZimmetBelgeNo.Text = Request.QueryString["b"].ToString();
                        }
                    }
                }
                else
                    bBos = true;

                if (bTif)
                    KriterTopla();
                else if (bZim)
                    KriterToplaZimmet(bZim);
                else if (!bZim && !bBos)
                    KriterToplaZimmet(bZim);
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

            if (txtKimeGitti.Text.Trim() != "")
                lblKimeGittiAd.Text = GenelIslemler.KodAd(36, txtKimeGitti.Text.Trim(), true);
            else
                lblKimeGittiAd.Text = "";

            if (txtNereyeGitti.Text.Trim() != "")
                lblNereyeGittiAd.Text = GenelIslemler.KodAd(35, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtNereyeGitti.Text.Trim(), true);
            else
                lblNereyeGittiAd.Text = "";
        }

        /// <summary>
        /// Demirbaþ listeleme kriterleri ekrandaki kontrollerden toplanýr ve SicilNumarasiDoldur yordamý çaðýrýlýr.
        /// </summary>
        private void KriterTopla()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);

            shBilgi.muhasebeKod = txtMuhasebe.Text.Trim();

            shBilgi.harcamaBirimKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");

            shBilgi.ambarKod = txtAmbar.Text.Trim();

            shBilgi.sicilNo = txtSicilNo.Text.Trim().Replace(".", "");

            shBilgi.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');

            shBilgi.kimeGitti = txtKimeGitti.Text.Trim();

            shBilgi.nereyeGitti = txtNereyeGitti.Text.Trim();

            shBilgi.ozellik.adi = txtEserAdi.Text.Trim();

            SicilNumarasiDoldur(shBilgi, null);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaþlarý sunucudan alýp gvSicilNo GridView kontrolüne dolduran yordam
        /// shBilgi parametresi dolu ise sunucunun BarkodSicilNoListele yordamý, zim parametresi dolu
        /// ise sunucunun ZimmetFisiAc yordamý çaðýrýlýr ve gelen demirbaþ bilgileri ekrana yazýlýr.
        /// </summary>
        /// <param name="shBilgi">Ambardaki demirbaþlarý listeleme kriterlerini tutan nesne</param>
        /// <param name="zim">Zimmetteki demirbaþlarý listeleme kriterlerini tutan nesne</param>
        private void SicilNumarasiDoldur(SicilNoHareket shBilgi, TNS.TMM.ZimmetForm zim)
        {
            ObjectArray bilgi = new ObjectArray();

            if (shBilgi != null)
                bilgi = servis.BarkodSicilNoListele(kullanan, shBilgi);
            else if (zim != null)
                bilgi = servis.ZimmetFisiAc(kullanan, zim);

            if (bilgi.sonuc.islemSonuc)
            {
                TNSCollection prSiciller = new TNSCollection();
                if (bilgi.objeler.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("sicilno");
                    dt.Columns.Add("kod");
                    dt.Columns.Add("ad");
                    dt.Columns.Add("kimeGitti");
                    dt.Columns.Add("eserBilgi");
                    dt.Columns.Add("eSicilNo");

                    if (shBilgi != null)
                    {
                        foreach (SicilNoHareket sh in bilgi.objeler)
                        {
                            string ozellik = "";
                            string eserBilgi = "";

                            if (!String.IsNullOrEmpty(sh.ozellik.markaAd))
                                ozellik = sh.ozellik.markaAd;
                            if (!String.IsNullOrEmpty(sh.ozellik.modelAd))
                            {
                                if (ozellik != "") ozellik += "-";
                                ozellik += sh.ozellik.modelAd;
                            }
                            if (!String.IsNullOrEmpty(sh.ozellik.plaka))
                            {
                                if (ozellik != "") ozellik += "-";
                                ozellik += sh.ozellik.plaka;
                            }
                            if (!String.IsNullOrEmpty(sh.ozellik.adi))
                            {
                                if (ozellik != "") ozellik += "-";
                                ozellik += sh.ozellik.adi;
                            }
                            if (!String.IsNullOrEmpty(sh.ozellik.disSicilNo))
                            {
                                if (ozellik != "") ozellik += "-";
                                ozellik += sh.ozellik.disSicilNo;
                            }
                            if (ozellik != "") ozellik = " (" + ozellik + ")";


                            if (!String.IsNullOrEmpty(sh.ozellik.yazarAdi))
                                eserBilgi = sh.ozellik.yazarAdi;
                            if (!String.IsNullOrEmpty(sh.ozellik.ciltNo))
                            {
                                if (eserBilgi != "") eserBilgi += "-";
                                eserBilgi += sh.ozellik.ciltNo;
                            }
                            if (!String.IsNullOrEmpty(sh.ozellik.yayinTarihi))
                            {
                                if (eserBilgi != "") eserBilgi += "-";
                                eserBilgi += sh.ozellik.yayinTarihi;
                            }
                            if (!String.IsNullOrEmpty(sh.ozellik.yeriKonusu))
                            {
                                if (eserBilgi != "") eserBilgi += "-";
                                eserBilgi += sh.ozellik.yeriKonusu;
                            }
                            if (!String.IsNullOrEmpty(sh.ozellik.saseNo))
                            {
                                if (eserBilgi != "") eserBilgi += "-";
                                eserBilgi += sh.ozellik.saseNo;
                            }
                            dt.Rows.Add(sh.sicilNo, sh.hesapPlanKod, sh.hesapPlanAd + " " + ozellik, sh.kimeGitti, eserBilgi, sh.ozellik.disSicilNo);
                            prSiciller.Add(sh.prSicilNo);
                        }
                    }
                    else if (zim != null)
                    {
                        foreach (TNS.TMM.ZimmetForm zimmet in bilgi.objeler)
                        {
                            ObjectArray zimmetDetay = servis.ZimmetFisiDetayListele(kullanan, zim);
                            foreach (TNS.TMM.ZimmetDetay zd in zimmetDetay.objeler)
                            {
                                string eserBilgi = "";

                                if (!String.IsNullOrEmpty(zd.ozellikSicil.yazarAdi))
                                    eserBilgi = zd.ozellikSicil.yazarAdi;
                                if (!String.IsNullOrEmpty(zd.ozellikSicil.ciltNo))
                                {
                                    if (eserBilgi != "") eserBilgi += "-";
                                    eserBilgi += zd.ozellikSicil.ciltNo;
                                }
                                if (!String.IsNullOrEmpty(zd.ozellikSicil.yayinTarihi))
                                {
                                    if (eserBilgi != "") eserBilgi += "-";
                                    eserBilgi += zd.ozellikSicil.yayinTarihi;
                                }
                                if (!String.IsNullOrEmpty(zd.ozellikSicil.yeriKonusu))
                                {
                                    if (eserBilgi != "") eserBilgi += "-";
                                    eserBilgi += zd.ozellikSicil.yeriKonusu;
                                }

                                dt.Rows.Add(zd.gorSicilNo, zd.hesapPlanKod, zd.hesapPlanAd + " " + zd.ozellik, zimmet.kisiAdi, eserBilgi, zd.ozellikSicil.disSicilNo);
                                prSiciller.Add(zd.prSicilNo);
                            }
                        }
                    }

                    gvSicilNo.DataSource = dt;
                }
                else
                {
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMBRK002);
                }

                gvSicilNo.DataBind();

                int prSayac = 0;
                foreach (System.Web.UI.WebControls.GridViewRow gvr in gvSicilNo.Rows)
                {
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)gvr.FindControl("chkSecim");
                    chk.ID += "_prSicil_" + ((int)prSiciller[prSayac]).ToString();
                    prSayac++;
                }
            }
            else
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Ara tuþuna basýlýnca çalýþan olay metodu
        /// Ekrana girilmiþ bilgilere bakarak ilgili demirbaþ listeleme yordamýný çaðýrýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, System.EventArgs e)
        {
            if (txtBelgeNo.Text.Trim() != string.Empty && txtZimmetBelgeNo.Text.Trim() != string.Empty)
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMBRK003);
            else if (txtBelgeNo.Text.Trim() == string.Empty && txtZimmetBelgeNo.Text.Trim() != string.Empty)
            {
                if (rdKisi.Checked)
                    KriterToplaZimmet(false);
                else if (rdOrtak.Checked)
                    KriterToplaZimmet(true);
            }
            else
                KriterTopla();

            ClientScript.RegisterStartupScript(this.GetType(), "tasarim", "YeniGG();TasarimGG();", true);
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Demirbaþ listeleme kriterleri ekrandaki kontrollerden toplanýr ve SicilNumarasiDoldur yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="zimmetOrtak">Ortak alan zimmet mi yoksa kiþi zimmet mi bilgisi</param>
        private void KriterToplaZimmet(bool zimmetOrtak)
        {
            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            zf.belgeTur = zimmetOrtak == false ? (int)ENUMZimmetBelgeTur.ZIMMETFISI : zimmetOrtak == true ? (int)ENUMZimmetBelgeTur.DAYANIKLITL : (int)ENUMZimmetBelgeTur.BELIRSIZ;

            zf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);

            zf.muhasebeKod = txtMuhasebe.Text.Trim();

            zf.harcamaBirimKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");

            zf.ambarKod = txtAmbar.Text.Trim();

            zf.fisNo = txtZimmetBelgeNo.Text == string.Empty ? string.Empty : txtZimmetBelgeNo.Text.Trim().PadLeft(6, '0');

            SicilNumarasiDoldur(null, zf);
        }

        /// <summary>
        /// Handles the Click event of the btnEtiketKaydet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnEtiketKaydet_Click(object sender, EventArgs e)
        {
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETYAZICI", rdZebra.Checked ? "zebra" : rdArgox.Checked ? "argox" : rdDatamax.Checked ? "datamax" : "diger");
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETFONT", rdBarkodCokKucuk.Checked ? "1" : rdBarkodKucuk.Checked ? "2" : rdBarkodNormal.Checked ? "3" : "4");
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETYUK", txtYukseklik.Text.ToString());
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETGEN", txtGenislik.Text.ToString());
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETSOL", txtSoldanBosluk.Text.ToString());
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETUST", txtUsttenBosluk.Text.ToString());

            string tasarimYeni = "0";
            if (chkYeni.Checked)
                tasarimYeni = "1";
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODYENI", tasarimYeni);
            ClientScript.RegisterStartupScript(this.GetType(), "tasarim", "YeniGG();TasarimGG();", true);
            
            string tasarim = "0";
            if (chkTasarim.Checked)
                tasarim = "1";
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETASARIM", tasarim);
        }

        /// <summary>
        /// Handles the Click event of the btnKaydetData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnKaydetData_Click(object sender, EventArgs e)
        {
            string tasarim = "0";
            if (chkTasarim.Checked)
                tasarim = "1";
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETASARIM", tasarim);

            string data = txtData.Text.Trim();

            for (int i = 0; i < 20; i++)
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETDATA" + i, "");

            int sayac = 0;
            while (data.Length > 0)
            {
                int uzunluk = 200;
                if (data.Length <= uzunluk)
                    uzunluk = data.Length;

                string parca = data.Substring(0, uzunluk);
                data = data.Substring(uzunluk, data.Length - uzunluk);

                GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETDATA" + sayac, parca);

                sayac++;
            }
            ClientScript.RegisterStartupScript(this.GetType(), "tasarim", "YeniGG();TasarimGG();", true);

        }

        //protected void btnBarkod_Click(object sender, EventArgs e)
        //{
        //    GenelIslemler.KullaniciDegiskenSakla(kullanan, "BYAZICITIP", rdZebra.Checked ? "zebra" : rdBixolon.Checked ? "bixolon" : rdDatamax.Checked ? "datamax" : "diger");

        //    //if (rdDiger.Checked)
        //    //    GenelIslemler.KullaniciDegiskenSakla(kullanan, "BYAZICIAD", txtYaziciAdi.Value.ToString());

        //    GenelIslemler.KullaniciDegiskenSakla(kullanan, "BETIKETEN", txtbEn.Value.ToString());

        //    if (chkZimmetBilgi.Checked && !chkEserBilgi.Checked)
        //        GenelIslemler.KullaniciDegiskenSakla(kullanan, "BZIMMETESERBILGI", "zimmet");
        //    else if (!chkZimmetBilgi.Checked && chkEserBilgi.Checked)
        //        GenelIslemler.KullaniciDegiskenSakla(kullanan, "BZIMMETESERBILGI", "eser");
        //    else if (chkZimmetBilgi.Checked && chkEserBilgi.Checked)
        //        GenelIslemler.KullaniciDegiskenSakla(kullanan, "BZIMMETESERBILGI", "zimmeteser");
        //    else
        //        GenelIslemler.KullaniciDegiskenSakla(kullanan, "BZIMMETESERBILGI", "");

        //    if (rdBarkodBuyuk.Checked)
        //        GenelIslemler.KullaniciDegiskenSakla(kullanan, "BBARKODBOYUT", "buyuk");
        //    else if (rdBarkodKucuk.Checked)
        //        GenelIslemler.KullaniciDegiskenSakla(kullanan, "BBARKODBOYUT", "kucuk");
        //    else
        //        GenelIslemler.KullaniciDegiskenSakla(kullanan, "BBARKODBOYUT", "");
        //}
    }
}