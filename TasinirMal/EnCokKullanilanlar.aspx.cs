using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// En çok kullanılan taşınır malzeme bilgilerinin raporlama işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class EnCokKullanilanlar : TMMSayfaV2
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ulaşmak için kullanılan değişken
        /// </summary>
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        ///     Sayfa ilk defa çağırılıyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMECK001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
                IlDoldur();
                DonemDoldur();
            }
        }

        private void IlDoldur()
        {
            ObjectArray bilgi = servisTMM.IlListele(kullanan, new Il());

            List<object> liste = new List<object>();
            foreach (Il ilimiz in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ilimiz.kod,
                    ADI = ilimiz.ad
                });
            }

            strIl.DataSource = liste;
            strIl.DataBind();
        }

        protected void IlceDoldur(object sender, StoreRefreshDataEventArgs e)
        {
            IlceDoldur(ddlIl.Value != null ? ddlIl.Value.ToString() : string.Empty);
        }

        private void IlceDoldur(string ilKod)
        {
            if (ilKod == "")
                return;

            ddlIlce.Items.Clear();

            Ilce ilce = new Ilce();
            ilce.ilKodu = ilKod;

            ObjectArray bilgi = servisTMM.IlceListele(kullanan, ilce);

            List<object> liste = new List<object>();
            foreach (Ilce ilcemiz in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ilcemiz.kod,
                    ADI = ilcemiz.ad
                });
            }
            strIlce.DataSource = liste;
            strIlce.DataBind();
        }

        /// <summary>
        /// Dönem bilgileri ddlDonem DropDownList kontrolüne doldurur.
        /// </summary>
        private void DonemDoldur()
        {
            List<object> liste = new List<object>();

            for (int i = 1; i <= 4; i++)
                liste.Add(new { KOD = i.ToString(), ADI = string.Format(Resources.TasinirMal.FRMECK002, i.ToString()) });

            strDonem.DataSource = liste;
            strDonem.DataBind();
        }

        /// <summary>
        /// Yazdır tuşuna basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çağırılır ve toplanan kriterler
        /// en çok kullanılan taşınırlar raporunu üreten EnCokKullanilanlarRaporu yordamına
        /// gönderilir, böylece excel raporu üretilip kullanıcıya gönderilmiş olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            EnCokKullanilanlarRaporu(KriterTopla());
        }

        /// <summary>
        /// Detay Yazdır tuşuna basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çağırılır ve toplanan kriterler en çok
        /// kullanılan taşınırlar detay raporunu üreten EnCokKullanilanlarRaporuDetay yordamına
        /// gönderilir, böylece excel raporu üretilip kullanıcıya gönderilmiş olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDetayYazdir_Click(object sender, EventArgs e)
        {
            EnCokKullanilanlarRaporuDetay(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki kontrollerden depo durum kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Depo durum kriter bilgileri döndürülür.</returns>
        private DepoDurum KriterTopla()
        {
            DepoDurum kriter = new DepoDurum();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.muhasebeAd = lblMuhasebeAd.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kriter.harcamaAd = lblHarcamaBirimiAd.Text.Trim();
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.ambarAd = lblAmbarAd.Text.Trim();
            kriter.ilKod = TasinirGenel.ComboDegerDondur(ddlIl);
            kriter.ilAd = TasinirGenel.ComboAdDondur(ddlIl);
            kriter.ilceKod = TasinirGenel.ComboDegerDondur(ddlIlce);
            kriter.ilceAd = TasinirGenel.ComboAdDondur(ddlIlce);
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.hesapPlanAd = lblHesapPlanAd.Text.Trim();
            kriter.donem = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlDonem), 0);

            //Detay raporu için
            if (rdMuhasebeBazinda.Checked)
                kriter.raporTur = 0;
            if (rdHarcamaBazinda.Checked)
                kriter.raporTur = 1;
            if (rdIlBazinda.Checked)
                kriter.raporTur = 2;
            kriter.yilDevri = true;

            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen depo durum kriterlerini sunucudaki taşınır depo
        /// durum yordamına gönderir, sunucudan gelen en çok kullanılan taşınırlar
        /// detay raporu bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void EnCokKullanilanlarRaporuDetay(DepoDurum kriter)
        {
            ObjectArray bilgi = servisTMM.TasinirDepoDurumu(kullanan, kriter);
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }
            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            ObjectArray personelSayilari = servisTMM.PersonelSayilari(kullanan, kriter);

            //ObjectArray personelSayilariMuhasebe = null;
            //DepoDurum kriterMuhasebe = null;
            //if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
            //{
            //    kriterMuhasebe = KriterTopla();
            //    kriterMuhasebe.raporTur = (int)ENUMDepoDurumRaporTur.MUHASEBE;
            //    personelSayilariMuhasebe = servisTMM.PersonelSayilari(kullanan, kriterMuhasebe);
            //}

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "EnCokKullanilanlar.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.SutunGizle(sutun + 9, sutun + 11, true);

            XLS.HucreAdBulYaz("Yil", kriter.yil);
            XLS.HucreAdBulYaz("Muhasebe", kriter.muhasebeKod + " - " + kriter.muhasebeAd);

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                XLS.HucreAdBulYaz("Harcama", kriter.harcamaKod + " - " + kriter.harcamaAd);
            else
                XLS.HucreAdBulYaz("Harcama", GenelIslemlerIstemci.VarsayilanKurumBulAd());

            XLS.HucreAdBulYaz("Ambar", kriter.ambarKod + " - " + kriter.ambarAd);
            XLS.HucreAdBulYaz("Il", kriter.ilKod + " - " + kriter.ilAd);
            XLS.HucreAdBulYaz("Ilce", kriter.ilceKod + " - " + kriter.ilceAd);
            XLS.HucreAdBulYaz("Tasinir", kriter.hesapPlanKod + " - " + kriter.hesapPlanAd);

            satir = kaynakSatir;

            if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
            {
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun, Resources.TasinirMal.FRMECK003);
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 1, Resources.TasinirMal.FRMECK004);
            }
            else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
            {
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun, Resources.TasinirMal.FRMECK005);
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 1, Resources.TasinirMal.FRMECK006);
            }
            else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
            {
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun, Resources.TasinirMal.FRMECK007);
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 1, Resources.TasinirMal.FRMECK008);
            }

            int personelSayisi = 0;
            decimal[] toplam = new decimal[7];
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                DepoDurum depo = (DepoDurum)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);

                if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                {
                    personelSayisi = PersonelSayisiDondur(personelSayilari.objeler, kriter.raporTur, depo.muhasebeKod);

                    XLS.HucreDegerYaz(satir, sutun, depo.muhasebeKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.muhasebeAd);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                {
                    //if (depo.harcamaKod.Replace(".", "").Contains("12010004"))
                    //    personelSayisi = PersonelSayisiDondur(personelSayilariMuhasebe.objeler, kriterMuhasebe.raporTur, depo.muhasebeKod);
                    //else
                    personelSayisi = PersonelSayisiDondur(personelSayilari.objeler, kriter.raporTur, depo.muhasebeKod + "-" + depo.harcamaKod.Replace(".", ""));

                    XLS.HucreDegerYaz(satir, sutun, depo.muhasebeKod + " - " + depo.harcamaKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.muhasebeAd + " - " + depo.harcamaAd);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                {
                    personelSayisi = PersonelSayisiDondur(personelSayilari.objeler, kriter.raporTur, depo.ilKod);

                    XLS.HucreDegerYaz(satir, sutun, depo.ilKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.ilAd);
                }

                XLS.HucreDegerYaz(satir, sutun + 2, depo.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(depo.cikanMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(depo.cikanTutar.ToString(), (double)0));

                depo.kalanMiktar = depo.girenMiktar - depo.cikanMiktar;
                depo.kalanTutar = depo.girenTutar - depo.cikanTutar;

                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(depo.kalanMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(depo.kalanTutar.ToString(), (double)0));

                if (personelSayisi > 0)
                {
                    decimal sutun14 = depo.cikanMiktar / personelSayisi;
                    XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDouble(sutun14.ToString(), (double)0));

                    decimal sutun15 = depo.cikanTutar / personelSayisi;
                    XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDouble(sutun15.ToString(), (double)0));
                }

                toplam[0] += depo.girenMiktar;
                toplam[1] += depo.girenTutar;
                toplam[2] += depo.cikanMiktar;
                toplam[3] += depo.cikanTutar;
                toplam[4] += depo.kalanMiktar;
                toplam[5] += depo.kalanTutar;
                toplam[6] += (decimal)personelSayisi;
            }

            /***Genel Toplam***
            *******************/
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 15, true);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);

            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK009);
            XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(toplam[0].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(toplam[1].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(toplam[2].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(toplam[3].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(toplam[4].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(toplam[5].ToString(), (double)0));

            if (toplam[6] > 0)
            {
                XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDouble((toplam[2] / toplam[6]).ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDouble((toplam[3] / toplam[6]).ToString(), (double)0));
            }
            /*******************/

            for (int i = kaynakSatir + 1; i <= satir; i++)
            {
                if (i != satir)
                {
                    decimal sutun12 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / toplam[1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(sutun12.ToString(), (double)0));

                    decimal sutun13 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / toplam[1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(sutun13.ToString(), (double)0));
                }
                else
                {
                    XLS.HucreDegerYaz(i, sutun + 12, Resources.TasinirMal.FRMECK010);
                    XLS.HucreDegerYaz(i, sutun + 13, Resources.TasinirMal.FRMECK010);
                }
            }

            AciklamaYaz(XLS, satir + 3, sutun);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen personel sayı bilgileri arasından verilen
        /// rapor türüne ve koda ait personel sayısını bulup döndüren yordam
        /// </summary>
        /// <param name="objeler">Personel sayı bilgileri listesini tutan nesne</param>
        /// <param name="raporTur">ENUMDepoDurumRaporTur listesindeki değerlerden biri olmalıdır.</param>
        /// <param name="kod">Personel sayısı aranan kod kriteri</param>
        /// <returns>Personel sayısı döndürülür.</returns>
        private int PersonelSayisiDondur(TNSCollection objeler, int raporTur, string kod)
        {
            foreach (string[] ps in objeler)
            {
                if ((raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || raporTur == (int)ENUMDepoDurumRaporTur.IL) && ps[0] == kod)
                    return OrtakFonksiyonlar.ConvertToInt(ps[1], 0);
                else if (raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA && ps[0] + "-" + ps[1].Replace(".", "") == kod)
                    return OrtakFonksiyonlar.ConvertToInt(ps[2], 0);
            }
            return 0;
        }

        /// <summary>
        /// Excel raporuna açıklama bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">Açıklama bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Açıklama bilgilerinin yazılacağı sütun numarası</param>
        private void AciklamaYaz(Tablo XLS, int satir, int sutun)
        {
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 15);
            XLS.HucreBirlestir(satir + 1, sutun, satir + 1, sutun + 15);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK011);
            XLS.HucreDegerYaz(satir + 1, sutun, Resources.TasinirMal.FRMECK012);
        }

        /// <summary>
        /// Parametre olarak verilen depo durum kriterlerini sunucudaki en çok kullanılan taşınırlar
        /// yordamına gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void EnCokKullanilanlarRaporu(DepoDurum kriter)
        {
            ObjectArray bilgi = servisTMM.EnCokKullanilanTasinirlar(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
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
            string sablonAd = "EnCokKullanilanlar.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", kriter.yil);
            XLS.HucreAdBulYaz("Muhasebe", kriter.muhasebeKod + " - " + kriter.muhasebeAd);

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                XLS.HucreAdBulYaz("Harcama", kriter.harcamaKod + " - " + kriter.harcamaAd);
            else
                XLS.HucreAdBulYaz("Harcama", GenelIslemlerIstemci.VarsayilanKurumBulAd());

            XLS.HucreAdBulYaz("Ambar", kriter.ambarKod + " - " + kriter.ambarAd);
            XLS.HucreAdBulYaz("Il", kriter.ilKod + " - " + kriter.ilAd);
            XLS.HucreAdBulYaz("Ilce", kriter.ilceKod + " - " + kriter.ilceAd);
            XLS.HucreAdBulYaz("Tasinir", kriter.hesapPlanKod + " - " + kriter.hesapPlanAd);

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                kosul = kriter.muhasebeKod;
            if (!string.IsNullOrEmpty(kriter.harcamaKod))
            {
                if (!string.IsNullOrEmpty(kosul))
                    kosul += "-";
                kosul += kriter.harcamaKod.Replace(".", "");
            }

            int personelSayisi = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(kullanan, "TASPERSONELSAYISI", kosul, true).ToString(), 0);
            XLS.HucreAdBulYaz("PersonelSayisi", personelSayisi);
            XLS.HucreAdBulYaz("Donem", kriter.donem);

            satir = kaynakSatir;

            decimal[,] toplam = new decimal[4, 9];
            int toplamSatir = 0;
            int index = 0;
            int sayac = 0;
            string yazilanHesapKod = string.Empty;
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                DepoDurum depo = (DepoDurum)bilgi.objeler[i];

                if (depo.girenMiktar == 0 && depo.girenTutar == 0)
                    continue;

                if (sayac < 20 && (yazilanHesapKod == depo.hesapPlanKod.Substring(0, 3) || string.IsNullOrEmpty(yazilanHesapKod)))
                {
                    if (string.IsNullOrEmpty(yazilanHesapKod))
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);
                    }

                    yazilanHesapKod = depo.hesapPlanKod.Substring(0, 3);
                    sayac++;

                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, depo.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, depo.olcuBirimAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(depo.cikanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(depo.cikanTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(depo.kalanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(depo.kalanTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(depo.gecenYilTuketimMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(depo.gecenYilTuketimTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(depo.donemselTuketimTutar.ToString(), (double)0));

                    toplam[index, 0] += depo.girenMiktar;
                    toplam[index, 1] += depo.girenTutar;
                    toplam[index, 2] += depo.cikanMiktar;
                    toplam[index, 3] += depo.cikanTutar;
                    toplam[index, 4] += depo.kalanMiktar;
                    toplam[index, 5] += depo.kalanTutar;
                    toplam[index, 6] += depo.gecenYilTuketimMiktar;
                    toplam[index, 7] += depo.gecenYilTuketimTutar;
                    toplam[index, 8] += depo.donemselTuketimTutar;
                }
                else if (yazilanHesapKod != depo.hesapPlanKod.Substring(0, 3))
                {
                    toplamSatir = satir - sayac;
                    ToplamYaz(XLS, kaynakSatir, toplamSatir, sutun, yazilanHesapKod, toplam, index);

                    yazilanHesapKod = depo.hesapPlanKod.Substring(0, 3);
                    sayac = 0;
                    i--;//Atlanan bir satır var, onu yazdırabilmek için

                    if (TNS.TMM.Arac.MakineCihazMi(yazilanHesapKod))
                        index = 1;
                    else if (TNS.TMM.Arac.TasitMi(yazilanHesapKod))
                        index = 2;
                    else if (TNS.TMM.Arac.DemirbasMi(yazilanHesapKod))
                        index = 3;

                    yazilanHesapKod = string.Empty;
                }
            }

            toplamSatir = satir - sayac;
            ToplamYaz(XLS, kaynakSatir, toplamSatir, sutun, yazilanHesapKod, toplam, index);

            /***Genel Toplam***
            *******************/
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 15, true);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);

            decimal[] genelToplam = new decimal[9];
            for (int j = 0; j < toplam.Length / 9; j++)
                for (int i = 0; i < genelToplam.Length; i++)
                    genelToplam[i] += toplam[j, i];

            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK009);
            XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(genelToplam[0].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(genelToplam[1].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(genelToplam[2].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(genelToplam[3].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(genelToplam[4].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(genelToplam[5].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(genelToplam[6].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(genelToplam[7].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(genelToplam[8].ToString(), (double)0));
            /*******************/

            for (int i = kaynakSatir + 1; i <= satir; i++)
            {
                string hesapKod = XLS.HucreDegerAl(i, sutun).Substring(0, 3);
                if (hesapKod == ((int)ENUMTasinirHesapKodu.TUKETIM).ToString())
                    index = 0;
                else if (TNS.TMM.Arac.MakineCihazMi(hesapKod))
                    index = 1;
                else if (TNS.TMM.Arac.TasitMi(hesapKod))
                    index = 2;
                else if (TNS.TMM.Arac.DemirbasMi(hesapKod))
                    index = 3;

                if (i != satir)
                {
                    decimal sutun12 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / genelToplam[1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(sutun12.ToString(), (double)0));

                    decimal sutun13 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / toplam[index, 1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(sutun13.ToString(), (double)0));
                }
                else
                {
                    XLS.HucreDegerYaz(i, sutun + 12, Resources.TasinirMal.FRMECK010);
                    XLS.HucreDegerYaz(i, sutun + 13, Resources.TasinirMal.FRMECK010);
                }

                if (personelSayisi > 0)
                {
                    decimal sutun14 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 5, 4).Trim(), (decimal)0) / personelSayisi;
                    XLS.HucreDegerYaz(i, sutun + 14, OrtakFonksiyonlar.ConvertToDouble(sutun14.ToString(), (double)0));

                    decimal sutun15 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 6, 4).Trim(), (decimal)0) / personelSayisi;
                    XLS.HucreDegerYaz(i, sutun + 15, OrtakFonksiyonlar.ConvertToDouble(sutun15.ToString(), (double)0));
                }
            }

            AciklamaYaz(XLS, satir + 3, sutun);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Excel raporuna en çok kullanılan taşınırlara ait toplam bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun başladığı kaynak satır numarası</param>
        /// <param name="toplamSatir">Toplam bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Toplam bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        /// <param name="yazilanHesapKod">Toplamı yazılacak taşınır hesap planı kodu</param>
        /// <param name="toplam">En çok kullanılan taşınırlara ait toplam bilgilerini tutan dizi</param>
        /// <param name="index">En çok kullanılan taşınırlara ait toplam bilgisinin dizideki indeksi</param>
        private void ToplamYaz(Tablo XLS, int kaynakSatir, int toplamSatir, int sutun, string yazilanHesapKod, decimal[,] toplam, int index)
        {
            XLS.KoyuYap(toplamSatir, sutun, toplamSatir, sutun + 15, true);

            XLS.HucreDegerYaz(toplamSatir, sutun, yazilanHesapKod);
            XLS.HucreDegerYaz(toplamSatir, sutun + 1, servisUZY.UzayDegeriStr(kullanan, "TASHESAPPLAN", yazilanHesapKod, true));
            //XLS.HucreDegerYaz(toplamSatir, sutun + 2, depo.olcuBirimAd);
            XLS.HucreDegerYaz(toplamSatir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 0].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 1].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 2].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 3].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 4].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 5].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 6].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 7].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 8].ToString(), (double)0));
        }
    }
}