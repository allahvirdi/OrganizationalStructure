"use client";

import { useMe } from "../auth/useAuth";

/** نام Permission مشاهده داده حساس. */
export const VIEW_SENSITIVE_DATA = "OrganizationStructure.Employee.ViewSensitiveData";

/**
 * آیا کاربر جاری مجوز مشاهده داده حساس را دارد؟
 * (تصمیم نمایشی؛ enforce اصلی سمت سرور است)
 */
export function useCanViewSensitiveData(): boolean {
  const { data: user } = useMe();
  return user?.roles?.includes("SystemAdmin") === true || user?.permissions?.includes(VIEW_SENSITIVE_DATA) === true;
}

/**
 * ماسک مقدار حساس (نمایش ••• در صورت نبود مجوز).
 */
export function maskSensitive(value: string | null | undefined, canView: boolean): string {
  if (!value) {
    return "—";
  }
  return canView ? value : "•••";
}
