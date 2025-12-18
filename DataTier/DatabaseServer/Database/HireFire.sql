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
                         cr_id       BIGINT REFERENCES company_representative(user_id) ON DELETE CASCADE,
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
                             posted_by_id  BIGINT REFERENCES recruiter(user_id) ON DELETE SET NULL,
                             is_closed     BOOLEAN NOT NULL DEFAULT FALSE
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
                         sender_id  BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                         body       TEXT NOT NULL,
                         sent_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

INSERT INTO HireFire.users (email, password, role, name)
VALUES (
           'admin@hirefire.local',
           '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
           'admin',
           'Admin'
       );

-- =========================================================
-- HireFire Seed Data (PostgreSQL) — ONLY predefined skills
-- Password for all seeded users: "123" (SHA-256 hex)
-- =========================================================

BEGIN;

SET search_path TO HireFire;

-- -----------------------------
-- 1) SKILLS (ONLY your list)
-- -----------------------------
INSERT INTO skill (id, name, category) VALUES
                                           (1,  'C#',             'Backend'),
                                           (2,  'Java',           'Backend'),
                                           (3,  'Spring Boot',    'Backend'),
                                           (4,  '.NET',           'Backend'),
                                           (5,  'SQL',            'Backend'),
                                           (6,  'PostgreSQL',     'Backend'),

                                           (7,  'JavaScript',     'Frontend'),
                                           (8,  'TypeScript',     'Frontend'),
                                           (9,  'React',          'Frontend'),
                                           (10, 'Blazor',         'Frontend'),
                                           (11, 'HTML/CSS',       'Frontend'),

                                           (12, 'Docker',         'DevOps'),
                                           (13, 'Kubernetes',     'DevOps'),
                                           (14, 'CI/CD',          'DevOps'),
                                           (15, 'Git',            'DevOps'),

                                           (16, 'Problem solving','Soft skills'),
                                           (17, 'Teamwork',       'Soft skills'),
                                           (18, 'Communication',  'Soft skills'),
                                           (19, 'Leadership',     'Soft skills')
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 2) LOCATION (ONLY Horsens)
-- -----------------------------
INSERT INTO location (id, city, postcode, address) VALUES
    (1, 'Horsens', '8700', 'Main Street 1')
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 3) USERS (password = "123" SHA-256)
-- NOTE: admin from your script remains (password "admin")
-- -----------------------------
-- SHA-256("123") = a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3
INSERT INTO users (id, email, password, role, name) VALUES
                                                        -- Company representatives
                                                        (2,  'cr.nordic@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'company_representative', 'Emma Jensen'),
                                                        (3,  'cr.vistula@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'company_representative', 'Piotr Nowak'),
                                                        (4,  'cr.baltic@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'company_representative', 'Sofia Nilsson'),

                                                        -- Recruiters
                                                        (5,  'rec.nordic1@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'recruiter', 'Lars Mikkelsen'),
                                                        (6,  'rec.nordic2@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'recruiter', 'Nina Pedersen'),
                                                        (7,  'rec.vistula1@hirefire.local','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'recruiter', 'Kamil Zielinski'),
                                                        (8,  'rec.vistula2@hirefire.local','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'recruiter', 'Anna Kowalczyk'),
                                                        (9,  'rec.baltic1@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'recruiter', 'Erik Hansen'),
                                                        (10, 'rec.baltic2@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'recruiter', 'Maja Larsen'),

                                                        -- Applicants (12)
                                                        (11, 'app.1@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Jakub Nowicki'),
                                                        (12, 'app.2@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Natalia Wozniak'),
                                                        (13, 'app.3@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Tymoteusz Zydkiewicz'),
                                                        (14, 'app.4@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Vanessa Hututuc'),
                                                        (15, 'app.5@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Yuchen Zhang'),
                                                        (16, 'app.6@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Maria Rossi'),
                                                        (17, 'app.7@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Oliver Smith'),
                                                        (18, 'app.8@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Amelia Brown'),
                                                        (19, 'app.9@hirefire.local',  'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Mateusz Maj'),
                                                        (20, 'app.10@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Julia Kaczmarek'),
                                                        (21, 'app.11@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Kacper Lewandowski'),
                                                        (22, 'app.12@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'applicant', 'Zuzanna Nowak'),

                                                        -- Extra admin for testing (password "123")
                                                        (23, 'admin2@hirefire.local', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'admin', 'Test Admin')
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 4) COMPANY REPRESENTATIVES
-- -----------------------------
INSERT INTO company_representative (user_id, position) VALUES
                                                           (2, 'Head of Talent'),
                                                           (3, 'HR Manager'),
                                                           (4, 'People Operations Lead')
    ON CONFLICT (user_id) DO NOTHING;

-- -----------------------------
-- 5) COMPANIES (all in Horsens -> location_id = 1)
-- -----------------------------
INSERT INTO company (id, name, description, website, is_approved, cr_id, location_id) VALUES
                                                                                          (1, 'NordicCloud', 'Cloud-native software and platform engineering.', 'https://nordiccloud.example', TRUE,  2, 1),
                                                                                          (2, 'VistulaTech', 'Product development studio focused on fintech and SaaS.', 'https://vistulatech.example', TRUE,  3, 1),
                                                                                          (3, 'BalticApps',  'Mobile + web applications for B2B customers.', 'https://balticapps.example', FALSE, 4, 1)
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 6) RECRUITERS
-- -----------------------------
INSERT INTO recruiter (user_id, position, hired_by, works_in) VALUES
                                                                  (5,  'Senior Recruiter', 2, 1),
                                                                  (6,  'Recruiter',        2, 1),
                                                                  (7,  'Tech Recruiter',   3, 2),
                                                                  (8,  'Recruiter',        3, 2),
                                                                  (9,  'Recruiter',        4, 3),
                                                                  (10, 'Junior Recruiter', 4, 3)
    ON CONFLICT (user_id) DO NOTHING;

-- -----------------------------
-- 7) APPLICANTS (all in Horsens -> location_id = 1)
-- -----------------------------
INSERT INTO applicant (user_id, experience, location_id) VALUES
                                                             (11, 'Backend-focused junior developer with strong Java fundamentals.', 1),
                                                             (12, 'Frontend developer with React + TypeScript experience.',          1),
                                                             (13, 'Full-stack candidate interested in .NET and modern web.',         1),
                                                             (14, 'Career switcher, strong soft skills and growing technical stack.',1),
                                                             (15, 'Junior backend developer with SQL and PostgreSQL.',               1),
                                                             (16, 'DevOps-oriented engineer with Docker and CI/CD.',                 1),
                                                             (17, 'Frontend candidate with Blazor curiosity and solid HTML/CSS.',    1),
                                                             (18, 'Backend candidate with Spring Boot and REST API experience.',     1),
                                                             (19, 'Generalist with strong teamwork and communication.',              1),
                                                             (20, 'Java developer with Git workflow and clean code habits.',         1),
                                                             (21, 'React developer, pragmatic problem solver.',                      1),
                                                             (22, 'Junior developer with broad foundations and leadership potential.',1)
    ON CONFLICT (user_id) DO NOTHING;

-- -----------------------------
-- 8) JOB LISTINGS (all in Horsens -> location_id = 1)
-- -----------------------------
INSERT INTO job_listing (id, title, description, salary, company_id, location_id, posted_by_id, is_closed) VALUES
                                                                                                               (1, 'Junior Backend Developer (Java)', 'Work on APIs and services in a collaborative team.', 45000.00, 1, 1, 5,  FALSE),
                                                                                                               (2, 'Frontend Developer (React)',      'Build polished UI with React and TypeScript.',      52000.00, 1, 1, 6,  FALSE),
                                                                                                               (3, 'DevOps Engineer (Docker)',        'Improve CI/CD pipelines and container workflows.',  60000.00, 1, 1, 5,  FALSE),

                                                                                                               (4, 'Full-stack Developer (.NET)',     'Develop web features using .NET and modern tooling.', 58000.00, 2, 1, 7,  FALSE),
                                                                                                               (5, 'Backend Developer (Spring Boot)', 'Create services and integrate with PostgreSQL.',      62000.00, 2, 1, 8,  FALSE),
                                                                                                               (6, 'Junior QA / Support Engineer',    'Support releases and improve developer workflow.',    38000.00, 2, 1, 8,  TRUE),

                                                                                                               (7, 'Frontend Developer (Blazor)',     'Build internal tools with Blazor and strong UX.',     54000.00, 3, 1, 9,  FALSE),
                                                                                                               (8, 'Backend Developer (SQL)',         'Data-driven backend development with SQL focus.',     61000.00, 3, 1, 10, FALSE),
                                                                                                               (9, 'Platform Engineer (Kubernetes)',  'Maintain clusters and deployment processes.',         70000.00, 3, 1, 9,  FALSE)
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 9) JOB LISTING SKILLS (ONLY predefined skills)
-- -----------------------------
INSERT INTO job_listing_skills (id, job_id, skill_id, priority) VALUES
                                                                    (1,  1, 2,  'must'),
                                                                    (2,  1, 3,  'must'),
                                                                    (3,  1, 5,  'nice'),
                                                                    (4,  1, 15, 'nice'),

                                                                    (5,  2, 9,  'must'),
                                                                    (6,  2, 8,  'must'),
                                                                    (7,  2, 11, 'nice'),
                                                                    (8,  2, 18, 'nice'),

                                                                    (9,  3, 12, 'must'),
                                                                    (10, 3, 14, 'must'),
                                                                    (11, 3, 15, 'nice'),
                                                                    (12, 3, 13, 'nice'),

                                                                    (13, 4, 4,  'must'),
                                                                    (14, 4, 10, 'nice'),
                                                                    (15, 4, 5,  'nice'),
                                                                    (16, 4, 15, 'nice'),

                                                                    (17, 5, 3,  'must'),
                                                                    (18, 5, 6,  'must'),
                                                                    (19, 5, 5,  'nice'),
                                                                    (20, 5, 18, 'nice'),

                                                                    (21, 6, 18, 'must'),
                                                                    (22, 6, 17, 'must'),
                                                                    (23, 6, 15, 'nice'),

                                                                    (24, 7, 10, 'must'),
                                                                    (25, 7, 11, 'must'),
                                                                    (26, 7, 4,  'nice'),
                                                                    (27, 7, 16, 'nice'),

                                                                    (28, 8, 5,  'must'),
                                                                    (29, 8, 6,  'must'),
                                                                    (30, 8, 15, 'nice'),

                                                                    (31, 9, 13, 'must'),
                                                                    (32, 9, 12, 'must'),
                                                                    (33, 9, 14, 'nice'),
                                                                    (34, 9, 16, 'nice')
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 10) APPLICANT SKILLS (ONLY predefined skills)
-- -----------------------------
INSERT INTO applicant_skill (id, applicant_id, skill_id, level) VALUES
                                                                    (1,  11, 2,  'junior'),
                                                                    (2,  11, 3,  'beginner'),
                                                                    (3,  11, 15, 'mid'),
                                                                    (4,  11, 17, 'mid'),

                                                                    (5,  12, 9,  'junior'),
                                                                    (6,  12, 8,  'junior'),
                                                                    (7,  12, 11, 'mid'),
                                                                    (8,  12, 18, 'mid'),

                                                                    (9,  13, 4,  'junior'),
                                                                    (10, 13, 10, 'beginner'),
                                                                    (11, 13, 5,  'beginner'),
                                                                    (12, 13, 16, 'mid'),

                                                                    (13, 14, 18, 'expert'),
                                                                    (14, 14, 17, 'expert'),
                                                                    (15, 14, 16, 'mid'),
                                                                    (16, 14, 11, 'beginner'),

                                                                    (17, 15, 5,  'junior'),
                                                                    (18, 15, 6,  'junior'),
                                                                    (19, 15, 2,  'beginner'),
                                                                    (20, 15, 15, 'mid'),

                                                                    (21, 16, 12, 'mid'),
                                                                    (22, 16, 14, 'mid'),
                                                                    (23, 16, 15, 'mid'),
                                                                    (24, 16, 13, 'beginner'),

                                                                    (25, 17, 11, 'mid'),
                                                                    (26, 17, 7,  'junior'),
                                                                    (27, 17, 10, 'beginner'),
                                                                    (28, 17, 18, 'mid'),

                                                                    (29, 18, 3,  'junior'),
                                                                    (30, 18, 2,  'junior'),
                                                                    (31, 18, 6,  'beginner'),
                                                                    (32, 18, 16, 'mid'),

                                                                    (33, 19, 17, 'expert'),
                                                                    (34, 19, 18, 'expert'),
                                                                    (35, 19, 16, 'mid'),
                                                                    (36, 19, 15, 'junior'),

                                                                    (37, 20, 2,  'mid'),
                                                                    (38, 20, 15, 'mid'),
                                                                    (39, 20, 5,  'beginner'),
                                                                    (40, 20, 18, 'mid'),

                                                                    (41, 21, 9,  'mid'),
                                                                    (42, 21, 8,  'junior'),
                                                                    (43, 21, 16, 'mid'),
                                                                    (44, 21, 11, 'mid'),

                                                                    (45, 22, 4,  'beginner'),
                                                                    (46, 22, 2,  'beginner'),
                                                                    (47, 22, 19, 'junior'),
                                                                    (48, 22, 17, 'mid')
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 11) APPLICATIONS
-- -----------------------------
INSERT INTO application (id, job_id, applicant_id, submitted_at, status) VALUES
                                                                             (1,  1, 11, now() - interval '8 days',  'under_review'),
                                                                             (2,  1, 18, now() - interval '7 days',  'matched'),
                                                                             (3,  2, 12, now() - interval '6 days',  'matched'),
                                                                             (4,  2, 21, now() - interval '5 days',  'under_review'),
                                                                             (5,  3, 16, now() - interval '4 days',  'matched'),

                                                                             (6,  4, 13, now() - interval '3 days',  'under_review'),
                                                                             (7,  4, 22, now() - interval '2 days',  'declined'),
                                                                             (8,  5, 18, now() - interval '9 days',  'matched'),
                                                                             (9,  5, 15, now() - interval '1 days',  'under_review'),

                                                                             (10, 7, 17, now() - interval '2 days',  'under_review'),
                                                                             (11, 7, 13, now() - interval '10 days', 'matched'),
                                                                             (12, 8, 15, now() - interval '6 days',  'matched'),
                                                                             (13, 8, 11, now() - interval '1 days',  'declined'),

                                                                             (14, 9, 16, now() - interval '5 days',  'under_review'),
                                                                             (15, 6, 14, now() - interval '4 days',  'under_review'),
                                                                             (16, 6, 19, now() - interval '3 days',  'matched')
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 12) CHAT THREADS (for matched applications)
-- -----------------------------
INSERT INTO chat_thread (chat_id, application_id) VALUES
                                                      (1,  2),
                                                      (2,  3),
                                                      (3,  5),
                                                      (4,  8),
                                                      (5,  11),
                                                      (6,  12),
                                                      (7,  16)
    ON CONFLICT (chat_id) DO NOTHING;

-- -----------------------------
-- 13) MESSAGES (sample)
-- -----------------------------
INSERT INTO message (id, chat_id, sender_id, body, sent_at) VALUES
                                                                (1,  1, 8,  'Hi! Thanks for applying. Are you available for a short call this week?', now() - interval '7 days' + interval '2 hours'),
                                                                (2,  1, 18, 'Yes, I am. Thursday afternoon works best for me.',                 now() - interval '7 days' + interval '3 hours'),

                                                                (3,  2, 6,  'Hello! We liked your profile. Could you share a GitHub link?',      now() - interval '6 days' + interval '1 hours'),
                                                                (4,  2, 12, 'Sure, I will send it in the next message. Thank you!',              now() - interval '6 days' + interval '2 hours'),

                                                                (5,  3, 5,  'Welcome! Let’s talk about your DevOps experience.',                 now() - interval '4 days' + interval '1 hours'),
                                                                (6,  3, 16, 'Great. I have been working with Docker and CI/CD pipelines.',       now() - interval '4 days' + interval '2 hours'),

                                                                (7,  4, 7,  'Hi! Quick question: how comfortable are you with PostgreSQL?',      now() - interval '9 days' + interval '1 hours'),
                                                                (8,  4, 18, 'I use it daily. I can also write optimized queries and indexes.',   now() - interval '9 days' + interval '2 hours'),

                                                                (9,  5, 9,  'Hello! We have a Blazor-based tool. Are you open to a take-home task?', now() - interval '10 days' + interval '1 hours'),
                                                                (10, 5, 13, 'Yes, that sounds good. Please send details.',                       now() - interval '10 days' + interval '2 hours'),

                                                                (11, 6, 10, 'Congrats on the match. When can you start?',                        now() - interval '6 days' + interval '1 hours'),
                                                                (12, 6, 15, 'I can start in 2-3 weeks, depending on paperwork.',                 now() - interval '6 days' + interval '2 hours'),

                                                                (13, 7, 8,  'Nice to meet you. Any questions about the role?',                   now() - interval '3 days' + interval '1 hours'),
                                                                (14, 7, 19, 'Yes, what does the onboarding process look like?',                  now() - interval '3 days' + interval '2 hours')
    ON CONFLICT (id) DO NOTHING;

