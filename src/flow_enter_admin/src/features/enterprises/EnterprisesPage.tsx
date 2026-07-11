import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable
} from "@tanstack/react-table";
import {
  Alert,
  Button,
  Card,
  DatePicker,
  Empty,
  Form,
  Input,
  InputNumber,
  Popover,
  Select,
  Space,
  Spin,
  Tabs,
  Popconfirm,
  Typography,
  message
} from "antd";
import dayjs from "dayjs";
import { useMemo, useState } from "react";
import {
  createEnperprise,
  deleteEnterpriseEmployment,
  createEnterpriseEmployment,
  fetchEnterpriseEmployments,
  fetchEnterprises,
  fetchLegalStructures,
  fetchPartyRoleTypes,
  updateEnterpriseEmploymentEffectiveDate,
  updateEnterpriseEmployment,
  updateEnperprise
} from "../../api/enterprises";
import { TopDrawerForm } from "../../components/TopDrawerForm";
import { CreateEmploymentRequest, CreateEnterpriseRequest, Enterprise, Employment } from "./types";

const { Title, Text } = Typography;

type EnterprisesPageProps = {
  apiBaseUrl?: string;
};

type FormValues = CreateEnterpriseRequest;
type EmploymentFormValues = CreateEmploymentRequest;
type EmploymentGroup = {
  employeePartyId: string;
  employeeFullName: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  employments: Employment[];
};

const defaultPageSize = 10;

