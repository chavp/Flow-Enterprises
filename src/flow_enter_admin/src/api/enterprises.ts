import { requestJson } from "./http";
import {
  Bed,
  Building,
  CreateBuildingRequest,
  CreateBedRequest,
  CreateEmploymentRequest,
  CreateEnterpriseBranchRequest,
  CreateEnterpriseRequest,
  CreateFloorRequest,
  CreateRoomRequest,
  EnterpriseFacilitiesTree,
  EnterpriseBranch,
  Employment,
  Floor,
  LegalStructure,
  EnterprisesResponse,
  PatchOperation,
  PartyRoleType,
  Room,
  UpdateBuildingRequest,
  UpdateEmploymentEffectiveDateRequest,
  UpdateEnterpriseBranchRequest,
  UpdateEmploymentRequest,
  UpdateFloorRequest,
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

export async function updateEnterpriseEmploymentBranchEffectiveDate(
  enterpriseId: string,
  employmentId: string,
  branchId: string,
  payload: UpdateEmploymentEffectiveDateRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/employments/${employmentId}/branches/${branchId}/effective-date`,
    { method: "PATCH", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function fetchEnterpriseBranchs(enterpriseId: string, apiBaseUrl?: string): Promise<EnterpriseBranch[]> {
  return requestJson<EnterpriseBranch[]>(`/api/parties/enterprises/${enterpriseId}/branchs`, { method: "GET" }, apiBaseUrl);
}

export async function createEnterpriseBranch(
  enterpriseId: string,
  payload: CreateEnterpriseBranchRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/branchs`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateEnterpriseBranch(
  enterpriseId: string,
  enterpriseBranchId: string,
  payload: UpdateEnterpriseBranchRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/branchs/${enterpriseBranchId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function deleteEnterpriseBranch(
  enterpriseId: string,
  enterpriseBranchId: string,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/branchs/${enterpriseBranchId}`,
    { method: "DELETE" },
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

export async function fetchEnterpriseFacilitiesTree(
  enterpriseId: string,
  apiBaseUrl?: string
): Promise<EnterpriseFacilitiesTree> {
  return requestJson<EnterpriseFacilitiesTree>(
    `/api/parties/enterprises/${enterpriseId}/facilities/tree`,
    { method: "GET" },
    apiBaseUrl
  );
}

export async function createEnterpriseBuilding(
  enterpriseId: string,
  payload: CreateBuildingRequest,
  apiBaseUrl?: string
): Promise<Building> {
  return requestJson<Building>(
    `/api/parties/enterprises/${enterpriseId}/facilities/buildings`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateEnterpriseBuilding(
  enterpriseId: string,
  buildingId: string,
  payload: UpdateBuildingRequest,
  apiBaseUrl?: string
): Promise<Building> {
  return requestJson<Building>(
    `/api/parties/enterprises/${enterpriseId}/facilities/buildings/${buildingId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function deleteEnterpriseBuilding(
  enterpriseId: string,
  buildingId: string,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/buildings/${buildingId}`,
    { method: "DELETE" },
    apiBaseUrl
  );
}

export async function createEnterpriseFloor(
  enterpriseId: string,
  payload: CreateFloorRequest,
  apiBaseUrl?: string
): Promise<Floor> {
  return requestJson<Floor>(
    `/api/parties/enterprises/${enterpriseId}/facilities/floors`,
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateEnterpriseFloor(
  enterpriseId: string,
  floorId: string,
  payload: UpdateFloorRequest,
  apiBaseUrl?: string
): Promise<Floor> {
  return requestJson<Floor>(
    `/api/parties/enterprises/${enterpriseId}/facilities/floors/${floorId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function deleteEnterpriseFloor(
  enterpriseId: string,
  floorId: string,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/floors/${floorId}`,
    { method: "DELETE" },
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

export async function deleteEnterpriseBed(
  enterpriseId: string,
  bedId: string,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/parties/enterprises/${enterpriseId}/facilities/beds/${bedId}`,
    { method: "DELETE" },
    apiBaseUrl
  );
}
