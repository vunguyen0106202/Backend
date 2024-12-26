namespace API.Dtos
{
    public class ResetPasswordModel
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class ResetPassword
    {
        public string UserName { get; set; }
        public string NewPass { get; set; }
        public string ConfirmPass { get; set; }
    }

}
