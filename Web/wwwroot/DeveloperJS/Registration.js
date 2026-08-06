var skey = "";
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    skey = $('#spnhdns').html();
    $(function () {
        let oldText = "";
        let oldMoment = null;
        const min = moment().subtract(75, 'year');
        const max = moment().subtract(18, 'year');

        if ($('#DOB_').data('DateTimePicker')) {
            $('#DOB_').data('DateTimePicker').destroy();
        }

        $('#DOB_').datetimepicker({
            format: 'DD/MM/YYYY',
            sideBySide: true,
            useCurrent: false,
            minDate: min,
            maxDate: max,
            showClear: false,
            showClose: false
        }).on('dp.show', function () {

            const picker = $(this).data('DateTimePicker');

            oldText = $(this).val();
            oldMoment = picker.date() ? picker.date().clone() : null;

            setTimeout(function () {
                const $widget = $('.bootstrap-datetimepicker-widget:visible').last();
                if (!$widget.length) return;

                // add buttons once
                if ($widget.find('.dtp-okcancel').length === 0) {
                    $widget.append(`
                <div class="dtp-okcancel">
                    <button type="button" class="btn btn-sm btn-secondary dtp-cancel">Cancel</button>
                    <button type="button" class="btn btn-sm btn-success ms-2 dtp-ok">OK</button>
                </div>
            `);

                    // OK
                    $widget.on('click', '.dtp-ok', function () {
                        picker.hide();
                    });

                    // Cancel
                    $widget.on('click', '.dtp-cancel', function () {
                        if (oldMoment)
                            picker.date(oldMoment);
                        else
                            picker.clear();
                        $('#DOB_').val(oldText);
                        picker.hide();
                    });
                }
            }, 0);
        }).on('dp.change', function (e) {

            var dob = e.date;

            if (!dob || !dob.isValid()) {
                return;
            }

            var docMin = dob.clone().add(18, 'year');
            var docMax = moment();

            var docPicker = $('#DOC_').data('DateTimePicker');

            if (docPicker) {
                docPicker.minDate(docMin);
                docPicker.maxDate(docMax);

                var selectedDoc = docPicker.date();

                if (selectedDoc && selectedDoc.isBefore(docMin, 'day')) {
                    docPicker.clear();
                    $('#DOC_').val('');
                }
            }
        });
        $('#DOB_').on('keydown', (e) => {
            e.preventDefault();
            return false;
        });

        if ($('#DOC_').data('DateTimePicker')) {
            $('#DOC_').data('DateTimePicker').destroy();
        }
        oldText = "";
        oldMoment = null;

        $('#DOC_').datetimepicker({
            format: 'DD/MM/YYYY',
            sideBySide: true,
            useCurrent: false,
            minDate: min,
            //maxDate: max,
            showClear: false,
            showClose: false
        }).on('dp.show', function () {

            const picker = $(this).data('DateTimePicker');

            oldText = $(this).val();
            oldMoment = picker.date() ? picker.date().clone() : null;

            setTimeout(function () {
                const $widget = $('.bootstrap-datetimepicker-widget:visible').last();
                if (!$widget.length) return;

                // add buttons once
                if ($widget.find('.dtp-okcancel').length === 0) {
                    $widget.append(`
                <div class="dtp-okcancel">
                    <button type="button" class="btn btn-sm btn-secondary dtp-cancel">Cancel</button>
                    <button type="button" class="btn btn-sm btn-success ms-2 dtp-ok">OK</button>
                </div>
            `);

                    // OK
                    $widget.on('click', '.dtp-ok', function () {
                        picker.hide();
                    });

                    // Cancel
                    $widget.on('click', '.dtp-cancel', function () {
                        if (oldMoment)
                            picker.date(oldMoment);
                        else
                            picker.clear();
                        $('#DOC_').val(oldText);
                        picker.hide();
                    });
                }
            }, 0);
        });
        $('#DOC_').on('keydown', (e) => {
            e.preventDefault();
            return false;
        });

    });

    // Attach click event to all radios with class submitTypeRadio
    $(document).on("change", ".submitTypeRadio", function () {
        const value = $(this).val();
        registrationEnableDisabledField(value);
    });

    //$('#DOB').on('change', function () {
    //    $("#DateOfCommissioning").val("");

    //    var dobValue = $(this).val(); 
    //    if (!dobValue) return;

    //    var parts = dobValue.split('-');

    //    var year = parseInt(parts[0]) + 18;
    //    var month = parts[1];
    //    var day = parts[2];

    //    var minDate = year + '-' + month + '-' + day;

    //    $('#DateOfCommissioning').attr('min', minDate);
    //});
    $('.paddress').on('change', function () {
        $("#PermanentAddress").val('Village - ' + $("#Village").val() + '\n Post Office-' + $("#PO").val() + ' \n Tehsil- ' + $("#Tehsil").val() + '\n District- ' + $("#District").val() + '\n State- ' + $("#State").val() + '\n Pin Code- ' + $("#PinCode").val());

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
        const encryptedOldArmyNo = sessionStorage.getItem("OldArmyNo");
        const encryptedOffType = sessionStorage.getItem("OffType");
        const encryptedRegistrationApplyFor = sessionStorage.getItem("RegistrationApplyFor");
        const encryptedlCardType = sessionStorage.getItem("lCardType");

        const decryptedArmyNo = decryptData(encryptedArmyNo, skey);
        const decryptedOldArmyNo = decryptData(encryptedOldArmyNo, skey);
        const decryptedOffType = decryptData(encryptedOffType, skey);
        const decryptedRegistrationApplyFor = decryptData(encryptedRegistrationApplyFor, skey);
        const decryptedlCardType = decryptData(encryptedlCardType, skey);

        $("#ServiceNumber").val(decryptedArmyNo);
        $("#OldServiceNo").val(decryptedOldArmyNo);

        if (decryptedlCardType == 4) {
            $('.OldServiceNo').removeClass("d-none");
        } else {
            $('.OldServiceNo').addClass("d-none");
        }




        $("#icarddetails").html(decryptedArmyNo);
        $("#ApplyForId").val(decryptedOffType);
        $("#RegistrationId").val(decryptedRegistrationApplyFor);
        $("#TypeId").val(decryptedlCardType);

        if (decryptedRegistrationApplyFor === "4" || decryptedRegistrationApplyFor === "9") {
            $('#FName').attr('readonly', false);
            $('#LName').attr('readonly', false);
            $('#NameAsPerRecord').attr('readonly', false);
            $('#DOB_').attr('readonly', false);
            $('#ServiceNo').attr('readonly', false);
            $('#DOC_').attr('readonly', false);
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

        } else {
            $('#FName').attr('readonly', true);
            $('#LName').attr('readonly', true);
            $('#NameAsPerRecord').attr('readonly', true);
            $('#DOB_').attr('readonly', true);
            $('#ServiceNo').attr('readonly', true);
            $('#DOC_').attr('readonly', true);
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

        }

        /* Getdatafromapi();*/

        getApplyIcardDetails();
    }
    $('.select2').select2({

        closeOnSelect: false
    });
    var someNumbers = [4];
    GetRemarks("ddlRemarks", 0, someNumbers);

    $('#ddlRemarks').select2({
        placeholder: "Please select",
        width: '100%',
        closeOnSelect: false
    });

    $('#ddlRemarks').on('change', function () {
        $("#RemarksIds").val($('#ddlRemarks').val());
    });
    $('#btnclear').on('click', function () {
        ResetField();
    });


});
function getApplyIcardDetails() {
    let userdata = {
        "ApplyForId": $("#ApplyForId").val(),
        "RegistrationId": $("#RegistrationId").val(),
        "TypeId": $("#TypeId").val()
    }
    $.ajax({
        url: "/Home/GetApplyCardDetails",
        type: "POST",
        data: { "request": encryptPayloadData(JSON.stringify(userdata)) },

        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response, status) {
            if (response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                    return;
                }

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
            "ICNumber": encryptPayloadData($("#ServiceNumber").val()),
            "lCardType": $("#TypeId").val()
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
            "ICNumber": encryptPayloadData($("#ServiceNumber").val()),
            "Type": $("#ApplyForId").val()
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
                const dobValue = response.Pers_birth_dt;

                if (dobValue) {
                    let dobMoment = moment(dobValue, "YYYY-MM-DD", true);

                    if (!dobMoment.isValid()) {
                        dobMoment = moment(dobValue);
                    }
                    if (dobMoment.isValid()) {
                        $("#DOB_").datetimepicker("date", dobMoment);
                        $("#DOB_").val(dobMoment.format("DD/MM/YYYY"));
                    }
                }

                const docValue = response.Pers_enrol_dt;

                if (docValue) {
                    let docMoment = moment(docValue, "YYYY-MM-DD", true);

                    if (!docMoment.isValid()) {
                        docMoment = moment(docValue);
                    }
                    if (docMoment.isValid()) {
                        $("#DOC_").datetimepicker("date", docMoment);
                        $("#DOC_").val(docMoment.format("DD/MM/YYYY"));
                    }
                }

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
            }
        }
    });
}
function registrationEnableDisabledField(val) {
    $("#ddlRemarks").val("");
    $("#RemarksIds").val("");
    if (val == 1) {
        $("#btnsubmit").prop('disabled', false);
        $(".Remarks").addClass("d-none");
        $("#btnsubmit").text("Process I-Card");
        $("#btnsubmit").removeClass("btn-danger");
        $("#btnsubmit").addClass("btn-success");
    }
    else {
        $("#btnsubmit").prop('disabled', false);
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

    let isDobValid = validateDateString("DOB_", "Date of Birth");
    let isDocValid = validateDateString("DOC_", "Date of Commissioning/ Enrollment");

    if (!isDobValid || !isDocValid) {
        return false;
    }

    if (!validateDOBAndDOC()) {
        return false;
    }

    const encryptedRegistrationApplyFor = sessionStorage.getItem("RegistrationApplyFor");
    const decryptedRegistrationApplyFor = decryptData(encryptedRegistrationApplyFor, skey);


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

                    var formData = {};
                    $("#Registration").serializeArray().forEach(function (item) {
                        formData[item.name] = item.value;
                    });

                    var jsonData = JSON.stringify(formData);

                    var encrypted = encryptPayloadData(jsonData);

                    $("#EncryptedData").val(encrypted);

                    $("#Registration")[0].submit(); // native submit
                    // $("#Registration").submit();
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

                    var formData = {};

                    $("#Registration").serializeArray().forEach(function (item) {
                        formData[item.name] = item.value;
                    });

                    var jsonData = JSON.stringify(formData);

                    var encrypted = encryptPayloadData(jsonData);

                    $("#EncryptedData").val(encrypted);

                    $("#Registration")[0].submit(); // native submit
                    // $("#Registration").submit();
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
function ResetField() {
    $('#FName').val("");
    $('#LName').val("");
    $("#NameAsPerRecord").val("");
    $("#ServiceNo").val("");
    $("#DOB_").val("");
    $("#DOC_").val("");
    $("#PermanentAddress").val("");
    $("#State").val("");
    $("#District").val("");
    $("#PS").val("");
    $("#PO").val("");
    $("#Tehsil").val("");
    $("#Village").val("");
    $("#PinCode").val("000000");
    $("#RankId").val("");
    $("#ArmedId").val("");
}
function setValidationMessage(fieldName, message) {
    $("span[data-valmsg-for='" + fieldName + "']").text(message);
}

function validateDateString(fieldName, displayName) {
    var value = ($("#" + fieldName).val() || "").trim();

    if (value === "") {
        setValidationMessage(fieldName, displayName + " is required.");
        return false;
    }

    // strict format only: 18/11/1982
    var pattern = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;

    if (!pattern.test(value)) {
        setValidationMessage(fieldName, displayName + " must be in DD/MM/YYYY format.");
        return false;
    }

    var m = moment(value, "DD/MM/YYYY", true);

    if (!m.isValid()) {
        setValidationMessage(fieldName, displayName + " is not a valid calendar date.");
        return false;
    }

    if (m.year() < 1900 || m.isAfter(moment(), "day")) {
        setValidationMessage(fieldName, displayName + " is not allowed.");
        return false;
    }

    setValidationMessage(fieldName, "");
    return true;
}
function validateDOBAndDOC() {
    var dob = moment($("#DOB_").val(), "DD/MM/YYYY", true);
    var doc = moment($("#DOC_").val(), "DD/MM/YYYY", true);

    if (!dob.isValid()) {
        $("span[data-valmsg-for='DOB_']").text("Date of Birth is required.");
        return false;
    }

    if (!doc.isValid()) {
        $("span[data-valmsg-for='DOC_']").text("Date of Commissioning/ Enrollment is required.");
        return false;
    }

    var minDocDate = dob.clone().add(18, "year");

    if (doc.isBefore(minDocDate, "day")) {
        $("span[data-valmsg-for='DOC_']")
            .text("Date of Commissioning/ Enrollment must be at least 18 years after Date of Birth.");
        return false;
    }

    $("span[data-valmsg-for='DOB_']").text("");
    $("span[data-valmsg-for='DOC_']").text("");

    return true;
}

/* =====================================================
   ECMS REGISTRATION PAGE UI HELPERS
   UI-only. Does not change registration validation, AJAX,
   encryption, form submit or session storage functionality.
===================================================== */
$(function () {
    document.body.classList.add("ecms-registration-body");

    // Keep Select2 controls aligned with the new card design.
    setTimeout(function () {
        try {
            $('.ecms-registration-page select').each(function () {
                if ($(this).data('select2')) {
                    $(this).select2({ width: '100%' });
                }
            });
        } catch (e) {
            console.warn("Registration UI Select2 resize skipped:", e);
        }
    }, 300);

    // Visual-only radio active state for clear selected feedback.
    function syncRegistrationRadioState() {
        $('.ecms-registration-page .submitTypeRadio').each(function () {
            $(this).closest('.col-sm-6').toggleClass('ecms-radio-selected', $('.ecms-registration-page .submitTypeRadio:checked').length > 0);
        });
    }

    $(document).on('change', '.ecms-registration-page .submitTypeRadio', syncRegistrationRadioState);
    syncRegistrationRadioState();
});


/* Safety class for page-scroll removal CSS */
$(function () {
    document.body.classList.add("ecms-registration-body");
});
