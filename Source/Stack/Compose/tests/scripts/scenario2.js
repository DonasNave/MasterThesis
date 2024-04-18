import http from 'k6/http';
import { check, sleep } from 'k6';
import fs from 'k6/compat/fs';

export let options = {
    vus: 1,
    duration: '1m',
};

export default function () {
    const serviceUrl = __ENV.SERVICE_URL;
    const compilationMode = __ENV.COMPILATION_MODE;
    const serviceName = "FUS";
    const testId = __ENV.TEST_ID;

    // Load the file. Make sure the path is correct.
    var file = fs.readFileSync('../sample.txt');

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
            dta_service: serviceName + '-' + compilationMode,
            test_scenario: 'scenario2',
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
    let fileId = uploadResponse.json().id; // Adjust this depending on the actual response structure

    var downloadParams = {
      tags: {
        dta_service: serviceName + '-' + compilationMode,
        test_scenario: 'scenario2',
        scenario_method: "file_download",
        test_id: testId
      }
    };

    // Perform the download
    let downloadResponse = http.get(`${serviceUrl}api/file/download/${fileId}`, downloadParams);
    check(downloadResponse, {
        'is status 200': (r) => r.status === 200,
    });

    // Optionally, print the response from download to validate
    console.log('Downloaded file content:', downloadResponse.body);

    // Sleep to avoid hammering the server too hard
    sleep(1);
}
