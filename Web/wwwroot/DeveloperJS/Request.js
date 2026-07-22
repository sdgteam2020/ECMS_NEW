var OffType = 0;
var RegistrationApplyFor = 0;
var lCardType = 0;
var IsValid = 0;
var Message = "";
var IsToken = true;
var IsWithTokenApply = true;
var skey = "";
var CurrentPrefixApplyForId = 0;

$(document).ready(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    skey = $('#spnhdns').html();

    $('#txtApplyForOldArmyNo, #txtApplyForArmyNo').on('input', function () {
        this.value = this.value.toUpperCase();
        toggleNextButton();
    });

    $('#ddlForOldArmyNoRulePrefix, #ddlForArmyNoRulePrefix').on('change', function () {
        toggleNextButton();
    });


    $("#btnApplicantsPostingout").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(ApplicantPostingOut);
    });

    $("#btnApplicantsClose").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(ApplicantClose);
    });

    $("#btnApplyCard").on("click", function () {
        RegistrationApplyFor = 0;

        $("#btnApplyCard").removeClass("btn-outline-primary").addClass("btn-primary");
        $(".cardmain").addClass("d-none");
        $("#btnarmytype").removeClass("d-none");

        resetArmyNoUi();
    });

    $("#btnaddOffrs").on("click", function () {
        $("#btnaddOffrs").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#btnJCOs").removeClass("btn-primary").addClass("btn-outline-primary");

        resetArmyNoUi();
        GetAllRegistrationApplyFor(1);
        getArmyPrefixRules(1);
    });

    $("#btnJCOs").on("click", function () {
        $("#btnJCOs").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#btnaddOffrs").removeClass("btn-primary").addClass("btn-outline-primary");

        resetArmyNoUi();
        GetAllRegistrationApplyFor(2);
        getArmyPrefixRules(2);
    });

    $("#btntokenrefresh").on("click", async function () {
        await GetTokenDetails("FetchUniqueTokenDetails", "txtApplyForArmyNo", "", "tokenmsg");
        applyArmyNoToControl($("#txtApplyForArmyNo").val(), "#ddlForArmyNoRulePrefix", "#txtApplyForArmyNo", false, false);
        $('#btnNext').removeClass("disabled").prop("disabled", false);
    });

    $("#btnNext").on("click", async function () {

        if ($("#ddlForArmyNoRulePrefix").val() == '') {
            toastr.error("Please select the Prefix.");
            return;
        }

        let fullArmyNo = getFullArmyNumber();

        if (fullArmyNo.length > 7 && fullArmyNo.length < 10) {

            if (parseInt(OffType) == 1) {
                if (
                    parseInt(RegistrationApplyFor) == 2 ||
                    parseInt(RegistrationApplyFor) == 3 ||
                    parseInt(RegistrationApplyFor) == 4 ||
                    parseInt(RegistrationApplyFor) == 10
                ) {
                    if (IsWithTokenApply == true) {
                        $("#txtApplyForArmyNo").val("");
                        await GetTokenDetails("FetchUniqueTokenDetails", "txtApplyForArmyNo", "", "tokenmsg", "ddlForArmyNoRulePrefix");
                    }
                } else {
                    if (IsToken == true && parseInt(RegistrationApplyFor) == 1) {
                        $("#txtApplyForArmyNo").val("");
                        await GetTokenDetails("FetchUniqueTokenDetails", "txtApplyForArmyNo", "", "tokenmsg", "ddlForArmyNoRulePrefix");
                    }
                }
            }

            if (lCardType == 4) {

                if ($("#ddlForOldArmyNoRulePrefix").val() == '') {
                    toastr.error("Please select the Old Prefix.");
                    return;
                }

                let OldServiceNo = getFullOldArmyNumber();
                let NewServiceNo = getFullArmyNumber();

                if (OldServiceNo.length > 7 && OldServiceNo.length < 10) {

                    if (OldServiceNo.toUpperCase() == NewServiceNo.toUpperCase()) {
                        toastr.error("Old Army No and New Army No not same.");
                        return;
                    }

                    let OldArmyNoFound = await CheckArmyNo(OldServiceNo);
                    let NewArmyNoFound = await CheckArmyNo(NewServiceNo);

                    let OldFirstTwo = await checkFirstTwoChars(OldServiceNo);
                    let NewFirstTwo = await checkFirstTwoChars(NewServiceNo);

                    let OldArmyNoSfx = await ChkSfx(OldServiceNo);
                    let NewArmyNoSfx = await ChkSfx(NewServiceNo);

                    if (OldArmyNoSfx == false) {
                        toastr.error("Invalid Old Army No.");
                    } else if (NewArmyNoSfx == false) {
                        toastr.error("Invalid New Army No.");
                    } else if (OldArmyNoFound == false) {
                        toastr.error("Old Army No not found.");
                    } else if (NewArmyNoFound == true) {
                        toastr.error("New Army No is already used.");
                    } else if (OldFirstTwo === '') {

                        if (NewFirstTwo === '') {
                            toastr.error("Both Old and New Army No is OR rank.");
                        } else if (
                            OffType == 2 &&
                            (NewFirstTwo === 'IC' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'SS' || NewFirstTwo === 'TA')
                        ) {
                            toastr.error("Please Select Offrs tab.");
                        } else if (OffType == 1 && NewFirstTwo === 'JC') {
                            toastr.error("Please Select JCOs/OR tab.");
                        } else {
                            CheckArmyNOExist();
                        }

                    } else if (OldFirstTwo !== '') {

                        if (OldFirstTwo === 'IC' && NewFirstTwo === '') {
                            toastr.error("Permanent Commissioned Officers are not downgraded.");
                        } else if (OldFirstTwo === 'IC' && NewFirstTwo === 'IC') {
                            toastr.error("Both Old and New Army No is permanent commissioned officers.");
                        } else if (
                            OldFirstTwo === 'IC' &&
                            (NewFirstTwo === 'SS' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'TA' || NewFirstTwo === 'JC')
                        ) {
                            toastr.error("Permanent Commissioned Officers are not downgraded.");
                        } else if (
                            (OldFirstTwo === 'SL' || OldFirstTwo === 'TA') &&
                            (NewFirstTwo === 'IC' || NewFirstTwo === 'SS' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'TA' || NewFirstTwo === 'JC')
                        ) {
                            toastr.error("SL / TA are not changed Army No.");
                        } else if (
                            (OldFirstTwo === 'SS' || OldFirstTwo === 'WC') &&
                            OffType == 2 &&
                            NewFirstTwo !== '' &&
                            NewFirstTwo === 'IC'
                        ) {
                            toastr.error("Please Select Offrs tab.");
                        } else if (
                            OldFirstTwo === 'JC' &&
                            OffType == 2 &&
                            NewFirstTwo !== '' &&
                            (NewFirstTwo === 'SS' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'TA')
                        ) {
                            toastr.error("Please Select Offrs tab.");
                        } else {
                            CheckArmyNOExist();
                        }
                    }

                } else {
                    toastr.error("Minimum eight and Maximum nine length of Old Army No.");
                }

            } else {

                if (parseInt(OffType) != 0 && parseInt(RegistrationApplyFor) != 0 && parseInt(lCardType) != 0) {

                    let ArmyNoSfx = getFullArmyNumber();
                    let result;

                    if (OffType == 1 && parseInt(RegistrationApplyFor) == 1) {
                        if (getFullArmyNumber() === $("#aspntokenarmyno").html().trim()) {
                            result = await ChkSfx(ArmyNoSfx);
                            IsValid = result == false ? 0 : 1;

                            if (result == false) {
                                toastr.error("Invalid Army No.");
                            }
                        } else {
                            Message = "Please Insert Valid Token. Token ArmyNo And Login ArmyNo Not Match";
                            IsValid = 0;
                        }
                    }

                    if (OffType == 1 && parseInt(RegistrationApplyFor) == 2) {
                        if (getFullArmyNumber() != "") {
                            result = await ChkSfx(ArmyNoSfx);
                            IsValid = result == false ? 0 : 1;

                            if (result == false) {
                                toastr.error("Invalid Army No.");
                            }
                        } else {
                            Message = "Please Insert Token";
                            IsValid = 0;
                        }
                    } else if (OffType == 1 && parseInt(RegistrationApplyFor) != 1) {
                        if (getFullArmyNumber() != "") {
                            result = await ChkSfx(ArmyNoSfx);
                            IsValid = result == false ? 0 : 1;

                            if (result == false) {
                                toastr.error("Invalid Army No.");
                            }
                        } else {
                            Message = "Please Enter Army No";
                            IsValid = 0;
                        }
                    } else if (OffType == 2) {
                        if (getFullArmyNumber() == "") {
                            IsValid = 0;
                            Message = "Please Enter Army No";
                        } else {
                            result = await ChkSfx(ArmyNoSfx);
                            IsValid = result == false ? 0 : 1;

                            if (result == false) {
                                toastr.error("Invalid Army No.");
                            }
                        }
                    }

                    if (IsValid == 1) {
                        CheckArmyNOExist();
                    } else if (Message != "") {
                        toastr.error(Message);
                    }

                } else {
                    toastr.error("Invalid Selected");
                }
            }

        } else {
            toastr.error("Minimum eight and Maximum nine length of Army No.");
        }
    });
});

