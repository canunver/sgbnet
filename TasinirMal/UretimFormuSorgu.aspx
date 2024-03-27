<%@ Page Language="C#" CodeBehind="UretimFormuSorgu.aspx.cs" Inherits="TasinirMal.UretimFormuSorgu" %>

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
                        <ext:RecordField Name="GIRENMUHASEBEKOD" />
                        <ext:RecordField Name="GIRENMUHASEBEADI" />
                        <ext:RecordField Name="GIRENHARCAMABIRIMKOD" />
                        <ext:RecordField Name="GIRENHARCAMABIRIMADI" />
                        <ext:RecordField Name="GIRENAMBARKOD" />
                        <ext:RecordField Name="GIRENAMBARADI" />
                        <ext:RecordField Name="CIKANMUHASEBEKOD" />
                        <ext:RecordField Name="CIKANMUHASEBEADI" />
                        <ext:RecordField Name="CIKANHARCAMABIRIMKOD" />
                        <ext:RecordField Name="CIKANHARCAMABIRIMADI" />
                        <ext:RecordField Name="CIKANAMBARKOD" />
                        <ext:RecordField Name="CIKANAMBARADI" />
                        <ext:RecordField Name="FISNO" />
                        <ext:RecordField Name="FISTARIHI" Type="Date" />
                        <ext:RecordField Name="DURUM" />
                        <ext:RecordField Name="ANAURUNHESAPKOD" />
                        <ext:RecordField Name="ANAURUNHESAPAD" />
                        <ext:RecordField Name="ISLEMYAPAN" />
                        <ext:RecordField Name="GIRENTIFFISNO" />
                        <ext:RecordField Name="CIKANTIFFISNO" />
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
                                <ext:Button ID="btnSorguTemizle" runat="server" Text="Temizle" Icon="PageWhite">
                                    <DirectEvents>
                                        <Click OnEvent="btnSorguTemizle_Click">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="btnListele" runat="server" Text="<%$ Resources:TasinirMal,FRMTIM033 %>" Icon="ApplicationGo">
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
                        <ext:PropertyGridParameter Name="prpYil" DisplayName="<%$ Resources:TasinirMal, FRMBRK005 %>">
                            <Editor>
                                <ext:SpinnerField runat="server">
                                </ext:SpinnerField>
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
                        <ext:PropertyGridParameter Name="prpAmbar" DisplayName="<%$ Resources:TasinirMal, FRMTIS019 %>">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpAmbar',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                       
                        <ext:PropertyGridParameter Name="prpBelgeNo1" DisplayName="<%$ Resources:TasinirMal, FRMZFS031 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeNo2" DisplayName="<%$ Resources:TasinirMal, FRMZFS032 %>">
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTarihi1" DisplayName="<%$ Resources:TasinirMal, FRMZFS033 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpBelgeTarihi2" DisplayName="<%$ Resources:TasinirMal, FRMZFS034 %>">
                            <Renderer Fn="TarihRenderer" />
                            <Editor>
                                <ext:DateField runat="server" Format="dd.m.Y" />
                            </Editor>
                        </ext:PropertyGridParameter>
                        <ext:PropertyGridParameter Name="prpHesapKod" DisplayName="Hesap Kodu">
                            <Editor>
                                <ext:TriggerField runat="server">
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Search" />
                                    </Triggers>
                                    <Listeners>
                                        <TriggerClick Handler="TriggerClickProperty('prpHesapKod',this.triggers[0]);" />
                                    </Listeners>
                                </ext:TriggerField>
                            </Editor>
                        </ext:PropertyGridParameter>
                        
                        <ext:PropertyGridParameter Name="prpSonislemyapan" DisplayName="<%$ Resources:TasinirMal, FRMGAS016 %>">
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
                                <ext:Button ID="btnListeYazdir" runat="server" Text="Liste Raporla" Icon="PageExcel">
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
                            <ext:TemplateColumn DataIndex="FISNO" Header="Belge No" Width="100">
                                <Template runat="server">
                                    <Html>
                                        <a href="javascript:BelgeAc('{FISNO}')">{FISNO}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <ext:DateColumn runat="server" Header="Belge Tarihi" DataIndex="FISTARIHI" Format="dd.m.Y"
                                Width="90" />
                            <ext:Column Header="Üretilen Malzeme" DataIndex="ANAURUNHESAPAD" Width="200" />
                            <ext:Column Header="Giren Muhasebe" DataIndex="GIRENMUHASEBEADI" Width="200" />
                            <ext:Column Header="Giren Harcama Birimi" DataIndex="GIRENHARCAMABIRIMADI" Width="200" />
                            <ext:Column Header="Giren TİF No" DataIndex="GIRENTIFFISNO" />
                            <ext:Column Header="Çıkan TİF No" DataIndex="CIKANTIFFISNO" />
                            <ext:Column Header="İşlem Yapan" DataIndex="ISLEMYAPAN" />
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
