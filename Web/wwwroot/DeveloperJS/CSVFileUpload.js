var lstUpdate = new Array();
$(function () {
    $("#CSVFile").on("change", function () {
        beforeUploadCSVFileCheck(this);
    });

    $("#btnsave").on("click", function () {
        ResetErrorMessage();
        let formId = '#csvUploadForm';
        $.validator.unobtrusive.parse($(formId));

        if ($(formId).valid()) {
            Swal.fire({
                title: 'Are you sure?',
                text: "",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Submit it!'
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
function Save() {
    var listItem = "";
    var formId = '#csvUploadForm';
    var fileInput = $(formId).find('#CSVFile')[0];
    var selectedFile = fileInput.files[0];

    var formData = new FormData();
    formData.append('CSVFile', selectedFile);  // Append the file

    $.ajax({
        url: '/BasicDetail/UploadCsv',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#tblData").DataTable().destroy();

            for (var i = 0; i < response.length; i++) {
                listItem += "<tr>";
                listItem += "<td class='align-middle'><div class='custom-control custom-checkbox small'><input type='checkbox' class='custom-control-input' id=" + response[i].RequestId + " data-chipNo=" + response[i].ChipNo + " data-cardSerialNo=" + response[i].CardSerialNo + " data-isValid=" + response[i].IsValid + "><label class='custom-control-label' for=" + response[i].RequestId +"></label></div></td>";
                listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                listItem += "<td class='align-middle'>" + response[i].RequestId + "</td>";
                listItem += "<td class='align-middle'>" + response[i].ServiceNo + "</td>";
                if (response[i].LName == "")
                    listItem += "<td class='align-middle'>" + response[i].RankName + " " + response[i].FName + "</td>";
                else
                    listItem += "<td class='align-middle'>" + response[i].RankName + " " + response[i].FName + " " + response[i].LName + "</td>";
                listItem += "<td class='align-middle'><span id='chipNo'>" + response[i].ChipNo + "</span></td>";
                listItem += "<td class='align-middle'><span id='cardSerialNo'>" + response[i].CardSerialNo + "</span></td>";
                listItem += "<td class='align-middle'><span id='isValid'>" + response[i].IsValid + "</span></td>";
                listItem += "</tr>";
            }
            $("#DetailBody").html(listItem);

            memberTable = $('#tblData').DataTable({
                retrieve: true,
                lengthChange: false,
                stateSave: true,
                "order": [[1, "asc"]],
                buttons: [{
                    extend: 'copy',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }, {
                    extend: 'excel',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }, {
                    extend: 'pdfHtml5',
                    orientation: 'portrait',
                    pageSize: 'A4',
                    title: 'E-IASC_Rank',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    },
                    customize: function (doc) {
                        WaterMarkOnPdf(doc)
                    }
                }]
            });

            var rows;
            $("#tblData #chkAll").on("click", function () {
                if ($(this).is(':checked')) {
                    rows = memberTable.rows({ 'search': 'applied' }).nodes();
                    $('input[type="checkbox"]', rows).prop('checked', this.checked);
                }
                else {
                    rows = memberTable.rows({ 'search': 'applied' }).nodes();
                    $('input[type="checkbox"]', rows).prop('checked', this.checked);
                }
            });
        },
        error: function (xhr) {
            listItem += "<tr><td class='text-center' colspan=8>No Record Found</td></tr>";
            $("#tblData").DataTable().destroy();
            $("#DetailBody").html(listItem);

            // Display error message from server
            var errorMessage = xhr.responseJSON?.message || "An unexpected error occurred.";
            alert('Error: ' + errorMessage);
        }
    });
}

function beforeUploadCSVFileCheck(id) {
    $("#lblCSVFile").html("");
    const file = id.files[0];
    if (file) {
        var size = parseFloat(file.size);
        var maxSizeKB = 5120; //Size in KB.
        var maxSize = maxSizeKB * 1024; //File size is returned in Bytes.
        var allowedTypes = ['text/csv'];

        if (!allowedTypes.includes(file.type)) {
            $("#lblCSVFile").html("Invalid file type. Only CSV files are allowed. </br>");
            $("#lblCSVFileNotification").addClass("text-danger");
            $("#lblCSVFileNotification").removeClass("text-success");
            $("#CSVFile").val(null);
            return false;
        }
        else {
            if (size > maxSize) {
                $("#lblCSVFile").html("Maximum file size " + maxSizeKB + "KB allowed. </br>");
                $("#CSVFile").val(null);
                $("#lblCSVFileNotification").addClass("text-danger");
                $("#lblCSVFileNotification").removeClass("text-success");
                return false;
            } else {
                $("#lblCSVFileNotification").addClass("text-success");
                $("#lblCSVFileNotification").removeClass("text-danger");
            }
        }
    }
}
function ResetErrorMessage() {
    $("#lblCSVFile").html("");
    $("#CSVFile-error").html("");
}
function Reset() {
    $("#CSVFile").val("");
}
$("#btnUpdate").on("click", function () {
    // Empty the array
    lstUpdate.length = 0;
    if (memberTable.$('input[type="checkbox"]:checked').length > 0) {
        memberTable.$('input[type="checkbox"]:checked').each(function () {
            let obj = {
                RequestId: $(this).attr("Id"),
                ChipNo: $(this).attr("data-chipNo"),
                CardSerialNo: $(this).attr("data-cardSerialNo"),
                IsValid: $(this).attr("data-isValid")
            };
            lstUpdate.push(obj);
        });
        $.ajax({
            url: '/BasicDetail/UploadChipAndSerial',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(lstUpdate),
            success: function (response) {
                if (response.Result == true)
                {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: response.Message,

                    })
                }
                else
                {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: response.Message,

                    })
                }
                
            },
            error: function (xhr) {
                // Display error message from server
                var errorMessage = xhr.responseJSON?.message || "An unexpected error occurred.";
                alert('Error: ' + errorMessage);
            }
        });

    } else {
        Swal.fire({
            text: "Please select atleast 1 request to Update."
        });
    }
});