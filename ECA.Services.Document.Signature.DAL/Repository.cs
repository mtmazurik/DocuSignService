using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ECA.Services.Document.Signature.DAL
{
    public class Repository : IRepository
    {

        private string _connection;
        private RepositoryContext _context; 

        ~Repository()        // dtor
        {
            _context.Dispose();
        }
        public void InitializeContext(string connection)
        {
            _connection = connection;
            var builder = new DbContextOptionsBuilder<RepositoryContext>();
            builder.UseSqlServer(_connection);
            _context = new RepositoryContext(builder.Options);
        }
        public Models.Signature ReadSignature(int signatureId)
        {
            Models.Signature signature;
            using (_context)
            {
                 signature = _context.Signatures.Find(signatureId);
            }
            return signature;
        }
        public List<Models.Signature> ReadAllSignatures()
        {
            return new List<Models.Signature>(_context.Signatures.FromSql("Select * from Signature"));

        }
        public void CreateSignature(Models.Signature newSignature)
        {
            newSignature.UTCDateTimeCreated = DateTime.UtcNow;
            _context.Add(newSignature);
            _context.SaveChanges();
        }
        public void UpdateSignature(Models.Signature updatedSignature)
        {
            var result = _context.Signatures.Find(updatedSignature.SignatureId);
            if( result != null)
            {
                result.Status = updatedSignature.Status;
                result.UTCDateTimeLastUpdated = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
    public class RepositoryContext : DbContext
    {

        public RepositoryContext(DbContextOptions<RepositoryContext> options = null) : base(options)
        {
        }
        public virtual DbSet<Models.Signature> Signatures { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.Signature>()
                .HasKey(p => p.SignatureId)
                .HasName("Signature");
        }
    }

}
