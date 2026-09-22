"use client";

import * as React from "react";
import { useOrganizations } from "../../../src/features/organizations/useOrganizations";
import type { OrganizationOption } from "../../../src/features/organizations/api";
import { useImportPosts } from "../../../src/features/import/useImportPosts";
import { POSTS_TEMPLATE_URL } from "../../../src/features/import/api";
import EmployeesImportPanel from "../../../src/features/import/EmployeesImportPanel";
import StagingBatchesPanel from "../../../src/features/import/StagingBatchesPanel";
import { useMe } from "../../../src/features/auth/useAuth";
import { hasPermission } from "../../../src/lib/permissions";
import { ApiError } from "../../../src/lib/api/client";

import {
  Alert,
  Autocomplete,
  Box,
  Button,
  CircularProgress,
  Container,
  Divider,
  Paper,
  Tab,
  Tabs,
  TextField,
  Typography,
} from "@mui/material";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import DownloadIcon from "@mui/icons-material/Download";

/**
 * صفحه بارگذاری ساختار سازمانی از فایل اکسل.
 */
export default function ImportPage() {
  const { data: user, isLoading } = useMe();
  const canImportPosts = hasPermission(user, "OrganizationStructure.Post.Create");
  const canImportEmployees = hasPermission(
    user,
    "OrganizationStructure.Employee.Import",
  );
  const [requestedTab, setRequestedTab] = React.useState<number | null>(null);
  const firstAllowedTab = canImportPosts ? 0 : canImportEmployees ? 1 : 2;
  const tab =
    requestedTab !== null &&
    ((requestedTab === 0 && canImportPosts) ||
      (requestedTab === 1 && canImportEmployees) ||
      (requestedTab === 2 && canImportEmployees))
      ? requestedTab
      : firstAllowedTab;

  if (isLoading) {
    return (
      <Container maxWidth="sm" sx={{ py: 8, textAlign: "center" }}>
        <CircularProgress />
      </Container>
    );
  }

  if (!canImportPosts && !canImportEmployees) {
    return (
      <Container maxWidth="sm" sx={{ py: 4 }}>
        <Alert severity="warning">برای این بخش دسترسی ندارید.</Alert>
      </Container>
    );
  }

  return (
    <>
      <Container maxWidth="sm" sx={{ pt: 4, pb: 1 }}>
        <Tabs
          value={tab}
          onChange={(_, newValue) => setRequestedTab(newValue)}
          variant="fullWidth"
        >
          <Tab label="ساختار پست‌ها" disabled={!canImportPosts} />
          <Tab label="پرسنل" disabled={!canImportEmployees} />
          <Tab label="جدول واسط" disabled={!canImportEmployees} />
        </Tabs>
      </Container>

      {tab === 0 && canImportPosts ? <ImportContent /> : null}
      {tab === 1 && canImportEmployees ? <EmployeesImportPanel /> : null}
      {tab === 2 && canImportEmployees ? <StagingBatchesPanel /> : null}
    </>
  );
}

function ImportContent() {
  const importMutation = useImportPosts();
  const [organization, setOrganization] =
    React.useState<OrganizationOption | null>(null);
  const [organizationSearchTerm, setOrganizationSearchTerm] =
    React.useState("");
  const [file, setFile] = React.useState<File | null>(null);
  const [successMessage, setSuccessMessage] = React.useState<string | null>(
    null,
  );
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const organizations = useOrganizations({
    searchTerm: organizationSearchTerm,
  });
  const allOrganizations = useOrganizations();

  const defaultOrganization =
    organization ??
    (allOrganizations.data?.find((o) => o.isCurrent) ??
      allOrganizations.data?.[0] ??
      null);
  const selectedOrganization = organization ?? defaultOrganization;

  const handleSubmit = async () => {
    if (!selectedOrganization || !file) return;
    setSuccessMessage(null);
    try {
      const result = await importMutation.mutateAsync({
        organizationId: selectedOrganization.id,
        file,
      });
      setSuccessMessage(
        `✓ ${result.importedCount} پست با موفقیت برای سازمان بارگذاری شد.`,
      );
      setFile(null);
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
    } catch {
      // خطا توسط Alert خودکار نمایش داده می‌شود
    }
  };

  const isSubmitting = importMutation.isPending;
  const canSubmit = Boolean(selectedOrganization && file && !isSubmitting);

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" sx={{ mb: 1 }}>
          بارگذاری ساختار سازمانی
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          ساختار پست‌های یک سازمان را از فایل اکسل (.xlsx) بارگذاری کنید.
        </Typography>

        <Divider sx={{ mb: 3 }} />

        <Autocomplete
          options={organizations.data ?? []}
          value={selectedOrganization}
          onChange={(_, newValue) => {
            setOrganization(newValue);
            setSuccessMessage(null);
          }}
          onInputChange={(_, newInput) => setOrganizationSearchTerm(newInput)}
          getOptionLabel={(option) => `${option.name} (${option.code})`}
          isOptionEqualToValue={(a, b) => a.id === b.id}
          loading={organizations.isLoading}
          renderInput={(params) => (
            <TextField
              {...params}
              label="سازمان"
              placeholder="جستجو با نام یا کد سازمان…"
            />
          )}
          sx={{ mb: 3 }}
          noOptionsText="سازمانی یافت نشد"
          loadingText="در حال بارگذاری…"
        />

        <Box sx={{ mb: 3 }}>
          <Button
            variant="outlined"
            component="label"
            startIcon={<UploadFileIcon />}
            sx={{ mb: 1 }}
          >
            انتخاب فایل اکسل
            <input
              ref={fileInputRef}
              type="file"
              accept=".xlsx"
              hidden
              onChange={(e) => {
                const selected = e.target.files?.[0] ?? null;
                setFile(selected);
                setSuccessMessage(null);
              }}
            />
          </Button>
          {file && (
            <Typography variant="body2" color="text.secondary">
              فایل انتخاب‌شده: {file.name} (
              {(file.size / 1024).toFixed(1)} کیلوبایت)
            </Typography>
          )}
        </Box>

        <Box sx={{ display: "flex", gap: 2, mb: 3 }}>
          <Button
            variant="contained"
            onClick={handleSubmit}
            disabled={!canSubmit}
            startIcon={
              isSubmitting ? (
                <CircularProgress size={20} />
              ) : (
                <UploadFileIcon />
              )
            }
          >
            {isSubmitting ? "در حال بارگذاری…" : "بارگذاری ساختار"}
          </Button>

          <Button
            variant="text"
            component="a"
            href={POSTS_TEMPLATE_URL}
            startIcon={<DownloadIcon />}
          >
            دانلود قالب نمونه
          </Button>
        </Box>

        {successMessage && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {successMessage}
          </Alert>
        )}

        {importMutation.isError && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {importMutation.error instanceof ApiError
              ? importMutation.error.message
              : "خطایی در بارگذاری رخ داد."}
          </Alert>
        )}

        <Divider sx={{ mb: 2 }} />
        <Typography variant="caption" color="text.secondary">
          قالب فایل: ستون‌های «کد پست»، «عنوان پست»، «شرح»، «کد پست والد».
          ردیف اول هدر است. حداکثر ۵۰۰ ردیف در هر بارگذاری.
        </Typography>
      </Paper>
    </Container>
  );
}