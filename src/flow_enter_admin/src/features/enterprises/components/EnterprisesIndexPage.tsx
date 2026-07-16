import { flexRender, type Table } from "@tanstack/react-table";
import type { UseQueryResult } from "@tanstack/react-query";
import { Alert, Button, Card, Space, Spin, Typography } from "antd";
import type { Enterprise, EnterprisesResponse } from "../types";

const { Title, Text } = Typography;

type EnterprisesIndexPageProps = {
  enterprisesQuery: UseQueryResult<EnterprisesResponse, unknown>;
  table: Table<Enterprise>;
  columnsLength: number;
  totalCount: number;
  pageIndex: number;
  pageCount: number;
  pageSize: number;
  onCreate: () => void;
  onSetPageSize: (size: number) => void;
  onPrevPage: () => void;
  onNextPage: () => void;
};

export function EnterprisesIndexPage({
  enterprisesQuery,
  table,
  columnsLength,
  totalCount,
  pageIndex,
  pageCount,
  pageSize,
  onCreate,
  onSetPageSize,
  onPrevPage,
  onNextPage
}: EnterprisesIndexPageProps) {
  return (
    <Card>
      <Space direction="vertical" size="middle" style={{ width: "100%" }}>
        <Space style={{ width: "100%", justifyContent: "space-between" }}>
          <div>
            <Title level={3} style={{ margin: 0 }}>
              Enterprises
            </Title>
            <Text type="secondary">Manage enterprise enterprises from Flow Enter backend.</Text>
          </div>
          <Button type="primary" onClick={onCreate}>
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
                    <td colSpan={columnsLength}>
                      <div className="table-loading">
                        <Spin />
                      </div>
                    </td>
                  </tr>
                ) : table.getRowModel().rows.length === 0 ? (
                  <tr>
                    <td colSpan={columnsLength}>No enterprises found.</td>
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
            <Button onClick={() => onSetPageSize(10)} disabled={pageSize === 10}>
              10 rows
            </Button>
            <Button onClick={() => onSetPageSize(25)} disabled={pageSize === 25}>
              25 rows
            </Button>
            <Button onClick={onPrevPage} disabled={pageIndex <= 0}>
              Prev
            </Button>
            <Button onClick={onNextPage} disabled={pageIndex >= pageCount - 1}>
              Next
            </Button>
          </Space>
        </Space>
      </Space>
    </Card>
  );
}
