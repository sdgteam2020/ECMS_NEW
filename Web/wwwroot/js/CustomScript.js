function registrationEnableDisabledField(val) {
    debugger
    if (val==1) {
        $("#btn1").prop('disabled', false);
        $("#btn2").prop('disabled', true);
    }
    else {
        $("#btn1").prop('disabled', true);
        $("#btn2").prop('disabled', false);
    }

}
function Aadhaarformate(e, elemt) {
    if (e.which !== 8) {
        if (elemt.value.length === 4 || elemt.value.length === 9) {
            elemt.value = elemt.value += ' ';
        }
    }
}
function beforeUploadSignatureSizeCheck(id) {
    $("#lblSignature").html("");
    const file = id.files[0];
    if (file) {
        var size = parseFloat(file.size);
        var maxSizeKB = 50; //Size in KB.
        var maxSize = maxSizeKB * 1024; //File size is returned in Bytes.
        if (size > maxSize) {
            $("#lblSignature").html("Maximum file size " + maxSizeKB + "KB allowed.");
            $("#SignaturePath").attr({
                'src': '/writereaddata/images/noimage.png',
                'width': '100px',
                'height': '80px'
            });
            $("#Signature_").val(null);
            return false;
        }
        else {
            signatureChange(id);
        }

    }
}
function beforeUploadPhotoSizeCheck(id) {
    $("#lblPhoto").html("");
    const file = id.files[0];
    if (file) {
        var size = parseFloat(file.size);
        var maxSizeKB = 200; //Size in KB.
        var maxSize = maxSizeKB * 1024; //File size is returned in Bytes.
        if (size > maxSize) {
            $("#lblPhoto").html("Maximum file size " + maxSizeKB + "KB allowed.");
            $("#PhotoPath").attr({
                'src': '/writereaddata/images/noimage.png',
                'width': '200px',
                'height': '200px'
            });
            $("#Photo_").val(null);
            return false;
        }
        else {
            photoChange(id);
        }

    }
}
function beforeSubmitValidateBasicDetail(id) {
    let formId = '#' + id;

    //$('#ArmService').prop('required', true);

    $("input").prop('required', true);
    $('#Rank').prop('required', true);
    $('#StateId').prop('required', true);
    $('#DistrictId').prop('required', true);
    $('#BloodGroup').prop('required', true);
    $('#PlaceOfIssue').prop('required', true);

    $.validator.unobtrusive.parse($(formId));
    if ($(formId).valid()) {
        var formData = $(formId).serialize();
        console.log(formData);

    }
}
function ProfileEnableDisabledField() {
    if ($("#TypeOfUnit").val() == 'Formation / Unit') {
        $("#Comd").prop('disabled', false);
        $("#Corps").prop('disabled', false);
        $("#Div").prop('disabled', false);
        $("#Bde").prop('disabled', false);
        $("#Bde").prop('disabled', false);
        $("#GISArmyNumberPart1").prop('disabled', false);
        $("#GISArmyNumberPart2").prop('readonly', false);
        $("#GISArmyNumberPart3").prop('disabled', false);
        $("#GISRank").prop('disabled', false);
        $("#GISName").prop('readonly', false);
        $("#GISAppointment").prop('readonly', false);
        $("#GISUnitFormation").prop('readonly', false);
    }
    else {
        $("#Comd").prop('disabled', true);
        $("#Corps").prop('disabled', true);
        $("#Div").prop('disabled', true);
        $("#Bde").prop('disabled', true);
        $("#GISArmyNumberPart1").prop('disabled', true);
        $("#GISArmyNumberPart2").prop('readonly', true);
        $("#GISArmyNumberPart3").prop('disabled', true);
        $("#GISRank").prop('disabled', true);
        $("#GISName").prop('readonly', true);
        $("#GISAppointment").prop('readonly', true);
        $("#GISUnitFormation").prop('readonly', true);
    }
}
function ProfileDisabledField(value) {
    if (value === "Formation / Unit") {
        $("#Comd").prop('disabled', false);
        $("#Corps").prop('disabled', false);
        $("#Div").prop('disabled', false);
        $("#Bde").prop('disabled', false);
        $("#Bde").prop('disabled', false);
        $("#GISArmyNumberPart1").prop('disabled', false);
        $("#GISArmyNumberPart2").prop('readonly', false);
        $("#GISArmyNumberPart3").prop('disabled', false);
        $("#GISRank").prop('disabled', false);
        $("#GISName").prop('readonly', false);
        $("#GISAppointment").prop('readonly', false);
        $("#GISUnitFormation").prop('readonly', false);
    }
    else {
        $("#Comd").prop('disabled', true);
        $("#Comd").val('');
        $("#Corps").prop('disabled', true);
        $("#Corps").val('');
        $("#Div").prop('disabled', true);
        $("#Div").val('');
        $("#Bde").prop('disabled', true);
        $("#Bde").val('');
        $("#GISArmyNumberPart1").prop('disabled', true);
        $("#GISArmyNumberPart1").val('');
        $("#GISArmyNumberPart2").prop('readonly', true);
        $("#GISArmyNumberPart2").val('');
        $("#GISArmyNumberPart3").prop('disabled', true);
        $("#GISArmyNumberPart3").val('');
        $("#GISRank").prop('disabled', true);
        $("#GISRank").val('');
        $("#GISName").prop('readonly', true);
        $("#GISName").val('');
        $("#GISAppointment").prop('readonly', true);
        $("#GISAppointment").val('');
        $("#GISUnitFormation").prop('readonly', true);
        $("#GISUnitFormation").val('');
    }
}
function resetForm(id) {
    document.getElementById(id).reset();
}
function ProfileDummyData() {
    $.ajax({
        url: "/ProfileData/DummyData",
        type: "POST",
        success: function (response, status) {
            alert(JSON.stringify(response));
            $("#ArmyNumberPart1").val(response.ArmyNumberPart1);
            $("#ArmyNumberPart2").val(response.ArmyNumberPart2);
            $("#ArmyNumberPart3").val(response.ArmyNumberPart3);
            $("#Rank").val(response.Rank);
            $("#Name").val(response.Name);
            $("#Appointment").val(response.Appointment);
            $("#DomainId").val(response.DomainId);
            $("#UnitSusNoPart1").val(response.UnitSusNoPart1);
            $("#UnitSusNoPart2").val(response.UnitSusNoPart2);
            $("#UnitName").val(response.UnitName);
            $("#Comd").val(response.Comd);
            $("#Corps").val(response.Corps);
            $("#Div").val(response.Div);
            $("#Bde").val(response.Bde);
            $("#TypeOfUnit").val(response.TypeOfUnit);
            $("#IOArmyNumberPart1").val(response.IOArmyNumberPart1);
            $("#IOArmyNumberPart2").val(response.IOArmyNumberPart2);
            $("#IOArmyNumberPart3").val(response.IOArmyNumberPart3);
            $("#IORank").val(response.IORank);
            $("#IOName").val(response.IOName);
            $("#IOAppointment").val(response.IOAppointment);
            $("#IOUnitFormation").val(response.IOUnitFormation);
            $("#GISArmyNumberPart1").val(response.GISArmyNumberPart1);
            $("#GISArmyNumberPart2").val(response.GISArmyNumberPart2);
            $("#GISArmyNumberPart3").val(response.GISArmyNumberPart3);
            $("#GISRank").val(response.GISRank);
            $("#GISName").val(response.GISName);
            $("#GISAppointment").val(response.GISAppointment);
            $("#GISUnitFormation").val(response.GISUnitFormation);
            if (response.TypeOfUnit == 'Formation / Unit') {
                $("#Comd").prop('disabled', false);
                $("#Corps").prop('disabled', false);
                $("#Div").prop('disabled', false);
                $("#Bde").prop('disabled', false);
                $("#Bde").prop('disabled', false);
                $("#GISArmyNumberPart1").prop('disabled', false);
                $("#GISArmyNumberPart2").prop('readonly', false);
                $("#GISArmyNumberPart3").prop('disabled', false);
                $("#GISRank").prop('disabled', false);
                $("#GISName").prop('readonly', false);
                $("#GISAppointment").prop('readonly', false);
                $("#GISUnitFormation").prop('readonly', false);
            }
        }
    });
}
function getDummyData() { 
    //$(document).ready(function () {
    //})
        $.ajax({
            url: "/BasicDetail/DummyData",
            type: "POST",
            success: function (response, status) {
                alert(JSON.stringify(response));
                $("#Name").val(response.Name);
                $("#Rank").val(response.Rank);
                $("#ArmService").val(response.ArmService);
                $("#ServiceNo").val(response.ServiceNo);
                $("#IdentityMark").val(response.IdentityMark);
                $("#DOB").val(response.DOB.split('T')[0]);
                $("#Height").val(response.Height);
                $("#AadhaarNo").val(response.AadhaarNo);
                $("#BloodGroup").val(response.BloodGroup);
                $("#StateId").val(response.StateId);
                $("#DistrictId").val(response.DistrictId);
                $("#PlaceOfIssue").val(response.PlaceOfIssue);
                $("#DateOfIssue").val(response.DateOfIssue.split('T')[0]);
                $("#DateOfCommissioning").val(response.DateOfCommissioning.split('T')[0]);
                $("#PermanentAddress").val(response.PermanentAddress);
            }
        });

}
function GetDistrictListByStateId(stateId) {
    $.ajax({
        url: "/BasicDetail/GetDistrictListByStateId",
        type: "POST",
        data: {
            "StateId": stateId
        },
        success: function (response, status) {
            $('#DistrictId').find('option').not(':first').remove();
            for (var i = 0; i < response.length; i++) {
                $('#DistrictId').append('<option value="' + response[i].DistrictId + '">' + response[i].Name + '</option>');
            }
        }
    });
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
function getUserData() {
    $.ajax({
        url: "/BasicDetail/GetUserData",
        type: "POST",
        data: {
            "ICNumber": $("#ServiceNo").val()
        },
        success: function (response, status) {
            //alert(JSON.stringify(response));
            //var myObj = JSON.parse(response.CourseId.length);
            //alert(myObj);
            $("#Name").val(response.Name);
            $("#ServiceNo").val(response.ServiceNo);
            $("#AadhaarNo").val(response.AadhaarNo);
            $("#Height").val(response.Ht);
        }
    });
}
function getData(id) {  
    let formId = '#' + id;
    // Check if the form exists
    if ($(formId).length === 0) {
        console.error("Form not found.");
        return;
    }
    $("#RegistrationType").prop('required', true);
    $("#ServiceNumber").prop('required', true);
    $.validator.unobtrusive.parse($(formId));
    if ($(formId).valid()) {
        var formData = $(formId).serialize();
        console.log(formData);
    }
     else {
        return false;
    }
    let regType = $("#RegistrationType").find(":selected").val();
    $.ajax({
        url: "/BasicDetail/GetData",
        type: "POST",
        data: {
            "ICNumber": $("#ServiceNumber").val()
        },
        success: function (response, status) {
            if (response.Status==false) {
                alert("Data Not Found.")
            }
            else {
                //alert(JSON.stringify(response));
                $("#Name").val(response.Name);
                $("#ServiceNo").val(response.ServiceNo);
                $("#DOB").val(response.DOB);
                $("#DOB_").val(moment(response.DOB).format("DD-MMM-YYYY"));
                $("#DateOfCommissioning").val(response.DateOfCommissioning);
                $("#DOC").val(moment(response.DateOfCommissioning).format("DD-MMM-YYYY"));
                $("#PermanentAddress").val(response.PermanentAddress);
                $("#RegType").val(regType);
            }
        }
    });
}
function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = '#deleteSpan_' + uniqueId;
    var confirmDeleteSpan = '#confirmDeleteSpan_' + uniqueId;
    if (isDeleteClicked) {
        $(deleteSpan).hide();
        $(confirmDeleteSpan).show();
    }
    else {
        $(deleteSpan).show();
        $(confirmDeleteSpan).hide();
    }
}
function confirmClose(uniqueId, isCloseClicked) {
    var closeSpan = '#closeSpan_' + uniqueId;
    var confirmCloseSpan = '#confirmCloseSpan_' + uniqueId;
    if (isCloseClicked) {
        $(closeSpan).hide();
        $(confirmCloseSpan).show();
    }
    else {
        $(closeSpan).show();
        $(confirmCloseSpan).hide();
    }
}
function ConfirmSave() {
    $('button.btnyes').click(function () {
        $('button.third').removeClass("disabled");
        $(this).addClass("disabled");
    })
    $('button.third').click(function () {
        $('button.second').removeClass("disabled");
        $(this).addClass("disabled");
    })
}
function printDiv() {

    var divToPrint = document.getElementById('DivIdToPrint');

    var newWin = window.open('', 'Print-Window');

    newWin.document.open();

    newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
    newWin.document.write('<link href="" rel="stylesheet" type="text/css" />');
    newWin.document.write('<link rel="\"stylesheet\"" href = "\"../lib/twitter-bootstrap/css/site.css\"" type = "\"text/css\"/" > ' );

    newWin.document.close();

    setTimeout(function () { newWin.close(); }, 10);

}
/* Load Data In Table */
function LoadJDTK(selector) {
    $(document).ready(function () {
        memberTable = $(selector).DataTable({
            retrieve: true,
            lengthChange: false,
            "order": [[2, "asc"]],
            buttons: [{
                extend: 'copy',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            }, {
                extend: 'excel',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            }, {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            }]
        });

        memberTable.buttons().container().appendTo('#myProjectTable_wrapper .col-md-6:eq(0)');

    });
}