function GetAllRegistrationApplyFor(Id) {
    $("#spnNext").addClass("d-none");
    RegistrationApplyFor = 0;

    var listItem = "";
    var userdata = {
        "ApplyForId": Id
    };

    $.ajax({
        url: '/Home/GetRegistrationApplyfor',
        data: { "request": encryptPayloadData(JSON.stringify(userdata)) },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({ text: errormsg });
                } else if (response == 0) {
                    $("#btnIcardFor").html("");
                    $("#icardrequestfor").html("");
                } else {
                    OffType = Id;

                    listItem += '<div class="seven"><h1>I-Card Appl Initiated for </h1></div>';

                    for (var i = 0; i < response.length; i++) {
                        listItem += '<button type="button" class="btn btn-outline-primary mr-2 applyforoffs btn1" id="icardFor' + response[i].RegistrationId + '">';
                        listItem += response[i].Name;
                        listItem += '<span class="spnRegistration d-none">' + response[i].RegistrationId + '</span>';
                        listItem += '</button>';
                    }

                    $("#btnIcardFor").html(listItem);
                    $("#icardrequestfor").html("");

                    $('.applyforoffs').on("click", function () {
                        $("#spnNext").addClass("d-none");
                        clearArmyNoFields();
                        hideArmyNoSection();
                        hideOldArmyNo();
                        setNewArmyNoPlaceholder("Enter Army No");

                        $('.applyforoffs').removeClass("btn-primary").addClass("btn-outline-primary");
                        $(this).removeClass("btn-outline-primary").addClass("btn-primary");

                        RegistrationApplyFor = $(this).closest("button").find(".spnRegistration").html();
                        AddAllCardType();
                    });
                }
            } else {
                $("#btnIcardFor").html("");
                $("#icardrequestfor").html("");
            }
        },
        error: function () {
            Swal.fire({ text: errormsg002 });
        }
    });
}
function AddAllCardType() {
    lCardType = 0;

    var list = '';
    list += '<div class="seven mt-4"><h1>Reason For Applying</h1></div>';
    list += '<button type="button" class="btn btn-outline-primary ml-2 applyforicard btn1">First time Smart card <span class="spnApplyForcard d-none">1</span></button>';
    list += '<button type="button" class="btn btn-outline-primary ml-2 applyforicard btn1">Fair wear and tear / Damaged <span class="spnApplyForcard d-none">2</span></button>';
    list += '<button type="button" class="btn btn-outline-primary ml-2 applyforicard btn1">Change of Rank <span class="spnApplyForcard d-none">3</span></button>';
    list += '<button type="button" class="btn btn-outline-primary ml-2 applyforicard btn1">Change of Army No <span class="spnApplyForcard d-none">4</span></button>';
    list += '<button type="button" class="btn btn-outline-primary ml-2 applyforicard btn1">Lost Card <span class="spnApplyForcard d-none">5</span></button>';

    $("#icardrequestfor").html(list);

    $('.applyforicard').on("click", function () {
        $('.applyforicard').removeClass("btn-primary").addClass("btn-outline-primary");
        $(this).removeClass("btn-outline-primary").addClass("btn-primary");

        $("#spnNext").removeClass("d-none");

        lCardType = parseInt($(this).closest("button").find(".spnApplyForcard").html());

        showArmyNoSection();
        showNewArmyNo();
        clearArmyNoFields();

        if (lCardType == 4) {
            setNewArmyNoPlaceholder("Enter New Army No");
            showOldArmyNo();
        } else {
            setNewArmyNoPlaceholder("Enter Army No");
            hideOldArmyNo();
        }

        if (OffType == 1) {
            GetByArmyNoIsToken($("#aspntokenarmyno").html());
        } else if (OffType == 2) {
            showArmyNoSection();
            showNewArmyNo();
            $("#btntokenrefresh").addClass("d-none");
            $('#txtApplyForArmyNo').attr('readonly', false);

            if (lCardType == 4) {
                showOldArmyNo();
            } else {
                hideOldArmyNo();
            }
        }

        toggleNextButton();
    });
}
function GetByArmyNoIsToken(ArmyNo) {
    let ArmyNoNew = encryptPayloadData(ArmyNo);

    var userdata = {
        "ArmyNo": ArmyNoNew
    };

    $.ajax({
        url: '/UserProfile/GetByArmyNoIsWithoutTokenApply',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({ text: errormsg });
                } else if (response == 4) {
                    Swal.fire({ text: "Invalid Army No." });
                } else {
                    IsWithTokenApply = response.IsWithTokenApply;
                    IsToken = response.IsToken;

                    if (parseInt(OffType) == 1) {
                        if ((parseInt(RegistrationApplyFor) == 2 || parseInt(RegistrationApplyFor) == 3 || parseInt(RegistrationApplyFor) == 4 || parseInt(RegistrationApplyFor) == 10)) {
                            if (IsWithTokenApply == true && lCardType != 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                showArmyNoSection();
                                hideNewArmyNo();
                                hideOldArmyNo();
                                $('#btnNext').addClass("disabled");
                            } else if (IsWithTokenApply == true && lCardType == 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                showArmyNoSection();
                                hideNewArmyNo();
                                showOldArmyNo();
                                $('#btnNext').addClass("disabled");
                            } else {
                                $("#btntokenrefresh").addClass("d-none");
                                showArmyNoSection();
                                showNewArmyNo();
                                clearNewArmyNoFields();
                                $('#txtApplyForArmyNo').attr('readonly', false);

                                if (lCardType != 4) {
                                    hideOldArmyNo();
                                } else {
                                    showOldArmyNo();
                                }

                                $('#btnNext').removeClass("disabled");
                            }
                        } else {
                            if (IsToken == true && parseInt(RegistrationApplyFor) == 1 && lCardType != 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                showArmyNoSection();
                                hideNewArmyNo();
                                hideOldArmyNo();
                                $('#btnNext').addClass("disabled");
                            } else if (IsToken == true && parseInt(RegistrationApplyFor) == 1 && lCardType == 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                showArmyNoSection();
                                hideNewArmyNo();
                                showOldArmyNo();
                                $('#btnNext').addClass("disabled");
                            } else {
                                $("#btntokenrefresh").addClass("d-none");
                                showArmyNoSection();
                                showNewArmyNo();

                                var tokenArmyNo = $("#aspntokenarmyno").html();

                                getArmyPrefixRules(OffType, function () {
                                    if (lCardType == 4) {
                                        showOldArmyNo();
                                        applyArmyNoToControl(tokenArmyNo, "#ddlForOldArmyNoRulePrefix", "#txtApplyForOldArmyNo", true, true);

                                        $("#ddlForArmyNoRulePrefix")
                                            .val("")
                                            .prop("disabled", false);

                                        $("#txtApplyForArmyNo")
                                            .val("")
                                            .prop("readonly", false);
                                    }
                                    else {
                                        hideOldArmyNo();
                                        applyArmyNoToControl(tokenArmyNo, "#ddlForArmyNoRulePrefix", "#txtApplyForArmyNo", true, true);
                                    }
                                    $('#btnNext').removeClass("disabled");
                                    toggleNextButton();
                                });
                            }
                        }
                    } else {
                        $("#btntokenrefresh").addClass("d-none");
                        showArmyNoSection();
                        showNewArmyNo();
                        $('#txtApplyForArmyNo').attr('readonly', false);

                        if (lCardType != 4) {
                            hideOldArmyNo();
                        } else {
                            showOldArmyNo();
                        }
                    }
                }
            }
        },
        error: function () {
            Swal.fire({ text: errormsg002 });
        }
    });
}
function CheckArmyNOExist() {
    let OldServiceNo = getFullOldArmyNumber();
    let NewServiceNo = getFullArmyNumber();
    $.ajax({
        url: "/BasicDetail/GetData",
        type: "POST",
        data: {
            "ICNumber": lCardType == 4 ? encryptPayloadData(OldServiceNo) : encryptPayloadData(NewServiceNo),
            "lCardType": lCardType
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response.Status == false) {
                toastr.error(response.Message);
            } else {
                Swal.fire({
                    title: "Are you sure?",
                    text: "You want to submit!",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Yes, submit it!"
                }).then((result) => {
                    if (result.isConfirmed) {
                        var encryptedArmyNo = encryptData(NewServiceNo, skey);
                        var encryptedOldArmyNo = encryptData(OldServiceNo, skey);
                        var encryptedOffType = encryptData(OffType.toString(), skey);
                        var encryptedRegistrationApplyFor = encryptData(RegistrationApplyFor.toString(), skey);
                        var encryptedlCardType = encryptData(lCardType.toString(), skey);

                        sessionStorage.setItem("OffType", encryptedOffType);
                        sessionStorage.setItem("RegistrationApplyFor", encryptedRegistrationApplyFor);
                        sessionStorage.setItem("lCardType", encryptedlCardType);
                        sessionStorage.setItem("ArmyNo", encryptedArmyNo);
                        sessionStorage.setItem("OldArmyNo", encryptedOldArmyNo);

                        window.location.href = "/BasicDetail/Registration";
                    }
                });
            }
        }
    });
}

