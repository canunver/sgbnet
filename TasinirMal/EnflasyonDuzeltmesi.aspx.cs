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
using System.IO;
using static TNS.FTPIslem;

namespace TasinirMal
{
    /// <summary>
    /// Web formu ile ilgili olaylarý (event) ve fonksiyonlarý tutan sýnýf
    /// </summary>
    public partial class EnflasyonDuzeltmesiSayfa : TMMSayfaV2
    {
        ITMMServis servisTMM = null;

        /// <summary>
        /// Sayfa hazýrlanýrken, çaðrýlan olay (event) fonksiyon.
        /// </summary>
        /// <param name="sender">Olayý uyandýran nesne</param>
        /// <param name="e">Olayýn parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            servisTMM = TNS.TMM.Arac.Tanimla();
            if (!X.IsAjaxRequest)
            {
                formAdi = "Enflasyon Düzeltmesi Sayfasý";
                yardimBag = yardimYol + "#YardimDosyasiAd";

                OrtakClass.GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                string harcamaBirimi = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                //pgFiltre.UpdateProperty("prpHarcamaBirimi", harcamaBirimi);
                //pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                //DonemListele("", harcamaBirimi);

                wndMesaj.Hide();
            }
        }

        [DirectMethod]
        public void DonemListele(string harcamaBirimiEski, string harcamaBirimi)
        {
            int varsayilanYil = DateTime.Now.Year;
            int varsayilanDonem = 12;

            List<Object> donemListe = new List<object>();

            donemListe.Add(new { KOD = 12, ADI = 12 });

            strDonem.DataSource = donemListe;
            strDonem.DataBind();

            if (string.IsNullOrWhiteSpace(harcamaBirimiEski))
            {
                pgFiltre.UpdateProperty("prpYil", varsayilanYil);
                pgFiltre.UpdateProperty("prpDonem", varsayilanDonem);
            }
        }


        private AmortismanKriter KriterOku()
        {
            AmortismanKriter form = new AmortismanKriter
            {
                //yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0),
                //donem = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDonem"].Value.Trim(), 0),
                muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim(),
                //harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim(),
                //ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim(),
            };

            return form;
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþacak olay fonksiyon
        /// </summary>
        /// <param name="sender">Olayý uyandýran nesne</param>
        /// <param name="e">Olayýn parametresi</param>
        protected void btnOku_Click(object sender, DirectEventArgs e)
        {
            AmortismanKriter form = KriterOku();

            TasinirGenel.DegiskenleriKaydet(kullanan, form.muhasebeKod, form.harcamaKod, "");


            form.raporTur = (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI;
            form.hesapPlanKod = "25%";

            ObjectArray formlar = servisTMM.AmortismanRaporuMB(kullanan, form);

            if (formlar.sonuc.islemSonuc)
            {
                stoAmortisman.DataSource = formlar.objeler;
                stoAmortisman.DataBind();
            }
            else
                GenelIslemler.MesajKutusu("Bilgi", formlar.sonuc.hataStr != "" ? formlar.sonuc.hataStr : Resources.TasinirMal.Amortisman002);
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþacak olay fonksiyon
        /// </summary>
        /// <param name="sender">Olayý uyandýran nesne</param>
        /// <param name="e">Olayýn parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            AmortismanKriter form = KriterOku();

            if (string.IsNullOrWhiteSpace(form.harcamaKod))
                ResourceManager1.AddScript("wndDurum.show(); Ext1.net.DirectMethods.EnflasyonDuzelmeKayitlariUret();");
            else
            {
                Sonuc sonuc = servisTMM.EnflasyonDuzeltmesiAmortismanKaydet(kullanan, form);

                if (!sonuc.islemSonuc)
                    GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
                else
                    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.Amortisman004);
            }
        }

        /// <summary>
        /// KaydetTif tuþuna basýlýnca çalýþacak olay fonksiyon
        /// </summary>
        /// <param name="sender">Olayý uyandýran nesne</param>
        /// <param name="e">Olayýn parametresi</param>
        protected void btnKaydetTif_Click(object sender, DirectEventArgs e)
        {
            AmortismanKriter form = KriterOku();

            //if (string.IsNullOrWhiteSpace(form.harcamaKod))
            //    ResourceManager1.AddScript("wndDurum.show(); Ext1.net.DirectMethods.EnflasyonDuzelmeTifUret();");
            //else
            {
                Sonuc sonuc = servisTMM.EnflasyonDuzeltmesiTifKaydet(kullanan, form);

                if (!sonuc.islemSonuc)
                    GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
                else
                    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.Amortisman004);
            }
        }

