function GetRequestHistory(spnRequestId) {
    var userdata = {

        "RequestId": spnRequestId,


    };
    var listItem = "";
    $.ajax({
        url: '/BasicDetail/GetRequestHistory',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
              
                ICardHistory = response.ICardHistory;
                PostingOut = response.PostingOut;
                FaultyCard = response.FaultyCard;
                CloseCard = response.CloseCard;
                if (ICardHistory.length > 0) {
                   
                    for (var i = 0; i < ICardHistory.length; i++) {
                        if (i == 0) {
                            listItem += '<div class="timeline-item">';
                            listItem += '<div class="timeline-item-marker">';
                            listItem += '<div class="timeline-item-marker-text "><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                            listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                            listItem += '</div>';
                            listItem += '<div class="timeline-item-content">';
                            listItem += 'I-Card Submit By -' + ICardHistory[i].FromDomain + '(' + ICardHistory[i].FromRank + ' ' + ICardHistory[i].FromProfile + ')';
                            if (i == 0) {
                                // Filter PostingOut based on matching TrnFwdId
                                let PostingOut1 = PostingOut.filter(p => p.TrnFwdId == 0);

                                // var PostingOut = PostingOut.filter(i => i.TrnFwdId == ICardHistory[i].TrnFwdId)
                                if (PostingOut1.length > 0) {
                                    listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'
                                    listItem += '<br> <strong class="text-center text-danger">' + PostingOut1[0].Reason + '</strong> <br> <span class="text-info">From Unit </span>  <br>' + PostingOut1[0].FromUnit + ' <br> <span class="text-info">To Unit </span>  <br>' + PostingOut1[0].UnitName + '';
                                }
                            }
                            listItem += '</div>';
                            listItem += '</div>';


                        }
                        listItem += '<div class="timeline-item">';
                        listItem += '<div class="timeline-item-marker">';

                        if (ICardHistory[i].IsComplete == 0 && ICardHistory[i].Status == "Pending")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                        else if (ICardHistory[i].Status == "Approved")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                        else if (ICardHistory[i].Status == "Reject")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-danger">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';
                        else if (ICardHistory[i].Status == "Internal Forward")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(ICardHistory[i].UpdatedOn) + '</span></div>';


                        listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                        listItem += '</div>';
                        listItem += '<div class="timeline-item-content">';


                        listItem += '' + ICardHistory[i].FromDomain + '(' + ICardHistory[i].FromRank + ' ' + ICardHistory[i].FromProfile + ')';

                        if (ICardHistory[i].IsComplete == 0 && ICardHistory[i].Status == "Pending")
                            listItem += '<br><span class="badge bg-success">' + 'Pending' + ' And Sent To</span>';
                        else if (ICardHistory[i].Status == "Approved")
                            listItem += '<br><span class="badge bg-success">' + ICardHistory[i].Status + ' And Sent To</span>';
                        else if (ICardHistory[i].Status == "Reject")
                            listItem += '<br><span class="badge bg-danger">' + ICardHistory[i].Status + ' And Sent To</span>';
                        else if (ICardHistory[i].Status == "Internal Forward")
                            listItem += '<br><span class="badge bg-success">' + ICardHistory[i].Status + ' And Sent To</span>';

                        listItem += '<br> <strong class="text-center">Remark</strong> <br>' + ICardHistory[i].Remark + '';

                        if (ICardHistory[i].Remarks2 != null) {
                            var rem = ICardHistory[i].Remarks2.split('#');
                            if (rem.length > 0) {

                                listItem += '<ul>';
                                for (var j = 0; j < rem.length; j++) {
                                    listItem += '<li>' + rem[j] + '</li>';
                                }
                                listItem += '</ul>';
                            }
                        }


                        listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'

                        if (ICardHistory[i].IsComplete == 0) {
                            listItem += '<br><span class="badge bg-warning ">Pending from </span>';
                        }
                        listItem += '<br>' + ICardHistory[i].ToDomain + '(' + ICardHistory[i].ToRank + ' ' + ICardHistory[i].ToProfile + ')';

                        
                      
                        // Build an array of valid TrnFwdIds from ICardHistory
                        const validTrnFwdIds = ICardHistory.map(h => ICardHistory[i].TrnFwdId);

                        // Filter PostingOut based on matching TrnFwdId
                        let  PostingOut1 = PostingOut.filter(p => validTrnFwdIds.includes(p.TrnFwdId));

                       // var PostingOut = PostingOut.filter(i => i.TrnFwdId == ICardHistory[i].TrnFwdId)
                        if (PostingOut1.length > 0) {
                            listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'
                            listItem += '<br> <strong class="text-center text-danger">' + PostingOut1[0].Reason + '</strong> <br> <span class="text-info">From Unit </span>  <br>' + PostingOut1[0].FromUnit + ' <br> <span class="text-info">To Unit </span>  <br>' + PostingOut1[0].UnitName + '';
                        }

                        let FaultyCard1 = FaultyCard.filter(p => validTrnFwdIds.includes(p.TrnFwdId));

                      
                        if (FaultyCard1.length > 0) {
                            let remarksfaulty = FaultyCard1[0].RemarksNameList.split('#');
                            let remarks = "<ul>";
                            for (let f = 0; f < remarksfaulty.length; f++) {
                                remarks += '<li>' + remarksfaulty[f] +'</li>';
                            }
                            remarks += "</ul>";
                            listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'
                            listItem += '<br><strong class="text-center text-danger text-decoration-underline">Faulty Card </strong> <br> <span class="text-danger">Reason</span> <br><strong class="text-left text-info">' + remarks + '</strong> By :-' + FaultyCard1[0].FaultyStage + '';
                        }

                        if (ICardHistory.length == i) {
                          
                            if (CloseCard != null) {
                                listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'
                                listItem += '<br> <strong class="text-center text-danger">Appl Close </strong> <br> Reason :-' + CloseCard.Reason + '';
                            }
                        }

                        listItem += '</div>';
                        listItem += '</div>';
                    }
                } else {
                    listItem += '<div class="timeline-item">';
                    listItem += '<div class="timeline-item-marker">';


                    listItem += '</div>';
                    listItem += '<div class="timeline-item-content">';
                    listItem += 'I-Card Submitted Succesfully';


                    let PostingOut1 = PostingOut.filter(p => p.TrnFwdId == 0);

                    // var PostingOut = PostingOut.filter(i => i.TrnFwdId == ICardHistory[i].TrnFwdId)
                    if (PostingOut1.length > 0) {
                        listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'
                        listItem += '<br> <strong class="text-center text-danger">' + PostingOut1[0].Reason + '</strong> <br> <span class="text-info">From Unit </span>  <br>' + PostingOut1[0].FromUnit + ' <br> <span class="text-info">To Unit </span>  <br>' + PostingOut1[0].UnitName + '';
                    }

                    listItem += '</div>';
                    listItem += '</div>';

                    $("#RequestHistory").html(listItem);
                }

                $("#RequestHistory").html(listItem);
            } else {

            }
        }

    });
}
