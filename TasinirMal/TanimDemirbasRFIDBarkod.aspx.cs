using Ext1.Net;
using OrtakClass;
using System;
using System.Data;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    public partial class TanimDemirbasRFIDBarkod : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Demirbaþ RFID Etiket Basýmý";
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                if (Request.QueryString["menuYok"] == "1")
                {
                    tabPanelAna.Margins = "0 0 0 0";
                    tabPanelAna.StyleSpec += "padding:5px";
                    //tabPanelAna.Border = true;
                    grdListe.Width = 200;
                }

                pgFiltre.UpdateProperty("prpYil", DateTime.Now.Year.ToString());
                if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    pgFiltre.UpdateProperty("prpMuhasebe", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE"));
                    pgFiltre.UpdateProperty("prpHarcamaBirimi", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA"));
                    pgFiltre.UpdateProperty("prpAmbar", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFAMBAR"));
                }

                txtYaziciAdres.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "RFIDYAZICIURL");
            }
        }

        protected void btnListe_Click(object sender, DirectEventArgs e)
        {
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "RFIDYAZICIURL", txtYaziciAdres.Text);

            RowSelectionModel sm = grdListe.SelectionModel.Primary as RowSelectionModel;
            if (sm != null)
            {
                sm.SelectedRows.Clear();
                sm.UpdateSelection();
            }
            PagingToolbar1.PageSize = OrtakFonksiyonlar.ConvertToInt(cmbPageSize.Text, 0);
            Session["RFIDBrkodKriter"] = KriterTopla();
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
            shBilgi.kimeGitti = pgFiltre.Source["prpKisiKod"].Value.Trim().Replace(".", "");
            shBilgi.nereyeGitti = pgFiltre.Source["prpOdaKod"].Value.Trim().Replace(".", "");
            shBilgi.eSicilNo = pgFiltre.Source["prpEskiSicilNo"].Value.Trim();
            shBilgi.durum = OrtakFonksiyonlar.ConvertToInt(pgFiltre.Source["prpDurumKod"].Value.Trim(), 0);
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
            string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");

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
            dt.Columns.Add("epc");

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

                string epc = TNS.TMM.Arac.EPCOlustur(sh.prSicilNo, kurumKod);

                dt.Rows.Add(tip, sh.prSicilNo, sh.sicilNo, sh.hesapPlanKod, sh.hesapPlanAd, sh.kimeGitti, epc);
            }

            strListe.DataSource = dt;
            strListe.DataBind();

            return bilgi.sonuc.kayitSay;
        }

        protected void btnSorguTemizle_Click(object sender, DirectEventArgs e)
        {
        }

        protected void strListe_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            SicilNoHareket kriter = (SicilNoHareket)Session["RFIDBrkodKriter"];
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

    }
}