window.navbarInterop = {
    registerOutsideClick: function (element, dotNetHelper) {
        document.addEventListener("click", function (event) {
            if (!element.contains(event.target)) {
                dotNetHelper.invokeMethodAsync("CloseDropdown");
            }
        });
    }
};