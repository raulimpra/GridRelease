    function FileUsers() {
        $(document).ready(function () {
            LoadTable();
        });
    }

function LoadTable(jsonInfo) {
    $('#TableErrorList').DataTable({
        scrollX: true,
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[1, 'asc']],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json',
        },
        dom: '<"wrapper"lf>rt<"bottom"Bp>',
        buttons: [
            {
                extend: 'excelHtml5',
                titleAttr: 'Exportar a Excel',
            },
            {
                extend: 'pdfHtml5',
                titleAttr: 'Exportar a PDF',
            },
            {
                extend: 'print',
                titleAttr: 'Imprimir',
            },
        ],
    });
 }
