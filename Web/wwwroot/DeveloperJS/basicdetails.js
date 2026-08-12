var skey = "";

let photoOriginalFiles = [];
let signatureOriginalFiles = [];
let pendingPhotoDataUrl = "";
let pendingSignatureDataUrl = "";
$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    skey = $('#spnhdns').html();

    $("#SaveForm").on("submit", async function (e) {
        e.preventDefault();

        const isValid = await CheckValidation();
        if (isValid) {
            this.submit();
        }
    });

    initializeMediaUploadModals();
    if ($("#spnBloodGroupId").val() > 0) {
        mMsater($("#spnBloodGroupId").val(), "BloodGroupId", BloodGroup, "");
    }
    else {
        mMsater("", "BloodGroupId", BloodGroup, "");
    }

    if ($("#spnArmedId").val() > 0) {
        mMsater($("#spnArmedId").val(), "ArmedId", ArmyType, "");
        if ($("#spnRegimentalId").val() == "null" && $("#spnRegimentalId").val() == null) {
            mMsater("", "RegimentalId", Regimental, $("#spnArmedId").val());
        }
        else {
            mMsater($("#spnRegimentalId").val(), "RegimentalId", Regimental, $("#spnArmedId").val());
        }

    }
    else {
        mMsater("", "ArmedId", ArmyType, "");
    }
    const [today] = new Date().toISOString().split('T');
    const maxDate = new Date();
    maxDate.setDate(maxDate.getDate() + 30);
    const [maxDateFormatted] = maxDate.toISOString().split('T');
    //const dateInput = document.getElementById('DateOfIssue');
    //dateInput.setAttribute('min', today);
    //dateInput.setAttribute('max', maxDateFormatted);
    document.getElementById('DateOfIssue').value = today;

    $("#ArmedId").on("change", function () {
        //GetRegimentalListByArmedId(this.value, "");
        mMsater("", "RegimentalId", Regimental, this.value);
        GetROListByArmedId(this.value, "");
    });

    $("#TermsConditions").on("click", function () {

        if ($("#TermsConditions").prop("checked") == true) {
            $("#btnsave").removeClass("disabled");
        }
        else {
            $("#btnsave").addClass("disabled");
        }
    });
    $('#Height').on('keyup', function () {

        if ($('#Height').val() > 250) {
            toastr.error('Please enter a value less than or equal to 250. ');
        }
    });


    const regId = $("#RegistrationId").val();

    if ([1, 2, 4, 6, 9, 10].includes(parseInt(regId))) {
        GetUnit();
        $('#txtUnit').prop('readonly', true);
    } else {
        $('#txtUnit').prop('readonly', false);
    }

    getApplyIcardDetails();

    if ($("#ApplyForId").val() == 1) {
        $(".OptionsRegimental").addClass("d-none");
        mMsater($("#spnrankid").val(), "RankId", Rank, "");
        if ($("#spnIssuingAuthorityId").val() > 0) {
            mMsater($("#spnIssuingAuthorityId").val(), "IssuingAuthorityId", IssuingAuthority, $("#ApplyForId").val());
        }
        else {
            mMsater("", "IssuingAuthorityId", IssuingAuthority, $("#ApplyForId").val());
        }

    } else if ($("#ApplyForId").val() == 2) {
        mMsater($("#spnrankid").val(), "RankId", RankJCo, "");
        $(".OptionsRegimental").removeClass("d-none");
        if ($("#spnIssuingAuthorityId").val() > 0) {
            mMsater($("#spnIssuingAuthorityId").val(), "IssuingAuthorityId", IssuingAuthority, $("#ApplyForId").val());
        }
        else {
            mMsater("", "IssuingAuthorityId", IssuingAuthority, $("#ApplyForId").val());
        }
    }

    if (sessionStorage.getItem("ArmyNo") != null) {
        const encryptedArmyNo = sessionStorage.getItem("ArmyNo");
        const encryptedOffType = sessionStorage.getItem("OffType");
        const encryptedlCardType = sessionStorage.getItem("lCardType");

        const decryptedArmyNo = decryptData(encryptedArmyNo, skey);
        const decryptedOffType = decryptData(encryptedOffType, skey);
        const decryptedlCardType = decryptData(encryptedlCardType, skey);



        $("#ServiceNumber").val(decryptedArmyNo);
        $("#icarddetails").html('I-Card Appl Request For  (' + decryptedArmyNo + ')');
        if (decryptedOffType === "1") {
            $(".OptionsRegimental").addClass("d-none");
            mMsater($("#spnrankid").val(), "RankId", Rank, "");
        }
        else if (decryptedOffType === "2") {
            mMsater($("#spnrankid").val(), "RankId", RankJCo, "");
            $(".OptionsRegimental").removeClass("d-none");
        }

        if (decryptedOffType !== "")
            $("#ApplyForId").val(decryptedOffType);

        $("#Type").val(decryptedOffType);

        if (decryptedlCardType !== "")
            $("#TypeId").val(decryptedlCardType);

    }

    $("#txtUnit").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                var param = { "UnitName": request.term };
                $("#UnitId").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitNameForBD',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.UnitName, value: item.UnitMapId };

                            }))
                        } else {

                            $("#txtUnit").val("");
                            $("#UnitId").val("0");
                            alert("SUS No not found.")
                        }


                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            /* $("#txtUnit").val(i.item.label);*/
            //alert(i.item.value)
            getunitbymapid(i.item.value);
        },

    });

    $("#ApplyForId, #RegimentalId").on("change", function () {
        $("#RegimentalId").valid();
    });
});
function GetROListByArmedId(ArmedId, sectid) {
    var userdata =
    {
        "ArmedId": ArmedId,
    };
    $.ajax({
        url: '/BasicDetail/GetROListByArmedId',
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

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="0">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].RecordOfficeId + '">' + response[i].Name + '</option>';
                    }
                    $("#RecordOfficeId").html(listItemddl);
                    if (sectid != '') {
                        $("#RecordOfficeId").val(sectid);

                    }
                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function GetRegimentalListByArmedId(ArmedId, sectid) {
    var userdata =
    {
        "ArmedId": ArmedId,
    };
    $.ajax({
        url: '/BasicDetail/GetRegimentalListByArmedId',
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

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="0">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].RegId + '">' + response[i].Name + '</option>';
                    }
                    $("#RegimentalId").html(listItemddl);

                    //if (TableId == 5 || TableId == 7 || TableId == 8) {

                    //    if (sectid != '') {
                    //        $("#" + ddl + " option").filter(function () {
                    //            return this.text == sectid;
                    //        }).attr('selected', true);

                    //    }
                    //}
                    //else
                    //{
                    if (sectid != '') {
                        $("#RegimentalId").val(sectid);

                    }

                    //}


                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function isInvalidValue(val) {
    return val === null || val === undefined || val.toString().trim() === '' || val === '0';
}
window.addEventListener("load", function () {
    if ($.validator) {
        $.validator.addMethod("regimentalRequired", function (value, element) {
            const applyForId = $("#ApplyForId").val();

            if (applyForId == '2') {
                return !isInvalidValue(value);
            }

            return true;
        }, "Regimental Centre is required.");
    }
});

async function CheckValidation() {

    if (!$("#TermsConditions").prop("checked")) {
        toastr.error('Please accept the Terms and Conditions');
        return false;
    }
    const form = $("#SaveForm");

    form.removeData("validator");
    form.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);

    $("#RegimentalId").rules("remove", "regimentalRequired");
    $("#RegimentalId").rules("add", {
        regimentalRequired: true
    });

    if (!form.valid()) {
        return false;
    }

    let formData = {};

    form.serializeArray().forEach(function (item) {
        formData[item.name] = item.value;
    });

    let jsonData = JSON.stringify(formData);

    let encrypted = encryptPayloadData(jsonData);

    $("#EncryptedData").val(encrypted);

    return true;
}
function GetUnit() {
    $.ajax({
        url: '/ConfigUser/GetTokenArmyNo',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == 0) {

                }
                else {
                    getunitbymapid(response.UnitId)
                }
            }
        }
    });
}
function getunitbymapid(value) {

    var param1 = { "UnitMapId": encryptPayloadData(value) };
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: param1,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {

            $("#txtUnit").val(data.UnitName);
            $("#PlaceOfIssue").val(data.UnitAbbreviation);
            $("#UnitId").val(data.UnitMapId);
        }
    });
}
function getApplyIcardDetails() {

    let userdata =
    {
        "ApplyForId": $("#ApplyForId").val(),
        "RegistrationId": $("#RegistrationId").val(),
        "TypeId": $("#TypeId").val()

    };
    $.ajax({
        url: "/Home/GetApplyCardDetails",
        type: "POST",
        data: { "request": encryptPayloadData(JSON.stringify(userdata)) },

        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response, status) {
            if (response != null) {

                if (response.ApplyFor == "Offrs") {
                    $("#IssuingAuth").val("OIC Unit");
                    $("#tempDateOfIssue").val("Depends on Unit of Second level approver");
                }
                else {
                    $("#IssuingAuth").val("OIC Unit");
                }
                $("#lblCategory").html(response.ApplyFor);
                $("#lblReason").html(response.Type);

                $("#RegdUser").html(response.RankAbbreviation + ' ' + response.Name + ' (' + response.ArmyNo + ') (' + response.DomainId + ')');

                if ($("#RegistrationId").val() == '3' || $("#RegistrationId").val() == '7') {
                    $("#lblunitname").html(response.Registraion);
                } else {
                    $("#lblunitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix + ')');
                }


            }

        }
    });
}
function initializeMediaUploadModals() {
    $("#btnOpenPhotoModal").on("click", function () {
        const photoInput = document.getElementById("Photo_");
        photoOriginalFiles = getCurrentFiles(photoInput);
        clearPhotoModalState();
        $("#photoUploadModal").modal("show");
    });

    $("#btnOpenSignatureModal").on("click", function () {
        const signatureInput = document.getElementById("Signature_");
        signatureOriginalFiles = getCurrentFiles(signatureInput);
        clearSignatureModalState();
        $("#signatureUploadModal").modal("show");
    });

    $("#Photo_").on("change", function () {
        handlePhotoSelection(this);
    });

    $("#Signature_").on("change", function () {
        handleSignatureSelection(this);
    });

    $("#btnUsePhoto").on("click", function () {
        if (!pendingPhotoDataUrl || !document.getElementById("Photo_").files.length) {
            return;
        }

        $("#PhotoPath").attr("src", pendingPhotoDataUrl);
        $("#lblPhoto").html("");
        $("#lblPhotoNotification")
            .removeClass("text-danger")
            .addClass("text-success")
            .text("Photo selected successfully");

        $("#btnOpenPhotoModal").text("Upload / Change Photo");
        $("#photoUploadModal").modal("hide");
    });

    $("#btnUseSignature").on("click", function () {
        if (!pendingSignatureDataUrl || !document.getElementById("Signature_").files.length) {
            return;
        }

        $("#SignaturePath").attr("src", pendingSignatureDataUrl);
        $("#lblSignature").html("");
        $("#lblSignatureNotification")
            .removeClass("text-danger")
            .addClass("text-success")
            .text("Signature selected successfully");

        $("#btnOpenSignatureModal").text("Upload / Change Signature");
        $("#signatureUploadModal").modal("hide");
    });

    $(document).on("click", ".js-photo-cancel", function () {
        restoreInputFiles(document.getElementById("Photo_"), photoOriginalFiles);
        $("#photoUploadModal").modal("hide");
    });

    $(document).on("click", ".js-signature-cancel", function () {
        restoreInputFiles(document.getElementById("Signature_"), signatureOriginalFiles);
        $("#signatureUploadModal").modal("hide");
    });

    $("#photoUploadModal").on("hidden.bs.modal", function () {
        clearPhotoModalState();
    });

    $("#signatureUploadModal").on("hidden.bs.modal", function () {
        clearSignatureModalState();
    });
}

