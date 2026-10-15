// SIGALE Web App - Global JavaScript

/**
 * Paginación y filtrado dinámico para tablas Bootstrap
 * Permite paginar a 10 registros por página y sincronizar con la búsqueda.
 */
function initTablePagination(tableId = 'tablaDatos', filterId = 'filtroTabla', pageSize = 10) {
    const table = document.getElementById(tableId);
    if (!table) return;

    const tbody = table.querySelector('tbody');
    if (!tbody) return;

    const rows = Array.from(tbody.querySelectorAll('tr'));
    // Si la tabla está vacía o contiene solo la fila informativa de "no hay registros", no paginar
    if (rows.length === 0 || (rows.length === 1 && rows[0].querySelector('td[colspan]'))) {
        return;
    }

    let currentPage = 1;
    let filteredRows = [...rows];

    // Contenedor de controles de paginación
    const card = table.closest('.card') || table.parentElement;
    let paginationContainer = document.getElementById('paginationControls_' + tableId);
    if (!paginationContainer) {
        paginationContainer = document.createElement('div');
        paginationContainer.id = 'paginationControls_' + tableId;
        paginationContainer.className = 'card-footer bg-white border-top d-flex flex-column flex-sm-row justify-content-between align-items-center py-2 px-3 gap-2';
        paginationContainer.innerHTML = `
            <small class="text-muted" id="paginationSummary_${tableId}"></small>
            <nav aria-label="Navegación de tabla">
                <ul class="pagination pagination-sm mb-0" id="paginationList_${tableId}"></ul>
            </nav>
        `;
        card.appendChild(paginationContainer);
    }

    const summaryEl = document.getElementById(`paginationSummary_${tableId}`);
    const listEl = document.getElementById(`paginationList_${tableId}`);

    function render() {
        const totalRows = filteredRows.length;
        const totalPages = Math.ceil(totalRows / pageSize) || 1;

        if (currentPage > totalPages) currentPage = totalPages;
        if (currentPage < 1) currentPage = 1;

        // Ocultar todas las filas originales primero
        rows.forEach(r => r.style.display = 'none');

        // Mostrar solo las filas de la página actual
        const startIndex = (currentPage - 1) * pageSize;
        const endIndex = Math.min(startIndex + pageSize, totalRows);

        for (let i = startIndex; i < endIndex; i++) {
            filteredRows[i].style.display = '';
        }

        // Texto informativo
        if (totalRows === 0) {
            summaryEl.textContent = 'No se encontraron registros coincidentes';
        } else {
            summaryEl.textContent = `Mostrando ${startIndex + 1} a ${endIndex} de ${totalRows} registro${totalRows !== 1 ? 's' : ''}${totalRows !== rows.length ? ` (filtrado de ${rows.length} total)` : ''}`;
        }

        // Botones de paginación
        listEl.innerHTML = '';

        // Botón Anterior
        const prevLi = document.createElement('li');
        prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
        prevLi.innerHTML = `<button class="page-link" type="button" aria-label="Anterior">&laquo; Anterior</button>`;
        prevLi.addEventListener('click', (e) => {
            e.preventDefault();
            if (currentPage > 1) {
                currentPage--;
                render();
            }
        });
        listEl.appendChild(prevLi);

        // Numeración de páginas
        let startPage = Math.max(1, currentPage - 2);
        let endPage = Math.min(totalPages, currentPage + 2);

        if (startPage > 1) {
            addPageButton(1);
            if (startPage > 2) addEllipsis();
        }

        for (let p = startPage; p <= endPage; p++) {
            addPageButton(p);
        }

        if (endPage < totalPages) {
            if (endPage < totalPages - 1) addEllipsis();
            addPageButton(totalPages);
        }

        // Botón Siguiente
        const nextLi = document.createElement('li');
        nextLi.className = `page-item ${currentPage === totalPages ? 'disabled' : ''}`;
        nextLi.innerHTML = `<button class="page-link" type="button" aria-label="Siguiente">Siguiente &raquo;</button>`;
        nextLi.addEventListener('click', (e) => {
            e.preventDefault();
            if (currentPage < totalPages) {
                currentPage++;
                render();
            }
        });
        listEl.appendChild(nextLi);
    }

    function addPageButton(pageNum) {
        const li = document.createElement('li');
        li.className = `page-item ${pageNum === currentPage ? 'active' : ''}`;
        li.innerHTML = `<button class="page-link" type="button">${pageNum}</button>`;
        li.addEventListener('click', (e) => {
            e.preventDefault();
            if (currentPage !== pageNum) {
                currentPage = pageNum;
                render();
            }
        });
        listEl.appendChild(li);
    }

    function addEllipsis() {
        const li = document.createElement('li');
        li.className = 'page-item disabled';
        li.innerHTML = `<span class="page-link">...</span>`;
        listEl.appendChild(li);
    }

    // Evento de búsqueda / filtrado
    const filterInput = document.getElementById(filterId);
    if (filterInput) {
        filterInput.addEventListener('keyup', function () {
            const term = this.value.toLowerCase().trim();
            if (!term) {
                filteredRows = [...rows];
            } else {
                filteredRows = rows.filter(r => r.textContent.toLowerCase().includes(term));
            }
            currentPage = 1;
            render();
        });
    }

    // Render inicial
    render();
}

// Auto-inicializar paginación en cualquier vista que tenga #tablaDatos
document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('tablaDatos')) {
        initTablePagination('tablaDatos', 'filtroTabla', 10);
    }
});
