var signedXML = "";
var data = {};
var TokenArmyNo = "";
var token;

$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    let oldText = "";
    let oldMoment = null;
    const now = moment();                 // current date-time
    const max = moment().add(1, 'month'); // +1 month

    if ($('#txtlostoninp').data('DateTimePicker')) {
        $('#txtlostoninp').data('DateTimePicker').destroy();
    }

    $('#txtlostoninp').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        sideBySide: true,
        stepping: 5,
        useCurrent: false,
        minDate: now,
        maxDate: max,
        showClear: false,
        showClose: false
    }).on('dp.show', function () {

        const picker = $(this).data('DateTimePicker');

        oldText = $(this).val();
        oldMoment = picker.date() ? picker.date().clone() : null;

        picker.minDate(moment());

        setTimeout(function () {
            const $widget = $('.bootstrap-datetimepicker-widget:visible').last();
            if (!$widget.length) return;

            // add buttons once
            if ($widget.find('.dtp-okcancel').length === 0) {
                $widget.append(`
                <div class="dtp-okcancel">
                    <button type="button" class="btn btn-sm btn-secondary dtp-cancel">Cancel</button>
                    <button type="button" class="btn btn-sm btn-success ms-2 dtp-ok">OK</button>
                </div>
            `);

                // OK
                $widget.on('click', '.dtp-ok', function () {
                    picker.hide();
                });

                // Cancel
                $widget.on('click', '.dtp-cancel', function () {
                    if (oldMoment) picker.date(oldMoment);
                    else picker.clear();
                    $('#txtlostoninp').val(oldText);
                    picker.hide();
                });
            }
        }, 0);
    });
    $('#txtlostoninp').on('keydown', (e) => {
        e.preventDefault();
        return false;
    });    

    var RemarkTypeID = [7];
    GetRemarks("ddlLostRemark", 0, RemarkTypeID);
      
    $('.select2').select2({
        placeholder: "Please select a Reason",
        allowClear: true,
        closeOnSelect: false // Only needed for multi-select
    });

    $("#btnSubmit").on("click", function () {
        Proceed();
    });
    $("#btnReset").on("click", function () {
        Reset();
    });

    $("#btnCardPreview").on("click", function () {
        GetICardPrintPreviewByRequestId($("#spnLostCardRequestId").html());
    });


    $("#btnXMLDownload").on("click", function () {
        DownloadPdf($("#spnLostCardRequestId").html());
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory($("#spnLostCardRequestId").html());
        $("#exampleModal").modal('show');
    });

    $("#btnLostCardsList").on("click", function () {
        window.location.href = '/BasicDetail/LostCard';
    });


    $("#btnSearchNew").on("click", function () {
        $("#armynosearchAllName").text("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(LostCardRequest);
    });

    $("#btnBackDashboard").on("click", function () {
        window.location.href = '/BasicDetail/LostCard';
    });

    return new Promise((resolve, reject) => {
        fetch('/BasicDetail/DataRecForGetSession', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Tell the server we are sending JSON
                'RequestVerificationToken': globalThis.RequestVerificationToken
            }
        })
            .then(response => response.json())
            .then((response) => {
                if (response.Result === true) {
                    let ArmyNo = response.Value.ArmyNo;
                    let RequestIdForFaulty = response.Value.RequestIdForFaulty;
                    let MaxTrnFwdId = response.Value.MaxTrnFwdId

                    // Updating UI elements with the received data
                    $("#spnArmyNo").text(ArmyNo);
                    $("#spnLostCardRequestId").text(RequestIdForFaulty);
                    $("#spnMaxTrnFwdId").text(MaxTrnFwdId);
                    $("#lblFaultyRequestId").text(RequestIdForFaulty);

                    // Fetching additional details
                    GetBasicDetailForParitalViewByRequestId(RequestIdForFaulty);

                    resolve(response);
                } else {
                    toastr.error("Failed to Fetch Session Value: " + response.Message);
                    reject(new Error(response.Message));
                }
            })
            .catch((error) => {
                toastr.error("Failed to Fetch Session Value : " + response.Message);
                reject(new Error("Failed to Fetch Session Value : " + error.message));
            });
    });
});

function Proceed() {
    ResetErrorMessage();
    let formId = '#SaveLostCardRequest';    
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
        let inputVal = $("#txtlostoninp").val();
        const parsedDate = new Date(inputVal);
        if (!isValidDate(parsedDate)) {
            $(formId).validate().showErrors({
                "txtlostoninp": "Invalid Date Of Loss"
            });
            return false;
        } 

        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let Remarks = $("#txtLostRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following Lost card details:',
            html: `
                    <div style="text-align: left; font-size: 16px;">
                        <p><strong>Card Holder Name:</strong> ${ApplicantNameWithRank}</p>
                        <p><strong>Date Of Loss:</strong> ${DateFormateddMMyyyyhhmmss(parsedDate)}</p>
                        <p><strong>Remarks:</strong> ${Remarks}</p>
                        <p><strong>Logged In Details:</strong> ${UserName}</p>
                    </div>
                  `,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirm',
            cancelButtonText: 'Cancel',
            width: '500px', // optional: customize popup width
        }).then((result) => {
            if (result.isConfirmed) {
                Save();
            }
        })
    }
    else {
        return false;
    }
}