export function EnterprisesPage({ apiBaseUrl }: EnterprisesPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(defaultPageSize);
  const [isCreateOpen, setCreateOpen] = useState(false);
  const [editingEnterprise, setEditingEnterprise] = useState<Enterprise | null>(null);
  const [peopleEnterprise, setPeopleEnterprise] = useState<Enterprise | null>(null);
  const [editingEmployment, setEditingEmployment] = useState<Employment | null>(null);
  const [isCreateEmploymentOpen, setCreateEmploymentOpen] = useState(false);
  const [effectiveDateDrafts, setEffectiveDateDrafts] = useState<
    Record<string, { fromDate: string; thruDate: string }>
  >({});
  const [peopleTabKey, setPeopleTabKey] = useState("people");
  const [createForm] = Form.useForm<FormValues>();
  const [editForm] = Form.useForm<FormValues>();
  const [employmentForm] = Form.useForm<EmploymentFormValues>();
  const [editEmploymentForm] = Form.useForm<EmploymentFormValues>();

  const enterprisesQuery = useQuery({
    queryKey: ["enterprises", pageIndex, pageSize, apiBaseUrl],
    queryFn: () => fetchEnterprises(pageIndex + 1, pageSize, apiBaseUrl)
  });

  const legalStructuresQuery = useQuery({
    queryKey: ["enterprises/legal-structures", apiBaseUrl],
    queryFn: () => fetchLegalStructures(apiBaseUrl)
  });

  const employmentsQuery = useQuery({
    queryKey: ["enterprise-employments", peopleEnterprise?.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseEmployments(peopleEnterprise!.enterpriseId, apiBaseUrl),
    enabled: Boolean(peopleEnterprise)
  });

  const partyRoleTypesQuery = useQuery({
    queryKey: ["party-role-types", apiBaseUrl],
    queryFn: () => fetchPartyRoleTypes(apiBaseUrl),
    enabled: Boolean(peopleEnterprise)
  });

  const createMutation = useMutation({
    mutationFn: (values: FormValues) => createEnperprise(values, apiBaseUrl),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["enterprises"] });
      setCreateOpen(false);
      createForm.resetFields();
      messageApi.success("Enterprise created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create failed");
    }
  });

  const updateEmploymentMutation = useMutation({
    mutationFn: async (values: EmploymentFormValues) => {
      if (!peopleEnterprise || !editingEmployment) {
        return;
      }

      await updateEnterpriseEmployment(
        peopleEnterprise.enterpriseId,
        editingEmployment.employmentId,
        values,
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-employments", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setEditingEmployment(null);
      editEmploymentForm.resetFields();
      messageApi.success("Employment updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update employment failed");
    }
  });

  const deleteEmploymentMutation = useMutation({
    mutationFn: async (employmentId: string) => {
      if (!peopleEnterprise) {
        return;
      }

      await deleteEnterpriseEmployment(peopleEnterprise.enterpriseId, employmentId, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-employments", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Employment deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete employment failed");
    }
  });

  const updateEmploymentEffectiveDateMutation = useMutation({
    mutationFn: async (payload: { employmentId: string; fromDate: string; thruDate: string }) => {
      if (!peopleEnterprise) {
        return;
      }

      await updateEnterpriseEmploymentEffectiveDate(
        peopleEnterprise.enterpriseId,
        payload.employmentId,
        {
          fromDate: payload.fromDate,
          thruDate: payload.thruDate
        },
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-employments", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Effective date updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update effective date failed");
    }
  });

  const updateMutation = useMutation({
    mutationFn: async (values: FormValues) => {
      if (!editingEnterprise) {
        return;
      }

      await updateEnperprise(
        {
          id: editingEnterprise.enterpriseId,
          changes: values
        },
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["enterprises"] });
      setEditingEnterprise(null);
      editForm.resetFields();
      messageApi.success("Enterprise updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update failed");
    }
  });

  const createEmploymentMutation = useMutation({
    mutationFn: async (values: EmploymentFormValues) => {
      if (!peopleEnterprise) {
        return;
      }

      await createEnterpriseEmployment(peopleEnterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-employments", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setCreateEmploymentOpen(false);
      employmentForm.resetFields();
      messageApi.success("Employment added");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create employment failed");
    }
  });

  const columns = useMemo<ColumnDef<Enterprise>[]>(
    () => [
      {
        accessorKey: "legalName",
        header: "Legal Name",
        cell: (info) => info.getValue<string>()
      },
      {
        accessorKey: "brandName",
        header: "Brand",
        cell: (info) => info.getValue<string>() || "-"
      },
      {
        accessorKey: "businessRegistrationNumber",
        header: "Registration No.",
        cell: (info) => info.getValue<string>() || "-"
      },
      {
        accessorKey: "taxId",
        header: "Tax ID",
        cell: (info) => info.getValue<string>() || "-"
      },
      {
        accessorKey: "fiscalYearStartMonth",
        header: "Fiscal Start Month",
        cell: (info) => info.getValue<number>()
      },
      {
        id: "actions",
        header: "Actions",
        cell: ({ row }) => (
          <Space>
            <Button
              size="small"
              onClick={() => {
                setEditingEnterprise(row.original);
                editForm.setFieldsValue({
                  legalName: row.original.legalName,
                  information: row.original.information,
                  brandName: row.original.brandName,
                  notes: row.original.notes,
                  legalStructureId: row.original.legalStructureId ?? "",
                  businessRegistrationNumber: row.original.businessRegistrationNumber,
                  taxId: row.original.taxId,
                  fiscalYearStartMonth: row.original.fiscalYearStartMonth
                });
              }}
            >
              Edit
            </Button>
            <Button size="small" onClick={() => setPeopleEnterprise(row.original)}>
              Manage People
            </Button>
          </Space>
        )
      }
    ],
    [editForm]
  );

  const enterprises = enterprisesQuery.data?.data ?? [];
  const totalCount = enterprisesQuery.data?.totalCount ?? 0;
  const pageCount = Math.max(1, Math.ceil(totalCount / pageSize));

  const table = useReactTable({
    data: enterprises,
    columns,
    pageCount,
    state: {
      pagination: { pageIndex, pageSize }
    },
    manualPagination: true,
    getCoreRowModel: getCoreRowModel()
  });

  const legalStructureOptions = (legalStructuresQuery.data ?? []).map((item) => ({
    value: item.id,
    label: `${item.name}${item.code ? ` (${item.code})` : ""}`
  }));

  const roleTypeOptions = (partyRoleTypesQuery.data ?? [])
    .filter((item) => item.code !== "ENTERPRISE")
    .map((item) => ({
      value: item.id,
      label: `${item.name}${item.code ? ` (${item.code})` : ""}`
    }));

  const employments = employmentsQuery.data ?? [];
  const employmentGroups = useMemo<EmploymentGroup[]>(() => {
    const map = new Map<string, EmploymentGroup>();
    for (const employment of employments) {
      const existing = map.get(employment.employeePartyId);
      if (existing) {
        existing.employments.push(employment);
        continue;
      }

      map.set(employment.employeePartyId, {
        employeePartyId: employment.employeePartyId,
        employeeFullName: employment.employeeFullName,
        firstName: employment.firstName,
        middleName: employment.middleName,
        lastName: employment.lastName,
        employments: [employment]
      });
    }

    return Array.from(map.values()).sort((left, right) => left.employeeFullName.localeCompare(right.employeeFullName));
  }, [employments]);

  const getEffectiveDateDraft = (employment: Employment) =>
    effectiveDateDrafts[employment.employmentId] ?? {
      fromDate: employment.fromDate,
      thruDate: employment.thruDate
    };

  if (peopleEnterprise) {
    return (
      <div className="page-container">
        {contextHolder}
        <Card>
          <Space direction="vertical" size="middle" style={{ width: "100%" }}>
            <Space style={{ width: "100%", justifyContent: "space-between" }}>
              <div>
                <Title level={3} style={{ margin: 0 }}>
                  Enterprise People
                </Title>
                <Text type="secondary">
                  Manage people under enterprise: <strong>{peopleEnterprise.legalName}</strong>
                </Text>
              </div>
              <Button
                onClick={() => {
                  setPeopleEnterprise(null);
                  setEditingEmployment(null);
                  setCreateEmploymentOpen(false);
                  setEffectiveDateDrafts({});
                  setPeopleTabKey("people");
                  employmentForm.resetFields();
                  editEmploymentForm.resetFields();
                }}
              >
                Back to Enterprises
              </Button>
            </Space>
            <Tabs
              activeKey={peopleTabKey}
              onChange={setPeopleTabKey}
              items={[
                {
                  key: "people",
                  label: "People",
                  children: <Empty description="People management UI for this enterprise will be added here." />
                },
                {
                  key: "employments",
                  label: "Employments",
                  children: (
                    <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                      <Space style={{ width: "100%", justifyContent: "flex-end" }}>
                        <Button type="primary" onClick={() => setCreateEmploymentOpen(true)}>
                          Add Employment
                        </Button>
                      </Space>

                      {employmentsQuery.isError ? (
                        <Alert
                          type="error"
                          message="Failed to load employments"
                          description={employmentsQuery.error instanceof Error ? employmentsQuery.error.message : "Unknown error"}
                          showIcon
                        />
                      ) : employmentsQuery.isLoading ? (
                        <Spin />
                      ) : employmentGroups.length === 0 ? (
                        <Empty description="No employments found for this enterprise." />
                      ) : (
                        <div className="tanstack-table-wrapper">
                          <table className="tanstack-table">
                            <thead>
                              <tr>
                                <th>Employee</th>
                                <th>Roles</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              {employmentGroups.map((employmentGroup) => (
                                <tr key={employmentGroup.employeePartyId}>
                                  <td>{employmentGroup.employeeFullName}</td>
                                  <td>
                                    <Popover
                                      trigger="click"
                                      content={
                                        <div style={{ width: 560 }}>
                                          <table className="tanstack-table">
                                            <thead>
                                              <tr>
                                                <th>Role</th>
                                                <th>From Date</th>
                                                <th>Thru Date</th>
                                                <th>Action</th>
                                              </tr>
                                            </thead>
                                            <tbody>
                                              {employmentGroup.employments.map((item) => {
                                                const draft = getEffectiveDateDraft(item);
                                                return (
                                                  <tr key={item.employmentId}>
                                                    <td>
                                                      {item.partyRoleTypeName}
                                                      {item.partyRoleTypeCode ? ` (${item.partyRoleTypeCode})` : ""}
                                                    </td>
                                                    <td>
                                                      <DatePicker
                                                        format="DD/MM/YYYY"
                                                        style={{ width: 150 }}
                                                        value={dayjs(draft.fromDate)}
                                                        onChange={(value) => {
                                                          if (!value) {
                                                            return;
                                                          }
                                                          setEffectiveDateDrafts((current) => ({
                                                            ...current,
                                                            [item.employmentId]: {
                                                              ...getEffectiveDateDraft(item),
                                                              fromDate: value.format("YYYY-MM-DD")
                                                            }
                                                          }));
                                                        }}
                                                      />
                                                    </td>
                                                    <td>
                                                      <DatePicker
                                                        format="DD/MM/YYYY"
                                                        style={{ width: 150 }}
                                                        value={dayjs(draft.thruDate)}
                                                        onChange={(value) => {
                                                          if (!value) {
                                                            return;
                                                          }
                                                          setEffectiveDateDrafts((current) => ({
                                                            ...current,
                                                            [item.employmentId]: {
                                                              ...getEffectiveDateDraft(item),
                                                              thruDate: value.format("YYYY-MM-DD")
                                                            }
                                                          }));
                                                        }}
                                                      />
                                                    </td>
                                                    <td>
                                                      <Button
                                                        size="small"
                                                        onClick={() =>
                                                          updateEmploymentEffectiveDateMutation.mutate({
                                                            employmentId: item.employmentId,
                                                            fromDate: getEffectiveDateDraft(item).fromDate,
                                                            thruDate: getEffectiveDateDraft(item).thruDate
                                                          })
                                                        }
                                                        loading={updateEmploymentEffectiveDateMutation.isPending}
                                                      >
                                                        Save
                                                      </Button>
                                                    </td>
                                                  </tr>
                                                );
                                              })}
                                            </tbody>
                                          </table>
                                        </div>
                                      }
                                    >
                                      <Button size="small">
                                        Roles ({employmentGroup.employments.length})
                                      </Button>
                                    </Popover>
                                  </td>
                                  <td>
                                    <Space>
                                      <Button
                                        size="small"
                                        onClick={() => {
                                          const primaryEmployment = employmentGroup.employments[0];
                                          setEditingEmployment(primaryEmployment);
                                          editEmploymentForm.setFieldsValue({
                                            firstName: employmentGroup.firstName,
                                            middleName: employmentGroup.middleName,
                                            lastName: employmentGroup.lastName,
                                            partyRoleTypeIds: employmentGroup.employments.map((item) => item.partyRoleTypeId)
                                          });
                                        }}
                                      >
                                        Edit
                                      </Button>
                                      <Popconfirm
                                        title="Delete employment?"
                                        description="This will remove this employee and all roles in this enterprise."
                                        okText="Delete"
                                        okButtonProps={{ danger: true, loading: deleteEmploymentMutation.isPending }}
                                        onConfirm={() =>
                                          deleteEmploymentMutation.mutate(employmentGroup.employments[0].employmentId)
                                        }
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
                  )
                }
              ]}
            />

            <TopDrawerForm
              open={isCreateEmploymentOpen}
              title="Add Employment"
              submitText="Add"
              onClose={() => {
                setCreateEmploymentOpen(false);
                employmentForm.resetFields();
              }}
              onSubmit={() => employmentForm.submit()}
              loading={createEmploymentMutation.isPending}
            >
              <Form<EmploymentFormValues>
                form={employmentForm}
                layout="vertical"
                onFinish={(values) => createEmploymentMutation.mutate(values)}
              >
                <Form.Item
                  name="firstName"
                  label="First Name"
                  rules={[{ required: true, message: "First name is required" }]}
                >
                  <Input maxLength={300} />
                </Form.Item>
                <Form.Item name="middleName" label="Middle Name">
                  <Input maxLength={300} />
                </Form.Item>
                <Form.Item
                  name="lastName"
                  label="Last Name"
                  rules={[{ required: true, message: "Last name is required" }]}
                >
                  <Input maxLength={500} />
                </Form.Item>
                <Form.Item
                  name="partyRoleTypeIds"
                  label="Party Roles"
                  rules={[{ required: true, message: "At least one party role is required" }]}
                >
                  <Select
                    mode="multiple"
                    showSearch
                    options={roleTypeOptions}
                    loading={partyRoleTypesQuery.isLoading}
                    placeholder="Select party roles"
                    optionFilterProp="label"
                  />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={Boolean(editingEmployment)}
              title="Edit Employment"
              submitText="Update"
              onClose={() => {
                setEditingEmployment(null);
                editEmploymentForm.resetFields();
              }}
              onSubmit={() => editEmploymentForm.submit()}
              loading={updateEmploymentMutation.isPending}
            >
              <Form<EmploymentFormValues>
                form={editEmploymentForm}
                layout="vertical"
                onFinish={(values) => updateEmploymentMutation.mutate(values)}
              >
                <Form.Item
                  name="firstName"
                  label="First Name"
                  rules={[{ required: true, message: "First name is required" }]}
                >
                  <Input maxLength={300} />
                </Form.Item>
                <Form.Item name="middleName" label="Middle Name">
                  <Input maxLength={300} />
                </Form.Item>
                <Form.Item
                  name="lastName"
                  label="Last Name"
                  rules={[{ required: true, message: "Last name is required" }]}
                >
                  <Input maxLength={500} />
                </Form.Item>
                <Form.Item
                  name="partyRoleTypeIds"
                  label="Party Roles"
                  rules={[{ required: true, message: "At least one party role is required" }]}
                >
                  <Select
                    mode="multiple"
                    showSearch
                    options={roleTypeOptions}
                    loading={partyRoleTypesQuery.isLoading}
                    placeholder="Select party roles"
                    optionFilterProp="label"
                  />
                </Form.Item>
              </Form>
            </TopDrawerForm>
          </Space>
        </Card>
      </div>
    );
  }

  return (
    <div className="page-container">
      {contextHolder}
      <Card>
        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <div>
              <Title level={3} style={{ margin: 0 }}>
                Enterprises
              </Title>
              <Text type="secondary">Manage enterprise enterprises from Flow Enter backend.</Text>
            </div>
            <Button type="primary" onClick={() => setCreateOpen(true)}>
              New Enterprises
            </Button>
          </Space>

          {enterprisesQuery.isError ? (
            <Alert
              type="error"
              message="Failed to load enterprises"
              description={enterprisesQuery.error instanceof Error ? enterprisesQuery.error.message : "Unknown error"}
              showIcon
            />
          ) : (
            <div className="tanstack-table-wrapper">
              <table className="tanstack-table">
                <thead>
                  {table.getHeaderGroups().map((headerGroup) => (
                    <tr key={headerGroup.id}>
                      {headerGroup.headers.map((header) => (
                        <th key={header.id}>
                          {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                        </th>
                      ))}
                    </tr>
                  ))}
                </thead>
                <tbody>
                  {enterprisesQuery.isLoading ? (
                    <tr>
                      <td colSpan={columns.length}>
                        <div className="table-loading">
                          <Spin />
                        </div>
                      </td>
                    </tr>
                  ) : table.getRowModel().rows.length === 0 ? (
                    <tr>
                      <td colSpan={columns.length}>No enterprises found.</td>
                    </tr>
                  ) : (
                    table.getRowModel().rows.map((row) => (
                      <tr key={row.id}>
                        {row.getVisibleCells().map((cell) => (
                          <td key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</td>
                        ))}
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          )}

          <Space style={{ justifyContent: "space-between", width: "100%" }}>
            <Text type="secondary">
              Total {totalCount} enterprises • Page {pageIndex + 1} / {pageCount}
            </Text>
            <Space>
              <Button onClick={() => setPageSize(10)} disabled={pageSize === 10}>
                10 rows
              </Button>
              <Button onClick={() => setPageSize(25)} disabled={pageSize === 25}>
                25 rows
              </Button>
              <Button onClick={() => setPageIndex((value) => Math.max(0, value - 1))} disabled={pageIndex <= 0}>
                Prev
              </Button>
              <Button
                onClick={() => setPageIndex((value) => Math.min(pageCount - 1, value + 1))}
                disabled={pageIndex >= pageCount - 1}
              >
                Next
              </Button>
            </Space>
          </Space>
        </Space>
      </Card>

      <TopDrawerForm
        open={isCreateOpen}
        title="Create Enterprise"
        submitText="Create"
        onClose={() => setCreateOpen(false)}
        onSubmit={() => createForm.submit()}
        loading={createMutation.isPending}
      >
        <EnterpriseForm
          form={createForm}
          legalStructureOptions={legalStructureOptions}
          loadingLegalStructures={legalStructuresQuery.isLoading}
          onFinish={(values) => createMutation.mutate(values)}
        />
      </TopDrawerForm>

      <TopDrawerForm
        open={Boolean(editingEnterprise)}
        title="Edit Enterprise"
        submitText="Update"
        onClose={() => {
          setEditingEnterprise(null);
          editForm.resetFields();
        }}
        onSubmit={() => editForm.submit()}
        loading={updateMutation.isPending}
      >
        <EnterpriseForm
          form={editForm}
          legalStructureOptions={legalStructureOptions}
          loadingLegalStructures={legalStructuresQuery.isLoading}
          onFinish={(values) => updateMutation.mutate(values)}
        />
      </TopDrawerForm>
    </div>
  );
}

type EnterpriseFormProps = {
  form: ReturnType<typeof Form.useForm<FormValues>>[0];
  legalStructureOptions: Array<{ value: string; label: string }>;
  loadingLegalStructures: boolean;
  onFinish: (values: FormValues) => void;
};

function EnterpriseForm({
  form,
  legalStructureOptions,
  loadingLegalStructures,
  onFinish
}: EnterpriseFormProps) {
  return (
    <Form<FormValues> form={form} layout="vertical" onFinish={onFinish} initialValues={{ fiscalYearStartMonth: 1 }}>
      <Form.Item name="legalName" label="Legal Name" rules={[{ required: true, message: "Legal name is required" }]}>
        <Input maxLength={200} />
      </Form.Item>
      <Form.Item name="brandName" label="Brand Name">
        <Input maxLength={100} />
      </Form.Item>
      <Form.Item
        name="legalStructureId"
        label="Legal Structure"
        rules={[{ required: true, message: "Legal structure is required" }]}
      >
        <Select
          showSearch
          options={legalStructureOptions}
          loading={loadingLegalStructures}
          placeholder="Select legal structure"
          optionFilterProp="label"
        />
      </Form.Item>
      <Form.Item name="businessRegistrationNumber" label="Business Registration Number">
        <Input maxLength={50} />
      </Form.Item>
      <Form.Item name="taxId" label="Tax ID">
        <Input maxLength={50} />
      </Form.Item>
      <Form.Item name="fiscalYearStartMonth" label="Fiscal Year Start Month" rules={[{ required: true }]}>
        <InputNumber min={1} max={12} style={{ width: "100%" }} />
      </Form.Item>
      <Form.Item name="information" label="Information">
        <Input.TextArea rows={3} maxLength={500} />
      </Form.Item>
      <Form.Item name="notes" label="Notes">
        <Input.TextArea rows={3} maxLength={500} />
      </Form.Item>
    </Form>
  );
}
