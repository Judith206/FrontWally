namespace FrontWally.DTOs.CotizacionDTOs
{
    public class CotizacionCreateDTO
    {
        public DateTime Fecha { get; set; }
        public int Contacto { get; set; }
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
    }
}
