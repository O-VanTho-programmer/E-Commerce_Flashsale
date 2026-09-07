import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, THRESHOLDS } from '../config.js';
import { registerOrLoginUser, getAuthHeaders } from '../utils/auth.js';
import { generateRandomEmail, ordersPlaced, checkoutDuration, safeJsonParse } from '../utils/helpers.js';

export const options = {
  stages: [
    { duration: '5s', target: 10 },
    { duration: '15s', target: 25 },
    { duration: '5s', target: 0 },
  ],
  thresholds: {
    'http_req_failed{status:500}': ['rate==0'],
    checkout_duration_ms: ['p(95)<500'],
  },
};

export default function () {
  // 1. User signs up
  const email = generateRandomEmail('checkout_vu');
  const token = registerOrLoginUser(email);

  if (!token) {
    return;
  }

  const authHeaders = getAuthHeaders(token);
  const targetVariantId = __ENV.TARGET_VARIANT_ID ? parseInt(__ENV.TARGET_VARIANT_ID) : 1;

  // 2. Add an item to cart first
  const addRes = http.post(
    ENDPOINTS.CART_ITEMS,
    JSON.stringify({
      productVariantId: targetVariantId,
      quantity: 1,
      isFlashSale: false,
    }),
    { headers: authHeaders, tags: { name: 'AddToCartBeforeCheckout' } }
  );

  if (addRes.status !== 200) {
    // If out of stock or variant doesn't exist, proceed to next iteration
    return;
  }

  sleep(0.5);

  // 3. Place the order
  const startOrderTime = Date.now();
  const orderRes = http.post(ENDPOINTS.ORDERS, null, {
    headers: authHeaders,
    tags: { name: 'PlaceOrder' },
  });
  checkoutDuration.add(Date.now() - startOrderTime);

  const orderSuccess = check(orderRes, {
    'place order status is 200': (r) => r.status === 200,
    'place order returns valid order id': (r) => {
      const orderId = safeJsonParse(r.body);
      return typeof orderId === 'number' && orderId > 0;
    },
  });

  if (orderSuccess) {
    ordersPlaced.add(1);
    const orderId = safeJsonParse(orderRes.body);

    sleep(0.5);

    // 4. Optionally test order cancellation
    if (__ENV.TEST_CANCEL === 'true' && orderId) {
      const cancelRes = http.post(`${ENDPOINTS.ORDERS}/${orderId}/cancel`, null, {
        headers: authHeaders,
        tags: { name: 'CancelOrder' },
      });

      check(cancelRes, {
        'cancel order returns 200 or 204': (r) => r.status === 200 || r.status === 204,
      });
    }
  }

  sleep(1);
}
