"use client";

import * as React from "react";
import { useParams } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  employeeSchema,
  type EmployeeForm,
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
  FormControlLabel,
  Paper,
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
            label="کد ملی"
            value={maskSensitive(employee.nationalCode, canViewSensitive)}
          />
          <InfoRow
            label="موبایل"
            value={maskSensitive(employee.mobile, canViewSensitive)}
          />
          <InfoRow
            label="تاریخ تولد"
            value={maskSensitive(employee.birthDate, canViewSensitive)}
          />
          <InfoRow
            label="سابقه حراست"
            value={
              employee.serviceYears !== null &&
              employee.serviceYears !== undefined
                ? `${employee.serviceYears} سال و ${employee.serviceMonths ?? 0} ماه`
                : "—"
            }
          />
          <InfoRow
            label="موبایل پژواک"
            value={maskSensitive(employee.pezhvakMobile, canViewSensitive)}
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
        />
        <AssignmentSection employeeId={employee.id} />
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
  } = useForm<EmployeeForm>({
    resolver: zodResolver(employeeSchema),
    defaultValues: { firstName, lastName, nationalCode, mobile },
  });

  const onSubmit = (values: EmployeeForm) => {
    updateEmployee.mutate({
      firstName: values.firstName,
      lastName: values.lastName,
      nationalCode: values.nationalCode,
      mobile: values.mobile || null,
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
            label="موبایل"
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
}: {
  employeeId: string;
  birthDate: string;
  serviceYears?: number | null;
  serviceMonths?: number | null;
  pezhvakMobile: string;
}) {
  const updateSupplementary = useUpdateSupplementary(employeeId);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<SupplementaryForm>({
    resolver: zodResolver(supplementarySchema),
    defaultValues: {
      birthDate,
      serviceYears: serviceYears?.toString() ?? "",
      serviceMonths: serviceMonths?.toString() ?? "",
      pezhvakMobile,
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
      pezhvakMobile: values.pezhvakMobile || null,
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
        <TextField
          label="تاریخ تولد (اختیاری)"
          placeholder="1360-05-12"
          fullWidth
          {...register("birthDate")}
        />
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
          label="موبایل پژواک (اختیاری)"
          fullWidth
          {...register("pezhvakMobile")}
        />
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

function AssignmentSection({ employeeId }: { employeeId: string }) {
  const { data: assignments } = useEmployeePosts(employeeId);
  const assignPost = useAssignPost(employeeId);
  const [dialogOpen, setDialogOpen] = React.useState(false);
  const [postId, setPostId] = React.useState("");
  const [isPrimary, setIsPrimary] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  const onAssign = () => {
    setError(null);
    assignPost.mutate(
      { postId: postId.trim(), isPrimary },
      {
        onSuccess: () => {
          setDialogOpen(false);
          setPostId("");
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
          <TextField
            label="شناسه پست"
            fullWidth
            sx={{ mt: 1 }}
            value={postId}
            onChange={(e) => setPostId(e.target.value)}
          />
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
            disabled={assignPost.isPending || postId.trim() === ""}
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


