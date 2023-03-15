using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;

namespace SoftwareDesing3.Models;

public class TriviaOption : IDataErrorInfo
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

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Title must be at least 3 and maximum 30 characters long")]
    public string Title { get; set; }

    [Required] public bool IsCorrect { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
    public string TriviaQuestionId { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, IsCorrect: {IsCorrect}, TriviaQuestionId: {TriviaQuestionId}";
    }
}

//public bool IsValid
//{
//    get
//    {
//        var context = new ValidationContext(this);
//        var results = new List<ValidationResult>();
//        Validator.TryValidateObject(this, context, results, true);
//        return results.Count == 0;
//    }
//}