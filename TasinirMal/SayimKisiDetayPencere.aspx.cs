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
    public partial class SayimKisiDetayPencere : TMMSayfaV2
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
                string sayimDetayKod = Request.QueryString["sayimDetayKod"] + "";
                string sayimKod = Request.QueryString["sayimKod"] + "";
                string kisiKod = Request.QueryString["kisiKod"] + "";
                string kisiAd = Request.QueryString["kisiAd"] + "";
                hdnMuhasebeKod.Value = Request.QueryString["muhasebeKod"] + "";
                hdnHarcamaBirimKod.Value = Request.QueryString["harcamaBirimKod"] + "";
                if (string.IsNullOrEmpty(sayimKod) || string.IsNullOrEmpty(kisiKod))
                {
                    GenelIslemler.SayfayaGirmesin(true);
                }
                KisiDetayEkraniHazirla(sayimDetayKod, sayimKod, kisiKod);
            }
        }

        /// <summary>
        /// Kisis the detay ekrani hazirla.
        /// </summary>
        /// <param name="sayimKod">The sayim kod.</param>
        /// <param name="kisiKod">The kisi kod.</param>
        /// <returns></returns>
        [DirectMethod]
        public void KisiDetayEkraniHazirla(string sayimDetayKod, string sayimKod, string kisiKod)
        {
            hdnSayimDetaySayimDetayKod.Value = sayimDetayKod;
            hdnSayimDetaySayimKod.Value = sayimKod;
            hdnSayimDetayKisiKod.Value = kisiKod;

            SayimKisiDetay form = new SayimKisiDetay();
            form.kisiKod = kisiKod;
            form.sayimKod = sayimKod;
            form.sayimDetayKod = sayimDetayKod;

            ObjectArray bilgiler = servisTMM.SayimKisiKisiDetayListele(kullanan, form);

            List<object> listeKisi = new List<object>();
            List<object> listeFazla = new List<object>();
            int sayacFazla = 0;
            int sayacZimmet = 0;
            int sayacOkunan = 0;
            if (bilgiler.sonuc.islemSonuc)
            {
                foreach (SayimKisiDetay item in bilgiler.objeler)
                {
                    object o = new
                    {
                        KOD = item.kod,
                        SAYIMKOD = item.sayimKod,
                        KISIKOD = item.kisiKod,
                        FAZLAKISIAD = item.fazlaKisiAd,
                        PRSICILNO = item.prSicilNo,
                        SICILNO = item.sicilNo,
                        MALZEMEAD = item.malzemeAd,
                        OKUNDU = item.okundu,
                    };

                    if (item.fazla == 0)
                    {
                        listeKisi.Add(o);
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

            storeZimmet.DataSource = listeKisi;
            storeZimmet.DataBind();

            storeZimmetFazla.DataSource = listeFazla;
            storeZimmetFazla.DataBind();
        }

        /// <summary>
        /// Sicils the no okutuldu.
        /// </summary>
        /// <param name="sicilNo">The sicil no.</param>
        /// <param name="okundu">The okundu.</param>
        [DirectMethod]
        public void SicilNoOkutuldu(string sicilNo, int okundu)
        {
            string sayimDetayKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimDetayKod.Value);
            string sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimKod.Value);
            string kisiKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetayKisiKod.Value);
            if (string.IsNullOrEmpty(sicilNo))
            {
                return;
            }

            if (okundu == 0) okundu = 1;
            else if (okundu == 1) okundu = 0;

            TNS.TMM.SayimKisiDetay kriter = new SayimKisiDetay();
            kriter.sicilNo = sicilNo;//Sicilno boş ise bütün dataların okundusu 1 olarak atar
            kriter.kisiKod = kisiKod;
            kriter.sayimKod = sayimKod;
            ObjectArray bilgiler = servisTMM.SayimKisiKisiDetayListele(kullanan, kriter);
            if (bilgiler.objeler.Count > 0)
            {
                foreach (SayimKisiDetay zk in bilgiler.objeler)
                {
                    if (zk.kisiKod == kisiKod)
                    {
                        SayimKisiDetay skd = new SayimKisiDetay();
                        skd.kod = zk.kod;
                        skd.sayimKod = sayimKod;
                        skd.kisiKod = kisiKod;
                        skd.sicilNo = zk.sicilNo;
                        skd.prSicilNo = zk.prSicilNo;
                        skd.okundu = okundu;
                        skd.islemYapanAd = kullanan.ADSOYAD;
                        skd.islemYapanKod = kullanan.KullaniciKodu;
                        skd.islemTarihi = new TNSDateTime(DateTime.Now);

                        Sonuc sonuc = servisTMM.SayimKisiDetayOkundu(kullanan, skd);
                    }
                }
            }
            else
            {
                SicilNoHareket form = new SicilNoHareket();
                form.sicilNo = sicilNo;
                form.muhasebeKod = OrtakFonksiyonlar.ConvertToStr(hdnMuhasebeKod.Value);
                form.harcamaBirimKod = hdnHarcamaBirimKod.Value.ToString().Replace(".", "");
                ObjectArray kontrol = servisTMM.BarkodSicilNoListele(kullanan, form, new Sayfalama());
                foreach (SicilNoHareket item in kontrol.objeler)
                {
                    string fazlaAciklama = item.kimeGitti;
                    if (fazlaAciklama == "") fazlaAciklama = item.ambarKod + " nolu AMBAR";
                    if (fazlaAciklama != "") fazlaAciklama += " (" + item.harcamaBirimAd + ")";
                    string fazlaKod = item.kisiAd;
                    if (fazlaKod == "") fazlaKod = item.ambarKod;

                    SayimKisiDetay skd = new SayimKisiDetay();
                    skd.sayimKod = sayimKod;
                    skd.kisiKod = kisiKod;
                    skd.prSicilNo = item.prSicilNo;
                    skd.sicilNo = item.sicilNo;
                    skd.malzemeAd = item.hesapPlanAd;
                    skd.okundu = 1;
                    skd.fazla = 1;
                    skd.fazlaKisiKod = fazlaKod;
                    skd.fazlaKisiAd = fazlaAciklama;
                    skd.islemYapanAd = kullanan.ADSOYAD;
                    skd.islemYapanKod = kullanan.KullaniciKodu;
                    skd.islemTarihi = new TNSDateTime(DateTime.Now);
                    skd.sayimDetayKod = sayimDetayKod;

                    ObjectArray skdListe = new ObjectArray();
                    skdListe.objeler.Add(skd);

                    Sonuc sonuc = servisTMM.SayimKisiDetayKaydet(kullanan, skdListe);
                }
            }

            if (chkYenile.Checked)
                KisiDetayEkraniHazirla(sayimDetayKod, sayimKod, kisiKod);

            txtSicil.Text = "";
        }

        /// <summary>
        /// Fazlas the sil.
        /// </summary>
        /// <param name="kod">The kod.</param>
        [DirectMethod]
        public void FazlaSil(string kod)
        {
            SayimKisiDetay skd = new SayimKisiDetay();
            skd.kod = kod;

            Sonuc sonuc = servisTMM.SayimKisiFazlaSil(kullanan, skd);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                string sayimDetayKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimDetayKod.Value);
                string sayimKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetaySayimKod.Value);
                string kisiKod = OrtakFonksiyonlar.ConvertToStr(hdnSayimDetayKisiKod.Value);
                KisiDetayEkraniHazirla(sayimDetayKod, sayimKod, kisiKod);
                GenelIslemler.ExtNotification("Fazla malzeme başarıyla silindi", "Bilgi", Icon.Lightbulb);

            }
        }

        protected void btnRapor_Click(object sender, DirectEventArgs e)
        {
            Sayimkisi kriter = new Sayimkisi();
            kriter.kod = hdnSayimDetaySayimDetayKod.Text;
            kriter.sayimKod = hdnSayimDetaySayimKod.Text;
            kriter.kisiKod = hdnSayimDetayKisiKod.Text;
            
            ObjectArray liste = servisTMM.SayimKisiRapor(kullanan, kriter);

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int satir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            XLS.DosyaAc(Server.MapPath("~") + "/RaporSablon/TMM/" + "SayimKisiRaporu.xlt", sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("muhasebe", hdnMuhasebeKod.Text);
            XLS.HucreAdBulYaz("birim", hdnHarcamaBirimKod.Text);

            satir = kaynakSatir;
            bool baslikTamam = true;
            bool baslikFazla = true;
            bool baslikEksik = true;

            Dictionary<string, List<Sayimkisi>> listeKisiOda = new Dictionary<string, List<Sayimkisi>>();

            foreach (Sayimkisi sh in liste.objeler)
            {
                if (sh.odaKod != "")
                    sh.kisiAd = sh.kisiAd + "-" + sh.odaAd;

                if (!listeKisiOda.ContainsKey(sh.kisiAd))
                    listeKisiOda.Add(sh.kisiAd, new List<Sayimkisi>());

                listeKisiOda[sh.kisiAd].Add(sh);
            }

            foreach (var item in listeKisiOda)
            {
                baslikTamam = true;
                baslikFazla = true;
                baslikEksik = true;

                if (satir != kaynakSatir)
                    XLS.SayfaSonuKoy(satir + 1);

                foreach (Sayimkisi sh in item.Value)
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

            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "SayimKisiRaporu", true, GenelIslemler.ExcelTur());
        }

        void AraBaslikKoy(Tablo XLS, int satir, string baslik)
        {
            XLS.HucreDegerYaz(satir, 0, baslik);
            XLS.HucreDegerYaz(satir, 1, baslik);
            XLS.HucreDegerYaz(satir, 2, baslik);
            XLS.HucreDegerYaz(satir, 3, baslik);
            XLS.ArkaPlanRenk(satir, 0, satir, 5, System.Drawing.Color.LightYellow);
            XLS.KoyuYap(satir, 0, satir, 5, true);
        }

    }
}
