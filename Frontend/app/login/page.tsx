import { redirect } from "next/navigation";

/** هدایت نشانی قدیمی ورود به صفحه لندینگ سامانه. */
export default function LoginRedirectPage() {
  redirect("/");
}
