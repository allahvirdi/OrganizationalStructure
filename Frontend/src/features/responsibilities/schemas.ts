import { z } from "zod";

/**
 * اعتبارسنجی فرم مسئولیت (همسو با Backend).
 * کد (Routing Key) به‌صورت خودکار در بک‌اند تولید می‌شود.
 */
export const responsibilitySchema = z.object({
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

export type ResponsibilityForm = z.infer<typeof responsibilitySchema>;
