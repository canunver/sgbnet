using Ext1.Net;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fiþi bilgilerinin kayýt, listeleme, onaylama, onay kaldýrma ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class ZimmetForm : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        /// <summary>
        /// Ortak alan zimmet fiþi mi yoksa kiþi zimmet fiþi mi olduðunu tutan deðiþken
        /// </summary>
        static string belgeTuru;

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
            if (Request.QueryString["belgeTur"] != null)
            {
                belgeTuru = Request.QueryString["belgeTur"] + "";
            }
            else
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMZFS001);
                return;
            }

            if (!X.IsAjaxRequest)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtBelgeTarihi.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    txtKimeVerildi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ZIMMETKISI");

                ddlBelgeTipi.SetValueAndFireSelect(1);
                ddlBelgeTuru.SetValueAndFireSelect(1);

                FormAyarla(belgeTuru);
                ListeTemizle();
                txtYil.Value = DateTime.Now.Year;
                BelgeTuruDoldur();
                BelgeTipiDoldur();

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    grdListe.ColumnModel.SetHidden(4, true); //KDV
                    grdListe.ColumnModel.SetColumnHeader(5, "Birim Fiyat"); //Birim Fiyat

                    txtKimeVerildi.Listeners.ClearListeners();
                    txtKimeVerildi.Triggers.RemoveAt(0);
                }

                //if (TasinirGenel.tasinirZimmeteOnay)
                //{
                //    btnEOnayYazdir.Visible = true;
                //    eOnayAlan.Visible = true;

                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG063, "0"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG059, "1"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG060, "4"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFG061, "5"));
                //    ddleOnayDurum.Items.Add(new ListItem(Resources.TasinirMal.FRMZFS007, ""));
                //    ddleOnayDurum.SelectedIndex = 4;
                //}
                //else
                //{
                //    btnEOnayYazdir.Visible = false;
                //    eOnayAlan.Visible = false;
                //}
            }
        }

        private void FormAyarla(string belgeTuru)
        {
            hdnBelgeTur.Value = belgeTuru == "10" ? ((int)ENUMZimmetBelgeTur.ZIMMETFISI).ToString() : belgeTuru == "20" ? ((int)ENUMZimmetBelgeTur.DAYANIKLITL).ToString() : ((int)ENUMZimmetBelgeTur.BELIRSIZ).ToString();

            if (belgeTuru == "10")
                formAdi = Resources.TasinirMal.FRMZFS012;
            else if (belgeTuru == "20")
                formAdi = Resources.TasinirMal.FRMZFS013;
            else
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMZFS014);
                return;
            }
        }

        /// <summary>
        /// Belge Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Zimmet fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            if (txtKimeVerildi.Text.Trim() != "")
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ZIMMETKISI", txtKimeVerildi.Text.Trim());

            TNS.TMM.ZimmetForm zimmet = new TNS.TMM.ZimmetForm();

            zimmet.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            zimmet.fisTarih = new TNSDateTime(txtBelgeTarihi.RawText);
            zimmet.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zimmet.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            zimmet.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zimmet.ambarKod = txtAmbar.Text.Replace(".", "");
            zimmet.tip = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlBelgeTipi), 0);
            zimmet.vermeDusme = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlBelgeTuru), 0);
            zimmet.kimeGitti = txtKimeVerildi.Text.Trim();
            zimmet.nereyeGitti = txtNereyeVerildi.Text.Trim();
            zimmet.belgeTur = belgeTuru == "10" ? (int)ENUMZimmetBelgeTur.ZIMMETFISI : belgeTuru == "20" ? (int)ENUMZimmetBelgeTur.DAYANIKLITL : (int)ENUMZimmetBelgeTur.BELIRSIZ;
            if (zimmet.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
                zimmet.dusmeTarih = new TNSDateTime(txtBelgeTarihi.RawText);

            ObjectArray zimmetDetayArray = new ObjectArray();

            int siraNo = 1;
            foreach (JContainer jc in satirlar)
            //foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                ZimmetDetay zimmetDetay = new ZimmetDetay();
                zimmetDetay.yil = zimmet.yil;
                zimmetDetay.muhasebeKod = zimmet.muhasebeKod;
                zimmetDetay.harcamaBirimKod = zimmet.harcamaBirimKod;
                zimmetDetay.belgeTur = zimmet.belgeTur;

                zimmetDetay.hesapPlanKod = jc.Value<string>("TASINIRHESAPKOD").Replace(".", "");//TasinirGenel.DegerAlStr(item, "TASINIRHESAPKOD");
                zimmetDetay.gorSicilNo = jc.Value<string>("SICILNO");
                zimmetDetay.ozellik = jc.Value<string>("ACIKLAMA");
                zimmetDetay.siraNo = siraNo;
                zimmetDetay.kdvOran = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("KDVORANI"), 0);
                //zimmetDetay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "BIRIMFIYATI"));
                zimmetDetay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal((jc.Value<double>("BIRIMFIYATI").ToString("R")));

                zimmetDetay.teslimDurum = jc.Value<string>("TESLIMEDILMEANINDADURUMU");

                if (zimmetDetay.gorSicilNo == string.Empty || zimmetDetay.hesapPlanKod == string.Empty)
                    continue;
                else
                    siraNo++;

                zimmetDetayArray.Ekle(zimmetDetay);
            }

            zimmet.islemTarih = new TNSDateTime(DateTime.Now);
            zimmet.islemYapan = kullanan.kullaniciKodu;

            Sonuc sonuc = servisTMM.ZimmetFisiKaydet(kullanan, zimmet, zimmetDetayArray);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                txtBelgeNo.Text = sonuc.anahtar;

                if (TasinirGenel.tasinirZimmeteOnay)
                {
                    string yazilimMailAdresi = OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "cevapyok@bist.com");
                    string mailAdres = "";
                    string bilgi = "Zimmet ePosta Onay için gönderilmiþtir.";
                    string tut = OrtakFonksiyonlar.MailAt(yazilimMailAdresi, mailAdres, "Zimmet ePosta Onay", bilgi, true, false, null);
                    if (tut == "1")
                    {
                        GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                        tut = mailAdres + " e-Posta adresine anket daveti gönderilmiþtir.";
                    }
                }

            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Belgeyi Bul resmine basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri zimmet form nesnesine doldurulur, sunucuya
        /// gönderilir ve zimmet fiþi bilgileri sunucudan alýnýr. Hata varsa
        /// ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.ZimmetForm zimmetForm = new TNS.TMM.ZimmetForm();

            if (txtBelgeNo.Text != "")
            {
                zimmetForm.fisNo = txtBelgeNo.Text;
            }

            zimmetForm.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            zimmetForm.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zimmetForm.muhasebeKod = txtMuhasebe.Text;
            zimmetForm.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zimmetForm.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            ObjectArray bilgi = servisTMM.ZimmetFisiAc(kullanan, zimmetForm);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            TNS.TMM.ZimmetForm tf = new TNS.TMM.ZimmetForm();
            tf = (TNS.TMM.ZimmetForm)bilgi[0];

            txtMuhasebe.Text = tf.muhasebeKod;
            txtHarcamaBirimi.Text = tf.harcamaBirimKod;
            txtBelgeNo.Text = tf.fisNo;
            txtAmbar.Text = tf.ambarKod;
            txtBelgeTarihi.Value = tf.fisTarih.ToString();
            txtKimeVerildi.Text = tf.kimeGitti;
            txtNereyeVerildi.Text = tf.nereyeGitti;
            ddlBelgeTipi.SetValueAndFireSelect(tf.tip);
            zimmetForm.tip = tf.tip;

            if (tf.durum == (int)ENUMBelgeDurumu.YENI || tf.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG005;
            else if (tf.durum == (int)ENUMBelgeDurumu.ONAYLI)
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG006;
            else if (tf.durum == (int)ENUMBelgeDurumu.IPTAL)
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG007;

            //if (TasinirGenel.tasinirZimmeteOnay)
            //lbleOnayDurum.Text = Resources.TasinirMal.FRMZFG059;

            if (txtKimeVerildi.Text.Trim() != "")
                lblKimeVerildi.Text = GenelIslemler.KodAd(36, txtKimeVerildi.Text.Trim(), true);
            else
                lblKimeVerildi.Text = "";

            if (txtNereyeVerildi.Text.Trim() != "")
                lblNereyeVerildi.Text = GenelIslemler.KodAd(35, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtNereyeVerildi.Text.Trim(), true);
            else
                lblNereyeVerildi.Text = "";

            ddlBelgeTuru.SetValueAndFireSelect(tf.vermeDusme);
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

                ObjectArray zimmeteOnay = servisTMM.ZimmetFisiEOnayListele(kullanan, zfeonay, zfeonaykriter);
                //if (zimmeteOnay.sonuc.islemSonuc)
                //{
                //    if (zimmeteOnay.objeler.Count > 0)
                //    {
                //        foreach (ZimmetFormEOnay dt in zimmeteOnay.objeler)
                //        {
                //            switch (dt.durum)
                //            {
                //                case 1:
                //                    lbleOnayDurum.Text = "eOnaya Gönderildi." + dt.emailGondermeTarih.Oku().ToShortDateString() + " tarihinde.";
                //                    break;
                //                case 4:
                //                    lbleOnayDurum.Text = "eOnaya Gönderildi." + dt.emailGondermeTarih.Oku().ToShortDateString() + " tarihinde. CEVAP GELDÝ.";
                //                    break;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        lbleOnayDurum.Text = "eOnaya GÖNDERÝLMEMÝÞ";
                //    }
                //}
            }

            ObjectArray detay = servisTMM.ZimmetFisiDetayListele(kullanan, zimmetForm);

            if (!detay.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyarý", detay.sonuc.hataStr);

            List<object> liste = new List<object>();

            foreach (ZimmetDetay dt in detay.objeler)
            {
                liste.Add(new
                {
                    TASINIRHESAPKOD = dt.hesapPlanKod,
                    SICILNO = dt.gorSicilNo,
                    ACIKLAMA = dt.ozellik,
                    TASINIRHESAPADI = dt.hesapPlanAd,
                    KDVORANI = dt.kdvOran,
                    BIRIMFIYATI = dt.birimFiyat,
                    TESLIMEDILMEANINDADURUMU = dt.teslimDurum
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();

            //ddlZimmetVermeDusme_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// Belge Onayla tuþuna basýlýnca çalýþan olay metodu
        /// Zimmet fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onaylanmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayla_Click(Object sender, DirectEventArgs e)
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
                GenelIslemler.MesajKutusu("Uyarý", hata + Resources.TasinirMal.FRMZFG030);
                return;
            }

            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            zf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            zf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            zf.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zf.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            Sonuc sonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, zf, "Onay");

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG006;
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Belge Onay Kaldýr tuþuna basýlýnca çalýþan olay metodu
        /// Zimmet fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onayý kaldýrýlmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayKaldir_Click(object sender, DirectEventArgs e)
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
                GenelIslemler.MesajKutusu("Uyarý", hata + Resources.TasinirMal.FRMZFG030);
                return;
            }

            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            zf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            zf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            zf.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zf.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            Sonuc sonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, zf, "OnayKaldir");

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG005;
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            ListeTemizle();
            txtYil.Value = DateTime.Now.Year;
            txtBelgeTarihi.Value = DateTime.Now.Date;
            txtBelgeNo.Clear();
            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            txtNereyeVerildi.Clear();
            lblNereyeVerildi.Text = "";
            txtKimeVerildi.Clear();
            lblKimeVerildi.Text = "";
            lblFormDurum.Text = "";
            ddlBelgeTuru.SetValueAndFireSelect(1);
            if (txtMuhasebe.Text == "") lblMuhasebeAd.Text = "";
            if (txtHarcamaBirimi.Text == "") lblHarcamaBirimiAd.Text = "";
            if (txtAmbar.Text == "") lblAmbarAd.Text = "";
        }

        protected void btnIptal_Click(object sender, DirectEventArgs e)
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
                GenelIslemler.MesajKutusu("Uyarý", hata + Resources.TasinirMal.FRMZFG030);
                return;
            }

            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

            zf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            zf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            zf.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            zf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            zf.belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);

            Sonuc sonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, zf, "Ýptal");

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG005;
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        protected void btnBelgeYazdir_Click(object sender, DirectEventArgs e)
        {
            string tempFileName = System.IO.Path.GetTempFileName();

            int yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            string muhasebeKodu = txtMuhasebe.Text.Replace(".", "");
            string harcamaKodu = txtHarcamaBirimi.Text.Replace(".", "");
            string belgeNo = txtBelgeNo.Text;
            int belgeTur = OrtakFonksiyonlar.ConvertToInt(hdnBelgeTur.Value, 0);
            bool resimEkle = chkResim.Checked;

            if (belgeNo == "") return;

            string tmpFile = System.IO.Path.GetTempFileName();
            ZimmetFormYazdir.Yazdir(kullanan, yil, belgeNo, harcamaKodu, muhasebeKodu, belgeTur, tmpFile, resimEkle);

            string sonucDosyaAd = harcamaKodu.Replace(".", "") + "_" + belgeNo + "." + GenelIslemler.ExcelTur();

            OrtakClass.DosyaIslem.DosyaGonder(tmpFile, sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        private void ListeTemizle()
        {
            List<object> liste = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                liste.Add(new
                {
                    KOD = ""
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        private void BelgeTuruDoldur()
        {
            List<object> liste = new List<object>();

            liste.Add(new { KOD = 1, ADI = Resources.TasinirMal.FRMZFG017 });
            liste.Add(new { KOD = 2, ADI = Resources.TasinirMal.FRMZFG018 });

            strBelgeTuru.DataSource = liste;
            strBelgeTuru.DataBind();
        }

        private void BelgeTipiDoldur()
        {
            List<object> liste = new List<object>();

            liste.Add(new { KOD = 1, ADI = Resources.TasinirMal.FRMZFG008 });
            liste.Add(new { KOD = 2, ADI = Resources.TasinirMal.FRMZFG009 });

            strBelgeTipi.DataSource = liste;
            strBelgeTipi.DataBind();
        }


        [DirectMethod]
        public void EklenenSicilNolar(string sicilNolar)
        {
            OturumBilgisiIslem.BilgiYazDegisken("ZIMMETSICILNOLAR", sicilNolar);
        }
    }
}