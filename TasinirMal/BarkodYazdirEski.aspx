<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BarkodYazdirEski.aspx.cs" Inherits="TasinirMal.BarkodYazdirEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=3"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/BarkodYazdir.js?v=1"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/base64.js?v=1"></script>
    <script language="javascript">
        function TasarimGG() {
            if (document.getElementById("chkTasarim").checked) {
                document.getElementById("divYaziciBilgi").style.display = "none";
                document.getElementById("divTasarimData").style.display = "block";
            }
            else {
                document.getElementById("divTasarimData").style.display = "none";
                document.getElementById("divYaziciBilgi").style.display = "block";
            }

        }
        function YeniGG() {
            if (document.getElementById("chkYeni").checked) {
                document.getElementById("divYeni").style.display = "block";
            }
            else {
                document.getElementById("divYeni").style.display = "none";
            }

        }
    </script>
</head>
<body>
    <form id="form" runat="server">
    <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <a href="JavaScript:showPopWin('BarkodYardim.htm', 700, 600, true, null);">
        <%= Resources.TasinirMal.FRMBRK004 %></a>
    <div class="form">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMBRK005 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 100px">
                <asp:DropDownList ID="ddlYil" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMBRK006 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMBRK007 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMBRK008 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox>
                </div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMBRK009 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMBRK010 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMBRK011 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtAmbar','txtHarcamaBirimi','lblAmbarAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                </div>
            </div>
        </div>
        <div class="row" id="divOrg0" runat="server" style="display: none">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMORG001 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <ext:DropDownField ID="ddlBirim" runat="server" Width="300" TriggerIcon="Ellipsis"
                    Mode="ValueText">
                    <Component>
                        <ext:Panel ID="Panel1" runat="server" Width="400">
                            <Items>
                                <ext:TreePanel ID="TreePanel1" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                    Border="false" RootVisible="false">
                                    <Root>
                                        <ext:AsyncTreeNode NodeID="0" Expanded="true" Icon="Note" />
                                    </Root>
                                    <Listeners>
                                        <BeforeLoad Fn="OrgBirimDoldur" />
                                        <Click Handler="#{ddlBirim}.setValue(node.id, node.text, false);#{ddlBirim}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtMuhasebe\', \'txtHarcamaBirimi\', \'txtAmbar\');');" />
                                    </Listeners>
                                </ext:TreePanel>
                            </Items>
                            <Listeners>
                                <Show Handler="try { TreePanel1.getRootNode().findChild('id', ddlBirim.getValue(), true).select(); } catch (e) { }" />
                            </Listeners>
                        </ext:Panel>
                    </Component>
                </ext:DropDownField>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK012 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtKimeGitti" runat="server" CssClass="veriAlan" MaxLength="11"
                    Width="100px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMBRK013 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListePersonel.aspx','txtKimeGitti','txtHarcamaBirimi','lblKimeGittiAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblKimeGittiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK043 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtNereyeGitti" runat="server" CssClass="veriAlan" MaxLength="10"
                    Width="100px"></asp:TextBox></div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMBRK044 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeOda.aspx','txtNereyeGitti','txtHarcamaBirimi','lblNereyeGittiAd');return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" /></div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan">
                <asp:Label ID="lblNereyeGittiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK014 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtBelgeNo" Columns="7" runat="server" MaxLength="6" CssClass="veriAlan"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK015 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtZimmetBelgeNo" Columns="7" runat="server" MaxLength="6" CssClass="veriAlan"></asp:TextBox>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="veriAlan" style="width: 200px">
                <input type="radio" name="zimmet" id="rdKisi" value="kisi" runat="server" checked="true" /><%= Resources.TasinirMal.FRMBRK016 %>
                <input type="radio" name="zimmet" id="rdOrtak" value="ortak" runat="server" /><%= Resources.TasinirMal.FRMBRK017 %>
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK018 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtSicilNo" runat="server" CssClass="veriAlan" MaxLength="40" Columns="30"></asp:TextBox>
            </div>
            <div class="veriAlan">
                <input type="image" alt="<%= Resources.TasinirMal.FRMBRK019 %>" style="height: 14px;
                    width: 16px;" class="veriAlan" onclick="javascript:SicilNoListesiAc();return false;"
                    tabindex="100" src="../App_themes/images/bul1.gif" />
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK020 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:TextBox ID="txtEserAdi" runat="server" CssClass="veriAlan" Width="190px"></asp:TextBox>
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <center>
        <asp:Button runat="server" ID="btnAra" CssClass="dugme" Text="<%$ Resources:TasinirMal, FRMBRK021 %>"
            OnClick="btnAra_Click" Width="150" />
        &nbsp;
        <input type="button" value="<%= Resources.TasinirMal.FRMBRK022 %>" class="dugme"
            onclick="BarkodYaz();" style="width: 100px" />
        <input type="button" value="<%$ Resources:TasinirMal, FRMBRK042 %>" class="dugme"
            onclick="RFIDBarkodYaz();" style="width: 130px" runat="server" id="btnRFIDYazdir"
            visible="false" />
    </center>
    <div class="form">
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK023 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 600px">
                <input type="text" id="txtbEn" maxlength="4" style="display: none; width: 30px; height: 14px;
                    font-size: 8pt; font-family: Verdana, Arial, Helvetica, sans-serif;" runat="server" />
                <asp:CheckBox ID="chkZimmetBilgi" Text="<%$ Resources:TasinirMal, FRMBRK024 %>" runat="server" />
                <asp:CheckBox ID="chkEserBilgi" Text="<%$ Resources:TasinirMal, FRMBRK025 %>" runat="server" />
            </div>
        </div>
        <div class="row">
            <div class="yaziAlan">
                <%= Resources.TasinirMal.FRMBRK030 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <input type="text" id="txtBarkodAciklama" maxlength="100" class="veriAlan" size="70"
                    runat="server" />
            </div>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <div class="row">
        <table width="100%" cellpadding="2" cellspacing="2">
            <tr>
                <td>
                    <a href="javascript:ListeSec(25);">
                        <img src="../App_themes/Images/sec25.gif" style="border: 0" alt="<%= Resources.TasinirMal.FRMBRK031 %>" /></a>
                    <a href="javascript:ListeSec(50);">
                        <img src="../App_themes/Images/sec50.gif" style="border: 0" alt="<%= Resources.TasinirMal.FRMBRK032 %>" /></a>
                    <a href="javascript:ListeSec(100);">
                        <img src="../App_themes/Images/sec100.gif" style="border: 0" alt="<%= Resources.TasinirMal.FRMBRK033 %>" /></a>
                    <a href="javascript:ListeSec(250);">
                        <img src="../App_themes/Images/sec250.gif" style="border: 0" alt="<%= Resources.TasinirMal.FRMBRK034 %>" /></a>
                    <a href="javascript:ListeSec(500);">
                        <img src="../App_themes/Images/sec500.gif" style="border: 0" alt="<%= Resources.TasinirMal.FRMBRK035 %>" /></a>
                    <a href="javascript:ListeSec(-1);">
                        <img src="../App_themes/Images/sec.gif" style="border: 0" alt="<%= Resources.TasinirMal.FRMBRK036 %>" /></a>
                </td>
            </tr>
        </table>
    </div>
    <div style="position: absolute; top: 110px; right: 20px; height: 300px; width: 430px">
        <div class="form">
            <asp:CheckBox ID="chkYeni" runat="server" OnCheckedChanged="btnEtiketKaydet_Click"
                Text="Barkod.ocx dosyası 01.05.2012 tarihinden yeni" AutoPostBack="true" />
            <div id="divYeni" style="display: none">
                <input type="checkbox" id="chkTasarim" runat="server" onclick="TasarimGG()">Kendi tasarım
                    dosyamı kullanacağım</input>
                <div id="divYaziciBilgi">
                    <div class="row" style="display:none">
                        <div class="yaziAlan">
                            Yazıcı</div>
                        <div class="ikiNokta">
                            :</div>
                        <div class="veriAlan">
                            <input type="radio" name="yazici" id="rdZebra" value="zebra" runat="server" />Zebra
                            <input type="radio" name="yazici" id="rdDatamax" value="bixolon" runat="server" />Datamax
                            <input type="radio" name="yazici" id="rdArgox" value="datamax" runat="server" />Argox
                        </div>
                    </div>
                    <div class="row">
                        <div class="yaziAlan">
                            <%= Resources.TasinirMal.FRMBRK026 %></div>
                        <div class="ikiNokta">
                            :</div>
                        <div class="veriAlan">
                            <asp:RadioButton ID="rdBarkodCokKucuk" runat="server" Text="Çok Küçük" GroupName="barkodBoyut" />
                            <asp:RadioButton ID="rdBarkodKucuk" runat="server" Text="<%$ Resources:TasinirMal, FRMBRK027 %>"
                                GroupName="barkodBoyut" />
                            <asp:RadioButton ID="rdBarkodNormal" runat="server" Text="<%$ Resources:TasinirMal, FRMBRK028 %>"
                                GroupName="barkodBoyut" />
                            <asp:RadioButton ID="rdBarkodBuyuk" runat="server" Text="<%$ Resources:TasinirMal, FRMBRK029 %>"
                                GroupName="barkodBoyut" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="yaziAlan">
                            Etiket Bilgisi</div>
                        <div class="ikiNokta">
                            :</div>
                        <div class="veriAlan">
                            Yükseklik:<asp:TextBox ID="txtYukseklik" runat="server" Width="30"></asp:TextBox>
                            Genislik:<asp:TextBox ID="txtGenislik" runat="server" Width="30"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="yaziAlan">
                            Boşluk Bilgisi</div>
                        <div class="ikiNokta">
                            :</div>
                        <div class="veriAlan">
                            Sol Boşluk:<asp:TextBox ID="txtSoldanBosluk" runat="server" Width="30"></asp:TextBox>
                            Üst Boşluk:<asp:TextBox ID="txtUsttenBosluk" runat="server" Width="30"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Button runat="server" ID="btnEtiketKaydet" CssClass="dugme" Text="Kaydet" OnClick="btnEtiketKaydet_Click"
                            Width="150" />
                    </div>
                </div>
                <div id="divTasarimData" style="height: 140px; display: none">
                    <div class="row" style="height: 120px">
                        <div class="yaziAlan">
                            Barkod Tasarım Bilgisi</div>
                        <div class="ikiNokta">
                            :</div>
                        <div class="veriAlan">
                            <asp:TextBox ID="txtData" runat="server" TextMode="MultiLine" Width="250px" Height="100px"
                                CssClass="veriAlan"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Button runat="server" ID="btnKaydetData" CssClass="dugme" Text="Kaydet" Width="150"
                            OnClick="btnKaydetData_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:GridView ID="gvSicilNo" runat="server" CellPadding="1" ForeColor="#333333" GridLines="None"
        Width="96%" AutoGenerateColumns="false">
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Left"
            Height="30px" />
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:TemplateField ItemStyle-Width="30px" HeaderText="Seç">
                <ItemTemplate>
                    <asp:CheckBox ID="chkSecim" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="sicilNo" HeaderText="<%$ Resources:TasinirMal, FRMBRK037 %>"
                ItemStyle-Width="200px" />
            <asp:BoundField DataField="kod" HeaderText="<%$ Resources:TasinirMal, FRMBRK038 %>"
                ItemStyle-Width="170px" />
            <asp:BoundField DataField="ad" HeaderText="<%$ Resources:TasinirMal, FRMBRK039 %>"
                ItemStyle-Width="300px" />
            <asp:BoundField DataField="kimeGitti" HeaderText="<%$ Resources:TasinirMal, FRMBRK040 %>"
                ItemStyle-Width="200px" />
            <asp:BoundField DataField="eserBilgi" HeaderText="<%$ Resources:TasinirMal, FRMBRK041 %>"
                ItemStyle-Width="300px" />
            <asp:BoundField DataField="eSicilNo" HeaderText="<%$ Resources:TasinirMal, FRMBRK045 %>"
                ItemStyle-Width="100px" />
        </Columns>
    </asp:GridView>
    <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    <input type="hidden" id="hdnYazilimAdi" runat="server" />
    <input type="hidden" id="hdnRFIDYaziciURL" runat="server" />
    </form>
</body>
</html>
