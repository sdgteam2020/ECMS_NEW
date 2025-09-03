using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public abstract class GenericRepositoryDL<T> : IGenericRepositoryDL<T> where T : class 
    {
        protected readonly ApplicationDbContext _context;

        protected GenericRepositoryDL(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves an entity of type <typeparamref name="T"/> by its unique identifier.
        /// If the entity with the provided ID is not found, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The entity of type <typeparamref name="T"/> if found, otherwise throws an exception.</returns>
        /// <exception cref="ArgumentNullException">Thrown when no entity with the provided ID is found in the database.</exception>
        public async Task<T> Get(int id)
        {
            // Use FindAsync to asynchronously find the entity by its ID
            var entity = await _context.Set<T>().FindAsync(id);

            // If the entity is not found, throw an ArgumentNullException with the parameter name
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            
            // Return the found entity
            return entity;

        }

        /// <summary>
        /// Asynchronously retrieves an entity of type <typeparamref name="T"/> by a given generic value.
        /// If the entity with the provided value is not found, a <see cref="KeyNotFoundException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <typeparam name="T2">The type of the value used to search for the entity (e.g., ID or other unique field).</typeparam>
        /// <param name="val1">The value used to find the entity (such as a unique identifier or other searchable property).</param>
        /// <returns>The entity of type <typeparamref name="T"/> if found, otherwise throws a <see cref="KeyNotFoundException"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no entity with the provided value is found in the database.</exception>
        public async Task<T> GetByGen<T2>(T2 val1)
        {
            // Use FindAsync to asynchronously find the entity by the provided value (val1)
            var entity = await _context.Set<T>().FindAsync(val1);
            
            // If the entity is not found, throw a KeyNotFoundException with the provided value
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {val1} not found.");
            }
            
            // Return the found entity
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves an entity of type <typeparamref name="T"/> by a byte-based ID.
        /// If the entity with the provided ID is not found, a <see cref="KeyNotFoundException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="id">The byte-based unique identifier of the entity to retrieve.</param>
        /// <returns>The entity of type <typeparamref name="T"/> if found, otherwise throws a <see cref="KeyNotFoundException"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no entity with the provided ID is found in the database.</exception>
        public async Task<T> GetByByte(byte id)
        {
            // Use FindAsync to asynchronously search for the entity by the provided ID
            var entity = await _context.Set<T>().FindAsync(id);

            // If no entity is found, throw a KeyNotFoundException with the provided ID
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }
            
            // Return the found entity
            return entity;
                 
        }


        /// <summary>
        /// Asynchronously retrieves all entities of type <typeparamref name="T"/> from the database.
        /// If no entities are found, an empty collection is returned instead of null.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <returns>A collection of entities of type <typeparamref name="T"/> if found, otherwise an empty collection.</returns>
        public async Task<IEnumerable<T>> GetAll()
        {
            var result = await _context.Set<T>().ToListAsync();
            
            // Handle the case where result is null or empty
            if (result == null)
            {
                return Enumerable.Empty<T>();  // Return an empty collection if null
            }
            return result;
        }

        /// <summary>
        /// Asynchronously adds a new entity of type <typeparamref name="T"/> to the database.
        /// If the entity is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the entity to add.</typeparam>
        /// <param name="entity">The entity to be added to the database.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided entity is null.</exception>
        public async Task Add(T entity)
        {
            // Validate the entity to ensure it is not null
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Add the entity to the corresponding DbSet asynchronously
            await _context.Set<T>().AddAsync(entity);

            // Save the changes asynchronously to the database
            await SaveAsync();
        }


        /// <summary>
        /// Asynchronously adds a new entity of type <typeparamref name="T"/> to the database and returns the added entity.
        /// If the entity is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the entity to add.</typeparam>
        /// <param name="entity">The entity to be added to the database.</param>
        /// <returns>The added entity of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided entity is null.</exception>
        public async Task<T> AddWithReturn(T entity)
        {
            // Validate the entity to ensure it is not null
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            // Add the entity to the corresponding DbSet
            _context.Set<T>().Add(entity);

            // Save the changes asynchronously to the database
            await _context.SaveChangesAsync();

            // Return the added entity
            return entity;
        }


        /// <summary>
        /// Asynchronously saves all changes made in the context to the database.
        /// This method commits any modifications, additions, or deletions made to entities in the context.
        /// </summary>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task SaveAsync()
        {
            // Asynchronously save all changes to the database
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Asynchronously deletes the specified entity of type <typeparamref name="T"/> from the database.
        /// If the entity is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the entity to delete.</typeparam>
        /// <param name="entity">The entity to be deleted from the database.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided entity is null.</exception>
        public async Task Delete(T entity)
        { 
            // Validate the entity to ensure it is not null
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Remove the entity from the corresponding DbSet
            _context.Set<T>().Remove(entity);

            // Save the changes asynchronously to the database (commit the delete operation)
            await SaveAsync();
        }


        /// <summary>
        /// Asynchronously updates the specified entity of type <typeparamref name="T"/> in the database.
        /// If the entity is null, an <see cref="ArgumentNullException"/> is thrown.
        /// The method marks the entity as modified and saves the changes to the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity to update.</typeparam>
        /// <param name="entity">The entity to be updated in the database.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided entity is null.</exception>
        public async Task Update(T entity)
        {
            // Validate the entity to ensure it is not null
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Mark the entity as modified so that the context will track the changes
            _context.Entry(entity).State = EntityState.Modified;

            // Save the changes asynchronously to the database
            await SaveAsync();
        }


        /// <summary>
        /// Asynchronously updates the specified entity of type <typeparamref name="T"/> in the database and returns the updated entity.
        /// If the entity is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the entity to update.</typeparam>
        /// <param name="entity">The entity to be updated in the database.</param>
        /// <returns>The updated entity of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided entity is null.</exception>
        public async Task<T> UpdateWithReturn(T entity)
        {
            // Validate the entity to ensure it is not null
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Mark the entity as modified so that the context will track the changes
            _context.Entry(entity).State = EntityState.Modified;

            // Save the changes asynchronously to the database
            await _context.SaveChangesAsync();

            // Return the updated entity
            return entity;
        }


        /// <summary>
        /// Asynchronously deletes the entity of type <typeparamref name="T"/> with the specified ID from the database.
        /// If the entity is not found, a <see cref="KeyNotFoundException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the entity to delete.</typeparam>
        /// <param name="id">The ID of the entity to be deleted from the database.</param>
        /// <returns>The deleted entity of type <typeparamref name="T"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no entity with the provided ID is found in the database.</exception>
        public async Task<T> Delete(int id)
        {
            // Use FindAsync to asynchronously search for the entity by the provided ID
            var entity = await _context.Set<T>().FindAsync(id);

            // If no entity is found, throw a KeyNotFoundException with the provided ID
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }

            // Remove the found entity from the corresponding DbSet
            _context.Set<T>().Remove(entity);

            // Save the changes asynchronously to the database
            await _context.SaveChangesAsync();

            // Return the deleted entity
            return entity;
        }


        /// <summary>
        /// Asynchronously retrieves a paginated response for a data table, applying filtering, sorting, and pagination based on the provided request.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="request">The request object containing parameters for filtering, sorting, and pagination.</param>
        /// <returns>A <see cref="DTODataTablesResponse{T}"/> containing the data for the requested page, along with total records count and filtered records count.</returns>
        public async Task<DTODataTablesResponse<T>> GetDataTableResponse(DTODataTablesRequest request)
        {
            var responseData = new DTODataTablesResponse<T>
            {
                draw = request.Draw,
                recordsTotal = 0, // Total records without filtering
                recordsFiltered = 0, // Total records after filtering
                data = new List<T> { },
            };
            try
            {
                var queryableData = _context.Set<T>().AsQueryable();

                // Total records without filtering
                var totalRecords = queryableData.Count();

                // Apply filtering
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    queryableData = queryableData.Where(item => EF.Property<string>(item, request.Choice).ToLower().Contains(searchValue));
                }

                //Apply sorting
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    //queryableData = queryableData.OrderBy(request.SortColumn + " " + request.SortColumnDirection);
                    queryableData = request.sortDirection.ToLower() == "asc"
                    ? queryableData.OrderBy(item => EF.Property<object>(item, request.sortColumn))
                    : queryableData.OrderByDescending(item => EF.Property<object>(item, request.sortColumn));
                }

                // Total records after filtering
                var filteredRecords = queryableData.Count();

                // Paginate the result
                var paginatedData = await queryableData.Skip(request.Start).Take(request.Length).ToListAsync();

                responseData = new DTODataTablesResponse<T>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords, // Total records without filtering
                    recordsFiltered = filteredRecords, // Total records after filtering
                    data = paginatedData
                };

            }
            catch (Exception ee)
            { 
                
            }
            return responseData;
        }

    }
}
