using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþi bilgilerinin kayýt, listeleme, onaylama, onay kaldýrma ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIslemFormGM : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        string acilistaIslemTipiSec = "";

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        ///     Sayfa adresinde yil, muhasebe, harcamaBirimi ve belgeNo girdi dizgileri dolu
        ///     geliyorsa istenen belgeyi listelemesi için btnListele_Click yordamý çaðýrýlýr.
        ///     Listeleme kriterleri sayfa adresinde gelmiyorsa sayým tutanaðý, kayýttan düþme
        ///     teklif ve onay tutanaðý, geçici alýndý fiþi veya ihtiyaç fazlasý taþýnýrlar formu
        ///     bilgilerinden herhangi biri sessionda kayýtlý mý diye kontrol edilir, eðer kayýtlýysa
        ///     ilgili bilgileri ekrana yazan yordam çaðýrýlýr. Bu sayede, taþýnýr iþlem fiþi diðer
        ///     formlarla entegre edilmiþ olur. Son olarak seçili olan birime gönderilmiþ devir çýkýþ
        ///     taþýnýr iþlem fiþi var mý kontrolü yapýlýr, varsa kullanýcýya listesi gösterilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //B-A Onayý Kontrolü
                string baOnayi = OrtakFonksiyonlar.WebConfigOku("TasinirBAOnayi", "");
                if (baOnayi == "1")
                {
                    btnBelgeOnayaGonder.Hidden = false;
                    btnBelgeOnayla.Hidden = true;
                    btnBelgeOnayKaldir.Hidden = true;
                }
                else
                    btnBelgeOnayaGonder.Hidden = true;
                //*******************************************

                string ekranTip = EkranTurDondur();
                hdnEkranTip.Value = ekranTip;
                if (ekranTip == "GM")
                {
                    formAdi = "Gayrimenkul Ýþlem Fiþi";
                    GMTurDoldur();
                    ddlGayrimenkulTuru.FieldLabel = "Gayrimenkül Türü";
                }
                else if (ekranTip == "YZ")
                {
                    formAdi = "Yazýlým Ýþlem Fiþi";
                    YZTurDoldur();
                    ddlGayrimenkulTuru.FieldLabel = "Yazýlým Türü";
                    YZEkraniHazirla();
                }

                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                MuhasebeIslemTipiDoldur(ekranTip);
                IslemTipiDoldur();



                txtYil.Value = DateTime.Now.Year;
                txtBelgeTarih.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtGonYil.Value = DateTime.Now.Year;

                if (Request.QueryString["belgeNo"] + "" != "")
                {
                    txtYil.Value = Request.QueryString["yil"];
                    txtBelgeNo.Value = Request.QueryString["belgeNo"] + "";
                    txtMuhasebe.Value = Request.QueryString["muhasebe"] + "";
                    txtHarcamaBirimi.Value = Request.QueryString["harcamaBirimi"] + "";
                    X.AddScript("txtBelgeNo.fireEvent('TriggerClick');");
                }

                int islemTur = OrtakFonksiyonlar.ConvertToInt(acilistaIslemTipiSec.Split('*')[1], 0);
                ddlIslemTipi.SetValueAndFireSelect(acilistaIslemTipiSec);
            }

        }

        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            TasinirIslemFormYazdir.Yazdir(kullanan, OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0), txtBelgeNo.Text, txtHarcamaBirimi.Text, txtMuhasebe.Text, "", "");
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            string hata = "";

            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.ambarKod = txtAmbar.Text;

            tf.fisTarih = new TNSDateTime(txtBelgeTarih.RawText);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[0], 0);
            tf.islemTipTur = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[1], 0);
            tf.aciklama = txtAciklama.Text;
            tf.neredenGeldi = ".";
            tf.nereyeGitti = txtKayittanCikisNedeni.Text;

            if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)//Devir Çýkýþ
            {
                tf.gMuhasebeKod = txtGonMuhasebe.Text;
                tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                tf.gAmbarKod = txtAmbar.Text;
                tf.gYil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
                tf.gFisNo = txtGonBelgeNo.Text.Trim().PadLeft(6, '0');

                if (tf.gMuhasebeKod == string.Empty)
                    hata += Resources.TasinirMal.FRMTIG067 + "<br>";
                if (tf.gHarcamaKod == string.Empty)
                    hata += Resources.TasinirMal.FRMTIG068 + "<br>";

                if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                    if (txtGonBelgeNo.Text.Trim() == "")
                        hata += Resources.TasinirMal.FRMTIG069 + "<br>";

                if (tf.muhasebeKod == tf.gMuhasebeKod && tf.harcamaKod == tf.gHarcamaKod && tf.ambarKod == tf.gAmbarKod)
                    hata += TasinirMalResI.TMM261 + "<br>";

                if (hata != "")
                {
                    GenelIslemler.MesajKutusu("Hata", hata);
                    return;
                }
            }

            TasinirIslemDetay td = new TasinirIslemDetay();
            td.hesapPlanKod = TasinirGenel.ComboDegerDondur(ddlGayrimenkulTuru);

            if (td.hesapPlanKod == "097.00")
            {
                string muhIslemTip = TasinirGenel.ComboDegerDondur(ddlMuhasebeIslemTipi);
                if (string.IsNullOrWhiteSpace(muhIslemTip))
                {
                    GenelIslemler.MesajKutusu("Hata", "Muhasebe iþlem tipi boþ olamaz.");
                    return;
                }
            }


            td.yil = tf.yil;
            td.muhasebeKod = tf.muhasebeKod;
            td.harcamaKod = tf.harcamaKod;
            td.ambarKod = tf.ambarKod;
            td.siraNo = 1;
            td.miktar = 1;
            td.kdvOran = 0;

            if (tf.islemTipTur > 50)//çýkýþ iþlemi ise 
            {
                td.gorSicilNo = txtSicilNo.Text;
                td.sicilNo = OrtakFonksiyonlar.ConvertToInt(hdnPrSicilNo.Value, 0);
                td.satisBedeli = OrtakFonksiyonlar.ConvertToDecimal(txtFiyati.Text.Replace(".", ""));
                td.birimFiyat = 0;
            }
            else
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(txtFiyati.Text.Replace(".", ""));

            td.ozellik.giai = txtAd.Text;
            td.ozellik.boyutlari = txtAlani.Text;
            td.ozellik.satirSayisi = txtAdano.Text;
            td.ozellik.yaprakSayisi = txtParselNo.Text;
            td.ozellik.sayfaSayisi = txtPaftaNo.Text;
            td.ozellik.yayinYeri = txtEdinmeSekli.Text;
            td.ozellik.yayinTarihi = new TNSDateTime(txtEdinmeTarihi.RawText).ToString();
            td.ozellik.gelisTarihi = new TNSDateTime(txtKayittanCikisTarihi.RawText).ToString();
            td.ozellik.neredenGeldi = txtKayittanCikisNedeni.Text;
            //td.ozellik.ciltTuru = TasinirGenel.ComboDegerDondur(ddlGayrimenkulTuru);
            td.ozellik.neredeBulundu = txtAdres.Text;
            td.ozellik.yazarAdi = txtAciklama.Text;
            td.ozellik.blokajKodu = TasinirGenel.ComboDegerDondur(ddlMuhasebeIslemTipi);
            td.ozellik.cagi = txtSicilNo.Text;
            td.ozellik.cesidi = hdnPrSicilNo.Text;


            if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
            {
                TNS.TMM.TasinirIslemForm tfGiris = new TNS.TMM.TasinirIslemForm();

                tfGiris.kod = hdnKod.Text;
                tfGiris.yil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
                tfGiris.muhasebeKod = txtGonMuhasebe.Text;
                tfGiris.harcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                tfGiris.fisNo = txtGonBelgeNo.Text;
                tfGiris.devirGirisiMi = true;

                ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tfGiris, true);

                if (bilgi.sonuc.islemSonuc)
                {
                    tfGiris = (TNS.TMM.TasinirIslemForm)bilgi.objeler[0];

                    TasinirIslemDetay tdGiris = (TasinirIslemDetay)tfGiris.detay[0];
                    td.gonMuhasebeKod = tf.gMuhasebeKod;
                    td.gonHarcamaKod = tf.gHarcamaKod;
                    td.gorSicilNo = tdGiris.gorSicilNo;
                    td.birimFiyat = tdGiris.birimFiyat;
                    td.ozellik.cesidi = "";
                }
                else
                {
                    GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
                    return;
                }

            }

            tf.detay.Ekle(td);

            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;

            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, tf);

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG011;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                txtBelgeNo.Text = sonuc.anahtar.Split('-')[0];
                hdnKod.Text = sonuc.anahtar.Split('-')[1];
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Belgeyi Bul resmine basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri taþýnýr iþlem form nesnesine doldurulur, sunucuya
        /// gönderilir ve taþýnýr iþlem fiþi bilgileri sunucudan alýnýr. Hata varsa
        /// ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            int islemTuru = 0;

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.muhasebeKod = txtMuhasebe.Text;
            tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);


            if (bilgi.sonuc.islemSonuc)
            {
                TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
                tform = (TNS.TMM.TasinirIslemForm)bilgi[0];

                X.AddScript("ddlIslemTipiSec('KOD'," + tform.islemTipKod + ");");

                islemTuru = IslemTuruGetir(tform.islemTipKod);

                if (tform.durum == (int)ENUMBelgeDurumu.YENI || tform.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    lblFormDurum.Text = Resources.TasinirMal.FRMTIG055;
                else if (tform.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    lblFormDurum.Text = Resources.TasinirMal.FRMTIG056;
                else if (tform.durum == (int)ENUMBelgeDurumu.IPTAL)
                    lblFormDurum.Text = Resources.TasinirMal.FRMTIG057;

                txtBelgeNo.Text = tform.fisNo;
                txtBelgeTarih.Value = tform.fisTarih.Oku().Date;
                txtAciklama.Text = tform.aciklama;

                lblMuhasebeAd.Text = tform.muhasebeAd;
                lblHarcamaBirimiAd.Text = tform.harcamaAd;

                hdnKod.Value = tform.kod;

                foreach (TasinirIslemDetay td in tform.detay.objeler)
                {
                    if (islemTuru > 50)//çýkýþ iþlemi ise 
                        txtFiyati.Text = td.satisBedeli.ToString("#,###.00");
                    else
                        txtFiyati.Text = td.birimFiyat.ToString("#,###.00");

                    txtAd.Text = (td.ozellik.giai + " " + td.ozellik.ekNo).Trim();
                    txtAlani.Text = td.ozellik.boyutlari;
                    txtAdano.Text = td.ozellik.satirSayisi;
                    txtPaftaNo.Text = td.ozellik.sayfaSayisi;
                    txtParselNo.Text = td.ozellik.yaprakSayisi;
                    txtEdinmeSekli.Text = td.ozellik.yayinYeri;
                    txtEdinmeTarihi.Value = td.ozellik.yayinTarihi;
                    txtKayittanCikisTarihi.Value = td.ozellik.gelisTarihi;
                    txtKayittanCikisNedeni.Text = td.ozellik.neredenGeldi;
                    ddlGayrimenkulTuru.SetValueAndFireSelect(td.hesapPlanKod);
                    txtAciklama.Text = td.ozellik.yazarAdi;
                    ddlMuhasebeIslemTipi.SetValueAndFireSelect(td.ozellik.blokajKodu);
                    txtSicilNo.Text = td.ozellik.cagi;
                    hdnPrSicilNo.Text = td.ozellik.cesidi;

                    txtAdres.Text = td.ozellik.neredeBulundu;
                }
            }
            else
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
        }

        protected void btnGonListele_Click(object sender, DirectEventArgs e)
        {
            string hata = "";

            int islemTuruSecili = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[1], 0);

            if (islemTuruSecili == (int)ENUMIslemTipi.DEVIRGIRIS)
            {
                if (txtGonMuhasebe.Text.Trim() == "")
                    hata = Resources.TasinirMal.FRMTIG067 + "<br>";

                if (txtGonHarcamaBirimi.Text.Trim() == "")
                    hata += Resources.TasinirMal.FRMTIG068 + "<br>";

                if (txtGonBelgeNo.Text.Trim() == "")
                    hata += Resources.TasinirMal.FRMTIG069 + "<br>";
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata);
                return;
            }

            txtGonBelgeNo.Text = txtGonBelgeNo.Text.Trim().PadLeft(6, '0');

            int islemTuru = 0;

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.kod = hdnKod.Text;
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
            tf.harcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
            tf.muhasebeKod = txtGonMuhasebe.Text;
            tf.fisNo = txtGonBelgeNo.Text;
            tf.devirGirisiMi = true;

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

            if (bilgi.sonuc.islemSonuc)
            {
                tf = (TNS.TMM.TasinirIslemForm)bilgi.objeler[0];
                if (tf.durum != (int)ENUMBelgeDurumu.ONAYLI)
                {
                    GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMTIG070);
                    return;
                }

                foreach (TasinirIslemDetay td in tf.detay.objeler)
                {
                    txtFiyati.Text = td.birimFiyat.ToString("#,###.00");

                    txtAd.Text = (td.ozellik.giai + " " + td.ozellik.ekNo).Trim();
                    txtAlani.Text = td.ozellik.boyutlari;
                    txtAdano.Text = td.ozellik.satirSayisi;
                    txtPaftaNo.Text = td.ozellik.sayfaSayisi;
                    txtParselNo.Text = td.ozellik.yaprakSayisi;
                    txtEdinmeSekli.Text = td.ozellik.yayinYeri;
                    txtEdinmeTarihi.Value = td.ozellik.yayinTarihi;
                    txtKayittanCikisTarihi.Value = td.ozellik.gelisTarihi;
                    txtKayittanCikisNedeni.Text = td.ozellik.neredenGeldi;
                    ddlGayrimenkulTuru.SetValueAndFireSelect(td.hesapPlanKod);
                    txtAciklama.Text = td.ozellik.yazarAdi;
                    ddlMuhasebeIslemTipi.SetValueAndFireSelect(td.ozellik.blokajKodu);
                    txtSicilNo.Text = td.ozellik.cagi;
                    hdnPrSicilNo.Text = td.ozellik.cesidi;

                    txtAdres.Text = td.ozellik.neredeBulundu;
                }

            }



        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            txtYil.Value = DateTime.Now.Year;
            txtBelgeTarih.Value = DateTime.Now.Date;
            txtBelgeNo.Clear();
            lblFormDurum.Text = "";
            ddlIslemTipi.Clear();
            ddlGayrimenkulTuru.Clear();
            txtFiyati.Clear();
            txtAdres.Clear();
            txtAd.Clear();
            txtPaftaNo.Clear();
            txtAdano.Clear();
            txtParselNo.Clear();
            txtAlani.Clear();
            txtEdinmeSekli.Clear();
            txtEdinmeTarihi.Clear();
            txtKayittanCikisNedeni.Clear();
            txtKayittanCikisTarihi.Clear();
            txtAciklama.Clear();
            ddlIslemTipi.SelectedIndex = 0;
            hdnKod.Clear();
            hdnPrSicilNo.Clear();
            txtSicilNo.Clear();
            ddlMuhasebeIslemTipi.Clear();
        }

        /// <summary>
        /// Onayla tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onaylanmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayla_Click(Object sender, DirectEventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG014 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + Resources.TasinirMal.FRMTIG015);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Value, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.kod = hdnKod.Text;

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "Onay");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG016;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Onay Kaldýr tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onayý kaldýrýlmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayKaldir_Click(object sender, DirectEventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG080 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG081 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG082 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + Resources.TasinirMal.FRMTIG083);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Value, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.kod = hdnKod.Text;

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "OnayKaldir");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG084;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        [DirectMethod]
        public void OnayaGonder(string aciklama)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG014 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + " Onaya gönderme iþlemi gerçekleþmedi.");
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Value, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.kod = hdnKod.Text;
            tf.onayAciklama = aciklama;

            Sonuc sonuc = servisTMM.TasinirIslemFisiOnayDurumDegistir(kullanan, tf, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = "B ONAYINA GÖNDERÝLDÝ";

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        protected void btnIptal_Click(object sender, DirectEventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG014 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Value, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.kod = hdnKod.Text;

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "Ýptal");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG007;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Ýþlem tipi bilgileri sunucudan çekilir ve ddlIslemTipi DropDownList kontrolüne doldurulur.
        /// </summary>
        private void IslemTipiDoldur()
        {
            List<object> listeStore = new List<object>();
            int islemTur = -1;

            List<IslemTip> islemListe = TasinirGenel.IslemTipListele(servisTMM, kullanan, false);

            foreach (IslemTip it in islemListe)
            {
                listeStore.Add(new { KOD = it.kod, AD = it.ad, TUR = it.tur, KODTUR = it.kod + "*" + it.tur });
                if (islemTur == -1)
                    islemTur = it.tur;

                if (it.ad == "Satýn Alma(Giriþ)")
                    acilistaIslemTipiSec = it.kod + "*" + it.tur;
            }

            ddlIslemTipi.GetStore().DataSource = listeStore;
            ddlIslemTipi.GetStore().DataBind();
        }

        private int IslemTuruGetir(int islemTipiKod)
        {
            List<IslemTip> islemListe = TasinirGenel.IslemTipListele(servisTMM, kullanan, false);

            foreach (IslemTip it in islemListe)
                if (it.kod == islemTipiKod)
                    return it.tur;
            return -1;
        }

        private void YZEkraniHazirla()
        {
            txtAdres.Hide();
            cmpKonum.Hide();

        }

        private void MuhasebeIslemTipiDoldur(string ekranTip)
        {
            List<object> listeStore = new List<object>();
            ObjectArray liste = servisTMM.MuhasebeIslemTipiListele(kullanan, new MuhasebeIslemTipi());

            listeStore.Add(new { KOD = "", AD = "Seçiniz" });
            foreach (MuhasebeIslemTipi mt in liste.objeler)
            {
                if (ekranTip == "GM" && mt.kod == "24")
                    continue; //Geçici Ýhraç
                listeStore.Add(new { KOD = mt.kod, AD = mt.ad });
            }

            strMuhasebeIslemTipi.DataSource = listeStore;
            strMuhasebeIslemTipi.DataBind();
        }

        public void GMTurDoldur()
        {
            string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULHESAPKOD") + "";
            hdnHesapKod.Value = "@" + hesapKodGM;

            txtAmbar.Value = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULAMBAR") + "";

            HesapPlaniSatir kriter = new HesapPlaniSatir();
            kriter.hesapKodAciklama = "@" + hesapKodGM;
            kriter.detay = true;
            ObjectArray dler = servisTMM.HesapPlaniListele(kullanan, kriter, new Sayfalama());

            List<object> liste = new List<object>();

            foreach (HesapPlaniSatir d in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = d.hesapKod,
                    AD = d.aciklama,
                });
            }
            strGMTur.DataSource = liste;
            strGMTur.DataBind();
        }

        public void YZTurDoldur()
        {
            string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMHESAPKOD") + "";
            hdnHesapKod.Value = "@" + hesapKodGM;

            txtAmbar.Value = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMAMBAR") + "";

            HesapPlaniSatir kriter = new HesapPlaniSatir();
            kriter.hesapKodAciklama = "@" + hesapKodGM;
            kriter.detay = true;
            ObjectArray dler = servisTMM.HesapPlaniListele(kullanan, kriter, new Sayfalama());

            List<object> liste = new List<object>();

            foreach (HesapPlaniSatir d in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = d.hesapKod,
                    AD = d.aciklama,
                });
            }
            strGMTur.DataSource = liste;
            strGMTur.DataBind();
        }

        public string EkranTurDondur()
        {
            string tur = Request.QueryString["gm"] + "";
            if (tur == "1") return "GM";
            else if (tur == "2") return "YZ";
            else return "";
        }
    }
}