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
  Layout,
  Menu,
  Popover,
  Select,
  Space,
  Spin,
  Tabs,
  Popconfirm,
  Typography,
  Tree,
  message
} from "antd";
import { AppstoreOutlined, ToolOutlined, GiftOutlined } from "@ant-design/icons";
import type { TreeDataNode } from "antd";
import dayjs from "dayjs";
import { useMemo, useState } from "react";
import {
  createEnterpriseBed,
  createEnterpriseBranch,
  createEnterpriseBuilding,
  createEnterpriseFloor,
  createEnperprise,
  createEnterpriseRoom,
  deleteEnterpriseBed,
  deleteEnterpriseBuilding,
  deleteEnterprise,
  deleteEnterpriseBranch,
  deleteEnterpriseFloor,
  deleteEnterpriseRoom,
  deleteEnterpriseEmployment,
  createEnterpriseEmployment,
  fetchEnterpriseFacilitiesTree,
  fetchEnterpriseBranchs,
  fetchEnterpriseEmployments,
  fetchEnterprises,
  fetchLegalStructures,
  fetchPartyRoleTypes,
  fetchEnterpriseRooms,
  updateEnterpriseBed,
  updateEnterpriseBranch,
  updateEnterpriseBuilding,
  updateEnterpriseEmploymentBranchEffectiveDate,
  updateEnterpriseEmploymentEffectiveDate,
  updateEnterpriseEmployment,
  updateEnterpriseFloor,
  updateEnterpriseRoom,
  updateEnperprise
} from "../../api/enterprises";
import { TopDrawerForm } from "../../components/TopDrawerForm";
import {
  Bed,
  CreateEnterpriseBranchRequest,
  Building,
  CreateBedRequest,
  CreateBuildingRequest,
  CreateEmploymentRequest,
  CreateEnterpriseRequest,
  CreateFloorRequest,
  CreateRoomRequest,
  Enterprise,
  EnterpriseBranch,
  Employment,
  Floor,
  Room
} from "./types";

const { Title, Text } = Typography;

type EnterprisesPageProps = {
  apiBaseUrl?: string;
};

