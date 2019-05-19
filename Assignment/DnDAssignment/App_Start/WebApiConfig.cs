using System.IO;
using System.Web.Http;
using Mono.Data.Sqlite;

namespace DnDAssignment
{
    public static class WebApiConfig
    {
        private const string DB_NAME = "dnd_database.sqlite";
        private const string DB_CONN_STR = "DataSource=" + DB_NAME + ";Version=3;";
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

//            config.Routes.MapHttpRoute(
//                name: "DefaultApi",
//                routeTemplate: "api/{controller}/{id}",
//                defaults: new { id = RouteParameter.Optional }
//            );
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.Insert(0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
            // Apply this GLOBALLY so that we don 't have to be bothered with it during other JSON operations

            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings {
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
            create_database();
            database_connect();
            create_character_table();
        }

        private static void create_database()
        {
            try
            {
                if (!File.Exists(DB_NAME))
                {
                    SqliteConnection.CreateFile(DB_NAME);
                }
            }
            catch (SqliteException sql_e)
            {
                //Exception handling here
            }
        }

        private static void database_connect()
        {
            SqliteConnection db_conn;
            
            db_conn = new SqliteConnection(DB_CONN_STR);
            db_conn.Open();
        }

        private static void create_character_table()
        {
            SqliteCommand command = null;
            string character_tbl_sql = "";

            try
            {
                using (SqliteConnection db_conn = new SqliteConnection(DB_CONN_STR))
                {
                    character_tbl_sql =
                        "CREATE TABLE characters (name VARCHAR(SQLITE_MAX_LENGTH) NOT NULL, age INT, gender VARCHAR(SQLITE_MAX_LENGTH), biography VARCHAR(SQLITE_MAX_LENGTH), level INT, race INT, class INT, spellcaster INT, hit_points INT, ab_constitution INT, ab_dexterity INT, ab_strength INT, ab_charisma INT, ab_intelligence INT, ab_wisdom INT)";
                    command = new SqliteCommand(character_tbl_sql, db_conn);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException sql_e)
            {
                //Exception handling here
            }
        }
    }
}
