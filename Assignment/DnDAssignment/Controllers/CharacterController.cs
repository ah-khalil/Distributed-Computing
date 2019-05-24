using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using System.Net.Http;
using System.Web;
using System.Xml;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace DnDAssignment.Controllers
{
    [RoutePrefix("DnD/DnDAssignment")]
    public class CharacterController : ApiController
    {
        /*
         * Purpose: Adds a character into the database
         * Params: req - [FromBody] Dictionary<string, string>
         * Return: None
         * Method: POST
         */
        [HttpPost]
        [Route("Characters")]
        public void add_character([FromBody] Dictionary<string, string> req)
        {
            string err_msg = "";
            string insert_q = "";
            SqliteCommand sql_comm = null;

            try
            {
                //Make sure the input character's details are within the bounds they're supposed to be in
                err_msg = validate_character(req);
                
                //If there's no error message
                if (err_msg == "")
                {
                    //Open connection with the database
                    using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                    {
                        db_conn.Open();
                        
                        //Query to execute, parameterized
                        insert_q = @"INSERT INTO characters
                                (name, age, gender, biography, level, race, class, spellcaster, hit_points, ab_con, ab_dex, ab_str, ab_cha, ab_int, ab_wis)
                                VALUES
                                (@name,@age,@gender,@biography,@level,@race,@class,@spellcaster,@hit_points,@ab_con,@ab_dex,@ab_str,@ab_cha,@ab_int,@ab_wis)";
                        
                        //Add all the parameterized values from the query using the character information contained in the request
                        sql_comm = new SqliteCommand(insert_q, db_conn);
                        sql_comm.Parameters.Add(new SqliteParameter("name", req["name"]));
                        sql_comm.Parameters.Add(new SqliteParameter("age", req["age"]));
                        sql_comm.Parameters.Add(new SqliteParameter("gender", req["gender"]));
                        sql_comm.Parameters.Add(new SqliteParameter("biography", req["biography"]));
                        sql_comm.Parameters.Add(new SqliteParameter("level", req["level"]));
                        sql_comm.Parameters.Add(new SqliteParameter("race", req["race"]));
                        sql_comm.Parameters.Add(new SqliteParameter("class", req["class"]));
                        sql_comm.Parameters.Add(new SqliteParameter("spellcaster", req["spellcaster"]));
                        sql_comm.Parameters.Add(new SqliteParameter("hit_points", req["hit_points"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_con", req["ab_con"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_dex", req["ab_dex"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_str", req["ab_str"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_cha", req["ab_cha"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_int", req["ab_int"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_wis", req["ab_wis"]));
                        
                        //Execute query
                        sql_comm.ExecuteNonQuery();
                        db_conn.Close();
                    }
                }
                else
                {
                    //If there is an error message throw exception with error message back the client
                    throw new ArgumentException("Invalid information: \n" + err_msg);
                }
            }
            catch (ArgumentException a_e)
            {
                //throw code 400
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.BadRequest, a_e.Message));
            }
            catch (SqliteException)
            {
                //throw code 500
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred connecting to the database"));
            }
            catch (Exception e)
            {
                //throw code 500
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }
        }
        
        /*
         * Purpose: Gets a specific character from the database
         * Params: id - int
         * Return: JSON string representing retrieved character information
         * Method: GET
         */
        [HttpGet]
        [Route("Characters/{id}")]
        public string get_character(int id)
        {
            string get_char_q = "";
            string json_string = "";
            SqliteCommand sql_comm = null;
            Dictionary<string, string> q_res = null;

            try
            {
                //Open connection with the database
                using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                {
                    db_conn.Open();
                    
                    //create select query to grab only the character with the same ID
                    q_res = new Dictionary<string, string>();
                    get_char_q = "SELECT * FROM characters WHERE ID = @id";
                    sql_comm = new SqliteCommand(get_char_q, db_conn);
                    sql_comm.Parameters.Add(new SqliteParameter("ID", id));
                    
                    using (SqliteDataReader reader = sql_comm.ExecuteReader())
                    {
                        //use .Read() only once as there should be only one result
                        //copy the acquired information from the database into the dictionary
                        reader.Read();
                        q_res.Add("id", reader["ID"].ToString());
                        q_res.Add("name", reader["name"].ToString());
                        q_res.Add("age", reader["age"].ToString());
                        q_res.Add("gender", reader["gender"].ToString());
                        q_res.Add("biography", reader["biography"].ToString());
                        q_res.Add("level", reader["level"].ToString());
                        q_res.Add("race", reader["race"].ToString());
                        q_res.Add("class", reader["class"].ToString());
                        q_res.Add("spellcaster", reader["spellcaster"].ToString());
                        q_res.Add("hit_points", reader["hit_points"].ToString());
                        q_res.Add("ab_con", reader["ab_con"].ToString());
                        q_res.Add("ab_dex", reader["ab_dex"].ToString());
                        q_res.Add("ab_str", reader["ab_str"].ToString());
                        q_res.Add("ab_cha", reader["ab_cha"].ToString());
                        q_res.Add("ab_int", reader["ab_int"].ToString());
                        q_res.Add("ab_wis", reader["ab_wis"].ToString());
                    }
                    
                    //if there were results serialize the q_res object so that character results are stringified
                    if(q_res.Count > 0)
                        json_string = JsonConvert.SerializeObject(q_res, Formatting.None);

                    db_conn.Close();
                }
            }
            catch (SqliteException)
            {
                //throw code 500
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred connecting to the database"));
            }
            catch (Exception e)
            {
                //throw code 500
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            return json_string;
        }
        
        /*
         * Purpose: Gets all characters from the database
         * Params: None
         * Return: None
         * Method: GET
         */
        [HttpGet]
        [Route("Characters")]
        public string get_all_character()
        {
            string json_string = "";
            string get_all_char_q = "";
            SqliteCommand sql_comm = null;
            List<Dictionary<string, string>> q_res = null;
            try
            {
                //Open connection with the database
                using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                {
                    q_res = new List<Dictionary<string, string>>();
                    db_conn.Open();

                    //create select query to grab all characters
                    get_all_char_q = "SELECT * FROM characters;";
                    sql_comm = new SqliteCommand(get_all_char_q, db_conn);

                    using (SqliteDataReader reader = sql_comm.ExecuteReader())
                    {
                        //for each result create a new dictionary entry in the q_res list
                        //and place the character information in that dictionary
                        while (reader.Read())
                        {
                            q_res.Add(new Dictionary<string, string>()
                                {
                                    { "id", reader["ID"].ToString()},
                                    { "name", reader["name"].ToString() },
                                    { "age",  reader["age"].ToString() },
                                    { "gender",  reader["gender"].ToString() },
                                    { "biography",  reader["biography"].ToString() },
                                    { "level",  reader["level"].ToString() },
                                    { "race",  reader["race"].ToString() },
                                    { "class",  reader["class"].ToString() },
                                    { "spellcaster",  reader["spellcaster"].ToString() },
                                    { "hit_points",  reader["hit_points"].ToString() },
                                    { "ab_con",  reader["ab_con"].ToString() },
                                    { "ab_dex",  reader["ab_dex"].ToString() },
                                    { "ab_str",  reader["ab_str"].ToString() },
                                    { "ab_cha",  reader["ab_cha"].ToString() },
                                    { "ab_int",  reader["ab_int"].ToString() },
                                    { "ab_wis",  reader["ab_wis"].ToString() }
                                }
                            );
                        }
                        
                        //serialize the list of dictionaries to send
                        json_string = JsonConvert.SerializeObject(q_res, Formatting.None); 
                    }
                    
                    db_conn.Close();
                }
            }
            catch (SqliteException)
            {
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred connecting to the database"));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            Console.WriteLine(json_string);

            return json_string;
        }
        
        /*
         * Purpose: Gets character with a specific name
         * Params: req - Dictionary<string, string> req
         * Return: JSON string representing the recieved character information
         * Method: POST
         */
        [HttpPost]
        [Route("Characters/Name/")]
        public string get_character([FromBody] Dictionary<string, string> req)
        {
            string get_char_q = "";
            string json_string = "";
            SqliteCommand sql_comm = null;
            Dictionary<string, string> q_res = null;
            
            try
            {
                //Open connection with the database
                using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                {
                    db_conn.Open();
                    
                    //create select query to grab only the character with the same name
                    //acquire name from the request body
                    q_res = new Dictionary<string, string>();
                    get_char_q = "SELECT * FROM characters WHERE name = @name";
                    sql_comm = new SqliteCommand(get_char_q, db_conn);
                    sql_comm.Parameters.Add(new SqliteParameter("name", req["name"]));
                    
                    //there shouldnt be more than one result as name parameter is unique to each character
                    using (SqliteDataReader reader = sql_comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //add the character information into the dictionary
                            q_res.Add("id", reader["ID"].ToString());
                            q_res.Add("name", reader["name"].ToString());
                            q_res.Add("age", reader["age"].ToString());
                            q_res.Add("gender", reader["gender"].ToString());
                            q_res.Add("biography", reader["biography"].ToString());
                            q_res.Add("level", reader["level"].ToString());
                            q_res.Add("race", reader["race"].ToString());
                            q_res.Add("class", reader["class"].ToString());
                            q_res.Add("spellcaster", reader["spellcaster"].ToString());
                            q_res.Add("hit_points", reader["hit_points"].ToString());
                            q_res.Add("ab_con", reader["ab_con"].ToString());
                            q_res.Add("ab_dex", reader["ab_dex"].ToString());
                            q_res.Add("ab_str", reader["ab_str"].ToString());
                            q_res.Add("ab_cha", reader["ab_cha"].ToString());
                            q_res.Add("ab_int", reader["ab_int"].ToString());
                            q_res.Add("ab_wis", reader["ab_wis"].ToString());                            
                        }
                    }
                    
                    //serialize the dictionary to send and then close database connection
                    json_string = JsonConvert.SerializeObject(q_res, Formatting.None);
                    db_conn.Close();
                }
            }
            catch (SqliteException)
            {
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred connecting to the database"));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }

            return json_string;
        }
        
        /*
         * Purpose: Updates a ID-specific character's information
         * Params: req - Dictionary<string, string> req
         * Return: None
         * Method: PUT
         */
        [HttpPut]
        [Route("Characters/{id}")]
        public void update_character([FromBody] Dictionary<string, string> req)
        {
            string err_msg = "";
            string update_q = "";
            SqliteCommand sql_comm = null;

            try
            {
                //check to see if the character contained in the request body is valid
                //if it isnt the err_msg string will non-empty due to subsequent error message
                err_msg = validate_character(req);

                //if character info is valid
                if (err_msg == "")
                {
                    //Open connection with the database 
                    using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                    {
                        //Update the row that contains the ID given in the request body with information supplied
                        db_conn.Open();
                        update_q = @" UPDATE characters SET
                                        name = @name,
                                        age = @age,
                                        gender = @gender,
                                        biography = @biography,
                                        level = @level,
                                        race = @race,
                                        class = @class,
                                        spellcaster = @spellcaster,
                                        hit_points = @hit_points,
                                        ab_con = @ab_con,
                                        ab_dex = @ab_dex,
                                        ab_str = @ab_str,
                                        ab_cha = @ab_cha,
                                        ab_int = @ab_int,
                                        ab_wis = @ab_wis
                                      WHERE id = @id";
                        
                        sql_comm = new SqliteCommand(update_q, db_conn);
                        sql_comm.Parameters.Add(new SqliteParameter("id", req["id"]));
                        sql_comm.Parameters.Add(new SqliteParameter("name", req["name"]));
                        sql_comm.Parameters.Add(new SqliteParameter("age", req["age"]));
                        sql_comm.Parameters.Add(new SqliteParameter("gender", req["gender"]));
                        sql_comm.Parameters.Add(new SqliteParameter("biography", req["biography"]));
                        sql_comm.Parameters.Add(new SqliteParameter("level", req["level"]));
                        sql_comm.Parameters.Add(new SqliteParameter("race", req["race"]));
                        sql_comm.Parameters.Add(new SqliteParameter("class", req["class"]));
                        sql_comm.Parameters.Add(new SqliteParameter("spellcaster", req["spellcaster"]));
                        sql_comm.Parameters.Add(new SqliteParameter("hit_points", req["hit_points"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_con", req["ab_con"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_dex", req["ab_dex"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_str", req["ab_str"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_cha", req["ab_cha"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_int", req["ab_int"]));
                        sql_comm.Parameters.Add(new SqliteParameter("ab_wis", req["ab_wis"]));

                        //execute query and close connection
                        sql_comm.ExecuteNonQuery();
                        db_conn.Close();
                    }
                }
                else
                    throw new ArgumentException("Invalid information: \n" + err_msg);
            }
            catch (ArgumentException a_e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.BadRequest, a_e.Message));
            }
            catch (SqliteException s_e)
            {
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred connecting to the database"));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }
        }
        
        /*
         * Purpose: Delete's an ID-specific character from the database
         * Params: id - int
         * Return: None
         * Method: DELETE
         */
        [HttpDelete]
        [Route("Characters/{id}")]
        public void delete_character(int id)
        {
            string delete_q = "";
            SqliteCommand sql_comm = null;

            try
            {
                //Open connection with the database
                using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                {
                    db_conn.Open();
                    
                    //construct deletion query to delete row where the ID is the one supplied
                    delete_q = "DELETE FROM characters WHERE ID = @id";
                    sql_comm = new SqliteCommand(delete_q, db_conn);
                    sql_comm.Parameters.Add(new SqliteParameter("ID", id));
                    
                    //execute query and close connection
                    sql_comm.ExecuteNonQuery();
                    db_conn.Close();
                }
            }
            catch (SqliteException)
            {
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred connecting to the database"));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError, "An error occurred on the server"));
            }
        }
        
        /*
         * Purpose: Create an XML document out of ID-specific character's information in database
         * Params: id - int
         * Return: None
         * Method: GET
         */
        [HttpGet]
        [Route("Characters/XML/{id}")]
        public void create_xml(int id)
        {
            int rand_num = 0;
            FileInfo file = null;
            string xml_name = "";
            string file_path = "";
            string get_char_q = "";
            Random random_gen = null;
            SqliteCommand sql_comm = null;
            Dictionary<string, string> q_res = null;

            try
            {
                //using random numbers to make created file unique
                random_gen = new Random();
                xml_name = "DnDCharacter";
                q_res = new Dictionary<string, string>();

                //Attempt to create the file using the random number; if there's another file like it then try again with
                //another random number
                do
                {
                    rand_num = random_gen.Next(100000);
                    file_path = "C:/Users/Haytham/Documents/Computing/DC/Distributed-Computing/Assignment/" + xml_name +
                                "_" + id.ToString() + "_" + rand_num + ".xml";
                    file = new FileInfo(file_path);
                } while (file.Exists);
    
                //Open connection with the database
                using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                {
                    db_conn.Open();

                    //construct selection query to select row where the ID is the one supplied
                    get_char_q = "SELECT * FROM characters WHERE ID = @id";
                    sql_comm = new SqliteCommand(get_char_q, db_conn);
                    sql_comm.Parameters.Add(new SqliteParameter("ID", id));
                    
                    using (SqliteDataReader reader = sql_comm.ExecuteReader())
                    {
                        //Add the information retrieved from the database into the dictionary
                        while (reader.Read())
                        {
                            q_res.Add("id", reader["ID"].ToString());
                            q_res.Add("name", reader["name"].ToString());
                            q_res.Add("age", reader["age"].ToString());
                            q_res.Add("gender", reader["gender"].ToString());
                            q_res.Add("biography", reader["biography"].ToString());
                            q_res.Add("level", reader["level"].ToString());
                            q_res.Add("race", reader["race"].ToString());
                            q_res.Add("class", reader["class"].ToString());
                            q_res.Add("spellcaster", reader["spellcaster"].ToString());
                            q_res.Add("hit_points", reader["hit_points"].ToString());
                            q_res.Add("ab_con", reader["ab_con"].ToString());
                            q_res.Add("ab_dex", reader["ab_dex"].ToString());
                            q_res.Add("ab_str", reader["ab_str"].ToString());
                            q_res.Add("ab_cha", reader["ab_cha"].ToString());
                            q_res.Add("ab_int", reader["ab_int"].ToString());
                            q_res.Add("ab_wis", reader["ab_wis"].ToString());
                        }
                    }
                    
                    //close connection
                    db_conn.Close();
                }
                
                //Writing into the path that was created
                using (XmlWriter writer = XmlWriter.Create(file_path))
                {
                    //Write each property of the character with the properties labelled
                    //Spellcaster is not simply a 1 or 0 but rather a Yes/No depending on the number
                    
                    //WriteStartElement and EndElement capture the subsequent information within a character
                    //tag
                    writer.WriteStartElement("Character");
                    writer.WriteElementString("ID", q_res["id"]);
                    writer.WriteElementString("Name", q_res["name"]);
                    writer.WriteElementString("Age", q_res["age"]);
                    writer.WriteElementString("Gender", q_res["gender"]);
                    writer.WriteElementString("Biography", q_res["biography"]);
                    writer.WriteElementString("Level", q_res["level"]);
                    writer.WriteElementString("Race", q_res["race"]);
                    writer.WriteElementString("Class", q_res["class"]);
                    writer.WriteElementString("Spellcaster", (q_res["spellcaster"] == "1") ? "Yes" : "No");
                    writer.WriteElementString("Hit_Points", q_res["hit_points"]);
                    writer.WriteElementString("Constitution_Ability", q_res["ab_con"]);
                    writer.WriteElementString("Dexterity_Ability", q_res["ab_dex"]);
                    writer.WriteElementString("Strength_Ability", q_res["ab_str"]);
                    writer.WriteElementString("Charisma_Ability", q_res["ab_cha"]);
                    writer.WriteElementString("Intelligence_Ability", q_res["ab_int"]);
                    writer.WriteElementString("Wisdom_Ability", q_res["ab_wis"]);
                    writer.WriteEndElement();
                    writer.Flush();
                }
                
                //Add headers to indicate to the browser that there is an attachment with the response
                //The attachment in this case is the XML file that was just created
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                HttpContext.Current.Response.ContentType = "text/xml";
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.TransmitFile(file.FullName);
                HttpContext.Current.Response.End();
            }
            catch (System.Xml.XmlException xml_e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(
                    System.Net.HttpStatusCode.InternalServerError, "An error occurred while creating XML file"));
            }
            catch (SqliteException sql_e)
            {
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError,
                        "An error occurred retrieving character from database"));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    this.Request.CreateResponse<object>(System.Net.HttpStatusCode.InternalServerError,
                        "An error occurred on the server"));
            }
        }
        
        /*
         * Purpose: validate the character information given in the request
         * Params: req_body - Dictionary<string, string>
         * Return: it will either return an empty string or an error message
         */
        private string validate_character(Dictionary<string, string> req_body)
        {
            string[] key_arr;
            string err_msg = "";
            bool keys_valid = true;

            //an array of keys expected to be found in the request body
            key_arr = new[] { "name", "age", "gender", "biography", "level", "race", "class", "spellcaster", "hit_points", "ab_con", "ab_dex", "ab_str", "ab_cha", "ab_int", "ab_wis" };
            
            //run through the above array and make sure the keys are in the request body
            foreach (var key in key_arr)
            {
                if (!req_body.ContainsKey(key))
                {
                    keys_valid = false;
                    err_msg += "Please include all editable fields";
                }
            }
            
            //if the expected keys are in the request body
            if (keys_valid)
            {
                if (req_body["name"] == "")
                    err_msg += "Name must not be empty\n";
                else if (check_name(req_body["name"]))
                    err_msg += "Name must be unique\n";

                if (req_body["age"] == "")
                    err_msg += "Age must not be empty\n";
                else if (!Int32.TryParse(req_body["age"], out int out_int))
                    err_msg += "Age must be a number\n";
                else if (out_int < 0 || out_int > 500)
                    err_msg += "Age must be between 0 and 500\n";

                if (req_body["gender"] == "")
                    err_msg += "Gender must not be empty\n";

                if (req_body["biography"] == "")
                    err_msg += "Biography must not be empty\n";
                else if (req_body["biography"].Length > 500)
                    err_msg += "Biography must be less than 500 characters\n";

                if (req_body["ab_con"] == "" || req_body["ab_dex"] == "" || req_body["ab_str"] == "" ||
                    req_body["ab_cha"] == "" || req_body["ab_int"] == "" || req_body["ab_wis"] == "")
                    err_msg += "Ability scores must not be blank (place a 0 for unapplied stats)\n";
                else if(!Int32.TryParse(req_body["ab_con"], out int out_ab_con) || !Int32.TryParse(req_body["ab_dex"], out int out_ab_dex) || !Int32.TryParse(req_body["ab_str"], out int out_ab_str) ||
                        !Int32.TryParse(req_body["ab_cha"], out int out_ab_cha) || !Int32.TryParse(req_body["ab_int"], out int out_ab_int) || !Int32.TryParse(req_body["ab_wis"], out int out_ab_wis))
                    err_msg += "Ability scores must be numbers\n";

                if (Int32.TryParse(req_body["tts"], out int out_tts) && (out_tts < 0 || out_tts > 20))
                    err_msg += "The number of ability points (not including race bonuses) must be between 0 and 20";
            }

            return err_msg;
        }
        
        private static bool check_name(string name)
        {
            string tbl_q;
            int row_count = 0;
            SqliteCommand command = null;

            try
            {
                using (SqliteConnection db_conn = new SqliteConnection(WebApiConfig.DB_CONN_STR))
                {
                    db_conn.Open();
                    
                    /*Selecting the amount of tables in the sqlite_master table with table_name as a name
                    The usage of sqlite_master is due to every database containing an sqlite_master that
                    contains schema information*/
                    tbl_q = "SELECT COUNT(*) FROM characters WHERE name = @name";
                
                    command = new SqliteCommand(tbl_q, db_conn);
                    command.Parameters.Add(new SqliteParameter("name", name));
                
                    //Execute the command and return a single value; the number of tables
                    row_count = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (SqliteException sql_e)
            {
                throw new SqliteException();
            }
            
            return (row_count == 1);
        }
    }
}