let menu = document.querySelectorAll('.menu a');
menu.forEach(item => {
    item.onclick = function() {
        menu.forEach(menu_item => menu_item.classList.remove('active'));
        item.classList.add('active');
    }
});