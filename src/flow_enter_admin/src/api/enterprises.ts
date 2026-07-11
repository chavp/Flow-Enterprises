import { requestJson } from "./http";
import {
  CreateEmploymentRequest,
  CreateEnterpriseRequest,
  Employment,
  LegalStructure,
  EnterprisesResponse,
  PatchOperation,
  PartyRoleType,
  UpdateEmploymentRequest,
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

export async function fetchPartyRoleTypes(apiBaseUrl?: string): Promise<PartyRoleType[]> {
  return requestJson<PartyRoleType[]>("/api/parties/role-types", { method: "GET" }, apiBaseUrl);
}

export async function fetchEnterpriseEmployments(enterpriseId: string, apiBaseUrl?: string): Promise<Employment[]> {
  return requestJson<Employment[]>(`/api/parties/enterprises/${enterpriseId}/employments`, { method: "GET" }, apiBaseUrl);
}

export async function createEnterpriseEmployment(
  enterpriseId: string,
  payload: CreateEmploymentRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/employments`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateEnterpriseEmployment(
  enterpriseId: string,
  employmentId: string,
  payload: UpdateEmploymentRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/employments/${employmentId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function deleteEnterpriseEmployment(
  enterpriseId: string,
  employmentId: string,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/employments/${employmentId}`,
    { method: "DELETE" },
    apiBaseUrl
  );
}
