import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

// Backend HTTPS address from YouScanDashboard.Server/Properties/launchSettings.json ("https" profile).
const apiTarget = 'https://localhost:7225';

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        port: 5173,
        proxy: {
            '/api': {
                target: apiTarget,
                // The ASP.NET Core development certificate is self-signed.
                secure: false,
            },
        },
    },
});