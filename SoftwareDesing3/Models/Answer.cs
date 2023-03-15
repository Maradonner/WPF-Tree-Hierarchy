﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareDesing3.Models;

public class Answer : IDataErrorInfo
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
    [ForeignKey("ActiveTrivia")] public int ActiveTriviaId { get; set; }
    [ForeignKey("TriviaOption")] public int TriviaOptionId { get; set; }
    [ForeignKey("TriviaQuestion")] public int TriviaQuestionId { get; set; }
    public bool IsCorrect { get; set; }

    [ForeignKey("TriviaOption")] public int CorrectAnswerId { get; set; }

    public override string ToString()
    {
        return
            $"Id: {Id}, ActiveTriviaId: {ActiveTriviaId}, TriviaOptionId: {TriviaOptionId}, TriviaQuestionId: {TriviaQuestionId}, IsCorrect: {IsCorrect}, CorrectAnswerId: {CorrectAnswerId}";
    }
}