        /// <summary>
        /// Yazdir tuþuna basýlýnca çalýþacak olay fonksiyon
        /// </summary>
        /// <param name="sender">Olayý uyandýran nesne</param>
        /// <param name="e">Olayýn parametresi</param>
        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            try
            {
                AmortismanKriter form = KriterOku();
                form.raporTur = (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI;
                form.hesapPlanKod = "25%";


                ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, form);

                if (!bilgi.sonuc.islemSonuc)
                {
                    GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                    return;
                }

                if (bilgi.objeler.Count <= 0)
                {
                    GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayýt bulunamadý.");
                    return;
                }

                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                int kaynakSatir = 0;

                string sonucDosyaAd = System.IO.Path.GetTempFileName();
                string sablonAd = "EnflasyonDuzeltmesiRaporu.xlt";
                XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

                satir = kaynakSatir + 3;

                foreach (TNS.TMM.AmortismanRapor ar in bilgi.objeler)
                {
                    satir++;

                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, ar.harcamaBirimiKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, ar.harcamaBirimiAdi);
                    XLS.HucreDegerYaz(satir, sutun + 2, ar.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 3, ar.gorSicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 4, ar.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, ar.girisTarih.Yil);
                    XLS.HucreDegerYaz(satir, sutun + 6, ar.edTutar);
                    XLS.HucreDegerYaz(satir, sutun + 7, ar.edTutarEnflasyonDuzeltmesi);
                    XLS.HucreDegerYaz(satir, sutun + 8, ar.edTutarToplam);

                    XLS.HucreDegerYaz(satir, sutun + 9, ar.edAmortisman);
                    XLS.HucreDegerYaz(satir, sutun + 10, ar.edAmortismanEnflasyonDuzeltmesi);
                }


                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());

            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu(this, ex.Message);
            }
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            //pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());


            //pgFiltre.UpdateProperty("prpHarcamaBirimi", "");
        }

        static int islemGorenBirim = 0;
        static int islemGorecekBirim = -1;

        static int islemGorenAmbar = 0;
        static int islemGorecekAmbar = -1;

        static float islemYuzde = 0;
        static bool durdur = false;
        static string birimAd = "";
        static string ambarAd = "";
        static string hata = "";

        protected void RefreshProgress(object sender, DirectEventArgs args)
        {
            if (args.ExtraParams["durdur"] == "true")
            {
                Durdur();
                hata = "Enflasyon Düzeltme Ýþlemi durduruldu!" + (hata != "" ? "<br>Hata : " + hata : "");
                btnDurdur.Disabled = true;
            }

            if (islemGorenBirim <= 1 && islemGorenAmbar == 0)
                Progress1.UpdateProgress(0, "Ýþlem yapýlacak ambarlar belirleniyor.");
            else if (islemGorenAmbar <= islemGorecekAmbar)
            {
                if (islemGorecekBirim > 0 || islemGorecekAmbar > 0)
                {
                    float birimYuzde1 = (((float)islemGorenBirim - 1) / (float)islemGorecekBirim) * 100;
                    float birimYuzde = ((float)islemGorenBirim / (float)islemGorecekBirim) * 100;
                    islemYuzde = (birimYuzde - birimYuzde1) * ((float)islemGorenAmbar / (float)islemGorecekAmbar) + birimYuzde1;

                    lblIslemYapilanBirim.Text = birimAd;
                    lblIslemYapilanAmbar.Text = ambarAd;
                    lblBilgi.Html = string.Format("Birim ({0}/{1}) - Ambar ({2}/{3})", islemGorenBirim, islemGorecekBirim, islemGorenAmbar, islemGorecekAmbar);

                    Progress1.UpdateProgress(islemYuzde / 100, "% " + islemYuzde.ToString("###"));
                }
            }

            if (durdur)
            {
                if (hata == "") hata = "Enflasyon Düzeltme kayýtlarý oluþturuldu.";
                ResourceManager1.AddScript("Bitir('{0}');", GenelIslemler.HataBilgisiniHMTLYap(hata).Replace(";", "<br />"));
            }
        }



        [DirectMethod]
        public void EnflasyonDuzelmeKayitlariUret()
        {
            islemYuzde = 0;
            islemGorenBirim = 0;
            islemGorecekBirim = -1;
            islemGorenAmbar = 0;
            islemGorecekAmbar = -1;
            durdur = false;
            hata = "";
            lblIslemYapilanBirim.Text = "-";
            lblIslemYapilanAmbar.Text = "-";
            lblBilgi.Html = "-";
            btnDurdur.Disabled = false;


            ResourceManager1.AddScript("{0}.startTask('IslemGostergec');", TaskManager1.ClientID);
            try
            {
                if (!System.Threading.ThreadPool.QueueUserWorkItem(GirisIsle))
                {
                    hata = "Process baþlatýlamadý";
                    Durdur();
                }
            }
            catch (Exception ex)
            {
                hata = ex.Message;
                Durdur();
            }
        }


        [DirectMethod]
        public void EnflasyonDuzelmeTifUret()
        {
            hata = "Process baþlatýlamadý";
            Durdur();
        }


        private void Durdur()
        {
            durdur = true;
        }

        void GirisIsle(object state)
        {
            AmortismanKriter kriter = KriterOku();

            HarcamaBirimi birim = new HarcamaBirimi { muhasebeKod = kriter.muhasebeKod, kod = kriter.harcamaKod, kapali = -100 }; //kapali = -100 - Yetkisi olan birimleri listelemek için
            ObjectArray listeBirim = servisTMM.HarcamaBirimiListele(kullanan, birim);

            islemGorecekBirim = 0;
            islemGorecekBirim = listeBirim.objeler.Count;

            foreach (HarcamaBirimi b in listeBirim.objeler)
            {
                if (durdur)
                    break;

                islemGorenBirim++;
                islemGorenAmbar = 0;

                birimAd = b.ad;

                Ambar ambar = new Ambar { muhasebeKod = kriter.muhasebeKod, harcamaBirimKod = b.kod, kod = kriter.ambarKod };
                ObjectArray listeAmbar = servisTMM.AmbarListele(kullanan, ambar);
                islemGorecekAmbar = listeAmbar.objeler.Count;

                AmortismanKriter form = new AmortismanKriter
                {
                    yil = kriter.yil,
                    donem = kriter.donem
                };

                if (listeAmbar.ObjeSayisi == 0)
                    hata = "Ýþlem yapýlacak ambar bulunamadý!";

                foreach (Ambar a in listeAmbar.objeler)
                {
                    if (durdur)
                        break;

                    islemGorenAmbar++;

                    ambarAd = a.ad;
                    form.muhasebeKod = a.muhasebeKod;
                    form.harcamaKod = a.harcamaBirimKod;
                    form.ambarKod = a.kod;
                    Sonuc sonuc = servisTMM.EnflasyonDuzeltmesiAmortismanKaydet(kullanan, form);

                    System.Threading.Thread.Sleep(1000);
                    if (!sonuc.islemSonuc)
                    {
                        //durdur = true;
                        hata += ";" + sonuc.hataStr;
                    }

                }

            }

            Durdur();

            if (listeBirim.objeler.Count == 0 && hata == "")
                hata = "Enflasyon düzelmesi için yetkili olduðunuz birim bulunamadý.";

        }

    }
}
