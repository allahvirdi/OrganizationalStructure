"use client";

import * as React from "react";
import Link from "next/link";
import RequirePermission from "../../../src/components/RequirePermission";
import {
  Alert,
  Autocomplete,
  Box,
  Button,
  Chip,
  Container,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { useEmployees, useSetEmployeeStatus } from "../../../src/features/employees/useEmployees";
import { maskSensitive, useCanViewSensitiveData } from "../../../src/features/employees/usePermissions";
import type { OrganizationOption } from "../../../src/features/organizations/api";
import { useOrganizations } from "../../../src/features/organizations/useOrganizations";

const pageSize = 10;

/** فیلترهای اعمال‌شده روی پرس‌وجوی فهرست. */
interface AppliedFilters {
  personnelCode?: string;
  nationalCode?: string;
  organizationId?: string;
}

/**
 * فهرست پرسنل.
 */
export default function EmployeesPage() {
  return (
    <RequirePermission permission="OrganizationStructure.Employee.View">
      <EmployeesContent />
    </RequirePermission>
  );
}

function EmployeesContent() {
  const [page, setPage] = React.useState(0);
  const [personnelCodeDraft, setPersonnelCodeDraft] = React.useState("");
  const [nationalCodeDraft, setNationalCodeDraft] = React.useState("");
  const [organizationFilter, setOrganizationFilter] =
    React.useState<OrganizationOption | null>(null);
  const [organizationSearchTerm, setOrganizationSearchTerm] = React.useState("");
  const [applied, setApplied] = React.useState<AppliedFilters>({});
  const canViewSensitive = useCanViewSensitiveData();

  const organizations = useOrganizations({ searchTerm: organizationSearchTerm });

  const employeesQuery = useEmployees({
    personnelCode: applied.personnelCode || undefined,
    nationalCode: applied.nationalCode || undefined,
    organizationId: applied.organizationId || undefined,
    page: page + 1,
    pageSize,
  });

  const applyFilters = () => {
    setPage(0);
    setApplied({
      personnelCode: personnelCodeDraft.trim() || undefined,
      nationalCode: nationalCodeDraft.trim() || undefined,
      organizationId: organizationFilter?.id,
    });
  };

  const resetFilters = () => {
    setPersonnelCodeDraft("");
    setNationalCodeDraft("");
    setOrganizationFilter(null);
    setPage(0);
    setApplied({});
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ py: 4 }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 2,
          }}
        >
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            پرسنل
          </Typography>
          <Button component={Link} href="/employees/new" variant="contained">
            پرسنل جدید
          </Button>
        </Box>
        <Paper sx={{ p: 2, mb: 2 }}>
          <Box
            sx={{
              display: "flex",
              flexWrap: "wrap",
              gap: 1.5,
              alignItems: "center",
            }}
          >
            <TextField
              label="کد پرسنلی"
              size="small"
              sx={{ minWidth: 160 }}
              value={personnelCodeDraft}
              onChange={(e) => setPersonnelCodeDraft(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") applyFilters();
              }}
            />
            <TextField
              label="کد ملی"
              size="small"
              sx={{ minWidth: 180 }}
              value={nationalCodeDraft}
              onChange={(e) => setNationalCodeDraft(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") applyFilters();
              }}
            />
            <Autocomplete
              size="small"
              sx={{ minWidth: 260 }}
              options={organizations.data ?? []}
              value={organizationFilter}
              onChange={(_, value) => setOrganizationFilter(value)}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              loading={organizations.isFetching}
              loadingText="در حال دریافت سازمان‌ها..."
              noOptionsText={
                organizations.isError
                  ? "خطا در دریافت سازمان‌ها"
                  : "سازمانی یافت نشد"
              }
              onInputChange={(_, value) => setOrganizationSearchTerm(value)}
              renderInput={(params) => (
                <TextField {...params} label="فیلتر سازمان" />
              )}
            />
            <Button variant="contained" onClick={applyFilters}>
              جستجو
            </Button>
            <Button variant="outlined" color="inherit" onClick={resetFilters}>
              پاک‌سازی
            </Button>
          </Box>
        </Paper>
        {employeesQuery.isError && (
          <Alert severity="error" sx={{ mb: 2 }}>
            خطا در دریافت فهرست پرسنل.
          </Alert>
        )}
        <TableContainer component={Paper} variant="outlined">
          <Table>
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 700 }}>کد پرسنلی</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>نام و نام خانوادگی</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>کد ملی</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>سازمان</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>وضعیت</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>عملیات</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(employeesQuery.data?.items ?? []).map((employee) => (
                <EmployeeRow
                  key={employee.id}
                  employee={employee}
                  canViewSensitive={canViewSensitive}
                />
              ))}
              {(employeesQuery.data?.items ?? []).length === 0 &&
                !employeesQuery.isLoading && (
                  <TableRow>
                    <TableCell colSpan={6} align="center">
                      <Typography
                        variant="body2"
                        color="text.secondary"
                        sx={{ py: 2 }}
                      >
                        پرسنلی یافت نشد.
                      </Typography>
                    </TableCell>
                  </TableRow>
                )}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={employeesQuery.data?.totalCount ?? 0}
            page={page}
            rowsPerPage={pageSize}
            rowsPerPageOptions={[pageSize]}
            onPageChange={(_, next) => setPage(next)}
            labelDisplayedRows={({ from, to, count }) =>
              `${from} تا ${to} از ${count}`
            }
          />
        </TableContainer>
      </Box>
    </Container>
  );
}

function EmployeeRow({
  employee,
  canViewSensitive,
}: {
  employee: {
    id: string;
    organizationName?: string | null;
    personnelCode: string;
    firstName: string;
    lastName: string;
    nationalCode: string;
    isActive: boolean;
  };
  canViewSensitive: boolean;
}) {
  const setStatus = useSetEmployeeStatus(employee.id);
  const [error, setError] = React.useState<string | null>(null);

  return (
    <TableRow hover>
      <TableCell>{employee.personnelCode}</TableCell>
      <TableCell>
        {employee.firstName} {employee.lastName}
      </TableCell>
      <TableCell>{maskSensitive(employee.nationalCode, canViewSensitive)}</TableCell>
      <TableCell>
        {employee.organizationName ?? (
          <Typography variant="caption" color="text.secondary">
            —
          </Typography>
        )}
      </TableCell>
      <TableCell>
        <Chip
          label={employee.isActive ? "فعال" : "غیرفعال"}
          size="small"
          color={employee.isActive ? "success" : "default"}
        />
      </TableCell>
      <TableCell>
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          <Button
            component={Link}
            href={`/employees/${employee.id}`}
            size="small"
          >
            جزئیات
          </Button>
          <Button
            size="small"
            color={employee.isActive ? "warning" : "success"}
            disabled={setStatus.isPending}
            onClick={() => {
              setError(null);
              setStatus.mutate(!employee.isActive, {
                onError: () =>
                  setError("خطا در تغییر وضعیت."),
              });
            }}
          >
            {employee.isActive ? "غیرفعال" : "فعال"}
          </Button>
        </Box>
        {error && (
          <Typography variant="caption" color="error">
            {error}
          </Typography>
        )}
      </TableCell>
    </TableRow>
  );
}
