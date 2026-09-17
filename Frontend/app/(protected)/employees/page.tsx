"use client";

import * as React from "react";
import Link from "next/link";
import RequirePermission from "../../../src/components/RequirePermission";
import {
  Alert,
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

const pageSize = 10;

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
  const [searchTerm, setSearchTerm] = React.useState("");
  const [draft, setDraft] = React.useState("");
  const canViewSensitive = useCanViewSensitiveData();

  const employeesQuery = useEmployees({
    searchTerm: searchTerm || undefined,
    page: page + 1,
    pageSize,
  });

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
        <Box sx={{ display: "flex", gap: 1, mb: 2 }}>
          <TextField
            label="جستجو (کد پرسنلی/کد ملی)"
            size="small"
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                setPage(0);
                setSearchTerm(draft);
              }
            }}
          />
          <Button
            variant="outlined"
            onClick={() => {
              setPage(0);
              setSearchTerm(draft);
            }}
          >
            جستجو
          </Button>
        </Box>
        {employeesQuery.isError && (
          <Alert severity="error">خطا در دریافت فهرست پرسنل.</Alert>
        )}
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>کد پرسنلی</TableCell>
                <TableCell>نام و نام خانوادگی</TableCell>
                <TableCell>کد ملی</TableCell>
                <TableCell>وضعیت</TableCell>
                <TableCell>عملیات</TableCell>
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
    <TableRow>
      <TableCell>{employee.personnelCode}</TableCell>
      <TableCell>
        {employee.firstName} {employee.lastName}
      </TableCell>
      <TableCell>{maskSensitive(employee.nationalCode, canViewSensitive)}</TableCell>
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
