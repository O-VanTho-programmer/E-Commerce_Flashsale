import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, THRESHOLDS } from '../config.js';
import { registerOrLoginUser, getAuthHeaders } from '../utils/auth.js';
import { generateRandomEmail, safeJsonParse } from '../utils/helpers.js';

export const options = {
  stages: [
    { duration: '5s', target: 15 },
    { duration: '10s', target: 30 },
    { duration: '5s', target: 0 },
  ],
  thresholds: THRESHOLDS.standard,
};

export default function () {
  // 1. Authenticate user per VU iteration
  const email = generateRandomEmail('cart_vu');
  const token = registerOrLoginUser(email);

  if (!token) {
    return;
  }

  const authHeaders = getAuthHeaders(token);

  // 2. Get initial cart
  const cartRes = http.get(ENDPOINTS.CART, {
    headers: authHeaders,
    tags: { name: 'GetCart' },
  });

  check(cartRes, {
    'get cart status is 200 or 204': (r) => r.status === 200 || r.status === 204,
  });

  sleep(0.5);

  // 3. Add standard item to cart (assume variant ID 1 exists or is available)
  const addItemPayload = JSON.stringify({
    productVariantId: 1,
    quantity: 1,
    isFlashSale: false,
  });

  const addItemRes = http.post(ENDPOINTS.CART_ITEMS, addItemPayload, {
    headers: authHeaders,
    tags: { name: 'AddToCart' },
  });

  const isAddOk = check(addItemRes, {
    'add item to cart returns 200 or business error': (r) => r.status === 200 || r.status === 400,
  });

  if (addItemRes.status === 200) {
    const cartItemId = safeJsonParse(addItemRes.body);

    if (cartItemId && typeof cartItemId === 'number') {
      sleep(0.5);

      // 4. Delete item from cart
      const deleteRes = http.del(`${ENDPOINTS.CART_ITEMS}/${cartItemId}`, null, {
        headers: authHeaders,
        tags: { name: 'RemoveFromCart' },
      });

      check(deleteRes, {
        'remove from cart returns 200 or 204': (r) => r.status === 200 || r.status === 204,
      });
    }
  }

  sleep(1);
}
