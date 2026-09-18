import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App.tsx';
import './index.css';

// Created once, outside of components: it holds the cache for the whole application lifetime.
const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            // Show the widget error state after one retry instead of three (the library default).
            retry: 1,
            // Dashboard data changes only through the user's own actions.
            refetchOnWindowFocus: false,
        },
    },
});

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <QueryClientProvider client={queryClient}>
            <App />
        </QueryClientProvider>
    </StrictMode>,
);