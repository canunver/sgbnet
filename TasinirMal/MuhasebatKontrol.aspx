<%@ Page Language="C#" CodeBehind="MuhasebatKontrol.aspx.cs" Inherits="TasinirMal.MuhasebatKontrol" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=13"></script>
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
        <ext:Store ID="strDuzey" runat="server">
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
                                        <ext:Button ID="btnYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMECK029%>" Icon="PageExcel">
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
                                <ext:ComboBox ID="ddlDuzey" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMMHK013%>" Width="120" StoreID="strDuzey" ValueField="KOD" DisplayField="ADI" QueryMode="Local" />
                                <ext:RadioGroup ID="rdGrup" runat="server" BoxMaxWidth="550">
                                    <Items>
                                        <ext:Radio ID="rdMuhasebeBazinda" runat="server" Checked="true" BoxLabel="<%$ Resources:TasinirMal, FRMMHK003 %>" Width="50" />
                                        <ext:Radio ID="rdHarcamaBazinda" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMMHK004 %>" Width="50" />
                                        <ext:Radio ID="rdMuhasebeHarcamaBazinda" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMMHK005 %>" Width="50" />
                                    </Items>
                                </ext:RadioGroup>
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
