$(document).ready(function () {

    const [today] = new Date().toISOString().split('T');

    const maxDate = new Date();
    maxDate.setDate(maxDate.getDate() + 30);
    const [maxDateFormatted] = maxDate.toISOString().split('T');

    const dateInput = document.getElementById('DateOfIssue');
    dateInput.setAttribute('min', today);
    dateInput.setAttribute('max', maxDateFormatted);

    $("#TermsConditions").click(function () {

        if ($("#TermsConditions").prop("checked") == true) {
            $("#btnsave").removeClass("disabled");
        }
        else {
            $("#btnsave").addClass("disabled");
        }
    });


    if ($("#RegistrationId").val() == 1 || $("#RegistrationId").val() == 2 || $("#RegistrationId").val() == 6) {
        GetUnit() 
        $('#txtUnit').attr('readonly', true);
      //  getunitbymapid($("#aspndomainUnitID").html())
    }
    else {
        $('#txtUnit').attr('readonly', false);
        getunitbymapid($("#spnUnitIdid").val())

    }
    getApplyIcardDetails();

    if ($("#ApplyForId").val() == 1) {
        $(".OptionsRegimental").addClass("d-none");
        mMsater($("#spnrankid").val(), "RankId", Rank, "");
    } else if ($("#ApplyForId").val() == 2) {
        mMsater($("#spnrankid").val(), "RankId", RankJCo, "");
        $(".OptionsRegimental").removeClass("d-none");
    }

    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#ServiceNumber").val(sessionStorage.getItem("ArmyNo"));
        $("#icarddetails").html('Request Details For ('+sessionStorage.getItem("ArmyNo")+')');
        if (sessionStorage.getItem("OffType") == 1) {
            $(".OptionsRegimental").addClass("d-none");
            mMsater($("#spnrankid").val(), "RankId", Rank, "");
        }
        else if (sessionStorage.getItem("OffType") == 2) {
            {
                mMsater($("#spnrankid").val(), "RankId", RankJCo, "");
                $(".OptionsRegimental").removeClass("d-none");
            }
        }
        if (sessionStorage.getItem("OffType") != "")
            $("#ApplyForId").val(sessionStorage.getItem("OffType"));
        $("#Type").val(sessionStorage.getItem("OffType"));

        if (sessionStorage.getItem("lCardType") != "")
            $("#TypeId").val(sessionStorage.getItem("lCardType"));

    }


    $("#txtUnit").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                var param = { "UnitName": request.term };
                $("#UnitId").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return { label: item.UnitName, value: item.UnitMapId };

                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
           /* $("#txtUnit").val(i.item.label);*/
            //alert(i.item.value)
            getunitbymapid(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

  
});
function CheckValidation() {
    
    if ($("#TermsConditions").prop("checked")) {
        if ($("#SaveForm")[0].checkValidity()) {
           // alert("Your Tracking Id -" + DateFormateMMddyyyy($("#DOB").val()) + "" + $("#AadhaarNo").val().substr($("#AadhaarNo").val().length - 4));
        }
        return true;
        
    }
    else {
        toastr.error('Please accept the Terms and Conditions');
        return false;
    }
}
function GetUnit() {
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/ConfigUser/GetTokenArmyNo',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == 0) {
                    //  alert("Plase Add Profile")
                }
                else {
                    getunitbymapid(response.UnitId)
                    
                   
                }
            }
        }
    });
}
function getunitbymapid(value)
{
   
    var param1 = { "UnitMapId": value };
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: param1,
        type: 'POST',
        success: function (data) {

            $("#txtUnit").val(data.UnitName);
            $("#UnitId").val(data.UnitMapId);
            //$("#lblProComd").html(data.ComdName);
            //$("#lblProCorps").html(data.CorpsName);
            //$("#lblProDiv").html(data.DivName);
            //$("#lblPrBde").html(data.BdeName);
            //$("#lblProSusno").html(data.Sus_no + '' + data.Suffix);

        }
    });
}

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


            }

        }
    });
}