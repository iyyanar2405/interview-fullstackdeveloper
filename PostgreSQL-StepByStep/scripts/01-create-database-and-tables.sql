-- Assumes database ecom_training already exists.
-- If not, create it first from psql:
--   CREATE DATABASE ecom_training;

\connect ecom_training

-- Schema
CREATE SCHEMA IF NOT EXISTS sales;

-- Tables
CREATE TABLE IF NOT EXISTS sales.customers (
  customer_id SERIAL PRIMARY KEY,
  first_name VARCHAR(50) NOT NULL,
  last_name VARCHAR(50) NOT NULL,
  email VARCHAR(255) NOT NULL UNIQUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS sales.products (
  product_id SERIAL PRIMARY KEY,
  sku VARCHAR(50) NOT NULL UNIQUE,
  name VARCHAR(200) NOT NULL,
  price NUMERIC(10,2) NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS sales.orders (
  order_id SERIAL PRIMARY KEY,
  customer_id INT NOT NULL REFERENCES sales.customers(customer_id),
  order_date TIMESTAMPTZ NOT NULL DEFAULT now(),
  status VARCHAR(20) NOT NULL DEFAULT 'NEW'
);

CREATE TABLE IF NOT EXISTS sales.order_items (
  order_item_id SERIAL PRIMARY KEY,
  order_id INT NOT NULL REFERENCES sales.orders(order_id),
  product_id INT NOT NULL REFERENCES sales.products(product_id),
  quantity INT NOT NULL CHECK (quantity > 0),
  unit_price NUMERIC(10,2) NOT NULL
);
