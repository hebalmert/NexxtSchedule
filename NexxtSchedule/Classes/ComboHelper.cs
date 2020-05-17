using NexxtSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Classes
{
    public class ComboHelper : IDisposable
    {
        private static NexxtCalContext db = new NexxtCalContext();

        //Combos de State
        public static List<Country> GetCountries()
        {
            var paises = db.Countries.ToList();
            paises.Add(new Country
            {
                CountryId = 0,
                Pais = @Resources.Resource.ComboSelect,
            });
            return paises.OrderBy(o=> o.Pais).ToList();
        }

        //Combos de Compañias
        public static List<Company> GetCompanies()
        {
            var companies = db.Companies.ToList();
            companies.Add(new Company
            {
                CompanyId = 0,
                Name = @Resources.Resource.ComboSelect,
            });
            return companies.OrderBy(d => d.Name).ToList();
        }

        //Combos de Identification
        public static List<Identification> GetIdentifications(int companyid)
        {
            var identification = db.Identifications.Where(c => c.CompanyId == companyid).ToList();
            identification.Add(new Identification
            {
                IdentificationId = 0,
                TipoDocumento = @Resources.Resource.ComboSelect,
            });
            return identification.OrderBy(d => d.TipoDocumento).ToList();
        }

        //Combos de Identification
        public static List<Client> GetClients(int companyid)
        {
            var clientes = db.Clients.Where(c => c.CompanyId == companyid).ToList();
            clientes.Add(new Client
            {
                ClientId = 0,
                Cliente = @Resources.Resource.ComboSelect,
            });
            return clientes.OrderBy(d => d.Cliente).ToList();
        }

        //Combos de Identification
        public static List<Professional> GetProfessional(int companyid)
        {
            var profesional = db.Professionals.Where(c => c.CompanyId == companyid).ToList();
            profesional.Add(new Professional
            {
                ProfessionalId = 0,
                FullName = @Resources.Resource.ComboSelect,
            });
            return profesional.OrderBy(d => d.FullName).ToList();
        }

        //Combos de categoria Servicios
        public static List<ServiceCategory> GetServicecategories(int companyid)
        {
            var categoryserivces = db.ServiceCategories.Where(c => c.CompanyId == companyid).ToList();
            categoryserivces.Add(new ServiceCategory
            {
                ServiceCategoryId = 0,
                Categoria = @Resources.Resource.ComboSelect,
            });
            return categoryserivces.OrderBy(d => d.Categoria).ToList();
        }

        //Combos de categoria Servicios
        public static List<Service> GetServices(int companyid)
        {
            var services = db.Services.Where(c => c.CompanyId == companyid).ToList();
            services.Add(new Service
            {
                ServiceId = 0,
                Servicio = @Resources.Resource.ComboSelect,
            });
            return services.OrderBy(d => d.Servicio).ToList();
        }

        //Combos de impuestos
        public static List<Tax> GetTaxes(int companyid)
        {
            var taxes = db.Taxes.Where(c => c.CompanyId == companyid).ToList();
            taxes.Add(new Tax
            {
                TaxId = 0,
                Impuesto = @Resources.Resource.ComboSelect,
            });
            return taxes.OrderBy(d => d.Impuesto).ToList();
        }

        //Combos de impuestos
        public static List<LevelPrice> GetLevelPrice()
        {
            var levelprice = db.LevelPrices.ToList();
            levelprice.Add(new LevelPrice
            {
                LevelPriceId = 0,
                NivelPrecio = @Resources.Resource.ComboSelect,
            });
            return levelprice.OrderBy(d => d.NivelPrecio).ToList();
        }

        //Combos de impuestos
        public static List<Color> GetColor()
        {
            var colores = db.Colors.ToList();
            colores.Add(new Color
            {
                ColorId = 0,
                ColorDate = @Resources.Resource.ComboSelect,
            });
            return colores.OrderBy(d => d.ColorDate).ToList();
        }

        //Combos de impuestos
        public static List<Hour> GetHora()
        {
            var horas = db.Hours.ToList();
            horas.Add(new Hour
            {
                HourId = 0,
                Hora = @Resources.Resource.ComboSelect,
            });
            return horas.OrderBy(d => d.Hora).ToList();
        }

        public static TimeZoneInfo GetTimeZone(int idcompany)
        {
            var pais = db.Companies.Find(idcompany);

            if (pais.CountryId == 2)
            {
                TimeZoneInfo tz = TimeZoneInfo.CreateCustomTimeZone("COLOMBIA", new TimeSpan(-2, 0, 0), "Colombia", "Colombia");
                return tz;
            }
           else
            {
                //pais.countryId == 1 es Venezuela
                TimeZoneInfo tz = TimeZoneInfo.CreateCustomTimeZone("VENEZUELA", new TimeSpan(-3, 0, 0), "Venezuela", "Venezuela");
                return tz;
            }
        }



        public void Dispose()
        {
            db.Dispose();
        }
    }
}