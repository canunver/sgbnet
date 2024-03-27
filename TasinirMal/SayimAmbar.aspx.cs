using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;
using TNS;
using TNS.KYM;
using TNS.TMM;
using OrtakClass;
using Ext1.Net;
using System.Collections.Generic;
using System.Xml;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SayimAmbar : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Ambardaki Malzemelerin Sayımı";
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                txtBitisTarih.Text = DateTime.Now.ToString();

                txtMuhasebe.Value = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "AMBARSAYIMMUHASEBE");
                txtHarcamaBirimi.Value = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "AMBARSAYIMHARCAMA");
                lblSayimBirimAdi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "AMBARSAYIMHARCAMAADI");
                lblHarcamaBirimiAd.Text = lblSayimBirimAdi.Text;

                if (OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value) != "" && OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Value) != "")
                    SayimListele();
                else
                    wndBirim.Show();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBaslat control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void btnBaslat_Click(object sender, DirectEventArgs e)
        {
            if (OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value) != "")
            {
                GenelIslemler.MesajKutusu("Hata", "Devam eden bir sayım bulunmaktadır. Bu sayımı sonlandırdıktan sonra yeni sayım başlatabilirsiniz.");
                return;
            }

            SayimAmbarForm form = new SayimAmbarForm();
            form.muhasebe = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Text);
            form.harcamaBirimi = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");
            form.baslamaTarih = new TNSDateTime(DateTime.Now.ToString());
            form.adi = txtSayimAdi.Text;
            form.islemYapanAd = kullanan.ADSOYAD;
            form.islemYapanKod = kullanan.KullaniciKodu;
            form.islemTarihi = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = servisTMM.SayimAmbarFormBaslat(kullanan, form);

            if (sonuc.islemSonuc)
            {
                SayimListele();
                GenelIslemler.ExtNotification("Yeni sayım işlemi başarıyla başlatıldı", "Bilgi", Icon.Lightbulb);
                wndSayimBaslat.Hide();
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Handles the Click event of the btnBitir control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DirectEventArgs" /> instance containing the event data.</param>
        public void btnBitir_Click(object sender, DirectEventArgs e)
        {
            if (OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value) == "")
            {
                GenelIslemler.MesajKutusu("Uyarı", "Devam eden bir sayım yok. Bu nedenle sayım sonlandırma işlemi yapılamaz.");
                return;
            }

            SayimAmbarForm form = new SayimAmbarForm();
            form.kod = OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value);
            form.bitisTarih = new TNSDateTime(txtBitisTarih.Text);
            form.islemYapanAd = kullanan.ADSOYAD;
            form.islemYapanKod = kullanan.KullaniciKodu;
            form.islemTarihi = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = servisTMM.SayimAmbarFormSonlandir(kullanan, form);

            if (sonuc.islemSonuc)
            {
                hdnAktifSayimKod.Value = "";
                btnYeniAmbar.Hide();
                SayimListele();
                GenelIslemler.ExtNotification("Sayım işlemi başarıyla sonlandırıldı", "Bilgi", Icon.Lightbulb);
                wndSayimBitir.Hide();
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Listeles this instance.
        /// </summary>
        public void SayimListele()
        {
            SayimAmbarForm form = new SayimAmbarForm();
            form.muhasebe = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value);
            form.harcamaBirimi = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");

            if (form.muhasebe == "" || form.harcamaBirimi == "")
                return;

            ObjectArray bilgiler = servisTMM.SayimAmbarFormListele(kullanan, form);

            hdnAktifSayimKod.Value = "";

            if (bilgiler.sonuc.islemSonuc)
            {
                List<object> liste = new List<object>();
                foreach (SayimAmbarForm item in bilgiler.objeler)
                {
                    string durum = "BİTTİ";
                    if (item.bitisTarih.isNull)
                    {
                        hdnAktifSayimKod.Value = item.kod;
                        durum = "DEVAM EDİYOR";
                    }

                    liste.Add(new
                        {
                            KOD = item.kod,
                            MUHASEBE = item.muhasebe,
                            HARCAMABIRIMI = item.harcamaBirimi,
                            BASLAMATARIH = item.baslamaTarih.Oku(),
                            BITISTARIH = item.bitisTarih.Oku(),
                            ADI = item.adi,
                            ISLEMYAPANAD = item.islemYapanAd,
                            ISLEMYAPANKOD = item.islemYapanKod,
                            ISLEMTARIHI = item.islemTarihi,
                            DURUM = durum
                        });
                }
                storeSayim.DataSource = liste;
                storeSayim.DataBind();
            }

            if (OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value) != "")
                SayimAmbarListele(OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value));
            else
            {
                btnYeniAmbar.Hidden = true;
                lblSayimAdi.Html = "";
                storeSayimAmbar.DataSource = new List<object>();
                storeSayimAmbar.DataBind();
            }
        }

        /// <summary>
        /// Sayims the secildi.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DirectEventArgs"/> instance containing the event data.</param>
        public void SayimSecildi(object sender, DirectEventArgs e)
        {
            string sayimKod = OrtakFonksiyonlar.ConvertToStr(e.ExtraParams["Kod"]);
            SayimAmbarListele(sayimKod);
        }

        /// <summary>
        /// Sayims the Ambar listele.
        /// </summary>
        /// <param name="sayimKod">The sayim kod.</param>
        [DirectMethod]
        public void SayimAmbarListele(string sayimKod)
        {
            hdnSayimKod.Value = sayimKod;

            SayimAmbarForm sayim = new SayimAmbarForm();
            sayim.kod = sayimKod;
            ObjectArray sayBilgiler = servisTMM.SayimAmbarFormListele(kullanan, sayim);
            foreach (SayimAmbarForm s in sayBilgiler.objeler)
            {
                if (s.bitisTarih.isNull)
                    btnYeniAmbar.Hidden = false;
                else
                    btnYeniAmbar.Hidden = true;

                lblSayimAdi.Html = "<b>" + s.adi + "</b> isimli sayımda işlem göre kişilerin listesi";
            }

            Sayimambar form = new Sayimambar();
            form.sayimKod = sayimKod;
            ObjectArray bilgiler = servisTMM.SayimAmbarAmbarListele(kullanan, form);

            List<object> liste = new List<object>();
            string durum = "";
            if (bilgiler.sonuc.islemSonuc)
            {
                foreach (Sayimambar item in bilgiler.objeler)
                {
                    if (item.fazla > 0) durum = "FAZLA";
                    else if (item.zimmetli > item.sayilan && item.sayilan > 0) durum = "EKSİK";
                    else if (item.zimmetli > item.sayilan || (item.zimmetli == 0 || item.sayilan == 0)) durum = "SAYILMADI";
                    else if (item.zimmetli == item.sayilan) durum = "TAMAM";

                    liste.Add(new
                    {
                        KOD = item.kod,
                        SAYIMKOD = item.sayimKod,
                        AMBARAD = item.ambarAd,
                        AMBARKOD = item.ambarKod,
                        ODAAD = item.odaAd,
                        ODAKOD = item.odaKod,
                        ISLEMYAPANAD = item.islemYapanAd,
                        ISLEMYAPANKOD = item.islemYapanKod,
                        ISLEMTARIHI = item.islemTarihi.Oku(),
                        DURUM = durum,
                    });
                }
            }
            storeSayimAmbar.DataSource = liste;
            storeSayimAmbar.DataBind();
        }

        /// <summary>
        /// Handles the Click event of the btnMuhasebeSecim control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DirectEventArgs"/> instance containing the event data.</param>
        public void btnMuhasebeSecim_Click(object sender, DirectEventArgs e)
        {
            string muhasebe = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value);
            string harcamaBirimi = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Value);
            string harcamaBirimiAdi = servisUZY.UzayDegeriStr(null, "TASHARCAMABIRIM", muhasebe + "-" + harcamaBirimi, true, "");

            if (muhasebe == "")
            {
                GenelIslemler.MesajKutusu("Hata", "Muhasebe birimi seçimi yapınız");
                return;
            }
            if (harcamaBirimi == "")
            {
                GenelIslemler.MesajKutusu("Hata", "Harcama birimi seçimi yapınız");
                return;
            }
            if (harcamaBirimiAdi == "")
            {
                GenelIslemler.MesajKutusu("Hata", "Harcama birimi seçimi yapınız");
                return;
            }

            SayimAmbarForm sayim = new SayimAmbarForm();
            sayim.muhasebe = muhasebe;
            sayim.harcamaBirimi = harcamaBirimi;
            Sonuc sonuc = servisTMM.SayimAmbarBirimeYetkilimi(kullanan, sayim);
            if (!sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                return;
            }


            GenelIslemler.KullaniciDegiskenSakla(kullanan, "AMBARSAYIMMUHASEBE", muhasebe);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "AMBARSAYIMHARCAMA", harcamaBirimi);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "AMBARSAYIMHARCAMAADI", harcamaBirimiAdi);

            lblSayimBirimAdi.Text = harcamaBirimiAdi;

            SayimListele();
            wndBirim.Hide();
        }

        /// <summary>
        /// Handles the Click event of the btnAmbarSayim control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DirectEventArgs"/> instance containing the event data.</param>
        public void btnYeniAmbarSayimBaslat_Click(object sender, DirectEventArgs e)
        {
            string ambarKod = txtAmbar.Text;

            Sayimambar ambar = new Sayimambar();
            ambar.ambarAd = "";//Sunucuda dolduruluyor
            ambar.ambarKod = ambarKod;
            ambar.sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value);
            ambar.islemYapanAd = kullanan.ADSOYAD;
            ambar.islemYapanKod = kullanan.KullaniciKodu;
            ambar.islemTarihi = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = servisTMM.SayimAmbarYeniAmbarSay(kullanan, ambar);
            if (!sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                return;
            }
            else
            {
                wndYeniAmbarSay.Hide();
                SayimAmbarListele(OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value));

                AmbarDetayEkraniAc(OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value), ambarKod);
                wndSayimAmbarDetay.Title = "Sayım (" + ambarKod + " nolu ambar)";
                wndSayimAmbarDetay.Show();
            }
        }

        [DirectMethod]
        public void AmbarDetayEkraniAc(string sayimKod, string ambarKod)
        {
            hdnSayimDetaySayimKod.Value = sayimKod;
            hdnSayimDetayAmbarKod.Value = ambarKod;

            SayimAmbarDetay form = new SayimAmbarDetay();
            form.ambarKod = ambarKod;
            form.sayimKod = sayimKod;

            ObjectArray bilgiler = servisTMM.SayimAmbarAmbarDetayListele(kullanan, form);

            List<object> listeAmbar = new List<object>();
            List<object> listeFazla = new List<object>();
            int sayacFazla = 0;
            int sayacZimmet = 0;
            int sayacOkunan = 0;
            if (bilgiler.sonuc.islemSonuc)
            {
                foreach (SayimAmbarDetay item in bilgiler.objeler)
                {
                    object o = new
                        {
                            KOD = item.kod,
                            SAYIMKOD = item.sayimKod,
                            KISIKOD = item.ambarKod,
                            FAZLAAMBARAD = item.fazlaAmbarAd,
                            PRSICILNO = item.prSicilNo,
                            SICILNO = item.sicilNo,
                            MALZEMEAD = item.malzemeAd,
                            OKUNDU = item.okundu,
                        };

                    if (item.fazla == 0)
                    {
                        listeAmbar.Add(o);
                        sayacZimmet++;
                        if (item.okundu > 0) sayacOkunan++;
                    }
                    else
                    {
                        listeFazla.Add(o);
                        sayacFazla++;
                    }
                }
            }

            string durumYazi = "{0} adet zimmetli malzemenin {1} tanesi sayıldı";
            durumYazi = string.Format(durumYazi, sayacZimmet, sayacOkunan);

            if (sayacOkunan != sayacZimmet)
                prgDurum.UpdateProgress(((float)sayacOkunan / (float)sayacZimmet), durumYazi);
            else
                prgDurum.UpdateProgress(1, "Bütün malzemeler sayıldı");

            storeZimmet.DataSource = listeAmbar;
            storeZimmet.DataBind();

            storeZimmetFazla.DataSource = listeFazla;
            storeZimmetFazla.DataBind();
        }

        /// <summary>
        /// Sicils the no okutuldu.
        /// </summary>
        /// <param name="sicilNo">The sicil no.</param>
        [DirectMethod]
        public void SicilNoOkutuldu(string sicilNo)
        {
            string sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimKod.Value);
            string ambarKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetayAmbarKod.Value);
            if (string.IsNullOrEmpty(sicilNo))
            {
                return;
            }

            TNS.TMM.SayimAmbarDetay kriter = new SayimAmbarDetay();
            kriter.sicilNo = sicilNo;//Sicilno boş ise bütün dataların okundusu 1 olarak atar
            kriter.ambarKod = ambarKod;
            kriter.sayimKod = sayimKod;
            ObjectArray bilgiler = servisTMM.SayimAmbarAmbarDetayListele(kullanan, kriter);
            if (bilgiler.objeler.Count > 0)
            {
                foreach (SayimAmbarDetay zk in bilgiler.objeler)
                {
                    if (zk.ambarKod == ambarKod)
                    {
                        SayimAmbarDetay skd = new SayimAmbarDetay();
                        skd.kod = zk.kod;
                        skd.sayimKod = sayimKod;
                        skd.ambarKod = ambarKod;
                        skd.sicilNo = zk.sicilNo;
                        skd.prSicilNo = zk.prSicilNo;
                        skd.okundu = 1;
                        skd.islemYapanAd = kullanan.ADSOYAD;
                        skd.islemYapanKod = kullanan.KullaniciKodu;
                        skd.islemTarihi = new TNSDateTime(DateTime.Now);

                        Sonuc sonuc = servisTMM.SayimAmbarDetayOkundu(kullanan, skd);
                    }
                }
            }
            else
            {
                SicilNoHareket form = new SicilNoHareket();
                form.sicilNo = sicilNo;
                form.muhasebeKod = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value);
                form.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
                ObjectArray kontrol = servisTMM.BarkodSicilNoListele(kullanan, form, new Sayfalama());
                foreach (SicilNoHareket item in kontrol.objeler)
                {
                    string fazlaAciklama = item.kimeGitti;
                    if (fazlaAciklama == "") fazlaAciklama = item.ambarKod + " nolu AMBAR";
                    if (fazlaAciklama != "") fazlaAciklama += " (" + item.harcamaBirimAd + ")";
                    string fazlaKod = item.kisiAd;
                    if (fazlaKod == "") fazlaKod = item.ambarKod;

                    SayimAmbarDetay skd = new SayimAmbarDetay();
                    skd.sayimKod = sayimKod;
                    skd.ambarKod = ambarKod;
                    skd.prSicilNo = item.prSicilNo;
                    skd.sicilNo = item.sicilNo;
                    skd.malzemeAd = item.hesapPlanAd;
                    skd.okundu = 1;
                    skd.fazla = 1;
                    skd.fazlaAmbarKod = fazlaKod;
                    skd.fazlaAmbarAd = fazlaAciklama;
                    skd.islemYapanAd = kullanan.ADSOYAD;
                    skd.islemYapanKod = kullanan.KullaniciKodu;
                    skd.islemTarihi = new TNSDateTime(DateTime.Now);

                    Sonuc sonuc = servisTMM.SayimAmbarDetayOkundu(kullanan, skd);
                }
            }
            AmbarDetayEkraniAc(sayimKod, ambarKod);

            txtSicil.Text = "";
        }

        [DirectMethod]
        public void FazlaSil(string kod)
        {
            SayimAmbarDetay skd = new SayimAmbarDetay();
            skd.kod = kod;

            Sonuc sonuc = servisTMM.SayimAmbarFazlaSil(kullanan, skd);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                string sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimKod.Value);
                string ambarKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetayAmbarKod.Value);
                AmbarDetayEkraniAc(sayimKod, ambarKod);
                GenelIslemler.ExtNotification("Fazla malzeme başarıyla silindi", "Bilgi", Icon.Lightbulb);

            }
        }

        [DirectMethod]
        public void Okundu(string sicilNo, int okundu)
        {
            string sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimKod.Value);
            string ambarKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetayAmbarKod.Value);

            TNS.TMM.SayimAmbarDetay kriter = new SayimAmbarDetay();
            kriter.sicilNo = sicilNo;
            //kriter.muhasebeKod = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value);
            //kriter.harcamaBirimKod = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");
            kriter.ambarKod = ambarKod;
            ObjectArray bilgiler = servisTMM.SayimAmbarAmbarDetayListele(kullanan, kriter);
            if (bilgiler.objeler.Count > 0)
            {
                foreach (SayimAmbarDetay zk in bilgiler.objeler)
                {
                    if (zk.ambarKod == ambarKod)
                    {
                        if (zk.ambarKod == ambarKod)
                        {
                            SayimAmbarDetay skd = new SayimAmbarDetay();
                            skd.kod = zk.kod;
                            skd.sayimKod = sayimKod;
                            skd.ambarKod = ambarKod;
                            skd.sicilNo = zk.sicilNo;
                            skd.prSicilNo = zk.prSicilNo;
                            if (okundu < 1)
                                skd.okundu = 1;
                            else
                                skd.okundu = 0;
                            skd.islemYapanAd = kullanan.ADSOYAD;
                            skd.islemYapanKod = kullanan.KullaniciKodu;
                            skd.islemTarihi = new TNSDateTime(DateTime.Now);

                            Sonuc sonuc = servisTMM.SayimAmbarDetayOkundu(kullanan, skd);
                        }
                        AmbarDetayEkraniAc(sayimKod, ambarKod);

                        txtSicil.Text = "";
                    }
                }
            }
            else
            {
                SicilNoHareket form = new SicilNoHareket();
                form.sicilNo = sicilNo;
                form.muhasebeKod = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value);
                form.harcamaBirimKod = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");
                form.ambarKod = ambarKod;
                ObjectArray kontrol = servisTMM.BarkodSicilNoListele(kullanan, form, new Sayfalama());
                foreach (SicilNoHareket item in kontrol.objeler)
                {
                    if (!string.IsNullOrEmpty(item.kimeGitti) && item.ambarKod == ambarKod)
                    {
                        SayimAmbarDetay skd = new SayimAmbarDetay();
                        skd.sayimKod = sayimKod;
                        skd.ambarKod = ambarKod;
                        skd.sicilNo = item.sicilNo;
                        skd.prSicilNo = item.prSicilNo;
                        skd.okundu = 1;
                        skd.fazla = 1;
                        skd.islemYapanAd = kullanan.ADSOYAD;
                        skd.islemYapanKod = kullanan.KullaniciKodu;
                        skd.islemTarihi = new TNSDateTime(DateTime.Now);

                        Sonuc sonuc = servisTMM.SayimAmbarDetayOkundu(kullanan, skd);
                    }
                }
                if (kontrol.objeler.Count > 0)
                {
                    GenelIslemler.ExtNotification("Mükerrer okuma yapıldı. Okunan malzeme fazla malzeme listesinde mevcut", "Bilgi", Icon.Lightbulb);
                    txtSicil.Text = "";
                    return;
                }
                AmbarDetayEkraniAc(sayimKod, ambarKod);

                txtSicil.Text = "";
            }
        }

        protected void btnRapor_Click(object sender, DirectEventArgs e)
        {
            Sayimambar kriter = new Sayimambar();
            kriter.sayimKod = hdnSayimKod.Value.ToString();
            kriter.harcamabirimKod = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");
            //kriter.ambarKod = hdnSayimDetayAmbarKod.Value.ToString();

            ObjectArray liste = servisTMM.SayimAmbarRapor(kullanan, kriter);

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int satir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            XLS.DosyaAc(Server.MapPath("~") + "/RaporSablon/TMM/" + "SayimAmbarRaporu.xlt", sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("muhasebe", txtMuhasebe.Text);
            XLS.HucreAdBulYaz("birim", txtHarcamaBirimi.Text);

            satir = kaynakSatir;
            bool baslikTamam = true;
            bool baslikFazla = true;
            bool baslikEksik = true;

            foreach (Sayimambar item in liste.objeler)
            {
                if (item.okundu > 0 && item.fazla == 0)
                {
                    if (baslikTamam)
                    {
                        XLS.HucreAdBulYaz("sayimAd", item.sayimAd);
                        if (!string.IsNullOrEmpty(item.bitisTarihi.ToString()))
                            XLS.HucreAdBulYaz("tarihler", item.baslamaTarihi + " - " + item.bitisTarihi);
                        XLS.HucreAdBulYaz("tarihler", item.baslamaTarihi.ToString());

                        satir++;
                        AraBaslikKoy(XLS, satir, "Sayılan Malzemeler");
                        baslikTamam = false;
                    }
                    satir++;
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, item.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, item.malzemeAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, item.ambarAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, item.fazlaAmbarAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, item.islemTarihi.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, item.islemYapanAd);
                }
                else if (item.okundu > 0 && item.fazla > 0)
                {
                    if (baslikFazla)
                    {
                        satir++;
                        AraBaslikKoy(XLS, satir, "Fazla Malzemeler");
                        baslikFazla = false;
                    }
                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, item.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, item.malzemeAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, item.ambarAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, item.fazlaAmbarAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, item.islemTarihi.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, item.islemYapanAd);
                }
                else if (item.okundu == 0)
                {
                    if (baslikEksik)
                    {
                        satir++;
                        AraBaslikKoy(XLS, satir, "Eksik Malzemeler");
                        baslikEksik = false;
                    }
                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, item.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, item.malzemeAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, item.ambarAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, item.fazlaAmbarAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, item.islemTarihi.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, item.islemYapanAd);
                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "SayimAmbarRaporu", true, GenelIslemler.ExcelTur());
        }

        void AraBaslikKoy(Tablo XLS, int satir, string baslik)
        {
            XLS.HucreDegerYaz(satir, 0, baslik);
            XLS.ArkaPlanRenk(satir, 0, satir, 5, System.Drawing.Color.LightYellow);
            XLS.KoyuYap(satir, 0, satir, 5, true);
        }

        //    string sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimKod.Value);
        //    string ambarKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetayAmbarKod.Value);

        //    TNS.TMM.SicilNoHareket kriter = new SicilNoHareket();
        //    kriter.sicilNo = sicilNo;
        //    kriter.muhasebeKod = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value);
        //    kriter.harcamaBirimKod = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");
        //    kriter.ambarKod = ambarKod;
        //    ObjectArray bilgiler = servisTMM.SicilRaporu(kullanan, kriter);
        //    foreach (SicilNoHareket zk in bilgiler.objeler)
        //    {
        //        if (zk.ambarKod == ambarKod)
        //        {
        //            SayimAmbarDetay skd = new SayimAmbarDetay();
        //            skd.sayimKod = sayimKod;
        //            skd.ambarKod = ambarKod;
        //            skd.sicilNo = zk.sicilNo;
        //            skd.prSicilNo = zk.prSicilNo;
        //            if (okundu < 1)
        //                skd.okundu = 1;
        //            else
        //                skd.okundu = 0;
        //            skd.islemYapanAd = kullanan.ADSOYAD;
        //            skd.islemYapanKod = kullanan.KullaniciKodu;
        //            skd.islemTarihi = new TNSDateTime(DateTime.Now);

        //            Sonuc sonuc = servisTMM.SayimAmbarDetayOkundu(kullanan, skd);
        //        }
        //        else
        //        {
        //            SayimAmbarDetay form = new SayimAmbarDetay();
        //            form.ambarKod = ambarKod;
        //            form.sayimKod = sayimKod;
        //            form.prSicilNo = zk.prSicilNo;
        //            ObjectArray kontrol = servisTMM.SayimAmbarAmbarDetayListele(kullanan, form);
        //            if (kontrol.objeler.Count > 0)
        //            {
        //                GenelIslemler.ExtNotification("Mükerrer okuma yapıldı. Okunan malzeme fazla malzeme listesinde mevcut", "Bilgi", Icon.Lightbulb);
        //                txtSicil.Text = "";
        //                return;
        //            }

        //            SayimAmbarDetay skd = new SayimAmbarDetay();
        //            skd.ambarKod = ambarKod;
        //            skd.prSicilNo = zk.prSicilNo;
        //            skd.sicilNo = zk.sicilNo;
        //            skd.okundu = 1;
        //            skd.fazla = 1;
        //            skd.fazlaAmbarAd = zk.ambarAd;
        //            skd.fazlaAmbarKod = zk.ambarKod;
        //            skd.malzemeAd = zk.hesapPlanAd;
        //            skd.sayimKod = sayimKod;
        //            skd.islemYapanAd = kullanan.ADSOYAD;
        //            skd.islemYapanKod = kullanan.KullaniciKodu;
        //            skd.islemTarihi = new TNSDateTime(DateTime.Now);

        //            ObjectArray skdListe = new ObjectArray();
        //            skdListe.objeler.Add(skd);

        //            Sonuc sonuc = servisTMM.SayimAmbarDetayKaydet(kullanan, skdListe);
        //        }
        //    }

        //    AmbarDetayEkraniAc(sayimKod, ambarKod);

        //    txtSicil.Text = "";
        //}

    }
}
