var param = 0;
var userLoaded;
var lowRole;
function Users(lr) { 
    $(document).ready(function () {
        param = 1;
        lowRole = lr.trim();
        LoadAdminUsersGrid('', '');
    });
}

function Report() {
    $(document).ready(function () {
        param = 2;
        LoadAdminUsersReport('', '');
    });
}

function LoadAdminUsersReport(role, family) {
    console.log("ENTRE");
    $('#TableReport').DataTable({
        "ajax": {
            "url": "/UserFamily/GetUserFamiliesReport",
            "type": "GET",
            "datatype": "json",
            "data": { "role": role, "family": family }
        },
        "columns": [
            {
                "data": "cdgemp", render: function (data, type, row) {
                    return '';
                }
            },
            { "data": "cdgemp" },
            { "data": "colemp" },
            { "data": "nmbemp" },
            { "data": "perfil" },
            { "data": "familia" },
        ],
        columnDefs: [
            {
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            },
        ],
        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        order: [[1, 'asc']],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json',
        },
        dom: '<"wrapper"lf>rt<"bottom"Bp>',
        lengthChange: true,
        buttons: ['excel', 'csv', 'print'],
    });
    console.log("SALI");
}

function LoadAdminUsersGrid(role, family) {
    var xindex;
    var userCurrentRol;
    $('#TableUsuarios').DataTable({
        "ajax": {
            "url": "/UserFamily/GetUsersList",
            "type": "GET",
            "datatype": "json",
            "data": { "role": role, "family": family }
        },
        "columns": [
            {
                "data": "cdgemp", render: function (data, type, row) {
                    return '';
                }
            },
            { "data": "cdgemp" },
            {
                "data": "colemp", render: function (data, type, row) {
                    xindex = data;
                    return data;
                }
            },
            { "data": "nmbemp" },
            {
                "data": "perfil", render: function (data, type, row) {
                    userCurrentRol = data.trim();
                    return data;
                }
            },
            {
                "data": "hasFamilies", render: function (data, type, row) {
                    var res = userCurrentRol.localeCompare(lowRole);
                    if (data > 0 && res !== 0) {
                        return '<a href="#" data-toggle="modal" data-target="AdminPopUp" onclick="ShowUserFamilies(\'' + xindex + '\')">Ver Familias</a>';
                    }
                    else { 
                        return '';
                    }
                }
            },
            {
                "data": "null", render: function (data, type, row) {
                    var res = userCurrentRol.localeCompare(lowRole);
                    if (res !== 0) {
                        return '<input class="btn btn-primary" type="submit" value="Editar/Asignar Familia" onclick="EditFamily(\'' + xindex + '\')">';
                    }
                    else {
                        return '';
                    }
                }
            }
        ],
        columnDefs: [
            {
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            },
            {
                //orderable: false,
                //searcheable: false,
                targets: 5
            },
            {
                orderable: false,
                searcheable: false,
                targets: 6
            }
        ],
        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        order: [[1, 'asc']],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json',
        },
        dom: '<"wrapper"lf>rt<"bottom"Bp>',
        lengthChange: false,
        buttons: ['excel', 'csv', 'print'],
    });

    var table = $('#TableUsuarios').DataTable();
    table.buttons().container().appendTo('#TableUsuarios_wrapper.small-6.columns:eq(0)');
}

//Obtiene la lista de familias asignadas al usuario, la converte en un array
//y se la pasa como parametro a la funcion DrawFamiliesList.
function LoadFamiliesList(user) {
    var familiesUser = [];
    userLoaded = user;

    $.ajax({
        type: "Get",
        datatype: "json",
        url: "/UserFamily/GetUserFamiliesList",
        data: { "user": user },
        success: function (response) {
            $.each(response.data, function (key, value) {
                familiesUser.push(value.idFam);
            })
            DrawFamiliesList(familiesUser);
        }
    });
}

