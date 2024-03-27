using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Ta��n�r demirba�lar�n�n �zellik bilgilerinin kay�t, listeleme ve silme i�lemlerinin yap�ld��� sayfa
    /// </summary>
    public partial class SicilNoOzellik : TMMSayfaV2
    {
        /// <summary>
        /// Ta��n�r mal servisine ula�mak i�in kullan�lan de�i�ken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa y�kleme olay�:
        ///     Kullan�c� session'dan okunur.
        ///     Yetki kontrol� yap�l�r.
        ///     Sayfa ilk defa �a��r�l�yorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlan�r.
        ///     Sayfa adresinde gelen girdi dizgileri ilgili kontrollere yaz�l�r
        ///     ve kontrollerden baz�lar� bu dizgilere g�re gizlenir/g�sterilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMSCO001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giri� izni varm�?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    grdListe.Width = 200;
                }

                if (Request.QueryString["tur"] + "" == "kutuphane")
                {
                    pgFiltre.UpdateProperty("prpHesapKod", Convert.ToString((int)ENUMTasinirHesapKodu.KUTUPHANE));
                    //pgFiltre.UpdateProperty("prpListedeGosterilecekler", Convert.ToString((int)ENUMTasinirHesapKodu.KUTUPHANE));

                    for (int i = 10; i <= 27; i++)
                    {
                        grdListe.ColumnModel.Columns[i].Hidden = true;
                    }

                    for (int i = 18; i <= 30; i++)
                    {
                        grdListe.ColumnModel.Columns[i].Hidden = false;
                    }
                }
                else if (Request.QueryString["tur"] + "" == "muze")
                {
                    pgFiltre.UpdateProperty("prpHesapKod", Convert.ToString((int)ENUMTasinirHesapKodu.MUZE));
                    //pgFiltre.UpdateProperty("prpListedeGosterilecekler", Convert.ToString((int)ENUMTasinirHesapKodu.MUZE));

                    for (int i = 10; i <= 15; i++)
                    {
                        grdListe.ColumnModel.Columns[i].Hidden = false;
                    }

                    for (int i = 17; i <= 30; i++)
                    {
                        grdListe.ColumnModel.Columns[i].Hidden = true;
                    }
                    grdListe.ColumnModel.Columns[10].Hidden = false;
                    grdListe.ColumnModel.Columns[17].Hidden = false;
                }
                else
                {
                    //pgFiltre.UpdateProperty("prpListedeGosterilecekler", "0");

                    for (int i = 12; i <= 31; i++)
                    {
                        grdListe.ColumnModel.Columns[i].Hidden = true;
                    }

                    if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    {
                        grdListe.ColumnModel.Columns[8].Hidden = true; //GIAI
                        grdListe.ColumnModel.Columns[9].Hidden = true; //EKNO
                    }
                }

                if (Request.QueryString["belgeNo"] + "" != "")
                {
                    pgFiltre.UpdateProperty("prpYil", Request.QueryString["yil"] + "");
                    pgFiltre.UpdateProperty("prpBelgeNoTif", Request.QueryString["belgeNo"] + "");
                }

                if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());

                if (Request.QueryString["mb"] + "" != "")
                    pgFiltre.UpdateProperty("prpMuhasebe", Request.QueryString["mb"] + "");
                else if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));


                if (Request.QueryString["hbk"] + "" != "")
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", Request.QueryString["hbk"] + "");
                else if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));


                if (Request.QueryString["ak"] + "" != "")
                    pgFiltre.UpdateProperty("prpAmbar", Request.QueryString["ak"] + "");
                else if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));


                ListeTemizle();
                MarkaDoldur();
                ModelDoldur();
                GosterilecekAlanlarDoldur(Request.QueryString["tur"] + "");
            }
        }

        /// <summary>
        /// Listeleme kriterleri SicilNoHareket nesnesinde parametre olarak al�n�r,
        /// sunucuya g�nderilir ve demirba� bilgileri sunucudan al�n�r. Hata varsa ekrana
        /// hata bilgisi yaz�l�r, yoksa gelen bilgiler grid kontrol�ne doldurulur.
        /// </summary>
        /// <param name="sicilNo">Demirba� listeleme kriterlerini tutan nesne</param>
        private int Listele(SicilNoHareket sicilNo, Sayfalama sayfa)
        {
            //Sayfalama �al��mad��� i�in bu konuldu. Sayfalama sorunu giderildi�inde kald�r�ls�n. Melih 24.04.2019
            sayfa = new Sayfalama();

            sicilNo.sadeceBirimdeOlanlar = true;
            ObjectArray yler = servisTMM.ButunSicilNoListele(kullanan, sicilNo, sayfa);

            if (!yler.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyar�", yler.sonuc.hataStr);

            if (yler.objeler.Count == 0)
                GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMSCO003);

            List<object> liste = new List<object>();
            foreach (TNS.TMM.SicilNoHareket y in yler.objeler)
            {
                liste.Add(new
                {
                    PRSICILNO = y.prSicilNo,
                    SICILNO = y.sicilNo,
                    HESAPPLANKOD = y.hesapPlanKod,
                    HESAPPLANAD = y.hesapPlanAd,
                    MARKAKOD = y.ozellik.markaKod,
                    MODELKOD = y.ozellik.modelKod,
                    ACIKLAMASERINO = y.ozellik.saseNo,
                    DISSICILNO = y.ozellik.disSicilNo + (y.ozellik.disSicilNo2 != "" ? " - " + y.ozellik.disSicilNo2 : ""),
                    ESERADI = y.ozellik.adi,
                    MUZEYEGELISTARIHI = y.ozellik.gelisTarihi.ToString(),
                    NEREDEBULUNDUGU = y.ozellik.neredeBulundu,
                    CAGI = y.ozellik.cagi,
                    DURUMUYAPIMMADDESI = y.ozellik.durumuMaddesi,
                    ONYUZU = y.ozellik.onYuz,
                    ARKAYUZU = y.ozellik.arkaYuz,
                    PUANI = y.ozellik.puan,
                    MUZEARSIVDEKIYERI = y.ozellik.yeriKonusu,
                    CILTNO = y.ozellik.ciltNo,
                    DIL = y.ozellik.dil,
                    YAZARCEVIRMENHATTATADI = y.ozellik.yazarAdi,
                    YAYINBASIMYERI = y.ozellik.yayinYeri,
                    YAYINBASIMTARIHI = y.ozellik.yayinTarihi,
                    NEREDENGELDIGI = y.ozellik.neredenGeldi,
                    AGIRLIGIBOYUTLARI = y.ozellik.boyutlari,
                    SATIRSAYISI = y.ozellik.satirSayisi,
                    YAPRAKSAYISI = y.ozellik.yaprakSayisi,
                    SAYFASAYISI = y.ozellik.sayfaSayisi,
                    BOYUTLARI = y.ozellik.boyutlari,
                    YERIKONUSU = y.ozellik.yeriKonusu,
                    DISSICILNO2 = y.ozellik.disSicilNo2,
                    BISNO = y.ozellik.bisNo,
                    ESKIBISNO1 = y.ozellik.eskiBisNo1,
                    ESKIBISNO2 = y.ozellik.eskiBisNo2,
                    RFIDETIKETTURKOD = y.ozellik.rfidEtiketTurKod,
                    AMORTISMANYILI = y.ozellik.amortismanYil,
                    AMORTISMANBITTI = y.ozellik.amortismanBitti,
                    GIAI = y.ozellik.giai,
                    EKNO = y.ozellik.ekNo,
                    BULUNDUGUYER = y.ozellik.bulunduguYerAd,
                    SICILNOOZELLIKYAPISTIR = 0
                });
            }
            strListe.DataSource = liste;
            strListe.DataBind();

            return yler.objeler.Count;
        }

        /// <summary>
        /// Listele tu�una bas�l�nca �al��an olay metodu
        /// Ekrandaki kriterler toplan�r ve Listele yordam� �a��r�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnListele_Click(object sender, System.EventArgs e)
        {
            RowSelectionModel sm = grdListe.SelectionModel.Primary as RowSelectionModel;
            if (sm != null)
            {
                sm.SelectedRows.Clear();
                sm.UpdateSelection();
            }
            PagingToolbar1.PageSize = OrtakFonksiyonlar.ConvertToInt(cmbPageSize.Text, 0);
            Session["SicilNoOzellik"] = KriterTopla();
        }

        private SicilNoHareket KriterTopla()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value, 0);
            shBilgi.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            shBilgi.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            shBilgi.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            shBilgi.hesapPlanKod = pgFiltre.Source["prpHesapKod"].Value.Trim().Replace(".", "");
            shBilgi.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim().Replace(".", "");
            shBilgi.sorguFisNoTif = pgFiltre.Source["prpBelgeNoTif"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNoTif"].Value.Trim().PadLeft(6, '0');
            shBilgi.sorguFisNoZimmet = pgFiltre.Source["prpBelgeNoZimmet"].Value == string.Empty ? string.Empty : pgFiltre.Source["prpBelgeNoZimmet"].Value.Trim().PadLeft(6, '0');

            if (pgFiltre.Source["prpEskiSicilNo"].Value.Trim() != "")
            {
                shBilgi.eSicilNo = pgFiltre.Source["prpEskiSicilNo"].Value.Trim();
            }

            return shBilgi;
        }

        protected void strListe_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            SicilNoHareket kriter = (SicilNoHareket)Session["SicilNoOzellik"];
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

            int kayitSayisi = Listele(kriter, sayfa);
            if (kayitSayisi >= 0)
            {
                e.Total = kayitSayisi;

                if (kayitSayisi == 0)
                {
                    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
                }
            }
        }

        /// <summary>
        /// Kaydet tu�una bas�l�nca �al��an olay metodu
        /// Demirba�lara ait �zellik bilgileri toplan�r ve kaydedilmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata varsa hata g�r�nt�lenir, yoksa bilgi mesaj� verilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnKaydet_Click(object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray detaySatirlari = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            Kaydet(detaySatirlari);
        }

        private void Kaydet(Newtonsoft.Json.Linq.JArray detaySatirlari)
        {
            ObjectArray o = new ObjectArray();

            string muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            string harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            string ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            int satir = 0;
            string marka = "";
            string model = "";
            string saseNo = "";
            string belgeTur = "0";// pgFiltre.Source["prpListedeGosterilecekler"].Value;

            foreach (Newtonsoft.Json.Linq.JObject item in detaySatirlari)
            {
                TNS.TMM.SicilNoOzellik s = new TNS.TMM.SicilNoOzellik();

                int prSicilNo = TasinirGenel.DegerAlInt(item, "PRSICILNO");
                if (prSicilNo <= 0)
                    continue;

                if (chkKaydet.Checked && satir == 0)
                {
                    marka = TasinirGenel.DegerAlStr(item, "MARKAKOD");
                    model = TasinirGenel.DegerAlStr(item, "MODELKOD");
                    saseNo = TasinirGenel.DegerAlStr(item, "ACIKLAMASERINO");
                }

                s.prSicilno = prSicilNo;
                //E�er disSicil envannter giri�inde verilmi� ve �zellik giri�i yap�l�yor ise d�ssicil k�t�phane ve m�ze d���nda siliniyordu.KAYDETME ile serverda update edilmemesi sa�land�.
                s.disSicilNo = "KAYDETME";

                if (belgeTur == Convert.ToString((int)ENUMTasinirHesapKodu.KUTUPHANE))
                {
                    s.disSicilNo = TasinirGenel.DegerAlStr(item, "DISSICILNO");
                    s.adi = TasinirGenel.DegerAlStr(item, "ESERADI");
                    s.ciltNo = TasinirGenel.DegerAlStr(item, "CILTNO");
                    //s.ciltTuru = TasinirGenel.DegerAlStr(item, "CILDINCINSI");
                    s.dil = TasinirGenel.DegerAlStr(item, "DIL");
                    s.yazarAdi = TasinirGenel.DegerAlStr(item, "YAZARCEVIRMENHATTATADI");
                    s.yayinYeri = TasinirGenel.DegerAlStr(item, "YAYINBASIMYERI");
                    s.yayinTarihi = TasinirGenel.DegerAlStr(item, "YAYINBASIMTARIHI");
                    s.satirSayisi = TasinirGenel.DegerAlStr(item, "SATIRSAYISI");
                    s.yaprakSayisi = TasinirGenel.DegerAlStr(item, "YAPRAKSAYISI");
                    s.sayfaSayisi = TasinirGenel.DegerAlStr(item, "SAYFASAYISI");
                    s.neredenGeldi = TasinirGenel.DegerAlStr(item, "NEREDENGELDIGI");
                    s.boyutlari = TasinirGenel.DegerAlStr(item, "BOYUTLARI");
                    //s.cesidi = TasinirGenel.DegerAlStr(item, "CESIDI");
                    s.puan = TasinirGenel.DegerAlStr(item, "PUANI");
                    s.yeriKonusu = TasinirGenel.DegerAlStr(item, "YERIKONUSU");
                    s.arkaYuz = TasinirGenel.DegerAlStr(item, "ARKAYUZU");
                    s.durumuMaddesi = TasinirGenel.DegerAlStr(item, "DURUMUYAPIMMADDESI");
                }
                else if (belgeTur == Convert.ToString((int)ENUMTasinirHesapKodu.MUZE))
                {
                    s.disSicilNo = TasinirGenel.DegerAlStr(item, "DISSICILNO");
                    s.gelisTarihi = TasinirGenel.DegerAlStr(item, "MUZEYEGELISTARIHI");
                    s.yeriKonusu = TasinirGenel.DegerAlStr(item, "MUZEARSIVDEKIYERI");
                    s.neredeBulundu = TasinirGenel.DegerAlStr(item, "NEREDEBULUNDUGU");
                    s.cagi = TasinirGenel.DegerAlStr(item, "CAGI");
                    s.boyutlari = TasinirGenel.DegerAlStr(item, "AGIRLIGIBOYUTLARI");
                    s.onYuz = TasinirGenel.DegerAlStr(item, "ONYUZU");
                    s.arkaYuz = TasinirGenel.DegerAlStr(item, "ARKAYUZU");
                    s.puan = TasinirGenel.DegerAlStr(item, "PUANI");
                    s.sayfaSayisi = TasinirGenel.DegerAlStr(item, "SAYFASAYISI");
                    s.neredenGeldi = TasinirGenel.DegerAlStr(item, "NEREDENGELDIGI");
                }
                else
                {
                    s.disSicilNo = TasinirGenel.DegerAlStr(item, "DISSICILNO");
                    s.yaprakSayisi = TasinirGenel.DegerAlStr(item, "YAPRAKSAYISI");
                    s.sayfaSayisi = TasinirGenel.DegerAlStr(item, "SAYFASAYISI");
                    s.neredenGeldi = TasinirGenel.DegerAlStr(item, "NEREDENGELDIGI");
                }

                //else
                //{
                if (marka != "")
                    s.markaKod = OrtakFonksiyonlar.ConvertToInt(marka, 0);
                else
                    s.markaKod = TasinirGenel.DegerAlInt(item, "MARKAKOD");

                if (model != "")
                    s.modelKod = OrtakFonksiyonlar.ConvertToInt(model, 0);
                else
                    s.modelKod = TasinirGenel.DegerAlInt(item, "MODELKOD");

                if (saseNo != "")
                    s.saseNo = saseNo;
                else
                    s.saseNo = TasinirGenel.DegerAlStr(item, "ACIKLAMASERINO");

                //s.amortismanYil = TasinirGenel.DegerAlInt(item, "AMORTISMANYILI");
                //s.amortismanBitti = TasinirGenel.DegerAlInt(item, "AMORTISMANBITTI");

                s.giai = TasinirGenel.DegerAlStr(item, "GIAI");
                s.ekNo = TasinirGenel.DegerAlStr(item, "EKNO");


                //Yap��t�r i�lemi ile sadece belirli alanlar�n kaydedilmesinin kontrol� i�i eklendi.
                s.kayitTur = TasinirGenel.DegerAlInt(item, "SICILNOOZELLIKYAPISTIR");

                o.objeler.Add(s);
                satir++;
            }


            //if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //{
            //    if (string.IsNullOrWhiteSpace(muhasebeKod) || string.IsNullOrWhiteSpace(harcamaBirimKod) || string.IsNullOrWhiteSpace(ambarKod))
            //    {
            //        SicilNoHareket kriter = new SicilNoHareket();
            //        kriter.prSicilNo = prSicilNo;

            //        ObjectArray bilgiler = servisTMM.ButunSicilNoListele(kullanan, kriter);
            //        if(bilgiler.objeler.Count>0)
            //        {
            //            kriter = (SicilNoHareket)bilgiler[0];
            //            muhasebeKod = kriter.muhasebeKod;
            //            harcamaBirimKod = kriter.harcamaBirimKod;
            //            ambarKod = kriter.ambarKod;
            //        }
            //    }
            //}


            Sonuc sonuc = servisTMM.SicilNoOzellikKaydet(kullanan, 0, muhasebeKod, harcamaBirimKod, ambarKod, o);

            if (sonuc.islemSonuc)
                GenelIslemler.ExtNotification(Resources.TasinirMal.FRMSCO042, "Bilgi", Icon.Lightbulb);
            else
                GenelIslemler.MesajKutusu("Uyar�", string.Format(Resources.TasinirMal.FRMSCO043, sonuc.hataStr));
        }

        /// <summary>
        /// Sil tu�una bas�l�nca �al��an olay metodu
        /// Demirba�lara ait �zellik bilgileri silinmek �zere sunucuya g�nderilir,
        /// gelen sonuca g�re hata varsa hata g�r�nt�lenir, yoksa bilgi mesaj� verilir.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            //Newtonsoft.Json.Linq.JArray detaySatirlari = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            //string muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            //string harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
            //string ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();

            //int[] prSicilNolar = new int[detaySatirlari.Count];

            //int i = 0;
            //foreach (Newtonsoft.Json.Linq.JObject item in detaySatirlari)
            //{
            //    prSicilNolar[i] = TasinirGenel.DegerAlInt(item, "PRSICILNO");
            //    i++;
            //}

            //Sonuc sonuc = servisTMM.SicilNoOzellikSil(kullanan, 0, muhasebeKod, harcamaBirimKod, ambarKod, prSicilNolar);

            //if (sonuc.islemSonuc)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMSCO044);
            //    btnListele_Click(null, null);
            //}
            //else
            //    GenelIslemler.MesajKutusu("Uyar�", string.Format(Resources.TasinirMal.FRMSCO045, sonuc.hataStr));
        }

        protected void btnSeriNoYukle_Click(object sender, DirectEventArgs e)
        {
            Newtonsoft.Json.Linq.JArray detaySatirlari = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            try
            {
                if (btnDosyaYukle.PostedFile == null || btnDosyaYukle.PostedFile.ContentLength <= 0)
                {
                    GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTIG078);
                    return;
                }

                string dosyaAdi = btnDosyaYukle.PostedFile.FileName;
                int count = Convert.ToInt16(btnDosyaYukle.PostedFile.InputStream.Length);
                byte[] myData = new byte[btnDosyaYukle.PostedFile.ContentLength];
                btnDosyaYukle.PostedFile.InputStream.Read(myData, 0, count);

                string dosyaAd = System.IO.Path.GetTempFileName();
                if (string.IsNullOrEmpty(dosyaAd))
                {
                    GenelIslemler.MesajKutusu("Uyar�", Resources.TasinirMal.FRMTIG077);
                    return;
                }

                System.IO.FileStream newFile = new System.IO.FileStream(dosyaAd, System.IO.FileMode.Create);
                newFile.Write(myData, 0, myData.Length);
                newFile.Close();

                int satir = 0;

                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(dosyaAd);
                foreach (Newtonsoft.Json.Linq.JObject item in detaySatirlari)
                {
                    string hucreBilgi = XLS.HucreDegerAl(satir, 0);
                    if (string.IsNullOrEmpty(hucreBilgi))
                        break;
                    if (TasinirGenel.DegerAlStr(item, "PRSICILNO") != "")
                        item["ACIKLAMASERINO"] = hucreBilgi;

                    satir++;
                }

                XLS.DosyaKapat();
                System.IO.File.Delete(dosyaAd);

                Kaydet(detaySatirlari);
            }
            catch (Exception ex)
            {
                GenelIslemler.MesajKutusu("Uyar�", ex.Message);
            }
        }

        private void ListeTemizle()
        {
            List<object> liste = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                liste.Add(new
                {
                    SICILNO = ""
                });
            }

            strListe.DataSource = liste;
            strListe.DataBind();
        }

        private void MarkaDoldur()
        {
            Marka marka = new Marka();
            ObjectArray bilgi = servisTMM.MarkaListele(kullanan, marka);

            List<object> liste = new List<object>();

            liste.Add(new
            {
                KOD = "",
                ADI = "",
            });

            foreach (TNS.TMM.Marka item in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    ADI = item.ad,
                });
            }

            strMarka.DataSource = liste;
            strMarka.DataBind();
        }

        private void ModelDoldur()
        {
            Model model = new Model();
            ObjectArray bilgi = servisTMM.ModelListele(kullanan, model);

            List<object> liste = new List<object>();

            liste.Add(new
            {
                KOD = "",
                ADI = "",
                MARKAKOD = ""
            });


            foreach (TNS.TMM.Model item in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = item.kod,
                    ADI = item.ad,
                    MARKAKOD = item.markaKodu
                });
            }

            strModel.DataSource = liste;
            strModel.DataBind();
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
            pgFiltre.UpdateProperty("prpHesapKod", string.Empty);
            pgFiltre.UpdateProperty("prpYil", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNoTif", string.Empty);
            pgFiltre.UpdateProperty("prpBelgeNoZimmet", string.Empty);
        }

        private void GosterilecekAlanlarDoldur(string tur)
        {
            List<object> liste = new List<object>();
            if (tur == "")
                liste.Add(new { KOD = 0, ADI = Resources.TasinirMal.FRMSCO055 });
            if (tur == "kutuphane")
                liste.Add(new { KOD = (int)ENUMTasinirHesapKodu.KUTUPHANE, ADI = Resources.TasinirMal.FRMSCO056 });
            if (tur == "muze")
                liste.Add(new { KOD = (int)ENUMTasinirHesapKodu.MUZE, ADI = Resources.TasinirMal.FRMSCO057 });

            strGosterilecekAlanlar.DataSource = liste;
            strGosterilecekAlanlar.DataBind();
        }

        /// <summary>
        /// Yazd�r tu�una bas�l�nca �al��an olay metodu
        /// Ekrandaki kriterler toplan�r ve Yazd�r yordam� �a��r�l�r.
        /// </summary>
        /// <param name="sender">Olay� tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            //SicilNoHareket y = new SicilNoHareket();
            //y.muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            //y.harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim();
            //y.ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();
            //y.sicilNo = pgFiltre.Source["prpSicilNo"].Value.Trim();
            ////if (txtBelgeNo.Text.Trim() != "")
            ////{
            ////    y.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpYil"].Value.Trim(),0);
            ////    y.fisNo = txtBelgeNo.Text.Trim().PadLeft(6, '0');
            ////}

            //y.sorguFisNoTif = pgFiltre.Source["prpBelgeNoTif"].Value.Trim();
            //y.sorguFisNoZimmet = pgFiltre.Source["prpBelgeNoZimmet"].Value.Trim();

            //string hata = "";
            //if (y.muhasebeKod == "")
            //hata += Resources.TasinirMal.FRMSCO004;
            //if (y.harcamaBirimKod == "")
            //hata += Resources.TasinirMal.FRMSCO005;

            //if (y.ambarKod == "")
            //    hata += Resources.TasinirMal.FRMSCO006;
            //if (y.sicilNo == "" && y.fisNo == "")
            //    hata += Resources.TasinirMal.FRMSCO007;

            Newtonsoft.Json.Linq.JArray detaySatirlari = (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(e.ExtraParams["SATIRLAR"]);

            SicilNoHareket kriter = KriterTopla();

            if (detaySatirlari.Count > 0)
            {
                int[] prSicilNolar = new int[detaySatirlari.Count];

                int i = 0;
                foreach (Newtonsoft.Json.Linq.JObject item in detaySatirlari)
                {
                    prSicilNolar[i] = TasinirGenel.DegerAlInt(item, "PRSICILNO");
                    i++;
                }
                kriter.sicilNolar = prSicilNolar;
            }

            /*if (hata != "")
                GenelIslemler.MesajKutusu(this, hata);
            else
            {*/
            ObjectArray bilgi = servisTMM.ButunSicilNoListele(kullanan, kriter);

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
            string sablonAd = "DayanikliTasinirListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            //XLS.HucreAdBulYaz("Baslik", tf.yil + " YILI BAKANLIK D�ZEY� KULLANILMAZ HAL VE HURDAYA AYRILAN TA�INIRLAR L�STES�");
            XLS.HucreAdBulYaz("harcamaBirimAd", pgFiltre.Source["prpHarcamaBirimi"].DisplayName.Trim());
            XLS.HucreAdBulYaz("harcamaBirimKod", kriter.harcamaBirimKod);
            XLS.HucreAdBulYaz("ambarAd", pgFiltre.Source["prpAmbar"].DisplayName.Trim());
            XLS.HucreAdBulYaz("ambarKod", kriter.ambarKod);
            XLS.HucreAdBulYaz("muhasebeBirimAd", pgFiltre.Source["prpMuhasebe"].DisplayName.Trim());
            XLS.HucreAdBulYaz("muhasebeBirimKod", kriter.muhasebeKod);
            XLS.HucreAdBulYaz("sicilNo", kriter.sicilNo);
            XLS.HucreAdBulYaz("belgeYili", kriter.yil);
            XLS.HucreAdBulYaz("hesapPlaniNo", kriter.hesapPlanKod);
            XLS.HucreAdBulYaz("belgeNumarasi", kriter.sorguFisNoTif + "/" + kriter.sorguFisNoZimmet);//Zimmeti de ekle??

            satir = kaynakSatir;

            //decimal miktarToplam = 0;
            //decimal tutarToplam = 0;
            foreach (TNS.TMM.SicilNoHareket s in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, s.ambarKod + "-" + s.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 1, s.yil);
                XLS.HucreDegerYaz(satir, sutun + 2, s.sorguFisNoTif);//Zimmeti de ekle??
                XLS.HucreDegerYaz(satir, sutun + 3, s.sicilNo);//+"/"+s.prSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 4, s.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 5, s.ozellik.markaKod.ToString() + "-" + s.ozellik.markaAd);
                XLS.HucreDegerYaz(satir, sutun + 6, s.ozellik.modelKod.ToString() + "-" + s.ozellik.modelAd);
                XLS.HucreDegerYaz(satir, sutun + 7, s.ozellik.saseNo);
            }

            ////Toplamlar yaz�l�yor
            //satir++;
            //XLS.SatirAc(satir, 1);
            //XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 5, satir, sutun);
            //XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
            //XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTHU002);
            //XLS.KoyuYap(satir, sutun, true);
            //XLS.DuseyHizala(satir, sutun, 1);
            //XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            //XLS.KoyuYap(satir, sutun + 4, true);
            //XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            //XLS.KoyuYap(satir, sutun + 5, true);
            //XLS.ArkaPlanRenk(satir, sutun, satir, sutun + 5, System.Drawing.Color.FromArgb(211, 211, 211));

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            //}
        }

        protected void btnHesapPlanKodDegistir_Click(object sender, DirectEventArgs e)
        {

            if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                GenelIslemler.MesajKutusu("Uyar�", string.Format("Uyar�", "Yetkisiz i�lem."));
            }
            else
            {
                HesapPlaniSatir form = new HesapPlaniSatir();
                form.hesapKod = txtHesapPlanKod.Text;

                int prSicilNo = OrtakFonksiyonlar.ConvertToInt(e.ExtraParams["PRSICILNO"], 0);
                string muhasebeKod = pgFiltre.Source["prpMuhasebe"].Value.Trim();
                string harcamaBirimKod = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim().Replace(".", "");
                string ambarKod = pgFiltre.Source["prpAmbar"].Value.Trim();

                Sonuc sonuc = servisTMM.HesapPlanKodDegistir(kullanan, muhasebeKod, harcamaBirimKod, ambarKod, prSicilNo, form);

                if (sonuc.islemSonuc)
                {
                    GenelIslemler.ExtNotification(sonuc.bilgiStr, "Bilgi", Icon.Lightbulb);

                    X.AddScript("wndHesapPlanKodDegistirGizle();");
                }
                else
                    GenelIslemler.MesajKutusu("Uyar�", string.Format(Resources.TasinirMal.FRMSCO043, sonuc.hataStr));

            }

            wndHesapPlanKodDegistir.Hide();
        }


    }
}