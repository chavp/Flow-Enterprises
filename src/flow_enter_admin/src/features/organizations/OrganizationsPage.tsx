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
  Form,
  Input,
  InputNumber,
  Modal,
  Select,
  Space,
  Spin,
  Typography,
  message
} from "antd";
import { useMemo, useState } from "react";
import { createOrganization, fetchLegalStructures, fetchOrganizations, updateOrganization } from "../../api/organizations";
import { CreateOrganizationRequest, Organization } from "./types";

const { Title, Text } = Typography;

type OrganizationsPageProps = {
  apiBaseUrl?: string;
};

type FormValues = CreateOrganizationRequest;

const defaultPageSize = 10;

export function OrganizationsPage({ apiBaseUrl }: OrganizationsPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(defaultPageSize);
  const [isCreateOpen, setCreateOpen] = useState(false);
  const [editingOrganization, setEditingOrganization] = useState<Organization | null>(null);
  const [createForm] = Form.useForm<FormValues>();
  const [editForm] = Form.useForm<FormValues>();

  const organizationsQuery = useQuery({
    queryKey: ["organizations", pageIndex, pageSize, apiBaseUrl],
    queryFn: () => fetchOrganizations(pageIndex + 1, pageSize, apiBaseUrl)
  });

  const legalStructuresQuery = useQuery({
    queryKey: ["legal-structures", apiBaseUrl],
    queryFn: () => fetchLegalStructures(apiBaseUrl)
  });

  const createMutation = useMutation({
    mutationFn: (values: FormValues) => createOrganization(values, apiBaseUrl),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["organizations"] });
      setCreateOpen(false);
      createForm.resetFields();
      messageApi.success("Organization created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create failed");
    }
  });

  const updateMutation = useMutation({
    mutationFn: async (values: FormValues) => {
      if (!editingOrganization) {
        return;
      }

      await updateOrganization(
        {
          id: editingOrganization.enterpriseId,
          changes: values
        },
        apiBaseUrl
      );
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["organizations"] });
      setEditingOrganization(null);
      editForm.resetFields();
      messageApi.success("Organization updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update failed");
    }
  });

  const columns = useMemo<ColumnDef<Organization>[]>(
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
          <Button
            size="small"
            onClick={() => {
              setEditingOrganization(row.original);
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
        )
      }
    ],
    [editForm]
  );

  const organizations = organizationsQuery.data?.data ?? [];
  const totalCount = organizationsQuery.data?.totalCount ?? 0;
  const pageCount = Math.max(1, Math.ceil(totalCount / pageSize));

  const table = useReactTable({
    data: organizations,
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

  return (
    <div className="page-container">
      {contextHolder}
      <Card>
        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <div>
              <Title level={3} style={{ margin: 0 }}>
                Organizations
              </Title>
              <Text type="secondary">Manage enterprise organizations from Flow Enter backend.</Text>
            </div>
            <Button type="primary" onClick={() => setCreateOpen(true)}>
              New Organization
            </Button>
          </Space>

          {organizationsQuery.isError ? (
            <Alert
              type="error"
              message="Failed to load organizations"
              description={organizationsQuery.error instanceof Error ? organizationsQuery.error.message : "Unknown error"}
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
                  {organizationsQuery.isLoading ? (
                    <tr>
                      <td colSpan={columns.length}>
                        <div className="table-loading">
                          <Spin />
                        </div>
                      </td>
                    </tr>
                  ) : table.getRowModel().rows.length === 0 ? (
                    <tr>
                      <td colSpan={columns.length}>No organizations found.</td>
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
              Total {totalCount} organizations • Page {pageIndex + 1} / {pageCount}
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

      <Modal
        open={isCreateOpen}
        title="Create Organization"
        onCancel={() => setCreateOpen(false)}
        onOk={() => createForm.submit()}
        confirmLoading={createMutation.isPending}
        destroyOnClose
      >
        <OrganizationForm
          form={createForm}
          legalStructureOptions={legalStructureOptions}
          loadingLegalStructures={legalStructuresQuery.isLoading}
          onFinish={(values) => createMutation.mutate(values)}
        />
      </Modal>

      <Modal
        open={Boolean(editingOrganization)}
        title="Edit Organization"
        onCancel={() => {
          setEditingOrganization(null);
          editForm.resetFields();
        }}
        onOk={() => editForm.submit()}
        confirmLoading={updateMutation.isPending}
        destroyOnClose
      >
        <OrganizationForm
          form={editForm}
          legalStructureOptions={legalStructureOptions}
          loadingLegalStructures={legalStructuresQuery.isLoading}
          onFinish={(values) => updateMutation.mutate(values)}
        />
      </Modal>
    </div>
  );
}

type OrganizationFormProps = {
  form: ReturnType<typeof Form.useForm<FormValues>>[0];
  legalStructureOptions: Array<{ value: string; label: string }>;
  loadingLegalStructures: boolean;
  onFinish: (values: FormValues) => void;
};

function OrganizationForm({
  form,
  legalStructureOptions,
  loadingLegalStructures,
  onFinish
}: OrganizationFormProps) {
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
