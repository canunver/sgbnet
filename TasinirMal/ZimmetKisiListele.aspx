<%@ Page Language="C#" CodeBehind="ZimmetKisiListele.aspx.cs" Inherits="TasinirMal.ZimmetKisiListele" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="Content-Security-Policy" content="img-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' 'frame-ancestors';" />
    <script language="JavaScript" type="text/javascript">
        function KimlikNoAl() {
            Ext1.Msg.prompt("Kimlik Numarası Giriniz.", "Zimmet bilgisini görmek istediğiniz kişinin kimlik numarasını yazınız",
                function (btn, kimlikNo) {
                    if (btn == 'ok') {
                        hdnKimlikNo.setValue(kimlikNo);
                        Ext1.net.DirectMethods.ZimmetKisiGrideYaz();
                    }
                }
            );
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnKimlikNo" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="MUHASEBE" />
                        <ext:RecordField Name="HARCAMABIRIMI" />
                        <ext:RecordField Name="AMBAR" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="MALZEMEADI" />
                        <ext:RecordField Name="FISTARIHI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:GridPanel ID="grdListe" runat="server" StoreID="strListe" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="true" Margins="104 20 10 20">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnRaporYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                    <DirectEvents>
                                        <Click OnEvent="btnRaporYazdir_Click" IsUpload="true" />
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel>
                        <Columns>
                            <ext:Column Header="Muhasebe" DataIndex="MUHASEBE" Width="200" />
                            <ext:Column Header="Harcama Birimi" DataIndex="HARCAMABIRIMI" Width="300" />
                            <ext:Column Header="Ambar" DataIndex="AMBAR" Width="325" />
                            <ext:Column Header="Sicil No" DataIndex="SICILNO" Width="200" />
                            <ext:Column Header="Malzeme Adı" DataIndex="MALZEMEADI" Width="275" />
                            <ext:Column Header="Fiş Tarihi" DataIndex="FISTARIHI" />
                        </Columns>
                    </ColumnModel>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="500" HideRefresh="true"
                            StoreID="strListe">
                            <Items>
                                <ext:Label ID="Label1" runat="server" Text="Satır sayısı:" />
                                <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                <ext:ComboBox ID="cmbPageSize" runat="server" Width="60">
                                    <Items>
                                        <ext:ListItem Text="50" />
                                        <ext:ListItem Text="100" />
                                        <ext:ListItem Text="250" />
                                        <ext:ListItem Text="500" />
                                        <ext:ListItem Text="1000" />
                                        <ext:ListItem Text="2500" />
                                        <ext:ListItem Text="5000" />
                                    </Items>
                                    <SelectedItem Value="500" />
                                    <Listeners>
                                        <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                    </Listeners>
                                </ext:ComboBox>
                            </Items>
                        </ext:PagingToolbar>
                    </BottomBar>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>

