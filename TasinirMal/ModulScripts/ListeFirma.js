function VeriGoster(bilgi) {
    var tDiziAciklama = bilgi.split('|');

    if (tDiziAciklama.length >= 5) {
        try {
            parent.document.getElementById('txtNeredenGeldi').value = tDiziAciklama[0];
        }
        catch (e) {
            try {
                parent.document.getElementById('txtIlgiliAd').value = tDiziAciklama[0];
                parent.document.getElementById('txtIlgiliNo').value = tDiziAciklama[1];
                parent.document.getElementById('txtIlgiliVD').value = tDiziAciklama[2];
                parent.document.getElementById('txtIlgiliBankaAd').value = tDiziAciklama[3];
                parent.document.getElementById('txtIlgiliBankaNo').value = tDiziAciklama[4];
            }
            catch (e) { }
        }

        parent.hidePopWin();
    }
}
