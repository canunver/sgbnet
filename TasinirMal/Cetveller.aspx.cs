using System;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using TNS.UZY;
using System.Collections;
using System.Text;
using Ext1.Net;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Taşınır malzeme cetvellerinin raporlama işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class Cetveller : TMMSayfaV2
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        ///     Sayfa adresinde gelen tur girdi dizgisi hangi cetvelin istendiğine işaret eder,
        ///     bu parametreye bakılarak gizlenecek/gösterilecek kontroller ayarlanır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (Request.QueryString["tur"] == "1")
                {
                    formAdi = Resources.TasinirMal.FRMCTV001;
                    cmpMuhasebe.Visible = true;
                    cmpHarcamaBirimi.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                }
                else if (Request.QueryString["tur"] == "2")
                {
                    formAdi = Resources.TasinirMal.FRMCTV002;
                    cmpMuhasebe.Visible = true;
                    cmpHarcamaBirimi.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                    chkMuhasebeRapor.Visible = true;
                }
                else if (Request.QueryString["tur"] == "3")
                {
                    formAdi = Resources.TasinirMal.FRMCTV003;
                    ddlIl.Visible = true;
                    ddlIlce.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                }
                else if (Request.QueryString["tur"] == "4")
                {
                    formAdi = Resources.TasinirMal.FRMCTV004;
                    cmpMuhasebe.Visible = true;
                    cmpHarcamaBirimi.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                    chkCSV.Visible = true;
                }
                else if (Request.QueryString["tur"] == "5")
                {
                    formAdi = Resources.TasinirMal.FRMCTV005;
                    cmpMuhasebe.Visible = true;
                    cmpHarcamaBirimi.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                    ddlSeviye.Visible = true;
                    chkCSV.Visible = true;
                }
                else if (Request.QueryString["tur"] == "6")
                {
                    formAdi = Resources.TasinirMal.FRMCTV006;
                    ddlIl.Visible = true;
                    cmpHarcamaBirimi.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                }
                else if (Request.QueryString["tur"] == "7")
                {
                    formAdi = Resources.TasinirMal.FRMCTV007;
                    cmpMuhasebe.Visible = true;
                    cmpHarcamaBirimi.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                }
                else if (Request.QueryString["tur"] == "8")
                {
                    formAdi = Resources.TasinirMal.FRMCTV008;
                    cmpBolge.Visible = true;
                    cmpMuhasebe.Visible = true;
                    cmpHarcamaBirimi.Visible = true;
                    cmpHesapPlan.Visible = true;
                    txtTarih1.Visible = true;
                    txtTarih2.Visible = true;
                }

                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                hdnFirmaHarcamadanAlma.Value = TasinirGenel.tasinirFirmaBilgisiniHarcamadanAlma;

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                if (Request.QueryString["tur"] != "6")
                    txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                txtYil.Value = DateTime.Now.Year;
                IlDoldur();
            }
        }

        /// <summary>
        /// Yazdır tuşuna basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çağırılır ve toplanan kriterler sayfa
        /// adresinde gelen tur girdi dizgisine bakılarak ilgili cetveli üreten yordama
        /// gönderilir, böylece excel raporu üretilip kullanıcıya gönderilmiş olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["tur"] == "1")
                TasinirSayimCetveli(KriterTopla());
            else if (Request.QueryString["tur"] == "2")
                TasinirYonetimHesabiCetveli(KriterTopla());
            else if (Request.QueryString["tur"] == "3")
            {
                TasinirHesapCetveli(KriterTopla());
            }
            else if (Request.QueryString["tur"] == "4")
                TasinirKesinHesapCetveli(KriterTopla(), chkCSV.Checked);
            else if (Request.QueryString["tur"] == "5")
            {
                TasinirKesinHesapIcmalCetveli(KriterTopla(), chkCSV.Checked);
            }
            else if (Request.QueryString["tur"] == "6")
                TasinirHesapCetveliIl(KriterTopla());
            else if (Request.QueryString["tur"] == "7")
                MuzeKutuphaneCetveli(KriterTopla());
            else if (Request.QueryString["tur"] == "8")
                TasinirHesapCetveliBolge(KriterTopla());
        }

        /// <summary>
        /// Sayfa adresinde gelen tur girdi dizgisine bakılarak ilgili
        /// kontrollerden cetvel kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Cetvel kriter bilgileri döndürülür.</returns>
        private Cetvel KriterTopla()
        {
            Cetvel kriter = new Cetvel();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);

            if (Request.QueryString["tur"] == "1" || Request.QueryString["tur"] == "2")
            {
                kriter.muhasebeKod = txtMuhasebe.Text.Trim();
                kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            }
            else if (Request.QueryString["tur"] == "3")
            {
                kriter.ilKod = TasinirGenel.ComboDegerDondur(ddlIl);
                kriter.ilceKod = TasinirGenel.ComboDegerDondur(ddlIlce);
            }
            else if (Request.QueryString["tur"] == "6")
            {
                kriter.ilKod = TasinirGenel.ComboDegerDondur(ddlIl);
                //kriter.muhasebeKod = txtMuhasebe.Text.Trim();
                kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            }
            else if (Request.QueryString["tur"] == "8")
            {
                kriter.bolgeKod = OrtakFonksiyonlar.ConvertToInt(txtBolge.Text.Trim(), 0);
                kriter.muhasebeKod = txtMuhasebe.Text.Trim();
                kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            }
            else if (Request.QueryString["tur"] == "4" || Request.QueryString["tur"] == "5" || Request.QueryString["tur"] == "7")
            {
                kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
                kriter.seviye = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlSeviye), 0);
            }

            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.tarih1 = new TNSDateTime(txtTarih1.RawText);
            kriter.tarih2 = new TNSDateTime(txtTarih2.RawText);

            return kriter;
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki taşınır sayım ve döküm cetveli yordamına gönderir,
        /// sunucudan gelen bilgi kümesini excele yazdırmak üzere TasinirSayimVEYonetimHesabiCetveli yordamına gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        private void TasinirSayimCetveli(Cetvel kriter)
        {
            ObjectArray bilgi = servisTMM.TasinirSayimCetveli(kullanan, kriter);
            TasinirSayimVEYonetimHesabiCetveli(bilgi, "TASINIRSAYIMCETVELI.xlt");
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki harcama birimi taşınır yönetim hesabı cetveli yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excele yazdırmak üzere chkMuhasebeRapor kontrolü işaretliyse
        /// TasinirSayimVEYonetimHesabiCetveliMuhasebe, değilse TasinirSayimVEYonetimHesabiCetveli yordamına gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        private void TasinirYonetimHesabiCetveli(Cetvel kriter)
        {
            ObjectArray bilgi = servisTMM.TasinirYonetimHesabiCetveli(kullanan, kriter);

            if (chkMuhasebeRapor.Checked)
                TasinirSayimVEYonetimHesabiCetveliMuhasebe(bilgi, "TASINIRSAYIMCETVELIMUHASEBE.XLT", kriter.yil, kriter.muhasebeKod, kriter.harcamaKod, kriter);
            else
            {
                TasinirSayimVEYonetimHesabiCetveli(bilgi, "TASINIRYONETIMHESABICETVELI.xlt");
            }
        }

        /// <summary>
        /// Taşınır sayım ve döküm cetveli veya harcama birimi taşınır yönetim hesabı cetveli excel raporunu hazırlayan yordam
        /// sablonAd parametresine bakarak hangi cetveli üreteceğine karar verir.
        /// </summary>
        /// <param name="bilgi">İstenen cetvele ait bilgileri tutan nesne</param>
        /// <param name="sablonAd">Cetvelin oluşturulacağı şablonun adı</param>
        private void TasinirSayimVEYonetimHesabiCetveli(ObjectArray bilgi, string sablonAd)
        {
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
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            string eskiHesapKod = string.Empty;

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];
            CetvelDetay cd = new CetvelDetay();
            CetvelDetay genelToplam = new CetvelDetay();
            CetvelDetay hesapToplam = new CetvelDetay();

            int siraNo = 0;
            for (int i = 0; i < cetvel.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                siraNo++;

                if (!detay.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                {
                    siraNo = 1;

                    if (i != 0)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                        //Toplam yazıyor burada
                        DetayYaz(XLS, satir, sutun, 0, cd);
                        cd = new CetvelDetay();

                        if (eskiHesapKod.Substring(0, 3) != detay.hesapPlanKod.Substring(0, 3) && !string.IsNullOrEmpty(eskiHesapKod))
                        {
                            satir++;
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                            DetayYaz(XLS, satir, sutun, -1, hesapToplam);
                            hesapToplam = new CetvelDetay();
                        }

                        if (sablonAd == "TASINIRSAYIMCETVELI.xlt")
                        {
                            TasinirSayimCetveliImzaEkle(XLS, ref satir, sutun, eskiHesapKod);
                            OrnekNoYaz(XLS, ++satir, sutun, 13);
                        }
                        else
                        {
                            TasinirYonetimHesabiCetveliImzaEkle(XLS, ref satir, sutun);
                            OrnekNoYaz(XLS, ++satir, sutun, 14);
                        }

                        XLS.SayfaSonuKoyHucresel(satir + 3);
                        satir += 3;
                    }

                    eskiHesapKod = detay.hesapPlanKod.Substring(0, 6);

                    XLS.SatirAc(satir, 8);
                    XLS.HucreKopyala(kaynakSatir - 8, sutun, kaynakSatir - 1, sutun + 14, satir, sutun);

                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);

                    for (int j = satir + 2; j < satir + 5; j++)
                    {
                        XLS.HucreBirlestir(j, sutun, j, sutun + 1);
                        XLS.HucreBirlestir(j, sutun + 3, j, sutun + 8);
                        XLS.HucreBirlestir(j, sutun + 10, j, sutun + 11);
                    }

                    XLS.HucreBirlestir(satir + 2, sutun + 13, satir + 2, sutun + 14);

                    XLS.HucreBirlestir(satir + 3, sutun + 12, satir + 4, sutun + 12);
                    XLS.HucreBirlestir(satir + 3, sutun + 13, satir + 4, sutun + 14);

                    YTLYaz(XLS, satir + 1, sutun + 14);

                    XLS.HucreDegerYaz(satir + 2, sutun + 3, cetvel.ilAd + "-" + cetvel.ilceAd);
                    XLS.HucreDegerYaz(satir + 2, sutun + 10, cetvel.ilKod + "-" + cetvel.ilceKod);
                    XLS.HucreDegerYaz(satir + 2, sutun + 13, txtYil.Text);
                    XLS.HucreDegerYaz(satir + 3, sutun + 3, cetvel.harcamaAd);
                    XLS.HucreDegerYaz(satir + 3, sutun + 10, cetvel.harcamaKod);
                    XLS.HucreDegerYaz(satir + 3, sutun + 13, eskiHesapKod);
                    XLS.HucreDegerYaz(satir + 4, sutun + 3, cetvel.muhasebeAd);
                    XLS.HucreDegerYaz(satir + 4, sutun + 10, cetvel.muhasebeKod);

                    XLS.HucreBirlestir(satir + 6, sutun, satir + 7, sutun);
                    XLS.HucreBirlestir(satir + 6, sutun + 1, satir + 7, sutun + 1);
                    XLS.HucreBirlestir(satir + 6, sutun + 2, satir + 7, sutun + 3);
                    XLS.HucreBirlestir(satir + 6, sutun + 4, satir + 7, sutun + 4);

                    for (int j = sutun + 5; j < sutun + 14; j += 2)
                        XLS.HucreBirlestir(satir + 6, j, satir + 6, j + 1);

                    satir += 7;
                }

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                //Detay yazıyorum
                DetayYaz(XLS, satir, sutun, siraNo, detay);
                DetayTopla(cd, detay);
                DetayTopla(hesapToplam, detay);
                DetayTopla(genelToplam, detay);
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            //Son toplamı yazıyorum
            DetayYaz(XLS, satir, sutun, 0, cd);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -1, hesapToplam);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -2, genelToplam);



            if (sablonAd == "TASINIRSAYIMCETVELI.xlt")
            {
                TasinirSayimCetveliImzaEkle(XLS, ref satir, sutun, eskiHesapKod);
                OrnekNoYaz(XLS, ++satir, sutun, 13);
            }
            else
            {
                TasinirYonetimHesabiCetveliImzaEkle(XLS, ref satir, sutun);
                OrnekNoYaz(XLS, ++satir, sutun, 14);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Harcama birimi taşınır yönetim hesabı cetveli excel raporunu muhasebe raporu formatında hazırlayan yordam
        /// </summary>
        /// <param name="bilgi">İstenen cetvele ait bilgileri tutan nesne</param>
        /// <param name="sablonAd">Cetvelin oluşturulacağı şablonun adı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcamaBirimKod">Harcama birimi</param>
        /// <param name="kriter">The kriter.</param>
        private void TasinirSayimVEYonetimHesabiCetveliMuhasebe(ObjectArray bilgi, string sablonAd, int yil, string muhasebe, string harcamaBirimKod, Cetvel kriter)
        {
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

            //Muhasebe Tarafında kayıt edilmiş olup taşınırda olmayanları kaçırmamak için
            string[] kodlar = MuhasebedeKullanilanTasinirKodlariBul(yil, muhasebe, harcamaBirimKod, kriter);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];
            CetvelDetay cd = new CetvelDetay();

            string eskiHesapKod = string.Empty;
            Cetvel cetvel2 = new Cetvel();
            CetvelDetay ccd = new CetvelDetay();

            foreach (CetvelDetay cc in cetvel.detay)
            {
                if (eskiHesapKod != cc.hesapPlanKod && eskiHesapKod != "")
                {
                    cetvel2.detay.Add(ccd);
                    ccd = new CetvelDetay();
                }

                ccd.hesapPlanKod = cc.hesapPlanKod;
                ccd.hesapPlanAd = cc.hesapPlanAd;
                ccd.gecenYildanDevreden.miktar += cc.gecenYildanDevreden.miktar;
                ccd.gecenYildanDevreden.tutar += cc.gecenYildanDevreden.tutar;
                ccd.yilIcindeGiren.miktar += cc.yilIcindeGiren.miktar;
                ccd.yilIcindeGiren.tutar += cc.yilIcindeGiren.tutar;
                ccd.toplam.miktar += cc.toplam.miktar;
                ccd.toplam.tutar += cc.toplam.tutar;
                ccd.yilIcindeCikan.miktar += cc.yilIcindeCikan.miktar;
                ccd.yilIcindeCikan.tutar += cc.yilIcindeCikan.tutar;
                ccd.gelecekYilaDevir.miktar += cc.gelecekYilaDevir.miktar;
                ccd.gelecekYilaDevir.tutar += cc.gelecekYilaDevir.tutar;

                eskiHesapKod = cc.hesapPlanKod;
            }

            if (ccd.hesapPlanKod != "")
                cetvel2.detay.Add(ccd);

            int siraNo = 0;
            double toplamMuhasebeBakiye = 0;
            double toplamBakiye = 0;
            for (int i = 0; i < cetvel2.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel2.detay[i];

                siraNo++;

                if (siraNo == 1)//Başlık kısmı ilk girişte yazdırılması için
                {
                    XLS.HucreDegerYaz(kaynakSatir - 6, sutun + 3, cetvel.ilAd + "-" + cetvel.ilceAd);
                    XLS.HucreDegerYaz(kaynakSatir - 6, sutun + 10, cetvel.ilKod + "-" + cetvel.ilceKod);
                    XLS.HucreDegerYaz(kaynakSatir - 6, sutun + 16, txtYil.Text);
                    XLS.HucreDegerYaz(kaynakSatir - 5, sutun + 3, cetvel.harcamaAd);
                    XLS.HucreDegerYaz(kaynakSatir - 5, sutun + 10, cetvel.harcamaKod);
                    XLS.HucreDegerYaz(kaynakSatir - 4, sutun + 3, cetvel.muhasebeAd);
                    XLS.HucreDegerYaz(kaynakSatir - 4, sutun + 10, cetvel.muhasebeKod);
                }

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                //Detay yazıyorum
                DetayYaz(XLS, satir, sutun, siraNo, detay);

                double bakiye = 0;
                string kharcamaBirimKod = "";
                string kBirim = "";
                harcamaBirimKod = harcamaBirimKod.Replace(".", "");
                if (harcamaBirimKod.Length > 8)
                {
                    kharcamaBirimKod = harcamaBirimKod.Substring(0, 8);
                    kBirim = harcamaBirimKod.Substring(8);
                }

                try
                {
                    TNS.MUH.OdemeEmriMIFForm oeForm = new TNS.MUH.OdemeEmriMIFForm();
                    oeForm.yil = yil;
                    oeForm.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
                    oeForm.fisTur = new TNS.MUH.ENUMFISTUR[2];
                    oeForm.fisTur[0] = TNS.MUH.ENUMFISTUR.ACILIS;
                    oeForm.fisTur[1] = TNS.MUH.ENUMFISTUR.DONEMICI;

                    if (kharcamaBirimKod.StartsWith("460604") && kBirim.StartsWith("32"))//EGO için ve metro ankaray vb
                        oeForm.birim = kBirim;

                    if (!kriter.tarih1.isNull)
                        oeForm.sorguIslemTarih = new TNSDateTime(kriter.tarih1.ToString()) + ";" + new TNSDateTime(kriter.tarih2.ToString());

                    oeForm.formDurum = 22;//Bütün Belgeler
                    oeForm.detay = new ObjectArray();
                    TNS.MUH.OdemeEmriMIFDetay oeDetay = new TNS.MUH.OdemeEmriMIFDetay(oeForm.yil);

                    if (string.IsNullOrEmpty(oeForm.birim))
                        oeDetay.kur = kharcamaBirimKod;

                    oeDetay.hesapKod = detay.hesapPlanKod;
                    oeForm.detay.objeler.Add(oeDetay);
                    kullanan.ad = "aktarim";
                    ObjectArray belgeler = servisMUH.OdemeEmriMIFListele(kullanan, oeForm, false, false);
                    bakiye = 0;

                    foreach (TNS.MUH.OdemeEmriMIFForm belge in belgeler.objeler)
                    {
                        bakiye += belge.borcToplam - belge.alacakToplam;
                    }
                }
                catch { bakiye = 0; }
                XLS.HucreDegerYaz(satir, sutun + 15, bakiye);
                toplamMuhasebeBakiye += bakiye;
                bakiye = Math.Round(bakiye - OrtakFonksiyonlar.ConvertToDouble(Math.Round(detay.gelecekYilaDevir.tutar, 2)), 2);
                toplamBakiye += bakiye;
                XLS.HucreDegerYaz(satir, sutun + 16, bakiye);

                DetayTopla(cd, detay);
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            //Son toplamı yazıyorum
            DetayYaz(XLS, satir, sutun, 0, cd);
            XLS.HucreDegerYaz(satir, sutun + 15, toplamMuhasebeBakiye);
            XLS.HucreDegerYaz(satir, sutun + 16, toplamBakiye);
            XLS.KoyuYap(satir, sutun + 15, true);
            XLS.KoyuYap(satir, sutun + 16, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        private string[] MuhasebedeKullanilanTasinirKodlariBul(int yil, string muhasebe, string harcamaBirimKod, Cetvel kriter)
        {
            string hesaplar = "";
            string kharcamaBirimKod = "";
            string kBirim = "";
            harcamaBirimKod = harcamaBirimKod.Replace(".", "");
            if (harcamaBirimKod.Length > 8)
            {
                kharcamaBirimKod = harcamaBirimKod.Substring(0, 8);
                kBirim = harcamaBirimKod.Substring(8);
            }

            TNS.MUH.OdemeEmriMIFForm oeForm = new TNS.MUH.OdemeEmriMIFForm();
            oeForm.yil = yil;
            oeForm.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
            oeForm.fisTur = new TNS.MUH.ENUMFISTUR[2];
            oeForm.fisTur[0] = TNS.MUH.ENUMFISTUR.ACILIS;
            oeForm.fisTur[1] = TNS.MUH.ENUMFISTUR.DONEMICI;

            if (kharcamaBirimKod.StartsWith("460604") && kBirim.StartsWith("32"))//EGO için ve metro ankaray vb
                oeForm.birim = kBirim;

            if (!kriter.tarih1.isNull)
                oeForm.sorguIslemTarih = new TNSDateTime(kriter.tarih1.ToString()) + ";" + new TNSDateTime(kriter.tarih2.ToString());

            oeForm.formDurum = 22;//Bütün Belgeler
            oeForm.detay = new ObjectArray();
            TNS.MUH.OdemeEmriMIFDetay oeDetay = new TNS.MUH.OdemeEmriMIFDetay(oeForm.yil);

            if (string.IsNullOrEmpty(oeForm.birim))
                oeDetay.kur = kharcamaBirimKod;

            oeDetay.hesapKod = "150;253;254;255";
            oeForm.detay.objeler.Add(oeDetay);
            kullanan.ad = "aktarim";
            int tur = 2;//İşlem tarihine göre işlem yapsın
            int listeTur = 0;//Para birimi için
            TNS.MUH.MizanBilgi mb = servisMUH.Mizan((TNS.KYM.Kullanici)kullanan, yil, GenelIslemlerIstemci.VarsayilanKurumBul(), oeForm.muhasebe, oeForm.birim, kriter.tarih1, kriter.tarih2, 9, "100", "999", tur, oeForm.fisTur, listeTur, "");
            foreach (TNS.MUH.AnaHesap ml in mb.anaHesaplar.objeler)
            {
                foreach (TNS.MUH.HesapBilgi ml2 in ml.detayHesaplar.objeler)
                {
                    if (hesaplar != "") hesaplar += ";";
                     hesaplar += ml2.hesapKod;
                }
            }

            return hesaplar.Split(';');
        }

        /// <summary>
        /// Taşınır sayım ve döküm cetveli excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">İmza bilgilerinin yazılmaya başlanacağı satır numarası</param>
        /// <param name="sutun">İmza bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        /// <param name="hesapKodu">Açıklamaya eklenecek taşınır hesap planı kodu</param>
        private void TasinirSayimCetveliImzaEkle(Tablo XLS, ref int satir, int sutun, string hesapKodu)
        {
            satir += 1;

            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", 0);

            string[] ad = new string[4];
            string[] unvan = new string[4];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.SAYIMKURULUBASKANI && string.IsNullOrEmpty(ad[1]))
                    {
                        ad[1] = iBilgi.adSoyad;
                        unvan[1] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.SAYIMKURULUUYE1 && string.IsNullOrEmpty(ad[2]))
                    {
                        ad[2] = iBilgi.adSoyad;
                        unvan[2] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.SAYIMKURULUUYE2 && string.IsNullOrEmpty(ad[3]))
                    {
                        ad[3] = iBilgi.adSoyad;
                        unvan[3] = iBilgi.unvan;
                    }
                }
            }

            XLS.SatirAc(satir, 13);
            XLS.HucreKopyala(0, sutun, 12, sutun + 14, satir, sutun);

            XLS.HucreBirlestir(satir + 1, sutun, satir + 3, sutun + 14);

            string pBirimi = TasinirGenel.ParaBirimiDondur();
            XLS.HucreDegerYaz(satir + 1, sutun, string.Format(Resources.TasinirMal.FRMCTV009, txtYil.Text, "", DateTime.Today.Date.ToShortDateString(), pBirimi));
            XLS.YatayHizala(satir + 1, sutun, 2);

            XLS.HucreDegerYaz(satir + 4, sutun + 10, Resources.TasinirMal.FRMCTV010);
            XLS.KoyuYap(satir + 4, sutun + 10, true);
            XLS.HucreDegerYaz(satir + 5, sutun + 10, string.Format(Resources.TasinirMal.FRMCTV011, ad[0]));
            XLS.HucreDegerYaz(satir + 6, sutun + 10, Resources.TasinirMal.FRMCTV012);

            for (int i = satir + 4; i < satir + 7; i++)
            {
                XLS.HucreBirlestir(i, sutun + 10, i, sutun + 13);
                XLS.DuseyHizala(i, sutun + 10, 0);
                XLS.YatayHizala(i, sutun + 10, 2);
            }

            satir += 7;

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMCTV013);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 2);
            XLS.YatayHizala(satir, sutun, 2);

            XLS.HucreDegerYaz(satir + 1, sutun + 3, Resources.TasinirMal.FRMCTV014);
            XLS.HucreDegerYaz(satir + 1, sutun + 7, Resources.TasinirMal.FRMCTV015);
            XLS.HucreDegerYaz(satir + 1, sutun + 11, Resources.TasinirMal.FRMCTV015);

            XLS.HucreDegerYaz(satir + 2, sutun + 1, Resources.TasinirMal.FRMCTV016);
            XLS.HucreDegerYaz(satir + 3, sutun + 1, Resources.TasinirMal.FRMCTV017);
            XLS.HucreDegerYaz(satir + 4, sutun + 1, Resources.TasinirMal.FRMCTV018);

            XLS.HucreDegerYaz(satir + 2, sutun + 3, ad[1]);
            XLS.HucreDegerYaz(satir + 3, sutun + 3, unvan[1]);

            XLS.HucreDegerYaz(satir + 2, sutun + 7, ad[2]);
            XLS.HucreDegerYaz(satir + 3, sutun + 7, unvan[2]);

            XLS.HucreDegerYaz(satir + 2, sutun + 11, ad[3]);
            XLS.HucreDegerYaz(satir + 3, sutun + 11, unvan[3]);

            for (int i = satir + 2; i < satir + 5; i++)
            {
                XLS.HucreBirlestir(i, sutun + 1, i, sutun + 2);
                XLS.KoyuYap(i, sutun + 1, true);
                XLS.DuseyHizala(i, sutun + 1, 0);
                XLS.YatayHizala(i, sutun + 1, 2);
            }

            for (int i = satir + 1; i < satir + 5; i++)
            {
                for (int j = sutun + 3; j <= sutun + 11; j += 4)
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

            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 14, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir - 8, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir - 8, satir + 5, sutun + 15, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 5;
        }

        /// <summary>
        /// Excel raporuna para birimini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">Para biriminin yazılacağı satır numarası</param>
        /// <param name="sutun">Para biriminin yazılacağı sütun numarası</param>
        private void YTLYaz(Tablo XLS, int satir, int sutun)
        {
            string pBirimi = TasinirGenel.ParaBirimiDondur();
            XLS.HucreKopyala(satir - 1, sutun, satir - 1, sutun, satir, sutun);
            XLS.YaziTipBuyuklugu(satir, sutun, 8);
            XLS.HucreDegerYaz(satir, sutun, pBirimi);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 2);
            XLS.YatayHizala(satir, sutun, 2);
        }

        /// <summary>
        /// Excel raporuna rapor numarasını ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">Rapor numarasının yazılacağı satır</param>
        /// <param name="sutun">Rapor numarasının yazılacağı sütun</param>
        /// <param name="ornekNo">Rapor numarası</param>
        private void OrnekNoYaz(Tablo XLS, int satir, int sutun, int ornekNo)
        {
            XLS.HucreKopyala(satir - 1, sutun, satir - 1, sutun, satir, sutun);
            XLS.YatayCizgiCiz(satir, sutun, sutun + 3, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir, sutun, OrtakClass.LineStyle.NONE, OrtakClass.TabloRenk.BLACK, true);
            XLS.HucreDegerYaz(satir, sutun, string.Format(Resources.TasinirMal.FRMCTV020, ornekNo.ToString()));
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 0);
            XLS.YatayHizala(satir, sutun, 2);
        }

        /// <summary>
        /// Müze/Kütüphane yönetim hesabı cetveli excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">İmza bilgilerinin yazılmaya başlanacağı satır numarası</param>
        /// <param name="sutun">İmza bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        private void MuzeKutuphaneCetveliImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 1;

            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", 0);

            string[] ad = new string[4];
            string[] unvan = new string[4];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI && string.IsNullOrEmpty(ad[1]))
                    {
                        ad[1] = iBilgi.adSoyad;
                        unvan[1] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.MUHASEBEYETKILISI && string.IsNullOrEmpty(ad[2]))
                    {
                        ad[2] = iBilgi.adSoyad;
                        unvan[2] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKONTROLYETKILISI && string.IsNullOrEmpty(ad[3]))
                    {
                        ad[3] = iBilgi.adSoyad;
                        unvan[3] = iBilgi.unvan;
                    }
                }
            }

            XLS.SatirAc(satir, 13);
            XLS.HucreKopyala(0, sutun, 12, sutun + 18, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir + 12, GenelIslemler.JexcelBirimtoExcelBirim(400));

            XLS.HucreDegerYaz(satir + 1, sutun + 2, Resources.TasinirMal.FRMCTV021);
            XLS.HucreDegerYaz(satir + 2, sutun + 2, Resources.TasinirMal.FRMCTV022);
            XLS.HucreDegerYaz(satir + 3, sutun + 2, string.Format(Resources.TasinirMal.FRMCTV011, ad[0]));
            XLS.HucreDegerYaz(satir + 4, sutun + 2, Resources.TasinirMal.FRMGAG007);
            XLS.HucreDegerYaz(satir + 5, sutun + 2, string.Format(Resources.TasinirMal.FRMCTV024, DateTime.Today.Date.ToShortDateString()));

            XLS.HucreDegerYaz(satir + 1, sutun + 7, Resources.TasinirMal.FRMCTV075);
            XLS.HucreDegerYaz(satir + 2, sutun + 7, Resources.TasinirMal.FRMCTV074);
            XLS.HucreDegerYaz(satir + 3, sutun + 7, ad[3]);
            XLS.HucreDegerYaz(satir + 4, sutun + 7, Resources.TasinirMal.FRMCTV027);
            XLS.HucreDegerYaz(satir + 5, sutun + 7, DateTime.Today.Date.ToShortDateString());

            XLS.HucreDegerYaz(satir + 1, sutun + 12, Resources.TasinirMal.FRMCTV028);
            XLS.HucreDegerYaz(satir + 2, sutun + 12, Resources.TasinirMal.FRMCTV029);
            XLS.HucreDegerYaz(satir + 3, sutun + 12, ad[2]);
            XLS.HucreDegerYaz(satir + 4, sutun + 12, Resources.TasinirMal.FRMCTV027);
            XLS.HucreDegerYaz(satir + 5, sutun + 12, DateTime.Today.Date.ToShortDateString());

            XLS.HucreDegerYaz(satir + 7, sutun + 9, Resources.TasinirMal.FRMCTV025);
            XLS.HucreDegerYaz(satir + 8, sutun + 9, Resources.TasinirMal.FRMCTV032);
            XLS.HucreDegerYaz(satir + 9, sutun + 9, ad[1]);
            XLS.HucreDegerYaz(satir + 10, sutun + 9, Resources.TasinirMal.FRMGAG007);
            //XLS.HucreDegerYaz(satir + 11, sutun + 9, Resources.TasinirMal.FRMCTV027);
            //XLS.HucreDegerYaz(satir + 11, sutun + 12, DateTime.Today.Date.ToShortDateString());

            for (int i = satir + 1; i < satir + 6; i++)
            {
                for (int j = sutun + 2; j <= sutun + 12; j += 5)
                {
                    XLS.HucreBirlestir(i, j, i, j + 3);

                    if (i == satir + 2)
                    {
                        XLS.KoyuYap(i, j, true);
                        XLS.DuseyHizala(i, j, 2);
                        XLS.YatayHizala(i, j, 2);
                    }
                }
            }

            for (int i = satir + 7; i < satir + 12; i++)
            {
                XLS.HucreBirlestir(i, sutun + 9, i, sutun + 12);
            }
            XLS.KoyuYap(satir + 8, sutun + 9, true);

            XLS.YatayCizgiCiz(satir + 12, sutun, sutun + 18, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 11, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 11, sutun + 19, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 11;
        }

        /// <summary>
        /// Harcama birimi taşınır yönetim hesabı cetveli excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">İmza bilgilerinin yazılmaya başlanacağı satır numarası</param>
        /// <param name="sutun">İmza bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        private void TasinirYonetimHesabiCetveliImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 1;

            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", 0);

            string[] ad = new string[4];
            string[] unvan = new string[4];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI && string.IsNullOrEmpty(ad[1]))
                    {
                        ad[1] = iBilgi.adSoyad;
                        unvan[1] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.MUHASEBEYETKILISI && string.IsNullOrEmpty(ad[2]))
                    {
                        ad[2] = iBilgi.adSoyad;
                        unvan[2] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKONTROLYETKILISI && string.IsNullOrEmpty(ad[3]))
                    {
                        ad[3] = iBilgi.adSoyad;
                        unvan[3] = iBilgi.unvan;
                    }
                }
            }

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 5, sutun + 14, satir, sutun);

            XLS.HucreDegerYaz(satir + 1, sutun + 2, Resources.TasinirMal.FRMCTV030);
            XLS.HucreDegerYaz(satir + 2, sutun + 2, Resources.TasinirMal.FRMCTV022);
            XLS.HucreDegerYaz(satir + 3, sutun + 2, string.Format(Resources.TasinirMal.FRMCTV011, ad[0]));
            XLS.HucreDegerYaz(satir + 4, sutun + 2, Resources.TasinirMal.FRMGAG007);

            XLS.HucreDegerYaz(satir + 1, sutun + 6, Resources.TasinirMal.FRMCTV075);
            XLS.HucreDegerYaz(satir + 2, sutun + 6, Resources.TasinirMal.FRMCTV074);
            XLS.HucreDegerYaz(satir + 3, sutun + 6, ad[3]);
            XLS.HucreDegerYaz(satir + 4, sutun + 6, Resources.TasinirMal.FRMCTV027);

            XLS.HucreDegerYaz(satir + 1, sutun + 10, Resources.TasinirMal.FRMCTV028);
            XLS.HucreDegerYaz(satir + 2, sutun + 10, Resources.TasinirMal.FRMCTV033);
            XLS.HucreDegerYaz(satir + 3, sutun + 10, ad[2]);
            XLS.HucreDegerYaz(satir + 4, sutun + 10, Resources.TasinirMal.FRMCTV027);


            XLS.HucreDegerYaz(satir + 6, sutun + 6, Resources.TasinirMal.FRMCTV025);
            XLS.HucreDegerYaz(satir + 7, sutun + 6, Resources.TasinirMal.FRMCTV032);
            XLS.HucreDegerYaz(satir + 8, sutun + 6, string.Format(Resources.TasinirMal.FRMCTV011, ad[1]));
            XLS.HucreDegerYaz(satir + 9, sutun + 6, Resources.TasinirMal.FRMGAG007);

            for (int i = satir + 1; i < satir + 10; i++)
            {
                for (int j = sutun + 2; j <= sutun + 10; j += 4)
                {
                    XLS.HucreBirlestir(i, j, i, j + 2);

                    if (i == satir + 2 || i == satir + 7)
                    {
                        XLS.KoyuYap(i, j, true);
                        XLS.DuseyHizala(i, j, 2);
                        XLS.YatayHizala(i, j, 2);
                    }
                }
            }

            XLS.YatayCizgiCiz(satir + 10, sutun, sutun + 14, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 9, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 9, sutun + 15, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 9;
        }

        /// <summary>
        /// Taşınır kesin hesap icmal cetveli excel raporuna cetvel detay satır bilgilerini yazan yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">Cetvel detay bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Cetvel detay bilgilerinin yazılacağı sütun numarası</param>
        /// <param name="detay">Cetvel detay bilgilerini tutan nesne</param>
        /// <param name="toplamMi">Toplam bilgileri mi yazılacak bilgisi</param>
        private void IcmalDetayYaz(Tablo XLS, int satir, int sutun, CetvelDetay detay, bool toplamMi, bool genelToplam)
        {
            if (!toplamMi)
            {
                XLS.HucreDegerYaz(satir, sutun, detay.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanAd);
            }
            else
            {
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
                if (genelToplam)
                    XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK009);
                else
                    XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMCTV034);

                for (int m = sutun; m <= sutun + 13; m++)
                    XLS.KoyuYap(satir, m, true);
            }

            XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(detay.gecenYildanDevreden.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeGiren.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(detay.toplam.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeCikan.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(detay.gelecekYilaDevir.tutar.ToString(), (double)0));
        }

        private string IcmalDetayYazCSV(CetvelDetay detay, int yil)
        {
            string satir = GenelIslemlerIstemci.VarsayilanKurumBulAd() + "|";
            satir += GenelIslemlerIstemci.VarsayilanKurumBul() + "|";
            satir += detay.muhasebeAd + "|";
            satir += detay.muhasebeKod + "|";
            satir += detay.hesapPlanKod.Substring(0, 3) + "|";
            satir += yil.ToString() + "|";
            satir += detay.hesapPlanKod.Substring(4, 2) + "|";
            satir += detay.hesapPlanAd + "|";
            satir += detay.gecenYildanDevreden.tutar.ToString() + "|";
            satir += detay.yilIcindeGiren.tutar.ToString() + "|";
            satir += detay.toplam.miktar.ToString() + "|";
            satir += detay.yilIcindeCikan.tutar.ToString() + "|";
            satir += detay.gelecekYilaDevir.tutar.ToString() + "|";
            satir += "" + "|";
            satir += "" + "|";
            satir += "" + "|";
            satir += DateTime.Now.ToShortDateString();
            return satir;
        }

        /// <summary>
        /// Excel raporlarına cetvel detay satır bilgilerini yazan yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">Cetvel detay bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Cetvel detay bilgilerinin yazılacağı sütun numarası</param>
        /// <param name="siraNo">Cetvel detayının rapordaki sıra numarası</param>
        /// <param name="detay">Cetvel detay bilgilerini tutan nesne</param>
        private void DetayYaz(Tablo XLS, int satir, int sutun, int siraNo, CetvelDetay detay)
        {
            if (siraNo > 0)
            {
                XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
                XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 4, detay.olcuBirimAd);
            }
            else
            {
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 4);
                if (siraNo == 0)
                    XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMCTV034);
                else if (siraNo == -1)
                    XLS.HucreDegerYaz(satir, sutun, detay.hesapPlanKod + " HESABI TOPLAMI");
                else if (siraNo == -2)
                    XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK009);

                for (int m = sutun; m <= sutun + 14; m++)
                    XLS.KoyuYap(satir, m, true);
            }

            XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(detay.gecenYildanDevreden.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.gecenYildanDevreden.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeGiren.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeGiren.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(detay.toplam.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.toplam.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeCikan.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeCikan.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(detay.gelecekYilaDevir.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDouble(detay.gelecekYilaDevir.tutar.ToString(), (double)0));
        }

        private string DetayYazCSV(CetvelDetay detay, int yil)
        {
            string satir = GenelIslemlerIstemci.VarsayilanKurumBulAd() + "|";
            satir += GenelIslemlerIstemci.VarsayilanKurumBul() + "|";
            satir += detay.muhasebeAd + "|";
            satir += detay.muhasebeKod + "|";
            satir += detay.hesapPlanKod.Substring(0, 6) + "|";
            satir += yil.ToString() + "|";
            satir += detay.hesapPlanKod.Substring(7, 2) + "|";
            satir += detay.hesapPlanAd + "|";
            satir += detay.olcuBirimAd + "|";
            satir += detay.gecenYildanDevreden.miktar.ToString() + "|";
            satir += detay.gecenYildanDevreden.tutar.ToString() + "|";
            satir += detay.yilIcindeGiren.miktar.ToString() + "|";
            satir += detay.yilIcindeGiren.tutar.ToString() + "|";
            satir += detay.toplam.miktar.ToString() + "|";
            satir += detay.toplam.tutar.ToString() + "|";
            satir += detay.yilIcindeCikan.miktar.ToString() + "|";
            satir += detay.yilIcindeCikan.tutar.ToString() + "|";
            satir += detay.gelecekYilaDevir.miktar.ToString() + "|";
            satir += detay.gelecekYilaDevir.tutar.ToString() + "|";
            satir += "" + "|";
            satir += DateTime.Now.ToShortDateString();
            return satir;
        }

        /// <summary>
        /// detay parametresindeki cetvel detay bilgilerini toplam parametresine ekleyen yordam
        /// </summary>
        /// <param name="toplam">Toplam cetvel detay bilgilerini tutan nesne</param>
        /// <param name="detay">Eklenecek cetvel detay bilgilerini tutan nesne</param>
        private void DetayTopla(CetvelDetay toplam, CetvelDetay detay)
        {
            toplam.gecenYildanDevreden.miktar += Math.Round(detay.gecenYildanDevreden.miktar, 2);
            toplam.gecenYildanDevreden.tutar += Math.Round(detay.gecenYildanDevreden.tutar, 2);
            toplam.yilIcindeGiren.miktar += Math.Round(detay.yilIcindeGiren.miktar, 2);
            toplam.yilIcindeGiren.tutar += Math.Round(detay.yilIcindeGiren.tutar, 2);
            toplam.toplam.miktar += Math.Round(detay.toplam.miktar, 2);
            toplam.toplam.tutar += Math.Round(detay.toplam.tutar, 2);
            toplam.yilIcindeCikan.miktar += Math.Round(detay.yilIcindeCikan.miktar, 2);
            toplam.yilIcindeCikan.tutar += Math.Round(detay.yilIcindeCikan.tutar, 2);
            toplam.gelecekYilaDevir.miktar += Math.Round(detay.gelecekYilaDevir.miktar, 2);
            toplam.gelecekYilaDevir.tutar += Math.Round(detay.gelecekYilaDevir.tutar, 2);

            toplam.hesapPlanKod = detay.hesapPlanKod.Substring(0, 3);
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki taşınır hesap cetveli yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        private void TasinirHesapCetveli(Cetvel kriter)
        {
            ObjectArray bilgi = servisTMM.TasinirHesapCetveli(kullanan, kriter);

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

            string kurKod = string.Empty;
            string kurAd = string.Empty;
            GenelIslemlerIstemci.VarsayilanKurumBul(out kurKod, out kurAd);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TASINIRHESAPCETVELI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            Cetvel eskiCetvel = new Cetvel();
            CetvelDetay cd = new CetvelDetay();
            CetvelDetay genelToplam = new CetvelDetay();
            CetvelDetay hesapToplam = new CetvelDetay();

            foreach (Cetvel cetvel in bilgi.objeler)
            {
                string eskiHesapKod = string.Empty;

                int siraNo = 0;
                for (int i = 0; i < cetvel.detay.Count; i++)
                {
                    CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                    siraNo++;

                    if (eskiCetvel.ilKod != cetvel.ilKod || eskiCetvel.ilceKod != cetvel.ilceKod || !detay.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                    {
                        if (!string.IsNullOrEmpty(eskiCetvel.ilKod))
                        {
                            satir++;
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                            //Toplam yazıyor
                            DetayYaz(XLS, satir, sutun, 0, cd);
                            cd = new CetvelDetay();

                            if (eskiHesapKod.Substring(0, 3) != detay.hesapPlanKod.Substring(0, 3) && !string.IsNullOrEmpty(eskiHesapKod))
                            {
                                satir++;
                                XLS.SatirAc(satir, 1);
                                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                                DetayYaz(XLS, satir, sutun, -1, hesapToplam);
                                hesapToplam = new CetvelDetay();
                            }


                            TasinirHesapCetveliImzaEkle(XLS, ref satir, sutun);
                            OrnekNoYaz(XLS, ++satir, sutun, 15);

                            XLS.SayfaSonuKoyHucresel(satir + 3);
                            satir += 3;
                        }

                        siraNo = 1;
                        eskiCetvel = cetvel;
                        eskiHesapKod = detay.hesapPlanKod.Substring(0, 6);

                        XLS.SatirAc(satir, 7);
                        XLS.HucreKopyala(kaynakSatir - 7, sutun, kaynakSatir - 1, sutun + 14, satir, sutun);

                        XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);

                        for (int j = satir + 2; j < satir + 4; j++)
                        {
                            XLS.HucreBirlestir(j, sutun, j, sutun + 1);
                            XLS.HucreBirlestir(j, sutun + 3, j, sutun + 8);
                            XLS.HucreBirlestir(j, sutun + 10, j, sutun + 11);
                            XLS.HucreBirlestir(j, sutun + 13, j, sutun + 14);
                        }

                        YTLYaz(XLS, satir + 1, sutun + 14);

                        if (cetvel.ilAd == "Merkez" && cetvel.ilceAd == "Merkez")
                            XLS.HucreDegerYaz(satir, sutun, string.Format(Resources.TasinirMal.FRMCTV035, cetvel.ilAd.ToUpper()));
                        else
                            XLS.HucreDegerYaz(satir, sutun, string.Format(Resources.TasinirMal.FRMCTV036, cetvel.ilAd.ToUpper(), cetvel.ilceAd.ToUpper()));

                        XLS.HucreDegerYaz(satir + 2, sutun + 3, cetvel.ilAd + "-" + cetvel.ilceAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 10, cetvel.ilKod + "-" + cetvel.ilceKod);
                        XLS.HucreDegerYaz(satir + 2, sutun + 13, txtYil.Text);
                        XLS.HucreDegerYaz(satir + 3, sutun + 3, kurAd);
                        XLS.HucreDegerYaz(satir + 3, sutun + 10, kurKod);
                        XLS.HucreDegerYaz(satir + 3, sutun + 13, eskiHesapKod);

                        XLS.HucreBirlestir(satir + 5, sutun, satir + 6, sutun);
                        XLS.HucreBirlestir(satir + 5, sutun + 1, satir + 6, sutun + 1);
                        XLS.HucreBirlestir(satir + 5, sutun + 2, satir + 6, sutun + 3);
                        XLS.HucreBirlestir(satir + 5, sutun + 4, satir + 6, sutun + 4);

                        for (int j = sutun + 5; j < sutun + 14; j += 2)
                            XLS.HucreBirlestir(satir + 5, j, satir + 5, j + 1);

                        satir += 6;
                    }

                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                    DetayYaz(XLS, satir, sutun, siraNo, detay);
                    DetayTopla(cd, detay);
                    DetayTopla(hesapToplam, detay);
                    DetayTopla(genelToplam, detay);
                }
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            //Son toplamı yazıyor
            DetayYaz(XLS, satir, sutun, 0, cd);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -1, hesapToplam);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -2, genelToplam);

            TasinirHesapCetveliImzaEkle(XLS, ref satir, sutun);
            OrnekNoYaz(XLS, ++satir, sutun, 15);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki taşınır hesap cetveli (bölge) yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        private void TasinirHesapCetveliBolge(Cetvel kriter)
        {
            ObjectArray bilgi = servisTMM.TasinirHesapCetveliBolge(kullanan, kriter);

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

            string kurKod = string.Empty;
            string kurAd = string.Empty;
            GenelIslemlerIstemci.VarsayilanKurumBul(out kurKod, out kurAd);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TASINIRHESAPCETVELI(Bolge).xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            Cetvel eskiCetvel = new Cetvel();
            CetvelDetay cd = new CetvelDetay();
            CetvelDetay genelToplam = new CetvelDetay();
            CetvelDetay hesapToplam = new CetvelDetay();

            foreach (Cetvel cetvel in bilgi.objeler)
            {
                string eskiHesapKod = string.Empty;

                int siraNo = 0;
                for (int i = 0; i < cetvel.detay.Count; i++)
                {
                    CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                    siraNo++;

                    if (eskiCetvel.bolgeKod != cetvel.bolgeKod || eskiCetvel.harcamaKod != cetvel.harcamaKod || !detay.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                    {
                        if (eskiCetvel.bolgeKod > 0)
                        {
                            satir++;
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                            //Toplam yazıyor
                            DetayYaz(XLS, satir, sutun, 0, cd);
                            cd = new CetvelDetay();

                            if (eskiHesapKod.Substring(0, 3) != detay.hesapPlanKod.Substring(0, 3) && !string.IsNullOrEmpty(eskiHesapKod))
                            {
                                satir++;
                                XLS.SatirAc(satir, 1);
                                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                                DetayYaz(XLS, satir, sutun, -1, hesapToplam);
                                hesapToplam = new CetvelDetay();
                            }


                            TasinirHesapCetveliImzaEkle(XLS, ref satir, sutun);
                            OrnekNoYaz(XLS, ++satir, sutun, 15);

                            XLS.SayfaSonuKoyHucresel(satir + 3);
                            satir += 3;
                        }

                        siraNo = 1;
                        eskiCetvel = cetvel;
                        eskiHesapKod = detay.hesapPlanKod.Substring(0, 6);

                        XLS.SatirAc(satir, 7);
                        XLS.HucreKopyala(kaynakSatir - 7, sutun, kaynakSatir - 1, sutun + 14, satir, sutun);

                        XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);

                        for (int j = satir + 2; j < satir + 4; j++)
                        {
                            XLS.HucreBirlestir(j, sutun, j, sutun + 1);
                            XLS.HucreBirlestir(j, sutun + 3, j, sutun + 8);
                            XLS.HucreBirlestir(j, sutun + 10, j, sutun + 11);
                            XLS.HucreBirlestir(j, sutun + 13, j, sutun + 14);
                        }

                        XLS.HucreBirlestir(satir + 2, sutun + 13, satir + 2, sutun + 14);
                        //XLS.HucreBirlestir(satir + 3, sutun + 12, satir + 4, sutun + 12);
                        //XLS.HucreBirlestir(satir + 3, sutun + 13, satir + 4, sutun + 14);

                        YTLYaz(XLS, satir + 1, sutun + 14);

                        XLS.HucreDegerYaz(satir, sutun, string.Format(Resources.TasinirMal.FRMCTV037, cetvel.bolgeAd.ToUpper()));

                        XLS.HucreDegerYaz(satir + 2, sutun + 3, cetvel.bolgeAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 10, cetvel.bolgeKod);
                        XLS.HucreDegerYaz(satir + 2, sutun + 13, txtYil.Text);
                        XLS.HucreDegerYaz(satir + 3, sutun + 3, kurAd);
                        XLS.HucreDegerYaz(satir + 3, sutun + 10, kurKod);
                        XLS.HucreDegerYaz(satir + 3, sutun + 13, eskiHesapKod);

                        XLS.HucreBirlestir(satir + 5, sutun, satir + 6, sutun);
                        XLS.HucreBirlestir(satir + 5, sutun + 1, satir + 6, sutun + 1);
                        XLS.HucreBirlestir(satir + 5, sutun + 2, satir + 6, sutun + 3);
                        XLS.HucreBirlestir(satir + 5, sutun + 4, satir + 6, sutun + 4);

                        for (int j = sutun + 5; j < sutun + 14; j += 2)
                            XLS.HucreBirlestir(satir + 5, j, satir + 5, j + 1);

                        satir += 6;
                    }

                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                    DetayYaz(XLS, satir, sutun, siraNo, detay);
                    DetayTopla(cd, detay);
                    DetayTopla(hesapToplam, detay);
                    DetayTopla(genelToplam, detay);
                }
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            //Son toplamı yazıyor
            DetayYaz(XLS, satir, sutun, 0, cd);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -1, hesapToplam);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -2, genelToplam);

            TasinirHesapCetveliImzaEkle(XLS, ref satir, sutun);
            OrnekNoYaz(XLS, ++satir, sutun, 15);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki taşınır hesap cetveli (il) yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        private void TasinirHesapCetveliIl(Cetvel kriter)
        {
            ObjectArray bilgi = servisTMM.TasinirHesapCetveliIl(kullanan, kriter);

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

            string kurKod = string.Empty;
            string kurAd = string.Empty;
            GenelIslemlerIstemci.VarsayilanKurumBul(out kurKod, out kurAd);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TASINIRHESAPCETVELI(IL).xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            Cetvel eskiCetvel = new Cetvel();
            CetvelDetay cd = new CetvelDetay();
            CetvelDetay genelToplam = new CetvelDetay();
            CetvelDetay hesapToplam = new CetvelDetay();

            foreach (Cetvel cetvel in bilgi.objeler)
            {
                string eskiHesapKod = string.Empty;

                int siraNo = 0;
                for (int i = 0; i < cetvel.detay.Count; i++)
                {
                    CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                    siraNo++;

                    if (eskiCetvel.ilKod != cetvel.ilKod || eskiCetvel.harcamaKod != cetvel.harcamaKod || !detay.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                    {
                        if (!string.IsNullOrEmpty(eskiCetvel.ilKod))
                        {
                            satir++;
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                            //Toplam yazıyor
                            DetayYaz(XLS, satir, sutun, 0, cd);
                            cd = new CetvelDetay();

                            if (eskiHesapKod.Substring(0, 3) != detay.hesapPlanKod.Substring(0, 3) && !string.IsNullOrEmpty(eskiHesapKod))
                            {
                                satir++;
                                XLS.SatirAc(satir, 1);
                                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                                DetayYaz(XLS, satir, sutun, -1, hesapToplam);
                                hesapToplam = new CetvelDetay();
                            }


                            TasinirHesapCetveliImzaEkle(XLS, ref satir, sutun);
                            OrnekNoYaz(XLS, ++satir, sutun, 15);

                            XLS.SayfaSonuKoyHucresel(satir + 3);
                            satir += 3;
                        }

                        siraNo = 1;
                        eskiCetvel = cetvel;
                        eskiHesapKod = detay.hesapPlanKod.Substring(0, 6);

                        XLS.SatirAc(satir, 7);
                        XLS.HucreKopyala(kaynakSatir - 7, sutun, kaynakSatir - 1, sutun + 14, satir, sutun);

                        XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);

                        for (int j = satir + 2; j < satir + 4; j++)
                        {
                            XLS.HucreBirlestir(j, sutun, j, sutun + 1);
                            XLS.HucreBirlestir(j, sutun + 3, j, sutun + 8);
                            XLS.HucreBirlestir(j, sutun + 10, j, sutun + 11);
                            XLS.HucreBirlestir(j, sutun + 13, j, sutun + 14);
                        }

                        XLS.HucreBirlestir(satir + 2, sutun + 13, satir + 2, sutun + 14);
                        //XLS.HucreBirlestir(satir + 3, sutun + 12, satir + 4, sutun + 12);
                        //XLS.HucreBirlestir(satir + 3, sutun + 13, satir + 4, sutun + 14);

                        YTLYaz(XLS, satir + 1, sutun + 14);

                        XLS.HucreDegerYaz(satir, sutun, string.Format(Resources.TasinirMal.FRMCTV039, cetvel.ilAd.ToUpper()));

                        XLS.HucreDegerYaz(satir + 2, sutun + 3, cetvel.ilAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 10, cetvel.ilKod);
                        XLS.HucreDegerYaz(satir + 2, sutun + 13, txtYil.Text);
                        XLS.HucreDegerYaz(satir + 3, sutun + 3, kurAd);
                        XLS.HucreDegerYaz(satir + 3, sutun + 10, kurKod);
                        XLS.HucreDegerYaz(satir + 3, sutun + 13, eskiHesapKod);

                        XLS.HucreBirlestir(satir + 5, sutun, satir + 6, sutun);
                        XLS.HucreBirlestir(satir + 5, sutun + 1, satir + 6, sutun + 1);
                        XLS.HucreBirlestir(satir + 5, sutun + 2, satir + 6, sutun + 3);
                        XLS.HucreBirlestir(satir + 5, sutun + 4, satir + 6, sutun + 4);

                        for (int j = sutun + 5; j < sutun + 14; j += 2)
                            XLS.HucreBirlestir(satir + 5, j, satir + 5, j + 1);

                        satir += 6;
                    }

                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                    DetayYaz(XLS, satir, sutun, siraNo, detay);
                    DetayTopla(cd, detay);
                    DetayTopla(hesapToplam, detay);
                    DetayTopla(genelToplam, detay);
                }
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            //Son toplamı yazıyor
            DetayYaz(XLS, satir, sutun, 0, cd);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -1, hesapToplam);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            DetayYaz(XLS, satir, sutun, -2, genelToplam);

            TasinirHesapCetveliImzaEkle(XLS, ref satir, sutun);
            OrnekNoYaz(XLS, ++satir, sutun, 15);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }
        /// <summary>
        /// Taşınır hesap cetveli excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">İmza bilgilerinin yazılmaya başlanacağı satır numarası</param>
        /// <param name="sutun">İmza bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        private void TasinirHesapCetveliImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 1;

            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", (int)ENUMImzaYer.TASINIRKONSOLIDEGOREVLISI);

            ImzaBilgisi iBilgi = new ImzaBilgisi();
            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
                iBilgi = (ImzaBilgisi)imza[0];

            XLS.SatirAc(satir, 7);
            XLS.HucreKopyala(0, sutun, 6, sutun + 14, satir, sutun);

            XLS.HucreDegerYaz(satir + 1, sutun + 2, string.Format(Resources.TasinirMal.FRMCTV041, DateTime.Today.Date.ToShortDateString()));
            XLS.HucreDegerYaz(satir + 2, sutun + 5, Resources.TasinirMal.FRMCTV042);
            XLS.HucreDegerYaz(satir + 3, sutun + 5, string.Format(Resources.TasinirMal.FRMCTV043, iBilgi.adSoyad));
            XLS.HucreDegerYaz(satir + 4, sutun + 5, string.Format(Resources.TasinirMal.FRMCTV044, iBilgi.unvan));
            XLS.HucreDegerYaz(satir + 5, sutun + 5, Resources.TasinirMal.FRMCTV045);

            XLS.HucreBirlestir(satir + 1, sutun + 2, satir + 1, sutun + 12);

            for (int i = satir + 2; i <= satir + 5; i++)
            {
                XLS.HucreBirlestir(i, sutun + 5, i, sutun + 9);

                if (i == satir + 2)
                {
                    XLS.KoyuYap(i, sutun + 5, true);
                    XLS.DuseyHizala(i, sutun + 5, 2);
                    XLS.YatayHizala(i, sutun + 5, 2);
                }
                else
                    XLS.DuseyHizala(i, sutun + 5, 0);
            }

            XLS.YatayCizgiCiz(satir + 7, sutun, sutun + 14, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 6, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 6, sutun + 15, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 6;
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki taşınır kesin hesap cetveli yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        /// <param name="csvFormati">if set to <c>true</c> [CSV formati].</param>
        private void TasinirKesinHesapCetveli(Cetvel kriter, bool csvFormati)
        {
            ObjectArray bilgi = servisTMM.TasinirKesinHesapCetveli(kullanan, kriter, csvFormati);
            //TasinirKesinHesapCetveliVEIcmal(bilgi, false, "TASINIRKESINHESAPCETVELI.xlt");

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

            string kurKod = string.Empty;
            string kurAd = string.Empty;
            GenelIslemlerIstemci.VarsayilanKurumBul(out kurKod, out kurAd);
            string sonucDosyaAd = System.IO.Path.GetTempFileName();

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];
            CetvelDetay cd = new CetvelDetay();
            CetvelDetay genelToplam = new CetvelDetay();
            CetvelDetay hesapToplam = new CetvelDetay();

            if (csvFormati)
            {
                StringBuilder rapor = new StringBuilder();

                for (int i = 0; i < cetvel.detay.Count; i++)
                {
                    CetvelDetay detay = (CetvelDetay)cetvel.detay[i];
                    rapor.AppendLine(DetayYazCSV(detay, kriter.yil));
                }

                OrtakFonksiyonlar.DosyaYaz(sonucDosyaAd, rapor.ToString(), true);
                DosyaIslem.DosyaGonder(sonucDosyaAd, "TasinirKesinHesap.csv", true, "txt");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sablonAd = "TASINIRKESINHESAPCETVELI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            string eskiHesapKod = string.Empty;

            int siraNo = 0;
            for (int i = 0; i < cetvel.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                siraNo++;

                if (!detay.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                {
                    siraNo = 1;


                    if (i != 0)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                        //Toplam yazıyor burada
                        DetayYaz(XLS, satir, sutun, 0, cd);
                        cd = new CetvelDetay();

                        if (eskiHesapKod.Substring(0, 3) != detay.hesapPlanKod.Substring(0, 3) && !string.IsNullOrEmpty(eskiHesapKod))
                        {
                            satir++;
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                            DetayYaz(XLS, satir, sutun, -1, hesapToplam);
                            hesapToplam = new CetvelDetay();
                        }

                        TasinirKesinHesapCetveliImzaEkle(XLS, ref satir, sutun);
                        OrnekNoYaz(XLS, ++satir, sutun, 16);

                        XLS.SayfaSonuKoyHucresel(satir + 3);
                        satir += 3;
                    }

                    eskiHesapKod = detay.hesapPlanKod.Substring(0, 6);

                    XLS.SatirAc(satir, 6);
                    XLS.HucreKopyala(kaynakSatir - 6, sutun, kaynakSatir - 1, sutun + 14, satir, sutun);

                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);

                    XLS.HucreBirlestir(satir + 2, sutun, satir + 2, sutun + 1);
                    XLS.HucreBirlestir(satir + 2, sutun + 3, satir + 2, sutun + 5);
                    XLS.HucreBirlestir(satir + 2, sutun + 7, satir + 2, sutun + 8);
                    XLS.HucreBirlestir(satir + 2, sutun + 10, satir + 2, sutun + 11);
                    XLS.HucreBirlestir(satir + 2, sutun + 13, satir + 2, sutun + 14);

                    YTLYaz(XLS, satir + 1, sutun + 14);

                    if (string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        XLS.HucreDegerYaz(satir + 2, sutun + 3, kurAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 7, kurKod);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir + 2, sutun + 3, kriter.harcamaAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 7, kriter.harcamaKod);
                    }

                    XLS.HucreDegerYaz(satir + 2, sutun + 10, eskiHesapKod);
                    XLS.HucreDegerYaz(satir + 2, sutun + 13, txtYil.Text);

                    XLS.HucreBirlestir(satir + 4, sutun, satir + 5, sutun);
                    XLS.HucreBirlestir(satir + 4, sutun + 1, satir + 5, sutun + 1);
                    XLS.HucreBirlestir(satir + 4, sutun + 2, satir + 5, sutun + 3);
                    XLS.HucreBirlestir(satir + 4, sutun + 4, satir + 5, sutun + 4);

                    for (int j = sutun + 5; j < sutun + 14; j += 2)
                        XLS.HucreBirlestir(satir + 4, j, satir + 4, j + 1);

                    satir += 5;
                }

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);

                //Detay yazıyorum
                DetayYaz(XLS, satir, sutun, siraNo, detay);
                DetayTopla(cd, detay);
                DetayTopla(hesapToplam, detay);
                DetayTopla(genelToplam, detay);
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            //Son toplamı yazıyorum
            DetayYaz(XLS, satir, sutun, 0, cd);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            //Son toplamı yazıyorum
            DetayYaz(XLS, satir, sutun, -1, hesapToplam);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
            //Son toplamı yazıyorum
            DetayYaz(XLS, satir, sutun, -2, genelToplam);

            TasinirKesinHesapCetveliImzaEkle(XLS, ref satir, sutun);
            OrnekNoYaz(XLS, ++satir, sutun, 16);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki müze/kütüphane yönetim hesabı cetveli
        /// yordamına gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        private void MuzeKutuphaneCetveli(Cetvel kriter)
        {
            ObjectArray bilgi = servisTMM.MuzeKutuphaneCetveli(kullanan, kriter);

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

            string kurKod = string.Empty;
            string kurAd = string.Empty;
            GenelIslemlerIstemci.VarsayilanKurumBul(out kurKod, out kurAd);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "MUZEKUTUPHANEYONETIMHESABICETVELI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            string eskiHesapKod = string.Empty;

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];
            CetvelDetay cd = new CetvelDetay();

            int siraNo = 0;
            for (int i = 0; i < cetvel.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                siraNo++;

                if (!detay.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                {
                    siraNo = 1;

                    eskiHesapKod = detay.hesapPlanKod.Substring(0, 6);

                    if (i != 0)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 18, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

                        //Toplam yazıyor burada
                        DetayYazMuzeKutuphane(XLS, satir, sutun, 0, cd);
                        cd = new CetvelDetay();

                        MuzeKutuphaneCetveliImzaEkle(XLS, ref satir, sutun);
                        OrnekNoYaz(XLS, ++satir, sutun, 18);

                        XLS.SayfaSonuKoyHucresel(satir + 3);
                        satir += 3;
                    }

                    XLS.SatirAc(satir, 7);
                    XLS.HucreKopyala(kaynakSatir - 7, sutun, kaynakSatir - 1, sutun + 18, satir, sutun);
                    XLS.SatirYukseklikAyarla(satir, satir + 5, GenelIslemler.JexcelBirimtoExcelBirim(400));
                    XLS.SatirYukseklikAyarla(satir + 6, satir + 6, GenelIslemler.JexcelBirimtoExcelBirim(1000));

                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 18);

                    XLS.HucreBirlestir(satir + 2, sutun, satir + 2, sutun + 1);
                    XLS.HucreBirlestir(satir + 2, sutun + 3, satir + 2, sutun + 7);
                    XLS.HucreBirlestir(satir + 2, sutun + 9, satir + 2, sutun + 11);
                    XLS.HucreBirlestir(satir + 2, sutun + 12, satir + 2, sutun + 13);
                    XLS.HucreBirlestir(satir + 2, sutun + 14, satir + 2, sutun + 16);

                    YTLYaz(XLS, satir + 1, sutun + 18);

                    if (string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        XLS.HucreDegerYaz(satir + 2, sutun + 3, kurAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 9, kurKod);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir + 2, sutun + 3, kriter.harcamaAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 9, kriter.harcamaKod);
                    }

                    XLS.HucreDegerYaz(satir + 2, sutun + 14, eskiHesapKod);
                    XLS.HucreDegerYaz(satir + 2, sutun + 18, txtYil.Text);

                    XLS.HucreBirlestir(satir + 4, sutun, satir + 6, sutun);
                    XLS.HucreBirlestir(satir + 4, sutun + 1, satir + 6, sutun + 1);
                    XLS.HucreBirlestir(satir + 4, sutun + 2, satir + 6, sutun + 3);

                    for (int j = sutun + 4; j < sutun + 18; j += 3)
                    {
                        XLS.HucreBirlestir(satir + 4, j, satir + 4, j + 2);
                        XLS.HucreBirlestir(satir + 5, j, satir + 5, j + 1);
                        XLS.HucreBirlestir(satir + 5, j + 2, satir + 6, j + 2);
                    }

                    satir += 6;
                }

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 18, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

                //Detay yazıyorum
                DetayYazMuzeKutuphane(XLS, satir, sutun, siraNo, detay);
                DetayTopla(cd, detay);
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 18, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            //Son toplamı yazıyorum
            DetayYazMuzeKutuphane(XLS, satir, sutun, 0, cd);

            MuzeKutuphaneCetveliImzaEkle(XLS, ref satir, sutun);
            OrnekNoYaz(XLS, ++satir, sutun, 18);

            //XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Müze/Kütüphane yönetim hesabı cetveli excel raporuna cetvel detay satır bilgilerini yazan yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">Cetvel detay bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Cetvel detay bilgilerinin yazılacağı sütun numarası</param>
        /// <param name="siraNo">Cetvel detayının rapordaki sıra numarası</param>
        /// <param name="detay">Cetvel detay bilgilerini tutan nesne</param>
        private void DetayYazMuzeKutuphane(Tablo XLS, int satir, int sutun, int siraNo, CetvelDetay detay)
        {
            if (siraNo != 0)
            {
                XLS.HucreDegerYaz(satir, sutun, siraNo.ToString());
                XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
            }
            else
            {
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
                XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMCTV034);

                for (int m = sutun; m <= sutun + 18; m++)
                    XLS.KoyuYap(satir, m, true);
            }

            XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(detay.gecenYildanDevreden.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.gecenYildanDevreden.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeGiren.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeGiren.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(detay.toplam.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(detay.toplam.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeCikan.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDouble(detay.yilIcindeCikan.tutar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 17, OrtakFonksiyonlar.ConvertToDouble(detay.gelecekYilaDevir.miktar.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 18, OrtakFonksiyonlar.ConvertToDouble(detay.gelecekYilaDevir.tutar.ToString(), (double)0));
        }

        /// <summary>
        /// Parametre olarak verilen cetvel kriterlerini sunucudaki taşınır kesin hesap icmal cetveli
        /// yordamına gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Cetvel kriter bilgilerini tutan nesne</param>
        /// <param name="csvFormati">if set to <c>true</c> [CSV formati].</param>
        private void TasinirKesinHesapIcmalCetveli(Cetvel kriter, bool csvFormati)
        {
            ObjectArray bilgi = servisTMM.TasinirKesinHesapIcmalCetveli(kullanan, kriter, csvFormati);
            //TasinirKesinHesapCetveliVEIcmal(bilgi, true, "TASINIRKESINHESAPICMALCETVELI.xlt");

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

            string kurKod = string.Empty;
            string kurAd = string.Empty;
            GenelIslemlerIstemci.VarsayilanKurumBul(out kurKod, out kurAd);
            string sonucDosyaAd = System.IO.Path.GetTempFileName();

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];
            CetvelDetay cd = new CetvelDetay();
            CetvelDetay genelToplam = new CetvelDetay();

            if (csvFormati)
            {
                StringBuilder rapor = new StringBuilder();

                for (int i = 0; i < cetvel.detay.Count; i++)
                {
                    CetvelDetay detay = (CetvelDetay)cetvel.detay[i];
                    rapor.AppendLine(IcmalDetayYazCSV(detay, kriter.yil));
                }

                OrtakFonksiyonlar.DosyaYaz(sonucDosyaAd, rapor.ToString(), true);
                DosyaIslem.DosyaGonder(sonucDosyaAd, "TasinirKesinHesapIcmal.csv", true, "txt");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sablonAd = "TASINIRKESINHESAPICMALCETVELI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            string eskiHesapKod = string.Empty;


            for (int i = 0; i < cetvel.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                if (!detay.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                {
                    if (i != 0)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
                        for (int j = sutun + 4; j < sutun + 13; j += 2)
                            XLS.HucreBirlestir(satir, j, satir, j + 1);

                        //Toplam yazıyor burada
                        IcmalDetayYaz(XLS, satir, sutun, cd, true, false);
                        cd = new CetvelDetay();

                        TasinirKesinHesapIcmalCetveliImzaEkle(XLS, ref satir, sutun);
                        OrnekNoYaz(XLS, ++satir, sutun, 17);

                        XLS.SayfaSonuKoyHucresel(satir + 3);
                        satir += 3;
                    }

                    eskiHesapKod = detay.hesapPlanKod.Substring(0, 3);

                    XLS.SatirAc(satir, 6);
                    XLS.HucreKopyala(kaynakSatir - 6, sutun, kaynakSatir - 1, sutun + 13, satir, sutun);

                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 13);

                    XLS.HucreBirlestir(satir + 2, sutun + 2, satir + 2, sutun + 6);
                    XLS.HucreBirlestir(satir + 2, sutun + 9, satir + 2, sutun + 10);

                    YTLYaz(XLS, satir + 1, sutun + 13);

                    if (string.IsNullOrEmpty(kriter.harcamaKod))
                    {
                        XLS.HucreDegerYaz(satir + 2, sutun + 2, kurAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 8, kurKod);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir + 2, sutun + 2, kriter.harcamaAd);
                        XLS.HucreDegerYaz(satir + 2, sutun + 8, kriter.harcamaKod);
                    }

                    XLS.HucreDegerYaz(satir + 2, sutun + 11, eskiHesapKod);
                    XLS.HucreDegerYaz(satir + 2, sutun + 13, txtYil.Text);

                    XLS.HucreBirlestir(satir + 4, sutun, satir + 5, sutun);
                    XLS.HucreBirlestir(satir + 4, sutun + 1, satir + 5, sutun + 3);

                    for (int j = sutun + 4; j < sutun + 13; j += 2)
                        XLS.HucreBirlestir(satir + 4, j, satir + 5, j + 1);

                    satir += 5;
                }

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 3);
                for (int j = sutun + 4; j < sutun + 13; j += 2)
                    XLS.HucreBirlestir(satir, j, satir, j + 1);

                //Detay yazıyorum
                IcmalDetayYaz(XLS, satir, sutun, detay, false, false);
                DetayTopla(cd, detay);
                DetayTopla(genelToplam, detay);
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
            for (int j = sutun + 4; j < sutun + 13; j += 2)
                XLS.HucreBirlestir(satir, j, satir, j + 1);
            //Son toplamı yazıyorum
            IcmalDetayYaz(XLS, satir, sutun, cd, true, false);

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
            for (int j = sutun + 4; j < sutun + 13; j += 2)
                XLS.HucreBirlestir(satir, j, satir, j + 1);
            //Son toplamı yazıyorum
            IcmalDetayYaz(XLS, satir, sutun, genelToplam, true, true);

            TasinirKesinHesapIcmalCetveliImzaEkle(XLS, ref satir, sutun);
            OrnekNoYaz(XLS, ++satir, sutun, 17);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Taşınır kesin hesap cetveli excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">İmza bilgilerinin yazılmaya başlanacağı satır numarası</param>
        /// <param name="sutun">İmza bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        private void TasinirKesinHesapCetveliImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 1;

            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", (int)ENUMImzaYer.MERKEZDEKITASINIRKONSOLIDEGOREVLISI);

            ImzaBilgisi iBilgi = new ImzaBilgisi();
            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
                iBilgi = (ImzaBilgisi)imza[0];

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 5, sutun + 14, satir, sutun);

            XLS.HucreDegerYaz(satir + 1, sutun + 5, Resources.TasinirMal.FRMCTV046);
            XLS.HucreDegerYaz(satir + 2, sutun + 5, string.Format(Resources.TasinirMal.FRMCTV011, iBilgi.adSoyad));
            XLS.HucreDegerYaz(satir + 3, sutun + 5, Resources.TasinirMal.FRMGAG007);
            XLS.HucreDegerYaz(satir + 4, sutun + 5, string.Format(Resources.TasinirMal.FRMCTV024, DateTime.Today.Date.ToShortDateString()));

            for (int i = satir + 1; i < satir + 5; i++)
            {
                XLS.HucreBirlestir(i, sutun + 5, i, sutun + 9);

                if (i == satir + 1)
                {
                    XLS.KoyuYap(i, sutun + 5, true);
                    XLS.DuseyHizala(i, sutun + 5, 2);
                    XLS.YatayHizala(i, sutun + 5, 2);
                }
                else
                    XLS.DuseyHizala(i, sutun + 5, 0);
            }

            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 14, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 15, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 5;
        }

        /// <summary>
        /// Taşınır kesin hesap icmal cetveli excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">İmza bilgilerinin yazılmaya başlanacağı satır numarası</param>
        /// <param name="sutun">İmza bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        private void TasinirKesinHesapIcmalCetveliImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 1;

            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", 0);

            string[] ad = new string[3];
            string[] unvan = new string[3];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.MERKEZDEKITASINIRKONSOLIDEGOREVLISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    //else if (iBilgi.imzaYer == (int)ENUMImzaYer.SAYIMKURULUBASKANI && string.IsNullOrEmpty(ad[1]))
                    //{
                    //    ad[1] = iBilgi.adSoyad;
                    //    unvan[1] = iBilgi.unvan;
                    //}
                    //else if (iBilgi.imzaYer == (int)ENUMImzaYer.SAYIMKURULUUYE1 && string.IsNullOrEmpty(ad[2]))
                    //{
                    //    ad[2] = iBilgi.adSoyad;
                    //    unvan[2] = iBilgi.unvan;
                    //}
                }
            }

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 5, sutun + 13, satir, sutun);

            XLS.HucreDegerYaz(satir + 1, sutun + 2, Resources.TasinirMal.FRMCTV046);
            XLS.HucreDegerYaz(satir + 1, sutun + 5, Resources.TasinirMal.FRMCTV048);
            XLS.HucreDegerYaz(satir + 1, sutun + 10, Resources.TasinirMal.FRMCTV049);

            XLS.HucreDegerYaz(satir + 2, sutun + 1, Resources.TasinirMal.FRMCTV050);
            XLS.HucreDegerYaz(satir + 3, sutun + 1, Resources.TasinirMal.FRMCTV051);
            XLS.HucreDegerYaz(satir + 4, sutun + 1, Resources.TasinirMal.FRMCTV052);

            XLS.HucreDegerYaz(satir + 2, sutun + 2, ad[0]);
            XLS.HucreDegerYaz(satir + 3, sutun + 2, Resources.TasinirMal.FRMCTV027);
            XLS.HucreDegerYaz(satir + 4, sutun + 2, DateTime.Today.Date.ToShortDateString());

            XLS.HucreDegerYaz(satir + 2, sutun + 5, Resources.TasinirMal.FRMCTV027);
            XLS.HucreDegerYaz(satir + 3, sutun + 5, Resources.TasinirMal.FRMCTV027);
            XLS.HucreDegerYaz(satir + 4, sutun + 5, DateTime.Today.Date.ToShortDateString());

            XLS.HucreDegerYaz(satir + 2, sutun + 10, Resources.TasinirMal.FRMCTV027);
            XLS.HucreDegerYaz(satir + 3, sutun + 10, Resources.TasinirMal.FRMCTV027);
            XLS.HucreDegerYaz(satir + 4, sutun + 10, DateTime.Today.Date.ToShortDateString());

            //for (int i = satir + 2; i < satir + 5; i++)
            //{
            //    XLS.HucreBirlestir(i, sutun + 1, i, sutun + 2);
            //    XLS.KoyuYap(i, sutun + 1, true);
            //    XLS.DuseyHizala(i, sutun + 1, 0);
            //    XLS.YatayHizala(i, sutun + 1, 2);
            //}

            for (int i = satir + 1; i < satir + 5; i++)
            {
                for (int j = sutun + 2; j <= sutun + 8; j += 3)
                {
                    if (j == sutun + 8)
                        j += 2;

                    if (j == sutun + 2)
                        XLS.HucreBirlestir(i, j, i, j + 1);
                    else
                        XLS.HucreBirlestir(i, j, i, j + 3);

                    if (i == satir + 1)
                    {
                        XLS.KoyuYap(i, j, true);
                        XLS.DuseyHizala(i, j, 2);
                        XLS.YatayHizala(i, j, 2);
                    }
                }
            }

            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 13, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 14, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 5;
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

    }
}