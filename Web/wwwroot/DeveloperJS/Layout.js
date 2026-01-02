// Set the options that I want
var RequestVerificationToken;
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
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    const myHeadersIncrement = new Headers();
    myHeadersIncrement.append("X-API-KEY", 'b03aae18-4c63-44e4-93a3-eba447270157');

    const requestIncrement = {
        method: "POST",
        redirect: "follow",
        headers: myHeadersIncrement
    };

    fetch("https://hitcounter.army.mil/api/ApplicationHit/IncrementHits", requestIncrement)
        .then((response) => response.text())
        .then((result) => console.log(result))
        .catch((error) => console.error(error));

    const myHeadersIncrementStart = new Headers();
    myHeadersIncrementStart.append("X-API-KEY", 'b03aae18-4c63-44e4-93a3-eba447270157');
    const requestStart = {
        method: "POST",
        redirect: "follow",
        headers: myHeadersIncrementStart
    };

    fetch("https://hitcounter.army.mil/api/Application/ApplicationSessionStart", requestStart)
        .then((response) => response.text())
        .then((result) => console.log(result))
        .catch((error) => console.error(error));



    $('img[data-fallback]').on('error', function () {
        const fallback = $(this).data('fallback');
        if (this.src !== fallback) {
            this.src = fallback;
        }
    });
    $(window).on('resize', function () {
        if ($.fn.DataTable.isDataTable('#tbldata')) {
            $('#tbldata').DataTable().columns.adjust();
        }
    });

    $('[data-toggle="tooltip"]').tooltip();

    $("#loadingToken").hide();
    $("#loading").hide();

    // 1) Global loader hooks (for jQuery + fetch)
    $(document).ajaxStart(function () {
        $("#loading").show();
    }).ajaxStop(function () {
        $("#loading").hide();
    });

    // 2) Wrap fetch so it fires ajaxStart/ajaxStop
    (function () {
        if (!window.fetch) {
            return;
        }

        const originalFetch = window.fetch.bind(window);
        let activeFetches = 0;

        window.fetch = function (...args) {
            if (activeFetches === 0) {
                $(document).trigger('ajaxStart');
            }
            activeFetches++;

            return originalFetch(...args)
                .then(response => {
                    activeFetches--;
                    if (activeFetches === 0) {
                        $(document).trigger('ajaxStop');
                    }
                    return response;
                })
                .catch(error => {
                    activeFetches--;
                    if (activeFetches === 0) {
                        $(document).trigger('ajaxStop');
                    }
                    throw error;
                });
        };
    })();

    // ====== Date / Time pickers ======
        
    $('.datepicker').datetimepicker({
        format: "L"
    });
    $('.timepicker').datetimepicker({
        format: "LT"
    });
    $('.datetimepicker').datetimepicker({
        sideBySide: true
    });

    $('.datepickerpast').datetimepicker({
        format: "L",
        maxDate: new Date()
    });


    $('.datetimepickerpast').datetimepicker({
        sideBySide: true,
        format: 'YYYY-MM-DD HH:mm',
        maxDate: new Date()
    }).on('dp.change', function (e) {
        // Set the formatted value (optional: adjust format)
        $(this).val(e.date.format('YYYY-MM-DD HH:mm'));

        // Trigger input for floating label (if CSS relies on :placeholder-shown)
        $(this).trigger('input');
    });

    // ====== Misc bindings ======
    $('.PersInfo').on("click", function () {
        sessionStorage.persid = null
    })

    // Fallback image for all <img>
    $("img").on('error', function () {
       
        $(this).attr("src", "/Images/user4.png");
    });

    // Highlight active menu item
    var path = window.location.href; // because the 'href' property of the DOM element is the absolute path
    $("#layouttask .nav-link").each(function () {
       
        if (this.href === path) {
            $(this).addClass("active");
        }
    });

    //Getaspntokenarmyno()

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
                        'RequestVerificationToken': globalThis.RequestVerificationToken
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
        appendTo: '#unitoffrsModal #suggesstion-box',
        position: { my: "left top", at: "left bottom", collision: "fit" },
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
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': globalThis.RequestVerificationToken
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
        open: function () {
            // make dropdown width = input width
            $(".ui-autocomplete:visible").outerWidth($(this).outerWidth());
        }
    });

    GetNotification();

    //javascript-obfuscator:disable
    fetch('/Home/VisitorStats', {
        method: 'POST',
        redirect: 'manual',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': globalThis.RequestVerificationToken
            // NOTE: No need for Content-Type or body if action koi data expect nahi kar raha
        },
        body: '{}'  // empty object so ASP.NET accepts the POST
    })
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
    //javascript-obfuscator:enable
});


