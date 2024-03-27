<%@ Page Language="C#" CodeBehind="KurusFarkRaporu.aspx.cs" Inherits="TasinirMal.KurusFarkRaporu" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/OrtakExt.js?v=15"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirOrtak.js?v=13"></script>
    <script language="javascript" type="text/javascript">
        function HesapPlaniGoster() {
            ListeAc('ListeHesapPlani.aspx', 'pnlTanim:txtHesapPlanKod', '', '');
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
                                        <ext:Button ID="btnListele" runat="server" Text="Raporla" Icon="PageExcel">
                                            <DirectEvents>
                                                <Click OnEvent="btnYazdirListe_Click" IsUpload="true">
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
                                <ext:DateField ID="txtTarih1" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV064 %>" Width="90" />
                                <ext:DateField ID="txtTarih2" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMCTV065 %>" Width="90" />
                                <ext:Checkbox ID="chkDetay" runat="server" BoxLabel="<%$Resources:TasinirMal,FRMKFR015 %>" />
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
