using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Books.Models
{
  public class BorrowedFromModel
  {
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public string Tel { get; set; }
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Borrow_date { get; set; }
    public BookModel Book { get; set; }

    public BorrowedFromModel()
    {
      Book = new BookModel();
    }
  }
}