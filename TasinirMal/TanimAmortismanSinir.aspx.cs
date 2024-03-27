using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;

namespace TasinirMal
{
    /// <summary>
    /// Amortisman alt sınır tutarının kayıt, silme ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TanimAmortismanSinir : TMMSayfaV2
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
            formAdi = Resources.TasinirMal.FRMTAS001;
            //Sayfaya giriş izni varmı?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);
            SayfaUstAltBolumYaz(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

            if (!IsPostBack)
            {
                Listele();
            }
        }

        ///// <summary>
        ///// Kaydet tuşuna basılınca çalışan olay metodu
        ///// Amortisman alt sınır tutarı kaydedilmek üzere sunucuya gönderilir,
        ///// gelen sonuca göre hata veya bilgi mesajı görüntülenir.
        ///// Son olarak güncel bilgilerin görünmesi için listeleme işlemi yapılır.
        ///// </summary>
        ///// <param name="yil">Yıl</param>
        ///// <param name="sinir">Amortşsman üst sınırı</param>
        [DirectMethod]
        public void YilGuncelle(int yil, double sinir, double sinirTasit, string mifNo)
        {
            AmortismanSinir sinir1 = new AmortismanSinir();
            sinir1.yil = yil;
            sinir1.sinir = sinir;
            sinir1.mifBelgeNo = mifNo;
            sinir1.sinirTasit = sinirTasit;
            Sonuc sonuc = servisTMM.AmortismanSinirKaydet(kullanan, sinir1);

            if (sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTAS002);
            else
                GenelIslemler.MesajKutusu("Uyarı", sonuc.hataStr);
        }

        public int SaymalikKod
        {
            get
            {
                if (string.IsNullOrWhiteSpace(cmbMuhasebe.Text))
                {
                    return 0;
                }
                else
                {
                    string kod = cmbMuhasebe.Text.Split('-')[0];
                    int iKod = OrtakFonksiyonlar.ConvertToInt(kod, 0);
                    return iKod;
                }
            }
        }

        [DirectMethod]
        public void AmortismanUret(int yil)
        {
            this.yil = yil;
            islemGorenAmbar = 0;
            islemGorecekAmbar = -1;
            durdur = false;
            hata = "";
            eksiBirSay = 0;

            ResourceManager1.AddScript("{0}.startTask('IslemGostergec');", TaskManager1.ClientID);
            try
            {
                if (!System.Threading.ThreadPool.QueueUserWorkItem(GirisIsle))
                {
                    hata = "Process başlatılamadı";
                    Durdur();
                }
            }
            catch (Exception ex)
            {
                hata = ex.Message;
                Durdur();
            }
        }

        private void Durdur()
        {
            //ResourceManager1.AddScript("{0}.stopTask('IslemGostergec');", TaskManager1.ClientID);
            //if(hata=="")
            //    Progress1.UpdateProgress(1, "İşlem tamamlandı.");
            //else
            //    Progress1.UpdateProgress(1, hata);
            islemGorenAmbar = 999999;
            if (islemGorecekAmbar > -1)
                islemGorecekAmbar = -1;
            else
                islemGorecekAmbar -= 1;
        }

        [DirectMethod]
        public void AmortismanYaz(int yil, bool kumulatif)
        {
            try
            {
                System.Collections.Hashtable raporParam = new System.Collections.Hashtable();

                raporParam["yil"] = yil.ToString();
                raporParam["muhasebeKod"] = SaymalikKod.ToString("00000");
                raporParam["harcamaKod"] = "";
                raporParam["ambarKod"] = "";
                raporParam["hesapPlanKod"] = "";
                raporParam["donem"] = "12";
                raporParam["raporTur"] = "4";
                raporParam["kumulatif"] = (kumulatif ? "1" : "0");

                TNS.Raporlama.RaporlamaServis.TekRaporAl(9000, 3, kullanan, null, raporParam);
            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu(this, ex.Message);
            }
        }

        [DirectMethod]
        public void AmortismanMIFUret(int yil, string mifBelgeNo)
        {
            try
            {
                Sonuc s = AmortismanRapor.AmortismanMIFUret(kullanan, yil, SaymalikKod, mifBelgeNo);
                if (s.islemSonuc)
                {
                    X.Js.AddScript("OdemeEmriNoYaz('{0}')", s.anahtar);
                }
                else
                    GenelIslemler.MesajKutusu("Hata", s.hataStr);
            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu(this, ex.Message);
            }
        }

