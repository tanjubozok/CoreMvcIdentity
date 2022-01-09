using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class RoleModel
    {
        public string Id { get; set; }

        [Display(Name = "Rol Adı")]
        [Required(ErrorMessage = "Rol zorunlu alandır.")]
        public string Name { get; set; }
    }
}
