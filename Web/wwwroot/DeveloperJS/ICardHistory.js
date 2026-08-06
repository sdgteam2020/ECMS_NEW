$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
});
function GetRequestHistory(spnRequestId) {
    var userdata = {

        "Request": encryptPayloadData(spnRequestId),


    };
    var listItem = "";
    $.ajax({
        url: '/BasicDetail/GetRequestHistory',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
              
                let  ICardHistory = response.ICardHistory;
                let  PostingOut = response.PostingOut;
                let  FaultyCard = response.FaultyCard;
                let CloseCard = response.CloseCard;

                //if (ICardHistory?.length > 0) {

                //    for (var i = 0; i < ICardHistory.length; i++) {
                //        if (i == 0) {
                //            listItem += '<div class="timeline-item">';
                //            listItem += '<div class="timeline-item-marker">';
                //            listItem += '<div class="timeline-item-marker-text "><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                //            listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                //            listItem += '</div>';
                //            listItem += '<div class="timeline-item-content">';
                //            listItem += 'I-Card Submit By -' + ICardHistory[i].FromDomain + '(' + ICardHistory[i].FromRank + ' ' + ICardHistory[i].FromProfile + ')';
                //            if (i == 0) {
                //                // Filter PostingOut based on matching TrnFwdId
                //                let PostingOut1 = PostingOut.filter(p => p.TrnFwdId == 0);

                //                // var PostingOut = PostingOut.filter(i => i.TrnFwdId == ICardHistory[i].TrnFwdId)
                //                if (PostingOut1.length > 0) {
                //                    listItem += '<br><div class="arrow-icon-box"><i class="fas fa-arrow-down"></i></div>'
                //                    listItem += '<br> <strong class="text-center text-danger">' + PostingOut1[0].Reason + '</strong> <br> <span class="text-info">From Unit </span>  <br>' + PostingOut1[0].FromUnit + ' <br> <span class="text-info">To Unit </span>  <br>' + PostingOut1[0].UnitName + '';
                //                }
                //            }
                //            listItem += '</div>';
                //            listItem += '</div>';


                //        }
                //        listItem += '<div class="timeline-item">';
                //        listItem += '<div class="timeline-item-marker">';

                //        if (ICardHistory[i].IsComplete == 0 && ICardHistory[i].Status == "Pending")
                //            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                //        else if (ICardHistory[i].Status == "Approved")
                //            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                //        else if (ICardHistory[i].Status == "Reject")
                //            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-danger">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                //        else if (ICardHistory[i].Status == "Internal Forward")
                //            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';


                //        listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                //        listItem += '</div>';
                //        listItem += '<div class="timeline-item-content">';


                //        listItem += '' + ICardHistory[i].FromDomain + '(' + ICardHistory[i].FromRank + ' ' + ICardHistory[i].FromProfile + ')';

                //        if (ICardHistory[i].IsComplete == 0 && ICardHistory[i].Status == "Pending")
                //            listItem += '<br><span class="badge bg-success">' + 'Pending' + ' And Sent To</span>';
                //        else if (ICardHistory[i].Status == "Approved")
                //            listItem += '<br><span class="badge bg-success">' + ICardHistory[i].Status + ' And Sent To</span>';
                //        else if (ICardHistory[i].Status == "Reject")
                //            listItem += '<br><span class="badge bg-danger">' + ICardHistory[i].Status + ' And Sent To</span>';
                //        else if (ICardHistory[i].Status == "Internal Forward")
                //            listItem += '<br><span class="badge bg-success">' + ICardHistory[i].Status + ' And Sent To</span>';

                //        listItem += '<br> <strong class="text-center">Remark</strong> <br>' + ICardHistory[i].Remark + '';

                //        if (ICardHistory[i].Remarks2 != null) {
                //            var rem = ICardHistory[i].Remarks2.split('#');
                //            if (rem.length > 0) {

                //                listItem += '<ul>';
                //                for (var j = 0; j < rem.length; j++) {
                //                    listItem += '<li>' + rem[j] + '</li>';
                //                }
                //                listItem += '</ul>';
                //            }
                //        }


                //        listItem += '<br><div class="arrow-icon-box"><i class="fas fa-arrow-down"></i></div>'

                //        if (ICardHistory[i].IsComplete == 0) {
                //            listItem += '<br><span class="badge bg-warning ">Pending from </span>';
                //        }
                //        listItem += '<br>' + ICardHistory[i].ToDomain + '(' + ICardHistory[i].ToRank + ' ' + ICardHistory[i].ToProfile + ')';



                //        // Build an array of valid TrnFwdIds from ICardHistory
                //        const validTrnFwdIds = ICardHistory.map(h => ICardHistory[i].TrnFwdId);

                //        // Filter PostingOut based on matching TrnFwdId
                //        let  PostingOut1 = PostingOut.filter(p => validTrnFwdIds.includes(p.TrnFwdId));

                //       // var PostingOut = PostingOut.filter(i => i.TrnFwdId == ICardHistory[i].TrnFwdId)
                //        if (PostingOut1.length > 0) {
                //            listItem += '<br><div class="arrow-icon-box"><i class="fas fa-arrow-down"></i></div>'
                //            listItem += '<br> <strong class="text-center text-danger">' + PostingOut1[0].Reason + '</strong> <br> <span class="text-info">From Unit </span>  <br>' + PostingOut1[0].FromUnit + ' <br> <span class="text-info">To Unit </span>  <br>' + PostingOut1[0].UnitName + '';
                //        }

                //        let FaultyCard1 = FaultyCard.filter(p => validTrnFwdIds.includes(p.TrnFwdId));


                //        if (FaultyCard1.length > 0) {
                //            let remarksfaulty = FaultyCard1[0].RemarksNameList.split('#');
                //            let remarks = "<ul>";
                //            for (let f = 0; f < remarksfaulty.length; f++) {
                //                remarks += '<li>' + remarksfaulty[f] +'</li>';
                //            }
                //            remarks += "</ul>";
                //            listItem += '<br><div class="arrow-icon-box"><i class="fas fa-arrow-down"></i></div>'
                //            listItem += '<br><strong class="text-center text-danger text-decoration-underline">Faulty Card </strong> <br> <span class="text-danger">Reason</span> <br><strong class="text-left text-info">' + remarks + '</strong> By :-' + FaultyCard1[0].FaultyStage + '';
                //        }

                //        if (ICardHistory.length == i) {

                //            if (CloseCard != null) {
                //                listItem += '<br><div class="arrow-icon-box"><i class="fas fa-arrow-down"></i></div>'
                //                listItem += '<br> <strong class="text-center text-danger">Appl Close </strong> <br> Reason :-' + CloseCard.Reason + '';
                //            }
                //        }

                //        listItem += '</div>';
                //        listItem += '</div>';
                //    }
                //} else {
                //    listItem += '<div class="timeline-item">';
                //    listItem += '<div class="timeline-item-marker">';


                //    listItem += '</div>';
                //    listItem += '<div class="timeline-item-content">';
                //    listItem += 'I-Card Submitted Succesfully';


                //    let PostingOut1 = PostingOut?.filter(p => p.TrnFwdId == 0);

                //    // var PostingOut = PostingOut.filter(i => i.TrnFwdId == ICardHistory[i].TrnFwdId)
                //    if (PostingOut1?.length > 0) {
                //        listItem += '<br><div class="arrow-icon-box"><i class="fas fa-arrow-down"></i></div>'
                //        listItem += '<br> <strong class="text-center text-danger">' + PostingOut1[0].Reason + '</strong> <br> <span class="text-info">From Unit </span>  <br>' + PostingOut1[0].FromUnit + ' <br> <span class="text-info">To Unit </span>  <br>' + PostingOut1[0].UnitName + '';
                //    }

                //    listItem += '</div>';
                //    listItem += '</div>';

                //    $("#RequestHistory").html(listItem);
                //}

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
                else if (PostingOut.length > 0) {

                    /*
                     * No forwarding movement exists yet.
                     * Show all Posting Out records created before movement.
                     */
                    const postingOutBeforeMovement = PostingOut.filter(
                        posting => Number(posting.TrnFwdId ?? 0) === 0
                    );

                    listItem = postingOutBeforeMovement
                        .map((posting, index) =>
                            createPostingOutWithoutMovementHtml(posting, index)
                        )
                        .join("");

                }
                else {

                    listItem = `
                        <div class="text-center text-muted py-3">
                            No application movement history available.
                        </div>
                    `;
                }
                const myModal = new bootstrap.Modal(document.getElementById("HistoryModal"));
                let HistoryModal_Header = document.getElementById("HistoryModal_Header");
                let HistoryModal_Title_Content = document.getElementById("HistoryModal_Title_Content");
                let HistoryModal_Body = document.getElementById("HistoryModal_Body");

                HistoryModal_Header.innerHTML = "I-Card Application History";
                HistoryModal_Title_Content.innerHTML = "Step-by-step I Card Application History";
                HistoryModal_Body.innerHTML = listItem;
                myModal.show();
            } else {

            }
        }

    });
}

