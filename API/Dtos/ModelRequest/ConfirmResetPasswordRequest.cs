﻿namespace API.Dtos.ModelRequest
{
	public class ConfirmResetPasswordRequest
	{
		public string Email { get; set; }
		public string Otp { get; set; }
		public string NewPassword { get; set; }
	}
}