function LoadJDT(selector) {
    $(document).ready(function () {
        oTable = $(selector).DataTable({
            /*dom: 'Blfrtip',*/
            dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
            buttons: [
                'excel', 'pdf', 'print'
            ],
            "scrollX": true,
            "processing": true,
            "fixedHeader": true
        });
    });
}
/*Datatable Document by CP for refined Exported Document*/
/* Load Data In Table */
function LoadDOC(selector, title) {
    $(document).ready(function () {
        // Function to convert an img URL to data URL
        function getBase64FromImageUrl(url) {
            var img = new Image();
            img.crossOrigin = "anonymous";
            img.onload = function () {
                var canvas = document.createElement("canvas");
                canvas.width = this.width;
                canvas.height = this.height;
                var ctx = canvas.getContext("2d");
                ctx.drawImage(this, 0, 0);
                var dataURL = canvas.toDataURL("image/png");
                return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
            };
            img.src = url;
        }
        // End Function to convert an img URL to data URL
        oTable = $(selector).DataTable({
            "dom": '<"dt-buttons"Bf><"clear">lirtp',
            "dom": '<"top"<"left-col"Bi><"center-col"l><"right-col"f>>rtp',
            "paging": true,
            "autoWidth": true,

            "buttons": [
                {
                    text: 'Excel',
                    extend: 'excelHtml5',
                },
                {
                    text: 'PDF',
                    extend: 'pdfHtml5',
                   /* filename: 'nhsrc_custom_pdf',*/
                    orientation: 'portrait', //portrait
                    pageSize: 'A4', //A3 , A5 , A6 , legal , letter
                    exportOptions: {
                        columns: ':visible',
                        search: 'applied',
                        order: 'applied'
                    },
                    customize: function (doc) {
                        //Remove the title created by datatTables
                        doc.content.splice(0, 1);
                        //Create a date string that we use in the footer. Format is dd-mm-yyyy
                        var now = new Date();
                        var jsDateTimeBy = now.getDate() + '-' + (now.getMonth('M') + 1) + '-' + now.getFullYear() + ' at ' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds() + ' by ' + $("#hdnID").val();
                        //NHSRC Logo converted to base64, we have used a online converter and paste the string in. Done on http://codebeautify.org/image-to-base64-converter
                        // It's a LONG string scroll down to see the rest of the code !!!
                        var logo = 'data:image/png;base64,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';
                        // A documentation reference can be found at
                        // https://github.com/bpampuch/pdfmake#getting-started
                        // Set page margins [left,top,right,bottom] or [horizontal,vertical] or one number for equal spread It's important to create enough space at the top for a header !!!
                        doc.pageMargins = [20, 60, 20, 30];
                        // Set the font size fot the entire document
                        doc.defaultStyle.fontSize = 7;
                        // Set the fontsize for the table header
                        doc.styles.tableHeader.fontSize = 7;
                        // Create a header object with 3 columns
                        // Left side: Logo
                        // Middle: brandname
                        // Right side: A document title
                        doc['header'] = (function () {
                            return {
                                columns: [
                                    {
                                        image: logo,
                                        width: 24
                                    },
                                    {
                                        alignment: 'left',
                                        italics: true,
                                        text: document.title,
                                        fontSize: 14,
                                        margin: [8, 0],
                                        width: 488
                                    },
                                    {
                                        //alignment: 'right',
                                        image: logo,
                                        width: 24
                                        //fontSize: 14,
                                        //text: title
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
                                        text: ['Generated on: ', { text: jsDateTimeBy.toString() }]
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
                },
                

            ]
        });
    });
}
function LoadDOCCustomPageLength(selector, title, pageLength ,orien, pagesize) {
    $(document).ready(function () {
        // Function to convert an img URL to data URL
        function getBase64FromImageUrl(url) {
            var img = new Image();
            img.crossOrigin = "anonymous";
            img.onload = function () {
                var canvas = document.createElement("canvas");
                canvas.width = this.width;
                canvas.height = this.height;
                var ctx = canvas.getContext("2d");
                ctx.drawImage(this, 0, 0);
                var dataURL = canvas.toDataURL("image/png");
                return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
            };
            img.src = url;
        }
        // End Function to convert an img URL to data URL
        oTable = $(selector).DataTable({
            "pageLength": pageLength,
            "dom": '<"dt-buttons"Bf><"clear">lirtp',
            "dom": '<"top"<"left-col"Bi><"center-col"l><"right-col"f>>rtp',

            "paging": true,
            "autoWidth": true,

            "buttons": [
                {
                    extend: 'print',
                    text: 'Print',
                    orientation: orien,
                    pageSize: pagesize,
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    text: 'Excel',
                    extend: 'excelHtml5',
                    orientation: orien,
                    pageSize: pagesize,
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    text: 'PDF',
                    extend: 'pdfHtml5',
                    /* filename: 'nhsrc_custom_pdf',*/
                    orientation: orien, //portrait
                    pageSize: pagesize, //A3 , A5 , A6 , legal , letter
                    exportOptions: {
                        columns: ':visible',
                        search: 'applied',
                        order: 'applied'
                    },
                    customize: function (doc) {
                        //Remove the title created by datatTables
                        doc.content.splice(0, 1);
                        //Create a date string that we use in the footer. Format is dd-mm-yyyy
                        var now = new Date();
                        var jsDateTimeBy = now.getDate() + '-' + (now.getMonth('M') + 1) + '-' + now.getFullYear() + ' at ' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds() + ' by ' + $("#hdnID").val();
                        //NHSRC Logo converted to base64, we have used a online converter and paste the string in. Done on http://codebeautify.org/image-to-base64-converter
                        // It's a LONG string scroll down to see the rest of the code !!!
                        var logo = 'data:image/png;base64,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';
                        // A documentation reference can be found at
                        // https://github.com/bpampuch/pdfmake#getting-started
                        // Set page margins [left,top,right,bottom] or [horizontal,vertical] or one number for equal spread It's important to create enough space at the top for a header !!!
                        doc.pageMargins = [20, 60, 20, 30];
                        // Set the font size fot the entire document
                        doc.defaultStyle.fontSize = 10;
                        // Set the fontsize for the table header
                        doc.styles.tableHeader.fontSize = 12;
                        // Create a header object with 3 columns
                        // Left side: Logo
                        // Middle: brandname
                        // Right side: A document title
                        doc['header'] = (function () {
                            return {
                                columns: [
                                    {
                                        image: logo,
                                        width: 24
                                    },
                                    {
                                        alignment: 'left',
                                        italics: true,
                                        text: document.title,
                                        fontSize: 14,
                                        margin: [8, 0],
                                        width: 488
                                    },
                                    {
                                        //alignment: 'right',
                                        image: logo,
                                        width: 24
                                        //fontSize: 14,
                                        //text: title
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
                                        text: ['Generated on: ', { text: jsDateTimeBy.toString() }]
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
                },


            ]
        });
    });
}
function LoadDOCCustom(selector, title, args, orien, pagesize) {
    $(document).ready(function () {
        // Function to convert an img URL to data URL
        function getBase64FromImageUrl(url) {
            var img = new Image();
            img.crossOrigin = "anonymous";
            img.onload = function () {
                var canvas = document.createElement("canvas");
                canvas.width = this.width;
                canvas.height = this.height;
                var ctx = canvas.getContext("2d");
                ctx.drawImage(this, 0, 0);
                var dataURL = canvas.toDataURL("image/png");
                return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
            };
            img.src = url;
        }
        // End Function to convert an img URL to data URL
        oTable = $(selector).DataTable({
            "dom": '<"dt-buttons"Bf><"clear">lirtp',
            "dom": '<"top"<"left-col"Bi><"center-col"l><"right-col"f>>rtp',
            "paging": true,
            "autoWidth": true,

            "buttons": [
                {
                    extend: 'print',
                    text: 'Print',
                    orientation: orien,
                    pageSize: pagesize,
                    exportOptions: {
                        columns: args
                    }
                },
                {
                    text: 'Excel',
                    extend: 'excelHtml5',
                    orientation: orien,
                    pageSize: pagesize,
                    exportOptions: {
                        columns : args
                    }
                },
                {
                    text: 'PDF',
                    extend: 'pdfHtml5',
                    /* filename: 'nhsrc_custom_pdf',*/
                    orientation: orien, //portrait
                    pageSize: pagesize, //A3 , A5 , A6 , legal , letter
                    exportOptions: {
                        columns: args,
                        search: 'applied',
                        order: 'applied'
                    },
                    customize: function (doc) {
                        //Remove the title created by datatTables
                        doc.content.splice(0, 1);
                        //Create a date string that we use in the footer. Format is dd-mm-yyyy
                        var now = new Date();
                        var jsDateTimeBy = now.getDate() + '-' + (now.getMonth('M') + 1) + '-' + now.getFullYear() + ' at ' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds() + ' by ' + $("#hdnID").val();
                        //NHSRC Logo converted to base64, we have used a online converter and paste the string in. Done on http://codebeautify.org/image-to-base64-converter
                        // It's a LONG string scroll down to see the rest of the code !!!
                        var logo = 'data:image/png;base64,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';
                        // A documentation reference can be found at
                        // https://github.com/bpampuch/pdfmake#getting-started
                        // Set page margins [left,top,right,bottom] or [horizontal,vertical] or one number for equal spread It's important to create enough space at the top for a header !!!
                        doc.pageMargins = [20, 60, 20, 30];
                        // Set the font size fot the entire document
                        doc.defaultStyle.fontSize = 10;
                        // Set the fontsize for the table header
                        doc.styles.tableHeader.fontSize = 12;
                        // Create a header object with 3 columns
                        // Left side: Logo
                        // Middle: brandname
                        // Right side: A document title
                        doc['header'] = (function () {
                            return {
                                columns: [
                                    {
                                        image: logo,
                                        width: 24
                                    },
                                    {
                                        alignment: 'left',
                                        italics: true,
                                        text: document.title,
                                        fontSize: 14,
                                        margin: [8, 0],
                                        width: 488
                                    },
                                    {
                                        //alignment: 'right',
                                        image: logo,
                                        width: 24
                                        //fontSize: 14,
                                        //text: title
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
                                        text: ['Generated on: ', { text: jsDateTimeBy.toString() }]
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
                },


            ]
        });
    });
}
/*With Custom Columns*/
function LoadJDTWithCustomCols(selector, args) {
    $(document).ready(function () {
        oTable = $(selector).DataTable({
            /*dom: 'Blfrtip',*/
            dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
            buttons: [
                {
                    extend: 'excel',
                    text: 'Excel',
                    exportOptions: {
                        columns: args
                    }
                },
                {
                    extend: 'pdf',
                    text: 'PDF',
                    exportOptions: {
                        columns: args
                    }
                },
                {
                    extend: 'print',
                    text: 'Print',
                    exportOptions: {
                        columns: args
                    }
                }
            ],

            //buttons: [
            //    'excel', 'pdf', 'print'
            //],

            "scrollX": true,
            "processing": true,
            "fixedHeader": true
        });
    });
}
/*With Custom*/
function LoadJDTCustom(selector, args,orien,pagesize) {
    $(document).ready(function () {
        oTable = $(selector).DataTable({
            /*dom: 'Blfrtip',*/
            dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
            buttons: [
                {
                    extend: 'excel',
                    text: 'Excel',
                    orientation: orien,
                    ageSize: pagesize,
                    exportOptions: {
                        columns: args
                    }
                },
                {
                    extend: 'pdf',
                    text: 'PDF',
                    orientation: orien,
                    pageSize: pagesize,
                    exportOptions: {
                        columns: args
                    }
                },
                {
                    extend: 'print',
                    text: 'Print',
                    exportOptions: {
                        columns: args
                    }
                }
            ],

            //buttons: [
            //    'excel', 'pdf', 'print'
            //],

            "scrollX": true,
            "processing": true,
            "fixedHeader": true
        });
    });
}

function LoadJDTSorted(selector) {
    $(document).ready(function () {
        oTable = $(selector).DataTable({
            dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
            buttons: [
                'excel', 'pdf', 'print'
            ],
            "scrollX": true,
            "processing": true,
            "fixedHeader": true,
            "order": [[0, "desc"]],
            "columnDefs": [{ "targets": [0], "visible": false, "searchable": false }]
        });
    });
}
function LoadJDTSimple(selector) {
    $(document).ready(function () {
        oTable = $(selector).DataTable({
            "scrollX": true,
            "processing": true,
            "fixedHeader": true,
        });
    });
}
//Sorted with Doc Export
function LoadJDTSortedExport(selector) {
    $(document).ready(function () {
        oTable = $(selector).DataTable({
            dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
            buttons: [
                'excel', 'pdf', 'print'
            ],
            "scrollX": true,
            "processing": true,
            "order": [[0, "desc"]],
            "columnDefs": [{ "targets": [0], "visible": false, "searchable": false }],
            dom: '<"top"iflp<"clear">>rt<"bottom"iflp<"clear">>',
        });
    });
}
/* Perform Delete Operation in Jquery Datatable */
function DeleteJDTRow(id, event, selector, Jsonurl) {
    if (confirm("Are you sure you want to delete this record...?")) {
        //var row = $(this).closest("tr");
        oTable = $(selector).DataTable();
        //var parent = $(this).parent('td').parent('tr');
        var parent = $(event).parents('tr');
        $.ajax({
            type: "POST",
            url: Jsonurl,
            data: '{id: ' + id + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data == "Deleted") {
                    alert("Record Deleted !");
                    $(selector).DataTable().row(parent).remove().draw(false);
                }
                else {
                    alert("Something Went Wrong!");
                }
            }
        });
    }
}
/*-----------Date Picker End---------- */
/*-----------Datatable with custom Date search*/

function LoadJDTSortedDate(selector) {
    $(document).ready(function () {
        oTable = $(selector).DataTable({
            "scrollX": true,
            "processing": true,
            "order": [[0, "desc"]],
            "columnDefs": [{ "targets": [0], "visible": false, "searchable": false }],
            "oLanguage": {
                "sSearch": "Filter Data"
            },

        });

        $("#datepicker_from").datepicker({
            showOn: "button",
            buttonImage: "images/calendar.gif",
            buttonImageOnly: false,
            "onSelect": function (date) {
                minDateFilter = new Date(date).getTime();
                oTable.fnDraw();
            }
        }).keyup(function () {
            minDateFilter = new Date(this.value).getTime();
            oTable.fnDraw();
        });

        $("#datepicker_to").datepicker({
            showOn: "button",
            buttonImage: "images/calendar.gif",
            buttonImageOnly: false,
            "onSelect": function (date) {
                maxDateFilter = new Date(date).getTime();
                oTable.fnDraw();
            }
        }).keyup(function () {
            maxDateFilter = new Date(this.value).getTime();
            oTable.fnDraw();
        });
    });
}

// Date range filter
//minDateFilter = "";
//maxDateFilter = "";
//$(document).ready(function () {
//    $("#show_hide_password a").on('click', function (event) {
//        event.preventDefault();
//        if ($('#show_hide_password input').attr("type") == "text") {
//            $('#show_hide_password input').attr('type', 'password');
//            $('#show_hide_password i').addClass("fa-eye-slash");
//            $('#show_hide_password i').removeClass("fa-eye");
//        } else if ($('#show_hide_password input').attr("type") == "password") {
//            $('#show_hide_password input').attr('type', 'text');
//            $('#show_hide_password i').removeClass("fa-eye-slash");
//            $('#show_hide_password i').addClass("fa-eye");
//        }
//    });
//});
/* Load Menu Tree View */
/* Param  selector== DivId*/
/* Param  Jsonurl== UrlActionMethod*/
function LoadMenuTreeView(selector, Jsonurl) {
    $(document).ready(function () {
        $(selector).jstree({
            'core': {
                'data': {
                    'url': Jsonurl,
                    'data': function (node) {
                        return { 'id': node.id };
                    }
                }
            }
        });
    });
}
/* Load Menu Tree View */
/* Param  selector== DivId*/
/* Param  Jsonurl== UrlActionMethod*/
function LoadMenuTreeViewContext(selector, Jsonurl) {
    $(document).ready(function () {
        $(selector).jstree({
            plugins: ["contextmenu"],
            'core': {
                "opened": true, "selected": true,
                "check_callback": true,
                "themes": { "stripes": true },
                'data': {
                    'url': Jsonurl,
                    'data': function (node) {
                        return { 'id': node.id };
                    }
                }
            },
            "contextmenu":
            {
                "items":
                {
                    opt1:
                    {
                        label: "Add Page",
                        action: function (data) {
                            //alert(data.reference.parent().attr('id'))
                            AddPage(data.reference.parent().attr('id'));
                        }
                    },

                    opt2:
                    {
                        label: "Edit Page",
                        action: function (data) {
                            EditPage(data.reference.parent().attr('id'))
                        }
                    }
                }
            },
        });
    });
}

/* Get Menu Tree View NodeId */
/* Param  selector== DivId*/
/* Param  NodeId== Related NodeId*/
function ChangeMenuTreeView(selector, NodeId) {
    $(document).ready(function () {
        $(selector).on('changed.jstree', function (e, data) {
            var id = $(selector).jstree('get_selected');
            //alert(id);
            $(NodeId).val(id);
            // $('#event_result').html('Selected: ' + r.join(', '));
        }).jstree();
    })
}

/*
 Home Page Scripts
 */