function handlePhotoSelection(input) {
    clearPhotoValidationMessage();
    pendingPhotoDataUrl = "";
    $("#btnUsePhoto").prop("disabled", true);
    $("#PhotoModalPreviewWrap").addClass("d-none");

    const file = input.files && input.files[0];
    if (!file) {
        return;
    }

    const validationMessage = validateImageUpload(file, 200);
    if (validationMessage) {
        $("#PhotoModalMessage").text(validationMessage);
        input.value = "";
        return;
    }

    const reader = new FileReader();
    reader.onload = function (event) {
        pendingPhotoDataUrl = event.target.result;
        $("#PhotoModalPreview").attr("src", pendingPhotoDataUrl);
        $("#PhotoModalFileInfo").text(buildFileInfo(file));
        $("#PhotoModalPreviewWrap").removeClass("d-none");
        $("#PhotoModalMessage")
            .removeClass("text-danger")
            .addClass("text-success")
            .text("Photo is valid. Review it and click Use Photo & Close.");
        $("#btnUsePhoto").prop("disabled", false);
    };
    reader.onerror = function () {
        $("#PhotoModalMessage").text("Unable to read the selected photo. Please choose another file.");
        input.value = "";
    };
    reader.readAsDataURL(file);
}

function handleSignatureSelection(input) {
    clearSignatureValidationMessage();
    pendingSignatureDataUrl = "";
    $("#btnUseSignature").prop("disabled", true);
    $("#SignatureModalPreviewWrap").addClass("d-none");

    const file = input.files && input.files[0];
    if (!file) {
        return;
    }

    const validationMessage = validateImageUpload(file, 50);
    if (validationMessage) {
        $("#SignatureModalMessage").text(validationMessage);
        input.value = "";
        return;
    }

    const reader = new FileReader();
    reader.onload = function (event) {
        pendingSignatureDataUrl = event.target.result;
        $("#SignatureModalPreview").attr("src", pendingSignatureDataUrl);
        $("#SignatureModalFileInfo").text(buildFileInfo(file));
        $("#SignatureModalPreviewWrap").removeClass("d-none");
        $("#SignatureModalMessage")
            .removeClass("text-danger")
            .addClass("text-success")
            .text("Signature is valid. Review it and click Use Signature & Close.");
        $("#btnUseSignature").prop("disabled", false);
    };
    reader.onerror = function () {
        $("#SignatureModalMessage").text("Unable to read the selected signature. Please choose another file.");
        input.value = "";
    };
    reader.readAsDataURL(file);
}

