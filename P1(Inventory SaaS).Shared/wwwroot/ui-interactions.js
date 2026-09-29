let dotnet = null;
let keyHandler = null;
let onlineHandler = null;
let offlineHandler = null;
let systemThemeHandler = null;
let fallbackActionHandler = null;
let activeTheme = 'system';

function resolvedTheme(mode) {
    if (mode === 'system') {
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    return mode;
}

export function getStoredTheme() {
    return localStorage.getItem('p1-theme') || 'system';
}

export function applyTheme(mode) {
    activeTheme = ['light', 'dark', 'system'].includes(mode) ? mode : 'system';
    localStorage.setItem('p1-theme', activeTheme);
    document.documentElement.dataset.themeMode = activeTheme;
    document.documentElement.dataset.theme = resolvedTheme(activeTheme);
}

function bindColumnResizers() {
    document.querySelectorAll('.data-table th.resizable').forEach((th) => {
        if (th.dataset.resizeBound === '1') return;
        th.dataset.resizeBound = '1';

        th.addEventListener('pointerdown', (event) => {
            const rect = th.getBoundingClientRect();
            const nearEdge = rect.right - event.clientX <= 8;
            if (!nearEdge) return;

            event.preventDefault();
            event.stopPropagation();
            const startX = event.clientX;
            const startWidth = rect.width;
            th.classList.add('is-resizing');
            document.body.classList.add('grid-resizing');

            const move = (moveEvent) => {
                const next = Math.max(80, Math.min(420, startWidth + moveEvent.clientX - startX));
                th.style.width = `${next}px`;
                th.style.minWidth = `${next}px`;
            };

            const up = () => {
                th.classList.remove('is-resizing');
                document.body.classList.remove('grid-resizing');
                window.removeEventListener('pointermove', move);
                window.removeEventListener('pointerup', up);
            };

            window.addEventListener('pointermove', move);
            window.addEventListener('pointerup', up, { once: true });
        });

        th.addEventListener('pointermove', (event) => {
            const rect = th.getBoundingClientRect();
            th.style.cursor = rect.right - event.clientX <= 8 ? 'col-resize' : '';
        });
    });
}

function hasBlazorClickHandler(button) {
    return [...button.attributes].some((attribute) => attribute.name.startsWith('_bl_'));
}

function normalizeButtonLabel(button) {
    return (button.innerText || button.textContent || '')
        .replace(/\s+/g, ' ')
        .replace(/^\W+|\W+$/g, '')
        .trim();
}

function actionGroup(button) {
    if (button.classList.contains('tab')) return 'tab';
    if (button.classList.contains('filter-chip')) return 'filter';
    if (button.classList.contains('role-item')) return 'role';
    if (button.closest('.settings-nav')) return 'settings';
    return 'action';
}

function selectPeer(button, group) {
    if (!['tab', 'filter', 'role', 'settings'].includes(group)) return;
    const container = button.closest('.tabbar, .search-filters, .role-list, .settings-nav');
    if (!container) return;
    container.querySelectorAll('button.active').forEach((item) => item.classList.remove('active'));
    button.classList.add('active');
}

function formKey(element, index) {
    const label = element.closest('label')?.querySelector('span')?.textContent?.trim();
    return element.name || element.id || label || `${element.tagName.toLowerCase()}-${index}`;
}

export function saveCurrentForm(path) {
    const values = [...document.querySelectorAll('main input, main select, main textarea')].map((element, index) => ({
        key: formKey(element, index),
        type: element.type,
        value: element.type === 'checkbox' ? element.checked : element.value
    }));
    localStorage.setItem(`stockpilot-form:${path}`, JSON.stringify(values));
}

export function restoreCurrentForm(path) {
    const stored = localStorage.getItem(`stockpilot-form:${path}`);
    const values = stored ? JSON.parse(stored) : null;
    [...document.querySelectorAll('main input, main select, main textarea')].forEach((element, index) => {
        const saved = values?.find((item) => item.key === formKey(element, index));
        if (saved) {
            if (element.type === 'checkbox') element.checked = Boolean(saved.value);
            else element.value = saved.value ?? '';
        } else if (element.type === 'checkbox') {
            element.checked = element.defaultChecked;
        } else {
            element.value = element.defaultValue ?? '';
        }
        element.dispatchEvent(new Event('change', { bubbles: true }));
    });
}

export function markAllTableRows(status) {
    document.querySelectorAll('.data-table tbody tr').forEach((row) => {
        const cell = row.lastElementChild;
        const pill = cell?.querySelector('.status-pill');
        if (pill) pill.textContent = status;
    });
}

export function keepOnlyFirstTableRow() {
    document.querySelectorAll('.data-table tbody').forEach((body) => {
        [...body.querySelectorAll('tr')].slice(1).forEach((row) => row.remove());
    });
}

export function downloadMockFile(fileName, content, mimeType) {
    const blob = new Blob([content], { type: mimeType || 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();
    URL.revokeObjectURL(url);
}

export function refreshAdvancedUi() {
    requestAnimationFrame(bindColumnResizers);
}

export function initializeUiInteractions(dotNetRef) {
    dotnet = dotNetRef;

    keyHandler = (event) => {
        const target = event.target;
        const typing = target && (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.tagName === 'SELECT' || target.isContentEditable);

        if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
            event.preventDefault();
            dotnet.invokeMethodAsync('HandleShortcut', 'command');
            return;
        }

        if (event.key === 'Escape') {
            dotnet.invokeMethodAsync('HandleShortcut', 'escape');
            return;
        }

        if (typing) return;

        if (event.key === '/') {
            event.preventDefault();
            dotnet.invokeMethodAsync('HandleShortcut', 'focus-search');
        } else if (event.key.toLowerCase() === 'n') {
            dotnet.invokeMethodAsync('HandleShortcut', 'new-product');
        } else if (event.key.toLowerCase() === 's') {
            dotnet.invokeMethodAsync('HandleShortcut', 'stock-in');
        } else if (event.key.toLowerCase() === 't') {
            dotnet.invokeMethodAsync('HandleShortcut', 'transfer');
        } else if (event.key.toLowerCase() === 'b') {
            dotnet.invokeMethodAsync('HandleShortcut', 'scanner');
        }
    };

    onlineHandler = () => dotnet?.invokeMethodAsync('HandleOnlineState', true);
    offlineHandler = () => dotnet?.invokeMethodAsync('HandleOnlineState', false);
    systemThemeHandler = () => {
        if (activeTheme === 'system') applyTheme('system');
    };
    fallbackActionHandler = (event) => {
        const button = event.target.closest('button');
        if (!button || button.disabled || hasBlazorClickHandler(button)) return;

        const label = normalizeButtonLabel(button);
        if (!label || label === 'Previous' || label === 'Next') return;

        event.preventDefault();
        const group = actionGroup(button);
        selectPeer(button, group);
        dotnet?.invokeMethodAsync('HandleUiAction', label, window.location.pathname, group);
    };

    window.addEventListener('keydown', keyHandler);
    window.addEventListener('online', onlineHandler);
    window.addEventListener('offline', offlineHandler);
    document.addEventListener('click', fallbackActionHandler);
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', systemThemeHandler);

    const stored = getStoredTheme();
    applyTheme(stored);
    bindColumnResizers();
    dotnet.invokeMethodAsync('HandleOnlineState', navigator.onLine);
}

export function disposeUiInteractions() {
    if (keyHandler) window.removeEventListener('keydown', keyHandler);
    if (onlineHandler) window.removeEventListener('online', onlineHandler);
    if (offlineHandler) window.removeEventListener('offline', offlineHandler);
    if (systemThemeHandler) window.matchMedia('(prefers-color-scheme: dark)').removeEventListener('change', systemThemeHandler);
    if (fallbackActionHandler) document.removeEventListener('click', fallbackActionHandler);
    keyHandler = onlineHandler = offlineHandler = systemThemeHandler = fallbackActionHandler = null;
    dotnet = null;
}
