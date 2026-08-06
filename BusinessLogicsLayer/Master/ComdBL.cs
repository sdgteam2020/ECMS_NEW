using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class ComdBL : GenericRepositoryDL<MComd>, IComdBL
    {
        protected new readonly ApplicationDbContext _context;// For Entity Framework operations
        private readonly IComdDB _iComdDB;
        private readonly ILogger<ComdBL> _logger;
        public ComdBL(ApplicationDbContext context, IComdDB comdDB, ILogger<ComdBL> logger) : base(context)
        {
            _iComdDB= comdDB;
            _logger = logger;
            _context = context;
        }

        public Task<IEnumerable<MComd>> GetAllByorder()
        {
            return _iComdDB.GetAllByorder();
        }

        public Task<DTOTreeViewUnitResponse> GetBinaryTree(int Id)
        {
            return _iComdDB.GetBinaryTree(Id);
        }

        public Task<int> GetByMaxOrder()
        {
            return _iComdDB.GetByMaxOrder();    
        }

        public Task<bool> GetByName(MComd Dto)
        {
            Dto.ComdName = Dto.ComdName.Trim().TrimEnd().TrimStart();    
           return _iComdDB.GetByName(Dto);   
        }

        /// <summary>
        /// Updates the order number of the command (MComd) by adjusting the `Orderby` value and swapping with the next command's order number.
        /// </summary>
        /// <param name="Dto">The <see cref="MComd"/> object containing the command ID and the new order number to be updated.</param>
        /// <returns>
        /// Returns a byte value indicating the result of the operation, where <c>KeyConstants.Success</c> signifies a successful operation.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. It retrieves the current order of the command from the <paramref name="Dto"/> object.
        /// 2. It then checks if the next order number exists in the database (using <see cref="_iComdDB.GetComdIdbyOrderby"/>).
        /// 3. If the next order number exists, it updates the order number for both the current command and the next command:
        ///    - The next command's order number is updated to the current command's order number.
        ///    - The current command's order number is updated to the next available order number.
        /// 4. The method uses a loop to increment the order number and check for an available next command order until a valid next command is found.
        /// </remarks>
        public async Task<DTOGenericResponse<string>> OrderByChange(MComd dto)
        {
            DTOGenericResponse<string> response = new DTOGenericResponse<string>();

            // Default response
            response.Result = false;
            response.Message = string.Empty;
            response.Value = string.Empty;

            if (dto == null || dto.ComdId == 0)
            {
                response.Message = "Invalid command details.";
                return response;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                MComd? currentCommand = await GetByGen<byte>(dto.ComdId);

                if (currentCommand == null)
                {
                    response.Message = "Command record not found.";
                    return response;
                }

                int currentOrder = currentCommand.Orderby;
                int nextOrder = currentOrder;

                MComd? nextCommand = null;

                while (nextOrder < int.MaxValue)
                {
                    nextOrder++;

                    byte nextCommandId = await _iComdDB.GetComdIdbyOrderby(nextOrder);

                    if (nextCommandId == 0)
                        continue;

                    nextCommand = await GetByGen<byte>(nextCommandId);

                    if (nextCommand != null)
                        break;
                }

                if (nextCommand == null)
                {
                    response.Message = "The selected command is already at the last position.";
                    return response;
                }

                // Swap order numbers
                nextCommand.Orderby = currentOrder;
                currentCommand.Orderby = nextOrder;

                // Mark both records as modified
                _context.Entry(nextCommand).State = EntityState.Modified;
                _context.Entry(currentCommand).State = EntityState.Modified;

                // Save both records in one operation
                await SaveAsync();

                await transaction.CommitAsync();

                response.Result = true;
                response.Message = "Order changed successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(1001, ex, "ComdBL->OrderByChange");
                response.Message = "An error occurred while changing the command order.";
            }
            return response;
        }
        public async Task<DTOComdIdCheckInFKTableResponse?> ComdIdCheckInFKTable(byte ComdId)
        {
            return await _iComdDB.ComdIdCheckInFKTable(ComdId);
        }
        public async Task<DTODataTablesResponse<DTOAllCommand_PaginationResponse>> GetAllCommand_Pagination(DTODataTablesRequest dTO)
        {
            return await _iComdDB.GetAllCommand_Pagination(dTO);
        }
    }
}
