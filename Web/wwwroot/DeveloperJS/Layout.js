// Set the options that I want
toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

$(function () {
   
    $("img").on('error', function () {
       
        $(this).attr("src", "/Images/user4.png");
    });
    var path = window.location.href; // because the 'href' property of the DOM element is the absolute path

    
    $("#layouttask .nav-link").each(function () {
       
        if (this.href === path) {
            $(this).addClass("active");
        }
    });

    Getaspntokenarmyno()
    if (window.location.pathname !="/UserProfile/Profile")
        CheckProfileExist();

    $("#btnSercharmynoSmart").on("click", async function () {
        if ($("#armynosearchAllName").html() != "") {

            $("#unitoffrsModal").modal("hide");

            try {
                const requestData = {
                    ArmyNo: $("#txtarmynosearchAll").val(),
                    RequestIdForFaulty: $("#RequestId_unitoffrsModal").val(),
                    MaxTrnFwdId: $("#MaxTrnFwdId_unitoffrsModal").val()
                };

                // Send POST request using fetch
                const response = await fetch('/BasicDetail/DataSendForSetSession', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json', // Tell the server we are sending JSON
                    },
                    body: JSON.stringify(requestData), // Convert the request data to JSON
                });

                // Parse the response as JSON
                const data = await response.json();

                if (data === true) {
                    // Redirect based on the value of #armynosearchTypeId
                    let TypeId = Number($("#armynosearchTypeId").val());
                    switch (TypeId) {
                        case ApplicantPostingOut:
                            window.location.href = "/Posting/PostingIn";
                            break;
                        case ApplicantClose:
                            window.location.href = "/Posting/ApplicationClose";
                            break;
                        case FaultyCardRequest:
                            window.location.href = "/BasicDetail/FaultyCardRequest";
                            break;
                        case HoltlistCardRequest:
                            window.location.href = "/BasicDetail/HotListCardRequest";
                            break;
                        case LostCardRequest:
                            window.location.href = "/BasicDetail/LostCardRequest";
                            break;
                        case DistributeCardRequest:
                            window.location.href = "/BasicDetail/DistributeCardRequest";
                            break;
                        case DestructionCardRequest:
                            window.location.href = "/BasicDetail/DestructionCardRequest";
                            break;
                        default:
                            break;
                    }
                } else {
                    toastr.error("Failed to Create Session: " + data.Message);
                }
            } catch (error) {
                // Catch any errors during the fetch and display the error
                toastr.error("Failed to Create Session: " + error.message);
            }
        } else {
            toastr.error("Please Enter Army No");
        }
    });

    $("#txtarmynosearchAll").autocomplete({
        source: function (request, response) {
            if (request.term.length > 1) {
                let param = new URLSearchParams(
                    {
                        ArmyNo: request.term,
                        TypeId: $("#armynosearchTypeId").val()
                    });
                $("#loading").addClass("d-none");
                $("#armynosearchAllName").html("");
                $("#armynosearchAllpic").attr("src", "");

                fetch('/BasicDetail/SearchAllServiceNo', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: param
                })
                    .then(res => {
                        if (!res.ok) {
                            throw new Error(`HTTP error! Status: ${res.status}`);
                        }
                        return res.json();
                    })
                    .then(data => {
                        
                        if (data.length !== 0) {
                            response(data.map(item => ({
                                label: `Service No - ${item.ServiceNo} Card Serial No - ${item.CardSerialNo != null ? item.CardSerialNo : ""} Chip No - ${item.ChipNo != null ? item.ChipNo : ""} `,
                                value: item.BasicDetailId,
                                ServiceNo: item.ServiceNo,
                                Name: item.FName + (item.LName ? item.LName : ""),
                                Image: item.Image,
                                RequestId: item.RequestId,
                                MaxTrnFwdId: item.MaxTrnFwdId ?? 0
                            })));
                        } else {
                            $("#armynosearchAllName").html("");
                            $("#txtarmynosearchAll").val("");
                            $("#RequestId_unitoffrsModal").val("");
                            $("#MaxTrnFwdId_unitoffrsModal").val("");
                            $("#armynosearchAllpic").attr("src", "");
                            //toastr.error("");
                            Swal.fire({
                                title: "OOPs!",
                                text: "Army no. not found.",
                                icon: "error",
                                confirmButtonText: "Ok"
                            });
                        }
                    })
                    .catch(error => {
                        console.error('Request failed', error);
                        alert("Error: " + error.message);
                    });
            }
        },
        select: function (e, i) {
            e.preventDefault();

            $("#armynosearchAllName").html("Name : " + i.item.Name);
            $("#txtarmynosearchAll").val(i.item.ServiceNo);
            $("#armynosearchAllpic").attr("src", i.item.Image);
            $("#RequestId_unitoffrsModal").val(i.item.RequestId);
            $("#MaxTrnFwdId_unitoffrsModal").val(i.item.MaxTrnFwdId);
        },
        appendTo: '#suggesstion-box'
    });

    GetNotification(1, 1);

    fetch('/Home/VisitorStats?_=' + new Date().getTime()) // cache busting
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // parse JSON
        })
        .then(data => {
            if (data) {
                $('#today').text("Visitors Today : " + (data.Today ?? 0));
                $('#week').text("Week : " + (data.Week ?? 0));
                $('#month').text((data.MonthName ?? "Month") + " : " + (data.Month ?? 0));
                $('#total').text("Total : " + (data.Total ?? 0));
                $('#monthName').text(data.MonthName ?? "");
            } else {
                console.warn('No visitor stats data received.');
            }
        })
        .catch(error => {
            console.error('Error fetching visitor stats:', error);
        });

});

