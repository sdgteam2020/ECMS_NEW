var signedXML = "";
var data = {};
var token;

$(async function () {
    token = $('input[name="__RequestVerificationToken"]').val();
    var RemarkTypeID = [7];
    GetRemarks("ddlLostRemark", 0, RemarkTypeID);
      
    $('.select2').select2({
        placeholder: "Please select a Reason",
        allowClear: true,
        closeOnSelect: false // Only needed for multi-select
    });

    
    if (sessionStorage.getItem("ArmyNo") != null && sessionStorage.getItem("RequestIdForFaulty") != null && sessionStorage.getItem("MaxTrnFwdId") != null) {
        var encryptedArmyNo = sessionStorage.getItem("ArmyNo");
        var encryptedRequestId = sessionStorage.getItem("RequestIdForFaulty");
        var encryptedMaxTrnFwdId = sessionStorage.getItem("MaxTrnFwdId");

        var secretKey = document.getElementById("spnUniqueSecretKey").innerText;

        var bytes = CryptoJS.AES.decrypt(encryptedArmyNo, secretKey);
        var decryptedArmyNo = bytes.toString(CryptoJS.enc.Utf8);

        var bytes = CryptoJS.AES.decrypt(encryptedRequestId, secretKey);
        var decryptedRequestId = bytes.toString(CryptoJS.enc.Utf8);

        var bytes = CryptoJS.AES.decrypt(encryptedMaxTrnFwdId, secretKey);
        var decryptedMaxTrnFwdId = bytes.toString(CryptoJS.enc.Utf8);


        $("#spnArmyNo").text(decryptedArmyNo);
        $("#spnLostCardRequestId").text(decryptedRequestId);
        $("#spnMaxTrnFwdId").text(decryptedMaxTrnFwdId);
        $("#lblFaultyRequestId").text(decryptedRequestId);

        GetBasicDetailForParitalViewByRequestId(decryptedRequestId);
    }

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
        if (!await GetTokenSignXml(xml)) {
            return false;
        }
    }
    data.SignedXML = signedXML;

    var formData = jsonToFormData(data);
    formData.append("File", $('#supportingDoc')[0].files[0]);

    $.ajax({
        url: '/BasicDetail/SaveLostCardRequest' ,
        type: 'POST',
        data: formData,
        processData: false, 
        contentType: false, 
        headers: {
            'RequestVerificationToken': token
        },
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
            'Content-Type': 'application/x-www-form-urlencoded'
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
        headers: {
            'RequestVerificationToken': token 
        },
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
        i = 1;
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
            headers: {
                'RequestVerificationToken': token
            },
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
