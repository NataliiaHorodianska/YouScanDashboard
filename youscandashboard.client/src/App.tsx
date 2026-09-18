import { App as AntApp, Flex, Layout, Spin, Typography } from 'antd';
import { CreateWidgetButton } from './components/CreateWidgetButton';
import { Dashboard } from './components/Dashboard';
import { QueryBoundary } from './components/QueryBoundary';
import { UploadFileButton } from './components/UploadFileButton';

function App() {
    return (
        // Ant Design's App provides the context for message notifications.
        <AntApp>
            <Layout className="app">
                <Layout.Content className="app-content">
                    <Flex justify="space-between" align="center" className="app-header">
                        <Typography.Title level={2}>Dashboard</Typography.Title>
                        <Flex gap={8}>
                            <UploadFileButton />
                            <CreateWidgetButton />
                        </Flex>
                    </Flex>
                    <QueryBoundary
                        fallback={
                            <div className="dashboard-loading">
                                <Spin size="large" />
                            </div>
                        }
                    >
                        <Dashboard />
                    </QueryBoundary>
                </Layout.Content>
            </Layout>
        </AntApp>
    );
}

export default App;