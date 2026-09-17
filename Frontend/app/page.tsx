"use client";

import * as React from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Box, Button, CircularProgress, Container, Typography } from "@mui/material";
import { useMe } from "../src/features/auth/useAuth";

/**
 * صفحه اول سامانه: در صورت ورود، به داشبورد هدایت می‌شود.
 */
export default function HomePage() {
  const router = useRouter();
  const { data: user, isLoading } = useMe();

  React.useEffect(() => {
    if (!isLoading && user?.userId) {
      router.replace("/dashboard");
    }
  }, [isLoading, user, router]);

  if (isLoading) {
    return (
      <Container maxWidth="md">
        <Box sx={{ py: 8, display: "flex", justifyContent: "center" }}>
          <CircularProgress />
        </Box>
      </Container>
    );
  }

  if (user?.userId) {
    return null;
  }

  return (
    <Container maxWidth="md">
      <Box sx={{ py: 8, textAlign: "center" }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }} gutterBottom>
          سامانه ساختار سازمانی
        </Typography>
        <Typography variant="body1" color="text.secondary" gutterBottom>
          هسته مرجع مدیریت ساختار سازمانی، پست‌ها، پرسنل، مسئولیت‌ها و اختیارها
        </Typography>
        <Button component={Link} href="/login" variant="contained" sx={{ mt: 2 }}>
          ورود به سامانه
        </Button>
      </Box>
    </Container>
  );
}
