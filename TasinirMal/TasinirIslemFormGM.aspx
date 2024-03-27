<%@ Page Language="C#" CodeBehind="TasinirIslemFormGM.aspx.cs" Inherits="TasinirMal.TasinirIslemFormGM" EnableEventValidation="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=13"></script>
    <script language="JavaScript" type="text/javascript">
        var IslemTipDegisti = function (a, b) {
            var deger = b.data.KODTUR;
            var degerler = deger.split('*');
            if (degerler.length > 1) {
                var tip = Number(degerler[1]);
                EkranAyarla(tip);
            }
        }

        var EkranAyarla = function (tip) {
            var ekranTip = hdnEkranTip.getValue();

            if (tip < 50) {
                cmpEdinme.show();
                cmpCikis.hide();
                txtAd.show();
                if (ekranTip == "GM") {
                    txtAdres.show();
                    cmpKonum.show();
                }
                cmpSicilNo.setFieldLabel("Bağlı Olduğu Sicil");
            }
            else {
                cmpEdinme.hide();
                cmpCikis.show();
                cmpKonum.hide();
                txtAdres.hide();
                txtAd.hide();
                cmpSicilNo.setFieldLabel("İşlem Yapılacak Sicil");
            }

            if (tip == 51) {
                fsGonderenBirim.show();
                fcGonYilBelgeNo.hide();
            }
            else if (tip == 5) {
                fsGonderenBirim.show();
                fcGonYilBelgeNo.show();
            }
            else {
                setTimeout("fsGonderenBirim.hide();", 100);
                fcGonYilBelgeNo.hide();
            }

            if (tip == 53 || tip < 50) {
                txtFiyati.show();
            }
            else
                txtFiyati.hide();

        }

        var SicilNoSec = function (field, trigger, index) {
            if (field.id == "txtSicilNo") {

                if (trigger.dom.className.indexOf("x-form-clear-trigger") > -1) {
                    field.clear();
                    field.focus();
                    lblSicilNo.setText("");
                    return;
                }

                var adres = "ListeSicilNoYeni.aspx";
                adres += "?menuYok=1&cagiran=" + "txtSicilNo" + "&cagiranLabel=" + "lblSicilNo";
                adres += "&mb=" + txtMuhasebe.getValue();
                adres += "&hb=" + txtHarcamaBirimi.getValue();
                adres += "&hk=" + hdnHesapKod.getValue();
                adres += "&vermeDusme=-1";
                adres += "&listeTur=TEKSECIM";

                var genislik = 800;
                var yukseklik = 500;

                showPopWin(adres, genislik, yukseklik, true, null);
            }
        }

        var ddlIslemTipiSec = function (alan, deger) {
            Ext1.each(ddlIslemTipi.getStore().data.items, function (item) {
                if (item.data[alan] == deger) {
                    ddlIslemTipi.setValue(item.data.KODTUR);
                }
            });
        }

        var BelgeOzellikGoster = function () {
            var adres = "SicilNoOzellik.aspx?menuYok=1";
            adres += "&yil=" + txtYil.getValue();
            adres += "&mb=" + txtMuhasebe.getValue();
            adres += "&hbk=" + txtHarcamaBirimi.getValue();
            adres += "&belgeNo=" + txtBelgeNo.getValue();
            adres += "&tur=" + hdnBelgeTur.getValue();

            showPopWin(adres, 800, 500, true, null);
        }

        var YazdirKontrol = function () {
            if (txtBelgeNo.getValue() == "") {
                Ext1.Msg.show({ title: "Uyarı", msg: "Belge No boş olamaz.", icon: Ext1.Msg.WARNING, buttons: Ext1.Msg.OK });
                return false;
            }
            return true;
        }

        var BelgeAc = function (yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            txtYil.setValue(yil);
            txtMuhasebe.setValue(muhasebeKod);
            txtHarcamaBirimi.setValue(harcamaBirimiKod);
            txtBelgeNo.setValue(belgeNo);
            txtBelgeNo.fireEvent("TriggerClick");
        }

        var DosyaGoster = function () {
            if (txtBelgeNo.getValue() != "" && txtBelgeNo.getValue() != undefined) {
                var adres = "TasinirIslemDosya.aspx?menuYok=1";
                adres += "&yil=" + txtYil.getValue();
                adres += "&mb=" + txtMuhasebe.getValue();
                adres += "&hb=" + txtHarcamaBirimi.getValue();
                adres += "&belgeNo=" + txtBelgeNo.getValue();

                showPopWin(adres, 800, 500, true, null);
            }
            else {
                extKontrol.Msg.alert("Uyarı", "Belge No Boş Bırakılamaz.").Show();
            }
        }

        var OnayaGonder = function () {
            Ext1.Msg.show({
                title: 'Onay',
                msg: "Belge B/A onayına gönderilecektir. Bu işlemi onaylıyor musunuz?<br><br><b>Açıklama:</b>",
                width: 450,
                closable: false,
                buttons: Ext1.Msg.YESNO,
                buttonText:
                {
                    yes: 'Evet',
                    no: 'Hayır'
                },
                multiline: true,
                fn: function (buttonValue, inputText, showConfig) {
                    if (buttonValue == "yes") {
                        Ext1.net.DirectMethods.OnayaGonder(inputText, { eventMask: { showMask: true } });
                    }
                },
                icon: Ext1.Msg.QUESTION
            });
        }

        var DevirListesiAc = function () {
            var islemTur = ddlIslemTipi.value.split('*')[1];

            if (Number(islemTur) != 5 && Number(islemTur) != 8)//5-Devir Giriş 8-Dağıtım/İade Belgesi (Giriş);
            {
                Ext1.Msg.show({ title: "Uyarı", msg: res_FRMJSC007, icon: Ext1.Msg.WARNING, buttons: Ext1.Msg.OK });
            }
            else {
                var adres = "ListeDevir.aspx?menuYok=1";
                adres += "&yil=" + txtYil.getValue();
                adres += "&mb=" + txtMuhasebe.getValue();
                adres += "&hbk=" + txtHarcamaBirimi.getValue();
                adres += "&ak=" + txtAmbar.getValue();
                adres += "&b=" + txtBelgeNo.getValue();
                adres += "&is=" + islemTur;

                showPopWin(adres, 700, 400, true, null);
            }
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();EkranAyarla(0);document.getElementById('txtSicilNo').readOnly = true;" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden runat="server" ID="hdnKod" />
        <ext:Hidden runat="server" ID="hdnBelgeTur" />
        <ext:Hidden runat="server" ID="hdnHesapKod" />
        <ext:Hidden runat="server" ID="hdnPrSicilNo" />
        <ext:Hidden runat="server" ID="hdnEkranTip" />
        <ext:Hidden runat="server" ID="txtAmbar" />
        <ext:Store ID="strGMTur" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="AD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="Panel1" runat="server" Region="Center" StyleSpec="background-color:white;padding:5px"
                    Border="false" Layout="BorderLayout">
                    <Items>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10" LabelWidth="150" AutoScroll="true">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMTIG114 %>" Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnTemizle_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIG112%>" Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click" Before="return YazdirKontrol();" IsUpload="true">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="islem" Value="belge" Mode="Value" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnOzellik" runat="server" Text="<%$ Resources:TasinirMal, FRMTIG116 %>"
                                            Icon="ApplicationFormMagnify" OnClientClick="BelgeOzellikGoster();" Hidden="true">
                                        </ext:Button>
                                        <ext:Button ID="btnDosyaIslemleri" runat="server" Text="Dosya İşlemleri"
                                            Icon="DiskDownload">
                                            <Listeners>
                                                <Click Handler="DosyaGoster();"></Click>
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:Button ID="btnBelgeOnayla" runat="server" Text="<%$Resources:TasinirMal,FRMZFS046%>" Icon="Tick">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayla_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnBelgeOnayaGonder" runat="server" Text="Onaya Gönder" Icon="Tick">
                                            <Listeners>
                                                <Click Handler="OnayaGonder();" />
                                            </Listeners>
                                        </ext:Button>
                                        <%--<ext:Button ID="btnBelgeOnayaGonder" runat="server" Text="Onaya Gönder" Icon="Tick">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayaGonder_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>--%>
                                        <ext:Button ID="btnBelgeOnayKaldir" runat="server" Text="<%$Resources:TasinirMal,FRMZFS047%>" Icon="Cross">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayKaldir_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaya gönderilecektir. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnBelgeIptal" runat="server" Text="<%$Resources:TasinirMal,FRMZFS048%>" Icon="Delete">
                                            <DirectEvents>
                                                <Click OnEvent="btnIptal_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge iptal edilecektir. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:CompositeField runat="server" FieldLabel="Yıl, Belge Tarihi, Belge No">
                                    <Items>
                                        <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>"
                                            Width="60" />
                                        <ext:DateField ID="txtBelgeTarih" runat="server" EmptyText="Belge Tarihi" Width="100" />
                                        <ext:TriggerField ID="txtBelgeNo" runat="server" Width="100" EmptyText="Belge No">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <DirectEvents>
                                                <TriggerClick OnEvent="btnListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </TriggerClick>
                                                <SpecialKey Before="return e.getKey() == Ext1.EventObject.ENTER;" OnEvent="btnListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </SpecialKey>
                                            </DirectEvents>
                                        </ext:TriggerField>
                                        <ext:DisplayField ID="lblFormDurum" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG031%>"
                                            Width="140">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblMuhasebeAd" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG033%>"
                                            Width="140">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <Change Fn="TriggerChange" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblHarcamaBirimiAd" runat="server" StyleSpec="white-space: nowrap;" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:ComboBox ID="ddlIslemTipi" runat="server" FieldLabel="İşlem Tipi" Editable="true"
                                    Mode="Local" TriggerAction="All" ForceSelection="true" AllowBlank="false" SelectOnFocus="true"
                                    DisplayField="AD" ValueField="KODTUR" Width="140">
                                    <Store>
                                        <ext:Store ID="strIslemTipi" runat="server">
                                            <Reader>
                                                <ext:JsonReader IDProperty="KOD">
                                                    <Fields>
                                                        <ext:RecordField Name="AD" />
                                                        <ext:RecordField Name="KOD" />
                                                        <ext:RecordField Name="TUR" />
                                                        <ext:RecordField Name="KODTUR" />
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                            <SortInfo Field="AD" Direction="ASC" />
                                        </ext:Store>
                                    </Store>
                                    <Listeners>
                                        <Select Fn="IslemTipDegisti" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:ComboBox ID="ddlGayrimenkulTuru" runat="server" Editable="true" TypeAhead="true" Mode="Local"
                                    ForceSelection="true" FieldLabel="Gayrimenkül Türü" Width="140" StoreID="strGMTur" DisplayField="AD"
                                    ValueField="KOD" SelectOnFocus="true">
                                    <Listeners>
                                       <Select Handler="if(ddlGayrimenkulTuru.getValue()=='097.00') {ddlMuhasebeIslemTipi.show();} else {ddlMuhasebeIslemTipi.hide();}" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:ComboBox ID="ddlMuhasebeIslemTipi" runat="server" FieldLabel="Muhasebe İşlem Tipi" Editable="false"
                                    Mode="Local" TriggerAction="All" ForceSelection="true" SelectOnFocus="true"
                                    DisplayField="AD" ValueField="KOD" Width="300" ListWidth="400" Hidden="true">
                                    <Store>
                                        <ext:Store ID="strMuhasebeIslemTipi" runat="server">
                                            <Reader>
                                                <ext:JsonReader IDProperty="KOD">
                                                    <Fields>
                                                        <ext:RecordField Name="AD" />
                                                        <ext:RecordField Name="KOD" />
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                        </ext:Store>
                                    </Store>
                                </ext:ComboBox>
                                <ext:CompositeField runat="server" ID="cmpSicilNo">
                                    <Items>
                                        <ext:TriggerField ID="txtSicilNo" runat="server" FieldLabel="İşlem Yapılacak Sicil No" Width="140" StyleSpec="background-color:yellow">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Clear" />
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="SicilNoSec" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblSicilNo" runat="server" />
                                    </Items>
                                </ext:CompositeField>

                                <ext:FieldSet ID="fsGonderenBirim" runat="server" Title="Gönderen/Gönderilen Birim Bilgisi"
                                    Collapsible="false" Layout="form" LabelWidth="140" Width="455">
                                    <Items>
                                        <ext:DisplayField runat="server" Height="10" />
                                        <ext:CompositeField ID="cfGonMuhasebe" runat="server">
                                            <Items>
                                                <ext:TriggerField ID="txtGonMuhasebe" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG031%>"
                                                    Width="140">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" />
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <TriggerClick Fn="TriggerClick" />
                                                        <Change Fn="TriggerChange" />
                                                    </Listeners>
                                                </ext:TriggerField>
                                                <ext:Label ID="lblGonMuhasebeAd" runat="server" StyleSpec="white-space: nowrap;" />
                                            </Items>
                                        </ext:CompositeField>
                                        <ext:CompositeField ID="cfGonHarcamaBirimi" runat="server">
                                            <Items>
                                                <ext:TriggerField ID="txtGonHarcamaBirimi" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMSYG033%>"
                                                    Width="140">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" />
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Change Fn="TriggerChange" />
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                </ext:TriggerField>
                                                <ext:Label ID="lblGonHarcamaBirimiAd" runat="server" StyleSpec="white-space: nowrap;" />
                                            </Items>
                                        </ext:CompositeField>
                                        <ext:CompositeField ID="cfGonAmbar" runat="server" FieldLabel="Taşınır Ambar Kodu"
                                            Hidden="true">
                                            <Items>
                                                <ext:TriggerField ID="txtGonAmbar" runat="server" EnableKeyEvents="true" Width="140">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" />
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Change Fn="TriggerChange" />
                                                        <TriggerClick Fn="TriggerClick" />
                                                    </Listeners>
                                                </ext:TriggerField>
                                                <ext:Label ID="lblGonAmbarAd" runat="server" StyleSpec="white-space: nowrap;" />
                                            </Items>
                                        </ext:CompositeField>
                                        <ext:CompositeField ID="fcGonYilBelgeNo" runat="server" Layout="HBoxLayout" FieldLabel="Gelen Belgenin Yılı, Nosu"
                                            Hidden="true">
                                            <Items>
                                                <ext:SpinnerField ID="txtGonYil" runat="server" Width="60" />
                                                <ext:TriggerField ID="txtGonBelgeNo" runat="server" Width="75">
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Search" />
                                                    </Triggers>
                                                    <DirectEvents>
                                                        <TriggerClick OnEvent="btnGonListele_Click">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                        </TriggerClick>
                                                        <SpecialKey Before="return e.getKey() == Ext1.EventObject.ENTER;" OnEvent="btnGonListele_Click">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                        </SpecialKey>
                                                    </DirectEvents>
                                                </ext:TriggerField>
                                            </Items>
                                        </ext:CompositeField>
                                    </Items>
                                    <Tools>
                                        <ext:Tool Handler="DevirListesiAc();" Type="Gear" Qtip="Devir Listesi Aç" />
                                    </Tools>
                                </ext:FieldSet>

                                <ext:TextField ID="txtAd" runat="server" FieldLabel="Tanımı" Width="300" />
                                <ext:TextField ID="txtFiyati" runat="server" FieldLabel="Fiyatı" Width="140" MaskRe="[,0-9]" StyleSpec="text-align:right" />
                                <ext:TextArea ID="txtAdres" runat="server" FieldLabel="Adres" Width="300" Height="70" />
                                <ext:CompositeField runat="server" ID="cmpKonum" FieldLabel="Konumu">
                                    <Items>
                                        <ext:TextField ID="txtPaftaNo" runat="server" Note="Pafta No" Width="50" />
                                        <ext:TextField ID="txtAdano" runat="server" Note="Ada No" Width="50" />
                                        <ext:TextField ID="txtParselNo" runat="server" Note="Parsel No" Width="50" />
                                        <ext:TextField ID="txtAlani" runat="server" Note="Alanı m²" Width="50" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server" ID="cmpEdinme" FieldLabel="Edinme Durumu">
                                    <Items>
                                        <ext:TextField ID="txtEdinmeSekli" runat="server" Note="Şekli" Width="140" />
                                        <ext:DateField ID="txtEdinmeTarihi" runat="server" Note="Tarihi" Width="100" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server" ID="cmpCikis" FieldLabel="Çıkış Durumu">
                                    <Items>
                                        <ext:TextField ID="txtKayittanCikisNedeni" runat="server" Note="Nedeni" Width="140" />
                                        <ext:DateField ID="txtKayittanCikisTarihi" runat="server" Note="Tarihi" Width="100" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:TextArea ID="txtAciklama" runat="server" FieldLabel="Açıklama" Width="300"
                                    LabelWidth="120" Height="70" />

                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
