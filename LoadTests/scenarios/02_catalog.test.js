import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, DEFAULT_HEADERS, THRESHOLDS } from '../config.js';

export const options = {
  stages: [
    { duration: '5s', target: 30 },  // Ramp up to 30 VUs
    { duration: '15s', target: 80 }, // Sustained heavy read load
    { duration: '5s', target: 0 },   // Ramp down
  ],
  thresholds: THRESHOLDS.readHeavy,
};

export default function () {
  // 1. Get Products
  const productsRes = http.get(ENDPOINTS.PRODUCTS, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'GetProducts' },
  });

  check(productsRes, {
    'get products status is 200': (r) => r.status === 200,
    'get products returns valid json': (r) => {
      try {
        const body = JSON.parse(r.body);
        return Array.isArray(body);
      } catch {
        return false;
      }
    },
  });

  sleep(0.3);

  // 2. Get Categories
  const categoriesRes = http.get(ENDPOINTS.CATEGORIES, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'GetCategories' },
  });

  check(categoriesRes, {
    'get categories status is 200': (r) => r.status === 200,
    'get categories returns array': (r) => {
      try {
        const body = JSON.parse(r.body);
        return Array.isArray(body);
      } catch {
        return false;
      }
    },
  });

  sleep(0.3);

  // 3. Get Active Flash Sale
  const flashSaleRes = http.get(ENDPOINTS.ACTIVE_FLASHSALE, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'GetActiveFlashSale' },
  });

  check(flashSaleRes, {
    'get active flash sale returns 200 or 204': (r) => r.status === 200 || r.status === 204,
  });

  sleep(0.5);
}