function CheckProfileExist() {
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/ConfigUser/CheckProfileExist',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response.UserId == 0 || response.UserId == null) {
                    alert('Please Add Profile First !');
                    window.location = "/UserProfile/Profile";
                }

            } else {
                alert('Please Add Profile First !');
                window.location = "/UserProfile/Profile";
            }
        }
    });
}
function Getaspntokenarmyno() {
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
                    $("#IpaddresGloble").html(response.IpAddress)
                    $("#aspntokenarmyno").html(response.ICNO)
                    $("#aspndomainUnitID").html(response.UnitId)
                    $("#ProfileName").html(response.Name)
                    $("#ProfileRankName").html(response.RankName)

                    $("#waterarmyno").html(response.ICNO)
                    $("#waterrank").html(response.RankName)
                    $("#waterProfileName").html(response.Name)
                    $("#waterIpaddresGloble").html(response.IpAddress)

                    $("#waterarmyno1").html(response.ICNO)
                    $("#waterrank1").html(response.RankName)
                    $("#waterProfileName1").html(response.Name)
                    $("#waterIpaddresGloble1").html(response.IpAddress)
                }
            }
        }
    });
}
function SaveNotification(NotificationTypeId, DisplayId, ReciverAspNetUsersId, RequestId) {
    var listItem = "";
    var userdata =
    {
        "NotificationTypeId": NotificationTypeId,
        "Read": false,
        "DisplayId": DisplayId,
        "ReciverAspNetUsersId": ReciverAspNetUsersId,
        "Url": "",
        "RequestId": RequestId
    };
    $.ajax({
        url: '/Home/SaveNotification',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == 1) {
                    //alert("Notofication Sent")
                }

            } else {
               
            }
        }
    });
}

function GetNotification(NotificationTypeId, ApplyForId) {
    var userdata =
    {
        "TypeId": NotificationTypeId,
        "applyForId": ApplyForId
    };
    $.ajax({
        url: '/Home/GetNotification',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response.length > 0) {
                    $("#Totalnotification").html(response.length);
                    var list = "";
                    for (var i = 0; i < response.length; i++) {
                        if ($("." + response[i].Spanname).html() == "")
                            $("." + response[i].Spanname).html(0);

                        $("." + response[i].Spanname).html(parseInt($("." + response[i].Spanname).html()) + 1);

                        list += `<div class="border border-1 p-1 mt-2">
                                    <a class="dropdown-item preview-item" href="${response[i].Url}">
                                        <div class="preview-thumbnail">
                                            <div class="preview-icon p-2">
                                                <i class="ti-bell1 mx-0"></i>
                                                <img id="notificationimg" src="${response[i].ExistingPhotoInBase64}" alt="profile" width="65px">
                                            </div>
                                        </div>
                                        <div class="preview-item-content">
                                            <h6 class="preview-subject font-weight-normal"> Appl No: ${response[i].ApplId} <br> Applicant Name:-${response[i].RankAbbreviation}  ${response[i].LName != null ? response[i].FName + ' ' + response[i].LName : response[i].FName} (${response[i].ServiceNo}) <br> ${response[i].Message}</h6>
                                            <p class="font-weight-light small-text mb-0 text-muted">
                                            </p>
                                        </div>
                                    </a>
                                </div>
                                `;
                    }
                    $(".preview-list").append(list);
                }

            } else {

            }
        }
    });
}

