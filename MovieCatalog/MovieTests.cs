


using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Text;

namespace MovieCatalog
{
    [TestFixture]

    public class MovieTests
    {
        private readonly static string BaseUrl = "https://d24hkho2ozf732.cloudfront.net/";
        protected IWebDriver driver;
        private Actions actions;
        string lastCreatedMovieTitle = "";
        string lastCreatedMovieDescription = "";

        [OneTimeSetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver();
            actions = new Actions(driver);
            driver.Navigate().GoToUrl(BaseUrl);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl($"{BaseUrl}User/Login");
            var loginForm = driver.FindElement(By.XPath("//form[@method='post']"));
            actions.ScrollToElement(loginForm).Perform();

            //Log in to the application

            driver.Navigate().GoToUrl($"{BaseUrl}User/Login");
            driver.FindElement(By.XPath("//input[@id='form2Example17']")).SendKeys("Examtest@gmail.com");
            driver.FindElement(By.XPath("//input[@id='form2Example27']")).SendKeys("Examtest");

            var loginButton = driver.FindElement(By.XPath("//button[text()='Login']"));
            actions.ScrollToElement(loginButton).Perform();
            loginButton.Click();


        }

        [Test, Order(1)]
        public void AddMovieWithoutTitleTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/Add#add");

            var addMovie = driver.FindElement(By.XPath("//div[@class='card-body p-4 p-lg-5 text-black']"));
            actions.ScrollToElement(addMovie).Perform();
          
            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            titleInput.SendKeys("");

            var addButton = driver.FindElement(By.XPath("//button[text()='Add']"));
            actions.ScrollToElement(addButton).Perform();
            addButton.Click();

