using System;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.TMM;
using Ext1.Net;
using System.Threading;
using System.Text;
using System.Data;

namespace TasinirMal
{
    /// <summary>
    /// Ambarlarý yýl bazýnda iþleme kapatma iþleminin yapýldýðý sayfa
    /// </summary>
    public partial class TopluIslem : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        static int aktarilanEvrak = 0;
        static int aktarilacakEvrak = -1;

        static StringBuilder islemSonuc = new StringBuilder();
        static bool islemDevam = true;

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            formAdi = Resources.TasinirMal.FRM000001;
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
            SayfaUstAltBolumYaz(this);

            //Sayfaya giriþ izni varmý?
            GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

            if (!IsPostBack)
            {
                OncedenKayitVarmi();

                GrupIslemListele();
            }
        }

        private void OncedenKayitVarmi()
        {
            TopluIslemForm tt = new TopluIslemForm();
            ObjectArray bilgi = servisTMM.TopluIslemFormListele(kullanan, tt);

            if (bilgi.objeler.Count > 0) return;

            TNS.TMM.TopluIslem t = new TNS.TMM.TopluIslem();
            bilgi = servisTMM.TopluIslemDetayGrupKodListele(kullanan);
            foreach (TopluIslemForm sh in bilgi.objeler)
            {
                sh.aciklama = "Önceden kayýt edilen grup";
                sh.onceliKayitlarinAktarimi = true;
                Sonuc sonuc = servisTMM.TopluIslemFormKaydet(kullanan, sh);
            }
        }

        private void GrupIslemListele()
        {
            TopluIslemForm tt = new TopluIslemForm();
            ObjectArray bilgi = servisTMM.TopluIslemFormListele(kullanan, tt);

            DataTable dt = new DataTable();
            dt.Columns.Add("grupKod");
            dt.Columns.Add("aciklama");
            foreach (TopluIslemForm sh in bilgi.objeler)
            {
                dt.Rows.Add(sh.grupKod, sh.aciklama);

            }

            listeStore.DataSource = dt;
            listeStore.DataBind();
        }

        protected void btnZimmetDus_Click(object sender, DirectEventArgs e)
        {
            IslemBaslat(1);
        }
        protected void btnBirimDevirVer_Click(object sender, DirectEventArgs e)
        {
            IslemBaslat(2);
        }
        protected void btnBirimDevirAl_Click(object sender, DirectEventArgs e)
        {
            IslemBaslat(3);
        }
        protected void btnZimmetVer_Click(object sender, DirectEventArgs e)
        {
            IslemBaslat(4);
        }

        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            TopluIslemForm tf = new TopluIslemForm();
            tf.grupKod = OrtakFonksiyonlar.ConvertToInt(txtGrupKod.Text, 0);
            tf.aciklama = txtAciklama.Text;

            if (tf.grupKod == 0 && btnDosyaYukleExt.PostedFile == null)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Lütfen iþlem yapýlacak excel dosyasýný seçin");
                return;
            }

            string dosyaAd = "";
            if (btnDosyaYukleExt.PostedFile != null && btnDosyaYukleExt.PostedFile.ContentLength > 0)
            {
                dosyaAd = System.IO.Path.GetTempFileName();
                System.Web.HttpPostedFile myFile = btnDosyaYukleExt.PostedFile;
                myFile.SaveAs(dosyaAd);

                tf.detay = ListeHazirla(dosyaAd);
            }

            Sonuc sonuc = servisTMM.TopluIslemFormKaydet(kullanan, tf);
            if (sonuc.islemSonuc)
            {
                GrupIslemListele();
                txtGrupKod.Text = sonuc.anahtar;
                GenelIslemler.MesajKutusu("Bilgi", "Grup Ýþlem Bilgisi baþarýyla kayýt edildi");
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        [DirectMethod]
        public void GrupIslemSil(string grupKod)
        {
            Sonuc sonuc = servisTMM.TopluIslemFormSil(kullanan, OrtakFonksiyonlar.ConvertToInt(grupKod, 0));
            if (sonuc.islemSonuc)
            {
                GrupIslemListele();
                GenelIslemler.MesajKutusu("Bilgi", "Grup Ýþlem Bilgisi baþarýyla silindi");
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        [DirectMethod]
        public void GrupIslemYazdir()
        {
            string grupKod = hdnGrupKod.Text;

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TopluIslem.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            kaynakSatir = satir;

            ObjectArray tler = ListeyiVer();

            foreach (TNS.TMM.TopluIslem tt in tler.objeler)
            {
                XLS.HucreDegerYaz(satir, 0, tt.sicilNo);
                XLS.HucreDegerYaz(satir, 1, tt.kaynakMuh);
                XLS.HucreDegerYaz(satir, 2, tt.kaynakHrc);
                XLS.HucreDegerYaz(satir, 3, tt.kaynakAmb);
                XLS.HucreDegerYaz(satir, 4, tt.kaynakTC);
                XLS.HucreDegerYaz(satir, 5, tt.kaynakOda);

                XLS.HucreDegerYaz(satir, 6, tt.hedefMuh);
                XLS.HucreDegerYaz(satir, 7, tt.hedefHrc);
                XLS.HucreDegerYaz(satir, 8, tt.hedefAmb);
                XLS.HucreDegerYaz(satir, 9, tt.hedefTC);
                XLS.HucreDegerYaz(satir, 10, tt.hedefOda);

                XLS.HucreDegerYaz(satir, 11, tt.zimmetDusBelgeNo);
                XLS.HucreDegerYaz(satir, 12, tt.devirVerBelgeNo);
                XLS.HucreDegerYaz(satir, 13, tt.devirAlBelgeNo);
                XLS.HucreDegerYaz(satir, 14, tt.zimmetVerBelgeNo);

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
            }

            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(sonucDosyaAd, "TopluIslem_" + grupKod + "." + GenelIslemler.ExcelTur(), true, GenelIslemler.ExcelTur());
        }

        [DirectMethod]
        public void GrupIslemBaslat()
        {
            if (OrtakFonksiyonlar.ConvertToInt(hdnGrupKod.Text, 0) == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Lütfen, Ýþlem Grup Numarasý giriniz");
                return;
            }

            ObjectArray tler = ListeyiVer();
            aktarilacakEvrak = tler.objeler.Count;
            lblToplamEvrak.Text = aktarilacakEvrak.ToString("#,##0");
            lblAktarilanEvrak.Text = "0";
            lblKalanEvrak.Text = aktarilacakEvrak.ToString("#,##0");

            grdListe.Hide();
            frmBilgi.Show();
            pnlDugmeler.Show();
            pnlIzleme.Show();
            pnlIslemSonuc.Html = "";
        }

        ObjectArray ListeyiVer()
        {
            TNS.TMM.TopluIslem t = new TNS.TMM.TopluIslem();
            t.grupKod = OrtakFonksiyonlar.ConvertToInt(hdnGrupKod.Text, 0);
            ObjectArray tler = servisTMM.TopluIslemListele(kullanan, t);
            aktarilacakEvrak = tler.objeler.Count;

            return tler;
        }

        ObjectArray ListeHazirla(string sablonAd)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd);

            ObjectArray o = new ObjectArray();
            int satir = 1;
            while (true)
            {
                TNS.TMM.TopluIslem t = new TNS.TMM.TopluIslem();
                t.sicilNo = XLS.HucreDegerAl(satir, 0).Trim();
                t.kaynakMuh = XLS.HucreDegerAl(satir, 1).Trim();
                t.kaynakHrc = XLS.HucreDegerAl(satir, 2).Trim();
                t.kaynakAmb = XLS.HucreDegerAl(satir, 3).Trim();
                t.kaynakTC = XLS.HucreDegerAl(satir, 4).Trim();
                t.kaynakOda = XLS.HucreDegerAl(satir, 5).Trim();
                t.hedefMuh = XLS.HucreDegerAl(satir, 6).Trim();
                t.hedefHrc = XLS.HucreDegerAl(satir, 7).Trim();
                t.hedefAmb = XLS.HucreDegerAl(satir, 8).Trim();
                t.hedefTC = XLS.HucreDegerAl(satir, 9).Trim();
                t.hedefOda = XLS.HucreDegerAl(satir, 10).Trim();

                if (string.IsNullOrEmpty(t.sicilNo))
                    break;

                satir++;
                o.objeler.Add(t);
            }
            XLS.DosyaKapat();

            return o;
        }

        void IslemBaslat(int tur)
        {
            islemDevam = true;
            pnlIslemSonuc.Html = "";
            ResourceManager1.AddScript("{0}.startTask('IslemGostergec');", TaskManager1.ClientID);

            if (tur == 1)
                ThreadPool.QueueUserWorkItem(ZimmetDusmeIsleminiYap);
            else if (tur == 2)
                ThreadPool.QueueUserWorkItem(DevirVermeIsleminiYap);
            else if (tur == 3)
                ThreadPool.QueueUserWorkItem(DevirAlmaIsleminiYap);
            else if (tur == 4)
                ThreadPool.QueueUserWorkItem(ZimmetVermeIsleminiYap);
        }

        void ZimmetDusmeIsleminiYap(object state)
        {
            ZimmetIsleminiYap((int)ENUMZimmetVermeDusme.ZIMMETTENDUSME);
        }

        void ZimmetVermeIsleminiYap(object state)
        {
            ZimmetIsleminiYap((int)ENUMZimmetVermeDusme.ZIMMETVERME);
        }

        void ZimmetIsleminiYap(int islem)
        {
            bool dusme = false;
            if (islem == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME) dusme = true;

            ObjectArray tler = ListeyiVer();

            OrtakFonksiyonlar.HataStrYaz("Dönen liste:" + tler.ObjeSayisi);

            if (tler.objeler.Count == 0)
            {
                aktarilacakEvrak = 0;
                return;
            }

            islemSonuc = new StringBuilder();

            foreach (TNS.TMM.TopluIslem tt in tler.objeler)
            {
                if (tt.zimmetDusBelgeNo != "" && tt.kaynakTC != "" && dusme) aktarilanEvrak++;
                if (tt.zimmetVerBelgeNo != "" && tt.hedefTC != "" && !dusme) aktarilanEvrak++;
            }
            aktarilacakEvrak = aktarilanEvrak;

            OrtakFonksiyonlar.HataStrYaz("aktarilanEvrak:" + aktarilanEvrak);

            tler.objeler.Add(new TNS.TMM.TopluIslem());//dongunun son satýrý için ayrý bir iþlem yapýlmamasý için

            int siraNo = 0;
            string sonIslemYapilan = "";
            string kontrolIslemYapilan = "";
            TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();
            TNS.TMM.ZimmetDetay zd = new ZimmetDetay();
            ObjectArray zdler = new ObjectArray();
            int detaySayac = 0;
            foreach (TNS.TMM.TopluIslem tt in tler.objeler)
            {
                if (dusme)
                {
                    kontrolIslemYapilan = tt.kaynakMuh + tt.kaynakHrc + tt.kaynakAmb + tt.kaynakTC + tt.kaynakOda;
                }
                else
                {
                    kontrolIslemYapilan = tt.hedefMuh + tt.hedefHrc + tt.hedefAmb + tt.hedefTC + tt.hedefOda;
                }

                if (zdler.objeler.Count > 0 && (sonIslemYapilan != kontrolIslemYapilan || detaySayac >= 500))
                {
                    ZimmetFisiKaydet(zf, zdler, tt, dusme);

                    zf = new TNS.TMM.ZimmetForm();
                    zd = new ZimmetDetay();
                    zdler = new ObjectArray();
                    siraNo = 0;
                    detaySayac = 1;
                }

                if (string.IsNullOrEmpty(tt.sicilNo)) continue;
                if (tt.zimmetDusBelgeNo != "" && dusme) continue;
                if (tt.zimmetVerBelgeNo != "" && !dusme) continue;
                if (dusme && string.IsNullOrEmpty(tt.kaynakTC.Trim())) continue;
                if (!dusme && string.IsNullOrEmpty(tt.hedefTC.Trim())) continue;

                detaySayac++;

                //Ýncelenen ürün Zimmettemi kontrolü
                SicilNoHareket sicilNoHareket = new SicilNoHareket();
                sicilNoHareket.sicilNo = tt.sicilNo;

                if (dusme)
                {
                    sicilNoHareket.muhasebeKod = tt.kaynakMuh;
                    sicilNoHareket.harcamaBirimKod = tt.kaynakHrc;
                    sicilNoHareket.ambarKod = tt.kaynakAmb;
                }
                else
                {
                    sicilNoHareket.muhasebeKod = tt.hedefMuh;
                    sicilNoHareket.harcamaBirimKod = tt.hedefHrc;
                    sicilNoHareket.ambarKod = tt.hedefAmb;
                }

                ObjectArray zimSicil = new ObjectArray();

                if (dusme)
                    zimSicil = servisTMM.ZimmetliSicilNoListele(kullanan, sicilNoHareket);
                else
                    zimSicil = servisTMM.SicilNoListele(kullanan, sicilNoHareket, 0, 0);

                OrtakFonksiyonlar.HataStrYaz("zimSicil sayýsý:" + zimSicil.ObjeSayisi);

                int belgeTur = 0;
                string hesapPlanKod = "";
                int prSicilNo = 0;
                int kdv = 0;
                decimal fiyat = 0;
                foreach (SicilNoHareket si in zimSicil.objeler)
                {
                    belgeTur = (int)ENUMZimmetBelgeTur.ZIMMETFISI;
                    if (si.islemTipKod == (int)ENUMZimmetIslemTipi.DTLVERME)
                        belgeTur = (int)ENUMZimmetBelgeTur.DAYANIKLITL;

                    hesapPlanKod = si.hesapPlanKod;
                    prSicilNo = si.prSicilNo;
                    kdv = si.kdvOran;
                    fiyat = si.fiyat;
                }
                if (zimSicil.objeler.Count == 0)
                {
                    islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, BelgeNo:{7}, Hata:{8}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.kaynakTC, tt.kaynakOda, tt.devirVerBelgeNo, "Sicil No bulunamadý");
                    islemSonuc.AppendLine();


                    OrtakFonksiyonlar.HataStrYaz("sicil bulunamadý sayýsý:" + tt.id + tt.sicilNo);

                    continue;
                }
                //*****************************************************

                zf.yil = DateTime.Now.Year;
                zf.fisNo = "";

                if (dusme)
                {
                    zf.muhasebeKod = tt.kaynakMuh;
                    zf.harcamaBirimKod = tt.kaynakHrc;
                    zf.ambarKod = tt.kaynakAmb;
                    zf.nereyeGitti = tt.kaynakOda;
                    zf.kimeGitti = tt.kaynakTC;
                    zf.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME;
                }
                else
                {
                    zf.muhasebeKod = tt.hedefMuh;
                    zf.harcamaBirimKod = tt.hedefHrc;
                    zf.ambarKod = tt.hedefAmb;
                    zf.nereyeGitti = tt.hedefOda;
                    zf.kimeGitti = tt.hedefTC;
                    zf.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETVERME;

                    if (!string.IsNullOrEmpty(tt.hedefOda))
                        belgeTur = (int)ENUMZimmetBelgeTur.DAYANIKLITL;
                }

                zf.belgeTur = belgeTur;
                zf.tip = (int)ENUMZimmetTipi.DEMIRBASCIHAZ;
                zf.islemYapan = kullanan.kullaniciKodu;
                zf.islemTarih = new TNSDateTime(DateTime.Now);
                zf.fisTarih = new TNSDateTime(DateTime.Now);
                zf.dusmeTarih = new TNSDateTime(DateTime.Now);

                zd = new ZimmetDetay();
                siraNo++;
                zd.yil = DateTime.Now.Year;
                zd.fisNo = "";

                zd.muhasebeKod = zf.muhasebeKod;
                zd.harcamaBirimKod = zf.harcamaBirimKod;
                zd.ambarKod = zf.ambarKod;

                zd.ambarAd = tt.id.ToString();//tt satýrýný belge ný ile eþleþtirmek için

                zd.siraNo = siraNo;
                zd.prSicilNo = prSicilNo;
                zd.hesapPlanKod = hesapPlanKod;
                zd.kdvOran = kdv;
                zd.birimFiyat = fiyat;
                zd.rfIdNo = 0;
                zd.gorSicilNo = tt.sicilNo;
                zd.teslimDurum = "";
                zd.belgeTur = belgeTur;

                zdler.objeler.Add(zd);

                aktarilanEvrak++;

                if (dusme)
                    sonIslemYapilan = tt.kaynakMuh + tt.kaynakHrc + tt.kaynakAmb + tt.kaynakTC + tt.kaynakOda;
                else
                    sonIslemYapilan = tt.hedefMuh + tt.hedefHrc + tt.hedefAmb + tt.hedefTC + tt.hedefOda;
            }

            islemDevam = false;
        }

        void ZimmetFisiKaydet(TNS.TMM.ZimmetForm zf, ObjectArray zdler, TNS.TMM.TopluIslem tt, bool dusme)
        {
            Sonuc sonuc = servisTMM.ZimmetFisiKaydet(kullanan, zf, zdler);
            if (sonuc.islemSonuc)
            {
                string belgeNo = sonuc.anahtar;

                //Kayýt edilen fiþ onaylanýyor
                zf.fisNo = belgeNo;
                servisTMM.ZimmetFisiDurumDegistir(kullanan, zf, "Onay");

                //TopluÝþ satýrý kayýt edilen fiþ numarasý ile güncelleniyor

                foreach (TNS.TMM.ZimmetDetay zd in zdler.objeler)
                {
                    TNS.TMM.TopluIslem ttf = new TNS.TMM.TopluIslem();

                    if (dusme)
                        ttf.zimmetDusBelgeNo = belgeNo;
                    else
                        ttf.zimmetVerBelgeNo = belgeNo;
                    ttf.id = OrtakFonksiyonlar.ConvertToInt(zd.ambarAd, 0);

                    sonuc = servisTMM.TopluIslemGuncelle(kullanan, ttf);
                    if (!sonuc.islemSonuc)
                    {
                        if (dusme)
                            islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, BelgeNo:{7}, Hata:{8}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.kaynakTC, tt.kaynakOda, tt.zimmetDusBelgeNo, sonuc.hataStr);
                        else
                            islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, BelgeNo:{7}, Hata:{8}", tt.id, tt.sicilNo, tt.hedefMuh, tt.hedefHrc, tt.hedefAmb, tt.hedefTC, tt.hedefOda, tt.zimmetDusBelgeNo, sonuc.hataStr);

                        islemSonuc.AppendLine();
                    }
                }
            }
            else
            {
                if (dusme)
                    islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, Hata:{7}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.kaynakTC, tt.kaynakOda, sonuc.hataStr);
                else
                    islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, Hata:{7}", tt.id, tt.sicilNo, tt.hedefMuh, tt.hedefHrc, tt.hedefAmb, tt.hedefTC, tt.hedefOda, sonuc.hataStr);
                islemSonuc.AppendLine();
            }
        }

        void DevirVermeIsleminiYap(object state)
        {
            ObjectArray tler = ListeyiVer();

            if (tler.objeler.Count == 0)
            {
                aktarilacakEvrak = 0;
                return;
            }

            IslemTip it = new IslemTip();
            it.tur = (int)ENUMIslemTipi.DEVIRCIKIS;
            ObjectArray itler = servisTMM.IslemTipListele(kullanan, it);
            if (!itler.sonuc.islemSonuc || itler.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMLDV010);
                return;
            }

            islemSonuc = new StringBuilder();

            foreach (TNS.TMM.TopluIslem tt in tler.objeler)
            {
                if (tt.devirVerBelgeNo != "") aktarilanEvrak++;
            }

            tler.objeler.Add(new TNS.TMM.TopluIslem());//dongunun son satýrý için ayrý bir iþlem yapýlmamasý için

            int siraNo = 0;
            string sonIslemYapilan = "";
            string kontrolIslemYapilan = "";
            TNS.TMM.TasinirIslemForm zf = new TNS.TMM.TasinirIslemForm();
            TNS.TMM.TasinirIslemDetay zd = new TNS.TMM.TasinirIslemDetay();
            ObjectArray zdler = new ObjectArray();
            int detaySayac = 0;
            foreach (TNS.TMM.TopluIslem tt in tler.objeler)
            {
                kontrolIslemYapilan = tt.kaynakMuh + tt.kaynakHrc + tt.kaynakAmb;

                if (tt.devirVerBelgeNo != "") continue;

                detaySayac++;

                if (zdler.objeler.Count > 0 && (sonIslemYapilan != kontrolIslemYapilan || detaySayac >= 500))
                {
                    DevirVermeFisiKaydet(zf, zdler, tt);

                    zf = new TNS.TMM.TasinirIslemForm();
                    zd = new TNS.TMM.TasinirIslemDetay();
                    zdler = new ObjectArray();
                    siraNo = 0;
                    detaySayac = 1;
                }

                if (string.IsNullOrEmpty(tt.sicilNo)) continue;

                //Ýncelenen ürün sicil bilgileri kontrolü
                SicilNoHareket sicilNoHareket = new SicilNoHareket();
                sicilNoHareket.sicilNo = tt.sicilNo;
                sicilNoHareket.muhasebeKod = tt.kaynakMuh;
                sicilNoHareket.harcamaBirimKod = tt.kaynakHrc;
                sicilNoHareket.ambarKod = tt.kaynakAmb;

                ObjectArray zimSicil = new ObjectArray();

                zimSicil = servisTMM.SicilNoListele(kullanan, sicilNoHareket);
                //servisTMM.DevirGirisTIFKaydetOnayla(kullanan,)

                string hesapPlanKod = "";
                int kdv = 0;
                decimal fiyat = 0;
                foreach (SicilNoHareket si in zimSicil.objeler)
                {
                    hesapPlanKod = si.hesapPlanKod;
                    kdv = si.kdvOran;
                    fiyat = si.fiyat;
                }
                if (zimSicil.objeler.Count == 0)
                {
                    islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, BelgeNo:{7}, Hata:{8}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.kaynakTC, tt.kaynakOda, tt.devirVerBelgeNo, "Sicil No bulunamadý");
                    islemSonuc.AppendLine();

                    continue;
                }
                //*****************************************************

                zf.yil = DateTime.Now.Year;
                zf.fisNo = "";

                zf.muhasebeKod = tt.kaynakMuh;
                zf.harcamaKod = tt.kaynakHrc;
                zf.ambarKod = tt.kaynakAmb;
                //zf.nereyeGitti = tt.kaynakOda;
                //zf.kimeGitti = tt.kaynakTC;

                zf.islemYapan = kullanan.kullaniciKodu;
                zf.islemTarih = new TNSDateTime(DateTime.Now);
                zf.fisTarih = new TNSDateTime(DateTime.Now);

                zf.gMuhasebeKod = tt.hedefMuh;
                zf.gHarcamaKod = tt.hedefHrc;
                zf.gAmbarKod = tt.hedefAmb;

                //zf.islemTipKod = 8;// (int)ENUMIslemTipi.DEVIRCIKIS;
                zf.islemTipKod = ((IslemTip)itler[0]).kod;
                zf.islemTipTur = ((IslemTip)itler[0]).tur;


                zd = new TasinirIslemDetay();
                siraNo++;
                zd.yil = DateTime.Now.Year;
                zd.fisNo = "";

                zd.muhasebeKod = zf.muhasebeKod;
                zd.harcamaKod = zf.harcamaKod;
                zd.ambarKod = zf.ambarKod;

                zd.eAciklama = tt.id.ToString();//tt satýrýný belge ný ile eþleþtirmek için

                zd.siraNo = siraNo;
                zd.miktar = 1;
                zd.hesapPlanKod = hesapPlanKod;
                zd.kdvOran = kdv;
                zd.birimFiyat = fiyat;
                zd.gorSicilNo = tt.sicilNo;

                zdler.objeler.Add(zd);

                aktarilanEvrak++;

                sonIslemYapilan = tt.kaynakMuh + tt.kaynakHrc + tt.kaynakAmb;
            }

            islemDevam = false;
        }

        void DevirVermeFisiKaydet(TNS.TMM.TasinirIslemForm zf, ObjectArray zdler, TNS.TMM.TopluIslem tt)
        {
            zf.detay = zdler;
            Sonuc sonuc = servisTMM.TasinirIslemFisiKaydet(kullanan, zf);
            if (sonuc.islemSonuc)
            {
                string belgeNo = sonuc.anahtar.Split('-')[0];

                //Kayýt edilen fiþ onaylanýyor
                zf.fisNo = belgeNo;
                servisTMM.TasinirIslemFisiDurumDegistir(kullanan, zf, "Onay");

                //TopluÝþ satýrý kayýt edilen fiþ numarasý ile güncelleniyor

                foreach (TNS.TMM.TasinirIslemDetay zd in zdler.objeler)
                {
                    TNS.TMM.TopluIslem ttf = new TNS.TMM.TopluIslem();

                    ttf.devirVerBelgeNo = belgeNo;
                    ttf.id = OrtakFonksiyonlar.ConvertToInt(zd.eAciklama, 0);

                    sonuc = servisTMM.TopluIslemGuncelle(kullanan, ttf);
                    if (!sonuc.islemSonuc)
                    {
                        islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, BelgeNo:{7}, Hata:{8}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.kaynakTC, tt.kaynakOda, tt.devirVerBelgeNo, sonuc.hataStr);
                        islemSonuc.AppendLine();
                    }
                }
            }
            else
            {
                islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, TC:{5}, Oda:{6}, Hata:{7}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.kaynakTC, tt.kaynakOda, sonuc.hataStr);
                islemSonuc.AppendLine();
            }
        }

        void DevirAlmaIsleminiYap(object state)
        {
            ObjectArray tler = ListeyiVer();

            if (tler.objeler.Count == 0)
            {
                aktarilacakEvrak = 0;
                return;
            }

            islemSonuc = new StringBuilder();

            IslemTip it = new IslemTip();
            it.tur = (int)ENUMIslemTipi.DEVIRGIRIS;
            ObjectArray itler = servisTMM.IslemTipListele(kullanan, it);
            if (!itler.sonuc.islemSonuc || itler.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMLDV010);
                return;
            }

            ObjectArray t2ler = new ObjectArray();

            foreach (TNS.TMM.TopluIslem tt in tler.objeler)
            {
                if (tt.devirVerBelgeNo == "" || tt.devirAlBelgeNo != "") aktarilanEvrak++;//devir yapýlmamýþ belgeleri geç
                if (tt.devirVerBelgeNo == "" || tt.devirAlBelgeNo != "") continue;//devir yapýlmamaýþ belgeleri geç

                string kontrolIslemYapilan = tt.kaynakMuh + tt.kaynakHrc + tt.kaynakAmb + tt.devirVerBelgeNo;
                bool bulundu = false;
                string kayitEdilmisBelgeNo = "";
                foreach (TNS.TMM.TopluIslem tt2 in t2ler.objeler)
                {
                    if (tt2.sicilNo == kontrolIslemYapilan)
                    {
                        bulundu = true;
                        kayitEdilmisBelgeNo = tt2.devirAlBelgeNo;
                    }
                }

                if (!bulundu)
                {
                    TNS.TMM.TasinirIslemForm giris = new TNS.TMM.TasinirIslemForm();
                    giris.yil = DateTime.Now.Year;
                    giris.muhasebeKod = tt.hedefMuh;
                    giris.harcamaKod = tt.hedefHrc;
                    giris.ambarKod = tt.hedefAmb;
                    giris.devirGirisiMi = true;
                    giris.islemTipKod = ((IslemTip)itler[0]).kod;
                    giris.islemTipTur = ((IslemTip)itler[0]).tur;
                    giris.fisTarih.Yaz(DateTime.Now.Date);
                    giris.islemTarih = new TNSDateTime(DateTime.Now);
                    giris.islemYapan = kullanan.kullaniciKodu;

                    TNSCollection cikislar = new TNSCollection();
                    TNS.TMM.TasinirIslemForm cikis = new TNS.TMM.TasinirIslemForm();
                    cikis.yil = DateTime.Now.Year;
                    cikis.muhasebeKod = tt.kaynakMuh;
                    cikis.harcamaKod = tt.kaynakHrc;
                    cikis.ambarKod = tt.kaynakAmb;
                    cikis.fisNo = tt.devirVerBelgeNo;
                    cikislar.Add(cikis);

                    if (cikislar.Count <= 0)
                    {
                        GenelIslemler.MesajKutusu("Uyarý", Resources.TasinirMal.FRMLDV009);
                        return;
                    }

                    string devirAlmaBelgeNo = DevirAlmaFisiKaydet(giris, cikislar, tt);

                    if (devirAlmaBelgeNo != "")
                    {
                        TNS.TMM.TopluIslem t2 = new TNS.TMM.TopluIslem();
                        t2.sicilNo = kontrolIslemYapilan;
                        t2.devirAlBelgeNo = devirAlmaBelgeNo;
                        t2ler.objeler.Add(t2);
                    }
                }
                else
                {
                    tt.devirAlBelgeNo = kayitEdilmisBelgeNo;
                    tt.zimmetVerBelgeNo = "";
                    tt.zimmetDusBelgeNo = "";
                    tt.devirVerBelgeNo = "";

                    Sonuc sonuc = servisTMM.TopluIslemGuncelle(kullanan, tt);
                    if (!sonuc.islemSonuc)
                    {
                        islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, BelgeNo:{5}, Hata:{6}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.devirVerBelgeNo, sonuc.hataStr);
                        islemSonuc.AppendLine();
                    }

                }
            }

            islemDevam = false;
        }

        string DevirAlmaFisiKaydet(TNS.TMM.TasinirIslemForm giris, TNSCollection cikislar, TNS.TMM.TopluIslem tt)
        {
            string belgeNo = "";
            Sonuc sonuc = servisTMM.DevirGirisTIFKaydetOnayla(kullanan, giris, cikislar);
            if (sonuc.islemSonuc)
            {
                belgeNo = sonuc.anahtar;
                tt.devirAlBelgeNo = belgeNo;
                tt.zimmetVerBelgeNo = "";
                tt.zimmetDusBelgeNo = "";
                tt.devirVerBelgeNo = "";

                sonuc = servisTMM.TopluIslemGuncelle(kullanan, tt);
                if (!sonuc.islemSonuc)
                {
                    islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, BelgeNo:{5}, Hata:{6}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, tt.devirVerBelgeNo, sonuc.hataStr);
                    islemSonuc.AppendLine();
                }

            }
            else
            {
                islemSonuc.AppendFormat("ID:{0}, SicilNo:{1}, Muhasebe:{2}, Harcama:{3}, Ambar:{4}, Hata:{5}", tt.id, tt.sicilNo, tt.kaynakMuh, tt.kaynakHrc, tt.kaynakAmb, sonuc.hataStr);
                islemSonuc.AppendLine();
            }

            return belgeNo;
        }

        protected void RefreshProgress(object sender, DirectEventArgs e)
        {
            if (aktarilacakEvrak == -1)
            {
                lblToplamEvrak.Text = "0";
                lblAktarilanEvrak.Text = "0";
                lblKalanEvrak.Text = "0";

                Progress1.UpdateProgress(0, "Belirleniyor.");
                return;
            }
            if (aktarilanEvrak < aktarilacakEvrak && islemDevam)
            {
                lblToplamEvrak.Text = aktarilacakEvrak.ToString("#,##0");
                lblAktarilanEvrak.Text = aktarilanEvrak.ToString("#,##0");
                lblKalanEvrak.Text = (aktarilacakEvrak - aktarilanEvrak).ToString("#,##0");
                Progress1.UpdateProgress(aktarilanEvrak / (float)aktarilacakEvrak, string.Format("% {0}", ((aktarilanEvrak / (float)aktarilacakEvrak) * 100).ToString("##0")));
                return;
            }
            else if (islemDevam)
            {
                Progress1.UpdateProgress(100, "Son iþlemler yapýlýyor. Lütfen bekleyin");
                return;
            }

            lblToplamEvrak.Text = aktarilacakEvrak.ToString("#,##0");
            lblAktarilanEvrak.Text = aktarilacakEvrak.ToString("#,##0");
            lblKalanEvrak.Text = "0";

            if (islemSonuc.Length == 0) islemSonuc.AppendLine("Sorun yok");
            pnlIslemSonuc.Html = islemSonuc.ToString();

            ResourceManager1.AddScript("{0}.stopTask('IslemGostergec');", TaskManager1.ClientID);
            Progress1.UpdateProgress(1, "Ýþlem tamamlandý.");

            if (aktarilacakEvrak > 0)
                GenelIslemler.MesajKutusu("Bilgi", "Ýþlem baþarýyla sonuçlandý.");
            else
                GenelIslemler.MesajKutusu("Bilgi", "Ýþlem yapýlacak bilgi yok.");

            aktarilanEvrak = 0;
            aktarilacakEvrak = -1;
        }
    }
}