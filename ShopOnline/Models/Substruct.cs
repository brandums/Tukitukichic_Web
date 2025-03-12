using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Models
{
    public class Substruct
    {
        [Key]
        public string Date { get; set; }      // carrito de compras
        public string[]? Nombre { get; set; } // nombre
        public string[]? Precio { get; set; }  // Precio
        public string[]? BreveDescripcion { get; set; }
        public string[]? Descripcion { get; set; }
        public string[]? Codigo { get; set; }
        public string[][]? Color { get; set; }  // color
        public string[][]? Talla { get; set; }  // talla o peso valor
        public string[]? Categoria { get; set; }
        public string[]? SubCategoria { get; set; }
        public string[][]? Images { get; set; }
        public string[]? Extra1 { get; set; }  // cantidad
        public string[]? Extra2 { get; set; } // peso o talla string
        public string[]? Extra3 { get; set; } // estado visible u oculto
        public string[]? Extra4 { get; set; }
        public string[]? Extra5 { get; set; }
        public string[]? Extra6 { get; set; }
        public string[]? Extra7 { get; set; } // id usuario en compras /// precio oferta en substruct[0]  /// index del product like
        public string[]? Extra8 { get; set; } // nombre de usuario
        public string[]? TiempoOferta { get; set; }
        public string[]? Ventas { get; set; }
        public string[]? VentasBase { get; set; }
        public string[]? LikesBase { get; set; }


        public Substruct()
        {
            Date = DateTime.Now.Date.ToString();
            Nombre = new string[0];
            Precio = new string[0];
            BreveDescripcion = new string[0];
            Descripcion = new string[0];
            Codigo = new string[0];
            Color = new string[0][];
            Talla = new string[0][];
            Categoria = new string[0];
            SubCategoria = new string[0];
            Images = new string[0][];
            Extra1 = new string[0];
            Extra2 = new string[0];
            Extra3 = new string[0];
            Extra4 = new string[0];
            Extra5 = new string[0];
            Extra6 = new string[0];
            Extra7 = new string[0];
            Extra8 = new string[0];
            TiempoOferta = new string[0];
            Ventas = new string[0];
            VentasBase = new string[0];
            LikesBase = new string[0];
        }
    }
}
