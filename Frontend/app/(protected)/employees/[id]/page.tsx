"use client";

import * as React from "react";
import { useParams } from "next/navigation";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  updateEmployeeBasicSchema,
  type EmployeeBasicForm,
} from "../../../../src/features/employees/schemas";
import {
  Alert,
  Box,
  Button,
  Checkbox,
  Chip,
  Container,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Autocomplete,
  FormControl,
  FormControlLabel,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import {
  useAssignPost,
  useEmployee,
  useEmployeePosts,
  useEndAssignment,
  useUpdateEmployee,
  useUpdateSupplementary,
} from "../../../../src/features/employees/useEmployees";
import {
  maskSensitive,
  useCanViewSensitiveData,
} from "../../../../src/features/employees/usePermissions";
import {
  supplementarySchema,
  type SupplementaryForm,
} from "../../../../src/features/employees/schemas";
import JalaliDatePicker from "../../../../src/components/JalaliDatePicker";
import {
  formatJalali,
  isoToJalali,
} from "../../../../src/lib/date/jalali";
import { fetchPosts } from "../../../../src/features/posts/api";
import type { PostSummary } from "../../../../src/features/org-chart/api";
import { useQuery } from "@tanstack/react-query";
import { ApiError } from "../../../../src/lib/api/client";

/**
 * صفحه جزئیات پرسنل (ویرایش + تکمیلی + انتساب).
 */
export default function EmployeeDetailPage() {
  return (
    <EmployeeDetailContent />
  );
}

function EmployeeDetailContent() {
  const params = useParams<{ id: string }>();
  const id = params.id;
  const { data: employee, isLoading, isError } = useEmployee(id);
  const canViewSensitive = useCanViewSensitiveData();

  if (isLoading) {
    return (
      <Container maxWidth="md">
        <Box sx={{ py: 6 }}>
          <Typography>در حال بارگذاری...</Typography>
        </Box>
      </Container>
    );
  }

  if (isError || !employee) {
    return (
      <Container maxWidth="md">
        <Box sx={{ py: 6 }}>
          <Alert severity="error">پرسنل یافت نشد یا دسترسی ندارید.</Alert>
        </Box>
      </Container>
    );
  }

  return (
    <Container maxWidth="md">
      <Box sx={{ py: 4, display: "grid", gap: 2 }}>
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          <Typography variant="h5" sx={{ fontWeight: 700, flexGrow: 1 }}>
            {employee.firstName} {employee.lastName}
          </Typography>
          <Chip
            label={employee.isActive ? "فعال" : "غیرفعال"}
            size="small"
            color={employee.isActive ? "success" : "default"}
          />
        </Box>
        <Paper sx={{ p: 3, display: "grid", gap: 1 }}>
          <InfoRow label="کد پرسنلی" value={employee.personnelCode} />
          <InfoRow
            label="سازمان"
            value={employee.organizationName ?? "—"}
          />
          <InfoRow
            label="کد ملی"
            value={maskSensitive(employee.nationalCode, canViewSensitive)}
          />
          <InfoRow
            label="شماره همراه"
            value={maskSensitive(employee.mobile, canViewSensitive)}
          />
          <InfoRow
            label="تاریخ تولد (شمسی)"
            value={formatBirthDate(employee.birthDate, canViewSensitive)}
          />
          <InfoRow
            label="سابقه حضور در حراست"
            value={
              employee.serviceYears !== null &&
              employee.serviceYears !== undefined
                ? `${employee.serviceYears} سال و ${employee.serviceMonths ?? 0} ماه`
                : "—"
            }
          />
          <InfoRow
            label="شماره ثبت شده در پیام رسان پژواک"
            value={maskSensitive(employee.pezhvakMobile, canViewSensitive)}
          />
          <InfoRow
            label="وضعیت شبکه پژواک"
            value={formatPezhvakStatus(employee.pezhvakIsActive)}
          />
        </Paper>
        <BasicInfoEditor
          employeeId={employee.id}
          firstName={employee.firstName}
          lastName={employee.lastName}
          nationalCode={employee.nationalCode}
          mobile={employee.mobile ?? ""}
        />
        <SupplementaryEditor
          employeeId={employee.id}
          birthDate={employee.birthDate ?? ""}
          serviceYears={employee.serviceYears}
          serviceMonths={employee.serviceMonths}
          pezhvakMobile={employee.pezhvakMobile ?? ""}
          pezhvakIsActive={employee.pezhvakIsActive}
        />
        <AssignmentSection
          employeeId={employee.id}
          organizationId={employee.organizationId}
        />
      </Box>
    </Container>
  );
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <Box sx={{ display: "flex", gap: 2 }}>
      <Typography variant="body2" color="text.secondary" sx={{ minWidth: 120 }}>
        {label}
      </Typography>
      <Typography variant="body2">{value}</Typography>
    </Box>
  );
}

