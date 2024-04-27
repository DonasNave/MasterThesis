// scenario1.js
// launch with k6 run scenario3.js --env SERVICE_URL=#url# --env SERVICE_NAME=#name# --env COMPILATION_MODE=#mode#
import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  stages: [
    { duration: '5s', target: 3 },
    { duration: '10s', target: 6 },
    { duration: '5s', target: 0 },
  ],
  thresholds: {
    'http_req_duration': ['p(95)<500'],
  },
};

export default async function () {
  const serviceUrl = __ENV.SERVICE_URL;
  const serviceName = "BPS";
  const compilationMode = __ENV.COMPILATION_MODE;
  const testId = __ENV.TEST_ID;

  const params = {
    headers: {
      'test_id': testId
    },
    tags: {
      dta_service: serviceName + '-' + compilationMode,
      test_scenario: 'scenario3',
      test_id: testId, 
    },
  };

  http.get(`${serviceUrl}api/processFibonacci/40`, params);

  sleep(1);
}