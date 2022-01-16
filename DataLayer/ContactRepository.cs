using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using Dapper;

namespace DataLayer
{
    public class ContactRepository : IContactRepository
    {
        private IDbConnection _db;

        public ContactRepository(string connString)
        {
            this._db = new SqlConnection(connString);
        }

        public Contact Find(int id)
        {
            return this._db.Query<Contact>("SELECT * FROM CONTACTS WHERE id=@id", new {id}).FirstOrDefault();
        }

        public List<Contact> GetAll()
        {
            return _db.Query<Contact>("SELECT * FROM Contacts").ToList();
        }

        public Contact Add(Contact contact)
        {
            var sql =
                "INSERT INTO Contacts (FirstName, LastName, Email, Company, Title) VALUES(@FirstName, @LastName, @Email, @Company, @Title); " +
                "SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = this._db.Query<int>(sql, contact).Single();
            contact.Id = id;
            return contact;
        }

        public Contact Update(Contact contact)
        {
            var sql =
                "UPDATE contacts SET FirstName = @FirstName, LastName  = @LastName,Email= @Email,Company= @Company,Title= @Title WHERE id=@id";
            this._db.Execute(sql, contact);
            return contact;
        }

        public Address Add(Address Address)
        {
            var sql =
                "INSERT INTO Addresses (ContactId, AddressType, StreetAddress, City, StateId,PostalCode) VALUES(@ContactId, @AddressType, @StreetAddress, @City, @StateId,@PostalCode); " +
                "SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = this._db.Query<int>(sql, Address).Single();
            Address.Id = id;
            return Address;
        }

        public Address Update(Address Address)
        {
            var sql =
                "UPDATE Addresses SET AddressType = @AddressType, StreetAddress  = @StreetAddress,City= @City,StateId= @StateId,PostalCode= @PostalCode WHERE id=@id";
            this._db.Execute(sql, Address);
            return Address;
        }

        public void Remove(int id)
        {
            this._db.Execute("DELETE FROM contacts where @id=id", new {id});
        }

        public Contact GetFullContact(int id)
        {
            var sql = "select * from contacts where id=@id " +
                      "select * from Addresses where ContactId=@id";
            using (var multipleResult = this._db.QueryMultiple(sql, new {Id = id}))
            {
                var contact = multipleResult.Read<Contact>().SingleOrDefault();
                var address = multipleResult.Read<Address>().ToList();

                if (contact != null && address != null)
                {
                    contact.Addresses.AddRange(address);
                }

                return contact;
            }
        }

        public void Save(Contact contact)
        {
            using var txScope = new TransactionScope();
            if (contact.IsNew)
            {
                this.Add(contact);
            }
            else
            {
                this.Update(contact);
            }

            foreach (var addr in contact.Addresses.Where(a => !a.IsDeleted))
            {
                addr.ContactId = contact.Id;
                if (addr.IsNew)
                {
                    this.Add(addr);
                }
                else
                {
                    this.Update(addr);
                }
            }

            foreach (var addr in contact.Addresses.Where(a => a.IsDeleted))
            {
                this._db.Execute("DELETE FROM Addresses wher id=@id", new {addr.Id});
            }
            txScope.Complete();
        }
    }
}