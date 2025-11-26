$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    $("#btnsave").on("click", function () {
        CheckValidation();
    });
    $("#Signature_").on("change", function () {
        beforeUploadSignatureCheck(this);
    });
    $("#Photo_").on("change", function () {
        beforeUploadPhotoCheck(this);
    });
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
        GetROListByArmedId(this.value,"");
    });

    $("#TermsConditions").on("click",function () {

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

    $('#RegimentalId').on('change', function () {
       
        //$("#IssuingAuth").val("Comdt, " + $('#RegimentalId option:selected').text());
        //$("#PlaceOfIssue").val($('#RegimentalId option:selected').text());
    });

    if ($("#RegistrationId").val() == 1 || $("#RegistrationId").val() == 2 || $("#RegistrationId").val() == 6) {
        GetUnit() 
        $('#txtUnit').attr('readonly', true);
      //  getunitbymapid($("#aspndomainUnitID").html())
    }
    else {
        $('#txtUnit').attr('readonly', false);
        //getunitbymapid($("#spnUnitIdid").val())

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

        const decryptedArmyNo = decryptData(encryptedArmyNo);
        const decryptedOffType = decryptData(encryptedOffType);
        const decryptedlCardType = decryptData(encryptedlCardType);



        $("#ServiceNumber").val(decryptedArmyNo);
        $("#icarddetails").html('I-Card Appl Request For  (' + decryptedArmyNo +')');
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
                    url: '/Master/GetALLByUnitName',
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
        appendTo: '#suggesstion-box'
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
function CheckValidation() {
    
    if ($("#TermsConditions").prop("checked")) {
        if ($("#SaveForm")[0].checkValidity()) {
           // alert("Your Tracking Id -" + DateFormateMMddyyyy($("#DOB").val()) + "" + $("#AadhaarNo").val().substr($("#AadhaarNo").val().length - 4));
        }
        return true;
        
    }
    else {
        toastr.error('Please accept the Terms and Conditions');
        return false;
    }
}
function GetUnit() {
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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == 0) {
                    //  alert("Plase Add Profile")
                }
                else {
                    getunitbymapid(response.UnitId)
                }
            }
        }
    });
}
function getunitbymapid(value)
{
   
    var param1 = { "UnitMapId": value };
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
            //$("#lblProComd").html(data.ComdName);
            //$("#lblProCorps").html(data.CorpsName);
            //$("#lblProDiv").html(data.DivName);
            //$("#lblPrBde").html(data.BdeName);
            //$("#lblProSusno").html(data.Sus_no + '' + data.Suffix);

        }
    });
}
function getApplyIcardDetails() {
    $.ajax({
        url: "/Home/GetApplyCardDetails",
        type: "POST",
        data: {
            "ApplyForId": $("#ApplyForId").val(),
            "RegistrationId": $("#RegistrationId").val(),
            "TypeId": $("#TypeId").val()
        },
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
function beforeUploadSignatureCheck(id) {
    $("#lblSignature").html("");
    //$("#Signature_").prop('required', true);
    const file = id.files[0];
    if (file) {
        var size = parseFloat(file.size);
        var maxSizeKB = 50; //Size in KB.
        var maxSize = maxSizeKB * 1024; //File size is returned in Bytes.
        var allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];
        if (!allowedTypes.includes(file.type)) {
            $("#lblSignature").html("Invalid file type. Only JPG, JPEG, and PNG files are allowed. </br>");
            $("#lblSignatureNotification").addClass("text-danger");
            $("#lblSignatureNotification").removeClass("text-success");
            //$("#SignaturePath").attr({
            //    'src': '/writereaddata/images/noimage.png',
            //    'width': '180px',
            //    'height': '80px'
            //});
            $("#Signature_").val(null);
            return false;
        }
        else {
            if (size > maxSize) {
                $("#lblSignature").html("Maximum file size " + maxSizeKB + "KB allowed.");
                //$("#SignaturePath").attr({
                //    'src': '/writereaddata/images/noimage.png',
                //    'width': '180px',
                //    'height': '80px'
                //});
                $("#Signature_").val(null);
                $("#lblSignatureNotification").addClass("text-danger");
                $("#lblSignatureNotification").removeClass("text-success");
                return false;
            }
            else {
                $("#lblSignatureNotification").addClass("text-success");
                $("#lblSignatureNotification").removeClass("text-danger");
                signatureChange(id);
            }
        }

    }
}
function beforeUploadPhotoCheck(id) {
    $("#lblPhoto").html("");
    const file = id.files[0];
    if (file) {
        var size = parseFloat(file.size);
        var maxSizeKB = 200; //Size in KB.
        var maxSize = maxSizeKB * 1024; //File size is returned in Bytes.
        var allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];

        if (!allowedTypes.includes(file.type)) {
            $("#lblPhoto").html("Invalid file type. Only JPG, JPEG, and PNG files are allowed. </br>");
            $("#lblPhotoNotification").addClass("text-danger");
            $("#lblPhotoNotification").removeClass("text-success");
            //$("#PhotoPath").attr({
            //    'src': '/writereaddata/images/noimage.png',
            //    'width': '180px',
            //    'height': '158px'
            //});
            $("#Photo_").val(null);
            return false;
        }
        else {
            if (size > maxSize) {
                $("#lblPhoto").html("Maximum file size " + maxSizeKB + "KB allowed. </br>");
                //$("#PhotoPath").attr({
                //    'src': '/writereaddata/images/noimage.png',
                //    'width': '180px',
                //    'height': '158px'
                //});
                $("#Photo_").val(null);
                $("#lblPhotoNotification").addClass("text-danger");
                $("#lblPhotoNotification").removeClass("text-success");
                return false;
            } else {
                $("#lblPhotoNotification").addClass("text-success");
                $("#lblPhotoNotification").removeClass("text-danger");
                photoChange(id);
            }
        }
    }
}
function beforeSubmitValidateBasicDetail(id) {
    let formId = '#' + id;
    let rType = $("#Type").val();
    if (rType == '2') {
        $("#lblRegimentalId").text('');
        let regId = $("#RegimentalId").val();
        if (regId == '') {
            $('#RegimentalId').prop('required', true);
            $("#lblRegimentalId").text('Regimental is required.')
        }
    }
    $.validator.unobtrusive.parse($(formId));

    return false;
    //if ($(formId).valid()) {
    //}
    //else {
    //    return false;
    //}
}
function signatureChange(id) {
    const file = id.files[0];
    if (file) {
        let reader = new FileReader();
        reader.onload = function (event) {
            $("#SignaturePath")
                .attr("src", event.target.result);
        };
        reader.readAsDataURL(file);
    }
}
function photoChange(id) {
    const file = id.files[0];
    if (file) {
        let reader = new FileReader();
        reader.onload = function (event) {
            $("#PhotoPath")
                .attr("src", event.target.result);
        };
        reader.readAsDataURL(file);
    }
}
