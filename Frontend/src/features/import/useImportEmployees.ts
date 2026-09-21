"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { importEmployees } from "./api";
import { employeesQueryKey } from "../employees/useEmployees";

/**
 * هوک بارگذاری دسته‌جمعی پرسنل از فایل اکسل/CSV.
 *
 * پس از موفقیت، کش پرسنل را باطل می‌کند.
 */
export function useImportEmployees() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (input: { organizationId: string; file: File }) =>
      importEmployees(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: employeesQueryKey });
    },
  });
}