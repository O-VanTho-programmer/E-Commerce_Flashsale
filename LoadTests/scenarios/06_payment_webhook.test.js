import http from 'k6/http';
import { check, sleep } from 'k6';
import { ENDPOINTS, DEFAULT_HEADERS } from '../config.js';
import { generateUUID, duplicateWebhooksHandled, safeJsonParse } from '../utils/helpers.js';

export const options = {
  scenarios: {
    duplicate_webhook_burst: {
      executor: 'per-vu-iterations',
      vus: 10,
      iterations: 5,
      maxDuration: '30s',
    },
  },
  thresholds: {
    'http_req_failed{status:500}': ['rate==0'],
  },
};

export default function () {
  // We simulate multiple webhook calls for the same event to test idempotency
  // VU 1 generates an eventId, or we use a fixed eventId per burst iteration
  const orderId = __ENV.TARGET_ORDER_ID ? parseInt(__ENV.TARGET_ORDER_ID) : 1;
  const duplicateBurstEventId = `pay_evt_fixed_batch_${__VU}`;

  const payload = JSON.stringify({
    eventId: duplicateBurstEventId,
    orderId: orderId,
    provider: 'StripeMock',
    amount: 199.99,
  });

  const res = http.post(ENDPOINTS.PAYMENT_WEBHOOK, payload, {
    headers: DEFAULT_HEADERS,
    tags: { name: 'PaymentWebhook' },
  });

  const body = safeJsonParse(res.body);

  check(res, {
    'webhook returns 200 OK': (r) => r.status === 200,
    'idempotency respected (success or already_processed)': () => {
      if (!body) return false;
      return body.status === 'success' || body.status === 'already_processed';
    },
  });

  if (body && body.status === 'already_processed') {
    duplicateWebhooksHandled.add(1);
  }

  sleep(0.5);
}
