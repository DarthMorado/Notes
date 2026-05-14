// Auto-grow TextArea
document.querySelectorAll('.auto-grow').forEach(textarea => {
    const resize = () => {
        textarea.style.height = 'auto';
        textarea.style.height = textarea.scrollHeight + 'px';
    };
    textarea.addEventListener('input', resize);
    resize();
});