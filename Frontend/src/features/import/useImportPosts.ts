"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { importPosts } from "./api";
import { postsQueryKey } from "../posts/usePosts";

/**
 * هوک بارگذاری ساختار پست‌ها از فایل اکسل.
 *
 * پس از موفقیت، کش پست‌ها را باطل می‌کند.
 */
export function useImportPosts() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: { organizationId: string; file: File }) =>
      importPosts(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: postsQueryKey });
    },
  });
}