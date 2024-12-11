using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zira.Common;

namespace Zira.Presentation.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AssignIdentityErrors(this ModelStateDictionary modelState, IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                string key = string.Empty;

                if (error.Code.StartsWith("Password", StringComparison.OrdinalIgnoreCase))
                {
                    key = "Password";
                }

                if (Text.ResourceManager != null)
                {
                    string errorMessage = Text.ResourceManager.GetStringOrDefault($"{error.Code}ErrorMessage");
                    modelState.AddModelError(key, errorMessage);
                }
                else
                {
                    modelState.AddModelError(key, $"{error.Code}ErrorMessage");
                }
            }
        }

        public static string? GetFirstGlobalError(this ModelStateDictionary modelState)
        {
            return modelState
                .Where(x => string.IsNullOrEmpty(x.Key))
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .FirstOrDefault();
        }
    }
}