            var errorMessage = driver.FindElement(By.XPath("//div[@class='toast-message']"));
            Assert.That(errorMessage.Text, Is.EqualTo("The Title field is required."));

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}Catalog/Add"));
                     

        }

        [Test, Order(2)]
        public void AddMovieWithoutDescriptionTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/Add#add");

            var addMovie = driver.FindElement(By.XPath("//div[@class='card-body p-4 p-lg-5 text-black']"));
            actions.ScrollToElement(addMovie).Perform();

            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            titleInput.SendKeys(lastCreatedMovieTitle);

            var descriptionInput = driver.FindElement(By.XPath("//textarea[@name='Description']"));
            actions.ScrollToElement(descriptionInput).Perform();
            descriptionInput.Clear();
            descriptionInput.SendKeys("");

            var addButton = driver.FindElement(By.XPath("//button[text()='Add']"));
            actions.ScrollToElement(addButton).Perform();
            addButton.Click();

            var errorMessage = driver.FindElement(By.XPath("//div[@class='toast-message']"));
            Assert.That(errorMessage.Text, Is.EqualTo("The Description field is required."));

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}Catalog/Add"));

        }
        [Test, Order(3)]
        public void AddMovieWithRandomTitleTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/Add#add");

            var addMovie = driver.FindElement(By.XPath("//div[@class='card-body p-4 p-lg-5 text-black']"));
            actions.ScrollToElement(addMovie).Perform();

            lastCreatedMovieTitle = GenerateRandomString(10);
            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            titleInput.SendKeys(lastCreatedMovieTitle);

            lastCreatedMovieDescription = GenerateRandomString(20);
            var descriptionInput = driver.FindElement(By.XPath("//textarea[@name='Description']"));
            actions.ScrollToElement(descriptionInput).Perform();
            descriptionInput.Clear();
            descriptionInput.SendKeys(lastCreatedMovieDescription);

            var addButton = driver.FindElement(By.XPath("//button[text()='Add']"));
            actions.ScrollToElement(addButton).Perform();
            addButton.Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}Catalog/All#all"));

            var paginationItems = driver.FindElements(By.CssSelector("ul.pagination li.page-item"));
            var lastPageItem = paginationItems.Last();
            actions.MoveToElement(lastPageItem).Perform();

            var lastPageLink = lastPageItem.FindElement(By.CssSelector("a.page-link"));
            lastPageLink.Click();

            var movies = driver.FindElements(By.CssSelector(".col-lg-4"));
            var lastMovieElement = movies.Last();
            var lastMovieElementTitle = lastMovieElement.FindElement(By.CssSelector("h2"));

            string actualMovieTitle = lastMovieElementTitle.Text.Trim();
            Assert.That(actualMovieTitle, Is.EqualTo(actualMovieTitle), "The last movie title does not match the expected value.");

        }
        [Test, Order(4)]
        public void EditLastMovieTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/All#all");

            var paginationItems = driver.FindElements(By.CssSelector("ul.pagination li.page-item"));
            var lastPageItem = paginationItems.Last();
            actions.MoveToElement(lastPageItem).Perform();

            var lastPageLink = lastPageItem.FindElement(By.CssSelector("a.page-link"));
            lastPageLink.Click();

           
            var lastEditButton = driver.FindElement(By.XPath("(//a[text()='Edit'])[last()]"));
            actions.ScrollToElement(lastEditButton).Perform();    
            lastEditButton.Click();

            var editForm = driver.FindElement(By.CssSelector(".card-body"));
            actions.ScrollToElement(editForm).Perform();

            var titleInput = driver.FindElement(By.XPath("//input[@name='Title']"));
            titleInput.Clear();
            lastCreatedMovieTitle = "UPDATED_" + lastCreatedMovieTitle;
            titleInput.SendKeys(lastCreatedMovieTitle);

            var editButtonForm = driver.FindElement(By.XPath("//button[@type='submit' and text()='Edit']"));
            actions.ScrollToElement(editButtonForm).Perform();
            editButtonForm.Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}Catalog/All#all"));

            var movies = driver.FindElements(By.CssSelector(".col-lg-4"));
            var lastMovieElement = movies.Last();
            var lastMovieElementTitle = lastMovieElement.FindElement(By.CssSelector("h2"));

            string actualMovieTitle = lastMovieElementTitle.Text.Trim();
            Assert.That(actualMovieTitle, Is.EqualTo(lastCreatedMovieTitle), "The last movie title does not match the expected value.");
           

        }
        [Test, Order(5)]
        public void MarkLastAddedMovieAsWatchedTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/All#all");

            var paginationItems = driver.FindElements(By.CssSelector("ul.pagination li.page-item"));
            var lastPageItem = paginationItems.Last();
            actions.MoveToElement(lastPageItem).Perform();

            var lastPageLink = lastPageItem.FindElement(By.CssSelector("a.page-link"));
            lastPageLink.Click();


            var watchedButton = driver.FindElement(By.XPath("(//a[text()='Mark as Watched'])[last()]"));
            actions.ScrollToElement(watchedButton).Perform();
            watchedButton.Click();

            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/Watched#watched");
            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}Catalog/Watched#watched"));


            var lastWatchedMovie = driver.FindElement(By.XPath("(//div[@class='col-lg-4']//h2)[last()]")).Text;
            Assert.That(lastWatchedMovie, Is.EqualTo(lastCreatedMovieTitle));

           
        }
        [Test, Order(6)]
        public void DeleteLastAddedMovieTest()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/All#all");

            var paginationItems = driver.FindElements(By.CssSelector("ul.pagination li.page-item"));
            var lastPageItem = paginationItems.Last();
            actions.MoveToElement(lastPageItem).Perform();

            var lastPageLink = lastPageItem.FindElement(By.CssSelector("a.page-link"));
            lastPageLink.Click();

            var movies = driver.FindElements(By.CssSelector(".col-lg-4"));
            var lastMovieElement = movies.Last();
            var lastMovieElementTitle = lastMovieElement.FindElement(By.CssSelector("h2"));

            driver.FindElement(By.XPath("(//a[text()='Delete'])[last()]")).Click();
           
            var deleteMovieButton = driver.FindElement(By.XPath("//button[@type='submit'and text()='Yes']"));
            deleteMovieButton.Click();
            var message = driver.FindElement(By.XPath("//div[@class='toast-message']"));

            Assert.That(message.Text, Is.EqualTo("The Movie is deleted successfully!"));

                                 



        }


        public static string GenerateRandomString(int length)
        {
            char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            StringBuilder result = new StringBuilder(length);

            if (length <= 0)
            {
                throw new ArgumentException("Length must be greater than zero.", nameof(length));
            }
            var random = new Random();


            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }


        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}