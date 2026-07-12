import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert, Button, Card, Form, Input, InputNumber, Popconfirm, Space, Spin, Tree, Typography, message } from "antd";
import type { TreeDataNode } from "antd";
import { useEffect, useMemo, useState } from "react";
import {
  createCountry,
  createDistrict,
  createProvince,
  createSubdistrict,
  deleteDistrict,
  deleteProvince,
  deleteSubdistrict,
  fetchCountriesTree,
  updateCountry,
  updateDistrict,
  updateProvince,
  updateSubdistrict
} from "../../api/countries";
import { TopDrawerForm } from "../../components/TopDrawerForm";
import {
  Country,
  CreateCountryRequest,
  CreateDistrictRequest,
  CreateProvinceRequest,
  CreateSubdistrictRequest,
  District,
  Province,
  Subdistrict
} from "./types";

const { Title, Text } = Typography;
const COUNTRIES_TREE_EXPANDED_KEYS_STORAGE_KEY = "countries-tree-expanded-keys";

type CountriesPageProps = {
  apiBaseUrl?: string;
};

type CountryFormValues = CreateCountryRequest;
type ProvinceFormValues = CreateProvinceRequest;
type DistrictFormValues = CreateDistrictRequest;
type SubdistrictFormValues = CreateSubdistrictRequest;
type PersistedTreeState = {
  expandedKeys: string[];
};

