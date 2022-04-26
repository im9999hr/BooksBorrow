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
  [TestClass]
  public class BorrowedFromControllerTest
  {
    public static TestPathProvider testProvider = new TestPathProvider();
    BorrowedFromController controller;
    public void getMoqProviderController()
    {
      var mock = new Mock<ControllerContext>();
      var mockSession = new MocHttpSession();
      mock.Setup(p => p.HttpContext.Session).Returns(mockSession);
      controller = new BorrowedFromController(testProvider);
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
          Borrow_date = new DateTime(2022, 04, 25),
          Book = new BookModel
          {
            Author = "James Fenimore Cooper",
            Id = "bk500",
            Available = false,
            Description = "Very interesting book...",
            Genre = "Historical",
            Price = 200.35m,
            Publish_date = new DateTime(2020, 10, 14),
            Title = "The Last of the Mohicans"
          }
        },
        new BorrowedFromModel
        {
          Id = 2,
          Name = "John",
          Surname = "Doe",
          Tel = "+12938453002",
          Borrow_date = new DateTime(2021, 04, 21),
          Book = new BookModel
          {
            Author = "Beth Lapides",
            Id = "bk105",
            Available = false,
            Description = "How many decisions do you make each day? Ten? Twenty? Fifty?" +
            " How often do you think about the way each of those little and not-so-little decisions" +
            " reverberate through your life?" +
            " And how often do you regret a choice you made and wish you could simply choose again?",
            Genre = "Comedy",
            Price = 303.99m,
            Publish_date = new DateTime(2020, 10, 14),
            Title = "So You Need to Decide"
          }
        }
       };
    }

    public void getMoqProviderController2()
    {
      var mock = new Mock<ControllerContext>();
      var mockSession = new MocHttpSession();
      mock.Setup(p => p.HttpContext.Session).Returns(mockSession);
      controller = new BorrowedFromController(testProvider);
      controller.ControllerContext = mock.Object;
      // Session doesn't contain borrowed books
      mockSession["BorrowedBooks"] = new List<BorrowedFromModel>();
    }

    [TestMethod]
    // test sucessful selection for borrowing the book that is selected from the list - public ActionResult Borrow(string idBook)
    public void Borrow1()
    {
      // Arrange
      getMoqProviderController(); // fill Session with books

      // Act
      ViewResult result = controller.Borrow("bk107") as ViewResult;
      BorrowedFromModel bookToBorrow = (BorrowedFromModel)result.ViewData.Model;

      // Assert
      Assert.AreEqual("bk107", bookToBorrow.Book.Id);
      Assert.AreEqual("Splish Splash", bookToBorrow.Book.Title);
      Assert.AreEqual(true, bookToBorrow.Book.Available);
      Assert.AreEqual(4.95m, bookToBorrow.Book.Price);
    }

    [TestMethod]
    // test wrong idBook sent to controller - public ActionResult Borrow(string idBook)
    public void Borrow2()
    {
      // Arrange
      getMoqProviderController(); // fill Session with books

      // Act
      ViewResult result = controller.Borrow("") as ViewResult;
      string m = (string)result.ViewBag.Message;

      // Assert
      Assert.AreEqual(true, m.Contains("Error retrieving book! Description:"));

    }

    [TestMethod]
    // test successful borrowing book that is not as first borrowed (Session already contains other books) - public ActionResult Borrow(BorrowedFromModel newBorrowedFrom)
    public void Borrow3()
    {
      // Arrange
      getMoqProviderController(); // fill Session with books

      // Act
      ViewResult result = controller.Borrow(new BorrowedFromModel
        {
          Id = 3,
          Name = "Marina",
          Surname = "Lasić",
          Tel = "0981762534",
          Borrow_date = DateTime.Now,
            Book = new BookModel
            {
              Author = "Ivo Andrić",
              Id = "bk712",
              Available = true,
              Description = "Construction begins in 1566, and five years later the bridge is completed," +
              " together with a caravanserai (or han). The bridge replaces the unreliable ferry transport " +
              "that was once the only means of traversing the river and comes to represent an important " +
              "link between the Bosnia Eyalet and the rest of the Ottoman Empire.",
              Genre = "History",
              Price = 129.44m,
              Publish_date = new DateTime(1945, 03, 07),
              Title = "The Bridge on the Drina"
            }
        }) as ViewResult;
      List<BorrowedFromModel> testArray = (List<BorrowedFromModel>)controller.Session["BorrowedBooks"];

      // Assert
      Assert.AreEqual(3, testArray.Count);
      Assert.AreEqual("The Bridge on the Drina", testArray[2].Book.Title);
      Assert.AreEqual("So You Need to Decide", testArray[1].Book.Title);
      Assert.AreEqual(false, testArray[2].Book.Available);
    }

    [TestMethod]
    // test sucessful borrowing first book (test result with including first book in Session) - public ActionResult Borrow(BorrowedFromModel newBorrowedFrom)
    public void Borrow4()
    {
      // Arrange
      getMoqProviderController2(); // this ensure empty Session

      // Act
      ViewResult result = controller.Borrow(new BorrowedFromModel
      {
        Id = 3,
        Name = "Marina",
        Surname = "Lasić",
        Tel = "0981762534",
        Borrow_date = DateTime.Now,
        Book = new BookModel
        {
          Author = "Ivo Andrić",
          Id = "bk712",
          Available = true,
          Description = "Construction begins in 1566, and five years later the bridge is completed," +
              " together with a caravanserai (or han). The bridge replaces the unreliable ferry transport " +
              "that was once the only means of traversing the river and comes to represent an important " +
              "link between the Bosnia Eyalet and the rest of the Ottoman Empire.",
          Genre = "History",
          Price = 129.44m,
          Publish_date = new DateTime(1945, 03, 07),
          Title = "The Bridge on the Drina"
        }
      }) as ViewResult;
      List<BorrowedFromModel> testArray = (List<BorrowedFromModel>)controller.Session["BorrowedBooks"];

      // Assert
      Assert.AreEqual(1, testArray.Count);
      Assert.AreEqual("The Bridge on the Drina", testArray[0].Book.Title);
      Assert.AreEqual(false, testArray[0].Book.Available);
    }

    [TestMethod]
    // test sucessful selection for Return the book that is selected from the list - public ActionResult Return(string idBook)
    public void Return1()
    {
      // Arrange
      getMoqProviderController(); // fill Session with books

      // Act
      ViewResult result = controller.Return("bk500") as ViewResult;
      BorrowedFromModel bookToReturn = (BorrowedFromModel)result.ViewData.Model;

      // Assert
      Assert.AreEqual("bk500", bookToReturn.Book.Id);
      Assert.AreEqual("The Last of the Mohicans", bookToReturn.Book.Title);
      Assert.AreEqual(false, bookToReturn.Book.Available);
      Assert.AreEqual(200.35m, bookToReturn.Book.Price);
    }

    [TestMethod]
    // test wrong idBook sent to controller - public ActionResult Return(string idBook)
    public void Return2()
    {
      // Arrange
      getMoqProviderController(); // fill Session with books

      // Act
      var result = (RedirectToRouteResult)controller.Return("aaa");

      // Assert
      Assert.AreEqual("List", result.RouteValues["action"]);
      Assert.AreEqual("Book", result.RouteValues["controller"]);
    }

    [TestMethod]
    // test error scenario when returning the book not included in Session - public ActionResult Return(BorrowedFromModel newBorrowedFrom)
    public void Return3()
    {
      // Arrange
      getMoqProviderController2(); // this ensure empty Session

      // Act
      var result = (RedirectToRouteResult)controller.Return(new BorrowedFromModel
      {
        Id = 3,
        Name = "Marina",
        Surname = "Lasić",
        Tel = "0981762534",
        Borrow_date = DateTime.Now,
        Book = new BookModel
        {
          Author = "Ivo Andrić",
          Id = "bk712",
          Available = true,
          Description = "Construction begins in 1566, and five years later the bridge is completed," +
              " together with a caravanserai (or han). The bridge replaces the unreliable ferry transport " +
              "that was once the only means of traversing the river and comes to represent an important " +
              "link between the Bosnia Eyalet and the rest of the Ottoman Empire.",
          Genre = "History",
          Price = 129.44m,
          Publish_date = new DateTime(1945, 03, 07),
          Title = "The Bridge on the Drina"
        }
      });
      List<BorrowedFromModel> testArray = (List<BorrowedFromModel>)controller.Session["BorrowedBooks"];

      // Assert
      Assert.AreEqual(0, testArray.Count);
      Assert.AreEqual("List", result.RouteValues["action"]);
      Assert.AreEqual("Book", result.RouteValues["controller"]);
    }

    [TestMethod]
    // test error scenario when returning the book not included in Session that is not empty - public ActionResult Return(BorrowedFromModel newBorrowedFrom)
    public void Return4()
    {
      // Arrange
      getMoqProviderController(); // fill Session with books

      // Act
      var result = (RedirectToRouteResult)controller.Return(new BorrowedFromModel
      {
        Id = 3,
        Name = "Marina",
        Surname = "Lasić",
        Tel = "0981762534",
        Borrow_date = DateTime.Now,
        Book = new BookModel
        {
          Author = "Ivo Andrić",
          Id = "bk712",
          Available = true,
          Description = "Construction begins in 1566, and five years later the bridge is completed," +
              " together with a caravanserai (or han). The bridge replaces the unreliable ferry transport " +
              "that was once the only means of traversing the river and comes to represent an important " +
              "link between the Bosnia Eyalet and the rest of the Ottoman Empire.",
          Genre = "History",
          Price = 129.44m,
          Publish_date = new DateTime(1945, 03, 07),
          Title = "The Bridge on the Drina"
        }
      });
      List<BorrowedFromModel> testArray = (List<BorrowedFromModel>)controller.Session["BorrowedBooks"];

      // Assert
      Assert.AreEqual(2, testArray.Count);
      Assert.AreEqual("The Last of the Mohicans", testArray[0].Book.Title);
      Assert.AreEqual(false, testArray[1].Book.Available);
    }

    [TestMethod]
    // test successful returning the book included in Session - public ActionResult Return(BorrowedFromModel newBorrowedFrom)
    public void Return5()
    {
      // Arrange
      getMoqProviderController();

      // Act
      ViewResult result = controller.Return(new BorrowedFromModel
      {
        Id = 2,
        Name = "John",
        Surname = "Doe",
        Tel = "+12938453002",
        Borrow_date = new DateTime(2021, 04, 21),
        Book = new BookModel
        {
          Author = "Beth Lapides",
          Id = "bk105",
          Available = false,
          Description = "How many decisions do you make each day? Ten? Twenty? Fifty?" +
            " How often do you think about the way each of those little and not-so-little decisions" +
            " reverberate through your life?" +
            " And how often do you regret a choice you made and wish you could simply choose again?",
          Genre = "Comedy",
          Price = 303.99m,
          Publish_date = new DateTime(2020, 10, 14),
          Title = "So You Need to Decide"
        }
      }) as ViewResult;
      List<BorrowedFromModel> testArray = (List<BorrowedFromModel>)controller.Session["BorrowedBooks"];

      // Assert
      Assert.AreEqual(1, testArray.Count);
      Assert.AreEqual("The Last of the Mohicans", testArray[0].Book.Title); //book that remains as borrowed
      Assert.AreEqual(false, testArray[0].Book.Available);
    }
  }
}
