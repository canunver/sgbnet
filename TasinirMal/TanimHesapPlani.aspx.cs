using System;
using System.Data;
using System.Web.UI.WebControls;
using OrtakClass;
using TNS;
using TNS.TMM;
using Ext1.Net;
using System.Xml;
using System.Collections.Generic;
using TNS.KYM;
using System.IO;

namespace TasinirMal
{
    /// <summary>
    /// Taþýnýr hesap planý bilgilerinin kayýt, silme, listeleme ve raporlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public partial class HesapPlani : TMMSayfaV2
    {
        /// <summary>
        /// Taþýnýr mal servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayý:
        ///     Kullanýcý session'dan okunur.
        ///     Yetki kontrolü yapýlýr.
        ///     Sayfa ilk defa çaðýrýlýyorsa kontrollere ilgili bilgiler doldurulur, sayfa ayarlanýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMTHP001;
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    tabPanelAna.Border = true;
                    grdListe.Width = 200;
                }
                else
                    hdnSecKapat.Value = 1;

                OlcuBirimDoldur();
                KDVOraniDoldur();
                MarkaDoldur();
                RFIDEtiketTuruDoldur();

                //EGO gibi kurumlarda hesap planý 40.000 satýr. Sayfa açýlmýyor
                //HesapPlaniSatir sozluk = new HesapPlaniSatir();
                //Listele(sozluk);

            }
        }

        [DirectMethod]
        public void KaydetKontrollu()
        {
            //bool kullanilmisMi = false;
            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.hesapKod = txtKod.Text.Trim().Replace(".", "");

            Sonuc sonuc = servisTMM.HesapPlaniKullanilmisMi(kullanan, hs);

            if (sonuc.islemSonuc)
            {
                X.Msg.Confirm("Uyarý", sonuc.bilgiStr, new MessageBoxButtonsConfig
                {
                    Yes = new MessageBoxButtonConfig
                    {
                        Handler = "hdnZorla.setValue('zorla'); Ext1.net.DirectMethods.btnKaydet_Click();",
                        Text = "Evet"
                    },
                    No = new MessageBoxButtonConfig
                    {
                        Text = "Hayýr"
                    }
                }).Show();
            }
            else
                btnKaydet_Click();

            return;
        }

        /// <summary>
        /// Kaydet tuþuna basýlýnca çalýþan olay metodu
        /// Ýlçe taným bilgileri kaydedilmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        [DirectMethod]
        public void btnKaydet_Click()
        {
            ObjectArray hesapPlani = new ObjectArray();
            HesapPlaniSatir hs = new HesapPlaniSatir();

            hs.hesapKod = txtKod.Text.Trim().Replace(".", "");
            hs.aciklama = txtAciklama.Text.Trim();
            hs.amortiYil = OrtakFonksiyonlar.ConvertToInt(txtAmortismanYili.Text, 0);
            hs.olcuBirim = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlOlcuBirim), 0);
            hs.kdv = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlKdv), 0);
            hs.kullanilmiyor = OrtakFonksiyonlar.ConvertToInt(chkKullanilmiyor.Checked, 0);
            hs.guncelleme = OrtakFonksiyonlar.ConvertToInt(chkGuncelleme.Checked, 0);
            hs.markaKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlMarka), 0);
            hs.modelKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlModel), 0);
            hs.rfidEtiketKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlRFIDEtiket), 0);
            hs.kistAmortisman = chkKist.Checked ? 1 : 0;
            hs.vurgula = OrtakFonksiyonlar.ConvertToInt(chkVurgula.Checked, 0);

            hesapPlani.objeler.Add(hs);

            bool kaydetZorla = false;

            if (hdnZorla.Value.ToString() != "")
                kaydetZorla = true;

            Sonuc sonuc = servisTMM.HesapPlaniKaydet(kullanan, hesapPlani, false, kaydetZorla);


            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                btnAra_Click(null, null);//Temizlenmeden sadece kayýt edilen listelensin
                btnTemizle_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTMA005);
                //X.AddScript("Ext1.net.Mask.hide();");
            }
        }

        /// <summary>
        /// Bul tuþuna basýlýnca çalýþan olay metodu
        /// Sunucudan HesapPlaniSatir taným bilgileri alýnýr ve listelenir.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAra_Click(object sender, DirectEventArgs e)
        {
            PagingToolbar1.PageSize = OrtakFonksiyonlar.ConvertToInt(cmbPageSize.Text, 0);
        }

        /// <summary>
        /// Sil tuþuna basýlýnca çalýþan olay metodu
        /// Seçili olan HesapPlaniSatir silinmek üzere sunucuya gönderilir,
        /// gelen sonuca göre hata veya bilgi mesajý görüntülenir.
        /// Son olarak güncel bilgilerin görünmesi için listeleme iþlemi yapýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            ObjectArray hesapPlani = new ObjectArray();
            HesapPlaniSatir hs = new HesapPlaniSatir();

            hs.hesapKod = txtKod.Text.Trim().Replace(".", "");
            hesapPlani.objeler.Add(hs);

            Sonuc sonuc = servisTMM.HesapPlaniSil(kullanan, hesapPlani);

            if (!sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
            else
            {
                hdnZorla.Value = "1";
                btnTemizle_Click(null, null);
                btnAra_Click(null, null);
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMTMA006);
            }
        }

        protected void btnTemizle_Click(object sender, DirectEventArgs e)
        {
            txtKod.Clear();
            txtAciklama.Clear();
            ddlOlcuBirim.Clear();
            txtAmortismanYili.Clear();
            ddlKdv.Clear();
            chkKullanilmiyor.Clear();
            chkGuncelleme.Clear();
            chkVurgula.Clear();
            ddlMarka.Clear();
            ddlModel.Clear();
            ddlRFIDEtiket.Clear();
            lblEkliDosyaAd.Clear();
            btnEkliDosyaSil.Hide();

            ddlOlcuBirim.Value = "";
            ddlOlcuBirim.SelectedItem.Value = "";
            ddlMarka.Value = "";
            ddlMarka.SelectedItem.Value = "";
            ddlModel.Value = "";
            ddlModel.SelectedItem.Value = "";
            ddlRFIDEtiket.Value = "";
            ddlRFIDEtiket.SelectedItem.Value = "";
            chkKist.Clear();
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                HesapPlaniSatir kriter = new HesapPlaniSatir();
                kriter.hesapKod = row.SelectSingleNode("KOD").InnerXml.Replace(".", "");
                kriter.hesapKodAciklama = "!" + kriter.hesapKod;//sadece bu hesabý getirsin

                if (OrtakFonksiyonlar.ConvertToInt(row.SelectSingleNode("KULLANILMIYOR ").InnerXml, 0) > 0)
                    kriter.kullanilmiyor = OrtakFonksiyonlar.ConvertToInt(row.SelectSingleNode("KULLANILMIYOR ").InnerXml, 0);

                ObjectArray dler = servisTMM.HesapPlaniListele(kullanan, kriter, new Sayfalama());
                foreach (HesapPlaniSatir dd in dler.objeler)
                {
                    txtKod.Text = dd.hesapKod;
                    txtAciklama.Text = dd.aciklama;
                    ddlOlcuBirim.SetValueAndFireSelect(dd.olcuBirim);
                    txtAmortismanYili.Text = dd.amortiYil.ToString();
                    ddlKdv.SetValueAndFireSelect(dd.kdv);
                    chkKullanilmiyor.Checked = dd.kullanilmiyor > 0;
                    chkGuncelleme.Checked = dd.guncelleme > 0;
                    chkVurgula.Checked = dd.vurgula > 0;
                    if (dd.markaKod > 0)
                        ddlMarka.SetValueAndFireSelect(dd.markaKod);
                    else
                    {
                        ddlMarka.Value = "";
                        ddlMarka.SelectedItem.Value = "";
                    }

                    if (dd.modelKod > 0)
                        ddlModel.Value = dd.modelKod;
                    else
                    {
                        ddlModel.Value = "";
                        ddlModel.SelectedItem.Value = "";
                    }

                    if (dd.rfidEtiketKod > 0)
                        ddlRFIDEtiket.SetValueAndFireSelect(dd.rfidEtiketKod);
                    else
                    {
                        ddlRFIDEtiket.Value = "";
                        ddlRFIDEtiket.SelectedItem.Value = "";
                    }
                    chkKist.Checked = dd.kistAmortisman > 0;

                    EkliDosyaOku();
                }

                //if (hdnSecKapat.Text == "1")
                //{
                //    X.AddScript("try { parent.kepAdresiEkle('" + hdnSeciliAdres.Text + "'); } catch (e) { }");
                //    return;
                //}
            }
        }

        /// <summary>
        /// Sayfadaki ilgili kontrollerden HesapPlaniSatir kriter bilgilerini toplayan ve döndüren yordam
        /// </summary>
        /// <returns>HesapPlaniSatir kriter bilgileri döndürülür.</returns>
        public HesapPlaniSatir KriterTopla()
        {
            HesapPlaniSatir d = new HesapPlaniSatir();
            d.hesapKod = txtKod.Text.Trim().Replace(".", "");
            d.aciklama = txtAciklama.Text;
            d.amortiYil = OrtakFonksiyonlar.ConvertToInt(txtAmortismanYili.Text, 0);
            d.kullanilmiyor = OrtakFonksiyonlar.ConvertToInt(chkKullanilmiyor.Checked, 0);
            d.guncelleme = OrtakFonksiyonlar.ConvertToInt(chkGuncelleme.Checked, 0);
            d.vurgula = OrtakFonksiyonlar.ConvertToInt(chkVurgula.Checked, 0);
            d.olcuBirim = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlOlcuBirim), 0);
            if (d.olcuBirim == 0)
                d.olcuBirim = -1;

            d.markaKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlMarka), 0);
            if (d.markaKod == 0)
                d.markaKod = -1;

            d.modelKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlModel), 0);
            if (d.modelKod == 0)
                d.modelKod = -1;

            d.rfidEtiketKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlRFIDEtiket), 0);
            if (d.rfidEtiketKod == 0)
                d.rfidEtiketKod = -1;

            return d;
        }

        /// <summary>
        /// Detay Güncelle tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr hesap planlarýnýn detay bilgilerini düzenlemesi için
        /// sunucudaki HesapPlaniDetaySayilariDuzenle yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnDetayGuncelle_Click(object sender, System.EventArgs e)
        {
            servisTMM.HesapPlaniDetaySayilariDuzenle();
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr hesap planý bilgilerini excel dosyasýna yazan
        /// TasinirGenel sýnýfýndaki HesapPlaniExceleYaz yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, EventArgs e)
        {
            TasinirGenel.HesapPlaniExceleYaz();
        }

        protected void StoreListe_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            HesapPlaniSatir kriter = KriterTopla();
            if (kriter == null)
                return;

            if (chkSonDuzeyGoster.Checked)
                kriter.detay = true;

            Sayfalama sayfalama = new Sayfalama();
            sayfalama.sayfaNo = (e.Start / e.Limit) + 1; ;
            sayfalama.kayitSayisi = e.Limit;
            sayfalama.siralamaAlani = e.Sort;
            sayfalama.siralamaYon = e.Dir == Ext1.Net.SortDirection.ASC ? "ASC" : "DESC";

            ObjectArray dler = servisTMM.HesapPlaniListele(kullanan, kriter, sayfalama);

            if (!dler.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", dler.sonuc.hataStr);
                return;
            }

            List<object> liste = new List<object>();
            foreach (HesapPlaniSatir item in dler.objeler)
            {
                e.Total = item.toplamKayitSayisi;

                liste.Add(new
                {
                    KOD = item.hesapKod,
                    ACIKLAMA = item.aciklama,
                    SEVIYE = item.seviye,
                    DETAY = item.detay,
                    OLCUBIRIMKOD = item.olcuBirimAd,
                    KDV = item.kdv,
                    KULLANILMIYOR = item.kullanilmiyor,
                    GUNCELLEME = item.guncelleme,
                    NUMARA = item.numara,
                    AMORTIYIL = item.amortiYil,
                    RFIDETIKETKOD = item.rfidEtiketKod,
                    MARKAKOD = item.markaKod,
                    MODELKOD = item.modelKod,
                    KISTAMORTISMAN = item.kistAmortisman,
                    VURGULA = item.vurgula,
                });
            }

            if (liste.Count == 0)
                e.Total = 0;

            strListe.DataSource = liste;
            strListe.DataBind();
        }


        /// <summary>
        /// Ölçü birim taným bilgileri sunucudaki ilgili yordam çaðýrýlarak
        /// alýnýr ve ddlOlcuBirim DropDownList kontrolüne doldurulur.
        /// </summary>
        private void OlcuBirimDoldur()
        {
            ObjectArray bilgi = servisTMM.OlcuBirimListele(kullanan, new OlcuBirim());

            List<object> liste = new List<object>();
            foreach (OlcuBirim ob in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ob.kod,
                    ADI = ob.ad
                });
            }

            strOlcubirim.DataSource = liste;
            strOlcubirim.DataBind();
        }

        /// <summary>
        /// KDV oranlarý ddlKdv DropDownList kontrolüne doldurulur.
        /// </summary>
        private void KDVOraniDoldur()
        {
            ddlKdv.Items.Add(new Ext1.Net.ListItem("", ""));
            ddlKdv.Items.Add(new Ext1.Net.ListItem("%0", "0"));
            ddlKdv.Items.Add(new Ext1.Net.ListItem("%1", "1"));
            ddlKdv.Items.Add(new Ext1.Net.ListItem("%8", "8"));
            ddlKdv.Items.Add(new Ext1.Net.ListItem("%10", "10"));
            ddlKdv.Items.Add(new Ext1.Net.ListItem("%18", "18"));
            ddlKdv.Items.Add(new Ext1.Net.ListItem("%20", "20"));
        }

        private void MarkaDoldur()
        {
            ObjectArray bilgi = servisTMM.MarkaListele(kullanan, new Marka());

            List<object> liste = new List<object>();
            foreach (Marka ob in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ob.kod,
                    ADI = ob.ad
                });
            }

            strMarka.DataSource = liste;
            strMarka.DataBind();
        }

        protected void ModelDoldur(object sender, StoreRefreshDataEventArgs e)
        {
            ObjectArray bilgi = servisTMM.ModelListele(kullanan, new Model() { markaKodu = OrtakFonksiyonlar.ConvertToInt(this.ddlMarka.SelectedItem.Value, 0) });

            List<object> liste = new List<object>();
            foreach (Model ob in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ob.kod,
                    ADI = ob.ad
                });
            }

            strModel.DataSource = liste;
            strModel.DataBind();
        }

        private void RFIDEtiketTuruDoldur()
        {
            ObjectArray bilgi = servisTMM.RFIDEtiketTuruListele(kullanan, new RFIDEtiketTuru());

            List<object> liste = new List<object>();
            foreach (RFIDEtiketTuru ob in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ob.kod,
                    ADI = ob.ad
                });
            }

            strRFIDEtiket.DataSource = liste;
            strRFIDEtiket.DataBind();
        }

        /// <summary>
        /// Yazdýr tuþuna basýlýnca çalýþan olay metodu
        /// Taþýnýr hesap planý bilgilerini excel dosyasýna yazan
        /// TasinirGenel sýnýfýndaki HesapPlaniExceleYaz yordamý çaðýrýlýr.
        /// </summary>
        /// <param name="sender">Olayý tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnAmortiAta_Click(object sender, EventArgs e)
        {
            HesapPlaniSatir y = new HesapPlaniSatir();
            y.hesapKod = txtKod.Text.Trim();
            y.amortiYil = OrtakFonksiyonlar.ConvertToInt(txtAmortismanYili.Text, 0);
            Sonuc s = servisTMM.AmortismanSureKaydet(kullanan, y);
            if (s.islemSonuc)
                GenelIslemler.MesajKutusu("Sonuç", "Ýþlem Baþarýlý");
            else
                GenelIslemler.MesajKutusu("Hata", s.hataStr);
        }

        protected void btnEkliDosyaSil_Click(object sender, DirectEventArgs e)
        {
            Sonuc sonuc = this.EkliDosyaSil();
            if (sonuc.islemSonuc)
            {
                lblEkliDosyaAd.Clear();
                btnEkliDosyaSil.Hide();
            }
            else
                GenelIslemler.MesajKutusu("Hata", sonuc.hataStr);
        }

        //protected void fuDosya_Secildi(object sender, DirectEventArgs e)
        //{
        //    string hata = "";
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(txtKod.Text))
        //        {
        //            Sonuc sonuc = this.EkliDosyaSil();
        //            if (sonuc.islemSonuc)
        //            {
        //                EkliDosyalar form = new EkliDosyalar();
        //                form.modul = "TMM";
        //                form.form = txtKod.Text.Trim().Replace(".", "");
        //                form.dosyaAdi = System.IO.Path.GetFileName(fuDosya.PostedFile.FileName);
        //                form.ekliDosyaIcerik = new byte[fuDosya.PostedFile.ContentLength];
        //                fuDosya.PostedFile.InputStream.Read(form.ekliDosyaIcerik, 0, fuDosya.PostedFile.ContentLength);

        //                sonuc = servisKYM.KaydetEkliDosya(kullanan, form);
        //                if (sonuc.islemSonuc)
        //                    EkliDosyaOku();
        //                else
        //                    hata = sonuc.hataStr;
        //            }
        //            else
        //                hata = sonuc.hataStr;
        //        }
        //        else
        //            hata = "Lütfen öncelikle Hesap Konudu giriniz.";

        //    }
        //    catch (Exception ex)
        //    {
        //        hata = "Dosya kaydedilemedi:" + ex.Message;
        //    }
        //    fuDosya.Reset();

        //    if (hata != "")
        //        GenelIslemler.MesajKutusu("Hata", hata);
        //}


        [DirectMethod]
        public void DosyaYukle(string dosyaAd, string tmpDosya)
        {
            string hata = "";
            string tmpPath = "";
            try
            {
                tmpPath = Path.GetTempPath() + tmpDosya;
                if (!string.IsNullOrWhiteSpace(txtKod.Text))
                {
                    if (File.Exists(tmpPath))
                    {
                        Sonuc sonuc = this.EkliDosyaSil();
                        if (sonuc.islemSonuc)
                        {
                            EkliDosyalar form = new EkliDosyalar();
                            form.modul = "TMM";
                            form.form = txtKod.Text.Trim().Replace(".", "");

                            form.dosyaAdi = dosyaAd;
                            form.ekliDosyaIcerik = File.ReadAllBytes(tmpPath);

                            sonuc = servisKYM.KaydetEkliDosya(kullanan, form);
                            if (sonuc.islemSonuc)
                                EkliDosyaOku();
                            else
                                hata = sonuc.hataStr;
                        }
                        else
                            hata = sonuc.hataStr;
                    }
                    else
                        hata = tmpDosya + " isimli geçici dosya bulunamadý!";
                }
                else
                    hata = "Lütfen öncelikle Hesap Konudu giriniz.";

            }
            catch (Exception ex)
            {
                hata = "Dosya kaydedilemedi: " + ex.Message;
            }


            try
            {
                if (!string.IsNullOrWhiteSpace(tmpPath) && File.Exists(tmpPath))
                    File.Delete(tmpPath);
            }
            catch (Exception ex)
            {
                hata += "<br>Geçici dosya silinemedi: " + ex.Message;
            }

            if (hata != "")
                GenelIslemler.MesajKutusu("Hata", hata);
        }

        private void EkliDosyaOku()
        {
            EkliDosyalar form = new EkliDosyalar();
            form.modul = "TMM";
            form.form = txtKod.Text.Trim().Replace(".", "");
            form.dosyaNo = 1;
            ObjectArray oa = servisKYM.ListeleEkliDosyalar(kullanan, form);

            if (oa.sonuc.islemSonuc && oa.objeler.Count > 0)
            {
                lblEkliDosyaAd.Text = string.Format("<a href=\"#\" onclick=\"javascript:btnEkliDosyaIndir.fireEvent('click');\">{0}</a>", ((EkliDosyalar)oa.objeler[0]).dosyaAdi);
                btnEkliDosyaSil.Show();
            }
            else
            {
                lblEkliDosyaAd.Clear();
                btnEkliDosyaSil.Hide();
            }
        }

        private Sonuc EkliDosyaSil()
        {
            EkliDosyalar form = new EkliDosyalar();
            form.modul = "TMM";
            form.form = txtKod.Text.Trim().Replace(".", "");
            form.dosyaNo = 1;

            return servisKYM.SilEkliDosyalar(kullanan, form);
        }

        protected void btnEkliDosyaIndir_Click(object sender, DirectEventArgs e)
        {
            EkliDosyalar form = new EkliDosyalar();
            form.modul = "TMM";
            form.form = txtKod.Text.Trim().Replace(".", "");
            form.dosyaNo = 1;
            ObjectVAN ed = servisKYM.OkuEkliDosyaIcerik(kullanan, form);
            if (ed.sonuc.islemSonuc && ed.o != null)
            {
                form = (EkliDosyalar)ed.o;

                Response.Clear();

                OrtakClass.GenelIslemler.ResponseBasla(form.dosyaAdi, form.uzanti, true);
                Response.Flush();
                Response.BinaryWrite(form.ekliDosyaIcerik);
                Response.End();
            }
        }

    }
}