﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace src.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Customer Attributes")]
    public partial class CustomerAttributesFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
#line 1 "CustomerAttributes.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Customer Attributes", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("CA002 Existing customer has no orders, (exists in silver.DimCustomerAttributes wi" +
            "th default values), now has new order")]
        public virtual void CA002ExistingCustomerHasNoOrdersExistsInSilver_DimCustomerAttributesWithDefaultValuesNowHasNewOrder()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("CA002 Existing customer has no orders, (exists in silver.DimCustomerAttributes wi" +
                    "th default values), now has new order", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 3
    this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
        testRunner.Given("the table \'DimCustomerAccountAttributes\' on the workbench is truncated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 5
        testRunner.And("the table \'DimCustomerAttributes\' on the workbench is truncated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 6
        testRunner.And("the table \'DimCustomerAccount\' on the workbench is truncated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                            "Comment",
                            "DimLatestCustomerAccountSKey [int]",
                            "FirstOrderReceiptID [int]",
                            "FirstOrderCustomerOrderReference [nvarchar]",
                            "FirstOrderDimBilledDateSKey [int]",
                            "FirstOrderDimShippedDateSKey [int]",
                            "FirstOrderDimCustomerAgeSKey [int]",
                            "FirstOrderCustomerAge [int]",
                            "FirstOrderDimAffiliateSKey [int]",
                            "FirstOrderDimDiscountSKey [int]",
                            "FirstOrderDimOrderCreatedDateSKey [int]",
                            "FirstOrderDimOrderCreatedTimeSKey [int]",
                            "Last24MonthsOrderCount [int]"});
                table1.AddRow(new string[] {
                            "No orders",
                            "1",
                            "-1",
                            "UNK",
                            "-1",
                            "-1",
                            "-1",
                            "0",
                            "-1",
                            "-1",
                            "-1",
                            "-1",
                            "0"});
#line 7
        testRunner.And("the table \'DimCustomerAttributes\' on the workbench contains the data:", ((string)(null)), table1, "And ");
#line hidden
                TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                            "Comment",
                            "DimCustomerAccountSKey [int]",
                            "FirstOrderReceiptID [int]",
                            "FirstOrderCustomerOrderReference [nvarchar]",
                            "FirstOrderDimOrderCreatedDateSKey [int]",
                            "FirstOrderDimOrderCreatedTimeSKey [int]",
                            "FirstOrderDimBilledDateSKey [int]",
                            "FirstOrderDimShippedDateSKey [int]",
                            "Last24MonthsOrderCount [int]"});
                table2.AddRow(new string[] {
                            "Existing Customer with 1 new order",
                            "1",
                            "1",
                            "Ref1",
                            "20190104",
                            "1",
                            "20190104",
                            "20190104",
                            "0"});
#line 10
        testRunner.And("the table \'DimCustomerAccountAttributes\' on the workbench contains the data:", ((string)(null)), table2, "And ");
#line hidden
                TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                            "Comment",
                            "DimCustomerAccountSKey [int]",
                            "DimLatestCustomerAccountSKey [int]"});
                table3.AddRow(new string[] {
                            "",
                            "1",
                            "1"});
#line 13
        testRunner.And("the table \'DimCustomerAccount\' on the workbench contains the data:", ((string)(null)), table3, "And ");
#line hidden
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "ParameterName",
                            "Value",
                            "Type"});
                table4.AddRow(new string[] {
                            "TriggerId",
                            "12345678-1234-1234-1234-123456789123",
                            "System.String"});
                table4.AddRow(new string[] {
                            "RunId",
                            "12345678-1234-1234-1234-123456789123",
                            "System.String"});
#line 16
        testRunner.When("the \'PopulateDimCustomerAttributes\' proc with params on workbench is executed:", ((string)(null)), table4, "When ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "Comment",
                            "DimLatestCustomerAccountSKey [int]",
                            "FirstOrderReceiptID [int]",
                            "FirstOrderCustomerOrderReference [nvarchar]",
                            "Last24MonthsOrderCount [int]"});
                table5.AddRow(new string[] {
                            "Updated record with 1 new order",
                            "1",
                            "1",
                            "Ref1",
                            "1"});
#line 20
        testRunner.Then("the table \'DimCustomerAttributes\' on the workbench should only contain the data w" +
                        "ithout strict ordering:", ((string)(null)), table5, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
