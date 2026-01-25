// Toast notification handler
function showAlert(message, isSuccess = true) {
    const toastEl = document.getElementById('liveToast');
    if (!toastEl) return;

    const toast = bootstrap.Toast.getOrCreateInstance(toastEl);
    $(toastEl).removeClass('bg-success bg-danger')
        .addClass(isSuccess ? 'bg-success' : 'bg-danger');

    $('#toastMessage').text(message);
    toast.show();
}

// Modal setup for Confirmations (Create, Delete, Update)
function setupModal(options) {
    const { title, body, btnClass, btnText, showInput = false, onConfirm } = options;

    // 1. Check if the modal element exists in DOM first
    const modalElement = document.getElementById('confirmModal');
    if (!modalElement) {
        console.error("Modal element #confirmModal not found in DOM.");
        return;
    }

    // 2. Update content using jQuery
    $('#mTitle').text(title);
    $('#mBody').html(body);
    $('#mBtn').attr('class', 'btn shadow-sm text-white ' + (btnClass || 'btn-primary'))
        .text(btnText || 'Confirm');

    $('#inputSection').toggleClass('d-none', !showInput);
    $('#mInput').val('');
    $('#mBtn').prop('disabled', showInput);

    // 3. Enable button when user types "Delete" (case-sensitive)
    if (showInput) {
        $('#mInput').off('input').on('input', function () {
            const isValid = $(this).val() === 'Delete';
            $('#mBtn').prop('disabled', !isValid);
        });
    }

    // 4. Securely show modal using Bootstrap 5 Native JS
    try {
        const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
        modalInstance.show();
    } catch (e) {
        // Fallback if bootstrap instance fails
        console.warn("Bootstrap instance failed, trying jQuery fallback.");
        $(modalElement).modal('show');
    }

    // 5. Detach previous clicks to prevent multiple submissions
    $('#mBtn').off('click').on('click', function () {
        if (showInput) {
            onConfirm($('#mInput').val());
        } else {
            onConfirm();
        }
    });
}
// Navigation helper
function goBack(defaultUrl) {
    window.history.length > 1 ? window.history.back() : window.location.href = defaultUrl;
}