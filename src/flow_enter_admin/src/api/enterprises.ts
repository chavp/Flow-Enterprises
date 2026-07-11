import { requestJson } from "./http";
import {
  Bed,
  CreateBedRequest,
  CreateEmploymentRequest,
  CreateEnterpriseRequest,
  CreateRoomRequest,
  Employment,
  LegalStructure,
  EnterprisesResponse,
  PatchOperation,
  PartyRoleType,
  Room,
  UpdateEmploymentEffectiveDateRequest,
  UpdateEmploymentRequest,
  UpdateBedRequest,
  UpdateRoomRequest,
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

export async function deleteEnterprise(enterpriseId: string, apiBaseUrl?: string): Promise<void> {
  await requestJson(`/api/parties/enterprises/${enterpriseId}`, { method: "DELETE" }, apiBaseUrl);
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

export async function updateEnterpriseEmploymentEffectiveDate(
  enterpriseId: string,
  employmentId: string,
  payload: UpdateEmploymentEffectiveDateRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/employments/${employmentId}/effective-date`,
    { method: "PATCH", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function fetchEnterpriseRooms(
  enterpriseId: string,
  searchText: string,
  apiBaseUrl?: string
): Promise<Room[]> {
  const query = new URLSearchParams();
  if (searchText.trim().length > 0) {
    query.set("searchText", searchText.trim());
  }

  return requestJson<Room[]>(
    `/api/parties/enterprises/${enterpriseId}/facilities/rooms${query.toString() ? `?${query.toString()}` : ""}`,
    { method: "GET" },
    apiBaseUrl
  );
}

export async function createEnterpriseRoom(
  enterpriseId: string,
  payload: CreateRoomRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/rooms`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateEnterpriseRoom(
  enterpriseId: string,
  roomId: string,
  payload: UpdateRoomRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/rooms/${roomId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function deleteEnterpriseRoom(
  enterpriseId: string,
  roomId: string,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/rooms/${roomId}`,
    { method: "DELETE" },
    apiBaseUrl
  );
}

export async function fetchEnterpriseBeds(
  enterpriseId: string,
  searchText: string,
  apiBaseUrl?: string
): Promise<Bed[]> {
  const query = new URLSearchParams();
  if (searchText.trim().length > 0) {
    query.set("searchText", searchText.trim());
  }

  return requestJson<Bed[]>(
    `/api/parties/enterprises/${enterpriseId}/facilities/beds${query.toString() ? `?${query.toString()}` : ""}`,
    { method: "GET" },
    apiBaseUrl
  );
}

export async function createEnterpriseBed(
  enterpriseId: string,
  payload: CreateBedRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/beds`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateEnterpriseBed(
  enterpriseId: string,
  bedId: string,
  payload: UpdateBedRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/beds/${bedId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}
