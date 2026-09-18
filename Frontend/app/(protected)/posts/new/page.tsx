"use client";

import * as React from "react";
import { useQuery } from "@tanstack/react-query";
import RequirePermission from "../../../../src/components/RequirePermission";
import { fetchPosts } from "../../../../src/features/posts/api";
import type { PostSummary } from "../../../../src/features/org-chart/api";
import type { OrganizationOption } from "../../../../src/features/organizations/api";
import { useOrganizations } from "../../../../src/features/organizations/useOrganizations";

import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Alert,
  Autocomplete,
  Box,
  Button,
  Container,
  Divider,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useCreatePost } from "../../../../src/features/posts/usePosts";
import { postSchema } from "../../../../src/features/posts/schemas";
import { ApiError } from "../../../../src/lib/api/client";
import { useAuthorities } from "../../../../src/features/authorities/useAuthorities";
import type { Authority } from "../../../../src/features/authorities/api";
import { assignAuthority } from "../../../../src/features/authorities/api";
import { useResponsibilities } from "../../../../src/features/responsibilities/useResponsibilities";
import type { Responsibility } from "../../../../src/features/responsibilities/api";
import { assignResponsibility } from "../../../../src/features/responsibilities/api";

const createSchema = postSchema.extend({
  parentId: z.string().optional().or(z.literal("")),
});

type CreateForm = z.infer<typeof createSchema>;

/** گزینه انتخاب مسئولیت در فرم ایجاد پست. */
type ResponsibilityOption = Pick<Responsibility, "code" | "title">;

/** گزینه انتخاب اختیار در فرم ایجاد پست. */
type AuthorityOption = Pick<Authority, "code" | "title">;

const authorityOptionsLimit = 50;
const responsibilityOptionsLimit = 50;

/**
 * صفحه ایجاد پست جدید.
 */
export default function NewPostPage() {
  return (
    <RequirePermission permission="OrganizationStructure.Post.Create">
      <NewPostContent />
    </RequirePermission>
  );
}

