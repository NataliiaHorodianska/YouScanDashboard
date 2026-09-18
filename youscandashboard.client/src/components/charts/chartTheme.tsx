// Categorical colors in a fixed order: a series keeps its color by position, never cycled or random.
// Validated for color-vision deficiency on adjacent pairs (lines, stacked segments, pie slices).
const SERIES_COLORS = ['#2a78d6', '#eb6834', '#1baf7a', '#eda100', '#e87ba4', '#008300', '#4a3aa7', '#e34948'];

// Fallback for more series than colors; the palette covers every provided dataset.
const OVERFLOW_COLOR = '#8c8b87';

const TEXT_PRIMARY = '#0b0b0b';
const TEXT_SECONDARY = '#52514e';

export function seriesColor(index: number): string {
    return SERIES_COLORS[index] ?? OVERFLOW_COLOR;
}

export const chartStyle = {
    grid: '#e8e7e4',
    surface: '#ffffff',
    axisTick: { fontSize: 12, fill: TEXT_SECONDARY },
    // Text stays in text colors; the colored mark next to it carries the series identity.
    tooltipItem: { color: TEXT_PRIMARY },
    label: { fontSize: 12, fill: TEXT_SECONDARY },
    // Keeps the last X-axis label from being clipped.
    margin: { top: 8, right: 32, bottom: 0, left: 0 },
} as const;

export function legendText(value: string) {
    return <span className="chart-legend-text">{value}</span>;
}