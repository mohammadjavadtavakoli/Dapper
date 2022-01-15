using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
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

        public void Remove(int id)
        {
            this._db.Execute("DELETE FROM contacts where @id=id", new{ id });
        }
    }
}