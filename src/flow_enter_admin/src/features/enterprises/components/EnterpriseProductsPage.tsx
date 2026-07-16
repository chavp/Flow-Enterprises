import { AppstoreOutlined, GiftOutlined, ToolOutlined } from "@ant-design/icons";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert, Button, Card, Empty, Form, Input, Layout, Menu, Popconfirm, Space, Spin, Tabs, Typography, message } from "antd";
import { useState } from "react";
import {
  createEnterpriseService,
  deleteEnterpriseService,
  fetchEnterpriseServices,
  updateEnterpriseService
} from "../../../api/enterprises";
import { TopDrawerForm } from "../../../components/TopDrawerForm";
import type { CreateEnterpriseServiceRequest, Enterprise, EnterpriseService } from "../types";

const { Title, Text } = Typography;

type EnterpriseProductsPageProps = {
  enterprise: Enterprise;
  apiBaseUrl?: string;
  productsTabKey: string;
  onProductsTabChange: (key: string) => void;
  productManagementTabKey: string;
  onProductManagementTabChange: (key: string) => void;
  onBack: () => void;
};

export function EnterpriseProductsPage({
  enterprise,
  apiBaseUrl,
  productsTabKey,
  onProductsTabChange,
  productManagementTabKey,
  onProductManagementTabChange,
  onBack
}: EnterpriseProductsPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [serviceCreateOpen, setServiceCreateOpen] = useState(false);
  const [editingService, setEditingService] = useState<EnterpriseService | null>(null);
  const [createServiceForm] = Form.useForm<CreateEnterpriseServiceRequest>();
  const [editServiceForm] = Form.useForm<CreateEnterpriseServiceRequest>();

  const servicesQuery = useQuery({
    queryKey: ["enterprise-services", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseServices(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "services"
  });

  const createServiceMutation = useMutation({
    mutationFn: async (values: CreateEnterpriseServiceRequest) => {
      await createEnterpriseService(enterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-services", enterprise.enterpriseId, apiBaseUrl]
      });
      setServiceCreateOpen(false);
      createServiceForm.resetFields();
      messageApi.success("Service created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create service failed");
    }
  });

  const updateServiceMutation = useMutation({
    mutationFn: async (values: CreateEnterpriseServiceRequest) => {
      if (!editingService) {
        throw new Error("No service selected");
      }

      await updateEnterpriseService(enterprise.enterpriseId, editingService.serviceId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-services", enterprise.enterpriseId, apiBaseUrl]
      });
      setEditingService(null);
      editServiceForm.resetFields();
      messageApi.success("Service updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update service failed");
    }
  });

  const deleteServiceMutation = useMutation({
    mutationFn: async (serviceId: string) => {
      await deleteEnterpriseService(enterprise.enterpriseId, serviceId, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-services", enterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Service deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete service failed");
    }
  });

  const services = servicesQuery.data ?? [];

  return (
    <div className="page-container" style={{ height: "calc(100vh - 112px)" }}>
      {contextHolder}
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
                    <Space direction="vertical" size="middle" style={{ width: "100%", height: "100%", padding: 12 }}>
                      <Space style={{ width: "100%", justifyContent: "flex-end" }}>
                        <Button type="primary" onClick={() => setServiceCreateOpen(true)}>
                          Add Service
                        </Button>
                      </Space>

                      {servicesQuery.isError ? (
                        <Alert
                          type="error"
                          message="Failed to load services"
                          description={servicesQuery.error instanceof Error ? servicesQuery.error.message : "Unknown error"}
                          showIcon
                        />
                      ) : servicesQuery.isLoading ? (
                        <div className="table-loading">
                          <Spin />
                        </div>
                      ) : services.length === 0 ? (
                        <Empty description="No services found for this enterprise." />
                      ) : (
                        <div className="tanstack-table-wrapper">
                          <table className="tanstack-table">
                            <thead>
                              <tr>
                                <th>Service Name</th>
                                <th>Description</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              {services.map((service) => (
                                <tr key={service.serviceId}>
                                  <td>{service.name}</td>
                                  <td>{service.description || "-"}</td>
                                  <td>
                                    <Space>
                                      <Button
                                        size="small"
                                        onClick={() => {
                                          setEditingService(service);
                                          editServiceForm.setFieldsValue({
                                            name: service.name,
                                            description: service.description
                                          });
                                        }}
                                      >
                                        Edit
                                      </Button>
                                      <Popconfirm
                                        title="Delete service?"
                                        description="This action cannot be undone."
                                        okText="Delete"
                                        okButtonProps={{ danger: true, loading: deleteServiceMutation.isPending }}
                                        onConfirm={() => deleteServiceMutation.mutate(service.serviceId)}
                                      >
                                        <Button size="small" danger>
                                          Delete
                                        </Button>
                                      </Popconfirm>
                                    </Space>
                                  </td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        </div>
                      )}
                    </Space>
                  ) : (
                    <Empty description="Goods products management UI for this enterprise will be added here." />
                  )}
                </div>
              </Layout.Content>
            </Layout>
          </Layout>
        </Space>
      </Card>

      <TopDrawerForm
        open={serviceCreateOpen}
        title="Create Service"
        submitText="Create"
        onClose={() => {
          setServiceCreateOpen(false);
          createServiceForm.resetFields();
        }}
        onSubmit={() => createServiceForm.submit()}
        loading={createServiceMutation.isPending}
      >
        <Form<CreateEnterpriseServiceRequest>
          form={createServiceForm}
          layout="vertical"
          onFinish={(values) => createServiceMutation.mutate(values)}
        >
          <Form.Item name="name" label="Service Name" rules={[{ required: true, message: "Service name is required" }]}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={500} />
          </Form.Item>
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingService)}
        title="Edit Service"
        submitText="Update"
        onClose={() => {
          setEditingService(null);
          editServiceForm.resetFields();
        }}
        onSubmit={() => editServiceForm.submit()}
        loading={updateServiceMutation.isPending}
      >
        <Form<CreateEnterpriseServiceRequest>
          form={editServiceForm}
          layout="vertical"
          onFinish={(values) => updateServiceMutation.mutate(values)}
        >
          <Form.Item name="name" label="Service Name" rules={[{ required: true, message: "Service name is required" }]}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={500} />
          </Form.Item>
        </Form>
      </TopDrawerForm>
    </div>
  );
}
