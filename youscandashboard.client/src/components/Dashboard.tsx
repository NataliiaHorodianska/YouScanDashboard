import { Col, Empty, Row } from 'antd';
import { useWidgets } from '../hooks/widgetQueries';
import { WidgetCard } from './WidgetCard';

// 24-column grid: span 8 = three widgets per row; rows wrap without limit.
const WIDGET_SPAN = 8;

export function Dashboard() {
    const { data: widgets } = useWidgets();

    if (widgets.length === 0) {
        return <Empty description="No widgets yet" />;
    }

    return (
        <Row gutter={[16, 16]}>
            {widgets.map((widget) => (
                // The widget id as key: a deleted widget takes its own card with it.
                <Col key={widget.id} span={WIDGET_SPAN}>
                    <WidgetCard id={widget.id} type={widget.type} />
                </Col>
            ))}
        </Row>
    );
}