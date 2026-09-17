import * as React from "react";
import RequireAuth from "../../src/components/RequireAuth";
import AppShell from "../../src/components/AppShell";

/**
 * چیدمان مشترک صفحات محافظت‌شده: احراز هویت + پوسته برنامه (سایدبار/هدر).
 */
export default function ProtectedLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <RequireAuth>
      <AppShell>{children}</AppShell>
    </RequireAuth>
  );
}
