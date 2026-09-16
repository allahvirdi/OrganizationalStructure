"use client";

import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { z } from "zod";
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
import RequireAuth from "../../../src/components/RequireAuth";
import { useMe } from "../../../src/features/auth/useAuth";
import { useCreatePost } from "../../../src/features/posts/usePosts";
import { postSchema } from "../../../src/features/posts/schemas";
import { ApiError } from "../../../src/lib/api/client";

const createSchema = postSchema.extend({
  parentId: z.string().optional().or(z.literal("")),
});

type CreateForm = z.infer<typeof createSchema>;

/**
 * صفحه ایجاد پست جدید.
 */
export default function NewPostPage() {
  return (
    <RequireAuth>
      <NewPostContent />
    </RequireAuth>
  );
}

function NewPostContent() {
  const router = useRouter();
  const { data: user } = useMe();
  const createPost = useCreatePost();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateForm>({ resolver: zodResolver(createSchema) });

  const onSubmit = (values: CreateForm) => {
    if (!user?.organizationId) {
      return;
    }
    createPost.mutate(
      {
        organizationId: user.organizationId,
        code: values.code,
        title: values.title,
        description: values.description || null,
        parentId: values.parentId || null,
      },
      {
        onSuccess: (id) => router.replace(`/posts/${id}`),
      },
    );
  };

  const serverError =
    createPost.error instanceof ApiError ? createPost.error.message : null;

  return (
    <Container maxWidth="sm">
      <Box sx={{ py: 4 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
          پست جدید
        </Typography>
        <Paper sx={{ p: 3 }}>
          <Box
            component="form"
            onSubmit={handleSubmit(onSubmit)}
            sx={{ display: "grid", gap: 2 }}
          >
            <TextField
              label="کد پست"
              fullWidth
              error={Boolean(errors.code)}
              helperText={errors.code?.message}
              {...register("code")}
            />
            <TextField
              label="عنوان پست"
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
              error={Boolean(errors.description)}
              helperText={errors.description?.message}
              {...register("description")}
            />
            {serverError && <Alert severity="error">{serverError}</Alert>}
            <Button
              type="submit"
              variant="contained"
              disabled={createPost.isPending}
            >
              ثبت پست
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}
