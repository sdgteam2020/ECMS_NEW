using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer
{
    public static class AuthorizationPolicies
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApplFlaggedViewPolicy",
                    policy => policy.RequireClaim("Appl Flagged View"));

                options.AddPolicy("ApplFlaggedPolicy",
                    policy => policy.RequireClaim("Appl Flagged"));
                
                options.AddPolicy("AFSACDataExporterPolicy",
                    policy => policy.RequireClaim("AFSAC Data Exporter"));

                options.AddPolicy("CoordinatorPolicy",
                    policy => policy.RequireClaim("Coordinator"));

                options.AddPolicy("RO_ValidatorPolicy",
                    policy => policy.RequireClaim("RO_Validator"));

                options.AddPolicy("RO_ObserverPolicy",
                    policy => policy.RequireClaim("RO_Observer"));

                options.AddPolicy("DteAdminPolicy",
                    policy => policy.RequireClaim("Dte Admin"));

            });
        }
    }
}
