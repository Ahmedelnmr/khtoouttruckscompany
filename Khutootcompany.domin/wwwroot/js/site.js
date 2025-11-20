// ============================================
// KhutootTrucks.Web/wwwroot/js/site.js
// Interactive Features & Utilities
// ============================================

// Document Ready
$(document).ready(function () {
    // Initialize all tooltips
    initializeTooltips();

    // Auto-dismiss alerts after 5 seconds
    autoDismissAlerts();

    // Confirm delete dialogs
    setupDeleteConfirmations();

    // DataTables default configuration
    setupDataTablesDefaults();

    // Form validation
    setupFormValidation();

    // Date pickers (Arabic)
    setupDatePickers();

    // Sidebar active link highlight
    highlightActiveMenuItem();

    // Auto-refresh dashboard
    setupAutoRefresh();
});

// ============================================
// Initialize Bootstrap Tooltips
// ============================================
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// ============================================
// Auto-dismiss Success Alerts
// ============================================
function autoDismissAlerts() {
    setTimeout(function () {
        $('.alert.alert-success').fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);
}

// ============================================
// Confirm Delete Dialogs
// ============================================
function setupDeleteConfirmations() {
    $('form[action*="/Delete"]').on('submit', function (e) {
        if (!confirm('هل أنت متأكد من الحذف؟ لا يمكن التراجع عن هذا الإجراء.')) {
            e.preventDefault();
            return false;
        }
    });

    $('.btn-delete').on('click', function (e) {
        if (!confirm('هل أنت متأكد من الحذف؟')) {
            e.preventDefault();
            return false;
        }
    });
}

// ============================================
// DataTables Default Configuration
// ============================================
function setupDataTablesDefaults() {
    // Set RTL and Arabic language for all DataTables
    $.extend(true, $.fn.dataTable.defaults, {
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/ar.json"
        },
        "pageLength": 25,
        "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "الكل"]],
        "dom": '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
            '<"row"<"col-sm-12"tr>>' +
            '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
        "responsive": true,
        "autoWidth": false
    });
}

// ============================================
// Form Validation Enhancement
// ============================================
function setupFormValidation() {
    // Add bootstrap validation classes
    $('form').each(function () {
        $(this).addClass('needs-validation');
    });

    // Validate on submit
    $('form').on('submit', function (e) {
        if (!this.checkValidity()) {
            e.preventDefault();
            e.stopPropagation();
        }
        $(this).addClass('was-validated');
    });

    // Real-time validation
    $('input, select, textarea').on('blur change', function () {
        if ($(this).is(':invalid')) {
            $(this).addClass('is-invalid').removeClass('is-valid');
        } else if ($(this).val() !== '') {
            $(this).addClass('is-valid').removeClass('is-invalid');
        }
    });
}

// ============================================
// Date Pickers (Arabic)
// ============================================
function setupDatePickers() {
    // If using a date picker library, configure it here
    $('input[type="date"]').each(function () {
        // Arabic locale configuration
        $(this).attr('placeholder', 'dd/mm/yyyy');
    });
}

// ============================================
// Highlight Active Menu Item
// ============================================
function highlightActiveMenuItem() {
    var currentPath = window.location.pathname;
    $('.sidebar a').each(function () {
        var href = $(this).attr('href');
        if (href && currentPath.startsWith(href) && href !== '/') {
            $(this).addClass('active');
        } else if (href === '/' && currentPath === '/') {
            $(this).addClass('active');
        }
    });
}

// ============================================
// Auto-refresh Dashboard
// ============================================
function setupAutoRefresh() {
    // Refresh dashboard every 5 minutes if on home page
    if (window.location.pathname === '/' || window.location.pathname === '/Home/Index') {
        setInterval(function () {
            console.log('Auto-refreshing dashboard...');
            location.reload();
        }, 300000); // 5 minutes
    }
}

// ============================================
// Utility Functions
// ============================================

// Format Currency
function formatCurrency(amount) {
    return parseFloat(amount).toLocaleString('ar-KW', {
        style: 'currency',
        currency: 'KWD',
        minimumFractionDigits: 3
    });
}

// Format Date
function formatDate(date) {
    var d = new Date(date);
    var day = ('0' + d.getDate()).slice(-2);
    var month = ('0' + (d.getMonth() + 1)).slice(-2);
    var year = d.getFullYear();
    return day + '/' + month + '/' + year;
}

// Calculate Days Until
function daysUntil(targetDate) {
    var today = new Date();
    var target = new Date(targetDate);
    var diffTime = target - today;
    var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
}

// Show Loading Spinner
function showLoading() {
    var loadingHtml = `
        <div class="loading-overlay" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; 
             background: rgba(0,0,0,0.5); z-index: 9999; display: flex; align-items: center; justify-content: center;">
            <div class="spinner-border text-light" style="width: 3rem; height: 3rem;" role="status">
                <span class="visually-hidden">جاري التحميل...</span>
            </div>
        </div>
    `;
    $('body').append(loadingHtml);
}

