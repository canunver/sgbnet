<%@ Page Language="C#" CodeBehind="TuketimUretimAna.aspx.cs" Inherits="TasinirMal.TuketimUretimAna" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title></title>
</head>
<body>
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Viewport ID="Viewport1" runat="server" StyleSpec="background-color: transparent;">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="104 20 10 20">
                    <ext:TabPanel ID="tabPanelAna" runat="server" ActiveTabIndex="0" Plain="true" StyleSpec="background-color:white;padding:10px">
                        <Items>
                            <ext:Panel ID="panelIslem" runat="server" Title="<%$ Resources:TasinirMal, FRMTUA001 %>" AutoScroll="false">
                                <AutoLoad ShowMask="true" MaskMsg="<%$ Resources:TasinirMal, FRMTUA002 %>" Mode="IFrame">
                                </AutoLoad>
                            </ext:Panel>
                            <ext:Panel ID="panelSorgu" runat="server" Title="<%$ Resources:TasinirMal, FRMTUA003 %>" AutoScroll="false">
                                <AutoLoad ShowMask="true" MaskMsg="<%$ Resources:TasinirMal, FRMTUA002 %>" Mode="IFrame">
                                </AutoLoad>
                            </ext:Panel>
                        </Items>
                    </ext:TabPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>