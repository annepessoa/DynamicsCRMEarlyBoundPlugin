using APX.Xrm;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace APX.XRM.Plugins
{
    public class TripsPostCreate : BasePlugin
    {
        public override string[] ExpectedMessages => new string[] { "Create" };
        public override string[] ExpectedEntities => new string[] { apx_Trips.EntityLogicalName };

        public override void ExecutePlugin(PluginContext ctx)
        {          
            var trip = (apx_Trips)ctx.PostImage;
            if (trip.apx_Name == null)
            {
                var location = ctx.Service.GetEntity<APX_Location>(trip.apx_Location, new ColumnSet(APX_Location.Fields.apx_Name));

                var tripToUpdate = new apx_Trips(trip.Id);
                tripToUpdate.apx_Name = $"Trip to {location?.apx_Name} on {trip?.apx_Start}";
                ctx.Service.Update(tripToUpdate);
            }
        }
    }
}
