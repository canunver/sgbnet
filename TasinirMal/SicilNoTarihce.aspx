<%@ Page Language="C#" CodeBehind="SicilNoTarihce.aspx.cs" Inherits="TasinirMal.SicilNoTarihce" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function Render(value, metadata) {
            metadata.attr = 'style="white-space: normal;word-wrap: break-word;"';
            return value;
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="prSicilNo">
                    <Fields>
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="SICILAD" />
                        <ext:RecordField Name="TARIH" />
                        <ext:RecordField Name="MUHASEBEBIRIMIKOD" />
                        <ext:RecordField Name="HARCAMABIRIMIKOD" />
                        <ext:RecordField Name="AMBAR" />
                        <ext:RecordField Name="BELGENO" />
                        <ext:RecordField Name="FIYATI" />
                        <ext:RecordField Name="ISLEM" />
                        <ext:RecordField Name="ESKISICILNO" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
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
                            </Source>
                            <Listeners>
                                <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                                <SortChange Handler="this.getStore().sortInfo = undefined;" />
                            </Listeners>
                            <View>
                                <ext:GridView ID="GridView1" ForceFit="true" runat="server" />
                            </View>
                        </ext:PropertyGrid>
                        <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" StoreID="strListe" Title="Tarihçe Listesi"
                            Border="true" Cls="gridExt">
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:RowNumbererColumn />
                                    <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Align="Left" Width="150" Header="Sicil No">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="SICILAD" DataIndex="SICILAD" Header="Sicil Ad" Align="Left">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="ESKISICILNO" DataIndex="ESKISICILNO" Header="Eski Sicil No">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="TARIH" DataIndex="TARIH" Header="Tarih"
                                        Width="75">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="MUHASEBEBIRIMIKOD" DataIndex="MUHASEBEBIRIMIKOD" Header="Muhasebe Birimi">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="HARCAMABIRIMIKOD" DataIndex="HARCAMABIRIMIKOD" Header="Harcama Birimi">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="AMBAR" DataIndex="AMBAR" Header="Ambar">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="BELGENO" DataIndex="BELGENO" Header="Belge No">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="FIYATI" DataIndex="FIYATI" Header="Fiyatı"
                                        Width="100" Align="Right">
                                        <Editor>
                                            <ext:TextField ReadOnly="true" runat="server"></ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column ColumnID="ISLEM" DataIndex="ISLEM" Header="İşlem" Width="180" Wrap="true">
                                        <Renderer Fn="Render" />
                                        <Editor>
                                            <ext:TextArea ReadOnly="true" runat="server"></ext:TextArea>
                                        </Editor>
                                    </ext:Column>
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel ID="grdListeSelectionModel" runat="server" />
                            </SelectionModel>
                            <BottomBar>
                                <ext:PagingToolbar ID="grdPagingToolbar" runat="server" PageSize="100" HideRefresh="true"
                                    StoreID="strListe">
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
