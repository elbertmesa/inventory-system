var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Inventario/Inventario/ObtenerHistorial"
        },
        "columns": [
            { "data": "fechaInicial", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleString();
                }
            },
            { "data": "fechaFinal", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleString();
                }
            },
            { "data": "bodega.nombre", "width": "15%" },
            {
                "data": function nombreUsuario(data) {
                    return (data.usuarioAplicacion.nombres + " " + data.usuarioAplicacion.apellidos).toUpperCase();
              }, "width":"20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Inventario/Inventario/DetalleHistorial/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                             Detalle
                            </a>                           
                        </div>
                        `;
                }, "width": "10%"
            }
        ]
    });
}


function Delete(url) {
    
    swal({
        title: "Esta Seguro que quiere Eliminar la Categoria?",
        text: "Este Registro no se podra recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });    
}