namespace LeaveApplication.Components.Services
{
    public class UserStateService
    {
        public string UserName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public string UserRole { get; set; } = "";
        public string UserId { get; set; } = "";

        public event Action? OnChange;

        public void SetUser(string Role, string name, string email)
        {
            UserRole = Role;
            UserName = name;
            UserEmail = email;

            NotifyStateChanged();
        }

        public void Clear()
        {
            UserRole = string.Empty;
            UserName = string.Empty;
            UserEmail = string.Empty;

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
