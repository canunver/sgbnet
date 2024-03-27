using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using TNS.KYM;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fiþi bilgilerinin kayýt, listeleme, onaylama, onay kaldýrma ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetFormEski : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Gride eklenecek satýr sayýsý
        /// </summary>
        int ekleSatirSayisi = 15;

        /// <summary>
        /// Ortak alan zimmet fiþi mi yoksa kiþi zimmet fiþi mi olduðunu tutan deðiþken
        /// </summary>
        string belgeTuru;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde belgeTur girdi dizgisi dolu deðilse hata verir
        ///     ve sayfayý yüklemez, dolu ise sayfada bazý ayarlamalar yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            TasinirGenel.JSResourceEkle_ZimmetForm(this);

            belgeTuru = string.Empty;

            if (Request.QueryString["belgeTur"] != null)
                belgeTuru = Request.QueryString["belgeTur"];
            else
            {
                GenelIslemler.HataYaz(this, Resources.TasinirMal.FRMZFG001);
                return;
            }

            hdnBelgeTur.Value = belgeTuru == "10" ? ((int)ENUMZimmetBelgeTur.ZIMMETFISI).ToString() : belgeTuru == "20" ? ((int)ENUMZimmetBelgeTur.DAYANIKLITL).ToString() : ((int)ENUMZimmetBelgeTur.BELIRSIZ).ToString();
            if (TasinirGenel.tasinirZimmeteOnay)
            {
                lbleOnayDurum.Visible = true;
                btnEOnayGonder.Visible = true;
            }
            else
            {
                lbleOnayDurum.Visible = false;
                btnEOnayGonder.Visible = false;
            }

            if (TasinirGenel.tasinirSicilNoRFIDFarkli)
            {
                chkResim.Checked = false;
                chkResim.Enabled = false;
            }
            else
            {
                chkResim.Checked = false;
                chkResim.Enabled = true;
            }

            FormAyarla(belgeTuru);

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan))
                    GenelIslemler.SayfayaGirmesin(true);

            this.fpL.Attributes.Add("onDataChanged", "HucreDegisti(this)");
            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnOnayla.Attributes.Add("onclick", "return OnayAl('OnayTek','btnOnayla');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");
            this.btnOnayKaldir.Attributes.Add("onclick", "return OnayAl('OnayTekKaldir','btnOnayKaldir');");
            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtNereyeGitti.Attributes.Add("onblur", "kodAdGetir('35','lblNereyeGittiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtNereyeGitti'),'KONTROLDENOKU');");
            this.txtKimeGitti.Attributes.Add("onblur", "kodAdGetir('36','lblKimeGittiAd',true,new Array('txtKimeGitti'),'KONTROLDENOKU');");

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
                ZimmetVermeDusmeDoldur();
                ZimmetTipiDoldur();
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
        /// Parametre olarak verilen zimmet belge türüne bakarak sayfada bazý ayarlamalar yapar.
        /// </summary>
        /// <param name="belgeTuru">Zimmet belge türü</param>
        private void FormAyarla(string belgeTuru)
        {
            if (belgeTuru == "10")
                formAdi = Resources.TasinirMal.FRMZFG002;
            else if (belgeTuru == "20")
                formAdi = Resources.TasinirMal.FRMZFG003;
            else
            {
                GenelIslemler.HataYaz(this, Resources.TasinirMal.FRMZFG004);
                return;
            }

            //if (belgeTuru == "10")
            //    divNereyeVerildi.Visible = false;

            if (belgeTuru == "20")
            {
                divZimmetFisiTipi.Visible = false;
            }
        }

        /// <summary>
        /// Belgeyi Bul resmine basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri zimmet form nesnesine doldurulur, sunucuya
        /// gönderilir ve zimmet fiþi bilgileri sunucudan alýnýr. Hata varsa
        /// ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, ImageClickEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = "";
            fpL.SaveChanges();

            TNS.TMM.ZimmetForm zimmetForm = new TNS.TMM.ZimmetForm();

            zimmetForm.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            zimmetForm.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zimmetForm.muhasebeKod = txtMuhasebe.Text;
            zimmetForm.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zimmetForm.belgeTur = belgeTuru == "10" ? (int)ENUMZimmetBelgeTur.ZIMMETFISI : belgeTuru == "20" ? (int)ENUMZimmetBelgeTur.DAYANIKLITL : (int)ENUMZimmetBelgeTur.BELIRSIZ;

            ObjectArray bilgi = servis.ZimmetFisiAc(kullanan, zimmetForm);

            if (bilgi.sonuc.islemSonuc)
            {
                TNS.TMM.ZimmetForm tf = new TNS.TMM.ZimmetForm();
                tf = (TNS.TMM.ZimmetForm)bilgi[0];

                txtMuhasebe.Text = tf.muhasebeKod;
                txtHarcamaBirimi.Text = tf.harcamaBirimKod;
                txtBelgeNo.Text = tf.fisNo;
                txtAmbar.Text = tf.ambarKod;
                txtBelgeTarih.Text = tf.fisTarih.ToString();
                txtKimeGitti.Text = tf.kimeGitti;
                txtNereyeGitti.Text = tf.nereyeGitti;
                ddlZimmetFisiTipi.SelectedValue = tf.tip.ToString();
                zimmetForm.tip = tf.tip;

                if (tf.durum == (int)ENUMBelgeDurumu.YENI || tf.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    lblFormDurum.Text = Resources.TasinirMal.FRMZFG005;
                else if (tf.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    lblFormDurum.Text = Resources.TasinirMal.FRMZFG006;
                else if (tf.durum == (int)ENUMBelgeDurumu.IPTAL)
                    lblFormDurum.Text = Resources.TasinirMal.FRMZFG007;

                if (TasinirGenel.tasinirZimmeteOnay)
                    lbleOnayDurum.Text = Resources.TasinirMal.FRMZFG059;

                if (txtKimeGitti.Text.Trim() != "")
                    lblKimeGittiAd.Text = GenelIslemler.KodAd(36, txtKimeGitti.Text.Trim(), true);
                else
                    lblKimeGittiAd.Text = "";

                if (txtNereyeGitti.Text.Trim() != "")
                    lblNereyeGittiAd.Text = GenelIslemler.KodAd(35, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtNereyeGitti.Text.Trim(), true);
                else
                    lblNereyeGittiAd.Text = "";

                ddlZimmetVermeDusme.SelectedValue = tf.vermeDusme.ToString();
                zimmetForm.vermeDusme = tf.vermeDusme;


                if (TasinirGenel.tasinirZimmeteOnay)
                {
                    ZimmetFormEOnay zfeonay = new ZimmetFormEOnay();
                    ZimmetFormEOnayKriter zfeonaykriter = new ZimmetFormEOnayKriter();

                    zfeonay.yil = zimmetForm.yil;
                    zfeonay.muhasebeKod = zimmetForm.muhasebeKod;
                    zfeonay.harcamaBirimKod = zimmetForm.harcamaBirimKod;
                    zfeonay.fisNo = zimmetForm.fisNo;
                    zfeonay.belgeTur = zimmetForm.belgeTur;

                    zfeonaykriter.eonayGondermeTarihBasla = new TNSDateTime();
                    zfeonaykriter.eonayGondermeTarihBitis = new TNSDateTime();
                    zfeonaykriter.eonayCevapTarihBasla = new TNSDateTime();
                    zfeonaykriter.eonayCevapTarihBitis = new TNSDateTime();

                    ObjectArray zimmeteOnay = servis.ZimmetFisiEOnayListele(kullanan, zfeonay, zfeonaykriter);
                    if (zimmeteOnay.sonuc.islemSonuc)
                    {
                        if (zimmeteOnay.objeler.Count > 0)
                        {
                            foreach (ZimmetFormEOnay dt in zimmeteOnay.objeler)
                            {
                                switch (dt.durum)
                                {
                                    case 1:
                                        lbleOnayDurum.Text = "eOnaya Gönderildi." + dt.emailGondermeTarih.Oku().ToShortDateString() + " tarihinde.";
                                        break;
                                    case 4:
                                        lbleOnayDurum.Text = "eOnaya Gönderildi." + dt.emailGondermeTarih.Oku().ToShortDateString() + " tarihinde. CEVAP GELDÝ.";
                                        break;
                                }
                            }
                        }
                        else
                        {
                            lbleOnayDurum.Text = "eOnaya GÖNDERÝLMEMÝÞ";
                        }
                    }

                }

                ObjectArray detay = servis.ZimmetFisiDetayListele(kullanan, zimmetForm);

                if (detay.sonuc.islemSonuc)
                {
                    if (detay.ObjeSayisi > fpL.Sheets[0].RowCount)
                        fpL.Sheets[0].RowCount = detay.ObjeSayisi + 10;

                    int satir = 0;
                    foreach (ZimmetDetay dt in detay.objeler)
                    {
                        fpL.Cells[satir, 0].Text = dt.hesapPlanKod;
                        fpL.Cells[satir, 1].Text = dt.gorSicilNo;
                        fpL.Cells[satir, 3].Text = dt.ozellik;
                        fpL.Cells[satir, 4].Text = dt.hesapPlanAd;
                        fpL.Cells[satir, 5].Text = dt.kdvOran.ToString();
                        fpL.Cells[satir, 6].Text = dt.birimFiyat.ToString();
                        fpL.Cells[satir, 7].Text = dt.teslimDurum;
                        satir++;
                    }

                    ddlZimmetVermeDusme_SelectedIndexChanged(null, null);
                }
                else
                    GenelIslemler.HataYaz(this, detay.sonuc.hataStr);
            }
            else
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Kaydedilmiþ belge ilgili kiþiye onaylamasý için 
        /// email olarak gönderililir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnEOnayGonder_Click(object sender, EventArgs e)
        {
            if (lblFormDurum.Text == Resources.TasinirMal.FRMZFG006)
            {
                string hata = "";
                if (txtMuhasebe.Text.Trim() == "")
                    hata = Resources.TasinirMal.FRMZFG027 + "<br>";

                if (txtHarcamaBirimi.Text.Trim() == "")
                    hata += Resources.TasinirMal.FRMZFG028 + "<br>";

                if (txtBelgeNo.Text.Trim() == "")
                    hata += Resources.TasinirMal.FRMZFG029 + "<br>";

                if (hata != "")
                {
                    GenelIslemler.HataYaz(this, hata + Resources.TasinirMal.FRMZFG030);
                    return;
                }

                TNS.TMM.ZimmetFormEOnay zfeonay = new TNS.TMM.ZimmetFormEOnay();

                zfeonay.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
                zfeonay.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
                zfeonay.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
                zfeonay.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
                zfeonay.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

                zfeonay.emailKimden = kullanan.mernis;
                zfeonay.emailKimdenAd = kullanan.ADSOYAD;
                zfeonay.emailKime = txtKimeGitti.Text.Trim();
                zfeonay.emailKimeAd = lblKimeGittiAd.Text.Trim();
                zfeonay.emailGondermeTarih = new TNSDateTime(DateTime.Now);
                zfeonay.durum = (int)ENUMEOnayDurumu.GONDERILDI;

                Sonuc sonuc = servis.ZimmetFisiEOnayKaydet(kullanan, zfeonay);

                if (sonuc.islemSonuc)
                {
                    GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                    lbleOnayDurum.Text = Resources.TasinirMal.FRMZFG059;
                }
                else
                    GenelIslemler.HataYaz(this, sonuc.hataStr);
            }
            else
            {
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMZFG058);
            }

        }

        /// <summary>
        /// Belge Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Zimmet fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.ZimmetForm zimmet = new TNS.TMM.ZimmetForm();

            zimmet.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            zimmet.fisTarih = new TNSDateTime(txtBelgeTarih.Text);
            zimmet.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zimmet.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            zimmet.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zimmet.ambarKod = txtAmbar.Text.Replace(".", "");
            zimmet.tip = OrtakFonksiyonlar.ConvertToInt(ddlZimmetFisiTipi.SelectedValue, 0);
            zimmet.vermeDusme = OrtakFonksiyonlar.ConvertToInt(ddlZimmetVermeDusme.SelectedValue, 0);
            zimmet.kimeGitti = txtKimeGitti.Text.Trim();
            zimmet.nereyeGitti = txtNereyeGitti.Text.Trim();
            zimmet.belgeTur = belgeTuru == "10" ? (int)ENUMZimmetBelgeTur.ZIMMETFISI : belgeTuru == "20" ? (int)ENUMZimmetBelgeTur.DAYANIKLITL : (int)ENUMZimmetBelgeTur.BELIRSIZ;
            if (zimmet.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
                zimmet.dusmeTarih = new TNSDateTime(txtBelgeTarih.Text);

            fpL.SaveChanges();

            ObjectArray zimmetDetayArray = new ObjectArray();

            int siraNo = 1;

            for (int i = 0; i < fpL.Rows.Count; i++)
            {
                ZimmetDetay zimmetDetay = new ZimmetDetay();
                zimmetDetay.yil = zimmet.yil;
                zimmetDetay.muhasebeKod = zimmet.muhasebeKod;
                zimmetDetay.harcamaBirimKod = zimmet.harcamaBirimKod;
                zimmetDetay.belgeTur = zimmet.belgeTur;

                zimmetDetay.hesapPlanKod = fpL.Cells[i, 0].Text.Trim().Replace(".", "");
                zimmetDetay.gorSicilNo = fpL.Cells[i, 1].Text.Trim().Replace("-", "");
                zimmetDetay.ozellik = fpL.Cells[i, 3].Text.Trim();
                zimmetDetay.siraNo = siraNo;
                zimmetDetay.kdvOran = OrtakFonksiyonlar.ConvertToInt(fpL.Cells[i, 5].Text, 0);
                zimmetDetay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(fpL.Cells[i, 6].Text);
                zimmetDetay.teslimDurum = fpL.Cells[i, 7].Text.Trim();

                if (zimmetDetay.gorSicilNo == string.Empty || zimmetDetay.hesapPlanKod == string.Empty)
                    continue;
                else
                    siraNo++;

                zimmetDetayArray.Ekle(zimmetDetay);
            }

            zimmet.islemTarih = new TNSDateTime(DateTime.Now);
            zimmet.islemYapan = kullanan.kullaniciKodu;

            Sonuc sonuc = servis.ZimmetFisiKaydet(kullanan, zimmet, zimmetDetayArray);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                txtBelgeNo.Text = sonuc.anahtar;

                if (TasinirGenel.tasinirZimmeteOnay)
                {
                    string yazilimMailAdresi = OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "cevapyok@bist.com");
                    string mailAdres = "";
                    string bilgi = "Zimmet ePosta Onay için gönderilmiþtir.";
                    string tut = OrtakFonksiyonlar.MailAt(yazilimMailAdresi, mailAdres, "Zimmet ePosta Onay", bilgi, true, false, null);
                    if (tut == "1")
                    {
                        GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                        tut = mailAdres + " e-Posta adresine anket daveti gönderilmiþtir.";
                    }
                }
                
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
        protected void btnTemizle_Click(object sender, EventArgs e)
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
            txtNereyeGitti.Text = string.Empty;
            txtKimeGitti.Text = string.Empty;
            lblKimeGittiAd.Text = "";
            lblNereyeGittiAd.Text = "";
            lblFormDurum.Text = "";
            lbleOnayDurum.Text = "";
            if (txtMuhasebe.Text == "")
                lblMuhasebeAd.Text = string.Empty;
            if (txtHarcamaBirimi.Text == "")
                lblHarcamaBirimiAd.Text = string.Empty;
            if (txtAmbar.Text == "")
                lblAmbarAd.Text = string.Empty;
        }

        /// <summary>
        /// Zimmet verilecek malzemelerin türlerini ddlZimmetFisiTipi DropDownList kontrolüne dolduran yordam
        /// </summary>
        private void ZimmetTipiDoldur()
        {
            ddlZimmetFisiTipi.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG008, "1"));
            ddlZimmetFisiTipi.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG009, "2"));
        }

        /// <summary>
        /// Zimmet verme/düþme seçimi deðiþtiðinde çalýþan olay metodu
        /// Seçilen iþlem ile ilgili sayfada ayarlamalar yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void ddlZimmetVermeDusme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender != null)
                if (sender.GetType() == typeof(DropDownList))
                    btnTemizle_Click(null, null);
        }

        /// <summary>
        /// Sayfadaki farpoint grid kontrolünün ilk yükleniþte ayarlanmasýný saðlayan yordam
        /// </summary>
        /// <param name="kontrol">Farpoint grid kontrolü</param>
        void GridInit(FarPoint.Web.Spread.FpSpread kontrol)
        {
            kontrol.RenderCSSClass = true;
            kontrol.EditModeReplace = false;

            kontrol.Sheets.Count = 1;
            kontrol.CommandBar.Visible = true;

            kontrol.Sheets[0].RowCount = ekleSatirSayisi;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 8;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMZFG010;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 1].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 1].Value = Resources.TasinirMal.FRMZFG011;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMZFG012;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMZFG013;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMZFG014;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMZFG015;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMZFG016;

            kontrol.Sheets[0].Columns[0].Width = 120;
            kontrol.Sheets[0].Columns[1].Width = 150;
            kontrol.Sheets[0].Columns[2].Width = 20;
            kontrol.Sheets[0].Columns[3].Width = 150;
            kontrol.Sheets[0].Columns[4].Width = 150;
            kontrol.Sheets[0].Columns[5].Width = 20;
            kontrol.Sheets[0].Columns[6].Width = 80;
            kontrol.Sheets[0].Columns[7].Width = 150;

            kontrol.Sheets[0].Columns[5, 6].HorizontalAlign = HorizontalAlign.Right;

            //TasinirGenel.MyLinkType hesapPlaniLink = new TasinirGenel.MyLinkType("HesapPlaniGoster()");
            //hesapPlaniLink.ImageUrl = "../App_themes/images/bul1.gif";

            TasinirGenel.MyLinkType sicilNoLink = new TasinirGenel.MyLinkType("SicilNoListesiAc()");
            sicilNoLink.ImageUrl = "../App_themes/images/bul1.gif";

            //kontrol.Sheets[0].Columns[1].CellType = hesapPlaniLink;
            kontrol.Sheets[0].Columns[2].CellType = sicilNoLink;

            kontrol.Sheets[0].Columns[0].Locked = true; //Hesap Kod
            kontrol.Sheets[0].Columns[1].Locked = true; //Sicil No
            kontrol.Sheets[0].Columns[4].Locked = true; //Hesap Planý Adý
            kontrol.Sheets[0].Columns[5].Locked = true; //KDV Oraný
            kontrol.Sheets[0].Columns[6].Locked = true; //Birim Fiyat
            kontrol.Sheets[0].Columns[0].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[1].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[4].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[5].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[6].BackColor = System.Drawing.Color.LightGoldenrodYellow;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;
            kontrol.Sheets[0].Columns[1].CellType = cTextType;
        }

        /// <summary>
        /// Zimmet verme, zimmet düþme iþlemleri ddlZimmetVermeDusme DropDownList kontrolüne dolduran yordam
        /// </summary>
        private void ZimmetVermeDusmeDoldur()
        {
            ddlZimmetVermeDusme.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG017, "1"));
            ddlZimmetVermeDusme.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG018, "2"));
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
                img.AlternateText = Resources.TasinirMal.FRMZFG019;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMZFG020;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMZFG021;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMZFG022;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow50.gif";
                img.AlternateText = Resources.TasinirMal.FRMZFG023;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc50(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow100.gif";
                img.AlternateText = Resources.TasinirMal.FRMZFG024;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc100(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow250.gif";
                img.AlternateText = Resources.TasinirMal.FRMZFG025;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc250(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow500.gif";
                img.AlternateText = Resources.TasinirMal.FRMZFG026;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc500(); return false;");
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

            if (tur == "bossatirekle50")
            {
                fpL.ActiveSheetView.RowCount += 50;
            }
            if (tur == "bossatirekle100")
            {
                fpL.ActiveSheetView.RowCount += 100;
            }
            if (tur == "bossatirekle250")
            {
                fpL.ActiveSheetView.RowCount += 250;
            }
            if (tur == "bossatirekle500")
            {
                fpL.ActiveSheetView.RowCount += 500;
            }

            fpL.SaveChanges();
        }

        /// <summary>
        /// Belge Onayla tuþuna basýlýnca çalýþan olay metodu
        /// Zimmet fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onaylanmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayla_Click(Object sender, EventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMZFG027 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMZFG028 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMZFG029 + "<br>";

            if (hata != "")
            {
                GenelIslemler.HataYaz(this, hata + Resources.TasinirMal.FRMZFG030);
                return;
            }

            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            zf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            zf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            zf.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zf.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            Sonuc sonuc = servis.ZimmetFisiDurumDegistir(kullanan, zf, "Onay");

            if (sonuc.islemSonuc)
            {
                GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG006;
            }
            else
                GenelIslemler.HataYaz(this, sonuc.hataStr);
        }

        /// <summary>
        /// Belge Onay Kaldýr tuþuna basýlýnca çalýþan olay metodu
        /// Zimmet fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onayý kaldýrýlmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayKaldir_Click(object sender, EventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMZFG027 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMZFG028 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMZFG029 + "<br>";

            if (hata != "")
            {
                GenelIslemler.HataYaz(this, hata + Resources.TasinirMal.FRMZFG030);
                return;
            }

            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            zf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            zf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            zf.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zf.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            Sonuc sonuc = servis.ZimmetFisiDurumDegistir(kullanan, zf, "OnayKaldir");

            if (sonuc.islemSonuc)
            {
                GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG005;
            }
            else
                GenelIslemler.HataYaz(this, sonuc.hataStr);
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