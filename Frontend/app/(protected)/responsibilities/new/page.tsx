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
import { useCreateResponsibility } from "../../../../src/features/responsibilities/useResponsibilities";
import {
  responsibilitySchema,
  type ResponsibilityForm,
} from "../../../../src/features/responsibilities/schemas";
import { ApiError } from "../../../../src/lib/api/client";

/**
 * صفحه تعریف مسئولیت جدید.
 */
export default function NewResponsibilityPage() {
  return (
    <NewResponsibilityContent />
  );
}

function NewResponsibilityContent() {
  const router = useRouter();
  const createItem = useCreateResponsibility();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ResponsibilityForm>({ resolver: zodResolver(responsibilitySchema) });

  const onSubmit = (values: ResponsibilityForm) => {
    createItem.mutate(
      {
        title: values.title,
        description: values.description || null,
      },
      {
        onSuccess: (newId) =>
          router.replace(
            `/responsibilities/${encodeURIComponent(newId)}`,
          ),
      },
    );
  };

  const serverError =
    createItem.error instanceof ApiError ? createItem.error.message : null;

  return (
    <Container maxWidth="sm">
      <Box sx={{ py: 4 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
          مسئولیت جدید
        </Typography>
        <Paper sx={{ p: 3 }}>
          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            sx={{ display: "grid", gap: 2 }}
          >
            <TextField
              label="عنوان"
              fullWidth
              error={Boolean(errors.title)}
              helperText={errors.title?.message}
              {...register("title")}
            />
            <TextField
              label="شرح (اختیاری)"
              fullWidth
              multiline
              rows={2}
              {...register("description")}
            />
            {serverError && <Alert severity="error">{serverError}</Alert>}
            <Button
              type="submit"
              variant="contained"
              disabled={createItem.isPending}
            >
              ثبت مسئولیت
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}
