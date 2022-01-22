using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Rol Adı")]
        [Required(ErrorMessage = "Rol zorunlu alandır.")]
        public string Name { get; set; }
    }
}
