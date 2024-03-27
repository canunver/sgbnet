function Aktar(tur) {
    var kontrol = document.getElementById('gvDosyadanGelenler');
    var listeKontrol = window.parent.document.getElementById('fpL');
    var aktifSatir = 0;
    var kontrolSatirSayisi = 0;
    if (listeKontrol) {
        aktifSatir = listeKontrol.GetActiveRow();
        kontrolSatirSayisi = listeKontrol.GetRowCount();
    }

    if (kontrol) {
        var sayac = 0;

        var kolon = 0;

        if (document.getElementById('rdKolonAmbar').checked)
            kolon = 4;
        else
            kolon = 5;

        for (var i = 1; i < kontrol.rows.length; i++) {
            //	        var chkId = "gvDosyadanGelenler$ctl";
            //	        var chkTemp ="";
            //	        if(i<9)
            //	            chkTemp = "0" + (i+1).toString();
            //	        else
            //	            chkTemp = (i+1).toString();
            //	        
            //	        chkId = chkId + chkTemp + "$chkSecim";
            //	        if (document.getElementById(chkId).checked || tur=='hepsi')
            if (kontrol.rows[i].cells[0].children[0].checked || tur == 'hepsi') {
                if (listeKontrol) {
                    try//bilgi text alana atılacaksa farpoint olmadığında hata vermesin
                    {
                        if (kontrolSatirSayisi < aktifSatir + sayac + 1) {
                            alert(res_FRMJSC010);
                            return;
                        }
                    }
                    catch (e) { }
                }

                var hesapPlanKod = GridHucreDegerAl(kontrol.rows[i].cells[2]);
                //var sicilNo = GridHucreDegerAl(kontrol.rows[i].cells[3]);
                var miktar = GridHucreDegerAl(kontrol.rows[i].cells[3]);

                window.parent.TerminalListesindenGrideYaz(sayac++, hesapPlanKod, miktar, kolon);
            }
        }
    }

    window.parent.hidePopWin();
}

function ListeSec() {
    var kontrol = document.getElementById('gvDosyadanGelenler');
    if (kontrol) {
        secim = kontrol.rows[0].cells[0].children[0].checked;
        for (var i = 1; i < kontrol.rows.length; i++)
            kontrol.rows[i].cells[0].children[0].checked = secim;
    }
}