import http from 'k6/http';
import { check } from 'k6';
import { ENDPOINTS, DEFAULT_HEADERS } from '../config.js';

/**
 * Registers a new user and returns their JWT token.
 * Falls back to login if user already exists.
 */
export function registerOrLoginUser(email, password = 'Password123!', role = 0) {
  const registerPayload = JSON.stringify({
    email: email,
    password: password,
    role: role,
  });

  const regRes = http.post(ENDPOINTS.REGISTER, registerPayload, {
    headers: DEFAULT_HEADERS,
  });

  if (regRes.status === 200) {
    let token = regRes.body;
    try {
      // If returned as JSON string or plain string
      token = JSON.parse(token);
    } catch {
      // keep raw string
    }
    return token;
  }

  // If already exists or registration failed, attempt login
  const loginPayload = JSON.stringify({
    email: email,
    password: password,
  });

  const loginRes = http.post(ENDPOINTS.LOGIN, loginPayload, {
    headers: DEFAULT_HEADERS,
  });

  if (loginRes.status === 200) {
    let token = loginRes.body;
    try {
      token = JSON.parse(token);
    } catch {
      // keep raw string
    }
    return token;
  }

  return null;
}

/**
 * Formats Bearer authorization headers
 */
export function getAuthHeaders(token) {
  return {
    ...DEFAULT_HEADERS,
    Authorization: `Bearer ${token}`,
  };
}
