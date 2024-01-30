var errormsg = "Error.Due to network issue, please try after some time.";
var errormsg001 = "Error 001. Due to network issue, please try after some time.";
var errormsg002 = "Error 002. Due to network issue, please try after some time.";
var memberTable = "";
$(document).ready(function () {

    $('.form-control').on('keypress', function () {
      
        if ($(this).val().match("^[a-zA-Z0-9 ]*$")) {
            
            $(this).val("");
            toastr.warning('special characters Not Allow');
        }
       
    });
    $('.Alphanumeric').on('change', function () {
        
        if ($('.Alphanumeric').val().match("^[a-zA-Z0-9 ]*$")) {
            return true
           
        }
        else {
            toastr.warning('Only Alphabets and Numbers allowed');

        }
    });
    $('.isNumerictxt').on('keypress', function () {

        if (isNumeric($('.isNumerictxt').val())) {
           
            return true

        }
        else {
            $(this).val("");
            toastr.warning('Only Numbers allowed');

        }
    });
    $(".btnPrint").click(function () {

        //var now = new Date();
        //var jsDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours()+ ':' + now.getMinutes() + ':' + now.getSeconds();

        //var WaterMark = '<div style="font-size:80px;opacity:0.2;transform:rotate(300deg);-webkit-transform:rotate(300deg);color:blue">' + $("#IpaddresGloble").html() + ' ' + jsDate +'</div>';
        //var printContents = document.getElementById('printdata').innerHTML;
        //var originalContents = document.body.innerHTML;
        //document.body.innerHTML = printContents + WaterMark;

        $('#printdata').printElement({
        });
    });


    setTimeout(function () {
        memberTable = $('#tbldatatabledata').DataTable({
        retrieve: true,
        lengthChange: false,
        "buttons": [
            {
                text: 'Download PDF',
                extend: 'pdfHtml5',
                filename: 'dt_custom_pdf',
                orientation: 'portrait', //portrait//landscape
                pageSize: 'A4', //A3 , A5 , A6 , legal , letter
                exportOptions: {
                   /* columns: ':visible',*/
                    search: 'applied',
                    order: 'applied',
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    //Remove the title created by datatTables
                    doc.content.splice(0, 1);
                    //Create a date string that we use in the footer. Format is dd-mm-yyyy
                    var now = new Date();
                    var jsDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours()+ ':' + now.getMinutes() + ':' + now.getSeconds();

                    
                   
                    // Logo converted to base64
                    // var logo = getBase64FromImageUrl('https://datatables.net/media/images/logo.png');
                    // The above call should work, but not when called from codepen.io
                    // So we use a online converter and paste the string in.
                    // Done on http://codebeautify.org/image-to-base64-converter
                    // It's a LONG string scroll down to see the rest of the code !!!
                    var logo = 'data:image/png;base64,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';
                    // A documentation reference can be found at
                    // https://github.com/bpampuch/pdfmake#getting-started
                    // Set page margins [left,top,right,bottom] or [horizontal,vertical]
                    // or one number for equal spread
                    // It's important to create enough space at the top for a header !!!
                    doc.pageMargins = [20, 60, 20, 30];
                    // Set the font size fot the entire document
                    doc.defaultStyle.fontSize = 7;
                    // Set the fontsize for the table header
                    doc.styles.tableHeader.fontSize = 7;
                    // Create a header object with 3 columns
                    // Left side: Logo
                    // Middle: brandname
                    // Right side: A document title
                    doc.watermark = { text: $("#IpaddresGloble").html() + ' ' + jsDate, color: 'blue', opacity: 0.2 };
                    doc['header'] = (function () {
                        return {
                            columns: [
                                {
                                    image: logo,
                                    width: 120
                                },
                                {
                                    alignment: 'left',
                                    italics: true,
                                    text: '',
                                    fontSize: 18,
                                    margin: [10, 0]
                                },
                                {
                                    alignment: 'right',
                                    fontSize: 14,
                                    text: 'PDF Download From E-IASAC'
                                }
                            ],
                            margin: 20
                        }
                    });
                    // Create a footer object with 2 columns
                    // Left side: report creation date
                    // Right side: current page and total pages
                    doc['footer'] = (function (page, pages) {
                        return {
                            columns: [
                                {
                                    alignment: 'left',
                                    text: ['Print Date on: ', { text: jsDate.toString() }]
                                },
                                {
                                    alignment: 'right',
                                    text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                }
                            ],
                            margin: 20
                        }
                    });
                    // Change dataTable layout (Table styling)
                    // To use predefined layouts uncomment the line below and comment the custom lines below
                    // doc.content[0].layout = 'lightHorizontalLines'; // noBorders , headerLineOnly
                    var objLayout = {};
                    objLayout['hLineWidth'] = function (i) { return .5; };
                    objLayout['vLineWidth'] = function (i) { return .5; };
                    objLayout['hLineColor'] = function (i) { return '#aaa'; };
                    objLayout['vLineColor'] = function (i) { return '#aaa'; };
                    objLayout['paddingLeft'] = function (i) { return 4; };
                    objLayout['paddingRight'] = function (i) { return 4; };
                    doc.content[0].layout = objLayout;
                }
            }]
    });
        memberTable.buttons().container().appendTo('#tbldatatabledata_wrapper .col-md-6:eq(0)');

    }, 1000);
    $("#tbldatatabledata #chkAll").click(function () {
        if ($(this).is(':checked')) {
            rows = memberTable.rows({ 'search': 'applied' }).nodes();
            $('input[type="checkbox"]', rows).prop('checked', this.checked);
        }
        else {
            rows = memberTable.rows({ 'search': 'applied' }).nodes();
            $('input[type="checkbox"]', rows).prop('checked', this.checked);
        }
    });
    memberTable = $('#tbldatatable').DataTable({
        retrieve: true,
        lengthChange: false,
        "buttons": [
            {
                text: 'Download PDF',
                extend: 'pdfHtml5',
                filename: 'dt_custom_pdf',
                orientation: 'portrait', //portrait//landscape
                pageSize: 'A4', //A3 , A5 , A6 , legal , letter
                exportOptions: {
                  /*  columns: ':visible',*/
                    search: 'applied',
                    order: 'applied',
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    //Remove the title created by datatTables
                    doc.content.splice(0, 1);
                    //Create a date string that we use in the footer. Format is dd-mm-yyyy
                    var now = new Date();
                    var jsDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();



                    // Logo converted to base64
                    // var logo = getBase64FromImageUrl('https://datatables.net/media/images/logo.png');
                    // The above call should work, but not when called from codepen.io
                    // So we use a online converter and paste the string in.
                    // Done on http://codebeautify.org/image-to-base64-converter
                    // It's a LONG string scroll down to see the rest of the code !!!
                    var logo = 'data:image/png;base64,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';
                    // A documentation reference can be found at
                    // https://github.com/bpampuch/pdfmake#getting-started
                    // Set page margins [left,top,right,bottom] or [horizontal,vertical]
                    // or one number for equal spread
                    // It's important to create enough space at the top for a header !!!
                    doc.pageMargins = [20, 60, 20, 30];
                    // Set the font size fot the entire document
                    doc.defaultStyle.fontSize = 7;
                    // Set the fontsize for the table header
                    doc.styles.tableHeader.fontSize = 7;
                    // Create a header object with 3 columns
                    // Left side: Logo
                    // Middle: brandname
                    // Right side: A document title
                    doc.watermark = { text: $("#IpaddresGloble").html() + ' ' + jsDate, color: 'blue', opacity: 0.2 };
                    doc['header'] = (function () {
                        return {
                            columns: [
                                {
                                    image: logo,
                                    width: 120
                                },
                                {
                                    alignment: 'left',
                                    italics: true,
                                    text: '',
                                    fontSize: 18,
                                    margin: [10, 0]
                                },
                                {
                                    alignment: 'right',
                                    fontSize: 14,
                                    text: 'PDF Download From E-IASAC'
                                }
                            ],
                            margin: 20
                        }
                    });
                    // Create a footer object with 2 columns
                    // Left side: report creation date
                    // Right side: current page and total pages
                    doc['footer'] = (function (page, pages) {
                        return {
                            columns: [
                                {
                                    alignment: 'left',
                                    text: ['Print Date on: ', { text: jsDate.toString() }]
                                },
                                {
                                    alignment: 'right',
                                    text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                }
                            ],
                            margin: 20
                        }
                    });
                    // Change dataTable layout (Table styling)
                    // To use predefined layouts uncomment the line below and comment the custom lines below
                    // doc.content[0].layout = 'lightHorizontalLines'; // noBorders , headerLineOnly
                    var objLayout = {};
                    objLayout['hLineWidth'] = function (i) { return .5; };
                    objLayout['vLineWidth'] = function (i) { return .5; };
                    objLayout['hLineColor'] = function (i) { return '#aaa'; };
                    objLayout['vLineColor'] = function (i) { return '#aaa'; };
                    objLayout['paddingLeft'] = function (i) { return 4; };
                    objLayout['paddingRight'] = function (i) { return 4; };
                    doc.content[0].layout = objLayout;
                }
            }]
    });
    $('#DetailBodyData').on('change', 'input[type="checkbox"]', function () {
        if (!this.checked) {
            var el = $('#chkAll').get(0);
            if (el && el.checked && ('indeterminate' in el)) {
                el.indeterminate = true;
            }
        }
    });
});

