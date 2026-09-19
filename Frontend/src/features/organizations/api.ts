import { apiFetch } from "../../lib/api/client";

/** گزینه سازمان قابل انتخاب کاربر (خود سازمان + زیرمجموعه‌ها). */
export interface OrganizationOption {
  id: string;
  name: string;
  code: string;
  parentId: string | null;
  depth: number;
  isCurrent: boolean;
}

/**
 * فهرست سازمان‌های قابل انتخاب کاربر جاری با جستجوی نام/کد.
 *
 * سرور خروجی را به محدوده سازمانی کاربر (خود سازمان + زیرمجموعه‌ها) محدود می‌کند.
 */
export function fetchOrganizations(input?: {
  searchTerm?: string;
}): Promise<OrganizationOption[]> {
  return apiFetch<OrganizationOption[]>("/api/v1/organizations", {
    query: { searchTerm: input?.searchTerm || undefined },
  });
}