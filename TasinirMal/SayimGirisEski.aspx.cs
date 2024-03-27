using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections;

namespace TasinirMal
{
    /// <summary>
    /// Sayým tutanaðý bilgilerinin kayýt, listeleme, silme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class SayimGirisEski : TMMSayfa
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
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama, ambar ve sayimNo
        ///     girdi dizgileri dolu ise ilgili sayým tutanaðý bilgileri listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMSYG001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                GenelIslemler.SayfayaGirmesin(true);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            this.btnTemizle.Attributes.Add("onclick", "return OnayAl('Temizle','btnTemizle');");
            this.btnSil.Attributes.Add("onclick", "return OnayAl('Sil','btnSil');");

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

                GridInit(fpL);
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year, DateTime.Now.Year);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");
                txtAmbar.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR");
                txtSayimTarihi.Value = DateTime.Now.ToShortDateString();

                if (!string.IsNullOrEmpty(Request.QueryString["yil"])
                     && !string.IsNullOrEmpty(Request.QueryString["muhasebe"])
                     && !string.IsNullOrEmpty(Request.QueryString["harcama"])
                     && !string.IsNullOrEmpty(Request.QueryString["ambar"])
                     && !string.IsNullOrEmpty(Request.QueryString["sayimNo"]))
                {
                    ddlYil.SelectedValue = Request.QueryString["yil"];
                    txtMuhasebe.Text = Request.QueryString["muhasebe"];
                    txtHarcamaBirimi.Text = Request.QueryString["harcama"];
                    txtAmbar.Text = Request.QueryString["ambar"];
                    hdnSayimNo.Value = Request.QueryString["sayimNo"];

                    ButonlariAktifYap();
                    Listele(KriterTopla());
                }
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
        }

        /// <summary>
        /// Listeleme kriterleri sayým form nesnesinde parametre olarak alýnýr,
        /// sunucuya gönderilir ve sayým tutanaðý bilgileri sunucudan alýnýr.
        /// Hata varsa ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="sf">Sayým tutanaðý listeleme kriterlerini tutan nesne</param>
        private void Listele(SayimForm sf)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            ObjectArray bilgi = servisTMM.SayimListele(kullanan, sf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            btnTemizle_Click(null, null);

            SayimForm sform = (SayimForm)bilgi.objeler[0];
            ddlYil.SelectedValue = sform.yil.ToString();
            txtMuhasebe.Text = sform.muhasebeKod;
            lblMuhasebeAd.Text = sform.muhasebeAd;
            txtHarcamaBirimi.Text = sform.harcamaKod;
            lblHarcamaBirimiAd.Text = sform.harcamaAd;
            txtAmbar.Text = sform.ambarKod;
            lblAmbarAd.Text = sform.ambarAd;
            txtSayimTarihi.Value = sform.sayimTarih.ToString();
            hdnSayimNo.Value = sform.sayimNo;

            if (sform.detay.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = sform.detay.Count + 10;

            int i = 0;
            foreach (SayimDetay sd in sform.detay)
            {
                fpL.Sheets[0].Cells[i, 0].Text = sd.hesapPlanKod;
                fpL.Sheets[0].Cells[i, 2].Text = sd.hesapPlanAd;
                fpL.Sheets[0].Cells[i, 3].Text = sd.olcuBirimAd;
                fpL.Sheets[0].Cells[i, 4].Text = sd.ambarMiktar.ToString();
                fpL.Sheets[0].Cells[i, 5].Text = sd.ortakMiktar.ToString();
                fpL.Sheets[0].Cells[i, 6].Text = sd.kayitKisiMiktar.ToString();
                fpL.Sheets[0].Cells[i, 7].Text = sd.aciklama;
                i++;
            }
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Sayým tutanaðý bilgilerini ekrandaki ilgili kontrollerden toplayan yordam çaðýrýlýr
        /// ve daha sonra toplanan bilgiler kaydedilmek üzere Kaydet yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
            Kaydet(KriterTopla());
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Sayým tutanaðý bilgilerini ekrandaki ilgili kontrollerden toplayan yordam çaðýrýlýr
        /// ve daha sonra toplanan bilgiler silinmek üzere Sil yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, EventArgs e)
        {
            Sil(KriterTopla());
        }

        /// <summary>
        /// Ambardakileri Aktar tuþuna basýlýnca çalýþan olay metodu
        /// Listeleme kriter bilgilerini ekrandaki ilgili kontrollerden toplayan yordam çaðýrýlýr ve
        /// daha sonra toplanan kriterler ambardaki malzemeleri listeleyen AmbarAktar yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAmbarAktar_Click(object sender, EventArgs e)
        {
            AmbarAktar(KriterTopla(), "LISTELE");
        }

        protected void btnAmbarAktarKaydet_Click(object sender, EventArgs e)
        {
            AmbarAktar(KriterTopla(), "KAYDET");
        }

        /// <summary>
        /// Listeleme kriterleri sayým form nesnesinde parametre olarak alýnýr,
        /// sunucuya gönderilir ve ambardaki taþýnýr malzemeleri sunucudan alýnýr.
        /// Hata varsa ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="kriter">Listeleme kriterlerini tutan nesne</param>
        /// <param name="islem">The islem.</param>
        private void AmbarAktar(SayimForm kriter, string islem)
        {
            SayimForm sf = new SayimForm();
            if (islem == "KAYDET")
            {
                sf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
                sf.muhasebeKod = txtMuhasebe.Text.Trim();
                sf.harcamaKod = txtHarcamaBirimi.Text.Trim();
                sf.ambarKod = txtAmbar.Text.Trim();
                sf.sayimNo = hdnSayimNo.Value.Trim();
                sf.sayimTarih = new TNSDateTime(txtSayimTarihi.Value.Trim());
            }

            try
            {
                string sayimNo = hdnSayimNo.Value.Trim();
                btnTemizle_Click(null, null);
                hdnSayimNo.Value = sayimNo;

                ObjectArray bilgi = servisTMM.SayimAmbardakileriGetir(kullanan, kriter);

                if (!bilgi.sonuc.islemSonuc)
                {
                    GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                    return;
                }

                if (bilgi.objeler.Count <= 0)
                {
                    GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                    return;
                }

                int i = 0;
                foreach (SayimDetay sd in bilgi.objeler)
                {
                    if (sd.kayitAmbarMiktar > 0 || sd.kayitOrtakMiktar > 0 || sd.kayitKisiMiktar > 0)
                    {
                        if (islem == "LISTELE")
                        {
                            fpL.Sheets[0].RowCount++;
                            fpL.Sheets[0].Cells[i, 0].Text = sd.hesapPlanKod;
                            fpL.Sheets[0].Cells[i, 2].Text = sd.hesapPlanAd;
                            fpL.Sheets[0].Cells[i, 3].Text = sd.olcuBirimAd;
                            fpL.Sheets[0].Cells[i, 4].Text = sd.kayitAmbarMiktar.ToString();
                            fpL.Sheets[0].Cells[i, 5].Text = sd.kayitOrtakMiktar.ToString();
                            fpL.Sheets[0].Cells[i, 6].Text = sd.kayitKisiMiktar.ToString();
                            i++;
                        }
                        else if (islem == "KAYDET")
                        {
                            SayimDetay detay = new SayimDetay();
                            detay.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
                            detay.muhasebeKod = txtMuhasebe.Text.Trim();
                            detay.harcamaKod = txtHarcamaBirimi.Text.Trim();
                            detay.ambarKod = txtAmbar.Text.Trim();
                            detay.sayimNo = hdnSayimNo.Value.Trim();
                            detay.hesapPlanKod = sd.hesapPlanKod;
                            detay.ambarMiktar = sd.kayitAmbarMiktar;
                            detay.ortakMiktar = sd.kayitOrtakMiktar;

                            sf.detay.Add(detay);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu(this, ex.ToString());
            }

            if (islem == "KAYDET")
            {
                Sonuc sonuc = servisTMM.SayimKaydet(kullanan, sf);
                if (!sonuc.islemSonuc)
                    GenelIslemler.HataYaz(this, sonuc.hataStr);
                else
                {
                    hdnSayimNo.Value = sonuc.anahtar;
                    ButonlariAktifYap();
                    Listele(KriterTopla());
                    GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMSYG017);
                }
            }
        }

        /// <summary>
        /// Noksandan TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Sayýmda eksik olduðu bulunan taþýnýr malzemeleri için taþýnýr
        /// iþlem fiþi oluþturmak üzere TIFOlustur yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIFNoksan_Click(object sender, EventArgs e)
        {
            TIFOlustur(true);
        }

        /// <summary>
        /// Fazladan TÝF Oluþtur tuþuna basýlýnca çalýþan olay metodu
        /// Sayýmda fazla olduðu bulunan taþýnýr malzemeleri için taþýnýr
        /// iþlem fiþi oluþturmak üzere TIFOlustur yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTIFFazla_Click(object sender, EventArgs e)
        {
            TIFOlustur(false);
        }

        /// <summary>
        /// Sayým tutanaðý ile ambardaki malzemeler arasýnda miktar farklýlýklarýný arar.
        /// Eðer farklýlýk varsa ilgili malzeme bilgilerini sessiona yazar ve taþýnýr iþlem
        /// fiþi ekranýna yönlendirir, yoksa farklýlýk olmadýðýna iliþkin hata mesajý verir.
        /// </summary>
        /// <param name="noksanMi">Eksik miktar farklýlýklarý mý, yoksa fazla miktar farklýlýklarý mý aranacak bilgisi</param>
        private void TIFOlustur(bool noksanMi)
        {
            SayimForm sf = KriterTopla();
            if (string.IsNullOrEmpty(sf.sayimNo))
            {
                GenelIslemler.HataYaz(this, Resources.TasinirMal.FRMSYG002);
                return;
            }

            ObjectArray bilgi = servisTMM.SayimRaporListele(kullanan, sf, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            bool eksikVarMi = false;

            SayimForm sForm = (SayimForm)bilgi.objeler[0];

            foreach (SayimDetay sd in sForm.detay)
                if (noksanMi)
                    sd.fazlaMiktar = 0;
                else
                    sd.noksanMiktar = 0;

            for (int i = 0; i < sForm.detay.Count; i++)
            {
                SayimDetay sd = (SayimDetay)sForm.detay[i];

                if ((sd.fazlaMiktar > 0 || sd.noksanMiktar > 0) && !eksikVarMi)
                    eksikVarMi = true;
                else if (sd.fazlaMiktar <= 0 && sd.noksanMiktar <= 0)
                {
                    sForm.detay.RemoveAt(i);
                    i--;
                }
            }

            if (eksikVarMi)
            {
                Session.Add("SayimdanTIF", sForm);
                Response.Redirect("TasinirIslemFormAna.aspx");
            }
            else
            {
                string aciklama = Resources.TasinirMal.FRMSYG003;
                if (noksanMi)
                    aciklama = Resources.TasinirMal.FRMSYG004;

                GenelIslemler.HataYaz(this, aciklama);
            }
        }

        /// <summary>
        /// Sayým Tutanaðý tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan
        /// kriterler SayimTutanakYaz yordamýna gönderilir ve rapor hazýrlanmýþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSayimTutanak_Click(object sender, EventArgs e)
        {
            SayimTutanakYaz(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen sayým formuna ait kriterleri
        /// sunucudaki sayým tutanaðý raporlama yordamýna gönderir,
        /// sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="sForm">Sayým tutanaðý kriter bilgilerini tutan nesne</param>
        private void SayimTutanakYaz(SayimForm sForm)
        {
            ObjectArray bilgi = servisTMM.SayimRaporListele(kullanan, sForm, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "SAYIMTUTANAK.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            string eskiHesapKod = string.Empty;

            SayimForm sf = (SayimForm)bilgi.objeler[0];

            for (int i = 0; i < sf.detay.Count; i++)
            {
                SayimDetay sd = (SayimDetay)sf.detay[i];

                if (!sd.hesapPlanKod.Contains(eskiHesapKod) || string.IsNullOrEmpty(eskiHesapKod))
                {
                    if (sd.hesapPlanKod.Length >= 9)
                        eskiHesapKod = sd.hesapPlanKod.Substring(0, 9);
                    else
                        eskiHesapKod = sd.hesapPlanKod.Substring(0, sd.hesapPlanKod.Length);

                    if (i != 0)
                    {
                        SayimTutanakImzaEkle(XLS, ref satir, sutun);

                        XLS.SayfaSonuKoyHucresel(satir + 3);
                        satir += 3;
                        XLS.SatirYukseklikAyarla(satir - 2, satir, GenelIslemler.JexcelBirimtoExcelBirim(315));
                    }

                    XLS.SatirAc(satir, 7);
                    XLS.SatirYukseklikAyarla(satir, satir + 5, GenelIslemler.JexcelBirimtoExcelBirim(315));
                    XLS.SatirYukseklikAyarla(satir + 6, satir + 6, GenelIslemler.JexcelBirimtoExcelBirim(1000));
                    XLS.HucreKopyala(kaynakSatir - 7, sutun, kaynakSatir - 1, sutun + 12, satir, sutun);

                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 12);

                    for (int j = satir + 2; j <= satir + 4; j++)
                        XLS.HucreBirlestir(j, sutun + 2, j, sutun + 8);

                    //XLS.HucreBirlestir(satir + 3, sutun + 11, satir + 5, sutun + 11);
                    //XLS.HucreBirlestir(satir + 3, sutun + 12, satir + 5, sutun + 12);

                    XLS.HucreBirlestir(satir + 6, sutun + 1, satir + 6, sutun + 3);

                    XLS.HucreDegerYaz(satir + 2, sutun + 2, sf.ilAd + "-" + sf.ilceAd);
                    XLS.HucreDegerYaz(satir + 2, sutun + 10, sf.ilKod + "-" + sf.ilceKod);
                    XLS.HucreDegerYaz(satir + 2, sutun + 12, sf.yil);
                    XLS.HucreDegerYaz(satir + 3, sutun + 2, sf.harcamaAd);
                    XLS.HucreDegerYaz(satir + 3, sutun + 10, sf.harcamaKod);
                    XLS.HucreDegerYaz(satir + 3, sutun + 12, eskiHesapKod);
                    XLS.HucreDegerYaz(satir + 4, sutun + 2, sf.ambarAd);
                    XLS.HucreDegerYaz(satir + 4, sutun + 10, sf.ambarKod);
                    //XLS.HucreDegerYaz(satir + 5, sutun + 2, sf.muhasebeAd);
                    //XLS.HucreDegerYaz(satir + 5, sutun + 10, sf.muhasebeKod);

                    satir += 6;
                }

                if (sd.kayitAmbarMiktar > 0 || sd.ambarMiktar > 0 ||
                    sd.kayitOrtakMiktar > 0 || sd.ortakMiktar > 0 || sd.kayitKisiMiktar > 0 ||
                    sd.fazlaMiktar > 0 || sd.noksanMiktar > 0)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 3);
                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(315));

                    XLS.HucreDegerYaz(satir, sutun, sd.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, sd.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 4, sd.olcuBirimAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(sd.kayitAmbarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(sd.ambarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(sd.kayitOrtakMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(sd.ortakMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(sd.kayitKisiMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(sd.fazlaMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(sd.noksanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 12, sd.aciklama);

                    if (sd.kayitAmbarMiktar != sd.ambarMiktar)
                        XLS.ArkaPlanRenk(satir, sutun + 5, satir, sutun + 6, OrtakClass.TabloRenk.VERY_LIGHT_YELLOW);
                    if (sd.kayitOrtakMiktar != sd.ortakMiktar)
                        XLS.ArkaPlanRenk(satir, sutun + 7, satir, sutun + 8, OrtakClass.TabloRenk.VERY_LIGHT_YELLOW);
                    if (sd.fazlaMiktar > 0)
                        XLS.ArkaPlanRenk(satir, sutun + 10, OrtakClass.TabloRenk.PALE_BLUE);
                    if (sd.noksanMiktar > 0)
                        XLS.ArkaPlanRenk(satir, sutun + 11, OrtakClass.TabloRenk.PALE_BLUE);
                }
            }
            SayimTutanakImzaEkle(XLS, ref satir, sutun);
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Sayým tutanaðý excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        private void SayimTutanakImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 2;

            ObjectArray imza1 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.SAYIMKURULUBASKANI);
            ImzaBilgisi i1 = null;
            if (imza1.sonuc.islemSonuc && imza1.objeler.Count > 0)
                i1 = (ImzaBilgisi)imza1.objeler[0];
            string ad1 = string.Empty;
            string unvan1 = string.Empty;
            if (i1 != null)
            {
                ad1 = i1.adSoyad;
                unvan1 = i1.unvan;
            }

            ObjectArray imza2 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.SAYIMKURULUUYE1);
            ImzaBilgisi i2 = null;
            if (imza2.sonuc.islemSonuc && imza2.objeler.Count > 0)
                i2 = (ImzaBilgisi)imza2.objeler[0];
            string ad2 = string.Empty;
            string unvan2 = string.Empty;
            if (i2 != null)
            {
                ad2 = i2.adSoyad;
                unvan2 = i2.unvan;
            }

            ObjectArray imza3 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.SAYIMKURULUUYE2);
            ImzaBilgisi i3 = null;
            if (imza3.sonuc.islemSonuc && imza3.objeler.Count > 0)
                i3 = (ImzaBilgisi)imza3.objeler[0];
            string ad3 = string.Empty;
            string unvan3 = string.Empty;
            if (i3 != null)
            {
                ad3 = i3.adSoyad;
                unvan3 = i3.unvan;
            }

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 6, sutun + 12, satir, sutun);

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 12);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMSYG005);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 2);
            XLS.YatayHizala(satir, sutun, 2);

            XLS.HucreDegerYaz(satir + 1, sutun + 1, Resources.TasinirMal.FRMSYG006);
            XLS.HucreDegerYaz(satir + 1, sutun + 5, Resources.TasinirMal.FRMSYG007);
            XLS.HucreDegerYaz(satir + 1, sutun + 9, Resources.TasinirMal.FRMSYG007);

            XLS.HucreDegerYaz(satir + 2, sutun + 1, string.Format(Resources.TasinirMal.FRMSYG008, ad1));
            XLS.HucreDegerYaz(satir + 3, sutun + 1, string.Format(Resources.TasinirMal.FRMSYG009, unvan1));
            XLS.HucreDegerYaz(satir + 4, sutun + 1, Resources.TasinirMal.FRMSYG010);

            XLS.HucreDegerYaz(satir + 2, sutun + 5, ad2);
            XLS.HucreDegerYaz(satir + 3, sutun + 5, unvan2);
            //XLS.HucreDegerYaz(satir + 4, sutun + 5, "Ýmzasý : ");

            XLS.HucreDegerYaz(satir + 2, sutun + 9, ad3);
            XLS.HucreDegerYaz(satir + 3, sutun + 9, unvan3);
            //XLS.HucreDegerYaz(satir + 4, sutun + 9, "Ýmzasý : ");

            for (int i = satir + 1; i < satir + 5; i++)
            {
                for (int j = sutun + 1; j <= sutun + 9; j += 4)
                {
                    XLS.HucreBirlestir(i, j, i, j + 2);

                    if (i == satir + 1)
                    {
                        XLS.KoyuYap(i, j, true);
                        XLS.DuseyHizala(i, j, 2);
                        XLS.YatayHizala(i, j, 2);
                    }
                }
            }

            XLS.YatayCizgiCiz(satir, sutun, sutun + 12, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 1, sutun, sutun + 12, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 12, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 13, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.SatirYukseklikAyarla(satir, satir + 5, GenelIslemler.JexcelBirimtoExcelBirim(315));

            satir += 5;
        }

        /// <summary>
        /// Ambar Devir ve Teslim Tutanaðý tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan kriterler
        /// AmbarDevirTutanakYaz yordamýna gönderilir ve rapor hazýrlanmýþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAmbarDevirTutanak_Click(object sender, EventArgs e)
        {
            AmbarDevirTutanakYaz(KriterTopla());
        }

        /// <summary>
        /// Parametre olarak verilen sayým formuna ait kriterleri
        /// sunucudaki sayým tutanaðý raporlama yordamýna gönderir,
        /// sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="sForm">Ambar devir ve teslim tutanaðý kriter bilgilerini tutan nesne</param>
        private void AmbarDevirTutanakYaz(SayimForm sForm)
        {
            ObjectArray bilgi = servisTMM.SayimRaporListele(kullanan, sForm, true);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AMBARDEVIRTESLIMTUTANAK.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            SayimForm sf = (SayimForm)bilgi.objeler[0];
            XLS.HucreAdBulYaz("IlAd", sf.ilAd + "-" + sf.ilceAd);
            XLS.HucreAdBulYaz("IlKod", sf.ilKod + "-" + sf.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", sf.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", sf.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", sf.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", sf.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", sf.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", sf.muhasebeKod);

            satir = kaynakSatir;

            int sayac = 0;
            for (int i = 0; i < sf.detay.Count; i++)
            {
                SayimDetay sd = (SayimDetay)sf.detay[i];

                if (sd.ambarMiktar > 0 || sd.kayitAmbarMiktar > 0 ||
                    sd.fazlaMiktar > 0 || sd.noksanMiktar > 0)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                    sayac++;
                    XLS.HucreDegerYaz(satir, sutun, sayac.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 1, sd.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, sd.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, sd.olcuBirimAd);
                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(sd.ambarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(sd.kayitAmbarMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(sd.fazlaMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(sd.noksanMiktar.ToString(), (double)0));
                }
            }

            AmbarDevirTutanakImzaEkle(XLS, ref satir, sutun);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Ambar devir ve teslim tutanaðý excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        private void AmbarDevirTutanakImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 2;

            ObjectArray imza1 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUBASKANI);
            ImzaBilgisi i1 = null;
            if (imza1.sonuc.islemSonuc && imza1.objeler.Count > 0)
                i1 = (ImzaBilgisi)imza1.objeler[0];
            string ad1 = string.Empty;
            string unvan1 = string.Empty;
            if (i1 != null)
            {
                ad1 = i1.adSoyad;
                unvan1 = i1.unvan;
            }

            ObjectArray imza2 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUUYE1);
            ImzaBilgisi i2 = null;
            if (imza2.sonuc.islemSonuc && imza2.objeler.Count > 0)
                i2 = (ImzaBilgisi)imza2.objeler[0];
            string ad2 = string.Empty;
            string unvan2 = string.Empty;
            if (i2 != null)
            {
                ad2 = i2.adSoyad;
                unvan2 = i2.unvan;
            }

            ObjectArray imza3 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.AMBARDEVIRVETESLIMKURULUUYE2);
            ImzaBilgisi i3 = null;
            if (imza3.sonuc.islemSonuc && imza3.objeler.Count > 0)
                i3 = (ImzaBilgisi)imza3.objeler[0];
            string ad3 = string.Empty;
            string unvan3 = string.Empty;
            if (i3 != null)
            {
                ad3 = i3.adSoyad;
                unvan3 = i3.unvan;
            }

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 6, sutun + 9, satir, sutun);

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMSYG011);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 2);
            XLS.YatayHizala(satir, sutun, 2);

            XLS.HucreDegerYaz(satir + 1, sutun, Resources.TasinirMal.FRMSYG006);
            XLS.HucreDegerYaz(satir + 1, sutun + 2, Resources.TasinirMal.FRMSYG007);
            XLS.HucreDegerYaz(satir + 1, sutun + 4, Resources.TasinirMal.FRMSYG007);
            XLS.HucreDegerYaz(satir + 1, sutun + 6, Resources.TasinirMal.FRMSYG012);
            XLS.HucreDegerYaz(satir + 1, sutun + 8, Resources.TasinirMal.FRMSYG013);

            XLS.HucreDegerYaz(satir + 2, sutun, string.Format(Resources.TasinirMal.FRMSYG008, ad1));
            XLS.HucreDegerYaz(satir + 3, sutun, string.Format(Resources.TasinirMal.FRMSYG009, unvan1));
            XLS.HucreDegerYaz(satir + 4, sutun, Resources.TasinirMal.FRMSYG010);

            XLS.HucreDegerYaz(satir + 2, sutun + 2, ad2);
            XLS.HucreDegerYaz(satir + 3, sutun + 2, unvan2);
            //XLS.HucreDegerYaz(satir + 4, sutun + 5, "Ýmzasý : ");

            XLS.HucreDegerYaz(satir + 2, sutun + 4, ad3);
            XLS.HucreDegerYaz(satir + 3, sutun + 4, unvan3);
            //XLS.HucreDegerYaz(satir + 4, sutun + 9, "Ýmzasý : ");

            XLS.HucreDegerYaz(satir + 2, sutun + 6, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 3, sutun + 6, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 4, sutun + 6, Resources.TasinirMal.FRMSYG014);

            XLS.HucreDegerYaz(satir + 2, sutun + 8, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 3, sutun + 8, Resources.TasinirMal.FRMSYG014);
            XLS.HucreDegerYaz(satir + 4, sutun + 8, Resources.TasinirMal.FRMSYG014);

            XLS.HucreDegerYaz(satir + 5, sutun + 2, string.Format(Resources.TasinirMal.FRMSYG015, DateTime.Today.Date.ToShortDateString()));
            XLS.HucreBirlestir(satir + 5, sutun + 2, satir + 5, sutun + 3);
            XLS.DuseyHizala(satir + 5, sutun + 2, 2);

            XLS.HucreDegerYaz(satir + 5, sutun + 7, string.Format(Resources.TasinirMal.FRMSYG015, DateTime.Today.Date.ToShortDateString()));
            XLS.HucreBirlestir(satir + 5, sutun + 7, satir + 5, sutun + 8);
            XLS.DuseyHizala(satir + 5, sutun + 7, 2);

            for (int i = satir + 1; i < satir + 5; i++)
            {
                for (int j = sutun; j <= sutun + 8; j += 2)
                {
                    XLS.HucreBirlestir(i, j, i, j + 1);

                    if (i == satir + 1)
                    {
                        XLS.KoyuYap(i, j, true);
                        XLS.DuseyHizala(i, j, 2);
                        XLS.YatayHizala(i, j, 2);
                    }
                }
            }

            XLS.YatayCizgiCiz(satir, sutun, sutun + 9, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 1, sutun, sutun + 5, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 9, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 6, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 10, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 5;
        }

        /// <summary>
        /// Sayfadaki buton kontrollerini aktif hale getiren yordam
        /// </summary>
        private void ButonlariAktifYap()
        {
            btnSil.Enabled = true;
            btnSayimTutanak.Enabled = true;
            btnAmbarDevirTutanak.Enabled = true;
            btnTIFNoksan.Enabled = true;
            btnTIFFazla.Enabled = true;
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden sayým tutanaðý bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Sayým tutanaðý bilgileri döndürülür.</returns>
        private SayimForm KriterTopla()
        {
            SayimForm sf = new SayimForm();
            sf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            sf.muhasebeKod = txtMuhasebe.Text.Trim();
            sf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            sf.ambarKod = txtAmbar.Text.Trim();
            sf.sayimNo = hdnSayimNo.Value.Trim();
            sf.sayimTarih = new TNSDateTime(txtSayimTarihi.Value.Trim());
            return sf;
        }

        /// <summary>
        /// Parametre olarak verilen sayým tutanaðý üst bilgilerine detay bilgileri ekleyip
        /// kaydedilmek üzere sunucuya gönderir ve iþlem sonucunu ekranda görüntüler.
        /// </summary>
        /// <param name="sf">Sayým tutanaðý bilgilerini tutan nesne</param>
        private void Kaydet(SayimForm sf)
        {
            fpL.SaveChanges();

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                SayimDetay detay = new SayimDetay();
                detay.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
                detay.muhasebeKod = txtMuhasebe.Text.Trim();
                detay.harcamaKod = txtHarcamaBirimi.Text.Trim();
                detay.ambarKod = txtAmbar.Text.Trim();
                detay.sayimNo = hdnSayimNo.Value.Trim();

                detay.hesapPlanKod = fpL.Sheets[0].Cells[i, 0].Text.Trim();
                detay.ambarMiktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 4].Text.Trim(), 0);
                detay.ortakMiktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 5].Text.Trim(), 0);
                detay.aciklama = fpL.Sheets[0].Cells[i, 7].Text.Trim();

                if (!string.IsNullOrEmpty(detay.hesapPlanKod) && (detay.ambarMiktar < 0 || detay.ortakMiktar < 0))
                {
                    GenelIslemler.HataYaz(this, string.Format(Resources.TasinirMal.FRMSYG016, (i + 1).ToString()));
                    return;
                }

                if (!string.IsNullOrEmpty(detay.hesapPlanKod))
                    sf.detay.Add(detay);
            }

            Sonuc sonuc = servisTMM.SayimKaydet(kullanan, sf);
            if (!sonuc.islemSonuc)
                GenelIslemler.HataYaz(this, sonuc.hataStr);
            else
            {
                hdnSayimNo.Value = sonuc.anahtar;
                ButonlariAktifYap();
                Listele(KriterTopla());
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMSYG017);
            }
        }

        /// <summary>
        /// Parametre olarak verilen sayým tutanaðýnýn bilgilerini silen sunucu
        /// yordamýný çaðýrýr ve dönen sonuca göre hata veya bilgi mesajý verir.
        /// </summary>
        /// <param name="sf">Sayým tutanaðý bilgilerini tutan nesne</param>
        private void Sil(SayimForm sf)
        {
            Sonuc sonuc = servisTMM.SayimSil(kullanan, sf);
            if (!sonuc.islemSonuc)
                GenelIslemler.HataYaz(this, sonuc.hataStr);
            else
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMSYG018);
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            hdnSayimNo.Value = "";

            fpL.CancelEdit();
            fpL.Sheets[0].Cells[0, 0, fpL.Sheets[0].RowCount - 1, fpL.Sheets[0].ColumnCount - 1].Text = "";
            fpL.Sheets[0].RowCount = ekleSatirSayisi;
        }

        /// <summary>
        /// Sayfadaki farpoint grid kontrolünün ilk yükleniþte ayarlanmasýný saðlayan yordam
        /// </summary>
        /// <param name="kontrol">Farpoint grid kontrolü</param>
        void GridInit(FarPoint.Web.Spread.FpSpread kontrol)
        {
            kontrol.RenderCSSClass = true;
            kontrol.EditModeReplace = true;

            kontrol.Sheets.Count = 1;

            fpL.Sheets[0].RowCount = ekleSatirSayisi;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 8;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].ColumnSpan = 2;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMSYG019;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMSYG020;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMSYG021;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMSYG022;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMSYG023;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMSYG024;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMSYG025;

            kontrol.Sheets[0].Columns[0].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[1].HorizontalAlign = HorizontalAlign.Center;
            kontrol.Sheets[0].Columns[2, 3].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[4, kontrol.Sheets[0].ColumnCount - 2].HorizontalAlign = HorizontalAlign.Right;
            kontrol.Sheets[0].Columns[kontrol.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Left;

            kontrol.Sheets[0].Columns[2, 3].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[2, 3].Locked = true;

            kontrol.Sheets[0].Columns[6].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[6].Locked = true;

            kontrol.Sheets[0].Columns[0].Width = 120;
            kontrol.Sheets[0].Columns[1].Width = 20;
            kontrol.Sheets[0].Columns[2].Width = 180;
            kontrol.Sheets[0].Columns[3].Width = 70;
            kontrol.Sheets[0].Columns[4, kontrol.Sheets[0].ColumnCount - 2].Width = 60;
            kontrol.Sheets[0].Columns[kontrol.Sheets[0].ColumnCount - 1].Width = 190;

            TasinirGenel.MyLinkType hesapPlaniLink = new TasinirGenel.MyLinkType("HesapPlaniGoster()");
            hesapPlaniLink.ImageUrl = "../App_themes/images/bul1.gif";
            kontrol.Sheets[0].Columns[1].CellType = hesapPlaniLink;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;

            kontrol.Attributes.Add("onDataChanged", "HucreDegisti(this)");
            GenelIslemlerIstemci.RakamAlanFormatla(kontrol, 4, 6, 4);
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
                img.AlternateText = Resources.TasinirMal.FRMSYG026;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMSYG027;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMSYG028;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMSYG029;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);
            }

            base.Render(writer);
        }

        /// <summary>
        /// Farpoint grid kontrolü ile ilgili boþ satýr ekleme, araya
        /// satýr ekleme ve satýr silme gibi iþlemlerin yapýldýðý yordam
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
            fpL.SaveChanges();
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
    }
}