function SetCompletedHistoryHeader(requestId) {

    let PdfbuttonHtml = `
        <button type="button"
                class="cls-btndownloadpdf btn btn-danger"
                data-request-id="${requestId}"
                data-toggle="tooltip"
                data-placement="top"
                title="Download Details">
            <i class="fas fa-file-pdf"></i>
        </button>`;

    let header = `I Card History ${PdfbuttonHtml}`;

    $("#exampleModalLabel_BasicDetailCompletedHistory").html(header);
}

function GetCompletedHistoryByRequestId(RequestId) {

    var userdata = {
        "Request": encryptPayloadData(RequestId),
    };

    $.ajax({
        url: '/BasicDetail/GetCompletedHistory',
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

                $("#CompletedHistory_BasicDetail_Data").html(applicantDetailsHtml);

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
                            .map(posting => createPostingOutHtml_CompletedHistory(posting))
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
                            .map(posting => createPostingOutHtml_CompletedHistory(posting))
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

                $("#CompletedHistory_Fwd_Details").html(Fwd_Details);


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
                $("#CompletedHistory_Dispatch_Details").html(CardMovement_Details);

                $("#BasicDetailCompletedHistory").modal("show");
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

function createPostingOutHtml_CompletedHistory(posting) {
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

function GetCompletedHistoryPdf(RequestId) {
    try {
        const encryptedRequest = encryptPayloadData(RequestId);

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/BasicDetail/GetCompletedHistoryPdf';
        form.target = '_blank';
        form.style.display = 'none';

        const requestInput = document.createElement('input');
        requestInput.type = 'hidden';
        requestInput.name = 'Request';
        requestInput.value = encryptedRequest;
        form.appendChild(requestInput);

        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = globalThis.RequestVerificationToken;
        form.appendChild(tokenInput);

        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    } catch (e) {
        Swal.fire({
            text: errormsg002
        });
    }
}