var ClaimValue;
var Field;
var SearchText;
var CategoryId;
var ToUnitId;
var ToUserId;
var RecordRegimentId;
$(async function () {
    const now = moment();                 // current date-time
    const max = moment().add(1, 'month'); // +1 month

    if ($('#txtDispatchDate').data('DateTimePicker')) {
        $('#txtDispatchDate').data('DateTimePicker').destroy();
    }

    $('#txtDispatchDate').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        sideBySide: true,
        stepping: 15,
        useCurrent: false,
        minDate: now,  // block past
        maxDate: max   // allow only up to 1 month from now
    }).on('dp.show', function () {
        // Refresh minDate in case page was open long time
        $(this).data('DateTimePicker').minDate(moment());
    });
    $('#txtDispatchDate').on('keydown', (e) => {
        e.preventDefault();
        return false;
    });


    mMsater(0, "ddlDispatch", DispatchMode, "");
    ClaimValue = parseInt($("#spnClaimValue").html());
    Field = $("#spnField").html();
    SearchText = $("#spnSearchText").html();
    if (ClaimValue == 1) {
        CategoryId = Field === "recordofficename" ? 1 : 2;
        RecordRegimentId = parseInt(SearchText);

        if (isNaN(RecordRegimentId)) {
            toastr.error('Invalid  ORO / Regiment.');
        }
        else {
            await GetDispatchToData(CategoryId, RecordRegimentId);
        }
    }
    else if (ClaimValue == 2) {
        CategoryId = 1;
        ToUnitId = parseInt(SearchText);
        if (isNaN(ToUnitId)) {
            toastr.error('Invalid  Unit.');
        }
        else {
            await GetddlRecordRegiment();
        }
    }
    else if (ClaimValue == 3) {
        CategoryId = 2;
        ToUnitId = parseInt(SearchText);
        if (isNaN(ToUnitId)) {
            toastr.error('Invalid  Unit.');
        }
        else {
            await GetddlRecordRegiment();
        }
    }
    $('#ddlDID').on('change', async function () {
        let AspNetUsersId = $(this).val(); // Get the selected value
        await GetUserIdWithName(AspNetUsersId);
    });
+

    $("#btnSubmit").on('click', async function (e) {
        let formId = '#SaveDispatchCard';
        $.validator.unobtrusive.parse($(formId));

        // 1. Check Form Validation
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
async function Save() {
    try
    {
        $("#loading").show();

        var token = $('input[name="__RequestVerificationToken"]').val();
        let formData = new FormData();

        formData.append('ApplyForId', CategoryId);
        formData.append('Step', ClaimValue == 1 ? 1 : 2);
        formData.append('DispatchDate', convertToISOWithTime_dtp($("#txtDispatchDate").val()));
        formData.append('DispatchModeId', $("#ddlDispatch").val());
        formData.append('NameOfCourierIncharge', $("#txtCourierIncharge").val());
        formData.append('RefOfDispatch', $("#txtRefOfDispatch").val());
        formData.append('FromRemark', $("#txtFromRemark").val());
        formData.append('RegId', CategoryId == 2 ? RecordRegimentId : '');
        formData.append('RecordOfficeId', CategoryId == 1 ? RecordRegimentId : '');
        formData.append('ToUnitId', ToUnitId);
        formData.append('ToAspNetUsersId', $("#ddlDID").val());
        formData.append('ToUserId', ToUserId);

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
            let responseHtml = `<p><strong>Lot No : </strong> ${result.Value.LotNo}</p>
                                <p><strong>Total Records:</strong> ${result.Value.TotalRecords}</p>
                                <p><strong>Valid Records:</strong> ${result.Value.ValidRecords}</p>
                                <p><strong>DbInvalid Records:</strong> ${result.Value.DbInValidRecords}</p>`;
            Swal.fire({
                title: "Dispatch Process Completed!",
                text: "Please download CSV with remarks.",
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

                    const closedBtn = document.createElement('button');
                    closedBtn.textContent = 'Close';
                    closedBtn.className = 'swal2-cancel swal2-styled';
                    closedBtn.style.backgroundColor = '#dc3545'; // red
                    closedBtn.onclick = function () {
                        Swal.close();
                        location.href = '/BasicDetail/DispatchCard';
                    };

                    btnGroup.appendChild(downloadBtn);
                    btnGroup.appendChild(closedBtn);

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
    finally {
        // Hide loader in all cases
        $("#loading").hide();
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
            ToUserId = result.Value.UserId;
            $("#txtArmyNo").val(`${result.Value.ArmyNo}`);
            $("#txtRkName").val(`${result.Value.RankAbbreviation} ${result.Value.Name}`);

        } else {
            ToUserId = 0;   
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
            $("#lblToUnitName").text(`${result.Value.Sus_no} ${result.Value.UnitAbbreviation}`);
            ToUnitId = result.Value.UnitId;
            ToUserId = result.Value.UserId;

            let listItemddl = "";
            listItemddl += '<option value="0">Select Domain Id</option>';
            listItemddl += `<option value="${result.Value.AspNetUsersId}">${result.Value.DomainId}</option>`;

            $("#ddlDID").html(listItemddl);
            $("#ddlDID").val(`${result.Value.AspNetUsersId}`);
            $("#txtArmyNo").val(`${result.Value.ArmyNo}`);
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
async function GetddlRecordRegiment() {
    let param = new URLSearchParams({
        "ToUnitId": ToUnitId
    });
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
            $("#lblToUnitName").text(`${result.Value.SUSNo} ${result.Value.UnitAbbreviation}`);
            $("#lblRecordRegiment").text(result.Value.Name);
            RecordRegimentId = result.Value.Id;

            await GetDDMappedForRecord(ToUnitId);

        } else {
            toastr.error(`${result.Message}`);
            Reset();
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}
async function GetDDMappedForRecord() {
    let param = new URLSearchParams({
        "UnitMapId": ToUnitId
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
function Reset() {
    ToUnitId=0;
    ToUserId=0;

    $("#lblToUnitName").text(``);
    $("#lblRecordRegiment").text(``);
    $("#txtArmyNo").val(``);
    $("#txtRkName").val(``);
    //$('#ddlDID').empty();

    //$("#ddlRecordRegiment").find("option").not(":first").remove();
    //$("#ddlRecordRegiment").val("0");

    $("#ddlDID").find("option").not(":first").remove();
    $("#ddlDID").val("0");
}