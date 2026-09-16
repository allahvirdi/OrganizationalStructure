import { apiFetch } from "../../lib/api/client";
import type { PostSummary } from "../org-chart/api";

/** پست تفصیلی از API. */
export interface PostDetail extends PostSummary {
  description?: string | null;
  hasSigningAuthority: boolean;
  responsibilities: Array<{
    id: string;
    responsibilityCode: string;
    responsibilityTitle: string;
    startDate?: string | null;
    endDate?: string | null;
    isActive: boolean;
  }>;
  authorities: Array<{
    id: string;
    authorityCode: string;
    authorityTitle: string;
    startDate?: string | null;
    endDate?: string | null;
    isActive: boolean;
  }>;
}

/** نتیجه صفحه‌بندی‌شده. */
export interface PagedPostsDetail {
  items: PostSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * فهرست پست‌ها با جستجو و صفحه‌بندی.
 */
export function fetchPosts(input: {
  organizationId?: string;
  searchTerm?: string;
  isActive?: boolean;
  page: number;
  pageSize: number;
}): Promise<PagedPostsDetail> {
  return apiFetch<PagedPostsDetail>("/api/v1/posts", {
    query: {
      organizationId: input.organizationId,
      searchTerm: input.searchTerm || undefined,
      isActive: input.isActive,
      page: input.page,
      pageSize: input.pageSize,
    },
  });
}

/**
 * دریافت پست با شناسه.
 */
export function fetchPost(id: string): Promise<PostDetail> {
  return apiFetch<PostDetail>(`/api/v1/posts/${id}`);
}

/**
 * ایجاد پست.
 */
export function createPost(input: {
  organizationId: string;
  code: string;
  title: string;
  description?: string | null;
  parentId?: string | null;
}): Promise<string> {
  return apiFetch<string>("/api/v1/posts", { method: "POST", body: input });
}

/**
 * ویرایش پست.
 */
export function updatePost(
  id: string,
  input: { code: string; title: string; description?: string | null },
): Promise<void> {
  return apiFetch<void>(`/api/v1/posts/${id}`, {
    method: "PUT",
    body: input,
  });
}

/**
 * جابجایی پست در درخت.
 */
export function movePost(id: string, newParentId: string | null): Promise<void> {
  return apiFetch<void>(`/api/v1/posts/${id}/move`, {
    method: "POST",
    body: { newParentId },
  });
}

/**
 * تعیین وضعیت فعال/غیرفعال.
 */
export function setPostStatus(id: string, isActive: boolean): Promise<void> {
  return apiFetch<void>(`/api/v1/posts/${id}/status`, {
    method: "PATCH",
    body: { isActive },
  });
}
