window.addEventListener('storage', event => {
    changeTheme(document, window.localStorage.getItem('theme'));
})

window.addEventListener('load', event => {
    changeTheme(document, window.localStorage.getItem('theme'));
})

function changeTheme(object, theme) {
    let classList = object.body.classList;
    switch (theme) {
        case 'light':
            classList.remove('dark');
            classList.add(theme);
            break;
        case 'dark':
            classList.remove('light');
            classList.add(theme);
            break;
    }
}