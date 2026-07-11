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
