using Ext1.Net;
using OrtakClass;
using System;
using System.Collections.Generic;
using TNS;
using TNS.KYM;
using TNS.TMM;
using TNS.UZY;

namespace TasinirMal
{
    public partial class TanimHesapPlaniBarkod : TMMSayfaV2
    {
        static TNS.TMM.ITMMServis servisTMM;//DalYukle metodu static olduðu için ihtiyaç duyuldu
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        protected void Page_Load(object sender, EventArgs e)
        {
            servisTMM = TNS.TMM.Arac.Tanimla();

            if (!X.IsAjaxRequest)
            {
                formAdi = Resources.TasinirMal.FRMBRKD001;
                SayfaUstAltBolumYaz(this);
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "mainYok", "<style type='text/css'>#main{display: none;}</style>");

                //Sayfaya giriþ izni varmý?
                GenelIslemler.SayfayaGirisYapabilirMi(kullanan, this);

                Ext1.Net.TreeNode root = new Ext1.Net.TreeNode(Resources.TasinirMal.FRMLHP002);
                root.Expanded = true;
                trvHesap.Root.Add(root);
                HesapPlaniYukle(root);

                if (hdnMuhasebeBirimi.Value == null || hdnHarcamaBirimi.Value == null || hdnHarcamaBirimi.Value == "" || hdnMuhasebeBirimi.Value == "")
                    wndGiris.Show();
            }
        }

        private void HesapPlaniYukle(Ext1.Net.TreeNode root)
        {
            TNS.TMM.HesapPlaniSatir hs = new TNS.TMM.HesapPlaniSatir();
            hs.seviye = 1;

            ObjectArray hv;
            try
            {
                hv = servisTMM.HesapPlaniListele(new TNS.KYM.Kullanici(), hs, new Sayfalama());
            }
            catch (Exception exc)
            {
                hv = new ObjectArray();
                hv.sonuc = new Sonuc(false, exc.Message, "");
            }

            if (hv.sonuc.islemSonuc)
            {
                Ext1.Net.AsyncTreeNode node;
                for (int i = 0; i < hv.ObjeSayisi; i++)
                {
                    hs = (TNS.TMM.HesapPlaniSatir)hv[i];

                    string ad = hs.hesapKod + " - " + hs.aciklama;
                    if (hs.olcuBirim != 0)
                        ad += " - " + hs.olcuBirimAd;

                    node = new AsyncTreeNode(hs.hesapKod, ad);
                    root.Nodes.Add(node);
                }

                string coklu = "false";

                //trvHesap.Listeners.BeforeLoad.Handler = "parent.Ext1.Net.DirectMethods.DalYukle(node, " + coklu + ")";

                trvHesap.Listeners.BeforeLoad.Handler = "DalYukle(node, " + coklu + ")";
            }
        }

