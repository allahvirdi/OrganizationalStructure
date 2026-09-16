"use client";

import * as React from "react";
import Link from "next/link";
import {
  Alert,
  Box,
  Button,
  Chip,
  Container,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import RequireAuth from "../../src/components/RequireAuth";
import { useMe } from "../../src/features/auth/useAuth";
import { usePosts, useSetPostStatus } from "../../src/features/posts/usePosts";
import { ApiError } from "../../src/lib/api/client";

const pageSize = 10;

/**
 * فهرست پست‌های سازمان کاربر جاری.
 */
export default function PostsPage() {
  return (
    <RequireAuth>
      <PostsContent />
    </RequireAuth>
  );
}

function PostsContent() {
  const { data: user } = useMe();
  const [page, setPage] = React.useState(0);
  const [searchTerm, setSearchTerm] = React.useState("");
  const [draft, setDraft] = React.useState("");

  const postsQuery = usePosts({
    organizationId: user?.organizationId ?? undefined,
    searchTerm: searchTerm || undefined,
    page: page + 1,
    pageSize,
  });

  return (
    <Container maxWidth="lg">
      <Box sx={{ py: 4 }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 2,
          }}
        >
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            پست‌های سازمانی
          </Typography>
          <Button component={Link} href="/posts/new" variant="contained">
            پست جدید
          </Button>
        </Box>
        <Box sx={{ display: "flex", gap: 1, mb: 2 }}>
          <TextField
            label="جستجو (کد/عنوان)"
            size="small"
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                setPage(0);
                setSearchTerm(draft);
              }
            }}
          />
          <Button
            variant="outlined"
            onClick={() => {
              setPage(0);
              setSearchTerm(draft);
            }}
          >
            جستجو
          </Button>
        </Box>
        {postsQuery.isError && (
          <Alert severity="error">خطا در دریافت فهرست پست‌ها.</Alert>
        )}
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>کد</TableCell>
                <TableCell>عنوان</TableCell>
                <TableCell>وضعیت</TableCell>
                <TableCell>عملیات</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {(postsQuery.data?.items ?? []).map((post) => (
                <PostRow key={post.id} post={post} />
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={postsQuery.data?.totalCount ?? 0}
            page={page}
            rowsPerPage={pageSize}
            rowsPerPageOptions={[pageSize]}
            onPageChange={(_, next) => setPage(next)}
            labelDisplayedRows={({ from, to, count }) =>
              `${from} تا ${to} از ${count}`
            }
          />
        </TableContainer>
      </Box>
    </Container>
  );
}

function PostRow({
  post,
}: {
  post: { id: string; code: string; title: string; isActive: boolean };
}) {
  const setStatus = useSetPostStatus(post.id);
  const [error, setError] = React.useState<string | null>(null);

  return (
    <TableRow>
      <TableCell>{post.code}</TableCell>
      <TableCell>{post.title}</TableCell>
      <TableCell>
        <Chip
          label={post.isActive ? "فعال" : "غیرفعال"}
          size="small"
          color={post.isActive ? "success" : "default"}
        />
      </TableCell>
      <TableCell>
        <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
          <Button component={Link} href={`/posts/${post.id}`} size="small">
            جزئیات
          </Button>
          <Button
            size="small"
            color={post.isActive ? "warning" : "success"}
            disabled={setStatus.isPending}
            onClick={() => {
              setError(null);
              setStatus.mutate(!post.isActive, {
                onError: (e) => {
                  setError(
                    e instanceof ApiError ? e.message : "خطا در تغییر وضعیت.",
                  );
                },
              });
            }}
          >
            {post.isActive ? "غیرفعال" : "فعال"}
          </Button>
        </Box>
        {error && (
          <Typography variant="caption" color="error">
            {error}
          </Typography>
        )}
      </TableCell>
    </TableRow>
  );
}
