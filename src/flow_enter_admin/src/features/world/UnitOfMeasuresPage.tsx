import { ClockCircleOutlined, DollarOutlined } from "@ant-design/icons";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert, Button, Card, Empty, Form, Input, Space, Spin, Tabs, Typography, message } from "antd";
import { useState } from "react";
import {
  createCurrencyMeasure,
  createTimeFrequencyMeasure,
  fetchCurrencyMeasures,
  fetchTimeFrequencyMeasures,
  updateCurrencyMeasure,
  updateTimeFrequencyMeasure
} from "../../api/unitOfMeasures";
import { TopDrawerForm } from "../../components/TopDrawerForm";
import type {
  CreateCurrencyMeasureRequest,
  CreateTimeFrequencyMeasureRequest,
  CurrencyMeasure,
  TimeFrequencyMeasure
} from "./types";

const { Title, Text } = Typography;

const UOM_TAB_STORAGE_KEY = "flow-enter-admin:world-unit-of-measure-tab";

type UnitOfMeasuresPageProps = {
  apiBaseUrl?: string;
};

export function UnitOfMeasuresPage({ apiBaseUrl }: UnitOfMeasuresPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [activeTab, setActiveTab] = useState<string>(() => {
    if (typeof window === "undefined") {
      return "currency-measure";
    }

    return window.localStorage.getItem(UOM_TAB_STORAGE_KEY) ?? "currency-measure";
  });
  const [createCurrencyMeasureOpen, setCreateCurrencyMeasureOpen] = useState(false);
  const [editingCurrencyMeasure, setEditingCurrencyMeasure] = useState<CurrencyMeasure | null>(null);
  const [createCurrencyMeasureForm] = Form.useForm<CreateCurrencyMeasureRequest>();
  const [editCurrencyMeasureForm] = Form.useForm<CreateCurrencyMeasureRequest>();
  const [createTimeFrequencyMeasureOpen, setCreateTimeFrequencyMeasureOpen] = useState(false);
  const [editingTimeFrequencyMeasure, setEditingTimeFrequencyMeasure] = useState<TimeFrequencyMeasure | null>(null);
  const [createTimeFrequencyMeasureForm] = Form.useForm<CreateTimeFrequencyMeasureRequest>();
  const [editTimeFrequencyMeasureForm] = Form.useForm<CreateTimeFrequencyMeasureRequest>();

  const currencyMeasuresQuery = useQuery({
    queryKey: ["world-currency-measures", apiBaseUrl],
    queryFn: () => fetchCurrencyMeasures(apiBaseUrl),
    enabled: activeTab === "currency-measure"
  });
  const timeFrequencyMeasuresQuery = useQuery({
    queryKey: ["world-time-frequency-measures", apiBaseUrl],
    queryFn: () => fetchTimeFrequencyMeasures(apiBaseUrl),
    enabled: activeTab === "time-frequency-measure"
  });

  const createCurrencyMeasureMutation = useMutation({
    mutationFn: async (values: CreateCurrencyMeasureRequest) => {
      await createCurrencyMeasure(values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["world-currency-measures", apiBaseUrl] });
      setCreateCurrencyMeasureOpen(false);
      createCurrencyMeasureForm.resetFields();
      messageApi.success("CurrencyMeasure created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create CurrencyMeasure failed");
    }
  });

  const updateCurrencyMeasureMutation = useMutation({
    mutationFn: async (values: CreateCurrencyMeasureRequest) => {
      if (!editingCurrencyMeasure) {
        throw new Error("No CurrencyMeasure selected");
      }

      await updateCurrencyMeasure(editingCurrencyMeasure.currencyMeasureId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["world-currency-measures", apiBaseUrl] });
      setEditingCurrencyMeasure(null);
      editCurrencyMeasureForm.resetFields();
      messageApi.success("CurrencyMeasure updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update CurrencyMeasure failed");
    }
  });
  const createTimeFrequencyMeasureMutation = useMutation({
    mutationFn: async (values: CreateTimeFrequencyMeasureRequest) => {
      await createTimeFrequencyMeasure(values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["world-time-frequency-measures", apiBaseUrl] });
      setCreateTimeFrequencyMeasureOpen(false);
      createTimeFrequencyMeasureForm.resetFields();
      messageApi.success("TimeFrequencyMeasure created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create TimeFrequencyMeasure failed");
    }
  });

  const updateTimeFrequencyMeasureMutation = useMutation({
    mutationFn: async (values: CreateTimeFrequencyMeasureRequest) => {
      if (!editingTimeFrequencyMeasure) {
        throw new Error("No TimeFrequencyMeasure selected");
      }

      await updateTimeFrequencyMeasure(editingTimeFrequencyMeasure.timeFrequencyMeasureId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["world-time-frequency-measures", apiBaseUrl] });
      setEditingTimeFrequencyMeasure(null);
      editTimeFrequencyMeasureForm.resetFields();
      messageApi.success("TimeFrequencyMeasure updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update TimeFrequencyMeasure failed");
    }
  });

  const handleTabChange = (key: string) => {
    setActiveTab(key);
    if (typeof window !== "undefined") {
      window.localStorage.setItem(UOM_TAB_STORAGE_KEY, key);
    }
  };

  return (
    <Card>
      {contextHolder}
      <Space direction="vertical" size="middle" style={{ width: "100%" }}>
        <div>
          <Title level={3} style={{ margin: 0 }}>
            Unit Of Measure
          </Title>
          <Text type="secondary">Manage CurrencyMeasure and TimeFrequencyMeasure.</Text>
        </div>

        <Tabs
          activeKey={activeTab}
          onChange={handleTabChange}
          items={[
            {
              key: "currency-measure",
              label: (
                <span style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
                  <DollarOutlined />
                  CurrencyMeasure
                </span>
              ),
              children: (
                <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                  <Space style={{ width: "100%", justifyContent: "flex-end" }}>
                    <Button type="primary" onClick={() => setCreateCurrencyMeasureOpen(true)}>
                      Add CurrencyMeasure
                    </Button>
                  </Space>
                  {currencyMeasuresQuery.isError ? (
                    <Alert
                      type="error"
                      message="Failed to load CurrencyMeasure"
                      description={currencyMeasuresQuery.error instanceof Error ? currencyMeasuresQuery.error.message : "Unknown error"}
                      showIcon
                    />
                  ) : currencyMeasuresQuery.isLoading ? (
                    <div className="table-loading">
                      <Spin />
                    </div>
                  ) : (currencyMeasuresQuery.data ?? []).length === 0 ? (
                    <Empty description="No CurrencyMeasure found." />
                  ) : (
                    <div className="tanstack-table-wrapper">
                      <table className="tanstack-table">
                        <thead>
                          <tr>
                            <th>Abbreviation</th>
                            <th>Description</th>
                            <th>Actions</th>
                          </tr>
                        </thead>
                        <tbody>
                          {(currencyMeasuresQuery.data ?? []).map((item) => (
                            <tr key={item.currencyMeasureId}>
                              <td>{item.abbreviation}</td>
                              <td>{item.description || "-"}</td>
                              <td>
                                <Button
                                  size="small"
                                  onClick={() => {
                                    setEditingCurrencyMeasure(item);
                                    editCurrencyMeasureForm.setFieldsValue({
                                      abbreviation: item.abbreviation,
                                      description: item.description
                                    });
                                  }}
                                >
                                  Edit
                                </Button>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </Space>
              )
            },
            {
              key: "time-frequency-measure",
              label: (
                <span style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
                  <ClockCircleOutlined />
                  TimeFrequencyMeasure
                </span>
              ),
              children: (
                <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                  <Space style={{ width: "100%", justifyContent: "flex-end" }}>
                    <Button type="primary" onClick={() => setCreateTimeFrequencyMeasureOpen(true)}>
                      Add TimeFrequencyMeasure
                    </Button>
                  </Space>
                  {timeFrequencyMeasuresQuery.isError ? (
                    <Alert
                      type="error"
                      message="Failed to load TimeFrequencyMeasure"
                      description={timeFrequencyMeasuresQuery.error instanceof Error ? timeFrequencyMeasuresQuery.error.message : "Unknown error"}
                      showIcon
                    />
                  ) : timeFrequencyMeasuresQuery.isLoading ? (
                    <div className="table-loading">
                      <Spin />
                    </div>
                  ) : (timeFrequencyMeasuresQuery.data ?? []).length === 0 ? (
                    <Empty description="No TimeFrequencyMeasure found." />
                  ) : (
                    <div className="tanstack-table-wrapper">
                      <table className="tanstack-table">
                        <thead>
                          <tr>
                            <th>Abbreviation</th>
                            <th>Description</th>
                            <th>Actions</th>
                          </tr>
                        </thead>
                        <tbody>
                          {(timeFrequencyMeasuresQuery.data ?? []).map((item) => (
                            <tr key={item.timeFrequencyMeasureId}>
                              <td>{item.abbreviation}</td>
                              <td>{item.description || "-"}</td>
                              <td>
                                <Button
                                  size="small"
                                  onClick={() => {
                                    setEditingTimeFrequencyMeasure(item);
                                    editTimeFrequencyMeasureForm.setFieldsValue({
                                      abbreviation: item.abbreviation,
                                      description: item.description
                                    });
                                  }}
                                >
                                  Edit
                                </Button>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </Space>
              )
            }
          ]}
        />
      </Space>

      <TopDrawerForm
        open={createCurrencyMeasureOpen}
        title="Create CurrencyMeasure"
        submitText="Create"
        onClose={() => {
          setCreateCurrencyMeasureOpen(false);
          createCurrencyMeasureForm.resetFields();
        }}
        onSubmit={() => createCurrencyMeasureForm.submit()}
        loading={createCurrencyMeasureMutation.isPending}
      >
        <Form<CreateCurrencyMeasureRequest>
          form={createCurrencyMeasureForm}
          layout="vertical"
          onFinish={(values) => createCurrencyMeasureMutation.mutate(values)}
        >
          <Form.Item name="abbreviation" label="Abbreviation" rules={[{ required: true, message: "Abbreviation is required" }]}>
            <Input maxLength={10} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={300} />
          </Form.Item>
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingCurrencyMeasure)}
        title="Edit CurrencyMeasure"
        submitText="Update"
        onClose={() => {
          setEditingCurrencyMeasure(null);
          editCurrencyMeasureForm.resetFields();
        }}
        onSubmit={() => editCurrencyMeasureForm.submit()}
        loading={updateCurrencyMeasureMutation.isPending}
      >
        <Form<CreateCurrencyMeasureRequest>
          form={editCurrencyMeasureForm}
          layout="vertical"
          onFinish={(values) => updateCurrencyMeasureMutation.mutate(values)}
        >
          <Form.Item name="abbreviation" label="Abbreviation" rules={[{ required: true, message: "Abbreviation is required" }]}>
            <Input maxLength={10} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={300} />
          </Form.Item>
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={createTimeFrequencyMeasureOpen}
        title="Create TimeFrequencyMeasure"
        submitText="Create"
        onClose={() => {
          setCreateTimeFrequencyMeasureOpen(false);
          createTimeFrequencyMeasureForm.resetFields();
        }}
        onSubmit={() => createTimeFrequencyMeasureForm.submit()}
        loading={createTimeFrequencyMeasureMutation.isPending}
      >
        <Form<CreateTimeFrequencyMeasureRequest>
          form={createTimeFrequencyMeasureForm}
          layout="vertical"
          onFinish={(values) => createTimeFrequencyMeasureMutation.mutate(values)}
        >
          <Form.Item name="abbreviation" label="Abbreviation" rules={[{ required: true, message: "Abbreviation is required" }]}>
            <Input maxLength={10} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={300} />
          </Form.Item>
        </Form>
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingTimeFrequencyMeasure)}
        title="Edit TimeFrequencyMeasure"
        submitText="Update"
        onClose={() => {
          setEditingTimeFrequencyMeasure(null);
          editTimeFrequencyMeasureForm.resetFields();
        }}
        onSubmit={() => editTimeFrequencyMeasureForm.submit()}
        loading={updateTimeFrequencyMeasureMutation.isPending}
      >
        <Form<CreateTimeFrequencyMeasureRequest>
          form={editTimeFrequencyMeasureForm}
          layout="vertical"
          onFinish={(values) => updateTimeFrequencyMeasureMutation.mutate(values)}
        >
          <Form.Item name="abbreviation" label="Abbreviation" rules={[{ required: true, message: "Abbreviation is required" }]}>
            <Input maxLength={10} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input.TextArea rows={4} maxLength={300} />
          </Form.Item>
        </Form>
      </TopDrawerForm>
    </Card>
  );
}
