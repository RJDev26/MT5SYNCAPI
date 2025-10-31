using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class CreateUserRequestVM
    {
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? Role { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class EditUserRequestVM
    {
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? Role { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ChangePasswordRequestVM
    {
        [Required]
        [MinLength(6)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestVM
    {
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserResponseVM
    {
        public bool IsSuccess { get; set; }
        public bool IsNotFound { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = new List<string>();
        public UserDetailsVM? Data { get; set; }
    }

    public class UserDetailsVM
    {
        public string? Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
}
