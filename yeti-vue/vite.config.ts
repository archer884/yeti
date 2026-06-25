import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  // Mounted under /author in Yeti.Web (see Yeti.Web/Program.cs MapFallbackToFile).
  base: '/author/',
  plugins: [
    vue(),
    vueJsx(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  build: {
    // Emit the production build straight into Yeti.Web so it's served at /author.
    outDir: '../Yeti.Web/wwwroot/author',
    emptyOutDir: true,
  },
})
