using BusinessLogicsLayer;
using BusinessLogicsLayer.MapUnitChange;
using BusinessLogicsLayer.Master;
using DataAccessLayer;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Web.Validation;
using Web.WebHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    [Authorize]
    public class MasterController : Controller
    {   
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserProfileBL userProfileBL;
        private readonly IChangeHierarchyMasterBL changeHierarchyMaster;
        private readonly ILogger<MasterController> _logger;
        private readonly IMasterBL _IMasterBL;
        private readonly IConfiguration _configuration;
        private readonly IMapUnitChangeBL _mapUnitChangeBL;
        private readonly IDataProtector protector;
        private readonly UserManager<ApplicationUser> _userManager;
        public MasterController(IUnitOfWork unitOfWork, IUserProfileBL userProfileBL, IChangeHierarchyMasterBL changeHierarchyMaster, ILogger<MasterController> logger, IMasterBL masterBL, IConfiguration configuration, IMapUnitChangeBL mapUnitChangeBL, DataProtectionPurposeStrings dataProtectionPurposeStrings, IDataProtectionProvider dataProtectionProvider, UserManager<ApplicationUser> userManager)
        {
            this.userProfileBL = userProfileBL;
            this.unitOfWork = unitOfWork;
            this.changeHierarchyMaster = changeHierarchyMaster;
            _logger = logger; 
            _IMasterBL = masterBL;
            _configuration = configuration;
            _mapUnitChangeBL= mapUnitChangeBL;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            _userManager = userManager;
        }

        #region Command Page

        /// <summary>
        /// Displays the Command view for users with the "admin" role.
        /// </summary>
        /// <remarks>
        /// This action method is restricted to users in the "admin" role.
        /// It retrieves the role of the currently authenticated user and returns the corresponding view.
        /// Currently, the retrieved role is stored in a local variable for potential use in the view or future logic.
        /// </remarks>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the Command view.
        /// </returns>
        [Authorize(Roles = "admin")]
        public IActionResult Command()
        {
            // Return the Command view to the client
            return View();
        }

        
        /// <summary>
        /// <remarks>
        /// This action method is restricted to users in the "admin" role. 
        /// It performs the following steps:
        /// 1. Sets default properties for the command, including <c>IsActive</c>, <c>UpdatedBy</c>, and <c>UpdatedOn</c>.
        /// 2. Trims and formats the command name and abbreviation.
        /// 3. Validates the model state before saving.
        /// 4. Checks if a command with the same name already exists.
        /// 5. Updates the record if <c>ComdId &gt; 0</c>, otherwise adds a new record with the next order value.
        /// 6. Returns appropriate JSON responses for save, update, existence check, or validation errors.
        /// 7. Logs any exceptions and returns a generic internal server error message.
        /// </remarks>
        /// <param name="dTO">The <see cref="MComd"/> object containing command details to be saved or updated.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing:
        /// - <c>KeyConstants.Save</c> if a new record is added successfully,
        /// - <c>KeyConstants.Update</c> if an existing record is updated successfully,
        /// - <c>KeyConstants.Exists</c> if the command already exists,
        /// - Model state errors if validation fails,
        /// - <c>KeyConstants.InternalServerError</c> in case of an exception.
        /// </returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> SaveCommand(MComd dTO)
        {
            try
            {
                // Set default active status and audit information
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;

                // Trim and format input strings
                dTO.ComdName = dTO.ComdName.Trim();
                dTO.ComdAbbreviation= dTO.ComdAbbreviation.Trim().ToUpper();

                // Validate model state
                if (ModelState.IsValid)
                {
                    // Check if a command with the same name already exists
                    if (!await unitOfWork.Comds.GetByName(dTO))
                    {
                        if (dTO.ComdId > 0)
                        {
                            // Update existing record
                            await unitOfWork.Comds.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            // Assign order for new record and add to database
                            dTO.Orderby=await unitOfWork.Comds.GetByMaxOrder();
                            await unitOfWork.Comds.Add(dTO);
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        // Command with same name already exists
                        return Json(KeyConstants.Exists);
                    }
                }
                else
                {
                    // Return model validation errors
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) 
            {
                // Log exception and return internal server error response
                _logger.LogError(1001, ex, "Master->SaveCommand");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all commands (MComd) sorted by their order in the database. This method is only accessible by users with the "admin" role.
        /// </summary>
        /// <param name="Id">An array of command IDs, though it is not utilized in this method as the query fetches all commands sorted by order.</param>
        /// <returns>
        /// Returns a JSON result with the list of commands ordered by their position in the database. If an error occurs, an internal server error message is returned.
        /// </returns>
        /// <remarks>
        /// This method is used to fetch and return all commands sorted by their order from the database. It is protected with the "admin" role authorization.
        /// In case of any exceptions, an internal server error is logged, and the client receives an appropriate error response.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllCommand(int[] Id)
        {
            try
            {
                // Fetches all commands ordered by their position in the database using the unitOfWork.Comds.GetAllByorder method
                return Json(await unitOfWork.Comds.GetAllByorder());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllCommand");
                return Json(KeyConstants.InternalServerError);
            }

        }

        /// <summary>
        /// Deletes a command (MComd) from the system after checking if it is referenced in any foreign key relationships.
        /// </summary>
        /// <param name="dTO">The MComd object containing the details of the command to be deleted.</param>
        /// <returns>
        /// Returns a JSON response indicating the result of the deletion operation:
        /// - <see cref="KeyConstants.Success"/> if the command was successfully deleted.
        /// - 5 if the command is referenced in one or more foreign key relationships (e.g., Corps, Bde, Div, MapUnit).
        /// - <see cref="KeyConstants.InternalServerError"/> if an error occurred during the process.
        /// </returns>
        /// <remarks>
        /// This method checks whether the command (MComd) is referenced in any related tables (such as Corps, Bde, Div, MapUnit) using the `ComdIdCheckInFKTable` function.
        /// If any references are found, the deletion is prevented and a specific response is returned.
        /// Otherwise, the command is deleted from the database.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCommand(MComd dTO)
        {
            try
            {
                // Check if the command is referenced in any foreign key relationships
                DTOComdIdCheckInFKTableResponse? dTOComdIdCheckInFKTableResponse = await unitOfWork.Comds.ComdIdCheckInFKTable(dTO.ComdId);
                if(dTOComdIdCheckInFKTableResponse != null && (dTOComdIdCheckInFKTableResponse.TotalCorps >0 || dTOComdIdCheckInFKTableResponse.TotalBde >0 || dTOComdIdCheckInFKTableResponse.TotalDiv >0 || dTOComdIdCheckInFKTableResponse.TotalMapUnit >0))
                {
                    return Json(5);// Command is referenced and cannot be deleted
                }
                else
                {
                    // If the command is not referenced, proceed with deletion
                    await unitOfWork.Comds.Delete(dTO);
                    return Json(KeyConstants.Success);// Return success response
                }

            }
            catch (Exception ex)
            {
                // Log any errors that occur during the operation
                _logger.LogError(1001, ex, "Master->DeleteCommand");
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Handles the request to change the order of a command (MComd) in the system.
        /// </summary>
        /// <param name="dTO">The <see cref="MComd"/> object containing the updated order information for the command.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating the result of the operation:
        /// - <c>KeyConstants.Success</c> if the order change is successful.
        /// - <c>KeyConstants.InternalServerError</c> if an exception occurs during the process.
        /// </returns>
        /// <remarks>
        /// This method updates the order of the specified command (MComd) based on the provided <paramref name="dTO"/> object.
        /// It calls the <see cref="unitOfWork.Comds.OrderByChange"/> method to perform the actual update in the database.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> OrderByChange(MComd dTO)
        {
            try
            {
                // Call the unitOfWork method to update the order of the command
                await unitOfWork.Comds.OrderByChange(dTO);
                
                // Return success response if the order change is successful
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the process
                _logger.LogError(1001, ex, "Master->OrderByChange");

                // Return error response in case of an exception
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Deletes multiple commands based on the provided list of command IDs.
        /// </summary>
        /// <param name="ints">An array of command IDs (<see cref="MComd.ComdId"/>) to delete from the database.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success or failure of the operation.
        /// Returns <see cref="KeyConstants.Success"/> on successful deletion, or <see cref="KeyConstants.InternalServerError"/> if an error occurs.
        /// </returns>
        /// <remarks>
        /// This method loops through the provided command IDs (<paramref name="ints"/>), creates a new <see cref="MComd"/> object for each ID,
        /// and deletes the corresponding record from the database using the <see cref="unitOfWork.Comds.Delete"/> method.
        /// The deletion process is handled one record at a time for each command ID in the array.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCommandMultiple(int[] ints)
        {
            try
            {
                MComd dto = new MComd(); // Create a new MComd object to represent each command to be deleted
                foreach (byte i in ints) // Iterate through the array of command IDs
                {
                    dto.ComdId = i; // Set the command ID for the current iteration
                    await unitOfWork.Comds.Delete(dto);  // Delete the command with the given ID
                }

                return Json(KeyConstants.Success); // Return success if all deletions were successful
            }
            catch (Exception ex) // Catch any exceptions during the deletion process
            {
                _logger.LogError(1001, ex, "Master->DeleteCommandMultiple"); // Log the error if something goes wrong
                return Json(KeyConstants.InternalServerError); // Return internal server error if an exception occurs
            }
        }


        /// <summary>
        /// Retrieves the binary tree structure for a given command ID.
        /// </summary>
        /// <param name="Id">The ID of the command for which the binary tree structure is requested. This ID corresponds to the <see cref="MComd.ComdId"/>.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing the binary tree structure. Returns the binary tree data if successful, or <see cref="KeyConstants.InternalServerError"/> if an error occurs.
        /// </returns>
        /// <remarks>
        /// This method fetches the binary tree representation of a command (and its related nodes) from the database using the provided <paramref name="Id"/>. 
        /// It calls the <see cref="unitOfWork.Comds.GetBinaryTree"/> method to retrieve the data and returns it as a JSON result.
        /// If an exception occurs during the retrieval, the method logs the error and returns an internal server error response.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetBinaryTree(int Id)
        {
            try
            {
                // Retrieve the binary tree for the given command ID
                var ret = Json(await unitOfWork.Comds.GetBinaryTree(Id));
                return ret; // Return the binary tree data as JSON
            }
            catch (Exception ex) // Handle any exceptions that occur during the process
            {
                _logger.LogError(1001, ex, "Master->GetAllCommand"); // Log the error
                return Json(KeyConstants.InternalServerError); // Return an internal server error response
            }
        }


        #endregion Command

        #region Corps 

        /// <summary>
        /// Displays the Corps management page for administrators.
        /// </summary>
        /// <remarks>
        /// This action is used to render the view where the administrator can manage and view the details of various corps. 
        /// The action is restricted to users who have the "admin" role, ensuring that only authorized personnel can access this page.
        /// </remarks>
        /// <returns>
        /// Returns a view representing the Corps management page for the administrator.
        /// </returns>
        [Authorize(Roles = "admin")]
        public IActionResult Corps()
        {
            return View();
        }


        /// <summary>
        /// Saves or updates the Corps information for the admin user. 
        /// This action either adds a new Corps entry or updates an existing one based on the provided data.
        /// </summary>
        /// <param name="dTO">The DTO containing the Corps data to be saved or updated.</param>
        /// <returns>
        /// A JSON result indicating the success or failure of the operation. Returns:
        /// - KeyConstants.Save: When a new Corps is successfully saved.
        /// - KeyConstants.Update: When an existing Corps is successfully updated.
        /// - KeyConstants.Exists: If a Corps with the same name already exists.
        /// - KeyConstants.InternalServerError: If an error occurs during the process.
        /// </returns>
        /// <remarks>
        /// This method checks whether the Corps already exists in the database by calling the GetByName method.
        /// If the Corps exists, it returns an "Exists" response. Otherwise, it either updates the Corps if it already exists (using the Update method)
        /// or adds a new Corps record (using the Add method). Additionally, if the Corps is updated, it updates the corresponding command using the 
        /// UpdateChageComdByCorps method. The method ensures that only an admin user can access it.
        /// </remarks>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> SaveCorps(MCorps dTO)
        {
            // Set the IsActive flag to true and capture the user who is updating the Corps.
            dTO.IsActive = true;
            dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            dTO.UpdatedOn = DateTime.Now;
            dTO.CorpsName = dTO.CorpsName.Trim();

            // Check if the model is valid
            if (ModelState.IsValid)
            {
                // Check if a Corps with the same name already exists
                if (!await unitOfWork.Corps.GetByName(dTO))
                {
                    // If updating an existing Corps
                    if (dTO.CorpsId > 0)
                    {
                        //this Corps update using UpdateChageComdByCorps method
                        //await unitOfWork.Corps.Update(dTO);

                        /////update Commd By CorpsId
                        MapUnit dat = new MapUnit();
                        dat.Corps = dTO;
                        dat.CorpsId = dTO.CorpsId;
                        dat.ComdId = dTO.ComdId;
                        bool result = await changeHierarchyMaster.UpdateChageComdByCorps(dat);
                        
                        // Return success or error message based on the update result
                        if (result)
                        {
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            return Json(KeyConstants.InternalServerError);
                        }
                        ////////End Code //////////////
                    }
                    else
                    {
                        // If adding a new Corps
                        await unitOfWork.Corps.Add(dTO);
                        return Json(KeyConstants.Save);
                    }
                }
                else
                {
                    return Json(KeyConstants.Exists); // Corps with the same name already exists
                }
            }
            else
            {
                // Return validation errors if the model is not valid
                //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => HtmlEncoder.Default.Encode(e.ErrorMessage))
                    .ToList();

                return Json(errors);
            }

        }


        /// <summary>
        /// Retrieves all Corps records from the database.
        /// This action is only accessible by users with the "admin" role and returns a JSON result containing a list of all Corps records.
        /// </summary>
        /// <param name="Id">The identifier used to fetch all Corps records. Although the parameter is not directly used in the method, it can be leveraged for future filtering or validation.</param>
        /// <returns>
        /// A JSON result containing:
        /// - A list of Corps records retrieved from the database.
        /// - KeyConstants.InternalServerError if an error occurs while fetching the data.
        /// </returns>
        /// <remarks>
        /// This method calls the GetALLCorps method from the unitOfWork.Corps service, which interacts with the database to fetch all Corps records.
        /// If an exception occurs during the data retrieval process, the exception is logged, and an internal server error message is returned.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllCorps(int Id)
        {
            try
            {
                // Fetch all Corps data from the database using the GetALLCorps method of unitOfWork.Corps
                return Json(await unitOfWork.Corps.GetALLCorps());
            }
            catch (Exception ex)
            {
                // Log the exception with an error message for debugging purposes
                _logger.LogError(1001, ex, "Master->GetAllCorps");

                // Return an internal server error response
                return Json(KeyConstants.InternalServerError);
            }
        }
        

        /// Deletes a Corps record from the database after checking for foreign key references.
        /// </summary>
        /// <param name="dTO">The <see cref="MCorps"/> object containing the CorpsId to delete.</param>
        /// <returns>
        /// Returns a <see cref="JsonResult"/>:
        /// - 5 if the Corps is referenced in Brigade, Division, or MapUnit tables and cannot be deleted.
        /// - <see cref="KeyConstants.Success"/> if the Corps is deleted successfully.
        /// - <see cref="KeyConstants.InternalServerError"/> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This method first checks if the specified Corps is referenced in any Brigade, Division, or MapUnit records.
        /// If references exist, deletion is prevented and a specific code (5) is returned.
        /// Otherwise, the Corps is deleted and a success code is returned.
        /// Any exceptions are logged and an internal server error code is returned.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCorps(MCorps dTO)
        {
            try
            {
                DTOCorpsIdCheckInFKTableResponse? dTOCorpsIdCheckIn = await unitOfWork.Corps.CorpsIdCheckInFKTable(dTO.CorpsId);// Check for foreign key references
                if (dTOCorpsIdCheckIn != null && (dTOCorpsIdCheckIn.TotalBde > 0 || dTOCorpsIdCheckIn.TotalDiv > 0 || dTOCorpsIdCheckIn.TotalMapUnit > 0))// If references exist, prevent deletion
                {
                    return Json(5);// Return code indicating deletion is not allowed
                }
                else
                {
                    await unitOfWork.Corps.Delete(dTO);// Delete the Corps if no references exist
                    return Json(KeyConstants.Success);// Return success code
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteCorps");// Log any exceptions that occur during the operation
                return Json(KeyConstants.InternalServerError);// Return internal server error code in case of an exception
            }

        }
        
        
        /// Deletes multiple Corps records based on the provided array of Corps IDs.
        /// </summary>
        /// <param name="ints">An array of Corps IDs to delete.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating the result of the operation:
        /// - <see cref="KeyConstants.Success"/> if all deletions succeed.
        /// - <see cref="KeyConstants.InternalServerError"/> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// Iterates through the provided Corps IDs, deletes each corresponding Corps record.
        /// Logs any exceptions and returns an appropriate error response.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCorpsMultiple(int[] ints)
        {
            try
            {
                MCorps dto = new MCorps();
                foreach (byte i in ints)// Iterate through the array of Corps IDs
                {
                    dto.CorpsId = i;
                    await unitOfWork.Corps.Delete(dto);// Delete each Corps by ID
                }

                return Json(KeyConstants.Success);// Return success if all deletions were successful
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteCorpsMultiple");//    Log any exceptions that occur during the operation
                return Json(KeyConstants.InternalServerError);//    Return internal server error code in case of an exception
            }
        }


        #endregion End Corps

        #region Div  


        /// <summary>
        /// Displays the Div management page for users with the "admin" role.
        /// This action is authorized only for users with the "admin" role.
        /// </summary>
        /// <returns>
        /// A view for managing Div data.
        /// </returns>
        [Authorize(Roles = "admin")]
        public IActionResult Div()
        {
            return View();
        }


        /// <summary>
        /// Saves or updates the Div information.
        /// This method is used to either add a new Div or update an existing one based on the provided data.
        /// </summary>
        /// <param name="dTO">The Div data transfer object (DTO) containing the information to be saved or updated.</param>
        /// <returns>
        /// A JSON result indicating the outcome of the operation:
        /// - `KeyConstants.Save` for successful save operation.
        /// - `KeyConstants.Update` for successful update operation.
        /// - `KeyConstants.Exists` if the Div name already exists.
        /// - `ModelState errors` if validation fails.
        /// - `KeyConstants.InternalServerError` if an exception occurs.
        /// </returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveDiv(MDiv dTO)
        {
            try
            {
                // Mark the division as active and set the updated information
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));// Get the current user
                dTO.UpdatedOn = DateTime.Now;
                dTO.DivName = dTO.DivName.Trim(); // Trim any leading/trailing whitespace from the division name

                // Check if the model is valid before proceeding
                if (ModelState.IsValid)
                {
                    // Check if a division with the same name already exists
                    if (!await unitOfWork.Div.GetByName(dTO))
                    {
                        if (dTO.DivId > 0)
                        {
                            //this Div update using UpdateComdCorpsByDivs method
                            //await unitOfWork.Div.Update(dTO);

                            /////update Commd By CorpsId
                            // If it's an existing division, update the associated command and corps
                            MapUnit dat = new MapUnit();
                            dat.Div = dTO;
                            dat.CorpsId = dTO.CorpsId;
                            dat.ComdId = dTO.ComdId;
                            dat.DivId=dTO.DivId;

                            // Update command and corps hierarchy based on the division
                            bool result = await changeHierarchyMaster.UpdateComdCorpsByDivs(dat);
                            if (result) 
                            {
                                return Json(KeyConstants.Update); // Return success message for update
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError); // Return error message if update fails
                            }
                            ////////End Code //////////////
                        }
                        else
                        {
                            await unitOfWork.Div.Add(dTO);// Add a new DIV if DivId is 0
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());// Return validation errors if the model is not valid
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveDiv");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all DIV (MDiv) from the database and returns them in JSON format.
        /// This method is only accessible to users with the "admin" role, and it handles any exceptions 
        /// that may occur during the process, logging errors if necessary.
        /// </summary>
        /// <param name="Id">The identifier (ID) of the command to which the DIVs are related. This ID is used to filter the data.</param>
        /// <returns>
        /// Returns a JSON response containing the list of DIVs. In case of an error, a JSON response 
        /// with an internal server error message is returned.
        /// </returns>
        /// <remarks>
        /// The method fetches all DIV (MDiv) data using the unit of work pattern and the `GetALLDiv` method of the `Div` repository.
        /// If an error occurs during the retrieval process, an exception is caught and logged, and an error response is returned.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllDiv(int Id)
        {
            try
            {
                // Fetch all DIV (MDiv) related to the provided ID
                return Json(await unitOfWork.Div.GetALLDiv());
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                _logger.LogError(1001, ex, "Master->GetAllDiv");

                // Return an error response in case of failure
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Deletes a DIV (MDiv) from the database after verifying it is not referenced in any foreign key tables.
        /// If the division is referenced in the `MBde` or `MapUnit` tables, the deletion is blocked.
        /// </summary>
        /// <param name="dTO">The `MDiv` object containing the DIV to be deleted.</param>
        /// <returns>
        /// - JSON result: 
        ///   - **5** if the DIV is referenced in other tables and cannot be deleted.
        ///   - **KeyConstants.Success** if the DIV is successfully deleted.
        /// </returns>
        /// <remarks>
        /// The method performs the following steps:
        /// 1. Checks if the DIV is referenced in any foreign key tables (e.g., `MBde`, `MapUnit`) using the `DivIdCheckInFKTable` method.
        /// 2. If referenced, the method prevents the deletion and returns a value of 5.
        /// 3. If not referenced, deletes the DIV from the `MDiv` table and returns a success message.
        /// 4. Any errors are logged, and an internal server error message is returned if an exception occurs.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDiv(MDiv dTO)
        {
            try
            {
                // Check if the division is referenced in other tables (MBde, MapUnit)
                DTODivIdCheckInFKTableResponse? dTODivIdCheckIn = await unitOfWork.Div.DivIdCheckInFKTable(dTO.DivId);
                if (dTODivIdCheckIn != null && (dTODivIdCheckIn.TotalBde > 0 || dTODivIdCheckIn.TotalMapUnit > 0))
                {
                    return Json(5);// Division cannot be deleted because it's in use by other tables
                }
                else
                {
                    // Proceed to delete the division if it's not referenced
                    await unitOfWork.Div.Delete(dTO);
                    return Json(KeyConstants.Success);// Return success if deletion is successful
                }

            }
            catch (Exception ex)
            {
                // Log any errors during the process
                _logger.LogError(1001, ex, "Master->DeleteDiv");
                return Json(KeyConstants.InternalServerError);  // Return error if an exception occurs
            }
        }


        /// <summary>
        /// Deletes multiple DIVs (MDiv) based on the provided array of DIV IDs.
        /// This method is intended for bulk deletion of DIVs by their IDs.
        /// </summary>
        /// <param name="ints">An array of DIV IDs to be deleted.</param>
        /// <returns>
        /// Returns a JSON response with a success or error status:
        /// - **Success**: If all specified DIVs are successfully deleted.
        /// - **InternalServerError**: If an exception occurs during the deletion process.
        /// </returns>
        /// <remarks>
        /// The method performs the following actions:
        /// 1. Iterates through the provided array of DIV IDs.
        /// 2. For each DIV ID, it creates a new `MDiv` object, sets the `DivId` to the current ID, and deletes the DIV using the `Delete` method of the `unitOfWork.Div`.
        /// 3. If an exception occurs, it logs the error and returns a failure response indicating the error.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDivMultiple(int[] ints)
        {
            try
            {
                MDiv dto = new MDiv();
                foreach (byte i in ints)
                {
                    dto.DivId = i;
                    await unitOfWork.Div.Delete(dto);   // Deletes the DIV by its ID
                }

                return Json(KeyConstants.Success); // Return success if all deletions were successful
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteDivMultiple");  // Log any errors that occur
                return Json(KeyConstants.InternalServerError);  // Return error response in case of an exception
            }
        }


        #endregion End Bde

        #region Bde  


        /// <summary>
        /// Displays the view for managing the BDE (Brigade) data.
        /// This action is accessible only to users with the "admin" role.
        /// </summary>
        /// <returns>
        /// Returns the view for managing BDEs (Brigades).
        /// The view allows the user to perform actions such as adding, editing, or deleting BDE data.
        /// </returns>
        /// <remarks>
        /// This method does not perform any business logic but simply returns the view that provides 
        /// the interface for the user to manage BDE data. Access is restricted to admin users through 
        /// the [Authorize] attribute, ensuring that only authorized personnel can interact with this page.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public IActionResult Bde()
        {

            return View();
        }


        /// <summary>
        /// Saves or updates a BDE (Brigade) record.
        /// This method is responsible for adding a new BDE record or updating an existing one based on the input.
        /// The action is accessible only to users with the "admin" role.
        /// </summary>
        /// <param name="dTO">The DTO (Data Transfer Object) containing the BDE data to be saved or updated.</param>
        /// <returns>
        /// Returns a JSON response indicating whether the operation was successful or not. 
        /// Possible responses include:
        /// - <see cref="KeyConstants.Save"/> if the new BDE was successfully added.
        /// - <see cref="KeyConstants.Update"/> if the existing BDE was successfully updated.
        /// - <see cref="KeyConstants.Exists"/> if a BDE with the same name already exists.
        /// - <see cref="KeyConstants.InternalServerError"/> in case of an error.
        /// </returns>
        /// <remarks>
        /// The method checks if the BDE already exists by calling the <see cref="unitOfWork.Bde.GetByName"/> method. 
        /// If the BDE exists, it will return a message indicating that the BDE already exists.
        /// If the BDE does not exist, it will either update the existing record or add a new one based on the <paramref name="dTO"/>.
        /// The update process also handles any necessary changes in related records, like updating the associated division and command.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveBde(MBde dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.BdeName = dTO.BdeName.Trim();
                if (ModelState.IsValid)
                {
                    bool? result = await unitOfWork.Bde.GetByName(dTO); // Check if BDE with the same name already exists
                    if (result != null)
                    {
                        if (result == true)
                        {
                            return Json(KeyConstants.Exists);
                        }
                        else
                        {
                            if (dTO.BdeId > 0)
                            {
                                // Update Brigade using UpdateComdCorpsByDivs method
                                MapUnit dat = new MapUnit();
                                dat.Bde = dTO;
                                dat.CorpsId = dTO.CorpsId;
                                dat.ComdId = dTO.ComdId;
                                dat.DivId = dTO.DivId;
                                dat.BdeId = dTO.BdeId;
                                bool result1 = await changeHierarchyMaster.UpdateComdCorpsDivsBybdes(dat); // Update command, corps, and division hierarchy based on the brigade
                                if (result1)
                                {
                                    return Json(KeyConstants.Update);
                                }
                                else
                                {
                                    return Json(KeyConstants.InternalServerError);
                                }
                                ////////End Code //////////////
                                ///
                            }
                            else
                            {
                                await unitOfWork.Bde.Add(dTO);// Add a new BDE if BdeId is 0
                                return Json(KeyConstants.Save);
                            }
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.InternalServerError);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());// Return validation errors if the model is not valid
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveBde");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves a list of all Brigade (BDE) categories from the database.
        /// This method fetches all the Brigade data and returns it in JSON format.
        /// It is accessible only by users with the "admin" role, ensuring secure access.
        /// </summary>
        /// <param name="Id">The ID used to filter the data. The exact usage of this parameter depends on the underlying implementation.</param>
        /// <returns>
        /// A JSON response containing a list of all Brigade (BDE) categories retrieved from the database.
        /// In case of an error, an internal server error message is returned.
        /// </returns>
        /// <remarks>
        /// This method utilizes the `unitOfWork.Bde.GetALLBdeCat()` method to fetch the Brigade data.
        /// If an error occurs during the data retrieval, it logs the error and returns a failure response.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllBde(int Id)
        {
            try
            {
                return Json(await unitOfWork.Bde.GetALLBdeCat());// Fetch all Brigade (BDE) categories from the database
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllBde");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes a Brigade (BDE) after checking if there are any foreign key references in the `MapUnit` table.
        /// If there are foreign references, the deletion is not allowed, and an error message is returned.
        /// </summary>
        /// <param name="dTO">The `MBde` object representing the Brigade to be deleted, containing the `BdeId`.</param>
        /// <returns>
        /// A JSON response indicating the success or failure of the operation:
        /// - `5` if the Brigade has foreign key references and cannot be deleted.
        /// - `KeyConstants.Success` if the deletion is successful.
        /// - `KeyConstants.InternalServerError` if an error occurs during the process.
        /// </returns>
        /// <remarks>
        /// This method first checks if the provided `BdeId` exists in the `MapUnit` table (foreign key references).
        /// If references are found, the method prevents deletion and returns an error.
        /// Otherwise, the Brigade is deleted, and the success status is returned.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBde(MBde dTO)
        {
            try
            {
                // Check if there are any foreign key references to the Brigade (BDE) in the MapUnit table.
                DTOBdeIdCheckInFKTableResponse? dTOBdeIdCheckIn = await unitOfWork.Bde.BdeIdCheckInFKTable(dTO.BdeId); // Check for foreign key references

                if (dTOBdeIdCheckIn != null && (dTOBdeIdCheckIn.TotalMapUnit > 0))
                {
                    // If there are references, return an error indicating the Brigade cannot be deleted
                    return Json(5);
                }
                else
                {
                    // If no references are found, proceed with deletion of the Brigade (BDE)
                    await unitOfWork.Bde.Delete(dTO);
                    return Json(KeyConstants.Success);
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the execution of the method
                _logger.LogError(1001, ex, "Master->DeleteBde");

                // Return an error response if something goes wrong
                return Json(KeyConstants.InternalServerError);
            }
        }



        /// <summary>
        /// Deletes multiple Brigades (BDEs) based on the provided array of `BdeId`s.
        /// The method iterates over each `BdeId`, performs the deletion, and returns a success message if all deletions are successful.
        /// </summary>
        /// <param name="ints">An array of `BdeId` values representing the Brigades to be deleted.</param>
        /// <returns>
        /// A JSON response indicating the success or failure of the operation:
        /// - `KeyConstants.Success` if all deletions are successful.
        /// - `KeyConstants.InternalServerError` if an error occurs during the process.
        /// </returns>
        /// <remarks>
        /// This method iterates over each Brigade ID (`BdeId`) in the input array and attempts to delete each corresponding Brigade. 
        /// If any error occurs during the deletion process, an error message is logged and the method returns an internal server error.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBdeMultiple(int[] ints)
        {
            try
            {
                MBde dto = new MBde();
                // Iterate through each BdeId in the array and delete the corresponding Brigade (BDE)
                foreach (byte i in ints)
                {
                    dto.BdeId = i;
                    // Deleting the Brigade with the current BdeId
                    await unitOfWork.Bde.Delete(dto);
                }

                // Return success message after successfully deleting all selected Brigades
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                // Log the error if any exception occurs during the deletion process
                _logger.LogError(1001, ex, "Master->DeleteBdeMultiple");

                // Return an internal server error if an exception was caught
                return Json(KeyConstants.InternalServerError);
            }
        }


        #endregion End Bde

        #region MapUnit  

        /// <summary>
        /// Displays the MapUnit view for managing or viewing unit mappings.
        /// </summary>
        /// <returns>
        /// The view for displaying the MapUnit page.
        /// </returns>
        /// <remarks>
        /// This action method is used to render the MapUnit page where users can manage unit mappings,
        /// such as linking specific units to their respective command, corps, and division.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public IActionResult MapUnit()
        {
            return View();
        }


        /// <summary>
        /// Retrieves the top unit information based on the provided SUSNo.
        /// </summary>
        /// <param name="SUSNo">The SUSNo (Service Unit Serial Number) used to query the unit data.</param>
        /// <returns>
        /// A JSON response containing the top unit information that matches the provided SUSNo.
        /// If an error occurs, a JSON response with an internal server error is returned.
        /// </returns>
        /// <remarks>
        /// This action method is used to fetch the unit details based on the provided SUSNo.
        /// It queries the database using the unitOfWork pattern to return the top matching unit information.
        /// </remarks>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetTopBySUSNo(string SUSNo)
        {
            try
            {
                // Fetch and return the top unit information by SUSNo using the unitOfWork
                return Json(await unitOfWork.Unit.GetTopBySUSNo(SUSNo));
            }
            catch (Exception ex)
            {
                // Log the error and return an internal server error response
                _logger.LogError(1001, ex, "Master->GetTopBySUSNo");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Saves the unit with mapping by admin after validating the request and checking for existing mappings.
        /// </summary>
        /// <param name="dTO">The DTO object containing the unit and mapping information.</param>
        /// <returns>A JsonResult indicating the success or failure of the save operation.</returns>
        /// <remarks>
        /// This method checks for duplicate units and mappings, and updates or saves the data accordingly.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO)
        {
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                dTO.Suffix= dTO.Suffix.Trim();
                dTO.Sus_no = dTO.Sus_no.Trim();
                if (ModelState.IsValid)
                {
                    string Sus_no = dTO.Sus_no + dTO.Suffix;
                    if (dTO.UnitId > 0 && dTO.UnitMapId == 0)
                    {
                        bool? CheckDuplicate = await unitOfWork.MappUnit.FindUnitId(dTO.UnitId); // Check for duplicate unit ID
                        if (CheckDuplicate == true)
                        {
                            return Json(KeyConstants.Exists);

                        }
                        else if (CheckDuplicate == false)
                        {
                            bool result = (bool)await unitOfWork.MappUnit.SaveUnitWithMapping(dTO);// Save the unit with mapping
                            if (result == true)
                            {
                                return Json(KeyConstants.Save);
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                        else
                        {
                            return Json(KeyConstants.InternalServerError);
                        }

                    }
                    else if (dTO.UnitId > 0 && dTO.UnitMapId > 0)
                    {
                        bool? CheckDuplicate = await unitOfWork.MappUnit.FindUnitIdMapped(dTO.UnitId,dTO.UnitMapId); // Check for duplicate unit ID in mapping
                        if (CheckDuplicate == true)
                        {
                            return Json(KeyConstants.Exists);

                        }
                        else if(CheckDuplicate == false)
                        {
                            bool result = (bool)await unitOfWork.MappUnit.SaveUnitWithMapping(dTO);// Save the unit with mapping
                            if (result == true)
                            {
                                return Json(KeyConstants.Update);
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                        else 
                        {
                            return Json(KeyConstants.InternalServerError);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.InternalServerError);
                    }
                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());// Return validation errors if the model is not valid
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->SaveUnitWithMapping");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Saves the mapping unit, adding or updating it based on whether a map unit ID exists.
        /// </summary>
        /// <param name="dTO">The DTO object containing map unit details.</param>
        /// <returns>A JsonResult indicating the success or failure of the operation.</returns>
        /// <remarks>
        /// This method handles the addition and update of map units. If the unit name already exists, it returns an error.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveMapUnit(MapUnit dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.MappUnit.GetByName(dTO)) // Check if map unit with the same name already exists
                    {
                        if (dTO.UnitMapId > 0)
                        {
                            await unitOfWork.MappUnit.Update(dTO); // Update existing map unit
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            await unitOfWork.MappUnit.Add(dTO); // Add new map unit
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors if the model is not valid
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveMapUnit");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all map units based on the given request parameters.
        /// </summary>
        /// <param name="dTO">The data transfer object containing filter parameters.</param>
        /// <returns>A JsonResult containing the filtered list of map units.</returns>
        /// <remarks>
        /// This method handles the retrieval of map units based on the provided filters. If the filters are not valid, it returns an empty list.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllMapUnit(DTODataTablesRequestForMapUnit dTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Json(await unitOfWork.MappUnit.GetALLUnit(dTO)); // Fetch all map units based on the request parameters
                }
                else
                {
                    List<DTOMapUnitResponse> dTOUserRegnResponses = new List<DTOMapUnitResponse>();
                    var responseData = new DTODataTablesResponse<DTOMapUnitResponse>
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOUserRegnResponses
                    };
                    return Json(responseData);
                    //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllMapUnit");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Retrieves all map units filtered by unit name.
        /// </summary>
        /// <param name="UnitName">The unit name to filter the map units by.</param>
        /// <returns>A JsonResult containing the filtered map units.</returns>
        /// <remarks>
        /// This method retrieves all map units that match the provided unit name, used for searching purposes.
        /// </remarks>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetALLByUnitName(string UnitName)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitName(UnitName));// Fetch all map units filtered by SUS NO
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetALLByUnitName");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Retrieves all map units filtered by unit map ID.
        /// </summary>
        /// <param name="UnitMapId">The unit map ID to filter by.</param>
        /// <returns>A JsonResult containing the filtered map units.</returns>
        /// <remarks>
        /// This method retrieves all map units based on the provided unit map ID.
        /// </remarks>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetALLByUnitMapId(int UnitMapId)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitMapId(UnitMapId)); // Fetch all map units filtered by unit map ID
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetALLByUnitMapId");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Retrieves all map units filtered by unit map ID based on the session's unit ID.
        /// </summary>
        /// <returns>A JsonResult containing the filtered map units.</returns>
        /// <remarks>
        /// This method retrieves all map units filtered by the unit ID stored in the session.
        /// </remarks>
        public async Task<IActionResult> GetALLByUnitMapWonUnit(int UnitMapId)
        {
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                }
                return Json(await unitOfWork.MappUnit.GetALLByUnitMapId(dtoSession.UnitId));// Fetch all map units filtered by unit map ID based on session's unit ID
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetALLByUnitMapId");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Retrieves all map units by the given unit ID.
        /// </summary>
        /// <param name="UnitId">The unit ID to filter the map units by.</param>
        /// <returns>A JsonResult containing the filtered map units.</returns>
        /// <remarks>
        /// This method retrieves all map units filtered by the unit ID provided.
        /// </remarks>
        public async Task<IActionResult> GetALLByUnitById(int UnitId)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitById(UnitId));// Fetch all map units filtered by unit ID
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetALLByUnitById");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes a map unit by its unit map ID.
        /// </summary>
        /// <param name="UnitMapId">The unit map ID of the map unit to be deleted.</param>
        /// <returns>A JsonResult indicating the success or failure of the deletion operation.</returns>
        /// <remarks>
        /// This method checks for any foreign key references before allowing the deletion of a map unit.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMapUnit(int UnitMapId)
        {
            try
            {
                DTOUnitMapIdCheckInFKTableResponse? dTOUnitMapId = await unitOfWork.MappUnit.UnitMapIdCheckInFKTable(UnitMapId); // Check for foreign key references
                if (dTOUnitMapId != null && (dTOUnitMapId.TotalBD > 0 || dTOUnitMapId.TotalRO >0 || dTOUnitMapId.TotalTDM >0 || dTOUnitMapId.TotalTF> 0 || dTOUnitMapId.TotalTPOFrom>0 || dTOUnitMapId.TotalTPOTo>0)) // If there are references, return an error indicating the map unit cannot be deleted
                {
                    return Json(5); // Map unit cannot be deleted because it's in use by other tables
                }
                else
                {
                    await unitOfWork.MappUnit.Delete(UnitMapId); // Proceed to delete the map unit if it's not referenced
                    return Json(KeyConstants.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteMapUnit");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Deletes multiple map units by their unit map IDs.
        /// </summary>
        /// <param name="ints">The array of unit map IDs to be deleted.</param>
        /// <returns>A JsonResult indicating the success or failure of the deletion operation.</returns>
        /// <remarks>
        /// This method allows the deletion of multiple map units by passing an array of unit map IDs.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMapUnitMultiple(int[] ints)
        {
            try
            {
                MapUnit dto = new MapUnit();
                foreach (int i in ints) // Iterate through each unit map ID in the array and delete the corresponding map unit
                {
                    dto.UnitMapId = i;
                    await unitOfWork.MappUnit.Delete(dto); // Deletes the map unit by its unit map ID
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteMapUnitMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Retrieves units by their hierarchy.
        /// </summary>
        /// <param name="Data">The DTO object containing hierarchy filter data.</param>
        /// <returns>A JsonResult containing the units filtered by their hierarchy.</returns>
        /// <remarks>
        /// This method retrieves units based on the provided hierarchy filter.
        /// </remarks>
        public async Task<IActionResult> GetUnitByHierarchy(DTOMHierarchyRequest Data)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetUnitByHierarchy(Data));// Fetch units by their hierarchy based on the provided filter data
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetUnitByHierarchy");
                return Json(KeyConstants.InternalServerError);
            }

        }


        #endregion End Unit

        #region Map Unit Change Request

        /// <summary>
        /// Displays the view for MapUnitChange and assigns the role name to the ViewBag.
        /// </summary>
        /// <returns>Returns the View for MapUnitChange.</returns>
        public IActionResult MapUnitChange() 
        {
            string RoleName = string.Empty;
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            RoleName = dtoSession != null ? dtoSession.RoleName : string.Empty;
            ViewBag.RoleName = RoleName;
            return View();
        }


        /// <summary>
        /// Retrieves the unit move history for a given MapUnitChangeRequestId.
        /// </summary>
        /// <param name="MapUnitChangeRequestId">The ID of the MapUnitChangeRequest.</param>
        /// <returns>Returns the unit move history details in JSON format.</returns>
        public async Task<IActionResult> GetUnitMoveHistory(int MapUnitChangeRequestId)
        {
            try
            {
                TrnMapUnitChangeRequest? mapUnitChangeRequest = await _mapUnitChangeBL.Get(MapUnitChangeRequestId);

                if (mapUnitChangeRequest != null)
                {
                    DTOMapUnitDetailsResponse dTOMapUnitDetails = new DTOMapUnitDetailsResponse();

                    // Map existing data and request data
                    string[] ExistingCh = mapUnitChangeRequest.ExistingCh.Split('#');
                    string[] RequestCh = mapUnitChangeRequest.RequestCh.Split('#');


                    dTOMapUnitDetails.MapUnitChangeRequestId = MapUnitChangeRequestId;
                    dTOMapUnitDetails.ComdId = Convert.ToByte(RequestCh[1]);
                    dTOMapUnitDetails.CorpsId = Convert.ToByte(RequestCh[2]);
                    dTOMapUnitDetails.DivId = Convert.ToByte(RequestCh[3]);
                    dTOMapUnitDetails.BdeId = Convert.ToByte(RequestCh[4]);
                    dTOMapUnitDetails.FmnBranchID = Convert.ToByte(RequestCh[5]);
                    dTOMapUnitDetails.PsoId = Convert.ToByte(RequestCh[6]);
                    dTOMapUnitDetails.SubDteId = Convert.ToByte(RequestCh[7]);

                    dTOMapUnitDetails = await _mapUnitChangeBL.GetUnitMoveHistory(dTOMapUnitDetails);
                    
                    dTOMapUnitDetails.MapUnitChangeRequestId = MapUnitChangeRequestId;
                    dTOMapUnitDetails.UnitAbbreviation = ExistingCh[0];
                    dTOMapUnitDetails.Sus_no = ExistingCh[1];
                    dTOMapUnitDetails.ExistingUnitType = Convert.ToInt32(ExistingCh[2]);
                    dTOMapUnitDetails.ExistingComdName = ExistingCh[3];
                    dTOMapUnitDetails.ExistingCorpsName = ExistingCh[4];
                    dTOMapUnitDetails.ExistingDivName = ExistingCh[5];
                    dTOMapUnitDetails.ExistingBdeName = ExistingCh[6];
                    dTOMapUnitDetails.ExistingBranchName = ExistingCh[7];
                    dTOMapUnitDetails.ExistingPSOName = ExistingCh[8];
                    dTOMapUnitDetails.ExistingSubDteName = ExistingCh[9];

                    dTOMapUnitDetails.RequestUnitType = Convert.ToInt32(RequestCh[0]);
                    dTOMapUnitDetails.ComdId = Convert.ToByte(RequestCh[1]);
                    dTOMapUnitDetails.CorpsId = Convert.ToByte(RequestCh[2]);
                    dTOMapUnitDetails.DivId = Convert.ToByte(RequestCh[3]);
                    dTOMapUnitDetails.BdeId = Convert.ToByte(RequestCh[4]);
                    dTOMapUnitDetails.FmnBranchID = Convert.ToByte(RequestCh[5]);
                    dTOMapUnitDetails.PsoId = Convert.ToByte(RequestCh[6]);
                    dTOMapUnitDetails.SubDteId = Convert.ToByte(RequestCh[7]);

                    dTOMapUnitDetails.UnitMapId = mapUnitChangeRequest.UnitMapId;
                    return Json(dTOMapUnitDetails);
                }
                else
                {
                    return Json(KeyConstants.InternalServerError);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetChangeMapUnitDetails");
                return Json(KeyConstants.InternalServerError);
            }
        }



        /// <summary>
        /// Retrieves change map unit details for a given MapUnitChangeRequestId.
        /// </summary>
        /// <param name="MapUnitChangeRequestId">The ID of the MapUnitChangeRequest.</param>
        /// <returns>Returns the change map unit details in JSON format.</returns>
        public async Task<IActionResult> GetChangeMapUnitDetails(int MapUnitChangeRequestId)
        {
            try
            {
                TrnMapUnitChangeRequest? mapUnitChangeRequest = await _mapUnitChangeBL.Get(MapUnitChangeRequestId); // Retrieve the map unit change request by its ID

                if (mapUnitChangeRequest != null) // If the map unit change request exists
                {
                    DTOProfileResponse? dTOProfile = await userProfileBL.GetProfileByUserId(mapUnitChangeRequest.FromUserId);

                    string[] ExistingCh = mapUnitChangeRequest.ExistingCh.Split('#');
                    string[] RequestCh = mapUnitChangeRequest.RequestCh.Split('#');
                    DTOChangeMapUnitDetailsResponse dTOChangeMapUnit = new DTOChangeMapUnitDetailsResponse();
                    dTOChangeMapUnit.UnitName = ExistingCh[0];
                    dTOChangeMapUnit.Sus_no= ExistingCh[1];
                    dTOChangeMapUnit.ExistingCh_UnitType = Convert.ToInt32(ExistingCh[2]);
                    dTOChangeMapUnit.ComdName = ExistingCh[3];
                    dTOChangeMapUnit.CorpsName = ExistingCh[4];
                    dTOChangeMapUnit.DivName = ExistingCh[5];
                    dTOChangeMapUnit.BdeName = ExistingCh[6];
                    dTOChangeMapUnit.BranchName = ExistingCh[7];
                    dTOChangeMapUnit.PSOName = ExistingCh[8];
                    dTOChangeMapUnit.SubDteName = ExistingCh[9];
                    
                    dTOChangeMapUnit.RequestCh_UnitType = Convert.ToInt32(RequestCh[0]);
                    dTOChangeMapUnit.ComdId = Convert.ToByte(RequestCh[1]);
                    dTOChangeMapUnit.CorpsId = Convert.ToByte(RequestCh[2]);
                    dTOChangeMapUnit.DivId = Convert.ToByte(RequestCh[3]);
                    dTOChangeMapUnit.BdeId = Convert.ToByte(RequestCh[4]);
                    dTOChangeMapUnit.FmnBranchID = Convert.ToByte(RequestCh[5]);
                    dTOChangeMapUnit.PsoId = Convert.ToByte(RequestCh[6]);
                    dTOChangeMapUnit.SubDteId = Convert.ToByte(RequestCh[7]);

                    dTOChangeMapUnit.UnitMapId = mapUnitChangeRequest.UnitMapId;
                    dTOChangeMapUnit.Remark = mapUnitChangeRequest.Remark;

                    if (dTOProfile != null)
                    {
                        dTOChangeMapUnit.RequestBy = dTOProfile.RankAbbreviation + " "+ dTOProfile.Name+" ("+ dTOProfile.ArmyNo + ")";
                    }

                    return Json(dTOChangeMapUnit);
                    //return Json(await unitOfWork.MappUnit.GetALLByUnitMapId(UnitMapId));
                }
                else
                {
                    return Json(KeyConstants.InternalServerError);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetChangeMapUnitDetails");
                return Json(KeyConstants.InternalServerError);
            }

        }



        /// <summary>
        /// Retrieves a list of all MapUnitChange requests based on the provided data transfer object.
        /// </summary>
        /// <param name="dTO">The DTO containing the necessary request parameters.</param>
        /// <returns>Returns the list of all MapUnitChange requests in JSON format.</returns>
        [HttpPost]
        public async Task<IActionResult> GetAllMapUnitChange(DTODataTablesRequestForMapUnitChange dTO)
        {
            string RoleName = string.Empty;
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            RoleName = dtoSession != null ? dtoSession.RoleName : string.Empty;
            try
            {
                dTO.RoleName = RoleName;
                dTO.UnitMapId = dtoSession != null ? dtoSession.UnitId : 0;
                return Json(await _mapUnitChangeBL.GetAllMapUnitChange(dTO)); // Fetch all map unit change requests based on the provided parameters
            }
            catch (Exception ex)
            {
                List<DTOProfileManageResponse> dTOUserRegnResponses = new List<DTOProfileManageResponse>();
                var responseData = new DTODataTablesResponse<DTOProfileManageResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = dTOUserRegnResponses
                };
                _logger.LogError(1001, ex, "Master->GetAllMapUnitChange");
                return Json(responseData);
            }
        }

        /// <summary>
        /// Handles the MapUnitChange request and saves the changes for the unit mapping.
        /// </summary>
        /// <param name="Id">The encrypted ID of the MapUnitChange request.</param>
        /// <returns>Returns the view or error message based on the process outcome.</returns>
        public async Task<IActionResult> MapUnitChangeRequest(string? Id)
        {
            int MapUnitId = 0;
            string RoleName = string.Empty;
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            int AspNetUsersId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;
            RoleName = dtoSession != null ? dtoSession.RoleName : string.Empty;
            if (Id != null)
            {
                try
                {
                    // Decrypt the  id using Unprotect method
                    decryptedId = protector.Unprotect(Id);

                    // Validate decrypted Id
                    if (!int.TryParse(decryptedId, out decryptedIntId))
                    {
                        _logger.LogWarning("Decrypted Id is not a valid integer: {DecryptedId}, UserId: {UserId}", decryptedId, AspNetUsersId);
                        TempData["error"] = "Invalid Request.";
                        TempData.Keep("error");
                        return RedirectToAction("ContactUs", "Home");
                    }
                    else
                    {
                        if (RoleName == "admin")
                        {
                            TrnMapUnitChangeRequest mapUnitChangeRequest = await _mapUnitChangeBL.Get(decryptedIntId);
                            if(mapUnitChangeRequest.IsEditAction == true)
                            {
                                TempData["error"] = "This action has already been completed by you.";
                                TempData.Keep("error");
                                return RedirectToAction("ContactUs", "Home");
                            }
                            else 
                            {
                                ViewBag.MapUnitId = MapUnitId;
                                ViewBag.RoleName = RoleName;
                                ViewBag.MapUnitChangeRequestId = mapUnitChangeRequest.MapUnitChangeRequestId;
                                return View();
                            }
                        }
                        else
                        {
                            TempData["error"] = "You are not authorized to edit this action.";
                            TempData.Keep("error");
                            return RedirectToAction("Dashboard", "Home");
                        }
                    }
                }
                catch (System.Security.Cryptography.CryptographicException ex)
                {
                    _logger.LogError(ex, "Cryptographic error occurred while processing the Id: {Id}.", Id);
                    TempData["error"] = "Invalid or tampered request.";
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, message: "This error occure because Id : {Id} value change by user.", Id);
                    TempData["error"] = ex.Message;
                    TempData.Keep("error");
                    return RedirectToAction("ContactUs", "Home");
                }
            }



            if (MapUnitId > 0)
            {
                bool result = await _mapUnitChangeBL.FindUnitIdMapped(MapUnitId);
                if(result)
                {
                    TempData["error"] = "Unit move change request has already been submitted.";
                    TempData.Keep("error");
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ViewBag.MapUnitId = MapUnitId;
                    ViewBag.RoleName = RoleName;
                    return View();
                }
            }
            else
            {
                TempData["error"] = "Session expired.";
                TempData.Keep("error");
                return RedirectToAction("Dashboard", "Home");
            }

        }


        /// <summary>
        /// Saves the MapUnitChangeRequest based on the provided data.
        /// </summary>
        /// <param name="dTO">The DTO containing the MapUnitChangeRequest data.</param>
        /// <returns>Returns a response indicating the success or failure of the save operation.</returns>
        public async Task<IActionResult> SaveMapUnitChangeRequest(DTOSaveMapUnitChangeRequest dTO)
        {
            DTOCommonSaveResponse dTOCommon = new DTOCommonSaveResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (dTO.MapUnitChangeRequestId > 0 || dTO.MapUnitChangeRequestId < 0)
                    {
                        dTOCommon.Result = false;
                        dTOCommon.Message = "This action is not allowed for you. Please check.";
                        return Json(dTOCommon);
                    }
                    else
                    {
                        int MapUnitId = 0;
                        DtoSession? dtoSession = new DtoSession();
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                        {
                            dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                        }
                        MapUnitId = dtoSession != null ? dtoSession.UnitId : 0;
                        if (MapUnitId > 0)
                        {
                            bool result = await _mapUnitChangeBL.FindUnitIdMapped(MapUnitId); // Check if a change request already exists for the unit
                            if (result)
                            {
                                TempData["error"] = "Unit Mapping Change Request already place.";
                                TempData.Keep("error");
                                return RedirectToAction("Dashboard", "Home");
                            }
                            else
                            {
                                DTOMapUnitResponse dTOMap =await unitOfWork.MappUnit.GetALLByUnitMapId(MapUnitId); // Retrieve the current mapping details for the unit


                                string ExistingCh = string.Join("#", new[]
                                {
                                    dTOMap.UnitName,
                                    $"{dTOMap.Sus_no}{dTOMap.Suffix}",
                                    dTOMap.UnitType.ToString(),
                                    dTOMap.ComdName,
                                    dTOMap.CorpsName,
                                    dTOMap.DivName,
                                    dTOMap.BdeName,
                                    dTOMap.BranchName,
                                    dTOMap.PSOName,
                                    dTOMap.SubDteName
                                });
                                string RequestCh = string.Join("#", new[]
                                {
                                    dTO.UnitType.ToString(),
                                    dTO.ComdId.ToString(),
                                    dTO.CorpsId.ToString(),
                                    dTO.DivId.ToString(),
                                    dTO.BdeId.ToString(),
                                    dTO.FmnBranchID.ToString(),
                                    dTO.PsoId.ToString(),
                                    dTO.SubDteId.ToString(),
                                });
                                TrnMapUnitChangeRequest unitChangeRequest = new TrnMapUnitChangeRequest
                                {
                                    MapUnitChangeRequestId = dTO.MapUnitChangeRequestId,
                                    UnitMapId = MapUnitId,
                                    ExistingCh = ExistingCh,
                                    RequestCh = RequestCh,
                                    Remark = dTO.Remark,
                                    AdminRemark = null,
                                    IsActive = true,
                                    IsComplete = false,
                                    RequestStatus = false,
                                    FromUserId = dtoSession != null ? dtoSession.UserId : 0,
                                    Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                                    UpdatedOn = DateTime.Now,
                                    AdminUserId = null,
                                    AdminUpdatedby = null,
                                    AdminUpdatedOn = null,
                                };
                                TrnMapUnitChangeRequest response = await _mapUnitChangeBL.AddWithReturn(unitChangeRequest); // Save the change request and get the response
                                dTOCommon.Result = true;
                                dTOCommon.Id = response.MapUnitChangeRequestId.ToString();
                                dTOCommon.CurrentTime= response.UpdatedOn ?? DateTime.Now;
                                dTOCommon.Message = "Unit Mapping Change request place successfully ";
                                return Json(dTOCommon);
                            }
                        }
                        else
                        {
                            TempData["error"] = "Session expired.";
                            TempData.Keep("error");
                            return RedirectToAction("Dashboard", "Home");
                        }
                    }
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0) // Check for model validation errors
                                .SelectMany(x => x.Value!.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();
                    if (errors.Any())
                    {
                        dTOCommon.Message = string.Join("; ", errors); // Concatenate all error messages
                    }
                    dTOCommon.Result = false;
                    return Json(dTOCommon);
                }
            }
            catch (Exception ex) {
                dTOCommon.Result = false;
                dTOCommon.Message = ex.Message;
                return Json(dTOCommon);
            }
        }


        /// <summary>
        /// Updates a Map Unit Change Request.
        /// This action can only be performed by users with the "admin" role.
        /// It checks whether the Map Unit Change Request can be updated and ensures that the action hasn't already been performed.
        /// </summary>
        /// <param name="dTO">The data transfer object containing the Map Unit Change Request information to be updated.</param>
        /// <returns>A JSON response indicating the result of the update action.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateMapUnitChangeRequest(DTOSaveMapUnitChangeRequest dTO)
        {
            DTOCommonSaveResponse dTOCommon = new DTOCommonSaveResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    TrnMapUnitChangeRequest? mapUnitChangeRequest = await _mapUnitChangeBL.Get(dTO.MapUnitChangeRequestId); // Retrieve the existing Map Unit Change Request by its ID
                    if (mapUnitChangeRequest!= null && mapUnitChangeRequest.IsEditAction == true) // Check if the request has already been edited
                    {
                        dTOCommon.Result = false;
                        dTOCommon.Message = "This action has already been completed by you.";
                        return Json(dTOCommon);
                    }
                    else if (mapUnitChangeRequest != null && mapUnitChangeRequest.IsEditAction == false) // If the request exists and hasn't been edited yet, proceed with the update
                    {
                        DtoSession? dtoSession = new DtoSession();
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                        {
                            dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                        }
                        mapUnitChangeRequest.IsEditAction = true;
                        mapUnitChangeRequest.IsComplete = true;
                        mapUnitChangeRequest.AdminUpdatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        mapUnitChangeRequest.AdminUserId = dtoSession != null ? dtoSession.UserId : 0;
                        mapUnitChangeRequest.AdminUpdatedOn = DateTime.Now;
                        return Json(await _mapUnitChangeBL.UpdateMapUnitChangeRequest(dTO, mapUnitChangeRequest)); // Call the business logic layer to update the request and return the result
                    }
                    else
                    {
                        dTOCommon.Result = false;
                        dTOCommon.Message = "Invalid input.";
                        return Json(dTOCommon);
                    }
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                                .SelectMany(x => x.Value!.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();
                    if (errors.Any())
                    {
                        dTOCommon.Message = string.Join("; ", errors); // Concatenate all error messages
                    }
                    dTOCommon.Result = false;
                    return Json(dTOCommon);
                }
            }
            catch (Exception ex)
            {
                dTOCommon.Result = false;
                dTOCommon.Message = ex.Message;
                return Json(dTOCommon);
            }
        }

        #endregion End Map Unit Change Request

        #region Unit  


        /// <summary>
        /// Returns the view for Unit.
        /// </summary>
        /// <returns>The Unit view.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult Unit()
        {
            return View();
        }

        /// <summary>
        /// Saves or updates a unit depending on the presence of a UnitId.
        /// </summary>
        /// <param name="dTO">The unit data transfer object containing unit details.</param>
        /// <returns>A JSON result indicating whether the unit was saved or updated.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveUnit(MUnit dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.UnitName = dTO.UnitName.Trim();
                dTO.Abbreviation = dTO.Abbreviation != null ? dTO.Abbreviation.Trim() : dTO.Abbreviation;
                dTO.Suffix = dTO.Suffix.Trim();
                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Unit.GetByName(dTO))
                    {
                        if (dTO.UnitId > 0)
                        {
                            await unitOfWork.Unit.Update(dTO); // Update existing unit
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            await unitOfWork.Unit.Add(dTO); // Add new unit
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveUnit");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all units with pagination and filters based on the provided request.
        /// </summary>
        /// <param name="dTO">The data transfer object containing pagination and filter information.</param>
        /// <returns>A JSON result containing the filtered list of units.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllUnit(DTODataTablesRequest dTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Json(await unitOfWork.Unit.GetAllUnit(dTO)); // Fetch all units based on the provided pagination and filter information
                }
                else
                {
                    List<MUnit> dTOUserRegnResponses = new List<MUnit>();
                    var responseData = new DTODataTablesResponse<MUnit>
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = dTOUserRegnResponses
                    };
                    return Json(responseData);
                    //return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllUnit");
                return Json(KeyConstants.InternalServerError);
            }
        }



        /// <summary>
        /// Deletes a specific unit based on its UnitId after checking for foreign key dependencies.
        /// </summary>
        /// <param name="dTO">The unit data transfer object containing the UnitId.</param>
        /// <returns>A JSON result indicating success or failure.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUnit(MUnit dTO)
        {
            try
            {
                DTOUnitIdCheckInFKTableResponse? dTOUnitIdCheckIn = await unitOfWork.Unit.UnitIdCheckInFKTable(dTO.UnitId); // Check for foreign key dependencies
                if (dTOUnitIdCheckIn != null && (dTOUnitIdCheckIn.TotalMapUnit > 0))
                {
                    return Json(5); // Return 5 if the unit is referenced in other tables and cannot be deleted
                }
                else
                {
                    await unitOfWork.Unit.Delete(dTO); // Delete the unit
                    return Json(KeyConstants.Success);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteUnit");
                return Json(KeyConstants.InternalServerError);
            }
        }



        /// <summary>
        /// Deletes multiple units based on their UnitIds.
        /// </summary>
        /// <param name="ints">The array of UnitIds to be deleted.</param>
        /// <returns>A JSON result indicating success or failure.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUnitMultiple(int[] ints)
        {
            try
            {
                MUnit dto = new MUnit();
                foreach (int i in ints)
                {
                    dto.UnitId = i;
                    await unitOfWork.Unit.Delete(dto); // Delete each unit by its UnitId
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteUnitMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Retrieves a unit by its Sus_no and Suffix.
        /// </summary>
        /// <param name="Data">The unit data transfer object containing the Sus_no and Suffix.</param>
        /// <returns>A JSON result containing the unit details or an error if not found.</returns>
        public async Task<IActionResult> GetBySusNO(MUnit Data)
        {
            try
            {
                var ret = await unitOfWork.Unit.GetBySusNo(Data.Sus_no + Data.Suffix); // Fetch the unit by its Sus_no and Suffix
                return Json(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetBySusNO");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Retrieves a unit by its UnitId for anonymous access.
        /// </summary>
        /// <param name="UnitId">The UnitId of the unit to retrieve.</param>
        /// <returns>A JSON result containing the unit details or an error if not found.</returns>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetUnitByUnitId(int UnitId)
        {
            try
            {
                var ret = await unitOfWork.Unit.Get(UnitId); // Fetch the unit by its UnitId
                var result = new DTOUnitResponse
                {
                    UnitId = ret.UnitId,
                    Sus_no = (ret.Sus_no + ret.Suffix).ToUpper(),
                    UnitName = ret.UnitName,
                    Abbreviation = ret.Abbreviation,
                    IsVerify = ret.IsVerify,
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetUnitByUnitId");
                return Json(KeyConstants.InternalServerError);
            }

        }

        #endregion End Unit

        #region Formation  

        /// <summary>
        /// Displays the Formation view for users with 'admin' role.
        /// </summary>
        /// <returns>View for Formation</returns>
        [Authorize(Roles = "admin")]
        public IActionResult Formation()
        {
            return View();
        }


        /// <summary>
        /// Saves the formation details. Updates the existing record if FormationId is provided, otherwise, creates a new formation.
        /// </summary>
        /// <param name="dTO">The formation object to be saved.</param>
        /// <returns>JSON response indicating success, update, or validation errors.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveFormation(MFormation dTO)
        {
            try
            {
               
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Formation.GetByName(dTO)) // Check if formation with the same name already exists
                    {
                        if (dTO.FormationId > 0)
                        {
                            await unitOfWork.Formation.Update(dTO); // Update existing formation
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Formation.Add(dTO); // Add new formation
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }


        /// <summary>
        /// Retrieves all formations for display or processing.
        /// </summary>
        /// <returns>JSON response containing a list of all formations.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllFormation()
        {
            try
            {
                return Json(await unitOfWork.Formation.GetAll());// Fetch all formations
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes a specific formation based on the provided DTO.
        /// </summary>
        /// <param name="dTO">The formation object to be deleted.</param>
        /// <returns>JSON response indicating success or failure.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteFormation(MFormation dTO)
        {
            try
            {
                await unitOfWork.Formation.Delete(dTO); // Delete the formation
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Deletes multiple formations based on an array of formation IDs.
        /// </summary>
        /// <param name="ints">An array of formation IDs to delete.</param>
        /// <returns>JSON response indicating success or failure.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteFormationMultiple(int[] ints)
        {
            try
            {
                MFormation dto = new MFormation();
                foreach (byte i in ints) // Iterate through each formation ID
                {
                    dto.FormationId = i;
                    await unitOfWork.Formation.Delete(dto); // Delete the formation
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }


        #endregion End Formation

        #region Appt  

        /// <summary>
        /// Displays the appointment view for admin users.
        /// </summary>
        /// <returns>The view for managing appointments.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult Appointment()
        {
            return View();
        }


        /// <summary>
        /// Saves an appointment. If the user is not authenticated, sets the appointment as unapproved.
        /// </summary>
        /// <param name="dTO">The appointment data transfer object.</param>
        /// <returns>JSON result indicating whether the appointment was saved or updated.</returns>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> SaveAppointment(MAppointment dTO)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Contains("admin"))
                    {
                        dTO.Approved = false;
                        dTO.ApptId = 0;
                    }
                    dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                }
                else
                {
                    dTO.ApptId = 0;
                    dTO.Approved = false;
                    dTO.Updatedby = null;
                }
                   
                dTO.IsActive = true;
                dTO.UpdatedOn = DateTime.Now;
                dTO.AppointmentName = dTO.AppointmentName.Trim();
                
                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Appt.GetByName(dTO)) // Check if an appointment with the same name already exists
                    {
                        if (dTO.ApptId > 0) 
                        {
                            await unitOfWork.Appt.Update(dTO); // Update existing appointment
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Appt.Add(dTO); // Add new appointment
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors
                }

            }
            catch (Exception ex) {
                _logger.LogError(1001, ex, "Master->SaveAppointment");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all appointments for admin users.
        /// </summary>
        /// <returns>JSON result containing all appointments.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllAppointment()
        {
            return Json(await unitOfWork.Appt.GetALLAppt()); // Fetch all appointments
        }


        /// <summary>
        /// Retrieves an appointment by its ID.
        /// </summary>
        /// <param name="ApptId">The ID of the appointment to retrieve.</param>
        /// <returns>JSON result containing the appointment details.</returns>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetByApptId(short ApptId)
        {
            try
            {
                return Json(await unitOfWork.Appt.GetByApptId(ApptId)); // Fetch appointment by its ID
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes an appointment, ensuring that it is not referenced in any foreign key tables.
        /// </summary>
        /// <param name="dTO">The appointment data transfer object to delete.</param>
        /// <returns>JSON result indicating whether the appointment was successfully deleted.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAppointment(MAppointment dTO)
        {
            try
            {
                DTOApptIdCheckInFKTableResponse? dTOApptIdCheckIn = await unitOfWork.Appt.ApptIdCheckInFKTable(dTO.ApptId); // Check for foreign key dependencies
                if (dTOApptIdCheckIn != null && (dTOApptIdCheckIn.TotalTDM > 0)) // If the appointment is referenced in other tables
                {
                    return Json(5); // Return 5 indicating it cannot be deleted
                }
                else
                {
                    await unitOfWork.Appt.Delete(dTO); // Delete the appointment
                    return Json(KeyConstants.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteAppointment");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Deletes multiple appointments at once.
        /// </summary>
        /// <param name="ints">The array of appointment IDs to delete.</param>
        /// <returns>JSON result indicating whether the appointments were successfully deleted.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAppointmentMultiple(short[] ints)
        {
            try
            {
                MAppointment dto = new MAppointment();
                foreach (byte i in ints) // Iterate through each appointment ID
                {
                    dto.ApptId = i;
                    await unitOfWork.Appt.Delete(dto); // Delete the appointment
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Retrieves appointments by appointment name.
        /// </summary>
        /// <param name="AppointmentName">The name of the appointment to search for.</param>
        /// <returns>JSON result containing appointments matching the name.</returns>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetALLByAppointmentName(string AppointmentName)
        {
            try
            {
                return Json(await unitOfWork.Appt.GetALLByAppointmentName(AppointmentName)); // Fetch appointments by name
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }


        #endregion End Appointment

        #region Rank Page

        /// <summary>
        /// Renders the Rank master view for administrators.
        /// </summary>
        /// <remarks>
        /// The view is expected to bootstrap its data via subsequent API calls
        /// such as <see cref="GetAllRank(int[])"/>.
        /// </remarks>
        /// <returns>The Rank management view.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult Rank()
        {
            return View();
        }


        /// <summary>
        /// Creates or updates a Rank record.
        /// </summary>
        /// <param name="dTO">Rank model carrying fields like RankId, RankName, RankAbbreviation, etc.</param>
        /// <returns>
        /// JSON:
        /// - <c>KeyConstants.Save</c> when a new rank is created,
        /// - <c>KeyConstants.Update</c> when an existing rank is updated,
        /// - <c>KeyConstants.Exists</c> when a duplicate (by name/abbr as per repo logic) is detected,
        /// - ModelState errors when input is invalid,
        /// - <c>KeyConstants.InternalServerError</c> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// For new ranks, the display order is set using <c>unitOfWork.Rank.GetByMaxOrder()</c>.
        /// Uses audit fields: <c>IsActive</c>, <c>Updatedby</c>, <c>UpdatedOn</c>.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveRank(MRank dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.RankName = dTO.RankName.Trim();
                dTO.RankAbbreviation = dTO.RankAbbreviation.Trim();

                if (ModelState.IsValid)
                {
                    // Uniqueness check as defined in repository
                    if (!await unitOfWork.Rank.GetByName(dTO))
                    {
                        if (dTO.RankId > 0)
                        {
                            // Update flow
                            await unitOfWork.Rank.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            // Create flow: set visual/processing order
                            dTO.Orderby = await unitOfWork.Rank.GetByMaxOrder();
                            await unitOfWork.Rank.Add(dTO);
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {
                    // Validation errors packaged for client display
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveRank");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all ranks ordered as defined in the repository.
        /// </summary>
        /// <param name="Id">
        /// An optional list of IDs (currently unused).
        /// Retained for signature compatibility; consider removing if not needed.
        /// </param>
        /// <returns>
        /// JSON array of ranks from <c>unitOfWork.Rank.GetAllByorder()</c>,
        /// or <c>KeyConstants.InternalServerError</c> on failure.
        /// </returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllRank(int[] Id)
        {
            try
            {
                // Repository returns the ordered list (e.g., by Orderby)
                return Json(await unitOfWork.Rank.GetAllByorder());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllRank");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes a single rank if it is not referenced by dependent tables.
        /// </summary>
        /// <param name="dTO">Rank model containing the <c>RankId</c> to delete.</param>
        /// <returns>
        /// JSON:
        /// - <c>5</c> when the rank is in use (FK check fails),
        /// - <c>KeyConstants.Success</c> when deleted,
        /// - <c>KeyConstants.InternalServerError</c> on exception.
        /// </returns>
        /// <remarks>
        /// Performs FK usage check via <c>unitOfWork.Rank.RankIdCheckInFKTable</c>
        /// (e.g., TotalBD / TotalBDT / TotalUP counters).
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRank(MRank dTO)
        {
            try
            {
                // Defensive FK usage check before deletion
                DTORankIdCheckInFKTableResponse? dTORankIdCheckIn = await unitOfWork.Rank.RankIdCheckInFKTable(dTO.RankId);
                if (dTORankIdCheckIn != null && (dTORankIdCheckIn.TotalBD > 0 || dTORankIdCheckIn.TotalBDT > 0 || dTORankIdCheckIn.TotalUP >0))
                {
                    // In-use sentinel value expected by client
                    return Json(5);
                }
                else
                {
                    await unitOfWork.Rank.Delete(dTO);
                    return Json(KeyConstants.Success);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRank");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Changes the display/order value of a rank.
        /// </summary>
        /// <param name="dTO">
        /// Rank model carrying ordering information (e.g., <c>Orderby</c> and <c>RankId</c>).
        /// </param>
        /// <returns>
        /// JSON <c>KeyConstants.Success</c> on success,
        /// or <c>KeyConstants.InternalServerError</c> on failure.
        /// </returns>
        /// <remarks>
        /// The actual ordering logic is encapsulated in <c>unitOfWork.Rank.OrderByChange</c>.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RankOrderByChange(MRank dTO)
        {
            try
            {
                await unitOfWork.Rank.OrderByChange(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->RankOrderByChange");
                return Json(KeyConstants.InternalServerError);
            }


        }


        /// <summary>
        /// Deletes multiple ranks by their identifiers.
        /// </summary>
        /// <param name="ints">Array of rank IDs to delete.</param>
        /// <returns>
        /// JSON <c>KeyConstants.Success</c> when all deletions are attempted,
        /// or <c>KeyConstants.InternalServerError</c> if an exception occurs.
        /// </returns>
        /// <remarks>
        /// This method does not do a pre-check for FK usage on each ID.
        /// Consider calling <see cref="DeleteRank(MRank)"/> semantics per ID,
        /// or adding a batch FK check for robustness.
        /// </remarks>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRankMultiple(int[] ints)
        {
            try
            {
                MRank dto = new MRank();
                foreach (byte i in ints)
                {
                    dto.RankId = i;
                    await unitOfWork.Rank.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRankMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }


        #endregion Command

        #region ArmedType Page

        /// <summary>
        /// Displays the view for managing ArmedTypes. Only accessible by users with the "admin" role.
        /// </summary>
        /// <returns>The ArmedType management view.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult ArmedType()
        {
            return View();
        }

        /// <summary>
        /// Saves a new or updated ArmedType to the database.
        /// It checks if the ArmedType already exists by name before saving or updating.
        /// </summary>
        /// <param name="dTO">The ArmedType data transfer object containing the details to be saved.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveArmed(MArmedType dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.ArmedName = dTO.ArmedName.Trim();
                dTO.Abbreviation= dTO.Abbreviation.Trim().ToUpper();

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Armed.GetByName(dTO)) // Check if an ArmedType with the same name already exists
                    {
                        if (dTO.ArmedId > 0)
                        {
                            await unitOfWork.Armed.Update(dTO); // Update existing ArmedType
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            await unitOfWork.Armed.Add(dTO); // Add new ArmedType
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveArmed");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all ArmedType entries from the database.
        /// </summary>
        /// <returns>A JSON result containing the list of all ArmedTypes.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllArmed()
        {
            try
            {
                return Json(await unitOfWork.Armed.GetALLArmed()); // Fetch all ArmedTypes
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllArmed");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes a specific ArmedType from the database if it is not referenced in other tables.
        /// If the ArmedType is being used in foreign keys, it will not be deleted.
        /// </summary>
        /// <param name="dTO">The ArmedType data transfer object containing the ArmedId to be deleted.</param>
        /// <returns>A JSON result indicating success, failure, or foreign key dependency.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteArmed(MArmedType dTO)
        {
            try
            {
                DTOArmedIdCheckInFKTableResponse? dTOArmedIdCheckIn = await unitOfWork.Armed.ArmedIdCheckInFKTable(dTO.ArmedId); // Check for foreign key dependencies
                if (dTOArmedIdCheckIn != null && (dTOArmedIdCheckIn.TotalBD > 0 || dTOArmedIdCheckIn.TotalRO >0))
                {
                    return Json(5); // Return 5 if the ArmedType is referenced in other tables and cannot be deleted
                }
                else
                {
                    await unitOfWork.Armed.Delete(dTO); // Delete the ArmedType
                    return Json(KeyConstants.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteArmed");
                return Json(KeyConstants.InternalServerError);
            }
        }

        /// <summary>
        /// Deletes multiple ArmedTypes from the database based on their ArmedId values.
        /// </summary>
        /// <param name="ints">An array of ArmedId values to delete.</param>
        /// <returns>A JSON result indicating success or failure of the operation.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteArmedMultiple(int[] ints)
        {
            try
            {
                MArmedType dto = new MArmedType();
                foreach (byte i in ints) // Iterate through each ArmedId
                {
                    dto.ArmedId = i;
                    await unitOfWork.Armed.Delete(dto); // Delete the ArmedType
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteArmedMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion ArmedType

        #region Regimental Page

        /// <summary>
        /// Displays the Regimental view for admin users.
        /// </summary>
        /// <returns>Returns the Regimental view.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult Regimental()
        {
            return View();
        }


        /// <summary>
        /// Saves a regimental record. It adds or updates the record based on the RegId.
        /// </summary>
        /// <param name="dTO">The regimental data transfer object containing the regimental details.</param>
        /// <returns>Returns a JSON response indicating the result of the save operation.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveRegimental([FromBody] MRegimental dTO)
        {
            // FormBody used when send null in Unit then not support action method (treat as string, not null)
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.Name = dTO.Name.Trim();
                dTO.Abbreviation=dTO.Abbreviation.Trim().ToUpper();
                dTO.Location= dTO.Location.Trim();

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Regimental.GetByName(dTO)) // Check if a regimental with the same name already exists
                    {
                        if (dTO.RegId > 0) // Update existing regimental
                        {
                            await unitOfWork.Regimental.Update(dTO); // Update existing regimental
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            await unitOfWork.Regimental.Add(dTO); // Add new regimental
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveRegimental");
                return Json(KeyConstants.InternalServerError); 
            }

        }


        /// <summary>
        /// Retrieves all regimental records.
        /// </summary>
        /// <param name="Id">An array of regimental IDs to filter the results (optional).</param>
        /// <returns>Returns a JSON response with the list of all regimental records.</returns>
        public async Task<IActionResult> GetAllRegimental(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Regimental.GetAllData()); // Fetch all regimental records
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllRegimental");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes a specific regimental record.
        /// </summary>
        /// <param name="dTO">The regimental data transfer object containing the regimental details to be deleted.</param>
        /// <returns>Returns a JSON response indicating the result of the delete operation.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRegimental(MRegimental dTO)
        {
            try
            {
                await unitOfWork.Regimental.Delete(dTO); // Delete the regimental record
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRegimental");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Deletes multiple regimental records based on the given array of regimental IDs.
        /// </summary>
        /// <param name="ints">An array of regimental IDs to be deleted.</param>
        /// <returns>Returns a JSON response indicating the result of the multiple delete operation.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRegimentalMultiple(int[] ints)
        {
            try
            {
                MRegimental dto = new MRegimental();
                foreach (byte i in ints) // Iterate through each regimental ID
                {
                    dto.RegId = i;
                    await unitOfWork.Regimental.Delete(dto); // Delete the regimental record
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRegimentalMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion ArmedType

        #region Record Office

        /// <summary>
        /// Displays the record office page for the admin.
        /// </summary>
        /// <returns>Returns the view for the record office.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult RecordOffice()
        {
            short ArmedIdForORO = Convert.ToInt16(_configuration["HardCodeId:ArmedIdForORO"]); //Get from appsettings.json
            //if (ArmedIdForORO == 0) ArmedIdForORO = 56;
            ViewBag.ArmedIdForORO = ArmedIdForORO;

            return View();
        }


        /// <summary>
        /// Saves a record office in the system.
        /// </summary>
        /// <param name="dTO">Data Transfer Object for the record office.</param>
        /// <returns>Returns a JSON result indicating the status of the operation.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> SaveRecordOffice(MRecordOffice dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.Name = dTO.Name.Trim();
                dTO.Abbreviation = dTO.Abbreviation.Trim().ToUpper();

                if (ModelState.IsValid)
                {
                    int result = await unitOfWork.RecordOffice.GetByName(dTO); // Check if a record office with the same name already exists
                    if (result == 1)
                    {
                        if (dTO.RecordOfficeId > 0)
                        {
                            await unitOfWork.RecordOffice.Update(dTO); // Update existing record office
                            return Json(6);
                        }
                        else
                        {
                            await unitOfWork.RecordOffice.Add(dTO); // Add new record office
                            return Json(5);
                        }
                    }
                    else
                    {
                        if(result == 2) // Exists
                        {
                            return Json(2);
                        }
                        //else if(result == 3)
                        //{
                        //    return Json(3);
                        //}
                        else if(result == 4) // Incorrect Data
                        {
                            return Json(4);
                        }
                        else
                        {
                            return Json(0);
                        }
                        
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            //catch (DbUpdateException ex) when (ex.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            //{
            //    _logger.LogError(1001, ex, "Master->SaveRecordOffice");
            //    return Json(KeyConstants.Exists);
            //}
            //catch (UniqueConstraintException ex)
            //{
            //    _logger.LogError(1001, ex, "Master->SaveRecordOffice");
            //    return Json(KeyConstants.Exists);
            //}
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->SaveRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Retrieves all record office data.
        /// </summary>
        /// <returns>Returns a JSON response containing all record office data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetAllRecordOffice()
        {
            try
            {
                return Json(await unitOfWork.RecordOffice.GetAllData()); // Fetch all record office data
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes a record office from the system.
        /// </summary>
        /// <param name="dTO">The record office DTO to be deleted.</param>
        /// <returns>Returns a JSON response indicating the success of the deletion.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteRecordOffice(MRecordOffice dTO)
        {
            try
            {
                await unitOfWork.RecordOffice.Delete(dTO); // Delete the record office
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Retrieves mapped record office data based on the type and search name.
        /// </summary>
        /// <param name="TypeId">The type ID for filtering the records.</param>
        /// <param name="SearchName">The name to search within the records.</param>
        /// <returns>Returns a JSON response with the mapped data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetMappedForRecord(int TypeId, string SearchName)
        {
            try
            {
                return Json(await unitOfWork.MasterBL.GetMappedForRecord(TypeId, SearchName)); // Fetch mapped record office data
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }



        /// <summary>
        /// Retrieves the domain ID based on the TDM ID.
        /// </summary>
        /// <param name="TDMId">The TDM ID to search for the domain.</param>
        /// <returns>Returns a JSON response with the domain ID.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> GetDomainIdByTDMId(int TDMId)
        {
            try
            {
                return Json(await unitOfWork.MasterBL.GetDomainIdByTDMId(TDMId)); // Fetch domain ID by TDM ID
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Retrieves record office data for updating a specific record office based on its ID.
        /// </summary>
        /// <param name="RecordOfficeId">The ID of the record office to retrieve.</param>
        /// <returns>Returns a JSON response with the record office data for updating.</returns>
        [HttpPost]
        public async Task<IActionResult> GetUpdateRecordOffice(int RecordOfficeId)
        {
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                }
                int UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                int TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;
                return Json(await unitOfWork.RecordOffice.GetUpdateRecordOffice(RecordOfficeId)); // Fetch record office data for updating
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetUpdateRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Displays the update page for the record office, ensuring the user is authorized.
        /// </summary>
        /// <returns>Returns the view for updating the record office if authorized, else redirects to an error page.</returns>
        [HttpGet]
        public async Task<IActionResult> UpdateRecordOffice()
        {
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            
            // Get the unit and TDM IDs from session
            int UnitId = dtoSession != null ? dtoSession.UnitId : 0;
            int TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;
            int UserId = dtoSession != null ? dtoSession.UserId : 0;
            ViewBag.UnitId = UnitId;


            DTOGetROByTDMIdResponse? dTOGetROByUserIdResponse = await unitOfWork.RecordOffice.GetROByTDMId(TDMId); // Fetch record office details by TDM ID
            if (dTOGetROByUserIdResponse == null)
            {
                TempData["error"] = "You are not authorizes this page.";
                return RedirectToActionPermanent("DashboardUserMgt", "Home");
            }
            else if (dTOGetROByUserIdResponse.IsRO == true || dTOGetROByUserIdResponse.IsORO == true || dTOGetROByUserIdResponse.TDMId == TDMId) // Check if the user is authorized to access this page
            {
                ViewBag.ROId = dTOGetROByUserIdResponse.RecordOfficeId;
                ViewBag.TDMId = dTOGetROByUserIdResponse.TDMId;
                return View();
            }
            else
            {
                TempData["error"] = "You are not authorizes this page.";
                return RedirectToActionPermanent("DashboardUserMgt", "Home");
            }
        }


        /// <summary>
        /// Retrieves DD mapped data for a specific unit map.
        /// </summary>
        /// <param name="UnitMapId">The ID of the unit map to retrieve mapped data for.</param>
        /// <returns>Returns a JSON response with the mapped DD data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetDDMappedForRecord(int UnitMapId)
        {
            try
            {
                return Json(await unitOfWork.RecordOffice.GetDDMappedForRecord(UnitMapId)); // Fetch DD mapped data for the specified unit map
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetDDMappedForRecord");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Updates the record office value based on the provided request.
        /// </summary>
        /// <param name="dTO">Data Transfer Object containing the updated record office value.</param>
        /// <returns>Returns a JSON response indicating the success of the update operation.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateROValue(DTOUpdateROValueRequest dTO)
        {
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (dTO.TDMId == dTO.OldTDMId)
                    {
                        return Json(1);
                    }
                    else
                    {
                        bool? result = (bool)await unitOfWork.RecordOffice.UpdateROValue(dTO); // Update the record office value
                        if (result ==true)
                        {
                            return Json(2);
                        }
                        else if(result == null)
                        {
                            return Json(4);
                        }
                        else
                        {
                            return Json(3);
                        }
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->SaveRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }

        }


        #endregion

        #region OROMapping

        /// <summary>
        /// Displays the ORO Mapping view for users with the "admin" role.
        /// </summary>
        /// <returns>Returns the view for ORO Mapping.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult OROMapping()
        {
            return View();
        }


        /// <summary>
        /// Retrieves all ORO mappings from the database.
        /// </summary>
        /// <returns>
        /// A <see cref="JsonResult"/> containing a list of all ORO mappings.
        /// If an error occurs, a JSON response indicating internal server error is returned.
        /// </returns>
        /// <exception cref="Exception">Throws when an error occurs during the retrieval of ORO mappings.</exception>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllOROMapping()
        {
            try
            {
                // Fetch all ORO mappings from the unit of work and return them as a JSON response
                return Json(await unitOfWork.OROMapping.GetAllOROMapping());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllOROMapping");
                return Json(KeyConstants.InternalServerError);
            }

        }



        /// <summary>
        /// Saves or updates the ORO mapping in the database.
        /// </summary>
        /// <param name="dTO">The ORO mapping data transfer object (DTO) containing the information to be saved or updated.</param>
        /// <returns>
        /// A JSON response indicating whether the ORO mapping was successfully saved or updated.
        /// Returns a custom error message ("5") if required fields (RankId or ArmedIdList) are missing.
        /// If model state validation fails, the validation errors are returned as a JSON response.
        /// </returns>
        /// <remarks>
        /// This method checks if the provided RankId and ArmedIdList are valid before proceeding.
        /// If the provided ORO mapping has an existing ID, it updates the mapping; otherwise, it creates a new entry.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if the RankId or ArmedIdList is null or empty.</exception>
        /// <exception cref="Exception">Throws if any exception occurs during the save or update operation.</exception>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveOROMapping(OROMapping dTO)
        {
            try
            {
                // Validate if required fields are provided
                if ((dTO.RankId == null || dTO.RankId == 0) && (dTO.ArmedIdList == null || dTO.ArmedIdList ==""))
                {
                    return Json("5");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        // If ORO Mapping has a valid ID, update it; otherwise, save as new
                        if (dTO.OROMappingId > 0)
                        {
                            await unitOfWork.OROMapping.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            await unitOfWork.OROMapping.Add(dTO);
                            return Json(KeyConstants.Save);
                        }
                    }
                    else
                    {
                        // Return model validation errors
                        return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList());
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->SaveRegimental");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Retrieves the list of arms (weapon systems) associated with the ORO mapping.
        /// </summary>
        /// <returns>
        /// A JSON response containing a list of arms. The list is fetched from the business layer.
        /// </returns>
        /// <exception cref="Exception">Throws when an error occurs while fetching the list of arms.</exception>
        public async Task<IActionResult> GetArmsList()
        {
            return Json(await _IMasterBL.GetArmsList());// Fetch the list of arms from the business layer and return as JSON
        }


        /// <summary>
        /// Deletes the specified ORO mapping from the database.
        /// </summary>
        /// <param name="dTO">The ORO Mapping object to be deleted.</param>
        /// <returns>
        /// A JSON response indicating the success or failure of the delete operation.
        /// If an error occurs, an internal server error message is returned.
        /// </returns>
        /// <remarks>
        /// This method attempts to delete an ORO mapping from the database. If the operation is successful,
        /// a success message is returned. If an exception occurs, the error is logged, and an internal server
        /// error message is returned.
        /// </remarks>
        /// <exception cref="Exception">Throws if any exception occurs during the delete operation.</exception>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteOROMapping(OROMapping dTO)
        {
            try
            {
                await unitOfWork.OROMapping.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteOROMapping");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion OROMapping

        #region AfsacCellMapping


        /// <summary>
        /// Displays the AFSAC Cell Mapping view for admin users.
        /// </summary>
        /// <returns>View for AFSAC Cell Mapping.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult AfsacCellMapping()
        {
            return View();
        }


        /// <summary>
        /// Retrieves all AFSAC Cell Mappings from the database.
        /// </summary>
        /// <returns>A JSON result containing the list of AFSAC Cell Mappings.</returns>
        /// <exception cref="Exception">Throws exception if retrieval fails.</exception>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllAfsacCellMapping()
        {
            try
            {
                return Json(await unitOfWork.AfsacCellMapping.GetAllAfsacCellMapping()); // Fetch all AFSAC Cell Mappings
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllAfsacCellMapping");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Saves the AFSAC Cell Mapping data, either adding a new record or updating an existing one.
        /// </summary>
        /// <param name="dTO">The AFSAC Cell Mapping data to be saved.</param>
        /// <returns>A JSON result indicating success or failure of the save operation.</returns>
        /// <exception cref="Exception">Throws exception if save operation fails.</exception>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveAfsacCellMapping(AfsacCellMapping dTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (dTO.AfsacCellMappingId > 0) // Update existing AFSAC Cell Mapping
                    {
                        await unitOfWork.AfsacCellMapping.Update(dTO); // Update existing AFSAC Cell Mapping
                        return Json(KeyConstants.Update);
                    }
                    else
                    {
                        await unitOfWork.AfsacCellMapping.Add(dTO); // Add new AFSAC Cell Mapping
                        return Json(KeyConstants.Save);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value?.Errors).Where(y => y?.Count > 0).ToList()); // Return validation errors
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->SaveAfsacCellMapping");
                return Json(KeyConstants.InternalServerError);
            }

        }


        /// <summary>
        /// Deletes an existing AFSAC Cell Mapping record from the database.
        /// </summary>
        /// <param name="dTO">The AFSAC Cell Mapping data to be deleted.</param>
        /// <returns>A JSON result indicating success or failure of the delete operation.</returns>
        /// <exception cref="Exception">Throws exception if deletion fails.</exception>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAfsacCellMapping(AfsacCellMapping dTO)
        {
            try
            {
                await unitOfWork.AfsacCellMapping.Delete(dTO); // Delete the AFSAC Cell Mapping
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteAfsacCellMapping");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion AfsacCellMapping

        #region Master Table 

        /// <summary>
        /// Retrieves all master data based on the provided request parameters.
        /// </summary>
        /// <param name="Data">The data transfer object containing the request parameters for fetching master data.</param>
        /// <returns>A JSON response containing the result of the request. Returns an internal server error if an exception occurs.</returns>
        [AllowAnonymous]
        [AnySessionRequired]
        [HttpPost]
        public async Task<IActionResult> GetAllMMaster([FromBody] DTOMasterRequest data)
        {
            if (data == null)
                return BadRequest(new { error = "Request body required." });

            try
            {
                var result = await unitOfWork.GetAllMMaster(data);
                return Json(result);
            }
            catch
            {
                return Json(KeyConstants.InternalServerError);
            }
        }


        /// <summary>
        /// Retrieves all master data based on the parent specified in the request.
        /// </summary>
        /// <param name="Data">The data transfer object containing the request parameters, including the parent identifier.</param>
        /// <returns>A JSON response containing the result of the request. Returns an internal server error if an exception occurs.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetAllMMasterByParent(DTOMHierarchyRequest Data)
        {
            try
            {
                var ret = await unitOfWork.GetAllMMasterByParent(Data); // Fetch all master data based on the parent specified in the request
                return Json(ret);
            }
            catch
            {
                return Json(KeyConstants.InternalServerError);
            }
        }


        #endregion End Master

        #region Dashboard

        /// <summary>
        /// Displays the Dashboard Master page. This action is accessible only to users with the 'admin' role.
        /// </summary>
        /// <returns>The view for the Dashboard Master page.</returns>
        [Authorize(Roles = "admin")]
        public IActionResult DashboardMaster()
        {
            return View();
        }

        /// <summary>
        /// Retrieves the count of dashboard master records.
        /// This action is asynchronous and accessible only to users with the 'admin' role.
        /// </summary>
        /// <returns>JSON result containing the count of dashboard master records.</returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetDashboardMasterCount()
        {
            return Json(await unitOfWork.MasterBL.GetDashboardMasterCount());// Fetch dashboard master count data and return as JSON
        }
        
        
        #endregion Dashboard
    }
}
