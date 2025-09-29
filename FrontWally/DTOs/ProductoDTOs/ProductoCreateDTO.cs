using System.ComponentModel.DataAnnotations;

namespace FrontWally.DTOs.ProductoDTOs
{
    public class ProductoCreateDTO
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public decimal Precio { get; set; }

        // Cambio: usar IFormFile para recibir la imagen desde la vista
        [Required(ErrorMessage = "El campo Imagen es obligatorio.")]
        public IFormFile? ImagenFile { get; set; }

        public byte[] Imagen { get; set; } = null!; // Esta es la que va al API

        public int UsuarioId { get; set; }
    }
}
