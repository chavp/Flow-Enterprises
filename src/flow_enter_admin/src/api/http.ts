export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number
  ) {
    super(message);
    this.name = "ApiError";
  }
}

export function resolveApiBaseUrl(override?: string): string {
  const configuredBaseUrl = override ?? import.meta.env.VITE_API_BASE_URL;
  if (configuredBaseUrl && configuredBaseUrl.trim().length > 0) {
    return configuredBaseUrl.replace(/\/+$/, "");
  }

  return "";
}

export async function requestJson<T>(
  path: string,
  init?: RequestInit,
  apiBaseUrl?: string
): Promise<T> {
  const response = await fetch(`${resolveApiBaseUrl(apiBaseUrl)}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers ?? {})
    }
  });

  if (!response.ok) {
    let message = `Request failed with status ${response.status}`;
    try {
      const body = await response.json();
      if (body && typeof body === "object") {
        message = JSON.stringify(body);
      }
    } catch {
      // keep default message when response body isn't JSON
    }
    throw new ApiError(message, response.status);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}
