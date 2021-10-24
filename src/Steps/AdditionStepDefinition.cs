using System;
using System.Linq;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace DataServices
{
    [Binding]
    public class AdditionStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private Database _db = new Database();

        public AdditionStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"the table 'DimCustomerAttributes' on the workbench is truncated")]
        public void GivenCustomerAttributesTruncated()
        {
            _db.Truncate("DimCustomerAttributes");
        }

        [Given(@"the table 'DimCustomerAccountAttributes' on the workbench is truncated")]
        public void GivenCustomerAccountAttributesTruncated()
        {
            _db.Truncate("DimCustomerAccountAttributes");
        }

        [Given(@"the table 'DimCustomerAccount' on the workbench is truncated")]
        public void GivenCustomerAccountTruncated()
        {
            _db.Truncate("DimCustomerAccount");
        }

        [Given(@"the table 'DimCustomerAttributes' on the workbench contains the data:")]
        public void GivenCustomerAttributesContainsTheData(TypedTable tableContents)
        {

            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAttributes", tableEnum);
        }

        [Given(@"the table 'DimCustomerAccountAttributes' on the workbench contains the data:")]
        public void GivenCustomerAccountAttributesContainsTheData(TypedTable tableContents)
        {

            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAccountAttributes", tableEnum);
        }

        [Given(@"the table 'DimCustomerAccount' on the workbench contains the data:")]
        public void GivenCustomerAccountContainsTheData(TypedTable tableContents)
        {

            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAccount", tableEnum);
        }

        [When(@"the 'PopulateDimCustomerAttributes' proc with params on workbench is executed:")]
        public void WhenTheProcWithParamsOnIsExecuted(Table parametersTable)
        {
            var parametersDictionary = parametersTable.Rows.ToDictionary(
                r => r["ParameterName"],
                r => Convert.ChangeType(r["Value"], Type.GetType(r["Type"], true))
            );
            _db.ExecProc("PopulateDimCustomerAttributes", parametersDictionary);
        }

        [Then(@"the table 'DimCustomerAttributes' on the workbench should only contain the data without strict ordering:")]
        public void ThenTheTableOnTheWorkbenchShouldOnlyContainTheDataNoOrdering(TypedTable expectedRows)
        {
            var expectedResults = expectedRows.ToDictionaries().ToList();
            expectedResults.Sort(DictionariesComparator.CompareTo);


            var actualResults = _db.ReadAll("DimCustomerAttributes")
                .Select(dic => dic
                    .Where(d => expectedRows.Header.Contains(d.Key))
                    .ToDictionary(d => d.Key, d => d.Value))
                .ToList();
            actualResults.Sort(DictionariesComparator.CompareTo);

            actualResults.Should().BeEquivalentTo(expectedResults
            );
        }

    }
}