// Hide Loading Spinner
function hideLoading() {
    $('.loading-overlay').fadeOut(300, function () {
        $(this).remove();
    });
}

// Toast Notification
function showToast(message, type = 'success') {
    var bgClass = type === 'success' ? 'bg-success' : 'bg-danger';
    var toastHtml = `
        <div class="toast align-items-center text-white ${bgClass} border-0" role="alert" 
             style="position: fixed; top: 80px; right: 20px; z-index: 9999;">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;
    $('body').append(toastHtml);
    var toastElement = $('.toast').last()[0];
    var toast = new bootstrap.Toast(toastElement);
    toast.show();

    setTimeout(function () {
        $(toastElement).remove();
    }, 5000);
}

// ============================================
// Export Functions
// ============================================

// Export Table to Excel
function exportTableToExcel(tableId, filename = 'export') {
    var table = document.getElementById(tableId);
    var html = table.outerHTML;
    var blob = new Blob([html], {
        type: 'application/vnd.ms-excel'
    });
    var link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename + '.xls';
    link.click();
}

// Print Table
function printTable(tableId) {
    var divToPrint = document.getElementById(tableId);
    var newWin = window.open('', 'Print-Window');
    newWin.document.open();
    newWin.document.write(`
        <html>
        <head>
            <title>طباعة</title>
            <style>
                body { font-family: Arial; direction: rtl; }
                table { width: 100%; border-collapse: collapse; }
                th, td { border: 1px solid #ddd; padding: 8px; text-align: right; }
                th { background-color: #343a40; color: white; }
            </style>
        </head>
        <body onload="window.print(); window.close();">
            ${divToPrint.outerHTML}
        </body>
        </html>
    `);
    newWin.document.close();
}

// ============================================
// Color Coding Helpers
// ============================================

// Apply color coding based on expiry date
function applyExpiryColorCoding(element, expiryDate) {
    var days = daysUntil(expiryDate);

    if (days < 0) {
        $(element).addClass('bg-danger text-white');
        $(element).attr('title', 'منتهي');
    } else if (days <= 30) {
        $(element).addClass('bg-danger text-white');
        $(element).attr('title', 'ينتهي خلال شهر');
    } else if (days <= 60) {
        $(element).addClass('bg-warning');
        $(element).attr('title', 'ينتهي خلال شهرين');
    } else {
        $(element).addClass('bg-success text-white');
        $(element).attr('title', 'صالح');
    }
}

// ============================================
// AJAX Helpers
// ============================================

// Generic AJAX POST
function ajaxPost(url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (successCallback) successCallback(response);
        },
        error: function (xhr, status, error) {
            if (errorCallback) errorCallback(error);
            else showToast('حدث خطأ: ' + error, 'danger');
        }
    });
}

// Generic AJAX GET
function ajaxGet(url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            if (successCallback) successCallback(response);
        },
        error: function (xhr, status, error) {
            if (errorCallback) errorCallback(error);
            else showToast('حدث خطأ: ' + error, 'danger');
        }
    });
}

// ============================================
// Mobile Sidebar Toggle
// ============================================
function toggleSidebar() {
    $('.sidebar').toggleClass('show');
}

// Add mobile menu button if on small screen
if ($(window).width() < 992) {
    $('.navbar').prepend(`
        <button class="btn btn-outline-light me-2" onclick="toggleSidebar()">
            <i class="fas fa-bars"></i>
        </button>
    `);
}

// ============================================
// Search Highlight
// ============================================
function highlightSearchTerm(term) {
    if (term && term.length > 0) {
        var regex = new RegExp(term, 'gi');
        $('table tbody td').each(function () {
            var text = $(this).text();
            var newText = text.replace(regex, '<mark>$&</mark>');
            $(this).html(newText);
        });
    }
}

// ============================================
// Keyboard Shortcuts
// ============================================
$(document).on('keydown', function (e) {
    // Ctrl+S to save form
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        $('form').submit();
    }

    // Esc to close modal
    if (e.key === 'Escape') {
        $('.modal').modal('hide');
    }
});

// ============================================
// Console Welcome Message
// ============================================
console.log('%cKhutootTrucks System', 'font-size: 20px; color: #3498db; font-weight: bold;');
console.log('%cVersion 1.0.0 | © 2025 شركة الخطوط الدولية', 'font-size: 12px; color: #7f8c8d;');
console.log('%c⚠️ تحذير: لا تقم بلصق أي كود هنا إلا إذا كنت تعرف ماذا تفعل!', 'font-size: 14px; color: #e74c3c; font-weight: bold;');