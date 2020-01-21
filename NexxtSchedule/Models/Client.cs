using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Client_Cliente_IdentificationNumber_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Client_Cliente_IdentificationNumber_Company_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_Cliente")]
        public string Cliente { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_TipoDocu")]
        public int IdentificationId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Client_Cliente_IdentificationNumber_Company_Index", 3, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_NumeroDocu")]
        public string IdentificationNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(256, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("User_UserName_Index", IsUnique = true)]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_Movil")]
        public string Movil { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_Phone")]
        public string Phone { get; set; }

        [MaxLength(256, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Client_Model_Address")]
        public string Address { get; set; }

        public virtual Company Company { get; set; }

        public virtual Identification Identification { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<ModelCalendar> ModelCalendars { get; set; }
    }
}