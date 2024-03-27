function Aktar(tur) {
    var kontrol = document.getElementById('gvTuketimler');
    var listeKontrol = window.parent.document.getElementById('fpL');
    var aktifSatir = listeKontrol.GetActiveRow(); //FireFox
    var kontrolSatirSayisi = listeKontrol.GetRowCount();

    if (kontrol) {
        var sicilNo = "";
        try {
            window.parent.StokSicilListesindenGrideYazKDVYazma(false);
        }
        catch (e) { }

        if (tur == 'hepsi') {
            var sayac = 0;
            for (var i = 1; i < kontrol.rows.length; i++) {
                try//bilgi text alana atılacaksa fgrid olmadığında hata vermesin
                {
                    if (kontrolSatirSayisi < aktifSatir + sayac + 1) {
                        alert(res_FRMJSC010);
                        return;
                    }
                }
                catch (e) { }

                var hesapPlanKod = GridHucreDegerAl(kontrol.rows[i].cells[0]);
                var miktar = GridHucreDegerAl(kontrol.rows[i].cells[2]);
                var kdvOran = GridHucreDegerAl(kontrol.rows[i].cells[3]);
                var birimFiyat = GridHucreDegerAl(kontrol.rows[i].cells[4]);
                window.parent.StokSicilListesindenGrideYaz(sayac++, hesapPlanKod, sicilNo, miktar, kdvOran, birimFiyat);
            }
        }
        else if (tur == 'miktar') {
            var miktar = document.getElementById('txtMiktar').value;
            miktar = miktar.replace(/\./g, '');
            miktar = miktar.replace(/,/g, '.');

            var gMiktar = parseFloat(miktar);

            if (gMiktar == "") {
                alert(res_FRMJSC017);
                return;
            }
            else if (isNaN(gMiktar)) {
                alert(res_FRMJSC018);
                return;
            }

            var gHesapPlanKod = document.getElementById('txtHesapPlaniKodu').value;

            var sHesapPlanKod = "";
            var sMiktar = 0;
            var sayac = 0;

            for (var i = 1; i < kontrol.rows.length; i++) {
                try//bilgi text alana atılacaksa fgrid olmadığında hata vermesin
                {
                    if (kontrolSatirSayisi < aktifSatir + sayac + 1) {
                        alert(res_FRMJSC010);
                        return;
                    }
                }
                catch (e) { }

                sHesapPlanKod = GridHucreDegerAl(kontrol.rows[i].cells[0]);
                if (sHesapPlanKod != gHesapPlanKod)
                    continue;
                else {
                    var miktar2 = GridHucreDegerAl(kontrol.rows[i].cells[2]);
                    miktar2 = miktar2.replace(/\./g, '');
                    miktar2 = miktar2.replace(/,/g, '.');

                    sMiktar = parseFloat(miktar2);

                    if (isNaN(sMiktar)) {
                        alert(res_FRMJSC018);
                        return;
                    }

                    var kdvOran = GridHucreDegerAl(kontrol.rows[i].cells[3]);
                    var birimFiyat = GridHucreDegerAl(kontrol.rows[i].cells[4]);

                    if (sMiktar >= gMiktar) {
                        yazMiktar = (gMiktar + "").replace('.', ','); //Gride yazarken nokta basamak ayıracı olduğu için değiştiriliyor
                        window.parent.StokSicilListesindenGrideYaz(sayac++, gHesapPlanKod, sicilNo, yazMiktar, kdvOran, birimFiyat);
                        gMiktar = 0;
                        break;
                    }
                    else {
                        yazMiktar = (sMiktar + "").replace('.', ','); //Gride yazarken nokta basamak ayıracı olduğu için değiştiriliyor
                        window.parent.StokSicilListesindenGrideYaz(sayac++, gHesapPlanKod, sicilNo, yazMiktar, kdvOran, birimFiyat);
                        gMiktar = gMiktar - sMiktar;
                    }
                }
            }
            if (gMiktar > 0) {
                alert(StrFormat(res_FRMJSC019, gHesapPlanKod));
            }
        }

        try {
            window.parent.StokSicilListesindenGrideYazKDVYazma(true);
        }
        catch (e) { }
        window.parent.hidePopWin();
    }
}