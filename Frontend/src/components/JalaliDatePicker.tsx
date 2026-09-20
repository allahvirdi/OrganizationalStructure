"use client";

import * as React from "react";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  MenuItem,
  Select,
  TextField,
  Typography,
} from "@mui/material";
import type { SelectChangeEvent } from "@mui/material";
import CalendarMonthIcon from "@mui/icons-material/CalendarMonth";
import {
  JALALI_MONTH_NAMES,
  formatJalali,
  jalaliMonthLength,
  jalaliToIso,
  isoToJalali,
  toPersianDigits,
  type JalaliDate,
} from "../lib/date/jalali";

/**
 * دیت‌پیکر فارسی (جلالی) با انتخاب جداگانهٔ سال، ماه و روز.
 *
 * @remarks
 * خروجی همیشه ISO میلادی است تا قرارداد Backend (DateOnly) حفظ شود و ورودی متنی آزاد
 * (که باعث ذخیرهٔ تاریخ اشتباه می‌شد) حذف گردد. فقط از پریمیتیوهای MUI استفاده می‌کند
 * و هیچ کتابخانهٔ تاریخ جدیدی به استک منجمد (ADR-001) اضافه نمی‌کند.
 */
export interface JalaliDatePickerProps {
  /** برچسب فیلد */
  label: string;
  /** مقدار فعلی به قالب ISO میلادی (yyyy-MM-dd) یا null */
  value: string | null | undefined;
  /** ثبت مقدار جدید ISO (null یعنی پاک‌شدن) */
  onChange: (iso: string | null) => void;
  /** پیام کمکی زیر فیلد */
  helperText?: string;
  /** وضعیت خطا */
  error?: boolean;
  /** غیرفعال بودن فیلد */
  disabled?: boolean;
  /** ستارهٔ اجباری بودن در برچسب */
  required?: boolean;
  /** کمترین سال جلالی قابل انتخاب */
  minYear?: number;
  /** بیشترین سال جلالی قابل انتخاب (پیش‌فرض: سال جاری) */
  maxYear?: number;
  /** شناسهٔ element برای اتصال به فرم */
  id?: string;
}

/** نمونهٔ قالب در placeholder (بدون لیترال رقم فارسی در سورس). */
const PERSIAN_PLACEHOLDER = toPersianDigits("1400/05/12");

/** سال جاری جلالی (مبتنی بر تاریخ مرورگر). */
function currentJalaliYear(): number {
  const now = new Date();
  const pad = (n: number) => String(n).padStart(2, "0");
  const iso = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`;
  return isoToJalali(iso)?.jy ?? 1405;
}
/**
 * انتخابگر تاریخ شمسی با مکالمهٔ جداگانهٔ سال، ماه و روز.
 */
export default function JalaliDatePicker({
  label,
  value,
  onChange,
  helperText,
  error = false,
  disabled = false,
  required = false,
  minYear = 1270,
  maxYear,
  id,
}: JalaliDatePickerProps) {
  const upperYear = maxYear ?? currentJalaliYear();
  const selectedIso = value ?? null;
  const selected = React.useMemo(
    () => isoToJalali(selectedIso),
    [selectedIso],
  );

  const [open, setOpen] = React.useState(false);
  const [draft, setDraft] = React.useState<JalaliDate>(
    selected ?? { jy: upperYear, jm: 1, jd: 1 },
  );

  /** باز کردن مکالمه با پیش‌نویس تازه از مقدار فعلی. */
  const openDialog = () => {
    setDraft(selected ?? { jy: upperYear, jm: 1, jd: 1 });
    setOpen(true);
  };

  const years = React.useMemo(() => {
    const list: number[] = [];
    for (let y = upperYear; y >= minYear; y -= 1) {
      list.push(y);
    }
    return list;
  }, [minYear, upperYear]);

  const days = React.useMemo(
    () =>
      Array.from({ length: jalaliMonthLength(draft.jy, draft.jm) }, (_, i) => i + 1),
    [draft.jy, draft.jm],
  );

  const onYearChange = (event: SelectChangeEvent<number>) => {
    const jy = Number(event.target.value);
    setDraft((prev) => ({
      jy,
      jm: prev.jm,
      jd: Math.min(prev.jd, jalaliMonthLength(jy, prev.jm)),
    }));
  };

  const onMonthChange = (event: SelectChangeEvent<number>) => {
    const jm = Number(event.target.value);
    setDraft((prev) => ({
      jy: prev.jy,
      jm,
      jd: Math.min(prev.jd, jalaliMonthLength(prev.jy, jm)),
    }));
  };

  const onDayChange = (event: SelectChangeEvent<number>) => {
    setDraft((prev) => ({ ...prev, jd: Number(event.target.value) }));
  };

  const onAccept = () => {
    onChange(jalaliToIso(draft));
    setOpen(false);
  };

  const onClear = () => {
    onChange(null);
    setOpen(false);
  };

  return (
    <Box sx={{ display: "grid", gap: 0.5 }}>
      <Box sx={{ display: "flex", gap: 1, alignItems: "flex-start" }}>
        <TextField
          id={id}
          label={`${label}${required ? " *" : ""}`}
          value={selected ? formatJalali(selected) : ""}
          placeholder={PERSIAN_PLACEHOLDER}
          fullWidth
          slotProps={{ htmlInput: { readOnly: true } }}
          disabled={disabled}
          error={error}
          helperText={helperText}
          onClick={openDialog}
        />
        <IconButton
          aria-label={`انتخاب ${label}`}
          disabled={disabled}
          onClick={openDialog}
          sx={{ mt: 0.75 }}
        >
          <CalendarMonthIcon />
        </IconButton>
      </Box>
      {!selected && !error && (
        <Typography variant="caption" color="text.secondary">
          تاریخ انتخاب نشده است.
        </Typography>
      )}

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>{label}</DialogTitle>
        <DialogContent sx={{ display: "grid", gap: 2, pt: 1 }}>
          <Select
            aria-label="سال"
            value={draft.jy}
            onChange={onYearChange}
            fullWidth
          >
            {years.map((y) => (
              <MenuItem key={y} value={y}>
                {`${toPersianDigits(y)} — سال`}
              </MenuItem>
            ))}
          </Select>
          <Select
            aria-label="ماه"
            value={draft.jm}
            onChange={onMonthChange}
            fullWidth
          >
            {JALALI_MONTH_NAMES.map((name, index) => (
              <MenuItem key={name} value={index + 1}>
                {`${toPersianDigits(String(index + 1).padStart(2, "0"))} — ${name}`}
              </MenuItem>
            ))}
          </Select>
          <Select
            aria-label="روز"
            value={draft.jd}
            onChange={onDayChange}
            fullWidth
          >
            {days.map((d) => (
              <MenuItem key={d} value={d}>
                {toPersianDigits(String(d).padStart(2, "0"))}
              </MenuItem>
            ))}
          </Select>
          <Typography variant="body2" color="text.secondary">
            {`پیش‌نمایش: ${formatJalali(draft)}`}
          </Typography>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={onClear} color="inherit" disabled={!selected}>
            پاک‌کردن
          </Button>
          <Box sx={{ flexGrow: 1 }} />
          <Button onClick={() => setOpen(false)}>انصراف</Button>
          <Button variant="contained" onClick={onAccept}>
            تأیید
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}