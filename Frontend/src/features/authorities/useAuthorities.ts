"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  assignAuthority,
  createAuthority,
  disableAuthority,
  endAuthorityAssignment,
  fetchAuthorities,
  fetchAuthority,
  fetchPostAuthorities,
  updateAuthority,
} from "./api";

export const authoritiesQueryKey = ["authorities"] as const;

/**
 * فهرست اختیارها.
 */
export function useAuthorities(input: {
  searchTerm?: string;
  page: number;
  pageSize: number;
}) {
  return useQuery({
    queryKey: [...authoritiesQueryKey, input],
    queryFn: () => fetchAuthorities(input),
  });
}

/**
 * جزئیات اختیار با کد.
 */
export function useAuthority(code: string) {
  return useQuery({
    queryKey: [...authoritiesQueryKey, "detail", code],
    queryFn: () => fetchAuthority(code),
  });
}

/**
 * اختیارهای یک پست.
 */
export function usePostAuthorities(postId: string) {
  return useQuery({
    queryKey: [...authoritiesQueryKey, "post", postId],
    queryFn: () => fetchPostAuthorities(postId),
  });
}

function useInvalidateAuthorities() {
  const queryClient = useQueryClient();
  return () => {
    queryClient.invalidateQueries({ queryKey: authoritiesQueryKey });
  };
}

/**
 * تعریف اختیار.
 */
export function useCreateAuthority() {
  const invalidate = useInvalidateAuthorities();
  return useMutation({
    mutationFn: createAuthority,
    onSuccess: () => invalidate(),
  });
}

/**
 * ویرایش اختیار.
 */
export function useUpdateAuthority(id: string) {
  const invalidate = useInvalidateAuthorities();
  return useMutation({
    mutationFn: (input: { title: string; description?: string | null }) =>
      updateAuthority(id, input),
    onSuccess: () => invalidate(),
  });
}

/**
 * غیرفعال‌سازی اختیار.
 */
export function useDisableAuthority(id: string) {
  const invalidate = useInvalidateAuthorities();
  return useMutation({
    mutationFn: () => disableAuthority(id),
    onSuccess: () => invalidate(),
  });
}

/**
 * انتساب به پست.
 */
export function useAssignAuthority() {
  const invalidate = useInvalidateAuthorities();
  return useMutation({
    mutationFn: assignAuthority,
    onSuccess: () => invalidate(),
  });
}

/**
 * پایان انتساب.
 */
export function useEndAuthorityAssignment(assignmentId: string) {
  const invalidate = useInvalidateAuthorities();
  return useMutation({
    mutationFn: (endDate: string) =>
      endAuthorityAssignment(assignmentId, endDate),
    onSuccess: () => invalidate(),
  });
}
