(() => {
    const themeKey = "clinic-management-theme";
    const themeButtons = document.querySelectorAll("[data-theme-toggle]");
    const themeLabels = document.querySelectorAll("[data-theme-label]");

    const applyTheme = (theme) => {
        document.documentElement.dataset.theme = theme;
        const isDark = theme === "dark";

        themeButtons.forEach((button) => {
            button.setAttribute(
                "aria-label",
                `Switch to ${isDark ? "light" : "dark"} theme`);
        });

        themeLabels.forEach((label) => {
            label.textContent = isDark ? "Light mode" : "Dark mode";
        });
    };

    const savedTheme = window.localStorage.getItem(themeKey);
    applyTheme(savedTheme === "dark" ? "dark" : "light");

    themeButtons.forEach((button) => {
        button.addEventListener("click", () => {
            const theme = document.documentElement.dataset.theme === "dark"
                ? "light"
                : "dark";
            window.localStorage.setItem(themeKey, theme);
            applyTheme(theme);
        });
    });

    document.documentElement.classList.add("js-enabled");
    const revealCards = document.querySelectorAll(".reveal-card");
    const reducedMotion = window.matchMedia(
        "(prefers-reduced-motion: reduce)").matches;

    if (reducedMotion || !("IntersectionObserver" in window)) {
        revealCards.forEach((card) => card.classList.add("is-visible"));
        return;
    }

    const observer = new IntersectionObserver((entries, currentObserver) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("is-visible");
                currentObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12 });

    revealCards.forEach((card) => observer.observe(card));
})();
