function EkSorguSaklaGoster() {
    hideshow('divEkSorgu');
    if (document.getElementById('divEkSorgu').style.display == "")
        document.getElementById('btnEkSorgu').innerHTML = res_FRMJSC002;
    else
        document.getElementById('btnEkSorgu').innerHTML = res_FRMJSC003;
}

function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    parent.App.tabPanelAna.setActiveTab("panelIslem");
    parent.App.panelIslem.getBody().BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo);
}

function TarihceGoster(yil, muhasebe, harcamaBirimi, belgeNo) {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    yil = document.getElementById('ddlYil').value;
    var belgeTur = document.getElementById('hdnBelgeTur').value;

    showPopWin("BelgeTarihce.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&harcamaBirimi=" + harcamaBirimi + "&belgeNo=" + belgeNo + "&belgeTur=" + belgeTur + "&tarihceTur=zimmet", 500, 350, true, null);
}

function IslemYap(islemTip) {
    if (!OnayAl(islemTip))
        return;

    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    var yil = "";
    var muhasebeKod = "";
    var harcamaBirimKod = "";
    var belgeNo = "";
    var belgeTur = "";

    var kontrol = document.getElementById('gvBelgeler');

    if (kontrol) {
        for (var i = 1; i < kontrol.rows.length; i++) {
            //	        var chkId = "gvBelgeler$ctl";
            //	        var chkTemp ="";
            //	        if(i<9)
            //	            chkTemp = "0" + (i+1).toString();
            //	        else
            //	            chkTemp = (i+1).toString();
            //	        
            //	        chkId = chkId + chkTemp + "$chkSecim";
            //	        if (document.getElementById(chkId).checked)
            if (kontrol.rows[i].cells[0].children[0].checked) {
                if (yil != "") yil = yil + ";";
                yil = yil + GridHucreDegerAl(kontrol.rows[i].cells[2]);
                if (muhasebeKod != "") muhasebeKod = muhasebeKod + ";";
                muhasebeKod = muhasebeKod + GridHucreDegerAl(kontrol.rows[i].cells[6]);
                if (harcamaBirimKod != "") harcamaBirimKod = harcamaBirimKod + ";";
                harcamaBirimKod = harcamaBirimKod + GridHucreDegerAl(kontrol.rows[i].cells[7]);
                if (belgeNo != "") belgeNo = belgeNo + ";";
                belgeNo = belgeNo + GridHucreDegerAl(kontrol.rows[i].cells[1]);
            }
        }
    }

    if (belgeNo == "") {
        HideProgress();
        alert(res_FRMJSC004);
        return;
    }

    belgeTur = document.getElementById('hdnBelgeTur').value;

    var fr = frames["frmIslem"];
    if (!fr)
        fr = frames[0];

    if (isIE()) {
        fr.DegerAt(yil, muhasebeKod, harcamaBirimKod, belgeNo, islemTip, belgeTur);
        fr.SubmitForm();
    }
    else {
        fr.contentWindow.DegerAt(yil, muhasebeKod, harcamaBirimKod, belgeNo, islemTip, belgeTur);
        fr.contentWindow.SubmitForm();
    }
}

function OnayAl(islemTip) {
    var button;

    if (islemTip != '') {
        var deger = BelgeOnayAl(islemTip);

        if (deger && islemTip != "Yazdir")
            ShowProgress();

        return deger;
    }
}

function ListeSec() {
    var kontrol = document.getElementById('gvBelgeler');
    if (kontrol) {
        secim = kontrol.rows[0].cells[0].children[0].checked;
        for (var i = 1; i < kontrol.rows.length; i++)
            kontrol.rows[i].cells[0].children[0].checked = secim;
    }
}