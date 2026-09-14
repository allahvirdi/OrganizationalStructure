/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    extend: {
      fontFamily: {
        sans: ['Vazirmatn', 'Tahoma', 'sans-serif'],
      },
      colors: {
        ink: {
          50: '#f6f8fa', 100: '#e9eef2', 200: '#d7e0e7', 300: '#b9c8d3',
          400: '#8ea3b2', 500: '#617889', 600: '#486070', 700: '#384b59',
          800: '#293944', 900: '#17242c',
        },
        brand: {
          50: '#ecfdf9', 100: '#d0f9ef', 200: '#a3f1df', 300: '#6be2cb',
          400: '#30c9ae', 500: '#13ad96', 600: '#0a8b7b', 700: '#0b7065',
          800: '#0d594f', 900: '#0e4a43',
        },
        secondary: {
          50: '#eff6ff', 100: '#dbeafe', 200: '#bfdbfe', 300: '#93c5fd',
          400: '#60a5fa', 500: '#3b82f6', 600: '#2563eb', 700: '#1d4ed8',
          800: '#1e40af', 900: '#1e3a8a',
        },
      },
      boxShadow: {
        card: '0 8px 30px rgba(31, 49, 61, 0.06)',
        float: '0 18px 50px rgba(31, 49, 61, 0.12)',
      },
    },
  },
  plugins: [],
};
