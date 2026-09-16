/**
 * خطای استاندارد API (ProblemDetails).
 */
export class ApiError extends Error {
  readonly status: number;
  readonly code?: string;

  constructor(status: number, code: string | undefined, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.code = code;
  }
}

type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

interface RequestOptions {
  method?: HttpMethod;
  body?: unknown;
  query?: Record<string, string | number | boolean | undefined>;
}

/**
 * فراخوانی یکپارچه API با کوکی BFF (credentials: include).
 * خطاها به ApiError با پیام فارسی سرور تبدیل می‌شوند.
 */
export async function apiFetch<T>(
  path: string,
  { method = "GET", body, query }: RequestOptions = {},
): Promise<T> {
  const url = new URL(path, window.location.origin);
  if (query) {
    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined) {
        url.searchParams.set(key, String(value));
      }
    }
  }

  const response = await fetch(url, {
    method,
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (response.status === 204) {
    return undefined as T;
  }

  const isJson = response.headers
    .get("content-type")
    ?.includes("application/json");

  const payload = isJson ? await response.json() : null;

  if (!response.ok) {
    const detail =
      (payload as { detail?: string; title?: string } | null)?.detail ??
      (payload as { title?: string } | null)?.title ??
      "خطایی رخ داده است.";
    const code = (payload as { title?: string } | null)?.title;
    throw new ApiError(response.status, code, detail);
  }

  return payload as T;
}
