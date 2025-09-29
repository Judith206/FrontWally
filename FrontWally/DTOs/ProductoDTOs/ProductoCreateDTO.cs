using System.ComponentModel.DataAnnotations;

namespace FrontWally.DTOs.ProductoDTOs
{
    public class ProductoCreateDTO
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Estado { get; set; } 
        public decimal Precio { get; set; }

       
        public IFormFile? ImagenFile { get; set; }

        public byte[]? Imagen { get; set; }  // Esta es la que va al API

        public int UsuarioId { get; set; }
    }
}
