import { apiFetch } from "../../lib/api/client";

/** مسئولیت از API. */
export interface Responsibility {
  id: string;
  code: string;
  title: string;
  description?: string | null;
  isActive: boolean;
}

/** انتساب مسئولیت. */
export interface ResponsibilityAssignment {
  id: string;
  responsibilityId: string;
  responsibilityCode: string;
  responsibilityTitle: string;
  organizationId: string;
  postId: string;
  postCode: string;
  postTitle: string;
  startDate?: string | null;
  endDate?: string | null;
  isActive: boolean;
}

/** نتیجه صفحه‌بندی‌شده. */
export interface PagedResponsibilities {
  items: Responsibility[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * فهرست مسئولیت‌ها.
 */
export function fetchResponsibilities(input: {
  searchTerm?: string;
  isActive?: boolean;
  page: number;
  pageSize: number;
}): Promise<PagedResponsibilities> {
  return apiFetch<PagedResponsibilities>("/api/v1/responsibilities", {
    query: {
      searchTerm: input.searchTerm || undefined,
      isActive: input.isActive,
      page: input.page,
      pageSize: input.pageSize,
    },
  });
}

/**
 * دریافت مسئولیت با کد.
 */
export function fetchResponsibility(code: string): Promise<Responsibility> {
  return apiFetch<Responsibility>(
    `/api/v1/responsibilities/${encodeURIComponent(code)}`,
  );
}

/**
 * تعریف مسئولیت.
 */
export function createResponsibility(input: {
  code: string;
  title: string;
  description?: string | null;
}): Promise<string> {
  return apiFetch<string>("/api/v1/responsibilities", {
    method: "POST",
    body: input,
  });
}

/**
 * ویرایش مسئولیت.
 */
export function updateResponsibility(
  id: string,
  input: { title: string; description?: string | null },
): Promise<void> {
  return apiFetch<void>(`/api/v1/responsibilities/${id}`, {
    method: "PUT",
    body: input,
  });
}

/**
 * غیرفعال‌سازی مسئولیت.
 */
export function disableResponsibility(id: string): Promise<void> {
  return apiFetch<void>(`/api/v1/responsibilities/${id}/disable`, {
    method: "PATCH",
  });
}

/**
 * انتساب به پست.
 */
export function assignResponsibility(input: {
  responsibilityCode: string;
  postId: string;
  startDate?: string | null;
  endDate?: string | null;
}): Promise<string> {
  return apiFetch<string>("/api/v1/responsibilities/assignments", {
    method: "POST",
    body: input,
  });
}

/**
 * پایان انتساب.
 */
export function endResponsibilityAssignment(
  assignmentId: string,
  endDate: string,
): Promise<void> {
  return apiFetch<void>(
    `/api/v1/responsibilities/assignments/${assignmentId}/end`,
    { method: "POST", body: { endDate } },
  );
}

/**
 * مسئولیت‌های یک پست.
 */
export function fetchPostResponsibilities(
  postId: string,
): Promise<ResponsibilityAssignment[]> {
  return apiFetch<ResponsibilityAssignment[]>(
    `/api/v1/posts/${postId}/responsibilities`,
  );
}

/**
 * انتساب‌های یک مسئولیت.
 */
export function fetchResponsibilityAssignments(
  code: string,
): Promise<ResponsibilityAssignment[]> {
  return apiFetch<ResponsibilityAssignment[]>(
    `/api/v1/responsibilities/${encodeURIComponent(code)}/assignments`,
  );
}
