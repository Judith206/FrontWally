namespace FrontWally.DTOs.ProductoDTOs
{
    public class ProductoReadDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public decimal Precio { get; set; }
        public string Imagen { get; set; } = null!;
        public int IdUsuario { get; set; }
    }
}
