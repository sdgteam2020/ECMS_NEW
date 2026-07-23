let table_ClosedHistory;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldatatabledata_ClosedHistory');

    let UserType = ($("#spnType").html());
    let ApplyForId = $("#spnJCOOR").html();
    BindData(UserType, ApplyForId, function () {
        // Reset global variables as explained
        globalThis.selectedIds = [];
        globalThis.previousSearchText = "";
        globalThis.isFirstSelectAll = true;
        globalThis.searchChanged = false;
        globalThis.globalAllChecked = false;
    });
    $(window).on('resize', function () {
        // Check if element exists AND is a DataTable
        if ($('#tbldatatabledata_ClosedHistory').length && $.fn.DataTable.isDataTable('#tbldatatabledata_ClosedHistory')) {
            $('#tbldatatabledata_ClosedHistory').DataTable().columns.adjust();
        }
    });
});

function BindData(UserType, ApplyForId) {
    globalThis.selectedIds = [];

    if ($.fn.DataTable.isDataTable("#tbldatatabledata_ClosedHistory")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_ClosedHistory").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_ClosedHistory thead").empty(); // Clear old thead
        $("#tbldatatabledata_ClosedHistory tbody").empty(); // Clear old tbody
    }


    const columns = getColumnsForClosedHistory(UserType, ApplyForId);
    table_ClosedHistory = $("#tbldatatabledata_ClosedHistory").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        scroller: true,           // ✅ Enable virtual scrolling for better performance
        deferScroll: true,        // ✅ Improve scrolling performance
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false, //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        order: [[2, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let searchStatus = getSearchStatusForBindData(data.search.value);

            // Clear old selectedIds on search change, but keep globalAllChecked state
            if (searchStatus.searchChanged) {
                globalThis.selectedIds = [];

                // Mark for re-fetch if needed
                if (globalThis.globalAllChecked) {
                    globalThis.isFirstSelectAll = true;
                }
            }

            // ✅ Determine if a fetch is needed
            const shouldFetchSelectedIds =
                globalThis.globalAllChecked && (globalThis.isFirstSelectAll || searchStatus.searchChanged) ||
                (!globalThis.globalAllChecked && searchStatus.searchChanged && globalThis.isFirstSelectAll);

            // If fetch is needed, manually set searchChanged to true
            if (shouldFetchSelectedIds) {
                searchStatus.searchChanged = true; // Manually set to true to ensure data fetch
            }


            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: searchStatus.currentSearchText,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order               
                CValue:0,
                UserType: UserType,              
                ApplyForId: ApplyForId,
                searchTextChanged: searchStatus.searchChanged
            };
            //alert(UserType)
            //alert(ApplyForId)
            let encryptedPayload = "";
            if (requestData) {
                const jsonData = JSON.stringify(requestData);
                encryptedPayload = encryptPayloadData(jsonData);

            }
            try {
                let response = await fetch("/BasicDetail/GetAllClosedHistory", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify({ data: encryptedPayload })
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();

                // 🔁 If no data returned, always clear selection
                if (result.data.length === 0) {
                    globalThis.selectedIds = [];
                    console.log("No results. Cleared selectedIds.");
                }
       
                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
        columnDefs: [
            {
                targets: '_all',
                orderSequence: ["asc", "desc"]  // Only global settings
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Appl ID" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip", // Add buttons to the DOM
        
        // 👇 Show modal only after table (header + data) is fully rendered
        initComplete: function () {
            if (typeof callback === "function") {
                callback(); // show modal now
            }
            // Force DataTables to calculate optimal widths
            this.api().columns.adjust();

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table_ClosedHistory.columns.adjust().responsive.recalc();
                }, 100);
            });
        },
        drawCallback: function (settings) {

            // Recalculate widths on each data load
            this.api().columns.adjust().responsive.recalc();

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            tooltipTriggerList.forEach(el => {
                new bootstrap.Tooltip(el);
            });
   
            $("#tbldatatabledata_ClosedHistory tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table_ClosedHistory.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetClosedHistoryByRequestId(rowData.RequestId);
                }
            });
        }
    });
    table_ClosedHistory.button('.buttons-copy').nodes().hide();
    table_ClosedHistory.button('.buttons-csv').nodes().hide();
    table_ClosedHistory.button('.buttons-print').nodes().hide();
    
}

