import federation from "@originjs/vite-plugin-federation";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";
export default defineConfig({
    plugins: [
        react(),
        federation({
            name: "flow_enter_admin",
            filename: "remoteEntry.js",
            exposes: {
                "./OrganizationsApp": "./src/mfe.tsx"
            },
            shared: ["react", "react-dom", "antd", "@tanstack/react-query", "@tanstack/react-table"]
        })
    ],
    server: {
        port: 4173,
        proxy: {
            "/api": {
                target: "http://localhost:5172",
                changeOrigin: true,
                secure: false
            }
        }
    },
    build: {
        target: "esnext"
    },
    preview: {
        port: 4173,
        proxy: {
            "/api": {
                target: "http://localhost:5172",
                changeOrigin: true,
                secure: false
            }
        }
    }
});
