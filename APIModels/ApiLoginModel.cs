namespace EventManagerASP.APIModels
{
    public class ApiLoginModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; internal set; }
    }
}
