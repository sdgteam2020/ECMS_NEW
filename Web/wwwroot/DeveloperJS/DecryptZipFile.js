$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

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
                    Save(formId);
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
            $("#lblZipFileNotification").addClass("text-danger");
            $("#lblZipFileNotification").removeClass("text-success");
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
    var formId = '#DecryptZipFile'; 
    var fileInput = $(formId).find('#ZipFile')[0];
    var selectedFile = fileInput.files[0]; 
    var privateKey = "1";

    var formData = new FormData();
    formData.append('ZipFile', selectedFile);  // Append the file
    formData.append('PrivateKey', privateKey); // Append the private key

    $.ajax({
        url: '/BasicDetail/DecryptZipFileData',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: "Data Not Decrypt, Internal Server Error"
                    });
                } else {
                    //window.location = "/WriteReadData/ExportAFSACCell/Temp/" + response;
                    //setTimeout(function () {
                    //    location.reload();
                    //}, 1000);

                    let responseHtml = "Data Decrypt Successfully!";
                    Swal.fire({
                        title: responseHtml,
                        text: "Please download Zip file.",
                        icon: "success",
                        showConfirmButton: false, // We'll create custom buttons
                        showCancelButton: false,
                        allowOutsideClick: false,
                        didOpen: () => {
                            const swal = Swal.getPopup();

                            const btnGroup = document.createElement('div');
                            btnGroup.style.display = 'flex';
                            btnGroup.style.justifyContent = 'center';
                            btnGroup.style.gap = '10px';

                            const downloadBtn = document.createElement('button');
                            downloadBtn.textContent = 'Download';
                            downloadBtn.className = 'swal2-confirm swal2-styled';
                            downloadBtn.style.backgroundColor = '#28a745'; // green
                            downloadBtn.onclick = function () {
                                const fileUrl = `/WriteReadData/ExportAFSACCell/Temp/${response}`;
                                const link = document.createElement('a');
                                link.href = fileUrl;
                                link.download = response; // This will prompt the file to download instead of opening it in a new tab
                                document.body.appendChild(link); // Append the link to the document
                                link.click(); // Trigger the download
                                document.body.removeChild(link); // Clean up by removing the link
                            };

                            const closedBtn = document.createElement('button');
                            closedBtn.textContent = 'Close';
                            closedBtn.className = 'swal2-cancel swal2-styled';
                            closedBtn.style.backgroundColor = '#dc3545'; // red
                            closedBtn.onclick = function () {
                                Swal.close();
                                location.reload();
                            };

                            btnGroup.appendChild(downloadBtn);
                            btnGroup.appendChild(closedBtn);

                            swal.appendChild(btnGroup);
                        }
                    });



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