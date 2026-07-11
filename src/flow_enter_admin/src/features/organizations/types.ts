export type Organization = {
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

export type OrganizationsResponse = {
  data: Organization[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
};

export type LegalStructure = {
  id: string;
  code?: string;
  name: string;
};

export type CreateOrganizationRequest = {
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

export type UpdateOrganizationRequest = {
  id: string;
  changes: Partial<CreateOrganizationRequest>;
};
