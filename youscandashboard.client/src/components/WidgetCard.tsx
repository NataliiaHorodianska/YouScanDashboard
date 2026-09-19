import { Card, Spin } from 'antd';
import { memo } from 'react';
import type { WidgetType } from '../api/types';
import { DeleteWidgetButton } from './DeleteWidgetButton';
import { QueryBoundary } from './QueryBoundary';
import { WidgetContent } from './WidgetContent';
import { widgetTypeLabels } from './widgetTypeLabels';

interface WidgetCardProps {
    id: string;
    type: WidgetType;
}

function WidgetCardView({ id, type }: WidgetCardProps) {
    const title = widgetTypeLabels[type];

    return (
        // Delete sits outside the boundary: a widget that failed to load can still be deleted.
        <Card title={title} extra={<DeleteWidgetButton id={id} title={title} />}>
            {/* Fixed-height body keeps cards in a row aligned in every state: loading, error, data. */}
            <div className="widget-body">
                <QueryBoundary
                    fallback={
                        <div className="widget-loading">
                            <Spin />
                        </div>
                    }
                >
                    <WidgetContent id={id} />
                </QueryBoundary>
            </div>
        </Card>
    );
}
export const WidgetCard = memo(WidgetCardView);