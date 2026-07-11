import { createRoot, Root } from "react-dom/client";
import { App } from "./App";

export type OrganizationsAppOptions = {
  apiBaseUrl?: string;
};

export type UnmountFn = () => void;

export function mountOrganizationsApp(
  container: Element,
  options: OrganizationsAppOptions = {}
): UnmountFn {
  const root: Root = createRoot(container);
  root.render(<App apiBaseUrl={options.apiBaseUrl} />);
  return () => root.unmount();
}

export default mountOrganizationsApp;
