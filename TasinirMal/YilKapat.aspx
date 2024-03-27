<%@ Page Language="C#" CodeBehind="YilKapat.aspx.cs" Inherits="TasinirMal.YilKapat" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function YilKapatListesiAc() {
            var adres = "ListeYilKapat.aspx?menuYok=1";
            var yil = txtYil.getValue();
            adres += "&yil=" + yil;

            showPopWin(adres, 700, 500, true, null, true);
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnSeciliKod" runat="server" />
        <ext:Hidden ID="hdnSecKapat" runat="server" />
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10" LabelWidth="150">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnKapat" runat="server" Text="<%$Resources:TasinirMal,FRMYLK011%>" Icon="ApplicationFormDelete">
                                            <DirectEvents>
                                                <Click OnEvent="btnKapat_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt kapatılacaktır. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnAc" runat="server" Text="<%$Resources:TasinirMal,FRMYLK012%>" Icon="ApplicationEdit">
                                            <DirectEvents>
                                                <Click OnEvent="btnAc_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Kayıt açılacaktır. Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                        <ext:Button ID="btnAra" runat="server" Text="<%$Resources:TasinirMal,FRMYLK013%>" Icon="Magnifier">
                                            <Listeners>
                                                <Click Handler="YilKapatListesiAc();"></Click>
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ToolbarSeparator />
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" />
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMYLK005 %>" Width="130">
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
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMYLK007 %>" Width="130">
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
                                        <ext:TriggerField ID="txtAmbar" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMYLK009 %>" Width="130">
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
                                <ext:Panel ID="pnlAciklama" runat="server" BodyPadding="5" Layout="VBoxLayout" Height="200">
                                    <Items>
                                        <ext:Label runat="server" Icon="BulletBlack" Html="<%$Resources:TasinirMal,FRMYLK014 %>" />
                                        <ext:Label runat="server" Icon="BulletBlack" Html="<%$Resources:TasinirMal,FRMYLK015 %>" />
                                        <ext:Label runat="server" Icon="BulletBlack" Html="<%$Resources:TasinirMal,FRMYLK016 %>" />
                                        <ext:Label runat="server" Icon="BulletBlack" Html="<%$Resources:TasinirMal,FRMYLK017 %>" />
                                        <ext:Label runat="server" Icon="BulletBlack" Html="<%$Resources:TasinirMal,FRMYLK018 %>" />
                                    </Items>
                                </ext:Panel>

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
