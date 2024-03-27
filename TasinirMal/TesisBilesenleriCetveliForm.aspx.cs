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


namespace TasinirMal
{
    public partial class TesisBilesenleriCetveliForm : TMMSayfaV2
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
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            }
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray detaySatirlari = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            TesisBilesenleri tk = new TesisBilesenleri();
            tk.refId = hdnRefId.Text;
            tk.muhasebe.kod = txtMuhasebe.Text.Trim();
            tk.harcamaBirimi.kod = txtHarcamaBirimi.Text.Trim();
            tk.ambar.kod = txtAmbar.Text.Trim();
            tk.belgeNo = OrtakFonksiyonlar.ConvertToInt(txtNumara.Text.Trim(), 0);
            tk.belgeTarihi = new TNSDateTime(txtTarih.Text.Trim());
            tk.sicilNo = txtSicilNo.Text;
            tk.bulunduguYer = txtBulunduguYer.Text;
            tk.durum = 1;

            if (tk.sicilNo.Length > 8)
                tk.hesapPlanKod = tk.sicilNo.Substring(0, tk.sicilNo.Length - 8);

            if (tk.belgeNo == 0) tk.refId = "";

            tk.detay = new ObjectArray();
            int siraNo = 1;
            foreach (Newtonsoft.Json.Linq.JObject item in detaySatirlari)
            {
                TesisBilesenleriDetay detay = new TesisBilesenleriDetay();

                double miktar = TasinirGenel.DegerAlDbl(item, "MIKTAR");

                if (miktar == 0)
                    detay.miktar = 1;
                else
                    detay.miktar = miktar;

                detay.siraNo = siraNo++;
                detay.adi = TasinirGenel.DegerAlStr(item, "ADI");
                detay.ozellik = TasinirGenel.DegerAlStr(item, "OZELLIK");

                if (detay.adi.Trim() == "") continue;

                tk.detay.objeler.Add(detay);
            }

            Sonuc sonuc = servisTMM.TesisBilesenleriKaydet(kullanan, tk);

            if (sonuc.islemSonuc)
            {
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
                TesisBilesenleri tkl = new TesisBilesenleri();
                tkl.refId = sonuc.anahtar;
                Listele(tkl);
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

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
            TesisBilesenleri tk = new TesisBilesenleri();

            if (!string.IsNullOrEmpty(txtNumara.Text))
            {
                tk.belgeNo = OrtakFonksiyonlar.ConvertToInt(txtNumara.Text, 0);
                tk.muhasebe.kod = txtMuhasebe.Text;
                tk.harcamaBirimi.kod = txtHarcamaBirimi.Text;
            }
            else if (!string.IsNullOrEmpty(hdnRefId.Text))
            {
                tk.refId = hdnRefId.Text;
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
            TesisBilesenleri tk = new TesisBilesenleri();
            tk.refId = hdnRefId.Text;

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
            List<object> listeDetay = new List<object>();

            for (int i = 0; i < 10; i++)
            {
                listeDetay.Add(new
                {
                    REFID = "",
                    REFIDANA = "",
                    SIRANO = "",
                    ADI = "",
                    MIKTAR = 0,
                    OZELLIK = ""
                });

            }
            txtNumara.Clear();
            lblDurum.Text = "";
            txtSicilNo.Clear();
            txtBulunduguYer.Clear();

            strListe.DataSource = listeDetay;
            strListe.DataBind();
        }

        public void Listele(TesisBilesenleri tk)
        {
            List<object> listeDetay = new List<object>();
            ObjectArray bilgi = servisTMM.TesisBilesenleriListele(kullanan, tk);
            if (!bilgi.sonuc.islemSonuc || bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarı", "Listelenecek kayıt bulunamadı." + bilgi.sonuc.hataStr);
                return;
            }

            tk = (TesisBilesenleri)bilgi.objeler[0];
            txtTarih.Value = tk.belgeTarihi.ToString();
            hdnRefId.Text = tk.refId;
            txtNumara.Text = tk.belgeNo.ToString("000000");
            txtMuhasebe.Text = tk.muhasebe.kod;
            txtHarcamaBirimi.Text = tk.harcamaBirimi.kod;
            txtAmbar.Text = tk.ambar.kod;
            txtSicilNo.Text = tk.sicilNo;
            txtBulunduguYer.Text = tk.bulunduguYer;

            lblDurum.Text = "Durumu: " + TasinirMalResI.ResourceManager.GetString("TESISBILESENDURUM_" + tk.durum.ToString("d3"));

            foreach (TesisBilesenleriDetay detay in tk.detay.objeler)
            {
                listeDetay.Add(new
                {
                    REFID = detay.refID,
                    REFIDANA = detay.refIDAna,
                    SIRANO = detay.siraNo,
                    ADI = detay.adi,
                    MIKTAR = detay.miktar,
                    OZELLIK = detay.ozellik,
                });
            }

            strListe.DataSource = listeDetay;
            strListe.DataBind();
        }
    }
}