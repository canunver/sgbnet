<%@ Page Language="C#" CodeBehind="TanimAmortismanSinir.aspx.cs" Inherits="TasinirMal.TanimAmortismanSinir" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<%@ Register Assembly="Istemci" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function btnDurdur_Click() {
            TaskManager1.stopTask('IslemGostergec');
            Ext1.net.DirecMethods.btnDurdur_Click();
        }

        function Bitir(mesaj) {
            TaskManager1.stopTask('IslemGostergec');
            alert(mesaj);
            wndDurum.hide();
        }

        function KomutSorIsle(komut, satir, satirNo, sutun) {
            if (komut == "yazk" || komut == "yaz" || komut == "ac") {
                KomutIsle(komut, satir, satirNo, sutun)
            }
            else {
                Ext1.Msg.show({
                    title: "İzin",
                    msg: "İşlem Yapılacak Onaylıyor musunuz?",
                    width: 300,
                    buttons: Ext1.Msg.OKCANCEL,
                    modal: true,
                    fn: function (btn, metin) {
                        if (btn == "ok") {
                            KomutIsle(komut, satir, satirNo, sutun)
                        }
                    }
                });
            }
        }

        var aktSatir;
        function KomutIsle(komut, satir, satirNo, sutun) {
            if (komut == "kaydet") {
                Ext1.net.DirectMethods.YilGuncelle(satir.data.yil, satir.data.sinir, satir.data.sinirTasit, satir.data.mifBelgeNo);
            }
            else if (komut == "uret") {
                wndDurum.setTitle(satir.data.yil + " yılına ait bilgiler oluşturuluyor");
                wndDurum.show();
                Ext1.net.DirectMethods.AmortismanUret(satir.data.yil);
            }
            else if (komut == "yaz") {
                Ext1.net.DirectMethods.AmortismanYaz(satir.data.yil, false, { isUpload: true });
            }
            else if (komut == "yazk") {
                Ext1.net.DirectMethods.AmortismanYaz(satir.data.yil, true, { isUpload: true });
            }
            else if (komut == "mif") {
                aktSatir = satir;
                Ext1.net.DirectMethods.AmortismanMIFUret(satir.data.yil, satir.data.mifBelgeNo);
            }
            else if (komut == "ac") {
                if (satir.data.mifBelgeNo != "") {
                    var bNo = satir.data.mifBelgeNo.split(",")[0];
                    sayfa = "../ButceMuhasebe/OdemeEmrimifFrame.aspx?menuYok=1&aramaYok=1&yil=" + satir.data.yil + "&belgeNo=" + bNo;
                    window.open(sayfa, "odemeEmri", "resizable,status");
                }
                else
                    alert("Ödeme emri oluşturulmamış.");
            }
        }

        function OdemeEmriNoYaz(belgeNo) {
            aktSatir.set("mifBelgeNo", belgeNo);
            alert(belgeNo + " numaralı belge üretildi");
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:GridPanel runat="server" ID="grdYillar" Margins="104 20 10 20" Region="Center"
                    ClicksToEdit="1" AutoExpandColumn="mifBelgeNo" StripeRows="true">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Label runat="server" Text="<%$ Resources:TasinirMal, FRMAMO007 %>" />
                                <ext:ToolbarSpacer Width="10" runat="server" />
                                <ext:ExtMuhSecim ID="cmbMuhasebe" runat="server" Width="460" Editable="true" MinChars="1"
                                    SelectOnFocus="true">
                                </ext:ExtMuhSecim>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store runat="server" ID="stoYillar">
                            <Reader>
                                <ext:JsonReader>
                                    <Fields>
                                        <ext:RecordField Name="yil" Type="Int" />
                                        <ext:RecordField Name="sinir" Type="Float" />
                                        <ext:RecordField Name="mifBelgeNo" Type="String" />
                                        <ext:RecordField Name="sinirTasit" Type="Float" />
                                    </Fields>
                                </ext:JsonReader>
                            </Reader>
                        </ext:Store>
                    </Store>
                    <ColumnModel>
                        <Columns>
                            <ext:Column DataIndex="yil" ColumnID="yil" Header="<%$ Resources:TasinirMal, FRMTAS004 %>"
                                Width="50">
                            </ext:Column>
                            <ext:NumberColumn DataIndex="sinir" ColumnID="sinir" Header="<%$ Resources:TasinirMal, FRMTAS005 %>"
                                Width="90" Align="Right" Format="9.990/i">
                                <Editor>
                                    <ext:NumberField runat="server" SelectOnFocus="true">
                                    </ext:NumberField>
                                </Editor>
                            </ext:NumberColumn>
                            <ext:NumberColumn DataIndex="sinirTasit" ColumnID="sinirTasit" Header="Taşıt Amortisman Sınırı"
                                Width="140" Align="Right" Format="9.990/i">
                                <Editor>
                                    <ext:NumberField runat="server" SelectOnFocus="true">
                                    </ext:NumberField>
                                </Editor>
                            </ext:NumberColumn>
                            <ext:Column DataIndex="mifBelgeNo" ColumnID="mifBelgeNo" Header="MİF Belge No" Width="150">
                            </ext:Column>
                            <ext:ImageCommandColumn Width="500" Header="İşlemler">
                                <Commands>
                                    <ext:ImageCommand CommandName="kaydet" Icon="Accept" Text="<%$ Resources:TasinirMal, FRMTAS006 %>" />
                                    <ext:ImageCommand CommandName="uret" Icon="Build" Text="<%$ Resources:TasinirMal, FRMTAS009 %>" />
                                    <ext:ImageCommand CommandName="yaz" Icon="PageExcel" Text="<%$ Resources:TasinirMal, Amortisman022 %>" />
                                    <ext:ImageCommand CommandName="yazk" Icon="PageExcel" Text="Kümülatif" />
                                    <ext:ImageCommand CommandName="mif" Icon="PageAdd" Text="MİF Üret" />
                                    <ext:ImageCommand CommandName="ac" Icon="PageExcel" Text="MİF Aç" />
                                </Commands>
                            </ext:ImageCommandColumn>
                        </Columns>
                    </ColumnModel>
                    <Listeners>
                        <Command Fn="KomutSorIsle" />
                    </Listeners>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
        <ext:TaskManager ID="TaskManager1" runat="server">
            <Tasks>
                <ext:Task TaskID="IslemGostergec" Interval="2000" AutoRun="false">
                    <DirectEvents>
                        <Update OnEvent="RefreshProgress" />
                    </DirectEvents>
                </ext:Task>
            </Tasks>
        </ext:TaskManager>
        <ext:Window runat="server" ID="wndDurum" Width="400" Height="250" Layout="FormLayout"
            Hidden="true" Modal="true" Closable="true" LabelWidth="300">
            <Items>
                <ext:Label ID="lblToplamAmbar" runat="server" FieldLabel="İşlem görecek ambar sayısı"
                    AnchorHorizontal="99%" />
                <ext:Label ID="lblAmbarSayac" runat="server" FieldLabel="İşlem gören ambar sayısı"
                    AnchorHorizontal="99%" />
                <ext:Label ID="lblKalanAmbar" runat="server" FieldLabel="Kalan ambar sayısı" AnchorHorizontal="99%" />
                <ext:Label ID="lblIslemYapilanAmbar" runat="server" FieldLabel="Ambar" AnchorHorizontal="99%"
                    HideLabel="true" Text="10" />
                <ext:Label ID="Label1" runat="server" Html="&nbsp;" AnchorHorizontal="99%" />
                <ext:ProgressBar ID="Progress1" runat="server" AnchorHorizontal="99%" />
            </Items>
            <Buttons>
                <ext:Button runat="server" Text="Durdur" ID="btnDurdur">
                    <Listeners>
                        <Click Fn="btnDurdur_Click"></Click>
                    </Listeners>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
