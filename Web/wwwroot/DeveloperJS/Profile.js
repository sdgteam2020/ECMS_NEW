
$(document).ready(function () {

    $("#btnAddProfile").click(function () {
        reset();
        $("#AddNewProfile").modal('show');

    });
    DataBindAll();

    $(".GSO").addClass("d-none");
    $(".IO").addClass("d-none");
  
    $("#btnGSOProAdd").click(function () {
        $(".GSO").removeClass("d-none");
        $(".GSOlbl").addClass("d-none");
        $("#btnGSOProAdd").addClass("d-none");
        $("#btnGSOProremove").removeClass("d-none");
        $("#btnSaveGSO").removeClass("d-none");

       $("#txtProGSOArmyNo").val('');
       $("#ddlProGSORank").val(0);
       $("#txtProGSOName").val('');
       $("#ddlProGSOAppointment").val(0);
       $("#spnUnitIdGSO").html(0);
    });
    $("#btnGSOProremove").click(function () {
        $(".GSO").addClass("d-none");
        $(".GSOlbl").removeClass("d-none");
        $("#btnGSOProAdd").removeClass("d-none");
        $("#btnGSOProremove").addClass("d-none");
        $("#btnSaveGSO").addClass("d-none");


        $("#spnUserIdGSO").html(0);
        $("#txtProGSOArmyNo").html('');
        $("#lblProGSORank").html('');
        $("#lblProGSOName").html('');
        $("#lblProGSOFormation").html('');
        $("#lblProGSOAppointment").html('');
        $("#lblGSOProGSOUnit").html('');
    });


    //$("#btnIOProAdd").click(function () {
    //    $(".IO").removeClass("d-none");
    //    $(".IOlbl").addClass("d-none");
    //    $("#btnIOProAdd").addClass("d-none");
    //    $("#btnIOProremove").removeClass("d-none");
    //    $("#btnSaveIO").removeClass("d-none");


    //    $("#txtProIOArmyNo").val('');
    //    $("#ddlProIORank").val(0);
    //    $("#txtProIOName").val('');
    //    $("#ddlProIOAppointment").val(0);
    //    $("#spnUnitIdIO").html(0);
    //});
    //$("#btnIOProremove").click(function () {
    //    $(".IO").addClass("d-none");
    //    $(".IOlbl").removeClass("d-none");
    //    $("#btnIOProAdd").removeClass("d-none");
    //    $("#btnIOProremove").addClass("d-none");
    //    $("#btnSaveIO").addClass("d-none");


    //    $("#spnUserIdIO").html(0);
    //    $("#txtProIOArmyNo").html('');
    //    $("#lblProIORank").html('');
    //    $("#lblProIOName").html('');
    //    $("#lblProIOFormation").html('');
    //    $("#lblProIOAppointment").html('');
    //    $("#lblIOProIOUnit").html('');
    //});
    $("#btnSaveIO").click(function () {
       /* if ($("#SaveIOForm")[0].checkValidity()) {*/

            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be Save!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Save it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    var ArmyNo = $("#txtProIOArmyNo").val();
                    var Rank = $("#ddlProIORank").val();
                    var Name = $("#txtProIOName").val();
                    var Appt = $("#ddlProIOAppointment").val();
                    var Unit = $("#spnUnitIdIO").html();
                    var UserId = $("#spnUserIdIO").html();

                    SaveUserProfile(ArmyNo, Rank, Name, Appt, Unit, 1, 0, 0, UserId)
                }                          
            })                             
                                           
        //} else {
        //    $("#SaveIOForm")[0].reportValidity();
        //}



        // 

    });
    $("#btnSaveGSO").click(function () {
        /* if ($("#SaveIOForm")[0].checkValidity()) {*/

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be Save!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Save it!'
        }).then((result) => {
            if (result.isConfirmed) {
                var ArmyNo = $("#txtProGSOArmyNo").val();
                var Rank = $("#ddlProGSORank").val();
                var Name = $("#txtProGSOName").val();
                var Appt = $("#ddlProGSOAppointment").val();
                var Unit = $("#spnUnitIdGSO").html();
                var UserId = $("#spnUserIdGSO").html();
                SaveUserProfile(ArmyNo, Rank, Name, Appt, Unit, 2,0,0,UserId)
            }
        })

        //} else {
        //    $("#SaveIOForm")[0].reportValidity();
        //}



        // 

    });


    $("#btnProfilesave").click(function () {

        if ($("#SaveFormProfile")[0].checkValidity()) {
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be Save!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Save it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    var ArmyNo = $("#txtProArmy").val();
                    var Rank = $("#ddlProRank").val();
                    var Name = $("#txtName").val();
                    var Appt = $("#ddlProAppointment").val();
                    var Unit = $("#spnUnitIdUserPro").html();
                    var userid = $("#spnUserId").html();
                    //SaveUserProfile(ArmyNo, Rank, Name, Appt, Unit, 2)
                    //alert($("#txtProArmy").val());
                    //alert($("#ddlProRank").val());
                    //alert($("#txtName").val());
                    //alert($("#ddlProFormation").val());
                    //alert($("#ddlProAppointment").val());
                    //alert($("#spnUnitIdUserPro").html());
                  ///  alert($("#spnUserIdIO").html());
                    // alert($("#spnUserIdGSO").html());
                    if (parseInt(Unit) == 0) {
                        toastr.error('Please Enter Unit');
                    }
                    else if (parseInt($("#spnUserIdIO").html()) == 0) {
                        toastr.error('Please Enter IO Details');
                    } else if (parseInt($("#spnUserIdGSO").html()) == 0) {
                        toastr.error('Please Enter GSO Details');
                    }
                    else {
                        SaveUserProfile(ArmyNo, Rank, Name, Appt, Unit, 3, $("#spnUserIdIO").html(), $("#spnUserIdGSO").html(), userid)
                    }
                    
                }
            })
             } else {
            $("#SaveFormProfile")[0].reportValidity();
        }

    });


    mMsater(0, "ddlProIOFormation", Formation, "");
    mMsater(0, "ddlProIORank", Rank, "");
    $('#ddlProIOFormation').on('change', function () {

        mMsater(0, "ddlProIOAppointment", Appt, $('#ddlProIOFormation').val());

    });

    mMsater(0, "ddlProGSOFormation", Formation, "");
    mMsater(0, "ddlProGSORank", Rank, "");
    $('#ddlProGSOFormation').on('change', function () {

        mMsater(0, "ddlProGSOAppointment", Appt, $('#ddlProGSOFormation').val());
       
    });

    mMsater(0, "ddlProFormation", Formation, "");
    mMsater(0, "ddlProRank", Rank, "");
    $('#ddlProFormation').on('change', function () {

        mMsater(0, "ddlProAppointment", Appt, $('#ddlProFormation').val());
        
    });

    $("#btnProUnitAdd").click(function () {
        
        $("#AddNewUnitmap").modal('show');

    });
    $("#btnIOUnitAdd").click(function () {
        
        $("#AddNewUnitmap").modal('show');

    });
    $("#btnGSOUnitAdd").click(function () {
        
        $("#AddNewUnitmap").modal('show');

    });
  

   $("#txtProArmy").autocomplete({


            source: function (request, response) {

           //var param = { "ICNumber": request.term };
           if (isNaN(request.term) && request.term.length == 8) {

               //$.ajax({
               //    url: 'https://localhost:7002/api/Fetch/GetData/' + request.term,
               //    contentType: 'application/x-www-form-urlencoded',

               //    type: 'GET',
               //    success: function (data) {
               //        console.log(data);
               //        response($.map(data, function (item) {

               //            $("#loading").addClass("d-none");
               //            return { label: item.ServiceNo, value: item.ServiceNo };

               //        }))
               //    },
               //    error: function (response) {
               //        alert(response.responseText);
               //    },
               //    failure: function (response) {
               //        alert(response.responseText);
               //    }
               //});
            
                   $.ajax({
                       url: "/BasicDetail/GetData",
                       type: "POST",
                       data: {
                           "ICNumber": request.term
                       },
                       success: function (response, status) {
                           if (response.Name != "") {
                               $('#txtProArmy').val(response.ServiceNo);
                               $('#txtName').val(response.Name);
                           }
                       }
                   });
              
           }
            },
            select: function (e, i) {
                e.preventDefault();
                $("#txtProArmy").val(i.item.label);
                alert(i.item.value)
                //var param1 = { "UnitMapId": i.item.value };
                //$.ajax({
                //    url: 'https://192.168.10.203:7002/api/Fetch/GetData/' + i.item.value,
                //    contentType: 'application/x-www-form-urlencoded',

                //    type: 'GET',
                //    success: function (data) {


                //        alert(data.Name)

                //    }
                //});

            },
            appendTo: '#suggesstion-box'
        });
    $("#txtProUnit").autocomplete({


        source: function (request, response) {

            var param = { "UnitName": request.term };
            $("#spnUnitIdUserPro").html(0);
            $.ajax({
                url: '/Master/GetALLByUnitName',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);
                    response($.map(data, function (item) {

                        $("#loading").addClass("d-none");
                        return { label: item.UnitName, value: item.UnitMapId };

                    }))
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
            $("#txtProUnit").val(i.item.label);
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value};
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {


                    $("#spnUnitIdUserPro").html(data.UnitId);
                    $("#lblProComd").html(data.ComdName);
                    $("#lblProCorps").html(data.CorpsName);
                    $("#lblProDiv").html(data.DivName);
                    $("#lblPrBde").html(data.BdeName);
                    $("#lblProSusno").html(data.Sus_no + '' + data.Suffix);
                    
                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $("#txtProIOformunit").autocomplete({


        source: function (request, response) {

            var param = { "UnitName": request.term };
            $("#spnUnitIdIO").html(0);
            $.ajax({
                url: '/Master/GetALLByUnitName',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);
                    response($.map(data, function (item) {

                        $("#loading").addClass("d-none");
                        return { label: item.UnitName, value: item.UnitMapId };

                    }))
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
            $("#txtProIOformunit").val(i.item.label);
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {


                    $("#spnUnitIdIO").html(data.UnitId);
                  

                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $("#txtProGSOformunit").autocomplete({


        source: function (request, response) {

            var param = { "UnitName": request.term };
            $("#spnUnitIdGSO").html(0);
            $.ajax({
                url: '/Master/GetALLByUnitName',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);
                    response($.map(data, function (item) {

                        $("#loading").addClass("d-none");
                        return { label: item.UnitName, value: item.UnitMapId };

                    }))
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
            $("#txtProGSOformunit").val(i.item.label);
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {


                    $("#spnUnitIdGSO").html(data.UnitId);


                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $("#txtProIOArmyNo").blur(function () {
        if ($("#spnUserIdIO").html() == 0) {
            $.ajax({
                url: "/BasicDetail/GetData",
                type: "POST",
                data: {
                    "ICNumber": $("#txtProIOArmyNo").val()
                },
                success: function (response, status) {
                   
                    if (status == "success") {
                        $("#spnUserIdIO").html(0)
                        $('#txtProIOArmyNo').val(response.ServiceNo);
                        $('#txtProIOName').val(response.Name);


                        $("#spnUserIdIO").html(0);
                        $("#ddlProIORank").val('');
                        $("#ddlProIOFormation").val('');
                        $("#ddlProIOAppointment").val('');
                       
                        $("#txtProIOformunit").val('');
                        $("#txtProIOformunit").attr('readonly', false);
                        $("#spnUnitIdIO").html(''); 
                        $("#btnSaveIO").removeClass("d-none");
                    }
                    else {
                toastr.error('IO Army No Not Found in API!');

                    }
                }
            });

        }
    });

    $("#txtProIOArmyNo").autocomplete({


        source: function (request, response) {

            var param = { "ArmyNo": request.term };
            $("#spnUserIdIO").html(0);
            $.ajax({
                url: '/UserProfile/GetByMasterArmyNo',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);
                   
                    response($.map(data, function (item) {

                        $("#loading").addClass("d-none");
                        return { label: item.ArmyNo, value: item.UserId };

                    }))
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
            $("#txtProIOArmyNo").val(i.item.label);
            //alert(i.item.value)
            // var param1 = { "UnitMapId": i.item.value };
           
            GetByArmyNo(i.item.label, 1) 
        },
        appendTo: '#suggesstion-box'
    });
    $("#txtProGSOArmyNo").blur(function () {
        if ($("#spnUserIdGSO").html() == 0) {
            $.ajax({
                url: "/BasicDetail/GetData",
                type: "POST",
                data: {
                    "ICNumber": $("#txtProGSOArmyNo").val()
                },
                success: function (response, status) {

                    if (status == "success") {
                        $("#spnUserIdGSO").html(0)
                        $('#txtProGSOArmyNo').val(response.ServiceNo);
                        $('#txtProGSOName').val(response.Name);
                    }
                    else {
                        toastr.error('GSO Army No Not Found in API!');

                    }
                }
            });

        }
    });
    $("#txtProGSOArmyNo").autocomplete({


        source: function (request, response) {

            var param = { "ArmyNo": request.term };
            $("#spnUserIdGSO").html(0);
            $.ajax({
                url: '/UserProfile/GetByMasterArmyNo',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);
                    response($.map(data, function (item) {

                        $("#loading").addClass("d-none");
                        return { label: item.ArmyNo, value: item.UserId };

                    }))
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
            $("#txtProGSOArmyNo").val(i.item.label);
            //alert(i.item.value)
            // var param1 = { "UnitMapId": i.item.value };

            GetByArmyNo(i.item.label, 2,0,0,0)
        },
        appendTo: '#suggesstion-box'
    });
});
function DataBindAll() {
    var listItem = "";
    var userdata =
    {
        "Id": '0',

    };
    $.ajax({
        url: '/UserProfile/GetAll',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == -1) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=15>No Record Found</td></tr>";
                    $("#tbldataProfile").DataTable().destroy();
                    $("#DetailBodyProfile").html(listItem);
                    $("#lblTotalProfile").html(0);
                }
                else if (response == InternalServerError) {
                    listItem += "<tr><td class='text-center' colspan=15>No Record Found</td></tr>";
                    $("#tbldataProfile").DataTable().destroy();
                    $("#DetailBodyProfile").html(listItem);
                    $("#lblTotalProfile").html(0);
                }

                else {
                  
                        //ArmyNo = user.ArmyNo,
                        //UserId = user.UserId,
                        //FormationId = forma.FormationId,
                        //FormationName = forma.FormationName,
                        //ApptId = app.ApptId,
                        //AppointmentName = app.AppointmentName,
                        //Rank = rank.RankName,
                        //Name = user.Name,
                        //UnitId = user.UnitId,
                        //Unit_desc = Uni.Unit_desc + Uni.Suffix,


                        //IOArmyNo = userio.ArmyNo,
                        //IOName = userio.Name,
                        //IOUserId = userio.UserId,
                        //UnitIdIo = UniO.UnitId,
                        //UnitIo = UniO.Unit_desc + UniO.Suffix,

                        //GSOArmyNo = usergso.ArmyNo,
                        //GSOName = usergso.Name,
                        //GSOUserId = usergso.UserId,
                        //UnitIdGSO = UnGSO.UnitId,
                        //UnitGSO = UnGSO.Unit_desc + UniO.Suffix,

                     //<th class=" wd-30-f">S No</th>
                     //           <th class="nowrap">Army No</th>
                     //           <th class="nowrap">Name</th>
                     //           <th class="nowrap">Rank</th>
                     //           <th class="nowrap">Appt.</th>
                     //           <th class="nowrap">Unit Name(SUSNo)</th>
                     //           <th class="nowrap">IO Army No</th>
                     //           <th class="nowrap">IO Name</th>
                     //           <th class="nowrap">IO Unit Name(SUSNo)</th>
                     //           <th class="nowrap">GSO Army No</th>
                     //           <th class="nowrap">GSO Name</th>
                     //           <th class="nowrap">GSO Unit Name(SUSNo)</th>
                     //           <th class="nowrap">Action</th>

                    for (var i = 0; i < response.length; i++) {
                     
                            listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnMMapId'>" + response[i].MapId + "</span><span id='spnproUserId'>" + response[i].UserId + "</span><span id='spnproUserUnitId'>" + response[i].UnitId + "</span></td>";
                            listItem += "<td>";
                            listItem += "<div class='custom-control custom-checkbox small'>";
                            listItem += "<input type='checkbox' class='custom-control-input' id='" + response[i].MapId + "'>";
                            listItem += "<label class='custom-control-label' for='" + response[i].MapId + "'></label>";
                            listItem += "</div>";
                            listItem += "</td>";
                            listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                            listItem += "<td class='align-middle'><span id='ArmyNo'>" + response[i].ArmyNo + "</span></td>";
                            listItem += "<td class='align-middle'><span id='Name'>" + response[i].Name + "</span></td>";
                            listItem += "<td class='align-middle'><span id='Rank'>" + response[i].Rank + "</span></td>";
                            listItem += "<td class='align-middle'><span id='AppointmentName'>" + response[i].AppointmentName + "</span></td>";
                             listItem += "<td class='align-middle'><span id='Unit_desc'>" + response[i].Unit_desc + "(" + response[i].SusNo +")</span></td>";
                            listItem += "<td class='align-middle'><span id='IOArmyNo'>" + response[i].IOArmyNo + "</span></td>";
                            listItem += "<td class='align-middle'><span id='IOName'>" + response[i].IOName + "</span></td>";
                             listItem += "<td class='align-middle'><span id='UnitIo'>" + response[i].UnitIo + "(" + response[i].IOSusNo +")</span></td>";

                            listItem += "<td class='align-middle'><span id='GSOArmyNo'>" + response[i].GSOArmyNo + "</span></td>";
                            listItem += "<td class='align-middle'><span id='GSOName'>" + response[i].GSOName + "</span></td>";
                              listItem += "<td class='align-middle'><span id='UnitGSO'>" + response[i].UnitGSO + "(" + response[i].GSOSusNo +")</span></td>";

                            listItem += "<td class='align-middle'><span id='btneditpro'><button type='button' class='cls-btneditpro btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";


                            /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                            listItem += "</tr>";
                       
                    }

                    $("#DetailBodyProfile").html(listItem);
                    $("#lblTotalProfile").html(response.length);

                    memberTable = $('#tbldataProfile').DataTable({
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

                    memberTable.buttons().container().appendTo('#tbldataProfile_wrapper .col-md-6:eq(0)');

                    var rows;
                    $("#tbldataProfile #chkAll").click(function () {
                        if ($(this).is(':checked')) {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                        else {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                    });
                    $('#DetailBodyProfile').on('change', 'input[type="checkbox"]', function () {
                        if (!this.checked) {
                            var el = $('#chkAll').get(0);
                            if (el && el.checked && ('indeterminate' in el)) {
                                el.indeterminate = true;
                            }
                        }
                    });

                    $("body").on("click", ".cls-btneditpro", function () {
                        $("#AddNewProfile").modal('show');

                      //  $(this).closest("tr").find("#spnproUserUnitId").html()

                        $("#spnProfileMapId").html($(this).closest("tr").find("#spnMMapId").html());
                        GetByArmyNo($(this).closest("tr").find("#ArmyNo").html(), 0) 
                        GetByArmyNo($(this).closest("tr").find("#IOArmyNo").html(), 1) 
                        GetByArmyNo($(this).closest("tr").find("#GSOArmyNo").html(), 2) 
                        //$(".spnCorpsId").html($(this).closest("tr").find("#spnMcorpsId").html());
                        //$("#txtCoprsName").val($(this).closest("tr").find("#corpsName").html());


                        var param1 = { "UnitId": $(this).closest("tr").find("#spnproUserUnitId").html() };
                        $.ajax({
                            url: '/Master/GetALLByUnitById',
                            contentType: 'application/x-www-form-urlencoded',
                            data: param1,
                            type: 'POST',
                            success: function (data) {


                                $("#spnUnitIdUserPro").html(data.UnitId);
                                $("#txtProUnit").val(data.UnitName);
                                $("#lblProComd").html(data.ComdName);
                                $("#lblProCorps").html(data.CorpsName);
                                $("#lblProDiv").html(data.DivName);
                                $("#lblPrBde").html(data.BdeName);
                                $("#lblProSusno").html(data.Sus_no + '' + data.Suffix);

                            }
                        });

                        $("#btnsave").val("Update");
                    });


                    $("body").on("click", ".cls-btnDelete", function () {

                        Swal.fire({
                            title: 'Are you sure?',
                            text: "You want to Delete ",
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#072697',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Yes, Delete It!'
                        }).then((result) => {
                            if (result.value) {

                                Delete($(this).closest("tr").find("#spnMcorpsId").html());

                            }
                        });
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=10>No Record Found</td></tr>";
                $("#tbldata").DataTable().destroy();
                $("#DetailBody").html(listItem);
                $("#lblTotal").html(0);
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });

}
function SaveUserProfile(ArmyNo, Rank, Name, Appt, Unit, Type, IO, GSO, UserId) {

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/UserProfile/SaveUserProfile',
        type: 'POST',
        data: { "ArmyNo": ArmyNo, "RankId": Rank, "Name": Name, "ApptId": Appt, "UnitId": Unit, "UserId": UserId }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('User has been saved');
                GetByArmyNo(ArmyNo, Type, Unit,IO, GSO);
                /*  $("#AddNewM").modal('hide');*/
                
            }
            else if (result == DataUpdate) {
                toastr.success('User has been Updated');

                /*  $("#AddNewM").modal('hide');*/
               
            }
            else if (result == DataExists) {

                toastr.error('Army No Exits!');

            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }


            }
        }
    });
}
function GetByArmyNo(ArmyNo, Type, Unit, IO, GSO) {
   
    var userdata =
    {
        "ArmyNo": ArmyNo,

    };
    $.ajax({
        url: '/UserProfile/GetByArmyNo',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    $("#spnUserIdIO").html(0);
                }

                else {

                    if (Type == 0) {

                        $("#spnUserId").html(response.UserId);
                        $("#txtProArmy").val(response.ArmyNo);
                        

                        mMsater(response.FormationId, "ddlProFormation", Formation, "");

                        mMsater(response.RankId, "ddlProRank", Rank, "");
                        
                        mMsater(response.ApptId, "ddlProAppointment", Appt, response.FormationId);

                       
                       
                        $("#txtName").val(response.Name);
                      
                       

                    }
                   else if (Type == 1) {
                       
                        $("#spnUserIdIO").html(response.UserId);
                        $("#txtProIOArmyNo").val(response.ArmyNo);
                        //$("#lblProIORank").html(response.Rank);
                        //$("#lblProIOName").html(response.Name);
                        //$("#lblProIOFormation").html(response.FormationName);
                        //$("#lblProIOAppointment").html(response.AppointmentName);
                        //$("#lblIOProIOUnit").html(response.Unit_desc);
                       // $("#lblProIORank").html(response.Rank);
                        $("#txtProIOName").val(response.Name);
                       // $("#lblProIOFormation").html(response.FormationName);
                       // $("#lblProIOAppointment").html(response.AppointmentName);
                       // $("#lblIOProIOUnit").html(response.Unit_desc);

                       
                        mMsater(response.RankId, "ddlProIORank", Rank, "");
                       

                        mMsater(response.FormationId, "ddlProIOFormation", Formation, "");
                        mMsater(response.ApptId, "ddlProIOAppointment", Appt, response.FormationId);
                        $("#txtProIOformunit").val(response.Unit_desc);
                        $("#txtProIOformunit").attr('readonly', true);
                        $("#spnUnitIdIO").html(response.UnitId); 

                        //$(".IO").addClass("d-none");
                        //$(".IOlbl").removeClass("d-none");
                        //$("#btnIOProAdd").removeClass("d-none");
                        //$("#btnIOProremove").addClass("d-none");
                        $("#btnSaveIO").addClass("d-none");
                    } else if (Type == 2) {

                        $("#spnUserIdGSO").html(response.UserId);
                        $("#txtProGSOArmyNo").val(response.ArmyNo);
                        $("#lblProGSORank").html(response.Rank);
                        $("#lblProGSOName").html(response.Name);
                        $("#lblProGSOFormation").html(response.FormationName);
                        $("#lblProGSOAppointment").html(response.AppointmentName);
                        $("#lblGSOProGSOUnit").html(response.Unit_desc);


                        $(".GSO").addClass("d-none");
                        $(".GSOlbl").removeClass("d-none");
                        $("#btnGSOProAdd").removeClass("d-none");
                        $("#btnGSOProremove").addClass("d-none");
                        $("#btnSaveGSO").addClass("d-none");
                    }
                    else if(Type == 3)
                    {
                        MappingIOGSOUNIT(response.UserId,Unit, IO, GSO);
                    }

                }
            }
            else {
               
                $("#spnUserIdIO").html(0);
                
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function MappingIOGSOUNIT(UserId, Unit, IO, GSO) {

    var userdata =
    {
        "Id": $("#spnProfileMapId").html(),
        "UserId": UserId,
        "UnitId": Unit,
        "IOId": IO,
        "GSOId": GSO,

    };
    $.ajax({
        url: '/UserProfile/MappingIOGSOUNIT',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (result) {


            if (result == DataSave) {


                toastr.success('Data has been saved');
                $("#AddNewProfile").modal('show');
                reset();
                DataBindAll();
                $("#AddNewProfile").modal('hide');

            }
            else if (result == DataUpdate) {


                toastr.success('Data has been Updated');
                $("#AddNewProfile").modal('show');
                reset();
                DataBindAll();
                $("#AddNewProfile").modal('hide');
            }
            else if (result == DataExists) {

                toastr.error('Army No Exits!');
            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }

            }
        }
  
       
    });
}

function reset() {

    $("#spnProfileMapId").html(0);
    $(".GSO").addClass("d-none");
    $(".GSOlbl").removeClass("d-none");
    $("#btnGSOProAdd").removeClass("d-none");
    $("#btnGSOProremove").addClass("d-none");
    $("#btnSaveGSO").addClass("d-none");


    $("#spnUserIdGSO").html(0);
    $("#txtProGSOArmyNo").html('');
    $("#lblProGSORank").html('');
    $("#lblProGSOName").html('');
    $("#lblProGSOFormation").html('');
    $("#lblProGSOAppointment").html('');
    $("#lblGSOProGSOUnit").html('');


    $(".IO").addClass("d-none");
    $(".IOlbl").removeClass("d-none");
    $("#btnIOProAdd").removeClass("d-none");
    $("#btnIOProremove").addClass("d-none");
    $("#btnSaveIO").addClass("d-none");


    $("#spnUserIdIO").html(0);
    $("#txtProIOArmyNo").html('');
    $("#lblProIORank").html('');
    $("#lblProIOName").html('');
    $("#lblProIOFormation").html('');
    $("#lblProIOAppointment").html('');
    $("#lblIOProIOUnit").html('');

    $("#txtProArmy").val('');
    $("#ddlProRank").val(0);
    $("#txtName").val('');
    $("#ddlProAppointment").val(0);
    $("#spnUnitIdUserPro").html(0);
    $("#spnUserIdIO").html(0)
    $("#spnUserIdGSO").html(0)



    $("#txtProUnit").html('');
    $("#lblProSusno").html('');
    $("#lblProComd").html('');
    $("#lblProCorps").html('');
    $("#lblProDiv").html('');
    $("#lblPrBde").html('');
}
