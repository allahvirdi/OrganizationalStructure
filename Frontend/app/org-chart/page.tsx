"use client";

import { useQueries, useQuery } from "@tanstack/react-query";
import {
  Alert,
  Box,
  CircularProgress,
  Container,
  Typography,
} from "@mui/material";
import RequireAuth from "../../src/components/RequireAuth";
import { useMe } from "../../src/features/auth/useAuth";
import { fetchOrgPosts, fetchSubtree } from "../../src/features/org-chart/api";
import OrgChartTree from "../../src/features/org-chart/OrgChartTree";

/**
 * صفحه چارت سازمانی (سازمان کاربر جاری).
 */
export default function OrgChartPage() {
  return (
    <RequireAuth>
      <OrgChartContent />
    </RequireAuth>
  );
}

function OrgChartContent() {
  const { data: user } = useMe();
  const organizationId = user?.organizationId ?? null;

  const postsQuery = useQuery({
    queryKey: ["org-posts", organizationId],
    queryFn: () => fetchOrgPosts(organizationId!),
    enabled: organizationId !== null,
  });

  const roots =
    postsQuery.data?.items.filter((p) => p.parentId === null) ?? [];

  const subtrees = useQueries({
    queries: roots.map((root) => ({
      queryKey: ["subtree", root.id],
      queryFn: () => fetchSubtree(root.id),
      enabled: organizationId !== null,
    })),
  });

  const isLoading =
    postsQuery.isLoading || subtrees.some((q) => q.isLoading);
  const isError =
    postsQuery.isError || subtrees.some((q) => q.isError);

  return (
    <Container maxWidth="lg">
      <Box sx={{ py: 4 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>
          چارت سازمانی
        </Typography>
        {organizationId === null && (
          <Alert severity="warning">سازمان کاربر مشخص نیست.</Alert>
        )}
        {isLoading && (
          <Box sx={{ display: "flex", justifyContent: "center", py: 6 }}>
            <CircularProgress />
          </Box>
        )}
        {isError && (
          <Alert severity="error">خطا در دریافت ساختار سازمانی.</Alert>
        )}
        {!isLoading && !isError && roots.length === 0 && (
          <Alert severity="info">پستی در این سازمان ثبت نشده است.</Alert>
        )}
        {!isLoading && !isError && (
          <OrgChartTree
            roots={subtrees
              .map((q) => q.data)
              .filter((n) => n !== undefined)}
          />
        )}
      </Box>
    </Container>
  );
}
