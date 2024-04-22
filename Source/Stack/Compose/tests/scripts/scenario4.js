// scenario4.js
// launch with k6 run scenario3.js --env SERVICE_URL=#url# --env SERVICE_NAME=#name# --env COMPILATION_MODE=#mode#
import http from 'k6/http';
import { sleep } from 'k6';
import { open } from 'k6/experimental/fs';

let file;
(async function () {
  file = await open('../sample-file.txt');
})();

export const options = {
    vus: 1,
    duration: '1m',
};

export default function () {
    const serviceUrl = __ENV.SERVICE_URL;
    const compilationMode = __ENV.COMPILATION_MODE;
    const testId = __ENV.TEST_ID;

    const fileinfo = await file.stat();
    if (fileinfo.name != 'sample-file.txt') {
        throw new Error('Unexpected file name');
    }

    // Define the boundary for the multipart form data
    var boundary = '----WebKitFormBoundaryxyxyxyxyxyxyx';
    var body = '--' + boundary + '\r\n' +
               'Content-Disposition: form-data; name="file"; filename="sample.txt"\r\n' +
               'Content-Type: text/plain\r\n' +
               '\r\n' +
               file + '\r\n' +
               '--' + boundary + '--';

    var params = {
        headers: {
            'Content-Type': 'multipart/form-data; boundary=' + boundary
        },
        tags: {
            dta_service: 'FUS-' + compilationMode,
            test_scenario: 'scenario4',
            scenario_method: "file_upload",
            test_id: testId
        }
    };

    // Perform the upload
    let uploadResponse = http.post(`${serviceUrl}api/file/upload`, body, params);
    check(uploadResponse, {
        'is status 200': (r) => r.status === 200,
    });

    // Assume the response returns an ID that can be used to download the file
    let fileId = uploadResponse.json().id;

    http.get(`${serviceUrl}api/simulateEvent/${fileId}`, {
        tags: {
            dta_service: serviceName + 'EPS-' + compilationMode,
            test_scenario: 'scenario4',
            test_id: testId,
        },
    });

    sleep(4);
}