function GetMovementHistory(spnRequestId) {
    var userdata = {
        "Request": encryptPayloadData(spnRequestId),
    };
    var listItem = "";
    $.ajax({
        url: '/BasicDetail/GetCardMovementHistory',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {

                if (response?.length > 0) {

                    for (var i = 0; i < response.length; i++) {

                        listItem += '<div class="timeline-item">';
                        listItem += '<div class="timeline-item-marker">'; 
                        if (response[i].StepName == "I-Card Lost" || response[i].StepName == "I-Card Holtist")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-danger">' + DateFormateddMMyyyyhhmmss(response[i].ReportedOn) + '</span></div>';
                        else
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(response[i].ReportedOn) + '</span></div>';

                        listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                        listItem += '</div>';
                        listItem += '<div class="timeline-item-content">';

                        if (response[i].StepName == "I-Card Lost" || response[i].StepName == "I-Card Holtist")
                            listItem += '<span class="badge bg-danger">' + response[i].StepName + '</span><br>';
                        else
                            listItem += '<span class="badge bg-success">' + response[i].StepName + '</span><br>';

                        listItem += 'by ' + response[i].ReportedBy;

                        listItem += '<br> <strong class="text-center">Remark</strong> <br>' + response[i].Remark + '';

                        if (response.length != i+1) {
                            listItem += '<br><div class="arrow-icon-box"><i class="fas fa-arrow-down"></i></div>';
                        }
                        listItem += '</div>';
                        listItem += '</div>';
                    }
                } else {

                }
                const myModal = new bootstrap.Modal(document.getElementById("HistoryModal"));
                let HistoryModal_Header = document.getElementById("HistoryModal_Header");
                let HistoryModal_Title_Content = document.getElementById("HistoryModal_Title_Content");
                let HistoryModal_Body = document.getElementById("HistoryModal_Body");

                HistoryModal_Header.innerHTML = "I-Card History";
                HistoryModal_Title_Content.innerHTML = "Step-by-step I-Card History";
                HistoryModal_Body.innerHTML = listItem;
                myModal.show();
            } else {

            }
        }

    });
}

function createPostingOutWithoutMovementHtml(posting, index) {

    const postingDate =
        posting.UpdatedOn ??
        posting.CreatedOn ??
        posting.ReportedOn ??
        null;

    const dateHtml = postingDate
        ? `
            <div class="timeline-item-marker-text">
                <span class="badge bg-danger">
                    ${DateFormateddMMyyyyhhmmss(postingDate)}
                </span>
            </div>
          `
        : `<div class="timeline-item-marker-text"></div>`;

    return `
        <div class="timeline-item">
            <div class="timeline-item-marker">

                ${dateHtml}

                <div class="timeline-item-marker-indicator bg-primary">
                </div>
            </div>

            <div class="timeline-item-content">

                <span class="badge bg-warning">
                    Posting Out Before Forward Movement
                </span>

                ${createPostingOutHtml(posting)}

            </div>
        </div>
    `;
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
