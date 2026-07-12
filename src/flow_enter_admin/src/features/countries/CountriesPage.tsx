import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert, Button, Card, Form, Input, InputNumber, Popconfirm, Space, Spin, Tree, Typography, message } from "antd";
import type { TreeDataNode } from "antd";
import { useMemo, useState } from "react";
import {
  createCountry,
  createProvince,
  deleteProvince,
  fetchCountriesTree,
  updateCountry,
  updateProvince
} from "../../api/countries";
import { TopDrawerForm } from "../../components/TopDrawerForm";
import { Country, CreateCountryRequest, CreateProvinceRequest, Province } from "./types";

const { Title, Text } = Typography;

type CountriesPageProps = {
  apiBaseUrl?: string;
};

type CountryFormValues = CreateCountryRequest;
type ProvinceFormValues = CreateProvinceRequest;

export function CountriesPage({ apiBaseUrl }: CountriesPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [isCreateCountryOpen, setCreateCountryOpen] = useState(false);
  const [editingCountry, setEditingCountry] = useState<Country | null>(null);
  const [isCreateProvinceOpen, setCreateProvinceOpen] = useState(false);
  const [activeCountryIdForProvinceCreate, setActiveCountryIdForProvinceCreate] = useState<string | null>(null);
  const [editingProvince, setEditingProvince] = useState<Province | null>(null);
  const [countryForm] = Form.useForm<CountryFormValues>();
  const [editCountryForm] = Form.useForm<CountryFormValues>();
  const [provinceForm] = Form.useForm<ProvinceFormValues>();
  const [editProvinceForm] = Form.useForm<ProvinceFormValues>();

  const countriesTreeQuery = useQuery({
    queryKey: ["countries-tree", apiBaseUrl],
    queryFn: () => fetchCountriesTree(apiBaseUrl)
  });

  const createCountryMutation = useMutation({
    mutationFn: (values: CountryFormValues) => createCountry(values, apiBaseUrl),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setCreateCountryOpen(false);
      countryForm.resetFields();
      messageApi.success("Country created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create country failed");
    }
  });

  const updateCountryMutation = useMutation({
    mutationFn: async (values: CountryFormValues) => {
      if (!editingCountry) {
        return;
      }

      await updateCountry(
        {
          id: editingCountry.id,
          changes: values
        },
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setEditingCountry(null);
      editCountryForm.resetFields();
      messageApi.success("Country updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update country failed");
    }
  });

  const createProvinceMutation = useMutation({
    mutationFn: async (values: ProvinceFormValues) => {
      if (!activeCountryIdForProvinceCreate) {
        return;
      }

      await createProvince(activeCountryIdForProvinceCreate, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setCreateProvinceOpen(false);
      setActiveCountryIdForProvinceCreate(null);
      provinceForm.resetFields();
      messageApi.success("Province created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create province failed");
    }
  });

  const updateProvinceMutation = useMutation({
    mutationFn: async (values: ProvinceFormValues) => {
      if (!editingProvince) {
        return;
      }

      await updateProvince(
        {
          id: editingProvince.id,
          changes: values
        },
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setEditingProvince(null);
      editProvinceForm.resetFields();
      messageApi.success("Province updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update province failed");
    }
  });

  const deleteProvinceMutation = useMutation({
    mutationFn: (provinceId: string) => deleteProvince(provinceId, apiBaseUrl),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      messageApi.success("Province deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete province failed");
    }
  });

  const treeData = useMemo<TreeDataNode[]>(() => {
    const items = countriesTreeQuery.data ?? [];

    return items.map((item) => ({
      key: `country-${item.country.id}`,
      title: (
        <Space style={{ width: "100%", justifyContent: "space-between" }}>
          <span>
            <strong>{item.country.name}</strong> ({item.country.isoCode2}) - {item.country.nationality}
          </span>
          <Space>
            <Button
              size="small"
              onClick={(event) => {
                event.stopPropagation();
                setEditingCountry(item.country);
                editCountryForm.setFieldsValue({
                  name: item.country.name,
                  nationality: item.country.nationality,
                  numeric: item.country.numeric,
                  isoCode2: item.country.isoCode2,
                  isoCode3: item.country.isoCode3
                });
              }}
            >
              Edit Country
            </Button>
            <Button
              size="small"
              type="primary"
              onClick={(event) => {
                event.stopPropagation();
                setActiveCountryIdForProvinceCreate(item.country.id);
                setCreateProvinceOpen(true);
              }}
            >
              Add Province
            </Button>
          </Space>
        </Space>
      ),
      children: item.provinces.map((province) => ({
        key: `province-${province.id}`,
        title: (
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <span>
              {province.name}
              {province.iso ? ` (ISO: ${province.iso})` : ""}
            </span>
            <Space>
              <Button
                size="small"
                onClick={(event) => {
                  event.stopPropagation();
                  setEditingProvince(province);
                  editProvinceForm.setFieldsValue({
                    name: province.name,
                    hs: province.hs,
                    iso: province.iso,
                    fips: province.fips
                  });
                }}
              >
                Edit
              </Button>
              <Popconfirm
                title="Delete province?"
                okText="Delete"
                okButtonProps={{ danger: true, loading: deleteProvinceMutation.isPending }}
                onConfirm={(event) => {
                  event?.stopPropagation();
                  deleteProvinceMutation.mutate(province.id);
                }}
              >
                <Button
                  size="small"
                  danger
                  onClick={(event) => event.stopPropagation()}
                >
                  Delete
                </Button>
              </Popconfirm>
            </Space>
          </Space>
        )
      }))
    }));
  }, [countriesTreeQuery.data, deleteProvinceMutation.isPending, editCountryForm, editProvinceForm]);

  return (
    <div className="page-container">
      {contextHolder}
      <Card>
        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <div>
              <Title level={3} style={{ margin: 0 }}>
                Countries
              </Title>
              <Text type="secondary">Manage countries and provinces in tree view.</Text>
            </div>
            <Button type="primary" onClick={() => setCreateCountryOpen(true)}>
              New Country
            </Button>
          </Space>

          {countriesTreeQuery.isError ? (
            <Alert
              type="error"
              message="Failed to load countries"
              description={countriesTreeQuery.error instanceof Error ? countriesTreeQuery.error.message : "Unknown error"}
              showIcon
            />
          ) : countriesTreeQuery.isLoading ? (
            <Spin />
          ) : (
            <Tree
              showLine
              defaultExpandAll
              blockNode
              treeData={treeData}
            />
          )}
        </Space>
      </Card>

      <TopDrawerForm
        open={isCreateCountryOpen}
        title="Create Country"
        submitText="Create"
        onClose={() => {
          setCreateCountryOpen(false);
          countryForm.resetFields();
        }}
        onSubmit={() => countryForm.submit()}
        loading={createCountryMutation.isPending}
      >
        <CountryForm form={countryForm} onFinish={(values) => createCountryMutation.mutate(values)} />
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingCountry)}
        title="Edit Country"
        submitText="Update"
        onClose={() => {
          setEditingCountry(null);
          editCountryForm.resetFields();
        }}
        onSubmit={() => editCountryForm.submit()}
        loading={updateCountryMutation.isPending}
      >
        <CountryForm form={editCountryForm} onFinish={(values) => updateCountryMutation.mutate(values)} />
      </TopDrawerForm>

      <TopDrawerForm
        open={isCreateProvinceOpen}
        title="Add Province"
        submitText="Add"
        onClose={() => {
          setCreateProvinceOpen(false);
          setActiveCountryIdForProvinceCreate(null);
          provinceForm.resetFields();
        }}
        onSubmit={() => provinceForm.submit()}
        loading={createProvinceMutation.isPending}
      >
        <ProvinceForm form={provinceForm} onFinish={(values) => createProvinceMutation.mutate(values)} />
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingProvince)}
        title="Edit Province"
        submitText="Update"
        onClose={() => {
          setEditingProvince(null);
          editProvinceForm.resetFields();
        }}
        onSubmit={() => editProvinceForm.submit()}
        loading={updateProvinceMutation.isPending}
      >
        <ProvinceForm form={editProvinceForm} onFinish={(values) => updateProvinceMutation.mutate(values)} />
      </TopDrawerForm>
    </div>
  );
}

