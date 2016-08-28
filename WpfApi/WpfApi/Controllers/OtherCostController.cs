using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WpfApi.Models;

namespace WpfApi.Controllers
{
    public class OtherCostController : ApiController
    {
        #region FillTypeOfCostCombobox
        [HttpGet]
        [ActionName("FillTypeOfCostCombobox")]
        [ResponseType(typeof(List<TypeOfCostModel>))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage FillTypeOfCostCombobox()
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        var typeOfCostList = db.TypeOfCost.ToList();

                        if (typeOfCostList.Count != 0)
                        {
                            List<TypeOfCostModel> typeOfCostToReturn = new List<TypeOfCostModel>();

                            foreach (var typeOfCost in typeOfCostList)
                            {
                                typeOfCostToReturn.Add(new TypeOfCostModel()
                                {
                                    ID = typeOfCost.ID,
                                    Type = typeOfCost.Type
                                });
                            }

                            return Request.CreateResponse(HttpStatusCode.OK, typeOfCostToReturn);
                        }
                        
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion FillFuelTypeByVehicleID

        #region AddNewCost
        [HttpPost]
        [ActionName("addnewcost")]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage AddNewCost([FromBody]OtherCostModel otherCost)
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        RefuelingDriverJournal otherCostToAdd = new RefuelingDriverJournal
                        {
                            ID = Guid.NewGuid(),
                            Date = otherCost.Date,
                            Cost = Convert.ToDecimal(otherCost.Cost),
                            Comment = otherCost.Comment,
                            TypeOfCostID = otherCost.TypeOfCostID,
                            VehicleID = otherCost.VehicleID
                        };

                        // Lägger till och sparar ny övrig kostnad.
                        db.RefuelingDriverJournal.Add(otherCostToAdd);
                        db.SaveChanges();

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion AddNewCost
    }
}