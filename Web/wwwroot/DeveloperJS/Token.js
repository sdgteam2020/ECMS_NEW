$(function () {
    $("#loadingToken").hide();
    $("#btnfetchtoken").on("click", async function () {
        await GetTokenDetails("FetchUniqueTokenDetails", "txtArmyNo");
    });
    $("#btnTokenFetchDetails").on("click", async function () {
        await GetTokenDetails('FetchUniqueTokenDetails', 'ICNo', '', 'tokenmsg')
    });
});

async function GetTokenvalidatepersid2fawiththumbprint(IcNo, msgid, txticno, thumbprint) {
    $("#loadingToken").show();

    //if (IcNo === "IC75695P") {
    //    IcNo = "9a4beb14b87de35d6bba98e2b16ad4eb341d52bda2bb3b7eadb064baf676cbd3"; //7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996
    //} else if (IcNo === "IC60056W") {
    //    IcNo = "A2A7D3ED10E454CDD66285EBDFCC293549762148F74D4A65221250769C8E6448";
    //}

    try {
        const response = await fetch(HostUrlDGISToken + '/Temporary_Listen_Addresses/FetchUniqueTokenDetails', {
            method: 'GET',
            cache: 'no-cache',
            headers: {
                'Accept': 'application/json'
            }
        });

        const data = await response.json();
        $("#loadingToken").hide();

        if (data && data.length > 0) {
            if (data[0].Status === '200') {
                await GetTokenvalidatepersid2fa(IcNo, msgid, txticno, thumbprint);
            } else if (data[0].Status === '404') {
                $("#" + msgid).html(`<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">${data[0].Remarks} </span></div>`);
                $("#" + txticno).val("");
            }
        }
    } catch (error) {
        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Appl Not Running</span>.</div>');
        $("#" + txticno).val("");
        $("#loadingToken").hide();
    }
}

async function GetTokenvalidatepersid2fa(IcNo, msgid, txticno, thumbprint) {
    $("#loadingToken").show();

    try {
        const response = await fetch(HostUrlDGISToken + '/Temporary_Listen_Addresses/validatepersid2fa', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8',
            },
            body: JSON.stringify({
                "inputPersID": IcNo,
            }),
        });

        const data = await response.json();
        $("#loadingToken").hide();

        if (data) {
            const validationResult = data.ValidatePersID2FAResult;

            if (validationResult === true) { //validationResult === false
                $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');

                if (txticno !== "") {
                    await GetTokenDetails('FetchUniqueTokenDetails', txticno, thumbprint);
                }
            } else {
                $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">ICNO Not Match Inserted Token </span></div>');
                $("#" + txticno).val("");
                $("#txtspnIsToken").val("");
            }
        }
    } catch (error) {
        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Appl Not Running</span>.</div>');
        $("#loadingToken").hide();
    }
}

async function GetTokenValidate(ApiId, IcNo, msgid) {
    $("#loadingToken").show();

    try {
        const response = await fetch(HostUrlDGISToken + '/Temporary_Listen_Addresses/' + ApiId, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8',
            },
            body: JSON.stringify({
                "inputpersId": IcNo,
            }),
        });

        const data = await response.json();
        $("#loadingToken").hide();

        if (data) {
            const result = data.ValidatePersIDResult;

            if (result[0].Status === '200') {
                $("#" + msgid).html(`<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">${result[0].Remark}</span></div>`);
            } else if (result[0].Status === '404') {
                $("#" + msgid).html(`<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">${result[0].Remark}</span></div>`);
                $("#txtspnIsToken").val("");
            }
        }
    } catch (error) {
        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Appl Not Running</span>.</div>');
        $("#loadingToken").hide();
    }
}

async function GetTokenDetails(ApiId, txt, thumbprint, msgid,ddl='') {
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
                if (datef2 <= validTo) { //validTo >= datef2
                    $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">Token Expired</span>.</div>');
                    $("#" + txt).val("");
                    if (thumbprint !== "") $("#" + thumbprint).val("");
                    $("#txtspnIsToken").val("");
                } else {
                    $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check" aria-hidden="true"></i><span class="m-lg-2">Token Detected</span></div>');
                    if (thumbprint !== "")
                        $("#" + thumbprint).val(data[0].Thumbprint);
                    $("#txtspnIsToken").val("Ok");

                    if (keyValuePairs.SERIALNUMBER.toLowerCase().trim() === "9a4beb14b87de35d6bba98e2b16ad4eb341d52bda2bb3b7eadb064baf676cbd3") { //"7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996"
                        if (ddl != '') {
                            $("#" + ddl).val("IC");
                            $("#" + txt).val("75695P");
                        } else {
                            $("#" + txt).val("IC75695P");
                        }
                    } else if (keyValuePairs.SERIALNUMBER.toLowerCase().trim() === "A2A7D3ED10E454CDD66285EBDFCC293549762148F74D4A65221250769C8E6448".toLowerCase().trim()) {
                        if (ddl != '') {
                            $("#" + ddl).val("IC");
                            $("#" + txt).val("60056W");
                        } else {
                            $("#" + txt).val("IC60056W");
                        }
                    } else {
                        $("#" + txt).val(keyValuePairs.SERIALNUMBER.toUpperCase().trim());
                    } 
                }
            }
            else if (data[0].Status === '404') {
                $("#" + msgid).html(`<div class="mt-4 alert alert-danger alert-dismissible fade show"><i class="fa fa-check" aria-hidden="true"></i><span class="m-lg-2">${data[0].Remarks}</span></div>`);
                $("#" + txt).val("");
                $("#txtspnIsToken").val("");
            }
            else if (data[0].Status === '500') {
                $("#" + msgid).html(`<div class="mt-4 alert alert-danger alert-dismissible fade show"><i class="fa fa-check" aria-hidden="true"></i><span class="m-lg-2">Technical Error While Fetching Token</span></div>`);
                $("#" + txt).val("");
                $("#txtspnIsToken").val("");
            }
        }
        else {
            $("#" + msgid).html(errormsg001);
            return 0;
        }
    }
    catch (error) {
        $("#" + msgid).html(`<div class="mt-4 alert alert-danger alert-dismissible fade show"><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Appl Not Running</span></div>`);
        $("#" + txt).val("");
        $("#loadingToken").hide();
    }
}