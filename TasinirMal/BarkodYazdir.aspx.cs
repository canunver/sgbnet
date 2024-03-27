using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class BarkodYazdir : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMBRK001;
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

                string barkodYazdirmaTur = System.Configuration.ConfigurationManager.AppSettings.Get("BarkodYazdirmaTur");
                hdnYazdirmaTur.SetValue(barkodYazdirmaTur);

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                string barkodYaziBuyuklugu = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETFONT");
                txtYukseklik.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETYUK");
                txtGenislik.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETGEN");
                txtSolBosluk.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETSOL");
                txtUstBosluk.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BARKODETIKETUST");

                if (barkodYaziBuyuklugu == "1") rdCokKucuk.Checked = true;
                else if (barkodYaziBuyuklugu == "2") rdKucuk.Checked = true;
                else if (barkodYaziBuyuklugu == "3") rdNormal.Checked = true;
                else rdBuyuk.Checked = true;

                string bZimmetEserBilgi = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BZIMMETESERBILGI");

                if (OrtakFonksiyonlar.ConvertToInt(bZimmetEserBilgi, 0) > 0)
                    cmbEkBilgi.SetValueAndFireSelect(bZimmetEserBilgi);
                else
                {
                    cmbEkBilgi.SetValueAndFireSelect("4");
                    txtAciklama.Text = bZimmetEserBilgi;
                    txtAciklama.Show();
                }

                string pYil = Request.QueryString["y"] + "";
                string pMuhasebe = Request.QueryString["m"] + "";
                string pHarcama = Request.QueryString["h"] + "";
                string pAmbar = Request.QueryString["a"] + "";
                string pBelgeNo = Request.QueryString["b"] + "";
                string pTur = Request.QueryString["bTur"] + "";

                if (pMuhasebe != "")
                {
                    pgFiltre.UpdateProperty("prpYil", pYil);
                    pgFiltre.UpdateProperty("prpMuhasebe", pMuhasebe);
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", pHarcama);
                    pgFiltre.UpdateProperty("prpAmbar", pAmbar);
                    pgFiltre.UpdateProperty("prpBelgeNo", pBelgeNo);

                    if (pTur == "TIF")
                    {
                        pgFiltre.UpdateProperty("prpDurumKod", "Ambarda");
                    }
                    else if (pTur == "ZIM" || pTur == "ORT")
                    {
                        pgFiltre.UpdateProperty("prpDurumKod", "Zimmette");
                    }
                }
            }
        }

        /// <summary>
        /// Demirbaþ listeleme kriterleri ekrandaki kontrollerden toplanýr ve SicilNumarasiDoldur yordamý çaðýrýlýr.
        /// </summary>
        private void KriterTopla()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value, 0);
            shBilgi.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            shBilgi.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            shBilgi.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            shBilgi.fisNo = pgFiltre.Source["prpBelgeNo"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo"].Value.Trim().PadLeft(6, '0');
            shBilgi.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim().Replace(".", "");
            shBilgi.hesapPlanKod = pgFiltre.Source["prpHesapKod"].Value.Trim();
            shBilgi.kimeGitti = pgFiltre.Source["prpKisiKod"].Value.Trim().Replace(".", "");
            shBilgi.nereyeGitti = pgFiltre.Source["prpOdaKod"].Value.Trim().Replace(".", "");
            shBilgi.ozellik.adi = pgFiltre.Source["prpEserAdi"].Value.Trim();
            shBilgi.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurumKod"].Value.Trim(), 0);

            if (GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "") == "1399") // Manas
            {
                shBilgi.kimeGitti = pgFiltre.Source["prpKisiKod"].Value.Trim();
                shBilgi.nereyeGitti = pgFiltre.Source["prpOdaKod"].Value.Trim();
            }

            SicilNumarasiDoldur(shBilgi);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaþlarý sunucudan alýp gvSicilNo GridView kontrolüne dolduran yordam
        /// shBilgi parametresi dolu ise sunucunun BarkodSicilNoListele yordamý, zim parametresi dolu
        /// ise sunucunun ZimmetFisiAc yordamý çaðýrýlýr ve gelen demirbaþ bilgileri ekrana yazýlýr.
        /// </summary>
        /// <param name="shBilgi">Ambardaki demirbaþlarý listeleme kriterlerini tutan nesne</param>
        private void SicilNumarasiDoldur(SicilNoHareket shBilgi)
        {
            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
                return;
            }

            List<object> liste = new List<object>();
            foreach (SicilNoHareket sh in bilgi.objeler)
            {
                string ozellik = "";
                string eserBilgi = "";

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


                if (!String.IsNullOrEmpty(sh.ozellik.yazarAdi))
                    eserBilgi = sh.ozellik.yazarAdi;
                if (!String.IsNullOrEmpty(sh.ozellik.ciltNo))
                {
                    if (eserBilgi != "") eserBilgi += "-";
                    eserBilgi += sh.ozellik.ciltNo;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.yayinTarihi))
                {
                    if (eserBilgi != "") eserBilgi += "-";
                    eserBilgi += sh.ozellik.yayinTarihi;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.yeriKonusu))
                {
                    if (eserBilgi != "") eserBilgi += "-";
                    eserBilgi += sh.ozellik.yeriKonusu;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.saseNo))
                {
                    if (eserBilgi != "") eserBilgi += "-";
                    eserBilgi += sh.ozellik.saseNo;
                }

                string tip = "TÝF";
                if (sh.kisiAd != "")
                    tip = "ZÝMMET";

                liste.Add(new
                {
                    TIP = tip,
                    SICILNO = sh.sicilNo,
                    HESAPPLANKOD = sh.hesapPlanKod,
                    HESAPPLANADI = sh.hesapPlanAd + " " + ozellik,
                    ZIMMETKISI = sh.kimeGitti,
                    ESERBILGISI = eserBilgi,
                    ESKISICILNO = sh.ozellik.disSicilNo,
                    MUHASEBEADI = sh.muhasebeAd,
                    HARCAMABIRIMADI = sh.harcamaBirimAd,
                    AMBARADI = sh.ambarAd,
                    SASENO = sh.ozellik.saseNo
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        /// <summary>
        /// Ara tuþuna basýlýnca çalýþan olay metodu
        /// Ekrana girilmiþ bilgilere bakarak ilgili demirbaþ listeleme yordamýný çaðýrýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListe_Click(object sender, System.EventArgs e)
        {
            KriterTopla();
        }

        /// <summary>
        /// Handles the Click event of the btnEtiketKaydet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnEtiketKaydet_Click(object sender, EventArgs e)
        {
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETFONT", rdCokKucuk.Checked ? "1" : rdKucuk.Checked ? "2" : rdNormal.Checked ? "3" : "4");
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETYUK", txtYukseklik.Text.ToString());
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETGEN", txtGenislik.Text.ToString());
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETSOL", txtSolBosluk.Text.ToString());
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BARKODETIKETUST", txtUstBosluk.Text.ToString());

            string bZimmetEserBilgi = "";
            string ekBilgiKod = cmbEkBilgi.Value.ToString();
            if (ekBilgiKod == "1" || ekBilgiKod == "2" || ekBilgiKod == "3" || ekBilgiKod == "5")
                bZimmetEserBilgi = ekBilgiKod;
            else
                bZimmetEserBilgi = txtAciklama.Text;

            GenelIslemler.KullaniciDegiskenSakla(kullanan, "BZIMMETESERBILGI", bZimmetEserBilgi);
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpKisiKod", string.Empty);
            pgFiltre.UpdateProperty("prpOdaKod", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNo", string.Empty);
            pgFiltre.UpdateProperty("prpDurum", string.Empty);
            pgFiltre.UpdateProperty("prpSicilNo", string.Empty);
            pgFiltre.UpdateProperty("prpEserAdi", string.Empty);
        }
    }
}