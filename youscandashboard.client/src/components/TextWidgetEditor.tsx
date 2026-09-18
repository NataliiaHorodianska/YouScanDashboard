import { Alert, Button, Input } from 'antd';
import { useState } from 'react';
import { useUpdateWidgetContent } from '../hooks/widgetQueries';

interface TextWidgetEditorProps {
    id: string;
    initialContent: string;
    onClose: () => void;
}

export function TextWidgetEditor({ id, initialContent, onClose }: TextWidgetEditorProps) {
    // The draft lives in the editor: typing re-renders only the editor.
    const [draft, setDraft] = useState(initialContent);
    const { mutate: save, isPending, isError, error } = useUpdateWidgetContent(id);

    // A failed save is shown next to the editor (not in the ErrorBoundary), so the draft is not lost.
    const handleSave = () => save(draft, { onSuccess: onClose });

    return (
        <div className="text-widget">
            <Input.TextArea
                className="text-widget-input"
                value={draft}
                onChange={(event) => setDraft(event.target.value)}
                autoFocus
                disabled={isPending}
            />
            {isError && <Alert type="error" showIcon title={error.message} />}
            <div className="text-widget-actions">
                <Button onClick={onClose} disabled={isPending}>
                    Cancel
                </Button>
                {/* Disabled while saving: a second click cannot send a competing request. */}
                <Button type="primary" onClick={handleSave} loading={isPending}>
                    Save
                </Button>
            </div>
        </div>
    );
}