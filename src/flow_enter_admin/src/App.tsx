import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ConfigProvider, Layout, Menu, Tabs } from "antd";
import { ApartmentOutlined, GlobalOutlined, BankOutlined, EnvironmentOutlined } from "@ant-design/icons";
import { useEffect, useMemo, useState, type ReactNode } from "react";
import { EnterprisesPage } from "./features/enterprises/EnterprisesPage";
import { CountriesPage } from "./features/countries/CountriesPage";
import "./styles.css";
import "antd/dist/reset.css";

const MAIN_MENU_ITEMS = [
  { key: "enterprises", label: "Enterprises", icon: <ApartmentOutlined /> },
  { key: "world", label: "World", icon: <GlobalOutlined /> }
];
const SUB_MENU_ITEMS: Record<string, { key: string; label: string; icon: ReactNode }[]> = {
  enterprises: [{ key: "enterpeise", label: "Enterpeise", icon: <BankOutlined /> }],
  world: [{ key: "geographic-boundaries", label: "Geographic Boundaries", icon: <EnvironmentOutlined /> }]
};
const DEFAULT_DOMAIN_KEY = "enterprises";
const DEFAULT_SUBDOMAIN_KEY = "enterpeise";

type AppProps = {
  apiBaseUrl?: string;
};

function getFirstSubdomainKey(domainKey: string): string {
  return (SUB_MENU_ITEMS[domainKey] ?? [])[0]?.key ?? "";
}

function getInitialNavigationState() {
  const params = new URLSearchParams(window.location.search);
  const requestedDomain = params.get("domain") ?? DEFAULT_DOMAIN_KEY;
  const resolvedDomain = SUB_MENU_ITEMS[requestedDomain] ? requestedDomain : DEFAULT_DOMAIN_KEY;

  const requestedSubdomain = params.get("subdomain");
  const allowedSubdomains = (SUB_MENU_ITEMS[resolvedDomain] ?? []).map((item) => item.key);
  const resolvedSubdomain =
    requestedSubdomain && allowedSubdomains.includes(requestedSubdomain)
      ? requestedSubdomain
      : getFirstSubdomainKey(resolvedDomain) || DEFAULT_SUBDOMAIN_KEY;

  return { domainKey: resolvedDomain, subdomainKey: resolvedSubdomain };
}

export function App({ apiBaseUrl }: AppProps) {
  const queryClient = useMemo(() => new QueryClient(), []);
  const initialNavigationState = useMemo(() => getInitialNavigationState(), []);
  const [domainKey, setDomainKey] = useState(initialNavigationState.domainKey);
  const [subdomainKey, setSubdomainKey] = useState(initialNavigationState.subdomainKey);

  const subdomainItems = (SUB_MENU_ITEMS[domainKey] ?? []).map((item) => ({
    key: item.key,
    label: (
      <span style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
        {item.icon}
        {item.label}
      </span>
    )
  }));

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    params.set("domain", domainKey);
    params.set("subdomain", subdomainKey);

    const queryString = params.toString();
    const nextUrl = `${window.location.pathname}${queryString ? `?${queryString}` : ""}${window.location.hash}`;
    window.history.replaceState(window.history.state, "", nextUrl);
  }, [domainKey, subdomainKey]);

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
                const nextDomainKey = String(info.key);
                setDomainKey(nextDomainKey);
                setSubdomainKey(getFirstSubdomainKey(nextDomainKey));
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
              ) : domainKey === "world" && subdomainKey === "geographic-boundaries" ? (
                <CountriesPage apiBaseUrl={apiBaseUrl} />
              ) : null}
            </Layout.Content>
          </Layout>
        </Layout>
      </QueryClientProvider>
    </ConfigProvider>
  );
}
