using SoftwareDesing3.Models;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SoftwareDesing3;

/// <summary>
/// Логика взаимодействия для TriviaOptionDialogue.xaml
/// </summary>
public partial class TriviaOptionDialogue : Window
{
    public TriviaOption TriviaOption { get; set; }
    public object MyModel { get; set; }

    public TriviaOptionDialogue(TriviaOption inputObject)
    {
        InitializeComponent();
        MyModel = inputObject;

        var properties = inputObject.GetType().GetProperties();


        foreach (var property in properties)
        {
            var label = new Label { Content = property.Name };
            var control = GetControlForProperty(property, inputObject);
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(control);
            FormContainer.Children.Add(stackPanel);
        }

        //TriviaOption = triviaOption;
        //DataContext = TriviaOption;
    }

    private UIElement GetControlForProperty(PropertyInfo property, object source)
    {
        var control = new Control();

        if (property.PropertyType == typeof(bool))
        {
            control = new CheckBox();
        }
        else if (property.PropertyType == typeof(int))
        {
            control = new TextBox();

            if (property.Name == "Id")
                control.SetValue(TextBoxBase.IsReadOnlyProperty, true);
        }
        else if (property.PropertyType == typeof(string))
        {
            control = new TextBox();
        }

        control.SetBinding(TextBox.TextProperty, new Binding(property.Name)
        {
            Source = MyModel,
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });

        // Set the initial value of the control
        var initialValue = property.GetValue(source);

        control.SetValue(TextBox.TextProperty, initialValue?.ToString());

        if (property.PropertyType == typeof(bool)) control.SetValue(ToggleButton.IsCheckedProperty, initialValue);


        return control;
    }

    //private void ValidationErrorHandler(object sender, ValidationErrorEventArgs e)
    //{
    //    if (e.Action == ValidationErrorEventAction.Added)
    //    {
    //        OKButton.IsEnabled = false;
    //    }
    //    else if (e.Action == ValidationErrorEventAction.Removed && !HasValidationErrors())
    //    {
    //        OKButton.IsEnabled = true;
    //    }
    //}

    //private bool HasValidationErrors()
    //{
    //    var errors = Validation.GetErrors(this);
    //    return errors.Any(e => e.ErrorContent != null);
    //}

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        //var results = new List<ValidationResult>();
        //var context = new ValidationContext(TriviaOption);

        //bool isValid = Validator.TryValidateObject(TriviaOption, context, results, true);

        //if(isValid)
        //{
        //    DialogResult = true;
        //}
        Debug.WriteLine(MyModel);
        if (TriviaOption == null)
            DialogResult = true;
    }
}