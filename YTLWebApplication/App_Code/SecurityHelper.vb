tem",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "server": "node server/index.js",
    "start": "concurrently \"npm run server\" \"npm run dev\""
  },
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.8.0",
    "axios": "^1.6.0",
    "bcryptjs": "^2.4.3",
    "jsonwebtoken": "^9.0.2",
    "express": "^4.18.2",
    "express-rate-limit": "^7.1.5",
    "helmet": "^7.1.0",
    "cors": "^2.8.5",
    "dotenv": "^16.3.1",
    "sqlite3": "^5.1.6",
    "express-validator": "^7.0.1",
    "cookie-parser": "^1.4.6",
    "express-session": "^1.17.3",
    "connect-sqlite3": "^0.9.13",
    "concurrently": "^8.2.2",
    "leaflet": "^1.9.4",
    "react-leaflet": "^4.2.1",
    "lucide-react": "^0.294.0",
    "clsx": "^2.0.0",
    "tailwind-merge": "^2.0.0"
  },
  "devDependencies": {
    "@types/react": "^18.2.43",
    "@types/react-dom": "^18.2.17",
    "@vitejs/plugin-react": "^4.2.1",
    "vite": "^5.0.8",
    "tailwindcss": "^3.3.6",
    "autoprefixer": "^10.4.16",
    "postcss": "^8.4.32"
  }
}