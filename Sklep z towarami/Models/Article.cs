using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sklep_z_towarami.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Za krótka nazwa")]
        [MaxLength(20, ErrorMessage = "Za długa nazwa")]
        public string Name { get; set; }
        public float Price { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        public string ImagePath { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public bool Promo { get; set; }
        public Category Category { get; set; }

        public Article(int id, string name, float price, string image_path, int category_id, bool promo)
        {
            Id = id;
            Name = name;
            Price = price;
            ImagePath = image_path;
            CategoryId = category_id;
            Promo = promo;
        }
        public Article(){}
    }
}
