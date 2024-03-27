using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TasinirMal
{
    /// <summary>
    /// Geçici alýndý fiþi bilgilerinin sorgulama ve yazdýrma iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class SicilNoDegerArtisSorgu : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

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
                formAdi = "Sicil No Deðer Artýþý";

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));

                string gelenPrSicilNo = Request.QueryString["prSicilNo"] + "";
                if (gelenPrSicilNo != "")
                {
                    SicilNoHareket shBilgi = new SicilNoHareket();
                    shBilgi.prSicilNo = OrtakFonksiyonlar.ConvertToInt(gelenPrSicilNo, 0);
                    ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
                    foreach (SicilNoHareket sh in bilgi.objeler)
                    {
                        pgFiltre.UpdateProperty("prpMuhasebe", sh.muhasebeKod);
                        pgFiltre.UpdateProperty("prpHarcamaBirimi", sh.harcamaBirimKod);
                        pgFiltre.UpdateProperty("prpSicilNo", sh.sicilNo);
                    }
                }

                DurumDoldur();
                TurDoldur();
            }
        }

        private void DurumDoldur()
        {
            List<object> liste = new List<object>
            {
                new { KOD = (int)ENUMBelgeDurumu.YENI, ADI = "Yeni" },
                new { KOD = (int)ENUMBelgeDurumu.ONAYLI, ADI = "Onaylý" },
                new { KOD = (int)ENUMBelgeDurumu.IPTAL, ADI = "Ýptal Edilen Belgeler" },
                new { KOD = 0, ADI = "Bütün Belgeler (Ýptaller Hariç)" }
            };

            strDurum.DataSource = liste;
            strDurum.DataBind();
            pgFiltre.UpdateProperty("prpDurum", 0);
        }

        private void TurDoldur()
        {
            List<object> liste = new List<object>
            {
                new { KOD = 1, ADI = "Enflasyon Düzeltme Farký" },
                new { KOD = 2, ADI = "Deðer Artýþý" },
                new { KOD = 3, ADI = "Deðer Azalýþý" },
                new { KOD = 0, ADI = "Hepsi" },
            };

            strTur.DataSource = liste;
            strTur.DataBind();
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
            SicilNoDegerArtis tf = new SicilNoDegerArtis();

            tf.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            tf.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            tf.sorguBelgeNoBas = pgFiltre.Source["prpBelgeNo1"].Value.Trim() == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo1"].Value.Trim().PadLeft(6, '0');
            tf.sorguBelgeNoBit = pgFiltre.Source["prpBelgeNo2"].Value.Trim() == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo2"].Value.Trim().PadLeft(6, '0');
            tf.sorguBelgeBasTarihi = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            tf.sorguBelgeBitTarihi = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            tf.gorSicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim();
            tf.sorguEskiSicilNo = pgFiltre.Source["prpEskiSicilNo"].Value.Trim();
            tf.tur = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpTur"].Value.Trim(), 0);
            tf.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurum"].Value.Trim(), 0);

            ObjectArray bilgi = servisTMM.SicilNoDegerArtisListele(kullanan, tf);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (SicilNoDegerArtis t in bilgi.objeler)
            {
                string durum = "";
                if (t.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    durum = "Onaylý";
                else if (t.durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                    durum = "Belge Onaysýz";
                else if (t.durum == (int)ENUMBelgeDurumu.IPTAL)
                    durum = "Ýptal";
                else if (t.durum == (int)ENUMBelgeDurumu.TANIMSIZ || t.durum == 3)
                    durum = "Onaya Gönderildi";
                else if (t.durum == (int)ENUMBelgeDurumu.YENI)
                    durum = "Yeni";

                liste.Add(new
                {
                    KOD = t.kod.Trim(),
                    MUHASEBEKOD = t.muhasebeKod,
                    HARCAMABIRIMIKOD = t.harcamaKod,
                    MUHASEBE = t.muhasebeKod + "-" + t.muhasebeAd,
                    HARCAMABIRIMI = t.harcamaKod + "-" + t.harcamaAd,
                    BELGENO = t.belgeNo,
                    BELGETARIHI = t.belgeTarihi.Oku(),
                    PRSICILNO = t.prSicilNo,
                    GORSICILNO = t.gorSicilNo,
                    TUTAR = TasinirGenel.ParaFormatla(t.tutar),
                    GEREKCE = t.gerekce,
                    DURUM = durum,
                    MALZEMEADI = t.gorSicilNo + "-" + t.malzemeAdi,
                    TUR = t.tur,
                    TURADI = TurAdi(t.tur),
                    AMORTISMAN = TasinirGenel.ParaFormatla(t.amotismanTutar)
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpTur", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNo1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNo2", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi1", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeTarihi2", string.Empty);
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
            SicilNoDegerArtis tf = new SicilNoDegerArtis();

            tf.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            tf.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            tf.sorguBelgeNoBas = pgFiltre.Source["prpBelgeNo1"].Value.Trim() == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo1"].Value.Trim().PadLeft(6, '0');
            tf.sorguBelgeNoBit = pgFiltre.Source["prpBelgeNo2"].Value.Trim() == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo2"].Value.Trim().PadLeft(6, '0');
            tf.sorguBelgeBasTarihi = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi1"].Value.Trim());
            tf.sorguBelgeBitTarihi = new TNSDateTime(pgFiltre.Source["prpBelgeTarihi2"].Value.Trim());
            tf.tur = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpTur"].Value.Trim(), 0);

            ObjectArray bilgi = servisTMM.SicilNoDegerArtisListele(kullanan, tf);

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
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "SicilNoDegerArtisListe.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                SicilNoDegerArtis t = (SicilNoDegerArtis)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 0, t.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 1, t.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 2, t.belgeNo);

                XLS.HucreDegerYaz(satir, sutun + 3, t.belgeTarihi.ToString());
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        protected void btnOnayaGonder_Click(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["json"];

            if (string.IsNullOrEmpty(json) || ((JArray)JSON.Deserialize(json)).Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listeden iþlem yapýlacak belge seçilmemiþ.");
                return;
            }

            Sonuc sonuc = new Sonuc();
            string bilgiStr = string.Empty;

            foreach (JContainer jc in (JArray)JSON.Deserialize(json))
            {
                SicilNoDegerArtis tf = new SicilNoDegerArtis();

                tf.belgeNo = (jc.Value<string>("BELGENO")).Trim().PadLeft(6, '0');
                tf.muhasebeKod = jc.Value<string>("MUHASEBEKOD");
                tf.harcamaKod = (jc.Value<string>("HARCAMABIRIMIKOD") + "").Replace(".", "");
                tf.kod = jc.Value<string>("KOD");

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



                AmortismanIslemForm form = new AmortismanIslemForm()
                {
                    yil = new TNSDateTime(jc.Value<DateTime>("BELGETARIHI")).Yil,
                    donem = jc.Value<int>("BELGENO"),
                    muhasebeKod = jc.Value<string>("MUHASEBEKOD"),
                    harcamaKod = (jc.Value<string>("HARCAMABIRIMIKOD") + "").Replace(".", ""),
                    ambarKod = ilgiliAmbarKod,
                    tip = "DEGERARTIS",
                    durum = (int)ENUMTasinirIslemFormOnayDurumu.TANIMSIZ,
                    onayDurum = (int)ENUMTasinirIslemFormOnayDurumu.TANIMSIZ
                };
                sonuc = servisTMM.AmortismanIslemFisiKaydet(kullanan, form);
                if (!sonuc.islemSonuc)
                {
                    GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                    return;
                }

                AmortismanIslemForm from = new AmortismanIslemForm()
                {
                    yil = new TNSDateTime(jc.Value<DateTime>("BELGETARIHI")).Yil,
                    donem = jc.Value<int>("BELGENO"),
                    muhasebeKod = jc.Value<string>("MUHASEBEKOD"),
                    harcamaKod = (jc.Value<string>("HARCAMABIRIMIKOD") + "").Replace(".", ""),
                    ambarKod = ilgiliAmbarKod,
                    tip = "DEGERARTIS",
                };

                sonuc = servisTMM.AmortismanIslemFisiOnayDurumDegistir(kullanan, from, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);
                if (!sonuc.islemSonuc)
                {
                    GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                    return;
                }

                sonuc = servisTMM.SicilNoDegerArtisDurumGuncelle(kullanan, tf, "OnayaGonder");

                if (sonuc.islemSonuc)
                    GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                else
                    GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            }
        }

        protected void btnIptal_Click(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["json"];

            if (string.IsNullOrEmpty(json) || ((JArray)JSON.Deserialize(json)).Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listeden iþlem yapýlacak belge seçilmemiþ.");
                return;
            }

            string hata = "";
            foreach (JContainer jc in (JArray)JSON.Deserialize(json))
            {
                SicilNoDegerArtis tf = new SicilNoDegerArtis();

                tf.belgeNo = (jc.Value<string>("BELGENO")).Trim().PadLeft(6, '0');
                tf.muhasebeKod = jc.Value<string>("MUHASEBEKOD");
                tf.harcamaKod = (jc.Value<string>("HARCAMABIRIMIKOD") + "").Replace(".", "");
                tf.kod = jc.Value<string>("KOD");

                Sonuc sonuc = servisTMM.SicilNoDegerArtisDurumGuncelle(kullanan, tf, "Iptal");
                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr + "<br>";
            }

            if (hata != "")
                GenelIslemler.MesajKutusu("Hata", hata);
            else
                GenelIslemler.MesajKutusu("Bilgi", "Ýptal iþlemi tamamlandý.");
        }

        private string TurAdi(int tur)
        {
            string adi = "";
            if (tur == 1) adi = "Enflasyon Düzeltme Farký";
            else if (tur == 2) adi = "Deðer Artýþý";
            else if (tur == 3) adi = "Deðer Azalýþý";

            return adi;
        }

    }
}