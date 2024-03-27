using Ext1.Net;
using Newtonsoft.Json.Linq;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TNS;
using TNS.DEG;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþi bilgilerinin kayýt, listeleme, onaylama, onay kaldýrma ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIslemFormMB : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Excel dosyasýndan bilgi yükleme iþleminin yapýlýp yapýlmadýðýný tutan deðiþken
        /// </summary>
        //bool dosyaYukle = false;

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
        /// Ýþlem tipi deðiþtiðinde gridi formatlansýn mý bilgisini tutan deðiþken
        /// </summary>
        bool islemTipiDetayiTemizlesin = true;

        string acilistaIslemTipiSec = "";

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
            if (!X.IsAjaxRequest)
            {
                BAOnayDugmesiAyarla(0);

                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                bool izin = false;
                if (dagitimIade)
                    izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan);
                else
                    izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan);

                if (!izin)
                    GenelIslemler.SayfayaGirmesin(true);


                //if (Request.QueryString["aramaYok"] + "" != "")
                //{
                //    pnlDosyaYukle.Visible = false;
                //}

                BaslikBilgileriniAyarla();
                hdnFirmaHarcamadanAlma.Value = TasinirGenel.tasinirFirmaBilgisiniHarcamadanAlma;

                hdnHesapKod.Value = (kutuphaneGoster ? "255.07" : (muzeGoster ? "255.06" : ""));
                YilDoldur();
                if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    DovizDoldur();
                IslemTipiDoldur();

                txtBelgeTarih.Value = DateTime.Now.Date;
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    txtAciklama.FieldLabel = "Dekont Açýklamasý";
                    hdnKurumTur.Value = "MerkezBanka";
                    MarkaDoldur();
                    ModelDoldur();
                    RFIDEtiketTuruDoldur();
                    grdListe.ColumnModel.SetHidden(7, true); //KDV ORan
                    grdListe.ColumnModel.SetColumnHeader(8, "Birim Fiyat");
                    grdListe.ColumnModel.SetColumnTooltip(8, "Birim Fiyat");


                    string ekranTip = EkranTurDondur();
                    hdnEkranTip.Value = ekranTip;
                    if (ekranTip == "YZ")
                    {
                        YZTurDoldur();
                        ResourceManager1.Listeners.DocumentReady.Handler += "DemirbasYazilimBilgisiYaz('yazilim');";
                    }
                    else if (ekranTip == "GM")
                    {
                        GMTurDoldur();
                        ResourceManager1.Listeners.DocumentReady.Handler += "DemirbasYazilimBilgisiYaz('gayrimenkul');";
                    }
                    else
                    {
                        txtAmbar.Value = "01";
                        lblAmbarAd.Text = "Malzeme Ambarý";
                        ResourceManager1.Listeners.DocumentReady.Handler += "DemirbasYazilimBilgisiYaz('demirbas');";
                    }

                    txtAmbar.Hide();
                    txtAmbar.FieldLabel = "Ambar";


                    hdnMutemmim.Value = MutemmimAktifMi() ? 1 : 0;

                    if (OrtakFonksiyonlar.ConvertToInt(hdnMutemmim.Value, 0) == 1)
                        pnlMutemmim.Show();
                }

                GridTemizle();

                if (Request.QueryString["belgeNo"] + "" == "")
                {

                    //Devir giriþi yapýlmamýþ kayýt var mý kontrolü 
                    //****************************************************
                    TNS.TMM.TasinirIslemForm kriter = new TNS.TMM.TasinirIslemForm();
                    kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
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


                                    //for (int i = 0; i < ddlIslemTipi.Items.Count; i++)
                                    //{
                                    //    string[] islem = ddlIslemTipi.Items[i].Value.Split('*');
                                    //    if (OrtakFonksiyonlar.ConvertToInt(islem[1], 0) == (int)ENUMIslemTipi.DEVIRGIRIS ||
                                    //        OrtakFonksiyonlar.ConvertToInt(islem[1], 0) == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                                    //    {
                                    //        ddlIslemTipi.SelectedIndex = i;
                                    //        break;
                                    //    }
                                    //}

                                    int islemTur = (int)ENUMIslemTipi.DEVIRGIRIS;
                                    int islemTipi = TasinirGenel.IslemTipiGetir(servisTMM, kullanan, islemTur, dagitimIade);
                                    if (islemTipi == -1)
                                    {
                                        islemTur = (int)ENUMIslemTipi.DAGITIMIADEGIRIS;
                                        islemTipi = TasinirGenel.IslemTipiGetir(servisTMM, kullanan, islemTur, dagitimIade);
                                    }

                                    acilistaIslemTipiSec = "";
                                    ddlIslemTipi.Value = islemTipi + "*" + islemTur;
                                    islemTipiDetayiTemizlesin = false;
                                    ddlIslemTipiSelect(null, null);

                                    //ClientScript.RegisterStartupScript(this.GetType(), "", "<script language=javascript>var tmp=setTimeout('DevirListesiAc()',200); </script>");
                                    Ext1.Net.X.AddScript("DevirListesiAc();");
                                }
                            }
                        }
                    }
                    //****************************************************
                }


                if (acilistaIslemTipiSec != "")
                {
                    int islemTur = OrtakFonksiyonlar.ConvertToInt(acilistaIslemTipiSec.Split('*')[1], 0);
                    ddlIslemTipi.SetValueAndFireSelect(acilistaIslemTipiSec);
                    IslemGizle(islemTur, 0);
                }

                if (Request.QueryString["belgeNo"] + "" != "")
                {
                    txtYil.Value = Request.QueryString["yil"];
                    txtBelgeNo.Value = Request.QueryString["belgeNo"] + "";
                    txtMuhasebe.Value = Request.QueryString["muhasebe"] + "";
                    txtHarcamaBirimi.Value = Request.QueryString["harcamaBirimi"] + "";
                    X.AddScript("txtBelgeNo.fireEvent('TriggerClick');");
                }
            }

        }

        private void BAOnayDugmesiAyarla(int tur)
        {
            //B-A Onayý Kontrolü
            string baOnayi = OrtakFonksiyonlar.WebConfigOku("TasinirBAOnayi", "");
            if (baOnayi == "1")
            {
                btnBelgeOnayaGonder.Hidden = false;
                btnBelgeOnayla.Icon = Icon.FontAdd;
                btnBelgeOnayKaldir.Icon = Icon.FontDelete;

                //if (tur == 0)
                //{
                //    btnBelgeOnayla.Text = "Numara Ver";
                //    btnBelgeOnayKaldir.Text = "Numara Kaldýr";
                //    btnBelgeOnayla.DirectEvents.Click.Confirmation.Message = "Belgedeki malzemelere numara verilecektir. Bu iþlemi onaylýyor musunuz?";
                //    btnBelgeOnayKaldir.DirectEvents.Click.Confirmation.Message = "Belgedeki malzemelerin numaralarý kaldýrýlacaktýr. Bu iþlemi onaylýyor musunuz?";
                //}
                //else
                //{
                btnBelgeOnayla.Text = "Numara Ver";
                btnBelgeOnayKaldir.Text = "Ön Onay Kaldýr";
                btnBelgeOnayla.DirectEvents.Click.Confirmation.Message = "Belgeye numara verilecektir. Bu iþlemi onaylýyor musunuz?";
                btnBelgeOnayKaldir.DirectEvents.Click.Confirmation.Message = "Belgenin ön onayý kaldýrýlacaktýr. Bu iþlemi onaylýyor musunuz?";
                //}
            }
            else
            {
                btnBelgeOnayaGonder.Hidden = true;
                btnBelgeOnayKaldir.Hidden = false;
            }
            //*******************************************
        }

        /// <summary>
        /// Ýþlem tipi seçimi deðiþtiðinde çalýþan olay metodu
        /// Seçilen iþlem tipi ile ilgili sayfada ayarlamalar yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void ddlIslemTipiSelect(object sender, DirectEventArgs e)
        {
            GridTemizle();
            //strListe.RemoveAll();
            //GridKolonKilitleAc(0, fpL.Sheets[0].ColumnCount - 1, false);

            //if (islemTipiDetayiTemizlesin)
            //    GridInit(fpL);//bu açýklama, bu satýr kapalý iken geçerli idi.(Melih) Sayýmdan veya kayýt düþmeden gelen bilgi dropdown bilgisi diðiþtirildiðinde kaybloluyordu.

            int islemTuru = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[1], 0);
            int islemKod = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[0], 0);
            IslemGizle(islemTuru, islemKod);

            if (islemTuru == (int)ENUMIslemTipi.YILDEVIRCIKIS)
            {
                StokHareketBilgi shBilgi = new StokHareketBilgi();
                shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
                shBilgi.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "").Trim();
                shBilgi.muhasebeKod = txtMuhasebe.Text.Trim();
                shBilgi.ambarKod = txtAmbar.Text.Trim();

                DataTable dt = GridDataTable();

                ObjectArray bilgi = servisTMM.TuketimListele(kullanan, shBilgi);

                if (bilgi.sonuc.islemSonuc)
                {
                    if (bilgi.objeler.Count > 0)
                    {
                        foreach (StokHareketBilgi sBilgi in bilgi.objeler)
                        {
                            DataRow row = dt.NewRow();
                            row["hesapPlanKod"] = sBilgi.hesapPlanKod;
                            row["miktar"] = sBilgi.miktar;
                            row["olcuBirimAd"] = "-";
                            row["kdvOran"] = sBilgi.kdvOran;
                            row["birimFiyat"] = sBilgi.birimFiyat;
                            row["hesapPlanAd"] = sBilgi.hesapPlanAd;

                            dt.Rows.Add(row);
                        }
                    }
                }

                strListe.DataSource = dt;
                strListe.DataBind();
            }
        }

        /// <summary>
        /// Belgeyi Bul resmine basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri taþýnýr iþlem form nesnesine doldurulur, sunucuya
        /// gönderilir ve taþýnýr iþlem fiþi bilgileri sunucudan alýnýr. Hata varsa
        /// ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, DirectEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
            BaslikBilgileriniAyarla();

            string baOnayi = OrtakFonksiyonlar.WebConfigOku("TasinirBAOnayi", "");

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            int islemTuru = 0;

            tf.kod = hdnKod.Text;
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.muhasebeKod = txtMuhasebe.Text;
            tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

            if (bilgi.sonuc.islemSonuc)
            {
                TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
                tform = (TNS.TMM.TasinirIslemForm)bilgi[0];

                X.AddScript("ddlIslemTipiSec('KOD'," + tform.islemTipKod + ");");

                islemTuru = IslemTuruGetir(tform.islemTipKod);
                IslemGizle(islemTuru, tform.islemTipKod);

                string durum = "";
                if (tform.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)
                    durum = "B ONAYINA GÖNDERÝLDÝ";
                else if (tform.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                    durum = "A ONAYINA GÖNDERÝLDÝ";
                else if (tform.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                    durum = Resources.TasinirMal.FRMTIG056;
                else if (tform.durum == (int)ENUMBelgeDurumu.YENI || tform.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    durum = Resources.TasinirMal.FRMTIG055;
                else if (tform.durum == (int)ENUMBelgeDurumu.ONAYLI)
                {
                    if (baOnayi == "1")
                    {
                        durum = "NUMARA VERÝLDÝ";
                    }
                    else
                        durum = Resources.TasinirMal.FRMTIG056;
                }
                else if (tform.durum == (int)ENUMBelgeDurumu.IPTAL)
                    durum = Resources.TasinirMal.FRMTIG057;

                DurumAdDegistir(durum);

                txtYil.Value = tform.yil;
                txtBelgeTarih.Value = tform.fisTarih.Oku().Date;
                txtAmbar.Text = tform.ambarKod;

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
                    txtGonYil.Value = tform.gYil.ToString();

                lblMuhasebeAd.Text = tform.muhasebeAd;
                lblHarcamaBirimiAd.Text = tform.harcamaAd;
                lblAmbarAd.Text = tform.ambarAd;
                lblGonMuhasebeAd.Text = tform.gMuhasebeAd;
                lblGonHarcamaBirimiAd.Text = tform.gHarcamaAd;
                lblGonAmbarAd.Text = tform.gAmbarAd;
                if (txtKimeGitti.Text.Trim() != "")
                    lblKimeGittiAd.Text = GenelIslemler.KodAd(36, txtKimeGitti.Text.Trim(), true);
                else
                    lblKimeGittiAd.Text = "";

                hdnKod.Value = tform.kod;
                txtAciklama.Text = tform.aciklama;
                txtBelgeNo.Text = tf.fisNo.PadLeft(6, '0');
                //ddlMutemmim.Value = tform.mutemmim;

                DataTable dt = GridDataTable();

                foreach (TasinirIslemDetay td in tform.detay.objeler)
                {
                    DataRow row = dt.NewRow();
                    row["hesapPlanKod"] = td.hesapPlanKod;
                    row["gorSicilNo"] = islemTuru != (int)ENUMIslemTipi.ACILIS ? td.gorSicilNo : string.Empty;
                    row["miktar"] = td.miktar;
                    row["olcuBirimAd"] = td.olcuBirimAd;
                    row["kdvOran"] = td.kdvOran;
                    row["birimFiyat"] = td.birimFiyat;
                    row["hesapPlanAd"] = td.hesapPlanAd;
                    row["disSicilNo"] = td.ozellik.disSicilNo;
                    row["yerleskeYeri"] = td.yerleskeYeri;
                    row["yerleskeYeriAd"] = td.yerleskeYeriAd;
                    row["satisBedeli"] = td.satisBedeli;
                    row["eAciklama"] = td.eAciklama;

                    if (kutuphaneGoster)
                    {
                        row["ciltNo"] = td.ozellik.ciltNo;
                        row["dil"] = td.ozellik.dil;
                        row["yazarAdi"] = td.ozellik.yazarAdi;
                        row["adi"] = td.ozellik.adi;
                        row["yayinYeri"] = td.ozellik.yayinYeri;
                        row["yayinTarihi"] = td.ozellik.yayinTarihi;
                        row["gelisTarihi"] = "";
                        row["neredenGeldi"] = td.ozellik.neredenGeldi;
                        row["neredeBulundu"] = "";
                        row["cagi"] = "";
                        row["boyutlari"] = td.ozellik.boyutlari;
                        row["satirSayisi"] = td.ozellik.satirSayisi;
                        row["yaprakSayisi"] = td.ozellik.yaprakSayisi;
                        row["sayfaSayisi"] = td.ozellik.sayfaSayisi;
                        row["ciltTuru"] = td.ozellik.ciltTuru;
                        row["cesidi"] = td.ozellik.cesidi;
                        row["durumuMaddesi"] = "";
                        row["onYuz"] = "";
                        row["arkaYuz"] = "";
                        row["puan"] = "";
                        row["yeriKonusu"] = td.ozellik.yeriKonusu;
                    }
                    else if (muzeGoster)
                    {
                        row["ciltNo"] = "";
                        row["dil"] = "";
                        row["yazarAdi"] = "";
                        row["adi"] = td.ozellik.adi;
                        row["yayinYeri"] = "";
                        row["yayinTarihi"] = "";
                        row["gelisTarihi"] = td.ozellik.gelisTarihi;
                        row["neredenGeldi"] = td.ozellik.neredenGeldi;
                        row["neredeBulundu"] = td.ozellik.neredeBulundu;
                        row["cagi"] = td.ozellik.cagi;
                        row["boyutlari"] = td.ozellik.boyutlari;
                        row["satirSayisi"] = "";
                        row["yaprakSayisi"] = "";
                        row["sayfaSayisi"] = "";
                        row["ciltTuru"] = "";
                        row["cesidi"] = "";
                        row["durumuMaddesi"] = td.ozellik.durumuMaddesi;
                        row["onYuz"] = td.ozellik.onYuz;
                        row["arkaYuz"] = td.ozellik.arkaYuz;
                        row["puan"] = td.ozellik.puan;
                        row["yeriKonusu"] = td.ozellik.yeriKonusu;
                    }
                    else
                    {
                        row["eSicilNo"] = td.eSicilNo;
                        row["eAlimTarihi"] = td.eAlimTarihi.Oku();
                        row["eTedarikSekli"] = td.eTedarikSekli;

                        row["garantiBitisTarihi"] = td.ozellik.garantiBitisTarihi.Oku();
                        row["giai"] = td.ozellik.giai;
                        row["rfidEtiketTurKod"] = td.ozellik.rfidEtiketTurKod;
                        row["markaKod"] = td.ozellik.markaKod;
                        row["modelKod"] = td.ozellik.modelKod;
                    }

                    dt.Rows.Add(row);
                }

                strListe.DataSource = dt;
                strListe.DataBind();
            }
            else
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
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
        protected void btnGonListele_Click(object sender, DirectEventArgs e)
        {
            int islemTuruSecili = 0;
            islemTuruSecili = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[1], 0);

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
                GenelIslemler.MesajKutusu("Hata", hata);
                return;
            }

            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            int islemTuru = 0;

            IslemGizle(islemTuruSecili, 0);

            txtGonBelgeNo.Text = txtGonBelgeNo.Text.Trim().PadLeft(6, '0');

            if (islemTuruSecili == (int)ENUMIslemTipi.DEVIRGIRIS)
            {
                tf.kod = hdnKod.Text;
                tf.yil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
                tf.harcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                tf.muhasebeKod = txtGonMuhasebe.Text;
                tf.fisNo = txtGonBelgeNo.Text;
                tf.devirGirisiMi = true;
            }
            else if (islemTuruSecili == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                tf.kod = hdnKod.Text;
                tf.yil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
                tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
                tf.muhasebeKod = txtMuhasebe.Text;
                tf.fisNo = txtGonBelgeNo.Text;
                tf.devirGirisiMi = true;
            }
            else if (islemTuruSecili == (int)ENUMIslemTipi.YILDEVIRGIRIS)
            {
                tf.kod = hdnKod.Text;
                tf.yil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
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
                    GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMTIG070);
                    return;
                }

                islemTuru = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(kullanan, "TASISLEMTIPTUR", tf.islemTipKod.ToString(), true), 0);

                if ((islemTuruSecili == (int)ENUMIslemTipi.DEVIRGIRIS || islemTuruSecili == (int)ENUMIslemTipi.DAGITIMIADEGIRIS) &&
                    (islemTuru != (int)ENUMIslemTipi.DEVIRCIKIS && islemTuru != (int)ENUMIslemTipi.DAGITIMIADECIKIS))
                {
                    GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMTIG071);
                    return;
                }

                if (islemTuruSecili == (int)ENUMIslemTipi.YILDEVIRGIRIS &&
                    islemTuru != (int)ENUMIslemTipi.YILDEVIRCIKIS)
                {
                    GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMTIG072);
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
                        GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMTIG073);
                        return;
                    }
                }

                DataTable dt = GridDataTable();

                foreach (TasinirIslemDetay td in tf.detay.objeler)
                {
                    DataRow row = dt.NewRow();
                    row["hesapPlanKod"] = td.hesapPlanKod;
                    row["gorSicilNo"] = td.gorSicilNo;
                    row["miktar"] = td.miktar;
                    row["olcuBirimAd"] = td.olcuBirimAd;
                    row["kdvOran"] = td.kdvOran;
                    row["birimFiyat"] = td.birimFiyat;
                    row["hesapPlanAd"] = td.hesapPlanAd;
                    row["eAciklama"] = td.eAciklama;
                    row["disSicilNo"] = td.ozellik.disSicilNo;

                    if (kutuphaneGoster)
                    {
                        row["ciltNo"] = td.ozellik.ciltNo;
                        row["dil"] = td.ozellik.dil;
                        row["yazarAdi"] = td.ozellik.yazarAdi;
                        row["adi"] = td.ozellik.adi;
                        row["yayinYeri"] = td.ozellik.yayinYeri;
                        row["yayinTarihi"] = td.ozellik.yayinTarihi;
                        row["neredenGeldi"] = td.ozellik.neredenGeldi;
                        row["boyutlari"] = td.ozellik.boyutlari;
                        row["satirSayisi"] = td.ozellik.satirSayisi;
                        row["yaprakSayisi"] = td.ozellik.yaprakSayisi;
                        row["sayfaSayisi"] = td.ozellik.sayfaSayisi;
                        row["ciltTuru"] = td.ozellik.ciltTuru;
                        row["cesidi"] = td.ozellik.cesidi;
                        row["yeriKonusu"] = td.ozellik.yeriKonusu;
                    }
                    else if (muzeGoster)
                    {
                        row["adi"] = td.ozellik.adi;
                        row["gelisTarihi"] = td.ozellik.gelisTarihi;
                        row["neredenGeldi"] = td.ozellik.neredenGeldi;
                        row["neredeBulundu"] = td.ozellik.neredeBulundu;
                        row["cagi"] = td.ozellik.cagi;
                        row["boyutlari"] = td.ozellik.boyutlari;
                        row["durumuMaddesi"] = td.ozellik.durumuMaddesi;
                        row["onYuz"] = td.ozellik.onYuz;
                        row["arkaYuz"] = td.ozellik.arkaYuz;
                        row["puan"] = td.ozellik.puan;
                        row["yeriKonusu"] = td.ozellik.yeriKonusu;
                    }
                    dt.Rows.Add(row);
                }

                strListe.DataSource = dt;
                strListe.DataBind();
            }
            else
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
        }

        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            if (e.ExtraParams["islem"] == "belge")
                TasinirIslemFormYazdir.Yazdir(kullanan, OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0), txtBelgeNo.Text, txtHarcamaBirimi.Text, txtMuhasebe.Text, "", hdnBelgeTur.Text);
            else if (e.ExtraParams["islem"] == "TIFSicil")
                TasinirIslemFormYazdir.TIFSicilYazdir(kullanan, OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0), txtMuhasebe.Text, txtHarcamaBirimi.Text, txtBelgeNo.Text);
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && DateTime.Now.Month == 1)
                ddlMutemmimSelect(null, null);

            TasinirFormKriter kriter = new TasinirFormKriter();

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.kod = hdnKod.Text;
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.muhasebeKod = txtMuhasebe.Text;
            tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');

            ObjectArray bilgiler = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

            if (bilgiler.objeler.Count > 0)
            {
                foreach (TNS.TMM.TasinirIslemForm item in bilgiler.objeler)
                {
                    if (item.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB || item.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                        GenelIslemler.MesajKutusu("Hata", "Onaya gönderilen kayýt üzerinde deðiþiklik yapýlamaz!"); //UstBilgiGuncelle(item); - Hatalý veri kaydý giriþine neden olduðunan kapatýldý. HÖ. 2022.05.10
                    else
                        Kaydet(e);
                }
            }
            else
            {
                Kaydet(e);
            }
        }

        private void Kaydet(DirectEventArgs e)
        {
            string json = e.ExtraParams["json"];
            JArray jArray = (JArray)JSON.Deserialize(json);
            if (string.IsNullOrEmpty(json) || jArray.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listede kaydedilecek kayýt bulunamadý.");
                return;
            }

            BaslikBilgileriniAyarla();
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
            string hata = "";
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.fisTarih = new TNSDateTime(txtBelgeTarih.RawText);
            tf.fisNo = txtBelgeNo.Text.Trim() == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.ambarKod = txtAmbar.Text.Replace(".", "");
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[0], 0);
            tf.islemTipTur = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[1], 0);
            tf.kod = tf.fisNo == string.Empty ? "" : hdnKod.Text;
            tf.mutemmim = OrtakFonksiyonlar.ConvertToInt(hdnMutemmim.Value, 0);

            //if (tf.islemTipKod == (int)ENUMIslemTipi.SATINALMAGIRIS)
            //{
            //    if (tf.yil > 2009 && (tf.harcamaKod.StartsWith("12")))//|| tf.harcamaKod.StartsWith("17") 17.07.2012 talep üzerine kaldýrýldý
            //    {
            //        GenelIslemler.MesajKutusu("Hata", Resources.TasinirMal.FRMTIG004);
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
                    GenelIslemler.MesajKutusu("Hata", "Bu Taþýnýr fiþi " + surecNo + " nolu harcama süreci ile oluþturulmuþtur. Fiþi ancak harcama sürecinden deðiþtirebilirsiniz.");
                    return;
                }
            }

            decimal dovizDeger = DovizDegerGetir();

            //if (!fcDayanak.Hidden)
            {
                tf.dayanakNo = txtDayanakNo.Text;
                tf.dayanakTarih = new TNSDateTime(txtDayanakTarih.RawText);
            }
            //if (!fcFatura.Hidden)
            {
                tf.faturaNo = txtFaturaNo.Text;
                tf.faturaTarih = new TNSDateTime(txtFaturaTarih.RawText);
            }
            //if (!fcMuayene.Hidden)
            {
                tf.muayeneNo = txtMuayeneNo.Text;
                tf.muayeneTarih = new TNSDateTime(txtMuayeneTarih.RawText);
            }
            //if (!txtNereyeGitti.Hidden)
            tf.nereyeGitti = txtNereyeGitti.Text;
            //if (!txtKimeGitti.Hidden)
            tf.kimeGitti = txtKimeGitti.Text;
            //if (!txtNeredenGeldi.Hidden)
            tf.neredenGeldi = txtNeredenGeldi.Text;

            if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
            {
                //Ekranda gösterilmedi için arka planda set edildi.
                txtGonMuhasebe.Text = tf.muhasebeKod;
                txtGonHarcamaBirimi.Text = tf.harcamaKod;
            }

            //if (!txtGonMuhasebe.Hidden)
            {
                tf.gMuhasebeKod = txtGonMuhasebe.Text;
                tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                tf.gAmbarKod = txtGonAmbar.Text;

                if (!txtGonAmbar.Hidden)
                    tf.gAmbarKod = txtGonAmbar.Text;
            }
            //if (!fcGonYilBelgeNo.Hidden)
            {
                tf.gYil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
                tf.gFisNo = txtGonBelgeNo.Text.Trim().PadLeft(6, '0');
            }

            tf.aciklama = txtAciklama.Text;

            string sicilAyni = "";
            string eskiAnaHesapKod = "";

            if (tf.fisTarih.Oku().Year != tf.yil)
                hata += string.Format(Resources.TasinirMal.FRMTIG005, tf.yil, tf.fisTarih.ToString()) + "<br>";

            //string[] islem = (ddlIslemTipi.Value + "").Split('*');
            //int islemTip = OrtakFonksiyonlar.ConvertToInt(islem[0], 0);
            //int islemTur = islem.Length > 1 ? OrtakFonksiyonlar.ConvertToInt(islem[1], 0) : 0;

            int index = 0;
            foreach (JContainer jc in jArray)
            {
                index++;
                string hesapKod = (jc.Value<string>("hesapPlanKod") + "").Split('-')[0].Replace(".", "").Trim();
                if (hesapKod == "")
                    continue;

                if (hesapKod != "")
                {
                    if (tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRCIKIS || tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS)
                    {
                        if (hesapKod.StartsWith("25"))
                        {
                            hata += Resources.TasinirMal.FRMTIG006 + "<br>";
                            break;
                        }
                    }
                    else if (tf.islemTipTur == (int)ENUMIslemTipi.ACILIS)
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
                        if (hesapKod != "" && !hesapKod.StartsWith(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString()))
                            hata += string.Format(Resources.TasinirMal.FRMTIG007, (index + 1).ToString()) + "<br>";
                    }
                    else if (muzeGoster)
                    {
                        if (hesapKod != "" && !hesapKod.StartsWith(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString()))
                            hata += string.Format(Resources.TasinirMal.FRMTIG008, (index + 1).ToString()) + "<br>";
                    }

                    string sicilNo = (jc.Value<string>("gorSicilNo") + "").Trim();
                    if (sicilNo != "")
                    {
                        int index2 = 0;
                        foreach (JContainer jc2 in jArray)
                        {
                            index2++;
                            if (index2 <= index)
                                continue;

                            if (sicilNo == (jc2.Value<string>("gorSicilNo") + "").Trim())
                                sicilAyni += string.Format(Resources.TasinirMal.FRMTIG009, (index + 1).ToString(), (index2 + 1).ToString()) + "<br>";
                        }
                    }
                }
            }

            if (sicilAyni != "")
            {
                GenelIslemler.MesajKutusu("Hata", sicilAyni);
                return;
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata);
                return;
            }


            TasinirIslemDetay[] tdListe = GridOku(jArray);
            if (tdListe != null && tdListe.Length > 0)
            {
                foreach (var td in tdListe)
                {
                    td.yil = tf.yil;
                    td.muhasebeKod = tf.muhasebeKod;
                    td.harcamaKod = tf.harcamaKod;
                    td.ambarKod = tf.ambarKod;

                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    {
                        if (tf.islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS)
                            td.birimFiyat = (decimal)0.01;
                        else
                            td.birimFiyat = OrtakFonksiyonlar.Yuvarla(td.birimFiyat, 2);

                        if (td.birimFiyat <= 0)
                            hata += "Fiyat alaný sýfýrdan büyük olmaldýr.<br>";

                    }

                    tf.detay.Ekle(td);
                }

                if (hata != "")
                {
                    GenelIslemler.MesajKutusu("Hata", hata);
                    return;
                }
            }
            else
            {
                GenelIslemler.MesajKutusu("Hata", "Listede kayýt bulunamadý.");
                return;
            }

            if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                int maxSatir = 1000;
                if (tf.detay.objeler.Count > maxSatir)
                {
                    GenelIslemler.MesajKutusu("Hata", string.Format(Resources.TasinirMal.FRMTIG010, maxSatir.ToString()));
                    return;
                }
            }

            tf.islemTarih = new TNSDateTime(DateTime.Now);
            tf.islemYapan = kullanan.kullaniciKodu;

            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, tf);

            if (sonuc.islemSonuc)
            {
                DurumAdDegistir(Resources.TasinirMal.FRMTIG011);

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                txtBelgeNo.Text = sonuc.anahtar.Split('-')[0];
                hdnKod.Text = sonuc.anahtar.Split('-')[1];
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        private void UstBilgiGuncelle(TNS.TMM.TasinirIslemForm bilgi)
        {
            Sonuc sonuc = new Sonuc();


            bilgi.dayanakNo = txtDayanakNo.Text;
            bilgi.dayanakTarih = new TNSDateTime(txtDayanakTarih.RawText);
            bilgi.faturaNo = txtFaturaNo.Text;
            bilgi.faturaTarih = new TNSDateTime(txtFaturaTarih.RawText);
            bilgi.muayeneNo = txtMuayeneNo.Text;
            bilgi.muayeneTarih = new TNSDateTime(txtMuayeneTarih.RawText);
            bilgi.nereyeGitti = txtNereyeGitti.Text;
            bilgi.kimeGitti = txtKimeGitti.Text;
            bilgi.neredenGeldi = txtNeredenGeldi.Text;
            if (bilgi.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS || bilgi.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
            {
                //Ekranda gösterilmedi için arka planda set edildi.
                txtGonMuhasebe.Text = bilgi.muhasebeKod;
                txtGonHarcamaBirimi.Text = bilgi.harcamaKod;
            }
            bilgi.gMuhasebeKod = txtGonMuhasebe.Text;
            bilgi.gHarcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
            bilgi.gAmbarKod = txtGonAmbar.Text;
            if (!txtGonAmbar.Hidden)
                bilgi.gAmbarKod = txtGonAmbar.Text;
            bilgi.gYil = OrtakFonksiyonlar.ConvertToInt(txtGonYil.Text, 0);
            bilgi.gFisNo = txtGonBelgeNo.Text.Trim().PadLeft(6, '0');
            bilgi.aciklama = txtAciklama.Text;

            sonuc = servisTMM.TasinirIslemFisiUstBilgiGuncelle(kullanan, bilgi);

            if (sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
                txtBelgeNo.Text = sonuc.anahtar.Split('-')[0];
                hdnKod.Text = sonuc.anahtar.Split('-')[1];
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            GridTemizle();

            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");

            EkAlanlariTemizle();

            //ddlIslemTipi.SelectedIndex = 0;
            IslemGizle(OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[1], 0), 0);
        }

        void EkAlanlariTemizle()
        {
            txtYil.Value = DateTime.Now.Year;
            txtBelgeTarih.Value = DateTime.Now.Date;
            txtBelgeNo.Text = string.Empty;
            lblFormDurum.Text = "";

            txtMuayeneNo.Text = string.Empty;
            txtMuayeneTarih.Text = string.Empty;
            txtDayanakNo.Text = string.Empty;
            txtDayanakTarih.Text = string.Empty;
            txtFaturaNo.Text = string.Empty;
            txtFaturaTarih.Text = string.Empty;
            txtNeredenGeldi.Text = string.Empty;
            txtNereyeGitti.Text = string.Empty;
            txtKimeGitti.Text = string.Empty;
            lblKimeGittiAd.Text = string.Empty;

            txtGonMuhasebe.Text = string.Empty;
            txtGonHarcamaBirimi.Text = string.Empty;
            txtGonAmbar.Text = string.Empty;
            txtGonBelgeNo.Text = string.Empty;
            lblGonMuhasebeAd.Text = string.Empty;
            lblGonHarcamaBirimiAd.Text = string.Empty;
            lblGonAmbarAd.Text = string.Empty;

            hdnKod.Text = string.Empty;
            txtAciklama.Text = string.Empty;
        }

        /// <summary>
        /// Onayla tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onaylanmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayla_Click(Object sender, DirectEventArgs e)
        {
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                int islemTipTur = OrtakFonksiyonlar.ConvertToInt((ddlIslemTipi.Value + "").Split('*')[1], 0);
                if (!(islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS || islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS))
                {
                    GenelIslemler.MesajKutusu("Hata", "\"Numara Verme\" iþlemi \"Satýn Alma Giriþ\" veya \"Baðýþ Giriþ\" için geçerlidir!");
                    return;
                }
            }

            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG014 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + Resources.TasinirMal.FRMTIG015);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.kod = hdnKod.Text;
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                tf.numaraVer = true;

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "Onay");

            if (sonuc.islemSonuc)
            {
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    DurumAdDegistir("NUMARA VERÝLDÝ");
                else
                    DurumAdDegistir(Resources.TasinirMal.FRMTIG016);

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        /// <summary>
        /// Onay Kaldýr tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþi bilgileri ekrandaki ilgili kontrollerden toplanýp onayý kaldýrýlmak
        /// üzere sunucuya gönderilir, gelen sonuca göre hata mesajý veya bilgi mesajý verilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOnayKaldir_Click(object sender, DirectEventArgs e)
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
                GenelIslemler.MesajKutusu("Hata", hata + Resources.TasinirMal.FRMTIG083);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.kod = hdnKod.Text;
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "OnayKaldir");

            if (sonuc.islemSonuc)
            {
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    DurumAdDegistir("ÖN ONAY KALDIRILDI");
                else
                    DurumAdDegistir(Resources.TasinirMal.FRMTIG084);

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        protected void btnDosyaYukle_Change(object sender, DirectEventArgs e)
        {
            if (btnDosyaYukle.PostedFile.InputStream.Length == 0)
                return;

            List<object> liste = new List<object>();


            string dosyaAdi = btnDosyaYukle.PostedFile.FileName;
            int count = OrtakFonksiyonlar.ConvertToInt(btnDosyaYukle.PostedFile.InputStream.Length, 0);
            byte[] buffer = new byte[count];
            btnDosyaYukle.PostedFile.InputStream.Read(buffer, 0, count);

            string dosyaAd = System.IO.Path.GetTempFileName();
            btnDosyaYukle.PostedFile.SaveAs(dosyaAd);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(dosyaAd);

            int satir = 0;
            int bosSatir = 0;

            while (true)
            {
                satir++;
                bosSatir++;
                if (bosSatir > 500)
                    break;

                if (XLS.HucreDegerAl(satir, 0) == "")
                {
                    continue;
                }

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    liste.Add(new
                    {
                        siraNo = satir,
                        hesapPlanKod = XLS.HucreDegerAl(satir, 0),
                        gorSicilNo = XLS.HucreDegerAl(satir, 1),
                        miktar = OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(satir, 2)),
                        olcuBirimAd = XLS.HucreDegerAl(satir, 3),
                        kdvOran = XLS.HucreDegerAl(satir, 4),
                        birimFiyat = OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(satir, 5)),
                        hesapPlanAd = XLS.HucreDegerAl(satir, 6),

                        eAciklama = XLS.HucreDegerAl(satir, 7),
                        giai = XLS.HucreDegerAl(satir, 8),
                        yerleskeYeri = XLS.HucreDegerAl(satir, 9),
                        yerleskeYeriAd = XLS.HucreDegerAl(satir, 10),

                        //disSicilNo = XLS.HucreDegerAl(satir, 7),
                        //adi = XLS.HucreDegerAl(satir, 8),
                        //yeriKonusu = XLS.HucreDegerAl(satir, 9),
                        //eAciklama = XLS.HucreDegerAl(satir, 10),
                    });
                }
                else
                {
                    liste.Add(new
                    {
                        siraNo = satir,
                        hesapPlanKod = XLS.HucreDegerAl(satir, 0),
                        gorSicilNo = XLS.HucreDegerAl(satir, 1),
                        miktar = OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(satir, 2)),
                        olcuBirimAd = XLS.HucreDegerAl(satir, 3),
                        kdvOran = XLS.HucreDegerAl(satir, 4),
                        birimFiyat = OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(satir, 5)),
                        hesapPlanAd = XLS.HucreDegerAl(satir, 6),
                        disSicilNo = XLS.HucreDegerAl(satir, 7),
                        adi = XLS.HucreDegerAl(satir, 8),
                        yeriKonusu = XLS.HucreDegerAl(satir, 9),
                        eAciklama = XLS.HucreDegerAl(satir, 10),
                    });
                }

                bosSatir = 0;
            }

            strListe.DataSource = liste;
            strListe.DataBind();


            //System.Web.HttpPostedFile myFile = fileListe.PostedFile;
            //int nFileLen = myFile.ContentLength;
            //if (nFileLen > 0)
            //{
            //    byte[] myData = new byte[nFileLen];

            //    myFile.InputStream.Read(myData, 0, nFileLen);

            //    string dosyaAd = System.IO.Path.GetTempFileName();

            //    System.IO.FileStream newFile = new System.IO.FileStream(dosyaAd, System.IO.FileMode.Create);
            //    newFile.Write(myData, 0, myData.Length);
            //    newFile.Close();

            //    if (dosyaAd != "")
            //    {
            //        try
            //        {
            //            fpL.OpenExcel(dosyaAd, Excel.ExcelOpenFlags.DataOnly);

            //            dosyaYukle = true;
            //            System.IO.File.Delete(dosyaAd);
            //        }
            //        catch
            //        {
            //            GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTIG074, dosyaAd));
            //        }
            //        GridInit(fpL);
            //        dosyaYukle = false;

            //        for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            //        {
            //            if (fpL.Sheets[0].Cells[i, 0].Text.Trim() == "")
            //            {
            //                fpL.Sheets[0].RowCount = i;
            //                break;
            //            }
            //        }

            //        int maxSatir = 1000;
            //        if (fpL.Sheets[0].RowCount > maxSatir)
            //        {
            //            GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTIG075, maxSatir.ToString(), fpL.Sheets[0].RowCount.ToString()));
            //            fpL.CancelEdit();
            //            fpL.Sheets[0].RowCount = 0;
            //            fpL.Sheets[0].RowCount = ekleSatirSayisi;
            //            return;
            //        }

            //        int yuklenenSatirSayisi = 0;

            //        yuklenenSatirSayisi = fpL.Sheets[0].RowCount;

            //        for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            //        {
            //            HesapPlaniSatir h = new HesapPlaniSatir();

            //            if (!kutuphaneGoster && !muzeGoster) //Kütüphane ve Müze'de alým tarihi yok
            //            {
            //                try
            //                {
            //                    h.hesapKod = fpL.Sheets[0].DataModel.GetValue(i, 0).ToString();
            //                    if (fpL.Sheets[0].Cells[i, 11].Text.Trim() != "")
            //                    {
            //                        TNSDateTime alimTarih = new TNSDateTime(fpL.Sheets[0].Cells[i, 11].Text);
            //                        fpL.Sheets[0].Cells[i, 11].Value = alimTarih.ToString();
            //                    }
            //                }
            //                catch { }
            //            }

            //            if (h.hesapKod == null || h.hesapKod == string.Empty)
            //                continue;

            //            ObjectArray o = servis.HesapPlaniListele(new TNS.KYM.Kullanici(), h);
            //            foreach (HesapPlaniSatir hs in o.objeler)
            //            {
            //                fpL.Sheets[0].DataModel.SetValue(i, 5, hs.olcuBirimAd);
            //                fpL.Sheets[0].DataModel.SetValue(i, 8, hs.aciklama);
            //            }
            //        }

            //        fpL.Sheets[0].RowCount = fpL.Sheets[0].RowCount + ekleSatirSayisi;
            //        IslemGizle(OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.SelectedValue.Split('*')[1], 0));
            //        GenelIslemler.MesajKutusu("Bilgi", string.Format(Resources.TasinirMal.FRMTIG076, yuklenenSatirSayisi.ToString()));
            //    }
            //    else
            //        GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG077);
            //}
            //else
            //    GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMTIG078);

        }

        /// <summary>
        /// Sakla tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandaki taþýnýr iþlem fiþi detay satýr bilgileri excel dosyasýna yazýlýr ve kullanýcýya gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDosyaSakla_Click(object sender, DirectEventArgs e)
        {
            string json = e.ExtraParams["json"];
            JArray jArray = (JArray)JSON.Deserialize(json);
            if (string.IsNullOrEmpty(json) || jArray.Count == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Listede kaydedilecek kayýt bulunamadý.");
                return;
            }

            if (Request.QueryString["kutuphane"] + "" != "")
                kutuphaneGoster = true;
            else
                kutuphaneGoster = false;

            if (Request.QueryString["muze"] + "" != "")
                muzeGoster = true;
            else
                muzeGoster = false;



            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.BosDosyaAc(sonucDosyaAd);

            XLS.HucreDegerYaz(satir, 0, Resources.TasinirMal.FRMTIG017);
            XLS.HucreDegerYaz(satir, 1, Resources.TasinirMal.FRMTIG018);
            XLS.HucreDegerYaz(satir, 2, Resources.TasinirMal.FRMTIG019);
            XLS.HucreDegerYaz(satir, 3, Resources.TasinirMal.FRMTIG020);
            XLS.HucreDegerYaz(satir, 4, "KDV Oraný (%)");
            XLS.HucreDegerYaz(satir, 5, "Birim Fiyat (KDV Hariç)");
            XLS.HucreDegerYaz(satir, 6, Resources.TasinirMal.FRMTIG023);

            if (kutuphaneGoster)
            {
                XLS.HucreDegerYaz(satir, 7, Resources.TasinirMal.FRMTIG024);
                XLS.HucreDegerYaz(satir, 8, Resources.TasinirMal.FRMTIG025);
                XLS.HucreDegerYaz(satir, 9, Resources.TasinirMal.FRMTIG026);
                XLS.HucreDegerYaz(satir, 10, Resources.TasinirMal.FRMTIG027);
                XLS.HucreDegerYaz(satir, 11, Resources.TasinirMal.FRMTIG028);
                XLS.HucreDegerYaz(satir, 12, Resources.TasinirMal.FRMTIG029);
                XLS.HucreDegerYaz(satir, 13, Resources.TasinirMal.FRMTIG030);
                XLS.HucreDegerYaz(satir, 14, Resources.TasinirMal.FRMTIG031);
                XLS.HucreDegerYaz(satir, 15, Resources.TasinirMal.FRMTIG032);
                XLS.HucreDegerYaz(satir, 16, Resources.TasinirMal.FRMTIG033);
                XLS.HucreDegerYaz(satir, 17, Resources.TasinirMal.FRMTIG034);
                XLS.HucreDegerYaz(satir, 18, Resources.TasinirMal.FRMTIG035);
                XLS.HucreDegerYaz(satir, 19, Resources.TasinirMal.FRMTIG036);
                XLS.HucreDegerYaz(satir, 20, Resources.TasinirMal.FRMTIG037);
                XLS.HucreDegerYaz(satir, 21, Resources.TasinirMal.FRMTIG038);
            }
            else if (muzeGoster)
            {
                XLS.HucreDegerYaz(satir, 7, Resources.TasinirMal.FRMTIG024);
                XLS.HucreDegerYaz(satir, 8, Resources.TasinirMal.FRMTIG028);
                XLS.HucreDegerYaz(satir, 9, Resources.TasinirMal.FRMTIG038);
                XLS.HucreDegerYaz(satir, 10, Resources.TasinirMal.FRMTIG051);
                XLS.HucreDegerYaz(satir, 11, Resources.TasinirMal.FRMTIG031);
                XLS.HucreDegerYaz(satir, 12, Resources.TasinirMal.FRMTIG043);
                XLS.HucreDegerYaz(satir, 13, Resources.TasinirMal.FRMTIG044);
                XLS.HucreDegerYaz(satir, 14, Resources.TasinirMal.FRMTIG032);
                XLS.HucreDegerYaz(satir, 15, Resources.TasinirMal.FRMTIG046);
                XLS.HucreDegerYaz(satir, 16, Resources.TasinirMal.FRMTIG047);
                XLS.HucreDegerYaz(satir, 17, Resources.TasinirMal.FRMTIG048);
                XLS.HucreDegerYaz(satir, 18, Resources.TasinirMal.FRMTIG049);
                XLS.HucreDegerYaz(satir, 19, Resources.TasinirMal.FRMTIG041);
            }
            else
            {
                XLS.HucreDegerYaz(satir, 7, Resources.TasinirMal.FRMTIG051);
                XLS.HucreDegerYaz(satir, 8, "K.Demirbaþ No (GIAI)");
                XLS.HucreDegerYaz(satir, 9, "Yerleþke Yeri Kod");
                XLS.HucreDegerYaz(satir, 10, "Yerleþke Yeri Ad");
            }


            TasinirIslemDetay[] tdListe = GridOku(jArray);
            if (tdListe != null && tdListe.Length > 0)
            {
                foreach (var td in tdListe)
                {
                    satir++;
                    XLS.HucreDegerYaz(satir, 0, td.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, 1, td.gorSicilNo);
                    XLS.HucreDegerYaz(satir, 2, td.miktar);
                    XLS.HucreDegerYaz(satir, 3, td.olcuBirimAd);
                    XLS.HucreDegerYaz(satir, 4, td.kdvOran);
                    XLS.HucreDegerYaz(satir, 5, td.birimFiyat);
                    XLS.HucreDegerYaz(satir, 6, td.hesapPlanAd);
                    //XLS.HucreDegerYaz(satir, 7, td.eAciklama);

                    if (kutuphaneGoster)
                    {
                        XLS.HucreDegerYaz(satir, 7, td.ozellik.disSicilNo);
                        XLS.HucreDegerYaz(satir, 8, td.ozellik.ciltNo);
                        XLS.HucreDegerYaz(satir, 9, td.ozellik.dil);
                        XLS.HucreDegerYaz(satir, 10, td.ozellik.yazarAdi);
                        XLS.HucreDegerYaz(satir, 11, td.ozellik.adi);
                        XLS.HucreDegerYaz(satir, 12, td.ozellik.yayinYeri);
                        XLS.HucreDegerYaz(satir, 13, td.ozellik.yayinTarihi);
                        XLS.HucreDegerYaz(satir, 14, td.ozellik.neredenGeldi);
                        XLS.HucreDegerYaz(satir, 15, td.ozellik.boyutlari);
                        XLS.HucreDegerYaz(satir, 16, td.ozellik.satirSayisi);
                        XLS.HucreDegerYaz(satir, 17, td.ozellik.yaprakSayisi);
                        XLS.HucreDegerYaz(satir, 18, td.ozellik.sayfaSayisi);
                        XLS.HucreDegerYaz(satir, 19, td.ozellik.ciltTuru);
                        XLS.HucreDegerYaz(satir, 20, td.ozellik.cesidi);
                        XLS.HucreDegerYaz(satir, 21, td.ozellik.yeriKonusu);
                    }
                    else if (muzeGoster)
                    {
                        XLS.HucreDegerYaz(satir, 7, td.ozellik.disSicilNo);
                        XLS.HucreDegerYaz(satir, 8, td.ozellik.adi);
                        XLS.HucreDegerYaz(satir, 9, td.ozellik.yeriKonusu);
                        XLS.HucreDegerYaz(satir, 10, td.eAciklama);
                        XLS.HucreDegerYaz(satir, 11, td.ozellik.neredenGeldi);
                        XLS.HucreDegerYaz(satir, 12, td.ozellik.neredeBulundu);
                        XLS.HucreDegerYaz(satir, 13, td.ozellik.cagi);
                        XLS.HucreDegerYaz(satir, 14, td.ozellik.boyutlari);
                        XLS.HucreDegerYaz(satir, 15, td.ozellik.durumuMaddesi);
                        XLS.HucreDegerYaz(satir, 16, td.ozellik.onYuz);
                        XLS.HucreDegerYaz(satir, 17, td.ozellik.arkaYuz);
                        XLS.HucreDegerYaz(satir, 18, td.ozellik.puan);
                        XLS.HucreDegerYaz(satir, 19, td.ozellik.gelisTarihi);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, 7, td.eAciklama);
                        XLS.HucreDegerYaz(satir, 8, td.ozellik.giai);
                        XLS.HucreDegerYaz(satir, 9, td.yerleskeYeri);
                        XLS.HucreDegerYaz(satir, 10, td.yerleskeYeriAd);
                    }
                }

                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), Path.GetFileName(sonucDosyaAd), true, GenelIslemler.ExcelTur());
            }
            else
                GenelIslemler.MesajKutusu("Hata", "Listede kayýt bulunamadý.");
        }

        /// <summary>
        /// Sayfadaki ilgili kontroller verilen iþlem türüne göre gizlenir, gösterilir.
        /// </summary>
        /// <param name="islemTuru">ENUMIslemTipi listesindeki deðerlerden biri olmalýdýr.</param>
        private void IslemGizle(int islemTuru, int islemKod)
        {
            BaslikBilgileriniAyarla();

            if (kutuphaneGoster)
            {
                grdListe.ColumnModel.SetHidden(17, true); //gelisTarihi
                grdListe.ColumnModel.SetHidden(19, true); //neredeBulundu
                grdListe.ColumnModel.SetHidden(20, true); //cagi
                grdListe.ColumnModel.SetHidden(27, true); //durumuMaddesi
                grdListe.ColumnModel.SetHidden(28, true); //onYuz
                grdListe.ColumnModel.SetHidden(30, true); //puan
            }
            else if (muzeGoster)
            {
                grdListe.ColumnModel.SetHidden(11, true); //ciltNo
                grdListe.ColumnModel.SetHidden(12, true); //dil
                grdListe.ColumnModel.SetHidden(13, true); //yazarAdi
                grdListe.ColumnModel.SetHidden(15, true); //yayinYeri
                grdListe.ColumnModel.SetHidden(16, true); //yayinTarihi
                grdListe.ColumnModel.SetHidden(22, true); //satirSayisi
                grdListe.ColumnModel.SetHidden(23, true); //yaprakSayisi
                grdListe.ColumnModel.SetHidden(24, true); //sayfaSayisi
                grdListe.ColumnModel.SetHidden(25, true); //ciltTuru
                grdListe.ColumnModel.SetHidden(26, true); //cesidi
            }
            else
            {
                grdListe.ColumnModel.SetHidden(34, true); //eSicilNo
                grdListe.ColumnModel.SetHidden(35, true); //eAlimTarihi
                grdListe.ColumnModel.SetHidden(36, true); //eTedarikSekli
            }

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                grdListe.ColumnModel.SetColumnHeader(5, "Adet"); //miktar
                grdListe.ColumnModel.SetHidden(6, true); //olcuBirimAd

                grdListe.ColumnModel.SetHidden(37, false); //garantiBitisTarihi
                grdListe.ColumnModel.SetHidden(38, false); //giai
                grdListe.ColumnModel.SetHidden(39, false); //rfidEtiketTurKod
                grdListe.ColumnModel.SetHidden(40, false); //markaKod
                grdListe.ColumnModel.SetHidden(41, false); //modelKod
            }

            //grdListe.ColumnModel.SetHidden(0, false); //RowNumbererColumn
            //grdListe.ColumnModel.SetHidden(1, false); //hesapPlanKod
            //grdListe.ColumnModel.SetHidden(2, false); //hesapPlanKodCommand
            //grdListe.ColumnModel.SetHidden(3, false); //gorSicilNo
            //grdListe.ColumnModel.SetHidden(4, false); //gorSicilNoCommand
            //grdListe.ColumnModel.SetHidden(5, false); //miktar
            //grdListe.ColumnModel.SetHidden(6, false); //olcuBirimAd
            //grdListe.ColumnModel.SetHidden(7, false); //kdvOran
            //grdListe.ColumnModel.SetHidden(8, false); //birimFiyat
            //grdListe.ColumnModel.SetHidden(9, false); //hesapPlanAd
            //grdListe.ColumnModel.SetHidden(10, false); //disSicilNo
            //grdListe.ColumnModel.SetHidden(11, ciltNo); //ciltNo
            //grdListe.ColumnModel.SetHidden(12, dil); //dil
            //grdListe.ColumnModel.SetHidden(13, yazarAdi); //yazarAdi
            //grdListe.ColumnModel.SetHidden(14, false); //adi
            //grdListe.ColumnModel.SetHidden(15, yayinYeri); //yayinYeri
            //grdListe.ColumnModel.SetHidden(16, yayinTarihi); //yayinTarihi
            //grdListe.ColumnModel.SetHidden(17, gelisTarihi); //gelisTarihi
            //grdListe.ColumnModel.SetHidden(18, false); //neredenGeldi
            //grdListe.ColumnModel.SetHidden(19, neredeBulundu); //neredeBulundu
            //grdListe.ColumnModel.SetHidden(20, cagi); //cagi
            //grdListe.ColumnModel.SetHidden(21, false); //boyutlari
            //grdListe.ColumnModel.SetHidden(22, satirSayisi); //satirSayisi
            //grdListe.ColumnModel.SetHidden(23, yaprakSayisi); //yaprakSayisi
            //grdListe.ColumnModel.SetHidden(24, sayfaSayisi); //sayfaSayisi
            //grdListe.ColumnModel.SetHidden(25, ciltTuru); //ciltTuru
            //grdListe.ColumnModel.SetHidden(26, cesidi); //cesidi
            //grdListe.ColumnModel.SetHidden(27, durumuMaddesi); //durumuMaddesi
            //grdListe.ColumnModel.SetHidden(28, onYuz); //onYuz
            //grdListe.ColumnModel.SetHidden(29, false); //arkaYuz
            //grdListe.ColumnModel.SetHidden(30, puan); //puan
            //grdListe.ColumnModel.SetHidden(31, false); //yeriKonusu
            grdListe.ColumnModel.SetHidden(32, true); //yerleskeYeriAd
                                                      //grdListe.ColumnModel.SetHidden(33, false); //eAciklama
                                                      //grdListe.ColumnModel.SetHidden(34, eSicilNo); //eSicilNo
                                                      //grdListe.ColumnModel.SetHidden(35, eAlimTarihi); //eAlimTarihi
                                                      //grdListe.ColumnModel.SetHidden(36, eTedarikSekli); //eTedarikSekli
                                                      //grdListe.ColumnModel.SetHidden(37, false); //garantiBitisTarihi
                                                      //grdListe.ColumnModel.SetHidden(38, false); //giai
                                                      //grdListe.ColumnModel.SetHidden(39, false); //rfidEtiketTurKod
                                                      //grdListe.ColumnModel.SetHidden(40, false); //markaKod
                                                      //grdListe.ColumnModel.SetHidden(41, false); //modelKod
                                                      //grdListe.ColumnModel.SetHidden(41, false); //yerleskeYeri
                                                      //grdListe.ColumnModel.SetHidden(42, false); //satisBedeli
            grdListe.ColumnModel.SetHidden(42, true); //satisBedeli

            List<int> kilitListe = new List<int>();
            kilitListe.AddRange(new int[] { 6, 9 });

            //GridKolonKilitleAc(0, 7, false);
            //GridKolonKilitleAc(5, 5, true);
            //GridKolonKilitleAc(8, 8, true);

            //grdListe.ColumnModel.SetEditable(6, false); //olcuBirimAd
            //grdListe.ColumnModel.SetEditable(9, false); //hesapPlanAd

            btnOzellik.Hidden = true;
            fsGonderenBirim.Hidden = true;

            if (!(islemTuru == (int)ENUMIslemTipi.YILDEVIRGIRIS || islemTuru == (int)ENUMIslemTipi.TUKETIMCIKIS ||
                islemTuru == (int)ENUMIslemTipi.KULLANILMAZCIKIS || islemTuru == (int)ENUMIslemTipi.YILDEVIRCIKIS))
                btnSicilYazdir.Hidden = false;
            else
                btnSicilYazdir.Hidden = true;

            if (islemTuru < 50)
                BAOnayDugmesiAyarla(0);
            else
                BAOnayDugmesiAyarla(1);

            if (islemTuru == (int)ENUMIslemTipi.SATINALMAGIRIS)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, true, true, true, true, false, false, false, false);
                //GridKolonKilitleAc(2, 3, true);
                kilitListe.AddRange(new int[] { 3, 4 });
                //grdListe.ColumnModel.SetEditable(3, false); //gorSicilNo
                //grdListe.ColumnModel.SetEditable(4, false); //gorSicilNoCommand
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    grdListe.ColumnModel.SetHidden(32, false); //yerleskeYeriAd
            }
            else if (islemTuru == (int)ENUMIslemTipi.BAGISGIRIS)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, true, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4 });
                //GridKolonKilitleAc(2, 3, true);
                //grdListe.ColumnModel.SetEditable(3, false); //gorSicilNo
                //grdListe.ColumnModel.SetEditable(4, false); //gorSicilNoCommand
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    grdListe.ColumnModel.SetHidden(32, false); //yerleskeYeriAd
            }
            else if (islemTuru == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4 });
                //GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.IADEGIRIS)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, true, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4 });
                //GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRGIRIS || islemTuru == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                fsGonderenBirim.Hidden = false;
                fsGonderenBirim.Tools[0].Hidden = false;
                //imgDevir.Hidden = false;
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, true, true, false);
                kilitListe.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 33, 37, 38, 39, 40, 41 });

                //GridKolonKilitleAc(0, fpL.Sheets[0].ColumnCount - 1, true);
                //fpL.Sheets[0].Columns[2].ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            }
            else if (islemTuru == (int)ENUMIslemTipi.URETILENGIRIS)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4 });
                //GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.TUKETIMCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, true);
                kilitListe.AddRange(new int[] { 3, 4, 5, 6, 7, 8 });
                //GridKolonKilitleAc(2, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRCIKIS || islemTuru == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
            {
                //imgDevir.Hidden = false;
                fsGonderenBirim.Hidden = false;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, true, false, false);
                kilitListe.AddRange(new int[] { 5, 6, 7, 8 });
                //GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.BAGISCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                kilitListe.AddRange(new int[] { 5, 6, 7, 8 });
                //GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.SATISCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, true, false, true, false, false, false);
                kilitListe.AddRange(new int[] { 5, 6, 7, 8 });

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    grdListe.ColumnModel.SetHidden(42, false); //satýþ fiyatý

                //GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.KULLANILMAZCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4, 5, 6, 7, 8 });
                //GridKolonKilitleAc(2, 7, true);

            }
            else if (islemTuru == (int)ENUMIslemTipi.HURDACIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                kilitListe.AddRange(new int[] { 1, 2, 5, 6, 7, 8, 37, 38, 39, 40, 41 });
                //GridKolonKilitleAc(0, 1, true);
                //GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.ACILIS)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                //if (!kutuphaneGoster && !muzeGoster)
                //    fpL.Sheets[0].Columns[ozellikKolonBasla, fpL.Sheets[0].Columns.Count - 1].Visible = true;

                grdListe.ColumnModel.SetHidden(34, false); //eSicilNo
                grdListe.ColumnModel.SetHidden(35, false); //eAlimTarihi
                grdListe.ColumnModel.SetHidden(36, false); //eTedarikSekli

                kilitListe.AddRange(new int[] { 3, 4 });
                //GridKolonKilitleAc(2, 3, true);
                //GridKolonKilitleAc(6, 7, false);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEGERARTTIR)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 5, 6, 7, 8 });
                //GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.ENFLASYONARTISI)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 5, 6, 7, 8 });
                //GridKolonKilitleAc(4, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.YILDEVIRGIRIS)
            {
                FormAlanGosterGizle(islemTuru, false, false, false, false, false, false, true, false);
                //GridKolonKilitleAc(0, fpL.Sheets[0].ColumnCount - 1, true);
                //fpL.Sheets[0].Columns[2].ForeColor = System.Drawing.Color.LightGoldenrodYellow;

                kilitListe.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 37, 38, 39, 40, 41 });
            }
            else if (islemTuru == (int)ENUMIslemTipi.YILDEVIRCIKIS)
            {
                FormAlanGosterGizle(islemTuru, false, false, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4, 5, 6, 7, 8 });
                //GridKolonKilitleAc(2, 7, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRGIRISKURUM)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, true, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4 });
                //GridKolonKilitleAc(2, 3, true);
            }
            else if (islemTuru == (int)ENUMIslemTipi.DEVIRCIKISKURUM)
            {
                FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, false);
                kilitListe.AddRange(new int[] { 5, 6, 7, 8 });
            }
            else if (islemTuru == (int)ENUMIslemTipi.SAYIMNOKSANICIKIS)
            {
                btnOzellik.Hidden = false;
                FormAlanGosterGizle(islemTuru, false, true, false, false, false, false, false, false);
                kilitListe.AddRange(new int[] { 3, 4 });
                //GridKolonKilitleAc(2, 3, true);
            }

            if (islemKod == 23)//ÝHRAÇ
                btnIhracYazdir.Show();
            else
                btnIhracYazdir.Hide();

            for (int i = 1; i <= 40; i++)
            {
                bool editable = true;
                foreach (var index in kilitListe)
                {
                    if (i == index)
                    {
                        editable = false;
                        break;
                    }
                }
                grdListe.ColumnModel.SetEditable(i, editable);
            }

            X.AddScript("GridKilitle(grdListe," + JSON.Serialize(kilitListe.ToArray()) + ");");

            //***************************************************
            string hesapKodEditorYok = "true";
            if (islemTuru == (int)ENUMIslemTipi.SATINALMAGIRIS || islemTuru == (int)ENUMIslemTipi.ACILIS || islemTuru == (int)ENUMIslemTipi.BAGISGIRIS)
                hesapKodEditorYok = "false";

            X.AddScript("hesapKodEditorYok=" + hesapKodEditorYok + ";");
            //***************************************************

            if (islemTuru == (int)ENUMIslemTipi.SATINALMAGIRIS || islemTuru == (int)ENUMIslemTipi.BAGISGIRIS)
                btnYerleskeSec.Show();
            else
                btnYerleskeSec.Hide();
        }

        /// <summary>
        /// Sayfa adresinde gelen kutuphane ve muze girdi dizgilerinin deðerlerine göre ilgili deðiþkenleri ayarlar.
        /// </summary>
        private void BaslikBilgileriniAyarla()
        {
            if (Request.QueryString["kutuphane"] + "" != "")
            {
                kutuphaneGoster = true;
                hdnBelgeTur.Value = "kutuphane";
            }
            else
                kutuphaneGoster = false;

            if (Request.QueryString["muze"] + "" != "")
            {
                muzeGoster = true;
                hdnBelgeTur.Value = "muze";
            }
            else
                muzeGoster = false;

            if (Request.QueryString["dagitimIade"] + "" != "")
            {
                hdnBelgeTur.Value = "dagitimIade";
                dagitimIade = true;
            }
            else
                dagitimIade = false;
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
            //FormAlanGosterGizle(islemTuru, false, true, false, false, true, false, false, true);

            cfNeredenGeldi.Hidden = !neredenGeldi;
            fcDayanak.Hidden = !dayanakBelge;
            fcFatura.Hidden = !fatura;
            fcMuayene.Hidden = !komisyon;
            cfNereyeGitti.Hidden = !nereyeGitti;
            cfKimeGitti.Hidden = !kimeGitti;
            //btnDevirListesi.Hidden = !gelenGonderilen;
            cfGonMuhasebe.Hidden = !gelenGonderilen;
            cfGonHarcamaBirimi.Hidden = !gelenGonderilen;
            cfGonAmbar.Hidden = !gelenGonderilen;
            fcGonYilBelgeNo.Hidden = !gelen;

            //divNeredenGeldi.Style["display"] = neredenGeldi == true ? "block" : "none";
            //divDayanakBelge.Style["display"] = dayanakBelge == true ? "block" : "none";
            //divFatura.Style["display"] = fatura == true ? "block" : "none";
            //divKomisyon.Style["display"] = komisyon == true ? "block" : "none";
            //divNereyeGitti.Style["display"] = nereyeGitti == true ? "block" : "none";
            //divKimeGitti.Style["display"] = kimeGitti == true ? "block" : "none";
            //divGonderilenBirim.Style["display"] = gelenGonderilen == true ? "block" : "none";
            //divGonderilenBelge.Style["display"] = gelen == true ? "block" : "none";

            bool dagitim = (islemTuru == (int)ENUMIslemTipi.DAGITIMIADECIKIS || islemTuru == (int)ENUMIslemTipi.DAGITIMIADEGIRIS);
            if (gelenGonderilen)
            {
                cfGonMuhasebe.Hidden = dagitim;
                cfGonHarcamaBirimi.Hidden = dagitim;
            }

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                fcDayanak.Hidden = true;

            //divGonderilenMuhasebe.Style["display"] = dagitim == true ? "none" : "block";
            //divGonderilenHarcamaBirimi.Style["display"] = dagitim == true ? "none" : "block";
            ////divIslemTipi.Style["display"] = dagitim == true ? "none" : "block";
            //divDayanakBelge.Style["display"] = dagitim == true ? "none" : "block";

            bool doviz = (islemTuru == (int)ENUMIslemTipi.ACILIS ||
                          islemTuru == (int)ENUMIslemTipi.BAGISGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.DAGITIMIADEGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.DEVIRGIRISKURUM ||
                          islemTuru == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS ||
                          islemTuru == (int)ENUMIslemTipi.URETILENGIRIS);

            string paraBirimi = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEPARABIRIMI");
            if (!string.IsNullOrEmpty(paraBirimi))
            {
                ddlDoviz.Hidden = (!doviz);
                ddlDoviz.SelectedIndex = 0;
                //divDoviz.Style["display"] = doviz == true ? "block" : "none";
                //ddlDoviz.SelectedIndex = 0;
            }
            else
                ddlDoviz.Hidden = true;

            ////***Kullanýcý birimi þeklinde çalýþýyor ise devir ambarýný gösterme*****************************
            //int devirSekli = OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRKULLANICIBIRIMI"), 0);
            //if (devirSekli > 0)
            //{
            //    bool devir = (islemTuru == (int)ENUMIslemTipi.DEVIRCIKIS || islemTuru == (int)ENUMIslemTipi.DEVIRGIRIS);
            //    divGonderilenAmbar.Style["display"] = devir == true ? "none" : "block";
            //}


            if (!cfGonAmbar.Hidden && TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                txtGonAmbar.Value = txtAmbar.Value;
                lblGonAmbarAd.Text = lblAmbarAd.Text;
                txtGonAmbar.FieldLabel = "Ambar";
                txtGonAmbar.Hide();
            }
        }

        private void YilDoldur()
        {
            txtYil.Value = DateTime.Now.Year;
            txtGonYil.Value = DateTime.Now.Year;
        }

        private void DovizDoldur()
        {
            TNS.HRC.IHRCServis servis = TNS.HRC.Arac.Tanimla();
            TNS.BKD.ParaBirimi para = new TNS.BKD.ParaBirimi();
            ObjectArray paralar = servis.ParaBirimiListele(kullanan, para);

            foreach (TNS.BKD.ParaBirimi bilgi in paralar.objeler)
                ddlDoviz.Items.Add(new Ext1.Net.ListItem(bilgi.kisaltma, bilgi.kisaltma));

            ddlDoviz.SelectedIndex = 0;
        }


        /// <summary>
        /// Ýþlem tipi bilgileri sunucudan çekilir ve ddlIslemTipi DropDownList kontrolüne doldurulur.
        /// </summary>
        private void IslemTipiDoldur()
        {
            List<object> listeStore = new List<object>();
            int islemTur = -1;

            List<IslemTip> islemListe = TasinirGenel.IslemTipListele(servisTMM, kullanan, dagitimIade);

            foreach (IslemTip it in islemListe)
            {
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && it.ad == "Envanter")
                    continue;

                listeStore.Add(new { KOD = it.kod, AD = it.ad, TUR = it.tur, KODTUR = it.kod + "*" + it.tur });
                if (islemTur == -1)
                    islemTur = it.tur;

                if (it.ad == "Satýn Alma(Giriþ)")
                    acilistaIslemTipiSec = it.kod + "*" + it.tur;

                //if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && it.ad == "Ýhraç" && it.tur == (int)ENUMIslemTipi.DEVIRCIKISKURUM)
                //    listeStore.Add(new { KOD = (int)ENUMIslemTipi.GECICIIHRAC, AD = "Ýhraç (Geçici)", TUR = it.tur, KODTUR = (int)ENUMIslemTipi.GECICIIHRAC + "*" + it.tur });
            }

            ddlIslemTipi.GetStore().DataSource = listeStore;
            ddlIslemTipi.GetStore().DataBind();
        }

        private int IslemTuruGetir(int islemTipiKod)
        {
            List<IslemTip> islemListe = TasinirGenel.IslemTipListele(servisTMM, kullanan, dagitimIade);

            foreach (IslemTip it in islemListe)
                if (it.kod == islemTipiKod)
                    return it.tur;
            return -1;
        }

        protected void HesapStore_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Parameters["query"]))
                return;

            List<object> liste = HesapListesiDoldur(e.Parameters["query"]);

            e.Total = 0;
            if (liste != null && liste.Count != 0)
            {
                var limit = e.Limit;
                if ((e.Start + e.Limit) > liste.Count)
                    limit = liste.Count - e.Start;

                e.Total = liste.Count;
                List<object> rangeData = (e.Start < 0 || limit < 0) ? liste : liste.GetRange(e.Start, limit);
                strHesapPlan.DataSource = (object[])rangeData.ToArray();
                strHesapPlan.DataBind();
            }
            else
            {
                strHesapPlan.DataSource = new object[] { };
                strHesapPlan.DataBind();
            }
        }

        List<object> HesapListesiDoldur(string kriter)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();

            if (hdnEkranTip.Text == "YZ")
                h.hesapKod = "094";
            else if (hdnEkranTip.Text == "GM")
                h.hesapKodAciklama = "@097 098";

            h.hesapKodAciklama = kriter;
            h.detay = true;
            ObjectArray hesap = servisTMM.HesapPlaniListele(kullanan, h, new Sayfalama());

            List<object> liste = new List<object>();
            foreach (HesapPlaniSatir detay in hesap.objeler)
            {
                if (hdnEkranTip.Text == "" && TNS.TMM.Arac.MerkezBankasiKullaniyor() && (detay.hesapKod.StartsWith("094") || detay.hesapKod.StartsWith("097") || detay.hesapKod.StartsWith("098")))
                    continue;

                liste.Add(new
                {
                    hesapPlanKod = detay.hesapKod,
                    hesapPlanAd = detay.aciklama,
                    olcuBirimAd = detay.olcuBirimAd,
                    kdvOran = detay.kdv,
                    rfidEtiketKod = detay.rfidEtiketKod,
                    markaKod = detay.markaKod,
                    modelKod = detay.modelKod,
                    vurgula = detay.vurgula
                });
            }
            return liste;
        }

        private decimal DovizDegerGetir()
        {
            decimal dovizDeger = (decimal)1;
            if (!ddlDoviz.Hidden)
            {
                string doviz = ddlDoviz.Value + "";
                string paraBirimi = TNS.TMM.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEPARABIRIMI");
                if (paraBirimi != "" && paraBirimi != doviz)
                {
                    TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

                    TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();

                    string kurtarih = (txtFaturaTarih.RawText == "" ? txtBelgeTarih.RawText : txtFaturaTarih.RawText);
                    if (kurtarih.Trim() == "") kurtarih = txtBelgeTarih.RawText;

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

            return dovizDeger;
        }

        private TasinirIslemDetay[] GridOku(JArray jArray)
        {
            List<TasinirIslemDetay> liste = new List<TasinirIslemDetay>();

            decimal dovizDeger = DovizDegerGetir();

            int siraNo = 0;
            foreach (JContainer jc in jArray)
            {
                TasinirIslemDetay td = new TasinirIslemDetay();
                td.hesapPlanKod = (jc.Value<string>("hesapPlanKod") + "").Split('-')[0].Replace(".", "");
                if (td.hesapPlanKod == string.Empty)
                    continue;

                siraNo++;
                //td.yil = tf.yil;
                //td.muhasebeKod = tf.muhasebeKod;
                //td.harcamaKod = tf.harcamaKod;
                //td.ambarKod = tf.ambarKod;
                td.siraNo = siraNo;
                td.gorSicilNo = jc.Value<string>("gorSicilNo") + "";
                td.miktar = OrtakFonksiyonlar.ConvertToDecimal((jc.Value<string>("miktar") + "").Replace(".", ","));
                td.kdvOran = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("kdvOran"), 0);
                td.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal((jc.Value<double>("birimFiyat").ToString("R") + "")) * dovizDeger;
                //Serverda hesaplanýyor (kdv mükellefi olan muhasebelerden dolayý commentlendi)
                //td.birimFiyatKDVLi = (1 + (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100)) * td.birimFiyat;
                td.hesapPlanAd = jc.Value<string>("hesapPlanAd") + "";
                td.yerleskeYeri = jc.Value<string>("yerleskeYeri");
                td.yerleskeYeriAd = jc.Value<string>("yerleskeYeriAd");
                if (!string.IsNullOrWhiteSpace(jc.Value<string>("satisBedeli")))
                    td.satisBedeli = OrtakFonksiyonlar.ConvertToDecimal((jc.Value<double>("satisBedeli").ToString("R") + ""));

                if (kutuphaneGoster)
                {
                    td.ozellik.ciltNo = jc.Value<string>("ciltNo") + "";
                    td.ozellik.dil = jc.Value<string>("dil") + "";
                    td.ozellik.yazarAdi = jc.Value<string>("yazarAdi") + "";
                    td.ozellik.adi = jc.Value<string>("adi") + "";
                    td.ozellik.yayinYeri = jc.Value<string>("yayinYeri") + "";
                    td.ozellik.yayinTarihi = jc.Value<string>("yayinTarihi") + "";
                    td.ozellik.neredenGeldi = jc.Value<string>("neredenGeldi") + "";
                    if (string.IsNullOrEmpty(td.ozellik.neredenGeldi) && !string.IsNullOrEmpty(txtNeredenGeldi.Text))
                        td.ozellik.neredenGeldi = txtNeredenGeldi.Text;

                    td.ozellik.boyutlari = jc.Value<string>("boyutlari") + "";
                    td.ozellik.satirSayisi = jc.Value<string>("satirSayisi") + "";
                    td.ozellik.yaprakSayisi = jc.Value<string>("yaprakSayisi") + "";
                    td.ozellik.sayfaSayisi = jc.Value<string>("sayfaSayisi") + "";
                    td.ozellik.ciltTuru = jc.Value<string>("ciltTuru") + "";
                    td.ozellik.cesidi = jc.Value<string>("cesidi") + "";
                }
                else if (muzeGoster)
                {
                    td.ozellik.adi = jc.Value<string>("adi") + "";
                    td.ozellik.gelisTarihi = jc.Value<string>("gelisTarihi") + "";
                    td.ozellik.neredenGeldi = jc.Value<string>("neredenGeldi") + "";
                    td.ozellik.neredeBulundu = jc.Value<string>("neredeBulundu") + "";
                    td.ozellik.cagi = jc.Value<string>("cagi") + "";
                    td.ozellik.boyutlari = jc.Value<string>("boyutlari") + "";
                    td.ozellik.durumuMaddesi = jc.Value<string>("durumuMaddesi") + "";
                    td.ozellik.onYuz = jc.Value<string>("onYuz") + "";
                    td.ozellik.arkaYuz = jc.Value<string>("arkaYuz") + "";
                    td.ozellik.puan = jc.Value<string>("puan") + "";
                }
                else
                {
                    td.eSicilNo = jc.Value<string>("eSicilNo") + ""; //Devir giriþi ise satýrdaki, Açýlýþ ise eski demirbaþ no, yoksa boþ.
                    if (!string.IsNullOrWhiteSpace(jc.Value<string>("eAlimTarihi")))
                        td.eAlimTarihi = new TNSDateTime(jc.Value<DateTime>("eAlimTarihi"));
                    td.eTedarikSekli = jc.Value<string>("eTedarikSekli") + "";
                    td.ozellik.yayinTarihi = jc.Value<string>("eAlimTarihi") + "";

                    //if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    //    if (string.IsNullOrEmpty(td.ozellik.yayinTarihi)) td.ozellik.yayinTarihi = txtBelgeTarih.Text;
                }

                td.ozellik.saseNo = jc.Value<string>("eAciklama") + ""; //Açýklama alaný ayný zamanda seri no bilgisi
                td.eAciklama = jc.Value<string>("eAciklama") + ""; //Devir giriþi ise satýrdaki, Açýlýþ ise eski demirbaþ no, yoksa boþ.
                td.ozellik.disSicilNo = jc.Value<string>("disSicilNo") + "";
                td.ozellik.adi = jc.Value<string>("adi") + "";
                td.ozellik.yeriKonusu = jc.Value<string>("yeriKonusu") + "";


                td.gonHarcamaKod = txtGonHarcamaBirimi.Text.Replace(".", "");
                td.gonMuhasebeKod = txtGonMuhasebe.Text;

                if (!string.IsNullOrWhiteSpace(jc.Value<string>("garantiBitisTarihi")))
                    td.ozellik.garantiBitisTarihi = new TNSDateTime(jc.Value<DateTime>("garantiBitisTarihi"));

                td.ozellik.giai = jc.Value<string>("giai") + "";
                td.ozellik.rfidEtiketTurKod = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("rfidEtiketTurKod"), 0);
                td.ozellik.markaKod = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("markaKod"), 0);
                td.ozellik.modelKod = OrtakFonksiyonlar.ConvertToInt(jc.Value<string>("modelKod"), 0);


                liste.Add(td);
                //tf.detay.Ekle(td);
            }

            return liste.ToArray();
        }

        private void GridTemizle(int satirSayisi = 30)
        {
            EkAlanlariTemizle();

            DataTable dt = GridDataTable();
            for (int i = 0; i < satirSayisi; i++)
                dt.Rows.Add(dt.NewRow());

            strListe.DataSource = dt;
            strListe.DataBind();
        }

        private DataTable GridDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[] {
                    new DataColumn("hesapPlanKod")   { DataType = typeof(string) },
                    new DataColumn("gorSicilNo")   { DataType = typeof(string) },
                    new DataColumn("miktar")   { DataType = typeof(decimal) },
                    new DataColumn("olcuBirimAd")   { DataType = typeof(string) },
                    new DataColumn("kdvOran")   { DataType = typeof(string) },
                    new DataColumn("birimFiyat")   { DataType = typeof(decimal) },
                    new DataColumn("hesapPlanAd")   { DataType = typeof(string) },

                    new DataColumn("disSicilNo")   { DataType = typeof(string) },
                    new DataColumn("ciltNo")   { DataType = typeof(string) },
                    new DataColumn("dil")   { DataType = typeof(string) },
                    new DataColumn("yazarAdi")   { DataType = typeof(string) },
                    new DataColumn("adi")   { DataType = typeof(string) },
                    new DataColumn("yayinYeri")   { DataType = typeof(string) },
                    new DataColumn("yayinTarihi")   { DataType = typeof(string) },
                    new DataColumn("gelisTarihi")   { DataType = typeof(string) },
                    new DataColumn("neredenGeldi")   { DataType = typeof(string) },
                    new DataColumn("neredeBulundu")   { DataType = typeof(string) },
                    new DataColumn("cagi")   { DataType = typeof(string) },
                    new DataColumn("boyutlari")   { DataType = typeof(string) },
                    new DataColumn("satirSayisi")   { DataType = typeof(string) },
                    new DataColumn("yaprakSayisi")   { DataType = typeof(string) },
                    new DataColumn("sayfaSayisi")   { DataType = typeof(string) },
                    new DataColumn("ciltTuru")   { DataType = typeof(string) },
                    new DataColumn("cesidi")   { DataType = typeof(string) },
                    new DataColumn("durumuMaddesi")   { DataType = typeof(string) },
                    new DataColumn("onYuz")   { DataType = typeof(string) },
                    new DataColumn("arkaYuz")   { DataType = typeof(string) },
                    new DataColumn("puan")   { DataType = typeof(string) },
                    new DataColumn("yeriKonusu")   { DataType = typeof(string) },
                    new DataColumn("eAciklama")   { DataType = typeof(string) },
                    new DataColumn("eSicilNo")   { DataType = typeof(string) },
                    new DataColumn("eAlimTarihi")   { DataType = typeof(DateTime) },
                    new DataColumn("eTedarikSekli")   { DataType = typeof(string) },
                    new DataColumn("garantiBitisTarihi")   { DataType = typeof(DateTime) },
                    new DataColumn("giai")   { DataType = typeof(string) },
                    new DataColumn("rfidEtiketTurKod")   { DataType = typeof(int) },
                    new DataColumn("markaKod")   { DataType = typeof(int) },
                    new DataColumn("modelKod")   { DataType = typeof(int) },
                    new DataColumn("yerleskeYeri")   { DataType = typeof(string) },
                    new DataColumn("yerleskeYeriAd")   { DataType = typeof(string) },
                    new DataColumn("satisBedeli")   { DataType = typeof(decimal) }
                });

            return dt;
        }

        protected void btnOnayaGonder_Click(object sender, DirectEventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTIG012 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG013 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTIG014 + "<br>";
            else if (hdnKod.Text == "")
                hata += "Belge numarasý sorgulanmamýþ.<br>Not: Belge numarasýný yanýnda bulunan mercek ile sorgulayýnýz." + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Hata", hata + " Onaya gönderme iþlemi gerçekleþmedi.");
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.kod = hdnKod.Text;
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Value, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");
            tf.onayAciklama = "";

            Sonuc sonuc = servisTMM.TasinirIslemFisiOnayDurumDegistir(kullanan, tf, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);

            if (sonuc.islemSonuc)
            {
                DurumAdDegistir("B ONAYINA GÖNDERÝLDÝ");

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);

        }

        protected void btnIptal_Click(object sender, DirectEventArgs e)
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
                GenelIslemler.MesajKutusu("Hata", hata);
                return;
            }

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.kod = hdnKod.Text;
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.fisNo = txtBelgeNo.Text == string.Empty ? string.Empty : txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.muhasebeKod = txtMuhasebe.Text.Replace(".", "");
            tf.harcamaKod = txtHarcamaBirimi.Text.Replace(".", "");

            Sonuc sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "Ýptal");

            if (sonuc.islemSonuc)
            {
                DurumAdDegistir(Resources.TasinirMal.FRMZFG007);

                GenelIslemler.MesajKutusu("Bilgi", sonuc.bilgiStr);
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        protected void btnIhracYazdir_Click(object sender, DirectEventArgs e)
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            t.belgeNo = txtBelgeNo.Text;
            t.raporTur = (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI;

            Raporlar.IhracAmortismanliHazirla(kullanan, servisTMM, t, "3", false);
        }

        protected void ddlMutemmimSelect(object sender, DirectEventArgs e)
        {
            bool mutemmim = OrtakFonksiyonlar.ConvertToInt(hdnMutemmim.Value, 0) == 1;

            if (mutemmim)
            {
                int yil = DateTime.Now.AddYears(-1).Year;
                txtYil.SetValue(DateTime.Now.AddYears(-1).Year);
                txtBelgeTarih.SetValue(new DateTime(yil, 12, 31));
            }
            else
            {
                txtYil.SetValue(DateTime.Now.Year);
                txtBelgeTarih.SetValue(DateTime.Now.Date);
            }
        }

        private void DurumAdDegistir(string durum)
        {
            lblFormDurum.Text = durum == "" ? "" :
                string.Format("<a href=\"javascript:TarihceGoster('{0}','{1}','{2}','{3}');\">{4}</a>", OrtakFonksiyonlar.ConvertToInt(txtYil.Value, 0), txtMuhasebe.Text, txtHarcamaBirimi.Text.Replace(".", ""), txtBelgeNo.Text.Trim().PadLeft(6, '0'), durum);
        }

        private void MarkaDoldur()
        {
            ObjectArray bilgi = servisTMM.MarkaListele(kullanan, new Marka());

            List<object> liste = new List<object>();
            foreach (Marka ob in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ob.kod,
                    ADI = ob.ad
                });
            }

            strMarka.DataSource = liste;
            strMarka.DataBind();
        }

        private void ModelDoldur()
        {
            Model model = new Model();
            ObjectArray bilgi = servisTMM.ModelListele(kullanan, model);

            List<object> liste = new List<object>();

            foreach (TNS.TMM.Model item in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    ADI = item.ad,
                    MARKAKOD = item.markaKodu
                });
            }

            strModel.DataSource = liste;
            strModel.DataBind();
        }

        private void RFIDEtiketTuruDoldur()
        {
            ObjectArray bilgi = servisTMM.RFIDEtiketTuruListele(kullanan, new RFIDEtiketTuru());

            List<object> liste = new List<object>();
            foreach (RFIDEtiketTuru ob in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ob.kod,
                    ADI = ob.ad
                });
            }

            strRFIDEtiket.DataSource = liste;
            strRFIDEtiket.DataBind();
        }

        public string EkranTurDondur()
        {
            string tur = Request.QueryString["gm"] + "";
            if (tur == "1") return "GM";
            else if (tur == "2") return "YZ";
            else return "";
        }
        public void YZTurDoldur()
        {
            string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMHESAPKOD") + "";
            hdnHesapKod.Value = "@" + hesapKodGM;
            txtAmbar.Value = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMAMBAR") + "";
            lblAmbarAd.Text = "Yazýlým Ambarý";
        }

        public void GMTurDoldur()
        {
            string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULHESAPKOD") + "";
            hdnHesapKod.Value = "@" + hesapKodGM;
            txtAmbar.Value = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULAMBAR") + "";
            lblAmbarAd.Text = "Gayrimenkul Ambarý";
        }

        private bool MutemmimAktifMi()
        {
            bool mutemmim = servisTMM.MutemmimAktifMi(kullanan, new AyarlarMB { muhasebeKod = txtMuhasebe.Text, harcamaKod = txtHarcamaBirimi.Text, ambarKod = txtAmbar.Text });
            return mutemmim;
        }

        public void MutemmimAlaniEkle()
        {
            //ddlMutemmim.Show();


            //bool mutemmim = false;

            ////if (DateTime.Now.Month == 1)
            ////{
            //AyarlarMB form = new AyarlarMB
            //{
            //    muhasebeKod = txtMuhasebe.Text,
            //    harcamaKod = txtHarcamaBirimi.Text,
            //    ambarKod = txtAmbar.Text
            //};

            //if (form.muhasebeKod == "" && form.harcamaKod == "" && form.ambarKod == "")
            //    mutemmim = true;
            //else
            //{
            //    mutemmim = servisTMM.MutemmimAktifMi(kullanan, form);
            //}
            ////}

            //if (mutemmim)
            //    ddlMutemmim.Show();
            //else
            //    ddlMutemmim.Hide();
        }
    }
}