<%@ Page Language="C#" CodeBehind="Defterler2.aspx.cs" Inherits="TasinirMal.Defterler2" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=13"></script>
    <script language="JavaScript" type="text/javascript" >
        function IlkRaporuSec()
        {
            grdListe.getSelectionModel().selectRow(strListe.find("KOD", "TMM001"));
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager2" runat="server">
            <Listeners>
                <DocumentReady Handler="IlkRaporuSec();AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnRaporKod" runat="server" />
        <ext:Hidden ID="hdnFirmaHarcamadanAlma" runat="server" />
        <ext:Hidden ID="hdnHesapKod" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="AD" />
                        <ext:RecordField Name="DOSYAAD" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
            <Items>
                <ext:BorderLayout ID="BorderLayout1" runat="server">
                    <Center MarginsSummary="104 20 10 20">
                        <ext:Panel ID="tabPanelAna" runat="server" StyleSpec="background-color:white;padding:10px">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnYazdir" runat="server" Text="Raporla" Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="RAPORBILGI" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:BorderLayout ID="BorderLayout2" runat="server">
                                    <Center>
                                        <ext:FormPanel ID="frmPanel" runat="server" Frame="true" AutoScroll="true" Padding="5"
                                            Title="Rapor Kriter Alanları" LabelWidth="140">
                                            <Items>
                                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" />
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
                                                <ext:CompositeField runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV062 %>">
                                                    <Items>
                                                        <ext:TriggerField ID="txtHesapPlanKod" runat="server" Width="120">
                                                            <Triggers>
                                                                <ext:FieldTrigger Icon="Search" />
                                                            </Triggers>
                                                            <Listeners>
                                                                <TriggerClick Fn="TriggerClick" />
                                                                <Change Fn="TriggerChange" />
                                                            </Listeners>
                                                        </ext:TriggerField>
                                                        <ext:Label ID="lblHesapPlanAd" runat="server" />
                                                    </Items>
                                                </ext:CompositeField>
                                                <ext:TextField ID="txtIlkKayit" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMDFT014 %>" Width="120" Hidden="true" />
                                                <ext:TextField ID="txtKayitSayisi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMDFT015 %>" Width="120" Hidden="true" />
                                                <ext:TextField ID="txtKitapAd" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMDFT016 %>" Width="120" Hidden="true" />
                                                <ext:TextField ID="txtYazarAd" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMDFT017 %>" Width="120" Hidden="true" />
                                                <ext:TextField ID="txtYer" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMDFT018 %>" Width="120" Hidden="true" />
                                            </Items>
                                        </ext:FormPanel>
                                    </Center>
                                    <West Split="true">
                                        <ext:GridPanel ID="grdListe" runat="server" StripeRows="true" TrackMouseOver="true"
                                            StoreID="strListe" Border="true" AutoExpandColumn="AD" Layout="FitLayout" Width="400"
                                            Title="Raporlar" HideHeaders="true">
                                            <ColumnModel ID="ColumnModel1" runat="server">
                                                <Columns>
                                                    <ext:RowNumbererColumn />
                                                    <ext:Column DataIndex="AD" MenuDisabled="true" Sortable="false" />
                                                </Columns>
                                            </ColumnModel>
                                            <SelectionModel>
                                                <ext:RowSelectionModel ID="grdListeSelectionModel" runat="server" SingleSelect="true">
                                                    <DirectEvents>
                                                        <RowSelect OnEvent="SatirSecildi" Buffer="100">
                                                            <EventMask ShowMask="true" Msg="Lütfen bekleyin..." />
                                                            <ExtraParams>
                                                                <ext:Parameter Name="GRIDPARAM" Value="Ext1.encode(#{grdListe}.getRowsValues({selectedOnly:true}))"
                                                                    Mode="Raw" />
                                                            </ExtraParams>
                                                        </RowSelect>
                                                    </DirectEvents>
                                                </ext:RowSelectionModel>
                                            </SelectionModel>
                                        </ext:GridPanel>
                                    </West>
                                </ext:BorderLayout>
                            </Items>
                        </ext:Panel>
                    </Center>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
