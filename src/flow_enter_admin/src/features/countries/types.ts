export type Country = {
  id: string;
  name: string;
  nationality: string;
  numeric?: number;
  isoCode2: string;
  isoCode3: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CountriesResponse = {
  data: Country[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
};

export type CreateCountryRequest = {
  name: string;
  nationality: string;
  numeric?: number;
  isoCode2: string;
  isoCode3: string;
};

export type PatchOperation = {
  op: "add" | "replace" | "remove";
  path: string;
  value?: string | number | null;
};

export type UpdateCountryRequest = {
  id: string;
  changes: Partial<CreateCountryRequest>;
};

export type Province = {
  id: string;
  countryId: string;
  name: string;
  hs?: string;
  iso?: string;
  fips?: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type District = {
  id: string;
  provinceId: string;
  name: string;
  prefixName?: string;
  prefixShortName?: string;
  postalCode?: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type Subdistrict = {
  id: string;
  districtId: string;
  name: string;
  prefixName?: string;
  prefixShortName?: string;
  postalCode?: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type DistrictTreeNode = {
  district: District;
  subdistricts: Subdistrict[];
};

export type ProvinceTreeNode = {
  province: Province;
  districts: DistrictTreeNode[];
};

export type CountryTreeNode = {
  country: Country;
  provinces: ProvinceTreeNode[];
};

export type CreateProvinceRequest = {
  name: string;
  hs?: string;
  iso?: string;
  fips?: string;
};

export type UpdateProvinceRequest = {
  id: string;
  changes: Partial<CreateProvinceRequest>;
};

export type CreateDistrictRequest = {
  name: string;
  prefixName?: string;
  prefixShortName?: string;
  postalCode?: string;
};

export type UpdateDistrictRequest = {
  id: string;
  changes: Partial<CreateDistrictRequest>;
};

export type CreateSubdistrictRequest = {
  name: string;
  prefixName?: string;
  prefixShortName?: string;
  postalCode?: string;
};

export type UpdateSubdistrictRequest = {
  id: string;
  changes: Partial<CreateSubdistrictRequest>;
};
