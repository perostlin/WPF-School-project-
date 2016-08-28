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
    public class DeeperAnalysisController : ApiController
    {
        #region GetAllVehiclesWithData
        [HttpGet]
        [ActionName("GetAllVehiclesWithData")]
        [ResponseType(typeof(List<DeeperAnalysisModel>))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage GetAllVehiclesWithData()
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Parameters sets to variables.
                        int year = DateTime.Today.Year;
                        int month = DateTime.Today.Month - 1;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList = db.Vehicle.OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year && x.Date.Month == month)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousMonth =
                            db.ReportDriverJournal.Where(x => x.Date.Month == month - 1)
                                .ToList();

                        if (vehicleList.Count == 0 || driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // List of vehicles to return.
                        List<DeeperAnalysisModel> vehiclesToReturn = new List<DeeperAnalysisModel>();

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
                            // Variables used to calculate.
                            decimal totalDriverJournalMilage = 0;
                            decimal totalDriverJournalFuelAmount = 0;
                            decimal totalMilage = 0;
                            decimal totalFuelCostToReturn = 0;

                            // Variables to return.
                            decimal averageFuelConsumptionToReturn = 0;

                            foreach (var driverJournal in driverJournalList)
                            {
                                if (driverJournal.VehicleID == vehicle.ID && previousVehicle == driverJournal.VehicleID)
                                {
                                    totalDriverJournalMilage += driverJournal.Milage - previousMilage;
                                    totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                    totalFuelCostToReturn += driverJournal.TotalPrice;
                                }
                                else if (driverJournal.VehicleID == vehicle.ID)
                                {
                                    if (previousMonth.Count != 0)
                                    {
                                        previousMonth.OrderByDescending(x => x.Date);

                                        int previousMonthMilage = previousMonth.Where(
                                            x => x.VehicleID == vehicle.ID && x.VehicleID == driverJournal.VehicleID)
                                            .Select(x => x.Milage)
                                            .FirstOrDefault();

                                        if (previousMonthMilage != 0)
                                        {
                                            totalDriverJournalMilage += driverJournal.Milage - previousMonthMilage;
                                            totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                            totalFuelCostToReturn += driverJournal.TotalPrice;
                                        }
                                        else
                                        {
                                            int vehicleOriginalMilage =
                                                db.Vehicle.Where(x => x.ID == driverJournal.VehicleID)
                                                    .Select(x => x.OriginalMileage)
                                                    .SingleOrDefault();

                                            totalDriverJournalMilage += driverJournal.Milage - vehicleOriginalMilage;
                                            totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                            totalFuelCostToReturn += driverJournal.TotalPrice;
                                        }
                                    }
                                }

                                // Sets the previous variable's to the last looped DriverJournal.
                                previousVehicle = driverJournal.VehicleID;
                                previousMilage = driverJournal.Milage;
                            }

                            if (totalDriverJournalMilage != 0)
                            {
                                totalMilage = totalDriverJournalMilage;
                                averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                                DeeperAnalysisModel vehicleToAdd = new DeeperAnalysisModel()
                                {
                                    VehicleID = vehicle.ID,
                                    RegNo = vehicle.RegNo,
                                    VehicleType = vehicle.VehicleType.Type,
                                    MilageLatestMonth = Convert.ToInt32(totalMilage),
                                    FuelConsumptionLatestMonth = averageFuelConsumptionToReturn,
                                    FuelCostLatestMonth = totalFuelCostToReturn,
                                    FuelType = vehicle.FuelType.Type
                                };

                                vehiclesToReturn.Add(vehicleToAdd);
                            }
                        }

                        // Sorts the list after lowest average fuelconsumption.
                        vehiclesToReturn.OrderBy(x => x.FuelConsumptionLatestMonth);

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, vehiclesToReturn);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion
    }
}