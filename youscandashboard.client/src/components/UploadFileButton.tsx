import { UploadOutlined } from '@ant-design/icons';
import { App, Button, Upload } from 'antd';
import { useUploadFile } from '../hooks/widgetQueries';

// A hint for the file picker only: the backend validates the format.
const ACCEPTED_FILES = '.xlsx,.xls,.csv';

export function UploadFileButton() {
    const { message } = App.useApp();
    const { mutate: upload, isPending } = useUploadFile();

    const handleFile = (file: File) => {
        upload(file, {
            onSuccess: (widgets) => message.success(`${file.name}: ${widgets.length} widget(s) added`),
            onError: (error) => message.error(`${file.name}: ${error.message}`),
        });

        // The file is sent by the API client, not by the Upload component itself.
        return false;
    };

    return (
        <Upload accept={ACCEPTED_FILES} showUploadList={false} beforeUpload={handleFile} disabled={isPending}>
            <Button icon={<UploadOutlined />} loading={isPending}>
                Upload file
            </Button>
        </Upload>
    );
}