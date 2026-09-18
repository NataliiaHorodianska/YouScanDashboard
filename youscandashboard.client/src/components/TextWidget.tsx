import { Button, Typography } from 'antd';
import { useState } from 'react';
import { TextWidgetEditor } from './TextWidgetEditor';

interface TextWidgetProps {
    id: string;
    content: string;
}

export function TextWidget({ id, content }: TextWidgetProps) {
    // Edit mode lives here, not in the dashboard: toggling it re-renders only this widget.
    const [isEditing, setIsEditing] = useState(false);

    if (isEditing) {
        // The editor mounts on every Edit, so its draft always starts from the saved text.
        return <TextWidgetEditor id={id} initialContent={content} onClose={() => setIsEditing(false)} />;
    }

    return (
        <div className="text-widget">
            <div className="text-widget-content">
                {content ? (
                    <Typography.Paragraph className="text-widget-text">{content}</Typography.Paragraph>
                ) : (
                    <Typography.Text type="secondary">No text yet</Typography.Text>
                )}
            </div>
            <div className="text-widget-actions">
                <Button onClick={() => setIsEditing(true)}>Edit</Button>
            </div>
        </div>
    );
}