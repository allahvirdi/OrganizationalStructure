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
import {
  useDisableResponsibility,
  useResponsibilities,
} from "../../../src/features/responsibilities/useResponsibilities";

/**
 * فهرست مسئولیت‌ها.
 */
export default function ResponsibilitiesPage() {
  return (
    <RequirePermission permission="OrganizationStructure.Responsibility.View">
      <ResponsibilitiesContent />
    </RequirePermission>
  );
}

const pageSize = 10;

function ResponsibilitiesContent() {
  const [page, setPage] = React.useState(0);
  const [searchTerm, setSearchTerm] = React.useState("");
  const [draft, setDraft] = React.useState("");

  const listQuery = useResponsibilities({
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
            مسئولیت‌ها
          </Typography>
          <Button
            component={Link}
            href="/responsibilities/new"
            variant="contained"
          >
            مسئولیت جدید
          </Button>
        </Box>
        <Box sx={{ display: "flex", gap: 1, mb: 2 }}>
          <TextField
            label="جستجو (کد/عنوان)"
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
        {listQuery.isError && (
          <Alert severity="error">خطا در دریافت فهرست.</Alert>
        )}
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>کد</TableCell>
                <TableCell>عنوان</TableCell>
                <TableCell>وضعیت</TableCell>
                <TableCell>عملیات</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(listQuery.data?.items ?? []).map((item) => (
                <ResponsibilityRow key={item.id} item={item} />
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={listQuery.data?.totalCount ?? 0}
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

function ResponsibilityRow({
  item,
}: {
  item: { id: string; code: string; title: string; isActive: boolean };
}) {
  const disable = useDisableResponsibility(item.id);
  const [error, setError] = React.useState<string | null>(null);

  return (
    <TableRow>
      <TableCell dir="ltr">{item.code}</TableCell>
      <TableCell>{item.title}</TableCell>
      <TableCell>
        <Chip
          label={item.isActive ? "فعال" : "غیرفعال"}
          size="small"
          color={item.isActive ? "success" : "default"}
        />
      </TableCell>
      <TableCell>
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          <Button
            component={Link}
            href={`/responsibilities/${encodeURIComponent(item.code)}`}
            size="small"
          >
            جزئیات
          </Button>
          {item.isActive && (
            <Button
              size="small"
              color="warning"
              disabled={disable.isPending}
              onClick={() => {
                setError(null);
                disable.mutate(undefined, {
                  onError: () =>
                    setError("غیرفعال‌سازی ممکن نیست (انتساب جاری دارد؟)."),
                });
              }}
            >
              غیرفعال
            </Button>
          )}
        </Box>
        {error && (
          <Typography variant="caption" color="error" sx={{ display: "block" }}>
            {error}
          </Typography>
        )}
      </TableCell>
    </TableRow>
  );
}