function GetNotificationRequestId(NotificationTypeId,ApplyForId) {
    var listItem = ""; 
    var tot = "0";
    var userdata =
    {
        "TypeId": NotificationTypeId,
        "applyForId": ApplyForId,

    };
    $.ajax({
        url: '/Home/GetNotificationRequestId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response.length > 0) {
                    //alert("Notofication Sent")
                    var countIo = 0;
                    var list = "";
                    for (var i = 0; i < response.length; i++) {
                        if ($("." + response[i].Spanname).html() == "")
                            $("." + response[i].Spanname).html(0);
                       
                        $("." + response[i].Spanname).html(parseInt($("." + response[i].Spanname).html()) + 1);


                       // var tot = $("#Totalnotification").html();
                        if (tot == "")
                            tot = 0;

                       // $("#Totalnotification").html(parseInt(tot) + parseInt($("." + response[i].Spanname).html()));
                        $("#Totalnotification").html(parseInt($("#Totalnotification").html()) + 1);

                        list += '<div class="border border-1 p-1 mt-2">';
                        list += '<a class="dropdown-item preview-item" href="' + response[i].Url + '">';
                        list += '<div class="preview-thumbnail ">';
                        list += '<div class="preview-icon p-2">';
                        list += '<i class="ti-bell1 mx-0"></i>';
                        list += '<img id="notificationimg" src="' + response[i].ExistingPhotoInBase64 + '" alt="profile" width="65px">';
                        list += '</div>';
                        list += '</div>';
                        list += ' <div class="preview-item-content">';
                        list += '<h6 class="preview-subject font-weight-normal"> Appl Id: ' + response[i].ApplId + '<br> Applicant Name:-' + response[i].RankAbbreviation + ' ' + response[i].LName != null ? response[i].FName + ' ' + response[i].LName : response[i].FName + ' (' + response[i].ServiceNo +') <br>' + response[i].Message+'</h6>';
                        list += '<p class="font-weight-light small-text mb-0 text-muted">';

                        
                        list += ' </p>';
                        list += '</div>';

                        list += ' </a>';
                        
                        list += '</div>';
                        //if (response[i].DisplayId == 2 || response[i].DisplayId == 3 || response[i].DisplayId == 7) {
                        //    var SpnIOself = 0;
                        //    var SpnGSOself = 0;
                        //    var SpnIORejectself = 0;
                        //    if ($(".SpnIOself").html() == "")
                        //        SpnIOself = 0;
                        //    else
                        //        SpnIOself = $(".SpnIOself").html();
                        //    if ($(".SpnGSOself").html() == "")
                        //        SpnGSOself = 0;
                        //    else
                        //        SpnGSOself = $(".SpnGSOself").html();
                        //    if ($(".SpnIORejectself").html() == "")
                        //        SpnIORejectself = 0;
                        //    else
                        //        SpnIORejectself = $(".SpnIORejectself").html();

                        //    //$("#IOTotal").html(parseInt(SpnIOself) + parseInt(SpnGSOself) + parseInt(SpnIORejectself));

                        //    //if ($("#IOTotal").html() == 0)
                        //    //    $("#IOTotal").html("");

                        //}
                    }
                    $(".preview-list").append(list);
                }

            } else {

            }
        }
    });
}

function UpdateNotification(DisplayId) {
    var listItem = "";
    var userdata =
    {
        "DisplayId": DisplayId,

    };
    $.ajax({
        url: '/Home/UpdateNotification',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response.length > 0) {
                    //alert("Notofication Sent")
                  
                }

            } else {

            }
        }
    });
}

function isValidDate(d) {
    return d instanceof Date && !isNaN(d);
}
function formatDateToSqlString(inputDate) {
    if (inputDate == '') {
        return null;
    }
    let date = new Date(inputDate);
    const pad = (num, size = 2) => String(num).padStart(size, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ` +
        `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}.` +
        `${pad(date.getMilliseconds(), 3)}`;
}
