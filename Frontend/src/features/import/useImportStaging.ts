"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  commitStagingBatch,
  fetchStagingBatches,
  fetchStagingRows,
  rejectStagingBatch,
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

/**
 * ثبت نهایی بارگذاری واسط (تأیید).
 */
export function useCommitStagingBatch(options?: {
  onSuccess?: (data: import("./api").StagingCommitResult) => void;
  onError?: (error: unknown) => void;
}) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (batchId: string) => commitStagingBatch(batchId),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: stagingQueryKey });
      options?.onSuccess?.(data);
    },
    onError: (error) => {
      options?.onError?.(error);
    },
  });
}

/**
 * رد بارگذاری واسط.
 */
export function useRejectStagingBatch(options?: {
  onSuccess?: () => void;
  onError?: (error: unknown) => void;
}) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: { batchId: string; notes?: string }) =>
      rejectStagingBatch(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: stagingQueryKey });
      options?.onSuccess?.();
    },
    onError: (error) => {
      options?.onError?.(error);
    },
  });
}