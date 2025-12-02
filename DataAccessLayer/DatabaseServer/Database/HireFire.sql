-- =========================================================
-- HireFire Database Schema (PostgreSQL) — v2 (with User base, unquoted name)
-- =========================================================

-- Create and use schema
CREATE SCHEMA IF NOT EXISTS HireFire;
SET search_path TO HireFire;

-- ---------- ENUM TYPES ----------

CREATE TYPE user_role AS ENUM ('admin','company_representative','recruiter','applicant');

-- ---------- BASE USER ----------
CREATE TABLE users (
                       id        BIGSERIAL PRIMARY KEY,
                       email     VARCHAR(255) UNIQUE NOT NULL,
                       password  TEXT NOT NULL,
                       role      user_role NOT NULL,
                       name      VARCHAR(120) NOT NULL
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
                                    id        BIGSERIAL PRIMARY KEY,
                                    job_id    BIGINT NOT NULL REFERENCES job_listing(id) ON DELETE CASCADE,
                                    skill_id  BIGINT NOT NULL REFERENCES skill(id) ON DELETE CASCADE,
                                    priority  VARCHAR(10) NOT NULL DEFAULT 'must',
                                    CONSTRAINT chk_job_skill_priority
                                        CHECK (priority IN ('must', 'nice')),
                                    CONSTRAINT uq_job_skill UNIQUE (job_id, skill_id)
);


-- ---------- APPLICANTS & SKILLS ----------
CREATE TABLE applicant_skill (
                                 id           BIGSERIAL PRIMARY KEY,
                                 applicant_id BIGINT NOT NULL REFERENCES applicant(user_id) ON DELETE CASCADE,
                                 skill_id     BIGINT NOT NULL REFERENCES skill(id) ON DELETE CASCADE,
                                 level        VARCHAR(20) NOT NULL,
                                 CONSTRAINT chk_applicant_skill_level
                                     CHECK (level IN ('beginner','junior','mid','senior','expert'))
);

-- ---------- APPLICATIONS ----------
CREATE TABLE application (
                             id            BIGSERIAL PRIMARY KEY,
                             job_id        BIGINT NOT NULL REFERENCES job_listing(id) ON DELETE CASCADE,
                             applicant_id  BIGINT NOT NULL REFERENCES applicant(user_id) ON DELETE CASCADE,
                             submitted_at  TIMESTAMPTZ NOT NULL DEFAULT now(),
                             status        VARCHAR(20) NOT NULL DEFAULT 'under_review',
                             CONSTRAINT chk_application_status
                                 CHECK (status IN ('under_review','matched','declined')),
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
