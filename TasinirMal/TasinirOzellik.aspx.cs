using Ext1.Net;
using OrtakClass;
using System;
using TNS;
using TNS.TMM;

namespace TasinirMal
{
    /// <summary>
    /// Bölge tanım bilgilerinin kayıt, silme ve listeleme işlemlerinin yapıldığı sayfa
    /// </summary>
    public partial class TasinirOzellik : TMMSayfaV2
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
            if (!X.IsAjaxRequest)
            {
                //formAdi = "";
                //SayfaUstAltBolumYaz(this);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriş izni varmı?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                txtTarih.InnerHtml = DateTime.Now.ToString("dd.MM.yyyy - hh:mm:ss");
            }
        }

        [DirectMethod]
        public string[] Listele()
        {
            SicilNoHareket kriter = new SicilNoHareket();
            kriter.muhasebeKod = Request.QueryString["muhasebeKod"] + "";
            kriter.harcamaBirimKod = Request.QueryString["harcamaBirimKod"] + "";
            kriter.ambarKod = Request.QueryString["ambarKod"] + "";
            kriter.prSicilNo = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["prSicilNo"] + "", 0);

            ObjectArray bilgiler = servisTMM.ButunSicilNoListele(kullanan, kriter);

            if (!bilgiler.sonuc.islemSonuc)
                GenelIslemler.MesajKutusu("Uyarı", bilgiler.sonuc.hataStr);

            if (bilgiler.objeler.Count == 0)
                GenelIslemler.MesajKutusu("Uyarı", Resources.TasinirMal.FRMSCO003);

            ObjectArray barkodSicilListe = servisTMM.BarkodSicilNoListele(kullanan, kriter, new Sayfalama());
            SicilNoHareket barkodSicilHareket = barkodSicilListe.objeler.Count == 1 ? (SicilNoHareket)barkodSicilListe[0] : new SicilNoHareket();

            if (barkodSicilListe.objeler.Count == 0)
                barkodSicilHareket = (SicilNoHareket)bilgiler.objeler[0];

            //raporParam["yil"] = DateTime.Now.Year.ToString();
            ////raporParam["donem"] = form.donem + "";
            //raporParam["muhasebeKod"] = pgFiltre.Source["prpMuhasebe"].Value.Trim();
            //raporParam["harcamaKod"] = pgFiltre.Source["prpHarcamaBirimi"].Value.Trim();
            //raporParam["ambarKod"] = "";
            //raporParam["hesapPlanKod"] = hesapKod;
            //raporParam["raporTur"] = 6; //Amortisman yapılmışlar
            //raporParam["prSicilNolar"] = prSicilNolar;

            AmortismanBilgi amb = TasinirGenel.AmortismanBilgileri(servisTMM, kullanan, kriter.prSicilNo, barkodSicilHareket.muhasebeKod, barkodSicilHareket.harcamaBirimKod);

            string[] ozellik = new string[39];
            foreach (TNS.TMM.SicilNoHareket t in bilgiler.objeler)
            {
                if (t.prSicilNo != barkodSicilHareket.prSicilNo)
                    continue;

                t.odaKod = barkodSicilHareket.odaKod;
                t.odaAd = barkodSicilHareket.odaAd;

                //Yazılım ve gayrimenkul bulunduğu yer bilgisini değiştirebiliyor. Yazılım zimmet yapılmış. Bu nedenle zimmet yer ile bulunduğu yer bilgisi eşitlenmeli
                if (!string.IsNullOrWhiteSpace(amb.bulunduguYer))
                {
                    t.odaKod = amb.bulunduguYer;
                    t.odaAd = amb.bulunduguYerAd;
                }

                ozellik[0] = t.sicilNo.ToString() + " " + t.ozellik.bisNo; //Demirbaş ve Bis No
                ozellik[1] = t.ozellik.disSicilNo + " " + t.ozellik.eskiBisNo1;  //Eski Dem. ve Bis No
                ozellik[2] = t.ozellik.giai; //Seri No / Açıklama
                ozellik[3] = t.ozellik.disSicilNo2 + " " + t.ozellik.eskiBisNo2;  //Daha Eski Dem. ve Bis No
                ozellik[4] = t.ozellik.ekNo; //"EK NO";
                ozellik[5] = t.hesapPlanKod + " - " + t.hesapPlanAd; //Malzeme Kodu
                ozellik[6] = t.fisTarih.ToString(); // t.ozellik.yayinTarihi; //"ALIŞ TARIHI";
                ozellik[7] = t.fisTarih.ToString(); //t.ozellik.hesapTarih; //"HESAP TARIHI";
                ozellik[8] = t.odaKod != "" ? t.odaKod + " - " + t.odaAd : t.ambarAd; //Oda Kod
                //if (string.IsNullOrWhiteSpace(t.odaKod))
                //    ozellik[8] = t.ozellik.bulunduguYer; //bulunduguYer
                //ozellik[9] = t.harcamaBirimKod + " - " + t.harcamaBirimAd;
                ozellik[9] = barkodSicilHareket.harcamaBirimKod + " - " + barkodSicilHareket.harcamaBirimAd;
                ozellik[10] = t.ozellik.butceKodu; //"BUTCE KODU";
                ozellik[11] = "";// t.ozellik.blokajKodu; // "BLOKAJ KODU"; blokajKodu başka bilgileri taşıyor
                ozellik[12] = Math.Truncate(amb.ilkAmartismanYuzdesi * 100) / 100 + "";//ILK AMORTI YUZDESI
                ozellik[13] = amb.buAmartismanYuzdesi.ToString("0.##");//BUL. AMORTI YUZDESI
                ozellik[14] = TasinirGenel.ParaFormatla(amb.ilkBedel);//ILK BEDEL
                ozellik[15] = TasinirGenel.ParaFormatla(amb.birikenAmortisman);//BIRIKEN AMORT.
                ozellik[16] = TasinirGenel.ParaFormatla(amb.bedelDuzeltmeFarki);//BEDEL DUZ FARKI
                ozellik[17] = TasinirGenel.ParaFormatla(amb.amortiDuzeltmeFarki);//AMORT DUZ FARKI
                ozellik[18] = TasinirGenel.ParaFormatla(amb.sonBedel);//SON BEDEL
                ozellik[19] = TasinirGenel.ParaFormatla(amb.duzBirikenAmortisman);//DUZ. BIRIKEN AMORT.
                ozellik[20] = TasinirGenel.ParaFormatla(amb.ayrilanSonAmortisman);//AYR. SON AMORT.
                ozellik[21] = TasinirGenel.ParaFormatla(amb.oncekiYillarAmortisman);//EV. YILLAR AMORT
                //ozellik[22] = t.ozellik.geldigiSube;

                ozellik[23] = "";//MUT. TARIHI
                ozellik[24] = "";//MUT. NO
                ozellik[25] = "";//ŞUBE MUT. TARIHI
                ozellik[26] = "";//SUBE MUT. NO 1
                ozellik[27] = "";//SUBE MUT. NO 2
                ozellik[28] = "";//SUBE MUT. NO 3
                ozellik[29] = "";//SUBE MUT. NO 4

                ozellik[30] = t.fisNo;
                ozellik[31] = t.ozellik.saseNo; //"Açıklama /Seri No";
                ozellik[32] = t.ozellik.markaAd; //"MARKA";
                ozellik[33] = t.ozellik.modelAd; //"MODEL";
                //ozellik[34] = t.ozellik.saseNo; //"Açıklama /Seri No";
                //ozellik[35] = t.ozellik.motorNo; //"MOTOR NO";
                //ozellik[36] = t.ozellik.plaka; //"PLAKA";
                //ozellik[37] = t.ozellik.garantiBitisTarihi.ToString(); // "GARANTI BITIS TARIHI";

                kriter.sicilNo = t.sicilNo;
            }

            //Harmoni Bilgisi **********************************************
            ozellik[34] = "";
            ozellik[35] = "";

            ObjectArray harmoniListe = servisTMM.HarmoniListele(kullanan, new Harmoni() { sicilNo = kriter.sicilNo });
            if (harmoniListe.objeler.Count > 0)
            {
                foreach (Harmoni harmoni in harmoniListe.objeler)
                {
                    if (harmoni.tip == ENUMHarmoniTip.LOKASYON)
                        ozellik[34] = harmoni.aciklama;
                    else if (harmoni.tip == ENUMHarmoniTip.ZIMMET)
                        ozellik[35] = harmoni.aciklama;
                }
            }
            //*******************************************************************

            ObjectArray tarihceler = servisTMM.SicilNoTarihceListele(kullanan, kriter);

            string sablonTarihce = "";

            foreach (SicilNoHareket sht in tarihceler.objeler)
            {
                string islemTipiAd = sht.islemTipiAd == "Zimmet(Ortak Alan) Verme" ? sht.islemTipiAd + "<br>" + sht.odaAd : sht.islemTipiAd;

                sablonTarihce += "<tr><td>" + sht.islemTarih.ToString() + "</td><td>" + sht.muhasebeKod + "</td><td>" + sht.harcamaBirimKod + "</td><td>" + sht.ambarKod + "</td><td>" + sht.fisNo + "</td><td>" + islemTipiAd + "</td></tr>";
            }

            ozellik[38] = /*"<tbody>" +*/ sablonTarihce/* + "</tbody>"*/;
            //sablonTarihce += "</table>";
            //sablon += "</br>" + sablonTarihceBaslik + sablonTarihce;

            return ozellik;
            //ltlBilgi.Text = sablon;


            //string sablon = "";
            //string sablonAyrinti = "<div id='divYazdir'><table border = '1' style='border:1px solid gray'>";
            //sablonAyrinti += "<tr><td><b>Demirbaş ve Bis No</b></td><td style='text-align:right'>{0}</td><td>&nbsp;</td><td><b>Seri No</b></td><td style='text-align:right'>{1}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Eski Dem. ve Bis No</b></td><td style='text-align:right'>{2}</td><td>&nbsp;</td><td><b>Ek No</b></td><td style='text-align:right'>{3}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Malzeme Kodu</b></td><td style='text-align:right'>{4}</td><td>&nbsp;</td><td><b>Yer Kodu</b></td><td style='text-align:right'>{5}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Alış Tarihi</b></td><td style='text-align:right'>{6}</td><td>&nbsp;</td><td><b>Hesap Tarihi</b></td><td style='text-align:right'>{7}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Müd Kodu</b></td><td style='text-align:right'>{8}</td><td>&nbsp;</td><td><b>Bütçe Kodu</b></td><td style='text-align:right'>{9}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Blokaj Kodu</b></td><td style='text-align:right'>{10}</td><td>&nbsp;</td></tr>";
            //sablonAyrinti += "<tr><td><b>İlk Amorti Yüzdesi</b></td><td style='text-align:right'>{11}</td><td>&nbsp;</td><td><b>Bul. Amorti Yüzdesi</b></td><td style='text-align:right'>{12}</td></tr>";
            //sablonAyrinti += "<tr><td><b>İlk Bedel</b></td><td style='text-align:right'>{13}</td><td>&nbsp;</td><td><b>Biriken Amorti</b></td><td style='text-align:right'>{14}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Bedel Düz. Farkı</b></td><td style='text-align:right'>{15}</td><td>&nbsp;</td><td><b>Amort Düz. Farkı</b></td><td style='text-align:right'>{16}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Son Bedel</b></td><td style='text-align:right'>{17}</td><td>&nbsp;</td><td><b>Düz. Biriken Amort.</b></td><td style='text-align:right'>{18}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Ayr. Son Amort.</b></td><td style='text-align:right'>{19}</td><td>&nbsp;</td><td><b>Ev. Yıllar Amort.</b></td><td style='text-align:right'>{20}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Mut. Tarihi</b></td><td style='text-align:right'>{21}</td><td>&nbsp;</td><td><b>Mut. No</b></td><td style='text-align:right'>{22}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Geldiği Şube</b></td><td style='text-align:right'>{23}</td><td>&nbsp;</td><td><b>Şube Mut. Tarihi</b></td><td style='text-align:right'>{24}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Fiş No</b></td><td style='text-align:right'>{25}</td><td>&nbsp;</td><td><b>İşlem No</b></td><td style='text-align:right'>{26}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Şube Mut. No 1</b></td><td style='text-align:right'>{27}</td><td>&nbsp;</td><td><b>Şube Mut. No 2</b></td><td style='text-align:right'>{28}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Şube Mut. No 3</b></td><td style='text-align:right'>{29}</td><td>&nbsp;</td><td><b>Şube Mut. No 4</b></td><td style='text-align:right'>{30}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Marka</b></td><td style='text-align:right'>{31}</td><td>&nbsp;</td><td><b>Model</b></td><td style='text-align:right'>{32}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Şase No</b></td><td style='text-align:right'>{33}</td><td>&nbsp;</td><td><b>Motor No</b></td><td style='text-align:right'>{34}</td></tr>";
            //sablonAyrinti += "<tr><td><b>Plaka</b></td><td style='text-align:right'>{35}</td><td>&nbsp;</td><td><b>Garanti Bitiş Tarihi</b></td><td style='text-align:right'>{36}</td></tr>";
            //sablonAyrinti += "</table></div>";

        }


        protected void btnIndir_Click(object sender, DirectEventArgs e)
        {
            string[] ozellik = Listele();

            if (ozellik == null || ozellik.Length == 0)
            {
                GenelIslemler.HataYaz(this, "Kayıt bulunamadı.");
                return;
            }

            string[] ozellikAlan = new string[]
            {
                "Demirbaş ve Bis No", "Eski Dem. ve Bis No",
                "Tanımı", "Daha Eski Dem. ve Bis No",
                "Ek No","Malzeme Kodu",
                "Alış Tarihi","Hesap Tarihi",
                "Yer Kodu","Müdürlük Kodu",
                "Harmoni Lokasyon","Harmoni Zimmet",
                "Bütçe Kodu","Blokaj Kodu",
                "Fiş No","Açıklama / Seri No",
                "Marka","Model",
                "İlk Amorti. Yüzdesi","Bul. Amorti. Yüzdesi",
                "İlk Bedel","Biriken Amorti.",
                "Bedel Düz. Farkı","Amort Düz. Farkı",
                "Son Bedel","Düz. Biriken Amort.",
                "Ayr. Son Dönem Amort.","Geçmiş Dönemler Amort.",
            };

            string[] ozellikDeger = new string[ozellikAlan.Length];

            ozellikDeger[0] = ozellik[0];
            ozellikDeger[1] = ozellik[1];
            ozellikDeger[2] = ozellik[2];
            ozellikDeger[3] = ozellik[3];
            ozellikDeger[4] = ozellik[4];
            ozellikDeger[5] = ozellik[5];
            ozellikDeger[6] = ozellik[6];
            ozellikDeger[7] = ozellik[7];
            ozellikDeger[8] = ozellik[8];
            ozellikDeger[9] = ozellik[9];
            ozellikDeger[10] = ozellik[34];
            ozellikDeger[11] = ozellik[35];
            ozellikDeger[12] = ozellik[10];
            ozellikDeger[13] = ozellik[11];
            ozellikDeger[14] = ozellik[30];
            ozellikDeger[15] = ozellik[31];
            ozellikDeger[16] = ozellik[32];
            ozellikDeger[17] = ozellik[33];
            ozellikDeger[18] = ozellik[12];
            ozellikDeger[19] = ozellik[13];
            ozellikDeger[20] = ozellik[14];
            ozellikDeger[21] = ozellik[15];
            ozellikDeger[22] = ozellik[16];
            ozellikDeger[23] = ozellik[17];
            ozellikDeger[24] = ozellik[18];
            ozellikDeger[25] = ozellik[19];
            ozellikDeger[26] = ozellik[20];
            ozellikDeger[27] = ozellik[21];


            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = System.IO.Path.GetTempFileName();
            string sablonAd = "TasinirOzellik.xlt";
            XLS.DosyaAc(raporSablonYol + "\\TMM\\" + sablonAd, sonucDosyaAd);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.HucreAdAdresCoz("BaslaSatir1", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            for (int i = 0; i < ozellikAlan.Length; i += 2)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, ozellikAlan[i]);
                XLS.HucreDegerYaz(satir, sutun + 2, ozellikDeger[i]);
                XLS.HucreDegerYaz(satir, sutun + 5, ozellikAlan[i + 1]);
                XLS.HucreDegerYaz(satir, sutun + 7, ozellikDeger[i + 1]);
            }

            XLS.SatirSil(kaynakSatir, 1);


            //Tarihçe

            string tarihceozellik = ozellik[38];
            tarihceozellik = tarihceozellik.Replace("<tr>", "");
            tarihceozellik = tarihceozellik.Replace("<td>", "");
            tarihceozellik = tarihceozellik.Replace("</td>", ";");
            tarihceozellik = tarihceozellik.Replace("</tr>", "!");

            string[] tarihce = tarihceozellik.Split('!');


            satir = 0;
            sutun = 0;
            kaynakSatir = 0;

            XLS.HucreAdAdresCoz("BaslaSatir2", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (string item in tarihce)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                string[] ta = item.Split(';');

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 20, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, ta[0]);
                XLS.HucreDegerYaz(satir, sutun + 1, ta[1]);
                XLS.HucreDegerYaz(satir, sutun + 3, ta[2]);
                XLS.HucreDegerYaz(satir, sutun + 5, ta[3]);
                XLS.HucreDegerYaz(satir, sutun + 7, ta[4]);
                XLS.HucreDegerYaz(satir, sutun + 8, ta[5]);
            }

            //************************************************************************************

            XLS.DosyaSaklaTamYol(); OrtakClass.DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), sonucDosyaAd, true, GenelIslemler.ExcelTur());
        }

    }
}