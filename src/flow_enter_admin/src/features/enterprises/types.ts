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

export type EnterpriseService = {
  serviceId: string;
  enterpriseId: string;
  name: string;
  description?: string;
  releaseDate?: string;
  discontinuedDate?: string;
  supportDiscontinuedDate?: string;
  hasCoverImage: boolean;
  coverImageName?: string;
  featureCount: number;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type EnterpriseGood = {
  goodId: string;
  enterpriseId: string;
  name: string;
  description?: string;
  releaseDate?: string;
  discontinuedDate?: string;
  supportDiscontinuedDate?: string;
  hasCoverImage: boolean;
  coverImageName?: string;
  featureCount: number;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type EnterpriseServiceFeatureApplicability = {
  productFeatureApplicabilityId: string;
  productFeatureId: string;
  productFeatureCode: string;
  productFeatureTitle: string;
  productFeatureApplicabilityType: string;
  order: number;
};

export type EnterpriseServiceFeatureApplicabilityRequest = {
  productFeatureId: string;
  productFeatureApplicabilityType: string;
  order: number;
};

export type EnterpriseServicePriceCoponent = {
  priceCoponentId: string;
  priceCoponentType: string;
  price?: number;
  percent?: number;
  unitOfMeasureId?: string;
  unitOfMeasureAbbreviation?: string;
  timeFrequencyMeasureId?: string;
  timeFrequencyMeasureAbbreviation?: string;
  fromDate: string;
  thruDate: string;
  description?: string;
};

export type EnterpriseServicePriceCoponentRequest = {
  priceCoponentType: string;
  price?: number;
  percent?: number;
  unitOfMeasureId?: string;
  timeFrequencyMeasureId?: string;
  fromDate?: string;
  thruDate?: string;
  description?: string;
};

export type CreateEnterpriseServiceRequest = {
  name: string;
  description?: string;
  releaseDate?: string;
  discontinuedDate?: string;
  supportDiscontinuedDate?: string;
  coverImage?: string;
  coverImageName?: string;
  productFeatureApplicabilities: EnterpriseServiceFeatureApplicabilityRequest[];
  priceCoponents: EnterpriseServicePriceCoponentRequest[];
};

export type UpdateEnterpriseServiceRequest = CreateEnterpriseServiceRequest & {
  removeCoverImage?: boolean;
};

export type ProductFeatureCategory = {
  productFeatureCategoryId: string;
  name: string;
  isGlobal: boolean;
};

export type CreateProductFeatureCategoryRequest = {
  name: string;
};

export type UpdateProductFeatureCategoryRequest = CreateProductFeatureCategoryRequest;

export type EnterpriseProductFeature = {
  productFeatureId: string;
  enterpriseId: string;
  productFeatureCategoryId: string;
  productFeatureCategoryName: string;
  productFeatureType: string;
  code: string;
  title: string;
  description?: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateEnterpriseProductFeatureRequest = {
  productFeatureType: string;
  productFeatureCategoryId: string;
  code: string;
  title: string;
  description?: string;
};

export type UpdateEnterpriseProductFeatureRequest = CreateEnterpriseProductFeatureRequest;

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
  employmentNumber: string;
  employerId: string;
  employeePartyRoleId: string;
  employeePartyId: string;
  branchIds: string[];
  branchLegalNames: string[];
  branchEmployments: EmploymentBranch[];
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
  employmentNumber: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  partyRoleTypeIds: string[];
  branchIds: string[];
};

export type UpdateEmploymentRequest = CreateEmploymentRequest;

export type UpdateEmploymentEffectiveDateRequest = {
  fromDate: string;
  thruDate: string;
};

export type EmploymentBranch = {
  branchId: string;
  branchLegalName: string;
  fromDate: string;
  thruDate: string;
};

export type EnterpriseBranch = {
  enterpriseBranchId: string;
  enterpriseId: string;
  branchId: string;
  branchLegalName: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateEnterpriseBranchRequest = {
  name: string;
};

export type UpdateEnterpriseBranchRequest = CreateEnterpriseBranchRequest;

export type Room = {
  roomId: string;
  number: string;
  description?: string;
  floorId: string;
  floorLevel: number;
  buildingId: string;
  buildingName: string;
  bedCount: number;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateRoomRequest = {
  number: string;
  floorId: string;
  description?: string;
};

export type UpdateRoomRequest = CreateRoomRequest;

export type Building = {
  buildingId: string;
  name: string;
  description?: string;
  branchIds: string[];
  branchLegalNames: string[];
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type Floor = {
  floorId: string;
  level: number;
  description?: string;
  buildingId: string;
  buildingName: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type Bed = {
  bedId: string;
  number: string;
  description?: string;
  roomId: string;
  roomNumber: string;
  floorId?: string;
  floorLevel?: number;
  buildingId?: string;
  buildingName?: string;
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

export type CreateBuildingRequest = {
  name: string;
  description?: string;
  branchIds: string[];
};

export type UpdateBuildingRequest = CreateBuildingRequest;

export type CreateFloorRequest = {
  buildingId: string;
  level: number;
  description?: string;
};

export type UpdateFloorRequest = CreateFloorRequest;

export type RoomFacilitiesNode = {
  room: Room;
  beds: Bed[];
};

export type FloorFacilitiesNode = {
  floor: Floor;
  rooms: RoomFacilitiesNode[];
};

export type BuildingFacilitiesNode = {
  building: Building;
  floors: FloorFacilitiesNode[];
};

export type EnterpriseFacilitiesTree = {
  buildings: BuildingFacilitiesNode[];
};
