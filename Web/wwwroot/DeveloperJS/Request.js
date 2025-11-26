var OffType = 0;
var RegistrationApplyFor = 0;
var lCardType = 0;
var IsValid = 0;
var Message = "";
var IsToken = true;
var IsWithTokenApply = true;
$(document).ready(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

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
    $('#txtApplyForArmyNo').on("change", function (e) {
        if ($('#txtApplyForArmyNo').val().length > 0) {
            $('#btnNext').removeClass("disabled");
        } else {
            $('#btnNext').addClass("disabled");
        }
    });
    $('#txtApplyForArmyNo').keypress(function (e) {
        if ($('#txtApplyForArmyNo').val().length > 0) {
            $('#btnNext').removeClass("disabled");
        } else {
            $('#btnNext').addClass("disabled");
        }
    });

    $("#btnApplyCard").on("click", function () {

        RegistrationApplyFor = 0;

        $("#btnApplyCard").removeClass("btn-outline-primary");
        $("#btnApplyCard").addClass("btn-primary");
        $(".cardmain").addClass("d-none");
        //var list = '';
        //list += '';
        $("#btnarmytype").removeClass("d-none");
    });
    $("#btnaddOffrs").on("click", function () {
        $("#btnaddOffrs").removeClass("btn-outline-primary");
        $("#btnaddOffrs").addClass("btn-primary");

        $("#btnJCOs").addClass("btn-outline-primary");
        $("#btnJCOs").removeClass("btn-primary");

        $("#txtApplyForArmyNo").addClass("d-none");
        $("#txtApplyForArmyNo").val("");

        $("#txtApplyForOldArmyNo").addClass("d-none");
        $("#txtApplyForOldArmyNo").val("");


        GetAllRegistrationApplyFor(1);

    });
    $("#btnJCOs").on("click", function () {

        $("#btnaddOffrs").removeClass("btn-primary");
        $("#btnaddOffrs").addClass("btn-outline-primary");

        $("#btnJCOs").addClass("btn-primary");
        $("#btnJCOs").removeClass("btn-outline-primary");
        $("#btnJCOs").addClass("btn-primary");

        $("#txtApplyForArmyNo").addClass("d-none");
        $("#txtApplyForArmyNo").val("");

        $("#txtApplyForOldArmyNo").addClass("d-none");
        $("#txtApplyForOldArmyNo").val("");

        GetAllRegistrationApplyFor(2);
    });
    $("#btntokenrefresh").on("click", async function () {
        await GetTokenDetails("FetchUniqueTokenDetails", "txtApplyForArmyNo", "", "tokenmsg");
        $('#btnNext').removeClass("disabled");
    });
    $("#btnNext").on("click", async function () {

        if ($("#txtApplyForArmyNo").val().length > 7 && $("#txtApplyForArmyNo").val().length < 10) {
            if (parseInt(OffType) == 1) {
                if ((parseInt(RegistrationApplyFor) == 2 || parseInt(RegistrationApplyFor) == 3 || parseInt(RegistrationApplyFor) == 4 || parseInt(RegistrationApplyFor) == 10)) {
                    if (IsWithTokenApply == true) {
                        $("#txtApplyForArmyNo").val("");
                        await GetTokenDetails("FetchUniqueTokenDetails", "txtApplyForArmyNo", "", "tokenmsg");
                    }
                }
                else {
                    if (IsToken == true && parseInt(RegistrationApplyFor) == 1) {
                        $("#txtApplyForArmyNo").val("");
                        await GetTokenDetails("FetchUniqueTokenDetails", "txtApplyForArmyNo", "", "tokenmsg");
                    }
                }
            }

            if (lCardType == 4) {
                if ($("#txtApplyForOldArmyNo").val().length > 7 && $("#txtApplyForOldArmyNo").val().length < 10) {
                    if ($("#txtApplyForOldArmyNo").val().toUpperCase() == $("#txtApplyForArmyNo").val().toUpperCase()) {
                        toastr.error("Old Army No and New Army No not same.");
                    }
                    else {
                        let OldServiceNo = $("#txtApplyForOldArmyNo").val();
                        let NewServiceNo = $("#txtApplyForArmyNo").val();

                        let OldArmyNoFound = await CheckArmyNo(OldServiceNo);
                        let NewArmyNoFound = await CheckArmyNo(NewServiceNo);

                        let OldFirstTwo = await checkFirstTwoChars(OldServiceNo);
                        let NewFirstTwo = await checkFirstTwoChars(NewServiceNo);

                        let OldArmyNoSfx = await ChkSfx(OldServiceNo);
                        let NewArmyNoSfx = await ChkSfx(NewServiceNo);

                        if (OldArmyNoSfx == false) {
                            toastr.error("Invalid Old Army No.");
                        }
                        else if (NewArmyNoSfx == false) {
                            toastr.error("Invalid New Army No.");
                        }
                        else if (OldArmyNoFound == false) {
                            toastr.error("Old Army No not found.");
                        }
                        else if (NewArmyNoFound == true) {
                            toastr.error("New Army No is alredy used.");
                        }
                        else if (OldFirstTwo === '') {
                            if (NewFirstTwo === '') {
                                toastr.error("Both Old and New Army No is OR rank.");
                            }
                            else if (OffType == 2 && (NewFirstTwo === 'IC' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'SS' || NewFirstTwo === 'TA')) {
                                toastr.error("Please Select Offrs tab.");
                            }
                            else if (OffType == 1 && NewFirstTwo === 'JC') {
                                toastr.error("Please Select JCOs/OR tab.");
                            }
                            else {
                                CheckArmyNOExist();
                            }
                        }
                        else if (OldFirstTwo !== '') {
                            if (OldFirstTwo === 'IC' && NewFirstTwo === '') {
                                toastr.error("Permanent Commissioned Officers are not downgraded.");
                            }
                            else if (OldFirstTwo === 'IC' && NewFirstTwo === 'IC') {
                                toastr.error("Both Old and New Army No is permanent commissioned officers.");
                            }
                            else if (OldFirstTwo === 'IC' && (NewFirstTwo === 'SS' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'TA' || NewFirstTwo === 'JC')) {
                                toastr.error("Permanent Commissioned Officers are not downgraded.");
                            }
                            else if ((OldFirstTwo === 'SL' || OldFirstTwo === 'TA') && (NewFirstTwo === 'IC' || NewFirstTwo === 'SS' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'TA' || NewFirstTwo === 'JC')) {
                                toastr.error("SL / TA are not changed Army No.");
                            }
                            else if ((OldFirstTwo === 'SS' || OldFirstTwo === 'WC') && OffType == 2 && NewFirstTwo !== '' && NewFirstTwo === 'IC') {
                                toastr.error("Please Select Offrs tab.");
                            }
                            else if (OldFirstTwo === 'JC' && OffType == 2 && NewFirstTwo !== '' && (NewFirstTwo === 'SS' || NewFirstTwo === 'SL' || NewFirstTwo === 'WC' || NewFirstTwo === 'TA')) {
                                toastr.error("Please Select  Offrs tab.");
                            }
                            else {
                                CheckArmyNOExist();
                            }
                        }
                    }
                }
                else {
                    toastr.error("Minimum eight and Maximum nine length of Old Army No.");
                }
            }
            else {
                if (parseInt(OffType) != 0 && parseInt(RegistrationApplyFor) != 0 && parseInt(lCardType) != 0) {
                    let ArmyNoSfx = $("#txtApplyForArmyNo").val();
                    let result;
                    if (OffType == 1 && parseInt(RegistrationApplyFor) == 1) {
                        if ($("#txtApplyForArmyNo").val() === $("#aspntokenarmyno").html()) {
                            result = await ChkSfx(ArmyNoSfx);
                            if (result == false) {
                                IsValid = 0;
                                toastr.error("Invalid Army No.");
                            }
                            else {
                                IsValid = 1;
                            }
                        } else {
                            Message = "Please Inset Valid Token Token ArmyNo And Login ArmyNo Not Match";
                            IsValid = 0;
                        }
                    }
                    if (OffType == 1 && parseInt(RegistrationApplyFor) == 2) {
                        if ($("#txtApplyForArmyNo").val() != "") {
                            result = await ChkSfx(ArmyNoSfx);
                            if (result == false) {
                                IsValid = 0;
                                toastr.error("Invalid Army No.");
                            }
                            else {
                                IsValid = 1;
                            }
                        }
                        else {
                            Message = "Please Inset Token";
                            IsValid = 0;
                        }
                    }
                    else if (OffType == 1 && parseInt(RegistrationApplyFor) != 1) {
                        if ($("#txtApplyForArmyNo").val() != "") {
                            result = await ChkSfx(ArmyNoSfx);
                            if (result == false) {
                                IsValid = 0;
                                toastr.error("Invalid Old Army No.");
                            }
                            else {
                                IsValid = 1;
                            }
                        }
                        else {
                            Message = "Please Enter Army No";
                            IsValid = 0;
                        }
                    }
                    else if (OffType == 2) {
                        if ($("#txtApplyForArmyNo").val() == "") {
                            IsValid = 0;
                            Message = "Please Enter Army No";
                        }
                        else {
                            result = await ChkSfx(ArmyNoSfx);
                            if (result == false) {
                                IsValid = 0;
                                toastr.error("Invalid Old Army No.");
                            }
                            else {
                                IsValid = 1;
                            }
                        }
                    }

                    if (IsValid == 1) {

                        CheckArmyNOExist();
                    }
                    else {
                        //toastr.error(Message);
                    }
                }
                else {
                    toastr.error("Invalid Selected");
                }
            }

        }
        else {
            toastr.error("Minimum eight and Maximum nine length of Army No.");
        }
    });
});
function GetAllRegistrationApplyFor(Id) {
    $("#spnNext").addClass("d-none");
    //$("#txtApplyForArmyNo").addClass("d-none");
    RegistrationApplyFor = 0;
    var listItem = "";
    var userdata =
    {
        "ApplyForId": Id,

    };
    $.ajax({
        url: '/Home/GetRegistrationApplyfor',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            let inputField = document.getElementById("txtApplyForArmyNo");

            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    $("#btnIcardFor").html("");
                    $("#icardrequestfor").html("");
                }

                else {

                    OffType = Id;
                    listItem += '<div class="seven"><h1>I-Card Appl Initiated for </h1>';
                    for (var i = 0; i < response.length; i++) {

                        listItem += '</div><button type="button" class="btn btn-outline-primary mt-4 mr-2 applyforoffs btn1" id="icardFor' + response[i].RegistrationId + '">' + response[i].Name + '<span class="spnRegistration d-none">' + response[i].RegistrationId + '</span></button>';


                    }

                    $("#btnIcardFor").html(listItem);
                    $("#icardrequestfor").html("");

                    $('.applyforoffs').on("click", function () {

                        $("#txtApplyForOldArmyNo").addClass("d-none");
                        $("#txtApplyForOldArmyNo").val("");

                        inputField.placeholder = "Enter Army No With pfx & sfx Ex.ICXXXXXP,JC391516M,15698645A";

                        $('.applyforoffs').removeClass("btn-primary");
                        $('.applyforoffs').addClass("btn-outline-primary");


                        $(this).removeClass("btn-outline-primary");
                        $(this).addClass("btn-primary");

                        RegistrationApplyFor = $(this).closest("button").find(".spnRegistration").html();
                        // alert($(this).closest("button").find(".spnRegistration").html());
                        AddAllCardType();
                    });

                }
            }
            else {
                $("#btnIcardFor").html("");
                $("#icardrequestfor").html("");
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });

}
function AddAllCardType() {
    let inputField = document.getElementById("txtApplyForArmyNo");
    lCardType = 0;
    var list = '';
    list += '<div class="seven mt-4" ><h1>Reason For Applying</h1>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">First time Smart card <span class="spnApplyForcard d-none">1</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Fair wear and tear / Damaged<span class="spnApplyForcard d-none">2</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Change of Rank <span class="spnApplyForcard d-none">3</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Change of Army No <span class="spnApplyForcard d-none">4</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Lost Card<span class="spnApplyForcard d-none">5</span></button>';

    $("#icardrequestfor").html(list);

    $('.applyforicard').on("click",function () {

        $('.applyforicard').removeClass("btn-primary");
        $('.applyforicard').addClass("btn-outline-primary");

        $(this).removeClass("btn-outline-primary");
        $(this).addClass("btn-primary");

        $("#spnNext").removeClass("d-none");


        lCardType = parseInt($(this).closest("button").find(".spnApplyForcard").html());

        if (lCardType == 4) {
            inputField.placeholder = "Enter New Army No With pfx & sfx Ex.ICXXXXXP,JC391516M,15698645A";

            $("#txtApplyForOldArmyNo").val("");
            $("#txtApplyForOldArmyNo").removeClass("d-none");
            
        }
        else {
            inputField.placeholder = "Enter Army No With pfx & sfx Ex.ICXXXXXP,JC391516M,15698645A";
            $("#txtApplyForOldArmyNo").addClass("d-none");
            $("#txtApplyForOldArmyNo").val("");
        }

        $("#txtApplyForArmyNo").addClass("d-none");
        $("#txtApplyForArmyNo").val("");
        if (OffType == 1) {
            GetByArmyNoIsToken($("#aspntokenarmyno").html());
        }
        else if (OffType == 2) {

            if (lCardType != 4) {
                $("#txtApplyForArmyNo").removeClass("d-none");
                $("#btntokenrefresh").addClass("d-none");
                $('#txtApplyForArmyNo').attr('readonly', false);

                $("#txtApplyForOldArmyNo").addClass("d-none");
                if ($("#txtApplyForOldArmyNo").attr('data-val-required')) {
                    $("#txtApplyForOldArmyNo").removeAttr('data-val-required');
                }
            }
            else {
                $("#txtApplyForArmyNo").removeClass("d-none");
                $("#btntokenrefresh").addClass("d-none");
                $('#txtApplyForArmyNo').attr('readonly', false);

                $("#txtApplyForOldArmyNo").removeClass("d-none");
                $("#txtApplyForOldArmyNo").attr('data-val-required', 'Old Army No is required.');
            }
        }
    });
}
function GetByArmyNoIsToken(ArmyNo) {
    var userdata =
    {
        "ArmyNo": ArmyNo,

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
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {

                }

                else {
                    IsWithTokenApply = response.IsWithTokenApply;
                    IsToken = response.IsToken;

                    if (parseInt(OffType) == 1) {
                        if ((parseInt(RegistrationApplyFor) == 2 || parseInt(RegistrationApplyFor) == 3 || parseInt(RegistrationApplyFor) == 4 || parseInt(RegistrationApplyFor) == 10)) {
                            if (IsWithTokenApply == true && lCardType != 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                $("#txtApplyForArmyNo").addClass("d-none");///for bypass for off
                                $('#btnNext').addClass("disabled");

                                $("#txtApplyForOldArmyNo").addClass("d-none");
                                if ($("#txtApplyForOldArmyNo").attr('data-val-required')) {
                                    $("#txtApplyForOldArmyNo").removeAttr('data-val-required');
                                }
                            }
                            else if (IsWithTokenApply == true && lCardType == 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                $("#txtApplyForArmyNo").addClass("d-none");///for bypass for off

                                $('#btnNext').addClass("disabled");

                                $("#txtApplyForOldArmyNo").removeClass("d-none");
                                $("#txtApplyForOldArmyNo").attr('data-val-required', 'Old Army No is required.');
                            }
                            else {
                                $("#btntokenrefresh").addClass("d-none");
                                $("#txtApplyForArmyNo").removeClass("d-none");///for bypass for off

                                $("#txtApplyForArmyNo").val("");
                                $('#txtApplyForArmyNo').attr('readonly', false);


                                $('#btnNext').removeClass("disabled");


                                if (lCardType != 4) {
                                    $("#txtApplyForOldArmyNo").addClass("d-none");
                                    if ($("#txtApplyForOldArmyNo").attr('data-val-required')) {
                                        $("#txtApplyForOldArmyNo").removeAttr('data-val-required');
                                    }
                                }
                                else {
                                    $("#txtApplyForOldArmyNo").removeAttr("d-none");
                                    $("#txtApplyForOldArmyNo").attr('data-val-required', 'Old Army No is required.');
                                }
                            }
                        }
                        else {
                            if (IsToken == true && parseInt(RegistrationApplyFor) == 1 && lCardType != 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                $("#txtApplyForArmyNo").addClass("d-none");///for bypass for off
                                $('#btnNext').addClass("disabled");

                                $("#txtApplyForOldArmyNo").addClass("d-none");
                                if ($("#txtApplyForOldArmyNo").attr('data-val-required')) {
                                    $("#txtApplyForOldArmyNo").removeAttr('data-val-required');
                                }
                            }
                            else if (IsToken == true && parseInt(RegistrationApplyFor) == 1 && lCardType == 4) {
                                $("#btntokenrefresh").removeClass("d-none");
                                $("#txtApplyForArmyNo").addClass("d-none");///for bypass for off
                                $('#btnNext').addClass("disabled");

                                $("#txtApplyForOldArmyNo").removeClass("d-none");
                                $("#txtApplyForOldArmyNo").attr('data-val-required', 'Old Army No is required.');
                            }
                            else {
                                $("#btntokenrefresh").addClass("d-none");
                                $("#txtApplyForArmyNo").removeClass("d-none");///for bypass for off

                                $("#txtApplyForArmyNo").val($("#aspntokenarmyno").html());
                                $('#txtApplyForArmyNo').attr('readonly', true);

                                if (lCardType != 4) {
                                    $("#txtApplyForOldArmyNo").addClass("d-none");
                                    if ($("#txtApplyForOldArmyNo").attr('data-val-required')) {
                                        $("#txtApplyForOldArmyNo").removeAttr('data-val-required');
                                    }
                                }
                                else {
                                    $("#txtApplyForOldArmyNo").removeAttr("d-none");
                                    $("#txtApplyForOldArmyNo").attr('data-val-required', 'Old Army No is required.');
                                }

                                $('#btnNext').removeClass("disabled");
                            }   
                        }
                    }
                    else {
                        if (lCardType != 4) {
                            $("#txtApplyForArmyNo").removeClass("d-none");
                            $("#btntokenrefresh").addClass("d-none");
                            $('#txtApplyForArmyNo').attr('readonly', false);

                            $("#txtApplyForOldArmyNo").addClass("d-none");
                            if ($("#txtApplyForOldArmyNo").attr('data-val-required')) {
                                $("#txtApplyForOldArmyNo").removeAttr('data-val-required');
                            }
                            
                        }
                        else {
                            $("#txtApplyForArmyNo").removeClass("d-none");
                            $("#btntokenrefresh").addClass("d-none");
                            $('#txtApplyForArmyNo').attr('readonly', false);

                            $("#txtApplyForOldArmyNo").removeClass("d-none");
                            $("#txtApplyForOldArmyNo").attr('data-val-required', 'Old Army No is required.');
                        }
                    }
                }
            }

        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function CheckArmyNOExist() {
    $.ajax({
        url: "/BasicDetail/GetData",
        type: "POST",
        data: {
            "ICNumber": lCardType == 4 ? $("#txtApplyForOldArmyNo").val() : $("#txtApplyForArmyNo").val(),
            "lCardType": lCardType
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response, status) {
            if (response.Status == false) {

                toastr.error(response.Message);
            }
            else {
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

                        var encryptedArmyNo = encryptData($("#txtApplyForArmyNo").val());
                        var encryptedOldArmyNo = encryptData($("#txtApplyForOldArmyNo").val());
                        var encryptedOffType = encryptData(OffType.toString());
                        var encryptedRegistrationApplyFor = encryptData(RegistrationApplyFor.toString());
                        var encryptedlCardType = encryptData(lCardType.toString());

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
    // Simulating an asynchronous check (no external fetch needed)
    await new Promise(resolve => resolve()); // The 'await' here is just for structure

    if (input.length >= 2 && /^[a-zA-Z]{2}/.test(input)) {
        return input.substring(0, 2).toUpperCase();
    } else {
        return ''; // Return empty if not alphabetic or less than 2 characters
    }
}
async function ChkSfx(ServiceNo) {
    // Simulating an asynchronous check (no external fetch needed)
    await new Promise(resolve => resolve()); // The 'await' here is just for structure

    let ArmyNo = ServiceNo;

    var armyno = ArmyNo.replace(/[A-Za-z]/g, '');

    var txt = ArmyNo.slice(-1);
    // Get last character
    const lastChar = ArmyNo.slice(-1);

    // Check if it is an alphabet
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
        case (0):
            {
                Sfx = "A"
                break;
            }
        case (1):
            {
                Sfx = "F"
                break;
            }
        case (2):
            {
                Sfx = "H"
                break;
            }
        case (3):
            {
                Sfx = "K"
                break;
            }
        case (4):
            {
                Sfx = "L"
                break;
            }
        case (5):
            {
                Sfx = "M"
                break;
            }
        case (6):
            {
                Sfx = "N"
                break;
            }
        case (7):
            {
                Sfx = "P"
                break;
            }
        case (8):
            {
                Sfx = "W"
                break;
            }
        case (9):
            {
                Sfx = "X"
                break;
            }
        case (10):
            {
                Sfx = "Y"
                break;
            }
    }
    if (Sfx === lastChar) {
        return true;
    }
    else {
        return false;
    }
    
}