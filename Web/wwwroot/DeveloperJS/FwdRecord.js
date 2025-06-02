//var photo = "";
//var sing = "";
var StepCounter = 0;
var applyfor = 0;
var xmlsign = 0;
var lstmultifwdarr = new Array();
var lstInternalFwd = new Array();
var lstCSVDownload = new Array();
var RegistrationApplyFor = 0;
var IsToken = true;
var IsWithTokenApply = true;
var IsValid = 0;
var IsDigitalSignReq = true;
var DataExportType = 1;
$(function () {

    $("#btntokenTofwd").on("click", async function () {
        $("#msgforfwd").html('');

        await GetTokenvalidatepersid2fawiththumbprint($("#aspntokenarmyno").html(), "tokenmsgforfwd", "txtspnTokenArmyNo", "txtspnTokenthumbprint");
    });
    sessionStorage.removeItem('ArmyNo');
    $('#btnDataExports').on("click", function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lst.push(id);
                console.log(id);

            });

            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Export",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Export it!'
            }).then((result) => {
                if (result.value) {
                    DataExportType = 0;
                    DataExport(lst);

                }
            });
        } else {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
    });

    $('#btnDataExportsEncry').on("click", function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lst.push(id);
            });

            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Export",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Export it!'
            }).then((result) => {
                if (result.value) {
                    DataExportType = 1;
                    DataExport(lst);

                }
            });
        } else {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
    });

    var spnStepId = 0;
    $('.select2').select2({
        dropdownParent: $('#BasicDetails'),
        closeOnSelect: true

    });
    $('.select3').select2({
        dropdownParent: $('#FwdRecord'),
        closeOnSelect: true
    });
    $('.select4').select2({
        dropdownParent: $('#FwdInternalRecord'),
        closeOnSelect: true
    });


    $(".historyRequest").on("click", function () {
        GetRequestHistory($(this).closest("tr").find(".spnRequestId").html());
    });

    $('#ddlfwdoffrs').on('change', function () {
        $("#spnFwdToAspNetUsersId").html(0);
        $("#spnFwdToUsersId").html(0);
        $(".spnFArmyNo").html("");
        $(".spnFtoname").html("");
        $(".spnFDomainName").html("");
        $(".spnFAppName").html("");

        $("#intoffsArmyNo").prop("checked", false);
        $("#intoffDomainId").prop("checked", false);
        $(".serchfwd").addClass("d-none");

        FwdData($('#ddlfwdoffrs').val());
    });
    $('#ddlfwdInternaloffrs').on('change', function () {
        const selectedValue = $(this).val();

        // Reset UI values
        $("#spnFwdToInternalUsersId").html(0);
        $(".spnInternalFArmyNo").html("");
        $(".spnInternalFtoname").html("");
        $(".spnInternalFDomainName").html("");
        $(".spnInternalFAppName").html("");

        // Check if an officer is selected
        if (!selectedValue || selectedValue.length === 0) {
            Swal.fire({
                text: "Please select Offr."
            });
            return; // Exit early
        }

        // Call profile detail function
        GetProfiledetailsByAspNetuseridForInternalFwd(selectedValue);

    });
    $('#ddlfwdInternaloffrs').on('click', function () {
        const val = $(this).val();

        // If only one option and user clicks it (again), manually trigger
        if ($('#ddlfwdInternaloffrs option').length === 1) {
            $('#ddlfwdInternaloffrs').trigger('change');
        }
    });

    //$('#ddlPhotos').on('change', function () {
    //    photo= $('#ddlPhotos').val();
    //});
    //$('#ddlsignature').on('change', function () {
    //    sing=$('#ddlsignature').val();
    //});
    //$("#btnRejected").click(function () {

    //    $("#txtFrejectedRemarks").val($("#txtFrejectedRemarks").val() + "" + photo + "" + sing);
    //});
    $("#btnMultipleForward").on("click", function () {



        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lstmultifwdarr.push(id);
                console.log(id);

            });
            $("#FwdRecord").modal('show');
            $(".chkforserach").addClass("d-none");
            $(".gsoio").html("HQ 54");
            $("#btnForward").html("Forward To HQ 54");

            StepCounter = 4;
            applyfor = 1;
            spnStepId = 0;
            $("#multiplefed").addClass("d-none");
            GetAllOffsByUnitId("ddlfwdoffrs", 0, spnHQ54UnitId, 0, 0, 0, 0);


            $(".Remarks").removeClass("d-none");
            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);


            var Reject = [2];
            GetRemarks("ddlRRemarks", 0, Reject);

        } else {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }

    });

    $("#btnShowForward").on("click", function () {
        $("#multiplefed").removeClass("d-none");
        $("#BasicDetails").modal('hide');

        $("#FwdRecord").modal('show');

        GetByArmyNoIsToken("", applyfor, RegistrationApplyFor, StepCounter);
    });

    $("input[name='Intoffrs']").on("change", function () {
        $(".serchfwd").removeClass("d-none");

        $("#spnFwdToAspNetUsersId").html(0);
        $("#spnFwdToUsersId").html(0);
        $(".spnFArmyNo").html("");
        $(".spnFtoname").html("");
        $(".spnFDomainName").html("");
        $(".spnFAppName").html("");

    });
    $(".btndownloadpdf").on("click", function () {
        DownloadPdf($(this).closest("tr").find(".spnRequestId").html())
    });
    $(".btndownloadxml").on("click", function () {
        DownloadXml($(this).closest("tr").find(".spnRequestId").html())
    });
    $(".fwdrecord").on("click", async function () {
        Reset();

        $("#multiplefed").addClass("d-none");
        $("#btntokenTofwd").addClass("d-none");
        $("#ddlRemarks").val("");


        IsDigitalSignReq = true;

        $(".spnFname").html($(this).closest("tr").find(".PersName").html());
        $(".spnFarmyno").html($(this).closest("tr").find(".ServiceNo").html());
        $("#spnStepCounter").html($(this).closest("tr").find(".spnStepCounterId").html());
        var spnTrnFwdId = $(this).closest("tr").find(".spnTrnFwdId").html();
        $("#spnCurrentspnTrnFwdId").html(spnTrnFwdId);
        var spnRequestId = $(this).closest("tr").find(".spnRequestId").html();
        $("#spnCurrentspnRequestId").html(spnRequestId);
        spnStepId = $(this).closest("tr").find(".spnStepId").html();
        const Unitidarmy = $(this).closest("tr").find(".spnarmyUnitId").html();

        StepCounter = $(this).closest("tr").find(".spnStepCounterId").html();
        applyfor = $(this).closest("tr").find(".spnApplyFor").html();
        RegistrationApplyFor = $(this).closest("tr").find(".spnRegistrationApplyFor").html();
        $("#spnCurrentRegistrationApplyFor").html(RegistrationApplyFor);
        $("#spnCurrentApplyFor").html(applyfor);
        $("#spnServiceNo").html($(this).closest("tr").find(".spnServiceNo").html());

        if (StepCounter == 1 || StepCounter == 7 || StepCounter == 8 || StepCounter == 9 || StepCounter == 10) {
            $(".recectopt").addClass("d-none");
            $("#btnRejected").addClass("d-none");

        }
        await GetBasicDetailByRequestIdForFwd($(this).closest("tr").find(".spnRequestId").html());

        if (StepCounter == 1 || StepCounter == 7 || StepCounter == 8 || StepCounter == 9 || StepCounter == 10 || StepCounter == 11 || StepCounter == 12 || StepCounter == 13 || StepCounter == 15) {

            if (applyfor == 1) {
                $(".gsoio").html("IO / Next Superior Offr");
                $(".gsoiotitle").html("IO / Next Superior Offr");
                $("#btnForward").html("Forward To IO / Superior");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, 0, 0, 0);
            } else {
                $(".gsoio").html("CO /OC / OC TPS or Offr Nominated");
                $(".gsoiotitle").html("CO / OC / OC TPS or Offr Nominated");
                $("#btnForward").html("Forward To CO / OC / OC TPS or Offr Nominated");

                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, 0, 0, 0);
            }
            $(".Remarks").removeClass("d-none");

            $("#btntokenTofwd").removeClass("d-none");

            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);
        } else if (StepCounter == 2) {
            $(".chkforserach").addClass("d-none");
            $(".serchfwd").addClass("d-none");
            if (applyfor == 1) {
                $(".gsoio").html("Record Office");
                $(".gsoiotitle").html("Offr Record Office (ORO) Approval");
                $("#btnForward").html("Forward To Record Office");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, spnIntORO, 0, $(this).closest("tr").find(".spnBasicDetailId").html());
            } else {
                $(".gsoio").html("Record Office (RO)");
                $(".gsoiotitle").html("Record Office (RO) Approval");
                $("#btnForward").html("Forward To Record Office (RO)");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, spnIntRO, 0, 0, $(this).closest("tr").find(".spnBasicDetailId").html());
            }
            $("#btntokenTofwd").removeClass("d-none");
            $(".Remarks").removeClass("d-none");
            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);

            var Reject = [2];
            GetRemarks("ddlRRemarks", 0, Reject);

        } else if (StepCounter == 3) {
            if (applyfor == 1) {
                $(".chkforserach").addClass("d-none");

                $(".gsoio").html("AFSAC Cell");
                $(".gsoiotitle").html("AFSAC Cell");
                $("#btnForward").html("Forward To AFSAC Cell");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, 0, 0, 0, spnIntAfsacCell, 0);
            } else {
                $(".chkforserach").addClass("d-none");
                $(".gsoiotitle").html("AFSAC Cell");
                $(".gsoio").html("AFSAC Cell");
                $("#btnForward").html("Forward To AFSAC Cell ");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, 0, 0, 0, spnIntAfsacCell, 0);
            }



            $(".Remarks").removeClass("d-none");
            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);

            var Reject = [2];
            GetRemarks("ddlRRemarks", 0, Reject);
        }
    });
    $("#tbldatatabledata tbody").off("click", ".cls-fwdrecord").on("click", ".cls-fwdrecord", async function () {
        var rowData = table.row($(this).closest("tr")).data();
        if (rowData != null) {
            Reset();

            $("#multiplefed").addClass("d-none");
            $("#btntokenTofwd").addClass("d-none");
            $("#ddlRemarks").val("");

            IsDigitalSignReq = true;

            $(".spnFname").html(`${rowData.RankName || ""} ${rowData.FName || ""} ${rowData.LName || ""}`.trim());
            $(".spnFarmyno").html(/^[A-Za-z]{2}/.test(rowData.ServiceNo) ? rowData.ServiceNo.slice(0, 2) + ' ' + rowData.ServiceNo.slice(2) : rowData.ServiceNo);
            $("#spnStepCounter").html(rowData.StepCounter);
            var spnTrnFwdId = rowData.IsTrnFwdId;
            $("#spnCurrentspnTrnFwdId").html(spnTrnFwdId);
            var spnRequestId = rowData.RequestId;
            $("#spnCurrentspnRequestId").html(spnRequestId);
            spnStepId = rowData.StepId;
            const Unitidarmy = rowData.UnitId;

            StepCounter = rowData.StepCounter;
            applyfor = rowData.ApplyForId;
            RegistrationApplyFor = rowData.RegistrationApplyFor;
            $("#spnCurrentRegistrationApplyFor").html(RegistrationApplyFor);
            $("#spnCurrentApplyFor").html(applyfor);
            $("#spnServiceNo").html(rowData.ServiceNo);

            if (StepCounter == 1 || StepCounter == 7 || StepCounter == 8 || StepCounter == 9 || StepCounter == 10) {
                $(".recectopt").addClass("d-none");
                $("#btnRejected").addClass("d-none");

            }
            await GetBasicDetailByRequestIdForFwd(rowData.RequestId);

            if (StepCounter == 1 || StepCounter == 7 || StepCounter == 8 || StepCounter == 9 || StepCounter == 10 || StepCounter == 11 || StepCounter == 12 || StepCounter == 13 || StepCounter == 15) {

                if (applyfor == 1) {
                    $(".gsoio").html("IO / Next Superior Offr");
                    $(".gsoiotitle").html("IO / Next Superior Offr");
                    $("#btnForward").html("Forward To IO / Superior");
                    GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, 0, 0, 0);
                } else {
                    $(".gsoio").html("CO /OC / OC TPS or Offr Nominated");
                    $(".gsoiotitle").html("CO / OC / OC TPS or Offr Nominated");
                    $("#btnForward").html("Forward To CO / OC / OC TPS or Offr Nominated");

                    GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, 0, 0, 0);
                }
                $(".Remarks").removeClass("d-none");

                //$(".chkforserach").addClass("d-none");
                $("#btntokenTofwd").removeClass("d-none");

                var someNumbers = [1];
                GetRemarks("ddlRemarks", 0, someNumbers);
            } else if (StepCounter == 2) {
                $(".chkforserach").addClass("d-none");
                $(".serchfwd").addClass("d-none");
                if (applyfor == 1) {
                    $(".gsoio").html("Record Office");
                    $(".gsoiotitle").html("Offr Record Office (ORO) Approval");
                    $("#btnForward").html("Forward To Record Office");
                    GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, spnIntORO, 0, rowData.BasicDetailId);
                } else {
                    $(".gsoio").html("Record Office (RO)");
                    $(".gsoiotitle").html("Record Office (RO) Approval");
                    $("#btnForward").html("Forward To Record Office (RO)");
                    GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, spnIntRO, 0, 0, rowData.BasicDetailId);
                }
                $("#btntokenTofwd").removeClass("d-none");
                $(".Remarks").removeClass("d-none");
                var someNumbers = [1];
                GetRemarks("ddlRemarks", 0, someNumbers);

                var Reject = [2];
                GetRemarks("ddlRRemarks", 0, Reject);

            } else if (StepCounter == 3) {
                if (applyfor == 1) {
                    $(".chkforserach").addClass("d-none");

                    $(".gsoio").html("AFSAC Cell");
                    $(".gsoiotitle").html("AFSAC Cell");
                    $("#btnForward").html("Forward To AFSAC Cell");
                    GetAllOffsByUnitId("ddlfwdoffrs", 0, 0, 0, 0, spnIntAfsacCell, 0);
                } else {
                    $(".chkforserach").addClass("d-none");
                    $(".gsoiotitle").html("AFSAC Cell");
                    $(".gsoio").html("AFSAC Cell");
                    $("#btnForward").html("Forward To AFSAC Cell ");
                    GetAllOffsByUnitId("ddlfwdoffrs", 0, 0, 0, 0, spnIntAfsacCell, 0);
                }



                $(".Remarks").removeClass("d-none");
                var someNumbers = [1];
                GetRemarks("ddlRemarks", 0, someNumbers);

                var Reject = [2];
                GetRemarks("ddlRRemarks", 0, Reject);
            }




        }
    });
    $("#txtFwdName").autocomplete({

        source: function (request, response) {
            var TypeId = 1;
            if ($("#intoffsArmyNo").prop("checked")) {
                TypeId = 1;
            } else if ($("#intoffName").prop("checked")) {
                TypeId = 2;
            } else if ($("#intoffDomainId").prop("checked")) {
                TypeId = 3;
            }
            var IsRO = 0;
            var IsORO = 0;
            if (applyfor == 1 && StepCounter == 2)
                IsORO = 1;
            else if (applyfor == 2 && StepCounter == 2)
                IsRO = 1;
            var param = {
                "Name": request.term,
                "TypeId": TypeId,
                "StepId": 1,
                "UnitId": 0,
                "IsRO": IsRO,
                "IsORO": IsORO
            };

            $("#spnFwdToAspNetUsersId").html(0);
            $.ajax({
                url: '/UserProfile/GetDataForFwd',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);
                    if (data.length != 0) {
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return {
                                label: item.ArmyNo + ' ' + item.RankAbbreviation + ' ' + item.Name + ' ' + item.DomainId,
                                value: item.AspNetUsersId
                            };

                        }))
                    } else {

                        $(".spnFArmyNo").html("");
                        $(".spnFtoname").html("");
                        $(".spnFDomainName").html("");
                        $(".spnFAppName").html("");

                        $("#txtFwdName").val("");
                        $("#spnFwdToAspNetUsersId").html("0");
                        $("#spnFwdToUsersId").html("0");
                        alert("Army No/Offr Name/Domain ID not found.")
                    }
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            e.preventDefault();
            //alert(i.item.value)
            $("#txtFwdName").val(i.item.label);
            //alert(i.item.value)
            // var param1 = { "UnitMapId": i.item.value };
            //$("#btnIOProfileSerch").addClass('d-none');
            FwdData(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

    $('#txtFwdName').on("keyup", function (e) {
        if (e.which == 46) {
            $(".spnFArmyNo").html("");
            $(".spnFtoname").html("");
            $(".spnFDomainName").html("");
            $(".spnFAppName").html("");

            $("#txtFwdName").val("");
            $("#spnFwdToAspNetUsersId").html("0");
            $("#spnFwdToUsersId").html("0");
        }
    });

    $("#btnForward").on("click", async function () {

        if (parseInt(StepCounter) == 1 || parseInt(StepCounter) == 7 || parseInt(StepCounter) == 8 || parseInt(StepCounter) == 9 || parseInt(StepCounter) == 10) {
            if (parseInt($("#spnCurrentApplyFor").html()) == 1) {
                let CurrentRegistrationApplyFor = parseInt($("#spnCurrentRegistrationApplyFor").html());
                if (CurrentRegistrationApplyFor == 2 || CurrentRegistrationApplyFor == 3 || CurrentRegistrationApplyFor == 4 || CurrentRegistrationApplyFor == 10) {
                    if (IsWithTokenApply == true) {
                        await GetTokenvalidatepersid2fawiththumbprint($("#spnServiceNo").html(), "tokenmsgforfwd", "txtspnTokenArmyNo", "txtspnTokenthumbprint");
                    }
                }
                else {
                    if (IsToken == true && CurrentRegistrationApplyFor == 1) {
                        await GetTokenvalidatepersid2fawiththumbprint($("#aspntokenarmyno").html(), "tokenmsgforfwd", "txtspnTokenArmyNo", "txtspnTokenthumbprint");
                    }
                }
            }
        }
        else {
            // When executed stepCounter not 1-drafted,2-Pending,7-rejected request by CO
            if (IsToken == true) {
                await GetTokenvalidatepersid2fawiththumbprint($("#aspntokenarmyno").html(), "tokenmsgforfwd", "txtspnTokenArmyNo", "txtspnTokenthumbprint");
            }
        }

        if (parseInt(spnStepId) != 0) {
            $("#msgforfwd").html('');
            if (parseInt(spnStepId) != 0) {
                Swal.fire({
                    title: 'Are you sure?',
                    /*  text: "You want be Forward!",*/
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Yes, Forward it!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        if (parseInt($("#spnFwdToAspNetUsersId").html()) != 0) {
                            var spnRequestId = $("#spnCurrentspnRequestId").html();

                            var Counter = parseInt($("#spnStepCounter").html());
                            if (Counter == 1 || Counter == 7 || Counter == 8 || Counter == 9 || Counter == 10) {
                                Counter = 2;
                            } else {
                                Counter = parseInt($("#spnStepCounter").html()) + 1;
                            }

                            if (parseInt(StepCounter) == 1 || parseInt(StepCounter) == 7 || parseInt(StepCounter) == 8 || parseInt(StepCounter) == 9 || parseInt(StepCounter) == 10) {
                                if (parseInt($("#spnCurrentApplyFor").html()) == 1) {
                                    let CurrentRegistrationApplyFor = parseInt($("#spnCurrentRegistrationApplyFor").html());
                                    if (CurrentRegistrationApplyFor == 2 || CurrentRegistrationApplyFor == 3 || CurrentRegistrationApplyFor == 4 || CurrentRegistrationApplyFor == 10) {
                                        if (IsWithTokenApply == true) {
                                            if ($("#spnServiceNo").html() == $("#txtspnTokenArmyNo").val()) {
                                                IsDigitalSignReq = true;
                                                UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");
                                            }
                                        }
                                        else {
                                            IsDigitalSignReq = false;
                                            UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");
                                        }
                                    }
                                    else {
                                        if (IsToken == true && CurrentRegistrationApplyFor == 1) {
                                            if (($("#aspntokenarmyno").html() == $("#txtspnTokenArmyNo").val()) && ($("#spnServiceNo").html() == $("#txtspnTokenArmyNo").val())) {
                                                IsDigitalSignReq = true;
                                                UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");
                                            }
                                        }
                                        else {
                                            IsDigitalSignReq = false;
                                            UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");
                                        }
                                    }
                                }
                                else {
                                    IsDigitalSignReq = false;
                                    UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");
                                }
                            }
                            else {
                                // When executed stepCounter not 1-drafted,7,8,9 & 10-rejected request
                                if (IsToken == true) {
                                    //alert("1" + $("#aspntokenarmyno").html());
                                    //alert("2" + $("#txtspnTokenArmyNo").val());
                                    if ($("#aspntokenarmyno").html() === $("#txtspnTokenArmyNo").val()) {
                                        IsDigitalSignReq = true;
                                        UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");
                                    }

                                }
                                else {
                                    IsDigitalSignReq = false;
                                    UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");
                                }
                            }
                        }
                        else {
                            toastr.error("Please Select Officer ");
                        }
                    }
                })
            }
        }
        else {
            $("#msgforfwd").html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Please Correct Token insert and Click refresh Button </span></div>');
        }
    });

    $("#btnRejected").on("click", function () {

        /*  $("#txtFrejectedRemarks").val($("#txtFrejectedRemarks").val() + "" + photo + "" + sing);*/
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be Forward!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Forward it!'
        }).then((result) => {
            if (result.isConfirmed) {
                var spnRequestId = $("#spnCurrentspnRequestId").html();
                var Counter = parseInt($("#spnStepCounter").html());
                if (Counter == 2)
                    Counter = 7
                else if (Counter == 3)
                    Counter = 8
                else if (Counter == 4)
                    Counter = 9
                else if (Counter == 5)
                    Counter = 10


                if ($("#txtFrejectedRemarks").val() != "" || $("#ddlRRemarks").val() != "")
                    UpdateStepCounter(spnStepId, spnRequestId, Counter, "R");
                else
                    toastr.error('Please Enter Remarks To Reject');

            }
        })
    });

    $("#btnInternalFwd").on("click", function () {
        // Empty the array
        lstInternalFwd.length = 0;
        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {
            GetAllOffsByUnitId("ddlfwdInternaloffrs", 0, 0, 0, 0, 0, 0);
            $(".RemarksInternalFwd").removeClass("d-none");
            var someNumbers = [1];
            GetRemarks("ddlInternalRemarks", 0, someNumbers);
            memberTable.$('input[type="checkbox"]:checked').each(function () {
                var id = $(this).attr("Id");
                lstInternalFwd.push(id);
            });


            $("#FwdInternalRecord").modal('show');
        } else {
            Swal.fire({
                text: "Please select atleast 1 request to Approval."
            });
        }
    });
    $("#btnCSVDownload").on("click", function () {
        // Empty the array
        lstCSVDownload.length = 0;
        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {
            memberTable.$('input[type="checkbox"]:checked').each(function () {
                var id = $(this).attr("Id");
                lstCSVDownload.push(id);
            });
            var userdata = {
                "Ids": lstCSVDownload,
            };
            $.ajax({
                url: '/BasicDetail/CreateCSV',
                contentType: 'application/x-www-form-urlencoded',
                data: userdata,
                type: 'POST',

                success: function (response) {
                    if (response != "null" && response != null) {
                        if (response == InternalServerError) {
                            Swal.fire({
                                text: errormsg
                            });
                        } else {
                            var url = "https://" + window.location.host + '/WriteReadData/CSVFile/' + response + ".csv";
                            window.location.href = url;
                        }
                    }
                },
                error: function (result) {
                    Swal.fire({
                        text: errormsg002
                    });
                }
            });
        }
        else {
            Swal.fire({
                text: "Please select atleast 1 request to download CSV File."
            });
        }
    });
    $("#btnInternalFwdSubmit").on("click", function () {
        ProceedForInternalFwd();
    });
});

