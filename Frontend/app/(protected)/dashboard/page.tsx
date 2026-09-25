"use client";

import * as React from "react";
import Link from "next/link";
import { Box, Paper, Typography } from "@mui/material";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import WorkIcon from "@mui/icons-material/Work";
import PeopleIcon from "@mui/icons-material/People";
import AssignmentIcon from "@mui/icons-material/Assignment";
import VerifiedUserIcon from "@mui/icons-material/VerifiedUser";
import DashboardIcon from "@mui/icons-material/Dashboard";
import { useMe } from "../../../src/features/auth/useAuth";
import { displayUserName } from "../../../src/lib/api/auth";

/**
 * دسترسی سریع داشبورد — اتصال بصری به تمام صفحات منو.
 */
const quickActions = [
  { label: "چارت سازمانی", path: "/org-chart", icon: AccountTreeIcon },
  { label: "پست‌ها", path: "/posts", icon: WorkIcon },
  { label: "پرسنل", path: "/employees", icon: PeopleIcon },
  { label: "مسئولیت‌ها", path: "/responsibilities", icon: AssignmentIcon },
  { label: "حق امضاها", path: "/authorities", icon: VerifiedUserIcon },
  { label: "داشبورد", path: "/dashboard", icon: DashboardIcon },
] as const;

/**
 * داشبورد (محافظت‌شده) — چیدمان مطابق قالب مرجع SampleAdminPanel.
 */
export default function DashboardPage() {
  return <DashboardContent />;
}

function DashboardContent() {
  const { data: user } = useMe();

  return (
    <Box>
      <Box sx={{ mb: 4 }}>
        <Typography variant="body2" color="text.secondary">
          مدیریت و کنترل
        </Typography>
        <Typography variant="h5" component="h2" sx={{ mt: 0.5 }}>
          داشبورد
        </Typography>
        <Typography
          variant="body2"
          color="text.secondary"
          sx={{ mt: 1, fontSize: 12 }}
        >
          نمای کلی سامانه ساختار سازمانی و دسترسی سریع به بخش‌ها
        </Typography>
      </Box>

      <Box
        sx={{
          display: "grid",
          gap: 3,
          gridTemplateColumns: { xs: "minmax(0, 1fr)", lg: "minmax(0, 2fr) minmax(0, 1fr)" },
        }}
      >
        <Paper sx={{ p: { xs: 2.5, sm: 4 } }}>
          <Typography variant="subtitle1" component="h3">
            خوش آمدید
          </Typography>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mt: 1, fontSize: 12 }}
          >
            اطلاعات نشست جاری شما (داده واقعی سامانه):
          </Typography>
          <Box
            sx={{
              mt: 2.5,
              display: "grid",
              gap: 1.5,
              gridTemplateColumns: { xs: "minmax(0, 1fr)", sm: "repeat(2, minmax(0, 1fr))" },
            }}
          >
            <InfoRow label="نام و نام خانوادگی" value={displayUserName(user)} />
            <InfoRow label="شناسه کاربر" value={user?.userId ?? "—"} ltr />
            <InfoRow label="مستأجر" value={user?.tenantId ?? "—"} ltr />
            <InfoRow label="سازمان" value={user?.organizationId ?? "—"} ltr />
            <InfoRow
              label="نقش‌ها"
              value={user?.roles?.length ? user.roles.join("، ") : "—"}
            />
          </Box>
        </Paper>

        <Paper sx={{ p: { xs: 2.5, sm: 3 } }}>
          <Typography variant="subtitle1" component="h3">
            دسترسی سریع
          </Typography>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mt: 0.5, fontSize: 12 }}
          >
            کارهای پرکاربرد خود را سریع انجام دهید
          </Typography>
          <Box
            sx={{
              mt: 2.5,
              display: "grid",
              gridTemplateColumns: { xs: "minmax(0, 1fr)", sm: "repeat(2, minmax(0, 1fr))" },
              gap: 1.5,
            }}
          >
            {quickActions.map(({ label, path, icon: Icon }) => (
              <Box
                key={path}
                component={Link}
                href={path}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 1.25,
                  p: 1.5,
                  borderRadius: 3,
                  border: 1,
                  borderColor: "divider",
                  textDecoration: "none",
                  fontSize: 12,
                  fontWeight: 700,
                  color: "text.primary",
                  transition: "all 0.2s ease",
                  "&:hover": {
                    borderColor: "primary.light",
                    bgcolor: "rgba(10, 139, 123, 0.06)",
                    color: "primary.dark",
                  },
                }}
              >
                <Icon fontSize="small" sx={{ color: "primary.main" }} />
                {label}
              </Box>
            ))}
          </Box>
        </Paper>
      </Box>
    </Box>
  );
}

function InfoRow({
  label,
  value,
  ltr = false,
}: {
  label: string;
  value: string;
  ltr?: boolean;
}) {
  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        gap: 1,
        px: 1.5,
        py: 1,
        borderRadius: 2,
        bgcolor: (theme) =>
          theme.palette.mode === "dark"
            ? "rgba(255, 255, 255, 0.05)"
            : "rgba(23, 36, 44, 0.03)",
      }}
    >
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography
        variant="caption"
        sx={{ fontWeight: 700, direction: ltr ? "ltr" : undefined, fontSize: 11 }}
        noWrap
      >
        {value}
      </Typography>
    </Box>
  );
}
