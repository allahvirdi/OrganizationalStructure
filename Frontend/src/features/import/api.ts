import { ApiError } from "../../lib/api/client";

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