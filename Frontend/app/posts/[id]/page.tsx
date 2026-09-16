"use client";

import * as React from "react";
import { useParams, useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Alert,
  Box,
  Button,
  Chip,
  Container,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import RequireAuth from "../../../src/components/RequireAuth";
import {
  useMovePost,
  usePost,
  useUpdatePost,
} from "../../../src/features/posts/usePosts";
import {
  postSchema,
  type PostForm,
} from "../../../src/features/posts/schemas";
import { ApiError } from "../../../src/lib/api/client";

/**
 * صفحه جزئیات و ویرایش پست.
 */
export default function PostDetailPage() {
  return (
    <RequireAuth>
      <PostDetailContent />
    </RequireAuth>
  );
}

function PostDetailContent() {
  const params = useParams<{ id: string }>();
  const id = params.id;
  const { data: post, isLoading, isError } = usePost(id);

  if (isLoading) {
    return (
      <Container maxWidth="md">
        <Box sx={{ py: 6 }}>
          <Typography>در حال بارگذاری...</Typography>
        </Box>
      </Container>
    );
  }

  if (isError || !post) {
    return (
      <Container maxWidth="md">
        <Box sx={{ py: 6 }}>
          <Alert severity="error">پست یافت نشد یا دسترسی ندارید.</Alert>
        </Box>
      </Container>
    );
  }

  return <PostEditor key={post.id} post={post} />;
}

function PostEditor({
  post,
}: {
  post: {
    id: string;
    code: string;
    title: string;
    description?: string | null;
    parentId?: string | null;
    isActive: boolean;
    hasSigningAuthority: boolean;
    responsibilities: Array<{
      responsibilityCode: string;
      responsibilityTitle: string;
      isActive: boolean;
    }>;
    authorities: Array<{
      authorityCode: string;
      authorityTitle: string;
      isActive: boolean;
    }>;
  };
}) {
  const router = useRouter();
  const updatePost = useUpdatePost(post.id);
  const movePost = useMovePost(post.id);
  const [moveOpen, setMoveOpen] = React.useState(false);
  const [newParentId, setNewParentId] = React.useState("");
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<PostForm>({
    resolver: zodResolver(postSchema),
    defaultValues: {
      code: post.code,
      title: post.title,
      description: post.description ?? "",
    },
  });

  const onSubmit = (values: PostForm) => {
    updatePost.mutate(
      {
        code: values.code,
        title: values.title,
        description: values.description || null,
      },
      { onSuccess: () => router.refresh() },
    );
  };

  const onMove = () => {
    movePost.mutate(newParentId.trim() === "" ? null : newParentId.trim(), {
      onSuccess: () => {
        setMoveOpen(false);
        router.refresh();
      },
    });
  };

  const serverError = ((): string | null => {
    const error = updatePost.error ?? movePost.error;
    return error instanceof ApiError ? error.message : null;
  })();

  return (
    <Container maxWidth="md">
      <Box sx={{ py: 4, display: "grid", gap: 2 }}>
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          <Typography variant="h5" sx={{ fontWeight: 700, flexGrow: 1 }}>
            {post.title}
          </Typography>
          {post.hasSigningAuthority && (
            <Chip label="صاحب امضا" size="small" color="primary" />
          )}
          <Chip
            label={post.isActive ? "فعال" : "غیرفعال"}
            size="small"
            color={post.isActive ? "success" : "default"}
          />
        </Box>
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
              {...register("description")}
            />
            {serverError && <Alert severity="error">{serverError}</Alert>}
            <Box sx={{ display: "flex", gap: 1 }}>
              <Button
                type="submit"
                variant="contained"
                disabled={updatePost.isPending}
              >
                ذخیره
              </Button>
              <Button variant="outlined" onClick={() => setMoveOpen(true)}>
                جابجایی در درخت
              </Button>
            </Box>
          </Box>
        </Paper>
        <Paper sx={{ p: 3 }}>
          <Typography variant="subtitle1" sx={{ fontWeight: 700 }} gutterBottom>
            مسئولیت‌ها
          </Typography>
          {post.responsibilities.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              مسئولیتی ثبت نشده است.
            </Typography>
          )}
          <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap" }}>
            {post.responsibilities.map((r) => (
              <Chip
                key={r.responsibilityCode}
                label={r.responsibilityTitle}
                size="small"
                variant="outlined"
              />
            ))}
          </Box>
          <Divider sx={{ my: 2 }} />
          <Typography variant="subtitle1" sx={{ fontWeight: 700 }} gutterBottom>
            اختیارها
          </Typography>
          {post.authorities.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              اختیاری ثبت نشده است.
            </Typography>
          )}
          <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap" }}>
            {post.authorities.map((a) => (
              <Chip
                key={a.authorityCode}
                label={a.authorityTitle}
                size="small"
                color="primary"
                variant="outlined"
              />
            ))}
          </Box>
        </Paper>
        <Dialog open={moveOpen} onClose={() => setMoveOpen(false)}>
          <DialogTitle>جابجایی پست در درخت</DialogTitle>
          <DialogContent>
            <TextField
              label="شناسه والد جدید (خالی یعنی ریشه)"
              fullWidth
              sx={{ mt: 1 }}
              value={newParentId}
              onChange={(e) => setNewParentId(e.target.value)}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setMoveOpen(false)}>انصراف</Button>
            <Button
              variant="contained"
              disabled={movePost.isPending}
              onClick={onMove}
            >
              جابجایی
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Container>
  );
}
