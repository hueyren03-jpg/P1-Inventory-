function bindSwipeRows() {
    document.querySelectorAll('.data-table tbody tr').forEach((row) => {
        if (row.dataset.swipeBound === '1') return;
        row.dataset.swipeBound = '1';
        let startX = 0;
        let startY = 0;

        row.addEventListener('touchstart', (event) => {
            const touch = event.changedTouches[0];
            startX = touch.clientX;
            startY = touch.clientY;
        }, { passive: true });

        row.addEventListener('touchend', (event) => {
            if (window.innerWidth > 820) return;
            const touch = event.changedTouches[0];
            const dx = touch.clientX - startX;
            const dy = touch.clientY - startY;
            if (Math.abs(dx) < 64 || Math.abs(dx) < Math.abs(dy) * 1.35) return;

            row.classList.add(dx < 0 ? 'swipe-left' : 'swipe-right');
            window.setTimeout(() => row.classList.remove('swipe-left', 'swipe-right'), 260);

            if (dx < 0) row.querySelector('.table-action')?.click();
            else row.querySelector('.record-button')?.click();
        }, { passive: true });
    });
}

const observer = new MutationObserver(() => requestAnimationFrame(bindSwipeRows));

function start() {
    bindSwipeRows();
    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', start, { once: true });
else start();
