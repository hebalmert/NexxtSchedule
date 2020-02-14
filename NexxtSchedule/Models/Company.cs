using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Company_Name_Index", IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Company_Rif_Index", IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Rif")]
        public string Rif { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Logo")]
        public string Logo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Country")]
        public int CountryId { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Logo")]
        public HttpPostedFileBase LogoFile { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Desde")]
        public DateTime DateDesde { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Hasta")]
        public DateTime DateHasta { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Precio")]
        public decimal Precio { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Active")]
        public bool Activo { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<Tax> Taxes { get; set; }

        public virtual ICollection<Identification> Identifications { get; set; }

        public virtual ICollection<HeadText> HeadTexts { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Professional> Professionals { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<ModelCalendar> ModelCalendars { get; set; }

        public virtual ICollection<ServiceCategory> ServiceCategories { get; set; }

        public virtual ICollection<Service> Services { get; set; }

        public virtual ICollection<Register> Registers { get; set; }

        public virtual ICollection<DirectPayment> DirectPayments { get; set; }

        public virtual ICollection<DirectGeneral> DirectGenerals { get; set; }

        public virtual ICollection<PayProfessional> PayProfessionals { get; set; }

        public virtual ICollection<PayProfessionalDetails> PayProfessionalDetails { get; set; }

        public virtual ICollection<Outcome> Outcomes { get; set; }
    }
}