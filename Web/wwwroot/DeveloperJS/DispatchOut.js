$(async function () {
    var dtToday = new Date();

    var month = dtToday.getMonth() + 1;
    var day = dtToday.getDate();
    var year = dtToday.getFullYear() + 1;

    if (month < 10)
        month = '0' + month.toString();
    if (day < 10)
        day = '0' + day.toString();

    var minDate = dtToday.getFullYear() + '-' + month + '-' + day;
    var maxDate = year + '-' + month + '-' + day;

    $('#txtDispatchDate').attr('min', minDate);
    $('#txtDispatchDate').attr('max', maxDate);

    $('#txtDispatchDate').on('change', function () {
        var dtToday = new Date();

        var month = dtToday.getMonth() + 1;
        var day = dtToday.getDate();
        var year = dtToday.getFullYear() + 1;

        if (month < 10)
            month = '0' + month.toString();
        if (day < 10)
            day = '0' + day.toString();
        var minDate = dtToday.getFullYear() + '-' + month + '-' + day;
        var maxDate = year + '-' + month + '-' + day;
        $('#txtDispatchDate').attr('min', minDate);
        $('#txtDispatchDate').attr('max', maxDate);
    });

    $('#txtDispatchDate').on('keydown', (e) => {
        e.preventDefault();
        return false;
    });


    mMsater(0, "ddlDispatch", DispatchMode, "");
    let ClaimValue = $("#spnClaimValue").html();
    let Field = $("#spnField").html();
    let SearchText = $("#spnSearchText").html();
    if (ClaimValue == 1) {
        let CategoryId = Field === "recordofficename" ? 1 : 2;
        let RecordRegimentId = SearchText;

        $("#txtUnitName").attr('readonly', true);
        
        await GetDispatchToData(CategoryId, RecordRegimentId);
    }
    else if (ClaimValue == 2) {
        await GetDDMappedForRecord(SearchText);
        if ($('#txtUnitName').attr('readonly') !== undefined) {
            $("#txtUnitName").removeAttr('readonly');
        }
    }
    else if (ClaimValue == 3) {
        await GetDDMappedForRecord(SearchText);
        if ($('#txtUnitName').attr('readonly') !== undefined) {
            $("#txtUnitName").removeAttr('readonly');
        }
    }
    $('#ddlDID').on('change', async function () {
        let AspNetUsersId = $(this).val(); // Get the selected value
        await GetUserIdWithName(AspNetUsersId);
    });
+

    $("#btnSubmit").on('click', async function (e) {
        let formId = '#SaveDispatchCard';
        var fileInput = $('#CSVFile')[0];
        var file = fileInput.files[0];
        $.validator.unobtrusive.parse($(formId));

        // 1. Check CSV File Validation
        const isValid = await validateCsvFileOnChange();
        if (!isValid) {
            return false; // If CSV file is not valid, stop further execution
        }

        // 2. Check Form Validation
        if ($(formId).valid()) {
            Swal.fire({
                title: 'Are you sure?',
                text: "",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Save it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    Save(file);
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
async function Save(file) {
    try
    {
        let ClaimValue = parseInt($("#spnClaimValue").html());
        var token = $('input[name="__RequestVerificationToken"]').val();
        let Categery = parseInt($("#ddlCategery").val());
        let formData = new FormData();

        formData.append('ApplyForId', $("#ddlCategery").val());
        formData.append('Step', ClaimValue == 1 ? 1 : 2);
        formData.append('DispatchDate', convertToISOWithTime($("#txtDispatchDate").val()));
        formData.append('DispatchModeId', $("#ddlDispatch").val());
        formData.append('NameOfCourierIncharge', $("#txtCourierIncharge").val());
        formData.append('RefOfDispatch', $("#txtRefOfDispatch").val());
        formData.append('LotNo', $("#txtLotNo").val());
        formData.append('FromRemark', $("#txtFromRemark").val());
        formData.append('RegId', Categery == 2 ? $("#ddlRecordRegiment").val() : '');
        formData.append('RecordOfficeId', Categery == 1 ? $("#ddlRecordRegiment").val() : '');
        formData.append('ToUnitId', $("#spnUnitMapId").html());
        formData.append('ToAspNetUsersId', $("#ddlDID").val());
        formData.append('ToUserId', $("#spnUserId").html());
        formData.append('CSVFile', file); // File is added here

        // Append the CSRF token if needed (depends on your backend configuration)
        formData.append('__RequestVerificationToken', token);

        const response = await fetch('/BasicDetail/DispatchOut', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            body: formData
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (Boolean(result.Result)) {
            let responseHtml = `<p><strong>Total Records:</strong> ${result.Value.TotalRecords}</p>
                                <p><strong>Valid Records:</strong> ${result.Value.ValidRecords}</p>
                                <p><strong>SheetInvalid Records:</strong> ${result.Value.SheetInValidRecords}</p>
                                <p><strong>DbInvalid Records:</strong> ${result.Value.DbInValidRecords}</p>`;
            Swal.fire({
                title: "Validation Complete!",
                text: "Please download validated CSV with remarks.",
                html: responseHtml,
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
                        //window.open(`/WriteReadData/CardDispatchCSVs/CSVWithRemarks/${result.Value.FileName}`, '_blank');
                        const fileUrl = `/WriteReadData/CardDispatchCSVs/CSVWithRemarks/${result.Value.FileName}`;
                        const link = document.createElement('a');
                        link.href = fileUrl;
                        link.download = result.Value.FileName; // This will prompt the file to download instead of opening it in a new tab
                        document.body.appendChild(link); // Append the link to the document
                        link.click(); // Trigger the download
                        document.body.removeChild(link); // Clean up by removing the link
                    };

                    const proceedBtn = document.createElement('button');
                    proceedBtn.textContent = 'Proceed';
                    proceedBtn.className = 'swal2-confirm swal2-styled';
                    proceedBtn.style.backgroundColor = '#007bff'; // blue
                    proceedBtn.onclick = function () {
                        Swal.close();
                        $.ajax({
                            url: '/BasicDetail/ICardDispatchValidRecordsUpload',
                            type: 'GET',
                            dataType: 'json',
                            success: function (data) {
                                if (data.Result) {
                                    Swal.fire({
                                        title: "Success!",
                                        text: data.Message,
                                        icon: "success",
                                        confirmButtonText: "OK"
                                    });
                                }
                                else {
                                    Swal.fire({
                                        title: "OOPs!",
                                        text: data.Message,
                                        icon: "error",
                                        confirmButtonText: "Ok"
                                    });
                                }
                            },
                            error: function (xhr, status, error) {
                                console.error('Error while uploading valid records:', error);
                            }
                        });
                    };

                    const cancelBtn = document.createElement('button');
                    cancelBtn.textContent = 'Cancel';
                    cancelBtn.className = 'swal2-cancel swal2-styled';
                    cancelBtn.style.backgroundColor = '#dc3545'; // red
                    cancelBtn.onclick = function () {
                        Swal.close();
                    };

                    btnGroup.appendChild(downloadBtn);
                    if (result.Value.ValidRecords > 0) {
                        btnGroup.appendChild(proceedBtn);
                    }

                    btnGroup.appendChild(cancelBtn);

                    swal.appendChild(btnGroup);
                }
            });

        } else {

            if (result.Message.length > 0) {

                let messages = result.Message.split(';');
                messages.forEach(msg => {
                    toastr.error(msg);
                });
            }
        }
    }
     catch (error) {
    alert("Error: " + error.message);
    }
}
async function GetDDMappedForRecord(UnitMapId) {
    let param = new URLSearchParams({
        "UnitMapId": UnitMapId
    });

    try {
        const response = await fetch('/Master/GetDDMappedForRecord', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (result != "null" && result != null) {
            if (response == InternalServerError) {
                Swal.fire({
                    text: errormsg
                });
            }

            else {

                var listItemddl = "";

                listItemddl += '<option value="0">Please Select</option>';

                for (var i = 0; i < result.length; i++) {
                    listItemddl += `<option value="${result[i].AspNetUsersId}">${result[i].DomainId} ${result[i].RankAbbreviation} ${result[i].Name} ${result[i].ArmyNo}</option>`;
                }
                $("#ddlDID").html(listItemddl);
            }
        }
        else {
            //Swal.fire({
            //    text: "No data found Offrs"
            //});
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}

async function GetUserIdWithName(AspNetUsersId) {
    let param = new URLSearchParams({
        "AspNetUsersId": AspNetUsersId
    });

    try {
        const response = await fetch('/BasicDetail/GetUserIdWithName', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (Boolean(result.Result)) {
            $("#txtArmyNo").val(`${result.Value.ArmyNo}`);
            $("#spnUserId").html(result.Value.UserId);

            $("#txtRkName").val(`${result.Value.RankAbbreviation} ${result.Value.Name}`);

        } else {
            $("#spnUserId").html(0);
            $("#txtRkName").val(``);
            $("#txtArmyNo").val(``);
            toastr.error(`${result.Message}`);
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}
async function GetDispatchToData(CategeryId, RecordRegimentId) {
    let param = new URLSearchParams({
        "CategeryId": CategeryId,
        "RecordRegimentId": RecordRegimentId
        });

    try {
        const response = await fetch('/BasicDetail/GetDispatchToData', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (Boolean(result.Result)) {
            $("#txtUnitName").val(`${result.Value.Sus_no} ${result.Value.UnitAbbreviation}`);
            $("#spnUnitMapId").html(result.Value.UnitId);


            let listItemddl = "";
            listItemddl += '<option value="0">Select Domain Id</option>';
            listItemddl += `<option value="${result.Value.AspNetUsersId}">${result.Value.DomainId}</option>`;

            $("#ddlDID").html(listItemddl);
            $("#ddlDID").val(`${result.Value.AspNetUsersId}`);

            $("#txtArmyNo").val(`${result.Value.ArmyNo}`);
            $("#spnUserId").html(result.Value.UserId);

            $("#txtRkName").val(`${result.Value.RankAbbreviation} ${result.Value.Name}`);

        }
        else {
            Reset();
            toastr.error(`${result.Message}`);
        }

    } catch (error) {
        Reset();
        alert("Error: " + error.message);
    }
}
function Reset() {
    $("#spnUnitMapId").html(0);
    $("#spnUserId").html(0);

    $("#txtUnitName").val(``);
    $("#txtArmyNo").val(``);
    $("#txtRkName").val(``);
    //$('#ddlDID').empty();

    //$("#ddlRecordRegiment").find("option").not(":first").remove();
    //$("#ddlRecordRegiment").val("0");

    $("#ddlDID").find("option").not(":first").remove();
    $("#ddlDID").val("0");
}