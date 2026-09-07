import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, DEFAULT_HEADERS } from '../config.js';
import { registerOrLoginUser, getAuthHeaders } from '../utils/auth.js';
import { generateRandomEmail, generateUUID, safeJsonParse } from '../utils/helpers.js';

export const options = {
  scenarios: {
    // 1. Browsers (60% of total activity)
    catalog_browsers: {
      executor: 'ramping-vus',
      startVUs: 5,
      stages: [
        { duration: '5s', target: 25 },
        { duration: '15s', target: 50 },
        { duration: '5s', target: 0 },
      ],
      exec: 'browseCatalog',
    },

    // 2. Flash Sale Hunters (25% of activity - High Concurrency Contention)
    flash_sale_hunters: {
      executor: 'ramping-vus',
      startVUs: 2,
      stages: [
        { duration: '5s', target: 15 },
        { duration: '10s', target: 30 },
        { duration: '5s', target: 0 },
      ],
      exec: 'rushFlashSale',
    },

    // 3. Checkout Users (10% of activity)
    checkout_users: {
      executor: 'ramping-vus',
      startVUs: 1,
      stages: [
        { duration: '5s', target: 5 },
        { duration: '10s', target: 10 },
        { duration: '5s', target: 0 },
      ],
      exec: 'performCheckout',
    },

    // 4. Background Webhooks (5% of activity)
    external_sync: {
      executor: 'constant-vus',
      vus: 2,
      duration: '20s',
      exec: 'syncWebhooks',
    },
  },
  thresholds: {
    'http_req_failed{status:500}': ['rate==0'],
    http_req_duration: ['p(95)<400'],
  },
};

// Default function fallback if CLI overrides scenarios
export default function () {
  browseCatalog();
}

// Scenario 1: Browse Catalog
export function browseCatalog() {
  http.get(ENDPOINTS.PRODUCTS, { headers: DEFAULT_HEADERS });
  sleep(0.3);
  http.get(ENDPOINTS.CATEGORIES, { headers: DEFAULT_HEADERS });
  sleep(0.3);
  http.get(ENDPOINTS.ACTIVE_FLASHSALE, { headers: DEFAULT_HEADERS });
  sleep(0.5);
}

// Scenario 2: Flash Sale Contention
export function rushFlashSale() {
  const email = generateRandomEmail('hunter');
  const token = registerOrLoginUser(email);
  if (!token) return;

  const authHeaders = getAuthHeaders(token);
  const targetVariantId = __ENV.TARGET_VARIANT_ID ? parseInt(__ENV.TARGET_VARIANT_ID) : 1;

  const res = http.post(
    ENDPOINTS.CART_ITEMS,
    JSON.stringify({
      productVariantId: targetVariantId,
      quantity: 1,
      isFlashSale: true,
    }),
    { headers: authHeaders }
  );

  check(res, {
    'rush: no 500 error': (r) => r.status !== 500,
  });

  sleep(0.5);
}

// Scenario 3: Place Order Checkout
export function performCheckout() {
  const email = generateRandomEmail('shopper');
  const token = registerOrLoginUser(email);
  if (!token) return;

  const authHeaders = getAuthHeaders(token);
  const targetVariantId = __ENV.TARGET_VARIANT_ID ? parseInt(__ENV.TARGET_VARIANT_ID) : 1;

  // Add item
  const addRes = http.post(
    ENDPOINTS.CART_ITEMS,
    JSON.stringify({
      productVariantId: targetVariantId,
      quantity: 1,
      isFlashSale: false,
    }),
    { headers: authHeaders }
  );

  if (addRes.status === 200) {
    sleep(0.3);
    const orderRes = http.post(ENDPOINTS.ORDERS, null, { headers: authHeaders });
    check(orderRes, {
      'checkout: order completed or handled cleanly': (r) => r.status === 200 || r.status === 400,
    });
  }

  sleep(1);
}

// Scenario 4: Background Webhooks
export function syncWebhooks() {
  const eventId = `rush_evt_${generateUUID()}`;
  const payload = JSON.stringify({
    eventId: eventId,
    orderId: 1,
    provider: 'StripeMock',
    amount: 99.99,
  });

  http.post(ENDPOINTS.PAYMENT_WEBHOOK, payload, { headers: DEFAULT_HEADERS });
  sleep(1);
}
