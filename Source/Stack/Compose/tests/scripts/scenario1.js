// scenario1.js
// launch with k6 run scenario1.js --env SERVICE_URL=#url# --env SERVICE_NAME=#name# --env COMPILATION_MODE=#mode#
import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  stages: [
    { duration: '5s', target: 10 },
    { duration: '10s', target: 20 },
    { duration: '5s', target: 0 },
  ],
  thresholds: {
    'http_req_duration': ['p(95)<500'],
  },
};

export default function () {
  const serviceUrl = __ENV.SERVICE_URL;
  const serviceName = __ENV.SERVICE_NAME;
  const compilationMode = __ENV.COMPILATION_MODE;
  const testId = __ENV.TEST_ID;

  http.get(`${serviceUrl}health`, {
    tags: {
      dta_service: serviceName + '-' + compilationMode,
      test_scenario: 'scenario1',
      test_id: testId,
    },
  });

  sleep(1);
}