        static int islemGorenAmbar = 0;
        static int islemGorecekAmbar = -1;
        static bool durdur = false;
        static string ambarAd = "";
        int yil;
        static string hata = "";
        static int eksiBirSay = 0;

        [DirectMethod]
        public void btnDurdur_Click()
        {
            durdur = true;
        }

        protected void RefreshProgress(object sender, DirectEventArgs args)
        {
            if (islemGorecekAmbar == -1 && islemGorenAmbar == 0)
            {
                eksiBirSay++;
                if (eksiBirSay > 5)
                {
                    eksiBirSay = 0;
                    islemGorecekAmbar = -2;
                }
                lblToplamAmbar.Text = "0";
                lblAmbarSayac.Text = "0";
                lblKalanAmbar.Text = "0";

                Progress1.UpdateProgress(0, "İşlem yapılacak ambarlar belirleniyor.");
            }
            else if (islemGorenAmbar < islemGorecekAmbar)
            {
                lblToplamAmbar.Text = islemGorecekAmbar.ToString("#,###");
                lblAmbarSayac.Text = islemGorenAmbar.ToString("#,###");
                lblKalanAmbar.Text = (islemGorecekAmbar - islemGorenAmbar).ToString("#,###");
                lblIslemYapilanAmbar.Text = ambarAd;
                Progress1.UpdateProgress(islemGorenAmbar / (float)islemGorecekAmbar, string.Format("% {0}", ((islemGorenAmbar / (float)islemGorecekAmbar) * 100).ToString("###")));
            }
            else
            {
                if (hata == "") hata = "Amortisman kayıtları oluşturuldu.";
                ResourceManager1.AddScript("Bitir('{0}');", GenelIslemler.HataBilgisiniHMTLYap(hata));
            }
        }

        /// <summary>
        /// Parametre olarak verilen yıl kriteri sunucuya gönderilir ve amortisman alt sınır
        /// tutar bilgileri alınır. Hata varsa ekrana hata bilgisi yazılır, yoksa gelen
        /// amortisman alt sınır tutar bilgileri dgListe GridView kontrolüne doldurulur.
        /// </summary>
        private void Listele()
        {
            ObjectArray listeVAN = servisTMM.AmortismanSinirListele(kullanan, 0);
            if (!listeVAN.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", listeVAN.sonuc.hataStr);
            }
            stoYillar.DataSource = listeVAN.objeler;
            stoYillar.DataBind();
        }

        void GirisIsle(object state)
        {
            //Bütün harcama birimlerini ve ambarları listele
            Ambar ambar = new Ambar();
            ambar.muhasebeKod = SaymalikKod.ToString("00000");
            ambar.harcamaBirimKod = "";
            ambar.kod = "";

            ObjectArray bilgi = servisTMM.AmbarListele(kullanan, ambar);
            //foreach ile ambarları dolaş
            islemGorecekAmbar = bilgi.objeler.Count;
            islemGorenAmbar = 0;
            AmortismanKriter form = new AmortismanKriter();
            form.yil = yil;
            form.donem = 12;
            if (bilgi.ObjeSayisi == 0)
                hata = "İşlem yapılacak ambar bulunamadı!";
            foreach (Ambar a in bilgi.objeler)
            {
                ambarAd = a.harcamaBirimAd + "-" + a.ad;
                form.muhasebeKod = a.muhasebeKod;
                form.harcamaKod = a.harcamaBirimKod;
                form.ambarKod = a.kod;
                form.raporTur = 3; //Amortisman yapılacaklar
                form.hesapPlanKod = "";
                Sonuc sonuc = servisTMM.AmortismanVerisiOlustur2(kullanan, form);
                if (!sonuc.islemSonuc)
                {
                    //durdur = true;
                    hata += ";" + sonuc.hataStr;
                }
                islemGorenAmbar++;
                if (durdur)
                    break;
            }
            islemGorenAmbar = islemGorecekAmbar;
            Durdur();
        }
    }
}