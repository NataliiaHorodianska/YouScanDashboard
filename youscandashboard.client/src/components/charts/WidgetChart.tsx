import type { ChartData, WidgetType } from '../../api/types';
import { BarChartView } from './BarChartView';
import { LineChartView } from './LineChartView';
import { PieChartView } from './PieChartView';

interface WidgetChartProps {
    type: Exclude<WidgetType, 'Text'>;
    data: ChartData;
}

export function WidgetChart({ type, data }: WidgetChartProps) {
    if (data.points.length === 0) {
        return <p className="chart-empty">No data to display</p>;
    }

    switch (type) {
        case 'LineChart':
            return <LineChartView data={data} />;
        case 'BarChart':
            return <BarChartView data={data} />;
        case 'StackedBarChart':
            return <BarChartView data={data} stacked />;
        case 'PieChart':
            return <PieChartView data={data} />;
    }
}