function NewPostContent() {
  const createPost = useCreatePost();
  const [organization, setOrganization] = React.useState<OrganizationOption | null>(
    null,
  );
  const [organizationSearchTerm, setOrganizationSearchTerm] = React.useState("");
  const [parent, setParent] = React.useState<PostSummary | null>(null);
  const [searchTerm, setSearchTerm] = React.useState("");
  const [parentPage, setParentPage] = React.useState(1);
  const [selectedResponsibilities, setSelectedResponsibilities] = React.useState<
    ResponsibilityOption[]
  >([]);
  const [selectedAuthorities, setSelectedAuthorities] = React.useState<
    AuthorityOption[]
  >([]);
  const [successMessage, setSuccessMessage] = React.useState<string | null>(null);

  const organizations = useOrganizations({ searchTerm: organizationSearchTerm });
  // همان پرس‌وجو بدون فیلتر (کلید کش یکسان با حالت «عبارت خالی») برای تشخیص «نبود سازمان در محدوده».
  const allOrganizations = useOrganizations();

  // انتخاب پیش‌فرض «سازمان خود کاربر» فقط وقتی عبارت جستجو خالی است و کاربر انتخابی نکرده باشد.
  const defaultOrganization =
    organizationSearchTerm === ""
      ? (organizations.data?.find((option) => option.isCurrent) ?? null)
      : null;
  const selectedOrganization = organization ?? defaultOrganization;
  const organizationId = selectedOrganization?.id;

  const parents = useQuery({
    queryKey: ["posts", "parent-options", organizationId, searchTerm, parentPage],
    enabled: Boolean(organizationId),
    queryFn: () =>
      fetchPosts({
        organizationId: organizationId!,
        searchTerm,
        page: parentPage,
        pageSize: 20,
      }),
  });

  // فهرست مسئولیت‌های فعال برای انتخاب در فرم ایجاد.
  const responsibilities = useResponsibilities({
    isActive: true,
    page: 1,
    pageSize: responsibilityOptionsLimit,
  });

  // فهرست اختیارهای فعال برای انتخاب در فرم ایجاد.
  const authorities = useAuthorities({
    isActive: true,
    page: 1,
    pageSize: authorityOptionsLimit,
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateForm>({ resolver: zodResolver(createSchema) });

  // با تغییر سازمان، والد انتخاب‌شده (متعلق به سازمان قبلی) پاک می‌شود.
  const onOrganizationChange = (value: OrganizationOption | null) => {
    setOrganization(value);
    setParent(null);
    setParentPage(1);
  };

  const resetForm = () => {
    reset({ code: "", title: "", description: "" });
    setParent(null);
    setSearchTerm("");
    setParentPage(1);
    setSelectedResponsibilities([]);
    setSelectedAuthorities([]);
  };

  const onSubmit = async (values: CreateForm) => {
    if (!selectedOrganization) {
      return;
    }

    setSuccessMessage(null);

    try {
      const postId = await createPost.mutateAsync({
        organizationId: selectedOrganization.id,
        code: values.code,
        title: values.title,
        description: values.description || null,
        parentId: parent?.id ?? null,
      });

      // انتساب مسئولیت‌ها و اختیارها به پست جدید.
      const failedAssignments: string[] = [];

      const authorityResults = await Promise.allSettled(
        selectedAuthorities.map((a) =>
          assignAuthority({ authorityCode: a.code, postId }),
        ),
      );
      authorityResults.forEach((result, index) => {
        if (result.status === "rejected") {
          failedAssignments.push(selectedAuthorities[index].title);
        }
      });

      const responsibilityResults = await Promise.allSettled(
        selectedResponsibilities.map((r) =>
          assignResponsibility({ responsibilityCode: r.code, postId }),
        ),
      );
      responsibilityResults.forEach((result, index) => {
        if (result.status === "rejected") {
          failedAssignments.push(selectedResponsibilities[index].title);
        }
      });

      resetForm();

      if (failedAssignments.length > 0) {
        setSuccessMessage(
          `پست با موفقیت ثبت شد؛ اما ثبت ${failedAssignments.length} انتساب (${failedAssignments.join("، ")}) ناموفق بود. می‌توانید آن‌ها را از صفحه جزئیات پست ثبت کنید.`,
        );
      } else {
        setSuccessMessage("پست با موفقیت ثبت شد.");
      }
    } catch {
      // خطا توسط serverError نمایش داده می‌شود.
    }
  };

  const serverError =
    createPost.error instanceof ApiError ? createPost.error.message : null;
  // هشدار «سازمانی وجود ندارد» باید به فهرست بدون فیلتر نگاه کند، نه نتیجه جستجوی جاری.
  const noOrganization =
    !allOrganizations.isLoading && (allOrganizations.data?.length ?? 0) === 0;
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
            <Autocomplete
              options={organizations.data ?? []}
              value={selectedOrganization}
              onChange={(_, value) => onOrganizationChange(value)}
              onInputChange={(_, value, reason) => {
                if (reason === "input" || reason === "clear") {
                  setOrganizationSearchTerm(value);
                }
              }}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              filterOptions={(options) => options}
              loading={organizations.isFetching}
              disabled={createPost.isPending}
              loadingText="در حال دریافت سازمان‌ها..."
              noOptionsText={
                organizations.isError
                  ? "خطا در دریافت سازمان‌ها"
                  : "سازمانی یافت نشد"
              }
              renderOption={(props, option) => {
                const { key, ...optionProps } = props;
                return (
                  <Box
                    component="li"
                    key={key}
                    {...optionProps}
                    sx={{
                      display: "flex",
                      gap: 1,
                      alignItems: "center",
                      paddingInlineStart: `${16 + option.depth * 16}px !important`,
                    }}
                  >
                    <Typography variant="body2">{option.name}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {option.code}
                    </Typography>
                  </Box>
                );
              }}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="سازمان پست (جستجوی نام سازمان)"
                  helperText="پیش‌فرض سازمان خود شماست؛ فقط سازمان شما و زیرمجموعه‌های آن قابل انتخاب است."
                />
              )}
            />
            {organizations.isError && (
              <Alert
                severity="error"
                action={
                  <Button onClick={() => organizations.refetch()}>
                    تلاش مجدد
                  </Button>
                }
              >
                دریافت فهرست سازمان‌ها ناموفق بود.
              </Alert>
            )}
            {noOrganization && !organizations.isError && (
              <Alert severity="warning">
                سازمانی برای انتخاب وجود ندارد؛ ثبت پست امکان‌پذیر نیست.
              </Alert>
            )}
            <Autocomplete
              options={parents.data?.items ?? []}
              value={parent}
              onChange={(_, value) => setParent(value)}
              onInputChange={(_, value, reason) => {
                if (reason === "input" || reason === "clear") {
                  setSearchTerm(value);
                  setParentPage(1);
                }
              }}
              getOptionLabel={(option) => `${option.code} — ${option.title}`}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              filterOptions={(options) => options}
              loading={parents.isFetching}
              disabled={!organizationId || createPost.isPending}
              loadingText="در حال جستجو..."
              noOptionsText={
                parents.isError ? "خطا در دریافت پستها" : "پستی یافت نشد"
              }
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="پست والد (جستجوی کد یا عنوان)"
                  helperText="بدون انتخاب والد، پست در ریشهٔ درخت سازمان انتخاب‌شده ثبت می‌شود."
                />
              )}
            />
            {parents.isError && (
              <Alert
                severity="error"
                action={
                  <Button onClick={() => parents.refetch()}>تلاش مجدد</Button>
                }
              >
                دریافت پست‌های سازمان ناموفق بود.
              </Alert>
            )}
            <Box
              sx={{
                display: "flex",
                gap: 1,
                alignItems: "center",
                flexWrap: "wrap",
              }}
            >
              <Button
                disabled={parentPage === 1 || parents.isFetching}
                onClick={() => setParentPage((page) => page - 1)}
              >
                نتایج قبلی
              </Button>
              <Typography variant="caption">صفحه {parentPage}</Typography>
              <Button
                disabled={
                  parents.isFetching ||
                  !parents.data ||
                  parentPage >= parents.data.totalPages
                }
                onClick={() => setParentPage((page) => page + 1)}
              >
                نتایج بعدی
              </Button>
            </Box>
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
            <Divider />
            <Typography variant="subtitle2" sx={{ fontWeight: 700 }}>
              حق امضا و مسئولیت‌ها (اختیاری)
            </Typography>
            <Autocomplete
              multiple
              options={(responsibilities.data?.items ?? []).map((item) => ({
                code: item.code,
                title: item.title,
              }))}
              value={selectedResponsibilities}
              onChange={(_, value) => setSelectedResponsibilities(value)}
              getOptionLabel={(option) => `${option.code} — ${option.title}`}
              isOptionEqualToValue={(option, value) => option.code === value.code}
              loading={responsibilities.isFetching}
              disabled={createPost.isPending}
              loadingText="در حال دریافت مسئولیت‌ها..."
              noOptionsText={
                responsibilities.isError
                  ? "خطا در دریافت مسئولیت‌ها"
                  : "مسئولیتی یافت نشد"
              }
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="مسئولیت‌های پست"
                  helperText="مسئولیت‌های سازمانی موردنیاز این پست را انتخاب کنید."
                />
              )}
            />
            <Autocomplete
              multiple
              options={(authorities.data?.items ?? []).map((item) => ({
                code: item.code,
                title: item.title,
              }))}
              value={selectedAuthorities}
              onChange={(_, value) => setSelectedAuthorities(value)}
              getOptionLabel={(option) => `${option.code} — ${option.title}`}
              isOptionEqualToValue={(option, value) => option.code === value.code}
              loading={authorities.isFetching}
              disabled={createPost.isPending}
              loadingText="در حال دریافت اختیارها..."
              noOptionsText={
                authorities.isError
                  ? "خطا در دریافت اختیارها"
                  : "اختیاری یافت نشد"
              }
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="حق امضا / اختیارهای پست"
                  helperText="اختیارهای امضا یا سازمانی موردنیاز این پست را انتخاب کنید."
                />
              )}
            />
            {successMessage && (
              <Alert severity="success">{successMessage}</Alert>
            )}
            {serverError && <Alert severity="error">{serverError}</Alert>}
            <Button
              type="submit"
              variant="contained"
              disabled={createPost.isPending || !selectedOrganization}
            >
              ثبت پست
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}