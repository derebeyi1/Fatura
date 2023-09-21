$(document).ready(function () {    
    //$("table tr").dblclick(function () {
    //    var tableData = $(this).children("td").map(function () {
    //        return $(this).text();
    //    }).get();
    //    alert("::>" + (tableData.length - 1) + ">>> " + tableData[tableData.length - 1]);
    //    window.location.href = "/Stok/StokGuncelle?st=null&id=" + tableData[tableData.length - 1];
    //});
    //$(function () {
    //    $("#dataTable").on("click", ".bntStokSil", function () {
    //        var btn = $(this);
    //        bootbox.confirm("Stoğu silmek istiyor musunuz?", function (result) {
    //            if (result) {
    //                var id = btn.data("id");

    //                $.ajax({
    //                    type: "GET",
    //                    url: "/Stok/Sil/",
    //                    data: { id: id },
    //                    success: function () {
    //                        btn.parent().parent().parent().parent().remove();
    //                        $('#kayitsayisi').html(parseInt($('#kayitsayisi').html()) - 1)
    //                    }
    //                });
    //            }
    //        })

    //    });
    //});
    //$('#dataTable').DataTable({
    //    language: {
    //        info: "_TOTAL_ kayıttan _START_ - _END_ kayıt gösteriliyor.",
    //        infoEmpty: "Gösterilecek hiç kayıt yok.",
    //        loadingRecords: "Kayıtlar yükleniyor.",
    //        zeroRecords: "Tablo boş",
    //        search: "Arama:",
    //        sLengthMenu: "Sayfada _MENU_ kayıt göster",
    //        infoFiltered: "(toplam _MAX_ kayıttan filtrelenenler)",
    //        buttons: {
    //            copyTitle: "Panoya kopyalandı.",
    //            copySuccess: "Panoya %d satır kopyalandı",
    //            copy: "Kopyala",
    //            print: "Yazdır",
    //        },

    //        paginate: {
    //            first: "İlk",
    //            previous: "Önceki",
    //            next: "Sonraki",
    //            last: "Son"
    //        },
    //    }
    //});
});
function DovizKuruGetir(adi, id) {
    debugger;
    if (id != 0) {
        $.ajax({
            type: "POST",
            url: "/Fatura/DovizKuruGetir",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ "id": id }),
            datatype: "json",
            async: false,
            success: function (data) {
                debugger;
                if (adi == "doviz_id") {
                    $("#doviz_kuru").val(data[0].kur_carpani);
                }
                else
                    $("#doviz_kuru1").val(data[0].kur_carpani);
            },
            error: function () {
                alert("Beklenmedik hata oluştu. FaturaFormu");
            },
            //complate: function () {
            //    alert("complate");
            //},
        });
    }
    else {
        if (adi == "doviz_id")
            $("#doviz_kuru").val(1);
        else
            $("#doviz_kuru1").val(1);
    }
}
function toastr_mesaj(mesaj, mesaj_tipi, zaman) {
    debugger;
    var tapToDismiss = true;
    if (zaman == 0)
        tapToDismiss = false;
    toastr.options = {
        "tapToDismiss": tapToDismiss,
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-full-width",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": zaman / 10,
        "hideDuration": zaman / 2,
        "timeOut": zaman,
        "extendedTimeOut": zaman,
        "showEasing": "swing",
        "hideEasing": "linear",
        "closeEasing": 'linear',
        "showMethod": "fadeIn",//"slideDown",fadeIn,show
        "hideMethod": "fadeOut",//"slideIn"fadeOut,hide
    }
    toastr[mesaj_tipi]("<span style='font-size:120%'>" + mesaj + "</span>")
}
function validateEmail(email) {
    debugger;
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}




