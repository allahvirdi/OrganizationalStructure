"use client";

import * as React from "react";
import { useParams } from "next/navigation";
import { useInfiniteQuery, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  Alert,
  Autocomplete,
  Box,
  Button,
  Container,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
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
  useAssignResponsibility,
  useEndResponsibilityAssignment,
  useResponsibility,
} from "../../../../src/features/responsibilities/useResponsibilities";
import type { ResponsibilityAssignment } from "../../../../src/features/responsibilities/api";
import { fetchResponsibilityAssignments } from "../../../../src/features/responsibilities/api";
import { ApiError } from "../../../../src/lib/api/client";
import { fetchPosts } from "../../../../src/features/posts/api";
import type { PostSummary } from "../../../../src/features/org-chart/api";
import type { OrganizationOption } from "../../../../src/features/organizations/api";
import { useOrganizations } from "../../../../src/features/organizations/useOrganizations";
import { useDebouncedValue } from "../../../../src/lib/hooks/useDebouncedValue";

/** اندازهٔ صفحهٔ گزینه‌های پست در انتخابگر انتساب. */
const postOptionsPageSize = 25;

/**
 * صفحه جزئیات مسئولیت + انتساب به پست.
 */
export default function ResponsibilityDetailPage() {
  return (
    <ResponsibilityDetailContent />
  );
}