async function CheckArmyNo(ArmyNo) {
    let param = new URLSearchParams({ ArmyNo: ArmyNo });

    try {
        const response = await fetch('/BasicDetail/CheckArmyNO', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (result != null) {
            return result;
        } else {
            toastr.error('Invalid Input.');
            window.location.href = '/BasicDetail/Request';
        }
    } catch (error) {
        alert("Error: " + error.message);
    }
}

async function checkFirstTwoChars(input) {
    await new Promise(resolve => resolve());

    if (input.length >= 2 && /^[a-zA-Z]{2}/.test(input)) {
        return input.substring(0, 2).toUpperCase();
    }

    return '';
}

async function ChkSfx(ServiceNo) {
    await new Promise(resolve => resolve());

    let ArmyNo = ServiceNo;
    var armyno = ArmyNo.replace(/[A-Za-z]/g, '');

    var txt = ArmyNo.slice(-1);
    const lastChar = ArmyNo.slice(-1);

    const isAlpha = /^[A-Za-z]$/.test(lastChar);

    if (txt == "" || isAlpha == false) {
        return false;
    }

    var vlength = armyno.length;
    var NumMulti = parseInt(vlength) + 1;
    var vMulti = 0;
    var vSum = 0;
    var Sfx;

    for (var i = 0; i < vlength; i++) {
        vMulti = parseInt(armyno.charAt(i)) * parseInt(NumMulti);
        vSum = parseInt(vSum) + parseInt(vMulti);
        NumMulti = parseInt(NumMulti) - 1;
    }

    var Reminder = parseInt(vSum) % 11;

    switch (Reminder) {
        case 0: Sfx = "A"; break;
        case 1: Sfx = "F"; break;
        case 2: Sfx = "H"; break;
        case 3: Sfx = "K"; break;
        case 4: Sfx = "L"; break;
        case 5: Sfx = "M"; break;
        case 6: Sfx = "N"; break;
        case 7: Sfx = "P"; break;
        case 8: Sfx = "W"; break;
        case 9: Sfx = "X"; break;
        case 10: Sfx = "Y"; break;
    }

    return Sfx === lastChar;
}

// Function to get Army Prefix rules
function getArmyPrefixRules(ApplyForId, onSuccess) {
    var requestData = {
        ApplyForId: ApplyForId
    };

    $.ajax({
        url: '/Home/GetArmyPrefixRules',
        method: 'POST',
        data: { "request": encryptPayloadData(JSON.stringify(requestData)) },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response && Array.isArray(response)) {
                CurrentPrefixApplyForId = ApplyForId;

                var selectedNew = $("#ddlForArmyNoRulePrefix").val();
                var selectedOld = $("#ddlForOldArmyNoRulePrefix").val();

                $('#ddlForArmyNoRulePrefix').empty();
                $('#ddlForOldArmyNoRulePrefix').empty();
                $("#ddlForArmyNoRulePrefix").prop("disabled", false);
                $('#ddlForOldArmyNoRulePrefix').prop("disabled", false);

                $('#ddlForArmyNoRulePrefix').append('<option value="">Select Prefix</option>');
                $('#ddlForOldArmyNoRulePrefix').append('<option value="">Select Prefix</option>');

                response.forEach(function (item) {
                    var optionText = item.Prefix === 'NO' ? 'NO (OR)' : item.Prefix;
                    $('#ddlForArmyNoRulePrefix').append('<option value="' + item.Prefix + '">' + optionText + '</option>');
                    $('#ddlForOldArmyNoRulePrefix').append('<option value="' + item.Prefix + '">' + optionText + '</option>');
                });

                if (selectedNew && $('#ddlForArmyNoRulePrefix option[value="' + selectedNew + '"]').length > 0) {
                    $('#ddlForArmyNoRulePrefix').val(selectedNew);
                }

                if (selectedOld && $('#ddlForOldArmyNoRulePrefix option[value="' + selectedOld + '"]').length > 0) {
                    $('#ddlForOldArmyNoRulePrefix').val(selectedOld);
                }

                if (typeof onSuccess === 'function') {
                    onSuccess(response);
                }
            } else {
                console.error("Error: Invalid data received.");
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error: " + error);
            console.error("Response: " + xhr.responseText);
        }
    });
}
/* =========================
   UI Helper Functions
========================= */

