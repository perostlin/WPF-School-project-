using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BilApi.Models;

namespace BilApi.DbConnection
{
    public class DatabaseConnector
    {
        string connectionString = "Server=bigwp4swfr.database.windows.net; Initial Catalog =bildatabasen; User Id=david; Password=b6A0$21!g%XA";
        SqlConnection dbConnection;

        public DatabaseConnector()
        {
            dbConnection = new SqlConnection(connectionString);
        }

        public List<Bil> GetAllCars()
        {
            dbConnection.Open();
            string cmdText = "GetAllCars";
            SqlCommand command = new SqlCommand(cmdText, dbConnection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader reader = command.ExecuteReader();

            List<Bil> bilLista = new List<Bil>();
            while (reader.Read())
            {
                Bil bil = new Bil();
                bil.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                bil.Marke = reader.GetString(reader.GetOrdinal("Marke"));
                bil.Modellbeteckning = reader.GetString(reader.GetOrdinal("Modellbeteckning"));
                bil.Registreringsnummer = reader.GetString(reader.GetOrdinal("RegNr"));
                bil.Farg = reader.GetString(reader.GetOrdinal("Farg"));
                bil.Metalliclack = reader.GetBoolean(reader.GetOrdinal("MetallicLack"));
                bil.Arsmodell = reader.GetInt32(reader.GetOrdinal("Arsmodell"));

                bilLista.Add(bil);
            }


            dbConnection.Close();

            return bilLista;
        }

        public Bil GetBilById(int Id)
        {
            Bil bilToReturn = new Bil();

            dbConnection.Open();
            string cmdText = "GetAllCars";
            SqlCommand command = new SqlCommand(cmdText, dbConnection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader reader = command.ExecuteReader();

            /// Gör inte så här! Skapa en ny SP istället.
            while (reader.Read())
            {
                if (reader.GetInt32(reader.GetOrdinal("Id")) == Id)
                {

                    bilToReturn.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                    bilToReturn.Marke = reader.GetString(reader.GetOrdinal("Marke"));
                    bilToReturn.Modellbeteckning = reader.GetString(reader.GetOrdinal("Modellbeteckning"));
                    bilToReturn.Registreringsnummer = reader.GetString(reader.GetOrdinal("RegNr"));
                    bilToReturn.Farg = reader.GetString(reader.GetOrdinal("Farg"));
                    bilToReturn.Metalliclack = reader.GetBoolean(reader.GetOrdinal("MetallicLack"));
                    bilToReturn.Arsmodell = reader.GetInt32(reader.GetOrdinal("Arsmodell"));

                    break;
                }

            }


            dbConnection.Close();

            return bilToReturn;

        }


        public IEnumerable<Bil> GetBilByRegNr(string RegNr)
        {
            Bil bilToReturn = new Bil();

            dbConnection.Open();
            string cmdText = "GetCarByRegNr";
            SqlCommand command = new SqlCommand(cmdText, dbConnection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("RegNr", RegNr));
            SqlDataReader reader = command.ExecuteReader();

            //Endast en rad ska vara resultatet, annars har vi dubbletter i DBn. Hanteras ej här.
            if (reader.Read())
            {
                    bilToReturn.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                    bilToReturn.Marke = reader.GetString(reader.GetOrdinal("Marke"));
                    bilToReturn.Modellbeteckning = reader.GetString(reader.GetOrdinal("Modellbeteckning"));
                    bilToReturn.Registreringsnummer = reader.GetString(reader.GetOrdinal("RegNr"));
                    bilToReturn.Farg = reader.GetString(reader.GetOrdinal("Farg"));
                    bilToReturn.Metalliclack = reader.GetBoolean(reader.GetOrdinal("MetallicLack"));
                    bilToReturn.Arsmodell = reader.GetInt32(reader.GetOrdinal("Arsmodell"));
            }


            dbConnection.Close();

            yield return bilToReturn;

        }

        public int InsertNewBil(Bil bil)
        {
            dbConnection.Open();
            string cmdText = "InsertNewCar";
            SqlCommand command = new SqlCommand(cmdText, dbConnection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("RegNr", bil.Registreringsnummer));
            command.Parameters.Add(new SqlParameter("Arsmodell", bil.Arsmodell));
            command.Parameters.Add(new SqlParameter("Modellbeteckning", bil.Modellbeteckning));
            command.Parameters.Add(new SqlParameter("Marke", bil.Marke));
            command.Parameters.Add(new SqlParameter("Farg", bil.Farg));

            int nrOfRows = command.ExecuteNonQuery();
            

            dbConnection.Close();
            return nrOfRows;
        }



    }
}