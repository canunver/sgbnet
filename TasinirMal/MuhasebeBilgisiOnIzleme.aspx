<%@ Page Language="C#" Inherits="TasinirMal.MuhasebeBilgisiOnIzleme " CodeBehind="MuhasebeBilgisiOnIzleme.aspx.cs" %>

<%@ Register TagPrefix="ext" Namespace="Ext1.Net" Assembly="Ext1.Net" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Muhasebe İşlemleri</title>
    <script type="text/javascript">
        var JsonGoster = function (islemRefNo) {
            var jsonStr = "{\"muhasebeIslemiList\": [ " + strMuhasebe.data.get(islemRefNo).data.JSON + "]}";
            var jsonObj = JSON.parse(jsonStr);
            var json = JSON.stringify(jsonObj, null, '\t');
            $("pre").text(json);
            winJson.show();
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Viewport ID="Viewport1" runat="server" Layout="BorderLayout" StyleSpec="background-color: transparent;">
            <Items>
                <ext:GridPanel ID="grdListe" runat="server" Region="Center" StripeRows="true" Header="false"
                    TrackMouseOver="true" Border="true" Margins="5 5 5 0" Split="true" AutoExpandColumn="ISLEMCINSI"
                    Cls="gridExt">
                    <Store>
                        <ext:Store runat="server" ID="strMuhasebe">
                            <Reader>
                                <ext:JsonReader IDProperty="ISLEMREFNO">
                                    <Fields>
                                        <ext:RecordField Name="ISLEMREFNO" />
                                        <ext:RecordField Name="ISLEMCINSI" />
                                        <ext:RecordField Name="SERVIS" />
                                        <ext:RecordField Name="DURUM" />
                                        <ext:RecordField Name="ISLEMYAPAN" />
                                        <ext:RecordField Name="ISLEMTARIH" Type="Date" />
                                        <ext:RecordField Name="JSON" />
                                    </Fields>
                                </ext:JsonReader>
                            </Reader>
                        </ext:Store>
                    </Store>
                    <ColumnModel>
                        <Columns>
                            <ext:RowNumbererColumn />
                            <ext:TemplateColumn ColumnID="ISLEMCINSI" DataIndex="ISLEMCINSI" Header="İşlem Cinsi" Align="Left"
                                Width="220" Groupable="false" Fixed="true" Hideable="false">
                                <Template ID="Template1" runat="server">
                                    <Html>
                                        <a href="javascript:JsonGoster('{ISLEMREFNO}')">{ISLEMCINSI}</a>
                                    </Html>
                                </Template>
                            </ext:TemplateColumn>
                            <%--<ext:Column DataIndex="ISLEMCINSI" ColumnID="ISLEMCINSI" Header="İşlem Cinsi" Width="180" />--%>
                            <ext:Column DataIndex="SERVIS" ColumnID="SERVIS" Header="Servis" Width="100" />
                            <ext:Column DataIndex="ISLEMYAPAN" ColumnID="ISLEMYAPAN" Header="İşlem Yapan" Width="200" />
                            <ext:DateColumn DataIndex="ISLEMTARIH" ColumnID="ISLEMTARIH" Header="İşlem Tarihi" Width="200" Format="dd.m.Y HH:mm:ss" Fixed="true" />
                        </Columns>
                    </ColumnModel>
                    <BottomBar>
                        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="500" HideRefresh="true">
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
                    <View>
                        <ext:GridView ID="GridView1" runat="server" />
                    </View>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
        <ext:Window ID="winJson" runat="server" Title="JSON" Width="600" Height="550"
            Modal="true" Hidden="true" Padding="5" Layout="FitLayout" PaddingSummary="20" AutoScroll="true">
            <Content>
                <pre id="json"></pre>
            </Content>
        </ext:Window>
    </form>
</body>
</html>
