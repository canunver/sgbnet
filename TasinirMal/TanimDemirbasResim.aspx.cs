using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class TanimDemirbasResim : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMRSM001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                    pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));
                }
            }
        }

        protected void btnListe_Click(object sender, DirectEventArgs e)
        {
            hdnSeciliSicilNo.SetValue("");

            RowSelectionModel sm = grdListe.SelectionModel.Primary as RowSelectionModel;
            if (sm != null)
            {
                sm.SelectedRows.Clear();
                sm.UpdateSelection();
            }
            PagingToolbar1.PageSize = OrtakFonksiyonlar.ConvertToInt(cmbPageSize.Text, 0);
            Session["DemirbasResim"] = KriterTopla();
        }

        private SicilNoHareket KriterTopla()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value, 0);
            shBilgi.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            shBilgi.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            shBilgi.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            //shBilgi.fisNo = pgFiltre.Source["prpBelgeNo"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNo"].Value.Trim().PadLeft(6, '0');
            shBilgi.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim().Replace(".", "");
            shBilgi.hesapPlanKod = pgFiltre.Source["prpHesapKod"].Value.Trim();
            shBilgi.hesapPlanAd = pgFiltre.Source["prpHesapAd"].Value.Trim();
            shBilgi.kimeGitti = pgFiltre.Source["prpKisiKod"].Value.Trim().Replace(".", "");
            shBilgi.nereyeGitti = pgFiltre.Source["prpOdaKod"].Value.Trim().Replace(".", "");
            shBilgi.eSicilNo = pgFiltre.Source["prpEskiSicilNo"].Value.Trim();
            shBilgi.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurumKod"].Value.Trim(), 0);
            shBilgi.accessNo = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDosyaVarYok"].Value.Trim(), 0);
            shBilgi.sorguFisNoTif = pgFiltre.Source["prpBelgeNoTif"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNoTif"].Value.Trim().PadLeft(6, '0');
            shBilgi.sorguFisNoZimmet = pgFiltre.Source["prpBelgeNoZimmet"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNoZimmet"].Value.Trim().PadLeft(6, '0');

            if (GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "") == "1399") // Manas
            {
                shBilgi.kimeGitti = pgFiltre.Source["prpKisiKod"].Value.Trim();
                shBilgi.nereyeGitti = pgFiltre.Source["prpOdaKod"].Value.Trim();
            }

            return shBilgi;
        }

        private int SicilNumarasiDoldur(SicilNoHareket shBilgi, Sayfalama sayfa)
        {
            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, sayfa);
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarý", bilgi.sonuc.hataStr);
                bilgi.sonuc.kayitSay = -1;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("tip");
            dt.Columns.Add("prSicilNo");
            dt.Columns.Add("sicilno");
            dt.Columns.Add("kod");
            dt.Columns.Add("ad");
            dt.Columns.Add("kimeGitti");
            dt.Columns.Add("dosyaVar");
            dt.Columns.Add("zimmetoda");

            foreach (SicilNoHareket sh in bilgi.objeler)
            {
                string ozellik = "";

                if (!String.IsNullOrEmpty(sh.ozellik.markaAd))
                    ozellik = sh.ozellik.markaAd;
                if (!String.IsNullOrEmpty(sh.ozellik.modelAd))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.modelAd;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.plaka))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.plaka;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.adi))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.adi;
                }
                if (!String.IsNullOrEmpty(sh.ozellik.disSicilNo))
                {
                    if (ozellik != "") ozellik += "-";
                    ozellik += sh.ozellik.disSicilNo;
                }
                if (ozellik != "") ozellik = " (" + ozellik + ")";

                string tip = "TÝF";
                if (sh.kisiAd != "")
                    tip = "ZÝMMET";

                string dosyaVar = sh.accessNo > 0 ? "Var-" + sh.accessNo : "Yok";

                dt.Rows.Add(tip, sh.prSicilNo, sh.sicilNo, sh.hesapPlanKod, sh.hesapPlanAd + " " + ozellik, sh.kimeGitti, dosyaVar, sh.odaAd);
            }

            strListe.DataSource = dt;
            strListe.DataBind();

            return bilgi.sonuc.kayitSay;
        }

        protected void strListe_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            SicilNoHareket kriter = (SicilNoHareket)Session["DemirbasResim"];
            if (kriter == null)
                return;

            Sayfalama sayfa = new Sayfalama();

            sayfa.sayfaNo = (e.Start / e.Limit) + 1;
            sayfa.kayitSayisi = e.Limit;
            sayfa.siralamaAlani = e.Sort;
            if (e.Dir == Ext1.Net.SortDirection.Default || e.Dir == Ext1.Net.SortDirection.ASC)
                sayfa.siralamaYon = "ASC";
            else
                sayfa.siralamaYon = "DESC";

            if (sayfa.siralamaAlani == "prSicilNo") sayfa.siralamaAlani = "PRSICILNO";
            else if (sayfa.siralamaAlani == "sicilno") sayfa.siralamaAlani = "SICILNO";
            else if (sayfa.siralamaAlani == "kod") sayfa.siralamaAlani = "HESAPPLANKOD";
            else if (sayfa.siralamaAlani == "ad") sayfa.siralamaAlani = "HESAPPLANAD";
            else if (sayfa.siralamaAlani == "kimeGitti") sayfa.siralamaAlani = "KIMEGITTI";
            else if (sayfa.siralamaAlani == "dosyaVar") sayfa.siralamaAlani = "DOSYAVAR";
            else if (sayfa.siralamaAlani == "zimmetoda") sayfa.siralamaAlani = "ZIMMETODA";

            int kayitSayisi = SicilNumarasiDoldur(kriter, sayfa);
            if (kayitSayisi >= 0)
            {
                e.Total = kayitSayisi;

                if (kayitSayisi == 0)
                {
                    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
                }
            }
        }

        protected void ResimKayit(object sender, DirectEventArgs e)
        {
            string fileName = fileResim.PostedFile.FileName;

            int intLength = Convert.ToInt32(fileResim.PostedFile.InputStream.Length);
            byte[] resimByte = new byte[intLength];
            fileResim.PostedFile.InputStream.Read(resimByte, 0, intLength);

            TasResim resim = ResimBoyutlandir(resimByte, System.Drawing.RotateFlipType.RotateNoneFlipNone, 1024, 1024);
            //resim = new TasResim();
            //resim.resim = resimByte;

            string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
            if (tasinirYol != "")
                resim.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirResimleri");

            resim.prSicilNo = OrtakFonksiyonlar.ConvertToInt(hdnSeciliSicilNo.Value.ToString(), 0);
            resim.siraNo = 1;
            resim.adi = fileResim.PostedFile.FileName;
            resim.boyutu = fileResim.PostedFile.ContentLength;
            resim.ekleyenKisiAdi = kullanan.ADSOYAD;
            resim.ekleyenKisiKod = kullanan.mernis;
            resim.eklemeTarihi = new TNSDateTime(DateTime.Now);
            servisTMM.TasinirResimKaydet(resim);

            ResimListele(resim.prSicilNo.ToString());

            fileResim.Reset();
        }

        public static TasResim ResimBoyutlandir(byte[] resimByte, System.Drawing.RotateFlipType yon, double maxGenislik, double maxYukseklik)
        {
            TasResim resim = new TasResim();
            resim.resim = resimByte;

            ////byte[] olan bilgi Image bilgisine dönüþtürüldü
            System.IO.MemoryStream msResim = new System.IO.MemoryStream(resim.resim);
            System.Drawing.Image orjinalResim = System.Drawing.Image.FromStream(msResim);

            double dosyaGenislik = orjinalResim.Width;
            double dosyaYukseklik = orjinalResim.Height;

            if (dosyaGenislik > maxGenislik || dosyaYukseklik > maxYukseklik)
            {
                OrtakClass.ResimIslemleri.ResimBoyutlari(orjinalResim, maxGenislik, maxYukseklik, ref dosyaGenislik, ref dosyaYukseklik);
            }

            System.Drawing.Bitmap kucukResim = new System.Drawing.Bitmap(OrtakFonksiyonlar.ConvertToInt(dosyaGenislik, 0), OrtakFonksiyonlar.ConvertToInt(dosyaYukseklik, 0));
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(kucukResim))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.DrawImage(orjinalResim, 0, 0, OrtakFonksiyonlar.ConvertToInt(dosyaGenislik, 0), OrtakFonksiyonlar.ConvertToInt(dosyaYukseklik, 0));
            }
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                System.Drawing.Imaging.PropertyItem prpi = orjinalResim.GetPropertyItem(OrientationKey);
                if (prpi != null)
                {
                    var orientation = (int)orjinalResim.GetPropertyItem(OrientationKey).Value[0];
                    switch (orientation)
                    {
                        case NotSpecified: // Assume it is good.
                        case NormalOrientation:
                            // No rotation required.
                            break;
                        case MirrorHorizontal:
                            kucukResim.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
                            break;
                        case UpsideDown:
                            kucukResim.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                            break;
                        case MirrorVertical:
                            kucukResim.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);
                            break;
                        case MirrorHorizontalAndRotateRight:
                            kucukResim.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipX);
                            break;
                        case RotateLeft:
                            kucukResim.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                            break;
                        case MirorHorizontalAndRotateLeft:
                            kucukResim.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipX);
                            break;
                        case RotateRight:
                            kucukResim.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
                            break;
                    }
                }

                kucukResim.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                resim.resim = ms.ToArray();
            }
            catch { }

            if (orjinalResim != null)
                orjinalResim.Dispose();
            if (msResim != null)
                msResim.Dispose();
            if (kucukResim != null)
                kucukResim.Dispose();

            return resim;
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string prSicilNo = e.ExtraParams["prSicilNo"];
            hdnSeciliSicilNo.SetValue(prSicilNo);
            ResimListele(prSicilNo);
        }

        [DirectMethod]
        public void SatirSil(int resimID)
        {
            string note = string.Empty;
            note = "Silme iþlemi baþarýlý.";
            if (resimID == 0)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Lütfen silmek istenilen resmi seçiniz.");
                return;
            }
            TasResim tr = new TasResim();
            tr.resimID = resimID;

            string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
            tr.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirResimleri");

            Sonuc sonuc = servisTMM.TasinirResimSil(tr);
            if (sonuc.islemSonuc)
                Ext1.Net.Notification.Show(new NotificationConfig() { Icon = Icon.Information, HideDelay = 2000, PinEvent = "click", Html = note, Title = "Bilgi" });
            else
                Ext1.Net.Notification.Show(new NotificationConfig() { Icon = Icon.Information, HideDelay = 2000, PinEvent = "click", Html = sonuc.hataStr, Title = "Hata" });

            ResimListele(hdnSeciliSicilNo.Value.ToString());
        }

        [DirectMethod]
        public void DosyaIndir(int resimID, int dosyaID)
        {
            string gonderilecekDosyaTmp = "";
            string gonderilecekDosya = "";

            ObjectArray liste = new ObjectArray();

            if (resimID > 0)
            {
                liste = servisTMM.TasinirResimGetir(OrtakFonksiyonlar.ConvertToInt(hdnSeciliSicilNo.Value.ToString(), 0), resimID.ToString());
            }
            else if (dosyaID > 0)
            {
                TNS.TMM.IslemDosya islemD = new IslemDosya();
                islemD.dosyaKod = dosyaID;
                liste = servisTMM.IslemDosyaGetir(islemD);
            }

            if (liste.sonuc.islemSonuc)
            {
                if (resimID > 0)
                {
                    foreach (TasResim resim in liste.objeler)
                    {
                        string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
                        resim.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirResimleri");
                        string dosyaYol = Path.Combine(resim.kayitEdilecekDosyaYol, "RESIM");
                        dosyaYol = Path.Combine(dosyaYol, "RESIM" + "_" + resim.resimID);
                        if (File.Exists(dosyaYol))
                            resim.resim = File.ReadAllBytes(dosyaYol);

                        gonderilecekDosyaTmp = Path.GetTempFileName();
                        gonderilecekDosya = resim.adi;
                        File.WriteAllBytes(gonderilecekDosyaTmp, resim.resim);
                    }
                }
                else if (dosyaID > 0)
                {
                    foreach (TNS.TMM.IslemDosya resim in liste.objeler)
                    {
                        string tasinirYol = TasinirGenel.TasinirDosyaYoluVer();
                        resim.kayitEdilecekDosyaYol = Path.Combine(tasinirYol, "TasinirIslemBelgesi");
                        string dosya = Path.Combine(resim.kayitEdilecekDosyaYol, resim.yil.ToString());
                        dosya = Path.Combine(dosya, resim.yil + "_" + resim.dosyaKod);
                        if (File.Exists(dosya))
                            resim.resim = File.ReadAllBytes(dosya);

                        gonderilecekDosyaTmp = Path.GetTempFileName();
                        gonderilecekDosya = resim.adi;
                        File.WriteAllBytes(gonderilecekDosyaTmp, resim.resim);
                    }
                }

                FileInfo file = new FileInfo(gonderilecekDosyaTmp);
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpContext.Current.Server.UrlEncode(gonderilecekDosya).Replace('+', ' '));
                System.Web.HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                System.Web.HttpContext.Current.Response.Buffer = false;
                System.Web.HttpContext.Current.Response.WriteFile(file.FullName);
                System.Web.HttpContext.Current.Response.Flush();

                System.Web.HttpContext.Current.Response.End();
            }
        }

        void ResimHazirla(string prSicilNo)
        {
            //hdnSeciliResim.SetValue("");

            //if (!string.IsNullOrEmpty(prSicilNo))
            //{
            //    List<object> tipler = new List<object>();
            //    ObjectArray liste = servisTMM.TasinirResimGetir(hdnSeciliSicilNo.Value.ToString(), string.Empty);

            //    if (liste.sonuc.islemSonuc)
            //    {
            //        foreach (TasResim resim in liste.objeler)
            //        {
            //            tipler.Add(new
            //            {
            //                resimID = resim.resimID,
            //                prSicilNo = resim.prSicilNo,
            //                url = "data:image/jpg;base64," + Convert.ToBase64String((byte[])resim.resim),
            //                siraNo = resim.siraNo,
            //            });
            //        }
            //    }

            //    var store = this.lwResim.GetStore();
            //    store.DataSource = tipler.ToArray();
            //    store.DataBind();
            //}
        }

        private void ResimListele(string prSicilNo)
        {
            if (!string.IsNullOrEmpty(prSicilNo))
            {
                List<object> resimler = new List<object>();
                ObjectArray liste = servisTMM.TasinirResimGetir(OrtakFonksiyonlar.ConvertToInt(prSicilNo, 0), string.Empty);

                if (liste.sonuc.islemSonuc)
                {
                    foreach (TasResim resim in liste.objeler)
                    {
                        resimler.Add(new
                        {
                            DOSYAID = 0,
                            RESIMID = resim.resimID,
                            PRSICILNO = resim.prSicilNo,
                            ADI = resim.adi,
                            BOYUTU = (resim.boyutu / 1000).ToString("#,###KB"),
                            EKLEYENKISIAD = resim.ekleyenKisiAdi,
                            EKLEYENKISIKOD = resim.ekleyenKisiKod,
                            EKLEMETARIHI = resim.eklemeTarihi.ToString()
                        });
                    }
                }

                SicilNoHareket sh = new SicilNoHareket();
                sh.prSicilNo = OrtakFonksiyonlar.ConvertToInt(prSicilNo, 0);
                ObjectArray shListe = servisTMM.DosyaSicilNoListele(kullanan, sh);

                foreach (SicilNoHareket item in shListe.objeler)
                {
                    TNS.TMM.IslemDosya islemD = new TNS.TMM.IslemDosya();
                    islemD.fisNo = item.fisNo;
                    islemD.muhasebeKod = item.muhasebeKod;
                    islemD.harcamaBirimKod = item.harcamaBirimKod.Replace(".", "");

                    ObjectArray islemDListe = servisTMM.IslemDosyaGetir(islemD);

                    foreach (TNS.TMM.IslemDosya islemDosya in islemDListe.objeler)
                    {
                        resimler.Add(new
                        {
                            DOSYAID = islemDosya.dosyaKod,
                            RESIMID = 0,
                            PRSICILNO = islemDosya.fisNo,
                            ADI = islemDosya.adi,
                            BOYUTU = (islemDosya.boyutu / 1000).ToString("#,###KB"),
                            EKLEYENKISIAD = islemDosya.ekleyenKisiAdi,
                            EKLEYENKISIKOD = islemDosya.ekleyenKisiKod,
                            EKLEMETARIHI = islemDosya.eklemeTarihi.ToString()
                        });
                    }
                }

                //var store = this.lwResim.GetStore();
                strResimListe.DataSource = resimler.ToArray();
                strResimListe.DataBind();
            }
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
        }

        /// <summary>
        /// Excel çýktý
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ToExcel(object sender, EventArgs e)
        {
            string json = hdnGridData.Value.ToString();
            StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(json, null);
            XmlNode xml = eSubmit.Xml;

            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=DemirbasListe.xls");
            XslCompiledTransform xtExcel = new XslCompiledTransform();
            xtExcel.Load(Server.MapPath("../RaporSablon/StoreCiktiSablonlar/Excel.xsl"));
            xtExcel.Transform(xml, null, Response.OutputStream);
            Response.End();
        }

    }
}