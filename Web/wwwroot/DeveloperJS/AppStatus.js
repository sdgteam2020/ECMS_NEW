$(function () {

    $(".ApplIdDetails").addClass("d-none");
   
    $("#btnTracking").on("click", function () {
        GetRequestHistoryByApplId($("#ApplId").val());
    });
    $("#btn-printtracking").on("click",function () {
        //var datef2 = new Date();
        //$(".watermark").html($(".ipaddress").html() + ' ' + DateFormateddMMyyyyhhmmss(datef2))
        //window.print();
        PrintAppStatusData("section-to-print-app-status");
    });
});
function GetRequestHistoryByApplId(ApplId) {
   
    var userdata =
    {
        "RequestId": ApplId,
    };
    var listItem = "";
    $.ajax({
        url: '/ApplicationStatus/GetRequestHistoryByRequestId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                if (response.length > 0) {
                    GetDataFromBasicDetails(response[0].RequestId)
                    $(".ApplIdDetails").removeClass("d-none");
                    $(".ApplIdHistory").removeClass("d-none");
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
                    $(".ApplIdDetails").addClass("d-none");
                    $(".ApplIdHistory").removeClass("d-none");
                    
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
function GetDataFromBasicDetails(RequestId) {
    var userdata =
    {
        "RequestId": RequestId,


    };
    $.ajax({
        url: '/ApplicationStatus/GetBasicDetailByRequestId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                $(".PhotoImagePath").attr('src', response.ExistingPhotoInBase64);
                $(".SignaturePath").attr('src', response.ExistingSignatureInBase64);
                $(".FName").html(response.FName);
                $(".LName").html(response.LName == null ? "&nbsp;" : response.LName);
                $(".RankName").html(response.RankName);
                $(".ArmedName").html(response.ArmedName);
                $(".ServiceNo").html(response.ServiceNo);
                $(".IdenMark1").html(response.IdenMark1);
                $(".DOB").html(DateFormateMMMM_dd_yyyy(response.DOB));
                $(".Height").html(response.Height+' CM');
                $(".AadhaarNo").html(response.AadhaarNo.replace(/\d(?=\d{4})/g, "X"));
                $(".BloodGroup").html(response.BloodGroup);
                $(".PlaceOfIssue").html(response.PlaceOfIssue);
                $(".DateOfIssue").html(DateFormateMMMM_dd_yyyy(response.DateOfIssue));
                $(".IssuingAuth").html(response.IssuingAuth);
                $(".DateOfCommissioning").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
                //$("#lblfdaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode);
            }
        }
    })
}
function PrintAppStatusData(div) {
    var divContent = document.getElementById(div).innerHTML;

    // List of stylesheets
    var stylesheets = [
        '/fonts/css/all.min.css',
        '/css/nunito.css',
        '/css/roboto.css',
        '/bootstrap/css/bootstrap.min.css',
        '/css/login.css',
        '/sweetalert2/sweetalert2.min.css',
        '/lib/jqueryui/themes/base/jquery-ui.min.css',
        '/css/normalize.min.css',
        '/css/feed.css',
        '/css/main.css',
        '/fonts/css/all.min.css',
        '/fonts/feather/feather.min.js',
        '/fonts/allfont/webfont.min.js',
        '/css/horizontaltimeline.css',
        '/css/appstatus.css',
    ];

    // Create a new window
    var printWindow = window.open('', '', 'height=800,width=1200');

    // Write the HTML structure of the new window
    printWindow.document.write('<html><head><title>Print Content</title>');

    // Loop through each stylesheet and add it to the document
    stylesheets.forEach(function (stylesheet) {
        printWindow.document.write('<link href="' + HostUrl + stylesheet + '" rel="stylesheet" />');
    });

    // Add watermark styles
    printWindow.document.write('<style>.watermark {position: fixed; bottom: 40%;left:0%; transform: rotate(310deg); opacity: 0.4; font-size: 75px; color: #ff0000; z-index: 9999; pointer-events: none; white-space: nowrap;}</style>');

    printWindow.document.write('</head><body>');

    // Get the current date and format it
    var datef2 = new Date();
    var watermarkContent = $(".ipaddress").html() + ' <br> ' + DateFormateddMMyyyyhhmmss(datef2);

    // Add watermark and div content
    printWindow.document.write('<div class="watermark">' + watermarkContent + '</div>');
    printWindow.document.write(divContent);

    // Close the body and HTML tags
    printWindow.document.write('</body></html>');

    // Close the document to complete writing
    printWindow.document.close();

    // Wait for the content to be fully loaded, then trigger the print dialog   
    printWindow.onload = function () {
        printWindow.print();
        printWindow.close();
    };
}
