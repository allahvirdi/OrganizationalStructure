import { z } from "zod";
import { isoToJalali, isValidJalali } from "../../lib/date/jalali";

/**
 * بررسی اعتبار کد ملی ایران (الگوریتم چک‌سام رسمی).
 * ۱۰ رقم: ارقام ۱ تا ۹ در ضرایب ۱۰ تا ۲ ضرب، مد ۱۱، مقایسه با رقم دهم.
 */
function isValidIranianNationalCode(value: string): boolean {
  const digits = value.trim();
  if (digits.length !== 10 || !/^\d{10}$/.test(digits)) return false;
  if (new Set(digits).size === 1) return false; // تمام ارقام یکسان

  let sum = 0;
  for (let i = 0; i < 9; i++) {
    sum += Number(digits[i]) * (10 - i);
  }
  const remainder = sum % 11;
  const checkDigit = Number(digits[9]);
  const expected = remainder < 2 ? remainder : 11 - remainder;
  return checkDigit === expected;
}

/**
 * اعتبارسنجی فرم پرسنل (همسو با Backend: سازمان اجباری، کد ۸ رقمی، چک‌سام کد ملی، موبایل ایرانی اجباری).
 */
export const employeeSchema = z.object({
  organizationId: z
    .string()
    .min(1, "انتخاب سازمان الزامی است."),
  personnelCode: z
    .string()
    .regex(/^[0-9]{8}$/, "کد پرسنلی باید عدد ۸ رقمی باشد."),
  firstName: z
    .string()
    .min(1, "نام الزامی است.")
    .max(200, "نام حداکثر ۲۰۰ کاراکتر است."),
  lastName: z
    .string()
    .min(1, "نام خانوادگی الزامی است.")
    .max(200, "نام خانوادگی حداکثر ۲۰۰ کاراکتر است."),
  nationalCode: z
    .string()
    .min(1, "کد ملی الزامی است.")
    .refine(isValidIranianNationalCode, "کد ملی معتبر نیست."),
  mobile: z
    .string()
    .min(1, "شماره همراه الزامی است.")
    .regex(/^09[0-9]{9}$/, "شماره همراه باید فرمت ایرانی معتبر داشته باشد (مانند 09121234567)."),
});

export type EmployeeForm = z.infer<typeof employeeSchema>;

/**
 * اعتبارسنجی ویرایش اطلاعات پایه پرسنل (بدون سازمان و کد پرسنلی).
 */
export const updateEmployeeBasicSchema = employeeSchema.pick({
  firstName: true,
  lastName: true,
  nationalCode: true,
  mobile: true,
});

export type EmployeeBasicForm = z.infer<typeof updateEmployeeBasicSchema>;

/**
 * اعتبارسنجی اطلاعات تکمیلی پرسنل.
 *
 * @remarks
 * - birthDate: خروجی دیت‌پیکر جلالی همیشه ISO میلادی است؛ خالی مجاز است.
 * - سال و ماه سابقه فقط با هم وارد می‌شوند (هر دو یا هیچ‌کدام).
 * - شماره پژواک و وضعیت فعال بودن آن اجباری‌اند (ADR-013).
 */
export const supplementarySchema = z
  .object({
    birthDate: z.string().optional().or(z.literal("")),
    serviceYears: z
      .string()
      .regex(/^\d{1,3}$/, "سال سابقه باید عدد نامنفی باشد.")
      .optional()
      .or(z.literal("")),
    serviceMonths: z
      .string()
      .regex(/^(?:[0-9]|1[01])$/, "ماه سابقه باید بین ۰ تا ۱۱ باشد.")
      .optional()
      .or(z.literal("")),
    pezhvakMobile: z
      .string()
      .min(1, "شماره ثبت شده در پیام رسان پژواک الزامی است.")
      .regex(
        /^09[0-9]{9}$/,
        "شماره پژواک باید فرمت موبایل ایرانی معتبر داشته باشد (مانند 09191234567).",
      ),
    pezhvakIsActive: z.string(),
  })
  .refine(
    (v) => (v.serviceYears || "") === "" === ((v.serviceMonths || "") === ""),
    {
      message: "سال و ماه سابقه باید با هم وارد شوند.",
      path: ["serviceMonths"],
    },
  )
  .refine(
    (v) => {
      const iso = (v.birthDate ?? "").trim();
      if (iso === "") {
        return true;
      }
      const jalali = isoToJalali(iso);
      return jalali !== null && isValidJalali(jalali);
    },
    {
      message: "تاریخ تولد معتبر نیست.",
      path: ["birthDate"],
    },
  )
  .refine(
    (v) => v.pezhvakIsActive === "true" || v.pezhvakIsActive === "false",
    {
      message: "وضعیت فعال بودن شماره در شبکه پژواک الزامی است.",
      path: ["pezhvakIsActive"],
    },
  );

export type SupplementaryForm = z.infer<typeof supplementarySchema>;
