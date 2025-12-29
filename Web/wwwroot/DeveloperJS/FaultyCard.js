var table; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData();
    $("#btnAdd").on("click",function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(FaultyCardRequest);
    });
});
function BindData() {
    $("#tbldata").DataTable().destroy();
    table = $("#tbldata").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
            };
            try {
                let response = await fetch("/BasicDetail/GetAllFaulty", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken,
                    },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            // Serial number column
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "ServiceNo",
                name: "ServiceNo",
                render: function (data, type, row) {
                    // Check if first two characters are alphabets
                    if (/^[A-Za-z]{2}/.test(data)) {
                        // Insert space after first two characters
                        return data.slice(0, 2) + ' ' + data.slice(2);
                    } else {
                        // No space needed
                        return data;
                    }
                }
            },
            {
                data: null,
                name: "FromName",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                    return (fullName);
                }
            },
            {
                data: "UnitAbbreviation",
                name: "UnitAbbreviation",
                orderable: false,
            },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: "RemarksIds",
                name: "RemarksIds",
                orderable: false,
                render: function (data, type, row) {
                    if (data != null) {
                        return `<button type='button' class='cls-remarks btn btn-icon btn-round btn-warning mr-1'><i class='fa fa-eye'></i></button>`;
                    }
                    else {
                        return `NA`;
                    }
                    return data;
                }
            },
            {
                data: "FromRemark",
                name: "FromRemark",
                render: function (data, type, row) {
                    if (data != null) {
                        let sentence = data;
                        let words = sentence.split(" ");

                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                        return `<span class='cls-FromRemark'>${truncatedSentence}</span>`;
                    } else {
                        return `NA`;
                    }

                }
            },
            {
                data: "ToRemark",
                name: "ToRemark",
                render: function (data, type, row) {
                    if (data != null) {
                        let sentence = data;
                        let words = sentence.split(" ");

                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                        return `<span class='cls-ToRemark'>${truncatedSentence}</span>`;
                    } else {
                        return `NA`;
                    }

                }
            },
            // Additional column for Edit action
            {
                data: "IsEditAction",
                name: "Action",
                orderable: false,
                render: function (data, type, row) {
                    if (data == false) {
                        return `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button>`;
                    } else {
                        return `NA`;
                    }
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip", // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                title: 'E-IASC_MapUnitChange',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.EncryptedId != null) {
                    sessionStorage.setItem("ArmyNo", rowData.ServiceNo);
                    window.location.href = '/BasicDetail/FaultyCardRequest?Id=' + encodeURIComponent(rowData.EncryptedId);
                }
            });

            $("#tbldata tbody").off("click", ".cls-remarks").on("click", ".cls-remarks", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RemarksIds != null) {
                    GetRemarksData(rowData.RemarksIds);
                }
            });


            $("#tbldata tbody").off("click", ".cls-FromRemark").on("click", ".cls-FromRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('Remark');
                    $("#MessageDialogBody").html(rowData.FromRemark);
                    $("#MessageDialog").modal('show');
                }
            });

            $("#tbldata tbody").off("click", ".cls-ToRemark").on("click", ".cls-ToRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('AFSAC Remark');
                    $("#MessageDialogBody").html(rowData.ToRemark);
                    $("#MessageDialog").modal('show');
                }
            });

        }
    });
    if ($("#spnClaimValue").html().toLowerCase() === "true") {
        table.column(8).visible(true);
    }
    else {
        table.column(8).visible(false);
    }
}

async function GetRemarksData(remarksRemarksIds) {

    let param = new URLSearchParams({ RemarksIds: remarksRemarksIds });

    try {
        const response = await fetch('/BasicDetail/GetRemarksData', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (result != null) {
            var remarksArray = result.split('#');
            if (remarksArray != null) {
                var listItem="";
                listItem += "<ul>";
                for (var j = 0; j < remarksArray.length; j++) {
                    listItem += "<li>" + remarksArray[j] + "</li>";
                }
                listItem += "</ul>";
                $("#MessageDialogLabel").html('Reason');
                $("#MessageDialogBody").html(listItem);
                $("#MessageDialog").modal('show');
            }

        } else {
            toastr.error('Invalid Input.');
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}