using SoftwareDesing3.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace SoftwareDesing3;

/// <summary>
/// Логика взаимодействия для DynamicForm.xaml
/// </summary>
public partial class DynamicForm : Window
{
    public TriviaOption TriviaOption { get; set; }
    public object MyModel { get; set; }

    public DynamicForm(object inputObject)
    {
        InitializeComponent();
        MyModel = inputObject;

        var properties = inputObject.GetType().GetProperties()
            .Where(p => p.Name != "Error" && p.Name != "Item" && p.Name != "Password");

        foreach (var property in properties)
        {
            var label = new Label { Content = property.Name };
            var control = GetControlForProperty(property, inputObject);
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(control);
            FormContainer.Children.Add(stackPanel);
        }
    }

    private UIElement GetControlForProperty(PropertyInfo property, object source)
    {
        var initialValue = property.GetValue(source);

        var control = new Control();

        if (property.PropertyType == typeof(bool))
        {
            control = new CheckBox();
            control.Style = (Style)FindResource("ModernCheckBox");
            control.SetValue(ToggleButton.IsCheckedProperty, initialValue);
        }
        else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(string))
        {
            control = new TextBox();
            control.Style = (Style)FindResource("ModernTextBoxStyle");
            control.SetValue(TextBox.TextProperty, initialValue?.ToString());
        }
        else if (property.PropertyType == typeof(DateTime))
        {
            control = new DatePicker();
            //control.Style = (Style)FindResource("ModernDatePickerStyle");
            control.SetValue(DatePicker.SelectedDateProperty, initialValue);
        }

        control.SetBinding(TextBox.TextProperty, new Binding(property.Name)
        {
            Source = MyModel,
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            ValidatesOnDataErrors = true,
            ValidationRules = { new MyValidationRule() }
        });

        if (property.Name.Contains("Id"))
            control.SetValue(TextBoxBase.IsReadOnlyProperty, true);

        control.SetValue(Validation.ErrorTemplateProperty,
            (ControlTemplate)FindResource("ValidationErrorTemplate"));

        return control;
    }


    private bool ValidationResult()
    {
        var context = new ValidationContext(MyModel);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        Validator.TryValidateObject(MyModel, context, results, true);
        return results.Count == 0;
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine(MyModel);
        if (ValidationResult())
            DialogResult = true;
        else
            MessageBox.Show("Invalid Form!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

public class MyValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return new ValidationResult(false, "Value cannot be empty.");

        return ValidationResult.ValidResult;
    }
}