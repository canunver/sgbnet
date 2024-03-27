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

namespace TasinirMal
{
    /// <summary>
    /// Web formu ile ilgili olayları (event) ve fonksiyonları tutan sınıf
    /// </summary>
    public partial class AmortismanForm : TMMSayfaV2
    {
        ITMMServis servisTMM = null;

        /// <summary>
        /// Sayfa hazırlanırken, çağrılan olay (event) fonksiyon.
        /// </summary>
        /// <param name="sender">Olayı uyandıran nesne</param>
        /// <param name="e">Olayın parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            servisTMM = TNS.TMM.Arac.Tanimla();
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.Amortisman001;
                yardimBag = yardimYol + "#YardimDosyasiAd";
                //kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
                OrtakClass.GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                string harcamaBirimi = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", harcamaBirimi);
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                DonemListele("", harcamaBirimi);

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    btnBelgeOnayaGonder.Hidden = false;
                    btnBelgeOnayKaldir.Hidden = false;
                    //mnuMuhasebat.Hidden = true;
                    mnuMaliyetMuhasebesi.Hidden = false;
                    btnBAOnay.Hidden = false;

                    for (int i = 2; i < 22; i++)
                        grdAmortisman.ColumnModel.Columns[i].Hidden = false;

                    for (int i = 22; i < 32; i++)
                        grdAmortisman.ColumnModel.Columns[i].Hidden = true;
                }

