using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using Ext1.Net;
using System.Collections.Generic;
using Aspose.Words;
using System.Data;
using System.Collections;

namespace TasinirMal
{
    public partial class UretimFormuForm : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                AlanTemizle();

                txtTarih.Text = DateTime.Now.ToString();
                txtGirenMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFGIRENMUHASEBE");
                txtGirenHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFGIRENHARCAMA");
                txtGirenAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFGIRENAMBAR");

                txtCikanMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFCIKANMUHASEBE");
                txtCikanHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFCIKANHARCAMA");
                txtCikanAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFCIKANAMBAR");

                txtImzaAdi1.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAADI1");
                txtImzaAdi2.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAADI2");
                txtImzaGorev1.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAGOREV1");
                txtImzaGorev2.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAGOREV2");

                MalzemeDoldur();
            }
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            UretimFormu tk = new UretimFormu();
            tk.kod = OrtakFonksiyonlar.ConvertToStr(hdnKod.Value);
            tk.girenMuhasebe.kod = txtGirenMuhasebe.Text.Trim();
            tk.girenHarcamaBirimi.kod = txtGirenHarcamaBirimi.Text.Trim();
            tk.girenAmbar.kod = txtGirenAmbar.Text.Trim();
            tk.cikanMuhasebe.kod = txtCikanMuhasebe.Text.Trim();
            tk.cikanHarcamaBirimi.kod = txtCikanHarcamaBirimi.Text.Trim();
            tk.cikanAmbar.kod = txtCikanAmbar.Text.Trim();
            tk.anaUrun.hesapKod = OrtakFonksiyonlar.ConvertToStr(ddlAnaHesap.Value);
            tk.miktar = OrtakFonksiyonlar.ConvertToInt(txtMiktar.Text, 0);
            tk.fisNo = OrtakFonksiyonlar.ConvertToInt(txtNumara.Text.Trim(), 0);
            tk.fisTarihi = new TNSDateTime(txtTarih.Text.Trim());
            tk.durum = 1;
            tk.girenFisNo = OrtakFonksiyonlar.ConvertToStr(hdnGirenFisNo.Value);
            tk.cikanFisNo = OrtakFonksiyonlar.ConvertToStr(hdnCikanFisNo.Value);

            Sonuc sonuc = servisTMM.UretimFormuKaydet(kullanan, tk);

            if (sonuc.islemSonuc)
            {
                GirenDegiskenleriKaydet(kullanan, txtGirenMuhasebe.Text.Trim(), txtGirenHarcamaBirimi.Text.Trim(), txtGirenAmbar.Text.Trim());
                CikanDegiskenleriKaydet(kullanan, txtGirenMuhasebe.Text.Trim(), txtGirenHarcamaBirimi.Text.Trim(), txtGirenAmbar.Text.Trim());
                UretimFormu tkl = new UretimFormu();
                tkl.kod = sonuc.anahtar;
                Listele(tkl);
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        private void GirenDegiskenleriKaydet(TNS.KYM.Kullanici kullanan, string muhasebe, string hb, string ambar)
        {
            if (muhasebe.Trim() != "")
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TIFGIRENMUHASEBE", muhasebe);
            if (hb.Trim() != "")
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TIFGIRENHARCAMA", hb);
            if (ambar.Trim() != "")
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TIFGIRENAMBAR", ambar);
        }

        private void CikanDegiskenleriKaydet(TNS.KYM.Kullanici kullanan, string muhasebe, string hb, string ambar)
        {
            if (muhasebe.Trim() != "")
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TIFCIKANMUHASEBE", muhasebe);
            if (hb.Trim() != "")
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TIFCIKANHARCAMA", hb);
            if (ambar.Trim() != "")
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TIFCIKANAMBAR", ambar);
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            AlanTemizle();
        }

        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            Yazdir();
        }

        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            UretimFormu tk = new UretimFormu();

            if (OrtakFonksiyonlar.ConvertToInt(txtNumara.Text.Trim(), 0) > 0)
            {
                tk.fisNo = OrtakFonksiyonlar.ConvertToInt(txtNumara.Text.Trim(), 0);
            }
            else
            {
                GenelIslemler.MesajKutusu("Uyarı", "Belge no boş bırakılamaz.");
                return;
            }

            Listele(tk);
        }

        private void Yazdir()
        {
            UretimFormu tk = new UretimFormu();
            tk.kod = hdnKod.Text;

            if (string.IsNullOrEmpty(tk.kod))
            {
                GenelIslemler.MesajKutusu("Uyarı", "Lütfen belge seçiniz.");
                return;
            }

            string temp = System.IO.Path.GetTempFileName();

            string sablonAd = Server.MapPath("~") + "/RaporSablon/TMM/UretimFormu.dotx";
            Document doc = new Document(sablonAd);
            DocumentBuilder docBuilder = new DocumentBuilder(doc);

            ObjectArray bilgiler = servisTMM.UretimFormuListele(kullanan, tk);

            DataTable dt = new DataTable();
            dt.TableName = "Detay";
            dt.Columns.Add("kulHesapKodu");
            dt.Columns.Add("kulHesapAdi");
            dt.Columns.Add("kulMiktar");
            dt.Columns.Add("kulBirimFiyat");
            dt.Columns.Add("kulTutar");

            string cikisMuhasebeKod = "";
            string cikisHarcamaKod = "";
            string cikisFisNo = "";
            int cikisYil = 0;
            string fisNo = "";
            foreach (UretimFormu b in bilgiler.objeler)
            {
                cikisYil = b.fisTarihi.Yil;
                cikisMuhasebeKod = b.cikanMuhasebe.kod;
                cikisHarcamaKod = b.cikanHarcamaBirimi.kod;
                cikisFisNo = b.cikanFisNo;
                fisNo = b.fisNo.ToString();

                TasinirGenel.AsposeAlanaYaz(docBuilder, "belgeNo", b.fisNo.ToString());
                TasinirGenel.AsposeAlanaYaz(docBuilder, "belgeTarih", b.fisTarihi.ToString());

                TasinirGenel.AsposeAlanaYaz(docBuilder, "islemYapan", b.islemYapan);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "anaUrunAdi", b.anaUrun.hesapKod + "-" + b.anaUrun.aciklama);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "miktari", b.miktar.ToString());
                TasinirGenel.AsposeAlanaYaz(docBuilder, "girenMuhasebe", b.girenMuhasebe.kod + "-" + b.girenMuhasebe.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "girenHarcamaBirimi", b.girenHarcamaBirimi.kod + "-" + b.girenHarcamaBirimi.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "girenAmbar", b.girenAmbar.kod + "-" + b.girenAmbar.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "girenTIFNo", b.girenFisNo);

                TasinirGenel.AsposeAlanaYaz(docBuilder, "cikanMuhasebe", b.cikanMuhasebe.kod + "-" + b.cikanMuhasebe.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "cikanHarcamaBirimi", b.cikanHarcamaBirimi.kod + "-" + b.cikanHarcamaBirimi.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "cikanAmbar", b.cikanAmbar.kod + "-" + b.cikanAmbar.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "cikanTIFNo", b.cikanFisNo);
            }

            string imzaAdi1 = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAADI1");
            string imzaGorev1 = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAGOREV1");
            string imzaAdi2 = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAADI2");
            string imzaGorev2 = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "URETIMIMZAGOREV2");

            TasinirGenel.AsposeAlanaYaz(docBuilder, "imzaAdi1", imzaAdi1);
            TasinirGenel.AsposeAlanaYaz(docBuilder, "imzaGorev1", imzaGorev1);
            TasinirGenel.AsposeAlanaYaz(docBuilder, "imzaAdi2", imzaAdi2);
            TasinirGenel.AsposeAlanaYaz(docBuilder, "imzaGorev2", imzaGorev2);
            TasinirGenel.AsposeAlanaYaz(docBuilder, "Yil", DateTime.Now.Year.ToString());

            TNS.TMM.TasinirIslemForm ts = new TNS.TMM.TasinirIslemForm();
            ts.yil = cikisYil;
            ts.muhasebeKod = cikisMuhasebeKod;
            ts.harcamaKod = cikisHarcamaKod;
            ts.fisNo = cikisFisNo;
            ObjectArray tsListe = servisTMM.TasinirIslemFisiAc(kullanan, ts, true);

            decimal toplamTutar = 0;
            foreach (TNS.TMM.TasinirIslemForm fis in tsListe.objeler)
            {
                foreach (TasinirIslemDetay satir in fis.detay.objeler)
                {
                    dt.Rows.Add(satir.hesapPlanKod, satir.hesapPlanAd, satir.miktar.ToString("#,###.##"), satir.birimFiyatKDVLi.ToString("#,###.##"), (satir.miktar * satir.birimFiyatKDVLi).ToString("#,###.##"));
                    toplamTutar += (satir.miktar * satir.birimFiyatKDVLi);
                }
            }

            if (dt.Rows.Count == 0)
                dt.Rows.Add("", "", "");

            TasinirGenel.AsposeAlanaYaz(docBuilder, "toplamTutar", toplamTutar.ToString("#,###.##"));

            doc.MailMerge.ExecuteWithRegions(dt);

            doc.Save(temp, Aspose.Words.SaveFormat.Pdf);
            OrtakClass.DosyaIslem.DosyaGonder(temp, "UretimFormu_" + fisNo, true, "pdf");
        }

        private void AlanTemizle()
        {
            hdnKod.Clear();
            txtNumara.Clear();
            lblDurum.Text = "";
            ddlAnaHesap.Clear();
            txtMiktar.Clear();
            lblGirisFisi.Html = "";
            lblCikisFisi.Html = "";
            hdnGirenFisNo.Clear();
            hdnCikanFisNo.Clear();
        }

        public void Listele(UretimFormu tk)
        {
            List<object> listeDetay = new List<object>();
            ObjectArray bilgi = servisTMM.UretimFormuListele(kullanan, tk);
            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarı", "Listelenecek kayıt bulunamadı." + bilgi.sonuc.hataStr);
                return;
            }

            tk = (UretimFormu)bilgi.objeler[0];
            hdnKod.Text = tk.kod;
            txtNumara.Text = tk.fisNo.ToString();
            ddlAnaHesap.SetValueAndFireSelect(tk.anaUrun.hesapKod);
            txtTarih.Text = tk.fisTarihi.ToString();
            txtGirenMuhasebe.Text = tk.girenMuhasebe.kod;
            txtGirenHarcamaBirimi.Text = tk.girenHarcamaBirimi.kod;
            txtGirenAmbar.Text = tk.girenAmbar.kod;
            txtCikanMuhasebe.Text = tk.cikanMuhasebe.kod;
            txtCikanHarcamaBirimi.Text = tk.cikanHarcamaBirimi.kod;
            txtCikanAmbar.Text = tk.cikanAmbar.kod;
            txtMiktar.Text = tk.miktar.ToString();

            lblCikanMuhasebeAd.Html = tk.cikanMuhasebe.ad;
            lblCikanHarcamaBirimiAd.Html = tk.cikanHarcamaBirimi.ad;
            lblCikanAmbarAd.Html = tk.cikanAmbar.ad;

            lblGirenMuhasebeAd.Html = tk.girenMuhasebe.ad;
            lblGirenHarcamaBirimiAd.Html = tk.girenHarcamaBirimi.ad;
            lblGirenAmbarAd.Html = tk.girenAmbar.ad;


            hdnGirenFisNo.Value = tk.girenFisNo;
            hdnCikanFisNo.Value = tk.cikanFisNo;

            if (tk.girenFisNo != "")
                lblGirisFisi.Html = "<a href='../TasinirMal/TasinirislemFormAna.aspx?yil=" + tk.fisTarihi.Yil + "&muhasebe=" + tk.girenMuhasebe.kod + "&harcamaBirimi=" + tk.girenHarcamaBirimi.kod + "&belgeNo=" + tk.girenFisNo + "' target='_blank'>" + tk.girenFisNo + "</a>";
            else
                lblGirisFisi.Html = "";

            if (tk.cikanFisNo != "")
                lblCikisFisi.Html = "<a href='../TasinirMal/TasinirislemFormAna.aspx?yil=" + tk.fisTarihi.Yil + "&muhasebe=" + tk.cikanMuhasebe.kod + "&harcamaBirimi=" + tk.cikanHarcamaBirimi.kod + "&belgeNo=" + tk.cikanFisNo + "' target='_blank'>" + tk.cikanFisNo + "</a>";
            else
                lblCikisFisi.Html = "";
        }

        private void MalzemeDoldur()
        {
            ObjectArray bilgi = servisTMM.UretimRecetesiListele(kullanan, new UretimRecetesi());

            List<object> liste = new List<object>();
            Hashtable htListe = new Hashtable();
            foreach (UretimRecetesi item in bilgi.objeler)
            {
                if (TasinirGenel.HashtableDegerVerDbl(htListe, item.anaUrun.hesapKod) > 0) continue;

                liste.Add(new
                {
                    ADI = item.anaUrun.aciklama,
                    KOD = item.anaUrun.hesapKod
                });

                htListe[item.anaUrun.hesapKod] = 1;

            }

            strUretilecekMalzeme.DataSource = liste;
            strUretilecekMalzeme.DataBind();
        }

        protected void btnImzaKaydet_Click(object sender, DirectEventArgs e)
        {
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "URETIMIMZAADI1", txtImzaAdi1.Text);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "URETIMIMZAADI2", txtImzaAdi2.Text);

            GenelIslemler.KullaniciDegiskenSakla(kullanan, "URETIMIMZAGOREV1", txtImzaGorev1.Text);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "URETIMIMZAGOREV2", txtImzaGorev2.Text);

            wndImza.Hide();
        }
    }
}