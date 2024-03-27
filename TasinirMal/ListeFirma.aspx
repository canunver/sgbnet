<%@ Page Language="C#" Inherits="TasinirMal.ListeFirma" CodeBehind="ListeFirma.aspx.cs" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= Resources.TasinirMal.FRMLFR001 %></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="ModulScripts/ListeFirma.js?v=1"></script>
</head>
<body>
    <form id="aspnetForm" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form" style="width: 96%">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMLFR002 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtAd" runat="server" TabIndex="1" Width="95%" CssClass="veriAlan"></asp:TextBox></div>
        </div>
        <br style="clear: both;" />
        <div style="clear: both;">
            <b>&nbsp;&nbsp;<%= Resources.TasinirMal.FRMLFR003 %></b></div>
        <div class="row">
            <div class="veriAlan">
                <asp:RadioButton GroupName="grpSecim" runat="server" Text="<%$ Resources:TasinirMal, FRMLFR004 %>"
                    ID="rdKendi" Checked="true"></asp:RadioButton>
                <asp:RadioButton GroupName="grpSecim" runat="server" Text="<%$ Resources:TasinirMal, FRMLFR005 %>"
                    ID="rdButun"></asp:RadioButton>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <asp:Button ID="btnBul" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMLFR006 %>"
            Width="100" OnClick="btnBul_Click"></asp:Button>
        <input type="button" id="btnKapat" class="dugme" style="width: 100px" value="<%= Resources.TasinirMal.FRMLFR007 %>"
            onclick="javascript:parent.hidePopWin();" />
        <input type="button" id="btnYeni" class="dugme" style="width: 100px" value="<%= Resources.TasinirMal.FRMLFR008 %>"
            onclick="javascript:PopupWin('TanimFirma.aspx?menuYok=1',620,500,'FirmaGiris');" />
    </center>
    <div class="row" style="width: 100%">
        <div style="overflow: auto; height: 350px;">
            <asp:DataGrid ID="dgListe" runat="server" AllowCustomPaging="False" AllowSorting="False"
                AutoGenerateColumns="False" BorderWidth="0" CellPadding="3" Font-Bold="False"
                Font-Italic="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="8pt"
                Font-Strikeout="False" Font-Underline="False" ForeColor="#333333" PageSize="1"
                Width="96%">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" Font-Italic="False" Font-Names="Tahoma"
                    Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False"
                    ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                <Columns>
                    <asp:BoundColumn DataField="ad" HeaderText="<%$ Resources:TasinirMal, FRMLFR009 %>">
                    </asp:BoundColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
    </form>
</body>
</html>
