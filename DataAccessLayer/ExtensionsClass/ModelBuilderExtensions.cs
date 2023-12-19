using DataTransferObject.Domain.Master;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ExtensionsClass
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            DateTime dt = new DateTime(2023,12,1,0,0,0);
            modelBuilder.Entity<MRegistration>().HasData(
                    new MRegistration
                    {
                        RegistrationId = 1,
                        Name = "Apply for Self (Officer)",
                        Order = 1,
                        ApplyForId = 1,
                        Updatedby = 1,
                        UpdatedOn = dt
                    },
                    new MRegistration
                    {
                        RegistrationId = 2,
                        Name = "Apply for Unit Officer",
                        Order = 2,
                        ApplyForId = 1,
                        Updatedby = 1,
                        UpdatedOn = dt
                    },
                    new MRegistration
                    {
                        RegistrationId = 3,
                        Name = "Apply for Other Unit Officer",
                        Order = 3,
                        ApplyForId = 1,
                        Updatedby = 1,
                        UpdatedOn = dt
                    },
                    new MRegistration
                    {
                        RegistrationId = 4,
                        Name = "Apply for Unit JCOs/OR",
                        Order = 4,
                        ApplyForId = 2,
                        Updatedby = 1,
                        UpdatedOn = dt
                    },
                    new MRegistration
                    {
                        RegistrationId = 5,
                        Name = "Apply for Other Unit JCOs/OR",
                        Order = 5,
                        ApplyForId = 2,
                        Updatedby = 1,
                        UpdatedOn = dt
                    }
                );
            modelBuilder.Entity<MICardType>().HasData(
                    new MICardType
                    {
                        TypeId = 1,
                        Name = "New I-Card Request"
                    },
                    new MICardType
                    {
                        TypeId = 2,
                        Name = "Loss of Card"
                    },
                    new MICardType
                    {
                        TypeId = 3,
                        Name = "Change of Appearance"
                    },
                    new MICardType
                    {
                        TypeId = 4,
                        Name = "Pramotion / Demotion"
                    },
                    new MICardType
                    {
                        TypeId = 5,
                        Name = "Fair Wear and Tear"
                    },
                    new MICardType
                    {
                        TypeId = 6,
                        Name = "Premature Damage"
                    },
                    new MICardType
                    {
                        TypeId = 7,
                        Name = "Loss of Token"
                    },
                    new MICardType
                    {
                        TypeId = 8,
                        Name = "Other"
                    }
                );
        }
    }
}
