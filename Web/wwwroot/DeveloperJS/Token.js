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

    if (IcNo === "IC75695P") {
        IcNo = "9a4beb14b87de35d6bba98e2b16ad4eb341d52bda2bb3b7eadb064baf676cbd3"; //7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996
    } else if (IcNo === "IC60056W") {
        IcNo = "A2A7D3ED10E454CDD66285EBDFCC293549762148F74D4A65221250769C8E6448";
    }

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
                thumbprint = data[0].Thumbprint;
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

        const response2 = await fetch(HostUrlDGISToken + '/Temporary_Listen_Addresses/FetchTokenOCSPDetails?ThumbPrint=' + thumbprint, {
            method: "GET",
            cache: "no-cache",
            headers: {
                "Accept": "application/json"
            }
        });

        const data = await response.json();

        const data2 = await response2.json();

        $("#loadingToken").hide();

        if (data) {
            const validationResult = data.ValidatePersID2FAResult;
            const CSPStatus = data2[0].OCSPCheck;

            if (validationResult === true) { //validationResult === false
                $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');

                if (CSPStatus === true)
                {
                    if (txticno !== "") {
                        await GetTokenDetails('FetchUniqueTokenDetails', txticno, thumbprint);
                    }
                }
                else
                {
                    $("#" + msgid).html(`<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">${data2[0].OCSPMsg}</span></div>`);
                    $("#" + txticno).val("");
                    $("#txtspnIsToken").val("");
                }
            }
            else {
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
                let validTo;

                pairs.forEach(pair => {
                    let [k, v] = pair.split("=");
                    keyValuePairs[k.trim()] = v ? v.trim() : "";
                });

                const datef2 = new Date();

                const validToDate = parseApiDate(data[0].ValidTo);

                if (validToDate === null) {
                    console.log("Invalid ValidTo date:", data[0].ValidTo);
                }
                else {
                    validTo = validToDate;
                }

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
function parseApiDate(dateString) {

    if (!dateString || typeof dateString !== "string") {
        return null;
    }

    dateString = dateString.trim();

    const monthNames = {
        jan: 0,
        january: 0,
        feb: 1,
        february: 1,
        mar: 2,
        march: 2,
        apr: 3,
        april: 3,
        may: 4,
        jun: 5,
        june: 5,
        jul: 6,
        july: 6,
        aug: 7,
        august: 7,
        sep: 8,
        sept: 8,
        september: 8,
        oct: 9,
        october: 9,
        nov: 10,
        november: 10,
        dec: 11,
        december: 11
    };

    let match;

    // ==========================================================
    // FORMAT 1:
    // 13-01-2029 14:57:39
    // 13/01/2029 14:57:39
    // 13.01.2029 14:57:39
    // ==========================================================
    match = dateString.match(
        /^(\d{1,2})[-\/.](\d{1,2})[-\/.](\d{2,4})\s+(\d{1,2}):(\d{2})(?::(\d{2}))?$/
    );

    if (match) {

        let day = Number(match[1]);
        let month = Number(match[2]) - 1;
        let year = Number(match[3]);
        let hour = Number(match[4]);
        let minute = Number(match[5]);
        let second = Number(match[6] || 0);

        if (year < 100) {
            year += 2000;
        }

        return createValidDate(
            year,
            month,
            day,
            hour,
            minute,
            second
        );
    }


    // ==========================================================
    // FORMAT 2:
    // 05-May-23 2:39:40 PM
    // 05-May-2023 2:39:40 PM
    // 05-May-2023 14:39:40
    // ==========================================================
    match = dateString.match(
        /^(\d{1,2})[-\/\s]([A-Za-z]+)[-\/\s](\d{2,4})\s+(\d{1,2}):(\d{2})(?::(\d{2}))?\s*(AM|PM)?$/i
    );

    if (match) {

        let day = Number(match[1]);
        let monthName = match[2].toLowerCase();
        let year = Number(match[3]);

        let hour = Number(match[4]);
        let minute = Number(match[5]);
        let second = Number(match[6] || 0);

        let ampm = match[7];

        let month = monthNames[monthName];

        if (month === undefined) {
            return null;
        }

        if (year < 100) {
            year += 2000;
        }

        // Convert 12-hour time to 24-hour
        if (ampm) {

            ampm = ampm.toUpperCase();

            if (ampm === "PM" && hour < 12) {
                hour += 12;
            }

            if (ampm === "AM" && hour === 12) {
                hour = 0;
            }
        }

        return createValidDate(
            year,
            month,
            day,
            hour,
            minute,
            second
        );
    }


    // ==========================================================
    // FORMAT 3:
    // 13-01-2029
    // 13/01/2029
    // ==========================================================
    match = dateString.match(
        /^(\d{1,2})[-\/.](\d{1,2})[-\/.](\d{2,4})$/
    );

    if (match) {

        let day = Number(match[1]);
        let month = Number(match[2]) - 1;
        let year = Number(match[3]);

        if (year < 100) {
            year += 2000;
        }

        return createValidDate(
            year,
            month,
            day,
            0,
            0,
            0
        );
    }


    // ==========================================================
    // FORMAT 4:
    // 05-May-23
    // 05 May 2023
    // ==========================================================
    match = dateString.match(
        /^(\d{1,2})[-\/\s]([A-Za-z]+)[-\/\s](\d{2,4})$/
    );

    if (match) {

        let day = Number(match[1]);
        let monthName = match[2].toLowerCase();
        let year = Number(match[3]);

        let month = monthNames[monthName];

        if (month === undefined) {
            return null;
        }

        if (year < 100) {
            year += 2000;
        }

        return createValidDate(
            year,
            month,
            day,
            0,
            0,
            0
        );
    }


    // ==========================================================
    // FORMAT 5:
    // ISO date
    // 2029-01-13T14:57:39
    // 2029-01-13T14:57:39Z
    // ==========================================================
    let nativeDate = new Date(dateString);

    if (!isNaN(nativeDate.getTime())) {
        return nativeDate;
    }


    // Unknown / invalid format
    return null;
}


// ==========================================================
// DATE VALIDATION
// Prevent invalid dates like 31-02-2029
// ==========================================================
function createValidDate(year, month, day, hour, minute, second) {

    if (
        month < 0 ||
        month > 11 ||
        day < 1 ||
        day > 31 ||
        hour < 0 ||
        hour > 23 ||
        minute < 0 ||
        minute > 59 ||
        second < 0 ||
        second > 59
    ) {
        return null;
    }

    const date = new Date(
        year,
        month,
        day,
        hour,
        minute,
        second
    );

    // Validate actual calendar date
    if (
        date.getFullYear() !== year ||
        date.getMonth() !== month ||
        date.getDate() !== day ||
        date.getHours() !== hour ||
        date.getMinutes() !== minute ||
        date.getSeconds() !== second
    ) {
        return null;
    }

    return date;
}