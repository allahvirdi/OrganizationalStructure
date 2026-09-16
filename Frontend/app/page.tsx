import { Box, Container, Typography } from "@mui/material";

export default function HomePage() {
  return (
    <Container maxWidth="md">
      <Box sx={{ py: 8, textAlign: "center" }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }} gutterBottom>
          سامانه ساختار سازمانی
        </Typography>
        <Typography variant="body1" color="text.secondary">
          هسته مرجع مدیریت ساختار سازمانی، پست‌ها، پرسنل، مسئولیت‌ها و اختیارها
        </Typography>
      </Box>
    </Container>
  );
}
