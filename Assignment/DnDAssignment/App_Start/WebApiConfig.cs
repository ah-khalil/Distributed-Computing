using System;
using System.IO;
using System.Net.Mime;
using System.Web.Http;
using Mono.Data.Sqlite;
using Newtonsoft.Json;

namespace DnDAssignment
{
    public static class WebApiConfig
    {
        private const string DB_NAME = "C:/Users/Haytham/Documents/Computing/DC/Distributed-Computing/Assignment/DnDAssignment/dnd_database.sqlite";
        public const string DB_CONN_STR = "Data Source=" + DB_NAME + ";Version=3;";
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.Insert(0, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
    
            /*Setting default JSON Serialize settings so that each call can apply the defaults without additional parameter input*/
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings {
                Formatting = Newtonsoft.Json.Formatting.None,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };

            /*Set JSON time handling to utilize UTC so that it is comprehensible to some browsers*/
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            
            database_init();
        }
        
        /*
         * Purpose: Initializes the database and creates the character table after check to see if it already exists
         * Params: None
         * Return: None
         */
        private static void database_init()
        {
            SqliteCommand command = null;
            string character_tbl_sql = "";

            try
            {
                //check if file already exists
                if (!File.Exists(DB_NAME))
                {
                    //if not, create it
                    SqliteConnection.CreateFile(DB_NAME);
                }
                
                using (SqliteConnection db_conn = new SqliteConnection(DB_CONN_STR))
                {
                    //open a connection with the sqlite file
                    db_conn.Open();
                    
                    //if the characters table doesnt exist then create it
                    if (!check_table(db_conn, "characters"))
                    {
                        character_tbl_sql =
                            @"CREATE TABLE characters(
                              ID INTEGER PRIMARY KEY,
                              name VARCHAR(255) UNIQUE,
                              age INT,
                              gender VARCHAR(255),
                              biography VARCHAR(500),
                              level INT,
                              race VARCHAR(255),
                              class VARCHAR(255),
                              spellcaster INT,
                              hit_points INT,
                              ab_con INT,
                              ab_dex INT,
                              ab_str INT,
                              ab_cha INT,
                              ab_int INT,
                              ab_wis INT
                            )";
                        
                        //register the string as a command and execute it
                        command = new SqliteCommand(character_tbl_sql, db_conn);
                        command.ExecuteNonQuery();
                    }

                    db_conn.Close();
                }
            }
            catch (IOException io_e)
            {
                Environment.Exit(1);
            }
            catch (SqliteException sql_e)
            {
                Environment.Exit(1);
            }
        }

        /*
         * Purpose: Checks to see if a database already has a table of the input name
         * Params: db_conn - SqliteConnection
         * Params: table_name - string
         * Return: whether or not the table_name table exists in database connected to by db_conn
         */
        private static bool check_table(SqliteConnection db_conn, string table_name)
        {
            string tbl_q;
            int row_count = 0;
            SqliteCommand command = null;

            try
            {
                /*Selecting the amount of tables in the sqlite_master table with table_name as a name
                The usage of sqlite_master is due to every database containing an sqlite_master that
                contains schema information*/
                tbl_q = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @name";
                
                command = new SqliteCommand(tbl_q, db_conn);
                command.Parameters.Add(new SqliteParameter("name", table_name));
                
                //Execute the command and return a single value; the number of tables
                row_count = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (SqliteException sql_e)
            {
                /*Because the function is part of the initialization process, if something goes wrong just quit*/
                Environment.Exit(1);
            }
            
            return (row_count == 1);
        }
    }
}
