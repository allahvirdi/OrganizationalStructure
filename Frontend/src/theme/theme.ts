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
 * فونت رسمی قالب مرجع.
 */
const fontStack = '"Vazirmatn", "Segoe UI", Tahoma, sans-serif';

/**
 * سایه کارت قالب (boxShadow.card).
 */
const shadowCard = "0 8px 30px rgba(31, 49, 61, 0.06)";

/**
 * سایه شناور قالب (boxShadow.float).
 */
const shadowFloat = "0 18px 50px rgba(31, 49, 61, 0.12)";

/**
 * ساخت تم MUI همسو با قالب (راست‌به‌چپ + وزیرمتن + روشن/تیره).
 */
export function createAppTheme(mode: PaletteMode) {
  const isDark = mode === "dark";
  const line = isDark ? "rgba(255, 255, 255, 0.1)" : ink[100];
  return createTheme({
    direction: "rtl",
    breakpoints: { values: { xs: 0, sm: 640, md: 768, lg: 1024, xl: 1280 } },
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
      divider: line,
    },
    typography: {
      fontFamily: fontStack,
      button: { fontFamily: fontStack, fontWeight: 700, textTransform: "none" },
      body1: { fontWeight: 400 },
      body2: { fontWeight: 500 },
      subtitle1: { fontWeight: 700 },
      subtitle2: { fontWeight: 700 },
      h1: { fontWeight: 800 },
      h2: { fontWeight: 800 },
      h3: { fontWeight: 700 },
      h4: { fontWeight: 700 },
      h5: { fontWeight: 700 },
      h6: { fontWeight: 700 },
    },
    shape: {
      // sx numeric radii use this multiplier; component overrides below use pixels.
      borderRadius: 4,
    },
    components: {
      MuiDrawer: {
        styleOverrides: {
          paper: { borderRadius: 0, boxShadow: "none", border: 0, borderInlineEnd: `1px solid ${line}` },
        },
      },
      MuiAppBar: {
        styleOverrides: { root: { borderRadius: 0, boxShadow: "none" } },
      },
      MuiMenu: {
        styleOverrides: { paper: { boxShadow: shadowFloat } },
      },
      MuiTextField: { defaultProps: { size: "small" } },
      MuiListItemText: {
        styleOverrides: { primary: { fontSize: 14, fontWeight: "inherit" } },
      },
      /**
       * سطح کارت قالب (.surface): گوشی ۱۶ + حاشیه + سایه card.
       */
      MuiTableCell: {
        styleOverrides: {
          head: {
            fontWeight: 700,
            whiteSpace: "nowrap",
          },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: {
            borderRadius: 16,
            border: "1px solid",
            borderColor: line,
            backgroundImage: "none",
            boxShadow: shadowCard,
          },
        },
      },
      /**
       * دکمه قالب: ارتفاع ۴۴px، گوشی ۱۲، برند-۶۰۰ با هاور برند-۷۰۰.
       */
      MuiButton: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            minHeight: 44,
            paddingInline: 20,
            fontSize: 13,
            "&.MuiButton-contained.MuiButton-colorPrimary": {
              backgroundColor: brand[600],
              boxShadow: "0 10px 24px rgba(10, 139, 123, 0.22)",
              "&:hover": { backgroundColor: brand[700] },
            },
          },
        },
      },
      /**
       * ورودی قالب (.input): ارتفاع ۴۴px، گوشی ۱۲، فوکوس برند-۵۰۰ با حلقه ۴px.
       */
      MuiOutlinedInput: {
        styleOverrides: {
          root: {
            height: 44,
            borderRadius: 12,
            backgroundColor: isDark ? "rgba(255,255,255,0.05)" : "#ffffff",
            "& fieldset": { borderColor: isDark ? "rgba(255,255,255,0.1)" : ink[200] },
            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: isDark ? "rgba(255,255,255,0.16)" : ink[300],
            },
            "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
              borderColor: brand[500],
              borderWidth: 1,
              boxShadow: "0 0 0 4px rgba(19, 173, 150, 0.10)",
            },
          },
        },
      },
      /**
       * آیتم منوی سایدبار قالب: انتخاب‌شده = برند-۵۰/برند-۷۰۰ (تیره: برند ۱۵٪/۳۰۰).
       */
      MuiListItemButton: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            minHeight: 44,
            paddingInline: 14,
            color: isDark ? ink[300] : ink[500],
            fontWeight: 600,
            "&:hover": {
              backgroundColor: isDark ? "rgba(255,255,255,0.05)" : ink[50],
              color: isDark ? "#ffffff" : ink[800],
            },
            "&.Mui-selected": {
              backgroundColor: isDark ? "rgba(19, 173, 150, 0.15)" : brand[50],
              color: isDark ? brand[300] : brand[700],
              fontWeight: 700,
              "&:hover": {
                backgroundColor: isDark ? "rgba(19, 173, 150, 0.18)" : brand[100],
              },
              "& .MuiListItemIcon-root": { color: isDark ? brand[300] : brand[600] },
            },
          },
        },
      },
      /**
       * آیکن دکمه (icon-button قالب): مربع ۴۰ با گوشی ۱۲ و هاور برند.
       */
      MuiIconButton: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            color: isDark ? ink[300] : ink[500],
            "&:hover": {
              backgroundColor: isDark ? "rgba(255,255,255,0.1)" : ink[50],
              color: isDark ? brand[300] : brand[600],
            },
          },
        },
      },
      /**
       * اسکرولبار باریک قالب (۶px) با رنگ ink قالب.
       */
      MuiCssBaseline: {
        styleOverrides: {
          html: {
            backgroundColor: isDark ? "#111b22" : "#f7f9fb",
          },
          body: {
            fontFamily: fontStack,
          },
          "::-webkit-scrollbar": {
            width: 6,
            height: 6,
          },
          "::-webkit-scrollbar-thumb": {
            backgroundColor: isDark ? ink[600] : ink[300],
            borderRadius: 999,
          },
        },
      },
    },
  });
}
