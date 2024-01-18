var OffType = 0;
var RegistrationApplyFor = 0;
var lCardType = 0;
var IsValid = 0;
var Message = "";
$(document).ready(function () {

    $("#btnSercharmynoSmart").click(function () {
        if ($("#armynosearchAllName").html() != "") {
            
            $("#unitoffrsModal").modal("hide");
            sessionStorage.setItem("ArmyNo", $("#txtarmynosearchAll").val());
            window.location.href = "/Posting/PostingIn";

        } else {
            toastr.error("Please Enter Army No");
        }


    });
    $("#btnApplyCard").click(function () {

        RegistrationApplyFor = 0;
       
        $("#btnApplyCard").removeClass("btn-outline-primary");
        $("#btnApplyCard").addClass("btn-primary");
        //var list = '';
        //list += '';
        $("#btnarmytype").removeClass("d-none");
    });
    $("#btnaddOffrs").click(function () {

        $("#btnaddOffrs").removeClass("btn-outline-success");
        $("#btnaddOffrs").addClass("btn-success");


        $("#btnJCOs").addClass("btn-outline-warning");
        $("#btnJCOs").removeClass("btn-warning");
        GetAllRegistrationApplyFor(1);

    });
    $("#btnJCOs").click(function () {

        $("#btnaddOffrs").removeClass("btn-success");
        $("#btnaddOffrs").addClass("btn-outline-success");

        $("#btnJCOs").addClass("btn-warning");
        $("#btnJCOs").removeClass("btn-outline-warning");
        $("#btnJCOs").addClass("btn-warning");

        GetAllRegistrationApplyFor(2);
    });
    $("#btntokenrefresh").click(function () {
        GetTokenDetails1("FetchUniqueTokenDetails", "txtApplyForArmyNo");
    });
    $("#btnNext").click(function () {
        if (parseInt(OffType)!= 0 && parseInt(RegistrationApplyFor) != 0 && parseInt(lCardType) != 0)
        {
                if(OffType == 1 && parseInt(RegistrationApplyFor) == 1) {
            if ($("#txtApplyForArmyNo").val() == $("#aspntokenarmyno").html()) {
                IsValid = 1;
            }
            else {
                Message = "Invalid Token Inserted ArmyNo Not Match";
                IsValid = 0;
            }
        }
        else if (OffType == 1 && parseInt(RegistrationApplyFor) != 1) {
            if ($("#txtApplyForArmyNo").val() != "") {
                IsValid = 1;
            }
            else {
                Message = "Please Inset Token";
                IsValid = 0;
            }
        }
        else if (OffType == 2) {
            if ($("#txtApplyForArmyNo").val() == "") {
                IsValid = 0;
                Message = "Plase Enter Army No";
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
    });
});
function GetAllRegistrationApplyFor(Id) {
    $("#spnNext").addClass("d-none");
    $("#txtApplyForArmyNo").addClass("d-none");
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

                    for (var i = 0; i < response.length; i++) {

                        listItem += '<button type="button" class="btn btn-outline-success mt-4 mr-2 applyforoffs btn1" id="icardFor' + response[i].RegistrationId + '">' + response[i].Name + '<span class="spnRegistration d-none">' + response[i].RegistrationId +'</span></button>';
                        
                       
                    }

                    $("#btnIcardFor").html(listItem);
                    $("#icardrequestfor").html("");
              
                    $('.applyforoffs').click(function () {
                        $('.applyforoffs').removeClass("btn-success");
                        $('.applyforoffs').addClass("btn-outline-success");

                      
                        $(this).removeClass("btn-outline-success");
                        $(this).addClass("btn-success");

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
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard btn1">First time Smart card <span class="spnApplyForcard d-none">1</span></button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard btn1">Fair wear and tear <span class="spnApplyForcard d-none">2</span></button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard btn1">Change of Rank <span class="spnApplyForcard d-none">3</span></button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard btn1">Change of Army No <span class="spnApplyForcard d-none">4</span></button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard btn1">Loss/ Damaged <span class="spnApplyForcard d-none">5</span></button>';

    $("#icardrequestfor").html(list);

    $('.applyforicard').click(function () {
        $('.applyforicard').removeClass("btn-info");
        $('.applyforicard').addClass("btn-outline-info");

        $(this).removeClass("btn-outline-info");
        $(this).addClass("btn-info");

        $("#spnNext").removeClass("d-none");
       
       
        lCardType = $(this).closest("button").find(".spnApplyForcard").html();



        $("#txtApplyForArmyNo").addClass("d-none");
        $("#txtApplyForArmyNo").val("");
        if (OffType == 1 && RegistrationApplyFor == 1) {
            GetTokenDetails1("FetchUniqueTokenDetails", "txtApplyForArmyNo");
            $("#btntokenrefresh").removeClass("d-none");

        }
        else if (OffType == 1 && RegistrationApplyFor != 1) {
            GetTokenDetails1("FetchUniqueTokenDetails", "txtApplyForArmyNo");
            $("#btntokenrefresh").removeClass("d-none");
        }
        else if (OffType == 2) {
            $("#txtApplyForArmyNo").removeClass("d-none");
            $("#btntokenrefresh").addClass("d-none");
        }

        // alert($(this).closest("button").find(".spnRegistration").html());
       // AddAllCardType();
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
                                    sessionStorage.setItem("OffType", OffType);
                                    sessionStorage.setItem("RegistrationApplyFor", RegistrationApplyFor);
                                    sessionStorage.setItem("lCardType", lCardType);
                                    sessionStorage.setItem("ArmyNo", $("#txtApplyForArmyNo").val());
                                    window.location.href = "/BasicDetail/Registration";
                                }
                            });
                        }
                    }
                });
     
}

