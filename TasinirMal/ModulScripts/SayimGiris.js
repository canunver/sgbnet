var hesapPlaniPenceresi = null;
var tur = null;

//function NodeSecildiYerel(node) {
//    FPDegerAta_HesapPlanListe(node, document.getElementById('fpL'), 0, 2);
//}

function NodeSecildiYerel(node) {
    var kodAd = node.text.split('-');
    document.getElementById('txtHesapPlanKod').value = kodAd[0];
   var olcu = kodAd[2];
   var ad = kodAd[1];
   var id = parent.panelIslem.getBody().grdListe.getSelectionModel().getSelections()[0];
   parent.panelIslem.getBody().grdListe.store.getById(id.id).set('HESAPPLANAD', ad);
   parent.panelIslem.getBody().grdListe.store.getById(id.id).set('OLCUBIRIMAD', olcu);

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

    if (col >= 4 && col <= 6)
        ParaNoktaKoy(control, null);
    if (col == 0) {
        var deger = control.GetValue(row, col);
        if (deger != "")
            TasinirBilgiGetir(control, row, col, 2, 3);
        //kodAdGetir('30','fpL|'+row+"x2",true,new Array(deger),'DEGISKEN'); 
        else
            control.SetValue(row, 2, "");
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

function ListeAcTerminal(adres) {
    adres += "?menuYok=1&tur=sayim";
    showPopWin(adres, 600, 400, true, null);
}

function TerminalListesindenGrideYaz(satirNo, hesapPlanKod, miktar, kolon) {
    var fp = document.getElementById('fpL');

    var satir = fp.GetActiveRow() + satirNo;

    if (fp.GetActiveRow() < 0)
        satir++;

    ListeyeDegerYaz(fp, satir, 0, hesapPlanKod, false);
    ListeyeDegerYaz(fp, satir, kolon, miktar, true);
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

function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('hdnSayimNo').value = belgeNo;
    document.getElementById('btnListele').click();
}

