using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventRegistration.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "Events" ("Name", "StartDateTime", "Location", "Capacity")
                SELECT v."Name", v."StartDateTime", v."Location", v."Capacity"
                FROM (VALUES
                    ('Ljubljana .NET Meetup: Minimal APIs and EF Core',  TIMESTAMP '2026-12-03 18:00' AT TIME ZONE 'Europe/Ljubljana', 'Innovation Hub, Tehnološki park 19, 1000 Ljubljana',            60),
                    ('AI and LLMs in Practice',                           TIMESTAMP '2026-12-10 17:30' AT TIME ZONE 'Europe/Ljubljana', 'Cankarjev dom, Prešernova cesta 10, 1000 Ljubljana',            150),
                    ('Python for Data Science Workshop',                  TIMESTAMP '2026-12-15 16:00' AT TIME ZONE 'Europe/Ljubljana', 'Coworking Maribor, Smetanova ulica 17, 2000 Maribor',           25),
                    ('Rust Beginners Hands-On',                           TIMESTAMP '2027-01-12 18:00' AT TIME ZONE 'Europe/Ljubljana', 'Code Lab, Trubarjeva cesta 25, 1000 Ljubljana',                10),
                    ('DevOps and Kubernetes Night',                       TIMESTAMP '2027-01-21 18:30' AT TIME ZONE 'Europe/Ljubljana', 'Kranj Tech Space, Slovenski trg 1, 4000 Kranj',                 40),
                    ('Cybersecurity Basics for Developers',               TIMESTAMP '2027-02-04 17:00' AT TIME ZONE 'Europe/Ljubljana', 'Celje Digital Center, Glavni trg 17, 3000 Celje',              50),
                    ('Machine Learning Study Group',                      TIMESTAMP '2027-02-18 18:00' AT TIME ZONE 'Europe/Ljubljana', 'Koper Makerspace, Titov trg 4, 6000 Koper',                     15),
                    ('Frontend Friday: React and TypeScript',             TIMESTAMP '2027-02-26 17:00' AT TIME ZONE 'Europe/Ljubljana', 'Novo mesto Innovation Hub, Glavni trg 11, 8000 Novo mesto',    30),
                    ('Cloud Native Slovenia Conference',                  TIMESTAMP '2027-03-11 09:00' AT TIME ZONE 'Europe/Ljubljana', 'Grand Hotel Union, Miklošičeva cesta 1, 1000 Ljubljana',       150),
                    ('Git and Code Review Workshop',                      TIMESTAMP '2027-03-18 17:00' AT TIME ZONE 'Europe/Ljubljana', 'Gorica Dev Space, Trg Edvarda Kardelja 1, 5000 Nova Gorica',   12),
                    ('Prompt Engineering Bootcamp',                       TIMESTAMP '2027-03-25 10:00' AT TIME ZONE 'Europe/Ljubljana', 'Coworking Maribor, Smetanova ulica 17, 2000 Maribor',          40),
                    ('PostgreSQL Performance Tuning',                     TIMESTAMP '2027-04-08 18:00' AT TIME ZONE 'Europe/Ljubljana', 'Innovation Hub, Tehnološki park 19, 1000 Ljubljana',           35),
                    ('Intro to Programming for Beginners',                TIMESTAMP '2027-04-15 16:30' AT TIME ZONE 'Europe/Ljubljana', 'Velenje Library, Titov trg 1, 3320 Velenje',                    20),
                    ('Open Source Contributors Day',                      TIMESTAMP '2027-04-24 10:00' AT TIME ZONE 'Europe/Ljubljana', 'Ptuj Town Hall, Mestni trg 1, 2250 Ptuj',                      45),
                    ('Embedded and IoT Hack Night',                       TIMESTAMP '2027-05-06 19:00' AT TIME ZONE 'Europe/Ljubljana', 'Bled Maker Hub, Cesta svobode 11, 4260 Bled',                  20),
                    ('Data Engineering Meetup',                           TIMESTAMP '2027-05-20 18:00' AT TIME ZONE 'Europe/Ljubljana', 'Pekarna Cultural Center, Pekarniška ulica 3, 2000 Maribor',    70),
                    ('Generative AI Hackathon',                           TIMESTAMP '2027-06-05 09:00' AT TIME ZONE 'Europe/Ljubljana', 'Faculty of Computer and Information Science, Večna pot 113, 1000 Ljubljana', 120),
                    ('Go Language Meetup',                                TIMESTAMP '2027-06-17 18:30' AT TIME ZONE 'Europe/Ljubljana', 'Pomurje Tech Park, Trg zmage 8, 9000 Murska Sobota',           25),
                    ('Clean Architecture in C#',                          TIMESTAMP '2027-09-09 18:00' AT TIME ZONE 'Europe/Ljubljana', 'Celje Digital Center, Glavni trg 17, 3000 Celje',              55),
                    ('AI Ethics and Responsible ML Panel',                TIMESTAMP '2027-09-23 17:30' AT TIME ZONE 'Europe/Ljubljana', 'Praetorian Palace, Titov trg 3, 6000 Koper',                   90)
                ) AS v("Name", "StartDateTime", "Location", "Capacity")
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Events" e
                    WHERE e."Name" = v."Name" AND e."StartDateTime" = v."StartDateTime"
                );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "Events" WHERE "Name" IN (
                'Ljubljana .NET Meetup: Minimal APIs and EF Core',
                'AI and LLMs in Practice',
                'Python for Data Science Workshop',
                'Rust Beginners Hands-On',
                'DevOps and Kubernetes Night',
                'Cybersecurity Basics for Developers',
                'Machine Learning Study Group',
                'Frontend Friday: React and TypeScript',
                'Cloud Native Slovenia Conference',
                'Git and Code Review Workshop',
                'Prompt Engineering Bootcamp',
                'PostgreSQL Performance Tuning',
                'Intro to Programming for Beginners',
                'Open Source Contributors Day',
                'Embedded and IoT Hack Night',
                'Data Engineering Meetup',
                'Generative AI Hackathon',
                'Go Language Meetup',
                'Clean Architecture in C#',
                'AI Ethics and Responsible ML Panel'
                );
            """);
        }
    }
}
