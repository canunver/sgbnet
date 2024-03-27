using Ext1.Net;
using OrtakClass;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS;
using TNS.KYM;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Geçici alýndý fiþi bilgilerinin kayýt ve listeleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class SicilNoDegerArtisForm : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Sicil No Deðer Artýþý";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

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

                txtBelgeTarihi.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                string gelenPrSicilNo = Request.QueryString["prSicilNo"] + "";
                if (gelenPrSicilNo != "")
                {
                    SicilNoHareket shBilgi = new SicilNoHareket();
                    shBilgi.prSicilNo = OrtakFonksiyonlar.ConvertToInt(gelenPrSicilNo, 0);
                    ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
                    foreach (SicilNoHareket sh in bilgi.objeler)
                    {
                        txtMuhasebe.Text = sh.muhasebeKod;
                        txtHarcamaBirimi.Text = sh.harcamaBirimKod;
                        txtSicilNo.Text = sh.sicilNo;
                        lblSicilNo.Text = sh.ozellik.giai + " " + sh.ozellik.ekNo;
                    }

                    decimal sonBedel = servisTMM.SonBedelGetir(kullanan, OrtakFonksiyonlar.ConvertToInt(gelenPrSicilNo, 0));
                    hdnSonBedel.Value = sonBedel;
                }
            }
        }

        protected void btnKaydet_Click(Object sender, DirectEventArgs e)
        {
            SicilNoDegerArtis tf = new SicilNoDegerArtis();
            tf.kod = hdnKod.Value.ToString();
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.belgeNo = txtBelgeNo.Text;
            tf.belgeTarihi = new TNSDateTime(txtBelgeTarihi.RawText);
            tf.prSicilNo = OrtakFonksiyonlar.ConvertToInt(hdnPrSicilNo.Value, 0);
            tf.gorSicilNo = txtSicilNo.Text.ToString();
            tf.tur = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlTur), 0);
            if (tf.tur == 1 && rdKaysayi.Checked) //Enflasyon Katsayýsý
                tf.katsayi = OrtakFonksiyonlar.ConvertToDouble(txtArtisTutar.Text, (double)0);
            else
                tf.tutar = OrtakFonksiyonlar.ConvertToDouble(txtArtisTutar.Text, (double)0);
            tf.gerekce = txtGerekce.Text;

            string hata = "";
            if (tf.tur == 1 && tf.tutar == 0)
            {
                //if (tf.katsayi <= 100)
                //    hata += "Enflasyon düzeltme oraný 100 den büyük olmalýdýr.<br>";
            }
            else if (tf.tutar == 0)
                hata += "Tutar bilgisi boþ býrakýlamaz.<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            Sonuc sonuc = servisTMM.SicilNoDegerArtisKaydet(kullanan, tf);
            if (sonuc.islemSonuc)
            {
                DurumAdDegistir(txtBelgeNo.Text == "" ? (int)ENUMBelgeDurumu.YENI : (int)ENUMBelgeDurumu.DEGISTIRILDI);

                string[] deger = sonuc.anahtar.Split(',');
                string kod = deger[0];
                string belgeNo = deger[1];

                hdnKod.Value = kod;
                txtBelgeNo.Text = belgeNo;
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            SicilNoDegerArtis sd = new SicilNoDegerArtis();
            sd.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            sd.muhasebeKod = txtMuhasebe.Text;
            sd.belgeNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            if (OrtakFonksiyonlar.ConvertToInt(sd.belgeNo, 0) == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Ýþlem Numarasý boþ býrakýlamaz.");
                return;
            }

            ObjectArray bilgi = servisTMM.SicilNoDegerArtisListele(kullanan, sd);

            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listelenecek kayýt bulunamadý." + bilgi.sonuc.hataStr);
                return;
            }

            SicilNoDegerArtis tf = (SicilNoDegerArtis)bilgi[0];

            SicilNoHareket shBilgi = new SicilNoHareket();
            shBilgi.prSicilNo = tf.prSicilNo;
            ObjectArray malzeme = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
            string malzemeAdi = "";
            foreach (SicilNoHareket sh in malzeme.objeler)
            {
                string ozellik = "";

                if (!String.IsNullOrEmpty(sh.ozellik.giai))
                    ozellik = sh.ozellik.giai;
                if (!String.IsNullOrEmpty(sh.ozellik.ekNo))
                {
                    if (ozellik != "") ozellik += " ";
                    ozellik += sh.ozellik.ekNo;
                }

                string adi = ozellik;
                if (adi != "") adi += " - ";
                malzemeAdi += sh.hesapPlanAd;
            }

            hdnKod.Value = tf.kod;
            txtBelgeNo.Text = tf.belgeNo;
            txtBelgeTarihi.Text = tf.belgeTarihi.ToString();
            txtSicilNo.Text = tf.gorSicilNo.ToString();
            hdnPrSicilNo.Value = tf.prSicilNo.ToString();
            txtArtisTutar.Text = (tf.tur == 3 ? Math.Abs(tf.tutar).ToString() : tf.tutar.ToString());
            txtGerekce.Text = tf.gerekce;
            ddlTur.SetValueAndFireSelect(tf.tur.ToString());
            lblSicilNo.Text = malzemeAdi;

            DurumAdDegistir(tf.durum);
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            hdnKod.Clear();
            txtBelgeTarihi.Value = DateTime.Now.Date;
            txtBelgeNo.Clear();
            txtArtisTutar.Clear();
            txtGerekce.Clear();
            ddlTur.SetValueAndFireSelect("1");

            string gelenPrSicilNo = Request.QueryString["prSicilNo"] + "";
            if (gelenPrSicilNo == "") txtSicilNo.Clear();

            lblFormDurum.Text = "";
            if (txtSicilNo.Text == "") hdnPrSicilNo.Value = "";
            if (txtMuhasebe.Text == "") lblMuhasebeAd.Text = "";
            if (txtHarcamaBirimi.Text == "") lblHarcamaBirimiAd.Text = "";
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

            SicilNoDegerArtis tf = new SicilNoDegerArtis();

            tf.belgeNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Value.ToString().Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Value.ToString().Replace(".", "");
            tf.kod = hdnKod.Value.ToString();

            Sonuc sonuc = servisTMM.SicilNoDegerArtisDurumGuncelle(kullanan, tf, "Onay");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG016;

                GenelIslemler.ExtNotification(sonuc.bilgiStr, "Bilgi", Icon.Information);
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

            SicilNoDegerArtis tf = new SicilNoDegerArtis();

            tf.belgeNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Value.ToString().Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Value.ToString().Replace(".", "");
            tf.kod = hdnKod.Value.ToString();

            Sonuc sonuc = servisTMM.SicilNoDegerArtisDurumGuncelle(kullanan, tf, "OnayKaldir");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG084;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        protected void btnOnayaGonder_Click(Object sender, DirectEventArgs e)
        {
            string hata = "";

            if (txtMuhasebe.Value.ToString().Replace(".", "") == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (txtHarcamaBirimi.Value.ToString().Replace(".", "") == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG014 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + " Onaya gönderme iþlemi gerçekleþmedi.");
                return;
            }

            SicilNoDegerArtis tf = new SicilNoDegerArtis();

            tf.belgeNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Value.ToString().Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Value.ToString().Replace(".", "");
            tf.kod = hdnKod.Value.ToString();

            ObjectArray bilgi = servisTMM.SicilNoDegerArtisListele(kullanan, tf);

            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listelenecek kayýt bulunamadý." + bilgi.sonuc.hataStr);
                return;
            }

            SicilNoDegerArtis sd = (SicilNoDegerArtis)bilgi[0];
            if (sd.durum != 1)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Belgenin durumu uygun olmadýðý için iþlem yapýlamaz.");
                return;
            }

            string ilgiliAmbarKod = "";

            SicilNoHareket shBilgi = new SicilNoHareket();
            shBilgi.prSicilNo = sd.prSicilNo;
            ObjectArray prBilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
            foreach (SicilNoHareket sh in prBilgi.objeler)
            {
                ilgiliAmbarKod = sh.ambarKod;
            }

            if (string.IsNullOrEmpty(ilgiliAmbarKod))
            {
                GenelIslemler.MesajKutusu("Uyarý", "Malzemenin bulunduðu ambar bilgisi alýnamadý.");
                return;
            }

            //***********************************************************************
            //***********************************************************************
            AmortismanIslemForm form = new AmortismanIslemForm()
            {
                yil = new TNSDateTime(txtBelgeTarihi.Text).Oku().Year,
                donem = OrtakFonksiyonlar.ConvertToInt(txtBelgeNo.Text, 0),
                muhasebeKod = txtMuhasebe.Value.ToString().Replace(".", ""),
                harcamaKod = txtHarcamaBirimi.Value.ToString().Replace(".", ""),
                ambarKod = ilgiliAmbarKod,
                tip = "DEGERARTIS",
                durum = (int)ENUMTasinirIslemFormOnayDurumu.TANIMSIZ,
                onayDurum = (int)ENUMTasinirIslemFormOnayDurumu.TANIMSIZ
            };
            Sonuc sonuc = servisTMM.AmortismanIslemFisiKaydet(kullanan, form);//BA Onayýna göndermek için burasý kullanýlýyor
                                                                              //***********************************************************************
            AmortismanIslemForm from = new AmortismanIslemForm()
            {
                yil = new TNSDateTime(txtBelgeTarihi.Text).Oku().Year,
                donem = OrtakFonksiyonlar.ConvertToInt(txtBelgeNo.Text, 0),
                muhasebeKod = txtMuhasebe.Value.ToString().Replace(".", ""),
                harcamaKod = txtHarcamaBirimi.Value.ToString().Replace(".", ""),
                ambarKod = ilgiliAmbarKod,
                tip = "DEGERARTIS",
            };

            sonuc = servisTMM.AmortismanIslemFisiOnayDurumDegistir(kullanan, from, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);
            //***********************************************************************

            sonuc = servisTMM.SicilNoDegerArtisDurumGuncelle(kullanan, tf, "OnayaGonder");

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
                GenelIslemler.MesajKutusu("Hata", hata + Resources.TasinirMal.FRMTIG015);
                return;
            }

            SicilNoDegerArtis tf = new SicilNoDegerArtis();

            tf.belgeNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Value.ToString().Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Value.ToString().Replace(".", "");
            tf.kod = hdnKod.Value.ToString();

            Sonuc sonuc = servisTMM.SicilNoDegerArtisDurumGuncelle(kullanan, tf, "Iptal");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMZFG007;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        public static string RaporHazirla(ITMMServis servisTMM, Kullanici kullanan, int yil, string fisNo, SicilNoDegerArtis sd, string sonucDosyaAd)
        {
            sd.belgeNo = sd.belgeNo.PadLeft(6, '0');

            return DegerArtisiRaporla(servisTMM, kullanan, yil, fisNo, sd, sonucDosyaAd);
        }

        private static string DegerArtisiRaporla(ITMMServis servisTMM, Kullanici kullanan, int yil, string fisNo, SicilNoDegerArtis sd, string sonucDosyaAd)
        {
            ObjectArray bilgi = servisTMM.SicilNoDegerArtisListele(kullanan, sd);
            SicilNoDegerArtis tf = (SicilNoDegerArtis)bilgi[0];

            SicilNoHareket shBilgi = new SicilNoHareket();
            shBilgi.prSicilNo = tf.prSicilNo;
            ObjectArray malzeme = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
            string malzemeAdi = "";
            foreach (SicilNoHareket sh in malzeme.objeler)
            {
                string ozellik = "";

                if (!String.IsNullOrEmpty(sh.ozellik.giai))
                    ozellik = sh.ozellik.giai;
                if (!String.IsNullOrEmpty(sh.ozellik.ekNo))
                {
                    if (ozellik != "") ozellik += " ";
                    ozellik += sh.ozellik.ekNo;
                }

                string adi = ozellik;
                if (adi != "") adi += " - ";
                malzemeAdi += sh.sicilNo + " / " + sh.hesapPlanAd;
            }

            AmortismanBilgi amb = TasinirGenel.AmortismanBilgileri(servisTMM, kullanan, tf.prSicilNo, tf.muhasebeKod, tf.harcamaKod);

            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");
            string sablonAd = raporSablonYol + "\\TMM\\" + "DegerArtisiForm.xlt";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            string islemTuru = (tf.tur == 1 ? "Enflasyon Düzeltme Farký" : (tf.tur == 2 ? "Deðer Artýþý" : "Deðer Azalýþý"));
            XLS.HucreAdBulYaz("tur", islemTuru);
            XLS.HucreAdBulYaz("muhasebe", tf.muhasebeKod + "-" + tf.muhasebeAd);
            XLS.HucreAdBulYaz("harcama", tf.harcamaKod + "-" + tf.harcamaAd);
            XLS.HucreAdBulYaz("islemTarihi", tf.belgeTarihi.ToString());
            XLS.HucreAdBulYaz("islemNumarasi", tf.belgeNo);
            XLS.HucreAdBulYaz("malzeme", malzemeAdi);
            XLS.HucreAdBulYaz("sonBedel", amb.sonBedel);
            XLS.HucreAdBulYaz("birikenAmortisman", amb.duzBirikenAmortisman);
            XLS.HucreAdBulYaz("tutar", tf.tutar);
            XLS.HucreAdBulYaz("gerekce", tf.gerekce);



            //Tarihçe bilgisi taþýnýr iþlem fiþi, deðer atrýþý ve amortisman iþlemlerinin B/A onay bilgilerini okumak için eklenmiþtir.
            MuhasebeIslemiKriter formBA = new MuhasebeIslemiKriter
            {
                yil = yil,
                muhasebeKod = tf.muhasebeKod,
                harcamaKod = tf.harcamaKod,
                fisNo = fisNo
            };

            ObjectArray listeTarihce = servisTMM.BAOnayiTarihceListele(formBA);
            string islemKontrol = "";
            foreach (TarihceBilgisi t in listeTarihce.objeler)
            {
                if (t.islem == "Deðiþtirildi")
                    continue;

                if (t.islem == "B Onayýna Gönderildi")
                {
                    formBA.girisZaman = t.islemTarih;
                    formBA.girisSicil = t.islemYapan + " - " + t.islemYapanAd;
                    islemKontrol = "B Onayýna Gönderildi";
                }
                else if (t.islem == "A Onayýna Gönderildi" && islemKontrol == "B Onayýna Gönderildi")
                {
                    formBA.bOnayZaman = t.islemTarih;
                    formBA.bOnaySicil = t.islemYapan + " - " + t.islemYapanAd;
                    islemKontrol = "A Onayýna Gönderildi";
                }
                else if (t.islem == "Onaylandý" && (islemKontrol == "A Onayýna Gönderildi" || islemKontrol == "Onaylandý"))
                {
                    formBA.aOnayZaman = t.islemTarih;
                    formBA.aOnaySicil = t.islemYapan + " - " + t.islemYapanAd;
                    islemKontrol = "Onaylandý";
                }
                else
                {
                    formBA.bOnayZaman = new TNSDateTime();
                    formBA.bOnaySicil = "";
                    formBA.aOnayZaman = new TNSDateTime();
                    formBA.aOnaySicil = "";
                }
            }
            //***************************************************

            XLS.HucreAdBulYaz("FisiYonlendiren", formBA.girisSicil);
            XLS.HucreAdBulYaz("BOnayi", formBA.bOnaySicil);
            XLS.HucreAdBulYaz("AOnayi", formBA.aOnaySicil);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void DurumAdDegistir(int durum)
        {
            string durumAd = "";
            if (durum == (int)ENUMBelgeDurumu.YENI || durum == (int)ENUMBelgeDurumu.DEGISTIRILDI || durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                durumAd = Resources.TasinirMal.FRMTIG055;
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                durumAd = Resources.TasinirMal.FRMTIG056;
            else if (durum == (int)ENUMBelgeDurumu.IPTAL)
                durumAd = Resources.TasinirMal.FRMTIG057;
            else if (durum == 3)// (int)ENUMBelgeDurumu.TANIMSIZ)
                durumAd = "BELGE ONAYA GÖNDERÝLDÝ";

            lblFormDurum.Text = durumAd;
        }

    }
}