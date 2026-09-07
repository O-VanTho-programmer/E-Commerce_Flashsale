import { Rate, Counter, Trend } from 'k6/metrics';

// Custom Metrics
export const lockBusyRate = new Rate('lock_contention_busy_rate');
export const outOfStockRate = new Rate('out_of_stock_rate');
export const successfulReservations = new Counter('successful_reservations');
export const duplicateWebhooksHandled = new Counter('duplicate_webhooks_handled');
export const ordersPlaced = new Counter('orders_placed');
export const checkoutDuration = new Trend('checkout_duration_ms');

/**
 * Generates a unique UUID v4 string
 */
export function generateUUID() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

/**
 * Generates a unique random email
 */
export function generateRandomEmail(prefix = 'k6_user') {
  const timestamp = Date.now();
  const randomPart = Math.floor(Math.random() * 100000);
  return `${prefix}_${timestamp}_${randomPart}@loadtest.local`;
}

/**
 * Random integer between min and max inclusive
 */
export function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

/**
 * Safe JSON parser helper
 */
export function safeJsonParse(data) {
  try {
    return JSON.parse(data);
  } catch (e) {
    return null;
  }
}
