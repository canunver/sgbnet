using Ext1.Net;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Zimmet fiþi bilgilerinin kayýt, listeleme, onaylama, onay kaldýrma ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirTransfer : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        /// <summary>
        /// Ortak alan zimmet fiþi mi yoksa kiþi zimmet fiþi mi olduðunu tutan deðiþken
        /// </summary>
        static string belgeTuru;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde belgeTur girdi dizgisi dolu deðilse hata verir
        ///     ve sayfayý yüklemez, dolu ise sayfada bazý ayarlamalar yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Taþýnýr Transfer";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtBelgeTarihi.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

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


                ListeTemizle();
            }
        }

        /// <summary>
        /// Belge Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Zimmet fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["json"];
            JArray jArray = (JArray)JSON.Deserialize(json);
            if (string.IsNullOrEmpty(json) || jArray.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listede kaydedilecek kayýt bulunamadý.");
                return;
            }

            string hata = "";
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.fisTarih = new TNSDateTime(txtBelgeTarihi.RawText);
            tf.yil = tf.fisTarih.Yil;
            tf.aciklama = txtAciklama.Text;
            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;
            tf.kayittanSonraOnayla = true;

            int index = 0;

            Hashtable htDetay = new Hashtable();

            foreach (JContainer jc in jArray)
            {
                index++;

                TasinirIslemDetay td = new TasinirIslemDetay();
                td.siraNo = index;
                td.hesapPlanKod = (jc.Value<string>("TASINIRHESAPKOD") + "") + "";
                td.sicilNo = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("PRSICILNO") + "", 0);
                td.gorSicilNo = jc.Value<string>("SICILNO") + "";

                if (string.IsNullOrEmpty(td.hesapPlanKod.Trim()) || string.IsNullOrEmpty(td.gorSicilNo.Trim()))
                    continue;

                td.miktar = 1;
                td.yil = tf.yil;
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(jc.Value<double>("FIYAT"));

                td.muhasebeKod = (jc.Value<string>("GONMUHASEBEKOD") + "").Split('-')[0].Replace(".", "").Trim();
                td.harcamaKod = (jc.Value<string>("GONHARACAMABIRIMKOD") + "").Split('-')[0].Replace(".", "").Trim();
                td.ambarKod = (jc.Value<string>("GONAMBARKOD") + "").Split('-')[0].Replace(".", "").Trim();

                td.gonMuhasebeKod = td.muhasebeKod;
                td.gonHarcamaKod = td.harcamaKod;

                SicilNoHareket shBilgi = new SicilNoHareket();
                shBilgi.prSicilNo = td.sicilNo;
                ObjectArray durum = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
                foreach (SicilNoHareket item in durum.objeler)
                {
                    if (item.islemTipiAd.StartsWith("Verme"))
                        hata += td.gorSicilNo + " nolu malzeme Zimmette olduðu için iþlem yapýlamaz<br>";
                }

                ObjectArray detay = (ObjectArray)htDetay[td.muhasebeKod + "-" + td.harcamaKod + "-" + td.ambarKod];
                if (detay == null) detay = new ObjectArray();

                detay.objeler.Add(td);
                htDetay[td.muhasebeKod + "-" + td.harcamaKod + "-" + td.ambarKod] = detay;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata);
                return;
            }

            Sonuc sonuc = new Sonuc();
            string sonucBilgi = "";
            foreach (DictionaryEntry entry in htDetay)
            {
                ObjectArray detay = (ObjectArray)htDetay[entry.Key];

                string[] bilgi = entry.Key.ToString().Split('-');

                //Devir çýkýþ fiþi kayýt
                tf.gMuhasebeKod = txtMuhasebe.Text.Replace(".", "");
                tf.gHarcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
                tf.gAmbarKod = txtAmbar.Text.Replace(".", "");

                tf.muhasebeKod = bilgi[0];
                tf.harcamaKod = bilgi[1];
                tf.ambarKod = bilgi[2];

                tf.detay = detay;
                tf.islemTipTur = (int)ENUMIslemTipi.DEVIRCIKIS;
                tf.islemTipKod = TasinirGenel.IslemTipiGetir(servisTMM, kullanan, tf.islemTipTur, false);

                tf.gFisNo = "";
                tf.gYil = 0;

                sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, tf);
                if (!sonuc.islemSonuc)
                {
                    sonucBilgi = sonuc.hataStr;
                    break;
                }

                string gFisNo = sonuc.anahtar.Split('-')[0];
                //Devir giriþ fiþi kayýt
                tf.gMuhasebeKod = tf.muhasebeKod;
                tf.gHarcamaKod = tf.harcamaKod;
                tf.gAmbarKod = tf.ambarKod;

                tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
                tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
                tf.ambarKod = txtAmbar.Text.Replace(".", "");

                tf.gFisNo = gFisNo;
                tf.gYil = tf.yil;

                foreach (TasinirIslemDetay item in tf.detay.objeler)
                {
                    item.muhasebeKod = tf.muhasebeKod;
                    item.harcamaKod = tf.harcamaKod;
                    item.ambarKod = tf.ambarAd;
                }

                tf.islemTipTur = (int)ENUMIslemTipi.DEVIRGIRIS;
                tf.islemTipKod = TasinirGenel.IslemTipiGetir(servisTMM, kullanan, tf.islemTipTur, false);
                sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, tf);
                if (!sonuc.islemSonuc)
                {
                    sonucBilgi = sonuc.hataStr;
                    break;
                }
            }


            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", "Transfer iþlemi baþarýyla yapýldý");
            else
                GenelIslemler.MesajKutusu("Hata", sonucBilgi);
        }

        protected void btnZimmetListele_Click(object sender, DirectEventArgs e)
        {
            //SicilNoHareket shBilgi = new SicilNoHareket();

            //shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            //shBilgi.muhasebeKod = txtMuhasebe.Text;
            //shBilgi.harcamaBirimKod = txtHarcamaBirimi.Text;
            //shBilgi.ambarKod = txtAmbar.Text;
            //shBilgi.kimeGitti = txtPersonel.Text;
            //shBilgi.nereyeGitti = txtNereden.Text;

            //ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());

            //List<object> liste = new List<object>();
            //foreach (SicilNoHareket dt in bilgi.objeler)
            //{
            //    liste.Add(new
            //    {
            //        FISNO = dt.fisNo,
            //        TASINIRHESAPKOD = dt.hesapPlanKod,
            //        SICILNO = dt.sicilNo,
            //        ACIKLAMA = dt.ozellik,
            //        TASINIRHESAPADI = dt.hesapPlanAd,
            //        KDVORANI = dt.kdvOran,
            //        BIRIMFIYATI = OrtakFonksiyonlar.ConvertToDbl(dt.fiyat),
            //        TESLIMEDILMEANINDADURUMU = "",
            //        PRSICILNO = dt.prSicilNo,
            //    });
            //}
            //strListe.DataSource = liste;
            //strListe.DataBind();
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            ListeTemizle();
            txtBelgeTarihi.Clear();
            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            //txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            //txtNereyeVerildi.Clear();
            //lblNereyeVerildi.Text = "";
            //txtKimeVerildi.Clear();
            //lblKimeVerildi.Text = "";

            if (txtMuhasebe.Text == "") lblMuhasebeAd.Text = "";
            if (txtHarcamaBirimi.Text == "") lblHarcamaBirimiAd.Text = "";
            //if (txtAmbar.Text == "") lblAmbarAd.Text = "";
        }

        private void ListeTemizle()
        {
            List<object> liste = new List<object>();
            for (int i = 0; i < 70; i++)
            {
                liste.Add(new
                {
                    KOD = ""
                });
            }

            strListe.DataSource = liste;
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
            SicilNoHareket h = new SicilNoHareket();
            h.muhasebeKod = txtMuhasebe.Text;
            h.harcamaBirimKod = txtHarcamaBirimi.Text;
            h.sorguButunSicilNolardaAra = kriter;
            ObjectArray hesap = servisTMM.BarkodSicilNoListele(kullanan, h, new Sayfalama());

            List<object> liste = new List<object>();
            foreach (SicilNoHareket detay in hesap.objeler)
            {
                liste.Add(new
                {
                    PRSICILNO = detay.prSicilNo,
                    SICILNO = detay.sicilNo,
                    FIYAT = detay.fiyat,
                    TASINIRHESAPKOD = detay.hesapPlanKod,
                    TASINIRHESAPADI = detay.hesapPlanAd,
                    GONMUHASEBEKOD = detay.muhasebeKod,
                    GONHARACAMABIRIMKOD = detay.harcamaBirimKod,
                    GONAMBARKOD = detay.ambarKod

                });
            }
            return liste;
        }
    }
}