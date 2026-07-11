import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ColumnDef, flexRender, getCoreRowModel, useReactTable } from "@tanstack/react-table";
import { Alert, Button, Card, Form, Input, Modal, Space, Spin, Typography, message } from "antd";
import { useMemo, useState } from "react";
import { createCountry, fetchCountries, updateCountry } from "../../api/countries";
import { Country, CreateCountryRequest } from "./types";

const { Title, Text } = Typography;

type CountriesPageProps = {
  apiBaseUrl?: string;
};

type FormValues = CreateCountryRequest;

const defaultPageSize = 10;

export function CountriesPage({ apiBaseUrl }: CountriesPageProps) {
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(defaultPageSize);
  const [isCreateOpen, setCreateOpen] = useState(false);
  const [editingCountry, setEditingCountry] = useState<Country | null>(null);
  const [createForm] = Form.useForm<FormValues>();
  const [editForm] = Form.useForm<FormValues>();

  const countriesQuery = useQuery({
    queryKey: ["countries", pageIndex, pageSize, apiBaseUrl],
    queryFn: () => fetchCountries(pageIndex + 1, pageSize, apiBaseUrl)
  });

  const createMutation = useMutation({
    mutationFn: (values: FormValues) => createCountry(values, apiBaseUrl),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["countries"] });
      setCreateOpen(false);
      createForm.resetFields();
      messageApi.success("Country created");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create failed");
    }
  });

  const updateMutation = useMutation({
    mutationFn: async (values: FormValues) => {
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
      await queryClient.invalidateQueries({ queryKey: ["countries"] });
      setEditingCountry(null);
      editForm.resetFields();
      messageApi.success("Country updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update failed");
    }
  });

  const columns = useMemo<ColumnDef<Country>[]>(
    () => [
      {
        accessorKey: "name",
        header: "Country",
        cell: (info) => info.getValue<string>()
      },
      {
        accessorKey: "isoCode2",
        header: "ISO Code 2",
        cell: (info) => info.getValue<string>()
      },
      {
        accessorKey: "isoCode3",
        header: "ISO Code 3",
        cell: (info) => info.getValue<string>()
      },
      {
        id: "actions",
        header: "Actions",
        cell: ({ row }) => (
          <Button
            size="small"
            onClick={() => {
              setEditingCountry(row.original);
              editForm.setFieldsValue({
                name: row.original.name,
                isoCode2: row.original.isoCode2,
                isoCode3: row.original.isoCode3
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

  const countries = countriesQuery.data?.data ?? [];
  const totalCount = countriesQuery.data?.totalCount ?? 0;
  const pageCount = Math.max(1, Math.ceil(totalCount / pageSize));

  const table = useReactTable({
    data: countries,
    columns,
    pageCount,
    state: {
      pagination: { pageIndex, pageSize }
    },
    manualPagination: true,
    getCoreRowModel: getCoreRowModel()
  });

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
              <Text type="secondary">Manage countries from Flow Enter backend.</Text>
            </div>
            <Button type="primary" onClick={() => setCreateOpen(true)}>
              New Country
            </Button>
          </Space>

          {countriesQuery.isError ? (
            <Alert
              type="error"
              message="Failed to load countries"
              description={countriesQuery.error instanceof Error ? countriesQuery.error.message : "Unknown error"}
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
                  {countriesQuery.isLoading ? (
                    <tr>
                      <td colSpan={columns.length}>
                        <div className="table-loading">
                          <Spin />
                        </div>
                      </td>
                    </tr>
                  ) : table.getRowModel().rows.length === 0 ? (
                    <tr>
                      <td colSpan={columns.length}>No countries found.</td>
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
              Total {totalCount} countries • Page {pageIndex + 1} / {pageCount}
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
        title="Create Country"
        onCancel={() => setCreateOpen(false)}
        onOk={() => createForm.submit()}
        confirmLoading={createMutation.isPending}
        destroyOnClose
      >
        <CountryForm
          form={createForm}
          onFinish={(values) => createMutation.mutate(values)}
        />
      </Modal>

      <Modal
        open={Boolean(editingCountry)}
        title="Edit Country"
        onCancel={() => {
          setEditingCountry(null);
          editForm.resetFields();
        }}
        onOk={() => editForm.submit()}
        confirmLoading={updateMutation.isPending}
        destroyOnClose
      >
        <CountryForm
          form={editForm}
          onFinish={(values) => updateMutation.mutate(values)}
        />
      </Modal>
    </div>
  );
}

type CountryFormProps = {
  form: ReturnType<typeof Form.useForm<FormValues>>[0];
  onFinish: (values: FormValues) => void;
};

function CountryForm({ form, onFinish }: CountryFormProps) {
  return (
    <Form<FormValues> form={form} layout="vertical" onFinish={onFinish}>
      <Form.Item name="name" label="Country Name" rules={[{ required: true, message: "Country name is required" }]}>
        <Input maxLength={200} />
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
