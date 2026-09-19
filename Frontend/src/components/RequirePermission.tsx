"use client";

import * as React from "react";
import { Alert, Box, CircularProgress } from "@mui/material";
import { useMe } from "../features/auth/useAuth";
import { hasPermission } from "../lib/permissions";

/**
 * نگهبان دسترسی: فقط با داشتن Permission مشخص محتوا را نشان می‌دهد.
 * (تصمیم نمایشی؛ enforce اصلی سمت سرور است)
 */
export default function RequirePermission({
  permission,
  children,
  fallback,
}: {
  permission: string;
  children: React.ReactNode;
  fallback?: React.ReactNode;
}) {
  const { data: user, isLoading } = useMe();

  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!hasPermission(user, permission)) {
    return (
      <>
        {fallback !== undefined ? fallback : (
          <Alert severity="warning">برای این بخش دسترسی ندارید.</Alert>
        )}
      </>
    );
  }

  return <>{children}</>;
}
