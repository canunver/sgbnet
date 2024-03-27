<%@ Page Language="C#" CodeBehind="TanimDemirbasResimGoster.aspx.cs" Inherits="TasinirMal.TanimDemirbasResimGoster" %>

<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Resim Göster</title>
    <script type="text/javascript">
        var resize = function (image, factor) {
            if (!factor || !image.complete) {
                return;
            }

            var orgSize = image.getOriginalSize();

            factor = parseFloat(factor);
            image.setSize(parseInt(orgSize.width * factor), parseInt(orgSize.height * factor));
        }

        var newFactor = function (combo, dir) {
            var index = combo.getSelectedIndex(),
                count = combo.store.getCount();

            index += dir;

            if (index >= 0 && index < count) {
                combo.setValueAndFireSelect(combo.store.getAt(index).get(combo.valueField));
            }
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="border">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Border="false" Region="North" Height="35" Padding="5">
                <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server" Flat="true">
                        <Items>
                            <ext:Button ID="Button1" runat="server" Icon="ZoomOut">
                                <Listeners>
                                    <Click Handler="newFactor(#{SizesCombo}, -1);" />
                                </Listeners>
                            </ext:Button>
                            <ext:ComboBox ID="SizesCombo" runat="server" TriggerIcon="SimpleMagnify" Editable="false">
                                <Items>
                                    <ext:ListItem Text="5%" Value="0.05" />
                                    <ext:ListItem Text="10%" Value="0.1" />
                                    <ext:ListItem Text="12%" Value="0.12" />
                                    <ext:ListItem Text="16%" Value="0.16" />
                                    <ext:ListItem Text="25%" Value="0.25" />
                                    <ext:ListItem Text="33%" Value="0.33" />
                                    <ext:ListItem Text="50%" Value="0.5" />
                                    <ext:ListItem Text="66%" Value="0.66" />
                                    <ext:ListItem Text="100%" Value="1" />
                                    <ext:ListItem Text="200%" Value="2" />
                                    <ext:ListItem Text="300%" Value="3" />
                                    <ext:ListItem Text="400%" Value="4" />
                                    <ext:ListItem Text="500%" Value="5" />
                                    <ext:ListItem Text="600%" Value="6" />
                                    <ext:ListItem Text="700%" Value="7" />
                                    <ext:ListItem Text="800%" Value="8" />
                                </Items>
                                <SelectedItem Value="1" />
                                <Listeners>
                                    <Select Handler="resize(#{imgResim}, record ? record.get('value') : null);" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:Button ID="Button2" runat="server" Icon="Zoom">
                                <Listeners>
                                    <Click Handler="newFactor(#{SizesCombo}, 1);" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnSagDondur" runat="server" Icon="ArrowRotateClockwise">
                                <DirectEvents>
                                    <Click OnEvent="btnSagDondur_Click">
                                        <EventMask ShowMask="true" MinDelay="20" Msg="Lütfen bekleyin..." />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:Panel>
            <ext:Panel ID="Panel2" runat="server" Border="false" AutoScroll="true" Region="Center"
                Padding="5">
                <Items>
                    <ext:Image ID="imgResim" runat="server" Resizable="true" ImageUrl="">
                        <ResizeConfig ID="ResizeConfig1" runat="server" PreserveRatio="true" HandlesSummary="e se s" />
                        <Listeners>
                            <Complete Handler="newFactor(#{SizesCombo}, 0);" />
                            <ResizerResize Handler="#{SizesCombo}.setValue('');" />
                        </Listeners>
                    </ext:Image>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Viewport>
    </form>
</body>
</html>
