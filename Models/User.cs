using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
namespace TheWall.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        [Required(ErrorMessage = "First Name is required!")]
        [MinLength(2, ErrorMessage="First Name must be at least 2 characters!")]
        [Display(Name = "First Name:")] 
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required!")]
        [MinLength(2, ErrorMessage="Last Name must be at least 2 characters!")]
        [Display(Name = "Last Name:")] 
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required!")]
        [Display(Name = "Email:")] 
        [EmailAddress]
        public string Email { get; set; }
        //[DataType(DataType.Password)]
        [Display(Name = "Password:")] 
        [Required(ErrorMessage = "Password is required!")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password { get; set; }
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        [NotMapped]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        [Display(Name = "Confirm Password:")] 
        //[DataType(DataType.Password)]
        public string Confirm {get;set;}
        public List<Message> CreatedMessages {get;set;}
        public List<Comment> CreatedComments {get;set;}
    }
}