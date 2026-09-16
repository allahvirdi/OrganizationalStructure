import { z } from "zod";

/**
 * اعتبارسنجی فرم پرسنل (همسو با Backend: کد ۸ رقمی).
 */
export const employeeSchema = z.object({
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
    .max(20, "کد ملی حداکثر ۲۰ کاراکتر است."),
  mobile: z.string().max(20).optional().or(z.literal("")),
});

export type EmployeeForm = z.infer<typeof employeeSchema>;

/**
 * اعتبارسنجی اطلاعات تکمیلی (سال و ماه با هم).
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
    pezhvakMobile: z.string().max(20).optional().or(z.literal("")),
  })
  .refine(
    (v) => (v.serviceYears || "") === "" === ((v.serviceMonths || "") === ""),
    {
      message: "سال و ماه سابقه باید با هم وارد شوند.",
      path: ["serviceMonths"],
    },
  );

export type SupplementaryForm = z.infer<typeof supplementarySchema>;
