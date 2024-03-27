var hesapPlaniPenceresi = null;

function NodeSecildiYerel(node) {
    FPDegerAta_HesapPlanListe(node, document.getElementById('fpL'), 0, 4);
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

    //    var hucre = fp.GetCellByRowCol(satir, sutunAciklama);
    //    hucre.removeAttribute("FpCellType");
    //    fp.SetValue(satir, sutunAciklama, kodAd[1], false);
    //    hucre.setAttribute("FpCellType", "readonly");
}

function HucreDegisti(control, sutunAd, sutunOlcu) {
    var row = event.cell.parentElement.rowIndex;
    var col = event.cell.cellIndex;

    if (col >= 2 && col <= 3)
        ParaNoktaKoy(control, null);
    else if (col == 0) {
        var deger = control.GetValue(row, col);
        if (deger != "") {
            TasinirBilgiGetir(control, row, col, sutunAd, sutunOlcu);
        }
        else {
            ListeyeDegerYaz(control, row, sutunOlcu, "", true);
            ListeyeDegerYaz(control, row, sutunAd, "", true);
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

function StokListesiAc() {
    var hesapPlanKod = document.getElementById('fpL').GetValue(document.getElementById('fpL').GetActiveRow(), 0); //FireFox
    var adres = "ListeStok.aspx?menuYok=1&hpk=" + hesapPlanKod;
    var yil = document.getElementById('ddlYil').value;
    adres += "&yil=" + yil;
    var muhasebeKod = document.getElementById('txtMuhasebe').value;
    adres += "&mb=" + muhasebeKod;
    var harcamaBirimKod = document.getElementById('txtHarcamaBirimi').value;
    adres += "&hbk=" + harcamaBirimKod;
    var ambarKod = document.getElementById('txtAmbar').value;
    adres += "&ak=" + ambarKod;

    showPopWin(adres, 800, 500, true, null);
}

function StokSicilListesindenGrideYaz(satirNo, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow + satirNo;

    ListeyeDegerYaz(fp, satir, 0, hesapPlanKod, false);
    ListeyeDegerYaz(fp, satir, 2, miktar, true);
    ListeyeDegerYaz(fp, satir, 5, kdvOran, true);
    ListeyeDegerYaz(fp, satir, 6, birimFiyat, true);
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

function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, istekYapanKod, belgeNo) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('hdnBelgeNo').value = belgeNo;
    document.getElementById('txtPersonel').value = istekYapanKod;
    document.getElementById('btnListele').click();
}

