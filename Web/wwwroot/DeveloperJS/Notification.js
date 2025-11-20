var table_Fwd; // Declare table variable outside the function to preserve the instance
$(function () {
    BindData(function () {
    });
});
function BindData() {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    if ($.fn.DataTable.isDataTable("#tbldatatabledata_Notification")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_Notification").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_Notification thead").empty(); // Clear old thead
        $("#tbldatatabledata_Notification tbody").empty(); // Clear old tbody
    }

    table_Fwd = $("#tbldatatabledata_Notification").DataTable({
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
                sortDirection: data.order.length > 0 ? data.order[0].dir : '' // Add a check for data.order
            };
            try {
                let response = await fetch("/Home/GetAllNotificationData", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify(requestData)
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
                title: "Sno",
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                title: "Date",
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                title: "Appl ID",
                data: "ApplId",
                name: "ApplId",
            },
            {
                title: "ServiceNo",
                data: "ServiceNo",
                name: "ServiceNo",
                render: function (data, type, row) {
                    // Check if first two characters are alphabets
                    if (/^[A-Za-z]{2}/.test(data)) {
                        // Insert space after first two characters
                        return `${data.slice(0, 2) + ' ' + data.slice(2)}`;

                    } else {
                        // No space needed
                        return `${data}`;;
                    }
                }
            },
            {
                title: "Rank & Name",
                data: null,
                name: "Name",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankAbbreviation || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                    return (fullName);
                }
            },
            {
                title: "Message",
                data: "Message",
                name: "Message"
            },
            {
                title: "Action Link",
                className: "noExport",
                data: "Url",
                name: "Url",
                orderable: false,
                render: function (data, type, row) {

                    if (data != "") {
                        return `<a href="${data}" class="btn btn-round btn-warning mr-2">Redirect Page</a>`;
                    }
                    else {
                        return '';
                    }
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Appl ID" // Add custom placeholder
        },
        dom: 'lBfrtip', // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'portrait',
                pageSize: 'A4', //A3 , A5 , A6 , legal , letter
                title: 'E-IASC_Appl',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
        }
    });
}