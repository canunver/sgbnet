<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SayimTutanagiSorgu.aspx.cs"
    Inherits="TasinirMal.SayimTutanagiSorgu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link type="text/css" rel="stylesheet" href="../Script/GridExt.css" />
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=1"></script>
    <script type="text/javascript">
        function BelgeGirisAc(degerler) {
            var d = degerler.split(';');
            var yil = d[0];
            var muhasebe = d[1];
            var harcama = d[2];
            var ambar = d[3];
            var sayım = d[4];

            parent.tabPanelAna.setActiveTab("panelIslem");
            parent.panelIslem.getBody().document.getElementById('ddlYil').value = yil;
            parent.panelIslem.getBody().document.getElementById('txtMuhasebe').value = muhasebe;
            parent.panelIslem.getBody().document.getElementById('txtHarcamaBirimi').value = harcama;
            parent.panelIslem.getBody().document.getElementById('txtAmbar').value = ambar;
            parent.panelIslem.getBody().document.getElementById('hdnSayimNo').value = sayım;
            //            parent.hdnArama.setValue('1');
            parent.panelIslem.getBody().document.getElementById('btnListele').click();

        }

        function YeniTutanakHazırla() {
            //            parent.panelIslem.getBody().document.getElementById('ddlYil').value = "";
            parent.panelIslem.getBody().document.getElementById('txtMuhasebe').value = "";
            parent.panelIslem.getBody().document.getElementById('txtHarcamaBirimi').value = "";
            parent.panelIslem.getBody().document.getElementById('txtAmbar').value = "";
            parent.panelIslem.getBody().document.getElementById('hdnSayimNo').value = "";
            parent.hdnArama.setValue('');
            parent.tabPanelAna.setActiveTab("panelIslem");
            parent.panelIslem.getBody().document.getElementById('btnListele').click();
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <input type="hidden" id="hdnMuhasebeNo" runat="server" />
        <input type="hidden" id="hdnBirimNo" runat="server" />
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Store runat="server" ID="stoIst1">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="Yil" />
                        <ext:RecordField Name="Muhasebe" />
                        <ext:RecordField Name="Harcama" />
                        <ext:RecordField Name="Ambar" />
                        <ext:RecordField Name="BelgeNo" />
                        <ext:RecordField Name="Tarih" Type="Date" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: #DFE8F6">
            <Items>
                <ext:BorderLayout ID="BorderLayout1" runat="server">
                    <West Split="true" UseSplitTips="true" CollapsibleSplitTip="Kapatmak için çift tıklayın"
                        ExpandableSplitTip="Açmak için çift tıklayın" MarginsSummary="5">
                        <ext:FormPanel ID="Panel3" runat="server" Collapsible="true" Title="Sorgu Alanları"
                            Width="350" Padding="5" StyleSpec="background-color: #DFE8F6" Border="false"
                            LabelWidth="130" BodyStyle="background-color: #DFE8F6">
                            <Items>
                                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Yıl" Width="80">
                                    <Items>
                                        <ext:ComboBox ID="ddlYil" runat="server" Editable="false" Width="80">
                                        </ext:ComboBox>
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="cf2" runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" MaxLength="5" FieldLabel="Muhasebe Birimi Kodu"
                                            Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:DisplayField ID="lblMuhasebeAd" runat="server">
                                        </ext:DisplayField>
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="cf3" runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" MaxLength="15" FieldLabel="Harcama Birimi Kodu"
                                            Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:DisplayField ID="lblHarcamaBirimiAd" runat="server">
                                        </ext:DisplayField>
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="cf4" runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtAmbar" runat="server" MaxLength="3" FieldLabel="Ambar Kodu"
                                            Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:DisplayField ID="lblAmbarAd" runat="server">
                                        </ext:DisplayField>
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="cf5" runat="server" FieldLabel="Birim" Hidden="true">
                                    <Items>
                                        <ext:DropDownField ID="ddlBirim" runat="server" Width="120" TriggerIcon="Ellipsis"
                                            Mode="ValueText">
                                            <Component>
                                                <ext:Panel ID="Panel1" runat="server" Width="400">
                                                    <Items>
                                                        <ext:TreePanel ID="TreePanel1" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                            Border="false" RootVisible="false">
                                                            <Root>
                                                                <ext:AsyncTreeNode NodeID="0" Expanded="true" Icon="Note" />
                                                            </Root>
                                                            <Listeners>
                                                                <BeforeLoad Fn="OrgBirimDoldur" />
                                                                <Click Handler="#{ddlBirim}.setValue(node.id, node.text, false);#{ddlBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebe\', \'txtHarcamaBirimi\', \'txtAmbar\');');" />
                                                            </Listeners>
                                                        </ext:TreePanel>
                                                    </Items>
                                                    <Listeners>
                                                        <Show Handler="try { TreePanel1.getRootNode().findChild('id', ddlBirim.getValue(), true).select(); } catch (e) { }" />
                                                    </Listeners>
                                                </ext:Panel>
                                            </Component>
                                        </ext:DropDownField>
                                    </Items>
                                </ext:CompositeField>
                            </Items>
                        </ext:FormPanel>
                    </West>
                    <Center MarginsSummary="5">
                        <ext:GridPanel ID="grdListe" runat="server" StoreID="stoIst1">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnListe" runat="server" Text="<%$ Resources:ButceMuhasebe, MUHGNL008 %>"
                                            Icon="TableGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListe_Click">
                                                    <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnYeniTutanak" runat="server" Text="Yeni Tutanak" Icon="PageWhite">
                                            <Listeners>
                                                <Click Fn="YeniTutanakHazırla" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:RowNumbererColumn />
                                    <ext:Column DataIndex="Yil" ColumnID="Yil" Header="Yıl" Width="80" Groupable="false"
                                        Align="Center" Fixed="true">
                                    </ext:Column>
                                    <ext:Column DataIndex="Muhasebe" ColumnID="Muhasebe" Header="<%$ Resources:TasinirMal, FRMSYL003 %>"
                                        Width="120">
                                    </ext:Column>
                                    <ext:Column DataIndex="Harcama" ColumnID="Harcama" Header="<%$ Resources:TasinirMal, FRMSYL005 %>"
                                        Width="120">
                                    </ext:Column>
                                    <ext:Column DataIndex="Ambar" ColumnID="Ambar" Header="<%$ Resources:TasinirMal, FRMSYL007 %>"
                                        Width="100">
                                    </ext:Column>
                                    <ext:Column DataIndex="BelgeNo" ColumnID="BelgeNo" Header="<%$ Resources:TasinirMal, FRMSYL011 %>"
                                        Align="Left" Width="100">
                                    </ext:Column>
                                    <ext:DateColumn DataIndex="Tarih" Header="<%$ Resources:TasinirMal, FRMSYL012 %>"
                                        Align="Center" Width="120" Format="dd/m/Y">
                                    </ext:DateColumn>
                                </Columns>
                            </ColumnModel>
                            <LoadMask ShowMask="true" Msg="Lütfen bekleyin..." />
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
                                    StoreID="stoIst1">
                                    <Items>
                                        <ext:Label ID="Label1" runat="server" Text="Sayfada gösterilecek satır sayısı:" />
                                        <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                        <ext:ComboBox ID="cmbPageSize" runat="server" Width="60">
                                            <Items>
                                                <ext:ListItem Text="50" />
                                                <ext:ListItem Text="100" />
                                                <ext:ListItem Text="250" />
                                                <ext:ListItem Text="500" />
                                                <ext:ListItem Text="1000" />
                                            </Items>
                                            <SelectedItem Value="100" />
                                            <Listeners>
                                                <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                    </Center>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
