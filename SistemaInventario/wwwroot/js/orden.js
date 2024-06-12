var datatable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("pendiente")) {
        loadDataTable("ObtenerOrdenLista?estado=pendiente");
    }
    else {
        if (url.includes("aprobado")) {
            loadDataTable("ObtenerOrdenLista?estado=aprobado");
        }
        else {
            if (url.includes("completado")) {
                loadDataTable("ObtenerOrdenLista?estado=completado");
            }
            else {
                if (url.includes("rechazado")) {
                    loadDataTable("ObtenerOrdenLista?estado=rechazado");
                }
                else {
                    loadDataTable("ObtenerOrdenLista?estado=todas");
                }
            }
        }
    }
    
});

function loadDataTable(url) {
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Admin/Orden/"+url
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "nombresCliente", "width": "15%" },
            { "data": "telefono", "width": "15%" },
            { "data": "usuarioAplicacion.email", "width": "15%" },
            { "data": "estadoOrden", "width": "15%" },
            { "data": "totalOrden", "width": "15%" },            
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Orden/Detalle/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-list-ul"></i>
                            </a>                           
                        </div>
                        `;
                }, "width": "5%"
            }
        ]
    });
}

