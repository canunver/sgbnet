using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;
using Ext1.Net;
using System.Collections.Generic;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r i�lem fi�i bilgilerinin sorgulama, yazd�rma ve durum de�i�tirme i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class TasinirIslemFormSorguYeni : TMMSayfa
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servis = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        IUZYServis servisUzy = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Ta��n�r i�lem fi�i sayfas� k�t�phane malzemeleri i�in mi a��ld� bilgisini tutan de�i�ken
        /// </summary>
        static bool kutuphaneGoster = false;

        /// <summary>
        /// Ta��n�r i�lem fi�i sayfas� m�ze malzemeleri i�in mi a��ld� bilgisini tutan de�i�ken
        /// </summary>
        static bool muzeGoster = false;

        /// <summary>
        /// Ta��n�r i�lem fi�i sayfas� da��t�m �ade i�in mi a��ld� bilgisini tutan de�i�ken
        /// </summary>
        static bool dagitimIade = false;

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            TasinirGenel.JSResourceEkle_TasinirIslemSorgu(this);
            GenelIslemlerIstemci.GenelJSResourceEkle(this);

            if (Request.QueryString["kutuphane"] + "" != "")
                kutuphaneGoster = true;
            else
                kutuphaneGoster = false;

            if (Request.QueryString["muze"] + "" != "")
                muzeGoster = true;
            else
                muzeGoster = false;
            if (Request.QueryString["dagitimIade"] + "" != "")
                dagitimIade = true;
            else
                dagitimIade = false;

            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giri� izni varm�?
            bool izin = false;
            if (dagitimIade)
                izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMiBirim(kullanan);
            else
                izin = TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan);

            if (!izin)
                GenelIslemler.SayfayaGirmesin(true);

            //this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            //this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            //this.txtAmbar.Attributes.Add("onblur", "kodAdGetir('33','lblAmbarAd',true,new Array('txtMuhasebe','txtHarcamaBirimi','txtAmbar'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                GenelIslemler.YilDoldur(ddlYil, 2007, DateTime.Now.Year + 1);
                pgFiltre.UpdateProperty("yil", DateTime.Now.Year);

                List<object> liste = new List<object>();
                liste.Add(new { Name = Resources.TasinirMal.FRMTIS002, ID = "1" });
                liste.Add(new { Name = Resources.TasinirMal.FRMTIS003, ID = "5" });
                liste.Add(new { Name = Resources.TasinirMal.FRMTIS004, ID = "9" });
                liste.Add(new { Name = Resources.TasinirMal.FRMTIS005, ID = "0" });
                StoreDurum.DataSource = liste;
                StoreDurum.DataBind();
                pgFiltre.UpdateProperty("birim", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                pgFiltre.UpdateProperty("muhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                pgFiltre.UpdateProperty("ambar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));

                IslemTipiDoldur();
            }
        }

        /// <summary>
        /// Listele tu�una bas�l�nca �al��an olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplan�r ve sunucuya g�nderilir
        /// ve ta��n�r i�lem fi�i bilgileri sunucudan al�n�r. Hata varsa ekrana hata bilgisi yaz�l�r,
        /// yoksa gelen ta��n�r i�lem fi�i bilgileri gvBelgeler GridView kontrol�ne doldurulur.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListe_Click(object sender, DirectEventArgs e)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRKULLANICIBIRIM))
            {
                if (txtMuhasebe.Text.Trim() == "" || txtHarcamaBirimi.Text.Trim() == "" || txtAmbar.Text.Trim() == "")
                {
                    GenelIslemler.HataYaz(this, Resources.TasinirMal.FRMLDV001);
                    return;
                }
            }


            //BARIS:Burada kald�m. aspx taraf�nda da d�zeltilmesi gereken yerler, eklenmesi gereken gizli kolonlar var. Headerlar da incelenecek... Eskisiyle k�ysalanacak. Linkler olu�turulacak...
            if (txtMuhasebe.Text.Trim() != "" && txtHarcamaBirimi.Text.Trim() != "" && txtAmbar.Text.Trim() != "")
                TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();
            tf.durum = OrtakFonksiyonlar.ConvertToInt(ddlDurum.Value, 0);
            tf.gMuhasebeKod = txtGonMuhasebe.Text.Trim();
            tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Trim();
            tf.gAmbarKod = txtGonAmbar.Text.Trim();

            TasinirFormKriter kriter = kriterTopla();
            ObjectArray bilgi = servis.TasinirIslemFisiListele(kullanan, tf, kriter);

            DataTable dt = new DataTable();
            dt.Columns.Add("fisno");
            dt.Columns.Add("yil");
            dt.Columns.Add("muhasebe");
            dt.Columns.Add("harcamaBirimi");
            dt.Columns.Add("harcamaBirimiAd");
            dt.Columns.Add("ambar");
            dt.Columns.Add("fistarih");
            dt.Columns.Add("islemtipi");
            dt.Columns.Add("durum");
            dt.Columns.Add("islemTarih");
            dt.Columns.Add("islemYapan");

            if (bilgi.sonuc.islemSonuc)
            {
                foreach (TNS.TMM.TasinirIslemForm tasForm in bilgi.objeler)
                {
                    string islemAd = servisUzy.UzayDegeriStr(kullanan, "TASISLEMTIPAD", tasForm.islemTipKod.ToString(), true);
                    string durum = "";
                    if (tasForm.durum == (int)ENUMBelgeDurumu.YENI)
                        durum = Resources.TasinirMal.FRMTIS006;
                    else if (tasForm.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                        durum = Resources.TasinirMal.FRMTIS007;
                    else if (tasForm.durum == (int)ENUMBelgeDurumu.ONAYLI)
                        durum = Resources.TasinirMal.FRMTIS008;
                    else if (tasForm.durum == (int)ENUMBelgeDurumu.IPTAL)
                        durum = Resources.TasinirMal.FRMTIS009;

                    if (!dagitimIade && (tasForm.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tasForm.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                        continue;
                    if (dagitimIade && (tasForm.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADECIKIS && tasForm.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                        continue;

                    dt.Rows.Add(tasForm.fisNo.Trim(), tasForm.yil, tasForm.muhasebeKod, tasForm.harcamaKod, tasForm.harcamaAd, tasForm.ambarKod + "-" + tasForm.ambarAd, tasForm.fisTarih, islemAd, durum, tasForm.islemTarih.ToString(), tasForm.islemYapan);
                }
            }
            else
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr, bilgi.sonuc.vtHatasi);

            StoreListe.DataSource = dt;
            StoreListe.DataBind();

            //BARIS
            //for (int i = 0; i < gvBelgeler.Rows.Count; i++)
            //    gvBelgeler.Rows[i].Cells[1].Text = Server.HtmlDecode(gvBelgeler.Rows[i].Cells[1].Text);
        }

        private TasinirFormKriter kriterTopla()
        {
            TasinirFormKriter kriter = new TasinirFormKriter();

            kriter.belgeNoBasla = pgFiltre.Source["belgeNo1"].Value == string.Empty ? string.Empty : pgFiltre.Source["belgeNo1"].Value.PadLeft(6, '0');
            kriter.belgeNoBit = pgFiltre.Source["belgeNo2"].Value == string.Empty ? string.Empty : pgFiltre.Source["belgeNo2"].Value.PadLeft(6, '0');
            kriter.belgeTarihBasla = new TNSDateTime(txtBelgeTarih1.Text);
            kriter.belgeTarihBit = new TNSDateTime(txtBelgeTarih2.Text);
            kriter.durumTarihBasla = new TNSDateTime(txtDurumTarih1.Text);
            kriter.durumTarihBit = new TNSDateTime(txtDurumTarih2.Text);
            kriter.hesapKodu = pgFiltre.Source["tasinirHesapKodu"].Value.Replace(".", "");
            kriter.islemTipi = OrtakFonksiyonlar.ConvertToInt(ddlIslemTipi.Value, 0);
            kriter.nereyeVeridi = pgFiltre.Source["nereye"].Value.Trim();
            kriter.kimeVeridi = pgFiltre.Source["kime"].Value.Trim();
            kriter.neredenGeldi = pgFiltre.Source["nereden"].Value.Trim();

            if (kutuphaneGoster && kriter.hesapKodu != "")
            {
                if (kriter.hesapKodu.IndexOf(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString()) < 0)
                    kriter.hesapKodu = "";
            }
            if (muzeGoster && kriter.hesapKodu != "")
            {
                if (kriter.hesapKodu.IndexOf(OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString()) < 0)
                    kriter.hesapKodu = "";
            }

            if (kutuphaneGoster && kriter.hesapKodu == "")
                kriter.hesapKodu = OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.KUTUPHANE, 0).ToString();
            else if (muzeGoster && kriter.hesapKodu == "")
                kriter.hesapKodu = OrtakFonksiyonlar.ConvertToInt((int)ENUMTasinirHesapKodu.MUZE, 0).ToString();

            return kriter;
        }

        /// <summary>
        /// Liste Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Listeleme kriterleri ekrandaki ilgili kontrollerden toplan�r, sunucuya g�nderilir
        /// ve ta��n�r i�lem fi�i bilgileri sunucudan al�n�r. Hata varsa ekrana hata bilgisi
        /// yaz�l�r, yoksa gelen ta��n�r i�lem fi�i bilgilerini i�eren excel raporu �retilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListeYazdir_Click(object sender, DirectEventArgs e)
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

            tf.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();
            tf.durum = OrtakFonksiyonlar.ConvertToInt(ddlDurum.Value, 0);

            TasinirFormKriter kriter = kriterTopla();
            ObjectArray bilgi = servis.TasinirIslemFisiListele(kullanan, tf, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.HataYaz(this, bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TIFListe.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                TNS.TMM.TasinirIslemForm tifBelge = (TNS.TMM.TasinirIslemForm)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, tifBelge.yil);
                XLS.HucreDegerYaz(satir, sutun + 1, tifBelge.muhasebeKod + " - " + tifBelge.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 2, tifBelge.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 3, tifBelge.fisNo);

                string islemAd = servisUzy.UzayDegeriStr(kullanan, "TASISLEMTIPAD", tifBelge.islemTipKod.ToString(), true);
                XLS.HucreDegerYaz(satir, sutun + 4, islemAd);

                string durum = "";
                if (tifBelge.durum == (int)ENUMBelgeDurumu.YENI)
                    durum = Resources.TasinirMal.FRMTIS006;
                else if (tifBelge.durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                    durum = Resources.TasinirMal.FRMTIS007;
                else if (tifBelge.durum == (int)ENUMBelgeDurumu.ONAYLI)
                    durum = Resources.TasinirMal.FRMTIS008;
                else if (tifBelge.durum == (int)ENUMBelgeDurumu.IPTAL)
                    durum = Resources.TasinirMal.FRMTIS009;
                XLS.HucreDegerYaz(satir, sutun + 5, durum);

                XLS.HucreDegerYaz(satir, sutun + 6, tifBelge.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 7, tifBelge.islemTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, tifBelge.islemYapan.ToString());
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// ��lem tipi bilgileri sunucudan �ekilir ve ddlIslemTipi DropDownList kontrol�ne doldurulur.
        /// </summary>
        private void IslemTipiDoldur()
        {
            ObjectArray bilgi = servis.IslemTipListele(kullanan, new IslemTip());

            if (dagitimIade)
                ddlIslemTipi.Items.Add(new Ext1.Net.ListItem(Resources.TasinirMal.FRMTIS010, "-999"));
            else
                ddlIslemTipi.Items.Add(new Ext1.Net.ListItem(Resources.TasinirMal.FRMTIS010, "0"));

            foreach (IslemTip tip in bilgi.objeler)
            {
                if (dagitimIade)
                {
                    if (tip.tur != (int)ENUMIslemTipi.DAGITIMIADECIKIS && tip.tur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        continue;
                }
                else
                {
                    if (tip.tur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tip.tur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        continue;
                }

                ddlIslemTipi.Items.Add(new Ext1.Net.ListItem(tip.ad, tip.kod.ToString()));
            }
            if (dagitimIade)
                ddlIslemTipi.SetValueAndFireSelect("-999");
            else
                ddlIslemTipi.SetValueAndFireSelect("0");
        }
    }
}