import { requestJson } from "./http";
import {
  CreateOrganizationRequest,
  LegalStructure,
  OrganizationsResponse,
  PatchOperation,
  UpdateOrganizationRequest
} from "../features/organizations/types";

export async function fetchOrganizations(
  pageNumber: number,
  pageSize: number,
  apiBaseUrl?: string
): Promise<OrganizationsResponse> {
  return requestJson<OrganizationsResponse>(
    `/api/parties/enterprises?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    { method: "GET" },
    apiBaseUrl
  );
}

export async function fetchLegalStructures(apiBaseUrl?: string): Promise<LegalStructure[]> {
  return requestJson<LegalStructure[]>("/api/parties/legal-structures", { method: "GET" }, apiBaseUrl);
}

export async function createOrganization(payload: CreateOrganizationRequest, apiBaseUrl?: string): Promise<void> {
  await requestJson("/api/parties/enterprises", { method: "POST", body: JSON.stringify(payload) }, apiBaseUrl);
}

function toPatchOperations(changes: UpdateOrganizationRequest["changes"]): PatchOperation[] {
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

export async function updateOrganization(payload: UpdateOrganizationRequest, apiBaseUrl?: string): Promise<void> {
  const operations = toPatchOperations(payload.changes);
  if (operations.length === 0) {
    return;
  }

  await requestJson(
    `/api/parties/enterprises/${payload.id}`,
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
