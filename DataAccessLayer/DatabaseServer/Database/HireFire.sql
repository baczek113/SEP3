-- =========================================================
-- HireFire Database Schema (PostgreSQL) — v2 (with User base, unquoted name)
-- =========================================================

-- Create and use schema
CREATE SCHEMA IF NOT EXISTS HireFire;
SET search_path TO HireFire;

-- ---------- ENUM TYPES ----------
CREATE TYPE user_role            AS ENUM ('admin','company_representative','recruiter','applicant');
CREATE TYPE skill_level          AS ENUM ('beginner','junior','mid','senior','expert');
CREATE TYPE application_status   AS ENUM ('under_review','matched','declined');
CREATE TYPE job_skill_priority   AS ENUM ('must','nice');

-- ---------- BASE USER ----------
CREATE TABLE users (
  id        BIGSERIAL PRIMARY KEY,
  email     VARCHAR(255) UNIQUE NOT NULL,
  password  TEXT NOT NULL,
  role      user_role NOT NULL,
  name      VARCHAR(120) NOT NULL
);

-- ---------- SUBTYPES (is-a User) ----------
CREATE TABLE admin (
  user_id   BIGINT PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE company_representative (
  user_id   BIGINT PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
  position  VARCHAR(120)
);

CREATE TABLE recruiter (
  user_id   BIGINT PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
  position  VARCHAR(120),
  hired_by  BIGINT REFERENCES company_representative(user_id) ON DELETE SET NULL,
  works_in  BIGINT  -- FK to company(id); added later
);

CREATE TABLE applicant (
  user_id     BIGINT PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
  experience  TEXT,
  location_id BIGINT  -- FK to location(id); added later
);

-- ---------- SUPPORTING ENTITIES ----------
CREATE TABLE location (
  id        BIGSERIAL PRIMARY KEY,
  city      VARCHAR(120) NOT NULL,
  postcode  VARCHAR(20),
  address   VARCHAR(255)
);

CREATE TABLE company (
  id          BIGSERIAL PRIMARY KEY,
  name        VARCHAR(255) NOT NULL,
  description TEXT,
  website     VARCHAR(255),
  is_approved BOOLEAN NOT NULL DEFAULT FALSE,
  cr_id       BIGINT REFERENCES company_representative(user_id) ON DELETE SET NULL,
  approved_by BIGINT REFERENCES admin(user_id) ON DELETE SET NULL,
  location_id BIGINT REFERENCES location(id) ON DELETE SET NULL
);

-- Wire foreign keys now that company and location exist
ALTER TABLE recruiter
  ADD CONSTRAINT fk_recruiter_works_in
  FOREIGN KEY (works_in) REFERENCES company(id) ON DELETE CASCADE;

ALTER TABLE applicant
  ADD CONSTRAINT fk_applicant_location
  FOREIGN KEY (location_id) REFERENCES location(id) ON DELETE RESTRICT;

CREATE TABLE skill (
  id        BIGSERIAL PRIMARY KEY,
  name      VARCHAR(120) NOT NULL,
  category  VARCHAR(120)
);

-- ---------- JOBS ----------
CREATE TABLE job_listing (
  id            BIGSERIAL PRIMARY KEY,
  title         VARCHAR(255) NOT NULL,
  description   TEXT,
  date_posted   TIMESTAMPTZ NOT NULL DEFAULT now(),
  salary        NUMERIC(12,2),
  company_id    BIGINT NOT NULL REFERENCES company(id) ON DELETE CASCADE,
  location_id   BIGINT NOT NULL REFERENCES location(id) ON DELETE RESTRICT,
  posted_by_id  BIGINT REFERENCES recruiter(user_id) ON DELETE SET NULL
);

CREATE TABLE job_listing_skills (
  job_id    BIGINT NOT NULL REFERENCES job_listing(id) ON DELETE CASCADE,
  skill_id  BIGINT NOT NULL REFERENCES skill(id) ON DELETE CASCADE,
  priority  job_skill_priority NOT NULL DEFAULT 'must',
  PRIMARY KEY (job_id, skill_id)
);

-- ---------- APPLICANTS & SKILLS ----------
CREATE TABLE applicant_skill (
  applicant_id BIGINT NOT NULL REFERENCES applicant(user_id) ON DELETE CASCADE,
  skill_id     BIGINT NOT NULL REFERENCES skill(id) ON DELETE CASCADE,
  level        skill_level NOT NULL,
  PRIMARY KEY (applicant_id, skill_id)
);

-- ---------- APPLICATIONS ----------
CREATE TABLE application (
  id            BIGSERIAL PRIMARY KEY,
  job_id        BIGINT NOT NULL REFERENCES job_listing(id) ON DELETE CASCADE,
  applicant_id  BIGINT NOT NULL REFERENCES applicant(user_id) ON DELETE CASCADE,
  submitted_at  TIMESTAMPTZ NOT NULL DEFAULT now(),
  status        application_status NOT NULL DEFAULT 'under_review',
  CONSTRAINT uq_application UNIQUE (job_id, applicant_id)
);

-- ---------- CHATS & MESSAGES ----------
CREATE TABLE chat_thread (
  chat_id        BIGSERIAL PRIMARY KEY,
  application_id BIGINT NOT NULL REFERENCES application(id) ON DELETE CASCADE
);

CREATE TABLE message (
  id         BIGSERIAL PRIMARY KEY,
  chat_id    BIGINT NOT NULL REFERENCES chat_thread(chat_id) ON DELETE CASCADE,
  sender_id  BIGINT NOT NULL REFERENCES users(id) ON DELETE SET NULL,
  body       TEXT NOT NULL,
  sent_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

