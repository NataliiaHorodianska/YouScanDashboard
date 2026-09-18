import { PlusOutlined } from '@ant-design/icons';
import { Button } from 'antd';
import { useState } from 'react';
import { CreateWidgetDialog } from './CreateWidgetDialog';

export function CreateWidgetButton() {
    // Dialog state lives next to the button: opening it does not re-render the dashboard.
    const [isOpen, setIsOpen] = useState(false);

    return (
        <>
            <Button type="primary" icon={<PlusOutlined />} onClick={() => setIsOpen(true)}>
                Create widget
            </Button>
            {/* Mounted only while open, so every opening starts with a fresh selection. */}
            {isOpen && <CreateWidgetDialog onClose={() => setIsOpen(false)} />}
        </>
    );
}