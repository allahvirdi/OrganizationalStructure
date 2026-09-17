"use client";

import * as React from "react";
import { useParams } from "next/navigation";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import {
  Alert,
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
  useAssignAuthority,
  useAuthority,
  useEndAuthorityAssignment,
} from "../../../../src/features/authorities/useAuthorities";
import { fetchAuthorityAssignments } from "../../../../src/features/authorities/api";
import type { AuthorityAssignment } from "../../../../src/features/authorities/api";
import { ApiError } from "../../../../src/lib/api/client";

/**
 * صفحه جزئیات اختیار + انتساب به پست.
 */
export default function AuthorityDetailPage() {
  return (
    <AuthorityDetailContent />
  );
}

function AuthorityDetailContent() {
  const params = useParams<{ code: string }>();
  const code = decodeURIComponent(params.code);
  const { data: item, isLoading, isError } = useAuthority(code);

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
          <Alert severity="error">اختیار یافت نشد یا دسترسی ندارید.</Alert>
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
        <Typography variant="body2" color="text.secondary" dir="ltr">
          {item.code}
        </Typography>
        <AssignmentSection code={item.code} />
      </Box>
    </Container>
  );
}

function AssignmentSection({ code }: { code: string }) {
  const queryClient = useQueryClient();
  const assignmentsQuery = useQuery({
    queryKey: ["authorities", "assignments", code],
    queryFn: () => fetchAuthorityAssignments(code),
  });
  const assign = useAssignAuthority();
  const [dialogOpen, setDialogOpen] = React.useState(false);
  const [postId, setPostId] = React.useState("");
  const [error, setError] = React.useState<string | null>(null);

  const onAssign = () => {
    setError(null);
    assign.mutate(
      { authorityCode: code, postId: postId.trim() },
      {
        onSuccess: () => {
          setDialogOpen(false);
          setPostId("");
          queryClient.invalidateQueries({
            queryKey: ["authorities", "assignments", code],
          });
        },
        onError: (e) => {
          setError(e instanceof ApiError ? e.message : "خطا در انتساب.");
        },
      },
    );
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
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)}>
        <DialogTitle>انتساب اختیار به پست</DialogTitle>
        <DialogContent sx={{ minWidth: 320 }}>
          <TextField
            label="شناسه پست"
            fullWidth
            sx={{ mt: 1 }}
            value={postId}
            onChange={(e) => setPostId(e.target.value)}
          />
          {error && <Alert severity="error">{error}</Alert>}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>انصراف</Button>
          <Button
            variant="contained"
            disabled={assign.isPending || postId.trim() === ""}
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
  assignment: AuthorityAssignment;
  code: string;
}) {
  const queryClient = useQueryClient();
  const endMutation = useEndAuthorityAssignment(assignment.id);
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
                  queryKey: ["authorities", "assignments", code],
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