/**
 * نمایش تاریخ تولد به قالب جلالی با حفظ ماسک داده حساس.
 */
function formatBirthDate(
  value: string | null | undefined,
  canView: boolean,
): string {
  if (!value) {
    return maskSensitive(value, canView);
  }
  const masked = maskSensitive(value, canView);
  if (masked !== value) {
    return masked;
  }
  const jalali = isoToJalali(value);
  return jalali ? formatJalali(jalali) : value;
}

/**
 * نمایش وضعیت فعال بودن شماره ثبت‌شده در شبکه پژواک.
 */
function formatPezhvakStatus(isActive: boolean | null | undefined): string {
  if (isActive === true) {
    return "فعال";
  }
  if (isActive === false) {
    return "غيرفعال";
  }
  return "تعيين نشده";
}

function BasicInfoEditor({
  employeeId,
  firstName,
  lastName,
  nationalCode,
  mobile,
}: {
  employeeId: string;
  firstName: string;
  lastName: string;
  nationalCode: string;
  mobile: string;
}) {
  const updateEmployee = useUpdateEmployee(employeeId);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<EmployeeBasicForm>({
    resolver: zodResolver(updateEmployeeBasicSchema),
    defaultValues: { firstName, lastName, nationalCode, mobile },
  });

  const onSubmit = (values: EmployeeBasicForm) => {
    updateEmployee.mutate({
      firstName: values.firstName,
      lastName: values.lastName,
      nationalCode: values.nationalCode,
      mobile: values.mobile,
    });
  };

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="subtitle1" sx={{ fontWeight: 700 }} gutterBottom>
        ویرایش اطلاعات پایه
      </Typography>
      <Box
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        sx={{ display: "grid", gap: 2 }}
      >
        <Box sx={{ display: "flex", gap: 2 }}>
          <TextField
            label="نام"
            fullWidth
            error={Boolean(errors.firstName)}
            helperText={errors.firstName?.message}
            {...register("firstName")}
          />
          <TextField
            label="نام خانوادگی"
            fullWidth
            error={Boolean(errors.lastName)}
            helperText={errors.lastName?.message}
            {...register("lastName")}
          />
        </Box>
        <Box sx={{ display: "flex", gap: 2 }}>
          <TextField
            label="کد ملی"
            fullWidth
            error={Boolean(errors.nationalCode)}
            helperText={errors.nationalCode?.message}
            {...register("nationalCode")}
          />
          <TextField
            label="شماره همراه"
            fullWidth
            error={Boolean(errors.mobile)}
            helperText={errors.mobile?.message}
            {...register("mobile")}
          />
        </Box>
        <Button
          type="submit"
          variant="outlined"
          disabled={updateEmployee.isPending}
        >
          ذخیره اطلاعات پایه
        </Button>
      </Box>
    </Paper>
  );
}

