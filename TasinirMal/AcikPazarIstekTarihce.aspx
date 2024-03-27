<%@ Page Language="C#" CodeBehind="AcikPazarIstekTarihce.aspx.cs" Inherits="TasinirMal.AcikPazarIstekTarihce" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="row">
        <div style="overflow: auto; height: 300px;">
            <asp:DataGrid HorizontalAlign="Center" ID="dgListe" runat="server" AutoGenerateColumns="False"
                CellPadding="2" Font-Bold="False" Font-Italic="False" Font-Names="Tahoma" Font-Overline="False"
                Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" ForeColor="#333333"
                PageSize="1" Width="95%" EnableViewState="False">
                <ItemStyle BorderColor="LightGray" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                    VerticalAlign="Middle" />
                <Columns>
                    <asp:BoundColumn DataField="SiraNo" HeaderText="<%$ Resources:TasinirMal, FRMAPT001 %>">
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="MuhasebeKod" HeaderText="<%$ Resources:TasinirMal, FRMAPT002 %>">
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="MuhasebeAd" HeaderText="<%$ Resources:TasinirMal, FRMAPT003 %>">
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="HarcamaKod" HeaderText="<%$ Resources:TasinirMal, FRMAPT004 %>">
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="HarcamaAd" HeaderText="<%$ Resources:TasinirMal, FRMAPT005 %>">
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="IstekTarihi" HeaderText="<%$ Resources:TasinirMal, FRMAPT006 %>">
                    </asp:BoundColumn>
                </Columns>
            </asp:DataGrid>
        </div>
    </div>
    </form>
</body>
</html>
