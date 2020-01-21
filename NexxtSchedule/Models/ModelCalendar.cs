using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class ModelCalendar
    {
        [Key]
        public int ModelCalendarId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Compania")]
        public int CompanyId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Profesional")]
        public int ProfessionalId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Cliente")]
        public int ClientId { get; set; }

        [Display(Name = "Cliente")]
        public string Cliente { get; set; }

        [Display(Name = "Profesional")]
        public string Profesional { get; set; }

        public virtual Company Company { get; set; }

        public virtual Professional Professional { get; set; }

        public virtual Client Client { get; set; }
    }
}