var memberTable = "";
$(document).ready(function () {

    $("body").on("click", ".cls-btnhistory", function () {
        
       
        $(".loginlodetails").html($(this).closest("tr").find(".DomainID").html() + '(' + $(this).closest("tr").find(".ArmyNo").html() +') (' + $(this).closest("tr").find(".RankName").html() + '' + $(this).closest("tr").find(".Name").html() + ') ');

        var fmdate = new Date($("#FmDate").val());
        var todate = new Date($("#ToDate").val());
        if (fmdate <= todate) {

            $("#modalLoginLog").modal('show');
            GetLog($(this).closest("tr").find("#AspNetUsersId").html(), $("#FmDate").val(), $("#ToDate").val())
        } else {
            if ($("#FmDate").val() == "" && $("#ToDate").val() == "") {
                $("#modalLoginLog").modal('show');
                GetLog($(this).closest("tr").find("#AspNetUsersId").html())
            }else
                toastr.error('Please Select Valid date');

        }
    });

});
function GetLog(AspNetUsersId, FmDate,ToDate) {
    
    var userdata =
    {
        "AspNetUsersId": AspNetUsersId,
        "FmDate": FmDate,
        "ToDate": ToDate,

    };
    $.ajax({
        url: '/Log/LoginLogByAspNetUsersId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == -1) {
                    $("#timelineData").html('<span class="timeline-label"><span class="label">No Login Log </span></span>');
                }
                else if (response == 0) {
                    $("#timelineData").html('<span class="timeline-label"><span class="label">No Login Log </span></span>');
                }
                else if (response.length == 0) {
                    $("#timelineData").html('<span class="timeline-label"><span class="label">No Login Log </span></span>');
                }

                else {
                    var listItem = "";
                    var listItem2 = "";
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
                        listItem2 += '<tr>';
                        listItem2 += '<td>' + response[i].RoleName +'</td>';
                        listItem2 += '<td>' + response[i].DomainID +'</td>';
                        listItem2 += '<td>' + response[i].ArmyNo +'</td>';
                        listItem2 += '<td>' + response[i].RankName +'</td>';
                        listItem2 += '<td>' + response[i].Name +'</td>';
                        listItem2 += '<td>' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</td>';
                        listItem2 += '<td>' + response[i].IP + '</td>';
                        listItem2 += '</tr>';



                        var datef2 = new Date(response[i].UpdatedOn);
                        fmdate = datef2;


                        ////////////////
                        if (years != datef2.getFullYear()) {

                            listItem += '<span class="timeline-label"><span class="label">' + datef2.getFullYear() + '</span></span>';

                        }
                        years = datef2.getFullYear();
                        

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
                        //listItem += '<p class="text-blue"> DomainId  : ' + response[i].DomainID + '<p>';
                        //listItem += '<p class="text-blue"> RoleName  : ' + response[i].RoleName + '<p>';
                        //listItem += '<p class="text-blue"> ArmyNo  : ' + response[i].ArmyNo + '<p>';
                        //listItem += '<p class="text-blue"> User Name : ' + response[i].RankName + ' ' + response[i].Name+' </p>';
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


                    $("#DataBoady").html(listItem2);

                    memberTable = $('#tbldata').DataTable({
                        retrieve: true,
                        lengthChange: false,
                        searching: false,
                        paging: false, info: false,
                        buttons: [{
                            extend: 'copy',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }, {
                            extend: 'excel',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }]
                    });
                    memberTable.buttons().container().appendTo('#tbldata_wrapper .col-md-6:eq(0)');
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