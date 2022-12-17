﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace QuasarCoreTimesheetsApp.Auth
{
    // Overriding the default Identity 
    public class UserContext : IdentityDbContext<User, Role, int>
    { 
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUserLogin<int>>(x => {
                x.Property(login => login.LoginProvider).HasMaxLength(128);
                x.Property(login => login.ProviderKey).HasMaxLength(128);
            });

            builder.Entity<IdentityUserToken<int>>(x =>
            {
                x.Property(token => token.LoginProvider).HasMaxLength(128);
            });
        }
    }
}
