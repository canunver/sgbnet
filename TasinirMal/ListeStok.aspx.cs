using System;
using System.Data;
using OrtakClass;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr sarf malzemeleri listesinin verilen kriterlere göre döndürülüp listelendiði sayfa
    /// </summary>
    public partial class ListeStok : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur, yoksa giriþ ekranýna yönlendirilir varsa sayfa yüklenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            TasinirGenel.JSResourceEkle_Ortak(this);
            GenelIslemlerIstemci.JSResourceEkle(Resources.TasinirMal.ResourceManager, this, "ListeStok", "FRMJSC010", "FRMJSC017", "FRMJSC018", "FRMJSC019");

            //Giriþ sýrasýnda kullanýcýnýn varlýðýný kontrol et yoksa sayfaya girme
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            this.txtHesapPlaniKodu.Attributes.Add("onblur", "kodAdGetir('30','lblHesapPlanAd',true,new Array('txtHesapPlaniKodu'),'KONTROLDENOKU');");

            if (!IsPostBack)
            {
                if (Request.QueryString["hpk"] != null) //hpk -> Hesap Planý Kodu
                    txtHesapPlaniKodu.Text = Request.QueryString["hpk"].Trim();
            }
        }

        /// <summary>
        /// Sayfa adresinde gelen girdi dizgilerindeki ve sayfadaki kontrollerdeki kriterler toplanýr
        /// ve kriterlere uygun olan sarf malzemeleri listelenmek üzere StokDoldur yordamý çaðýrýlýr.
        /// </summary>
        void KriterTopla()
        {
            StokHareketBilgi shBilgi = new StokHareketBilgi();

            int islemTur = 0;

            if (Request.QueryString["mb"] != null) //mb -> Muhasebe Birimi Kodu
                shBilgi.muhasebeKod = Request.QueryString["mb"].Trim();

            if (Request.QueryString["hbk"] != null) //hbk -> Harcama Birimi Kodu
                shBilgi.harcamaKod = Request.QueryString["hbk"].Trim().Replace(".", "");

            if (Request.QueryString["yil"] != null) //Yoruma gerek var mý
                shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0);

            if (Request.QueryString["ak"] != null) //ak -> Ambar Kodu
                shBilgi.ambarKod = Request.QueryString["ak"].Trim().Replace(".", "");

            if (Request.QueryString["it"] != null) //islem turu (satin alma, hurda cikis vs.)
                islemTur = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["it"].Trim().Replace(".", ""), 0);

            if (txtHesapPlaniKodu.Text.Trim() != "")
                shBilgi.hesapPlanKod = txtHesapPlaniKodu.Text.Trim().Replace(".", "");
            else
            {
                if (txtHesapPlaniAdi.Text.Trim() != "")
                    shBilgi.hesapPlanAd = txtHesapPlaniAdi.Text.Trim().Replace(".", "");
            }
            StokDoldur(shBilgi);
        }

        /// <summary>
        /// Parametre olarak verilen kriterlere uyan sarf malzemelerini sayfadaki GridView kontrolüne dolduran yordam
        /// </summary>
        /// <param name="shBilgi">Sarf malzemeleri listeleme kriter bilgilerini tutan nesne</param>
        /// <param name="islemTur">Listeleme iþlem türü</param>
        private void StokDoldur(StokHareketBilgi shBilgi)
        {
            ObjectArray bilgi = servisTMM.TuketimListele(kullanan, shBilgi);

            if (bilgi.sonuc.islemSonuc)
            {
                if (bilgi.objeler.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("kod");
                    dt.Columns.Add("ad");
                    dt.Columns.Add("miktar");
                    dt.Columns.Add("kdvOran");
                    dt.Columns.Add("birimFiyat");

                    foreach (StokHareketBilgi sBilgi in bilgi.objeler)
                        dt.Rows.Add("<a href='#' onclick=\"VeriGoster('" + sBilgi.hesapPlanKod + "');return false;\">" + sBilgi.hesapPlanKod + "</a>", sBilgi.hesapPlanAd, sBilgi.miktar, sBilgi.kdvOran, sBilgi.birimFiyat);

                    dt.Columns[0].ColumnName = Resources.TasinirMal.FRMLST002;
                    dt.Columns[1].ColumnName = Resources.TasinirMal.FRMLST003;
                    dt.Columns[2].ColumnName = Resources.TasinirMal.FRMLST004;
                    dt.Columns[3].ColumnName = Resources.TasinirMal.FRMLST005;
                    dt.Columns[4].ColumnName = Resources.TasinirMal.FRMLST006;

                    gvTuketimler.DataSource = dt;
                    gvTuketimler.DataBind();

                    for (int i = 0; i < gvTuketimler.Rows.Count; i++)
                        gvTuketimler.Rows[i].Cells[0].Text = Server.HtmlDecode(gvTuketimler.Rows[i].Cells[0].Text);
                }
                else
                    GenelIslemler.MesajKutusu(this, Resources.TasinirMal.FRMLST007);
            }
            else
                GenelIslemler.MesajKutusu(this, bilgi.sonuc.hataStr);
        }

        /// <summary>
        /// Listele tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr sarf malzemeleri listelensin diye KriterTopla yordamýný çaðýrýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, EventArgs e)
        {
            gvTuketimler.DataBind();
            KriterTopla();
        }
    }
}