-- -----------------------------
-- 14) Fix sequences (so next inserts work)
-- -----------------------------
SELECT setval('HireFire.skill_id_seq',              (SELECT COALESCE(MAX(id), 1) FROM skill));
SELECT setval('HireFire.location_id_seq',           (SELECT COALESCE(MAX(id), 1) FROM location));
SELECT setval('HireFire.users_id_seq',              (SELECT COALESCE(MAX(id), 1) FROM users));
SELECT setval('HireFire.company_id_seq',            (SELECT COALESCE(MAX(id), 1) FROM company));
SELECT setval('HireFire.job_listing_id_seq',        (SELECT COALESCE(MAX(id), 1) FROM job_listing));
SELECT setval('HireFire.job_listing_skills_id_seq', (SELECT COALESCE(MAX(id), 1) FROM job_listing_skills));
SELECT setval('HireFire.applicant_skill_id_seq',    (SELECT COALESCE(MAX(id), 1) FROM applicant_skill));
SELECT setval('HireFire.application_id_seq',        (SELECT COALESCE(MAX(id), 1) FROM application));
SELECT setval('HireFire.chat_thread_chat_id_seq',   (SELECT COALESCE(MAX(chat_id), 1) FROM chat_thread));
SELECT setval('HireFire.message_id_seq',            (SELECT COALESCE(MAX(id), 1) FROM message));

COMMIT;