export function CountriesPage({ apiBaseUrl }: CountriesPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const legacyExpandedKeysStorageKey = `countries-tree-expanded-keys:${apiBaseUrl ?? "default"}`;
  const [isCreateCountryOpen, setCreateCountryOpen] = useState(false);
  const [editingCountry, setEditingCountry] = useState<Country | null>(null);
  const [isCreateProvinceOpen, setCreateProvinceOpen] = useState(false);
  const [activeCountryIdForProvinceCreate, setActiveCountryIdForProvinceCreate] = useState<string | null>(null);
  const [editingProvince, setEditingProvince] = useState<Province | null>(null);
  const [isCreateDistrictOpen, setCreateDistrictOpen] = useState(false);
  const [activeProvinceIdForDistrictCreate, setActiveProvinceIdForDistrictCreate] = useState<string | null>(null);
  const [editingDistrict, setEditingDistrict] = useState<District | null>(null);
  const [isCreateSubdistrictOpen, setCreateSubdistrictOpen] = useState(false);
  const [activeDistrictIdForSubdistrictCreate, setActiveDistrictIdForSubdistrictCreate] = useState<string | null>(null);
  const [editingSubdistrict, setEditingSubdistrict] = useState<Subdistrict | null>(null);
  const [countryForm] = Form.useForm<CountryFormValues>();
  const [editCountryForm] = Form.useForm<CountryFormValues>();
  const [provinceForm] = Form.useForm<ProvinceFormValues>();
  const [editProvinceForm] = Form.useForm<ProvinceFormValues>();
  const [districtForm] = Form.useForm<DistrictFormValues>();
  const [editDistrictForm] = Form.useForm<DistrictFormValues>();
  const [subdistrictForm] = Form.useForm<SubdistrictFormValues>();
  const [editSubdistrictForm] = Form.useForm<SubdistrictFormValues>();
  const [expandedKeys, setExpandedKeys] = useState<string[] | null>(() => {
    if (typeof window === "undefined") {
      return null;
    }

    try {
      const stored =
        localStorage.getItem(COUNTRIES_TREE_EXPANDED_KEYS_STORAGE_KEY) ??
        localStorage.getItem(legacyExpandedKeysStorageKey);
      if (!stored) {
        return null;
      }

      const parsed = JSON.parse(stored) as PersistedTreeState | string[];
      if (Array.isArray(parsed)) {
        return parsed.filter((key): key is string => typeof key === "string");
      }

      if (!parsed || !Array.isArray(parsed.expandedKeys)) {
        return null;
      }

      return parsed.expandedKeys.filter((key): key is string => typeof key === "string");
    } catch {
      return null;
    }
  });

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

  const createDistrictMutation = useMutation({
    mutationFn: async (values: DistrictFormValues) => {
      if (!activeProvinceIdForDistrictCreate) {
        return;
      }

      await createDistrict(activeProvinceIdForDistrictCreate, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setCreateDistrictOpen(false);
      setActiveProvinceIdForDistrictCreate(null);
      districtForm.resetFields();
      messageApi.success("District created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create district failed");
    }
  });

  const updateDistrictMutation = useMutation({
    mutationFn: async (values: DistrictFormValues) => {
      if (!editingDistrict) {
        return;
      }

      await updateDistrict(
        {
          id: editingDistrict.id,
          changes: values
        },
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setEditingDistrict(null);
      editDistrictForm.resetFields();
      messageApi.success("District updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update district failed");
    }
  });

  const deleteDistrictMutation = useMutation({
    mutationFn: (districtId: string) => deleteDistrict(districtId, apiBaseUrl),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      messageApi.success("District deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete district failed");
    }
  });

  const createSubdistrictMutation = useMutation({
    mutationFn: async (values: SubdistrictFormValues) => {
      if (!activeDistrictIdForSubdistrictCreate) {
        return;
      }

      await createSubdistrict(activeDistrictIdForSubdistrictCreate, values, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setCreateSubdistrictOpen(false);
      setActiveDistrictIdForSubdistrictCreate(null);
      subdistrictForm.resetFields();
      messageApi.success("Subdistrict created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create subdistrict failed");
    }
  });

  const updateSubdistrictMutation = useMutation({
    mutationFn: async (values: SubdistrictFormValues) => {
      if (!editingSubdistrict) {
        return;
      }

      await updateSubdistrict(
        {
          id: editingSubdistrict.id,
          changes: values
        },
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      setEditingSubdistrict(null);
      editSubdistrictForm.resetFields();
      messageApi.success("Subdistrict updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update subdistrict failed");
    }
  });

  const deleteSubdistrictMutation = useMutation({
    mutationFn: (subdistrictId: string) => deleteSubdistrict(subdistrictId, apiBaseUrl),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries-tree"] });
      messageApi.success("Subdistrict deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete subdistrict failed");
    }
  });

  const treeData = useMemo<TreeDataNode[]>(() => {
    const items = countriesTreeQuery.data ?? [];

    return items.map((countryItem) => ({
      key: `country-${countryItem.country.id}`,
      title: (
        <Space style={{ width: "100%", justifyContent: "space-between" }}>
          <span>
            <strong>{countryItem.country.name}</strong> ({countryItem.country.isoCode2}) - {countryItem.country.nationality}
          </span>
          <Space>
            <Button
              size="small"
              onClick={(event) => {
                event.stopPropagation();
                setEditingCountry(countryItem.country);
                editCountryForm.setFieldsValue({
                  name: countryItem.country.name,
                  nationality: countryItem.country.nationality,
                  numeric: countryItem.country.numeric,
                  isoCode2: countryItem.country.isoCode2,
                  isoCode3: countryItem.country.isoCode3
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
                setActiveCountryIdForProvinceCreate(countryItem.country.id);
                setCreateProvinceOpen(true);
              }}
            >
              Add Province
            </Button>
          </Space>
        </Space>
      ),
      children: countryItem.provinces.map((provinceItem) => ({
        key: `province-${provinceItem.province.id}`,
        title: (
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <span>{provinceItem.province.name}</span>
            <Space>
              <Button
                size="small"
                onClick={(event) => {
                  event.stopPropagation();
                  setEditingProvince(provinceItem.province);
                  editProvinceForm.setFieldsValue({
                    name: provinceItem.province.name,
                    hs: provinceItem.province.hs,
                    iso: provinceItem.province.iso,
                    fips: provinceItem.province.fips
                  });
                }}
              >
                Edit Province
              </Button>
              <Button
                size="small"
                type="primary"
                onClick={(event) => {
                  event.stopPropagation();
                  setActiveProvinceIdForDistrictCreate(provinceItem.province.id);
                  setCreateDistrictOpen(true);
                }}
              >
                Add District
              </Button>
              <Popconfirm
                title="Delete province?"
                okText="Delete"
                okButtonProps={{ danger: true, loading: deleteProvinceMutation.isPending }}
                onConfirm={(event) => {
                  event?.stopPropagation();
                  deleteProvinceMutation.mutate(provinceItem.province.id);
                }}
              >
                <Button size="small" danger onClick={(event) => event.stopPropagation()}>
                  Delete
                </Button>
              </Popconfirm>
            </Space>
          </Space>
        ),
        children: provinceItem.districts.map((districtItem) => ({
          key: `district-${districtItem.district.id}`,
          title: (
            <Space style={{ width: "100%", justifyContent: "space-between" }}>
              <span>{districtItem.district.name}</span>
              <Space>
                <Button
                  size="small"
                  onClick={(event) => {
                    event.stopPropagation();
                    setEditingDistrict(districtItem.district);
                    editDistrictForm.setFieldsValue({
                      name: districtItem.district.name,
                      prefixName: districtItem.district.prefixName,
                      prefixShortName: districtItem.district.prefixShortName,
                      postalCode: districtItem.district.postalCode
                    });
                  }}
                >
                  Edit District
                </Button>
                <Button
                  size="small"
                  type="primary"
                  onClick={(event) => {
                    event.stopPropagation();
                    setActiveDistrictIdForSubdistrictCreate(districtItem.district.id);
                    setCreateSubdistrictOpen(true);
                  }}
                >
                  Add Subdistrict
                </Button>
                <Popconfirm
                  title="Delete district?"
                  okText="Delete"
                  okButtonProps={{ danger: true, loading: deleteDistrictMutation.isPending }}
                  onConfirm={(event) => {
                    event?.stopPropagation();
                    deleteDistrictMutation.mutate(districtItem.district.id);
                  }}
                >
                  <Button size="small" danger onClick={(event) => event.stopPropagation()}>
                    Delete
                  </Button>
                </Popconfirm>
              </Space>
            </Space>
          ),
          children: districtItem.subdistricts.map((subdistrict) => ({
            key: `subdistrict-${subdistrict.id}`,
            title: (
              <Space style={{ width: "100%", justifyContent: "space-between" }}>
                <span>{subdistrict.name}</span>
                <Space>
                  <Button
                    size="small"
                    onClick={(event) => {
                      event.stopPropagation();
                      setEditingSubdistrict(subdistrict);
                      editSubdistrictForm.setFieldsValue({
                        name: subdistrict.name,
                        prefixName: subdistrict.prefixName,
                        prefixShortName: subdistrict.prefixShortName,
                        postalCode: subdistrict.postalCode
                      });
                    }}
                  >
                    Edit
                  </Button>
                  <Popconfirm
                    title="Delete subdistrict?"
                    okText="Delete"
                    okButtonProps={{ danger: true, loading: deleteSubdistrictMutation.isPending }}
                    onConfirm={(event) => {
                      event?.stopPropagation();
                      deleteSubdistrictMutation.mutate(subdistrict.id);
                    }}
                  >
                    <Button size="small" danger onClick={(event) => event.stopPropagation()}>
                      Delete
                    </Button>
                  </Popconfirm>
                </Space>
              </Space>
            )
          }))
        }))
      }))
    }));
  }, [
    countriesTreeQuery.data,
    deleteProvinceMutation.isPending,
    deleteDistrictMutation.isPending,
    deleteSubdistrictMutation.isPending,
    editCountryForm,
    editProvinceForm,
    editDistrictForm,
    editSubdistrictForm
  ]);

  const allExpandableTreeKeys = useMemo<string[]>(() => {
    const keys: string[] = [];
    const collect = (nodes: TreeDataNode[]) => {
      nodes.forEach((node) => {
        if (node.children && node.children.length > 0) {
          keys.push(String(node.key));
          collect(node.children as TreeDataNode[]);
        }
      });
    };

    collect(treeData);
    return keys;
  }, [treeData]);

  useEffect(() => {
    if (expandedKeys !== null) {
      return;
    }

    if (allExpandableTreeKeys.length === 0) {
      return;
    }

    setExpandedKeys(allExpandableTreeKeys);
  }, [allExpandableTreeKeys, expandedKeys]);

  useEffect(() => {
    if (expandedKeys === null) {
      return;
    }

    if (allExpandableTreeKeys.length === 0) {
      return;
    }

    const validKeys = new Set(allExpandableTreeKeys);
    const filteredKeys = expandedKeys.filter((key) => validKeys.has(key));
    if (filteredKeys.length !== expandedKeys.length) {
      setExpandedKeys(filteredKeys);
    }
  }, [allExpandableTreeKeys, expandedKeys]);

  useEffect(() => {
    if (typeof window === "undefined") {
      return;
    }

    if (expandedKeys === null) {
      return;
    }

    localStorage.setItem(
      COUNTRIES_TREE_EXPANDED_KEYS_STORAGE_KEY,
      JSON.stringify({ expandedKeys } as PersistedTreeState)
    );
  }, [expandedKeys]);

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
              <Text type="secondary">Manage countries, provinces, districts and subdistricts in tree view.</Text>
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
              blockNode
              treeData={treeData}
              expandedKeys={expandedKeys ?? undefined}
              onExpand={(keys) => setExpandedKeys(keys.map((key) => String(key)))}
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

      <TopDrawerForm
        open={isCreateDistrictOpen}
        title="Add District"
        submitText="Add"
        onClose={() => {
          setCreateDistrictOpen(false);
          setActiveProvinceIdForDistrictCreate(null);
          districtForm.resetFields();
        }}
        onSubmit={() => districtForm.submit()}
        loading={createDistrictMutation.isPending}
      >
        <DistrictForm form={districtForm} onFinish={(values) => createDistrictMutation.mutate(values)} />
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingDistrict)}
        title="Edit District"
        submitText="Update"
        onClose={() => {
          setEditingDistrict(null);
          editDistrictForm.resetFields();
        }}
        onSubmit={() => editDistrictForm.submit()}
        loading={updateDistrictMutation.isPending}
      >
        <DistrictForm form={editDistrictForm} onFinish={(values) => updateDistrictMutation.mutate(values)} />
      </TopDrawerForm>

      <TopDrawerForm
        open={isCreateSubdistrictOpen}
        title="Add Subdistrict"
        submitText="Add"
        onClose={() => {
          setCreateSubdistrictOpen(false);
          setActiveDistrictIdForSubdistrictCreate(null);
          subdistrictForm.resetFields();
        }}
        onSubmit={() => subdistrictForm.submit()}
        loading={createSubdistrictMutation.isPending}
      >
        <SubdistrictForm form={subdistrictForm} onFinish={(values) => createSubdistrictMutation.mutate(values)} />
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingSubdistrict)}
        title="Edit Subdistrict"
        submitText="Update"
        onClose={() => {
          setEditingSubdistrict(null);
          editSubdistrictForm.resetFields();
        }}
        onSubmit={() => editSubdistrictForm.submit()}
        loading={updateSubdistrictMutation.isPending}
      >
        <SubdistrictForm form={editSubdistrictForm} onFinish={(values) => updateSubdistrictMutation.mutate(values)} />
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

type DistrictFormProps = {
  form: ReturnType<typeof Form.useForm<DistrictFormValues>>[0];
  onFinish: (values: DistrictFormValues) => void;
};

function DistrictForm({ form, onFinish }: DistrictFormProps) {
  return (
    <Form<DistrictFormValues> form={form} layout="vertical" onFinish={onFinish}>
      <Form.Item name="name" label="District Name" rules={[{ required: true, message: "District name is required" }]}>
        <Input maxLength={200} />
      </Form.Item>
      <Form.Item name="prefixName" label="Prefix Name">
        <Input maxLength={10} />
      </Form.Item>
      <Form.Item name="prefixShortName" label="Prefix Short Name">
        <Input maxLength={5} />
      </Form.Item>
      <Form.Item name="postalCode" label="Postal Code">
        <Input maxLength={10} />
      </Form.Item>
    </Form>
  );
}

type SubdistrictFormProps = {
  form: ReturnType<typeof Form.useForm<SubdistrictFormValues>>[0];
  onFinish: (values: SubdistrictFormValues) => void;
};

function SubdistrictForm({ form, onFinish }: SubdistrictFormProps) {
  return (
    <Form<SubdistrictFormValues> form={form} layout="vertical" onFinish={onFinish}>
      <Form.Item name="name" label="Subdistrict Name" rules={[{ required: true, message: "Subdistrict name is required" }]}>
        <Input maxLength={200} />
      </Form.Item>
      <Form.Item name="prefixName" label="Prefix Name">
        <Input maxLength={10} />
      </Form.Item>
      <Form.Item name="prefixShortName" label="Prefix Short Name">
        <Input maxLength={5} />
      </Form.Item>
      <Form.Item name="postalCode" label="Postal Code">
        <Input maxLength={10} />
      </Form.Item>
    </Form>
  );
}
