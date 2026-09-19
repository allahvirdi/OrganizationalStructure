"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  assignPost,
  createEmployee,
  endAssignment,
  fetchEmployee,
  fetchEmployeePosts,
  fetchEmployees,
  setEmployeeStatus,
  updateEmployee,
  updateSupplementary,
} from "./api";

export const employeesQueryKey = ["employees"] as const;

/**
 * فهرست صفحه‌بندی‌شده پرسنل.
 */
export function useEmployees(input: {
  searchTerm?: string;
  page: number;
  pageSize: number;
}) {
  return useQuery({
    queryKey: [...employeesQueryKey, input],
    queryFn: () => fetchEmployees(input),
  });
}

/**
 * جزئیات پرسنل.
 */
export function useEmployee(id: string) {
  return useQuery({
    queryKey: [...employeesQueryKey, "detail", id],
    queryFn: () => fetchEmployee(id),
  });
}

/**
 * پست‌های پرسنل.
 */
export function useEmployeePosts(id: string) {
  return useQuery({
    queryKey: [...employeesQueryKey, "posts", id],
    queryFn: () => fetchEmployeePosts(id),
  });
}

function useInvalidateEmployees() {
  const queryClient = useQueryClient();
  return () => {
    queryClient.invalidateQueries({ queryKey: employeesQueryKey });
  };
}

/**
 * ثبت پرسنل.
 */
export function useCreateEmployee() {
  const invalidate = useInvalidateEmployees();
  return useMutation({
    mutationFn: createEmployee,
    onSuccess: () => invalidate(),
  });
}

/**
 * ویرایش پرسنل.
 */
export function useUpdateEmployee(id: string) {
  const invalidate = useInvalidateEmployees();
  return useMutation({
    mutationFn: (input: {
      firstName: string;
      lastName: string;
      nationalCode: string;
      mobile: string;
    }) => updateEmployee(id, input),
    onSuccess: () => invalidate(),
  });
}

/**
 * ویرایش اطلاعات تکمیلی.
 */
export function useUpdateSupplementary(id: string) {
  const invalidate = useInvalidateEmployees();
  return useMutation({
    mutationFn: (input: {
      birthDate?: string | null;
      serviceYears?: number | null;
      serviceMonths?: number | null;
      pezhvakMobile?: string | null;
    }) => updateSupplementary(id, input),
    onSuccess: () => invalidate(),
  });
}

/**
 * انتساب به پست.
 */
export function useAssignPost(id: string) {
  const invalidate = useInvalidateEmployees();
  return useMutation({
    mutationFn: (input: {
      postId: string;
      fromDate?: string | null;
      toDate?: string | null;
      isPrimary: boolean;
    }) => assignPost(id, input),
    onSuccess: () => invalidate(),
  });
}

/**
 * پایان انتساب.
 */
export function useEndAssignment(id: string, postId: string) {
  const invalidate = useInvalidateEmployees();
  return useMutation({
    mutationFn: (endDate: string) => endAssignment(id, postId, endDate),
    onSuccess: () => invalidate(),
  });
}

/**
 * تغییر وضعیت پرسنل.
 */
export function useSetEmployeeStatus(id: string) {
  const invalidate = useInvalidateEmployees();
  return useMutation({
    mutationFn: (isActive: boolean) => setEmployeeStatus(id, isActive),
    onSuccess: () => invalidate(),
  });
}
