﻿<%@ Page Language="C#" CodeBehind="TanimHarcamaBirimi.aspx.cs" Inherits="TasinirMal.TanimHarcamaBirimi" %>

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
        <ext:Hidden ID="hdnIlce" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="MUHASEBEKOD" />
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                        <ext:RecordField Name="KAPALI" />
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
                            Border="true" StoreID="strListe" Region="West" Width="400" Split="true" AutoExpandColumn="ADI" Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtFiltre" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMORT003 %>"
                                            LabelWidth="40" Width="250" EnableKeyEvents="true">
                                            <Listeners>
                                                <KeyUp Handler="ApplyFilter('ADI');" />
                                                <TriggerClick Fn="TriggerClick" />
                                            </Listeners>
                                            <Triggers>
                                                <ext:FieldTrigger Icon="SimpleCross" />
                                            </Triggers>
                                        </ext:TriggerField>
                                        <ext:ToolbarFill />
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:RowNumbererColumn />
                                    <ext:Column ColumnID="MUHASEBEKOD" DataIndex="MUHASEBEKOD" Header="Muhasebe Kodu" />
                                    <ext:Column ColumnID="KOD" DataIndex="KOD" Header="Harcama Birimi Kodu" />
                                    <ext:Column ColumnID="ADI" DataIndex="ADI" Header="Adı" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" />
                            </SelectionModel>
                            <DirectEvents>
                                <CellClick OnEvent="SatirSecildi">
                                    <ExtraParams>
                                        <ext:Parameter Name="GRIDPARAM" Value="Ext1.encode(grdListe.getRowsValues({selectedOnly:true}))"
                                            Mode="Raw" />
                                    </ExtraParams>
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                </CellClick>
                            </DirectEvents>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="100" HideRefresh="true"
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
                                            <SelectedItem Value="100" />
                                            <Listeners>
                                                <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10" LabelWidth="150">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKaydet" runat="server" Text="<%$Resources:TasinirMal,FRMTMH018%>" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnKaydet_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnAra" runat="server" Text="<%$Resources:TasinirMal,FRMTMH020%>" Icon="Magnifier">
                                            <DirectEvents>
                                                <Click OnEvent="btnAra_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnSil" runat="server" Text="<%$Resources:TasinirMal,FRMTMH019%>" Icon="Delete">
                                            <DirectEvents>
                                                <Click OnEvent="btnSil_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt silinecektir. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnTemizle" runat="server" Text="<%$Resources:TasinirMal,FRMTMH021%>"
                                            Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnTemizle_Click">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHB010 %>" Width="130">
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
                                <ext:TextField ID="txtKod" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHB012 %>" Width="130" />
                                <ext:TextField ID="txtAd" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTHB013 %>" Width="300" />
                                <ext:Checkbox ID="chkKapali" runat="server" MarginSpec="0 0 0 155" BoxLabel="<%$ Resources:TasinirMal, FRMTHB014 %>" />
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
