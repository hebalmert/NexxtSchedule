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

        public void Dispose()
        {
            db.Dispose();
        }
    }
}