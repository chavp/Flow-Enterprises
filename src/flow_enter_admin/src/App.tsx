import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ConfigProvider } from "antd";
import { useMemo } from "react";
import { EnterprisesPage } from "./features/enterprises/EnterprisesPage";
import "./styles.css";
import "antd/dist/reset.css";

type AppProps = {
  apiBaseUrl?: string;
};

export function App({ apiBaseUrl }: AppProps) {
  const queryClient = useMemo(() => new QueryClient(), []);

  return (
    <ConfigProvider>
      <QueryClientProvider client={queryClient}>
        <EnterprisesPage apiBaseUrl={apiBaseUrl} />
      </QueryClientProvider>
    </ConfigProvider>
  );
}
