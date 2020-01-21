using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class Identification
    {
        [Key]
        public int IdentificationId { get; set; }

        [Required(ErrorMessage = "El Campo {0} es Requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Index("Identification_TipoDocumento_Company_Index", 1, IsUnique = true)]
        [Display(Name = "Compañia")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "El Campo {0} es Requerido")]
        [MaxLength(50, ErrorMessage = " El Campo {0} debe ser menor de {1} Caracteres")]
        [Index("Identification_TipoDocumento_Company_Index", 2, IsUnique = true)]
        [Display(Name = "Tipo Documento")]
        public string TipoDocumento { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Professional> Professionals { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}