<%@ Page Language="C#" CodeBehind="TanimDemirbasRFIDBarkod.aspx.cs" Inherits="TasinirMal.TanimDemirbasRFIDBarkod" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/base64.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/AnkarefBarcodePrint.js?v=2"></script>
    <script language="javascript" type="text/javascript">
        function BarkodYazdir() {

            var secili = grdListe.getRowsValues({ selectedOnly: true });

            var basilacakListe = [];
            secili.forEach(function (entry) {
                var barkod = [entry.epc, entry.sicilno, entry.ad];

                basilacakListe.push(barkod);
            });

            InventoryPointLabelPrint(basilacakListe);
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server" IgnoreExtraFields="false" AutoLoad="false"
            RemotePaging="true" RemoteSort="true" OnRefreshData="strListe_Refresh" RemoteGroup="true">
            <Reader>
                <ext:JsonReader IDProperty="prSicilNo">
                    <Fields>
                        <ext:RecordField Name="tip" />
                        <ext:RecordField Name="prSicilNo" />
                        <ext:RecordField Name="sicilno" />
                        <ext:RecordField Name="kod" />
                        <ext:RecordField Name="ad" />
                        <ext:RecordField Name="kimeGitti" />
                        <ext:RecordField Name="epc" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
            <BaseParams>
                <ext:Parameter Name="start" Value="0" Mode="Raw" />
                <ext:Parameter Name="limit" Value="0" Mode="Raw" />
                <ext:Parameter Name="sort" Value="" />
                <ext:Parameter Name="dir" Value="ASC" />
            </BaseParams>
            <Proxy>
                <ext:PageProxy />
            </Proxy>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" Split="true" Border="true" Width="250">
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
                                        <ext:Button ID="btnListe" runat="server" Text="<%$Resources:Evrak,FRMSRG119 %>" Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListe_Click" Timeout="2400000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Source>
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
                                <ext:PropertyGridParameter Name="prpSicilNo" DisplayName="<%$ Resources:TasinirMal, FRMBRK018 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpSicilNo',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpEskiSicilNo" DisplayName="Eski Sicil No">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpKisiKod" DisplayName="TC Kimlik No">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpKisiKod',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpOdaKod" DisplayName="Oda Kodu">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpOdaKod',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpEserAdi" DisplayName="<%$ Resources:TasinirMal, FRMBRK020 %>">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpYil" DisplayName="<%$ Resources:TasinirMal, FRMBRK005 %>">
                                    <Editor>
                                        <ext:SpinnerField runat="server">
                                        </ext:SpinnerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpBelgeNoTif" DisplayName="Belge No TİF">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpBelgeNoZimmet" DisplayName="Belge No Zimmet">
                                </ext:PropertyGridParameter>
                                <%--<ext:PropertyGridParameter Name="prpBelgeNo" DisplayName="Belge No (TİF, Zimmet)">
                                </ext:PropertyGridParameter>--%>
                                <ext:PropertyGridParameter Name="prpDurumKod" DisplayName="Durumu">
                                    <Renderer Handler="if (value=='1') return 'Ambarda'; else if (value=='2') return 'Zimmette'; else return 'Hepsi';" />
                                    <Editor>
                                        <ext:ComboBox ID="dllDurum" runat="server" Editable="false">
                                            <Items>
                                                <ext:ListItem Text="Hepsi" Value="" />
                                                <ext:ListItem Text="Ambarda" Value="1" />
                                                <ext:ListItem Text="Zimmette" Value="2" />
                                            </Items>
                                        </ext:ComboBox>
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
                        <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" StoreID="strListe"
                            Border="true" Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button runat="server" Text="Raporla" Icon="PageExcel">
                                            <Listeners>
                                                <Click Handler="BarkodYazdir();" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:TextField ID="txtYaziciAdres" runat="server" Width="200" EmptyText="Yazıcı URL. Ör:172.16.89.20:6754">
                                        </ext:TextField>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <LoadMask ShowMask="true" Msg="Lütfen Bekleyiniz..." />
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:Column ColumnID="tip" DataIndex="tip" Align="Left" Width="70" Header="Belge Türü" />
                                    <ext:Column ColumnID="prSicilNo" DataIndex="prSicilNo" Hidden="true" Align="Left" />
                                    <ext:Column ColumnID="sicilno" DataIndex="sicilno" Header="<%$ Resources:TasinirMal, FRMBRK037 %>" Align="Left" Width="160" />
                                    <ext:Column ColumnID="kod" DataIndex="kod" Header="<%$ Resources:TasinirMal, FRMBRK038 %>" Align="Left" Width="160" />
                                    <ext:Column ColumnID="ad" DataIndex="ad" Header="<%$ Resources:TasinirMal, FRMBRK039 %>" Align="Left" Width="225" />
                                    <ext:Column ColumnID="kimeGitti" DataIndex="kimeGitti" Header="<%$ Resources:TasinirMal, FRMBRK040 %>" Align="Left" Width="160" />
                                    <ext:Column ColumnID="epc" DataIndex="epc" Header="EPC" Align="Left" Width="160" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel runat="server" />
                            </SelectionModel>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" HideRefresh="true"
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
                                            <SelectedItem Value="250" />
                                            <Listeners>
                                                <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
