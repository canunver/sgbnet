<%@ Page Language="C#" CodeBehind="ListeHesapPlani.aspx.cs" Inherits="TasinirMal.ListeHesapPlani" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/ListeHesapPlani.js?v=23"></script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Hidden ID="hdnAktifYil" runat="server" />
        <ext:Hidden ID="hdnBilgi" runat="server" />
        <ext:Hidden ID="hdnCagiran" runat="server" />
        <ext:Hidden ID="hdnCagiranLabel" runat="server" />
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="FitLayout">
            <Items>
                <ext:TabPanel ID="tabPanelGenel" runat="server" Plain="true" StyleSpec="background-color:white;padding:5px">
                    <Items>
                        <ext:Panel ID="tabListe" runat="server" Title="<%$ Resources:TasinirMal, FRMLHP005 %>"
                            Layout="FitLayout">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:Button ID="btnSec" runat="server" Icon="BulletPlus" Text="<%$ Resources:TasinirMal, FRMLHP007 %>"
                                            Visible="false">
                                            <Listeners>
                                                <Click Handler="SeciliKodBul();" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:TreePanel ID="trvHesap" runat="server" AutoWidth="true" AutoScroll="true" Border="false">
                                    <Listeners>
                                        <BeforeClick Fn="NodeKodSecKapat" />
                                    </Listeners>
                                </ext:TreePanel>
                            </Items>
                        </ext:Panel>
                        <ext:Panel ID="tabArama" runat="server" Title="<%$ Resources:TasinirMal, FRMLHP006 %>"
                            Layout="FitLayout">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:TextField ID="txtHesapKod" runat="server" EmptyText="<%$ Resources:TasinirMal, FRMLHP010 %>" />
                                        <ext:TextField ID="txtHesapAd" runat="server" EmptyText="<%$ Resources:TasinirMal, FRMLHP008 %>" />
                                        <ext:Button ID="btnArama" runat="server" Icon="Magnifier" Text="<%$ Resources:TasinirMal, FRMLHP009 %>">
                                            <Listeners>
                                                <Click Handler="DalYukleArama('arama')" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:TreePanel ID="trvHesapArama" runat="server" AutoScroll="true" Border="false"
                                    Height="200">
                                    <Root>
                                        <ext:TreeNode Text="Hesap Planı">
                                        </ext:TreeNode>
                                    </Root>
                                    <Listeners>
                                        <BeforeClick Fn="NodeKodSecKapat" />
                                    </Listeners>
                                </ext:TreePanel>
                            </Items>
                        </ext:Panel>
                        <ext:Panel ID="tabBarkodlaArama" runat="server" Title="<%$ Resources:TasinirMal, FRMLHP011 %>"
                            Layout="FitLayout">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar3" runat="server">
                                    <Items>
                                        <ext:TextField ID="txtHesapKodBarkod" runat="server" EmptyText="<%$ Resources:TasinirMal, FRMLHP012 %>" />
                                        <ext:Button ID="btnBarkodlaArama" runat="server" Icon="Magnifier" Text="<%$ Resources:TasinirMal, FRMLHP009 %>">
                                            <Listeners>
                                                <Click Handler="DalYukleArama('barkod')" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:TreePanel ID="trvHesapAramaBarkod" runat="server" AutoScroll="true" Border="false">
                                    <Root>
                                        <ext:TreeNode Text="Hesap Planı">
                                        </ext:TreeNode>
                                    </Root>
                                    <Listeners>
                                        <BeforeClick Fn="NodeKodSecKapat" />
                                    </Listeners>
                                </ext:TreePanel>
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:TabPanel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
