using System.ComponentModel.DataAnnotations;

namespace FrontWally.DTOs.UsuarioDTOs
{
    public class UsuarioRegistroDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener mas de 100 caracteres")]

        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es valido")]
        [StringLength(100, ErrorMessage = "El email no puede tener mas de 100 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe " +
            "tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
    }
}
