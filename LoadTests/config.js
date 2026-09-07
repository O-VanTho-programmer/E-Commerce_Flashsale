/**
 * Global Configuration for k6 Load Testing Suite
 * Configurable via environment variables (e.g., -e BASE_URL=http://localhost:5235)
 */

export const BASE_URL = __ENV.BASE_URL || 'http://localhost:5235';

export const ENDPOINTS = {
  // Auth
  REGISTER: `${BASE_URL}/api/v1/auth/register`,
  LOGIN: `${BASE_URL}/api/v1/auth/login`,

  // Catalog
  CATEGORIES: `${BASE_URL}/api/v1/catalog/categories`,
  PRODUCTS: `${BASE_URL}/api/v1/catalog/products`,

  // Flash Sale
  ACTIVE_FLASHSALE: `${BASE_URL}/api/v1/flashsale/active`,
  CREATE_FLASHSALE: `${BASE_URL}/api/v1/flashsale`,

  // Cart
  CART: `${BASE_URL}/api/v1/cart`,
  CART_ITEMS: `${BASE_URL}/api/v1/cart/items`,

  // Orders
  ORDERS: `${BASE_URL}/api/v1/order`,

  // Webhooks
  PAYMENT_WEBHOOK: `${BASE_URL}/api/v1/webhooks/payment`,
  SHOPEE_WEBHOOK: `${BASE_URL}/api/v1/webhooks/shopee/orders`,
};

export const DEFAULT_HEADERS = {
  'Content-Type': 'application/json',
  'Accept': 'application/json',
};

export const THRESHOLDS = {
  standard: {
    http_req_failed: ['rate<0.01'], // less than 1% error rate
    http_req_duration: ['p(95)<300'], // 95% of requests under 300ms
  },
  readHeavy: {
    http_req_failed: ['rate<0.005'], // less than 0.5% errors
    http_req_duration: ['p(95)<150'], // 95% of requests under 150ms
  },
  concurrencySpike: {
    // Under high concurrency lock contention, business rejections (400 Bad Request) are expected,
    // but 500 Internal Server Errors MUST be 0.
    'http_req_failed{status:500}': ['rate==0'],
  },
};
