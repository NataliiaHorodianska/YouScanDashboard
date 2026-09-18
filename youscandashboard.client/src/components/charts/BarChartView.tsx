import { Bar, BarChart, CartesianGrid, Legend, Tooltip, XAxis, YAxis } from 'recharts';
import type { ChartData } from '../../api/types';
import { chartStyle, legendText, seriesColor } from './chartTheme';

interface BarChartViewProps {
    data: ChartData;
    stacked?: boolean;
}

const STACK_ID = 'total';
const MAX_BAR_SIZE = 24;
const ROUNDED_TOP: [number, number, number, number] = [4, 4, 0, 0];

export function BarChartView({ data, stacked = false }: BarChartViewProps) {
    const lastIndex = data.series.length - 1;

    return (
        <BarChart responsive data={data.points} margin={chartStyle.margin} className="chart">
            <CartesianGrid vertical={false} stroke={chartStyle.grid} />
            <XAxis dataKey="label" tick={chartStyle.axisTick} />
            <YAxis tick={chartStyle.axisTick} width={40} />
            <Tooltip itemStyle={chartStyle.tooltipItem} cursor={{ fill: chartStyle.grid, opacity: 0.4 }} />
            {/* A single series needs no legend: the widget title names it. */}
            {data.series.length > 1 && <Legend itemSorter={null} formatter={legendText} />}
            {data.series.map((name, index) => (
                <Bar
                    key={name}
                    name={name}
                    dataKey={(point) => point.values[name]}
                    fill={seriesColor(index)}
                    maxBarSize={MAX_BAR_SIZE}
                    stackId={stacked ? STACK_ID : undefined}
                    // Surface-colored stroke leaves a gap between stacked segments and adjacent bars.
                    stroke={chartStyle.surface}
                    strokeWidth={stacked ? 2 : 0}
                    // Only the free end is rounded: the top segment of a stack, or every bar when not stacked.
                    radius={!stacked || index === lastIndex ? ROUNDED_TOP : 0}
                />
            ))}
        </BarChart>
    );
}