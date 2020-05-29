using Everest_Video_Library.Models.VideoLibrary.BaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Everest_Video_Library.Models.VideoLibrary
{
    public class Category : ObjectProperty
    {
        [Required]
        [MinLength(20, ErrorMessage = "Description should be greater than 20.")]
        [MaxLength(500, ErrorMessage = "Description Should not be moore than 500.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Category Description")]
        public string Description { get; set; }
        [NotMapped]
        public HttpPostedFileBase CategoryImage { get; set; }
        [Display(Name = "Category Image")]
        [DataType(DataType.Upload)]
        public string ImageUrl { get; set; }
    }
}