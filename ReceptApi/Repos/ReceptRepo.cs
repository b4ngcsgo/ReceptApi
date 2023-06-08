using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReceptApi.Models;
using System.Data.SqlClient;

namespace ReceptApi.Repos
{
    public class ReceptRepo
    {
        private readonly IConfiguration _config;
        public ReceptRepo(IConfiguration config)
        {
            _config = config;
        }

        public async Task SkapaRecept(Recept recept)
        {
            /*
             *  titel VARCHAR(255) NOT NULL,
    beskrivning TEXT NOT NULL,
    ingridienser TEXT NOT NULL,
    kategoriid INT NOT NULL,
             * 
             * */
            // Skapa recept
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteScalarAsync($"INSERT INTO recept VALUES ({recept.userid}, '{recept.titel}', '{recept.beskrivning}', '{recept.ingredienser}', {recept.kategoriid}, {0})");
        }
        public async Task UppdateraRecept(Recept recept)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var existingRecept = await connection.QuerySingleOrDefaultAsync<Recept>("SELECT * FROM recept WHERE receptid = @ReceptId", new { ReceptId = recept.receptid });

            if (existingRecept == null)
            {
                throw new ArgumentException($"Receptet med receptid {recept.receptid} finns inte.");
            }

            if (existingRecept.userid != recept.userid)
            {
                throw new ArgumentException($"Felaktigt användar-ID. Du har inte behörighet att uppdatera receptet med receptid {recept.receptid}.");
            }

            await connection.ExecuteAsync("UPDATE recept SET titel = @Titel, beskrivning = @Beskrivning, ingredienser = @Ingredienser, kategoriid = @KategoriId WHERE receptid = @ReceptId",
                new
                {
                    Titel = recept.titel,
                    Beskrivning = recept.beskrivning,
                    Ingredienser = recept.ingredienser,
                    KategoriId = recept.kategoriid,
                    ReceptId = recept.receptid
                });
        }

        public async Task TaBortRecept(int receptid, int userid)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var existingRecept = await connection.QuerySingleOrDefaultAsync<Recept>("SELECT * FROM recept WHERE receptid = @ReceptId", new { ReceptId = receptid });
            if (existingRecept == null)
            {
                throw new ArgumentException($"Receptet med receptid {receptid} finns inte.");
            }

            if (existingRecept.userid != userid)
            {
                throw new ArgumentException($"Du har inte behörighet att ta bort receptet med receptid {receptid}.");
            }

            await connection.ExecuteScalarAsync("DELETE FROM recept WHERE receptid = @ReceptId", new { ReceptId = receptid });
        }

        public async Task<IEnumerable<Recept>> HämtaAllaRecept()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var recept = await connection.QueryAsync<Recept>("SELECT * FROM recept");
            return recept;
        }
        public async Task BetygsattRecept(int receptId, int userId, int betyg)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            var recept = await connection.QuerySingleOrDefaultAsync<Recept>("SELECT * FROM recept WHERE receptid = @ReceptId AND userid = @UserId", new { ReceptId = receptId, UserId = userId });

            if (recept != null)
            {
                throw new ArgumentException($"Du kan inte betygssätta ditt eget recept med receptid {receptId}.");
            }

            if (betyg < 1 || betyg > 5)
            {
                throw new ArgumentException("Betyget måste vara mellan 1 och 5.");
            }

            var existingRating = await connection.QuerySingleOrDefaultAsync<int?>("SELECT betyg FROM betyg WHERE receptid = @ReceptId AND userid = @UserId", new { ReceptId = receptId, UserId = userId });

            if (existingRating != null)
            {
                throw new ArgumentException($"Du har redan betygssatt receptet med receptid {receptId}.");
            }
            await connection.ExecuteAsync("INSERT INTO betyg (receptid, userid, betyg) VALUES (@ReceptId, @UserId, @Betyg)", new { ReceptId = receptId, UserId = userId, Betyg = betyg });
        }
        public async Task<IEnumerable<Recept>> SearchRecept(string titel)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM recept WHERE titel LIKE @Sökterm";
            var parameters = new { Sökterm = "%" + titel + "%" };
            {
                return await connection.QueryAsync<Recept>(sql, parameters);
            }
        }


    }
}
