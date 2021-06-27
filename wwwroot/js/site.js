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
