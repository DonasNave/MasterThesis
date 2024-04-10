// config.js
export const services = [

    { name: "SRS", compilation: "JIT", url: "http://localhost:12001/", protocol: "http"},
    { name: "FUS", compilation: "JIT", url: "http://localhost:12002/", protocol: "http"},
    { name: "FUS", compilation: "JIT", url: "http://localhost:12102/", protocol: "grpc"},
    { name: "BPS", compilation: "JIT", url: "http://localhost:12003/", protocol: "http"},
    { name: "EPS", compilation: "JIT", url: "http://localhost:12004/", protocol: "http"},

    { name: "SRS", compilation: "AOT", url: "http://localhost:13001/", protocol: "http"},
    { name: "FUS", compilation: "AOT", url: "http://localhost:13002/", protocol: "http"},
    { name: "FUS", compilation: "AOT", url: "http://localhost:13102/", protocol: "grpc"},
    { name: "BPS", compilation: "AOT", url: "http://localhost:13003/", protocol: "http"},
    { name: "EPS", compilation: "AOT", url: "http://localhost:13004/", protocol: "http"},

    // { name: "SRS-JIT", url: "http://srs-jit-service:8080/" },
    // { name: "SRS-AOT", url: "http://srs-aot-service:8080/" },
    // { name: "FUS-JIT", url: "http://fus-jit-service:8080/" },
    // { name: "FUS-AOT", url: "http://fus-aot-service:8080/" },
    // { name: "BPS-JIT", url: "http://bps-jit-service:8080/" },
    // { name: "BPS-AOT", url: "http://bps-aot-service:8080/" },
    // { name: "EPS-JIT", url: "http://eps-jit-service:8080/" },
    // { name: "EPS-AOT", url: "http://eps-aot-service:8080/" }
  ];
  
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
  