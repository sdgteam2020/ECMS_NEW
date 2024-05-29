$(document).ready(function () {
    mMsater(0, "ddlCommand", 1, "");
   
    $('#ddlCommand').on('change', function () {
       
        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
       
    });

    $('#ddlCorps').on('change', function () {

        mMsaterByParent(0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
       
    });
    $('#ddlDiv').on('change', function () {

        mMsaterByParent(0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0);///ComdId,CorpsId,DivId,BdeId
       

    });
    //$('#ddlBde').on('change', function () {
       
    //});
  
    $('#txtSusno').on('input', function () {
        $("#txtUnit").val("");
        $("#SpnUnitMapId").html(0);
        $('#txtUnit').attr('readonly', false);
        if ($(this).val().length > 7) {
            GetUnitDetails($(this).val(), 0);
        }
    });

});
function GetUnitDetails(val, flag) {
    var userdata =
    {
        "Sus_no": val,

    };
    $.ajax({
        url: '/Master/GetBySusNO',
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
                else if (response == 0) {
                    $("#txtUnit").val(""); $("#SpnUnitMapId").html(0); $('#txtUnit').attr('readonly', false);
                }

                else {

                    $("#txtUnit").val(response.Unit_desc);
                    $('#txtUnit').attr('readonly', true);
                    $("#SpnUnitMapId").html(response.UnitId);

                    if (flag == 2) {
                        SaveUnitMap();
                    }

                }
            }
            else {
                $("#txtUnit").val("");
                $("#SpnUnitMapId").html(0);
                $('#txtUnit').attr('readonly', false);
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}


function UnitSave() {

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/Master/SaveUnit',
        type: 'POST',
        data: { "Sus_no": $("#txtSusno").val().substring(0, 7), "UnitId": 0, "Suffix": $("#txtSusno").val().substring(8, 7), "Unit_desc": $("#txtUnit").val(), "IsVerify": false }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Unit has been saved');
                GetUnitDetails($("#txtSusno").val(), 2);
                /*  $("#AddNewM").modal('hide');*/

            }
            else if (result == DataUpdate) {
                toastr.success('Unit has been Updated');

                /*  $("#AddNewM").modal('hide');*/


            }
            else if (result == DataExists) {

                toastr.error('Unit Name Exits!');

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