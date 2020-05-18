const PROXY_CONFIG = [
    {
        context: [
            "/api",
            "/signalr"
        ],
        target: "https://localhost:4444",
        secure: false,
        ws: true
    }
]

module.exports = PROXY_CONFIG;
