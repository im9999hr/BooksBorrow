using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Books.Models;
using Books.Models.DAL;
using Books.Utility;

namespace Books.Controllers
{
  public class BookController : Controller
  {
    public static string filePath;
    public BookController()
    {
      PathProvider serviceProvider = new PathProvider();
      filePath = serviceProvider.MapPath("~/App_Data/books.xml");
    }
    public BookController(IPathProvider testProvider)
    {
      filePath = testProvider.MapPath("C:/TestData/books.xml");
    }

    [HttpGet]
    public ActionResult List()
    {
      
      List<BookModel> lstBooks = new List<BookModel>();
      try
      {
        lstBooks = Repository.RetrieveBooks(filePath);
        if (Session["BorrowedBooks"] != null)
        {
          List<BorrowedFromModel> borrowedBooksFromUser = (List<BorrowedFromModel>)Session["BorrowedBooks"];
          foreach (BorrowedFromModel userBook in borrowedBooksFromUser)
          {
            foreach (BookModel book in lstBooks)
            {
              if (userBook.Book.Id == book.Id)
              {
                book.Available = false;
                break;
              }
            }
          }
        }
        ViewBag.Message = "Successfully displaying book list!";
      }
      catch (Exception ex)
      {
        ViewBag.Message = "Error retrieving book list! Description: " + ex.Message;
      }
      return View(lstBooks);
    }

    [HttpGet]
    public ActionResult Details(string idBook)
    {
      BookModel book = new BookModel();
      try
      {
        book = Repository.RetrieveBook(idBook, filePath);
        if (Session["BorrowedBooks"] != null)
        {
          List<BorrowedFromModel> borrowedBooksFromUser = (List<BorrowedFromModel>)Session["BorrowedBooks"];
          foreach (BorrowedFromModel userBook in borrowedBooksFromUser)
          {
            if (userBook.Book.Id == book.Id)
            {
              book.Available = false;
              break;
            }
          }
        }
        ViewBag.Message = "Successfully displaying book!";
      }
      catch (Exception ex)
      {
        ViewBag.Message = "Error retrieving book! Description: " + ex.Message;
      }
      return View(book);
    }

  }
}