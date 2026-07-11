import { mountOrganizationsApp } from "./mfe";

const rootElement = document.getElementById("root");

if (rootElement) {
  const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined;
  mountOrganizationsApp(rootElement, { apiBaseUrl });
}
