"use client";

import { SimpleTreeView } from "@mui/x-tree-view/SimpleTreeView";
import { TreeItem } from "@mui/x-tree-view/TreeItem";
import { Box, Chip, Typography } from "@mui/material";
import StarIcon from "@mui/icons-material/Star";
import type { PostTreeNode } from "./api";

/**
 * برچسب گره درخت با نشان صاحب‌امضا.
 */
function NodeLabel({ node }: { node: PostTreeNode }) {
  return (
    <Box sx={{ display: "flex", alignItems: "center", gap: 1, py: 0.5 }}>
      <Typography variant="body2" sx={{ fontWeight: 600 }}>
        {node.title}
      </Typography>
      <Typography variant="caption" color="text.secondary">
        {node.code}
      </Typography>
      {!node.isActive && (
        <Chip label="غیرفعال" size="small" color="default" />
      )}
      {node.hasSigningAuthority && (
        <Chip
          icon={<StarIcon />}
          label="صاحب امضا"
          size="small"
          color="primary"
        />
      )}
    </Box>
  );
}

function renderNode(node: PostTreeNode): React.ReactNode {
  return (
    <TreeItem key={node.id} itemId={node.id} label={<NodeLabel node={node} />}>
      {node.children.map((child) => renderNode(child))}
    </TreeItem>
  );
}

/**
 * درخت چارت سازمانی.
 */
export default function OrgChartTree({ roots }: { roots: PostTreeNode[] }) {
  return (
    <SimpleTreeView defaultExpandedItems={roots.map((r) => r.id)}>
      {roots.map((root) => renderNode(root))}
    </SimpleTreeView>
  );
}
