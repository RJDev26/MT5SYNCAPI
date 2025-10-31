using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MobileAccounting.Entities;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserResponseVM> CreateUserAsync(CreateUserRequestVM request)
        {
            var existingByName = await _userManager.FindByNameAsync(request.UserName);
            if (existingByName != null)
            {
                return BuildFailureResponse("Username already exists.");
            }

            var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingByEmail != null)
            {
                return BuildFailureResponse("Email already exists.");
            }

            var identityUser = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                Role = request.Role,
                IsActive = request.IsActive,
                LockoutEnabled = !request.IsActive
            };

            if (!request.IsActive)
            {
                identityUser.LockoutEnd = DateTimeOffset.MaxValue;
            }

            identityUser.UserId = await GetNextUserIdAsync();

            var creationResult = await _userManager.CreateAsync(identityUser, request.Password);
            if (!creationResult.Succeeded)
            {
                return BuildFailureResponse("Unable to create user.", creationResult.Errors);
            }

            return BuildSuccessResponse("User created successfully.", identityUser);
        }

        public async Task<UserResponseVM> UpdateUserAsync(string id, EditUserRequestVM request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BuildNotFoundResponse();
            }

            if (!string.Equals(user.UserName, request.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var existingByName = await _userManager.FindByNameAsync(request.UserName);
                if (existingByName != null && existingByName.Id != user.Id)
                {
                    return BuildFailureResponse("Username already exists.");
                }
            }

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingByEmail != null && existingByEmail.Id != user.Id)
                {
                    return BuildFailureResponse("Email already exists.");
                }
            }

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.Role = request.Role;
            user.IsActive = request.IsActive;
            user.LockoutEnabled = !request.IsActive;
            user.LockoutEnd = request.IsActive ? null : DateTimeOffset.MaxValue;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BuildFailureResponse("Unable to update user.", updateResult.Errors);
            }
            return BuildSuccessResponse("User updated successfully.", user);
        }

        public async Task<UserResponseVM> ChangePasswordAsync(string id, ChangePasswordRequestVM request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BuildNotFoundResponse();
            }

            var changeResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!changeResult.Succeeded)
            {
                return BuildFailureResponse("Unable to change password.", changeResult.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(user);
            return BuildSuccessResponse("Password changed successfully.", user, includeDetails: false);
        }

        public async Task<UserResponseVM> ResetPasswordAsync(string id, ResetPasswordRequestVM request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BuildNotFoundResponse();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!resetResult.Succeeded)
            {
                return BuildFailureResponse("Unable to reset password.", resetResult.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(user);
            return BuildSuccessResponse("Password reset successfully.", user, includeDetails: false);
        }

        private async Task<int> GetNextUserIdAsync()
        {
            var maxIdentityId = 0;
            if (await _userManager.Users.AnyAsync())
            {
                maxIdentityId = await _userManager.Users.MaxAsync(u => u.UserId);
            }
            return maxIdentityId + 1;
        }

        private static UserResponseVM BuildSuccessResponse(string message, ApplicationUser user, bool includeDetails = true)
        {
            return new UserResponseVM
            {
                IsSuccess = true,
                Message = message,
                Data = includeDetails ? new UserDetailsVM
                {
                    Id = user.Id,
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive
                } : null
            };
        }

        private static UserResponseVM BuildFailureResponse(string message, IEnumerable<IdentityError>? errors = null)
        {
            return new UserResponseVM
            {
                IsSuccess = false,
                Message = message,
                Errors = errors?.Select(e => e.Description).Where(e => !string.IsNullOrWhiteSpace(e)).ToList()
                         ?? new List<string>()
            };
        }

        private static UserResponseVM BuildNotFoundResponse()
        {
            return new UserResponseVM
            {
                IsSuccess = false,
                IsNotFound = true,
                Message = "User not found."
            };
        }
    }
}
