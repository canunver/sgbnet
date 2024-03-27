<%@ Page Language="C#" CodeBehind="TerminalOku.aspx.cs" Inherits="TasinirMal.TerminalOku" EnableViewState="false" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%= Resources.TasinirMal.FRMTOK001 %></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TerminalOku.js?mc=03022015&v=2013"></script>
</head>
<body>
    <form id="aspnetForm" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMTOK002 %></div>
            <div class="veriAlan">
                <asp:FileUpload ID="fuDosya" runat="server" />
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
        <div class="row">
            <div class="veriAlan" style="width: 100%">
                <center>
                    <asp:Button ID="btnDosyaOku" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMTOK003 %>"
                        OnClick="btnDosyaOku_Click" />
                    <input id="Button1" type="button" value="<%= Resources.TasinirMal.FRMTOK004 %>" class="dugme"
                        onclick="javascript:Aktar();" />
                </center>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
        <div class="row" runat="server" id="divKolonSecim" style="display: none">
            <div class="veriAlan">
                <asp:RadioButton ID="rdKolonAmbar" runat="server" GroupName="rdKolon" Text="<%$ Resources:TasinirMal, FRMTOK005 %>" />
                <asp:RadioButton ID="rdKolonOrtakAlan" runat="server" GroupName="rdKolon" Text="<%$ Resources:TasinirMal, FRMTOK006 %>" />
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <div class="footer">
        &nbsp;</div>
    <div style="overflow: auto; height: 320px; width: 100%">
        <asp:GridView ID="gvDosyadanGelenler" runat="server" CellPadding="1" ForeColor="#333333"
            GridLines="None" Width="98%" AutoGenerateColumns="false">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Left"
                Height="30px" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:TemplateField ItemStyle-Width="50px" HeaderText="<input type='checkbox' onclick='javascript:ListeSec();' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSecim" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="yer" HeaderText="<%$ Resources:TasinirMal, FRMTOK007 %>">
                    <ItemStyle HorizontalAlign="Left" Width="100px" />
                </asp:BoundField>
                <asp:BoundField DataField="hesapplankod" HeaderText="<%$ Resources:TasinirMal, FRMTOK008 %>">
                    <ItemStyle HorizontalAlign="Left" Width="300px" />
                </asp:BoundField>
                <asp:BoundField DataField="miktar" HeaderText="<%$ Resources:TasinirMal, FRMTOK009 %>">
                    <ItemStyle HorizontalAlign="Left" Width="100px" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </div>
    <input type="hidden" id="hdnCagiran" runat="server" />
    </form>
</body>
</html>
