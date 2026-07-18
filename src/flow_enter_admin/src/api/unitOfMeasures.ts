import { requestJson } from "./http";
import type {
  CreateCurrencyMeasureRequest,
  CreateTimeFrequencyMeasureRequest,
  CurrencyMeasure,
  TimeFrequencyMeasure,
  UpdateCurrencyMeasureRequest,
  UpdateTimeFrequencyMeasureRequest
} from "../features/world/types";

export async function fetchCurrencyMeasures(apiBaseUrl?: string): Promise<CurrencyMeasure[]> {
  return requestJson<CurrencyMeasure[]>("/api/products/unit-of-measures/currency-measures", { method: "GET" }, apiBaseUrl);
}

export async function createCurrencyMeasure(payload: CreateCurrencyMeasureRequest, apiBaseUrl?: string): Promise<void> {
  await requestJson(
    "/api/products/unit-of-measures/currency-measures",
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateCurrencyMeasure(
  currencyMeasureId: string,
  payload: UpdateCurrencyMeasureRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/products/unit-of-measures/currency-measures/${currencyMeasureId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function fetchTimeFrequencyMeasures(apiBaseUrl?: string): Promise<TimeFrequencyMeasure[]> {
  return requestJson<TimeFrequencyMeasure[]>("/api/products/unit-of-measures/time-frequency-measures", { method: "GET" }, apiBaseUrl);
}

export async function createTimeFrequencyMeasure(payload: CreateTimeFrequencyMeasureRequest, apiBaseUrl?: string): Promise<void> {
  await requestJson(
    "/api/products/unit-of-measures/time-frequency-measures",
    { method: "POST", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}

export async function updateTimeFrequencyMeasure(
  timeFrequencyMeasureId: string,
  payload: UpdateTimeFrequencyMeasureRequest,
  apiBaseUrl?: string
): Promise<void> {
  await requestJson(
    `/api/products/unit-of-measures/time-frequency-measures/${timeFrequencyMeasureId}`,
    { method: "PUT", body: JSON.stringify(payload) },
    apiBaseUrl
  );
}
