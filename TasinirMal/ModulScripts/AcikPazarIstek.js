function ListeSec() {
    var kontrol = document.getElementById('dgListe');
    if (kontrol) {
        secim = kontrol.rows[0].cells[0].children[0].checked;
        for (var i = 1; i < kontrol.rows.length; i++)
            kontrol.rows[i].cells[0].children[0].checked = secim;
    }
}