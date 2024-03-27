using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Sayým tutanaðý bilgilerinin kayýt, listeleme, silme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class SayimGiris : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama, ambar ve sayimNo
        ///     girdi dizgileri dolu ise ilgili sayým tutanaðý bilgileri listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMSYG001;
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtYil.Value = DateTime.Now.Year;
                GridTemizle();

                if (!string.IsNullOrEmpty(Request.QueryString["yil"])
                     && !string.IsNullOrEmpty(Request.QueryString["muhasebe"])
                     && !string.IsNullOrEmpty(Request.QueryString["harcama"])
                     && !string.IsNullOrEmpty(Request.QueryString["ambar"])
                     && !string.IsNullOrEmpty(Request.QueryString["sayimNo"]))
                {
                    txtYil.Value = Request.QueryString["yil"];
                    txtMuhasebe.Value = Request.QueryString["muhasebe"];
                    txtHarcamaBirimi.Value = Request.QueryString["harcama"];
                    txtAmbar.Value = Request.QueryString["ambar"];
                    txtBelgeNo.Value = Request.QueryString["sayimNo"];

                    Listele(KriterTopla());
                }
                else
                {
                    txtSayimTarihi.Value = DateTime.Now.Date;
                    txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                    txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                    txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
                }
            }
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Sayým tutanaðý bilgilerini ekrandaki ilgili kontrollerden toplayan yordam çaðýrýlýr
        /// ve daha sonra toplanan bilgiler kaydedilmek üzere Kaydet yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            SayimForm sf = KriterTopla();

            if (sf.yil != sf.sayimTarih.Yil)
            {
                GenelIslemler.ExtNotification("Sayým yýlý ile sayým tarihi arasýnda uyumsuzluk var. Lütfen ayný yýl içinde tarih bilgisi seçin", "Uyarý", Icon.Information);
                return;
            }

            int siraNo = 1;
            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                SayimDetay detay = new SayimDetay();
                detay.yil = sf.yil;
                detay.muhasebeKod = sf.muhasebeKod;
                detay.harcamaKod = sf.harcamaKod;
                detay.ambarKod = sf.ambarKod;
                detay.sayimNo = sf.sayimNo;

                detay.hesapPlanKod = TasinirGenel.DegerAlStr(item, "hesapPlanKod");

                detay.ambarMiktar = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlStr(item, "ambarMiktar").Replace(".", ","));
                detay.ortakMiktar = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlStr(item, "ortakMiktar").Replace(".", ","));

                detay.aciklama = TasinirGenel.DegerAlStr(item, "aciklama");

                if (!string.IsNullOrEmpty(detay.hesapPlanKod) && (detay.ambarMiktar < 0 || detay.ortakMiktar < 0))
                {
                    GenelIslemler.MesajKutusu("Hata", string.Format(Resources.TasinirMal.FRMSYG016, (siraNo).ToString()));
                    return;
                }

                if (!string.IsNullOrEmpty(detay.hesapPlanKod))
                    sf.detay.Add(detay);

                siraNo++;
            }

            Sonuc sonuc = servisTMM.SayimKaydet(kullanan, sf);
            if (sonuc.islemSonuc)
            {
                txtBelgeNo.Value = sonuc.anahtar;
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

                ButonlariAktifYap();
                Listele(KriterTopla());
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Listele resmine basýlýnca çalýþan olay metodu
        /// Sunucudan geçici alýndý fiþinin bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Sayým tutanaðý bilgilerini ekrandaki ilgili kontrollerden toplayan yordam çaðýrýlýr
        /// ve daha sonra toplanan bilgiler silinmek üzere Sil yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            SayimForm sf = KriterTopla();
            Sonuc sonuc = servisTMM.SayimSil(kullanan, sf);
            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG018, "Bilgi", Icon.Information);
                btnTemizle_Click(null, null);
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
            GridTemizle();

            txtBelgeNo.Value = "";
        }

        /// <summary>
        /// Ambardakileri Aktar tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriter bilgilerini ekrandaki ilgili kontrollerden toplayan yordam çaðýrýlýr ve
        /// daha sonra toplanan kriterler ambardaki malzemeleri listeleyen AmbarAktar yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAmbarAktar_Click(object sender, DirectEventArgs e)
        {
            string islem = e.ExtraParams["islem"];

            SayimForm kriter = KriterTopla();

            if (kriter.yil != kriter.sayimTarih.Yil)
            {
                GenelIslemler.ExtNotification("Sayým yýlý ile sayým tarihi arasýnda uyumsuzluk var. Lütfen ayný yýl içinde tarih bilgisi seçin", "Uyarý", Icon.Information);
                return;
            }

            ObjectArray bilgi = servisTMM.SayimAmbardakileriGetir(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }


            if (islem == "LISTELE")
            {
                DataTable dt = GridDataTable();

                foreach (SayimDetay sd in bilgi.objeler)
                {
                    if (sd.kayitAmbarMiktar > 0 || sd.kayitOrtakMiktar > 0 || sd.kayitKisiMiktar > 0)
                    {
                        DataRow row = dt.NewRow();
                        row["hesapPlanKod"] = sd.hesapPlanKod;
                        row["hesapPlanAd"] = sd.hesapPlanAd;
                        row["olcuBirimAd"] = sd.olcuBirimAd;
                        row["ambarMiktar"] = sd.kayitAmbarMiktar;
                        row["ortakMiktar"] = sd.kayitOrtakMiktar;
                        row["kayitKisiMiktar"] = sd.kayitKisiMiktar;
                        row["aciklama"] = sd.aciklama;

                        dt.Rows.Add(row);
                    }

                }

                strListe.DataSource = dt;
                strListe.DataBind();
            }
            else if (islem == "KAYDET")
            {
                SayimForm sf = new SayimForm();

                sf.yil = kriter.yil;
                sf.muhasebeKod = kriter.muhasebeKod;
                sf.harcamaKod = kriter.harcamaKod;
                sf.ambarKod = kriter.ambarKod;
                sf.sayimNo = kriter.sayimNo;
                sf.sayimTarih = kriter.sayimTarih;


                foreach (SayimDetay sd in bilgi.objeler)
                {
                    if (sd.kayitAmbarMiktar > 0 || sd.kayitOrtakMiktar > 0 || sd.kayitKisiMiktar > 0)
                    {
                        SayimDetay detay = new SayimDetay();
                        detay.yil = kriter.yil;
                        detay.muhasebeKod = kriter.muhasebeKod;
                        detay.harcamaKod = kriter.harcamaKod;
                        detay.ambarKod = kriter.ambarKod;
                        detay.sayimNo = kriter.sayimNo;
                        detay.hesapPlanKod = sd.hesapPlanKod;
                        detay.ambarMiktar = sd.kayitAmbarMiktar;
                        detay.ortakMiktar = sd.kayitOrtakMiktar;

                        sf.detay.Add(detay);
                    }
                }

                Sonuc sonuc = servisTMM.SayimKaydet(kullanan, sf);
                if (!sonuc.islemSonuc)
                    GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
                else
                {
                    txtBelgeNo.Value = sonuc.anahtar;

                    ButonlariAktifYap();
                    Listele(KriterTopla());
                    GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
                }
            }


        }

        /// <summary>
        /// Sayým Tutanaðý tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan
        /// kriterler SayimTutanakYaz yordamýna gönderilir ve rapor hazýrlanmýþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSayimTutanak_Click(object sender, DirectEventArgs e)
        {
            SayimTutanakYaz(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen sayým formuna ait kriterleri
        /// sunucudaki sayým tutanaðý raporlama yordamýna gönderir,
        /// sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="sForm">Sayým tutanaðý kriter bilgilerini tutan nesne</param>
        private void SayimTutanakYaz(SayimForm sForm)
        {
            ObjectArray bilgi = servisTMM.SayimRaporListele(kullanan, sForm, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "SAYIMTUTANAK.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            string eskiHesapKod = string.Empty;

            SayimForm sf = (SayimForm)bilgi.objeler[0];

            for (int i = 0; i < sf.detay.Count; i++)
            {
                SayimDetay sd = (SayimDetay)sf.detay[i];

                if (!sd.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                {
                    if (sd.hesapPlanKod.Length >= 9)
                        eskiHesapKod = sd.hesapPlanKod.Substring(0, 9);
                    else
                        eskiHesapKod = sd.hesapPlanKod.Substring(0, sd.hesapPlanKod.Length);

                    if (i != 0)
                    {
                        SayimTutanakImzaEkle(XLS, ref satir, sutun);

                        XLS.SayfaSonuKoyHucresel(satir + 3);
                        satir += 3;
                        XLS.SatirYukseklikAyarla(satir - 2, satir, GenelIslemler.JexcelBirimtoExcelBirim(315));
                    }

                    XLS.SatirAc(satir, 7);
                    XLS.SatirYukseklikAyarla(satir, satir + 5, GenelIslemler.JexcelBirimtoExcelBirim(315));
                    XLS.SatirYukseklikAyarla(satir + 6, satir + 6, GenelIslemler.JexcelBirimtoExcelBirim(1000));
                    XLS.HucreKopyala(kaynakSatir - 7, sutun, kaynakSatir - 1, sutun + 12, satir, sutun);

                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 12);

                    for (int j = satir + 2; j <= satir + 4; j++)
                        XLS.HucreBirlestir(j, sutun + 2, j, sutun + 8);

                    //XLS.HucreBirlestir(satir + 3, sutun + 11, satir + 5, sutun + 11);
                    //XLS.HucreBirlestir(satir + 3, sutun + 12, satir + 5, sutun + 12);

                    XLS.HucreBirlestir(satir + 6, sutun + 1, satir + 6, sutun + 3);

                    XLS.HucreDegerYaz(satir + 2, sutun + 2, sf.ilAd + "-" + sf.ilceAd);
                    XLS.HucreDegerYaz(satir + 2, sutun + 10, sf.ilKod + "-" + sf.ilceKod);
                    XLS.HucreDegerYaz(satir + 2, sutun + 12, sf.yil);
                    XLS.HucreDegerYaz(satir + 3, sutun + 2, sf.harcamaAd);
                    XLS.HucreDegerYaz(satir + 3, sutun + 10, sf.harcamaKod);
                    XLS.HucreDegerYaz(satir + 3, sutun + 12, eskiHesapKod);
                    XLS.HucreDegerYaz(satir + 4, sutun + 2, sf.ambarAd);
                    XLS.HucreDegerYaz(satir + 4, sutun + 10, sf.ambarKod);
                    //XLS.HucreDegerYaz(satir + 5, sutun + 2, sf.muhasebeAd);
                    //XLS.HucreDegerYaz(satir + 5, sutun + 10, sf.muhasebeKod);

                    satir += 6;
                }

                if (sd.kayitAmbarMiktar > 0 || sd.ambarMiktar > 0 ||
                    sd.kayitOrtakMiktar > 0 || sd.ortakMiktar > 0 || sd.kayitKisiMiktar > 0 ||
                    sd.fazlaMiktar > 0 || sd.noksanMiktar > 0)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 3);
                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(315));

                    XLS.HucreDegerYaz(satir, sutun, sd.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, sd.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, sd.olcuBirimAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(sd.kayitAmbarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(sd.ambarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(sd.kayitOrtakMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(sd.ortakMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(sd.kayitKisiMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(sd.fazlaMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(sd.noksanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 12, sd.aciklama);

                    if (sd.kayitAmbarMiktar != sd.ambarMiktar)
                        XLS.ArkaPlanRenk(satir, sutun + 5, satir, sutun + 6, OrtakClass.TabloRenk.VERY_LIGHT_YELLOW);
                    if (sd.kayitOrtakMiktar != sd.ortakMiktar)
                        XLS.ArkaPlanRenk(satir, sutun + 7, satir, sutun + 8, OrtakClass.TabloRenk.VERY_LIGHT_YELLOW);
                    if (sd.fazlaMiktar > 0)
                        XLS.ArkaPlanRenk(satir, sutun + 10, OrtakClass.TabloRenk.PALE_BLUE);
                    if (sd.noksanMiktar > 0)
                        XLS.ArkaPlanRenk(satir, sutun + 11, OrtakClass.TabloRenk.PALE_BLUE);
                }
            }
            SayimTutanakImzaEkle(XLS, ref satir, sutun);
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Sayým tutanaðý excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        private void SayimTutanakImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 2;

            ObjectArray imza1 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.SAYIMKURULUBASKANI);
            ImzaBilgisi i1 = null;
            if (imza1.sonuc.islemSonuc && imza1.objeler.Count > 0)
                i1 = (ImzaBilgisi)imza1.objeler[0];
            string ad1 = string.Empty;
            string unvan1 = string.Empty;
            if (i1 != null)
            {
                ad1 = i1.adSoyad;
                unvan1 = i1.unvan;
            }

            ObjectArray imza2 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.SAYIMKURULUUYE1);
            ImzaBilgisi i2 = null;
            if (imza2.sonuc.islemSonuc && imza2.objeler.Count > 0)
                i2 = (ImzaBilgisi)imza2.objeler[0];
            string ad2 = string.Empty;
            string unvan2 = string.Empty;
            if (i2 != null)
            {
                ad2 = i2.adSoyad;
                unvan2 = i2.unvan;
            }

            ObjectArray imza3 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.SAYIMKURULUUYE2);
            ImzaBilgisi i3 = null;
            if (imza3.sonuc.islemSonuc && imza3.objeler.Count > 0)
                i3 = (ImzaBilgisi)imza3.objeler[0];
            string ad3 = string.Empty;
            string unvan3 = string.Empty;
            if (i3 != null)
            {
                ad3 = i3.adSoyad;
                unvan3 = i3.unvan;
            }

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 6, sutun + 12, satir, sutun);

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 12);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMSYG005);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 2);
            XLS.YatayHizala(satir, sutun, 2);

            XLS.HucreDegerYaz(satir + 1, sutun + 1, Resources.TasinirMal.FRMSYG006);
            XLS.HucreDegerYaz(satir + 1, sutun + 5, Resources.TasinirMal.FRMSYG007);
            XLS.HucreDegerYaz(satir + 1, sutun + 9, Resources.TasinirMal.FRMSYG007);

            XLS.HucreDegerYaz(satir + 2, sutun + 1, string.Format(Resources.TasinirMal.FRMSYG008, ad1));
            XLS.HucreDegerYaz(satir + 3, sutun + 1, string.Format(Resources.TasinirMal.FRMSYG009, unvan1));
            XLS.HucreDegerYaz(satir + 4, sutun + 1, Resources.TasinirMal.FRMSYG010);

            XLS.HucreDegerYaz(satir + 2, sutun + 5, ad2);
            XLS.HucreDegerYaz(satir + 3, sutun + 5, unvan2);
            //XLS.HucreDegerYaz(satir + 4, sutun + 5, "Ýmzasý : ");

            XLS.HucreDegerYaz(satir + 2, sutun + 9, ad3);
            XLS.HucreDegerYaz(satir + 3, sutun + 9, unvan3);
            //XLS.HucreDegerYaz(satir + 4, sutun + 9, "Ýmzasý : ");

            for (int i = satir + 1; i < satir + 5; i++)
            {
                for (int j = sutun + 1; j <= sutun + 9; j += 4)
                {
                    XLS.HucreBirlestir(i, j, i, j + 2);

                    if (i == satir + 1)
                    {
                        XLS.KoyuYap(i, j, true);
                        XLS.DuseyHizala(i, j, 2);
                        XLS.YatayHizala(i, j, 2);
                    }
                }
            }

            XLS.YatayCizgiCiz(satir, sutun, sutun + 12, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 1, sutun, sutun + 12, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 12, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 13, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.SatirYukseklikAyarla(satir, satir + 5, GenelIslemler.JexcelBirimtoExcelBirim(315));

            satir += 5;
        }

        /// <summary>
        /// Ambar Devir ve Teslim Tutanaðý tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler
        /// AmbarDevirTutanakYaz yordamýna gönderilir ve rapor hazýrlanmýþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAmbarDevirTutanak_Click(object sender, DirectEventArgs e)
        {
            AmbarDevirTutanakYaz(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen sayým formuna ait kriterleri
        /// sunucudaki sayým tutanaðý raporlama yordamýna gönderir,
        /// sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="sForm">Ambar devir ve teslim tutanaðý kriter bilgilerini tutan nesne</param>
        private void AmbarDevirTutanakYaz(SayimForm sForm)
        {
            ObjectArray bilgi = servisTMM.SayimRaporListele(kullanan, sForm, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AMBARDEVIRTESLIMTUTANAK.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            SayimForm sf = (SayimForm)bilgi.objeler[0];
            XLS.HucreAdBulYaz("IlAd", sf.ilAd + "-" + sf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", sf.ilKod + "-" + sf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", sf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", sf.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", sf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", sf.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", sf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", sf.muhasebeKod);

            satir = kaynakSatir;

            int sayac = 0;
            for (int i = 0; i < sf.detay.Count; i++)
            {
                SayimDetay sd = (SayimDetay)sf.detay[i];

                if (sd.ambarMiktar > 0 || sd.kayitAmbarMiktar > 0 ||
                    sd.fazlaMiktar > 0 || sd.noksanMiktar > 0)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                    sayac++;
                    XLS.HucreDegerYaz(satir, sutun, sayac.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 1, sd.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, sd.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, sd.olcuBirimAd);
                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(sd.ambarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(sd.kayitAmbarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(sd.fazlaMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(sd.noksanMiktar.ToString(), (double)0));
                }
            }

            AmbarDevirTutanakImzaEkle(XLS, ref satir, sutun);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Ambar devir ve teslim tutanaðý excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        private void AmbarDevirTutanakImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 2;

            ObjectArray imza1 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUBASKANI);
            ImzaBilgisi i1 = null;
            if (imza1.sonuc.islemSonuc && imza1.objeler.Count > 0)
                i1 = (ImzaBilgisi)imza1.objeler[0];
            string ad1 = string.Empty;
            string unvan1 = string.Empty;
            if (i1 != null)
            {
                ad1 = i1.adSoyad;
                unvan1 = i1.unvan;
            }

            ObjectArray imza2 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUUYE1);
            ImzaBilgisi i2 = null;
            if (imza2.sonuc.islemSonuc && imza2.objeler.Count > 0)
                i2 = (ImzaBilgisi)imza2.objeler[0];
            string ad2 = string.Empty;
            string unvan2 = string.Empty;
            if (i2 != null)
            {
                ad2 = i2.adSoyad;
                unvan2 = i2.unvan;
            }

            ObjectArray imza3 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUUYE2);
            ImzaBilgisi i3 = null;
            if (imza3.sonuc.islemSonuc && imza3.objeler.Count > 0)
                i3 = (ImzaBilgisi)imza3.objeler[0];
            string ad3 = string.Empty;
            string unvan3 = string.Empty;
            if (i3 != null)
            {
                ad3 = i3.adSoyad;
                unvan3 = i3.unvan;
            }

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 6, sutun + 9, satir, sutun);

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMSYG011);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 2);
            XLS.YatayHizala(satir, sutun, 2);

            XLS.HucreDegerYaz(satir + 1, sutun, Resources.TasinirMal.FRMSYG006);
            XLS.HucreDegerYaz(satir + 1, sutun + 2, Resources.TasinirMal.FRMSYG007);
            XLS.HucreDegerYaz(satir + 1, sutun + 4, Resources.TasinirMal.FRMSYG007);
            XLS.HucreDegerYaz(satir + 1, sutun + 6, Resources.TasinirMal.FRMSYG012);
            XLS.HucreDegerYaz(satir + 1, sutun + 8, Resources.TasinirMal.FRMSYG013);

            XLS.HucreDegerYaz(satir + 2, sutun, string.Format(Resources.TasinirMal.FRMSYG008, ad1));
            XLS.HucreDegerYaz(satir + 3, sutun, string.Format(Resources.TasinirMal.FRMSYG009, unvan1));
            XLS.HucreDegerYaz(satir + 4, sutun, Resources.TasinirMal.FRMSYG010);

            XLS.HucreDegerYaz(satir + 2, sutun + 2, ad2);
            XLS.HucreDegerYaz(satir + 3, sutun + 2, unvan2);
            //XLS.HucreDegerYaz(satir + 4, sutun + 5, "Ýmzasý : ");

            XLS.HucreDegerYaz(satir + 2, sutun + 4, ad3);
            XLS.HucreDegerYaz(satir + 3, sutun + 4, unvan3);
            //XLS.HucreDegerYaz(satir + 4, sutun + 9, "Ýmzasý : ");

            XLS.HucreDegerYaz(satir + 2, sutun + 6, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 3, sutun + 6, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 4, sutun + 6, Resources.TasinirMal.FRMSYG014);

            XLS.HucreDegerYaz(satir + 2, sutun + 8, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 3, sutun + 8, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 4, sutun + 8, Resources.TasinirMal.FRMSYG014);

            XLS.HucreDegerYaz(satir + 5, sutun + 2, string.Format(Resources.TasinirMal.FRMSYG015, DateTime.Today.Date.ToShortDateString()));
            XLS.HucreBirlestir(satir + 5, sutun + 2, satir + 5, sutun + 3);
            XLS.DuseyHizala(satir + 5, sutun + 2, 2);

            XLS.HucreDegerYaz(satir + 5, sutun + 7, string.Format(Resources.TasinirMal.FRMSYG015, DateTime.Today.Date.ToShortDateString()));
            XLS.HucreBirlestir(satir + 5, sutun + 7, satir + 5, sutun + 8);
            XLS.DuseyHizala(satir + 5, sutun + 7, 2);

            for (int i = satir + 1; i < satir + 5; i++)
            {
                for (int j = sutun; j <= sutun + 8; j += 2)
                {
                    XLS.HucreBirlestir(i, j, i, j + 1);

                    if (i == satir + 1)
                    {
                        XLS.KoyuYap(i, j, true);
                        XLS.DuseyHizala(i, j, 2);
                        XLS.YatayHizala(i, j, 2);
                    }
                }
            }

            XLS.YatayCizgiCiz(satir, sutun, sutun + 9, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 1, sutun, sutun + 5, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 9, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 6, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 10, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 5;
        }

        /// <summary>
        /// Noksandan TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Sayýmda eksik olduðu bulunan taþýnýr malzemeleri için taþýnýr
        /// iþlem fiþi oluþturmak üzere TIFOlustur yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIFNoksan_Click(object sender, DirectEventArgs e)
        {
            TIFOlustur(true);
        }

        /// <summary>
        /// Fazladan TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Sayýmda fazla olduðu bulunan taþýnýr malzemeleri için taþýnýr
        /// iþlem fiþi oluþturmak üzere TIFOlustur yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIFFazla_Click(object sender, DirectEventArgs e)
        {
            TIFOlustur(false);
        }

        /// <summary>
        /// Sayým tutanaðý ile ambardaki malzemeler arasýnda miktar farklýlýklarýný arar.
        /// Eðer farklýlýk varsa ilgili malzeme bilgilerini sessiona yazar ve taþýnýr iþlem
        /// fiþi ekranýna yönlendirir, yoksa farklýlýk olmadýðýna iliþkin hata mesajý verir.
        /// </summary>
        /// <param name="noksanMi">Eksik miktar farklýlýklarý mý, yoksa fazla miktar farklýlýklarý mý aranacak bilgisi</param>
        private void TIFOlustur(bool noksanMi)
        {
            SayimForm sf = KriterTopla();
            if (string.IsNullOrEmpty(sf.sayimNo))
            {
                GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMSYG002);
                return;
            }

            ObjectArray bilgi = servisTMM.SayimRaporListele(kullanan, sf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            bool eksikVarMi = false;

            SayimForm sForm = (SayimForm)bilgi.objeler[0];

            foreach (SayimDetay sd in sForm.detay)
                if (noksanMi)
                    sd.fazlaMiktar = 0;
                else
                    sd.noksanMiktar = 0;

            for (int i = 0; i < sForm.detay.Count; i++)
            {
                SayimDetay sd = (SayimDetay)sForm.detay[i];

                if ((sd.fazlaMiktar > 0 || sd.noksanMiktar > 0) && !eksikVarMi)
                    eksikVarMi = true;
                else if (sd.fazlaMiktar <= 0 && sd.noksanMiktar <= 0)
                {
                    sForm.detay.RemoveAt(i);
                    i--;
                }
            }

            if (eksikVarMi)
            {
                TNS.TMM.TasinirIslemForm form = new TNS.TMM.TasinirIslemForm();

                form.yil = DateTime.Now.Year;
                form.muhasebeKod = sForm.muhasebeKod;
                form.harcamaKod = sForm.harcamaKod;
                form.ambarKod = sForm.ambarKod;
                form.fisTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
                form.nereyeGitti = ".";

                int siraNo = 1;
                foreach (SayimDetay detay in sForm.detay)
                {
                    if (detay.fazlaMiktar == 0 && detay.noksanMiktar == 0)
                        continue;

                    if (form.islemTipKod == 0)
                    {
                        if (detay.fazlaMiktar > 0)
                            form.islemTipTur = (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS;
                        else if (detay.noksanMiktar > 0)
                            form.islemTipTur = (int)ENUMIslemTipi.SAYIMNOKSANICIKIS;

                        form.islemTipKod = TasinirGenel.IslemTipiGetir(servisTMM, kullanan, form.islemTipTur, false);
                    }

                    TNS.TMM.TasinirIslemDetay tfd = new TasinirIslemDetay();
                    tfd.yil = form.yil;
                    tfd.muhasebeKod = form.muhasebeKod;
                    tfd.harcamaKod = form.harcamaKod;
                    tfd.ambarKod = form.ambarKod;
                    tfd.hesapPlanKod = detay.hesapPlanKod;
                    tfd.hesapPlanAd = detay.hesapPlanAd;
                    tfd.olcuBirimAd = detay.olcuBirimAd;
                    tfd.siraNo = siraNo++;

                    if (detay.fazlaMiktar > 0)
                        tfd.miktar = detay.fazlaMiktar;
                    else if (detay.noksanMiktar > 0)
                        tfd.miktar = detay.noksanMiktar;

                    //tfd.gorSicilNo = detay.gorSicilNo;
                    //tfd.kdvOran = detay.kdvOran;
                    //tfd.birimFiyat = detay.birimFiyat;

                    form.detay.objeler.Add(tfd);
                }
                Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, form);

                if (sonuc.islemSonuc)
                {
                    GenelIslemler.MesajKutusu("Bilgi", "Taþýnýr Ýþlem Fiþi baþarýyla oluþturuldu.<br><br>Belge Numarasý: <a href='../TasinirMal/TasinirislemFormAna.aspx?yil=" + form.yil + "&muhasebe=" + form.muhasebeKod + "&harcamaBirimi=" + form.harcamaKod + "&belgeNo=" + sonuc.anahtar.Split('-')[0] + "' target='_blank'>" + sonuc.anahtar.Split('-')[0] + "</a>");
                }
                else
                {
                    GenelIslemler.MesajKutusu("Hata", "Taþýnýr Ýþlem Fiþi oluþturma sýrasýnda hata oluþtu.<br>Hata: " + sonuc.hataStr);
                }
            }
            else
            {
                string aciklama = Resources.TasinirMal.FRMSYG003;
                if (noksanMi)
                    aciklama = Resources.TasinirMal.FRMSYG004;

                GenelIslemler.MesajKutusu("Hata", aciklama);
            }
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden sayým tutanaðý bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Sayým tutanaðý bilgileri döndürülür.</returns>
        private SayimForm KriterTopla()
        {
            SayimForm sf = new SayimForm();
            sf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Value, 0);
            sf.muhasebeKod = txtMuhasebe.Text.Trim();
            sf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            sf.ambarKod = txtAmbar.Text.Trim();
            sf.sayimNo = (OrtakFonksiyonlar.ConvertToInt(txtBelgeNo.Text, 0) > 0 ? OrtakFonksiyonlar.ConvertToInt(txtBelgeNo.Text, 0).ToString() : "");
            sf.sayimTarih = new TNSDateTime(txtSayimTarihi.SelectedDate);
            return sf;
        }

        /// <summary>
        /// Sayfadaki buton kontrollerini aktif hale getiren yordam
        /// </summary>
        private void ButonlariAktifYap()
        {
            //btnSayimTutanak.Disabled = false;
            //btnAmbarDevirTutanak.Disabled = false;
            //btnTIFNoksan.Disabled = false;
            //btnTIFFazla.Disabled = false;
        }

        /// <summary>
        /// Listeleme kriterleri sayým form nesnesinde parametre olarak alýnýr,
        /// sunucuya gönderilir ve sayým tutanaðý bilgileri sunucudan alýnýr.
        /// Hata varsa ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sf">Sayým tutanaðý listeleme kriterlerini tutan nesne</param>
        private void Listele(SayimForm sf)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            if (string.IsNullOrEmpty(sf.sayimNo))
            {
                GenelIslemler.MesajKutusu("Uyarý", "Sayým Numarasý boþ býrakýlamaz.");
                return;
            }

            ObjectArray bilgi = servisTMM.SayimListele(kullanan, sf, true);

            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listelenecek kayýt bulunamadý." + bilgi.sonuc.hataStr);
                GridTemizle();
                return;
            }

            ButonlariAktifYap();
            DataTable dt = GridDataTable();

            SayimForm sform = (SayimForm)bilgi.objeler[0];
            txtYil.Value = sform.yil.ToString();
            txtMuhasebe.Text = sform.muhasebeKod;
            lblMuhasebeAd.Text = sform.muhasebeAd;
            txtHarcamaBirimi.Text = sform.harcamaKod;
            lblHarcamaBirimiAd.Text = sform.harcamaAd;
            txtAmbar.Text = sform.ambarKod;
            lblAmbarAd.Text = sform.ambarAd;
            txtSayimTarihi.Value = sform.sayimTarih.ToString();

            foreach (SayimDetay sd in sform.detay)
            {
                DataRow row = dt.NewRow();
                row["hesapPlanKod"] = sd.hesapPlanKod;
                row["hesapPlanAd"] = sd.hesapPlanAd;
                row["olcuBirimAd"] = sd.olcuBirimAd;
                row["ambarMiktar"] = sd.ambarMiktar;
                row["ortakMiktar"] = sd.ortakMiktar;
                row["kayitKisiMiktar"] = sd.kayitKisiMiktar;
                row["aciklama"] = sd.aciklama;

                dt.Rows.Add(row);
            }

            strListe.DataSource = dt;
            strListe.DataBind();
        }

        protected void HesapStore_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Parameters["query"]))
                return;

            List<object> liste = HesapListesiDoldur(e.Parameters["query"]);

            e.Total = 0;
            if (liste != null && liste.Count != 0)
            {
                var limit = e.Limit;
                if ((e.Start + e.Limit) > liste.Count)
                    limit = liste.Count - e.Start;

                e.Total = liste.Count;
                List<object> rangeData = (e.Start < 0 || limit < 0) ? liste : liste.GetRange(e.Start, limit);
                strHesapPlan.DataSource = (object[])rangeData.ToArray();
                strHesapPlan.DataBind();
            }
            else
            {
                strHesapPlan.DataSource = new object[] { };
                strHesapPlan.DataBind();
            }
        }

        List<object> HesapListesiDoldur(string kriter)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.hesapKodAciklama = kriter;
            h.detay = true;
            ObjectArray hesap = servisTMM.HesapPlaniListele(kullanan, h, new Sayfalama());

            List<object> liste = new List<object>();
            foreach (HesapPlaniSatir detay in hesap.objeler)
            {
                liste.Add(new
                {
                    KOD = detay.hesapKod + " - " + detay.aciklama + " - " + detay.olcuBirimAd
                });
            }
            return liste;
        }

        private void GridTemizle(int satirSayisi = 30)
        {
            DataTable dt = GridDataTable();
            for (int i = 0; i < satirSayisi; i++)
                dt.Rows.Add(dt.NewRow());

            strListe.DataSource = dt;
            strListe.DataBind();
        }

        private DataTable GridDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[] {
                    new DataColumn("hesapPlanKod")   { DataType = typeof(string) },
                    new DataColumn("hesapPlanAd")   { DataType = typeof(string) },
                    new DataColumn("olcuBirimAd")   { DataType = typeof(string) },
                    new DataColumn("ambarMiktar")   { DataType = typeof(decimal) },
                    new DataColumn("ortakMiktar")   { DataType = typeof(decimal) },
                    new DataColumn("kayitKisiMiktar")   { DataType = typeof(decimal) },
                    new DataColumn("aciklama")   { DataType = typeof(string) }
                });

            return dt;
        }
    }
}