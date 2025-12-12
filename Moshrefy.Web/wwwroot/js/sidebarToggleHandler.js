    document.addEventListener('DOMContentLoaded', function() {
        const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    const pushMenu = document.querySelector('[data-widget="pushmenu"]');

    if (isCollapsed && pushMenu) {
        // Trigger the actual button click for smooth animation
        setTimeout(function () {
            pushMenu.click();
        }, 100);
        }

    // Save state when toggled
    if (pushMenu) {
        pushMenu.addEventListener('click', function () {
            setTimeout(function () {
                const collapsed = document.body.classList.contains('sidebar-collapse');
                localStorage.setItem('sidebarCollapsed', collapsed);
            }, 350);
        });
        }
    });
