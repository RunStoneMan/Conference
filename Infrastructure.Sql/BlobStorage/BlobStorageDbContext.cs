// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://go.microsoft.com/fwlink/p/?LinkID=258575
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

namespace Infrastructure.Sql.BlobStorage
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.EntityFrameworkCore.SqlServer;
    using System.Threading.Tasks;

    public class BlobStorageDbContext : DbContext
    {
        public const string SchemaName = "BlobStorage";

        public async Task<byte[]> Find(string id)
        {
            var blob = await this.Set<BlobEntity>().FindAsync(id);
            if (blob == null)
                return null;

            return blob.Blob; 
        }

        public async Task Save(string id, string contentType, byte[] blob)
        {
            var existing = await this.Set<BlobEntity>().FindAsync(id);
            string blobString = "";
            if (contentType == "text/plain")
            {
                Stream stream = null;
                try
                {
                    stream = new MemoryStream(blob);
                    using (var reader = new StreamReader(stream))
                    {
                        stream = null;
                        blobString = await reader.ReadToEndAsync();
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();
                }
            }

            if (existing != null)
            {
                existing.Blob = blob;
                existing.BlobString = blobString;
            }
            else
            {
                this.Set<BlobEntity>().Add(new BlobEntity(id, contentType, blob, blobString));
            }

            await this.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var blob =await this.Set<BlobEntity>().FindAsync(id);
            if (blob == null)
                return;

            this.Set<BlobEntity>().Remove(blob);

            await this.SaveChangesAsync();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlobEntity>()
                .ToTable("blogs", SchemaName);
        }

    }
}
