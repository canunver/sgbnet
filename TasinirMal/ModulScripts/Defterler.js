function NodeSecildiYerel(node) {
    var kodAd = node.text.split('-');
    document.getElementById('txtHesapPlanKod').value = kodAd[0];
    document.getElementById('lblHesapPlanAd').innerHTML = kodAd[1];
}
