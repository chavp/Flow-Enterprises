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
