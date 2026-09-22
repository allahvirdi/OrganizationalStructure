import { ApiError, apiFetch } from "../../lib/api/client";

/** نتیجه موفق بارگذاری ساختار سازمانی. */
export interface ImportPostsResult {
  organizationId: string;
  importedCount: number;
}

/**
 * بارگذاری ساختار پست‌های سازمان از فایل اکسل.
 *
 * درخواست به‌صورت multipart/form-data ارسال می‌شود.
 */
export async function importPosts(input: {
  organizationId: string;
  file: File;
}): Promise<ImportPostsResult> {
  const formData = new FormData();
  formData.append("organizationId", input.organizationId);
  formData.append("file", input.file);

  const response = await fetch("/api/v1/import/posts", {
    method: "POST",
    credentials: "include",
    body: formData,
  });

  if (!response.ok) {
    let detail = "خطایی رخ داده است.";
    let title: string | undefined;
    try {
      const payload = await response.json();
      detail = payload.detail ?? payload.title ?? detail;
      title = payload.title;
    } catch {
      // پاسخ JSON نبود
    }
    throw new ApiError(response.status, title, detail);
  }

  return response.json() as Promise<ImportPostsResult>;
}

/** آدرس دانلود قالب نمونه فایل اکسل (با هدر فارسی). */
export const POSTS_TEMPLATE_URL = "/api/v1/import/posts/template";

/** نتیجه موفق بارگذاری دسته‌جمعی پرسنل. */
export interface ImportEmployeesResult {
  organizationId: string;
  importedCount: number;
}

/**
 * بارگذاری دسته‌جمعی پرسنل سازمان از فایل اکسل یا CSV.
 *
 * درخواست به‌صورت multipart/form-data ارسال می‌شود.
 */
export async function importEmployees(input: {
  organizationId: string;
  file: File;
}): Promise<ImportEmployeesResult> {
  const formData = new FormData();
  formData.append("organizationId", input.organizationId);
  formData.append("file", input.file);

  const response = await fetch("/api/v1/import/employees", {
    method: "POST",
    credentials: "include",
    body: formData,
  });

  if (!response.ok) {
    let detail = "خطایی رخ داده است.";
    let title: string | undefined;
    try {
      const payload = await response.json();
      detail = payload.detail ?? payload.title ?? detail;
      title = payload.title;
    } catch {
      // پاسخ JSON نبود
    }
    throw new ApiError(response.status, title, detail);
  }

  return response.json() as Promise<ImportEmployeesResult>;
}

/** آدرس دانلود قالب نمونه پرسنل (اکسل). */
export const EMPLOYEES_EXCEL_TEMPLATE_URL =
  "/api/v1/import/employees/template?format=xlsx";

/** آدرس دانلود قالب نمونه پرسنل (CSV). */
export const EMPLOYEES_CSV_TEMPLATE_URL =
  "/api/v1/import/employees/template?format=csv";

// ─── Staging (جدول واسط) ──────────────────────────────────────────────

/** وضعیت بارگذاری واسط. */
export type ImportBatchStatus = 1 | 2 | 3 | 4;

/** بارگذاری واسط. */
export interface ImportBatch {
  id: string;
  organizationId: string;
  organizationName?: string | null;
  source: number;
  fileName?: string | null;
  status: ImportBatchStatus;
  totalRows: number;
  validRows: number;
  invalidRows: number;
  committedCount?: number | null;
  notes?: string | null;
  createdAt: string;
  reviewedAt?: string | null;
  committedAt?: string | null;
}

/** نتیجه صفحه‌بندی‌شده بارگذاری‌ها. */
export interface PagedImportBatches {
  items: ImportBatch[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/** خطای ردیف واسط. */
export interface StagingRowError {
  rowNumber?: number | null;
  columnName?: string | null;
  errorCode: string;
  message: string;
}

/** ردیف واسط. */
export interface StagingRow {
  id: string;
  rowNumber: number;
  personnelCode: string;
  firstName: string;
  lastName: string;
  nationalCode: string;
  mobile?: string | null;
  birthDate?: string | null;
  serviceYears?: number | null;
  serviceMonths?: number | null;
  validationStatus: number;
  commitStatus: number;
  employeeId?: string | null;
  errors: StagingRowError[];
}

/** نتیجه صفحه‌بندی‌شده ردیف‌ها. */
export interface PagedStagingRows {
  items: StagingRow[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/** نتیجه بارگذاری در جدول واسط. */
export interface StagingUploadResult {
  batchId: string;
  totalRows: number;
  validRows: number;
  invalidRows: number;
}

/**
 * بارگذاری فایل پرسنل در جدول واسط (بدون ثبت نهایی).
 */
export async function uploadToStaging(input: {
  organizationId: string;
  file: File;
}): Promise<StagingUploadResult> {
  const formData = new FormData();
  formData.append("organizationId", input.organizationId);
  formData.append("file", input.file);

  const response = await fetch("/api/v1/import/employees/staging/upload", {
    method: "POST",
    credentials: "include",
    body: formData,
  });

  if (!response.ok) {
    let detail = "خطایی رخ داده است.";
    let title: string | undefined;
    try {
      const payload = await response.json();
      detail = payload.detail ?? payload.title ?? detail;
      title = payload.title;
    } catch {
      // پاسخ JSON نبود
    }
    throw new ApiError(response.status, title, detail);
  }

  return response.json() as Promise<StagingUploadResult>;
}

/**
 * فهرست بارگذاری‌های واسط.
 */
export function fetchStagingBatches(input: {
  organizationId?: string;
  status?: number;
  page: number;
  pageSize: number;
}): Promise<PagedImportBatches> {
  return apiFetch<PagedImportBatches>(
    "/api/v1/import/employees/staging/batches",
    {
      query: {
        organizationId: input.organizationId || undefined,
        status: input.status,
        page: input.page,
        pageSize: input.pageSize,
      },
    },
  );
}

/**
 * ردیف‌های یک بارگذاری واسط.
 */
export function fetchStagingRows(input: {
  batchId: string;
  page: number;
  pageSize: number;
}): Promise<PagedStagingRows> {
  return apiFetch<PagedStagingRows>(
    `/api/v1/import/employees/staging/${input.batchId}/rows`,
    {
      query: {
        page: input.page,
        pageSize: input.pageSize,
      },
    },
  );
}