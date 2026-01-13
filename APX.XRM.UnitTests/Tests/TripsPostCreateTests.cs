using APX.Xrm;
using APX.XRM.Plugins;
using APX.XRM.UnitTests.Helper;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APX.XRM.UnitTests
{
    [TestClass]
    public class TripsTests : FakeXrmEasyTestsBase
    {
        [TestMethod]
        public void OnCreation_WithCorrectMessage_ShouldUpdateName()
        {           
            var testData = TestDataInitializer.CreateTestData();
            _context.Initialize(testData);
            var trip = (apx_Trips)testData.Where(d => d.LogicalName == apx_Trips.EntityLogicalName).FirstOrDefault();

            var pluginContext = _context.GetDefaultPluginContext();
            pluginContext.PostEntityImages = new EntityImageCollection() { new KeyValuePair<string, Entity>("PostImage", trip) };
            pluginContext.InputParameters = new ParameterCollection() { new KeyValuePair<string, object>("Target", trip) };
            pluginContext.MessageName = "Create";

            var plugin = new TripsPostCreate();
            _context.ExecutePluginWith(pluginContext, plugin);

            var tripUpdated = _context.CreateQuery<apx_Trips>().Where(t => t.Id == trip.Id).FirstOrDefault();
            Console.WriteLine($"Name: {tripUpdated.apx_Name}");
            Assert.IsNotNull(tripUpdated.apx_Name);
        }
        
        [TestMethod]
        public void OnCreation_WithWrongMessage_ShouldNotUpdateName()
        {        
            var testData = TestDataInitializer.CreateTestData();
            _context.Initialize(testData);
            var trip = (apx_Trips)testData.Where(d => d.LogicalName == apx_Trips.EntityLogicalName).FirstOrDefault();

            var pluginContext = _context.GetDefaultPluginContext();
            pluginContext.PostEntityImages = new EntityImageCollection() { new KeyValuePair<string, Entity>("PostImage", trip) };
            pluginContext.InputParameters = new ParameterCollection() { new KeyValuePair<string, object>("Target", trip) };
            pluginContext.MessageName = "Update";

            var plugin = new TripsPostCreate();
            _context.ExecutePluginWith(pluginContext, plugin);

            var tripUpdated = _context.CreateQuery<apx_Trips>().Where(t => t.Id == trip.Id).FirstOrDefault();
            Assert.IsNull(tripUpdated.apx_Name);
        }
    }
}
