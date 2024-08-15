let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/company/getAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            
            { data: 'name', "autoWidth": true },
            { data: 'city', "autoWidth": true },
            { data: 'country', "autoWidth": true },
            { data: 'phoneNumber', "autoWidth": true },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/company/upsert/${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick=Delete('/admin/company/delete/${data}') class="btn btn-danger mx-2">
                                <i class="bi bi-trash-fill"></i>
                            </a>
                        </div>`;
                },
                "autoWidth": true
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();

                    toastr.success(data.message);
                },
                error: function (xhr, status, error) {
                    toastr.error('An error occurred while deleting the product.');
                }
            });
        }
    });
}
