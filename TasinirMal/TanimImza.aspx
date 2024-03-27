<%@ Page Language="C#" Inherits="TasinirMal.TanimImza" CodeBehind="TanimImza.aspx.cs" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:Hidden ID="hdnSeciliKod" runat="server" />
        <ext:Hidden ID="hdnSecKapat" runat="server" />
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="HARCAMABIRIMKOD" />
                        <ext:RecordField Name="AMBARKOD" />
                        <ext:RecordField Name="IMZAYERKOD" />
                        <ext:RecordField Name="IMZAYER" />
                        <ext:RecordField Name="AD" />
                        <ext:RecordField Name="UNVAN" />
                        <ext:RecordField Name="GOREVUNVAN" />
                        <ext:RecordField Name="TARIH" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>

                        <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" Header="false" TrackMouseOver="true"
                            Border="true" StoreID="strListe" Region="Center" ClicksToEdit="1" Cls="gridExt">
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:RowNumbererColumn />
                                    <ext:Column ColumnID="IMZAYER" DataIndex="IMZAYER" Header="İmza Yeri" Width="220" />
                                    <ext:Column DataIndex="AD" Header="Ad Soyad" Width="220">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column DataIndex="UNVAN" Header="Unvanı" Width="220">
                                        <Editor>
                                            <ext:TextField runat="server" SelectOnFocus="true" />
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column DataIndex="TARIH" Header="Tarih Yaz" Width="55" Align="Center">
                                        <Editor>
                                            <ext:Checkbox runat="server" />
                                        </Editor>
                                    </ext:Column>
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" />
                            </SelectionModel>
                        </ext:GridPanel>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="North" Padding="10" LabelWidth="150" Height="120">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMTIM034%>" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                    <ExtraParams>
                                                        <ext:Parameter Name="IMZASATIRLARI" Value="Ext1.encode(#{grdListe}.getRowsValues(false, false, false))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnListele" runat="server" Text="<%$Resources:TasinirMal,FRMTIM033%>" Icon="Magnifier">
                                            <DirectEvents>
                                                <Click OnEvent="btnListele_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTAM011 %>" Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblMuhasebeAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTAM013 %>" Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblHarcamaBirimiAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTIM031 %>" Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblAmbarAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
