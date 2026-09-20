/**
 * تبدیل دقیق تاریخ جلالی (شمسی) به میلادی و برعکس — بدون وابستگی به کتابخانه خارجی.
 *
 * @remarks
 * الگوریتم بر پایهٔ جدول چرخه‌های ۳۳ سالهٔ جلالی (همان الگوریتم استاندارد jalaali-js) است.
 * دلیل پیاده‌سازی داخلی: استک Frontend طبق ADR-001 منجمد است و هیچ کتابخانهٔ تاریخ
 * (moment/dayjs/@mui/x-date-pickers) در پروژه نصب نیست؛ افزودن وابستگی جدید نیازمند ADR است.
 * همهٔ توابع خالص‌اند و خروجی به Backend همیشه ISO میلادی (yyyy-MM-dd) می‌ماند.
 */

/** تاریخ جلالی (سال/ماه/روز). */
export interface JalaliDate {
  /** سال جلالی */
  jy: number;
  /** ماه جلالی (۱..۱۲) */
  jm: number;
  /** روز جلالی (۱..۳۱) */
  jd: number;
}

/** تاریخ میلادی (سال/ماه/روز). */
export interface GregorianDate {
  /** سال میلادی */
  gy: number;
  /** ماه میلادی (۱..۱۲) */
  gm: number;
  /** روز میلادی (۱..۳۱) */
  gd: number;
}

/** نام ماه‌های جلالی به فارسی. */
export const JALALI_MONTH_NAMES = [
  "فروردین",
  "اردیبهشت",
  "خرداد",
  "تیر",
  "مرداد",
  "شهریور",
  "مهر",
  "آبان",
  "آذر",
  "دی",
  "بهمن",
  "اسفند",
] as const;

/** نقطه‌های شکست چرخهٔ کبیسهٔ جلالی. */
const JALALI_BREAKS = [
  -61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210, 1635, 2060, 2097, 2192,
  2262, 2324, 2394, 2456, 3178,
];

/** تقسیم صحیح (گرد کردن به سمت صفر). */
function div(a: number, b: number): number {
  return Math.trunc(a / b);
}

/** باقیماندهٔ سازگار با تقسیم صحیح. */
function mod(a: number, b: number): number {
  return a - Math.trunc(a / b) * b;
}

/** محاسبهٔ پارامترهای چرخهٔ کبیسه و مبدأ مارچ برای یک سال جلالی. */
function jalCal(jy: number): { leap: number; gy: number; march: number } {
  const bl = JALALI_BREAKS.length;
  const gy = jy + 621;
  let leapJ = -14;
  let jp = JALALI_BREAKS[0];
  let jump = 0;

  for (let i = 1; i < bl; i += 1) {
    const jm = JALALI_BREAKS[i];
    jump = jm - jp;
    if (jy < jm) {
      break;
    }
    leapJ = leapJ + div(jump, 33) * 8 + div(mod(jump, 33), 4);
    jp = jm;
  }

  let n = jy - jp;
  leapJ = leapJ + div(n, 33) * 8 + div(mod(n, 33) + 3, 4);
  if (mod(jump, 33) === 4 && jump - n === 4) {
    leapJ += 1;
  }

  const leapG = div(gy, 4) - div((div(gy, 100) + 1) * 3, 4) - 150;
  const march = 20 + leapJ - leapG;

  if (jump - n < 6) {
    n = n - jump + div(jump + 4, 33) * 33;
  }
  let leap = mod(mod(n + 1, 33) - 1, 4);
  if (leap === -1) {
    leap = 4;
  }

  return { leap, gy, march };
}
/** آیا سال جلالی کبیسه است؟ */
export function isLeapJalaliYear(jy: number): boolean {
  return jalCal(jy).leap === 0;
}

/** طول ماه جلالی بر حسب روز. */
export function jalaliMonthLength(jy: number, jm: number): number {
  if (jm <= 6) {
    return 31;
  }
  if (jm <= 11) {
    return 30;
  }
  return isLeapJalaliYear(jy) ? 30 : 29;
}

/** تبدیل تاریخ میلادی به شماره روز ژولیان. */
function g2d(gy: number, gm: number, gd: number): number {
  const d =
    div((gy + div(gm - 8, 6) + 100100) * 1461, 4) +
    div(153 * mod(gm + 9, 12) + 2, 5) +
    gd -
    34840408;
  return d - div(div(gy + 100100 + div(gm - 8, 6), 100) * 3, 4) + 752;
}

