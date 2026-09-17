"use client";

import * as React from "react";
import { useRouter } from "next/navigation";
import { Box, CircularProgress } from "@mui/material";
import { useMe } from "../features/auth/useAuth";

/**
 * نگهبان مسیر: بدون احراز به صفحه ورود هدایت می‌کند.
 * همچنین بررسی می‌کند که کاربر دارای شناسه معتبر باشد (userId).
 * این برای مقابله با APIهایی که به‌جای ۴۰۱، ۲۰۰ با خطا برمی‌گردانند.
 */
export default function RequireAuth({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const { data: user, isLoading, isError } = useMe();

  React.useEffect(() => {
    // کاربر معتبر باید userId داشته باشد
    const isAuthenticated = !isLoading && !isError && user?.userId;
    if (!isLoading && !isAuthenticated) {
      router.replace("/login");
    }
  }, [isLoading, isError, user, router]);

  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  // کاربر معتبر باید userId داشته باشد
  const isAuthenticated = !isLoading && !isError && !!user?.userId;

  if (!isAuthenticated) {
    return null;
  }

  return <>{children}</>;
}
