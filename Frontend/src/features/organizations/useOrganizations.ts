"use client";

import { useQuery } from "@tanstack/react-query";
import { fetchOrganizations } from "./api";

export const organizationsQueryKey = ["organizations"] as const;

/**
 * سازمان‌های قابل انتخاب کاربر جاری (خود سازمان + زیرمجموعه‌ها).
 */
export function useOrganizations(input?: { searchTerm?: string }) {
  return useQuery({
    queryKey: [...organizationsQueryKey, input?.searchTerm ?? ""],
    queryFn: () => fetchOrganizations(input),
  });
}