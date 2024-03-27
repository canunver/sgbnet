using Aspose.Words;
using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TNS;
using TNS.TMM;


namespace TasinirMal
{
    public partial class AktarimForm : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                AlanTemizle();
                IslemTipiDoldur();

                txtTarih.Text = DateTime.Now.ToString();
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                //btnDevamEt.Show();
            }
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.Aktarim tk = new TNS.TMM.Aktarim();
            tk.kod = hdnKod.Text;
            tk.muhasebe.kod = txtMuhasebe.Text.Trim();
            tk.harcamaBirimi.kod = txtHarcamaBirimi.Text.Trim();
            tk.ambar.kod = txtAmbar.Text.Trim();

            tk.belgeTarihi = new TNSDateTime(txtTarih.Text.Trim());
            tk.belgeTur = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(cmbBelgeTuru), 0);
            tk.islemTipiKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlIslemTipi), 0);
            tk.neredenGeldi = txtBulunduguYer.Text.Trim();
            tk.aciklama = txtAciklama.Text.Trim();
            tk.durum = 1;

            string hata = "";

            if (tk.muhasebe.kod == "")
                hata += "Muhasebe kodu boş bırakılamaz<br>";
            if (tk.harcamaBirimi.kod == "")
                hata += "Harcama Birim kodu boş bırakılamaz<br>";
            if (tk.ambar.kod == "")
                hata += "Ambar kodu boş bırakılamaz<br>";
            if (tk.belgeTarihi.Yil == 1)
                hata += "Belge Tarihi boş bırakılamaz<br>";
            if (tk.belgeTur == 0)
                hata += "Fiş Türü boş bırakılamaz<br>";
            if (tk.islemTipiKod == 0)
                hata += "İşlem Tipi boş bırakılamaz<br>";
            if (tk.neredenGeldi == "")
                hata += "Nereden Geldi bilgisi boş bırakılamaz<br>";
            if (tk.aciklama == "")
                hata += "Açıklama bilgisi boş bırakılamaz<br>";

            //Yüklenen dosya işlemi
            if (btnDosyaYukle.PostedFile == null || btnDosyaYukle.PostedFile.ContentLength <= 0)
                hata += Resources.TasinirMal.FRMTIG078;

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarı", hata);
                return;
            }

            string dosyaAdi = btnDosyaYukle.PostedFile.FileName;
            int count = Convert.ToInt32(btnDosyaYukle.PostedFile.InputStream.Length);
            byte[] myData = new byte[btnDosyaYukle.PostedFile.ContentLength];
            btnDosyaYukle.PostedFile.InputStream.Read(myData, 0, count);

            string dosyaAd = System.IO.Path.GetTempFileName();
            if (string.IsNullOrEmpty(dosyaAd))
            {
                GenelIslemler.MesajKutusu("Uyarı", Resources.TasinirMal.FRMTIG077);
                return;
            }
            tk.yuklenenDosyaAdi = dosyaAd;

            System.IO.FileStream newFile = new System.IO.FileStream(dosyaAd, System.IO.FileMode.Create);
            newFile.Write(myData, 0, myData.Length);
            newFile.Close();
            //***********************************************

            Sonuc sonuc = servisTMM.AktarimKaydet(kullanan, tk);

            if (sonuc.islemSonuc)
            {
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
                TNS.TMM.Aktarim tkl = new TNS.TMM.Aktarim();
                tkl.kod = sonuc.anahtar;
                Listele(tkl);
                btnTarihceYenile_Click(null, null);
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        protected void btnTarihceYenile_Click(object sender, DirectEventArgs e)
        {
            string ilgiKod = hdnKod.Value.ToString();
            ObjectArray tarihce = servisTMM.AktarimListeleTarihce(kullanan, ilgiKod);

            pnlTarihce.Show();
            lblTarihce.Clear();

            StringBuilder tt = new StringBuilder();
            foreach (TNS.TMM.Aktarim t in tarihce.objeler)
            {
                tt.Append("<small><span style='color:goldenrod'>" + t.islemTarih.Oku().ToString() + "</span></small> " + t.aciklama + "<br>");

                TimeSpan span = DateTime.Now.Subtract(t.islemTarih.Oku());

                if (span.TotalMinutes > 30)
                    btnDevamEt.Show();
            }

            lblTarihce.Html = tt.ToString();
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            AlanTemizle();
        }

        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            Yazdir();
        }

        protected void btnDevamEt_Click(object sender, DirectEventArgs e)
        {
            TNS.TMM.Aktarim tk = new TNS.TMM.Aktarim();
            tk.kod = hdnKod.Text;
            tk.muhasebe.kod = txtMuhasebe.Text.Trim();
            tk.harcamaBirimi.kod = txtHarcamaBirimi.Text.Trim();
            tk.ambar.kod = txtAmbar.Text.Trim();

            Sonuc sonuc = servisTMM.AktarimDevamEt(kullanan, tk);

            if (sonuc.islemSonuc)
            {
                btnTarihceYenile_Click(null, null);
                GenelIslemler.ExtNotification("Fiş oluşturma işlemine devam ediliyor", "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        [DirectMethod]
        public void btnListele_Click()
        {
            TNS.TMM.Aktarim tk = new TNS.TMM.Aktarim();

            tk.kod = hdnKod.Text;

            if (tk.kod == "") return;

            Listele(tk);
        }

        private void Yazdir()
        {
            TesisBilesenleri tk = new TesisBilesenleri();
            tk.refId = hdnKod.Text;

            if (string.IsNullOrEmpty(tk.refId))
            {
                GenelIslemler.MesajKutusu("Uyarı", "Lütfen belge seçiniz.");
                return;
            }

            string temp = System.IO.Path.GetTempFileName();

            string sablonAd = Server.MapPath("~") + "/RaporSablon/TMM/TesisBilesenleriCetveli.dotx";
            Document doc = new Document(sablonAd);
            DocumentBuilder docBuilder = new DocumentBuilder(doc);

            ObjectArray bilgiler = servisTMM.TesisBilesenleriListele(kullanan, tk);

            DataTable dt = new DataTable();
            dt.TableName = "Detay";
            dt.Columns.Add("siraNo");
            dt.Columns.Add("bilesenAdi");
            dt.Columns.Add("bilesenMiktar");
            dt.Columns.Add("bilesenOzellik");

            foreach (TesisBilesenleri b in bilgiler.objeler)
            {
                TasinirGenel.AsposeAlanaYaz(docBuilder, "belgeNo", b.belgeNo.ToString("d6"));
                TasinirGenel.AsposeAlanaYaz(docBuilder, "belgeTarih", b.belgeTarihi.ToString());
                TasinirGenel.AsposeAlanaYaz(docBuilder, "hbAdi", b.harcamaBirimi.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "hbKodu", b.harcamaBirimi.kod);

                TasinirGenel.AsposeAlanaYaz(docBuilder, "ilAdi", b.il.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "ilceAdi", b.ilce.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "ilKodu", b.il.kod);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "ilceKodu", b.ilce.kod);

                TasinirGenel.AsposeAlanaYaz(docBuilder, "tasinirAdi", b.hesapPlanAd);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "hesapPlanKod", b.hesapPlanKod);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "sicilNo", b.sicilNo);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "bulunduguYer", b.bulunduguYer);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "ambarAdi", b.ambar.ad);
                TasinirGenel.AsposeAlanaYaz(docBuilder, "ambarKodu", b.ambar.kod);

                TasinirGenel.AsposeAlanaYaz(docBuilder, "bAdet", b.detay.objeler.Count.ToString());

                foreach (TesisBilesenleriDetay tt in b.detay.objeler)
                {
                    dt.Rows.Add(tt.siraNo.ToString(), tt.adi, tt.miktar.ToString("#,###.##"), tt.ozellik);
                }

                if (b.detay.objeler.Count < 11)
                {
                    for (int i = 0; i < 11 - b.detay.objeler.Count; i++)
                    {
                        dt.Rows.Add("", "", "", "");
                    }
                }

                ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "", 0);
                bool bulundu = false;
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI)
                    {
                        TasinirGenel.AsposeAlanaYaz(docBuilder, "yetkliAdi", iBilgi.adSoyad);
                        TasinirGenel.AsposeAlanaYaz(docBuilder, "yetkiliUnvan", iBilgi.unvan);
                        bulundu = true;
                        break;
                    }
                }

                if (!bulundu)
                {
                    TasinirGenel.AsposeAlanaYaz(docBuilder, "yetkliAdi", "");
                    TasinirGenel.AsposeAlanaYaz(docBuilder, "yetkiliUnvan", "");
                }
            }

            doc.MailMerge.ExecuteWithRegions(dt);

            doc.Save(temp, Aspose.Words.SaveFormat.Docx);
            OrtakClass.DosyaIslem.DosyaGonder(temp, "TesisBilesenleriCetveli", true, "docx");
        }

        private void AlanTemizle()
        {
            lblDurum.Text = "";
            txtBulunduguYer.Clear();
            txtAciklama.Clear();
            ddlIslemTipi.Clear();
            cmbBelgeTuru.Clear();
            //cptBilgi.Hide();
            lblBilgi.Clear();
            lblTarihce.Clear();
            hdnKod.Clear();
        }

        public void Listele(TNS.TMM.Aktarim tk)
        {
            ObjectArray bilgi = servisTMM.AktarimListele(kullanan, tk);
            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarı", "Listelenecek kayıt bulunamadı." + bilgi.sonuc.hataStr);
                return;
            }
            foreach (TNS.TMM.Aktarim tkk in bilgi.objeler)
            {
                hdnKod.Text = tkk.kod;
                txtMuhasebe.Text = tkk.muhasebe.kod;
                txtHarcamaBirimi.Text = tkk.harcamaBirimi.kod;
                txtAmbar.Text = tkk.ambar.kod;

                txtTarih.Value = tkk.belgeTarihi.ToString();
                cmbBelgeTuru.SetValueAndFireSelect(tkk.belgeTur.ToString());
                ddlIslemTipi.SetValueAndFireSelect(tkk.islemTipiKod);
                txtBulunduguYer.Text = tkk.neredenGeldi;
                txtAciklama.Text = tkk.aciklama;

                string kayitBilgi = "<b>Toplam Satır Sayısı: </b>" + tkk.toplamKayit.ToString("#,##0") + "<br>";
                kayitBilgi += "<b>İşlenen Satır Sayısı: </b>" + tkk.islenenKayit.ToString("#,##0") + "<br>";
                //kayitBilgi += "<b>Oluşan Belgeler:</b>" + tkk.olusanBelgeNo;

                lblBilgi.Html = kayitBilgi;

                lblDurum.Text = "Durumu: " + TasinirMalResI.ResourceManager.GetString("TESISBILESENDURUM_" + tkk.durum.ToString("d3"));
            }

            btnTarihceYenile_Click(null, null);
        }

        private void IslemTipiDoldur()
        {
            ObjectArray bilgi = servisTMM.IslemTipListele(kullanan, new IslemTip());

            List<object> liste = new List<object>();
            foreach (IslemTip tip in bilgi.objeler)
            {
                if (tip.tur > 50) continue;

                liste.Add(new
                {
                    KOD = tip.kod,
                    ADI = tip.ad
                });
            }

            strIslemTipi.DataSource = liste;
            strIslemTipi.DataBind();
        }

    }
}