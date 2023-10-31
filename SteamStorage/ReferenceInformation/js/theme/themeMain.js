let themeSelect = document.querySelector('.theme-container select');

window.addEventListener('load', event => {
    changeTheme(document, themeSelect.value);
})

themeSelect.addEventListener('change', event => {
    changeTheme(document, themeSelect.value);
})

window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
    changeTheme(document, themeSelect.value);
});

function changeTheme(object, theme) {
    window.localStorage.setItem('theme', theme);
    if (theme == 'light') {
        object.body.classList.remove('dark');
        object.body.classList.add('light');
    } else if (theme == 'dark') {
        object.body.classList.remove('light');
        object.body.classList.add('dark');
    } else {
        let prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)').matches;
        changeTheme(object, prefersDarkScheme ? 'dark' : 'light');
    }
}