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
    public partial class SayimKisi : TMMSayfaV2
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
                formAdi = "Kişi Üzerindeki Malzemelerin Sayımı";
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                KisiDoldur();
                OdaDoldur();
                txtBitisTarih.Text = DateTime.Now.ToString();

                txtMuhasebe.Value = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "KISISAYIMMUHASEBE");
                txtHarcamaBirimi.Value = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "KISISAYIMHARCAMA");
                lblSayimBirimAdi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "KISISAYIMHARCAMAADI");
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

            SayimKisiForm form = new SayimKisiForm();
            form.muhasebe = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Text);
            form.harcamaBirimi = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");
            form.baslamaTarih = new TNSDateTime(DateTime.Now.ToString());
            form.adi = txtSayimAdi.Text;
            form.islemYapanAd = kullanan.ADSOYAD;
            form.islemYapanKod = kullanan.KullaniciKodu;
            form.islemTarihi = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = servisTMM.SayimKisiFormBaslat(kullanan, form);

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

            SayimKisiForm form = new SayimKisiForm();
            form.kod = OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value);
            form.bitisTarih = new TNSDateTime(txtBitisTarih.Text);
            form.islemYapanAd = kullanan.ADSOYAD;
            form.islemYapanKod = kullanan.KullaniciKodu;
            form.islemTarihi = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = servisTMM.SayimKisiFormSonlandir(kullanan, form);

            if (sonuc.islemSonuc)
            {
                hdnAktifSayimKod.Value = "";
                btnYeniKisi.Hide();
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
            SayimKisiForm form = new SayimKisiForm();
            form.muhasebe = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Value);
            form.harcamaBirimi = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");

            if (form.muhasebe == "" || form.harcamaBirimi == "")
                return;

            ObjectArray bilgiler = servisTMM.SayimKisiFormListele(kullanan, form);

            hdnAktifSayimKod.Value = "";

            if (bilgiler.sonuc.islemSonuc)
            {
                List<object> liste = new List<object>();
                foreach (SayimKisiForm item in bilgiler.objeler)
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
                SayimKisiListele(OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value));
            else
            {
                btnYeniKisi.Hidden = true;
                lblSayimAdi.Html = "";
                storeSayimKisi.DataSource = new List<object>();
                storeSayimKisi.DataBind();
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
            SayimKisiListele(sayimKod);
        }

        /// <summary>
        /// Sayims the kisi listele.
        /// </summary>
        /// <param name="sayimKod">The sayim kod.</param>
        [DirectMethod]
        public void SayimKisiListele(string sayimKod)
        {
            hdnSayimKod.Value = sayimKod;

            SayimKisiForm sayim = new SayimKisiForm();
            sayim.kod = sayimKod;
            ObjectArray sayBilgiler = servisTMM.SayimKisiFormListele(kullanan, sayim);
            foreach (SayimKisiForm s in sayBilgiler.objeler)
            {
                if (s.bitisTarih.isNull)
                    btnYeniKisi.Hidden = false;
                else
                    btnYeniKisi.Hidden = true;

                lblSayimAdi.Html = "<b>" + s.adi + "</b> isimli sayımda işlem göre kişilerin listesi";
            }

            Sayimkisi form = new Sayimkisi();
            form.sayimKod = sayimKod;
            ObjectArray bilgiler = servisTMM.SayimKisiKisiListele(kullanan, form);

            List<object> liste = new List<object>();
            string durum = "";
            if (bilgiler.sonuc.islemSonuc)
            {
                foreach (Sayimkisi item in bilgiler.objeler)
                {
                    if (item.fazla > 0) durum = "FAZLA";
                    else if (item.zimmetli > item.sayilan && item.sayilan > 0) durum = "EKSİK";
                    else if (item.zimmetli > item.sayilan || (item.zimmetli == 0 || item.sayilan == 0)) durum = "SAYILMADI";
                    else if (item.zimmetli == item.sayilan) durum = "TAMAM";

                    liste.Add(new
                    {
                        KOD = item.kod,
                        SAYIMKOD = item.sayimKod,
                        KISIAD = item.kisiAd,
                        KISIKOD = item.kisiKod,
                        ODAAD = item.odaAd,
                        ODAKOD = item.odaKod,
                        ISLEMYAPANAD = item.islemYapanAd,
                        ISLEMYAPANKOD = item.islemYapanKod,
                        ISLEMTARIHI = item.islemTarihi.Oku(),
                        DURUM = durum,
                    });
                }
            }
            storeSayimKisi.DataSource = liste;
            storeSayimKisi.DataBind();
        }

        /// <summary>
        /// Kisis the doldur.
        /// </summary>
        private void KisiDoldur()
        {
            ObjectArray kisiler = servisTMM.PersonelListele(kullanan, new Personel());
            if (kisiler.sonuc.islemSonuc)
            {
                List<object> liste = new List<object>();
                foreach (Personel item in kisiler.objeler)
                {
                    liste.Add(new
                    {
                        KOD = item.kod,
                        AD = item.ad
                    });
                }
                storeKisi.DataSource = liste;
                storeKisi.DataBind();
            }
        }

        /// <summary>
        /// Odas the doldur.
        /// </summary>
        private void OdaDoldur()
        {
            ObjectArray odalar = servisTMM.OdaListele(kullanan, new Oda());
            if (odalar.sonuc.islemSonuc)
            {
                List<object> liste = new List<object>();
                foreach (Oda item in odalar.objeler)
                {
                    liste.Add(new
                    {
                        //KOD = item.muhasebeKod + "-" + item.harcamaBirimKod + "-" + item.kod,
                        KOD = item.kod,
                        AD = item.ad
                    });
                }
                storeOda.DataSource = liste;
                storeOda.DataBind();
            }
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

            SayimKisiForm sayim = new SayimKisiForm();
            sayim.muhasebe = muhasebe;
            sayim.harcamaBirimi = harcamaBirimi;
            Sonuc sonuc = servisTMM.SayimKisiBirimeYetkilimi(kullanan, sayim);
            if (!sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                return;
            }

            GenelIslemler.KullaniciDegiskenSakla(kullanan, "KISISAYIMMUHASEBE", muhasebe);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "KISISAYIMHARCAMA", harcamaBirimi);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "KISISAYIMHARCAMAADI", harcamaBirimiAdi);

            lblSayimBirimAdi.Text = harcamaBirimiAdi;

            SayimListele();
            wndBirim.Hide();
        }

        /// <summary>
        /// Handles the Click event of the btnKisiSayim control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DirectEventArgs"/> instance containing the event data.</param>
        public void btnYeniKisiSayimBaslat_Click(object sender, DirectEventArgs e)
        {
            string kisiAd = TasinirGenel.ComboAdDondur(ddlKisi);
            string kisiKod = TasinirGenel.ComboDegerDondur(ddlKisi);
            string odaAd = TasinirGenel.ComboAdDondur(ddlOda);
            string odaKod = TasinirGenel.ComboDegerDondur(ddlOda);

            Sayimkisi kisi = new Sayimkisi();
            kisi.kisiAd = kisiAd;
            kisi.kisiKod = kisiKod;
            kisi.odaAd = odaAd;
            kisi.odaKod = odaKod;
            kisi.sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value);
            kisi.islemYapanAd = kullanan.ADSOYAD;
            kisi.islemYapanKod = kullanan.KullaniciKodu;
            kisi.islemTarihi = new TNSDateTime(DateTime.Now);

            Sonuc sonuc = servisTMM.SayimKisiYeniKisiSay(kullanan, kisi);
            if (!sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                return;
            }
            else
            {
                wndYeniKisiSay.Hide();
                SayimKisiListele(OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value));

                string muhasebe = OrtakFonksiyonlar.ConvertToStr(txtMuhasebe.Text);
                string harcamaBirimi = OrtakFonksiyonlar.ConvertToStr(txtHarcamaBirimi.Text).Replace(".", "");

                if (kisiKod == "") { kisiKod = odaKod; kisiAd = odaAd; }

                string sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnAktifSayimKod.Value);

                string sayimDetayKod = sonuc.anahtar;

                X.AddScript("SayimKisiDetayPencereAc('" + sayimDetayKod + "','" + sayimKod + "', '" + kisiKod + "', '" + kisiAd + "', '" + muhasebe + "', '" + harcamaBirimi + "');");
            }
        }

        protected void btnRapor_Click(object sender, DirectEventArgs e)
        {
            Sayimkisi kriter = new Sayimkisi();
            kriter.sayimKod = hdnSayimKod.Value.ToString();
            ObjectArray liste = servisTMM.SayimKisiRapor(kullanan, kriter);

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int satir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            XLS.DosyaAc(Server.MapPath("~") + "/RaporSablon/TMM/" + "SayimKisiRaporu.xlt", sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("muhasebe", txtMuhasebe.Text);
            XLS.HucreAdBulYaz("birim", txtHarcamaBirimi.Text);

            satir = kaynakSatir;
            bool baslikTamam = true;
            bool baslikFazla = true;
            bool baslikEksik = true;

            foreach (Sayimkisi sh in liste.objeler)
            {
                if (sh.odaKod != "")
                    sh.kisiAd = sh.kisiAd + "-" + sh.odaAd;

                if (sh.okundu > 0 && sh.fazla == 0)
                {
                    if (baslikTamam)
                    {
                        XLS.HucreAdBulYaz("sayimAd", sh.sayimAd);
                        if (!string.IsNullOrEmpty(sh.bitisTarihi.ToString()))
                            XLS.HucreAdBulYaz("tarihler", sh.baslamaTarihi + "-" + sh.bitisTarihi);
                        XLS.HucreAdBulYaz("tarihler", sh.baslamaTarihi.ToString());

                        satir++;
                        AraBaslikKoy(XLS, satir, "Sayılan Malzemeler");
                        baslikTamam = false;
                    }
                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, sh.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, sh.malzemeAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, sh.kisiAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, sh.fazlaKisiAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, sh.islemTarihi.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, sh.islemYapanAd);
                }
                else if (sh.okundu > 0 && sh.fazla > 0)
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
                    XLS.HucreDegerYaz(satir, sutun, sh.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, sh.malzemeAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, sh.kisiAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, sh.fazlaKisiAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, sh.islemTarihi.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, sh.islemYapanAd);
                }
                else if (sh.okundu == 0)
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
                    XLS.HucreDegerYaz(satir, sutun, sh.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, sh.malzemeAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, sh.kisiAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, sh.fazlaKisiAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, sh.islemTarihi.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, sh.islemYapanAd);
                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "SayimKisiRaporu", true, GenelIslemler.ExcelTur());
        }

        void AraBaslikKoy(Tablo XLS, int satir, string baslik)
        {
            XLS.HucreDegerYaz(satir, 0, baslik);
            XLS.ArkaPlanRenk(satir, 0, satir, 5, System.Drawing.Color.LightYellow);
            XLS.KoyuYap(satir, 0, satir, 5, true);
        }

        protected void btnKisiRapor_Click(object sender, DirectEventArgs e)
        {
            Sayimkisi kriter = new Sayimkisi();
            kriter.sayimKod = hdnSayimKod.Value.ToString();
            kriter.muhasebeKod = txtMuhasebe.Text;
            kriter.harcamabirimKod = txtHarcamaBirimi.Text.ToString().Replace(".", "");
            ObjectArray liste = servisTMM.SayimKisiRaporRapor(kullanan, kriter);

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int satir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            XLS.DosyaAc(Server.MapPath("~") + "/RaporSablon/TMM/" + "SayimSayilmayanKisiRaporu.xlt", sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("muhasebe", txtMuhasebe.Text);
            XLS.HucreAdBulYaz("birim", txtHarcamaBirimi.Text);

            satir = kaynakSatir;

            foreach (Sayimkisi item in liste.objeler)
            {
                XLS.HucreAdBulYaz("sayimAd", txtSayimAdi.Text);
                if (!string.IsNullOrEmpty(item.bitisTarihi.ToString()))
                    XLS.HucreAdBulYaz("tarihler", item.baslamaTarihi + " - " + item.bitisTarihi);
                XLS.HucreAdBulYaz("tarihler", item.baslamaTarihi.ToString());
                satir++;
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun, item.kisiKod);
                XLS.HucreDegerYaz(satir, sutun, item.kisiAd);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "SayimSayilmayanKisiRaporu", true, GenelIslemler.ExcelTur());
        }

    }
}
