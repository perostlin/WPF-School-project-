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
    public class VehicleController : ApiController
    {
        #region AddVehicle
        [HttpPost]
        [ActionName("AddVehicle")]
        [ResponseType(typeof(VehicleModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage AddVehicle([FromBody]VehicleModel vehicle)
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        Vehicle vehicleToAdd = new Vehicle
                        {
                            ID = Guid.NewGuid(),
                            RegNo = vehicle.RegNo,
                            Desription = vehicle.Description,
                            OriginalMileage = vehicle.OriginalMilage,
                            ColorID = vehicle.ColorID,
                            FuelTypeID = vehicle.FuelTypeID,
                            ModelYearID = vehicle.ModelYearID,
                            VehicleTypeID = vehicle.VehicleTypeID
                        };

                        // Lägger till och sparar ny användare.
                        db.Vehicle.Add(vehicleToAdd);
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
        #endregion AddVehicle

        #region GetAllVehicles
        [HttpGet]
        [ActionName("GetAllVehicles")]
        [ResponseType(typeof(List<VehicleModel>))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage GetAllVehicles()
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Skapar och fyller en lista med Users.
                        List<Vehicle> vehicleList = db.Vehicle.ToList();

                        // Skapar en lista som skall fyllas och returneras.
                        List<VehicleModel> vehicleListToReturn = new List<VehicleModel>();

                        // Loopar igenom varje användare och lägger till dessa i listan ovan.
                        foreach (var vehicle in vehicleList)
                        {
                            VehicleModel vehicleToAdd = new VehicleModel
                            {
                                ID = vehicle.ID,
                                RegNo = vehicle.RegNo,
                                Description = vehicle.Desription,
                                OriginalMilage = vehicle.OriginalMileage,
                                VehicleType = vehicle.VehicleType.Type,
                                VehicleTypeID = vehicle.VehicleTypeID,
                                FuelType = vehicle.FuelType.Type,
                                FuelTypeID = vehicle.FuelTypeID,
                                ModelYear = Convert.ToInt32(vehicle.ModelYear.Year),
                                ModelYearID = vehicle.ModelYearID,
                                Color = vehicle.Color.Color1,
                                ColorID = vehicle.ColorID
                            };

                            vehicleListToReturn.Add(vehicleToAdd);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, vehicleListToReturn);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion GetAllVehicles

        #region GetSelectedVehicle
        [HttpPost]
        [ActionName("GetSelectedVehicle")]
        [ResponseType(typeof(VehicleModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage GetSelectedVehicle([FromBody]VehicleModel vehicle)
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
                            VehicleModel vehicleToReturn = new VehicleModel
                            {
                                ID = selectedVehicle.ID,
                                RegNo = selectedVehicle.RegNo,
                                Description = selectedVehicle.Desription,
                                OriginalMilage = selectedVehicle.OriginalMileage,
                                VehicleTypeID = selectedVehicle.VehicleTypeID,
                                VehicleType = selectedVehicle.VehicleType.Type,
                                FuelTypeID = selectedVehicle.FuelTypeID,
                                FuelType = selectedVehicle.FuelType.Type,
                                ModelYearID = selectedVehicle.ModelYearID,
                                ModelYear = selectedVehicle.ModelYear.Year,
                                ColorID = selectedVehicle.ColorID,
                                Color = selectedVehicle.Color.Color1
                            };

                            return Request.CreateResponse(HttpStatusCode.OK, vehicleToReturn);
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion GetSelectedUser

        #region FillAllComboBoxes
        [HttpGet]
        [ActionName("FillAllComboBoxes")]
        [ResponseType(typeof(ComboboxModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage FillAllComboBoxes()
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Skapar Combobox-klassen.
                        ComboboxModel comboBoxModel = new ComboboxModel();

                        // Hämtar alla värden och lägger till dom i listor.

                        // Färger
                        List<Color> colorList = db.Color.ToList();
                        comboBoxModel.ColorList = new List<ColorModel>();
                        foreach (var color in colorList)
                        {
                            ColorModel colorToAdd = new ColorModel
                            {
                                ID = color.ID,
                                ColorName = color.Color1
                            };
                            comboBoxModel.ColorList.Add(colorToAdd);
                        }

                        // Bränsletyper
                        List<FuelType> fuelTypeList = db.FuelType.ToList();
                        comboBoxModel.FuelTypeList = new List<FuelTypeModel>();
                        foreach (var fuelType in fuelTypeList)
                        {
                            FuelTypeModel fuelTypeToAdd = new FuelTypeModel
                            {
                                ID = fuelType.ID,
                                Type = fuelType.Type
                            };
                            comboBoxModel.FuelTypeList.Add(fuelTypeToAdd);
                        }

                        // Årsmodeller
                        List<ModelYear> modelYearList = db.ModelYear.ToList();
                        comboBoxModel.ModelYearList = new List<ModelYearModel>();
                        foreach (var modelYear in modelYearList)
                        {
                            ModelYearModel modelYearToAdd = new ModelYearModel
                            {
                                ID = modelYear.ID,
                                Year = modelYear.Year
                            };
                            comboBoxModel.ModelYearList.Add(modelYearToAdd);
                        }

                        // Fordon's typer
                        List<VehicleType> vehicleTypeList = db.VehicleType.ToList();
                        comboBoxModel.VehicleTypeList = new List<VehicleTypeModel>();
                        foreach (var vehicleType in vehicleTypeList)
                        {
                            VehicleTypeModel vehicleTypeToAdd = new VehicleTypeModel
                            {
                                ID = vehicleType.ID,
                                Type = vehicleType.Type
                            };
                            comboBoxModel.VehicleTypeList.Add(vehicleTypeToAdd);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, comboBoxModel);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion GetAllVehicles

        #region GetAllVehicleTypes
        [HttpGet]
        [ActionName("GetAllVehicleTypes")]
        [ResponseType(typeof(List<VehicleTypeModel>))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage GetAllVehicleTypes()
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Skapar och fyller en lista med Users.
                        List<VehicleType> vehicleList = db.VehicleType.ToList();

                        // Skapar en lista som skall fyllas och returneras.
                        List<VehicleTypeModel> vehicleTypeListToReturn = new List<VehicleTypeModel>();

                        // Loopar igenom varje användare och lägger till dessa i listan ovan.
                        foreach (var vehicleType in vehicleList)
                        {
                            VehicleTypeModel vehicleToAdd = new VehicleTypeModel
                            {
                                ID = vehicleType.ID,
                                Type = vehicleType.Type
                            };

                            vehicleTypeListToReturn.Add(vehicleToAdd);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, vehicleTypeListToReturn);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion GetAllVehicleTypes

        #region GetAllVehiclesByChauffeurID
        [HttpPost]
        [ActionName("GetAllVehiclesByChauffeurID")]
        [ResponseType(typeof(List<VehicleModel>))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage GetAllVehiclesByChauffeurID([FromBody]Guid chauffeurID)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Skapar och fyller en listor med information.
                        List<ReportDriverJournal> reportDriverJournalList =
                            db.ReportDriverJournal.Where(x => x.ChauffeurID == chauffeurID).ToList();

                        List<Vehicle> vehicleList = db.Vehicle.ToList();

                        // Skapar en lista som skall fyllas och returneras.
                        List<VehicleModel> vehicleListToReturn = new List<VehicleModel>();

                        foreach (var reportDriverJournal in reportDriverJournalList)
                        {
                            foreach (var vehicle in vehicleList)
                            {
                                if (vehicleListToReturn.Any(x => x.ID == vehicle.ID))
                                {
                                    // Do nothing...
                                }
                                else if (vehicle.ID == reportDriverJournal.VehicleID)
                                {
                                    VehicleModel vehicleToAdd = new VehicleModel
                                    {
                                        ID = vehicle.ID,
                                        RegNo = vehicle.RegNo,
                                        Description = vehicle.Desription,
                                        OriginalMilage = vehicle.OriginalMileage,
                                        VehicleType = vehicle.VehicleType.Type,
                                        VehicleTypeID = vehicle.VehicleTypeID,
                                        FuelType = vehicle.FuelType.Type,
                                        FuelTypeID = vehicle.FuelTypeID,
                                        ModelYear = Convert.ToInt32(vehicle.ModelYear.Year),
                                        ModelYearID = vehicle.ModelYearID,
                                        Color = vehicle.Color.Color1,
                                        ColorID = vehicle.ColorID
                                    };

                                    vehicleListToReturn.Add(vehicleToAdd);
                                }
                            }
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, vehicleListToReturn);
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