$(document).ready(function () {

    $("#btnApplyCard").click(function () {



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
  
});
function GetAllRegistrationApplyFor(Id) {
   
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



                    for (var i = 0; i < response.length; i++) {

                        listItem += '<button type="button" class="btn btn-outline-success mt-4 mr-2 applyforoffs" id="icardFor' + response[i].RegistrationId + '">' + response[i].Name + '<span class="spnRegistration d-none">' + response[i].RegistrationId +'</span></button>';
                        
                       
                    }

                    $("#btnIcardFor").html(listItem);
                    $("#icardrequestfor").html("");
              
                    $('.applyforoffs').click(function () {
                        $('.applyforoffs').removeClass("btn-success");
                        $('.applyforoffs').addClass("btn-outline-success");

                      
                        $(this).removeClass("btn-outline-success");
                        $(this).addClass("btn-success");

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
    var list = '';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard">First time Smart card</button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard">Fair wear and tear </button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard">Change of Rank</button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard">Change of Army No</button>';
    list += '<button type="button" class="btn btn-outline-info mt-4 ml-2 applyforicard">Loss/ Damaged</button>';

    $("#icardrequestfor").html(list);

    $('.applyforicard').click(function () {
        $('.applyforicard').removeClass("btn-info");
        $('.applyforicard').addClass("btn-outline-info");


        $(this).removeClass("btn-outline-info");
        $(this).addClass("btn-info");

        // alert($(this).closest("button").find(".spnRegistration").html());
       // AddAllCardType();
    });
}