function getColumnsForClosedHistory(UserType, ApplyForId) {
    let columns = [];
    switch (ApplyForId) {
        case "0":
            columns = [
                
                // Serial number column
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    className: "text-center col-sno",
                    width: "30px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl Id",
                    data: "RequestId",
                    name: "ApplId",
                    className: "nowrap",
                    width: "60px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    orderable: false,
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
                    title: "Rank & Name",
                    data: null,
                    name: "Name",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.Name || ""}`.trim();
                        if (!fullName) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                    }
                },
                {
                    title: "Reason",
                    data: "Reason",
                    name: "Reason",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Closed On",
                    data: "ClosedOn",
                    name: "ClosedOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Remark",
                    data: "Remarks",
                    name: "Remarks",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Auth",
                    data: "Authority",
                    name: "Authority",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: `<div>History</div>`,
                    className: "noExport",
                    width: "100px",
                    data: null,
                    name: "History",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                    }
                },                                                    
            ];
            break;
        default:
            columns = [
                
                // Serial number column
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    className: "text-center col-sno",
                    width: "30px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl Id",
                    data: "RequestId",
                    name: "ApplId",
                    className: "nowrap",
                    width: "60px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    orderable: false,
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
                    title: "Rank & Name",
                    data: null,
                    name: "Name",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.Name || ""}`.trim();
                        if (!fullName) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                    }
                },
                {
                    title: "Reason",
                    data: "Reason",
                    name: "Reason",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Closed On",
                    data: "ClosedOn",
                    name: "ClosedOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Remark",
                    data: "Remarks",
                    name: "Remarks",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Auth",
                    data: "Authority",
                    name: "Authority",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                                           
                {
                    title: `<div>History</div>`,
                    className: "noExport",
                    width: "100px",
                    data: null,
                    name: "History",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                    }
                },
               
            ];
    }
    return columns;
}
function getSearchStatusForBindData(search) {
    const currentSearchText = search.trim();

    // Ensure searchChanged is only true when the actual search field or text changes.
    globalThis.searchChanged = (
        (currentSearchText !== globalThis.previousSearchText)
    );

    // Update previous values after comparison
    globalThis.previousSearchText = currentSearchText;

    return {
        searchChanged: globalThis.searchChanged,
        currentSearchText
    };
}


