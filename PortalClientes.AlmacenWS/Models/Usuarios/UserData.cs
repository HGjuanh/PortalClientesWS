using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalClientes.AlmacenWS.Contexts;

namespace PortalClientes.AlmacenWS.Models {
    public class UserData {
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserDataID { get; set; }

        public string Name { get; set; }

        public string FatherLastName { get; set; }

        public string MotherLastName { get; set; }

        public byte[] Photo { get; set; }

        public string UserID { get; set; }

        public virtual IdentityUser User { get; set; }

        [NotMapped]
        public virtual string FullName { get { return $"{Name} {FatherLastName} {MotherLastName}"; } }
    }

    public class UserDataRegister {
        [Required(ErrorMessage = "Nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Apellido Paterno es obligatorio.")]
        [Display(Name = "Apellido Paterno")]
        public string FatherLastName { get; set; }

        [Display(Name = "Apellido Materno")]
        public string MotherLastName { get; set; }

        [Required(ErrorMessage = "Teléfono es obligatorio.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{2})\)?[-. ]?([0-9]{2})[-. ]?([0-9]{6})$", ErrorMessage = "El formato de Teléfono ingresado no es válido.")]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de Correo electrónico es inválido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Logo:")]
        public byte[] Logo { get; set; }

        [Display(Name = "Logo BN")]
        public byte[] LogoBco { get; set; }

        [Display(Name = "Modo Oscuro:")]
        public bool DarkMode { get; set; }

        [Display(Name = "Idioma:")]
        public string Language { get; set; }

        public bool HasShipments { get; set; }

        public bool HasQuotations { get; set; }

        public bool HasBillingAccounts { get; set; }

        public string DefaultCompanyID { get; set; }

        public bool Status { get; set; }
    }

    public static class UserDatas {
        public static UserData GetUserData(IdentityUser user) {
            UserData userData = new UserData {
                Name = "",
                FatherLastName = "",
                MotherLastName = "",
                User = user
            };

            using (var dbContext = new AppDbContext()) {
                userData = dbContext.UserDatas.Include(u => u.User).Where(u => u.UserID.Equals(user.Id)).FirstOrDefault();
            }

            return userData;
        }
    }
}
