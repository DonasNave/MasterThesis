import http from 'k6/http';
import { check, sleep } from 'k6';

const file = open("../sample-file.txt");  

export let options = {
    vus: 1,
    duration: '1m',
};

export default async function () {
    const serviceUrl = __ENV.SERVICE_URL;
    const compilationMode = __ENV.COMPILATION_MODE;
    const serviceName = "FUS";
    const testId = __ENV.TEST_ID;

    const formData = {
        'file': http.file(file, 'sample-file.txt')
    };

    var params = {
        tags: {
            dta_service: serviceName + '-' + compilationMode,
            test_scenario: 'scenario2',
            scenario_method: "file_upload",
            test_id: testId
        }
    };

    // Perform the upload
    let uploadResponse = http.post(`${serviceUrl}api/file/upload`, formData, params);

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

    // Sleep to avoid hammering the server too hard
    sleep(1);
}
