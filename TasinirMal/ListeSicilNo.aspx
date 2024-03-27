<%@ Page Language="C#" CodeBehind="ListeSicilNo.aspx.cs" Inherits="TasinirMal.ListeSicilNo"
    EnableEventValidation="false" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= Resources.TasinirMal.FRMLSC001 %></title>
    <meta http-equiv="Pragma" content="no-cache" />
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/ListeSicilNo.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Defterler.js?v=1"></script>
    <script language="JavaScript" type="text/javascript">
        function TasinirFormunaSicilNoYukle() {
            parent.document.getElementById('btnSicilNoYukle').click();
        }
    </script>
</head>
<body onunload="HesapPlaniKapat();">
    <form id="aspnetForm" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div class="form">
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC003 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 157px;">
                <asp:TextBox ID="txtHesapPlaniKodu" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox>
            </div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMLSC004 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:HesapPlaniGoster();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblHesapPlanAd" Width="80%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC005 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 227px;">
                <asp:TextBox ID="txtHesapPlaniAdi" runat="server" CssClass="veriAlan" MaxLength="40"
                    Width="150px"></asp:TextBox></div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC027 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 150px;">
                <asp:TextBox ID="txtSicilNo" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC006 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtMarka" runat="server" CssClass="veriAlan" MaxLength="5" Width="50px"></asp:TextBox>
            </div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMLSC007 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMarka.aspx','txtMarka','','lblMarkaAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblMarkaAd" Width="150px" runat="server" CssClass="veriAlan"></asp:Label>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC008 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 150px;">
                <asp:TextBox ID="txtPlaka" runat="server" CssClass="veriAlan" MaxLength="20" Width="100px"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC009 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtModel" runat="server" CssClass="veriAlan" MaxLength="5" Width="50px"></asp:TextBox>
            </div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMLSC010 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeModel.aspx','txtModel','txtMarka','lblModelAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblModelAd" Width="150px" runat="server" CssClass="veriAlan"></asp:Label>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC011 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 150px;">
                <asp:TextBox ID="txtEserAdi" runat="server" CssClass="veriAlan" MaxLength="255" Width="100px"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC012 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 60px;">
                <asp:DropDownList ID="ddlYil" runat="server">
                </asp:DropDownList>
                &nbsp;
            </div>
            <div class="veriAlan" style="width: 177px;">
                <asp:TextBox ID="txtBelgeNo" runat="server" CssClass="veriAlan" MaxLength="6" Width="50px"></asp:TextBox></div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC013 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 150px;">
                <asp:TextBox ID="txtEskiSicil" runat="server" CssClass="veriAlan" MaxLength="255"
                    Width="100px"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMLSC025 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtSeriNo" runat="server" CssClass="veriAlan" Width="200px"></asp:TextBox>&nbsp;<%= Resources.TasinirMal.FRMLSC026 %>&nbsp;
                <asp:FileUpload runat="server" ID="fupSeriNo" />
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <div style="height: 3px">
        </div>
        <%= Resources.TasinirMal.FRMLSC014 %>&nbsp;&nbsp;<asp:TextBox ID="txtKayitSay" runat="server"
            CssClass="veriAlan" MaxLength="5" Width="30px"></asp:TextBox>&nbsp;&nbsp;<%= Resources.TasinirMal.FRMLSC015 %>
        <div style="height: 3px">
        </div>
        <%= Resources.TasinirMal.FRMLSC022 %>&nbsp;&nbsp;<asp:TextBox runat="server" ID="txtSayfaNo"
            Text="" CssClass="veriAlan" MaxLength="5" Width="30px"></asp:TextBox>
        <div style="height: 3px">
        </div>
        <asp:Button ID="btnListele" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMLSC016 %>"
            Width="120" OnClick="btnListele_Click" />
        <asp:Button ID="btnSonrakiSayfa" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMLSC023 %>"
            Width="120" OnClick="btnListele_Click" Visible="false" />
        <input type="button" id="btnSecilenAktar" runat="server" class="dugme" value="<%$ Resources:TasinirMal, FRMLSC017 %>"
            style="width: 120px; display: none" onclick="javascript:Aktar('secilen');return false;" />
        <asp:Button ID="btnSecilenleriAktar2" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMLSC017 %>"
            Width="120" OnClick="btnSecilenleriAktar2_Click" Visible="false" />
        <asp:Button ID="btnYeniSorgu" runat="server" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMLSC024 %>"
            Width="120" OnClick="btnYeniSorgu_Click" Visible="false" />
        <div style="height: 3px">
        </div>
    </center>
    <div style="overflow: auto; height: 280px; width: 100%;">
        <asp:GridView ID="gvSicilNo" runat="server" CellPadding="1" ForeColor="#333333" GridLines="None"
            Width="97%" AutoGenerateColumns="false">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Left"
                Height="30px" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:TemplateField ItemStyle-Width="5%" HeaderText="<input type='checkbox' onclick='javascript:ListeSec();' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSecim" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="sicilNo" HeaderText="<%$ Resources:TasinirMal, FRMLSC018 %>"
                    ItemStyle-Width="20%" />
                <asp:BoundField DataField="kod" HeaderText="<%$ Resources:TasinirMal, FRMLSC003 %>"
                    ItemStyle-Width="15%" />
                <asp:BoundField DataField="ad" HeaderText="<%$ Resources:TasinirMal, FRMLSC005 %>"
                    ItemStyle-Width="35%" />
                <asp:BoundField DataField="kdvoran" HeaderText="<%$ Resources:TasinirMal, FRMLSC019 %>"
                    ItemStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="birimfiyat" HeaderText="<%$ Resources:TasinirMal, FRMLSC020 %>"
                    ItemStyle-Width="10%" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="rfid" Visible="false" HeaderText="<%$ Resources:TasinirMal, FRMLSC021 %>"
                    ItemStyle-Width="10%" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
            </Columns>
        </asp:GridView>
    </div>
    <input type="hidden" id="hdnCagiran" runat="server" />
    <input type="hidden" id="hdnCagiranLabel" runat="server" />
    <input type="hidden" id="hdnSicilNolar" runat="server" />
    </form>
</body>
</html>