        [Ext1.Net.DirectMethod(ClientProxy = Ext1.Net.ClientProxy.Ignore)]
        public static string DalYukle(string hesapKod, bool coklu)
        {
            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            string[] hesapKirilimlar = hesapKod.Split('.');

            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.hesapKod = hesapKod;
            if (hesapKod == "")
                hs.seviye = 1;
            else if (hesapKod.Length < 3)
                hs.seviye = 1;
            else
                hs.seviye = hesapKirilimlar.Length + 1;

            ObjectArray hv = new ObjectArray();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    hv = servisTMM.HesapPlaniListele(new Kullanici(), hs, new Sayfalama());
                    if (hv.objeler.Count == 0)
                        hs.seviye++;
                    else
                        break;
                }
            }
            catch (Exception exc)
            {
                hv = new ObjectArray();
                hv.sonuc = new Sonuc(false, exc.Message, "");
            }
            Ext1.Net.TreeNodeCollection nodes = new Ext1.Net.TreeNodeCollection();
            foreach (HesapPlaniSatir h in hv.objeler)
            {
                Ext1.Net.AsyncTreeNode asyncNode = new Ext1.Net.AsyncTreeNode();
                if (h.olcuBirim == 0)
                    asyncNode.Text = h.hesapKod + " - " + h.aciklama;
                else
                    asyncNode.Text = h.hesapKod + " - " + h.aciklama + " - " + h.olcuBirimAd;

                asyncNode.NodeID = h.hesapKod;
                if (h.detay)
                {
                    asyncNode.Leaf = true;
                    if (coklu)
                        asyncNode.Checked = ThreeStateBool.False;
                }
                else
                    asyncNode.Leaf = false;
                nodes.Add(asyncNode);
            }

            return nodes.ToJson();
        }

        [DirectMethod(Namespace = "Stratek")]
        public void AfterEdit(int id, string field, string oldValue, string newValue, object customer)
        {
            if (hdnHarcamaBirimi.Value.ToString() == "" || hdnHarcamaBirimi.Value.ToString() == "")
                return;

            TasHesapPlaniBarkod brk = new TasHesapPlaniBarkod();
            brk.tasBarkodID = id;
            brk.barkod = newValue;
            brk.tasHesapNo = hdnSeciliKod.Value.ToString().Trim().Replace(".", "");
            brk.harcamaBirimKod = hdnHarcamaBirimi.Value.ToString().Replace(".", "");
            brk.muhasebeKod = hdnMuhasebeBirimi.Value.ToString();
            Sonuc sonuc = servisTMM.HesapPlaniBarkodKaydet(kullanan, brk);
            if (sonuc.islemSonuc)
                X.Msg.Notify("Bilgi", string.Format("Barkod baþarý ile kayýt edildi")).Show();
            else
                GenelIslemler.MesajKutusu("Hata", "Kayýt iþlemi baþarýsýz");

            Yukle(hdnSeciliKod.Value.ToString());

        }

        //seçilen hesap kodu ile ilgili bilgileri gride yükler
        [DirectMethod]
        public void Yukle(string nodeID)
        {
            string muhasebe = OrtakFonksiyonlar.ConvertToStr(hdnMuhasebeBirimi.Value);
            string hb = OrtakFonksiyonlar.ConvertToStr(hdnHarcamaBirimi.Value);
            if (muhasebe == "" || hb == "")
            {
                Ext1.Net.X.AddScript("wndGiris.show()");
                return;
            }


            if (!string.IsNullOrEmpty(nodeID))
            {
                List<object> tipler = new List<object>();
                TasHesapPlaniBarkod brk = new TasHesapPlaniBarkod();
                brk.tasHesapNo = nodeID;
                brk.muhasebeKod = muhasebe;
                brk.harcamaBirimKod = hb;

                ObjectArray liste = servisTMM.HesapPlaniBarkodGetir(brk);

                if (liste.sonuc.islemSonuc)
                {
                    foreach (TasHesapPlaniBarkod b in liste.objeler)
                    {
                        tipler.Add(new
                        {
                            barkodID = b.tasBarkodID,
                            hesapNo = b.tasHesapNo,
                            barkod = b.barkod,
                            muhasebeKod = b.muhasebeKod,
                            harcamaBirimKod = b.harcamaBirimKod,
                        });
                    }
                }

                strListe.DataSource = tipler;
                strListe.DataBind();
            }
        }

        protected void btnSil_Click(object sender, DirectEventArgs e)
        {
            string secilenKodlar = e.ExtraParams["secilenKodlar"];

            if (!string.IsNullOrEmpty(secilenKodlar))
            {
                Sonuc sonuc = servisTMM.HesapPlaniBarkodSil(kullanan, secilenKodlar);

                if (sonuc.islemSonuc)
                {
                    Ext1.Net.Notification.Show(new NotificationConfig() { Icon = Icon.Information, HideDelay = 2000, PinEvent = "click", Html = "Silme iþlemi tamamlandý", Title = "Bilgi" });
                    Yukle(hdnSeciliKod.Value.ToString());
                }
                else
                    GenelIslemler.MesajKutusu(this, sonuc.hataStr);
            }
            else
                GenelIslemler.MesajKutusu(this, "Lütfen listeden iþlem uygulanacak <font color='red'><b>barkodlarý</b></font> seçiniz.");
        }

        protected void btnKapat_Click(object sender, DirectEventArgs e)
        {
            string muhasebe = txtMuhasebe.Text;
            string hb = txtHarcamaBirimi.Text;

            ObjectArray muhListe = servisTMM.MuhasebeListele(kullanan, new TNS.TMM.Muhasebe() { kod = muhasebe });
            bool bulundu = false;
            foreach (TNS.TMM.Muhasebe m in muhListe.objeler)
            {
                if (m.kod == muhasebe)
                {
                    bulundu = true;
                }
            }

            if (!bulundu)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Muhasebe bulunamadý");
                return;
            }

            ObjectArray hbListe = servisTMM.HarcamaBirimiListele(kullanan, new HarcamaBirimi() { muhasebeKod = muhasebe, kod = hb });
            bulundu = false;
            foreach (HarcamaBirimi m in hbListe.objeler)
            {
                if (m.kod.Replace(".", "") == hb.Replace(".", ""))
                {
                    bulundu = true;
                }
            }

            if (!bulundu)
            {
                GenelIslemler.MesajKutusu("Uyarý", "Harcama Birimi bulunamadý");
                return;
            }

            hdnMuhasebeBirimi.Value = muhasebe;
            hdnHarcamaBirimi.Value = hb;
            Ext1.Net.X.AddScript("wndGiris.hide()");

        }

    }
}