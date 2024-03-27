using Ext1.Net;
using OrtakClass;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fi�i bilgilerinin kay�t, listeleme, onaylama, onay kald�rma ve raporlama i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class ZimmetTransfer : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        /// <summary>
        /// Ortak alan zimmet fi�i mi yoksa ki�i zimmet fi�i mi oldu�unu tutan de�i�ken
        /// </summary>
        static string belgeTuru;

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa adresinde belgeTur girdi dizgisi dolu de�ilse hata verir
        ///     ve sayfay� y�klemez, dolu ise sayfada baz� ayarlamalar yap�l�r.
        ///     �lk y�kleni�te, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Zimmet Transfer";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtBelgeTarihi.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    btnZimmetListele.Hidden = true; //M.Bankas� b�t�n demirba�lar� listeleyip yanl��l�la hepsini zimmetten d���yorlar. H.�

                    txtKimeVerildi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ZIMMETKISI");
                    txtPersonel.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ZIMMETKISI");

                    txtPersonel.Listeners.ClearListeners();
                    txtPersonel.Triggers.RemoveAt(0);
                }


                hdnBelgeTur.Value = Request.QueryString["Tur"] + "";

                ListeTemizle();
            }
        }

        /// <summary>
        /// Belge Kaydet tu�una bas�l�nca �al��an olay metodu
        /// Zimmet fi�i bilgileri ekrandaki ilgili kontrollerden toplan�p kaydedilmek
        /// �zere sunucuya g�nderilir, gelen sonuca g�re hata mesaj� veya bilgi mesaj� verilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            if (hdnIslemTur.Text == "1" && !chkSadeceDusme.Checked)
            {
                GenelIslemler.MesajKutusu("Uyar�", "Sicil No ile sadece d��me i�lemi yap�labilir.");
                return;
            }

            if (hdnIslemTur.Text == "1")
            {
                SicilNoyaGoreIslemYap();
                return;
            }

            if (!chkSadeceDusme.Checked && txtKimeVerildi.Text == "")
            {
                GenelIslemler.MesajKutusu("Uyar�", "Kime Verilecek alan� bo� b�rak�lamaz");
                return;
            }

            GenelIslemler.KullaniciDegiskenSakla(kullanan, "ZIMMETKISI", txtPersonel.Text.Trim());

            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            Sonuc sonuc = new Sonuc();
            TNS.TMM.ZimmetForm zimmet = new TNS.TMM.ZimmetForm();
            string belgeTuru = hdnBelgeTur.Value.ToString();

            zimmet.yil = new TNSDateTime(txtBelgeTarihi.Text).Oku().Year;
            zimmet.fisTarih = new TNSDateTime(txtBelgeTarihi.Text);
            zimmet.fisNo = "";
            zimmet.muhasebeKod = txtMuhasebe.Text;
            zimmet.harcamaBirimKod = txtHarcamaBirimi.Text;
            zimmet.ambarKod = txtAmbar.Text;
            //zimmet.tip = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlBelgeTipi), 0);
            zimmet.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME;
            zimmet.kimeGitti = txtPersonel.Text.Trim();
            zimmet.nereyeGitti = txtNereyeVerildi.Text.Trim();
            zimmet.belgeTur = belgeTuru == "1" ? (int)ENUMZimmetBelgeTur.ZIMMETFISI : belgeTuru == "2" ? (int)ENUMZimmetBelgeTur.DAYANIKLITL : (int)ENUMZimmetBelgeTur.BELIRSIZ;

            ObjectArray zimmetDetayArray = new ObjectArray();

            int siraNo = 1;
            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                ZimmetDetay zimmetDetay = new ZimmetDetay();
                zimmetDetay.yil = zimmet.yil;
                zimmetDetay.muhasebeKod = zimmet.muhasebeKod;
                zimmetDetay.harcamaBirimKod = zimmet.harcamaBirimKod.Replace(".", "");
                zimmetDetay.belgeTur = zimmet.belgeTur;

                zimmetDetay.hesapPlanKod = TasinirGenel.DegerAlStr(item, "TASINIRHESAPKOD");
                zimmetDetay.gorSicilNo = TasinirGenel.DegerAlStr(item, "SICILNO");
                zimmetDetay.ozellik = TasinirGenel.DegerAlStr(item, "ACIKLAMA");
                zimmetDetay.siraNo = siraNo;
                zimmetDetay.kdvOran = TasinirGenel.DegerAlInt(item, "KDVORANI");
                zimmetDetay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(TasinirGenel.DegerAlDbl(item, "BIRIMFIYATI"));
                zimmetDetay.teslimDurum = TasinirGenel.DegerAlStr(item, "TESLIMEDILMEANINDADURUMU");
                zimmetDetay.prSicilNo = TasinirGenel.DegerAlInt(item, "PRSICILNO");

                if (zimmetDetay.prSicilNo == 0 || zimmetDetay.gorSicilNo == "") continue;

                siraNo++;
                zimmetDetayArray.Ekle(zimmetDetay);
            }

            zimmet.islemTarih = new TNSDateTime(DateTime.Now);
            zimmet.islemYapan = kullanan.kullaniciKodu;
            zimmet.tip = (int)ENUMZimmetTipi.DEMIRBASCIHAZ;

            //D��me fi�ini kaydet
            sonuc = servisTMM.ZimmetFisiKaydet(kullanan, zimmet, zimmetDetayArray);
            string dusmeFisNo = sonuc.anahtar;

            //Kayit edilen d��me fi�ini onayla
            if (sonuc.islemSonuc)
            {
                zimmet.fisNo = dusmeFisNo;
                sonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, zimmet, "Onay");
            }
            else
            {
                GenelIslemler.MesajKutusu("Hata", "D��me i�lemi s�ras�nda bir hata olu�tu.<br>Hata:" + sonuc.hataStr);
                return;
            }

            string islemAciklama = "Sadece d��me i�lemi";
            string vermeFisNo = "";
            if (!chkSadeceDusme.Checked)
            {
                zimmet.fisNo = "";
                zimmet.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETVERME;
                zimmet.kimeGitti = txtKimeVerildi.Text.Trim();
                zimmet.nereyeGitti = txtNereyeVerildi.Text.Trim();
                zimmet.dusmeTarih = zimmet.fisTarih;

                //Verme Fi�ini Kaydet
                sonuc = servisTMM.ZimmetFisiKaydet(kullanan, zimmet, zimmetDetayArray);
                vermeFisNo = sonuc.anahtar;

                //Kayit edilen verme fi�ini onayla
                if (sonuc.islemSonuc)
                {
                    zimmet.fisNo = vermeFisNo;
                    sonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, zimmet, "Onay");
                }
                else
                {
                    GenelIslemler.MesajKutusu("Hata", "Verme i�lemi s�ras�nda bir hata olu�tu.<br>Hata:" + sonuc.hataStr);
                    return;
                }
                islemAciklama = "Transfer i�lemi";
            }

            string mesaj = islemAciklama + " ba�ar�yla ger�ekle�ti.<br><br><br>D��me Belge Numaras�:" + dusmeFisNo;
            if (!string.IsNullOrEmpty(vermeFisNo))
                mesaj += "<br>Verme Belge Numaras�:" + vermeFisNo;

            GenelIslemler.MesajKutusu("Bilgi", mesaj);

            X.AddScript("parent.hidePopWin();");
        }

        private void SicilNoyaGoreIslemYap()
        {
            var sicilNolar = txtSicilNolar.Text;
            string[] sicilNo = sicilNolar.Split('\n');

            ObjectArray zListe = new ObjectArray();
            string islemSonuc = "";

            foreach (string sc in sicilNo)
            {
                if (sc.Trim() == "") continue;

                TNS.TMM.ZimmetForm zForm = servisTMM.ZimmetVarMi(kullanan, sc);

                zForm.fisNo = "";
                zForm.fisTarih = new TNSDateTime(txtBelgeTarihi.Text);
                zForm.yil = zForm.fisTarih.Yil;
                zForm.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME;
                zForm.islemTarih = new TNSDateTime(DateTime.Now);
                zForm.islemYapan = kullanan.kullaniciKodu;

                if (zForm.sorguDetay.objeler.Count > 0)
                {
                    string sNo = ((TNS.TMM.ZimmetDetay)zForm.sorguDetay[0]).gorSicilNo;
                    if (sNo != "")
                        zListe.Ekle(zForm);
                    else
                        islemSonuc += $"UYARI: Sicil No:{sc} Z�MMET KAYDI BULUNAMADI<br>";
                }
                else
                    islemSonuc += $"UYARI: Sicil No:{sc} Z�MMET KAYDI BULUNAMADI<br>";
            }

            if (zListe.ObjeSayisi == 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "��lem yap�lacak kay�t bulunamad�");

                X.AddScript("parent.hidePopWin();");
                return;
            }

            foreach (TNS.TMM.ZimmetForm item in zListe.objeler)
            {
                foreach (TNS.TMM.ZimmetForm item2 in zListe.objeler)
                {
                    if (item.yil == item2.yil &&
                        item.muhasebeKod == item2.muhasebeKod &&
                        item.harcamaBirimKod == item2.harcamaBirimKod &&
                        item.ambarKod == item2.ambarKod &&
                        item.kimeGitti == item2.kimeGitti &&
                        item.nereyeGitti == item2.nereyeGitti &&
                        item.tip == item2.tip &&
                        item.belgeTur == item2.belgeTur)
                    {
                        if (item2.sorguDetay.objeler.Count > 1) continue;

                        TNS.TMM.ZimmetDetay zd = new ZimmetDetay();

                        foreach (TNS.TMM.ZimmetDetay ditem2 in item2.sorguDetay.objeler)
                        {
                            if (ditem2.rfIdNo == 99) continue;

                            bool bulundu = false;
                            foreach (TNS.TMM.ZimmetDetay ditem in item.sorguDetay.objeler)
                            {
                                if (ditem.rfIdNo != 99 && ditem.gorSicilNo == ditem2.gorSicilNo)
                                {
                                    bulundu = true;
                                    break;
                                }
                            }
                            if (bulundu) continue;

                            zd = new ZimmetDetay();
                            zd = ditem2;
                            item.sorguDetay.Ekle(zd);

                            ditem2.rfIdNo = 99;
                        }
                    }
                }


            }


            Sonuc sonuc = new Sonuc();

            foreach (TNS.TMM.ZimmetForm item in zListe.objeler)
            {
                string sNo = ((TNS.TMM.ZimmetDetay)item.sorguDetay[0]).gorSicilNo;
                if (sNo.Trim() == "") continue;

                int rfId = ((TNS.TMM.ZimmetDetay)item.sorguDetay[0]).rfIdNo;//Ba�ka zimmetforma aktar�ld� gruplanmak i�in
                if (rfId == 99) continue;

                foreach (TNS.TMM.ZimmetDetay ditem in item.sorguDetay.objeler)
                {
                    ditem.yil = item.yil;
                    ditem.muhasebeKod = item.muhasebeKod;
                    ditem.harcamaBirimKod = item.harcamaBirimKod;
                    ditem.ambarKod = item.ambarKod;
                    ditem.belgeTur = item.belgeTur;
                }

                //D��me fi�ini kaydet
                sonuc = servisTMM.ZimmetFisiKaydet(kullanan, item, item.sorguDetay);
                string dusmeFisNo = sonuc.anahtar;

                //Kayit edilen d��me fi�ini onayla
                if (sonuc.islemSonuc)
                {
                    item.fisNo = dusmeFisNo;
                    sonuc = servisTMM.ZimmetFisiDurumDegistir(kullanan, item, "Onay");

                    if (sonuc.islemSonuc)
                        islemSonuc += $"TAMAM: Sicil No:{sNo} i�in {dusmeFisNo} nolu zimmet fi�i �retildi ve onayland�<br>";
                    else
                        islemSonuc += $"UYARI: Sicil No:{sNo} i�in {dusmeFisNo} nolu zimmet fi�i �retildi fakat ONAYLANAMADI. Hata:{sonuc.hataStr} <br>";
                }
                else
                    islemSonuc += $"UYARI: Sicil No:{sNo} i�in zimmet fi�i �RET�LEMED�, Hata:{sonuc.hataStr}<br>";
            }

            string islemAciklama = "D��me i�lemi";

            string mesaj = islemAciklama + " ba�ar�yla ger�ekle�ti.<br><br><br>" + islemSonuc;

            GenelIslemler.MesajKutusu("Bilgi", mesaj);

            X.AddScript("parent.hidePopWin();");
        }

        protected void btnZimmetListele_Click(object sender, DirectEventArgs e)
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.yil = new TNSDateTime(txtBelgeTarihi.Text).Oku().Year;
            shBilgi.muhasebeKod = txtMuhasebe.Text;
            shBilgi.harcamaBirimKod = txtHarcamaBirimi.Text;
            shBilgi.ambarKod = txtAmbar.Text;
            shBilgi.kimeGitti = txtPersonel.Text;
            shBilgi.nereyeGitti = txtNereden.Text;
            shBilgi.hesapPlanKod = txtHesapKodu.Text.Trim();

            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
            int sinir = 400;
            if (bilgi.objeler.Count > sinir)
            {
                string mesaj = "Ki�i �zerinde <b>" + bilgi.objeler.Count + " adet malzeme bulunmaktad�r.</b> �lk " + sinir + " adet malzeme a�a��da listelenmi�tir. Bu malzemelerin listelenmesi i�in l�tfen filtreme yap�n�z.";
                mesaj += "<br>Malzemelerin hesap koduna g�re da��l�m� �u �ekildedir,<br>";
                Hashtable ht = new Hashtable();
                foreach (SicilNoHareket dt in bilgi.objeler)
                {
                    string hesapKod = dt.hesapPlanKod.Substring(0, 9);
                    ht[hesapKod] = OrtakFonksiyonlar.ConvertToInt(ht[hesapKod], 0) + 1;
                }
                foreach (DictionaryEntry di in ht)
                {
                    string hesapKod = (string)di.Key;
                    int adet = (int)di.Value;
                    mesaj += hesapKod + " hesap kodlu malzeme " + adet + " adet<br>";
                }

                GenelIslemler.MesajKutusu("Uyar�", mesaj);
            }

            List<object> liste = new List<object>();
            int sayac = 0;
            foreach (SicilNoHareket dt in bilgi.objeler)
            {
                if (sayac > sinir) break;
                sayac++;
                liste.Add(new
                {
                    FISNO = dt.fisNo,
                    TASINIRHESAPKOD = dt.hesapPlanKod,
                    SICILNO = dt.sicilNo,
                    ACIKLAMA = dt.ozellik,
                    TASINIRHESAPADI = dt.hesapPlanAd,
                    KDVORANI = dt.kdvOran,
                    BIRIMFIYATI = OrtakFonksiyonlar.ConvertToDbl(dt.fiyat),
                    TESLIMEDILMEANINDADURUMU = "",
                    PRSICILNO = dt.prSicilNo,
                    DISSICILNO = dt.ozellik.disSicilNo + (dt.ozellik.disSicilNo2 != "" ? " - " + dt.ozellik.disSicilNo2 : ""),
                    BULUNDUGUYER = dt.ozellik.bulunduguYerAd
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            ListeTemizle();
            txtBelgeTarihi.Clear();
            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            txtNereyeVerildi.Clear();
            lblNereyeVerildi.Text = "";
            txtKimeVerildi.Clear();
            lblKimeVerildi.Text = "";

            if (txtMuhasebe.Text == "") lblMuhasebeAd.Text = "";
            if (txtHarcamaBirimi.Text == "") lblHarcamaBirimiAd.Text = "";
            if (txtAmbar.Text == "") lblAmbarAd.Text = "";
        }

        private void ListeTemizle()
        {
            List<object> liste = new List<object>();
            for (int i = 0; i < 15; i++)
            {
                liste.Add(new
                {
                    KOD = ""
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }


        protected void SicilNoStore_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Parameters["query"]))
                return;

            List<object> liste = SicilNoDoldur(e.Parameters["query"]);

            e.Total = 0;
            if (liste != null && liste.Count != 0)
            {
                var limit = e.Limit;
                if ((e.Start + e.Limit) > liste.Count)
                    limit = liste.Count - e.Start;

                e.Total = liste.Count;
                List<object> rangeData = (e.Start < 0 || limit < 0) ? liste : liste.GetRange(e.Start, limit);
                strSicilNo.DataSource = (object[])rangeData.ToArray();
                strSicilNo.DataBind();
            }
            else
            {
                strSicilNo.DataSource = new object[] { };
                strSicilNo.DataBind();
            }
        }

        List<object> SicilNoDoldur(string kriter)
        {
            SicilNoHareket h = new SicilNoHareket();
            h.muhasebeKod = txtMuhasebe.Text;
            h.harcamaBirimKod = txtHarcamaBirimi.Text;
            h.sorguButunSicilNolardaAra = kriter;
            ObjectArray hesap = servisTMM.BarkodSicilNoListele(kullanan, h, new Sayfalama());

            List<object> liste = new List<object>();
            foreach (SicilNoHareket detay in hesap.objeler)
            {
                string eskiSicilNo = detay.ozellik.disSicilNo;
                if (detay.ozellik.eskiBisNo1 != "") eskiSicilNo += "-" + detay.ozellik.eskiBisNo1;

                liste.Add(new
                {
                    PRSICILNO = detay.prSicilNo,
                    SICILNO = detay.sicilNo,
                    ESICILNO = eskiSicilNo,
                    FIYAT = detay.fiyat,
                    TASINIRHESAPKOD = detay.hesapPlanKod,
                    TASINIRHESAPADI = detay.hesapPlanAd,
                    GONMUHASEBEKOD = detay.muhasebeKod,
                    GONHARACAMABIRIMKOD = detay.harcamaBirimKod,
                    GONAMBARKOD = detay.ambarKod,
                    BIRIMFIYAT = detay.fiyat,
                    BULUNDUGUYER = detay.ozellik.bulunduguYerAd
                });
            }
            return liste;
        }

    }
}