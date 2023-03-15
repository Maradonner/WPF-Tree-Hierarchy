using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareDesing3.Models;

public class TriviaQuiz : IDataErrorInfo
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
    public int UserId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Title must be at least 3 and maximum 30 characters long")]
    public string Title { get; set; }

    public int QuestionTime { get; set; }
    public int LivesCount { get; set; }
    public int AccumulateTime { get; set; }
    public string PictureUrl { get; set; } = string.Empty;

    public override string ToString()
    {
        return
            $"Id: {Id}, UserId: {UserId}, Title: {Title}, QuestionTime: {QuestionTime}, LivesCount: {LivesCount}, AccumulateTime: {AccumulateTime}, PictureUrl: {PictureUrl}";
    }
}