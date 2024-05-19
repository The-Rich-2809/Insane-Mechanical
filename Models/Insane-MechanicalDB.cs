﻿using Microsoft.EntityFrameworkCore;

namespace Insane_Mechanical.Models
{
    public class Insane_MechanicalDB : DbContext
    {
        public Insane_MechanicalDB(DbContextOptions<Insane_MechanicalDB> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuario { get; set; }
    }
}