using System.Data;
using OrtakClass;
using TNS;
using TNS.DEG;
using TNS.TMM;
using System.Web.UI.WebControls;
using System;
using Ext1.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TasinirMal
{
    /// <summary>
    /// Devir giriþ iþlemi yapýlmamýþ devir çýkýþ taþýnýr iþlem fiþlerinin verilen kritere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeDevir : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Sayfa adresinde gelen yil, mb, hbk ve ak girdi dizgileri kullanýlarak devir giriþ
        ///     iþlemi yapýlmamýþ devir çýkýþ taþýnýr iþlem fiþleri cevap (response) olarak döndürülür.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //Giriþ sýrasýnda kullanýcýnýn varlýðýný kontrol et yoksa sayfaya girme
            //kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            if (!IsPostBack)
            {
                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                TNS.TMM.TasinirIslemForm kriter = RequestKriterleriniAl();
                if (kriter.yil <= 0 ||
                    kriter.muhasebeKod == "" ||
                    kriter.harcamaKod == "" ||
                    kriter.ambarKod == "")
                {
                    GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMLDV001);
                    X.AddScript("parent.hidePopWin();");
                    return;
                }

                //***Kullanýcý birimi þeklinde çalýþýyor ise devir ambarýný gösterme*****************************
                int devirSekli = OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRKULLANICIBIRIMI"), 0);
                if (devirSekli > 0 && kriter.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    kriter.ambarKod = "";
                //***********************************************************************************************

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    toolBar.Visible = false;
                    CheckboxSelectionModel1.Visible = false;
                }

                ObjectArray bilgi = servisTMM.GirisiYapilmamisDevirCikislari(kullanan, kriter);

                if (!bilgi.sonuc.islemSonuc)
                {
                    GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
                    X.AddScript("parent.hidePopWin();");
                    return;
                }

                if (bilgi.objeler.Count <= 0)
                {
                    GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                    X.AddScript("parent.hidePopWin();");
                    return;
                }

                List<object> storeListe = new List<object>();

                foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
                {
                    if (tif.yil == kriter.yil &&
                        tif.gMuhasebeKod.Replace(".", "") == kriter.muhasebeKod &&
                        tif.gHarcamaKod.Replace(".", "") == kriter.harcamaKod &&
                        (string.IsNullOrEmpty(tif.gAmbarKod) || tif.gAmbarKod.Replace(".", "") == kriter.ambarKod || (devirSekli > 0 && kriter.ambarKod == "")))
                        storeListe.Add(new
                        {
                            yil = tif.yil,
                            muhasebeKod = tif.muhasebeKod,
                            muhasebeAd = tif.muhasebeAd,
                            harcamaKod = tif.harcamaKod,
                            harcamaAd = tif.harcamaAd,
                            ambarKod = tif.ambarKod,
                            ambarAd = tif.ambarAd,
                            fisNo = tif.fisNo,
                            aciklama = tif.aciklama
                        });
                }

                if (storeListe.Count == 0)
                {
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLDV002);
                    X.AddScript("parent.hidePopWin();");
                    return;
                }

                storeDevir.DataSource = storeListe;
                storeDevir.DataBind();
            }
        }

        private TNS.TMM.TasinirIslemForm RequestKriterleriniAl()
        {
            TNS.TMM.TasinirIslemForm kriter = new TNS.TMM.TasinirIslemForm();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"] + "", 0);
            kriter.muhasebeKod = (Request.QueryString["mb"] + "").Replace(".", "");
            kriter.harcamaKod = (Request.QueryString["hbk"] + "").Replace(".", "");
            kriter.ambarKod = (Request.QueryString["ak"] + "").Replace(".", "");
            kriter.islemTipTur = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["is"] + "", 0);

            return kriter;
        }

        protected void btnKaydetOnayla_Click(object sender, DirectEventArgs e)
        {
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                //GenelIslemler.MesajKutusu("Uyarý", "Lütfen listeden tek tek seçerek devir giriþi yapýnýz.");
                //return;
            }

            string json = e.ExtraParams["json"];
            JArray jArray = (JArray)JSON.Deserialize(json);
            if (string.IsNullOrEmpty(json) || jArray.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMLDV009);
                return;
            }

            ObjectArray itler = servisTMM.IslemTipListele(kullanan, new IslemTip() { tur = (int)ENUMIslemTipi.DEVIRGIRIS });
            if (!itler.sonuc.islemSonuc || itler.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMLDV010);
                return;
            }

            TNS.TMM.TasinirIslemForm giris = RequestKriterleriniAl();
            giris.devirGirisiMi = true;
            giris.islemTipKod = ((IslemTip)itler[0]).kod;
            giris.islemTipTur = ((IslemTip)itler[0]).tur;

            giris.fisTarih.Yaz(DateTime.Now.Date);

            if (giris.fisTarih.Yil > giris.yil)
                giris.fisTarih = new TNSDateTime("31.12." + giris.yil);

            giris.islemTarih = new TNSDateTime(DateTime.Now);
            giris.islemYapan = kullanan.kullaniciKodu;

            TNSCollection cikislar = new TNSCollection();

            foreach (JContainer jc in jArray)
            {
                TNS.TMM.TasinirIslemForm cikis = new TNS.TMM.TasinirIslemForm();
                cikis.yil = giris.yil;
                cikis.muhasebeKod = jc.Value<string>("muhasebeKod");
                cikis.harcamaKod = jc.Value<string>("harcamaKod");
                cikis.ambarKod = jc.Value<string>("ambarKod");
                cikis.fisNo = jc.Value<string>("fisNo");

                cikislar.Add(cikis);
            }

            Sonuc sonuc = servisTMM.DevirGirisTIFKaydetOnayla(kullanan, giris, cikislar);
            if (sonuc.islemSonuc)
            {
                GenelIslemler.ExtNotification(sonuc.bilgiStr, "Bilgi", Icon.Accept);
                X.AddScript("parent.hidePopWin();");
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

    }
}