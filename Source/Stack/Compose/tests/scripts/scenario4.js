// scenario4.js
// launch with k6 run scenario3.js --env SERVICE_URL=#url# --env SERVICE_NAME=#name# --env COMPILATION_MODE=#mode#
import http from 'k6/http';
import { check, sleep } from 'k6';

const file = open("../sample-file.txt");  

export const options = {
    vus: 1,
    duration: '1m',
};

export default async function () {
    const serviceUrl = __ENV.SERVICE_URL;
    const compilationMode = __ENV.COMPILATION_MODE;
    const testId = __ENV.TEST_ID;
    const fusUrl = __ENV.FUS_URL;

    const formData = {
        'file': http.file(file, 'sample-file.txt')
    };

    var params = {
        tags: {
            dta_service: 'FUS-' + compilationMode,
            test_scenario: 'scenario4',
            scenario_method: "file_upload",
            test_id: testId
        }
    };

    // Perform the upload
    let uploadResponse = http.post(`${fusUrl}api/file/upload`, formData, params);
    check(uploadResponse, {
        'is status 200': (r) => r.status === 200,
    });

    // Assume the response returns an ID that can be used to download the file
    let fileId = uploadResponse.json().id;

    http.get(`${serviceUrl}api/simulateEvent/${fileId}`, {
        tags: {
            dta_service: 'EPS-' + compilationMode,
            test_scenario: 'scenario4',
            test_id: testId,
        },
    });

    sleep(5);
}