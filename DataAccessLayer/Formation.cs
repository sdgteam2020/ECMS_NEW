using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class FormationDB : GenericRepositoryDL<MFormation>, IFormationDB
    {
        protected new readonly ApplicationDbContext _context;
        public FormationDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Checks whether a formation with the specified name exists in the database, case-insensitively.
        /// </summary>
        /// <param name="Dto">The `MFormation` DTO containing the formation name to search for.</param>
        /// <returns>A boolean value indicating whether the formation name exists in the database.</returns>
        public async Task<bool> GetByName(MFormation Dto)
        {
            // Use AnyAsync for efficiency, and perform a case-insensitive comparison directly in the query
            var ret = await _context.MFormation
                                    .AnyAsync(p => p.FormationName.Equals(Dto.FormationName, StringComparison.OrdinalIgnoreCase));

            return ret;
        }
    }
}