function validateImageUpload(file, maxSizeKB) {
    const allowedTypes = ["image/jpeg", "image/jpg", "image/png"];

    if (!allowedTypes.includes(file.type)) {
        return "Invalid file type. Only JPG, JPEG and PNG files are allowed.";
    }

    const maxSizeBytes = maxSizeKB * 1024;
    if (file.size > maxSizeBytes) {
        return "Maximum file size " + maxSizeKB + " KB allowed. Selected file is " + formatFileSize(file.size) + ".";
    }

    return "";
}

function buildFileInfo(file) {
    return file.name + " • " + formatFileSize(file.size);
}

function formatFileSize(bytes) {
    return (bytes / 1024).toFixed(1) + " KB";
}

function getCurrentFiles(input) {
    if (!input || !input.files) {
        return [];
    }

    return Array.from(input.files);
}

function restoreInputFiles(input, files) {
    if (!input) {
        return;
    }

    if (!files || files.length === 0) {
        input.value = "";
        return;
    }

    if (typeof DataTransfer === "undefined") {
        return;
    }

    const transfer = new DataTransfer();
    files.forEach(function (file) {
        transfer.items.add(file);
    });
    input.files = transfer.files;
}

function clearPhotoValidationMessage() {
    $("#PhotoModalMessage")
        .removeClass("text-success")
        .addClass("text-danger")
        .text("");
}

