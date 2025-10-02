using System.ComponentModel.DataAnnotations;

namespace FrontWally.DTOs.CotizacionDTOs
{
    public class CotizacionCreateDTO
    {
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El contacto es requerido")]
        public string Contacto { get; set; } = "";

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, 1000, ErrorMessage = "La cantidad debe ser entre 1 y 1000")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El total es requerido")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }
    }
}
