function FPDegerAta_HesapPlanListe(node, fp, sutun, sutunAciklama, tertipTur, ekoUzun, noEventKod, noEventAciklama) {
    var kodAd = node.text.split('-');
    var satir = fp.GetActiveRow();

    ListeyeDegerYaz(fp, satir, 0, kodAd[0], noEventKod);
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

function HucreDegisti(control) {
    var row = event.cell.parentElement.rowIndex;
    var col = event.cell.cellIndex;

    if (col == 0) {
        var deger = control.GetValue(row, col);
        TasinirBilgiGetir(control, row, col);
    }
    if (col == 5) {
        deger = ParaNoktaKoy(control, null);
    }
}

function BelgeAc(yil, muhasebeKod, harcamaBirimiKod, belgeNo) {
    document.getElementById('ddlYil').value = yil;
    document.getElementById('txtMuhasebe').value = muhasebeKod;
    document.getElementById('txtHarcamaBirimi').value = harcamaBirimiKod
    document.getElementById('txtBelgeNo').value = belgeNo;
    document.getElementById('btnListele').click();
}

function ToplamHesapla() {
    var liste = document.getElementById('fpL');

    var satirSayisi = liste.GetRowCount();

    var fisToplami = 0;

    for (var row = 0; row < satirSayisi; row++) {
        var birimFiyat = liste.GetValue(row, 5);
        var miktar = liste.GetValue(row, 2);
        var kdv = liste.GetValue(row, 4);

        if (birimFiyat != "" || miktar != "") {
            birimFiyat = Number(ScriptParaCevir(birimFiyat));
            miktar = Number(ScriptParaCevir(miktar));

            kdv = 1 + (Number(kdv) / 100);
            fisToplami += (birimFiyat * miktar * kdv);
        }
    }

    alert(ParaNoktala(fisToplami));
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

function TasinirBilgiGetir(control, satir, sutun) {
    param = new Object;
    param.control = control;
    param.satir = satir;
    param.sutun = sutun;

    var deger = control.GetValue(satir, sutun);
    var adres = "TasinirBilgiXML.aspx?HesapPlanKod=" + deger;

    var c1 = new HttpCagir(adres, TasinirBilgiYaz, param);
    delete (c1);
}

function TasinirBilgiYaz(cevap, param) {
    var olcuBirimAd = "";
    var hesapPlanAd = "";
    var kdv = "";

    try {
        var elem = document.createElement(cevap);
        olcuBirimAd = elem.getAttribute("OLCUBIRIMAD");
        hesapPlanAd = elem.getAttribute("HESAPPLANAD");
        kdv = elem.getAttribute("KDV");
        delete (elem);
    }
    catch (err) {
    }

    if (olcuBirimAd != null)
        ListeyeDegerYaz(param.control, param.satir, 3, olcuBirimAd, true);
    if (kdv != null)
        ListeyeDegerYaz(param.control, param.satir, 4, kdv, true);
    if (hesapPlanAd != null)
        ListeyeDegerYaz(param.control, param.satir, 6, hesapPlanAd, true);

    delete (param);
}