import { apiFetch } from "../../lib/api/client";

/** گره خلاصه پست از API. */
export interface PostSummary {
  id: string;
  organizationId: string;
  organizationName?: string | null;
  code: string;
  title: string;
  parentId: string | null;
  isActive: boolean;
  hasSigningAuthority?: boolean;
  responsibilityTitles?: string[];
  authorityTitles?: string[];
}

/** پاسخ صفحه‌بندی‌شده. */
export interface PagedPosts {
  items: PostSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/** گره درختی پست از API. */
export interface PostTreeNode {
  id: string;
  code: string;
  title: string;
  hasSigningAuthority: boolean;
  isActive: boolean;
  children: PostTreeNode[];
}

/**
 * فهرست پست‌های سازمان (حداکثر ۱۰۰ رکورد اول برای چارت).
 */
export function fetchOrgPosts(organizationId: string): Promise<PagedPosts> {
  return apiFetch<PagedPosts>("/api/v1/posts", {
    query: { organizationId, page: 1, pageSize: 100 },
  });
}

/**
 * زیرشاخه چندسطحی یک پست.
 */
export function fetchSubtree(
  postId: string,
  maxDepth?: number,
): Promise<PostTreeNode> {
  return apiFetch<PostTreeNode>(`/api/v1/posts/${postId}/subtree`, {
    query: maxDepth === undefined ? undefined : { maxDepth },
  });
}
