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
  createEnterpriseBed,
  createEnperprise,
  createEnterpriseRoom,
  deleteEnterpriseBed,
  deleteEnterprise,
  deleteEnterpriseRoom,
  deleteEnterpriseEmployment,
  createEnterpriseEmployment,
  fetchEnterpriseBeds,
  fetchEnterpriseEmployments,
  fetchEnterprises,
  fetchLegalStructures,
  fetchPartyRoleTypes,
  fetchEnterpriseRooms,
  updateEnterpriseBed,
  updateEnterpriseEmploymentEffectiveDate,
  updateEnterpriseEmployment,
  updateEnterpriseRoom,
  updateEnperprise
} from "../../api/enterprises";
import { TopDrawerForm } from "../../components/TopDrawerForm";
import { Bed, CreateBedRequest, CreateEmploymentRequest, CreateEnterpriseRequest, CreateRoomRequest, Enterprise, Employment, Room } from "./types";

const { Title, Text } = Typography;

type EnterprisesPageProps = {
  apiBaseUrl?: string;
};

type FormValues = CreateEnterpriseRequest;
type EmploymentFormValues = CreateEmploymentRequest;
type RoomFormValues = CreateRoomRequest;
type BedFormValues = CreateBedRequest;
type EmploymentGroup = {
  employeePartyId: string;
  employeeFullName: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  employments: Employment[];
};

function toLocalDatePickerValue(value: string) {
  if (value.includes("T")) {
    return dayjs(new Date(value));
  }

  return dayjs(new Date(`${value}T00:00:00`));
}

