$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
});

async function mMsater(sectid = '', ddl, TableId, ParentId) {

    const userdata = new URLSearchParams({
        id: TableId,
        ParentId: ParentId
    });

    try {
        const response = await fetch('/Master/GetAllMMaster', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            credentials: 'include',          // <--- IMPORTANT ensures the browser sends .AspNetCore.Session cookie with the request. when using fetch API
            body: userdata
        });

        const data = await response.json();

        if (data != "null" && data != null) {
            if (data === InternalServerError) {
                Swal.fire({
                    text: errormsg
                });
            } else {

                let listItemddl = "";

                if (parseInt(TableId) === 7) {
                    listItemddl += '<option value="">Select Rank</option>';
                } else {
                    listItemddl += '<option value="">Please Select</option>';
                }

                for (let i = 0; i < data.length; i++) {
                    listItemddl += `<option value="${data[i].Id}">${data[i].Name}</option>`;
                }

                document.getElementById(ddl).innerHTML = listItemddl;

                if (sectid !== '') {
                    document.getElementById(ddl).value = sectid;
                }
            }
        } else {
            // No data found case (optional alert as in original)
            // Swal.fire({
            //     text: "No data found Offrs"
            // });
        }

    } catch (error) {
        Swal.fire({
            text: errormsg002
        });
    }
}
function mMsaterByParent(sectid = '', ddl, TableId, ComdId,CorpsId,DivId,BdeId) {


    var userdata =
    {
        "TableId": TableId,
        "ComdId": ComdId,
        "CorpsId": CorpsId,
        "DivId": DivId,
        "BdeId": BdeId,

    };
    $.ajax({
        url: '/Master/GetAllMMasterByParent',
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

                    listItemddl += '<option value="">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';
                    }
                    $("#" + ddl + "").html(listItemddl);

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
                        $("#" + ddl + "").val(sectid);

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
function yeardata(sectid = '', ddl) {
    var listItemddl = "";
   
    listItemddl += '<option value="">select One</option>';


    for (var i = new Date().getFullYear(); i >= 1950; i--) {
        listItemddl += '<option value="' + i + '">' + i + '</option>';
    }
    $("#" + ddl + "").html(listItemddl);


    if (sectid != '') {
        $("#" + ddl + "").val(sectid);

    }
}

function GetAllOffsByUnitId(ddl, sectid, UnitId, IsRO, IsORO, IsAfsacCell,BasicDetailsId) {
    var userdata =
    {
        "id": 0,
        "UnitId": UnitId,
        "IsRO": IsRO,
        "IsORO": IsORO,
        "IsAfsacCell": IsAfsacCell,
        "BasicDetailsId": BasicDetailsId
    };
    $.ajax({
        url: '/UserProfile/GetOffrsByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    
                }

                else {
                    if (response[0].IsError == true) {
                        $("#ErrMess1").removeClass("d-none");
                        $("#ErrMess1").html(response[0].ErrorMessage);
                        $("#btnForward").prop("disabled", true);

                    }
                    else {
                        $("#ErrMess1").addClass("d-none");
                        $("#ErrMess1").html("");

                        $("#btnForward").prop("disabled", false);
                    }



                    var listItemddl = "";

                    listItemddl += '<option value="">Select Offr</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].AspNetUsersId + '">' + response[i].ArmyNo + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + '</option>';
                        
                    }
                    $("#" + ddl + "").html(listItemddl);

                   
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                    if ((IsRO == 1 || IsORO == 1 || IsAfsacCell == 1) && response.length == 1) {

                        $("#" + ddl + "").val(response[0].AspNetUsersId)

                        $("#spnFwdToAspNetUsersId").html(0);
                        $("#spnFwdToUsersId").html(0);
                        $(".spnFArmyNo").html("");
                        $(".spnFtoname").html("");
                        $(".spnFDomainName").html("");
                        $(".spnFAppName").html("");

                        $("#intoffsArmyNo").prop("checked", false);
                        $("#intoffDomainId").prop("checked", false);
                        $(".serchfwd").addClass("d-none");
                        FwdData(response[0].AspNetUsersId);
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

async function GetRemarks(ddl, sectid, RemarkTypeId) {
    const userdata = new URLSearchParams({
        RemarkTypeId: RemarkTypeId
    });

    try {
        const response = await fetch('/BasicDetail/GetRemarks', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: userdata
        });

        const data = await response.json();

        if (data != "null" && data != null) {

            if (data === InternalServerError) {
                Swal.fire({ text: errormsg });
            } else {

                let listItemddl = "";
                let currentRemarkTypeId = 0;
                let count = 1;

                for (let i = 0; i < data.length; i++) {

                    if (data[i].RemarkTypeId != currentRemarkTypeId) {
                        if (currentRemarkTypeId != 0) listItemddl += '</optgroup>';
                        count = 1;
                        listItemddl += `<optgroup label="${data[i].RemarksType}">`;
                    }

                    listItemddl += `<option value="${data[i].RemarksId}">${count}. ${data[i].Remarks}</option>`;
                    currentRemarkTypeId = data[i].RemarkTypeId;
                    count++;
                }

                listItemddl += '</optgroup>';
                $("#" + ddl).html(listItemddl);

                if (sectid !== '') {
                    $("#" + ddl).val(sectid);
                }
            }

        }
    } catch (error) {
        Swal.fire({ text: errormsg002 });
    }
}