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
  Empty,
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
import { createEnperprise, fetchLegalStructures, fetchEnterprises, updateEnperprise } from "../../api/enterprises";
import { CreateEnterpriseRequest, Enterprise } from "./types";

const { Title, Text } = Typography;

type EnterprisesPageProps = {
  apiBaseUrl?: string;
};

type FormValues = CreateEnterpriseRequest;

const defaultPageSize = 10;

export function EnterprisesPage({ apiBaseUrl }: EnterprisesPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(defaultPageSize);
  const [isCreateOpen, setCreateOpen] = useState(false);
  const [editingEnterprise, setEditingEnterprise] = useState<Enterprise | null>(null);
  const [peopleEnterprise, setPeopleEnterprise] = useState<Enterprise | null>(null);
  const [createForm] = Form.useForm<FormValues>();
  const [editForm] = Form.useForm<FormValues>();

  const enterprisesQuery = useQuery({
    queryKey: ["enterprises", pageIndex, pageSize, apiBaseUrl],
    queryFn: () => fetchEnterprises(pageIndex + 1, pageSize, apiBaseUrl)
  });

  const legalStructuresQuery = useQuery({
    queryKey: ["enterprises/legal-structures", apiBaseUrl],
    queryFn: () => fetchLegalStructures(apiBaseUrl)
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

  if (peopleEnterprise) {
    return (
      <div className="page-container">
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
              <Button onClick={() => setPeopleEnterprise(null)}>Back to Enterprises</Button>
            </Space>
            <Empty description="People management UI for this enterprise will be added here." />
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

      <Modal
        open={isCreateOpen}
        title="Create Enterprise"
        onCancel={() => setCreateOpen(false)}
        onOk={() => createForm.submit()}
        confirmLoading={createMutation.isPending}
        destroyOnClose
      >
        <EnterpriseForm
          form={createForm}
          legalStructureOptions={legalStructureOptions}
          loadingLegalStructures={legalStructuresQuery.isLoading}
          onFinish={(values) => createMutation.mutate(values)}
        />
      </Modal>

      <Modal
        open={Boolean(editingEnterprise)}
        title="Edit Enterprise"
        onCancel={() => {
          setEditingEnterprise(null);
          editForm.resetFields();
        }}
        onOk={() => editForm.submit()}
        confirmLoading={updateMutation.isPending}
        destroyOnClose
      >
        <EnterpriseForm
          form={editForm}
          legalStructureOptions={legalStructureOptions}
          loadingLegalStructures={legalStructuresQuery.isLoading}
          onFinish={(values) => updateMutation.mutate(values)}
        />
      </Modal>
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
