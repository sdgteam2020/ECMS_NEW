$(document).ready(function () {
    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#ServiceNumber").val(sessionStorage.getItem("ArmyNo"));


        $("#ApplyForId").val(sessionStorage.getItem("OffType"));
        $("#RegistrationId").val(sessionStorage.getItem("RegistrationApplyFor"));
        $("#TypeId").val(sessionStorage.getItem("lCardType"));

        Getdatafromapi();

        getApplyIcardDetails();
    }
    $('.select2').select2({
       
        closeOnSelect: false
    });
    var someNumbers = [1];
    GetRemarks("ddlRemarks", 0, someNumbers);


    $('#ddlRemarks').on('change', function () {
        $("#RemarksIds").val($('#ddlRemarks').val()); 
    });


});
function getApplyIcardDetails()
{
    $.ajax({
        url: "/Home/GetApplyCardDetails",
        type: "POST",
        data: {
            "ApplyForId": $("#ApplyForId").val(),
            "RegistrationId": $("#RegistrationId").val(),
            "TypeId": $("#TypeId").val()
        },
        success: function (response, status) {
            if (response != null) {

                $("#icarddetails").html('For '+response.ApplyFor + ' And (' + response.Registraion + ') For ' + response.Type);
            }
            
        }
    }); 
}

function getData(id) {
    //let formId = '#' + id;
    //// Check if the form exists
    //if ($(formId).length === 0) {
    //    console.error("Form not found.");
    //    return;
    //}
    /*$("#RegistrationId").prop('required', true);*/
    $("#ServiceNumber").prop('required', true);
    //$.validator.unobtrusive.parse($(formId));
    //if ($(formId).valid()) {
    //    var formData = $(formId).serialize();
    //    console.log(formData);
    //}
    //else {
    //    return false;
    //}
    ////let regId = $("#RegistrationId").find(":selected").val();

    var param = { "ArmyNo": $("#ServiceNumber").val() };

  
    Getdatafromapi();

}
function Getdatafromapi() {

    $.ajax({
        url: "/BasicDetail/GetData",
        type: "POST",
        data: {
            "ICNumber": $("#ServiceNumber").val()
        },
        success: function (response, status) {
            if (response.Status == false) {
                alert(response.Message)
            }
            else {

                CallDataFromAPI();
                //alert(JSON.stringify(response));
                //$("#Name").val(response.Name);
                //$("#ServiceNo").val(response.ServiceNo);
                //$("#DOB").val(response.DOB);
                //$("#DOB_").val(moment(response.DOB).format("DD-MMM-YYYY"));
                //$("#DateOfCommissioning").val(response.DateOfCommissioning);
                //$("#DOC").val(moment(response.DateOfCommissioning).format("DD-MMM-YYYY"));
                //$("#PermanentAddress").val(response.PermanentAddress);
                //$("#RegId").val(regId);
            }
        }
    });
}
function CallDataFromAPI() {
    $.ajax({
        url: "/Api/LoginApi",
        type: "POST",
        data: {
            "ICNumber": $("#ServiceNumber").val()
        },
        success: function (response, status) {
            if (response.Status == false) {

                toastr.error(response.Message);
               
            }
            else {
                toastr.success('Data Get From Api');
               // $("#Name").val(response.Pers_name);
                //alert(JSON.stringify(response));
                $("#Name").val(response.Pers_name);
                $("#ServiceNo").val(response.Pers_Army_No);
                $("#DOB").val(response.Pers_birth_dt);
                /*$("#DOB_").val(moment(response.Pers_birth_dt).format("DD-MM-YYYY"));*/
                $("#DateOfCommissioning").val(response.Pers_enrol_dt);
                /*$("#DOC").val(moment(response.Pers_enrol_dt).format("DD-MM-YYYY"));*/
                $("#PermanentAddress").val('Village - ' + response.Pers_Village + '\n Post Office-' + response.Pers_Post_office + ' \n Tehsil- ' + response.Pers_Tehsil + '\n District- ' + response.Pers_District + '\n State- ' + response.Pers_State + '\n Pin Code- ' + response.Pers_Pin_code);
                //$("#RegId").val(regId);

                $("#State").val(response.Pers_State);
                $("#District").val(response.Pers_District);
                $("#PS").val(response.Pers_Police_stn);
                $("#PO").val(response.Pers_Post_office);
                $("#Tehsil").val(response.Pers_Tehsil);
                $("#Village").val(response.Pers_Village);
                $("#PinCode").val(response.Pers_Pin_code);
                $("#IdenMark1").val(response.Pers_Iden_mark_1);
                $("#IdenMark2").val(response.Pers_Iden_mark_2);
                $("#AadhaarNo").val(response.Pers_UID);
                //if (response.Pers_Height!="")
                //    $("#Height").val(response.Pers_Height);
                //else
                //    $("#Height").val(0);

                $("#BloodGroup").val(response.Pers_Blood_Gp);
            }
        }
    });
}
function registrationEnableDisabledField(val) {
    $("#ddlRemarks").val("");
    $("#RemarksIds").val("");
    if (val == 1) {
        $("#btnsubmit").prop('disabled', false);
        $("#Observations").val('');
        $("#Observations").prop('readonly', true);
        $(".Remarks").addClass("d-none");
        $("#btnsubmit").text("Process I-Card");
    }
    else {
        $("#btnsubmit").prop('disabled', false);
        $("#Observations").prop('readonly', false);
        $(".Remarks").removeClass("d-none");
        $("#btnsubmit").text("Send Inaccurate Data");
    }

}
function Proceed(id) {
    let formId = '#' + id;

    
    // Check if the form exists
    if ($(formId).length === 0) {
        console.error("Form not found.");
        return;
    }
    let stype = parseInt($("input[name='SubmitType']:checked").val());
    if (stype != 1) {
        $("#Observations").prop('required', true);
        $("#lblObservations").text('Observations is required.')
    }
    if ($("#DOB").val() == '') {
        $("#lblDOB").text('Date of Birth is required.')
    }
    else {
        $("#lblDOB").text('')
    }
    if ($("#DOC").val() == '') {
        $("#lblDateOfCommissioning").text('Date of Commissioning/ Enrollment is required.')
    }
    else {
        $("#lblDateOfCommissioning").text('')
    }
    $.validator.unobtrusive.parse($(formId));
    if ($(formId).valid()) {

        if (stype == 1) {
            Swal.fire({
                title: "Are you sure?",
                text: "You Want to Process I-Card Request",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, Process it!"
            }).then((result) => {
                if (result.isConfirmed) {

                    $("#Registration").submit();
                }
                else {
                    return false;
                }
            });
        }
        else {
            Swal.fire({
                title: "Are you sure?",
                text: "You Want to Send Inaccurate Data",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, Send it!"
            }).then((result) => {
                if (result.isConfirmed) {

                    $("#Registration").submit();
                }
                else {
                    return false;
                }
            });
        }
       
    }
    else {
        return false;
    }
}