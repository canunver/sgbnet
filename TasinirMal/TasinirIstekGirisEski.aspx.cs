using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using System.Collections;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr istek belgesi bilgilerinin kayýt, listeleme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class TasinirIstekGirisEski : TMMSayfa
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
        ///     Ýlk yükleniþte, sayfadaki kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        ///     Sayfa adresinde gelen yil, muhasebe, harcama, ambar ve belgeNo girdi
        ///     dizgileri dolu ise ilgili taþýnýr istek belgesi bilgileri listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
                formAdi = Resources.TasinirMal.FRMIGF001;
            else
                formAdi = Resources.TasinirMal.FRMIGF042;

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(false);

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
            {
                string istekUrl = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirIstekURL");
                link1.Attributes.Add("onclick", "this.style.behavior='url(#default#homepage)';this.setHomePage('" + istekUrl + "');");
                link2.Attributes.Add("href", "javascript:window.external.AddFavorite('" + istekUrl + "','" + Resources.TasinirMal.FRMIGF002 + "')");
                divPersonelSecim.Visible = false;
            }
            else
            {
                kolon1.Visible = false;
                kolon2.Visible = false;
            }

            SayfaUstAltBolumYaz(this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");
            this.txtPersonel.Attributes.Add("onblur", "kodAdGetir('36','lblPersonelAd',true,new Array('txtPersonel'),'KONTROLDENOKU');");

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
                txtBelgeTarihi.Value = DateTime.Now.ToShortDateString();

                if (!string.IsNullOrEmpty(Request.QueryString["yil"])
                     && !string.IsNullOrEmpty(Request.QueryString["muhasebe"])
                     && !string.IsNullOrEmpty(Request.QueryString["harcama"])
                     && !string.IsNullOrEmpty(Request.QueryString["ambar"])
                     && !string.IsNullOrEmpty(Request.QueryString["istekYapan"])
                     && !string.IsNullOrEmpty(Request.QueryString["belgeNo"]))
                {
                    ddlYil.SelectedValue = Request.QueryString["yil"];
                    txtMuhasebe.Text = Request.QueryString["muhasebe"];
                    txtHarcamaBirimi.Text = Request.QueryString["harcama"];
                    txtAmbar.Text = Request.QueryString["ambar"];
                    txtPersonel.Text = Request.QueryString["istekYapan"];
                    hdnBelgeNo.Value = Request.QueryString["belgeNo"];
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

            if (txtPersonel.Text.Trim() != "")
                lblPersonelAd.Text = GenelIslemler.KodAd(36, txtPersonel.Text.Trim(), true);
            else
                lblPersonelAd.Text = "";
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çaðýrýlýr ve toplanan
        /// kriterler Yazdir yordamýna gönderilir ve rapor hazýrlanmýþ olur.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            ObjectArray bilgi = servisTMM.IstekListele(kullanan, KriterTopla(), true);
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

            if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
                Yazdir((IstekForm)bilgi.objeler[0]);
            else
                Yazdir_Hediyelik((IstekForm)bilgi.objeler[0]);
        }

        /// <summary>
        /// Parametre olarak verilen istek formuna ait kriterleri
        /// sunucudaki taþýnýr istek belgesi raporlama yordamýna gönderir,
        /// sunucudan gelen bilgi kümesini excel raporuna aktarýr.
        /// </summary>
        /// <param name="iForm">Taþýnýr istek belgesi kriter bilgilerini tutan nesne</param>
        private void Yazdir(IstekForm istek)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TASINIRISTEKBELGESI.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("IstekYapan", istek.harcamaAd);
            XLS.HucreAdBulYaz("Tarih", istek.belgeTarihi.ToString());
            XLS.HucreAdBulYaz("BelgeNo", istek.belgeNo.PadLeft(6, '0'));

            satir = kaynakSatir;

            for (int i = 0; i < istek.detay.Count; i++)
            {
                IstekDetay id = (IstekDetay)istek.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                XLS.HucreDegerYaz(satir, sutun, i + 1);
                XLS.HucreDegerYaz(satir, sutun + 1, id.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, id.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, id.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(id.istenilenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(id.karsilananMiktar.ToString(), (double)0));

            }

            ImzaEkle(XLS, ref satir, sutun);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Taþýnýr istek belgesi excel raporuna imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili iþlemleri yapan nesne</param>
        /// <param name="satir">Ýmza bilgilerinin yazýlmaya baþlanacaðý satýr numarasý</param>
        /// <param name="sutun">Ýmza bilgilerinin yazýlmaya baþlanacaðý sütun numarasý</param>
        private void ImzaEkle(Tablo XLS, ref int satir, int sutun)
        {
            satir += 2;

            //ObjectArray imza1 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.HARCAMAYETKILISI);
            //ImzaBilgisi i1 = null;
            //if (imza1.sonuc.islemSonuc && imza1.objeler.Count > 0)
            //    i1 = (ImzaBilgisi)imza1.objeler[0];
            //string ad1 = string.Empty;
            //string unvan1 = string.Empty;
            //if (i1 != null)
            //{
            //    ad1 = i1.adSoyad;
            //    unvan1 = i1.unvan;
            //}
            //Melih
            //string ad1 = string.Empty;
            //string unvan1 = string.Empty;

            //TNS.TMM.Personel p = new TNS.TMM.Personel();
            //p.kod = tcKimlikNo;
            //ObjectArray imza1 = servisTMM.PersonelListele(kullanan, p);
            //if (imza1.sonuc.islemSonuc)
            //{
            //    if (imza1.objeler.Count > 0)
            //    {
            //        foreach (TNS.TMM.Personel per in imza1.objeler)
            //        {
            //            ad1 = per.ad;
            //            unvan1 = per.unvan;
            //        }
            //    }
            //}

            ObjectArray imza1 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.BIRIMYETKILISI);
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

            ObjectArray imza2 = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), (int)ENUMImzaYer.TASINIRKAYITYETKILISI);
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

            XLS.SatirAc(satir, 6);
            XLS.HucreKopyala(0, sutun, 5, sutun + 7, satir, sutun);

            for (int i = satir; i < satir + 5; i++)
            {
                XLS.HucreBirlestir(i, sutun, i, sutun + 3);
                XLS.HucreBirlestir(i, sutun + 4, i, sutun + 7);
            }
            XLS.HucreBirlestir(satir + 5, sutun, satir + 5, sutun + 7);

            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMIGF003);
            XLS.DuseyHizala(satir, sutun, 2);

            XLS.HucreDegerYaz(satir, sutun + 4, Resources.TasinirMal.FRMIGF004);
            XLS.DuseyHizala(satir, sutun + 4, 2);

            XLS.HucreDegerYaz(satir + 1, sutun, Resources.TasinirMal.FRMIGF005);
            XLS.DuseyHizala(satir + 1, sutun, 2);
            XLS.KoyuYap(satir + 1, sutun, true);

            XLS.HucreDegerYaz(satir + 1, sutun + 4, Resources.TasinirMal.FRMIGF006);
            XLS.DuseyHizala(satir + 1, sutun + 4, 2);
            XLS.KoyuYap(satir + 1, sutun + 4, true);

            XLS.HucreDegerYaz(satir + 2, sutun, string.Format(Resources.TasinirMal.FRMIGF007, ad1));
            //XLS.KoyuYap(satir + 2, sutun, true);

            XLS.HucreDegerYaz(satir + 2, sutun + 4, string.Format(Resources.TasinirMal.FRMIGF007, ad2));
            //XLS.KoyuYap(satir + 2, sutun + 4, true);

            XLS.HucreDegerYaz(satir + 3, sutun, string.Format(Resources.TasinirMal.FRMIGF008, unvan1));
            //XLS.KoyuYap(satir + 3, sutun, true);

            XLS.HucreDegerYaz(satir + 3, sutun + 4, string.Format(Resources.TasinirMal.FRMIGF008, unvan2));
            //XLS.KoyuYap(satir + 3, sutun + 4, true);

            XLS.HucreDegerYaz(satir + 4, sutun, Resources.TasinirMal.FRMIGF009);
            //XLS.KoyuYap(satir + 4, sutun, true);

            XLS.HucreDegerYaz(satir + 4, sutun + 4, Resources.TasinirMal.FRMIGF009);
            //XLS.KoyuYap(satir + 4, sutun + 4, true);

            XLS.HucreDegerYaz(satir + 5, sutun, Resources.TasinirMal.FRMIGF010);

            XLS.YatayCizgiCiz(satir, sutun, sutun + 7, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir + 6, sutun, sutun + 7, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);
            XLS.DuseyCizgiCiz(satir, satir + 5, sutun + 8, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK, true);

            satir += 5;
        }

        private void Yazdir_Hediyelik(IstekForm istek)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "DogrudanVerme.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("BelgeNo", istek.belgeNo.PadLeft(6, '0'));
            XLS.HucreAdBulYaz("KimeVerildi", istek.istekYapanAd);
            XLS.HucreAdBulYaz("BelgeTarih", istek.belgeTarihi.ToString());
            XLS.HucreAdBulYaz("HarcamaAd", istek.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", istek.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", istek.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", istek.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", istek.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", istek.muhasebeKod);

            ImzaEkle_Hediyelik(XLS);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < istek.detay.Count; i++)
            {
                IstekDetay id = (IstekDetay)istek.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 9, satir, sutun + 11);

                XLS.HucreDegerYaz(satir, sutun, i + 1);
                XLS.HucreDegerYaz(satir, sutun + 1, id.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, id.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, id.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(id.istenilenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(id.birimFiyat.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble((id.istenilenMiktar * id.birimFiyat * ((decimal)id.kdvOran / 100 + 1)).ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 9, id.aciklama);
            }

            if (satir - kaynakSatir % 56 > 45)
                XLS.SayfaSonuKoyHucresel(satir);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        private void ImzaEkle_Hediyelik(Tablo XLS)
        {
            ObjectArray imzalar = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), 0);
            if (!imzalar.sonuc.islemSonuc || imzalar.objeler.Count <= 0)
                return;

            Hashtable ht = new Hashtable();
            ht.Add((int)ENUMImzaYer.TASINIRKAYITYETKILISI, new string[2] { "AdSoyadTKKY", "UnvanTKKY" });
            ht.Add((int)ENUMImzaYer.HARCAMAYETKILISI, new string[2] { "AdSoyadHY", "UnvanHY" });

            foreach (ImzaBilgisi imza in imzalar.objeler)
            {
                string[] imzaAdres = (string[])ht[imza.imzaYer];
                if (imzaAdres == null)
                    continue;

                XLS.HucreAdBulYaz(imzaAdres[0], imza.adSoyad);
                XLS.HucreAdBulYaz(imzaAdres[1], imza.unvan);
            }
        }

        /// <summary>
        /// Listeleme kriterleri istek form nesnesinde parametre olarak alýnýr,
        /// sunucuya gönderilir ve taþýnýr istek belgesi bilgileri sunucudan alýnýr.
        /// Hata varsa ekrana hata bilgisi yazýlýr, yoksa gelen bilgiler ekrana yazýlýr.
        /// </summary>
        /// <param name="iForm">Taþýnýr istek belgesi listeleme kriterlerini tutan nesne</param>
        private void Listele(IstekForm iForm)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            ObjectArray bilgi = servisTMM.IstekListele(kullanan, iForm, true);

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

            IstekForm istekForm = (IstekForm)bilgi.objeler[0];
            ddlYil.SelectedValue = istekForm.yil.ToString();
            txtMuhasebe.Text = istekForm.muhasebeKod;
            lblMuhasebeAd.Text = istekForm.muhasebeAd;
            txtHarcamaBirimi.Text = istekForm.harcamaKod;
            lblHarcamaBirimiAd.Text = istekForm.harcamaAd;
            txtAmbar.Text = istekForm.ambarKod;
            lblAmbarAd.Text = istekForm.ambarAd;
            txtBelgeTarihi.Value = istekForm.belgeTarihi.ToString();
            hdnBelgeNo.Value = istekForm.belgeNo;
            txtPersonel.Text = istekForm.istekYapanKod;
            lblPersonelAd.Text = istekForm.istekYapanAd;

            if (istekForm.detay.Count > fpL.Sheets[0].RowCount)
                fpL.Sheets[0].RowCount = istekForm.detay.Count + 10;

            int i = 0;
            foreach (IstekDetay id in istekForm.detay)
            {
                fpL.Sheets[0].Cells[i, 0].Text = id.hesapPlanKod;
                fpL.Sheets[0].Cells[i, 2].Text = id.istenilenMiktar.ToString();
                fpL.Sheets[0].Cells[i, 3].Text = id.karsilananMiktar.ToString();
                fpL.Sheets[0].Cells[i, 4].Text = id.olcuBirimAd;
                fpL.Sheets[0].Cells[i, 5].Text = id.hesapPlanAd;
                fpL.Sheets[0].Cells[i, 6].Text = id.kdvOran.ToString();
                fpL.Sheets[0].Cells[i, 7].Text = id.birimFiyat.ToString();
                fpL.Sheets[0].Cells[i, 8].Text = id.aciklama;
                i++;
            }
        }

        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele(KriterTopla());
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr istek belgesi bilgilerini ekrandaki ilgili kontrollerden toplayan yordam çaðýrýlýr
        /// ve daha sonra toplanan bilgiler kaydedilmek üzere Kaydet yordamýna gönderilir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            Kaydet(KriterTopla());
        }

        /// <summary>
        /// Sayfadaki buton kontrollerini aktif hale getiren yordam
        /// </summary>
        private void ButonlariAktifYap()
        {
            btnYazdir.Enabled = true;
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden taþýnýr istek belgesi bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>Taþýnýr istek belgesi bilgileri döndürülür.</returns>
        private IstekForm KriterTopla()
        {
            IstekForm iForm = new IstekForm();
            iForm.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
            iForm.muhasebeKod = txtMuhasebe.Text.Trim();
            iForm.harcamaKod = txtHarcamaBirimi.Text.Trim();
            iForm.ambarKod = txtAmbar.Text.Trim();
            iForm.belgeNo = hdnBelgeNo.Value.Trim();
            iForm.belgeTarihi = new TNSDateTime(txtBelgeTarihi.Value.Trim());
            iForm.istekYapanKod = txtPersonel.Text.Trim();
            iForm.hediyelik = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0);
            return iForm;
        }

        /// <summary>
        /// Parametre olarak verilen taþýnýr istek belgesi üst bilgilerine detay bilgileri
        /// ekleyip kaydedilmek üzere sunucuya gönderir ve iþlem sonucunu ekranda görüntüler.
        /// </summary>
        /// <param name="iForm">Taþýnýr istek belgesi bilgilerini tutan nesne</param>
        private void Kaydet(IstekForm iForm)
        {
            fpL.SaveChanges();

            for (int i = 0; i < fpL.Sheets[0].RowCount; i++)
            {
                IstekDetay detay = new IstekDetay();
                detay.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.SelectedValue.Trim(), 0);
                detay.muhasebeKod = txtMuhasebe.Text.Trim();
                detay.harcamaKod = txtHarcamaBirimi.Text.Trim();
                detay.ambarKod = txtAmbar.Text.Trim();
                detay.belgeNo = hdnBelgeNo.Value.Trim();

                detay.hesapPlanKod = fpL.Sheets[0].Cells[i, 0].Text.Trim();
                detay.istenilenMiktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 2].Text.Trim(), 0);

                if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
                {
                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                        detay.karsilananMiktar = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 3].Text.Trim(), 0);
                }
                else
                {
                    detay.kdvOran = OrtakFonksiyonlar.ConvertToInt(fpL.Sheets[0].Cells[i, 6].Text.Trim(), 0);
                    detay.birimFiyat = OrtakFonksiyonlar.ConvertToDecimal(fpL.Sheets[0].Cells[i, 7].Text.Trim(), 0);
                    detay.aciklama = fpL.Sheets[0].Cells[i, 8].Text.Trim();
                }

                if (!string.IsNullOrEmpty(detay.hesapPlanKod))
                {
                    int satir = 1;
                    foreach (IstekDetay d in iForm.detay)
                    {
                        if (d.hesapPlanKod.Replace(".", "") == detay.hesapPlanKod.Replace(".", ""))
                        {
                            GenelIslemler.HataYaz(this, string.Format(Resources.TasinirMal.FRMIGF011, (i + 1).ToString(), detay.hesapPlanKod, satir.ToString()));
                            return;
                        }
                        satir++;
                    }
                    iForm.detay.Add(detay);
                }
            }

            Sonuc sonuc = servisTMM.IstekKaydet(kullanan, iForm);
            if (!sonuc.islemSonuc)
                GenelIslemler.HataYaz(this, sonuc.hataStr);
            else
            {
                hdnBelgeNo.Value = sonuc.anahtar;
                ButonlariAktifYap();
                Listele(KriterTopla());
                GenelIslemler.BilgiYaz(this, Resources.TasinirMal.FRMIGF012);
            }
        }

        /// <summary>
        /// Temizle tuþuna basýlýnca çalýþan olay metodu
        /// Kullanýcý tarafýndan sayfadaki kontrollere yazýlmýþ bilgiler temizlenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            hdnBelgeNo.Value = "";

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
            kontrol.ClientAutoCalculation = false;
            //kontrol.EnableClientScript = false;
            kontrol.HierarchicalView = false;
            kontrol.IsPrint = false;

            kontrol.Sheets.Count = 1;
            kontrol.Sheets[0].RowCount = 20;

            kontrol.Sheets[0].AllowSort = false;
            kontrol.Sheets[0].AllowPage = false;
            kontrol.Sheets[0].IsTrackingViewState = true;
            kontrol.Sheets[0].RowHeaderVisible = true;
            kontrol.Sheets[0].RowHeaderWidth = 25;
            kontrol.Sheets[0].RowHeader.Rows[-1].Resizable = false;

            kontrol.Sheets[0].ColumnHeader.RowCount = 1;
            kontrol.Sheets[0].ColumnHeader.Columns.Count = 9;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].ColumnSpan = 2;

            kontrol.Sheets[0].ColumnHeader.Cells[0, 0].Value = Resources.TasinirMal.FRMIGF013;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 2].Value = Resources.TasinirMal.FRMIGF014;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 3].Value = Resources.TasinirMal.FRMIGF015;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 4].Value = Resources.TasinirMal.FRMIGF016;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 5].Value = Resources.TasinirMal.FRMIGF017;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 6].Value = Resources.TasinirMal.FRMIGF039;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 7].Value = Resources.TasinirMal.FRMIGF040;
            kontrol.Sheets[0].ColumnHeader.Cells[0, 8].Value = Resources.TasinirMal.FRMIGF041;

            int sutunAd, sutunOlcu = 0;
            string js;
            if (OrtakFonksiyonlar.ConvertToInt(Request.QueryString["hediyelik"], 0) <= 0)
            {
                sutunAd = 5;
                sutunOlcu = 4;
                js = "HesapPlaniGoster()";

                kontrol.Sheets[0].Columns[6, 8].Visible = false;
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR))
                {
                    kontrol.Sheets[0].Columns[3].Visible = false;
                    sutunAd--;
                    sutunOlcu--;
                }
            }
            else
            {
                sutunAd = 4;
                sutunOlcu = 3;
                js = "StokListesiAc()";
                kontrol.Sheets[0].Columns[3].Visible = false;
            }

            kontrol.Sheets[0].Columns[0, 1].HorizontalAlign = HorizontalAlign.Left;
            kontrol.Sheets[0].Columns[2, 3].HorizontalAlign = HorizontalAlign.Right;
            kontrol.Sheets[0].Columns[kontrol.Sheets[0].ColumnCount - 2, kontrol.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Left;

            kontrol.Sheets[0].Columns[4, kontrol.Sheets[0].ColumnCount - 2].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            kontrol.Sheets[0].Columns[4, kontrol.Sheets[0].ColumnCount - 2].Locked = true;

            kontrol.Sheets[0].Columns[0].Width = 150;
            kontrol.Sheets[0].Columns[1].Width = 20;
            kontrol.Sheets[0].Columns[2, 3].Width = 100;
            kontrol.Sheets[0].Columns[4].Width = 120;
            kontrol.Sheets[0].Columns[5].Width = 200;
            kontrol.Sheets[0].Columns[6].Width = 60;
            kontrol.Sheets[0].Columns[7].Width = 100;
            kontrol.Sheets[0].Columns[8].Width = 150;

            TasinirGenel.MyLinkType hesapPlaniLink = new TasinirGenel.MyLinkType(js);
            hesapPlaniLink.ImageUrl = "../App_themes/images/bul1.gif";
            kontrol.Sheets[0].Columns[1].CellType = hesapPlaniLink;

            FarPoint.Web.Spread.TextCellType cTextType = new FarPoint.Web.Spread.TextCellType();
            kontrol.Sheets[0].Columns[0].CellType = cTextType;

            kontrol.Attributes.Add("onDataChanged", "HucreDegisti(this, " + sutunAd + ", " + sutunOlcu + ")");
            GenelIslemlerIstemci.RakamAlanFormatla(kontrol, 2, 3, 4);
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
                img.AlternateText = Resources.TasinirMal.FRMIGF018;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "BosSatirAc(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/InsertRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMIGF019;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ArayaSatirEkle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/DeleteRow.gif";
                img.AlternateText = Resources.TasinirMal.FRMIGF020;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "SatirSil(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/ClearRows.gif";
                img.AlternateText = Resources.TasinirMal.FRMIGF021;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ListeTemizle(); return false;");
                tc1.Controls.Add(new LiteralControl("&nbsp;"));
                tc1.Controls.Add(img);

                img = new Image();
                img.ImageUrl = "../App_themes/images/sigma.gif";
                img.AlternateText = Resources.TasinirMal.FRMIGF022;
                img.Attributes.Add("style", "border-color:buttonface;border-width:1px;border-style:Solid;");
                img.Attributes.Add("width", "22");
                img.Attributes.Add("height", "22");
                img.Attributes.Add("onclick", "ToplamHesapla(); return false;");
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
            GenelIslemlerIstemci.RakamAlanFormatla(fpL, 2, 3, 4);
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