using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// İhtiyaç fazlası taşınır istek bilgilerinin kayıt ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class AcikPazarIstek : TMMSayfa
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        ///     Sayfa ilk defa çağırılıyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);

            formAdi = Resources.TasinirMal.FRMAPI001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            this.txtMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblMuhasebeAd',true,new Array('txtMuhasebe'),'KONTROLDENOKU');");
            this.txtHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblHarcamaBirimiAd',true,new Array('txtMuhasebe','txtHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtIYMuhasebe.Attributes.Add("onblur", "kodAdGetir('31','lblIYMuhasebeAd',true,new Array('txtIYMuhasebe'),'KONTROLDENOKU');");
            this.txtIYHarcamaBirimi.Attributes.Add("onblur", "kodAdGetir('32','lblIYHarcamaBirimiAd',true,new Array('txtIYMuhasebe','txtIYHarcamaBirimi'),'KONTROLDENOKU');");
            this.txtHesapPlanKod.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlanKod'),'KONTROLDENOKU');");

            this.btnKaydet.Attributes.Add("onclick", "return OnayAl('Kaydet','btnKaydet');");
            //this.btnYazdir.Attributes.Add("onclick", "return OnayAl('Yazdir','btnYazdir');");

            if (!IsPostBack)
            {
                txtIYMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtIYHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                IlDoldur();
            }

            if (txtMuhasebe.Text.Trim() != "")
                lblMuhasebeAd.Text = GenelIslemler.KodAd(31, txtMuhasebe.Text.Trim(), true);
            else
                lblMuhasebeAd.Text = "";

            if (txtHarcamaBirimi.Text.Trim() != "")
                lblHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true);
            else
                lblHarcamaBirimiAd.Text = "";

            if (txtHesapPlanKod.Text.Trim() != "")
                lblHesapPlanAd.Text = GenelIslemler.KodAd(30, txtHesapPlanKod.Text.Trim(), true);
            else
                lblHesapPlanAd.Text = "";

            if (txtIYMuhasebe.Text.Trim() != "")
                lblIYMuhasebeAd.Text = GenelIslemler.KodAd(31, txtIYMuhasebe.Text.Trim(), true);
            else
                lblIYMuhasebeAd.Text = "";

            if (txtIYHarcamaBirimi.Text.Trim() != "")
                lblIYHarcamaBirimiAd.Text = GenelIslemler.KodAd(32, txtIYMuhasebe.Text.Trim() + "-" + txtIYHarcamaBirimi.Text.Trim(), true);
            else
                lblIYHarcamaBirimiAd.Text = "";
        }

        /// <summary>
        /// İl bilgileri sunucudan çekilir ve ddlIl DropDownList kontrolüne doldurulur.
        /// Daha sonra ddlIl_SelectedIndexChanged yordamı çağırılır.
        /// </summary>
        private void IlDoldur()
        {
            ObjectArray bilgi = servisTMM.IlListele(kullanan, new Il());
            ddlIl.Items.Add(new ListItem("", ""));
            foreach (Il ilimiz in bilgi.objeler)
            {
                ddlIl.Items.Add(new ListItem(ilimiz.ad, ilimiz.kod.ToString()));
            }

            ddlIl_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// Parametre olarak verilen ilin ilçelerini sunucudan alır ve ddlIlce DropDownList kontrolüne doldurur.
        /// </summary>
        /// <param name="ilKod">İl kriteri</param>
        private void IlceDoldur(string ilKod)
        {
            ddlIlce.Items.Clear();

            Ilce ilce = new Ilce();
            ilce.ilKodu = ilKod;

            ObjectArray bilgi = servisTMM.IlceListele(kullanan, ilce);

            ddlIlce.Items.Add(new ListItem("", ""));
            foreach (Ilce ilcemiz in bilgi.objeler)
            {
                ddlIlce.Items.Add(new ListItem(ilcemiz.ad, ilcemiz.kod.ToString()));
            }
        }

        /// <summary>
        /// İl seçimi değiştiğinde çalışan olay metodu
        /// Seçilen ile ait ilçeler sayfadaki ilgili kontrole doldurulur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void ddlIl_SelectedIndexChanged(object sender, EventArgs e)
        {
            IlceDoldur(ddlIl.SelectedValue);
        }

        /// <summary>
        /// Kaydet tuşuna basılınca çalışan olay metodu
        /// Ekrandan seçilen ihtiyaç fazlası taşınır demirbaşları toplanıp istek olarak
        /// kaydedilmek üzere sunucuya gönderilir, gelen sonuca göre hata varsa hata görüntülenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            TNS.TMM.AcikPazarForm apf = (TNS.TMM.AcikPazarForm)Session["listeKriter"];
            TNSCollection istekler = KriterTopla(apf);
            if (istekler.Count > 0)
            {
                Sonuc sonuc = servisTMM.AcikPazarIstekKaydet(kullanan, apf, istekler);
                if (sonuc.islemSonuc)
                    GenelIslemler.BilgiYaz(this, sonuc.bilgiStr);
                else
                    GenelIslemler.HataYaz(this, sonuc.hataStr);
            }
        }

        /// <summary>
        /// Ekrandan seçilen ihtiyaç fazlası taşınır demirbaşlarını toplayıp döndüren yordam
        /// </summary>
        /// <param name="apf">İstek yapan birim bilgilerini tutan nesne</param>
        /// <returns>Taşınır demirbaşlarına yapılacak istek bilgileri listesini tutan nesne</returns>
        private TNSCollection KriterTopla(TNS.TMM.AcikPazarForm apf)
        {
            TNSCollection istekler = new TNSCollection();
            for (int i = 0; i < dgListe.Items.Count; i++)
            {
                CheckBox chk = (CheckBox)dgListe.Items[i].FindControl("chkSecim");
                if (chk.Checked)
                {
                    TNS.TMM.AcikPazarIstek istek = new TNS.TMM.AcikPazarIstek();
                    istek.muhasebeKod = dgListe.Items[i].Cells[6].Text.Trim();
                    istek.muhasebeAd = dgListe.Items[i].Cells[7].Text.Trim();
                    istek.harcamaKod = dgListe.Items[i].Cells[8].Text.Trim();
                    istek.harcamaAd = dgListe.Items[i].Cells[9].Text.Trim();
                    istek.prSicilNo = OrtakFonksiyonlar.ConvertToInt(dgListe.Items[i].Cells[1].Text.Trim(), 0);
                    istek.gorSicilNo = dgListe.Items[i].Cells[2].Text.Trim();
                    istek.istekYapanMuhasebeKod = apf.istekYapanMuhasebeKod;
                    istek.istekYapanMuhasebeAd = apf.istekYapanMuhasebeAd;
                    istek.istekYapanHarcamaKod = apf.istekYapanHarcamaKod;
                    istek.istekYapanHarcamaAd = apf.istekYapanHarcamaAd;
                    istek.istekTarihi.Yaz(DateTime.Now);

                    istekler.Add(istek);
                }
            }

            if (istekler.Count <= 0)
                GenelIslemler.HataYaz(this, Resources.TasinirMal.FRMAPI002);

            return istekler;
        }

        /// <summary>
        /// Listele tuşuna basılınca çalışan olay metodu
        /// Sunucudan ihtiyaç fazlası taşınır ve istek bilgileri alınır ve listelenir.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            Listele();
        }

        /// <summary>
        /// Listeleme kriterleri ihtiyaç fazlası taşınır nesnesine doldurulur ve sunucuya gönderilir
        /// ve ihtiyaç fazlası taşınır ve istek bilgileri sunucudan alınır. Hata varsa ekrana hata bilgisi yazılır,
        /// yoksa gelen ihtiyaç fazlası taşınır ve istek bilgileri dgListe DataGrid kontrolüne doldurulur.
        /// </summary>
        private void Listele()
        {
            TNS.TMM.AcikPazarForm apfKriter = new TNS.TMM.AcikPazarForm();
            apfKriter.istekYapanMuhasebeKod = txtIYMuhasebe.Text.Trim();
            apfKriter.istekYapanMuhasebeAd = lblIYMuhasebeAd.Text.Trim();
            apfKriter.istekYapanHarcamaKod = txtIYHarcamaBirimi.Text.Trim();
            apfKriter.istekYapanHarcamaAd = lblIYHarcamaBirimiAd.Text.Trim();
            apfKriter.hesapKod = txtHesapPlanKod.Text.Trim();
            apfKriter.hesapAd = lblHesapPlanAd.Text.Trim();
            apfKriter.ilKod = ddlIl.SelectedValue.Trim();
            apfKriter.ilAd = ddlIl.SelectedItem.Text.Trim();
            apfKriter.ilceKod = ddlIlce.SelectedValue.Trim();
            apfKriter.ilceAd = ddlIlce.SelectedItem.Text.Trim();
            apfKriter.muhasebeKod = txtMuhasebe.Text.Trim();
            apfKriter.muhasebeAd = lblMuhasebeAd.Text.Trim();
            apfKriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            apfKriter.harcamaAd = lblHarcamaBirimiAd.Text.Trim();

            ObjectArray apfVan = servisTMM.AcikPazarListele(kullanan, apfKriter, true);

            if (!apfVan.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, apfVan.sonuc.hataStr);
                dgListe.DataBind();
                lblKacKayit.Visible = false;
                return;
            }

            if (apfVan.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, apfVan.sonuc.bilgiStr);
                dgListe.DataBind();
                lblKacKayit.Visible = false;
                return;
            }

            Session.Add("listeKriter", apfKriter);

            DataTable table = new DataTable();
            table.Columns.Add("prSicilNo");
            table.Columns.Add("sicilNo");
            table.Columns.Add("hesapAd");
            table.Columns.Add("aciklama");
            table.Columns.Add("eklenisTarihi");
            table.Columns.Add("muhasebeKod");
            table.Columns.Add("muhasebeAd");
            table.Columns.Add("harcamaKod");
            table.Columns.Add("harcamaAd");

            TNS.TMM.AcikPazarForm apf = (TNS.TMM.AcikPazarForm)apfVan.objeler[0];
            foreach (TNS.TMM.AcikPazarDetay detay in apf.detaylar)
            {
                table.Rows.Add(detay.prSicilNo, detay.gorSicilNo, detay.hesapAd, detay.aciklama, detay.eklenisTarihi.ToString(),
                               detay.muhasebeKod, detay.muhasebeAd, detay.harcamaKod, detay.harcamaAd);
            }

            dgListe.DataSource = table;
            dgListe.DataBind();

            int satir = 0;
            foreach (TNS.TMM.AcikPazarDetay detay in apf.detaylar)
            {
                CheckBox chk = (CheckBox)dgListe.Items[satir].FindControl("chkSecim");
                chk.Checked = detay.istekVar;
                satir++;
            }

            lblKacKayit.Visible = true;
            lblKacKayit.Text = string.Format(Resources.TasinirMal.FRMAPI003, apf.detaylar.Count.ToString());
        }
    }
}