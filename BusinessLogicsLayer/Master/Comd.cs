using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class Comd : GenericRepositoryDL<MComd>, IComd
    {
        private readonly IComdDB _iComdDB;

        public Comd(ApplicationDbContext context, IComdDB comdDB) : base(context)
        {
            _iComdDB= comdDB;   
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
        public async Task<byte> OrderByChange(MComd Dto)
        {
            ////Current Order
            int i = Dto.Orderby;
            increment:
            i++;
            
            // Get the command ID of the next order number
            byte ComdIdnext =await _iComdDB.GetComdIdbyOrderby(i);

            // If the next command ID is zero, increment and check again
            if (ComdIdnext == 0)
            {
                goto increment;
            }
            else 
            {
                // Subtraction order no Next Comd
                // Update the next command's order number to the current command's order number
                var datanext = await GetByByte(ComdIdnext);
                datanext.Orderby = Dto.Orderby;
                await Update(datanext);

                ////////Change Order No For Click
                // Update the current command's order number to the next available order number
                MComd data = new MComd();
                data = await GetByByte(Dto.ComdId);
                data.Orderby = i;
                await Update(data);
                /////////////////////////

            }
            // Return success code after successfully updating the order numbers
            return KeyConstants.Success;
        }
        public async Task<DTOComdIdCheckInFKTableResponse?> ComdIdCheckInFKTable(byte ComdId)
        {
            return await _iComdDB.ComdIdCheckInFKTable(ComdId);
        }
    }
}
