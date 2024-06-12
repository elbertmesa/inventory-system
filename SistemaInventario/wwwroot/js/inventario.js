var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Inventario/Inventario/ObtenerTodos"
        },
        "columns": [
            { "data": "bodega.nombre", "width": "20%" },
            { "data": "producto.descripcion", "width": "30%" },
            { "data": "producto.costo", "width": "10%", "className": "text-right" },
            { "data": "cantidad", "width": "10%", "className": "text-right" }            
        ]
    });
}

