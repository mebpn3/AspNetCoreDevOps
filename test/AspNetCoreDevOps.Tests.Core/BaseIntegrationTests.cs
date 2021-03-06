﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using AspNetCoreDevOps.Api.Data;
using AspNetCoreDevOps.Seeder;

namespace AspNetCoreDevOps.Tests.Core
{
    public abstract class BaseIntegrationTests
    {
        protected ApplicationDbContext Context;

        public virtual void SetUp()
        {
            var helper = new Helper();
            var result = helper.GetContextAdnUserManager();
            Console.WriteLine("Deleting database");
            result.Database.EnsureDeleted();

            Context = result;


            Console.WriteLine("Applying Migrations");
            result.Database.Migrate();

            Console.WriteLine("Making sure database is created ");
            result.Database.EnsureCreated();

            Console.WriteLine("Going to save the data ");


            Data.CreateData(result);
            Console.WriteLine("Adding Data into database");
            result.SaveChanges();

            Console.WriteLine("Database successfully seeded");
            var totalTopic = result.People.ToList();
            Console.WriteLine($"Total People seeded is {totalTopic.Count()}");
        }

        [TearDown]
        public virtual void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}