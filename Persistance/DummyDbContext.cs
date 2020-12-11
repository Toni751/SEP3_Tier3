using Microsoft.EntityFrameworkCore;
using SEP3_Tier3.Models;

namespace SEP3_T3.Persistance
{
    /// <summary>
    /// Dummy db context class used in unit testing for accessing the dummy database
    /// </summary>
    public class DummyDbContext : ShapeAppDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;port=3606;database=dummy_sep3;user=root;password=29312112");
        }
    }
}