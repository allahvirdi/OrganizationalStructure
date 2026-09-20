import { apiFetch } from "../../lib/api/client";

/** پرسنل از API. */
export interface Employee {
  id: string;
  organizationId: string;
  organizationName?: string | null;
  personnelCode: string;
  firstName: string;
  lastName: string;
  nationalCode: string;
  mobile?: string | null;
  birthDate?: string | null;
  serviceYears?: number | null;
  serviceMonths?: number | null;
  pezhvakMobile?: string | null;
  pezhvakIsActive?: boolean | null;
  userId?: string | null;
  isActive: boolean;
}

/** انتساب پرسنل به پست. */
export interface EmployeePostAssignment {
  postId: string;
  postCode: string;
  postTitle: string;
  fromDate?: string | null;
  toDate?: string | null;
  isPrimary: boolean;
}

/** نتیجه صفحه‌بندی‌شده. */
export interface PagedEmployees {
  items: Employee[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * فهرست پرسنل با فیلترهای پیشرفته (کد پرسنلی، کد ملی، سازمان).
 */
export function fetchEmployees(input: {
  searchTerm?: string;
  personnelCode?: string;
  nationalCode?: string;
  organizationId?: string;
  isActive?: boolean;
  page: number;
  pageSize: number;
}): Promise<PagedEmployees> {
  return apiFetch<PagedEmployees>("/api/v1/employees", {
    query: {
      searchTerm: input.searchTerm || undefined,
      personnelCode: input.personnelCode || undefined,
      nationalCode: input.nationalCode || undefined,
      organizationId: input.organizationId || undefined,
      isActive: input.isActive,
      page: input.page,
      pageSize: input.pageSize,
    },
  });
}

/**
 * دریافت پرسنل.
 */
export function fetchEmployee(id: string): Promise<Employee> {
  return apiFetch<Employee>(`/api/v1/employees/${id}`);
}

/**
 * ثبت پرسنل (سازمان اجباری است).
 */
export function createEmployee(input: {
  organizationId: string;
  personnelCode: string;
  firstName: string;
  lastName: string;
  nationalCode: string;
  mobile: string;
  birthDate?: string | null;
  serviceYears?: number | null;
  serviceMonths?: number | null;
  pezhvakMobile?: string | null;
}): Promise<string> {
  return apiFetch<string>("/api/v1/employees", {
    method: "POST",
    body: { ...input, userId: null },
  });
}

/**
 * ویرایش اطلاعات پرسنلی.
 */
export function updateEmployee(
  id: string,
  input: {
    firstName: string;
    lastName: string;
    nationalCode: string;
    mobile: string;
  },
): Promise<void> {
  return apiFetch<void>(`/api/v1/employees/${id}`, {
    method: "PUT",
    body: input,
  });
}

/**
 * ویرایش اطلاعات تکمیلی.
 */
export function updateSupplementary(
  id: string,
  input: {
    birthDate?: string | null;
    serviceYears?: number | null;
    serviceMonths?: number | null;
    pezhvakMobile?: string | null;
    pezhvakIsActive?: boolean | null;
  },
): Promise<void> {
  return apiFetch<void>(`/api/v1/employees/${id}/supplementary`, {
    method: "PATCH",
    body: input,
  });
}

/**
 * انتساب به پست.
 */
export function assignPost(
  id: string,
  input: {
    postId: string;
    fromDate?: string | null;
    toDate?: string | null;
    isPrimary: boolean;
  },
): Promise<void> {
  return apiFetch<void>(`/api/v1/employees/${id}/posts`, {
    method: "POST",
    body: input,
  });
}

/**
 * پایان انتساب.
 */
export function endAssignment(
  id: string,
  postId: string,
  endDate: string,
): Promise<void> {
  return apiFetch<void>(`/api/v1/employees/${id}/posts/${postId}`, {
    method: "DELETE",
    body: { endDate },
  });
}

/**
 * تعیین وضعیت.
 */
export function setEmployeeStatus(
  id: string,
  isActive: boolean,
): Promise<void> {
  return apiFetch<void>(`/api/v1/employees/${id}/status`, {
    method: "PATCH",
    body: { isActive },
  });
}

/**
 * پست‌های پرسنل.
 */
export function fetchEmployeePosts(
  id: string,
): Promise<EmployeePostAssignment[]> {
  return apiFetch<EmployeePostAssignment[]>(`/api/v1/employees/${id}/posts`);
}
