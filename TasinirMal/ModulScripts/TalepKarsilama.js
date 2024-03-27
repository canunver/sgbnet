function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo, personelTC) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('txtBelgeNo').value = belgeNo;
    document.getElementById('txtPersonel').value = personelTC;
    document.getElementById('btnListele').click();
}

function BelgeAcDepo(yil, muhasebeKod, harcamaBirimiKod, belgeNo, refID) {
    if (refID != undefined && refID != "")
        hdnRefId.setValue(refID);

    try {
        txtMuhasebeKod.setValue(muhasebeKod);
        txtHarcamaKod.setValue(harcamaBirimiKod);
        txtNumara.setValue(belgeNo);

    } catch (e) {

    }

    btnListele.fireEvent("click")
}

var hesapPlaniPenceresi = null;

function NodeSecildiYerel(node) {
    FPDegerAta_HesapPlanListe(node, document.getElementById('fpL'), 0, document.getElementById("hdnHesapAdKolon").value);
}

function HesapPlaniGoster() {
    if (hesapPlaniPenceresi == null || hesapPlaniPenceresi.closed) {
        hesapPlaniPenceresi = window.open("ListeHesapPlani.aspx?menuYok=1", "hesapPlani", "resizable,status,width=480,height=410");
        hesapPlaniPenceresi.focus();
    }
    else
        hesapPlaniPenceresi.focus();

    hesapPlaniPenceresi.moveTo(screen.width - 505, 30);
}

function HesapPlaniKapat() {
    if (hesapPlaniPenceresi != null)
        hesapPlaniPenceresi.close();
}

function FPDegerAta_HesapPlanListe(node, fp, sutun, sutunAciklama) {
    var kodAd = node.text.split('-');
    var satir = fp.GetActiveRow();

    fp.SetValue(satir, sutun, kodAd[0], false);

    var hucre = fp.GetCellByRowCol(satir, sutunAciklama);
    hucre.removeAttribute("FpCellType");
    fp.SetValue(satir, sutunAciklama, kodAd[1], false);
    hucre.setAttribute("FpCellType", "readonly");
}

function HucreDegisti(control) {
    var row = event.cell.parentElement.rowIndex;
    var col = event.cell.cellIndex;

    if (col == 3 || (col >= 5 && col <= 6))
        ParaNoktaKoy(control, null);
    else if (col == 0) {
        var deger = control.GetValue(row, col);
        var hesapAdKolon = Number(document.getElementById("hdnHesapAdKolon").value);
        if (deger != "") {
            TasinirBilgiGetir(control, row, col, hesapAdKolon, hesapAdKolon + 1);
        }
        else {
            var hucre = control.GetCellByRowCol(row, hesapAdKolon);
            hucre.removeAttribute("FpCellType");
            control.SetValue(row, hesapAdKolon, "");
            hucre.setAttribute("FpCellType", "readonly");
            hucre = control.GetCellByRowCol(row, hesapAdKolon + 1);
            hucre.removeAttribute("FpCellType");
            control.SetValue(row, hesapAdKolon + 1, "");
            hucre.setAttribute("FpCellType", "readonly");
        }
    }
}

function BosSatirAc() {
    document.getElementById('fpL').CallBack("bossatirekle");
}

function ArayaSatirEkle() {
    document.getElementById('fpL').CallBack("arayasatirekle");
}

function SatirSil() {
    document.getElementById('fpL').CallBack("satirsil");
}

function ListeTemizle() {
    var liste = document.getElementById('fpL');
    var rg = liste.GetSelectedRange();
    for (var i = rg.row; i < rg.row + rg.rowCount; i++) {
        ListeyeDegerYaz(liste, i, 0, "", true);
        ListeyeDegerYaz(liste, i, 2, "", true);
        ListeyeDegerYaz(liste, i, 3, "", true);
        ListeyeDegerYaz(liste, i, 4, "", true);
        ListeyeDegerYaz(liste, i, 5, "", true);
        ListeyeDegerYaz(liste, i, 6, "", true);
        ListeyeDegerYaz(liste, i, 7, "", true);
        ListeyeDegerYaz(liste, i, 8, "", true);
        ListeyeDegerYaz(liste, i, 9, "", true);
        ListeyeDegerYaz(liste, i, 10, "", true);
        ListeyeDegerYaz(liste, i, 11, "", true);
    }
}

function ListeyeDegerYaz(kontrol, row, col, deger, noEvent) {
    var hucre = kontrol.GetCellByRowCol(row, col);
    if (hucre) {
        var durum = hucre.getAttribute("FpCellType");
        hucre.removeAttribute("FpCellType");
        kontrol.SetValue(row, col, deger, noEvent);
        if (durum != "")
            hucre.setAttribute("FpCellType", durum);
    }
}