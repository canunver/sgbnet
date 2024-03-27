using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;
using TNS;
using TNS.KYM;
using TNS.TMM;
using OrtakClass;
using Ext1.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TasinirMal
{
    /// <summary>
    /// Web formu ile ilgili olayları (event) ve fonksiyonları tutan sınıf
    /// </summary>
    public partial class MuhasebeBilgisiOnIzleme : TMMSayfaV2
    {
        ITMMServis servisTMM = null;

        /// <summary>
        /// Sayfa hazırlanırken, çağrılan olay (event) fonksiyon.
        /// </summary>
        /// <param name="sender">Olayı uyandıran nesne</param>
        /// <param name="e">Olayın parametresi</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            servisTMM = TNS.TMM.Arac.Tanimla();

            if (!X.IsAjaxRequest)
            {
                formAdi = "Muhasebe Ön İzleme";
                yardimBag = yardimYol + "#YardimDosyasiAd";
                OrtakClass.GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                Listele();
            }
        }

        private void Listele()
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["yil"], 0);
            int donem = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["donem"], 0);
            int islemTipi = OrtakFonksiyonlar.ConvertToInt(Request.QueryString["islemTipi"], 0);
            string muhasebeKod = Request.QueryString["muhasebeKod"] + "";
            string harcamaKod = Request.QueryString["harcamaKod"] + "";
            string ambarKod = Request.QueryString["ambarKod"] + "";
            string fisNo = Request.QueryString["fisNo"] + "";

            List<object> listeStore = new List<object>();

            if (yil > 0)
            {
                if (islemTipi == 0)
                {
                    //Amortisman Form Oku
                    AmortismanIslemForm fromAmortisman = new AmortismanIslemForm()
                    {
                        yil = yil,
                        muhasebeKod = muhasebeKod,
                        harcamaKod = harcamaKod,
                        ambarKod = ambarKod,
                        fisNo = fisNo
                    };

                    ObjectArray formlar = servisTMM.AmortismanIslemFisiListele(kullanan, fromAmortisman);

                    if (formlar.sonuc.islemSonuc)
                    {
                        foreach (AmortismanIslemForm aif in formlar.objeler)
                        {
                            if (aif.donem == donem)
                            {
                                fromAmortisman = aif;
                                break;
                            }
                        }



                        ENUMMuhasebeIslemTur muhasebeIslemTur = ENUMMuhasebeIslemTur.AMORTISMAN;
                        //if (form.fisNo.StartsWith("B"))
                        //    muhasebeIslemTur = ENUMMuhasebeIslemTur.DEGERARTIS;

                        MuhasebeIslemiKriter mk = new MuhasebeIslemiKriter()
                        {
                            yil = fromAmortisman.yil,
                            muhasebeKod = fromAmortisman.muhasebeKod,
                            harcamaKod = fromAmortisman.harcamaKod,
                            ambarKod = fromAmortisman.ambarKod,
                            fisNo = fromAmortisman.fisNo,
                            donem = fromAmortisman.donem,
                            muhasebeIslemTur = muhasebeIslemTur
                        };

                        ObjectArray listeOnizleme = servisTMM.MuhasebeBilgiOnIzlemeOlustur(kullanan, mk);
                        if (listeOnizleme.sonuc.islemSonuc)
                        {
                            int siraNo = 1;
                            foreach (MuhasebeIslemi mi in listeOnizleme.objeler)
                            {
                                listeStore.Add(new
                                {
                                    ISLEMREFNO = siraNo++,
                                    ISLEMCINSI = mi.islemCinsi,
                                    SERVIS = mi.servis,
                                    DURUM = mi.durum,
                                    ISLEMYAPAN = mi.islemYapan,
                                    ISLEMTARIH = mi.islemTarih.Oku(),
                                    JSON = mi.json
                                });
                            }
                        }
                        else
                            GenelIslemler.MesajKutusu("HATA", listeOnizleme.sonuc.hataStr);

                        //OrtakFonksiyonlar.HataDosyaYaz("MerkezBankasiMuhasebeServisi.txt", string.Format("{0} : {1} \n{2}", "TasinirIslemFormOnaySorguMB", DateTime.Now.ToString("yyyy -MM-dd HH:mm:ss"), listeOnizleme.sonuc.hataStr), false);
                    }
                    else
                        GenelIslemler.MesajKutusu("HATA", formlar.sonuc.hataStr);

                    //***************************************************
                }
                else if (islemTipi == -1)
                {
                    //Değer Artış

                }
                else
                {
                    //Taşınır

                    TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                    tf.yil = yil;
                    tf.fisNo = fisNo;
                    tf.harcamaKod = harcamaKod;
                    tf.muhasebeKod = muhasebeKod;

                    ObjectArray bilgi = TasinirGenel.TasinirIslemFisiAc(servisTMM, kullanan, tf, false);

                    if (!bilgi.sonuc.islemSonuc)
                    {
                        GenelIslemler.MesajKutusu("HATA", bilgi.sonuc.hataStr);
                        return;
                    }
                    tf = (TNS.TMM.TasinirIslemForm)bilgi[0];


                    MuhasebeIslemiKriter mk = new MuhasebeIslemiKriter()
                    {
                        yil = tf.yil,
                        muhasebeKod = tf.muhasebeKod,
                        harcamaKod = tf.harcamaKod,
                        ambarKod = tf.ambarKod,
                        fisNo = tf.fisNo,
                        muhasebeIslemTur = ENUMMuhasebeIslemTur.TASINIRISLEMFISI,
                        islemTipTur = tf.islemTipTur,
                        islemTipKod = tf.islemTipKod
                    };

                    ObjectArray listeOnizleme = servisTMM.MuhasebeBilgiOnIzlemeOlustur(kullanan, mk);
                    if (listeOnizleme.sonuc.islemSonuc)
                    {
                        int siraNo = 1;
                        foreach (MuhasebeIslemi mi in listeOnizleme.objeler)
                        {
                            listeStore.Add(new
                            {
                                ISLEMREFNO = siraNo++,
                                ISLEMCINSI = mi.islemCinsi,
                                SERVIS = mi.servis,
                                DURUM = mi.durum,
                                ISLEMYAPAN = mi.islemYapan,
                                ISLEMTARIH = mi.islemTarih.Oku(),
                                JSON = mi.json
                            });
                        }
                    }
                    else
                        GenelIslemler.MesajKutusu("HATA", listeOnizleme.sonuc.hataStr);

                    //OrtakFonksiyonlar.HataDosyaYaz("MerkezBankasiMuhasebeServisi.txt", string.Format("{0} : {1} \n{2}", "TasinirIslemFormOnaySorguMB", DateTime.Now.ToString("yyyy -MM-dd HH:mm:ss"), listeOnizleme.sonuc.hataStr), false);

                }
            }

            strMuhasebe.DataSource = listeStore;
            strMuhasebe.DataBind();
        }

    }


}
