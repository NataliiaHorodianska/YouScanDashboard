import { Legend, Pie, PieChart, Tooltip } from 'recharts';
import type { ChartData } from '../../api/types';
import { chartStyle, legendText, seriesColor } from './chartTheme';

interface PieChartViewProps {
    data: ChartData;
}

export function PieChartView({ data }: PieChartViewProps) {
    // A pie shows one series: the first one. Each slice gets its color by position.
    const [series] = data.series;
    const slices = data.points.map((point, index) => ({
        name: point.label,
        value: point.values[series] ?? 0,
        fill: seriesColor(index),
    }));

    return (
        <PieChart responsive className="chart">
            <Pie
                data={slices}
                dataKey="value"
                nameKey="name"
                outerRadius="75%"
                stroke={chartStyle.surface}
                strokeWidth={2}
                label={chartStyle.label}
            />
            <Tooltip itemStyle={chartStyle.tooltipItem} />
            <Legend itemSorter={null} formatter={legendText} />
        </PieChart>
    );
}