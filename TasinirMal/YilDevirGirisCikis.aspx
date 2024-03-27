<%@ Page Language="C#" CodeBehind="YilDevirGirisCikis.aspx.cs" Inherits="TasinirMal.YilDevirGirisCikis" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10" LabelWidth="150">
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" />
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMC011 %>" Width="120">
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
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMC013 %>" Width="120">
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
                                <ext:Panel ID="frmBilgi" runat="server" Width="250" Header="true" BodyStyle="padding:5px" LabelWidth="150">
                                    <TopBar>
                                        <ext:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <ext:Button ID="btnCikis" runat="server" Width="120px" Text="Çıkış Fişi Üret" Icon="CartRemove">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnCikis_Click">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:Button>

                                                <ext:Button ID="btnGiris" runat="server" Width="120px" Text="Giriş Fişi Üret" Icon="CartPut"
                                                    StyleSpec="color:red">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnGiris_Click">
                                                            <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:Button>
                                            </Items>
                                        </ext:Toolbar>
                                    </TopBar>
                                    <Items>
                                        <ext:Label ID="lblToplamAmbar" runat="server" FieldLabel="İşlem görecek ambar sayısı"
                                            Text="0" />
                                        <ext:Label ID="lblAmbarSayac" runat="server" FieldLabel="İşlem gören ambar sayısı"
                                            Text="0" />
                                        <ext:Label ID="lblKalanAmbar" runat="server" FieldLabel="Kalan ambar sayısı" Text="0" />
                                        <ext:Label ID="lblBelgeSayac" runat="server" FieldLabel="Üretilen belge sayısı" Text="0" />
                                        <ext:ProgressBar ID="Progress1" runat="server" Width="230" />
                                        <ext:Label ID="Label1" runat="server" Html="&nbsp;" />
                                        <ext:Button ID="btnIptal" runat="server" Width="120px" Text="İşlemi Durdur" Icon="Cancel"
                                            Hidden="true" StyleSpec="color:red">
                                            <DirectEvents>
                                                <Click OnEvent="btnIptal_Click">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Panel>
                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <ext:TaskManager ID="TaskManager1" runat="server">
            <Tasks>
                <ext:Task TaskID="IslemGostergec" Interval="2000" AutoRun="false">
                    <DirectEvents>
                        <Update OnEvent="RefreshProgress" />
                    </DirectEvents>
                </ext:Task>
            </Tasks>
        </ext:TaskManager>

        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
