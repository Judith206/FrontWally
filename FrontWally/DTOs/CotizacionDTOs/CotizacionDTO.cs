namespace FrontWally.DTOs.CotizacionDTOs
{
    public class CotizacionDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int Contacto { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = "";
        public decimal ProductoPrecio { get; set; }
    }
}
