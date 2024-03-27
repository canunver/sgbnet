using System;
using System.Web;
using OrtakClass;
using TNS;
using TNS.DEG;
using TNS.KYM;
using System.IO;
using System.Diagnostics;
using Ext1.Net;

namespace OrtakSayfa
{
    public partial class Giris : istemciUzayi.GenelGiris
    {
        //public static bool tabloLogoOlustu = false; 

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (TNS.HRC.Arac.CumhurbaskanligiMi())
            //{
            //    this.Response.Redirect("GirisCB.aspx");
            //    return;
            //}

            //if (IsTabletOrMobile(this.Request))
            //{
            //    int girisSayfasiTek = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/GirisSayfasiTek") + "", 0);
            //    if (girisSayfasiTek == 0)
            //        Response.Redirect("GirisIc.aspx");
            //}

            //MesajYaz(3);
            PageLoad(this);
            //MesajYaz(4);

            if (!string.IsNullOrEmpty(sistemUyarisi))
            {
                wndBilgi.Hidden = false;
                dspBilgi.Html = sistemUyarisi;
            }
            else
                wndBilgi.Hidden = true;

            hdnIkiAsamaliGiris.Text = this.ikiAsamaliGiris;

            if (aktifDil != "tr")
                divSolMenu.Visible = false;

            if (GenelIslemlerIstemci.SifresizBolumYok())
            {
                divSolMenu.Visible = false;
                frmGiris.Style["margin-left"] = "245px";
            }
            else
                divSolMenu.Visible = true;

            //MesajYaz(6);
            string logoYok = TNS.DEG.Arac.DegiskenDegerBul(DateTime.Now.Year, "/LOGOYOK");
            if (!string.IsNullOrEmpty(logoYok) && logoYok == "1")
                divUstMenu.Visible = false;
            else
                divUstMenu.Visible = true;
            int girisDenemeSayisi = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/GirisDenemeSayi") + "", 0);
            if (girisDenemeSayisi > 0)
            {
                int hataliGiris = GirisDenemeSayisiKontrolu();
                if (hataliGiris >= girisDenemeSayisi)
                    hdnGuvenlikResmi.Hidden = false;
            }
            if (!IsPostBack)
            {
                DilLinkEkle();
                if (OrtakFonksiyonlar.ConvertToInt(GenelIslemler.WebConfigOku("Entegrasyon", "0"), 0) == 0)//entegrasyon yok
                    Session.Abandon();

                string ldapAdres = TNS.DEG.Arac.DegiskenDegerBul(0, "/LDAP");
                //LDAP ise parolamı unuttum olmasın
                if (!string.IsNullOrWhiteSpace(ldapAdres))
                {
                    lnkUnuttum.Hidden = true;
                }

                //////sürüm
                //////****************************
                ////System.Reflection.Assembly sistemBilgi = System.Reflection.Assembly.GetExecutingAssembly();
                ////System.Version AppVersion = sistemBilgi.GetName().Version;

                ////lblSurum.Text = AppVersion.Major.ToString()
                ////+ "." + AppVersion.Minor.ToString()
                ////+ "." + AppVersion.Build.ToString()
                ////+ "." + AppVersion.Revision.ToString();
                //////****************************

                HttpCookie cookKul = Request.Cookies["KulAd"];
                if (cookKul != null)
                {
                    txtKKod.Text = cookKul.Value;
                    chkHatirla.Checked = true;
                    txtParola.Focus(true, 100);
                }
                else
                {
                    txtKKod.Focus(true, 100);
                }

                lblKurumAdi.Text = TNS.DEG.Arac.DegiskenDegerBul(DateTime.Now.Year, "/KURUMAD");

                SurumBilgisi();

                bool sifresizGoster = false;
                trEvrak.Visible = false;
                if (OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("zimmetLinkiGoster") + "", 0) == 0)
                    trZimmet.Visible = false;
                else
                {
                    trZimmet.Visible = true;
                    sifresizGoster = true;
                }

                if (OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("zimmetePostaOnayLinkiGoster") + "", 0) == 0)
                    trZimmetePostaOnay.Visible = false;
                else
                {
                    trZimmetePostaOnay.Visible = true;
                    sifresizGoster = true;
                }

