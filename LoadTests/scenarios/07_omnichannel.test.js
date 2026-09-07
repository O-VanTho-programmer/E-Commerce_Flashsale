import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, DEFAULT_HEADERS, THRESHOLDS } from '../config.js';
import { generateUUID, safeJsonParse } from '../utils/helpers.js';

export const options = {
  stages: [
    { duration: '5s', target: 15 },
    { duration: '10s', target: 40 },
    { duration: '5s', target: 0 },
  ],
  thresholds: THRESHOLDS.standard,
};

export default function () {
  // Generate distinct external Shopee order ID
  const externalOrderId = `SHP-${generateUUID().substring(0, 8)}`;
  const sku = __ENV.TARGET_SKU || 'SNK-42';

  const payload = JSON.stringify({
    orderId: externalOrderId,
    itemSku: sku,
    quantity: 1,
  });

  const res = http.post(ENDPOINTS.SHOPEE_WEBHOOK, payload, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'ShopeeOrderWebhook' },
  });

  const body = safeJsonParse(res.body);

  check(res, {
    'shopee webhook returns 200 or clean error': (r) => r.status === 200 || r.status === 400,
    'shopee webhook returns expected json structure': () => {
      return body !== null && typeof body === 'object' && ('code' in body);
    },
  });

  // Test immediate duplicate delivery of the same Shopee Order ID (Idempotency)
  const duplicateRes = http.post(ENDPOINTS.SHOPEE_WEBHOOK, payload, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'ShopeeOrderWebhookDuplicate' },
  });

  check(duplicateRes, {
    'duplicate shopee webhook handled gracefully without 500': (r) => r.status !== 500,
  });

  sleep(0.5);
}
