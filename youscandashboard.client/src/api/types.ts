export const widgetTypes = ['LineChart', 'BarChart', 'StackedBarChart', 'PieChart', 'Text'] as const;

export type WidgetType = (typeof widgetTypes)[number];

export interface WidgetSummary {
    id: string;
    type: WidgetType;
}

export interface ChartPoint {
    label: string;
    values: Record<string, number | null>;
}

export interface ChartData {
    series: string[];
    points: ChartPoint[];
}

export interface WidgetDetails extends WidgetSummary {
    content: string | null;
    chart: ChartData | null;
}