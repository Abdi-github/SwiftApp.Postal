using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftApp.Postal.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedOperationalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var system = "system";

            // Branch IDs (from SeedReferenceData)
            var brZurich  = "c1000000-0000-0000-0000-000000000001";
            var brBern    = "c1000000-0000-0000-0000-000000000002";
            var brGeneva  = "c1000000-0000-0000-0000-000000000003";
            var brBasel   = "c1000000-0000-0000-0000-000000000004";
            var brLugano  = "c1000000-0000-0000-0000-000000000005";

            // Employee IDs (from SeedReferenceData)
            // d1..01 = admin-postal (ADMIN, ZH)
            // d1..02 = manager1-postal (BM, GE)
            // d1..03 = manager2-postal (BM, TI)
            // d1..04 = manager3-postal (BM, BE)
            // d1..05 = emp1 (EMP, ZH)
            // d1..06 = emp2 (EMP, ZH)
            // d1..07 = emp3 (EMP, GE)
            // d1..08 = emp4 (EMP, BS)
            // d1..09 = emp5 (EMP, BE)
            // d1..10 = emp6 (EMP, TI)
            var empThomas  = "d1000000-0000-0000-0000-000000000005";
            var empAnna    = "d1000000-0000-0000-0000-000000000006";
            var empMarco   = "d1000000-0000-0000-0000-000000000007";
            var empElena   = "d1000000-0000-0000-0000-000000000008";
            var empDavid   = "d1000000-0000-0000-0000-000000000009";
            var empLaura   = "d1000000-0000-0000-0000-000000000010";
            var mgrMarie   = "d1000000-0000-0000-0000-000000000002";
            var mgrLuca    = "d1000000-0000-0000-0000-000000000003";
            var mgrSophie  = "d1000000-0000-0000-0000-000000000004";
            var admHans    = "d1000000-0000-0000-0000-000000000001";

            // ══════════════════════════════════════════════════════════
            // 1. CUSTOMERS (15 customers across Switzerland)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO customers ("Id", customer_number, first_name, last_name, email, phone, status, preferred_locale, created_at, created_by)
                VALUES
                  ('f1000000-0000-0000-0000-000000000001', 'CU-000001', 'Peter',    'Zimmermann', 'peter.zimmermann@mail.ch',   '+41 44 123 45 01', 'Active',    'de', '2026-01-15 08:30:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000002', 'CU-000002', 'Isabelle', 'Favre',      'isabelle.favre@mail.ch',     '+41 22 234 56 02', 'Active',    'fr', '2026-01-20 10:00:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000003', 'CU-000003', 'Giuseppe', 'Bernasconi', 'giuseppe.bernasconi@mail.ch', '+41 91 345 67 03', 'Active',    'it', '2026-02-01 09:15:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000004', 'CU-000004', 'Sandra',   'Widmer',     'sandra.widmer@mail.ch',      '+41 61 456 78 04', 'Active',    'de', '2026-02-05 11:00:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000005', 'CU-000005', 'Marc',     'Dubois',     'marc.dubois@mail.ch',        '+41 22 567 89 05', 'Active',    'fr', '2026-02-10 14:30:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000006', 'CU-000006', 'Ursula',   'Bühler',     'ursula.buehler@mail.ch',     '+41 31 678 90 06', 'Active',    'de', '2026-02-15 08:45:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000007', 'CU-000007', 'Reto',     'Steiner',    'reto.steiner@mail.ch',       '+41 44 789 01 07', 'Active',    'de', '2026-02-20 09:00:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000008', 'CU-000008', 'Claudia',  'Perret',     'claudia.perret@mail.ch',     '+41 21 890 12 08', 'Active',    'fr', '2026-03-01 10:30:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000009', 'CU-000009', 'Andreas',  'Hofer',      'andreas.hofer@mail.ch',      '+41 62 901 23 09', 'Active',    'de', '2026-03-05 13:00:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000010', 'CU-000010', 'Valentina','Pellegrini', 'valentina.pellegrini@mail.ch','+41 91 012 34 10', 'Active',    'it', '2026-03-10 11:15:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000011', 'CU-000011', 'Stefan',   'Gerber',     'stefan.gerber@mail.ch',      '+41 31 111 22 11', 'Active',    'de', '2026-03-15 14:00:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000012', 'CU-000012', 'Nathalie', 'Rochat',     'nathalie.rochat@mail.ch',    '+41 22 222 33 12', 'Active',    'fr', '2026-03-20 09:30:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000013', 'CU-000013', 'Thomas',   'Brunner',    'thomas.brunner@mail.ch',     '+41 52 333 44 13', 'Inactive',  'de', '2026-01-10 08:00:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000014', 'CU-000014', 'Martina',  'Schmid',     'martina.schmid@mail.ch',     '+41 41 444 55 14', 'Suspended', 'de', '2026-02-01 12:00:00+00', '{system}'),
                  ('f1000000-0000-0000-0000-000000000015', 'CU-000015', 'Jean-Luc', 'Bonvin',     'jeanluc.bonvin@mail.ch',     '+41 27 555 66 15', 'Active',    'fr', '2026-03-25 15:00:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 2. PARCELS (20 parcels in various statuses)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO parcels ("Id", tracking_number, status, type, weight_kg, length_cm, width_cm, height_cm, price,
                  sender_customer_id, origin_branch_id, sender_name, sender_street, sender_zip_code, sender_city, sender_phone,
                  recipient_name, recipient_street, recipient_zip_code, recipient_city, recipient_phone, created_at, created_by)
                VALUES
                  -- DELIVERED parcels (6)
                  ('f2000000-0000-0000-0000-000000000001', '99.12.101.201.301.01', 'Delivered',      'Standard',   2.500, 30.0, 20.0, 15.0,  7.50,
                    'f1000000-0000-0000-0000-000000000001', '{brZurich}', 'Peter Zimmermann', 'Bahnhofstrasse 42', '8001', 'Zürich', '+41 44 123 45 01',
                    'Isabelle Favre', 'Rue du Rhône 15', '1204', 'Genève', '+41 22 234 56 02', '2026-03-01 08:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000002', '99.12.102.202.302.02', 'Delivered',      'Priority',   1.200, 25.0, 18.0, 10.0, 12.00,
                    'f1000000-0000-0000-0000-000000000002', '{brGeneva}', 'Isabelle Favre', 'Rue du Rhône 15', '1204', 'Genève', '+41 22 234 56 02',
                    'Giuseppe Bernasconi', 'Via Nassa 7', '6900', 'Lugano', '+41 91 345 67 03', '2026-03-05 09:30:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000003', '99.12.103.203.303.03', 'Delivered',      'Express',    0.500, 20.0, 15.0,  5.0, 18.50,
                    'f1000000-0000-0000-0000-000000000007', '{brZurich}', 'Reto Steiner', 'Limmatquai 78', '8001', 'Zürich', '+41 44 789 01 07',
                    'Sandra Widmer', 'Petersgraben 31', '4051', 'Basel', '+41 61 456 78 04', '2026-03-10 07:15:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000004', '99.12.104.204.304.04', 'Delivered',      'Registered', 3.800, 40.0, 30.0, 25.0, 15.00,
                    'f1000000-0000-0000-0000-000000000006', '{brBern}', 'Ursula Bühler', 'Kramgasse 12', '3011', 'Bern', '+41 31 678 90 06',
                    'Marc Dubois', 'Quai Wilson 45', '1201', 'Genève', '+41 22 567 89 05', '2026-03-12 10:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000005', '99.12.105.205.305.05', 'Delivered',      'Standard',   0.800, 22.0, 15.0, 10.0,  7.50,
                    'f1000000-0000-0000-0000-000000000009', '{brBasel}', 'Andreas Hofer', 'Marktplatz 5', '4051', 'Basel', '+41 62 901 23 09',
                    'Ursula Bühler', 'Kramgasse 12', '3011', 'Bern', '+41 31 678 90 06', '2026-03-15 11:30:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000006', '99.12.106.206.306.06', 'Delivered',      'Insured',    5.200, 50.0, 40.0, 30.0, 25.00,
                    'f1000000-0000-0000-0000-000000000003', '{brLugano}', 'Giuseppe Bernasconi', 'Via Nassa 7', '6900', 'Lugano', '+41 91 345 67 03',
                    'Peter Zimmermann', 'Bahnhofstrasse 42', '8001', 'Zürich', '+41 44 123 45 01', '2026-03-18 08:45:00+00', '{system}'),

                  -- IN_TRANSIT parcels (4)
                  ('f2000000-0000-0000-0000-000000000007', '99.13.107.207.307.07', 'InTransit',      'Standard',   1.500, 30.0, 20.0, 12.0,  7.50,
                    'f1000000-0000-0000-0000-000000000004', '{brBasel}', 'Sandra Widmer', 'Petersgraben 31', '4051', 'Basel', '+41 61 456 78 04',
                    'Claudia Perret', 'Avenue de Cour 33', '1007', 'Lausanne', '+41 21 890 12 08', '2026-04-08 08:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000008', '99.13.108.208.308.08', 'InTransit',      'Priority',   2.100, 35.0, 25.0, 20.0, 12.00,
                    'f1000000-0000-0000-0000-000000000005', '{brGeneva}', 'Marc Dubois', 'Quai Wilson 45', '1201', 'Genève', '+41 22 567 89 05',
                    'Stefan Gerber', 'Bundesplatz 1', '3003', 'Bern', '+41 31 111 22 11', '2026-04-09 09:15:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000009', '99.13.109.209.309.09', 'InTransit',      'Express',    0.300, 15.0, 10.0,  5.0, 18.50,
                    'f1000000-0000-0000-0000-000000000010', '{brLugano}', 'Valentina Pellegrini', 'Riva Caccia 1', '6900', 'Lugano', '+41 91 012 34 10',
                    'Reto Steiner', 'Limmatquai 78', '8001', 'Zürich', '+41 44 789 01 07', '2026-04-10 07:30:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000010', '99.13.110.210.310.10', 'InTransit',      'Registered', 4.000, 45.0, 35.0, 25.0, 15.00,
                    'f1000000-0000-0000-0000-000000000011', '{brBern}', 'Stefan Gerber', 'Bundesplatz 1', '3003', 'Bern', '+41 31 111 22 11',
                    'Nathalie Rochat', 'Place Bel-Air 8', '1003', 'Lausanne', '+41 22 222 33 12', '2026-04-10 14:00:00+00', '{system}'),

                  -- OUT_FOR_DELIVERY parcels (3)
                  ('f2000000-0000-0000-0000-000000000011', '99.14.111.211.311.11', 'OutForDelivery',  'Standard',   1.800, 28.0, 20.0, 15.0,  7.50,
                    'f1000000-0000-0000-0000-000000000001', '{brZurich}', 'Peter Zimmermann', 'Bahnhofstrasse 42', '8001', 'Zürich', '+41 44 123 45 01',
                    'Andreas Hofer', 'Marktplatz 5', '4051', 'Basel', '+41 62 901 23 09', '2026-04-11 06:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000012', '99.14.112.212.312.12', 'OutForDelivery',  'Priority',   0.900, 20.0, 15.0,  8.0, 12.00,
                    'f1000000-0000-0000-0000-000000000008', '{brGeneva}', 'Claudia Perret', 'Avenue de Cour 33', '1007', 'Lausanne', '+41 21 890 12 08',
                    'Jean-Luc Bonvin', 'Rue de Sion 22', '1950', 'Sion', '+41 27 555 66 15', '2026-04-11 07:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000013', '99.14.113.213.313.13', 'OutForDelivery',  'Express',    2.300, 32.0, 22.0, 18.0, 18.50,
                    'f1000000-0000-0000-0000-000000000012', '{brBern}', 'Nathalie Rochat', 'Place Bel-Air 8', '1003', 'Lausanne', '+41 22 222 33 12',
                    'Ursula Bühler', 'Kramgasse 12', '3011', 'Bern', '+41 31 678 90 06', '2026-04-11 08:30:00+00', '{system}'),

                  -- CREATED / LABEL_GENERATED (new parcels, 3)
                  ('f2000000-0000-0000-0000-000000000014', '99.15.114.214.314.14', 'Created',         'Standard',   1.000, 20.0, 15.0, 10.0,  7.50,
                    'f1000000-0000-0000-0000-000000000007', '{brZurich}', 'Reto Steiner', 'Limmatquai 78', '8001', 'Zürich', '+41 44 789 01 07',
                    'Giuseppe Bernasconi', 'Via Nassa 7', '6900', 'Lugano', '+41 91 345 67 03', '2026-04-12 08:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000015', '99.15.115.215.315.15', 'LabelGenerated',  'Priority',   3.500, 40.0, 30.0, 20.0, 12.00,
                    'f1000000-0000-0000-0000-000000000015', '{brGeneva}', 'Jean-Luc Bonvin', 'Rue de Sion 22', '1950', 'Sion', '+41 27 555 66 15',
                    'Isabelle Favre', 'Rue du Rhône 15', '1204', 'Genève', '+41 22 234 56 02', '2026-04-12 09:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000016', '99.15.116.216.316.16', 'Created',         'Insured',    8.000, 60.0, 40.0, 35.0, 25.00,
                    'f1000000-0000-0000-0000-000000000004', '{brBasel}', 'Sandra Widmer', 'Petersgraben 31', '4051', 'Basel', '+41 61 456 78 04',
                    'Valentina Pellegrini', 'Riva Caccia 1', '6900', 'Lugano', '+41 91 012 34 10', '2026-04-12 10:00:00+00', '{system}'),

                  -- RETURNED (1)
                  ('f2000000-0000-0000-0000-000000000017', '99.16.117.217.317.17', 'Returned',        'Standard',   1.100, 22.0, 16.0, 12.0,  7.50,
                    'f1000000-0000-0000-0000-000000000002', '{brGeneva}', 'Isabelle Favre', 'Rue du Rhône 15', '1204', 'Genève', '+41 22 234 56 02',
                    'Thomas Brunner', 'Technopark 1', '8400', 'Winterthur', '+41 52 333 44 13', '2026-03-25 08:00:00+00', '{system}'),

                  -- CANCELLED (1)
                  ('f2000000-0000-0000-0000-000000000018', '99.16.118.218.318.18', 'Cancelled',       'Priority',   0.600, 18.0, 12.0,  8.0, 12.00,
                    'f1000000-0000-0000-0000-000000000006', '{brBern}', 'Ursula Bühler', 'Kramgasse 12', '3011', 'Bern', '+41 31 678 90 06',
                    'Marc Dubois', 'Quai Wilson 45', '1201', 'Genève', '+41 22 567 89 05', '2026-04-01 11:00:00+00', '{system}'),

                  -- PICKED_UP (2) — at sorting center
                  ('f2000000-0000-0000-0000-000000000019', '99.17.119.219.319.19', 'PickedUp',        'Registered', 2.800, 35.0, 25.0, 18.0, 15.00,
                    'f1000000-0000-0000-0000-000000000009', '{brBasel}', 'Andreas Hofer', 'Marktplatz 5', '4051', 'Basel', '+41 62 901 23 09',
                    'Stefan Gerber', 'Bundesplatz 1', '3003', 'Bern', '+41 31 111 22 11', '2026-04-11 16:00:00+00', '{system}'),

                  ('f2000000-0000-0000-0000-000000000020', '99.17.120.220.320.20', 'PickedUp',        'Standard',   1.400, 25.0, 18.0, 14.0,  7.50,
                    'f1000000-0000-0000-0000-000000000011', '{brBern}', 'Stefan Gerber', 'Bundesplatz 1', '3003', 'Bern', '+41 31 111 22 11',
                    'Peter Zimmermann', 'Bahnhofstrasse 42', '8001', 'Zürich', '+41 44 123 45 01', '2026-04-12 07:30:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 3. TRACKING RECORDS (one per parcel)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO tracking_records ("Id", tracking_number, current_status, current_branch_id, estimated_delivery, created_at, created_by)
                VALUES
                  ('f3000000-0000-0000-0000-000000000001', '99.12.101.201.301.01', 'Delivered',       '{brGeneva}',  '2026-03-04 17:00:00+00', '2026-03-01 08:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000002', '99.12.102.202.302.02', 'Delivered',       '{brLugano}',  '2026-03-08 17:00:00+00', '2026-03-05 09:30:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000003', '99.12.103.203.303.03', 'Delivered',       '{brBasel}',   '2026-03-11 17:00:00+00', '2026-03-10 07:15:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000004', '99.12.104.204.304.04', 'Delivered',       '{brGeneva}',  '2026-03-15 17:00:00+00', '2026-03-12 10:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000005', '99.12.105.205.305.05', 'Delivered',       '{brBern}',    '2026-03-18 17:00:00+00', '2026-03-15 11:30:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000006', '99.12.106.206.306.06', 'Delivered',       '{brZurich}',  '2026-03-21 17:00:00+00', '2026-03-18 08:45:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000007', '99.13.107.207.307.07', 'InTransit',       '{brBasel}',   '2026-04-12 17:00:00+00', '2026-04-08 08:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000008', '99.13.108.208.308.08', 'InTransit',       '{brGeneva}',  '2026-04-13 17:00:00+00', '2026-04-09 09:15:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000009', '99.13.109.209.309.09', 'InTransit',       '{brBasel}',   '2026-04-12 12:00:00+00', '2026-04-10 07:30:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000010', '99.13.110.210.310.10', 'InTransit',       '{brBern}',    '2026-04-13 17:00:00+00', '2026-04-10 14:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000011', '99.14.111.211.311.11', 'OutForDelivery',  '{brBasel}',   '2026-04-12 17:00:00+00', '2026-04-11 06:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000012', '99.14.112.212.312.12', 'OutForDelivery',  '{brGeneva}',  '2026-04-12 17:00:00+00', '2026-04-11 07:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000013', '99.14.113.213.313.13', 'OutForDelivery',  '{brBern}',    '2026-04-12 17:00:00+00', '2026-04-11 08:30:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000014', '99.15.114.214.314.14', 'Registered',      '{brZurich}',  '2026-04-16 17:00:00+00', '2026-04-12 08:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000015', '99.15.115.215.315.15', 'Registered',      '{brGeneva}',  '2026-04-16 17:00:00+00', '2026-04-12 09:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000016', '99.15.116.216.316.16', 'Registered',      '{brBasel}',   '2026-04-17 17:00:00+00', '2026-04-12 10:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000017', '99.16.117.217.317.17', 'Returned',        '{brGeneva}',  NULL,                     '2026-03-25 08:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000018', '99.16.118.218.318.18', 'Cancelled',       '{brBern}',    NULL,                     '2026-04-01 11:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000019', '99.17.119.219.319.19', 'PickedUp',        '{brBasel}',   '2026-04-14 17:00:00+00', '2026-04-11 16:00:00+00', '{system}'),
                  ('f3000000-0000-0000-0000-000000000020', '99.17.120.220.320.20', 'PickedUp',        '{brBern}',    '2026-04-15 17:00:00+00', '2026-04-12 07:30:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 4. TRACKING EVENTS (multiple per parcel for realistic history)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO tracking_events ("Id", tracking_number, event_type, branch_id, location, description_key, event_timestamp, scanned_by_employee_id, created_at, created_by)
                VALUES
                  -- Parcel 01: ZH→GE (Delivered)
                  ('f4000000-0000-0000-0000-000000000001', '99.12.101.201.301.01', 'Registered',       '{brZurich}', 'Zürich HB',           'Parcel registered',             '2026-03-01 08:00:00+00', '{empThomas}', '2026-03-01 08:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000002', '99.12.101.201.301.01', 'PickedUp',         '{brZurich}', 'Zürich HB',           'Parcel picked up from sender',  '2026-03-01 14:30:00+00', '{empThomas}', '2026-03-01 14:30:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000003', '99.12.101.201.301.01', 'ArrivedAtSorting', '{brBasel}',  'Sortierzentrum Basel','Arrived at sorting center',     '2026-03-02 06:00:00+00', '{empElena}',  '2026-03-02 06:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000004', '99.12.101.201.301.01', 'DepartedSorting',  '{brBasel}',  'Sortierzentrum Basel','Departed sorting center',       '2026-03-02 20:00:00+00', '{empElena}',  '2026-03-02 20:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000005', '99.12.101.201.301.01', 'ArrivedAtBranch',  '{brGeneva}', 'Genève Centre',       'Arrived at destination branch', '2026-03-03 08:00:00+00', '{empMarco}',  '2026-03-03 08:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000006', '99.12.101.201.301.01', 'OutForDelivery',   '{brGeneva}', 'Genève Centre',       'Out for delivery',              '2026-03-04 07:30:00+00', '{empMarco}',  '2026-03-04 07:30:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000007', '99.12.101.201.301.01', 'Delivered',        '{brGeneva}', 'Genève',              'Delivered to recipient',        '2026-03-04 11:45:00+00', '{empMarco}',  '2026-03-04 11:45:00+00', '{system}'),

                  -- Parcel 07: BS→Lausanne (InTransit)
                  ('f4000000-0000-0000-0000-000000000008', '99.13.107.207.307.07', 'Registered',       '{brBasel}',  'Basel Post',          'Parcel registered',             '2026-04-08 08:00:00+00', '{empElena}',  '2026-04-08 08:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000009', '99.13.107.207.307.07', 'PickedUp',         '{brBasel}',  'Basel Post',          'Parcel picked up from sender',  '2026-04-08 15:00:00+00', '{empElena}',  '2026-04-08 15:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000010', '99.13.107.207.307.07', 'ArrivedAtSorting', '{brBasel}',  'Sortierzentrum Basel','Arrived at sorting center',     '2026-04-09 06:00:00+00', '{empElena}',  '2026-04-09 06:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000011', '99.13.107.207.307.07', 'DepartedSorting',  '{brBasel}',  'Sortierzentrum Basel','Departed sorting center',       '2026-04-10 04:00:00+00', '{empElena}',  '2026-04-10 04:00:00+00', '{system}'),

                  -- Parcel 11: ZH→BS (OutForDelivery)
                  ('f4000000-0000-0000-0000-000000000012', '99.14.111.211.311.11', 'Registered',       '{brZurich}', 'Zürich HB',           'Parcel registered',             '2026-04-11 06:00:00+00', '{empThomas}', '2026-04-11 06:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000013', '99.14.111.211.311.11', 'PickedUp',         '{brZurich}', 'Zürich HB',           'Parcel picked up from sender',  '2026-04-11 10:00:00+00', '{empAnna}',   '2026-04-11 10:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000014', '99.14.111.211.311.11', 'ArrivedAtSorting', '{brBasel}',  'Sortierzentrum Basel','Arrived at sorting center',     '2026-04-11 18:00:00+00', '{empElena}',  '2026-04-11 18:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000015', '99.14.111.211.311.11', 'DepartedSorting',  '{brBasel}',  'Sortierzentrum Basel','Departed sorting center',       '2026-04-12 04:00:00+00', '{empElena}',  '2026-04-12 04:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000016', '99.14.111.211.311.11', 'ArrivedAtBranch',  '{brBasel}',  'Basel Post',          'Arrived at destination branch', '2026-04-12 06:30:00+00', '{empElena}',  '2026-04-12 06:30:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000017', '99.14.111.211.311.11', 'OutForDelivery',   '{brBasel}',  'Basel Post',          'Out for delivery',              '2026-04-12 08:00:00+00', '{empElena}',  '2026-04-12 08:00:00+00', '{system}'),

                  -- Parcel 17: GE→ZH (Returned — delivery failed)
                  ('f4000000-0000-0000-0000-000000000018', '99.16.117.217.317.17', 'Registered',       '{brGeneva}', 'Genève Centre',       'Parcel registered',             '2026-03-25 08:00:00+00', '{empMarco}',  '2026-03-25 08:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000019', '99.16.117.217.317.17', 'PickedUp',         '{brGeneva}', 'Genève Centre',       'Parcel picked up',              '2026-03-25 14:00:00+00', '{empMarco}',  '2026-03-25 14:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000020', '99.16.117.217.317.17', 'ArrivedAtSorting', '{brBasel}',  'Sortierzentrum Basel','Arrived at sorting center',     '2026-03-26 06:00:00+00', '{empElena}',  '2026-03-26 06:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000021', '99.16.117.217.317.17', 'ArrivedAtBranch',  '{brZurich}', 'Zürich HB',           'Arrived at destination branch', '2026-03-27 08:00:00+00', '{empThomas}', '2026-03-27 08:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000022', '99.16.117.217.317.17', 'OutForDelivery',   '{brZurich}', 'Zürich',              'Out for delivery',              '2026-03-28 07:30:00+00', '{empAnna}',   '2026-03-28 07:30:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000023', '99.16.117.217.317.17', 'DeliveryFailed',   '{brZurich}', 'Winterthur',          'Delivery failed — absent',      '2026-03-28 14:00:00+00', '{empAnna}',   '2026-03-28 14:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000024', '99.16.117.217.317.17', 'Returned',         '{brGeneva}', 'Genève Centre',       'Returned to sender',            '2026-04-02 10:00:00+00', '{empMarco}',  '2026-04-02 10:00:00+00', '{system}'),

                  -- Parcel 14: Created today (only registered)
                  ('f4000000-0000-0000-0000-000000000025', '99.15.114.214.314.14', 'Registered',       '{brZurich}', 'Zürich HB',           'Parcel registered',             '2026-04-12 08:00:00+00', '{empThomas}', '2026-04-12 08:00:00+00', '{system}'),

                  -- Parcel 19: PickedUp
                  ('f4000000-0000-0000-0000-000000000026', '99.17.119.219.319.19', 'Registered',       '{brBasel}',  'Basel Post',          'Parcel registered',             '2026-04-11 16:00:00+00', '{empElena}',  '2026-04-11 16:00:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000027', '99.17.119.219.319.19', 'PickedUp',         '{brBasel}',  'Basel Post',          'Parcel picked up from sender',  '2026-04-12 09:00:00+00', '{empElena}',  '2026-04-12 09:00:00+00', '{system}'),

                  -- Parcel 20: PickedUp
                  ('f4000000-0000-0000-0000-000000000028', '99.17.120.220.320.20', 'Registered',       '{brBern}',   'Bern HB',             'Parcel registered',             '2026-04-12 07:30:00+00', '{empDavid}',  '2026-04-12 07:30:00+00', '{system}'),
                  ('f4000000-0000-0000-0000-000000000029', '99.17.120.220.320.20', 'PickedUp',         '{brBern}',   'Bern HB',             'Parcel picked up from sender',  '2026-04-12 10:00:00+00', '{empDavid}',  '2026-04-12 10:00:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 5. DELIVERY ROUTES (4 active routes)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO delivery_routes ("Id", route_code, branch_id, assigned_employee_id, status, date, created_at, created_by)
                VALUES
                  ('f5000000-0000-0000-0000-000000000001', 'RT-ZH-20260412-A', '{brZurich}', '{empThomas}', 'InProgress', '2026-04-12', '2026-04-12 06:00:00+00', '{system}'),
                  ('f5000000-0000-0000-0000-000000000002', 'RT-BS-20260412-A', '{brBasel}',  '{empElena}',  'InProgress', '2026-04-12', '2026-04-12 06:00:00+00', '{system}'),
                  ('f5000000-0000-0000-0000-000000000003', 'RT-GE-20260412-A', '{brGeneva}', '{empMarco}',  'Planned',    '2026-04-12', '2026-04-12 06:00:00+00', '{system}'),
                  ('f5000000-0000-0000-0000-000000000004', 'RT-BE-20260412-A', '{brBern}',   '{empDavid}',  'Planned',    '2026-04-12', '2026-04-12 06:00:00+00', '{system}'),
                  ('f5000000-0000-0000-0000-000000000005', 'RT-ZH-20260410-A', '{brZurich}', '{empAnna}',   'Completed',  '2026-04-10', '2026-04-10 06:00:00+00', '{system}'),
                  ('f5000000-0000-0000-0000-000000000006', 'RT-GE-20260405-A', '{brGeneva}', '{empMarco}',  'Completed',  '2026-04-05', '2026-04-05 06:00:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 6. DELIVERY SLOTS
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO delivery_slots ("Id", delivery_route_id, tracking_number, sequence_order, status, recipient_signature, created_at, created_by)
                VALUES
                  -- Route ZH today (InProgress) — 2 slots
                  ('f6000000-0000-0000-0000-000000000001', 'f5000000-0000-0000-0000-000000000001', '99.14.111.211.311.11', 1, 'Pending',   NULL, '2026-04-12 06:00:00+00', '{system}'),
                  ('f6000000-0000-0000-0000-000000000002', 'f5000000-0000-0000-0000-000000000001', '99.15.114.214.314.14', 2, 'Pending',   NULL, '2026-04-12 06:00:00+00', '{system}'),

                  -- Route BS today (InProgress) — 2 slots
                  ('f6000000-0000-0000-0000-000000000003', 'f5000000-0000-0000-0000-000000000002', '99.13.107.207.307.07', 1, 'Pending',   NULL, '2026-04-12 06:00:00+00', '{system}'),
                  ('f6000000-0000-0000-0000-000000000004', 'f5000000-0000-0000-0000-000000000002', '99.13.109.209.309.09', 2, 'Pending',   NULL, '2026-04-12 06:00:00+00', '{system}'),

                  -- Route GE today (Planned) — 2 slots
                  ('f6000000-0000-0000-0000-000000000005', 'f5000000-0000-0000-0000-000000000003', '99.14.112.212.312.12', 1, 'Pending',   NULL, '2026-04-12 06:00:00+00', '{system}'),
                  ('f6000000-0000-0000-0000-000000000006', 'f5000000-0000-0000-0000-000000000003', '99.13.108.208.308.08', 2, 'Pending',   NULL, '2026-04-12 06:00:00+00', '{system}'),

                  -- Route BE today (Planned) — 1 slot
                  ('f6000000-0000-0000-0000-000000000007', 'f5000000-0000-0000-0000-000000000004', '99.14.113.213.313.13', 1, 'Pending',   NULL, '2026-04-12 06:00:00+00', '{system}'),

                  -- Completed route ZH (past) — 2 delivered
                  ('f6000000-0000-0000-0000-000000000008', 'f5000000-0000-0000-0000-000000000005', '99.12.103.203.303.03', 1, 'Delivered', 'S. Widmer', '2026-04-10 06:00:00+00', '{system}'),
                  ('f6000000-0000-0000-0000-000000000009', 'f5000000-0000-0000-0000-000000000005', '99.12.105.205.305.05', 2, 'Delivered', 'U. Bühler', '2026-04-10 06:00:00+00', '{system}'),

                  -- Completed route GE (past) — 1 delivered, 1 failed
                  ('f6000000-0000-0000-0000-000000000010', 'f5000000-0000-0000-0000-000000000006', '99.12.102.202.302.02', 1, 'Delivered', 'G. Bernasconi', '2026-04-05 06:00:00+00', '{system}'),
                  ('f6000000-0000-0000-0000-000000000011', 'f5000000-0000-0000-0000-000000000006', '99.16.117.217.317.17', 2, 'Failed',   NULL, '2026-04-05 06:00:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 7. DELIVERY ATTEMPTS (for completed/failed slots)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO delivery_attempts ("Id", delivery_slot_id, result, notes, attempt_timestamp, created_at, created_by)
                VALUES
                  ('f7000000-0000-0000-0000-000000000001', 'f6000000-0000-0000-0000-000000000008', 'Delivered',    'Left with recipient',      '2026-04-10 10:30:00+00', '2026-04-10 10:30:00+00', '{system}'),
                  ('f7000000-0000-0000-0000-000000000002', 'f6000000-0000-0000-0000-000000000009', 'Delivered',    'Left with recipient',      '2026-04-10 11:15:00+00', '2026-04-10 11:15:00+00', '{system}'),
                  ('f7000000-0000-0000-0000-000000000003', 'f6000000-0000-0000-0000-000000000010', 'Delivered',    'Signed by G. Bernasconi',  '2026-04-05 09:45:00+00', '2026-04-05 09:45:00+00', '{system}'),
                  ('f7000000-0000-0000-0000-000000000004', 'f6000000-0000-0000-0000-000000000011', 'Absent',       'Nobody home, left notice', '2026-03-28 14:00:00+00', '2026-03-28 14:00:00+00', '{system}'),
                  ('f7000000-0000-0000-0000-000000000005', 'f6000000-0000-0000-0000-000000000011', 'WrongAddress', 'Address does not exist',   '2026-03-30 10:00:00+00', '2026-03-30 10:00:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 8. PICKUP REQUESTS (5 requests in various states)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO pickup_requests ("Id", customer_id, pickup_street, pickup_zip_code, pickup_city, preferred_date,
                  preferred_time_from, preferred_time_to, status, created_at, created_by)
                VALUES
                  ('f8000000-0000-0000-0000-000000000001', 'f1000000-0000-0000-0000-000000000007', 'Limmatquai 78',      '8001', 'Zürich',   '2026-04-13', '09:00', '12:00', 'Confirmed', '2026-04-11 08:00:00+00', '{system}'),
                  ('f8000000-0000-0000-0000-000000000002', 'f1000000-0000-0000-0000-000000000005', 'Quai Wilson 45',     '1201', 'Genève',   '2026-04-13', '14:00', '17:00', 'Pending',   '2026-04-12 09:00:00+00', '{system}'),
                  ('f8000000-0000-0000-0000-000000000003', 'f1000000-0000-0000-0000-000000000009', 'Marktplatz 5',       '4051', 'Basel',    '2026-04-14', '08:00', '10:00', 'Pending',   '2026-04-12 10:00:00+00', '{system}'),
                  ('f8000000-0000-0000-0000-000000000004', 'f1000000-0000-0000-0000-000000000006', 'Kramgasse 12',       '3011', 'Bern',     '2026-04-12', '10:00', '12:00', 'Assigned',  '2026-04-11 14:00:00+00', '{system}'),
                  ('f8000000-0000-0000-0000-000000000005', 'f1000000-0000-0000-0000-000000000001', 'Bahnhofstrasse 42',  '8001', 'Zürich',   '2026-04-10', '09:00', '11:00', 'PickedUp',  '2026-04-09 15:00:00+00', '{system}'),
                  ('f8000000-0000-0000-0000-000000000006', 'f1000000-0000-0000-0000-000000000010', 'Riva Caccia 1',      '6900', 'Lugano',   '2026-04-08', '14:00', '16:00', 'Cancelled', '2026-04-07 12:00:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 9. NOTIFICATION LOGS (email + SMS notifications sent)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO notification_logs ("Id", recipient_email, recipient_phone, type, status, subject, body, reference_id, event_type, retry_count, created_at, created_by)
                VALUES
                  -- Parcel confirmation emails
                  ('f9000000-0000-0000-0000-000000000001', 'peter.zimmermann@mail.ch', NULL, 'Email', 'Sent',
                    'Ihre Sendung wurde registriert — 99.12.101.201.301.01',
                    'Sehr geehrter Herr Zimmermann, Ihre Sendung mit der Trackingnummer 99.12.101.201.301.01 wurde erfolgreich registriert.',
                    '99.12.101.201.301.01', 'PARCEL_CREATED', 0, '2026-03-01 08:05:00+00', '{system}'),

                  ('f9000000-0000-0000-0000-000000000002', 'isabelle.favre@mail.ch', NULL, 'Email', 'Sent',
                    'Votre envoi a été enregistré — 99.12.102.202.302.02',
                    'Chère Madame Favre, votre envoi avec le numéro de suivi 99.12.102.202.302.02 a été enregistré avec succès.',
                    '99.12.102.202.302.02', 'PARCEL_CREATED', 0, '2026-03-05 09:35:00+00', '{system}'),

                  -- Delivery notifications
                  ('f9000000-0000-0000-0000-000000000003', 'isabelle.favre@mail.ch', NULL, 'Email', 'Sent',
                    'Ihre Sendung wurde zugestellt — 99.12.101.201.301.01',
                    'Ihre Sendung 99.12.101.201.301.01 wurde erfolgreich an Isabelle Favre zugestellt.',
                    '99.12.101.201.301.01', 'PARCEL_DELIVERED', 0, '2026-03-04 11:50:00+00', '{system}'),

                  ('f9000000-0000-0000-0000-000000000004', 'giuseppe.bernasconi@mail.ch', NULL, 'Email', 'Sent',
                    'Il vostro pacco è stato consegnato — 99.12.102.202.302.02',
                    'Il pacco 99.12.102.202.302.02 è stato consegnato con successo.',
                    '99.12.102.202.302.02', 'PARCEL_DELIVERED', 0, '2026-03-08 14:30:00+00', '{system}'),

                  -- In-transit updates
                  ('f9000000-0000-0000-0000-000000000005', 'sandra.widmer@mail.ch', NULL, 'Email', 'Sent',
                    'Sendung unterwegs — 99.13.107.207.307.07',
                    'Ihre Sendung 99.13.107.207.307.07 ist unterwegs und wird voraussichtlich am 12.04.2026 zugestellt.',
                    '99.13.107.207.307.07', 'PARCEL_IN_TRANSIT', 0, '2026-04-09 06:05:00+00', '{system}'),

                  ('f9000000-0000-0000-0000-000000000006', 'marc.dubois@mail.ch', NULL, 'Email', 'Sent',
                    'Envoi en cours de livraison — 99.13.108.208.308.08',
                    'Votre envoi 99.13.108.208.308.08 est en cours de livraison.',
                    '99.13.108.208.308.08', 'PARCEL_IN_TRANSIT', 0, '2026-04-09 09:20:00+00', '{system}'),

                  -- Failed delivery notification
                  ('f9000000-0000-0000-0000-000000000007', 'isabelle.favre@mail.ch', NULL, 'Email', 'Sent',
                    'Zustellversuch fehlgeschlagen — 99.16.117.217.317.17',
                    'Die Zustellung Ihrer Sendung 99.16.117.217.317.17 war leider nicht möglich. Bitte kontaktieren Sie Ihre Postfiliale.',
                    '99.16.117.217.317.17', 'DELIVERY_FAILED', 0, '2026-03-28 14:05:00+00', '{system}'),

                  -- Out for delivery
                  ('f9000000-0000-0000-0000-000000000008', 'andreas.hofer@mail.ch', NULL, 'Email', 'Sent',
                    'Sendung wird heute zugestellt — 99.14.111.211.311.11',
                    'Ihre Sendung 99.14.111.211.311.11 wird heute zugestellt.',
                    '99.14.111.211.311.11', 'OUT_FOR_DELIVERY', 0, '2026-04-12 08:05:00+00', '{system}'),

                  -- Pickup confirmation
                  ('f9000000-0000-0000-0000-000000000009', 'reto.steiner@mail.ch', NULL, 'Email', 'Sent',
                    'Abholauftrag bestätigt',
                    'Ihr Abholauftrag für den 13.04.2026 zwischen 09:00 und 12:00 wurde bestätigt.',
                    'f8000000-0000-0000-0000-000000000001', 'PICKUP_CONFIRMED', 0, '2026-04-11 08:05:00+00', '{system}'),

                  -- Failed email
                  ('f9000000-0000-0000-0000-000000000010', 'thomas.brunner@mail.ch', NULL, 'Email', 'Failed',
                    'Sendung retourniert — 99.16.117.217.317.17',
                    'Ihre Sendung 99.16.117.217.317.17 wurde an den Absender zurückgeschickt.',
                    '99.16.117.217.317.17', 'PARCEL_RETURNED', 2, '2026-04-02 10:05:00+00', '{system}'),

                  -- Recent sends
                  ('f9000000-0000-0000-0000-000000000011', 'claudia.perret@mail.ch', NULL, 'Email', 'Pending',
                    'Envoi en cours — 99.14.112.212.312.12',
                    'Votre envoi 99.14.112.212.312.12 est en cours de livraison aujourd''hui.',
                    '99.14.112.212.312.12', 'OUT_FOR_DELIVERY', 0, '2026-04-12 08:10:00+00', '{system}'),

                  ('f9000000-0000-0000-0000-000000000012', 'ursula.buehler@mail.ch', NULL, 'Email', 'Pending',
                    'Sendung wird heute zugestellt — 99.14.113.213.313.13',
                    'Ihre Sendung 99.14.113.213.313.13 wird heute zugestellt. Bitte stellen Sie sicher, dass jemand anwesend ist.',
                    '99.14.113.213.313.13', 'OUT_FOR_DELIVERY', 0, '2026-04-12 08:35:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 10. IN-APP NOTIFICATIONS (visible in employee inbox)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO in_app_notifications ("Id", target_employee_id, target_role, target_branch_id, title, message,
                  category, reference_url, sender_employee_id, created_at, created_by)
                VALUES
                  -- System-wide announcements (target_role)
                  ('fa000000-0000-0000-0000-000000000001', NULL, 'ADMIN', NULL,
                    'System Maintenance Scheduled',
                    'Planned maintenance on April 15, 2026 from 02:00–04:00 CET. All services may be temporarily unavailable.',
                    'System', '/app', NULL, '2026-04-10 12:00:00+00', '{system}'),

                  ('fa000000-0000-0000-0000-000000000002', NULL, NULL, NULL,
                    'New Parcel Processing Guidelines',
                    'Updated parcel processing guidelines are now in effect. Please review the new weight and dimension limits for Express shipments.',
                    'Info', '/app/parcels', '{admHans}', '2026-04-08 09:00:00+00', '{system}'),

                  -- Branch-specific notifications
                  ('fa000000-0000-0000-0000-000000000003', NULL, NULL, '{brZurich}',
                    'High Volume Alert — Zürich HB',
                    'Expected high parcel volume today due to Easter holiday returns. Additional sorting shifts may be required.',
                    'Warning', '/app/parcels', '{mgrSophie}', '2026-04-12 06:30:00+00', '{system}'),

                  ('fa000000-0000-0000-0000-000000000004', NULL, NULL, '{brGeneva}',
                    'Route RT-GE-20260412-A Ready',
                    'Delivery route RT-GE-20260412-A has been planned with 2 parcels. Please begin deliveries by 09:00.',
                    'Info', '/app/delivery', '{mgrMarie}', '2026-04-12 07:00:00+00', '{system}'),

                  -- Personal notifications (targeted to specific employees)
                  ('fa000000-0000-0000-0000-000000000005', '{empThomas}', NULL, NULL,
                    'Route Assignment — RT-ZH-20260412-A',
                    'You have been assigned to delivery route RT-ZH-20260412-A for today. 2 parcels to deliver in the Zürich area.',
                    'Info', '/app/delivery', '{mgrSophie}', '2026-04-12 06:15:00+00', '{system}'),

                  ('fa000000-0000-0000-0000-000000000006', '{empElena}', NULL, NULL,
                    'Route Assignment — RT-BS-20260412-A',
                    'You have been assigned to delivery route RT-BS-20260412-A for today. 2 parcels to deliver in the Basel area.',
                    'Info', '/app/delivery', '{mgrSophie}', '2026-04-12 06:15:00+00', '{system}'),

                  ('fa000000-0000-0000-0000-000000000007', '{empMarco}', NULL, NULL,
                    'Pickup Request Pending — CU-000005',
                    'Customer Marc Dubois has requested a pickup at Quai Wilson 45, 1201 Genève for April 13. Please confirm or reassign.',
                    'Warning', '/app/pickups', '{mgrMarie}', '2026-04-12 09:05:00+00', '{system}'),

                  ('fa000000-0000-0000-0000-000000000008', '{empDavid}', NULL, NULL,
                    'Delivery Attempt Failed — 99.16.117.217.317.17',
                    'Parcel 99.16.117.217.317.17 delivery failed. Customer was absent at the delivery address. Parcel has been returned to sender.',
                    'Urgent', '/app/parcels', NULL, '2026-03-28 14:05:00+00', '{system}'),

                  ('fa000000-0000-0000-0000-000000000009', '{empAnna}', NULL, NULL,
                    'Training Session Reminder',
                    'Reminder: Mandatory safety training on April 16, 2026 at 14:00 in Room 3B at Zürich HB branch.',
                    'Info', NULL, '{admHans}', '2026-04-11 10:00:00+00', '{system}'),

                  ('fa000000-0000-0000-0000-000000000010', '{empLaura}', NULL, NULL,
                    'Shift Change Approved',
                    'Your shift change request for April 14 has been approved. New shift: 06:00–14:00.',
                    'Info', NULL, '{mgrLuca}', '2026-04-11 16:00:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 11. IN-APP NOTIFICATION READ RECEIPTS (some read)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO in_app_notification_reads (id, in_app_notification_id, employee_id, read_at)
                VALUES
                  ('fc000000-0000-0000-0000-000000000001', 'fa000000-0000-0000-0000-000000000002', '{empThomas}', '2026-04-08 10:00:00+00'),
                  ('fc000000-0000-0000-0000-000000000002', 'fa000000-0000-0000-0000-000000000002', '{empAnna}',   '2026-04-08 11:00:00+00'),
                  ('fc000000-0000-0000-0000-000000000003', 'fa000000-0000-0000-0000-000000000005', '{empThomas}', '2026-04-12 06:20:00+00'),
                  ('fc000000-0000-0000-0000-000000000004', 'fa000000-0000-0000-0000-000000000006', '{empElena}',  '2026-04-12 06:20:00+00'),
                  ('fc000000-0000-0000-0000-000000000005', 'fa000000-0000-0000-0000-000000000009', '{empAnna}',   '2026-04-11 11:00:00+00')
                ON CONFLICT DO NOTHING;
                """);

            // ══════════════════════════════════════════════════════════
            // 12. NOTIFICATION PREFERENCES (for active customers)
            // ══════════════════════════════════════════════════════════
            migrationBuilder.Sql($"""
                INSERT INTO notification_preferences ("Id", customer_id, email_enabled, sms_enabled, in_app_enabled, preferred_locale, created_at, created_by)
                VALUES
                  ('fb000000-0000-0000-0000-000000000001', 'f1000000-0000-0000-0000-000000000001', true,  false, true, 'de', '2026-01-15 08:30:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000002', 'f1000000-0000-0000-0000-000000000002', true,  true,  true, 'fr', '2026-01-20 10:00:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000003', 'f1000000-0000-0000-0000-000000000003', true,  false, true, 'it', '2026-02-01 09:15:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000004', 'f1000000-0000-0000-0000-000000000004', true,  false, true, 'de', '2026-02-05 11:00:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000005', 'f1000000-0000-0000-0000-000000000005', true,  true,  true, 'fr', '2026-02-10 14:30:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000006', 'f1000000-0000-0000-0000-000000000006', true,  false, true, 'de', '2026-02-15 08:45:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000007', 'f1000000-0000-0000-0000-000000000007', true,  false, true, 'de', '2026-02-20 09:00:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000008', 'f1000000-0000-0000-0000-000000000008', true,  true,  true, 'fr', '2026-03-01 10:30:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000009', 'f1000000-0000-0000-0000-000000000009', true,  false, true, 'de', '2026-03-05 13:00:00+00', '{system}'),
                  ('fb000000-0000-0000-0000-000000000010', 'f1000000-0000-0000-0000-000000000010', true,  false, true, 'it', '2026-03-10 11:15:00+00', '{system}')
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM in_app_notification_reads WHERE \"Id\" LIKE 'fc000000%';");
            migrationBuilder.Sql("DELETE FROM in_app_notifications WHERE \"Id\" LIKE 'fa000000%';");
            migrationBuilder.Sql("DELETE FROM notification_preferences WHERE \"Id\" LIKE 'fb000000%';");
            migrationBuilder.Sql("DELETE FROM notification_logs WHERE \"Id\" LIKE 'f9000000%';");
            migrationBuilder.Sql("DELETE FROM pickup_requests WHERE \"Id\" LIKE 'f8000000%';");
            migrationBuilder.Sql("DELETE FROM delivery_attempts WHERE \"Id\" LIKE 'f7000000%';");
            migrationBuilder.Sql("DELETE FROM delivery_slots WHERE \"Id\" LIKE 'f6000000%';");
            migrationBuilder.Sql("DELETE FROM delivery_routes WHERE \"Id\" LIKE 'f5000000%';");
            migrationBuilder.Sql("DELETE FROM tracking_events WHERE \"Id\" LIKE 'f4000000%';");
            migrationBuilder.Sql("DELETE FROM tracking_records WHERE \"Id\" LIKE 'f3000000%';");
            migrationBuilder.Sql("DELETE FROM parcels WHERE \"Id\" LIKE 'f2000000%';");
            migrationBuilder.Sql("DELETE FROM customers WHERE \"Id\" LIKE 'f1000000%';");
        }
    }
}
