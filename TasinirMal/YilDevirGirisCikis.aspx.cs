using Ext1.Net;
using OrtakClass;
using System;
using System.Threading;
using TNS;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Ambarlarý yýl bazýnda iþleme kapatma iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class YilDevirGirisCikis : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        static int islemGorenAmbar = 0;
        static int islemGorecekAmbar = -1;
        static int iptal = -1;
        static int olusanFis = 0;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMYLD001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                txtYil.Value = DateTime.Now.Year;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnIptal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Ext1.Net.DirectEventArgs"/> instance containing the event data.</param>
        protected void btnIptal_Click(object sender, DirectEventArgs e)
        {
            iptal = 100;
        }

        /// <summary>
        /// Handles the Click event of the btnCikis control.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnCikis_Click(object sender, DirectEventArgs e)
        {
            btnCikis.Hidden = true;
            btnGiris.Hidden = true;
            btnIptal.Hidden = false;

            ResourceManager1.AddScript("{0}.startTask('IslemGostergec');", TaskManager1.ClientID);
            ThreadPool.QueueUserWorkItem(CikisIsle);
        }

        /// <summary>
        /// Handles the Click event of the btnGiris control.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnGiris_Click(object sender, DirectEventArgs e)
        {
            btnCikis.Hidden = true;
            btnGiris.Hidden = true;
            btnIptal.Hidden = false;

            ResourceManager1.AddScript("{0}.startTask('IslemGostergec');", TaskManager1.ClientID);
            ThreadPool.QueueUserWorkItem(GirisIsle);
        }

        void CikisIsle(object state)
        {
            //Bütün harcama birimlerini ve ambarlarý listele
            Ambar ambar = new Ambar();
            ambar.muhasebeKod = txtMuhasebe.Text;
            ambar.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            ambar.kod = txtAmbar.Text;

            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, ambar);
            //foreach ile ambarlarý dolaþ
            islemGorecekAmbar = bilgi.objeler.Count;
            islemGorenAmbar = 0;

            foreach (Ambar a in bilgi.objeler)
            {
                if (iptal == 100)
                {
                    islemGorenAmbar = 999999;
                    break;
                }

                //Yýl Devri çýkýþ listeyi çaðýr
                YilDevriCikisListele(a);
                islemGorenAmbar++;
            }
        }
        void GirisIsle(object state)
        {
            //Bütün harcama birimlerini ve ambarlarý listele
            Ambar ambar = new Ambar();
            ambar.muhasebeKod = txtMuhasebe.Text;
            ambar.harcamaBirimKod = txtHarcamaBirimi.Text.Replace(".", "");
            ambar.kod = txtAmbar.Text;

            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, ambar);
            //foreach ile ambarlarý dolaþ
            islemGorecekAmbar = bilgi.objeler.Count;
            islemGorenAmbar = 0;

            foreach (Ambar a in bilgi.objeler)
            {
                if (iptal == 100)
                {
                    islemGorenAmbar = 999999;
                    break;
                }

                //Yýl Devri çýkýþ listeyi çaðýr
                YilDevriGirisListele(a);
                islemGorenAmbar++;
            }
        }

        //Geçen seneki yýl devr çýkýþ fiþlerini ambar ambar listele
        //Eðer fiþ bilgisi gelir ise (sonraki ambara geç)
        ///////Bu sene için yýl devri giriþ fiþi üretilmiþ mi listele (üretilmiþ ise geç)
        ////////////Üretilmemiþ ise üret
        void YilDevriGirisListele(Ambar ambar)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0) - 1;
            tf.muhasebeKod = ambar.muhasebeKod;
            tf.harcamaKod = ambar.harcamaBirimKod;
            tf.ambarKod = ambar.kod;
            tf.durum = (int)ENUMBelgeDurumu.ONAYLI;
            tf.gYil = 0;
            tf.gFisNo = "";

            TasinirFormKriter kriter = new TasinirFormKriter();
            kriter.islemTipi = (int)servisUZY.UzayDegeriDbl(kullanan, "TASISLEMTIPKOD", ((int)ENUMIslemTipi.YILDEVIRCIKIS).ToString(), true, "");
            kriter.belgeTarihBasla = new TNSDateTime();
            kriter.belgeTarihBit = new TNSDateTime();
            kriter.durumTarihBasla = new TNSDateTime();
            kriter.durumTarihBit = new TNSDateTime();

            ObjectArray liste = servisTMM.TasinirIslemFisiListele(kullanan, tf, kriter);

            if (liste.sonuc.islemSonuc && liste.objeler.Count > 0)
            {
                foreach (TNS.TMM.TasinirIslemForm tasForm in liste.objeler)
                {
                    //*****Geçen sene çýkan belge bu sene girmiþ mi?
                    tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
                    tf.gYil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0) - 1;
                    tf.gFisNo = tasForm.fisNo;
                    kriter.islemTipi = (int)servisUZY.UzayDegeriDbl(kullanan, "TASISLEMTIPKOD", ((int)ENUMIslemTipi.YILDEVIRGIRIS).ToString(), true, "");
                    ObjectArray listeGirisKontrol = servisTMM.TasinirIslemFisiListele(kullanan, tf, kriter);
                    if (listeGirisKontrol.sonuc.islemSonuc && listeGirisKontrol.objeler.Count == 0)
                    {
                        //*****Eðer girmemiþ  ise fisac ile çýkýþ belgesini aç, iþlem tipini yildevrigiriþ yap ve belgeKaydete gönder
                        tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0) - 1;
                        tf.fisNo = tasForm.fisNo;
                        tf.gYil = 0;
                        tf.gFisNo = "";

                        ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

                        if (bilgi.sonuc.islemSonuc)
                        {
                            TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
                            tform = (TNS.TMM.TasinirIslemForm)bilgi[0];
                            GirisBelgeUret(ambar, tform);
                        }
                    }
                }
            }
        }
        void GirisBelgeUret(Ambar ambar, TNS.TMM.TasinirIslemForm tf)
        {
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = ambar.muhasebeKod;
            tf.harcamaKod = ambar.harcamaBirimKod;
            tf.ambarKod = ambar.kod;
            tf.gYil = tf.yil - 1;
            tf.gFisNo = tf.fisNo;//Gelen çýkýþ tif belgesi giriþ tif belgesi olarak kayýt edileceði
            tf.fisNo = "";
            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;
            tf.fisTarih = new TNSDateTime("01.01." + txtYil.Text);
            tf.islemTipTur = (int)ENUMIslemTipi.YILDEVIRGIRIS;
            tf.islemTipKod = (int)servisUZY.UzayDegeriDbl(kullanan, "TASISLEMTIPKOD", tf.islemTipTur.ToString(), true, "");

            foreach (TasinirIslemDetay d in tf.detay.objeler)
            {
                d.yil = tf.yil;
                d.fisNo = "";
            }

            BelgeKaydet(tf);
        }

        //Stok listesini ambar ambar çaðýr
        //Dolu olan stok bilgilerinden yýl devri çýkýþ belgesi üret
        void YilDevriCikisListele(Ambar ambar)
        {
            StokHareketBilgi shBilgi = new StokHareketBilgi();
            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            shBilgi.muhasebeKod = ambar.muhasebeKod;
            shBilgi.harcamaKod = ambar.harcamaBirimKod;
            shBilgi.ambarKod = ambar.kod;

            if (GenelIslemlerIstemci.VarsayilanKurumBul() == "12")
                shBilgi.khkHaric = true;

            ObjectArray liste = servisTMM.TuketimListele(kullanan, shBilgi);

            if (liste.sonuc.islemSonuc && liste.objeler.Count > 0)
            {
                CikisBelgeUret(ambar, liste);
            }
        }
        void CikisBelgeUret(Ambar ambar, ObjectArray liste)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = ambar.muhasebeKod;
            tf.harcamaKod = ambar.harcamaBirimKod;
            tf.ambarKod = ambar.kod;
            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;

            TNSDateTime dt = new TNSDateTime("01.01." + (tf.yil + 1).ToString());
            tf.fisTarih = new TNSDateTime(dt.Oku().AddDays(-1));

            tf.islemTipTur = (int)ENUMIslemTipi.YILDEVIRCIKIS;
            tf.islemTipKod = (int)servisUZY.UzayDegeriDbl(kullanan, "TASISLEMTIPKOD", tf.islemTipTur.ToString(), true, "");

            int siraNo = 1;
            foreach (StokHareketBilgi sBilgi in liste.objeler)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();

                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.harcamaKod = tf.harcamaKod;
                td.ambarKod = tf.ambarKod;
                td.siraNo = siraNo;
                td.hesapPlanKod = sBilgi.hesapPlanKod;
                td.miktar = sBilgi.miktar;
                td.kdvOran = sBilgi.kdvOran;
                td.birimFiyat = sBilgi.birimFiyat;

                tf.detay.Ekle(td);

                siraNo++;
                if (siraNo > 1000)
                {
                    BelgeKaydet(tf);
                    tf.fisNo = "";
                    siraNo = 1;
                    tf.detay = new ObjectArray();
                }
            }

            if (tf.detay.objeler.Count > 0)
                BelgeKaydet(tf);
        }

        //Çýkýþ ve Giriþ için ortak
        void BelgeKaydet(TNS.TMM.TasinirIslemForm tf)
        {
            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, tf);
            //OrtakFonksiyonlar.HataStrYaz("SONUC:" + sonuc.hataStr);
            if (sonuc.islemSonuc)
            {
                olusanFis++;
                tf.fisNo = sonuc.anahtar.Split('-')[0];
                BelgeOnayla(tf);
            }
        }
        void BelgeOnayla(TNS.TMM.TasinirIslemForm tf)
        {
            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "Onay");
        }

        /// <summary>
        /// Refreshes the progress.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Ext1.Net.DirectEventArgs"/> instance containing the event data.</param>
        protected void RefreshProgress(object sender, DirectEventArgs e)
        {
            if (islemGorecekAmbar == -1)
            {
                lblToplamAmbar.Text = "0";
                lblAmbarSayac.Text = "0";
                lblKalanAmbar.Text = "0";
                lblBelgeSayac.Text = "0";

                Progress1.UpdateProgress(0, "Ýþlem yapýlacak ambarlar belirleniyor.");
            }
            else if (islemGorenAmbar < islemGorecekAmbar)
            {
                lblToplamAmbar.Text = islemGorecekAmbar.ToString("#,###");
                lblAmbarSayac.Text = islemGorenAmbar.ToString("#,###");
                lblKalanAmbar.Text = (islemGorecekAmbar - islemGorenAmbar).ToString("#,###");
                lblBelgeSayac.Text = olusanFis.ToString("#,###");
                Progress1.UpdateProgress(islemGorenAmbar / (float)islemGorecekAmbar, string.Format("% {0}", ((islemGorenAmbar / (float)islemGorecekAmbar) * 100).ToString("###")));
            }
            else
            {
                ResourceManager1.AddScript("{0}.stopTask('IslemGostergec');", TaskManager1.ClientID);
                Progress1.UpdateProgress(1, "Ýþlem tamamlandý.");
                islemGorenAmbar = 999999;
                btnCikis.Hidden = false;
                btnGiris.Hidden = false;
                btnIptal.Hidden = true;
                islemGorecekAmbar = -1;
                iptal = -1;
                olusanFis = 0;

                ResourceManager1.AddScript("alert('{0}');", "Yýl devri iþlemi baþarýyla sonuçlandý.");
            }
        }
    }
}