<%@ Page Language="C#" Inherits="TasinirMal.ListeDevir" CodeBehind="ListeDevir.aspx.cs" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= Resources.TasinirMal.FRMLDV003 %></title>
    <script language="JavaScript" type="text/javascript">
        var VeriGoster = function (yil, muhasebeKod, harcamaKod, ambarKod, fisNo) {
            parent.txtGonYil.setValue(yil);
            parent.document.getElementById('txtGonMuhasebe').value = muhasebeKod;
            parent.document.getElementById('txtGonHarcamaBirimi').value = harcamaKod;
            parent.document.getElementById('txtGonAmbar').value = ambarKod;
            parent.document.getElementById('txtGonBelgeNo').value = fisNo;
            try {
                parent.txtGonBelgeNo.fireEvent("TriggerClick");
            } catch (e) { }
            parent.hidePopWin();
        }
    </script>
    <style type="text/css">
        .x-grid3-cell-inner, .x-grid3-hd-inner {
            white-space: normal !important;
        }

        .x-grid3-row td, .x-grid3-summary-row td {
            vertical-align: middle;
        }
    </style>
</head>
<body>
    <form id="aspnetForm" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
            <Items>
                <ext:BorderLayout ID="BorderLayout2" runat="server">
                    <Center>
                        <ext:GridPanel ID="grdDevir" runat="server" Layout="FitLayout" StripeRows="true"
                            AutoExpandColumn="muhasebeKod" TrackMouseOver="true" Border="false" ButtonAlign="Center">
                            <TopBar>
                                <ext:Toolbar runat="server" ID="toolBar">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$ Resources:TasinirMal, FRMLDV008 %>"
                                            Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydetOnayla_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="json" Value="Ext1.encode(grdDevir.getRowsValues({selectedOnly: true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Store>
                                <ext:Store ID="storeDevir" runat="server">
                                    <Reader>
                                        <ext:JsonReader>
                                            <Fields>
                                                <ext:RecordField Name="yil" />
                                                <ext:RecordField Name="muhasebeKod" />
                                                <ext:RecordField Name="muhasebeAd" />
                                                <ext:RecordField Name="harcamaKod" />
                                                <ext:RecordField Name="harcamaAd" />
                                                <ext:RecordField Name="ambarKod" />
                                                <ext:RecordField Name="ambarAd" />
                                                <ext:RecordField Name="fisNo" />
                                                <ext:RecordField Name="aciklama" />
                                            </Fields>
                                        </ext:JsonReader>
                                    </Reader>
                                </ext:Store>
                            </Store>
                            <ColumnModel>
                                <Columns>
                                    <ext:TemplateColumn ColumnID="muhasebeKod" DataIndex="muhasebeKod" Header="<%$ Resources:TasinirMal, FRMLDV004%>"
                                        Align="Left" Hideable="false">
                                        <Template ID="Template1" runat="server">
                                            <Html>
                                                <a href="javascript:VeriGoster('{yil}','{muhasebeKod}','{harcamaKod}','{ambarKod}','{fisNo}')">{muhasebeKod} - {muhasebeAd}</a>
                                            </Html>
                                        </Template>
                                    </ext:TemplateColumn>
                                    <ext:TemplateColumn ColumnID="harcamaKod" DataIndex="harcamaKod" Header="<%$ Resources:TasinirMal, FRMLDV005%>">
                                        <Template ID="Template2" runat="server">
                                            <Html>
                                           {harcamaKod}</br>{harcamaAd}
                                        </Html>
                                        </Template>
                                    </ext:TemplateColumn>
                                    <ext:TemplateColumn ColumnID="ambarKod" DataIndex="ambarKod" Header="<%$ Resources:TasinirMal, FRMLDV006%>">
                                        <Template ID="Template3" runat="server">
                                            <Html>
                                           {ambarKod} - {ambarAd}
                                        </Html>
                                        </Template>
                                    </ext:TemplateColumn>
                                    <ext:Column ColumnID="fisNo" DataIndex="fisNo" Header="<%$ Resources:TasinirMal, FRMLDV007%>"
                                        Width="50" Fixed="true" />
                                    <ext:Column ColumnID="aciklama" DataIndex="aciklama" Header="Açıklama" />
                                </Columns>
                            </ColumnModel>
                            <View>
                                <ext:GridView ID="GridView1" runat="server" ForceFit="true" AutoFill="true" />
                            </View>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
                            </SelectionModel>
                        </ext:GridPanel>
                    </Center>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
