using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using BilApi.Models;
using BilApi.DbConnection;

namespace BilApi.Controllers
{
    public class BilController : ApiController
    {
        [HttpGet]
       // [IdentityBasicAuthentication]
        [Authorize (Roles = "Admin")]
        public IEnumerable<Bil> Get()
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
               
                DatabaseConnector connection = new DatabaseConnector();

                var queryString = this.Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

                //Man skulle kunna tänka sig att man kan skicka in flera regNr. 
                if (queryString.Count == 1)
                {
                    var valuePair = queryString.First();
                    if (valuePair.Key.ToLower() == "regnr")
                    {
                        IEnumerable<Bil> bil = connection.GetBilByRegNr(valuePair.Value);
                        return bil;
                    }
                    //Här hamnar man om man enbart har fyllt i registreringsnumret. dvs ?ABC123  
                    else if (string.IsNullOrEmpty(valuePair.Value))
                    {
                        IEnumerable<Bil> bil = connection.GetBilByRegNr(valuePair.Key);
                        return bil;
                    }
                }

                List<Bil> bilLista = connection.GetAllCars();
                return bilLista;
            }

            //Borde vara ett HttpResponseMessage "Not allowed" eller liknande
            return new List<Bil>();

       
        }

        [HttpGet]
        public IEnumerable<Bil> GetBilById(int Id)
        {
            DatabaseConnector connection = new DatabaseConnector();
            Bil bil = connection.GetBilById(Id);
            yield return bil;
        }



        [HttpPost]
        public int PostBil([FromBody] Bil bil)
        {
            DatabaseConnector connector = new DatabaseConnector();
            return connector.InsertNewBil(bil);
        }


    }
}
