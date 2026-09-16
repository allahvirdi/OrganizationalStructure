import { createTheme } from "@mui/material/styles";

/**
 * تم سراسری MUI (راست‌به‌چپ + فونت وزیرمتن).
 */
const theme = createTheme({
  direction: "rtl",
  typography: {
    fontFamily: '"Vazirmatn", "Segoe UI", Tahoma, sans-serif',
  },
  palette: {
    primary: {
      main: "#1565c0",
    },
    secondary: {
      main: "#6a1b9a",
    },
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

export default theme;
