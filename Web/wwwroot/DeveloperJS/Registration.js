$(document).ready(function () {
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


    //$("#DOB").datepicker({
    //    changeMonth: true,
    //    changeYear:true,
    //    dateFormat: "dd-mm-yy" // Set the date format
    //});
    //$("#DOB_").datepicker({
    //    dateFormat: "dd-mm-yy" // Set the date format
    //});
    $(function () {
        var dtToday = new Date();

        var month = dtToday.getMonth() + 1;
        var day = dtToday.getDate();
        var year = dtToday.getFullYear() - 18;

        if (month < 10)
            month = '0' + month.toString();
        if (day < 10)
            day = '0' + day.toString();

        var maxDate = day + '/' + month + '/' + year;
        $('#DOB').attr('max', maxDate);
    });

    $('#DOB').on('change', function () {
        $("#DateOfCommissioning").val("");
        var dtToday = new Date();

        var month = dtToday.getMonth() + 1;
        var day = dtToday.getDate();
        var year = dtToday.getFullYear() - 18;

        if (month < 10)
            month = '0' + month.toString();
        if (day < 10)
            day = '0' + day.toString();

        var maxDate = day + '/' + month + '/' + year;
        $('#DateOfCommissioning').attr('min', maxDate);
    });
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

    $("#State").keyup(function () {
        if ($("#State").val().length == 0) {
            $("#lblState").html('State is required.');
        }
        else {
            $("#lblState").html('');
        }
    });

    $("#PS").keyup(function () {
        if ($("#PS").val().length == 0) {
            $("#lblPS").html('Police Station is required.');
        }
        else {
            $("#lblPS").html('');
        }
    });

    $("#Village").keyup(function () {
        if ($("#Village").val().length == 0) {
            $("#lblVillage").html('Village is required.');
        }
        else {
            $("#lblVillage").html('');
        }
    });

    $("#District").keyup(function () {
        if ($("#District").val().length == 0) {
            $("#lblDistrict").html('District is required.');
        }
        else {
            $("#lblDistrict").html('');
        }
    });

    $("#PO").keyup(function () {
        if ($("#PO").val().length == 0) {
            $("#lblPO").html('Post Office is required.');
        }
        else {
            $("#lblPO").html('');
        }
    });

    $("#Tehsil").keyup(function () {
        if ($("#Tehsil").val().length == 0) {
            $("#lblTehsil").html('Tehsil is required.');
        }
        else {
            $("#lblTehsil").html('');
        }
    });


    if (sessionStorage.getItem("ArmyNo") != null) {
        const encryptedArmyNo = sessionStorage.getItem("ArmyNo");
        const encryptedOffType = sessionStorage.getItem("OffType");
        const encryptedRegistrationApplyFor = sessionStorage.getItem("RegistrationApplyFor");
        const encryptedlCardType = sessionStorage.getItem("lCardType");

        const secretKey = document.getElementById("spnUniqueSecretKey").innerText;

        const decryptedArmyNo = CryptoJS.AES.decrypt(encryptedArmyNo, secretKey).toString(CryptoJS.enc.Utf8);
        const decryptedOffType = CryptoJS.AES.decrypt(encryptedOffType, secretKey).toString(CryptoJS.enc.Utf8);
        const decryptedRegistrationApplyFor = CryptoJS.AES.decrypt(encryptedRegistrationApplyFor, secretKey).toString(CryptoJS.enc.Utf8);
        const decryptedlCardType = CryptoJS.AES.decrypt(encryptedlCardType, secretKey).toString(CryptoJS.enc.Utf8);


        $("#ServiceNumber").val(decryptedArmyNo);

        $("#icarddetails").html(decryptedArmyNo);
        $("#ApplyForId").val(decryptedOffType);
        $("#RegistrationId").val(decryptedRegistrationApplyFor);
        $("#TypeId").val(decryptedlCardType);

        if (decryptedRegistrationApplyFor === "4" || decryptedRegistrationApplyFor === "9") {
            $('#FName').attr('readonly', false);
            $('#LName').attr('readonly', false);
            $('#NameAsPerRecord').attr('readonly', false);
            $('#DOB').attr('readonly', false);
            $('#ServiceNo').attr('readonly', false);
            $('#DateOfCommissioning').attr('readonly', false);
            $('.persAddress').addClass('d-none');
            $('.entryaddress').removeClass('d-none');

            $("#ServiceNo").val($("#ServiceNumber").val());
            $(".spnhideServiceNo").addClass('d-none');

            $("#State").prop('required', true);
            $("#District").prop('required', true);
            $("#PS").prop('required', true);
            $("#PO").prop('required', true);
            $("#Tehsil").prop('required', true);
            $("#Village").prop('required', true);
            $("#PinCode").prop('required', true);
            $("#PermanentAddress").prop('required', false);

            $('#DOB').removeClass('d-none');
            $("#DOB_").addClass('d-none');

            $('#DateOfCommissioning').removeClass('d-none');
            $("#DOC_").addClass('d-none');

        } else {
            $('#FName').attr('readonly', true);
            $('#LName').attr('readonly', true);
            $('#NameAsPerRecord').attr('readonly', true);
            $('#DOB').attr('readonly', true);
            $('#ServiceNo').attr('readonly', true);
            $('#DateOfCommissioning').attr('readonly', true);
            $('.persAddress').removeClass('d-none');
            $('.entryaddress').addClass('d-none');

            $("#State").prop('required', false);
            $("#District").prop('required', false);
            $("#PS").prop('required', false);
            $("#PO").prop('required', false);
            $("#Tehsil").prop('required', false);
            $("#Village").prop('required', false);
            $("#PinCode").prop('required', false);
            $("#PermanentAddress").prop('required', true);

            $('#DOB').addClass('d-none');
            $("#DOB_").removeClass('d-none');

            $('#DateOfCommissioning').addClass('d-none');
            $("#DOC_").removeClass('d-none');
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
function getApplyIcardDetails() {
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
            "ICNumber": $("#ServiceNumber").val(),
            "lCardType": $("#TypeId").val()
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
                $("#DOB_").val(DateFormateMMMM_dd_yyyy(response.Pers_birth_dt));
                $("#DateOfCommissioning").val(response.Pers_enrol_dt);
                $("#DOC_").val(DateFormateMMMM_dd_yyyy(response.Pers_enrol_dt));

                //let address;
                //if (response.Pers_House_no == null || response.Pers_House_no == '') {
                //    address = response.Pers_House_no;
                //}
                //if (response.Pers_Moh_st == null || response.Pers_Moh_st == '') {
                //    address = address + ' ' + response.Pers_Moh_st;
                //}
                //if (response.Pers_Village == null || response.Pers_Village == '') {
                //    address = address + ' ' + response.Pers_Village;
                //}
                $("#PermanentAddress").val('Village - ' + response.Pers_Address.Pers_Village + '\n Post Office-' + response.Pers_Address.Pers_Post_office + ' \n Tehsil- ' + response.Pers_Address.Pers_Tehsil + '\n District- ' + response.Pers_Address.Pers_District + '\n State- ' + response.Pers_Address.Pers_State + '\n Pin Code- ' + response.Pers_Address.Pers_Pin_code);
                //$("#RegId").val(regId);

                $("#State").val(response.Pers_Address.Pers_State);
                $("#District").val(response.Pers_Address.Pers_District);
                $("#PS").val(response.Pers_Address.Pers_Police_stn);
                $("#PO").val(response.Pers_Address.Pers_Post_office);
                $("#Tehsil").val(response.Pers_Address.Pers_Tehsil);
                $("#Village").val(response.Pers_Address.Pers_Village);
                if (response.Pers_Address.Pers_Pin_code == null || response.Pers_Address.Pers_Pin_code == '') {
                    $("#PinCode").val("000000");
                }
                else {
                    $("#PinCode").val(response.Pers_Address.Pers_Pin_code);
                }
                $("#IdenMark1").val('');
                $("#IdenMark2").val('');
                $("#AadhaarNo").val('');
                $("#BloodGroup").val('');

                //$("#IdenMark1").val(response.pers_Iden_mark_1);
                //$("#IdenMark2").val(response.pers_Iden_mark_2);
                //$("#AadhaarNo").val(response.pers_UID);
                //if (response.Pers_Height!="")
                //    $("#Height").val(response.Pers_Height);
                //else
                //    $("#Height").val(0);

                //$("#BloodGroup").val(response.pers_Blood_Gp);
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

    const encryptedRegistrationApplyFor = sessionStorage.getItem("RegistrationApplyFor");
    const secretKey = document.getElementById("spnUniqueSecretKey").innerText;
    const decryptedRegistrationApplyFor = CryptoJS.AES.decrypt(encryptedRegistrationApplyFor, secretKey).toString(CryptoJS.enc.Utf8);


    if (decryptedRegistrationApplyFor === "4" || decryptedRegistrationApplyFor === "9") {
        if ($("#State").val().length == 0) {
            $("#lblState").html('State is required.');
        }
        if ($("#PS").val().length == 0) {
            $("#lblPS").html('Police Station is required.');
        }
        if ($("#Village").val().length == 0) {
            $("#lblVillage").html('Village is required.');
        }
        if ($("#District").val().length == 0) {
            $("#lblDistrict").html('District is required.');
        }
        if ($("#PO").val().length == 0) {
            $("#lblPO").html('Post Office is required.');
        }
        if ($("#Tehsil").val().length == 0) {
            $("#lblTehsil").html('Tehsil is required.');
        }
    }
    else {
        $("#lblState").html('');
        $("#lblPS").html('');
        $("#lblVillage").html('');
        $("#lblDistrict").html('');
        $("#lblPO").html('');
        $("#lblTehsil").html('');
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