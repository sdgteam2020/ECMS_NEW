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

$(document).ready(function () {

   
    var path = window.location.href; // because the 'href' property of the DOM element is the absolute path

    
    $("#layouttask .nav-link").each(function () {
       
        if (this.href === path) {
            $(this).addClass("active");
        }
    });

    Getaspntokenarmyno()
    if (window.location.pathname !="/UserProfile/Profile")
        CheckProfileExist();
    $("#btnApplicantsPostingout").click(function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
    });
   
    $("#txtarmynosearchAll").autocomplete({

        source: function (request, response) {
            var TypeId = 1;
            
            var param = { "ICNumber": request.term };
            $("#loading").addClass("d-none");
            $("#armynosearchAllName").html("");
            /*$("#txtarmynosearchAll").val("");*/
            $("#armynosearchAllpic").attr("src", "");

            $.ajax({
                url: '/BasicDetail/SearchAllServiceNo',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);

                    response($.map(data, function (item) {

                        
                        return { label: item.ServiceNo, value: item.BasicDetailId, Name: item.Name, Image: item.Image };

                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            e.preventDefault();
            //alert(i.item.value)
            
            $("#armynosearchAllName").html("Name : " + i.item.Name);
           /* $("#armynosearchAllBasicId").val(i.item.value);*/
            $("#txtarmynosearchAll").val(i.item.label);
            $("#armynosearchAllpic").attr("src", "/WriteReadData/Photo/"+i.item.Image);
            //alert(i.item.value)
            // var param1 = { "UnitMapId": i.item.value };
            //$("#btnIOProfileSerch").addClass('d-none');
           
        },
        appendTo: '#suggesstion-box'
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
    var listItem = "";
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
                if (response.length >0) {
                    //alert("Notofication Sent")
                    var countIo = 0;
                    var list = "";
                    for (var i = 0; i < response.length; i++) {
                        if ($("." + response[i].Spanname).html() == "")
                            $("." + response[i].Spanname).html(0);
                        //if (response[i].DisplayId == 2) {
                        $("." + response[i].Spanname).html(parseInt($("." + response[i].Spanname).html()) + 1);

                        var tot = $("#Totalnotification").html();
                        if (tot == "")
                            tot = 0;

                        $("#Totalnotification").html(parseInt(tot) + parseInt($("." + response[i].Spanname).html()));

                        list += '<div class="border border-1 p-1 mt-2">';
                        list += '<a class="dropdown-item preview-item">';
                        list += '<div class="preview-thumbnail ">';
                        list += '<div class="preview-icon bg-success">';
                        list += '<i class="ti-bell mx-0"></i>';
                        list += '</div>';
                        list += '</div>';
                        list += ' <div class="preview-item-content">';
                        list += '<h6 class="preview-subject font-weight-normal">' + response[i].Message + '</h6>';
                        list += '<p class="font-weight-light small-text mb-0 text-muted">';


                        list += ' </p>';
                        list += '</div>';

                        list += ' </a>';

                        list += '</div>';
                       // }

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

                        //    $("#IOTotal").html(parseInt(SpnIOself) + parseInt(SpnGSOself) + parseInt(SpnIORejectself));

                        //    if ($("#IOTotal").html() == 0)
                        //        $("#IOTotal").html("");

                        //}
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


                        var tot = $("#Totalnotification").html();
                        if (tot == "")
                            tot = 0;

                        $("#Totalnotification").html(parseInt(tot) + parseInt($("." + response[i].Spanname).html()));


                        list += '<div class="border border-1 p-1 mt-2">';
                        list += '<a class="dropdown-item preview-item">';
                        list += '<div class="preview-thumbnail ">';
                        list += '<div class="preview-icon bg-success">';
                        list += '<i class="ti-bell mx-0"></i>';
                        list += '</div>';
                        list += '</div>';
                        list += ' <div class="preview-item-content">';
                        list += '<h6 class="preview-subject font-weight-normal">' + response[i].Message+'</h6>';
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