function ProceedForInternalFwd() {
    ResetErrorMessage();
    let formId = '#SaveInternalRecordFwd';
    $.validator.unobtrusive.parse($(formId));


    if ($(formId).valid()) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You want to Forwad",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Forwad it!'
        }).then((result) => {
            if (result.isConfirmed) {
                SaveInternalFwd();
            }
        })
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill required field.',

        })
        return false;
    }
}

function SaveInternalFwd() {
    var remarks = "" + $("#ddlInternalRemarks").val() + "";
    var userdata = {
        "TrnFwdIds": lstInternalFwd,
        "ToAspNetUsersId": $('#ddlfwdInternaloffrs').val(),
        "ToUserId": $("#spnFwdToInternalUsersId").html(),
        "Remark": $('#txtFRemarksInternal').val().length > 0 ? $('#txtFRemarksInternal').val() : null,
        "FwdStatusId": 4,
        "TypeId": 3,
        "RemarksIds": remarks,
    };
    $.ajax({
        url: '/BasicDetail/SaveInternalFwd',
        type: 'POST',
        data: userdata,
        success: function (response) {
            if (response == true) {
                toastr.success('Fwd successfully.');

                $("#FwdInternalRecord").modal('hide');
                setTimeout(function () {
                    location.reload();
                }, 2000);
            } else if (response == false) {
                toastr.error('Something went wrong or Invalid Entry!');
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: 'Something went wrong or Invalid Entry!',
                })
            } else if (response != "null" && response != null) {
                toastr.error('Something went wrong or Invalid Entry!');
            }
            //else if (response.length > 1) {
            //    for (var i = 0; i < response.length; i++) {
            //        toastr.error(response[i][0])
            //    }
            //}
        }
    });
}

