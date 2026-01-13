using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using Microsoft.Xrm.Sdk;

namespace APX.XRM.UnitTests
{
    public class FakeXrmEasyTestsBase
    {
        protected readonly IXrmFakedContext _context;
        protected readonly IOrganizationService _service;

        public FakeXrmEasyTestsBase()
        {
            _context = MiddlewareBuilder
                            .New()
                            .AddCrud()
                            .AddFakeMessageExecutors()
                            .UseCrud()
                            .UseMessages()
                            .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                            .Build();
            _service = _context.GetOrganizationService();
        }
    }
}
