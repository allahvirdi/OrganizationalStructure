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
  /** نام (Claim اختیاری first_name توکن IAM؛ ممکن است برای نشست‌های قدیمی خالی باشد). */
  firstName?: string | null;
  /** نام خانوادگی (Claim اختیاری last_name توکن IAM؛ ممکن است برای نشست‌های قدیمی خالی باشد). */
  lastName?: string | null;
  roles: string[];
  permissions?: string[] | null;
}

/**
 * نام نمایشی کاربر جاری: «نام نام‌خانوادگی»؛ در نبود آن، userId کوتاه‌شده.
 */
export function displayUserName(user: CurrentUser | null | undefined): string {
  const full = `${user?.firstName ?? ""} ${user?.lastName ?? ""}`.trim();
  if (full) return full;
  return user?.userId ? user.userId.slice(0, 8) : "—";
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
 * دریافت کاربر جاری.
 *
 * نبودِ نشست خطا نیست: سرور در این حالت `204 No Content` برمی‌گرداند و `null` برگردانده می‌شود
 * (تمایز «ناشناس» از «خطای واقعی» بدون خطای ۴۰۱ در کنسول مرورگر).
 */
export function fetchMe(): Promise<CurrentUser | null> {
  return apiFetch<CurrentUser | null>("/api/v1/auth/me").then(
    (user) => user ?? null,
  );
}
