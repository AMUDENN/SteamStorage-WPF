window.addEventListener('storage', event => {
    changeTheme(document, window.localStorage.getItem('theme'));
})

window.addEventListener('load', event => {
    changeTheme(document, window.localStorage.getItem('theme'));
})

function changeTheme(object, theme) {
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