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
    public class ReportDriverJournalController : ApiController
    {
        #region FillFuelTypeByVehicleID
        [HttpPost]
        [ActionName("FillFuelTypeByVehicleID")]
        [ResponseType(typeof(List<FuelTypeModel>))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage FillFuelTypeByVehicleID([FromBody]VehicleModel vehicle)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        var selectedVehicle = db.Vehicle.Where(x => x.ID == vehicle.ID).SingleOrDefault();

                        if (selectedVehicle != null)
                        {
                            List<FuelTypeModel> fuelTypeToReturn = new List<FuelTypeModel>();

                            if (selectedVehicle.FuelType.Type != null)
                            {
                                if (selectedVehicle.FuelTypeID == 4 || selectedVehicle.FuelType.Type == "Hybrid Bensin/El")
                                {
                                    fuelTypeToReturn.Add(new FuelTypeModel()
                                    {
                                        ID =
                                            db.FuelType.Where(x => x.Type == "Bensin")
                                                .Select(x => x.ID)
                                                .FirstOrDefault(),
                                        Type = "Bensin"
                                    });
                                }
                                else if (selectedVehicle.FuelTypeID == 5 || selectedVehicle.FuelType.Type == "Hybrid Etanol/Bensin")
                                {
                                    fuelTypeToReturn.Add(new FuelTypeModel()
                                    {
                                        ID =
                                            db.FuelType.Where(x => x.Type == "Etanol")
                                                .Select(x => x.ID)
                                                .FirstOrDefault(),
                                        Type = "Etanol"
                                    });
                                    fuelTypeToReturn.Add(new FuelTypeModel()
                                    {
                                        ID =
                                            db.FuelType.Where(x => x.Type == "Bensin")
                                                .Select(x => x.ID)
                                                .FirstOrDefault(),
                                        Type = "Bensin"
                                    });
                                }
                                else
                                {
                                    fuelTypeToReturn.Add(new FuelTypeModel()
                                    {
                                        ID = selectedVehicle.FuelTypeID,
                                        Type =
                                            db.FuelType.Where(x => x.ID == selectedVehicle.FuelTypeID)
                                                .Select(x => x.Type)
                                                .FirstOrDefault()
                                    });
                                }
                            }

                            return Request.CreateResponse(HttpStatusCode.OK, fuelTypeToReturn);
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

        #region FillDriverJournalByVehicleID
        [HttpPost]
        [ActionName("FillDriverJournalByVehicleID")]
        [ResponseType(typeof(List<ReportDriverJournalModel>))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage FillDriverJournalByVehicleID([FromBody]VehicleModel vehicle)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        var driverJournalList = db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID).ToList();

                        if (driverJournalList != null)
                        {
                            List<ReportDriverJournalModel> driverJournalToReturn = new List<ReportDriverJournalModel>();

                            foreach (var driverJournal in driverJournalList)
                            {
                                driverJournalToReturn.Add(new ReportDriverJournalModel()
                                {
                                    ID = driverJournal.ID,
                                    Date = driverJournal.Date,
                                    Milage = driverJournal.Milage,
                                    FuelAmount = driverJournal.FuelAmount,
                                    PricePerUnit = driverJournal.PricePerUnit,
                                    TotalPrice = driverJournal.TotalPrice,
                                    ChauffeurID = driverJournal.ChauffeurID,
                                    FuelTypeID = driverJournal.FuelTypeID,
                                    VehicleID = driverJournal.VehicleID
                                });
                            }

                            return Request.CreateResponse(HttpStatusCode.OK, driverJournalToReturn);
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
        #endregion FillDriverJournalByVehicleID

        #region AddDriverJournal
        [HttpPost]
        [ActionName("AddDriverJournal")]
        [Authorize(Roles = "User")]
        public HttpResponseMessage AddDriverJournal([FromBody]ReportDriverJournal driverJournal)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        ReportDriverJournal journalToAdd = new ReportDriverJournal
                        {
                            ID = Guid.NewGuid(),
                            Date = driverJournal.Date,
                            Milage = driverJournal.Milage,
                            PricePerUnit = driverJournal.PricePerUnit,
                            FuelAmount = driverJournal.FuelAmount,
                            TotalPrice = driverJournal.TotalPrice,
                            ChauffeurID = driverJournal.ChauffeurID,
                            FuelTypeID = driverJournal.FuelTypeID,
                            VehicleID = driverJournal.VehicleID
                        };

                        // Lägger till och sparar ny körjournal.
                        db.ReportDriverJournal.Add(journalToAdd);
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
        #endregion AddDriverJournal
    }
}