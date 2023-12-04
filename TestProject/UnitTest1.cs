using System.Numerics;
using dotnetapp.Controllers;
using dotnetapp.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Net;

namespace TestProject
{
    public class Tests
    {
        private ApplicationDbContext _context;

        private HttpClient _httpClient;


        [SetUp]
        public void SetUp()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8080/");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Test_GetMoviews_EndPoint_Status()
        {
            // Send an HTTP GET request to the API endpoint.
            HttpResponseMessage response = await _httpClient.GetAsync("api/movies");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(responseBody);
        }

        [Test]
        public void Test_Migration_Exists()
        {
            string folderPath = @"/home/coder/project/workspace/dotnetapp/Migrations/";
            bool folderExists = Directory.Exists(folderPath);

            Assert.IsTrue(folderExists, "The folder does not exist.");

            if (folderExists)
            {
                string[] files = Directory.GetFiles(folderPath);
                Assert.IsTrue(files.Length > 0, "No files found in the folder.");
            }
        }

        [Test]
        public void GetReviews_MovieController_Returns_By_MovieID()
        {
            string assemblyName = "dotnetapp";
            Assembly assembly = Assembly.Load(assemblyName);
            string modelType = "dotnetapp.Models.Review";
            string controllerTypeName = "dotnetapp.Controllers.ReviewController";
            Type controllerType = assembly.GetType(controllerTypeName);
            Type modelTypeName = assembly.GetType(modelType);

            MethodInfo method = controllerType.GetMethod("GetReviews", new Type[] { typeof(int) });

            if (method != null)
            {
                var controller = Activator.CreateInstance(controllerType, _context);
                var result = method.Invoke(controller, new object[] { 10 });

                Assert.IsNotNull(result);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void SearchMovies_MovieController_Returns_By_Genre()
        {
            string assemblyName = "dotnetapp";
            Assembly assembly = Assembly.Load(assemblyName);
            string modelType = "dotnetapp.Models.Movie";
            string controllerTypeName = "dotnetapp.Controllers.MovieController";
            Type controllerType = assembly.GetType(controllerTypeName);
            Type modelTypeName = assembly.GetType(modelType);

            MethodInfo method = controllerType.GetMethod("SearchMovies", new Type[] {typeof(string)});

            if (method != null)
            {
                var controller = Activator.CreateInstance(controllerType, _context);
                var result = method.Invoke(controller, new object[] {"demo"});
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void AddMovie_MovieControllerr_AddsTo_DB()
        {
            string assemblyName = "dotnetapp";
            Assembly assembly = Assembly.Load(assemblyName);
            string controllerTypeName = "dotnetapp.Controllers.MovieController";
            string typeName = "dotnetapp.Models.Movie";

            Type controllerType = assembly.GetType(controllerTypeName);
            Type controllerType2 = assembly.GetType(typeName);

            MethodInfo method = controllerType.GetMethod("AddMovie", new[] { controllerType2 });

            if (method != null)
            {

                var teamData = new Dictionary<string, object>
                    {
                        { "Movie", "demo1" },
                        { "Title", "demo2" },
                        { "Description", "demo" },
                        { "ReleaseDate", new DateTime(2023, 8, 22) },
                        { "Genre", "demo" }
                    };
                var team = Activator.CreateInstance(controllerType2);
                foreach (var kvp in teamData)
                {
                    var propertyInfo = controllerType2.GetProperty(kvp.Key);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(team, kvp.Value);
                    }
                }
                var controller = Activator.CreateInstance(controllerType, _context);
                var result = method.Invoke(controller, new object[] { team });
                Assert.IsNotNull(result);
                Type CourseType = assembly.GetType(typeName);

                PropertyInfo propertyInfo1 = CourseType.GetProperty("MovieID");
                Type contextType = assembly.GetTypes().FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t));
                var propertyInfo2 = contextType.GetProperty("Movies");
                var courses = propertyInfo2.GetValue(_context, null);
                if (courses is IEnumerable<Movie> courseList)
                {
                    var res = courseList.AsEnumerable().FirstOrDefault(r => (int)propertyInfo1.GetValue(r) == 1);
                    if (res != null)
                    {
                        Console.WriteLine(res.ToString());
                        Assert.IsNotNull(res);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void AddReview_ReviewControllerr_AddsTo_DB_MovieID()
        {
            string assemblyName = "dotnetapp";
            Assembly assembly = Assembly.Load(assemblyName);
            string controllerTypeName = "dotnetapp.Controllers.ReviewController";
            string typeName = "dotnetapp.Models.Review";

            Type controllerType = assembly.GetType(controllerTypeName);
            Type controllerType2 = assembly.GetType(typeName);

            MethodInfo method = controllerType.GetMethod("AddReview", new[] { typeof(int) ,controllerType2 });

            if (method != null)
            {

                var teamData = new Dictionary<string, object>
                    {
                        { "Rating", 5 },
                        { "Comment", "demo2" }
                    };
                var team = Activator.CreateInstance(controllerType2);
                foreach (var kvp in teamData)
                {
                    var propertyInfo = controllerType2.GetProperty(kvp.Key);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(team, kvp.Value);
                    }
                }
                var controller = Activator.CreateInstance(controllerType, _context);
                var result = method.Invoke(controller, new object[] { 1, team });
                Assert.IsNotNull(result);
                Type CourseType = assembly.GetType(typeName);

                PropertyInfo propertyInfo1 = CourseType.GetProperty("ReviewID");
                Type contextType = assembly.GetTypes().FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t));
                var propertyInfo2 = contextType.GetProperty("Reviews");
                var prop1 = propertyInfo2.Name;
                var courses = propertyInfo2.GetValue(_context, null);
                if (courses is IEnumerable<Review> courseList)
                {
                    var res = courseList.AsEnumerable().FirstOrDefault(r => (int)propertyInfo1.GetValue(r) == 1);
                    if (res != null)
                    {
                        Console.WriteLine(res.ToString());
                        Assert.IsNotNull(res);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetMovies_MovieController_Returns()
        {
            string assemblyName = "dotnetapp";
            Assembly assembly = Assembly.Load(assemblyName);
            string modelType = "dotnetapp.Models.Movie";
            string controllerTypeName = "dotnetapp.Controllers.MovieController";
            Type controllerType = assembly.GetType(controllerTypeName);
            Type modelTypeName = assembly.GetType(modelType);

            MethodInfo method = controllerType.GetMethod("GetMovies");

            if (method != null)
            {
                var controller = Activator.CreateInstance(controllerType, _context);
                var result = method.Invoke(controller, null);
                Assert.IsNotNull(result);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetMovie_MovieController_Returns_By_ID()
        {
            string assemblyName = "dotnetapp";
            Assembly assembly = Assembly.Load(assemblyName);
            string modelType = "dotnetapp.Models.Movie";
            string controllerTypeName = "dotnetapp.Controllers.MovieController";
            Type controllerType = assembly.GetType(controllerTypeName);
            Type modelTypeName = assembly.GetType(modelType);
            MethodInfo method = controllerType.GetMethod("GetMovie", new Type[] {typeof(int)});
            if (method != null)
            {
                //var teamData = new Dictionary<string, object>
                //    {
                //        { "Movie", "demo1" },
                //        { "Title", "demo2" },
                //        { "Description", "demo" },
                //        { "ReleaseDate", new DateTime(2023, 8, 22) },
                //        { "Genre", "demo" }
                //    };
                //var team = Activator.CreateInstance(modelTypeName);
                //foreach (var kvp in teamData)
                //{
                //    var propertyInfo = modelTypeName.GetProperty(kvp.Key);
                //    if (propertyInfo != null)
                //    {
                //        propertyInfo.SetValue(team, kvp.Value);
                //    }
                //}
                //_context.Movies.Add((Movie)team);
                //_context.SaveChanges();
                var controller = Activator.CreateInstance(controllerType, _context);
                var result = method.Invoke(controller, new object[] {1});
                Assert.NotNull(result);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ApplicationDbContext_ContainsDbSet_Movie()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(ApplicationDbContext));
            Type contextType = assembly.GetTypes().FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t));
            if (contextType == null)
            {
                Assert.Fail("No DbContext found in the assembly");
                return;
            }
            Type MovieType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Movie");
            if (MovieType == null)
            {
                Assert.Fail("No DbSet found in the DbContext");
                return;
            }
            var propertyInfo = contextType.GetProperty("Movies");
            if (propertyInfo == null)
            {
                Assert.Fail("Movies property not found in the DbContext");
                return;
            }
            else
            {
                Assert.AreEqual(typeof(DbSet<>).MakeGenericType(MovieType), propertyInfo.PropertyType);
            }
        }



        [Test]
        public void ApplicationDbContext_ContainsDbSet_Review()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(ApplicationDbContext));
            Type contextType = assembly.GetTypes().FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t));
            if (contextType == null)
            {
                Assert.Fail("No DbContext found in the assembly");
                return;
            }
            Type ReviewType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Review");
            if (ReviewType == null)
            {
                Assert.Fail("No DbSet found in the DbContext");
                return;
            }
            var propertyInfo = contextType.GetProperty("Reviews");
            if (propertyInfo == null)
            {
                Assert.Fail("Reviews property not found in the DbContext");
                return;
            }
            else
            {
                Assert.AreEqual(typeof(DbSet<>).MakeGenericType(ReviewType), propertyInfo.PropertyType);
            }
        }

        [Test]
        public void Movie_Models_ClassExists()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Movie";
            Assembly assembly = Assembly.Load(assemblyName);
            Type MovieType = assembly.GetType(typeName);
            Assert.IsNotNull(MovieType);
        }

        [Test]
        public void Movie_MovieID_PropertyExists_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Movie";
            Assembly assembly = Assembly.Load(assemblyName);
            Type MovieType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = MovieType.GetProperty("MovieID");
            Assert.IsNotNull(propertyInfo, "Property MovieID does not exist in Movie class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), expectedType, "Property MovieID in Movie class is not of type int");
        }

        [Test]
        public void Movie_Title_PropertyExists_ReturnExpectedDataTypes_string()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Movie";
            Assembly assembly = Assembly.Load(assemblyName);
            Type MovieType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = MovieType.GetProperty("Title");
            Assert.IsNotNull(propertyInfo, "Property Title does not exist in Movie class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), expectedType, "Property Title in Movie class is not of type string");
        }

        [Test]
        public void Movie_Description_PropertyExists_ReturnExpectedDataTypes_string()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Movie";
            Assembly assembly = Assembly.Load(assemblyName);
            Type MovieType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = MovieType.GetProperty("Description");
            Assert.IsNotNull(propertyInfo, "Property Description does not exist in Movie class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), expectedType, "Property Description in Movie class is not of type string");
        }

        [Test]
        public void Movie_ReleaseDate_PropertyExists_ReturnExpectedDataTypes_DateTime()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Movie";
            Assembly assembly = Assembly.Load(assemblyName);
            Type MovieType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = MovieType.GetProperty("ReleaseDate");
            Assert.IsNotNull(propertyInfo, "Property ReleaseDate does not exist in Movie class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(DateTime), expectedType, "Property ReleaseDate in Movie class is not of type DateTime");
        }

        [Test]
        public void Movie_Genre_PropertyExists_ReturnExpectedDataTypes_string()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Movie";
            Assembly assembly = Assembly.Load(assemblyName);
            Type MovieType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = MovieType.GetProperty("Genre");
            Assert.IsNotNull(propertyInfo, "Property Genre does not exist in Movie class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), expectedType, "Property Genre in Movie class is not of type string");
        }

        [Test]
        public void MovieController_Controllers_ClassExists()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Controllers.MovieController";
            Assembly assembly = Assembly.Load(assemblyName);
            Type MovieControllerType = assembly.GetType(typeName);
            Assert.IsNotNull(MovieControllerType);
        }