function Getaspntokenarmyno() {
    $.ajax({
        url: '/ConfigUser/GetTokenArmyNo',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

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
function SaveNotification(StepId, DisplayId, ReciverAspNetUsersId, RequestIds) {
    var userdata =
    {
        "StepId": StepId,
        "Read": false,
        "DisplayId": DisplayId,
        "ReciverAspNetUsersId": ReciverAspNetUsersId,
        "Url": "",
        "RequestIds": RequestIds
    };
    $.ajax({
        url: '/Home/SaveNotification',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

        success: function (response) {
            if (response == 0) {
                toastr.error("Notofication failed");
            }
        }
    });
}

function GetNotification() {
    $.ajax({
        url: '/Home/GetNotification',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

        success: function (response) {

            // Clear and handle empty/null safely
            $(".preview-list").html("");

            if (response && response.TotalCount > 0) {

                if (response.TotalCount > 99) {
                    $("#Totalnotification").html("99+");
                }
                else {
                    $("#Totalnotification").html(response.TotalCount);
                }

                // Header

                var list = `<h6 class="dropdown-header text-muted fw-bold">Pending Action</h6>`;

                // Items

                for (var i = 0; i < response.Items.length; i++) {
                    if ($("." + response.Items[i].Spanname).html() == "")
                        $("." + response.Items[i].Spanname).html(0);

                    $("." + response.Items[i].Spanname).html(parseInt($("." + response.Items[i].Spanname).html()) + 1);

                    list += `<div class="border border-1 p-1 mt-2">
                                    <a class="dropdown-item preview-item" href="${response.Items[i].Url}">
                                        <div class="preview-thumbnail">
                                            <div class="preview-icon p-2">
                                                <i class="ti-bell1 mx-0"></i>
                                                <img id="notificationimg" src="${response.Items[i].ExistingPhotoInBase64}" alt="profile" width="65px">
                                            </div>
                                        </div>
                                        <div class="preview-item-content">
                                            <h6 class="preview-subject font-weight-normal">
                                                Appl No: ${response.Items[i].ApplId} <br>
                                                Applicant Name:-${response.Items[i].RankAbbreviation}  ${response.Items[i].LName != null ? response.Items[i].FName + ' ' + response.Items[i].LName : response.Items[i].FName} (${response.Items[i].ServiceNo}) <br>
                                                ${response.Items[i].Message} (${response.Items[i].DomainId})</h6>
                                            <p class="font-weight-light small-text mb-0 text-muted">
                                            </p>
                                        </div>
                                    </a>
                                </div>
                                `;
                }
                // "View more" footer only if server says more exist than shown
                var remaining = (response.TotalCount || 0) - ((response.Items || []).length);
                if (remaining > 0) {
                    list += `
                    <div class="dropdown-footer text-uppercase text-center p-2">
                        <a href="/Home/Notification">View ${remaining} more notification${remaining > 1 ? 's' : ''}</a>
                    </div>`;
                }
                $(".preview-list").append(list);

            }
            else {
                $("#Totalnotification").html("0");
                $(".preview-list").html(`
                        <h6 class="dropdown-header text-uppercase text-muted fw-bold">Pending Action</h6>
                        <div class="text-center p-2 small text-muted">No new notifications</div>
                    `);
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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

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
function encryptData(plainText) {
    const secretKey = spnUniqueKey;
    if (!secretKey) return "";

    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.substring(0, 16)); // 16 bytes

    const encrypted = CryptoJS.AES.encrypt(plainText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    return encrypted.toString();   // Base64 output
}
function decryptData(cipherText) {

    if (!cipherText) return "";

    const secretKey = spnUniqueKey;
    if (!secretKey) return "";

    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.substring(0, 16));

    // fix if spaces replaced +
    cipherText = cipherText.replace(/ /g, "+");

    const decrypted = CryptoJS.AES.decrypt(cipherText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    const result = decrypted.toString(CryptoJS.enc.Utf8);
    return result;
}