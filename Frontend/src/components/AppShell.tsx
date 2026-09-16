"use client";

import * as React from "react";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import {
  AppBar,
  Avatar,
  Box,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
  Toolbar,
  Typography,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import LogoutIcon from "@mui/icons-material/Logout";
import AccountTreeIcon from "@mui/icons-material/AccountTree";
import { useColorMode } from "../theme/ThemeRegistry";
import { filterMenuByPermissions } from "../config/menu";
import { useLogout, useMe } from "../features/auth/useAuth";

const drawerWidth = 272;

/**
 * پوسته برنامه مطابق قالب مرجع (سایدبار + هدر + محتوا).
 */
export default function AppShell({ children }: { children: React.ReactNode }) {
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const pathname = usePathname();

  const drawer = <SidebarContent onNavigate={() => setMobileOpen(false)} />;

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      <Box
        component="nav"
        sx={{ width: { lg: drawerWidth }, flexShrink: { lg: 0 } }}
      >
        <Drawer
          variant="temporary" anchor="right"
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: "block", lg: "none" },
            "& .MuiDrawer-paper": { width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent" anchor="right"
          sx={{
            display: { xs: "none", lg: "block" },
            "& .MuiDrawer-paper": { width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      <Box sx={{ flexGrow: 1, minWidth: 0 }}>
        <Header onMenu={() => setMobileOpen(true)} pathname={pathname} />
        <Box
          component="main"
          sx={{ p: { xs: 2, sm: 3 }, maxWidth: 1440, mx: "auto" }}
        >
          {children}
        </Box>
      </Box>
    </Box>
  );
}

function SidebarContent({ onNavigate }: { onNavigate: () => void }) {
  const { data: user } = useMe();
  const pathname = usePathname();
  const visibleMenu = React.useMemo(
    () => filterMenuByPermissions(user?.permissions),
    [user?.permissions],
  );

  return (
    <Box
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
        p: 2.5,
      }}
    >
      <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, px: 1, mb: 4 }}>
        <Box
          sx={{
            width: 44,
            height: 44,
            borderRadius: 3,
            bgcolor: "primary.main",
            color: "#fff",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <AccountTreeIcon />
        </Box>
        <Box>
          <Typography variant="subtitle1" sx={{ fontWeight: 800 }}>
            ساختار سازمانی
          </Typography>
          <Typography variant="caption" color="text.secondary">
            پنل مدیریت ساختار
          </Typography>
        </Box>
      </Box>
      <Typography
        variant="caption"
        color="text.secondary"
        sx={{ px: 1.5, mb: 1, fontWeight: 700 }}
      >
        منوی اصلی
      </Typography>
      <List sx={{ display: "grid", gap: 0.5, p: 0 }}>
        {visibleMenu.map((item) => {
          const Icon = item.icon;
          const active =
            pathname === item.path || pathname.startsWith(`${item.path}/`);
          return (
            <ListItemButton
              key={item.id}
              component={Link}
              href={item.path}
              onClick={onNavigate}
              selected={active}
              sx={{ borderRadius: 2 }}
            >
              <ListItemIcon sx={{ minWidth: 36 }}>
                <Icon fontSize="small" />
              </ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          );
        })}
      </List>
      <Box sx={{ mt: "auto" }}>
        <Divider sx={{ my: 2 }} />
        <UserCard />
      </Box>
    </Box>
  );
}

function UserCard() {
  const router = useRouter();
  const { data: user } = useMe();
  const logout = useLogout();
  const [anchor, setAnchor] = React.useState<null | HTMLElement>(null);

  return (
    <>
      <Box
        onClick={(e) => setAnchor(e.currentTarget)}
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 1.5,
          p: 1,
          borderRadius: 2,
          cursor: "pointer",
          "&:hover": { bgcolor: "action.hover" },
        }}
      >
        <Avatar sx={{ bgcolor: "secondary.main", width: 36, height: 36 }}>
          ک
        </Avatar>
        <Box sx={{ minWidth: 0 }}>
          <Typography variant="body2" sx={{ fontWeight: 700 }} noWrap>
            کاربر جاری
          </Typography>
          <Typography variant="caption" color="text.secondary" noWrap dir="ltr">
            {user?.userId?.slice(0, 8) ?? "—"}
          </Typography>
        </Box>
      </Box>
      <Menu
        anchorEl={anchor}
        open={Boolean(anchor)}
        onClose={() => setAnchor(null)}
      >
        <MenuItem
          onClick={() => {
            setAnchor(null);
            logout.mutate(undefined, {
              onSettled: () => router.replace("/login"),
            });
          }}
        >
          <LogoutIcon fontSize="small" sx={{ ml: 1 }} />
          خروج
        </MenuItem>
      </Menu>
    </>
  );
}

function Header({
  onMenu,
  pathname,
}: {
  onMenu: () => void;
  pathname: string;
}) {
  const { mode, toggleMode } = useColorMode();
  const { data: user } = useMe();
  const title =
    filterMenuByPermissions(user?.permissions).find(
      (item) => pathname === item.path || pathname.startsWith(`${item.path}/`),
    )?.label ?? "داشبورد";

  return (
    <AppBar
      position="sticky"
      color="default"
      elevation={0}
      sx={{
        borderBottom: 1,
        borderColor: "divider",
        bgcolor: "background.default",
      }}
    >
      <Toolbar sx={{ minHeight: 82 }}>
        <IconButton
          onClick={onMenu}
          sx={{ display: { lg: "none" }, ml: 1 }}
          aria-label="باز کردن منو"
        >
          <MenuIcon />
        </IconButton>
        <Typography variant="h6" component="h1" sx={{ fontWeight: 800 }}>
          {title}
        </Typography>
        <Box sx={{ flexGrow: 1 }} />
        <IconButton onClick={toggleMode} aria-label="تغییر حالت نمایش">
          {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
        </IconButton>
      </Toolbar>
    </AppBar>
  );
}
