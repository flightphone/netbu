using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
 
namespace netbu.Models
{
    public static class textHelper
    {
        public static HtmlString FromString(this IHtmlHelper html, string text)
        {
            return new HtmlString(text);
        }
    }
}