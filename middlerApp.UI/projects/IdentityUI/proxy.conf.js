const PROXY_CONFIG = [
    {
        context: [
            "/idp",
            "/.well-known",
            "/connect"
        ],
        target: "https://localhost:4445",
        //changeOrigin: true,
        secure: false,
        "bypass": function (req, res, proxyOptions) {
            
            req.headers["X-Forwarded-For"] = "localhost:4200";
            req.headers["X-Forwarded-Proto"] = "http"
            req.headers["X-Forwarded-Host"] = "https://localhost:4200"
        }
    }
]

module.exports = PROXY_CONFIG;