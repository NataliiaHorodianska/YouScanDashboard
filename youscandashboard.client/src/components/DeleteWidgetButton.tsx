import { DeleteOutlined } from '@ant-design/icons';
import { App, Button, Popconfirm } from 'antd';
import { useDeleteWidget } from '../hooks/widgetQueries';

interface DeleteWidgetButtonProps {
    id: string;
    title: string;
}

export function DeleteWidgetButton({ id, title }: DeleteWidgetButtonProps) {
    const { message } = App.useApp();
    const { mutate: remove, isPending } = useDeleteWidget(id);

    // Errors of the async request are handled in the callback: an ErrorBoundary does not catch them.
    const handleDelete = () => remove(undefined, { onError: (error) => message.error(error.message) });

    return (
        <Popconfirm title={`Delete "${title}"?`} okText="Delete" okButtonProps={{ danger: true }} onConfirm={handleDelete}>
            <Button type="text" icon={<DeleteOutlined />} loading={isPending} aria-label={`Delete ${title}`} />
        </Popconfirm>
    );
}