                wndMesaj.Hide();
            }
        }

        [DirectMethod]
        public void DonemListele(string harcamaBirimiEski, string harcamaBirimi)
        {
            int varsayilanYil = DateTime.Now.Year;
            int varsayilanDonem = 12;

            List<Object> donemListe = new List<object>();

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                if (harcamaBirimi.Replace(".", "") == "010") //Banknot birimi ise aylık
                {
                    for (int i = 1; i <= 12; i++)
                        donemListe.Add(new { KOD = i, ADI = i });

                    varsayilanDonem = DateTime.Now.AddMonths(-1).Month;
                    varsayilanYil = DateTime.Now.AddMonths(-1).Year;
                }
                else //Diğer birimler 3'er aylık
                {
                    for (int i = 3; i <= 12; i += 3)
                        donemListe.Add(new { KOD = i, ADI = i });

                    if (DateTime.Now.Month <= 3)
                    {
                        varsayilanYil -= 1;
                        varsayilanDonem = 12;
                    }
                    else if (DateTime.Now.Month <= 6)
                        varsayilanDonem = 3;
                    else if (DateTime.Now.Month <= 9)
                        varsayilanDonem = 6;
                    else if (DateTime.Now.Month <= 12)
                        varsayilanDonem = 9;
                }

            }
            else
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
                yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(), 0),
                donem = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDonem"].Value.Trim(), 0),
                muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim(),
                harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim(),
                ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim(),
                hesapPlanKod = pgFiltre.Source["prpHesapKod"].Value.Trim().Replace(".", "")
            };

            return form;
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışacak olay fonksiyon
        /// </summary>
        /// <param name="sender">Olayı uyandıran nesne</param>
        /// <param name="e">Olayın parametresi</param>
        protected void btnOku_Click(object sender, DirectEventArgs e)
        {
            AmortismanKriter form = KriterOku();

            form.raporTur = 4;

            ObjectArray formlar = new ObjectArray();
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                formlar = servisTMM.AmortismanRaporla3(kullanan, form);
            else
                formlar = servisTMM.AmortismanRaporla2(kullanan, form);

            if (formlar.sonuc.islemSonuc)
            {
                stoAmortisman.DataSource = formlar.objeler;
                stoAmortisman.DataBind();
            }
            else
                GenelIslemler.MesajKutusu("Bilgi", formlar.sonuc.hataStr != "" ? formlar.sonuc.hataStr : Resources.TasinirMal.Amortisman002);
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışacak olay fonksiyon
        /// </summary>
        /// <param name="sender">Olayı uyandıran nesne</param>
        /// <param name="e">Olayın parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            AmortismanKriter form = KriterOku();

            if (string.IsNullOrWhiteSpace(form.harcamaKod) || string.IsNullOrWhiteSpace(form.ambarKod))
                ResourceManager1.AddScript("wndDurum.show(); Ext1.net.DirectMethods.AmortismanUret();");
            else
            {
                form.raporTur = 3; //Amortisman yapılacaklar
                Sonuc sonuc = null;
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    sonuc = servisTMM.AmortismanVerisiOlustur3(kullanan, form);
                else
                    sonuc = servisTMM.AmortismanVerisiOlustur2(kullanan, form);

                if (!sonuc.islemSonuc)
                    GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
                else
                    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.Amortisman004);
            }
        }

        /// <summary>
        /// Yazdir tuşuna basılınca çalışacak olay fonksiyon
        /// </summary>
        /// <param name="sender">Olayı uyandıran nesne</param>
        /// <param name="e">Olayın parametresi</param>
        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            try
            {
                AmortismanKriter form = KriterOku();

                System.Collections.Hashtable raporParam = new System.Collections.Hashtable();
                raporParam["yil"] = form.yil + "";
                raporParam["donem"] = form.donem + "";
                raporParam["muhasebeKod"] = form.muhasebeKod;
                raporParam["harcamaKod"] = form.harcamaKod;
                raporParam["ambarKod"] = form.ambarKod;
                raporParam["hesapPlanKod"] = form.hesapPlanKod;
                raporParam["raporTur"] = OrtakFonksiyonlar.ConvertToInt(e.ExtraParams["tur"], 2);
                raporParam["raporSekli"] = OrtakFonksiyonlar.ConvertToInt(e.ExtraParams["raporSekli"], 2);
                raporParam["ihracDahil"] = OrtakFonksiyonlar.ConvertToInt(e.ExtraParams["ihracDahil"], 0);

                if (OrtakFonksiyonlar.ConvertToInt(e.ExtraParams["tur"], 2) == 23)
                {
                    string tmp = Path.GetTempFileName();
                    string excel = tmp.Replace(".tmp", ".xlsx");
                    File.Move(tmp, excel);

                    AmortismanKriter ak = new TNS.TMM.AmortismanKriter();
                    ak.yil = form.yil;
                    ak.donem = form.donem;
                    ak.muhasebeKod = form.muhasebeKod;
                    ak.harcamaKod = form.harcamaKod;
                    ak.ambarKod = form.ambarKod;
                    //ak.raporTur = 100;
                    ak.raporTur = 6;

                    Raporlar.BAOnayiAmortismanRaporu(kullanan, servisTMM, ak, "", "1", false);
                    //excel = System.IO.Path.ChangeExtension(excel, ".pdf");
                    //System.IO.File.Copy(excelE, excel);
                }
                else
                {
                    TNS.Raporlama.RaporlamaServis.TekRaporAl(9000, 2, kullanan, servisTMM, raporParam);
                }

            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu(this, ex.Message);
            }
        }

        [DirectMethod]
        public void OnayaGonder(string aciklama)
        {
            string hata = "";

            AmortismanKriter kriter = KriterOku();

            if (kriter.muhasebeKod.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (kriter.harcamaKod.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + " Onaya gönderme işlemi gerçekleşmedi.");
                return;
            }

            AmortismanIslemForm from = new AmortismanIslemForm()
            {
                yil = kriter.yil,
                donem = kriter.donem,
                muhasebeKod = kriter.muhasebeKod,
                harcamaKod = kriter.harcamaKod,
                ambarKod = kriter.ambarKod,
                fisAciklama = aciklama
            };

            Sonuc sonuc = servisTMM.AmortismanIslemFisiOnayDurumDegistir(kullanan, from, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        protected void btnOnayKaldir_Click(object sender, DirectEventArgs e)
        {
            string hata = "";

            AmortismanKriter kriter = KriterOku();

            if (kriter.muhasebeKod.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (kriter.harcamaKod.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata);
                return;
            }

            AmortismanIslemForm from = new AmortismanIslemForm()
            {
                yil = kriter.yil,
                donem = kriter.donem,
                muhasebeKod = kriter.muhasebeKod,
                harcamaKod = kriter.harcamaKod,
                ambarKod = kriter.ambarKod
            };

            Sonuc sonuc = servisTMM.AmortismanIslemFisiDurumDegistir(kullanan, from, ENUMAmortismanFormDurumu.IPTAL);

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
            pgFiltre.UpdateProperty("prpHesapKod", string.Empty);
        }

        [DirectMethod]
        public void DegerDegistir(string kod, int yil, int donem, int prsicilNo, int tur, double deger, string saymanlikKod, string birimKod, string ambarKod)
        {
            string degerArtisKod = "";

            Sonuc sonuc = servisTMM.AmortismanRakamKaydet(kullanan, kod, yil, donem, prsicilNo, ENUMAmortismanTur.NORMAL, Convert.ToDecimal(deger), saymanlikKod, birimKod, ambarKod);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
            else
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.Amortisman004);
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
                hata = "Amortisman İşlemi durduruldu!" + (hata != "" ? "<br>Hata : " + hata : "");
                btnDurdur.Disabled = true;
            }

            if (islemGorenBirim <= 1 && islemGorenAmbar == 0)
                Progress1.UpdateProgress(0, "İşlem yapılacak ambarlar belirleniyor.");
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
                if (hata == "") hata = "Amortisman kayıtları oluşturuldu.";
                ResourceManager1.AddScript("Bitir('{0}');", GenelIslemler.HataBilgisiniHMTLYap(hata).Replace(";", "<br />"));
            }
        }



        [DirectMethod]
        public void AmortismanUret()
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
                    hata = "Process başlatılamadı";
                    Durdur();
                }
            }
            catch (Exception ex)
            {
                hata = ex.Message;
                Durdur();
            }
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
                    hata = "İşlem yapılacak ambar bulunamadı!";

                foreach (Ambar a in listeAmbar.objeler)
                {
                    if (durdur)
                        break;

                    islemGorenAmbar++;

                    ambarAd = a.ad;
                    form.muhasebeKod = a.muhasebeKod;
                    form.harcamaKod = a.harcamaBirimKod;
                    form.ambarKod = a.kod;
                    form.raporTur = 3; //Amortisman yapılacaklar
                    form.hesapPlanKod = "";
                    //Sonuc sonuc = servisTMM.AmortismanVerisiOlustur2(kullanan, form);
                    Sonuc sonuc = null;
                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        sonuc = servisTMM.AmortismanVerisiOlustur3(kullanan, form);
                    else
                        sonuc = servisTMM.AmortismanVerisiOlustur2(kullanan, form);

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
                hata = "Amortisman için yetkili olduğunuz birim bulunamadı.";

        }

    }
}