function showArmyNoSection() {
    $("#armyNoSection").removeClass("d-none");
}

function hideArmyNoSection() {
    $("#armyNoSection").addClass("d-none");
    clearArmyNoFields();
}

function showOldArmyNo() {
    $("#oldArmyNoWrapper").removeClass("d-none");
    $("#txtApplyForOldArmyNo").attr("data-val-required", "Old Army No is required.");
}

function hideOldArmyNo() {
    $("#oldArmyNoWrapper").addClass("d-none");
    $("#txtApplyForOldArmyNo").val("").removeAttr("data-val-required");
    $("#ddlForOldArmyNoRulePrefix").val("").prop("disabled", false);
    $("#txtApplyForOldArmyNo").prop("readonly", false);
}

function showNewArmyNo() {
    $("#newArmyNoWrapper").removeClass("d-none");
}

function hideNewArmyNo() {
    $("#newArmyNoWrapper").addClass("d-none");
    $("#txtApplyForArmyNo").val("");
}

function clearArmyNoFields() {
    clearNewArmyNoFields();
    $("#txtApplyForOldArmyNo").val("").prop("readonly", false);
    $("#ddlForOldArmyNoRulePrefix").val("").prop("disabled", false);
}
function clearNewArmyNoFields() {
    $("#txtApplyForArmyNo").val("").prop("readonly", false);
    $("#ddlForArmyNoRulePrefix").val("").prop("disabled", false);
}
function resetArmyNoUi() {
    hideArmyNoSection();
    hideOldArmyNo();
    showNewArmyNo();
    $("#btntokenrefresh").addClass("d-none");
    $("#btnNext").addClass("disabled");
    $("#txtApplyForArmyNo").attr("readonly", false);
}

