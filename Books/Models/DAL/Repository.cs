using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;

namespace Books.Models.DAL
{
  public class Repository
  {
    public static DataSet ds = new DataSet("Book");
    //public static string pathToXML = HttpContext.Current.Server.MapPath("~/App_Data/books.xml");
    public static FileStream streamRead;

    #region Books
    public static List<BookModel> RetrieveBooks(string pathToXML)
    {
      List<BookModel> booksList = null;
      try
      {
        booksList = new List<BookModel>();
        streamRead = new FileStream(pathToXML, FileMode.Open);
        ds.ReadXml(streamRead);
        foreach (DataRow row in ds.Tables[0].Rows)
        {
          BookModel book = new BookModel
          {
            Id = row["id"].ToString(),
            Author = row["author"].ToString(),
            Title = row["title"].ToString(),
            Genre = row["genre"].ToString(),
            Price = Decimal.Parse(row["price"].ToString()),
            Publish_date = DateTime.Parse(row["publish_date"].ToString()),
            Description = row["description"].ToString(),
            Available = true
          };
          booksList.Add(book);
        }
      }
      catch (Exception e)
      {
        ds.Clear();
        streamRead.Close();
        streamRead.Dispose();
        throw e;
      }
      ds.Clear();
      streamRead.Close();
      streamRead.Dispose();
      return booksList;
    }

    public static BookModel RetrieveBook(string idBook, string pathToXML)
    {
      BookModel book = null;
      try
      {
        streamRead = new FileStream(pathToXML, FileMode.Open);
        ds.ReadXml(streamRead);
        DataRow row = (from DataRow r in ds.Tables[0].Rows where r["id"].ToString() == idBook select r).SingleOrDefault();
        book = new BookModel
        {
          Id = row["id"].ToString(),
          Author = row["author"].ToString(),
          Title = row["title"].ToString(),
          Genre = row["genre"].ToString(),
          Price = Decimal.Parse(row["price"].ToString().Replace('.', ',')),
          Publish_date = DateTime.Parse(row["publish_date"].ToString()),
          Description = row["description"].ToString(),
          Available = true
        };
      }
      catch (Exception e)
      {
        ds.Clear();
        streamRead.Close();
        streamRead.Dispose();
        throw e;
      }
      ds.Clear();
      streamRead.Close();
      streamRead.Dispose();
      return book;
    }

    #endregion
  }
}