// config.js
export const services = [

    { name: "SRS", compilation: "JIT", url: "http://localhost:12001/", protocol: "http"},
    { name: "FUS", compilation: "JIT", url: "http://localhost:12002/", protocol: "http"},
    { name: "FUS", compilation: "JIT", url: "http://localhost:12102/", protocol: "grpc"},
    { name: "EPS", compilation: "JIT", url: "http://localhost:12003/", protocol: "http"},
    { name: "BPS", compilation: "JIT", url: "http://localhost:12004/", protocol: "http"},

    { name: "SRS", compilation: "AOT", url: "http://localhost:13001/", protocol: "http"},
    { name: "FUS", compilation: "AOT", url: "http://localhost:13002/", protocol: "http"},
    { name: "FUS", compilation: "AOT", url: "http://localhost:13102/", protocol: "grpc"},
    { name: "EPS", compilation: "AOT", url: "http://localhost:13003/", protocol: "http"},
    { name: "BPS", compilation: "AOT", url: "http://localhost:13004/", protocol: "http"},

    // { name: "SRS-JIT", compilation: "JIT", url: "http://srs-jit-service:8080/", protocol: "http" },
    // { name: "SRS-AOT", compilation: "AOT", url: "http://srs-aot-service:8080/", protocol: "http" },

    // { name: "FUS-JIT", compilation: "JIT", url: "http://fus-jit-service:8080/", protocol: "http" },
    // { name: "FUS-JIT", compilation: "JIT", url: "http://fus-jit-service:8081/", protocol: "grpc" },
    // { name: "FUS-AOT", compilation: "AOT", url: "http://fus-aot-service:8080/", protocol: "http" },
    // { name: "FUS-AOT", compilation: "AOT", url: "http://fus-aot-service:8081/", protocol: "grpc" },

    // { name: "BPS-JIT", compilation: "JIT", url: "http://bps-jit-service:8080/", protocol: "http" },
    // { name: "BPS-AOT", compilation: "AOT", url: "http://bps-aot-service:8080/", protocol: "http" },

    // { name: "EPS-JIT", compilation: "JIT", url: "http://eps-jit-service:8080/", protocol: "http" },
    // { name: "EPS-AOT", compilation: "AOT", url: "http://eps-aot-service:8080/", protocol: "http" }
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
  