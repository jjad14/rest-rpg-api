namespace rpg_api.Models
{
    // ServiceResponse is used to return a wrapper object to the client with every service call. 
    // Advantages are that you can add additional information to the returning result like a 
    // success or exception message.
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = null;
    }
}