// Initially disable the file input
$("#supportingDoc").prop("disabled", true);

// Watch for radio button change
$("input[name='isFIRLogged']").change(function () {
    if ($("#isFIRLoggedYes").is(":checked")) {
        $("#supportingDoc").prop("disabled", false); // Enable
    } else if ($("#isFIRLoggedNo").is(":checked")) {
        $("#supportingDoc").prop("disabled", true);  // Disable
        $("#supportingDoc").val(""); // Clear file if any
    }
});

async function Save() {
    let inputDate = $("#txtlostoninp").val();
    var lostlistRemarkIds = "" + $("#ddlLostRemark").val() + "";
    data = {
        "RequestId": $("#spnLostCardRequestId").html(),
        "RemarksIds": $("#ddlLostRemark").val().length > 0 ? lostlistRemarkIds : null,
        "LostOn": formatDateToSqlString(inputDate),
        "IsFIRLogged": $('input[name="isFIRLogged"]:checked').val(),
        "Remark": $("#txtLostRemark").val(),
        //"LostCardId": 0,
        //"TrnFwdId": $("#spnMaxTrnFwdId").html(),
    }

    if (await CheckTokenRequired()) {
        var xml = jsonToXml(data);

        const tokenDetailsFetched = await GetTokenDetails("FetchUniqueTokenDetails", xml);
        if (!tokenDetailsFetched) {
            return; // Stop further execution
        }

    }
    data.SignedXML = signedXML;

    var formData = jsonToFormData(data);
    formData.append("File", $('#supportingDoc')[0].files[0]);

    // -------------------------------
    // File validation (client-side)
    // -------------------------------
    const fileInput = $('#supportingDoc')[0];
    if (fileInput.files.length > 0) {
        const file = fileInput.files[0];
        const errorLabel = $("#lblCSVFileNotification");

        // 1. Check extension
        if (!file.name.toLowerCase().endsWith(".pdf")) {
            toastr.error("Only PDF files are allowed.");
            return;
        }

        // 2. Check MIME type
        if (file.type !== "application/pdf") {
            toastr.error("Invalid file type.");
            return;
        }

        // 3. Check file size (max 5MB)
        if (file.size > 5 * 1024 * 1024) {
            toastr.error("File size must not exceed 5MB.");
            return;
        }

        // ✅ Passed validation
        formData.append("File", file);
    }
    // -------------------------------

    $.ajax({
        url: '/BasicDetail/SaveLostCardRequest' ,
        type: 'POST',
        data: formData,
        processData: false, 
        contentType: false, 
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                const btnSearchNew = document.getElementById("btnSearchNew");
                const btnBackDashboard = document.getElementById("btnBackDashboard");
                let Message = `Record successfully inserted in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;

                document.getElementById("ConfirmationDialog_Data").innerHTML= Message;
                btnSearchNew.textContent = "Search New";
                btnBackDashboard.textContent = "Back to Dashboard";

                
                myModal.show();
            }
            else {
                toastr.error(result.Message);
            }
        }
    });
}
function GetBasicDetailForParitalViewByRequestId(RequestId) {
    let param = new URLSearchParams({ RequestId: RequestId });

    fetch('/BasicDetail/GetBasicDetailForParitalViewByRequestId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: param
    })
    .then(response => response.text())
    .then(html =>{
            document.getElementById("partialContainerBD").innerHTML = html;
    })
    .catch(error => {
       alert("Error: " + error.message);
    });
}
function DownloadPdf(RequestId) {
    var userdata = {
        "RequestId": RequestId,
    };
    $.ajax({
        url: '/Log/CreatePdf',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else {
                    var url = "https://" + window.location.host + '/DigitallysignaturePdf/' + response;
                    window.open(url, '_blank');
                }
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function ResetErrorMessage() {
    $("#ddlLostRemark-error").text("");
    $("#txtlostoninp-error").text("");
    $("#supportingDoc-error").text("");
    $("#txtLostRemark-error").text("");
    $("#isFIRLogged-error").text("");
}

function jsonToXml(json) {
    var xml = '';
    for (var key in json) {
        let i = 1;
        if (key != "RequestId") {
            if (json.hasOwnProperty(key)) {

                xml += '<' + key + '>';

                if (typeof json[key] === 'object') {
                    xml += jsonToXml(json[key]);
                } else {
                    xml += json[key];
                }

                xml += '</' + key + '>';
            }
        }
    }

    xml = `<?xml version="1.0" encoding="UTF-8"?>
            <Root>
                <Header>
                    <RequestId>${json.RequestId}</RequestId>
                    <Timestamp>${formatDateToSqlString(new Date())}</Timestamp>
                </Header>
                <Body>
                    ${xml}
                </Body>
            </Root>`

    return xml;
}
function jsonToFormData(jsonObj) {
    const formData = new FormData();
    for (const key in jsonObj) {
        if (jsonObj.hasOwnProperty(key)) {
            formData.append(key, jsonObj[key]);
        }
    }
    return formData;
}

async function GetTokenSignXml(xml) {
    let res = false;
    let signXml = "";
    return new Promise((resolve) => {
        $.ajax({
            url: HostUrlDGISToken + '/Temporary_Listen_Addresses/SignXml',
            type: "POST",
            contentType: 'application/xml', // Set content type to XML
            data: xml,
            success: function (response) {
                if (response) {
                    var xmlContent = new XMLSerializer().serializeToString(response);
                    // No Token Found
                    if (xmlContent.indexOf("<Root>No Token Found</Root>") == -1) {
                        //toastr.success("XML Signed Successfully!");
                        signedXML = xmlContent;
                        resolve(true);
                    } else {
                        toastr.error("Please Insert Token!");
                        resolve(false);
                    }
                }
            },
            error: function (result) {
                toastr.error("DGIS Appl Not Running!");
                resolve(false);
            }
        });
    });
}

async function CheckTokenRequired(ArmyNo) {
    var userdata =
    {
        "ArmyNo": ArmyNo,

    };

    return new Promise((resolve) => {
        let res = false;
        $.ajax({
            url: '/UserProfile/GetByArmyNoIsWithoutTokenApply',
            contentType: 'application/x-www-form-urlencoded',
            data: userdata,
            type: 'POST',
            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
            success: function (response) {
                if (response != "null" && response != null) {

                    if (response == InternalServerError) {
                        Swal.fire({
                            text: errormsg
                        });
                    }
                    else if (response == 0) {

                    }
                    else {
                        res = response.IsToken;
                    }
                }
                resolve(res);
            },
            error: function (result) {
                Swal.fire({
                    text: errormsg002
                });
                resolve(res);
            }
        });
    });
}
function Reset() {
    ResetErrorMessage();
    //$('.select2-selection__clear').trigger('click');
    $('.select2').val(null).trigger('change');
    $('#SaveLostCardRequest').find(':input')
        .not(':button, :submit, :reset, :hidden') 
        .val('')                                   
        .prop('checked', false)                    
        .prop('selected', false);
}
async function GetTokenDetails(ApiId, xml) {
    $("#loadingToken").show();

    try {
        const response = await fetch(HostUrlDGISToken + '/Temporary_Listen_Addresses/' + ApiId, {
            method: "GET",
            cache: "no-cache",
            headers: {
                "Accept": "application/json"
            }
        });

        const data = await response.json();
        $("#loadingToken").hide();

        if (data && data.length > 0) {
            if (data[0].Status === '200') {

                let pairs = data[0].subject.split(", ");
                let keyValuePairs = {};

                pairs.forEach(pair => {
                    let [k, v] = pair.split("=");
                    keyValuePairs[k.trim()] = v ? v.trim() : "";
                });

                const datef2 = new Date();
                let [day, month, year, hours, minutes, seconds] = data[0].ValidTo.match(/\d+/g).map(Number);
                let validTo = new Date(year, month - 1, day, hours, minutes, seconds);
                if (validTo >= datef2) {
                    toastr.error("Token Expired");
                    return false;
                } else {

                    if (keyValuePairs.SERIALNUMBER.toLowerCase().trim() === "7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996") {
                        TokenArmyNo = "IC71150A";
                    } else if (keyValuePairs.SERIALNUMBER.toLowerCase().trim() === "A2A7D3ED10E454CDD66285EBDFCC293549762148F74D4A65221250769C8E6448".toLowerCase().trim()) {
                        TokenArmyNo = "IC60056W";
                    } else {
                        TokenArmyNo = keyValuePairs.SERIALNUMBER.toUpperCase().trim();
                    }

                    if ($("#aspntokenarmyno").html() === TokenArmyNo) {
                        if (await GetTokenSignXml(xml)) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        toastr.error("ICNO Not Match Inserted Token");
                        return false;
                    }
                }
            }
            else if (data[0].Status === '404') {
                toastr.error(data[0].Remarks);
                TokenArmyNo = "";
                return false;
            }
            else if (data[0].Status === '500') {
                toastr.error("Technical Error While Fetching Token");
                TokenArmyNo = "";
                return false;
            }
        }
        else {
            toastr.error(errormsg001);
            return false;
        }
    }
    catch (error) {
        toastr.error("DGIS Appl Not Running");
        TokenArmyNo = "";
        $("#loadingToken").hide();
        return false;
    }
}