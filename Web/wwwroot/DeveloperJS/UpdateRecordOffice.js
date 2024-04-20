$(document).ready(function () {
    GetDDMappedForRecord($("#spnUnitMapId").html(), $("#spnOldTDMId").html());
    GetUpdateRecordOffice();
    $("#btnROUpdate").on("click", function () {
        Proceed();
    });
});
function GetDDMappedForRecord(UnitMapId, TDMId) {
    var userdata =
    {
        "UnitMapId": UnitMapId,
    };
    $.ajax({
        url: '/Master/GetDDMappedForRecord',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
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
                        listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                    }
                    $("#ddlTDMId").html(listItemddl);
                    if (TDMId != '') {
                        $("#ddlTDMId").val(TDMId);

                    }
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
}
function GetUpdateRecordOffice() {
    $.ajax({
        url: '/Master/GetUpdateRecordOffice',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',
        success: function (data) {
            $("#spnRecordOfficeId").html(data.RecordOfficeId);
            $("#lblROName").html(data.RecordOfficeName);
            $("#lblROAbbreviation").html(data.Abbreviation);
            $("#lblArms").html(data.ArmedName);
        }
    });
}

function Proceed() {
    ResetErrorMessage();

    let formId = '#UpdateRecordOffice';
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be Save!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Save it!'
        }).then((result) => {
            if (result.isConfirmed) {
                Update();
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
}
function Update() {

    $.ajax({
        url: '/Master/UpdateROValue',
        type: 'POST',
        data: { "RecordOfficeId": $("#spnRecordOfficeId").html(), "TDMId": $("#ddlTDMId").val(), "OldTDMId": $("#spnOldTDMId").html(), "OldUserId": $("#spnOldUserId").html() }, 
        success: function (result) {
            if(result == 1) {
                toastr.error('Select diffrent Domain Id.');
            }
            else if (result == 2) {
                toastr.success('Domain Id has been Updated');
            }
            else if (result == 3)
            {
                toastr.error('Id not present.');
            }
            else if (result == 4) {
                toastr.error('Invalid Entry!');
            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }


            }
        }
    });
}
function Reset() {
    $("#spnRecordOfficeId").html("0");
    $("#spnTrnDomainMappingId").html("0");
    $("#ddlTDMId").val("");
}
function ResetErrorMessage() {
    $("#ddlTDMId-error").html("");
}