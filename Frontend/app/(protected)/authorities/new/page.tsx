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
import { useCreateAuthority } from "../../../../src/features/authorities/useAuthorities";
import {
  authoritySchema,
  type AuthorityForm,
} from "../../../../src/features/authorities/schemas";
import { ApiError } from "../../../../src/lib/api/client";

/**
 * صفحه تعریف اختیار جدید.
 */
export default function NewAuthorityPage() {
  return (
    <NewAuthorityContent />
  );
}

function NewAuthorityContent() {
  const router = useRouter();
  const createItem = useCreateAuthority();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<AuthorityForm>({ resolver: zodResolver(authoritySchema) });

  const onSubmit = (values: AuthorityForm) => {
    createItem.mutate(
      {
        code: values.code,
        title: values.title,
        description: values.description || null,
      },
      {
        onSuccess: () =>
          router.replace(`/authorities/${encodeURIComponent(values.code)}`),
      },
    );
  };

  const serverError =
    createItem.error instanceof ApiError ? createItem.error.message : null;

  return (
    <Container maxWidth="sm">
      <Box sx={{ py: 4 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
          اختیار جدید
        </Typography>
        <Paper sx={{ p: 3 }}>
          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            sx={{ display: "grid", gap: 2 }}
          >
            <TextField
              label="کد"
              fullWidth
              dir="ltr"
              error={Boolean(errors.code)}
              helperText={errors.code?.message}
              {...register("code")}
            />
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
              ثبت اختیار
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}
