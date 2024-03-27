<%@ Page Language="C#" CodeBehind="AktarimSorgu.aspx.cs" Inherits="TasinirMal.AktarimSorgu" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function BelgeAc(kod) {
            parent.tabPanelAna.setActiveTab("panelIslem");
            parent.panelIslem.getBody().BelgeAc(kod);
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="MUHASEBEADI" />
                        <ext:RecordField Name="HARCAMABIRIMKOD" />
                        <ext:RecordField Name="HARCAMABIRIMADI" />
                        <ext:RecordField Name="AMBARKOD" />
                        <ext:RecordField Name="AMBARADI" />
                        <ext:RecordField Name="ISLEMTARIH" Type="Date" />
                        <ext:RecordField Name="BELGETARIH" Type="Date" />
                        <ext:RecordField Name="ISLEMTIPIAD" />
                        <ext:RecordField Name="BELGETURAD" />
                        <ext:RecordField Name="ISLEMYAPAN" />
                        <ext:RecordField Name="DURUM" />
                        <ext:RecordField Name="TOPLAMKAYIT" />
                        <ext:RecordField Name="ISLENENKAYIT" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strDurum" runat="server">
            <Reader>
                <ext:JsonReader runat="server" IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" ForceFit="true" Collapsible="true"
                    Width="250" Margins="5 0 5 5" Split="true" AutoRender="false" Header="false">
                    <TopBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button ID="btnSorguTemizle" runat="server" Text="Temizle" Icon="PageWhite" Hidden="true">
                                    <DirectEvents>
                                        <Click OnEvent="btnSorguTemizle_Click">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="btnListele" runat="server" Text="<%$ Resources:TasinirMal,FRMTIM033 %>"
                                    Icon="ApplicationGo">
                                    <DirectEvents>
                                        <Click OnEvent="btnListele_Click">
                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Source>
                        <ext:PropertyGridParameter Name="prpDurum" DisplayName="Durum">
                            <Renderer Handler="return PropertyRenderer(strDurum,value);" />
                            <Editor>
                                <ext:ComboBox ID="ddlDurum" runat="server" ForceSelection="true" StoreID="strDurum"
                                    ValueField="KOD" DisplayField="ADI" QueryMode="Local">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Clear" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Fn="TriggerClick" />
                                    </Listeners>
                                </ext:ComboBox>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpMuhasebe" DisplayName="<%$ Resources:TasinirMal, FRMBRK006 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpMuhasebe',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMBRK008 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpHarcamaBirimi',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
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
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false" TrackMouseOver="true"
                    Border="true" StoreID="strListe" Margins="5 5 5 0" Split="true" Cls="gridExt">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:Button ID="btnListeYazdir" runat="server" Text="Liste Raporla" Icon="PageExcel" Hidden="true">
                                    <DirectEvents>
                                        <Click OnEvent="btnListeYazdir_Click" IsUpload="true" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill />
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:TemplateColumn DataIndex="KOD" Header="Belge No" Width="55">
                                <Template runat="server">
                                    <Html>
                                        <a href="javascript:BelgeAc('{KOD}')">{KOD}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <ext:DateColumn runat="server" Header="Belge Tarihi" DataIndex="BELGETARIH" Format="dd.m.Y"
                                Width="75" />
                            <ext:Column Header="Muhasebe" DataIndex="MUHASEBEADI" Width="200" />
                            <ext:Column Header="Harcama Birimi" DataIndex="HARCAMABIRIMADI" Width="200" />
                            <ext:DateColumn runat="server" Header="İşlem Tarihi" DataIndex="ISLEMTARIH" Format="dd.m.Y"
                                Width="75" />
                            <ext:Column Header="Toplam Kayıt" DataIndex="TOPLAMKAYIT" Align="Right" />
                            <ext:Column Header="İşlenen Kayıt" DataIndex="ISLENENKAYIT" Align="Right" />
                            <ext:Column Header="Belge Türü" DataIndex="BELGETURAD" />
                            <ext:Column Header="İşlem Tipi" DataIndex="ISLEMTIPIAD" />
                            <ext:Column Header="İşlemi Yapan" DataIndex="ISLEMYAPAN" />
                            <ext:Column Header="Durum" DataIndex="DURUM" />
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
    </form>
</body>
</html>
