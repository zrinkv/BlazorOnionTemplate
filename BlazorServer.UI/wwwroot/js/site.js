export function setTheme(themeName) {
    localStorage.setItem('theme', themeName);
    document.getElementById("themeId").href = `_content/Radzen.Blazor/css/${themeName}.css`;
}

function getStoredThemeName() {
    if (localStorage.getItem("theme") != null) {
        let theme = localStorage.getItem('theme');
        return theme;
    }
}