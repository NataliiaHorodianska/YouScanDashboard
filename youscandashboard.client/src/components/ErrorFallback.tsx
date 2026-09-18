import { Alert, Button } from 'antd';
import { getErrorMessage, type FallbackProps } from 'react-error-boundary';

export function ErrorFallback({ error, resetErrorBoundary }: FallbackProps) {
    return (
        <Alert
            type="error"
            showIcon
            title="Something went wrong"
            description={getErrorMessage(error) ?? 'Unknown error'}
            action={
                <Button size="small" onClick={() => resetErrorBoundary()}>
                    Retry
                </Button>
            }
        />
    );
}