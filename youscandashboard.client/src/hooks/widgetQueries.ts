import { useMutation, useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import type { WidgetDetails, WidgetSummary, WidgetType } from '../api/types';
import { widgetsApi } from '../api/widgetsApi';

export const widgetKeys = {
    all: ['widgets'] as const,
    detail: (id: string) => ['widgets', id] as const,
};

export function useWidgets() {
    return useSuspenseQuery({
        queryKey: widgetKeys.all,
        queryFn: ({ signal }) => widgetsApi.getAll(signal),
    });
}

export function useWidget(id: string) {
    return useSuspenseQuery({
        queryKey: widgetKeys.detail(id),
        queryFn: ({ signal }) => widgetsApi.get(id, signal),
    });
}

export function useCreateWidget() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (type: WidgetType) => widgetsApi.create(type),
        onSuccess: (widget) => {
            // The response already has the widget's data: the new card renders it without another request.
            queryClient.setQueryData(widgetKeys.detail(widget.id), widget);
            // `exact` refreshes only the list; the other widgets keep their cached data.
            return queryClient.invalidateQueries({ queryKey: widgetKeys.all, exact: true });
        },
    });
}

export function useUpdateWidgetContent(id: string) {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (content: string) => widgetsApi.updateContent(id, content),
        // The backend returns 204 without a body: put the saved text into the cache instead of refetching.
        onSuccess: (_, content) => {
            queryClient.setQueryData<WidgetDetails>(widgetKeys.detail(id), (widget) => widget && { ...widget, content });
        },
    });
}

export function useDeleteWidget(id: string) {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => widgetsApi.remove(id),
        onSuccess: () => {
            // Remove the card from the list first, then drop its cached data.
            queryClient.setQueryData<WidgetSummary[]>(widgetKeys.all, (widgets) => widgets?.filter((w) => w.id !== id));
            queryClient.removeQueries({ queryKey: widgetKeys.detail(id), exact: true });
        },
    });
}
export function useUploadFile() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (file: File) => widgetsApi.upload(file),
        // New widgets load their own data when their cards mount: refreshing the list is enough.
        onSuccess: () => queryClient.invalidateQueries({ queryKey: widgetKeys.all, exact: true }),
    });
}