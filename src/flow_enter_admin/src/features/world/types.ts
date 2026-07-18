export type CurrencyMeasure = {
  currencyMeasureId: string;
  abbreviation: string;
  description?: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateCurrencyMeasureRequest = {
  abbreviation: string;
  description?: string;
};

export type UpdateCurrencyMeasureRequest = CreateCurrencyMeasureRequest;

export type TimeFrequencyMeasure = {
  timeFrequencyMeasureId: string;
  abbreviation: string;
  description?: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  revision: number;
};

export type CreateTimeFrequencyMeasureRequest = {
  abbreviation: string;
  description?: string;
};

export type UpdateTimeFrequencyMeasureRequest = CreateTimeFrequencyMeasureRequest;
