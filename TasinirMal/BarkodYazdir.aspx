<%@ Page Language="C#" CodeBehind="BarkodYazdir.aspx.cs" Inherits="TasinirMal.BarkodYazdir" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/BarkodYazdir.js?v=26"></script>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/base64.js?v=25"></script>
    <script language="javascript" type="text/javascript">
        var AciklamaSecimi = function (a, b, c, d) {
            if (c == "4")
                txtAciklama.show();
            else
                txtAciklama.hide();
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="strListe" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="SICILNO">
                    <Fields>
                        <ext:RecordField Name="TIP" />
                        <ext:RecordField Name="SICILNO" />
                        <ext:RecordField Name="HESAPPLANKOD" />
                        <ext:RecordField Name="HESAPPLANADI" />
                        <ext:RecordField Name="ZIMMETKISI" />
                        <ext:RecordField Name="ESERBILGISI" />
                        <ext:RecordField Name="ESKISICILNO" />
                        <ext:RecordField Name="MUHASEBEADI" />
                        <ext:RecordField Name="HARCAMABIRIMADI" />
                        <ext:RecordField Name="AMBARADI" />
                        <ext:RecordField Name="SASENO" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Store ID="strIslemTip" runat="server">
            <Reader>
                <ext:JsonReader IDProperty="KOD">
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Hidden ID="hdnYazdirmaTur" runat="server"></ext:Hidden>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:PropertyGrid ID="pgFiltre" runat="server" Region="West" Split="true" Border="true" Width="250">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnSorguTemizle" runat="server" Text="Temizle" Icon="PageWhite">
                                            <DirectEvents>
                                                <Click OnEvent="btnSorguTemizle_Click">
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:Button ID="btnListe" runat="server" Text="<%$Resources:Evrak,FRMSRG119 %>" Icon="ApplicationGo">
                                            <DirectEvents>
                                                <Click OnEvent="btnListe_Click" Timeout="2400000">
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Source>
                                <ext:PropertyGridParameter Name="prpYil" DisplayName="<%$ Resources:TasinirMal, FRMBRK005 %>">
                                    <Editor>
                                        <ext:SpinnerField runat="server">
                                        </ext:SpinnerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpMuhasebe" DisplayName="<%$ Resources:TasinirMal, FRMBRK006 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpMuhasebe',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpHarcamaBirimi" DisplayName="<%$ Resources:TasinirMal, FRMBRK008 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpHarcamaBirimi',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpAmbar" DisplayName="<%$ Resources:TasinirMal, FRMTIS019 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpAmbar',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpBelgeNo" DisplayName="Belge No (TİF, Zimmet)">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpSicilNo" DisplayName="<%$ Resources:TasinirMal, FRMBRK018 %>">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpSicilNo',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpHesapKod" DisplayName="Hesap Kodu">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpHesapKod',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpKisiKod" DisplayName="TC Kimlik No">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpKisiKod',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpOdaKod" DisplayName="Oda Kodu">
                                    <Editor>
                                        <ext:TriggerField runat="server">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="TriggerClickProperty('prpOdaKod',this.triggers[0]);" />
                                            </Listeners>
                                        </ext:TriggerField>
                                    </Editor>
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpEserAdi" DisplayName="<%$ Resources:TasinirMal, FRMBRK020 %>">
                                </ext:PropertyGridParameter>
                                <ext:PropertyGridParameter Name="prpDurumKod" DisplayName="Durumu">
                                    <Renderer Handler="if (value=='1') return 'Ambarda'; else if (value=='2') return 'Zimmette'; else return 'Hepsi';" />
                                    <Editor>
                                        <ext:ComboBox ID="dllDurum" runat="server" Editable="false">
                                            <Items>
                                                <ext:ListItem Text="Hepsi" Value="" />
                                                <ext:ListItem Text="Ambarda" Value="1" />
                                                <ext:ListItem Text="Zimmette" Value="2" />
                                            </Items>
                                        </ext:ComboBox>
                                    </Editor>
                                </ext:PropertyGridParameter>
                            </Source>
                            <Listeners>
                                <Render Handler="function() { this.getStore().sortInfo = undefined; this.getColumnModel().config[0].sortable = false;this.getColumnModel().config[1].sortable = false;}" />
                                <SortChange Handler="this.getStore().sortInfo = undefined;" />
                            </Listeners>
                            <View>
                                <ext:GridView ID="GridView1" ForceFit="true" runat="server" />
                            </View>
                        </ext:PropertyGrid>
                        <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" StoreID="strListe"
                            Border="true" Cls="gridExt">
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:Button ID="btnBarkodYazdir" runat="server" Text="<%$Resources:TasinirMal,FRMBRK022%>" Icon="PrinterGo">
                                            <Listeners>
                                                <Click Handler="BarkodYaz();" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:Button ID="btnBarkodAyar" runat="server" Text="Ayarlar" Icon="Cog">
                                            <Listeners>
                                                <Click Handler="wndBarkodAyar.show();" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ToolbarSeparator runat="server" />
                                        <ext:LinkButton runat="server" Text="Yardım">
                                            <Listeners>
                                                <Click Handler="JavaScript:showPopWin('BarkodYardim.htm', 700, 600, true, null);" />
                                            </Listeners>
                                        </ext:LinkButton>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <ColumnModel ID="ColumnModel1" runat="server">
                                <Columns>
                                    <ext:Column ColumnID="TIP" DataIndex="TIP" Width="70" Header="Belge Türü" />
                                    <ext:Column ColumnID="SICILNO" DataIndex="SICILNO" Width="160" Header="Sicil No" />
                                    <ext:Column ColumnID="HESAPPLANKOD" DataIndex="HESAPPLANKOD" Width="160" Header="Hesap Planı Kod" />
                                    <ext:Column ColumnID="HESAPPLANADI" DataIndex="HESAPPLANADI" Width="225" Header="Hesap Planı Adı" />
                                    <ext:Column ColumnID="ZIMMETKISI" DataIndex="ZIMMETKISI" Width="160" Header="Zimmetli Kişi" />
                                    <ext:Column ColumnID="SASENO" DataIndex="SASENO" Width="160" Header="Seri No" />
                                    <ext:Column ColumnID="ESERBILGISI" DataIndex="ESERBILGISI" Width="160" Header="Bilgiler" />
                                    <ext:Column ColumnID="ESKISICILNO" DataIndex="ESKISICILNO" Width="160" Header="Eski Sicil No" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:CheckboxSelectionModel runat="server" />
                            </SelectionModel>
                            <BottomBar>
                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="500" HideRefresh="true"
                                    StoreID="strListe">
                                    <Items>
                                        <ext:Label ID="Label1" runat="server" Text="Satır sayısı:" />
                                        <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                        <ext:ComboBox ID="cmbPageSize" runat="server" Width="60">
                                            <Items>
                                                <ext:ListItem Text="50" />
                                                <ext:ListItem Text="100" />
                                                <ext:ListItem Text="250" />
                                                <ext:ListItem Text="500" />
                                                <ext:ListItem Text="1000" />
                                                <ext:ListItem Text="2500" />
                                                <ext:ListItem Text="5000" />
                                            </Items>
                                            <SelectedItem Value="500" />
                                            <Listeners>
                                                <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:PagingToolbar>
                            </BottomBar>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <ext:Window ID="wndBarkodAyar" runat="server" Title="Ayarlar" Width="500" Height="320"
            Modal="true" Hidden="true" Padding="5">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button ID="btnEtiketKaydet" runat="server" Text="Kaydet" Icon="Disk">
                            <DirectEvents>
                                <Click OnEvent="btnEtiketKaydet_Click" Timeout="50000">
                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:RadioGroup ID="rdGrup" runat="server" FieldLabel="Barkod Boyutu" BoxMaxWidth="400">
                    <Items>
                        <ext:Radio ID="rdCokKucuk" runat="server" BoxLabel="Çok Küçük" Width="50" />
                        <ext:Radio ID="rdKucuk" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMBRK027 %>" Width="50" />
                        <ext:Radio ID="rdNormal" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMBRK028 %>" Width="50" />
                        <ext:Radio ID="rdBuyuk" runat="server" BoxLabel="<%$ Resources:TasinirMal, FRMBRK029 %>" Width="50" />
                    </Items>
                </ext:RadioGroup>
                <ext:CompositeField ID="cmpEtiketBilgi" runat="server" FieldLabel="Etiket Bilgisi" Height="50">
                    <Items>
                        <ext:TextField ID="txtYukseklik" runat="server" Note="Yükseklik" Width="50" />
                        <ext:TextField ID="txtGenislik" runat="server" Note="Genişlik" Width="50" />
                    </Items>
                </ext:CompositeField>
                <ext:CompositeField ID="cmpBoslukBilgi" runat="server" FieldLabel="Boşluk Bilgisi" Height="50">
                    <Items>
                        <ext:TextField ID="txtSolBosluk" runat="server" Note="Sol Boşluk" Width="50" />
                        <ext:TextField ID="txtUstBosluk" runat="server" Note="Üst Boşluk" Width="50" />
                    </Items>
                </ext:CompositeField>
                <ext:ComboBox ID="cmbEkBilgi" runat="server" FieldLabel="Ek Bilgi Yazdırma" Width="400">
                    <Items>
                        <ext:ListItem Text="Ek bilgi yazdırma yapılmayacak" Value="1" />
                        <ext:ListItem Text="Seri No bilgisi yaz" Value="5" />
                        <ext:ListItem Text="Zimmet verilen kişi adını yaz" Value="2" />
                        <ext:ListItem Text="Özellik Bilgilerini yaz" Value="3" />
                        <ext:ListItem Text="Açıklama alanındaki bilgileri yaz" Value="4" />
                    </Items>
                    <Listeners>
                        <Select Fn="AciklamaSecimi" />
                    </Listeners>
                </ext:ComboBox>

                <ext:TextField ID="txtAciklama" runat="server" FieldLabel="<%$ Resources:TasinirMal, FRMBRK030 %>" Width="350" Hidden="true" />
            </Items>
        </ext:Window>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