type FormValues = CreateEnterpriseRequest;
type EmploymentFormValues = CreateEmploymentRequest;
type EnterpriseBranchFormValues = CreateEnterpriseBranchRequest;
type BuildingFormValues = CreateBuildingRequest;
type FloorFormValues = CreateFloorRequest;
type RoomFormValues = CreateRoomRequest;
type BedFormValues = CreateBedRequest;
type EmploymentGroup = {
  employeePartyId: string;
  employmentNumber: string;
  branchIds: string[];
  branchLegalNames: string[];
  branchEmployments: Employment["branchEmployments"];
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
  const [productsEnterprise, setProductsEnterprise] = useState<Enterprise | null>(null);
  const [editingEmployment, setEditingEmployment] = useState<Employment | null>(null);
  const [isCreateEmploymentOpen, setCreateEmploymentOpen] = useState(false);
  const [isCreateEnterpriseBranchOpen, setCreateEnterpriseBranchOpen] = useState(false);
  const [editingEnterpriseBranch, setEditingEnterpriseBranch] = useState<EnterpriseBranch | null>(null);
  const [isCreateBuildingOpen, setCreateBuildingOpen] = useState(false);
  const [editingBuilding, setEditingBuilding] = useState<Building | null>(null);
  const [isCreateFloorOpen, setCreateFloorOpen] = useState(false);
  const [activeBuildingIdForFloorCreate, setActiveBuildingIdForFloorCreate] = useState<string | null>(null);
  const [editingFloor, setEditingFloor] = useState<Floor | null>(null);
  const [isCreateRoomOpen, setCreateRoomOpen] = useState(false);
  const [activeFloorIdForRoomCreate, setActiveFloorIdForRoomCreate] = useState<string | null>(null);
  const [editingRoom, setEditingRoom] = useState<Room | null>(null);
  const [isCreateBedOpen, setCreateBedOpen] = useState(false);
  const [activeRoomIdForBedCreate, setActiveRoomIdForBedCreate] = useState<string | null>(null);
  const [editingBed, setEditingBed] = useState<Bed | null>(null);
  const [effectiveDateDrafts, setEffectiveDateDrafts] = useState<
    Record<string, { fromDate: string; thruDate: string }>
  >({});
  const [branchEffectiveDateDrafts, setBranchEffectiveDateDrafts] = useState<
    Record<string, { fromDate: string; thruDate: string }>
  >({});
  const [peopleTabKey, setPeopleTabKey] = useState("people");
  const [productsTabKey, setProductsTabKey] = useState("manage-products");
  const [productManagementTabKey, setProductManagementTabKey] = useState("services");
  const [createForm] = Form.useForm<FormValues>();
  const [editForm] = Form.useForm<FormValues>();
  const [employmentForm] = Form.useForm<EmploymentFormValues>();
  const [editEmploymentForm] = Form.useForm<EmploymentFormValues>();
  const [enterpriseBranchForm] = Form.useForm<EnterpriseBranchFormValues>();
  const [editEnterpriseBranchForm] = Form.useForm<EnterpriseBranchFormValues>();
  const [createBuildingForm] = Form.useForm<BuildingFormValues>();
  const [editBuildingForm] = Form.useForm<BuildingFormValues>();
  const [createFloorForm] = Form.useForm<FloorFormValues>();
  const [editFloorForm] = Form.useForm<FloorFormValues>();
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

  const enterpriseBranchsQuery = useQuery({
    queryKey: ["enterprise-branchs", peopleEnterprise?.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseBranchs(peopleEnterprise!.enterpriseId, apiBaseUrl),
    enabled: Boolean(peopleEnterprise)
  });

  const partyRoleTypesQuery = useQuery({
    queryKey: ["party-role-types", apiBaseUrl],
    queryFn: () => fetchPartyRoleTypes(apiBaseUrl),
    enabled: Boolean(peopleEnterprise)
  });

  const facilitiesTreeQuery = useQuery({
    queryKey: ["enterprise-facilities-tree", peopleEnterprise?.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseFacilitiesTree(peopleEnterprise!.enterpriseId, apiBaseUrl),
    enabled: Boolean(peopleEnterprise)
  });

  const roomOptionsQuery = useQuery({
    queryKey: ["enterprise-room-options", peopleEnterprise?.enterpriseId, apiBaseUrl],
    queryFn: () => fetchEnterpriseRooms(peopleEnterprise!.enterpriseId, "", apiBaseUrl),
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

  const updateEmploymentBranchEffectiveDateMutation = useMutation({
    mutationFn: async (payload: { employmentId: string; branchId: string; fromDate: string; thruDate: string }) => {
      if (!peopleEnterprise) {
        return;
      }

      await updateEnterpriseEmploymentBranchEffectiveDate(
        peopleEnterprise.enterpriseId,
        payload.employmentId,
        payload.branchId,
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
      messageApi.success("Branch effective date updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update branch effective date failed");
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

  const createEnterpriseBranchMutation = useMutation({
    mutationFn: async (values: EnterpriseBranchFormValues) => {
      if (!peopleEnterprise) {
        return;
      }

      await createEnterpriseBranch(peopleEnterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-branchs", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setCreateEnterpriseBranchOpen(false);
      enterpriseBranchForm.resetFields();
      messageApi.success("Enterprise branch added");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create enterprise branch failed");
    }
  });

  const updateEnterpriseBranchMutation = useMutation({
    mutationFn: async (values: EnterpriseBranchFormValues) => {
      if (!peopleEnterprise || !editingEnterpriseBranch) {
        return;
      }

      await updateEnterpriseBranch(peopleEnterprise.enterpriseId, editingEnterpriseBranch.enterpriseBranchId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-branchs", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setEditingEnterpriseBranch(null);
      editEnterpriseBranchForm.resetFields();
      messageApi.success("Enterprise branch updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update enterprise branch failed");
    }
  });

  const deleteEnterpriseBranchMutation = useMutation({
    mutationFn: async (enterpriseBranchId: string) => {
      if (!peopleEnterprise) {
        return;
      }

      await deleteEnterpriseBranch(peopleEnterprise.enterpriseId, enterpriseBranchId, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-branchs", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Enterprise branch deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete enterprise branch failed");
    }
  });

  const createBuildingMutation = useMutation({
    mutationFn: async (values: BuildingFormValues) => {
      if (!peopleEnterprise) {
        return;
      }

      await createEnterpriseBuilding(peopleEnterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setCreateBuildingOpen(false);
      createBuildingForm.resetFields();
      messageApi.success("Building added");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create building failed");
    }
  });

  const updateBuildingMutation = useMutation({
    mutationFn: async (values: BuildingFormValues) => {
      if (!peopleEnterprise || !editingBuilding) {
        return;
      }

      await updateEnterpriseBuilding(peopleEnterprise.enterpriseId, editingBuilding.buildingId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setEditingBuilding(null);
      editBuildingForm.resetFields();
      messageApi.success("Building updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update building failed");
    }
  });

  const deleteBuildingMutation = useMutation({
    mutationFn: async (buildingId: string) => {
      if (!peopleEnterprise) {
        return;
      }

      await deleteEnterpriseBuilding(peopleEnterprise.enterpriseId, buildingId, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Building deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete building failed");
    }
  });

  const createFloorMutation = useMutation({
    mutationFn: async (values: FloorFormValues) => {
      if (!peopleEnterprise) {
        return;
      }

      await createEnterpriseFloor(peopleEnterprise.enterpriseId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setCreateFloorOpen(false);
      setActiveBuildingIdForFloorCreate(null);
      createFloorForm.resetFields();
      messageApi.success("Floor added");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Create floor failed");
    }
  });

  const updateFloorMutation = useMutation({
    mutationFn: async (values: FloorFormValues) => {
      if (!peopleEnterprise || !editingFloor) {
        return;
      }

      await updateEnterpriseFloor(peopleEnterprise.enterpriseId, editingFloor.floorId, values, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setEditingFloor(null);
      editFloorForm.resetFields();
      messageApi.success("Floor updated");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Update floor failed");
    }
  });

  const deleteFloorMutation = useMutation({
    mutationFn: async (floorId: string) => {
      if (!peopleEnterprise) {
        return;
      }

      await deleteEnterpriseFloor(peopleEnterprise.enterpriseId, floorId, apiBaseUrl);
    },
    onSuccess: async () => {
      if (!peopleEnterprise) {
        return;
      }

      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      messageApi.success("Floor deleted");
    },
    onError: (error) => {
      messageApi.error(error instanceof Error ? error.message : "Delete floor failed");
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
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setCreateRoomOpen(false);
      setActiveFloorIdForRoomCreate(null);
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
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
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
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
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
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      setCreateBedOpen(false);
      setActiveRoomIdForBedCreate(null);
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
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
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
        queryKey: ["enterprise-room-options", peopleEnterprise.enterpriseId, apiBaseUrl]
      });
      await queryClient.invalidateQueries({
        queryKey: ["enterprise-facilities-tree", peopleEnterprise.enterpriseId, apiBaseUrl]
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
            <Button
              size="small"
              onClick={() => {
                setProductsEnterprise(null);
                setPeopleEnterprise(row.original);
              }}
            >
              People & Organization
            </Button>
            <Button
              size="small"
              onClick={() => {
                setPeopleEnterprise(null);
                setProductsEnterprise(row.original);
                setProductsTabKey("manage-products");
                setProductManagementTabKey("services");
              }}
            >
              Products
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
  const enterpriseBranchs = enterpriseBranchsQuery.data ?? [];
  const enterpriseBranchOptions = enterpriseBranchs.map((item) => ({
    value: item.branchId,
    label: item.branchLegalName
  }));
  const facilitiesTree = facilitiesTreeQuery.data?.buildings ?? [];
  const buildingOptions = facilitiesTree.map((node) => ({
    value: node.building.buildingId,
    label: node.building.name
  }));
  const floorOptions = facilitiesTree.flatMap((buildingNode) =>
    buildingNode.floors.map((floorNode) => ({
      value: floorNode.floor.floorId,
      label: `${buildingNode.building.name} / Floor ${floorNode.floor.level}`
    }))
  );
  const roomOptions = (roomOptionsQuery.data ?? []).map((item) => ({
    value: item.roomId,
    label: `${item.buildingName} / Floor ${item.floorLevel} / Room ${item.number}`
  }));
  const facilitiesTreeData = useMemo<TreeDataNode[]>(
    () =>
      facilitiesTree.map((buildingNode) => ({
        key: `building-${buildingNode.building.buildingId}`,
        title: (
          <Space style={{ width: "100%", justifyContent: "space-between" }}>
            <span>
              <strong>{buildingNode.building.name}</strong>
              {buildingNode.building.description ? ` - ${buildingNode.building.description}` : ""}
              {buildingNode.building.branchLegalNames.length > 0
                ? ` [${buildingNode.building.branchLegalNames.join(", ")}]`
                : ""}
            </span>
            <Space>
              <Button
                size="small"
                onClick={(event) => {
                  event.stopPropagation();
                  setEditingBuilding(buildingNode.building);
                  editBuildingForm.setFieldsValue({
                    name: buildingNode.building.name,
                    description: buildingNode.building.description,
                    branchIds: buildingNode.building.branchIds
                  });
                }}
              >
                Edit
              </Button>
              <Button
                size="small"
                type="primary"
                onClick={(event) => {
                  event.stopPropagation();
                  setActiveBuildingIdForFloorCreate(buildingNode.building.buildingId);
                  createFloorForm.setFieldsValue({ buildingId: buildingNode.building.buildingId });
                  setCreateFloorOpen(true);
                }}
              >
                Add Floor
              </Button>
              <Popconfirm
                title="Delete building?"
                okText="Delete"
                okButtonProps={{ danger: true, loading: deleteBuildingMutation.isPending }}
                onConfirm={(event) => {
                  event?.stopPropagation();
                  deleteBuildingMutation.mutate(buildingNode.building.buildingId);
                }}
              >
                <Button size="small" danger onClick={(event) => event.stopPropagation()}>
                  Delete
                </Button>
              </Popconfirm>
            </Space>
          </Space>
        ),
        children: buildingNode.floors.map((floorNode) => ({
          key: `floor-${floorNode.floor.floorId}`,
          title: (
            <Space style={{ width: "100%", justifyContent: "space-between" }}>
              <span>
                Floor {floorNode.floor.level}
                {floorNode.floor.description ? ` - ${floorNode.floor.description}` : ""}
              </span>
              <Space>
                <Button
                  size="small"
                  onClick={(event) => {
                    event.stopPropagation();
                    setEditingFloor(floorNode.floor);
                    editFloorForm.setFieldsValue({
                      level: floorNode.floor.level,
                      description: floorNode.floor.description,
                      buildingId: floorNode.floor.buildingId
                    });
                  }}
                >
                  Edit
                </Button>
                <Button
                  size="small"
                  type="primary"
                  onClick={(event) => {
                    event.stopPropagation();
                    setActiveFloorIdForRoomCreate(floorNode.floor.floorId);
                    createRoomForm.setFieldsValue({ floorId: floorNode.floor.floorId });
                    setCreateRoomOpen(true);
                  }}
                >
                  Add Room
                </Button>
                <Popconfirm
                  title="Delete floor?"
                  okText="Delete"
                  okButtonProps={{ danger: true, loading: deleteFloorMutation.isPending }}
                  onConfirm={(event) => {
                    event?.stopPropagation();
                    deleteFloorMutation.mutate(floorNode.floor.floorId);
                  }}
                >
                  <Button size="small" danger onClick={(event) => event.stopPropagation()}>
                    Delete
                  </Button>
                </Popconfirm>
              </Space>
            </Space>
          ),
          children: floorNode.rooms.map((roomNode) => ({
            key: `room-${roomNode.room.roomId}`,
            title: (
              <Space style={{ width: "100%", justifyContent: "space-between" }}>
                <span>
                  Room {roomNode.room.number}
                  {roomNode.room.description ? ` - ${roomNode.room.description}` : ""} ({roomNode.room.bedCount} beds)
                </span>
                <Space>
                  <Button
                    size="small"
                    onClick={(event) => {
                      event.stopPropagation();
                      setEditingRoom(roomNode.room);
                      editRoomForm.setFieldsValue({
                        number: roomNode.room.number,
                        description: roomNode.room.description,
                        floorId: roomNode.room.floorId
                      });
                    }}
                  >
                    Edit
                  </Button>
                  <Button
                    size="small"
                    type="primary"
                    onClick={(event) => {
                      event.stopPropagation();
                      setActiveRoomIdForBedCreate(roomNode.room.roomId);
                      createBedForm.setFieldsValue({ roomId: roomNode.room.roomId });
                      setCreateBedOpen(true);
                    }}
                  >
                    Add Bed
                  </Button>
                  <Popconfirm
                    title="Delete room?"
                    okText="Delete"
                    okButtonProps={{ danger: true, loading: deleteRoomMutation.isPending }}
                    onConfirm={(event) => {
                      event?.stopPropagation();
                      deleteRoomMutation.mutate(roomNode.room.roomId);
                    }}
                  >
                    <Button size="small" danger onClick={(event) => event.stopPropagation()}>
                      Delete
                    </Button>
                  </Popconfirm>
                </Space>
              </Space>
            ),
            children: roomNode.beds.map((bed) => ({
              key: `bed-${bed.bedId}`,
              title: (
                <Space style={{ width: "100%", justifyContent: "space-between" }}>
                  <span>
                    Bed {bed.number}
                    {bed.description ? ` - ${bed.description}` : ""}
                  </span>
                  <Space>
                    <Button
                      size="small"
                      onClick={(event) => {
                        event.stopPropagation();
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
                      okText="Delete"
                      okButtonProps={{ danger: true, loading: deleteBedMutation.isPending }}
                      onConfirm={(event) => {
                        event?.stopPropagation();
                        deleteBedMutation.mutate(bed.bedId);
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
      })),
    [
      createBedForm,
      createFloorForm,
      createRoomForm,
      deleteBedMutation.isPending,
      deleteBuildingMutation.isPending,
      deleteFloorMutation.isPending,
      deleteRoomMutation.isPending,
      editBedForm,
      editBuildingForm,
      editFloorForm,
      editRoomForm,
      facilitiesTree
    ]
  );
  const employmentGroups = useMemo<EmploymentGroup[]>(() => {
    const map = new Map<string, EmploymentGroup>();
    for (const employment of employments) {
      const existing = map.get(employment.employeePartyId);
      if (existing) {
        existing.employments.push(employment);
        existing.branchIds = Array.from(new Set([...existing.branchIds, ...(employment.branchIds ?? [])]));
        existing.branchLegalNames = Array.from(new Set([...existing.branchLegalNames, ...(employment.branchLegalNames ?? [])]));
        const branchMap = new Map(existing.branchEmployments.map((item) => [item.branchId, item] as const));
        for (const branchEmployment of employment.branchEmployments ?? []) {
          if (!branchMap.has(branchEmployment.branchId)) {
            branchMap.set(branchEmployment.branchId, branchEmployment);
          }
        }
        existing.branchEmployments = Array.from(branchMap.values()).sort((left, right) =>
          left.branchLegalName.localeCompare(right.branchLegalName)
        );
        continue;
      }

      map.set(employment.employeePartyId, {
        employeePartyId: employment.employeePartyId,
        employmentNumber: employment.employmentNumber,
        branchIds: employment.branchIds ?? [],
        branchLegalNames: employment.branchLegalNames ?? [],
        branchEmployments: [...(employment.branchEmployments ?? [])].sort((left, right) =>
          left.branchLegalName.localeCompare(right.branchLegalName)
        ),
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

  const getBranchEffectiveDateDraft = (employmentGroup: EmploymentGroup, branchId: string) => {
    const key = `${employmentGroup.employeePartyId}:${branchId}`;
    const current = employmentGroup.branchEmployments.find((item) => item.branchId === branchId);
    return (
      branchEffectiveDateDrafts[key] ?? {
        fromDate: current ? toLocalDateApiValue(current.fromDate) : toLocalDateApiValue(new Date().toISOString()),
        thruDate: current ? toLocalDateApiValue(current.thruDate) : toLocalDateApiValue(new Date().toISOString())
      }
    );
  };

  if (peopleEnterprise) {
    return (
      <div className="page-container">
        {contextHolder}
        <Card>
          <Space direction="vertical" size="middle" style={{ width: "100%", minHeight: "calc(100vh - 180px)" }}>
            <Space style={{ width: "100%", justifyContent: "space-between" }}>
              <div>
                <Title level={3} style={{ margin: 0 }}>
                  Enterprise People & Organization
                </Title>
                <Text type="secondary">
                  Manage people and organization under enterprise: <strong>{peopleEnterprise.legalName}</strong>
                </Text>
              </div>
              <Button
                onClick={() => {
                  setPeopleEnterprise(null);
                  setEditingEmployment(null);
                  setEditingEnterpriseBranch(null);
                  setEditingBuilding(null);
                  setEditingFloor(null);
                  setEditingRoom(null);
                  setEditingBed(null);
                  setCreateEmploymentOpen(false);
                  setCreateEnterpriseBranchOpen(false);
                  setCreateBuildingOpen(false);
                  setCreateFloorOpen(false);
                  setCreateRoomOpen(false);
                  setCreateBedOpen(false);
                  setActiveBuildingIdForFloorCreate(null);
                  setActiveFloorIdForRoomCreate(null);
                  setActiveRoomIdForBedCreate(null);
                  setEffectiveDateDrafts({});
                  setBranchEffectiveDateDrafts({});
                  setPeopleTabKey("people");
                  employmentForm.resetFields();
                  editEmploymentForm.resetFields();
                  enterpriseBranchForm.resetFields();
                  editEnterpriseBranchForm.resetFields();
                  createBuildingForm.resetFields();
                  editBuildingForm.resetFields();
                  createFloorForm.resetFields();
                  editFloorForm.resetFields();
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
                                <th>Employment Number</th>
                                <th>Branches</th>
                                <th>Roles</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              {employmentGroups.map((employmentGroup) => (
                                <tr key={employmentGroup.employeePartyId}>
                                  <td>{employmentGroup.employeeFullName}</td>
                                  <td>{employmentGroup.employmentNumber}</td>
                                  <td>
                                    <Popover
                                      trigger="click"
                                      content={
                                        <div style={{ width: 560 }}>
                                          <table className="tanstack-table">
                                            <thead>
                                              <tr>
                                                <th>Branch</th>
                                                <th>From Date</th>
                                                <th>Thru Date</th>
                                                <th>Action</th>
                                              </tr>
                                            </thead>
                                            <tbody>
                                              {employmentGroup.branchEmployments.map((item) => {
                                                const primaryEmployment = employmentGroup.employments[0];
                                                const key = `${employmentGroup.employeePartyId}:${item.branchId}`;
                                                const draft = getBranchEffectiveDateDraft(employmentGroup, item.branchId);
                                                return (
                                                  <tr key={item.branchId}>
                                                    <td>{item.branchLegalName}</td>
                                                    <td>
                                                      <DatePicker
                                                        format="DD/MM/YYYY"
                                                        style={{ width: 150 }}
                                                        value={toLocalDatePickerValue(draft.fromDate)}
                                                        onChange={(value) => {
                                                          if (!value) {
                                                            return;
                                                          }
                                                          setBranchEffectiveDateDrafts((current) => ({
                                                            ...current,
                                                            [key]: {
                                                              ...getBranchEffectiveDateDraft(employmentGroup, item.branchId),
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
                                                          setBranchEffectiveDateDrafts((current) => ({
                                                            ...current,
                                                            [key]: {
                                                              ...getBranchEffectiveDateDraft(employmentGroup, item.branchId),
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
                                                          updateEmploymentBranchEffectiveDateMutation.mutate({
                                                            employmentId: primaryEmployment.employmentId,
                                                            branchId: item.branchId,
                                                            fromDate: getBranchEffectiveDateDraft(
                                                              employmentGroup,
                                                              item.branchId
                                                            ).fromDate,
                                                            thruDate: getBranchEffectiveDateDraft(
                                                              employmentGroup,
                                                              item.branchId
                                                            ).thruDate
                                                          })
                                                        }
                                                        loading={updateEmploymentBranchEffectiveDateMutation.isPending}
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
                                        Branches ({employmentGroup.branchEmployments.length})
                                      </Button>
                                    </Popover>
                                  </td>
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
                                            employmentNumber: employmentGroup.employmentNumber,
                                            branchIds: employmentGroup.branchIds,
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
                  key: "branchs",
                  label: "Branchs",
                  children: (
                    <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                      <Space style={{ width: "100%", justifyContent: "flex-end" }}>
                        <Button type="primary" onClick={() => setCreateEnterpriseBranchOpen(true)}>
                          Add Branch
                        </Button>
                      </Space>

                      {enterpriseBranchsQuery.isError ? (
                        <Alert
                          type="error"
                          message="Failed to load enterprise branchs"
                          description={
                            enterpriseBranchsQuery.error instanceof Error
                              ? enterpriseBranchsQuery.error.message
                              : "Unknown error"
                          }
                          showIcon
                        />
                      ) : enterpriseBranchsQuery.isLoading ? (
                        <Spin />
                      ) : enterpriseBranchs.length === 0 ? (
                        <Empty description="No enterprise branchs found." />
                      ) : (
                        <div className="tanstack-table-wrapper">
                          <table className="tanstack-table">
                            <thead>
                              <tr>
                                <th>Branch Legal Name</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              {enterpriseBranchs.map((enterpriseBranch) => (
                                <tr key={enterpriseBranch.enterpriseBranchId}>
                                  <td>{enterpriseBranch.branchLegalName}</td>
                                  <td>
                                    <Space>
                                      <Button
                                        size="small"
                                        onClick={() => {
                                          setEditingEnterpriseBranch(enterpriseBranch);
                                          editEnterpriseBranchForm.setFieldsValue({
                                            name: enterpriseBranch.branchLegalName
                                          });
                                        }}
                                      >
                                        Edit
                                      </Button>
                                      <Popconfirm
                                        title="Delete branch?"
                                        description="This removes the branch relationship from this enterprise."
                                        okText="Delete"
                                        okButtonProps={{ danger: true, loading: deleteEnterpriseBranchMutation.isPending }}
                                        onConfirm={() => deleteEnterpriseBranchMutation.mutate(enterpriseBranch.enterpriseBranchId)}
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
                      <Card size="small" title="Buildings / Floors / Rooms / Beds">
                        <Space direction="vertical" size="middle" style={{ width: "100%" }}>
                          <Space style={{ width: "100%", justifyContent: "flex-end" }}>
                            <Button type="primary" onClick={() => setCreateBuildingOpen(true)}>
                              Add Building
                            </Button>
                          </Space>

                          {facilitiesTreeQuery.isError ? (
                            <Alert
                              type="error"
                              message="Failed to load facilities"
                              description={
                                facilitiesTreeQuery.error instanceof Error
                                  ? facilitiesTreeQuery.error.message
                                  : "Unknown error"
                              }
                              showIcon
                            />
                          ) : facilitiesTreeQuery.isLoading ? (
                            <Spin />
                          ) : facilitiesTree.length === 0 ? (
                            <Empty description="No facilities found." />
                          ) : (
                            <Tree showLine defaultExpandAll blockNode treeData={facilitiesTreeData} />
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
                  name="employmentNumber"
                  label="Employment Number"
                  rules={[{ required: true, message: "Employment number is required" }]}
                >
                  <Input maxLength={32} />
                </Form.Item>
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
                  name="branchIds"
                  label="Branches"
                >
                  <Select
                    mode="multiple"
                    showSearch
                    options={enterpriseBranchOptions}
                    loading={enterpriseBranchsQuery.isLoading}
                    placeholder="Select branchs"
                    optionFilterProp="label"
                  />
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
                  name="employmentNumber"
                  label="Employment Number"
                  rules={[{ required: true, message: "Employment number is required" }]}
                >
                  <Input maxLength={32} />
                </Form.Item>
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
                  name="branchIds"
                  label="Branches"
                >
                  <Select
                    mode="multiple"
                    showSearch
                    options={enterpriseBranchOptions}
                    loading={enterpriseBranchsQuery.isLoading}
                    placeholder="Select branchs"
                    optionFilterProp="label"
                  />
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
              open={isCreateEnterpriseBranchOpen}
              title="Add Enterprise Branch"
              submitText="Add"
              onClose={() => {
                setCreateEnterpriseBranchOpen(false);
                enterpriseBranchForm.resetFields();
              }}
              onSubmit={() => enterpriseBranchForm.submit()}
              loading={createEnterpriseBranchMutation.isPending}
            >
              <Form<EnterpriseBranchFormValues>
                form={enterpriseBranchForm}
                layout="vertical"
                onFinish={(values) => createEnterpriseBranchMutation.mutate(values)}
              >
                <Form.Item
                  name="name"
                  label="Branch Name"
                  rules={[{ required: true, message: "Branch name is required" }]}
                >
                  <Input maxLength={200} />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={Boolean(editingEnterpriseBranch)}
              title="Edit Enterprise Branch"
              submitText="Update"
              onClose={() => {
                setEditingEnterpriseBranch(null);
                editEnterpriseBranchForm.resetFields();
              }}
              onSubmit={() => editEnterpriseBranchForm.submit()}
              loading={updateEnterpriseBranchMutation.isPending}
            >
              <Form<EnterpriseBranchFormValues>
                form={editEnterpriseBranchForm}
                layout="vertical"
                onFinish={(values) => updateEnterpriseBranchMutation.mutate(values)}
              >
                <Form.Item
                  name="name"
                  label="Branch Name"
                  rules={[{ required: true, message: "Branch name is required" }]}
                >
                  <Input maxLength={200} />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={isCreateBuildingOpen}
              title="Add Building"
              submitText="Add"
              onClose={() => {
                setCreateBuildingOpen(false);
                createBuildingForm.resetFields();
              }}
              onSubmit={() => createBuildingForm.submit()}
              loading={createBuildingMutation.isPending}
            >
              <Form<BuildingFormValues>
                form={createBuildingForm}
                layout="vertical"
                onFinish={(values) => createBuildingMutation.mutate(values)}
                initialValues={{ branchIds: [] }}
              >
                <Form.Item
                  name="name"
                  label="Building Name"
                  rules={[{ required: true, message: "Building name is required" }]}
                >
                  <Input maxLength={200} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
                <Form.Item name="branchIds" label="Branches">
                  <Select
                    mode="multiple"
                    showSearch
                    options={enterpriseBranchOptions}
                    loading={enterpriseBranchsQuery.isLoading}
                    placeholder="Select branchs"
                    optionFilterProp="label"
                  />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={Boolean(editingBuilding)}
              title="Edit Building"
              submitText="Update"
              onClose={() => {
                setEditingBuilding(null);
                editBuildingForm.resetFields();
              }}
              onSubmit={() => editBuildingForm.submit()}
              loading={updateBuildingMutation.isPending}
            >
              <Form<BuildingFormValues>
                form={editBuildingForm}
                layout="vertical"
                onFinish={(values) => updateBuildingMutation.mutate(values)}
                initialValues={{ branchIds: [] }}
              >
                <Form.Item
                  name="name"
                  label="Building Name"
                  rules={[{ required: true, message: "Building name is required" }]}
                >
                  <Input maxLength={200} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
                <Form.Item name="branchIds" label="Branches">
                  <Select
                    mode="multiple"
                    showSearch
                    options={enterpriseBranchOptions}
                    loading={enterpriseBranchsQuery.isLoading}
                    placeholder="Select branchs"
                    optionFilterProp="label"
                  />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={isCreateFloorOpen}
              title="Add Floor"
              submitText="Add"
              onClose={() => {
                setCreateFloorOpen(false);
                setActiveBuildingIdForFloorCreate(null);
                createFloorForm.resetFields();
              }}
              onSubmit={() => createFloorForm.submit()}
              loading={createFloorMutation.isPending}
            >
              <Form<FloorFormValues> form={createFloorForm} layout="vertical" onFinish={(values) => createFloorMutation.mutate(values)}>
                <Form.Item
                  name="buildingId"
                  label="Building"
                  initialValue={activeBuildingIdForFloorCreate ?? undefined}
                  rules={[{ required: true, message: "Building is required" }]}
                >
                  <Select showSearch options={buildingOptions} placeholder="Select building" optionFilterProp="label" />
                </Form.Item>
                <Form.Item name="level" label="Floor Level" rules={[{ required: true, message: "Floor level is required" }]}>
                  <InputNumber style={{ width: "100%" }} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={Boolean(editingFloor)}
              title="Edit Floor"
              submitText="Update"
              onClose={() => {
                setEditingFloor(null);
                editFloorForm.resetFields();
              }}
              onSubmit={() => editFloorForm.submit()}
              loading={updateFloorMutation.isPending}
            >
              <Form<FloorFormValues> form={editFloorForm} layout="vertical" onFinish={(values) => updateFloorMutation.mutate(values)}>
                <Form.Item
                  name="buildingId"
                  label="Building"
                  rules={[{ required: true, message: "Building is required" }]}
                >
                  <Select showSearch options={buildingOptions} placeholder="Select building" optionFilterProp="label" />
                </Form.Item>
                <Form.Item name="level" label="Floor Level" rules={[{ required: true, message: "Floor level is required" }]}>
                  <InputNumber style={{ width: "100%" }} />
                </Form.Item>
                <Form.Item name="description" label="Description">
                  <Input.TextArea rows={3} maxLength={500} />
                </Form.Item>
              </Form>
            </TopDrawerForm>

            <TopDrawerForm
              open={isCreateRoomOpen}
              title="Add Room"
              submitText="Add"
              onClose={() => {
                setCreateRoomOpen(false);
                setActiveFloorIdForRoomCreate(null);
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
                <Form.Item
                  name="floorId"
                  label="Floor"
                  initialValue={activeFloorIdForRoomCreate ?? undefined}
                  rules={[{ required: true, message: "Floor is required" }]}
                >
                  <Select showSearch options={floorOptions} placeholder="Select floor" optionFilterProp="label" />
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
                <Form.Item
                  name="floorId"
                  label="Floor"
                  rules={[{ required: true, message: "Floor is required" }]}
                >
                  <Select showSearch options={floorOptions} placeholder="Select floor" optionFilterProp="label" />
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
                setActiveRoomIdForBedCreate(null);
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
                  initialValue={activeRoomIdForBedCreate ?? undefined}
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

  if (productsEnterprise) {
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
                  Manage products under enterprise: <strong>{productsEnterprise.legalName}</strong>
                </Text>
              </div>
              <Button
                onClick={() => {
                  setProductsEnterprise(null);
                  setProductsTabKey("manage-products");
                  setProductManagementTabKey("services");
                }}
              >
                Back to Enterprises
              </Button>
            </Space>
            <Layout
              style={{ background: "#fff", border: "1px solid #f0f0f0", borderRadius: 8, flex: 1, minHeight: 0, height: "100%" }}
            >
              <Layout.Sider
                width={220}
                theme="light"
                style={{ borderRight: "1px solid #f0f0f0", display: "flex", flexDirection: "column" }}
              >
                <Menu
                  mode="inline"
                  selectedKeys={[productsTabKey]}
                  items={[{ key: "manage-products", label: "Manage Products", icon: <AppstoreOutlined /> }]}
                  onClick={(info) => setProductsTabKey(String(info.key))}
                  style={{ height: "100%", borderInlineEnd: "none" }}
                />
              </Layout.Sider>
              <Layout style={{ minHeight: 0, height: "100%" }}>
                <Layout.Header style={{ background: "#fff", padding: "0 16px", flex: "0 0 auto" }}>
                  <Tabs
                    activeKey={productManagementTabKey}
                    onChange={setProductManagementTabKey}
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
                      }
                    ]}
                  />
                </Layout.Header>
                <Layout.Content style={{ padding: 16, flex: 1, minHeight: 0 }}>
                  <div
                    style={{
                      height: "100%",
                      width: "100%",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      border: "1px dashed #f0f0f0",
                      borderRadius: 8
                    }}
                  >
                    {productManagementTabKey === "services" ? (
                      <Empty description="Service products management UI for this enterprise will be added here." />
                    ) : (
                      <Empty description="Goods products management UI for this enterprise will be added here." />
                    )}
                  </div>
                </Layout.Content>
              </Layout>
            </Layout>
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
