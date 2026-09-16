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
  Container,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useLogin } from "../../src/features/auth/useAuth";
import { ApiError } from "../../src/lib/api/client";

const loginSchema = z.object({
  userName: z.string().min(1, "نام کاربری الزامی است."),
  password: z.string().min(1, "رمز عبور الزامی است."),
});

type LoginForm = z.infer<typeof loginSchema>;

/**
 * صفحه ورود (BFF — کوکی HttpOnly سمت سرور تنظیم می‌شود).
 */
export default function LoginPage() {
  const router = useRouter();
  const login = useLogin();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({ resolver: zodResolver(loginSchema) });

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

  return (
    <Container maxWidth="xs">
      <Box sx={{ py: 10 }}>
        <Paper sx={{ p: 4 }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
            ورود به سامانه
          </Typography>
          <Typography variant="body2" color="text.secondary" gutterBottom>
            سامانه ساختار سازمانی
          </Typography>
          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            sx={{ mt: 2, display: "grid", gap: 2 }}
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
              ورود
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}
