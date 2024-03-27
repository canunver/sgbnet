using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.DEG;
using TNS.TMM;
using TNS.UZY;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþi bilgilerinin kayýt, listeleme, onaylama, onay kaldýrma ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIslemForm : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Gride eklenecek satýr sayýsý
        /// </summary>
        int ekleSatirSayisi = 30;

        /// <summary>
        /// Excel dosyasýndan bilgi yükleme iþleminin yapýlýp yapýlmadýðýný tutan deðiþken
        /// </summary>
        bool dosyaYukle = false;

        /// <summary>
        /// Taþýnýr malzemelerine ait özellik bilgilerinin baþladýðý fgrid gridindeki kolon numarasý
        /// </summary>
        int ozellikKolonBasla = 9;

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý kütüphane malzemeleri için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        bool kutuphaneGoster = false;

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý müze malzemeleri için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        bool muzeGoster = false;

        /// <summary>
        /// Taþýnýr iþlem fiþi sayfasý daðýtým Ýade için mi açýldý bilgisini tutan deðiþken
        /// </summary>
        bool dagitimIade = false;

        /// <summary>
        /// Kütüphane malzemelerine ait özellik bilgilerinin baþladýðý fgrid gridindeki kolon numarasý
        /// </summary>
        int kutuphaneKolonBasla = 9;

        /// <summary>
        /// Müze malzemelerine ait özellik bilgilerinin baþladýðý fgrid gridindeki kolon numarasý
        /// </summary>
        int muzeKolonBasla = 9;

        /// <summary>
        /// Ýþlem tipi deðiþtiðinde fgrid gridi formatlansýn mý bilgisini tutan deðiþken
        /// </summary>
        bool islemTipiDetayiTemizlesin = true;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        ///     Sayfa adresinde yil, muhasebe, harcamaBirimi ve belgeNo girdi dizgileri dolu
        ///     geliyorsa istenen belgeyi listelemesi için btnListele_Click yordamý çaðýrýlýr.
        ///     Listeleme kriterleri sayfa adresinde gelmiyorsa sayým tutanaðý, kayýttan düþme
        ///     teklif ve onay tutanaðý, geçici alýndý fiþi veya ihtiyaç fazlasý taþýnýrlar formu
        ///     bilgilerinden herhangi biri sessionda kayýtlý mý diye kontrol edilir, eðer kayýtlýysa
        ///     ilgili bilgileri ekrana yazan yordam çaðýrýlýr. Bu sayede, taþýnýr iþlem fiþi diðer
        ///     formlarla entegre edilmiþ olur. Son olarak seçili olan birime gönderilmiþ devir çýkýþ
        ///     taþýnýr iþlem fiþi var mý kontrolü yapýlýr, varsa kullanýcýya listesi gösterilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            TasinirGenel.JSResourceEkle_TasinirIslemForm(this);
            GenelIslemlerIstemci.GenelJSResourceEkle(this);

            if (Request.QueryString["aramaYok"] + "" != "")
            {
                pnlDosyaYukle.Visible = false;
            }
            BaslikBilgileriniAyarla();
            hdnFirmaHarcamadanAlma.Value = TasinirGenel.tasinirFirmaBilgisiniHarcamadanAlma;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            bool izin = false;
            if (dagitimIade)
                izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan);
            else
                izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan);

            if (!izin)
                GenelIslemler.SayfayaGirmesin(true);

            this.fpL.Attributes.Add("onDataChanged", "HucreDegisti(this)");
            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnOnayla.Attributes.Add("onclick", "return OnayAl('OnayTek','btnOnayla');");
            this.btnOnayKaldir.Attributes.Add("onclick", "return OnayAl('OnayTekKaldir','btnOnayKaldir');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");
            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtGonMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblGonMuhasebeAd',true,new Array('txtGonMuhasebe'),'KONTROLDENOKU');");
            this.txtGonHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblGonHarcamaBirimiAd',true,new Array('txtGonMuhasebe','txtGonHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtGonAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblGonAmbarAd',true,new Array('txtGonMuhasebe','txtGonHarcamaBirimi','txtGonAmbar'),'KONTROLDENOKU');");
            this.txtKimeGitti.Attributes.Add("onblur", "kodAdGetir('36','lblKimeGittiAd',true,new Array('txtKimeGitti'),'KONTROLDENOKU');");
            this.txtBelgeTarih.Attributes.Add("onblur", "BelgeFaturaTarihKontrol();");
            this.txtFaturaTarih.Attributes.Add("onblur", "BelgeFaturaTarihKontrol();");

            //fpL iþlemlerini karþýlamak için
            //***********************************************
            if (Request.Form["__EVENTTARGET"] == "fpL")
            {
                string arg = Request.Form["__EVENTARGUMENT"] + "";
                fpL_ButtonCommand(arg);
            }

            if (!IsPostBack)
            {
                ViewState["fpID"] = DateTime.Now.ToLongTimeString();
                YilDoldur();
                DovizDoldur();
                IslemTipiDoldur();
                GridInit(fpL);
                txtBelgeTarih.Text = DateTime.Now.Date.ToShortDateString();
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                if (Request.QueryString["belgeNo"] + "" != "")
                {
                    ddlYil.SelectedValue = Request.QueryString["yil"] + "";
                    txtBelgeNo.Text = Request.QueryString["belgeNo"] + "";
                    txtMuhasebe.Text = Request.QueryString["muhasebe"] + "";
                    txtHarcamaBirimi.Text = Request.QueryString["harcamaBirimi"] + "";
                    btnListele_Click(null, null);
                }
                else
                {
                    //Baþka formlardan tif hazýrlamak için sessin bilgilerine kayýt edilen listeleri ekrana aktarýr
                    SayimForm sf = (SayimForm)Session["SayimdanTIF"];
                    KayittanDusmeForm kf = (KayittanDusmeForm)Session["KayittanDusmedenTIF"];
                    TNS.TMM.TasinirIslemForm tf = (TNS.TMM.TasinirIslemForm)Session["GeciciAlindidanTIF"];
                    TNS.TMM.AcikPazarForm apf = (TNS.TMM.AcikPazarForm)Session["AcikPazardanTIF"];
                    if (sf != null)
                        SayimdanGeleniYaz(sf);
                    else if (kf != null)
                        KayittanDusmedenGeleniYaz(kf);
                    else if (tf != null && tf.detay != null)
                        GeciciAlindidanGeleniYaz(tf);
                    else if (apf != null)
                        AcikPazardanGeleniYaz(apf);
                    else
                    {
                        //Devir giriþi yapýlmamýþ kayýt var mý kontrolü 
                        //****************************************************
                        TNS.TMM.TasinirIslemForm kriter = new TNS.TMM.TasinirIslemForm();
                        kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
                        kriter.muhasebeKod = txtMuhasebe.Text;
                        kriter.harcamaKod = txtHarcamaBirimi.Text;
                        kriter.ambarKod = txtAmbar.Text;
                        kriter.islemTipTur = dagitimIade ? (int)ENUMIslemTipi.DAGITIMIADEGIRIS : (int)ENUMIslemTipi.DEVIRGIRIS;

                        if (kriter.yil > 0 && kriter.muhasebeKod != "" && kriter.harcamaKod != "" && kriter.ambarKod != "")
                        {
                            ObjectArray bilgi = servisTMM.GirisiYapilmamisDevirCikislari(kullanan, kriter);
                            bool var = false;
                            if (bilgi.sonuc.islemSonuc)
                            {
                                if (bilgi.objeler.Count > 0)
                                {
                                    foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
                                    {
                                        if (tif.yil == kriter.yil &&
                                            tif.gMuhasebeKod.Replace(".", "") == kriter.muhasebeKod &&
                                            tif.gHarcamaKod.Replace(".", "") == kriter.harcamaKod.Replace(".", "") &&
                                            tif.gAmbarKod.Replace(".", "") == kriter.ambarKod)
                                        {
                                            var = true;
                                            break;
                                        }
                                    }

                                    if (var)
                                    {
                                        for (int i = 0; i < ddlIslemTipi.Items.Count; i++)
                                        {
                                            string[] islem = ddlIslemTipi.Items[i].Value.Split('*');
                                            if (OrtakFonksiyonlar.ConvertToInt(islem[1], 0) == (int)ENUMIslemTipi.DEVIRGIRIS ||
                                                OrtakFonksiyonlar.ConvertToInt(islem[1], 0) == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                                            {
                                                ddlIslemTipi.SelectedIndex = i;
                                                break;
                                            }
                                        }

                                        //ClientScript.RegisterStartupScript(this.GetType(), "", "<script language=javascript>var tmp=setTimeout('DevirListesiAc()',200); </script>");
                                        Ext1.Net.X.AddScript("DevirListesiAc();");
                                    }
                                }
                            }
                        }
                        //****************************************************
                    }
                }
                if (!string.IsNullOrEmpty(ddlIslemTipi.SelectedValue.ToString()))
                    IslemGizle(OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0));
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtAmbar.Text.Trim() != "")
                lblAmbarAd.Text = GenelIslemler.KodAd(33, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim() + "-" + txtAmbar.Text.Trim(), true);
            else
                lblAmbarAd.Text = "";

            if (txtKimeGitti.Text.Trim() != "")
                lblKimeGittiAd.Text = GenelIslemler.KodAd(36, txtKimeGitti.Text.Trim(), true);
            else
                lblKimeGittiAd.Text = "";

            if (txtGonMuhasebe.Text.Trim() != "")
                lblGonMuhasebeAd.Text = GenelIslemler.KodAd(31, txtGonMuhasebe.Text.Trim(), true);
            else
                lblGonMuhasebeAd.Text = "";

            if (txtGonHarcamaBirimi.Text.Trim() != "")
                lblGonHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtGonMuhasebe.Text.Trim() + "-" + txtGonHarcamaBirimi.Text.Trim(), true);
            else
                lblGonHarcamaBirimiAd.Text = "";

            if (txtGonAmbar.Text.Trim() != "")
                lblGonAmbarAd.Text = GenelIslemler.KodAd(33, txtGonMuhasebe.Text.Trim() + "-" + txtGonHarcamaBirimi.Text.Trim() + "-" + txtGonAmbar.Text.Trim(), true);
            else
                lblGonAmbarAd.Text = "";
        }

        /// <summary>
        /// Parametre olarak verilen ihtiyaç fazlasý taþýnýr bilgilerini ekrana yazan yordam
        /// </summary>
        /// <param name="apform">Ýhtiyaç fazlasý taþýnýr bilgilerini tutan nesne</param>
        private void AcikPazardanGeleniYaz(TNS.TMM.AcikPazarForm apform)
        {
            //Ýþlem tipini otomatik seçmek için
            //****
            foreach (ListItem li in ddlIslemTipi.Items)
            {
                if (li.Value.Split('*')[1] == ((int)ENUMIslemTipi.DEVIRCIKIS).ToString() ||
                    li.Value.Split('*')[1] == ((int)ENUMIslemTipi.DAGITIMIADECIKIS).ToString())
                {
                    ddlIslemTipi.SelectedValue = li.Value;
                    break;
                }
            }

            islemTipiDetayiTemizlesin = false;
            ddlIslemTipi_SelectedIndexChanged(null, null);
            //****

            txtMuhasebe.Text = apform.muhasebeKod;
            lblMuhasebeAd.Text = apform.muhasebeAd;
            txtHarcamaBirimi.Text = apform.harcamaKod;
            lblHarcamaBirimiAd.Text = apform.harcamaAd;
            txtGonMuhasebe.Text = apform.istekYapanMuhasebeKod;
            lblGonMuhasebeAd.Text = apform.istekYapanMuhasebeAd;
            txtGonHarcamaBirimi.Text = apform.istekYapanHarcamaKod;
            lblGonHarcamaBirimiAd.Text = apform.istekYapanHarcamaAd;

            if (apform.detaylar.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = apform.detaylar.Count + 10;

            int i = 0;
            foreach (TNS.TMM.AcikPazarDetay apd in apform.detaylar)
            {
                fpL.Sheets[0].Cells[i, 0].Text = apd.hesapKod;
                fpL.Sheets[0].Cells[i, 2].Text = apd.gorSicilNo;
                //fpL.Sheets[0].Columns[8].Locked = false;
                fpL.Sheets[0].Cells[i, 8].Text = apd.hesapAd;
                //fpL.Sheets[0].Columns[8].Locked = true;
                fpL.Sheets[0].Cells[i, 4].Text = "1";
                fpL.Sheets[0].Cells[i, 5].Text = "Adet";
                fpL.Sheets[0].Cells[i, 6].Text = apd.kdvOran.ToString();
                fpL.Sheets[0].Cells[i, 7].Text = apd.birimFiyat.ToString();
                i++;
            }

            Session.Remove("AcikPazardanTIF");
        }

        /// <summary>
        /// Parametre olarak verilen geçici alýndý fiþi bilgilerini ekrana yazan yordam
        /// </summary>
        /// <param name="tf">Geçici alýndý fiþi bilgilerini tutan nesne</param>
        private void GeciciAlindidanGeleniYaz(TNS.TMM.TasinirIslemForm tf)
        {
            ddlYil.SelectedValue = tf.yil.ToString();
            txtMuhasebe.Text = tf.muhasebeKod;
            lblMuhasebeAd.Text = tf.muhasebeAd;
            txtHarcamaBirimi.Text = tf.harcamaKod;
            lblHarcamaBirimiAd.Text = tf.harcamaAd;
            txtAmbar.Text = tf.ambarKod;
            lblAmbarAd.Text = tf.ambarAd;
            txtNeredenGeldi.Text = tf.neredenGeldi;
            txtFaturaTarih.Text = tf.faturaTarih.ToString();
            txtFaturaNo.Text = tf.faturaNo;
            for (int i = 0; i < ddlIslemTipi.Items.Count; i++)
            {
                string[] islem = ddlIslemTipi.Items[i].Value.Split('*');
                if (OrtakFonksiyonlar.ConvertToInt(islem[1], 0) == (int)ENUMIslemTipi.SATINALMAGIRIS)
                {
                    ddlIslemTipi.SelectedIndex = i;
                    break;
                }
            }

            if (tf.detay.objeler.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = tf.detay.objeler.Count + 10;

            fpL.Sheets[0].Columns[5].Locked = false;
            fpL.Sheets[0].Columns[8].Locked = false;
            int sayac = 0;
            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                fpL.Sheets[0].Cells[sayac, 0].Text = td.hesapPlanKod;
                fpL.Sheets[0].Cells[sayac, 2].Text = td.gorSicilNo;
                fpL.Sheets[0].Cells[sayac, 4].Text = td.miktar.ToString();
                fpL.Sheets[0].Cells[sayac, 5].Text = td.olcuBirimAd;
                fpL.Sheets[0].Cells[sayac, 6].Text = td.kdvOran.ToString();
                fpL.Sheets[0].Cells[sayac, 7].Text = td.birimFiyat.ToString();
                fpL.Sheets[0].Cells[sayac, 8].Text = td.hesapPlanAd;
                sayac++;
            }
            fpL.Sheets[0].Columns[5].Locked = true;
            fpL.Sheets[0].Columns[8].Locked = true;

            Session.Remove("GeciciAlindidanTIF");
            Session.Remove("GeciciAlindiDetaydanTIF");
        }

        /// <summary>
        /// Parametre olarak verilen sayým tutanaðý bilgilerini ekrana yazan yordam
        /// </summary>
        /// <param name="sform">Sayým tutanaðý bilgilerini tutan nesne</param>
        private void SayimdanGeleniYaz(SayimForm sform)
        {
            ddlYil.SelectedValue = sform.yil.ToString();
            txtMuhasebe.Text = sform.muhasebeKod;
            lblMuhasebeAd.Text = sform.muhasebeAd;
            txtHarcamaBirimi.Text = sform.harcamaKod;
            lblHarcamaBirimiAd.Text = sform.harcamaAd;
            txtAmbar.Text = sform.ambarKod;
            lblAmbarAd.Text = sform.ambarAd;

            //Ýþlem tipini otomatik seçmek için
            //****
            SayimDetay detay = (SayimDetay)sform.detay[0];

            string islemTur = string.Empty;
            if (detay.fazlaMiktar > 0)
                islemTur = ((int)ENUMIslemTipi.SAYIMFAZLASIGIRIS).ToString();
            else if (detay.noksanMiktar > 0)
                islemTur = ((int)ENUMIslemTipi.SAYIMNOKSANICIKIS).ToString();

            foreach (ListItem li in ddlIslemTipi.Items)
            {
                if (li.Value.Split('*')[1] == islemTur)
                {
                    ddlIslemTipi.SelectedValue = li.Value;
                    break;
                }
            }

            islemTipiDetayiTemizlesin = false;
            ddlIslemTipi_SelectedIndexChanged(null, null);
            //****

            if (sform.detay.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = sform.detay.Count + 10;

            int i = 0;
            foreach (SayimDetay sd in sform.detay)
            {
                if (sd.fazlaMiktar > 0 || sd.noksanMiktar > 0)
                {
                    fpL.Sheets[0].Cells[i, 0].Text = sd.hesapPlanKod;
                    fpL.Sheets[0].Columns[8].Locked = false;
                    fpL.Sheets[0].Cells[i, 8].Text = sd.hesapPlanAd;
                    fpL.Sheets[0].Columns[8].Locked = true;
                    if (sd.fazlaMiktar > 0)
                        fpL.Sheets[0].Cells[i, 4].Text = sd.fazlaMiktar.ToString();
                    else if (sd.noksanMiktar > 0)
                        fpL.Sheets[0].Cells[i, 4].Text = sd.noksanMiktar.ToString();
                    fpL.Sheets[0].Cells[i, 5].Text = sd.olcuBirimAd;
                    i++;
                }
            }

            Session.Remove("SayimdanTIF");
        }

        /// <summary>
        /// Parametre olarak verilen kayýttan düþme teklif ve onay tutanaðý bilgilerini ekrana yazan yordam
        /// </summary>
        /// <param name="kform">Kayýttan düþme teklif ve onay tutanaðý bilgilerini tutan nesne</param>
        private void KayittanDusmedenGeleniYaz(KayittanDusmeForm kform)
        {
            //Ýþlem tipini otomatik seçmek için
            //****
            if (kform.detay.Count == 0)
                return;

            KayittanDusmeDetay detay = (KayittanDusmeDetay)kform.detay[0];

            foreach (ListItem li in ddlIslemTipi.Items)
            {
                if (detay.hesapPlanKod.Substring(0, 3) == ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() && li.Value.Split('*')[1] == ((int)ENUMIslemTipi.KULLANILMAZCIKIS).ToString())
                {
                    ddlIslemTipi.SelectedValue = li.Value;
                    break;
                }
                else if (detay.hesapPlanKod.Substring(0, 3) != ((int)ENUMTasinirHesapKodu.TUKETIM).ToString() && li.Value.Split('*')[1] == ((int)ENUMIslemTipi.HURDACIKIS).ToString())
                {
                    ddlIslemTipi.SelectedValue = li.Value;
                    break;
                }
            }

            islemTipiDetayiTemizlesin = false;
            ddlIslemTipi_SelectedIndexChanged(null, null);
            //****

            ddlYil.SelectedValue = kform.yil.ToString();
            txtMuhasebe.Text = kform.muhasebeKod;
            lblMuhasebeAd.Text = kform.muhasebeAd;
            txtHarcamaBirimi.Text = kform.harcamaKod;
            lblHarcamaBirimiAd.Text = kform.harcamaAd;
            txtAmbar.Text = kform.ambarKod;
            lblAmbarAd.Text = kform.ambarAd;

            if (kform.detay.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = kform.detay.Count + 10;

            int i = 0;
            foreach (KayittanDusmeDetay kd in kform.detay)
            {
                fpL.Sheets[0].Cells[i, 0].Text = kd.hesapPlanKod;
                fpL.Sheets[0].Cells[i, 2].Text = kd.gorSicilNo;
                fpL.Sheets[0].Columns[8].Locked = false;
                fpL.Sheets[0].Cells[i, 8].Text = kd.hesapPlanAd;
                fpL.Sheets[0].Columns[8].Locked = true;
                fpL.Sheets[0].Cells[i, 4].Text = kd.miktar.ToString();
                fpL.Sheets[0].Cells[i, 5].Text = kd.olcuBirimAd;
                fpL.Sheets[0].Cells[i, 6].Text = kd.kdvOran.ToString();
                fpL.Sheets[0].Cells[i, 7].Text = kd.birimFiyat.ToString();
                i++;
            }

            Session.Remove("KayittanDusmedenTIF");
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(Object sender, EventArgs e)
        {
            fpL.CancelEdit();
            fpL.Sheets[0].RowCount = 0;
            GridInit(fpL);
            fpL.Sheets[0].RowCount = ekleSatirSayisi;
            ddlYil.SelectedValue = DateTime.Now.Year.ToString();
            txtBelgeTarih.Text = DateTime.Now.Date.ToShortDateString();
            txtBelgeNo.Text = string.Empty;
            lblFormDurum.Text = "";
            //txtMuhasebe.Text = string.Empty;
            //txtHarcamaBirimi.Text = string.Empty;
            //txtAmbar.Text = string.Empty;
            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
            txtMuayeneNo.Text = string.Empty;
            txtMuayeneTarih.Text = string.Empty;
            txtDayanakNo.Text = string.Empty;
            txtDayanakTarih.Text = string.Empty;
            txtFaturaNo.Text = string.Empty;
            txtFaturaTarih.Text = string.Empty;
            txtNeredenGeldi.Text = string.Empty;
            txtNereyeGitti.Text = string.Empty;
            txtKimeGitti.Text = string.Empty;
            txtGonMuhasebe.Text = string.Empty;
            txtGonHarcamaBirimi.Text = string.Empty;
            txtGonAmbar.Text = string.Empty;
            txtGonBelgeNo.Text = string.Empty;
            //ddlGonYil.SelectedValue = DateTime.Now.Year.ToString();
            ddlIslemTipi.SelectedIndex = 0;
            if (txtMuhasebe.Text == "")
                lblMuhasebeAd.Text = string.Empty;
            if (txtHarcamaBirimi.Text == "")
                lblHarcamaBirimiAd.Text = string.Empty;
            if (txtAmbar.Text == "")
                lblAmbarAd.Text = string.Empty;
            if (txtKimeGitti.Text == "")
                lblKimeGittiAd.Text = string.Empty;
            if (txtGonMuhasebe.Text == "")
                lblGonMuhasebeAd.Text = string.Empty;
            if (txtGonHarcamaBirimi.Text == "")
                lblGonHarcamaBirimiAd.Text = string.Empty;
            if (txtGonAmbar.Text == "")
                lblGonAmbarAd.Text = string.Empty;

            IslemGizle(OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0));
        }

        /// <summary>
        /// Belge Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp kaydedilmek
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(Object sender, EventArgs e)
        {
            BaslikBilgileriniAyarla();
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
            string hata = "";
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.fisTarih = new TNSDateTime(txtBelgeTarih.Text);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.ambarKod = txtAmbar.Text.Replace(".", "");
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[0], 0);
            tf.islemTipTur = OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0);

            //if (tf.islemTipKod == (int)ENUMIslemTipi.SATINALMAGIRIS)
            //{
            //    if (tf.yil > 2009 && (tf.harcamaKod.StartsWith("12")))//|| tf.harcamaKod.StartsWith("17") 17.07.2012 talep üzerine kaldýrýldý
            //    {
            //        GenelIslemler.MesajKutusu("Uyarý", "<br><font size='medium'>" + Resources.TasinirMal.FRMTIG004 + "</font>");
            //        return;
            //    }
            //}

            if (tf.fisNo != "" && GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "") == "1399")
            {
                ObjectArray sListe = servisTMM.TasinirSurecNoListele(tf);
                string surecNo = "";
                foreach (TasinirIslemMIF tif in sListe.objeler)
                {
                    surecNo = tif.mifBelgeNo;
                }

                if (surecNo != "")
                {
                    GenelIslemler.MesajKutusu("Uyarý", "<br>Bu Taþýnýr fiþi " + surecNo + " nolu harcama süreci ile oluþturulmuþtur. Fiþi ancak harcama sürecinden deðiþtirebilirsiniz.");
                    return;
                }
            }


            decimal dovizDeger = (decimal)1;
            if (divDoviz.Style["display"] != "none")
            {
                string doviz = ddlDoviz.SelectedValue;
                string paraBirimi = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEPARABIRIMI");
                if (paraBirimi != "" && paraBirimi != doviz)
                {
                    TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

                    TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();

                    string kurtarih = txtFaturaTarih.Text;
                    if (kurtarih.Trim() == "") kurtarih = txtBelgeTarih.Text;

                    kriter.kurTarih = new TNSDateTime(kurtarih);
                    string[] kurlar = { doviz };

                    ObjectArray liste = servisHRC.MerkezBankKurDegerleri(kriter, kurlar);
                    if (liste.ObjeSayisi == 1)
                    {
                        TNS.HRC.KurDegerleri kur = (TNS.HRC.KurDegerleri)liste.objeler[0];
                        dovizDeger = Convert.ToDecimal(kur.alis);
                    }
                }
            }

            if (divDayanakBelge.Style["display"] != "none")
            {
                tf.dayanakNo = txtDayanakNo.Text;
                tf.dayanakTarih = new TNSDateTime(txtDayanakTarih.Text);
            }
            if (divFatura.Style["display"] != "none")
            {
                tf.faturaNo = txtFaturaNo.Text;
                tf.faturaTarih = new TNSDateTime(txtFaturaTarih.Text);
            }
            if (divKomisyon.Style["display"] != "none")
            {
                tf.muayeneNo = txtMuayeneNo.Text;
                tf.muayeneTarih = new TNSDateTime(txtMuayeneTarih.Text);
            }
            if (divNereyeGitti.Style["display"] != "none")
                tf.nereyeGitti = txtNereyeGitti.Text;
            if (divKimeGitti.Style["display"] != "none")
                tf.kimeGitti = txtKimeGitti.Text;
            if (divNeredenGeldi.Style["display"] != "none")
                tf.neredenGeldi = txtNeredenGeldi.Text;

            if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
            {
                //Ekranda gösterilmedi için arka planda set edildi.
                txtGonMuhasebe.Text = tf.muhasebeKod;
                txtGonHarcamaBirimi.Text = tf.harcamaKod;
            }

            if (divGonderilenBirim.Style["display"] != "none")
            {
                tf.gMuhasebeKod = txtGonMuhasebe.Text;
                tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                tf.gAmbarKod = txtGonAmbar.Text;

                if (divGonderilenAmbar.Style["display"] != "none")
                    tf.gAmbarKod = txtGonAmbar.Text;
            }
            if (divGonderilenBelge.Style["display"] != "none")
            {
                tf.gYil = OrtakFonksiyonlar.ConvertToInt(ddlGonYil.SelectedValue, 0);
                tf.gFisNo = txtGonBelgeNo.Text.Trim().PadLeft(6, '0');
            }

            fpL.SaveChanges();

            ObjectArray tdArray = new ObjectArray();

            int siraNo = 1;
            string sicilAyni = "";
            string eskiAnaHesapKod = "";

            if (tf.fisTarih.Oku().Year != tf.yil)
                hata += string.Format(Resources.TasinirMal.FRMTIG005, tf.yil, tf.fisTarih.ToString()) + "<br>";

            for (int i = 0; i < fpL.Rows.Count; i++)
            {
                string hesapKod = fpL.Cells[i, 0].Text.Trim().Replace(".", "");
                if (hesapKod.Trim() == "")
                    continue;

                if (hesapKod != "")
                {
                    if (OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0) == (int)ENUMIslemTipi.YILDEVIRCIKIS ||
                        OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0) == (int)ENUMIslemTipi.YILDEVIRGIRIS)
                    {
                        if (hesapKod.StartsWith("25"))
                        {
                            hata += Resources.TasinirMal.FRMTIG006 + "<br>";
                            break;
                        }
                    }
                    if (OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0) == (int)ENUMIslemTipi.ACILIS)
                    {
                        string anaHesapKod = hesapKod.Substring(0, 3);
                        if (anaHesapKod != eskiAnaHesapKod && eskiAnaHesapKod != "")
                        {
                            //hata += "Envanter (Açýlýþ) Fiþinde 150, 253, 254, 255 hesap kodlarýný ayrý ayrý girmelisiniz.";
                            //break;
                        }

                        eskiAnaHesapKod = anaHesapKod;
                    }

                    if (kutuphaneGoster)
                    {
                        if (hesapKod.Trim() != "" && !hesapKod.StartsWith(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString()))
                            hata += string.Format(Resources.TasinirMal.FRMTIG007, (i + 1).ToString()) + "<br>";
                    }
                    else if (muzeGoster)
                    {
                        if (hesapKod.Trim() != "" && !hesapKod.StartsWith(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString()))
                            hata += string.Format(Resources.TasinirMal.FRMTIG008, (i + 1).ToString()) + "<br>";
                    }

                    string sicilNo = fpL.Cells[i, 2].Text.Trim();
                    if (sicilNo != "" && (i + 1) <= fpL.Sheets[0].RowCount)
                    {
                        for (int j = i + 1; j < fpL.Rows.Count; j++)
                        {
                            if (sicilNo == fpL.Cells[j, 2].Text.Trim())
                                sicilAyni += string.Format(Resources.TasinirMal.FRMTIG009, (i + 1).ToString(), (j + 1).ToString()) + "<br>";
                        }
                    }
                }
            }

            if (sicilAyni != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", sicilAyni);
                return;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            for (int i = 0; i < fpL.Rows.Count; i++)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();
                td.hesapPlanKod = fpL.Cells[i, 0].Text.Trim().Replace(".", "");
                if (td.hesapPlanKod == string.Empty)
                    continue;

                td.yil = tf.yil;
                td.muhasebeKod = tf.muhasebeKod;
                td.harcamaKod = tf.harcamaKod;
                td.ambarKod = tf.ambarKod;
                td.siraNo = siraNo;
                td.gorSicilNo = fpL.Cells[i, 2].Text.Trim();
                td.miktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Cells[i, 4].Text);
                td.kdvOran = OrtakFonksiyonlar.ConvertToInt(fpL.Cells[i, 6].Text, 0);
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(fpL.Cells[i, 7].Text) * dovizDeger;
                //Serverda hesaplanýyor (kdv mükellefi olan muhasebelerden dolayý commentlendi)
                //td.birimFiyatKDVLi = (1 + (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100)) * td.birimFiyat;

                if (kutuphaneGoster)
                {
                    td.ozellik.disSicilNo = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla].Text.Trim();
                    td.ozellik.ciltNo = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 1].Text.Trim();
                    td.ozellik.dil = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 2].Text.Trim();
                    td.ozellik.yazarAdi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 3].Text.Trim();
                    td.ozellik.adi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 4].Text.Trim();
                    td.ozellik.yayinYeri = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 5].Text.Trim();
                    td.ozellik.yayinTarihi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 6].Text.Trim();
                    td.ozellik.neredenGeldi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 7].Text.Trim();
                    if (string.IsNullOrEmpty(td.ozellik.neredenGeldi) && !string.IsNullOrEmpty(txtNeredenGeldi.Text))
                        td.ozellik.neredenGeldi = txtNeredenGeldi.Text;

                    td.ozellik.boyutlari = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 8].Text.Trim();
                    td.ozellik.satirSayisi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 9].Text.Trim();
                    td.ozellik.yaprakSayisi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 10].Text.Trim();
                    td.ozellik.sayfaSayisi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 11].Text.Trim();
                    td.ozellik.ciltTuru = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 12].Text.Trim();
                    td.ozellik.cesidi = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 13].Text.Trim();
                    td.ozellik.yeriKonusu = fpL.Sheets[0].Cells[i, kutuphaneKolonBasla + 14].Text.Trim();
                }
                else if (muzeGoster)
                {
                    td.ozellik.disSicilNo = fpL.Sheets[0].Cells[i, muzeKolonBasla].Text.Trim();
                    td.ozellik.adi = fpL.Sheets[0].Cells[i, muzeKolonBasla + 1].Text.Trim();
                    td.ozellik.gelisTarihi = fpL.Sheets[0].Cells[i, muzeKolonBasla + 2].Text.Trim();
                    td.ozellik.neredenGeldi = fpL.Sheets[0].Cells[i, muzeKolonBasla + 3].Text.Trim();
                    td.ozellik.neredeBulundu = fpL.Sheets[0].Cells[i, muzeKolonBasla + 4].Text.Trim();
                    td.ozellik.cagi = fpL.Sheets[0].Cells[i, muzeKolonBasla + 5].Text.Trim();
                    td.ozellik.boyutlari = fpL.Sheets[0].Cells[i, muzeKolonBasla + 6].Text.Trim();
                    td.ozellik.durumuMaddesi = fpL.Sheets[0].Cells[i, muzeKolonBasla + 7].Text.Trim();
                    td.ozellik.onYuz = fpL.Sheets[0].Cells[i, muzeKolonBasla + 8].Text.Trim();
                    td.ozellik.arkaYuz = fpL.Sheets[0].Cells[i, muzeKolonBasla + 9].Text.Trim();
                    td.ozellik.puan = fpL.Sheets[0].Cells[i, muzeKolonBasla + 10].Text.Trim();
                    td.ozellik.yeriKonusu = fpL.Sheets[0].Cells[i, muzeKolonBasla + 11].Text.Trim();
                }
                else
                {
                    td.ozellik.saseNo = fpL.Cells[i, ozellikKolonBasla].Text.Trim(); //Açýklama alaný ayný zamanda seri no bilgisi
                    td.eAciklama = fpL.Cells[i, ozellikKolonBasla].Text.Trim(); //Devir giriþi ise satýrdaki, Açýlýþ ise eski demirbaþ no, yoksa boþ.
                    td.eSicilNo = fpL.Cells[i, ozellikKolonBasla + 1].Text.Trim(); //Devir giriþi ise satýrdaki, Açýlýþ ise eski demirbaþ no, yoksa boþ.
                    td.ozellik.disSicilNo = td.eSicilNo;

                    string eAlimTarih = OrtakFonksiyonlar.ConvertToStr(fpL.Cells[i, ozellikKolonBasla + 2].Value);
                    td.eAlimTarihi = new TNSDateTime(eAlimTarih.Trim());

                    td.eTedarikSekli = fpL.Cells[i, ozellikKolonBasla + 3].Text.Trim();
                }

                td.gonHarcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                td.gonMuhasebeKod = txtGonMuhasebe.Text;

                if (td.hesapPlanKod == string.Empty)
                    continue;
                else
                    siraNo++;

                tf.detay.Ekle(td);
            }

            int maxSatir = 1000;
            if (tf.detay.objeler.Count > maxSatir)
            {
                GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTIG010, maxSatir.ToString()));
                return;
            }

            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;

            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, tf);

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG011;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                txtBelgeNo.Text = sonuc.anahtar.Split('-')[0];
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Onayla tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onaylanmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayla_Click(Object sender, EventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG014 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata + Resources.TasinirMal.FRMTIG015);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "Onay");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG016;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Ýþlem tipi bilgileri sunucudan çekilir ve ddlIslemTipi DropDownList kontrolüne doldurulur.
        /// </summary>
        private void IslemTipiDoldur()
        {
            ObjectArray bilgi = servisTMM.IslemTipListele(kullanan, new IslemTip());

            foreach (IslemTip it in bilgi.objeler)
            {
                if (dagitimIade)
                {
                    if (it.tur != (int)ENUMIslemTipi.DAGITIMIADECIKIS && it.tur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        continue;
                }
                else
                {
                    if (it.tur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || it.tur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        continue;
                }
                ddlIslemTipi.Items.Add(new ListItem(it.ad, it.kod.ToString() + "*" + it.tur.ToString()));
            }
        }

        /// <summary>
        /// Sayfadaki ddlYil ve ddlGonYil DropDownList kontrollerine yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year + 1, DateTime.Now.Year);
            GenelIslemler.YilDoldur(ddlGonYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        private void DovizDoldur()
        {
            TNS.HRC.IHRCServis servis = TNS.HRC.Arac.Tanimla();
            TNS.BKD.ParaBirimi para = new TNS.BKD.ParaBirimi();
            ObjectArray paralar = servis.ParaBirimiListele(kullanan, para);

            foreach (TNS.BKD.ParaBirimi bilgi in paralar.objeler)
                ddlDoviz.Items.Add(new ListItem(bilgi.kisaltma, bilgi.kisaltma));
        }

        /// <summary>
        /// Sayfadaki fgrid grid kontrolünün ilk yükleniþte ayarlanmasýný saðlayan yordam
        /// </summary>
        /// <param name="kontrol">grid kontrolü</param>
        void GridInit(FarPoint.Web.Spread.FpSpread kontrol)
        {
            BaslikBilgileriniAyarla();

            if (!dosyaYukle)
            {
                kontrol.Reset();
                kontrol.Sheets.Clear();
            }

            kontrol.Sheets.Count = 1;
            kontrol.Sheets[0].AllowSort = true;
            kontrol.RenderCSSClass = true;
            kontrol.EditModeReplace = true;
            kontrol.EnableViewState = true;

            if (!dosyaYukle)
                fpL.Sheets[0].RowCount = ekleSatirSayisi;

            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            int sutunSayisi = 0;
            if (kutuphaneGoster)
                sutunSayisi = 24;
            else if (muzeGoster)
                sutunSayisi = 21;
            else
                sutunSayisi = 13;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = sutunSayisi;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMTIG017;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMTIG018;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMTIG019;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMTIG020;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMTIG021;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMTIG022;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMTIG023;

            if (kutuphaneGoster)
            {
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla].Value = Resources.TasinirMal.FRMTIG024;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 1].Value = Resources.TasinirMal.FRMTIG025;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 2].Value = Resources.TasinirMal.FRMTIG026;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 3].Value = Resources.TasinirMal.FRMTIG027;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 4].Value = Resources.TasinirMal.FRMTIG028;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 5].Value = Resources.TasinirMal.FRMTIG029;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 6].Value = Resources.TasinirMal.FRMTIG030;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 7].Value = Resources.TasinirMal.FRMTIG031;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 8].Value = Resources.TasinirMal.FRMTIG032;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 9].Value = Resources.TasinirMal.FRMTIG033;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 10].Value = Resources.TasinirMal.FRMTIG034;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 11].Value = Resources.TasinirMal.FRMTIG035;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 12].Value = Resources.TasinirMal.FRMTIG036;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 13].Value = Resources.TasinirMal.FRMTIG037;
                kontrol.Sheets[0].ColumnHeader.Cells[0, kutuphaneKolonBasla + 14].Value = Resources.TasinirMal.FRMTIG038;
            }
            else if (muzeGoster)
            {
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla].Value = Resources.TasinirMal.FRMTIG039;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 1].Value = Resources.TasinirMal.FRMTIG040;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 2].Value = Resources.TasinirMal.FRMTIG041;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 3].Value = Resources.TasinirMal.FRMTIG042;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 4].Value = Resources.TasinirMal.FRMTIG043;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 5].Value = Resources.TasinirMal.FRMTIG044;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 6].Value = Resources.TasinirMal.FRMTIG045;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 7].Value = Resources.TasinirMal.FRMTIG046;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 8].Value = Resources.TasinirMal.FRMTIG047;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 9].Value = Resources.TasinirMal.FRMTIG048;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 10].Value = Resources.TasinirMal.FRMTIG049;
                kontrol.Sheets[0].ColumnHeader.Cells[0, muzeKolonBasla + 11].Value = Resources.TasinirMal.FRMTIG050;
            }
            else
            {
                kontrol.Sheets[0].ColumnHeader.Cells[0, ozellikKolonBasla].Value = Resources.TasinirMal.FRMTIG051;
                kontrol.Sheets[0].ColumnHeader.Cells[0, ozellikKolonBasla + 1].Value = Resources.TasinirMal.FRMTIG052;
                kontrol.Sheets[0].ColumnHeader.Cells[0, ozellikKolonBasla + 2].Value = Resources.TasinirMal.FRMTIG053;
                kontrol.Sheets[0].ColumnHeader.Cells[0, ozellikKolonBasla + 3].Value = Resources.TasinirMal.FRMTIG054;

                FarPoint.Web.Spread.DateTimeCellType dCellType = new FarPoint.Web.Spread.DateTimeCellType();
                kontrol.Sheets[0].Columns[ozellikKolonBasla + 2].CellType = dCellType;

                FarPoint.Web.Spread.TextCellType cTextTypeEskiSicil = new FarPoint.Web.Spread.TextCellType();
                kontrol.Sheets[0].Columns[0, ozellikKolonBasla + 1].CellType = cTextTypeEskiSicil;
            }

            kontrol.Sheets[0].Columns[0].Width = 120;
            kontrol.Sheets[0].Columns[1].Width = 30;
            kontrol.Sheets[0].Columns[2].Width = 150;
            kontrol.Sheets[0].Columns[3].Width = 30;
            kontrol.Sheets[0].Columns[4].Width = 60;
            kontrol.Sheets[0].Columns[5].Width = 60;
            kontrol.Sheets[0].Columns[6].Width = 30;
            kontrol.Sheets[0].Columns[7].Width = 100;
            kontrol.Sheets[0].Columns[8].Width = 190;
            kontrol.Sheets[0].Columns[0].VerticalAlign = VerticalAlign.Top;

            if (kutuphaneGoster || muzeGoster)
                kontrol.Sheets[0].Columns[ozellikKolonBasla, kontrol.Sheets[0].ColumnCount - 5].Width = 100;

            kontrol.Sheets[0].Columns[kontrol.Sheets[0].ColumnCount - 4].Width = 120;
            kontrol.Sheets[0].Columns[kontrol.Sheets[0].ColumnCount - 3].Width = 60;
            kontrol.Sheets[0].Columns[kontrol.Sheets[0].ColumnCount - 2].Width = 60;
            kontrol.Sheets[0].Columns[kontrol.Sheets[0].ColumnCount - 1].Width = 100;

            kontrol.Sheets[0].Columns[4, 4].HorizontalAlign = HorizontalAlign.Right;
            kontrol.Sheets[0].Columns[6, 7].HorizontalAlign = HorizontalAlign.Right;

            string hesapKod = "";
            if (kutuphaneGoster)
                hesapKod = "255.07";
            if (muzeGoster)
                hesapKod = "255.06";

            TasinirGenel.MyLinkType hesapPlaniLink = new TasinirGenel.MyLinkType("HesapPlaniGoster('" + hesapKod + "')");
            hesapPlaniLink.ImageUrl = "../App_themes/images/bul1.gif";

            TasinirGenel.MyLinkType sicilNoLink = new TasinirGenel.MyLinkType("SicilNoListesiAc()");
            sicilNoLink.ImageUrl = "../App_themes/images/bul1.gif";

            kontrol.Sheets[0].Columns[1].CellType = hesapPlaniLink;
            kontrol.Sheets[0].Columns[3].CellType = sicilNoLink;

            kontrol.Sheets[0].Columns[5].Locked = true; //Ölçü Birimi
            kontrol.Sheets[0].Columns[8].Locked = true; //Hesap Planý Adý

            kontrol.Sheets[0].Columns[0, kontrol.Sheets[0].Columns.Count - 1].ForeColor = System.Drawing.Color.Black;

            kontrol.Sheets[0].Columns[5].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[8].BackColor = System.Drawing.Color.LightGoldenrodYellow;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;
            kontrol.Sheets[0].Columns[2].CellType = cTextType;
            kontrol.Sheets[0].Columns[8, kontrol.Sheets[0].Columns.Count - 1].CellType = cTextType;

            if (kutuphaneGoster)
                kontrol.Sheets[0].Columns[kutuphaneKolonBasla, kontrol.Sheets[0].Columns.Count - 1].Visible = true;
            else if (muzeGoster)
                kontrol.Sheets[0].Columns[muzeKolonBasla, kontrol.Sheets[0].Columns.Count - 1].Visible = true;
            else
                kontrol.Sheets[0].Columns[ozellikKolonBasla + 1, kontrol.Sheets[0].Columns.Count - 1].Visible = false;
        }

        /// <summary>
        /// Belgeyi Bul resmine basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri taþýnýr iþlem form nesnesine doldurulur, sunucuya
        /// gönderilir ve taþýnýr iþlem fiþi bilgileri sunucudan alýnýr. Hata varsa
        /// ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, ImageClickEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
            BaslikBilgileriniAyarla();

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            int islemTuru = 0;

            fpL.CancelEdit();
            fpL.Sheets[0].RowCount = 0;
            GridInit(fpL);

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.muhasebeKod = txtMuhasebe.Text;
            tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

            if (bilgi.sonuc.islemSonuc)
            {
                TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
                tform = (TNS.TMM.TasinirIslemForm)bilgi[0];

                foreach (ListItem li in ddlIslemTipi.Items)
                {
                    if (li.Value.Split('*')[0] == tform.islemTipKod.ToString())
                    {
                        ddlIslemTipi.SelectedValue = li.Value;
                        break;
                    }
                }

                islemTuru = OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0);
                IslemGizle(islemTuru);

                if (tform.durum == (int)ENUMBelgeDurumu.YENI || tform.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    lblFormDurum.Text = Resources.TasinirMal.FRMTIG055;
                else if (tform.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    lblFormDurum.Text = Resources.TasinirMal.FRMTIG056;
                else if (tform.durum == (int)ENUMBelgeDurumu.IPTAL)
                    lblFormDurum.Text = Resources.TasinirMal.FRMTIG057;

                txtBelgeTarih.Text = tform.fisTarih.ToString();
                txtAmbar.Text = tform.ambarKod;
                lblAmbarAd.Text = tform.ambarAd;
                txtMuayeneNo.Text = tform.muayeneNo;
                txtMuayeneTarih.Text = tform.muayeneTarih.ToString();
                txtFaturaNo.Text = tform.faturaNo;
                txtFaturaTarih.Text = tform.faturaTarih.ToString();
                txtDayanakNo.Text = tform.dayanakNo;
                txtDayanakTarih.Text = tform.dayanakTarih.ToString();
                txtNereyeGitti.Text = tform.nereyeGitti;
                txtNeredenGeldi.Text = tform.neredenGeldi;
                txtKimeGitti.Text = tform.kimeGitti;
                txtGonMuhasebe.Text = tform.gMuhasebeKod;
                txtGonHarcamaBirimi.Text = tform.gHarcamaKod;
                txtGonAmbar.Text = tform.gAmbarKod;
                txtGonBelgeNo.Text = tform.gFisNo;
                if (tform.gYil > 2000)//null kayit edilmesi ise kontrolü
                    ddlGonYil.SelectedValue = tform.gYil.ToString();

                lblGonMuhasebeAd.Text = tform.gMuhasebeAd;
                lblGonHarcamaBirimiAd.Text = tform.gHarcamaAd;
                lblGonAmbarAd.Text = tform.gAmbarAd;
                if (txtKimeGitti.Text.Trim() != "")
                    lblKimeGittiAd.Text = GenelIslemler.KodAd(36, txtKimeGitti.Text.Trim(), true);
                else
                    lblKimeGittiAd.Text = "";

                fpL.Sheets[0].RowCount = tform.detay.ObjeSayisi;
                int i = 0;
                foreach (TasinirIslemDetay td in tform.detay.objeler)
                {
                    fpL.Cells[i, 0].Text = td.hesapPlanKod;
                    fpL.Cells[i, 2].Text = islemTuru != (int)ENUMIslemTipi.ACILIS ? td.gorSicilNo : string.Empty;
                    fpL.Cells[i, 4].Value = td.miktar.ToString();
                    fpL.Cells[i, 5].Text = td.olcuBirimAd;
                    fpL.Cells[i, 6].Value = td.kdvOran.ToString();
                    fpL.Cells[i, 7].Value = td.birimFiyat.ToString();
                    fpL.Cells[i, 8].Text = td.hesapPlanAd;

                    if (kutuphaneGoster)
                    {
                        fpL.Cells[i, kutuphaneKolonBasla].Text = td.ozellik.disSicilNo;
                        fpL.Cells[i, kutuphaneKolonBasla + 1].Text = td.ozellik.ciltNo;
                        fpL.Cells[i, kutuphaneKolonBasla + 2].Text = td.ozellik.dil;
                        fpL.Cells[i, kutuphaneKolonBasla + 3].Text = td.ozellik.yazarAdi;
                        fpL.Cells[i, kutuphaneKolonBasla + 4].Text = td.ozellik.adi;
                        fpL.Cells[i, kutuphaneKolonBasla + 5].Text = td.ozellik.yayinYeri;
                        fpL.Cells[i, kutuphaneKolonBasla + 6].Text = td.ozellik.yayinTarihi;
                        fpL.Cells[i, kutuphaneKolonBasla + 7].Text = td.ozellik.neredenGeldi;
                        fpL.Cells[i, kutuphaneKolonBasla + 8].Text = td.ozellik.boyutlari;
                        fpL.Cells[i, kutuphaneKolonBasla + 9].Text = td.ozellik.satirSayisi;
                        fpL.Cells[i, kutuphaneKolonBasla + 10].Text = td.ozellik.yaprakSayisi;
                        fpL.Cells[i, kutuphaneKolonBasla + 11].Text = td.ozellik.sayfaSayisi;
                        fpL.Cells[i, kutuphaneKolonBasla + 12].Text = td.ozellik.ciltTuru;
                        fpL.Cells[i, kutuphaneKolonBasla + 13].Text = td.ozellik.cesidi;
                        fpL.Cells[i, kutuphaneKolonBasla + 14].Text = td.ozellik.yeriKonusu;
                    }
                    else if (muzeGoster)
                    {
                        fpL.Cells[i, muzeKolonBasla].Text = td.ozellik.disSicilNo;
                        fpL.Cells[i, muzeKolonBasla + 1].Text = td.ozellik.adi;
                        fpL.Cells[i, muzeKolonBasla + 2].Text = td.ozellik.gelisTarihi;
                        fpL.Cells[i, muzeKolonBasla + 3].Text = td.ozellik.neredenGeldi;
                        fpL.Cells[i, muzeKolonBasla + 4].Text = td.ozellik.neredeBulundu;
                        fpL.Cells[i, muzeKolonBasla + 5].Text = td.ozellik.cagi;
                        fpL.Cells[i, muzeKolonBasla + 6].Text = td.ozellik.boyutlari;
                        fpL.Cells[i, muzeKolonBasla + 7].Text = td.ozellik.durumuMaddesi;
                        fpL.Cells[i, muzeKolonBasla + 8].Text = td.ozellik.onYuz;
                        fpL.Cells[i, muzeKolonBasla + 9].Text = td.ozellik.arkaYuz;
                        fpL.Cells[i, muzeKolonBasla + 10].Text = td.ozellik.puan;
                        fpL.Cells[i, muzeKolonBasla + 11].Text = td.ozellik.yeriKonusu;
                    }
                    else if (islemTuru == (int)ENUMIslemTipi.ACILIS)
                    {
                        fpL.Cells[i, fpL.Sheets[0].Columns.Count - 4].Text = td.eAciklama;
                        fpL.Cells[i, fpL.Sheets[0].Columns.Count - 3].Text = td.eSicilNo;
                        fpL.Cells[i, fpL.Sheets[0].Columns.Count - 2].Text = td.eAlimTarihi.ToString();
                        fpL.Cells[i, fpL.Sheets[0].Columns.Count - 1].Text = td.eTedarikSekli;
                    }
                    else
                        fpL.Cells[i, fpL.Sheets[0].Columns.Count - 4].Text = td.eAciklama;

                    i++;
                }
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Sayfadaki ilgili kontroller verilen iþlem türüne göre gizlenir, gösterilir.
        /// </summary>
        /// <param name="islemTuru">ENUMIslemTipi listesindeki deðerlerden biri olmalýdýr.</param>
        private void IslemGizle(int islemTuru)
        {
            BaslikBilgileriniAyarla();

            if (kutuphaneGoster)
                fpL.Sheets[0].Columns[kutuphaneKolonBasla, fpL.Sheets[0].Columns.Count - 1].Visible = true;
            else if (muzeGoster)
                fpL.Sheets[0].Columns[muzeKolonBasla, fpL.Sheets[0].Columns.Count - 1].Visible = true;
            else
                fpL.Sheets[0].Columns[ozellikKolonBasla + 1, fpL.Sheets[0].Columns.Count - 1].Visible = false;

            btnOzellik.Visible = false;
            fpL.Sheets[0].Columns[2].ForeColor = System.Drawing.Color.Black;
            GridKolonKilitleAc(0, 7, false);
            GridKolonKilitleAc(5, 5, true);
            GridKolonKilitleAc(8, 8, true);

            if (!(islemTuru == (int)ENUMIslemTipi.YILDEVIRGIRIS || islemTuru == (int)ENUMIslemTipi.TUKETIMCIKIS ||
                islemTuru == (int)ENUMIslemTipi.KULLANILMAZCIKIS || islemTuru == (int)ENUMIslemTipi.YILDEVIRCIKIS))
                btnSicilYazdir.Visible = true;
            else
                btnSicilYazdir.Visible = false;

            if (islemTuru == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, true, true, true, true, false, false, false, false);
                GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.BAGISGIRIS)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, true, true, false, false, false, false, false, false);
                GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.IADEGIRIS)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, true, true, false, false, false, false, false, false);
                GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRGIRIS || islemTuru == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                imgDevir.Visible = true;
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, true, true, false);
                GridKolonKilitleAc(0, fpL.Sheets[0].ColumnCount - 1, true);
                fpL.Sheets[0].Columns[2].ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            }
            else if (islemTuru == (int)ENUMIslemTipi.URETILENGIRIS)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.TUKETIMCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, true);
                GridKolonKilitleAc(2, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRCIKIS || islemTuru == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
            {
                imgDevir.Visible = false;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, true, false, false);
                GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.BAGISCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.SATISCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, true, false, true, false, false, false);
                GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.KULLANILMAZCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                GridKolonKilitleAc(2, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.HURDACIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                GridKolonKilitleAc(0, 1, true);
                GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.ACILIS)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                if (!kutuphaneGoster && !muzeGoster)
                    fpL.Sheets[0].Columns[ozellikKolonBasla, fpL.Sheets[0].Columns.Count - 1].Visible = true;
                GridKolonKilitleAc(2, 3, true);
                GridKolonKilitleAc(6, 7, false);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEGERARTTIR)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.ENFLASYONARTISI)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.YILDEVIRGIRIS)
            {
                FormAlanGosterGizle(islemTuru, false, false, false, false, false, false, true, false);
                GridKolonKilitleAc(0, fpL.Sheets[0].ColumnCount - 1, true);
                fpL.Sheets[0].Columns[2].ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            }
            else if (islemTuru == (int)ENUMIslemTipi.YILDEVIRCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, false, false, false, false, false, false, false);
                GridKolonKilitleAc(2, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRGIRISKURUM)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, true, true, false, false, false, false, false, false);
                GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRCIKISKURUM)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.SAYIMNOKSANICIKIS)
            {
                btnOzellik.Visible = true;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                GridKolonKilitleAc(2, 3, true);
            }
        }

        /// <summary>
        /// Sayfadaki kontrollerin htmle çevrilmesini yapan yordam
        /// </summary>
        /// <param name="writer">Kontrollerin içeriðini yazan nesne</param>
        protected override void Render(HtmlTextWriter writer)
        {
            GenelIslemler.ListeYazdirDugmeGizle(fpL.FindControl("Print"));
            GenelIslemler.ListeYazdirDugmeGizle(fpL.FindControl("Cancel"));
            GenelIslemler.ListeYazdirDugmeGizle(fpL.FindControl("Update"));

            Control updateBtn = fpL.FindControl("Paste");
            if (updateBtn != null)
            {
                TableCell tc = (TableCell)updateBtn.Parent;
                TableRow tr = (TableRow)tc.Parent;

                TableCell tc1 = new TableCell();
                tr.Cells.Add(tc1);

                Image img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG058;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG059;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG060;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG061;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/sigma.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG062;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ToplamHesapla(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow50.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG063;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc50(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow100.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG064;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc100(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow250.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG065;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc250(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertLastRow500.gif";
                img.AlternateText = Resources.TasinirMal.FRMTIG066;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc500(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);
            }

            base.Render(writer);
        }

        /// <summary>
        /// fgrid grid kontrolü ile ilgili boþ satýr ekleme, araya
        /// satýr ekleme ve satýr silme iþlemlerinin yapýldýðý yordam
        /// </summary>
        /// <param name="tur">Ýþlemin ne olduðu</param>
        void fpL_ButtonCommand(string tur)
        {
            if (tur == "bossatirekle")
            {
                fpL.ActiveSheetView.RowCount += ekleSatirSayisi;
            }
            if (tur == "arayasatirekle")
            {
                try
                {
                    int aktifSatir = fpL.ActiveSheetView.ActiveRow;
                    int acSatir = Math.Abs(aktifSatir - fpL.ActiveSheetView.SelectionModel.LeadRow) + 1;
                    fpL.ActiveSheetView.AddRows(aktifSatir, acSatir);
                }
                catch { }
            }
            if (tur == "satirsil")
            {
                try
                {
                    int aktifSatir = fpL.ActiveSheetView.ActiveRow;
                    int acSatir = Math.Abs(aktifSatir - fpL.ActiveSheetView.SelectionModel.LeadRow) + 1;
                    fpL.ActiveSheetView.RemoveRows(aktifSatir, acSatir);
                }
                catch { }
            }
            if (tur == "bossatirekle50")
            {
                fpL.ActiveSheetView.RowCount += 50;
            }
            if (tur == "bossatirekle100")
            {
                fpL.ActiveSheetView.RowCount += 100;
            }
            if (tur == "bossatirekle250")
            {
                fpL.ActiveSheetView.RowCount += 250;
            }
            if (tur == "bossatirekle500")
            {
                fpL.ActiveSheetView.RowCount += 500;
            }

            fpL.SaveChanges();
        }

        /// <summary>
        /// Ýþlem tipi seçimi deðiþtiðinde çalýþan olay metodu
        /// Seçilen iþlem tipi ile ilgili sayfada ayarlamalar yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void ddlIslemTipi_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridKolonKilitleAc(0, fpL.Sheets[0].ColumnCount - 1, false);

            if (islemTipiDetayiTemizlesin)
                GridInit(fpL);//bu açýklama, bu satýr kapalý iken geçerli idi.(Melih) Sayýmdan veya kayýt düþmeden gelen bilgi dropdown bilgisi diðiþtirildiðinde kaybloluyordu.

            int islemTuru = OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0);
            IslemGizle(islemTuru);

            if (islemTuru == (int)ENUMIslemTipi.YILDEVIRCIKIS)
            {
                StokHareketBilgi shBilgi = new StokHareketBilgi();
                shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
                shBilgi.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "").Trim();
                shBilgi.muhasebeKod = txtMuhasebe.Text.Trim();
                shBilgi.ambarKod = txtAmbar.Text.Trim();

                ObjectArray bilgi = servisTMM.TuketimListele(kullanan, shBilgi);

                if (bilgi.sonuc.islemSonuc)
                {
                    if (bilgi.objeler.Count > 0)
                    {
                        fpL.Sheets[0].RowCount = bilgi.objeler.Count;
                        int satir = 0;
                        foreach (StokHareketBilgi sBilgi in bilgi.objeler)
                        {
                            fpL.Sheets[0].Cells[satir, 0].Value = sBilgi.hesapPlanKod;
                            fpL.Sheets[0].Cells[satir, 4].Value = sBilgi.miktar;
                            fpL.Sheets[0].Cells[satir, 5].Value = "-";
                            fpL.Sheets[0].Cells[satir, 6].Value = sBilgi.kdvOran;
                            fpL.Sheets[0].Cells[satir, 7].Value = sBilgi.birimFiyat;
                            fpL.Sheets[0].Cells[satir, 8].Value = sBilgi.hesapPlanAd;
                            satir++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sayfadaki bazý bölümlerin gösterililmesi, gizlenmesi iþlemlerini yapan yordam
        /// </summary>
        /// <param name="islemTuru">The islem turu.</param>
        /// <param name="neredenGeldi">divNeredenGeldi bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        /// <param name="dayanakBelge">divDayanakBelge bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        /// <param name="fatura">divFatura bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        /// <param name="komisyon">divKomisyon bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        /// <param name="nereyeGitti">divNereyeGitti bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        /// <param name="gelenGonderilen">divGonderilenBirim bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        /// <param name="gelen">divGonderilenBelge bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        /// <param name="kimeGitti">divKimeGitti bölümünün gösterilip gösterilmeyeceði bilgisi</param>
        private void FormAlanGosterGizle(int islemTuru, bool neredenGeldi, bool dayanakBelge, bool fatura, bool komisyon, bool nereyeGitti, bool gelenGonderilen, bool gelen, bool kimeGitti)
        {
            divNeredenGeldi.Style["display"] = neredenGeldi == true ? "block" : "none";
            divDayanakBelge.Style["display"] = dayanakBelge == true ? "block" : "none";
            divFatura.Style["display"] = fatura == true ? "block" : "none";
            divKomisyon.Style["display"] = komisyon == true ? "block" : "none";
            divNereyeGitti.Style["display"] = nereyeGitti == true ? "block" : "none";
            divKimeGitti.Style["display"] = kimeGitti == true ? "block" : "none";
            divGonderilenBirim.Style["display"] = gelenGonderilen == true ? "block" : "none";
            divGonderilenBelge.Style["display"] = gelen == true ? "block" : "none";

            bool dagitim = (islemTuru == (int)ENUMIslemTipi.DAGITIMIADECIKIS || islemTuru == (int)ENUMIslemTipi.DAGITIMIADEGIRIS);
            divGonderilenMuhasebe.Style["display"] = dagitim == true ? "none" : "block";
            divGonderilenHarcamaBirimi.Style["display"] = dagitim == true ? "none" : "block";
            //divIslemTipi.Style["display"] = dagitim == true ? "none" : "block";
            divDayanakBelge.Style["display"] = dagitim == true ? "none" : "block";

            bool doviz = (islemTuru == (int)ENUMIslemTipi.ACILIS ||
                          islemTuru == (int)ENUMIslemTipi.BAGISGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.DAGITIMIADEGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.DEVIRGIRISKURUM ||
                          islemTuru == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.URETILENGIRIS);

            string paraBirimi = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEPARABIRIMI");
            if (paraBirimi != "")
            {
                divDoviz.Style["display"] = doviz == true ? "block" : "none";
                ddlDoviz.SelectedIndex = 0;
            }

            ////***Kullanýcý birimi þeklinde çalýþýyor ise devir ambarýný gösterme*****************************
            //int devirSekli = OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRKULLANICIBIRIMI"), 0);
            //if (devirSekli > 0)
            //{
            //    bool devir = (islemTuru == (int)ENUMIslemTipi.DEVIRCIKIS || islemTuru == (int)ENUMIslemTipi.DEVIRGIRIS);
            //    divGonderilenAmbar.Style["display"] = devir == true ? "none" : "block";
            //}
        }

        /// <summary>
        /// Parametre olarak verilen kolon numaralarý arasýndaki kolonlarý kilitleyen/kilidi açan yordam
        /// </summary>
        /// <param name="basKolonNo">Ýlk kolon numarasý</param>
        /// <param name="bitKolonNo">Son kolon numarasý</param>
        /// <param name="kilitleAc">Kilitlenecek mi açýlacak mý bilgisi</param>
        private void GridKolonKilitleAc(int basKolonNo, int bitKolonNo, bool kilitleAc)
        {
            fpL.Sheets[0].Columns[basKolonNo, bitKolonNo].Locked = kilitleAc;
            if (kilitleAc)
                fpL.Sheets[0].Columns[basKolonNo, bitKolonNo].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            else
                fpL.Sheets[0].Columns[basKolonNo, bitKolonNo].BackColor = System.Drawing.Color.Transparent;
        }

        /// <summary>
        /// Belgeyi Bul resmine basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri taþýnýr iþlem form nesnesine doldurulur, sunucuya
        /// gönderilir ve taþýnýr iþlem fiþi bilgileri sunucudan alýnýr. Hata varsa
        /// ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// Devir giriþ ve yýl devri giriþ iþlem türlerinde kullanýlan bir yordamdýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnGonListele_Click(object sender, ImageClickEventArgs e)
        {
            int islemTuruSecili = 0;
            islemTuruSecili = OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0);

            string hata = "";

            if (islemTuruSecili == (int)ENUMIslemTipi.DEVIRGIRIS)
            {
                if (txtGonMuhasebe.Text.Trim() == "")
                    hata = Resources.TasinirMal.FRMTIG067 + "<br>";

                if (txtGonHarcamaBirimi.Text.Trim() == "")
                    hata += Resources.TasinirMal.FRMTIG068 + "<br>";

                if (txtGonBelgeNo.Text.Trim() == "")
                    hata += Resources.TasinirMal.FRMTIG069 + "<br>";
            }

            if (islemTuruSecili == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                if (txtGonBelgeNo.Text.Trim() == "")
                    hata += Resources.TasinirMal.FRMTIG069 + "<br>";
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            int islemTuru = 0;

            fpL.Sheets[0].RowCount = 0;
            GridInit(fpL);

            IslemGizle(islemTuruSecili);

            txtGonBelgeNo.Text = txtGonBelgeNo.Text.Trim().PadLeft(6, '0');

            if (islemTuruSecili == (int)ENUMIslemTipi.DEVIRGIRIS)
            {
                tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlGonYil.SelectedValue, 0);
                tf.harcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                tf.muhasebeKod = txtGonMuhasebe.Text;
                tf.fisNo = txtGonBelgeNo.Text;
                tf.devirGirisiMi = true;
            }
            else if (islemTuruSecili == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlGonYil.SelectedValue, 0);
                tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
                tf.muhasebeKod = txtMuhasebe.Text;
                tf.fisNo = txtGonBelgeNo.Text;
                tf.devirGirisiMi = true;
            }
            else if (islemTuruSecili == (int)ENUMIslemTipi.YILDEVIRGIRIS)
            {
                tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlGonYil.SelectedValue, 0);
                tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
                tf.muhasebeKod = txtMuhasebe.Text;
                tf.fisNo = txtGonBelgeNo.Text;
            }

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);


            if (bilgi.sonuc.islemSonuc)
            {
                tf = (TNS.TMM.TasinirIslemForm)bilgi.objeler[0];
                if (tf.durum != (int)ENUMBelgeDurumu.ONAYLI)
                {
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG070);
                    return;
                }

                islemTuru = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(kullanan, "TASISLEMTIPTUR", tf.islemTipKod.ToString(), true), 0);

                if ((islemTuruSecili == (int)ENUMIslemTipi.DEVIRGIRIS || islemTuruSecili == (int)ENUMIslemTipi.DAGITIMIADEGIRIS) &&
                    (islemTuru != (int)ENUMIslemTipi.DEVIRCIKIS && islemTuru != (int)ENUMIslemTipi.DAGITIMIADECIKIS))
                {
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG071);
                    return;
                }

                if (islemTuruSecili == (int)ENUMIslemTipi.YILDEVIRGIRIS &&
                    islemTuru != (int)ENUMIslemTipi.YILDEVIRCIKIS)
                {
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG072);
                    return;
                }

                if (islemTuruSecili == (int)ENUMIslemTipi.DEVIRGIRIS || islemTuruSecili == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                {
                    string mK = txtGonMuhasebe.Text.Trim();
                    string hK = txtGonHarcamaBirimi.Text.Trim().Replace(".", "");
                    string aK = txtGonAmbar.Text.Trim();

                    if (islemTuruSecili == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    {
                        mK = txtMuhasebe.Text.Trim();
                        hK = txtHarcamaBirimi.Text.Trim();
                    }


                    ////***Kullanýcý birimi þeklinde çalýþýyor ise devir ambarýný gösterme*****************************
                    //int devirSekli = OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRKULLANICIBIRIMI"), 0);
                    //if (devirSekli > 0)
                    //    aK = tf.ambarKod;

                    if (tf.muhasebeKod != mK ||
                        tf.harcamaKod.Replace(".", "") != hK.Replace(".", "") ||
                        tf.ambarKod != aK)
                    {
                        GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG073);
                        return;
                    }
                }

                fpL.Sheets[0].RowCount = tf.detay.ObjeSayisi;

                int i = 0;
                foreach (TasinirIslemDetay td in tf.detay.objeler)
                {
                    fpL.Cells[i, 0].Text = td.hesapPlanKod;
                    fpL.Cells[i, 2].Text = td.gorSicilNo;
                    fpL.Cells[i, 4].Text = td.miktar.ToString();
                    fpL.Cells[i, 5].Text = td.olcuBirimAd;
                    fpL.Cells[i, 6].Text = td.kdvOran.ToString();
                    fpL.Cells[i, 7].Text = td.birimFiyat.ToString();
                    fpL.Cells[i, 8].Text = td.hesapPlanAd;
                    fpL.Cells[i, 9].Text = td.eAciklama;

                    if (kutuphaneGoster)
                    {
                        fpL.Cells[i, kutuphaneKolonBasla].Text = td.ozellik.disSicilNo;
                        fpL.Cells[i, kutuphaneKolonBasla + 1].Text = td.ozellik.ciltNo;
                        fpL.Cells[i, kutuphaneKolonBasla + 2].Text = td.ozellik.dil;
                        fpL.Cells[i, kutuphaneKolonBasla + 3].Text = td.ozellik.yazarAdi;
                        fpL.Cells[i, kutuphaneKolonBasla + 4].Text = td.ozellik.adi;
                        fpL.Cells[i, kutuphaneKolonBasla + 5].Text = td.ozellik.yayinYeri;
                        fpL.Cells[i, kutuphaneKolonBasla + 6].Text = td.ozellik.yayinTarihi;
                        fpL.Cells[i, kutuphaneKolonBasla + 7].Text = td.ozellik.neredenGeldi;
                        fpL.Cells[i, kutuphaneKolonBasla + 8].Text = td.ozellik.boyutlari;
                        fpL.Cells[i, kutuphaneKolonBasla + 9].Text = td.ozellik.satirSayisi;
                        fpL.Cells[i, kutuphaneKolonBasla + 10].Text = td.ozellik.yaprakSayisi;
                        fpL.Cells[i, kutuphaneKolonBasla + 11].Text = td.ozellik.sayfaSayisi;
                        fpL.Cells[i, kutuphaneKolonBasla + 12].Text = td.ozellik.ciltTuru;
                        fpL.Cells[i, kutuphaneKolonBasla + 13].Text = td.ozellik.cesidi;
                        fpL.Cells[i, kutuphaneKolonBasla + 14].Text = td.ozellik.yeriKonusu;
                    }
                    else if (muzeGoster)
                    {
                        fpL.Cells[i, muzeKolonBasla].Text = td.ozellik.disSicilNo;
                        fpL.Cells[i, muzeKolonBasla + 1].Text = td.ozellik.adi;
                        fpL.Cells[i, muzeKolonBasla + 2].Text = td.ozellik.gelisTarihi;
                        fpL.Cells[i, muzeKolonBasla + 3].Text = td.ozellik.neredenGeldi;
                        fpL.Cells[i, muzeKolonBasla + 4].Text = td.ozellik.neredeBulundu;
                        fpL.Cells[i, muzeKolonBasla + 5].Text = td.ozellik.cagi;
                        fpL.Cells[i, muzeKolonBasla + 6].Text = td.ozellik.boyutlari;
                        fpL.Cells[i, muzeKolonBasla + 7].Text = td.ozellik.durumuMaddesi;
                        fpL.Cells[i, muzeKolonBasla + 8].Text = td.ozellik.onYuz;
                        fpL.Cells[i, muzeKolonBasla + 9].Text = td.ozellik.arkaYuz;
                        fpL.Cells[i, muzeKolonBasla + 10].Text = td.ozellik.puan;
                        fpL.Cells[i, muzeKolonBasla + 11].Text = td.ozellik.yeriKonusu;
                    }
                    i++;
                }
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Yükle tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri verilen excel dosyasýndan okunur
        /// ve sorun yoksa ekrana yazýlýr, varsa hata mesajý görüntülenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDosyaYukle_Click(object sender, System.EventArgs e)
        {
            if (fileListe.PostedFile == null)
                return;
            else
            {
                System.Web.HttpPostedFile myFile = fileListe.PostedFile;
                int nFileLen = myFile.ContentLength;
                if (nFileLen > 0)
                {
                    byte[] myData = new byte[nFileLen];

                    myFile.InputStream.Read(myData, 0, nFileLen);

                    string dosyaAd = System.IO.Path.GetTempFileName();

                    System.IO.FileStream newFile = new System.IO.FileStream(dosyaAd, System.IO.FileMode.Create);
                    newFile.Write(myData, 0, myData.Length);
                    newFile.Close();

                    if (dosyaAd != "")
                    {
                        try
                        {
                            fpL.OpenExcel(dosyaAd, FarPoint.Excel.ExcelOpenFlags.DataOnly);

                            dosyaYukle = true;
                            System.IO.File.Delete(dosyaAd);
                        }
                        catch
                        {
                            GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTIG074, dosyaAd));
                        }
                        GridInit(fpL);
                        dosyaYukle = false;

                        for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
                        {
                            if (fpL.Sheets[0].Cells[i, 0].Text.Trim() == "")
                            {
                                fpL.Sheets[0].RowCount = i;
                                break;
                            }
                        }

                        int maxSatir = 1000;
                        if (fpL.Sheets[0].RowCount > maxSatir)
                        {
                            GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTIG075, maxSatir.ToString(), fpL.Sheets[0].RowCount.ToString()));
                            fpL.CancelEdit();
                            fpL.Sheets[0].RowCount = 0;
                            fpL.Sheets[0].RowCount = ekleSatirSayisi;
                            return;
                        }

                        int yuklenenSatirSayisi = 0;

                        yuklenenSatirSayisi = fpL.Sheets[0].RowCount;

                        for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
                        {
                            HesapPlaniSatir h = new HesapPlaniSatir();

                            if (!kutuphaneGoster && !muzeGoster) //Kütüphane ve Müze'de alým tarihi yok
                            {
                                try
                                {
                                    h.hesapKod = fpL.Sheets[0].DataModel.GetValue(i, 0).ToString();
                                    if (fpL.Sheets[0].Cells[i, 11].Text.Trim() != "")
                                    {
                                        TNSDateTime alimTarih = new TNSDateTime(fpL.Sheets[0].Cells[i, 11].Text);
                                        fpL.Sheets[0].Cells[i, 11].Value = alimTarih.ToString();
                                    }
                                }
                                catch { }
                            }

                            if (h.hesapKod == null || h.hesapKod == string.Empty)
                                continue;

                            ObjectArray o = servisTMM.HesapPlaniListele(new TNS.KYM.Kullanici(), h, new Sayfalama());
                            foreach (HesapPlaniSatir hs in o.objeler)
                            {
                                fpL.Sheets[0].DataModel.SetValue(i, 5, hs.olcuBirimAd);
                                fpL.Sheets[0].DataModel.SetValue(i, 8, hs.aciklama);
                            }
                        }

                        fpL.Sheets[0].RowCount = fpL.Sheets[0].RowCount + ekleSatirSayisi;
                        IslemGizle(OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0));
                        GenelIslemler.MesajKutusu("Bilgi", string.Format(Resources.TasinirMal.FRMTIG076, yuklenenSatirSayisi.ToString()));
                    }
                    else
                        GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG077);
                }
                else
                    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG078);
            }
        }

        /// <summary>
        /// Sakla tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandaki taþýnýr iþlem fiþi detay satýr bilgileri excel dosyasýna yazýlýr ve kullanýcýya gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDosyaSakla_Click(object sender, System.EventArgs e)
        {
            string dosyaAd = System.IO.Path.GetTempFileName();
            fpL.SaveChanges();

            try
            {
                fpL.SaveExcel(dosyaAd, FarPoint.Web.Spread.Model.IncludeHeaders.BothCustomOnly);
                OrtakClass.GenelIslemler.ResponseBasla("TifDetay.xls", "xls");
                System.Web.HttpContext.Current.Response.WriteFile(dosyaAd);
                OrtakClass.GenelIslemler.ResponseBitir();
                System.IO.File.Delete(dosyaAd);
            }
            catch
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG079);
            }
        }

        /// <summary>
        /// Sayfadaki farpoint grid kontrolünün format bilgilerini sessiona saklayan ya da okuyan yordam
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void fpL_SaveOrLoadSheetState(object sender, FarPoint.Web.Spread.SheetViewStateEventArgs e)
        {
            object o;
            object temp = null;

            if (e.IsSave)
                Session["SpreadData" + e.Index + ViewState["fpID"]] = e.SheetView.SaveViewState();
            else
            {
                o = Session["SpreadData" + e.Index + ViewState["fpID"]];
                if (!(object.ReferenceEquals(o, temp)))
                    e.SheetView.LoadViewState(o);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Sayfa adresinde gelen kutuphane ve muze girdi dizgilerinin deðerlerine göre ilgili deðiþkenleri ayarlar.
        /// </summary>
        private void BaslikBilgileriniAyarla()
        {
            if (Request.QueryString["kutuphane"] + "" != "")
            {
                kutuphaneGoster = true;
                txtBelgeTur.Value = "kutuphane";
            }
            else
                kutuphaneGoster = false;

            if (Request.QueryString["muze"] + "" != "")
            {
                muzeGoster = true;
                txtBelgeTur.Value = "muze";
            }
            else
                muzeGoster = false;

            if (Request.QueryString["dagitimIade"] + "" != "")
            {
                txtBelgeTur.Value = "dagitimIade";
                dagitimIade = true;
            }
            else
                dagitimIade = false;
        }

        /// <summary>
        /// Onay Kaldýr tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onayý kaldýrýlmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayKaldir_Click(object sender, EventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG080 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG081 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG082 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata + Resources.TasinirMal.FRMTIG083);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "OnayKaldir");

            if (sonuc.islemSonuc)
            {
                lblFormDurum.Text = Resources.TasinirMal.FRMTIG084;

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", sonuc.hataStr);
        }

        /// <summary>
        /// Handles the Click event of the btnSicilNoYukle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSicilNoYukle_Click(object sender, EventArgs e)
        {
            ObjectArray o = (ObjectArray)OturumBilgisiIslem.BilgiOkuDegisken("DevirSicilNolar", false);

            fpL.Sheets[0].RowCount = fpL.Sheets[0].RowCount + o.objeler.Count + 10;

            int sayac = fpL.Sheets[0].ActiveRow;
            foreach (TasinirIslemDetay ts in o.objeler)
            {

                fpL.Sheets[0].Cells[sayac, 0].Text = ts.hesapPlanKod;
                fpL.Sheets[0].Cells[sayac, 2].Text = ts.gorSicilNo;
                fpL.Sheets[0].Cells[sayac, 4].Text = ts.miktar.ToString();
                fpL.Sheets[0].Cells[sayac, 5].Text = "Adet";
                fpL.Sheets[0].Cells[sayac, 6].Text = ts.kdvOran.ToString();
                fpL.Sheets[0].Cells[sayac, 7].Text = ts.birimFiyat.ToString();
                fpL.Sheets[0].Cells[sayac, 8].Text = Server.HtmlDecode(ts.hesapPlanAd);

                sayac++;
            }
            OturumBilgisiIslem.BilgiYazDegisken("DevirSicilNolar", null);
        }
    }
}