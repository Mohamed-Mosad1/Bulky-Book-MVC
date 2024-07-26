﻿let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/product/getAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                data: 'productImages',
                "render": function (data) {
                    if (Array.isArray(data) && data.length > 0) {              
                        return `<img src="${data[0].imageUrl}" alt="Product Image" class="img-thumbnail" style="width: 50px; height: 50px;" />`;
                    }
                    else {
                        return `<img src="/images/default-product-image.jpeg" alt="No Image Available" class="img-thumbnail" style="width: 50px; height: 50px;" />`;
                    }
                },
                "autoWidth": true
            }
,
            { data: 'title', "autoWidth": true },
            { data: 'isbn', "autoWidth": true },
            { data: 'listPrice', "autoWidth": true },
            { data: 'author', "autoWidth": true },
            { data: 'categoryName', "autoWidth": true },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/product/upsert/${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2">
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
