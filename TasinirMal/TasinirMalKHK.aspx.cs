using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;
using System.Collections;

namespace TasinirMal
{
    public partial class TasinirMalKHK : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                formAdi = "KHK Kapsamında işlem gören Taşınır Bilgileri";
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan)) GenelIslemler.SayfayaGirmesin(true);

                spnYil.Text = DateTime.Now.Year.ToString();
                txtMuhasebeKod.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaKod.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                if (txtMuhasebeKod.Text.Trim() != "")
                    lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebeKod.Text.Trim(), true);
                else
                    lblMuhasebeAd.Text = "";

                if (txtHarcamaKod.Text.Trim() != "")
                    lblHarcamaAd.Text = GenelIslemler.KodAd(32, txtMuhasebeKod.Text.Trim() + "-" + txtHarcamaKod.Text.Trim(), true);
                else
                    lblHarcamaAd.Text = "";
            }
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            string listeBilgileri = e.ExtraParams["ListeBilgileri"];
            Newtonsoft.Json.Linq.JObject[] o = (Newtonsoft.Json.Linq.JObject[])JSON.Deserialize(listeBilgileri, typeof(Newtonsoft.Json.Linq.JObject[]));

            TNS.TMM.TasinirIslemForm kriter = new TNS.TMM.TasinirIslemForm();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(spnYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebeKod.Text.Trim();
            kriter.harcamaKod = txtHarcamaKod.Text.Trim();
            kriter.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray ebilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, kriter, true);

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf = (TNS.TMM.TasinirIslemForm)ebilgi[0];
            tf.faturaNo = "";
            tf.faturaTarih = new TNSDateTime();

            string eskiFisNo = tf.fisNo;
            tf.fisNo = hdnYeniBelgeNo.Text;

            //Eğer eski yıla yeni kayıt yapılmak isteniyorsa izin verme
            //Önceki yıla kayıt edilmiş bir fiş değişmesi isteniyorsa bilgileri değiştirmeden devam et
            if (tf.yil != DateTime.Now.Year && string.IsNullOrEmpty(tf.fisNo))
            {
                tf.yil = DateTime.Now.Year;
                tf.fisTarih = new TNSDateTime(DateTime.Now);
            }


            tf.detay = new ObjectArray();

            int siraNo = 1;
            foreach (Newtonsoft.Json.Linq.JObject row in o)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();
                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.harcamaKod = tf.harcamaKod;
                td.ambarKod = tf.ambarKod;
                td.siraNo = siraNo;
                td.hesapPlanKod = row.Value<string>("hesapKod");
                td.miktar = OrtakFonksiyonlar.ConvertToDecimal(row.Value<string>("miktar"));
                td.kdvOran = OrtakFonksiyonlar.ConvertToInt(row.Value<string>("kdv"), 0);
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(row.Value<double>("yBirimFiyat"));

                siraNo++;

                tf.detay.Ekle(td);
            }

            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;

            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, tf);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                tf.fisNo = sonuc.anahtar.Split('-')[0];
                hdnYeniBelgeNo.Text = tf.fisNo;
                lblYeniBelgeNo.Html = "Bu belge ile <b>" + tf.fisNo + "</b> nolu TİF üretilmiştir.";

                //sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "Onay");

                tf.gFisNo = tf.fisNo;
                tf.fisNo = eskiFisNo;
                sonuc = servisTMM.TasinirIslemFisiKHKFisNoGuncelle(kullanan, tf);
            }
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            ArrayList liste = Listele("liste");
            StoreListe.DataSource = (object[])liste.ToArray(typeof(object[]));
            StoreListe.DataBind();
        }

        private ArrayList Listele(string tur)
        {
            ArrayList liste = new ArrayList();

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(spnYil.Text, 0);
            tf.muhasebeKod = txtMuhasebeKod.Text.Trim();
            tf.harcamaKod = txtHarcamaKod.Text.Trim();
            tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);


            string kontrolIslemTipKod = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirKHKIslemKodu") + "";
            
            if (bilgi.sonuc.islemSonuc)
            {
                TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
                tform = (TNS.TMM.TasinirIslemForm)bilgi[0];

                hdnYeniBelgeNo.Value = "";
                lblYeniBelgeNo.Html = "";

                if (tform.durum != (int)ENUMBelgeDurumu.ONAYLI)
                {
                    GenelIslemler.MesajKutusu("Uyarı", tf.fisNo + " nolu TİF Onaylı olmadığı için işlem yapılamaz. İlk önce TİF ekranından Onaylama yapınız");
                }
                else if (tform.islemTipKod != OrtakFonksiyonlar.ConvertToInt(kontrolIslemTipKod, 0))
                {
                    GenelIslemler.MesajKutusu("Uyarı", tf.fisNo + " nolu TİF KHK işlemi içermemektedir.");
                }
                else
                {
                    if (tform.gFisNo != "")
                    {
                        hdnYeniBelgeNo.Value = tform.gFisNo;
                        lblYeniBelgeNo.Html = "Bu belge ile <b>" + tform.gFisNo + "</b> nolu giriş TİF'i üretilmiştir.";
                    }

                    foreach (TasinirIslemDetay td in tform.detay.objeler)
                    {
                        liste.Add(new object[] { td.hesapPlanKod, td.hesapPlanAd, td.miktar, td.olcuBirimAd, td.kdvOran, td.birimFiyat, td.birimFiyat });
                    }
                }
            }
            else
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);

            return liste;
        }



    }
}