// test.js
import http from 'k6/http';
import { sleep } from 'k6';
import { services, options } from './config.js';

export { options };

export default function () {
  services.filter(service => service.name == 'EPS' && service.protocol == 'http').forEach(service => {

    http.get(service.url + 'health', {
      tags: {
        dta_service: service.name + '-' + service.compilation,
        test_scenario: 'scenario1',
      },
    });
  
    sleep(1);
  });
}
