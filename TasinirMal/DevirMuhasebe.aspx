<%@ Page Language="C#" CodeBehind="DevirMuhasebe.aspx.cs" Inherits="TasinirMal.DevirMuhasebe" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form" runat="server">
        <asp:Literal ID="ltlUstBolum" runat="server"></asp:Literal>
        <ext:ResourceManager ID="ResourceManager1" runat="server">
            <Listeners>
                <DocumentReady Handler="AlanIsimleriniYaz();" />
            </Listeners>
        </ext:ResourceManager>
        <ext:Hidden ID="hdnKod" runat="server" />
        <ext:Store ID="strYil" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="KOD" />
                        <ext:RecordField Name="ADI" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="tabPanelAna" runat="server" Region="Center" StyleSpec="background-color:white;padding:10px"
                    Border="false" Margins="104 20 10 20" Layout="BorderLayout">
                    <Items>
                        <ext:FormPanel ID="pnlTanim" runat="server" Region="Center" Padding="10" LabelWidth="150">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <ext:Button ID="btnOlustur" runat="server" Text="Fiş Oluştur" Icon="Disk">
                                            <DirectEvents>
                                                <Click OnEvent="btnOlustur_Click">
                                                    <Confirmation ConfirmRequest="true" Title="Onay" Message="Onaylıyor musunuz?" />
                                                    <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                                <ext:SpinnerField ID="txtYil" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMHST002 %>" Width="60" />
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtMuhasebe" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMC011 %>" Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblMuhasebeAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField runat="server">
                                    <Items>
                                        <ext:TriggerField ID="txtHarcamaBirimi" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMC013 %>" Width="120">
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Fn="TriggerClick" />
                                                <Change Fn="TriggerChange" />
                                            </Listeners>
                                        </ext:TriggerField>
                                        <ext:Label ID="lblHarcamaBirimiAd" runat="server" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:ComboBox ID="ddlTur" runat="server" Editable="false" FieldLabel="İşlem Tür" SelectOnFocus="true" Width="200">
                                    <Items>
                                        <ext:ListItem Text="Devir İşlemleri" Value="500" />
                                        <ext:ListItem Text="Açılış (Giriş)" Value="49" />
                                        <ext:ListItem Text="Satın Alma (Giriş)" Value="1" />
                                        <ext:ListItem Text="Bağış (Giriş)" Value="2" />
                                        <ext:ListItem Text="Sayım Fazlası (Giriş)" Value="3" />
                                        <ext:ListItem Text="İade (Giriş)" Value="4" />
                                        <ext:ListItem Text="Üretilen (Giriş)" Value="6" />
                                        <ext:ListItem Text="Devir Kurum (Giriş)" Value="7" />
                                        <ext:ListItem Text="Dağıtım İade (Giriş)" Value="8" />
                                        <ext:ListItem Text="Tüketim (Çıkış)" Value="50" />
                                        <ext:ListItem Text="Bağış (Çıkış)" Value="52" />
                                        <ext:ListItem Text="Satış (Çıkış)" Value="53" />
                                        <ext:ListItem Text="Kullanılmaz Hal (Çıkış)" Value="54" />
                                        <ext:ListItem Text="Hurda (Çıkış)" Value="55" />
                                        <ext:ListItem Text="Devir Kurum (Çıkış)" Value="56" />
                                        <ext:ListItem Text="Sayım Noksanı (Çıkış)" Value="57" />
                                        <ext:ListItem Text="Dağıtım İade (Çıkış)" Value="59" />
                                    </Items>
                                </ext:ComboBox>
                                <ext:ComboBox ID="ddlHesap" runat="server" Editable="false" FieldLabel="Malzeme Türü" SelectOnFocus="true" Width="200">
                                    <Items>
                                        <ext:ListItem Text="Bütün Malzemeler" Value="" />
                                        <ext:ListItem Text="Dayanıklı" Value="25" />
                                        <ext:ListItem Text="Tüketim" Value="150" />
                                    </Items>
                                </ext:ComboBox>
                                <ext:DateField ID="txtTarih1" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMC019 %>" Width="90" />
                                <ext:DateField ID="txtTarih2" runat="server" FieldLabel="<%$Resources:TasinirMal,FRMTMC020 %>" Width="90" />
                                <ext:TextField ID="txtBelgeNo" runat="server" FieldLabel="Muhasebe Belge No" AutoWidth="true" Note="Oluşturulacak Muhasebe Belgesinin, önceden oluşturulmuş bir belgenin üzerine yazılmasını istiyorsanız belge numarasını yazın. Yeni belge için boş bırakın." Width="90" />
                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Items>
        </ext:Viewport>
        <asp:Literal ID="ltlAltBolum" runat="server"></asp:Literal>
    </form>
</body>
</html>
