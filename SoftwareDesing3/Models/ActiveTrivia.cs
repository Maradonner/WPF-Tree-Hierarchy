using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareDesing3.Models;

public class ActiveTrivia : IDataErrorInfo
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
    public DateTime StartTime { get; set; }
    public int TriviaQuizId { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, StartTime: {StartTime}, TriviaQuizId: {TriviaQuizId}";
    }
}