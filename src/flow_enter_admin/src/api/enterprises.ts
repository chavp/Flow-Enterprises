import { requestJson } from "./http";
import {
  CreateEnterpriseRequest,
  LegalStructure,
  EnterprisesResponse,
  PatchOperation,
  UpdateEnterpriseRequest
} from "../features/enterprises/types";

export async function fetchEnterprises(
  pageNumber: number,
  pageSize: number,
  apiBaseUrl?: string
): Promise<EnterprisesResponse> {
  return requestJson<EnterprisesResponse>(
    `/api/parties/enterprises?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    { method: "GET" },
    apiBaseUrl
  );
}

export async function fetchLegalStructures(apiBaseUrl?: string): Promise<LegalStructure[]> {
  return requestJson<LegalStructure[]>("/api/parties/enterprises/legal-structures", { method: "GET" }, apiBaseUrl);
}

export async function createEnperprise(payload: CreateEnterpriseRequest, apiBaseUrl?: string): Promise<void> {
  await requestJson("/api/parties/enterprises", { method: "POST", body: JSON.stringify(payload) }, apiBaseUrl);
}

function toPatchOperations(changes: UpdateEnterpriseRequest["changes"]): PatchOperation[] {
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

export async function updateEnperprise(payload: UpdateEnterpriseRequest, apiBaseUrl?: string): Promise<void> {
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
