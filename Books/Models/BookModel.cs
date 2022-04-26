using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Books.Models
{
  public class BookModel
  {
    [Required]
    public string Id { get; set; }
    [Required]
    public string Author { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Genre { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Publish_date { get; set; }
    [Required]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }
    [Required]
    public bool Available { get; set; }
  }
}