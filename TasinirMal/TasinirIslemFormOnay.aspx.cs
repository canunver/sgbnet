using Ext1.Net;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþi bilgilerinin Onay iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIslemFormOnay : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý kütüphane malzemeleri için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        static bool kutuphaneGoster = false;

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý müze malzemeleri için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        static bool muzeGoster = false;

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý daðýtým Ýade için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        static bool dagitimIade = false;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!X.IsAjaxRequest)
            {
                formAdi = "B-A Onay Sayfasý";
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");


                //Sayfaya giriþ izni varmý?
                bool izin = false;
                if (dagitimIade)
                    izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan);
                else
                    izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan);

                if (!izin)
                    GenelIslemler.SayfayaGirmesin(true);

                if (Request.QueryString["menuYok"] == "1")
                {
                    pnlAna.Margins = "0 0 0 0";
                    pnlAna.StyleSpec += "padding:5px";
                }

                kutuphaneGoster = (Request.QueryString["kutuphane"] + "" != "") ? true : false;
                muzeGoster = (Request.QueryString["muze"] + "" != "") ? true : false;
                dagitimIade = (Request.QueryString["dagitimIade"] + "" != "") ? true : false;

                OnayDurumDoldur();
                IslemTipDoldur();
                btnListele_Click(null, null);
            }

        }

        private void OnayDurumDoldur()
        {
            List<object> storeListe = new List<object>();

            storeListe.Add(new { KOD = (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB, ADI = "B Onayýna Gönderildi" });
            storeListe.Add(new { KOD = (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA, ADI = "A Onayýna Gönderildi" });
            storeListe.Add(new { KOD = (int)ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI, ADI = "Tamamlandý" });

            strOnayDurum.DataSource = storeListe;
            strOnayDurum.DataBind();
        }

        private void IslemTipDoldur()
        {
            List<object> storeListe = new List<object>();

            List<IslemTip> liste = TasinirGenel.IslemTipListele(servisTMM, kullanan, dagitimIade);
            foreach (var it in liste)
                storeListe.Add(new { KOD = it.kod, ADI = it.ad, TUR = it.tur });

            storeListe.Add(new { KOD = 0, ADI = "Amortisman", TUR = 0 });
            storeListe.Add(new { KOD = -1, ADI = "Deðer Düzeltme", TUR = -1 });

            strIslemTipi.DataSource = storeListe;
            strIslemTipi.DataBind();
        }

        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("fisNo")   { DataType = typeof(string) },
                new DataColumn("yil")     { DataType = typeof(int) },
                new DataColumn("donem")     { DataType = typeof(int) },
                new DataColumn("muhasebe")    { DataType = typeof(string) },
                new DataColumn("harcamaBirimi") { DataType = typeof(string) },
                new DataColumn("harcamaBirimiAd") { DataType = typeof(string) },
                new DataColumn("ambar") { DataType = typeof(string) },
                new DataColumn("fisTarihi") { DataType = typeof(string) },
                new DataColumn("islemTipi") { DataType = typeof(int) },
                new DataColumn("durum") { DataType = typeof(string) },
                new DataColumn("islemTarih") { DataType = typeof(string) },
                new DataColumn("islemYapan") { DataType = typeof(string) },
                new DataColumn("kod") { DataType = typeof(string) },
                new DataColumn("onayDurum") { DataType = typeof(int) },
                new DataColumn("onayAciklama") { DataType = typeof(string) }
           });

            BAOnay kriter = new BAOnay();
            kriter.onayDurum = (int)ENUMTasinirIslemFormOnayDurumu.ONAY;
            kriter.muhasebeKod = "00001";

            //ObjectArray bilgi = TasinirIslemFisiListele();
            ObjectArray bilgi = servisTMM.BAOnayiYapilacaklarListele(kullanan, kullanan.TCKIMLIKNO, kriter);
            if (bilgi != null)
            {
                if (bilgi.sonuc.islemSonuc)
                {
                    foreach (TNS.TMM.TasinirIslemForm tasForm in bilgi.objeler)
                    {
                        string durum = "";
                        if (tasForm.durum == (int)ENUMBelgeDurumu.YENI)
                            durum = Resources.TasinirMal.FRMTIS006;
                        else if (tasForm.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                            durum = Resources.TasinirMal.FRMTIS007;
                        else if (tasForm.durum == (int)ENUMBelgeDurumu.ONAYLI)
                            durum = Resources.TasinirMal.FRMTIS008;
                        else if (tasForm.durum == (int)ENUMBelgeDurumu.IPTAL)
                            durum = Resources.TasinirMal.FRMTIS009;

                        if (!dagitimIade && (tasForm.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tasForm.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                            continue;
                        if (dagitimIade && (tasForm.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADECIKIS && tasForm.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                            continue;

                        if (tasForm.fisNo.StartsWith("B"))
                            tasForm.islemTipKod = -1;

                        dt.Rows.Add(
                            tasForm.fisNo.Trim(),
                            tasForm.yil, 
                            tasForm.gYil, 
                            tasForm.muhasebeKod, 
                            tasForm.harcamaKod, 
                            tasForm.harcamaAd, 
                            tasForm.ambarKod, 
                            tasForm.fisTarih.ToString(), 
                            tasForm.islemTipKod, 
                            durum, 
                            tasForm.islemTarih.ToString(),
                            tasForm.islemYapan,
                            tasForm.kod, 
                            tasForm.onayDurum, 
                            tasForm.onayAciklama);
                    }
                }
                else
                    GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr + bilgi.sonuc.vtHatasi);
            }

            strListe.DataSource = dt;
            strListe.DataBind();
        }

        protected void btnIslem_Click(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["json"];
            string islem = e.ExtraParams["islem"];

            if (string.IsNullOrEmpty(json) || ((JArray)JSON.Deserialize(json)).Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listeden iþlem yapýlacak belge seçilmemiþ.");
                return;
            }

            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;
            string bilgi = string.Empty;

            foreach (JContainer jc in (JArray)JSON.Deserialize(json))
            {
                int tip = jc.Value<int>("islemTipi");

                if (tip == 0 || tip == -1) //Amortisman // Deðer Artýþý
                {
                    AmortismanIslemForm af = new AmortismanIslemForm();

                    af.kod = jc.Value<string>("kod");
                    af.yil = jc.Value<int>("yil");
                    af.donem = jc.Value<int>("donem");
                    af.muhasebeKod = jc.Value<string>("muhasebe");
                    af.harcamaKod = (jc.Value<string>("harcamaBirimi") + "").Replace(".", "");
                    af.fisNo = (jc.Value<string>("fisNo") + "").PadLeft(6, '0');
                    af.onayDurum = jc.Value<int>("onayDurum");

                    ENUMTasinirIslemFormOnayDurumu yeniDurum = ENUMTasinirIslemFormOnayDurumu.TANIMSIZ;
                    if (islem == "Onay")
                        yeniDurum = (af.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB ? ENUMTasinirIslemFormOnayDurumu.GONDERILDIA : ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI);
                    else if (islem == "GeriGonder")
                        yeniDurum = ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI;

                    sonuc = servisTMM.AmortismanIslemFisiOnayDurumDegistir(kullanan, af, yeniDurum);
                }
                else
                {
                    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                    tf.yil = jc.Value<int>("yil");
                    tf.muhasebeKod = jc.Value<string>("muhasebe");
                    tf.harcamaKod = (jc.Value<string>("harcamaBirimi") + "").Replace(".", "");
                    tf.fisNo = (jc.Value<string>("fisNo") + "").PadLeft(6, '0');
                    tf.kod = jc.Value<string>("kod");
                    tf.onayDurum = jc.Value<int>("onayDurum");

                    int islemTur = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(null, "TASISLEMTIPTUR", tip.ToString(), true, "").ToString(), 0);
                    if (islemTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                        tf.devirGirisiMi = true;

                    ENUMTasinirIslemFormOnayDurumu yeniDurum = ENUMTasinirIslemFormOnayDurumu.TANIMSIZ;
                    if (islem == "Onay")
                        yeniDurum = (tf.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB ? ENUMTasinirIslemFormOnayDurumu.GONDERILDIA : ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI);
                    else if (islem == "GeriGonder")
                        yeniDurum = ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI;

                    sonuc = servisTMM.TasinirIslemFisiOnayDurumDegistir(kullanan, tf, yeniDurum);
                }

                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr;
                else
                    bilgi += sonuc.bilgiStr;
            }

            if (hata != "")
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else if (bilgi != "")
            {
                pnlDokuman.ClearContent();

                lblYil.Text = "";
                lblBelgeNo.Text = "";
                lblMuhasebe.Text = "";
                lblHarcamaBirimi.Text = "";

                GenelIslemler.ExtNotification(bilgi, "Bilgi", Icon.LightningGo);
            }

            btnListele_Click(null, null);
        }

        //private ObjectArray TasinirIslemFisiListele()
        //{
        //    string hata = "";

        //    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
        //    tf.onayDurum = -100; //Onay yetki kontrolü için gerekli

        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRKULLANICIBIRIM))
        //    {
        //        if (tf.muhasebeKod == "" || tf.harcamaKod == "" || tf.ambarKod == "")
        //            hata = Resources.TasinirMal.FRMLDV001;
        //    }

        //    if (hata == "")
        //    {
        //        TasinirFormKriter kriter = new TasinirFormKriter();
        //        kriter.mernis = kullanan.TCKIMLIKNO; //Onay yetki kontrolü için gerekli
        //        kriter.belgeTarihBasla = new TNSDateTime();
        //        kriter.belgeTarihBit = new TNSDateTime();
        //        kriter.durumTarihBasla = new TNSDateTime();
        //        kriter.durumTarihBit = new TNSDateTime();

        //        if (kutuphaneGoster && kriter.hesapKodu != "")
        //        {
        //            if (kriter.hesapKodu.IndexOf(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString()) < 0)
        //                kriter.hesapKodu = "";
        //        }
        //        if (muzeGoster && kriter.hesapKodu != "")
        //        {
        //            if (kriter.hesapKodu.IndexOf(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString()) < 0)
        //                kriter.hesapKodu = "";
        //        }

        //        if (kutuphaneGoster && kriter.hesapKodu == "")
        //            kriter.hesapKodu = OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString();
        //        else if (muzeGoster && kriter.hesapKodu == "")
        //            kriter.hesapKodu = OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString();

        //        return servisTMM.TasinirIslemFisiListele(kullanan, tf, kriter);
        //    }
        //    else
        //        GenelIslemler.MesajKutusu("Hata", hata);

        //    return null;
        //}

        [DirectMethod(Timeout = 9000000)]
        public void DosyaGoruntule(int yil, int donem, int islemTipi, string harcamaKod, string muhasebeKod, string fisNo, int tamEkran, string ambarKod)
        {
            Ext1.Net.Panel pnl = tamEkran == 1 ? pnlDokumanTamEkran : pnlDokuman;

            string tmp = Path.GetTempFileName();
            string excel = tmp.Replace(".tmp", ".xlsx");
            File.Move(tmp, excel);

            if (islemTipi == 0)
            {
                AmortismanKriter ak = new TNS.TMM.AmortismanKriter();
                ak.yil = yil;
                ak.donem = donem;
                ak.muhasebeKod = muhasebeKod;
                ak.harcamaKod = harcamaKod;
                ak.ambarKod = ambarKod;
                //ak.raporTur = 100;
                ak.raporTur = 6;

                //excel = AmortismanRapor.AmortismanRaporla2(ak, excel);
                //string excelE = Raporlar.MalzemeGruplarinaGoreDemirbasHazirla(kullanan, servisTMM, ak, "3", false);
                string excelE = Raporlar.BAOnayiAmortismanRaporu(kullanan, servisTMM, ak, fisNo, "3", false);
                excel = System.IO.Path.ChangeExtension(excel, ".pdf");
                System.IO.File.Copy(excelE, excel);
            }
            else if (islemTipi == -1)
            {
                SicilNoDegerArtis sd = new SicilNoDegerArtis();
                sd.muhasebeKod = muhasebeKod;
                sd.harcamaKod = harcamaKod;
                sd.belgeNo = donem.ToString();

                excel = SicilNoDegerArtisForm.RaporHazirla(servisTMM, kullanan, yil, fisNo, sd, excel);
            }
            else
                TasinirIslemFormYazdir.Yazdir(kullanan, yil, fisNo, harcamaKod, muhasebeKod, excel, "", islemTipi);

            OturumBilgisiIslem.BilgiYazDegisken("DosyaGoruntuleDosyaAdi", excel);

            if (File.Exists(excel))
            {
                pnl.LoadContent("DosyaGoruntule.aspx");//?dosya=" + excel
                if (tamEkran == 1)
                    winTamEkran.Show();
            }
            else
                pnl.ClearContent();

            lblYil.Text = yil.ToString();
            lblBelgeNo.Text = fisNo;
            lblMuhasebe.Text = muhasebeKod;
            lblHarcamaBirimi.Text = harcamaKod;

            cDokumanBilgi.Show();
        }

    }
}