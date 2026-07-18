import { AppstoreOutlined, GiftOutlined, TagsOutlined, ToolOutlined } from "@ant-design/icons";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert, Button, Card, Empty, Form, Input, InputNumber, Layout, Menu, Modal, Popconfirm, Select, Space, Spin, Tabs, Typography, message } from "antd";
import { useEffect, useMemo, useState } from "react";
import {
  createEnterpriseProductFeature,
  createEnterpriseProductFeatureCategory,
  createEnterpriseService,
  deleteEnterpriseProductFeatureCategory,
  deleteEnterpriseProductFeature,
  deleteEnterpriseService,
  fetchEnterpriseGoods,
  fetchEnterpriseProductFeatureApplicabilityTypes,
  fetchEnterpriseProductFeatureApplicabilities,
  fetchEnterpriseProductFeatureCategories,
  fetchEnterpriseProductFeatureTypes,
  fetchEnterpriseProductFeatures,
  fetchEnterpriseBranchs,
  fetchEnterpriseServiceFeatureApplicabilities,
  fetchEnterpriseServicePriceCoponents,
  fetchEnterpriseServices,
  updateEnterpriseProductFeatureCategory,
  updateEnterpriseProductFeature,
  updateEnterpriseService
} from "../../../api/enterprises";
import { fetchCurrencyMeasures, fetchTimeFrequencyMeasures } from "../../../api/unitOfMeasures";
import { TopDrawerForm } from "../../../components/TopDrawerForm";
import type {
  CreateEnterpriseProductFeatureRequest,
  CreateEnterpriseServiceRequest,
  CreateProductFeatureCategoryRequest,
  Enterprise,
  EnterpriseGood,
  ProductFeatureCategory,
  EnterpriseProductFeature,
  EnterpriseService,
  EnterpriseServicePriceCoponentRequest
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
  const [selectedProductForFeatures, setSelectedProductForFeatures] = useState<{ productId: string; productName: string } | null>(null);
  const [createServiceCoverImageBase64, setCreateServiceCoverImageBase64] = useState<string | undefined>(undefined);
  const [createServiceCoverImageName, setCreateServiceCoverImageName] = useState<string | undefined>(undefined);
  const [editServiceCoverImageBase64, setEditServiceCoverImageBase64] = useState<string | undefined>(undefined);
  const [editServiceCoverImageName, setEditServiceCoverImageName] = useState<string | undefined>(undefined);
  const [editServiceCoverImageRemoved, setEditServiceCoverImageRemoved] = useState(false);

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
    enabled:
      productsTabKey === "manage-products" &&
      (productManagementTabKey === "features" || productManagementTabKey === "services" || productManagementTabKey === "goods")
  });

  const featureTypesQuery = useQuery({
    queryKey: ["enterprise-product-feature-types", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseProductFeatureTypes(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "features"
  });

  const featuresQuery = useQuery({
    queryKey: ["enterprise-product-features", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseProductFeatures(enterprise.enterpriseId, apiBaseUrl),
    enabled:
      productsTabKey === "manage-products" &&
      (productManagementTabKey === "features" || productManagementTabKey === "services" || productManagementTabKey === "goods")
  });

  const goodsQuery = useQuery({
    queryKey: ["enterprise-goods", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseGoods(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "goods"
  });

  const featureApplicabilityTypesQuery = useQuery({
    queryKey: ["enterprise-product-feature-applicability-types", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseProductFeatureApplicabilityTypes(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "services"
  });

  const serviceFeatureApplicabilitiesQuery = useQuery({
    queryKey: ["enterprise-service-feature-applicabilities", enterprise.enterpriseId, editingService?.serviceId, apiBaseUrl],
    queryFn: () =>
      fetchEnterpriseServiceFeatureApplicabilities(enterprise.enterpriseId, editingService!.serviceId, apiBaseUrl),
    enabled: Boolean(editingService)
  });
  const servicePriceCoponentsQuery = useQuery({
    queryKey: ["enterprise-service-price-coponents", enterprise.enterpriseId, editingService?.serviceId, apiBaseUrl],
    queryFn: () => fetchEnterpriseServicePriceCoponents(enterprise.enterpriseId, editingService!.serviceId, apiBaseUrl),
    enabled: Boolean(editingService)
  });
  const enterpriseBranchsQuery = useQuery({
    queryKey: ["enterprise-branchs", enterprise.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseBranchs(enterprise.enterpriseId, apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "services"
  });
  const currencyMeasuresQuery = useQuery({
    queryKey: ["currency-measures", apiBaseUrl],
    queryFn: () => fetchCurrencyMeasures(apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "services"
  });
  const timeFrequencyMeasuresQuery = useQuery({
    queryKey: ["time-frequency-measures", apiBaseUrl],
    queryFn: () => fetchTimeFrequencyMeasures(apiBaseUrl),
    enabled: productsTabKey === "manage-products" && productManagementTabKey === "services"
  });

  const productFeatureApplicabilitiesQuery = useQuery({
    queryKey: ["enterprise-product-feature-applicabilities", enterprise.enterpriseId, selectedProductForFeatures?.productId, apiBaseUrl],
    queryFn: () =>
      fetchEnterpriseProductFeatureApplicabilities(enterprise.enterpriseId, selectedProductForFeatures!.productId, apiBaseUrl),
    enabled: Boolean(selectedProductForFeatures)
  });

  const createServiceMutation = useMutation({
    mutationFn: async (values: CreateEnterpriseServiceRequest) => {
      await createEnterpriseService(
        enterprise.enterpriseId,
        normalizeServicePayload({
          ...values,
          coverImage: createServiceCoverImageBase64,
          coverImageName: createServiceCoverImageName
        }),
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-services", enterprise.enterpriseId, apiBaseUrl]
      });
      setServiceCreateOpen(false);
      createServiceForm.resetFields();
      setCreateServiceCoverImageBase64(undefined);
      setCreateServiceCoverImageName(undefined);
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

      await updateEnterpriseService(
        enterprise.enterpriseId,
        editingService.serviceId,
        normalizeServicePayload({
          ...values,
          coverImage: editServiceCoverImageBase64,
          coverImageName: editServiceCoverImageName,
          removeCoverImage: editServiceCoverImageRemoved
        }),
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-services", enterprise.enterpriseId, apiBaseUrl]
      });
      setEditingService(null);
      editServiceForm.resetFields();
      setEditServiceCoverImageBase64(undefined);
      setEditServiceCoverImageName(undefined);
      setEditServiceCoverImageRemoved(false);
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
  const goods = goodsQuery.data ?? [];
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
  const productFeatureOptions = useMemo(
    () =>
      productFeatures.map((item) => ({
        value: item.productFeatureId,
        label: `${item.code} - ${item.title}`
      })),
    [productFeatures]
  );
  const productFeatureApplicabilityTypeOptions = useMemo(
    () =>
      (featureApplicabilityTypesQuery.data ?? []).map((item) => ({
        value: item,
        label: item
      })),
    [featureApplicabilityTypesQuery.data]
  );
  const unitOfMeasureOptions = useMemo(
    () =>
      (currencyMeasuresQuery.data ?? []).map((item) => ({
        value: item.currencyMeasureId,
        label: item.abbreviation
      })),
    [currencyMeasuresQuery.data]
  );
  const timeFrequencyMeasureOptions = useMemo(
    () =>
      (timeFrequencyMeasuresQuery.data ?? []).map((item) => ({
        value: item.timeFrequencyMeasureId,
        label: item.abbreviation
      })),
    [timeFrequencyMeasuresQuery.data]
  );
  const enterpriseBranchOptions = useMemo(
    () =>
      (enterpriseBranchsQuery.data ?? []).map((item) => ({
        value: item.branchId,
        label: item.branchLegalName
      })),
    [enterpriseBranchsQuery.data]
  );
  const priceCoponentTypeOptions = useMemo(
    () => [
      { value: "BasePrice", label: "BasePrice" },
      { value: "RecurringCharge", label: "RecurringCharge" }
    ],
    []
  );

  useEffect(() => {
    if (!editingService || !serviceFeatureApplicabilitiesQuery.data || !servicePriceCoponentsQuery.data) {
      return;
    }

    editServiceForm.setFieldsValue({
      name: editingService.name,
      description: editingService.description,
      releaseDate: editingService.releaseDate,
      discontinuedDate: editingService.discontinuedDate,
      supportDiscontinuedDate: editingService.supportDiscontinuedDate,
      productFeatureApplicabilities: serviceFeatureApplicabilitiesQuery.data.map((item) => ({
        productFeatureId: item.productFeatureId,
        productFeatureApplicabilityType: item.productFeatureApplicabilityType,
        order: item.order
      })),
      priceCoponents: servicePriceCoponentsQuery.data.map((item) => ({
        priceCoponentType: item.priceCoponentType,
        specifiedForPartyId: item.specifiedForPartyId,
        price: item.price,
        percent: item.percent,
        unitOfMeasureId: item.unitOfMeasureId,
        timeFrequencyMeasureId: item.timeFrequencyMeasureId,
        fromDate: item.fromDate,
        thruDate: item.thruDate,
        description: item.description
      }))
    });
  }, [editingService, serviceFeatureApplicabilitiesQuery.data, servicePriceCoponentsQuery.data, editServiceForm]);

  const openProductFeaturesModal = (product: EnterpriseService | EnterpriseGood) => {
    const productId = "serviceId" in product ? product.serviceId : product.goodId;
    setSelectedProductForFeatures({
      productId,
      productName: product.name
    });
  };

  const normalizeServicePayload = <T extends CreateEnterpriseServiceRequest>(values: T): T =>
    ({
      ...values,
      releaseDate: values.releaseDate || undefined,
      discontinuedDate: values.discontinuedDate || undefined,
      supportDiscontinuedDate: values.supportDiscontinuedDate || undefined,
      coverImage: values.coverImage || undefined,
      coverImageName: values.coverImageName || undefined,
      productFeatureApplicabilities: values.productFeatureApplicabilities ?? [],
      priceCoponents:
        (values.priceCoponents ?? []).map((item: EnterpriseServicePriceCoponentRequest) => ({
          ...item,
          specifiedForPartyId: item.specifiedForPartyId || undefined,
          unitOfMeasureId: item.unitOfMeasureId || undefined,
          timeFrequencyMeasureId: item.timeFrequencyMeasureId || undefined,
          fromDate: item.fromDate || undefined,
          thruDate: item.thruDate || undefined,
          description: item.description || undefined
        })) ?? []
    }) as T;

  const toBase64 = async (file: File): Promise<string> => {
    const dataUrl = await new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(String(reader.result ?? ""));
      reader.onerror = () => reject(new Error("Failed to read selected image"));
      reader.readAsDataURL(file);
    });

    const markerIndex = dataUrl.indexOf(",");
    return markerIndex >= 0 ? dataUrl.slice(markerIndex + 1) : dataUrl;
  };

  const handleServiceCoverImageChange = async (file: File, mode: "create" | "edit") => {
    if (!file.type.startsWith("image/")) {
      messageApi.error("Please select an image file.");
      return;
    }

    const base64 = await toBase64(file);
    if (mode === "create") {
      setCreateServiceCoverImageBase64(base64);
      setCreateServiceCoverImageName(file.name);
      return;
    }

    setEditServiceCoverImageBase64(base64);
    setEditServiceCoverImageName(file.name);
    setEditServiceCoverImageRemoved(false);
  };

  const renderServiceFeatureApplicabilities = () => (
    <Form.List name="productFeatureApplicabilities">
      {(fields, { add, remove }) => (
        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <Text strong>Product Feature Applicability</Text>
            <Button
              type="dashed"
              onClick={() =>
                add({
                  productFeatureId: undefined,
                  productFeatureApplicabilityType: undefined,
                  order: 0
                })
              }
            >
              Add Applicability
            </Button>
          </Space>
          {fields.length === 0 ? (
            <Empty description="No applicability rows." />
          ) : (
            <div className="tanstack-table-wrapper">
              <table className="tanstack-table">
                <thead>
                  <tr>
                    <th style={{ width: "45%" }}>Product Feature</th>
                    <th style={{ width: "35%" }}>Applicability Type</th>
                    <th style={{ width: "12%" }}>Order</th>
                    <th style={{ width: "8%" }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {fields.map((field) => (
                    <tr key={field.key}>
                      <td>
                        <Form.Item
                          name={[field.name, "productFeatureId"]}
                          rules={[{ required: true, message: "Product feature is required" }]}
                          style={{ marginBottom: 0 }}
                        >
                          <Select
                            options={productFeatureOptions}
                            loading={featuresQuery.isLoading}
                            placeholder="Select product feature"
                            optionFilterProp="label"
                            showSearch
                          />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item
                          name={[field.name, "productFeatureApplicabilityType"]}
                          rules={[{ required: true, message: "Applicability type is required" }]}
                          style={{ marginBottom: 0 }}
                        >
                          <Select
                            options={productFeatureApplicabilityTypeOptions}
                            loading={featureApplicabilityTypesQuery.isLoading}
                            placeholder="Select applicability type"
                            optionFilterProp="label"
                            showSearch
                          />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item
                          name={[field.name, "order"]}
                          rules={[{ required: true, message: "Order is required" }]}
                          style={{ marginBottom: 0 }}
                        >
                          <InputNumber min={0} style={{ width: "100%" }} />
                        </Form.Item>
                      </td>
                      <td>
                        <Button danger onClick={() => remove(field.name)}>
                          Remove
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </Space>
      )}
    </Form.List>
  );

  const renderServicePriceCoponents = () => (
    <Form.List name="priceCoponents">
      {(fields, { add, remove }) => (
        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <Text strong>PriceCoponent</Text>
            <Button
              type="dashed"
              onClick={() =>
                add({
                  priceCoponentType: "BasePrice",
                  specifiedForPartyId: undefined,
                  price: undefined,
                  percent: undefined,
                  unitOfMeasureId: undefined,
                  timeFrequencyMeasureId: undefined,
                  fromDate: undefined,
                  thruDate: undefined,
                  description: undefined
                })
              }
            >
              Add PriceCoponent
            </Button>
          </Space>
          {fields.length === 0 ? (
            <Empty description="No price coponents." />
          ) : (
            <div className="tanstack-table-wrapper">
              <table className="tanstack-table">
                <thead>
                  <tr>
                    <th>Type</th>
                    <th>Specified For Party</th>
                    <th>Price</th>
                    <th>Percent</th>
                    <th>Unit</th>
                    <th>Time Frequency</th>
                    <th>From</th>
                    <th>Thru</th>
                    <th>Description</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {fields.map((field) => (
                    <tr key={field.key}>
                      <td>
                        <Form.Item
                          name={[field.name, "priceCoponentType"]}
                          rules={[{ required: true, message: "Type is required" }]}
                          style={{ marginBottom: 0 }}
                        >
                          <Select options={priceCoponentTypeOptions} />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item name={[field.name, "specifiedForPartyId"]} style={{ marginBottom: 0 }}>
                          <Select
                            options={enterpriseBranchOptions}
                            loading={enterpriseBranchsQuery.isLoading}
                            placeholder="Branch"
                            optionFilterProp="label"
                            showSearch
                            allowClear
                          />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item name={[field.name, "price"]} style={{ marginBottom: 0 }}>
                          <InputNumber style={{ width: "100%" }} min={0} />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item name={[field.name, "percent"]} style={{ marginBottom: 0 }}>
                          <InputNumber style={{ width: "100%" }} min={0} max={100} />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item
                          name={[field.name, "unitOfMeasureId"]}
                          style={{ marginBottom: 0 }}
                        >
                          <Select
                            options={unitOfMeasureOptions}
                            loading={currencyMeasuresQuery.isLoading}
                            placeholder="Unit"
                            optionFilterProp="label"
                            showSearch
                            allowClear
                          />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item
                          noStyle
                          shouldUpdate={(prevValues, currentValues) =>
                            prevValues.priceCoponents?.[field.name]?.priceCoponentType !==
                            currentValues.priceCoponents?.[field.name]?.priceCoponentType
                          }
                        >
                          {({ getFieldValue }) => {
                            const type = getFieldValue(["priceCoponents", field.name, "priceCoponentType"]);
                            const isRecurring = type === "RecurringCharge";
                            return (
                              <Form.Item
                                name={[field.name, "timeFrequencyMeasureId"]}
                                rules={isRecurring ? [{ required: true, message: "Time frequency is required" }] : []}
                                style={{ marginBottom: 0 }}
                              >
                                <Select
                                  options={timeFrequencyMeasureOptions}
                                  loading={timeFrequencyMeasuresQuery.isLoading}
                                  placeholder="Time Frequency"
                                  optionFilterProp="label"
                                  showSearch
                                  disabled={!isRecurring}
                                />
                              </Form.Item>
                            );
                          }}
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item name={[field.name, "fromDate"]} style={{ marginBottom: 0 }}>
                          <Input type="date" />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item name={[field.name, "thruDate"]} style={{ marginBottom: 0 }}>
                          <Input type="date" />
                        </Form.Item>
                      </td>
                      <td>
                        <Form.Item name={[field.name, "description"]} style={{ marginBottom: 0 }}>
                          <Input maxLength={300} />
                        </Form.Item>
                      </td>
                      <td>
                        <Button danger onClick={() => remove(field.name)}>
                          Remove
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </Space>
      )}
    </Form.List>
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
                        <Button
                          type="primary"
                          onClick={() => {
                            setCreateServiceCoverImageBase64(undefined);
                            setCreateServiceCoverImageName(undefined);
                            setServiceCreateOpen(true);
                          }}
                        >
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
                                <th>Price</th>
                                <th>Features</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              {services.map((service) => (
                                <tr key={service.serviceId}>
                                  <td>{service.name}</td>
                                  <td>{service.description || "-"}</td>
                                  <td>{service.priceDisplay ?? (service.price != null ? service.price.toLocaleString() : "-")}</td>
                                  <td>
                                    <Button type="link" onClick={() => openProductFeaturesModal(service)}>
                                      {service.featureCount}
                                    </Button>
                                  </td>
                                  <td>
                                    <Space>
                                      <Button
                                        size="small"
                                        onClick={() => {
                                          setEditingService(service);
                                          setEditServiceCoverImageBase64(undefined);
                                          setEditServiceCoverImageName(
                                            service.coverImageName ?? (service.hasCoverImage ? "Current cover image" : undefined)
                                          );
                                          setEditServiceCoverImageRemoved(false);
                                          editServiceForm.setFieldsValue({
                                            name: service.name,
                                            description: service.description,
                                            releaseDate: service.releaseDate,
                                            discontinuedDate: service.discontinuedDate,
                                            supportDiscontinuedDate: service.supportDiscontinuedDate,
                                            productFeatureApplicabilities: [],
                                            priceCoponents: []
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
                  ) : productManagementTabKey === "goods" ? (
                    <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                      {goodsQuery.isError ? (
                        <Alert
                          type="error"
                          message="Failed to load goods"
                          description={goodsQuery.error instanceof Error ? goodsQuery.error.message : "Unknown error"}
                          showIcon
                        />
                      ) : goodsQuery.isLoading ? (
                        <div className="table-loading">
                          <Spin />
                        </div>
                      ) : goods.length === 0 ? (
                        <Empty description="No goods found for this enterprise." />
                      ) : (
                        <div className="tanstack-table-wrapper">
                          <table className="tanstack-table">
                            <thead>
                              <tr>
                                <th>Good Name</th>
                                <th>Description</th>
                                <th>Features</th>
                              </tr>
                            </thead>
                            <tbody>
                              {goods.map((good) => (
                                <tr key={good.goodId}>
                                  <td>{good.name}</td>
                                  <td>{good.description || "-"}</td>
                                  <td>
                                    <Button type="link" onClick={() => openProductFeaturesModal(good)}>
                                      {good.featureCount}
                                    </Button>
                                  </td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        </div>
                      )}
                    </Space>
                  ) : (
                    <Empty description="Unsupported tab." />
                  )}
                </div>
              </Layout.Content>
            </Layout>
          </Layout>
        </Space>
      </Card>

      <Modal
        open={Boolean(selectedProductForFeatures)}
        title={`Product Features - ${selectedProductForFeatures?.productName ?? ""}`}
        width={900}
        onCancel={() => setSelectedProductForFeatures(null)}
        footer={null}
        destroyOnHidden
      >
        {productFeatureApplicabilitiesQuery.isError ? (
          <Alert
            type="error"
            message="Failed to load product features"
            description={productFeatureApplicabilitiesQuery.error instanceof Error ? productFeatureApplicabilitiesQuery.error.message : "Unknown error"}
            showIcon
          />
        ) : productFeatureApplicabilitiesQuery.isLoading ? (
          <div className="table-loading">
            <Spin />
          </div>
        ) : (productFeatureApplicabilitiesQuery.data ?? []).length === 0 ? (
          <Empty description="No features configured for this product." />
        ) : (
          <div className="tanstack-table-wrapper">
            <table className="tanstack-table">
              <thead>
                <tr>
                  <th>Code</th>
                  <th>Title</th>
                  <th>Applicability Type</th>
                  <th>Order</th>
                </tr>
              </thead>
              <tbody>
                {(productFeatureApplicabilitiesQuery.data ?? []).map((item) => (
                  <tr key={item.productFeatureApplicabilityId}>
                    <td>{item.productFeatureCode}</td>
                    <td>{item.productFeatureTitle}</td>
                    <td>{item.productFeatureApplicabilityType}</td>
                    <td>{item.order}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Modal>

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
          setCreateServiceCoverImageBase64(undefined);
          setCreateServiceCoverImageName(undefined);
        }}
        onSubmit={() => createServiceForm.submit()}
        loading={createServiceMutation.isPending}
      >
        <Form<CreateEnterpriseServiceRequest>
          form={createServiceForm}
          layout="vertical"
          initialValues={{ productFeatureApplicabilities: [], priceCoponents: [] }}
          onFinish={(values) => createServiceMutation.mutate(values)}
        >
          <Form.Item name="name" label="Service Name" rules={[{ required: true, message: "Service name is required" }]}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={500} />
          </Form.Item>
          <Form.Item name="releaseDate" label="Release Date">
            <Input type="date" />
          </Form.Item>
          <Form.Item name="discontinuedDate" label="Discontinued Date">
            <Input type="date" />
          </Form.Item>
          <Form.Item name="supportDiscontinuedDate" label="Support Discontinued Date">
            <Input type="date" />
          </Form.Item>
          <Form.Item label="Product Cover Image">
            <Space direction="vertical" size="small" style={{ width: "100%" }}>
              <Input
                type="file"
                accept="image/*"
                onChange={async (event) => {
                  const file = event.currentTarget.files?.[0];
                  if (!file) {
                    return;
                  }

                  await handleServiceCoverImageChange(file, "create");
                }}
              />
              {createServiceCoverImageName ? (
                <Space>
                  <Text type="secondary">Selected: {createServiceCoverImageName}</Text>
                  <Button
                    size="small"
                    danger
                    onClick={() => {
                      setCreateServiceCoverImageBase64(undefined);
                      setCreateServiceCoverImageName(undefined);
                    }}
                  >
                    Remove
                  </Button>
                </Space>
              ) : null}
            </Space>
          </Form.Item>
          {renderServiceFeatureApplicabilities()}
          {renderServicePriceCoponents()}
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingService)}
        title="Edit Service"
        submitText="Update"
        onClose={() => {
          setEditingService(null);
          editServiceForm.resetFields();
          setEditServiceCoverImageBase64(undefined);
          setEditServiceCoverImageName(undefined);
          setEditServiceCoverImageRemoved(false);
        }}
        onSubmit={() => editServiceForm.submit()}
        loading={updateServiceMutation.isPending || serviceFeatureApplicabilitiesQuery.isLoading || servicePriceCoponentsQuery.isLoading}
      >
        <Form<CreateEnterpriseServiceRequest>
          form={editServiceForm}
          layout="vertical"
          initialValues={{ productFeatureApplicabilities: [], priceCoponents: [] }}
          onFinish={(values) => updateServiceMutation.mutate(values)}
        >
          <Form.Item name="name" label="Service Name" rules={[{ required: true, message: "Service name is required" }]}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={500} />
          </Form.Item>
          <Form.Item name="releaseDate" label="Release Date">
            <Input type="date" />
          </Form.Item>
          <Form.Item name="discontinuedDate" label="Discontinued Date">
            <Input type="date" />
          </Form.Item>
          <Form.Item name="supportDiscontinuedDate" label="Support Discontinued Date">
            <Input type="date" />
          </Form.Item>
          <Form.Item label="Product Cover Image">
            <Space direction="vertical" size="small" style={{ width: "100%" }}>
              <Input
                type="file"
                accept="image/*"
                onChange={async (event) => {
                  const file = event.currentTarget.files?.[0];
                  if (!file) {
                    return;
                  }

                  await handleServiceCoverImageChange(file, "edit");
                }}
              />
              {editServiceCoverImageName ? (
                <Space>
                  <Text type="secondary">Current/Selected: {editServiceCoverImageName}</Text>
                  <Button
                    size="small"
                    danger
                    onClick={() => {
                      setEditServiceCoverImageBase64(undefined);
                      setEditServiceCoverImageName(undefined);
                      setEditServiceCoverImageRemoved(true);
                    }}
                  >
                    Remove
                  </Button>
                </Space>
              ) : null}
            </Space>
          </Form.Item>
          {renderServiceFeatureApplicabilities()}
          {renderServicePriceCoponents()}
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
