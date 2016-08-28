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
    public class CalulateController : ApiController
    {
        #region Vehicle
        #region CalulateFuelConsumptionByVehicleID
        [HttpPost]
        [ActionName("CalulateFuelConsumptionByVehicleID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateFuelConsumptionByVehicleID([FromBody]Guid id)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        decimal totalMilage = 0;
                        decimal totalFuelAmount = 0;
                        decimal averageFuelComsumption = 0;
                        decimal totalFuelCost = 0;

                        Guid idToCheck = id;

                        Vehicle vehicle = db.Vehicle.Where(x => x.ID == idToCheck).SingleOrDefault();

                        List<ReportDriverJournal> driverJournalList = null;
                        ReportDriverJournal driverJournal = null;
                        if (vehicle != null)
                        {
                            driverJournalList =
                                db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID).ToList();

                            driverJournalList.RemoveAll(x => x.FuelTypeID == 1);
                        }
                        if (vehicle.FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }
                        else if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }
                        else if (driverJournalList.Count == 1)
                        {
                            driverJournal = driverJournalList.OrderByDescending(x => x.Date).FirstOrDefault();
                            totalMilage = driverJournal.Milage - vehicle.OriginalMileage;
                        }
                        else if (driverJournalList.Count >= 2)
                        {
                            driverJournal =
                                driverJournalList.OrderByDescending(x => x.Date).FirstOrDefault();
                            ReportDriverJournal beforeLatestDriverJournal =
                                driverJournalList.OrderByDescending(x => x.Date).ElementAt(1);

                            totalMilage = driverJournal.Milage - beforeLatestDriverJournal.Milage;
                        }

                        totalFuelAmount = driverJournal.FuelAmount;
                        averageFuelComsumption += totalFuelAmount / totalMilage;
                        totalFuelCost = driverJournal.TotalPrice;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelComsumption,
                            TotalFuelValue = totalFuelCost
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumption

        #region CalulateMonthlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateMonthlyFuelConsumption")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateMonthlyFuelConsumption([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        Guid vehicleID = values.VehicleID;
                        int year = values.Year;
                        int month = values.Month;

                        List<Vehicle> vehicleList = db.Vehicle.Where(x => x.ID == vehicleID).OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year && x.Date.Month == month)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();


                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }
                        else if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousMonth =
                            db.ReportDriverJournal.Where(x => x.Date.Month == month - 1)
                                .ToList();

                        previousMonth.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                        }

                        if (totalDriverJournalMilage != 0)
                        {
                            totalMilage = totalDriverJournalMilage;
                            averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateMonthlyFuelConsumption

        #region CalulateYearlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateYearlyFuelConsumption")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateYearlyFuelConsumption([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        Guid vehicleID = values.VehicleID;
                        int year = values.Year;
                        int month = values.Month;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList = db.Vehicle.Where(x => x.ID == vehicleID).OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();


                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }
                        else if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousYear =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year - 1)
                                .ToList();

                        previousYear.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                                    if (previousYear.Count != 0)
                                    {
                                        previousYear.OrderByDescending(x => x.Date);

                                        int lastYearMilage = previousYear.Where(
                                            x => x.VehicleID == vehicle.ID && x.VehicleID == driverJournal.VehicleID)
                                            .Select(x => x.Milage)
                                            .FirstOrDefault();

                                        if (lastYearMilage != 0)
                                        {
                                            totalDriverJournalMilage += driverJournal.Milage - lastYearMilage;
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
                        }

                        totalMilage = totalDriverJournalMilage;
                        averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumption

        #region CalulateFuelConsumptionSinceTheBeginning
        [HttpPost]
        [ActionName("CalulateFuelConsumptionSinceTheBeginning")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateFuelConsumptionSinceTheBeginning([FromBody]Guid id)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        Guid vehicleID = id;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList = db.Vehicle.Where(x => x.ID == vehicleID).OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's since the beginning.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();


                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }
                        else if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                                    int vehicleOriginalMilage =
                                        db.Vehicle.Where(x => x.ID == driverJournal.VehicleID)
                                            .Select(x => x.OriginalMileage)
                                            .SingleOrDefault();

                                    totalDriverJournalMilage += driverJournal.Milage - vehicleOriginalMilage;
                                    totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                    totalFuelCostToReturn += driverJournal.TotalPrice;
                                }

                                // Sets the previous variable's to the last looped DriverJournal.
                                previousVehicle = driverJournal.VehicleID;
                                previousMilage = driverJournal.Milage;
                            }
                        }

                        totalMilage = totalDriverJournalMilage;
                        averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumptionSinceTheBeginning

        #region CalulateTotalVehicleCost
        [HttpPost]
        [ActionName("CalulateTotalVehicleCost")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateTotalVehicleCost([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        decimal totalMontlyCost = 0;
                        decimal totalYearlyCost = 0;

                        Guid vehicleID = values.VehicleID;
                        int year = values.Year;
                        int month = values.Month;

                        Vehicle vehicle = db.Vehicle.Where(x => x.ID == vehicleID).SingleOrDefault();

                        List<ReportDriverJournal> driverJournalListYearly =
                            db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year).ToList();

                        List<ReportDriverJournal> driverJournalListMonthly =
                            db.ReportDriverJournal.Where(
                                x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month).ToList();

                        driverJournalListYearly.RemoveAll(x => x.FuelTypeID == 1);
                        driverJournalListMonthly.RemoveAll(x => x.FuelTypeID == 1);

                        List<RefuelingDriverJournal> otherCostListYearly =
                            db.RefuelingDriverJournal.Where(x => x.VehicleID == vehicleID && x.Date.Year == year)
                                .ToList();

                        List<RefuelingDriverJournal> otherCostListMonthly =
                            db.RefuelingDriverJournal.Where(
                                x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month).ToList();

                        if (driverJournalListYearly.Count != 0 || driverJournalListMonthly.Count != 0)
                        {
                            // Loops througt all driver journals.
                            foreach (var driverJournalYearly in driverJournalListYearly)
                            {
                                totalYearlyCost += driverJournalYearly.TotalPrice;
                            }

                            foreach (var driverJournalMonthly in driverJournalListMonthly)
                            {
                                totalMontlyCost += driverJournalMonthly.TotalPrice;
                            }
                        }
                        if (otherCostListYearly.Count != 0 || otherCostListMonthly.Count != 0)
                        {
                            // Loops througt all driver journals.
                            foreach (var otherCostYearly in otherCostListYearly)
                            {
                                totalYearlyCost += otherCostYearly.Cost;
                            }

                            foreach (var otherCostMonthly in otherCostListMonthly)
                            {
                                totalMontlyCost += otherCostMonthly.Cost;
                            }
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            TotalMonthlyVehicleCost = totalMontlyCost,
                            TotalYearlyVehicleCost = totalYearlyCost
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateTotalVehicleCost
        #endregion

        #region Chauffeur
        #region CalulateFuelConsumption
        [HttpPost]
        [ActionName("CalulateFuelConsumptionByChauffeurID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateFuelConsumptionByChauffeurID([FromBody]Guid id)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    // Variables used to calculate.
                    decimal totalDriverJournalMilage = 0;
                    decimal totalDriverJournalFuelAmount = 0;
                    decimal totalDriverJournalFuelCost = 0;
                    decimal totalMilage = 0;
                    decimal averageFuelAmountToReturn = 0;
                    int countDriverJournalsOnChauffeurID = 0;

                    // Variables to return.
                    decimal averageFuelConsumptionToReturn = 0;
                    decimal totalFuelCostToReturn = 0;

                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Parameters sets to variables.
                        Guid chauffeurID = id;

                        List<ReportDriverJournal> allDriverJournalsList =
                            db.ReportDriverJournal.OrderByDescending(x => x.Date).ToList();

                        // Gets latest DriverJournal's.
                        ReportDriverJournal latestDriverJournal =
                            allDriverJournalsList.Where(x => x.ChauffeurID == chauffeurID).OrderByDescending(x => x.Date).FirstOrDefault();

                        if (latestDriverJournal == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        // Gets previous DriverJournal's.
                        int previousDriverJournalMilage =
                            allDriverJournalsList.Where(x => x.VehicleID == latestDriverJournal.VehicleID)
                                .OrderByDescending(x => x.Date).Select(x => x.Milage)
                                .ElementAt(1);

                        if (previousDriverJournalMilage != 0)
                        {
                            totalDriverJournalMilage += latestDriverJournal.Milage - previousDriverJournalMilage;
                            totalDriverJournalFuelAmount += latestDriverJournal.FuelAmount;
                            totalFuelCostToReturn += latestDriverJournal.TotalPrice;
                        }
                        else
                        {
                            int vehicleOriginalMilage =
                                db.Vehicle.Where(x => x.ID == latestDriverJournal.VehicleID)
                                    .Select(x => x.OriginalMileage)
                                    .SingleOrDefault();

                            totalDriverJournalMilage += latestDriverJournal.Milage - vehicleOriginalMilage;
                            totalDriverJournalFuelAmount += latestDriverJournal.FuelAmount;
                            totalFuelCostToReturn += latestDriverJournal.TotalPrice;
                        }

                        countDriverJournalsOnChauffeurID =
                            allDriverJournalsList.Where(x => x.ChauffeurID == chauffeurID).ToList().Count;
                    }

                    totalMilage = totalDriverJournalMilage;

                    averageFuelConsumptionToReturn = totalDriverJournalFuelAmount / totalMilage;
                    averageFuelAmountToReturn = totalDriverJournalFuelAmount / countDriverJournalsOnChauffeurID;
                    totalDriverJournalFuelCost = totalFuelCostToReturn / countDriverJournalsOnChauffeurID;

                    CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                    {
                        AverageFuelConsumption = averageFuelAmountToReturn,
                        AverageFuelValue = averageFuelConsumptionToReturn,
                        TotalFuelValue = totalFuelCostToReturn,
                        AverageFuelCost = totalDriverJournalFuelCost,
                    };

                    // Returnerar OK om det går igenom.
                    return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumption

        #region CalulateMonthlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateMonthlyFuelConsumptionByChauffeurID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateMonthlyFuelConsumptionByChauffeurID([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;
                        int countDriverJournalsOnChauffeurID = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        Guid chauffeurID = values.ChauffeurID;
                        int year = values.Year;
                        int month = values.Month;

                        // Gets all DriverJournal's year and month.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year && x.Date.Month == month)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date)
                                .ToList();

                        // Gets all DriverJournal's from previous month.
                        List<ReportDriverJournal> previousMonth =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year && x.Date.Month == month - 1)
                                .ToList();

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var driverJournal in driverJournalList)
                        {
                            if (driverJournal.ChauffeurID == chauffeurID && previousVehicle == driverJournal.VehicleID)
                            {
                                totalDriverJournalMilage += driverJournal.Milage - previousMilage;
                                totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                totalFuelCostToReturn += driverJournal.TotalPrice;
                            }
                            else if (driverJournal.ChauffeurID == chauffeurID)
                            {
                                if (previousMonth.Count != 0)
                                {
                                    previousMonth.OrderByDescending(x => x.Date);

                                    int previousMonthMilage = previousMonth.Where(
                                        x => x.ChauffeurID == chauffeurID && x.VehicleID == driverJournal.VehicleID)
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

                        countDriverJournalsOnChauffeurID =
                            driverJournalList.Where(x => x.ChauffeurID == chauffeurID).ToList().Count;

                        decimal averageFuelConsumption = 0;
                        decimal averageFuelCost = 0;
                        if (totalDriverJournalMilage != 0)
                        {
                            totalMilage = totalDriverJournalMilage;
                            averageFuelConsumptionToReturn = totalDriverJournalFuelAmount / totalMilage;
                            averageFuelConsumption = totalDriverJournalFuelAmount / countDriverJournalsOnChauffeurID;
                            averageFuelCost = totalFuelCostToReturn / countDriverJournalsOnChauffeurID;
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn,
                            AverageFuelConsumption = averageFuelConsumption,
                            AverageFuelCost = averageFuelCost
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateMonthlyFuelConsumption

        #region CalulateYearlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateYearlyFuelConsumptionByChauffeurID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateYearlyFuelConsumptionByChauffeurID([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;
                        int countDriverJournalsOnChauffeurID = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        Guid chauffeurID = values.ChauffeurID;
                        int year = values.Year;

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousYear =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year - 1)
                                .ToList();

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var driverJournal in driverJournalList)
                        {
                            if (driverJournal.ChauffeurID == chauffeurID && previousVehicle == driverJournal.VehicleID)
                            {
                                totalDriverJournalMilage += driverJournal.Milage - previousMilage;
                                totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                totalFuelCostToReturn += driverJournal.TotalPrice;
                            }
                            else if (driverJournal.ChauffeurID == chauffeurID)
                            {
                                if (previousYear.Count != 0)
                                {
                                    previousYear.OrderByDescending(x => x.Date);

                                    int lastYearMilage = previousYear.Where(
                                        x => x.ChauffeurID == chauffeurID && x.VehicleID == driverJournal.VehicleID)
                                        .Select(x => x.Milage)
                                        .FirstOrDefault();

                                    if (lastYearMilage != 0)
                                    {
                                        totalDriverJournalMilage += driverJournal.Milage - lastYearMilage;
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

                        countDriverJournalsOnChauffeurID =
                            driverJournalList.Where(x => x.ChauffeurID == chauffeurID).ToList().Count;

                        decimal averageFuelConsumption = 0;
                        decimal averageFuelCost = 0;
                        if (totalDriverJournalMilage != 0)
                        {
                            totalMilage = totalDriverJournalMilage;
                            averageFuelConsumptionToReturn = totalDriverJournalFuelAmount / totalMilage;
                            averageFuelConsumption = totalDriverJournalFuelAmount / countDriverJournalsOnChauffeurID;
                            averageFuelCost = totalFuelCostToReturn / countDriverJournalsOnChauffeurID;
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn,
                            AverageFuelConsumption = averageFuelConsumption,
                            AverageFuelCost = averageFuelCost
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumption

        #region CalulateFuelConsumptionSinceTheBeginning
        [HttpPost]
        [ActionName("CalulateFuelConsumptionSinceTheBeginningByChauffeurID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateFuelConsumptionSinceTheBeginningByChauffeurID([FromBody]Guid id)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;
                        int countDriverJournalsOnChauffeurID = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        Guid chauffeurID = id;

                        // Gets all DriverJournal's since the beginning.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Milage)
                                .ThenBy(x => x.Date)
                                .ToList();

                        if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's.
                        foreach (var driverJournal in driverJournalList)
                        {
                            if (driverJournal.ChauffeurID == chauffeurID && previousVehicle == driverJournal.VehicleID)
                            {
                                totalDriverJournalMilage += driverJournal.Milage - previousMilage;
                                totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                totalFuelCostToReturn += driverJournal.TotalPrice;
                            }
                            else if (driverJournal.ChauffeurID == chauffeurID)
                            {
                                int vehicleOriginalMilage =
                                    db.Vehicle.Where(x => x.ID == driverJournal.VehicleID)
                                        .Select(x => x.OriginalMileage)
                                        .SingleOrDefault();

                                totalDriverJournalMilage += driverJournal.Milage - vehicleOriginalMilage;
                                totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                totalFuelCostToReturn += driverJournal.TotalPrice;
                            }

                            // Sets the previous variable's to the last looped DriverJournal.
                            previousVehicle = driverJournal.VehicleID;
                            previousMilage = driverJournal.Milage;
                        }

                        countDriverJournalsOnChauffeurID =
                            driverJournalList.Where(x => x.ChauffeurID == chauffeurID).ToList().Count;

                        decimal averageFuelConsumption = 0;
                        decimal averageFuelCost = 0;
                        if (totalDriverJournalMilage != 0)
                        {
                            totalMilage = totalDriverJournalMilage;
                            averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;
                            averageFuelConsumption = totalDriverJournalFuelAmount / countDriverJournalsOnChauffeurID;
                            averageFuelCost = totalFuelCostToReturn / countDriverJournalsOnChauffeurID;
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn,
                            AverageFuelConsumption = averageFuelConsumption,
                            AverageFuelCost = averageFuelCost
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumptionSinceTheBeginning

        #region CalulateTotalVehicleCost
        //[HttpPost]
        //[ActionName("CalulateTotalVehicleCost")]
        //[ResponseType(typeof(CalculateValuesModel))]
        //public HttpResponseMessage CalulateTotalVehicleCost([FromBody]CalculateValuesModel values)
        //{
        //    try
        //    {
        //        using (var db = new WpfProjectDatabaseEntities())
        //        {
        //            decimal totalMontlyCost = 0;
        //            decimal totalYearlyCost = 0;

        //            Guid vehicleID = values.VehicleID;
        //            int year = values.Year;
        //            int month = values.Month;

        //            Vehicle vehicle = db.Vehicle.Where(x => x.ID == vehicleID).SingleOrDefault();

        //            List<ReportDriverJournal> driverJournalListYearly =
        //                db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year).ToList();

        //            List<ReportDriverJournal> driverJournalListMonthly =
        //                db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month).ToList();

        //            List<RefuelingDriverJournal> otherCostListYearly =
        //                db.RefuelingDriverJournal.Where(x => x.VehicleID == vehicleID && x.Date.Year == year).ToList();

        //            List<RefuelingDriverJournal> otherCostListMonthly =
        //                db.RefuelingDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month).ToList();

        //            if (driverJournalListYearly.Count != 0 || driverJournalListMonthly.Count != 0 ||
        //                otherCostListYearly.Count != 0 || otherCostListMonthly.Count != 0)
        //            {
        //                // Loops througt all driver journals.
        //                foreach (var driverJournalYearly in driverJournalListYearly)
        //                {
        //                    totalYearlyCost += driverJournalYearly.TotalPrice;
        //                }

        //                foreach (var driverJournalMonthly in driverJournalListMonthly)
        //                {
        //                    totalMontlyCost += driverJournalMonthly.TotalPrice;
        //                }

        //                // Loops througt all driver journals.
        //                foreach (var otherCostYearly in otherCostListYearly)
        //                {
        //                    totalYearlyCost += otherCostYearly.Cost;
        //                }

        //                foreach (var otherCostMonthly in otherCostListMonthly)
        //                {
        //                    totalMontlyCost += otherCostMonthly.Cost;
        //                }
        //            }

        //            CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
        //            {
        //                TotalMonthlyVehicleCost = totalMontlyCost,
        //                TotalYearlyVehicleCost = totalYearlyCost
        //            };

        //            // Returnerar OK om det går igenom.
        //            return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
        //    }
        //}
        #endregion CalulateTotalVehicleCost
        #endregion

        #region VehicleType
        #region CalulateFuelConsumption
        //[HttpPost]
        //[ActionName("CalulateFuelConsumptionByVehicleTypeID")]
        //[ResponseType(typeof(CalculateValuesModel))]
        //public HttpResponseMessage CalulateFuelConsumptionByVehicleTypeID([FromBody]int id)
        //{
        //    try
        //    {
        //        using (var db = new WpfProjectDatabaseEntities())
        //        {
        //            decimal totalMilage = 0;
        //            decimal totalFuelAmount = 0;
        //            decimal averageFuelComsumption = 0;
        //            decimal totalFuelCost = 0;

        //            int idToCheck = id;

        //            List<Vehicle> vehicleList = db.Vehicle.Where(x => x.VehicleTypeID == idToCheck).ToList();

        //            List<ReportDriverJournal> driverJournalListToReturn = new List<ReportDriverJournal>();
        //            ReportDriverJournal latestDriverJournal = null;
        //            ReportDriverJournal beforeLatestDriverJournal = null;

        //            foreach (var vehicle in vehicleList)
        //            {
        //                List<ReportDriverJournal> reportDriverJournalListToLoop = db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID).ToList();

        //                foreach (var reportDriverJournal in reportDriverJournalListToLoop)
        //                {
        //                    ReportDriverJournal reportDriverJournalToAdd = new ReportDriverJournal()
        //                    {
        //                        ID = reportDriverJournal.ID,
        //                        Date = reportDriverJournal.Date,
        //                        FuelAmount = reportDriverJournal.FuelAmount,
        //                        FuelType = reportDriverJournal.FuelType,
        //                        Milage = reportDriverJournal.Milage,
        //                        TotalPrice = reportDriverJournal.TotalPrice,
        //                        PricePerUnit = reportDriverJournal.PricePerUnit,
        //                        ChauffeurID = reportDriverJournal.ChauffeurID,
        //                        FuelTypeID = reportDriverJournal.FuelTypeID,
        //                        VehicleID = reportDriverJournal.VehicleID
        //                    };
        //                    driverJournalListToReturn.Add(reportDriverJournalToAdd);
        //                }
        //            }

        //            if (driverJournalListToReturn.Count == 0)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.NoContent);
        //            }
        //            else
        //            {
        //                latestDriverJournal = driverJournalListToReturn.OrderByDescending(x => x.Date).FirstOrDefault();
        //                beforeLatestDriverJournal = driverJournalListToReturn.OrderByDescending(x => x.Date).ElementAt(1);
        //            }

        //            totalMilage = latestDriverJournal.Milage - beforeLatestDriverJournal.Milage;
        //            totalFuelAmount = latestDriverJournal.FuelAmount;
        //            averageFuelComsumption += totalFuelAmount / totalMilage;
        //            totalFuelCost = latestDriverJournal.TotalPrice;

        //            CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
        //            {
        //                AverageFuelValue = averageFuelComsumption,
        //                TotalFuelValue = totalFuelCost
        //            };

        //            // Returnerar OK om det går igenom.
        //            return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
        //    }
        //}
        #endregion CalulateFuelConsumption

        #region CalulateMonthlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateMonthlyFuelConsumptionByVehicleTypeID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage CalulateMonthlyFuelConsumptionByVehicleTypeID([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        int vehicleTypeID = values.VehicleTypeID;
                        int year = values.Year;
                        int month = values.Month;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList =
                            db.Vehicle.Where(x => x.VehicleTypeID == vehicleTypeID).OrderBy(x => x.ID).ToList();

                        if (vehicleList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year && x.Date.Month == month)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousMonth =
                            db.ReportDriverJournal.Where(x => x.Date.Month == month - 1)
                                .ToList();

                        previousMonth.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                        }

                        if (totalDriverJournalMilage == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        totalMilage = totalDriverJournalMilage;
                        averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateMonthlyFuelConsumption

        #region CalulateYearlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateYearlyFuelConsumptionByVehicleTypeID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage CalulateYearlyFuelConsumptionByVehicleTypeID([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        int vehicleTypeID = values.VehicleTypeID;
                        int year = values.Year;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList =
                            db.Vehicle.Where(x => x.VehicleTypeID == vehicleTypeID).OrderBy(x => x.ID).ToList();

                        if (vehicleList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousYear =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year - 1)
                                .ToList();

                        previousYear.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                                    if (previousYear.Count != 0)
                                    {
                                        previousYear.OrderByDescending(x => x.Date);

                                        int lastYearMilage = previousYear.Where(
                                            x => x.VehicleID == vehicle.ID && x.VehicleID == driverJournal.VehicleID)
                                            .Select(x => x.Milage)
                                            .FirstOrDefault();

                                        if (lastYearMilage != 0)
                                        {
                                            totalDriverJournalMilage += driverJournal.Milage - lastYearMilage;
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
                        }

                        if (totalDriverJournalMilage == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        totalMilage = totalDriverJournalMilage;
                        averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumption

        #region CalulateFuelConsumptionSinceTheBeginning
        [HttpPost]
        [ActionName("CalulateFuelConsumptionSinceTheBeginningByVehicleTypeID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage CalulateFuelConsumptionSinceTheBeginningByVehicleTypeID([FromBody]int id)
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        int vehicleTypeID = id;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList =
                            db.Vehicle.Where(x => x.VehicleTypeID == vehicleTypeID).OrderBy(x => x.ID).ToList();

                        if (vehicleList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        // Gets all DriverJournal's since the beginning.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                                    int vehicleOriginalMilage =
                                        db.Vehicle.Where(x => x.ID == driverJournal.VehicleID)
                                            .Select(x => x.OriginalMileage)
                                            .SingleOrDefault();

                                    totalDriverJournalMilage += driverJournal.Milage - vehicleOriginalMilage;
                                    totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                    totalFuelCostToReturn += driverJournal.TotalPrice;
                                }

                                // Sets the previous variable's to the last looped DriverJournal.
                                previousVehicle = driverJournal.VehicleID;
                                previousMilage = driverJournal.Milage;
                            }
                        }

                        if (totalDriverJournalMilage == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        totalMilage = totalDriverJournalMilage;
                        averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumptionSinceTheBeginning

        #region CalulateTotalVehicleCost
        [HttpPost]
        [ActionName("CalulateTotalVehicleCostByVehicleTypeID")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage CalulateTotalVehicleCostByVehicleTypeID([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        decimal totalMontlyCost = 0;
                        decimal totalYearlyCost = 0;

                        int vehicleTypeID = values.VehicleTypeID;
                        int year = values.Year;
                        int month = values.Month;

                        List<Vehicle> vehicleList = db.Vehicle.Where(x => x.VehicleTypeID == vehicleTypeID).ToList();

                        if (vehicleList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        List<ReportDriverJournal> driverJournalListYearlyToReturn = new List<ReportDriverJournal>();
                        List<ReportDriverJournal> driverJournalListMonthlyToReturn = new List<ReportDriverJournal>();
                        List<RefuelingDriverJournal> otherCostListYearlyToReturn = new List<RefuelingDriverJournal>();
                        List<RefuelingDriverJournal> otherCostListMonthlyToReturn = new List<RefuelingDriverJournal>();

                        foreach (var vehicle in vehicleList)
                        {
                            List<ReportDriverJournal> driverJournalListYearly =
                                db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year)
                                    .ToList();
                            List<ReportDriverJournal> driverJournalListMonthly =
                                db.ReportDriverJournal.Where(
                                    x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month)
                                    .ToList();

                            driverJournalListYearly.RemoveAll(x => x.FuelTypeID == 1);
                            driverJournalListMonthly.RemoveAll(x => x.FuelTypeID == 1);

                            List<RefuelingDriverJournal> otherCostListYearly =
                                db.RefuelingDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year)
                                    .ToList();
                            List<RefuelingDriverJournal> otherCostListMonthly =
                                db.RefuelingDriverJournal.Where(
                                    x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month)
                                    .ToList();

                            if (driverJournalListYearly.Count != 0 || driverJournalListMonthly.Count != 0)
                            {
                                foreach (var yearlyDriverJournal in driverJournalListYearly)
                                {
                                    ReportDriverJournal reportDriverJournalToAdd = new ReportDriverJournal()
                                    {
                                        ID = yearlyDriverJournal.ID,
                                        Date = yearlyDriverJournal.Date,
                                        FuelAmount = yearlyDriverJournal.FuelAmount,
                                        FuelType = yearlyDriverJournal.FuelType,
                                        Milage = yearlyDriverJournal.Milage,
                                        TotalPrice = yearlyDriverJournal.TotalPrice,
                                        PricePerUnit = yearlyDriverJournal.PricePerUnit,
                                        ChauffeurID = yearlyDriverJournal.ChauffeurID,
                                        FuelTypeID = yearlyDriverJournal.FuelTypeID,
                                        VehicleID = yearlyDriverJournal.VehicleID
                                    };
                                    driverJournalListYearlyToReturn.Add(reportDriverJournalToAdd);
                                }

                                foreach (var monthlyDriverJournal in driverJournalListMonthly)
                                {
                                    ReportDriverJournal reportDriverJournalToAdd = new ReportDriverJournal()
                                    {
                                        ID = monthlyDriverJournal.ID,
                                        Date = monthlyDriverJournal.Date,
                                        FuelAmount = monthlyDriverJournal.FuelAmount,
                                        FuelType = monthlyDriverJournal.FuelType,
                                        Milage = monthlyDriverJournal.Milage,
                                        TotalPrice = monthlyDriverJournal.TotalPrice,
                                        PricePerUnit = monthlyDriverJournal.PricePerUnit,
                                        ChauffeurID = monthlyDriverJournal.ChauffeurID,
                                        FuelTypeID = monthlyDriverJournal.FuelTypeID,
                                        VehicleID = monthlyDriverJournal.VehicleID
                                    };
                                    driverJournalListMonthlyToReturn.Add(reportDriverJournalToAdd);
                                }
                            }
                            if (otherCostListYearly.Count != 0 || otherCostListMonthly.Count != 0)
                            {
                                foreach (var yearlyOtherCost in otherCostListYearly)
                                {
                                    RefuelingDriverJournal otherCostToAdd = new RefuelingDriverJournal()
                                    {
                                        ID = yearlyOtherCost.ID,
                                        Date = yearlyOtherCost.Date,
                                        Comment = yearlyOtherCost.Comment,
                                        Cost = yearlyOtherCost.Cost,
                                        TypeOfCost = yearlyOtherCost.TypeOfCost,
                                        TypeOfCostID = yearlyOtherCost.TypeOfCostID,
                                        VehicleID = yearlyOtherCost.VehicleID
                                    };
                                    otherCostListYearlyToReturn.Add(otherCostToAdd);
                                }

                                foreach (var monthlyOtherCost in otherCostListMonthly)
                                {
                                    RefuelingDriverJournal otherCostToAdd = new RefuelingDriverJournal()
                                    {
                                        ID = monthlyOtherCost.ID,
                                        Date = monthlyOtherCost.Date,
                                        Comment = monthlyOtherCost.Comment,
                                        Cost = monthlyOtherCost.Cost,
                                        TypeOfCost = monthlyOtherCost.TypeOfCost,
                                        TypeOfCostID = monthlyOtherCost.TypeOfCostID,
                                        VehicleID = monthlyOtherCost.VehicleID
                                    };
                                    otherCostListMonthlyToReturn.Add(otherCostToAdd);
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NoContent);
                            }
                        }

                        if (driverJournalListYearlyToReturn.Count != 0 || driverJournalListMonthlyToReturn.Count != 0)
                        {
                            // Loops througt all driver journals.
                            foreach (var driverJournalYearly in driverJournalListYearlyToReturn)
                            {
                                totalYearlyCost += driverJournalYearly.TotalPrice;
                            }

                            foreach (var driverJournalMonthly in driverJournalListMonthlyToReturn)
                            {
                                totalMontlyCost += driverJournalMonthly.TotalPrice;
                            }
                        }
                        if (otherCostListYearlyToReturn.Count != 0 || otherCostListMonthlyToReturn.Count != 0)
                        {
                            foreach (var otherCostYearly in otherCostListYearlyToReturn)
                            {
                                totalYearlyCost += otherCostYearly.Cost;
                            }

                            foreach (var otherCostMonthly in otherCostListMonthlyToReturn)
                            {
                                totalMontlyCost += otherCostMonthly.Cost;
                            }
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            TotalMonthlyVehicleCost = totalMontlyCost,
                            TotalYearlyVehicleCost = totalYearlyCost
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateTotalVehicleCost
        #endregion

        #region AllVehicles
        #region CalulateFuelConsumption
        //[HttpPost]
        //[ActionName("CalulateFuelConsumptionByVehicleTypeID")]
        //[ResponseType(typeof(CalculateValuesModel))]
        //public HttpResponseMessage CalulateFuelConsumptionByVehicleTypeID([FromBody]int id)
        //{
        //    try
        //    {
        //        using (var db = new WpfProjectDatabaseEntities())
        //        {
        //            decimal totalMilage = 0;
        //            decimal totalFuelAmount = 0;
        //            decimal averageFuelComsumption = 0;
        //            decimal totalFuelCost = 0;

        //            int idToCheck = id;

        //            List<Vehicle> vehicleList = db.Vehicle.Where(x => x.VehicleTypeID == idToCheck).ToList();

        //            List<ReportDriverJournal> driverJournalListToReturn = new List<ReportDriverJournal>();
        //            ReportDriverJournal latestDriverJournal = null;
        //            ReportDriverJournal beforeLatestDriverJournal = null;

        //            foreach (var vehicle in vehicleList)
        //            {
        //                List<ReportDriverJournal> reportDriverJournalListToLoop = db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID).ToList();

        //                foreach (var reportDriverJournal in reportDriverJournalListToLoop)
        //                {
        //                    ReportDriverJournal reportDriverJournalToAdd = new ReportDriverJournal()
        //                    {
        //                        ID = reportDriverJournal.ID,
        //                        Date = reportDriverJournal.Date,
        //                        FuelAmount = reportDriverJournal.FuelAmount,
        //                        FuelType = reportDriverJournal.FuelType,
        //                        Milage = reportDriverJournal.Milage,
        //                        TotalPrice = reportDriverJournal.TotalPrice,
        //                        PricePerUnit = reportDriverJournal.PricePerUnit,
        //                        ChauffeurID = reportDriverJournal.ChauffeurID,
        //                        FuelTypeID = reportDriverJournal.FuelTypeID,
        //                        VehicleID = reportDriverJournal.VehicleID
        //                    };
        //                    driverJournalListToReturn.Add(reportDriverJournalToAdd);
        //                }
        //            }

        //            if (driverJournalListToReturn.Count == 0)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.NoContent);
        //            }
        //            else
        //            {
        //                latestDriverJournal = driverJournalListToReturn.OrderByDescending(x => x.Date).FirstOrDefault();
        //                beforeLatestDriverJournal = driverJournalListToReturn.OrderByDescending(x => x.Date).ElementAt(1);
        //            }

        //            totalMilage = latestDriverJournal.Milage - beforeLatestDriverJournal.Milage;
        //            totalFuelAmount = latestDriverJournal.FuelAmount;
        //            averageFuelComsumption += totalFuelAmount / totalMilage;
        //            totalFuelCost = latestDriverJournal.TotalPrice;

        //            CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
        //            {
        //                AverageFuelValue = averageFuelComsumption,
        //                TotalFuelValue = totalFuelCost
        //            };

        //            // Returnerar OK om det går igenom.
        //            return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
        //    }
        //}
        #endregion CalulateFuelConsumption

        #region CalulateMonthlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateMonthlyFuelConsumptionByAllVehicles")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateMonthlyFuelConsumptionByAllVehicles([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        int year = values.Year;
                        int month = values.Month;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList = db.Vehicle.OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year && x.Date.Month == month)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousMonth =
                            db.ReportDriverJournal.Where(x => x.Date.Month == month - 1)
                                .ToList();

                        previousMonth.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                        }
                        if (totalDriverJournalMilage != 0)
                        {
                            totalMilage = totalDriverJournalMilage;
                            averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateMonthlyFuelConsumption

        #region CalulateYearlyFuelConsumption
        [HttpPost]
        [ActionName("CalulateYearlyFuelConsumptionByAllVehicles")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateYearlyFuelConsumptionByAllVehicles([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Parameters sets to variables.
                        int year = values.Year;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList = db.Vehicle.OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year)
                                .OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        // Gets all DriverJournal's from previous year.
                        List<ReportDriverJournal> previousYear =
                            db.ReportDriverJournal.Where(x => x.Date.Year == year - 1)
                                .ToList();

                        previousYear.RemoveAll(x => x.FuelTypeID == 1);

                        if (vehicleList.FirstOrDefault().FuelTypeID == 1)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        }

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                                    if (previousYear.Count != 0)
                                    {
                                        previousYear.OrderByDescending(x => x.Date);

                                        int lastYearMilage = previousYear.Where(
                                            x => x.VehicleID == vehicle.ID && x.VehicleID == driverJournal.VehicleID)
                                            .Select(x => x.Milage)
                                            .FirstOrDefault();

                                        if (lastYearMilage != 0)
                                        {
                                            totalDriverJournalMilage += driverJournal.Milage - lastYearMilage;
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
                        }

                        totalMilage = totalDriverJournalMilage;
                        averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumption

        #region CalulateFuelConsumptionSinceTheBeginning
        [HttpGet]
        [ActionName("CalulateFuelConsumptionSinceTheBeginningByAllVehicles")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateFuelConsumptionSinceTheBeginningByAllVehicles([FromBody]int id)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Variables used to calculate.
                        decimal totalDriverJournalMilage = 0;
                        decimal totalDriverJournalFuelAmount = 0;
                        decimal totalMilage = 0;

                        // Variables to return.
                        decimal averageFuelConsumptionToReturn = 0;
                        decimal totalFuelCostToReturn = 0;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList = db.Vehicle.OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's since the beginning.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Date.Month)
                                .ThenBy(x => x.Milage)
                                .ToList();

                        if (driverJournalList.Count == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NoContent);
                        }

                        driverJournalList.RemoveAll(x => x.FuelTypeID == 1);

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;

                        // Loop's throught all DriverJournal's (current month).
                        foreach (var vehicle in vehicleList)
                        {
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
                                    int vehicleOriginalMilage =
                                        db.Vehicle.Where(x => x.ID == driverJournal.VehicleID)
                                            .Select(x => x.OriginalMileage)
                                            .SingleOrDefault();

                                    totalDriverJournalMilage += driverJournal.Milage - vehicleOriginalMilage;
                                    totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                    totalFuelCostToReturn += driverJournal.TotalPrice;
                                }

                                // Sets the previous variable's to the last looped DriverJournal.
                                previousVehicle = driverJournal.VehicleID;
                                previousMilage = driverJournal.Milage;
                            }
                        }

                        totalMilage = totalDriverJournalMilage;
                        averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            AverageFuelValue = averageFuelConsumptionToReturn,
                            TotalFuelValue = totalFuelCostToReturn
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateFuelConsumptionSinceTheBeginning

        #region CalulateTotalVehicleCost
        [HttpPost]
        [ActionName("CalulateTotalVehicleCostByAllVehicles")]
        [ResponseType(typeof(CalculateValuesModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage CalulateTotalVehicleCostByAllVehicles([FromBody]CalculateValuesModel values)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        decimal totalMontlyCost = 0;
                        decimal totalYearlyCost = 0;

                        int year = values.Year;
                        int month = values.Month;

                        List<Vehicle> vehicleList = db.Vehicle.ToList();
                        List<ReportDriverJournal> driverJournalListYearlyToReturn = new List<ReportDriverJournal>();
                        List<ReportDriverJournal> driverJournalListMonthlyToReturn = new List<ReportDriverJournal>();

                        List<RefuelingDriverJournal> otherCostListYearlyToReturn = new List<RefuelingDriverJournal>();
                        List<RefuelingDriverJournal> otherCostListMonthlyToReturn = new List<RefuelingDriverJournal>();

                        foreach (var vehicle in vehicleList)
                        {
                            List<ReportDriverJournal> driverJournalListYearly =
                                db.ReportDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year)
                                    .ToList();
                            List<ReportDriverJournal> driverJournalListMonthly =
                                db.ReportDriverJournal.Where(
                                    x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month)
                                    .ToList();

                            driverJournalListYearly.RemoveAll(x => x.FuelTypeID == 1);
                            driverJournalListMonthly.RemoveAll(x => x.FuelTypeID == 1);

                            List<RefuelingDriverJournal> otherCostListYearly =
                                db.RefuelingDriverJournal.Where(x => x.VehicleID == vehicle.ID && x.Date.Year == year)
                                    .ToList();
                            List<RefuelingDriverJournal> otherCostListMonthly =
                                db.RefuelingDriverJournal.Where(
                                    x => x.VehicleID == vehicle.ID && x.Date.Year == year && x.Date.Month == month)
                                    .ToList();

                            if (driverJournalListYearly.Count != 0 || driverJournalListMonthly.Count != 0)
                            {
                                foreach (var yearlyDriverJournal in driverJournalListYearly)
                                {
                                    ReportDriverJournal reportDriverJournalToAdd = new ReportDriverJournal()
                                    {
                                        ID = yearlyDriverJournal.ID,
                                        Date = yearlyDriverJournal.Date,
                                        FuelAmount = yearlyDriverJournal.FuelAmount,
                                        FuelType = yearlyDriverJournal.FuelType,
                                        Milage = yearlyDriverJournal.Milage,
                                        TotalPrice = yearlyDriverJournal.TotalPrice,
                                        PricePerUnit = yearlyDriverJournal.PricePerUnit,
                                        ChauffeurID = yearlyDriverJournal.ChauffeurID,
                                        FuelTypeID = yearlyDriverJournal.FuelTypeID,
                                        VehicleID = yearlyDriverJournal.VehicleID
                                    };
                                    driverJournalListYearlyToReturn.Add(reportDriverJournalToAdd);
                                }

                                foreach (var monthlyDriverJournal in driverJournalListMonthly)
                                {
                                    ReportDriverJournal reportDriverJournalToAdd = new ReportDriverJournal()
                                    {
                                        ID = monthlyDriverJournal.ID,
                                        Date = monthlyDriverJournal.Date,
                                        FuelAmount = monthlyDriverJournal.FuelAmount,
                                        FuelType = monthlyDriverJournal.FuelType,
                                        Milage = monthlyDriverJournal.Milage,
                                        TotalPrice = monthlyDriverJournal.TotalPrice,
                                        PricePerUnit = monthlyDriverJournal.PricePerUnit,
                                        ChauffeurID = monthlyDriverJournal.ChauffeurID,
                                        FuelTypeID = monthlyDriverJournal.FuelTypeID,
                                        VehicleID = monthlyDriverJournal.VehicleID
                                    };
                                    driverJournalListMonthlyToReturn.Add(reportDriverJournalToAdd);
                                }
                            }
                            if (otherCostListYearly.Count != 0 || otherCostListMonthly.Count != 0)
                            {
                                foreach (var yearlyOtherCost in otherCostListYearly)
                                {
                                    RefuelingDriverJournal otherCostToAdd = new RefuelingDriverJournal()
                                    {
                                        ID = yearlyOtherCost.ID,
                                        Date = yearlyOtherCost.Date,
                                        Comment = yearlyOtherCost.Comment,
                                        Cost = yearlyOtherCost.Cost,
                                        TypeOfCost = yearlyOtherCost.TypeOfCost,
                                        TypeOfCostID = yearlyOtherCost.TypeOfCostID,
                                        VehicleID = yearlyOtherCost.VehicleID
                                    };
                                    otherCostListYearlyToReturn.Add(otherCostToAdd);
                                }

                                foreach (var monthlyOtherCost in otherCostListMonthly)
                                {
                                    RefuelingDriverJournal otherCostToAdd = new RefuelingDriverJournal()
                                    {
                                        ID = monthlyOtherCost.ID,
                                        Date = monthlyOtherCost.Date,
                                        Comment = monthlyOtherCost.Comment,
                                        Cost = monthlyOtherCost.Cost,
                                        TypeOfCost = monthlyOtherCost.TypeOfCost,
                                        TypeOfCostID = monthlyOtherCost.TypeOfCostID,
                                        VehicleID = monthlyOtherCost.VehicleID
                                    };
                                    otherCostListMonthlyToReturn.Add(otherCostToAdd);
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NoContent);
                            }
                        }

                        if (driverJournalListYearlyToReturn.Count != 0 || driverJournalListMonthlyToReturn.Count != 0)
                        {
                            // Loops througt all driver journals.
                            foreach (var driverJournalYearly in driverJournalListYearlyToReturn)
                            {
                                totalYearlyCost += driverJournalYearly.TotalPrice;
                            }

                            foreach (var driverJournalMonthly in driverJournalListMonthlyToReturn)
                            {
                                totalMontlyCost += driverJournalMonthly.TotalPrice;
                            }
                        }
                        if (otherCostListYearlyToReturn.Count != 0 || otherCostListMonthlyToReturn.Count != 0)
                        {
                            // Loops througt all driver journals.
                            foreach (var otherCostYearly in otherCostListYearlyToReturn)
                            {
                                totalYearlyCost += otherCostYearly.Cost;
                            }

                            foreach (var otherCostMonthly in otherCostListMonthlyToReturn)
                            {
                                totalMontlyCost += otherCostMonthly.Cost;
                            }
                        }

                        CalculateValuesModel calculateValuesModel = new CalculateValuesModel()
                        {
                            TotalMonthlyVehicleCost = totalMontlyCost,
                            TotalYearlyVehicleCost = totalYearlyCost
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, calculateValuesModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion CalulateTotalVehicleCost
        #endregion

        #region BestChauffeur
        [HttpGet]
        [ActionName("CalulateBestChauffeurFuelConsumption")]
        [ResponseType(typeof(BestValueModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage CalulateBestChauffeurFuelConsumption()
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Parameters sets to variables.
                        int month = DateTime.Today.Month;

                        // Gets all Vehicles.
                        List<User> chauffeurList = db.User.OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Month == month).OrderBy(x => x.VehicleID)
                                .ThenBy(x => x.Milage)
                                .ThenBy(x => x.Date.Month)
                                .ToList();

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;
                        List<BestValueModel> bestValueList = new List<BestValueModel>();

                        // Loop's throught all DriverJournal's.
                        foreach (var chauffeur in chauffeurList)
                        {
                            // Variables used to calculate.
                            decimal totalDriverJournalMilage = 0;
                            decimal totalDriverJournalFuelAmount = 0;
                            decimal totalMilage = 0;

                            // Variables to return.
                            decimal averageFuelConsumptionToReturn = 0;
                            decimal totalFuelCostToReturn = 0;

                            foreach (var driverJournal in driverJournalList)
                            {
                                if (driverJournal.ChauffeurID == chauffeur.ID &&
                                    previousVehicle == driverJournal.VehicleID)
                                {
                                    totalDriverJournalMilage += driverJournal.Milage - previousMilage;
                                    totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                    totalFuelCostToReturn += driverJournal.TotalPrice;
                                }
                                else if (driverJournal.ChauffeurID == chauffeur.ID)
                                {
                                    int vehicleOriginalMilage =
                                        db.Vehicle.Where(x => x.ID == driverJournal.VehicleID)
                                            .Select(x => x.OriginalMileage)
                                            .SingleOrDefault();

                                    totalDriverJournalMilage += driverJournal.Milage - vehicleOriginalMilage;
                                    totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                    totalFuelCostToReturn += driverJournal.TotalPrice;
                                }

                                // Sets the previous variable's to the last looped DriverJournal.
                                previousVehicle = driverJournal.VehicleID;
                                previousMilage = driverJournal.Milage;
                            }

                            if (totalDriverJournalMilage == 0)
                            {
                                return Request.CreateResponse(HttpStatusCode.NoContent);
                            }
                            // Does the calculation.
                            totalMilage = totalDriverJournalMilage;
                            averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                            // Adds the ChauffeurID and the calculated value to a Dictionary.
                            bestValueList.Add(new BestValueModel
                            {
                                Name = chauffeur.Username,
                                FuelConsumption = averageFuelConsumptionToReturn
                            });
                        }
                        BestValueModel bestChauffeur = bestValueList.OrderBy(x => x.FuelConsumption).FirstOrDefault();
                        BestValueModel chauffeurWithLowestFuelConsumption = new BestValueModel()
                        {
                            Name = bestChauffeur.Name,
                            FuelConsumption = bestChauffeur.FuelConsumption
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, chauffeurWithLowestFuelConsumption);
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

        #region BestVehicle
        [HttpGet]
        [ActionName("CalulateBestVehicleFuelConsumption")]
        [ResponseType(typeof(BestValueModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage CalulateBestVehicleFuelConsumption()
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Parameters sets to variables.
                        int month = DateTime.Today.Month;

                        // Gets all Vehicles.
                        List<Vehicle> vehicleList = db.Vehicle.OrderBy(x => x.ID).ToList();

                        // Gets all DriverJournal's year.
                        List<ReportDriverJournal> driverJournalList =
                            db.ReportDriverJournal.Where(x => x.Date.Month == month).OrderBy(x => x.ChauffeurID)
                                .ThenBy(x => x.VehicleID)
                                .ThenBy(x => x.Milage)
                                .ThenBy(x => x.Date.Month)
                                .ToList();

                        int previousMilage = 0;
                        Guid previousVehicle = Guid.Empty;
                        List<BestValueModel> bestValueList = new List<BestValueModel>();

                        // Loop's throught all DriverJournal's.
                        foreach (var vehicle in vehicleList)
                        {
                            // Variables used to calculate.
                            decimal totalDriverJournalMilage = 0;
                            decimal totalDriverJournalFuelAmount = 0;
                            decimal totalMilage = 0;

                            // Variables to return.
                            decimal averageFuelConsumptionToReturn = 0;
                            decimal totalFuelCostToReturn = 0;

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
                                    int vehicleOriginalMilage =
                                        db.Vehicle.Where(x => x.ID == driverJournal.VehicleID)
                                            .Select(x => x.OriginalMileage)
                                            .SingleOrDefault();

                                    totalDriverJournalMilage += driverJournal.Milage - vehicleOriginalMilage;
                                    totalDriverJournalFuelAmount += driverJournal.FuelAmount;
                                    totalFuelCostToReturn += driverJournal.TotalPrice;
                                }

                                // Sets the previous variable's to the last looped DriverJournal.
                                previousVehicle = driverJournal.VehicleID;
                                previousMilage = driverJournal.Milage;
                            }

                            if (totalDriverJournalMilage == 0)
                            {
                                return Request.CreateResponse(HttpStatusCode.NoContent);
                            }

                            // Does the calculation.
                            totalMilage = totalDriverJournalMilage;
                            averageFuelConsumptionToReturn += totalDriverJournalFuelAmount / totalMilage;

                            // Adds the ChauffeurID and the calculated value to a Dictionary.
                            bestValueList.Add(new BestValueModel
                            {
                                Name = vehicle.RegNo,
                                FuelConsumption = averageFuelConsumptionToReturn
                            });
                        }

                        BestValueModel bestVehicle = bestValueList.OrderBy(x => x.FuelConsumption).FirstOrDefault();
                        BestValueModel vehicleWithLowestFuelConsumption = new BestValueModel()
                        {
                            Name = bestVehicle.Name,
                            FuelConsumption = bestVehicle.FuelConsumption
                        };

                        // Returnerar OK om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, vehicleWithLowestFuelConsumption);
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