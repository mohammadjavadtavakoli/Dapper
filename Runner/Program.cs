using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataLayer;
using Microsoft.Extensions.Configuration;

namespace Runner
{
    class Program
    {
        private static IConfiguration _config;

        private static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("app-settings.json", optional: true, reloadOnChange: true);
            _config = builder.Build();
        }

        private static IContactRepository CreateRepository()
        {
            // return new ContactRepository(_config.GetConnectionString("DefaultConnection"));
            return new ContactRepositoryContribe(_config.GetConnectionString("DefaultConnection"));
        }

        static void delete_shouId_remove_entity(int id)
        {
            //arrange
            var repository = CreateRepository();
            //act
            repository.Remove(id);

            var deletedEntity = repository.Find(id);
            Debug.Assert(deletedEntity==null);
            Console.WriteLine("*** Contact Deleted ***");

        }
        
        static void Modify_should_update_existing_entity(int id)
        {
            // arrange
            var repository = CreateRepository();

            // act
            var contact = repository.Find(id);
            contact.FirstName = "Bob";
            repository.Update(contact);

            // create a new repository for verification purposes
            var repository2 = CreateRepository();
            var modifiedContact = repository.Find(id);;

            // assert
            Console.WriteLine("*** Contact Modified ***");
            modifiedContact.Output();
            Debug.Assert(modifiedContact.FirstName == "Bob");
        }

        private static void find_shouId_retrieve_existing_entity(int id)
        {
            var repository = CreateRepository();

            var contact = repository.Find(id);

            Console.WriteLine("*** Get Contact ***");
            contact.Output();
            Debug.Assert(contact.FirstName == "Joe");
            Debug.Assert(contact.LastName == "Blow");
        }

        private static int Insert_ShouId_assign_identity_to_new_entity()
        {
            // arrange
            var repository = CreateRepository();

            var contact = new Contact
            {
                FirstName = "Joe",
                LastName = "Blow",
                Email = "joe.blow@gmail.com",
                Company = "Microsoft",
                Title = "Developer"
            };
            // var address = new Address
            // {
            //     AddressType = "Home",
            //     StreetAddress = "123 Main Street",
            //     City = "Baltimore",
            //     StateId = 1,
            //     PostalCode = "22222"
            // };
            //act
            repository.Add(contact);

            //asset
            Debug.Assert(contact.Id != 0);
            Console.WriteLine("*** Contact Inserted ***");
            Console.WriteLine($"New ID: {contact.Id}");

            return contact.Id;
        }

        private static void Get_All_ShouId_Return_6Results()
        {
            //arrange
            var repository = CreateRepository();

            //act
            var contacts = repository.GetAll();

            //assert
            Console.WriteLine($"Count: {contacts.Count}");
            Debug.Assert(contacts.Count == 6);
            contacts.Output();
        }

        static void Main(string[] args)
        {
            Initialize();

            Get_All_ShouId_Return_6Results();
            var id = Insert_ShouId_assign_identity_to_new_entity();
            find_shouId_retrieve_existing_entity(id);
            Modify_should_update_existing_entity(id);
            delete_shouId_remove_entity(id);
             
        }
    }
}