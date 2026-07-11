import { requestJson } from "./http";
import {
  CountriesResponse,
  CreateCountryRequest,
  PatchOperation,
  UpdateCountryRequest
} from "../features/countries/types";

export async function fetchCountries(
  pageNumber: number,
  pageSize: number,
  apiBaseUrl?: string
): Promise<CountriesResponse> {
  return requestJson<CountriesResponse>(
    `/api/geographic-boundaries/countries?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    { method: "GET" },
    apiBaseUrl
  );
}

export async function createCountry(payload: CreateCountryRequest, apiBaseUrl?: string): Promise<void> {
  await requestJson("/api/geographic-boundaries/countries", { method: "POST", body: JSON.stringify(payload) }, apiBaseUrl);
}

function toPatchOperations(changes: UpdateCountryRequest["changes"]): PatchOperation[] {
  const operations: PatchOperation[] = [];

  const entries = Object.entries(changes) as Array<[string, string | number | undefined]>;
  for (const [key, value] of entries) {
    if (value === undefined) {
      continue;
    }

    operations.push({
      op: "replace",
      path: `/${key.charAt(0).toUpperCase()}${key.slice(1)}`,
      value
    });
  }

  return operations;
}

export async function updateCountry(payload: UpdateCountryRequest, apiBaseUrl?: string): Promise<void> {
  const operations = toPatchOperations(payload.changes);
  if (operations.length === 0) {
    return;
  }

  await requestJson(
    `/api/geographic-boundaries/countries/${payload.id}`,
    {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json-patch+json"
      },
      body: JSON.stringify(operations)
    },
    apiBaseUrl
  );
}
