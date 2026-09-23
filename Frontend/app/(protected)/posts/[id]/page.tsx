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
import {
  useMovePost,
  usePost,
  useUpdatePost,
} from "../../../../src/features/posts/usePosts";
import {
  postSchema,
  type PostForm,
} from "../../../../src/features/posts/schemas";
import { ApiError } from "../../../../src/lib/api/client";
import {
  useAssignAuthority,
  useAuthorities,
} from "../../../../src/features/authorities/useAuthorities";
import type { Authority } from "../../../../src/features/authorities/api";
import { endAuthorityAssignment } from "../../../../src/features/authorities/api";
import {
  useAssignResponsibility,
  useResponsibilities,
} from "../../../../src/features/responsibilities/useResponsibilities";
import type { Responsibility } from "../../../../src/features/responsibilities/api";
import { endResponsibilityAssignment } from "../../../../src/features/responsibilities/api";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { fetchPosts } from "../../../../src/features/posts/api";
import type { PostSummary } from "../../../../src/features/org-chart/api";
import { postsQueryKey } from "../../../../src/features/posts/usePosts";
import { Autocomplete, Tooltip } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import AddIcon from "@mui/icons-material/Add";

/**
 * صفحه جزئیات و ویرایش پست.
 */
export default function PostDetailPage() {
  return (
    <PostDetailContent />
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
    organizationId: string;
    code: string;
    title: string;
    description?: string | null;
    parentId?: string | null;
    isActive: boolean;
    hasSigningAuthority: boolean;
    responsibilities: Array<{
      id: string;
      responsibilityCode: string;
      responsibilityTitle: string;
      startDate?: string | null;
      endDate?: string | null;
      isActive: boolean;
    }>;
    authorities: Array<{
      id: string;
      authorityCode: string;
      authorityTitle: string;
      startDate?: string | null;
      endDate?: string | null;
      isActive: boolean;
    }>;
  };
}) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const updatePost = useUpdatePost(post.id);
  const movePost = useMovePost(post.id);
  const [moveOpen, setMoveOpen] = React.useState(false);
  const [newParent, setNewParent] = React.useState<PostSummary | null>(null);
  const [parentSearchTerm, setParentSearchTerm] = React.useState("");
  const [parentPage, setParentPage] = React.useState(1);
  const [assignMessage, setAssignMessage] = React.useState<string | null>(null);

  // --- Parent search for move dialog (same pattern as new/page.tsx) ---
  const parents = useQuery({
    queryKey: ["posts", "parent-options", post.organizationId, parentSearchTerm, parentPage],
    enabled: moveOpen && Boolean(post.organizationId),
    queryFn: () =>
      fetchPosts({
        organizationId: post.organizationId,
        searchTerm: parentSearchTerm || undefined,
        page: parentPage,
        pageSize: 10,
      }),
  });
  const [assignError, setAssignError] = React.useState<string | null>(null);
  const [endingAssignment, setEndingAssignment] = React.useState(false);

  // --- Responsibility assignment ---
  const assignResponsibility = useAssignResponsibility();
  const responsibilitiesList = useResponsibilities({ isActive: true, page: 1, pageSize: 50 });
  const [selectedResponsibility, setSelectedResponsibility] = React.useState<
    Pick<Responsibility, "code" | "title"> | null
  >(null);

  // --- Authority assignment ---
  const assignAuthority = useAssignAuthority();
  const authoritiesList = useAuthorities({ isActive: true, page: 1, pageSize: 50 });
  const [selectedAuthority, setSelectedAuthority] = React.useState<
    Pick<Authority, "code" | "title"> | null
  >(null);

  const invalidatePost = () => {
    queryClient.invalidateQueries({ queryKey: [...postsQueryKey, "detail", post.id] });
  };

  const handleAssignResponsibility = () => {
    if (!selectedResponsibility) return;
    setAssignMessage(null);
    setAssignError(null);
    assignResponsibility.mutate(
      { responsibilityCode: selectedResponsibility.code, postId: post.id },
      {
        onSuccess: () => {
          setSelectedResponsibility(null);
          setAssignMessage("مسئولیت با موفقیت اضافه شد.");
          invalidatePost();
        },
        onError: (e) => {
          setAssignError(e instanceof ApiError ? e.message : "خطا در انتساب مسئولیت.");
        },
      },
    );
  };

  const handleAssignAuthority = () => {
    if (!selectedAuthority) return;
    setAssignMessage(null);
    setAssignError(null);
    assignAuthority.mutate(
      { authorityCode: selectedAuthority.code, postId: post.id },
      {
        onSuccess: () => {
          setSelectedAuthority(null);
          setAssignMessage("حق امضا با موفقیت اضافه شد.");
          invalidatePost();
        },
        onError: (e) => {
          setAssignError(e instanceof ApiError ? e.message : "خطا در انتساب حق امضا.");
        },
      },
    );
  };

  const handleEndResponsibility = async (assignmentId: string) => {
    setAssignMessage(null);
    setAssignError(null);
    setEndingAssignment(true);
    try {
      const today = new Date().toISOString().split("T")[0];
      await endResponsibilityAssignment(assignmentId, today);
      setAssignMessage("مسئولیت با موفقیت حذف شد.");
      invalidatePost();
    } catch (e) {
      setAssignError(e instanceof ApiError ? e.message : "خطا در حذف مسئولیت.");
    } finally {
      setEndingAssignment(false);
    }
  };

  const handleEndAuthority = async (assignmentId: string) => {
    setAssignMessage(null);
    setAssignError(null);
    setEndingAssignment(true);
    try {
      const today = new Date().toISOString().split("T")[0];
      await endAuthorityAssignment(assignmentId, today);
      setAssignMessage("حق امضا با موفقیت حذف شد.");
      invalidatePost();
    } catch (e) {
      setAssignError(e instanceof ApiError ? e.message : "خطا در حذف حق امضا.");
    } finally {
      setEndingAssignment(false);
    }
  };
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
    movePost.mutate(newParent?.id ?? null, {
      onSuccess: () => {
        setMoveOpen(false);
        setNewParent(null);
        setParentSearchTerm("");
        setParentPage(1);
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
          <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap", mb: 2 }}>
            {post.responsibilities.map((r) => (
              <Chip
                key={r.id}
                label={r.responsibilityTitle}
                size="small"
                variant="outlined"
                disabled={endingAssignment}
                onDelete={() => handleEndResponsibility(r.id)}
                deleteIcon={
                  <Tooltip title="حذف مسئولیت">
                    <CloseIcon fontSize="small" />
                  </Tooltip>
                }
              />
            ))}
          </Box>
          <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
            <Autocomplete
              size="small"
              sx={{ minWidth: 250 }}
              options={(responsibilitiesList.data?.items ?? []).map((item) => ({
                code: item.code,
                title: item.title,
              }))}
              value={selectedResponsibility}
              onChange={(_, value) => setSelectedResponsibility(value)}
              getOptionLabel={(option) => option.title}
              isOptionEqualToValue={(option, value) => option.code === value.code}
              loading={responsibilitiesList.isFetching}
              loadingText="در حال دریافت..."
              noOptionsText="مسئولیتی یافت نشد"
              renderInput={(params) => (
                <TextField {...params} label="افزودن مسئولیت" size="small" />
              )}
            />
            <Button
              size="small"
              variant="outlined"
              startIcon={<AddIcon />}
              disabled={!selectedResponsibility || assignResponsibility.isPending}
              onClick={handleAssignResponsibility}
            >
              افزودن
            </Button>
          </Box>
          <Divider sx={{ my: 2 }} />
          <Typography variant="subtitle1" sx={{ fontWeight: 700 }} gutterBottom>
            حق امضاها
          </Typography>
          {post.authorities.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              حق امضایی ثبت نشده است.
            </Typography>
          )}
          <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap", mb: 2 }}>
            {post.authorities.map((a) => (
              <Chip
                key={a.id}
                label={a.authorityTitle}
                size="small"
                color="primary"
                variant="outlined"
                disabled={endingAssignment}
                onDelete={() => handleEndAuthority(a.id)}
                deleteIcon={
                  <Tooltip title="حذف حق امضا">
                    <CloseIcon fontSize="small" />
                  </Tooltip>
                }
              />
            ))}
          </Box>
          <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
            <Autocomplete
              size="small"
              sx={{ minWidth: 250 }}
              options={(authoritiesList.data?.items ?? []).map((item) => ({
                code: item.code,
                title: item.title,
              }))}
              value={selectedAuthority}
              onChange={(_, value) => setSelectedAuthority(value)}
              getOptionLabel={(option) => option.title}
              isOptionEqualToValue={(option, value) => option.code === value.code}
              loading={authoritiesList.isFetching}
              loadingText="در حال دریافت..."
              noOptionsText="حق امضایی یافت نشد"
              renderInput={(params) => (
                <TextField {...params} label="افزودن حق امضا" size="small" />
              )}
            />
            <Button
              size="small"
              variant="outlined"
              startIcon={<AddIcon />}
              disabled={!selectedAuthority || assignAuthority.isPending}
              onClick={handleAssignAuthority}
            >
              افزودن
            </Button>
          </Box>
          {assignMessage && (
            <Alert severity="success" sx={{ mt: 2 }}>{assignMessage}</Alert>
          )}
          {assignError && (
            <Alert severity="error" sx={{ mt: 2 }}>{assignError}</Alert>
          )}
        </Paper>
        <Dialog open={moveOpen} onClose={() => setMoveOpen(false)} maxWidth="sm" fullWidth>
          <DialogTitle>جابجایی پست در درخت</DialogTitle>
          <DialogContent>
            <Autocomplete
              sx={{ mt: 1 }}
              options={parents.data?.items ?? []}
              value={newParent}
              onChange={(_, value) => setNewParent(value)}
              getOptionLabel={(option) => `${option.code} — ${option.title}`}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              onInputChange={(_, value) => {
                setParentSearchTerm(value);
                setParentPage(1);
              }}
              loading={parents.isFetching}
              loadingText="در حال دریافت پستها..."
              noOptionsText={
                parents.isError ? "خطا در دریافت پستها" : "پستی یافت نشد"
              }
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="والد جدید (خالی یعنی ریشه)"
                  helperText="پست موردنظر را جستجو و انتخاب کنید."
                />
              )}
            />
            {parents.isError && (
              <Box sx={{ mt: 1, display: "flex", alignItems: "center", gap: 1 }}>
                <Typography variant="caption" color="error">
                  خطا در دریافت پستها
                </Typography>
                <Button size="small" onClick={() => parents.refetch()}>تلاش مجدد</Button>
              </Box>
            )}
            <Box sx={{ mt: 2, display: "flex", alignItems: "center", gap: 1, justifyContent: "center" }}>
              <Button
                size="small"
                disabled={parentPage === 1 || parents.isFetching}
                onClick={() => setParentPage((page) => page - 1)}
              >
                قبلی
              </Button>
              <Typography variant="caption">صفحه {parentPage}</Typography>
              <Button
                size="small"
                disabled={
                  parents.isFetching ||
                  !parents.data ||
                  parentPage >= parents.data.totalPages
                }
                onClick={() => setParentPage((page) => page + 1)}
              >
                بعدی
              </Button>
            </Box>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => { setMoveOpen(false); setNewParent(null); setParentSearchTerm(""); setParentPage(1); }}>انصراف</Button>
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
