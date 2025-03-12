using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Models
{
    public class Opiniones
    {
        [Key]
        public string UserId { get; set; }
        public string[]? Codigo { get; set; }
        public string[]? Calificacion { get; set; }
        public string[]? Comentario { get; set; }
        public string[]? Data1 { get; set; }
        public string[]? Data2 { get; set; }
        public string[]? Data3 { get; set; }
        public string[]? Data4 { get; set; }

        public Opiniones()
        {
            Codigo = new string[0];
            Calificacion = new string[0];
            Comentario = new string[0];
            Data1 = new string[0];
            Data2 = new string[0];
            Data3 = new string[0];
            Data4 = new string[0];
        }
    }
}
