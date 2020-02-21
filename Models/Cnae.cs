using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace utools.Models
{
    public class Cnae
    {
        [Key]
        public int Id { get; set; }
        public string code { get; set; }
        public string text { get; set; }
    }
}