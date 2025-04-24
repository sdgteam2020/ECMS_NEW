var OffType = 0;
var RegistrationApplyFor = 0;
var lCardType = 0;
var IsValid = 0;
var Message = "";
var IsToken = true;
var IsWithTokenApply = true;
$(document).ready(function () {

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
            if (parseInt(OffType) != 0 && parseInt(RegistrationApplyFor) != 0 && parseInt(lCardType) != 0) {
                if (OffType == 1 && parseInt(RegistrationApplyFor) == 1) {

                    if ($("#txtApplyForArmyNo").val() === $("#aspntokenarmyno").html()) {
                        IsValid = 1;
                    } else {

                        Message = "Please Inset Valid Token Token ArmyNo And Login ArmyNo Not Match";
                        IsValid = 0;
                    }


                }
                if (OffType == 1 && parseInt(RegistrationApplyFor) == 2) {


                    if ($("#txtApplyForArmyNo").val() != "") {
                        IsValid = 1;
                    }
                    else {

                        Message = "Please Inset Token";
                        IsValid = 0;
                    }
                }
                else if (OffType == 1 && parseInt(RegistrationApplyFor) != 1) {
                    if ($("#txtApplyForArmyNo").val() != "") {
                        IsValid = 1;
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
                        IsValid = 1;
                    }
                }
                if (IsValid == 1) {

                    CheckArmyNOExist();
                }
                else {
                    toastr.error(Message);
                }
            }
            else {
                toastr.error("Invalid Selected");
            }
        }
        else {
            toastr.error("Army No minlength is eight character.");
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

        success: function (response) {
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

                    $('.applyforoffs').click(function () {
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
    lCardType = 0;
    var list = '';
    list += '<div class="seven mt-4" ><h1>Reason For Applying</h1>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">First time Smart card <span class="spnApplyForcard d-none">1</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Fair wear and tear <span class="spnApplyForcard d-none">2</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Change of Rank <span class="spnApplyForcard d-none">3</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Change of Army No <span class="spnApplyForcard d-none">4</span></button>';
    list += '<button type="button" class="btn btn-outline-primary mt-4 ml-2 applyforicard btn1">Loss/ Damaged <span class="spnApplyForcard d-none">5</span></button>';

    $("#icardrequestfor").html(list);

    $('.applyforicard').click(function () {

        $('.applyforicard').removeClass("btn-primary");
        $('.applyforicard').addClass("btn-outline-primary");

        $(this).removeClass("btn-outline-primary");
        $(this).addClass("btn-primary");

        $("#spnNext").removeClass("d-none");


        lCardType = $(this).closest("button").find(".spnApplyForcard").html();

        $("#txtApplyForArmyNo").addClass("d-none");
        $("#txtApplyForArmyNo").val("");
        if (OffType == 1) {
            GetByArmyNoIsToken($("#aspntokenarmyno").html());
        }
        else if (OffType == 2) {
            $("#txtApplyForArmyNo").removeClass("d-none");
            $("#btntokenrefresh").addClass("d-none");
            $('#txtApplyForArmyNo').attr('readonly', false);
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
                            if (IsWithTokenApply == true) {
                                $("#btntokenrefresh").removeClass("d-none");
                                $("#txtApplyForArmyNo").addClass("d-none");///for bypass for off
                                $('#btnNext').addClass("disabled");
                            }
                            else {
                                $("#btntokenrefresh").addClass("d-none");
                                $("#txtApplyForArmyNo").removeClass("d-none");///for bypass for off

                                $("#txtApplyForArmyNo").val("");
                                $('#txtApplyForArmyNo').attr('readonly', false);

                                $('#btnNext').removeClass("disabled");
                            }
                        }
                        else {
                            if (IsToken == true && parseInt(RegistrationApplyFor) == 1) {
                                $("#btntokenrefresh").removeClass("d-none");
                                $("#txtApplyForArmyNo").addClass("d-none");///for bypass for off
                                $('#btnNext').addClass("disabled");
                            }
                            else {
                                $("#btntokenrefresh").addClass("d-none");
                                $("#txtApplyForArmyNo").removeClass("d-none");///for bypass for off

                                $("#txtApplyForArmyNo").val($("#aspntokenarmyno").html());
                                $('#txtApplyForArmyNo').attr('readonly', true);

                                $('#btnNext').removeClass("disabled");
                            }
                        }
                    }
                    else {
                        $("#btntokenrefresh").removeClass("d-none");
                        $("#txtApplyForArmyNo").addClass("d-none");///for bypass for off
                        $('#txtApplyForArmyNo').attr('readonly', false);
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
            "ICNumber": $("#txtApplyForArmyNo").val()
        },
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
                        var secretKey = document.getElementById("spnUniqueSecretKey").innerText;

                        var encryptedArmyNo = CryptoJS.AES.encrypt($("#txtApplyForArmyNo").val(), secretKey).toString();
                        var encryptedOffType = CryptoJS.AES.encrypt(OffType.toString(), secretKey).toString();
                        var encryptedRegistrationApplyFor = CryptoJS.AES.encrypt(RegistrationApplyFor.toString(), secretKey).toString();
                        var encryptedlCardType = CryptoJS.AES.encrypt(lCardType.toString(), secretKey).toString();

                        sessionStorage.setItem("OffType", encryptedOffType);
                        sessionStorage.setItem("RegistrationApplyFor", encryptedRegistrationApplyFor);
                        sessionStorage.setItem("lCardType", encryptedlCardType);
                        sessionStorage.setItem("ArmyNo", encryptedArmyNo);
                        window.location.href = "/BasicDetail/Registration";
                    }
                });
            }
        }
    });

}

