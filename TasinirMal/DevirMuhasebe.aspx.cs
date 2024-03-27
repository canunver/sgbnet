using Ext1.Net;
using OrtakClass;
using System;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r t�ketim malzemelerinin ��k�� ve fark bilgilerinin raporlama i�leminin yap�ld��� sayfa
    /// </summary>
    public partial class DevirMuhasebe : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ile ilgili ayarlamalar yap�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Devir Giri�/��k�� Muhasebe Fi�i Olu�turma";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                SayfaUstAltBolumYaz(this);

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam �a��r�l�r ve toplanan kriterler
        /// sayfa adresinde gelen fark girdi dizgisine bak�larak TuketimCikisFarkYazdir
        /// yordam�na veya TuketimCikisYazdir yordam�na g�nderilir ve rapor haz�rlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOlustur_Click(object sender, EventArgs e)
        {
            BelgeOlustur(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden kriter bilgilerini toplayan ve d�nd�ren yordam
        /// </summary>
        /// <returns>T�ketim ��k�� kriter bilgileri d�nd�r�l�r.</returns>
        private TNS.TMM.DevirCikis KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            TNS.TMM.DevirCikis kriter = new TNS.TMM.DevirCikis();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.tarih1 = new TNSDateTime(txtTarih1.RawText);
            kriter.tarih2 = new TNSDateTime(txtTarih2.RawText);
            kriter.miktar = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlTur), 0);
            kriter.gMuhasebeKod = TasinirGenel.ComboAdDondur(ddlTur);
            if (kriter.miktar == 500)
                kriter.miktar = (int)ENUMIslemTipi.DEVIRCIKIS;

            return kriter;
        }

        private void BelgeOlustur(TNS.TMM.DevirCikis kriter)
        {
            string hata = "";
            if (kriter.muhasebeKod.Trim() == "")
                hata += "Muhasebe bilgisi bo� b�rak�lamaz<br>";
            //if (kriter.harcamaKod.Trim() == "")
            //    hata += "Harcama Birim bilgisi bo� b�rak�lamaz<br>";
            if (kriter.tarih1.isNull || kriter.tarih2.isNull)
                hata += "Tarih bilgisi bo� b�rak�lamaz<br>";
            if (kriter.miktar == 0)
                hata += "��lem t�r� bo� b�rak�lamaz<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyar�", hata);
                return;
            }

            kriter.hesapPlanKod = "-999";
            ObjectArray bilgi = servisTMM.DevirCikis(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyar�", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            //string birimKod = "";
            //if (kriter.harcamaKod.Trim() == "")
            //    birimKod = "000";
            //else
            //    birimKod = kriter.harcamaKod.Trim().Substring(kriter.harcamaKod.Trim().Length - 3);

            string kurKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIKURUMSALKOD") + "";
            string fonKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIFONKSIYONELKOD") + "";
            string finKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIFINANSKOD") + "";
            string pebKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIPEBKOD") + "";
            string tertip = kurKod + "-" + fonKod + "-" + finKod + "-{EKOKOD}";
            if (pebKod != "")
                tertip += "-" + pebKod;
            if (kurKod == "") tertip = "";

            TNS.MUH.OdemeEmriMIFForm oe = new TNS.MUH.OdemeEmriMIFForm();
            oe.yil = DateTime.Now.Year;
            oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
            oe.muhasebe = OrtakFonksiyonlar.ConvertToInt(kriter.muhasebeKod, 0);
            oe.birim = "000";
            oe.belgeNo = txtBelgeNo.Text.ToString();
            oe.fisTurYap(TNS.MUH.ENUMFISTUR.DONEMICI);
            oe.islemTarih = new TNSDateTime(DateTime.Now);

            string hesapTur = TasinirGenel.ComboDegerDondur(ddlHesap);

            if (kriter.miktar == (int)ENUMIslemTipi.DEVIRCIKIS)
                oe.aciklama = kriter.tarih1.ToString() + "/" + kriter.tarih2.ToString() + " aras� devir giri�/��k��� yap�lan Malzemelerinin muhasebe fi�i";
            else
                oe.aciklama = kriter.tarih1.ToString() + "/" + kriter.tarih2.ToString() + " aras� " + kriter.gMuhasebeKod + " i�lemi yap�lan Malzemelerinin muhasebe fi�i";

            oe.detay = new ObjectArray();
            double borcToplam = 0;
            double alacakToplam = 0;
            foreach (DevirCikis tk in bilgi.objeler)
            {
                string detayBirim = tk.harcamaKod.Trim().Substring(tk.harcamaKod.Trim().Length - 3);
                string gdetayBirim = "";
                if (!string.IsNullOrWhiteSpace(tk.gHarcamaKod))
                    gdetayBirim = tk.gHarcamaKod.Trim().Substring(tk.gHarcamaKod.Trim().Length - 3);

                if (hesapTur != "" && !tk.hesapPlanKod.StartsWith(hesapTur))
                    continue;
                if (kriter.miktar == (int)ENUMIslemTipi.DEVIRCIKIS)
                {
                    TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = tk.hesapPlanKod;
                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = 0;
                    od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.satirAciklama = "Fi� No:" + tk.fisNo;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = tk.hesapPlanKod;
                    od.detayBirim = gdetayBirim + "." + kurKod;
                    od.borc = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.alacak = 0;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);
                }
                else if (kriter.miktar == (int)ENUMIslemTipi.ACILIS ||
                    kriter.miktar == (int)ENUMIslemTipi.BAGISGIRIS ||
                    kriter.miktar == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS ||
                    kriter.miktar == (int)ENUMIslemTipi.IADEGIRIS ||
                    kriter.miktar == (int)ENUMIslemTipi.DEVIRGIRISKURUM)
                {
                    TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = tk.hesapPlanKod;
                    od.detayBirim = detayBirim + "." + kurKod; ;
                    od.borc = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.alacak = 0;
                    od.satirAciklama = "Fi� No:" + tk.fisNo;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = "5000203";
                    if (tk.hesapPlanKod.StartsWith("150")) od.hesapKod = "5000206";
                    if (tk.hesapPlanKod.StartsWith("253")) od.hesapKod += "04";
                    if (tk.hesapPlanKod.StartsWith("254")) od.hesapKod += "05";
                    if (tk.hesapPlanKod.StartsWith("255")) od.hesapKod += "06";

                    od.detayBirim = detayBirim + "." + kurKod; ;
                    od.borc = 0;
                    od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);
                }
                else if (kriter.miktar == (int)ENUMIslemTipi.URETILENGIRIS)
                {
                    TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = tk.hesapPlanKod;
                    od.detayBirim = detayBirim + "." + kurKod; ;
                    od.borc = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.alacak = 0;
                    od.satirAciklama = "GFi� No:" + tk.fisNo;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    borcToplam += od.borc;
                    oe.detay.objeler.Add(od);

                    //�retim fi�inden kar�� �al��an T�F'i bul ve i�indeki malzemeleri belgeye ekle
                    UretimFormu utk = new UretimFormu();
                    utk.sorguYil = tk.yil;
                    utk.girenMuhasebe.kod = tk.muhasebeKod;
                    utk.girenHarcamaBirimi.kod = tk.harcamaKod;
                    utk.girenFisNo = tk.fisNo;
                    ObjectArray utkListe = servisTMM.UretimFormuListele(kullanan, utk);
                    utk = (UretimFormu)utkListe.objeler[0];

                    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
                    tf.yil = utk.fisTarihi.Yil;
                    tf.muhasebeKod = utk.cikanMuhasebe.kod;
                    tf.harcamaKod = utk.cikanHarcamaBirimi.kod;
                    tf.fisNo = utk.cikanFisNo;
                    ObjectArray tfListe = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);
                    foreach (TNS.TMM.TasinirIslemForm tfitem in tfListe.objeler)
                    {
                        foreach (TNS.TMM.TasinirIslemDetay tditem in tfitem.detay.objeler)
                        {
                            od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                            od.hesapKod = tditem.hesapPlanKod.Replace(".", "").Substring(0, 7);

                            od.detayBirim = tfitem.harcamaKod.Trim().Substring(tfitem.harcamaKod.Trim().Length - 3);
                            od.detayBirim += "." + kurKod; ;
                            od.borc = 0;
                            od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tditem.miktar * tditem.birimFiyatKDVLi), 2);
                            if (tertip != "")
                                od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                            od.satirAciklama = "�Fi� No:" + tfitem.fisNo;

                            alacakToplam += od.alacak;
                            oe.detay.objeler.Add(od);
                        }
                    }
                }
                else if (kriter.miktar == (int)ENUMIslemTipi.BAGISCIKIS ||
                         kriter.miktar == (int)ENUMIslemTipi.SAYIMNOKSANICIKIS ||
                         kriter.miktar == (int)ENUMIslemTipi.DEVIRCIKISKURUM)
                {
                    TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = tk.hesapPlanKod;
                    od.detayBirim = detayBirim + "." + kurKod; ;
                    od.borc = 0;
                    od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.satirAciklama = "Fi� No:" + tk.fisNo;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = "5000203";
                    if (tk.hesapPlanKod.StartsWith("150")) od.hesapKod = "5000206";
                    if (tk.hesapPlanKod.StartsWith("253")) od.hesapKod += "04";
                    if (tk.hesapPlanKod.StartsWith("254")) od.hesapKod += "05";
                    if (tk.hesapPlanKod.StartsWith("255")) od.hesapKod += "06";

                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.alacak = 0;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);
                }
                else if (kriter.miktar == (int)ENUMIslemTipi.HURDACIKIS ||
                         kriter.miktar == (int)ENUMIslemTipi.KULLANILMAZCIKIS)
                {
                    TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = tk.hesapPlanKod;
                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = 0;
                    od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.satirAciklama = "Fi� No:" + tk.fisNo;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = "294";
                    if (tk.hesapPlanKod.StartsWith("150")) od.hesapKod += "01";
                    if (tk.hesapPlanKod.StartsWith("253")) od.hesapKod += "0201";
                    if (tk.hesapPlanKod.StartsWith("254")) od.hesapKod += "0202";
                    if (tk.hesapPlanKod.StartsWith("255")) od.hesapKod += "0203";

                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.alacak = 0;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);
                }
                else if (kriter.miktar == (int)ENUMIslemTipi.SATISCIKIS)
                {
                    TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = tk.hesapPlanKod;
                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = 0;
                    od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.satirAciklama = "Fi� No:" + tk.fisNo;
                    if (tertip != "")
                        od.TertipAta(tertip.Replace("{EKOKOD}", "032"));
                    oe.detay.objeler.Add(od);

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = "100.01";
                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.alacak = 0;
                    oe.detay.objeler.Add(od);

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = "800.03.04.01.99";
                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = 0;
                    od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    oe.detay.objeler.Add(od);

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = "805";
                    od.detayBirim = detayBirim + "." + kurKod;
                    od.borc = Math.Round(OrtakFonksiyonlar.ConvertToDbl(tk.tutarKDVli), 2);
                    od.alacak = 0;
                    oe.detay.objeler.Add(od);

                }
            }

            double kurusfarki = borcToplam - alacakToplam;
            if (kurusfarki != 0)
            {
                TNS.MUH.OdemeEmriMIFDetay od2 = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                od2.hesapKod = "630.11.01.05.01";
                od2.detayBirim = "";
                od2.borc = kurusfarki < 0 ? Math.Abs(kurusfarki) : 0;
                od2.alacak = kurusfarki > 0 ? Math.Abs(kurusfarki) : 0;
                oe.detay.objeler.Add(od2);
            }

            Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, oe);

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", "<a href='../ButceMuhasebe/OdemeEmriMIFFrame.aspx?yil=" + oe.yil + "&belgeNo=" + sonuc.anahtar + "' target='_blank'>" + sonuc.anahtar + "</a> nolu belge olu�turuldu");
            else
                GenelIslemler.MesajKutusu("Uyar�", sonuc.hataStr);
        }
    }
}