function isNumeric(str) {
    if (typeof str != "string") return false // we only process strings!  
    return !isNaN(str) && // use type coercion to parse the _entirety_ of the string (`parseFloat` alone does not do this)...
        !isNaN(parseFloat(str)) // ...and ensure strings of whitespace fail
}
function DateFormateMMddyyyy(date) {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        var datemmddyyyy = dayans + `` + monthsans + `` + year
        return datemmddyyyy;
    }
    else {
        return '';
    }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateFormatehhmmss(date) {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    var hh = `${datef2.getHours()}`;
    var mm = `${datef2.getMinutes()}`;
    var ss = `${datef2.getSeconds()}`;
    if (year > 1902) {
        hh = Pad(hh);
        mm = Pad(mm);
        ss = Pad(ss);
        var datemmddyyyy = hh + `:` + mm + `:` + ss
        return datemmddyyyy;
    }
    else {
        return '';
    }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateFormateddMMyyyyhhmmss(date) {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    var hh = `${datef2.getHours()}`;
    var mm = `${datef2.getMinutes()}`;
    var ss = `${datef2.getSeconds()}`;
    if (year > 1902) {

        var datemmddyyyy = dayans + `/` + monthsans + `/` + year + ` ` + hh + `:` + mm + `:` + ss
        return datemmddyyyy;
    }
    else {
        return '';
    }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
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
function DateFormatedd_mm_yyyy(date) {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    if (year > 1902) {

        var datemmddyyyy = dayans + `-` + monthsans + `-` + year + ` ` + "00:00";
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
function Pad(d) {
    return (d < 10) ? '0' + d.toString() : d.toString();
}
(function ($) {
    'use strict'

    $.fn.printElement = function (options) {
        let settings = $.extend({
            title: "Pdf DownLoad",
            css: 'extend',
            ecss: null,
            lcss: [],
            keepHide: [],
            wrapper: {
                wrapper: null,
                selector: null,
            }
        }, options);

        const element = $(this).clone();
        let html = document.createElement('html');

        let head = document.createElement('head');
        if (settings.title != null && settings.title != '') {
            head = $(head).append($(document.createElement('title')).text(settings.title));
        }
        else {
            head = $(head);
        }

        if (settings.css == 'extend' || settings.css == 'link') {
            $('link[rel=stylesheet]').each(function (index, linkcss) {
                head = head.append($(document.createElement('link')).attr('href', $(linkcss).attr('href')).attr('rel', 'stylesheet').attr('media', 'print'));
            })
        }

        for (var i = 0; i < settings.lcss.length; i++) {
            head = head.append($(document.createElement('link')).attr('href', settings.lcss[i]).attr('rel', 'stylesheet').attr('media', 'print'));
        }

        if (settings.css == 'extend' || settings.css == 'style') {
            head.append($(document.createElement('style')).append($('style').clone().html()));
        }

        if (settings.ecss != null) {
            head.append($(document.createElement('style')).html(settings.ecss));
        }
       

        if (settings.wrapper.wrapper === null) {
            var body = document.createElement('body');
            body = $(body).append(element);
        }
        else {
            var body = $(settings.wrapper.wrapper).clone();
            body.find(settings.wrapper.selector).append(element);
        }

        for (let i = 0; i < settings.keepHide.length; i++) {
            $(body).find(settings.keepHide[i]).each(function (index, data) {
                $(this).css('display', 'none');
            })
        }
        

        var now = new Date();
        var jsDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();

        var WaterMark = '<div style="position:fixed;top:50%;font-size:70px;font-weight:800;opacity:0.1;transform:rotate(300deg);color:blue">' + $("#IpaddresGloble").html() + ' ' + jsDate + '</div>';
        $(body).append(WaterMark);

        html = $(html).append(head).append(body);
       
        const fn_window = document.open('', settings.title, 'width=' + $(document).width() + ',height=' + $(document).width() + '');

       
        fn_window.document.write(html.clone().html());
        

        setTimeout(function () { fn_window.print(); fn_window.close(); }, 250);

        return $(this);
    }
}(jQuery));
