using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Sklep_z_towarami.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Category() { }
    }
}
