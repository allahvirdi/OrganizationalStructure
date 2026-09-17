"use client";

import { Box, Button, Container, Typography } from "@mui/material";
import { useLogout, useMe } from "../../../src/features/auth/useAuth";

/**
 * داشبورد (محافظت‌شده).
 */
export default function DashboardPage() {
  return (
    <DashboardContent />
  );
}

function DashboardContent() {
  const { data: user } = useMe();
  const logout = useLogout();

  return (
    <Container maxWidth="md">
      <Box sx={{ py: 6 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
          داشبورد
        </Typography>
        <Typography variant="body2" color="text.secondary" gutterBottom>
          شناسه کاربر: {user?.userId ?? "—"}
        </Typography>
        <Button
          variant="outlined"
          onClick={() => logout.mutate()}
          disabled={logout.isPending}
        >
          خروج
        </Button>
      </Box>
    </Container>
  );
}
