using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class Outcome
    {
        [Key]
        public int OutcomeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Outcome_Egreso_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Outcome_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Outcome_Model_Date")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Outcome_Model_Egreso")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Outcome_Egreso_Company_Index", 2, IsUnique = true)]
        public string Egreso { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Outcome_Model_Beneficiario")]
        public string Beneficiario { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(256, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Outcome_Model_Detalle")]
        public string Detalle { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "OutComes_Model_Close")]
        public bool cerrado { get; set; }

        public Company Company { get; set; }

        public virtual ICollection<OutcomeDetail> OutcomeDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Display(ResourceType = typeof(Resource), Name = "OutComes_Model_TotalValue")]
        public decimal TotalValue { get { return OutcomeDetails == null ? 0 : OutcomeDetails.Sum(d => d.Precio); } }
    }
}