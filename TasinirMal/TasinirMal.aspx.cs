using System;
using OrtakClass;
using TNS;
using TNS.TMM;
using System.Collections.Generic;

namespace TasinirMal
{
    public partial class TasinirMal : TMMSayfaV2
    {
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);

            //Sayfaya giriş izni varmı?
            if (!TNS.TMM.Yetki.SayfayaGirisYapabilirMi(kullanan))
                GenelIslemler.SayfayaGirmesin(true);

            if (!IsPostBack)
            {
                DepoDurumRaporu(KriterTopla());
            }
        }

        private DepoDurum KriterTopla()
        {
            DepoDurum kriter = new DepoDurum();
            kriter.yil = DateTime.Now.Year;
            if (!string.IsNullOrEmpty(Request.QueryString["muhasebe"]))
                kriter.muhasebeKod = Request.QueryString["muhasebe"];
            if (!string.IsNullOrEmpty(Request.QueryString["harcama"]))
                kriter.harcamaKod = Request.QueryString["harcama"];
            if (!string.IsNullOrEmpty(Request.QueryString["ambar"]))
                kriter.ambarKod = Request.QueryString["ambar"];
            kriter.raporTur = (int)ENUMDepoDurumRaporTur.KURUM;
            kriter.yilDevri = true;
            return kriter;
        }

        private void DepoDurumRaporu(DepoDurum kriter)
        {
            ObjectArray bilgi = servisTMM.TasinirDepoDurumu(kullanan, kriter);
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }
            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            decimal[] toplam = new decimal[8];
            List<object> liste = new List<object>();
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                DepoDurum depo = (DepoDurum)bilgi.objeler[i];

                object degerler = new
                {
                    TasinirKodu = depo.hesapPlanKod,
                    TasinirAdi = depo.hesapPlanAd,
                    OlcuBirimi = depo.olcuBirimAd,
                    GirenMiktar = depo.girenMiktar.ToString("#,###.00"),
                    GirenTutar = depo.girenTutar.ToString("#,###.00"),
                    CikanMiktar = depo.cikanMiktar.ToString("#,###.00"),
                    CikanTutar = depo.cikanTutar.ToString("#,###.00"),
                    ZimmetMiktar = (depo.zimmetKisiMiktar + depo.zimmetOrtakMiktar).ToString("#,###.00"),
                    ZimmetTutar = (depo.zimmetKisiTutar + depo.zimmetOrtakTutar).ToString("#,###.00"),
                    KalanMiktar = depo.kalanMiktar.ToString("#,###.00"),
                    KalanTutar = depo.kalanTutar.ToString("#,###.00")
                };

                liste.Add(degerler);

                toplam[0] += depo.girenMiktar;
                toplam[1] += depo.girenTutar;
                toplam[2] += depo.cikanMiktar;
                toplam[3] += depo.cikanTutar;
                toplam[4] += depo.zimmetKisiMiktar + depo.zimmetOrtakMiktar;
                toplam[5] += depo.zimmetKisiTutar + depo.zimmetOrtakTutar;
                toplam[6] += depo.kalanMiktar;
                toplam[7] += depo.kalanTutar;
            }

            object toplamSatir = new
            {
                TasinirKodu = "<B>" + Resources.TasinirMal.FRMTML012 + "</B>",
                TasinirAdi = string.Empty,
                OlcuBirimi = string.Empty,
                GirenMiktar = "<B>" + toplam[0].ToString("#,###.00") + "</B>",
                GirenTutar = "<B>" + toplam[1].ToString("#,###.00") + "</B>",
                CikanMiktar = "<B>" + toplam[2].ToString("#,###.00") + "</B>",
                CikanTutar = "<B>" + toplam[3].ToString("#,###.00") + "</B>",
                ZimmetMiktar = "<B>" + toplam[4].ToString("#,###.00") + "</B>",
                ZimmetTutar = "<B>" + toplam[5].ToString("#,###.00") + "</B>",
                KalanMiktar = "<B>" + toplam[6].ToString("#,###.00") + "</B>",
                KalanTutar = "<B>" + toplam[7].ToString("#,###.00") + "</B>"
            };

            liste.Add(toplamSatir);

            storeTasinir.DataSource = liste;
            storeTasinir.DataBind();
        }
    }
}