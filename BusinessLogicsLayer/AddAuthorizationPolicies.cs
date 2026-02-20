using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicsLayer
{
    public static class AuthorizationPolicies
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewFlaggedICardApplPolicy",
                    policy => policy.RequireClaim("View Flagged ICard Appl: Disp Case"));

                options.AddPolicy("FlagICardApplPolicy",
                    policy => policy.RequireClaim("Flag ICard Appl: Disp Case"));
                
                options.AddPolicy("ICardExportDataPolicy",
                    policy => policy.RequireClaim("ICard Export Data: AFSAC Cell"));

                options.AddPolicy("InternalWkDistrPolicy",
                    policy => policy.RequireClaim("Internal Wk Distr: Record Office"));

                options.AddPolicy("ApplApproverPolicy",
                    policy => policy.RequireClaim("Appl Approver"));

                options.AddPolicy("ViewIndlIncorrectDataPolicy",
                    policy => policy.RequireClaim("View Indl Incorrect Data: Record Office"));

                options.AddPolicy("ArmyLevelReportsPolicy",
                    policy => policy.RequireClaim("Army Level Reports"));
                
                options.AddPolicy("FmnLevelReportsPolicy",
                    policy => policy.RequireClaim("Fmn Level Reports"));

                options.AddPolicy("ICardDispatchPolicy", policy =>
                {
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "Dispatch Card") ||
                        context.User.HasClaim(c => c.Type == "ICard Export Data: AFSAC Cell") ||
                        (context.User.HasClaim(c => c.Type == "Dispatch Card") && context.User.HasClaim(c => c.Type == "Appl Approver"))
                    );
                });
                options.AddPolicy("ddlRecordRegimentPolicy", policy =>
                {
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "Dispatch Card") ||
                        (context.User.HasClaim(c => c.Type == "Dispatch Card") && context.User.HasClaim(c => c.Type == "Appl Approver"))
                    );
                });

            });
        }
    }
}
