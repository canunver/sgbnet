using System;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Collections;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Yetkili imza bilgilerinin kayýt, silme ve listeleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TanimImza : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTIM001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    tabPanelAna.Border = true;
                    grdListe.Width = 200;
                }
                else
                    hdnSecKapat.Value = 1;

                btnListele_Click(null, null);
            }
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Yetkili imza bilgileri grid kontrolünden toplanýr ve kaydedilmek üzere
        /// sunucuya gönderilir, gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray satirlar = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["IMZASATIRLARI"]);

            ObjectArray imzalar = new ObjectArray();

            foreach (Newtonsoft.Json.Linq.JObject item in satirlar)
            {
                ImzaBilgisi imza = new ImzaBilgisi();
                //imza.kullaniciKodu = kullanan.kullaniciKodu;
                imza.muhasebe = txtMuhasebe.Text.Trim();
                imza.birim = txtHarcamaBirimi.Text.Replace(".", "").Trim();
                imza.ambar = txtAmbar.Text.Trim();
                imza.imzaYer = TasinirGenel.DegerAlInt(item, "IMZAYERKOD");
                imza.adSoyad = TasinirGenel.DegerAlStr(item, "AD");
                imza.unvan = TasinirGenel.DegerAlStr(item, "UNVAN");
                //imza.kullanilmiyor = TasinirGenel.DegerAlInt(item, "KULLANILMIYOR");
                imza.gorevUnvan = "";
                imza.tarih = TasinirGenel.DegerAlInt(item, "TARIH");

                imzalar.objeler.Add(imza);
            }

            Sonuc sonuc = servisTMM.ImzaKaydet(kullanan, imzalar);

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan yetkili imza bilgileri alýnýr ve grid kontrolüne doldurulur.
        /// Hata varsa hata mesajý, yoksa bilgi mesajý görüntülenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, System.EventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIM002 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIM003 + "<br>";

            if (txtAmbar.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIM004 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + Resources.TasinirMal.FRMTIM006);
                return;
            }

            SortedList htSozluk = new SortedList();
            htSozluk[((int)ENUMImzaYer.TASINIRKAYITYETKILISI)] = Resources.TasinirMal.FRMTIM007;
            htSozluk[((int)ENUMImzaYer.HARCAMAYETKILISI)] = Resources.TasinirMal.FRMTIM008;
            htSozluk[((int)ENUMImzaYer.MUHASEBEYETKILISI)] = Resources.TasinirMal.FRMTIM009;
            htSozluk[((int)ENUMImzaYer.TASINIRKONSOLIDEGOREVLISI)] = Resources.TasinirMal.FRMTIM010;
            htSozluk[((int)ENUMImzaYer.MERKEZDEKITASINIRKONSOLIDEGOREVLISI)] = Resources.TasinirMal.FRMTIM011;
            htSozluk[((int)ENUMImzaYer.KAYITTANDUSMEKOMISYONBASKANI)] = Resources.TasinirMal.FRMTIM012;
            htSozluk[((int)ENUMImzaYer.KAYITTANDUSMEKOMISYONUYESI)] = Resources.TasinirMal.FRMTIM013;
            htSozluk[((int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUBASKANI)] = Resources.TasinirMal.FRMTIM014;
            htSozluk[((int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUUYE1)] = string.Format(Resources.TasinirMal.FRMTIM015, "1");
            htSozluk[((int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUUYE2)] = string.Format(Resources.TasinirMal.FRMTIM015, "2");
            htSozluk[((int)ENUMImzaYer.SAYIMKURULUBASKANI)] = Resources.TasinirMal.FRMTIM016;
            htSozluk[((int)ENUMImzaYer.SAYIMKURULUUYE1)] = string.Format(Resources.TasinirMal.FRMTIM017, "1");
            htSozluk[((int)ENUMImzaYer.SAYIMKURULUUYE2)] = string.Format(Resources.TasinirMal.FRMTIM017, "2");
            htSozluk[((int)ENUMImzaYer.BIRIMYETKILISI)] = Resources.TasinirMal.FRMTIM018;
            htSozluk[((int)ENUMImzaYer.USTYONETICI)] = Resources.TasinirMal.FRMTIM019;
            htSozluk[((int)ENUMImzaYer.TASINIRKONTROLYETKILISI)] = Resources.TasinirMal.FRMTIM035;

            ObjectArray iV = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Replace(".", "").Trim(), txtAmbar.Text.Trim(), 0);

            List<object> liste = new List<object>();
            if (iV.sonuc.islemSonuc)
            {
                foreach (ImzaBilgisi imza in iV.objeler)
                {
                    int imzaYerKod = 0;
                    string imzaYerAd = "";
                    foreach (DictionaryEntry entry in htSozluk)
                    {
                        if ((int)entry.Key == imza.imzaYer)
                        {
                            imzaYerKod = OrtakFonksiyonlar.ConvertToInt(entry.Key, 0);
                            imzaYerAd = (string)entry.Value;
                            break;
                        }

                    }
                    liste.Add(new
                    {
                        IMZAYERKOD = imzaYerKod,
                        IMZAYER = imzaYerAd,
                        AD = imza.adSoyad,
                        UNVAN = imza.unvan,
                        GOREVUNVAN = imza.gorevUnvan,
                        TARIH = imza.tarih
                    });
                }

                for (int i = iV.objeler.Count; i < htSozluk.Count;)
                {
                    i++;
                    liste.Add(new
                    {
                        IMZAYERKOD = i,
                        IMZAYER = OrtakFonksiyonlar.ConvertToStr(htSozluk[i]),
                        AD = "",
                        UNVAN = "",
                        GOREVUNVAN = "",
                        TARIH = ""
                    });
                }

                strListe.DataSource = liste;
                strListe.DataBind();
            }
            else
            {
                GenelIslemler.MesajKutusu("Hata", iV.sonuc.hataStr);
                return;
            }
        }
    }
}