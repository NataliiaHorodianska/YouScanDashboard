import { Alert, Modal, Radio } from 'antd';
import { useState } from 'react';
import { widgetTypes, type WidgetType } from '../api/types';
import { useCreateWidget } from '../hooks/widgetQueries';
import { widgetTypeLabels } from './widgetTypeLabels';

interface CreateWidgetDialogProps {
    onClose: () => void;
}

const typeOptions = widgetTypes.map((type) => ({ value: type, label: widgetTypeLabels[type] }));

export function CreateWidgetDialog({ onClose }: CreateWidgetDialogProps) {
    const [type, setType] = useState<WidgetType>(widgetTypes[0]);
    const { mutate: create, isPending, isError, error } = useCreateWidget();

    // A failed request keeps the dialog open with the error, so the user can retry.
    const handleCreate = () => create(type, { onSuccess: onClose });

    return (
        <Modal
            open
            title="Create widget"
            okText="Create"
            onOk={handleCreate}
            onCancel={onClose}
            // Disabled while creating: a second click cannot create a second widget.
            confirmLoading={isPending}
            cancelButtonProps={{ disabled: isPending }}
            mask={{ closable: !isPending }}
        >
            <Radio.Group
                vertical
                options={typeOptions}
                value={type}
                onChange={(event) => setType(event.target.value)}
                disabled={isPending}
            />
            {isError && <Alert className="dialog-error" type="error" showIcon title={error.message} />}
        </Modal>
    );
}