/** تبدیل شماره روز ژولیان به تاریخ میلادی. */
function d2g(jdn: number): GregorianDate {
  let j = 4 * jdn + 139361631;
  j = j + div(div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908;
  const i = div(mod(j, 1461), 4) * 5 + 308;
  const gd = div(mod(i, 153), 5) + 1;
  const gm = mod(div(i, 153), 12) + 1;
  const gy = div(j, 1461) - 100100 + div(8 - gm, 6);
  return { gy, gm, gd };
}

/** شماره روز ژولیانِ اول فروردین یک سال جلالی (نوروز). */
function nowruzJdn(jy: number): number {
  const r = jalCal(jy);
  return g2d(r.gy, 3, r.march);
}

/** تبدیل تاریخ جلالی به میلادی. */
export function jalaliToGregorian(date: JalaliDate): GregorianDate {
  const dayOfYear =
    (date.jm - 1) * 31 - div(date.jm, 7) * (date.jm - 7) + date.jd;
  return d2g(nowruzJdn(date.jy) + dayOfYear - 1);
}

/**
 * تبدیل تاریخ میلادی به جلالی.
 *
 * @remarks
 * ابتدا سال جلالی تقریبی (سال میلادی منهای ۶۲۱) بررسی و در صورت نیاز یک سال
 * جلو/عقب می‌شود؛ سپس روزِ درون‌سال نسبت به نوروز همان سال به ماه/روز تبدیل می‌گردد.
 */
export function gregorianToJalali(date: GregorianDate): JalaliDate {
  const jdn = g2d(date.gy, date.gm, date.gd);
  const gy = d2g(jdn).gy;
  let jy = gy - 621;
  const r = jalCal(jy);
  const jdn1f = g2d(gy, 3, r.march);
  const k = jdn - jdn1f;

  // k منفی یعنی تاریخ قبل از نوروز همان سال میلادی است؛ پس به سال جلالی قبلی می‌رویم.
  if (k >= 0) {
    if (k <= 185) {
      return { jy, jm: 1 + div(k, 31), jd: mod(k, 31) + 1 };
    }
  } else {
    jy -= 1;
  }

  const adjusted = k < 0 ? k + 179 + (r.leap === 1 ? 1 : 0) : k - 186;
  return { jy, jm: 7 + div(adjusted, 30), jd: mod(adjusted, 30) + 1 };
}

/** بررسی اعتبار یک تاریخ جلالی. */
export function isValidJalali(date: JalaliDate): boolean {
  if (
    !Number.isInteger(date.jy) ||
    !Number.isInteger(date.jm) ||
    !Number.isInteger(date.jd)
  ) {
    return false;
  }
  if (date.jm < 1 || date.jm > 12) {
    return false;
  }
  return date.jd >= 1 && date.jd <= jalaliMonthLength(date.jy, date.jm);
}

/** تبدیل رشتهٔ ISO میلادی (yyyy-MM-dd) به تاریخ میلادی؛ نامعتبر = null. */
export function parseIsoDate(value: string | null | undefined): GregorianDate | null {
  if (!value) {
    return null;
  }
  const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value.trim());
  if (!match) {
    return null;
  }
  return { gy: Number(match[1]), gm: Number(match[2]), gd: Number(match[3]) };
}

/** ساخت رشتهٔ ISO میلادی از تاریخ میلادی. */
export function toIsoDate(date: GregorianDate): string {
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${date.gy}-${pad(date.gm)}-${pad(date.gd)}`;
}

/** تبدیل رشتهٔ ISO میلادی به تاریخ جلالی؛ خالی/نامعتبر = null. */
export function isoToJalali(value: string | null | undefined): JalaliDate | null {
  const gregorian = parseIsoDate(value);
  return gregorian ? gregorianToJalali(gregorian) : null;
}

/** تبدیل تاریخ جلالی به رشتهٔ ISO میلادی. */
export function jalaliToIso(date: JalaliDate): string {
  return toIsoDate(jalaliToGregorian(date));
}

/** کد یونیکد رقم صفر فارسی (U+06F0) — ارقام به‌صورت محاسباتی ساخته می‌شوند تا لیترال فارسی در کد نباشد. */
const PERSIAN_DIGIT_ZERO = 0x06f0;

/** تبدیل ارقام لاتین به ارقام فارسی (فقط برای نمایش). */
export function toPersianDigits(value: string | number): string {
  return String(value).replace(/[0-9]/g, (d) =>
    String.fromCharCode(PERSIAN_DIGIT_ZERO + Number(d)),
  );
}

/** نمایش جلالی به قالب «۱۳۶۰/۰۵/۱۲»؛ بدون روز: «۱۳۶۰/۰۵». */
export function formatJalali(date: JalaliDate | null, withDay = true): string {
  if (!date) {
    return "";
  }
  const pad = (n: number) => String(n).padStart(2, "0");
  const head = `${toPersianDigits(date.jy)}/${toPersianDigits(pad(date.jm))}`;
  return withDay ? `${head}/${toPersianDigits(pad(date.jd))}` : head;
}

/** نمایش جلالی همراه با نام ماه: «۱۲ مرداد ۱۳۰». */
export function formatJalaliLong(date: JalaliDate | null): string {
  if (!date) {
    return "";
  }
  return `${toPersianDigits(date.jd)} ${JALALI_MONTH_NAMES[date.jm - 1]} ${toPersianDigits(date.jy)}`;
}