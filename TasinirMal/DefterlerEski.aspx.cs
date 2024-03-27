using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr malzeme defterlerinin raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class DefterlerEski : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde gelen tur girdi dizgisi hangi defterin istendiðine iþaret eder,
        ///     bu parametreye bakýlarak gizlenecek/gösterilecek kontroller ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            if (Request.QueryString["tur"] == "1")
                formAdi = Resources.TasinirMal.FRMDFT001;
            else if (Request.QueryString["tur"] == "2")
                formAdi = Resources.TasinirMal.FRMDFT002;
            else if (Request.QueryString["tur"] == "3")
                formAdi = Resources.TasinirMal.FRMDFT003;
            else if (Request.QueryString["tur"] == "4")
            {
                divIlkKayit.Attributes.Remove("style");
                divKayitSayisi.Attributes.Remove("style");
                divKitapAd.Attributes.Remove("style");
                divYazarAd.Attributes.Remove("style");
                divYer.Attributes.Remove("style");
                formAdi = Resources.TasinirMal.FRMDFT004;
            }

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                YilDoldur();
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtAmbar.Text.Trim() != "")
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = "";

            if (txtHesapPlanKod.Text.Trim() != "")
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = "";
        }

        /// <summary>
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        /// <summary>
        /// Defter Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler sayfa
        /// adresinde gelen tur girdi dizgisine bakýlarak ilgili defteri üreten yordama
        /// gönderilir, böylece excel raporu üretilip kullanýcýya gönderilmiþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOlustur_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["tur"] == "1")
                TuketimMalzemeDefteri(KriterTopla());
            else if (Request.QueryString["tur"] == "2")
                DayanikliTasinirDefteri(KriterTopla());
            else if (Request.QueryString["tur"] == "3")
                MuzeDefteri(KriterTopla());
            else if (Request.QueryString["tur"] == "4")
                KutuphaneDefteri(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden defter kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Cetvel kriter bilgileri döndürülür.</returns>
        private DefterKriter KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            DefterKriter kriter = new DefterKriter();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.ilkKayit = OrtakFonksiyonlar.ConvertToInt(txtIlkKayit.Text.Trim(), 0);
            kriter.kayitSayisi = OrtakFonksiyonlar.ConvertToInt(txtKayitSayisi.Text.Trim(), 0);
            kriter.kitapAd = txtKitapAd.Text.Trim();
            kriter.yazarAd = txtYazarAd.Text.Trim();
            kriter.yer = txtYer.Text.Trim();
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen defter kriterlerini sunucudaki tüketim malzemeleri defteri yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="kriter">Defter kriter bilgilerini tutan nesne</param>
        private void TuketimMalzemeDefteri(DefterKriter kriter)
        {
            ObjectArray bilgi = servisTMM.TuketimMalzemeDefteriOlustur(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TUKETIMDEFTERI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);

            int satirEkle = kaynakSatir + 1;
            int aktifSayfa = 0;
            int sayfadakiSatirSayisi = 50000;

            //Boþ sayfa yarat ve 0. sayfayý þablon olarak kullan
            //*****************************************************
            XLS.YeniSheetEkle(0);
            aktifSayfa++;
            XLS.AktifSheetDegistir(aktifSayfa);
            //*****************************************************

            for (int k = 0; k < bilgi.objeler.Count; k++)
            {
                TMDBilgi tmd = (TMDBilgi)bilgi.objeler[k];

                //liste satýr sayýsýný aþýyor ise yeni sayfaya geç
                if (satirEkle + 1000 > sayfadakiSatirSayisi)
                {
                    satirEkle = kaynakSatir + 1;
                    XLS.YeniSheetEkle(0);
                    aktifSayfa++;
                    XLS.AktifSheetDegistir(aktifSayfa);
                }

                XLS.SatirAc(satirEkle, 9);
                XLS.HucreKopyala(0, kaynakSutun, 8, kaynakSutun + 15, satirEkle, kaynakSutun);

                XLS.HucreBirlestir(satirEkle, kaynakSutun, satirEkle, kaynakSutun + 15);
                XLS.HucreBirlestir(satirEkle + 1, kaynakSutun, satirEkle + 1, kaynakSutun + 1);
                XLS.HucreBirlestir(satirEkle + 1, kaynakSutun + 14, satirEkle + 1, kaynakSutun + 15);

                for (int birlestirSatir = satirEkle + 2; birlestirSatir < satirEkle + 6; birlestirSatir++)
                {
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun, birlestirSatir, kaynakSutun + 1);
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 5, birlestirSatir, kaynakSutun + 6);
                }

                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 8, satirEkle + 5, kaynakSutun + 8);
                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 9, satirEkle + 3, kaynakSutun + 9);
                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 10, satirEkle + 3, kaynakSutun + 15);

                for (int birlestirSatir = satirEkle + 4; birlestirSatir < satirEkle + 6; birlestirSatir++)
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 10, birlestirSatir, kaynakSutun + 15);

                for (int birlestirSutun = kaynakSutun; birlestirSutun <= kaynakSutun + 8; birlestirSutun += 8)
                {
                    XLS.HucreBirlestir(satirEkle + 7, birlestirSutun, satirEkle + 8, birlestirSutun);
                    XLS.HucreBirlestir(satirEkle + 7, birlestirSutun + 1, satirEkle + 7, birlestirSutun + 2);
                }

                for (int birlestirSutun = kaynakSutun + 3; birlestirSutun <= kaynakSutun + 6; birlestirSutun++)
                    XLS.HucreBirlestir(satirEkle + 7, birlestirSutun, satirEkle + 8, birlestirSutun);

                for (int birlestirSutun = kaynakSutun + 11; birlestirSutun <= kaynakSutun + 15; birlestirSutun++)
                    XLS.HucreBirlestir(satirEkle + 7, birlestirSutun, satirEkle + 8, birlestirSutun);

                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 3, tmd.ilAd + "-" + tmd.ilceAd);
                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 5, tmd.ilKod + "-" + tmd.ilceKod);
                XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 3, tmd.harcamaAd);
                XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 5, tmd.harcamaKod);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 3, tmd.ambarAd);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 5, tmd.ambarKod);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 3, tmd.muhasebeAd);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 5, tmd.muhasebeKod);
                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 10, tmd.hesapAd);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 10, tmd.hesapKod);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 10, tmd.olcuBirimAd);

                satirEkle += 9;
                int eskiSatirEkle = satirEkle;
                int satir = satirEkle;

                for (int i = 0; i < tmd.detaylar.Count; i++)
                {
                    int sutun = 0;

                    TMDBilgiDetay detay = (TMDBilgiDetay)tmd.detaylar[i];
                    if (detay.islemTipi.tur >= 1 && detay.islemTipi.tur <= 49)
                    {
                        sutun = kaynakSutun;

                        XLS.SatirAc(satirEkle, 1);
                        XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 15, satirEkle, kaynakSutun);
                        satirEkle++;

                        XLS.HucreDegerYaz(satir, sutun, detay.siraNo);
                        XLS.HucreDegerYaz(satir, sutun + 1, detay.tifTarih.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, detay.tifNo);
                        XLS.HucreDegerYaz(satir, sutun + 3, detay.islemTipi.ad);
                        XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(detay.miktar.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.tutar.ToString(), (double)0));

                        int sayacCikis = -1;
                        decimal toplam = 0;
                        for (int j = i + 1; j < tmd.detaylar.Count; j++)
                        {
                            TMDBilgiDetay detayCikis = (TMDBilgiDetay)tmd.detaylar[j];
                            if ((detayCikis.islemTipi.tur > 49 || detayCikis.islemTipi.tur == -1) &&
                                detay.birimFiyatKDVLi.ToString("#,###.##") == detayCikis.birimFiyatKDVLi.ToString("#,###.##") &&
                                toplam < detay.miktar &&
                                detayCikis.miktar <= detay.miktar)
                            {
                                sayacCikis++;
                                sutun = kaynakSutun + 8;
                                toplam += detayCikis.miktar;

                                if (sayacCikis != 0)
                                {
                                    XLS.SatirAc(satirEkle, 1);
                                    XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 15, satirEkle, kaynakSutun);
                                    satirEkle++;
                                    satir++;
                                }

                                XLS.HucreDegerYaz(satir, sutun, detayCikis.siraNo);
                                XLS.HucreDegerYaz(satir, sutun + 1, detayCikis.tifTarih.ToString());
                                XLS.HucreDegerYaz(satir, sutun + 2, detayCikis.tifNo);
                                XLS.HucreDegerYaz(satir, sutun + 3, detayCikis.islemTipi.ad);

                                if (!string.IsNullOrEmpty(detayCikis.kimeGitti))
                                {
                                    string ad = uzyServis.UzayDegeriStr(kullanan, "TASPERSONEL", detayCikis.kimeGitti, true, "");
                                    if (string.IsNullOrEmpty(ad))
                                        XLS.HucreDegerYaz(satir, sutun + 4, detayCikis.kimeGitti);
                                    else
                                        XLS.HucreDegerYaz(satir, sutun + 4, ad);
                                }
                                else if (!string.IsNullOrEmpty(detayCikis.nereyeGitti))
                                    XLS.HucreDegerYaz(satir, sutun + 4, detayCikis.nereyeGitti);
                                else if (!string.IsNullOrEmpty(detayCikis.gonHarcamaAd))
                                    XLS.HucreDegerYaz(satir, sutun + 4, detayCikis.gonHarcamaAd + "-" + detayCikis.gonAmbarAd);

                                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(detayCikis.miktar.ToString(), (double)0));
                                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detayCikis.birimFiyatKDVLi.ToString(), (double)0));
                                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(detayCikis.tutar.ToString(), (double)0));

                                tmd.detaylar.RemoveAt(j);
                                j--;
                            }
                        }

                        tmd.detaylar.RemoveAt(i);
                        i--;

                        satir++;
                    }
                }

                XLS.SayfaSonuKoyHucresel(satirEkle + 4);
                satirEkle += 4;
                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satirEkle, GenelIslemler.JexcelBirimtoExcelBirim(400));
            }

            //Þablon olrak kullanýlan ilk sayfayý sil
            XLS.SheetSil(0);
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen defter kriterlerini sunucudaki dayanýklý taþýnýr defteri yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="kriter">Defter kriter bilgilerini tutan nesne</param>
        private void DayanikliTasinirDefteri(DefterKriter kriter)
        {
            ObjectArray bilgi = servisTMM.DayanikliTasinirlarDefteriOlustur(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "DAYANIKLIDEFTERI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);

            int satirEkle = kaynakSatir + 1;
            int aktifSayfa = 0;
            int sayfadakiSatirSayisi = 50000;

            //Boþ sayfa yarat ve 0. sayfayý þablon olarak kullan
            //*****************************************************
            XLS.YeniSheetEkle(0);
            aktifSayfa++;
            XLS.AktifSheetDegistir(aktifSayfa);
            //*****************************************************

            for (int k = 0; k < bilgi.objeler.Count; k++)
            {
                DTDBilgi dtd = (DTDBilgi)bilgi.objeler[k];

                //liste satýr sayýsýný aþýyor ise yeni sayfaya geç
                if (satirEkle + 1000 > sayfadakiSatirSayisi)
                {
                    satirEkle = kaynakSatir + 1;
                    XLS.YeniSheetEkle(0);
                    aktifSayfa++;
                    XLS.AktifSheetDegistir(aktifSayfa);
                }

                XLS.SatirAc(satirEkle, 9);
                XLS.HucreKopyala(0, kaynakSutun, 8, kaynakSutun + 19, satirEkle, kaynakSutun);
                XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 6, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.SatirYukseklikAyarla(satirEkle + 7, satirEkle + 8, GenelIslemler.JexcelBirimtoExcelBirim(600));

                XLS.HucreBirlestir(satirEkle, kaynakSutun, satirEkle, kaynakSutun + 19);
                XLS.HucreBirlestir(satirEkle + 1, kaynakSutun, satirEkle + 1, kaynakSutun + 1);
                XLS.HucreBirlestir(satirEkle + 1, kaynakSutun + 18, satirEkle + 1, kaynakSutun + 19);

                for (int birlestirSatir = satirEkle + 2; birlestirSatir < satirEkle + 6; birlestirSatir++)
                {
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun, birlestirSatir, kaynakSutun + 2);
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 4, birlestirSatir, kaynakSutun + 6);
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 8, birlestirSatir, kaynakSutun + 9);
                }

                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 11, satirEkle + 5, kaynakSutun + 11);
                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 12, satirEkle + 3, kaynakSutun + 13);
                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 14, satirEkle + 3, kaynakSutun + 19);

                for (int birlestirSatir = satirEkle + 4; birlestirSatir < satirEkle + 6; birlestirSatir++)
                {
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 12, birlestirSatir, kaynakSutun + 13);
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 14, birlestirSatir, kaynakSutun + 19);
                }

                XLS.HucreBirlestir(satirEkle + 7, kaynakSutun, satirEkle + 8, kaynakSutun);
                XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 1, satirEkle + 7, kaynakSutun + 2);

                for (int birlestirSutun = kaynakSutun + 3; birlestirSutun <= kaynakSutun + 9; birlestirSutun++)
                    XLS.HucreBirlestir(satirEkle + 7, birlestirSutun, satirEkle + 8, birlestirSutun);

                XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 11, satirEkle + 7, kaynakSutun + 13);

                for (int birlestirSutun = kaynakSutun + 14; birlestirSutun <= kaynakSutun + 19; birlestirSutun++)
                    XLS.HucreBirlestir(satirEkle + 7, birlestirSutun, satirEkle + 8, birlestirSutun);

                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 4, dtd.ilAd + "-" + dtd.ilceAd);
                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 8, dtd.ilKod + "-" + dtd.ilceKod);
                XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 4, dtd.harcamaAd);
                XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 8, dtd.harcamaKod);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 4, dtd.ambarAd);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 8, dtd.ambarKod);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 4, dtd.muhasebeAd);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 8, dtd.muhasebeKod);
                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 14, dtd.hesapAd);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 14, dtd.hesapKod);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 14, dtd.olcuBirimAd);

                satirEkle += 9;
                int eskiSatirEkle = satirEkle;
                int satir = satirEkle;

                int siraNo = 0;
                int oncekiIslemTipTur = -1;
                for (int i = 0; i < dtd.detaylar.Count; i++)
                {
                    int sutun = 0;

                    DTDBilgiDetay detay = (DTDBilgiDetay)dtd.detaylar[i];

                    bool satirEklemeliMi = true;
                    if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur <= (int)ENUMIslemTipi.DEGERARTTIR)
                        && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
                    {
                        siraNo++;
                        sutun = kaynakSutun;
                        satirEklemeliMi = true;
                    }
                    else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                    {
                        sutun = kaynakSutun + 11;
                        if (oncekiIslemTipTur >= (int)ENUMIslemTipi.SATINALMAGIRIS && oncekiIslemTipTur <= (int)ENUMIslemTipi.ACILIS
                            && oncekiIslemTipTur != (int)ENUMIslemTipi.ZFDUSME && oncekiIslemTipTur != (int)ENUMIslemTipi.DTLDUSME)
                            satirEklemeliMi = false;
                        else
                            satirEklemeliMi = true;
                    }

                    oncekiIslemTipTur = detay.islemTipi.tur;

                    //Yeni satýr gerekli mi 
                    //bool satirEklemeliMi = true;
                    //for (int j = eskiSatirEkle; j < satirEkle; j++)
                    //    if (string.IsNullOrEmpty(XLS.HucreDegerAl(j, sutun).Trim()))
                    //        satirEklemeliMi = false;

                    if (satirEklemeliMi)
                    {
                        XLS.SatirAc(satirEkle, 1);
                        XLS.SatirYukseklikAyarla(satirEkle, satirEkle, GenelIslemler.JexcelBirimtoExcelBirim(400));
                        XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 19, satirEkle, kaynakSutun);
                        satirEkle++;
                    }
                    else
                        satir--;

                    if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur == (int)ENUMIslemTipi.DEGERARTTIR)
                        && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
                    {
                        XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
                        XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilNo);
                        XLS.HucreDegerYaz(satir, sutun + 4, detay.islemTipi.ad);

                        if (!string.IsNullOrEmpty(detay.neredenGeldi))
                            XLS.HucreDegerYaz(satir, sutun + 5, detay.neredenGeldi);
                        else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
                            XLS.HucreDegerYaz(satir, sutun + 5, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);

                        XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(detay.miktar.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(detay.degerArtisi.ToString(), (double)0));
                        //XLS.HucreDegerYaz(satir, sutun + 8, detay.miktar);
                        XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(detay.toplamDeger.ToString(), (double)0));

                        XLS.HucreDegerYaz(satir, sutun + 18, detay.ozellikler);

                    }
                    else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                    {
                        XLS.HucreDegerYaz(satir, sutun, detay.cikisCinsi);
                        XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
                        XLS.HucreDegerYaz(satir, sutun + 3, detay.islemTipi.ad);

                        if (!string.IsNullOrEmpty(detay.kimeVerildi))
                        {
                            string ad = uzyServis.UzayDegeriStr(kullanan, "TASPERSONEL", detay.kimeVerildi, true, "");
                            if (string.IsNullOrEmpty(ad))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.kimeVerildi);
                            else
                                XLS.HucreDegerYaz(satir, sutun + 4, ad);
                        }

                        if (detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                        {
                            string odaAd = uzyServis.UzayDegeriStr(null, "TASODA", dtd.muhasebeKod + "-" + dtd.harcamaKod + "-" + detay.nereyeVerildi, true, "");
                            if (!string.IsNullOrEmpty(odaAd))
                                XLS.HucreDegerYaz(satir, sutun + 5, detay.nereyeVerildi + "-" + odaAd);
                            else
                                XLS.HucreDegerYaz(satir, sutun + 5, detay.nereyeVerildi);
                            XLS.HucreDegerYaz(satir, sutun + 7, detay.ozellikler);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(detay.nereyeGonderildi))
                                XLS.HucreDegerYaz(satir, sutun + 6, detay.nereyeGonderildi);
                            else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
                                XLS.HucreDegerYaz(satir, sutun + 6, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);

                            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(detay.toplamDeger.ToString(), (double)0));
                        }
                    }
                    satir++;
                }

                XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 4, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.SayfaSonuKoyHucresel(satirEkle + 4);
                satirEkle += 4;
            }

            //Þablon olrak kullanýlan ilk sayfayý sil
            XLS.SheetSil(0);
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen defter kriterlerini sunucudaki müze defteri yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="kriter">Defter kriter bilgilerini tutan nesne</param>
        private void MuzeDefteri(DefterKriter kriter)
        {
            ObjectArray bilgi = servisTMM.MuzeDefteriOlustur(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "MUZEDEFTERI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);

            int satirEkle = kaynakSatir + 1;

            for (int k = 0; k < bilgi.objeler.Count; k++)
            {
                MuzeBilgi muze = (MuzeBilgi)bilgi.objeler[k];

                XLS.SatirAc(satirEkle, 10);
                XLS.HucreKopyala(0, kaynakSutun, 9, kaynakSutun + 23, satirEkle, kaynakSutun);
                XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 6, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.SatirYukseklikAyarla(satirEkle + 7, satirEkle + 9, GenelIslemler.JexcelBirimtoExcelBirim(780));

                XLS.HucreBirlestir(satirEkle, kaynakSutun, satirEkle, kaynakSutun + 23);

                for (int birlestirSatir = satirEkle + 2; birlestirSatir < satirEkle + 6; birlestirSatir++)
                {
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun, birlestirSatir, kaynakSutun + 4);
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 6, birlestirSatir, kaynakSutun + 12);
                    XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 14, birlestirSatir, kaynakSutun + 17);
                }

                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 19, satirEkle + 5, kaynakSutun + 19);
                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 20, satirEkle + 3, kaynakSutun + 20);
                XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 21, satirEkle + 3, kaynakSutun + 23);
                XLS.HucreBirlestir(satirEkle + 4, kaynakSutun + 21, satirEkle + 4, kaynakSutun + 23);
                XLS.HucreBirlestir(satirEkle + 5, kaynakSutun + 21, satirEkle + 5, kaynakSutun + 23);

                XLS.HucreBirlestir(satirEkle + 7, kaynakSutun, satirEkle + 9, kaynakSutun);
                XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 1, satirEkle + 8, kaynakSutun + 2);
                XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 3, satirEkle + 7, kaynakSutun + 17);

                for (int birlestirSutun = kaynakSutun + 3; birlestirSutun <= kaynakSutun + 17; birlestirSutun++)
                    XLS.HucreBirlestir(satirEkle + 8, birlestirSutun, satirEkle + 9, birlestirSutun);

                XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 19, satirEkle + 7, kaynakSutun + 23);
                XLS.HucreBirlestir(satirEkle + 8, kaynakSutun + 19, satirEkle + 8, kaynakSutun + 21);
                XLS.HucreBirlestir(satirEkle + 8, kaynakSutun + 22, satirEkle + 9, kaynakSutun + 22);
                XLS.HucreBirlestir(satirEkle + 8, kaynakSutun + 23, satirEkle + 9, kaynakSutun + 23);

                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 6, muze.ilAd + "-" + muze.ilceAd);
                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 14, muze.ilKod + "-" + muze.ilceKod);
                XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 6, muze.harcamaAd);
                XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 14, muze.harcamaKod);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 6, muze.ambarAd);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 14, muze.ambarKod);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 6, muze.muhasebeAd);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 14, muze.muhasebeKod);

                XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 21, muze.hesapAd);
                XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 21, muze.hesapKod);
                XLS.HucreDegerYaz(satirEkle + 5, kaynakSutun + 21, muze.olcuBirimAd);

                satirEkle += 10;
                int eskiSatirEkle = satirEkle;
                int satir = satirEkle;

                int siraNo = 0;
                int oncekiIslemTipTur = -1;
                for (int i = 0; i < muze.detaylar.Count; i++)
                {
                    int sutun = 0;

                    MuzeBilgiDetay detay = (MuzeBilgiDetay)muze.detaylar[i];

                    bool satirEklemeliMi = true;
                    if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur <= (int)ENUMIslemTipi.DEGERARTTIR)
                        && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
                    {
                        siraNo++;
                        sutun = kaynakSutun;
                        satirEklemeliMi = true;
                    }
                    else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                    {
                        sutun = kaynakSutun + 19;
                        if (oncekiIslemTipTur >= (int)ENUMIslemTipi.SATINALMAGIRIS && oncekiIslemTipTur <= (int)ENUMIslemTipi.ACILIS
                            && oncekiIslemTipTur != (int)ENUMIslemTipi.ZFDUSME && oncekiIslemTipTur != (int)ENUMIslemTipi.DTLDUSME)
                            satirEklemeliMi = false;
                        else
                            satirEklemeliMi = true;
                    }

                    oncekiIslemTipTur = detay.islemTipi.tur;

                    //Yeni satýr gerekli mi 
                    //bool satirEklemeliMi = true;
                    //for (int j = eskiSatirEkle; j < satirEkle; j++)
                    //    if (string.IsNullOrEmpty(XLS.HucreDegerAl(j, sutun).Trim()))
                    //        satirEklemeliMi = false;

                    if (satirEklemeliMi)
                    {
                        XLS.SatirAc(satirEkle, 1);
                        XLS.SatirYukseklikAyarla(satirEkle, satirEkle, GenelIslemler.JexcelBirimtoExcelBirim(400));
                        XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 23, satirEkle, kaynakSutun);
                        satirEkle++;
                    }
                    else
                        satir--;

                    if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur == (int)ENUMIslemTipi.DEGERARTTIR)
                        && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
                    {
                        XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
                        XLS.HucreDegerYaz(satir, sutun + 3, detay.islemTipi.ad);
                        XLS.HucreDegerYaz(satir, sutun + 4, detay.sicilNo);
                        XLS.HucreDegerYaz(satir, sutun + 5, detay.adi);
                        XLS.HucreDegerYaz(satir, sutun + 6, detay.gelisTarihi);

                        if (!string.IsNullOrEmpty(detay.neredenGeldi))
                            XLS.HucreDegerYaz(satir, sutun + 7, detay.neredenGeldi);
                        else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
                            XLS.HucreDegerYaz(satir, sutun + 7, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);

                        XLS.HucreDegerYaz(satir, sutun + 8, detay.neredeBulundu);
                        XLS.HucreDegerYaz(satir, sutun + 9, detay.cagi);
                        XLS.HucreDegerYaz(satir, sutun + 10, detay.boyutlari);
                        XLS.HucreDegerYaz(satir, sutun + 11, detay.durumuMaddesi);
                        XLS.HucreDegerYaz(satir, sutun + 12, detay.onYuz);
                        XLS.HucreDegerYaz(satir, sutun + 13, detay.arkaYuz);
                        XLS.HucreDegerYaz(satir, sutun + 14, detay.fotograf);
                        XLS.HucreDegerYaz(satir, sutun + 15, detay.puan);
                        XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 17, detay.yeriKonusu);
                    }
                    else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                    {
                        XLS.HucreDegerYaz(satir, sutun, detay.cikisCinsi);
                        XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
                        XLS.HucreDegerYaz(satir, sutun + 3, detay.islemTipi.ad);

                        if (!string.IsNullOrEmpty(detay.kimeVerildi))
                        {
                            string ad = uzyServis.UzayDegeriStr(kullanan, "TASPERSONEL", detay.kimeVerildi, true, "");
                            if (string.IsNullOrEmpty(ad))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.kimeVerildi);
                            else
                                XLS.HucreDegerYaz(satir, sutun + 4, ad);
                        }

                        if (detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                        {
                            string odaAd = uzyServis.UzayDegeriStr(null, "TASODA", muze.muhasebeKod + "-" + muze.harcamaKod + "-" + detay.nereyeVerildi, true, "");
                            if (!string.IsNullOrEmpty(odaAd))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.nereyeVerildi + "-" + odaAd);
                            else
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.nereyeVerildi);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(detay.nereyeGonderildi))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.nereyeGonderildi);
                            else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);
                        }
                    }
                    satir++;
                }

                XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 4, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.SayfaSonuKoyHucresel(satirEkle + 4);
                satirEkle += 4;
            }
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen defter kriterlerini sunucudaki kütüphane defteri yordamýna
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasýna yazar ve kullanýcýya gönderir.
        /// </summary>
        /// <param name="kriter">Defter kriter bilgilerini tutan nesne</param>
        private void KutuphaneDefteri(DefterKriter kriter)
        {
            ObjectArray bilgi = servisTMM.KutuphaneDefteriOlustur(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "KUTUPHANEDEFTERI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);
            int satirEkle = kaynakSatir + 1;

            //Kaç sayfa olacaðýna karar ver
            //*******************************************
            int orjinalkaynakSatir = kaynakSatir;
            int orjinalkaynakSutun = kaynakSutun;
            int orjinalsatirEkle = satirEkle;

            int aktifSayfa = 0;
            int genelSatirSat = 0;

            int sayfadakiSatirSayisi = 50000;
            int toplamSatir = 0;
            for (int k = 0; k < bilgi.objeler.Count; k++)
            {
                KutuphaneBilgi kutuphane = (KutuphaneBilgi)bilgi.objeler[k];
                int oncekiIslemTipTur = 0;
                //Satýr sayýsýna karar verir. Ayný iþlem aþaðýda yazma sýrasýnda da var.
                for (int i = 0; i < kutuphane.detaylar.Count; i++)
                {
                    //Sadece giriþ kayýtlarý için satýr açýlýyor. zimmet ve çýkýþlar için deðil
                    KutuphaneBilgiDetay detay = (KutuphaneBilgiDetay)kutuphane.detaylar[i];
                    if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur <= (int)ENUMIslemTipi.DEGERARTTIR)
                        && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
                        toplamSatir++;//Yeni giriþ Satýr ekle
                    else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                    {
                        if (!(oncekiIslemTipTur >= (int)ENUMIslemTipi.SATINALMAGIRIS && oncekiIslemTipTur <= (int)ENUMIslemTipi.ACILIS
                            && oncekiIslemTipTur != (int)ENUMIslemTipi.ZFDUSME && oncekiIslemTipTur != (int)ENUMIslemTipi.DTLDUSME))
                            toplamSatir++;//Önceki iþlem den dolayý Satýr ekle
                    }
                    oncekiIslemTipTur = detay.islemTipi.tur;
                }
            }

            if (toplamSatir > sayfadakiSatirSayisi)
            {
                decimal sayfaSayisi = Math.Round((decimal)toplamSatir / (decimal)sayfadakiSatirSayisi);

                for (int i = 0; i < OrtakFonksiyonlar.ConvertToInt(sayfaSayisi, 0); i++)
                    XLS.YeniSheetEkle(0);

                XLS.AktifSheetDegistir(aktifSayfa);
            }
            //*******************************************

            for (int k = 0; k < bilgi.objeler.Count; k++)
            {
                KutuphaneBilgi kutuphane = (KutuphaneBilgi)bilgi.objeler[k];

                KutuphaneBaslikYaz(kutuphane, XLS, satirEkle, kaynakSutun);

                satirEkle += 10;
                int eskiSatirEkle = satirEkle;
                int satir = satirEkle;

                int siraNo = 0;
                int oncekiIslemTipTur = -1;
                for (int i = 0; i < kutuphane.detaylar.Count; i++)
                {
                    int sutun = 0;

                    KutuphaneBilgiDetay detay = (KutuphaneBilgiDetay)kutuphane.detaylar[i];

                    bool satirEklemeliMi = true;
                    if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS || detay.islemTipi.tur <= (int)ENUMIslemTipi.DEGERARTTIR)
                        && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
                    {
                        siraNo++;
                        sutun = kaynakSutun;
                        satirEklemeliMi = true;
                    }
                    else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                    {
                        sutun = kaynakSutun + 21;
                        if (oncekiIslemTipTur >= (int)ENUMIslemTipi.SATINALMAGIRIS && oncekiIslemTipTur <= (int)ENUMIslemTipi.ACILIS
                            && oncekiIslemTipTur != (int)ENUMIslemTipi.ZFDUSME && oncekiIslemTipTur != (int)ENUMIslemTipi.DTLDUSME)
                            satirEklemeliMi = false;
                        else
                            satirEklemeliMi = true;
                    }

                    oncekiIslemTipTur = detay.islemTipi.tur;

                    //Yeni satýr gerekli mi 
                    //bool satirEklemeliMi = true;
                    //for (int j = eskiSatirEkle; j < satirEkle; j++)
                    //    if (string.IsNullOrEmpty(XLS.HucreDegerAl(j, sutun).Trim()))
                    //        satirEklemeliMi = false;

                    if (satirEklemeliMi)
                    {
                        //Eðer sayfadaki satir sayýsýný aþarsa yeni sayfaya geç
                        //********************************************************
                        genelSatirSat++;
                        if (genelSatirSat > sayfadakiSatirSayisi)
                        {
                            genelSatirSat = 1;
                            aktifSayfa++;
                            XLS.AktifSheetDegistir(aktifSayfa);
                            kaynakSatir = orjinalkaynakSatir;
                            kaynakSutun = orjinalkaynakSutun;
                            satirEkle = orjinalsatirEkle;

                            KutuphaneBaslikYaz(kutuphane, XLS, satirEkle, kaynakSutun);

                            satirEkle += 10;
                            eskiSatirEkle = satirEkle;
                            satir = satirEkle;
                        }
                        //********************************************************

                        XLS.SatirAc(satirEkle, 1);
                        XLS.SatirYukseklikAyarla(satirEkle, satirEkle, GenelIslemler.JexcelBirimtoExcelBirim(400));
                        XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 25, satirEkle, kaynakSutun);
                        satirEkle++;
                    }
                    else
                        satir--;

                    if ((detay.islemTipi.tur >= (int)ENUMIslemTipi.SATINALMAGIRIS && detay.islemTipi.tur <= (int)ENUMIslemTipi.ACILIS && detay.islemTipi.tur != (int)ENUMIslemTipi.ZFDUSME && detay.islemTipi.tur != (int)ENUMIslemTipi.DTLDUSME)
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DEGERARTTIR)
                    {
                        XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
                        XLS.HucreDegerYaz(satir, sutun + 3, detay.islemTipi.ad);
                        XLS.HucreDegerYaz(satir, sutun + 4, detay.sicilNo);
                        XLS.HucreDegerYaz(satir, sutun + 5, detay.ciltNo);
                        XLS.HucreDegerYaz(satir, sutun + 6, detay.dil);
                        XLS.HucreDegerYaz(satir, sutun + 7, detay.yazarAdi);
                        XLS.HucreDegerYaz(satir, sutun + 8, detay.adi);
                        XLS.HucreDegerYaz(satir, sutun + 9, detay.yayinYeri);
                        XLS.HucreDegerYaz(satir, sutun + 10, detay.yayinTarihi);

                        if (!string.IsNullOrEmpty(detay.neredenGeldi))
                            XLS.HucreDegerYaz(satir, sutun + 11, detay.neredenGeldi);
                        else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
                            XLS.HucreDegerYaz(satir, sutun + 11, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);

                        XLS.HucreDegerYaz(satir, sutun + 12, detay.boyutlari);
                        XLS.HucreDegerYaz(satir, sutun + 13, detay.satirSayisi);
                        XLS.HucreDegerYaz(satir, sutun + 14, detay.yaprakSayisi);
                        XLS.HucreDegerYaz(satir, sutun + 15, detay.sayfaSayisi);
                        XLS.HucreDegerYaz(satir, sutun + 16, detay.ciltTuru);
                        XLS.HucreDegerYaz(satir, sutun + 17, detay.fotograf);
                        XLS.HucreDegerYaz(satir, sutun + 18, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 19, detay.yeriKonusu);
                    }
                    else if (detay.islemTipi.tur > (int)ENUMIslemTipi.ACILIS
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                    {
                        XLS.HucreDegerYaz(satir, sutun, detay.cikisCinsi);
                        XLS.HucreDegerYaz(satir, sutun + 1, detay.belgeTarih.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, detay.belgeNo);
                        XLS.HucreDegerYaz(satir, sutun + 3, detay.islemTipi.ad);

                        if (!string.IsNullOrEmpty(detay.kimeVerildi))
                        {
                            string ad = uzyServis.UzayDegeriStr(kullanan, "TASPERSONEL", detay.kimeVerildi, true, "");
                            if (string.IsNullOrEmpty(ad))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.kimeVerildi);
                            else
                                XLS.HucreDegerYaz(satir, sutun + 4, ad);
                        }

                        if (detay.islemTipi.tur == (int)ENUMIslemTipi.ZFDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.ZFVERME
                        || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLDUSME || detay.islemTipi.tur == (int)ENUMIslemTipi.DTLVERME)
                        {
                            string odaAd = uzyServis.UzayDegeriStr(null, "TASODA", kutuphane.muhasebeKod + "-" + kutuphane.harcamaKod + "-" + detay.nereyeVerildi, true, "");
                            if (!string.IsNullOrEmpty(odaAd))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.nereyeVerildi + "-" + odaAd);
                            else
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.nereyeVerildi);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(detay.nereyeGonderildi))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.nereyeGonderildi);
                            else if (!string.IsNullOrEmpty(detay.gonHarcamaAd))
                                XLS.HucreDegerYaz(satir, sutun + 4, detay.gonHarcamaAd + "-" + detay.gonAmbarAd);
                        }
                    }
                    satir++;
                }

                XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 4, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.SayfaSonuKoyHucresel(satirEkle + 4);
                satirEkle += 4;
            }
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Kütüphane defteri excel raporunun baþlýk bilgilerini yazan ve formatlayan yordam
        /// </summary>
        /// <param name="kutuphane">Kütüphane defterinin üst bilgilerini tutan nesne</param>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satirEkle">Baþlýk bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="kaynakSutun">Baþlýk bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        private void KutuphaneBaslikYaz(KutuphaneBilgi kutuphane, Tablo XLS, int satirEkle, int kaynakSutun)
        {
            XLS.SatirAc(satirEkle, 10);
            XLS.HucreKopyala(0, kaynakSutun, 9, kaynakSutun + 25, satirEkle, kaynakSutun);
            XLS.SatirYukseklikAyarla(satirEkle, satirEkle + 8, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.SatirYukseklikAyarla(satirEkle + 9, satirEkle + 9, GenelIslemler.JexcelBirimtoExcelBirim(2000));

            XLS.HucreBirlestir(satirEkle, kaynakSutun, satirEkle, kaynakSutun + 25);

            for (int birlestirSatir = satirEkle + 2; birlestirSatir < satirEkle + 6; birlestirSatir++)
            {
                XLS.HucreBirlestir(birlestirSatir, kaynakSutun, birlestirSatir, kaynakSutun + 6);
                XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 8, birlestirSatir, kaynakSutun + 16);
                XLS.HucreBirlestir(birlestirSatir, kaynakSutun + 18, birlestirSatir, kaynakSutun + 19);
            }

            XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 21, satirEkle + 2, kaynakSutun + 21);
            //XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 22, satirEkle + 3, kaynakSutun + 22);
            //XLS.HucreBirlestir(satirEkle + 2, kaynakSutun + 23, satirEkle + 3, kaynakSutun + 25);
            XLS.HucreBirlestir(satirEkle + 3, kaynakSutun + 23, satirEkle + 3, kaynakSutun + 25);
            XLS.HucreBirlestir(satirEkle + 4, kaynakSutun + 23, satirEkle + 4, kaynakSutun + 25);

            XLS.HucreBirlestir(satirEkle + 7, kaynakSutun, satirEkle + 9, kaynakSutun);
            XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 1, satirEkle + 8, kaynakSutun + 2);
            XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 3, satirEkle + 7, kaynakSutun + 19);

            for (int birlestirSutun = kaynakSutun + 3; birlestirSutun <= kaynakSutun + 19; birlestirSutun++)
            {
                if (birlestirSutun == 9)
                {
                    XLS.HucreBirlestir(satirEkle + 8, birlestirSutun, satirEkle + 8, birlestirSutun + 1);
                    birlestirSutun++;
                }
                else if (birlestirSutun == 12)
                {
                    XLS.HucreBirlestir(satirEkle + 8, birlestirSutun, satirEkle + 8, birlestirSutun + 4);
                    birlestirSutun += 4;
                }
                else
                    XLS.HucreBirlestir(satirEkle + 8, birlestirSutun, satirEkle + 9, birlestirSutun);
            }

            XLS.HucreBirlestir(satirEkle + 7, kaynakSutun + 21, satirEkle + 7, kaynakSutun + 25);
            XLS.HucreBirlestir(satirEkle + 8, kaynakSutun + 21, satirEkle + 8, kaynakSutun + 23);
            XLS.HucreBirlestir(satirEkle + 8, kaynakSutun + 24, satirEkle + 9, kaynakSutun + 24);
            XLS.HucreBirlestir(satirEkle + 8, kaynakSutun + 25, satirEkle + 9, kaynakSutun + 25);

            XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 8, kutuphane.ilAd + "-" + kutuphane.ilceAd);
            XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 18, kutuphane.ilKod + "-" + kutuphane.ilceKod);
            XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 8, kutuphane.harcamaAd);
            XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 18, kutuphane.harcamaKod);
            //XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 8, kutuphane.ambarAd);
            //XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 18, kutuphane.ambarKod);
            XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 8, kutuphane.muhasebeAd);
            XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 18, kutuphane.muhasebeKod);

            XLS.HucreDegerYaz(satirEkle + 2, kaynakSutun + 23, kutuphane.hesapAd);
            XLS.HucreDegerYaz(satirEkle + 3, kaynakSutun + 23, kutuphane.hesapKod);
            XLS.HucreDegerYaz(satirEkle + 4, kaynakSutun + 23, kutuphane.olcuBirimAd);
        }
    }
}