function setNewArmyNoPlaceholder(text) {
    $("#txtApplyForArmyNo").attr("placeholder", text);
}

function toggleNextButton() {
    var newPrefix = $("#ddlForArmyNoRulePrefix").val();
    var newArmyNo = $("#txtApplyForArmyNo").val().trim();

    var oldPrefix = $("#ddlForOldArmyNoRulePrefix").val();
    var oldArmyNo = $("#txtApplyForOldArmyNo").val().trim();

    var isChangeArmyNo = parseInt(lCardType) === 4;

    var validNew = newPrefix !== "" && newArmyNo.length > 0;
    var validOld = oldPrefix !== "" && oldArmyNo.length > 0;

    if (isChangeArmyNo) {
        if (validNew && validOld) {
            $("#btnNext").removeClass("disabled").prop("disabled", false);
        } else {
            $("#btnNext").addClass("disabled").prop("disabled", true);
        }
    } else {
        if (validNew) {
            $("#btnNext").removeClass("disabled").prop("disabled", false);
        } else {
            $("#btnNext").addClass("disabled").prop("disabled", true);
        }
    }
}


function getFullArmyNumber() {
    if ($("#ddlForArmyNoRulePrefix").val() != 'No(OR)')
        return $("#ddlForArmyNoRulePrefix").val() + $("#txtApplyForArmyNo").val();
    else
        return $("#txtApplyForArmyNo").val();
}

