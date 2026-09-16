import { createTheme, type PaletteMode } from "@mui/material/styles";

/**
 * پالت‌های استخراج‌شده از SampleAdminPanel (مرجع بصری/UX).
 */
const ink = {
  50: "#f6f8fa",
  100: "#e9eef2",
  200: "#d7e0e7",
  300: "#b9c8d3",
  400: "#8ea3b2",
  500: "#617889",
  600: "#486070",
  700: "#384b59",
  800: "#293944",
  900: "#17242c",
};

const brand = {
  50: "#ecfdf9",
  100: "#d0f9ef",
  200: "#a3f1df",
  300: "#6be2cb",
  400: "#30c9ae",
  500: "#13ad96",
  600: "#0a8b7b",
  700: "#0b7065",
  800: "#0d594f",
  900: "#0e4a43",
};

const secondaryScale = {
  50: "#eff6ff",
  100: "#dbeafe",
  200: "#bfdbfe",
  300: "#93c5fd",
  400: "#60a5fa",
  500: "#3b82f6",
  600: "#2563eb",
  700: "#1d4ed8",
  800: "#1e40af",
  900: "#1e3a8a",
};

/**
 * ساخت تم MUI همسو با قالب (راست‌به‌چپ + وزیرمتن + روشن/تیره).
 */
export function createAppTheme(mode: PaletteMode) {
  const isDark = mode === "dark";
  return createTheme({
    direction: "rtl",
    palette: {
      mode,
      primary: {
        main: brand[600],
        dark: brand[700],
        light: brand[300],
        contrastText: "#ffffff",
      },
      secondary: {
        main: secondaryScale[600],
        contrastText: "#ffffff",
      },
      background: {
        default: isDark ? "#111b22" : "#f7f9fb",
        paper: isDark ? "#18252d" : "#ffffff",
      },
      text: {
        primary: isDark ? "#ffffff" : ink[900],
        secondary: isDark ? ink[300] : ink[500],
      },
      divider: isDark ? "rgba(255,255,255,0.1)" : ink[100],
    },
    typography: {
      fontFamily: '"Vazirmatn", "Segoe UI", Tahoma, sans-serif',
    },
    shape: {
      borderRadius: 12,
    },
    components: {
      MuiCssBaseline: {
        styleOverrides: {
          body: {
            fontFamily: '"Vazirmatn", "Segoe UI", Tahoma, sans-serif',
          },
        },
      },
    },
  });
}
