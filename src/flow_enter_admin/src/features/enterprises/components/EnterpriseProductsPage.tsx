import { AppstoreOutlined, GiftOutlined, TagsOutlined, ToolOutlined } from "@ant-design/icons";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert, Button, Card, Empty, Form, Input, Layout, Menu, Modal, Popconfirm, Select, Space, Spin, Tabs, Typography, message } from "antd";
import { useMemo, useState } from "react";
import {
  createEnterpriseProductFeature,
  createEnterpriseProductFeatureCategory,
  createEnterpriseService,
  deleteEnterpriseProductFeatureCategory,
  deleteEnterpriseProductFeature,
  deleteEnterpriseService,
  fetchEnterpriseProductFeatureCategories,
  fetchEnterpriseProductFeatureTypes,
  fetchEnterpriseProductFeatures,
  fetchEnterpriseServices,
  updateEnterpriseProductFeatureCategory,
  updateEnterpriseProductFeature,
  updateEnterpriseService
} from "../../../api/enterprises";
import { TopDrawerForm } from "../../../components/TopDrawerForm";
import type {
  CreateEnterpriseProductFeatureRequest,
  CreateEnterpriseServiceRequest,
  CreateProductFeatureCategoryRequest,
  Enterprise,
  ProductFeatureCategory,
  EnterpriseProductFeature,
  EnterpriseService
} from "../types";

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
  const [featureCreateOpen, setFeatureCreateOpen] = useState(false);
  const [editingFeature, setEditingFeature] = useState<EnterpriseProductFeature | null>(null);
  const [featureCategoryManagerOpen, setFeatureCategoryManagerOpen] = useState(false);
  const [featureCategoryCreateOpen, setFeatureCategoryCreateOpen] = useState(false);
  const [editingFeatureCategory, setEditingFeatureCategory] = useState<ProductFeatureCategory | null>(null);

  const [createServiceForm] = Form.useForm<CreateEnterpriseServiceRequest>();
  const [editServiceForm] = Form.useForm<CreateEnterpriseServiceRequest>();
  const [createFeatureForm] = Form.useForm<CreateEnterpriseProductFeatureRequest>();
  const [editFeatureForm] = Form.useForm<CreateEnterpriseProductFeatureRequest>();
  const [createFeatureCategoryForm] = Form.useForm<CreateProductFeatureCategoryRequest>();
  const [editFeatureCategoryForm] = Form.useForm<CreateProductFeatureCategoryRequest>();

  const servicesQuery = useQuery({
    queryKey: ["enterprise-services", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseServices(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "services"
  });

  const featureCategoriesQuery = useQuery({
    queryKey: ["enterprise-product-feature-categories", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseProductFeatureCategories(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "features"
  });

  const featureTypesQuery = useQuery({
    queryKey: ["enterprise-product-feature-types", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseProductFeatureTypes(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "features"
  });

  const featuresQuery = useQuery({
    queryKey: ["enterprise-product-features", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseProductFeatures(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "features"
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

  const createFeatureMutation = useMutation({
    mutationFn: async (values: CreateEnterpriseProductFeatureRequest) => {
      await createEnterpriseProductFeature(enterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-product-features", enterprise.enterpriseId, apiBaseUrl]
      });
      setFeatureCreateOpen(false);
      createFeatureForm.resetFields();
      messageApi.success("Product feature created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create product feature failed");
    }
  });

  const updateFeatureMutation = useMutation({
    mutationFn: async (values: CreateEnterpriseProductFeatureRequest) => {
      if (!editingFeature) {
        throw new Error("No product feature selected");
      }

      await updateEnterpriseProductFeature(enterprise.enterpriseId, editingFeature.productFeatureId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-product-features", enterprise.enterpriseId, apiBaseUrl]
      });
      setEditingFeature(null);
      editFeatureForm.resetFields();
      messageApi.success("Product feature updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update product feature failed");
    }
  });

  const deleteFeatureMutation = useMutation({
    mutationFn: async (productFeatureId: string) => {
      await deleteEnterpriseProductFeature(enterprise.enterpriseId, productFeatureId, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-product-features", enterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Product feature deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete product feature failed");
    }
  });

  const createFeatureCategoryMutation = useMutation({
    mutationFn: async (values: CreateProductFeatureCategoryRequest) => {
      await createEnterpriseProductFeatureCategory(enterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-product-feature-categories", enterprise.enterpriseId, apiBaseUrl]
      });
      setFeatureCategoryCreateOpen(false);
      createFeatureCategoryForm.resetFields();
      messageApi.success("Product feature category created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create product feature category failed");
    }
  });

  const updateFeatureCategoryMutation = useMutation({
    mutationFn: async (values: CreateProductFeatureCategoryRequest) => {
      if (!editingFeatureCategory) {
        throw new Error("No product feature category selected");
      }

      await updateEnterpriseProductFeatureCategory(
        enterprise.enterpriseId,
        editingFeatureCategory.productFeatureCategoryId,
        values,
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-product-feature-categories", enterprise.enterpriseId, apiBaseUrl]
      });
      setEditingFeatureCategory(null);
      editFeatureCategoryForm.resetFields();
      messageApi.success("Product feature category updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update product feature category failed");
    }
  });

  const deleteFeatureCategoryMutation = useMutation({
    mutationFn: async (productFeatureCategoryId: string) => {
      await deleteEnterpriseProductFeatureCategory(enterprise.enterpriseId, productFeatureCategoryId, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-product-feature-categories", enterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Product feature category deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete product feature category failed");
    }
  });

  const services = servicesQuery.data ?? [];
  const productFeatures = featuresQuery.data ?? [];
  const productFeatureCategories = featureCategoriesQuery.data ?? [];
  const productFeatureCategoryOptions = useMemo(
    () =>
      productFeatureCategories.map((item) => ({
        value: item.productFeatureCategoryId,
        label: item.name
      })),
    [productFeatureCategories]
  );
  const productFeatureTypeOptions = useMemo(
    () =>
      (featureTypesQuery.data ?? []).map((item) => ({
        value: item,
        label: item
      })),
    [featureTypesQuery.data]
  );

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
                    },
                    {
                      key: "features",
                      label: (
                        <span style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
                          <TagsOutlined />
                          Features
                        </span>
                      )
                    }
                  ]}
                />
              </Layout.Header>
              <Layout.Content style={{ padding: 16, flex: 1, minHeight: 0 }}>
                <div style={{ height: "100%", width: "100%", border: "1px dashed #f0f0f0", borderRadius: 8, padding: 12, overflow: "auto" }}>
                  {productManagementTabKey === "services" ? (
                    <Space direction="vertical" size="middle" style={{ width: "100%" }}>
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
                  ) : productManagementTabKey === "features" ? (
                    <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                      <Space style={{ width: "100%", justifyContent: "flex-end" }}>
                        <Button onClick={() => setFeatureCategoryManagerOpen(true)}>Manage Feature Categories</Button>
                        <Button type="primary" onClick={() => setFeatureCreateOpen(true)}>
                          Add Product Feature
                        </Button>
                      </Space>

                      {featuresQuery.isError ? (
                        <Alert
                          type="error"
                          message="Failed to load product features"
                          description={featuresQuery.error instanceof Error ? featuresQuery.error.message : "Unknown error"}
                          showIcon
                        />
                      ) : featuresQuery.isLoading ? (
                        <div className="table-loading">
                          <Spin />
                        </div>
                      ) : productFeatures.length === 0 ? (
                        <Empty description="No product features found for this enterprise." />
                      ) : (
                        <div className="tanstack-table-wrapper">
                          <table className="tanstack-table">
                            <thead>
                              <tr>
                                <th>Code</th>
                                <th>Title</th>
                                <th>Type</th>
                                <th>Category</th>
                                <th>Description</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              {productFeatures.map((feature) => (
                                <tr key={feature.productFeatureId}>
                                  <td>{feature.code}</td>
                                  <td>{feature.title}</td>
                                  <td>{feature.productFeatureType}</td>
                                  <td>{feature.productFeatureCategoryName}</td>
                                  <td>{feature.description || "-"}</td>
                                  <td>
                                    <Space>
                                      <Button
                                        size="small"
                                        onClick={() => {
                                          setEditingFeature(feature);
                                          editFeatureForm.setFieldsValue({
                                            productFeatureType: feature.productFeatureType,
                                            productFeatureCategoryId: feature.productFeatureCategoryId,
                                            code: feature.code,
                                            title: feature.title,
                                            description: feature.description
                                          });
                                        }}
                                      >
                                        Edit
                                      </Button>
                                      <Popconfirm
                                        title="Delete product feature?"
                                        description="This action cannot be undone."
                                        okText="Delete"
                                        okButtonProps={{ danger: true, loading: deleteFeatureMutation.isPending }}
                                        onConfirm={() => deleteFeatureMutation.mutate(feature.productFeatureId)}
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

      <Modal
        open={featureCategoryManagerOpen}
        title="Manage Product Feature Categories"
        width={900}
        onCancel={() => setFeatureCategoryManagerOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
          <Space style={{ width: "100%", justifyContent: "flex-end" }}>
            <Button type="primary" onClick={() => setFeatureCategoryCreateOpen(true)}>
              Add Product Feature Category
            </Button>
          </Space>

          {featureCategoriesQuery.isError ? (
            <Alert
              type="error"
              message="Failed to load product feature categories"
              description={featureCategoriesQuery.error instanceof Error ? featureCategoriesQuery.error.message : "Unknown error"}
              showIcon
            />
          ) : featureCategoriesQuery.isLoading ? (
            <div className="table-loading">
              <Spin />
            </div>
          ) : productFeatureCategories.length === 0 ? (
            <Empty description="No product feature categories found." />
          ) : (
            <div className="tanstack-table-wrapper">
              <table className="tanstack-table">
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Scope</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {productFeatureCategories.map((category) => (
                    <tr key={category.productFeatureCategoryId}>
                      <td>{category.name}</td>
                      <td>{category.isGlobal ? "Global" : "Enterprise"}</td>
                      <td>
                        <Space>
                          <Button
                            size="small"
                            disabled={category.isGlobal}
                            onClick={() => {
                              setEditingFeatureCategory(category);
                              editFeatureCategoryForm.setFieldsValue({
                                name: category.name
                              });
                            }}
                          >
                            Edit
                          </Button>
                          <Popconfirm
                            title="Delete product feature category?"
                            description="This action cannot be undone."
                            okText="Delete"
                            disabled={category.isGlobal}
                            okButtonProps={{ danger: true, loading: deleteFeatureCategoryMutation.isPending }}
                            onConfirm={() => deleteFeatureCategoryMutation.mutate(category.productFeatureCategoryId)}
                          >
                            <Button size="small" danger disabled={category.isGlobal}>
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
      </Modal>

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

      <TopDrawerForm
        open={featureCategoryCreateOpen}
        title="Create Product Feature Category"
        submitText="Create"
        onClose={() => {
          setFeatureCategoryCreateOpen(false);
          createFeatureCategoryForm.resetFields();
        }}
        onSubmit={() => createFeatureCategoryForm.submit()}
        loading={createFeatureCategoryMutation.isPending}
      >
        <Form<CreateProductFeatureCategoryRequest>
          form={createFeatureCategoryForm}
          layout="vertical"
          onFinish={(values) => createFeatureCategoryMutation.mutate(values)}
        >
          <Form.Item
            name="name"
            label="Category Name"
            rules={[{ required: true, message: "Product feature category name is required" }]}
          >
            <Input maxLength={200} />
          </Form.Item>
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingFeatureCategory)}
        title="Edit Product Feature Category"
        submitText="Update"
        onClose={() => {
          setEditingFeatureCategory(null);
          editFeatureCategoryForm.resetFields();
        }}
        onSubmit={() => editFeatureCategoryForm.submit()}
        loading={updateFeatureCategoryMutation.isPending}
      >
        <Form<CreateProductFeatureCategoryRequest>
          form={editFeatureCategoryForm}
          layout="vertical"
          onFinish={(values) => updateFeatureCategoryMutation.mutate(values)}
        >
          <Form.Item
            name="name"
            label="Category Name"
            rules={[{ required: true, message: "Product feature category name is required" }]}
          >
            <Input maxLength={200} />
          </Form.Item>
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={featureCreateOpen}
        title="Create Product Feature"
        submitText="Create"
        onClose={() => {
          setFeatureCreateOpen(false);
          createFeatureForm.resetFields();
        }}
        onSubmit={() => createFeatureForm.submit()}
        loading={createFeatureMutation.isPending}
      >
        <Form<CreateEnterpriseProductFeatureRequest>
          form={createFeatureForm}
          layout="vertical"
          onFinish={(values) => createFeatureMutation.mutate(values)}
        >
          <Form.Item
            name="productFeatureType"
            label="Product Feature Type"
            rules={[{ required: true, message: "Product feature type is required" }]}
          >
            <Select
              options={productFeatureTypeOptions}
              loading={featureTypesQuery.isLoading}
              placeholder="Select product feature type"
              optionFilterProp="label"
              showSearch
            />
          </Form.Item>
          <Form.Item
            name="productFeatureCategoryId"
            label="Category"
            rules={[{ required: true, message: "Product feature category is required" }]}
          >
            <Select
              options={productFeatureCategoryOptions}
              loading={featureCategoriesQuery.isLoading}
              placeholder="Select category"
              optionFilterProp="label"
              showSearch
            />
          </Form.Item>
          <Form.Item name="code" label="Code" rules={[{ required: true, message: "Product feature code is required" }]}>
            <Input maxLength={100} />
          </Form.Item>
          <Form.Item name="title" label="Title" rules={[{ required: true, message: "Product feature title is required" }]}>
            <Input maxLength={300} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={500} />
          </Form.Item>
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingFeature)}
        title="Edit Product Feature"
        submitText="Update"
        onClose={() => {
          setEditingFeature(null);
          editFeatureForm.resetFields();
        }}
        onSubmit={() => editFeatureForm.submit()}
        loading={updateFeatureMutation.isPending}
      >
        <Form<CreateEnterpriseProductFeatureRequest>
          form={editFeatureForm}
          layout="vertical"
          onFinish={(values) => updateFeatureMutation.mutate(values)}
        >
          <Form.Item
            name="productFeatureType"
            label="Product Feature Type"
            rules={[{ required: true, message: "Product feature type is required" }]}
          >
            <Select
              options={productFeatureTypeOptions}
              loading={featureTypesQuery.isLoading}
              placeholder="Select product feature type"
              optionFilterProp="label"
              showSearch
            />
          </Form.Item>
          <Form.Item
            name="productFeatureCategoryId"
            label="Category"
            rules={[{ required: true, message: "Product feature category is required" }]}
          >
            <Select
              options={productFeatureCategoryOptions}
              loading={featureCategoriesQuery.isLoading}
              placeholder="Select category"
              optionFilterProp="label"
              showSearch
            />
          </Form.Item>
          <Form.Item name="code" label="Code" rules={[{ required: true, message: "Product feature code is required" }]}>
            <Input maxLength={100} />
          </Form.Item>
          <Form.Item name="title" label="Title" rules={[{ required: true, message: "Product feature title is required" }]}>
            <Input maxLength={300} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={500} />
          </Form.Item>
        </Form>
      </TopDrawerForm>
    </div>
  );
}
