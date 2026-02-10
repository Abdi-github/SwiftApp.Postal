using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftApp.Postal.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = "2026-04-12 00:00:00+00";
            var system = "system";

            // ── Roles ────────────────────────────────────────────────
            migrationBuilder.Sql($"""
                INSERT INTO roles ("Id", name, description, created_at, created_by)
                VALUES
                  ('a1000000-0000-0000-0000-000000000001', 'ADMIN',          'Full system access',                      '{now}', '{system}'),
                  ('a1000000-0000-0000-0000-000000000002', 'BRANCH_MANAGER', 'Manages a specific post office branch',   '{now}', '{system}'),
                  ('a1000000-0000-0000-0000-000000000003', 'EMPLOYEE',       'Postal worker — parcels and deliveries',  '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ── Permissions ──────────────────────────────────────────
            migrationBuilder.Sql($"""
                INSERT INTO permissions ("Id", name, description, created_at, created_by)
                VALUES
                  ('b1000000-0000-0000-0000-000000000001', 'PARCEL_VIEW',     'View parcels',            '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000002', 'PARCEL_CREATE',   'Create parcels',          '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000003', 'PARCEL_UPDATE',   'Update parcels',          '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000004', 'PARCEL_DELETE',   'Delete parcels',          '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000005', 'BRANCH_VIEW',     'View branches',           '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000006', 'BRANCH_CREATE',   'Create branches',         '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000007', 'BRANCH_UPDATE',   'Update branches',         '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000008', 'BRANCH_DELETE',   'Delete branches',         '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000009', 'DELIVERY_VIEW',   'View deliveries',         '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000010', 'DELIVERY_CREATE', 'Create deliveries',       '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000011', 'DELIVERY_UPDATE', 'Update deliveries',       '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000012', 'TRACKING_VIEW',   'View tracking',           '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000013', 'TRACKING_RECORD', 'Record tracking events',  '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000014', 'EMPLOYEE_VIEW',   'View employees',          '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000015', 'EMPLOYEE_CREATE', 'Create employees',        '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000016', 'EMPLOYEE_UPDATE', 'Update employees',        '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000017', 'EMPLOYEE_DELETE', 'Delete employees',        '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000018', 'CUSTOMER_VIEW',   'View customers',          '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000019', 'CUSTOMER_CREATE', 'Create customers',        '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000020', 'CUSTOMER_UPDATE', 'Update customers',        '{now}', '{system}'),
                  ('b1000000-0000-0000-0000-000000000021', 'CUSTOMER_DELETE', 'Delete customers',        '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ADMIN: all 21 permissions
            migrationBuilder.Sql("""
                INSERT INTO role_permissions (role_id, permission_id)
                SELECT 'a1000000-0000-0000-0000-000000000001', "Id" FROM permissions
                  WHERE "Id"::text LIKE 'b1000000%'
                ON CONFLICT DO NOTHING;
                """);

            // BRANCH_MANAGER: all except BRANCH_DELETE, EMPLOYEE_DELETE, CUSTOMER_DELETE
            migrationBuilder.Sql("""
                INSERT INTO role_permissions (role_id, permission_id)
                SELECT 'a1000000-0000-0000-0000-000000000002', "Id" FROM permissions
                  WHERE "Id"::text LIKE 'b1000000%'
                    AND "Id" NOT IN (
                      'b1000000-0000-0000-0000-000000000008',
                      'b1000000-0000-0000-0000-000000000017',
                      'b1000000-0000-0000-0000-000000000021'
                    )
                ON CONFLICT DO NOTHING;
                """);

            // EMPLOYEE: operational permissions only
            migrationBuilder.Sql("""
                INSERT INTO role_permissions (role_id, permission_id)
                VALUES
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000001'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000002'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000003'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000009'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000010'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000011'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000012'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000013'),
                  ('a1000000-0000-0000-0000-000000000003', 'b1000000-0000-0000-0000-000000000018')
                ON CONFLICT DO NOTHING;
                """);

            // ── Branches ─────────────────────────────────────────────
            migrationBuilder.Sql($"""
                INSERT INTO branches ("Id", branch_code, type, status, street, zip_code, city, canton, phone, email, latitude, longitude, created_at, created_by)
                VALUES
                  ('c1000000-0000-0000-0000-000000000001', 'CH-ZH-001', 'PostOffice',    'Active', 'Kasernenstrasse 95',   '8004', 'Zuerich', 'ZH', '+41 58 667 60 00', 'zuerich@swiftapp.ch', 47.3769, 8.5417, '{now}', '{system}'),
                  ('c1000000-0000-0000-0000-000000000002', 'CH-BE-001', 'PostOffice',    'Active', 'Schanzenstrasse 4',    '3000', 'Bern',    'BE', '+41 58 667 20 00', 'bern@swiftapp.ch',    46.9471, 7.4474, '{now}', '{system}'),
                  ('c1000000-0000-0000-0000-000000000003', 'CH-GE-001', 'PostOffice',    'Active', 'Rue de Rive 2',        '1204', 'Geneve',  'GE', '+41 58 667 35 00', 'geneve@swiftapp.ch',  46.2044, 6.1432, '{now}', '{system}'),
                  ('c1000000-0000-0000-0000-000000000004', 'CH-BS-001', 'SortingCenter', 'Active', 'Hochbergerstrasse 40', '4057', 'Basel',   'BS', '+41 58 667 41 00', 'basel@swiftapp.ch',   47.5596, 7.5886, '{now}', '{system}'),
                  ('c1000000-0000-0000-0000-000000000005', 'CH-TI-001', 'PostOffice',    'Active', 'Via Pretorio 11',      '6900', 'Lugano',  'TI', '+41 58 667 55 00', 'lugano@swiftapp.ch',  45.9920, 8.9580, '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ── Branch Translations ──────────────────────────────────
            migrationBuilder.Sql($"""
                INSERT INTO branch_translations ("Id", branch_id, locale, name, description, created_at, created_by)
                VALUES
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000001', 'de', 'Postfiliale Zuerich HB',       'Hauptpostfiliale im Zuercher HB',                   '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000001', 'fr', 'Bureau de poste Zurich gare', 'Bureau de poste principal a la gare de Zurich',     '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000001', 'it', 'Ufficio postale Zurigo',      'Ufficio postale principale alla stazione di Zurigo', '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000001', 'en', 'Zurich Main Station PO',      'Main post office at Zurich central station',         '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000002', 'de', 'Postfiliale Bern HB',         'Hauptpostfiliale im Berner Bahnhof',                 '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000002', 'fr', 'Bureau de poste Berne gare',  'Bureau de poste principal a la gare de Berne',      '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000002', 'it', 'Ufficio postale Berna',       'Ufficio postale alla stazione di Berna',             '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000002', 'en', 'Bern Main Station PO',        'Post office at Bern central station',                '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000003', 'de', 'Postfiliale Genf Zentrum',    'Postfiliale im Genfer Stadtzentrum',                 '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000003', 'fr', 'Bureau de poste Geneve',      'Bureau de poste au centre-ville de Geneve',         '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000003', 'it', 'Ufficio postale Ginevra',     'Ufficio postale nel centro di Ginevra',              '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000003', 'en', 'Geneva City Centre PO',       'Post office in central Geneva',                     '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000004', 'de', 'Sortierzentrum Basel',        'Regionales Paketsortierzentrum Basel',               '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000004', 'fr', 'Centre de tri Bale',          'Centre regional de tri des colis a Bale',           '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000004', 'it', 'Centro smistamento Basilea',  'Centro regionale di smistamento pacchi a Basilea',  '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000004', 'en', 'Basel Sorting Centre',        'Regional parcel sorting centre in Basel',            '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000005', 'de', 'Postfiliale Lugano Centro',   'Postfiliale im Zentrum von Lugano',                  '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000005', 'fr', 'Bureau de poste Lugano',      'Bureau de poste au centre de Lugano',               '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000005', 'it', 'Ufficio postale Lugano',      'Ufficio postale nel centro di Lugano',               '{now}', '{system}'),
                  (gen_random_uuid(), 'c1000000-0000-0000-0000-000000000005', 'en', 'Lugano City Centre PO',       'Post office in central Lugano',                     '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ── Employees (linked to Keycloak users) ─────────────────
            migrationBuilder.Sql($"""
                INSERT INTO employees ("Id", employee_number, first_name, last_name, email, role, status, assigned_branch_id, hire_date, preferred_locale, keycloak_user_id, created_at, created_by)
                VALUES
                  ('d1000000-0000-0000-0000-000000000001', 'E001', 'Hans',   'Mueller',  'admin@swiftapp.ch',         'Admin',         'Active', NULL,                                   '2020-01-01', 'de', '9267a650-02a9-40fd-af50-62def3b85db8', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000002', 'E002', 'Marie',  'Dupont',   'marie.dupont@swiftapp.ch',  'BranchManager', 'Active', 'c1000000-0000-0000-0000-000000000001', '2021-03-15', 'fr', '040c5b2c-f47a-4127-b47c-9209c9d6053a', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000003', 'E003', 'Luca',   'Rossi',    'luca.rossi@swiftapp.ch',    'BranchManager', 'Active', 'c1000000-0000-0000-0000-000000000002', '2021-06-01', 'it', '279ced3e-eeda-4e96-b83c-b631b2b5c2e9', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000004', 'E004', 'Sophie', 'Meier',    'sophie.meier@swiftapp.ch',  'BranchManager', 'Active', 'c1000000-0000-0000-0000-000000000003', '2022-02-01', 'de', '5b674e15-c532-49bc-ac11-b897a3c4526d', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000005', 'E005', 'Thomas', 'Keller',   'thomas.keller@swiftapp.ch', 'Employee',      'Active', 'c1000000-0000-0000-0000-000000000001', '2022-09-01', 'de', '0108ee34-dbf3-4468-8192-a1b08702c681', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000006', 'E006', 'Anna',   'Fischer',  'anna.fischer@swiftapp.ch',  'Employee',      'Active', 'c1000000-0000-0000-0000-000000000001', '2023-01-15', 'de', '32191eb1-16ae-4e5c-8489-510a0a01dbad', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000007', 'E007', 'Marco',  'Bianchi',  'marco.bianchi@swiftapp.ch', 'Employee',      'Active', 'c1000000-0000-0000-0000-000000000002', '2023-04-01', 'it', '08535f5e-7c01-40d7-ae8f-09ec806a0120', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000008', 'E008', 'Elena',  'Weber',    'elena.weber@swiftapp.ch',   'Employee',      'Active', 'c1000000-0000-0000-0000-000000000002', '2023-07-01', 'de', 'e279c31f-30b1-4239-9ee4-e84af96a8a5a', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000009', 'E009', 'David',  'Brunner',  'david.brunner@swiftapp.ch', 'Employee',      'Active', 'c1000000-0000-0000-0000-000000000003', '2023-10-01', 'de', 'c3ffdad0-7478-4765-939f-3da4af70f036', '{now}', '{system}'),
                  ('d1000000-0000-0000-0000-000000000010', 'E010', 'Laura',  'Schmid',   'laura.schmid@swiftapp.ch',  'Employee',      'Active', 'c1000000-0000-0000-0000-000000000005', '2024-02-01', 'de', 'd74f9bd6-fe08-431e-b659-bed1d12d9568', '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ── Swiss Addresses ───────────────────────────────────────
            migrationBuilder.Sql($"""
                INSERT INTO swiss_addresses ("Id", zip_code, city, canton, municipality, created_at, created_by)
                VALUES
                  (gen_random_uuid(), '8001', 'Zuerich',      'ZH', 'Zuerich',      '{now}', '{system}'),
                  (gen_random_uuid(), '8002', 'Zuerich',      'ZH', 'Zuerich',      '{now}', '{system}'),
                  (gen_random_uuid(), '8004', 'Zuerich',      'ZH', 'Zuerich',      '{now}', '{system}'),
                  (gen_random_uuid(), '8400', 'Winterthur',   'ZH', 'Winterthur',   '{now}', '{system}'),
                  (gen_random_uuid(), '8500', 'Frauenfeld',   'TG', 'Frauenfeld',   '{now}', '{system}'),
                  (gen_random_uuid(), '3001', 'Bern',         'BE', 'Bern',         '{now}', '{system}'),
                  (gen_random_uuid(), '3012', 'Bern',         'BE', 'Bern',         '{now}', '{system}'),
                  (gen_random_uuid(), '2502', 'Biel Bienne',  'BE', 'Biel Bienne',  '{now}', '{system}'),
                  (gen_random_uuid(), '3400', 'Burgdorf',     'BE', 'Burgdorf',     '{now}', '{system}'),
                  (gen_random_uuid(), '1200', 'Geneve',       'GE', 'Geneve',       '{now}', '{system}'),
                  (gen_random_uuid(), '1201', 'Geneve',       'GE', 'Geneve',       '{now}', '{system}'),
                  (gen_random_uuid(), '1204', 'Geneve',       'GE', 'Geneve',       '{now}', '{system}'),
                  (gen_random_uuid(), '1003', 'Lausanne',     'VD', 'Lausanne',     '{now}', '{system}'),
                  (gen_random_uuid(), '1004', 'Lausanne',     'VD', 'Lausanne',     '{now}', '{system}'),
                  (gen_random_uuid(), '1800', 'Vevey',        'VD', 'Vevey',        '{now}', '{system}'),
                  (gen_random_uuid(), '4001', 'Basel',        'BS', 'Basel',        '{now}', '{system}'),
                  (gen_random_uuid(), '4051', 'Basel',        'BS', 'Basel',        '{now}', '{system}'),
                  (gen_random_uuid(), '6003', 'Luzern',       'LU', 'Luzern',       '{now}', '{system}'),
                  (gen_random_uuid(), '6004', 'Luzern',       'LU', 'Luzern',       '{now}', '{system}'),
                  (gen_random_uuid(), '6900', 'Lugano',       'TI', 'Lugano',       '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ── Notification Templates ────────────────────────────────
            migrationBuilder.Sql($"""
                INSERT INTO notification_templates ("Id", template_code, type, event_type, created_at, created_by)
                VALUES
                  ('e1000000-0000-0000-0000-000000000001', 'PARCEL_CREATED',   'Email', 'ParcelCreated',   '{now}', '{system}'),
                  ('e1000000-0000-0000-0000-000000000002', 'PARCEL_DELIVERED', 'Email', 'ParcelDelivered', '{now}', '{system}'),
                  ('e1000000-0000-0000-0000-000000000003', 'DELIVERY_FAILED',  'Email', 'DeliveryFailed',  '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ── Notification Template Translations ────────────────────
            // Note: Scriban template variables use {{variable}} syntax.
            // In C# interpolated strings, {{ produces a literal { so {{variable}} renders as {variable} in the SQL.
            var t1 = "e1000000-0000-0000-0000-000000000001";
            var t2 = "e1000000-0000-0000-0000-000000000002";
            var t3 = "e1000000-0000-0000-0000-000000000003";
            var col = "notification_template_id, locale, subject, body, created_at, created_by";

            migrationBuilder.Sql(
                "INSERT INTO notification_template_translations (\"Id\", " + col + ") VALUES " +
                $"(gen_random_uuid(), '{t1}', 'de', 'Ihr Paket {{{{tracking_number}}}} wurde registriert', 'Sehr geehrte/r {{{{customer_name}}}}, Ihr Paket {{{{tracking_number}}}} wurde erfolgreich registriert. Mit freundlichen Gruessen SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t1}', 'fr', 'Votre colis {{{{tracking_number}}}} a ete enregistre', 'Cher/Chere {{{{customer_name}}}}, Votre colis {{{{tracking_number}}}} a ete enregistre avec succes. Cordialement, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t1}', 'it', 'Il suo pacco {{{{tracking_number}}}} e stato registrato', 'Gentile {{{{customer_name}}}}, Il suo pacco {{{{tracking_number}}}} e stato registrato. Cordiali saluti, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t1}', 'en', 'Your parcel {{{{tracking_number}}}} has been registered', 'Dear {{{{customer_name}}}}, Your parcel {{{{tracking_number}}}} has been successfully registered. Kind regards, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t2}', 'de', 'Ihr Paket {{{{tracking_number}}}} wurde zugestellt', 'Sehr geehrte/r {{{{customer_name}}}}, Ihr Paket {{{{tracking_number}}}} wurde erfolgreich zugestellt. Mit freundlichen Gruessen SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t2}', 'fr', 'Votre colis {{{{tracking_number}}}} a ete livre', 'Cher/Chere {{{{customer_name}}}}, Votre colis {{{{tracking_number}}}} a ete livre. Cordialement, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t2}', 'it', 'Il suo pacco {{{{tracking_number}}}} e stato consegnato', 'Gentile {{{{customer_name}}}}, Il suo pacco {{{{tracking_number}}}} e stato consegnato. Cordiali saluti, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t2}', 'en', 'Your parcel {{{{tracking_number}}}} delivered', 'Dear {{{{customer_name}}}}, Your parcel {{{{tracking_number}}}} has been delivered. Kind regards, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t3}', 'de', 'Zustellung von {{{{tracking_number}}}} fehlgeschlagen', 'Sehr geehrte/r {{{{customer_name}}}}, Paket {{{{tracking_number}}}} konnte nicht zugestellt werden. Bitte holen Sie es in unserer Filiale ab. Mit freundlichen Gruessen SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t3}', 'fr', 'Echec livraison {{{{tracking_number}}}}', 'Cher/Chere {{{{customer_name}}}}, Votre colis {{{{tracking_number}}}} n''a pas pu etre livre. Veuillez le recuperer dans notre agence. Cordialement, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t3}', 'it', 'Consegna {{{{tracking_number}}}} fallita', 'Gentile {{{{customer_name}}}}, Il pacco {{{{tracking_number}}}} non e stato consegnato. Ritirarlo presso la nostra filiale. Cordiali saluti, SwiftApp Postal', '{now}', '{system}')," +
                $"(gen_random_uuid(), '{t3}', 'en', 'Delivery of {{{{tracking_number}}}} failed', 'Dear {{{{customer_name}}}}, Parcel {{{{tracking_number}}}} could not be delivered. Please collect it from our branch. Kind regards, SwiftApp Postal', '{now}', '{system}') " +
                "ON CONFLICT DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM notification_template_translations WHERE notification_template_id::text LIKE 'e1000000%';");
            migrationBuilder.Sql("DELETE FROM notification_templates WHERE \"Id\"::text LIKE 'e1000000%';");
            migrationBuilder.Sql("DELETE FROM swiss_addresses WHERE created_by = 'system';");
            migrationBuilder.Sql("DELETE FROM employees WHERE \"Id\"::text LIKE 'd1000000%';");
            migrationBuilder.Sql("DELETE FROM branch_translations WHERE branch_id::text LIKE 'c1000000%';");
            migrationBuilder.Sql("DELETE FROM branches WHERE \"Id\"::text LIKE 'c1000000%';");
            migrationBuilder.Sql("DELETE FROM role_permissions WHERE role_id::text LIKE 'a1000000%';");
            migrationBuilder.Sql("DELETE FROM permissions WHERE \"Id\"::text LIKE 'b1000000%';");
            migrationBuilder.Sql("DELETE FROM roles WHERE \"Id\"::text LIKE 'a1000000%';");
        }
    }
}