//Obtiene los valores del catalogo de familias y lo dibuja en la tabla. despues verifica
//que familias tiene asignadas el usuario y las marca en la lista.
function DrawFamiliesList(userFamilies) { 
    var table = $('#TableFamiliesList').DataTable();
    table.destroy();

    $('#TableFamiliesList').DataTable({
        "ajax": {
            "url": "/UserFamily/GetFamiliesList",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "idFam", render: function (data, type, row) {
                    return '';
                }
            },
            { "data": "idFam" },
            { "data": "family" },
        ],
        columnDefs: [
            {
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            }
        ],
        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        order: [[1, 'asc']],
        "rowCallback": function (row, data, dataIndex) {
            var rowId = data.idFam;
            if ($.inArray(rowId, userFamilies) !== -1) {
                $(row).find('input[type="checkbox"]').prop('checked', true);
                $(row).addClass('selected');
            }
        },
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json',
        },
        dom: 'Lftp',
    });
}

function Suf() { 
    var table = $('#TableFamiliesList').DataTable();
    var lst = table.rows('.selected').data();

    var sufRow = [];
    if (lst != null && lst.length > 0) {
        for (var cont = 0; cont <= lst.length - 1; cont++) {
            var data = lst[cont];
            sufRow.push(data.idFam);
        }
    }

    //if(sufRow.length > 0){
        $.ajax({
            type: "POST",
            datatype: "json",
            url: "/UserFamily/SaveUserFamilies",
            data: { "user": userLoaded, "families": sufRow },
            success: function (response) {
                cosole.log(response);
            }
        });
        LoadFamiliesList(user);
    //}
}
function FilterData() {
    var role = $('#ddlPerfiles').find(":selected").text();
    var family = $('#ddlFamilias').find(":selected").val();

    if (role == "-- Seleccione un Perfil --") role = "";
    if (family == "-- Seleccione una Familia --") family = "";

    var table;
    if(param == 1) table = $('#TableUsuarios').DataTable();
    else table = table = $('#TableReport').DataTable();

    table.clear()
    table.destroy();

    if(param == 1) LoadAdminUsersGrid(role, family);
    else LoadAdminUsersReport(role, family);
}

function SetRole() {
    var table
    if (param = 1) table = $('#TableUsuarios').DataTable();
    else table = $('#TableFamilies').DataTable();

    var lst = table.rows('.selected').data(); 
    var users = [];

    var selectedValue = $('#ddlAsignarPerfil').find(":selected").text();
    if(selectedValue != null){
        if(selectedValue == "-- Seleccione un Perfil --"){
            alert("Por favor seleccione un Perfil.");
            return;
        }
    }

    if (lst != null && lst.length > 0) {
        for (var cont = 0; cont <= lst.length - 1; cont++) {
            var data = lst[cont];
            users.push(data.colemp);
        }
    }
    else{
        alert("No ha seleccionado ningun usuario.");
        return;
    }

    $.ajax({
        url: "/UserFamily/SetRoles",
        type:"POST",
        data: { "role": selectedValue, "users": users},
        success: function (data) {
            //var table = $('#TableFamilies').DataTable();
            table.clear()
            table.destroy();
            if (param = 1) LoadAdminUsersGrid('', '');
            else LoadTable('', '');
        },
        failure: function(err){
            console.log(err);
        }
    });
}

function ShowUserFamilies(user) { 
    $.ajax({
        type: "Get",
        url: "/UserFamily/GetUserFamiliesView",
        data: { "user": user },
        success: function (response) {
            $("#AdminPopUp").find(".modal-body").html(response);
            $("#AdminPopUp").modal('show');
            LoadUserFamiliesView(user);
            $("#btnGuardar").hide();
            $("#btnBorrarTodas").hide();
        }
    });
}

function LoadUserFamiliesView(user) {
    var table = $('#TableFamiliesList').DataTable();
    table.destroy();

    $('#TableFamiliesList').DataTable({
        "ajax": {
            "url": "/UserFamily/GetUserFamiliesList",
            "type": "GET",
            "datatype": "json",
            "data": { "user": user }
        },
        "columns": [
            { "data": "idFam" },
            { "data": "family" }
        ],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json',
        },
        dom: 'Lftp', 
    });
}

function EditFamily(user) { 
    $.ajax({
        type: "Get",
        url: "/UserFamily/EdittUserFamilies",
        data: { "user": user },
        success: function (response) {
            $("#AdminPopUp").find(".modal-body").html(response);
            $("#AdminPopUp").modal('show');
            LoadFamiliesList(user);
            $("#btnGuardar").show();
            $("#btnBorrarTodas").hide();
        }
    });
}
