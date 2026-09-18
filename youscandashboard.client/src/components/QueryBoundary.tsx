import { QueryErrorResetBoundary } from '@tanstack/react-query';
import { Suspense, type ReactNode } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { ErrorFallback } from './ErrorFallback';

interface QueryBoundaryProps {
    /** Element shown while data inside is loading. */
    fallback: ReactNode;
    children: ReactNode;
}

/**
 * Loading and error states for everything inside:
 * Suspense shows `fallback` while queries load; ErrorBoundary catches failed queries
 * and rendering errors, and Retry resets the failed queries before rendering again.
 */
export function QueryBoundary({ fallback, children }: QueryBoundaryProps) {
    return (
        <QueryErrorResetBoundary>
            {({ reset }) => (
                <ErrorBoundary onReset={reset} FallbackComponent={ErrorFallback}>
                    <Suspense fallback={fallback}>{children}</Suspense>
                </ErrorBoundary>
            )}
        </QueryErrorResetBoundary>
    );
}