import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, DEFAULT_HEADERS, THRESHOLDS } from '../config.js';
import { generateRandomEmail } from '../utils/helpers.js';

export const options = {
  stages: [
    { duration: '5s', target: 20 },  // Ramp up to 20 users
    { duration: '10s', target: 50 }, // Ramp up to 50 users
    { duration: '5s', target: 0 },   // Ramp down
  ],
  thresholds: THRESHOLDS.standard,
};

export default function () {
  const email = generateRandomEmail('auth_test');
  const password = 'StrongPassword123!';

  // 1. Register a new user
  const registerPayload = JSON.stringify({
    email: email,
    password: password,
    role: 0, // Customer
  });

  const regRes = http.post(ENDPOINTS.REGISTER, registerPayload, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'RegisterUser' },
  });

  check(regRes, {
    'register status is 200': (r) => r.status === 200,
    'register returns jwt token': (r) => r.body && r.body.length > 20,
  });

  sleep(0.5);

  // 2. Login with the created credentials
  const loginPayload = JSON.stringify({
    email: email,
    password: password,
  });

  const loginRes = http.post(ENDPOINTS.LOGIN, loginPayload, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'LoginUser' },
  });

  check(loginRes, {
    'login status is 200': (r) => r.status === 200,
    'login returns jwt token': (r) => r.body && r.body.length > 20,
  });

  sleep(0.5);

  // 3. Negative check: Invalid login
  const invalidLoginRes = http.post(
    ENDPOINTS.LOGIN,
    JSON.stringify({ email: email, password: 'WrongPassword!' }),
    {
      headers: DEFAULT_HEADERS,
      tags: { name: 'InvalidLogin' },
    }
  );

  check(invalidLoginRes, {
    'invalid login returns 400 Bad Request': (r) => r.status === 400,
  });

  sleep(1);
}
