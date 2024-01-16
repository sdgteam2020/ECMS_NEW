$(document).ready(function () {

    $("body").on("click", ".cls-btnhistory", function () {
        
        $("#modalLoginLog").modal('show');
        GetLog($(this).closest("tr").find("#AspNetUsersId").html())
    });

});
function GetLog(AspNetUsersId) {
    
    var userdata =
    {
        "AspNetUsersId": AspNetUsersId,

    };
    $.ajax({
        url: '/Log/LoginLogByAspNetUsersId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == -1) {
                    $("#timelineData").html('<span class="timeline-label"><span class="label">No Record</span></span>');
                }
                else if (response == 0) {
                    $("#timelineData").html('<span class="timeline-label"><span class="label">No Record</span></span>');
                }
                else if (response.length == 0) {
                    $("#timelineData").html('<span class="timeline-label"><span class="label">No Record</span></span>');
                }

                else {
                    var listItem = "";
                    var listItemProc = "";
                    var details = "";
                    var type = "";
                    var fmdate = "";
                    var years = "";
                    var color = "";
                    var txt = "";
                    var leftmarpre = 0;
                    var colorTime = "";
                    var headerlve = 0;

                   
                    for (var i = 0; i < response.length; i++) {


                        var datef2 = new Date(response[i].UpdatedOn);
                        fmdate = datef2;


                        ////////////////
                        if (years != datef2.getFullYear()) {

                            listItem += '<span class="timeline-label"><span class="label">' + datef2.getFullYear() + '</span></span>';

                        }
                        years = datef2.getFullYear();
                        //if (response[i].RoleName == "User") {
                        //    type = "Leave Event";
                        //    imgname = "leave.png";
                        //    color = "badge badge-success";
                        //    txt = "text-success";

                        //}
                        //else if (response[i].type == "admin") {
                        //    type = "Family Event";
                        //    imgname = "familyevent.jpg";
                        //    color = "badge badge-orange";
                        //    txt = "text-orange";
                        //}
                        //else if (response[i].type == 3) {
                        //    type = "Persent Event";
                        //    type = "Promotion Event";
                        //    color = "badge badge-danger";
                        //    txt = "text-danger";
                        //}
                        //else if (response[i].type == 4) {
                        //    type = "Course Event";
                        //    type = "Promotion Event";
                        //    color = "badge badge-blue";
                        //    txt = "text-blue";
                        //}
                        //else if (response[i].type == 5) {
                        //    type = "Promotion Event";
                        //    imgname = "promotion.png";
                        //    color = "badge badge-purple";
                        //    txt = "text-purple";
                        //}

                        listItem += ' <div class="timeline-item">';
                        listItem += '<div class="timeline-point timeline-point"></div>';
                        listItem += '<div class="timeline-event">';
                        listItem += '<div class="widget has-shadow">';
                        listItem += '<div class="widget-header d-flex align-items-center">';
                        //listItem += '<div class="user-image">';
                        //listItem += '<img class="rounded-circle" src="../images/asset/' + imgname + '" alt="' + type +'" />';
                        //listItem += '</div>';
                        listItem += '<div class="d-flex flex-column mr-auto">';
                        listItem += '<div class="title">';
                        //<span class="username">' + type + '</span>
                        listItem += ' <span class="badge badge-danger">' + DateFormatehhmmss(response[i].UpdatedOn) + '</span> ';

                        listItem += '</div>';
                        listItem += '</div>';

                        listItem += '<div class="widget-options">';
                        listItem += '<div class="dropdown">';
                        listItem += '<span class="badge badge-orange">' + DateFormateMMMM_dd_yyyy(response[i].UpdatedOn) + '</span>';
                        listItem += ' </div> </div>';
                        listItem += '</div>';
                        listItem += ' <div class="widget-body">';
                        listItem += '<p class="text-blue"> IP Address  : <span class="badge badge-purple">' + response[i].IP + '</span><p>';
                        listItem += '<p class="text-blue"> DomainId  : ' + response[i].DomainID + '<p>';
                        listItem += '<p class="text-blue"> RoleName  : ' + response[i].RoleName + '<p>';
                        listItem += '<p class="text-blue"> ArmyNo  : ' + response[i].ArmyNo + '<p>';
                        listItem += '<p class="text-blue"> User Name : ' + response[i].RankName + ' ' + response[i].Name+' </p>';
                        listItem += '</div>';
                        listItem += ' </div>';
                        listItem += ' <div class="time-right">' + DateCalculateago(response[i].UpdatedOn) + '</div>';
                        listItem += '</div>';
                        listItem += '</div>';


                    }
                    listItem += '<span class="timeline-label">';
                    listItem += '<span class="label bg-primary"> End ' + fmdate.getFullYear() + '</span>';
                    listItem += '</span>';



                    $("#timelineData").html(listItem);



                }
            }
            else {
                //listItem += "<tr><td class='text-center' colspan=10>No Record Found</td></tr>";

                //$("#DetailBody").html(listItem);
                //$("#lblTotal").html(0);

            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}