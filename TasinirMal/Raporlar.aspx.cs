using Ext1.Net;
using OrtakClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    /// <summary>
    /// Taşınır malzeme cetvellerinin raporlama işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class Raporlar : TMMSayfaV2
    {
        /// <summary>
        /// Taşınır mal servisine ulaşmak için kullanılan değişken
        /// </summary>
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
        TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ulaşmak için kullanılan değişken
        /// </summary>
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Formun sayfa yükleme olayı:
        ///     Kullanıcı session'dan okunur.
        ///     Yetki kontrolü yapılır.
        ///     Sayfa adresinde gelen tur girdi dizgisi hangi cetvelin istendiğine işaret eder,
        ///     bu parametreye bakılarak gizlenecek/gösterilecek kontroller ayarlanır.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                formAdi = "Raporlar";
                kullanan = OturumBilgisiIslem.KullaniciBilgiOku(true);
                SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtMuhasebe.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFMUHASEBE");
                txtHarcamaBirimi.Text = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "TIFHARCAMA");

                txtYil.Value = DateTime.Now.Year;
                RaporDoldur();
                DonemDoldur();
                IlDoldur();
                IslemTipiDoldur();

                string secRapor = "TMM001";
                if (EkranTurDondur() == "GM")
                {
                    GMTurDoldur();
                    secRapor = "TMM014";
                }
                else if (EkranTurDondur() == "YZ")
                {
                    YZTurDoldur();
                    ddlGayrimenkulTuru.FieldLabel = "Yazılım Türü";
                    secRapor = "TMM016";
                }


                X.AddScript("IlkRaporuSec('" + secRapor + "'); AlanIsimleriniYaz();");

                ddlCiktiTur.SetValueAndFireSelect("1");
                cmbSiralama.SetValueAndFireSelect("HESAPPLANKOD");

                //EtapDoldur();
                //DurumDoldur();
                //NitelikDoldur();
                //KonuDoldur();
                //KonuTurDoldur();
                //AyDoldur();
                //HibeMakineRaporlar.CiktiTurDoldur(ddlCiktiTur);
            }
        }

        void RaporDoldur()
        {
            List<object> tipler = new List<object>();

            if (EkranTurDondur() == "GM")
            {
                tipler.Add(new { KOD = "TMM014", AD = "Gayrimenkul Listesi", DOSYAAD = "GayrimenkulListesi.xlt" });
            }
            else if (EkranTurDondur() == "YZ")
            {
                tipler.Add(new { KOD = "TMM016", AD = "Yazılım Listesi", DOSYAAD = "YazilimListesi.xlt" });
            }
            else
            {
                tipler.Add(new { KOD = "TMM001", AD = "Hesap Planına Göre Demirbaş Dağılımı", DOSYAAD = "HesapPlaninaGoreMalzemeDagilim.xlt" });
                tipler.Add(new { KOD = "TMM002", AD = "Taşınır Depo Durum Raporu", DOSYAAD = "1" });
                tipler.Add(new { KOD = "TMM003", AD = "Hurda ve Kullanılmaz Demirbaş Raporu", DOSYAAD = "TasinirHurdaRaporu.xlt" });
                tipler.Add(new { KOD = "TMM004", AD = "Taşınır İşlem Bilgileri Raporu (Malzeme Tarihçe)", DOSYAAD = "TasinirIslemMalzemeTarihce.xlt" });
                tipler.Add(new { KOD = "TMM005", AD = "Belge Kayıt kütüğü Raporu", DOSYAAD = "BelgeKayitKutugu.xlt" });
                tipler.Add(new { KOD = "TMM006", AD = "Taşınır Sicil Raporu", DOSYAAD = "TasinirSicilRaporu.xlt" });
                tipler.Add(new { KOD = "TMM007", AD = "Taşınır Devir Çıkış Rapuru", DOSYAAD = "DevirCikis.xlt" });
                tipler.Add(new { KOD = "TMM008", AD = "En Çok Kullanılan Taşınırlar Raporu", DOSYAAD = "EnCokKullanilanlar.xlt" });
                tipler.Add(new { KOD = "TMM009", AD = "En Çok Kullanılan Taşınırlar Raporu (Detay)", DOSYAAD = "EnCokKullanilanlar.xlt" });
                tipler.Add(new { KOD = "TMM010", AD = "Kişiye Verilmiş Taşınırların Listesi", DOSYAAD = "ZimmetKisi.xlt" });
                tipler.Add(new { KOD = "TMM011", AD = "Zimmet Listesi", DOSYAAD = "ZimmetListesi.xlt" });
                tipler.Add(new { KOD = "TMM012", AD = "Ortak Alana Verilmiş Taşınırların Tam Listesi", DOSYAAD = "ZimmetOrtakAlan.xlt" });
                tipler.Add(new { KOD = "TMM013", AD = "Depodaki Taşınır Listesi", DOSYAAD = "AmbardakiTasinirListesi.xlt" });
                tipler.Add(new { KOD = "TMM015", AD = "Tüketim Malzemeleri Çıkış Listesi", DOSYAAD = "TuketimCikis.xlt" });
                tipler.Add(new { KOD = "TMM017", AD = "İşlem Yapan Bilgileri", DOSYAAD = "IslemYapilanBilgiListesi.xlt" });
            }

            tipler.Add(new { KOD = "TMM018", AD = "Tamamı Amorti Edilmemiş (Yıl Bazında) İcmal Listesi", DOSYAAD = "AmortismanYilBazli.xlt" });
            tipler.Add(new { KOD = "TMM031", AD = "Tamamı Amorti Edilmiş (Yıl Bazında) İcmal Listesi", DOSYAAD = "AmortismanYilBazli.xlt" });
            tipler.Add(new { KOD = "TMM020", AD = "Tamamı Amorti Edilmemişler (Amorti % Bazında) İcmal Listesi", DOSYAAD = "AmortismanYilBazli.xlt" });
            tipler.Add(new { KOD = "TMM019", AD = "Tamamı Amorti Edilmiş (Amorti % Bazında) İcmal Listesi", DOSYAAD = "AmortismanYilBazli.xlt" });

            tipler.Add(new { KOD = "TMM032", AD = "Tamamı Amorti Edilmemiş Demirbaş Listesi", DOSYAAD = "AmortismanOrtak.xlt" });
            tipler.Add(new { KOD = "TMM033", AD = "Tamamı Amorti Edilmiş Demirbaş Listesi", DOSYAAD = "AmortismanOrtak.xlt" });

            tipler.Add(new { KOD = "TMM023", AD = "Malzeme Gruplarına Göre Demirbaş Listesi", DOSYAAD = "MalzemeGruplarinaGoreDemirbas.xlt" });

            tipler.Add(new { KOD = "TMM026", AD = "Demirbaş Listesi", DOSYAAD = "AmortismanOrtak.xlt" });
            tipler.Add(new { KOD = "TMM028", AD = "Malzeme Listesi (Amortisman ve Alım Yılı Sıralı)", DOSYAAD = "AmortismanOrtak.xlt" });
            tipler.Add(new { KOD = "TMM030", AD = "Malzeme Listesi (Cins Sıralı)", DOSYAAD = "AmortismanOrtak.xlt" });

            tipler.Add(new { KOD = "TMM025", AD = "İhraç Listesi (Cins Sıralı)", DOSYAAD = "AmortismanOrtak.xlt" });
            tipler.Add(new { KOD = "TMM029", AD = "İhraç Listesi (A.% Sıralı)", DOSYAAD = "AmortismanOrtak.xlt" });

            tipler.Add(new { KOD = "TMM021", AD = "Sigorta Listesi", DOSYAAD = "GenelSigortaListesi.xlt" });
            tipler.Add(new { KOD = "TMM022", AD = "Oto ve Tablolar Hariç Sigorta Listesi", DOSYAAD = "GenelSigortaListesi.xlt" });
            tipler.Add(new { KOD = "TMM034", AD = "Yerleşke Yerine Göre Demirbaş Listesi", DOSYAAD = "AmortismanOrtak.xlt" });
            tipler.Add(new { KOD = "TMM035", AD = "Sosyal Tesislere Ayrılan Cari Amortisman Tutarları", DOSYAAD = "SosyalTesislereAyrilanCariAmortisman.xlt" });

            tipler.Add(new { KOD = "TMM036", AD = "KHK Raporu", DOSYAAD = "KHK Raporu.xlt" });
            tipler.Add(new { KOD = "TMM037", AD = "Yıllık Tüketim Raporu", DOSYAAD = "YillikTuketimRaporu.xlt" });

            tipler.Add(new { KOD = "TMM038", AD = "Birimlere Göre Demirbaş Dağılımı", DOSYAAD = "BirimlereGoreDemirbas.xlt" });
            tipler.Add(new { KOD = "TMM039", AD = "Tüketim Malzemesi İhtiyaç Formu", DOSYAAD = "TuketimMalzemeIhtiyacFormu.xlt" });
            tipler.Add(new { KOD = "TMM040", AD = "Birimlere Göre Demirbaş/Yazilim Amortismanları", DOSYAAD = "BirimlereGoreMBDemirbas.xlt" });
            tipler.Add(new { KOD = "TMM041", AD = "Birimlere Göre Gayrimenkul Amortismanları", DOSYAAD = "BirimlereGoreMBGayrimenkul.xlt" });
            tipler.Add(new { KOD = "TMM042", AD = "Duran Varlık Hareket Raporu", DOSYAAD = "DuranVarlikHareketRaporu.xlt" });

            tipler.Add(new { KOD = "TMM043", AD = "Demirbaş Eşya Kayıt Defteri", DOSYAAD = "DEMIRBASDEFTERIMB.xlt" });
            tipler.Add(new { KOD = "TMM044", AD = "Muhasebe Bakiye Karşılaştırma", DOSYAAD = "MuhasebeBakiyeKarsilastirma.xlt" });

            strListe.DataSource = tipler;
            strListe.DataBind();
        }

        void EkranHazirla(string kod)
        {
            ddlCiktiTur.Show();

            txtYil.Hide();
            cmpMuhasebe.Hide();
            cmpHarcamaBirimi.Hide();
            cmpHesapPlan.Hide();
            cmpAmbar.Hide();
            rdGrup.Hide();
            chkYilIci.Hide();
            chkYilDevri.Hide();
            chkMevcut.Hide();
            cmpTarih.Hide();
            chk2Duzey.Hide();
            txtBaslangicTarih.Hide();
            txtBitisTarih.Hide();
            cmpKimeVerildi.Hide();
            ddlIslemTipi.Hide();
            cmpGonMuhasebe.Hide();
            cmpGonHarcamaBirimi.Hide();
            cmpGonAmbar.Hide();
            cmpKimeGitti.Hide();
            cmpNereyeVerildi.Hide();
            txtNeredenGeldi.Hide();
            txtNereyeGitti.Hide();
            txtBirimFiyat.Hide();
            ddlIl.Hide();
            ddlIlce.Hide();
            ddlDonem.Hide();
            ddlOnayDurum.Hide();
            ddlGirisCikis.Hide();
            ddlGayrimenkulTuru.Hide();
            chkKisiDahilEt.Hide();
            chkDetay.Hide();
            txtBelgeNo.Hide();
            rblKHKDurumu.Hide();
            rdbRaporListesi.Hide();
            txtKisiSayisi.Hide();
            cmbSiralama.Hide();

            ddlAy.Hide();
            cmpHesapPlan2.Hide();
            ddlHesapPlanKodIslemDurumu.Hide();
            ddlIhracTur2.Hide();
            ddlIhracEdilmislerDurum.Hide();
            chkIhracSatislariDahilEt.Hide();

            if (kod == "TMM001")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpHesapPlan.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();

            }
            else if (kod == "TMM002")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                rdGrup.Show();
                rdAmbarBazinda.Show();
                rdKurumBazinda.Show();
                chkYilIci.Show();
                chkYilDevri.Show();
                chkMevcut.Show();
                rblKHKDurumu.Show();

            }
            else if (kod == "TMM003")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpHesapPlan.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                chk2Duzey.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                rdbRaporListesi.Show();

            }
            else if (kod == "TMM004")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                ddlIslemTipi.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                chk2Duzey.Show();
                cmpGonMuhasebe.Show();
                cmpGonHarcamaBirimi.Show();
                cmpGonAmbar.Show();
                cmpKimeGitti.Show();
                txtNeredenGeldi.Show();
                txtNereyeGitti.Show();
                ddlOnayDurum.Show();
                cmbSiralama.Show();
            }
            else if (kod == "TMM005")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                ddlIslemTipi.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpGonMuhasebe.Show();
                cmpGonHarcamaBirimi.Show();
                cmpGonAmbar.Show();
                cmpKimeGitti.Show();
                txtNeredenGeldi.Show();
                txtNereyeGitti.Show();

            }
            else if (kod == "TMM006")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                cmpKimeVerildi.Show();
                cmpNereyeVerildi.Show();
                ddlIslemTipi.Show();
                txtBirimFiyat.Show();
                cmpTarih.Show();
                ddlGirisCikis.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();

            }
            else if (kod == "TMM007")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();

            }
            else if (kod == "TMM008")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                ddlIl.Show();
                ddlIlce.Show();
                ddlDonem.Show();
                rdGrup.Show();
                rdAmbarBazinda.Hide();
                rdKurumBazinda.Hide();

            }
            else if (kod == "TMM009")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                ddlIl.Show();
                ddlIlce.Show();
                ddlDonem.Show();
                rdGrup.Show();
                rdAmbarBazinda.Hide();
                rdKurumBazinda.Hide();

            }
            else if (kod == "TMM010")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpKimeVerildi.Show();
                cmpHesapPlan.Show();

            }
            else if (kod == "TMM011")
            {
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
                cmpKimeVerildi.Show();
            }
            else if (kod == "TMM012")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
                cmpNereyeVerildi.Show();
                chkKisiDahilEt.Show();
            }
            else if (kod == "TMM013")
            {
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM014")
            {
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                ddlGayrimenkulTuru.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM015")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();


                if (!TNS.TMM.Arac.MeclisKullaniyor())//Meclis ambar gösterimesini istemiyor
                    cmpAmbar.Show();

                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                chkDetay.Show();
            }
            else if (kod == "TMM016")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                ddlGayrimenkulTuru.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM017")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM018")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM019")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM020")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM021")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan2.Show();
                ddlHesapPlanKodIslemDurumu.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM022")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM023")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM024")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpNereyeVerildi.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM025")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan.Show();
                txtBelgeNo.Show();
                ddlCiktiTur.Show();
                ddlIhracTur2.Show();
                chkIhracSatislariDahilEt.Show();
            }
            else if (kod == "TMM026")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpNereyeVerildi.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM027")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan.Show();
                cmpNereyeVerildi.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM028")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpNereyeVerildi.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM029")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan.Show();
                txtBelgeNo.Show();
                ddlCiktiTur.Show();
                ddlIhracTur2.Show();
                chkIhracSatislariDahilEt.Show();
            }
            else if (kod == "TMM030")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpNereyeVerildi.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM031")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM032")
            {
                txtYil.Show();
                ddlAy.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM033")
            {
                txtYil.Show();
                ddlAy.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan.Show();

                ddlCiktiTur.Show();
            }
            else if (kod == "TMM034")
            {
                txtYil.Show();
                cmpMuhasebe.Show();

                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                cmpHesapPlan.Show();
                cmpNereyeVerildi.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM035")
            {
                txtYil.Show();
                ddlAy.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM036")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                ddlIl.Show();
                cmpHesapPlan.Show();
                txtNeredenGeldi.Show();
                txtTCVergiNo.Show();
                cmpTarih.Show();
                txtBaslangicTarih.Show();
                txtBitisTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM037")
            {
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpHesapPlan.Show();
                ddlCiktiTur.Show();
                txtKisiSayisi.Show();
            }
            else if (kod == "TMM038")
            {
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpHesapPlan.Show();
            }
            else if (kod == "TMM040")
            {
                txtYil.Show();
                ddlAy.Show();
                cmpMuhasebe.Show();
                //cmpHarcamaBirimi.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM041")
            {
                txtYil.Show();
                ddlAy.Show();
                cmpMuhasebe.Show();
                //cmpHarcamaBirimi.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM042")
            {
                txtYil.Show();
                ddlAy.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                //cmpTarih.Show();
                //txtBaslangicTarih.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM043")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpHarcamaBirimi.Show();
                cmpAmbar.Show();
                ddlCiktiTur.Show();
            }
            else if (kod == "TMM044")
            {
                txtYil.Show();
                cmpMuhasebe.Show();
                cmpAmbar.Show();
                ddlCiktiTur.Show();
            }
        }

        protected void SatirSecildi(object sender, DirectEventArgs e)
        {
            string secilisatir = e.ExtraParams["GRIDPARAM"].ToString();
            XmlNode xml = JSON.DeserializeXmlNode("{records:{record:" + secilisatir + "}}");
            foreach (XmlNode row in xml.SelectNodes("records/record"))
            {
                string kod = row.SelectSingleNode("KOD").InnerXml;
                string ad = row.SelectSingleNode("AD").InnerXml;
                frmPanel.SetTitle("Rapor Kriter Alanları - " + ad);
                hdnRaporKod.Value = kod;
                EkranHazirla(kod);
            }
        }

        /// <summary>
        /// Yazdır tuşuna basılınca çalışan olay metodu
        /// Ekrandan kriter bilgilerini toplayan yordam çağırılır ve toplanan kriterler sayfa
        /// adresinde gelen tur girdi dizgisine bakılarak ilgili cetveli üreten yordama
        /// gönderilir, böylece excel raporu üretilip kullanıcıya gönderilmiş olur.
        /// </summary>
        /// <param name="sender">Olayı tetikleyen nesne</param>
        /// <param name="e">Olay parametresi</param>
        protected void btnYazdir_Click(object sender, DirectEventArgs e)
        {
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                bool yetki = servisTMM.GormeYetkisiVarMi(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());
                if (!yetki)
                {
                    GenelIslemler.MesajKutusu("Uyarı", "Lütfen yetkili olduğunuz birime ait biglileri seçiniz.");
                    return;
                }
            }

            string rapor = e.ExtraParams["RAPORBILGI"];
            string raporDosyaAd = "";
            string raporKod = "";
            string raporAd = "";
            if (!string.IsNullOrEmpty(rapor))
            {
                foreach (Newtonsoft.Json.Linq.JContainer jc in (Newtonsoft.Json.Linq.JArray)JSON.Deserialize(rapor))
                {
                    raporKod = OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("KOD"));
                    raporDosyaAd = OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("DOSYAAD"));
                    raporAd = OrtakFonksiyonlar.ConvertToStr(jc.Value<string>("AD"));
                }
            }
            if (raporDosyaAd.Trim() == "")
            {
                GenelIslemler.MesajKutusu("Uyarı", "Listeden görmek istediğiniz raporu seçiniz.");
                return;
            }
            if (raporKod == "TMM001")
                HesapPlanınaGoreMalzemeDagilimRaporu(raporDosyaAd);
            else if (raporKod == "TMM002")
                DepoDurumRaporu();
            else if (raporKod == "TMM003")
            {
                TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
                tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
                tf.muhasebeKod = txtMuhasebe.Text.Trim();
                tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
                tf.ambarKod = txtAmbar.Text.Trim();

                TNS.TMM.TasinirFormKriter kriter = new TNS.TMM.TasinirFormKriter();
                kriter.belgeTarihBasla = new TNSDateTime(txtBaslangicTarih.RawText);
                kriter.belgeTarihBit = new TNSDateTime(txtBitisTarih.RawText);
                kriter.hesapKodu = txtHesapPlanKod.Text.Trim();
                //kriter.islemTipleri = "11;12;17";//"54;55;56"
                int[] turListe = { 54, 55 };//56 
                kriter.islemTipleri = TipListesiVer(turListe);

                if (string.IsNullOrEmpty(tf.muhasebeKod))
                    tf.muhasebeKod = "empty";

                //if (!chk2Duzey.Checked)
                //    TasinirIslemTarihceRaporu(tf, kriter);
                //else
                //    TasinirIslemTarihceRaporu2Duzey(tf, kriter);

                if (!chk2Duzey.Checked)
                {
                    if (rHKodlu.Checked)
                        TasinirKodGrupluTasinirHurdaRaporu(tf, kriter);
                    else if (rHBirim.Checked)
                        BirimItibariylaTasinirHurdaRaporu(tf, kriter);
                    else if (rHBakanlik.Checked)
                        BakanlikDuzeyiTasinirHurdaRaporu(tf, kriter);
                    else if (rHIller.Checked)
                        IlBazliTasinirHurdaRaporu(tf, kriter);
                    else
                        TasinirIslemTarihceRaporu(tf, kriter);
                }
                else
                    TasinirIslemTarihceRaporu2Duzey(tf, kriter);


            }
            else if (raporKod == "TMM004")
            {
                if (!chk2Duzey.Checked)
                    TasinirIslemTarihceRaporu();
                else
                    TasinirIslemTarihceRaporu2Duzey();
            }
            else if (raporKod == "TMM005")
                BelgeKayitKutuguRaporu();
            else if (raporKod == "TMM006")
                SicilRaporu();
            else if (raporKod == "TMM007")
                DevirCikisRaporu();
            else if (raporKod == "TMM008")
                EnCokKullanilanlarRaporu();
            else if (raporKod == "TMM009")
                EnCokKullanilanlarRaporuDetay();
            else if (raporKod == "TMM010")
                ZimmetKisiYazdir();
            else if (raporKod == "TMM011")
                ZimmetListesiRaporu();
            else if (raporKod == "TMM012")
                ZimmetOrtakAlanYazdir();
            else if (raporKod == "TMM013")
                AmbardakiTasinirListesiRaporu();
            else if (raporKod == "TMM014")
                GayrimenkulListesiRaporu();
            else if (raporKod == "TMM015")
                TuketimCikisYazdir();
            else if (raporKod == "TMM016")
            {
                DemirbasListesi((int)ENUMMBRaporTur.YAZILIM);
                //YazilimListesiRaporu();
            }
            else if (raporKod == "TMM017")
                IslemYapanBilgileriRaporu();
            else if (raporKod == "TMM018")
                YilBazindaAmortiEdilmemis((int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER);
            else if (raporKod == "TMM019")
                YilBazindaAmortiEdilmemis((int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS);
            else if (raporKod == "TMM020")
                YilBazindaAmortiEdilmemis((int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMEMISLER);
            else if (raporKod == "TMM031")
                YilBazindaAmortiEdilmemis((int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER);
            else if (raporKod == "TMM021")
                SigortaListesi((int)ENUMMBRaporTur.GENELSIGORTALISTESI);
            else if (raporKod == "TMM022")
                SigortaListesi((int)ENUMMBRaporTur.OTOVETABLOLARHARICSIGORTALISTESI);
            else if (raporKod == "TMM023")
                MalzemeGruplarinaGoreDemirbas((int)ENUMMBRaporTur.MALZEMEGURUBUNAGOREADETVEBEDELLER);
            //else if (raporKod == "TMM024")
            //    DemirbasEsyaIhracListesiAmortiOlmamislar();//TMM029 AYNISI
            else if (raporKod == "TMM025")
                DemirbasListesi((int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU);//DemirbasEsyaIhracListesi();
            else if (raporKod == "TMM026")
                DemirbasListesi((int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE);
            else if (raporKod == "TMM028")
                DemirbasListesi((int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI);//BirBirimdekiMalzemelerinSorgulanmasi();
            else if (raporKod == "TMM029")
                IhracAmortismanli((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI);//DemirbasEsyaIhracListesiIlkASirali();
            else if (raporKod == "TMM030")
                DemirbasListesi((int)ENUMMBRaporTur.BIRIMMALZEMELISTESI);
            else if (raporKod == "TMM032")
                DemirbasListesi((int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR);
            else if (raporKod == "TMM033")
                DemirbasListesi((int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR);
            else if (raporKod == "TMM034")
                YerineGoreDemirbasListesi((int)ENUMMBRaporTur.YERLESKEYERINEGORETOPLAMAMORTISMAN);
            else if (raporKod == "TMM035")
                SosyalTesislereAyrilanAmortismanListesi();
            else if (raporKod == "TMM036")
                KHKRaporu();
            else if (raporKod == "TMM037")
                YillikTuketimRaporu();
            else if (raporKod == "TMM038")
                BirimlereGoreDemirbasDagilimi();
            else if (raporKod == "TMM040")
                BirimlereGoreDemirbasAmortismanTTK();
            else if (raporKod == "TMM041")
                BirimlereGoreGayrimenkulAmortismanTTK();
            else if (raporKod == "TMM042")
                DuranVarlikHareketRaporu();
            else if (raporKod == "TMM043")
                DemirbasDefteri();
            else if (raporKod == "TMM044")
                MuhasebeBakiyeKarsilastirma();
        }

        private void BirimlereGoreDemirbasAmortismanTTK()
        {
            AmortismanKriter ak = new AmortismanKriter();
            ak.muhasebeKod = txtMuhasebe.Text;
            //ak.ambarKod = txtAmbar.Text;
            ak.ambarKod = "01";

            ak.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            ak.donem = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
            ak.muhasebeKod = txtMuhasebe.Text;
            //ak.harcamaKod = txtHarcamaBirimi.Text;

            ak.raporTur = 7;
            ak.ihracDahil = false;


            ObjectArray bilgi = servisTMM.AmortismanRaporla2(kullanan, ak);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Dictionary<string, decimal[]> grupListe = new Dictionary<string, decimal[]>();

            foreach (TNS.TMM.AmortismanRapor detay in bilgi.objeler)
            {

                if (!grupListe.ContainsKey(detay.harcamaBirimiAdi))
                    grupListe.Add(detay.harcamaBirimiAdi, new decimal[4]);

                grupListe[detay.harcamaBirimiAdi][0] += detay.maliyetTutar;
                grupListe[detay.harcamaBirimiAdi][1] += detay.toplamTutar;
                grupListe[detay.harcamaBirimiAdi][2] += detay.toplamAmortismanTutar;
                grupListe[detay.harcamaBirimiAdi][3] += detay.kalanTutar;
            }


            //Geçici acil çözüm için yazılım ayrı sorgulandı.
            ak.ambarKod = "51";

            bilgi = servisTMM.AmortismanRaporla2(kullanan, ak);
            if (bilgi.objeler.Count > 0)
            {
                foreach (TNS.TMM.AmortismanRapor detay in bilgi.objeler)
                {
                    if (!grupListe.ContainsKey(detay.harcamaBirimiAdi))
                        grupListe.Add(detay.harcamaBirimiAdi, new decimal[4]);

                    grupListe[detay.harcamaBirimiAdi][0] += detay.maliyetTutar;
                    grupListe[detay.harcamaBirimiAdi][1] += detay.toplamTutar;
                    grupListe[detay.harcamaBirimiAdi][2] += detay.toplamAmortismanTutar;
                    grupListe[detay.harcamaBirimiAdi][3] += detay.kalanTutar;
                }
            }

            //************************************************************************************


            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BirimlereGoreMBDemirbas.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            XLS.HucreAdBulYaz("baslik", "Birimlere Göre Demirbaş ve Yazılım Dağılımı");

            satir = kaynakSatir;

            decimal[] genelToplam = new decimal[6];

            foreach (var item in grupListe)
            {
                satir++;

                string birimAd = item.Key;
                decimal maliyet = item.Value[0];
                decimal toplamTutar = item.Value[1];
                decimal amortisman = item.Value[2];
                decimal netDeger = item.Value[3];
                decimal sigorta = 0;


                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, birimAd);       //BİRİM
                XLS.HucreDegerYaz(satir, sutun + 1, maliyet);       //MALİYET BEDELİ
                XLS.HucreDegerYaz(satir, sutun + 2, toplamTutar);   //KAYITLI DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 3, amortisman);    //BİRİKMİŞ AMORTISMAN
                XLS.HucreDegerYaz(satir, sutun + 4, netDeger);      //NET DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 5, sigorta);       //SİGORTA DEĞERİ

                genelToplam[1] += maliyet;
                genelToplam[2] += toplamTutar;
                genelToplam[3] += amortisman;
                genelToplam[4] += netDeger;

            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "GENEL TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam[1]);
            XLS.HucreDegerYaz(satir, sutun + 2, genelToplam[2]);
            XLS.HucreDegerYaz(satir, sutun + 3, genelToplam[3]);
            XLS.HucreDegerYaz(satir, sutun + 4, genelToplam[4]);
            XLS.HucreDegerYaz(satir, sutun + 5, genelToplam[5]);

            XLS.SatirSil(kaynakSatir, kaynakSatir);

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void BirimlereGoreGayrimenkulAmortismanTTK()
        {
            AmortismanKriter ak = new AmortismanKriter();
            ak.muhasebeKod = txtMuhasebe.Text;
            //ak.ambarKod = txtAmbar.Text;
            ak.ambarKod = "50";
            ak.hesapPlanKod = "098%";

            ak.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            ak.donem = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
            ak.muhasebeKod = txtMuhasebe.Text;
            //ak.harcamaKod = txtHarcamaBirimi.Text;

            ak.raporTur = 7;
            ak.ihracDahil = false;


            ObjectArray bilgi = servisTMM.AmortismanRaporla2(kullanan, ak);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Dictionary<string, decimal[]> grupListe = new Dictionary<string, decimal[]>();

            foreach (TNS.TMM.AmortismanRapor detay in bilgi.objeler)
            {

                if (!grupListe.ContainsKey(detay.harcamaBirimiAdi))
                    grupListe.Add(detay.harcamaBirimiAdi, new decimal[4]);

                grupListe[detay.harcamaBirimiAdi][0] += detay.maliyetTutar;
                grupListe[detay.harcamaBirimiAdi][1] += detay.toplamTutar;
                grupListe[detay.harcamaBirimiAdi][2] += detay.toplamAmortismanTutar;
                grupListe[detay.harcamaBirimiAdi][3] += detay.kalanTutar;
            }


            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BirimlereGoreMBGayrimenkul.xlt";

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            XLS.HucreAdBulYaz("baslik", "Birimlere Gayrimenkul Dağılımı");

            satir = kaynakSatir;

            decimal[] genelToplam = new decimal[6];

            foreach (var item in grupListe)
            {
                satir++;

                string birimAd = item.Key;
                decimal maliyet = item.Value[0];
                decimal toplamTutar = item.Value[1];
                decimal amortisman = item.Value[2];
                decimal netDeger = item.Value[3];
                decimal sigorta = 0;


                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, birimAd);       //BİRİM
                XLS.HucreDegerYaz(satir, sutun + 1, maliyet);       //MALİYET BEDELİ
                XLS.HucreDegerYaz(satir, sutun + 2, toplamTutar);   //KAYITLI DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 3, amortisman);    //BİRİKMİŞ AMORTISMAN
                XLS.HucreDegerYaz(satir, sutun + 4, netDeger);      //NET DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 5, sigorta);       //SİGORTA DEĞERİ

                genelToplam[1] += maliyet;
                genelToplam[2] += toplamTutar;
                genelToplam[3] += amortisman;
                genelToplam[4] += netDeger;

            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "BİNALAR TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam[1]);
            XLS.HucreDegerYaz(satir, sutun + 2, genelToplam[2]);
            XLS.HucreDegerYaz(satir, sutun + 3, genelToplam[3]);
            XLS.HucreDegerYaz(satir, sutun + 4, genelToplam[4]);
            XLS.HucreDegerYaz(satir, sutun + 5, genelToplam[5]);

            XLS.SatirSil(kaynakSatir, kaynakSatir);





            //Arsalar acil çözüm için ayrı sorgulandı.

            decimal[] genelToplam2 = new decimal[6];

            ak.hesapPlanKod = "097%";

            grupListe = new Dictionary<string, decimal[]>();

            bilgi = servisTMM.AmortismanRaporla2(kullanan, ak);
            if (bilgi.objeler.Count > 0)
            {
                foreach (TNS.TMM.AmortismanRapor detay in bilgi.objeler)
                {
                    if (!grupListe.ContainsKey(detay.harcamaBirimiAdi))
                        grupListe.Add(detay.harcamaBirimiAdi, new decimal[4]);

                    grupListe[detay.harcamaBirimiAdi][0] += detay.maliyetTutar;
                    grupListe[detay.harcamaBirimiAdi][1] += detay.toplamTutar;
                    grupListe[detay.harcamaBirimiAdi][2] += detay.toplamAmortismanTutar;
                    grupListe[detay.harcamaBirimiAdi][3] += detay.kalanTutar;
                }


                satir = 0;
                sutun = 0;
                kaynakSatir = 0;

                XLS.HucreAdAdresCoz("BaslaSatir2", ref kaynakSatir, ref sutun);

                satir = kaynakSatir;

                foreach (var item in grupListe)
                {
                    satir++;

                    string birimAd = item.Key;
                    decimal maliyet = item.Value[0];
                    decimal toplamTutar = item.Value[1];
                    decimal amortisman = item.Value[2];
                    decimal netDeger = item.Value[3];
                    decimal sigorta = 0;


                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, birimAd);       //BİRİM
                    XLS.HucreDegerYaz(satir, sutun + 1, maliyet);       //MALİYET BEDELİ
                    XLS.HucreDegerYaz(satir, sutun + 2, toplamTutar);   //KAYITLI DEĞERİ
                    XLS.HucreDegerYaz(satir, sutun + 3, amortisman);    //BİRİKMİŞ AMORTISMAN
                    XLS.HucreDegerYaz(satir, sutun + 4, netDeger);      //NET DEĞERİ
                    XLS.HucreDegerYaz(satir, sutun + 5, sigorta);       //SİGORTA DEĞERİ

                    genelToplam2[1] += maliyet;
                    genelToplam2[2] += toplamTutar;
                    genelToplam2[3] += amortisman;
                    genelToplam2[4] += netDeger;

                }
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

                XLS.HucreDegerYaz(satir, sutun + 0, "ARSA TOPLAM");
                XLS.HucreDegerYaz(satir, sutun + 1, genelToplam2[1]);
                XLS.HucreDegerYaz(satir, sutun + 2, genelToplam2[2]);
                XLS.HucreDegerYaz(satir, sutun + 3, genelToplam2[3]);
                XLS.HucreDegerYaz(satir, sutun + 4, genelToplam2[4]);
                XLS.HucreDegerYaz(satir, sutun + 5, genelToplam2[5]);

                XLS.SatirSil(kaynakSatir, kaynakSatir);

            }

            //************************************************************************************


            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "GENEL TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam[1] + genelToplam2[1]);
            XLS.HucreDegerYaz(satir, sutun + 2, genelToplam[2] + genelToplam2[2]);
            XLS.HucreDegerYaz(satir, sutun + 3, genelToplam[3] + genelToplam2[3]);
            XLS.HucreDegerYaz(satir, sutun + 4, genelToplam[4] + genelToplam2[4]);
            XLS.HucreDegerYaz(satir, sutun + 5, genelToplam[5] + genelToplam2[5]);


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void BirimlereGoreDemirbasAmortisman()
        {
            AmortismanKriter t = new AmortismanKriter();
            t.muhasebeKod = txtMuhasebe.Text;
            //t.ambarKod = txtAmbar.Text;
            t.ambarKod = "01;51";

            //t.harcamaKod = txtHarcamaBirimi.Text;
            //t.ambarKod = "01";
            //t.nereyeGitti = txtNereyeVerildi.Text;
            //t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            //t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            //t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);

            t.raporTur = (int)ENUMMBRaporTur.BIRIMLEREGOREDEMIRBASAMORTISMAN;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BirimlereGoreMBDemirbas.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            XLS.HucreAdBulYaz("baslik", "Birimlere Göre Demirbaş ve Yazılım Dağılımı");

            satir = kaynakSatir;

            decimal[] genelToplam = new decimal[6];

            foreach (object[] obs in bilgi.objeler)
            {
                satir++;

                //birimAd,
                //amortisman,
                //maliyet,
                //degerArtis,
                //netDeger

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, (string)obs[0]);       //BİRİM
                XLS.HucreDegerYaz(satir, sutun + 1, (decimal)obs[2]);      //MALİYET BEDELİ
                XLS.HucreDegerYaz(satir, sutun + 2, (decimal)obs[3] + (decimal)obs[2]);  //KAYITLI DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 3, (decimal)obs[1]); //BİRİKMİŞ AMORTISMAN
                XLS.HucreDegerYaz(satir, sutun + 4, (decimal)obs[4]); //NET DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 5, 0);               //SİGORTA DEĞERİ

                genelToplam[1] += (decimal)obs[2];
                genelToplam[2] += (decimal)obs[3] + (decimal)obs[2];
                genelToplam[3] += (decimal)obs[1];
                genelToplam[4] += (decimal)obs[4];

            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "GENEL TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam[1]);
            XLS.HucreDegerYaz(satir, sutun + 2, genelToplam[2]);
            XLS.HucreDegerYaz(satir, sutun + 3, genelToplam[3]);
            XLS.HucreDegerYaz(satir, sutun + 4, genelToplam[4]);
            XLS.HucreDegerYaz(satir, sutun + 5, genelToplam[5]);

            XLS.SatirSil(kaynakSatir, kaynakSatir);

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void BirimlereGoreGayrimenkulAmortisman()
        {
            AmortismanKriter t = new AmortismanKriter();
            t.muhasebeKod = txtMuhasebe.Text;
            //t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = "50";
            t.hesapPlanKod = "098%";
            //t.nereyeGitti = txtNereyeVerildi.Text;
            //t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            //t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            //t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);

            t.raporTur = (int)ENUMMBRaporTur.BIRIMLEREGOREGAYRIMENKULAMORTISMAN;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BirimlereGoreMBGayrimenkul.xlt";

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            XLS.HucreAdBulYaz("baslik", "Birimlere Gayrimenkul Dağılımı");

            satir = kaynakSatir;

            decimal[] genelToplam = new decimal[6];

            foreach (object[] obs in bilgi.objeler)
            {
                satir++;

                //birimAd,
                //amortisman,
                //maliyet,
                //degerArtis,
                //netDeger

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, (string)obs[0]);       //BİRİM
                XLS.HucreDegerYaz(satir, sutun + 1, (decimal)obs[2]);      //MALİYET BEDELİ
                XLS.HucreDegerYaz(satir, sutun + 2, (decimal)obs[3] + (decimal)obs[2]);  //KAYITLI DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 3, (decimal)obs[1]); //BİRİKMİŞ AMORTISMAN
                XLS.HucreDegerYaz(satir, sutun + 4, (decimal)obs[4]); //NET DEĞERİ
                XLS.HucreDegerYaz(satir, sutun + 5, 0);               //SİGORTA DEĞERİ

                genelToplam[1] += (decimal)obs[2];
                genelToplam[2] += (decimal)obs[3] + (decimal)obs[2];
                genelToplam[3] += (decimal)obs[1];
                genelToplam[4] += (decimal)obs[4];

            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "BİNALAR TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam[1]);
            XLS.HucreDegerYaz(satir, sutun + 2, genelToplam[2]);
            XLS.HucreDegerYaz(satir, sutun + 3, genelToplam[3]);
            XLS.HucreDegerYaz(satir, sutun + 4, genelToplam[4]);
            XLS.HucreDegerYaz(satir, sutun + 5, genelToplam[5]);

            XLS.SatirSil(kaynakSatir, kaynakSatir);


            //Arsalar

            decimal[] genelToplam2 = new decimal[6];

            t.hesapPlanKod = "097%";
            bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);
            if (bilgi.objeler.Count > 0)
            {
                satir = 0;
                sutun = 0;
                kaynakSatir = 0;

                XLS.HucreAdAdresCoz("BaslaSatir2", ref kaynakSatir, ref sutun);

                satir = kaynakSatir;

                foreach (object[] obs in bilgi.objeler)
                {
                    satir++;

                    //birimAd,
                    //amortisman,
                    //maliyet,
                    //degerArtis,
                    //netDeger

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, (string)obs[0]);       //BİRİM
                    XLS.HucreDegerYaz(satir, sutun + 1, (decimal)obs[2]);      //MALİYET BEDELİ
                    XLS.HucreDegerYaz(satir, sutun + 2, (decimal)obs[3] + (decimal)obs[2]);  //KAYITLI DEĞERİ
                    XLS.HucreDegerYaz(satir, sutun + 3, (decimal)obs[1]); //BİRİKMİŞ AMORTISMAN
                    XLS.HucreDegerYaz(satir, sutun + 4, (decimal)obs[4]); //NET DEĞERİ
                    XLS.HucreDegerYaz(satir, sutun + 5, 0);               //SİGORTA DEĞERİ

                    genelToplam2[1] += (decimal)obs[2];
                    genelToplam2[2] += (decimal)obs[3] + (decimal)obs[2];
                    genelToplam2[3] += (decimal)obs[1];
                    genelToplam2[4] += (decimal)obs[4];

                }
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

                XLS.HucreDegerYaz(satir, sutun + 0, "ARSA TOPLAM");
                XLS.HucreDegerYaz(satir, sutun + 1, genelToplam2[1]);
                XLS.HucreDegerYaz(satir, sutun + 2, genelToplam2[2]);
                XLS.HucreDegerYaz(satir, sutun + 3, genelToplam2[3]);
                XLS.HucreDegerYaz(satir, sutun + 4, genelToplam2[4]);
                XLS.HucreDegerYaz(satir, sutun + 5, genelToplam2[5]);

                XLS.SatirSil(kaynakSatir, kaynakSatir);

            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "GENEL TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam[1] + genelToplam2[1]);
            XLS.HucreDegerYaz(satir, sutun + 2, genelToplam[2] + genelToplam2[2]);
            XLS.HucreDegerYaz(satir, sutun + 3, genelToplam[3] + genelToplam2[3]);
            XLS.HucreDegerYaz(satir, sutun + 4, genelToplam[4] + genelToplam2[4]);
            XLS.HucreDegerYaz(satir, sutun + 5, genelToplam[5] + genelToplam2[5]);


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void DuranVarlikHareketRaporu()
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.donem = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;

            //t.nereyeGitti = txtNereyeVerildi.Text;
            //t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            //t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            //t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
            //t.yil = t.baslamaTarihi.Yil;
            //t.donem = t.baslamaTarihi.Ay;
            //t.harcamaKod = "015";


            t.raporTur = (int)ENUMMBRaporTur.DURANVARLIKHAREKETRAPORU;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "DuranVarlikHareketRaporu.xlt";

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            //XLS.HucreAdBulYaz("baslik", "Duran Varlık Hareket Tablosu");

            satir = kaynakSatir;

            decimal[] genelToplam = new decimal[5];

            DuranHareketTablosuMB[] bolum1 = null;
            DuranHareketTablosuMB[] bolum2 = null;
            DuranHareketTablosuMB[] bolum3 = null;


            int indexBolum = 1;
            foreach (ArrayList arrayList in bilgi.objeler)
            {
                if (indexBolum == 1)
                    bolum1 = (DuranHareketTablosuMB[])arrayList.ToArray(typeof(DuranHareketTablosuMB));
                else if (indexBolum == 2)
                    bolum2 = (DuranHareketTablosuMB[])arrayList.ToArray(typeof(DuranHareketTablosuMB));
                else if (indexBolum == 3)
                    bolum3 = (DuranHareketTablosuMB[])arrayList.ToArray(typeof(DuranHareketTablosuMB));

                indexBolum++;
            }


            foreach (DuranHareketTablosuMB d in bolum1)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, d.islemAd);     //İşlem Adı
                XLS.HucreDegerYaz(satir, sutun + 1, d.gayrimenkul); //GayriMenkul
                XLS.HucreDegerYaz(satir, sutun + 2, d.demirbas);    //Demirbaş
                XLS.HucreDegerYaz(satir, sutun + 3, d.yazilim);     //Yazılım
                XLS.HucreDegerYaz(satir, sutun + 4, d.toplam);      //Toplam

                genelToplam[1] += d.gayrimenkul;
                genelToplam[2] += d.demirbas;
                genelToplam[3] += d.yazilim;
                genelToplam[4] += d.toplam;

            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun, satir, sutun + 4, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "31 Aralık net bilanço değeri");
            XLS.HucreDegerYaz(satir, sutun + 1, genelToplam[1]);
            XLS.HucreDegerYaz(satir, sutun + 2, genelToplam[2]);
            XLS.HucreDegerYaz(satir, sutun + 3, genelToplam[3]);
            XLS.HucreDegerYaz(satir, sutun + 4, genelToplam[4]);

            XLS.SatirSil(kaynakSatir, kaynakSatir);


            //T Yıl
            {
                decimal[] genelToplam2 = new decimal[5];

                satir = 0;
                sutun = 0;
                kaynakSatir = 0;

                XLS.HucreAdAdresCoz("BaslaSatir2", ref kaynakSatir, ref sutun);

                satir = kaynakSatir;

                XLS.HucreDegerYaz(satir - 1, sutun + 0, t.yil);

                foreach (DuranHareketTablosuMB d in bolum2)
                {
                    satir++;

                    //islemAd,
                    //gayrimenkul,
                    //demirbas,
                    //yazilim,
                    //toplam

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, d.islemAd);     //İşlem Adı
                    XLS.HucreDegerYaz(satir, sutun + 1, d.gayrimenkul); //GayriMenkul
                    XLS.HucreDegerYaz(satir, sutun + 2, d.demirbas);    //Demirbaş
                    XLS.HucreDegerYaz(satir, sutun + 3, d.yazilim);     //Yazılım
                    XLS.HucreDegerYaz(satir, sutun + 4, d.toplam);      //Toplam

                    genelToplam2[1] += d.gayrimenkul;
                    genelToplam2[2] += d.demirbas;
                    genelToplam2[3] += d.yazilim;
                    genelToplam2[4] += d.toplam;


                }
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                XLS.KoyuYap(satir, sutun, satir, sutun + 4, true);

                XLS.HucreDegerYaz(satir, sutun + 0, "Net bilanço değeri");
                XLS.HucreDegerYaz(satir, sutun + 1, genelToplam2[1]);
                XLS.HucreDegerYaz(satir, sutun + 2, genelToplam2[2]);
                XLS.HucreDegerYaz(satir, sutun + 3, genelToplam2[3]);
                XLS.HucreDegerYaz(satir, sutun + 4, genelToplam2[4]);

                XLS.HucreDegerYaz(kaynakSatir - 1, 0, t.yil);

                XLS.SatirSil(kaynakSatir, kaynakSatir);

            }


            //T-1 Yıl
            {
                decimal[] genelToplam3 = new decimal[5];

                satir = 0;
                sutun = 0;
                kaynakSatir = 0;

                XLS.HucreAdAdresCoz("BaslaSatir3", ref kaynakSatir, ref sutun);

                satir = kaynakSatir;

                XLS.HucreDegerYaz(satir - 1, sutun + 0, t.yil - 1);


                foreach (DuranHareketTablosuMB d in bolum3)
                {
                    satir++;

                    //islemAd,
                    //gayrimenkul,
                    //demirbas,
                    //yazilim,
                    //toplam

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, d.islemAd);     //İşlem Adı
                    XLS.HucreDegerYaz(satir, sutun + 1, d.gayrimenkul); //GayriMenkul
                    XLS.HucreDegerYaz(satir, sutun + 2, d.demirbas);    //Demirbaş
                    XLS.HucreDegerYaz(satir, sutun + 3, d.yazilim);     //Yazılım
                    XLS.HucreDegerYaz(satir, sutun + 4, d.toplam);      //Toplam

                    genelToplam3[1] += d.gayrimenkul;
                    genelToplam3[2] += d.demirbas;
                    genelToplam3[3] += d.yazilim;
                    genelToplam3[4] += d.toplam;


                }
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                XLS.KoyuYap(satir, sutun, satir, sutun + 4, true);

                XLS.HucreDegerYaz(satir, sutun + 0, "Net bilanço değeri");
                XLS.HucreDegerYaz(satir, sutun + 1, genelToplam3[1]);
                XLS.HucreDegerYaz(satir, sutun + 2, genelToplam3[2]);
                XLS.HucreDegerYaz(satir, sutun + 3, genelToplam3[3]);
                XLS.HucreDegerYaz(satir, sutun + 4, genelToplam3[4]);

                XLS.HucreDegerYaz(kaynakSatir - 1, 0, t.yil - 1);

                XLS.SatirSil(kaynakSatir, kaynakSatir);

            }


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void DemirbasDefteri()
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            //t.ambarKod = "01;51"; //Demirbaş ve yazılım

            t.raporTur = (int)ENUMMBRaporTur.DEMIRBASDEFTERI;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "DEMIRBASDEFTERIMB.xlt";

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            string harcamaAd = "Tüm Birimler";
            if (!string.IsNullOrWhiteSpace(t.harcamaKod))
            {
                harcamaAd = servisUZY.UzayDegeriStr(kullanan, "TASHARCAMABIRIM", t.muhasebeKod + "-" + t.harcamaKod, true, "");
                harcamaAd = t.harcamaKod + "-" + harcamaAd;
            }

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("RaporTarih", DateTime.Now.ToString("dd.MM.yyyy"));
            XLS.HucreAdBulYaz("HarcamaAd", harcamaAd);

            satir = kaynakSatir;

            int siraNo = 1;

            var liste = new Dictionary<string, decimal>();
            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                DegerEkle(liste, "gToplamIlkBedel", tb.maliyetTutar);
                DegerEkle(liste, "gToplamBedDuzFark", tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);
                DegerEkle(liste, "gToplamEnfDuzFark", tb.enflasyonDegerArtisTutar);
                DegerEkle(liste, "gToplamSonBedel", tb.maliyetTutar + tb.degerArtisTutar);
                DegerEkle(liste, "gToplamBirAmortisman", tb.maliyetAmortismanBirikmisTutar);
                DegerEkle(liste, "gToplamBirAmoFark", tb.degerArtisAmortismanTutar);
                DegerEkle(liste, "gDuzelmisBiriken", tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);


                satir++;

                //XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 30, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, siraNo++);//SIRANO
                XLS.HucreDegerYaz(satir, sutun + 1, tb.gorSicilNo);//YENİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 2, tb.disSicilNo);//ESKİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 3, tb.hesapPlanAd);//MALZEME ADI
                XLS.HucreDegerYaz(satir, sutun + 4, tb.ozellik?.Trim());//MARKA VE MODEL
                XLS.HucreDegerYaz(satir, sutun + 5, tb.neredenGeldi?.Trim());//ALINDIĞI YER
                XLS.HucreDegerYaz(satir, sutun + 6, tb.harcamaBirimiKod);//BIRIM KOD
                XLS.HucreDegerYaz(satir, sutun + 7, tb.faturaTarih?.ToString());//FATURA TARIH
                XLS.HucreDegerYaz(satir, sutun + 8, tb.faturaNo);//FATURA NO
                XLS.HucreDegerYaz(satir, sutun + 9, tb.girisTarih?.Oku().ToString("dd.MM.yyyy"));//GİRİŞ TARİHİ
                if (tb.amortismanSuresi > 0) XLS.HucreDegerYaz(satir, sutun + 10, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                XLS.HucreDegerYaz(satir, sutun + 11, tb.maliyetTutar);//ILK BEDEL
                XLS.HucreDegerYaz(satir, sutun + 12, tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);//BED. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 13, tb.enflasyonDegerArtisTutar);//ENF. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 14, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL
                XLS.HucreDegerYaz(satir, sutun + 15, tb.maliyetAmortismanBirikmisTutar);// + tb.degerlemeAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                XLS.HucreDegerYaz(satir, sutun + 16, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
                XLS.HucreDegerYaz(satir, sutun + 17, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//DÜZEL. BİRİKEN
                XLS.HucreDegerYaz(satir, sutun + 18, tb.nereyeGittiAd);//NEREYE GİTTİ

            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 22, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 30, true);

            XLS.HucreDegerYaz(satir, sutun + 11, liste["gToplamIlkBedel"] /*gToplamIlkBedel*/);
            XLS.HucreDegerYaz(satir, sutun + 12, liste["gToplamBedDuzFark"] /*gToplamBedDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 13, liste["gToplamEnfDuzFark"] /*gToplamEnfDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 14, liste["gToplamSonBedel"]);
            XLS.HucreDegerYaz(satir, sutun + 15, liste["gToplamBirAmortisman"] /*gToplamBırAmortısman*/);
            XLS.HucreDegerYaz(satir, sutun + 16, liste["gToplamBirAmoFark"] /*gToplamBırAmoFark*/);
            XLS.HucreDegerYaz(satir, sutun + 17, liste["gDuzelmisBiriken"] /*gToplamBırAmoFark*/);


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void MuhasebeBakiyeKarsilastirma()
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.ambarKod = txtAmbar.Text;

            t.raporTur = (int)ENUMMBRaporTur.MUHASEBEBAKIYEKARSILASTIRMA;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count != 2)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }


            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "MuhasebeBakiyeKarsilastirma.xlt";

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());

            satir = kaynakSatir;


            string[] yerAdi = { "Demirbaş Sistemi", "Muhasebe Sistemi" };

            int index = 0;

            foreach (MuhasebeBakiyeKarsilastirmaMB d in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, yerAdi[index]);
                XLS.HucreDegerYaz(satir, sutun + 1, d.demirbas.bedel);
                XLS.HucreDegerYaz(satir, sutun + 2, d.demirbas.enflasyonDuzeltmeFarki);
                XLS.HucreDegerYaz(satir, sutun + 3, d.demirbas.sonBedeli);
                XLS.HucreDegerYaz(satir, sutun + 4, d.demirbas.amortisman);
                XLS.HucreDegerYaz(satir, sutun + 5, d.demirbas.birikmisAmortismanFarki);
                XLS.HucreDegerYaz(satir, sutun + 6, d.demirbas.duzelmisBirikenAmortisman);
                index++;
            }
            XLS.SatirSil(kaynakSatir, kaynakSatir);

            XLS.HucreAdAdresCoz("BaslaSatir2", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            index = 0;

            foreach (MuhasebeBakiyeKarsilastirmaMB d in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, yerAdi[index]);
                XLS.HucreDegerYaz(satir, sutun + 1, d.yazilim.bedel);
                XLS.HucreDegerYaz(satir, sutun + 2, d.yazilim.enflasyonDuzeltmeFarki);
                XLS.HucreDegerYaz(satir, sutun + 3, d.yazilim.sonBedeli);
                XLS.HucreDegerYaz(satir, sutun + 4, d.yazilim.amortisman);
                XLS.HucreDegerYaz(satir, sutun + 5, d.yazilim.birikmisAmortismanFarki);
                XLS.HucreDegerYaz(satir, sutun + 6, d.yazilim.duzelmisBirikenAmortisman);
                index++;
            }
            XLS.SatirSil(kaynakSatir, kaynakSatir);

            XLS.HucreAdAdresCoz("BaslaSatir3", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            index = 0;

            foreach (MuhasebeBakiyeKarsilastirmaMB d in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, yerAdi[index]);
                XLS.HucreDegerYaz(satir, sutun + 1, d.gayrimenkul.bedel);
                XLS.HucreDegerYaz(satir, sutun + 2, d.gayrimenkul.enflasyonDuzeltmeFarki);
                XLS.HucreDegerYaz(satir, sutun + 3, d.gayrimenkul.sonBedeli);
                XLS.HucreDegerYaz(satir, sutun + 4, d.gayrimenkul.amortisman);
                XLS.HucreDegerYaz(satir, sutun + 5, d.gayrimenkul.birikmisAmortismanFarki);
                XLS.HucreDegerYaz(satir, sutun + 6, d.gayrimenkul.duzelmisBirikenAmortisman);
                index++;
            }
            XLS.SatirSil(kaynakSatir, kaynakSatir);



            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }




        private void BirimlereGoreDemirbasDagilimi()
        {
            TasinirIslemDetay form = new TasinirIslemDetay();
            //form.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            form.muhasebeKod = txtMuhasebe.Text;
            form.harcamaKod = txtHarcamaBirimi.Text;
            //form.ambarKod = txtAmbar.Text;
            form.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");

            ObjectArray bilgi = servisTMM.BirimlereGoreDemirbasRaporu(kullanan, form);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BirimlereGoreDemirbas.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal toplamMiktar = 0;
            decimal toplamTutar = 0;

            foreach (TasinirIslemDetay td in bilgi.objeler)
            {
                satir++;
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 3, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 0, td.harcamaKod + "-" + td.gonHarcamaKod);     //Birim Kod - Birim Adı
                XLS.HucreDegerYaz(satir, sutun + 1, td.hesapPlanKod + "-" + td.hesapPlanAd);     //Birim Kod - Birim Adı
                XLS.HucreDegerYaz(satir, sutun + 2, td.miktar);
                XLS.HucreDegerYaz(satir, sutun + 3, td.tutar);

                toplamMiktar += td.miktar;
                toplamTutar += td.tutar;
            }
            satir++;
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 5, satir, sutun);

            XLS.HucreDegerYaz(satir, sutun + 1, "Toplam");
            XLS.HucreDegerYaz(satir, sutun + 2, toplamMiktar);
            XLS.HucreDegerYaz(satir, sutun + 3, toplamTutar);

            XLS.KoyuYap(satir, sutun, satir, sutun + 3, true);
            XLS.SatirSil(kaynakSatir, kaynakSatir);


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void HesapPlanınaGoreMalzemeDagilimRaporu(string sablonAd)
        {
            Cetvel kriter = new Cetvel();

            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.tarih1 = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.tarih2 = new TNSDateTime(txtBitisTarih.RawText);

            ObjectArray bilgi = servisTMM.TasinirSayimCetveli(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Hata", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 1;
            string eskiHesapKod = string.Empty;

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];
            XLS.HucreAdBulYaz("muhasebe", cetvel.muhasebeAd);
            XLS.HucreAdBulYaz("harcamaBirimi", cetvel.harcamaAd);
            XLS.HucreAdBulYaz("ambar", "");
            XLS.HucreAdBulYaz("tarih", DateTime.Now.ToShortDateString());

            decimal adet = 0;
            decimal tutar = 0;
            int siraNo = 1;
            for (int i = 0; i < cetvel.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel.detay[i];

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);


                XLS.HucreDegerYaz(satir, 0, siraNo);
                XLS.HucreDegerYaz(satir, 1, detay.hesapPlanKod);
                XLS.HucreDegerYaz(satir, 2, detay.hesapPlanAd);
                XLS.HucreDegerYaz(satir, 3, detay.olcuBirimAd);
                XLS.HucreDegerYaz(satir, 4, OrtakFonksiyonlar.ConvertToDouble(detay.toplam.miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, 5, OrtakFonksiyonlar.ConvertToDouble(detay.toplam.tutar.ToString(), (double)0));

                siraNo++;
                satir++;

                adet += detay.toplam.miktar;
                tutar += detay.toplam.tutar;
            }
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
            XLS.KoyuYap(satir, 0, satir, 5, true);
            XLS.HucreDegerYaz(satir, 3, "TOPLAM:");
            XLS.HucreDegerYaz(satir, 4, OrtakFonksiyonlar.ConvertToDouble(adet.ToString(), (double)0));
            XLS.HucreDegerYaz(satir, 5, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void DepoDurumRaporu()
        {
            DepoDurum kriter = new DepoDurum();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");

            //if(rdKHKDahil.Checked)
            //    kriter.khkDurumu = 0;
            if (rdKHKSiz.Checked)
                kriter.khkDurumu = 1;
            if (rdSadeceKHK.Checked)
                kriter.khkDurumu = 2;

            if (rdMuhasebeBazinda.Checked)
            {
                kriter.raporTur = 0;
                kriter.harcamaKod = "";
            }
            if (rdHarcamaBazinda.Checked)
            {
                kriter.raporTur = 1;
                kriter.ambarKod = "";
            }
            if (rdAmbarBazinda.Checked)
                kriter.raporTur = 2;
            if (rdIlBazinda.Checked)
                kriter.raporTur = 3;
            if (rdKurumBazinda.Checked)
                kriter.raporTur = 4;
            kriter.yilIci = chkYilIci.Checked;
            kriter.yilDevri = chkYilDevri.Checked;

            ObjectArray bilgi = servisTMM.TasinirDepoDurumu(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (chkYilIci.Checked)
            {
                DepoDurum devredenKriter = new DepoDurum();
                devredenKriter.yil = kriter.yil - 1;
                devredenKriter.muhasebeKod = kriter.muhasebeKod;
                devredenKriter.harcamaKod = kriter.harcamaKod;
                devredenKriter.ambarKod = kriter.ambarKod;
                devredenKriter.hesapPlanKod = kriter.hesapPlanKod;
                devredenKriter.khkDurumu = kriter.khkDurumu;
                devredenKriter.raporTur = kriter.raporTur;
                devredenKriter.yilIci = false;
                devredenKriter.yilDevri = false;
                ObjectArray bilgiDevreden = servisTMM.TasinirDepoDurumu(kullanan, devredenKriter);
                foreach (DepoDurum itemDevreden in bilgiDevreden.objeler)
                {
                    bool bulundu = false;
                    foreach (DepoDurum item in bilgi.objeler)
                    {
                        if (item.hesapPlanKod == itemDevreden.hesapPlanKod && item.olcuBirimKod == itemDevreden.olcuBirimKod)
                        {
                            item.devredenMiktar = itemDevreden.girenMiktar - itemDevreden.cikanMiktar;
                            item.devredenTutar = itemDevreden.girenTutar - itemDevreden.cikanTutar;
                            if (item.girenMiktar > 0 && item.hesapPlanKod.StartsWith("15"))
                            {
                                item.girenMiktar = item.girenMiktar - item.devredenMiktar;
                                item.girenTutar = item.girenTutar - item.devredenTutar;
                            }

                            if (item.hesapPlanKod.StartsWith("25"))
                            {
                                item.zimmetKisiTutar += itemDevreden.zimmetKisiTutar;
                                item.zimmetOrtakTutar += itemDevreden.zimmetOrtakTutar;
                                item.zimmetKisiMiktar += itemDevreden.zimmetKisiMiktar;
                                item.zimmetOrtakMiktar += itemDevreden.zimmetOrtakMiktar;

                                item.kalanMiktar = item.kalanMiktar + (item.devredenMiktar - (itemDevreden.zimmetKisiMiktar + itemDevreden.zimmetOrtakMiktar));
                                item.kalanTutar = item.kalanTutar + (item.devredenTutar - (itemDevreden.zimmetKisiTutar + itemDevreden.zimmetOrtakTutar));
                            }

                            bulundu = true;
                        }
                    }

                    if (!bulundu)
                    {
                        itemDevreden.devredenMiktar = itemDevreden.girenMiktar - itemDevreden.cikanMiktar;
                        itemDevreden.devredenTutar = itemDevreden.girenTutar - itemDevreden.cikanTutar;
                        itemDevreden.girenMiktar = 0;
                        itemDevreden.cikanMiktar = 0;
                        itemDevreden.girenTutar = 0;
                        itemDevreden.cikanTutar = 0;
                        itemDevreden.kalanMiktar = itemDevreden.devredenMiktar - (itemDevreden.zimmetKisiMiktar + itemDevreden.zimmetOrtakMiktar);
                        itemDevreden.kalanTutar = itemDevreden.devredenTutar - (itemDevreden.zimmetKisiTutar + itemDevreden.zimmetOrtakTutar);
                        bilgi.objeler.Add(itemDevreden);
                    }
                }
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }

            bilgi.objeler.Sort(new SiralaHesapKod());

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = string.Empty;
            //if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || kriter.raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
            //    sablonAd = "TasinirDepoDurum.xlt";
            //else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA || kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
            //    sablonAd = "TasinirDepoDurumDiger.xlt";

            if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || kriter.raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
            {
                if (chkYilIci.Checked)
                    sablonAd = "TasinirDepoDurumDevreden.xlt";
                else if (rdSadeceKHK.Checked)
                    sablonAd = "TasinirDepoDurumKHK.xlt";
                else
                    sablonAd = "TasinirDepoDurum.xlt";
            }
            else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA || kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
            {
                if (rdSadeceKHK.Checked)
                    sablonAd = "TasinirDepoDurumDigerKHK.xlt";
                else
                    sablonAd = "TasinirDepoDurumDiger.xlt";
            }

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            string baslik = "TAŞINIR DEPO DURUMU";
            if (rdKHKSiz.Checked)
                baslik = "TAŞINIR DEPO DURUMU KHK'SIZ";

            XLS.HucreAdBulYaz("Baslik", baslik);

            satir = kaynakSatir;

            int siraNo = 0, toplamCount;
            if (rdSadeceKHK.Checked)
                toplamCount = 6;
            else
                toplamCount = 8;
            decimal[] toplam = new decimal[toplamCount];
            int ekKolon = 0;
            decimal devredenMiktarToplam = 0;
            decimal devredenTutarToplam = 0;

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                DepoDurum depo = (DepoDurum)bilgi.objeler[i];
                if (i == 0)
                {
                    string raporTur = "";
                    if (rdMuhasebeBazinda.Checked)
                        raporTur = Resources.TasinirMal.FRMTDD020;
                    if (rdHarcamaBazinda.Checked)
                        raporTur = Resources.TasinirMal.FRMTDD021;
                    if (rdAmbarBazinda.Checked)
                        raporTur = Resources.TasinirMal.FRMTDD022;
                    if (rdIlBazinda.Checked)
                        raporTur = Resources.TasinirMal.FRMTDD023;
                    if (rdKurumBazinda.Checked)
                        raporTur = Resources.TasinirMal.FRMTDD024;

                    XLS.HucreAdBulYaz("RaporTur", raporTur);
                    if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.AMBAR)
                    {
                        if (kriter.muhasebeKod.Trim() != "")
                        {
                            XLS.HucreAdBulYaz("IlAd", depo.ilAd + "-" + depo.ilceAd);
                            XLS.HucreAdBulYaz("IlKod", depo.ilKod + "-" + depo.ilceKod);
                            XLS.HucreAdBulYaz("MuhasebeAd", depo.muhasebeAd);
                            XLS.HucreAdBulYaz("MuhasebeKod", depo.muhasebeKod);
                        }
                        else
                        {
                            XLS.HucreAdBulYaz("IlAd", Resources.TasinirMal.FRMTDD002);
                            XLS.HucreAdBulYaz("IlKod", "");
                            XLS.HucreAdBulYaz("MuhasebeAd", Resources.TasinirMal.FRMTDD002);
                            XLS.HucreAdBulYaz("MuhasebeKod", "");
                        }

                        if (kriter.harcamaKod != "")
                        {
                            XLS.HucreAdBulYaz("HarcamaAd", depo.harcamaAd);
                            XLS.HucreAdBulYaz("HarcamaKod", depo.harcamaKod);
                        }

                        if (kriter.ambarKod != "")
                        {
                            XLS.HucreAdBulYaz("AmbarAd", depo.ambarAd);
                            XLS.HucreAdBulYaz("AmbarKod", depo.ambarKod);
                        }
                    }
                    else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
                    {
                        XLS.HucreAdBulYaz("IlAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
                        XLS.HucreAdBulYaz("IlKod", GenelIslemlerIstemci.VarsayilanKurumBul());
                        int kurumGizliSatir = 0;
                        int kurumGizliSutun = 0;
                        XLS.HucreAdAdresCoz("IlKod", ref kurumGizliSatir, ref kurumGizliSutun);
                        XLS.SatirGizle(kurumGizliSatir + 1, kurumGizliSatir + 3, true);
                        XLS.HucreDegerYaz(kurumGizliSatir, sutun, Resources.TasinirMal.FRMTDD003);
                    }
                    else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                    {
                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 1, Resources.TasinirMal.FRMTDD004);
                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 2, Resources.TasinirMal.FRMTDD005);
                    }
                    else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                    {
                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 1, Resources.TasinirMal.FRMTDD006);
                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 2, Resources.TasinirMal.FRMTDD007);
                        //XLS.SutunGizle(sutun + 2, sutun + 4, true);
                    }
                    else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                    {
                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 1, Resources.TasinirMal.FRMTDD008);
                        XLS.HucreDegerYaz(kaynakSatir - 1, sutun + 2, Resources.TasinirMal.FRMTDD009);
                    }

                    if (kriter.raporTur != (int)ENUMDepoDurumRaporTur.AMBAR)
                    {
                        XLS.HucreAdBulYaz("TasinirAd", depo.hesapPlanAd);
                        XLS.HucreAdBulYaz("TasinirKod", depo.hesapPlanKod);
                    }
                }

                if (depo.kalanMiktar == (decimal)0 && chkMevcut.Checked)
                    continue;

                siraNo++;
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                XLS.HucreDegerYaz(satir, sutun, siraNo);
                if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.AMBAR || kriter.raporTur == (int)ENUMDepoDurumRaporTur.KURUM)
                {
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, depo.hesapPlanAd);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                {
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.muhasebeKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, depo.muhasebeAd);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                {
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.muhasebeKod + " - " + depo.harcamaKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, depo.muhasebeAd + " - " + depo.harcamaAd);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                {
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.ilKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, depo.ilAd);
                }

                XLS.HucreDegerYaz(satir, sutun + 5, depo.olcuBirimAd);

                if (chkYilIci.Checked)
                {
                    ekKolon = 2;
                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(depo.devredenMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(depo.devredenTutar.ToString(), (double)0));
                }

                XLS.HucreDegerYaz(satir, sutun + 6 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.cikanMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 9 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.cikanTutar.ToString(), (double)0));

                if (rdSadeceKHK.Checked)
                {
                    XLS.HucreDegerYaz(satir, sutun + 10 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.kalanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.kalanTutar.ToString(), (double)0));
                }
                else
                {
                    XLS.HucreDegerYaz(satir, sutun + 10 + ekKolon, OrtakFonksiyonlar.ConvertToDouble((depo.zimmetKisiMiktar + depo.zimmetOrtakMiktar).ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11 + ekKolon, OrtakFonksiyonlar.ConvertToDouble((depo.zimmetKisiTutar + depo.zimmetOrtakTutar).ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 12 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.kalanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 13 + ekKolon, OrtakFonksiyonlar.ConvertToDouble(depo.kalanTutar.ToString(), (double)0));

                }

                devredenMiktarToplam += depo.devredenMiktar;
                devredenTutarToplam += depo.devredenTutar;

                toplam[0] += depo.girenMiktar;
                toplam[1] += depo.girenTutar;
                toplam[2] += depo.cikanMiktar;
                toplam[3] += depo.cikanTutar;

                if (rdSadeceKHK.Checked)
                {
                    toplam[4] += depo.kalanMiktar;
                    toplam[5] += depo.kalanTutar;
                }
                else
                {
                    toplam[4] += depo.zimmetKisiMiktar + depo.zimmetOrtakMiktar;
                    toplam[5] += depo.zimmetKisiTutar + depo.zimmetOrtakTutar;
                    toplam[6] += depo.kalanMiktar;
                    toplam[7] += depo.kalanTutar;
                }
            }

            satir++;

            if (ekKolon > 0)
            {
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(devredenMiktarToplam.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(devredenTutarToplam.ToString(), (double)0));
            }

            int sutunSayac = 6;
            foreach (decimal deger in toplam)
            {
                XLS.HucreDegerYaz(satir, sutun + sutunSayac + ekKolon, OrtakFonksiyonlar.ConvertToDouble(toplam[sutunSayac - 6].ToString(), (double)0));
                sutunSayac++;
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            //string gidenAd = "TasinirDepoDurum" + kriter.harcamaKod;

            string gidenAd = "";
            if (rdSadeceKHK.Checked)
                gidenAd = "TasinirDepoDurumKHK" + kriter.harcamaKod;
            else
                gidenAd = "TasinirDepoDurum" + kriter.harcamaKod;

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu2Duzey(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, tfKriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirHurdaRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                string eskiHesap = string.Empty;
                decimal miktarToplam2Duzey = 0;
                decimal tutarToplam2Duzey = 0;
                decimal miktar = 0;
                decimal tutar = 0;
                int sayac = 0;
                int sonSatirdongu = 1;

                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    sayac++;

                    if ((!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod.Substring(0, 9)) || sayac == tif.detay.objeler.Count)
                    {
                        if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod.Substring(0, 9) && sayac == tif.detay.objeler.Count)
                            sonSatirdongu = 2;

                        for (int i = 0; i < sonSatirdongu; i++)
                        {
                            satir++;

                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);

                            XLS.HucreDegerYaz(satir, sutun + 3, tif.muhasebeKod + " - " + tif.muhasebeAd);
                            XLS.HucreDegerYaz(satir, sutun + 4, tif.harcamaKod + " - " + tif.harcamaAd);
                            XLS.HucreDegerYaz(satir, sutun + 5, tif.ambarKod + " - " + tif.ambarAd);
                            XLS.HucreDegerYaz(satir, sutun + 6, tif.fisNo);
                            XLS.HucreDegerYaz(satir, sutun + 7, tif.fisTarih.ToString());
                            XLS.HucreDegerYaz(satir, sutun + 8, tif.islemTipAd);

                            XLS.HucreDegerYaz(satir, sutun + 9, tif.nereyeGitti);

                            if (sayac == tif.detay.objeler.Count)
                            {
                                //if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                                //    miktar = detay.miktar;
                                //else
                                //    miktar = -detay.miktar;
                                miktar = detay.miktar;

                                tutar = miktar * detay.birimFiyatKDVLi;

                                if (eskiHesap == detay.hesapPlanKod.Substring(0, 9))
                                {
                                    miktarToplam2Duzey += miktar;
                                    tutarToplam2Duzey += tutar;
                                }
                                else if (i == 1 || string.IsNullOrEmpty(eskiHesap))
                                {
                                    miktarToplam2Duzey = miktar;
                                    tutarToplam2Duzey = tutar;

                                    eskiHesap = detay.hesapPlanKod.Substring(0, 9);
                                }
                            }

                            XLS.HucreDegerYaz(satir, sutun + 10, eskiHesap);
                            XLS.HucreDegerYaz(satir, sutun + 11, servisUZY.UzayDegeriStr(null, "TASHESAPPLAN", eskiHesap, true));
                            XLS.HucreDegerYaz(satir, sutun + 12, servisUZY.UzayDegeriStr(null, "TASOLCUBIRIMAD", eskiHesap.Replace(".", ""), true));

                            XLS.HucreDegerYaz(satir, sutun + 19, OrtakFonksiyonlar.ConvertToDouble(miktarToplam2Duzey.ToString(), (double)0));
                            XLS.HucreDegerYaz(satir, sutun + 20, OrtakFonksiyonlar.ConvertToDouble(tutarToplam2Duzey.ToString(), (double)0));

                            if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 13, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                                XLS.HucreDegerYaz(satir, sutun + 14, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                                XLS.HucreDegerYaz(satir, sutun + 15, tif.gAmbarKod + " - " + tif.gAmbarAd);
                            }

                            miktarToplam += miktarToplam2Duzey;
                            tutarToplam += tutarToplam2Duzey;

                            miktarToplam2Duzey = 0;
                            tutarToplam2Duzey = 0;
                        }
                    }

                    if (sayac != tif.detay.objeler.Count)
                    {
                        //if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                        //    miktar = detay.miktar;
                        //else
                        //    miktar = -detay.miktar;
                        miktar = detay.miktar;

                        tutar = miktar * detay.birimFiyatKDVLi;

                        miktarToplam2Duzey += miktar;
                        tutarToplam2Duzey += tutar;

                        eskiHesap = detay.hesapPlanKod.Substring(0, 9);
                    }
                }
            }

            //Toplamlar yazılıyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 11);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTHU002);
            XLS.KoyuYap(satir, sutun + 1, true);
            XLS.DuseyHizala(satir, sutun + 1, 1);
            XLS.HucreDegerYaz(satir, sutun + 19, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 19, true);
            XLS.HucreDegerYaz(satir, sutun + 20, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 20, true);

            //XLS.SutunGizle(sutun + 10, sutun + 11, true);
            XLS.SutunGizle(sutun, sutun + 2, true);
            XLS.SutunGizle(sutun + 13, sutun + 18, true);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, tfKriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirHurdaRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal birimFiyatToplam = 0;
            decimal amortismanArtisFarkiToplam = 0;
            decimal amortismanToplam = 0;
            decimal bedelArtisToplam = 0;
            decimal bedelToplam = 0;

            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, detay.siraNo);

                    //double amortismanYuzde = 0;
                    //if (snh.amortismanYil > 0) amortismanYuzde = Math.Round(100 / snh.amortismanYil, 2);
                    XLS.HucreDegerYaz(satir, sutun + 1, "");//İLK A% EKLENECEK
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.eAlimTarihi.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 4, tif.harcamaKod + " - " + tif.harcamaAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, tif.ambarKod + " - " + tif.ambarAd);
                    XLS.HucreDegerYaz(satir, sutun + 6, tif.fisNo);
                    XLS.HucreDegerYaz(satir, sutun + 7, tif.fisTarih.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 10, detay.hesapPlanKod);//TAŞINIR KOD
                    XLS.HucreDegerYaz(satir, sutun + 11, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 13, "");//AMORTİSMAN ARTIŞ FARKI
                    XLS.HucreDegerYaz(satir, sutun + 14, "");//AMORTİSMAN
                    XLS.HucreDegerYaz(satir, sutun + 15, "");//BEDEL ARTIŞ
                    XLS.HucreDegerYaz(satir, sutun + 16, "");//BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 17, detay.birimFiyat);

                    //XLS.HucreDegerYaz(satir, sutun, tif.muhasebeKod + " - " + tif.muhasebeAd);
                    //XLS.HucreDegerYaz(satir, sutun + 1, tif.harcamaKod + " - " + tif.harcamaAd);
                    //XLS.HucreDegerYaz(satir, sutun + 2, tif.ambarKod + " - " + tif.ambarAd);
                    //XLS.HucreDegerYaz(satir, sutun + 3, tif.fisNo);
                    //XLS.HucreDegerYaz(satir, sutun + 4, tif.fisTarih.ToString());
                    //XLS.HucreDegerYaz(satir, sutun + 5, tif.islemTipAd);

                    //XLS.HucreDegerYaz(satir, sutun + 6, tif.nereyeGitti);

                    //XLS.HucreDegerYaz(satir, sutun + 7, detay.hesapPlanKod);
                    //XLS.HucreDegerYaz(satir, sutun + 8, detay.hesapPlanAd);
                    //XLS.HucreDegerYaz(satir, sutun + 9, detay.olcuBirimAd);

                    //decimal miktar = 0;
                    //if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                    //    miktar = detay.miktar;
                    //else
                    //    miktar = -detay.miktar;

                    decimal miktar = detay.miktar;

                    decimal tutar = miktar * detay.birimFiyatKDVLi;

                    //XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyat.ToString(), (double)0));
                    //XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                    //XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(miktar.ToString(), (double)0));
                    //XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));

                    if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 13, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                        XLS.HucreDegerYaz(satir, sutun + 14, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                        XLS.HucreDegerYaz(satir, sutun + 15, tif.gAmbarKod + " - " + tif.gAmbarAd);
                    }

                    miktarToplam += miktar;
                    amortismanArtisFarkiToplam += 0;
                    amortismanToplam += 0;
                    bedelArtisToplam += 0;
                    bedelToplam += 0;
                    birimFiyatToplam += detay.birimFiyat;
                }
            }

            //Toplamlar yazılıyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
            XLS.KoyuYap(satir, 11, satir, 20, true);
            XLS.HucreDegerYaz(satir, sutun + 11, Resources.TasinirMal.FRMTHU002);
            XLS.HucreDegerYaz(satir, sutun + 13, amortismanArtisFarkiToplam);
            XLS.HucreDegerYaz(satir, sutun + 14, amortismanToplam);
            XLS.HucreDegerYaz(satir, sutun + 15, bedelArtisToplam);
            XLS.HucreDegerYaz(satir, sutun + 16, bedelToplam);
            XLS.HucreDegerYaz(satir, sutun + 17, birimFiyatToplam);

            XLS.SutunGizle(sutun + 3, sutun + 3, true);
            XLS.SutunGizle(sutun + 8, sutun + 9, true);
            XLS.SutunGizle(sutun + 12, sutun + 12, true);
            XLS.SutunGizle(sutun + 18, sutun + 20, true);
            //XLS.HucreBirlestir(satir, sutun, satir, sutun + 11);

            //XLS.KoyuYap(satir, sutun, true);
            //XLS.DuseyHizala(satir, sutun, 1);
            //XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            //XLS.KoyuYap(satir, sutun + 12, true);
            //XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            //XLS.KoyuYap(satir, sutun + 13, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen işlem türleri listesindeki her bir türün kullanıldığı
        /// işlem tiplerini bulur ve işlem tipi kodlarını ; ayracı ile birleştirerek döndürür.
        /// </summary>
        /// <param name="tur">İşlem türleri dizisi</param>
        /// <returns>; ayracı ile birleştirilmiş işlem tipi kodları döndürülür.</returns>
        private string TipListesiVer(int[] tur)
        {
            string tipler = "";
            ObjectArray turTip = new ObjectArray();
            IslemTip islemTip = new IslemTip();

            for (int i = 0; i < tur.Length; i++)
            {
                islemTip.tur = tur[i];
                turTip = servisTMM.IslemTipListele(kullanan, islemTip);
                foreach (IslemTip iT in turTip.objeler)
                {
                    if (tipler != "") tipler += ";";
                    tipler += iT.kod;
                }
            }

            return tipler;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetKisi yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetKisiYazdir()
        {
            TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new TNS.TMM.ZimmetOrtakAlanVeKisi();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.tcKimlikNo = txtKimeVerildi.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");

            ObjectArray bilgi = servisTMM.ZimmetKisi(kullanan, kriter, true);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZimmetKisi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            //XLS.HucreAdBulYaz("HarcamaAd", zoa.harcamaAd);
            //XLS.HucreAdBulYaz("HarcamaKod", zoa.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", zoa.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", zoa.muhasebeKod);
            XLS.HucreAdBulYaz("KisiAd", zoa.kisiAd);
            XLS.HucreAdBulYaz("KisiKod", zoa.tcKimlikNo);

            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 8, satir, sutun + 9);

                XLS.HucreDegerYaz(satir, sutun, detay.harcamaKod + " - " + detay.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 1, detay.ambarKod + " - " + detay.ambarAd);

                if (TasinirGenel.rfIdVarMi)
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo + " - " + detay.rfIdNo);
                else
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo);

                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilAd);
                XLS.HucreDegerYaz(satir, sutun + 5, detay.odaAd);//zoa.odaKod + "-" +
                XLS.HucreDegerYaz(satir, sutun + 6, detay.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 7, detay.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, (detay.saseNo + " " + detay.aciklama).Trim());
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu2Duzey()
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlIslemTipi), 0);
            tf.gMuhasebeKod = txtGonMuhasebe.Text.Trim();
            tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Trim();
            tf.gAmbarKod = txtGonAmbar.Text.Trim();
            tf.kimeGitti = txtKimeGitti.Text.Trim();
            tf.neredenGeldi = txtNeredenGeldi.Text.Trim();
            tf.nereyeGitti = txtNereyeGitti.Text.Trim();
            tf.adaNo = TasinirGenel.ComboDegerDondur(cmbSiralama);//SIRALAMA

            TNS.TMM.TasinirFormKriter kriter = new TNS.TMM.TasinirFormKriter();
            kriter.belgeTarihBasla = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.belgeTarihBit = new TNSDateTime(txtBitisTarih.RawText);
            kriter.hesapKodu = txtHesapPlanKod.Text.Trim();

            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, kriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirIslemMalzemeTarihce.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                string eskiHesap = string.Empty;
                decimal miktarToplam2Duzey = 0;
                decimal tutarToplam2Duzey = 0;
                decimal miktar = 0;
                decimal tutar = 0;
                int sayac = 0;
                int sonSatirdongu = 1;

                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    sayac++;

                    if ((!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod.Substring(0, 9)) || sayac == tif.detay.objeler.Count)
                    {
                        if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod.Substring(0, 9) && sayac == tif.detay.objeler.Count)
                            sonSatirdongu = 2;

                        for (int i = 0; i < sonSatirdongu; i++)
                        {
                            satir++;

                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);

                            XLS.HucreDegerYaz(satir, sutun, tif.muhasebeKod + " - " + tif.muhasebeAd);
                            XLS.HucreDegerYaz(satir, sutun + 1, tif.harcamaKod + " - " + tif.harcamaAd);
                            XLS.HucreDegerYaz(satir, sutun + 2, tif.ambarKod + " - " + tif.ambarAd);
                            XLS.HucreDegerYaz(satir, sutun + 3, tif.fisNo);
                            XLS.HucreDegerYaz(satir, sutun + 4, tif.fisTarih.ToString());
                            XLS.HucreDegerYaz(satir, sutun + 5, tif.islemTipAd);

                            if (sayac == tif.detay.objeler.Count)
                            {
                                if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                                    miktar = detay.miktar;
                                else
                                    miktar = -detay.miktar;

                                tutar = miktar * detay.birimFiyatKDVLi;

                                if (eskiHesap == detay.hesapPlanKod.Substring(0, 9))
                                {
                                    miktarToplam2Duzey += miktar;
                                    tutarToplam2Duzey += tutar;
                                }
                                else if (i == 1 || string.IsNullOrEmpty(eskiHesap))
                                {
                                    miktarToplam2Duzey = miktar;
                                    tutarToplam2Duzey = tutar;

                                    eskiHesap = detay.hesapPlanKod.Substring(0, 9);
                                }
                            }

                            XLS.HucreDegerYaz(satir, sutun + 6, eskiHesap);
                            XLS.HucreDegerYaz(satir, sutun + 7, servisUZY.UzayDegeriStr(null, "TASHESAPPLAN", eskiHesap, true));
                            XLS.HucreDegerYaz(satir, sutun + 8, servisUZY.UzayDegeriStr(null, "TASOLCUBIRIMAD", eskiHesap.Replace(".", ""), true));

                            //XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyat.ToString(), (double)0));
                            //XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktarToplam2Duzey.ToString(), (double)0));
                            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutarToplam2Duzey.ToString(), (double)0));

                            XLS.HucreDegerYaz(satir, sutun + 13, tif.faturaTarih.ToString());
                            XLS.HucreDegerYaz(satir, sutun + 14, tif.faturaNo);

                            if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 15, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                                XLS.HucreDegerYaz(satir, sutun + 16, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                                XLS.HucreDegerYaz(satir, sutun + 17, tif.gAmbarKod + " - " + tif.gAmbarAd);
                            }

                            if (tif.islemTipTur == (int)ENUMIslemTipi.TUKETIMCIKIS)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 15, tif.parselNo);//tif.kimeGitti + "-" + 
                                XLS.HucreDegerYaz(satir, sutun + 18, tif.nereyeGitti);
                            }
                            else
                            {
                                XLS.HucreDegerYaz(satir, sutun + 18, tif.neredenGeldi);
                            }

                            XLS.HucreDegerYaz(satir, sutun + 19, tif.durum == 5 ? "Onaylı" : "Onaysız");

                            miktarToplam += miktarToplam2Duzey;
                            tutarToplam += tutarToplam2Duzey;

                            miktarToplam2Duzey = 0;
                            tutarToplam2Duzey = 0;
                        }
                    }

                    if (sayac != tif.detay.objeler.Count)
                    {
                        if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                            miktar = detay.miktar;
                        else
                            miktar = -detay.miktar;

                        tutar = miktar * detay.birimFiyatKDVLi;

                        miktarToplam2Duzey += miktar;
                        tutarToplam2Duzey += tutar;

                        eskiHesap = detay.hesapPlanKod.Substring(0, 9);
                    }
                }
            }

            //Toplamlar yazılıyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 10);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTMT002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 11, true);
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 12, true);

            XLS.SutunGizle(sutun + 9, sutun + 10, true);
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void TasinirIslemTarihceRaporu()
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlIslemTipi), 0);
            tf.gMuhasebeKod = txtGonMuhasebe.Text.Trim();
            tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Trim();
            tf.gAmbarKod = txtGonAmbar.Text.Trim();
            tf.kimeGitti = txtKimeGitti.Text.Trim();
            tf.neredenGeldi = txtNeredenGeldi.Text.Trim();
            tf.nereyeGitti = txtNereyeGitti.Text.Trim();
            tf.durum = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlOnayDurum), 0);
            tf.adaNo = TasinirGenel.ComboDegerDondur(cmbSiralama);//SIRALAMA

            TNS.TMM.TasinirFormKriter kriter = new TNS.TMM.TasinirFormKriter();
            kriter.belgeTarihBasla = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.belgeTarihBit = new TNSDateTime(txtBitisTarih.RawText);
            kriter.hesapKodu = txtHesapPlanKod.Text.Trim();

            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, kriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirIslemMalzemeTarihce.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 16, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, tif.muhasebeKod + " - " + tif.muhasebeAd);
                    XLS.HucreDegerYaz(satir, sutun + 1, tif.harcamaKod + " - " + tif.harcamaAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, tif.ambarKod + " - " + tif.ambarAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, tif.fisNo);
                    XLS.HucreDegerYaz(satir, sutun + 4, tif.fisTarih.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 5, tif.islemTipAd);

                    XLS.HucreDegerYaz(satir, sutun + 6, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 7, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 8, detay.olcuBirimAd);

                    decimal miktar = 0;
                    if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                        miktar = detay.miktar;
                    else
                        miktar = -detay.miktar;

                    decimal tutar = miktar * detay.birimFiyatKDVLi;

                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyat.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));

                    XLS.HucreDegerYaz(satir, sutun + 13, tif.faturaTarih.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 14, tif.faturaNo);


                    if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 15, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                        XLS.HucreDegerYaz(satir, sutun + 16, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                        XLS.HucreDegerYaz(satir, sutun + 17, tif.gAmbarKod + " - " + tif.gAmbarAd);
                    }

                    if (tif.islemTipTur == (int)ENUMIslemTipi.TUKETIMCIKIS)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 15, tif.parselNo);//tif.kimeGitti + "-" + 
                        XLS.HucreDegerYaz(satir, sutun + 18, tif.nereyeGitti);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, sutun + 18, tif.neredenGeldi);
                    }

                    XLS.HucreDegerYaz(satir, sutun + 19, tif.durum == 5 ? "Onaylı" : "Onaysız");

                    miktarToplam += miktar;
                    tutarToplam += tutar;
                }
            }

            //Toplamlar yazılıyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 10);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTMT002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 11, true);
            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 12, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// Belge Kayıt Kütüğü rapor formatında çıktı üretir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void BelgeKayitKutuguRaporu()
        {
            TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
            tf.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            tf.muhasebeKod = txtMuhasebe.Text.Trim();
            tf.harcamaKod = txtHarcamaBirimi.Text.Trim();
            tf.ambarKod = txtAmbar.Text.Trim();
            tf.islemTipKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlIslemTipi), 0);
            tf.gMuhasebeKod = txtGonMuhasebe.Text.Trim();
            tf.gHarcamaKod = txtGonHarcamaBirimi.Text.Trim();
            tf.gAmbarKod = txtGonAmbar.Text.Trim();
            tf.kimeGitti = txtKimeGitti.Text.Trim();
            tf.neredenGeldi = txtNeredenGeldi.Text.Trim();
            tf.nereyeGitti = txtNereyeGitti.Text.Trim();

            TNS.TMM.TasinirFormKriter kriter = new TNS.TMM.TasinirFormKriter();
            kriter.belgeTarihBasla = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.belgeTarihBit = new TNSDateTime(txtBitisTarih.RawText);
            kriter.hesapKodu = txtHesapPlanKod.Text.Trim();

            if (string.IsNullOrEmpty(tf.muhasebeKod) || string.IsNullOrEmpty(tf.harcamaKod))
            {
                GenelIslemler.MesajKutusu("Uyarı", Resources.TasinirMal.FRMTMT026);
                return;
            }

            ObjectArray bilgi = servisTMM.TasinirIslemMalzemeTarihceRaporu(kullanan, tf, kriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BelgeKayitKutugu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            TNS.TMM.TasinirIslemForm tifIlk = (TNS.TMM.TasinirIslemForm)bilgi.objeler[0];
            XLS.HucreAdBulYaz("Muhasebe", tifIlk.muhasebeKod + " - " + tifIlk.muhasebeAd + " / " + tifIlk.harcamaKod + " - " + tifIlk.harcamaAd);
            XLS.HucreAdBulYaz("Yil", tifIlk.yil);

            satir = kaynakSatir;

            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun, tif.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 1, tif.fisTarih.ToString());

                XLS.HucreDegerYaz(satir, sutun + 2, tif.faturaNo);
                XLS.HucreDegerYaz(satir, sutun + 3, tif.faturaTarih.ToString());

                XLS.HucreDegerYaz(satir, sutun + 4, tif.islemTipAd);

                string girisCikisNeden;
                if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                    girisCikisNeden = string.Format(Resources.TasinirMal.FRMTMT027, tif.islemTipAd);
                else
                    girisCikisNeden = string.Format(Resources.TasinirMal.FRMTMT028, tif.islemTipAd);
                XLS.HucreDegerYaz(satir, sutun + 5, girisCikisNeden);

                if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                    tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS ||
                    tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS ||
                    tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    XLS.HucreDegerYaz(satir, sutun + 10, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd + " / " + tif.gHarcamaKod + " - " + tif.gHarcamaAd);

                decimal miktar = 0;
                decimal tutar = 0;
                decimal tutarKDV = 0;
                string aciklamalar = string.Empty;
                foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                {
                    miktar += detay.miktar;
                    tutar += detay.birimFiyat * detay.miktar;
                    tutarKDV += detay.birimFiyatKDVLi * detay.miktar;
                    aciklamalar += detay.eAciklama;
                }
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 7, tutar);
                XLS.HucreDegerYaz(satir, sutun + 8, tutarKDV);
                XLS.HucreDegerYaz(satir, sutun + 9, tif.neredenGeldi);
                XLS.HucreDegerYaz(satir, sutun + 10, aciklamalar);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// İşlem tipi bilgileri sunucudan çekilir ve ddlIslemTipi DropDownList kontrolüne doldurulur.
        /// </summary>
        private void IslemTipiDoldur()
        {
            ObjectArray bilgi = servisTMM.IslemTipListele(kullanan, new IslemTip());

            List<object> liste = new List<object>();
            liste.Add(new { KOD = 0, ADI = Resources.TasinirMal.FRMTMT003 });

            foreach (IslemTip tip in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = tip.kod,
                    ADI = tip.ad
                });
            }

            strIslemTipi.DataSource = liste;
            strIslemTipi.DataBind();
        }

        /// <summary>
        /// Parametre olarak verilen demirbaş listeleme kriterlerini sunucudaki SicilRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void SicilRaporu()
        {
            SicilNoHareket kriter = new SicilNoHareket();
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaBirimKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.kimeGitti = txtKimeVerildi.Text.Trim();
            kriter.nereyeGitti = txtNereyeVerildi.Text.Trim();
            kriter.fisTarih = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.islemTarih = new TNSDateTime(txtBitisTarih.RawText);
            kriter.fiyat = OrtakFonksiyonlar.ConvertToDecimal(txtBirimFiyat.Text.Trim(), (decimal)0);
            kriter.islemTipKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlIslemTipi), 0);
            if (kriter.islemTipKod == 0)
                kriter.islemTipKod = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlGirisCikis), 0);

            kriter.listeYazdir = Path.Combine(Path.GetTempPath(), Session.SessionID);
            ObjectArray bilgi = servisTMM.SicilRaporu(kullanan, kriter);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0 && !string.IsNullOrEmpty(bilgi.sonuc.bilgiStr))
            {
                GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
                return;
            }


            TNSDateTime faturaTarih = new TNSDateTime();
            string faturaNo = "";
            string aciklamaSeriNo = "";
            if (kriter.fisNo != "")
            {
                //***************  Fatura Tarihi için Tif Oku   ********************************
                TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm
                {
                    yil = kriter.yil,
                    muhasebeKod = kriter.muhasebeKod,
                    harcamaKod = kriter.harcamaBirimKod,
                    fisNo = kriter.fisNo
                };

                ObjectArray tfListe = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, true);

                if (tfListe.sonuc.islemSonuc)
                {
                    TNS.TMM.TasinirIslemForm tform = new TNS.TMM.TasinirIslemForm();
                    tform = (TNS.TMM.TasinirIslemForm)tfListe[0];

                    faturaTarih = tform.faturaTarih;
                    faturaNo = tform.faturaNo;
                    aciklamaSeriNo = tform.aciklama;
                }
            }
            //****************************************************************

            if (!File.Exists(kriter.listeYazdir)) return;

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirSicilRaporu.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            const int BufferSize = 128;
            using (var fileStream = File.OpenRead(kriter.listeYazdir))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] alanlar = line.Split('|');

                    satir++;

                    //XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                    if (TasinirGenel.tasinirSicilNoRFIDFarkli)
                        XLS.HucreDegerYaz(satir, sutun, alanlar[0] + " -" + alanlar[1]);
                    else
                        XLS.HucreDegerYaz(satir, sutun, alanlar[0]);

                    double amortismanYuzde = 0;
                    if (OrtakFonksiyonlar.ConvertToDbl(alanlar[2]) > 0) amortismanYuzde = Math.Round(100 / OrtakFonksiyonlar.ConvertToDbl(alanlar[2]), 2);

                    XLS.HucreDegerYaz(satir, sutun + 1, alanlar[3]);
                    XLS.HucreDegerYaz(satir, sutun + 2, alanlar[4]);
                    XLS.HucreDegerYaz(satir, sutun + 3, amortismanYuzde.ToString("0.00"));
                    XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(alanlar[5], (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(alanlar[6], (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 6, alanlar[7]);
                    XLS.HucreDegerYaz(satir, sutun + 7, alanlar[8] + " " + alanlar[9]);

                    if (!string.IsNullOrEmpty(alanlar[10]))
                        XLS.HucreDegerYaz(satir, sutun + 8, alanlar[10]);
                    else if (OrtakFonksiyonlar.ConvertToInt(alanlar[11], 0) == (int)ENUMIslemTipi.ZFVERME)
                        XLS.HucreDegerYaz(satir, sutun + 8, Resources.TasinirMal.FRMTSR002);
                    else if (OrtakFonksiyonlar.ConvertToInt(alanlar[11], 0) == (int)ENUMIslemTipi.DTLVERME)
                        XLS.HucreDegerYaz(satir, sutun + 8, Resources.TasinirMal.FRMTSR003);
                    else if (OrtakFonksiyonlar.ConvertToInt(alanlar[11], 0) == (int)ENUMIslemTipi.ZFDUSME)
                        XLS.HucreDegerYaz(satir, sutun + 8, Resources.TasinirMal.FRMTSR004);
                    else if (OrtakFonksiyonlar.ConvertToInt(alanlar[11], 0) == (int)ENUMIslemTipi.DTLDUSME)
                        XLS.HucreDegerYaz(satir, sutun + 8, Resources.TasinirMal.FRMTSR005);

                    string aciklama = alanlar[12];
                    aciklama += " " + alanlar[13];

                    XLS.HucreDegerYaz(satir, sutun + 9, alanlar[14]);
                    XLS.HucreDegerYaz(satir, sutun + 10, alanlar[15]);
                    XLS.HucreDegerYaz(satir, sutun + 11, alanlar[16]);
                    XLS.HucreDegerYaz(satir, sutun + 12, alanlar[17]);
                    XLS.HucreDegerYaz(satir, sutun + 13, alanlar[18]);
                    XLS.HucreDegerYaz(satir, sutun + 14, alanlar[19] + "-" + alanlar[20]);
                    XLS.HucreDegerYaz(satir, sutun + 15, alanlar[21]);
                    XLS.HucreDegerYaz(satir, sutun + 16, alanlar[22]);
                    XLS.HucreDegerYaz(satir, sutun + 17, alanlar[23]);
                    XLS.HucreDegerYaz(satir, sutun + 18, aciklama.Trim()); //Açıklama/SeriNo
                    XLS.HucreDegerYaz(satir, sutun + 19, alanlar[24]);
                    XLS.HucreDegerYaz(satir, sutun + 20, alanlar[25]);
                    XLS.HucreDegerYaz(satir, sutun + 21, alanlar[26]);
                    XLS.HucreDegerYaz(satir, sutun + 22, faturaTarih.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 23, faturaNo);
                    //XLS.HucreDegerYaz(satir, sutun + 24, aciklamaSeriNo); 

                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void IslemYapanBilgileriRaporu()
        {
            string muhasebeKod = txtMuhasebe.Text.Trim();
            string harcamaBirimKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            TNSDateTime baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            TNSDateTime bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);

            ObjectArray bilgi = servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "IslemYapilanBilgiListesi.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (TarihceBilgisi tb in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, tb.yil + "/" + tb.belgeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, tb.islemYapan);
                XLS.HucreDegerYaz(satir, sutun + 2, tb.islemTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 3, tb.islem);
                XLS.HucreDegerYaz(satir, sutun + 4, tb.ipAdresi);
            }

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void YilBazindaAmortiEdilmemis(int tur)
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);

            if ((int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER == tur)
                t.raporTur = (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER;
            else if (tur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER)
                t.raporTur = (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER;
            else if (tur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS)
                t.raporTur = (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS;
            else if (tur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMEMISLER)
                t.raporTur = (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMEMISLER;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmortismanYilBazli.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            if (tur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER)
            {
                XLS.HucreAdBulYaz("baslik", "TAMAMI AMORTİ EDİLMEMİŞ (YIL BAZINDA) İCMAL LİSTESİ");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                XLS.SutunGizle(0, 1, true);
            }
            if (tur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER)
            {
                XLS.HucreAdBulYaz("baslik", "TAMAMI AMORTİ EDİLMİŞ (YIL BAZINDA) İCMAL LİSTESİ");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            }
            else if (tur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS)
            {
                XLS.HucreAdBulYaz("baslik", "TAMAMI AMORTİ EDİLMİŞ (AMORTİ % BAZINDA) İCMAL LİSTESİ");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            }
            else if (tur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMEMISLER)
            {
                XLS.HucreAdBulYaz("baslik", "TAMAMI AMORTİ EDİLMEMİŞLER (AMORTİ % BAZINDA) İCMAL LİSTESİ");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                XLS.SutunGizle(1, 1, true);
            }

            satir = kaynakSatir;

            int toplamAdet = 0;
            int amortismanYil = 0;

            Hashtable ht = new Hashtable();
            decimal gToplamBedel = 0;
            decimal gToplamCariYildakiToplam = 0;
            decimal gToplamYillarinToplami = 0;
            decimal gToplamBirikenAmortisman = 0;

            bool oranBazli = false, yilBazli = false;
            if ((int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS == tur || (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMEMISLER == tur)
                oranBazli = true;
            else if ((int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER == tur || (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER == tur)
                yilBazli = true;

            if (bilgi.sonuc.islemSonuc)
            {

                if (oranBazli)
                    XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 2, "Amortisman Yüzdesi");

                SortedList liste = (SortedList)bilgi.objeler[0];
                foreach (DictionaryEntry dic in liste)
                {
                    decimal amortismanYuzde = 0;
                    int yil = 0;

                    if ((int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMEMISLER == tur)
                    {
                        amortismanYuzde = (decimal)dic.Key;
                        SortedList listYil = (SortedList)dic.Value;

                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                        XLS.KoyuYap(satir, sutun + 2, true);
                        XLS.HucreDegerYaz(satir, sutun + 2, string.Format("%{0} Amortisman", amortismanYuzde));


                        decimal araToplamBedel = 0;
                        decimal araToplamCariYildakiToplam = 0;
                        decimal araToplamYillarinToplami = 0;
                        decimal araToplamBirikenAmortisman = 0;

                        foreach (DictionaryEntry dic2 in listYil)
                        {
                            yil = (int)dic2.Key;
                            decimal[] d2 = (decimal[])dic2.Value;

                            araToplamBedel += d2[0];
                            araToplamCariYildakiToplam += d2[1];
                            araToplamYillarinToplami += d2[2];
                            araToplamBirikenAmortisman += d2[3];

                            satir++;

                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                            XLS.HucreDegerYaz(satir, sutun + 2, yil);
                            XLS.HucreDegerYaz(satir, sutun + 3, d2[0]);
                            XLS.HucreDegerYaz(satir, sutun + 4, d2[1]);
                            XLS.HucreDegerYaz(satir, sutun + 5, d2[2]);
                            XLS.HucreDegerYaz(satir, sutun + 6, d2[3]);

                        }

                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                        XLS.KoyuYap(satir, sutun + 2, satir, sutun + 6, true);

                        XLS.HucreDegerYaz(satir, sutun + 2, "Toplam");
                        XLS.HucreDegerYaz(satir, sutun + 3, araToplamBedel);
                        XLS.HucreDegerYaz(satir, sutun + 4, araToplamCariYildakiToplam);
                        XLS.HucreDegerYaz(satir, sutun + 5, araToplamYillarinToplami);
                        XLS.HucreDegerYaz(satir, sutun + 6, araToplamBirikenAmortisman);


                        gToplamBedel += araToplamBedel;
                        gToplamCariYildakiToplam += araToplamCariYildakiToplam;
                        gToplamYillarinToplami += araToplamYillarinToplami;
                        gToplamBirikenAmortisman += araToplamBirikenAmortisman;

                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                        XLS.SayfaYonuAta(SayfaYonu.DUSEY);
                    }
                    else
                    {
                        if ((int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER == tur)
                            yil = (int)dic.Key;
                        else
                            amortismanYuzde = (decimal)dic.Key;

                        decimal[] d = (decimal[])dic.Value;

                        gToplamBedel += d[0];
                        gToplamCariYildakiToplam += d[1];
                        gToplamYillarinToplami += d[2];
                        gToplamBirikenAmortisman += d[3];

                        toplamAdet++;

                        satir++;

                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                        XLS.HucreDegerYaz(satir, sutun + 2, oranBazli ? "%" + amortismanYuzde.ToString("F2") : yil + "");
                        XLS.HucreDegerYaz(satir, sutun + 3, d[0]);
                        XLS.HucreDegerYaz(satir, sutun + 4, d[1]);
                        XLS.HucreDegerYaz(satir, sutun + 5, d[2]);
                        XLS.HucreDegerYaz(satir, sutun + 6, d[3]);

                    }
                }
            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
            XLS.KoyuYap(satir, sutun + 2, satir, sutun + 6, true);

            XLS.HucreDegerYaz(satir, sutun + 2, "GENEL TOPLAM:");
            XLS.HucreDegerYaz(satir, sutun + 3, gToplamBedel);
            XLS.HucreDegerYaz(satir, sutun + 4, gToplamCariYildakiToplam);
            XLS.HucreDegerYaz(satir, sutun + 5, gToplamYillarinToplami);
            XLS.HucreDegerYaz(satir, sutun + 6, gToplamBirikenAmortisman);

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void SigortaListesi(int tur)
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            t.hesapPlanKod = txtHesapPlanKod2.Text.Trim().Replace(".", "");
            t.hesapPlanIslemDurum = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlHesapPlanKodIslemDurumu), 0);
            t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);

            if ((int)ENUMMBRaporTur.GENELSIGORTALISTESI == tur)
                t.raporTur = (int)ENUMMBRaporTur.GENELSIGORTALISTESI;
            else if ((int)ENUMMBRaporTur.OTOVETABLOLARHARICSIGORTALISTESI == tur)
                t.raporTur = (int)ENUMMBRaporTur.OTOVETABLOLARHARICSIGORTALISTESI;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            //XLS.SutunGizle(4, 4, true);

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "GenelSigortaListesi.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());

            if (tur == (int)ENUMMBRaporTur.GENELSIGORTALISTESI)
            {
                XLS.HucreAdBulYaz("baslik", "SİGORTA LİSTESİ");
                XLS.SutunGizle(5, 5, true);
            }
            else if (tur == (int)ENUMMBRaporTur.OTOVETABLOLARHARICSIGORTALISTESI)
            {
                XLS.HucreAdBulYaz("baslik", "OTO VE TABLOLAR HARİÇ SİGORTA LİSTESİ");
                XLS.SutunGizle(5, 5, true);
            }
            satir = kaynakSatir;

            decimal sonBedel = 0;
            decimal ilksonBedel = 0;
            int siraNo = 1;

            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, siraNo++);//MÜD. KODU
                XLS.HucreDegerYaz(satir, sutun + 1, tb.nereyeGitti);//MÜD. KODU
                XLS.HucreDegerYaz(satir, sutun + 2, tb.nereyeGittiAd);//MÜD.ADI
                XLS.HucreDegerYaz(satir, sutun + 3, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL TOPLAMI
                XLS.HucreDegerYaz(satir, sutun + 4, tb.maliyetTutar);//İLK BEDEL TOPLAMI
                XLS.HucreDegerYaz(satir, sutun + 5, "");//İLK BEDEL TOPLAMI

                sonBedel += tb.maliyetTutar + tb.degerArtisTutar;
                ilksonBedel += tb.maliyetTutar;
            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
            XLS.KoyuYap(satir, sutun + 2, satir, sutun + 4, true);

            XLS.HucreDegerYaz(satir, sutun + 2, "TOPLAM:");
            XLS.HucreDegerYaz(satir, sutun + 3, sonBedel);
            XLS.HucreDegerYaz(satir, sutun + 4, ilksonBedel);

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void YerineGoreDemirbasListesi(int tur)
        {
            AmortismanKriter t = new AmortismanKriter();
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            t.nereyeGitti = txtNereyeVerildi.Text;
            t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);

            if ((int)ENUMMBRaporTur.YERLESKEYERINEGORETOPLAMAMORTISMAN == tur)
                t.raporTur = (int)ENUMMBRaporTur.YERLESKEYERINEGORETOPLAMAMORTISMAN;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "GenelSigortaListesi.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());

            if (tur == (int)ENUMMBRaporTur.YERLESKEYERINEGORETOPLAMAMORTISMAN)
                XLS.HucreAdBulYaz("baslik", "YERLEŞKE YERİNE GÖRE DEMİRBAŞ LİSTESİ");

            satir = kaynakSatir;

            decimal sonBedel = 0;
            decimal ilksonBedel = 0;
            decimal amortisman = 0;
            int siraNo = 1;

            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, siraNo++);//MÜD. KODU
                XLS.HucreDegerYaz(satir, sutun + 1, tb.nereyeGitti);//MÜD. KODU
                XLS.HucreDegerYaz(satir, sutun + 2, tb.nereyeGittiAd);//MÜD.ADI
                XLS.HucreDegerYaz(satir, sutun + 3, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL TOPLAMI
                XLS.HucreDegerYaz(satir, sutun + 4, tb.maliyetTutar);//İLK BEDEL TOPLAMI
                XLS.HucreDegerYaz(satir, sutun + 5, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//BIRIKEN AMORTISMAN
                sonBedel += tb.maliyetTutar + tb.degerArtisTutar;
                ilksonBedel += tb.maliyetTutar;
                amortisman += tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

            XLS.KoyuYap(satir, sutun + 2, satir, sutun + 5, true);

            XLS.HucreDegerYaz(satir, sutun + 2, "GENEL TOPLAM:");
            XLS.HucreDegerYaz(satir, sutun + 3, sonBedel);
            XLS.HucreDegerYaz(satir, sutun + 4, ilksonBedel);
            XLS.HucreDegerYaz(satir, sutun + 5, amortisman);

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void MalzemeGruplarinaGoreDemirbas(int tur)
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);

            t.raporTur = tur;

            string ciktiTur = TasinirGenel.ComboDegerDondur(ddlCiktiTur);

            MalzemeGruplarinaGoreDemirbasHazirla(kullanan, servisTMM, t, ciktiTur, true);
        }

        public static string MalzemeGruplarinaGoreDemirbasHazirla(Kullanici kullanan, ITMMServis servisTMM, AmortismanKriter t, string ciktiTur, bool raporlarEkranimi)
        {
            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return "";
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return "";
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "MalzemeGruplarinaGoreDemirbas.xlt";
            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());

            if (raporlarEkranimi)
            {
                XLS.SutunGizle(1, 1, true);
                XLS.SutunGizle(4, 4, true);
                XLS.SutunGizle(7, 7, true);
            }

            satir = kaynakSatir;

            decimal adet = 0;
            decimal sonBedel = 0;
            decimal biriken = 0;
            decimal ilkBedel = 0;
            decimal cariAmortismanTutar = 0;
            int siraNo = 1;

            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, siraNo++);
                if (tb.amortismanSuresi > 0)
                    XLS.HucreDegerYaz(satir, sutun + 1, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));
                XLS.HucreDegerYaz(satir, sutun + 2, tb.hesapPlanKod + " " + tb.hesapPlanAd);//CİNSI
                XLS.HucreDegerYaz(satir, sutun + 3, tb.hesapPlanAdet);//ADET
                XLS.HucreDegerYaz(satir, sutun + 4, tb.maliyetTutar);//ILK BEDEL
                XLS.HucreDegerYaz(satir, sutun + 5, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL
                XLS.HucreDegerYaz(satir, sutun + 6, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//DÜZEL. BİRİKEN
                XLS.HucreDegerYaz(satir, sutun + 7, tb.cariMaliyetAmortismanTutar + tb.cariDegerlemeAmortismanTutar);

                adet += tb.hesapPlanAdet;
                ilkBedel += tb.maliyetTutar;
                sonBedel += tb.maliyetTutar + tb.degerArtisTutar;
                biriken += tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
                cariAmortismanTutar += tb.cariMaliyetAmortismanTutar + tb.cariDegerlemeAmortismanTutar;
            }
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
            XLS.KoyuYap(satir, sutun + 1, satir, sutun + 7, true);
            XLS.HucreDegerYaz(satir, sutun + 2, "TOPLAM:");
            XLS.HucreDegerYaz(satir, sutun + 3, adet);
            XLS.HucreDegerYaz(satir, sutun + 4, ilkBedel);
            XLS.HucreDegerYaz(satir, sutun + 5, sonBedel);
            XLS.HucreDegerYaz(satir, sutun + 6, biriken);
            XLS.HucreDegerYaz(satir, sutun + 7, cariAmortismanTutar);

            if (ciktiTur == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (ciktiTur == "2" || ciktiTur == "3")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                if (ciktiTur == "2")
                    OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }

            return XLS.SonucDosyaAd();
        }

        //private void DemirbasEsyaIhracListesiAmortiOlmamislar()
        //{
        //    AmortismanKriter t = new AmortismanKriter();
        //    t.muhasebeKod = txtMuhasebe.Text;
        //    t.harcamaKod = txtHarcamaBirimi.Text;
        //    t.ambarKod = txtAmbar.Text;
        //    t.raporTur = 15;

        //    ObjectArray bilgi = servisTMM.AmortismanRaporla2(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

        //    if (!bilgi.sonuc.islemSonuc)
        //    {
        //        GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
        //        return;
        //    }

        //    if (bilgi.objeler.Count <= 0)
        //    {
        //        GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
        //        return;
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    int satir = 0;
        //    int sutun = 0;
        //    int kaynakSatir = 0;

        //    string sonucDosyaAd = System.IO.Path.GetTempFileName();
        //    string sablonAd = "AmortismanOrtak.xlt";

        //    XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    XLS.HucreAdBulYaz("baslik", "DEMİRBAŞ EŞYA İHRAÇ LİSTESİ (AMORTİ OLMAMIŞLAR)");

        //    XLS.SutunGizle(2, 6, true);
        //    XLS.SutunGizle(8, 9, true);
        //    XLS.SutunGizle(10, 17, true);

        //    satir = kaynakSatir;

        //    decimal ilkBedel = 0;
        //    decimal bedDuzFark = 0;
        //    decimal birAmortisman = 0;
        //    decimal birAmoFark = 0;
        //    int yil = 0;

        //    foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
        //    {
        //        satir++;

        //        if (tb.yil == yil || yil == 0)
        //        {
        //            yil = tb.yil;
        //            ilkBedel += 0;//İLK BEDEL 
        //            bedDuzFark += 0;//BED. DÜZ. FARK 
        //            birAmortisman += 0;//BİR. AMORTİSMAN 
        //            birAmoFark += 0;//BİR. AMO. FARK 
        //        }
        //        else
        //        {
        //            XLS.SatirAc(satir, 1);
        //            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
        //            XLS.HucreDegerYaz(satir, sutun + 8, ilkBedel);//İLK BEDEL
        //            XLS.HucreDegerYaz(satir, sutun + 9, bedDuzFark);//BED. DÜZ. FARK
        //            XLS.HucreDegerYaz(satir, sutun + 10, birAmortisman);//BİR. AMORTİSMAN
        //            XLS.HucreDegerYaz(satir, sutun + 11, birAmoFark);//BİR. AMO. FARK

        //            yil = tb.yil;
        //            ilkBedel = 0;//İLK BEDEL 10
        //            bedDuzFark = 0;//BED. DÜZ. FARK 20 
        //            birAmortisman = 0;//BİR. AMORTİSMAN 30 
        //            birAmoFark = 0;//BİR. AMO. FARK 40
        //        }

        //        XLS.SatirAc(satir, 1);
        //        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
        //        XLS.HucreDegerYaz(satir, sutun + 0, "");//İLK AMORTİSMAN YÜZDESİ 
        //        XLS.HucreDegerYaz(satir, sutun + 1, "");//AMORTİSMAN YILI
        //        XLS.HucreDegerYaz(satir, sutun + 2, "");//YENİ DEMİRBAŞ NO
        //        XLS.HucreDegerYaz(satir, sutun + 3, "");//BİS
        //        XLS.HucreDegerYaz(satir, sutun + 4, "");//ESKİ DEMİRBAŞ NO
        //        XLS.HucreDegerYaz(satir, sutun + 5, "");//EBİS
        //        XLS.HucreDegerYaz(satir, sutun + 6, "");//MALZEME KODU
        //        XLS.HucreDegerYaz(satir, sutun + 7, "");//MALZEME ADI
        //        XLS.HucreDegerYaz(satir, sutun + 8, "");//İLK BEDEL
        //        XLS.HucreDegerYaz(satir, sutun + 9, "");//BED. DÜZ. FARK
        //        XLS.HucreDegerYaz(satir, sutun + 10, "");//BİR. AMORTİSMAN
        //        XLS.HucreDegerYaz(satir, sutun + 11, "");//BİR. AMO. FARK
        //    }
        //    satir++;

        //    if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
        //    {
        //        XLS.DosyaSaklaTamYol();
        //        OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        //    }
        //    else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
        //    {
        //        XLS.DosyaSaklamaFormatAta("pdf");
        //        XLS.DosyaSaklaTamYol();

        //        OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
        //    }
        //}

        //private void DemirbasEsyaIhracListesi()
        //{
        //    AmortismanKriter t = new AmortismanKriter();
        //    t.muhasebeKod = txtMuhasebe.Text;
        //    t.harcamaKod = txtHarcamaBirimi.Text;
        //    t.ambarKod = txtAmbar.Text;
        //    t.raporTur = 15;

        //    ObjectArray bilgi = servisTMM.AmortismanRaporla2(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

        //    if (!bilgi.sonuc.islemSonuc)
        //    {
        //        GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
        //        return;
        //    }

        //    if (bilgi.objeler.Count <= 0)
        //    {
        //        GenelIslemler.MesajKutusu("Bilgi", bilgi.sonuc.bilgiStr);
        //        return;
        //    }

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    int satir = 0;
        //    int sutun = 0;
        //    int kaynakSatir = 0;

        //    string sonucDosyaAd = System.IO.Path.GetTempFileName();
        //    string sablonAd = "AmortismanOrtak.xlt";

        //    XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

        //    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
        //    XLS.HucreAdBulYaz("baslik", "DEMİRBAŞ EŞYA İHRAÇ LİSTESİ");

        //    XLS.SutunGizle(1, 6, true);
        //    XLS.SutunGizle(11, 12, true);
        //    XLS.SutunGizle(13, 14, true);
        //    XLS.SutunGizle(15, 16, true);
        //    XLS.SutunGizle(17, 18, true);//sadece 17 olacak

        //    satir = kaynakSatir;

        //    decimal sonBedel = 0;
        //    int adet = 0;
        //    int cins = 0;

        //    foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
        //    {
        //        satir++;

        //        if (tb.yil == cins || cins == 0)
        //        {
        //            cins = 0;//tb.cins;
        //            adet += 1;
        //            sonBedel += 0;//SONBEDEL
        //        }
        //        else
        //        {
        //            XLS.SatirAc(satir, 1);
        //            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
        //            XLS.HucreDegerYaz(satir, sutun + 0, "TOPLAM ADET" + adet);
        //            XLS.HucreDegerYaz(satir, sutun + 8, sonBedel);//BİR. AMO. FARK

        //            cins = 0;//tb.cins;
        //            adet = 1;
        //            sonBedel = 0;//SONBEDEL
        //        }

        //        XLS.SatirAc(satir, 1);
        //        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
        //        XLS.HucreDegerYaz(satir, sutun + 0, "");//İLK AMORTİSMAN YÜZDESİ 
        //        XLS.HucreDegerYaz(satir, sutun + 1, "");//AMORTİSMAN TARIHI
        //        XLS.HucreDegerYaz(satir, sutun + 2, "");//YENİ DEMİRBAŞ NO
        //        XLS.HucreDegerYaz(satir, sutun + 3, "");//BİS
        //        XLS.HucreDegerYaz(satir, sutun + 4, "");//ESKİ DEMİRBAŞ NO
        //        XLS.HucreDegerYaz(satir, sutun + 5, "");//EBİS
        //        XLS.HucreDegerYaz(satir, sutun + 6, "");//CINSI
        //        XLS.HucreDegerYaz(satir, sutun + 7, "");//MARKA VE MODEL
        //        XLS.HucreDegerYaz(satir, sutun + 8, "");//SON BEDEL
        //    }
        //    satir++;

        //    if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
        //    {
        //        XLS.DosyaSaklaTamYol();
        //        OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        //    }
        //    else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
        //    {
        //        XLS.DosyaSaklamaFormatAta("pdf");
        //        XLS.DosyaSaklaTamYol();

        //        OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
        //    }
        //}

        private void DegerEkle(Dictionary<string, decimal> dict, string anahtar, decimal deger)
        {
            if (!dict.ContainsKey(anahtar))
                dict.Add(anahtar, 0);
            dict[anahtar] += deger;
        }

        private void DemirbasListesi(int tur)
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            if ((int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE == tur) //(int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE
            {
                t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
                t.nereyeGitti = txtNereyeVerildi.Text;
                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE;
            }
            else if ((int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI == tur)
            {
                t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
                t.nereyeGitti = txtNereyeVerildi.Text;
                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI;
            }
            else if ((int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU == tur)
            {
                t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
                t.belgeNo = txtBelgeNo.Text;
                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU;
                t.raporTurEkBilgi = TasinirGenel.ComboDegerDondur(ddlIhracTur2);

                string ihracEdilmislerDurum = TasinirGenel.ComboDegerDondur(ddlIhracEdilmislerDurum);
                if (!string.IsNullOrWhiteSpace(ihracEdilmislerDurum))
                    t.raporTurEkBilgi += ihracEdilmislerDurum;

                var ihracSatislariDahilEt = chkIhracSatislariDahilEt.Checked;
                if (ihracSatislariDahilEt)
                {
                    if (t.raporTurEkBilgi != "")
                        t.raporTurEkBilgi += ";";
                    t.raporTurEkBilgi += "satislariDahilEt";
                }
            }
            else if ((int)ENUMMBRaporTur.BIRIMMALZEMELISTESI == tur) //(int)ENUMMBRaporTur.BIRIMMALZEMELISTESI
            {
                t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
                t.nereyeGitti = txtNereyeVerildi.Text;
                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.BIRIMMALZEMELISTESI;
            }
            else if ((int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR == tur)
            {
                t.donem = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
                t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR;
            }
            else if ((int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR == tur)
            {
                t.donem = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
                t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR;
            }
            else if ((int)ENUMMBRaporTur.YAZILIM == tur)
            {
                //t.hesapPlanKod = "051";//YAZILIM RAPORUNU ALABİLMEK İÇİN
                t.hesapPlanKod = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMHESAPKOD") + "";

                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE;
                tur = (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE;
            }


            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int satir2 = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = 0;
            int baslaSatir2 = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmortismanOrtak.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("kaynakSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslaSatir2", ref baslaSatir2, ref sutun);

            XLS.SutunGizle(17, 20, true);

            if ((int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE == tur)
            {
                XLS.HucreAdBulYaz("baslik", "DEMİRBAŞ LİSTESİ");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                XLS.SutunGizle(2, 2, true);
                //XLS.SutunGizle(14, 14, true);
                XLS.SatirGizle(3, 4, true);
                XLS.SatirGizle(7, 7, true);
            }
            else if ((int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI == tur)
            {
                XLS.HucreAdBulYaz("baslik", "MALZEME LİSTESİ (AMORTİSMANLI VE ALIM YILI SIRALI)");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                //XLS.SutunGizle(3, 3, true);
                //XLS.SutunGizle(7, 7, true);
                //XLS.SutunGizle(9, 11, true);
                //XLS.SutunGizle(18, 18, true);
                XLS.SutunGizle(2, 2, true);
                XLS.SatirGizle(3, 4, true);
                XLS.SatirGizle(7, 7, true);
            }
            else if ((int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU == tur)
            {
                if (t.raporTurEkBilgi == "2")
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ EDİLECEK LİSTE (CİNS SIRALI)");
                else if (t.raporTurEkBilgi.StartsWith("3"))
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ LİSTESİ (CİNS SIRALI) / (GEÇİCİ)");
                else if (t.raporTurEkBilgi.StartsWith("4"))
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ EDİLECEK LİSTE (CİNS SIRALI) / (GEÇİCİ)");
                else
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ LİSTESİ (CİNS SIRALI)");

                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                XLS.SutunGizle(2, 2, true);
                XLS.SutunGizle(6, 6, true);
                XLS.SatirGizle(3, 4, true);
                XLS.SatirGizle(7, 7, true);
                XLS.SutunGizle(12, 12, true);
                XLS.SutunGizle(16, 16, true);
                XLS.SutunGizle(19, 19, false);

                //bilgi.objeler.Sort(new SiralaMalzemeKodu());

                //Malzeme cinsi sıralamasından sonra demirbaş numarasına göre sıralama yapılsın
                var siraliListe = ((TNS.TMM.AmortismanRapor[])bilgi.objeler.ToArray(typeof(TNS.TMM.AmortismanRapor))).OrderBy(x => x.hesapPlanKod).ThenBy(n => n.gorSicilNo);

                bilgi.objeler = new TNSCollection();
                foreach (TNS.TMM.AmortismanRapor item in siraliListe)
                    bilgi.objeler.Add(item);
            }
            //else if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
            //{
            //    XLS.HucreAdBulYaz("baslik", "İHRAÇ LİSTESİ (İLK A% SIRALI)");
            //    XLS.SutunGizle(5, 5, true);
            //    XLS.SutunGizle(10, 10, true);
            //    //XLS.SutunGizle(7, 7, true);
            //    //XLS.SutunGizle(9, 11, true);
            //    XLS.SatirGizle(5, 7, true);
            //}
            else if ((int)ENUMMBRaporTur.BIRIMMALZEMELISTESI == tur)
            {
                XLS.HucreAdBulYaz("baslik", "MALZEME LİSTESİ (CİNS SIRALI)");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                XLS.SutunGizle(2, 2, true);
                XLS.SutunGizle(11, 11, true);
                XLS.SutunGizle(12, 12, true);
                XLS.SutunGizle(14, 16, true);
                XLS.SatirGizle(3, 4, true);
                XLS.SatirGizle(7, 7, true);
            }
            else if ((int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR == tur || (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR == tur)
            {
                string tarih = t.yil.ToString();
                if (t.donem > 0)
                    tarih = OrtakFonksiyonlar.AyAdiGetir(t.donem) + "/" + tarih;

                XLS.HucreAdBulYaz("baslik", tarih + " DÖNEMİ İTİBARİYLE TAMAMI AMORTİ EDİLMİŞ DEMİRBAŞ EŞYA LİSTESİ");
                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());

                if ((int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR == tur)
                    XLS.HucreAdBulYaz("baslik", tarih + " DÖNEMİ İTİBARİYLE TAMAMI AMORTİ EDİLMEMİŞ DEMİRBAŞ EŞYA LİSTESİ");
                XLS.SutunGizle(2, 2, true);
                XLS.SatirGizle(3, 4, true);
                XLS.SatirGizle(7, 7, true);
            }

            satir = baslaSatir - 1;
            satir2 = baslaSatir2 - 1;

            int yil = 0;
            int toplamAdet = 0;
            string cins = "";
            int siraNo = 1;
            int amortismanSuresi = 0;

            //Hashtable ht = new Hashtable();
            Dictionary<string, decimal> liste = new Dictionary<string, decimal>();
            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                if (tur != (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE)
                {
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "adet", 1);
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "ilkBedel", tb.maliyetTutar);
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "bedDuzFark", tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "enfDuzFark", tb.enflasyonDegerArtisTutar);
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmortisman", tb.maliyetAmortismanBirikmisTutar);
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmoFark", tb.degerArtisAmortismanTutar);
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "sonBedel", tb.maliyetTutar + tb.degerArtisTutar);
                    DegerEkle(liste, tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "duzelmisBiriken", tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);

                    DegerEkle(liste, tb.hesapPlanKod + "-" + "adet", 1);
                    DegerEkle(liste, tb.hesapPlanKod + "-" + "ilkBedel", tb.maliyetTutar);
                    DegerEkle(liste, tb.hesapPlanKod + "-" + "bedDuzFark", tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);
                    DegerEkle(liste, tb.hesapPlanKod + "-" + "enfDuzFark", tb.enflasyonDegerArtisTutar);
                    DegerEkle(liste, tb.hesapPlanKod + "-" + "birAmortisman", tb.maliyetAmortismanBirikmisTutar);
                    DegerEkle(liste, tb.hesapPlanKod + "-" + "birAmoFark", tb.degerArtisAmortismanTutar);
                    DegerEkle(liste, tb.hesapPlanKod + "-" + "sonBedel", tb.maliyetTutar + tb.degerArtisTutar);
                    DegerEkle(liste, tb.hesapPlanKod + "-" + "duzelmisBiriken", tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);
                }

                DegerEkle(liste, "gToplamSonBedel", tb.maliyetTutar + tb.degerArtisTutar);
                DegerEkle(liste, "gToplamIlkBedel", tb.maliyetTutar);
                DegerEkle(liste, "gToplamBedDuzFark", tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);
                DegerEkle(liste, "gToplamEnfDuzFark", tb.enflasyonDegerArtisTutar);
                DegerEkle(liste, "gToplamBirAmortisman", tb.maliyetAmortismanBirikmisTutar);
                DegerEkle(liste, "gToplamBirAmoFark", tb.degerArtisAmortismanTutar);
                DegerEkle(liste, "gBirikenAmortisman", tb.maliyetAmortismanBirikmisTutar);
                DegerEkle(liste, "gDuzelmisBiriken", tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);
                DegerEkle(liste, "gToplamAdet", 1);

                //ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "adet"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "adet"]) + 1;
                //ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "ilkBedel"]) + tb.maliyetTutar;
                //ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "bedDuzFark"]) + tb.degerArtisTutar;
                //ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;// + tb.degerlemeAmortismanBirikmisTutar;
                //ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar + tb.enflasyonDegerArtisTutar;
                //ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                //ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "duzelmisBiriken"]) + +tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar + tb.enflasyonDegerArtisTutar;

                //ht["gToplamSonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gToplamSonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                //ht["gToplamIlkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gToplamIlkBedel"]) + tb.maliyetTutar;
                //ht["gToplamBedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gToplamBedDuzFark"]) + tb.degerArtisTutar;
                //ht["gToplamBirAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gToplamBirAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;// + tb.degerlemeAmortismanBirikmisTutar;
                //ht["gToplamBirAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gToplamBirAmoFark"]) + tb.degerArtisAmortismanTutar + tb.enflasyonDegerArtisTutar;
                //ht["gBirikenAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gBirikenAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                //ht["gDuzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gDuzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar + tb.enflasyonDegerArtisTutar;
                //ht["gToplamAdet"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gToplamAdet"]) + 1;

                //ht[tb.hesapPlanKod + "-" + "adet"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.hesapPlanKod + "-" + "adet"]) + 1;
                //ht[tb.hesapPlanKod + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.hesapPlanKod + "-" + "ilkBedel"]) + tb.maliyetTutar;
                //ht[tb.hesapPlanKod + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.hesapPlanKod + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                //ht[tb.hesapPlanKod + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.hesapPlanKod + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar + tb.enflasyonDegerArtisTutar;

                if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur || (int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI == tur)
                {
                    if (yil != tb.girisTarih.Yil && yil != 0)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 22, satir, sutun);
                        XLS.KoyuYap(satir, sutun, satir, sutun + 22, true);
                        XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);

                        XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");
                        XLS.HucreDegerYaz(satir, sutun + 3, liste[yil + "-" + amortismanSuresi + "-" + "adet"]);
                        XLS.HucreDegerYaz(satir, sutun + 10, liste[yil + "-" + amortismanSuresi + "-" + "ilkBedel"]);//İLK BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 11, liste[yil + "-" + amortismanSuresi + "-" + "bedDuzFark"]);//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 12, liste[yil + "-" + amortismanSuresi + "-" + "enfDuzFark"]);//ENF. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 13, liste[yil + "-" + amortismanSuresi + "-" + "sonBedel"]);//SON BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 14, liste[yil + "-" + amortismanSuresi + "-" + "birAmortisman"]);//BİR. AMORTİSMAN
                        XLS.HucreDegerYaz(satir, sutun + 15, liste[yil + "-" + amortismanSuresi + "-" + "birAmoFark"]);//BİR. AMO. FARK
                        XLS.HucreDegerYaz(satir, sutun + 16, liste[yil + "-" + amortismanSuresi + "-" + "duzelmisBiriken"]);//BİR. AMO. FARK
                    }
                }
                else if ((int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU == tur || (int)ENUMMBRaporTur.BIRIMMALZEMELISTESI == tur)
                {
                    if (tb.hesapPlanKod != cins && cins != "")
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                        XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);
                        XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 1);

                        XLS.HucreDegerYaz(satir, sutun + 0, "TOPLAM ADET:");
                        XLS.HucreDegerYaz(satir, sutun + 3, liste[cins + "-" + "adet"]);
                        XLS.HucreDegerYaz(satir, sutun + 10, liste[cins + "-" + "ilkBedel"]);//İLK BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 11, liste[cins + "-" + "bedDuzFark"]);//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 12, liste[cins + "-" + "enfDuzFark"]);//ENF. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 13, liste[cins + "-" + "sonBedel"]);//SON BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 14, liste[cins + "-" + "birAmortisman"]);//BİR. AMORTİSMAN
                        XLS.HucreDegerYaz(satir, sutun + 15, liste[cins + "-" + "birAmoFark"]);//BİR. AMO. FARK
                        XLS.HucreDegerYaz(satir, sutun + 16, liste[cins + "-" + "duzelmisBiriken"]); //BİR.AMO.FARK
                    }
                }

                cins = tb.hesapPlanKod;
                toplamAdet++;
                yil = tb.girisTarih.Yil;
                amortismanSuresi = tb.amortismanSuresi;

                satir++;

                //XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 22, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, siraNo++);//SIRANO
                if (tb.amortismanSuresi > 0)
                    XLS.HucreDegerYaz(satir, sutun + 1, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                XLS.HucreDegerYaz(satir, sutun + 2, tb.girisTarih.Yil);//AMORTİSMAN YILI
                XLS.HucreDegerYaz(satir, sutun + 3, tb.girisTarih.ToString());//AMORTİSMAN TARIHI
                XLS.HucreDegerYaz(satir, sutun + 4, tb.gorSicilNo);//YENİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 5, tb.disSicilNo);//ESKİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 6, tb.nereyeGittiAd);//MUDUR ADI 
                XLS.HucreDegerYaz(satir, sutun + 7, tb.hesapPlanKod);//MALZE. KODU
                XLS.HucreDegerYaz(satir, sutun + 8, tb.hesapPlanAd);//MALZEME ADI
                XLS.HucreDegerYaz(satir, sutun + 9, tb.ozellik);//MARKA VE MODEL
                XLS.HucreDegerYaz(satir, sutun + 10, tb.maliyetTutar);//ILK BEDEL
                XLS.HucreDegerYaz(satir, sutun + 11, tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);//BED. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 12, tb.enflasyonDegerArtisTutar);//ENF. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 13, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL
                XLS.HucreDegerYaz(satir, sutun + 14, tb.maliyetAmortismanBirikmisTutar);// + tb.degerlemeAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                XLS.HucreDegerYaz(satir, sutun + 15, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
                XLS.HucreDegerYaz(satir, sutun + 16, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//DÜZEL. BİRİKEN
                if (tb.faturaTarih != null)
                    XLS.HucreDegerYaz(satir, sutun + 17, tb.faturaTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 18, tb.faturaNo);

                if (tb.islemTip.kod > 0)
                {
                    if (!string.IsNullOrWhiteSpace(tb.cikisFisNo))
                        tb.cikisFisNo += "-";
                    tb.cikisFisNo += tb.islemTip.ad;
                }

                if (tb.islemTip.kod == 8)
                    XLS.ArkaPlanRenk(satir, sutun, satir + 1, sutun + 19, System.Drawing.Color.Yellow);

                XLS.HucreDegerYaz(satir, sutun + 19, tb.cikisFisNo); //Belge No
                XLS.HucreDegerYaz(satir, sutun + 20, tb.cikisTarih?.ToString()); //Belge Tarihi

                if (toplamAdet == bilgi.objeler.Count && tur != (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE && (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR != tur && (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR != tur)
                {

                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 22, satir, sutun);
                    XLS.KoyuYap(satir, sutun, satir, sutun + 22, true);
                    XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 1);

                    if (tur == (int)ENUMMBRaporTur.BIRIMMALZEMELISTESI || tur == (int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 0, "TOPLAM ADET:");
                        XLS.HucreDegerYaz(satir, sutun + 3, liste[cins + "-" + "adet"]);
                        XLS.HucreDegerYaz(satir, sutun + 10, liste[cins + "-" + "ilkBedel"]);//İLK BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 11, liste[cins + "-" + "bedDuzFark"]);//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 12, liste[cins + "-" + "enfDuzFark"]);//ENF. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 13, liste[cins + "-" + "sonBedel"]);//SON BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 14, liste[cins + "-" + "birAmortisman"]);//BİR. AMORTİSMAN
                        XLS.HucreDegerYaz(satir, sutun + 15, liste[cins + "-" + "birAmoFark"]);//BİR. AMO. FARK
                        XLS.HucreDegerYaz(satir, sutun + 16, liste[cins + "-" + "duzelmisBiriken"]); //BİR.AMO.FARK
                    }
                    else
                    {
                        XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);
                        XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");
                        XLS.HucreDegerYaz(satir, sutun + 3, liste[yil + "-" + amortismanSuresi + "-" + "adet"]);
                        XLS.HucreDegerYaz(satir, sutun + 10, liste[yil + "-" + amortismanSuresi + "-" + "ilkBedel"]);//İLK BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 11, liste[yil + "-" + amortismanSuresi + "-" + "bedDuzFark"]);//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 12, liste[yil + "-" + amortismanSuresi + "-" + "enfDuzFark"]);//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 13, liste[yil + "-" + amortismanSuresi + "-" + "sonBedel"]);//SON BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 14, liste[yil + "-" + amortismanSuresi + "-" + "birAmortisman"]);//BİR. AMORTİSMAN
                        XLS.HucreDegerYaz(satir, sutun + 15, liste[yil + "-" + amortismanSuresi + "-" + "birAmoFark"]);//BİR. AMO. FARK
                        XLS.HucreDegerYaz(satir, sutun + 16, liste[yil + "-" + amortismanSuresi + "-" + "duzelmisBiriken"]);//BİR. AMO. FARK
                        //}
                    }
                }
            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 22, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 22, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "GENEL TOPLAM:");

            //XLS.HucreDegerYaz(satir, sutun + 1, (decimal)ht["gToplamAdet"]);
            XLS.HucreDegerYaz(satir, sutun + 10, liste["gToplamIlkBedel"] /*gToplamIlkBedel*/);
            XLS.HucreDegerYaz(satir, sutun + 11, liste["gToplamBedDuzFark"] /*gToplamBedDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 12, liste["gToplamEnfDuzFark"] /*gToplamEnfDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 13, liste["gToplamSonBedel"]);
            XLS.HucreDegerYaz(satir, sutun + 14, liste["gToplamBirAmortisman"] /*gToplamBırAmortısman*/);
            XLS.HucreDegerYaz(satir, sutun + 15, liste["gToplamBirAmoFark"] /*gToplamBırAmoFark*/);
            XLS.HucreDegerYaz(satir, sutun + 16, liste["gDuzelmisBiriken"] /*gToplamBırAmoFark*/);


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void IhracAmortismanli(int tur)
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;

            if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
            {
                t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
                t.belgeNo = txtBelgeNo.Text;
                t.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
                t.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
                t.raporTur = (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI;
                t.raporTurEkBilgi = TasinirGenel.ComboDegerDondur(ddlIhracTur2);

                string ihracEdilmislerDurum = TasinirGenel.ComboDegerDondur(ddlIhracEdilmislerDurum);
                if (!string.IsNullOrWhiteSpace(ihracEdilmislerDurum))
                    t.raporTurEkBilgi += ihracEdilmislerDurum;

                var ihracSatislariDahilEt = chkIhracSatislariDahilEt.Checked;
                if (ihracSatislariDahilEt)
                {
                    if (t.raporTurEkBilgi != "")
                        t.raporTurEkBilgi += ";";
                    t.raporTurEkBilgi += "satislariDahilEt";
                }
            }

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int satir2 = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = 0;
            int baslaSatir2 = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmortismanOrtak.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("kaynakSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslaSatir2", ref baslaSatir2, ref sutun);

            if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
            {
                if (t.raporTurEkBilgi == "2")
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ EDİLECEK LİSTE (İLK A% SIRALI)");
                else if (t.raporTurEkBilgi.StartsWith("3"))
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ LİSTESİ (İLK A% SIRALI) / (GEÇİCİ)");
                else if (t.raporTurEkBilgi.StartsWith("4"))
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ EDİLECEK LİSTE (İLK A% SIRALI) / (GEÇİCİ)");
                else
                    XLS.HucreAdBulYaz("baslik", "İHRAÇ LİSTESİ (İLK A% SIRALI)");

                XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
                //XLS.SutunGizle(2, 2, true);
                //XLS.SutunGizle(6, 6, true);
                //XLS.SutunGizle(12, 12, true);
                //XLS.SutunGizle(15, 15, true);
                //XLS.SutunGizle(16, 17, true);

                XLS.SutunGizle(2, 2, true);
                XLS.SutunGizle(6, 6, true);
                //XLS.SutunGizle(13, 13, true);
                XLS.SutunGizle(17, 18, true);

                //bilgi.objeler.Sort(new SiralaAmortismaSuresi());

                //İlk amortisman sıralamasından sonra demirbaş numarasına göre sıralama yapılsın
                var siraliListe = ((TNS.TMM.AmortismanRapor[])bilgi.objeler.ToArray(typeof(TNS.TMM.AmortismanRapor))).OrderBy(x => x.amortismanSuresi).ThenBy(n => n.gorSicilNo);

                bilgi.objeler = new TNSCollection();
                foreach (TNS.TMM.AmortismanRapor item in siraliListe)
                    bilgi.objeler.Add(item);
            }

            satir = baslaSatir - 1;
            satir2 = baslaSatir2;

            //int yil = 0;
            int toplamAdet = 0;
            int siraNo = 1;
            int siraNo2 = 1;
            decimal kalanAmortisman = 0;
            bool kalanMi = false;
            bool bittiMi = false;
            decimal amortismanSuresi = 0;

            decimal gToplamSonBedel = 0;
            decimal gToplamIlkBedel = 0;
            decimal gToplamBedDuzFark = 0;
            decimal gToplamEnfDuzFark = 0;
            decimal gToplamBirAmortisman = 0;
            decimal gToplamBirAmoFark = 0;
            decimal gBirikenAmortisman = 0;
            decimal gToplamDuzelBiriken = 0;
            decimal gToplamAdet = 0;
            //int amortSuresi = 0;

            bool yilToplami1 = false;
            bool amortismanOraniToplami1 = false;
            bool yilToplami2 = false;
            bool amortismanOraniToplami2 = false;

            bool amortiOlmus = false;
            bool yilToplamiYaz = false;
            string yilAmortisman = "";


            int[] yil = new int[2];
            int[] amortSuresi = new int[2];


            Hashtable ht = new Hashtable();
            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                amortiOlmus = tb.kalanTutar > 0 ? false : true;
                int i = amortiOlmus ? 0 : 1;

                if (tb.amortismanSuresi > 0 && amortiOlmus)
                {
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "adet"] = +1;
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "ilkBedel"]) + tb.maliyetTutar;
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "bedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "enfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "enfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + tb.girisTarih.Yil + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
                }

                if (!amortiOlmus && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
                {
                    //kalanMi = true;

                    //if (bittiMi)
                    //    kalanMi = false;
                    //else
                    //    kalanMi = true;

                    kalanAmortisman = tb.kalanTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "adet"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "adet"]) + 1;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "ilkBedel"]) + tb.maliyetTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "bedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "enfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "enfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "kalan" + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;

                    ht["kgToplamSonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamSonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht["kgToplamIlkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamIlkBedel"]) + tb.maliyetTutar;
                    ht["kgToplamBedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamBedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht["kgToplamEnfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamEnfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht["kgToplamBirAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamBirAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht["kgToplamBirAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamBirAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht["kgBirikenAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgBirikenAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht["kgDuzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgDuzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
                    ht["kgToplamAdet"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamAdet"]) + 1;

                }
                else
                {
                    //bittiMi = true;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "adet"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "adet"]) + 1;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "ilkBedel"]) + tb.maliyetTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "bedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "enfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "enfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;// + tb.degerlemeAmortismanBirikmisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + tb.amortismanSuresi + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;


                    ht[tb.amortismanSuresi + "-" + "adet"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "adet"]) + 1;
                    ht[tb.amortismanSuresi + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "ilkBedel"]) + tb.maliyetTutar;
                    ht[tb.amortismanSuresi + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "bedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht[tb.amortismanSuresi + "-" + "enfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "enfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht[tb.amortismanSuresi + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;// + tb.degerlemeAmortismanBirikmisTutar;
                    ht[tb.amortismanSuresi + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht[tb.amortismanSuresi + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht[tb.amortismanSuresi + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.amortismanSuresi + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;


                    gToplamSonBedel += tb.maliyetTutar + tb.degerArtisTutar;
                    gToplamIlkBedel += tb.maliyetTutar;
                    gToplamBedDuzFark += tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    gToplamEnfDuzFark += tb.enflasyonDegerArtisTutar;
                    gToplamBirAmortisman += tb.maliyetAmortismanBirikmisTutar;// + tb.degerlemeAmortismanBirikmisTutar;
                    gToplamBirAmoFark += tb.degerArtisAmortismanTutar;
                    gBirikenAmortisman += tb.maliyetAmortismanBirikmisTutar;
                    gToplamDuzelBiriken += tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
                    gToplamAdet += 1;
                }

                if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur || (int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI == tur)
                {

                    //if (yil[i] != 0 && (yil[i] != tb.girisTarih.Yil || amortSuresi[i] != tb.amortismanSuresi))
                    if (yil[i] != 0 && (yil[i] * 100 + amortSuresi[i] != tb.girisTarih.Yil * 100 + tb.amortismanSuresi))
                    {
                        if (!amortiOlmus && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
                        {
                            satir2++;
                            satir++;
                            XLS.SatirAc(satir2, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                            XLS.KoyuYap(satir2, sutun, satir2, sutun + 20, true);
                            XLS.HucreBirlestir(satir2, sutun + 0, satir2, sutun + 2);

                            XLS.HucreDegerYaz(satir2, sutun + 0, "Alındığı Yıl Toplamı:");
                            XLS.HucreDegerYaz(satir2, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "adet"]));
                            XLS.HucreDegerYaz(satir2, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "ilkBedel"]));//İLK BEDEL
                            XLS.HucreDegerYaz(satir2, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                            XLS.HucreDegerYaz(satir2, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "enfDuzFark"]));//ENF. DÜZ. FARK
                            XLS.HucreDegerYaz(satir2, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "sonBedel"]));//SON BEDEL
                            XLS.HucreDegerYaz(satir2, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                            XLS.HucreDegerYaz(satir2, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "birAmoFark"]));//BİR. AMO. FARK
                            XLS.HucreDegerYaz(satir2, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
                        }
                        else if (amortiOlmus)
                        {
                            satir++;
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                            XLS.KoyuYap(satir, sutun, satir, sutun + 19, true);
                            XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);

                            XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");
                            XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "adet"]));
                            XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "ilkBedel"]));//İLK BEDEL
                            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "enfDuzFark"]));//ENF. DÜZ. FARK
                            XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "sonBedel"]));//SON BEDEL
                            XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                            XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "birAmoFark"]));//BİR. AMO. FARK
                            XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "duzelmisBiriken"]));//düzelmiş biriken

                            if (amortSuresi[i] != tb.amortismanSuresi)
                            {
                                satir++;
                                XLS.SatirAc(satir, 1);
                                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                                XLS.KoyuYap(satir, sutun, satir, sutun + 19, true);
                                XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);

                                XLS.HucreDegerYaz(satir, sutun + 0, "Amortisman Oranı Toplamı:");
                                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "adet"]));
                                XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "ilkBedel"]));//İLK BEDEL
                                XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                                XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "enfDuzFark"]));//ENF. DÜZ. FARK
                                XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "sonBedel"]));//SON BEDEL
                                XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                                XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "birAmoFark"]));//BİR. AMO. FARK
                                XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
                            }
                        }

                    }
                }

                toplamAdet++;

                yil[i] = tb.girisTarih.Yil;
                amortSuresi[i] = tb.amortismanSuresi;

                //yil = tb.girisTarih.Yil;
                //amortSuresi = tb.amortismanSuresi;

                //if (tb.kalanTutar > 0)
                //    kalanMi = true;

                if (!amortiOlmus && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
                {
                    satir2++;
                    satir++;

                    XLS.SatirAc(satir2, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                    XLS.HucreDegerYaz(satir2, sutun + 0, siraNo2++);//İLK AMORTİSMAN YÜZDESİ 
                    if (tb.amortismanSuresi > 0)
                        XLS.HucreDegerYaz(satir2, sutun + 1, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                    XLS.HucreDegerYaz(satir2, sutun + 2, tb.girisTarih.Yil);//AMORTİSMAN YILI
                    XLS.HucreDegerYaz(satir2, sutun + 3, tb.girisTarih.ToString());//AMORTİSMAN TARIHI
                    XLS.HucreDegerYaz(satir2, sutun + 4, tb.gorSicilNo);//YENİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir2, sutun + 5, tb.disSicilNo);//ESKİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir2, sutun + 6, tb.nereyeGittiAd);//MUDUR ADI 
                    XLS.HucreDegerYaz(satir2, sutun + 7, tb.hesapPlanKod);//CINSI
                    XLS.HucreDegerYaz(satir2, sutun + 8, tb.hesapPlanAd);//CINSI
                    XLS.HucreDegerYaz(satir2, sutun + 9, tb.ozellik);//MARKA VE MODEL
                    XLS.HucreDegerYaz(satir2, sutun + 10, tb.maliyetTutar);//ILK BEDEL
                    XLS.HucreDegerYaz(satir2, sutun + 11, tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);//BED. DÜZ. FARK
                    XLS.HucreDegerYaz(satir2, sutun + 12, tb.enflasyonDegerArtisTutar);//ENF. DÜZ. FARK
                    XLS.HucreDegerYaz(satir2, sutun + 13, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL
                    XLS.HucreDegerYaz(satir2, sutun + 14, tb.maliyetAmortismanBirikmisTutar);// + tb.degerlemeAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                    XLS.HucreDegerYaz(satir2, sutun + 15, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
                    XLS.HucreDegerYaz(satir2, sutun + 16, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//DÜZEL. BİRİKEN
                    XLS.HucreDegerYaz(satir2, sutun + 19, tb.cikisFisNo);//Belge No
                    XLS.HucreDegerYaz(satir2, sutun + 20, tb.cikisTarih?.ToString());//Belge Taihi

                }
                else
                {
                    //amortismanSuresi = tb.amortismanSuresi;
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, siraNo++);//İLK AMORTİSMAN YÜZDESİ 
                    if (tb.amortismanSuresi > 0)
                        XLS.HucreDegerYaz(satir, sutun + 1, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                    XLS.HucreDegerYaz(satir, sutun + 2, tb.girisTarih.Yil);//AMORTİSMAN YILI
                    XLS.HucreDegerYaz(satir, sutun + 3, tb.girisTarih.ToString());//AMORTİSMAN TARIHI
                    XLS.HucreDegerYaz(satir, sutun + 4, tb.gorSicilNo);//YENİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir, sutun + 5, tb.disSicilNo);//ESKİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir, sutun + 6, tb.nereyeGittiAd);//MUDUR ADI 
                    XLS.HucreDegerYaz(satir, sutun + 7, tb.hesapPlanKod);//MALZE. KODU
                    XLS.HucreDegerYaz(satir, sutun + 8, tb.hesapPlanAd);//MALZEME ADI
                    XLS.HucreDegerYaz(satir, sutun + 9, tb.ozellik);//MARKA VE MODEL
                    XLS.HucreDegerYaz(satir, sutun + 10, tb.maliyetTutar);//ILK BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 11, tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);//BED. DÜZ. FARK
                    XLS.HucreDegerYaz(satir, sutun + 12, tb.enflasyonDegerArtisTutar);//ENF. DÜZ. FARK
                    XLS.HucreDegerYaz(satir, sutun + 13, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 14, tb.maliyetAmortismanBirikmisTutar);// + tb.degerlemeAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                    XLS.HucreDegerYaz(satir, sutun + 15, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
                    XLS.HucreDegerYaz(satir, sutun + 16, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//DÜZEL. BİRİKEN
                    XLS.HucreDegerYaz(satir, sutun + 19, tb.cikisFisNo);//Belge No
                    XLS.HucreDegerYaz(satir, sutun + 20, tb.cikisTarih?.ToString());//Belge Taihi
                }

            }


            if (tur != (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE && (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR != tur && (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR != tur)
            {
                //if (!amortiOlmus && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
                //{
                satir2++;
                satir++;

                XLS.SatirAc(satir2, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                XLS.KoyuYap(satir2, sutun, satir2, sutun + 20, true);
                XLS.HucreBirlestir(satir2, sutun + 0, satir2, sutun + 1);

                //}
                //else
                //{
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);
                XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 1);
                //}

                int i = 1;

                //if (!amortiOlmus && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur)
                //{
                XLS.HucreBirlestir(satir2, sutun + 0, satir2, sutun + 2);
                XLS.HucreDegerYaz(satir2, sutun + 0, "Alındığı Yıl Toplamı:");
                XLS.HucreDegerYaz(satir2, sutun + 2, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "adet"]));
                XLS.HucreDegerYaz(satir2, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "ilkBedel"]));//İLK BEDEL
                XLS.HucreDegerYaz(satir2, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                XLS.HucreDegerYaz(satir2, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "enfDuzFark"]));//ENF. DÜZ. FARK
                XLS.HucreDegerYaz(satir2, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "sonBedel"]));//SON BEDEL
                XLS.HucreDegerYaz(satir2, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                XLS.HucreDegerYaz(satir2, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "birAmoFark"]));//BİR. AMO. FARK
                XLS.HucreDegerYaz(satir2, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "kalan" + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
                //}
                //else
                //{

                i = 0;

                XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);
                XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");
                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "adet"]));
                XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "ilkBedel"]));//İLK BEDEL
                XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "enfDuzFark"]));//ENF. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "sonBedel"]));//SON BEDEL
                XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "birAmoFark"]));//BİR. AMO. FARK,
                XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[yil[i] + "-" + amortSuresi[i] + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK,


                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);
                XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);

                XLS.HucreDegerYaz(satir, sutun + 0, "Amortisman Oranı Toplamı:");
                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "adet"]));
                XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "ilkBedel"]));//İLK BEDEL
                XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "enfDuzFark"]));//ENF. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "sonBedel"]));//SON BEDEL
                XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "birAmoFark"]));//BİR. AMO. FARK
                XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[amortSuresi[i] + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
            }


            if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == tur && kalanAmortisman > 0)
            {
                satir2++;
                satir++;

                XLS.SatirAc(satir2, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                XLS.KoyuYap(satir2, sutun, satir2, sutun + 19, true);

                XLS.HucreDegerYaz(satir2, sutun + 0, "GENEL TOPLAM:");


                //XLS.HucreDegerYaz(satir, sutun + 1, (decimal)ht["gToplamAdet"]);
                XLS.HucreDegerYaz(satir2, sutun + 10, (decimal)ht["kgToplamIlkBedel"] /*gToplamIlkBedel*/);
                XLS.HucreDegerYaz(satir2, sutun + 11, (decimal)ht["kgToplamBedDuzFark"] /*gToplamBedDuzFark*/);
                XLS.HucreDegerYaz(satir2, sutun + 12, (decimal)ht["kgToplamEnfDuzFark"] /*gToplamEnfDuzFark*/);
                XLS.HucreDegerYaz(satir2, sutun + 13, (decimal)ht["kgToplamSonBedel"]);
                XLS.HucreDegerYaz(satir2, sutun + 14, (decimal)ht["kgToplamBirAmortisman"] /*gToplamBırAmortısman*/);
                XLS.HucreDegerYaz(satir2, sutun + 15, (decimal)ht["kgToplamBirAmoFark"] /*gToplamBırAmoFark*/);
                XLS.HucreDegerYaz(satir2, sutun + 16, (decimal)ht["kgDuzelmisBiriken"]);//BİR. AMO. FARK
            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "GENEL TOPLAM:");


            //XLS.HucreDegerYaz(satir, sutun + 1, (decimal)ht["gToplamAdet"]);
            XLS.HucreDegerYaz(satir, sutun + 10, gToplamIlkBedel /*gToplamIlkBedel*/);
            XLS.HucreDegerYaz(satir, sutun + 11, gToplamBedDuzFark /*gToplamBedDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 12, gToplamEnfDuzFark /*gToplamEnfDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 13, gToplamSonBedel);
            XLS.HucreDegerYaz(satir, sutun + 14, gToplamBirAmortisman /*gToplamBırAmortısman*/);
            XLS.HucreDegerYaz(satir, sutun + 15, gToplamBirAmoFark /*gToplamBırAmoFark*/);
            XLS.HucreDegerYaz(satir, sutun + 16, gToplamDuzelBiriken /*gToplamBırAmoFark*/);


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        public static void IhracAmortismanliHazirla(Kullanici kullanan, ITMMServis servisTMM, AmortismanKriter t, string ciktiTur, bool raporlarEkranimi)
        {
            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int satir2 = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = 0;
            int baslaSatir2 = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmortismanOrtak.xlt";
            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("kaynakSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslaSatir2", ref baslaSatir2, ref sutun);

            if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur)
            {
                XLS.HucreAdBulYaz("baslik", "İHRAÇ LİSTESİ (İLK A% SIRALI)");
                XLS.SutunGizle(6, 6, true);
                //XLS.SutunGizle(11, 11, true);
                //XLS.SutunGizle(7, 7, true);
                //XLS.SutunGizle(9, 11, true);
                //XLS.SatirGizle(5, 7, true);
            }

            satir = baslaSatir - 1;
            satir2 = baslaSatir2;

            int yil = 0;
            int toplamAdet = 0;
            int siraNo = 1;
            int siraNo2 = 1;
            decimal kalanAmortisman = 0;
            bool kalanMi = false;
            bool bittiMi = false;
            decimal amortismanSuresi = 0;

            decimal gToplamSonBedel = 0;
            decimal gToplamIlkBedel = 0;
            decimal gToplamBedDuzFark = 0;
            decimal gToplamEdfDuzFark = 0;
            decimal gToplamBirAmortisman = 0;
            decimal gToplamBirAmoFark = 0;
            decimal gBirikenAmortisman = 0;
            decimal gToplamDuzelBiriken = 0;
            decimal gToplamAdet = 0;
            int amortSuresi = 0;

            Hashtable ht = new Hashtable();
            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                if (tb.amortismanSuresi > 0)
                {
                    ht[100 / tb.amortismanSuresi + "-" + "adet"] = +1;
                    ht[100 / tb.amortismanSuresi + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "ilkBedel"]) + tb.maliyetTutar;
                    ht[100 / tb.amortismanSuresi + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "bedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + "edfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "edfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht[100 / tb.amortismanSuresi + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht[100 / tb.amortismanSuresi + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
                }

                if (tb.kalanTutar > 0 && tb.toplamAmortismanTutar == 0 && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur)
                {
                    if (bittiMi)
                        kalanMi = false;
                    else
                        kalanMi = true;
                    kalanAmortisman = tb.kalanTutar;
                    ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "adet"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "adet"]) + 1;
                    ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "ilkBedel"]) + tb.maliyetTutar;
                    ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "bedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "edfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "edfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "kalan" + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;

                    ht["kgToplamSonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamSonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht["kgToplamIlkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamIlkBedel"]) + tb.maliyetTutar;
                    ht["kgToplamBedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamBedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht["kgToplamEdfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamEdfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht["kgToplamBirAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamBirAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht["kgToplamBirAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgToplamBirAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht["kgBirikenAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgBirikenAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht["kgDuzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht["kgDuzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
                    ht["kgToplamAdet"] = OrtakFonksiyonlar.ConvertToDecimal(ht["gkToplamAdet"]) + 1;

                }
                else
                {
                    bittiMi = true;
                    ht[tb.girisTarih.Yil + "-" + "adet"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "adet"]) + 1;
                    ht[tb.girisTarih.Yil + "-" + "ilkBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "ilkBedel"]) + tb.maliyetTutar;
                    ht[tb.girisTarih.Yil + "-" + "bedDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "bedDuzFark"]) + tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + "edfDuzFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "edfDuzFark"]) + tb.enflasyonDegerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + "birAmortisman"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "birAmortisman"]) + tb.maliyetAmortismanBirikmisTutar;
                    ht[tb.girisTarih.Yil + "-" + "birAmoFark"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "birAmoFark"]) + tb.degerArtisAmortismanTutar;
                    ht[tb.girisTarih.Yil + "-" + "sonBedel"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "sonBedel"]) + tb.maliyetTutar + tb.degerArtisTutar;
                    ht[tb.girisTarih.Yil + "-" + "duzelmisBiriken"] = OrtakFonksiyonlar.ConvertToDecimal(ht[tb.girisTarih.Yil + "-" + "duzelmisBiriken"]) + tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;

                    gToplamSonBedel += tb.maliyetTutar + tb.degerArtisTutar;
                    gToplamIlkBedel += tb.maliyetTutar;
                    gToplamBedDuzFark += tb.degerArtisTutar - tb.enflasyonDegerArtisTutar;
                    gToplamEdfDuzFark += tb.enflasyonDegerArtisTutar;
                    gToplamBirAmortisman += tb.maliyetAmortismanBirikmisTutar;
                    gToplamBirAmoFark += tb.degerArtisAmortismanTutar;
                    gBirikenAmortisman += tb.maliyetAmortismanBirikmisTutar;
                    gToplamDuzelBiriken += tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar;
                    gToplamAdet += 1;
                }

                if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur || (int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI == t.raporTur)
                {
                    if (yil != tb.girisTarih.Yil && yil != 0)
                    {
                        if (/*tb.kalanAmortismanSure > 1*/(bittiMi && kalanMi && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur) || gToplamAdet == 0)
                        {
                            if (gToplamAdet > 0)
                            {
                                kalanMi = false;
                                bittiMi = true;
                            }

                            //satir2++;
                            //satir++;
                            //XLS.SatirAc(satir2, 1);
                            //XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                            //XLS.KoyuYap(satir2, sutun, satir2, sutun + 19, true);
                            //XLS.HucreBirlestir(satir2, sutun + 0, satir2, sutun + 2);

                            //XLS.HucreDegerYaz(satir2, sutun + 0, "Alındığı Yıl Toplamı:");
                            //XLS.HucreDegerYaz(satir2, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "adet"]));
                            //XLS.HucreDegerYaz(satir2, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "ilkBedel"]));//İLK BEDEL
                            //XLS.HucreDegerYaz(satir2, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "sonBedel"]));//SON BEDEL
                            //XLS.HucreDegerYaz(satir2, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                            //XLS.HucreDegerYaz(satir2, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                            //XLS.HucreDegerYaz(satir2, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "birAmoFark"]));//BİR. AMO. FARK
                            //XLS.HucreDegerYaz(satir2, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
                        }
                        else /*if (kalanAmortisman == 0)*/
                        {
                            //if (kalanAmortisman > 0)
                            //    kalanMi = true;

                            bittiMi = false;
                            satir++;
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                            XLS.KoyuYap(satir, sutun, satir, sutun + 19, true);
                            XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);

                            XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");
                            XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "adet"]));
                            XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "ilkBedel"]));//İLK BEDEL
                            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                            XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "edfDuzFark"]));//EDF. DÜZ. FARK
                            XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "sonBedel"]));//SON BEDEL
                            XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                            XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "birAmoFark"]));//BİR. AMO. FARK
                            XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "duzelmisBiriken"]));//düzelmiş biriken


                            if (amortismanSuresi != tb.amortismanSuresi)
                            {
                                satir++;
                                XLS.SatirAc(satir, 1);
                                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                                XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);
                                XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);

                                XLS.HucreDegerYaz(satir, sutun + 0, "Amortisman Oranı Toplamı:");
                                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "adet"]));
                                XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "ilkBedel"]));//İLK BEDEL
                                XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                                XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "edfDuzFark"]));//EDF. DÜZ. FARK
                                XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "sonBedel"]));//SON BEDEL
                                XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                                XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "birAmoFark"]));//BİR. AMO. FARK
                                XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / amortismanSuresi + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
                            }
                        }

                    }
                }

                toplamAdet++;
                yil = tb.girisTarih.Yil;
                amortSuresi = tb.amortismanSuresi;

                if (tb.kalanTutar > 0 && tb.toplamAmortismanTutar == 0 && kalanMi && !bittiMi && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur)
                {
                    satir2++;
                    satir++;

                    XLS.SatirAc(satir2, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                    XLS.HucreDegerYaz(satir2, sutun + 0, siraNo2++);//İLK AMORTİSMAN YÜZDESİ 
                    if (tb.amortismanSuresi > 0)
                        XLS.HucreDegerYaz(satir2, sutun + 1, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                    XLS.HucreDegerYaz(satir2, sutun + 2, tb.girisTarih.Yil);//AMORTİSMAN YILI
                    XLS.HucreDegerYaz(satir2, sutun + 3, tb.girisTarih.ToString());//AMORTİSMAN TARIHI
                    XLS.HucreDegerYaz(satir2, sutun + 4, tb.gorSicilNo);//YENİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir2, sutun + 5, tb.gorSicilNo);//ESKİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir2, sutun + 6, tb.nereyeGittiAd);//MUDUR ADI 
                    XLS.HucreDegerYaz(satir2, sutun + 7, tb.hesapPlanKod);//CINSI
                    XLS.HucreDegerYaz(satir2, sutun + 8, tb.hesapPlanAd);//CINSI
                    XLS.HucreDegerYaz(satir2, sutun + 9, tb.ozellik);//MARKA VE MODEL
                    XLS.HucreDegerYaz(satir2, sutun + 10, tb.maliyetTutar);//ILK BEDEL
                    XLS.HucreDegerYaz(satir2, sutun + 11, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL
                    XLS.HucreDegerYaz(satir2, sutun + 12, tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);//BED. DÜZ. FARK
                    XLS.HucreDegerYaz(satir2, sutun + 13, tb.enflasyonDegerArtisTutar);//EDF. DÜZ. FARK
                    XLS.HucreDegerYaz(satir2, sutun + 14, tb.maliyetAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                    XLS.HucreDegerYaz(satir2, sutun + 15, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
                    XLS.HucreDegerYaz(satir2, sutun + 16, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//DÜZEL. BİRİKEN

                }
                else
                {
                    amortismanSuresi = tb.amortismanSuresi;
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, siraNo++);//İLK AMORTİSMAN YÜZDESİ 
                    if (tb.amortismanSuresi > 0)
                        XLS.HucreDegerYaz(satir, sutun + 1, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                    XLS.HucreDegerYaz(satir, sutun + 2, tb.girisTarih.Yil);//AMORTİSMAN YILI
                    XLS.HucreDegerYaz(satir, sutun + 3, tb.girisTarih.ToString());//AMORTİSMAN TARIHI
                    XLS.HucreDegerYaz(satir, sutun + 4, tb.gorSicilNo);//YENİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir, sutun + 5, tb.gorSicilNo);//ESKİ DEMİRBAŞ NO
                    XLS.HucreDegerYaz(satir, sutun + 6, tb.nereyeGittiAd);//MUDUR ADI 
                    XLS.HucreDegerYaz(satir, sutun + 7, tb.hesapPlanKod);//MALZE. KODU
                    XLS.HucreDegerYaz(satir, sutun + 8, tb.hesapPlanAd);//MALZEME ADI
                    XLS.HucreDegerYaz(satir, sutun + 9, tb.ozellik);//MARKA VE MODEL
                    XLS.HucreDegerYaz(satir, sutun + 10, tb.maliyetTutar);//ILK BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 11, tb.maliyetTutar + tb.degerArtisTutar);//SON BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 12, tb.degerArtisTutar - tb.enflasyonDegerArtisTutar);//BED. DÜZ. FARK
                    XLS.HucreDegerYaz(satir, sutun + 13, tb.enflasyonDegerArtisTutar);//EDF. DÜZ. FARK
                    XLS.HucreDegerYaz(satir, sutun + 14, tb.maliyetAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                    XLS.HucreDegerYaz(satir, sutun + 15, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
                    XLS.HucreDegerYaz(satir, sutun + 16, tb.maliyetAmortismanBirikmisTutar + tb.degerArtisAmortismanTutar);//DÜZEL. BİRİKEN
                }

                if (toplamAdet == bilgi.objeler.Count && t.raporTur != (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE && (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMEMISDEMIRBASLAR != t.raporTur && (int)ENUMMBRaporTur.TAMAMIAMORTIEDILMISDEMIRBASLAR != t.raporTur)
                {
                    if (tb.kalanTutar > 0 && tb.toplamAmortismanTutar == 0 && kalanMi && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur)
                    {
                        satir2++;
                        satir++;

                        XLS.SatirAc(satir2, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                        XLS.KoyuYap(satir2, sutun, satir2, sutun + 20, true);
                        XLS.HucreBirlestir(satir2, sutun + 0, satir2, sutun + 1);

                    }
                    else
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                        XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);
                        XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 1);
                    }


                    if (tb.kalanTutar > 0 && tb.toplamAmortismanTutar == 0 && kalanMi && (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur)
                    {
                        XLS.HucreBirlestir(satir2, sutun + 0, satir2, sutun + 2);
                        XLS.HucreDegerYaz(satir2, sutun + 0, "Alındığı Yıl Toplamı:");
                        XLS.HucreDegerYaz(satir2, sutun + 2, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "adet"]));
                        XLS.HucreDegerYaz(satir2, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "ilkBedel"]));//İLK BEDEL
                        XLS.HucreDegerYaz(satir2, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "sonBedel"]));//SON BEDEL
                        XLS.HucreDegerYaz(satir2, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir2, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "edfDuzFark"]));//EDF. DÜZ. FARK
                        XLS.HucreDegerYaz(satir2, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                        XLS.HucreDegerYaz(satir2, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "birAmoFark"]));//BİR. AMO. FARK
                        XLS.HucreDegerYaz(satir2, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "kalan" + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
                    }
                    else
                    {
                        XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);
                        XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");
                        XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "adet"]));
                        XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "ilkBedel"]));//İLK BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "sonBedel"]));//SON BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "edfDuzFark"]));//EDF. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                        XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "birAmoFark"]));//BİR. AMO. FARK,
                        XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[yil + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK,


                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
                        XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);
                        XLS.HucreBirlestir(satir, sutun + 0, satir, sutun + 2);

                        XLS.HucreDegerYaz(satir, sutun + 0, "Amortisman Oranı Toplamı:");
                        XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "adet"]));
                        XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "ilkBedel"]));//İLK BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "sonBedel"]));//SON BEDEL
                        XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "bedDuzFark"]));//BED. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "edfDuzFark"]));//EDF. DÜZ. FARK
                        XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "birAmortisman"]));//BİR. AMORTİSMAN
                        XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "birAmoFark"]));//BİR. AMO. FARK
                        XLS.HucreDegerYaz(satir, sutun + 16, OrtakFonksiyonlar.ConvertToDecimal(ht[100 / tb.amortismanSuresi + "-" + "duzelmisBiriken"]));//BİR. AMO. FARK
                    }
                }
            }

            if ((int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI == t.raporTur && kalanAmortisman > 0)
            {
                satir2++;
                satir++;

                XLS.SatirAc(satir2, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir2, sutun);
                XLS.KoyuYap(satir2, sutun, satir2, sutun + 20, true);

                XLS.HucreDegerYaz(satir2, sutun + 0, "GENEL TOPLAM:");


                //XLS.HucreDegerYaz(satir, sutun + 1, (decimal)ht["gToplamAdet"]);
                XLS.HucreDegerYaz(satir2, sutun + 10, (decimal)ht["kgToplamIlkBedel"] /*gToplamIlkBedel*/);
                XLS.HucreDegerYaz(satir2, sutun + 11, (decimal)ht["kgToplamSonBedel"]);
                XLS.HucreDegerYaz(satir2, sutun + 12, (decimal)ht["kgToplamBedDuzFark"] /*gToplamBedDuzFark*/);
                XLS.HucreDegerYaz(satir2, sutun + 13, (decimal)ht["kgToplamEdfDuzFark"] /*gToplamEdfDuzFark*/);
                XLS.HucreDegerYaz(satir2, sutun + 14, (decimal)ht["kgToplamBirAmortisman"] /*gToplamBırAmortısman*/);
                XLS.HucreDegerYaz(satir2, sutun + 15, (decimal)ht["kgToplamBirAmoFark"] /*gToplamBırAmoFark*/);
                XLS.HucreDegerYaz(satir2, sutun + 16, (decimal)ht["kgDuzelmisBiriken"]);//BİR. AMO. FARK
            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 21, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 20, true);

            XLS.HucreDegerYaz(satir, sutun + 0, "GENEL TOPLAM:");


            //XLS.HucreDegerYaz(satir, sutun + 1, (decimal)ht["gToplamAdet"]);
            XLS.HucreDegerYaz(satir, sutun + 10, gToplamIlkBedel /*gToplamIlkBedel*/);
            XLS.HucreDegerYaz(satir, sutun + 11, gToplamSonBedel);
            XLS.HucreDegerYaz(satir, sutun + 12, gToplamBedDuzFark /*gToplamBedDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 13, gToplamEdfDuzFark /*gToplamEdfDuzFark*/);
            XLS.HucreDegerYaz(satir, sutun + 14, gToplamBirAmortisman /*gToplamBırAmortısman*/);
            XLS.HucreDegerYaz(satir, sutun + 15, gToplamBirAmoFark /*gToplamBırAmoFark*/);
            XLS.HucreDegerYaz(satir, sutun + 16, gToplamDuzelBiriken /*gToplamBırAmoFark*/);

            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());

        }

        private void BirBirimdekiMalzemelerinSorgulanmasi()
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmortismanOrtak.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("baslik", "Bir Birimdeki Malzemelerin Sorgulanması");

            XLS.SutunGizle(2, 2, true);
            XLS.SutunGizle(6, 6, true);
            XLS.SutunGizle(8, 8, true);
            XLS.SutunGizle(9, 9, true);
            XLS.SutunGizle(10, 10, true);
            XLS.SutunGizle(16, 19, true);

            satir = kaynakSatir;

            decimal ilkBedel = 0;
            decimal bedDuzFark = 0;
            decimal birAmortisman = 0;
            decimal birAmoFark = 0;
            int yil = 0;

            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                satir++;

                if (tb.girisTarih.Yil == yil || yil == 0)
                {
                    yil = tb.girisTarih.Yil;
                    ilkBedel += tb.maliyetTutar;//İLK BEDEL 
                    bedDuzFark += tb.degerArtisTutar;//BED. DÜZ. FARK 
                    birAmortisman += tb.maliyetAmortismanBirikmisTutar;//BİR. AMORTİSMAN 
                    birAmoFark += tb.degerArtisAmortismanTutar;//BİR. AMO. FARK 
                }
                else
                {
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");//İLK BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 13, ilkBedel);//İLK BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 14, bedDuzFark);//BED. DÜZ. FARK
                    XLS.HucreDegerYaz(satir, sutun + 15, birAmortisman);//BİR. AMORTİSMAN
                    XLS.HucreDegerYaz(satir, sutun + 16, birAmoFark);//BİR. AMO. FARK

                    yil = tb.girisTarih.Yil;
                    ilkBedel = tb.maliyetTutar;//İLK BEDEL 10
                    bedDuzFark = tb.degerArtisTutar;//BED. DÜZ. FARK 20 
                    birAmortisman = tb.maliyetAmortismanBirikmisTutar;//BİR. AMORTİSMAN 30 
                    birAmoFark = tb.degerArtisAmortismanTutar;//BİR. AMO. FARK 40
                    satir++;
                }

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                if (tb.amortismanSuresi > 0)
                    XLS.HucreDegerYaz(satir, sutun + 0, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                XLS.HucreDegerYaz(satir, sutun + 1, tb.girisTarih.Yil);//AMORTİSMAN YILI
                XLS.HucreDegerYaz(satir, sutun + 3, tb.disSicilNo);//YENİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 4, tb.bisNo);//BİS
                XLS.HucreDegerYaz(satir, sutun + 5, "");//ESKİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 7, tb.eskiBisNo);//EBİS
                XLS.HucreDegerYaz(satir, sutun + 11, "");//MALZEME KODU
                XLS.HucreDegerYaz(satir, sutun + 12, "");//MALZEME ADI
                XLS.HucreDegerYaz(satir, sutun + 13, tb.maliyetTutar);//İLK BEDEL
                XLS.HucreDegerYaz(satir, sutun + 14, tb.degerArtisTutar); //BED. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 15, tb.maliyetAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                XLS.HucreDegerYaz(satir, sutun + 16, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
            }
            //satir++;

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void DemirbasEsyaIhracListesiIlkASirali()
        {
            AmortismanKriter t = new AmortismanKriter();
            t.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            t.muhasebeKod = txtMuhasebe.Text;
            t.harcamaKod = txtHarcamaBirimi.Text;
            t.ambarKod = txtAmbar.Text;
            t.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, t);//servisTMM.IslemFormTarihceRapor(kullanan, muhasebeKod, harcamaBirimKod, baslamaTarihi, bitisTarihi);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmortismanOrtak.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("baslik", "İhraç Listesi(Amortisman Sütunlu) İlk A% Sıralı");

            XLS.SutunGizle(2, 2, true);
            XLS.SutunGizle(6, 6, true);
            XLS.SutunGizle(8, 10, true);
            XLS.SutunGizle(16, 19, true);

            satir = kaynakSatir;

            decimal ilkBedel = 0;
            decimal bedDuzFark = 0;
            decimal birAmortisman = 0;
            decimal birAmoFark = 0;
            int yil = 0;

            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                satir++;

                if (tb.girisTarih.Yil == yil || yil == 0)
                {
                    yil = tb.girisTarih.Yil;
                    ilkBedel += tb.maliyetTutar;//İLK BEDEL 
                    bedDuzFark += tb.degerArtisTutar;//BED. DÜZ. FARK 
                    birAmortisman += tb.maliyetAmortismanBirikmisTutar;//BİR. AMORTİSMAN 
                    birAmoFark += tb.degerArtisAmortismanTutar;//BİR. AMO. FARK 
                }
                else
                {
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, "Alındığı Yıl Toplamı:");//İLK BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 13, ilkBedel);//İLK BEDEL
                    XLS.HucreDegerYaz(satir, sutun + 14, bedDuzFark);//BED. DÜZ. FARK
                    XLS.HucreDegerYaz(satir, sutun + 15, birAmortisman);//BİR. AMORTİSMAN
                    XLS.HucreDegerYaz(satir, sutun + 16, birAmoFark);//BİR. AMO. FARK

                    yil = tb.girisTarih.Yil;
                    ilkBedel = tb.maliyetTutar;//İLK BEDEL 10
                    bedDuzFark = tb.degerArtisTutar;//BED. DÜZ. FARK 20 
                    birAmortisman = tb.maliyetAmortismanBirikmisTutar;//BİR. AMORTİSMAN 30 
                    birAmoFark = tb.degerArtisAmortismanTutar;//BİR. AMO. FARK 40
                    satir++;
                }

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                if (tb.amortismanSuresi > 0)
                    XLS.HucreDegerYaz(satir, sutun + 0, AmortismanBilgi.AmartismanYuzdesiGetir(tb.amortismanSuresi));//İLK AMORTİSMAN YÜZDESİ 
                XLS.HucreDegerYaz(satir, sutun + 1, tb.girisTarih.Yil);//AMORTİSMAN YILI
                XLS.HucreDegerYaz(satir, sutun + 3, tb.disSicilNo);//YENİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 4, tb.bisNo);//BİS
                XLS.HucreDegerYaz(satir, sutun + 5, "");//ESKİ DEMİRBAŞ NO
                XLS.HucreDegerYaz(satir, sutun + 7, tb.eskiBisNo);//EBİS
                XLS.HucreDegerYaz(satir, sutun + 11, "");//MALZEME KODU
                XLS.HucreDegerYaz(satir, sutun + 12, "");//MALZEME ADI
                XLS.HucreDegerYaz(satir, sutun + 13, tb.maliyetTutar);//İLK BEDEL
                XLS.HucreDegerYaz(satir, sutun + 14, tb.degerArtisTutar); //BED. DÜZ. FARK
                XLS.HucreDegerYaz(satir, sutun + 15, tb.maliyetAmortismanBirikmisTutar);//BİR. AMORTİSMAN
                XLS.HucreDegerYaz(satir, sutun + 16, tb.degerArtisAmortismanTutar);//BİR. AMO. FARK
            }
            //satir++;

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void SosyalTesislereAyrilanAmortismanListesi()
        {
            AmortismanKriter form = new AmortismanKriter();
            form.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            form.donem = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
            form.muhasebeKod = txtMuhasebe.Text;
            form.harcamaKod = txtHarcamaBirimi.Text;
            form.ambarKod = txtAmbar.Text;
            form.baslamaTarihi = new TNSDateTime(txtBaslangicTarih.RawText);
            form.bitisTarihi = new TNSDateTime(txtBitisTarih.RawText);
            form.raporTur = (int)ENUMMBRaporTur.SOSYALTESISLEREAYRILANCARIAMAMORTISMAN;

            ObjectArray bilgi = servisTMM.AmortismanRaporuMB(kullanan, form);

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count <= 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", "Verilen kriterlere uygun kayıt bulunamadı.");
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "SosyalTesislereAyrilanCariAmortisman.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("baslik", string.Format("{0} YILI {1}SOSYAL TESİSLERE AYRILAN CARİ AMORTİSMAN TUTARLARI", form.yil, (form.donem > 0 ? form.donem + ".DÖNEM " : "")));
            XLS.HucreAdBulYaz("raporTarihi", new TNSDateTime(DateTime.Now.ToShortDateString()).ToString());
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir + 3;

            decimal toplam = 0;
            foreach (TNS.TMM.AmortismanRapor tb in bilgi.objeler)
            {
                satir++;

                //XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 1, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun, tb.nereyeGittiAd);
                XLS.HucreDegerYaz(satir, sutun + 1, tb.cariToplamAmortismanTutar);
                toplam += tb.cariToplamAmortismanTutar;
            }
            satir++;

            //XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 1, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 1, true);
            XLS.HucreDegerYaz(satir, sutun, "GENEL TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 1, toplam);

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişi kriterlerini sunucudaki GirisiYapilmamisDevirCikislari
        /// yordamına gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        private void DevirCikisRaporu()
        {
            TNS.TMM.TasinirIslemForm kriter = new TNS.TMM.TasinirIslemForm();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.islemTipTur = (int)ENUMIslemTipi.DEVIRGIRIS;

            ObjectArray bilgi = servisTMM.GirisiYapilmamisDevirCikislari(kullanan, kriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "DevirCikis.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, tif.yil);
                XLS.HucreDegerYaz(satir, sutun + 1, tif.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 2, tif.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 3, tif.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 4, tif.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 5, tif.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 6, tif.gMuhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 7, tif.gHarcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 8, tif.gAmbarAd);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));


            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }
        private void IlDoldur()
        {
            ObjectArray bilgi = servisTMM.IlListele(kullanan, new Il());

            List<object> liste = new List<object>();
            foreach (Il ilimiz in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ilimiz.kod,
                    ADI = ilimiz.ad
                });
            }

            strIl.DataSource = liste;
            strIl.DataBind();
        }

        protected void IlceDoldur(object sender, StoreRefreshDataEventArgs e)
        {
            IlceDoldur(ddlIl.Value != null ? ddlIl.Value.ToString() : string.Empty);
        }

        private void IlceDoldur(string ilKod)
        {
            if (ilKod == "")
                return;

            ddlIlce.Items.Clear();

            Ilce ilce = new Ilce();
            ilce.ilKodu = ilKod;

            ObjectArray bilgi = servisTMM.IlceListele(kullanan, ilce);

            List<object> liste = new List<object>();
            foreach (Ilce ilcemiz in bilgi.objeler)
            {
                liste.Add(new
                {
                    KOD = ilcemiz.kod,
                    ADI = ilcemiz.ad
                });
            }
            strIlce.DataSource = liste;
            strIlce.DataBind();
        }


        /// <summary>
        /// Parametre olarak verilen depo durum kriterlerini sunucudaki taşınır depo
        /// durum yordamına gönderir, sunucudan gelen en çok kullanılan taşınırlar
        /// detay raporu bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void EnCokKullanilanlarRaporuDetay()
        {
            DepoDurum kriter = new DepoDurum();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.muhasebeAd = lblMuhasebeAd.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kriter.harcamaAd = lblHarcamaBirimiAd.Text.Trim();
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.ambarAd = lblAmbarAd.Text.Trim();
            kriter.ilKod = TasinirGenel.ComboDegerDondur(ddlIl);
            kriter.ilAd = TasinirGenel.ComboAdDondur(ddlIl);
            kriter.ilceKod = TasinirGenel.ComboDegerDondur(ddlIlce);
            kriter.ilceAd = TasinirGenel.ComboAdDondur(ddlIlce);
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.hesapPlanAd = lblHesapPlanAd.Text.Trim();
            kriter.donem = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlDonem), 0);

            //Detay raporu için
            if (rdMuhasebeBazinda.Checked)
                kriter.raporTur = 0;
            if (rdHarcamaBazinda.Checked)
                kriter.raporTur = 1;
            if (rdIlBazinda.Checked)
                kriter.raporTur = 2;
            kriter.yilDevri = true;

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

            ObjectArray personelSayilari = servisTMM.PersonelSayilari(kullanan, kriter);

            //ObjectArray personelSayilariMuhasebe = null;
            //DepoDurum kriterMuhasebe = null;
            //if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
            //{
            //    kriterMuhasebe = KriterTopla();
            //    kriterMuhasebe.raporTur = (int)ENUMDepoDurumRaporTur.MUHASEBE;
            //    personelSayilariMuhasebe = servisTMM.PersonelSayilari(kullanan, kriterMuhasebe);
            //}

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "EnCokKullanilanlar.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.SutunGizle(sutun + 9, sutun + 11, true);

            XLS.HucreAdBulYaz("Yil", kriter.yil);
            XLS.HucreAdBulYaz("Muhasebe", kriter.muhasebeKod + " - " + kriter.muhasebeAd);

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                XLS.HucreAdBulYaz("Harcama", kriter.harcamaKod + " - " + kriter.harcamaAd);
            else
                XLS.HucreAdBulYaz("Harcama", GenelIslemlerIstemci.VarsayilanKurumBulAd());

            XLS.HucreAdBulYaz("Ambar", kriter.ambarKod + " - " + kriter.ambarAd);
            XLS.HucreAdBulYaz("Il", kriter.ilKod + " - " + kriter.ilAd);
            XLS.HucreAdBulYaz("Ilce", kriter.ilceKod + " - " + kriter.ilceAd);
            XLS.HucreAdBulYaz("Tasinir", kriter.hesapPlanKod + " - " + kriter.hesapPlanAd);

            satir = kaynakSatir;

            if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
            {
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun, Resources.TasinirMal.FRMECK003);
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 1, Resources.TasinirMal.FRMECK004);
            }
            else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
            {
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun, Resources.TasinirMal.FRMECK005);
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 1, Resources.TasinirMal.FRMECK006);
            }
            else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
            {
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun, Resources.TasinirMal.FRMECK007);
                XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 1, Resources.TasinirMal.FRMECK008);
            }

            int personelSayisi = 0;
            decimal[] toplam = new decimal[7];
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                DepoDurum depo = (DepoDurum)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);

                if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                {
                    personelSayisi = PersonelSayisiDondur(personelSayilari.objeler, kriter.raporTur, depo.muhasebeKod);

                    XLS.HucreDegerYaz(satir, sutun, depo.muhasebeKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.muhasebeAd);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
                {
                    //if (depo.harcamaKod.Replace(".", "").Contains("12010004"))
                    //    personelSayisi = PersonelSayisiDondur(personelSayilariMuhasebe.objeler, kriterMuhasebe.raporTur, depo.muhasebeKod);
                    //else
                    personelSayisi = PersonelSayisiDondur(personelSayilari.objeler, kriter.raporTur, depo.muhasebeKod + "-" + depo.harcamaKod.Replace(".", ""));

                    XLS.HucreDegerYaz(satir, sutun, depo.muhasebeKod + " - " + depo.harcamaKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.muhasebeAd + " - " + depo.harcamaAd);
                }
                else if (kriter.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                {
                    personelSayisi = PersonelSayisiDondur(personelSayilari.objeler, kriter.raporTur, depo.ilKod);

                    XLS.HucreDegerYaz(satir, sutun, depo.ilKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.ilAd);
                }

                XLS.HucreDegerYaz(satir, sutun + 2, depo.olcuBirimAd);
                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(depo.cikanMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(depo.cikanTutar.ToString(), (double)0));

                depo.kalanMiktar = depo.girenMiktar - depo.cikanMiktar;
                depo.kalanTutar = depo.girenTutar - depo.cikanTutar;

                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(depo.kalanMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(depo.kalanTutar.ToString(), (double)0));

                if (personelSayisi > 0)
                {
                    decimal sutun14 = depo.cikanMiktar / personelSayisi;
                    XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDouble(sutun14.ToString(), (double)0));

                    decimal sutun15 = depo.cikanTutar / personelSayisi;
                    XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDouble(sutun15.ToString(), (double)0));
                }

                toplam[0] += depo.girenMiktar;
                toplam[1] += depo.girenTutar;
                toplam[2] += depo.cikanMiktar;
                toplam[3] += depo.cikanTutar;
                toplam[4] += depo.kalanMiktar;
                toplam[5] += depo.kalanTutar;
                toplam[6] += (decimal)personelSayisi;
            }

            /***Genel Toplam***
            *******************/
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 15, true);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);

            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK009);
            XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(toplam[0].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(toplam[1].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(toplam[2].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(toplam[3].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(toplam[4].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(toplam[5].ToString(), (double)0));

            if (toplam[6] > 0)
            {
                XLS.HucreDegerYaz(satir, sutun + 14, OrtakFonksiyonlar.ConvertToDouble((toplam[2] / toplam[6]).ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 15, OrtakFonksiyonlar.ConvertToDouble((toplam[3] / toplam[6]).ToString(), (double)0));
            }
            /*******************/

            for (int i = kaynakSatir + 1; i <= satir; i++)
            {
                if (i != satir)
                {
                    decimal sutun12 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / toplam[1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(sutun12.ToString(), (double)0));

                    decimal sutun13 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / toplam[1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(sutun13.ToString(), (double)0));
                }
                else
                {
                    XLS.HucreDegerYaz(i, sutun + 12, Resources.TasinirMal.FRMECK010);
                    XLS.HucreDegerYaz(i, sutun + 13, Resources.TasinirMal.FRMECK010);
                }
            }

            AciklamaYaz(XLS, satir + 3, sutun);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen personel sayı bilgileri arasından verilen
        /// rapor türüne ve koda ait personel sayısını bulup döndüren yordam
        /// </summary>
        /// <param name="objeler">Personel sayı bilgileri listesini tutan nesne</param>
        /// <param name="raporTur">ENUMDepoDurumRaporTur listesindeki değerlerden biri olmalıdır.</param>
        /// <param name="kod">Personel sayısı aranan kod kriteri</param>
        /// <returns>Personel sayısı döndürülür.</returns>
        private int PersonelSayisiDondur(TNSCollection objeler, int raporTur, string kod)
        {
            foreach (string[] ps in objeler)
            {
                if ((raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE || raporTur == (int)ENUMDepoDurumRaporTur.IL) && ps[0] == kod)
                    return OrtakFonksiyonlar.ConvertToInt(ps[1], 0);
                else if (raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA && ps[0] + "-" + ps[1].Replace(".", "") == kod)
                    return OrtakFonksiyonlar.ConvertToInt(ps[2], 0);
            }
            return 0;
        }

        /// <summary>
        /// Excel raporuna açıklama bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="satir">Açıklama bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Açıklama bilgilerinin yazılacağı sütun numarası</param>
        private void AciklamaYaz(Tablo XLS, int satir, int sutun)
        {
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 15);
            XLS.HucreBirlestir(satir + 1, sutun, satir + 1, sutun + 15);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK011);
            XLS.HucreDegerYaz(satir + 1, sutun, Resources.TasinirMal.FRMECK012);
        }

        /// <summary>
        /// Parametre olarak verilen depo durum kriterlerini sunucudaki en çok kullanılan taşınırlar
        /// yordamına gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void EnCokKullanilanlarRaporu()
        {
            DepoDurum kriter = new DepoDurum();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.muhasebeAd = lblMuhasebeAd.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kriter.harcamaAd = lblHarcamaBirimiAd.Text.Trim();
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.ambarAd = lblAmbarAd.Text.Trim();
            kriter.ilKod = TasinirGenel.ComboDegerDondur(ddlIl);
            kriter.ilAd = TasinirGenel.ComboAdDondur(ddlIl);
            kriter.ilceKod = TasinirGenel.ComboDegerDondur(ddlIlce);
            kriter.ilceAd = TasinirGenel.ComboAdDondur(ddlIlce);
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.hesapPlanAd = lblHesapPlanAd.Text.Trim();
            kriter.donem = OrtakFonksiyonlar.ConvertToInt(TasinirGenel.ComboDegerDondur(ddlDonem), 0);

            //Detay raporu için
            if (rdMuhasebeBazinda.Checked)
                kriter.raporTur = 0;
            if (rdHarcamaBazinda.Checked)
                kriter.raporTur = 1;
            if (rdIlBazinda.Checked)
                kriter.raporTur = 2;
            kriter.yilDevri = true;

            ObjectArray bilgi = servisTMM.EnCokKullanilanTasinirlar(kullanan, kriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "EnCokKullanilanlar.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", kriter.yil);
            XLS.HucreAdBulYaz("Muhasebe", kriter.muhasebeKod + " - " + kriter.muhasebeAd);

            if (!string.IsNullOrEmpty(kriter.harcamaKod))
                XLS.HucreAdBulYaz("Harcama", kriter.harcamaKod + " - " + kriter.harcamaAd);
            else
                XLS.HucreAdBulYaz("Harcama", GenelIslemlerIstemci.VarsayilanKurumBulAd());

            XLS.HucreAdBulYaz("Ambar", kriter.ambarKod + " - " + kriter.ambarAd);
            XLS.HucreAdBulYaz("Il", kriter.ilKod + " - " + kriter.ilAd);
            XLS.HucreAdBulYaz("Ilce", kriter.ilceKod + " - " + kriter.ilceAd);
            XLS.HucreAdBulYaz("Tasinir", kriter.hesapPlanKod + " - " + kriter.hesapPlanAd);

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kriter.muhasebeKod))
                kosul = kriter.muhasebeKod;
            if (!string.IsNullOrEmpty(kriter.harcamaKod))
            {
                if (!string.IsNullOrEmpty(kosul))
                    kosul += "-";
                kosul += kriter.harcamaKod.Replace(".", "");
            }

            int personelSayisi = OrtakFonksiyonlar.ConvertToInt(servisUZY.UzayDegeriDbl(kullanan, "TASPERSONELSAYISI", kosul, true).ToString(), 0);
            XLS.HucreAdBulYaz("PersonelSayisi", personelSayisi);
            XLS.HucreAdBulYaz("Donem", kriter.donem);

            satir = kaynakSatir;

            decimal[,] toplam = new decimal[4, 9];
            int toplamSatir = 0;
            int index = 0;
            int sayac = 0;
            string yazilanHesapKod = string.Empty;
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                DepoDurum depo = (DepoDurum)bilgi.objeler[i];

                if (depo.girenMiktar == 0 && depo.girenTutar == 0)
                    continue;

                if (sayac < 20 && (yazilanHesapKod == depo.hesapPlanKod.Substring(0, 3) || string.IsNullOrEmpty(yazilanHesapKod)))
                {
                    if (string.IsNullOrEmpty(yazilanHesapKod))
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);
                    }

                    yazilanHesapKod = depo.hesapPlanKod.Substring(0, 3);
                    sayac++;

                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, depo.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, depo.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, depo.olcuBirimAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(depo.cikanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(depo.cikanTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(depo.kalanMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(depo.kalanTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(depo.gecenYilTuketimMiktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(depo.gecenYilTuketimTutar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(depo.donemselTuketimTutar.ToString(), (double)0));

                    toplam[index, 0] += depo.girenMiktar;
                    toplam[index, 1] += depo.girenTutar;
                    toplam[index, 2] += depo.cikanMiktar;
                    toplam[index, 3] += depo.cikanTutar;
                    toplam[index, 4] += depo.kalanMiktar;
                    toplam[index, 5] += depo.kalanTutar;
                    toplam[index, 6] += depo.gecenYilTuketimMiktar;
                    toplam[index, 7] += depo.gecenYilTuketimTutar;
                    toplam[index, 8] += depo.donemselTuketimTutar;
                }
                else if (yazilanHesapKod != depo.hesapPlanKod.Substring(0, 3))
                {
                    toplamSatir = satir - sayac;
                    ToplamYaz(XLS, kaynakSatir, toplamSatir, sutun, yazilanHesapKod, toplam, index);

                    yazilanHesapKod = depo.hesapPlanKod.Substring(0, 3);
                    sayac = 0;
                    i--;//Atlanan bir satır var, onu yazdırabilmek için

                    if (TNS.TMM.Arac.MakineCihazMi(yazilanHesapKod))
                        index = 1;
                    else if (TNS.TMM.Arac.TasitMi(yazilanHesapKod))
                        index = 2;
                    else if (TNS.TMM.Arac.DemirbasMi(yazilanHesapKod))
                        index = 3;

                    yazilanHesapKod = string.Empty;
                }
            }

            toplamSatir = satir - sayac;
            ToplamYaz(XLS, kaynakSatir, toplamSatir, sutun, yazilanHesapKod, toplam, index);

            /***Genel Toplam***
            *******************/
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);
            XLS.KoyuYap(satir, sutun, satir, sutun + 15, true);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);

            decimal[] genelToplam = new decimal[9];
            for (int j = 0; j < toplam.Length / 9; j++)
                for (int i = 0; i < genelToplam.Length; i++)
                    genelToplam[i] += toplam[j, i];

            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMECK009);
            XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(genelToplam[0].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(genelToplam[1].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(genelToplam[2].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(genelToplam[3].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(genelToplam[4].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(genelToplam[5].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(genelToplam[6].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(genelToplam[7].ToString(), (double)0));
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(genelToplam[8].ToString(), (double)0));
            /*******************/

            for (int i = kaynakSatir + 1; i <= satir; i++)
            {
                string hesapKod = XLS.HucreDegerAl(i, sutun).Substring(0, 3);
                if (hesapKod == ((int)ENUMTasinirHesapKodu.TUKETIM).ToString())
                    index = 0;
                else if (TNS.TMM.Arac.MakineCihazMi(hesapKod))
                    index = 1;
                else if (TNS.TMM.Arac.TasitMi(hesapKod))
                    index = 2;
                else if (TNS.TMM.Arac.DemirbasMi(hesapKod))
                    index = 3;

                if (i != satir)
                {
                    decimal sutun12 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / genelToplam[1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(sutun12.ToString(), (double)0));

                    decimal sutun13 = 0;
                    if (toplam[index, 1] > 0)
                        sutun13 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 4, 4).Trim(), (decimal)0) / toplam[index, 1] * 100;
                    XLS.HucreDegerYaz(i, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(sutun13.ToString(), (double)0));
                }
                else
                {
                    XLS.HucreDegerYaz(i, sutun + 12, Resources.TasinirMal.FRMECK010);
                    XLS.HucreDegerYaz(i, sutun + 13, Resources.TasinirMal.FRMECK010);
                }

                if (personelSayisi > 0)
                {
                    decimal sutun14 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 5, 4).Trim(), (decimal)0) / personelSayisi;
                    XLS.HucreDegerYaz(i, sutun + 14, OrtakFonksiyonlar.ConvertToDouble(sutun14.ToString(), (double)0));

                    decimal sutun15 = OrtakFonksiyonlar.ConvertToDecimal(XLS.HucreDegerAl(XLS.AktifSheet(), i, sutun + 6, 4).Trim(), (decimal)0) / personelSayisi;
                    XLS.HucreDegerYaz(i, sutun + 15, OrtakFonksiyonlar.ConvertToDouble(sutun15.ToString(), (double)0));
                }
            }

            AciklamaYaz(XLS, satir + 3, sutun);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(300));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Excel raporuna en çok kullanılan taşınırlara ait toplam bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun başladığı kaynak satır numarası</param>
        /// <param name="toplamSatir">Toplam bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Toplam bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        /// <param name="yazilanHesapKod">Toplamı yazılacak taşınır hesap planı kodu</param>
        /// <param name="toplam">En çok kullanılan taşınırlara ait toplam bilgilerini tutan dizi</param>
        /// <param name="index">En çok kullanılan taşınırlara ait toplam bilgisinin dizideki indeksi</param>
        private void ToplamYaz(Tablo XLS, int kaynakSatir, int toplamSatir, int sutun, string yazilanHesapKod, decimal[,] toplam, int index)
        {
            XLS.KoyuYap(toplamSatir, sutun, toplamSatir, sutun + 15, true);

            XLS.HucreDegerYaz(toplamSatir, sutun, yazilanHesapKod);
            XLS.HucreDegerYaz(toplamSatir, sutun + 1, servisUZY.UzayDegeriStr(kullanan, "TASHESAPPLAN", yazilanHesapKod, true));
            //XLS.HucreDegerYaz(toplamSatir, sutun + 2, depo.olcuBirimAd);
            XLS.HucreDegerYaz(toplamSatir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 0].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 1].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 2].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 3].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 4].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 5].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 9, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 6].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 7].ToString(), (double)0));
            XLS.HucreDegerYaz(toplamSatir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(toplam[index, 8].ToString(), (double)0));
        }



        /// <summary>
        /// Parametre olarak verilen tüketim çıkış kriterlerini sunucudaki TuketimCikis
        /// yordamına gönderir, sunucudan gelen bilgi kümesini excel raporuna aktarır.
        /// </summary>
        /// <param name="kriter">Tüketim çıkış kriter bilgilerini tutan nesne</param>
        private void TuketimCikisYazdir()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TuketimCikis kriter = new TNS.TMM.TuketimCikis();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.tarih1 = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.tarih2 = new TNSDateTime(txtBitisTarih.RawText);

            if (TNS.TMM.Arac.MeclisKullaniyor())
                kriter.ambarKod = "";

            ObjectArray bilgi = servisTMM.TuketimCikis(kullanan, kriter, false);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TuketimCikis.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.TuketimCikis tk = (TNS.TMM.TuketimCikis)bilgi.objeler[0];
            XLS.HucreAdBulYaz("IlAd", tk.ilAd + "-" + tk.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tk.ilKod + "-" + tk.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tk.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tk.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tk.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tk.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tk.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tk.muhasebeKod);

            decimal toplamTutar = 0;
            foreach (TNS.TMM.TuketimCikisDetay detay in tk.detay)
                toplamTutar += (decimal)OrtakFonksiyonlar.YuvarlaYukari(OrtakFonksiyonlar.ConvertToDbl(detay.tutar[0]), 2);

            string pBirimi = TasinirGenel.ParaBirimiDondur();

            string aciklama = string.Empty;
            if (!(kriter.tarih1.isNull && kriter.tarih2.isNull))
                aciklama = string.Format(Resources.TasinirMal.FRMTMC003, kriter.tarih1.ToString(), kriter.tarih2.ToString(), toplamTutar.ToString("#,###.00"), pBirimi);
            else
                aciklama = string.Format(Resources.TasinirMal.FRMTMC004, toplamTutar.ToString("#,###.00"), pBirimi);

            if (!chkDetay.Checked)
            {
                XLS.HucreDegerYaz(satir - 1, sutun, Resources.TasinirMal.FRMTMC005);
                XLS.HucreDegerYaz(satir - 1, sutun + 2, Resources.TasinirMal.FRMTMC006);
                XLS.HucreDegerYaz(satir - 1, sutun + 6, Resources.TasinirMal.FRMTMC007);

                XLS.HucreBirlestir(satir - 1, sutun, satir - 1, sutun + 1);
                XLS.HucreBirlestirme(satir - 1, sutun + 2, satir - 1, sutun + 4);
                XLS.HucreBirlestir(satir - 1, sutun + 2, satir - 1, sutun + 5);
                XLS.HucreBirlestir(satir - 1, sutun + 6, satir - 1, sutun + 7);
            }

            ImzaEkle(XLS, aciklama);

            decimal toplam2Duzey = 0;
            string eskiHesap = string.Empty;
            string eskiHesapAd = string.Empty;
            int siraNo = 0;
            for (int i = 0; i < tk.detay.Count; i++)
            {
                TNS.TMM.TuketimCikisDetay detay = (TNS.TMM.TuketimCikisDetay)tk.detay[i];

                if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod)
                {
                    if (chkDetay.Checked)
                        TuketimCikisToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam2Duzey);
                    else
                        TuketimCikisToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam2Duzey);

                    toplam2Duzey = 0;
                }

                if (chkDetay.Checked)
                {
                    siraNo++;
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, detay.olcuBirimAd);

                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.miktar.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[0].ToString("#,###.00"), (double)0));
                }

                toplam2Duzey += (decimal)OrtakFonksiyonlar.YuvarlaYukari(OrtakFonksiyonlar.ConvertToDbl(detay.tutar[0]), 2);
                eskiHesap = detay.hesapPlanKod;
                eskiHesapAd = detay.hesapPlanAd;
            }

            if (chkDetay.Checked)
                TuketimCikisToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam2Duzey);
            else
                TuketimCikisToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam2Duzey);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Parametre olarak verilen tüketim çıkış kriterlerini sunucudaki TuketimCikis
        /// yordamına gönderir, sunucudan gelen bilgi kümesini excel raporuna aktarır.
        /// </summary>
        /// <param name="kriter">Tüketim çıkış kriter bilgilerini tutan nesne</param>
        private void TuketimCikisFarkYazdir()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim());

            TNS.TMM.TuketimCikis kriter = new TNS.TMM.TuketimCikis();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.ambarKod = txtAmbar.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.tarih1 = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.tarih2 = new TNSDateTime(txtBitisTarih.RawText);

            ObjectArray bilgi = servisTMM.TuketimCikis(kullanan, kriter, true);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "KurusFarkRaporuTuketim.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.TuketimCikis tk = (TNS.TMM.TuketimCikis)bilgi.objeler[0];
            XLS.HucreAdBulYaz("IlAd", tk.ilAd + "-" + tk.ilceAd);
            XLS.HucreAdBulYaz("IlKod", tk.ilKod + "-" + tk.ilceKod);
            XLS.HucreAdBulYaz("HarcamaAd", tk.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", tk.harcamaKod);
            XLS.HucreAdBulYaz("AmbarAd", tk.ambarAd);
            XLS.HucreAdBulYaz("AmbarKod", tk.ambarKod);
            XLS.HucreAdBulYaz("MuhasebeAd", tk.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", tk.muhasebeKod);

            string aciklama = string.Empty;
            if (!(kriter.tarih1.isNull && kriter.tarih2.isNull))
                aciklama = string.Format(Resources.TasinirMal.FRMTMC008, kriter.tarih1.ToString(), kriter.tarih2.ToString());
            else
                aciklama = Resources.TasinirMal.FRMTMC009;

            ImzaEkle(XLS, aciklama);

            if (!chkDetay.Checked)
            {
                XLS.HucreDegerYaz(satir - 1, sutun, Resources.TasinirMal.FRMTMC005);
                XLS.HucreBirlestir(satir - 1, sutun, satir - 1, sutun + 1);
                XLS.SutunGizle(sutun + 5, sutun + 6, true);
            }

            decimal[] toplam = new decimal[4];
            string eskiHesap = string.Empty;
            string eskiHesapAd = string.Empty;
            int siraNo = 0;
            for (int i = 0; i < tk.detay.Count; i++)
            {
                TNS.TMM.TuketimCikisDetay detay = (TNS.TMM.TuketimCikisDetay)tk.detay[i];

                if (!string.IsNullOrEmpty(eskiHesap) && eskiHesap != detay.hesapPlanKod)
                {
                    if (chkDetay.Checked)
                        FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam);
                    else
                        FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam);

                    for (int k = 0; k < toplam.Length; k++)
                        toplam[k] = 0;
                }

                if (chkDetay.Checked)
                {
                    siraNo++;
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, detay.hesapPlanKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, detay.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, detay.olcuBirimAd);

                    XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(detay.miktar.ToString(), (double)0));

                    decimal fark = 0;
                    for (int k = 0; k < detay.tutar.Length; k++)
                    {
                        fark += OrtakFonksiyonlar.ConvertToDecimal(detay.tutar[k].ToString("#,###.00"), (decimal)0) - detay.tutar[k];
                        XLS.HucreDegerYaz(satir, sutun + 7 + k, OrtakFonksiyonlar.ConvertToDouble(detay.tutar[k].ToString("#,###.00"), (double)0));
                    }
                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(fark.ToString(), (double)0));
                }

                for (int k = 0; k < toplam.Length; k++)
                    toplam[k] += detay.tutar[k];

                eskiHesap = detay.hesapPlanKod;
                eskiHesapAd = detay.hesapPlanAd;
            }

            if (chkDetay.Checked)
                FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, " ", toplam);
            else
                FarkToplamYaz(XLS, kaynakSatir, ref satir, sutun, eskiHesap, eskiHesapAd, toplam);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

        /// <summary>
        /// Tüketim malzemeleri fark excel raporuna toplam tutar bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun başladığı kaynak satır numarası</param>
        /// <param name="satir">Toplam bilgilerinin yazılmaya başlanacağı satır numarası</param>
        /// <param name="sutun">Toplam bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        /// <param name="hesapKod">Toplam bilgilerinin ait olduğu taşınır hesap planı kodu</param>
        /// <param name="hesapAd">Toplam bilgilerinin ait olduğu taşınır hesap planı adı</param>
        /// <param name="toplam">Tüketim malzemeleri farklarına ait toplam bilgilerini tutan dizi</param>
        private void FarkToplamYaz(Tablo XLS, int kaynakSatir, ref int satir, int sutun, string hesapKod, string hesapAd, decimal[] toplam)
        {
            decimal fark = 0;
            for (int i = 0; i < toplam.Length; i++)
                fark += OrtakFonksiyonlar.ConvertToDecimal(toplam[i].ToString("#,###.00"), (decimal)0) - toplam[i];
            decimal fark2Duzey = OrtakFonksiyonlar.ConvertToDecimal(fark.ToString("#,###.00"), (decimal)0);

            if (((fark2Duzey >= (decimal)0.01 || fark2Duzey <= (decimal)-0.01) && !chkDetay.Checked) || chkDetay.Checked)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);

                if (chkDetay.Checked)
                {
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 6);
                    XLS.DuseyHizala(satir, sutun, 1);
                    XLS.KoyuYap(satir, sutun, true);
                    XLS.HucreDegerYaz(satir, sutun, hesapKod);
                    XLS.KoyuYap(satir, sutun + 7, satir, sutun + 11, true);

                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(fark.ToString(), (double)0));
                }
                else
                {
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 4);
                    XLS.HucreDegerYaz(satir, sutun, hesapKod);
                    XLS.HucreDegerYaz(satir, sutun + 2, hesapAd);

                    XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(fark2Duzey.ToString(), (double)0));
                }

                for (int i = 0; i < toplam.Length; i++)
                    XLS.HucreDegerYaz(satir, sutun + 7 + i, OrtakFonksiyonlar.ConvertToDouble(toplam[i].ToString("#,###.00"), (double)0));
            }
        }

        /// <summary>
        /// Tüketim malzemeleri çıkış excel raporuna toplam tutar bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="kaynakSatir">Raporun başladığı kaynak satır numarası</param>
        /// <param name="satir">Toplam bilgilerinin yazılacağı satır numarası</param>
        /// <param name="sutun">Toplam bilgilerinin yazılmaya başlanacağı sütun numarası</param>
        /// <param name="hesapKod">Toplam bilgilerinin ait olduğu taşınır hesap planı kodu</param>
        /// <param name="hesapAd">Toplam bilgilerinin ait olduğu taşınır hesap planı adı</param>
        /// <param name="toplam">Taşınır hesap planına ait toplam tutarı</param>
        private void TuketimCikisToplamYaz(Tablo XLS, int kaynakSatir, ref int satir, int sutun, string hesapKod, string hesapAd, decimal toplam)
        {
            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);

            if (chkDetay.Checked)
            {
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 6);
                XLS.DuseyHizala(satir, sutun, 1);
                XLS.KoyuYap(satir, sutun, true);
                XLS.HucreDegerYaz(satir, sutun, hesapKod);
                XLS.KoyuYap(satir, sutun + 7, satir, sutun + 7, true);
                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(toplam.ToString("#,###.00"), (double)0));
            }
            else
            {
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 5);
                XLS.HucreBirlestir(satir, sutun + 6, satir, sutun + 7);
                XLS.HucreDegerYaz(satir, sutun, hesapKod);
                XLS.HucreDegerYaz(satir, sutun + 2, hesapAd);
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(toplam.ToString("#,###.00"), (double)0));
            }
        }

        /// <summary>
        /// Tüketim malzemeleri çıkış ve fark excel raporlarına imza bilgilerini ekleyen yordam
        /// </summary>
        /// <param name="XLS">Excel ile ilgili işlemleri yapan nesne</param>
        /// <param name="aciklama">Rapora eklenecek açıklama bilgisi</param>
        private void ImzaEkle(Tablo XLS, string aciklama)
        {
            ObjectArray imza = servisTMM.ImzaListele(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), txtAmbar.Text.Trim(), 0);

            string[] ad = new string[3];
            string[] unvan = new string[3];

            if (imza.sonuc.islemSonuc && imza.objeler.Count > 0)
            {
                foreach (ImzaBilgisi iBilgi in imza.objeler)
                {
                    if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKAYITYETKILISI && string.IsNullOrEmpty(ad[0]))
                    {
                        ad[0] = iBilgi.adSoyad;
                        unvan[0] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.HARCAMAYETKILISI && string.IsNullOrEmpty(ad[1]))
                    {
                        ad[1] = iBilgi.adSoyad;
                        unvan[1] = iBilgi.unvan;
                    }
                    else if (iBilgi.imzaYer == (int)ENUMImzaYer.TASINIRKONTROLYETKILISI && string.IsNullOrEmpty(ad[2]))
                    {
                        ad[2] = iBilgi.adSoyad;
                        unvan[2] = iBilgi.unvan;
                    }
                }
            }

            if (!string.IsNullOrEmpty(aciklama))
                XLS.HucreAdBulYaz("Aciklama", aciklama);

            if (!string.IsNullOrEmpty(ad[0]))
                XLS.HucreAdBulYaz("AdSoyad1", ad[0]);

            if (!string.IsNullOrEmpty(unvan[0]))
                XLS.HucreAdBulYaz("Unvan1", unvan[0]);

            XLS.HucreAdBulYaz("Tarih1", DateTime.Today.Date.ToShortDateString());

            if (!string.IsNullOrEmpty(ad[1]))
                XLS.HucreAdBulYaz("AdSoyad2", ad[1]);

            if (!string.IsNullOrEmpty(unvan[1]))
                XLS.HucreAdBulYaz("Unvan2", unvan[1]);

            XLS.HucreAdBulYaz("Tarih2", DateTime.Today.Date.ToShortDateString());

            if (!string.IsNullOrEmpty(ad[2]))
                XLS.HucreAdBulYaz("AdSoyad3", ad[2]);

            if (!string.IsNullOrEmpty(unvan[2]))
                XLS.HucreAdBulYaz("Unvan3", unvan[2]);

            XLS.HucreAdBulYaz("Tarih3", DateTime.Today.Date.ToShortDateString());
        }

        /// <summary>
        /// Parametre olarak verilen zimmet listeleme kriterlerini sunucudaki ZimmetOrtakAlan yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Zimmet kriter bilgilerini tutan nesne</param>
        private void ZimmetOrtakAlanYazdir()
        {
            TasinirGenel.DegiskenleriKaydet(kullanan, txtMuhasebe.Text.Trim(), txtHarcamaBirimi.Text.Trim(), "");

            TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new TNS.TMM.ZimmetOrtakAlanVeKisi();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.odaKod = txtNereyeVerildi.Text.Trim();
            if (chkKisiDahilEt.Checked)
                kriter.kisiDahilEt = 1;

            ObjectArray bilgi = servisTMM.ZimmetOrtakAlan(kullanan, kriter);

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

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZimmetOrtakAlan.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            TNS.TMM.ZimmetOrtakAlanVeKisi zoa = (TNS.TMM.ZimmetOrtakAlanVeKisi)bilgi.objeler[0];
            XLS.HucreAdBulYaz("HarcamaAd", zoa.harcamaAd);
            XLS.HucreAdBulYaz("HarcamaKod", zoa.harcamaKod);
            XLS.HucreAdBulYaz("MuhasebeAd", zoa.muhasebeAd);
            XLS.HucreAdBulYaz("MuhasebeKod", zoa.muhasebeKod);
            XLS.HucreAdBulYaz("OdaAd", zoa.odaAd);
            XLS.HucreAdBulYaz("OdaKod", zoa.odaKod);
            //XLS.HucreAdBulYaz("OdaAd", lblNereyeVerildi.Text.Trim());     //İstemci tarafından doldurulan label değeri server'a ulaşamadı için kapatıldı. HÖ
            //XLS.HucreAdBulYaz("OdaKod", txtNereyeVerildi.Text.Trim());

            for (int i = 0; i < zoa.detay.Count; i++)
            {
                TNS.TMM.ZimmetOrtakAlanVeKisiDetay detay = (TNS.TMM.ZimmetOrtakAlanVeKisiDetay)zoa.detay[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 8);

                XLS.HucreDegerYaz(satir, sutun, detay.harcamaKod + " - " + detay.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 1, detay.ambarKod + " - " + detay.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 2, detay.gorSicilNo);
                XLS.HucreDegerYaz(satir, sutun + 3, detay.sicilAd);
                XLS.HucreDegerYaz(satir, sutun + 5, detay.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 6, detay.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 7, detay.sorumluKisiAd);
                XLS.HucreDegerYaz(satir, sutun + 9, detay.aciklama);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }


        /// <summary>
        /// Dönem bilgileri ddlDonem DropDownList kontrolüne doldurur.
        /// </summary>
        private void DonemDoldur()
        {
            List<object> liste = new List<object>();

            for (int i = 1; i <= 4; i++)
                liste.Add(new { KOD = i.ToString(), ADI = string.Format(Resources.TasinirMal.FRMECK002, i.ToString()) });

            strDonem.DataSource = liste;
            strDonem.DataBind();
        }

        private void ZimmetListesiRaporu()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            //shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            shBilgi.muhasebeKod = txtMuhasebe.Text.Trim();
            shBilgi.muhasebeAd = lblMuhasebeAd.Text.Trim();
            shBilgi.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            shBilgi.harcamaBirimAd = lblHarcamaBirimiAd.Text.Trim();
            shBilgi.ambarKod = txtAmbar.Text.Trim();
            shBilgi.ambarAd = lblAmbarAd.Text.Trim();
            shBilgi.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            shBilgi.hesapPlanAd = lblHesapPlanAd.Text.Trim();
            shBilgi.durum = 2;//ZİMMETTEKİLER
            shBilgi.kimeGitti = txtKimeVerildi.Text.Trim();

            string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");

            ObjectArray bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, new Sayfalama());
            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            if (bilgi.objeler.Count == 0)
            {
                GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "ZimmetListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (SicilNoHareket sh in bilgi.objeler)
            {
                if (sh.kisiAd != "")//  && sh.durum == 1kisi adı boş olmayan ve durumu zimmette olanlar
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, sh.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, sh.hesapPlanKod.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 2, sh.hesapPlanAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, sh.kimeGitti);
                    XLS.HucreDegerYaz(satir, sutun + 4, sh.odaAd);
                    XLS.HucreDegerYaz(satir, sutun + 5, sh.zimmetOzellik);
                    XLS.HucreDegerYaz(satir, sutun + 6, sh.kdvOran);
                    XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(sh.fiyat.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(sh.kdvliBirimFiyat.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 9, sh.muhasebeAd);
                    XLS.HucreDegerYaz(satir, sutun + 10, sh.harcamaBirimAd);
                    XLS.HucreDegerYaz(satir, sutun + 11, sh.ambarAd);
                    XLS.HucreDegerYaz(satir, sutun + 12, sh.ozellik.markaAd);
                    XLS.HucreDegerYaz(satir, sutun + 13, sh.ozellik.modelAd);
                    XLS.HucreDegerYaz(satir, sutun + 14, sh.ozellik.saseNo);
                    XLS.HucreDegerYaz(satir, sutun + 15, sh.ozellik.motorNo);
                    XLS.HucreDegerYaz(satir, sutun + 16, sh.ozellik.plaka);
                    XLS.HucreDegerYaz(satir, sutun + 17, sh.ozellik.eskiBisNo1);

                }

            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void AmbardakiTasinirListesiRaporu()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            //shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            shBilgi.muhasebeKod = txtMuhasebe.Text.Trim();
            shBilgi.muhasebeAd = lblMuhasebeAd.Text.Trim();
            shBilgi.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            shBilgi.harcamaBirimAd = lblHarcamaBirimiAd.Text.Trim();
            shBilgi.ambarKod = txtAmbar.Text.Trim();
            shBilgi.ambarAd = lblAmbarAd.Text.Trim();
            shBilgi.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            shBilgi.hesapPlanAd = lblHesapPlanAd.Text.Trim();
            shBilgi.durum = 1;//AMBARDAKİLER

            //XLS hazırlığı
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "AmbardakiTasinirListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            //****************************************************************************

            string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");
            ObjectArray bilgi = new ObjectArray();
            Sayfalama sayfa = new Sayfalama();
            sayfa.sayfaNo = 1;
            sayfa.kayitSayisi = 20000;
            while (true)
            {
                bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, sayfa);
                if (bilgi.objeler.Count == 0) break;

                foreach (SicilNoHareket sh in bilgi.objeler)
                {
                    if (sh.kisiAd == "")//kisi adı boş olan ve durumu ambarda olanlar
                    {
                        satir++;

                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                        XLS.HucreDegerYaz(satir, sutun, sh.sicilNo);
                        XLS.HucreDegerYaz(satir, sutun + 1, sh.hesapPlanKod.ToString());
                        XLS.HucreDegerYaz(satir, sutun + 2, sh.hesapPlanAd);
                        XLS.HucreDegerYaz(satir, sutun + 3, sh.kdvOran);
                        if (sh.amortismanYil > 0)
                            XLS.HucreDegerYaz(satir, sutun + 4, 100 / sh.amortismanYil);
                        XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(sh.fiyat.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(sh.kdvliBirimFiyat.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 7, sh.muhasebeAd);
                        XLS.HucreDegerYaz(satir, sutun + 8, sh.harcamaBirimAd);
                        XLS.HucreDegerYaz(satir, sutun + 9, sh.ambarAd);
                        XLS.HucreDegerYaz(satir, sutun + 10, sh.ozellik.markaAd);
                        XLS.HucreDegerYaz(satir, sutun + 11, sh.ozellik.modelAd);
                        XLS.HucreDegerYaz(satir, sutun + 12, sh.ozellik.saseNo);
                        XLS.HucreDegerYaz(satir, sutun + 13, sh.ozellik.motorNo);
                        XLS.HucreDegerYaz(satir, sutun + 14, sh.ozellik.plaka);
                        XLS.HucreDegerYaz(satir, sutun + 15, sh.ozellik.eskiBisNo1);
                    }
                }

                sayfa.sayfaNo++;
            }

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            //if (bilgi.objeler.Count == 0)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
            //    return;
            //}

            //XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void GayrimenkulListesiRaporu()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            //shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            shBilgi.muhasebeKod = txtMuhasebe.Text.Trim();
            shBilgi.muhasebeAd = lblMuhasebeAd.Text.Trim();
            shBilgi.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            shBilgi.harcamaBirimAd = lblHarcamaBirimiAd.Text.Trim();
            shBilgi.hesapPlanKod = TasinirGenel.ComboDegerDondur(ddlGayrimenkulTuru);

            if (shBilgi.hesapPlanKod == "")
            {
                string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULHESAPKOD") + "";
                shBilgi.hesapPlanKod = "@" + hesapKodGM;
            }

            //XLS hazırlığı
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "GayrimenkulListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("harcamaBirimi", GenelIslemler.KodAd(32, txtMuhasebe.Text.Trim() + "-" + txtHarcamaBirimi.Text.Trim(), true));
            satir = kaynakSatir;
            //****************************************************************************

            string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");
            ObjectArray bilgi = new ObjectArray();
            Sayfalama sayfa = new Sayfalama();
            sayfa.sayfaNo = 1;
            sayfa.kayitSayisi = 20000;
            sayfa.siralamaAlani = "HESAPPLANKOD";
            while (true)
            {
                bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, sayfa);
                if (bilgi.objeler.Count == 0) break;

                foreach (SicilNoHareket sh in bilgi.objeler)
                {
                    //if (sh.kisiAd == "")//kisi adı boş olan ve durumu ambarda olanlar
                    //{
                    satir++;

                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 15, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, sh.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, sh.hesapPlanKod.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 2, sh.hesapPlanAd);
                    //XLS.HucreDegerYaz(satir, sutun + 3, sh.ozellik.ciltTuru);
                    XLS.HucreDegerYaz(satir, sutun + 3, sh.ozellik.giai);
                    XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(sh.fiyat.ToString(), (double)0));
                    if (sh.ozellik.amortismanYil > 0)
                        XLS.HucreDegerYaz(satir, sutun + 5, 100 / sh.ozellik.amortismanYil);
                    XLS.HucreDegerYaz(satir, sutun + 6, sh.ozellik.bulunduguYerAd);// + "-" + sh.ozellik.bulunduguYer);
                    XLS.HucreDegerYaz(satir, sutun + 7, sh.ozellik.neredeBulundu);
                    XLS.HucreDegerYaz(satir, sutun + 8, sh.ozellik.sayfaSayisi);
                    XLS.HucreDegerYaz(satir, sutun + 9, sh.ozellik.satirSayisi);
                    XLS.HucreDegerYaz(satir, sutun + 10, sh.ozellik.yaprakSayisi);
                    XLS.HucreDegerYaz(satir, sutun + 11, sh.ozellik.boyutlari);
                    XLS.HucreDegerYaz(satir, sutun + 12, sh.ozellik.yayinYeri);
                    XLS.HucreDegerYaz(satir, sutun + 13, new TNSDateTime(sh.ozellik.yayinTarihi).ToString());
                    XLS.HucreDegerYaz(satir, sutun + 14, sh.ozellik.neredenGeldi);
                    XLS.HucreDegerYaz(satir, sutun + 15, sh.ozellik.gelisTarihi);
                    //}
                }

                sayfa.sayfaNo++;
            }

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            //if (bilgi.objeler.Count == 0)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
            //    return;
            //}

            //XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        private void YazilimListesiRaporu()
        {
            SicilNoHareket shBilgi = new SicilNoHareket();

            //shBilgi.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            shBilgi.muhasebeKod = txtMuhasebe.Text.Trim();
            shBilgi.muhasebeAd = lblMuhasebeAd.Text.Trim();
            shBilgi.harcamaBirimKod = txtHarcamaBirimi.Text.Trim();
            shBilgi.harcamaBirimAd = lblHarcamaBirimiAd.Text.Trim();
            shBilgi.hesapPlanKod = TasinirGenel.ComboDegerDondur(ddlGayrimenkulTuru);

            if (shBilgi.hesapPlanKod == "")
            {
                string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMHESAPKOD") + "";
                shBilgi.hesapPlanKod = "@" + hesapKodGM;
            }

            //XLS hazırlığı
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "YazilimListesi.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;
            //****************************************************************************

            string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");
            ObjectArray bilgi = new ObjectArray();
            Sayfalama sayfa = new Sayfalama();
            sayfa.sayfaNo = 1;
            sayfa.kayitSayisi = 20000;
            while (true)
            {
                bilgi = servisTMM.BarkodSicilNoListele(kullanan, shBilgi, sayfa);
                if (bilgi.objeler.Count == 0) break;

                foreach (SicilNoHareket sh in bilgi.objeler)
                {
                    //if (sh.kisiAd == "")//kisi adı boş olan ve durumu ambarda olanlar
                    //{
                    satir++;

                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, sh.sicilNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, sh.hesapPlanKod.ToString());
                    XLS.HucreDegerYaz(satir, sutun + 2, sh.hesapPlanAd);
                    //XLS.HucreDegerYaz(satir, sutun + 3, sh.ozellik.ciltTuru);
                    XLS.HucreDegerYaz(satir, sutun + 3, sh.ozellik.giai);
                    XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(sh.fiyat.ToString(), (double)0));
                    XLS.HucreDegerYaz(satir, sutun + 5, sh.ozellik.yayinYeri);
                    XLS.HucreDegerYaz(satir, sutun + 6, sh.ozellik.yayinTarihi);
                    XLS.HucreDegerYaz(satir, sutun + 7, sh.ozellik.neredenGeldi);
                    XLS.HucreDegerYaz(satir, sutun + 8, sh.ozellik.gelisTarihi);
                    //}
                }

                sayfa.sayfaNo++;
            }

            if (!bilgi.sonuc.islemSonuc)
            {
                GenelIslemler.MesajKutusu("Uyarı", bilgi.sonuc.hataStr);
                return;
            }

            //if (bilgi.objeler.Count == 0)
            //{
            //    GenelIslemler.MesajKutusu("Bilgi", Resources.TasinirMal.FRMBRK002);
            //    return;
            //}

            //XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        public void GMTurDoldur()
        {
            List<object> liste = new List<object>();
            string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRGAYRIMENKULHESAPKOD") + "";

            ObjectArray dler = new ObjectArray();
            if (!string.IsNullOrEmpty(hesapKodGM))
            {
                HesapPlaniSatir kriter = new HesapPlaniSatir();
                kriter.hesapKodAciklama = "@" + hesapKodGM;
                kriter.detay = true;
                dler = servisTMM.HesapPlaniListele(kullanan, kriter, new Sayfalama());
            }

            string hepsiKod = "@" + hesapKodGM;
            liste.Add(new
            {
                KOD = hepsiKod,
                AD = "Hepsi",
            });

            foreach (HesapPlaniSatir d in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = d.hesapKod,
                    AD = d.aciklama,
                });
            }
            strGMTur.DataSource = liste;
            strGMTur.DataBind();

            ddlGayrimenkulTuru.SetValueAndFireSelect(hepsiKod);
        }

        public void YZTurDoldur()
        {
            List<object> liste = new List<object>();
            string hesapKodGM = TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRYAZILIMHESAPKOD") + "";

            ObjectArray dler = new ObjectArray();
            if (!string.IsNullOrEmpty(hesapKodGM))
            {
                HesapPlaniSatir kriter = new HesapPlaniSatir();
                kriter.hesapKodAciklama = "@" + hesapKodGM;
                kriter.detay = true;
                dler = servisTMM.HesapPlaniListele(kullanan, kriter, new Sayfalama());
            }

            string hepsiKod = "@" + hesapKodGM;
            liste.Add(new
            {
                KOD = hepsiKod,
                AD = "Hepsi",
            });

            foreach (HesapPlaniSatir d in dler.objeler)
            {
                liste.Add(new
                {
                    KOD = d.hesapKod,
                    AD = d.aciklama,
                });
            }
            strGMTur.DataSource = liste;
            strGMTur.DataBind();

            ddlGayrimenkulTuru.SetValueAndFireSelect(hepsiKod);
        }

        public string EkranTurDondur()
        {
            string tur = Request.QueryString["gm"] + "";
            if (tur == "1") return "GM";
            else if (tur == "2") return "YZ";
            else return "";
        }

        public static string BAOnayiAmortismanRaporu(Kullanici kullanan, ITMMServis servisTMM, AmortismanKriter ak, string fisNo, string ciktiTur, bool raporlarEkranimi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BAOnayOncesiAmortisman.xlt";
            string raporSablonYol = System.Configuration.ConfigurationManager.AppSettings.Get("RaporSablonYol");

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            string aciklamaA = "", aciklama2A = "";
            string aciklamaB = "", aciklama2B = "";
            decimal adetA = 0, adet2A = 0;
            decimal adetB = 0, adet2B = 0;
            decimal ilkBedel = 0, ilkBedel2 = 0;
            decimal bedelDuzeltmeFarki = 0, bedelDuzeltmeFarki2 = 0;
            decimal sonBedelA = 0, sonBedel2A = 0;
            decimal sonBedelB = 0, sonBedel2B = 0;
            decimal birikenAmortisman = 0, birikenAmortisman2 = 0;
            decimal amortismanDuzeltmeFarki = 0, amortismanDuzeltmeFarki2 = 0;
            decimal duzeltilmisBirikenAmortisman = 0, duzeltilmisBirikenAmortisman2 = 0;






            //Amortisman Form Oku


            AmortismanIslemForm fromAmortisman = new AmortismanIslemForm()
            {
                yil = ak.yil,
                muhasebeKod = ak.muhasebeKod,
                harcamaKod = ak.harcamaKod,
                ambarKod = ak.ambarKod,
                fisNo = fisNo
            };

            ObjectArray formlar = servisTMM.AmortismanIslemFisiListele(kullanan, fromAmortisman);

            List<object> liste = new List<object>();

            if (formlar.sonuc.islemSonuc)
            {
                foreach (AmortismanIslemForm aif in formlar.objeler)
                {
                    if (aif.donem == ak.donem)
                    {
                        XLS.HucreAdBulYaz("FisNo", aif.fisNo);
                        XLS.HucreAdBulYaz("FisTarih", aif.fisTarih.ToString());
                        XLS.HucreAdBulYaz("HarcamaAd", aif.harcamaAd);
                        XLS.HucreAdBulYaz("MuhasebeAd", aif.muhasebeAd);
                        XLS.HucreAdBulYaz("FisAciklamasi", aif.fisAciklama);
                        break;
                    }
                }
            }


            //***************************************************


            AmortismanRaporBA raporBA = servisTMM.AmortismanRaporlaBA(kullanan, ak);

            adetA = raporBA.adetA;
            ilkBedel = raporBA.ilkBedel;
            bedelDuzeltmeFarki = raporBA.bedelDuzeltmeFarki;
            sonBedelA = raporBA.sonBedelA;
            birikenAmortisman = raporBA.birikenAmortisman;
            amortismanDuzeltmeFarki = raporBA.amortismanDuzeltmeFarki;
            duzeltilmisBirikenAmortisman = raporBA.duzeltilmisBirikenAmortisman;

            sonBedelB = raporBA.sonBedelB;
            adet2A = raporBA.adet2A;
            ilkBedel2 = raporBA.ilkBedel2;
            bedelDuzeltmeFarki2 = raporBA.bedelDuzeltmeFarki2;
            sonBedel2A = raporBA.sonBedel2A;
            birikenAmortisman2 = raporBA.birikenAmortisman2;
            amortismanDuzeltmeFarki2 = raporBA.amortismanDuzeltmeFarki2;
            duzeltilmisBirikenAmortisman2 = raporBA.duzeltilmisBirikenAmortisman2;
            sonBedel2B = raporBA.sonBedel2B;

            //ObjectArray aListe = servisTMM.AmortismanRaporla3(kullanan, ak);

            //foreach (TNS.TMM.AmortismanRapor3 ar in aListe.objeler)
            //{
            //    if (ar.maliyetTutar == 0 || ar.amortismanSuresi == 0 || ar.toplamCariAmortisman == 0)
            //        continue;

            //    if (ar.toplamKalanAmortisman == 0)
            //    {
            //        adetA++;
            //        ilkBedel += ar.maliyetTutar;
            //        bedelDuzeltmeFarki += ar.degerDuzeltmeToplamTutar;
            //        sonBedelA += ar.toplamTutar;

            //        birikenAmortisman += ar.maliyetBirikmisAmortisman;
            //        amortismanDuzeltmeFarki += ar.degerDuzeltmeBirikmisAmortisman;
            //        duzeltilmisBirikenAmortisman += ar.toplamBirikmisAmortisman;

            //        sonBedelB += ar.sonDonemAmortisman;
            //    }
            //    else
            //    {
            //        adet2A++;
            //        ilkBedel2 += ar.maliyetTutar;
            //        bedelDuzeltmeFarki2 += ar.degerDuzeltmeToplamTutar;
            //        sonBedel2A += ar.toplamTutar;

            //        birikenAmortisman2 += ar.maliyetBirikmisAmortisman;
            //        amortismanDuzeltmeFarki2 += ar.degerDuzeltmeBirikmisAmortisman;
            //        duzeltilmisBirikenAmortisman2 += ar.toplamBirikmisAmortisman;

            //        sonBedel2B += ar.sonDonemAmortisman;
            //    }
            //}


            string baslik2 = "DEMİRBAŞ";
            if (ak.ambarKod == "50")
                baslik2 = "GAYRİMENKUL";
            else if (ak.ambarKod == "51")
                baslik2 = "YAZILIM";



            XLS.HucreAdBulYaz("Baslik", string.Format("{0} YILI {1}. AY AMORTİSMAN RAPORU", ak.yil, ak.donem));
            XLS.HucreAdBulYaz("Baslik2", baslik2);

            XLS.HucreAdBulYaz("AciklamaA", string.Format("TAMAMI AMORTİ EDİLMİŞ {0} ADET", adetA));
            XLS.HucreAdBulYaz("IlkBedel", (double)ilkBedel);
            XLS.HucreAdBulYaz("BedelDuzeltmeFarki", (double)bedelDuzeltmeFarki);
            XLS.HucreAdBulYaz("SonBedelA", (double)sonBedelA);
            XLS.HucreAdBulYaz("SonBedelB", (double)sonBedelB);
            XLS.HucreAdBulYaz("BirikenAmortisman", (double)birikenAmortisman);
            XLS.HucreAdBulYaz("AmortismanDuzeltmeFarki", (double)amortismanDuzeltmeFarki);
            XLS.HucreAdBulYaz("DuzelmisBirikenAmortisman", (double)duzeltilmisBirikenAmortisman);

            XLS.HucreAdBulYaz("Aciklama2A", string.Format("TAMAMI AMORTİ EDİLMEMİŞ {0} ADET", adet2A));
            XLS.HucreAdBulYaz("IlkBedel2", (double)ilkBedel2);
            XLS.HucreAdBulYaz("BedelDuzeltmeFarki2", (double)bedelDuzeltmeFarki2);
            XLS.HucreAdBulYaz("SonBedel2A", (double)sonBedel2A);
            XLS.HucreAdBulYaz("SonBedel2B", (double)sonBedel2B);
            XLS.HucreAdBulYaz("BirikenAmortisman2", (double)birikenAmortisman2);
            XLS.HucreAdBulYaz("AmortismanDuzeltmeFarki2", (double)amortismanDuzeltmeFarki2);
            XLS.HucreAdBulYaz("DuzelmisBirikenAmortisman2", (double)duzeltilmisBirikenAmortisman2);

            //Tarihçe bilgisi taşınır işlem fişi, değer atrışı ve amortisman işlemlerinin B/A onay bilgilerini okumak için eklenmiştir.
            MuhasebeIslemiKriter formBA = new MuhasebeIslemiKriter
            {
                yil = ak.yil,
                muhasebeKod = ak.muhasebeKod,
                harcamaKod = ak.harcamaKod,
                fisNo = fisNo
            };

            ObjectArray listeTarihce = servisTMM.BAOnayiTarihceListele(formBA);
            string islemKontrol = "";
            foreach (TarihceBilgisi t in listeTarihce.objeler)
            {
                if (t.islem == "Değiştirildi")
                    continue;

                if (t.islem == "B Onayına Gönderildi")
                {
                    formBA.girisZaman = t.islemTarih;
                    formBA.girisSicil = t.islemYapan + " -" + t.islemYapanAd;
                    islemKontrol = "B Onayına Gönderildi";
                }
                else if (t.islem == "A Onayına Gönderildi" && islemKontrol == "B Onayına Gönderildi")
                {
                    formBA.bOnayZaman = t.islemTarih;
                    formBA.bOnaySicil = t.islemYapan + " -" + t.islemYapanAd;
                    islemKontrol = "A Onayına Gönderildi";
                }
                else if (t.islem == "Onaylandı" && (islemKontrol == "A Onayına Gönderildi" || islemKontrol == "Onaylandı"))
                {
                    formBA.aOnayZaman = t.islemTarih;
                    formBA.aOnaySicil = t.islemYapan + " -" + t.islemYapanAd;
                    islemKontrol = "Onaylandı";
                }
                else
                {
                    formBA.bOnayZaman = new TNSDateTime();
                    formBA.bOnaySicil = "";
                    formBA.aOnayZaman = new TNSDateTime();
                    formBA.aOnaySicil = "";
                }
            }
            //***************************************************

            XLS.HucreAdBulYaz("FisiYonlendiren", formBA.girisSicil);
            XLS.HucreAdBulYaz("BOnayi", formBA.bOnaySicil);
            XLS.HucreAdBulYaz("AOnayi", formBA.aOnaySicil);


            if (ciktiTur == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (ciktiTur == "2" || ciktiTur == "3")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                if (ciktiTur == "2")
                    OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }

            return XLS.SonucDosyaAd();
        }

        /// <summary>
        /// Parametre olarak verilen depo durum kriterlerini sunucudaki taşınır depo durum yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void KHKRaporu()
        {
            DepoDurum kriter = new DepoDurum();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(txtYil.Text, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim().Replace(".", "");
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim().Replace(".", "");
            kriter.ilKod = TasinirGenel.ComboDegerDondur(ddlIl);
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.firma = txtNeredenGeldi.Text.Trim();
            kriter.TcVNo = txtTCVergiNo.Text.Trim();
            kriter.baslamaTarih = new TNSDateTime(txtBaslangicTarih.RawText);
            kriter.bitisTarih = new TNSDateTime(txtBitisTarih.RawText);

            ObjectArray bilgi = servisTMM.KHKRaporu(kullanan, kriter);

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
            string sablonAd = string.Empty;

            sablonAd = "KHKRaporu.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            int siraNo = 0, toplamCount;
            toplamCount = 6;
            decimal[] toplam = new decimal[toplamCount];
            XLS.HucreAdBulYaz("IlAd", kriter.ilAd);
            XLS.HucreAdBulYaz("ilKod", kriter.ilKod);
            XLS.HucreAdBulYaz("muhasebeAd", lblMuhasebeAd.Text.Trim());
            XLS.HucreAdBulYaz("muhasebeKod", kriter.muhasebeKod);
            XLS.HucreAdBulYaz("harcamaAd", lblHarcamaBirimiAd.Text.Trim());
            XLS.HucreAdBulYaz("harcamaKod", kriter.harcamaKod);
            XLS.HucreAdBulYaz("YIL", kriter.yil != 0 ? kriter.yil.ToString() : "");
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                DepoDurum depo = (DepoDurum)bilgi.objeler[i];

                siraNo++;
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, depo.firma);
                XLS.HucreDegerYaz(satir, sutun + 1, depo.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 2, depo.hesapPlanAd);

                XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(depo.cikanMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble(depo.cikanTutar.ToString(), (double)0));

                XLS.HucreDegerYaz(satir, sutun + 7, OrtakFonksiyonlar.ConvertToDouble(depo.kalanMiktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 8, OrtakFonksiyonlar.ConvertToDouble(depo.kalanTutar.ToString(), (double)0));

                toplam[0] += depo.girenMiktar;
                toplam[1] += depo.girenTutar;
                toplam[2] += depo.cikanMiktar;
                toplam[3] += depo.cikanTutar;

                toplam[4] += depo.kalanMiktar;
                toplam[5] += depo.kalanTutar;

            }

            satir++;
            int sutunSayac = 3;

            int count = 0;
            foreach (decimal deger in toplam)
            {
                XLS.HucreDegerYaz(satir, sutun + sutunSayac, OrtakFonksiyonlar.ConvertToDouble(toplam[count].ToString(), (double)0));
                sutunSayac++;
                count++;
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            string gidenAd = "";

            gidenAd = "KHKRaporu" + kriter.harcamaKod;

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void TasinirKodGrupluTasinirHurdaRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.TasinirKodGrupluTasinirHurdaRaporu(kullanan, tf, tfKriter);

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
            string sablonAd = "TasinirHurdaRaporuTasinirKodGruplu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            foreach (TNS.TMM.TasinirIslemForm tif in bilgi.objeler)
            {
                //foreach (TNS.TMM.TasinirIslemDetay detay in tif.detay.objeler)
                //{
                TasinirIslemDetay detay = (TasinirIslemDetay)tif.detay.objeler[0];
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 17, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, tif.muhasebeKod + " - " + tif.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 1, tif.harcamaKod + " - " + tif.harcamaAd);
                XLS.HucreDegerYaz(satir, sutun + 2, tif.ambarKod + " - " + tif.ambarAd);
                XLS.HucreDegerYaz(satir, sutun + 3, tif.fisNo);
                XLS.HucreDegerYaz(satir, sutun + 4, tif.fisTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun + 5, tif.islemTipAd);

                XLS.HucreDegerYaz(satir, sutun + 6, tif.nereyeGitti);

                XLS.HucreDegerYaz(satir, sutun + 7, detay.hesapPlanKod);
                XLS.HucreDegerYaz(satir, sutun + 8, detay.hesapPlanAd);
                XLS.HucreDegerYaz(satir, sutun + 9, detay.olcuBirimAd);

                //decimal miktar = 0;
                //if (tif.islemTipTur <= (int)ENUMIslemTipi.ACILIS)
                //    miktar = detay.miktar;
                //else
                //    miktar = -detay.miktar;

                //decimal miktar = detay.miktar;

                //decimal tutar = miktar * detay.birimFiyatKDVLi;

                //XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyat.ToString(), (double)0));
                //XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(detay.birimFiyatKDVLi.ToString(), (double)0));
                //XLS.HucreDegerYaz(satir, sutun + 12, OrtakFonksiyonlar.ConvertToDouble(miktar.ToString(), (double)0));
                //XLS.HucreDegerYaz(satir, sutun + 13, OrtakFonksiyonlar.ConvertToDouble(tutar.ToString(), (double)0));

                XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(detay.miktar.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(detay.tutar.ToString(), (double)0));

                //if (tif.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tif.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                //{
                //    XLS.HucreDegerYaz(satir, sutun + 13, tif.gMuhasebeKod + " - " + tif.gMuhasebeAd);
                //    XLS.HucreDegerYaz(satir, sutun + 14, tif.gHarcamaKod + " - " + tif.gHarcamaAd);
                //    XLS.HucreDegerYaz(satir, sutun + 15, tif.gAmbarKod + " - " + tif.gAmbarAd);
                //}

                miktarToplam += detay.miktar;
                tutarToplam += detay.tutar;
                //}
            }

            //Toplamlar yazılıyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 9);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTHU002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 10, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 10, true);
            XLS.HucreDegerYaz(satir, sutun + 11, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 11, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void IlBazliTasinirHurdaRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray iller = servisTMM.IlListele(kullanan, new Il());
            ObjectArray bilgi = servisTMM.IlBazliTasinirHurdaRaporu(kullanan, tf, tfKriter);
            ObjectArray hesapKodlari = servisTMM.HesapPlaniListele(kullanan, new HesapPlaniSatir { seviye = 1 }, new Sayfalama());
            ObjectArray toplamlar = new ObjectArray();

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
            int kaynakSutun = 0;
            int formatSatir = 0;
            int formatSutun = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "IlBazliTasinirHurdaRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("FormatOrnek", ref formatSatir, ref formatSutun);
            XLS.HucreAdBulYaz("Baslik", tf.yil + " YILI İLLER İTİBARIYLA KULLANILMAZ HAL VE HURDAYA AYRILAN TAŞINIRLAR LİSTESİ");

            satir = kaynakSatir;
            kaynakSutun = sutun;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            decimal ilMiktarToplam = 0;
            decimal ilTutarToplam = 0;
            int cnt = 0;
            foreach (Il il in iller.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                //XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun, satir, (hesapKodlari.objeler.Count * 2) + 3);
                XLS.HucreDegerYaz(satir, sutun - 1, OrtakFonksiyonlar.ConvertToInt(il.kod, 0));
                XLS.HucreDegerYaz(satir, sutun, il.ad);
                foreach (HesapPlaniSatir hesapPlaniKod in hesapKodlari.objeler)
                {
                    foreach (TNS.TMM.HurdaRaporu tif in bilgi.objeler)
                    {
                        if (tif.ilKod == "-1" && cnt < hesapKodlari.objeler.Count)
                        {
                            toplamlar.objeler.Add(tif);
                            cnt++;
                        }

                        if (il.kod == tif.ilKod)
                        {
                            if (hesapPlaniKod.hesapKod == tif.hesapPlanKod)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(tif.miktar.ToString(), (double)0));
                                XLS.HucreDegerYaz(satir, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(tif.tutar.ToString(), (double)0));
                                //XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
                                ilMiktarToplam += tif.miktar;
                                ilTutarToplam += tif.tutar;
                            }
                        }
                    }
                    sutun = sutun + 2;
                }

                XLS.HucreDegerYaz(satir, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(ilMiktarToplam.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(ilTutarToplam.ToString(), (double)0));

                sutun = kaynakSutun;
                ilMiktarToplam = 0;
                ilTutarToplam = 0;
            }

            int sonSatir = satir;
            satir = kaynakSatir;
            sutun = kaynakSutun;

            XLS.SatirAc(sonSatir + 1, 1);
            XLS.HucreDegerYaz(sonSatir + 1, sutun, "TOPLAM");

            foreach (HesapPlaniSatir hesapPlaniKod in hesapKodlari.objeler)
            {
                foreach (TNS.TMM.HurdaRaporu tif in toplamlar.objeler)
                {
                    if (hesapPlaniKod.hesapKod == tif.hesapPlanKod)
                    {
                        XLS.HucreDegerYaz(sonSatir + 1, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(tif.miktar.ToString(), (double)0));
                        XLS.HucreDegerYaz(sonSatir + 1, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(tif.tutar.ToString(), (double)0));

                        miktarToplam += tif.miktar;
                        tutarToplam += tif.tutar;
                    }
                }
                //XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, (hesapKodlari.objeler.Count*2)+3, satir, sutun);
                XLS.HucreBirlestir(satir - 2, sutun + 1, satir - 2, sutun + 2);

                XLS.HucreDegerYaz(satir - 2, sutun + 1, hesapPlaniKod.hesapKod);
                XLS.HucreDegerYaz(satir - 1, sutun + 1, "MİKTAR");
                XLS.HucreDegerYaz(satir - 1, sutun + 2, "TUTAR");

                sutun = sutun + 2;
            }

            XLS.HucreDegerYaz(sonSatir + 1, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.HucreDegerYaz(sonSatir + 1, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));

            XLS.ArkaPlanRenk(sonSatir + 1, kaynakSutun, sonSatir + 1, sutun + 2, System.Drawing.Color.FromArgb(232, 232, 232));
            XLS.KoyuYap(sonSatir + 1, kaynakSutun, sonSatir + 1, sutun + 2, true);

            XLS.HucreBirlestir(satir - 2, sutun + 1, satir - 2, sutun + 2);
            XLS.HucreDegerYaz(satir - 2, sutun + 1, "TOPLAM");
            XLS.HucreDegerYaz(satir - 1, sutun + 1, "MİKTAR");
            XLS.HucreDegerYaz(satir - 1, sutun + 2, "TUTAR");

            XLS.ArkaPlanRenk(satir - 2, kaynakSutun, satir - 1, sutun + 2, System.Drawing.Color.FromArgb(232, 232, 232));

            //XLS.CerceveCiz(2, sonSatir + 2, 2, sutun + 3, LineStyle.MEDIUM, TabloRenk.BLACK);
            XLS.CerceveCizgiCiz(3, sonSatir + 1, 2, sutun + 2, LineStyle.MEDIUM, TabloRenk.BLACK);

            sutun = kaynakSutun;

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void BirimItibariylaTasinirHurdaRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray birimler = servisTMM.ABSKuraGoreHarcamaBirimleriListele(kullanan, tf.yil);
            ObjectArray bilgi = servisTMM.BirimItibariylaTasinirHurdaRaporu(kullanan, tf, tfKriter);
            ObjectArray hesapKodlari = servisTMM.HesapPlaniListele(kullanan, new HesapPlaniSatir { seviye = 1 }, new Sayfalama());
            ObjectArray toplamlar = new ObjectArray();

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
            int kaynakSutun = 0;
            int formatSatir = 0;
            int formatSutun = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "BirimBazliTasinirHurdaRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("FormatOrnek", ref formatSatir, ref formatSutun);
            XLS.HucreAdBulYaz("Baslik", tf.yil + " YILI BİRİMLER İTİBARIYLA KULLANILMAZ HAL VE HURDAYA AYRILAN TAŞINIRLAR LİSTESİ");

            satir = kaynakSatir;
            kaynakSutun = sutun;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            decimal ilMiktarToplam = 0;
            decimal ilTutarToplam = 0;
            int cnt = 0;
            foreach (HarcamaBirimi birim in birimler.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                //XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun, satir, (hesapKodlari.objeler.Count * 2) + 3);
                //XLS.HucreDegerYaz(satir, sutun - 1, OrtakFonksiyonlar.ConvertToInt(birim.kod, 0));
                XLS.HucreDegerYaz(satir, sutun, birim.ad);
                if (birim.kod.Length == 6)
                    XLS.KoyuYap(satir, sutun, satir, (hesapKodlari.objeler.Count * 2) + 3, true);
                else
                    XLS.KoyuYap(satir, sutun, satir, (hesapKodlari.objeler.Count * 2) + 3, false);
                foreach (HesapPlaniSatir hesapPlaniKod in hesapKodlari.objeler)
                {
                    foreach (TNS.TMM.HurdaRaporu tif in bilgi.objeler)
                    {
                        if (tif.hBirimKod == "-1" && cnt < hesapKodlari.objeler.Count)
                        {
                            toplamlar.objeler.Add(tif);
                            cnt++;
                        }

                        if (birim.kod == tif.hBirimKod)
                        {
                            if (hesapPlaniKod.hesapKod == tif.hesapPlanKod)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(tif.miktar.ToString(), (double)0));
                                XLS.HucreDegerYaz(satir, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(tif.tutar.ToString(), (double)0));
                                //XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
                                ilMiktarToplam += tif.miktar;
                                ilTutarToplam += tif.tutar;
                            }
                        }
                    }
                    sutun = sutun + 2;
                }

                XLS.HucreDegerYaz(satir, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(ilMiktarToplam.ToString(), (double)0));
                XLS.HucreDegerYaz(satir, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(ilTutarToplam.ToString(), (double)0));

                sutun = kaynakSutun;
                ilMiktarToplam = 0;
                ilTutarToplam = 0;
            }

            int sonSatir = satir;
            satir = kaynakSatir;
            sutun = kaynakSutun;

            XLS.SatirAc(sonSatir + 1, 1);
            XLS.HucreDegerYaz(sonSatir + 1, sutun, "TOPLAM");

            foreach (HesapPlaniSatir hesapPlaniKod in hesapKodlari.objeler)
            {
                foreach (TNS.TMM.HurdaRaporu tif in toplamlar.objeler)
                {
                    if (hesapPlaniKod.hesapKod == tif.hesapPlanKod)
                    {
                        XLS.HucreDegerYaz(sonSatir + 1, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(tif.miktar.ToString(), (double)0));
                        XLS.HucreDegerYaz(sonSatir + 1, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(tif.tutar.ToString(), (double)0));

                        miktarToplam += tif.miktar;
                        tutarToplam += tif.tutar;
                        break;
                    }
                }
                //XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, (hesapKodlari.objeler.Count*2)+3, satir, sutun);
                XLS.HucreBirlestir(satir - 2, sutun + 1, satir - 2, sutun + 2);

                XLS.HucreDegerYaz(satir - 2, sutun + 1, hesapPlaniKod.hesapKod + "- " + hesapPlaniKod.aciklama);
                XLS.HucreDegerYaz(satir - 1, sutun + 1, "MİKTAR");
                XLS.HucreDegerYaz(satir - 1, sutun + 2, "TUTAR");

                sutun = sutun + 2;
            }

            XLS.HucreDegerYaz(sonSatir + 1, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.HucreDegerYaz(sonSatir + 1, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));

            XLS.ArkaPlanRenk(sonSatir + 1, kaynakSutun, sonSatir + 1, sutun + 2, System.Drawing.Color.FromArgb(232, 232, 232));
            XLS.KoyuYap(sonSatir + 1, kaynakSutun, sonSatir + 1, sutun + 2, true);

            XLS.HucreBirlestir(satir - 2, sutun + 1, satir - 2, sutun + 2);
            XLS.HucreDegerYaz(satir - 2, sutun + 1, "TOPLAM");
            XLS.HucreDegerYaz(satir - 1, sutun + 1, "MİKTAR");
            XLS.HucreDegerYaz(satir - 1, sutun + 2, "TUTAR");

            XLS.ArkaPlanRenk(satir - 2, kaynakSutun, satir - 1, sutun + 2, System.Drawing.Color.FromArgb(232, 232, 232));

            //XLS.CerceveCiz(2, sonSatir + 2, 2, sutun + 3, LineStyle.MEDIUM, TabloRenk.BLACK);
            XLS.CerceveCizgiCiz(3, sonSatir + 1, 2, sutun + 2, LineStyle.MEDIUM, TabloRenk.BLACK);

            sutun = kaynakSutun;

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri sunucudaki TasinirIslemMalzemeTarihceRaporu yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi üst kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Taşınır işlem fişi detay kriter bilgilerini tutan nesne</param>
        private void BakanlikDuzeyiTasinirHurdaRaporu(TNS.TMM.TasinirIslemForm tf, TNS.TMM.TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = servisTMM.BakanlikDuzeyiTasinirHurdaRaporu(kullanan, tf, tfKriter);

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
            string sablonAd = "BakanlikDuzeyiTasinirHurdaRaporu.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdBulYaz("Baslik", tf.yil + " YILI BAKANLIK DÜZEYİ KULLANILMAZ HAL VE HURDAYA AYRILAN TAŞINIRLAR LİSTESİ");

            satir = kaynakSatir;

            decimal miktarToplam = 0;
            decimal tutarToplam = 0;
            foreach (TNS.TMM.HurdaRaporu tif in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);

                string[] x = tif.hesapPlanKod.Split('.');

                if (x.Length > 1)
                {
                    for (int i = 0; i < x.Length; i++)
                        XLS.HucreDegerYaz(satir, sutun + i, OrtakFonksiyonlar.ConvertToInt(x[i], 0));

                    miktarToplam += tif.miktar;
                    tutarToplam += tif.tutar;
                }
                else
                {
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 2);
                    XLS.HucreDegerYaz(satir, sutun, tif.hesapPlanKod);
                    XLS.ArkaPlanRenk(satir, sutun, satir, sutun + 5, System.Drawing.Color.FromArgb(232, 232, 232));
                }

                XLS.HucreDegerYaz(satir, sutun + 3, tif.hesapPlanAd);

                XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(tif.miktar.ToString(), (double)0));

                XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(tif.tutar.ToString(), (double)0));
            }

            //Toplamlar yazılıyor
            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 5, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
            XLS.HucreDegerYaz(satir, sutun, Resources.TasinirMal.FRMTHU002);
            XLS.KoyuYap(satir, sutun, true);
            XLS.DuseyHizala(satir, sutun, 1);
            XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(miktarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 4, true);
            XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(tutarToplam.ToString(), (double)0));
            XLS.KoyuYap(satir, sutun + 5, true);
            XLS.ArkaPlanRenk(satir, sutun, satir, sutun + 5, System.Drawing.Color.FromArgb(211, 211, 211));

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, "pdf");
            }
        }

        /// <summary>
        /// Parametre olarak verilen depo durum kriterlerini sunucudaki taşınır depo durum yordamına
        /// gönderir, sunucudan gelen bilgi kümesini excel dosyasına yazar ve kullanıcıya gönderir.
        /// </summary>
        /// <param name="kriter">Depo durum kriter bilgilerini tutan nesne</param>
        private void YillikTuketimRaporu()
        {
            string[] sayilar = txtKisiSayisi.Text.Split(new Char[] { ',', ';', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (sayilar.Length != 4)
            {
                GenelIslemler.HataYaz(this, "Kişi sayısı boş geçilemez. Harcama birimindeki yıl bazlı kişi sayılarını \',\', \';\' karakterler ile ayırarak giriniz. Bulunduğunuz yıldan itibaren 3 yıl geriye dönük yıllara ait kişi sayılarını girmelisiniz. Örnek: (133,122,144,155)");
                return;
            }

            int[] kisiSayisi = new int[sayilar.Length];

            for (int s = 0; s < sayilar.Length; s++)
                kisiSayisi[s] = OrtakFonksiyonlar.ConvertToInt(sayilar[s], 0);

            DepoDurum kriter = new DepoDurum();
            kriter.muhasebeKod = txtMuhasebe.Text.Trim();
            kriter.muhasebeAd = lblMuhasebeAd.Text.Trim();
            kriter.harcamaKod = txtHarcamaBirimi.Text.Trim();
            kriter.harcamaAd = lblHarcamaBirimiAd.Text.Trim();
            kriter.hesapPlanKod = txtHesapPlanKod.Text.Trim();
            kriter.hesapPlanAd = lblHesapPlanAd.Text.Trim();

            string hesapPlanKod = kriter.hesapPlanKod;

            ObjectArray hesapPlanlari = servisTMM.HesapPlaniListele(kullanan, new HesapPlaniSatir { hesapKod = kriter.hesapPlanKod }, new Sayfalama());

            if (!hesapPlanlari.sonuc.islemSonuc)
            {
                GenelIslemler.HataYaz(this, hesapPlanlari.sonuc.hataStr);
                return;
            }

            if (hesapPlanlari.objeler.Count <= 0)
            {
                GenelIslemler.BilgiYaz(this, hesapPlanlari.sonuc.bilgiStr);
                return;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = string.Empty;

            sablonAd = "YillikTuketimRaporu.xlt";

            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            XLS.HucreAdBulYaz("muhasebeAd", kriter.muhasebeAd);
            XLS.HucreAdBulYaz("harcamaAd", kriter.harcamaAd);
            XLS.HucreAdBulYaz("muhasebeKod", kriter.muhasebeKod);
            XLS.HucreAdBulYaz("harcamaKod", kriter.harcamaKod);

            foreach (HesapPlaniSatir hp in hesapPlanlari.objeler)
            {
                satir++;
                sutun = 0;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 25, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun, hp.hesapKod);
                XLS.HucreDegerYaz(satir, sutun + 1, hp.aciklama);
                kriter.hesapPlanKod = hp.hesapKod;
                sutun = 2;

                ObjectArray bilgi = servisTMM.HesapPlaniBazindaYillikTuketimRaporu(kullanan, kriter);
                for (int i = 0; i < bilgi.objeler.Count; i++)
                {
                    DepoDurum depo = (DepoDurum)bilgi.objeler[i];

                    if (depo.yil == System.DateTime.Now.Year - 2)
                        sutun = 8;
                    else if (depo.yil == System.DateTime.Now.Year - 1)
                        sutun = 14;
                    else if (depo.yil == System.DateTime.Now.Year)
                        sutun = 20;
                    else
                        sutun = 2;

                    XLS.HucreDegerYaz(kaynakSatir - 2, sutun + 1, depo.yil);

                    if (depo.tip == 1)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 1, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 2, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.ConvertToDouble((depo.girenTutar / depo.girenMiktar).ToString(), (double)0));
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, sutun + 4, OrtakFonksiyonlar.ConvertToDouble(depo.girenMiktar.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 5, OrtakFonksiyonlar.ConvertToDouble(depo.girenTutar.ToString(), (double)0));
                        XLS.HucreDegerYaz(satir, sutun + 6, OrtakFonksiyonlar.ConvertToDouble((depo.girenMiktar / 12).ToString(), (double)0));
                    }
                }
            }

            satir++;

            int satirToplam = 0, sutunToplam = 0, kSayisi;

            XLS.HucreAdAdresCoz("BaslaSatirToplam", ref satirToplam, ref sutunToplam);
            kriter.hesapPlanKod = hesapPlanKod;

            ObjectArray toplam = servisTMM.ToplamYillikTuketimRaporu(kullanan, kriter);

            foreach (DepoDurum dp in toplam.objeler)
            {
                if (dp.yil == System.DateTime.Now.Year - 3)
                {
                    sutunToplam = 4;
                    kSayisi = kisiSayisi[3];
                }
                else if (dp.yil == System.DateTime.Now.Year - 2)
                {
                    sutunToplam = 6;
                    kSayisi = kisiSayisi[2];
                }
                else if (dp.yil == System.DateTime.Now.Year - 1)
                {
                    sutunToplam = 8;
                    kSayisi = kisiSayisi[1];
                }
                else
                {
                    sutunToplam = 10;
                    kSayisi = kisiSayisi[0];
                }

                XLS.HucreDegerYaz(satirToplam, sutunToplam, dp.yil);

                if (dp.tip == 1)
                {
                    XLS.HucreDegerYaz(satirToplam + 1, sutunToplam, OrtakFonksiyonlar.ConvertToDouble(dp.girenMiktar.ToString(), (double)0));
                }
                else if (dp.tip == 2)
                {
                    XLS.HucreDegerYaz(satirToplam + 2, sutunToplam, OrtakFonksiyonlar.ConvertToDouble(dp.girenMiktar.ToString(), (double)0));
                }
                else if (dp.tip == 3)
                {
                    XLS.HucreDegerYaz(satirToplam + 3, sutunToplam, OrtakFonksiyonlar.ConvertToDouble((dp.girenMiktar / kSayisi).ToString(), (double)0));
                }
                else if (dp.tip == 4)
                {
                    XLS.HucreDegerYaz(satirToplam + 4, sutunToplam, OrtakFonksiyonlar.ConvertToDouble((dp.girenMiktar / kSayisi).ToString(), (double)0));
                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            string gidenAd = "";

            gidenAd = "YillikTuketimRaporu" + kriter.harcamaKod;

            //OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, GenelIslemler.ExcelTur());

            if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "1")
            {
                XLS.DosyaSaklaTamYol();
                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, GenelIslemler.ExcelTur());
            }
            else if (TasinirGenel.ComboDegerDondur(ddlCiktiTur) == "2")
            {
                XLS.DosyaSaklamaFormatAta("pdf");
                XLS.DosyaSaklaTamYol();

                OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), gidenAd, true, "pdf");
            }
        }
    }

    public class SiralaAmortismaSuresi : System.Collections.IComparer
    {
        public int Compare(object o1, object o2)
        {
            if (o1 == o2) return 0;
            TNS.TMM.AmortismanRapor i_o1 = (TNS.TMM.AmortismanRapor)o1;
            TNS.TMM.AmortismanRapor i_o2 = (TNS.TMM.AmortismanRapor)o2;
            int deger = (i_o2.amortismanSuresi).CompareTo((i_o1.amortismanSuresi));

            return deger != 0 ? deger : i_o1.amortismanBaslamaYil.CompareTo(i_o2.amortismanBaslamaYil);
        }
    }

    public class SiralaMalzemeKodu : System.Collections.IComparer
    {
        public int Compare(object o1, object o2)
        {
            if (o1 == o2) return 0;
            TNS.TMM.AmortismanRapor i_o1 = (TNS.TMM.AmortismanRapor)o1;
            TNS.TMM.AmortismanRapor i_o2 = (TNS.TMM.AmortismanRapor)o2;
            int deger = (i_o1.hesapPlanKod).CompareTo((i_o2.hesapPlanKod));

            return deger != 0 ? deger : i_o1.amortismanBaslamaYil.CompareTo(i_o2.amortismanBaslamaYil);
        }
    }

    public class SiralaHesapKod : System.Collections.IComparer
    {
        public int Compare(object o1, object o2)
        {
            if (o1 == o2) return 0;
            TNS.TMM.DepoDurum i_o1 = (TNS.TMM.DepoDurum)o1;
            TNS.TMM.DepoDurum i_o2 = (TNS.TMM.DepoDurum)o2;
            return (i_o1.hesapPlanKod).CompareTo((i_o2.hesapPlanKod));
        }
    }
}