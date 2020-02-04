using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class ServiceCategory
    {
        [Key]
        public int ServiceCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("ServiceCategory_Company_Categoria_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "ServiceCategory_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("ServiceCategory_Company_Categoria_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "ServiceCategory_Model_Categoria")]
        public string Categoria { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "ServiceCateogory_Model_Detail")]
        [DataType(DataType.MultilineText)]
        public string Detalle { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Service> Services { get; set; }

        public virtual ICollection<DirectPayment> DirectPayments { get; set; }
    }
}