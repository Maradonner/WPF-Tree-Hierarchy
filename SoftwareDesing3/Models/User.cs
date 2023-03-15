using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SoftwareDesing3.Models;

public class User : IDataErrorInfo
{
    public string Error => null;

    public string this[string columnName]
    {
        get
        {
            var context = new ValidationContext(this) { MemberName = columnName };
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(
                GetType().GetProperty(columnName).GetValue(this),
                context,
                results);

            if (results.Any())
                return results.First().ErrorMessage;

            return null;
        }
    }

    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = new byte[32];
    public byte[] PasswordSalt { get; set; } = new byte[32];
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpired { get; set; }
    public int? RoleId { get; set; } = 1;

    public override string ToString()
    {
        return $"Id: {Id}, Username: {Username}, ImageUrl: {ImageUrl}, RoleId: {RoleId}";
    }
}