// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$("#fileUpload").on('change', function () {
    var files = $('#fileUpload').prop("files");
    var url = "/Products/OnPostMyUploader";
    formData = new FormData();
    formData.append("myUploader", files[0]);

    jQuery.ajax({
        type: 'POST',
        url: url,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.status == "success") {
                $("#fileUploadValue").val(data.uniqueFileName)
                alert("File : " + repo.filename + " is uploaded successfully");
            }
        },
        error: function() {
            alert("Error occurs");
        }
    });
});

$('.addToCart').each(function () {
    var $this = $(this);
    $this.on("click", function () {
        var url = "/ShoppingCart/AddToCard";
        var _this = $(this)
        var data = { productId: +_this.data("productid"),
        userId: _this.data("userid")} 
        jQuery.ajax({
            type: 'POST',
            url: url,
            data: data,
            cache: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (data) {
                GetCartCount()
                alert("Product successfully added to Cart");
               
            },
            error: function() {
                alert("Error occurs");
            }
        });
    });
});


$(document).ready(function() {
    GetCartCount();
})

function GetCartCount(){
    var url = "/ShoppingCart/GetShoppingCartCount";
    jQuery.ajax({
        type: 'POST',
        url: url,
        cache: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {           
            $("#shoppingCounter").text("Shopping Cart ( " + data.count + " )");

        },
        error: function() {
            alert("Error occurs");
        }
    });
}