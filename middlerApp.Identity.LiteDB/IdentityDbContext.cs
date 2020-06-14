using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LiteDB;
using middlerApp.Identity.Models.Models;

namespace middlerApp.Identity.LiteDB
{
    public class IdentityDbContext
    {
        private readonly LiteDatabase _database;

        public IdentityDbContext(string connectionString)
        {

            var con = new ConnectionString(connectionString);

            var fi = new FileInfo(con.Filename);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            _database = new LiteDatabase(connectionString);

        }

        protected LiteDatabase Database { get { return _database; } }
    }
}