        [Test]
        public void Review_Models_ClassExists()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Review";
            Assembly assembly = Assembly.Load(assemblyName);
            Type ReviewType = assembly.GetType(typeName);
            Assert.IsNotNull(ReviewType);
        }

        [Test]
        public void Review_ReviewID_PropertyExists_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Review";
            Assembly assembly = Assembly.Load(assemblyName);
            Type ReviewType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = ReviewType.GetProperty("ReviewID");
            Assert.IsNotNull(propertyInfo, "Property ReviewID does not exist in Review class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), expectedType, "Property ReviewID in Review class is not of type int");
        }

        [Test]
        public void Review_MovieID_PropertyExists_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Review";
            Assembly assembly = Assembly.Load(assemblyName);
            Type ReviewType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = ReviewType.GetProperty("MovieID");
            Assert.IsNotNull(propertyInfo, "Property MovieID does not exist in Review class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), expectedType, "Property MovieID in Review class is not of type int");
        }

        [Test]
        public void Review_Rating_PropertyExists_ReturnExpectedDataTypes_int()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Review";
            Assembly assembly = Assembly.Load(assemblyName);
            Type ReviewType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = ReviewType.GetProperty("Rating");
            Assert.IsNotNull(propertyInfo, "Property Rating does not exist in Review class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(int), expectedType, "Property Rating in Review class is not of type int");
        }

        [Test]
        public void Review_Comment_PropertyExists_ReturnExpectedDataTypes_string()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.Review";
            Assembly assembly = Assembly.Load(assemblyName);
            Type ReviewType = assembly.GetType(typeName);
            PropertyInfo propertyInfo = ReviewType.GetProperty("Comment");
            Assert.IsNotNull(propertyInfo, "Property Comment does not exist in Review class");
            Type expectedType = propertyInfo.PropertyType;
            Assert.AreEqual(typeof(string), expectedType, "Property Comment in Review class is not of type string");
        }

        [Test]
        public void ReviewController_Controllers_ClassExists()
        {
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Controllers.ReviewController";
            Assembly assembly = Assembly.Load(assemblyName);
            Type ReviewControllerType = assembly.GetType(typeName);
            Assert.IsNotNull(ReviewControllerType);
        }

    }
}
