namespace FrontWally.DTOs.ProductoDTOs
{
    public class ProductoDetailDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public decimal Precio { get; set; }
        public string Imagen { get; set; } = null!;
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;

        // Lista de IDs de cotizaciones asociadas
        public List<int> CotizacionIds { get; set; } = new();
    }
}
