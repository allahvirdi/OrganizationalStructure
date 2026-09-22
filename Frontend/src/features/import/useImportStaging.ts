"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  fetchStagingBatches,
  fetchStagingRows,
  uploadToStaging,
} from "./api";

export const stagingQueryKey = ["staging"] as const;

/**
 * فهرست بارگذاری‌های واسط.
 */
export function useStagingBatches(input: {
  organizationId?: string;
  status?: number;
  page: number;
  pageSize: number;
}) {
  return useQuery({
    queryKey: [...stagingQueryKey, "batches", input],
    queryFn: () => fetchStagingBatches(input),
  });
}

/**
 * ردیف‌های یک بارگذاری واسط.
 */
export function useStagingRows(input: {
  batchId: string;
  page: number;
  pageSize: number;
}) {
  return useQuery({
    queryKey: [...stagingQueryKey, "rows", input],
    queryFn: () => fetchStagingRows(input),
    enabled: !!input.batchId,
  });
}

/**
 * بارگذاری فایل در جدول واسط.
 */
export function useUploadToStaging() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: { organizationId: string; file: File }) =>
      uploadToStaging(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: stagingQueryKey });
    },
  });
}