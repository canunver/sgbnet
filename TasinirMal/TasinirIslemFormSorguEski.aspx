<%@ Page Language="C#" CodeBehind="TasinirIslemFormSorguEski.aspx.cs" Inherits="TasinirMal.TasinirIslemFormSorguEski" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <link href="../App_themes/TakvimYeni.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript" src="../Script/TakvimYeni.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/hideShow.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/XmlHttp.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="../script/paraFormat.js?mc=03022015"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/TasinirIslemSorgu.js?mc=03102016&v=2012_01_05"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/Ortak.js?v=2"></script>
</head>
<body>
    <form id="form" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server">
        <Listeners>
            <DocumentReady Handler="ddlBirim.onTriggerClick();Panel1.hide();" />
        </Listeners>
    </ext:ResourceManager>
    <div class="form" style="margin-top: -2px;">
        <div class="row">
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTIS013 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan" style="width: 250px;">
                <asp:DropDownList ID="ddlYil" runat="server" CssClass="veriAlanDDL">
                </asp:DropDownList>
            </div>
            <div class="kolonArasi">
            </div>
            <div class="yaziAlanK">
                <%= Resources.TasinirMal.FRMTIS014 %></div>
            <div class="ikiNokta">
                :</div>
            <div class="veriAlan">
                <asp:DropDownList ID="ddlDurum" runat="server" CssClass="veriAlanDDL">
                </asp:DropDownList>
            </div>
        </div>
        <div id="divMHA0" runat="server">
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIS015 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIS016 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtMuhasebe','','lblMuhasebeAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIS017 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                        Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIS018 %>" style="height: 14px;
                        width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtHarcamaBirimi','txtMuhasebe','lblHarcamaBirimiAd');return false;"
                        tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                <div class="kolonArasi">
                </div>
                <div class="veriAlan">
                    <asp:Label ID="lblHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
            </div>
            <div class="row">
                <div class="yaziAlanK">
                    <%= Resources.TasinirMal.FRMTIS019 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                <div class="veriAlan">
                    <input type="image" alt="<%= Resources.TasinirMal.FRMTIS020 %>" style="height: 14px;
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
            <div class="yaziAlanK">
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
        <div id="divEkSorgu">
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS021 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtBelgeNo1" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS022 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtBelgeNo2" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS023 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtBelgeTarih1" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                    <img src="../App_themes/Images/takvim.gif" id="imgBelgeTarih1" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtBelgeTarih1', false, 'dmy', '.');" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS024 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtBelgeTarih2" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                    <img src="../App_themes/Images/takvim.gif" id="imgBelgeTarih2" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtBelgeTarih2', false, 'dmy', '.');" />
                </div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS025 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtDurumTarih1" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                    <img src="../App_themes/Images/takvim.gif" id="imgDurumTarih1" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtDurumTarih1', false, 'dmy', '.');" />
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS026 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtDurumTarih2" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                    <img src="../App_themes/Images/takvim.gif" id="imgDurumTarih2" alt="" style="cursor: pointer;"
                        onclick="JavaScript:displayDatePicker('txtDurumTarih2', false, 'dmy', '.');" />
                </div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS027 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtNereye" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS028 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtKime" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS029 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtTasinirHesapKodu" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox></div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS030 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <asp:TextBox ID="txtNereden" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox></div>
            </div>
            <div class="row">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS031 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:DropDownList ID="ddlIslemTipi" runat="server" CssClass="veriAlanDDL">
                    </asp:DropDownList>
                </div>
                <div class="kolonArasi">
                </div>
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMTIS032 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan" style="width: 250px;">
                    <asp:TextBox ID="txtIslemYapan" runat="server" CssClass="veriAlan" Width="100px"></asp:TextBox></div>
            </div>
            <div id="divMHA1" runat="server">
                <div class="row">
                    <div class="yaziAlan">
                        <%= Resources.TasinirMal.FRMTIS033 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtGonMuhasebe" runat="server" CssClass="veriAlan" MaxLength="5"
                            Width="100px"></asp:TextBox></div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMTIS034 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeMuhasebe.aspx','txtGonMuhasebe','','lblGonMuhasebeAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblGonMuhasebeAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
                </div>
                <div class="row">
                    <div class="yaziAlan">
                        <%= Resources.TasinirMal.FRMTIS035 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtGonHarcamaBirimi" runat="server" CssClass="veriAlan" MaxLength="15"
                            Width="100px"></asp:TextBox></div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMTIS036 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeHarcamaBirimi.aspx','txtGonHarcamaBirimi','txtGonMuhasebe','lblGonHarcamaBirimiAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblGonHarcamaBirimiAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label></div>
                </div>
                <div class="row">
                    <div class="yaziAlan">
                        <%= Resources.TasinirMal.FRMTIS037 %></div>
                    <div class="ikiNokta">
                        :</div>
                    <div class="veriAlan">
                        <asp:TextBox ID="txtGonAmbar" runat="server" CssClass="veriAlan" MaxLength="3" Width="100px"></asp:TextBox></div>
                    <div class="veriAlan">
                        <input type="image" alt="<%= Resources.TasinirMal.FRMTIS038 %>" style="height: 14px;
                            width: 16px;" class="veriAlan" onclick="javascript:ListeAc('ListeAmbar.aspx','txtGonAmbar','txtGonHarcamaBirimi','lblGonAmbarAd');return false;"
                            tabindex="100" src="../App_themes/images/bul1.gif" /></div>
                    <div class="kolonArasi">
                    </div>
                    <div class="veriAlan">
                        <asp:Label ID="lblGonAmbarAd" Width="100%" runat="server" CssClass="veriAlan"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="row" id="divOrg1" runat="server" style="display: none">
                <div class="yaziAlan">
                    <%= Resources.TasinirMal.FRMORG004 %></div>
                <div class="ikiNokta">
                    :</div>
                <div class="veriAlan">
                    <ext:DropDownField ID="ddlBirim2" runat="server" Width="300" TriggerIcon="Ellipsis"
                        Mode="ValueText">
                        <Component>
                            <ext:Panel ID="Panel2" runat="server" Width="400">
                                <Items>
                                    <ext:TreePanel ID="TreePanel2" runat="server" Height="250" AutoWidth="true" AutoScroll="true"
                                        Border="false" RootVisible="false">
                                        <Root>
                                            <ext:AsyncTreeNode NodeID="0" Expanded="true" Icon="Note" />
                                        </Root>
                                        <Listeners>
                                            <BeforeLoad Fn="OrgBirimDoldur" />
                                            <Click Handler="#{ddlBirim2}.setValue(node.id, node.text, false);#{ddlBirim2}.collapse();OrgBirimBul(node, 'MuhasebeHarcamaAmbarBul(\'txtGonMuhasebe\', \'txtGonHarcamaBirimi\', \'txtGonAmbar\');');" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                                <Listeners>
                                    <Show Handler="try { TreePanel2.getRootNode().findChild('id', ddlBirim2.getValue(), true).select(); } catch (e) { }" />
                                </Listeners>
                            </ext:Panel>
                        </Component>
                    </ext:DropDownField>
                </div>
            </div>
        </div>
        <div class="row">
            &nbsp;<a onclick="EkSorguSaklaGoster();return false;" href="#"><img src="..\app_themes\images\gelismisarama.gif"
                alt="" />&nbsp;<b><span id="btnEkSorgu"></span></b></a><script language="javascript"
                    type="text/javascript">                                                                           EkSorguSaklaGoster();</script>
        </div>
        <div class="footer">
            &nbsp;</div>
    </div>
    <table>
        <tr>
            <td>
                <fieldset>
                    <legend>
                        <%= Resources.TasinirMal.FRMTIS039 %></legend>
                    <div style="height: 70px;">
                        <asp:Button ID="btnListe" TabIndex="99" runat="server" CssClass="dugme" Width="80"
                            Text="<%$ Resources:TasinirMal, FRMTIS040 %>" OnClick="btnListe_Click"></asp:Button>&nbsp;
                        <asp:Button ID="btnListeYazdir" runat="server" CssClass="dugme" Width="80" Text="<%$ Resources:TasinirMal, FRMTIS041 %>"
                            OnClick="btnListeYazdir_Click"></asp:Button>
                    </div>
                </fieldset>
            </td>
            <td>
                <fieldset>
                    <legend>
                        <%= Resources.TasinirMal.FRMTIS042 %></legend>
                    <div style="height: 34px;">
                        <input type="button" id="btnIslemYazdir" tabindex="99" class="dugme" style="width: 80px"
                            value="<%= Resources.TasinirMal.FRMTIS043 %>" onclick="IslemYap('Yazdir');return false;" />&nbsp;
                        <input type="button" id="btnIslemOnayla" tabindex="99" class="dugme" style="width: 80px"
                            value="<%= Resources.TasinirMal.FRMTIS044 %>" onclick="IslemYap('Onay');return false;" />
                    </div>
                    <div style="height: 36px;">
                        <input type="button" id="btnIslemOnayKaldir" tabindex="99" class="dugme" style="width: 80px"
                            value="<%= Resources.TasinirMal.FRMTIS045 %>" onclick="IslemYap('OnayKaldir');return false;" />&nbsp;
                        <input type="button" id="btnIslemIptal" tabindex="99" class="dugme" style="width: 80px"
                            value="<%= Resources.TasinirMal.FRMTIS046 %>" onclick="IslemYap('İptal');return false;" />
                    </div>
                </fieldset>
            </td>
        </tr>
    </table>
    <div class="row">
        <div style="overflow: auto; width: 98%; height: 450px; border: solid 1px gray;">
            <asp:GridView ID="gvBelgeler" runat="server" CellPadding="1" ForeColor="#333333"
                GridLines="None" Width="1000px" AutoGenerateColumns="false">
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                    Height="30px" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:TemplateField ItemStyle-Width="30px" HeaderText="<input type='checkbox' onclick='javascript:ListeSec();' />">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSecim" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="fisno" Visible="false" HeaderText="BelgeNoGizli"></asp:BoundField>
                    <asp:TemplateField HeaderText="<%$ Resources:TasinirMal, FRMTIS047 %>">
                        <ItemStyle Width="50px" BorderColor="LightGray" HorizontalAlign="Left" />
                        <ItemTemplate>
                            <a href="javascript:BelgeAc('<%# DataBinder.Eval(Container.DataItem, "yil")%>','<%# DataBinder.Eval(Container.DataItem, "muhasebe")%>','<%# DataBinder.Eval(Container.DataItem, "harcamaBirimi")%>','<%# DataBinder.Eval(Container.DataItem, "fisno")%>');">
                                <%# DataBinder.Eval(Container.DataItem, "fisno")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="yil" HeaderText="<%$ Resources:TasinirMal, FRMTIS013 %>"
                        ItemStyle-Width="40px" />
                    <asp:BoundField DataField="fistarih" HeaderText="<%$ Resources:TasinirMal, FRMTIS048 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="muhasebe" HeaderText="<%$ Resources:TasinirMal, FRMTIS049 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="harcamaBirimi" HeaderText="<%$ Resources:TasinirMal, FRMTIS050 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="harcamaBirimiAd" HeaderText="<%$ Resources:TasinirMal, FRMTIS051 %>"
                        ItemStyle-Width="180px" />
                    <asp:BoundField DataField="ambar" HeaderText="<%$ Resources:TasinirMal, FRMTIS052 %>"
                        ItemStyle-Width="120px" />
                    <asp:BoundField DataField="islemtipi" HeaderText="<%$ Resources:TasinirMal, FRMTIS031 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="durum" Visible="false" HeaderText="DurumGizli"></asp:BoundField>
                    <asp:TemplateField HeaderText="Durum">
                        <ItemStyle Width="80px" BorderColor="LightGray" />
                        <ItemTemplate>
                            <a href="javascript:TarihceGoster('<%# DataBinder.Eval(Container.DataItem, "yil")%>','<%# DataBinder.Eval(Container.DataItem, "muhasebe")%>','<%# DataBinder.Eval(Container.DataItem, "harcamaBirimi")%>','<%# DataBinder.Eval(Container.DataItem, "fisno")%>');">
                                <%# DataBinder.Eval(Container.DataItem, "durum")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="islemTarih" HeaderText="<%$ Resources:TasinirMal, FRMTIS053 %>"
                        ItemStyle-Width="80px" />
                    <asp:BoundField DataField="islemYapan" HeaderText="<%$ Resources:TasinirMal, FRMTIS054 %>"
                        ItemStyle-Width="80px" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div id="divBekle" class="bekleKutusu">
        <br />
        <b>
            <%= Resources.TasinirMal.FRMTIS055 %></b>
        <br />
        <br />
        <img alt="<%= Resources.TasinirMal.FRMTIS056 %>" src="../App_themes/images/loading.gif" />
        <br />
        <br />
    </div>
    <iframe id="frmIslem" src="TasinirIslemSorguIslem.aspx?kutuphane=<%=Request["kutuphane"]+""%>&muze=<%=Request["muze"]+""%>"
        frameborder="0" scrolling="no" width="1" height="1"></iframe>
    </form>
</body>
</html>
