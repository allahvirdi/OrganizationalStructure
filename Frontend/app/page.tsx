"use client";

import * as React from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import { useLogin, useMe } from "../src/features/auth/useAuth";
import { ApiError } from "../src/lib/api/client";

const loginSchema = z.object({
  userName: z.string().min(1, "نام کاربری الزامی است."),
  password: z.string().min(1, "رمز عبور الزامی است."),
});

type LoginForm = z.infer<typeof loginSchema>;

/**
 * صفحه لندینگ سامانه = صفحه ورود (الگوی BFF — کوکی HttpOnly سمت سرور).
 * کاربر دارای نشست معتبر (توکن سمت سرور) مستقیماً به داشبورد هدایت می‌شود.
 */
export default function LandingLoginPage() {
  const router = useRouter();
  const { data: user, isLoading: isAuthLoading } = useMe();
  const login = useLogin();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({ resolver: zodResolver(loginSchema) });

  const isAuthenticated = Boolean(user?.userId);

  React.useEffect(() => {
    if (!isAuthLoading && isAuthenticated) {
      router.replace("/dashboard");
    }
  }, [isAuthLoading, isAuthenticated, router]);

  const onSubmit = (values: LoginForm) => {
    login.mutate(
      { ...values, rememberMe: false },
      {
        onSuccess: (result) => {
          if (!result.requiresMfa) {
            router.replace("/dashboard");
          }
        },
      },
    );
  };

  const serverError =
    login.error instanceof ApiError ? login.error.message : null;

  if (isAuthLoading || isAuthenticated) {
    return (
      <Box
        sx={{
          minHeight: "100vh",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          bgcolor: "background.default",
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        p: 2,
        bgcolor: "background.default",
      }}
    >
      <Box sx={{ width: "100%", maxWidth: 424 }}>
        <Box sx={{ textAlign: "center", mb: 3 }}>
          <Box
            sx={{
              width: 56,
              height: 56,
              borderRadius: 4,
              mx: "auto",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              bgcolor: "primary.main",
              color: "#fff",
              boxShadow: "0 18px 50px rgba(31, 49, 61, 0.12)",
            }}
          >
            <AccountTreeIcon />
          </Box>
          <Typography variant="h5" component="h1" sx={{ mt: 2 }}>
            سامانه ساختار سازمانی
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
            هسته مرجع مدیریت پست‌ها، پرسنل، مسئولیت‌ها و اختیارها
          </Typography>
        </Box>
        <Paper sx={{ p: { xs: 3, sm: 4 } }}>
          <Typography variant="subtitle1" component="h2" gutterBottom>
            ورود به سامانه
          </Typography>
          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            noValidate
            sx={{ mt: 1, display: "grid", gap: 2 }}
          >
            <TextField
              label="نام کاربری"
              fullWidth
              error={Boolean(errors.userName)}
              helperText={errors.userName?.message}
              {...register("userName")}
            />
            <TextField
              label="رمز عبور"
              type="password"
              fullWidth
              error={Boolean(errors.password)}
              helperText={errors.password?.message}
              {...register("password")}
            />
            {serverError && <Alert severity="error">{serverError}</Alert>}
            <Button
              type="submit"
              variant="contained"
              disabled={login.isPending}
              fullWidth
            >
              {login.isPending ? "در حال ورود…" : "ورود"}
            </Button>
          </Box>
        </Paper>
      </Box>
    </Box>
  );
}
