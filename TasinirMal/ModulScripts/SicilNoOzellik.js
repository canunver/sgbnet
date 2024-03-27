function MarkaGoster() {
    var adres = "ListeMarka.aspx";
    adres += "?menuYok=1&listeyeYaz=MarkaSecKapatDeger";

    showPopWin(adres, 500, 420, true, null);
}

function ModelGoster() {
    var adres = "ListeModel.aspx";
    adres += "?menuYok=1&listeyeYaz=ModelSecKapatDeger";

    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow();
    var marka = fp.GetValue(satir, 3);
    if (marka == "") {
        alert(res_FRMJSC011);
        return false;
    }
    else
        marka = marka.split('-')[0];

    adres += "&marka=" + marka;

    showPopWin(adres, 500, 420, true, null);
}

function MarkaSecKapatDeger(deger, aciklama) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow();
    fp.GetCellByRowCol(satir, 3).removeAttribute("FpCellType");
    fp.SetValue(satir, 3, deger + "-" + aciklama, true);
    fp.GetCellByRowCol(satir, 3).setAttribute("FpCellType", "readonly");
    hidePopWin();
}

function ModelSecKapatDeger(deger, aciklama) {
    var fp = document.getElementById('fpL');
    var satir = fp.GetActiveRow();
    fp.GetCellByRowCol(satir, 5).removeAttribute("FpCellType");
    fp.SetValue(satir, 5, deger + "-" + aciklama, true);
    fp.GetCellByRowCol(satir, 5).setAttribute("FpCellType", "readonly");
    hidePopWin();
}