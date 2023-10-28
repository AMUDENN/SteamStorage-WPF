let themeSelect = document.querySelector('.theme-container select');
let iframe = document.querySelector('iframe');

themeSelect.addEventListener('change', event => {
    changeTheme(themeSelect.value)
})

iframe.addEventListener('load', event => {
    changeTheme(themeSelect.value)
})

window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
    changeTheme(themeSelect.value)
});

function changeTheme(theme) {
    let doc = iframe.contentWindow.document;
    if (theme == 'light') {
        document.body.removeAttribute('theme');
        doc.body.removeAttribute('theme');
    } else if (theme == 'dark') {
        document.body.setAttribute('theme', theme);
        doc.body.setAttribute('theme', theme);
    } else {
        let prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)').matches;
        changeTheme(prefersDarkScheme ? 'dark' : 'light');
    }
}