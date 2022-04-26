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
  public class BorrowedFromController : Controller
  {
    public static string filePath;

    public BorrowedFromController()
    {
      PathProvider serviceProvider = new PathProvider();
      filePath = serviceProvider.MapPath("~/App_Data/books.xml");
    }

    public BorrowedFromController(IPathProvider testProvider)
    {
      filePath = testProvider.MapPath("C:/TestData/books.xml");
    }

    [HttpGet]
    public ActionResult Borrow(string idBook)
    {
      BookModel book = new BookModel();
      BorrowedFromModel borrowedFrom = new BorrowedFromModel();
      try
      {
        book = Repository.RetrieveBook(idBook, filePath);
      }
      catch (Exception ex)
      {
        ViewBag.Message = "Error retrieving book! Description: " + ex.Message;
      }
      borrowedFrom.Book = book;
      borrowedFrom.Borrow_date = DateTime.Now;
      return View(borrowedFrom);
    }

    [HttpPost]
    public ActionResult Borrow(BorrowedFromModel newBorrowedFrom)
    {
      try
      {
        if (ModelState.IsValid)
        {
          if (Session["BorrowedBooks"] != null)
          {
            List<BorrowedFromModel> borrowedBooksFromUser = (List<BorrowedFromModel>)Session["BorrowedBooks"];
            newBorrowedFrom.Book.Available = false;
            borrowedBooksFromUser.Add(newBorrowedFrom);
            Session["BorrowedBooks"] = borrowedBooksFromUser;
          }
          else
          {
            List<BorrowedFromModel> borrowedBooksFromUser = new List<BorrowedFromModel>();
            newBorrowedFrom.Book.Available = false;
            borrowedBooksFromUser.Add(newBorrowedFrom);
            Session["BorrowedBooks"] = borrowedBooksFromUser;
          }
          TempData["Message"] = "Book \"" + newBorrowedFrom.Book.Title + "\" successfully borrowed!";
          return RedirectToAction("List", "Book");
        }
        else
        {
          ViewBag.Message = "Error while validating input data!";
          return View(newBorrowedFrom);
        }
      }
      catch (Exception e)
      {
        ViewBag.Message = "Error happen: " + e.Message;
        return View(newBorrowedFrom);
      }
    }

    [HttpGet]
    public ActionResult Return(string idBook)
    {
      BookModel book = new BookModel();
      try
      {
        book = Repository.RetrieveBook(idBook, filePath);
      }
      catch (Exception ex)
      {
        ViewBag.Message = "Error retrieving book! Description: " + ex.Message;
      }
      if (Session["BorrowedBooks"] != null)
      {
        List<BorrowedFromModel> borrowedBooksFromUser = (List<BorrowedFromModel>)Session["BorrowedBooks"];
        foreach (BorrowedFromModel userBook in borrowedBooksFromUser)
        {
          if (userBook.Book.Id == idBook)
          {
            return View(userBook);
          }
        }
      }
      TempData["Message"] = "Error: book \"" + book.Title + "\" can not be successfully returned!";
      return RedirectToAction("List", "Book");
    }

    [HttpPost]
    public ActionResult Return(BorrowedFromModel newBorrowedFrom)
    {
      try
      {
        if (Session["BorrowedBooks"] != null)
        {
          List<BorrowedFromModel> borrowedBooksFromUser = (List<BorrowedFromModel>)Session["BorrowedBooks"];
          List<BorrowedFromModel> newBorrowedBooksFromUser = borrowedBooksFromUser.Where(x => x.Book.Id != newBorrowedFrom.Book.Id).ToList();
          Session["BorrowedBooks"] = newBorrowedBooksFromUser;
          if (newBorrowedBooksFromUser.Count < borrowedBooksFromUser.Count)
          {
            TempData["Message"] = "Book \"" + newBorrowedFrom.Book.Title + "\" successfully returned!";
          } else
          {
            TempData["Message"] = "Error: book \"" + newBorrowedFrom.Book.Title + "\" not found in borrowed book list!";
          } 
        }
        else
        {
          TempData["Message"] = "Error: book \"" + newBorrowedFrom.Book.Title + "\" is not successfully returned!";
        }
      }
      catch (Exception e)
      {
        TempData["Message"] = "Error happen: " + e.Message;
      }
      return RedirectToAction("List", "Book");
    }
  }
}