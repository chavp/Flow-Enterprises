import { AppstoreOutlined, GiftOutlined, ToolOutlined } from "@ant-design/icons";
import { Button, Card, Empty, Layout, Menu, Space, Tabs, Typography } from "antd";
import type { Enterprise } from "../types";

const { Title, Text } = Typography;

type EnterpriseProductsPageProps = {
  enterprise: Enterprise;
  productsTabKey: string;
  onProductsTabChange: (key: string) => void;
  productManagementTabKey: string;
  onProductManagementTabChange: (key: string) => void;
  onBack: () => void;
};

export function EnterpriseProductsPage({
  enterprise,
  productsTabKey,
  onProductsTabChange,
  productManagementTabKey,
  onProductManagementTabChange,
  onBack
}: EnterpriseProductsPageProps) {
  return (
    <div className="page-container" style={{ height: "calc(100vh - 112px)" }}>
      <Card style={{ height: "100%" }} styles={{ body: { height: "100%" } }}>
        <Space className="enterprise-products-space" direction="vertical" size="middle" style={{ width: "100%", height: "100%" }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <div>
              <Title level={3} style={{ margin: 0 }}>
                Enterprise Products
              </Title>
              <Text type="secondary">
                Manage products under enterprise: <strong>{enterprise.legalName}</strong>
              </Text>
            </div>
            <Button onClick={onBack}>Back to Enterprises</Button>
          </Space>
          <Layout style={{ background: "#fff", border: "1px solid #f0f0f0", borderRadius: 8, flex: 1, minHeight: 0, height: "100%" }}>
            <Layout.Sider width={220} theme="light" style={{ borderRight: "1px solid #f0f0f0", display: "flex", flexDirection: "column" }}>
              <Menu
                mode="inline"
                selectedKeys={[productsTabKey]}
                items={[{ key: "manage-products", label: "Manage Products", icon: <AppstoreOutlined /> }]}
                onClick={(info) => onProductsTabChange(String(info.key))}
                style={{ height: "100%", borderInlineEnd: "none" }}
              />
            </Layout.Sider>
            <Layout style={{ minHeight: 0, height: "100%" }}>
              <Layout.Header style={{ background: "#fff", padding: "0 16px", flex: "0 0 auto" }}>
                <Tabs
                  activeKey={productManagementTabKey}
                  onChange={onProductManagementTabChange}
                  tabBarStyle={{ marginBottom: 0 }}
                  items={[
                    {
                      key: "services",
                      label: (
                        <span style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
                          <ToolOutlined />
                          Services
                        </span>
                      )
                    },
                    {
                      key: "goods",
                      label: (
                        <span style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
                          <GiftOutlined />
                          Goods
                        </span>
                      )
                    }
                  ]}
                />
              </Layout.Header>
              <Layout.Content style={{ padding: 16, flex: 1, minHeight: 0 }}>
                <div
                  style={{
                    height: "100%",
                    width: "100%",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    border: "1px dashed #f0f0f0",
                    borderRadius: 8
                  }}
                >
                  {productManagementTabKey === "services" ? (
                    <Empty description="Service products management UI for this enterprise will be added here." />
                  ) : (
                    <Empty description="Goods products management UI for this enterprise will be added here." />
                  )}
                </div>
              </Layout.Content>
            </Layout>
          </Layout>
        </Space>
      </Card>
    </div>
  );
}
