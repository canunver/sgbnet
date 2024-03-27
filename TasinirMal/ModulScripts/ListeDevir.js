function VeriGoster(bilgi) {
    var tDiziAciklama = bilgi.split('|');

    if (tDiziAciklama.length >= 4) {
        parent.document.getElementById('txtGonMuhasebe').value = tDiziAciklama[0];
        parent.document.getElementById('txtGonHarcamaBirimi').value = tDiziAciklama[1];
        parent.document.getElementById('txtGonAmbar').value = tDiziAciklama[2];
        parent.document.getElementById('txtGonBelgeNo').value = tDiziAciklama[3];

        parent.hidePopWin();
    }
}

function ListeSec() {
    var kontrol = document.getElementById('dgListe');
    if (kontrol) {
        secim = kontrol.rows[0].cells[0].children[0].checked;
        for (var i = 1; i < kontrol.rows.length; i++)
            kontrol.rows[i].cells[0].children[0].checked = secim;
    }
}