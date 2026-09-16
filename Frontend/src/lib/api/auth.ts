import { apiFetch } from "./client";

/** نتیجه ورود (مرحله اول یا نهایی). */
export interface LoginResult {
  requiresMfa: boolean;
  mfaChallengeToken?: string | null;
  userId?: string | null;
  expiresAt?: string | null;
}

/** اطلاعات کاربر جاری. */
export interface CurrentUser {
  userId?: string | null;
  tenantId?: string | null;
  organizationId?: string | null;
  roles: string[];
}

/**
 * ورود با نام کاربری و رمز عبور.
 */
export function login(input: {
  userName: string;
  password: string;
  rememberMe: boolean;
}): Promise<LoginResult> {
  return apiFetch<LoginResult>("/api/v1/auth/login", {
    method: "POST",
    body: input,
  });
}

/**
 * تکمیل ورود دومرحله‌ای.
 */
export function verifyMfa(input: {
  mfaChallengeToken: string;
  code: string;
  rememberMe: boolean;
}): Promise<LoginResult> {
  return apiFetch<LoginResult>("/api/v1/auth/verify-mfa", {
    method: "POST",
    body: input,
  });
}

/**
 * خروج (ابطال نشست سمت سرور + حذف کوکی).
 */
export function logout(): Promise<void> {
  return apiFetch<void>("/api/v1/auth/logout", { method: "POST" });
}

/**
 * دریافت کاربر جاری (401 در صورت عدم احراز).
 */
export function fetchMe(): Promise<CurrentUser> {
  return apiFetch<CurrentUser>("/api/v1/auth/me");
}
