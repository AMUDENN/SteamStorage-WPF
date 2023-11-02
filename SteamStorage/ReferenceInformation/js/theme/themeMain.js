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
    let classList = object.body.classList;
    switch (theme) {
        case 'light':
            classList.remove('dark');
            classList.add(theme);
            window.localStorage.setItem('theme', theme);
            break;
        case 'dark':
            classList.remove('light');
            classList.add(theme);
            window.localStorage.setItem('theme', theme);
            break;
        default:
            let prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)').matches;
            changeTheme(object, prefersDarkScheme ? 'dark' : 'light');
    }
}