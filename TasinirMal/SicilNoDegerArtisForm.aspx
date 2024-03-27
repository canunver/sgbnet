<%@ Page Language="C#" CodeBehind="SicilNoDegerArtisForm.aspx.cs" Inherits="TasinirMal.SicilNoDegerArtisForm" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%--<script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=23"></script>--%>
    <%--<script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=21"></script>--%>
    <script language="JavaScript" type="text/javascript">
        var BelgeAc = function (yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
            txtMuhasebe.setValue(muhasebeKod);
            txtHarcamaBirimi.setValue(harcamaBirimiKod);
            txtBelgeNo.setValue(belgeNo);
            txtBelgeNo.fireEvent("TriggerClick");
        }

        var artisTutarTurDegisti = function () {
            var katsayi = rgArtisTutar.isVisible() && rdKaysayi.checked;
            cfArtisTutar.setFieldLabel(katsayi ? "Enflasyon Oranı % " : "Artış Tutarı")
            cfArtisTutar.label.setStyle({ "color": katsayi ? "#820d0d" : "#686868" })

            if (katsayi) {
                txtArtisTutar.setValue("");
                pnlEnflasyonAciklama.show();

                lblSonBedel1.setText(formatMoney(hdnSonBedel.getValue().replace(",", ".")));
            }
            else
                pnlEnflasyonAciklama.hide();

            txtArtisTutar.focus();
        }

        var islemTuruDegisti = function () {
            if (ddlTur.getValue() == "1") //Enflasyon 
            {
                rgArtisTutar.show();
                if (txtArtisTutar.getValue() != "")
                    rdTutar.setValue(true);
            }
            else {
                rgArtisTutar.hide();
            }
            artisTutarTurDegisti();

            if (ddlTur.getValue() == "3") //Değer Azalışı 
                cfArtisTutar.setFieldLabel("Azalış Tutarı")
            else {
                var katsayi = rgArtisTutar.isVisible() && rdKaysayi.checked;
                cfArtisTutar.setFieldLabel(katsayi ? "Enflasyon Oranı % " : "Artış Tutarı")
            }
        }

        var enflasyonHesapla = function () {
            if (ddlTur.getValue() == "1") //Enflasyon 
            {
                var katsayi = rgArtisTutar.isVisible() && rdKaysayi.checked;

                var sonBedel = parseFloat(hdnSonBedel.getValue().replace(",", "."));;
                var sonBedel2 = 0;

                if (katsayi)
                    sonBedel2 = sonBedel * (parseFloat(txtArtisTutar.getValue().replace(",", ".")) / 100);

                lblSonBedel2.setText(formatMoney(sonBedel2));
            }
        }

        Number.prototype.formatMoney = function (fractionDigits, decimal, separator) {
            fractionDigits = isNaN(fractionDigits = Math.abs(fractionDigits)) ? 2 : fractionDigits;

            decimal = typeof (decimal) === "undefined" ? "." : decimal;

            separator = typeof (separator) === "undefined" ? "," : separator;

            var number = this;

            var neg = number < 0 ? "-" : "";

            var wholePart = parseInt(number = Math.abs(+number || 0).toFixed(fractionDigits)) + "";

            var separtorIndex = (separtorIndex = wholePart.length) > 3 ? separtorIndex % 3 : 0;

            return neg +

                (separtorIndex ? wholePart.substr(0, separtorIndex) + separator : "") +

                wholePart.substr(separtorIndex).replace(/(\d{3})(?=\d)/g, "$1" + separator) +

                (fractionDigits ? decimal + Math.abs(number - wholePart).toFixed(fractionDigits).slice(2) : "");

        };

        var formatMoney = function (raw) {
            return Number(raw).formatMoney(2, ',', '.');
        }

    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();document.getElementById('txtSicilNo').readOnly = true; islemTuruDegisti();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Hidden ID="hdnBelgeNo" runat="server" />
        <ext:Hidden ID="hdnPrSicilNo" runat="server" />
        <ext:Hidden ID="txtAmbar" runat="server" />
        <ext:Hidden ID="hdnSonBedel" runat="server" />
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="Panel1" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Layout="BorderLayout">
                    <Items>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10" LabelWidth="150">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMKDG029%>"
                                            Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt işlemi yapılacak. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMGAG038%>"
                                            Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnTemizle_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Formda görülen bilgiler temizlenecektir. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:Button ID="btnBelgeOnayla" runat="server" Text="<%$Resources:TasinirMal,FRMZFS046%>"
                                            Icon="Tick">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayla_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnBelgeOnayaGonder" runat="server" Text="Onaya Gönder" Icon="Tick" Hidden="true">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayaGonder_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaylanacaktır. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnBelgeOnayKaldir" runat="server" Text="<%$Resources:TasinirMal,FRMZFS047%>"
                                            Icon="Cross">
                                            <DirectEvents>
                                                <Click OnEvent="btnOnayKaldir_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Belge onaya gönderilecektir. Bu işlemi onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnBelgeIptal" runat="server" Text="<%$Resources:TasinirMal,FRMZFS048%>"
                                            Icon="Delete">
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
                                <ext:CompositeField runat="server" FieldLabel="İşlem Tarihi, Numara">
                                    <Items>
                                        <ext:DateField ID="txtBelgeTarihi" runat="server" EmptyText="İşlem Tarihi" Width="100" />
                                        <ext:TriggerField ID="txtBelgeNo" runat="server" Width="100" EmptyText="İşlem No">
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
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG025 %>"
                                            Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblMuhasebeAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG027 %>"
                                            Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblHarcamaBirimiAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>

                                <ext:ComboBox ID="ddlTur" runat="server" FieldLabel="İşlem Türü" Editable="false" TypeAhead="true"
                                    Mode="Local"
                                    ForceSelection="true"
                                    TriggerAction="All"
                                    SelectOnFocus="true">
                                    <Items>
                                        <ext:ListItem Text="Enflasyon Düzeltme Farkı" Value="1" />
                                        <ext:ListItem Text="Değer Artışı" Value="2" />
                                        <ext:ListItem Text="Değer Azalışı" Value="3" />
                                    </Items>
                                    <SelectedItem Value="1" />
                                    <Listeners>
                                        <Select Handler="islemTuruDegisti();" />
                                    </Listeners>
                                </ext:ComboBox>

                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtSicilNo" runat="server" FieldLabel="Sicil No" Width="120" StyleSpec="background-color:yellow">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblSicilNo" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="cfArtisTutar" runat="server" FieldLabel="Artış Tutarı">
                                    <Items>
                                        <ext:TextField ID="txtArtisTutar" runat="server" Width="120" EnableKeyEvents="true">
                                            <Listeners>
                                                <KeyUp Handler="enflasyonHesapla();" />
                                            </Listeners>
                                        </ext:TextField>
                                        <ext:RadioGroup ID="rgArtisTutar" runat="server" ColumnsWidths="65,65" Hidden="true">
                                            <Items>
                                                <ext:Radio runat="server" ID="rdKaysayi" BoxLabel="Oran" />
                                                <ext:Radio runat="server" ID="rdTutar" BoxLabel="Tutar" Checked="true" />
                                            </Items>
                                            <Listeners>
                                                <Change Handler="artisTutarTurDegisti();" />
                                            </Listeners>
                                        </ext:RadioGroup>
                                    </Items>
                                </ext:CompositeField>
                                <ext:Panel ID="pnlEnflasyonAciklama" runat="server" Border="false" HideLabel="true">
                                    <Items>
                                        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Düzeltme Tutarı" StyleSpec="font-weight:bold;">
                                            <Items>
                                                <ext:Label ID="lblSonBedel2" runat="server" Text="0" Width="120" />
                                                <ext:Label ID="lblSonBedel1" runat="server" Text="0" />
                                            </Items>
                                        </ext:CompositeField>
                                        <ext:DisplayField FieldLabel="<span style='color:red;'>Enflasyon Açıklaması</span>" runat="server" Text="Artış oranını % olarak giriniz. Girilen rakam -100 den büyük olacak. Ekrana % rakamı girildiğinde son bedelden hesaplanarak ekrana yazılsın." />
                                    </Items>
                                </ext:Panel>
                                <ext:TextArea ID="txtGerekce" runat="server" Width="300" FieldLabel="Gerekçe"
                                    EmptyText="Gerekçe" Height="130" />

                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