function ResetErrorMessage() {
    $("#ddlfwdInternaloffrs-error").html("");
    $("#ddlInternalRemarks-error").html("");
    $("#txtFRemarksInternal-error").html("");
}

function Reset() {
    $("#spnFwdToAspNetUsersId").html(0);
    $("#spnFwdToUsersId").html(0);
    $(".spnFArmyNo").html("");
    $(".spnFtoname").html("");
    $(".spnFDomainName").html("");
    $(".spnFAppName").html("");

    $("#intoffsArmyNo").prop("checked", false);
    $("#intoffDomainId").prop("checked", false);
    $("#txtFwdName").val("");
    $(".serchfwd").addClass("d-none");
}
async function GetBasicDetailByRequestIdForFwd(RequestId) {
    let param = new URLSearchParams({ RequestId: RequestId });

    fetch('/BasicDetail/GetBasicDetailForParitalViewByRequestId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: param
    })
        .then(response => response.text())
        .then(html => {
            document.getElementById("BasicDetails_Data").innerHTML = html;
            $("#BasicDetails").modal('show');
        })
        .catch(error => {
            alert("Error: " + error.message);
        });
}
function FwdData(AspNetUsersId) {
    var userdata = {
        "AspNetUsersId": AspNetUsersId,


    };
    $.ajax({
        url: '/UserProfile/GetByAspnetUserIdBy',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else if (response == 0) {

                } else {
                    $("#spnFwdToAspNetUsersId").html(response.AspNetUsersId);
                    $("#spnFwdToUsersId").html(response.UserId);

                    GetProfiledetailsByAspNetuserid(AspNetUsersId)
                    //$(".HProfileDetails").removeClass("d-none");
                    //$("#ForwardDetails").html("");
                    //$("#btnForward").removeClass("d-none");
                    //$("#spnCurrentspnRequestId").html(response.RequestId);
                    //if (StepCounter == 1) {
                    //    $(".spnFtoarmyno").html(response.IOArmyNo);
                    //    $(".spnFtoname").html(response.IOName);


                    //    $("#spnFrom").html(response.UserId);
                    //    $("#spnForwardTo").html(response.IOUserId);
                    //    $("#spnFwssusno").html(0);
                    //} else if (StepCounter == 2) {
                    //    $(".spnFtoarmyno").html(response.GSOArmyNo);
                    //    $(".spnFtoname").html(response.GSOName);

                    //    $("#spnFrom").html(response.IOUserId);
                    //    $("#spnForwardTo").html(response.GSOUserId);
                    //    $("#spnFwssusno").html(0);
                    //}
                    //else if (StepCounter == 3) {

                    //    $(".HProfileDetails").addClass("d-none");
                    //    $("#spnFrom").html(response.GSOUserId);
                    //    $("#spnFwssusno").html(101);

                    //}
                    //else if (StepCounter == 4) {

                    //    $(".HProfileDetails").addClass("d-none");
                    //    $("#spnFrom").html(101);
                    //    $("#spnForwardTo").html(29);
                    //    $("#spnFwssusno").html(0);

                    //}
                }
            } else {
                //$(".HProfileDetails").addClass("d-none");
                //$("#btnForward").addClass("d-none");
                //$("#ForwardDetails").html("Please Add Self Profile");

                //$(".spnFtoarmyno").html("");
                //$(".spnFtoname").html("");
                //$("#spnForwardTo").html(0);
                //$("#spnCurrentspnRequestId").html(0);


            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function GetProfiledetailsByAspNetuserid(AspNetUsersId) {
    //var param = "";
    //if (StepCounter == 3 && applyfor==1)
    //    var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    //else if (StepCounter == 4 && applyfor == 1)
    //    var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    //else if ((StepCounter == 2 ||StepCounter == 3) && applyfor == 2)
    //    var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    //else
    var param = {
        "Name": AspNetUsersId,
        "TypeId": 0,
        "UnitId": 0
    };
    $.ajax({
        url: '/UserProfile/GetDataForFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: param,
        type: 'POST',
        success: function (data) {
            if (data != null) {
                $(".spnFArmyNo").html(data[0].ArmyNo);
                $(".spnFtoname").html(data[0].RankAbbreviation + " " + data[0].Name);
                $(".spnFDomainName").html(data[0].DomainId);
                $(".spnFAppName").html(data[0].AppointmentName);
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}

function GetProfiledetailsByAspNetuseridForInternalFwd(AspNetUsersId) {

    var param = {
        "Name": AspNetUsersId,
        "TypeId": 0,
        "UnitId": 0
    };
    $.ajax({
        url: '/UserProfile/GetDataForFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: param,
        type: 'POST',
        success: function (data) {
            if (data != null) {
                $(".spnInternalFArmyNo").html(data[0].ArmyNo);
                $(".spnInternalFtoname").html(data[0].RankAbbreviation + " " + data[0].Name);
                $(".spnInternalFDomainName").html(data[0].DomainId);
                $(".spnInternalFAppName").html(data[0].AppointmentName);
                $("#spnFwdToInternalUsersId").html(data[0].UserId);
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}

//function GetForwardHHierarchy(ArmyNo, StepCounter, spnRequestId) {

//    var userdata =
//    {
//        "StepId": StepCounter,
//        "RequestId": spnRequestId,

//    };
//    $.ajax({
//        url: '/UserProfile/GetDataForFwd',
//        contentType: 'application/x-www-form-urlencoded',
//        data: userdata,
//        type: 'POST',

//        success: function (response) {
//            if (response != "null" && response != null) {

//                if (response == InternalServerError) {
//                    Swal.fire({
//                        text: errormsg
//                    });
//                }
//                else if (response == 0) {

//                }

//                else {
//                    $(".HProfileDetails").removeClass("d-none");
//                    $("#ForwardDetails").html("");
//                    $("#btnForward").removeClass("d-none");
//                    $("#spnCurrentspnRequestId").html(response.RequestId);
//                    //if (StepCounter == 1) {
//                    //    $(".spnFtoarmyno").html(response.IOArmyNo);
//                    //    $(".spnFtoname").html(response.IOName);


//                    //    $("#spnFrom").html(response.UserId);
//                    //    $("#spnForwardTo").html(response.IOUserId);
//                    //    $("#spnFwssusno").html(0);
//                    //} else if (StepCounter == 2) {
//                    //    $(".spnFtoarmyno").html(response.GSOArmyNo);
//                    //    $(".spnFtoname").html(response.GSOName);

//                    //    $("#spnFrom").html(response.IOUserId);
//                    //    $("#spnForwardTo").html(response.GSOUserId);
//                    //    $("#spnFwssusno").html(0);
//                    //}
//                    //else if (StepCounter == 3) {

//                    //    $(".HProfileDetails").addClass("d-none");
//                    //    $("#spnFrom").html(response.GSOUserId);
//                    //    $("#spnFwssusno").html(101);

//                    //}
//                    //else if (StepCounter == 4) {

//                    //    $(".HProfileDetails").addClass("d-none");
//                    //    $("#spnFrom").html(101);
//                    //    $("#spnForwardTo").html(29);
//                    //    $("#spnFwssusno").html(0);

//                    //}
//                }
//            }
//            else {
//                $(".HProfileDetails").addClass("d-none");
//                $("#btnForward").addClass("d-none");
//                $("#ForwardDetails").html("Please Add Self Profile");

//                $(".spnFtoarmyno").html("");
//                $(".spnFtoname").html("");
//                $("#spnForwardTo").html(0);
//                $("#spnCurrentspnRequestId").html(0);


//            }
//        },
//        error: function (result) {
//            Swal.fire({
//                text: errormsg002
//            });
//        }
//    });
//}
function ForwardTo(RequestId, HType) {

    var remarks = "" + $("#ddlRemarks").val() + "";
    var userdata = {
        "TrnFwdId": $("#spnCurrentspnTrnFwdId").html(),
        "RequestId": RequestId,
        "ToAspNetUsersId": $("#spnFwdToAspNetUsersId").html(),
        "ToUserId": $("#spnFwdToUsersId").html(),
        /*"FromUserId": $("#spnFrom").html(),*/
        // "ToUserId": $("#spnForwardTo").html(),
        /* "SusNo": $("#spnFwssusno").html(),*/
        "Remark": $("#txtFRemarks").val(),
        "FwdStatusId": 2,
        "TypeId": HType,
        "StepId": HType,
        "IsComplete": false,
        "RemarksIds": remarks,
    };
    $.ajax({
        url: '/BasicDetail/IcardFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {

                // if ($("#txtspnTokenArmyNo").val() != "") {
                if (HType == 2 || HType == 3 || HType == 4 || HType == 5) {
                    var lsts = new Array();
                    var ids = $("#spnCurrentspnRequestId").html();
                    lsts.push(ids);
                    if (IsToken == true) {

                        DataSignDigitaly(lsts, "tokenmsgforfwd", response.RequestId, HType);


                        //   DownloadPdf(RequestId);
                    } else {
                        DataSignDigitaly(lsts, "tokenmsgforfwd", response.RequestId, HType);
                        //setTimeout(function () {
                        //    location.reload();
                        //}, 2000);
                    }


                    // }
                    //else {
                    //    setTimeout(function () {
                    //        location.reload();
                    //    }, 2000);
                    //}
                } else {
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
            }
        }

    });
}

function RejecteTo(RequestId, HType) {
    var remarks = "" + $("#ddlRRemarks").val() + "";
    var userdata = {
        "TrnFwdId": 0,
        "RequestId": RequestId,
        "ToAspNetUsersId": $("#spnFwdToAspNetUsersId").html(),
        "ToUserId": $("#spnFwdToUsersId").html(),
        /*"FromUserId": $("#spnFrom").html(),*/
        // "ToUserId": $("#spnForwardTo").html(),
        /* "SusNo": $("#spnFwssusno").html(),*/
        "Remark": $("#txtFrejectedRemarks").val(),
        "FwdStatusId": 3,
        "TypeId": HType,
        "StepId": HType,
        "IsComplete": false,
        "RemarksIds": remarks
    };
    $.ajax({
        url: '/BasicDetail/IcardRejecte',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                setTimeout(function () {
                    location.reload();
                }, 2000);
            }
        }

    });
}

function UpdateStepCounter(stepId, spnRequestId, Counter, Flag) {
    var userdata = {
        "Id": stepId,
        "RequestId": spnRequestId,
        "StepId": Counter,
        "Flag": Flag
    };
    $.ajax({
        url: '/BasicDetail/UpdateStepCounter',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                if (response.Result == true) {
                    $("#FwdRecord").modal('hide');
                    var HType = 0;
                    if (Counter == 3) {
                        HType = 1;
                    } else if (Counter == 4) {
                        HType = 2;
                    }
                    if (Flag == "R") {
                        RejecteTo(spnRequestId, Counter);
                    } else {
                        ForwardTo(spnRequestId, Counter);
                    }
                    if (applyfor == 1) {
                        SaveNotification(1, Counter, $("#spnFwdToAspNetUsersId").html(), spnRequestId)
                    } else {
                        SaveNotification(1, (parseInt(Counter) + 10), $("#spnFwdToAspNetUsersId").html(), spnRequestId)
                    }
                }
                else {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: response.Message
                    });
                }
            }
        }

    });
}


function DataExport(Data) {

    var userdata = {
        "Ids": Data,
        "IsJco": $("#Isspnjcoor").html(),
        "DataExportType": DataExportType

    };
    $.ajax({
        url: '/BasicDetail/DataExport',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: "Data Not Export Internal Server Error"
                    });
                } else {
                    //var blob = new Blob([response], {
                    //    type: 'application/json'
                    //});
                    //var link = document.createElement('a');
                    //link.href = 'data:text/plain;charset=utf-8,' + encodeURIComponent(blob);
                    //link.download = "export.json";
                    //link.click();
                    if (DataExportType == 1) {
                        //window.location = "/WriteReadData/ExportAFSACCell/" + response + '_en.zip';
                        window.location = "/WriteReadData/ExportAFSACCell/" + response + '.zip';
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    } else {
                        window.location = "/WriteReadData/ExportAFSACCell/" + response + '.zip';
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    }


                    // var blob = new Blob([JSON.stringify(response, null, "\t")], { type: "application/json" });

                    // // Create a temporary anchor element
                    // var link = document.createElement("a");
                    // link.href = window.URL.createObjectURL(blob);




                    //// GetTokenSignXml(blob);
                    // // Set the file name
                    // link.download = "data.json";

                    // // Append the anchor to the body
                    // document.body.appendChild(link);

                    // // Trigger the click event
                    // link.click();

                    // // Remove the anchor from the body
                    // document.body.removeChild(link);


                    // setTimeout(function () {
                    //     location.reload();
                    // }, 1000);
                }


            }




        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function DataSignDigitaly(Data, msgid, RequestId, stepId) {
    var userdata = {
        "Ids": Data,
        "StepId": stepId
    };
    $.ajax({
        url: '/BasicDetail/DataDigitalXmlSign',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else {
                    if (IsDigitalSignReq == true) {
                        if (response.Id == undefined) {
                            var xmlString = jsonToXml(response);
                            GetTokenSignXml(xmlString, msgid, RequestId, 0)
                        }
                        else {
                            if (response.jsonfile == undefined) {
                                var xmlString1 = response.XmlFiles;
                                GetTokenSignXml(xmlString1, msgid, RequestId, response.Id)
                            } else {
                                var xmlString2 = jsonToXml(response.jsonfile);

                                GetTokenSignXml(xmlString2, msgid, RequestId, response.Id)
                            }
                        }
                    }
                    else {
                        if (stepId == 2) {
                            if (response.Id == undefined) {
                                var xmlString = jsonToXml(response);
                                SignXmlSendTOdatabase(xmlString, RequestId, 0);
                            } else {
                                var xmlString2 = jsonToXml(response.jsonfile);
                                SignXmlSendTOdatabase(xmlString2, RequestId, response.Id);
                            }
                        }
                        else {
                            var xmlString1 = response.XmlFiles;
                            SignXmlSendTOdatabase(xmlString1, RequestId, response.Id);
                        }
                    }
                }
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function GetTokenSignXml(xml, msgid, RequestId, Id) {
    $.ajax({
        url: HostUrlDGISToken + '/Temporary_Listen_Addresses/SignXml',
        type: "POST",
        contentType: 'application/xml', // Set content type to XML
        data: xml, // Set the XML data
        success: function (response) {
            if (response) {

                var xmlContent = new XMLSerializer().serializeToString(response);

                // No Token Found
                if (xmlContent.indexOf("<Root>No Token Found</Root>") == -1) {

                    $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected  </span></div>');

                    SignXmlSendTOdatabase(xmlContent, RequestId, Id);

                } else {
                    $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2"> No Token Found</span>.</div>');
                }
            }
        },
        error: function (result) {
            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Appl Not Running</span>.</div>');
        }
    });
}

function SignXmlSendTOdatabase(XmlFile, RequestId, Id) {
    var userdata = {
        "RequestId": RequestId,
        "XmlFiles": XmlFile,
        "Id": Id
    };
    $.ajax({
        url: '/Log/XmlFileDigitalSign',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else {
                    if (IsDigitalSignReq == true) {
                        toastr.success('Xml Digital Sign Sucess');
                    }
                    else {
                        toastr.success('Xml Digital Log Sucess');
                    }
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                }
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function jsonToXml(json) {
    var xml = '';
    for (var key in json) {
        i = 1;
        if (json.hasOwnProperty(key)) {

            xml += '<' + key + '>';

            if (typeof json[key] === 'object') {
                xml += jsonToXml(json[key]);
            } else {
                xml += json[key];
            }

            xml += '</' + key + '>';
        }
    }
    return xml;
}
function DownloadXml(RequestId) {
    var userdata = {
        "RequestId": RequestId,
    };
    $.ajax({
        url: '/Log/CreateXml',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else {

                    var url = "https://" + window.location.host + '/DigitallysignatureXml/' + response;
                    window.open(url, '_blank');
                }
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function DownloadPdf(RequestId) {
    var userdata = {
        "RequestId": RequestId,


    };
    $.ajax({
        url: '/Log/CreatePdf',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else {


                    //  window.open('/DigitallysignaturePdf/' + response, '_blank');
                    //  if ($("#aspntokenarmyno").html() == $("#txtspnTokenArmyNo").val()) {
                    var url = "https://" + window.location.host + '/DigitallysignaturePdf/' + response;
                    window.open(url, '_blank');
                    // digitalpdfsignature($("#txtspnTokenthumbprint").val(), url, '40', '65', RequestId);

                    // }
                    //var blob = new Blob([JSON.stringify(response, null, "\t")], { type: "application/json" });

                    //// Create a temporary anchor element
                    //var link = document.createElement("a");
                    //link.href = window.URL.createObjectURL(blob);




                    //// GetTokenSignXml(blob);
                    //// Set the file name
                    //link.download = "data.json";

                    //// Append the anchor to the body
                    //document.body.appendChild(link);

                    //// Trigger the click event
                    //link.click();

                    //// Remove the anchor from the body
                    //document.body.removeChild(link);
                }


            }




        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function digitalpdfsignature(Thumbprint, pdfpath, XCoordinate, YCoordinate, RequestId) {
    $("#loadingToken").show();
    $.ajax({
        url: HostUrlDGISToken + '/Temporary_Listen_Addresses/ByteDigitalSignAsync',
        type: "Post",
        contentType: 'application/json; charset=utf-8',
        'data': JSON.stringify([{
            "Thumbprint": Thumbprint,
            "pdfpath": pdfpath,
            "XCoordinate": XCoordinate,
            "YCoordinate": YCoordinate,

        }]),
        success: function (response) {
            if (response) {

                $("#loadingToken").hide();

                // No Token Found
                if (response.Valid) {

                    // base64toPDF(response.Message);
                    digitalpdfsignatureSave(RequestId, response.Message);
                    toastr.success('Pdf Digital Sign Sucess');
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                } else {
                    alert(response.Message)

                }
            }

        },
        error: function (result) {

            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Appl Not Running</span>.</div>');


        }
    });

}
function digitalpdfsignatureSave(RequestId, base64) {
    var userdata = {
        "RequestId": RequestId,
        "base64": base64,


    };
    $.ajax({
        url: '/Log/DigitalpdfsignatureSave',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else {


                    //  window.open('/DigitallysignaturePdf/' + response, '_blank');
                    //  if ($("#aspntokenarmyno").html() == $("#txtspnTokenArmyNo").val()) {
                    var url = "https://" + window.location.host + '/DigitallysignaturePdf/' + response;
                    window.open(url, '_blank');

                }


            }




        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function base64toPDF(data) {
    var datat = data;
    var bufferArray = base64ToArrayBuffer(data);
    var blobStore = new Blob([bufferArray], {
        type: "application/pdf"
    });
    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobStore);
        return;
    }
    var data = window.URL.createObjectURL(blobStore);
    var link = document.createElement('a');
    document.body.appendChild(link);
    link.href = data;
    link.download = "digitalsignature.pdf";
    link.click();

    const fileWindow = window.open();
    const url = 'data:application/pdf;base64,' + btoa(
        new Uint8Array(bufferArray)
            .reduce((data1, byte) => data1 + String.fromCharCode(byte), '')
    );
    fileWindow.document.write(
        '<title>Digital Signature Pdf</title>' +
        '<body style="overflow: hidden; margin: 0">' +
        '<object width="100%" width="-webkit-fill-available" height="100%" height="-webkit-fill-available" type="application/pdf" data="' + encodeURI(url) + '"></object>' +
        '</body>'
    );

    window.URL.revokeObjectURL(data);
    link.remove();
}

function base64ToArrayBuffer(data) {
    var bString = window.atob(data);
    var bLength = bString.length;
    var bytes = new Uint8Array(bLength);
    for (var i = 0; i < bLength; i++) {
        var ascii = bString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
};

function GetByArmyNoIsToken(ArmyNo, OffType, RegApplyFor, stepCounter) {
    var userdata = {
        "ArmyNo": ArmyNo,
    };
    $.ajax({
        url: '/UserProfile/GetByArmyNoOrAspnetuserId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else if (response == 0) {

                } else {
                    IsToken = response.IsToken;
                    IsWithTokenApply = response.IsWithTokenApply

                    if (parseInt(OffType) == 1) {
                        if (parseInt(RegApplyFor) == 2 || parseInt(RegApplyFor) == 3 || parseInt(RegApplyFor) == 4 || parseInt(RegApplyFor) == 10) {
                            //  Other Officer Appl Request
                            if (parseInt(stepCounter) == 1 || parseInt(stepCounter) == 2 || parseInt(stepCounter) == 7) {
                                if (IsWithTokenApply == true) {
                                    $("#btntokenTofwd").removeClass("d-none");
                                }
                                else {
                                    $("#btntokenTofwd").addClass("d-none");
                                }
                            }
                            else {
                                if (IsToken == true) {
                                    $("#btntokenTofwd").removeClass("d-none");
                                } else {
                                    $("#btntokenTofwd").addClass("d-none");
                                }
                            }
                        }
                        else {
                            // Self Officer Appl Request
                            if (parseInt(stepCounter) == 1 || parseInt(stepCounter) == 2 || parseInt(stepCounter) == 7) {
                                if (IsToken == true && parseInt(RegApplyFor) == 1) {
                                    $("#btntokenTofwd").removeClass("d-none");
                                }
                                else {
                                    $("#btntokenTofwd").addClass("d-none");
                                }
                            } else {
                                if (IsToken == true) {
                                    $("#btntokenTofwd").removeClass("d-none");
                                } else {
                                    $("#btntokenTofwd").addClass("d-none");
                                }
                            }

                        }
                    }
                    else {

                        // JCO/OR Appl Request
                        if (parseInt(stepCounter) == 1 || parseInt(stepCounter) == 7 || parseInt(stepCounter) == 8 || parseInt(stepCounter) == 9 || parseInt(stepCounter) == 10) {
                            $("#btntokenTofwd").addClass("d-none");
                        } else {
                            if (IsToken == true) {
                                $("#btntokenTofwd").removeClass("d-none");
                            }
                            else {
                                $("#btntokenTofwd").addClass("d-none");
                            }

                        }
                    }
                }
            }

        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}