type CountryFormProps = {
  form: ReturnType<typeof Form.useForm<CountryFormValues>>[0];
  onFinish: (values: CountryFormValues) => void;
};

function CountryForm({ form, onFinish }: CountryFormProps) {
  return (
    <Form<CountryFormValues> form={form} layout="vertical" onFinish={onFinish}>
      <Form.Item name="name" label="Country Name" rules={[{ required: true, message: "Country name is required" }]}>
        <Input maxLength={200} />
      </Form.Item>
      <Form.Item name="nationality" label="Nationality" rules={[{ required: true, message: "Nationality is required" }]}>
        <Input maxLength={200} />
      </Form.Item>
      <Form.Item name="numeric" label="Numeric">
        <InputNumber min={0} max={999} style={{ width: "100%" }} />
      </Form.Item>
      <Form.Item name="isoCode2" label="ISO Code 2" rules={[{ required: true, message: "ISO code 2 is required" }]}>
        <Input maxLength={2} />
      </Form.Item>
      <Form.Item name="isoCode3" label="ISO Code 3" rules={[{ required: true, message: "ISO code 3 is required" }]}>
        <Input maxLength={3} />
      </Form.Item>
    </Form>
  );
}

type ProvinceFormProps = {
  form: ReturnType<typeof Form.useForm<ProvinceFormValues>>[0];
  onFinish: (values: ProvinceFormValues) => void;
};

function ProvinceForm({ form, onFinish }: ProvinceFormProps) {
  return (
    <Form<ProvinceFormValues> form={form} layout="vertical" onFinish={onFinish}>
      <Form.Item name="name" label="Province Name" rules={[{ required: true, message: "Province name is required" }]}>
        <Input maxLength={200} />
      </Form.Item>
      <Form.Item name="hs" label="HS">
        <Input maxLength={3} />
      </Form.Item>
      <Form.Item name="iso" label="ISO">
        <Input maxLength={6} />
      </Form.Item>
      <Form.Item name="fips" label="FIPS">
        <Input maxLength={5} />
      </Form.Item>
    </Form>
  );
}
