<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TasinirIslemSorguIslem.aspx.cs" Inherits="TasinirMal.TasinirIslemSorguIslem" %>
<%@ Register Assembly="Ext1.Net" Namespace="Ext1.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="../App_themes/Form.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">
        function DegerAt(yil, muhasebeKod, harcamaBirimKod, fisNo, islem)
        {
            document.getElementById('txtYil').value=yil;
            document.getElementById('txtMuhasebeKod').value=muhasebeKod;
            document.getElementById('txtHarcamaBirimKod').value=harcamaBirimKod;
            document.getElementById('txtFisNo').value=fisNo;
            document.getElementById('txtIslem').value=islem;
        }
        
        function SubmitForm()
        {
            document.forms[0].submit();
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <input type="hidden" id="txtYil" runat="server" />
        <input type="hidden" id="txtMuhasebeKod" runat="server" />
        <input type="hidden" id="txtHarcamaBirimKod" runat="server" />
        <input type="hidden" id="txtFisNo" runat="server" />
        <input type="hidden" id="txtIslem" runat="server" />
    </form>
    <script language="javascript" type="text/javascript">
        try
        {parent.HideProgress();}
        catch (e){}

    </script>
</body>
</html>