using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
namespace TheWall.Models
{
    public class Comment
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        public int CommentId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        [Required(ErrorMessage = "Cannot post an empty comment!")]
        [Display(Name = "Comment:")] 
        public string Content { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}