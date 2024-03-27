function Aktar(tur) {
    var kontrol = document.getElementById('gvSicilNo');
    var listeKontrol = window.parent.document.getElementById('fpL');
    var aktifSatir = 0;
    var kontrolSatirSayisi = 0;
    if (listeKontrol) {
        aktifSatir = listeKontrol.GetActiveRow();
        kontrolSatirSayisi = listeKontrol.GetRowCount();
    }

    if (kontrol) {
        var sayac = 0;
        try {
            window.parent.StokSicilListesindenGrideYazKDVYazma(false);
        }
        catch (e) { }

        for (var i = 1; i < kontrol.rows.length; i++) {
            if (kontrol.rows[i].cells[0].children[0].checked || tur == 'hepsi') {
                if (listeKontrol) {
                    try//bilgi text alana atılacaksa fgrid olmadığında hata vermesin
                    {
                        if (kontrolSatirSayisi < aktifSatir + sayac + 1) {
                            alert(res_FRMJSC010);
                            return;
                        }
                    }
                    catch (e) { }
                }
                var sicilNo = GridHucreDegerAl(kontrol.rows[i].cells[1]);
                var hesapPlanKod = GridHucreDegerAl(kontrol.rows[i].cells[2]);
                var kdvOran = GridHucreDegerAl(kontrol.rows[i].cells[4]);
                var birimFiyat = GridHucreDegerAl(kontrol.rows[i].cells[5]);
                window.parent.StokSicilListesindenGrideYaz(sayac++, hesapPlanKod, sicilNo, "1", kdvOran, birimFiyat);
            }
        }
    }

    try {
        window.parent.StokSicilListesindenGrideYazKDVYazma(true);
    }
    catch (e) { }

    window.parent.hidePopWin();
}

function ListeSec() {
    var kontrol = document.getElementById('gvSicilNo');
    if (kontrol) {
        secim = kontrol.rows[0].cells[0].children[0].checked;
        for (var i = 1; i < kontrol.rows.length; i++)
            kontrol.rows[i].cells[0].children[0].checked = secim;
    }
}

function SicilNolariAl() {
    var listeKontrol = window.parent.document.getElementById('fpL');

    if (listeKontrol) {
        aktifSatir = listeKontrol.GetActiveRow();
        var kontrolSatirSayisi = listeKontrol.GetRowCount();
    }

    var sicilNolar = "";
    var hucreDeger = "";

    for (i = 0; i < kontrolSatirSayisi - 1; i++) {
        hucreDeger = listeKontrol.GetValue(i, 2); //tasinir islem

        if (hucreDeger == "")
            hucreDeger = listeKontrol.GetValue(i, 1); //zimmet

        if (hucreDeger != "") {
            sicilNolar += hucreDeger;
            sicilNolar += "\;";
        }
    }

    document.getElementById('hdnSicilNolar').value = sicilNolar;
}