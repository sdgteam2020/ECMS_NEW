$(document).ready(function () {
    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#ServiceNumber").val(sessionStorage.getItem("ArmyNo"));
        Getdatafromapi();
    }
});

function getData(id) {
    let formId = '#' + id;
    // Check if the form exists
    if ($(formId).length === 0) {
        console.error("Form not found.");
        return;
    }
    /*$("#RegistrationId").prop('required', true);*/
    $("#ServiceNumber").prop('required', true);
    $.validator.unobtrusive.parse($(formId));
    if ($(formId).valid()) {
        var formData = $(formId).serialize();
        console.log(formData);
    }
    else {
        return false;
    }
    //let regId = $("#RegistrationId").find(":selected").val();

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

               // $("#Name").val(response.Pers_name);
                //alert(JSON.stringify(response));
                $("#Name").val(response.Pers_name);
                $("#ServiceNo").val(response.Pers_Army_No);
                $("#DOB").val(response.Pers_birth_dt);
                /*$("#DOB_").val(moment(response.Pers_birth_dt).format("DD-MM-YYYY"));*/
                $("#DateOfCommissioning").val(response.Pers_enrol_dt);
                /*$("#DOC").val(moment(response.Pers_enrol_dt).format("DD-MM-YYYY"));*/
                $("#PermanentAddress").val(response.Pers_Village + ', ' + response.Pers_Post_office + ', ' + response.Pers_Tehsil + ', ' + response.Pers_District + ', ' + response.Pers_State + ', ' + response.Pers_Pin_code);
                //$("#RegId").val(regId);
            }
        }
    });
}
function registrationEnableDisabledField(val) {
    if (val == 1) {
        $("#btnsubmit").prop('disabled', false);
        $("#Observations").val('');
        $("#Observations").prop('readonly', true);
    }
    else {
        $("#btnsubmit").prop('disabled', false);
        $("#Observations").prop('readonly', false);
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
        var formData = $(formId).serialize();
        console.log(formData);
    }
    else {
        return false;
    }
}