using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class EnflasyonArtisi : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Toplu Deðer Düzeltme Sayfasý";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    //tabPanelAna.Border = true;
                    grdListe.Width = 200;
                }

                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));
                AmortismanDurumDoldur();

                txtBelgeTarihi.Value = DateTime.Now.Date;
            }
        }

        private void AmortismanDurumDoldur()
        {
            List<object> liste = new List<object>
            {
                new { KOD = 0, ADI = "Devam Ediyor" },
                new { KOD = 1, ADI = "Bitti" },
                new { KOD = -1, ADI = "Hepsi" }
            };

            strAmortismanDurum.DataSource = liste;
            strAmortismanDurum.DataBind();
            pgFiltre.UpdateProperty("prpAmoDurum", "-1");
        }

        protected void btnEnflasonArtisiKaydet_Click(object sender, DirectEventArgs e)
        {
            string bilgi = e.ExtraParams["BILGI"];

            SicilNoDegerArtis tf = new SicilNoDegerArtis();
            tf.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            tf.harcamaKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            tf.belgeTarihi = new TNSDateTime(txtBelgeTarihi.RawText);
            tf.tur = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlTur), 0);
            if (tf.tur == 1 && rdKaysayi.Checked) //Enflasyon Katsayýsý
                tf.katsayi = OrtakFonksiyonlar.ConvertToDouble(txtArtisTutar.Text, (double)0);
            else
                tf.tutar = OrtakFonksiyonlar.ConvertToDouble(txtArtisTutar.Text, (double)0);
            tf.gerekce = txtGerekce.Text;

            string hata = "";
            if (tf.tur == 1 && tf.tutar == 0)
            {
                //if (tf.katsayi <= 100)
                //    hata += "Enflasyon düzeltme oraný 100 den büyük olmalýdýr.<br>";
            }
            else if (tf.tutar == 0)
                hata += "Tutar bilgisi boþ býrakýlamaz.<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }


            ObjectArray bilgiler = new ObjectArray();
            if (!string.IsNullOrEmpty(bilgi))
            {
                foreach (Newtonsoft.Json.Linq.JContainer jc in (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(bilgi))
                {
                    SicilNoDegerArtis tf2 = new SicilNoDegerArtis();
                    tf2.muhasebeKod = tf.muhasebeKod;
                    tf2.harcamaKod = tf.harcamaKod;
                    tf2.belgeTarihi = tf.belgeTarihi;
                    tf2.katsayi = tf.katsayi;
                    tf2.tur = tf.tur;
                    tf2.tutar = tf.tutar;
                    tf2.gerekce = tf.gerekce;
                    tf2.prSicilNo = OrtakFonksiyonlar.ConvertToInt(jc.Value<int>("prSicilNo"), 0);
                    tf2.gorSicilNo = OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("sicilno"));

                    bilgiler.Ekle(tf2);
                }
            }

            Sonuc sonuc = servisTMM.SicilNoDegerArtisKaydet(kullanan, bilgiler);
            if (sonuc.islemSonuc)
            {
                //DurumAdDegistir(txtBelgeNo.Text == "" ? (int)ENUMBelgeDurumu.YENI : (int)ENUMBelgeDurumu.DEGISTIRILDI);

                string[] deger = sonuc.anahtar.Split(',');
                string kod = deger[0];
                string belgeNo = deger[1];

                //hdnKod.Value = kod;
                //txtBelgeNo.Text = belgeNo;
                Temizle();
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSYG017, "Bilgi", Icon.Information);
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        protected void btnListe_Click(object sender, DirectEventArgs e)
        {

            RowSelectionModel sm = grdListe.SelectionModel.Primary as RowSelectionModel;
            if (sm != null)
            {
                sm.SelectedRows.Clear();
                sm.UpdateSelection();
            }
            PagingToolbar1.PageSize = OrtakFonksiyonlar.ConvertToInt(cmbPageSize.Text, 0);
            Session["RFIDBrkodKriter"] = KriterTopla();
        }

        private SicilNoHareket KriterTopla()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            shBilgi.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            shBilgi.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            //shBilgi.fisNo = pgFiltre.Source["prpBelgeNo"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo"].Value.Trim().PadLeft(6, '0');
            shBilgi.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim().Replace(".", "");
            shBilgi.hesapPlanKod = pgFiltre.Source["prpHesapKod"].Value.Trim();
            shBilgi.eSicilNo = pgFiltre.Source["prpEskiSicilNo"].Value.Trim();
            shBilgi.sorguFiyat1 = OrtakFonksiyonlar.ConvertToDecimal(pgFiltre.Source["prpFiyat1"].Value);
            shBilgi.sorguFiyat2 = OrtakFonksiyonlar.ConvertToDecimal(pgFiltre.Source["prpFiyat2"].Value);
            shBilgi.ozellik.sorguAlimTarihi1 = new TNSDateTime(pgFiltre.Source["prpAlimTarih1"].Value.Trim());
            shBilgi.ozellik.sorguAlimTarihi2 = new TNSDateTime(pgFiltre.Source["prpAlimTarih2"].Value.Trim());
            shBilgi.amortismanYil = OrtakFonksiyonlar.ConvertToDouble(pgFiltre.Source["prpAmoYuzde"].Value);
            shBilgi.ozellik.amortismanBitti = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpAmoDurum"].Value, 0);

            return shBilgi;
        }

        private int SicilNumarasiDoldur(SicilNoHareket shBilgi, Sayfalama sayfa)
        {
            string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");

            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, sayfa);
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                bilgi.sonuc.kayitSay = -1;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("prSicilNo");
            dt.Columns.Add("sicilno");
            dt.Columns.Add("kod");
            dt.Columns.Add("ad");
            dt.Columns.Add("eskiSicilNo");
            dt.Columns.Add("amoYuzde");
            dt.Columns.Add("birimFiyat");
            dt.Columns.Add("aciklama");
            dt.Columns.Add("alimTarihi");

            foreach (SicilNoHareket sh in bilgi.objeler)
            {
                string ozellik = "";

                if (!String.IsNullOrEmpty(sh.ozellik.markaAd))
                    ozellik = sh.ozellik.markaAd;
                if (!String.IsNullOrEmpty(sh.ozellik.modelAd))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.modelAd;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.plaka))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.plaka;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.adi))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.adi;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.disSicilNo))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.disSicilNo;
                }
                if (ozellik != "") ozellik = " (" + ozellik + ")";

                string tip = "TÝF";
                if (sh.kisiAd != "")
                    tip = "ZÝMMET";


                dt.Rows.Add(sh.prSicilNo, sh.sicilNo, sh.hesapPlanKod, sh.hesapPlanAd, sh.eSicilNo, sh.amortismanYil > 0 ? Math.Round(100 / sh.amortismanYil, 2) : sh.amortismanYil, Math.Round(sh.fiyat, 2), ozellik, sh.ozellik.hesapTarih);
            }

            strListe.DataSource = dt;
            strListe.DataBind();

            return bilgi.sonuc.kayitSay;
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpHesapKod", DateTime.Now.Year.ToString());
            pgFiltre.UpdateProperty("prpSicilNo", string.Empty);
            pgFiltre.UpdateProperty("prpEskiSicilNo", string.Empty);

            pgFiltre.UpdateProperty("prpFiyat1", string.Empty);
            pgFiltre.UpdateProperty("prpFiyat2", string.Empty);
            pgFiltre.UpdateProperty("prpAlimTarih1", string.Empty);
            pgFiltre.UpdateProperty("prpAlimTarih2", string.Empty);
        }

        private void Temizle()
        {
            txtBelgeTarihi.Clear();
            ddlTur.SetValueAndFireSelect(1);
            txtArtisTutar.Clear();
            txtGerekce.Clear();
            wndEnflasyonArtisi.Hide();
        }

        protected void strListe_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            SicilNoHareket kriter = (SicilNoHareket)Session["RFIDBrkodKriter"];
            if (kriter == null)
                return;

            Sayfalama sayfa = new Sayfalama();

            sayfa.sayfaNo = (e.Start / e.Limit) + 1;
            sayfa.kayitSayisi = e.Limit;
            sayfa.siralamaAlani = e.Sort;
            if (e.Dir == Ext1.Net.SortDirection.Default || e.Dir == Ext1.Net.SortDirection.ASC)
                sayfa.siralamaYon = "ASC";
            else
                sayfa.siralamaYon = "DESC";

            if (sayfa.siralamaAlani == "prSicilNo") sayfa.siralamaAlani = "PRSICILNO";
            else if (sayfa.siralamaAlani == "sicilno") sayfa.siralamaAlani = "SICILNO";
            else if (sayfa.siralamaAlani == "kod") sayfa.siralamaAlani = "HESAPPLANKOD";
            else if (sayfa.siralamaAlani == "ad") sayfa.siralamaAlani = "HESAPPLANAD";
            else if (sayfa.siralamaAlani == "kimeGitti") sayfa.siralamaAlani = "KIMEGITTI";

            int kayitSayisi = SicilNumarasiDoldur(kriter, sayfa);
            if (kayitSayisi >= 0)
            {
                e.Total = kayitSayisi;

                if (kayitSayisi == 0)
                {
                    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
                }
            }
        }

    }
}