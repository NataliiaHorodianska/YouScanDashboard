import { useWidget } from '../hooks/widgetQueries';
import { WidgetChart } from './charts/WidgetChart';
import { TextWidget } from './TextWidget';

interface WidgetContentProps {
    id: string;
}

export function WidgetContent({ id }: WidgetContentProps) {
    const { data: widget } = useWidget(id);

    if (widget.type === 'Text') {
        return <TextWidget id={widget.id} content={widget.content ?? ''} />;
    }

    return widget.chart ? (
        <WidgetChart type={widget.type} data={widget.chart} />
    ) : (
        <p className="chart-empty">No data to display</p>
    );
}