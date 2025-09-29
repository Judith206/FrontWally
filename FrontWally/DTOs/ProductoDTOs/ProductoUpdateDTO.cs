using Microsoft.AspNetCore.Http;

namespace FrontWally.DTOs.ProductoDTOs
{
    public class ProductoUpdateDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public decimal Precio { get; set; }
        public byte[] Imagen { get; set; } = null!;

        // CORRECTO: IFormFile para recibir la imagen desde el formulario
        public IFormFile? ImagenFile { get; set; }
    }
}
