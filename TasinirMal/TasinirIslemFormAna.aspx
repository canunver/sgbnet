﻿<%@ Page Language="C#" CodeBehind="TasinirIslemFormAna.aspx.cs" Inherits="TasinirMal.TasinirIslemFormAna" %>

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
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:TabPanel ID="tabPanelAna" runat="server" Region="Center" Margins="104 20 10 20" ActiveTabIndex="0" Plain="true" StyleSpec="background-color:white;padding:10px">
                    <Items>
                        <ext:Panel ID="panelIslem" runat="server" Title="Belge Düzenleme" AutoScroll="false">
                            <AutoLoad ShowMask="true" MaskMsg="Lütfen bekleyin..." Mode="IFrame">
                            </AutoLoad>
                        </ext:Panel>
                        <ext:Panel ID="panelSorgu" runat="server" Title="Belge Arama" AutoScroll="false">
                            <AutoLoad ShowMask="true" MaskMsg="Lütfen bekleyin..." Mode="IFrame">
                            </AutoLoad>
                        </ext:Panel>
                        <ext:Panel ID="panelIslemYeni" runat="server" Title="Belge Düzenleme (Eski)" AutoScroll="false" Hidden="true">
                            <AutoLoad ShowMask="true" MaskMsg="Lütfen bekleyin..." Mode="IFrame">
                            </AutoLoad>
                        </ext:Panel>
                    </Items>
                </ext:TabPanel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>