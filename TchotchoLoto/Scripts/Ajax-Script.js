
$(document).ready(
    function () {

        InitializeSectionOuvrierToAddDT();

        InitializeSectionTravailOuvrierDT();

        //Begin Populate PayCompagniePaiementId on dropdown PayMethodePaiementId change
        var ddCP = $('#PayCompagniePaiementId');
        var noCB = $('#NoCompteBancaire');

        if ($('#disabledCPNCB').length && $('#disabledCPNCB').val() == "True") {
            ddCP.prop('disabled', true);
            noCB.prop('readonly', true);
        }

        $('#ddlMethodePaiementId').change(

            function () {

                var id = 'MPId=' + $(this).val();

                $.getJSON({
                    url: $('#url-get-cp').data('url'),
                    data: id,
                    success: function (data) {
                        if (data != null) {

                            if (data["typeCompagnie"] == "ctel") {
                                $('#label-NoCompteBancaire').text('Phone Number');
                            }
                            else {
                                $('#label-NoCompteBancaire').text('Bank Account Number');
                            }

                            ddCP.prop('disabled', false);
                            noCB.prop('readonly', false);

                            ddCP.html('');

                            if (data["CPList"].length == 0) {

                                ddCP.prop('disabled', true);
                                noCB.prop('readonly', true);
                                noCB.val('');

                            }

                            for (var item in data["CPList"]) {

                                ddCP.append('<option value = "' + data["CPList"][item]["PayCompagniePaiementId"] + '" >' + data["CPList"][item]["Description"] + '</options>');

                            }

                        }
                        else {

                            ddCP.prop('disabled', true);
                            noCB.prop('readonly', true);
                            noCB.val('');
                        }

                    }

                });

            }

        );
        //End

        //Begin Populate StyleFamilleId on dropdown ProductionStyleFamilleId change <<Add Production>>
        $('#ddlProductionStyleFamilleId').change(

            function () {

                var ddlSF = $('#ddlStyleFamilleId');
                var id = 'PSFId=' + $(this).val() + "&FloorId=" + $('#Add-Production-Section-Form .FloorId').val() + "&DP=" + $('#Add-Production-Section-Form .DP').val();

                $('#StyleId').html('');
                $('#SizeId').html('');
                ddlSF.html('');
                ddlSF.append('<option>Select a Style Category</options>');


                $.getJSON({
                    url: $('#url-get-sf-ddl-id').data('url'),
                    data: id,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["SFList"]) {

                                ddlSF.append('<option value = "' + data["SFList"][item]["StyleFamilleId"] + '" >' + data["SFList"][item]["Label"] + '</options>');

                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate StyleId and SizeId on dropdown StyleFamilleId change <<Add Production>>
        $('#ddlStyleFamilleId').change(

            function () {


                var ddlStyle = $('#StyleId');
                var ddlSize = $('#SizeId');
                var id = 'SFId=' + $(this).val() + "&PSFId=" + $('#ddlProductionStyleFamilleId').val() + "&FloorId=" + $('#Add-Production-Section-Form .FloorId').val() + "&DP=" + $('#Add-Production-Section-Form .DP').val();

                $('#StyleFamilleId').attr('value', $(this).val());

                ddlStyle.html('');
                ddlSize.html('');


                $.getJSON({
                    url: $('#url-get-style-and-size-ddl-id').data('url'),
                    data: id,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["StyleList"]) {

                                ddlStyle.append('<option value = "' + data["StyleList"][item]["StyleId"] + '" >' + data["StyleList"][item]["CodeStyle"] + '</options>');

                            }

                            for (var item in data["SizeList"]) {

                                ddlSize.append('<option value = "' + data["SizeList"][item]["SizeId"] + '" >' + data["SizeList"][item]["Description"] + '</options>');

                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate ddlAnneeFiscaleId  on dropdown PayCategorieId change <<Report Payroll>>
        $('.PayCategorieId #ddlPayCategorieId').change(

            function () {

                var ddlAnneeFiscale = $('#ddlPayAnneeFiscaleId');

                var id = 'CatId=' + $(this).val();

                ddlAnneeFiscale.html('');
                ddlAnneeFiscale.append('<option>Fiscal Years</options>');

                $.getJSON({
                    url: $('#url-get-annee-fiscale-ddl-id').data('url'),
                    data: id,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["AFList"]) {

                                ddlAnneeFiscale.append('<option value = "' + data["AFList"][item]["PayAnneeFiscaleId"] + '" >' + data["AFList"][item]["Label"] + '</options>');

                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate ddlPeriodePayrollId  on dropdown PayAnneeFiscaleId change <<Report Payroll>>
        $('.PayAnneeFiscaleId #ddlPayAnneeFiscaleId').change(

            function () {

                var ddlPeriodePayroll = $('#ddlPayPeriodePayrollId');

                var id = 'CatId=' + $('#ddlPayCategorieId').val() + '&AFId=' + $(this).val();

                ddlPeriodePayroll.html('');
                ddlPeriodePayroll.append('<option>Payrolls Period</options>');

                $.getJSON({
                    url: $('#url-get-periode-payroll-ddl-id').data('url'),
                    data: id,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["PPList"]) {

                                ddlPeriodePayroll.append('<option value = "' + data["PPList"][item]["PayPeriodePayrollId"] + '" >' + data["PPList"][item]["Label"] + '</options>');

                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate ddlPayPosteId  on dropdown ddlPayTypeEmployeId change <<Add Employee>>
        $('#ddlPayTypeEmployeId').change(

            function () {

                var ddlPosteId = $('#ddlPayPosteId');

                var id = 'TEId=' + $(this).val();

                ddlPosteId.html('');
                ddlPosteId.append('<option>Select a Position</options>');

                $.getJSON({
                    url: $('#url-get-poste').data('url'),
                    data: id,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["PList"]) {

                                ddlPosteId.append('<option value = "' + data["PList"][item]["PayPosteId"] + '" >' + data["PList"][item]["Description"] + '</options>');

                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate ddlSectionId  on dropdown ddlFloorId change <<Worker Location>>
        $('.section-ouvrier#ddlFloorId').change(

            function () {

                var ddlSectionId = $('.section-ouvrier#ddlSectionId');

                $('.FloorId').val($(this).val());

                var id = 'FloorId=' + $(this).val();

                ddlSectionId.html('');
                ddlSectionId.append('<option>Sections</options>');


                $.getJSON({
                    url: $('#url-get-floor-section').data('url'),
                    data: id,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["sections"]) {

                                ddlSectionId.append('<option value = "' + data["sections"][item]["SectionId"] + '" >' + data["sections"][item]["Description"] + '</options>');

                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate ddlFloorId  on select Date Production change <<Worker Section>>
        $('#date-sto-form #DP').change(

            function (e) {

                $('#add-other-worker-btn').prop('disabled', true);

                e.preventDefault();

                var ddlFloorId = $('.section-travail-ouvrier#ddlFloorId');

                ddlFloorId.html('');
                ddlFloorId.append('<option>Floors</options>');

                var ddlSectionId = $('.section-travail-ouvrier#ddlSectionId');

                ddlSectionId.html('');
                ddlSectionId.append('<option>Section</options>');

                var ddlOperationId = $('#add-autre-ouvrier-modal #ddlOperationId');

                ddlOperationId.html('');
                ddlOperationId.append('<option>Operations</options>');

                var sectionDesc = $('.selected-section-desc');
                var nbreOuvrierSTO = $('.nbre-ouvrier-sto');

                sectionDesc.text('');
                nbreOuvrierSTO.text('');

                $('#section-ouvrier-to-add-dt tbody').html('<tr class="odd"><td valign="top" colspan="5" class="dataTables_empty">No data available in table</td></tr>');
                $('#section-travail-ouvrier-dt tbody').html('<tr class="odd"><td valign="top" colspan="5" class="dataTables_empty">No data available in table</td></tr>');

                $('.DP').val($(this).val());


                var url = $('#date-sto-form').attr('action');
                var data = $('#date-sto-form').serialize();

                $.getJSON({
                    url: url,
                    data: data,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["floors"]) {

                                ddlFloorId.append('<option value = "' + data["floors"][item]["FloorId"] + '" >' + data["floors"][item]["Description"] + '</options>');

                            }

                            if (data["floors"].length == 0) {
                                $('.text-info').text("There are no Planning for this Date!");
                                $('#validation-link').trigger("click");
                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate ddlSectionId  on dropdown ddlFloorId change <<Worker Section>>
        $('#floor-sto-form #ddlFloorId').change(

            function () {

                $('#add-other-worker-btn').prop('disabled', true);

                var ddlSectionId = $('.section-travail-ouvrier#ddlSectionId');

                ddlSectionId.html('');
                ddlSectionId.append('<option>Section</options>');

                var ddlOperationId = $('#add-autre-ouvrier-modal #ddlOperationId');

                ddlOperationId.html('');
                ddlOperationId.append('<option>Operations</options>');

                var sectionDesc = $('.selected-section-desc');
                var nbreOuvrierSTO = $('.nbre-ouvrier-sto');

                sectionDesc.text('');
                nbreOuvrierSTO.text('');

                $('.DP').val($('#floor-sto-form .DP').val());
                $('.FloorId').val($(this).val());

                $('#section-ouvrier-to-add-dt tbody').html('<tr class="odd"><td valign="top" colspan="5" class="dataTables_empty">No data available in table</td></tr>');
                $('#section-travail-ouvrier-dt tbody').html('<tr class="odd"><td valign="top" colspan="5" class="dataTables_empty">No data available in table</td></tr>');

                var url = $('#floor-sto-form').attr('action');
                var data = $('#floor-sto-form').serialize();

                $.getJSON({
                    url: url,
                    data: data,
                    success: function (data) {
                        if (data != null) {

                            for (var item in data["sections"]) {

                                ddlSectionId.append('<option value = "' + data["sections"][item]["SectionId"] + '" >' + data["sections"][item]["Description"] + '</options>');

                            }

                            if (data["sections"].length == 0) {
                                $('.text-info').text("There are no Planning for the Section of this Floor for this Date!");
                                $('#validation-link').trigger("click");
                            }

                        }

                    }

                });

            }

        );
        //End


        //Begin Populate Table Section Ouvrier  on dropdown ddlSectionId change <<Worker Section>>
        $('#section-sto-form #ddlSectionId').change(

            function () {
                fillSOSTO();
            }

        );
        //End


        //Form Submit Ajax

        $('#AddOtherWorker-STO-Form .submit').click(
            function (e) {

                e.preventDefault();

                $('#add-autre-ouvrier-modal .form-overlay-loading').fadeIn();

                var url = $('#AddOtherWorker-STO-Form').attr('action');
                var data = $('#AddOtherWorker-STO-Form').serialize();

                $.ajax({
                    traditional: true,
                    type: "POST",
                    url: url,
                    data: data,
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    success: function (data) {

                        $('#add-autre-ouvrier-modal .form-overlay-loading').fadeOut();

                        if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                            $('#add-autre-ouvrier-modal').modal('hide');
                            $('.text-info').text("Not Logged In!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["noPermission"] != null && data["noPermission"] == true) {
                            $('#add-autre-ouvrier-modal').modal('hide');
                            $('.text-info').text("Access Denied. No Permission!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                            $('#add-autre-ouvrier-modal').modal('hide');

                            fillSOSTO();

                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");


                        }
                        else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }
                        else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }


                    },
                    error: function (error) {
                        $('#add-autre-ouvrier-modal .form-overlay-loading').fadeOut();
                    }


                });

            }
        );



        $('#Add-Worker-to-Section-Form .submit').click(
            function (e) {

                e.preventDefault();

                $('#add-worker-to-sto-modal .form-overlay-loading').fadeIn();

                var url = $('#Add-Worker-to-Section-Form').attr('action');
                var data = $('#Add-Worker-to-Section-Form').serialize();

                $.ajax({
                    traditional: true,
                    type: "POST",
                    url: url,
                    data: data,
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    success: function (data) {

                        $('#add-worker-to-sto-modal .form-overlay-loading').fadeOut();

                        if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                            $('#add-worker-to-sto-modal').modal('hide');
                            $('.text-info').text("Not Logged In!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["noPermission"] != null && data["noPermission"] == true) {
                            $('#add-worker-to-sto-modal').modal('hide');
                            $('.text-info').text("Access Denied. No Permission!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                            $('#add-worker-to-sto-modal').modal('hide');

                            fillSOSTO();

                            if (data["notAll"] == true) {
                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");
                            }


                        }
                        else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {
                            $('#add-worker-to-sto-modal').modal('hide');
                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }


                    },
                    error: function (error) {
                        $('#add-worker-to-sto-modal .form-overlay-loading').fadeOut();
                    }


                });

            }
        );



        $('#Add-Production-Section-Form .submit').click(
            function (e) {

                e.preventDefault();

                $('#add-production-section-modal .form-overlay-loading').fadeIn();

                var url = $('#Add-Production-Section-Form').attr('action');
                var data = $('#Add-Production-Section-Form').serialize();

                $.ajax({
                    traditional: true,
                    type: "POST",
                    url: url,
                    data: data,
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    success: function (data) {

                        $('#add-production-section-modal .form-overlay-loading').fadeOut();

                        if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                            $('#add-production-section-modal').modal('hide');
                            $('.text-info').text("Not Logged In!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["noPermission"] != null && data["noPermission"] == true) {
                            $('#add-production-section-modal').modal('hide');
                            $('.text-info').text("Access Denied. No Permission!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                            $('#add-production-section-modal').modal('hide');
                            fillProductionSectionPV();
                                                       
                        }
                        else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {
                            
                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }
                        else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {
                            
                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }


                    },
                    error: function (error) {
                        $('#add-production-section-modal .form-overlay-loading').fadeOut();
                    }


                });

            }
        );


        $('#Delete-STO-Form .submit').click(
            function (e) {

                e.preventDefault();

                $('#delete-Modal .form-overlay-loading').fadeIn();

                var url = $('#Delete-STO-Form').attr('action');
                var data = $('#Delete-STO-Form').serialize();

                $.ajax({
                    traditional: true,
                    type: "POST",
                    url: url,
                    data: data,
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    success: function (data) {

                        $('#delete-Modal .form-overlay-loading').fadeOut();

                        if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                            $('#delete-Modal').modal('hide');
                            $('.text-info').text("Not Logged In!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["noPermission"] != null && data["noPermission"] == true) {
                            $('#delete-Modal').modal('hide');
                            $('.text-info').text("Access Denied. No Permission!");
                            $('#validation-link').trigger("click");
                        }
                        else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                            $('#delete-Modal').modal('hide');

                            fillSOSTO();

                            if (data["notAll"] == true) {
                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");
                            }


                        }
                        else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {
                            $('#delete-Modal').modal('hide');
                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }


                    },
                    error: function (error) {
                        $('#delete-Modal .form-overlay-loading').fadeOut();
                    }


                });

            }
        );


        //End Form Submit Ajax

        
    }

);


function fillSOSTO() {

    $('.pv-overlay-loading').fadeIn();

    $('.select-checkbox').removeClass('selected');

    var sotbody = $('#section-ouvrier-to-add-dt tbody');

    var stotbody = $('#section-travail-ouvrier-dt tbody');

    $('#add-other-worker-btn').prop('disabled', true);

    var sectionDesc = $('.selected-section-desc');
    var nbreOuvrierSTO = $('.nbre-ouvrier-sto');

    var ddlOperationId = $('#add-autre-ouvrier-modal #ddlOperationId');

    ddlOperationId.html('');
    ddlOperationId.append('<option>Operations</options>');

    sectionDesc.text('');
    nbreOuvrierSTO.text('');

    $('#section-ouvrier-to-add-dt').DataTable().clear();
    $('#section-travail-ouvrier-dt').DataTable().clear();

    $('.SectionId').val($('#section-sto-form #ddlSectionId').val());

    var url = $('#section-sto-form').attr('action') + '/?';
    var data = $('#section-sto-form').serialize();

    $.getJSON({
        url: url,
        data: data,
        success: function (data) {

            $('.pv-overlay-loading').fadeOut();

            $('#section-ouvrier-to-add-dt').DataTable().destroy();
            $('#section-travail-ouvrier-dt').DataTable().destroy();

            $('#section-ouvrier-to-add #datatableMSOnly').html('<thead class="bg-success"><tr><th class = "so"></th><th>Code</th><th>Full Name</th><th>Operation</th></tr ></thead><tbody></tbody>');
            $('#section-travail-ouvrier #datatableMSOnly').html('<thead class="bg-success"><tr><th></th><th>Code</th><th>Full Name</th><th>Operation</th></tr ></thead><tbody></tbody>');

            for (var item in data["operations"]) {

                ddlOperationId.append('<option value = "' + data["operations"][item]["OperationId"] + '" >' + data["operations"][item]["Description"] + '</options>');

            }

            if (data["noOperationSF"] == true) {
                $('.text-info').text("Operation not Avalaible for this Style Category!");
                $('#validation-link').trigger("click");
            }

            if (data["returnToLogin"] == true) {
                $('.text-info').text("Not Logged In!");
                $('#validation-link').trigger("click");
            }
            else if (data["noPermission"] == true) {
                $('.text-info').text("Access Denied...No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["module"] == true) {

                if (data["sectionValide"] == true) {
                    $('#add-other-worker-btn').prop('disabled', true);
                    $('.text-info').text("Production for this Section already Validated for this Date!");
                    $('#validation-link').trigger("click");
                }

                if (data["section"]["Description"] != null) {
                    sectionDesc.text('- ' + data["section"]["Description"]);
                }

                if (data["section"]["infoOperation"] != null) {
                    nbreOuvrierSTO.text(data["section"]["infoOperation"]);
                }

                if (data["sectionOuvriers"] != null && data["sectionOuvriers"].length > 0) {
                    sotbody.html('');
                    $('#add-other-worker-btn').prop('disabled', false);

                    if (data["sectionValide"] == true) {
                        $('#add-other-worker-btn').prop('disabled', true);
                        $('.select-checkbox').removeClass('.select-checkbox');
                    }

                }

                for (var item in data["sectionOuvriers"]) {
                    var tr = document.createElement('tr');
                    tr.setAttribute('id', data["sectionOuvriers"][item]["SectionOuvrierId"]);
                    tr.innerHTML = '<td></td>' +
                        '<td>' + data["sectionOuvriers"][item]["code"] + '</td>' +
                        '<td>' + data["sectionOuvriers"][item]["nomComplet"] + '</td>' +
                        '<td>' + data["sectionOuvriers"][item]["operation"] + '</td>';

                    sotbody.append(tr);
                }

                if (data["sectionTravailOuvriers"] != null && data["sectionTravailOuvriers"].length > 0) {
                    stotbody.html('');

                }

                for (var item in data["sectionTravailOuvriers"]) {
                    var tr = document.createElement('tr');
                    tr.setAttribute('id', data["sectionTravailOuvriers"][item]["SectionTravailOuvrierId"]);
                    tr.innerHTML = '<td></td >' +
                        '<td>' + data["sectionTravailOuvriers"][item]["code"] + '</td>' +
                        '<td>' + data["sectionTravailOuvriers"][item]["nomComplet"] + '</td>' +
                        '<td>' + data["sectionTravailOuvriers"][item]["operation"] + '</td>' +
                        '<td>' + data["sectionTravailOuvriers"][item]["autreOuvrier"] + '</td>';

                    stotbody.append(tr);
                }

            }
            else if (data["module"] == false) {

                if (data["sectionValide"] == true) {
                    $('#add-other-worker-btn').prop('disabled', true);
                    $('.text-info').text("Production for this Section already Validated for this Date!");
                    $('#validation-link').trigger("click");
                }

                if (data["section"]["Description"] != null) {
                    sectionDesc.text('-' + data["section"]["Description"]);
                }

                if (data["section"]["infoOperation"] != null) {
                    nbreOuvrierSTO.text(data["section"]["infoOperation"]);
                }

                if (data["sectionOuvriers"] != null && data["sectionOuvriers"].length > 0) {
                    sotbody.html('');
                    $('#add-other-worker-btn').prop('disabled', false);

                    if (data["sectionValide"] == true) {
                        $('#add-other-worker-btn').prop('disabled', true);
                    }

                }

                for (var item in data["sectionOuvriers"]) {
                    var tr = document.createElement('tr');
                    tr.setAttribute('id', data["sectionOuvriers"][item]["SectionOuvrierId"]);
                    tr.innerHTML = '<td></td >' +
                        '<td>' + data["sectionOuvriers"][item]["code"] + '</td>' +
                        '<td>' + data["sectionOuvriers"][item]["nomComplet"] + '</td>' +
                        '<td>' + data["sectionOuvriers"][item]["operation"] + '</td>';

                    sotbody.append(tr);
                }


                if (data["sectionTravailOuvriers"] != null && data["sectionTravailOuvriers"].length > 0) {
                    stotbody.html('');
                }

                for (var item in data["sectionTravailOuvriers"]) {
                    var tr = document.createElement('tr');
                    tr.setAttribute('id', data["sectionTravailOuvriers"][item]["SectionTravailOuvrierId"]);
                    tr.innerHTML = '<td></td >' +
                        '<td>' + data["sectionTravailOuvriers"][item]["code"] + '</td>' +
                        '<td>' + data["sectionTravailOuvriers"][item]["nomComplet"] + '</td>' +
                        '<td>' + data["sectionTravailOuvriers"][item]["operation"] + '</td>' +
                        '<td>' + data["sectionTravailOuvriers"][item]["autreOuvrier"] + '</td>';

                    stotbody.append(tr);
                }

            }

            InitializeSectionOuvrierToAddDT();

            InitializeSectionTravailOuvrierDT();

        },
        error: function (error) {
            $('.pv-overlay-loading').fadeOut();
        }

    });

}


function InitializeSectionOuvrierToAddDT() {


    var sectionOuvrierToAddDT = $('#section-ouvrier-to-add-dt').DataTable(
        {
            columnDefs: [{
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            }],
            select: {
                style: 'multi',
                selector: 'td:first-child'
            },
            destroy: true,
            "aaSorting": [],
            buttons: [],
            info: false,
            lengthChange: false,
            searching: false,
            paging: false
        }

    );

    
    sectionOuvrierToAddDT.on("click", "th.so.select-checkbox", function () {

        if ($('th.so.select-checkbox').hasClass('selected')) {
            sectionOuvrierToAddDT.rows().deselect();
            $('th.so.select-checkbox').removeClass('selected');
        }
        else {
            sectionOuvrierToAddDT.rows().select();
            $('th.so.select-checkbox').addClass('selected');
        }

    }).on("select deselect", function () {

        if (sectionOuvrierToAddDT.rows({ selected: true }).count() != sectionOuvrierToAddDT.rows().count()) {
            $('th.so.select-checkbox').removeClass('selected');
        }
        else {
            $('th.so.select-checkbox').addClass('selected');
        }

    });

    $('.add-worker-to-sto').attr('disabled', true);

    sectionOuvrierToAddDT.on("select", function () {

        if (sectionOuvrierToAddDT.rows({ selected: true }).count() == sectionOuvrierToAddDT.rows().count()) {
            $('#section-ouvrier-to-add-dt.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#section-ouvrier-to-add-dt.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = sectionOuvrierToAddDT.rows({ selected: true }).nodes().toArray();



        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#soIds').val(rowId.toString());

        if (rowId.length > 0) {
            $('.add-worker-to-sto').attr('disabled', false);

        }
        else {
            $('.add-worker-to-sto').attr('disabled', true);

        }

    });

    sectionOuvrierToAddDT.on("deselect", function () {

        if (sectionOuvrierToAddDT.rows({ selected: true }).count() == sectionOuvrierToAddDT.rows().count()) {
            $('#section-ouvrier-to-add-dt.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#section-ouvrier-to-add-dt.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = sectionOuvrierToAddDT.rows({ selected: true }).nodes().toArray();



        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#soIds').val(rowId.toString());

        if (rowId.length > 0) {
            $('.add-worker-to-sto').attr('disabled', false);

        }
        else {
            $('.add-worker-to-sto').attr('disabled', true);

        }

    });
}



function InitializeSectionTravailOuvrierDT() {


    var sectionTravailOuvrierdDT = $('#section-travail-ouvrier-dt').DataTable(
        {
            columnDefs: [{
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            }],
            select: {
                style: 'multi',
                selector: 'td:first-child'
            },
            destroy: true,
            "aaSorting": [],
            buttons: [],
            info: false,
            lengthChange: false,
            searching: false,
            paging: false
        }

    );

    sectionTravailOuvrierdDT.on("click", "th.select-checkbox", function () {

        if ($('th.select-checkbox').hasClass('selected')) {
            sectionTravailOuvrierdDT.rows().deselect();
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').removeClass('selected');
        }
        else {
            sectionTravailOuvrierdDT.rows().select();
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').addClass('selected');
        }

    }).on("select deselect", function () {

        if (sectionTravailOuvrierdDT.rows({ selected: true }).count() != sectionTravailOuvrierdDT.rows().count()) {
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').removeClass('selected');
        }
        else {
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').addClass('selected');
        }

    });

    $('.delete-link-sto').attr('disabled', true);

    sectionTravailOuvrierdDT.on("select", function () {

        if (sectionTravailOuvrierdDT.rows({ selected: true }).count() == sectionTravailOuvrierdDT.rows().count()) {
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = sectionTravailOuvrierdDT.rows({ selected: true }).nodes().toArray();



        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#stoIds').val(rowId.toString());

        if (rowId.length > 0) {
            $('.delete-link-sto').attr('disabled', false);

        }
        else {
            $('.delete-link-sto').attr('disabled', true);

        }

    });

    sectionTravailOuvrierdDT.on("deselect", function () {

        if (sectionTravailOuvrierdDT.rows({ selected: true }).count() == sectionTravailOuvrierdDT.rows().count()) {
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#section-travail-ouvrier-dt.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = sectionTravailOuvrierdDT.rows({ selected: true }).nodes().toArray();
               
        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#stoIds').val(rowId.toString());

        if (rowId.length > 0) {
            $('.delete-link-sto').attr('disabled', false);

        }
        else {
            $('.delete-link-sto').attr('disabled', true);

        }

    });
}