function clearSignatureValidationMessage() {
    $("#SignatureModalMessage")
        .removeClass("text-success")
        .addClass("text-danger")
        .text("");
}

function clearPhotoModalState() {
    pendingPhotoDataUrl = "";
    clearPhotoValidationMessage();
    $("#PhotoModalPreview").attr("src", "");
    $("#PhotoModalFileInfo").text("");
    $("#PhotoModalPreviewWrap").addClass("d-none");
    $("#btnUsePhoto").prop("disabled", true);
}

function clearSignatureModalState() {
    pendingSignatureDataUrl = "";
    clearSignatureValidationMessage();
    $("#SignatureModalPreview").attr("src", "");
    $("#SignatureModalFileInfo").text("");
    $("#SignatureModalPreviewWrap").addClass("d-none");
    $("#btnUseSignature").prop("disabled", true);
}

/* BasicDetail UI-only helper */
$(function () {
    document.body.classList.add("ecms-basicdetail-body");

    // Recalculate select2 widths after existing dropdowns are populated.
    setTimeout(function () {
        try {
            $(".ecms-basicdetail-page select").each(function () {
                if ($(this).data("select2")) {
                    $(this).select2({ width: "100%" });
                }
            });
        } catch (e) {
            console.warn("BasicDetail UI select resize skipped:", e);
        }
    }, 400);
});