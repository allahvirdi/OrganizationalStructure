"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  assignResponsibility,
  createResponsibility,
  disableResponsibility,
  endResponsibilityAssignment,
  fetchPostResponsibilities,
  fetchResponsibilities,
  fetchResponsibility,
  updateResponsibility,
} from "./api";

export const responsibilitiesQueryKey = ["responsibilities"] as const;

/**
 * فهرست مسئولیت‌ها.
 */
export function useResponsibilities(input: {
  searchTerm?: string;
  isActive?: boolean;
  page: number;
  pageSize: number;
}) {
  return useQuery({
    queryKey: [...responsibilitiesQueryKey, input],
    queryFn: () => fetchResponsibilities(input),
  });
}

/**
 * جزئیات مسئولیت با کد.
 */
export function useResponsibility(code: string) {
  return useQuery({
    queryKey: [...responsibilitiesQueryKey, "detail", code],
    queryFn: () => fetchResponsibility(code),
  });
}

/**
 * مسئولیت‌های یک پست.
 */
export function usePostResponsibilities(postId: string) {
  return useQuery({
    queryKey: [...responsibilitiesQueryKey, "post", postId],
    queryFn: () => fetchPostResponsibilities(postId),
  });
}

function useInvalidateResponsibilities() {
  const queryClient = useQueryClient();
  return () => {
    queryClient.invalidateQueries({ queryKey: responsibilitiesQueryKey });
  };
}

/**
 * تعریف مسئولیت.
 */
export function useCreateResponsibility() {
  const invalidate = useInvalidateResponsibilities();
  return useMutation({
    mutationFn: createResponsibility,
    onSuccess: () => invalidate(),
  });
}

/**
 * ویرایش مسئولیت.
 */
export function useUpdateResponsibility(id: string) {
  const invalidate = useInvalidateResponsibilities();
  return useMutation({
    mutationFn: (input: { title: string; description?: string | null }) =>
      updateResponsibility(id, input),
    onSuccess: () => invalidate(),
  });
}

/**
 * غیرفعال‌سازی مسئولیت.
 */
export function useDisableResponsibility(id: string) {
  const invalidate = useInvalidateResponsibilities();
  return useMutation({
    mutationFn: () => disableResponsibility(id),
    onSuccess: () => invalidate(),
  });
}

/**
 * انتساب به پست.
 */
export function useAssignResponsibility() {
  const invalidate = useInvalidateResponsibilities();
  return useMutation({
    mutationFn: assignResponsibility,
    onSuccess: () => invalidate(),
  });
}

/**
 * پایان انتساب.
 */
export function useEndResponsibilityAssignment(assignmentId: string) {
  const invalidate = useInvalidateResponsibilities();
  return useMutation({
    mutationFn: (endDate: string) =>
      endResponsibilityAssignment(assignmentId, endDate),
    onSuccess: () => invalidate(),
  });
}
