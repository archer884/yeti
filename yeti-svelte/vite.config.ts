import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'

// https://vite.dev/config/
export default defineConfig({
  // Mounted under /author in Yeti.Web (see Yeti.Web/Program.cs MapFallbackToFile).
  base: '/author/',
  plugins: [svelte()],
  build: {
    // Emit the production build straight into Yeti.Web so it's served at /author.
    outDir: '../Yeti.Web/wwwroot/author',
    emptyOutDir: true,
  },
  server: {
    // Same-origin /api requests are proxied to the locally-run API, mirroring the Caddy
    // edge in production (which strips /api). Keeps the SPA CORS-free in dev too.
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
    },
  },
})
