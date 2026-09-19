import type { WidgetDetails, WidgetSummary, WidgetType } from './types';

export class ApiError extends Error {
    readonly status: number;

    constructor(status: number, message: string) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
    }
}

interface ProblemDetails {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
}

async function readErrorMessage(response: Response): Promise<string> {
    const fallback = `Request failed with status ${response.status}`;

    try {
        const problem = (await response.json()) as ProblemDetails;
        const validationErrors = problem.errors ? Object.values(problem.errors).flat().join(' ') : '';
        return validationErrors || problem.detail || problem.title || fallback;
    } catch {
        return fallback;
    }
}

async function request<T>(method: string, path: string, body?: unknown, signal?: AbortSignal): Promise<T> {
  
    const isJson = body !== undefined && !(body instanceof FormData);

    const response = await fetch(`/api${path}`, {
        method,
        signal,
        headers: isJson ? { 'Content-Type': 'application/json' } : undefined,
        body: isJson ? JSON.stringify(body) : (body as FormData | undefined),
    });

    if (!response.ok) {
        throw new ApiError(response.status, await readErrorMessage(response));
    }

    return response.status === 204 ? (undefined as T) : ((await response.json()) as T);
}

export const widgetsApi = {
    getAll: (signal?: AbortSignal) => request<WidgetSummary[]>('GET', '/widgets', undefined, signal),
    get: (id: string, signal?: AbortSignal) => request<WidgetDetails>('GET', `/widgets/${id}`, undefined, signal),
    create: (type: WidgetType) => request<WidgetDetails>('POST', '/widgets', { type }),
    updateContent: (id: string, content: string) => request<void>('PUT', `/widgets/${id}/content`, { content }),
    remove: (id: string) => request<void>('DELETE', `/widgets/${id}`),
    upload: (file: File) => {
        const form = new FormData();
        form.append('file', file);
        return request<WidgetSummary[]>('POST', '/imports', form);
    },
};