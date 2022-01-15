using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;

namespace DataLayer
{
    public class ContactRepositoryContribe : IContactRepository
    {
        private IDbConnection _db;

        public ContactRepositoryContribe(string connString)
        {
            this._db = new SqlConnection(connString);
        }

        public Contact Find(int id)
        {
            return this._db.Get<Contact>(id);
        }

        public List<Contact> GetAll()
        {
            return this._db.GetAll<Contact>().ToList();
        }

        public Contact Add(Contact contact)
        {
            var id = this._db.Insert(contact);
            contact.Id = (int) id;
            return contact;
        }

        public Contact Update(Contact contact)
        {
            this._db.Update(contact);
            return contact;
        }

        public void Remove(int id)
        {
            this._db.Delete(new Contact {Id = id});
        }

        public Contact GetFullContact(int id)
        {
            throw new NotImplementedException();
        }
    }
}