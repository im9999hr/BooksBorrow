using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Books.Models;
using Books.Controllers;
using Books.Utility;
using Moq;
using System.Web;

namespace Books.Tests.Controllers
{
  public class MocHttpSession : HttpSessionStateBase
  {
    Dictionary<string, object> _sessionDictionary = new Dictionary<string, object>();
    public override object this[string name]
    {
      get { return _sessionDictionary[name]; }
      set { _sessionDictionary[name] = value; }
    }
  }

  [TestClass]
  public class BookControllerTest
  {
    public static TestPathProvider testProvider = new TestPathProvider();
    BookController controller;
    public void getMoqProviderController ()
    {
      var mock = new Mock<ControllerContext>();
      //var mockSession = new Mock<HttpSessionStateBase>();
      var mockSession = new MocHttpSession();
      //mock.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
      mock.Setup(p => p.HttpContext.Session).Returns(mockSession);
      controller = new BookController(testProvider);
      controller.ControllerContext = mock.Object;
      // fill Session with two books (this means that these books are already borrowed)
      mockSession["BorrowedBooks"] = new List<BorrowedFromModel>
      {
        new BorrowedFromModel
        {
          Id = 1,
          Name = "Ivo",
          Surname = "Biočić",
          Tel = "0916105112",
          Borrow_date = new DateTime(2022, 04, 26),
          Book = new BookModel
          {
            Author = "James Fenimore Cooper",
            Id = "bk200",
            Available = false,
            Description = "Very interesting book...",
            Genre = "Historical film",
            Price = 200,
            Publish_date = new DateTime(2020, 09, 24),
            Title = "The Last of the Mohicans"
          }
        },
        new BorrowedFromModel
        {
          Id = 2,
          Name = "John",
          Surname = "Doe",
          Tel = "+12938453002",
          Borrow_date = new DateTime(2021, 12, 02),
          Book = new BookModel
          {
            Author = "Beth Lapides",
            Id = "bk102",
            Available = false,
            Description = "How many decisions do you make each day? Ten? Twenty? Fifty?" +
            " How often do you think about the way each of those little and not-so-little decisions" +
            " reverberate through your life?" +
            " And how often do you regret a choice you made and wish you could simply choose again?",
            Genre = "Comedy",
            Price = 300.99m,
            Publish_date = new DateTime(2020, 10, 14),
            Title = "So You Need to Decide"
          }
        }
       };
      
    }
    [TestMethod]
    // test successful retrieval of all books - public ActionResult List()
    public void List1()
    {
      // Arrange
      getMoqProviderController();
      List<BorrowedFromModel> testArray = (List<BorrowedFromModel>)controller.Session["BorrowedBooks"];

      // Act
      ViewResult result = controller.List() as ViewResult;
      List<BookModel> bookList = (List<BookModel>)result.ViewData.Model;

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual("Successfully displaying book list!", result.ViewBag.Message);
      Assert.AreEqual("bk103", bookList[2].Id);
      Assert.AreEqual("The Last of the Mohicans", testArray[0].Book.Title);
      Assert.AreEqual(300.99m, testArray[1].Book.Price);

    }

    [TestMethod]
    // test correct idBook sent to controller - public ActionResult Details(string idBook)
    public void Details1()
    {
      // Arrange
      getMoqProviderController();
      List<BorrowedFromModel> testArray = (List<BorrowedFromModel>)controller.Session["BorrowedBooks"];

      // Act
      ViewResult result = controller.Details("bk102") as ViewResult;
      BookModel book = (BookModel)result.ViewData.Model;

      // Assert
      Assert.AreEqual("Successfully displaying book!", result.ViewBag.Message);
      Assert.AreEqual("bk102", book.Id);
      Assert.AreEqual("Midnight Rain", book.Title);
      Assert.AreEqual(false, book.Available);
      Assert.AreEqual(200, testArray[0].Book.Price);
      Assert.AreEqual(5.95m, book.Price);

    }

    [TestMethod]
    // test wrong idBook sent to controller - public ActionResult Details(string idBook)
    public void Details2()
    {
      // Arrange
      getMoqProviderController();

      // Act
      ViewResult result = controller.Details("") as ViewResult;
      string m = (string)result.ViewBag.Message;

      // Assert
      Assert.AreEqual(true, m.Contains("Error retrieving book! Description:"));

    }
  }
}
