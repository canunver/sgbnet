using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr tüketim malzemelerinin çýkýþ ve fark bilgilerinin raporlama iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class TuketimCikisMuhasebe : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ile ilgili ayarlamalar yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTMC002;
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                SayfaUstAltBolumYaz(this);

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler
        /// sayfa adresinde gelen fark girdi dizgisine bakýlarak TuketimCikisFarkYazdir
        /// yordamýna veya TuketimCikisYazdir yordamýna gönderilir ve rapor hazýrlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOlustur_Click(object sender, EventArgs e)
        {
            BelgeOlustur(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Tüketim çýkýþ kriter bilgileri döndürülür.</returns>
        private TNS.TMM.TuketimCikis KriterTopla()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            TNS.TMM.TuketimCikis kriter = new TNS.TMM.TuketimCikis();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.tarih1 = new TNSDateTime(txtTarih1.RawText);
            kriter.tarih2 = new TNSDateTime(txtTarih2.RawText);
            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen tüketim çýkýþ kriterlerini sunucudaki TuketimCikis
        /// yordamýna gönderir, sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="kriter">Tüketim çýkýþ kriter bilgilerini tutan nesne</param>
        private void BelgeOlustur(TNS.TMM.TuketimCikis kriter)
        {
            string hata = "";
            if (kriter.muhasebeKod.Trim() == "")
                hata += "Muhasebe bilgisi boþ býrakýlamaz<br>";
            if (kriter.muhasebeKod.Trim() == "")
                hata += "Harcama Birim bilgisi boþ býrakýlamaz<br>";
            if (kriter.tarih1.isNull || kriter.tarih2.isNull)
                hata += "Tarih bilgisi boþ býrakýlamaz<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            ObjectArray bilgi = servisTMM.TuketimCikis(kullanan, kriter, false);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            TNS.TMM.TuketimCikis tk = (TNS.TMM.TuketimCikis)bilgi.objeler[0];

            string birimKod = tk.harcamaKod.Substring(tk.harcamaKod.Length - 3);
            Hashtable htGider = new Hashtable();

            string kurKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIKURUMSALKOD") + "";
            string fonKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIFONKSIYONELKOD") + "";
            string finKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIFINANSKOD") + "";
            string pebKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIPEBKOD") + "";
            string tertip = kurKod + "-" + fonKod + "-" + finKod + "-{EKOKOD}";
            if (pebKod != "")
                tertip += "-" + pebKod;

            TNS.MUH.OdemeEmriMIFForm oe = new TNS.MUH.OdemeEmriMIFForm();
            oe.yil = kriter.tarih1.Yil;
            oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
            oe.muhasebe = OrtakFonksiyonlar.ConvertToInt(tk.muhasebeKod, 0);
            oe.birim = birimKod;
            oe.belgeNo = txtBelgeNo.Text.ToString();
            oe.fisTurYap(TNS.MUH.ENUMFISTUR.DONEMICI);
            oe.islemTarih = new TNSDateTime(DateTime.Now);
            oe.aciklama = kriter.tarih1.ToString() + "/" + kriter.tarih2.ToString() + " arasý çýkýþý yapýlan Tüketim Malzemelerinin muhasebe fiþi";
            oe.detay = new ObjectArray();

            for (int i = 0; i < tk.detay.Count; i++)
            {
                TNS.TMM.TuketimCikisDetay detay = (TNS.TMM.TuketimCikisDetay)tk.detay[i];
                TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                od.hesapKod = detay.hesapPlanKod;
                od.borc = 0;
                od.alacak = Math.Round(OrtakFonksiyonlar.ConvertToDbl(detay.tutar[0]), 2);
                od.TertipAta(tertip.Replace("{EKOKOD}", "032"));

                string giderHK = "63014" + detay.hesapPlanKod.Replace(".", "").Substring(3, 2);

                htGider[giderHK] = Math.Round(TasinirGenel.HashtableDegerVerDbl(htGider, giderHK) + od.alacak, 2);

                bool bulundu = false;
                foreach (TNS.MUH.OdemeEmriMIFDetay odd in oe.detay.objeler)
                {
                    if (odd.hesapKod == od.hesapKod)
                    {
                        odd.alacak += Math.Round(od.alacak, 2);
                        bulundu = true;
                        break;
                    }
                }

                if (!bulundu)
                    oe.detay.objeler.Add(od);
            }

            foreach (DictionaryEntry entry in htGider)
            {
                string giderHK = (string)entry.Key;
                double giderTutar = Math.Round((double)entry.Value, 2);

                TNS.MUH.OdemeEmriMIFDetay od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                od.hesapKod = giderHK;
                od.borc = giderTutar;
                od.alacak = 0;
                od.TertipAta("");
                oe.detay.objeler.Add(od);
            }

            Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, oe);

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.anahtar + " nolu belge oluþturuldu");
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }
    }
}