function getFullOldArmyNumber() {
    if ($("#ddlForOldArmyNoRulePrefix").val() != 'No(OR)')
        return $("#ddlForOldArmyNoRulePrefix").val() + $("#txtApplyForOldArmyNo").val();
    else
        return $("#txtApplyForOldArmyNo").val();
}

function getArmyPrefix(armyNo) {
    if (!armyNo || armyNo.length < 2) {
        return "";
    }

    let withoutSuffix = armyNo.slice(0, -1);
    let match = withoutSuffix.match(/^[A-Za-z]+/);

    return match ? match[0].toUpperCase() : "";
}

function applyArmyNoToControl(fullArmyNo, ddlSelector, txtSelector, disablePrefix, readonlyTxt) {
    fullArmyNo = (fullArmyNo || "").toUpperCase().trim();

    var prefix = getArmyPrefix(fullArmyNo);
    var body = fullArmyNo;

    if (prefix !== "") {
        body = fullArmyNo.substring(prefix.length);
        if ($(ddlSelector + ' option[value="' + prefix + '"]').length > 0) {
            $(ddlSelector).val(prefix);
        } else {
            $(ddlSelector).val("");
        }
    } else {
        if ($(ddlSelector + ' option[value="NO"]').length > 0) {
            $(ddlSelector).val("NO");
        } else {
            $(ddlSelector).val("");
        }
        body = fullArmyNo;
    }

    $(txtSelector).val(body).prop("readonly", !!readonlyTxt);
    $(ddlSelector).prop("disabled", !!disablePrefix);

    toggleNextButton();
}