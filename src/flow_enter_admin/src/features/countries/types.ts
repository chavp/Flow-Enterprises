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

export type CountryTreeNode = {
  country: Country;
  provinces: Province[];
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
