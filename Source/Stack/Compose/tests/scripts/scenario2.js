// test.js
import http from 'k6/http';
import { sleep } from 'k6';
import { services, options } from './config.js';

export { options };

export default function () {
  services.filter(service => service.name == 'SRS').forEach(service => {

    http.get(service.url + 'api/signals/5', {
      tags: {
        dta_service: service.name + '-' + service.compilation,
        test_scenario: 'scenario2',
      },
    });
  
    sleep(1);
  });
}