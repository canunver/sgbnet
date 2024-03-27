using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace TasinirMal
{
    /// <summary>
    /// Geçici alýndý fiþi bilgilerinin sorgulama ve yazdýrma iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIslemFormOnaySorguMB : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

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
                formAdi = "B/A Onayý Sorgulama";

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                //pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                pgFiltre.UpdateProperty("prpBelgeTarihi1", DateTime.Now.AddDays(-30).Date);
                pgFiltre.UpdateProperty("prpBelgeTarihi2", DateTime.Now.Date);
                pgFiltre.UpdateProperty("prpOnayTur", (int)ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI);

                OnayDurumDoldur();
                IslemTipDoldur();
            }
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplanýr ve sunucuya gönderilir
        /// ve geçici alýndý fiþi bilgileri sunucudan alýnýr. Hata varsa ekrana hata bilgisi yazýlýr,
        /// yoksa gelen geçici alýndý fiþi bilgileri gvBelgeler GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
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

            kriter.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            kriter.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            kriter.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            kriter.bOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            kriter.aOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            //kriter.aOnayTarih.AddDays(1);
            //kriter.aOnaylayan = pgFiltre.Source["prpKisi"].Value;
            kriter.onayDurum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpOnayTur"].Value, 0);

            if (kriter.onayDurum == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Onay Tür boþ býrakýlamaz.");
                return;
            }


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

                        dt.Rows.Add(tasForm.fisNo.Trim(), tasForm.yil, tasForm.gYil, tasForm.muhasebeKod, tasForm.harcamaKod, tasForm.harcamaAd, tasForm.ambarKod, tasForm.fisTarih.ToString(), tasForm.islemTipKod, durum, tasForm.islemTarih.ToString(), tasForm.islemYapan, tasForm.kod, tasForm.onayDurum, tasForm.onayAciklama);
                    }
                }
                else
                    GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr + bilgi.sonuc.vtHatasi);
            }

            strListe.DataSource = dt;
            strListe.DataBind();
        }

        /// <summary>
        /// Liste Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplanýr, sunucuya gönderilir
        /// ve geçici alýndý fiþi bilgileri sunucudan alýnýr. Hata varsa ekrana hata bilgisi
        /// yazýlýr, yoksa gelen geçici alýndý fiþi bilgilerini içeren excel raporu üretilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            BAOnay kriter = new BAOnay();

            kriter.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            kriter.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            kriter.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            kriter.bOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            kriter.aOnayTarih = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            kriter.aOnayTarih.AddDays(1);
            //kriter.aOnaylayan = pgFiltre.Source["prpKisi"].Value;

            ObjectArray bilgi = servisTMM.BAOnayiSorgulama(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int yazSatir = 0;

            string sablonAd = "BAOnayListesi.xlt";
            string sonucDosyaAd = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref yazSatir, ref sutun);

            foreach (BAOnay onay in bilgi.objeler)
            {
                string tur = "Fiþ";
                if (onay.fisNo.StartsWith("A")) tur = "Amortisman";
                else if (onay.fisNo.StartsWith("B")) tur = "Deðer Artýþý";

                XLS.SatirAc(yazSatir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, yazSatir, sutun);

                XLS.HucreDegerYaz(yazSatir, sutun, onay.fisNo);
                XLS.HucreDegerYaz(yazSatir, sutun + 1, tur);
                XLS.HucreDegerYaz(yazSatir, sutun + 2, onay.gonderenKisi);
                XLS.HucreDegerYaz(yazSatir, sutun + 3, onay.gonderimTarih.ToString());
                XLS.HucreDegerYaz(yazSatir, sutun + 4, onay.bOnaylayan);
                XLS.HucreDegerYaz(yazSatir, sutun + 5, onay.bOnayTarih.ToString());
                XLS.HucreDegerYaz(yazSatir, sutun + 6, onay.aOnaylayan);
                XLS.HucreDegerYaz(yazSatir, sutun + 7, onay.aOnayTarih.ToString());

                yazSatir++;
            }

            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpBelgeTarihi1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi2", string.Empty);
            //pgFiltre.UpdateProperty("prpKisi", string.Empty);
            pgFiltre.UpdateProperty("prpOnayTur", (int)ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI);
        }

        [DirectMethod]
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
                TasinirIslemFormYazdir.Yazdir(kullanan, yil, fisNo, harcamaKod, muhasebeKod, excel, "");

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
    }
}