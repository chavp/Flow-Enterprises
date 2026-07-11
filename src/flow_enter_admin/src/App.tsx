import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ConfigProvider, Layout, Menu, Tabs } from "antd";
import { useMemo, useState } from "react";
import { EnterprisesPage } from "./features/enterprises/EnterprisesPage";
import "./styles.css";
import "antd/dist/reset.css";

const MAIN_MENU_ITEMS = [{ key: "enterprises", label: "Enterprises" }];
const SUB_MENU_ITEMS: Record<string, { key: string; label: string }[]> = {
  enterprises: [{ key: "enterpeise", label: "Enterpeise" }]
};

type AppProps = {
  apiBaseUrl?: string;
};

export function App({ apiBaseUrl }: AppProps) {
  const queryClient = useMemo(() => new QueryClient(), []);
  const [domainKey, setDomainKey] = useState("enterprises");
  const [subdomainKey, setSubdomainKey] = useState("enterpeise");

  const subdomainItems = SUB_MENU_ITEMS[domainKey] ?? [];

  return (
    <ConfigProvider>
      <QueryClientProvider client={queryClient}>
        <Layout className="app-shell">
          <Layout.Sider width={220} theme="light" className="app-shell-sider">
            <div className="app-shell-sider-title">Domains</div>
            <Menu
              mode="inline"
              selectedKeys={[domainKey]}
              items={MAIN_MENU_ITEMS}
              onClick={(info) => {
                setDomainKey(info.key);
                setSubdomainKey((SUB_MENU_ITEMS[info.key] ?? [])[0]?.key ?? "");
              }}
            />
          </Layout.Sider>
          <Layout>
            <Layout.Header className="app-shell-header">
              <Tabs
                activeKey={subdomainKey}
                items={subdomainItems}
                onChange={setSubdomainKey}
                tabBarStyle={{ marginBottom: 0 }}
              />
            </Layout.Header>
            <Layout.Content className="app-shell-content">
              {domainKey === "enterprises" && subdomainKey === "enterpeise" ? (
                <EnterprisesPage apiBaseUrl={apiBaseUrl} />
              ) : null}
            </Layout.Content>
          </Layout>
        </Layout>
      </QueryClientProvider>
    </ConfigProvider>
  );
}
