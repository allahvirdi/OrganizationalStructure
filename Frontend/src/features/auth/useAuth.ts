"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { fetchMe, login, logout, verifyMfa } from "../../lib/api/auth";
import { ApiError } from "../../lib/api/client";

export const meQueryKey = ["me"] as const;

/**
 * کاربر جاری (null در صورت عدم احراز).
 */
export function useMe() {
  return useQuery({
    queryKey: meQueryKey,
    queryFn: fetchMe,
    retry: (count, error) =>
      error instanceof ApiError && error.status === 401 ? false : count < 1,
  });
}

/**
 * ورود (شامل حالت نیاز به دومرحله‌ای).
 */
export function useLogin() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: login,
    onSuccess: (result) => {
      if (!result.requiresMfa) {
        queryClient.invalidateQueries({ queryKey: meQueryKey });
      }
    },
  });
}

/**
 * تأیید دومرحله‌ای.
 */
export function useVerifyMfa() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: verifyMfa,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: meQueryKey });
    },
  });
}

/**
 * خروج.
 */
export function useLogout() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: logout,
    onSettled: () => {
      queryClient.setQueryData(meQueryKey, null);
    },
  });
}
