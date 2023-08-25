$(document).ready(function () {

    $('.ly-overlay-loading').fadeOut();

    var audioElement = document.createElement('audio');
    audioElement.setAttribute('src', $('#notif-sound-1').data('src'));


    if ($('.home-body').length) {

        $('.body-content').addClass('bg-image');

    } else {

        $('.body-content').removeClass('bg-image');

    }

    // Dropdown List Searchable Begin
    $(document).on("keyup", ".combobox-auto-select .input-search", function (e) {

        var ddl = $(this).siblings('.ddl-to-filter');

        var options = $('#' + ddl.attr('id') + ' option').toArray();

        for (var i = 0; i < options.length; i++) {

            if (options[i].innerHTML.toLowerCase().startsWith($(this).val().toLowerCase())) {

                var start = $(this).val().length;

                ddl.val(options[i]['value']).trigger('change');

                var textTyped = options[i].innerHTML.substring(0, start);

                $(this).val(textTyped);

                $(this).css('color', '#fd7979');

                break;

            }
            else {

                ddl.find('option:eq(0)').removeAttr('value')

                ddl.val('');

                $('.clearLotInfo').text('');
                $('.LotId').val(0);

                $(this).css('color', 'black');
            }

        }


    });

    $(document).on("focusin", ".combobox-auto-select .input-search", function () {

        $(this).val('');

    });

    $(document).on("focusout", ".combobox-auto-select .input-search", function () {

        $(this).val('');

        if ($(this).siblings('.ddl-to-filter').val() == null) {

            $(this).siblings('.ddl-to-filter').find('option:eq(0)').prop('selected', true);
        }

    });

    $(document).on("change", ".combobox-auto-select .ddl-to-filter", function () {
        $(this).siblings('.input-search').val('');
    });
    // Dropdown List Searchable End

    //End


    $(document).on("change", "#Add-Role-Permission-Form #ddlAppNavigationId", function () {

        var ddlPermissionId = $('#Add-Role-Permission-Form #ddlPermissionId');

        ddlPermissionId.html('');

        var url = $('#url-get-role-permission-id').data('url');

        var data = { AppNavigationId: $(this).val() };

        $.get({
            url: url,
            data: data,
            success: function (data) {

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }

                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data != null) {

                    for (var item in data["permissions"]) {

                        ddlPermissionId.append('<option value = ' + data["permissions"][item]["PermissionId"] + '>' + data["permissions"][item]["Label"] + '</option>');
                        $('#ddlPermissionId option:contains("Select Permissions")').attr("disabled", "disabled");

                    }

                    $('.multiselect').multiselect('destroy');
                    initializeMultiSelect();

                }

            },
            error: function (error) {
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );





    //Begin Side Menu Sub Menu Click Evt

    $('.main-menu-menu').on("click", ".nav-sub li a", function (e) {

        e.preventDefault();

        var url = $(this).attr('href');

        $('.bc-overlay-loading').fadeIn();

        $('#btn-main-menu').trigger('click');

        $.get({
            url: url,
            success: function (data) {

                $('.bc-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {

                    $('.body-content').html(data);

                    $('#Add-Client-Sabotay-Form .info-sabotay').hide();
                    $('#Edit-Client-Sabotay-Form .info-sabotay-edit').hide();
                    tableInitializer();
                    TableMSInitialiser();
                    initializeMultiSelect();
                    initializeMultiSelectSA();


                    var title = $('title');
                    title.text($('.title').data('title') + " | " + title.text().split(' | ')[1]);

                    if ($('.home-body').length) {

                        $('.body-content').addClass('bg-image');

                    } else {

                        $('.body-content').removeClass('bg-image');

                    }

                }

            },
            error: function (error) {
                $('.bc-overlay-loading').fadeOut();

                console.log(error.responseText);

                if (error.status == 404) {
                    $('.text-info').text("Page Not Found!");
                    $('#validation-link').trigger("click");
                }
                else {
                    $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                    $('#validation-link').trigger("click");
                }

            }
        });

    }
    );


    //End Side Menu Sub Menu Click Evt

    //Begin Event Listener Delegation

    $(document).on("click", ".sub-view", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        var data = {};

        $('.bc-overlay-loading').fadeIn();

        $.get({
            url: url,
            data: data,
            success: function (data) {

                $('.bc-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {

                    $('.body-content').html(data);

                    tableInitializer();
                    TableMSInitialiser();
                    initializeMultiSelect();
                    initializeMultiSelectSA();

                    $('.bc-overlay-loading').fadeOut();

                    var title = $('title');
                    title.text($('.title').data('title') + " | " + title.text().split(' | ')[1]);

                    $('.multiselect').multiselect('destroy');
                    initializeMultiSelect();

                    $('.multiselect-sa').multiselect('destroy');
                    initializeMultiSelectSA();

                    if ($('.home-body').length) {

                        $('.body-content').addClass('bg-image');


                    } else {

                        $('.body-content').removeClass('bg-image');

                    }

                }

            },
            error: function (error) {

                $('.bc-overlay-loading').fadeOut();

                if (error.status == 404) {
                    $('.text-info').text("Page Not Found!");
                    $('#validation-link').trigger("click");
                }
                else {
                    $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                    $('#validation-link').trigger("click");
                }

            }
        });

    }
    );


    $(document).on("click", ".sub-view-back-to-parent-link", function (e) {

        e.preventDefault();

        var url = $(this).attr('href');

        var clientTypeId = $(this).data('client-type-id');
        var fraisCompagnieTypeId = $(this).data('frais-compagnie-type-id');

        var mobileUserTypeId = $(this).data('mobile-user-type-id');

        $('.bc-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {

                $('.bc-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {

                    $('.body-content').html(data);

                    tableInitializer();
                    TableMSInitialiser();
                    tableReportInitializer();

                    var title = $('title');
                    title.text($('.title').data('title') + " | " + title.text().split(' | ')[1]);


                    if ($('.home-body').length) {

                        $('.body-content').addClass('bg-image');


                    } else {

                        $('.body-content').removeClass('bg-image');

                    }

                }

            },
            error: function (error) {
                $('.bc-overlay-loading').fadeOut();

                if (error.status == 404) {
                    $('.text-info').text("Page Not Found!");
                    $('#validation-link').trigger("click");
                }
                else {
                    $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                    $('#validation-link').trigger("click");
                }

            }
        });
    }
    );



    $(document).on("focusin", "#Password", function () {

        $('.password-strong-tooltip').show()
    }
    );


    $(document).on("focusout", "#Password", function () {

        $('.password-strong-tooltip').hide();
    }
    );


    $(document).on("keyup", "#Password", function () {

        if (new RegExp("^.*[0-9]").test($(this).val())) {
            $('.password-strong-tooltip #digit-criteria span').addClass('password-strong-tooltip-item-bg-success');
        }
        else {
            $('.password-strong-tooltip #digit-criteria span').removeClass('password-strong-tooltip-item-bg-success');
        }

        if (new RegExp(".*[A-Z]").test($(this).val())) {
            $('.password-strong-tooltip #uc-letter-criteria span').addClass('password-strong-tooltip-item-bg-success');
        }
        else {
            $('.password-strong-tooltip #uc-letter-criteria span').removeClass('password-strong-tooltip-item-bg-success');
        }

        if (new RegExp(".*[a-z]").test($(this).val())) {
            $('.password-strong-tooltip #lc-letter-criteria span').addClass('password-strong-tooltip-item-bg-success');
        }
        else {
            $('.password-strong-tooltip #lc-letter-criteria span').removeClass('password-strong-tooltip-item-bg-success');
        }

        if (new RegExp(".*[.!@$%]").test($(this).val())) {
            $('.password-strong-tooltip #schar-criteria span').addClass('password-strong-tooltip-item-bg-success');
        }
        else {
            $('.password-strong-tooltip #schar-criteria span').removeClass('password-strong-tooltip-item-bg-success');
        }

        if ($(this).val().length >= 8) {
            $('.password-strong-tooltip #minchar-criteria span').addClass('password-strong-tooltip-item-bg-success');
        }
        else {
            $('.password-strong-tooltip #minchar-criteria span').removeClass('password-strong-tooltip-item-bg-success');
        }

        if ($(this).val().length >= 8 && $(this).val().length <= 32) {
            $('.password-strong-tooltip #maxchar-criteria span').addClass('password-strong-tooltip-item-bg-success');
        }
        else {
            $('.password-strong-tooltip #maxchar-criteria span').removeClass('password-strong-tooltip-item-bg-success');
        }

    }
    );


    $(document).on('click', '.lock-link-usr', function () { $('.lock-confirmed').attr('href', '/Users/LockToggle/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-usr', function () { $('.delete-confirmed').attr('href', '/Users/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-mobile-user', function () { $('.delete-confirmed').attr('href', '/AutreUsers/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-mobile-user-voyage', function () { $('.delete-confirmed').attr('href', '/MobileUsers/Delete/' + $(this).attr('id')); });

    $(document).on('click', '.delete-link-rol', function () { $('.delete-confirmed').attr('href', '/Roles/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-permission', function () { $('.delete-confirmed').attr('href', '/Permissions/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-permission-application', function () { $('.delete-confirmed').attr('href', '/PermissionApplications/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-app-navigation', function () { $('.delete-confirmed').attr('href', '/AppNavigations/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-app-navigation-permission', function () { $('.delete-confirmed').attr('href', '/AppNavigationPermissions/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-app-navigation-application', function () { $('.delete-confirmed').attr('href', '/AppNavigationApplications/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-rop', function () { $('.delete-confirmed').attr('href', '/RolePermissions/Delete/' + $(this).attr('id')); });
    $(document).on('click', '.delete-link-application', function () { $('.delete-confirmed').attr('href', '/Applications/Delete/' + $(this).attr('id')); });



    $(document).on("click", ".lock-confirmed",
        function (e) {

            e.preventDefault();

            var url = $(this).attr('href');

            $.ajax({
                traditional: true,
                type: "GET",
                url: url,
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                success: function (data) {

                    if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                        $('#lock-Modal').modal('hide');
                        $('#unlock-Modal').modal('hide');

                        $('.text-info').text("You are not Logged In!");
                        $('#validation-link').trigger("click");

                        setTimeout(function () {

                            window.location.reload();

                        },
                            2000
                        );

                    }
                    else if (data["newSession"] != null && data["newSession"] == true) {

                        $('.text-info').text(data["message1"]);
                        $('#validation-link').trigger("click");

                        setTimeout(function () {

                            window.location.reload();

                        },
                            2000
                        );

                    }
                    else if (data["noPermission"] != null && data["noPermission"] == true) {

                        $('#lock-Modal').modal('hide');
                        $('#unlock-Modal').modal('hide');

                        $('.text-info').text("Access Denied. No Permission!");
                        $('#validation-link').trigger("click");
                    }
                    else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                        $('#lock-Modal').modal('hide');
                        $('#unlock-Modal').modal('hide');

                        setTimeout(function () {

                            fillUserListPV();

                        },
                            1500
                        );

                        $('.text-info').text(data["message"]);
                        $('#validation-link').trigger("click");

                    }
                    else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                        $('#lock-Modal').modal('hide');
                        $('#unlock-Modal').modal('hide');

                        $('.text-info').text(data["message"]);
                        $('#validation-link').trigger("click");
                    }
                    else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                        $('#lock-Modal').modal('hide');
                        $('#unlock-Modal').modal('hide');

                        $('.text-info').text(data["message"]);
                        $('#validation-link').trigger("click");
                    }


                },
                error: function (error) {

                    $('#lock-Modal').modal('hide');
                    $('#unlock-Modal').modal('hide');

                    $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                    $('#validation-link').trigger("click");
                }


            });

        }
    );


    $(document).on("click", ".delete-confirmed",
        function (e) {

            e.preventDefault();

            var url = $(this).attr('href');

            $.ajax({
                traditional: true,
                async: true,
                type: "GET",
                url: url,
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                success: function (data) {

                    if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                        $('#delete-modal').modal('hide');

                        $('.text-info').text("You are not Logged In!");
                        $('#validation-link').trigger("click");

                        setTimeout(function () {

                            window.location.reload();

                        },
                            2000
                        );

                    }
                    else if (data["newSession"] != null && data["newSession"] == true) {

                        $('.text-info').text(data["message1"]);
                        $('#validation-link').trigger("click");

                        setTimeout(function () {

                            window.location.reload();

                        },
                            2000
                        );

                    }
                    else if (data["noPermission"] != null && data["noPermission"] == true) {

                        $('#delete-modal').modal('hide');

                        $('.text-info').text("Access Denied. No Permission!");
                        $('#validation-link').trigger("click");
                    }
                    else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                        $('#delete-modal').modal('hide');
                        $('.modal-backdrop').hide();

                        if (data["ctrlName"] != null) {

                            if (data["ctrlName"] == "Users") {

                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");

                                setTimeout(function () {

                                    if ($('#user-list-pv').length) {
                                        fillUserListPV();
                                    }

                                    if ($('#user-application-list-pv').length) {
                                        fillUserApplicationListPV();
                                    }

                                    if ($('#user-application-role-list-pv').length) {
                                        fillUserApplicationRoleListPV();
                                    }


                                }, 1500);

                            }
                            else if (data["ctrlName"] == "Roles") {

                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");

                                setTimeout(function () {

                                    if ($('#url-get-role-list').length) {

                                        fillRoleListPV();
                                    }
                                    else if ($('#url-get-role-permission-list').length) {
                                        fillRolePermissionListPV();
                                    }


                                }, 1500);


                            }
                            else if (data["ctrlName"] == "Applications") {

                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");

                                setTimeout(function () {

                                    if ($('#url-get-application-list').length) {

                                        fillApplicationListPV();
                                    }


                                }, 1500);

                            }
                            else if (data["ctrlName"] == "Permissions") {

                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");

                                setTimeout(function () {

                                    if ($('#permission-list-pv').length) {
                                        fillPermissionListPV();
                                    }
                                    else if ($('#permission-application-list-pv').length) {
                                        fillPermissionApplicationListPV();
                                    }

                                }, 1500);

                            }
                            else if (data["ctrlName"] == "MobileUsers") {

                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");

                                setTimeout(function () {

                                    if ($('#mobile-user-list-pv').length) {
                                        fillMobileUserListPV();
                                    }
                                    else if ($('#mobile-user-voyage-list-pv').length) {
                                        fillMobileUserVoyageListPV();
                                    }

                                }, 1500);

                            }
                            else if (data["ctrlName"] == "AppNavigations") {

                                $('.text-info').text(data["message"]);
                                $('#validation-link').trigger("click");

                                setTimeout(function () {

                                    if ($('#app-navigation-list-pv').length) {
                                        fillAppNavigationListPV();
                                    }
                                    else if ($('#app-navigation-permission-list-pv').length) {
                                        fillAppNavigationPermissionListPV();
                                    }
                                    else if ($('#app-navigation-application-list-pv').length) {
                                        fillAppNavigationApplicationListPV();
                                    }


                                }, 1500);

                            }



                        }
                        else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                            $('#delete-modal').modal('hide');

                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }
                        else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

                            $('#delete-modal').modal('hide');

                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }
                        else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                            $('#delete-modal').modal('hide');

                            $('.text-info').text(data["message"]);
                            $('#validation-link').trigger("click");
                        }


                    }
                    else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                        $('#delete-modal').modal('hide');

                        $('.text-info').text(data["message"]);
                        $('#validation-link').trigger("click");
                    }
                    else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

                        $('#delete-modal').modal('hide');

                        $('.text-info').text(data["message"]);
                        $('#validation-link').trigger("click");
                    }
                    else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                        $('#delete-modal').modal('hide');

                        $('.text-info').text(data["message"]);
                        $('#validation-link').trigger("click");

                    }


                },
                error: function (error) {

                    $('#delete-modal').modal('hide');

                    $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                    $('#validation-link').trigger("click");
                }


            });

        }
    );



    $(document).on("change", "#LogoCompagnie", function () {

        var file = this.files[0],
            reader = new FileReader();

        if ((file.size / 1024) <= 50) {

            reader.onloadend = function () {

                var img = new Image();

                img.height = 200;
                img.width = 200;

                img.src = reader.result;

                $('#logo-preview').html(img);

            };

            reader.readAsDataURL(file);

        }
        else {

            $(this).val('');

            $('.text-info').text('Image size should not exceed 50KB (About 300x300px)');
            $('#validation-link').trigger("click");
        }

    }
    );


    $(document).on("change", "#ProfilePhoto", function () {

        var file = this.files[0],
            reader = new FileReader();

        if ((file.size / 1024) <= 1024) {

            reader.onloadend = function () {

                var img = new Image();

                img.height = 100;
                img.width = 100;

                img.src = reader.result;

                $('#profile-photo-preview').html(img);

            };

            reader.readAsDataURL(file);

        }
        else {

            $(this).val('');

            $('.text-info').text('Image size should not exceed 1MB (About 300x300px)');
            $('#validation-link').trigger("click");
        }


    }
    );




    $(document).on("change", "#SignatureResponsableApplication", function () {

        var file = this.files[0],
            reader = new FileReader();

        if ((file.size / 1024) <= 1024) {

            reader.onloadend = function () {

                var img = new Image();

                img.height = 80;
                img.width = 80;

                img.src = reader.result;

                $('#SignatureResponsableApplication-photo-preview').html(img);


            };

            reader.readAsDataURL(file);

        }
        else {

            $(this).val('');

            $('.text-info').text('Image size should not exceed 1MB (About 300x300px)');
            $('#validation-link').trigger("click");
        }


    }
    );


    $(document).on("change", "#LogoApplication", function () {

        var file = this.files[0],
            reader = new FileReader();

        if ((file.size / 1024) <= 1024) {

            reader.onloadend = function () {

                var img = new Image();

                img.height = 80;
                img.width = 80;

                img.src = reader.result;

                $('#LogoApplication-photo-preview').html(img);


            };

            reader.readAsDataURL(file);

        }
        else {

            $(this).val('');

            $('.text-info').text('Image size should not exceed 1MB (About 300x300px)');
            $('#validation-link').trigger("click");
        }


    }
    );



    $(document).on("change", "#PhotoClient", function () {

        var file = this.files[0],
            reader = new FileReader();
        /*(file.size / 1024) <= 1024)*/
        if ((file.size / 1024) <= 2560) {

            reader.onloadend = function () {

                var img = new Image();

                img.height = 40;
                img.width = 40;

                img.src = reader.result;

                $('#Client-photo-preview').html(img);
                //$('#New-Client-photo-preview').html(img);

            };

            reader.readAsDataURL(file);

        }
        else {

            $(this).val('');

            $('.text-info').text('Image trop Grande. Diminuer la taille, puis Réessayer egale a 5MB (300x300px)');
            $('#validation-link').trigger("click");
        }


    }
    );


    $(document).on("change", "#New-PhotoClient", function () {

        var file3 = this.files[0],
            reader3 = new FileReader();

        if ((file3.size / 1024) <= 2560) {

            reader3.onloadend = function () {

                var img3 = new Image();

                img3.height = 80;
                img3.width = 80;

                img3.src = reader3.result;

                $('#NewClient-photo-preview').html(img3);

            };

            reader3.readAsDataURL(file3);

        }
        else {

            $(this).val('');

            $('.text-info').text('Image trop Grande. Diminuer la taille, puis Réessayer (300x300px)');
            $('#validation-link').trigger("click");
        }


    }
    );



    $(document).on("change", "#SignatureClient", function () {


        var fileSignatureClient = this.files[0],
            readerSignatureClient = new FileReader();

        if ((fileSignatureClient.size / 1024) <= 2560) {

            readerSignatureClient.onloadend = function () {

                var imgSignatureClient = new Image();

                imgSignatureClient.height = 40;
                imgSignatureClient.width = 40;

                imgSignatureClient.src = readerSignatureClient.result;

                $('#client-signature-preview').html(imgSignatureClient);


            };

            readerSignatureClient.readAsDataURL(fileSignatureClient);


        }
        else {

            $(this).val('');

            $('.text-info').text('Image trop Grande. Diminuer la taille, puis Réessayer (300x300px)');
            $('#validation-link').trigger("click");
        }


    }
    );


    $(document).on("change", "#New-SignatureClient", function () {

        var file2 = this.files[0],
            reader2 = new FileReader();


        if ((file2.size / 1024) <= 2560) {

            reader2.onloadend = function () {

                var img2 = new Image();

                img2.height = 80;
                img2.width = 80;

                img2.src = reader2.result;

                $('#New-Client-signature-preview').html(img2);


            };

            reader2.readAsDataURL(file2);

        }
        else {

            $(this).val('');

            $('.text-info').text('Image trop Grande. Diminuer la taille, puis Réessayer (300x300px)');
            $('#validation-link').trigger("click");
        }


    }
    );


    // End Image File EventListener


    //Auto Submit Form

    $(document).on("change", "#activation-client-form #ddlActivationId", function () {

        $(".ActivationId").val($(this).val());

        if ($(this).val() != "") {
            fillClientListPV();


        }
    });




    //End Auto Submit Form


    // Change Status Table Object EventListener

    $(document).on("click", ".change-statut-agence-btn", function (e) {

        e.preventDefault();

        var url = $(this).attr('href');
        $.ajax({
            traditional: true,
            type: "GET",
            url: url,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    fillAgenceListPV();

                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }


            },
            error: function (error) {
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    //End Change Status Table Object EventListener



    // Add Table Object EventListener


    $(document).on("click", "#Create-User-Form .submit", function (e) {

        e.preventDefault();

        $('#create-user-modal .form-overlay-loading').fadeIn();

        var url = $('#Create-User-Form').attr('action');
        var data = $('#Create-User-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-user-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-user-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-user-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-user-modal').modal('hide');

                    setTimeout(function () {

                        fillUserListPV();

                    },
                        1500
                    );


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

                $('#create-user-modal').modal('hide');

                if (!window.navigator.onLine) {
                    $('.text-info').text('You are Offline. Please Check your Internet Connection!');
                    $('#validation-link').trigger("click");
                }
                else {
                    $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                    $('#validation-link').trigger("click");
                }
            }


        });

    }
    );


    $(document).on("click", "#Add-Role-Form .submit", function (e) {

        e.preventDefault();

        $('#create-role-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-Role-Form').attr('action');
        var data = $('#Add-Role-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-role-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-role-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-role-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-role-modal').modal('hide');

                    setTimeout(function () {

                        fillRoleListPV();

                    },
                        2000
                    );

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

                    $('#create-role-modal').modal('hide');
                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }


            },
            error: function (error) {
                currentBtn.removeClass('button-processing');
                $('#create-role-modal').modal('hide');

                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");

            }


        });

    }
    );



    $(document).on("click", "#Add-Application-Form .submit", function (e) {

        e.preventDefault();

        $('#create-application-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-Application-Form').attr('action');
        var data = $('#Add-Application-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-application-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-application-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-application-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-application-modal').modal('hide');

                    setTimeout(function () {

                        fillApplicationListPV();

                    },
                        2000
                    );

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

                    $('#create-application-modal').modal('hide');
                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }


            },
            error: function (error) {
                currentBtn.removeClass('button-processing');
                $('#create-application-modal').modal('hide');

                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");

            }


        });

    }
    );


    $(document).on("click", "#Add-Role-Permission-Form .submit", function (e) {

        e.preventDefault();

        $('#create-role-permission-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-Role-Permission-Form').attr('action');
        var data = $('#Add-Role-Permission-Form').serialize() + '&PermissionIds=' + $('#Add-Role-Permission-Form #ddlPermissionId').val();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            async: true,
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-role-permission-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-role-permission-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-role-permission-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-role-permission-modal').modal('hide');

                    setTimeout(function () {

                        fillRolePermissionListPV();

                    },
                        1500
                    );

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
                currentBtn.removeClass('button-processing');
                $('#create-role-permission-modal').modal('hide');

                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");

            }


        });

    }
    );




    $(document).on("click", "#Add-Permission-Form .submit", function (e) {

        e.preventDefault();

        $('#create-permission-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-Permission-Form').attr('action');
        var data = $('#Add-Permission-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            async: true,
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-role-permission-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-permission-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-permission-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-permission-modal').modal('hide');


                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        fillPermissionListPV();

                    },
                        1500
                    );

                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

                    $('#create-permission-modal').modal('hide');
                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }


            },
            error: function (error) {
                currentBtn.removeClass('button-processing');
                $('#create-permission-modal').modal('hide');

                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");

            }


        });

    }
    );



    $(document).on("click", "#Add-App-Navigation-Form .submit", function (e) {

        e.preventDefault();

        $('#create-app-navigation-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-App-Navigation-Form').attr('action');
        var data = $('#Add-App-Navigation-Form').serialize();

        var currentBtn = $(this);

        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-app-navigation-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-app-navigation-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-app-navigation-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-app-navigation-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        fillAppNavigationListPV();

                    },
                        1500
                    );


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
                currentBtn.removeClass('button-processing');
                $('#create-app-navigation-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", "#Add-App-Navigation-Permission-Form .submit", function (e) {

        e.preventDefault();

        $('#create-app-navigation-permission-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-App-Navigation-Permission-Form').attr('action');
        var data = $('#Add-App-Navigation-Permission-Form').serialize() + '&PermissionIds=' + $('#Add-App-Navigation-Permission-Form #ddlPermissionId').val();

        var currentBtn = $(this);

        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-app-navigation-permission-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-app-navigation-permission-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-app-navigation-permission-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-app-navigation-permission-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        fillAppNavigationPermissionListPV();

                    },
                        1500
                    );


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
                currentBtn.removeClass('button-processing');
                $('#create-app-navigation-permission-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", "#Add-App-Navigation-Application-Form .submit", function (e) {

        e.preventDefault();

        $('#create-app-navigation-application-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-App-Navigation-Application-Form').attr('action');
        var data = $('#Add-App-Navigation-Application-Form').serialize() + '&ApplicationIds=' + $('#Add-App-Navigation-Application-Form #ddlApplicationId').val();

        var currentBtn = $(this);

        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-app-navigation-application-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-app-navigation-application-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-app-navigation-application-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-app-navigation-application-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        fillAppNavigationApplicationListPV();

                    },
                        1500
                    );


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
                currentBtn.removeClass('button-processing');
                $('#create-app-navigation-application-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", "#Add-Logo-Form .submit", function (e) {

        e.preventDefault();

        $('#add-compagnie-logo-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-Logo-Form').attr('action');

        var form = $('#Add-Logo-Form');

        var formData = new FormData(form.get(0));
        formData.append('LogoCompagnie', $('#Add-Logo-Form input[type="file"]')[0].files[0]);

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {

                currentBtn.removeClass('button-processing');
                $('#add-compagnie-logo-modal .form-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#add-compagnie-logo-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );


                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#add-compagnie-logo-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#add-compagnie-logo-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        if ($('#url-get-compagnie').length) {
                            fillCompagniePV();
                        }

                    },
                        2000
                    );




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

                currentBtn.removeClass('button-processing');

                $('#add-compagnie-logo-modal .form-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );







    $(document).on("click", "#Add-Profile-Photo-Form .submit", function (e) {

        e.preventDefault();

        $('#add-profil-photo-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-Profile-Photo-Form').attr('action');

        var form = $('#Add-Profile-Photo-Form');

        var formData = new FormData(form.get(0));
        formData.append('ProfilPhoto', $('#Add-Profile-Photo-Form input[type="file"]')[0].files[0]);

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {

                currentBtn.removeClass('button-processing');
                $('#add-profil-photo-modal .form-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#add-profil-photo-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );


                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#add-profil-photo-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#add-profil-photo-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        //$('.current-user-info-btn').html($('#profilPictureAfterEdit').html());
                        //if ($('#url-get-compagnie').length) {
                        //    fillCompagniePV();
                        //}

                    },
                        2000
                    );




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

                currentBtn.removeClass('button-processing');

                $('#add-profil-photo-modal .form-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );





    // Edit Table Object EventListener

    $(document).on("click", "#Edit-User-Form .submit", function (e) {

        e.preventDefault();

        $('#edit-user-modal .form-overlay-loading').fadeIn();

        var url = $('#Edit-User-Form').attr('action');
        var data = $('#Edit-User-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#edit-user-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#edit-user-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );


                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#edit-user-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {


                    $('#edit-user-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        fillUserListPV();

                    },
                        2000
                    );

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
                currentBtn.removeClass('button-processing');
                $('#edit-user-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );




    $(document).on("click", "#Edit-Role-Form .submit", function (e) {

        e.preventDefault();

        $('#edit-role-modal .form-overlay-loading').fadeIn();

        var url = $('#Edit-Role-Form').attr('action');
        var data = $('#Edit-Role-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#edit-role-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#edit-role-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#edit-role-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#edit-role-modal').modal('hide');

                    setTimeout(function () {

                        fillRoleListPV();

                    },
                        1500
                    );

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
                currentBtn.removeClass('button-processing');
                $('#edit-role-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );



    $(document).on("click", "#Edit-Application-Form .submit", function (e) {

        e.preventDefault();

        var fileName = $('#fileName').text();

        $('#edit-application-modal .form-overlay-loading').fadeIn();

        var url = $('#Edit-Application-Form').attr('action');
        var form = $('#Edit-Application-Form');

        var formData = new FormData(form.get(0));
        formData.append('LogoApplication', $('#Edit-Application-Form input[type="file"]')[0].files[0]);

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {

                $('#edit-application-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#edit-application-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#edit-application-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#edit-application-modal').modal('hide');

                    setTimeout(function () {

                        fillApplicationListPV();

                    },
                        1500
                    );

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

                currentBtn.removeClass('button-processing');
                $('#edit-application-modal .form-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );




    //$(document).on("click", "#Edit-Application-Form .submit", function (e) {

    //    e.preventDefault();

    //    $('#edit-application-modal .form-overlay-loading').fadeIn();

    //    var url = $('#Edit-Application-Form').attr('action');
    //    var data = $('#Edit-Application-Form').serialize();

    //    var currentBtn = $(this);
    //    currentBtn.addClass('button-processing');

    //    $.ajax({
    //        traditional: true,
    //        type: "POST",
    //        url: url,
    //        data: data,
    //        contentType: "application/x-www-form-urlencoded; charset=utf-8",
    //        success: function (data) {

    //            $('#edit-application-modal .form-overlay-loading').fadeOut();
    //            currentBtn.removeClass('button-processing');

    //            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
    //                $('#edit-application-modal').modal('hide');
    //                $('.text-info').text("You are not Logged In!");
    //                $('#validation-link').trigger("click");

    //                setTimeout(function () {

    //                    window.location.reload();

    //                },
    //                    2000
    //                );

    //            }
    //            else if (data["newSession"] != null && data["newSession"] == true) {

    //                $('.text-info').text(data["message1"]);
    //                $('#validation-link').trigger("click");

    //                setTimeout(function () {

    //                    window.location.reload();

    //                },
    //                    2000
    //                );

    //            }
    //            else if (data["noPermission"] != null && data["noPermission"] == true) {
    //                $('#edit-application-modal').modal('hide');
    //                $('.text-info').text("Access Denied. No Permission!");
    //                $('#validation-link').trigger("click");
    //            }
    //            else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

    //                $('#edit-application-modal').modal('hide');

    //                setTimeout(function () {

    //                    fillApplicationListPV();

    //                },
    //                    1500
    //                );

    //                $('.text-info').text(data["message"]);
    //                $('#validation-link').trigger("click");

    //            }
    //            else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

    //                $('.text-info').text(data["message"]);
    //                $('#validation-link').trigger("click");
    //            }
    //            else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

    //                $('.text-info').text(data["message"]);
    //                $('#validation-link').trigger("click");
    //            }


    //        },
    //        error: function (error) {
    //            currentBtn.removeClass('button-processing');
    //            $('#edit-application-modal').modal('hide');
    //            $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
    //            $('#validation-link').trigger("click");
    //        }


    //    });

    //}
    //);





    $(document).on("click", "#Edit-App-Navigation-Form .submit", function (e) {

        e.preventDefault();

        $('#edit-app-navigation-modal .form-overlay-loading').fadeIn();

        var url = $('#Edit-App-Navigation-Form').attr('action');
        var data = $('#Edit-App-Navigation-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#edit-app-navigation-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#edit-permission-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#edit-app-navigation-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#edit-app-navigation-modal').modal('hide');

                    setTimeout(function () {

                        fillAppNavigationListPV();

                    },
                        1500
                    );

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
                currentBtn.removeClass('button-processing');
                $('#edit-app-navigation-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", "#Edit-Permission-Form .submit", function (e) {

        e.preventDefault();

        $('#edit-permission-modal .form-overlay-loading').fadeIn();

        var url = $('#Edit-Permission-Form').attr('action');
        var data = $('#Edit-Permission-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#edit-role-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#edit-permission-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#edit-permission-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#edit-permission-modal').modal('hide');

                    setTimeout(function () {

                        fillPermissionListPV();

                    },
                        1500
                    );

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
                currentBtn.removeClass('button-processing');
                $('#edit-permission-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );





    $(document).on("click", "#Add-Permission-Application-Form .submit", function (e) {

        e.preventDefault();

        $('#add-permission-application-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-Permission-Application-Form').attr('action');
        var data = $('#Add-Permission-Application-Form').serialize() + '&ApplicationIds=' + $('#Add-Permission-Application-Form #ddlApplicationId').val();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            async: true,
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                currentBtn.removeClass('button-processing');
                $('#add-permission-application-modal .form-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#add-permission-application-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#add-permission-application-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {


                    $('#add-permission-application-modal').modal('hide');

                    setTimeout(function () {

                        fillPermissionApplicationListPV();

                    },
                        1500
                    );

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

                currentBtn.removeClass('button-processing');
                $('#add-permission-application-modal').modal('hide');

                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");

            }


        });

    }
    );


    $(document).on("click", "#Edit-App-Navigation-Permission-Order-Form .submit", function (e) {

        e.preventDefault();

        $('#edit-permission-order-modal .form-overlay-loading').fadeIn();

        var url = $('#Edit-App-Navigation-Permission-Order-Form').attr('action');
        var data = $('#Edit-App-Navigation-Permission-Order-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                currentBtn.removeClass('button-processing');
                $('#edit-role-modal .form-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#edit-permission-order-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#edit-permission-order-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#edit-permission-order-modal').modal('hide');

                    setTimeout(function () {

                        fillAppNavigationPermissionListPV();

                    },
                        2000
                    );

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["dbEx"] != null && data["dbEx"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }


            },
            error: function (error) {

                currentBtn.removeClass('button-processing');
                $('#edit-permission-order-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", "#Add-App-Navigation-Form .submit", function (e) {

        e.preventDefault();

        $('#create-app-navigation-modal .form-overlay-loading').fadeIn();

        var url = $('#Add-App-Navigation-Form').attr('action');
        var data = $('#Add-App-Navigation-Form').serialize();

        var currentBtn = $(this);

        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('#create-app-navigation-modal .form-overlay-loading').fadeOut();
                currentBtn.removeClass('button-processing');

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#create-app-navigation-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#create-app-navigation-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#create-app-navigation-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        fillAppNavigationListPV();

                    },
                        1500
                    );


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
                currentBtn.removeClass('button-processing');
                $('#create-app-navigation-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );



    // Edit Button EventListener

    $(document).on("click", ".edit-app-navigation-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $('#pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {
                $('#pv-overlay-loading').fadeOut();
                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#edit-app-navigation-modal .modal-dialog').html(data);

                    $('#edit-app-navigation-modal').modal('show');
                }

            },
            error: function (error) {
                $('#pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );


    $(document).on("click", ".edit-autre-user-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $('#pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {
                $('#pv-overlay-loading').fadeOut();
                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#edit-autre-user-modal .modal-dialog').html(data);

                    $('#edit-autre-user-modal').modal('show');
                }

            },
            error: function (error) {
                $('#pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );

    $(document).on("click", ".edit-user-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $('#pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {
                $('#pv-overlay-loading').fadeOut();
                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#edit-user-modal .modal-dialog').html(data);

                    $('#edit-user-modal').modal('show');
                }

            },
            error: function (error) {
                $('#pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );



    $(document).on("click", ".edit-role-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $('#pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {
                $('#pv-overlay-loading').fadeOut();
                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#edit-role-modal .modal-dialog').html(data);

                    $('#edit-role-modal').modal('show');
                }

            },
            error: function (error) {
                $('#pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );



    $(document).on("click", ".edit-application-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $('#pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {
                $('#pv-overlay-loading').fadeOut();
                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#edit-application-modal .modal-dialog').html(data);

                    $('#edit-application-modal').modal('show');
                }

            },
            error: function (error) {
                $('#pv-overlay-loading').fadeout();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );

    $(document).on("click", ".edit-permission-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $('#pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {
                $('#pv-overlay-loading').fadeOut();
                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#edit-permission-modal .modal-dialog').html(data);

                    $('#edit-permission-modal').modal('show');
                }

            },
            error: function (error) {
                $('#pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );


    $(document).on("click", ".edit-permission-order-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $('#pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {
                $('#pv-overlay-loading').fadeOut();
                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#edit-permission-order-modal .modal-dialog').html(data);

                    $('#edit-permission-order-modal').modal('show');
                }

            },
            error: function (error) {
                $('#pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );




    // End Edit Button EventListener


    $(document).on("click", "#Login-Form .submit", function (e) {

        e.preventDefault();

        var url = $('#Login-Form').attr('action');
        var data = $('#Login-Form').serialize();

        var ddlComp = $('#ddlCompagnie');

        ddlComp.html('<option>Select Company</option>');

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,

            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {
                console.log(data);

                if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    currentBtn.removeClass('button-processing');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");


                }
                else if (data["nCompagnie"] != null && data["nCompagnie"] == true) {

                    currentBtn.removeClass('button-processing');
                    $('#Login-Form .form-control').addClass('disabled-control');

                    $('.login-btn-block').slideUp(100);
                    $('.reset-password-link').slideUp(100);

                    for (var item in data["compagnies"]) {
                        ddlComp.append('<option value = ' + data["compagnies"][item]["CompagnieId"] + '>' + data["compagnies"][item]["Label"] + '</option>');
                    }

                    $('#Change-Comp-Form').slideDown(100);

                }
                else if (data["logged"] != null && data["logged"] == true) {

                    window.location.replace(data["url"]);

                }
                else if (data["authForm"] != null && data["authForm"] == true) {

                    ShowTwoFactorAuthForm(data["url"]);

                }
                else {

                    $('#account-user-modal .modal-dialog').html(data);

                    $('.show-form-account-user-btn').trigger("click");

                    $('#account-user-modal').modal('show');

                    currentBtn.removeClass('button-processing');

                }


            },
            error: function (error) {

                currentBtn.removeClass('button-processing');

                if (!window.navigator.onLine) {
                    $('.text-info').text('You are Offline. Please Check your Internet Connection!');
                    $('#validation-link').trigger("click");
                }
                else {
                    $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                    $('#validation-link').trigger("click");
                }

            }


        });

    }
    );


    $(document).on("click", "#Account-User-Form .submit", function (e) {

        e.preventDefault();

        $('#account-user-modal .form-overlay-loading').fadeIn();

        var url = $('#Account-User-Form').attr('action');
        var data = $('#Account-User-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                currentBtn.removeClass('button-processing');
                $('#account-user-modal .form-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#account-user-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );


                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#account-user-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["logged"] != null && data["logged"] == true) {

                    window.location.replace(data["url"]);

                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#account-user-modal').modal('hide');

                    $('.text-info').text(data["message"] + " Woy");
                    $('#validation-link').trigger("click");


                }
                else if (data["authForm"] != null && data["authForm"] == true) {

                    $('#two-factor-auth-modal .modal-dialog').html(data["html"]);

                    $('#two-factor-auth-modal').modal('show');

                    currentBtn.removeClass('button-processing');

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

                currentBtn.removeClass('button-processing');
                $('#account-user-modal').modal('hide');
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", "#Two-Factor-Auth-Form .submit", function (e) {

        e.preventDefault();

        var url = $('#Two-Factor-Auth-Form').attr('action');
        var data = $('#Two-Factor-Auth-Form').serialize();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        $('.right-pan #pv-overlay-loading').fadeIn();

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                currentBtn.removeClass('button-processing');
                $('#two-factor-auth-modal .form-overlay-loading').fadeOut();

                $('.right-pan #pv-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#two-factor-auth-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );


                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#two-factor-auth-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["logged"] != null && data["logged"] == true) {

                    $('#two-factor-auth-modal').modal('hide');

                    setTimeout(function () {

                        window.location.replace(data["url"]);

                    },
                        2000
                    );

                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('#two-factor-auth-modal').modal('hide');

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");

                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

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

                currentBtn.removeClass('button-processing');
                $('#two-factor-auth-modal').modal('hide');
                $('.right-pan #pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", ".password-forgotten-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');
        var data = { email: $('#Login-Form #Email').val() };

        $('.right-pan #pv-overlay-loading').fadeIn();

        $.ajax({
            traditional: true,
            type: "POST",
            url: url,
            data: data,
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            success: function (data) {

                $('.right-pan #pv-overlay-loading').fadeOut();

                if (data["notFound"] != null && data["notFound"] == true) {
                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["saved"] != null && data["saved"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");


                }


            },
            error: function (error) {

                $('.right-pan #pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );


    $(document).on("click", ".change-comp-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $.get({
            url: url,
            success: function (data) {

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {

                    $('#change-comp-modal .modal-dialog').html(data);

                    $('#change-comp-modal').modal('show');
                }

            },
            error: function (error) {
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );



    $(document).on("click", ".account-user-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $.get({
            url: url,
            success: function (data) {

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#account-user-modal .modal-dialog').html(data);

                    $('#account-user-modal').modal('show');
                }

            },
            error: function (error) {
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );


    $(document).on("click", ".profil-photo-user-btn", function (e) {

        e.preventDefault();

        var url = $(this).data('url');

        $.get({
            url: url,
            success: function (data) {

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {
                    $('#add-profil-photo-modal .modal-dialog').html(data);

                    $('#add-profil-photo-modal').modal('show');
                }

            },
            error: function (error) {
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }

        });
    }
    );




    $(document).on("click", "#Change-Comp-Form .submit", function (e) {

        e.preventDefault();

        $('#change-comp-modal .form-overlay-loading').fadeIn();

        var currentBtn = $(this);
        currentBtn.addClass('button-processing');

        var url = $('#Change-Comp-Form').attr('action');
        var data = $('#Change-Comp-Form').serialize();

        $.get({
            url: url,
            data: data,
            success: function (data) {
                console.log(data);
                currentBtn.removeClass('button-processing');

                $('#change-comp-modal .form-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {
                    $('#change-comp-modal').modal('hide');
                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );


                }
                else if (data["newSession"] != null && data["newSession"] == true) {

                    $('.text-info').text(data["message1"]);
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {
                    $('#change-comp-modal').modal('hide');
                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["authForm"] != null && data["authForm"] == true) {

                    currentBtn.removeClass('button-processing');

                    ShowTwoFactorAuthForm(data["url"]);

                }
                else if (data["changed"] != null && data["changed"] == true && data["message"] != null) {

                    $('#change-comp-modal').modal('hide');

                    if (data["change"] == true) {

                        $('.text-info').text(data["message"]);
                        $('#validation-link').trigger("click");

                        setTimeout(function () {

                            $('#modal-dialog-message').modal('hide');

                            window.location.replace(data["url"]);

                        },
                            2000
                        );

                    }
                    else {
                        window.location.replace(data["url"]);
                    }

                }


            },
            error: function (error) {

                $('#change-comp-modal').modal('hide');

                currentBtn.removeClass('button-processing');

                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }


        });

    }
    );




    //End Event Listener Delegation


    $(document).on("click", ".add-after-edit", function () {

        $('.id-resetable').attr('value', 0);
        $('.editor-resetable').prop('value', '');
        $('.key-search-resetable').prop('value', '');
        $('.dropdown-resetable').find('option:eq(0)').prop('selected', true);
        $('.editor-resetable-disabled').attr('disabled', false);
        $('.dropdown-resetable').attr('disabled', false);
        $('.date-picker-resetable').val('');
        $('.radio-button-resetable').prop('checked', false);
        $('.check-box-resetable').prop('checked', false);
        $('.check-box').prop('checked', false);
        $('.control-text-resetable').text('');
        $('.file-resetable').val('');
        $('.placeholder-resetable-search-emp').prop('placeholder', 'Enter the Employee Code...');
        $('.img-resetable').prop('src', '');
        $('.img-resetable').prop('title', '');
        $('.img-resetable').remove();

        $('.dropdown-resetable1').html('');

        $('.modal-header .title-conditionned').text($('.modal-header .title-conditionned').text().replace("Edit", "Add"));

        $('.modal-header .info-text').text('');

        $('.multiselect').multiselect('clearSelection');
        $('.multiselect').multiselect('refresh');
        $('.multiselect').multiselect('destroy');
        initializeMultiSelect();

        $('.multiselect-sa').multiselect('clearSelection');
        $('.multiselect-sa').multiselect('refresh');
        $('.multiselect-sa').multiselect('destroy');
        initializeMultiSelectSA();

    }
    );


    if ($('.validation-message').length) {
        $('#validation-link').trigger("click");
    }


    $('#menu-liste').slideToggle(10);
    $('#menu-client').slideToggle(10);
    $('#menu-personne').slideToggle(10);
    $('#menu-container').slideToggle(10);
    $('#menu-operation').slideToggle(10);
    $('#menu-verification').slideToggle(10);
    $('#menu-imex').slideToggle(10);
    $('#menu-tally').slideToggle(10);
    $('#menu-dommage').slideToggle(10);
    $('#menu-tools').slideToggle(10);
    $('#menu-report').slideToggle(10);
    $('#menu-statistique').slideToggle(10);
    $('#menu-securite').slideToggle(10);

    $('#option-liste').click(function () {
        $('#menu-liste').slideToggle(50);
        $('#chevron-liste').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });


    $('#option-client').click(function () {
        $('#menu-client').slideToggle(50);
        $('#chevron-client').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });


    $('#option-personne').click(function () {
        $('#menu-personne').slideToggle(50);
        $('#chevron-personne').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });

    $('#option-container').click(function () {
        $('#menu-container').slideToggle(50);
        $('#chevron-container').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });

    $('#option-operation').click(function () {
        $('#menu-operation').slideToggle(50);
        $('#chevron-operation').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });



    $('#option-verification').click(function () {
        $('#menu-verification').slideToggle(50);
        $('#chevron-verification').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });


    $('#option-imex').click(function () {
        $('#menu-imex').slideToggle(50);
        $('#chevron-imex').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });

    $('#option-tally').click(function () {
        $('#menu-tally').slideToggle(50);
        $('#chevron-tally').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });

    $('#option-tools').click(function () {
        $('#menu-tools').slideToggle(50);
        $('#chevron-tools').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });

    $('#option-securite').click(function () {
        $('#menu-securite').slideToggle(50);
        $('#chevron-securite').toggleClass('glyphicon-chevron-down').toggleClass('glyphicon-chevron-up');
    });



    // Menu Collapse
    btnMMClicked = false;

    $('#btn-main-menu').click(
        function () {

            $('.main-menu').removeClass('iconify-side-menu');
            $('.main-menu-menu').removeClass('iconify-side-menu');
            $('.body-content').removeClass('maximize-body-content-width');

            $('#btn-main-menu > .glyphicon').toggleClass('glyphicon-menu-hamburger').toggleClass('glyphicon-remove');


            if (btnMMClicked == false) {
                $('.main-menu').attr('style', 'display:block !important;');

                btnMMClicked = true;
            }
            else {
                $('.main-menu').attr('style', 'display:none;');

                btnMMClicked = false;
            }

        }
    );



    $('#btn-iconify-main-menu').click(
        function () {
            $('.main-menu-menu .nav-sub').hide(50);
            $('.main-menu').toggleClass('iconify-side-menu');
            $('.main-menu-menu').toggleClass('iconify-side-menu');
            $('.body-content').toggleClass('maximize-body-content-width');
            $('.other-module').slideToggle(50);

        }
    );

    $('.main-menu-menu > .nav-sidebar > .header').click(
        function () {

            $('.main-menu').removeClass('iconify-side-menu');
            $('.main-menu-menu').removeClass('iconify-side-menu');
            $('.body-content').removeClass('maximize-body-content-width');
            $('.other-module').show();
        }
    );


    $(document).on("click", ".show-form-account-user-btn", function () {

        $('.show-form-account-user-btn-block').slideUp(100);
        $('#Account-User-Form').slideDown(100);

    }
    );




    $(document).on("click", ".view-action-button-collapse-btn", function (e) {

        $('#' + this.id + ' > span.glyphicon').toggleClass('glyphicon-menu-down').toggleClass('glyphicon-menu-up');
    }
    );


    $(document).on("click", ".action-button-collapse button", function () {

        $('.action-button-collapse').removeClass('in');
        $('.view-action-button-collapse-btn span.glyphicon').addClass('glyphicon-menu-down').removeClass('glyphicon-menu-up');

    }
    );



    $(document).on("change", "#ManifestFile", function (e) {

        e.preventDefault();

        $('#fileNameText').text($(this).val());

    }
    );

    $(document).on("click", ".current-user-info-btn", function () {

        $('.dopdown-menu').hide(100);

        $('#current-user-info-dropdown-menu').slideToggle(50);
    }
    );

    $(document).on("click", "#current-user-info-dropdown-menu li a", function () {

        $('#current-user-info-dropdown-menu').hide(50);
    }
    );

    $(document).on("click", function (event) {

        var trigger = $(".dropdown");

        if (trigger !== event.target && !trigger.has(event.target).length) {
            trigger.removeClass('open-sm');
        }


        var trigger2 = $(".action-button-collapse.collapse");

        if (trigger2 !== event.target && !trigger2.has(event.target).length) {
            trigger2.removeClass('in');
        }

    });


    $(document).on("click", ".current-user-info.sm", function () {

        $('.user-name-dropdown > .dropdown').toggleClass('open-sm');

        $(".dropdown:not(.usrsm)").removeClass('open-sm');

    });


    $(document).on("click", ".user-name-dropdown > .dropdown .dropdown-menu li a", function () {

        $('.user-name-dropdown > .dropdown').removeClass('open-sm open');

    });




    //Init Notification

    var url = $('#url-get-notif-info').data('url');

    if (url != null) {
        $.get({
            url: url,
            success: function (data) {

                if (data != null && data["notifInfo"]) {

                    if (data["notifInfo"]["notifications"] != null && data["notifInfo"]["notifications"].length > 0) {

                        var notifDDM = $('.notification-dropdown .dropdown .dropdown-menu');

                        if ($('#old-notif').val() != JSON.stringify(data["notifInfo"]["notifications"])) {

                            notifDDM.html('<li style="display:block; width:100%; font-weight:bold; padding:5px;"> Notifications </li >');

                            for (var item in data["notifInfo"]["notifications"]) {

                                notifDDM.append('<li style="">' +
                                    '<a style = "cursor:pointer; min-height:60px; padding:8px;" data-url="' + data["notifInfo"]["notifications"][item]["Url"] + '" class= "sub-view" title = "Cliquez pour voir les détails" >' +
                                    '<span style="display:inline-block; width:12%; margin-right:8px; top:50%; float:left;">' +
                                    '<span style="text-align:center; display:inline-block; background:#0aafca; border-radius:50px; height:35px; width:35px;">' +
                                    '<span class="glyphicon ' + data["notifInfo"]["notifications"][item]["Icon"] + '" style="transform:translateY(50%);"></span>' +
                                    '</span>' +
                                    '</span>' +
                                    '<span style="display:inline-block; width: 80%;white-space: normal;">' +
                                    '' + data["notifInfo"]["notifications"][item]["Label"] + ' : <strong class="text-danger">' + data["notifInfo"]["notifications"][item]["Value"] + '</strong>' +
                                    '</span></a></li>');

                            }

                            $('.notif-icon .badge').text(data["notifInfo"]["notifications"].length);

                            $('.notification-dropdown').show();

                            $('#old-notif').val(JSON.stringify(data["notifInfo"]["notifications"]));


                        }


                    }
                    else {

                        $('.notification-dropdown').hide(100);

                    }

                }

            },
            error: function (error) {

            }

        });
    }


    //Init Notification

    $(document).on("click", ".notif-icon", function () {

        $('.notif-icon .glyphicon-bell').removeClass('bell-ring-animation');

    });


    $(document).on("click", ".notif-icon.sm", function () {

        $('.notification-dropdown > .dropdown').toggleClass('open-sm');

        $(".dropdown:not(.noticonsm)").removeClass('open-sm');

    });


    $(document).on("click", ".notification-dropdown > .dropdown .dropdown-menu li a", function () {

        $('.notification-dropdown > .dropdown').removeClass('open-sm open');

    });

    //Notification Long Polling
    setInterval(function () {

        var url = $('#url-get-notif-info').data('url');

        if (url != null) {
            $.get({
                url: url,
                success: function (data) {

                    if (data != null && data["notifInfo"]) {


                        if ($('#show-notif-resume').length && $('#show-notif-resume').data('val') == true && data["notifInfo"]["employeSansEmpreinte"] != $('.emp-sans-empreinte .data-val').text() && $('#home-page').length && $('#home-page').data('homepage') == true) {

                            $('.emp-sans-empreinte .data-val').text(data["notifInfo"]["employeSansEmpreinte"]);
                            $('.emp-sans-empreinte').css({ 'color': '#ec0101' });

                            $('.notif-resume-bell-box .bell-notif-resume').addClass('bell-ring-animation');
                            $('.notif-resume-bell-box').show(200);

                            if (audioElement != null) {
                                audioElement.play();
                            }

                            setTimeout(function () {

                                $('.notif-resume-bell-box .bell-notif-resume').removeClass('bell-ring-animation');
                                $('.notif-resume-bell-box').hide(200);


                            }, 60000);

                        }

                        if (data["notifInfo"]["notifications"] != null && data["notifInfo"]["notifications"].length > 0 && $('#home-page').length && $('#home-page').data('homepage') == true) {

                            var notifDDM = $('.notification-dropdown .dropdown .dropdown-menu');

                            if ($('#old-notif').val() != JSON.stringify(data["notifInfo"]["notifications"])) {

                                notifDDM.html('<li style="display:block; width:100%; font-weight:bold; padding:5px;"> Notifications </li >');

                                for (var item in data["notifInfo"]["notifications"]) {

                                    notifDDM.append('<li style="">' +
                                        '<a style = "cursor:pointer; min-height:60px; padding:8px;" data-url="' + data["notifInfo"]["notifications"][item]["Url"] + '" class= "sub-view" title = "Cliquez pour voir les détails" >' +
                                        '<span style="display:inline-block; width:12%; margin-right:8px; top:50%; float:left;">' +
                                        '<span style="text-align:center; display:inline-block; background:#0aafca; border-radius:50px; height:35px; width:35px;">' +
                                        '<span class="glyphicon ' + data["notifInfo"]["notifications"][item]["Icon"] + '" style="transform:translateY(50%);"></span>' +
                                        '</span>' +
                                        '</span>' +
                                        '<span style="display:inline-block; width: 80%; margin - left: 10px;; white-space: normal;">' +
                                        '' + data["notifInfo"]["notifications"][item]["Label"] + ' : <strong class="text-danger">' + data["notifInfo"]["notifications"][item]["Value"] + '</strong>' +
                                        '</span></a></li>');

                                }

                                $('.notif-icon .badge').text(data["notifInfo"]["notifications"].length);
                                $('.notif-icon .glyphicon-bell').addClass('bell-ring-animation');

                                if (audioElement != null) {
                                    audioElement.play();
                                }

                                $('.notification-dropdown').show(100);

                                $('#old-notif').val(JSON.stringify(data["notifInfo"]["notifications"]));

                                setTimeout(function () {

                                    $('.notif-icon .glyphicon-bell').removeClass('bell-ring-animation');

                                }, 30000);

                            }

                        }
                        else {

                            $('.notification-dropdown').hide(100);

                        }

                    }

                },
                error: function (error) {

                }

            });
        }


    }, 5000);


    function ShowTwoFactorAuthForm(url) {

        $('.right-pan #pv-overlay-loading').fadeIn();

        $.get({
            url: url,
            success: function (data) {

                $('.right-pan #pv-overlay-loading').fadeOut();

                if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                    $('.text-info').text("You are not Logged In!");
                    $('#validation-link').trigger("click");

                    setTimeout(function () {

                        window.location.reload();

                    },
                        2000
                    );

                }
                else if (data["noPermission"] != null && data["noPermission"] == true) {

                    $('.text-info').text("Access Denied. No Permission!");
                    $('#validation-link').trigger("click");
                }
                else if (data["validationError"] != null && data["validationError"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                    $('.text-info').text(data["message"]);
                    $('#validation-link').trigger("click");
                }
                else {

                    $('#two-factor-auth-modal .modal-dialog').html(data);

                    $('#two-factor-auth-modal').modal('show');
                }

            },
            error: function (error) {

                $('.right-pan #pv-overlay-loading').fadeOut();
                $('.text-info').text('An error has occurred. Please try again or contact Admin if this persists!');
                $('#validation-link').trigger("click");
            }
        });
    }

    $(document).on("click", ".show-form-account-user-btn", function () {

        $('.show-form-account-user-btn-block').slideUp(100);
        $('#Account-User-Form').slideDown(100);

    }
    );

});


function TableMSInitialiser() {

    //DataTable with Multiple Selection initialisation
    var tableMS = $('#datatableMS').DataTable({
        columnDefs: [{
            orderable: false,
            className: 'select-checkbox',
            targets: 0
        }],
        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        order: [
            [1, 'asc']
        ],
        lengthChange: false,
        buttons: ['excel', 'pdf', {
            extend: 'print', exportOptions: { columns: ':visible' }
        },
            { extend: 'colvis' }
        ],
        "aaSorting": []
    });

    tableMS.on("click", "th.select-checkbox", function () {

        if ($('th.select-checkbox').hasClass('selected')) {
            tableMS.rows().deselect();
            $('th.select-checkbox').removeClass('selected');
        }
        else {
            tableMS.rows(':not(.disabled)').select();
            $('th.select-checkbox').addClass('selected');
        }
    }).on("select deselect", function () {
        if (tableMS.rows({ selected: true }).count() != tableMS.rows().count()) {
            $('th.select-checkbox').removeClass('selected');
        }
        else {
            $('th.select-checkbox').addClass('selected');
        }


    });


    ///======================

    ///============================

    tableMS.on("select", function () {

        if (tableMS.rows({ selected: true }).count() == tableMS.rows().count()) {
            $('table.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('table.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = tableMS.rows({ selected: true }).nodes().toArray();



        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);

        }

        $('#EmpIds').val(rowId.toString());

        if (rowId.length > 0) {

            $('.add-punch-manuel-presence-btn').attr('disabled', false);

        }
        else {

            $('.add-punch-manuel-presence-btn').attr('disabled', true);

        }

    });

    tableMS.on("deselect", function () {

        if (tableMS.rows({ selected: true }).count() == tableMS.rows().count()) {
            $('table.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('table.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = tableMS.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#EmpIds').val(rowId.toString());


        if (rowId.length > 0) {

            $('.add-punch-manuel-presence-btn').attr('disabled', false);

        }
        else {

            $('.add-punch-manuel-presence-btn').attr('disabled', true);

        }

    });



    tableMS.buttons().container()
        .appendTo('#datatableMS_wrapper .col-sm-6:eq(0)');


}





function fillApplicationListPV() {


    $('#pv-overlay-loading').fadeIn();
    $.get({
        async: true,
        url: $('#url-get-application-list').data('url'),
        success: function (data) {

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );


            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#application-list-pv').html(data);
                $('#hb-nbre-application').text($('#nbre-application').val());
                tableInitializer();
                $('#pv-overlay-loading').fadeOut();
            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}




function TableMSInvertInitialiser() {

    //DataTable with Multiple Selection initialisation
    var tableMS = $('#datatableMSInvert').DataTable({

        select: {
            style: 'multi',
            selector: 'td:last-child'
        },
        order: [
            [1, 'asc']
        ],
        lengthChange: false,
        buttons: ['excel', 'pdf', {
            extend: 'print', exportOptions: { columns: ':visible' }
        },
            { extend: 'colvis' }
        ],
        "aaSorting": []
    });

    tableMS.on("click", "th.select-checkbox", function () {

        if ($('th.select-checkbox').hasClass('selected')) {
            tableMS.rows().deselect();
            $('th.select-checkbox').removeClass('selected');
        }
        else {
            tableMS.rows(':not(.disabled)').select();
            $('th.select-checkbox').addClass('selected');
        }
    }).on("select deselect", function () {
        if (tableMS.rows({ selected: true }).count() != tableMS.rows().count()) {
            $('th.select-checkbox').removeClass('selected');
        }
        else {
            $('th.select-checkbox').addClass('selected');
        }


    });


    ///======================


    ///============================

    tableMS.on("select", function () {

        if (tableMS.rows({ selected: true }).count() == tableMS.rows().count()) {
            $('table.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('table.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = tableMS.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('.TitreTransportVoyageIds').val(rowId.toString());
        $('.ContainerVoyageIds').val(rowId.toString());

        if (rowId.length > 0) {

            $('.update-titre-transport-voyage-client-btn').attr('disabled', false);
            $('.update-container-voyage-size-btn').attr('disabled', false);

        }
        else {

            $('.update-titre-transport-voyage-client-btn').attr('disabled', true);
            $('.update-container-voyage-size-btn').attr('disabled', true);

        }

    });

    tableMS.on("deselect", function () {

        if (tableMS.rows({ selected: true }).count() == tableMS.rows().count()) {
            $('table.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('table.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = tableMS.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('.TitreTransportVoyageIds').val(rowId.toString());
        $('.ContainerVoyageIds').val(rowId.toString());

        if (rowId.length > 0) {

            $('.update-titre-transport-voyage-client-btn').attr('disabled', false);
            $('.update-container-voyage-size-btn').attr('disabled', false);

        }
        else {

            $('.update-titre-transport-voyage-client-btn').attr('disabled', true);
            $('.update-container-voyage-size-btn').attr('disabled', true);

        }

    });



    tableMS.buttons().container()
        .appendTo('#datatableMSInvert_wrapper .col-sm-6:eq(0)');


}



function reset() {


    $('#versement_lient').hide();
    $('#balance_client').hide();
    $('#paiement_entreprise').hide();
    $('#balance_entreprise').hide();
    $('#date_leve_sabotay').hide();
    $('#dep').hide();
    $('#ret').hide();






    $('.id-resetable').attr('value', 0);
    $('.editor-resetable').prop('value', '');
    $('.key-search-resetable').prop('value', '');
    $('.dropdown-resetable').find('option:eq(0)').prop('selected', true);
    $('.editor-resetable-disabled').attr('disabled', false);
    $('.dropdown-resetable').attr('disabled', false);
    $('.date-picker-resetable').val('');
    $('.radio-button-resetable').prop('checked', false);
    $('.check-box-resetable').prop('checked', false);
    $('.check-box').prop('checked', false);
    $('.control-text-resetable').text('');
    $('.file-resetable').val('');
    $('.placeholder-resetable-search-emp').prop('placeholder', 'Enter the Employee Code...');
    $('.img-resetable').prop('src', '');
    $('.img-resetable').prop('title', '');
    $('.img-resetable').remove();

    $('.dropdown-resetable1').html('');

    $('.modal-header .title-conditionned').text($('.modal-header .title-conditionned').text().replace("Edit", "Add"));

    $('.modal-header .info-text').text('');

    $('.multiselect').multiselect('clearSelection');
    $('.multiselect').multiselect('refresh');
    $('.multiselect').multiselect('destroy');
    initializeMultiSelect();

    $('.multiselect-sa').multiselect('clearSelection');
    $('.multiselect-sa').multiselect('refresh');
    $('.multiselect-sa').multiselect('destroy');
    initializeMultiSelectSA();

}



function TableMSNoSAInitialiser() {

    //DataTable with Multiple Selection initialisation
    var tableMS = $('#datatableMSNoSA').DataTable({

        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        order: [
            [1, 'asc']
        ],
        lengthChange: false,
        buttons: ['excel', 'pdf', {
            extend: 'print', exportOptions: { columns: ':visible' }
        },
            { extend: 'colvis' }
        ],
        "aaSorting": []
    });

    tableMS.on("click", "th.select-checkbox", function () {

        if ($('th.select-checkbox').hasClass('selected')) {
            tableMS.rows().deselect();
            $('th.select-checkbox').removeClass('selected');
        }
        else {
            tableMS.rows(':not(.disabled)').select();
            $('th.select-checkbox').addClass('selected');
        }
    }).on("select deselect", function () {
        if (tableMS.rows({ selected: true }).count() != tableMS.rows().count()) {
            $('th.select-checkbox').removeClass('selected');
        }
        else {
            $('th.select-checkbox').addClass('selected');
        }


    });


    ///======================


    ///============================

    tableMS.on("select", function () {

        if (tableMS.rows({ selected: true }).count() == tableMS.rows().count()) {
            $('table.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('table.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = tableMS.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }


        $('.ContainerVoyageIds').val(rowId.toString());

        if (rowId.length > 0) {

            $('.update-container-voyage-status-btn').attr('disabled', false);

        }
        else {

            $('.update-container-voyage-status-btn').attr('disabled', true);

        }

    });

    tableMS.on("deselect", function () {

        if (tableMS.rows({ selected: true }).count() == tableMS.rows().count()) {
            $('table.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('table.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = tableMS.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('.ContainerVoyageIds').val(rowId.toString());

        if (rowId.length > 0) {

            $('.update-container-voyage-status-btn').attr('disabled', false);

        }
        else {

            $('.update-container-voyage-status-btn').attr('disabled', true);

        }

    });



    tableMS.buttons().container()
        .appendTo('#datatableMSNoSA_wrapper .col-sm-6:eq(0)');


}



function datatableMSCVEInitializer() {


    var datatableMSCVE = $('#datatableMSCVE').DataTable(
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
            searching: true,
            paging: true
        }

    );


    datatableMSCVE.on("click", "th.table1.select-checkbox", function () {

        if ($('th.table1.select-checkbox').hasClass('selected')) {
            datatableMSCVE.rows().deselect();
            $('th.table1.select-checkbox').removeClass('selected');
        }
        else {
            datatableMSCVE.rows().select();
            $('th.table1.select-checkbox').addClass('selected');
        }

    }).on("select deselect", function () {

        if (datatableMSCVE.rows({ selected: true }).count() != datatableMSCVE.rows().count()) {
            $('th.table1.select-checkbox').removeClass('selected');
        }
        else {
            $('th.table1.select-checkbox').addClass('selected');
        }

    });

    $('.delete-container-voyage-export-btn').attr('disabled', true);

    datatableMSCVE.on("select", function () {

        if (datatableMSCVE.rows({ selected: true }).count() == datatableMSCVE.rows().count()) {
            $('#datatableMSCVE.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#datatableMSCVE.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = datatableMSCVE.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#ids').val(rowId.toString());
        $('#ContainerVoyageExportIds').val(rowId.toString());
        $('#ContainerImportTallyIds').val(rowId.toString());
        $('#ContainerExportTallyIds').val(rowId.toString());
        $('#ContainerEmptyExportTallyIds').val(rowId.toString());


        if (rowId.length > 0) {
            $('.delete-container-voyage-export-btn').attr('disabled', false);
            $('.delete-container-empty-export-tally-btn').attr('disabled', false);
            $('.delete-container-import-tally-btn').attr('disabled', false);
            $('.delete-container-export-tally-btn').attr('disabled', false);

        }
        else {
            $('.delete-container-voyage-export-btn').attr('disabled', true);
            $('.delete-container-empty-export-tally-btn').attr('disabled', true);
            $('.delete-container-import-tally-btn').attr('disabled', true);
            $('.delete-container-export-tally-btn').attr('disabled', true);

        }

    });

    datatableMSCVE.on("deselect", function () {

        if (datatableMSCVE.rows({ selected: true }).count() == datatableMSCVE.rows().count()) {
            $('#datatableMSCVE.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#datatableMSCVE.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = datatableMSCVE.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#ids').val(rowId.toString());
        $('#ContainerVoyageExportIds').val(rowId.toString());
        $('#ContainerImportTallyIds').val(rowId.toString());
        $('#ContainerExportTallyIds').val(rowId.toString());
        $('#ContainerEmptyExportTallyIds').val(rowId.toString());

        if (rowId.length > 0) {
            $('.delete-container-voyage-export-btn').attr('disabled', false);
            $('.delete-container-empty-export-tally-btn').attr('disabled', false);
            $('.delete-container-import-tally-btn').attr('disabled', false);
            $('.delete-container-export-tally-btn').attr('disabled', false);

        }
        else {
            $('.delete-container-voyage-export-btn').attr('disabled', true);
            $('.delete-container-empty-export-tally-btn').attr('disabled', true);
            $('.delete-container-import-tally-btn').attr('disabled', true);
            $('.delete-container-export-tally-btn').attr('disabled', true);

        }

    });
}



function datatableMSCVFEInitializer() {


    var datatableMSCVFE = $('#datatableMSCVFE').DataTable(
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
            searching: true,
            paging: true
        }

    );


    datatableMSCVFE.on("click", "th.table2.select-checkbox", function () {

        if ($('th.table2.select-checkbox').hasClass('selected')) {
            datatableMSCVFE.rows().deselect();
            $('th.table2.select-checkbox').removeClass('selected');
        }
        else {
            datatableMSCVFE.rows().select();
            $('th.table2.select-checkbox').addClass('selected');
        }

    }).on("select deselect", function () {

        if (datatableMSCVFE.rows({ selected: true }).count() != datatableMSCVFE.rows().count()) {
            $('th.table2.select-checkbox').removeClass('selected');
        }
        else {
            $('th.table2.select-checkbox').addClass('selected');
        }

    });

    $('.add-container-voyage-export-btn').attr('disabled', true);

    datatableMSCVFE.on("select", function () {

        if (datatableMSCVFE.rows({ selected: true }).count() == datatableMSCVFE.rows().count()) {
            $('#datatableMSCVFE.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#datatableMSCVFE.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = datatableMSCVFE.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#ids').val(rowId.toString());
        $('#ContainerVoyageIds').val(rowId.toString());

        if (rowId.length > 0) {
            $('.add-container-voyage-export-btn').attr('disabled', false);
            $('.add-container-empty-export-tally-btn').attr('disabled', false);
            $('.add-container-import-tally-btn').attr('disabled', false);
            $('.add-container-export-tally-btn').attr('disabled', false);

        }
        else {
            $('.add-container-voyage-export-btn').attr('disabled', true);
            $('.add-container-empty-export-tally-btn').attr('disabled', true);
            $('.add-container-import-tally-btn').attr('disabled', true);
            $('.add-container-export-tally-btn').attr('disabled', true);

        }

    });

    datatableMSCVFE.on("deselect", function () {

        if (datatableMSCVFE.rows({ selected: true }).count() == datatableMSCVFE.rows().count()) {
            $('#datatableMSCVFE.dataTable tr th.select-checkbox').addClass('selected');
        }
        else {
            $('#datatableMSCVFE.dataTable tr th.select-checkbox').removeClass('selected');
        }

        var rowId = [];

        var trs = [];
        trs = datatableMSCVFE.rows({ selected: true }).nodes().toArray();

        for (var i = 0; i < trs.length; i++) {
            rowId.push(trs[i]['id']);
        }

        $('#ids').val(rowId.toString());
        $('#ContainerVoyageIds').val(rowId.toString());

        if (rowId.length > 0) {
            $('.add-container-voyage-export-btn').attr('disabled', false);
            $('.add-container-empty-export-tally-btn').attr('disabled', false);
            $('.add-container-import-tally-btn').attr('disabled', false);
            $('.add-container-export-tally-btn').attr('disabled', false);

        }
        else {
            $('.add-container-voyage-export-btn').attr('disabled', true);
            $('.add-container-empty-export-tally-btn').attr('disabled', true);
            $('.add-container-import-tally-btn').attr('disabled', true);
            $('.add-container-export-tally-btn').attr('disabled', true);

        }

    });
}



function tableInitializer() {

    var table = $('#datatable').DataTable({
        destroy: true,
        lengthChange: false,
        buttons: [{
            extend: 'excel', exportOptions: { columns: ':not(.no-print)' }
        },
        {
            extend: 'pdf', exportOptions: { columns: ':not(.no-print)' }
        },
        {
            extend: 'print', exportOptions: { columns: ':visible :not(.no-print)' }
        },
        { extend: 'colvis' }
        ],
        "aaSorting": []
    });

    table.buttons().container()
        .appendTo('#datatable_wrapper .col-sm-6:eq(0)');

}


function table1Initializer() {

    var title = $('#xlsxNotImportedDatatable').data('title');

    var table1 = $('#xlsxNotImportedDatatable').DataTable({
        lengthChange: false,
        buttons: [{
            extend: 'excel', exportOptions: { columns: ':not(.no-print)' },
            title: title
        }],
        "aaSorting": [],
        "searching": false,
        "paging": false,
        "info": false
    });

    table1.buttons().container()
        .appendTo('#xlsxNotImportedDatatable_wrapper .col-sm-6:eq(0)');

}



function table2Initializer() {

    var title = $('#datatable-employee-pending').data('title');

    var table2 = $('#datatable-employee-pending').DataTable({
        lengthChange: false,
        buttons: [{
            extend: 'excel', exportOptions: { columns: ':not(.no-print)' },
            title: title
        }],
        "aaSorting": [],
        info: false
    });

    table2.buttons().container()
        .appendTo('#datatable-employee-pending_wrapper .col-sm-6:eq(0)');

}


function table3Initializer() {

    var title = $('#datatable-employee-reintegrate').data('title');

    var table3 = $('#datatable-employee-reintegrate').DataTable({
        lengthChange: false,
        buttons: [{
            extend: 'excel', exportOptions: { columns: ':not(.no-print)' },
            title: title
        }],
        "aaSorting": [],
        info: false
    });

    table3.buttons().container()
        .appendTo('#datatable-employee-reintegrate_wrapper .row .col-sm-6:eq(0)');

}


function datatableListEmpInitializer() {

    var table = $('#datatable-list-emp').DataTable({
        responsive: true,
        destroy: true,
        lengthChange: false,
        columnDefs: [{
            "searchable": false,
            "orderable": false,
            "targets": 0
        }],
        buttons: [{
            extend: 'excel', exportOptions: { columns: ':not(.no-print)' }
        }
        ],
        "aaSorting": [],
        info: false,
        paging: false
    });

    table.on('order.dt search.dt', function () {
        table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();

    table.buttons().container()
        .appendTo('#datatable-list-emp_wrapper .col-sm-6:eq(0)');

}


function fillUserListPV() {

    $('#pv-overlay-loading').fadeIn();
    $.get({
        url: $('#url-get-user-list').data('url'),
        success: function (data) {

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#user-list-pv').html(data);
                $('#hb-nbre-user').text($('#nbre-user').val());
                tableInitializer();
                $('#pv-overlay-loading').fadeOut();
            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}



function fillRoleListPV() {

    $('#pv-overlay-loading').fadeIn();

    $.get({
        url: $('#url-get-role-list').data('url'),
        success: function (data) {

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#role-list-pv').html(data);
                $('#hb-nbre-role').text($('#nbre-role').val());
                tableInitializer();
                $('#pv-overlay-loading').fadeOut();
            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}


function fillRolePermissionListPV() {

    $('#pv-overlay-loading').fadeIn();
    $.get({
        async: true,
        url: $('#url-get-role-permission-list').data('url'),
        success: function (data) {

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );


            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#role-permission-list-pv').html(data);
                $('#hb-nbre-role-permission').text($('#nbre-role-permission').val());
                tableInitializer();
                $('#pv-overlay-loading').fadeOut();
            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}



function fillAppNavigationListPV() {

    $('#pv-overlay-loading').fadeIn();
    $.get({
        async: true,
        url: $('#url-get-app-navigation-list').data('url'),
        success: function (data) {
            $('#pv-overlay-loading').fadeOut();
            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );


            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#app-navigation-list-pv').html(data);
                $('#hb-nbre-app-navigation').text($('#nbre-app-navigation').val());
                tableInitializer();

            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}



function fillAppNavigationPermissionListPV() {

    $('#pv-overlay-loading').fadeIn();
    $.get({
        async: true,
        url: $('#url-get-app-navigation-permission-list').data('url'),
        success: function (data) {

            $('#pv-overlay-loading').fadeOut();

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );


            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#app-navigation-permission-list-pv').html(data);
                $('#hb-nbre-app-navigation-permission').text($('#nbre-app-navigation-permission').val());
                tableInitializer();

                $('#ddlPermissionId').html($('#ddlPermissionIdFilled').html());

                $('.multiselect').multiselect('destroy');
                initializeMultiSelect();

                $('#ddlPermissionIdFilled').html('');

            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}




function fillAppNavigationApplicationListPV() {

    $('#pv-overlay-loading').fadeIn();
    $.get({
        async: true,
        url: $('#url-get-app-navigation-application-list').data('url'),
        success: function (data) {

            $('#pv-overlay-loading').fadeOut();

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );


            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#app-navigation-application-list-pv').html(data);
                $('#hb-nbre-app-navigation-application').text($('#nbre-app-navigation-application').val());
                tableInitializer();

                $('#ddlApplicationId').html($('#ddlApplicationIdFilled').html());

                $('.multiselect').multiselect('destroy');
                initializeMultiSelect();

                $('#ddlApplicationIdFilled').html('');

            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}



function fillRolePermissionListPV() {

    $('#pv-overlay-loading').fadeIn();
    $.get({
        async: true,
        url: $('#url-get-role-permission-list').data('url'),
        success: function (data) {

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );


            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#role-permission-list-pv').html(data);
                $('#hb-nbre-role-permission').text($('#nbre-role-permission').val());
                tableInitializer();
                $('#pv-overlay-loading').fadeOut();
            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}


function fillPermissionListPV() {

    $('#pv-overlay-loading').fadeIn();
    $.get({
        async: true,
        url: $('#url-get-permission-list').data('url'),
        success: function (data) {

            if (data["returnToLogin"] != null && data["returnToLogin"] == true) {

                $('.text-info').text("You are not Logged In!");
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );


            }
            else if (data["newSession"] != null && data["newSession"] == true) {

                $('.text-info').text(data["message1"]);
                $('#validation-link').trigger("click");

                setTimeout(function () {

                    window.location.reload();

                },
                    2000
                );

            }
            else if (data["noPermission"] != null && data["noPermission"] == true) {

                $('.text-info').text("Access Denied. No Permission!");
                $('#validation-link').trigger("click");
            }
            else if (data["notFound"] != null && data["notFound"] == true && data["message"] != null) {

                $('.text-info').text(data["message"]);
                $('#validation-link').trigger("click");
            }
            else {
                $('#permission-list-pv').html(data);
                $('#hb-nbre-permission').text($('#nbre-permission').val());
                tableInitializer();
                $('#pv-overlay-loading').fadeOut();
            }

        },
        error: function (error) {
            $('#pv-overlay-loading').fadeOut();

        }
    });

}



function tableReportInitializer() {

    var table = $('#datatable-report').DataTable({
        responsive: true,
        destroy: true,
        lengthChange: false,
        buttons: [{
            extend: 'excel', exportOptions: { columns: ':not(.no-print)' }
        }
        ],
        "aaSorting": [],
        paging: false,
        info: false,
        searching: false
    });

    table.buttons().container()
        .appendTo('#datatable-report_wrapper .col-sm-6:eq(0)');

}


function initializeMultiSelect() {
    $('.multiselect').multiselect({

        includeSelectAllOption: true

    });
}

function initializeMultiSelectSA() {
    $('.multiselect-sa').multiselect({

        includeSelectAllOption: true,
        numberDisplayed: 2

    });
}



