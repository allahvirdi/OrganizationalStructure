"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  createPost,
  fetchPost,
  fetchPosts,
  movePost,
  setPostStatus,
  updatePost,
} from "./api";

export const postsQueryKey = ["posts"] as const;

/**
 * فهرست صفحه‌بندی‌شده پست‌ها.
 */
export function usePosts(input: {
  organizationId?: string;
  searchTerm?: string;
  page: number;
  pageSize: number;
}) {
  return useQuery({
    queryKey: [...postsQueryKey, input],
    queryFn: () => fetchPosts(input),
  });
}

/**
 * جزئیات یک پست.
 */
export function usePost(id: string) {
  return useQuery({
    queryKey: [...postsQueryKey, "detail", id],
    queryFn: () => fetchPost(id),
  });
}

function useInvalidatePosts() {
  const queryClient = useQueryClient();
  return () => {
    queryClient.invalidateQueries({ queryKey: postsQueryKey });
  };
}

/**
 * ایجاد پست.
 */
export function useCreatePost() {
  const invalidate = useInvalidatePosts();
  return useMutation({
    mutationFn: createPost,
    onSuccess: () => invalidate(),
  });
}

/**
 * ویرایش پست.
 */
export function useUpdatePost(id: string) {
  const invalidate = useInvalidatePosts();
  return useMutation({
    mutationFn: (input: {
      code: string;
      title: string;
      description?: string | null;
    }) => updatePost(id, input),
    onSuccess: () => invalidate(),
  });
}

/**
 * جابجایی پست.
 */
export function useMovePost(id: string) {
  const invalidate = useInvalidatePosts();
  return useMutation({
    mutationFn: (newParentId: string | null) => movePost(id, newParentId),
    onSuccess: () => invalidate(),
  });
}

/**
 * تغییر وضعیت پست.
 */
export function useSetPostStatus(id: string) {
  const invalidate = useInvalidatePosts();
  return useMutation({
    mutationFn: (isActive: boolean) => setPostStatus(id, isActive),
    onSuccess: () => invalidate(),
  });
}
