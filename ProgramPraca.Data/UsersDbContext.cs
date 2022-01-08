using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgramPraca.Domain;
namespace ProgramPraca.Data
    
{
    class UsersDbContext : DbContext
    {
        List<User> users = new();
    }
}
