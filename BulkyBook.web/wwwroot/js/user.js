let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url:'/admin/user/getall'},
        "columns": [
            { "data": "userName", "autoWidth": true },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "autoWidth": true },
            { "data": "roleName", "autoWidth": true },
            { "data": "company.name", "autoWidth": true },
            {
                data: { id:"id", lockoutEnd:"lockoutEnd"},
                "render": function (data) {
                    const today = new Date().getTime();
                    const lockout = new Date(data.lockoutEnd).getTime();

                    return `
                        <div class="text-center">
                            <a onclick="LockUnlock('${data.id}')" class="btn btn-${lockout > today ? 'danger' : 'success'} text-white" style="cursor:pointer; width:100px;">
                                <i class="bi bi-${lockout < today ? 'lock-fill' : 'unlock-fill'}"></i>  ${lockout < today ? 'Lock' : 'Unlock'}
                            </a>
                            <a href="/Admin/User/RoleManagement?userId=${data.id}" class="btn btn-danger mt-2 mt-xl-0 text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>
                    `;

                   
                },
                "autoWidth": true
            }
        ]
    });
}


function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}