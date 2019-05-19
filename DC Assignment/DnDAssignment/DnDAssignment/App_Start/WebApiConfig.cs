using System;
using System.IO;
using System.Web.Http;
using Mono.Data.Sqlite;

namespace DnDAssignment
{
    public static class WebApiConfig
    {
        public const string DB_NAME = "dnd_database.sqlite";
        public const string DB_CONN_STR = "Data Source=" + DB_NAME + ";Version=3";

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.Insert(0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
            // Apply this GLOBALLY so that we don 't have to be bothered with it during other JSON operations

            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            /* If we don 't do this then JSON will send dates in a local - time
            format that is not consistently interpreted by Chrome and IE
            ( the default format misses defining the timezone , so it is USELESS .
            This one will always be UTC )*/
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

            database_init();
        }

        private static void database_init()
        {
            SqliteCommand command = null;
            string character_tbl_sql = "";

            try
            {
                if (!File.Exists(DB_NAME))
                {
                    SqliteConnection.CreateFile(DB_NAME);
                }

                using (SqliteConnection db_conn = new SqliteConnection(DB_CONN_STR))
                {
                    db_conn.Open();

                    character_tbl_sql =
                        "CREATE TABLE characters (name VARCHAR(255) NOT NULL, age INT, gender VARCHAR(255), biography VARCHAR(255), level INT, race INT, class INT, spellcaster INT, hit_points INT, ab_constitution INT, ab_dexterity INT, ab_strength INT, ab_charisma INT, ab_intelligence INT, ab_wisdom INT)";
                    command = new SqliteCommand(character_tbl_sql, db_conn);
                    command.ExecuteNonQuery();

                    db_conn.Close();
                }
            }
            catch (SqliteException sql_e)
            {
                Console.WriteLine("Error: " + sql_e.ToString());
            }
        }
    }
}
