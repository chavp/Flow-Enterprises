import { Button, Drawer, Space } from "antd";
import type { ReactNode } from "react";

type TopDrawerFormProps = {
  open: boolean;
  title: string;
  submitText: string;
  loading?: boolean;
  onClose: () => void;
  onSubmit: () => void;
  children: ReactNode;
};

export function TopDrawerForm({
  open,
  title,
  submitText,
  loading,
  onClose,
  onSubmit,
  children
}: TopDrawerFormProps) {
  return (
    <Drawer
      open={open}
      placement="top"
      height="100%"
      title={title}
      onClose={onClose}
      destroyOnClose
      footer={
        <Space style={{ width: "100%", justifyContent: "flex-end" }}>
          <Button onClick={onClose}>Cancel</Button>
          <Button type="primary" loading={loading} onClick={onSubmit}>
            {submitText}
          </Button>
        </Space>
      }
    >
      {children}
    </Drawer>
  );
}
