"use client";

import { QueryClient } from "@tanstack/react-query";

/**
 * نمونه سراسری TanStack Query.
 */
export function createQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      queries: {
        retry: 1,
        refetchOnWindowFocus: false,
      },
    },
  });
}
