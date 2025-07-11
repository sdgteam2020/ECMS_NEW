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
    if (ClaimValue == 1) {
        $("#DispatchOut_Categery").removeClass("d-none");

        $("#txtUnitName").attr('readonly', true);
        
        $('#ddlCategery').on('change', async function () {
            let CategeryId = $(this).val(); // Get the selected value
            await GetddlRecordRegiment(CategeryId);
        });
        $('#ddlRecordRegiment').on('change', async function () {
            let RecordRegimentId = parseInt($(this).val()); // Get the selected value

            if (RecordRegimentId == 0) {
                $("#spnUnitMapId").html(0);
                $("#spnUserId").html(0);

                $("#txtUnitName").val(``);
                $("#txtArmyNo").val(``);
                $("#txtRkName").val(``);

                $("#ddlDID").find("option").not(":first").remove();
                $("#ddlDID").val("0");
            }
            if (RecordRegimentId > 0) {
                await GetDispatchToData($('#ddlCategery').val(), RecordRegimentId);
            }
        });
    }
    else if (ClaimValue == 2) {
        $("#DispatchOut_Categery").addClass("d-none");

        if ($('#txtUnitName').attr('readonly') !== undefined) {
            $("#txtUnitName").removeAttr('readonly');
        }

        $("#ddlCategery").val(1); // Set default value for Categery
        await GetddlRecordRegiment(1);
    }
    else if (ClaimValue == 3) {
        $("#DispatchOut_Categery").addClass("d-none");

        if ($('#txtUnitName').attr('readonly') !== undefined) {
            $("#txtUnitName").removeAttr('readonly');
        }
        $("#ddlCategery").val(2); // Set default value for Categery
        await GetddlRecordRegiment(2);
    }
    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                $("#spnUnitMapId").html('');
                var param = { "UnitName": request.term };
                $("#spnUnitMapId").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return {
                                    label: `${item.Sus_no}${item.Suffix} ${item.UnitName}`,
                                    value: `${item.UnitMapId}`
                                };

                            }))
                        }
                        else {
                            $("#txtUnitName").val("");
                            $("#spnUnitMapId").html("");
                            $("#ddlDID").find("option").not(":first").remove();
                            $("#ddlDID").val("0");

                            $("#spnUserId").html(0);
                            $("#txtRkName").val(``);
                            $("#txtArmyNo").val(``);

                            alert("Unit not found.")
                        }
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtUnitName").val(i.item.label);
            $("#spnUnitMapId").html(i.item.value);
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetDDMappedForRecord',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',

                success: function (response) {
                    if (response != "null" && response != null) {
                        if (response == InternalServerError) {
                            Swal.fire({
                                text: errormsg
                            });
                        }

                        else {

                            var listItemddl = "";

                            listItemddl += '<option value="0">Please Select</option>';

                            for (var i = 0; i < response.length; i++) {
                                listItemddl += `<option value="${response[i].AspNetUsersId}">${response[i].DomainId} ${response[i].RankAbbreviation} ${response[i].Name} ${response[i].ArmyNo}</option>`;
                            }
                            $("#ddlDID").html(listItemddl);
                        }
                    }
                    else {
                        //Swal.fire({
                        //    text: "No data found Offrs"
                        //});
                    }
                },
                error: function (result) {
                    Swal.fire({
                        text: errormsg002
                    });
                }
            });
        },
        appendTo: '#suggesstion-box'
    });

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
async function validateCsvFileOnChange() {
    var fileInput = $('#CSVFile')[0];
    var file = fileInput.files[0];

    if (!file) {
        toastr.error('Please select a CSV file.');
        return false;
    }

    var fileType = file.name.split('.').pop().toLowerCase();
    if (fileType !== 'csv') {
        toastr.error('Only CSV files are allowed.');
        return false;
    }

    return new Promise((resolve) => {
        var reader = new FileReader();
        reader.onload = function (event) {
            var content = event.target.result;
            const lines = content.split(/\r\n|\n/).filter(line => line.trim() !== "");
            if (lines.length === 0) {
                toastr.error('The selected file is empty.');
                resolve(false);
                return;
            }
            if (lines.length < 2) {
                toastr.error('The CSV file must contain at least 1 data row.');
                resolve(false);
                return;
            }
            var headers = lines[0].split(",");
            var expectedColumns = ['ChipNo'];
            var missingColumns = expectedColumns.filter(col => !headers.includes(col));
            var duplicateColumns = headers.filter((value, index, self) => self.indexOf(value) !== index);

            if (missingColumns.length > 0) {
                toastr.error('Missing columns: ' + missingColumns.join(', '));
                resolve(false);
                return;
            }
            if (duplicateColumns.length > 0) {
                toastr.error('Duplicate columns found: ' + duplicateColumns.join(', '));
                resolve(false);
                return;
            }
            resolve(true); // Resolve as true if all validations pass
        };
        reader.onerror = function () {
            toastr.error('Error reading the CSV file.');
            resolve(false); // Resolve false if there is an error reading the file
        };
        reader.readAsText(file);
    });
}
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
async function GetddlRecordRegiment(CategeryId) {
    let param = new URLSearchParams({ CategeryId: CategeryId });

    try {
        const response = await fetch('/BasicDetail/GetddlRecordRegiment', {
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
            let listItemddl = "";
            listItemddl += '<option value="0">Select Record Office / Regiment</option>';

            for (let i = 0; i < result.Value.length; i++) {
                listItemddl += `<option value="${result.Value[i].Id}">${result.Value[i].Name}</option>`;
            }

            document.getElementById("ddlRecordRegiment").innerHTML = listItemddl;

            $("#spnUnitMapId").html(0);
            $("#spnUserId").html(0);

            $("#txtUnitName").val(``);
            $("#txtArmyNo").val(``);
            $("#txtRkName").val(``);

            $('#ddlDID').empty();

        } else {
            toastr.error(`${result.Message}`);
            Reset();
        }

    } catch (error) {
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