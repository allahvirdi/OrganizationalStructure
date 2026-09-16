import { z } from "zod";

/**
 * اعتبارسنجی فرم مسئولیت (همسو با Backend).
 */
export const responsibilitySchema = z.object({
  code: z
    .string()
    .min(1, "کد مسئولیت الزامی است.")
    .max(100, "کد حداکثر ۱۰۰ کاراکتر است."),
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
