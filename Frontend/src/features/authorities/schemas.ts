import { z } from "zod";

/**
 * اعتبارسنجی فرم حق امضا (همسو با Backend).
 *
 * کد به‌صورت خودکار در بک‌اند تولید می‌شود (GUID) و ورودی کاربر نیست.
 */
export const authoritySchema = z.object({
  title: z
    .string()
    .min(1, "عنوان الزامی است.")
    .max(200, "عنوان حداکثر ۲۰۰ کاراکتر است."),
  description: z
    .string()
    .max(1000, "شرح حداکثر ۱۰۰۰ کاراکتر است.")
    .optional()
    .or(z.literal("")),
});

export type AuthorityForm = z.infer<typeof authoritySchema>;
