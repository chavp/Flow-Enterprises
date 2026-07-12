export type Enterprise = {
  enterpriseId: string;
  legalName: string;
  information?: string;
  brandName?: string;
  notes?: string;
  legalStructureId?: string;
  businessRegistrationNumber?: string;
  taxId?: string;
  fiscalYearStartMonth: number;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type EnterprisesResponse = {
  data: Enterprise[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
};

export type LegalStructure = {
  id: string;
  code?: string;
  name: string;
};

export type CreateEnterpriseRequest = {
  legalName: string;
  information?: string;
  brandName?: string;
  notes?: string;
  legalStructureId: string;
  businessRegistrationNumber?: string;
  taxId?: string;
  fiscalYearStartMonth: number;
};

export type PatchOperation = {
  op: "add" | "replace" | "remove";
  path: string;
  value?: string | number | null;
};

export type UpdateEnterpriseRequest = {
  id: string;
  changes: Partial<CreateEnterpriseRequest>;
};

export type PartyRoleType = {
  id: string;
  code?: string;
  name: string;
  description?: string;
};

export type Employment = {
  employmentId: string;
  employerId: string;
  employeePartyRoleId: string;
  employeePartyId: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  employeeFullName: string;
  partyRoleTypeId: string;
  partyRoleTypeCode: string;
  partyRoleTypeName: string;
  fromDate: string;
  thruDate: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateEmploymentRequest = {
  firstName: string;
  middleName?: string;
  lastName: string;
  partyRoleTypeIds: string[];
};

export type UpdateEmploymentRequest = CreateEmploymentRequest;

export type UpdateEmploymentEffectiveDateRequest = {
  fromDate: string;
  thruDate: string;
};

export type Room = {
  roomId: string;
  number: string;
  description?: string;
  bedCount: number;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateRoomRequest = {
  number: string;
  description?: string;
};

export type UpdateRoomRequest = CreateRoomRequest;

export type Bed = {
  bedId: string;
  number: string;
  description?: string;
  roomId: string;
  roomNumber: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateBedRequest = {
  number: string;
  roomId: string;
  description?: string;
};

export type UpdateBedRequest = CreateBedRequest;
