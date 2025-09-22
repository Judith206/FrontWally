using System.ComponentModel.DataAnnotations;

namespace FrontWally.DTOs.UsuarioDTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener mas de 100 caracteres")]

        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es valido")]
        [StringLength(100, ErrorMessage = "El email no puede tener mas de 100 caracteres")]

        public string Email { get; set; }

        public string Rol {  get; set; }
    }
}
