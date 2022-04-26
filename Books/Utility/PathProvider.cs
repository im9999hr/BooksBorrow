using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Books.Utility
{
  public interface IPathProvider
  {
    string MapPath(string path);
  }
  public class PathProvider : IPathProvider
  {
    public string MapPath(string path)
    {
      return HttpContext.Current.Server.MapPath(path);
    }
  }

  public class TestPathProvider : IPathProvider
  {
    public string MapPath(string path)
    {
      return Path.Combine("", path);
    }
  }

}