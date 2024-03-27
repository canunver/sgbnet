<%@ Page Language="C#" CodeBehind="ZimmetKisiListeleEski.aspx.cs" Inherits="TasinirMal.ZimmetKisiListeleEski" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%= Resources.TasinirMal.FRMZKL008 %></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript">
        function ListeSec() {
            var kontrol = document.getElementById('gvZimmet');
            if (kontrol) {
                secim = kontrol.rows[0].cells[0].children[0].checked;
                for (var i = 1; i < kontrol.rows.length; i++)
                    kontrol.rows[i].cells[0].children[0].checked = secim;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <center>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div>
        <asp:Label ID="lblBilgi" runat="server" Text="<%$ Resources:TasinirMal, FRMZKL009 %>"></asp:Label>
        <br />
        <div id="LDAPdogrulama" runat="server" class="araBilgi" visible="false">
            <asp:Label ID="lblLDAPbilgi" runat="server" Text="<%$ Resources:TasinirMal, FRMZMO004 %>"></asp:Label>
            <asp:TextBox ID="txtLDAPUser" runat="server" TabIndex="1" Text=""></asp:TextBox>
            <asp:TextBox ID="txtLDAPPassword" runat="server" TextMode="password" TabIndex="2" Text=""></asp:TextBox>
            <asp:TextBox ID="lblLDAPDurum" runat="server" Enabled="false" ForeColor="Red" TabIndex="3" Text="<%$ Resources:TasinirMal, FRMZMO005 %>"></asp:TextBox>
            <asp:Button ID="btnLDAP" runat="server" Text="<%$ Resources:TasinirMal, FRMZMO002 %>" OnClick="btnLDAP_Click" TabIndex="4" />
        </div>
        <br />
        <asp:TextBox ID="txtTCKimlik" runat="server" MaxLength="11" Columns="11" TabIndex="1"></asp:TextBox>
        <asp:Button ID="btnSorgula" runat="server" Text="<%$ Resources:TasinirMal, FRMZKL010 %>" OnClick="btnSorgula_Click" TabIndex="2" />
        <asp:Button ID="btnRapor" runat="server" Text="<%$ Resources:TasinirMal, FRMZKL011 %>" OnClick="btnRapor_Click" TabIndex="3" />
    </div>
    <br />
    <div>
        <asp:GridView ID="gvZimmet" runat="server" CellPadding="4" ForeColor="#333333"
        Visible="false" GridLines="None" Width="98%" Font-Names="Arial" Font-Size="8pt">
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" />
            <EditRowStyle BackColor="#999999" Font-Size="8pt" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White"  HorizontalAlign="Center" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="<input type='checkbox' onclick='javascript:ListeSec();' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSecim" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    </center>
    </form>
</body>
</html>