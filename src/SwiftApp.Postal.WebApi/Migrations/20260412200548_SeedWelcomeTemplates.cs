using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftApp.Postal.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedWelcomeTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = "2026-04-12T20:00:00Z";
            var system = "system";

            // ── Welcome Templates ─────────────────────────────────────
            migrationBuilder.Sql($"""
                INSERT INTO notification_templates ("Id", template_code, type, event_type, created_at, created_by)
                VALUES
                  ('e1000000-0000-0000-0000-000000000004', 'employee-welcome', 'Email', 'EmployeeCreated', '{now}', '{system}'),
                  ('e1000000-0000-0000-0000-000000000005', 'customer-welcome', 'Email', 'CustomerCreated', '{now}', '{system}')
                ON CONFLICT DO NOTHING;
                """);

            var t4 = "e1000000-0000-0000-0000-000000000004";
            var t5 = "e1000000-0000-0000-0000-000000000005";
            var col = "notification_template_id, locale, subject, body, created_at, created_by";

            // Employee Welcome – 4 languages
            migrationBuilder.Sql(
                "INSERT INTO notification_template_translations (\"Id\", " + col + ") VALUES " +
                $"(gen_random_uuid(), '{t4}', 'de', 'Willkommen bei Swiss Postal, {{{{employee_number}}}}', " +
                $"'Sehr geehrte/r Mitarbeiter/in {{{{employee_number}}}}, Herzlich willkommen bei Swiss Postal! " +
                $"Ihr Konto wurde erfolgreich angelegt. Sie koennen sich ab sofort im System anmelden und Ihre Aufgaben verwalten. " +
                $"Bei Fragen steht Ihnen Ihr Filialleiter gerne zur Verfuegung. Mit freundlichen Gruessen, SwiftApp Postal', '{now}', '{system}'), " +

                $"(gen_random_uuid(), '{t4}', 'fr', 'Bienvenue chez Swiss Postal, {{{{employee_number}}}}', " +
                $"'Cher/Chere collegue {{{{employee_number}}}}, Bienvenue chez Swiss Postal! " +
                $"Votre compte a ete cree avec succes. Vous pouvez desormais vous connecter et gerer vos taches. " +
                $"N''hesitez pas a contacter votre responsable pour toute question. Cordialement, SwiftApp Postal', '{now}', '{system}'), " +

                $"(gen_random_uuid(), '{t4}', 'it', 'Benvenuto/a in Swiss Postal, {{{{employee_number}}}}', " +
                $"'Gentile collaboratore/trice {{{{employee_number}}}}, Benvenuto/a in Swiss Postal! " +
                $"Il suo account e stato creato con successo. Puo accedere al sistema e gestire le sue attivita. " +
                $"Per qualsiasi domanda, si rivolga al responsabile della filiale. Cordiali saluti, SwiftApp Postal', '{now}', '{system}'), " +

                $"(gen_random_uuid(), '{t4}', 'en', 'Welcome to Swiss Postal, {{{{employee_number}}}}', " +
                $"'Dear colleague {{{{employee_number}}}}, Welcome to Swiss Postal! " +
                $"Your account has been created successfully. You can now log in and manage your tasks. " +
                $"Please contact your branch manager if you have any questions. Kind regards, SwiftApp Postal', '{now}', '{system}') " +
                "ON CONFLICT DO NOTHING;");

            // Customer Welcome – 4 languages
            migrationBuilder.Sql(
                "INSERT INTO notification_template_translations (\"Id\", " + col + ") VALUES " +
                $"(gen_random_uuid(), '{t5}', 'de', 'Willkommen bei Swiss Postal, {{{{customer_number}}}}', " +
                $"'Sehr geehrte/r Kunde/in {{{{customer_number}}}}, Willkommen bei Swiss Postal! " +
                $"Ihr Kundenkonto wurde erfolgreich erstellt. Sie koennen nun Pakete versenden, verfolgen und Benachrichtigungen erhalten. " +
                $"Vielen Dank fuer Ihr Vertrauen. Mit freundlichen Gruessen, SwiftApp Postal', '{now}', '{system}'), " +

                $"(gen_random_uuid(), '{t5}', 'fr', 'Bienvenue chez Swiss Postal, {{{{customer_number}}}}', " +
                $"'Cher/Chere client/e {{{{customer_number}}}}, Bienvenue chez Swiss Postal! " +
                $"Votre compte client a ete cree avec succes. Vous pouvez desormais envoyer et suivre vos colis. " +
                $"Merci pour votre confiance. Cordialement, SwiftApp Postal', '{now}', '{system}'), " +

                $"(gen_random_uuid(), '{t5}', 'it', 'Benvenuto/a in Swiss Postal, {{{{customer_number}}}}', " +
                $"'Gentile cliente {{{{customer_number}}}}, Benvenuto/a in Swiss Postal! " +
                $"Il suo account cliente e stato creato con successo. Ora puo inviare e tracciare i suoi pacchi. " +
                $"Grazie per la sua fiducia. Cordiali saluti, SwiftApp Postal', '{now}', '{system}'), " +

                $"(gen_random_uuid(), '{t5}', 'en', 'Welcome to Swiss Postal, {{{{customer_number}}}}', " +
                $"'Dear customer {{{{customer_number}}}}, Welcome to Swiss Postal! " +
                $"Your customer account has been created successfully. You can now send and track parcels. " +
                $"Thank you for your trust. Kind regards, SwiftApp Postal', '{now}', '{system}') " +
                "ON CONFLICT DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM notification_template_translations WHERE notification_template_id IN ('e1000000-0000-0000-0000-000000000004', 'e1000000-0000-0000-0000-000000000005');");
            migrationBuilder.Sql("DELETE FROM notification_templates WHERE \"Id\" IN ('e1000000-0000-0000-0000-000000000004', 'e1000000-0000-0000-0000-000000000005');");
        }
    }
}