                if (OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("tasinirTalepLinkiGoster") + "", 1) == 0)
                    trTalepKarsilama.Visible = false;
                else
                {
                    trTalepKarsilama.Visible = true;
                    sifresizGoster = true;
                }

                if (OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("kutuphaneLinkiGoster") + "", 0) == 0)
                    trKutuphane.Visible = false;
                else
                {
                    trKutuphane.Visible = true;
                    sifresizGoster = true;
                }

                if (OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("raporLinkiGoster") + "", 0) == 0)
                    divRaporBolum.Visible = false;
                else
                    divRaporBolum.Visible = true;

                if (OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("yollukLinkiGoster") + "", 0) == 0)
                {
                    trYolluk.Visible = false;
                    trYollukDEG.Visible = false;
                    trYollukYDG.Visible = false;
                    trYollukYDS.Visible = false;
                    trYollukYIG.Visible = false;
                    trYollukYIS.Visible = false;
                }
                else
                {
                    trYolluk.Visible = true;
                    trYollukDEG.Visible = true;
                    trYollukYDG.Visible = true;
                    trYollukYDS.Visible = true;
                    trYollukYIG.Visible = true;
                    trYollukYIS.Visible = true;
                    sifresizGoster = true;
                }

                if (OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("izinLinkiGoster") + "", 1) == 0)
                    trIzin.Visible = false;
                else
                {
                    trIzin.Visible = true;
                    sifresizGoster = true;
                }

                if (sifresizGoster)
                    divSifresizBolum.Visible = true;
                else
                    divSifresizBolum.Visible = false;

                if (OrtakFonksiyonlar.ConvertToInt(GenelIslemler.WebConfigOku("Entegrasyon", "0"), 0) > 0)//entegrasyon yok
                {
                    string kullaniciAdi = "";
                    string sessionId = "";

                    if (!string.IsNullOrEmpty(Request.QueryString["kullaniciAdi"]))
                        kullaniciAdi = Request.QueryString["kullaniciAdi"].ToString();
                    if (!string.IsNullOrEmpty(Request.QueryString["sessionId"]))
                        sessionId = Request.QueryString["sessionId"].ToString();
                    if (!string.IsNullOrEmpty(kullaniciAdi)
                        && !string.IsNullOrEmpty(sessionId))
                    {
                        OturumBilgisiIslem.BilgiYazDegisken("kullaniciAdi", kullaniciAdi);
                        OturumBilgisiIslem.BilgiYazDegisken("sessionId", sessionId);
                        GirisYap(kullaniciAdi, sessionId, true);
                    }
                }

                //**ht:Logo İçin 
                /*if (!tabloLogoOlustu)
                {
                    TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
                    Sonuc sonuc = servisKYM.LogoTabloOlustur(null);
                    tabloLogoOlustu = true;
                }*/

            }
        }

        //protected void MesajYaz(int siraNo)
        //{
        //    if (this.Request.UserHostAddress == "172.17.15.107" || this.Request.UserHostAddress == "::1")
        //        OrtakFonksiyonlar.HataDosyaYaz("Giris.txt", siraNo.ToString() + "::" + SaatYap(DateTime.Now), false);
        //}

        private string SaatYap(DateTime n)
        {
            return string.Format("{0:00}.{1:00}.{2:00}.{3:000} {4}", n.Hour, n.Minute, n.Second, n.Millisecond, this.Request.UserHostAddress);
        }

        protected void btnIkinciKodYenile_Click(object sender, System.EventArgs e)
        {
            IkinciKodYenile(true);
        }

        private void IkinciKodYenile(bool yenilemeyiZorla)
        {
            Kullanici kullanici = (Kullanici)OturumBilgisiIslem.BilgiOkuDegisken("IkinciKodKullanici", false);
            if (kullanici == null)
            {
                GenelIslemler.HataYaz(this, "Zaman aşımı oluştu, lütfen kod giriş ekranını kapatıp, tekrar giriş yapınız!");
                return;
            }
            string ikinciParola;
            if (yenilemeyiZorla || YenilensinMi(kullanici, out ikinciParola))
            {
                string tut = IkinciParolaUret();
                IkinciParolaMesajiAt(kullanici, tut);
                GenelIslemler.KullaniciDegiskenSakla(kullanici, "IkinciParola", tut + ";" + DateTime.Now.Ticks);
            }
        }

        private void IkinciParolaMesajiAt(Kullanici kullanici, string tut)
        {
            string mesaj = $"SGB.Net sistemine giriş kodunuz: <bold>{tut}</bold><br /> Bu kod 8 saat süre ile geçerli olacaktır, bu süre zarfında yapacağınız girişlerde aynı kodu kullanabilirsiniz. Bu mesajı kaybeder ve kodu unutursanız, giriş ekranındaki yenile tuşuna basarak yeniden mesaj alabilirsiniz!";
            TNS.KYM.Arac.Tanimla().EPostaGonder("", kullanici.EPOSTA, "SGB.Net Doğrulama Postası", mesaj, true, false, null);
        }

        private string IkinciParolaUret()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            return (100000 + random.Next(900000)).ToString();
        }

        private bool YenilensinMi(Kullanici kullanici, out string ikinciParola)
        {
            DateTime ikinciParolaZaman;
            IkinciParolaOku(kullanici, out ikinciParola, out ikinciParolaZaman);
            TimeSpan ts = DateTime.Now - ikinciParolaZaman;
            if (ts.TotalHours > 8)
                return true;
            else
                return false;
        }

        private void IkinciParolaOku(Kullanici kullanici, out string ikinciParola, out DateTime ikinciParolaZaman)
        {
            ikinciParola = "";
            ikinciParolaZaman = DateTime.MinValue;
            string parolaVeZaman = GenelIslemler.KullaniciDegiskenGetir(kullanici.kullaniciKodu, "IkinciParola");
            if (!string.IsNullOrWhiteSpace(parolaVeZaman))
            {
                string p1 = "";
                string p2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(parolaVeZaman, ";", ref p1, ref p2);
                ikinciParola = p1;
                if (!string.IsNullOrWhiteSpace(p2))
                {
                    try
                    {
                        ikinciParolaZaman = new DateTime(OrtakFonksiyonlar.ConvertToLong(p2));
                    }
                    catch { }
                }
            }
        }

        protected void btnGiris_Click(object sender, System.EventArgs e)
        {
            string tekKullanici = TNS.DEG.Arac.DegiskenDegerBul(0, "/TEKKULLANICI");
            if (!string.IsNullOrEmpty(tekKullanici) && tekKullanici != txtKKod.Text)
            {
                GenelIslemler.HataYaz(this, "Sistem bakıma alınmıştır. Lütfen daha sonra deneyiniz.");
                lblHata.Hidden = false;
            }
            else
                GirisYap(txtKKod.Text, txtParola.Text, false);
        }

        private KullaniciVAN SSOGiris()
        {
            string ssoLink = TNS.DEG.Arac.DegiskenDegerBul(0, "/SSO2");

            if (string.IsNullOrEmpty(ssoLink)) return null;
            string[] ssoParca = ssoLink.Split(',');
            if (ssoParca.Length < 2) return null;

            string dllAd = ssoParca[0];
            string nesneAd = ssoParca[1];
            try
            {
                System.Runtime.Remoting.ObjectHandle oh = Activator.CreateInstance(dllAd, nesneAd);
                if (oh != null)
                {
                    TNS.KYM.ITekNoktadanGiris ob = (TNS.KYM.ITekNoktadanGiris)oh.Unwrap();
                    return ob.GirisYap(txtKKod.Text, txtParola.Text, null);
                }
            }
            catch { }
            return null;
        }

        [DirectMethod]
        public Sonuc ParolaDogruMu(string kullaniciKodu, string parola, string guvenlikKodu)
        {
            string guvenlikKoduKontrol = (string)OturumBilgisiIslem.BilgiOkuDegisken("guvenlikKodu", false);

            if (guvenlikKoduKontrol != guvenlikKodu && hdnGuvenlikResmi.Hidden == false)
            {
                return new Sonuc("Güvenlik kodu yanlış!");
            }

            KullaniciVAN kv = servisKym.GirisYapabilirMi(kullaniciKodu, parola, false);

            if (!kv.sonuc.islemSonuc)
            {
                OturumBilgisiIslem.BilgiYazDegisken("IkinciKodKullanici", null);

                int girisDenemeSayisi = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/GirisDenemeSayi") + "", 0);
                if (girisDenemeSayisi > 0)
                {
                    int hataliGiris = GirisDenemeSayisiKontrolu() + 1;
                    if (hataliGiris >= girisDenemeSayisi)
                        hdnGuvenlikResmi.Hidden = false;
                }
            }
            else
            {
                OturumBilgisiIslem.BilgiYazDegisken("IkinciKodKullanici", kv.kullanici);
                IkinciKodYenile(false);
            }
            return kv.sonuc;
        }

        private void GirisYap(string kullaniciID, string sessionIDsifre, bool sessiondan)
        {
            KullaniciVAN kullanici = new KullaniciVAN();
            string hataKontrol = "";
            bool SSOGirisMi = false;

            string guvenlikKoduKontrol = (string)OturumBilgisiIslem.BilgiOkuDegisken("guvenlikKodu", false);

            if (guvenlikKoduKontrol != txtRegisterGuvenlikKodu.Text && hdnGuvenlikResmi.Hidden == false)
            {
                GenelIslemler.HataYaz(this, "Güvenlik kodu yanlış!");
                return;
            }

            if (sessiondan)
            {
                Kullanici tempKullanan = new Kullanici();
                tempKullanan.kullaniciTipi = new ENUMKullaniciTipi[1];
                tempKullanan.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;
                //hataKontrol += "<br>KullaniciOkuWebServisten-->" + kullaniciID + " parola:" + sessionIDsifre;
                //OrtakFonksiyonlar.HataStrYaz(hataKontrol);

                kullanici = servisKym.KullaniciOkuWebServisten(tempKullanan, kullaniciID, sessionIDsifre, true);
                //OrtakFonksiyonlar.HataStrYaz("döndü");

                if (kullanici.sonuc.islemSonuc)
                {
                    //hataKontrol += "<br>GirisYapSession-->" + kullanici.kullanici.kullaniciKodu + " parola:" + kullanici.kullanici.ad;
                    kullanici.sonuc = servisKym.GirisYapSession(kullanici.kullanici);
                }
                //else
                //    hataKontrol += "<br>Kullanici Web service den Okunamadı-->";
            }
            else
            {
                kullanici = SSOGiris();
                if (kullanici == null)
                {
                    string ipAdres = GenelIslemler.GetIPAddress();
                    kullanici = servisKym.GirisYap(kullaniciID, sessionIDsifre);
                    IkinciKodKontrol(kullanici);
                }
                else
                    SSOGirisMi = true;
            }

            if (kullanici.sonuc.islemSonuc)
            {
                //hataKontrol += "<br>GirisYaptı-->";

                if (chkHatirla.Checked)
                {
                    HttpCookie cookKul = new HttpCookie("KulAd");
                    cookKul.Value = txtKKod.Text.Trim();
                    cookKul.Expires = DateTime.Now.AddYears(1);
                    cookKul.HttpOnly = true;
                    cookKul.Secure = true;
                    Response.Cookies.Add(cookKul);
                }

                HttpCookie cookKul2 = new HttpCookie("KullaniciAdi");
                cookKul2.Value = kullanici.kullanici.kullaniciKodu + "-" + kullanici.kullanici.ADSOYAD;
                cookKul2.Expires = DateTime.Now.AddDays(2);
                cookKul2.HttpOnly = true;
                cookKul2.Secure = true;
                Response.Cookies.Add(cookKul2);

                int girisDenemeSayisi = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/GirisDenemeSayi") + "", 0);
                if (girisDenemeSayisi > 0)
                {
                    servisKym.DegiskenKaydet(GenelIslemler.GetIPAddress(), "OlumsuzGirisSayisi", new TNSCollection());
                }
                Izle(kullaniciID, TNS.KYM.ENUMIzleme.BASARILIGIRIS);
                Yonlendir(kullanici, sessiondan, SSOGirisMi, true);
            }
            else if (OrtakFonksiyonlar.ConvertToInt(GenelIslemler.WebConfigOku("Entegrasyon", "0"), 0) == 0)
            {
                int girisDenemeSayisi = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/GirisDenemeSayi") + "", 0);
                if (girisDenemeSayisi > 0)
                {
                    int hataliGiris = GirisDenemeSayisiKontrolu() + 1;
                    TNSCollection coll = new TNSCollection();
                    coll.Add(hataliGiris.ToString());
                    servisKym.DegiskenKaydet(GenelIslemler.GetIPAddress(), "OlumsuzGirisSayisi", coll);

                    if (hataliGiris >= girisDenemeSayisi)
                        hdnGuvenlikResmi.Hidden = false;
                }

                GenelIslemler.HataYaz(this, kullanici.sonuc.hataStr);
                lblHata.Hidden = false;
                Izle(kullaniciID, TNS.KYM.ENUMIzleme.HATALIGIRIS);
            }
            else if (!string.IsNullOrEmpty(GenelIslemler.WebConfigOku("YonlenecekSayfa", "")))
            {
                OturumBilgisiIslem.BilgiYazDegisken("HataBilgisi", kullanici.sonuc.hataStr + hataKontrol);
                Response.Write("<script>oturum=window.open('OrtakSayfa/BeklenmedikHata.aspx' ,500,500);</script>");
                Response.Write("<script>parent.location.href='" + GenelIslemler.WebConfigOku("YonlenecekSayfa", "") + "';</script>");
            }
        }

        private void Izle(string kullaniciID, ENUMIzleme izlemeKod)
        {
            TNS.KYM.Arac.Tanimla().IzlemeKaydet(GenelIslemler.GetIPAddress(), kullaniciID, izlemeKod, "KYM", Request.RawUrl, null);
        }

        private void IkinciKodKontrol(KullaniciVAN kullanici)
        {
            if (kullanici == null || kullanici.sonuc == null || !kullanici.sonuc.islemSonuc) return;
            if (this.ikiAsamaliGiris == "1" || this.ikiAsamaliGiris == "2")
            {
                string ikinciParola;
                if (!YenilensinMi(kullanici.kullanici, out ikinciParola))
                {
                    if (ikinciParola != txtIniknciKod.Text)
                        kullanici.sonuc.DegerAta("Kontrol kodunu hatalı girdiniz, lütfen tekrar deneyiniz. Unutmuşsanız, yenileyeniniz!");
                }
                else
                    kullanici.sonuc.DegerAta("Kontrol kodu zaman aşımına uğramıştır, lütfen tekrar alınız!");
            }
        }

        private int GirisDenemeSayisiKontrolu()
        {
            TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
            int hataliGiris = 1;
            TNSCollection coll = servisKYM.DegiskenleriListele(GenelIslemler.GetIPAddress(), "OlumsuzGirisSayisi");
            foreach (var item in coll)
            {
                hataliGiris = OrtakFonksiyonlar.ConvertToInt(item, 0);
            }

            return hataliGiris;
        }

        private void DilLinkEkle()
        {
            string yazilimUrl = GenelIslemler.YazilimURLBul(this.Request, false);
            for (int dilNo = 0; dilNo < TNS.BKD.Arac.DillerLength; dilNo++)
            {
                string dil = TNS.BKD.Arac.Diller(dilNo);
                if (!string.IsNullOrEmpty(dil))
                    ltlDil.Text += "<a tabindex=\"-1\" href=\"" + yazilimUrl + "?dil=" + dil + "\">" + "<img src=\"app_themes\\images\\bayrak\\" + dil + ".png\" alt=\"" + dil + "\" />" + "</a>&nbsp;";
            }
        }

        private void SurumBilgisi()
        {
            System.Reflection.Assembly sistemBilgi = System.Reflection.Assembly.GetExecutingAssembly();
            string anaYol = Path.GetDirectoryName(sistemBilgi.CodeBase);
            anaYol = anaYol.Replace("file:\\", "");

            DirectoryInfo dirInfo = new DirectoryInfo(anaYol);
            if (dirInfo.Exists)
            {
                FileInfo[] files = dirInfo.GetFiles("istemci.dll");

                foreach (FileInfo f in files)
                {
                    string dosyaYolu = Path.Combine(anaYol, f.Name);
                    FileVersionInfo fInfo = FileVersionInfo.GetVersionInfo(dosyaYolu);
                    lblSurum.Html = fInfo.FileVersion;// f.LastWriteTime.ToString();
                }
            }
        }

        private string GuvenlikKoduKontrol(string kimlikNo, string guvenlikKodu)
        {
            string guvenlikKoduKontrol = (string)OturumBilgisiIslem.BilgiOkuDegisken("guvenlikKodu", false);
            if (guvenlikKodu.ToUpper() != guvenlikKoduKontrol)
                return "!Güvenlik kodu eşleşmiyor. ";

            return "";
        }
    }
}