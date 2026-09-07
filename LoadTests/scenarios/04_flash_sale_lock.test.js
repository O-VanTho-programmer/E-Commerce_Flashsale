import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, THRESHOLDS } from '../config.js';
import { registerOrLoginUser, getAuthHeaders } from '../utils/auth.js';
import {
  generateRandomEmail,
  lockBusyRate,
  outOfStockRate,
  successfulReservations,
} from '../utils/helpers.js';

export const options = {
  // High-concurrency spike: 50 concurrent VUs simultaneously contending for the same item
  scenarios: {
    flash_sale_rush: {
      executor: 'ramping-vus',
      startVUs: 1,
      stages: [
        { duration: '3s', target: 20 },  // Immediate spike to 20 users
        { duration: '8s', target: 60 },  // Peak rush with 60 simultaneous users
        { duration: '3s', target: 0 },   // Cool down
      ],
      gracefulRampDown: '2s',
    },
  },
  thresholds: {
    // 500 Internal Server Error must NEVER occur under lock contention
    'http_req_failed{status:500}': ['rate==0'],
  },
};

export default function () {
  // 1. Each VU gets its own authenticated user session
  const email = generateRandomEmail('rush_buyer');
  const token = registerOrLoginUser(email);

  if (!token) {
    return;
  }

  const authHeaders = getAuthHeaders(token);

  // 2. All VUs target the same flash sale variant ID (e.g. Variant 1)
  const targetVariantId = __ENV.TARGET_VARIANT_ID ? parseInt(__ENV.TARGET_VARIANT_ID) : 1;

  const payload = JSON.stringify({
    productVariantId: targetVariantId,
    quantity: 1,
    isFlashSale: true, // Triggers Redis Distributed Lock (RedLock) and 15-min StockReservation
  });

  // 3. Fire the reservation request
  const res = http.post(ENDPOINTS.CART_ITEMS, payload, {
    headers: authHeaders,
    tags: { name: 'FlashSaleAddToCart' },
    responseCallback: http.expectedStatuses(200, 400),
  });

  // 4. Validate Distributed Lock & Concurrency Responses
  const is200 = res.status === 200;
  const is400 = res.status === 400;
  const is500 = res.status >= 500;

  check(res, {
    'no 500 internal server error': () => !is500,
    'status is 200 (reserved) or 400 (lock/stock rejection)': () => is200 || is400,
  });

  if (is200) {
    successfulReservations.add(1);
  } else if (is400) {
    const errorText = res.body || '';
    if (errorText.includes('busy') || errorText.includes('Too many people')) {
      // Redis Distributed Lock timeout/busy rejection
      lockBusyRate.add(1);
    } else if (errorText.includes('stock') || errorText.includes('Not enough')) {
      // Stock sold out / inventory exhausted
      outOfStockRate.add(1);
    }
  }

  sleep(0.3);
}
