import { CartesianGrid, Legend, Line, LineChart, Tooltip, XAxis, YAxis } from 'recharts';
import type { ChartData } from '../../api/types';
import { chartStyle, legendText, seriesColor } from './chartTheme';

interface LineChartViewProps {
    data: ChartData;
}

export function LineChartView({ data }: LineChartViewProps) {
    return (
        <LineChart responsive data={data.points} margin={chartStyle.margin} className="chart">
            <CartesianGrid vertical={false} stroke={chartStyle.grid} />
            <XAxis dataKey="label" tick={chartStyle.axisTick} minTickGap={24} />
            <YAxis tick={chartStyle.axisTick} width={40} />
            <Tooltip itemStyle={chartStyle.tooltipItem} />
            <Legend itemSorter={null} formatter={legendText} />
            {data.series.map((name, index) => (
                <Line
                    key={name}
                    name={name}
                    dataKey={(point) => point.values[name]}
                    stroke={seriesColor(index)}
                    strokeWidth={2}
                    dot={false}
                    activeDot={{ r: 4, stroke: chartStyle.surface, strokeWidth: 2 }}
                />
            ))}
        </LineChart>
    );
}