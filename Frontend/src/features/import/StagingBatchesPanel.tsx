"use client";

import * as React from "react";
import { useOrganizations } from "../organizations/useOrganizations";
import type { OrganizationOption } from "../organizations/api";
import {
  useStagingBatches,
  useStagingRows,
  useUploadToStaging,
} from "./useImportStaging";
import type { ImportBatch, StagingRow } from "./api";
import { ApiError } from "../../lib/api/client";
import { formatJalali, isoToJalali } from "../../lib/date/jalali";

import {
  Alert,
  Autocomplete,
  Box,
  Button,
  CircularProgress,
  Container,
  Dialog,
  DialogContent,
  DialogTitle,
  Divider,
  IconButton,
  MenuItem,
  Paper,
  Select,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import VisibilityIcon from "@mui/icons-material/Visibility";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";

/** وضعیت‌های بارگذاری واسط. */
const BATCH_STATUS_LABELS: Record<number, string> = {
  1: "در انتظار درج",
  2: "آماده",
  3: "ثبت‌شده",
  4: "ردشده",
};

/** وضعیت‌های اعتبارسنجی ردیف. */
const VALIDATION_STATUS_LABELS: Record<number, string> = {
  1: "در انتظار",
  2: "معتبر",
  3: "نامعتبر",
};

/** وضعیت‌های ثبت ردیف. */
const COMMIT_STATUS_LABELS: Record<number, string> = {
  1: "در انتظار",
  2: "ثبت‌شده",
  3: "ردشده",
};

/**
 * پنل بارگذاری‌های واسط (جدول واسط پرسنل).
 */
export default function StagingBatchesPanel() {
  const [organization, setOrganization] =
    React.useState<OrganizationOption | null>(null);
  const [organizationSearchTerm, setOrganizationSearchTerm] =
    React.useState("");
  const [statusFilter, setStatusFilter] = React.useState<number | "">("");
  const [page, setPage] = React.useState(1);
  const [pageSize, setPageSize] = React.useState(10);
  const [file, setFile] = React.useState<File | null>(null);
  const [uploadSuccess, setUploadSuccess] = React.useState<string | null>(
    null,
  );
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const [selectedBatch, setSelectedBatch] = React.useState<ImportBatch | null>(
    null,
  );
  const [rowsPage, setRowsPage] = React.useState(1);

  const organizations = useOrganizations({ searchTerm: organizationSearchTerm });
  const allOrganizations = useOrganizations();

  const batches = useStagingBatches({
    organizationId: organization?.id,
    status: statusFilter || undefined,
    page,
    pageSize,
  });

  const uploadMutation = useUploadToStaging();

  const defaultOrganization =
    organization ??
    (allOrganizations.data?.find((o) => o.isCurrent) ??
      allOrganizations.data?.[0] ??
      null);
  const selectedOrganization = organization ?? defaultOrganization;

  const canUpload = !!file && !!selectedOrganization;

  const handleUpload = async () => {
    if (!file || !selectedOrganization) return;
    setUploadSuccess(null);
    try {
      const result = await uploadMutation.mutateAsync({
        organizationId: selectedOrganization.id,
        file,
      });
      setUploadSuccess(
        `بارگذاری در جدول واسط موفق بود: ${result.validRows} معتبر، ${result.invalidRows} نامعتبر از ${result.totalRows} ردیف.`,
      );
      setFile(null);
      if (fileInputRef.current) fileInputRef.current.value = "";
    } catch {
      // خطا توسط isError نشان داده می‌شود
    }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" sx={{ mb: 1 }}>
          جدول واسط پرسنل
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          بارگذاری فایل در جدول واسط و بازبینی ردیف‌ها قبل از ثبت نهایی.
        </Typography>

        <Divider sx={{ mb: 3 }} />

        <Box sx={{ display: "flex", gap: 2, mb: 3, flexWrap: "wrap", alignItems: "center" }}>
          <Autocomplete
            options={organizations.data ?? []}
            value={selectedOrganization}
            onChange={(_, newValue) => { setOrganization(newValue); setPage(1); }}
            onInputChange={(_, newInput) => setOrganizationSearchTerm(newInput)}
            getOptionLabel={(option) => `${option.name} (${option.code})`}
            isOptionEqualToValue={(a, b) => a.id === b.id}
            loading={organizations.isLoading}
            renderInput={(params) => (
              <TextField {...params} label="سازمان" placeholder="جستجو با نام یا کد سازمان…" />
            )}
            sx={{ minWidth: 260 }}
            noOptionsText="سازمانی یافت نشد"
            loadingText="در حال بارگذاری…"
          />

          <Select
            value={statusFilter}
            onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
            displayEmpty
            sx={{ minWidth: 160 }}
          >
            <MenuItem value="">همه وضعیت‌ها</MenuItem>
            {Object.entries(BATCH_STATUS_LABELS).map(([value, label]) => (
              <MenuItem key={value} value={Number(value)}>{label}</MenuItem>
            ))}
          </Select>

          <Box sx={{ flexGrow: 1 }} />

          <Button variant="outlined" component="label" startIcon={<UploadFileIcon />}>
            انتخاب فایل
            <input
              ref={fileInputRef}
              type="file"
              accept=".xlsx,.csv"
              hidden
              onChange={(e) => { setFile(e.target.files?.[0] ?? null); setUploadSuccess(null); }}
            />
          </Button>

          <Button
            variant="contained"
            onClick={handleUpload}
            disabled={!canUpload || uploadMutation.isPending}
            startIcon={uploadMutation.isPending ? <CircularProgress size={20} /> : <UploadFileIcon />}
          >
            {uploadMutation.isPending ? "در حال بارگذاری…" : "بارگذاری در جدول واسط"}
          </Button>
        </Box>

        {file && (
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            فایل انتخاب‌شده: {file.name} ({(file.size / 1024).toFixed(1)} کیلوبایت)
          </Typography>
        )}

        {uploadSuccess && <Alert severity="success" sx={{ mb: 2 }}>{uploadSuccess}</Alert>}

        {uploadMutation.isError && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {uploadMutation.error instanceof ApiError
              ? uploadMutation.error.message
              : "خطایی در بارگذاری رخ داد."}
          </Alert>
        )}

        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>فایل</TableCell>
              <TableCell>سازمان</TableCell>
              <TableCell>وضعیت</TableCell>
              <TableCell align="center">کل</TableCell>
              <TableCell align="center">معتبر</TableCell>
              <TableCell align="center">نامعتبر</TableCell>
              <TableCell>تاریخ</TableCell>
              <TableCell align="center">عملیات</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {(batches.data?.items ?? []).map((batch) => (
              <TableRow key={batch.id} hover>
                <TableCell>{batch.fileName ?? "—"}</TableCell>
                <TableCell>{batch.organizationName ?? "—"}</TableCell>
                <TableCell>{BATCH_STATUS_LABELS[batch.status] ?? batch.status}</TableCell>
                <TableCell align="center">{batch.totalRows}</TableCell>
                <TableCell align="center">{batch.validRows}</TableCell>
                <TableCell align="center">{batch.invalidRows}</TableCell>
                <TableCell>{formatJalali(isoToJalali(batch.createdAt))}</TableCell>
                <TableCell align="center">
                  <Tooltip title="مشاهده ردیف‌ها">
                    <IconButton size="small" onClick={() => { setSelectedBatch(batch); setRowsPage(1); }}>
                      <VisibilityIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>

        {batches.isLoading && (
          <Box sx={{ display: "flex", justifyContent: "center", py: 3 }}>
            <CircularProgress />
          </Box>
        )}

        {!batches.isLoading && (batches.data?.items.length ?? 0) === 0 && (
          <Typography variant="body2" color="text.secondary" sx={{ py: 3, textAlign: "center" }}>
            بارگذاری‌ای یافت نشد.
          </Typography>
        )}

        {(batches.data?.totalCount ?? 0) > 0 && (
          <TablePagination
            component="div"
            count={batches.data?.totalCount ?? 0}
            page={page - 1}
            onPageChange={(_, newPage) => setPage(newPage + 1)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={(e) => { setPageSize(Number(e.target.value)); setPage(1); }}
            labelRowsPerPage="ردیف در صفحه:"
            labelDisplayedRows={({ from, to, count }) => `${from}–${to} از ${count}`}
            rowsPerPageOptions={[10, 20, 50]}
          />
        )}
      </Paper>

      {selectedBatch && (
        <StagingRowsDialog
          batch={selectedBatch}
          page={rowsPage}
          onPageChange={setRowsPage}
          onClose={() => setSelectedBatch(null)}
        />
      )}
    </Container>
  );
}

/**
 * دیالوگ مشاهده ردیف‌های یک بارگذاری واسط.
 */
function StagingRowsDialog({
  batch,
  page,
  onPageChange,
  onClose,
}: {
  batch: ImportBatch;
  page: number;
  onPageChange: (page: number) => void;
  onClose: () => void;
}) {
  const pageSize = 10;
  const rows = useStagingRows({ batchId: batch.id, page, pageSize });

  return (
    <Dialog open onClose={onClose} fullWidth maxWidth="lg">
      <DialogTitle sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <IconButton onClick={onClose} size="small">
          <ArrowBackIcon fontSize="small" />
        </IconButton>
        ردیف‌های بارگذاری {batch.fileName ?? batch.id}
      </DialogTitle>

      <DialogContent dividers>
        <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap", mb: 2 }}>
          <Typography variant="body2">
            وضعیت: {BATCH_STATUS_LABELS[batch.status] ?? batch.status}
          </Typography>
          <Typography variant="body2">کل: {batch.totalRows}</Typography>
          <Typography variant="body2">معتبر: {batch.validRows}</Typography>
          <Typography variant="body2">نامعتبر: {batch.invalidRows}</Typography>
        </Box>

        {rows.isLoading && (
          <Box sx={{ display: "flex", justifyContent: "center", py: 3 }}>
            <CircularProgress />
          </Box>
        )}

        {rows.isError && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {rows.error instanceof ApiError
              ? rows.error.message
              : "خطایی در دریافت ردیف‌ها رخ داد."}
          </Alert>
        )}

        {!rows.isLoading && !rows.isError && (
          <>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell align="center">ردیف</TableCell>
                  <TableCell>کد پرسنلی</TableCell>
                  <TableCell>نام</TableCell>
                  <TableCell>نام خانوادگی</TableCell>
                  <TableCell>کد ملی</TableCell>
                  <TableCell>اعتبارسنجی</TableCell>
                  <TableCell>ثبت</TableCell>
                  <TableCell>خطاها</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {(rows.data?.items ?? []).map((row: StagingRow) => (
                  <TableRow key={row.id} hover>
                    <TableCell align="center">{row.rowNumber}</TableCell>
                    <TableCell>{row.personnelCode}</TableCell>
                    <TableCell>{row.firstName}</TableCell>
                    <TableCell>{row.lastName}</TableCell>
                    <TableCell>{row.nationalCode}</TableCell>
                    <TableCell>
                      {VALIDATION_STATUS_LABELS[row.validationStatus] ?? row.validationStatus}
                    </TableCell>
                    <TableCell>
                      {COMMIT_STATUS_LABELS[row.commitStatus] ?? row.commitStatus}
                    </TableCell>
                    <TableCell sx={{ maxWidth: 320 }}>
                      {row.errors.length > 0 ? (
                        <Typography variant="caption" color="error">
                          {row.errors.map((error) => error.message).join("؛ ")}
                        </Typography>
                      ) : (
                        "—"
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>

            {(rows.data?.totalCount ?? 0) > 0 && (
              <TablePagination
                component="div"
                count={rows.data?.totalCount ?? 0}
                page={page - 1}
                onPageChange={(_, newPage) => onPageChange(newPage + 1)}
                rowsPerPage={pageSize}
                labelRowsPerPage=""
                labelDisplayedRows={({ from, to, count }) => `${from}–${to} از ${count}`}
                rowsPerPageOptions={[]}
              />
            )}
          </>
        )}
      </DialogContent>
    </Dialog>
  );
}