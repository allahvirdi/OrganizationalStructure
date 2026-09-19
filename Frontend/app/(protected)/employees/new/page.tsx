"use client";

import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Alert,
  Box,
  Button,
  Container,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useCreateEmployee } from "../../../../src/features/employees/useEmployees";
import {
  employeeSchema,
  type EmployeeForm,
} from "../../../../src/features/employees/schemas";
import { ApiError } from "../../../../src/lib/api/client";

/**
 * صفحه ثبت پرسنل جدید.
 */
export default function NewEmployeePage() {
  return (
    <NewEmployeeContent />
  );
}

function NewEmployeeContent() {
  const router = useRouter();
  const createEmployee = useCreateEmployee();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<EmployeeForm>({ resolver: zodResolver(employeeSchema) });

  const onSubmit = (values: EmployeeForm) => {
    createEmployee.mutate(
      {
        personnelCode: values.personnelCode,
        firstName: values.firstName,
        lastName: values.lastName,
        nationalCode: values.nationalCode,
        mobile: values.mobile,
      },
      {
        onSuccess: (id) => router.replace(`/employees/${id}`),
      },
    );
  };

  const serverError =
    createEmployee.error instanceof ApiError
      ? createEmployee.error.message
      : null;

  return (
    <Container maxWidth="sm">
      <Box sx={{ py: 4 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
          پرسنل جدید
        </Typography>
        <Paper sx={{ p: 3 }}>
          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            sx={{ display: "grid", gap: 2 }}
          >
            <TextField
              label="کد پرسنلی (۸ رقم)"
              fullWidth
              error={Boolean(errors.personnelCode)}
              helperText={errors.personnelCode?.message}
              {...register("personnelCode")}
            />
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
            {serverError && <Alert severity="error">{serverError}</Alert>}
            <Button
              type="submit"
              variant="contained"
              disabled={createEmployee.isPending}
            >
              ثبت پرسنل
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}