function SupplementaryEditor({
  employeeId,
  birthDate,
  serviceYears,
  serviceMonths,
  pezhvakMobile,
  pezhvakIsActive,
}: {
  employeeId: string;
  birthDate: string;
  serviceYears?: number | null;
  serviceMonths?: number | null;
  pezhvakMobile: string;
  pezhvakIsActive?: boolean | null;
}) {
  const updateSupplementary = useUpdateSupplementary(employeeId);
  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<SupplementaryForm>({
    resolver: zodResolver(supplementarySchema),
    defaultValues: {
      birthDate,
      serviceYears: serviceYears?.toString() ?? "",
      serviceMonths: serviceMonths?.toString() ?? "",
      pezhvakMobile,
      pezhvakIsActive:
        pezhvakIsActive === true
          ? "true"
          : pezhvakIsActive === false
            ? "false"
            : "",
    },
  });

  const onSubmit = (values: SupplementaryForm) => {
    updateSupplementary.mutate({
      birthDate: values.birthDate || null,
      serviceYears:
        values.serviceYears === "" || values.serviceYears === undefined
          ? null
          : Number(values.serviceYears),
      serviceMonths:
        values.serviceMonths === "" || values.serviceMonths === undefined
          ? null
          : Number(values.serviceMonths),
      pezhvakMobile: values.pezhvakMobile,
      pezhvakIsActive: values.pezhvakIsActive === "true",
    });
  };

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="subtitle1" sx={{ fontWeight: 700 }} gutterBottom>
        اطلاعات تکمیلی
      </Typography>
      <Box
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        sx={{ display: "grid", gap: 2 }}
      >
        <Controller
          name="birthDate"
          control={control}
          render={({ field }) => (
            <JalaliDatePicker
              label="تاریخ تولد"
              value={field.value || null}
              onChange={(iso) => field.onChange(iso ?? "")}
              error={Boolean(errors.birthDate)}
              helperText={errors.birthDate?.message}
            />
          )}
        />
        <Typography variant="body2" sx={{ fontWeight: 600 }}>
          سابقه حضور در حراست
        </Typography>
        <Box sx={{ display: "flex", gap: 2 }}>
          <TextField
            label="سال سابقه"
            fullWidth
            error={Boolean(errors.serviceYears)}
            helperText={errors.serviceYears?.message}
            {...register("serviceYears")}
          />
          <TextField
            label="ماه سابقه (۰ تا ۱۱)"
            fullWidth
            error={Boolean(errors.serviceMonths)}
            helperText={errors.serviceMonths?.message}
            {...register("serviceMonths")}
          />
        </Box>
        <TextField
          label="شماره ثبت شده در پیام رسان پژواک"
          fullWidth
          required
          error={Boolean(errors.pezhvakMobile)}
          helperText={errors.pezhvakMobile?.message}
          {...register("pezhvakMobile")}
        />
        <FormControl fullWidth error={Boolean(errors.pezhvakIsActive)} required>
          <InputLabel id="pezhvak-active-label">
            فعال بودن شماره در شبکه پژواک
          </InputLabel>
          <Controller
            name="pezhvakIsActive"
            control={control}
            render={({ field }) => (
              <Select
                labelId="pezhvak-active-label"
                label="فعال بودن شماره در شبکه پژواک"
                value={field.value}
                onChange={field.onChange}
              >
                <MenuItem value="true">فعال است</MenuItem>
                <MenuItem value="false">غيرفعال است</MenuItem>
              </Select>
            )}
          />
          {errors.pezhvakIsActive && (
            <Typography variant="caption" color="error">
              {errors.pezhvakIsActive.message}
            </Typography>
          )}
        </FormControl>
        <Button
          type="submit"
          variant="outlined"
          disabled={updateSupplementary.isPending}
        >
          ذخیره اطلاعات تکمیلی
        </Button>
      </Box>
    </Paper>
  );
}

