$(function () {
    $("#ZipFile").on("change", function () {
        beforeUploadZipFileCheck(this);
    });
    $("#btnsave").on("click", function () {
        ResetErrorMessage();
        let formId = '#DecryptZipFile';
        $.validator.unobtrusive.parse($(formId));

        if ($(formId).valid()) {
            Swal.fire({
                title: 'Are you sure?',
                text: "",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Decrypt it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    Save();
                }
            })
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Please fill required field.',

            })
            toastr.error('Please fill required field.');
            return false;
        }
    });
});
function beforeUploadZipFileCheck(id) {
    $("#lblZipFile").html("");
    const file = id.files[0];
    if (file) {
        var size = parseFloat(file.size);
        var maxSizeKB = 5120; //Size in KB.
        var maxSize = maxSizeKB * 1024; //File size is returned in Bytes.
        var allowedTypes = ['application/x-zip-compressed'];

        if (!allowedTypes.includes(file.type)) {
            $("#lblZipFile").html("Invalid file type. Only Zip files are allowed. </br>");
            $("#lblPhotoNotification").addClass("text-danger");
            $("#lblPhotoNotification").removeClass("text-success");
            $("#ZipFile").val(null);
            return false;
        }
        else {
            if (size > maxSize) {
                $("#lblZipFile").html("Maximum file size " + maxSizeKB + "KB allowed. </br>");
                $("#ZipFile").val(null);
                $("#lblZipFileNotification").addClass("text-danger");
                $("#lblZipFileNotification").removeClass("text-success");
                return false;
            } else {
                $("#lblZipFileNotification").addClass("text-success");
                $("#lblZipFileNotification").removeClass("text-danger");
            }
        }
    }
}
function Save() {
    $.ajax({
        url: '/BasicDetail/DecryptZipFile',
        type: 'POST',
        data: {
            "ZipFile": $("#ZipFile").val(),
        }, 
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: "Data Not Export Internal Server Error"
                    });
                } else {
                    window.location = "/WriteReadData/ExportAFSACCell/Temp/" + response + '.zip';
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                }
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function ResetErrorMessage() {
    $("#lblZipFile").html("");
    $("#ZipFile-error").html("");
}
function Reset() {
    $("#ZipFile").val("");
}