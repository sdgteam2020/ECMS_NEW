$(document).ready(function () {
    $("#btnTracking").on("click", function () {
        
        GetRequestHistoryByTrackingId($("#TrackingId").val());
   });
});
function GetRequestHistoryByTrackingId(TrackingId) {
   
    var userdata =
    {

        "TrackingId": TrackingId,


    };
    var listItem = "";
    $.ajax({
        url: '/ApplicationStatus/GetRequestHistoryByTrackingId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                if (response.length > 0) {
                    for (var i = 0; i < response.length; i++) {
                        if (i == 0) {
                            listItem += '<div class="timeline-item">';
                            listItem += '<div class="timeline-item-marker">';
                            listItem += '<div class="timeline-item-marker-text "><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';
                            listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                            listItem += '</div>';
                            listItem += '<div class="timeline-item-content">';
                            listItem += 'I-Card Submit By -' + response[i].FromDomain + '(' + response[i].FromRank + ' ' + response[i].FromProfile + ')';

                            listItem += '</div>';
                            listItem += '</div>';
                        }
                        listItem += '<div class="timeline-item">';
                        listItem += '<div class="timeline-item-marker">';

                        if (response[i].IsComplete == 0 && response[i].Status == "Pending")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';
                        else if (response[i].Status == "Approved")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';
                        else if (response[i].Status == "Reject")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-danger">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';
                        else if (response[i].Status == "Internal Forward")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';


                        listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                        listItem += '</div>';
                        listItem += '<div class="timeline-item-content">';


                        listItem += '' + response[i].FromDomain + '(' + response[i].FromRank + ' ' + response[i].FromProfile + ')';

                        if (response[i].IsComplete == 0 && response[i].Status == "Pending")
                            listItem += '<br><span class="badge bg-success">' + 'Pending' + ' And Sent To</span>';
                        else if (response[i].Status == "Approved")
                            listItem += '<br><span class="badge bg-success">' + response[i].Status + ' And Sent To</span>';
                        else if (response[i].Status == "Reject")
                            listItem += '<br><span class="badge bg-danger">' + response[i].Status + ' And Sent To</span>';
                        else if (response[i].Status == "Internal Forward")
                            listItem += '<br><span class="badge bg-success">' + response[i].Status + ' And Sent To</span>';

                        listItem += '<br> <strong class="text-center">Remark</strong> <br>' + response[i].Remark + '';

                        if (response[i].Remarks2 != null) {
                            var rem = response[i].Remarks2.split('#');
                            if (rem.length > 0) {

                                listItem += '<ul>';
                                for (var j = 0; j < rem.length; j++) {
                                    listItem += '<li>' + rem[j] + '</li>';
                                }
                                listItem += '</ul>';
                            }
                        }


                        listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'

                        if (response[i].IsComplete == 0) {
                            listItem += '<br><span class="badge bg-warning ">Pending from </span>';
                        }
                        listItem += '<br>' + response[i].ToDomain + '(' + response[i].ToRank + ' ' + response[i].ToProfile + ')';


                        if (response[i].Reason != null) {
                            listItem += '<br> <strong class="text-center text-danger">' + response[i].Reason + '</strong> <br> Unit Name :-' + response[i].UnitName + '';
                        }



                        listItem += '</div>';
                        listItem += '</div>';
                    }
                }
                else {
                    listItem += '<div class="timeline-item">';
                    listItem += '<div class="timeline-item-marker">';


                    listItem += '</div>';
                    listItem += '<div class="timeline-item-content">';
                    listItem += 'TrackingId Not Found';

                    listItem += '</div>';
                    listItem += '</div>';

                    $("#RequestHistoryTrackingId").html(listItem);
                }

                $("#RequestHistoryTrackingId").html(listItem);
            } else {

            }
        }

    });
}