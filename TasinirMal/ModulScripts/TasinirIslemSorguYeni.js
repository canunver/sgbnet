
function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    parent.tabPanelAna.setActiveTab("panelIslem");
    parent.panelIslem.getBody().BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo);
}

function TarihceGoster(yil, muhasebe, harcamaBirimi, belgeNo) {
    try { document.getElementById('lblHata').innerHTML = ""; }
    catch (e) { }

    showPopWin("BelgeTarihce.aspx?menuYok=1&yil=" + yil + "&muhasebe=" + muhasebe + "&harcamaBirimi=" + harcamaBirimi + "&belgeNo=" + belgeNo, 500, 350, true, null);
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

    var kontrol = document.getElementById('gvBelgeler');

    if (kontrol) {
        for (var i = 1; i < kontrol.rows.length; i++) {
            //	        var chkId = "gvBelgeler_ctl";
            //	        var chkTemp ="";
            //	        if(i<9)
            //	            chkTemp = "0" + (i+1).toString();
            //	        else
            //	            chkTemp = (i+1).toString();
            //	        
            //	        chkId = chkId + chkTemp + "_chkSecim";
            //	        if (document.getElementById(chkId).checked)
            if (kontrol.rows[i].cells[0].childNodes[0].checked) {
                if (yil != "") yil = yil + ";";
                yil = yil + kontrol.rows[i].cells[2].innerText;
                if (muhasebeKod != "") muhasebeKod = muhasebeKod + ";";
                muhasebeKod = muhasebeKod + kontrol.rows[i].cells[4].innerText;
                if (harcamaBirimKod != "") harcamaBirimKod = harcamaBirimKod + ";";
                harcamaBirimKod = harcamaBirimKod + kontrol.rows[i].cells[5].innerText;
                if (belgeNo != "") belgeNo = belgeNo + ";";
                belgeNo = belgeNo + kontrol.rows[i].cells[1].innerText;
            }
        }
    }
  
    if (belgeNo == "") {
        alert("Listeden işlem yapılacak belge seçilmemiş.");
        return;
    }
 
    var fr = frames["frmIslem"];
    fr.DegerAt(yil, muhasebeKod, harcamaBirimKod, belgeNo, islemTip);
    fr.SubmitForm();
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
        secim = kontrol.rows[0].cells[0].childNodes[0].checked;
        for (var i = 1; i < kontrol.rows.length; i++)
            kontrol.rows[i].cells[0].childNodes[0].checked = secim;
    }
}