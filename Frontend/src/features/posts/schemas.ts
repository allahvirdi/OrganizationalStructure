import { z } from "zod";

/**
 * اعتبارسنجی فرم پست (همسو با Backend).
 */
export const postSchema = z.object({
  code: z
    .string()
    .min(1, "کد پست الزامی است.")
    .max(50, "کد پست حداکثر ۵۰ کاراکتر است."),
  title: z
    .string()
    .min(1, "عنوان پست الزامی است.")
    .max(200, "عنوان پست حداکثر ۲۰۰ کاراکتر است."),
  description: z
    .string()
    .max(1000, "شرح حداکثر ۱۰۰۰ کاراکتر است.")
    .optional()
    .or(z.literal("")),
  parentId: z.string().optional().or(z.literal("")),
});

export type PostForm = z.infer<typeof postSchema>;
