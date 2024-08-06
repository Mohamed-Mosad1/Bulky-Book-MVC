let dataTable;

$(document).ready(function () {
    const url = window.location.href;
    const status = ["processing", "completed", "pending", "approved"].find(s => url.includes(s)) || "all";
    loadDataTable(status);
});


function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: `/admin/order/getall?status=${status}` },
        "columns": [
            { data: 'id', "autoWidth": true },
            { data: 'userName', "autoWidth": true },
            { data: 'orderAddress.phoneNumber', "autoWidth": true },
            { data: 'email', "autoWidth": true },
            { data: 'orderStatus', "autoWidth": true },
            { data: 'orderTotal', "autoWidth": true },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>               
                    
                    </div>`
                },
                "autoWidth": true
            }
        ]
    });
}
