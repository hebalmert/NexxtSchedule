using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class Register
    {
        [Key]
        public int RegisterId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Register_Model_BillingNote")]
        public int NotaCobro { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Register_Model_Expenses")]
        public int Egresos { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Register_Model_ComprobantePago")]
        public int Compobantepago { get; set; }

        public virtual Company Company { get; set; }
    }
}