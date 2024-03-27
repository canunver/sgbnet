<%@ Page Language="C#" CodeBehind="TasinirIslemFormSorguYeni.aspx.cs" Inherits="TasinirMal.TasinirIslemFormSorguYeni" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirIslemSorguYeni.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=1"></script>
    <style type="text/css">
        .search-item
        {
            font: normal 11px tahoma, arial, helvetica, sans-serif;
            padding: 3px 10px 3px 10px;
            border: 1px solid #fff;
            border-bottom: 1px solid #eeeeee;
            white-space: normal;
            color: #555;
        }
        
        .white-menu .x-menu
        {
            background: white !important;
        }
    </style>
    <script type="text/javascript">
        var durumRenderer = function () {
            try {
                return ddlDurum.getText();
            } catch (e) {
                return ddlDurum.getStore().getById(1).data.Name;
            }
        };
        var islemTipiRenderer = function () {
            try {
                return ddlIslemTipi.getText();
            } catch (e) {
                return ddlIslemTipi.getStore().data.items[0].data.text;
            }
        };
    </script>
</head>
<body>
    <form id="form" runat="server">
    <input type="hidden" id="hdnMuhasebeNo" runat="server" />
    <input type="hidden" id="hdnBirimNo" runat="server" />
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <ext:Store ID="StoreDurum" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID">
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="Name" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreHesapPlan" runat="server" AutoLoad="false">
        <%--BARIS OnRefreshData="HesapStore_Refresh">--%>
        <Proxy>
            <ext:PageProxy />
        </Proxy>
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="KOD" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreListe" runat="server" IgnoreExtraFields="false" AutoLoad="false"
        RemotePaging="true" RemoteSort="true" RemoteGroup="true" GroupField="durum">
        <%--BARIS OnRefreshData="StoreListe_Refresh">--%>
        <Proxy>
            <ext:PageProxy />
        </Proxy>
        <Reader>
            <ext:JsonReader IDProperty="belgeNo">
                <Fields>
                    <ext:RecordField Name="belgeNo">
                    </ext:RecordField>
                    <ext:RecordField Name="yevmiyeNo">
                    </ext:RecordField>
                    <ext:RecordField Name="belgeTur">
                    </ext:RecordField>
                    <ext:RecordField Name="belgeTarih" Type="Date">
                    </ext:RecordField>
                    <ext:RecordField Name="muhasebe">
                    </ext:RecordField>
                    <ext:RecordField Name="birim">
                    </ext:RecordField>
                    <ext:RecordField Name="borc" Type="Float">
                    </ext:RecordField>
                    <ext:RecordField Name="alacak" Type="Float">
                    </ext:RecordField>
                    <ext:RecordField Name="ilgili">
                    </ext:RecordField>
                    <ext:RecordField Name="yevmiyeTarih" Type="Date">
                    </ext:RecordField>
                    <ext:RecordField Name="durum">
                    </ext:RecordField>
                    <ext:RecordField Name="durumTarih" Type="Date">
                    </ext:RecordField>
                    <ext:RecordField Name="islemYapan">
                    </ext:RecordField>
                    <ext:RecordField Name="OEBNo">
                    </ext:RecordField>
                    <ext:RecordField Name="OEBTarih" Type="Date">
                    </ext:RecordField>
                    <ext:RecordField Name="cekNo">
                    </ext:RecordField>
                    <ext:RecordField Name="cekTarih" Type="Date">
                    </ext:RecordField>
                    <ext:RecordField Name="surecNo" Type="Date">
                    </ext:RecordField>
                </Fields>
            </ext:JsonReader>
        </Reader>
        <BaseParams>
            <ext:Parameter Name="start" Value="0" Mode="Raw" />
            <ext:Parameter Name="limit" Value="250" Mode="Raw" />
            <ext:Parameter Name="sort" Value="belgeTur" />
            <ext:Parameter Name="dir" Value="" />
            <ext:Parameter Name="groupBy" Value="belgeTur" />
        </BaseParams>
    </ext:Store>
    <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <West Split="true" UseSplitTips="true" CollapsibleSplitTip="Kapatmak için çift tıklayın"
                    ExpandableSplitTip="Açmak için çift tıklayın" MarginsSummary="5">
                    <ext:Panel ID="Panel3" runat="server" Collapsible="true" Title="Sorgu Alanları" Width="250"
                        AutoScroll="true">
                        <Items>
                            <ext:FitLayout ID="FitLayout" runat="server">
                                <Items>
                                    <ext:PropertyGrid ID="pgFiltre" runat="server" Border="false">
                                        <Source>
                                            <ext:PropertyGridParameter Name="yil" DisplayName="<%$ Resources:TasinirMal, FRMTIS013 %>">
                                                <Editor>
                                                    <ext:ComboBox ID="ddlYil" runat="server" Editable="false" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="durum" DisplayName="<%$ Resources:TasinirMal, FRMTIS014 %>">
                                                <Renderer Fn="durumRenderer" />
                                                <Editor>
                                                    <ext:ComboBox ID="ddlDurum" runat="server" Mode="Local" TriggerAction="All" ForceSelection="true"
                                                        Editable="false" StoreID="StoreDurum" DisplayField="Name" ValueField="ID" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="muhasebe" DisplayName="<%$ Resources:TasinirMal, FRMTIS015 %>">
                                                <Editor>
                                                    <ext:TriggerField ID="txtMuhasebe" runat="server" MaxLength="5">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" Qtip="<%$ Resources:TasinirMal, FRMTIS016 %>" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','');return false;" />
                                                            <%--lblMuhasebeAd--%>
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="birim" DisplayName="<%$ Resources:TasinirMal, FRMTIS017 %>">
                                                <Editor>
                                                    <ext:TriggerField ID="txtHarcamaBirimi" runat="server" MaxLength="15">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" Qtip="<%$ Resources:TasinirMal, FRMTIS018 %>" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','');return false;" />
                                                            <%--lblHarcamaBirimiAd--%>
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="ambar" DisplayName="<%$ Resources:TasinirMal, FRMTIS019 %>">
                                                <Editor>
                                                    <ext:TriggerField ID="txtAmbar" runat="server" MaxLength="3">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" Qtip="<%$ Resources:TasinirMal, FRMTIS020 %>" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="ListeAc('ListeAmbar.aspx','txtAmbar','txtHarcamaBirimi','');return false;" />
                                                            <%--lblAmbarAd--%>
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="birimDDL" DisplayName="<%$ Resources:TasinirMal, FRMORG001 %>">
                                                <Editor>
                                                    <ext:DropDownField ID="ddlBirim" runat="server" Width="300" TriggerIcon="Ellipsis"
                                                        Mode="ValueText">
                                                        <Component>
                                                            <ext:Panel ID="Panel1" runat="server" Width="400">
                                                                <Items>
                                                                    <ext:TreePanel ID="TreePanel5" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
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
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="belgeNo1" DisplayName="<%$ Resources:TasinirMal, FRMTIS021 %>" />
                                            <ext:PropertyGridParameter Name="belgeNo2" DisplayName="<%$ Resources:TasinirMal, FRMTIS022 %>" />
                                            <ext:PropertyGridParameter Name="belgeTarihi1" DisplayName="<%$ Resources:TasinirMal, FRMTIS023 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtBelgeTarih1" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="belgeTarihi2" DisplayName="<%$ Resources:TasinirMal, FRMTIS024 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtBelgeTarih2" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="durumTarih1" DisplayName="<%$ Resources:TasinirMal, FRMTIS025 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtDurumTarih1" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="durumTarih2" DisplayName="<%$ Resources:TasinirMal, FRMTIS026 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtDurumTarih2" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="nereye" DisplayName="<%$ Resources:TasinirMal, FRMTIS027 %>" />
                                            <ext:PropertyGridParameter Name="kime" DisplayName="<%$ Resources:TasinirMal, FRMTIS028 %>" />
                                            <ext:PropertyGridParameter Name="tasinirHesapKodu" DisplayName="<%$ Resources:TasinirMal, FRMTIS029 %>" />
                                            <ext:PropertyGridParameter Name="nereden" DisplayName="<%$ Resources:TasinirMal, FRMTIS030 %>" />
                                            <ext:PropertyGridParameter Name="islemTipi" DisplayName="<%$ Resources:TasinirMal, FRMTIS031 %>">
                                                <Renderer Fn="islemTipiRenderer" />
                                                <Editor>
                                                    <ext:ComboBox ID="ddlIslemTipi" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="islemYapan" DisplayName="<%$ Resources:TasinirMal, FRMTIS032 %>" />
                                            <ext:PropertyGridParameter Name="gonMuhasebe" DisplayName="<%$ Resources:TasinirMal, FRMTIS033 %>">
                                                <Editor>
                                                    <ext:TriggerField ID="txtGonMuhasebe" runat="server" MaxLength="5">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" Qtip="<%$ Resources:TasinirMal, FRMTIS034 %>" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="ListeAc('ListeMuhasebe.aspx','txtGonMuhasebe','','');return false;" />
                                                            <%--lblGonMuhasebeAd--%>
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="gonHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMTIS035 %>">
                                                <Editor>
                                                    <ext:TriggerField ID="txtGonHarcamaBirimi" runat="server" MaxLength="15">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" Qtip="<%$ Resources:TasinirMal, FRMTIS036 %>" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="ListeAc('ListeHarcamaBirimi.aspx','txtGonHarcamaBirimi','txtGonMuhasebe','');return false;" />
                                                            <%--lblGonHarcamaBirimiAd--%>
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="gonAmbar" DisplayName="<%$ Resources:TasinirMal, FRMTIS037 %>">
                                                <Editor>
                                                    <ext:TriggerField ID="txtGonAmbar" runat="server" MaxLength="3">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" Qtip="<%$ Resources:TasinirMal, FRMTIS038 %>" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="ListeAc('ListeAmbar.aspx','txtGonAmbar','txtGonHarcamaBirimi','');return false;" />
                                                            <%--lblGonAmbarAd--%>
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="birim2" DisplayName="<%$ Resources:TasinirMal, FRMORG004 %>">
                                                <Editor>
                                                    <ext:DropDownField ID="ddlBirim2" runat="server" Width="300" TriggerIcon="Ellipsis"
                                                        Mode="ValueText">
                                                        <Component>
                                                            <ext:Panel ID="Panel2" runat="server" Width="400">
                                                                <Items>
                                                                    <ext:TreePanel ID="TreePanel2" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                                        Border="false" RootVisible="false">
                                                                        <Root>
                                                                            <ext:AsyncTreeNode NodeID="0" Expanded="true" Icon="Note" />
                                                                        </Root>
                                                                        <Listeners>
                                                                            <BeforeLoad Fn="OrgBirimDoldur" />
                                                                            <Click Handler="#{ddlBirim2}.setValue(node.id, node.text, false);#{ddlBirim2}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtGonMuhasebe\', \'txtGonHarcamaBirimi\', \'txtGonAmbar\');');" />
                                                                        </Listeners>
                                                                    </ext:TreePanel>
                                                                </Items>
                                                                <Listeners>
                                                                    <Show Handler="try { TreePanel2.getRootNode().findChild('id', ddlBirim2.getValue(), true).select(); } catch (e) { }" />
                                                                </Listeners>
                                                            </ext:Panel>
                                                        </Component>
                                                    </ext:DropDownField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                        </Source>
                                        <Listeners>
                                            <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                                            <SortChange Handler="this.getStore().sortInfo = undefined;" />
                                        </Listeners>
                                        <View>
                                            <ext:GridView ID="GridView1" ForceFit="true" runat="server" />
                                        </View>
                                    </ext:PropertyGrid>
                                </Items>
                            </ext:FitLayout>
                        </Items>
                    </ext:Panel>
                </West>
                <Center MarginsSummary="5">
                    <ext:GridPanel ID="gvBelgeler" runat="server" StoreID="StoreListe" ForceFit="true"
                        StartCollapsed="false">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnListe" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS040 %>"
                                        Icon="TableGo">
                                        <DirectEvents>
                                            <Click OnEvent="btnListe_Click" Timeout="240000">
                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnListeYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS041%>"
                                        Icon="Printer">
                                        <DirectEvents>
                                            <Click Json="true" IsUpload="true" OnEvent="btnListeYazdir_Click" Timeout="240000">
                                                <EventMask MinDelay="200" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnIslemler" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS042%>"
                                        Icon="TableGear">
                                        <Menu>
                                            <ext:Menu ID="Menu2" runat="server">
                                                <Items>
                                                    <ext:MenuItem ID="btnIslemOnayla" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS044%>"
                                                        Icon="Accept">
                                                        <Listeners>
                                                            <%--IslemYap('Onay');return false;
                                                            <Click OnEvent="btnIslemOnayla_Click" Timeout="240000">
                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                            </Click>--%>
                                                        </Listeners>
                                                    </ext:MenuItem>
                                                    <ext:MenuItem ID="btnIslemOnayKaldir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS045%>"
                                                        Icon="Delete">
                                                        <Listeners>
                                                            <%--IslemYap('OnayKaldir');return false;
                                                            <Click OnEvent="btnIslemOnayKaldir_Click" Timeout="240000">                                                               
                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                            </Click>--%>
                                                        </Listeners>
                                                    </ext:MenuItem>
                                                    <ext:MenuItem ID="btnIslemYazdir" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS043%>"
                                                        Icon="Printer">
                                                        <Listeners>
                                                            <%--IslemYap('Yazdir');return false;
                                                                <Click Json="true" IsUpload="true" OnEvent="btnIslemYazdir_Click" Timeout="240000">
                                                                <EventMask MinDelay="200" />
                                                            </Click>--%>
                                                        </Listeners>
                                                    </ext:MenuItem>
                                                    <ext:MenuItem ID="btnIslemIptal" runat="server" Text="<%$ Resources:TasinirMal, FRMTIS046%>"
                                                        Icon="PageWhiteDelete">
                                                        <Listeners>
                                                            <%--IslemYap('İptal');return false;
                                                                <Click OnEvent="btnIptal_Click" Timeout="240000">
                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                            </Click>--%>
                                                        </Listeners>
                                                    </ext:MenuItem>
                                                </Items>
                                            </ext:Menu>
                                        </Menu>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <View>
                            <ext:GroupingView ID="GroupingView1" HideGroupedColumn="true" runat="server" ForceFit="true"
                                StartCollapsed="false" EnableRowBody="true">
                            </ext:GroupingView>
                        </View>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:RowNumbererColumn />
                                <%-- <ext:TemplateColumn ColumnID="belgeNo" DataIndex="belgeNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL084%>"
                                    Align="Center" Width="80" Groupable="false" Fixed="true" Hideable="false">
                                    <Template ID="Template1" runat="server">
                                        <Html>
                                            <a href="javascript:BelgeGirisAc('{belgeNo}')">{belgeNo}</a>
                                        </Html>
                                    </Template>
                                </ext:TemplateColumn>--%>,
                                <ext:Column DataIndex="YIL" Header="<%$ Resources:TasinirMal, FRMTIS013 %>" Width="40"
                                    Groupable="false" Fixed="true" />
                                <ext:DateColumn DataIndex="BELGETARIHI" Header="<%$ Resources:TasinirMal, FRMTIS023%>"
                                    Align="Center" Width="70" Fixed="true" Format="dd/m/Y" />
                                <ext:Column DataIndex="MUHASEBE" Header="<%$ Resources:TasinirMal, FRMTIS015%>" Width="100" />
                                <ext:Column DataIndex="HARCAMABIRIMI" Header="<%$ Resources:TasinirMal, FRMTIS017%>"
                                    Width="100" />
                                <ext:Column DataIndex="HARCAMABIRIMIADI" Header="<%$ Resources:TasinirMal, FRMTIS017%>"
                                    Width="100" />
                                <ext:Column DataIndex="AMBAR" Header="<%$ Resources:TasinirMal, FRMTIS019%>" Width="50" />
                                <ext:Column DataIndex="ISLEMTIPI" Header="<%$ Resources:TasinirMal, FRMTIS031%>"
                                    Width="50" />
                                <ext:Column DataIndex="DURUM" Header="<%$ Resources:TasinirMal, FRMTIS014%>" Width="50" />
                                <ext:DateColumn DataIndex="ENSONISLEMTARIHI" Header="En Son İşlem Tarihi" Align="Center"
                                    Width="50" Format="dd/m/Y" />
                                <ext:Column DataIndex="ENSONISLEMIYAPAN" Header="En Son İşlemi Yapan" Width="50" />
                                <%--<ext:TemplateColumn DataIndex="durum" Header="<%$ Resources:ButceMuhasebe, MUHGNL092%>"
                                    Width="70" Fixed="true">
                                    <Template ID="Template2" runat="server">
                                        <Html>
                                            <a href="javascript:TarihceGoster('{belgeNo}')">{durum}</a>
                                        </Html>
                                    </Template>
                                </ext:TemplateColumn>
                                <ext:Column DataIndex="cekNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL272%>" Hidden="true"
                                    Groupable="false" Width="70">
                                </ext:Column>
                                <ext:DateColumn DataIndex="cekTarih" Header="<%$ Resources:ButceMuhasebe, MUHGNL273%>"
                                    Align="Center" Hidden="true" Width="70" Format="dd/m/Y">
                                </ext:DateColumn>
                                <ext:Column DataIndex="surecNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL097%>"
                                    Hidden="true" Groupable="false">
                                </ext:Column>--%>
                            </Columns>
                        </ColumnModel>
                        <LoadMask ShowMask="true" Msg="Lütfen bekleyin..." />
                        <SelectionModel>
                            <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" CheckOnly="true" runat="server" />
                        </SelectionModel>
                        <Plugins>
                            <ext:GroupingSummary ID="GroupingSummary1" runat="server" ForceFit="true">
                            </ext:GroupingSummary>
                            <ext:RowExpander ID="RowExpander1" runat="server" EnableCaching="false">
                                <Template ID="Template3" runat="server">
                                    <Html>
                                        <div id="row-{belgeNo}" style="background-color: White;"></div>
                                    </Html>
                                </Template>
                                <DirectEvents>
                                    <%-- <BeforeExpand OnEvent="BeforeExpand" Before="return !body.rendered;" Success="body.rendered=true;">
                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="={grdListe.body}" />
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.id" Mode="Raw" />
                                        </ExtraParams>
                                    </BeforeExpand>--%>
                                </DirectEvents>
                            </ext:RowExpander>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
                                StoreID="StoreListe">
                                <Items>
                                    <ext:Label ID="Label1" runat="server" Text="Sayfada göstericek satır sayısı:" />
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
    <%--   
            <asp:GridView>
                <Columns>
                    <asp:TemplateField ItemStyle-Width="30px" HeaderText="<input type='checkbox' onclick='javascript:ListeSec();' />">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSecim" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="fisno" Visible="false" HeaderText="BelgeNoGizli"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$ Resources:TasinirMal, FRMTIS047 %>">
                        <ItemStyle Width="50px" BorderColor="LightGray" HorizontalAlign="Left" />
                        <ItemTemplate>
                            <a href="javascript:BelgeAc('<%# DataBinder.Eval(Container.DataItem, "yil")%>','<%# DataBinder.Eval(Container.DataItem, "muhasebe")%>','<%# DataBinder.Eval(Container.DataItem, "harcamaBirimi")%>','<%# DataBinder.Eval(Container.DataItem, "fisno")%>');">
                                <%# DataBinder.Eval(Container.DataItem, "fisno")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="yil" HeaderText="<%$ Resources:TasinirMal, FRMTIS013 %>"
                        ItemStyle-Width="40px" />
                    <asp:BoundField DataField="fistarih" HeaderText="<%$ Resources:TasinirMal, FRMTIS048 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="muhasebe" HeaderText="<%$ Resources:TasinirMal, FRMTIS049 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="harcamaBirimi" HeaderText="<%$ Resources:TasinirMal, FRMTIS050 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="harcamaBirimiAd" HeaderText="<%$ Resources:TasinirMal, FRMTIS051 %>"
                        ItemStyle-Width="180px" />
                    <asp:BoundField DataField="ambar" HeaderText="<%$ Resources:TasinirMal, FRMTIS052 %>"
                        ItemStyle-Width="120px" />
                    <asp:BoundField DataField="islemtipi" HeaderText="<%$ Resources:TasinirMal, FRMTIS031 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="durum" Visible="false" HeaderText="DurumGizli"></asp:BoundField>
                    <asp:TemplateField HeaderText="Durum">
                        <ItemStyle Width="80px" BorderColor="LightGray" />
                        <ItemTemplate>
                            <a href="javascript:TarihceGoster('<%# DataBinder.Eval(Container.DataItem, "yil")%>','<%# DataBinder.Eval(Container.DataItem, "muhasebe")%>','<%# DataBinder.Eval(Container.DataItem, "harcamaBirimi")%>','<%# DataBinder.Eval(Container.DataItem, "fisno")%>');">
                                <%# DataBinder.Eval(Container.DataItem, "durum")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="islemTarih" HeaderText="<%$ Resources:TasinirMal, FRMTIS053 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="islemYapan" HeaderText="<%$ Resources:TasinirMal, FRMTIS054 %>"
                        ItemStyle-Width="80px" />
                </Columns>
            </asp:GridView>
     
            <%= Resources.TasinirMal.FRMTIS055 %>
        <img alt="<%= Resources.TasinirMal.FRMTIS056 %>" src="../App_themes/images/loading.gif" />
       
    <iframe id="frmIslem" src="TasinirIslemSorguIslem.aspx?kutuphane=<%=Request["kutuphane"]+""%>&muze=<%=Request["muze"]+""%>"
        frameborder="0" scrolling="no" width="1" height="1"></iframe>
    --%>
</body>
</html>