function AssignmentSection({
  employeeId,
  organizationId,
}: {
  employeeId: string;
  organizationId: string;
}) {
  const { data: assignments } = useEmployeePosts(employeeId);
  const assignPost = useAssignPost(employeeId);
  const [dialogOpen, setDialogOpen] = React.useState(false);
  const [selectedPost, setSelectedPost] = React.useState<PostSummary | null>(
    null,
  );
  const [postSearch, setPostSearch] = React.useState("");
  const [postPage, setPostPage] = React.useState(1);
  const [isPrimary, setIsPrimary] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  // جستجوی سروری پست‌ها فقط در سازمان کارمند (ADR-004: هر سازمان درخت پست مستقل دارد).
  const postsQuery = useQuery({
    queryKey: [
      "posts",
      "employee-assign-options",
      organizationId,
      postSearch,
      postPage,
    ],
    enabled: dialogOpen && Boolean(organizationId),
    queryFn: () =>
      fetchPosts({
        organizationId,
        searchTerm: postSearch || undefined,
        isActive: true,
        page: postPage,
        pageSize: 20,
      }),
  });

  const assignedPostIds = new Set((assignments ?? []).map((a) => a.postId));
  const postOptions = (postsQuery.data?.items ?? []).filter(
    (post) => post.isActive && !assignedPostIds.has(post.id),
  );

  const onAssign = () => {
    if (!selectedPost) {
      setError("انتخاب پست الزامی است.");
      return;
    }
    setError(null);
    assignPost.mutate(
      { postId: selectedPost.id, isPrimary },
      {
        onSuccess: () => {
          setDialogOpen(false);
          setSelectedPost(null);
          setPostSearch("");
          setPostPage(1);
        },
        onError: (e) => {
          setError(e instanceof ApiError ? e.message : "خطا در انتساب.");
        },
      },
    );
  };

  return (
    <Paper sx={{ p: 3 }}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 2,
        }}
      >
        <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
          پست‌های منتسب
        </Typography>
        <Button variant="outlined" onClick={() => setDialogOpen(true)}>
          انتساب جدید
        </Button>
      </Box>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>کد پست</TableCell>
            <TableCell>عنوان پست</TableCell>
            <TableCell>اصلی</TableCell>
            <TableCell>عملیات</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {(assignments ?? []).map((a) => (
            <AssignmentRow
              key={a.postId}
              employeeId={employeeId}
              assignment={a}
            />
          ))}
        </TableBody>
      </Table>
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}>
        <DialogTitle>انتساب به پست</DialogTitle>
        <DialogContent sx={{ display: "grid", gap: 2, pt: 2, minWidth: 320 }}>
          <Autocomplete
            options={postOptions}
            value={selectedPost}
            onChange={(_, value) => setSelectedPost(value)}
            onInputChange={(_, value, reason) => {
              if (reason === "input" || reason === "clear") {
                setPostSearch(value);
                setPostPage(1);
              }
            }}
            getOptionLabel={(option) => `${option.code} - ${option.title}`}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            filterOptions={(options) => options}
            loading={postsQuery.isFetching}
            disabled={!organizationId || assignPost.isPending}
            loadingText="در حال جستجو..."
            noOptionsText={
              postsQuery.isError ? "خطا در دریافت پست‌ها" : "پستی یافت نشد"
            }
            renderInput={(params) => (
              <TextField
                {...params}
                label="پست (جستجوی کد یا عنوان)"
                helperText="فقط پست‌های فعالِ همان سازمان کارمند قابل انتخاب‌اند."
              />
            )}
          />
          {postsQuery.isError && (
            <Alert
              severity="error"
              action={
                <Button onClick={() => postsQuery.refetch()}>تلاش مجدد</Button>
              }
            >
              دریافت پست‌های سازمان ناموفق بود.
            </Alert>
          )}
          <FormControlLabel
            control={
              <Checkbox
                checked={isPrimary}
                onChange={(e) => setIsPrimary(e.target.checked)}
              />
            }
            label="انتساب اصلی"
          />
          {error && <Alert severity="error">{error}</Alert>}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>انصراف</Button>
          <Button
            variant="contained"
            disabled={assignPost.isPending || selectedPost === null}
            onClick={onAssign}
          >
            انتساب
          </Button>
        </DialogActions>
      </Dialog>
    </Paper>
  );
}

function AssignmentRow({
  employeeId,
  assignment,
}: {
  employeeId: string;
  assignment: {
    postId: string;
    postCode: string;
    postTitle: string;
    isPrimary: boolean;
    toDate?: string | null;
  };
}) {
  const endAssignment = useEndAssignment(employeeId, assignment.postId);
  const [error, setError] = React.useState<string | null>(null);

  if (assignment.toDate) {
    return null;
  }

  return (
    <TableRow>
      <TableCell>{assignment.postCode}</TableCell>
      <TableCell>{assignment.postTitle}</TableCell>
      <TableCell>{assignment.isPrimary ? "بله" : "خیر"}</TableCell>
      <TableCell>
        <Button
          size="small"
          color="warning"
          disabled={endAssignment.isPending}
          onClick={() => {
            setError(null);
            const today = new Date().toISOString().slice(0, 10);
            endAssignment.mutate(today, {
              onError: (e) => {
                setError(e instanceof ApiError ? e.message : "خطا.");
              },
            });
          }}
        >
          پایان انتساب
        </Button>
        {error && (
          <Typography variant="caption" color="error" sx={{ display: "block" }}>
            {error}
          </Typography>
        )}
      </TableCell>
    </TableRow>
  );
}