function ResponsibilityDetailContent() {
  const params = useParams<{ code: string }>();
  const code = decodeURIComponent(params.code);
  const { data: item, isLoading, isError } = useResponsibility(code);

  if (isLoading) {
    return (
      <Container maxWidth="md">
        <Box sx={{ py: 6 }}>
          <Typography>در حال بارگذاری...</Typography>
        </Box>
      </Container>
    );
  }

  if (isError || !item) {
    return (
      <Container maxWidth="md">
        <Box sx={{ py: 6 }}>
          <Alert severity="error">مسئولیت یافت نشد یا دسترسی ندارید.</Alert>
        </Box>
      </Container>
    );
  }

  return (
    <Container maxWidth="md">
      <Box sx={{ py: 4, display: "grid", gap: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>
          {item.title}
        </Typography>
        <AssignmentSection code={item.code} />
      </Box>
    </Container>
  );
}

function AssignmentSection({ code }: { code: string }) {
  const queryClient = useQueryClient();
  const assignmentsQuery = useQuery({
    queryKey: ["responsibilities", "assignments", code],
    queryFn: () => fetchResponsibilityAssignments(code),
  });
  const assign = useAssignResponsibility();
  const [dialogOpen, setDialogOpen] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  // --- انتخاب سازمان (محدود به درخت دسترسی کاربر) ---
  const [selectedOrganization, setSelectedOrganization] =
    React.useState<OrganizationOption | null>(null);
  const [orgSearchTerm, setOrgSearchTerm] = React.useState("");
  const debouncedOrgSearchTerm = useDebouncedValue(orgSearchTerm);
  const organizations = useOrganizations({ searchTerm: debouncedOrgSearchTerm });

  // --- انتخاب پست (جستجوی سروری + بارگذاری تدریجی در سازمان انتخاب‌شده) ---
  const [selectedPost, setSelectedPost] = React.useState<PostSummary | null>(null);
  const [postSearchTerm, setPostSearchTerm] = React.useState("");
  const debouncedPostSearchTerm = useDebouncedValue(postSearchTerm);
  const postsQuery = useInfiniteQuery({
    queryKey: [
      "posts",
      "assign-options",
      selectedOrganization?.id,
      debouncedPostSearchTerm,
    ],
    enabled: dialogOpen && Boolean(selectedOrganization?.id),
    initialPageParam: 1,
    queryFn: ({ pageParam }) =>
      fetchPosts({
        organizationId: selectedOrganization!.id,
        searchTerm: debouncedPostSearchTerm || undefined,
        isActive: true,
        page: pageParam,
        pageSize: postOptionsPageSize,
      }),
    getNextPageParam: (lastPage) =>
      lastPage.page < lastPage.totalPages ? lastPage.page + 1 : undefined,
  });

  const postOptions = postsQuery.data?.pages.flatMap((page) => page.items) ?? [];
  const postTotalCount = postsQuery.data?.pages[0]?.totalCount ?? 0;

  const onAssign = () => {
    setError(null);
    if (!selectedPost) return;
    assign.mutate(
      { responsibilityCode: code, postId: selectedPost.id },
      {
        onSuccess: () => {
          setDialogOpen(false);
          setSelectedOrganization(null);
          setSelectedPost(null);
          setOrgSearchTerm("");
          setPostSearchTerm("");
          queryClient.invalidateQueries({
            queryKey: ["responsibilities", "assignments", code],
          });
        },
        onError: (e) => {
          setError(e instanceof ApiError ? e.message : "خطا در انتساب.");
        },
      },
    );
  };

  const closeDialog = () => {
    setDialogOpen(false);
    setSelectedOrganization(null);
    setSelectedPost(null);
    setOrgSearchTerm("");
    setPostSearchTerm("");
    setError(null);
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
          انتساب به پست
        </Typography>
        <Button variant="outlined" onClick={() => setDialogOpen(true)}>
          انتساب جدید
        </Button>
      </Box>
      {assignmentsQuery.isError && (
        <Alert severity="error">خطا در دریافت انتساب‌ها.</Alert>
      )}
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>کد پست</TableCell>
            <TableCell>عنوان پست</TableCell>
            <TableCell>عملیات</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {(assignmentsQuery.data ?? []).map((a) => (
            <AssignmentRow key={a.id} assignment={a} code={code} />
          ))}
        </TableBody>
      </Table>
      <Dialog open={dialogOpen} onClose={closeDialog} maxWidth="sm" fullWidth>
        <DialogTitle>انتساب مسئولیت به پست</DialogTitle>
        <DialogContent sx={{ minWidth: 360, display: "grid", gap: 2, pt: "8px !important" }}>
          {/* انتخاب سازمان — دسترسی درختی: ستاد→استان‌ها، استان→مناطق، منطقه→فقط خودش */}
          <Autocomplete<OrganizationOption>
            options={organizations.data ?? []}
            value={selectedOrganization}
            onChange={(_, value) => {
              setSelectedOrganization(value);
              setSelectedPost(null);
              setPostSearchTerm("");
            }}
            getOptionLabel={(option) => option.name}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            filterOptions={(options) => options}
            loading={organizations.isFetching}
            loadingText="در حال دریافت سازمان‌ها..."
            noOptionsText={
              organizations.isError
                ? "خطا در دریافت سازمان‌ها"
                : "سازمانی یافت نشد"
            }
            onInputChange={(_, value) => setOrgSearchTerm(value)}
            renderOption={(props, option) => {
              const { key, ...optionProps } = props;
              return (
                <li {...optionProps} key={key} style={{ paddingRight: option.depth * 16 + 16 }}>
                  {option.name}
                </li>
              );
            }}
            renderInput={(params) => (
              <TextField {...params} label="سازمان" helperText="سازمان موردنظر را جستجو و انتخاب کنید." />
            )}
          />
          {/* انتخاب پست — جستجوی سروری با بارگذاری تدریجی در سازمان انتخاب‌شده */}
          <Autocomplete<PostSummary>
            options={postOptions}
            value={selectedPost}
            onChange={(_, value) => setSelectedPost(value)}
            getOptionLabel={(option) => `${option.code} — ${option.title}`}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            filterOptions={(options) => options}
            loading={postsQuery.isFetching}
            disabled={!selectedOrganization}
            loadingText="در حال دریافت پست‌ها..."
            noOptionsText={
              !selectedOrganization
                ? "ابتدا سازمان را انتخاب کنید"
                : postsQuery.isError
                  ? "خطا در دریافت پست‌ها"
                  : "پستی یافت نشد"
            }
            onInputChange={(_, value) => setPostSearchTerm(value)}
            slotProps={{
              listbox: {
                onScroll: (event: React.UIEvent<HTMLUListElement>) => {
                  const listbox = event.currentTarget;
                  const reachedEnd =
                    listbox.scrollTop + listbox.clientHeight >=
                    listbox.scrollHeight - 8;
                  if (
                    reachedEnd &&
                    postsQuery.hasNextPage &&
                    !postsQuery.isFetchingNextPage
                  ) {
                    void postsQuery.fetchNextPage();
                  }
                },
              },
            }}
            renderInput={(params) => (
              <TextField
                {...params}
                label="پست"
                helperText={
                  !selectedOrganization
                    ? "ابتدا سازمان را انتخاب کنید."
                    : `پست‌های یافت‌شده: ${postTotalCount}${
                        postsQuery.hasNextPage
                          ? " — برای نتایج بیشتر اسکرول کنید"
                          : ""
                      }`
                }
              />
            )}
          />
          {error && <Alert severity="error">{error}</Alert>}
        </DialogContent>
        <DialogActions>
          <Button onClick={closeDialog}>انصراف</Button>
          <Button
            variant="contained"
            disabled={assign.isPending || !selectedPost}
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
  assignment,
  code,
}: {
  assignment: ResponsibilityAssignment;
  code: string;
}) {
  const queryClient = useQueryClient();
  const endMutation = useEndResponsibilityAssignment(assignment.id);
  const [error, setError] = React.useState<string | null>(null);

  return (
    <TableRow>
      <TableCell>{assignment.postCode}</TableCell>
      <TableCell>{assignment.postTitle}</TableCell>
      <TableCell>
        <Button
          size="small"
          color="warning"
          disabled={endMutation.isPending}
          onClick={() => {
            setError(null);
            const today = new Date().toISOString().slice(0, 10);
            endMutation.mutate(today, {
              onSuccess: () => {
                queryClient.invalidateQueries({
                  queryKey: ["responsibilities", "assignments", code],
                });
              },
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
