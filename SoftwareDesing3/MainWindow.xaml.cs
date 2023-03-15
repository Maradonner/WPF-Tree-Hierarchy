using Dapper;
using SoftwareDesing3.Models;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SoftwareDesing3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string _connectionString =
        "Data Source=(local);Initial Catalog=Quizario;Integrated Security=True";

    public MainWindow()
    {
        InitializeComponent();
        ConnectAll();
    }

    private TreeViewItem CreateTreeViewItem(string header, object tag = null)
    {
        var item = new TreeViewItem { Header = header, Tag = tag };
        item.Style = (Style)FindResource("ModernTreeViewItemStyle");
        return item;
    }

    public void ConnectAll()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var quizLevel = CreateTreeViewItem("Quizzes");
            var roleLevel = CreateTreeViewItem("Users");
            var activeTriviaLevel = CreateTreeViewItem("Activa Trivias");

            var quizzes = connection.Query<TriviaQuiz>
                ("SELECT * FROM [TriviaQuizs] tq");

            foreach (var quiz in quizzes)
            {
                var quizItem = CreateTreeViewItem(quiz.ToString(), quiz);


                var questions = connection.Query<TriviaQuestion>(
                    "SELECT * FROM TriviaQuestions WHERE TriviaQuizId = @Id",
                    new { Id = quiz.Id }).ToList();

                foreach (var question in questions)
                {
                    var questionItem = CreateTreeViewItem(question.ToString(), question);

                    var options = connection.Query<TriviaOption>(
                        "SELECT * FROM TriviaOptions WHERE TriviaQuestionId = @Id",
                        new { Id = question.Id }).ToList();

                    foreach (var option in options)
                    {
                        var optionItem = CreateTreeViewItem(option.ToString(), option);
                        questionItem.Items.Add(optionItem);
                    }

                    quizItem.Items.Add(questionItem);
                }

                quizLevel.Items.Add(quizItem);
            }

            var roles = connection.Query<Role>
                ("SELECT * FROM [Role] usr");

            foreach (var role in roles)
            {
                var roleItem = CreateTreeViewItem(role.ToString(), role);
                var users = connection.Query<User>(
                    "SELECT * FROM Users WHERE RoleId = @Id",
                    new { Id = role.Id }).ToList();

                foreach (var user in users)
                {
                    var userItem = CreateTreeViewItem(user.ToString(), user);
                    roleItem.Items.Add(userItem);
                }

                roleLevel.Items.Add(roleItem);
            }

            var activeTrivias = connection.Query<ActiveTrivia>
                ("SELECT * FROM [ActiveTrivias] act");

            foreach (var activeTrivia in activeTrivias)
            {
                var activeTriviaItem = CreateTreeViewItem(activeTrivia.ToString(), activeTrivia);

                var answers = connection.Query<Answer>(
                    "SELECT * FROM Answers WHERE ActiveTriviaId = @Id",
                    new { Id = activeTrivia.Id }).ToList();

                foreach (var answer in answers)
                {
                    var answerItem = CreateTreeViewItem(answer.ToString(), answer);
                    activeTriviaItem.Items.Add(answerItem);
                }

                activeTriviaLevel.Items.Add(activeTriviaItem);
            }

            Tree.Items.Add(quizLevel);
            Tree.Items.Add(activeTriviaLevel);
            Tree.Items.Add(roleLevel);
        }
    }


    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var selectedTreeViewItem = Tree.SelectedItem as TreeViewItem;

        if (selectedTreeViewItem == null)
            return;


        var parentItem = selectedTreeViewItem?.Parent as TreeViewItem;
        if (parentItem == null)
            return;

        var model = selectedTreeViewItem.Tag;

        if (model is TriviaQuiz quiz)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query<TriviaQuiz>(
                    "DELETE FROM TriviaQuizs WHERE Id = @Id",
                    new { Id = quiz.Id });
            }
        else if (model is TriviaQuestion question)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query<TriviaQuiz>(
                    "DELETE FROM TriviaQuestions WHERE Id = @Id",
                    new { Id = question.Id });
            }
        else if (model is TriviaOption option)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query<TriviaQuiz>(
                    "DELETE FROM TriviaOptions WHERE Id = @Id",
                    new { Id = option.Id });
            }
        else if (model is Answer answer)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query<TriviaQuiz>(
                    "DELETE FROM Answers WHERE Id = @Id",
                    new { Id = answer.Id });
            }
        else if (model is ActiveTrivia activeTrivia)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query<TriviaQuiz>(
                    "DELETE FROM ActiveTrivias WHERE Id = @Id",
                    new { Id = activeTrivia.Id });
            }
        else if (model is Role role)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query<TriviaQuiz>(
                    "DELETE FROM Role WHERE Id = @Id",
                    new { Id = role.Id });
            }

        else if (model is User user)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query<TriviaQuiz>(
                    "DELETE FROM Users WHERE Id = @Id",
                    new { Id = user.Id });
            }

        parentItem.Items.Remove(selectedTreeViewItem);
    }


    private void Create_Click(object sender, RoutedEventArgs e)
    {
        var selectedTreeViewItem = Tree.SelectedItem as TreeViewItem;

        if (selectedTreeViewItem == null)
            return;

        var model = selectedTreeViewItem.Tag;

        if (selectedTreeViewItem.Header == "Users")
        {
            var dialog = new DynamicForm(new User());

            if (dialog.ShowDialog() == true)
            {
                var user = (User)dialog.MyModel;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var userId = connection.ExecuteScalar<int>(
                        "INSERT INTO [Users] (Username, ImageUrl, PasswordHash, RefreshToken, TokenCreated, TokenExpired, RoleId) " +
                        "VALUES (@Username, @ImageUrl, @PasswordHash, @RefreshToken, @TokenCreated, @TokenExpired, @RoleId)",
                        user);

                    user.Id = userId;
                }

                var newTreeViewItem = CreateTreeViewItem(user.ToString(), user);
                selectedTreeViewItem.Items.Add(newTreeViewItem);
            }
        }

        if (selectedTreeViewItem.Header == "Quizzes")
        {
            var dialog = new DynamicForm(new TriviaQuiz());

            if (dialog.ShowDialog() == true)
            {
                var triviaQuiz = (TriviaQuiz)dialog.MyModel;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var triviaQuizId = connection.ExecuteScalar<int>(
                        "INSERT INTO [TriviaQuizs] (Title, PictureUrl, QuestionTime, AccumulateTime, LivesCount, UserId) " +
                        "VALUES (@Title, @PictureUrl, @QuestionTime, @AccumulateTime, @LivesCount, @UserId)  SELECT CAST(SCOPE_IDENTITY() AS INT)",
                        triviaQuiz);


                    triviaQuiz.Id = triviaQuizId;
                }

                var newTreeViewItem = CreateTreeViewItem(triviaQuiz.ToString(), triviaQuiz);
                selectedTreeViewItem.Items.Add(newTreeViewItem);
            }
        }

        if (model is TriviaQuiz quiz)
        {
            var dialog = new DynamicForm(new TriviaQuestion() { TriviaQuizId = quiz.Id });

            if (dialog.ShowDialog() == true)
            {
                var triviaQuestion = (TriviaQuestion)dialog.MyModel;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var optionId = connection.ExecuteScalar<int>(
                        "INSERT INTO [TriviaQuestions] (Title, PictureUrl, TriviaQuizId) " +
                        "VALUES (@Title, @PictureUrl, @TriviaQuizId) SELECT CAST(SCOPE_IDENTITY() AS INT)",
                        triviaQuestion);

                    triviaQuestion.Id = optionId;
                }

                var newTreeViewItem = CreateTreeViewItem(triviaQuestion.ToString(), triviaQuestion);
                selectedTreeViewItem.Items.Add(newTreeViewItem);
            }
        }
        else if (model is TriviaQuestion question)
        {
            var dialog = new DynamicForm(new TriviaOption() { TriviaQuestionId = question.Id.ToString() });

            if (dialog.ShowDialog() == true)
            {
                var triviaOption = (TriviaOption)dialog.MyModel;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var optionId = connection.ExecuteScalar<int>(
                        "INSERT INTO [TriviaOptions] (Title, IsCorrect, TriviaQuestionId) " +
                        "VALUES (@Title, @IsCorrect, @TriviaQuestionId) SELECT CAST(SCOPE_IDENTITY() AS INT)",
                        triviaOption);

                    triviaOption.Id = optionId;
                }

                var newTreeViewItem = CreateTreeViewItem(triviaOption.ToString(), triviaOption);
                selectedTreeViewItem.Items.Add(newTreeViewItem);
            }
        }
        else if (model is Role role)
        {
            var dialog = new DynamicForm(new User());

            if (dialog.ShowDialog() == true)
            {
                var user = (User)dialog.MyModel;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var userId = connection.ExecuteScalar<int>(
                        "INSERT INTO [Users] (Username, ImageUrl, PasswordHash, RefreshToken,  RoleId) " +
                        "VALUES (@Username, @ImageUrl, @PasswordHash, @RefreshToken,  @RoleId) SELECT CAST(SCOPE_IDENTITY() AS INT)",
                        user);

                    user.Id = userId;
                }


                var newTreeViewItem = CreateTreeViewItem(user.ToString(), user);
                selectedTreeViewItem.Items.Add(newTreeViewItem);
            }
        }
    }

    private void Update_Click(object sender, RoutedEventArgs e)
    {
        var selectedTreeViewItem = Tree.SelectedItem as TreeViewItem;

        if (selectedTreeViewItem == null)
            return;

        var model = selectedTreeViewItem.Tag;


        if (model is TriviaQuiz quiz)
        {
            var dialog = new DynamicForm(quiz);

            if (dialog.ShowDialog() == true)
            {
                var triviaQuiz = (TriviaQuiz)dialog.MyModel;
                selectedTreeViewItem.Tag = triviaQuiz;
                selectedTreeViewItem.Header = triviaQuiz.ToString();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var questions = connection.Query<TriviaQuiz>(
                        "UPDATE TriviaQuizs SET Title = @Title, PictureUrl = @PictureUrl, QuestionTime = @QuestionTime," +
                        " AccumulateTime = @AccumulateTime, LivesCount = @LivesCount, UserId = @UserId WHERE Id = @Id",
                        triviaQuiz);
                }
            }
        }
        else if (model is TriviaQuestion question)
        {
            var dialog = new DynamicForm(question);

            if (dialog.ShowDialog() == true)
            {
                var triviaQuestion = (TriviaQuestion)dialog.MyModel;
                selectedTreeViewItem.Tag = triviaQuestion;
                selectedTreeViewItem.Header = triviaQuestion.ToString();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var questions = connection.Query<TriviaQuestion>(
                        "UPDATE TriviaQuestions SET Title = @Title, PictureUrl = @PictureUrl, TriviaQuizId = @TriviaQuizId WHERE Id = @Id",
                        triviaQuestion);
                }
            }
        }
        else if (model is TriviaOption option)
        {
            var dialog = new DynamicForm(option);

            if (dialog.ShowDialog() == true)
            {
                var triviaOption = (TriviaOption)dialog.MyModel;
                selectedTreeViewItem.Tag = triviaOption;
                selectedTreeViewItem.Header = triviaOption.ToString();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var questions = connection.Query<TriviaOption>(
                        "UPDATE TriviaOptions SET Title = @Title, IsCorrect = @IsCorrect, TriviaQuestionId = @TriviaQuestionId WHERE Id = @Id",
                        triviaOption);
                }
            }
        }
        else if (model is User userModel)
        {
            var dialog = new DynamicForm(userModel);

            if (dialog.ShowDialog() == true)
            {
                var user = (User)dialog.MyModel;
                selectedTreeViewItem.Tag = user;
                selectedTreeViewItem.Header = user.ToString();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var questions = connection.Query<User>(
                        "UPDATE Users SET Username = @Username, ImageUrl = @ImageUrl, PasswordHash = @PasswordHash, RefreshToken = @RefreshToken," +
                        " TokenCreated = @TokenCreated, TokenExpired = @TokenExpired, RoleId = @RoleId WHERE Id = @Id",
                        user);
                }
            }
        }

        else if (model is Answer answerModel)
        {
            var dialog = new DynamicForm(answerModel);

            if (dialog.ShowDialog() == true)
            {
                var answer = (Answer)dialog.MyModel;
                selectedTreeViewItem.Tag = answer;
                selectedTreeViewItem.Header = answer.ToString();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var questions = connection.Query<Answer>(
                        "UPDATE Answers SET ActiveTriviaId = @ActiveTriviaId, TriviaOptionId = @TriviaOptionId, TriviaQuestionId = @TriviaQuestionId, IsCorrect = @IsCorrect," +
                        " CorrectAnswerId = @CorrectAnswerId WHERE Id = @Id",
                        answer);
                }
            }
        }

        else if (model is ActiveTrivia activeTriviaModel)
        {
            var dialog = new DynamicForm(activeTriviaModel);

            if (dialog.ShowDialog() == true)
            {
                var activeTrivia = (ActiveTrivia)dialog.MyModel;
                selectedTreeViewItem.Tag = activeTrivia;
                selectedTreeViewItem.Header = activeTrivia.ToString();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var questions = connection.Query<ActiveTrivia>(
                        "UPDATE ActiveTrivias SET StartTime = @StartTime, TriviaQuizId = @TriviaQuizId WHERE Id = @Id",
                        activeTrivia);
                }
            }
        }

        else if (model is Role roleModel)
        {
            var dialog = new DynamicForm(roleModel);

            if (dialog.ShowDialog() == true)
            {
                var role = (Role)dialog.MyModel;
                selectedTreeViewItem.Tag = role;
                selectedTreeViewItem.Header = role.ToString();

                using (var connection = new SqlConnection(_connectionString))
                {
                    var questions = connection.Query<Role>(
                        "UPDATE Role SET Name = @Name WHERE Id = @Id",
                        role);
                }
            }
        }
    }
}