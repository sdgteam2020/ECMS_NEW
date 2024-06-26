﻿$(document).ready(function () {

    //const [today] = new Date().toISOString().split('T');
    //const maxDate = new Date();
    //maxDate.setDate(maxDate.getDate() + 30);
    //const [maxDateFormatted] = maxDate.toISOString().split('T');
    //const dateInput = document.getElementById('DOB');
    //dateInput.setAttribute('min', today);
    //dateInput.setAttribute('max', maxDateFormatted);
    //const Commissioning = document.getElementById('Commissioning');
    //Commissioning.setAttribute('min', today);
    //Commissioning.setAttribute('max', maxDateFormatted);
    $('.paddress').on('change', function () {
        $("#PermanentAddress").val('Village - ' + $("#Village").val() + '\n Post Office-' + $("#PO").val() + ' \n Tehsil- ' + $("#Tehsil").val() + '\n District- ' + $("#District").val() + '\n State- ' + $("#State").val() + '\n Pin Code- ' + $("#PinCode").val());
        $("#AadhaarNo").val("");
       
    });
    $("#btngetdata").on("click", function () {
        $("#ServiceNumber").prop('required', true);
        Getdatafromapi();
    });
    $("#btnsubmit").on("click", function () {
        Proceed('Registration');
    });
   

    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#ServiceNumber").val(sessionStorage.getItem("ArmyNo"));

        $("#icarddetails").html(sessionStorage.getItem("ArmyNo"));
        $("#ApplyForId").val(sessionStorage.getItem("OffType"));
        $("#RegistrationId").val(sessionStorage.getItem("RegistrationApplyFor"));
        $("#TypeId").val(sessionStorage.getItem("lCardType"));

        if (sessionStorage.getItem("RegistrationApplyFor") == '4' || sessionStorage.getItem("RegistrationApplyFor") == '9') {
            $('#Name').attr('readonly', false);
            $('#NameAsPerRecord').attr('readonly', false);
            $('#DOB').attr('readonly', false);
            $('#ServiceNo').attr('readonly', false);
            $('#DateOfCommissioning').attr('readonly', false);
            $('.persAddress').addClass('d-none');
            $('.entryaddress').removeClass('d-none');

            $("#ServiceNo").val($("#ServiceNumber").val());
            $(".spnhideServiceNo").addClass('d-none');
        } else {
            $('#Name').attr('readonly', true);
            $('#NameAsPerRecord').attr('readonly', true);
            $('#DOB').attr('readonly', true);
            $('#ServiceNo').attr('readonly', true);
            $('#DateOfCommissioning').attr('readonly', true);
            $('.persAddress').removeClass('d-none');
            $('.entryaddress').addClass('d-none');
        }

       /* Getdatafromapi();*/

        getApplyIcardDetails();
    }
    $('.select2').select2({
       
        closeOnSelect: false
    });
    var someNumbers = [4];
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
                $("#lblCategory").html(response.ApplyFor);
                $("#lblReason").html(response.Type);

                $("#RegdUser").html(response.RankAbbreviation + ' ' + response.Name + ' (' + response.ArmyNo + ') (' + response.DomainId + ')');

                if ($("#RegistrationId").val() == '3' || $("#RegistrationId").val() == '7') {
                    $("#lblunitname").html(response.Registraion);
                } else {
                    $("#lblunitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix + ')');
                }


               
                mMsater(0, "ArmedId", ArmyType, "");
              
                if ($("#ApplyForId").val() == 1) {

                    mMsater(0, "RankId", Rank, "");
                } else if ($("#ApplyForId").val() == 2) {
                    mMsater(0, "RankId", RankJCo, "");

                }

               
            }
            
        }
    }); 
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
            }
        }
    });
}
function CallDataFromAPI() {
    $.ajax({
        url: "/Api/LoginApi",
        type: "POST",
        data: {
            "ICNumber": $("#ServiceNumber").val(),
            "Type": $("#ApplyForId").val()
        },
        success: function (response, status) {
            if (response.Status == false) {

                toastr.error(response.Message);
                $("#btngetdata").removeClass("btn-primary");
                $("#btngetdata").addClass("btn-danger");
            }
            else {
                $("#btngetdata").removeClass("btn-primary");
                $("#btngetdata").addClass("btn-success");
                toastr.success('Data Fetched From Api');
               // $("#Name").val(response.Pers_name);
                //alert(JSON.stringify(response));
                if (response.Pers_name.length > 18) {
                    $('#FName').attr('readonly', false);
                    $('#LName').attr('readonly', false);
                }
                else {
                    $("#FName").val(response.Pers_name);
                    $('#FName').attr('readonly', true);
                    $('#LName').attr('readonly', true);
                }
                $("#NameAsPerRecord").val(response.Pers_name);
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
        $("#btnsubmit").removeClass("btn-danger");
        $("#btnsubmit").addClass("btn-success");
    }
    else {
        $("#btnsubmit").prop('disabled', false);
        $("#Observations").prop('readonly', false);
        $(".Remarks").removeClass("d-none");
        $("#btnsubmit").text("Raised Obsn");
        $("#btnsubmit").removeClass("btn-success");
        $("#btnsubmit").addClass("btn-danger");
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