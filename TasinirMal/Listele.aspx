<%@ Page Language="C#" CodeBehind="Listele.aspx.cs" Inherits="TasinirMal.Listele" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="JavaScript" type="text/javascript" src="ModulScripts/base64.js?v=1"></script>
    <script type="text/javascript">
        function Calistir(format) {
            var o;
            if (format == 1) o = {};
            else o = { isUpload: true };
            var tut = txtSql.getValue();
            txtSql.setValue('');
            Ext1.net.DirectMethods.SqlCalistir("Komut:" + Base64.encode(tut), cmbBaglanti.getValue(), format, o);
            txtSql.setValue(tut);
        }

        function Calistir1() {
            Calistir(1);
        }

        function Calistir2() {
            Calistir(2);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Viewport runat="server" StyleSpec="background-color: transparent;" Layout="BorderLayout">
            <Items>
                <ext:FormPanel runat="server" Region="North" HideLabels="true" Height="240" Collapsible="true">
                    <Items>
                        <ext:TextArea runat="server" ID="txtSql" AnchorHorizontal="90%" Height="180" Hidden="true">
                        </ext:TextArea>
                        <ext:CompositeField runat="server">
                            <Items>
                                <ext:ComboBox ID="cmbBaglanti" TabIndex="98" runat="server" Width="200" Editable="true" Hidden="true">
                                    <Items>
                                        <ext:ListItem Text="BaglantiSatiriATM" />
                                        <ext:ListItem Text="BaglantiSatiriBHF" />
                                        <ext:ListItem Text="BaglantiSatiriBHM" />
                                        <ext:ListItem Text="BaglantiSatiriBHMv2" />
                                        <ext:ListItem Text="BaglantiSatiriBKD" />
                                        <ext:ListItem Text="BaglantiSatiriBUM" />
                                        <ext:ListItem Text="BaglantiSatiriEGM" />
                                        <ext:ListItem Text="BaglantiSatiriEKG" />
                                        <ext:ListItem Text="BaglantiSatiriEVR" />
                                        <ext:ListItem Text="BaglantiSatiriGEL" />
                                        <ext:ListItem Text="BaglantiSatiriGRV" />
                                        <ext:ListItem Text="BaglantiSatiriHRC" />
                                        <ext:ListItem Text="BaglantiSatiriKHM" />
                                        <ext:ListItem Text="BaglantiSatiriIKY" />
                                        <ext:ListItem Text="BaglantiSatiriYAT" />
                                        <ext:ListItem Text="BaglantiSatiriKYM" />
                                        <ext:ListItem Text="BaglantiSatiriKYS" />
                                        <ext:ListItem Text="BaglantiSatiriOMK" />
                                        <ext:ListItem Text="BaglantiSatiriPB" />
                                        <ext:ListItem Text="BaglantiSatiriPEB" />
                                        <ext:ListItem Text="BaglantiSatiriPEBY" />
                                        <ext:ListItem Text="BaglantiSatiriPER" />
                                        <ext:ListItem Text="BaglantiSatiriPERv2" />
                                        <ext:ListItem Text="BaglantiSatiriPYS" />
                                        <ext:ListItem Text="BaglantiSatiriRCV" />
                                        <ext:ListItem Text="BaglantiSatiriSTR" />
                                        <ext:ListItem Text="BaglantiSatiriSTRY" />
                                        <ext:ListItem Text="BaglantiSatiriTMM" />
                                        <ext:ListItem Text="BaglantiSatiriYAT" />
                                    </Items>
                                </ext:ComboBox>
                                <ext:Button runat="server" Text="Listele" Icon="PageLightning">
                                    <Listeners>
                                        <Click Fn="Calistir1" />
                                    </Listeners>
                                </ext:Button>
                                <ext:Button ID="Button1" runat="server" Text="Excel" Icon="PageExcel">
                                    <Listeners>
                                        <Click Fn="Calistir2" />
                                    </Listeners>
                                </ext:Button>
                            </Items>
                        </ext:CompositeField>
                    </Items>
                </ext:FormPanel>
                <ext:Panel runat="server" Region="Center" ID="pnlSonuc" AutoScroll="true">
                </ext:Panel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
