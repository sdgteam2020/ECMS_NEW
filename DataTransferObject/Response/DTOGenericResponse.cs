namespace DataTransferObject.Response
{
    public class DTOGenericResponse<T>
    {
        public bool Result { get; set; }  // Represents whether the operation was successful or not
        public string Message { get; set; } // Message to be displayed to the end user
        public T Value { get; set; } // Can hold any type (List, int, string, etc.)
        public int StatusId { get; set; } 

        // Constructor to initialize the response
        public DTOGenericResponse(bool result, string message, T value, int statusId)
        {
            Result = result;
            Message = message;
            Value = value;
            StatusId = statusId;
        }

        // Optional: You can add a default constructor if you need one
        public DTOGenericResponse() { }
    }
}
