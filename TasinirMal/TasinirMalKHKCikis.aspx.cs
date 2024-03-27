using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;
using Ext1.Net;
using System.Collections;

namespace TasinirMal
{
    public partial class TasinirMalKHKCikis : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                formAdi = "KHK Kapsamında işlem gören Taşınır Bilgileri (Çıkış)";
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

            string kontrolIslemTipKod = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirKHKIslemKoduCikis") + "";

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(spnYil.Text, 0);
            tf.muhasebeKod = txtMuhasebeKod.Text.Trim();
            tf.harcamaKod = txtHarcamaKod.Text.Trim();
            tf.ambarKod = hdnAmbarKod.Text;
            tf.fisNo = hdnYeniBelgeNo.Text;
            tf.dayanakNo = txtDayanakNo.Text.Trim();

            tf.yil = DateTime.Now.Year;
            tf.fisTarih = new TNSDateTime(DateTime.Now);

            if (!txtDayanakTarih.IsEmpty)
                tf.dayanakTarih = new TNSDateTime(txtDayanakTarih.Text);

            tf.nereyeGitti = txtNereye.Text.Trim();
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt(kontrolIslemTipKod, 0);
            tf.detay = new ObjectArray();

            string eskiFisNo = txtBelgeNo.Text;
            eskiFisNo = eskiFisNo.PadLeft(6, '0');

            if (OrtakFonksiyonlar.ConvertToInt(eskiFisNo, 0) == 0)
            {
                GenelIslemler.MesajKutusu("Uyarı", "Giriş Fiş No boş bırakılamaz. Bu numara Çıkış işlemi yapılacak fişin numarasıdır.");
                return;
            }


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
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(row.Value<double>("birimFiyat"));
                td.gorSicilNo = row.Value<string>("sicilNo");

                siraNo++;

                tf.detay.Ekle(td);
            }

            //tf.kimeGitti = eskiFisNo;

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

                tf.kimeGitti = tf.fisNo;
                tf.fisNo = eskiFisNo;
                sonuc = servisTMM.TasinirIslemFisiKHKFisNoGuncelle(kullanan, tf);

                btnListele_Click(null, null);
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
            ObjectArray tbilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

            string kontrolIslemTipKod = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirKHKIslemKodu") + "";
            
            TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
            if (tbilgi.sonuc.islemSonuc)
            {
                tform = (TNS.TMM.TasinirIslemForm)tbilgi[0];

                hdnYeniBelgeNo.Value = "";
                lblYeniBelgeNo.Html = "";

                if (tform.durum != (int)ENUMBelgeDurumu.ONAYLI)
                {
                    GenelIslemler.MesajKutusu("Uyarı", tf.fisNo + " nolu TİF Onaylı olmadığı için işlem yapılamaz. İlk önce TİF ekranından Onaylama yapınız");
                    return liste;
                }
                else if (tform.islemTipKod != OrtakFonksiyonlar.ConvertToInt(kontrolIslemTipKod, 0))
                {
                    GenelIslemler.MesajKutusu("Uyarı", tf.fisNo + " nolu TİF KHK işlemi içermemektedir.");
                    return liste;
                }

                if (tform.kimeGitti != "")
                {
                    hdnYeniBelgeNo.Value = tform.kimeGitti;
                    lblYeniBelgeNo.Html = "Bu belge ile <b>" + tform.kimeGitti + "</b> nolu çıkış TİF'i üretilmiştir.";
                }

                hdnAmbarKod.Text = tform.ambarKod;
            }

            //Siciller varsa listele
            SicilNoHareket shBilgi = new SicilNoHareket();
            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(spnYil.Text, 0);
            shBilgi.muhasebeKod = txtMuhasebeKod.Text.Trim();
            shBilgi.harcamaBirimKod = txtHarcamaKod.Text.Trim();
            shBilgi.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray bilgi = servisTMM.SicilNoListele(kullanan, shBilgi, 0, 9999999);
            if (bilgi.sonuc.islemSonuc)
            {
                foreach (SicilNoHareket td in bilgi.objeler)
                {
                    if (td.islemTipKod > 1000)
                        liste.Add(new object[] { td.hesapPlanKod, td.hesapPlanAd, "(ZİMMETTE)" + td.sicilNo, 1, "Adet", td.kdvOran, td.fiyat });
                    else
                        liste.Add(new object[] { td.hesapPlanKod, td.hesapPlanAd, td.sicilNo, 1, "Adet", td.kdvOran, td.fiyat });
                }
            }

            foreach (TasinirIslemDetay td in tform.detay.objeler)
            {
                if (!TNS.TMM.Arac.DemirbasCesidiMi(td.hesapPlanKod))
                    liste.Add(new object[] { td.hesapPlanKod, td.hesapPlanAd, "", td.miktar, td.olcuBirimAd, td.kdvOran, td.birimFiyat });
            }

            return liste;
        }



    }
}