//$(function(){
//    $("#tblDatatable").DataTable({
//        "language": {
//            "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Turkish.json"
//        }
//    });
//});
//$(function () {
//    $("#tblDatatable").on("click", ".bntKullaniciSil", function () {
//        var btn = $(this);
//        bootbox.confirm("Kullanıcıyı silmek istediğinize emin misiniz?", function (result) {
//            if (result) {
//                var id = btn.data("id");

//                $.ajax({
//                    type: "GET",
//                    url: "/Security/KullaniciSil/" + id,
//                    success: function () {
//                        //btn.parent().parent().remove();
//                        window.location.href = "/Kullanici/KullaniciListesi";
//                    }
//                });
//            }
//        })
 
//    });
//});
