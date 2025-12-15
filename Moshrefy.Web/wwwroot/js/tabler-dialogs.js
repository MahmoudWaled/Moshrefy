/**
 * Tabler Dialog Utility
 * A lightweight replacement for SweetAlert2 using native Tabler/Bootstrap modals
 */
window.TablerDialog = (function() {
    'use strict';
    


    // Create modal container if it doesn't exist
    function ensureModalContainer() {
        let container = document.getElementById('tabler-dialog-container');
        if (!container) {

            container = document.createElement('div');
            container.id = 'tabler-dialog-container';
            document.body.appendChild(container);
        }
        return container;
    }

    // Generate unique ID
    function generateId() {
        return 'tabler-dialog-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
    }

    // Get icon HTML based on type
    function getIconHtml(icon) {
        const icons = {
            success: '<div class="text-center mb-3"><i class="ti ti-circle-check text-success" style="font-size: 3.5rem;"></i></div>',
            error: '<div class="text-center mb-3"><i class="ti ti-circle-x text-danger" style="font-size: 3.5rem;"></i></div>',
            warning: '<div class="text-center mb-3"><i class="ti ti-alert-triangle text-warning" style="font-size: 3.5rem;"></i></div>',
            info: '<div class="text-center mb-3"><i class="ti ti-info-circle text-info" style="font-size: 3.5rem;"></i></div>',
            question: '<div class="text-center mb-3"><i class="ti ti-help text-primary" style="font-size: 3.5rem;"></i></div>'
        };
        return icons[icon] || '';
    }

    // Get button color class
    function getButtonClass(color) {
        if (!color) return 'btn-primary';
        const colorMap = {
            '#dc3545': 'btn-danger',
            '#d63939': 'btn-danger',
            '#28a745': 'btn-success',
            '#2fb344': 'btn-success',
            '#ffc107': 'btn-warning',
            '#17a2b8': 'btn-info',
            '#6c757d': 'btn-secondary'
        };
        return colorMap[color.toLowerCase()] || 'btn-primary';
    }

    // Safely get Bootstrap Modal constructor
    function getModalConstructor() {
        if (typeof bootstrap !== 'undefined' && bootstrap.Modal) return bootstrap.Modal;
        if (window.bootstrap && window.bootstrap.Modal) return window.bootstrap.Modal;
        if (window.tabler && window.tabler.Modal) return window.tabler.Modal; // Sometimes bundled here
        return null;
    }

    // Create and show modal
    function createModal(options) {

        const id = generateId();
        const container = ensureModalContainer();
        
        const defaults = {
            title: '',
            text: '',
            html: '',
            icon: '',
            showCancelButton: false,
            showConfirmButton: true,
            confirmButtonText: 'OK',
            cancelButtonText: 'Cancel',
            confirmButtonColor: '',
            cancelButtonColor: '',
            timer: null,
            showLoaderOnConfirm: false,
            input: null,
            inputPlaceholder: '',
            inputValidator: null,
            preConfirm: null,
            allowOutsideClick: true
        };

        const config = { ...defaults, ...options };
        const content = config.html || config.text;
        const iconHtml = getIconHtml(config.icon);
        const confirmBtnClass = getButtonClass(config.confirmButtonColor);

        // Build input field
        let inputHtml = '';
        if (config.input === 'text') {
            inputHtml = `
                <div class="mt-3">
                    <input type="text" class="form-control" id="${id}-input" placeholder="${config.inputPlaceholder}">
                    <div class="invalid-feedback" id="${id}-validation"></div>
                </div>
            `;
        }

        const modalHtml = `
            <div class="modal modal-blur fade" id="${id}" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-body text-center py-4">
                            ${iconHtml}
                            ${config.title ? `<h3 class="mb-2">${config.title}</h3>` : ''}
                            ${content ? `<div class="text-muted">${content}</div>` : ''}
                            ${inputHtml}
                        </div>
                        <div class="modal-footer justify-content-center border-0 pt-0">
                            ${config.showCancelButton ? `<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">${config.cancelButtonText}</button>` : ''}
                            ${config.showConfirmButton ? `<button type="button" class="btn ${confirmBtnClass}" id="${id}-confirm">${config.confirmButtonText}</button>` : ''}
                        </div>
                    </div>
                </div>
            </div>
        `;

        container.innerHTML = modalHtml;
        const modalElement = document.getElementById(id);

        // Try to initialize Bootstrap modal
        const ModalConstructor = getModalConstructor();
        if (!ModalConstructor) {
            console.error('Bootstrap Modal Constructor not found!');
            alert('Error: Bootstrap Modal library not loaded.');
            return Promise.resolve({ isConfirmed: false });
        }

        // Normalize allowOutsideClick (SweetAlert2 allows function/boolean, Bootstrap needs boolean)
        let allowClick = config.allowOutsideClick;
        if (typeof allowClick === 'function') {
            allowClick = allowClick(); // Evaluate it
        }
        // Ensure it is strictly boolean
        const isStatic = allowClick === false || allowClick === 'false';

        const modal = new ModalConstructor(modalElement, {
            backdrop: isStatic ? 'static' : true,
            keyboard: !isStatic
        });

        return new Promise((resolve) => {
            const confirmBtn = document.getElementById(`${id}-confirm`);
            const inputField = document.getElementById(`${id}-input`);
            const validationMsg = document.getElementById(`${id}-validation`);
            let isConfirmed = false;

            if (confirmBtn) {
                confirmBtn.addEventListener('click', async function() {
                    let inputValue = inputField ? inputField.value : null;

                    if (config.inputValidator && inputField) {
                        const validationResult = config.inputValidator(inputValue);
                        if (validationResult) {
                            inputField.classList.add('is-invalid');
                            validationMsg.textContent = validationResult;
                            validationMsg.style.display = 'block';
                            return;
                        }
                    }

                    if (config.showLoaderOnConfirm) {
                        confirmBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Loading...';
                        confirmBtn.disabled = true;
                    }

                    if (config.preConfirm) {
                        try {
                            if (validationMsg) validationMsg.style.display = 'none';
                            
                            const result = await config.preConfirm(inputValue);
                            
                            // Check if validation message is now visible 
                            if (validationMsg && validationMsg.style.display === 'block') {
                                // Validation failed, do not close
                                confirmBtn.innerHTML = config.confirmButtonText;
                                confirmBtn.disabled = false;
                                return;
                            }

                            isConfirmed = true;
                            modal.hide();
                            resolve({ isConfirmed: true, value: result });
                        } catch (error) {
                            if (validationMsg) {
                                inputField.classList.add('is-invalid');
                                validationMsg.textContent = error.message || 'An error occurred';
                                validationMsg.style.display = 'block';
                            }
                            confirmBtn.innerHTML = config.confirmButtonText;
                            confirmBtn.disabled = false;
                        }
                    } else {
                        isConfirmed = true;
                        modal.hide();
                        resolve({ isConfirmed: true, value: inputValue });
                    }
                });
            }

            modalElement.addEventListener('hidden.bs.modal', function() {
                if (!isConfirmed) {
                    resolve({ isConfirmed: false, isDismissed: true });
                }
                setTimeout(() => {
                    if (container.contains(modalElement)) {
                        container.removeChild(modalElement); // Clean DOM manually
                    }
                }, 150);
            });

            if (config.timer) {
                setTimeout(() => {
                    isConfirmed = true;
                    modal.hide();
                    resolve({ isConfirmed: true, isDismissed: true });
                }, config.timer);
            }


            modal.show();
        });
    }

    return {
        fire: createModal,
        confirm: (opts) => createModal({ icon: 'question', showCancelButton: true, ...opts }),
        success: (t, txt, timer = 2000) => createModal({ icon: 'success', title: t || 'Success!', text: txt || '', timer: timer, showConfirmButton: false }),
        error: (t, txt) => createModal({ icon: 'error', title: t || 'Error', text: txt || 'Something went wrong', confirmButtonColor: '#dc3545' }),
        warning: (t, txt) => createModal({ icon: 'warning', title: t || 'Warning', text: txt || '' }),
        info: (t, txt) => createModal({ icon: 'info', title: t, text: txt }),
        
        // Validation helper
        showValidationMessage: (msg) => {
            const validation = document.querySelector('#tabler-dialog-container .invalid-feedback');
            const input = document.querySelector('#tabler-dialog-container .form-control');
            if (validation && input) {
                input.classList.add('is-invalid');
                validation.textContent = msg;
                validation.style.display = 'block';
            }
        },

        // Missing SweetAlert2 methods (for compatibility)
        isLoading: () => {
             const btn = document.querySelector('#tabler-dialog-container button[disabled]');
             return !!btn; // If a button is disabled, we assume it's loading
        },
        close: () => {
             const modalEl = document.querySelector('#tabler-dialog-container .modal.show');
             if (modalEl) {
                 // Try to get instance and hide
                 const ModalConstructor = getModalConstructor();
                 if (ModalConstructor) {
                      const instance = ModalConstructor.getInstance(modalEl);
                      if (instance) instance.hide();
                 }
             }
        },
        getPopup: () => document.querySelector('#tabler-dialog-container .modal-content'),
        getTitle: () => document.querySelector('#tabler-dialog-container h3'),
        getConfirmButton: () => document.querySelector('#tabler-dialog-container .btn-primary, #tabler-dialog-container .btn-danger, #tabler-dialog-container .btn-success'),
    };
})();


