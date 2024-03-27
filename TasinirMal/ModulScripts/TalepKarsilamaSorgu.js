function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo, personelTC) {
    parent.tabPanelAna.setActiveTab("panelIslem");
    parent.panelIslem.getBody().BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo, personelTC);
}

function BelgeAcDepo(yil, muhasebeKod, harcamaBirimiKod, belgeNo, refID) {

    parent.tabPanelAna.setActiveTab("panelIslem");
    parent.panelIslem.getBody().BelgeAcDepo(yil, muhasebeKod, harcamaBirimiKod, belgeNo, refID);
}

var DurumRenderer = function (value) {
    var r = strDurum.getById(value);

    if (r == null || Ext1.isEmpty(r)) {
        return "";
    }

    return r.data.ADI;
};