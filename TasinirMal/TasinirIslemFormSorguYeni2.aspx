<%@ Page Language="C#" CodeBehind="TasinirIslemFormSorguYeni2.aspx.cs" Inherits="TasinirMal.TasinirIslemFormSorguYeni2" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirIslemSorguYeni2.js?mc=03022015"></script>
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
        var durumRenderer = function (value) {
            var r = durumStore.getById(value);

            if (Ext1.isEmpty(r)) {
                return "";
            }

            return r.data.Name;
        };
        var belgeTurRenderer = function (value) {
            var r = belgeTurStore.getById(value);

            if (Ext1.isEmpty(r)) {
                return "";
            }

            return r.data.Name;
        };
    </script>
</head>
<body>
    <form id="form" runat="server">
    <input type="hidden" id="hdnMuhasebeNo" runat="server" />
    <input type="hidden" id="hdnBirimNo" runat="server" />
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Store ID="durumStore" runat="server">
        <Reader>
            <ext:ArrayReader IDProperty="ID">
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="Name" />
                </Fields>
            </ext:ArrayReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="belgeTurStore" runat="server">
        <Reader>
            <ext:ArrayReader IDProperty="ID">
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="Name" />
                </Fields>
            </ext:ArrayReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreHesapPlan" runat="server" AutoLoad="false" OnRefreshData="HesapStore_Refresh">
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
        RemotePaging="true" RemoteSort="true" RemoteGroup="true" OnRefreshData="StoreListe_Refresh"
        GroupField="belgeTur">
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
                                            <ext:PropertyGridParameter Name="yil" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL036 %>">
                                                <Editor>
                                                    <ext:ComboBox ID="ddlYil" runat="server" Editable="false">
                                                    </ext:ComboBox>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="durum" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES010 %>">
                                                <Renderer Fn="durumRenderer" />
                                                <Editor>
                                                    <ext:ComboBox ID="ddlDurum" runat="server" Mode="Local" TriggerAction="All" ForceSelection="true"
                                                        StoreID="durumStore" DisplayField="Name" ValueField="ID" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="belgeTur" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES012 %>">
                                                <Renderer Fn="belgeTurRenderer" />
                                                <Editor>
                                                    <ext:ComboBox ID="ddlBelgeTur" runat="server" Mode="Local" TriggerAction="All" ForceSelection="true"
                                                        StoreID="belgeTurStore" DisplayField="Name" ValueField="ID" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="muhasebe" DisplayName="Muhasebe">
                                                <Editor>
                                                    <ext:TriggerField ID="txtMuhasebe" runat="server">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="muhasebeGetirHandler(this)" />
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="birim" DisplayName="Harcama Birimi">
                                                <Editor>
                                                    <ext:TriggerField ID="txtBirim" runat="server">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Search" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="muhasebeGetirHandler(this)" />
                                                        </Listeners>
                                                    </ext:TriggerField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="yevmiyeNo1" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES015 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtYevmiyeNo1" runat="server">
                                                    </ext:TextField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="yevmiyeNo2" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES015 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtYevmiyeNo2" runat="server">
                                                    </ext:TextField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="belgeNo1" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES016 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtBelgeNo1" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="belgeNo2" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES016 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtBelgeNo2" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="belgeTarihi1" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES017 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtBelgeTarih1" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="belgeTarihi2" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES017 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtBelgeTarih2" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="hesapNo" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL176 %>">
                                                <Editor>
                                                    <ext:ComboBox ID="ComboBox1" runat="server" DisplayField="KOD" ValueField="KOD" TypeAhead="false"
                                                        ForceSelection="false" LoadingText="Lütfen bekleyin..." PageSize="20" HideTrigger="true"
                                                        ItemSelector="div.search-item" MinChars="3" SelectOnFocus="true" StoreID="StoreHesapPlan"
                                                        Resizable="true" ListWidth="300">
                                                        <Template ID="Template4" runat="server">
                                                            <Html>
                                                                <tpl for=".">
                                                                    <div class="search-item">
                                                                        <span>{KOD}</span>
                                                                    </div>
                                                                </tpl>
                                                            </Html>
                                                        </Template>
                                                    </ext:ComboBox>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="althesapNo" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL285 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtAltHesapNo" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="ilgili" DisplayName="<%$ Resources:ButceMuhasebe, FRMOEM027 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtIlgiliAd" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="ilgiliNo" DisplayName="<%$ Resources:ButceMuhasebe, FRMOEM029 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtIlgiliNo" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="kur" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL137%>">
                                                <Editor>
                                                    <ext:DropDownField ID="DropDownField0" runat="server" Width="300" TriggerIcon="SimpleArrowDown">
                                                        <Component>
                                                            <ext:Panel ID="Panel2" runat="server" Width="400">
                                                                <Items>
                                                                    <ext:TreePanel ID="TreePanel1" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                                        Border="false">
                                                                        <Root>
                                                                            <ext:AsyncTreeNode NodeID="-1" Text="<%$ Resources:ButceMuhasebe, MUHGNL288%>" Expanded="true"
                                                                                Icon="Note" />
                                                                        </Root>
                                                                        <Listeners>
                                                                            <BeforeLoad Handler="nodeLoadPar('1', #{ddlYil}.getValue(), #{DropDownField0}.getValue(), node, true)" />
                                                                            <Click Handler="var tut=node.id; if(tut=='-1') tut=''; #{DropDownField0}.setValue(tut, node.text, false);#{DropDownField0}.collapse();" />
                                                                        </Listeners>
                                                                    </ext:TreePanel>
                                                                </Items>
                                                            </ext:Panel>
                                                        </Component>
                                                        <Listeners>
                                                            <Blur Handler="var tut = #{DropDownField0}.getText(); #{DropDownField0}.setValue(tut, tut, false);" />
                                                        </Listeners>
                                                    </ext:DropDownField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="fon" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL177%>">
                                                <Editor>
                                                    <ext:DropDownField ID="DropDownField1" runat="server" Editable="true" Width="300"
                                                        TriggerIcon="SimpleArrowDown">
                                                        <Component>
                                                            <ext:Panel ID="Panel4" runat="server" Width="400">
                                                                <Items>
                                                                    <ext:TreePanel ID="TreePanel2" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                                        Border="false">
                                                                        <Root>
                                                                            <ext:AsyncTreeNode NodeID="-1" Text="<%$ Resources:ButceMuhasebe, MUHGNL288%>" Expanded="true"
                                                                                Icon="Note" />
                                                                        </Root>
                                                                        <Listeners>
                                                                            <BeforeLoad Handler="nodeLoadPar('2', #{ddlYil}.getValue(), #{DropDownField1}.getValue(), node, true)" />
                                                                            <Click Handler="var tut=node.id; if(tut=='-1') tut=''; #{DropDownField1}.setValue(tut, node.text, false);#{DropDownField1}.collapse();" />
                                                                        </Listeners>
                                                                    </ext:TreePanel>
                                                                </Items>
                                                            </ext:Panel>
                                                        </Component>
                                                        <Listeners>
                                                            <Blur Handler="var tut = #{DropDownField1}.getText(); #{DropDownField1}.setValue(tut, tut, false);" />
                                                        </Listeners>
                                                    </ext:DropDownField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="fin" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL178%>">
                                                <Editor>
                                                    <ext:DropDownField ID="DropDownField2" runat="server" Editable="true" Width="300"
                                                        TriggerIcon="SimpleArrowDown">
                                                        <Component>
                                                            <ext:Panel ID="Panel5" runat="server" Width="400">
                                                                <Items>
                                                                    <ext:TreePanel ID="TreePanel3" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                                        Border="false">
                                                                        <Root>
                                                                            <ext:AsyncTreeNode NodeID="-1" Text="<%$ Resources:ButceMuhasebe, MUHGNL288%>" Expanded="true"
                                                                                Icon="Note" />
                                                                        </Root>
                                                                        <Listeners>
                                                                            <BeforeLoad Handler="nodeLoadPar('3', #{ddlYil}.getValue(), #{DropDownField2}.getValue(), node, true)" />
                                                                            <Click Handler="var tut=node.id; if(tut=='-1') tut=''; #{DropDownField2}.setValue(tut, node.text, false);#{DropDownField2}.collapse();" />
                                                                        </Listeners>
                                                                    </ext:TreePanel>
                                                                </Items>
                                                            </ext:Panel>
                                                        </Component>
                                                        <Listeners>
                                                            <Blur Handler="var tut = #{DropDownField2}.getText(); #{DropDownField2}.setValue(tut, tut, false);" />
                                                        </Listeners>
                                                    </ext:DropDownField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="eko" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL179%>">
                                                <Editor>
                                                    <ext:DropDownField ID="DropDownField3" runat="server" Editable="true" Width="300"
                                                        TriggerIcon="SimpleArrowDown">
                                                        <Component>
                                                            <ext:Panel ID="Panel6" runat="server" Width="400">
                                                                <Items>
                                                                    <ext:TreePanel ID="TreePanel4" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                                        Border="false">
                                                                        <Root>
                                                                            <ext:AsyncTreeNode NodeID="-1" Text="<%$ Resources:ButceMuhasebe, MUHGNL288%>" Expanded="true"
                                                                                Icon="Note" />
                                                                        </Root>
                                                                        <Listeners>
                                                                            <BeforeLoad Handler="nodeLoadPar('4', #{ddlYil}.getValue(), #{DropDownField3}.getValue(), node, true)" />
                                                                            <Click Handler="var tut=node.id; if(tut=='-1') tut=''; #{DropDownField3}.setValue(tut, node.text, false);#{DropDownField3}.collapse();" />
                                                                        </Listeners>
                                                                    </ext:TreePanel>
                                                                </Items>
                                                            </ext:Panel>
                                                        </Component>
                                                        <Listeners>
                                                            <Blur Handler="var tut = #{DropDownField3}.getText(); #{DropDownField3}.setValue(tut, tut, false);" />
                                                        </Listeners>
                                                    </ext:DropDownField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="peb" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL287%>">
                                                <Editor>
                                                    <ext:DropDownField ID="DropDownField5" runat="server" Editable="true" Width="300"
                                                        TriggerIcon="SimpleArrowDown">
                                                        <Component>
                                                            <ext:Panel ID="Panel8" runat="server" Width="400">
                                                                <Items>
                                                                    <ext:TreePanel ID="TreePanel6" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                                                        Border="false">
                                                                        <Root>
                                                                            <ext:AsyncTreeNode NodeID="-1" Text="<%$ Resources:ButceMuhasebe, MUHGNL288%>" Expanded="true"
                                                                                Icon="Note" />
                                                                        </Root>
                                                                        <Listeners>
                                                                            <BeforeLoad Handler="nodeLoadPar('14', #{ddlYil}.getValue(), #{DropDownField5}.getValue(), node, true)" />
                                                                            <Click Handler="var tut=node.id; if(tut=='-1') tut=''; #{DropDownField5}.setValue(tut, node.text, false);#{DropDownField5}.collapse();" />
                                                                        </Listeners>
                                                                    </ext:TreePanel>
                                                                </Items>
                                                            </ext:Panel>
                                                        </Component>
                                                        <Listeners>
                                                            <Blur Handler="var tut = #{DropDownField5}.getText(); #{DropDownField5}.setValue(tut, tut, false);" />
                                                        </Listeners>
                                                    </ext:DropDownField>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="yevmiyeTarihi1" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES018 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtYevmiyeTarih1" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="yevmiyeTarihi2" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES018 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtYevmiyeTarih2" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="durumTarihi1" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES019 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtDurumTarih1" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="durumTarihi2" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES019 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtDurumTarih2" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="borc" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES021 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtBorc" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="alacak" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES022 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtAlacak" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="fisTur" DisplayName="<%$ Resources:ButceMuhasebe, FRMOEM057 %>">
                                                <Editor>
                                                    <ext:ComboBox ID="ddlFisTur" runat="server" Editable="false">
                                                        <Items>
                                                            <ext:ListItem Text="<%$ Resources:ButceMuhasebe, FRMOEM058 %>" Value="0" />
                                                            <ext:ListItem Text="<%$ Resources:ButceMuhasebe, FRMOEM059 %>" Value="1" />
                                                            <ext:ListItem Text="<%$ Resources:ButceMuhasebe, FRMOEM060 %>" Value="2" />
                                                        </Items>
                                                    </ext:ComboBox>
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="islemYapan" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES023 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtIslemYapan" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="aciklama" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL155 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtAciklama" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="cekNoSorgu" DisplayName="<%$ Resources:ButceMuhasebe, MUHGNL272 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtCekNoSorgu" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="OdEsTur" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES024 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtOdEsBeTur" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="OdEsNo" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES026 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtOdEsBeNo" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="OdEsTarihi" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES027 %>">
                                                <Editor>
                                                    <ext:DateField ID="txtOdEsTarih" runat="server" />
                                                </Editor>
                                            </ext:PropertyGridParameter>
                                            <ext:PropertyGridParameter Name="yatirimProjeNo" DisplayName="<%$ Resources:ButceMuhasebe, FRMOES025 %>">
                                                <Editor>
                                                    <ext:TextField ID="txtYatirimProjeNo" runat="server" />
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
                    <ext:GridPanel ID="grdListe" runat="server" StoreID="storeListe" ForceFit="true"
                        StartCollapsed="false">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnListe" runat="server" Text="<%$ Resources:ButceMuhasebe, MUHGNL008 %>"
                                        Icon="TableGo">
                                        <DirectEvents>
                                            <Click OnEvent="btnListe_Click" Timeout="240000">
                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnListeYazdir" runat="server" Text="<%$ Resources:ButceMuhasebe, MUHGNL009%>"
                                        Icon="Printer">
                                        <DirectEvents>
                                            <Click Json="true" IsUpload="true" OnEvent="btnListeYazdir_Click" Timeout="240000">
                                                <EventMask MinDelay="200" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnIslemler" runat="server" Text="<%$ Resources:ButceMuhasebe, MUHGNL251%>"
                                        Icon="TableGear">
                                        <Menu>
                                            <ext:Menu ID="Menu2" runat="server">
                                                <Items>
                                                    <ext:MenuItem ID="btnOnayla" runat="server" Text="Onayla" Icon="Accept">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnOnayla_Click" Timeout="240000">
                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:MenuItem>
                                                    <ext:MenuItem ID="btnOnayKaldir" runat="server" Text="Onay Kaldır" Icon="Delete">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnOnayKaldir_Click" Timeout="240000">
                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:MenuItem>
                                                    <ext:MenuItem ID="btnBelgeYazdir" runat="server" Text="<%$ Resources:ButceMuhasebe, MUHGNL116%>"
                                                        Icon="Printer">
                                                        <DirectEvents>
                                                            <Click Json="true" IsUpload="true" OnEvent="btnBelgeYazdir_Click" Timeout="240000">
                                                                <EventMask MinDelay="200" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:MenuItem>
                                                    <ext:MenuItem ID="btnIslemIptal" runat="server" Text="<%$ Resources:ButceMuhasebe, MUHGNL117%>"
                                                        Icon="PageWhiteDelete">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnIptal_Click" Timeout="240000">
                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:MenuItem>
                                                </Items>
                                            </ext:Menu>
                                        </Menu>
                                    </ext:Button>
                                    <ext:ToolbarSeparator ID="ToolbarSeparator1" runat="server" />
                                    <ext:Button ID="btnYevmiye" runat="server" Text="Yevmiye İşlemleri" Icon="TableEdit">
                                        <Menu>
                                            <ext:Menu ID="menuYevmiye" runat="server">
                                                <Items>
                                                    <ext:FormPanel ID="FormPanel1" runat="server" LabelAlign="Top" Width="250" Height="80"
                                                        Padding="5" Layout="Form">
                                                        <Items>
                                                            <ext:TextField ID="txtIslemYevmiyeNo" runat="server" FieldLabel="Yevmiye No" AnchorHorizontal="100%" />
                                                        </Items>
                                                        <BottomBar>
                                                            <ext:Toolbar ID="Toolbar2" runat="server">
                                                                <Items>
                                                                    <ext:Button ID="btnYevmiyeVer" runat="server" Text="Yevmiye No Ver" Icon="PageWhiteAdd">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnYevmiyeVer_Click" Timeout="240000">
                                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="yevmiyeNo" Mode="Raw" Value="#{txtIslemYevmiyeNo}.getValue()">
                                                                                    </ext:Parameter>
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnYevmiyeNoKaldir" runat="server" Text="Yevmiye No Kaldır" Icon="PageWhiteDelete">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnYevmiyeNoKaldir_Click" Timeout="240000">
                                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </BottomBar>
                                                    </ext:FormPanel>
                                                </Items>
                                            </ext:Menu>
                                        </Menu>
                                    </ext:Button>
                                    <ext:Button ID="btnCek" runat="server" Text="Çek İşlemleri" Icon="TableEdit">
                                        <Menu>
                                            <ext:Menu ID="menuCek" runat="server">
                                                <Items>
                                                    <ext:FormPanel ID="FormPanel2" runat="server" LabelAlign="Top" Width="250" Height="80"
                                                        Padding="5" Layout="Form">
                                                        <Items>
                                                            <ext:TextField ID="txtIslemCekNo" runat="server" FieldLabel="Çek No / Gönderme Emri No"
                                                                AnchorHorizontal="100%" />
                                                        </Items>
                                                        <BottomBar>
                                                            <ext:Toolbar ID="Toolbar3" runat="server">
                                                                <Items>
                                                                    <ext:Button ID="btnCekNoVer" runat="server" Text="No Güncelle" Icon="PageWhiteAdd">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnCekNoVer_Click" Timeout="240000">
                                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="cekNo" Mode="Raw" Value="#{txtIslemCekNo}.getValue()">
                                                                                    </ext:Parameter>
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnCekNoKaldir" runat="server" Text="No Kaldır" Icon="PageWhiteDelete">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnCekNoKaldir_Click" Timeout="240000">
                                                                                <Confirmation ConfirmRequest="true" Title="İşlem Onayı" Message="İşlem yapılacak. Onaylıyor musunuz?" />
                                                                                <EventMask ShowMask="true" MinDelay="200" Msg="Lütfen bekleyin..." />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnCekNoYazdir" runat="server" Text="Yazdır" Icon="Printer">
                                                                        <DirectEvents>
                                                                            <Click Json="true" IsUpload="true" OnEvent="btnCekNoYazdir_Click" Timeout="240000">
                                                                                <EventMask MinDelay="200" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="cekNo" Mode="Raw" Value="#{txtIslemCekNo}.getValue()">
                                                                                    </ext:Parameter>
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </BottomBar>
                                                    </ext:FormPanel>
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
                                <ext:TemplateColumn ColumnID="belgeNo" DataIndex="belgeNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL084%>"
                                    Align="Center" Width="80" Groupable="false" Fixed="true" Hideable="false">
                                    <Template ID="Template1" runat="server">
                                        <Html>
                                            <a href="javascript:BelgeGirisAc('{belgeNo}')">{belgeNo}</a>
                                        </Html>
                                    </Template>
                                </ext:TemplateColumn>
                                <ext:Column DataIndex="yevmiyeNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL064%>"
                                    Width="80" Groupable="false" Fixed="true">
                                </ext:Column>
                                <ext:Column DataIndex="belgeTur" Header="<%$ Resources:ButceMuhasebe, MUHGNL085%>"
                                    Width="30" Hidden="true">
                                </ext:Column>
                                <ext:DateColumn DataIndex="belgeTarih" Header="<%$ Resources:ButceMuhasebe, MUHGNL086%>"
                                    Align="Center" Width="70" Fixed="true" Format="dd/m/Y">
                                </ext:DateColumn>
                                <ext:Column DataIndex="muhasebe" Header="<%$ Resources:ButceMuhasebe, MUHGNL087%>"
                                    Width="100">
                                </ext:Column>
                                <ext:Column DataIndex="birim" Header="<%$ Resources:ButceMuhasebe, MUHGNL088%>" Width="100">
                                </ext:Column>
                                <ext:GroupingSummaryColumn DataIndex="borc" Header="<%$ Resources:ButceMuhasebe, MUHGNL081%>"
                                    Align="Right" Width="100" Groupable="false" SummaryType="Sum" Fixed="true">
                                    <Renderer Format="Number" FormatArgs="'0,0.0/i'" />
                                </ext:GroupingSummaryColumn>
                                <ext:GroupingSummaryColumn DataIndex="alacak" Header="<%$ Resources:ButceMuhasebe, MUHGNL082%>"
                                    Align="Right" Width="100" Groupable="false" SummaryType="Sum" Fixed="true">
                                    <Renderer Format="Number" FormatArgs="'0,0.0/i'" />
                                </ext:GroupingSummaryColumn>
                                <ext:Column DataIndex="ilgili" Header="<%$ Resources:ButceMuhasebe, MUHGNL089%>"
                                    Align="Left" Width="50">
                                </ext:Column>
                                <ext:DateColumn DataIndex="yevmiyeTarih" Header="<%$ Resources:ButceMuhasebe, MUHGNL090%>"
                                    Align="Center" Width="50" Hidden="true" Format="dd/m/Y">
                                </ext:DateColumn>
                                <ext:TemplateColumn DataIndex="durum" Header="<%$ Resources:ButceMuhasebe, MUHGNL092%>"
                                    Width="70" Fixed="true">
                                    <Template ID="Template2" runat="server">
                                        <Html>
                                            <a href="javascript:TarihceGoster('{belgeNo}')">{durum}</a>
                                        </Html>
                                    </Template>
                                </ext:TemplateColumn>
                                <ext:DateColumn DataIndex="durumTarih" Header="<%$ Resources:ButceMuhasebe, MUHGNL093%>"
                                    Align="Center" Hidden="true" Width="50" Format="dd/m/Y">
                                </ext:DateColumn>
                                <ext:Column DataIndex="islemYapan" Header="<%$ Resources:ButceMuhasebe, MUHGNL094%>"
                                    Hidden="true">
                                </ext:Column>
                                <ext:Column DataIndex="OEBNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL095%>" Hidden="true"
                                    Groupable="false">
                                </ext:Column>
                                <ext:DateColumn DataIndex="OEBTarih" Header="<%$ Resources:ButceMuhasebe, MUHGNL096%>"
                                    Align="Center" Hidden="true" Width="50" Format="dd/m/Y">
                                </ext:DateColumn>
                                <ext:Column DataIndex="cekNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL272%>" Hidden="true"
                                    Groupable="false" Width="70">
                                </ext:Column>
                                <ext:DateColumn DataIndex="cekTarih" Header="<%$ Resources:ButceMuhasebe, MUHGNL273%>"
                                    Align="Center" Hidden="true" Width="70" Format="dd/m/Y">
                                </ext:DateColumn>
                                <ext:Column DataIndex="surecNo" Header="<%$ Resources:ButceMuhasebe, MUHGNL097%>"
                                    Hidden="true" Groupable="false">
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <LoadMask ShowMask="true" Msg="Lütfen bekleyin..." />
                        <SelectionModel>
                            <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" CheckOnly="true" runat="server">
                            </ext:CheckboxSelectionModel>
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
                                    <BeforeExpand OnEvent="BeforeExpand" Before="return !body.rendered;" Success="body.rendered=true;">
                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="={grdListe.body}" />
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.id" Mode="Raw" />
                                        </ExtraParams>
                                    </BeforeExpand>
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
</body>
</html>