function GetClosedHistoryByRequestId(RequestId) {
    var userdata = {
        "RequestId": encryptPayloadData(RequestId),
    };
    $.ajax({
        url: '/BasicDetail/GetClosedHistoryByRequestId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
          
            if (response.Result == true) {
                let listItem = "";
                let listItem2 = "";
                let applicantDetailsHtml;
                let BasicDetail = response.Value.BasicDetail;
                let ICardHistory = response.Value.ICardHistory;
                let PostingOut = response.Value.PostingOut;
                let FaultyCard = response.Value.FaultyCard;
                let CardMovement = response.Value.CardMovement;
                let CloseCard = response.Value.CloseCard;

                const photoSource = BasicDetail.PhotoInBase64
                    ? BasicDetail.PhotoInBase64
                    : "/Images/user4.png";

                const signatureSource = BasicDetail.SignatureInBase64
                    ? BasicDetail.SignatureInBase64
                    : "/Images/Signature.png";

                const buildAddress = function () {
                    return [
                        BasicDetail.Village,
                        BasicDetail.Tehsil,
                        BasicDetail.PO,
                        BasicDetail.PS,
                        BasicDetail.District,
                        BasicDetail.State,
                        BasicDetail.PinCode
                    ]
                        .filter(value =>
                            value !== null &&
                            value !== undefined &&
                            value.toString().trim() !== ""
                        )
                        .join(", ");
                };
                applicantDetailsHtml = `
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="feature-box3 h-100 shadow-lg pr-3 pl-3 bg-body rounded">

                                    <div class="top-block_ind d-flex">
                                        <div class="text-block">
                                            <h5 class="mb-1 text-font2 font-weight600">
                                                Applicant’s Details
                                            </h5>
                                        </div>
                                    </div>

                                    <div class="row pr-2 pl-2">

                                        <div class="col-sm-9">

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label col-5 labelprofile text-left">
                                                    Name As Per Record
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.NameAsPerRecord ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label col-5 labelprofile text-left">
                                                    First Name
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.FName ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label col-5 labelprofile text-left">
                                                    Last Name
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.LName ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Rank
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.RankName ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Arm / Service
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.ArmedName ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Army No
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.ServiceNo ?? ""}
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Card Serial No
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.CardSerialNo}
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Chip No
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.ChipNo}
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Date of Birth
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${DateFormatedd_mm_yyyy_no_time(BasicDetail.DOB)}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Height (Cm)
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.Height ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    AADHAAR No
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.AadhaarNo ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Blood Group
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.BloodGroup ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Place of Issue
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.PlaceOfIssue ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Date of Issue
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${DateFormateddMMyyyyhhmmss(BasicDetail.DateOfIssue)}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Issuing Authority
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${BasicDetail.IssuingAuthorityName ?? ""}
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <label class="col-form-label labelprofile col-5 text-left">
                                                    Date of Commissioning / Enrollment
                                                </label>
                                                <div class="col-7">
                                                    <label class="form-control-plaintext text-uppercase">
                                                        ${DateFormatedd_mm_yyyy_no_time(BasicDetail.DateOfCommissioning)}
                                                    </label>
                                                </div>
                                            </div>

                                        </div>

                                        <div class="col-sm-3">

                                            <div class="form-group row mb-3">
                                                <div class="col-12 text-center">
                                                    <img src="${photoSource}"
                                                            width="100"
                                                            class="border border-primary p-2"
                                                            onerror="this.onerror=null;this.src='/Images/user4.png';" />
                                                </div>
                                            </div>

                                            <div class="form-group row mb-0">
                                                <div class="col-12 text-center">
                                                    <img src="${signatureSource}"
                                                            width="100"
                                                            height="50"
                                                            class="border border-primary p-2"
                                                            onerror="this.onerror=null;this.src='/Images/Signature.png';" />
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col-sm-12">

                                        <div class="form-group row mb-0">
                                            <label class="col-form-label labelprofile col-3 text-left">
                                                Identification Mark
                                            </label>
                                            <div class="col-9">
                                                <label class="form-control-plaintext text-uppercase ml-4">
                                                    ${BasicDetail.IdenMark1 ?? ""}
                                                </label>
                                            </div>
                                        </div>

                                        <div class="form-group row mb-0">
                                            <label class="col-form-label labelprofile col-3 text-left">
                                                Permt Address as per Service Records
                                            </label>
                                            <div class="col-9">
                                                <label class="form-control-plaintext text-uppercase ml-4">
                                                    ${buildAddress()}
                                                </label>
                                            </div>
                                        </div>

                                    </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;

                $("#ClosedHistory_BasicDetail_Data").html(applicantDetailsHtml);

                if (ICardHistory?.length > 0) {

                    listItem = ICardHistory.map((item, index) => {

                        const isFirstItem = index === 0;
                        const isLastItem = index === ICardHistory.length - 1;

                        let badgeClass = "bg-success";
                        let statusText = "";

                        if (item.IsComplete === 0 && item.Status === "Pending") {
                            statusText = "Pending And Sent To";
                        } else if (item.Status === "Approved") {
                            statusText = "Approved And Sent To";
                        } else if (item.Status === "Reject") {
                            badgeClass = "bg-danger";
                            statusText = "Reject And Sent To";
                        } else if (item.Status === "Internal Forward") {
                            statusText = "Internal Forward And Sent To";
                        }

                        const statusHtml = statusText
                            ? `
                                <br>
                                <span class="badge ${badgeClass}">
                                    ${statusText}
                                </span>
                              `
                            : "";

                        const remarks2Html = item.Remarks2
                            ? `
                                <ul>
                                    ${item.Remarks2
                                .split("#")
                                .filter(x => x.trim() !== "")
                                .map(x => `<li>${x}</li>`)
                                .join("")}
                                </ul>
                              `
                            : "";

                        /*
                         * Posting Out created before the first forward movement.
                         * These records have TrnFwdId = 0.
                         */
                        const postingOutBeforeForward = isFirstItem
                            ? (PostingOut ?? []).filter(
                                p => Number(p.TrnFwdId) === 0
                            )
                            : [];

                        const postingOutBeforeHtml = postingOutBeforeForward
                            .map(posting => createPostingOutHtml(posting))
                            .join("");

                        const initialSubmissionHtml = isFirstItem
                            ? `
                                <div class="timeline-item">
                                    <div class="timeline-item-marker">

                                        <div class="timeline-item-marker-text">
                                            <span class="badge bg-success">
                                                ${DateFormateddMMyyyyhhmmss(item.UpdatedOn)}
                                            </span>
                                        </div>

                                        <div class="timeline-item-marker-indicator bg-primary"></div>
                                    </div>

                                    <div class="timeline-item-content">
                                        I-Card Submit By -
                                        ${item.FromDomain ?? ""}
                                        (${item.FromRank ?? ""} ${item.FromProfile ?? ""})

                                        ${postingOutBeforeHtml}
                                    </div>
                                </div>
                              `
                            : "";

                        /*
                         * Posting Out created after this particular forward movement.
                         * It is matched with the current history TrnFwdId.
                         * TrnFwdId = 0 is excluded to prevent duplication.
                         */
                        const postingOutAfterForward = (PostingOut ?? []).filter(
                            posting =>
                                Number(posting.TrnFwdId) !== 0 &&
                                Number(posting.TrnFwdId) === Number(item.TrnFwdId)
                        );

                        const postingOutAfterHtml = postingOutAfterForward
                            .map(posting => createPostingOutHtml(posting))
                            .join("");

                        const faultyCardItem = (FaultyCard ?? []).find(
                            faulty =>
                                Number(faulty.TrnFwdId) === Number(item.TrnFwdId)
                        );

                        const faultyRemarksHtml = faultyCardItem?.RemarksNameList
                            ? `
                                <ul>
                                    ${faultyCardItem.RemarksNameList
                                .split("#")
                                .filter(x => x.trim() !== "")
                                .map(x => `<li>${x}</li>`)
                                .join("")}
                                </ul>
                              `
                            : "";

                        const faultyCardHtml = faultyCardItem
                            ? `
                                <br>
                                <div class="arrow-icon-box">
                                    <i class="fas fa-arrow-down"></i>
                                </div>

                                <strong class="text-danger text-decoration-underline">
                                    Faulty Card
                                </strong>

                                <br>
                                <span class="text-danger">Reason</span>

                                ${faultyRemarksHtml}

                                By :- ${faultyCardItem.FaultyStage ?? ""}
                              `
                            : "";

                        const pendingFromHtml = item.IsComplete === 0
                            ? `
                                <br>
                                <span class="badge bg-warning">
                                    Pending from
                                </span>
                              `
                            : "";

                        const closeCardHtml = isLastItem && CloseCard
                            ? `
                                <br>
                                <div class="arrow-icon-box">
                                    <i class="fas fa-arrow-down"></i>
                                </div>

                                <strong class="text-danger">
                                    Appl Close
                                </strong>

                                <br>
                                Reason :- ${CloseCard.Reason ?? ""}
                              `
                            : "";

                        const movementHtml = `
                            <div class="timeline-item">
                                <div class="timeline-item-marker">

                                    <div class="timeline-item-marker-text">
                                        <span class="badge ${badgeClass}">
                                            ${DateFormateddMMyyyyhhmmss(item.UpdatedOn)}
                                        </span>
                                    </div>

                                    <div class="timeline-item-marker-indicator bg-primary"></div>
                                </div>

                                <div class="timeline-item-content">

                                ${item.FromDomain ?? ""}
                                (${item.FromRank ?? ""} ${item.FromProfile ?? ""})

                                ${statusHtml}

                                <br>
                                <strong>Remark</strong>
                                <br>
                                ${item.Remark ?? ""}

                                ${remarks2Html}

                                <br>
                                <div class="arrow-icon-box">
                                    <i class="fas fa-arrow-down"></i>
                                </div>

                                ${pendingFromHtml}

                                <br>
                                ${item.ToDomain ?? ""}
                                (${item.ToRank ?? ""} ${item.ToProfile ?? ""})

                                <!-- Posting Out shown at end of this forward movement -->
                                ${postingOutAfterHtml}

                                ${faultyCardHtml}
                                ${closeCardHtml}

                            </div>
                        </div>
                    `;

                        return initialSubmissionHtml + movementHtml;

                    }).join("");
                }

                let Fwd_Details = `<div class="row">
                                       <div class="col-sm-12">
                                            <div class="card">
                                                <div class="card-header">
                                                    <h4>I-Card Application History</h4>
                                                </div>
                                                <div class="card-body">
                                                    <div><h5 class="text-center badge badge-success">Step-by-step I Card Application History</h5></div>
                                                    <div class="timeline timeline-xs">
                                                        ${listItem}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>`;
                $("#ClosedHistory_Fwd_Details").html(Fwd_Details);


                if (CardMovement?.length > 0) {

                    listItem2 = CardMovement.map((item, index) => {

                        const isLostOrHoltist = item.StepName === "I-Card Lost" || item.StepName === "I-Card Holtist";

                        const badgeClass = isLostOrHoltist ? "bg-danger" : "bg-success";

                        const arrowHtml = index < CardMovement.length - 1
                            ? `
                                <br>
                                <div class="arrow-icon-box">
                                    <i class="fas fa-arrow-down"></i>
                                </div>
                              `
                            : "";

                        return `
                            <div class="timeline-item">
                                <div class="timeline-item-marker">
                                    <div class="timeline-item-marker-text">
                                        <span class="badge ${badgeClass}">
                                            ${DateFormateddMMyyyyhhmmss(item.ReportedOn)}
                                        </span>
                                    </div>

                                    <div class="timeline-item-marker-indicator bg-primary"></div>
                                </div>

                                <div class="timeline-item-content">
                                    <span class="badge ${badgeClass}">
                                        ${item.StepName ?? ""}
                                    </span>
                                    <br>

                                    by ${item.ReportedBy ?? ""}

                                    <br>
                                    <strong class="text-center">Remark</strong>
                                    <br>
                                    ${item.Remark ?? ""}

                                    ${arrowHtml}
                                </div>
                            </div>
                            `;
                    }).join("");
                } else {

                }

                let CardMovement_Details = `<div class="row">
                                               <div class="col-sm-12">
                                                    <div class="card">
                                                        <div class="card-header">
                                                            <h4>I-Card History</h4>
                                                        </div>
                                                        <div class="card-body">
                                                            <div><h5 class="text-center badge badge-success">Step-by-step I-Card History</h5></div>
                                                            <div class="timeline timeline-xs">
                                                                ${listItem2}
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>`;
                $("#ClosedHistory_Dispatch_Details").html(CardMovement_Details);

                $("#BasicDetailClosedHistory").modal("show");
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.Message,

                })
            }
        }

    });
}

function createPostingOutHtml(posting) {
    return `
        <div class="posting-out-details">
            <br>

            <div class="arrow-icon-box">
                <i class="fas fa-arrow-down"></i>
            </div>

            <strong class="text-danger">
                ${posting.Reason ?? "Posting Out"}
            </strong>

            <br>
            <span class="text-info">From Unit</span>
            <br>
            ${posting.FromUnit ?? ""}

            <br>
            <span class="text-info">To Unit</span>
            <br>
            ${posting.UnitName ?? ""}
        </div>
    `;
}