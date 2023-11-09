var errormsg = "Error.Due to network issue, please try after some time.";
var errormsg001 = "Error 001. Due to network issue, please try after some time.";
var errormsg002 = "Error 002. Due to network issue, please try after some time.";
var memberTable = "";
$(document).ready(function () {
    $('.Alphanumeric').on('change', function () {
      
        if ($('.Alphanumeric').val().match("^[a-zA-Z0-9 ]*$")) {
            return true
           
        }
        else {
            toastr.warning('Only Alphabets and Numbers allowed');

        }
    });

});
function isNumeric(str) {
    if (typeof str != "string") return false // we only process strings!  
    return !isNaN(str) && // use type coercion to parse the _entirety_ of the string (`parseFloat` alone does not do this)...
        !isNaN(parseFloat(str)) // ...and ensure strings of whitespace fail
}
function DateFormateMM_dd_yyyy(date) {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        var datemmddyyyy = dayans + `/` + monthsans + `/` + year
        return datemmddyyyy;
    }
    else {
        return '';
    }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateFormateMMMM_dd_yyyy(date) {
    const monthNames = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        var datemmddyyyy = dayans + `  ` + monthNames[datef2.getMonth()] + `  ` + year
        return datemmddyyyy;
    }
    else {
        return '';
    }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateFormateYYYY_mm_dd(date) {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        var datemmddyyyy = year + `-` + monthsans + `-` + dayans
        return datemmddyyyy;
    }
    else {
        return '';
    }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateCalculateago(fmDate) {
    ////////ago///////////
    var ago = "";
    var start_actual_time = fmDate;
    var end_actual_time = new Date();

    start_actual_time = new Date(start_actual_time);
    end_actual_time = new Date(end_actual_time);

    var diff = end_actual_time - start_actual_time;

    var diffSeconds = diff / 1000;
    var HH = Math.floor(diffSeconds / 3600);
    var MM = Math.floor(diffSeconds % 3600) / 60;

    var formatted = ((HH < 10) ? ("0" + HH) : HH) + ":" + ((MM < 10) ? ("0" + MM) : MM)

    var futureDate = new Date();
    var todayDate = new Date(fmDate);
    var milliseconds = futureDate.getTime() - todayDate.getTime();
    var hours = Math.floor(milliseconds / (60 * 60 * 1000));
    var formatted1 = formatted.substring(0, 2);
    if (parseInt(formatted1) == 00) {
        ago = formatted.substring(0, 5) + ' Minutes ago</h6>';;
    }
    else if (hours <= 24) {
        ago = hours + ' Hours ago</h6>';
    }
    else if (hours <= 730) {
        ago = Math.floor(hours / 24) + ' Days ago</h6>';;
    }
    else if (hours <= 8766) {
        ago = Math.floor(Math.floor(hours / 24) / 30) + ' Months ago</h6>';;
    }
    else {
        ago = Math.floor(Math.floor(Math.floor(hours / 24) / 30) / 12) + ' Years ago</h6>';;
    }
    return ago;
}
function DateCalculateyearmonthago(fmDate) {
    ////////ago///////////
    var year = "";
    var month = "";
    var start_actual_time = fmDate;
    var end_actual_time = new Date();

    start_actual_time = new Date(start_actual_time);
    end_actual_time = new Date(end_actual_time);

    var diff = end_actual_time - start_actual_time;

    var diffSeconds = diff / 1000;
    var HH = Math.floor(diffSeconds / 3600);
    var MM = Math.floor(diffSeconds % 3600) / 60;

    var formatted = ((HH < 10) ? ("0" + HH) : HH) + ":" + ((MM < 10) ? ("0" + MM) : MM)

    var futureDate = new Date();
    var todayDate = new Date(fmDate);
    var milliseconds = futureDate.getTime() - todayDate.getTime();
    var hours = Math.floor(milliseconds / (60 * 60 * 1000));
    var formatted1 = formatted.substring(0, 2);


    year = Math.floor(Math.floor(Math.floor(hours / 24) / 30) / 12);
    month = Math.floor(Math.floor(Math.floor(hours / 24) / 30) % 12);

    return year + 'y' + month + 'm';
}