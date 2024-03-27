﻿<%@ Page Language="C#" CodeBehind="Cetveller.aspx.cs" Inherits="TasinirMal.Cetveller" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=13"></script>
    <script language="javascript" type="text/javascript">
        function IlceDoldur() {
            var ilKod = App.ddlIl.getValue();
            App.direct.IlceDoldur(ilKod);
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:Hidden ID="hdnFirmaHarcamadanAlma" runat="server" />
        <ext:Hidden ID="hdnHesapKod" runat="server" />
        <ext:Store ID="strIl" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strIlce" runat="server" OnRefreshData="IlceDoldur">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="pnlDis" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10" LabelWidth="150">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMCTV067%>" Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdir_Click" IsUpload="true">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" />
                                <ext:CompositeField ID="cmpBolge" runat="server" Visible="false">
                                    <Items>
                                        <ext:TriggerField ID="txtBolge" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV054 %>" Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblBolgeAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:ComboBox ID="ddlIl" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV056%>" Visible="false" Width="120" StoreID="strIl" ValueField="KOD" DisplayField="ADI" QueryMode="Local">
                                    <Listeners>
                                        <Select Handler="#{strIlce}.reload(); #{ddlIlce}.setValue(null);" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:ComboBox ID="ddlIlce" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV057%>" Visible="false" Width="120" StoreID="strIlce" ValueField="KOD" DisplayField="ADI" QueryMode="Local" />
                                <ext:CompositeField ID="cmpMuhasebe" runat="server" Visible="false">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG025 %>" Width="120">
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
                                <ext:CompositeField ID="cmpHarcamaBirimi" runat="server" Visible="false">
                                    <Items>
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMGAG027 %>" Width="120">
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
                                <ext:CompositeField ID="cmpHesapPlan" runat="server" Visible="false">
                                    <Items>
                                        <ext:TriggerField ID="txtHesapPlanKod" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV062 %>" Width="120">
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
                                <ext:DateField ID="txtTarih1" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV064 %>" Width="90" Visible="false" />
                                <ext:DateField ID="txtTarih2" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV065 %>" Width="90" Visible="false" />
                                <ext:ComboBox ID="ddlSeviye" runat="server" Editable="false" FieldLabel="Seviye" SelectOnFocus="true" Width="40" Visible="false">
                                    <Items>
                                        <ext:ListItem Text="1" Value="1" />
                                        <ext:ListItem Text="2" Value="2" />
                                        <ext:ListItem Text="3" Value="3" />
                                    </Items>
                                </ext:ComboBox>
                                <ext:Checkbox ID="chkMuhasebeRapor" runat="server" BoxLabel="<%$Resources:TasinirMal,FRMCTV066 %>" Visible="false" />
                                <ext:Checkbox ID="chkCSV" runat="server" BoxLabel="Sayıştay CSV Formatında çıktı üret" Visible="false" />
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
