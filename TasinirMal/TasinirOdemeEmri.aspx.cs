using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.MUH;
using TNS.TMM;
using TNS.UZY;
using System.Collections.Generic;


namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr iþlem fiþinden ödeme emri belgesi hazýrlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirOdemeEmri : TMMSayfa
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Bütçe muhasebe servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMTOE001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.btnOlustur.Attributes.Add("onclick", "return OnayAl('odemeEmriOlustur','btnOlustur');");
            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtHesapKod.Attributes.Add("onblur", "kodAdGetir('13','lblHesapPlanAd',true,new Array('hdnYil','txtHesapKod'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                ViewState["fpID"] = DateTime.Now.ToLongTimeString();

                YilDoldur();
                TevkifatDoldur();
                GridInit(fpL);
                btnTemizle_Click(null, null);
                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtHesapKod.Text.Trim() != "")
                lblHesapPlanAd.Text = GenelIslemler.KodAd(13, hdnYil.Value.Trim() + "-" + txtHesapKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = "";
        }

        /// <summary>
        /// Belgeyi Bul resmine basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri sunucuya gönderilir ve taþýnýr iþlem fiþine ait ödeme emri bilgileri
        /// sunucudan alýnýr. Hata varsa ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, ImageClickEventArgs e)
        {
            string hata = "";
            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTOE002 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTOE003 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTOE004 + "<br>";

            bool kdvSatirOlustur = false;
            bool durumKDV = servisTMM.MuhasebeKdvMukellefiMi(kullanan, txtMuhasebe.Text.Trim());
            if (durumKDV)
                kdvSatirOlustur = true;
            else
                kdvSatirOlustur = false;

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");
            hdnYil.Value = ddlYil.SelectedValue;//hesap Kodu listesi almak için gerekli

            fpL.CancelEdit();
            GridInit(fpL);

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.muhasebeKod = txtMuhasebe.Text.Trim();

            ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, false);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                return;
            }

            tf = (TNS.TMM.TasinirIslemForm)bilgi[0];

            if (tf.durum != (int)TNS.TMM.ENUMBelgeDurumu.ONAYLI)
                hata += Resources.TasinirMal.FRMTOE005 + "<br>";

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            txtMuhasebe.Text = tf.muhasebeKod;
            txtHarcamaBirimi.Text = tf.harcamaKod;
            txtBelgeNo.Text = tf.fisNo;

            if (tf.neredenGeldi != "")
            {
                ObjectArray fv = new ObjectArray();
                TNS.TMM.FirmaBilgisi fSorgu = new TNS.TMM.FirmaBilgisi();
                fSorgu.ad = tf.neredenGeldi;

                fv = servisTMM.FirmaListele(fSorgu);

                if (fv.sonuc.islemSonuc)
                {
                    if (fv.objeler.Count > 0)
                    {
                        TNS.TMM.FirmaBilgisi f = new TNS.TMM.FirmaBilgisi();
                        f = (TNS.TMM.FirmaBilgisi)fv.objeler[0];
                        txtIlgiliAd.Text = f.ad;
                        txtIlgiliNo.Text = f.vno;
                        txtIlgiliVD.Text = f.vd;
                        txtIlgiliBankaAd.Text = f.banka;
                        txtIlgiliBankaNo.Text = f.hesapNo;
                    }
                }
            }

            string faturaTarihi = tf.faturaTarih.ToString();
            string faturaNo = tf.faturaNo;
            txtFaturaTarih.Text = faturaTarihi;
            txtFaturaNo.Text = faturaNo;
            hdnMuhasebe.Value = txtMuhasebe.Text;
            hdnBirim.Value = txtHarcamaBirimi.Text.Trim().Replace(".", "").Substring(8, 3);
            hdnKur.Value = txtHarcamaBirimi.Text.Trim().Replace(".", "").Substring(0, 8);

            txtAciklama.Text = string.Format(Resources.TasinirMal.FRMTOE006, tf.fisNo);

            if (faturaTarihi != "")
                txtAciklama.Text += string.Format(Resources.TasinirMal.FRMTOE007, tf.faturaTarih.ToString());

            if (faturaNo != "")
                txtAciklama.Text += string.Format(Resources.TasinirMal.FRMTOE008, tf.faturaNo);

            string islemTipAd = servisUZY.UzayDegeriStr(null, "TASISLEMTIPAD", tf.islemTipKod.ToString(), true, "");
            int islemTur = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(null, "TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "").ToString(), 0);

            if (islemTur == (int)ENUMIslemTipi.TUKETIMCIKIS ||
                islemTur == (int)ENUMIslemTipi.YILDEVIRGIRIS ||
                islemTur == (int)ENUMIslemTipi.YILDEVIRCIKIS)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTOE009);
                return;
            }

            if (islemTur == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                islemTur == (int)ENUMIslemTipi.URETILENGIRIS ||
                islemTur == (int)ENUMIslemTipi.DEGERARTTIR ||
                islemTur == (int)ENUMIslemTipi.ENFLASYONARTISI)
            {
                chkDamga.Checked = false;
                chkDamga.Visible = true;
            }
            else
            {
                chkDamga.Checked = false;
                chkDamga.Visible = false;
            }


            if (GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "") == "1399") // Manas
            {
                chkDamga.Checked = false;
                chkDamga.Visible = false;
            }


            //Yeni iþlem þekline göre 
            bool kdvSatiriOlacak = true;
            bool yansitmaOlacak = true;
            if (tf.yil == DateTime.Now.Year)
                hdnBelgeTarih.Value = DateTime.Now.ToString();
            else
                hdnBelgeTarih.Value = "31.12." + tf.yil.ToString();

            chkDamga.Visible = true;

            if (islemTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                islemTur == (int)ENUMIslemTipi.DEVIRGIRIS ||
                islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS ||
                islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                divBilgiler.Visible = false;
                txtAciklama.Text += ENUMIslemTipi.DEVIRCIKIS.ToString();
                yansitmaOlacak = false;
            }

            if (islemTur == (int)ENUMIslemTipi.ACILIS)
            {
                chkDamga.Checked = false;
                chkDamga.Visible = false;
                kdvSatiriOlacak = false;
                yansitmaOlacak = false;
                txtAciklama.Text += Resources.TasinirMal.FRMTOE010;
                hdnBelgeTarih.Value = "02.01." + ddlYil.SelectedValue;
            }
            if (kdvSatirOlustur && islemTur != (int)ENUMIslemTipi.SATINALMAGIRIS)
                kdvSatiriOlacak = false;

            if (yansitmaOlacak)
                fpL.Sheets[0].Columns[3].Visible = true;
            else
                fpL.Sheets[0].Columns[3].Visible = false;

            string eskiHesapKod = "";
            string eskiKDVYuzdesi = "";
            decimal tutar = 0;
            decimal tutarKDVSiz = 0;
            string kdvYuzdesi = "";

            ObjectArray satirlar = new ObjectArray();
            TasinirIslemMIFDetay odemeEmriDetay = new TasinirIslemMIFDetay();

            foreach (TasinirIslemDetay td in tf.detay.objeler)
            {
                string hesapKod = td.hesapPlanKod.Substring(0, 9);
                kdvYuzdesi = td.kdvOran.ToString();

                if ((hesapKod != eskiHesapKod && eskiHesapKod != "") || (kdvSatirOlustur && kdvYuzdesi != eskiKDVYuzdesi && eskiKDVYuzdesi != ""))
                {
                    odemeEmriDetay = new TasinirIslemMIFDetay();
                    odemeEmriDetay.hesapKod = eskiHesapKod;
                    if (islemTur < 50)
                    {
                        odemeEmriDetay.borcKDVDahil = OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0);
                        odemeEmriDetay.borcKDVHaric = OrtakFonksiyonlar.ConvertToDouble(tutarKDVSiz.ToString(), (double)0);
                    }
                    else
                    {
                        odemeEmriDetay.alacakKDVDahil = OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0);
                        odemeEmriDetay.alacakKDVHaric = OrtakFonksiyonlar.ConvertToDouble(tutarKDVSiz.ToString(), (double)0);
                    }
                    odemeEmriDetay.kdvYuzdesi = eskiKDVYuzdesi;

                    satirlar.objeler.Add(odemeEmriDetay);
                    tutar = 0;
                    tutarKDVSiz = 0;
                }

                decimal tutarX = td.miktar * td.birimFiyat;
                decimal kdv = td.miktar * td.birimFiyat * (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100);

                //decimal tutarX = Math.Round(td.miktar * td.birimFiyat, 2);
                //decimal kdv = Math.Round(tutarX * (OrtakFonksiyonlar.ConvertToDecimal(td.kdvOran) / 100), 2);

                tutar += Math.Round(tutarX + kdv, 2);
                tutarKDVSiz += Math.Round(tutarX, 2);

                eskiKDVYuzdesi = kdvYuzdesi;
                eskiHesapKod = hesapKod;
            }
            //Döngüde son bölüm
            odemeEmriDetay = new TasinirIslemMIFDetay();
            odemeEmriDetay.hesapKod = eskiHesapKod;
            if (islemTur < 50)
            {
                odemeEmriDetay.borcKDVDahil = OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0);
                odemeEmriDetay.borcKDVHaric = OrtakFonksiyonlar.ConvertToDouble(tutarKDVSiz.ToString(), (double)0);
            }
            else
            {
                odemeEmriDetay.alacakKDVDahil = OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0);
                odemeEmriDetay.alacakKDVHaric = OrtakFonksiyonlar.ConvertToDouble(tutarKDVSiz.ToString(), (double)0);
            }
            odemeEmriDetay.kdvYuzdesi = kdvYuzdesi;

            satirlar.objeler.Add(odemeEmriDetay);

            //Taþýnýr detay bilgilerini gride yaz
            fpL.Sheets[0].RowCount = satirlar.objeler.Count;
            int satir = 0;

            ObjectArray hy = new ObjectArray();

            double toplamBorc = 0.0;
            double toplamAlacak = 0.0;

            foreach (TasinirIslemMIFDetay o in satirlar.objeler)
            {
                fpL.Sheets[0].Cells[satir, 0].Value = o.hesapKod;
                if (kdvSatirOlustur)
                {
                    fpL.Sheets[0].Cells[satir, 1].Value = o.borcKDVHaric.ToString("#,###.00");
                    fpL.Sheets[0].Cells[satir, 2].Value = o.alacakKDVHaric.ToString("#,###.00");
                    toplamBorc += o.borcKDVHaric;
                    toplamAlacak += o.alacakKDVHaric;
                }
                else
                {
                    fpL.Sheets[0].Cells[satir, 1].Value = o.borcKDVDahil.ToString("#,###.00");
                    fpL.Sheets[0].Cells[satir, 2].Value = o.alacakKDVDahil.ToString("#,###.00");
                    toplamBorc += o.borcKDVDahil;
                    toplamAlacak += o.alacakKDVDahil;
                }
                fpL.Sheets[0].Cells[satir, 7].Value = o.borcKDVHaric;
                fpL.Sheets[0].Cells[satir, 8].Value = o.borcKDVDahil.ToString("#,###.00"); ;
                fpL.Sheets[0].Cells[satir, 9].Value = o.alacakKDVDahil.ToString("#,###.00");

                if (kdvSatirOlustur && !kdvSatiriOlacak)//KDV Mülkellefi için KDV satýrlarý oluþmayacak ise
                {
                    fpL.Sheets[0].Cells[satir, 8].Value = o.borcKDVHaric.ToString("#,###.00");
                    fpL.Sheets[0].Cells[satir, 9].Value = o.alacakKDVHaric.ToString("#,###.00");
                }

                o.hesapAd = servisUZY.UzayDegeriStr(null, "TASHESAPPLAN", o.hesapKod, true, "");
                fpL.Sheets[0].Cells[satir, 6].Value = o.hesapAd;

                TNS.MUH.HesapPlaniSatir h = new TNS.MUH.HesapPlaniSatir();
                h.yil = tf.yil;
                h.hesapKod = o.hesapKod;

                hy.objeler.Add(h);

                satir++;

                if (kdvSatirOlustur && kdvSatiriOlacak)
                {
                    fpL.Sheets[0].RowCount += 1;

                    fpL.Sheets[0].Cells[satir, 0].BackColor = System.Drawing.Color.White;
                    fpL.Sheets[0].Cells[satir, 0].Locked = false;

                    string kdvHesabi = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebeKDVHesabi" + o.kdvYuzdesi) + "";

                    fpL.Sheets[0].Cells[satir, 0].Value = kdvHesabi;
                    fpL.Sheets[0].Cells[satir, 1].Value = (o.borcKDVDahil - o.borcKDVHaric);
                    fpL.Sheets[0].Cells[satir, 2].Value = (o.alacakKDVDahil - o.alacakKDVHaric);
                    fpL.Sheets[0].Cells[satir, 7].Value = "";
                    fpL.Sheets[0].Cells[satir, 8].Value = "";
                    fpL.Sheets[0].Cells[satir, 9].Value = "";

                    o.hesapAd = servisUZY.UzayDegeriStr(null, "TASHESAPPLAN", kdvHesabi.Replace(".", ""), true, "");
                    fpL.Sheets[0].Cells[satir, 6].Value = o.hesapAd;

                    satir++;
                }
            }

            hdnToplamBorc.Value = toplamBorc.ToString();
            hdnToplamAlacak.Value = toplamAlacak.ToString();

            if (yansitmaOlacak)
            {
                string hk = "";
                string[] yansitma = new string[fpL.Sheets[0].Rows.Count];

                ObjectArray hyv = servisMUH.HesapPlaniYansitmaListele(kullanan, hy);
                if (hyv.sonuc.islemSonuc)
                {
                    foreach (HesapPlaniYansitma hpy in hyv.objeler)
                    {
                        for (int i = 0; i < fpL.Sheets[0].Rows.Count; i++)
                        {
                            hk = fpL.Sheets[0].Cells[i, 0].Text;
                            ObjectArray dizi = new ObjectArray();
                            int sayac = OrtakFonksiyonlar.StringiParcala(hk, "-", dizi);

                            if (sayac > 0)
                            {
                                if (((string)dizi.objeler[0]).Replace(".", "").Trim() == hpy.hesapKod.Replace(".", ""))
                                {
                                    if (yansitma[i] != null)
                                        yansitma[i] += "|";

                                    if (hpy.yansitmaHesapKod.StartsWith("83"))//sadece 830 ve 834 lerin çýkmasý için
                                        yansitma[i] += hpy.yansitmaHesapKod + " - " + hpy.yKodAciklama;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < fpL.Sheets[0].Rows.Count; i++)
                    {
                        if (yansitma[i] != "" && yansitma[i] != null && fpL.Sheets[0].Cells[i, 3].CellType != new FarPoint.Web.Spread.ComboBoxCellType())
                        {
                            ObjectArray s = new ObjectArray();
                            int say = OrtakFonksiyonlar.StringiParcala(yansitma[i], "|", s);
                            string[] y = new string[say];

                            for (int j = 0; j < say; j++)
                                y[j] = (string)s.objeler[j];

                            FarPoint.Web.Spread.ComboBoxCellType cType = new FarPoint.Web.Spread.ComboBoxCellType(y);
                            cType.ShowButton = true;
                            fpL.Sheets[0].Cells[i, 3].CellType = cType;
                        }
                    }
                }
            }

            if (islemTur == (int)ENUMIslemTipi.ACILIS ||
                islemTur == (int)ENUMIslemTipi.DEVIRGIRIS ||
                islemTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS ||
                islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                string hesapKod = fpL.Sheets[0].Cells[0, 0].Text;
                if (hesapKod != "")
                {
                    if (hesapKod.StartsWith("150"))
                        txtHesapKod.Text = "500.02.06";
                    else if (hesapKod.StartsWith("253"))
                        txtHesapKod.Text = "500.02.03.04";
                    else if (hesapKod.StartsWith("254"))
                        txtHesapKod.Text = "500.02.03.05";
                    else if (hesapKod.StartsWith("255"))
                        txtHesapKod.Text = "500.02.03.06";
                }
            }

            divAnaBilgi.Visible = false;
            divEkBilgi.Visible = true;
            btnTemizle.Visible = true;
            btnOlustur.Visible = true;

            divListe.Visible = false;
            btnMIFListele.Visible = false;
            fpL.Visible = true;

            string MIFNo = "<img src='../App_themes/Images/aramainfo.gif'/>&nbsp;" + string.Format(Resources.TasinirMal.FRMTOE011, tf.fisNo);
            lblMIFNo.Text = MIFNo;
            tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
            ObjectArray ss = servisTMM.TasinirMIFNoListele(kullanan, tf);
            if (ss.sonuc.anahtar != "")
            {
                hdnBelgeNo.Value = ss.sonuc.anahtar;
                lblMIFNo.Text = "<img src='../App_themes/Images/aramainfo.gif'/>&nbsp;" + string.Format(Resources.TasinirMal.FRMTOE012, ss.sonuc.anahtar);
            }
        }

        /// <summary>
        /// Belge Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr iþlem fiþine ait ödeme emri bilgileri ekrandaki ilgili kontrollerden
        /// toplanýp kaydedilmek üzere sunucuya gönderilir, gelen sonuca göre hata mesajý
        /// veya bilgi mesajý verilir. Ek olarak ödeme emri belgesinin numarasý taþýnýr iþlem
        /// fiþi ile eþletirilmek üzere sunucudaki ilgili yordam aracýlýðýyla kayýt edilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnOlustur_Click(object sender, EventArgs e)
        {
            string hata = "";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            string taahhutProjeNo = "";
            int aciliOdemeEmriYil = 0;
            string aciliOdemeEmriNo = "";
            double aciliOdemeEmriTutar = 0.0;
            double kalanOdemeEmriTutari = 0.0;
            double toplamOdemeEmriTutari = 0.0;

            fpL.SaveChanges();

            if (txtMuhasebe.Text.Trim() == "")
                hata = Resources.TasinirMal.FRMTOE002 + "<br>";

            if (txtHarcamaBirimi.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTOE003 + "<br>";

            if (txtBelgeNo.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTOE004 + "<br>";

            if (txtHarcamaBirimi.Text.Trim().Replace(".", "").Length != 11)
                hata += Resources.TasinirMal.FRMTOE013 + "<br>";

            if (txtHesapKod.Text.Trim() == "")
                hata += Resources.TasinirMal.FRMTOE014 + "<br>";
            //else
            //{
            //    TNS.MUH.HesapPlaniSatir hKod = new TNS.MUH.HesapPlaniSatir();

            //    hKod.yil = yil;
            //    hKod.hesapKod = txtHesapKod.Text.Trim();

            //    hKod = servisMUH.HesapKodOzellikAl(hKod);
            //    bool kasaHesabiVar = false;
            //    if (hKod != null)
            //    {
            //        if (GenelIslemler.NitelikVarmi(hKod.nitelik, (int)ENUMHesapKodNitelik.KASAHESABI))
            //            kasaHesabiVar = true;
            //    }
            //    //Devir giriþ/çýkýþ iþlemlerinde hesap kodu alanýna 500.02.06 yazýlýyor.(Baþka bir hesapta yazýlabilir. Bu hesapta kasa hesabý olmadýðý için burasý kapatýldý.
            //    //if (!kasaHesabiVar)
            //    //    hata += "Ödeme hesap plan kodu olarak girmiþ olduðunuz kod hatalý. Lütfen listeden uygun bir kod seçiniz.<br>";
            //}

            TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
            bilgi.muteahhitKimlikNo = txtIlgiliNo.Text;
            bilgi.kod = OrtakFonksiyonlar.ConvertToInt(hdnTaahhutKod.Value, 0);
            if (bilgi.muteahhitKimlikNo.Trim() != "" && bilgi.kod > -1)
            {
                ObjectArray tListe = servisMUH.TaahhutKartiListele(kullanan, bilgi, 0);
                if (tListe.objeler.Count > 0)
                {
                    if (OrtakFonksiyonlar.ConvertToInt(hdnTaahhutKod.Value, 0) == 0)
                        hata += txtIlgiliNo.Text + " vergi numaralý " + txtIlgiliAd.Text + " firmanýn Taahhüt Dosyasý bulunmaktadýr. Lütfen ilgili Taahhüt dosyasýný seçiniz<br>";
                    else
                    {
                        TaahhutKartiBilgi tb = (TaahhutKartiBilgi)tListe.objeler[0];
                        taahhutProjeNo = tb.projeSiraNo;

                        //Açýlýþ taahhauk ödeme emri yýl ve nosu için
                        aciliOdemeEmriYil = tb.odemeEmriYil;
                        aciliOdemeEmriNo = tb.odemeEmriNo;
                        aciliOdemeEmriTutar = tb.ihaleBedeli;
                        foreach (TaahhutKartiDegerArtisi da in tb.degerArtisi.objeler)
                        {
                            aciliOdemeEmriTutar += da.sozlesmeTutari;
                        }

                        if (string.IsNullOrEmpty(aciliOdemeEmriNo.Trim()) || aciliOdemeEmriYil == 0)
                        {
                            hata += taahhutProjeNo + " nolu Taahhüt Dosyasý'nýn tahakkuku yapýlmamýþtýr. Lütfen ilk önce bu belgeyi oluþturun";
                            return;
                        }

                        toplamOdemeEmriTutari = servisMUH.TaahhutOdemeEmriHazirlananToplamTutar(kullanan, tb.kod);
                        kalanOdemeEmriTutari = aciliOdemeEmriTutar - toplamOdemeEmriTutari;
                    }
                }
            }

            if (hata != "")
            {
                GenelIslemler.MesajKutusu("Uyarý", hata);
                return;
            }

            bool yansitmaYapilacak = true;
            if (fpL.Sheets[0].Columns[3].Visible)
            {//KDV için cell boþ olabiliyor
                //for (int i = 0; i < fpL.Sheets[0].Rows.Count; i++)
                //{
                //    if (fpL.Sheets[0].Cells[i, 3].Value == null)
                //    {
                //        GenelIslemler.MesajKutusu("Uyarý", i + 1 + ". satýrda Taþýnýr Hesap Kodu karþýlýðý Bütçe kodu (Yansýtma hesabý) bulunamadý");
                //        return;
                //    }
                //}
            }
            else
                yansitmaYapilacak = false;

            double netOdenecek = 0;
            double damgaVergisi = 0;

            for (int i = 0; i < fpL.Sheets[0].Rows.Count; i++)
            {
                netOdenecek += Math.Round(OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 8].Value) - OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 9].Value), 2);

                if (chkDamga.Checked && chkDamga.Visible)
                    damgaVergisi += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 7].Value);
            }

            if (damgaVergisi > 0)
                damgaVergisi = damgaVergisi * 0.0075;
            else
                damgaVergisi = 0;

            damgaVergisi = Math.Round(damgaVergisi, 2);

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            if (hdnBelgeNo.Value != "")
            {
                tf = new TNS.TMM.TasinirIslemForm();
                tf.yil = yil;
                tf.muhasebeKod = txtMuhasebe.Text.Trim();
                tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
                tf.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
                ObjectArray ss = servisTMM.TasinirMIFNoListele(kullanan, tf);
                hdnBelgeNo.Value = ss.sonuc.anahtar;
            }

            if (OrtakFonksiyonlar.ConvertToLong(hdnBelgeNo.Value) == 0)
                hdnBelgeNo.Value = "";

            OdemeEmriMIFForm form = new OdemeEmriMIFForm();

            if (hdnBelgeTarih.Value.Trim() == "")
            {
                if (yil == DateTime.Now.Year)
                    hdnBelgeTarih.Value = DateTime.Now.ToString();
                else
                    hdnBelgeTarih.Value = "31.12." + yil.ToString();
            }
            form.yil = yil;
            form.islemTarih = new TNSDateTime(hdnBelgeTarih.Value);
            form.belgeNo = hdnBelgeNo.Value;
            form.kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            form.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text, 0);
            form.birim = txtHarcamaBirimi.Text.Trim().Replace(".", "").Substring(8, 3);
            form.ilgiliAd = txtIlgiliAd.Text;
            form.ilgiliNo = txtIlgiliNo.Text;
            form.ilgiliVD = txtIlgiliVD.Text;
            form.ilgiliBankaAd = txtIlgiliBankaAd.Text;
            form.ilgiliBankaNo = txtIlgiliBankaNo.Text;
            form.aciklama = txtAciklama.Text;
            form.islemYapan = kullanan.kullaniciKodu;
            form.projeNo = taahhutProjeNo;

            if (txtFaturaNo.Text != "" && txtFaturaTarih.Text != "")
            {
                form.OEBTarih = new TNSDateTime(txtFaturaTarih.Text);
                form.OEBNo = txtFaturaNo.Text;
                form.OEBTur = "FATURA";
            }

            ObjectArray flerVAN = servisMUH.OdemeEmriMIFTekBelgeListele(kullanan, form);
            if (form.belgeNo != "" && flerVAN.sonuc.islemSonuc && flerVAN.objeler.Count > 0)
            {
                OdemeEmriMIFForm ff = new OdemeEmriMIFForm();
                ff = (OdemeEmriMIFForm)flerVAN.objeler[0];
                if (ff.belgeNo != "")
                {
                    if (ff.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.IPTAL)
                        form.belgeNo = "";
                    else if (!(ff.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.YENI || ff.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.DEGISTIRILDI))
                    {
                        GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTOE015, form.belgeNo));
                        return;
                    }
                    form.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.DEGISTIRILDI;

                    //Eðer oluþturulacak belge daha önce düzenlenmiþ ve þu anda deðiþiklik yapýlýyorsa önceki tutar taahhüt toplamýndan düþmeli
                    foreach (OdemeEmriMIFDetay od in ff.detay.objeler)
                    {
                        if (od.hesapKod.Replace(".", "").StartsWith("921"))
                        {
                            toplamOdemeEmriTutari = toplamOdemeEmriTutari - od.borc;
                            kalanOdemeEmriTutari = kalanOdemeEmriTutari + od.borc;
                        }
                    }

                }
                else
                    form.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YENI;
            }

            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(yil);
            double giderToplamBorc = 0;
            double giderToplamAlacak = 0;
            double gelirToplamBorc = 0;
            double gelirToplamAlacak = 0;

            string tertip830 = "";

            for (int i = 0; i < fpL.Sheets[0].Rows.Count; i++)
            {
                detay = new OdemeEmriMIFDetay(yil);
                detay.kesinti = 0;
                detay.hesapKod = fpL.Sheets[0].Cells[i, 0].Text;

                //////////BTertip
                //if (GenelIslemlerIstemci.MasrafPlaniMi() && yil > 2009)
                //    detay.tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                //else
                //    detay.tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);


                if (fpL.Sheets[0].Cells[i, 3].Text.Trim() != "" || fpL.Sheets[0].Columns[3].Visible == false)
                {
                    string tertip = fpL.Sheets[0].Cells[i, 4].Text.Trim();

                    if (tertip == "")
                    {
                        GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTOE016, (i + 1).ToString()));
                        return;
                    }

                    string[] tler = tertip.Split('-');
                    if (tler.Length == 4)
                        TNS.MUH.Arac.TertipAta(ref detay, tler[0] + "-" + tler[1] + "-" + tler[2] + "-" + tler[3].Replace(".", "").Substring(0, 3));
                    else if (tler.Length > 4)
                        TNS.MUH.Arac.TertipAta(ref detay, tertip);
                    else
                    {
                        GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTOE017, (i + 1).ToString()));
                        return;
                    }
                }
                ////////////////////////////////////

                detay.borc = OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 1].Text);
                detay.alacak = OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 2].Text);

                form.detay.objeler.Add(detay);
            }

            string damgaVergisiHesapKodu = "";
            bool damgaVergisiConfigdenGeldi = false;
            if (chkDamga.Checked && chkDamga.Visible)
            {
                damgaVergisiHesapKodu = System.Configuration.ConfigurationManager.AppSettings.Get("DamgaVergisiHK");
                if (string.IsNullOrEmpty(damgaVergisiHesapKodu))
                    damgaVergisiHesapKodu = "600.01.05.01.01";
                else
                    damgaVergisiConfigdenGeldi = true;

                detay = new OdemeEmriMIFDetay(yil);
                detay.kesinti = 0;
                detay.hesapKod = damgaVergisiHesapKodu;//Damga vergisi
                detay.borc = 0;
                detay.alacak = damgaVergisi;
                form.detay.objeler.Add(detay);
            }

            string tevkifatHesapKod = ddlTevkifat.SelectedValue;
            double tevkifatTutar = OrtakFonksiyonlar.ConvertToDbl(txtTevkifat.Text);
            if (tevkifatHesapKod != "" && tevkifatTutar > 0)
            {
                detay = new OdemeEmriMIFDetay(yil);
                detay.kesinti = 1;
                detay.hesapKod = tevkifatHesapKod;//Tevkifat
                detay.borc = 0;
                detay.alacak = tevkifatTutar;
                form.detay.objeler.Add(detay);
            }
            else
                tevkifatTutar = 0;//tevkifat hesap kodu boþ ise tutarda 0 olsun 

            string kurFarkiHesapKod = txtKurFarkiHesap.Text;
            double kurFarkiTutar = OrtakFonksiyonlar.ConvertToDbl(txtKurFarkiTutar.Text);
            if (kurFarkiHesapKod != "" && kurFarkiTutar > 0)
            {
                detay = new OdemeEmriMIFDetay(yil);
                detay.kesinti = 0;
                detay.hesapKod = kurFarkiHesapKod;//kurFarki

                if (detay.hesapKod.StartsWith("600"))
                {
                    detay.borc = 0;
                    detay.alacak = kurFarkiTutar;
                }
                else
                {
                    detay.borc = kurFarkiTutar;
                    detay.alacak = 0;
                }
                kurFarkiTutar = Math.Abs(detay.borc - detay.alacak);//600 olduðunda netOdenecek'e eklensin 630 olunca çýkartýlsýn
                form.detay.objeler.Add(detay);
            }
            else
                kurFarkiTutar = 0;//kurFarkiTutar hesap kodu boþ ise tutarda 0 olsun 

            //Net ödenecek
            detay = new OdemeEmriMIFDetay(yil);
            detay.kesinti = 0;
            detay.hesapKod = txtHesapKod.Text.Trim();
            detay.altHesapKod = txtAltHesapKod.Text.Trim();
            detay.mahsupBelgeNo = txtMahsupBelgeNo.Text.Trim();
            TNS.MUH.Arac.TertipAta(ref detay, txtOdemeTertip.Text);

            netOdenecek = Math.Round(netOdenecek, 2);
            if (netOdenecek > 0)
            {
                detay.borc = 0;
                detay.alacak = Math.Abs(netOdenecek) - damgaVergisi - tevkifatTutar - kurFarkiTutar;
            }
            else
            {
                detay.borc = Math.Abs(netOdenecek) - damgaVergisi - tevkifatTutar - kurFarkiTutar;
                detay.alacak = 0;
            }
            form.detay.objeler.Add(detay);

            //DDL den Yansýtma olarak seçilen hesaplar iþleniyor
            for (int i = 0; i < fpL.Sheets[0].Rows.Count; i++)
            {
                if (!yansitmaYapilacak)
                    break;

                string deger = fpL.Sheets[0].Cells[i, 3].Text;
                string[] degerler = deger.Split('-');

                if (degerler.Length < 2)
                    continue;

                bool bulundu = false;

                foreach (OdemeEmriMIFDetay d in form.detay.objeler)
                {
                    if (d.hesapKod.Trim().Replace(".", "") == degerler[0].Trim().Replace(".", ""))
                    {
                        d.borc += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 8].Text);
                        d.alacak += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 9].Text);
                        bulundu = true;
                        break;
                    }
                }

                TNS.MUH.HesapPlaniSatir h = new TNS.MUH.HesapPlaniSatir();
                h.yil = yil;
                h.hesapKod = degerler[0].Trim().Replace(".", "");
                h = servisMUH.HesapKodOzellikAl(h);

                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI))
                {
                    giderToplamBorc += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].GetValue(i, 8));
                    giderToplamAlacak += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].GetValue(i, 9));
                }
                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GELIRBUTCESI))
                {
                    gelirToplamBorc += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].GetValue(i, 8));
                    gelirToplamAlacak += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].GetValue(i, 9));
                }

                if (!bulundu)
                {
                    detay = new OdemeEmriMIFDetay(yil);
                    detay.hesapKod = degerler[0].Trim().Replace(".", ""); ;
                    detay.kesinti = 0;

                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI))
                    {
                        string tertip = fpL.Sheets[0].Cells[i, 4].Text.Trim();

                        if (tertip == "")
                        {
                            GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTOE016, (i + 1).ToString()));
                            return;
                        }

                        string[] tler = tertip.Split('-');
                        if (tler.Length == 4)
                        {
                            string tmpHesKod = detay.hesapKod.Replace(".", "").Trim();
                            tmpHesKod = tmpHesKod.Substring(3);
                            if (OrtakFonksiyonlar.ConvertToInt(tmpHesKod.Substring(0, 2), 0) < 9)
                                tmpHesKod = tmpHesKod.Substring(0, 2) + tmpHesKod.Substring(3, 1) + tmpHesKod.Substring(5, 1) + tmpHesKod.Substring(6, 2);

                            TNS.MUH.Arac.TertipAta(ref detay, tler[0] + "-" + tler[1] + "-" + tler[2] + "-" + tmpHesKod);
                            tertip830 = tler[0] + "-" + tler[1] + "-" + tler[2] + "-" + tmpHesKod;
                        }
                        else if (tler.Length > 4)
                        {
                            TNS.MUH.Arac.TertipAta(ref detay, tertip);
                            tertip830 = tertip;
                        }
                        else
                        {
                            GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTOE017, (i + 1).ToString()));
                            return;
                        }
                    }

                    detay.borc += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 8].Text);
                    detay.alacak += OrtakFonksiyonlar.ConvertToDbl(fpL.Sheets[0].Cells[i, 9].Text);

                    form.detay.objeler.Add(detay);
                }
            }

            if (chkDamga.Checked && chkDamga.Visible)
            {
                string damgaVergisiYansitmaHesapKodu = System.Configuration.ConfigurationManager.AppSettings.Get("DamgaVergisiYK");
                if (string.IsNullOrEmpty(damgaVergisiYansitmaHesapKodu) && !damgaVergisiConfigdenGeldi)
                    damgaVergisiYansitmaHesapKodu = "800.01.05.01.01";

                if (damgaVergisiYansitmaHesapKodu.StartsWith("800"))
                {
                    //800
                    detay = new OdemeEmriMIFDetay(yil);
                    detay.kesinti = 0;
                    detay.hesapKod = damgaVergisiYansitmaHesapKodu;
                    detay.borc = 0;
                    detay.alacak = damgaVergisi;
                    form.detay.objeler.Add(detay);
                    gelirToplamAlacak += damgaVergisi;
                }
            }

            //835
            if (yansitmaYapilacak && (giderToplamBorc > 0 || giderToplamAlacak > 0))
            {
                detay = new OdemeEmriMIFDetay(yil);
                detay.kesinti = 0;
                detay.hesapKod = "835";
                if (netOdenecek > 0)
                {
                    detay.borc = 0;
                    detay.alacak = Math.Abs(netOdenecek);
                }
                else
                {
                    detay.borc = Math.Abs(netOdenecek);
                    detay.alacak = 0;
                }
                form.detay.objeler.Add(detay);
            }

            if (yansitmaYapilacak && ((chkDamga.Checked && chkDamga.Visible) || gelirToplamAlacak > 0))
            {
                bool yansitmaVar = false;
                foreach (OdemeEmriMIFDetay d in form.detay.objeler)
                {
                    if (d.hesapKod.StartsWith("800") && gelirToplamAlacak > 0)
                        yansitmaVar = true;
                }

                if (yansitmaVar)
                {
                    //805
                    detay = new OdemeEmriMIFDetay(yil);
                    detay.hesapKod = "805";
                    detay.kesinti = 0;
                    detay.borc = gelirToplamAlacak;
                    detay.alacak = 0;
                    form.detay.objeler.Add(detay);
                }
            }

            if (yansitmaYapilacak && gelirToplamBorc > 0)
            {
                //805
                detay = new OdemeEmriMIFDetay(yil);
                detay.hesapKod = "805";
                detay.kesinti = 0;
                detay.borc = 0;
                detay.alacak = gelirToplamBorc;
                form.detay.objeler.Add(detay);
            }

            int taahhutKod = OrtakFonksiyonlar.ConvertToInt(hdnTaahhutKod.Value, 0);
            if (taahhutKod > 0)
            {
                double tutar = OrtakFonksiyonlar.ConvertToDouble(hdnToplamBorc.Value) - OrtakFonksiyonlar.ConvertToDouble(hdnToplamAlacak.Value);
                tutar = Math.Abs(tutar);
                string hesap920 = "";
                string tertip920 = "";
                string hesap921 = "";
                TNS.MUH.OdemeEmriMIFForm oeTahakkuk = new TNS.MUH.OdemeEmriMIFForm();
                oeTahakkuk.yil = aciliOdemeEmriYil;
                oeTahakkuk.belgeNo = aciliOdemeEmriNo;

                if (Math.Round(kalanOdemeEmriTutari, 2) < Math.Round(tutar, 2))
                {
                    GenelIslemler.MesajKutusu("Uyarý", "Taahhüt Tutarý:" + aciliOdemeEmriTutar.ToString("#,###.00") + "TL,<br>Önceden hazýrlanan Ödeme Emri Tutarý:" + toplamOdemeEmriTutari.ToString("#,###.00") + "TL,<br>Kalan Tutar:" + kalanOdemeEmriTutari.ToString("#,###.00") + "TL,<br>bu bilgiler doðrultusunda " + tutar.ToString("#,###.00") + "TL'lik belge hazýrlanamaz.");
                    return;
                }

                //Eðer açýlýþ taahhuk yapýlmýþ ise
                if (oeTahakkuk.yil > 0)
                {
                    ObjectArray oeTahListe = servisMUH.OdemeEmriMIFListele(kullanan, oeTahakkuk, true);
                    foreach (TNS.MUH.OdemeEmriMIFForm oem in oeTahListe.objeler)
                    {
                        foreach (TNS.MUH.OdemeEmriMIFDetay oed in oem.detay.objeler)
                        {
                            if (oed.hesapKod.StartsWith("920"))
                            {
                                hesap920 = oed.hesapKod;
                                tertip920 = oed.kur + "-" + oed.fon + "-" + oed.fin + "-" + oed.eko;
                                if (oed.peb != "") tertip920 += "-" + oed.peb;
                            }
                            if (oed.hesapKod.StartsWith("921"))
                                hesap921 = oed.hesapKod;
                        }
                    }

                    if (tertip830.Replace(".", "") != tertip920.Replace(".", ""))
                    {
                        GenelIslemler.MesajKutusu("Uyarý", "Taahhüt Tertibi:" + tertip920.Replace(".", "") + " iken " + tertip830.Replace(".", "") + " tertibi kullanýlamaz.");
                        return;
                    }

                    if (hesap920 != "" && hesap921 != "")
                    {
                        detay = new TNS.MUH.OdemeEmriMIFDetay(yil);
                        detay.hesapKod = hesap921;
                        detay.borc = tutar;
                        detay.alacak = 0;
                        form.detay.objeler.Add(detay);

                        detay = new TNS.MUH.OdemeEmriMIFDetay(yil);
                        detay.hesapKod = hesap920;
                        detay.borc = 0;
                        detay.alacak = tutar;
                        detay.altHesapKod = form.ilgiliNo;
                        TNS.MUH.Arac.TertipAta(ref detay, tertip920);
                        form.detay.objeler.Add(detay);
                    }
                }
            }


            Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, form);

            if (sonuc.islemSonuc)
            {
                hdnYil.Value = ddlYil.SelectedValue;
                hdnBelgeNo.Value = sonuc.anahtar;

                tf = new TNS.TMM.TasinirIslemForm();
                tf.yil = yil;
                tf.muhasebeKod = txtMuhasebe.Text.Trim();
                tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
                tf.fisNo = txtBelgeNo.Text.Trim();
                tf.durum = (int)TNS.TMM.ENUMBelgeDurumu.YENI;
                servisTMM.TasinirMIFNoKaydet(kullanan, tf, sonuc.anahtar);

                GenelIslemler.MesajKutusu("Bilgi", string.Format(Resources.TasinirMal.FRMTOE018, sonuc.anahtar));
                ClientScript.RegisterStartupScript(this.GetType(), "goster", "OdemeEmriGoster();", true);
            }
            else
                GenelIslemler.MesajKutusu("Uyarý", string.Format(Resources.TasinirMal.FRMTOE019, sonuc.hataStr));
        }

        /// <summary>
        /// Yeni Ýþlem tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            divEkBilgi.Visible = false;
            divAnaBilgi.Visible = true;

            btnTemizle.Visible = false;
            btnOlustur.Visible = false;
            fpL.Visible = false;

            divListe.Visible = true;
            btnMIFListele.Visible = true;

            fpL.CancelEdit();
            GridInit(fpL);

            ddlYil.SelectedValue = DateTime.Now.Year.ToString();
            txtBelgeNo.Text = string.Empty;
            txtHesapKod.Text = string.Empty;
            txtOdemeTertip.Text = string.Empty;
            hdnBelgeNo.Value = string.Empty;
            hdnMuhasebe.Value = string.Empty;
            hdnBirim.Value = string.Empty;
            hdnTaahhutKod.Value = string.Empty;

            txtIlgiliAd.Text = string.Empty;
            txtIlgiliNo.Text = string.Empty;
            txtIlgiliVD.Text = string.Empty;
            txtIlgiliBankaAd.Text = string.Empty;
            txtIlgiliBankaNo.Text = string.Empty;
            txtFaturaTarih.Text = string.Empty;
            txtFaturaNo.Text = string.Empty;

            txtAciklama.Text = "";
            txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
            txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
            if (txtMuhasebe.Text == "")
                lblMuhasebeAd.Text = string.Empty;
            if (txtHarcamaBirimi.Text == "")
                lblHarcamaBirimiAd.Text = string.Empty;
        }

        /// <summary>
        /// Sayfadaki fgrid grid kontrolünün ilk yükleniþte ayarlanmasýný saðlayan yordam
        /// </summary>
        /// <param name="kontrol">fgrid grid kontrolü</param>
        void GridInit(FarPoint.Web.Spread.FpSpread kontrol)
        {
            kontrol.RenderCSSClass = true;
            kontrol.EditModeReplace = false;

            kontrol.Sheets.Count = 1;
            kontrol.CommandBar.Visible = false;

            kontrol.Sheets[0].RowCount = 0;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 10;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMTOE020;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 1].Value = Resources.TasinirMal.FRMTOE021;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMTOE022;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMTOE023;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].ColumnSpan = 2;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMTOE024;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMTOE025;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMTOE026;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMTOE027;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 9].Value = Resources.TasinirMal.FRMTOE028;

            kontrol.Sheets[0].Columns[0].Width = 80;
            kontrol.Sheets[0].Columns[1].Width = 80;
            kontrol.Sheets[0].Columns[2].Width = 80;
            kontrol.Sheets[0].Columns[3].Width = 250;
            kontrol.Sheets[0].Columns[4].Width = 190;
            kontrol.Sheets[0].Columns[5].Width = 30;
            kontrol.Sheets[0].Columns[6].Width = 250;
            kontrol.Sheets[0].Columns[7, 9].Visible = false;

            kontrol.Sheets[0].Columns[1, 2].HorizontalAlign = HorizontalAlign.Right;

            kontrol.Sheets[0].Columns[0].Locked = true; //Hesap Kod
            kontrol.Sheets[0].Columns[1].Locked = true; //Borç
            kontrol.Sheets[0].Columns[2].Locked = true; //Alacak
            kontrol.Sheets[0].Columns[6].Locked = true; //Açýklama
            kontrol.Sheets[0].Columns[0].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[1].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[2].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[6].BackColor = System.Drawing.Color.LightGoldenrodYellow;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;

            TasinirGenel.MyLinkType tertipLink = new TasinirGenel.MyLinkType("TertipGoster()");
            tertipLink.ImageUrl = "../App_themes/images/bul1.gif";

            kontrol.Sheets[0].Columns[5].CellType = tertipLink;

            GenelIslemlerIstemci.RakamAlanFormatla(kontrol, 1, 1, 2);
        }

        /// <summary>
        /// Sayfadaki grid kontrolünün format bilgilerini sessiona saklayan ya da okuyan yordam
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
        /// Sayfadaki ddlYil DropDownList kontrolüne yýl bilgileri doldurulur.
        /// </summary>
        private void YilDoldur()
        {
            GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);
        }

        private void TevkifatDoldur()
        {
            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            TNS.MUH.HesapPlaniSatir hs = new TNS.MUH.HesapPlaniSatir();
            hs.yil = DateTime.Now.Year;
            hs.hesapKod = "360;361";
            hs.detay = true;
            ObjectArray hv = servisMUH.HesapPlaniListele(kullanan, hs);

            ddlTevkifat.Items.Add(new ListItem("", ""));

            foreach (TNS.MUH.HesapPlaniSatir h in hv.objeler)
            {
                string hesapAd = h.aciklama;
                if (hesapAd.Length > 55)
                    hesapAd = hesapAd.Substring(0, 55) + "...";
                ddlTevkifat.Items.Add(new ListItem(h.hesapKod + "-" + hesapAd, h.hesapKod));
            }
        }


        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriterleri taþýnýr iþlem form nesnesine doldurulur, sunucuya gönderilir
        /// ve taþýnýr iþlem fiþlerine ait ödeme emri bilgileri sunucudan alýnýr. Hata varsa ekrana
        /// hata bilgisi yazýlýr, yoksa gelen bilgiler gvListe GridView kontrolüne doldurulur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnMIFListele_Click(object sender, EventArgs e)
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();

            ObjectArray bilgi = servisTMM.TasinirMIFNoListele(kullanan, tf);

            if (bilgi.sonuc.islemSonuc)
            {
                if (bilgi.objeler.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("yil");
                    dt.Columns.Add("muhasebe");
                    dt.Columns.Add("harcamaBirim");
                    dt.Columns.Add("fisNo");
                    dt.Columns.Add("belgeNo");

                    foreach (TasinirIslemMIF o in bilgi.objeler)
                    {
                        if (o.durum < (int)TNS.TMM.ENUMBelgeDurumu.IPTAL)
                            dt.Rows.Add(o.yil, o.muhasebeKod, o.harcamaKod, o.tifBelgeNo, "<a href='#' onclick=\"OdemeEmriLinktenGoster('" + o.yil + "','" + o.mifBelgeNo + "');\">" + o.mifBelgeNo + "</a>");
                    }
                    gvListe.DataSource = dt;
                    gvListe.DataBind();

                    for (int i = 0; i < gvListe.Rows.Count; i++)
                        gvListe.Rows[i].Cells[4].Text = Server.HtmlDecode(gvListe.Rows[i].Cells[4].Text);
                }
                else
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMTOE029);
            }
            else
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Teminatlars the listele.
        /// </summary>
        [Ext1.Net.DirectMethod]
        public void TaahhutDosyasiDoldur()
        {
            List<object> liste = new List<object>();

            TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
            bilgi.muteahhitKimlikNo = txtIlgiliNo.Text;
            ObjectArray tListe = servisMUH.TaahhutKartiListele(kullanan, bilgi, 0);
            if (tListe != null)
            {
                liste.Add(new
                {
                    KOD = -1,
                    PROJENO = "",
                    KONU = "Taahhüt dosyasý dahilinde yapýlan iþ deðil",
                    SOZLESMETARIH = "",
                    IHALETUTAR = "0",
                });

                foreach (TaahhutKartiBilgi t in tListe.objeler)
                {
                    double ihaleBedeli = t.ihaleBedeli;
                    foreach (TNS.MUH.TaahhutKartiDegerArtisi da in t.degerArtisi.objeler)
                    {
                        ihaleBedeli += da.sozlesmeTutari;
                    }

                    liste.Add(new
                    {
                        KOD = t.kod,
                        PROJENO = t.projeSiraNo,
                        KONU = t.konu,
                        SOZLESMETARIH = t.sozlesmeTarihi.ToString(),
                        IHALETUTAR = string.Format("{0:n2}", ihaleBedeli),

                    });
                }
            }
            storeTaahhut.DataSource = liste;
            storeTaahhut.DataBind();
        }

        //private string EkonomikKodaDonustur(int yil, string hesapKod)
        //{
        //    Tertip hesapT = new Tertip(yil, ENUMTabloKod.TABOdemeEmri);
        //    string kirilimHesap = hesapT.Kirilimlar(0).Replace(".", "");
        //    hesapKod = hesapKod.Replace(".", "");
        //    hesapKod = hesapKod.PadRight(kirilimHesap.Length, '0');

        //    Tertip butceT = new Tertip(yil, ENUMTabloKod.TABOdenek);
        //    string[] kirilimlarEko = butceT.Kirilimlar(3).Split('.');

        //    hesapT.TertipAta(hesapKod);

        //    string[] kirilimlarHesap = hesapT.Kirilimlar(0).Split('.');
        //    string[] hParca = new string[kirilimlarHesap.Length];
        //    int ekoSayac = 0;
        //    string eko = "";

        //    for (int i = 0; hesapKod != ""; i++)
        //    {
        //        hParca[i] = hesapKod.Substring(0, kirilimlarHesap[i].Length);
        //        hesapKod = hesapKod.Substring(kirilimlarHesap[i].Length);

        //        if (OrtakFonksiyonlar.ConvertToInt(hParca[i], 0) > 0 && i > 0)
        //        {
        //            ekoSayac++;
        //            if (kirilimlarEko.Length >= ekoSayac)
        //            {
        //                string bilgi = "";
        //                if (kirilimlarEko[ekoSayac - 1].Length == hParca[i].Length)
        //                    bilgi = hParca[i];
        //                else
        //                    bilgi = hParca[i].Substring(1);

        //                eko += bilgi;
        //            }
        //        }
        //    }

        //    return eko;
        //}
    }
}