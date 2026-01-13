using APX.Xrm;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APX.XRM.UnitTests.Helper
{
    public class TestDataInitializer
    {
        public static List<Entity> CreateTestData()
        {

            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = "Primary Contact ",
                LastName = "1",
                EmailAddress1 = "contact_1@email.com"

            };
            var location = new APX_Location()
            {
                Id = Guid.NewGuid(),
                apx_Name = "Dublin",
                apx_Contact = contact.ToEntityReference()
            };
            var trip = new apx_Trips()
            {
                Id = Guid.NewGuid(),
                apx_Location = location.ToEntityReference(),
                apx_Start = DateTime.Now.AddDays(-30),
                apx_End = DateTime.Now.AddDays(-20),
            };

            return new List<Entity>() { location, trip, contact };

        }
    }
}
