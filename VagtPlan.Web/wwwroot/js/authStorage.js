window.authStorage = {
    key: 'vagtplan.authToken',
    getToken: function () {
        try {
            return localStorage.getItem(this.key);
        } catch {
            return null;
        }
    },
    setToken: function (token) {
        try {
            localStorage.setItem(this.key, token);
        } catch {
            // ignore storage errors (private browsing, etc.)
        }
    },
    removeToken: function () {
        try {
            localStorage.removeItem(this.key);
        } catch {
            // ignore storage errors
        }
    }
};
