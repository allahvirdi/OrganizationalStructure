import { apiFetch } from "../../lib/api/client";

/** اختیار از API. */
export interface Authority {
  id: string;
  code: string;
  title: string;
  description?: string | null;
  isActive: boolean;
}

/** انتساب اختیار. */
export interface AuthorityAssignment {
  id: string;
  authorityId: string;
  authorityCode: string;
  authorityTitle: string;
  organizationId: string;
  postId: string;
  postCode: string;
  postTitle: string;
  startDate?: string | null;
  endDate?: string | null;
  isActive: boolean;
}

/** نتیجه صفحه‌بندی‌شده. */
export interface PagedAuthorities {
  items: Authority[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * فهرست اختیارها.
 */
export function fetchAuthorities(input: {
  searchTerm?: string;
  isActive?: boolean;
  page: number;
  pageSize: number;
}): Promise<PagedAuthorities> {
  return apiFetch<PagedAuthorities>("/api/v1/authorities", {
    query: {
      searchTerm: input.searchTerm || undefined,
      isActive: input.isActive,
      page: input.page,
      pageSize: input.pageSize,
    },
  });
}

/**
 * دریافت اختیار با کد.
 */
export function fetchAuthority(code: string): Promise<Authority> {
  return apiFetch<Authority>(
    `/api/v1/authorities/${encodeURIComponent(code)}`,
  );
}

/**
 * تعریف حق امضا (اختیار سازمانی) — کد به‌صورت خودکار در بک‌اند تولید می‌شود (GUID).
 */
export function createAuthority(input: {
  title: string;
  description?: string | null;
}): Promise<string> {
  return apiFetch<string>("/api/v1/authorities", {
    method: "POST",
    body: input,
  });
}

/**
 * ویرایش اختیار.
 */
export function updateAuthority(
  id: string,
  input: { title: string; description?: string | null },
): Promise<void> {
  return apiFetch<void>(`/api/v1/authorities/${id}`, {
    method: "PUT",
    body: input,
  });
}

/**
 * غیرفعال‌سازی اختیار.
 */
export function disableAuthority(id: string): Promise<void> {
  return apiFetch<void>(`/api/v1/authorities/${id}/disable`, {
    method: "PATCH",
  });
}

/**
 * انتساب به پست.
 */
export function assignAuthority(input: {
  authorityCode: string;
  postId: string;
  startDate?: string | null;
  endDate?: string | null;
}): Promise<string> {
  return apiFetch<string>("/api/v1/authorities/assignments", {
    method: "POST",
    body: input,
  });
}

/**
 * پایان انتساب.
 */
export function endAuthorityAssignment(
  assignmentId: string,
  endDate: string,
): Promise<void> {
  return apiFetch<void>(
    `/api/v1/authorities/assignments/${assignmentId}/end`,
    { method: "POST", body: { endDate } },
  );
}

/**
 * اختیارهای یک پست.
 */
export function fetchPostAuthorities(
  postId: string,
): Promise<AuthorityAssignment[]> {
  return apiFetch<AuthorityAssignment[]>(
    `/api/v1/posts/${postId}/authorities`,
  );
}

/**
 * انتساب‌های یک اختیار.
 */
export function fetchAuthorityAssignments(
  code: string,
): Promise<AuthorityAssignment[]> {
  return apiFetch<AuthorityAssignment[]>(
    `/api/v1/authorities/${encodeURIComponent(code)}/assignments`,
  );
}