function toLocalDateApiValue(value: string) {
  return toLocalDatePickerValue(value).format("YYYY-MM-DD");
}

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
  const [roomSearchText, setRoomSearchText] = useState("");
  const [isCreateRoomOpen, setCreateRoomOpen] = useState(false);
  const [editingRoom, setEditingRoom] = useState<Room | null>(null);
  const [bedSearchText, setBedSearchText] = useState("");
  const [isCreateBedOpen, setCreateBedOpen] = useState(false);
  const [editingBed, setEditingBed] = useState<Bed | null>(null);
  const [effectiveDateDrafts, setEffectiveDateDrafts] = useState<
    Record<string, { fromDate: string; thruDate: string }>
  >({});
  const [peopleTabKey, setPeopleTabKey] = useState("people");
  const [createForm] = Form.useForm<FormValues>();
  const [editForm] = Form.useForm<FormValues>();
  const [employmentForm] = Form.useForm<EmploymentFormValues>();
  const [editEmploymentForm] = Form.useForm<EmploymentFormValues>();
  const [createRoomForm] = Form.useForm<RoomFormValues>();
  const [editRoomForm] = Form.useForm<RoomFormValues>();
  const [createBedForm] = Form.useForm<BedFormValues>();
  const [editBedForm] = Form.useForm<BedFormValues>();

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

  const roomsQuery = useQuery({
    queryKey: ["enterprise-rooms", peopleEnterprise?.enterpriseId, roomSearchText, apiBaseUrl],
    queryFn: () => fetchEnterpriseRooms(peopleEnterprise!.enterpriseId, roomSearchText, apiBaseUrl),
    enabled: Boolean(peopleEnterprise)
  });

  const roomOptionsQuery = useQuery({
    queryKey: ["enterprise-room-options", peopleEnterprise?.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseRooms(peopleEnterprise!.enterpriseId, "", apiBaseUrl),
    enabled: Boolean(peopleEnterprise)
  });

  const bedsQuery = useQuery({
    queryKey: ["enterprise-beds", peopleEnterprise?.enterpriseId, bedSearchText, apiBaseUrl],
    queryFn: () => fetchEnterpriseBeds(peopleEnterprise!.enterpriseId, bedSearchText, apiBaseUrl),
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

  const deleteEnterpriseMutation = useMutation({
    mutationFn: async (enterpriseId: string) => {
      await deleteEnterprise(enterpriseId, apiBaseUrl);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["enterprises"] });
      messageApi.success("Enterprise deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete enterprise failed");
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

  const createRoomMutation = useMutation({
    mutationFn: async (values: RoomFormValues) => {
      if (!peopleEnterprise) {
        return;
      }

      await createEnterpriseRoom(peopleEnterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-rooms", peopleEnterprise.enterpriseId, roomSearchText, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setCreateRoomOpen(false);
      createRoomForm.resetFields();
      messageApi.success("Room added");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create room failed");
    }
  });

  const updateRoomMutation = useMutation({
    mutationFn: async (values: RoomFormValues) => {
      if (!peopleEnterprise || !editingRoom) {
        return;
      }

      await updateEnterpriseRoom(peopleEnterprise.enterpriseId, editingRoom.roomId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-rooms", peopleEnterprise.enterpriseId, roomSearchText, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-beds", peopleEnterprise.enterpriseId, bedSearchText, apiBaseUrl]
      });
      setEditingRoom(null);
      editRoomForm.resetFields();
      messageApi.success("Room updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update room failed");
    }
  });

  const deleteRoomMutation = useMutation({
    mutationFn: async (roomId: string) => {
      if (!peopleEnterprise) {
        return;
      }

      await deleteEnterpriseRoom(peopleEnterprise.enterpriseId, roomId, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-rooms", peopleEnterprise.enterpriseId, roomSearchText, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-beds", peopleEnterprise.enterpriseId, bedSearchText, apiBaseUrl]
      });
      messageApi.success("Room deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete room failed");
    }
  });

  const createBedMutation = useMutation({
    mutationFn: async (values: BedFormValues) => {
      if (!peopleEnterprise) {
        return;
      }

      await createEnterpriseBed(peopleEnterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-beds", peopleEnterprise.enterpriseId, bedSearchText, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-rooms", peopleEnterprise.enterpriseId, roomSearchText, apiBaseUrl]
      });
      setCreateBedOpen(false);
      createBedForm.resetFields();
      messageApi.success("Bed added");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create bed failed");
    }
  });

  const updateBedMutation = useMutation({
    mutationFn: async (values: BedFormValues) => {
      if (!peopleEnterprise || !editingBed) {
        return;
      }

      await updateEnterpriseBed(peopleEnterprise.enterpriseId, editingBed.bedId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-beds", peopleEnterprise.enterpriseId, bedSearchText, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-rooms", peopleEnterprise.enterpriseId, roomSearchText, apiBaseUrl]
      });
      setEditingBed(null);
      editBedForm.resetFields();
      messageApi.success("Bed updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update bed failed");
    }
  });

  const deleteBedMutation = useMutation({
    mutationFn: async (bedId: string) => {
      if (!peopleEnterprise) {
        return;
      }

      await deleteEnterpriseBed(peopleEnterprise.enterpriseId, bedId, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-beds", peopleEnterprise.enterpriseId, bedSearchText, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-rooms", peopleEnterprise.enterpriseId, roomSearchText, apiBaseUrl]
      });
      messageApi.success("Bed deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete bed failed");
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
            <Popconfirm
              title="Delete enterprise?"
              description="This will remove enterprise and all related data."
              okText="Delete"
              okButtonProps={{ danger: true, loading: deleteEnterpriseMutation.isPending }}
              onConfirm={() => deleteEnterpriseMutation.mutate(row.original.enterpriseId)}
            >
              <Button size="small" danger>
                Delete
              </Button>
            </Popconfirm>
          </Space>
        )
      }
    ],
    [deleteEnterpriseMutation, editForm]
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
  const rooms = roomsQuery.data ?? [];
  const roomOptions = (roomOptionsQuery.data ?? []).map((item) => ({
    value: item.roomId,
    label: item.number
  }));
  const beds = bedsQuery.data ?? [];
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
      fromDate: toLocalDateApiValue(employment.fromDate),
      thruDate: toLocalDateApiValue(employment.thruDate)
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
                  setEditingRoom(null);
                  setEditingBed(null);
                  setCreateEmploymentOpen(false);
                  setCreateRoomOpen(false);
                  setCreateBedOpen(false);
                  setRoomSearchText("");
                  setBedSearchText("");
                  setEffectiveDateDrafts({});
                  setPeopleTabKey("people");
                  employmentForm.resetFields();
                  editEmploymentForm.resetFields();
                  createRoomForm.resetFields();
                  editRoomForm.resetFields();
                  createBedForm.resetFields();
                  editBedForm.resetFields();
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
                                                        value={toLocalDatePickerValue(draft.fromDate)}
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
                                                        value={toLocalDatePickerValue(draft.thruDate)}
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
                },
                {
                  key: "facilities",
                  label: "Facilities",
                  children: (
                    <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                      <Card size="small" title="Rooms">
                        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                          <Space style={{ width: "100%", justifyContent: "space-between" }}>
                            <Input.Search
                              allowClear
                              placeholder="Search room number"
                              value={roomSearchText}
                              onChange={(event) => setRoomSearchText(event.target.value)}
                              style={{ maxWidth: 320 }}
                            />
                            <Button type="primary" onClick={() => setCreateRoomOpen(true)}>
                              Add Room
                            </Button>
                          </Space>

                          {roomsQuery.isError ? (
                            <Alert
                              type="error"
                              message="Failed to load rooms"
                              description={roomsQuery.error instanceof Error ? roomsQuery.error.message : "Unknown error"}
                              showIcon
                            />
                          ) : roomsQuery.isLoading ? (
                            <Spin />
                          ) : rooms.length === 0 ? (
                            <Empty description="No rooms found." />
                          ) : (
                            <div className="tanstack-table-wrapper">
                              <table className="tanstack-table">
                                <thead>
                                  <tr>
                                    <th>Room Number</th>
                                    <th>Description</th>
                                    <th>Beds</th>
                                    <th>Actions</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  {rooms.map((room) => (
                                    <tr key={room.roomId}>
                                      <td>{room.number}</td>
                                      <td>{room.description || "-"}</td>
                                      <td>{room.bedCount}</td>
                                      <td>
                                        <Space>
                                          <Button
                                            size="small"
                                            onClick={() => {
                                              setEditingRoom(room);
                                              editRoomForm.setFieldsValue({ number: room.number, description: room.description });
                                            }}
                                          >
                                            Edit
                                          </Button>
                                          <Popconfirm
                                            title="Delete room?"
                                            description="This removes the room from this enterprise."
                                            okText="Delete"
                                            okButtonProps={{ danger: true, loading: deleteRoomMutation.isPending }}
                                            onConfirm={() => deleteRoomMutation.mutate(room.roomId)}
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
                      </Card>

                      <Card size="small" title="Beds">
                        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                          <Space style={{ width: "100%", justifyContent: "space-between" }}>
                            <Input.Search
                              allowClear
                              placeholder="Search bed or room number"
                              value={bedSearchText}
                              onChange={(event) => setBedSearchText(event.target.value)}
                              style={{ maxWidth: 320 }}
                            />
                            <Button type="primary" onClick={() => setCreateBedOpen(true)}>
                              Add Bed
                            </Button>
                          </Space>

                          {bedsQuery.isError ? (
                            <Alert
                              type="error"
                              message="Failed to load beds"
                              description={bedsQuery.error instanceof Error ? bedsQuery.error.message : "Unknown error"}
                              showIcon
                            />
                          ) : bedsQuery.isLoading ? (
                            <Spin />
                          ) : beds.length === 0 ? (
                            <Empty description="No beds found." />
                          ) : (
                            <div className="tanstack-table-wrapper">
                              <table className="tanstack-table">
                                <thead>
                                  <tr>
                                    <th>Bed Number</th>
                                    <th>Description</th>
                                    <th>Room</th>
                                    <th>Actions</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  {beds.map((bed) => (
                                    <tr key={bed.bedId}>
                                      <td>{bed.number}</td>
                                      <td>{bed.description || "-"}</td>
                                      <td>{bed.roomNumber}</td>
                                      <td>
                                        <Space>
                                          <Button
                                            size="small"
                                            onClick={() => {
                                              setEditingBed(bed);
                                              editBedForm.setFieldsValue({
                                                number: bed.number,
                                                roomId: bed.roomId,
                                                description: bed.description
                                              });
                                            }}
                                          >
                                            Edit
                                          </Button>
                                          <Popconfirm
                                            title="Delete bed?"
                                            description="This removes the bed from this enterprise."
                                            okText="Delete"
                                            okButtonProps={{ danger: true, loading: deleteBedMutation.isPending }}
                                            onConfirm={() => deleteBedMutation.mutate(bed.bedId)}
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
                      </Card>
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

            <TopDrawerForm
              open={isCreateRoomOpen}
              title="Add Room"
              submitText="Add"
              onClose={() => {
                setCreateRoomOpen(false);
                createRoomForm.resetFields();
              }}
              onSubmit={() => createRoomForm.submit()}
              loading={createRoomMutation.isPending}
            >
              <Form<RoomFormValues> form={createRoomForm} layout="vertical" onFinish={(values) => createRoomMutation.mutate(values)}>
                <Form.Item
                  name="number"
                  label="Room Number"
                  rules={[{ required: true, message: "Room number is required" }]}
                >
                  <Input maxLength={100} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={Boolean(editingRoom)}
              title="Edit Room"
              submitText="Update"
              onClose={() => {
                setEditingRoom(null);
                editRoomForm.resetFields();
              }}
              onSubmit={() => editRoomForm.submit()}
              loading={updateRoomMutation.isPending}
            >
              <Form<RoomFormValues> form={editRoomForm} layout="vertical" onFinish={(values) => updateRoomMutation.mutate(values)}>
                <Form.Item
                  name="number"
                  label="Room Number"
                  rules={[{ required: true, message: "Room number is required" }]}
                >
                  <Input maxLength={100} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={isCreateBedOpen}
              title="Add Bed"
              submitText="Add"
              onClose={() => {
                setCreateBedOpen(false);
                createBedForm.resetFields();
              }}
              onSubmit={() => createBedForm.submit()}
              loading={createBedMutation.isPending}
            >
              <Form<BedFormValues> form={createBedForm} layout="vertical" onFinish={(values) => createBedMutation.mutate(values)}>
                <Form.Item
                  name="number"
                  label="Bed Number"
                  rules={[{ required: true, message: "Bed number is required" }]}
                >
                  <Input maxLength={100} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
                <Form.Item
                  name="roomId"
                  label="Room"
                  rules={[{ required: true, message: "Room is required" }]}
                >
                  <Select
                    showSearch
                    options={roomOptions}
                    loading={roomOptionsQuery.isLoading}
                    placeholder="Select room"
                    optionFilterProp="label"
                  />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={Boolean(editingBed)}
              title="Edit Bed"
              submitText="Update"
              onClose={() => {
                setEditingBed(null);
                editBedForm.resetFields();
              }}
              onSubmit={() => editBedForm.submit()}
              loading={updateBedMutation.isPending}
            >
              <Form<BedFormValues> form={editBedForm} layout="vertical" onFinish={(values) => updateBedMutation.mutate(values)}>
                <Form.Item
                  name="number"
                  label="Bed Number"
                  rules={[{ required: true, message: "Bed number is required" }]}
                >
                  <Input maxLength={100} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
                <Form.Item
                  name="roomId"
                  label="Room"
                  rules={[{ required: true, message: "Room is required" }]}
                >
                  <Select
                    showSearch
                    options={roomOptions}
                    loading={roomOptionsQuery.isLoading}
                    placeholder="Select room"
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
