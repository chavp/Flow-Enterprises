import { requestJson } from "./http";
import {
  CountryTreeNode,
  CreateDistrictRequest,
  CreateProvinceRequest,
  CreateSubdistrictRequest,
  CountriesResponse,
  CreateCountryRequest,
  PatchOperation,
  Province,
  UpdateDistrictRequest,
  UpdateProvinceRequest,
  UpdateSubdistrictRequest,
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

export async function fetchCountriesTree(apiBaseUrl?: string): Promise<CountryTreeNode[]> {
  return requestJson<CountryTreeNode[]>("/api/geographic-boundaries/countries/tree", { method: "GET" }, apiBaseUrl);
}

export async function fetchCountryProvinces(countryId: string, apiBaseUrl?: string): Promise<Province[]> {
  return requestJson<Province[]>(`/api/geographic-boundaries/countries/${countryId}/provinces`, { method: "GET" }, apiBaseUrl);
}

export async function createProvince(
  countryId: string,
  payload: CreateProvinceRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/geographic-boundaries/countries/${countryId}/provinces`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateProvince(payload: UpdateProvinceRequest, apiBaseUrl?: string): Promise<void> {
  const operations = toPatchOperations(payload.changes);
  if (operations.length === 0) {
    return;
  }

  await requestJson(
    `/api/geographic-boundaries/provinces/${payload.id}`,
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

export async function deleteProvince(provinceId: string, apiBaseUrl?: string): Promise<void> {
  await requestJson(`/api/geographic-boundaries/provinces/${provinceId}`, { method: "DELETE" }, apiBaseUrl);
}

export async function createDistrict(
  provinceId: string,
  payload: CreateDistrictRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/geographic-boundaries/provinces/${provinceId}/districts`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateDistrict(payload: UpdateDistrictRequest, apiBaseUrl?: string): Promise<void> {
  const operations = toPatchOperations(payload.changes);
  if (operations.length === 0) {
    return;
  }

  await requestJson(
    `/api/geographic-boundaries/districts/${payload.id}`,
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

export async function deleteDistrict(districtId: string, apiBaseUrl?: string): Promise<void> {
  await requestJson(`/api/geographic-boundaries/districts/${districtId}`, { method: "DELETE" }, apiBaseUrl);
}

export async function createSubdistrict(
  districtId: string,
  payload: CreateSubdistrictRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/geographic-boundaries/districts/${districtId}/subdistricts`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateSubdistrict(payload: UpdateSubdistrictRequest, apiBaseUrl?: string): Promise<void> {
  const operations = toPatchOperations(payload.changes);
  if (operations.length === 0) {
    return;
  }

  await requestJson(
    `/api/geographic-boundaries/subdistricts/${payload.id}`,
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

export async function deleteSubdistrict(subdistrictId: string, apiBaseUrl?: string): Promise<void> {
  await requestJson(`/api/geographic-boundaries/subdistricts/${subdistrictId}`, { method: "DELETE" }, apiBaseUrl);
}
